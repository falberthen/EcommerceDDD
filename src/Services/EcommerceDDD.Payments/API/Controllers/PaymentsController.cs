using System.Net;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Payments.Domain;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Payments.Domain.Commands;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Payments.API.Controllers.Requests;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Payments.API.Controllers;

[Authorize]
[Route("api/payments")]
[ApiController]
public class PaymentsController : CustomControllerBase
{
    public PaymentsController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> Create([FromBody] PaymentRequest request)
    {
        var command = RequestPayment.Create(
            CustomerId.Of(request.CustomerId),
            OrderId.Of(request.OrderId),
            Money.Of(request.TotalAmount, request.CurrencyCode),
            Currency.OfCode(request.CurrencyCode));

        return await Response(command);
    }

    [HttpDelete("{paymentId}")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> Cancel([FromRoute] Guid paymentId, [FromBody] CancelPaymentRequest request)
    {
        var command = CancelPayment.Create(
            PaymentId.Of(paymentId),
            (PaymentCancellationReason)request.PaymentCancellationReason);

        return await Response(command);
    }
}
