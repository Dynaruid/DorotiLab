using System.Collections;
using System.Globalization;

namespace Doroti.Runtime;

/// <summary>
/// Explicit index contract for enum-shaped values outside the CLR enum type system.
/// </summary>
public interface IDartEnumIndex
{
    long DartEnumIndex { get; }
}

/// <summary>Dart language primitives; framework diagnostics and platform behavior live outside Runtime.</summary>
public static class FoundationRuntimePorts
{
    public static Duration kLongPressTimeout => Duration.Create(milliseconds: 500);
    /// <summary>Host mapping for Dart bool.fromEnvironment('dart.vm.product').</summary>
    public static bool kReleaseMode =>
#if DEBUG
        false;
#else
        true;
#endif

    /// <summary>Host mapping for Dart bool.fromEnvironment('dart.vm.profile').</summary>
    public static bool kProfileMode => false;

    /// <summary>Dart enum.index / indexable dynamic index accessor.</summary>
    public static long EnumIndex(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value is IDartEnumIndex indexed) return indexed.DartEnumIndex;
        return Convert.ToInt64(value, CultureInfo.InvariantCulture);
    }

    public static long? EnumIndexNullable(object? value) =>
        value is null ? null : EnumIndex(value);

    public static int ObjectHash(params object?[] values)
    {
        var hash = new HashCode();
        foreach (var value in values)
        {
            hash.Add(value);
        }
        return hash.ToHashCode();
    }

    public static int ObjectHashAll<T>(IEnumerable<T> values) => ObjectHash(values.Cast<object?>().ToArray());

    public static int Length(object? value) => value switch
    {
        string text => text.Length,
        ICollection collection => collection.Count,
        IEnumerable sequence => sequence.Cast<object?>().Count(),
        null => throw new NullReferenceException("Dart length was read from null."),
        _ => throw new InvalidOperationException($"{value.GetType().FullName} has no Dart length contract."),
    };

    public static object? Index(object? value, object? index) => value switch
    {
        string text => text[Convert.ToInt32(index, CultureInfo.InvariantCulture)],
        IList list => list[Convert.ToInt32(index, CultureInfo.InvariantCulture)],
        IDictionary map => map[index!],
        null => throw new NullReferenceException("Dart index access targeted null."),
        _ => throw new InvalidOperationException($"{value.GetType().FullName} is not Dart-indexable."),
    };
}
