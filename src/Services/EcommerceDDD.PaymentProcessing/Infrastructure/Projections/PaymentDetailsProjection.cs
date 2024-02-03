using PaymentCompleted = EcommerceDDD.PaymentProcessing.Domain.Events.PaymentCompleted;

namespace EcommerceDDD.PaymentProcessing.Infrastructure.Projections;

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