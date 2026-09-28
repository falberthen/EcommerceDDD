namespace EcommerceDDD.OrderProcessing.Application.Orders.CancelingOrder;

public class CancelOrderHandler(
	IOrderNotificationService orderNotificationService,
	IProductInventoryHandler productInventoryHandler,
	IEventStoreRepository<Order> orderWriteRepository
)
{
	private readonly IOrderNotificationService _orderNotificationService = orderNotificationService
		?? throw new ArgumentNullException(nameof(orderNotificationService));
	private readonly IProductInventoryHandler _productInventoryHandler = productInventoryHandler
		?? throw new ArgumentNullException(nameof(productInventoryHandler));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task<Result> HandleAsync(CancelOrder command, CancellationToken cancellationToken)
	{
		var order = await _orderWriteRepository
			.FetchForWritingAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		if (order.Status == OrderStatus.Canceled)
			return Result.Ok();

		// Stock is decreased when the order is processed.		
		// The inventory is increased back only by what an order took.
		// A retry after a failed commit never restocks twice nor loses the restock.
		if (order.OrderLines is { Count: > 0 })
			await _productInventoryHandler.IncreaseQuantityInStockAsync(
				order.OrderLines.Select(ol => new ProductItemData
				{
					ProductId = ol.ProductItem.ProductId,
					ProductName = ol.ProductItem.ProductName,
					Quantity = ol.ProductItem.Quantity,
					UnitPrice = ol.ProductItem.UnitPrice
				}).ToList(),
				order.Id,
				cancellationToken);

		order.Cancel(command.CancellationReason);

		var orderCanceledEvent = order.GetUncommittedEvents()
			.OfType<OrderCanceled>()
			.FirstOrDefault();

		// Committed with the order, so the saga is guaranteed to cancel the payment when the order had already been paid
		await _orderWriteRepository
			.AppendEventsAndCommitAsync(order, cancellationToken, orderCanceledEvent!);

		await _orderNotificationService.UpdateOrderStatusAsync(
			order.CustomerId.Value,
			command.OrderId.Value,
			order.Status.ToString(),
			(int)order.Status,
			cancellationToken);

		return Result.Ok();
	}
}
