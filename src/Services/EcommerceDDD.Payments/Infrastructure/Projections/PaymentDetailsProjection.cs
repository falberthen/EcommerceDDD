using PaymentCompleted = EcommerceDDD.Payments.Domain.Events.PaymentCompleted;

namespace EcommerceDDD.Payments.Infrastructure.Projections;

public class PaymentDetailsProjection : SingleStreamProjection<PaymentDetails>
{
    public PaymentDetailsProjection()
    {
        ProjectEvent<PaymentCreated>((item, @event) => item.Apply(@event));
        ProjectEvent<PaymentCompleted>((item, @event) => item.Apply(@event));
        ProjectEvent<PaymentCanceled>((item, @event) => item.Apply(@event));
    }
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream