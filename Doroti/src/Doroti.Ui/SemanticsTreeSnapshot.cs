namespace Doroti.Ui;

/// <summary>Semantic ownership is independent of already-projected view coordinates.</summary>
public sealed class SemanticsTreeSnapshot
{
    public IReadOnlyDictionary<int, SemanticsNodeUpdate> Nodes { get; }
    public IReadOnlyDictionary<int, int> Parents { get; }
    public IReadOnlyList<int> Roots { get; }

    public SemanticsTreeSnapshot(IEnumerable<SemanticsNodeUpdate> nodes)
    {
        var values = nodes.ToDictionary(n => n.id, n => n with { children = n.children.ToArray() });
        var parents = new Dictionary<int, int>();
        foreach (var node in values.Values)
            foreach (var child in node.children.Where(values.ContainsKey).Distinct())
            {
                if (child == node.id || !parents.TryAdd(child, node.id))
                    throw new ArgumentException("A semantic node must have one parent and cannot parent itself.", nameof(nodes));
            }
        foreach (var node in values.Values)
        {
            var visited = new HashSet<int> { node.id };
            var current = node.id;
            while (parents.TryGetValue(current, out current))
                if (!visited.Add(current)) throw new ArgumentException("Semantic hierarchy contains a cycle.", nameof(nodes));
        }
        Nodes = new System.Collections.ObjectModel.ReadOnlyDictionary<int, SemanticsNodeUpdate>(values);
        Parents = new System.Collections.ObjectModel.ReadOnlyDictionary<int, int>(parents);
        Roots = Array.AsReadOnly(values.Keys.Where(id => !parents.ContainsKey(id))
            .OrderBy(id => values[id].indexInParent ?? int.MaxValue).ThenBy(id => id).ToArray());
    }

    public IReadOnlyList<int> Children(int id) => Nodes.TryGetValue(id, out var node)
        ? node.children.Where(Nodes.ContainsKey).OrderBy(child => Nodes[child].indexInParent ?? int.MaxValue).ToArray() : [];
}

public static class SemanticsActionPolicy
{
    public static bool Allows(SemanticsNodeUpdate node, SemanticsAction action) => action != 0 && Enum.IsDefined(action)
        && node.actions.HasFlag(action) && node.flags?.isHidden != true && node.flags?.isEnabled != Tristate.isFalse
        && !(node.flags?.isReadOnly == true && action is SemanticsAction.setText or SemanticsAction.cut
            or SemanticsAction.paste or SemanticsAction.increase or SemanticsAction.decrease)
        && !(node.flags?.isObscured == true && action is SemanticsAction.copy or SemanticsAction.cut);
}
