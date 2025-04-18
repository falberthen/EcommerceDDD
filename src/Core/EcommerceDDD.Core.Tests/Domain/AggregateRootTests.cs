using NSubstitute.Core;

namespace EcommerceDDD.Core.Tests.Domain;

public class AggregateRootTests
{
    [Fact]
    public void TwoIdenticalAggregates_ShouldReturnEqualsFalse()
    {
        // Given
        var aggregateId = new DummyAggregateId(Guid.NewGuid());

        // When
        var aggregateRoot1 = new DummyAggregateRoot(aggregateId);
        var aggregateRoot2 = new DummyAggregateRoot(aggregateId);

		// Then
		Assert.False(aggregateRoot1.GetHashCode() == aggregateRoot2.GetHashCode());
		Assert.False(aggregateRoot1.Equals(aggregateRoot2));
	}

    [Fact]
    public void AppendSeveralDomainEvents_ShouldReturnEqualUncommitedEventsAmount()
    {
        // Given
        var aggregateId = new DummyAggregateId(Guid.NewGuid());
        var aggregateRoot = new DummyAggregateRoot(aggregateId);
        var domainEventsNumber = 10;

        // When
        for (int i = 0; i < domainEventsNumber; i++)
            aggregateRoot.DoSomething();

		// Then
		Assert.Equal(aggregateRoot.GetUncommittedEvents().Count(), domainEventsNumber);
    }
}