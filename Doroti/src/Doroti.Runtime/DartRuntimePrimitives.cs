using System.Diagnostics;

namespace Doroti.Runtime;

public sealed class AssertionError(object? message = null) : Exception(message?.ToString() ?? "Assertion failed.");

public sealed class NoSuchMethodError(object? message = null) : Exception(message?.ToString() ?? "No such method.");

public sealed class TypeError(object? message = null) : Exception(message?.ToString() ?? "Dart type error.");

/// <summary>Runtime bindings for Dart semantics that do not belong to a Flutter framework type.</summary>
public static class DartRuntimePrimitives
{
    // Kept non-const so Roslyn does not reject the explicit Dart fallback arm
    // after proving that all currently named CLR enum members were matched.
    public static bool NonExhaustiveSwitchGuard => true;

    public static List<T> CreateList<T>(long length, Func<long, T> generator) =>
        Enumerable.Range(0, checked((int)length)).Select(index => generator(index)).ToList();

    public static DateTime CreateDateTime(
        long year,
        long month = 1,
        long day = 1,
        long hour = 0,
        long minute = 0,
        long second = 0,
        long millisecond = 0,
        long microsecond = 0) =>
        new DateTime(checked((int)year), checked((int)month), checked((int)day), checked((int)hour),
            checked((int)minute), checked((int)second), checked((int)millisecond), DateTimeKind.Unspecified)
            .AddTicks(checked(microsecond * 10));
    private sealed class DartNullRuntimeType;

    public static Exception AsException(object? value) =>
        value as Exception ?? new Exception(value?.ToString() ?? "Dart threw null.");

    public static IEnumerable<T> ConvertEnumerable<T>(System.Collections.IEnumerable values)
    {
        foreach (var value in values)
        {
            if (value is T typed) yield return typed;
            else yield return (T)Convert.ChangeType(value!, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
        }
    }

    public static DartMap<TKey, TValue> ConvertMap<TKey, TValue>(System.Collections.IDictionary values)
    {
        if (values is DartMap<TKey, TValue> alreadyTyped)
        {
            return alreadyTyped;
        }
        var result = new DartMap<TKey, TValue>();
        foreach (System.Collections.DictionaryEntry entry in values)
        {
            var key = entry.Key is TKey typedKey ? typedKey : (TKey)Convert.ChangeType(entry.Key!, Nullable.GetUnderlyingType(typeof(TKey)) ?? typeof(TKey));
            var value = entry.Value is TValue typedValue ? typedValue : (TValue)Convert.ChangeType(entry.Value!, Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue));
            result[key] = value;
        }
        return result;
    }

    /// <summary>Converts a dynamically typed Dart value to the statically emitted CLR contract.</summary>
    public static T ConvertValue<T>(object? value)
    {
        if (value is T typed)
        {
            return typed;
        }
        if (value is null)
        {
            return default!;
        }
        var target = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        if (target.IsEnum)
        {
            return (T)Enum.ToObject(target, Convert.ChangeType(value, Enum.GetUnderlyingType(target))!);
        }
        return (T)Convert.ChangeType(value, target);
    }

    public static Action? AdaptAsyncCallback(Func<Future>? callback) =>
        callback is null ? null : () => Ignore(callback());

    public static System.Diagnostics.StackTrace StackTraceFrom(object? value) =>
        value is System.Diagnostics.StackTrace stackTrace ? stackTrace : new System.Diagnostics.StackTrace(true);

    public static long MillisecondsSinceEpoch(DateTime value) => new DateTimeOffset(value).ToUnixTimeMilliseconds();

    public static long? MillisecondsSinceEpoch(DateTime? value) =>
        value is { } resolved ? new DateTimeOffset(resolved).ToUnixTimeMilliseconds() : null;

    /// <summary>Implements Dart's postfix null assertion for nullable value types.</summary>
    public static T RequireValue<T>(T? value) where T : struct =>
        value ?? throw new NullReferenceException("Dart null assertion failed.");

    public static T RequireValue<T>(T value) where T : struct => value;

    /// <summary>Implements Dart's postfix null assertion for reference types.</summary>
    public static T RequireValue<T>(T? value, bool referenceType = true) where T : class
    {
        _ = referenceType;
        return value ?? throw new NullReferenceException("Dart null assertion failed.");
    }

    public static T RequireReference<T>(T value) =>
        value is null ? throw new NullReferenceException("Dart null assertion failed.") : value;

    /// <summary>Evaluates a Dart null-aware member access without applying <c>?.</c> to an unconstrained result type.</summary>
    public static TResult? NullAware<TTarget, TResult>(TTarget? target, Func<TTarget, TResult> access)
        where TTarget : class
    {
        ArgumentNullException.ThrowIfNull(access);
        return target is null ? default : access(target);
    }

    public static T Min<T>(T left, T right) =>
        Comparer<T>.Default.Compare(left, right) <= 0 ? left : right;

    public static T Max<T>(T left, T right) =>
        Comparer<T>.Default.Compare(left, right) >= 0 ? left : right;

    /// <summary>
    /// Implements Dart's identity relation without boxing value types into a false
    /// <see cref="ReferenceEquals(object?, object?)"/> comparison. Floating-point
    /// identity preserves the sign of zero and the NaN payload.
    /// </summary>
    public static bool Identical<T>(T left, T right) =>
        typeof(T).IsValueType
            ? EqualityComparer<T>.Default.Equals(left, right)
            : ReferenceEquals(left, right);

    public static bool Identical(double left, double right) =>
        BitConverter.DoubleToInt64Bits(left) == BitConverter.DoubleToInt64Bits(right);

    public static bool Identical(double? left, double? right) =>
        left.HasValue == right.HasValue && (!left.HasValue || Identical(left.Value, right!.Value));

    public static void Noop() { }

    public static void Ignore<T>(T value) => _ = value;

    /// <summary>Evaluates a Dart void expression used in a dynamic value context.</summary>
    public static object? CaptureVoid(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        action();
        return null;
    }

    public static T? LerpNullable<T>(T? a, T? b, double t, Func<T?, T?, double, T> lerp)
        where T : struct =>
        EqualityComparer<T?>.Default.Equals(a, b) || t == 0 ? a : t == 1 ? b : lerp(a, b, t);

    /// <summary>Implements Dart's postfix null assertion for reference types.</summary>
    public static T RequireNotNull<T>(T? value) where T : class =>
        value ?? throw new NullReferenceException("Dart null assertion failed.");

    [Conditional("DEBUG")]
    public static void Assert(Func<bool> condition)
    {
        ArgumentNullException.ThrowIfNull(condition);
        if (!condition())
        {
            throw new InvalidOperationException("A transpiled Dart assert failed.");
        }
    }

    [Conditional("DEBUG")]
    public static void Assert(Func<bool> condition, Func<object?> message)
    {
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(message);
        if (!condition())
        {
            throw new InvalidOperationException(message()?.ToString() ?? "A transpiled Dart assert failed.");
        }
    }

    public static string RuntimeTypeName(object? value) => value switch
    {
        null => "Null",
        string => "String",
        int => "int",
        double => "double",
        bool => "bool",
        _ => value.GetType().Name,
    };

    public static Type RuntimeType(object? value) => value?.GetType() ?? typeof(DartNullRuntimeType);
}
