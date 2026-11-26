using System.Collections.Immutable;

namespace Doroti.Ui;

/// <summary>Projects dirty framework nodes into one view's immutable accessibility snapshot.</summary>
public sealed class RetainedSemanticsProjection
{
    private readonly Dictionary<int, Entry> _entries = [];
    private ImmutableList<SemanticsNodeUpdate> _snapshot = [];
    private double _devicePixelRatio = 1;

    public SemanticsUpdate Update(SemanticsUpdate update)
    {
        var ratio = update.viewDevicePixelRatio;
        if (!double.IsFinite(ratio) || ratio <= 0) throw new ArgumentOutOfRangeException(nameof(update));

        var previous = _snapshot;
        var topologyChanged = false;
        var exposureChanged = false;
        List<Entry>? dirty = null;
        foreach (var node in update.nodes)
        {
            if (!_entries.TryGetValue(node.id, out var entry))
            {
                _entries.Add(node.id, new(node));
                topologyChanged = true;
                continue;
            }
            if (ReferenceEquals(entry.Source, node)) continue;
            topologyChanged |= entry.Source.indexInParent != node.indexInParent
                || entry.Source.traversalParent != node.traversalParent
                || !entry.Source.children.SequenceEqual(node.children);
            exposureChanged |= (entry.Source.flags?.isHidden == true) != (node.flags?.isHidden == true)
                || (entry.Source.platformViewId is null) != (node.platformViewId is null);
            entry.Source = node;
            (dirty ??= []).Add(entry);
        }

        var ratioChanged = ratio != _devicePixelRatio;
        _devicePixelRatio = ratio;
        if (topologyChanged)
        {
            RebuildHierarchy();
            _snapshot = _entries.Values.OrderBy(entry => entry.Source.indexInParent ?? int.MaxValue)
                .ThenBy(entry => entry.Source.id)
                .Select((entry, index) =>
                {
                    entry.Index = index;
                    return entry.Projected!;
                }).ToImmutableList();
            return new(update.generation, _snapshot, update.urgency);
        }

        if (ratioChanged)
            (dirty ??= []).AddRange(_entries.Values.Where(entry => entry.Parent is null));
        List<SemanticsNodeUpdate>? changed = null;
        if (dirty is not null)
        {
            var pending = new Stack<(Entry Node, Matrix4 ParentTransform)>();
            dirty.Sort(static (left, right) => left.Depth.CompareTo(right.Depth));
            foreach (var entry in dirty)
            {
                // The parent can have updated this node already.
                if (ReferenceEquals(entry.ProjectedSource, entry.Source) && !ratioChanged) continue;
                pending.Push((entry, entry.Parent?.ChildTransform ?? RootTransform(entry.Source)));
                while (pending.TryPop(out var next))
                {
                    var old = next.Node.Projected;
                    var oldChildTransform = next.Node.ChildTransform;
                    Project(next.Node, next.ParentTransform);
                    if (!ReferenceEquals(old, next.Node.Projected))
                    {
                        _snapshot = _snapshot.SetItem(next.Node.Index, next.Node.Projected!);
                        (changed ??= []).Add(next.Node.Projected!);
                    }
                    if (ReferenceEquals(oldChildTransform, next.Node.ChildTransform)) continue;
                    foreach (var child in next.Node.Children) pending.Push((child, next.Node.ChildTransform!));
                }
            }
        }

        return new(update.generation, _snapshot, update.urgency)
        {
            snapshotChanges = exposureChanged ? null : new(previous, changed ?? []),
        };
    }

    public void Clear()
    {
        _entries.Clear();
        _snapshot = [];
        _devicePixelRatio = 1;
    }

