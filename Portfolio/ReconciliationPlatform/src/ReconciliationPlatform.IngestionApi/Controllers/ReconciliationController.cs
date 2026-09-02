using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReconciliationPlatform.Core;
namespace ReconciliationPlatform.IngestionApi.Controllers;

[ApiController, Route("api/reconciliation")] public sealed class ReconciliationController(IngestionService ingestion, IReconciliationStore store) : ControllerBase { [HttpPost("mismatches"), Authorize(Policy = "Ingest")] public async Task<IActionResult> Ingest(MismatchEvent input, CancellationToken ct) { var accepted = await ingestion.IngestAsync(input, ct); return accepted ? Accepted(new { input.MessageId, input.CorrelationId }) : Ok(new { input.MessageId, input.CorrelationId, Duplicate = true }); } [HttpGet("dead-letters"), Authorize(Policy = "Operations")] public Task<IReadOnlyList<DeadLetter>> DeadLetters(CancellationToken ct) => store.DeadLettersAsync(ct); }
