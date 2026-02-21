namespace EcommerceDDD.Core.Infrastructure.WebApi;

public static class ControllerBaseProblemDetailsExtensions
{
	/// <summary>
	/// Creates a standardized ProblemDetails response for any status code.
	/// </summary>
	public static IActionResult ProblemResponse(
		this ControllerBase controller,
		int statusCode,
		string detail,
		string? title = null,
		string? type = null,
		IDictionary<string, object?>? extensions = null)
	{
		var problem = new ProblemDetails
		{
			Status = statusCode,
			Detail = detail,
			Title = title ?? GetDefaultTitle(statusCode),
			Type = type ?? $"https://httpstatuses.com/{statusCode}"
		};

		if (controller.HttpContext is not null)
		{
			problem.Extensions["traceId"] = controller.HttpContext.TraceIdentifier;
		}

		if (extensions is not null)
		{
			foreach (var kvp in extensions)
			{
				problem.Extensions[kvp.Key] = kvp.Value;
			}
		}

		return controller.StatusCode(statusCode, problem);
	}

	/// <summary>
	/// Creates a standardized ValidationProblemDetails response (422 by default).
	/// </summary>
	public static IActionResult ValidationProblemResponse(
		this ControllerBase controller,
		string detail,
		IDictionary<string, string[]>? errors = null,
		string? title = null,
		int statusCode = StatusCodes.Status422UnprocessableEntity,
		string? type = null,
		IDictionary<string, object?>? extensions = null)
	{
		errors ??= new Dictionary<string, string[]>
		{
			["error"] = new[] { detail }
		};

		var problem = new ValidationProblemDetails(errors)
		{
			Status = statusCode,
			Detail = detail,
			Title = title ?? "Validation failed",
			Type = type ?? $"https://httpstatuses.com/{statusCode}"
		};

		if (controller.HttpContext is not null)
		{
			problem.Extensions["traceId"] = controller.HttpContext.TraceIdentifier;
		}

		if (extensions is not null)
		{
			foreach (var kvp in extensions)
			{
				problem.Extensions[kvp.Key] = kvp.Value;
			}
		}

		return controller.StatusCode(statusCode, problem);
	}
	
	// Convenience helpers for common status codes
	public static IActionResult BadRequestProblem(
		this ControllerBase controller,
		string detail,
		string? title = null,
		IDictionary<string, object?>? extensions = null)
		=> controller.ProblemResponse(
			statusCode: StatusCodes.Status400BadRequest,
			detail: detail,
			title: title ?? "Bad request",
			extensions: extensions);

	public static IActionResult UnauthorizedProblem(
		this ControllerBase controller,
		string detail,
		string? title = null,
		IDictionary<string, object?>? extensions = null)
		=> controller.ProblemResponse(
			statusCode: StatusCodes.Status401Unauthorized,
			detail: detail,
			title: title ?? "Unauthorized",
			extensions: extensions);

	public static IActionResult ForbiddenProblem(
		this ControllerBase controller,
		string detail,
		string? title = null,
		IDictionary<string, object?>? extensions = null)
		=> controller.ProblemResponse(
			statusCode: StatusCodes.Status403Forbidden,
			detail: detail,
			title: title ?? "Forbidden",
			extensions: extensions);

	public static IActionResult NotFoundProblem(
		this ControllerBase controller,
		string detail,
		string? title = null,
		IDictionary<string, object?>? extensions = null)
		=> controller.ProblemResponse(
			statusCode: StatusCodes.Status404NotFound,
			detail: detail,
			title: title ?? "Resource not found",
			extensions: extensions);

	public static IActionResult ConflictProblem(
		this ControllerBase controller,
		string detail,
		string? title = null,
		IDictionary<string, object?>? extensions = null)
		=> controller.ProblemResponse(
			statusCode: StatusCodes.Status409Conflict,
			detail: detail,
			title: title ?? "Conflict",
			extensions: extensions);

	public static IActionResult UnprocessableEntityProblem(
		this ControllerBase controller,
		string detail,
		string? title = null,
		IDictionary<string, object?>? extensions = null)
		=> controller.ProblemResponse(
			statusCode: StatusCodes.Status422UnprocessableEntity,
			detail: detail,
			title: title ?? "Unprocessable entity",
			extensions: extensions);

	public static IActionResult InternalServerErrorProblem(
		this ControllerBase controller,
		string detail,
		string? title = null,
		IDictionary<string, object?>? extensions = null)
		=> controller.ProblemResponse(
			statusCode: StatusCodes.Status500InternalServerError,
			detail: detail,
			title: title ?? "Internal server error",
			extensions: extensions);

	private static string GetDefaultTitle(int statusCode) => statusCode switch
	{
		StatusCodes.Status400BadRequest => "Bad request",
		StatusCodes.Status401Unauthorized => "Unauthorized",
		StatusCodes.Status403Forbidden => "Forbidden",
		StatusCodes.Status404NotFound => "Resource not found",
		StatusCodes.Status409Conflict => "Conflict",
		StatusCodes.Status422UnprocessableEntity => "Validation failed",
		StatusCodes.Status500InternalServerError => "Internal server error",
		_ => "Request failed"
	};
}