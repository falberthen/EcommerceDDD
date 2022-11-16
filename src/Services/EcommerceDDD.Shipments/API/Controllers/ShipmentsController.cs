using System.Net;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Shipments.Domain;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Shipments.API.Controllers.Requests;

namespace EcommerceDDD.Shipments.API.Controllers;

[Authorize]
[Route("api/shipments")]
[ApiController]
public class ShipmentsController : CustomControllerBase
{
    public ShipmentsController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> RequestShipment([FromBody] ShipOrderRequest request)
    {
        var productItems = request.ProductItems.Select(p =>
            new ProductItem(
                ProductId.Of(p.ProductId),
                p.Quantity))
            .ToList();

        var command = Domain.Commands.RequestShipment.Create(
            OrderId.Of(request.OrderId), 
            productItems);

        return await Response(command);
    }
}