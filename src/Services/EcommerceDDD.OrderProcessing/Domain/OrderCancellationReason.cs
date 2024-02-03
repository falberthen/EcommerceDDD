
namespace EcommerceDDD.OrderProcessing.Domain;

public enum OrderCancellationReason
{
    [Description("Processment Error")]
    ProcessmentError = 0,
    [Description("Canceled by Customer")]
    CanceledByCustomer = 1,
    [Description("Products out of stock")]
    ProductWasOutOfStock = 2,
    [Description("Customer reached credit limit")]
    CustomerReachedCreditLimit = 3,
    [Description("Payment failed")]
    PaymentFailed = 4,
    [Description("Shipment failed")]
    ShipmentFailed = 5
}