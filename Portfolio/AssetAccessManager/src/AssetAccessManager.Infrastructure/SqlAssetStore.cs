using AssetAccessManager.Application;
using AssetAccessManager.Domain;
using Microsoft.EntityFrameworkCore;
namespace AssetAccessManager.Infrastructure;

public sealed class SqlAssetStore(AssetAccessDbContext db) : IAssetStore
{
    public async Task<IReadOnlyList<AssetListItem>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    { var q = db.Assets.AsNoTracking(); if (!string.IsNullOrWhiteSpace(query)) { var term = query.Trim(); q = q.Where(x => x.AssetTag.Contains(term) || x.SerialNumber.Contains(term) || x.Model.Contains(term)); } return await q.OrderBy(x => x.AssetTag).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new AssetListItem(x.Id, x.AssetTag, x.SerialNumber, x.Model, x.Status, x.Version)).ToListAsync(ct); }
    public async Task<AssignmentReceipt?> TryAssignAsync(Guid assetId, string employeeId, string actor, DateTimeOffset now, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var affected = await db.Assets.Where(x => x.Id == assetId && x.Status == AssetStatus.Available).ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, AssetStatus.Assigned).SetProperty(x => x.Version, x => x.Version + 1), ct);
        if (affected != 1) { await tx.RollbackAsync(ct); return null; }
        var assignment = new AssetAssignment(Guid.NewGuid(), assetId, employeeId, actor, now); db.Assignments.Add(assignment);
        try { await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return new(assignment.Id, assetId, employeeId, actor, now); }
        catch (DbUpdateException) { await tx.RollbackAsync(ct); return null; }
    }
    public async Task<bool> ReturnAsync(Guid assignmentId, string actor, DateTimeOffset now, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct); var assignment = await db.Assignments.SingleOrDefaultAsync(x => x.Id == assignmentId && x.ReturnedAt == null, ct); if (assignment is null) return false; assignment.Return(actor, now);
        var changed = await db.Assets.Where(x => x.Id == assignment.AssetId && x.Status == AssetStatus.Assigned).ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, AssetStatus.Available).SetProperty(x => x.Version, x => x.Version + 1), ct); if (changed != 1) return false; await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return true;
    }
}
