// <doroti-reviewed-framework-source />
#nullable enable
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/foundation/observer_list.dart
using System;
using System.Collections.Generic;
using System.Linq;
using Doroti.Runtime;
using static Doroti.Runtime.FoundationRuntimePorts;

namespace Doroti.Framework.Foundation;

public class ObserverList<T> : IEnumerable<T> where T : notnull
{
    private List<T> _list { get; } = new List<T>();
    private bool _isDirty = false;
    private HashSet<T> _set { get; } = new HashSet<T>();

    public void add(T item)
    {
        _isDirty = true;
        _list.Add(item);
    }

    public bool remove(T item)
    {
        bool removed = _list.Remove(item);
        if (removed)
        {
            _isDirty = true;
            _set.Clear();
        }
        return removed;
    }

    public void clear()
    {
        _isDirty = false;
        _list.Clear();
        _set.Clear();
    }

    public bool contains(T element)
    {
        if ((_list.Count < 3))
        {
            return _list.Contains(element);
        }
        if (_isDirty)
        {
            _set.UnionWith(_list);
            _isDirty = false;
        }
        return _set.Contains(element);
    }

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    public int Count => _list.Count;
    public T single => _list.Single();
    public bool isEmpty => (_list.Count == 0);
    public bool isNotEmpty => (_list.Count != 0);
    public List<T> toList(bool growable = true)
    {
        return _list.ToList();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

public class HashedObserverList<T> : IEnumerable<T> where T : notnull
{
    private Dictionary<T, int?> _map { get; } = new Dictionary<T, int?>();

    public void add(T item)
    {
        _map[item] = (((_map.GetValueOrDefault(item) ?? 0)) + 1);
    }

    public bool remove(T item)
    {
        int? value = _map.GetValueOrDefault(item);
        if ((value is null))
        {
            return false;
        }
        if ((value == 1))
        {
            _map.Remove(item);
        }
        else
        {
            _map[item] = (value - 1);
        }
        return true;
    }

    public void clear() => _map.Clear();
    public bool contains(T element) => _map.ContainsKey(element);
    public IEnumerator<T> GetEnumerator() => _map.Keys.GetEnumerator();
    public bool isEmpty => (_map.Count == 0);
    public bool isNotEmpty => (_map.Count != 0);
    public List<T> toList(bool growable = true)
    {
        IEnumerator<T> iterator = _map.Keys.GetEnumerator();
        return new List<T>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, _map.Count), (_) => (((Func<IEnumerator<T>>)(() =>
{
    var __cascade = GetEnumerator();
    __cascade.MoveNext();
    return __cascade;
}))()).Current));
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
