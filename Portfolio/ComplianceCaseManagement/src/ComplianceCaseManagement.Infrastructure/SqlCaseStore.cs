using ComplianceCaseManagement.Cases;
using Microsoft.EntityFrameworkCore;
namespace ComplianceCaseManagement.Infrastructure;

public sealed class SqlCaseStore(CaseDbContext db) : ICaseStore
{
    private ComplianceCase? loaded; public async Task AddAsync(ComplianceCase item, CancellationToken ct) { loaded = item; await db.Cases.AddAsync(item, ct); }
    public async Task<ComplianceCase?> FindAsync(Guid id, CancellationToken ct) => loaded = await db.Cases.Include(x => x.Audit).SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task SaveAsync(long expected, CancellationToken ct) { if (loaded is not null && db.Entry(loaded).State != EntityState.Added) db.Entry(loaded).Property(x => x.Version).OriginalValue = expected; try { await db.SaveChangesAsync(ct); } catch (DbUpdateConcurrencyException) { throw new CaseConflictException(loaded?.Id ?? Guid.Empty); } }
    public async Task<int> EscalateOverdueAsync(DateTimeOffset now, string actor, CancellationToken ct) { var ids = await db.Cases.AsNoTracking().Where(x => x.Deadline <= now && (x.Status == CaseStatus.Open || x.Status == CaseStatus.UnderInvestigation)).Select(x => x.Id).ToListAsync(ct); var count = 0; foreach (var id in ids) { db.ChangeTracker.Clear(); var item = await db.Cases.Include(x => x.Audit).SingleAsync(x => x.Id == id, ct); var version = item.Version; if (!item.EscalateIfOverdue(actor, now)) continue; db.Entry(item).Property(x => x.Version).OriginalValue = version; try { await db.SaveChangesAsync(ct); count++; } catch (DbUpdateConcurrencyException) { db.ChangeTracker.Clear(); } } return count; }
}
