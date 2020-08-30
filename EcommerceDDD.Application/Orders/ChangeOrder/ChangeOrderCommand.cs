using EcommerceDDD.Application.Base.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Application.Orders.ChangeOrder
{
    public class ChangeOrderCommand : Command<CommandHandlerResult>
    {
        public Guid CustomerId { get; protected set; }
        public Guid OrderId { get; protected set; }
        public string Currency { get; protected set; }
        public List<ProductDto> Products { get; protected set; }

        public ChangeOrderCommand(Guid customerId, Guid orderId, List<ProductDto> products, string currency)
        {
            CustomerId = customerId;
            OrderId = orderId;
            Products = products;
            Currency = currency;
        }

        public override bool IsValid()
        {
            ValidationResult = new ChangeOrderCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
