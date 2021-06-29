using System;

namespace EcommerceDDD.Domain.Core.Base
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
