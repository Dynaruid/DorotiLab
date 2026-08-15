// <doroti-reviewed-framework-source />
#nullable enable
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/foundation/key.dart
using System;
using System.Collections.Generic;
using System.Linq;
using Doroti.Runtime;
using static Doroti.Runtime.FoundationRuntimePorts;

namespace Doroti.Generated.Framework.Foundation;

public abstract class Key
{
    public static Key Create(string value)
        => new ValueKey<string>(value);

    protected Key()
    {
    }

}

public abstract class LocalKey : Key
{
    protected LocalKey() : base()
    {
    }

}

public class UniqueKey : LocalKey
{
    public UniqueKey()
    {
    }

    public override string ToString() => $"[#{DiagnosticsLibrary.shortHash(this)}]";
}

public class ValueKey<T> : LocalKey
{
    public T value { get; }

    public ValueKey(T value)
    {
        this.value = value;
    }

    public override bool Equals(object? other)
    {
        var __other = other as ValueKey<T>;
        if (__other is null) return false;
        if ((__other.GetType() != this.GetType()))
        {
            return false;
        }
        return ((__other is ValueKey<T>) && EqualityComparer<T>.Default.Equals(__other.value, value));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.GetType(), value);
    public override string ToString()
    {
        var valueString = ((typeof(T) == typeof(string)) ? $"<'{value}'>" : $"<{value}>");
        if ((this.GetType() == typeof(ValueKey<T>)))
        {
            return $"[{valueString}]";
        }
        return $"[{typeof(T)} {valueString}]";
    }

}
