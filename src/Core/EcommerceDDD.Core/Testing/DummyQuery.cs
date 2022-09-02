using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Core.Testing;

public record class DummyQuery(DummyAggregateId Id) : Query<DummyAggregateRoot>
{
    public override ValidationResult Validate()
    {
        return new DummyQueryValidator().Validate(this);
    }
}

public class DummyQueryValidator : AbstractValidator<DummyQuery>
{
    public DummyQueryValidator()
    {
        RuleFor(x => x.Id.Value).NotEqual(Guid.Empty).WithMessage("Id is empty.");
    }
}