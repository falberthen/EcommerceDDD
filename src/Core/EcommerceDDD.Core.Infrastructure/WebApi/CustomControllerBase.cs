namespace EcommerceDDD.Core.Infrastructure.WebApi;

public class CustomControllerBase : ControllerBase
{
	private readonly ICommandBus? _commandBus;
	private readonly IQueryBus? _queryBus;

	public CustomControllerBase(){}

	protected CustomControllerBase(ICommandBus commandBus, IQueryBus queryBus)
	{
		_commandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));
		_queryBus = queryBus ?? throw new ArgumentNullException(nameof(queryBus));
	}

	/// <summary>
	/// Executes a query through the query bus and maps FluentResults failures to HTTP ProblemDetails.
	/// </summary>
	protected async Task<IActionResult> Response<TResult>(
		IQuery<TResult> query,
		CancellationToken cancellationToken)
	{
		EnsureQueryBus();

		var result = await _queryBus!.SendAsync(query, cancellationToken);
		return result.IsFailed ? MapFailure(result) : Ok(result.Value);
	}

	/// <summary>
	/// Executes a command through the command bus and maps FluentResults failures to HTTP ProblemDetails.
	/// </summary>
	protected async Task<IActionResult> Response(
		ICommand command,
		CancellationToken cancellationToken)
	{
		EnsureCommandBus();

		var result = await _commandBus!.SendAsync(command, cancellationToken);
		return result.IsFailed ? MapFailure(result) : Ok();
	}

	/// <summary>
	/// Maps FluentResults failures into standardized HTTP responses using ProblemDetails.
	/// </summary>
	protected virtual IActionResult MapFailure(IResultBase result)
	{
		var firstMessage = result.Errors.FirstOrDefault()?.Message ?? "Unexpected error.";

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

	private void EnsureCommandBus()
	{
		if (_commandBus is null)
		{
			throw new InvalidOperationException(
				$"{nameof(CustomControllerBase)} command helper was used, but no {nameof(ICommandBus)} was provided. " +
				"Use the CQRS constructor or avoid calling Response(ICommand, ...).");
		}
	}

	private void EnsureQueryBus()
	{
		if (_queryBus is null)
		{
			throw new InvalidOperationException(
				$"{nameof(CustomControllerBase)} query helper was used, but no {nameof(IQueryBus)} was provided. " +
				"Use the CQRS constructor or avoid calling Response(IQuery<T>, ...).");
		}
	}
}