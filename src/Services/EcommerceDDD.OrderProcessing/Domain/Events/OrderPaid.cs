namespace EcommerceDDD.OrderProcessing.Domain.Events;

public record OrderPaid(
    Guid OrderId,
    Guid PaymentId,
    IList<Guid> OrderLineProducts,
    string CurrencyCode,
    decimal TotalPaid) : DomainEvent;
