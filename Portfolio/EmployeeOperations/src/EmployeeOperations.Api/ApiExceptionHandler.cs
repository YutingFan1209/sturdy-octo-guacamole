using EmployeeOperations.Application;
using EmployeeOperations.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOperations.Api;

public sealed class ApiExceptionHandler(IProblemDetailsService problemDetails, ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            RequestNotFoundException => (404, "Request not found"),
            RequestConcurrencyException => (409, "Concurrent update detected"),
            InvalidRequestTransitionException => (409, "Invalid request transition"),
            ArgumentException => (400, "Invalid request"),
            _ => (500, "Unexpected server error")
        };
        logger.Log(status >= 500 ? LogLevel.Error : LogLevel.Warning, exception, "Request failed with {StatusCode}", status);
        context.Response.StatusCode = status;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails { Status = status, Title = title, Detail = exception.Message }
        });
    }
}
