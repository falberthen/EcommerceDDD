using FluentValidation.Results;
using MediatR;

namespace EcommerceDDD.Core.CQRS.CommandHandling;

public interface ICommand: IRequest 
{
    ValidationResult ValidationResult { get; init; }
    ValidationResult Validate();
}