using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.OrderProcessing.Application.Shipments.RequestingShipment;

public class RequestShipmentHandler(
	ApiGatewayClient apiGatewayClient,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<RequestShipment>
{
	private readonly ApiGatewayClient _apiGatewayClient = apiGatewayClient;
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository;

	public async Task HandleAsync(RequestShipment command, CancellationToken cancellationToken)
	{
		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken)
			?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

		var productItemsRequest = order.OrderLines
			.Select(ol => new ProductItemRequest()
			{
				ProductId = ol.ProductItem.ProductId.Value,
				ProductName = ol.ProductItem.ProductName,
				Quantity = ol.ProductItem.Quantity,
				UnitPrice = Convert.ToDouble(ol.ProductItem.UnitPrice.Amount)
			}).ToList();

		await RequestShipmentAsync(order.Id, productItemsRequest, cancellationToken);
	}

	public async Task RequestShipmentAsync(OrderId orderId, List<ProductItemRequest> productItemsRequest, CancellationToken cancellationToken)
	{
		try
		{
			// Requesting shippment	
			var shipOrderRequest = new ShipOrderRequest()
			{
				OrderId = orderId.Value,
				ProductItems = productItemsRequest
			};

			var shipmentsRequestBuilder = _apiGatewayClient.Api.Shipments;
			await shipmentsRequestBuilder
				.PostAsync(shipOrderRequest, cancellationToken: cancellationToken);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException(
				$"An error occurred requesting shipping order.");
		}
	}
}