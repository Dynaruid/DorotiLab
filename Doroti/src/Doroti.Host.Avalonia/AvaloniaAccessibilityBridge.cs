using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Doroti.Platform;
using AvaloniaRect = Avalonia.Rect;

namespace Doroti.Host.Avalonia;

internal sealed class AvaloniaAccessibilityBridge : IAccessibilityBridge, IAccessibilityDiagnostics
{
    private readonly AvaloniaDisplayListControl _control;
    private readonly WindowId _windowId;
    private readonly Func<WindowMetrics> _metrics;
    private readonly AvaloniaHostDiagnostics _diagnostics;
    private DorotiSemanticsRootPeer? _peer;
    private Func<SemanticsActionRequest, bool>? _performAction;

    internal AvaloniaAccessibilityBridge(
        AvaloniaDisplayListControl control,
        WindowId windowId,
        Func<WindowMetrics> metrics,
        AvaloniaHostDiagnostics diagnostics)
    {
        _control = control;
        _windowId = windowId;
        _metrics = metrics;
        _diagnostics = diagnostics;
        control.AttachAccessibility(this);
    }

    public SemanticsTreeSnapshot? LastSnapshot { get; private set; }

    public void Update(SemanticsTreeSnapshot snapshot, Func<SemanticsActionRequest, bool> performAction)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(performAction);
        AvaloniaWindowBackend.RequireUiThread();
        LastSnapshot = snapshot;
        _performAction = performAction;
        _peer?.Refresh();
        _diagnostics.Record(
            "semantics-updated",
            _windowId,
            _metrics(),
            $"generation={snapshot.Generation};nodes={Count(snapshot.Root)}");
    }

    public bool InvokeAction(int nodeId, SemanticsAction action, object? arguments = null)
    {
        AvaloniaWindowBackend.RequireUiThread();
        var handled = _performAction?.Invoke(new(nodeId, action, arguments)) is true;
        _diagnostics.Record(
            "semantics-action",
            _windowId,
            _metrics(),
            $"node={nodeId};action={action};handled={handled}");
        return handled;
    }

    internal AutomationPeer CreatePeer()
    {
        _peer ??= new(_control, this);
        return _peer;
    }

    internal IReadOnlyList<AutomationPeer> CreateRootChildren()
    {
        if (LastSnapshot is not { } snapshot)
        {
            return [];
        }
        return [new DorotiSemanticsNodePeer(_control, this, snapshot.Root)];
    }

    private static int Count(SemanticsNodeSnapshot node) => 1 + node.Children.Sum(Count);
}

internal sealed class DorotiSemanticsRootPeer(
    AvaloniaDisplayListControl owner,
    AvaloniaAccessibilityBridge bridge) : ControlAutomationPeer(owner)
{
    internal void Refresh() => InvalidateChildren();

    protected override IReadOnlyList<AutomationPeer>? GetChildrenCore() => bridge.CreateRootChildren();

    protected override string? GetNameCore() => "Doroti content";

    protected override string? GetAutomationIdCore() => "Doroti.SemanticsRoot";

    protected override string GetClassNameCore() => "DorotiSemanticsRoot";

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Pane;
}

internal sealed class DorotiSemanticsNodePeer : ControlAutomationPeer, IInvokeProvider
{
    private readonly AvaloniaDisplayListControl _owner;
    private readonly AvaloniaAccessibilityBridge _bridge;
    private readonly SemanticsNodeSnapshot _node;

    internal DorotiSemanticsNodePeer(
        AvaloniaDisplayListControl owner,
        AvaloniaAccessibilityBridge bridge,
        SemanticsNodeSnapshot node)
        : base(owner)
    {
        _owner = owner;
        _bridge = bridge;
        _node = node;
    }

    public void Invoke()
    {
        EnsureEnabled();
        if (!_node.Actions.HasFlag(SemanticsAction.Tap) || !_bridge.InvokeAction(_node.Id, SemanticsAction.Tap))
        {
            throw new InvalidOperationException($"Semantics node {_node.Id} does not support tap.");
        }
    }

    protected override IReadOnlyList<AutomationPeer>? GetChildrenCore() =>
        _node.Children.Select(child => (AutomationPeer)new DorotiSemanticsNodePeer(_owner, _bridge, child)).ToArray();

    protected override string? GetNameCore() => _node.Label ?? _node.Value;

    protected override string? GetAutomationIdCore() => $"Doroti.Semantics.{_node.Id}";

    protected override string GetClassNameCore() => $"Doroti{_node.Role}";

    protected override AutomationControlType GetAutomationControlTypeCore() => _node.Role switch
    {
        SemanticsRole.Button => AutomationControlType.Button,
        SemanticsRole.Text => AutomationControlType.Text,
        SemanticsRole.TextField => AutomationControlType.Edit,
        SemanticsRole.Image => AutomationControlType.Image,
        SemanticsRole.List => AutomationControlType.List,
        SemanticsRole.ListItem => AutomationControlType.ListItem,
        SemanticsRole.Dialog => AutomationControlType.Pane,
        _ => AutomationControlType.Custom,
    };

    protected override AvaloniaRect GetBoundingRectangleCore() => new(
        _node.Bounds.Left,
        _node.Bounds.Top,
        _node.Bounds.Width,
        _node.Bounds.Height);

    protected override bool HasKeyboardFocusCore() => _node.State.HasFlag(SemanticsState.Focused);

    protected override bool IsEnabledCore() => _node.State.HasFlag(SemanticsState.Enabled);

    protected override bool IsKeyboardFocusableCore() => _node.Actions.HasFlag(SemanticsAction.Focus);

    protected override bool IsOffscreenCore() => _node.State.HasFlag(SemanticsState.Hidden);

    protected override void SetFocusCore()
    {
        EnsureEnabled();
        if (!_node.Actions.HasFlag(SemanticsAction.Focus) || !_bridge.InvokeAction(_node.Id, SemanticsAction.Focus))
        {
            throw new InvalidOperationException($"Semantics node {_node.Id} does not support focus.");
        }
    }

    protected override object? GetProviderCore(Type providerType)
    {
        if (providerType == typeof(IInvokeProvider) && !_node.Actions.HasFlag(SemanticsAction.Tap))
        {
            return null;
        }
        return base.GetProviderCore(providerType);
    }
}
