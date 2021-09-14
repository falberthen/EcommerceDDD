using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Quotes;
using BuildingBlocks.CQRS.CommandHandling;
using EcommerceDDD.Domain.Products;

namespace EcommerceDDD.Application.Quotes.SaveQuote
{
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
                throw new InvalidDataException("Quote not found.");

            if (product == null)
                throw new InvalidDataException("Product not found.");

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
}
