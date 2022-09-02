using MediatR;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Shipments.Domain;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.IntegrationServices.Shipments.Requests;
using EcommerceDDD.Shipments.Application.ShippingPackage;

namespace EcommerceDDD.Shipments.API.Controllers;

[Authorize]
[Route("api/shipments")]
[ApiController]
public class ShipmentsController : CustomControllerBase
{
    public ShipmentsController(IMediator mediator)
        : base(mediator) { }

    [HttpPost]
    public async Task<IActionResult> RequestShipment([FromBody] ShipOrderRequest request)
    {
        var productItems = request.ProductItems.Select(p =>
            new ProductItem(
                ProductId.Of(p.ProductId),
                p.Quantity))
            .ToList();

        var command = new ShipPackage(OrderId.Of(request.OrderId), productItems);
        return await Response(command);
    }
}
