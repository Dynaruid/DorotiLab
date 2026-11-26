namespace Doroti.Ui;

public enum SemanticsPlatformViewExposure
{
    Include,
    ExcludeNode,
    ExcludeSubtree,
}

/// <summary>Retains the exposed tree and filters full or projected snapshots against the applied state.</summary>
public sealed class SemanticsVisibleTree(SemanticsPlatformViewExposure platformViews, bool filterChildren = true)
{
    private readonly Dictionary<int, SemanticsNodeUpdate> _nodes = [];
    private IReadOnlyList<SemanticsNodeUpdate>? _source;
    private IReadOnlyList<int> _order = [];

    public IReadOnlyDictionary<int, SemanticsNodeUpdate> Nodes => _nodes;
    public IReadOnlyList<int> Order => _order;

    public SemanticsVisibleUpdate Prepare(SemanticsUpdate update)
    {
        if (ReferenceEquals(update.nodes, _source)) return new(update.nodes, [], [], null);
        if (update.snapshotChanges is { } projected && ReferenceEquals(projected.previousNodes, _source))
            return CompareRetained(update.nodes, projected.changedNodes);

        if (_source is { } source && source.Count == update.nodes.Count)
        {
            using var previous = source.GetEnumerator();
            foreach (var current in update.nodes)
            {
                previous.MoveNext();
                if (!SameShape(previous.Current, current)) return CompareSnapshot(update.nodes);
            }
            return CompareRetained(update.nodes, update.nodes);
        }
        return CompareSnapshot(update.nodes);
    }

    public void Commit(SemanticsVisibleUpdate update)
    {
        foreach (var id in update.Removed) _nodes.Remove(id);
        foreach (var change in update.Changes) _nodes[change.Current.id] = change.Current;
        if (update.Order is { } order) _order = order;
        _source = update.Source;
    }

    public void Clear()
    {
        _nodes.Clear();
        _source = null;
        _order = [];
    }

    private SemanticsVisibleUpdate CompareRetained(
        IReadOnlyList<SemanticsNodeUpdate> source, IEnumerable<SemanticsNodeUpdate> candidates)
    {
        var changes = new List<SemanticsVisibleNodeChange>();
        foreach (var current in candidates)
        {
            if (!_nodes.TryGetValue(current.id, out var previous) || ReferenceEquals(previous, current)) continue;
            var visible = filterChildren && !ReferenceEquals(current.children, previous.children)
                ? current with { children = previous.children }
                : current;
            var properties = SemanticsUpdateDiffer.ChangedProperties(previous, visible);
            if (properties != SemanticsNodeProperty.none) changes.Add(new(previous, visible, properties));
        }
        return new(source, changes, [], null);
    }

    private SemanticsVisibleUpdate CompareSnapshot(IReadOnlyList<SemanticsNodeUpdate> source)
    {
        var byId = source.ToDictionary(node => node.id);
        var hidden = new HashSet<int>();
        var pending = new Stack<int>();
        foreach (var node in source)
            if (node.flags?.isHidden == true
                || (platformViews == SemanticsPlatformViewExposure.ExcludeSubtree && node.platformViewId is not null))
                pending.Push(node.id);
        while (pending.TryPop(out var id))
            if (hidden.Add(id) && byId.TryGetValue(id, out var node))
                foreach (var child in node.children) pending.Push(child);
        if (platformViews == SemanticsPlatformViewExposure.ExcludeNode)
            foreach (var node in source)
                if (node.platformViewId is not null) hidden.Add(node.id);

        var changes = new List<SemanticsVisibleNodeChange>();
        var order = new List<int>();
        foreach (var current in source)
        {
            if (hidden.Contains(current.id)) continue;
            order.Add(current.id);
            var visible = filterChildren && current.children.Any(id => hidden.Contains(id) || !byId.ContainsKey(id))
                ? current with { children = current.children.Where(id => !hidden.Contains(id) && byId.ContainsKey(id)).ToArray() }
                : current;
            _nodes.TryGetValue(visible.id, out var previous);
            var properties = previous is null
                ? SemanticsNodeProperty.none
                : SemanticsUpdateDiffer.ChangedProperties(previous, visible);
            if (previous is null || properties != SemanticsNodeProperty.none)
                changes.Add(new(previous, visible, properties));
        }
        var removed = _nodes.Keys.Where(id => hidden.Contains(id) || !byId.ContainsKey(id)).ToArray();
        return new(source, changes, removed, order.SequenceEqual(_order) ? null : order);
    }

    private bool SameExposure(SemanticsNodeUpdate previous, SemanticsNodeUpdate current) =>
        (previous.flags?.isHidden == true) == (current.flags?.isHidden == true)
        && (platformViews == SemanticsPlatformViewExposure.Include
            || (previous.platformViewId is null) == (current.platformViewId is null));

    private bool SameShape(SemanticsNodeUpdate previous, SemanticsNodeUpdate current) =>
        ReferenceEquals(previous, current) || (previous.id == current.id && SameExposure(previous, current)
            && previous.indexInParent == current.indexInParent && previous.traversalParent == current.traversalParent
            && previous.children.SequenceEqual(current.children));
}

public sealed record SemanticsVisibleUpdate(
    IReadOnlyList<SemanticsNodeUpdate> Source,
    IReadOnlyList<SemanticsVisibleNodeChange> Changes,
    IReadOnlyList<int> Removed,
    IReadOnlyList<int>? Order)
{
    public bool HasChanges => Changes.Count != 0 || Removed.Count != 0 || Order is not null;
    public bool TopologyChanged => Order is not null || Removed.Count != 0
        || Changes.Any(change => change.Previous is null
            || (change.Properties & (SemanticsNodeProperty.children | SemanticsNodeProperty.traversal)) != 0);
}

public readonly record struct SemanticsVisibleNodeChange(
    SemanticsNodeUpdate? Previous, SemanticsNodeUpdate Current, SemanticsNodeProperty Properties);
