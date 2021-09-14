using EcommerceDDD.Domain.Quotes;
using EcommerceDDD.Domain.Core.Events;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Database.Context
{
    public sealed class EcommerceDDDContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<StoredEvent> StoredEvents { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public EcommerceDDDContext(DbContextOptions<EcommerceDDDContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<DomainEvent>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcommerceDDDContext).Assembly);
        }
    }
}