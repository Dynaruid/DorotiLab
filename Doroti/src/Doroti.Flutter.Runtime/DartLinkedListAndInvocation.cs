namespace Doroti.Flutter.Runtime;

public sealed class Invocation
{
    public Invocation(object? memberName = null, IReadOnlyList<object?>? positionalArguments = null,
        IReadOnlyDictionary<object, object?>? namedArguments = null)
    {
        this.memberName = memberName;
        this.positionalArguments = positionalArguments ?? [];
        this.namedArguments = namedArguments ?? new Dictionary<object, object?>();
    }

    public object? memberName { get; }
    public IReadOnlyList<object?> positionalArguments { get; }
    public IReadOnlyDictionary<object, object?> namedArguments { get; }
}

public sealed class DeepCollectionEquality
{
    public bool equals(object? left, object? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        if (left is System.Collections.IDictionary leftMap && right is System.Collections.IDictionary rightMap)
        {
            if (leftMap.Count != rightMap.Count) return false;
            foreach (System.Collections.DictionaryEntry entry in leftMap)
            {
                if (!rightMap.Contains(entry.Key) || !equals(entry.Value, rightMap[entry.Key])) return false;
            }
            return true;
        }
        if (left is System.Collections.IEnumerable leftItems && right is System.Collections.IEnumerable rightItems &&
            left is not string && right is not string)
        {
            var leftEnumerator = leftItems.GetEnumerator();
            var rightEnumerator = rightItems.GetEnumerator();
            while (true)
            {
                var hasLeft = leftEnumerator.MoveNext();
                var hasRight = rightEnumerator.MoveNext();
                if (hasLeft != hasRight) return false;
                if (!hasLeft) return true;
                if (!equals(leftEnumerator.Current, rightEnumerator.Current)) return false;
            }
        }
        return object.Equals(left, right);
    }

    public int hash(object? value)
    {
        if (value is null) return 0;
        if (value is System.Collections.IDictionary map)
        {
            var hash = new HashCode();
            foreach (System.Collections.DictionaryEntry entry in map)
            {
                hash.Add(hashCode(entry.Key) ^ hashCode(entry.Value));
            }
            return hash.ToHashCode();
        }
        if (value is System.Collections.IEnumerable items && value is not string)
        {
            var hash = new HashCode();
            foreach (var item in items) hash.Add(hashCode(item));
            return hash.ToHashCode();
        }
        return value.GetHashCode();
    }

    private int hashCode(object? value) => hash(value);
}

public abstract class DartLinkedListEntry<T> where T : DartLinkedListEntry<T>
{
    private DartLinkedList<T>? _list;
    public DartLinkedList<T>? list => _list;
    public T? previous { get; internal set; }
    public T? next { get; internal set; }

    internal void Attach(DartLinkedList<T> list) => _list = list;

    public void insertAfter(T entry)
    {
        if (_list is null) throw new InvalidOperationException("The entry is not linked.");
        _list.InsertAfter((T)this, entry);
    }

    public void unlink() => _list?.remove((T)this);

    internal void Detach()
    {
        _list = null;
        previous = null;
        next = null;
    }
}

public sealed class DartLinkedList<T> : IEnumerable<T> where T : DartLinkedListEntry<T>
{
    public T? first { get; private set; }
    public T? last { get; private set; }
    public bool isEmpty => first is null;

    public bool contains(T entry) => this.Contains(entry);

    public void addFirst(T entry)
    {
        if (contains(entry)) throw new InvalidOperationException("The entry is already linked.");
        entry.next = first;
        if (first is not null) first.previous = entry;
        first = entry;
        last ??= entry;
        entry.Attach(this);
    }

    public void add(T entry)
    {
        if (last is null) addFirst(entry);
        else InsertAfter(last, entry);
    }

    internal void InsertAfter(T existing, T entry)
    {
        if (contains(entry)) throw new InvalidOperationException("The entry is already linked.");
        entry.previous = existing;
        entry.next = existing.next;
        if (existing.next is not null) existing.next.previous = entry;
        existing.next = entry;
        if (ReferenceEquals(last, existing)) last = entry;
        entry.Attach(this);
    }

    public bool remove(T entry)
    {
        if (!contains(entry)) return false;
        if (entry.previous is not null) entry.previous.next = entry.next;
        else first = entry.next;
        if (entry.next is not null) entry.next.previous = entry.previous;
        else last = entry.previous;
        entry.Detach();
        return true;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (var current = first; current is not null; current = current.next) yield return current;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
