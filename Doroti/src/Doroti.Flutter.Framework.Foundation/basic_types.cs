// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/basic_types.dart
using System.Collections;

namespace Doroti.Generated.Framework.Foundation;

public delegate void ValueChanged<in T>(T value);
public delegate void ValueSetter<in T>(T value);
public delegate T ValueGetter<out T>();
public delegate IEnumerable<T> IterableFilter<T>(IEnumerable<T> input);
public delegate Task AsyncCallback();
public delegate Task AsyncValueSetter<in T>(T value);
public delegate Task<T> AsyncValueGetter<T>();
public sealed class Factory<T>
{
    public Factory(Func<T> constructor) => this.constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
    public Func<T> constructor { get; }
    public Type type => typeof(T);
    public T Invoke() => constructor();
    public static implicit operator Factory<T>(Func<T> constructor) => new(constructor);
    public override string ToString() => $"Factory(type: {type})";
}

public static class BasicTypesLibrary
{
    public static TimeSpan lerpDuration(TimeSpan a, TimeSpan b, double t) =>
        TimeSpan.FromTicks(a.Ticks + (long)Math.Round((b.Ticks - a.Ticks) * t, MidpointRounding.AwayFromZero));
}

/// <summary>A lazy iterable that enumerates its source at most once and caches observed values.</summary>
public sealed class CachingIterable<T> : IEnumerable<T>
{
    private readonly object _gate = new();
    private readonly IEnumerator<T> _source;
    private readonly List<T> _results = [];
    private bool _complete;

    public CachingIterable(IEnumerator<T> source) => _source = source ?? throw new ArgumentNullException(nameof(source));

    public int length
    {
        get
        {
            FillToEnd();
            return _results.Count;
        }
    }

    public T elementAt(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        lock (_gate)
        {
            while (_results.Count <= index && FillNextCore())
            {
            }
            return index < _results.Count ? _results[index] : throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    public List<T> toList(bool growable = true)
    {
        _ = growable;
        FillToEnd();
        lock (_gate)
        {
            return [.. _results];
        }
    }

    public CachingIterable<TResult> map<TResult>(Func<T, TResult> transform) =>
        new(Enumerable.Select(this, transform ?? throw new ArgumentNullException(nameof(transform))).GetEnumerator());

    public CachingIterable<T> where(Func<T, bool> predicate) =>
        new(Enumerable.Where(this, predicate ?? throw new ArgumentNullException(nameof(predicate))).GetEnumerator());

    public CachingIterable<TResult> expand<TResult>(Func<T, IEnumerable<TResult>> transform) =>
        new(Enumerable.SelectMany(this, transform ?? throw new ArgumentNullException(nameof(transform))).GetEnumerator());

    public CachingIterable<T> take(int count) => new(Enumerable.Take(this, count).GetEnumerator());

    public CachingIterable<T> takeWhile(Func<T, bool> predicate) => new(Enumerable.TakeWhile(this, predicate).GetEnumerator());

    public CachingIterable<T> skip(int count) => new(Enumerable.Skip(this, count).GetEnumerator());

    public CachingIterable<T> skipWhile(Func<T, bool> predicate) => new(Enumerable.SkipWhile(this, predicate).GetEnumerator());

    public IEnumerator<T> GetEnumerator() => new _LazyListIterator<T>(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal bool TryGet(int index, out T value)
    {
        lock (_gate)
        {
            while (_results.Count <= index && FillNextCore())
            {
            }
            if (index < _results.Count)
            {
                value = _results[index];
                return true;
            }
            value = default!;
            return false;
        }
    }

    private void FillToEnd()
    {
        lock (_gate)
        {
            while (FillNextCore())
            {
            }
        }
    }

    private bool FillNextCore()
    {
        if (_complete)
        {
            return false;
        }
        if (!_source.MoveNext())
        {
            _complete = true;
            _source.Dispose();
            return false;
        }
        _results.Add(_source.Current);
        return true;
    }
}

internal sealed class _LazyListIterator<T>(CachingIterable<T> owner) : IEnumerator<T>
{
    private int _index = -1;

    public T Current { get; private set; } = default!;

    object? IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (!owner.TryGet(++_index, out var value))
        {
            Current = default!;
            return false;
        }
        Current = value;
        return true;
    }

    public void Reset() => throw new NotSupportedException();

    public void Dispose()
    {
    }
}
