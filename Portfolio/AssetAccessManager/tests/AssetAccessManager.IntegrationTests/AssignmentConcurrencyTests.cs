using AssetAccessManager.Application;
using AssetAccessManager.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
namespace AssetAccessManager.IntegrationTests;

[TestClass]
public sealed class AssignmentConcurrencyTests
{
    [TestMethod]
    public async Task Simultaneous_administrators_produce_one_assignment_and_clear_conflicts()
    {
        const int attempts = 32; var store = new AtomicStore(); var service = new AssetService(store, TimeProvider.System); var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var tasks = Enumerable.Range(0, attempts).Select(async i => { await gate.Task; try { await service.AssignAsync(Guid.Parse("10000000-0000-0000-0000-000000000001"), new($"employee-{i}"), $"admin-{i}", default); return true; } catch (AssetAlreadyAssignedException) { return false; } }).ToArray(); gate.SetResult(); var results = await Task.WhenAll(tasks);
        Assert.AreEqual(1, results.Count(x => x)); Assert.AreEqual(attempts - 1, results.Count(x => !x)); Assert.AreEqual(1, store.SideEffects);
    }
    [TestMethod]
    public void Sql_model_has_concurrency_token_and_one_active_assignment_constraint()
    {
        using var db = new AssetAccessDbContext(new DbContextOptionsBuilder<AssetAccessDbContext>().UseSqlServer("Server=unused").Options); var asset = db.Model.FindEntityType(typeof(AssetAccessManager.Domain.Asset))!; Assert.IsTrue(asset.FindProperty("Version")!.IsConcurrencyToken); var assignment = db.Model.FindEntityType(typeof(AssetAccessManager.Domain.AssetAssignment))!; var index = assignment.GetIndexes().Single(x => x.Properties.Single().Name == "AssetId"); Assert.IsTrue(index.IsUnique); Assert.AreEqual("[ReturnedAt] IS NULL", index.GetFilter());
    }
    private sealed class AtomicStore : IAssetStore
    {
        private int _assigned; public int SideEffects;
        public Task<IReadOnlyList<AssetListItem>> SearchAsync(string? q, int p, int ps, CancellationToken ct) => Task.FromResult<IReadOnlyList<AssetListItem>>([]);
        public Task<AssignmentReceipt?> TryAssignAsync(Guid id, string employee, string actor, DateTimeOffset now, CancellationToken ct) { if (Interlocked.CompareExchange(ref _assigned, 1, 0) != 0) return Task.FromResult<AssignmentReceipt?>(null); Interlocked.Increment(ref SideEffects); return Task.FromResult<AssignmentReceipt?>(new(Guid.NewGuid(), id, employee, actor, now)); }
        public Task<bool> ReturnAsync(Guid id, string actor, DateTimeOffset now, CancellationToken ct) => Task.FromResult(false);
    }
}
