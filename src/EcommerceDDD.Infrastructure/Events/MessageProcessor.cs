using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using Microsoft.Extensions.Logging;

namespace EcommerceDDD.Infrastructure.Events
{
    public interface IMessageProcessor
    {
        Task ProcessMessages(int batchSize, CancellationToken cancellationToken);
    }

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
            var messages = await _unitOfWork.StoredEvents.
                FetchUnprocessed(batchSize, cancellationToken);

            foreach (var message in messages)
            {
                try
                {                    
                    await _publisher.Publish(message, cancellationToken);

                    message.SetProcessedAt(DateTime.Now);
                    _unitOfWork.StoredEvents.UpdateProcessedAt(message);

                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation($"\n--- Message {message.Id} processed at {message.ProcessedAt}\n");
                }
                catch (Exception ex)
                {                    
                    _logger.LogError($"\n--- An error has occurred while processing message {message.Id}: {ex.Message}\n");
                }
            }
        }
    }    
}
