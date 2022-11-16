namespace EcommerceDDD.Core.CQRS.CommandHandling;

public interface ICommandBus
{
    Task Send<TCommand>(TCommand command)
        where TCommand : ICommand;
}