    private void RebuildHierarchy()
    {
        // A framework root defines reachability. Legacy snapshots can be a forest.
        if (_entries.ContainsKey(0))
        {
            var reachable = new HashSet<int>();
            var pending = new Stack<int>();
            pending.Push(0);
            while (pending.TryPop(out var id))
                if (_entries.TryGetValue(id, out var node) && reachable.Add(id))
                    foreach (var child in node.Source.children) pending.Push(child);
            foreach (var id in _entries.Keys.Where(id => !reachable.Contains(id)).ToArray())
                _entries.Remove(id);
        }

        var childIds = _entries.Values.SelectMany(entry => entry.Source.children).ToHashSet();
        var ordered = _entries.Values.OrderBy(entry => entry.Source.id).ToArray();
        foreach (var entry in ordered)
        {
            entry.Parent = null;
            entry.Children.Clear();
        }
        var visited = new HashSet<Entry>();
        // Visit roots first, then any remaining cyclic inputs in the same order as the reference projection.
        foreach (var root in ordered.Where(entry => !childIds.Contains(entry.Source.id)).Concat(ordered))
        {
            if (visited.Contains(root)) continue;
            var pending = new Stack<(Entry Node, Entry? Parent)>();
            pending.Push((root, null));
            while (pending.TryPop(out var next))
            {
                var entry = next.Node;
                if (!visited.Add(entry)) continue;
                entry.Parent = next.Parent;
                entry.Depth = (next.Parent?.Depth ?? -1) + 1;
                next.Parent?.Children.Add(entry);
                Project(entry, next.Parent?.ChildTransform ?? RootTransform(entry.Source));
                for (var index = entry.Source.children.Count - 1; index >= 0; index--)
                    if (_entries.TryGetValue(entry.Source.children[index], out var child))
                        pending.Push((child, entry));
            }
        }
    }

    private Matrix4 RootTransform(SemanticsNodeUpdate node) => node.coordinateTransform is { Count: 16 }
        ? Matrix4.diagonal3Values(1 / _devicePixelRatio, 1 / _devicePixelRatio, 1)
        : Matrix4.identity();

    private static void Project(Entry entry, Matrix4 parentTransform)
    {
        var source = entry.Source;
        var previous = entry.ProjectedSource;
        var sameTransform = previous is not null && SameMatrix(entry.ParentTransform, parentTransform)
            && (ReferenceEquals(previous.coordinateTransform, source.coordinateTransform)
                || (previous.coordinateTransform ?? []).SequenceEqual(source.coordinateTransform ?? []));
        if (sameTransform && ReferenceEquals(previous, source)) return;

        var explicitTransform = source.coordinateTransform is { Count: 16 };
        var transform = entry.Transform;
        if (!sameTransform)
        {
            transform = new Matrix4(parentTransform.storage);
            if (explicitTransform) transform.multiply(new Matrix4(source.coordinateTransform!));
            entry.Transform = transform;
            entry.ParentTransform = parentTransform;
        }

        var bounds = sameTransform && previous!.rect == source.rect
            ? entry.Projected!.rect
            : SemanticsGeometryProjection.TransformRect(source.rect, transform!);
        var geometry = source.flags?.isObscured == true || source.textGeometry is null
            ? null
            : sameTransform && ReferenceEquals(previous!.textGeometry, source.textGeometry)
                && previous.flags?.isObscured != true && entry.Projected!.rect == bounds
                ? entry.Projected.textGeometry
                : SemanticsGeometryProjection.ProjectText(source.textGeometry, transform!, bounds);
        var projected = source with { rect = bounds, coordinateTransform = null, textGeometry = geometry };
        if (entry.Projected is null
            || SemanticsUpdateDiffer.ChangedProperties(entry.Projected, projected) != SemanticsNodeProperty.none)
            entry.Projected = projected;

        if (!sameTransform || (!explicitTransform && previous!.rect.topLeft != source.rect.topLeft))
        {
            var childTransform = transform!;
            if (!explicitTransform)
            {
                childTransform = new Matrix4(transform!.storage);
                childTransform.multiply(Matrix4.translationValues(source.rect.left, source.rect.top, 0));
            }
            if (!SameMatrix(entry.ChildTransform, childTransform)) entry.ChildTransform = childTransform;
        }
        entry.ProjectedSource = source;
    }

    private static bool SameMatrix(Matrix4? left, Matrix4 right) =>
        ReferenceEquals(left, right) || left is not null && left.storage.SequenceEqual(right.storage);

    private sealed class Entry(SemanticsNodeUpdate source)
    {
        internal SemanticsNodeUpdate Source = source;
        internal SemanticsNodeUpdate? ProjectedSource;
        internal SemanticsNodeUpdate? Projected;
        internal Entry? Parent;
        internal readonly List<Entry> Children = [];
        internal Matrix4? ParentTransform;
        internal Matrix4? Transform;
        internal Matrix4? ChildTransform;
        internal int Index;
        internal int Depth;
    }
}
