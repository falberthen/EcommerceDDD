﻿namespace EcommerceDDD.OrderProcessing.Application.Shipments.RequestingShipment;

public class RequestShipmentHandler(
    IIntegrationHttpService integrationHttpService,
    IEventStoreRepository<Order> orderWriteRepository,
    IConfiguration configuration) : ICommandHandler<RequestShipment>
{
    private readonly IIntegrationHttpService _integrationHttpService = integrationHttpService;
    private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository;
    private readonly IConfiguration _configuration = configuration;

    public async Task HandleAsync(RequestShipment command, CancellationToken cancellationToken)
    {
        var order = await _orderWriteRepository
            .FetchStreamAsync(command.OrderId.Value)
            ?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

        var productItemsRequest = order.OrderLines
            .Select(ol => new ProductItemRequest(
                ol.ProductItem.ProductId.Value,
                ol.ProductItem.ProductName,
                ol.ProductItem.UnitPrice.Amount,
                ol.ProductItem.Quantity))
            .ToList();

        // Requesting shippment
        var apiRoute = _configuration["ApiRoutes:ShipmentProcessing"];
        var response = await _integrationHttpService.PostAsync(apiRoute,
             new ShipOrderRequest(command.OrderId.Value, productItemsRequest));

        if (response?.Success == false)
            throw new ApplicationLogicException("An error occurred requesting shipping order.");
    }
}

public record class ShipOrderRequest(
    Guid OrderId,
    IReadOnlyList<ProductItemRequest> ProductItems);

public record ProductItemRequest(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity);