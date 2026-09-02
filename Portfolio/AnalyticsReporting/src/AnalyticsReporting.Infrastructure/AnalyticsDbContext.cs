using AnalyticsReporting.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace AnalyticsReporting.Infrastructure;

public sealed class AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : DbContext(options) { public DbSet<ImportJob> Jobs => Set<ImportJob>(); public DbSet<OperationalRecord> Records => Set<OperationalRecord>(); protected override void OnModelCreating(ModelBuilder b) { var j = b.Entity<ImportJob>(); j.HasKey(x => x.Id); j.Property(x => x.FileName).HasMaxLength(255); j.Property(x => x.StorageKey).HasMaxLength(500); j.Property(x => x.Status).HasConversion<string>().HasMaxLength(20); j.Property(x => x.Error).HasMaxLength(2000); j.Property(x => x.Version).IsConcurrencyToken(); j.HasIndex(x => new { x.Status, x.CreatedAt }); var r = b.Entity<OperationalRecord>(); r.HasKey(x => x.Id); r.Property(x => x.Category).HasMaxLength(100); r.Property(x => x.Amount).HasPrecision(18, 2); r.HasIndex(x => new { x.JobId, x.LineNumber }).IsUnique(); r.HasIndex(x => x.Category); } }
public sealed class AnalyticsDbContextFactory : IDesignTimeDbContextFactory<AnalyticsDbContext> { public AnalyticsDbContext CreateDbContext(string[] a) => new(new DbContextOptionsBuilder<AnalyticsDbContext>().UseSqlServer("Server=localhost;Database=AnalyticsReporting;Trusted_Connection=True;TrustServerCertificate=True").Options); }
