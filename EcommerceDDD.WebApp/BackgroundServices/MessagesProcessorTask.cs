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

        public MessagesProcessorTask(MessageProcessorTaskOptions options, 
            ILogger<MessagesProcessorTask> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting message processor...");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Processing new messages...");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetRequiredService<IMessageProcessor>();
                    await processor.ProcessMessages(_options.BatchSize, stoppingToken);
                }

                _logger.LogInformation($"Messages processed, next execution in {_options.Interval}.");

                await Task.Delay(_options.Interval, stoppingToken);
            }
        }
    }
}
