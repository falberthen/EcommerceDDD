using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Core.EventBus;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IDomainEvent @event);
}