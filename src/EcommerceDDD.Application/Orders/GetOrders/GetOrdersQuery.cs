using System;
using System.Collections.Generic;
using FluentValidation.Results;
using FluentValidation;
using EcommerceDDD.Application.Orders.GetOrderDetails;
using EcommerceDDD.Application.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Application.Orders.GetOrders
{
    public class GetOrdersQuery : Query<List<OrderDetailsViewModel>>
    {
        public Guid CustomerId { get; set; }

        public GetOrdersQuery(Guid customerId)
        {
            CustomerId = customerId;
        }

        public override ValidationResult Validate()
        {
            return new GetOrdersQueryValidator().Validate(this);
        }
    }

    public class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
    {
        public GetOrdersQueryValidator()
        {
            RuleFor(x => x.CustomerId).NotEqual(Guid.Empty).WithMessage("CustomerId is empty.");
        }
    }
}
