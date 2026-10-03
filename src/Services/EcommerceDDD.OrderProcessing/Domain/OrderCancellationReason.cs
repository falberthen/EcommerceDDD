
namespace EcommerceDDD.OrderProcessing.Domain;

public enum OrderCancellationReason
{
    [Description("Products out of stock")]
    ProductWasOutOfStock = 1,
    [Description("Customer reached store credit limit")]
    CustomerReachedStoreCreditLimit = 2,
    [Description("Shipment could not be delivered")]
    ShipmentNotDelivered = 3,
    [Description("Quote is no longer available")]
    QuoteUnavailable = 4
}