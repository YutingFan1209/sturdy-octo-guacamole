namespace EmployeeOperations.Domain;

public enum EquipmentRequestStatus { Draft, Submitted, Approved, Rejected, Completed, Cancelled }

public sealed class InvalidRequestTransitionException(string message) : InvalidOperationException(message);

public sealed class EquipmentRequest
{
    private readonly List<RequestTransition> _transitions = [];

    private EquipmentRequest() { }

    private EquipmentRequest(Guid id, string employeeId, string item, string justification, DateTimeOffset now)
    {
        Id = id;
        EmployeeId = Require(employeeId, nameof(employeeId));
        Item = Require(item, nameof(item));
        Justification = Require(justification, nameof(justification));
        Status = EquipmentRequestStatus.Draft;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public string EmployeeId { get; private set; } = string.Empty;
    public string Item { get; private set; } = string.Empty;
    public string Justification { get; private set; } = string.Empty;
    public EquipmentRequestStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long Version { get; private set; }
    public IReadOnlyCollection<RequestTransition> Transitions => _transitions.AsReadOnly();

    public static EquipmentRequest Create(string employeeId, string item, string justification, DateTimeOffset now) =>
        new(Guid.NewGuid(), employeeId, item, justification, now);

    public void Submit(string actorId, DateTimeOffset now) => TransitionTo(EquipmentRequestStatus.Submitted, actorId, null, now);
    public void Approve(string actorId, string? reason, DateTimeOffset now) => TransitionTo(EquipmentRequestStatus.Approved, actorId, reason, now);
    public void Reject(string actorId, string? reason, DateTimeOffset now) => TransitionTo(EquipmentRequestStatus.Rejected, actorId, reason, now);
    public void Complete(string actorId, string? reason, DateTimeOffset now) => TransitionTo(EquipmentRequestStatus.Completed, actorId, reason, now);
    public void Cancel(string actorId, string? reason, DateTimeOffset now) => TransitionTo(EquipmentRequestStatus.Cancelled, actorId, reason, now);

    private void TransitionTo(EquipmentRequestStatus next, string actorId, string? reason, DateTimeOffset now)
    {
        var valid = (Status, next) switch
        {
            (EquipmentRequestStatus.Draft, EquipmentRequestStatus.Submitted) => true,
            (EquipmentRequestStatus.Submitted, EquipmentRequestStatus.Approved) => true,
            (EquipmentRequestStatus.Submitted, EquipmentRequestStatus.Rejected) => true,
            (EquipmentRequestStatus.Approved, EquipmentRequestStatus.Completed) => true,
            (EquipmentRequestStatus.Draft, EquipmentRequestStatus.Cancelled) => true,
            (EquipmentRequestStatus.Submitted, EquipmentRequestStatus.Cancelled) => true,
            _ => false
        };

        if (!valid) throw new InvalidRequestTransitionException($"Cannot transition an equipment request from {Status} to {next}.");
        var previous = Status;
        Status = next;
        UpdatedAt = now;
        Version++;
        _transitions.Add(new RequestTransition(Guid.NewGuid(), Id, previous, next, Require(actorId, nameof(actorId)), now, reason));
    }

    private static string Require(string value, string name) =>
        string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Value is required.", name) : value.Trim();
}

public sealed class RequestTransition
{
    private RequestTransition() { }
    public RequestTransition(Guid id, Guid requestId, EquipmentRequestStatus previousStatus, EquipmentRequestStatus newStatus,
        string actorId, DateTimeOffset occurredAt, string? reason)
    {
        Id = id; RequestId = requestId; PreviousStatus = previousStatus; NewStatus = newStatus;
        ActorId = actorId; OccurredAt = occurredAt; Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
    }
    public Guid Id { get; private set; }
    public Guid RequestId { get; private set; }
    public EquipmentRequestStatus PreviousStatus { get; private set; }
    public EquipmentRequestStatus NewStatus { get; private set; }
    public string ActorId { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public string? Reason { get; private set; }
}
