using System;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using EcommerceDDD.Application.Core.CQRS.QueryHandling;
using EcommerceDDD.Application.Core.EventSourcing.StoredEventsData;

namespace EcommerceDDD.Application.Orders.ListOrderStoredEvents
{
    public class ListOrderStoredEventsQuery : Query<IList<StoredEventData>>
    {
        public Guid OrderId { get; }

        public ListOrderStoredEventsQuery(Guid orderId)
        {
            OrderId = orderId;
        }

        public override ValidationResult Validate()
        {
            return new ListOrderStoredEventsQueryValidator().Validate(this);
        }
    }

    public class ListOrderStoredEventsQueryValidator : AbstractValidator<ListOrderStoredEventsQuery>
    {
        public ListOrderStoredEventsQueryValidator()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("OrderId is empty.");
        }
    }
}
