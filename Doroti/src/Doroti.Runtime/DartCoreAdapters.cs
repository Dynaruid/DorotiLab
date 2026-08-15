using System.Collections;
using System.Globalization;
using System.Text;

namespace Doroti.Runtime;

public readonly record struct Duration(long microseconds) : IComparable<Duration>
{
    public Duration(double seconds) : this(checked((long)(seconds * 1_000_000d))) { }

    /// <summary>Dart <c>Duration</c> named-argument constructor.</summary>
    public static Duration Create(
        long days = 0,
        long hours = 0,
        long minutes = 0,
        long seconds = 0,
        long milliseconds = 0,
        long microseconds = 0) =>
        new(checked(
            microseconds +
            milliseconds * microsecondsPerMillisecond +
            seconds * microsecondsPerMillisecond * millisecondsPerSecond +
            minutes * microsecondsPerMillisecond * millisecondsPerSecond * secondsPerMinute +
            hours * microsecondsPerMillisecond * millisecondsPerSecond * secondsPerMinute * minutesPerHour +
            days * microsecondsPerMillisecond * millisecondsPerSecond * secondsPerMinute * minutesPerHour * hoursPerDay));

    public const long microsecondsPerMillisecond = 1_000;
    public const long millisecondsPerSecond = 1_000;
    public const long microsecondsPerSecond = microsecondsPerMillisecond * millisecondsPerSecond;
    public const long secondsPerMinute = 60;
    public const long minutesPerHour = 60;
    public const long hoursPerDay = 24;

    public long inMicroseconds { get; } = microseconds;
    public static Duration zero => new(0);
    public long inMilliseconds => inMicroseconds / microsecondsPerMillisecond;
    public long inSeconds => inMilliseconds / millisecondsPerSecond;
    public long inMinutes => inSeconds / secondsPerMinute;
    public long inHours => inMinutes / minutesPerHour;
    public long inDays => inHours / hoursPerDay;

    public int CompareTo(Duration other) => inMicroseconds.CompareTo(other.inMicroseconds);
    public static Duration operator +(Duration left, Duration right) => new(left.inMicroseconds + right.inMicroseconds);
    public static Duration operator -(Duration left, Duration right) => new(left.inMicroseconds - right.inMicroseconds);
    public static Duration operator *(Duration value, double factor) =>
        new(checked((long)(value.inMicroseconds * factor)));
    public static Duration operator *(double factor, Duration value) => value * factor;
    public static bool operator <(Duration left, Duration right) => left.inMicroseconds < right.inMicroseconds;
    public static bool operator >(Duration left, Duration right) => left.inMicroseconds > right.inMicroseconds;
    public static bool operator <=(Duration left, Duration right) => left.inMicroseconds <= right.inMicroseconds;
    public static bool operator >=(Duration left, Duration right) => left.inMicroseconds >= right.inMicroseconds;
    public static implicit operator TimeSpan(Duration value) => TimeSpan.FromTicks(value.inMicroseconds * 10);
    public static implicit operator Duration(TimeSpan value) => new(value.Ticks / 10);
}

public sealed class StringBuffer
{
    private readonly StringBuilder _builder = new();
    public StringBuffer(object? content = null) { if (content is not null) _builder.Append(content); }
    public void write(object? value) => _builder.Append(value);
    public void writeCharCode(long value) => _builder.Append(char.ConvertFromUtf32(checked((int)value)));
    public void writeln(object? value = null) => _builder.AppendLine(value?.ToString());
    public override string ToString() => _builder.ToString();
}

public class PriorityQueue<T>
{
    private readonly List<T> _items = [];
    private readonly Comparison<T> _comparison;

    public PriorityQueue(Comparison<T> comparison) => _comparison = comparison;

    public PriorityQueue(Func<T, T, long> comparison) =>
        _comparison = (left, right) => Math.Sign(comparison(left, right));
    public int Count => _items.Count;
    public T first => _items[0];

    public void Add(T value)
    {
        _items.Add(value);
        _items.Sort(_comparison);
    }

    public void Add<TValue>(TValue value)
    {
        if (value is not T typed)
        {
            throw new InvalidOperationException(
                $"Dart covariant queue value {typeof(TValue).FullName} is not assignable to {typeof(T).FullName}.");
        }
        Add(typed);
    }

