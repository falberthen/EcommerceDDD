using System;

namespace EcommerceDDD.Domain.SeedWork
{
    public abstract class StronglyTypedId<T> : ValueObject<StronglyTypedId<T>>
    {
        public Guid Value { get; }

        protected StronglyTypedId(Guid value)
        {
            Value = value;
        }

        protected override bool EqualsCore(StronglyTypedId<T> other)
        {
            return this.Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            unchecked
            {
                return Value.GetHashCode();
            }
        }
    }
}