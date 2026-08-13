// Manual replacement for Flutter ValueKey<T> runtime-type equality and hashing.
namespace Doroti.Generated.P3.Foundation;

public partial class ValueKey<T> : LocalKey, IEquatable<ValueKey<T>>
{
    public ValueKey(T value) => this.value = value;

    public T value { get; }

    public bool Equals(ValueKey<T>? other) =>
        other is not null && GetType() == other.GetType() && EqualityComparer<T>.Default.Equals(value, other.value);

    public override bool Equals(object? obj) => obj is ValueKey<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(GetType(), value);
}
