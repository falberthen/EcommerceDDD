using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Core.Testing;

namespace EcommerceDDD.Core.Tests.Domain;

public class StronglyTypedIdTests
{
    [Fact]
    public void EmptyGuid_ShouldThrowDomainException()
    {
        // Given
        var ex = Assert.Throws<BusinessRuleException>(() => 
            new DummyAggregateId(new Guid())); // When

        // Then
        ex.GetType().Should().Be(typeof(BusinessRuleException));
    }

    [Fact]
    public void TwoIdenticalIds_ShouldReturnEqualsTrue()
    {
        // Given
        var idValue = Guid.NewGuid();

        var aggregateId1 = new DummyAggregateId(idValue);
        var aggregateId2 = new DummyAggregateId(idValue);

        // When
        var areEqual = aggregateId1 == aggregateId2;

        // Then
        areEqual.Should().BeTrue();
        aggregateId1.Should().Be(aggregateId2);
    }

    [Fact]
    public void TwoIdenticalValueIdsForDifferentTypes_ShouldReturnEqualsFalse()
    {
        // Given
        var idValue = Guid.NewGuid();

        var aggregateId1 = new DummyAggregateId(idValue);
        var aggregateId2 = new AnotherFakeAggregateId(idValue);

        // When
        var areEqual = aggregateId1 == aggregateId2;

        // Then
        areEqual.Should().BeFalse();
        aggregateId1.Should().NotBe(aggregateId2);
    }
} 
