using System;

namespace EcommerceDDD.Application.Core.CQRS.CommandHandling;

/// <summary>
/// Interface for CommandHandlerResult implementation
/// </summary>
public interface ICommandHandlerResult
{
    public ValidationResult ValidationResult { get; set; }
}

/// <summary>
/// CommandHandler result class
/// Only Id of struct and ValidationResult properties 
/// Preventing CommandHandlers from being used as QueryHandlers
/// </summary>
/// <typeparam name="TID"></typeparam>
public sealed record class CommandHandlerResult<TID> : ICommandHandlerResult
    where TID : struct
{
    /// <summary>
    /// Id of an entity class
    /// </summary>
    public TID Id { get; set; }

    /// <summary>
    /// Validation result
    /// </summary>
    public ValidationResult ValidationResult { get; set; }

    public CommandHandlerResult(ICommand<ICommandHandlerResult> command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        ValidationResult = command.Validate();
    }
}