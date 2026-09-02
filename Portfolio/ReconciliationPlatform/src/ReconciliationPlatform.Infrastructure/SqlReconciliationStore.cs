using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ReconciliationPlatform.Core;
namespace ReconciliationPlatform.Infrastructure;

public sealed class SqlReconciliationStore(ReconciliationDbContext db, TimeProvider clock) : IReconciliationStore
{
    public async Task<bool> AcceptAsync(MismatchEvent e, CancellationToken ct) { if (await db.Inbox.AnyAsync(x => x.MessageId == e.MessageId, ct)) return false; db.Inbox.Add(new(e, JsonSerializer.Serialize(e))); try { await db.SaveChangesAsync(ct); return true; } catch (DbUpdateException) { return false; } }
    public async Task<MismatchEvent?> NextAsync(CancellationToken ct) { var item = await db.Inbox.AsNoTracking().OrderBy(x => x.ReceivedAt).FirstOrDefaultAsync(x => x.ProcessedAt == null, ct); return item is null ? null : JsonSerializer.Deserialize<MismatchEvent>(item.Payload); }
    public async Task<bool> CreateExceptionOnceAsync(MismatchEvent e, CancellationToken ct) { await using var tx = await db.Database.BeginTransactionAsync(ct); var inbox = await db.Inbox.SingleAsync(x => x.MessageId == e.MessageId, ct); if (inbox.ProcessedAt is not null) return false; var exists = await db.Cases.AnyAsync(x => x.SourceSystem == e.SourceSystem && x.RecordKey == e.RecordKey, ct); if (!exists) { var item = new ExceptionCase(Guid.NewGuid(), e.SourceSystem, e.RecordKey, e.ExpectedAmount, e.ActualAmount, e.CorrelationId, clock.GetUtcNow()); db.Cases.Add(item); db.Notifications.Add(new(Guid.NewGuid(), item.Id, e.CorrelationId, clock.GetUtcNow())); } inbox.MarkProcessed(clock.GetUtcNow()); try { await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return !exists; } catch (DbUpdateException) { await tx.RollbackAsync(ct); return false; } }
    public Task<NotificationMessage?> NextNotificationAsync(DateTimeOffset now, CancellationToken ct) => db.Notifications.OrderBy(x => x.NextAttemptAt).FirstOrDefaultAsync(x => x.SentAt == null && x.NextAttemptAt <= now, ct);
    public async Task MarkNotificationSentAsync(NotificationMessage m, DateTimeOffset now, CancellationToken ct) { m.Sent(now); await db.SaveChangesAsync(ct); }
    public async Task RetryOrDeadLetterAsync(NotificationMessage m, string error, DateTimeOffset now, int max, CancellationToken ct) { m.Failed(error, now); if (m.Attempts >= max) { db.DeadLetters.Add(new(Guid.NewGuid(), m.Id, m.CorrelationId, error, JsonSerializer.Serialize(new { m.CaseId, m.Attempts }), now)); db.Notifications.Remove(m); } await db.SaveChangesAsync(ct); }
    public async Task<IReadOnlyList<DeadLetter>> DeadLettersAsync(CancellationToken ct) => await db.DeadLetters.AsNoTracking().OrderByDescending(x => x.DeadLetteredAt).ToListAsync(ct);
}
