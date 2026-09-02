using System.ComponentModel.DataAnnotations;
using AssetAccessManager.Domain;
namespace AssetAccessManager.Application;

public sealed record AssignAsset([Required, StringLength(100)] string EmployeeId);
public sealed record AssetListItem(Guid Id, string AssetTag, string SerialNumber, string Model, AssetStatus Status, long Version);
public sealed record AssignmentReceipt(Guid AssignmentId, Guid AssetId, string EmployeeId, string AssignedBy, DateTimeOffset AssignedAt);
public sealed class AssetAlreadyAssignedException(Guid id) : Exception($"Asset {id} was assigned by another administrator. Refresh the inventory and choose another asset.");
public interface IAssetStore
{
    Task<IReadOnlyList<AssetListItem>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
    Task<AssignmentReceipt?> TryAssignAsync(Guid assetId, string employeeId, string actor, DateTimeOffset now, CancellationToken ct);
    Task<bool> ReturnAsync(Guid assignmentId, string actor, DateTimeOffset now, CancellationToken ct);
}
public sealed class AssetService(IAssetStore store, TimeProvider clock)
{
    public Task<IReadOnlyList<AssetListItem>> SearchAsync(string? query, int page, CancellationToken ct) => store.SearchAsync(query, Math.Max(1, page), 25, ct);
    public async Task<AssignmentReceipt> AssignAsync(Guid id, AssignAsset input, string actor, CancellationToken ct) =>
        await store.TryAssignAsync(id, input.EmployeeId.Trim(), actor, clock.GetUtcNow(), ct) ?? throw new AssetAlreadyAssignedException(id);
    public async Task ReturnAsync(Guid assignmentId, string actor, CancellationToken ct)
    { if (!await store.ReturnAsync(assignmentId, actor, clock.GetUtcNow(), ct)) throw new KeyNotFoundException("Active assignment was not found."); }
}
