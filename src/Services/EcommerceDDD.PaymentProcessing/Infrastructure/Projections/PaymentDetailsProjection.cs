using PaymentCompleted = EcommerceDDD.PaymentProcessing.Domain.Events.PaymentCompleted;

namespace EcommerceDDD.PaymentProcessing.Infrastructure.Projections;

public partial class PaymentDetailsProjection : SingleStreamProjection<PaymentDetails, Guid>
{
    public static void Apply(PaymentDetails item, PaymentCreated @event) => item.Apply(@event);
    public static void Apply(PaymentDetails item, PaymentCompleted @event) => item.Apply(@event);
    public static void Apply(PaymentDetails item, PaymentCanceled @event) => item.Apply(@event);
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream