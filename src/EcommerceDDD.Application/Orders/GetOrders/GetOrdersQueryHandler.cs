using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Application.Orders.GetOrderDetails;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.SharedKernel;
using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Domain.Customers;

namespace EcommerceDDD.Application.Orders.GetOrders
{
    public class GetOrdersQueryHandler : QueryHandler<GetOrdersQuery, 
        List<OrderDetailsViewModel>>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public GetOrdersQueryHandler(
            IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<List<OrderDetailsViewModel>> ExecuteQuery(GetOrdersQuery query, 
            CancellationToken cancellationToken)
        {
            List<OrderDetailsViewModel> viewModelList = new List<OrderDetailsViewModel>();

            var customerId = CustomerId.Of(query.CustomerId);
            var customer = await _unitOfWork.Customers
                .GetById(customerId, cancellationToken);

            if (customer == null)
                throw new InvalidDataException("Custumer not found.");

            foreach (var order in customer.Orders)
            {
                var payment = await _unitOfWork.Payments
                    .GetByOrderId(order.Id, cancellationToken);
                
                var productIds = order.OrderLines.
                    Select(p => p.ProductId).ToList();

                var products = await _unitOfWork.Products
                    .GetByIds(productIds, cancellationToken);

                OrderDetailsViewModel viewModel = new OrderDetailsViewModel();
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
                        ProductPrice = orderLine.ProductExchangePrice.Value,
                        ProductName = product.Name,
                        CurrencySymbol = currency.Symbol,
                    });
                }

                viewModel.CalculateTotalOrderPrice();
                viewModelList.Add(viewModel);
            }

            return viewModelList;
        }        
    }
}
