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
		return result.IsFailed ? MapFailure(result) : Ok(result.Value);
	}

	/// <summary>
	/// Executes a command through Wolverine and maps FluentResults failures to HTTP ProblemDetails.
	/// </summary>
	protected async Task<IActionResult> Response(
		ICommand command,
		CancellationToken cancellationToken)
	{
		var result = await Bus.InvokeAsync<Result>(command, cancellationToken);
		return result.IsFailed ? MapFailure(result) : Ok();
	}

	/// <summary>
	/// Maps FluentResults failures into standardized HTTP responses using ProblemDetails.
	/// </summary>
	protected virtual IActionResult MapFailure(IResultBase result)
	{
		var firstMessage = result.Errors.FirstOrDefault()?.Message ?? "Unexpected error.";

		// 403 - Authenticated, but the resource belongs to someone else
		if (result.Errors.OfType<ForbiddenError>().Any())
		{
			return this.ForbiddenProblem(
				detail: firstMessage,
				title: "Forbidden");
		}

		// 404 - Not found
		if (result.Errors.OfType<RecordNotFoundError>().Any())
		{
			return this.NotFoundProblem(
				detail: firstMessage,
				title: "Resource not found");
		}

		// 422 - Validation/business rule failure
		if (result.Errors.OfType<ValidationError>().Any())
		{
			var validationErrors = result.Errors
				.OfType<ValidationError>()
				.Select((e, index) => new { Key = $"error{index + 1}", Message = e.Message })
				.GroupBy(x => x.Key)
				.ToDictionary(
					g => g.Key,
					g => g.Select(x => x.Message).ToArray()
				);

			return this.ValidationProblemResponse(
				detail: firstMessage,
				errors: validationErrors,
				title: "Validation failed");
		}

		// 500 - Unexpected/internal failure
		return this.InternalServerErrorProblem(
			detail: firstMessage,
			title: "Internal server error");
	}

	private IMessageBus Bus => _bus ?? throw new InvalidOperationException(
		$"{nameof(CustomControllerBase)} was built without an {nameof(IMessageBus)}. " +
		"Use the CQRS constructor, or do not call Response(...).");
}
