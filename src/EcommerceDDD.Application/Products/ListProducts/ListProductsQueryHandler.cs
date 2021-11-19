using System;
using System.Threading;
using EcommerceDDD.Domain;
using System.Threading.Tasks;
using System.Collections.Generic;
using EcommerceDDD.Domain.SharedKernel;
using EcommerceDDD.Application.Products;
using EcommerceDDD.Application.Products.ListProducts;
using EcommerceDDD.Application.Core.CQRS.QueryHandling;
using EcommerceDDD.Application.Core.ExceptionHandling;

namespace EcommerceDDD.Application.Customers.ListCustomerEventHistory;

public class ListProductsQueryHandler : QueryHandler<ListProductsQuery, IList<ProductViewModel>> 
{
    private readonly IEcommerceUnitOfWork _unitOfWork;
    private readonly ICurrencyConverter _currencyConverter;

    public ListProductsQueryHandler(
        IEcommerceUnitOfWork unitOfWork,
        ICurrencyConverter currencyConverter)
    {
        _unitOfWork = unitOfWork;
        _currencyConverter = currencyConverter;
    }

    public override async Task<IList<ProductViewModel>> ExecuteQuery(ListProductsQuery query, 
        CancellationToken cancellationToken)
    {
        IList<ProductViewModel> productsViewModel = new List<ProductViewModel>();
        var products = await _unitOfWork.Products
            .ListAll(cancellationToken);

        if (string.IsNullOrEmpty(query.Currency))
            throw new ApplicationDataException("Currency code cannot be empty.");

        var currency = Currency.FromCode(query.Currency);
        foreach (var product in products)
        {
            var convertedPrice = _currencyConverter
                .Convert(currency, product.Price);

            productsViewModel.Add(new ProductViewModel
            {
                Id = product.Id.Value,
                Name = product.Name,
                Price = Math.Round(convertedPrice.Value, 2).ToString(),
                CurrencySymbol = currency.Symbol
            });
        }

        return productsViewModel;
    }
}
