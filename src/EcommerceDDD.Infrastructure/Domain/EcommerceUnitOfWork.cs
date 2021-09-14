using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Quotes;
using EcommerceDDD.Domain.Core.Events;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Infrastructure.Database.Context;
using EcommerceDDD.Infrastructure.Events;

namespace EcommerceDDD.Infrastructure.Domain
{
    public class EcommerceUnitOfWork : UnitOfWork<EcommerceDDDContext>, IEcommerceUnitOfWork
    {
        public ICustomers Customers { get; }
        public IOrders Orders { get; }
        public IStoredEvents StoredEvents { get; }
        public IProducts Products { get; }
        public IQuotes Quotes { get; }
        public IPayments Payments { get; }

        private readonly IEventSerializer _eventSerializer;

        public EcommerceUnitOfWork(EcommerceDDDContext dbContext,
            ICustomers customers,
            IOrders orders,
            IStoredEvents storedEvents,
            IProducts products,
            IPayments payments,
            IQuotes quotes,
            IEventSerializer eventSerializer) : base(dbContext)
        {
            Customers = customers ?? throw new ArgumentNullException(nameof(customers));
            Orders = orders ?? throw new ArgumentNullException(nameof(orders));
            StoredEvents = storedEvents ?? throw new ArgumentNullException(nameof(storedEvents));
            Products = products ?? throw new ArgumentNullException(nameof(products));
            Quotes = quotes ?? throw new ArgumentNullException(nameof(quotes));
            Payments = payments ?? throw new ArgumentNullException(nameof(payments));

            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
        }

        protected async override Task StoreEvents(CancellationToken cancellationToken)
        {
            var entities = DbContext.ChangeTracker.Entries()
                .Where(e => e.Entity is IAggregateRoot c && c.DomainEvents != null)
                .Select(e => e.Entity as IAggregateRoot)
                .ToArray();

            foreach (var entity in entities)
            {
                var messages = entity.DomainEvents
                    .Select(e => StoredEventHelper.BuildFromDomainEvent(e as DomainEvent, _eventSerializer))
                    .ToArray();

                entity.ClearDomainEvents();
                await DbContext.AddRangeAsync(messages, cancellationToken);
            }
        }
    }
}
