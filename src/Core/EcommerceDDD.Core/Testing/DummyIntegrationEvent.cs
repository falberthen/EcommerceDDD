using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Core.Testing;

public sealed class DummyIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
    public string Text { get; set; }
}