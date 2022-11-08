using Polly;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;

namespace EcommerceDDD.Core.Infrastructure.Outbox.Workers;

public interface IOutboxMessageProcessor
{
    Task StartProcessingAsync(CancellationToken cancellationToken = default);
}

public class OutboxMessageProcessor : IOutboxMessageProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OutboxSettings _outboxSettings;
    private readonly ILogger<OutboxMessageProcessor> _logger;

    public OutboxMessageProcessor(
        IServiceScopeFactory scopeFactory,
        IOptions<OutboxSettings> outboxSettings,
        ILogger<OutboxMessageProcessor> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _outboxSettings = outboxSettings.Value ?? throw new ArgumentNullException(nameof(outboxSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        var policy = Policy
           .Handle<Exception>()
           .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        await policy.ExecuteAsync(
           async () =>
           {
               _logger.LogInformation("Starting outbox message processor...");

               using var scope = _scopeFactory.CreateScope();
               var messageService = scope.ServiceProvider
                   .GetRequiredService<IOutboxMessageService>();

               while (!cancellationToken.IsCancellationRequested)
               {
                   var messages = await messageService
                       .FetchUnprocessedMessagesAsync(_outboxSettings.BatchSize, cancellationToken);

                   foreach (var message in messages)
                       await messageService.ProcessMessageAsync(message, cancellationToken);

                   await Task.Delay(TimeSpan.FromSeconds(_outboxSettings.Interval), cancellationToken);
               }
           });
    }
}
