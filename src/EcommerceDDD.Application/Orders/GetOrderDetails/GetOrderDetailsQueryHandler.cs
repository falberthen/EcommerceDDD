using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.SharedKernel;
using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Domain.Orders;

namespace EcommerceDDD.Application.Orders.GetOrderDetails
{
    public class GetOrderDetailsQueryHandler : QueryHandler<GetOrderDetailsQuery, OrderDetailsViewModel>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public GetOrderDetailsQueryHandler(
            IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<OrderDetailsViewModel> ExecuteQuery(GetOrderDetailsQuery query, 
            CancellationToken cancellationToken)
        {
            OrderDetailsViewModel viewModel = new OrderDetailsViewModel();
            var orderId = OrderId.Of(query.OrderId);
            var order = await _unitOfWork.Orders
                .GetById(orderId, cancellationToken);
            
            if (order == null)
                throw new InvalidDataException("Order not found.");

            var productIds = order.OrderLines
                .Select(p => p.ProductId)
                .ToList();

            var products = await _unitOfWork.Products
                .GetByIds(productIds, cancellationToken);

            if (products == null)
                throw new InvalidDataException("Products not found");

            viewModel.OrderId = order.Id.Value;
            viewModel.CreatedAt = order.CreatedAt.ToString();
            viewModel.Status = OrderStatusPrettier.Prettify(order.Status);

            foreach (var orderLine in order.OrderLines)
            {
                var product = products.Single(
                    (System.Func<Domain.Products.Product, bool>)
                    (p => p.Id == orderLine.ProductId));

                var currency = Currency
                    .FromCode(orderLine.ProductExchangePrice.CurrencyCode);

                viewModel.OrderLines.Add(new OrderLinesDetailsViewModel
                {
                    ProductId = orderLine.ProductId.Value,                    
                    ProductQuantity = orderLine.Quantity,
                    ProductName = product.Name,
                    ProductPrice = orderLine.ProductExchangePrice.Value,
                    CurrencySymbol = currency.Symbol,
                });
            }

            viewModel.CalculateTotalOrderPrice();
            return viewModel;
        }
    }    
}
