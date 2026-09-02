using AssetAccessManager.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace AssetAccessManager.Infrastructure;

public sealed class AssetAccessDbContext(DbContextOptions<AssetAccessDbContext> options) : DbContext(options)
{
    public DbSet<Asset> Assets => Set<Asset>(); public DbSet<AssetAssignment> Assignments => Set<AssetAssignment>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        var a = b.Entity<Asset>(); a.HasKey(x => x.Id); a.Property(x => x.AssetTag).HasMaxLength(40).IsRequired(); a.HasIndex(x => x.AssetTag).IsUnique(); a.Property(x => x.SerialNumber).HasMaxLength(100).IsRequired(); a.HasIndex(x => x.SerialNumber).IsUnique(); a.Property(x => x.Model).HasMaxLength(100).IsRequired(); a.Property(x => x.Status).HasConversion<string>().HasMaxLength(20); a.Property(x => x.Version).IsConcurrencyToken(); a.HasIndex(x => new { x.Status, x.AssetTag });
        var assignment = b.Entity<AssetAssignment>(); assignment.HasKey(x => x.Id); assignment.Property(x => x.EmployeeId).HasMaxLength(100).IsRequired(); assignment.Property(x => x.AssignedBy).HasMaxLength(100).IsRequired(); assignment.Property(x => x.ReturnedBy).HasMaxLength(100); assignment.HasOne<Asset>().WithMany().HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.Restrict); assignment.HasIndex(x => x.AssetId).IsUnique().HasFilter("[ReturnedAt] IS NULL");
        a.HasData(Asset.RegisterSeed(new Guid("10000000-0000-0000-0000-000000000001"), "LT-1001", "SN-APPLE-001", "MacBook Pro 14"), Asset.RegisterSeed(new Guid("10000000-0000-0000-0000-000000000002"), "LT-1002", "SN-DELL-002", "Dell Latitude 7450"), Asset.RegisterSeed(new Guid("10000000-0000-0000-0000-000000000003"), "LT-1003", "SN-LENOVO-003", "ThinkPad T14"));
    }
}
public sealed class AssetAccessDbContextFactory : IDesignTimeDbContextFactory<AssetAccessDbContext> { public AssetAccessDbContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<AssetAccessDbContext>().UseSqlServer("Server=localhost;Database=AssetAccessManager;Trusted_Connection=True;TrustServerCertificate=True").Options); }
