using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using BuildingBlocks.CQRS.QueryHandling;
using System;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Orders.Specifications;

namespace EcommerceDDD.Application.Quotes.GetCurrentQuote
{
    public class GetCurrentQuoteQueryHandler : QueryHandler<GetCurrentQuoteQuery, Guid>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public GetCurrentQuoteQueryHandler(IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async override Task<Guid> ExecuteQuery(GetCurrentQuoteQuery query, 
            CancellationToken cancellationToken)
        {
            var customerId = CustomerId.Of(query.CustomerId);
            var currentQuote = await _unitOfWork.Quotes
                .GetCurrentQuote(customerId, cancellationToken);

            if(currentQuote != null)
            {
                // Checking if the quote was ordered
                var isOrderFromQuote = new IsOrderFromQuote(currentQuote.Id);
                var ordersOfQuote = await _unitOfWork.Orders.Find(isOrderFromQuote);

                if(ordersOfQuote.Count == 0)
                   return currentQuote.Id.Value;
            }
            
            return Guid.Empty;
        }
    }
}
