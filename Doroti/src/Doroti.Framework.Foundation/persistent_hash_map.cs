// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/persistent_hash_map.dart
using System.Collections;
using System.Numerics;

namespace Doroti.Generated.Framework.Foundation;

/// <summary>An immutable map value with structural copy-on-write operations.</summary>
public sealed class PersistentHashMap<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _values;

    public PersistentHashMap() : this(new Dictionary<TKey, TValue>()) { }

    public static PersistentHashMap<TKey, TValue> CreateEmpty() => new();

    private PersistentHashMap(Dictionary<TKey, TValue> values) => _values = values;

    public int Count => _values.Count;
    public IEnumerable<TKey> Keys => _values.Keys;
    public IEnumerable<TValue> Values => _values.Values;
    public TValue this[TKey key] => _values[key];
    public bool isEmpty => _values.Count == 0;
    public bool isNotEmpty => _values.Count != 0;

    public PersistentHashMap<TKey, TValue> put(TKey key, TValue value)
    {
        var copy = new Dictionary<TKey, TValue>(_values) { [key] = value };
        return new(copy);
    }

    public PersistentHashMap<TKey, TValue> remove(TKey key)
    {
        if (!_values.ContainsKey(key))
        {
            return this;
        }
        var copy = new Dictionary<TKey, TValue>(_values);
        copy.Remove(key);
        return new(copy);
    }

    public bool containsKey(TKey key) => _values.ContainsKey(key);
    public bool ContainsKey(TKey key) => containsKey(key);
    public TValue? valueFor(TKey key) => _values.GetValueOrDefault(key);
    public bool TryGetValue(TKey key, out TValue value) => _values.TryGetValue(key, out value!);
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal abstract class _TrieNode<TKey, TValue> where TKey : notnull;
internal sealed class _CompressedNode<TKey, TValue> : _TrieNode<TKey, TValue> where TKey : notnull;
internal sealed class _FullNode<TKey, TValue> : _TrieNode<TKey, TValue> where TKey : notnull;
internal sealed class _HashCollisionNode<TKey, TValue> : _TrieNode<TKey, TValue> where TKey : notnull;

internal static class PersistentHashMapLibrary
{
    internal static int _bitCount(int value) => BitOperations.PopCount((uint)value);
    internal static T[] _copy<T>(IReadOnlyList<T> source) => source.ToArray();
    internal static T?[] _makeArray<T>(int length) => new T?[length];
    internal static T _unsafeCast<T>(object value) => (T)value;
}
