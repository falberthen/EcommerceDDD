namespace EcommerceDDD.Core.Infrastructure.Outbox.Workers;

/// <summary>
/// Periodically purges processed outbox entries older than <see cref="RetentionDays"/> days.
/// Prevents <c>mt_doc_integrationevent</c> from growing unboundedly — Debezium reads via WAL
/// and never removes rows it has already forwarded to Kafka.
/// </summary>
public class OutboxCleanupWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxCleanupWorker> logger) : BackgroundService
{
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromHours(24);
    private const int RetentionDays = 7;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Initial delay: let infrastructure settle before the first cleanup attempt.
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox cleanup cycle failed.");
            }

            await Task.Delay(CleanupInterval, stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        var cutoff = DateTime.UtcNow.AddDays(-RetentionDays);
        session.DeleteWhere<IntegrationEvent>(e => e.CreatedAt < cutoff);
        await session.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Outbox cleanup: removed entries older than {Days} days (before {Cutoff:u}).",
            RetentionDays, cutoff);
    }
}
