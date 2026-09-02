using ComplianceCaseManagement.Cases;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace ComplianceCaseManagement.Api; public sealed class ApiExceptionHandler(IProblemDetailsService details) : IExceptionHandler { public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception e, CancellationToken ct) { var status = e switch { CaseConflictException => 409, InvalidOperationException => 409, KeyNotFoundException => 404, ArgumentException => 400, _ => 500 }; context.Response.StatusCode = status; return await details.TryWriteAsync(new ProblemDetailsContext { HttpContext = context, ProblemDetails = new ProblemDetails { Status = status, Title = status == 409 ? "Case update conflict" : "Request failed", Detail = e.Message } }); } }