    public T removeFirst()
    {
        var value = _items[0];
        _items.RemoveAt(0);
        return value;
    }
}

public sealed class HeapPriorityQueue<T> : PriorityQueue<T>
{
    public HeapPriorityQueue(Comparison<T> comparison) : base(comparison) { }

    public HeapPriorityQueue(Func<T, T, long> comparison) : base(comparison) { }
}

public sealed class DartRandom(long seed)
{
    private readonly Random _random = new(checked((int)seed));

    public DartRandom() : this(Random.Shared.NextInt64()) { }

    public double nextDouble() => _random.NextDouble();
}

public sealed class DartArgumentError(object? invalidValue, string? name = null, string? message = null)
    : ArgumentException(message, name)
{
    public object? invalidValue { get; } = invalidValue;
}

/// <summary>Core-library members whose Dart names have no direct CLR member.</summary>
public static class DartCoreExtensions
{
    public static bool isBefore(this DateTime value, DateTime other) => value < other;
    public static bool isAfter(this DateTime value, DateTime other) => value > other;
    public static bool isAtSameMomentAs(this DateTime value, DateTime other) => value == other;
    public static DateTime add(this DateTime value, Duration duration) => value.Add(duration);
    public static DateTime subtract(this DateTime value, Duration duration) => value.Subtract(duration);
    public static Duration difference(this DateTime value, DateTime other) => value - other;
    public static long ToDartWeekday(this DayOfWeek value) => value == DayOfWeek.Sunday ? 7 : (long)value;
    public static IEnumerable<(long index, T value)> indexed<T>(this IEnumerable<T> values) =>
        values.Select((value, index) => ((long)index, value));
    public static double sum(this IEnumerable<double> values) => values.Sum();
    public static long sum(this IEnumerable<long> values) => values.Sum();
    public static long max(this IEnumerable<long> values) => values.Max();
    public static double max(this IEnumerable<double> values) => values.Max();
    public static long min(this IEnumerable<long> values) => values.Min();
    public static double min(this IEnumerable<double> values) => values.Min();
    public static List<T> toList<T>(this IEnumerable<T> values, bool growable = true) => values.ToList();
    public static T? weakTarget<T>(WeakReference<T>? reference) where T : class =>
        reference is not null && reference.TryGetTarget(out var target) ? target : null;

    public static long abs(this long value) => Math.Abs(value);

    public static double abs(this double value) => Math.Abs(value);

    public static long round(this double value) => checked((long)Math.Round(value, MidpointRounding.AwayFromZero));

    public static long floor(this double value) => checked((long)Math.Floor(value));
    public static long ceil(this double value) => checked((long)Math.Ceiling(value));
    public static double floorToDouble(this double value) => Math.Floor(value);
    public static double ceilToDouble(this double value) => Math.Ceiling(value);
    public static double roundToDouble(this double value) => Math.Round(value, MidpointRounding.AwayFromZero);
    public static long toInt(this double value) => checked((long)Math.Truncate(value));
    public static long floor(this long value) => value;
    public static long ceil(this long value) => value;
    public static bool isNegative(this double value) => double.IsNegative(value);
    public static double toDouble(this double value) => value;
    public static double toDouble(this int value) => value;
    public static double toDouble(this long value) => value;
    public static double truncateToDouble(this double value) => Math.Truncate(value);
    public static double nextDouble(this Random random) => random.NextDouble();
    public static bool isFinite(this double value) => double.IsFinite(value);
    public static bool isFinite(this long value) => true;
    public static bool isInfinite(this double value) => double.IsInfinity(value);
    public static double clamp(this double value, double lower, double upper) => Math.Clamp(value, lower, upper);
    public static long clamp(this long value, long lower, long upper) => Math.Clamp(value, lower, upper);
    public static string toStringAsFixed(this double value, long fractionDigits) =>
        value.ToString($"F{fractionDigits}", CultureInfo.InvariantCulture);
    public static string toStringAsPrecision(this double value, long precision) =>
        value.ToString($"G{precision}", CultureInfo.InvariantCulture);
    public static string toRadixString(this long value, long radix)
    {
        if (radix is < 2 or > 36) throw new ArgumentOutOfRangeException(nameof(radix));
        if (value == 0) return "0";
        const string digits = "0123456789abcdefghijklmnopqrstuvwxyz";
        var negative = value < 0;
        ulong remaining = negative ? unchecked((ulong)(-(value + 1))) + 1UL : (ulong)value;
        var buffer = new Stack<char>();
        while (remaining != 0) { buffer.Push(digits[(int)(remaining % (ulong)radix)]); remaining /= (ulong)radix; }
        return (negative ? "-" : string.Empty) + new string(buffer.ToArray());
    }

