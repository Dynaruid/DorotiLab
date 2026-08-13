using Doroti.Core;
using Doroti.Graphics;
using Doroti.Rendering;

namespace Doroti.Widgets;

[Flags]
public enum KeyboardModifiers
{
    None = 0,
    Shift = 1,
    Control = 2,
    Alt = 4,
    Meta = 8,
}

public sealed class FocusNode : ChangeNotifier
{
    private bool _canRequestFocus;
    private bool _skipTraversal;
    private FocusManager? _manager;

    public FocusNode(string? debugLabel = null, bool canRequestFocus = true, bool skipTraversal = false)
    {
        DebugLabel = debugLabel;
        _canRequestFocus = canRequestFocus;
        _skipTraversal = skipTraversal;
    }

    public string? DebugLabel { get; }

    public bool HasFocus => ReferenceEquals(_manager?.PrimaryFocus, this);

    public bool CanRequestFocus
    {
        get => _canRequestFocus;
        set
        {
            if (_canRequestFocus == value)
            {
                return;
            }
            _canRequestFocus = value;
            if (!value && HasFocus)
            {
                _manager!.ClearFocus(this);
            }
            NotifyListeners();
        }
    }

    public bool SkipTraversal
    {
        get => _skipTraversal;
        set
        {
            if (_skipTraversal != value)
            {
                _skipTraversal = value;
                NotifyListeners();
            }
        }
    }

    public bool RequestFocus() => _manager?.RequestFocus(this) is true;

    public void Unfocus() => _manager?.ClearFocus(this);

    internal void Attach(FocusManager manager)
    {
        if (_manager is not null && !ReferenceEquals(_manager, manager))
        {
            throw new InvalidOperationException("A FocusNode cannot be attached to two FocusManagers.");
        }
        _manager = manager;
        manager.Register(this);
    }

    internal void Detach(FocusManager manager)
    {
        if (ReferenceEquals(_manager, manager))
        {
            manager.Unregister(this);
            _manager = null;
        }
    }

    internal void NotifyFocusChanged() => NotifyListeners();

    public override string ToString() => DebugLabel ?? base.ToString()!;
}

public sealed class FocusManager
{
    private readonly List<FocusNode> _nodes = [];

    public FocusNode? PrimaryFocus { get; private set; }

    public IReadOnlyList<FocusNode> TraversalOrder => _nodes.ToArray();

    public bool RequestFocus(FocusNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (!_nodes.Contains(node) || !node.CanRequestFocus)
        {
            return false;
        }
        if (ReferenceEquals(PrimaryFocus, node))
        {
            return true;
        }
        var previous = PrimaryFocus;
        PrimaryFocus = node;
        previous?.NotifyFocusChanged();
        node.NotifyFocusChanged();
        return true;
    }

    public bool NextFocus(bool reverse = false)
    {
        var candidates = _nodes.Where(node => node.CanRequestFocus && !node.SkipTraversal).ToArray();
        if (candidates.Length == 0)
        {
            return false;
        }
        var current = Array.IndexOf(candidates, PrimaryFocus);
        var next = reverse
            ? current <= 0 ? candidates.Length - 1 : current - 1
            : current < 0 || current == candidates.Length - 1 ? 0 : current + 1;
        return RequestFocus(candidates[next]);
    }

    public void ClearFocus(FocusNode node)
    {
        if (ReferenceEquals(PrimaryFocus, node))
        {
            PrimaryFocus = null;
            node.NotifyFocusChanged();
        }
    }

    internal void Register(FocusNode node)
    {
        if (!_nodes.Contains(node))
        {
            _nodes.Add(node);
        }
    }

    internal void Unregister(FocusNode node)
    {
        ClearFocus(node);
        _nodes.Remove(node);
    }
}

public abstract record Intent;

public sealed record ActivateIntent : Intent;

public sealed record NextFocusIntent(bool Reverse = false) : Intent;

public readonly record struct ShortcutActivator(uint LogicalKey, KeyboardModifiers Modifiers = KeyboardModifiers.None);

public interface IWidgetAction
{
    bool Invoke(Intent intent);
}

