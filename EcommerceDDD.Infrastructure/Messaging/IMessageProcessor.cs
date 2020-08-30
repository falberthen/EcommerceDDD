using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Infrastructure.Messaging
{
    public interface IMessageProcessor
    {
        Task ProcessMessages(int batchSize, CancellationToken cancellationToken);
    }
}