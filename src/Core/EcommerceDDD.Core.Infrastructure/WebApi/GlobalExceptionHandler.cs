using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Core.Infrastructure.WebApi;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
	: Microsoft.AspNetCore.Diagnostics.IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		if (exception is OperationCanceledException)
		{
			logger.LogInformation("Request was canceled.");
			return true;
		}

		var (statusCode, message) = exception switch
		{
			DomainException e => (StatusCodes.Status422UnprocessableEntity, e.Message),
			_ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
		};

		if (statusCode >= 500)
		{
			logger.LogError(exception, "Server error: {Message}", exception.Message);
			Activity.Current?.SetStatus(ActivityStatusCode.Error, exception.Message);
			Activity.Current?.AddException(exception);
		}
		else
		{
			logger.LogWarning("Request error ({StatusCode}): {Message}", statusCode, exception.Message);
			Activity.Current?.SetStatus(ActivityStatusCode.Error, message);
		}

		var traceId = Activity.Current?.TraceId.ToString()
			?? httpContext.TraceIdentifier;

		httpContext.Response.StatusCode = statusCode;
		httpContext.Response.ContentType = "application/json";

		await httpContext.Response.WriteAsJsonAsync(
			new { success = false, message, traceId },
			cancellationToken);
		return true;
	}
}
