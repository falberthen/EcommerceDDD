using MediatR;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Payments.Domain;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.IntegrationServices.Payments.Requests;
using EcommerceDDD.Payments.Application.RequestingPayment;

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
}
