// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/persistent_hash_map.dart
using System.Collections;

namespace Doroti.Framework.Foundation;

/// <summary>An immutable map value with structural copy-on-write operations.</summary>
public sealed class PersistentHashMap<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly PersistentHashTrie<TKey, TValue>? _root;
    public PersistentHashMap() { }
    private PersistentHashMap(PersistentHashTrie<TKey, TValue>? root, int count) { _root = root; Count = count; }
    public static PersistentHashMap<TKey, TValue> CreateEmpty() => new();
    public int Count { get; }
    public IEnumerable<TKey> Keys => this.Select(pair => pair.Key);
    public IEnumerable<TValue> Values => this.Select(pair => pair.Value);
    public bool isEmpty => Count == 0;
    public bool isNotEmpty => Count != 0;
    public TValue this[TKey key] => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
    private static uint Hash(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return unchecked((uint)EqualityComparer<TKey>.Default.GetHashCode(key));
    }
    public PersistentHashMap<TKey, TValue> put(TKey key, TValue value)
    {
        using var profile = Doroti.Ui.FrameworkComponentProfile.Begin(Doroti.Ui.FrameworkComponentProfile.Kind.InheritancePut, Count);
        var hash = Hash(key);
        if (_root is null) return new(new PersistentHashTrie<TKey, TValue>.Leaf(hash, [new(key, value)]), 1);
        var added = false;
        var root = _root.Put(key, value, hash, 0, ref added);
        return new(root, Count + (added ? 1 : 0));
    }
    public PersistentHashMap<TKey, TValue> remove(TKey key)
    {
        using var profile = Doroti.Ui.FrameworkComponentProfile.Begin(Doroti.Ui.FrameworkComponentProfile.Kind.InheritanceRemove, Count);
        var hash = Hash(key);
        var removed = false;
        var root = _root?.Remove(key, hash, 0, ref removed);
        return removed ? new(root, Count - 1) : this;
    }
    public bool containsKey(TKey key) => TryGetValue(key, out _);
    public bool ContainsKey(TKey key) => containsKey(key);
    public TValue? valueFor(TKey key) => TryGetValue(key, out var value) ? value : default;
    public bool TryGetValue(TKey key, out TValue value)
    {
        var hash = Hash(key);
        if (_root is not null) return _root.Find(key, hash, 0, out value!);
        value = default!;
        return false;
    }
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => (_root?.Entries() ?? []).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
