// <doroti-reviewed-framework-source />
// .NET representation of Flutter 56b8e1a8 persistent_hash_map.dart.
using System.Numerics;

namespace Doroti.Framework.Foundation;

// Immutable bitmap HAMT. Nodes and arrays are immutable after publication.
// No map/node pooling: older inherited snapshots can outlive the current tree.
internal abstract class PersistentHashTrie<TKey, TValue> where TKey : notnull
{
    internal abstract bool Find(TKey key, uint hash, int shift, out TValue value);
    internal abstract PersistentHashTrie<TKey, TValue> Put(TKey key, TValue value, uint hash, int shift, ref bool added);
    internal abstract PersistentHashTrie<TKey, TValue>? Remove(TKey key, uint hash, int shift, ref bool removed);
    internal abstract IEnumerable<KeyValuePair<TKey, TValue>> Entries();

    internal sealed class Leaf(uint hash, KeyValuePair<TKey, TValue>[] entries) : PersistentHashTrie<TKey, TValue>
    {
        internal readonly uint Hash = hash;
        internal override bool Find(TKey key, uint hash, int shift, out TValue value)
        {
            if (hash == Hash)
                foreach (var pair in entries)
                    if (EqualityComparer<TKey>.Default.Equals(pair.Key, key)) { value = pair.Value; return true; }
            value = default!;
            return false;
        }
        internal override PersistentHashTrie<TKey, TValue> Put(TKey key, TValue value, uint hash, int shift, ref bool added)
        {
            if (hash != Hash)
            {
                added = true;
                return Merge(this, new Leaf(hash, [new(key, value)]), shift);
            }
            for (var i = 0; i < entries.Length; i++)
                if (EqualityComparer<TKey>.Default.Equals(entries[i].Key, key))
                {
                    var copy = (KeyValuePair<TKey, TValue>[])entries.Clone();
                    // Dictionary retains the original key and always stores the new value.
                    copy[i] = new(entries[i].Key, value);
                    return new Leaf(Hash, copy);
                }
            var expanded = new KeyValuePair<TKey, TValue>[entries.Length + 1];
            Array.Copy(entries, expanded, entries.Length);
            expanded[^1] = new(key, value);
            added = true;
            return new Leaf(Hash, expanded);
        }
        internal override PersistentHashTrie<TKey, TValue>? Remove(TKey key, uint hash, int shift, ref bool removed)
        {
            if (hash != Hash) return this;
            for (var i = 0; i < entries.Length; i++)
                if (EqualityComparer<TKey>.Default.Equals(entries[i].Key, key))
                {
                    removed = true;
                    if (entries.Length == 1) return null;
                    return new Leaf(Hash, Without(entries, i));
                }
            return this;
        }
        internal override IEnumerable<KeyValuePair<TKey, TValue>> Entries() => entries;
    }

    private sealed class Branch(uint bitmap, PersistentHashTrie<TKey, TValue>[] children) : PersistentHashTrie<TKey, TValue>
    {
        internal override bool Find(TKey key, uint hash, int shift, out TValue value)
        {
            var bit = Bit(hash, shift);
            if ((bitmap & bit) != 0) return children[Index(bitmap, bit)].Find(key, hash, shift + 5, out value);
            value = default!;
            return false;
        }
        internal override PersistentHashTrie<TKey, TValue> Put(TKey key, TValue value, uint hash, int shift, ref bool added)
        {
            var bit = Bit(hash, shift);
            var index = Index(bitmap, bit);
            if ((bitmap & bit) != 0)
            {
                var copy = (PersistentHashTrie<TKey, TValue>[])children.Clone();
                copy[index] = children[index].Put(key, value, hash, shift + 5, ref added);
                return new Branch(bitmap, copy);
            }
            var expanded = new PersistentHashTrie<TKey, TValue>[children.Length + 1];
            Array.Copy(children, 0, expanded, 0, index);
            expanded[index] = new Leaf(hash, [new(key, value)]);
            Array.Copy(children, index, expanded, index + 1, children.Length - index);
            added = true;
            return new Branch(bitmap | bit, expanded);
        }
        internal override PersistentHashTrie<TKey, TValue>? Remove(TKey key, uint hash, int shift, ref bool removed)
        {
            var bit = Bit(hash, shift);
            if ((bitmap & bit) == 0) return this;
            var index = Index(bitmap, bit);
            var child = children[index].Remove(key, hash, shift + 5, ref removed);
            if (!removed) return this;
            if (child is not null)
            {
                if (children.Length == 1 && child is Leaf) return child;
                var copy = (PersistentHashTrie<TKey, TValue>[])children.Clone();
                copy[index] = child;
                return new Branch(bitmap, copy);
            }
            if (children.Length == 1) return null;
            var remaining = Without(children, index);
            // A leaf uses the complete hash; a branch must retain its shift level.
            if (remaining.Length == 1 && remaining[0] is Leaf) return remaining[0];
            return new Branch(bitmap & ~bit, remaining);
        }
        internal override IEnumerable<KeyValuePair<TKey, TValue>> Entries()
        {
            foreach (var child in children)
                foreach (var pair in child.Entries()) yield return pair;
        }
    }
    private static uint Bit(uint hash, int shift) => 1U << (int)((hash >> shift) & 31);
    private static int Index(uint bitmap, uint bit) => BitOperations.PopCount(bitmap & (bit - 1));
    private static PersistentHashTrie<TKey, TValue> Merge(Leaf left, Leaf right, int shift)
    {
        var a = Bit(left.Hash, shift);
        var b = Bit(right.Hash, shift);
        if (a == b) return new Branch(a, [Merge(left, right, shift + 5)]);
        return new Branch(a | b, a < b ? [left, right] : [right, left]);
    }
    private static T[] Without<T>(T[] source, int index)
    {
        var result = new T[source.Length - 1];
        Array.Copy(source, 0, result, 0, index);
        Array.Copy(source, index + 1, result, index, result.Length - index);
        return result;
    }
}
