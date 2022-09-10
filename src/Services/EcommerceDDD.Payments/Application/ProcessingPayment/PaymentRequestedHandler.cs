using MediatR;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Payments.Domain.Events;
using EcommerceDDD.Core.Domain;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;
using EcommerceDDD.IntegrationServices.Customers;
using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Payments.Application.ProcessingPayment;

public class PaymentRequestedHandler : INotificationHandler<DomainEventNotification<PaymentRequested>>
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentRequestedHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(DomainEventNotification<PaymentRequested> notification, CancellationToken cancellationToken)
    {
        using var scopedService = _serviceProvider.CreateScope();
        var paymentWriteRepository = scopedService
           .ServiceProvider.GetRequiredService<IEventStoreRepository<Payment>>();
        
        var @event = notification.DomainEvent;

        var payment = await paymentWriteRepository
            .FetchStream(@event.PaymentId.Value);

        if (payment == null)
            throw new ApplicationException($"Cannot find payment {@event.PaymentId}.");

        // Checking customer's credit
        await CheckCustomerAvailableCredit(scopedService, @event, cancellationToken);

        // Recording payment processement
        payment.RecordProcessement();

        await paymentWriteRepository
            .AppendEventsAsync(payment);
    }

    private async Task CheckCustomerAvailableCredit(IServiceScope scopedService, PaymentRequested @event, CancellationToken cancellationToken)
    {
        var integrationServicesOptions = scopedService.ServiceProvider
            .GetRequiredService<IOptions<IntegrationServicesSettings>>();

        if (integrationServicesOptions == null)
            throw new ArgumentNullException(nameof(integrationServicesOptions));

        var customersService = scopedService.ServiceProvider
            .GetRequiredService<ICustomersService>();

        var eventProcuder = scopedService.ServiceProvider
            .GetRequiredService<IEventProducer>();

        var integrationServicesSettings = integrationServicesOptions.Value;

        var availableCreditLimit = await customersService
            .RequestAvailableCreditLimit(integrationServicesSettings.ApiGatewayBaseUrl, @event.CustomerId.Value);

        if(availableCreditLimit == null)
            throw new ApplicationException($"An error ocurred trying to obtain the credit limit for customer {@event.CustomerId.Value}");

        // Just comparing with the customer credit limit
        if(@event.TotalAmount.Value > availableCreditLimit.AvailableCreditLimit)
        {
            await eventProcuder
                .PublishAsync(new CustomerReachedCreditLimit(@event.OrderId.Value), cancellationToken);
            throw new ApplicationException($"Customer credit limit is not enough.");
        }
    }
}