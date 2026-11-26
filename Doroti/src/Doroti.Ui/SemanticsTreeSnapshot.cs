namespace Doroti.Ui;

/// <summary>Semantic ownership is independent of already-projected view coordinates.</summary>
public sealed class SemanticsTreeSnapshot
{
    private readonly Dictionary<int, IReadOnlyList<int>> _children = [];
    public IReadOnlyDictionary<int, SemanticsNodeUpdate> Nodes { get; }
    public IReadOnlyDictionary<int, int> Parents { get; }
    public IReadOnlyList<int> Roots { get; }

    public SemanticsTreeSnapshot(IEnumerable<SemanticsNodeUpdate> nodes)
        : this(new System.Collections.ObjectModel.ReadOnlyDictionary<int, SemanticsNodeUpdate>(
            nodes.ToDictionary(n => n.id, n => n with { children = n.children.ToArray() })))
    {
    }

    /// <summary>Indexes the topology of a retained dictionary whose values can be replaced in place.</summary>
    public static SemanticsTreeSnapshot IndexRetainedNodes(IReadOnlyDictionary<int, SemanticsNodeUpdate> nodes) =>
        new(nodes);

    private SemanticsTreeSnapshot(IReadOnlyDictionary<int, SemanticsNodeUpdate> nodes)
    {
        var parents = new Dictionary<int, int>();
        foreach (var node in nodes.Values)
        {
            if (node.children.Count == 0) continue;
            var children = node.children.Where(nodes.ContainsKey).Distinct()
                .OrderBy(child => nodes[child].indexInParent ?? int.MaxValue).ToArray();
            _children.Add(node.id, children);
            foreach (var child in children)
            {
                if (child == node.id || !parents.TryAdd(child, node.id))
                    throw new ArgumentException("A semantic node must have one parent and cannot parent itself.", nameof(nodes));
            }
        }
        var checkedNodes = new HashSet<int>();
        var path = new HashSet<int>();
        foreach (var id in nodes.Keys)
        {
            if (checkedNodes.Contains(id)) continue;
            var current = id;
            while (!checkedNodes.Contains(current))
            {
                if (!path.Add(current)) throw new ArgumentException("Semantic hierarchy contains a cycle.", nameof(nodes));
                if (!parents.TryGetValue(current, out current)) break;
            }
            checkedNodes.UnionWith(path);
            path.Clear();
        }
        Nodes = nodes;
        Parents = new System.Collections.ObjectModel.ReadOnlyDictionary<int, int>(parents);
        Roots = Array.AsReadOnly(nodes.Keys.Where(id => !parents.ContainsKey(id))
            .OrderBy(id => nodes[id].indexInParent ?? int.MaxValue).ThenBy(id => id).ToArray());
    }

    public IReadOnlyList<int> Children(int id) => _children.GetValueOrDefault(id) ?? [];
}

public static class SemanticsActionPolicy
{
    public static bool Allows(SemanticsNodeUpdate node, SemanticsAction action) => action != 0 && Enum.IsDefined(action)
        && node.actions.HasFlag(action) && node.flags?.isHidden != true && node.flags?.isEnabled != Tristate.isFalse
        && !(node.flags?.isReadOnly == true && action is SemanticsAction.setText or SemanticsAction.cut
            or SemanticsAction.paste or SemanticsAction.increase or SemanticsAction.decrease)
        && !(node.flags?.isObscured == true && action is SemanticsAction.copy or SemanticsAction.cut);
}
