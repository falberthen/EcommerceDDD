using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Core.Tests.Domain;

public class StronglyTypedIdTests
{
    [Fact]
    public void EmptyGuid_ShouldThrowDomainException()
    {
        // Given
        var ex = Assert.Throws<DomainException>(() => 
            new DummyAggregateId(new Guid())); // When

		// Then
		Assert.Equal(typeof(DomainException), ex.GetType());
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
		Assert.True(areEqual);
		Assert.Equal(aggregateId1, aggregateId2);		
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
		Assert.False(areEqual);
    }
} 
