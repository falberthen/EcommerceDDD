using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Infrastructure.Messaging
{
    public interface IEventSerializer
    {
        string Serialize<TE>(TE @event) where TE : Event;
    }
}