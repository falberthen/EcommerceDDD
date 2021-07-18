using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.Core.Events;
using EcommerceDDD.Domain.Customers;
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
        public IStoredEvents StoredEvents { get; }
        public IProducts Products { get; }
        public ICarts Carts { get; }
        public IPayments Payments { get; }

        private readonly IEventSerializer _eventSerializer;

        public EcommerceUnitOfWork(EcommerceDDDContext dbContext,
            ICustomers customers,
            IStoredEvents storedEvents,
            IProducts products,
            IPayments payments,
            ICarts carts,
            IEventSerializer eventSerializer) : base(dbContext)
        {
            Customers = customers ?? throw new ArgumentNullException(nameof(customers));
            StoredEvents = storedEvents ?? throw new ArgumentNullException(nameof(storedEvents));
            Products = products ?? throw new ArgumentNullException(nameof(products));
            Carts = carts ?? throw new ArgumentNullException(nameof(carts));
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
                    .Select(e => 
                    StoredEventHelper.BuildFromDomainEvent(e as DomainEvent, _eventSerializer)).ToArray();

                entity.ClearDomainEvents();
                await DbContext.AddRangeAsync(messages, cancellationToken);
            }
        }
    }
}
