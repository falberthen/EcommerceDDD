using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Quotes;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Application.Core.CQRS.CommandHandling;
using EcommerceDDD.Application.Core.ExceptionHandling;

namespace EcommerceDDD.Application.Quotes.ChangeQuote;

public class ChangeQuoteCommandHandler : CommandHandler<ChangeQuoteCommand, Guid>
{
    private readonly IEcommerceUnitOfWork _unitOfWork;

    public ChangeQuoteCommandHandler(
        IEcommerceUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<Guid> ExecuteCommand(ChangeQuoteCommand command, 
        CancellationToken cancellationToken)
    {
        var quoteId = QuoteId.Of(command.QuoteId);
        var quote = await _unitOfWork.Quotes.
            GetById(quoteId, cancellationToken);

        var productId = ProductId.Of(command.Product.Id);
        var product = await _unitOfWork.Products
            .GetById(productId, cancellationToken);

        if (quote == null)
            throw new ApplicationDataException("Quote not found.");

        if (product == null)
            throw new ApplicationDataException("Product not found.");

        var quantity = command.Product.Quantity;
        var quotetemProductData = new QuoteItemProductData(
            product.Id, 
            product.Price, quantity
        );

        quote.ChangeItem(quotetemProductData);
            
        await _unitOfWork.CommitAsync();
        return quote.Id.Value;
    }
}
