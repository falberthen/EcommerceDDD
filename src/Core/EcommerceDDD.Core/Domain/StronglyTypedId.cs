namespace EcommerceDDD.Core.Domain;

public abstract class StronglyTypedId<T> : ValueObject<StronglyTypedId<T>>
{
	public T Value { get; }

	public StronglyTypedId(T value)
	{
		if (value is null)
			throw new ArgumentNullException(nameof(value));
		if (value.Equals(Guid.Empty))
			throw new DomainException("A valid id must be provided.");

		Value = value;
	}

	public override int GetHashCode()
		=> EqualityComparer<T>.Default.GetHashCode(Value);

	// Stringifies to the wrapped value so [Audit] / logging / OpenTelemetry tags
	// carry the raw id rather than the type name.
	public override string ToString() => Value?.ToString() ?? string.Empty;

	public static bool operator ==(StronglyTypedId<T>? left, StronglyTypedId<T>? right) 
		=> Equals(left, right);

	public static bool operator !=(StronglyTypedId<T>? left, StronglyTypedId<T>? right) 
		=> !Equals(left, right);

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Value;
	}
}