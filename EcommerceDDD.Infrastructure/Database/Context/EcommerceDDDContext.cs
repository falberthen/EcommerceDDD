using EcommerceDDD.Domain.Core.Messaging;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Customers.Orders;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Database.Context
{
    public sealed class EcommerceDDDContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StoredEvent> StoredEvents { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }

        public EcommerceDDDContext(DbContextOptions<EcommerceDDDContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Event>();
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new StoredMessageConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }
    }
}
