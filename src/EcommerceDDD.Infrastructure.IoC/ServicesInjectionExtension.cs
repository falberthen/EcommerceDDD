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
using EcommerceDDD.Application.Customers.RegisterCustomer;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Domain.SharedKernel;
using EcommerceDDD.Infrastructure.Domain.Carts;
using EcommerceDDD.Infrastructure.Domain.CurrencyExchange;
using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Application.Customers.DomainServices;
using EcommerceDDD.Domain.Customers.Orders;
using EcommerceDDD.Infrastructure.Events;
using EcommerceDDD.Application.Orders;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Infrastructure.Identity.Users;
using EcommerceDDD.Infrastructure.Identity.Services;
using EcommerceDDD.Infrastructure.Identity.Claims;
using Microsoft.AspNetCore.Http;

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
            services.AddScoped<IOrderStatusBroadcaster, OrderStatusBroadcaster>();

            // Infra - Domain persistence
            services.AddScoped<IEcommerceUnitOfWork, EcommerceUnitOfWork>();
            services.AddScoped<ICustomers, Customers>();
            services.AddScoped<IProducts, Products>();
            services.AddScoped<ICarts, Carts>();
            services.AddScoped<IPayments, Payments>();

            // Infrastructure - Data EventSourcing
            services.AddScoped<IStoredEvents, StoredEvents>();
            services.AddSingleton<IEventSerializer, EventSerializer>();

            // Infrastructure - Identity     
            services.AddTransient<IAuthorizationHandler, ClaimsRequirementHandler>();
            services.AddTransient<IApplicationUserDbAccessor, ApplicationUserDbAccessor>();
            services.AddTransient<IJwtService, JwtService>();
            services.AddTransient<IApplicationUser, ApplicationUser>();
            services.AddHttpContextAccessor();

            // Messaging
            services.AddScoped<IMessagePublisher, MessagePublisher>();
            services.AddScoped<IMessageProcessor, MessageProcessor>();

            services.AddHttpContextAccessor();
        }
    }
}