    public static string trimRight(this string value) => value.TrimEnd();
    public static bool startsWith(this string value, string prefix) => value.StartsWith(prefix, StringComparison.Ordinal);
    public static bool endsWith(this string value, string suffix) => value.EndsWith(suffix, StringComparison.Ordinal);
    public static string replaceAll(this string value, string from, string replacement) =>
        value.Replace(from, replacement, StringComparison.Ordinal);

    public static long codeUnitAt(this string value, long index) => value[checked((int)index)];
    public static string substring(this string value, long start, long? end = null) =>
        end is null ? value[checked((int)start)..] : value[checked((int)start)..checked((int)end.Value)];
    public static bool contains(this string value, string other) => value.Contains(other, StringComparison.Ordinal);
    public static bool contains<T>(this IEnumerable<T> values, T item) => values.Contains(item);
    public static T elementAt<T>(this IEnumerable<T> values, long index) => values.ElementAt(checked((int)index));
    public static string replaceRange(this string value, long start, long? end, string replacement) =>
        value[..checked((int)start)] + replacement + value[checked((int)(end ?? value.Length))..];
    public static IEnumerable<long> runes(this string value) => value.EnumerateRunes().Select(rune => (long)rune.Value);
    public static string toLowerCase(this string value) => value.ToLowerInvariant();
    public static string toUpperCase(this string value) => value.ToUpperInvariant();
    public static Characters characters(this string value) => new(value);

    public static List<string> split(this string value, string pattern) => [.. value.Split(pattern, StringSplitOptions.None)];

    public static string padLeft(this string value, long width, string padding = " ") =>
        Pad(value, width, padding, left: true);

    public static string padRight(this string value, long width, string padding = " ") =>
        Pad(value, width, padding, left: false);

