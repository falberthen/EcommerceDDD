using System;

namespace EcommerceDDD.Domain.SeedWork
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
