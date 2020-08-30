using System;
using System.Collections.Generic;
using System.Text;
using EcommerceDDD.Application.Base.Commands;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : Command<CommandHandlerResult>
    {
        public Guid CustomerId { get; protected set; }

        public string Name { get; protected set; }

        public UpdateCustomerCommand(Guid customerId, string name)
        {
            CustomerId = customerId;
            Name = name;
        }

        public override bool IsValid()
        {
            ValidationResult = new UpdateCustomerCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
