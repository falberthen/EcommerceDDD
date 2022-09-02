using MediatR;
using System.Threading;

namespace EcommerceDDD.Core.CQRS.CommandHandling;

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// To override
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task Handle(TCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// MediatR Handle implementation
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    async Task<Unit> IRequestHandler<TCommand, Unit>.Handle(TCommand command, CancellationToken cancellationToken)
    {
        try
        {
            command.Validate();
            if (command.ValidationResult.IsValid)
                await Handle(command, cancellationToken);
        }
        catch (Exception) 
        {
            throw;
        }

        return Unit.Value;
    }
}