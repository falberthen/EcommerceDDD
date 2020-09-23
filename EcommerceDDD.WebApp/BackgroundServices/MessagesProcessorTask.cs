using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EcommerceDDD.WebApp.BackgroundServices
{
    public class MessagesProcessorTask : BackgroundService
    {
        private readonly ILogger<MessagesProcessorTask> _logger;
        private readonly MessageProcessorTaskOptions _options;
        private readonly IServiceScopeFactory _scopeFactory;

        public MessagesProcessorTask(
            MessageProcessorTaskOptions options, 
            IServiceScopeFactory scopeFactory,
            ILogger<MessagesProcessorTask> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("\n--------Processing messages:\n");

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetRequiredService<IMessageProcessor>();
                    await processor.ProcessMessages(_options.BatchSize, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.IntervalOnSeconds), cancellationToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
