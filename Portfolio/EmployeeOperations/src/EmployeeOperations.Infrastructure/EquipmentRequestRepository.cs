using EmployeeOperations.Application;
using EmployeeOperations.Domain;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOperations.Infrastructure;

public sealed class EquipmentRequestRepository(EmployeeOperationsDbContext db) : IEquipmentRequestRepository
{
    private EquipmentRequest? _loaded;
    public async Task AddAsync(EquipmentRequest request, CancellationToken ct) { _loaded = request; await db.EquipmentRequests.AddAsync(request, ct); }
    public async Task<EquipmentRequest?> FindAsync(Guid id, CancellationToken ct) =>
        _loaded = await db.EquipmentRequests.Include(x => x.Transitions).SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task SaveChangesAsync(long expectedVersion, CancellationToken ct)
    {
        if (_loaded is not null && db.Entry(_loaded).State != EntityState.Added)
            db.Entry(_loaded).Property(x => x.Version).OriginalValue = expectedVersion;
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException) { throw new RequestConcurrencyException(_loaded?.Id ?? Guid.Empty); }
    }
}
