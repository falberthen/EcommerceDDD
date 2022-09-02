using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Customers.Application.GettingCustomerDetails;

namespace EcommerceDDD.Customers.Api.Application.GettingCustomerDetails;

public record class GetCustomerDetails(string Token) : Query<CustomerDetails>
{
    public override ValidationResult Validate()
    {
        return new GetCustomerDetailsValidator().Validate(this);
    }
}

public class GetCustomerDetailsValidator : AbstractValidator<GetCustomerDetails>
{
    public GetCustomerDetailsValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage("Token is empty.");
    }
}
