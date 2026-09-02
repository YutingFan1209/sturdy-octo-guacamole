using System.ComponentModel.DataAnnotations;
namespace ComplianceCaseManagement.Cases;

public enum CaseStatus { Open, UnderInvestigation, Escalated, Resolved, Closed }
public sealed class ComplianceCase
{
    private readonly List<CaseAudit> _audit = []; private ComplianceCase() { }
    private ComplianceCase(Guid id, string title, string owner, DateTimeOffset deadline, DateTimeOffset now) { Id = id; Title = title; OwnerId = owner; Deadline = deadline; Status = CaseStatus.Open; UpdatedAt = now; _audit.Add(CaseAudit.Create(id, owner, "Created", null, CaseStatus.Open, now)); }
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty; public string OwnerId { get; private set; } = string.Empty; public DateTimeOffset Deadline { get; private set; }
    public CaseStatus Status { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long Version { get; private set; }
    public IReadOnlyCollection<CaseAudit> Audit => _audit.AsReadOnly();
    public static ComplianceCase Open(string title, string owner, DateTimeOffset deadline, DateTimeOffset now) => new(Guid.NewGuid(), Required(title), Required(owner), deadline, now);
    public void BeginInvestigation(string actor, DateTimeOffset now) => Change(CaseStatus.UnderInvestigation, actor, "Investigation started", now, CaseStatus.Open);
    public void Resolve(string actor, string reason, DateTimeOffset now) => Change(CaseStatus.Resolved, actor, Required(reason), now, CaseStatus.UnderInvestigation, CaseStatus.Escalated);
    public void Close(string actor, string reason, DateTimeOffset now) => Change(CaseStatus.Closed, actor, Required(reason), now, CaseStatus.Resolved);
    public bool EscalateIfOverdue(string actor, DateTimeOffset now) { if (Deadline > now || Status is not (CaseStatus.Open or CaseStatus.UnderInvestigation)) return false; Change(CaseStatus.Escalated, actor, "Deadline exceeded", now, CaseStatus.Open, CaseStatus.UnderInvestigation); return true; }
    private void Change(CaseStatus next, string actor, string reason, DateTimeOffset now, params CaseStatus[] allowed) { if (!allowed.Contains(Status)) throw new InvalidOperationException($"Cannot transition from {Status} to {next}."); var previous = Status; Status = next; UpdatedAt = now; Version++; _audit.Add(CaseAudit.Create(Id, Required(actor), reason, previous, next, now)); }
    private static string Required(string value) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Value is required.") : value.Trim();
}
public sealed class CaseAudit { private CaseAudit() { } public Guid Id { get; private set; } public Guid CaseId { get; private set; } public string ActorId { get; private set; } = string.Empty; public string Action { get; private set; } = string.Empty; public CaseStatus? PreviousStatus { get; private set; } public CaseStatus NewStatus { get; private set; } public DateTimeOffset OccurredAt { get; private set; } public static CaseAudit Create(Guid id, string actor, string action, CaseStatus? previous, CaseStatus next, DateTimeOffset now) => new() { Id = Guid.NewGuid(), CaseId = id, ActorId = actor, Action = action, PreviousStatus = previous, NewStatus = next, OccurredAt = now }; }
public sealed record CreateCase([Required, StringLength(200)] string Title, [Required] string OwnerId, DateTimeOffset Deadline); public sealed record CaseDto(Guid Id, string Title, string OwnerId, DateTimeOffset Deadline, CaseStatus Status, long Version, IReadOnlyCollection<CaseAuditDto> Audit); public sealed record CaseAuditDto(string ActorId, string Action, CaseStatus? PreviousStatus, CaseStatus NewStatus, DateTimeOffset OccurredAt); public sealed class CaseConflictException(Guid id) : Exception($"Case {id} changed concurrently. Refresh and retry.");
public interface ICaseStore { Task AddAsync(ComplianceCase item, CancellationToken ct); Task<ComplianceCase?> FindAsync(Guid id, CancellationToken ct); Task SaveAsync(long expected, CancellationToken ct); Task<int> EscalateOverdueAsync(DateTimeOffset now, string actor, CancellationToken ct); }
public sealed class CaseService(ICaseStore store, TimeProvider clock)
{
    public async Task<CaseDto> CreateAsync(CreateCase input, string actor, CancellationToken ct) { var item = ComplianceCase.Open(input.Title, input.OwnerId, input.Deadline, clock.GetUtcNow()); await store.AddAsync(item, ct); await store.SaveAsync(0, ct); return Map(item); }
    public Task<CaseDto> BeginAsync(Guid id, long version, string actor, CancellationToken ct) => Change(id, version, actor, (c, a, n) => c.BeginInvestigation(a, n), ct); public Task<CaseDto> ResolveAsync(Guid id, long version, string actor, string reason, CancellationToken ct) => Change(id, version, actor, (c, a, n) => c.Resolve(a, reason, n), ct);
    private async Task<CaseDto> Change(Guid id, long expected, string actor, Action<ComplianceCase, string, DateTimeOffset> action, CancellationToken ct) { var item = await store.FindAsync(id, ct) ?? throw new KeyNotFoundException(); if (item.Version != expected) throw new CaseConflictException(id); action(item, actor, clock.GetUtcNow()); await store.SaveAsync(expected, ct); return Map(item); }
    public Task<int> EscalateAsync(string actor, CancellationToken ct) => store.EscalateOverdueAsync(clock.GetUtcNow(), actor, ct);
    private static CaseDto Map(ComplianceCase c) => new(c.Id, c.Title, c.OwnerId, c.Deadline, c.Status, c.Version, c.Audit.Select(a => new CaseAuditDto(a.ActorId, a.Action, a.PreviousStatus, a.NewStatus, a.OccurredAt)).ToArray());
}
