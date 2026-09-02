using EmployeeOperations.Domain;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOperations.Infrastructure;

public sealed class EmployeeOperationsDbContext(DbContextOptions<EmployeeOperationsDbContext> options) : DbContext(options)
{
    public DbSet<EquipmentRequest> EquipmentRequests => Set<EquipmentRequest>();
    public DbSet<RequestTransition> RequestTransitions => Set<RequestTransition>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var request = modelBuilder.Entity<EquipmentRequest>();
        request.ToTable("EquipmentRequests"); request.HasKey(x => x.Id);
        request.Property(x => x.EmployeeId).HasMaxLength(100).IsRequired();
        request.Property(x => x.Item).HasMaxLength(100).IsRequired();
        request.Property(x => x.Justification).HasMaxLength(1000).IsRequired();
        request.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        request.Property(x => x.Version).IsConcurrencyToken();
        request.HasMany(x => x.Transitions).WithOne().HasForeignKey(x => x.RequestId).OnDelete(DeleteBehavior.Restrict);
        request.Navigation(x => x.Transitions).UsePropertyAccessMode(PropertyAccessMode.Field);
        request.HasIndex(x => new { x.EmployeeId, x.Status, x.UpdatedAt });
        var transition = modelBuilder.Entity<RequestTransition>();
        transition.ToTable("RequestTransitions"); transition.HasKey(x => x.Id);
        transition.Property(x => x.PreviousStatus).HasConversion<string>().HasMaxLength(20);
        transition.Property(x => x.NewStatus).HasConversion<string>().HasMaxLength(20);
        transition.Property(x => x.ActorId).HasMaxLength(100).IsRequired();
        transition.Property(x => x.Reason).HasMaxLength(500);
        transition.HasIndex(x => new { x.RequestId, x.OccurredAt });
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (ChangeTracker.Entries<RequestTransition>().Any(e => e.State is EntityState.Modified or EntityState.Deleted))
            throw new InvalidOperationException("Request transition history is append-only.");
        return base.SaveChangesAsync(cancellationToken);
    }
}
