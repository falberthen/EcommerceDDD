using EcommerceDDD.Application.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Application.Orders.GetOrderDetails;

public record class GetOrderDetailsQuery : Query<OrderDetailsViewModel>
{
    public Guid CustomerId { get; init; }
    public Guid OrderId { get; init; }

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