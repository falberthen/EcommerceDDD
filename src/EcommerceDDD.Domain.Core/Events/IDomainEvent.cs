using MediatR;
using System;

namespace EcommerceDDD.Domain.Core.Events
{
    public interface IDomainEvent : INotification
    {
        DateTime CreatedAt { get; }
    }
}