public sealed class CallbackAction<TIntent>(Func<TIntent, bool> callback) : IWidgetAction
    where TIntent : Intent
{
    public bool Invoke(Intent intent) => intent is TIntent typed && callback(typed);
}

public sealed class ShortcutMap
{
    private readonly IReadOnlyDictionary<ShortcutActivator, Intent> _bindings;

    public ShortcutMap(IEnumerable<KeyValuePair<ShortcutActivator, Intent>> bindings) =>
        _bindings = bindings?.ToDictionary() ?? throw new ArgumentNullException(nameof(bindings));

    public Intent? Find(KeyboardEvent input) =>
        _bindings.TryGetValue(new(input.LogicalKey, input.Modifiers), out var intent) ? intent : null;
}

public interface IFocusableKeyboardTarget : IKeyboardEventTarget
{
    bool RequestFocus();
}

public sealed class Focus : SingleChildRenderObjectWidget
{
    public Focus(
        FocusManager manager,
        FocusNode node,
        Widget? child = null,
        ShortcutMap? shortcuts = null,
        IReadOnlyDictionary<Type, IWidgetAction>? actions = null,
        Key? key = null)
        : base(child, key)
    {
        Manager = manager ?? throw new ArgumentNullException(nameof(manager));
        Node = node ?? throw new ArgumentNullException(nameof(node));
        Shortcuts = shortcuts;
        Actions = actions ?? new Dictionary<Type, IWidgetAction>();
    }

    public FocusManager Manager { get; }

    public FocusNode Node { get; }

    public ShortcutMap? Shortcuts { get; }

    public IReadOnlyDictionary<Type, IWidgetAction> Actions { get; }

    public override RenderObject CreateRenderObject(BuildContext context) => new RenderFocus(Manager, Node, Shortcuts, Actions);

    public override void UpdateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var focus = (RenderFocus)renderObject;
        focus.Update(Manager, Node, Shortcuts, Actions);
    }
}

public sealed class RenderFocus : RenderProxyBox, IPointerEventTarget, IFocusableKeyboardTarget, IDisposable
{
    private FocusManager _manager;
    private FocusNode _node;
    private ShortcutMap? _shortcuts;
    private IReadOnlyDictionary<Type, IWidgetAction> _actions;

    public RenderFocus(
        FocusManager manager,
        FocusNode node,
        ShortcutMap? shortcuts,
        IReadOnlyDictionary<Type, IWidgetAction> actions)
    {
        _manager = manager;
        _node = node;
        _shortcuts = shortcuts;
        _actions = actions;
        _node.Attach(_manager);
    }

    public void Update(
        FocusManager manager,
        FocusNode node,
        ShortcutMap? shortcuts,
        IReadOnlyDictionary<Type, IWidgetAction> actions)
    {
        if (!ReferenceEquals(_manager, manager) || !ReferenceEquals(_node, node))
        {
            _node.Detach(_manager);
            _manager = manager;
            _node = node;
            _node.Attach(_manager);
        }
        _shortcuts = shortcuts;
        _actions = actions;
    }

    public bool RequestFocus() => _manager.RequestFocus(_node);

    public void HandlePointerEvent(PointerEvent input)
    {
        if (input.Phase is PointerEventPhase.Down)
        {
            RequestFocus();
        }
    }

    public bool HandleKeyboardEvent(KeyboardEvent input)
    {
        if (!_node.HasFocus || input.Phase is not KeyboardEventPhase.Down)
        {
            return false;
        }
        if (input.LogicalKey == 0x09)
        {
            return _manager.NextFocus(input.Modifiers.HasFlag(KeyboardModifiers.Shift));
        }
        var intent = _shortcuts?.Find(input);
        return intent is not null && _actions.TryGetValue(intent.GetType(), out var action) && action.Invoke(intent);
    }

    public void Dispose() => _node.Detach(_manager);

    protected override void PerformLayout()
    {
        if (Child is null)
        {
            SetSize(Constraints.Constrain(Size.Zero));
            return;
        }
        Child.Layout(Constraints, parentUsesSize: true);
        SetSize(Constraints.Constrain(Child.Size));
        ((BoxParentData)Child.ParentData!).Offset = Offset.Zero;
    }

    protected override bool HitTestSelf(Offset position) => true;
}
