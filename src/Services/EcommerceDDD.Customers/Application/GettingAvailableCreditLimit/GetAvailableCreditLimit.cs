using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Customers.Application.GettingAvailableCreditLimit;

namespace EcommerceDDD.Customers.Api.Application.GettingAvailableCreditLimit;

public record class GetAvailableCreditLimit(Guid CustomerId) : Query<AvailableCreditLimitModel>
{
    public override ValidationResult Validate()
    {
        return new GetAvailableCreditLimitValidator().Validate(this);
    }
}

public class GetAvailableCreditLimitValidator : AbstractValidator<GetAvailableCreditLimit>
{
    public GetAvailableCreditLimitValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().NotEqual(Guid.Empty).WithMessage("CustomerId is empty.");
    }
}
