﻿namespace EcommerceDDD.OrderProcessing.Application.Payments.RequestingPayment;

public class RequestPaymentHandler(
    IIntegrationHttpService integrationHttpService,
    IOrderStatusBroadcaster orderStatusBroadcaster,
    IEventStoreRepository<Order> orderWriteRepository,
    IConfiguration configuration) : ICommandHandler<RequestPayment>
{
    private readonly IIntegrationHttpService _integrationHttpService = integrationHttpService;
    private readonly IOrderStatusBroadcaster _orderStatusBroadcaster = orderStatusBroadcaster;
    private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository;
    private readonly IConfiguration _configuration = configuration;

    public async Task HandleAsync(RequestPayment command, CancellationToken cancellationToken)
    {
        var order = await _orderWriteRepository
            .FetchStreamAsync(command.OrderId.Value)
            ?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

        var apiRoute = _configuration["ApiRoutes:PaymentProcessing"];
        var response = await _integrationHttpService.PostAsync(
            apiRoute,
            new PaymentRequest(
                command.CustomerId.Value,
                command.OrderId.Value,
                command.TotalPrice.Amount,
                command.Currency.Code)
            );

        if (response?.Success == false)
            throw new ApplicationLogicException($"An error occurred requesting payment for order {command.OrderId}.");

        // Updating order status on the UI with SignalR
        await _orderStatusBroadcaster.UpdateOrderStatus(
            new UpdateOrderStatusRequest(
                command.CustomerId.Value,
                command.OrderId.Value,
                order.Status.ToString(),
                (int)order.Status));
    }
}

public record class PaymentRequest(
    Guid CustomerId,
    Guid OrderId,
    decimal TotalAmount,
    string currencyCode);
