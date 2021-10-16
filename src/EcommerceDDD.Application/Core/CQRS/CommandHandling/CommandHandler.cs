using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace EcommerceDDD.Application.Core.CQRS.CommandHandling
{
    /// <summary>
    /// Interface for Command Handler implementation
    /// </summary>
    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult> {}

    /// <summary>
    /// Abstract class for Command Handler implementation
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TID"></typeparam>
    public abstract class CommandHandler<TCommand, TID> : ICommandHandler<TCommand, CommandHandlerResult<TID>>
        where TCommand : ICommand<CommandHandlerResult<TID>>
        where TID : struct
    {
        /// <summary>
        /// To override
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<TID> ExecuteCommand(TCommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// MediatR Handle implementation
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CommandHandlerResult<TID>> Handle(TCommand command, CancellationToken cancellationToken = default)
        {
            CommandHandlerResult<TID> result = new CommandHandlerResult<TID>(command);

            try
            {
                if (result.ValidationResult.IsValid)
                    result.Id = await ExecuteCommand(command, cancellationToken);
            }
            catch (Exception) { throw; }
            return result;
        }
    }
}