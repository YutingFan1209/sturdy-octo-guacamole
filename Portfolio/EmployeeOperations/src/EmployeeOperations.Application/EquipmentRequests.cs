using System.ComponentModel.DataAnnotations;
using EmployeeOperations.Domain;

namespace EmployeeOperations.Application;

public sealed record CreateEquipmentRequest([Required, StringLength(100)] string Item, [Required, StringLength(1000)] string Justification);
public sealed record TransitionRequest([StringLength(500)] string? Reason, long ExpectedVersion);
public sealed record TransitionDto(EquipmentRequestStatus PreviousStatus, EquipmentRequestStatus NewStatus, string ActorId, DateTimeOffset OccurredAt, string? Reason);
public sealed record EquipmentRequestDto(Guid Id, string EmployeeId, string Item, string Justification, EquipmentRequestStatus Status,
    DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, long Version, IReadOnlyCollection<TransitionDto> History);

public interface IEquipmentRequestRepository
{
    Task AddAsync(EquipmentRequest request, CancellationToken cancellationToken);
    Task<EquipmentRequest?> FindAsync(Guid id, CancellationToken cancellationToken);
    Task SaveChangesAsync(long expectedVersion, CancellationToken cancellationToken);
}

public sealed class RequestNotFoundException(Guid id) : Exception($"Equipment request {id} was not found.");
public sealed class RequestConcurrencyException(Guid id) : Exception($"Equipment request {id} was changed by another user. Refresh and retry.");

public sealed class EquipmentRequestService(IEquipmentRequestRepository repository, TimeProvider timeProvider)
{
    public async Task<EquipmentRequestDto> CreateAsync(string employeeId, CreateEquipmentRequest input, CancellationToken ct)
    {
        var request = EquipmentRequest.Create(employeeId, input.Item, input.Justification, timeProvider.GetUtcNow());
        await repository.AddAsync(request, ct);
        await repository.SaveChangesAsync(0, ct);
        return Map(request);
    }

    public async Task<EquipmentRequestDto> GetAsync(Guid id, CancellationToken ct) =>
        Map(await repository.FindAsync(id, ct) ?? throw new RequestNotFoundException(id));

    public Task<EquipmentRequestDto> SubmitAsync(Guid id, string actor, TransitionRequest input, CancellationToken ct) =>
        ChangeAsync(id, actor, input, (r, a, reason, now) => r.Submit(a, now), ct);
    public Task<EquipmentRequestDto> ApproveAsync(Guid id, string actor, TransitionRequest input, CancellationToken ct) =>
        ChangeAsync(id, actor, input, (r, a, reason, now) => r.Approve(a, reason, now), ct);
    public Task<EquipmentRequestDto> RejectAsync(Guid id, string actor, TransitionRequest input, CancellationToken ct) =>
        ChangeAsync(id, actor, input, (r, a, reason, now) => r.Reject(a, reason, now), ct);
    public Task<EquipmentRequestDto> CompleteAsync(Guid id, string actor, TransitionRequest input, CancellationToken ct) =>
        ChangeAsync(id, actor, input, (r, a, reason, now) => r.Complete(a, reason, now), ct);
    public Task<EquipmentRequestDto> CancelAsync(Guid id, string actor, TransitionRequest input, CancellationToken ct) =>
        ChangeAsync(id, actor, input, (r, a, reason, now) => r.Cancel(a, reason, now), ct);

    private async Task<EquipmentRequestDto> ChangeAsync(Guid id, string actor, TransitionRequest input,
        Action<EquipmentRequest, string, string?, DateTimeOffset> transition, CancellationToken ct)
    {
        var request = await repository.FindAsync(id, ct) ?? throw new RequestNotFoundException(id);
        if (request.Version != input.ExpectedVersion) throw new RequestConcurrencyException(id);
        transition(request, actor, input.Reason, timeProvider.GetUtcNow());
        await repository.SaveChangesAsync(input.ExpectedVersion, ct);
        return Map(request);
    }

    private static EquipmentRequestDto Map(EquipmentRequest request) => new(request.Id, request.EmployeeId, request.Item,
        request.Justification, request.Status, request.CreatedAt, request.UpdatedAt, request.Version,
        request.Transitions.OrderBy(x => x.OccurredAt).Select(x => new TransitionDto(x.PreviousStatus, x.NewStatus, x.ActorId, x.OccurredAt, x.Reason)).ToArray());
}
