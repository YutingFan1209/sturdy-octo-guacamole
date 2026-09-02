using System.Security.Claims;
using ComplianceCaseManagement.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ComplianceCaseManagement.Api.Controllers;

[ApiController, Route("api/cases"), Authorize(Policy = "Analyst")] public sealed class CasesController(CaseService service) : ControllerBase { [HttpPost] public Task<CaseDto> Create(CreateCase input, CancellationToken ct) => service.CreateAsync(input, Actor(), ct); [HttpPost("{id:guid}/begin")] public Task<CaseDto> Begin(Guid id, long expectedVersion, CancellationToken ct) => service.BeginAsync(id, expectedVersion, Actor(), ct); [HttpPost("{id:guid}/resolve")] public Task<CaseDto> Resolve(Guid id, long expectedVersion, string reason, CancellationToken ct) => service.ResolveAsync(id, expectedVersion, Actor(), reason, ct); private string Actor() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? throw new UnauthorizedAccessException(); }
