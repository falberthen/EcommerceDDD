namespace EcommerceDDD.Core.CQRS.CommandHandling;

public interface ICommandBus
{
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand;
}
