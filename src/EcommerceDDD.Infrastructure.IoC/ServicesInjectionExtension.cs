using System;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Core.Events;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Infrastructure.Domain;
using EcommerceDDD.Infrastructure.Domain.Customers;
using EcommerceDDD.Infrastructure.Domain.Products;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Infrastructure.Identity.Claims;
using EcommerceDDD.Infrastructure.Identity.IdentityUser;
using EcommerceDDD.Infrastructure.Identity.Services;
using EcommerceDDD.Application.Customers.RegisterCustomer;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Domain.SharedKernel;
using EcommerceDDD.Infrastructure.Domain.Carts;
using EcommerceDDD.Infrastructure.Domain.CurrencyExchange;
using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Application.Customers.DomainServices;
using EcommerceDDD.Domain.Customers.Orders;
using EcommerceDDD.Infrastructure.Events;

namespace EcommerceDDD.Infrastructure.IoC
{
    public static class ServicesInjectionExtension
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Domain services
            services.AddScoped<ICustomerUniquenessChecker, CustomerUniquenessChecker>();
            services.AddScoped<ICurrencyConverter, CurrencyConverter>();
            services.AddScoped<IOrderStatusWorkflow, OrderStatusWorkflow>();
            
            // Application - Handlers            
            services.AddMediatR(typeof(RegisterCustomerCommandHandler).GetTypeInfo().Assembly);

            // Infra - Domain persistence
            services.AddScoped<IEcommerceUnitOfWork, EcommerceUnitOfWork>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            // Infrastructure - Data EventSourcing
            services.AddScoped<IStoredEventRepository, StoredEventRepository>();
            services.AddSingleton<IEventSerializer, EventSerializer>();

            // Infrastructure - Identity     
            services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUser, User>();

            // Messaging
            services.AddScoped<IMessagePublisher, MessagePublisher>();
            services.AddScoped<IMessageProcessor, MessageProcessor>();
        }
    }
}
