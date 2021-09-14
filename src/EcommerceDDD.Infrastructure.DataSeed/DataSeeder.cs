using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Core.Events;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.Infrastructure.Database.Context;
using EcommerceDDD.Infrastructure.Domain.Customers;
using EcommerceDDD.Infrastructure.Domain.Products;
using EcommerceDDD.Infrastructure.Domain;
using Microsoft.EntityFrameworkCore;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Infrastructure.Domain.Quotes;
using EcommerceDDD.Domain.Quotes;
using EcommerceDDD.Infrastructure.Domain.CurrencyExchange;
using EcommerceDDD.Domain.SharedKernel;
using EcommerceDDD.Infrastructure.Events;

namespace EcommerceDDD.Infrastructure.DataSeed
{
    class DataSeeder
    {
        private static IServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            RegisterServices();

            var unitOfWork = _serviceProvider.GetService<IEcommerceUnitOfWork>();
            var currencyConverter = _serviceProvider.GetService<ICurrencyConverter>();
            await SeedProducts.SeedData(unitOfWork, currencyConverter);

            DisposeServices();
            Console.Read();
        }

        private static void RegisterServices()
        {            
            var services = new ServiceCollection();
            var connString = GetDbConnection();
            services.AddDbContext<EcommerceDDDContext>(options =>
            options.UseSqlServer(connString));

            services.AddScoped<IEcommerceUnitOfWork, EcommerceUnitOfWork>();
            services.AddScoped<ICustomers, Customers>();
            services.AddScoped<IQuotes, Quotes>();
            services.AddScoped<IProducts, Products>();
            services.AddScoped<IPayments, Payments>();
            services.AddScoped<IStoredEvents, StoredEvents>();
            services.AddScoped<IEventSerializer, EventSerializer>();
            services.AddScoped<ICurrencyConverter, CurrencyConverter>();
            services.AddScoped<EcommerceDDDContext>();
            
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
                return;
            if (_serviceProvider is IDisposable)
                ((IDisposable)_serviceProvider).Dispose();
        }

        private static string GetDbConnection()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            string strConnection = builder.Build().GetConnectionString("DefaultConnection");

            return strConnection;
        }
    }
}
