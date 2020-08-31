using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Core.Messaging;
using Microsoft.Extensions.Logging;

namespace EcommerceDDD.Infrastructure.Messaging
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly IMessagePublisher _publisher;
        private readonly ILogger<MessageProcessor> _logger;

        public MessageProcessor(IEcommerceUnitOfWork unitOfWork, 
            IMessagePublisher publisher, ILogger<MessageProcessor> logger)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ProcessMessages(int batchSize, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching messages...");

            var messages = await _unitOfWork.MessageRepository.FetchUnprocessed(batchSize, cancellationToken);
            foreach (var message in messages)
            {
                try
                {
                    message.SetProcessedAt(DateTime.UtcNow);
                    await _publisher.Publish(message, cancellationToken);

                    _unitOfWork.MessageRepository.UpdateProcessedAt(message);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"an error has occurred while processing message {message.Id}: {ex.Message}");
                }
            }
        }
    }
}
