using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Application.Orders.GetOrderDetails;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Shared;
using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Domain.Orders;

namespace EcommerceDDD.Application.Orders.GetOrders
{
    public class GetOrdersQueryHandler : QueryHandler<GetOrdersQuery, List<OrderDetailsViewModel>>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public GetOrdersQueryHandler(
            IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<List<OrderDetailsViewModel>> ExecuteQuery(GetOrdersQuery query, CancellationToken cancellationToken)
        {
            List<OrderDetailsViewModel> viewModelList = new List<OrderDetailsViewModel>();
            var orders = await _unitOfWork.OrderRepository.GetByCustomerId(query.CustomerId, cancellationToken);

            if (orders == null)
                throw new InvalidDataException("Orders not found.");

            foreach (var order in orders)
            {
                var payment = await _unitOfWork.PaymentRepository.GetByOrderId(order.Id, cancellationToken);                
                var productIds = order.OrderLines.Select(p => p.ProductId).ToList();
                var products = await _unitOfWork.ProductRepository.GetByIds(productIds, cancellationToken);

                OrderDetailsViewModel viewModel = new OrderDetailsViewModel();
                viewModel.OrderId = order.Id;
                viewModel.CreatedAt = order.CreatedAt.ToString();
                viewModel.Status = PrettifyOrderStatus(order.Status);

                foreach (var orderLine in order.OrderLines)
                {
                    var product = products.Single(p => p.Id == orderLine.ProductId);
                    var currency = Currency.FromCode(orderLine.ProductExchangePrice.CurrencyCode);

                    viewModel.OrderLines.Add(new OrderLinesDetailsViewModel
                    {
                        ProductId = orderLine.ProductId,
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

        private string PrettifyOrderStatus(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.ReadyToShip => "Ready to be shipped.",
                OrderStatus.WaitingForPayment => "Waiting for payment to be processed.",
                _ => status.ToString()
            };
        }
    }
}
