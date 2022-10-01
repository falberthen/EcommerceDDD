namespace EcommerceDDD.Core.Testing;

public sealed class DummyAggregateId : StronglyTypedId<Guid>
{
    public DummyAggregateId(Guid value) : base(value)
    {
    }
}

public sealed class AnotherFakeAggregateId : StronglyTypedId<Guid>
{
    public AnotherFakeAggregateId(Guid value) : base(value)
    {
    }
}