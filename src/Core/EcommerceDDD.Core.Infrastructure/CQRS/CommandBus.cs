namespace EcommerceDDD.Core.Infrastructure.CQRS;

public class CommandBus(
	IServiceProvider serviceProvider,
	ILogger<CommandBus> logger
) : ICommandBus
{
	private static readonly ActivitySource _activitySource = new(ActivitySources.CommandBus);

	public async Task<Result> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
		where TCommand : ICommand
	{
		logger.LogInformation("Sending command: {command}", command);

		var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
		dynamic? handler = serviceProvider.GetService(handlerType)
			?? throw new InvalidOperationException($"Handler for command {command.GetType().Name} not registered.");

		using var activity = _activitySource.StartActivity(command.GetType().Name, ActivityKind.Internal);

		Result result;
		try
		{
			result = await handler.HandleAsync((dynamic)command, cancellationToken);
		}
		catch (Exception ex)
		{
			activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
			activity?.AddException(ex);
			throw;
		}

		if (result.IsFailed)
		{
			var message = result.Errors.FirstOrDefault()?.Message;
			activity?.SetStatus(ActivityStatusCode.Error, message);
		}

		return result;
	}
}
