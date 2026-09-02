using AnalyticsReporting.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AnalyticsReporting.Api.Controllers;

[ApiController, Route("api/jobs"), Authorize(Policy = "ReportingUser")]
public sealed class JobsController(ImportService imports, IJobStore store) : ControllerBase
{ [HttpPost("upload"), RequestSizeLimit(1_073_741_824)] public async Task<ActionResult<JobDto>> Upload(IFormFile file, CancellationToken ct) { await using var stream = file.OpenReadStream(); var result = await imports.UploadAsync(file.FileName, file.Length, stream, ct); return AcceptedAtAction(nameof(Get), new { id = result.Id }, result); } [HttpGet("{id:guid}")] public Task<JobDto> Get(Guid id, CancellationToken ct) => imports.GetAsync(id, ct); [HttpGet("/api/reports/category-totals")] public async Task<IActionResult> Report(CancellationToken ct) => Ok((await store.ReportAsync(ct)).Select(x => new { x.Category, x.Total })); }
