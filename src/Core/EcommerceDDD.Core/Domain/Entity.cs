namespace EcommerceDDD.Core.Domain;

public abstract class Entity<TKey>
    where TKey : StronglyTypedId<Guid>
{
    public TKey Id { get; set; } = default!;
}