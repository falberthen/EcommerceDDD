using MediatR;

namespace EcommerceDDD.Core.CQRS.QueryHandling;

public interface IQuery<out TResponse> : IRequest<TResponse> {}