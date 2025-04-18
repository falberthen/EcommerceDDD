namespace EcommerceDDD.Core.CQRS.CommandHandling;

public interface ICommandHandler<TCommand> 
	where TCommand : ICommand
{
	Task HandleAsync(TCommand command, CancellationToken cancellationToken);
}