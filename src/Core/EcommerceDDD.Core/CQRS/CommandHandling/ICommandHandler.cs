using MediatR;

namespace EcommerceDDD.Core.CQRS.CommandHandling;

public interface ICommandHandler<in T>: IRequestHandler<T>
    where T : ICommand {}