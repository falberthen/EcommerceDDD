using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.EventBus;
using Microsoft.Extensions.Logging;
using EcommerceDDD.Core.Infrastructure.Outbox;
using EcommerceDDD.Core.Infrastructure.Outbox.Persistence;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;

namespace EcommerceDDD.Core.Infrastructure.Tests.Outbox;

public class OutboxMessageServiceTests
{
    [Fact]
    public async Task SaveAsOutboxMessage_FromIntegrationEvent_ShouldCreateMessage()
    {
        // Given
        var integrationEvent = new DummyIntegrationEvent();
        integrationEvent.Id = Guid.NewGuid();
        integrationEvent.Text = "My dummy integration event";
        var service = new OutboxMessageService(_eventProducer.Object, _outboxMessageRepository.Object, _logger.Object);

        // When
        var outboxMessage = await service.SaveAsOutboxMessageAsync(integrationEvent);

        // Then
        outboxMessage.Should().NotBeNull();
        outboxMessage.ProcessedAt.Should().Be(null);
        outboxMessage.Payload.Should().NotBe(null);
        outboxMessage.Type.Should().Be(integrationEvent.GetType().Name);
    }

    [Fact]
    public async Task FetchUnprocessed_FromIntegrationEvents_ShouldReturnMessages()
    {
        // Given
        var integrationEvent = new DummyIntegrationEvent();
        integrationEvent.Id = Guid.NewGuid();
        integrationEvent.Text = "My dummy integration event";

        var service = new OutboxMessageService(_eventProducer.Object, _outboxMessageRepository.Object, _logger.Object);
        var outboxMessage = await service.SaveAsOutboxMessageAsync(integrationEvent);
        IReadOnlyCollection<OutboxMessage> repoMessages = new List<OutboxMessage>()
        {
            outboxMessage,
            outboxMessage,
            outboxMessage
        };

        _outboxMessageRepository.Setup(r => r.FetchUnprocessedMessagesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(repoMessages));

        // When
        var outboxMessages = await service.FetchUnprocessedMessagesAsync(100, CancellationToken.None);

        // Then
        outboxMessages.Should().NotBeNull();
        outboxMessages.Count().Should().Be(repoMessages.Count());
    }

    [Fact]
    public async Task ProcessMessage_FromIntegrationEvent_ShouldProcessMessage()
    {
        // Given
        var integrationEvent = new DummyIntegrationEvent();
        integrationEvent.Id = Guid.NewGuid();
        integrationEvent.Text = "My dummy integration event";
        var service = new OutboxMessageService(_eventProducer.Object, _outboxMessageRepository.Object, _logger.Object);
        var outboxMessage = await service.SaveAsOutboxMessageAsync(integrationEvent);

        // When
        await service.ProcessMessageAsync(outboxMessage, CancellationToken.None);

        // Then
        outboxMessage.ProcessedAt.Should().NotBeNull();
    }

    private Mock<IOutboxMessageRepository> _outboxMessageRepository = new();
    private Mock<IEventProducer> _eventProducer = new();
    private Mock<ILogger<OutboxMessageService>> _logger = new();
}