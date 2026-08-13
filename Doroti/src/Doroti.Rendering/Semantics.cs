using Doroti.Graphics;
using Doroti.Platform;

namespace Doroti.Rendering;

public sealed class SemanticsConfiguration
{
    private readonly Dictionary<SemanticsAction, Action> _handlers = [];

    public bool IsSemanticBoundary { get; set; }

    public SemanticsRole Role { get; set; }

    public string? Label { get; set; }

    public string? Value { get; set; }

    public SemanticsState State { get; set; } = SemanticsState.Enabled;

    public int? IndexInParent { get; set; }

    public SemanticsAction Actions => _handlers.Keys.Aggregate(SemanticsAction.None, (current, action) => current | action);

    public bool HasSemantics => IsSemanticBoundary || Role != SemanticsRole.Generic || Label is not null || Value is not null || _handlers.Count != 0;

    public void On(SemanticsAction action, Action handler)
    {
        if (action is SemanticsAction.None || !IsSingleFlag(action))
        {
            throw new ArgumentOutOfRangeException(nameof(action), "A semantics handler must target one action.");
        }
        _handlers[action] = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    internal IReadOnlyDictionary<SemanticsAction, Action> Handlers => _handlers;

    private static bool IsSingleFlag(SemanticsAction action)
    {
        var value = (int)action;
        return (value & (value - 1)) == 0;
    }
}

public sealed class SemanticsOwner
{
    private readonly Dictionary<(int Id, SemanticsAction Action), Action> _actions = [];
    private long _generation;

    public SemanticsTreeSnapshot? Snapshot { get; private set; }

    public SemanticsTreeSnapshot Build(RenderBox root)
    {
        ArgumentNullException.ThrowIfNull(root);
        _actions.Clear();
        var ids = new IdGenerator();
        var nodes = BuildNodes(root, ids);
        var rootNode = nodes.Count == 1
            ? nodes[0]
            : new SemanticsNodeSnapshot(0, SemanticsRole.Generic, null, null, SemanticsState.Enabled, SemanticsAction.None,
                Rect.FromLeftTopWidthHeight(0, 0, root.Size.Width, root.Size.Height), nodes);
        Snapshot = new(++_generation, rootNode);
        return Snapshot;
    }

    public bool PerformAction(int id, SemanticsAction action)
    {
        if (!_actions.TryGetValue((id, action), out var handler))
        {
            return false;
        }
        handler();
        return true;
    }

    private IReadOnlyList<SemanticsNodeSnapshot> BuildNodes(RenderObject renderObject, IdGenerator ids)
    {
        var children = new List<SemanticsNodeSnapshot>();
        renderObject.VisitChildrenForSemantics(child => children.AddRange(BuildNodes(child, ids)));
        var configuration = new SemanticsConfiguration();
        renderObject.DescribeSemanticsConfiguration(configuration);
        if (!configuration.HasSemantics)
        {
            return children;
        }
        var id = ids.Next++;
        foreach (var handler in configuration.Handlers)
        {
            _actions[(id, handler.Key)] = handler.Value;
        }
        var bounds = renderObject is RenderBox box && box.Attached
            ? BoundsInGlobal(box)
            : Rect.Zero;
        return [new(id, configuration.Role, configuration.Label, configuration.Value, configuration.State, configuration.Actions, bounds, children, configuration.IndexInParent)];
    }

    private static Rect BoundsInGlobal(RenderBox box)
    {
        var origin = box.LocalToGlobal(Offset.Zero);
        return Rect.FromLeftTopWidthHeight(origin.X, origin.Y, box.Size.Width, box.Size.Height);
    }

    private sealed class IdGenerator
    {
        internal int Next { get; set; } = 1;
    }
}
