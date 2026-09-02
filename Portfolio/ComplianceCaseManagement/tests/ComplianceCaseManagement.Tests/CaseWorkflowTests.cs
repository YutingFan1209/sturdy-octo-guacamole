using ComplianceCaseManagement.Cases;
using ComplianceCaseManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
namespace ComplianceCaseManagement.Tests;

[TestClass]
public sealed class CaseWorkflowTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 2, 0, 0, 0, TimeSpan.Zero);
    [TestMethod] public void Investigation_resolution_and_close_preserve_audit() { var c = ComplianceCase.Open("Review payment", "analyst-1", Now.AddDays(1), Now); c.BeginInvestigation("analyst-1", Now.AddHours(1)); c.Resolve("analyst-2", "False positive", Now.AddHours(2)); c.Close("supervisor-1", "Reviewed", Now.AddHours(3)); Assert.AreEqual(CaseStatus.Closed, c.Status); Assert.HasCount(4, c.Audit); CollectionAssert.AreEqual(new[] { "Created", "Investigation started", "False positive", "Reviewed" }, c.Audit.Select(x => x.Action).ToArray()); }
    [TestMethod] public void Invalid_transition_changes_neither_state_nor_audit() { var c = ComplianceCase.Open("Review", "analyst", Now.AddDays(1), Now); var count = c.Audit.Count; Assert.ThrowsExactly<InvalidOperationException>(() => c.Resolve("analyst", "No evidence", Now)); Assert.AreEqual(CaseStatus.Open, c.Status); Assert.HasCount(count, c.Audit); }
    [TestMethod]
    public async Task Two_analysts_and_worker_allow_one_winner_without_lost_audit()
    {
        var source = ComplianceCase.Open("Overdue review", "owner", Now.AddMinutes(-1), Now.AddDays(-2)); var attempts = new[] { Clone(source), Clone(source), Clone(source) }; var version = 0; ComplianceCase? persisted = null; var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var tasks = attempts.Select(async (c, i) => { await gate.Task; if (i < 2) c.BeginInvestigation($"analyst-{i + 1}", Now); else c.EscalateIfOverdue("system:deadline-worker", Now); if (Interlocked.CompareExchange(ref version, 1, 0) == 0) { persisted = c; return true; } return false; }).ToArray(); gate.SetResult(); var results = await Task.WhenAll(tasks);
        Assert.AreEqual(1, results.Count(x => x)); Assert.HasCount(2, persisted!.Audit); Assert.AreEqual(1, persisted.Version); Assert.IsTrue(persisted.Status is CaseStatus.UnderInvestigation or CaseStatus.Escalated);
    }
    [TestMethod] public void Ef_model_protects_version_and_audit_rows() { using var db = new CaseDbContext(new DbContextOptionsBuilder<CaseDbContext>().UseSqlServer("Server=unused").Options); var entity = db.Model.FindEntityType(typeof(ComplianceCase))!; Assert.IsTrue(entity.FindProperty("Version")!.IsConcurrencyToken); Assert.AreEqual(DeleteBehavior.Restrict, entity.GetForeignKeys().SingleOrDefault()?.DeleteBehavior ?? db.Model.FindEntityType(typeof(CaseAudit))!.GetForeignKeys().Single().DeleteBehavior); }
    private static ComplianceCase Clone(ComplianceCase source) { var c = ComplianceCase.Open(source.Title, source.OwnerId, source.Deadline, source.Audit.Single().OccurredAt); return c; }
}
