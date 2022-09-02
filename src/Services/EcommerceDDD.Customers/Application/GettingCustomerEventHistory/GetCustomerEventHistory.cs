using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Customers.Domain;

namespace EcommerceDDD.Customers.Application.GettingCustomerEventHistory;

public record class GetCustomerEventHistory(CustomerId CustomerId) : Query<IList<CustomerEventHistory>>
{
    public override ValidationResult Validate()
    {
        return new GetCustomerHistoryValidator().Validate(this);
    }
}

public class GetCustomerHistoryValidator : AbstractValidator<GetCustomerEventHistory>
{
    public GetCustomerHistoryValidator()
    {
        RuleFor(x => x.CustomerId.Value).NotEqual(Guid.Empty).WithMessage("Customer is empty.");
    }
}
