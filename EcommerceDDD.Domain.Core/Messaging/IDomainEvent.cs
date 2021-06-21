using MediatR;
using System;

namespace EcommerceDDD.Domain.Core.Messaging
{
    public interface IDomainEvent : INotification
    {
        DateTime CreatedAt { get; }
    }
}
