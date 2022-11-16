using MediatR;

namespace EcommerceDDD.Core.EventBus;

public interface IIntegrationEvent : INotification {
    public Guid Id { get; set; }
}
