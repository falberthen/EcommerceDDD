namespace EcommerceDDD.Core.Infrastructure.CQRS;

public class CommandBus(
	IServiceProvider serviceProvider,
	ILogger<CommandBus> logger
) : ICommandBus
{
	public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
		where TCommand : ICommand
	{
		logger.LogInformation("Sending command: {command}", command);

		var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
		dynamic? handler = serviceProvider.GetService(handlerType)
			?? throw new InvalidOperationException($"Handler for command {command.GetType().Name} not registered.");

		await handler.HandleAsync((dynamic)command, cancellationToken);
	}
}