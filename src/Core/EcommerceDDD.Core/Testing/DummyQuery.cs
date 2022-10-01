using EcommerceDDD.Core.CQRS.QueryHandling;

namespace EcommerceDDD.Core.Testing;

public record class DummyQuery(DummyAggregateId Id) : IQuery<DummyAggregateRoot> {}