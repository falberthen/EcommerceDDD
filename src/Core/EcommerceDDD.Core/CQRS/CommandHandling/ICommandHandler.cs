namespace EcommerceDDD.Core.CQRS.CommandHandling;

public interface ICommandHandler<TCommand>
	where TCommand : ICommand
{
	Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken);
}