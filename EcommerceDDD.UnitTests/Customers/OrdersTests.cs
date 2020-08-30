using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.CurrencyExchange;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Customers.Orders;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.Shared;
using NSubstitute;
using NUnit.Framework;

namespace EcommerceDDD.UnitTests
{
    [TestFixture]
    public class OrdersTests
    {
        const string name = "Customer";
        const string email = "test@email.com";        

        [Test]
        public void Order_Cannot_Be_Placed_Without_Products()
        {
            // Arrange
            var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
            var currencyConverter = Substitute.For<ICurrencyConverter>();
            customerUniquenessChecker.IsUserUnique(email).Returns(true);
            var customer = Customer.CreateCustomer(email, name, customerUniquenessChecker);
            Basket cart = new Basket(Currency.USDollar.Name);

            // Assert
            var businessRuleValidationException = Assert.Catch<BusinessRuleException>(() =>
            {
                // Act
                customer.PlaceOrder(cart, currencyConverter);
            });

            Assert.True(customer.Orders.Count == 0);
        }

        [Test]
        public void Order_Has_Orderlines()
        {
            // Arrange
            var baseCurrency = Currency.USDollar;
            var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
            customerUniquenessChecker.IsUserUnique(email).Returns(true);

            var currencyConverter = Substitute.For<ICurrencyConverter>();
            currencyConverter.GetBaseCurrency().Returns(baseCurrency);

            var customer = Customer.CreateCustomer(email, name, customerUniquenessChecker);

            Product product1 = new Product("Product 1", Money.Of((decimal)1.50, baseCurrency.Name));
            Product product2 = new Product("Product 2", Money.Of((decimal)4.00, baseCurrency.Name));
            
            Basket basket = new Basket(Currency.USDollar.Name);
            basket.AddProduct(product1.Id, product1.Price, 1);
            basket.AddProduct(product2.Id, product1.Price, 1);

            // Act
            customer.PlaceOrder(basket, currencyConverter);
            var orderLines = customer.Orders.SelectMany(order => order.OrderLines).ToList();

            // Assert
            Assert.True(orderLines.Count == 2);
        }
    }
}