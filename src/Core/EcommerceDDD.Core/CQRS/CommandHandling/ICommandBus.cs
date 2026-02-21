namespace EcommerceDDD.Core.CQRS.CommandHandling;

public interface ICommandBus
{
	Task<Result> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
		where TCommand : ICommand;
}
