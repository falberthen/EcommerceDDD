using System;
using BuildingBlocks.CQRS.QueryHandling;
using FluentValidation;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Orders.GetOrderDetails
{
    public class GetOrderDetailsQuery : Query<OrderDetailsViewModel>
    {
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }

        public GetOrderDetailsQuery(Guid customerId, Guid orderId)
        {
            CustomerId = customerId;
            OrderId = orderId;
        }

        public override ValidationResult Validate()
        {
            return new GetOrderDetailsQueryValidator().Validate(this);
        }
    }

    public class GetOrderDetailsQueryValidator : AbstractValidator<GetOrderDetailsQuery>
    {
        public GetOrderDetailsQueryValidator()
        {
            RuleFor(x => x.CustomerId).NotEqual(Guid.Empty).WithMessage("CustomerId is empty.");
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("OrderId is empty.");
        }
    }
}
