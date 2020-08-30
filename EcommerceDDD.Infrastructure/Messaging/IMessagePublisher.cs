using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Infrastructure.Messaging
{
    public interface IMessagePublisher
    {
        Task Publish(StoredEvent message, System.Threading.CancellationToken cancellationToken);
    }
}
