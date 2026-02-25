using ProductItemRequest = EcommerceDDD.ServiceClients.ShipmentProcessing.Models.ProductItemRequest;

namespace EcommerceDDD.OrderProcessing.Application.Shipments.RequestingShipment;

public class RequestShipmentHandler(
	ShipmentProcessingClient shipmentProcessingClient,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<RequestShipment>
{
	private readonly ShipmentProcessingClient _shipmentProcessingClient = shipmentProcessingClient
		?? throw new ArgumentNullException(nameof(shipmentProcessingClient));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task<Result> HandleAsync(RequestShipment command, CancellationToken cancellationToken)
	{
		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		var productItemsRequest = order.OrderLines
			.Select(ol => new ProductItemRequest()
			{
				ProductId = ol.ProductItem.ProductId.Value,
				ProductName = ol.ProductItem.ProductName,
				Quantity = ol.ProductItem.Quantity,
				UnitPrice = Convert.ToDouble(ol.ProductItem.UnitPrice.Amount)
			}).ToList();

		return await RequestShipmentAsync(order.Id, productItemsRequest, cancellationToken);
	}

	private async Task<Result> RequestShipmentAsync(OrderId orderId, List<ProductItemRequest> productItemsRequest, CancellationToken cancellationToken)
	{
		try
		{
			var shipOrderRequest = new ShipOrderRequest()
			{
				OrderId = orderId.Value,
				ProductItems = productItemsRequest
			};

			var shipmentsRequestBuilder = _shipmentProcessingClient.Api.V2.Internal.Shipments;
			await shipmentsRequestBuilder
				.PostAsync(shipOrderRequest, cancellationToken: cancellationToken);

			return Result.Ok();
		}
		catch (Microsoft.Kiota.Abstractions.ApiException)
		{
			return Result.Fail("An error occurred requesting shipping order.");
		}
	}
}
