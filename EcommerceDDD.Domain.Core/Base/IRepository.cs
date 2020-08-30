using System;

namespace EcommerceDDD.Domain.Core.Base
{
    public interface IRepository<T> where T : IAggregateRoot
    {
    }
}