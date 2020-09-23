using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.Shared;
using FluentAssertions;
using System;
using Xunit;

namespace EcommerceDDD.Tests.Domain
{
    public class MoneyTests
    {
        [Fact]
        public void Two_money_are_equal_with_same_values()
        {
            Money money1 = Money.Of(10, "USD");
            Money money2 = Money.Of(10, "USD");

            money1.Should().Be(money2);
            money1.GetHashCode().Should().Be(money2.GetHashCode());
        }

        [Fact]
        public void Two_money_are_not_equal_with_different_amounts()
        {
            Money money1 = Money.Of(20, "USD");
            Money money2 = Money.Of(10, "USD");

            money1.Should().NotBe(money2);
            money1.GetHashCode().Should().NotBe(money2.GetHashCode());
        }

        [Fact]
        public void Mmoney_amount_cannot_be_negative()
        {
            Action action = () => Money.Of(-20, "USD");
            action.Should().Throw<BusinessRuleException>();
        }

        [Fact]
        public void Sum_cannot_allow_different_currencies()
        {
            Money money1 = Money.Of(10, "USD");
            Money money2 = Money.Of(10, "CAD");

            var ex = Assert.Throws<BusinessRuleException>(() => money1 + money2);
            ex.GetType().Should().Be(typeof(BusinessRuleException));
        }

        [Fact]
        public void Sum_must_be_correct()
        {
            Money money1 = Money.Of(10, "USD");
            Money money2 = Money.Of(10, "USD");

            (money1 + money2).Value.Should().Be(20);
            (money1 + money2).CurrencySymbol.Should().Be("US$");
        }
    }
}
