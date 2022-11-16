using Marten.Events.Aggregation;
using EcommerceDDD.Payments.Domain.Events;

namespace EcommerceDDD.Payments.Infrastructure.Projections;

public class PaymentDetailsProjection : SingleStreamAggregation<PaymentDetails>
{
    public PaymentDetailsProjection()
    {
        ProjectEvent<PaymentCreated>((item, @event) => item.Apply(@event));
        ProjectEvent<PaymentCompleted>((item, @event) => item.Apply(@event));
        ProjectEvent<PaymentCanceled>((item, @event) => item.Apply(@event));
    }
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream