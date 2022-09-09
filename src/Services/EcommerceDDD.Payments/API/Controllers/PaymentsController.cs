using MediatR;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Payments.Domain;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.IntegrationServices.Payments.Requests;
using EcommerceDDD.Payments.Application.RequestingPayment;
using EcommerceDDD.Payments.Application.CancelingPayment;

namespace EcommerceDDD.Payments.API.Controllers;

[Authorize]
[Route("api/payments")]
[ApiController]
public class PaymentsController : CustomControllerBase
{
    public PaymentsController(IMediator mediator)
        : base(mediator) { }

    [HttpPost]
    public async Task<IActionResult> RequestPayment([FromBody] PaymentRequest request)
    {
        var command = new RequestPayment(
            CustomerId.Of(request.CustomerId),
            OrderId.Of(request.OrderId), 
            Money.Of(request.TotalAmount, request.currencyCode), 
            Currency.OfCode(request.currencyCode));

        return await Response(command);
    }

    [HttpDelete("{paymentId}")]
    public async Task<IActionResult> RequestCancelPayment([FromRoute] Guid paymentId, [FromBody] CancelPaymentRequest request)
    {
        var command = new CancelPayment(
            PaymentId.Of(paymentId),
            (PaymentCancellationReason)request.PaymentCancellationReason
        );

        return await Response(command);
    }
}
