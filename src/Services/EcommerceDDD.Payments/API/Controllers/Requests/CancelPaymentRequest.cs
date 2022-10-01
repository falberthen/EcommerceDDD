using System.ComponentModel.DataAnnotations;

namespace EcommerceDDD.Payments.API.Controllers.Requests;

public record class CancelPaymentRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    public int PaymentCancellationReason { get; init; }
}