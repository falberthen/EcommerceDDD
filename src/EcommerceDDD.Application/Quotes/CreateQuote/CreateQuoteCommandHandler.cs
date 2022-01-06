using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Quotes;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Application.Core.Exceptions;
using EcommerceDDD.Application.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Application.Quotes.SaveQuote;

public class CreateQuoteCommandHandler : CommandHandler<CreateQuoteCommand, Guid>
{
    private readonly IEcommerceUnitOfWork _unitOfWork;

    public CreateQuoteCommandHandler(
        IEcommerceUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<Guid> ExecuteCommand(CreateQuoteCommand command, 
        CancellationToken cancellationToken)
    {
        var customerId = new CustomerId(command.CustomerId);
        var customer = await _unitOfWork.Customers
            .GetById(customerId, cancellationToken);

        var productId = new ProductId(command.Product.Id);
        var product = await _unitOfWork.Products
            .GetById(productId, cancellationToken);

        if (customer == null)
            throw new ApplicationDataException("Customer not found.");

        if (product == null)
            throw new ApplicationDataException("Product not found.");

        var quantity = command.Product.Quantity;
        var quotetemProductData = new QuoteItemProductData(
            product.Id, 
            product.Price, quantity
        );

        var quote = Quote.CreateNew(customerId);
        quote.AddItem(quotetemProductData);

        await _unitOfWork.Quotes
            .Add(quote, cancellationToken);
   
        await _unitOfWork.CommitAsync();
        return quote.Id.Value;
    }
}