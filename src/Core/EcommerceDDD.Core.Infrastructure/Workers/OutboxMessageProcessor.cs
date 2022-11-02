using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;

namespace EcommerceDDD.Core.Infrastructure.Workers;

public class OutboxMessageProcessor : BackgroundService
{
    private const int _batchSize = 100;
    private TimeSpan _interval = TimeSpan.FromSeconds(10);
    private readonly ILogger<OutboxMessageProcessor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public OutboxMessageProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxMessageProcessor> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting outbox message worker...");

            using var scope = _scopeFactory.CreateScope();
            var messageService = scope.ServiceProvider
                .GetRequiredService<IOutboxMessageService>();

            while (!cancellationToken.IsCancellationRequested)
            {
                var messages = await messageService
                    .FetchUnprocessedMessagesAsync(_batchSize, cancellationToken);

                foreach (var message in messages)
                    await messageService.ProcessMessageAsync(message, cancellationToken);

                await Task.Delay(_interval, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError("An error occurred when processing message: {Message} {StackTrace}", e.Message, e.StackTrace);
        }
    }
}
