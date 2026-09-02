using System.Security.Claims;
using EmployeeOperations.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOperations.Api.Controllers;

[ApiController, Route("api/equipment-requests"), Authorize]
public sealed class EquipmentRequestsController(EquipmentRequestService service) : ControllerBase
{
    [HttpPost, Authorize(Policy = "Employee")]
    public async Task<ActionResult<EquipmentRequestDto>> Create(CreateEquipmentRequest input, CancellationToken ct)
    { var result = await service.CreateAsync(ActorId(), input, ct); return CreatedAtAction(nameof(Get), new { id = result.Id }, result); }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EquipmentRequestDto>> Get(Guid id, CancellationToken ct) => Ok(await service.GetAsync(id, ct));
    [HttpPost("{id:guid}/submit"), Authorize(Policy = "Employee")]
    public Task<EquipmentRequestDto> Submit(Guid id, TransitionRequest input, CancellationToken ct) => service.SubmitAsync(id, ActorId(), input, ct);
    [HttpPost("{id:guid}/cancel"), Authorize(Policy = "Employee")]
    public Task<EquipmentRequestDto> Cancel(Guid id, TransitionRequest input, CancellationToken ct) => service.CancelAsync(id, ActorId(), input, ct);
    [HttpPost("{id:guid}/approve"), Authorize(Policy = "Manager")]
    public Task<EquipmentRequestDto> Approve(Guid id, TransitionRequest input, CancellationToken ct) => service.ApproveAsync(id, ActorId(), input, ct);
    [HttpPost("{id:guid}/reject"), Authorize(Policy = "Manager")]
    public Task<EquipmentRequestDto> Reject(Guid id, TransitionRequest input, CancellationToken ct) => service.RejectAsync(id, ActorId(), input, ct);
    [HttpPost("{id:guid}/complete"), Authorize(Policy = "Operations")]
    public Task<EquipmentRequestDto> Complete(Guid id, TransitionRequest input, CancellationToken ct) => service.CompleteAsync(id, ActorId(), input, ct);
    private string ActorId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? throw new UnauthorizedAccessException();
}
