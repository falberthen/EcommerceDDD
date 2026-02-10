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
			BusinessRuleException e => (StatusCodes.Status422UnprocessableEntity, e.Message),
			RecordNotFoundException e => (StatusCodes.Status404NotFound, e.Message),
			ApplicationLogicException e => (StatusCodes.Status500InternalServerError, e.Message),
			_ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
		};

		if (statusCode >= 500)
			logger.LogError(exception, "Server error: {Message}", exception.Message);
		else
			logger.LogWarning("Request error ({StatusCode}): {Message}", statusCode, exception.Message);

		httpContext.Response.StatusCode = statusCode;
		httpContext.Response.ContentType = "application/json";

		var response = new ApiResponse<object>
		{
			Success = false,
			Message = message
		};

		await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
		return true;
	}
}
