namespace EcommerceDDD.Core.Infrastructure.CQRS;

public class CommandBus(IMediator mediator, ILogger<CommandBus> logger) : ICommandBus
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<CommandBus> _logger = logger;

    public Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand
    {
        _logger.LogInformation("Sending command: {command}", command);
        return _mediator.Send(command, cancellationToken);
    }
}
