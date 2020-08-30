using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Application.Base.Commands
{
    public interface ICommandHandler<in TCommand, TResult> :
        IRequestHandler<TCommand, CommandHandlerResult> where TCommand : ICommand<CommandHandlerResult> {}
}
