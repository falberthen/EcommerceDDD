using EcommerceDDD.Domain.Quotes;
using EcommerceDDD.Domain.SeedWork;
using System;
using System.Linq.Expressions;

namespace EcommerceDDD.Domain.Orders.Specifications
{    
    public class IsOrderFromQuote : Specification<Order>
    {
        private QuoteId QuoteId;

        public IsOrderFromQuote(QuoteId quoteId)
        {
            QuoteId = quoteId;
        }

        public override Expression<Func<Order, bool>> ToExpression()
        {
            return order => order.QuoteId == QuoteId;
        }
    }
}
