using System;
using System.Collections.Generic;
using System.Text;
using EcommerceDDD.Application.Base.Commands;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class PlaceOrderCommand : Command<CommandHandlerResult>
    {
        public Guid CustomerId { get; private set; }
        public List<ProductDto> Products { get; private set; }
        public string Currency { get; private set; }

        public PlaceOrderCommand(
            Guid customerId,
            List<ProductDto> products,
            string currency)
        {
            CustomerId = customerId;
            Products = products;
            Currency = currency;
        }

        public override bool IsValid()
        {
            ValidationResult = new PlaceOrderCommandValidator().Validate(this);
            return ValidationResult.IsValid; 
        }
    }
}
