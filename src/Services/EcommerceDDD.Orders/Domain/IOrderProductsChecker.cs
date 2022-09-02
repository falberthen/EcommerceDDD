using EcommerceDDD.Orders.Application.Quotes;

namespace EcommerceDDD.Orders.Domain
{
    public interface IOrderProductsChecker
    {
        Task CheckFromQuote(ConfirmedQuote confirmedQuote);
    }
}