using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.Services;
using EcommerceDDD.Domain.Shared;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Tests.Domain
{
    [TestFixture]
    public class OrdersTests
    {
        const string name = "Customer";
        const string email = "test@email.com";

        [Test]
        public void Place_an_Order()
        {
            // Arrange
            var currency = Currency.USDollar;
            var productPrice = 12.5;
            var productQuantity = 2;

            var productMoney = Money.Of(Convert.ToDecimal(12.5), "USD");
            var currencyConverter = Substitute.For<ICurrencyConverter>();
            currencyConverter.Convert(currency, Money.Of(Convert.ToDecimal(productPrice * productQuantity), currency.Code))
                .Returns(productMoney);

            var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
            customerUniquenessChecker.IsUserUnique(email).Returns(true);
            
            var customer = Customer.CreateCustomer(email, name, customerUniquenessChecker);
            
            var cart = new Cart(customer);
            cart.AddItem(new Product("Test Product", productMoney), productQuantity);

            // Act
            var order = Order.PlaceOrder(cart, currency, currencyConverter);

            // Assert
            Assert.IsNotNull(order);
        }
    }
}
