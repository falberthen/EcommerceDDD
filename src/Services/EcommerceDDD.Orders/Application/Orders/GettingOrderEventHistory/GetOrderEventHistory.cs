using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Orders.Application.GettingOrderEventHistory;

public record class GetOrderEventHistory(Guid OrderId) : Query<IList<OrderEventHistory>>
{
    public override ValidationResult Validate()
    {
        return new GetOrderEventHistoryValidator().Validate(this);
    }
}

public class GetOrderEventHistoryValidator : AbstractValidator<GetOrderEventHistory>
{
    public GetOrderEventHistoryValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("OrderId is empty.");
    }
}
