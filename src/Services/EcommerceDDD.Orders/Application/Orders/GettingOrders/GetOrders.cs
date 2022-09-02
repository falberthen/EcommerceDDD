using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Orders.Application.Orders.GettingOrders;

public record class GetOrders(Guid CustomerId) : Query<List<OrderViewModel>>
{
    public override ValidationResult Validate()
    {
        return new GetOrdersQueryValidator().Validate(this);
    }
}

public class GetOrdersQueryValidator : AbstractValidator<GetOrders>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(x => x.CustomerId).NotEqual(Guid.Empty).WithMessage("Customer is empty.");
    }
}