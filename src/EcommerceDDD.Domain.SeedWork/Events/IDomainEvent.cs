using System;
using MediatR;

namespace EcommerceDDD.Domain.Core.Events;

public interface IDomainEvent : INotification
{
    DateTime CreatedAt { get; }
}