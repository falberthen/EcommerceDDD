namespace EcommerceDDD.Core.Infrastructure.Workers;

public abstract class BackgroundWorker(
    ILogger<BackgroundWorker> logger,
    Func<CancellationToken, Task> perform) : BackgroundService
{
    private readonly ILogger<BackgroundWorker> _logger = logger;
    private readonly Func<CancellationToken, Task> _perform = perform;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Background worker started...");

		try
		{
			await _perform(stoppingToken).ConfigureAwait(false);
		}
		catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
		{
			_logger.LogInformation("Background worker cancellation requested.");
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, "Unhandled exception in background worker.");
			throw;
		}
		finally
		{
			_logger.LogInformation("Background worker stopped...");
		}
	}
}