using ReconciliationPlatform.Core;
namespace ReconciliationPlatform.Tests;

[TestClass]
public sealed class IdempotencyAndFailureTests
{
    [TestMethod]
    public async Task Duplicate_mismatch_and_notification_outage_create_one_case_then_dead_letter()
    { var store = new MemoryStore(); var e = new MismatchEvent(Guid.NewGuid(), "billing", "invoice-42", 100, 90, "trace-abc", DateTimeOffset.UtcNow); var ingestion = new IngestionService(store); Assert.IsTrue(await ingestion.IngestAsync(e, default)); Assert.IsFalse(await ingestion.IngestAsync(e, default)); var reconcile = new ReconciliationService(store); Assert.IsTrue(await reconcile.ProcessNextAsync(default)); Assert.IsFalse(await reconcile.ProcessNextAsync(default)); var notifications = new NotificationService(store, new UnavailableGateway(), TimeProvider.System); for (var i = 0; i < 3; i++) Assert.IsTrue(await notifications.ProcessNextAsync(default)); Assert.AreEqual(1, store.CaseCount); Assert.AreEqual(0, store.SentCount); Assert.HasCount(1, await store.DeadLettersAsync(default)); Assert.AreEqual("trace-abc", (await store.DeadLettersAsync(default)).Single().CorrelationId); }
    [TestMethod] public async Task Different_message_ids_for_same_business_record_have_one_side_effect() { var s = new MemoryStore(); foreach (var id in new[] { Guid.NewGuid(), Guid.NewGuid() }) await s.AcceptAsync(new(id, "erp", "order-1", 10, 11, "trace-1", DateTimeOffset.UtcNow), default); var service = new ReconciliationService(s); await service.ProcessNextAsync(default); await service.ProcessNextAsync(default); Assert.AreEqual(1, s.CaseCount); }
    private sealed class UnavailableGateway : INotificationGateway { public Task SendAsync(Guid id, string c, CancellationToken ct) => throw new HttpRequestException("notification service unavailable"); }
    private sealed class MemoryStore : IReconciliationStore
    {
        private readonly Queue<MismatchEvent> inbox = []; private readonly HashSet<Guid> messages = []; private readonly HashSet<string> cases = []; private NotificationMessage? notification; private readonly List<DeadLetter> dead = []; public int CaseCount => cases.Count; public int SentCount;
        public Task<bool> AcceptAsync(MismatchEvent e, CancellationToken ct) { if (!messages.Add(e.MessageId)) return Task.FromResult(false); inbox.Enqueue(e); return Task.FromResult(true); }
        public Task<MismatchEvent?> NextAsync(CancellationToken ct) => Task.FromResult(inbox.Count == 0 ? null : inbox.Dequeue()); public Task<bool> CreateExceptionOnceAsync(MismatchEvent e, CancellationToken ct) { var added = cases.Add($"{e.SourceSystem}:{e.RecordKey}"); if (added) notification = new(Guid.NewGuid(), Guid.NewGuid(), e.CorrelationId, DateTimeOffset.UtcNow); return Task.FromResult(added); }
        public Task<NotificationMessage?> NextNotificationAsync(DateTimeOffset n, CancellationToken ct) => Task.FromResult(notification); public Task MarkNotificationSentAsync(NotificationMessage m, DateTimeOffset n, CancellationToken ct) { m.Sent(n); SentCount++; notification = null; return Task.CompletedTask; }
        public Task RetryOrDeadLetterAsync(NotificationMessage m, string error, DateTimeOffset n, int max, CancellationToken ct) { m.Failed(error, n); if (m.Attempts >= max) { dead.Add(new(Guid.NewGuid(), m.Id, m.CorrelationId, error, "{}", n)); notification = null; } return Task.CompletedTask; }
        public Task<IReadOnlyList<DeadLetter>> DeadLettersAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<DeadLetter>>(dead);
    }
}
