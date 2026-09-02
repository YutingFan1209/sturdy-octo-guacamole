using AnalyticsReporting.Core;
using Microsoft.EntityFrameworkCore;
namespace AnalyticsReporting.Infrastructure;

public sealed class LocalFileStore(string root) : IFileStore
{ public async Task<string> SaveAsync(Guid id, Stream source, CancellationToken ct) { Directory.CreateDirectory(root); var key = $"{id:N}.csv"; var path = Path.Combine(root, key); await using var target = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 65536, FileOptions.Asynchronous | FileOptions.SequentialScan); var buffer = new byte[65536]; int read; while ((read = await source.ReadAsync(buffer, ct)) > 0) await target.WriteAsync(buffer.AsMemory(0, read), ct); return key; } public Task<Stream> OpenReadAsync(string key, CancellationToken ct) { Stream stream = new FileStream(Path.Combine(root, Path.GetFileName(key)), FileMode.Open, FileAccess.Read, FileShare.Read, 65536, FileOptions.Asynchronous | FileOptions.SequentialScan); return Task.FromResult(stream); } }
public sealed class SqlJobStore(AnalyticsDbContext db) : IJobStore
{
    public async Task AddAsync(ImportJob job, CancellationToken ct) { db.Jobs.Add(job); await db.SaveChangesAsync(ct); }
    public Task<ImportJob?> FindAsync(Guid id, CancellationToken ct) => db.Jobs.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<ImportJob?> LeaseNextAsync(CancellationToken ct) => db.Jobs.OrderBy(x => x.CreatedAt).FirstOrDefaultAsync(x => x.Status == JobStatus.Queued || x.Status == JobStatus.Processing, ct);
    public async Task SaveAsync(ImportJob job, CancellationToken ct) { if (db.Entry(job).State == EntityState.Detached) db.Update(job); await db.SaveChangesAsync(ct); }
    public async Task AddRecordsAsync(IReadOnlyCollection<OperationalRecord> records, CancellationToken ct) { if (records.Count == 0) return; var job = records.First().JobId; var min = records.Min(x => x.LineNumber); var max = records.Max(x => x.LineNumber); var existing = await db.Records.Where(x => x.JobId == job && x.LineNumber >= min && x.LineNumber <= max).Select(x => x.LineNumber).ToHashSetAsync(ct); db.Records.AddRange(records.Where(x => !existing.Contains(x.LineNumber))); await db.SaveChangesAsync(ct); db.ChangeTracker.Clear(); }
    public async Task<IReadOnlyList<(string Category, decimal Total)>> ReportAsync(CancellationToken ct) => (await db.Records.AsNoTracking().GroupBy(x => x.Category).Select(g => new ReportRow(g.Key, g.Sum(x => x.Amount))).OrderByDescending(x => x.Total).ToListAsync(ct)).Select(x => (x.Category, x.Total)).ToArray(); private sealed record ReportRow(string Category, decimal Total);
}
