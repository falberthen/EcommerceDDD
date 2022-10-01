using MediatR;

namespace EcommerceDDD.Core.Domain;

public class DomainNotification<TDomainEvent> : INotification
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; }

    public DomainNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
}