    public static void forEach<T>(this IEnumerable<T> values, Action<T> callback)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(callback);
        foreach (var value in values)
        {
            callback(value);
        }
    }

    public static IEnumerable<TResult> map<T, TResult>(this IEnumerable<T> values, Func<T, TResult> transform) => values.Select(transform);
    public static IEnumerable<TResult> map<TKey, TValue, TResult>(this IReadOnlyDictionary<TKey, TValue> values, Func<TKey, TValue, TResult> transform) => values.Select(pair => transform(pair.Key, pair.Value));
    public static DartMap<TResultKey, TResultValue> map<TKey, TValue, TResultKey, TResultValue>(this IReadOnlyDictionary<TKey, TValue> values, Func<TKey, TValue, MapEntry<TResultKey, TResultValue>> transform) =>
        new(values.Select(pair => transform(pair.Key, pair.Value)).Select(entry => new KeyValuePair<TResultKey, TResultValue>(entry.key, entry.value)));
    public static TResult fold<T, TResult>(this IEnumerable<T> values, TResult initial, Func<TResult, T, TResult> combine) => values.Aggregate(initial, combine);
    public static IEnumerable<T> reversed<T>(this IEnumerable<T> values) => values.Reverse();
    public static T reduce<T>(this IEnumerable<T> values, Func<T, T, T> combine) => values.Aggregate(combine);
    public static IEnumerable<TResult> expand<T, TResult>(this IEnumerable<T> values, Func<T, IEnumerable<TResult>> transform) => values.SelectMany(transform);
    public static IEnumerable<T> where<T>(this IEnumerable<T> values, Func<T, bool> predicate) => values.Where(predicate);
    public static IEnumerable<T> where<T>(this IEnumerable<T> values, Delegate predicate) =>
        values.Where(value => InvokePredicate(predicate, value));
    public static IEnumerable<T> skip<T>(this IEnumerable<T> values, long count) => values.Skip(checked((int)count));
    public static IEnumerable<T> take<T>(this IEnumerable<T> values, long count) => values.Take(checked((int)count));
    public static bool any<T>(this IEnumerable<T> values, Func<T, bool> predicate) => values.Any(predicate);
    public static bool any<T>(this IEnumerable<T> values, Delegate predicate) =>
        values.Any(value => InvokePredicate(predicate, value));
    public static IEnumerable<TResult> cast<TResult>(this IEnumerable values) => values.Cast<TResult>();
    public static HashSet<T> toSet<T>(this IEnumerable<T> values) => new(values);
    public static IEnumerable<T> followedBy<T>(this IEnumerable<T> values, IEnumerable<T> other) => values.Concat(other);
    public static HashSet<T> difference<T>(this HashSet<T> values, IEnumerable<T> other)
    {
        var result = new HashSet<T>(values);
        result.ExceptWith(other);
        return result;
    }
    public static HashSet<T> intersection<T>(this IEnumerable<T> values, IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(other);
        var result = new HashSet<T>(values);
        result.IntersectWith(other);
        return result;
    }
    public static T firstWhere<T>(this IEnumerable<T> values, Func<T, bool> predicate) =>
        values.First(predicate);
    public static T firstWhere<T>(this IEnumerable<T> values, Func<T, bool> predicate, Func<T>? orElse)
    {
        foreach (var value in values) if (predicate(value)) return value;
        return orElse is null ? throw new InvalidOperationException("No element") : orElse();
    }
    public static T? firstWhereOrNull<T>(this IEnumerable<T> values, Func<T, bool> predicate) =>
        values.FirstOrDefault(predicate);
    public static T lastWhere<T>(this IEnumerable<T> values, Func<T, bool> predicate) =>
        values.Last(predicate);
    public static T lastWhere<T>(this IEnumerable<T> values, Func<T, bool> predicate, Func<T>? orElse)
    {
        var found = false;
        var result = default(T)!;
        foreach (var value in values)
        {
            if (!predicate(value)) continue;
            found = true;
            result = value;
        }
        return found ? result : orElse is null ? throw new InvalidOperationException("No element") : orElse();
    }
    public static T? elementAtOrNull<T>(this IEnumerable<T> values, long index) =>
        index < 0 ? default : values.ElementAtOrDefault(checked((int)index));
    public static long indexWhere<T>(this IEnumerable<T> values, Func<T, bool> predicate, long start = 0)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(predicate);
        var index = 0L;
        foreach (var value in values)
        {
            if (index >= start && predicate(value)) return index;
            index++;
        }
        return -1;
    }
    public static DartMap<long, T> asMap<T>(this IEnumerable<T> values) =>
        new(values.Select((value, index) => new KeyValuePair<long, T>(index, value)));
    public static bool remove<TKey, TValue>(this IDictionary<TKey, TValue> values, TKey key) =>
        values.Remove(key);
    public static bool remove<T>(this ISet<T> values, T item) => values.Remove(item);
    public static void clear<TKey, TValue>(this IDictionary<TKey, TValue> values) => values.Clear();
    public static void forEach(this IDictionary values, Action<object?, object?> action)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(action);
        foreach (DictionaryEntry entry in values) action(entry.Key, entry.Value);
    }

    public static bool add<T>(this ISet<T> values, T value) => values.Add(value);
    public static TValue putIfAbsent<TKey, TValue>(
        this IDictionary<TKey, TValue> values,
        TKey key,
        Func<TValue> ifAbsent)
    {
        if (values.TryGetValue(key, out var value)) return value;
        value = ifAbsent();
        values.Add(key, value);
        return value;
    }
    public static void removeWhere<T>(this HashSet<T> values, Func<T, bool> predicate) =>
        values.RemoveWhere(item => predicate(item));
    public static void removeWhere<T>(this HashSet<T> values, Delegate predicate) =>
        values.RemoveWhere(item => InvokePredicate(predicate, item));
    public static void removeWhere<T>(this List<T> values, Func<T, bool> predicate) =>
        values.RemoveAll(item => predicate(item));
    public static void removeWhere<T>(this List<T> values, Delegate predicate) =>
        values.RemoveAll(item => InvokePredicate(predicate, item));
    public static void setLength<T>(this List<T> values, long length)
    {
        var target = checked((int)length);
        if (target < 0) throw new ArgumentOutOfRangeException(nameof(length));
        if (target < values.Count)
        {
            values.RemoveRange(target, values.Count - target);
            return;
        }
        if (target > values.Count)
        {
            values.AddRange(Enumerable.Repeat(default(T)!, target - values.Count));
        }
    }
    public static void removeAll<T>(this HashSet<T> values, IEnumerable<T> other) =>
        values.ExceptWith(other);

    private static bool InvokePredicate<T>(Delegate predicate, T value) =>
        predicate.DynamicInvoke(value) is true;

    public static T _merge<T>(this T value, T other) where T : struct, Enum =>
        Convert.ToInt64(value, CultureInfo.InvariantCulture) >= Convert.ToInt64(other, CultureInfo.InvariantCulture)
            ? value
            : other;
    public static T removeLast<T>(this List<T> values)
    {
        var index = values.Count - 1;
        var value = values[index];
        values.RemoveAt(index);
        return value;
    }

    public static T removeLast<T>(this Queue<T> values)
    {
        if (values.Count == 0) throw new InvalidOperationException("Cannot remove from an empty queue.");
        var retained = values.ToArray();
        values.Clear();
        for (var index = 0; index < retained.Length - 1; index++) values.Enqueue(retained[index]);
        return retained[^1];
    }
    public static T removeAt<T>(this List<T> values, long index)
    {
        var checkedIndex = checked((int)index);
        var value = values[checkedIndex];
        values.RemoveAt(checkedIndex);
        return value;
    }
    public static List<T> GetRange<T>(this List<T> values, long index, long count) =>
        values.GetRange(checked((int)index), checked((int)count));
    public static int lastIndexWhere<T>(this List<T> values, Func<T, bool> predicate, int? start = null)
    {
        for (var index = Math.Min(start ?? values.Count - 1, values.Count - 1); index >= 0; index--)
            if (predicate(values[index])) return index;
        return -1;
    }
    public static void sort<T>(this List<T> values, Comparison<T>? comparison = null) =>
        values.Sort(comparison is null ? null : Comparer<T>.Create(comparison));

    public static void sort<T>(this List<T> values, Func<T, T, long> comparison) =>
        values.Sort((left, right) => Math.Sign(comparison(left, right)));

    public static void AddRange<T>(this Queue<T> values, IEnumerable<T> additions)
    {
        foreach (var value in additions) values.Enqueue(value);
    }

    public static void AddRange<T>(this ISet<T> values, IEnumerable<T> additions)
    {
        foreach (var value in additions) values.Add(value);
    }

    public static TKey? firstKeyAfter<TKey, TValue>(this SortedDictionary<TKey, TValue> values, TKey key) where TKey : notnull =>
        values.Keys.FirstOrDefault(candidate => Comparer<TKey>.Default.Compare(candidate, key) > 0);

    public static TKey? lastKeyBefore<TKey, TValue>(this SortedDictionary<TKey, TValue> values, TKey key) where TKey : notnull =>
        values.Keys.LastOrDefault(candidate => Comparer<TKey>.Default.Compare(candidate, key) < 0);

    public static TKey? firstKey<TKey, TValue>(this SortedDictionary<TKey, TValue> values) where TKey : notnull =>
        values.Count == 0 ? default : values.Keys.First();

    public static TKey? lastKey<TKey, TValue>(this SortedDictionary<TKey, TValue> values) where TKey : notnull =>
        values.Count == 0 ? default : values.Keys.Last();

    public static void addFirst<T>(this Queue<T> values, T value)
    {
        var existing = values.ToArray();
        values.Clear();
        values.Enqueue(value);
        foreach (var item in existing) values.Enqueue(item);
    }

    public static void addLast<T>(this Queue<T> values, T value) => values.Enqueue(value);

    public static void forEach<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> values, Action<TKey, TValue> callback)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(callback);
        foreach (var pair in values)
        {
            callback(pair.Key, pair.Value);
        }
    }

    public static string repeat(string value, long count)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (count < 0 || count > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        return string.Concat(Enumerable.Repeat(value, checked((int)count)));
    }

    private static string Pad(string value, long width, string padding, bool left)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrEmpty(padding);
        if (width <= value.Length)
        {
            return value;
        }
        var needed = checked((int)(width - value.Length));
        var repeated = repeat(padding, (needed + padding.Length - 1) / padding.Length)[..needed];
        return left ? repeated + value : value + repeated;
    }
}

