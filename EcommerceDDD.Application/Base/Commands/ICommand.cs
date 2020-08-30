using FluentValidation.Results;
using MediatR;
using System;

namespace EcommerceDDD.Application.Base.Commands
{
    public interface ICommand : IRequest
    {
        ValidationResult ValidationResult { get; set; }
        bool IsValid();
    }

    public interface ICommand<out TResult> : IRequest<CommandHandlerResult>
    {
        ValidationResult ValidationResult { get; set; }
        bool IsValid();
    }
}