using System;
using System.Collections.Generic;
using BuildingBlocks.CQRS.QueryHandling;
using FluentValidation;
using FluentValidation.Results;
using EcommerceDDD.Application.EventSourcing.StoredEventsData;

namespace EcommerceDDD.Application.Customers.ListCustomerStoredEvents
{
    public class ListCustomerStoredEventsQuery : Query<IList<CustomerStoredEventData>>
    {
        public Guid CustomerId { get; }

        public ListCustomerStoredEventsQuery(Guid customerId)
        {
            CustomerId = customerId;
        }

        public override ValidationResult Validate()
        {
            return new ListCustomerStoredEventsQueryValidator().Validate(this);
        }
    }

    public class ListCustomerStoredEventsQueryValidator : AbstractValidator<ListCustomerStoredEventsQuery>
    {
        public ListCustomerStoredEventsQueryValidator()
        {
            RuleFor(x => x.CustomerId).NotEqual(Guid.Empty).WithMessage("CustomerId is empty.");
        }
    }
}
