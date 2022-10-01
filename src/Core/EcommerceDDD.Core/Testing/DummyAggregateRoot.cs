using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Core.Testing;

public class DummyAggregateRoot : AggregateRoot<DummyAggregateId>
{
    public DummyAggregateRoot()
    {
    }

    public DummyAggregateRoot(DummyAggregateId fakeId)
    {
        Id = fakeId;        
    }

    public void DoSomething()
    {
        AppendEvent(new DummyDomainEvent());
    }
}