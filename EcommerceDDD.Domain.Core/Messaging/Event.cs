using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Domain.Core.Messaging
{
    public abstract class Event : Message, INotification
    {
        public DateTime CreatedAt { get; private set; }
        protected Event()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
