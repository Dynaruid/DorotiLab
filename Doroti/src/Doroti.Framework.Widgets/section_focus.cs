using Doroti.Framework.Services;

namespace Doroti.Framework.Widgets;

/// <summary>Connects section-boundary Tab traversal to indexed materialization.
/// Section ids/order and scroll ownership are supplied by the list owner.</summary>
public sealed class SectionFocusCoordinator : IDisposable
{
    private readonly int[] _order;
    private readonly Dictionary<int, int> _positions;
    private readonly Dictionary<int, FocusNode> _nodes = [];
    private readonly System.Action<int> _materialize;
    private readonly ReadingOrderTraversalPolicy _policy = new();
    private long _request;
    private bool _disposed;
    private bool _pending;
    private readonly Queue<bool> _queuedTabs = new();
    public SectionFocusCoordinator(IEnumerable<int> order, System.Action<int> materialize)
    {
        _order = order.ToArray(); _materialize = materialize;
        if (_order.Length is < 1 or > 4096) throw new ArgumentOutOfRangeException(nameof(order));
        _positions = _order.Select((id, position) => (id, position)).ToDictionary(p => p.id, p => p.position);
    }
    public int? FocusedSection => _nodes.Where(pair => pair.Value.hasFocus).Select(pair => (int?)pair.Key).FirstOrDefault();
    public void CancelPendingTraversal() { _request++; _pending = false; _queuedTabs.Clear(); }
    public Widget Wrap(int id, Widget child)
    {
        if (!_positions.ContainsKey(id)) throw new ArgumentOutOfRangeException(nameof(id));
        if (!_nodes.TryGetValue(id, out var node)) _nodes[id] = node = new FocusNode(skipTraversal: true, debugLabel: $"section-{id}");
        return new Focus(focusNode: node, skipTraversal: true, includeSemantics: false, child: child,
            onKeyEvent: (scope, key) => Handle(id, scope, key));
    }
    private KeyEventResult Handle(int id, FocusNode scope, KeyEvent key)
    {
        var keyboard = HardwareKeyboard.instance;
        if (key is not (KeyDownEvent or KeyRepeatEvent) || !Equals(key.logicalKey, LogicalKeyboardKey.tab) ||
            keyboard.isControlPressed || keyboard.isAltPressed || keyboard.isMetaPressed) return KeyEventResult.ignored;
        if (_pending) { _queuedTabs.Enqueue(keyboard.isShiftPressed); return KeyEventResult.handled; }
        var primary = FocusManager.instance.primaryFocus;
        if (primary is null) return KeyEventResult.ignored;
        var members = _policy.sortDescendants(scope.traversalDescendants, primary).ToArray();
        var reverse = keyboard.isShiftPressed;
        if (members.Length == 0 || !ReferenceEquals(primary, reverse ? members[0] : members[^1])) return KeyEventResult.ignored;
        var next = _positions[id] + (reverse ? -1 : 1);
        if ((uint)next >= _order.Length) return KeyEventResult.ignored;
        Request(next, reverse, primary, ++_request);
        return KeyEventResult.handled;
    }
    private void Request(int position, bool reverse, FocusNode primary, long request)
    {
        if (_disposed || request != _request) return;
        _pending = true;
        _materialize(_order[position]);
        WidgetsBinding.instance.addPostFrameCallback(_ =>
        {
            if (_disposed || request != _request) return;
            if (!ReferenceEquals(FocusManager.instance.primaryFocus, primary)) { CancelPendingTraversal(); return; }
            var scope = _nodes.GetValueOrDefault(_order[position]);
            var members = scope?.context is null ? [] : _policy.sortDescendants(scope.traversalDescendants, scope).ToArray();
            if (members.Length != 0)
            {
                FocusTraversalPolicy.defaultTraversalRequestFocusCallback(reverse ? members[^1] : members[0],
                    alignmentPolicy: reverse ? ScrollPositionAlignmentPolicy.keepVisibleAtStart : ScrollPositionAlignmentPolicy.keepVisibleAtEnd);
                _pending = false; DrainAfterFrame(request);
                return;
            }
            var next = position + (reverse ? -1 : 1);
            if ((uint)next < _order.Length) Request(next, reverse, primary, request);
            else
            {
                if (reverse) primary.previousFocus(); else primary.nextFocus();
                _pending = false; DrainAfterFrame(request);
            }
        });
        Doroti.Framework.Scheduler.SchedulerBinding.instance.ensureVisualUpdate();
    }
    private void DrainAfterFrame(long request)
    {
        if (_queuedTabs.Count == 0) return;
        WidgetsBinding.instance.addPostFrameCallback(_ =>
        {
            if (_disposed || request != _request || _pending || _queuedTabs.Count == 0) return;
            var reverse = _queuedTabs.Dequeue();
            var primary = FocusManager.instance.primaryFocus;
            if (primary is null) { CancelPendingTraversal(); return; }
            var id = FocusedSection;
            if (id is { } section)
            {
                var members = _policy.sortDescendants(_nodes[section].traversalDescendants, primary).ToArray();
                var next = _positions[section] + (reverse ? -1 : 1);
                if (members.Length != 0 && ReferenceEquals(primary, reverse ? members[0] : members[^1]) && (uint)next < _order.Length)
                { Request(next, reverse, primary, request); return; }
            }
            if (reverse) primary.previousFocus(); else primary.nextFocus();
            DrainAfterFrame(request);
        });
        Doroti.Framework.Scheduler.SchedulerBinding.instance.ensureVisualUpdate();
    }
    public void Dispose()
    {
        _disposed = true; _request++;
        _queuedTabs.Clear();
        foreach (var node in _nodes.Values) node.dispose();
        _nodes.Clear();
    }
}
