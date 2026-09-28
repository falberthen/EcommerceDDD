namespace EcommerceDDD.Core.Infrastructure.WebApi;

[ProducesErrorResponseType(typeof(ProblemDetails))]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[ApiController]
public class CustomControllerBase : ControllerBase
{
	private readonly IMessageBus? _bus;

	public CustomControllerBase(){}

	protected CustomControllerBase(IMessageBus bus)
		=> _bus = bus ?? throw new ArgumentNullException(nameof(bus));

	/// <summary>
	/// Executes a query through Wolverine and maps FluentResults failures to HTTP ProblemDetails.
	/// </summary>
	protected async Task<IActionResult> Response<TResult>(
		IQuery<TResult> query,
		CancellationToken cancellationToken)
	{
		var result = await Bus.InvokeAsync<Result<TResult>>(query, cancellationToken);
		return result.IsFailed ? Failure(result) : Ok(result.Value);
	}

	/// <summary>
	/// Executes a command through Wolverine and maps FluentResults failures to HTTP ProblemDetails.
	/// </summary>
	protected async Task<IActionResult> Response(
		ICommand command,
		CancellationToken cancellationToken)
	{
		var result = await Bus.InvokeAsync<Result>(command, cancellationToken);
		return result.IsFailed ? Failure(result) : Ok();
	}

	/// <summary>
	/// Maps a failed result and logs it. A Result failure never throws, so GlobalExceptionHandler
	/// never sees it: this is the only place it gets logged. Same levels as the exception path.
	/// </summary>
	private IActionResult Failure(IResultBase result)
	{
		var response = MapFailure(result);
		var statusCode = (response as IStatusCodeActionResult)?.StatusCode
			?? StatusCodes.Status500InternalServerError;
		var messages = string.Join("; ", result.Errors.Select(e => e.Message));
		var logger = HttpContext.RequestServices
			.GetRequiredService<ILoggerFactory>()
			.CreateLogger(GetType());

		if (statusCode >= 500)
			logger.LogError("Request failed ({StatusCode}): {Message}", statusCode, messages);
		else
			logger.LogWarning("Request failed ({StatusCode}): {Message}", statusCode, messages);

		return response;
	}

	/// <summary>
	/// Maps FluentResults failures into standardized HTTP responses using ProblemDetails.
	/// </summary>
	protected virtual IActionResult MapFailure(IResultBase result)
	{
		var errors = result.Errors;
		var firstMessage = errors.FirstOrDefault()?.Message ?? "Unexpected error.";

		// Arms are checked in order, so with mixed errors the first matching type wins.
		return errors switch
		{
			// 403 - Authenticated, but the resource belongs to someone else
			_ when errors.Any(e => e is ForbiddenError) =>
				this.ForbiddenProblem(detail: firstMessage, title: "Forbidden"),

			// 404 - Not found
			_ when errors.Any(e => e is RecordNotFoundError) =>
				this.NotFoundProblem(detail: firstMessage, title: "Resource not found"),

			// 422 - Validation/business rule failure
			_ when errors.Any(e => e is ValidationError) =>
				this.ValidationProblemResponse(
					detail: firstMessage,
					errors: errors
						.OfType<ValidationError>()
						.Select((e, index) => (Key: $"error{index + 1}", e.Message))
						.ToDictionary(x => x.Key, x => new[] { x.Message }),
					title: "Validation failed"),

			// 500 - Unexpected/internal failure
			_ => this.InternalServerErrorProblem(detail: firstMessage, title: "Internal server error")
		};
	}

	private IMessageBus Bus => _bus ?? throw new InvalidOperationException(
		$"{nameof(CustomControllerBase)} was built without an {nameof(IMessageBus)}. " +
		"Use the CQRS constructor, or do not call Response(...).");
}
