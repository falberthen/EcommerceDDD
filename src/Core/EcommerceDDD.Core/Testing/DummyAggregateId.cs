namespace EcommerceDDD.Core.Testing;

public sealed class DummyAggregateId(Guid value) : StronglyTypedId<Guid>(value) { }

public sealed class AnotherFakeAggregateId(Guid value) : StronglyTypedId<Guid>(value) { }