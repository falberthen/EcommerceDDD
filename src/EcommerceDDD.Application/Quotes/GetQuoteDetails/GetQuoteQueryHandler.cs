using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.SharedKernel;
using EcommerceDDD.Domain.Quotes;
using EcommerceDDD.Application.Core.CQRS.QueryHandling;
using EcommerceDDD.Application.Core.Exceptions;

namespace EcommerceDDD.Application.Quotes.GetQuoteDetails;

public class GetQuoteQueryHandler : QueryHandler<GetQuoteDetailsQuery, QuoteDetailsViewModel>
{
    private readonly IEcommerceUnitOfWork _unitOfWork;
    private readonly ICurrencyConverter _currencyConverter;

    public GetQuoteQueryHandler(IEcommerceUnitOfWork unitOfWork, ICurrencyConverter currencyConverter)
    {
        _unitOfWork = unitOfWork;
        _currencyConverter = currencyConverter;
    }

    public async override Task<QuoteDetailsViewModel> ExecuteQuery(GetQuoteDetailsQuery query, 
        CancellationToken cancellationToken)
    {
        QuoteDetailsViewModel viewModel = new QuoteDetailsViewModel();

        var quoteId = new QuoteId(query.QuoteId);
        var quote = await _unitOfWork.Quotes
            .GetById(quoteId, cancellationToken);

        if (quote == null)
            throw new ApplicationDataException("Quote not found.");

        if (string.IsNullOrWhiteSpace(query.Currency))
            throw new ApplicationDataException("Currency can't be empty.");

        if (quote.Items.Count > 0)
        {
            viewModel.QuoteId = quote.Id.Value;
            var currency = Currency.FromCode(query.Currency);
            var productIds = quote.Items.Select(p => p.ProductId).ToList();
            var products = await _unitOfWork.Products
                .GetByIds(productIds, cancellationToken);

            if (products == null)
                throw new ApplicationDataException("Products not found");

            foreach (var quoteItem in quote.Items)
            {
                var product = products.Single(p => p.Id == quoteItem.ProductId);
                var convertedPrice = _currencyConverter.Convert(currency, product.Price);
                viewModel.QuoteItems.Add(new QuoteItemDetailsViewModel
                {                        
                    ProductId = quoteItem.ProductId.Value,
                    ProductQuantity = quoteItem.Quantity,
                    ProductName = product.Name,
                    ProductPrice = Math.Round(convertedPrice.Value, 2),
                    CurrencySymbol = currency.Symbol,
                }); ;
            }

            viewModel.CalculateTotalOrderPrice();
        }

        return viewModel;
    }
}