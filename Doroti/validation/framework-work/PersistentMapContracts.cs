using System.Runtime.CompilerServices;
using Doroti.Framework.Foundation;

internal static class PersistentMapContracts
{
    internal static void Verify()
    {
        foreach (var count in new[] { 4, 16, 64, 29, 290, 4096 })
        {
            var map = new PersistentHashMap<Key, Value?>();
            var oracle = new Dictionary<Key, Value?>();
            var snapshots = new List<(PersistentHashMap<Key, Value?> Map, Dictionary<Key, Value?> Expected)>();
            for (var i = 0; i < count; i++)
            {
                // Shared low 30 bits, negative hashes, and true collisions.
                var key = new Key(i, i % 4 == 0 ? 7 : unchecked(i * 1073741824 + i / 4));
                var value = i % 7 == 0 ? null : new Value(i);
                map = map.put(key, value); oracle[key] = value;
                if (i is 0 or 3 or 15 || i == count - 1) snapshots.Add((map, new(oracle)));
            }
            Check(map, oracle);
            foreach (var key in oracle.Keys.ToArray())
            {
                var next = new Value(key.Id); // Equal values must still replace references.
                map = map.put(new Key(key.Id, key.Hash), next); oracle[key] = next;
                Require(ReferenceEquals(map[key], next), "replacement reference");
            }
            Check(map, oracle);
            foreach (var key in oracle.Keys.Where(k => k.Id % 2 == 0).ToArray())
            { map = map.remove(key); oracle.Remove(key); }
            Check(map, oracle);
            foreach (var (snapshot, expected) in snapshots) Check(snapshot, expected);
            var missing = new Key(-1, 7);
            Require(ReferenceEquals(map, map.remove(missing)), "absent removal identity");
            Require(!map.TryGetValue(missing, out _) && map.valueFor(missing) is null, "missing lookup");
            foreach (var key in oracle.Keys.ToArray()) map = map.remove(key);
            Require(map.Count == 0 && map.isEmpty && !map.isNotEmpty && !map.Any(), "empty after removals");
        }
        var keyed = new Key(1, -1);
        var first = new PersistentHashMap<Key, Value>().put(keyed, new(1));
        Require(ReferenceEquals(first.put(new(1, -1), new(2)).Keys.Single(), keyed), "original equal key retained");
        try { new PersistentHashMap<string, int>().put(null!, 1); throw new Exception("null accepted"); }
        catch (ArgumentNullException) { }
        var lifetime = RemovedReference();
        GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
        Require(!lifetime.Value.IsAlive && lifetime.Map.Count == 0, "removed value is not rooted by empty snapshot");
        Console.WriteLine("PASS persistent map snapshots, collisions, high hash bits, nullable values, key/value identity, enumeration, remove, lifetime (4/16/64/29/290/4096)");
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (PersistentHashMap<int, object> Map, WeakReference Value) RemovedReference()
    {
        var value = new object();
        return (new PersistentHashMap<int, object>().put(1, value).remove(1), new(value));
    }
    private static void Check(PersistentHashMap<Key, Value?> map, Dictionary<Key, Value?> expected)
    {
        Require(map.Count == expected.Count, "count");
        foreach (var (key, value) in expected)
            Require(map.ContainsKey(key) && map.TryGetValue(key, out var actual) && ReferenceEquals(value, actual), "lookup");
        Require(map.Select(p => p.Key.Id).Order().SequenceEqual(expected.Keys.Select(k => k.Id).Order()), "enumeration membership");
        Require(map.Keys.Count() == map.Count && map.Values.Count() == map.Count, "keys/values");
    }
    private static void Require(bool result, string message) { if (!result) throw new Exception(message); }
    private sealed record Value(int Id);
    private sealed class Key(int id, int hash) : IEquatable<Key>
    {
        internal int Id => id;
        internal int Hash => hash;
        public bool Equals(Key? other) => other?.Id == id;
        public override bool Equals(object? obj) => obj is Key other && Equals(other);
        public override int GetHashCode() => hash;
    }
}