/// <summary>Dart core top-level parsing members selected by resolved AST lowering.</summary>
public static class Dart_coreLibrary
{
    public static dynamic parse(string value) =>
        value.IndexOfAny(['.', 'e', 'E']) >= 0
            ? double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture)
            : long.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);

    public static string decodeComponent(string value) => Uri.UnescapeDataString(value);

    public static int identityHashCode(object? value) => value is null
        ? 0
        : System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(value);

    public static string escape(string value) => System.Text.RegularExpressions.Regex.Escape(value);

    public static void throwWithStackTrace(object error, object? stackTrace)
    {
        _ = stackTrace;
        if (error is Exception exception)
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
        throw new InvalidOperationException(error?.ToString());
    }

    public static double? tryParse(string value) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : null;

    public static long? tryParse(string value, long radix) =>
        TryParseRadix(value, checked((int)radix), out var result) ? result : null;

    private static bool TryParseRadix(string value, int radix, out long result)
    {
        try
        {
            result = Convert.ToInt64(value, radix);
            return true;
        }
        catch (Exception exception) when (exception is FormatException or OverflowException or ArgumentException)
        {
            result = default;
            return false;
        }
    }

    public static long hashAllUnordered<T>(IEnumerable<T> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var sum = 0L;
        var xor = 0L;
        var count = 0L;
        foreach (var value in values)
        {
            var hash = value?.GetHashCode() ?? 0;
            sum = unchecked(sum + hash);
            xor ^= hash;
            count++;
        }
        return HashCode.Combine(sum, xor, count);
    }
}

