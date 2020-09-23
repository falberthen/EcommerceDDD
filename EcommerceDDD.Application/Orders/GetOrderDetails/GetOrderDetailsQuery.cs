using System;
using BuildingBlocks.CQRS.QueryHandling;
using FluentValidation;
using FluentValidation.Results;

namespace EcommerceDDD.Application.Orders.GetOrderDetails
{
    public class GetOrderDetailsQuery : Query<OrderDetailsViewModel>
    {
        public Guid OrderId { get; set; }

        public GetOrderDetailsQuery(Guid orderId)
        {
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
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("OrderId is empty.");
        }
    }
}
