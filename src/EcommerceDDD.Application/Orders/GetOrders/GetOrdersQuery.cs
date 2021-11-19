using System.Collections.Generic;
using EcommerceDDD.Application.Orders.GetOrderDetails;
using EcommerceDDD.Application.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Application.Orders.GetOrders;

public record class GetOrdersQuery : Query<List<OrderDetailsViewModel>>
{
    public Guid CustomerId { get; init; }

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