public sealed class Characters : IReadOnlyCollection<string>
{
    private readonly string[] _elements;
    public Characters(string value) => _elements = value.EnumerateRunes().Select(rune => rune.ToString()).ToArray();
    public int Count => _elements.Length;
    public string first => _elements.First();
    public string last => _elements.Last();
    public CharacterRange iterator => new(_elements);
    public string characterAt(long index) => _elements[checked((int)index)];
    public Characters GetRange(long start, long? end = null)
    {
        var first = checked((int)start);
        var last = end is null ? _elements.Length : checked((int)end.Value);
        return new Characters(string.Concat(_elements[first..last]));
    }
    public static implicit operator Characters(string value) => new(value);
    public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)_elements).GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

public sealed class CharacterRange
{
    private readonly string[] elements;
    private int _start;
    private int _end;
    public CharacterRange(string[] elements) => this.elements = elements;
    public CharacterRange(string value) : this(value.EnumerateRunes().Select(rune => rune.ToString()).ToArray()) { }
    public CharacterRange(string value, long index)
    {
        elements = value.EnumerateRunes().Select(rune => rune.ToString()).ToArray();
        _start = Math.Clamp(checked((int)index), 0, elements.Length);
        _end = _start;
    }
    public long stringBeforeLength => string.Concat(elements[.._start]).Length;
    public long stringAfterLength => string.Concat(elements[_end..]).Length;
    public string stringBefore => string.Concat(elements[.._start]);
    public string stringAfter => string.Concat(elements[_end..]);
    public long Count => current.Length;
    public bool moveNext(long count = 1) { _start = _end; _end = Math.Min(elements.Length, _end + checked((int)count)); return _start < elements.Length; }
    public bool MoveNext() => moveNext();
    public bool moveBack(long count = 1) { _end = _start; _start = Math.Max(0, _start - checked((int)count)); return _end > 0; }
    public void expandNext(long count = 1) => _end = Math.Min(elements.Length, _end + checked((int)count));
    public string current => string.Concat(elements[_start.._end]);
    public string Current => current;
    public Characters currentCharacters => new(current);
}
