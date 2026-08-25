using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// Immutable retained semantics used by the child-HWND UI Automation provider.
/// The snapshot is deliberately detached from framework mutation so COM callers
/// can enumerate it without entering the engine or the platform window loop.
/// </summary>
internal sealed record FlutterWindowsUiaSemanticsSnapshot(
    long Generation,
    IReadOnlyDictionary<int, SemanticsNodeUpdate> Nodes,
    IReadOnlyDictionary<int, int> ParentByNodeId,
    IReadOnlyList<int> RootNodeIds)
{
    internal static FlutterWindowsUiaSemanticsSnapshot Empty { get; } = new(
        0,
        new ReadOnlyDictionary<int, SemanticsNodeUpdate>(new Dictionary<int, SemanticsNodeUpdate>()),
        new ReadOnlyDictionary<int, int>(new Dictionary<int, int>()),
        []);

    internal bool TryGetNode(int nodeId, [NotNullWhen(true)] out SemanticsNodeUpdate? node) =>
        Nodes.TryGetValue(nodeId, out node);
}

/// <summary>
/// Observable F7 structural evidence.  The physical Narrator and Accessibility
/// Insights rows intentionally remain false until a product binary is exercised
/// on a Windows desktop; these counters are not a substitute for that gate.
/// </summary>
internal sealed record FlutterWindowsUiaBridgeSnapshot(
    nint ChildHwnd,
    long SemanticsGeneration,
    int SemanticsNodeCount,
    int RootSemanticsNodeCount,
    long SemanticsUpdateCount,
    long StaleSemanticsUpdateCount,
    long WmGetObjectRequestCount,
    long UiaRootProviderReturnCount,
    long RootProviderCreationCount,
    long DuplicateRootProviderCount,
    long FragmentProviderCount,
    long BrokenFragmentCount,
    long QueuedActionCount,
    long RejectedActionCount,
    long UiaEventCount,
    long UiaEventFailureCount,
    bool AttachedToChildHost,
    bool IsDisposed,
    bool NarratorPhysicalAcceptanceVerified,
    bool AccessibilityInsightsPhysicalAcceptanceVerified);

/// <summary>
/// Child-HWND-only UI Automation bridge for the Flutter-style Windows path.
/// It answers UIA root-object queries from the typed child WndProc hook, keeps
/// an immutable semantics tree for fragment navigation, and queues all actions
/// back to the engine task runner rather than calling the engine from a COM
/// accessibility client thread.
/// </summary>
internal sealed class FlutterWindowsUiaBridge : IDisposable
{
    private const uint WmGetObject = 0x003D;
    private static readonly nint UiaRootObjectId = new(-25);
    private static readonly object AttachedChildGate = new();
    private static readonly Dictionary<nint, FlutterWindowsUiaBridge> AttachedChildBridges = [];

    private readonly nint _childHwnd;
    private readonly IFlutterWindowsEngineTaskRunner _engineTaskRunner;
    private readonly Action<int, SemanticsAction, object?> _dispatchSemanticsAction;
    private readonly Action? _requestChildFocus;
    private readonly object _providerGate = new();
    private readonly Dictionary<int, FlutterWindowsUiaFragmentProvider> _fragments = [];
    private readonly FlutterWindowsUiaRootProvider _rootProvider;
    private FlutterWindowsUiaSemanticsSnapshot _semantics = FlutterWindowsUiaSemanticsSnapshot.Empty;
    private FlutterWindowsUiaGeometrySnapshot _geometry;
    private FlutterWindowsHostWindow? _hostWindow;
    private FlutterWindowsViewMetricsCoordinator? _metricsCoordinator;
    private Func<FlutterWindowsChildMessage, FlutterWindowsChildMessageResult>? _childMessageHandler;
    private int _ownsAttachedChild;
    private int _disposed;
    private long _semanticsUpdateCount;
    private long _staleSemanticsUpdateCount;
    private long _wmGetObjectRequestCount;
    private long _uiaRootProviderReturnCount;
    private long _rootProviderCreationCount;
    private long _duplicateRootProviderCount = 0;
    private long _fragmentProviderCount;
    private long _brokenFragmentCount;
    private long _queuedActionCount;
    private long _rejectedActionCount;
    private long _uiaEventCount;
    private long _uiaEventFailureCount;

    private FlutterWindowsUiaBridge(
        nint childHwnd,
        WindowsViewMetrics initialMetrics,
        IFlutterWindowsEngineTaskRunner engineTaskRunner,
        Action<int, SemanticsAction, object?> dispatchSemanticsAction,
        Action? requestChildFocus)
    {
        if (childHwnd == 0) throw new ArgumentOutOfRangeException(nameof(childHwnd));
        ArgumentNullException.ThrowIfNull(initialMetrics);
        ArgumentNullException.ThrowIfNull(engineTaskRunner);
        ArgumentNullException.ThrowIfNull(dispatchSemanticsAction);
        _childHwnd = childHwnd;
        _geometry = FlutterWindowsUiaGeometrySnapshot.From(initialMetrics);
        _engineTaskRunner = engineTaskRunner;
        _dispatchSemanticsAction = dispatchSemanticsAction;
        _requestChildFocus = requestChildFocus;
        _rootProvider = new FlutterWindowsUiaRootProvider(this);
        Interlocked.Increment(ref _rootProviderCreationCount);
    }

    /// <summary>
    /// Attaches only to the raw child-HWND hook. The standard top-level window
    /// never participates, preserving native caption, Snap, and non-client UIA.
    /// <paramref name="requestChildFocus"/> is an optional engine-thread request
    /// owned by the input host; this bridge never calls native focus APIs from a
    /// UIA COM caller thread.
    /// </summary>
    internal static FlutterWindowsUiaBridge AttachToHostWindow(
        FlutterWindowsHostWindow hostWindow,
        FlutterWindowsViewMetricsCoordinator metricsCoordinator,
        IFlutterWindowsEngineTaskRunner engineTaskRunner,
        Action<int, SemanticsAction, object?> dispatchSemanticsAction,
        Action? requestChildFocus = null)
    {
        ArgumentNullException.ThrowIfNull(hostWindow);
        ArgumentNullException.ThrowIfNull(metricsCoordinator);
        ArgumentNullException.ThrowIfNull(engineTaskRunner);
        ArgumentNullException.ThrowIfNull(dispatchSemanticsAction);
        if (hostWindow.ViewHwnd == 0)
            throw new InvalidOperationException("The UIA bridge requires a live Flutter child HWND.");

        var bridge = new FlutterWindowsUiaBridge(
            hostWindow.ViewHwnd,
            metricsCoordinator.Current,
            engineTaskRunner,
            dispatchSemanticsAction,
            requestChildFocus)
        {
            _hostWindow = hostWindow,
            _metricsCoordinator = metricsCoordinator,
        };
        bridge._childMessageHandler = bridge.HandleChildMessage;
        ClaimAttachedChildOwnership(bridge);
        try
        {
            hostWindow.ChildMessageReceived += bridge._childMessageHandler;
            metricsCoordinator.MetricsPublished += bridge.HandleMetricsPublished;
            return bridge;
        }
        catch
        {
            bridge.Dispose();
            throw;
        }
    }

    /// <summary>
    /// A narrow construction seam for deterministic F7 fixtures. Production
    /// construction must use <see cref="AttachToHostWindow"/> so WM_GETOBJECT
    /// remains connected only to the real child HWND. A fixture may omit the
    /// child-focus request when it does not exercise the root SetFocus contract.
    /// </summary>
    internal static FlutterWindowsUiaBridge CreateForValidation(
        nint childHwnd,
        WindowsViewMetrics initialMetrics,
        IFlutterWindowsEngineTaskRunner engineTaskRunner,
        Action<int, SemanticsAction, object?> dispatchSemanticsAction,
        Action? requestChildFocus = null) => new(
        childHwnd,
        initialMetrics,
        engineTaskRunner,
        dispatchSemanticsAction,
        requestChildFocus);

    internal FlutterWindowsUiaBridgeSnapshot Snapshot
    {
        get
        {
            var semantics = Volatile.Read(ref _semantics);
            return new(
                _childHwnd,
                semantics.Generation,
                semantics.Nodes.Count,
                semantics.RootNodeIds.Count,
                Interlocked.Read(ref _semanticsUpdateCount),
                Interlocked.Read(ref _staleSemanticsUpdateCount),
                Interlocked.Read(ref _wmGetObjectRequestCount),
                Interlocked.Read(ref _uiaRootProviderReturnCount),
                Interlocked.Read(ref _rootProviderCreationCount),
                Interlocked.Read(ref _duplicateRootProviderCount),
                Interlocked.Read(ref _fragmentProviderCount),
                Interlocked.Read(ref _brokenFragmentCount),
                Interlocked.Read(ref _queuedActionCount),
                Interlocked.Read(ref _rejectedActionCount),
                Interlocked.Read(ref _uiaEventCount),
                Interlocked.Read(ref _uiaEventFailureCount),
                _hostWindow is not null,
                IsDisposed,
                NarratorPhysicalAcceptanceVerified: false,
                AccessibilityInsightsPhysicalAcceptanceVerified: false);
        }
    }

    internal FlutterWindowsUiaSemanticsSnapshot SemanticsSnapshot => Volatile.Read(ref _semantics);

    /// <summary>
    /// Replaces the retained UIA tree with a fully immutable semantics snapshot.
    /// The framework has already pruned unreachable nodes before this boundary.
    /// </summary>
    internal bool UpdateSemantics(SemanticsUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);
        if (IsDisposed)
        {
            Interlocked.Increment(ref _rejectedActionCount);
            return false;
        }

        var previous = Volatile.Read(ref _semantics);
        if (update.generation < previous.Generation)
        {
            Interlocked.Increment(ref _staleSemanticsUpdateCount);
            return false;
        }

        var next = CreateSemanticsSnapshot(update, out var brokenFragments);
        var delta = SemanticsUpdateDiffer.Diff(previous.Nodes, next.Nodes.Values.ToArray());
        Volatile.Write(ref _semantics, next);
        DisconnectRemovedFragments(next);
        Interlocked.Increment(ref _semanticsUpdateCount);
        if (brokenFragments != 0) Interlocked.Add(ref _brokenFragmentCount, brokenFragments);
        RaiseLayoutInvalidatedIfNeeded(delta);
        return true;
    }

    internal void ClearSemantics()
    {
        if (IsDisposed) return;
        var previous = Volatile.Read(ref _semantics);
        if (previous.Nodes.Count == 0) return;
        var empty = FlutterWindowsUiaSemanticsSnapshot.Empty with { Generation = previous.Generation };
        var delta = SemanticsUpdateDiffer.Diff(previous.Nodes, []);
        Volatile.Write(ref _semantics, empty);
        DisconnectRemovedFragments(empty);
        RaiseLayoutInvalidatedIfNeeded(delta);
    }

    /// <summary>
    /// Directly handles the only child message owned by this bridge. It is also
    /// exposed for an F7 fixture to prove the WM_GETOBJECT response without a
    /// second WndProc or a top-level message hook.
    /// </summary>
    internal bool TryHandleChildWindowMessage(uint message, nuint wParam, nint lParam, out nint result)
    {
        result = 0;
        if (IsDisposed || message != WmGetObject || lParam != UiaRootObjectId) return false;

        Interlocked.Increment(ref _wmGetObjectRequestCount);
        try
        {
            result = NativeMethods.UiaReturnRawElementProvider(
                _childHwnd,
                unchecked((nint)wParam),
                lParam,
                _rootProvider);
            Interlocked.Increment(ref _uiaRootProviderReturnCount);
            return true;
        }
        catch (DllNotFoundException)
        {
            return false;
        }
        catch (EntryPointNotFoundException)
        {
            return false;
        }
    }

    /// <summary>
    /// Deterministic action seam used by F7 validation. The same permission
    /// check is used by the COM providers before they enqueue engine work.
    /// </summary>
    internal bool TryDispatchActionForValidation(
        int nodeId,
        SemanticsAction action,
        object? arguments = null)
    {
        if (!TryGetNode(nodeId, out var node) || action == SemanticsAction.none ||
            !IsEnabled(node) || !node.actions.HasFlag(action))
            return false;
        return TryQueueSemanticsAction(nodeId, action, arguments);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        var hostWindow = Interlocked.Exchange(ref _hostWindow, null);
        var handler = Interlocked.Exchange(ref _childMessageHandler, null);
        if (hostWindow is not null && handler is not null)
            hostWindow.ChildMessageReceived -= handler;

        var metricsCoordinator = Interlocked.Exchange(ref _metricsCoordinator, null);
        if (metricsCoordinator is not null)
            metricsCoordinator.MetricsPublished -= HandleMetricsPublished;

        ReleaseAttachedChildOwnership();
        var semantics = Volatile.Read(ref _semantics);
        Volatile.Write(ref _semantics, FlutterWindowsUiaSemanticsSnapshot.Empty with { Generation = semantics.Generation });
        DisconnectProviders(DetachAllFragments());
        DisconnectProvider(_rootProvider);
    }

    private bool IsDisposed => Volatile.Read(ref _disposed) != 0;

    private FlutterWindowsChildMessageResult HandleChildMessage(FlutterWindowsChildMessage message)
    {
        if (message.Hwnd != _childHwnd ||
            !TryHandleChildWindowMessage(message.Message, message.WParam, message.LParam, out var result))
        {
            return FlutterWindowsChildMessageResult.Unhandled;
        }
        return FlutterWindowsChildMessageResult.HandledResult(result);
    }

    private void HandleMetricsPublished(WindowsViewMetrics metrics)
    {
        if (!IsDisposed) Volatile.Write(ref _geometry, FlutterWindowsUiaGeometrySnapshot.From(metrics));
    }

    private static void ClaimAttachedChildOwnership(FlutterWindowsUiaBridge bridge)
    {
        lock (AttachedChildGate)
        {
            if (AttachedChildBridges.TryGetValue(bridge._childHwnd, out var existing))
            {
                if (!existing.IsDisposed)
                {
                    Interlocked.Increment(ref existing._duplicateRootProviderCount);
                    throw new InvalidOperationException("Only one UIA root provider may attach to a Flutter child HWND.");
                }
                AttachedChildBridges.Remove(bridge._childHwnd);
            }

            AttachedChildBridges.Add(bridge._childHwnd, bridge);
            Volatile.Write(ref bridge._ownsAttachedChild, 1);
        }
    }

    private void ReleaseAttachedChildOwnership()
    {
        if (Interlocked.Exchange(ref _ownsAttachedChild, 0) == 0) return;
        lock (AttachedChildGate)
        {
            if (AttachedChildBridges.TryGetValue(_childHwnd, out var existing) && ReferenceEquals(existing, this))
                AttachedChildBridges.Remove(_childHwnd);
        }
    }

    private void DisconnectRemovedFragments(FlutterWindowsUiaSemanticsSnapshot current)
    {
        List<FlutterWindowsUiaFragmentProvider> removed = [];
        lock (_providerGate)
        {
            foreach (var nodeId in _fragments.Keys.Where(nodeId => !current.Nodes.ContainsKey(nodeId)).ToArray())
            {
                removed.Add(_fragments[nodeId]);
                _fragments.Remove(nodeId);
            }
        }
        DisconnectProviders(removed);
    }

    private List<FlutterWindowsUiaFragmentProvider> DetachAllFragments()
    {
        lock (_providerGate)
        {
            var fragments = _fragments.Values.ToList();
            _fragments.Clear();
            return fragments;
        }
    }

    private static void DisconnectProviders(IEnumerable<IFlutterWindowsRawElementProviderSimple> providers)
    {
        foreach (var provider in providers) DisconnectProvider(provider);
    }

    private static void DisconnectProvider(IFlutterWindowsRawElementProviderSimple provider)
    {
        try
        {
            _ = NativeMethods.UiaDisconnectProvider(provider);
        }
        catch (DllNotFoundException)
        {
            // The deterministic source fixture is permitted to run without a
            // physical UIAutomationCore deployment assertion.
        }
        catch (EntryPointNotFoundException)
        {
            // Keep shutdown idempotent even on an incomplete Windows image.
        }
    }

    private static FlutterWindowsUiaSemanticsSnapshot CreateSemanticsSnapshot(
        SemanticsUpdate update,
        out int brokenFragments)
    {
        var nodes = new Dictionary<int, SemanticsNodeUpdate>();
        brokenFragments = 0;
        foreach (var node in update.nodes)
        {
            if (!nodes.TryAdd(node.id, node with { children = node.children.ToArray() }))
            {
                // Retain the most recent copy to preserve the framework's last
                // writer behavior, but never expose duplicate native fragments.
                nodes[node.id] = node with { children = node.children.ToArray() };
                brokenFragments++;
            }
        }

        var parents = new Dictionary<int, int>();
        foreach (var parent in nodes.Values.OrderBy(node => node.id))
        {
            foreach (var childId in parent.children)
            {
                if (!nodes.ContainsKey(childId) || childId == parent.id || parents.ContainsKey(childId))
                {
                    brokenFragments++;
                    continue;
                }
                parents.Add(childId, parent.id);
            }
        }

        // Explicit traversal parents are an ordering hint from the framework.
        // They fill only a gap; a real child edge remains the parent authority.
        foreach (var node in nodes.Values.OrderBy(node => node.id))
        {
            if (node.traversalParent is not { } parentId || parentId == node.id ||
                !nodes.ContainsKey(parentId) || parents.ContainsKey(node.id))
            {
                continue;
            }
            parents.Add(node.id, parentId);
        }

        BreakParentCycles(parents, ref brokenFragments);
        var roots = nodes.ContainsKey(0)
            ? new[] { 0 }
            : nodes.Keys.Where(nodeId => !parents.ContainsKey(nodeId)).OrderBy(nodeId => nodeId).ToArray();
        return new(
            update.generation,
            new ReadOnlyDictionary<int, SemanticsNodeUpdate>(nodes),
            new ReadOnlyDictionary<int, int>(parents),
            roots);
    }

    private static void BreakParentCycles(Dictionary<int, int> parents, ref int brokenFragments)
    {
        foreach (var start in parents.Keys.ToArray())
        {
            var visited = new HashSet<int>();
            var current = start;
            while (parents.TryGetValue(current, out var parent))
            {
                if (!visited.Add(current))
                {
                    parents.Remove(current);
                    brokenFragments++;
                    break;
                }
                current = parent;
            }
        }
    }

    private void RaiseLayoutInvalidatedIfNeeded(SemanticsUpdateDelta delta)
    {
        if (!delta.HasChanges || IsDisposed) return;
        try
        {
            if (!NativeMethods.UiaClientsAreListening()) return;
            var hr = NativeMethods.UiaRaiseAutomationEvent(
                _rootProvider,
                (int)FlutterWindowsUiaEventId.LayoutInvalidated);
            if (hr >= 0) Interlocked.Increment(ref _uiaEventCount);
            else Interlocked.Increment(ref _uiaEventFailureCount);
        }
        catch (DllNotFoundException)
        {
            Interlocked.Increment(ref _uiaEventFailureCount);
        }
        catch (EntryPointNotFoundException)
        {
            Interlocked.Increment(ref _uiaEventFailureCount);
        }
    }

    internal bool TryGetNode(int nodeId, [NotNullWhen(true)] out SemanticsNodeUpdate? node)
    {
        if (IsDisposed)
        {
            node = null;
            return false;
        }
        return Volatile.Read(ref _semantics).TryGetNode(nodeId, out node);
    }

    internal FlutterWindowsUiaFragmentProvider? GetFragment(int nodeId)
    {
        if (!TryGetNode(nodeId, out _)) return null;
        lock (_providerGate)
        {
            if (IsDisposed || !TryGetNode(nodeId, out _)) return null;
            if (_fragments.TryGetValue(nodeId, out var existing)) return existing;
            var created = new FlutterWindowsUiaFragmentProvider(this, nodeId);
            _fragments.Add(nodeId, created);
            Interlocked.Increment(ref _fragmentProviderCount);
            return created;
        }
    }

    internal FlutterWindowsUiaRootProvider RootProvider => _rootProvider;

    internal IFlutterWindowsRawElementProviderFragment? NavigateFragment(
        int nodeId,
        FlutterWindowsUiaNavigateDirection direction)
    {
        var snapshot = Volatile.Read(ref _semantics);
        if (!snapshot.TryGetNode(nodeId, out var node)) return null;
        return direction switch
        {
            FlutterWindowsUiaNavigateDirection.Parent => snapshot.ParentByNodeId.TryGetValue(nodeId, out var parentId)
                ? GetFragment(parentId)
                : _rootProvider,
            FlutterWindowsUiaNavigateDirection.FirstChild => GetExistingChild(snapshot, node.children, forward: true),
            FlutterWindowsUiaNavigateDirection.LastChild => GetExistingChild(snapshot, node.children, forward: false),
            FlutterWindowsUiaNavigateDirection.NextSibling => GetSibling(snapshot, nodeId, +1),
            FlutterWindowsUiaNavigateDirection.PreviousSibling => GetSibling(snapshot, nodeId, -1),
            _ => null,
        };
    }

    internal IFlutterWindowsRawElementProviderFragment? GetFocusedFragment()
    {
        var snapshot = Volatile.Read(ref _semantics);
        var focused = snapshot.Nodes.Values
            .Where(node => node.flags?.isFocused == Tristate.isTrue)
            .OrderBy(node => node.id)
            .FirstOrDefault();
        return focused is null ? null : GetFragment(focused.id);
    }

    internal IFlutterWindowsRawElementProviderFragment? ElementProviderFromScreenPoint(double x, double y)
    {
        var snapshot = Volatile.Read(ref _semantics);
        foreach (var node in snapshot.Nodes.Values
                     .Where(node => node.flags?.isHidden != true)
                     .OrderBy(node => LogicalArea(node.rect)))
        {
            var bounds = GetScreenBounds(node);
            if (bounds.Contains(x, y)) return GetFragment(node.id);
        }
        return GetRootScreenBounds().Contains(x, y) ? _rootProvider : null;
    }

    internal FlutterWindowsUiaRect GetRootScreenBounds()
    {
        var geometry = Volatile.Read(ref _geometry);
        return geometry.TryGetScreenOrigin(_childHwnd, out var origin)
            ? new(origin.X, origin.Y, Math.Max(0, geometry.PhysicalWidth), Math.Max(0, geometry.PhysicalHeight))
            : default;
    }

    internal FlutterWindowsUiaRect GetScreenBounds(SemanticsNodeUpdate node)
    {
        if (IsDisposed) return default;
        var geometry = Volatile.Read(ref _geometry);
        if (!geometry.HasDrawableSize || !geometry.TryGetScreenOrigin(_childHwnd, out var origin)) return default;
        try
        {
            var left = LogicalCoordinateToPhysical(node.rect.left, geometry.DevicePixelRatio);
            var top = LogicalCoordinateToPhysical(node.rect.top, geometry.DevicePixelRatio);
            var width = LogicalExtentToPhysical(node.rect.right - node.rect.left, geometry.DevicePixelRatio);
            var height = LogicalExtentToPhysical(node.rect.bottom - node.rect.top, geometry.DevicePixelRatio);
            return new(origin.X + left, origin.Y + top, width, height);
        }
        catch (ArgumentOutOfRangeException)
        {
            Interlocked.Increment(ref _brokenFragmentCount);
            return default;
        }
        catch (OverflowException)
        {
            Interlocked.Increment(ref _brokenFragmentCount);
            return default;
        }
    }

    internal object? GetRootPropertyValue(int propertyId) => (FlutterWindowsUiaPropertyId)propertyId switch
    {
        FlutterWindowsUiaPropertyId.RuntimeId => _rootProvider.GetRuntimeId(),
        FlutterWindowsUiaPropertyId.ProcessId => Environment.ProcessId,
        FlutterWindowsUiaPropertyId.ControlType => (int)FlutterWindowsUiaControlTypeId.Pane,
        FlutterWindowsUiaPropertyId.Name => "Doroti",
        FlutterWindowsUiaPropertyId.ClassName => "Doroti.Flutter.Windows.View",
        FlutterWindowsUiaPropertyId.NativeWindowHandle => unchecked((int)_childHwnd),
        FlutterWindowsUiaPropertyId.FrameworkId => "Doroti",
        FlutterWindowsUiaPropertyId.IsControlElement => true,
        FlutterWindowsUiaPropertyId.IsContentElement => true,
        FlutterWindowsUiaPropertyId.IsEnabled => !IsDisposed,
        FlutterWindowsUiaPropertyId.IsKeyboardFocusable => !IsDisposed,
        FlutterWindowsUiaPropertyId.HasKeyboardFocus => NativeMethods.GetFocus() == _childHwnd,
        FlutterWindowsUiaPropertyId.IsOffscreen => IsDisposed || !Volatile.Read(ref _geometry).HasDrawableSize,
        _ => null,
    };

    internal object? GetFragmentPropertyValue(int nodeId, int propertyId)
    {
        if (!TryGetNode(nodeId, out var node)) return null;
        var flags = node.flags;
        var scroll = GetScrollState(node);
        return (FlutterWindowsUiaPropertyId)propertyId switch
        {
            FlutterWindowsUiaPropertyId.RuntimeId => FlutterWindowsUiaRuntimeId.ForNode(_childHwnd, node.id),
            FlutterWindowsUiaPropertyId.ProcessId => Environment.ProcessId,
            FlutterWindowsUiaPropertyId.ControlType => (int)ControlTypeFor(node),
            FlutterWindowsUiaPropertyId.Name => node.label ?? node.value ?? string.Empty,
            FlutterWindowsUiaPropertyId.AutomationId => $"semantics-{node.id}",
            FlutterWindowsUiaPropertyId.ClassName => $"Doroti.Semantics.{node.role}",
            FlutterWindowsUiaPropertyId.NativeWindowHandle => unchecked((int)_childHwnd),
            FlutterWindowsUiaPropertyId.FrameworkId => "Doroti",
            FlutterWindowsUiaPropertyId.IsControlElement => IsControlElement(node),
            FlutterWindowsUiaPropertyId.IsContentElement => flags?.isHidden != true,
            FlutterWindowsUiaPropertyId.IsEnabled => IsEnabled(node),
            FlutterWindowsUiaPropertyId.IsKeyboardFocusable => node.actions.HasFlag(SemanticsAction.focus) || flags?.isTextField == true,
            FlutterWindowsUiaPropertyId.HasKeyboardFocus => flags?.isFocused == Tristate.isTrue,
            FlutterWindowsUiaPropertyId.IsOffscreen => flags?.isHidden == true || GetScreenBounds(node).IsEmpty,
            FlutterWindowsUiaPropertyId.IsInvokePatternAvailable => node.actions.HasFlag(SemanticsAction.tap),
            FlutterWindowsUiaPropertyId.IsValuePatternAvailable => SupportsValue(node),
            FlutterWindowsUiaPropertyId.IsScrollPatternAvailable => SupportsScroll(node),
            FlutterWindowsUiaPropertyId.ValueValue => node.value ?? string.Empty,
            FlutterWindowsUiaPropertyId.ValueIsReadOnly => IsValueReadOnly(node),
            FlutterWindowsUiaPropertyId.ScrollHorizontalScrollPercent => scroll.HorizontalPercent,
            FlutterWindowsUiaPropertyId.ScrollHorizontalViewSize => scroll.HorizontalViewSize,
            FlutterWindowsUiaPropertyId.ScrollVerticalScrollPercent => scroll.VerticalPercent,
            FlutterWindowsUiaPropertyId.ScrollVerticalViewSize => scroll.VerticalViewSize,
            FlutterWindowsUiaPropertyId.ScrollHorizontallyScrollable => scroll.CanScrollHorizontally,
            FlutterWindowsUiaPropertyId.ScrollVerticallyScrollable => scroll.CanScrollVertically,
            _ => null,
        };
    }

    internal bool SupportsInvoke(int nodeId) =>
        TryGetNode(nodeId, out var node) && IsEnabled(node) && node.actions.HasFlag(SemanticsAction.tap);

    internal bool SupportsValue(int nodeId) =>
        TryGetNode(nodeId, out var node) && SupportsValue(node);

    internal bool SupportsScroll(int nodeId) =>
        TryGetNode(nodeId, out var node) && SupportsScroll(node);

    internal void Invoke(int nodeId)
    {
        if (!SupportsInvoke(nodeId)) throw ElementNotEnabledOrUnsupported();
        if (!TryQueueSemanticsAction(nodeId, SemanticsAction.tap, null))
            throw ElementNotEnabledOrUnsupported();
    }

    internal string GetValue(int nodeId) =>
        TryGetNode(nodeId, out var node) ? node.value ?? string.Empty : string.Empty;

    internal bool IsValueReadOnly(int nodeId) =>
        !TryGetNode(nodeId, out var node) || IsValueReadOnly(node);

    internal void SetValue(int nodeId, string? value)
    {
        if (!TryGetNode(nodeId, out var node) || !IsEnabled(node) ||
            !SupportsValue(node) || IsValueReadOnly(node))
            throw ElementNotEnabledOrUnsupported();
        if (!TryQueueSemanticsAction(nodeId, SemanticsAction.setText, value ?? string.Empty))
            throw ElementNotEnabledOrUnsupported();
    }

    internal FlutterWindowsUiaScrollState GetScrollState(int nodeId) =>
        TryGetNode(nodeId, out var node) ? GetScrollState(node) : FlutterWindowsUiaScrollState.None;

    internal void Scroll(int nodeId, FlutterWindowsUiaScrollAmount horizontalAmount, FlutterWindowsUiaScrollAmount verticalAmount)
    {
        if (!TryGetNode(nodeId, out var node)) throw ElementNotEnabledOrUnsupported();
        var scroll = GetScrollState(node);
        if (!SupportsScroll(node) || !scroll.HasAnyAxis ||
            !CanDispatchDirectionalScroll(node, horizontalAmount, horizontal: true) ||
            !CanDispatchDirectionalScroll(node, verticalAmount, horizontal: false))
        {
            throw ElementNotEnabledOrUnsupported();
        }
        if (!QueueDirectionalScroll(node, horizontalAmount, horizontal: true) ||
            !QueueDirectionalScroll(node, verticalAmount, horizontal: false))
        {
            throw ElementNotEnabledOrUnsupported();
        }
    }

    internal void SetScrollPercent(int nodeId, double horizontalPercent, double verticalPercent)
    {
        if (!TryGetNode(nodeId, out var node)) throw ElementNotEnabledOrUnsupported();
        var scroll = GetScrollState(node);
        if (!SupportsScroll(node) || !scroll.HasAnyAxis ||
            !node.actions.HasFlag(SemanticsAction.scrollToOffset))
            throw ElementNotEnabledOrUnsupported();
        ValidateScrollPercent(horizontalPercent);
        ValidateScrollPercent(verticalPercent);
        if ((horizontalPercent != FlutterWindowsUiaScrollState.NoScroll && !scroll.CanScrollHorizontally) ||
            (verticalPercent != FlutterWindowsUiaScrollState.NoScroll && !scroll.CanScrollVertically))
        {
            throw ElementNotEnabledOrUnsupported();
        }
        if (horizontalPercent == FlutterWindowsUiaScrollState.NoScroll &&
            verticalPercent == FlutterWindowsUiaScrollState.NoScroll)
        {
            return;
        }

        // Framework semantics describe a single primary scroll extent.  The
        // unused Flutter Offset axis must stay zero rather than copying that
        // primary position into both coordinates.
        var horizontal = scroll.CanScrollHorizontally
            ? scroll.OffsetForPercent(horizontalPercent, horizontal: true)
            : 0.0;
        var vertical = scroll.CanScrollVertically
            ? scroll.OffsetForPercent(verticalPercent, horizontal: false)
            : 0.0;
        if (!TryQueueSemanticsAction(
                nodeId,
                SemanticsAction.scrollToOffset,
                new Float64List(new[] { horizontal, vertical })))
        {
            throw ElementNotEnabledOrUnsupported();
        }
    }

    internal void SetFocus(int nodeId)
    {
        if (TryGetNode(nodeId, out var node) && node.actions.HasFlag(SemanticsAction.focus))
        {
            if (!TryQueueSemanticsAction(nodeId, SemanticsAction.focus, null))
                throw ElementNotEnabledOrUnsupported();
            return;
        }
        if (TryGetNode(nodeId, out node) && node.actions.HasFlag(SemanticsAction.didGainAccessibilityFocus))
        {
            if (!TryQueueSemanticsAction(nodeId, SemanticsAction.didGainAccessibilityFocus, null))
                throw ElementNotEnabledOrUnsupported();
            return;
        }
        throw ElementNotEnabledOrUnsupported();
    }

    internal void SetChildFocus()
    {
        // The child input host owns the actual SetFocus call.  UIA is allowed
        // only to request it through the engine task boundary, never directly
        // from the COM client thread or through a platform window-loop callback.
        if (_requestChildFocus is null || !TryQueueEngineWork(_requestChildFocus))
            throw ElementNotEnabledOrUnsupported();
        Interlocked.Increment(ref _queuedActionCount);
    }

    private bool TryQueueSemanticsAction(int nodeId, SemanticsAction action, object? arguments)
    {
        if (!TryQueueEngineWork(() => _dispatchSemanticsAction(nodeId, action, arguments)))
            return false;
        Interlocked.Increment(ref _queuedActionCount);
        return true;
    }

    private bool TryQueueEngineWork(Action work)
    {
        ArgumentNullException.ThrowIfNull(work);
        if (IsDisposed)
        {
            Interlocked.Increment(ref _rejectedActionCount);
            return false;
        }
        try
        {
            _engineTaskRunner.PostEngineTask(() =>
            {
                if (IsDisposed)
                {
                    Interlocked.Increment(ref _rejectedActionCount);
                    return;
                }
                work();
            });
        }
        catch (Exception)
        {
            Interlocked.Increment(ref _rejectedActionCount);
            return false;
        }
        return true;
    }

    private bool QueueDirectionalScroll(
        SemanticsNodeUpdate node,
        FlutterWindowsUiaScrollAmount amount,
        bool horizontal)
    {
        if (amount == FlutterWindowsUiaScrollAmount.NoAmount) return true;
        var action = ScrollActionFor(amount, horizontal);
        return node.actions.HasFlag(action) && TryQueueSemanticsAction(node.id, action, null);
    }

    private static FlutterWindowsUiaScrollState GetScrollState(SemanticsNodeUpdate node)
    {
        var horizontal = node.actions.HasFlag(SemanticsAction.scrollLeft) || node.actions.HasFlag(SemanticsAction.scrollRight);
        var vertical = node.actions.HasFlag(SemanticsAction.scrollUp) || node.actions.HasFlag(SemanticsAction.scrollDown);
        return new(
            horizontal,
            vertical,
            node.scrollPosition,
            node.scrollExtentMin,
            node.scrollExtentMax,
            Math.Max(0, node.rect.right - node.rect.left),
            Math.Max(0, node.rect.bottom - node.rect.top));
    }

    private static bool SupportsScroll(SemanticsNodeUpdate node) =>
        IsEnabled(node) && GetScrollState(node).HasAnyAxis;

    private static bool CanDispatchDirectionalScroll(
        SemanticsNodeUpdate node,
        FlutterWindowsUiaScrollAmount amount,
        bool horizontal) =>
        amount == FlutterWindowsUiaScrollAmount.NoAmount || node.actions.HasFlag(ScrollActionFor(amount, horizontal));

    private static SemanticsAction ScrollActionFor(FlutterWindowsUiaScrollAmount amount, bool horizontal) =>
        amount is FlutterWindowsUiaScrollAmount.LargeIncrement or FlutterWindowsUiaScrollAmount.SmallIncrement
            ? horizontal ? SemanticsAction.scrollRight : SemanticsAction.scrollDown
            : horizontal ? SemanticsAction.scrollLeft : SemanticsAction.scrollUp;

    private static bool SupportsValue(SemanticsNodeUpdate node) =>
        node.flags?.isTextField == true || node.actions.HasFlag(SemanticsAction.setText);

    private static bool IsValueReadOnly(SemanticsNodeUpdate node) =>
        node.flags?.isReadOnly == true || !node.actions.HasFlag(SemanticsAction.setText);

    private static bool IsEnabled(SemanticsNodeUpdate node) =>
        node.flags?.isEnabled != Tristate.isFalse;

    private static bool IsControlElement(SemanticsNodeUpdate node) =>
        node.actions != SemanticsAction.none || node.flags?.isTextField == true || node.flags?.isButton == true ||
        node.role is not SemanticsRole.none and not SemanticsRole.contentInfo;

    private static FlutterWindowsUiaControlTypeId ControlTypeFor(SemanticsNodeUpdate node)
    {
        if (node.flags?.isTextField == true) return FlutterWindowsUiaControlTypeId.Edit;
        if (node.flags?.isButton == true || node.actions.HasFlag(SemanticsAction.tap)) return FlutterWindowsUiaControlTypeId.Button;
        if (node.flags?.isSlider == true) return FlutterWindowsUiaControlTypeId.Slider;
        return node.role switch
        {
            SemanticsRole.tab => FlutterWindowsUiaControlTypeId.TabItem,
            SemanticsRole.tabBar => FlutterWindowsUiaControlTypeId.Tab,
            SemanticsRole.table => FlutterWindowsUiaControlTypeId.Table,
            SemanticsRole.cell or SemanticsRole.row => FlutterWindowsUiaControlTypeId.DataItem,
            SemanticsRole.columnHeader => FlutterWindowsUiaControlTypeId.HeaderItem,
            SemanticsRole.dragHandle => FlutterWindowsUiaControlTypeId.Thumb,
            SemanticsRole.spinButton or SemanticsRole.loadingSpinner => FlutterWindowsUiaControlTypeId.Spinner,
            SemanticsRole.comboBox => FlutterWindowsUiaControlTypeId.ComboBox,
            SemanticsRole.menuBar => FlutterWindowsUiaControlTypeId.MenuBar,
            SemanticsRole.menu => FlutterWindowsUiaControlTypeId.Menu,
            SemanticsRole.menuItem or SemanticsRole.menuItemCheckbox or SemanticsRole.menuItemRadio => FlutterWindowsUiaControlTypeId.MenuItem,
            SemanticsRole.list => FlutterWindowsUiaControlTypeId.List,
            SemanticsRole.listItem => FlutterWindowsUiaControlTypeId.ListItem,
            SemanticsRole.tooltip => FlutterWindowsUiaControlTypeId.ToolTip,
            SemanticsRole.progressBar => FlutterWindowsUiaControlTypeId.ProgressBar,
            SemanticsRole.status => FlutterWindowsUiaControlTypeId.StatusBar,
            SemanticsRole.dialog or SemanticsRole.alertDialog => FlutterWindowsUiaControlTypeId.Window,
            SemanticsRole.form or SemanticsRole.main or SemanticsRole.navigation or SemanticsRole.region => FlutterWindowsUiaControlTypeId.Group,
            _ when node.flags?.isImage == true => FlutterWindowsUiaControlTypeId.Image,
            _ => FlutterWindowsUiaControlTypeId.Text,
        };
    }

    private IFlutterWindowsRawElementProviderFragment? GetSibling(
        FlutterWindowsUiaSemanticsSnapshot snapshot,
        int nodeId,
        int direction)
    {
        IReadOnlyList<int> siblings = snapshot.ParentByNodeId.TryGetValue(nodeId, out var parentId) &&
                                      snapshot.TryGetNode(parentId, out var parent)
            ? parent.children
            : snapshot.RootNodeIds;
        var current = -1;
        for (var index = 0; index < siblings.Count; index++)
        {
            if (siblings[index] != nodeId) continue;
            current = index;
            break;
        }
        if (current < 0) return null;
        for (var candidate = current + direction;
             candidate >= 0 && candidate < siblings.Count;
             candidate += direction)
        {
            if (snapshot.Nodes.ContainsKey(siblings[candidate])) return GetFragment(siblings[candidate]);
        }
        return null;
    }

    private IFlutterWindowsRawElementProviderFragment? GetExistingChild(
        FlutterWindowsUiaSemanticsSnapshot snapshot,
        IReadOnlyList<int> children,
        bool forward)
    {
        var index = forward ? 0 : children.Count - 1;
        var step = forward ? 1 : -1;
        while (index >= 0 && index < children.Count)
        {
            var childId = children[index];
            if (snapshot.Nodes.ContainsKey(childId)) return GetFragment(childId);
            index += step;
        }
        return null;
    }

    private static int LogicalCoordinateToPhysical(double coordinate, double dpr)
    {
        if (!double.IsFinite(coordinate) || !double.IsFinite(dpr) || dpr <= 0)
            throw new ArgumentOutOfRangeException(nameof(coordinate));
        return checked((int)Math.Round(coordinate * dpr, MidpointRounding.AwayFromZero));
    }

    private static int LogicalExtentToPhysical(double extent, double dpr)
    {
        if (!double.IsFinite(extent) || !double.IsFinite(dpr) || dpr <= 0)
            throw new ArgumentOutOfRangeException(nameof(extent));
        return WindowsViewMetrics.LogicalToPhysical(Math.Max(0, extent), dpr);
    }

    private static double LogicalArea(Rect rect) =>
        Math.Max(0, rect.right - rect.left) * Math.Max(0, rect.bottom - rect.top);

    private static void ValidateScrollPercent(double percent)
    {
        if (percent == FlutterWindowsUiaScrollState.NoScroll) return;
        if (!double.IsFinite(percent) || percent < 0 || percent > 100)
            throw new ArgumentOutOfRangeException(nameof(percent), "UIA scroll percent must be in [0, 100] or NoScroll.");
    }

    private static COMException ElementNotEnabledOrUnsupported() => new(
        "The retained Doroti semantics node cannot perform this UIA action.",
        unchecked((int)0x80040200)); // UIA_E_ELEMENTNOTENABLED

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    internal sealed class FlutterWindowsUiaRootProvider :
        IFlutterWindowsRawElementProviderSimple,
        IFlutterWindowsRawElementProviderFragment,
        IFlutterWindowsRawElementProviderFragmentRoot
    {
        private readonly FlutterWindowsUiaBridge _bridge;

        internal FlutterWindowsUiaRootProvider(FlutterWindowsUiaBridge bridge) => _bridge = bridge;

        public FlutterWindowsUiaProviderOptions GetProviderOptions() => FlutterWindowsUiaProviderOptions.ServerSideProvider;

        public object? GetPatternProvider(int patternId) => null;

        public object? GetPropertyValue(int propertyId) => _bridge.GetRootPropertyValue(propertyId);

        public IFlutterWindowsRawElementProviderSimple? GetHostRawElementProvider()
        {
            if (_bridge.IsDisposed) return null;
            try
            {
                return NativeMethods.UiaHostProviderFromHwnd(_bridge._childHwnd, out var provider) >= 0
                    ? provider
                    : null;
            }
            catch (DllNotFoundException)
            {
                return null;
            }
            catch (EntryPointNotFoundException)
            {
                return null;
            }
        }

        public IFlutterWindowsRawElementProviderFragment? Navigate(FlutterWindowsUiaNavigateDirection direction)
        {
            var roots = _bridge.SemanticsSnapshot.RootNodeIds;
            return direction switch
            {
                FlutterWindowsUiaNavigateDirection.FirstChild => roots.Count == 0 ? null : _bridge.GetFragment(roots[0]),
                FlutterWindowsUiaNavigateDirection.LastChild => roots.Count == 0 ? null : _bridge.GetFragment(roots[^1]),
                _ => null,
            };
        }

        public int[] GetRuntimeId() => FlutterWindowsUiaRuntimeId.ForRoot(_bridge._childHwnd);

        public FlutterWindowsUiaRect GetBoundingRectangle() => _bridge.GetRootScreenBounds();

        public IFlutterWindowsRawElementProviderFragmentRoot[]? GetEmbeddedFragmentRoots() => null;

        public void SetFocus() => _bridge.SetChildFocus();

        public IFlutterWindowsRawElementProviderFragmentRoot? GetFragmentRoot() => this;

        public IFlutterWindowsRawElementProviderFragment? ElementProviderFromPoint(double x, double y) =>
            _bridge.ElementProviderFromScreenPoint(x, y);

        public IFlutterWindowsRawElementProviderFragment? GetFocus() => _bridge.GetFocusedFragment();
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    internal sealed class FlutterWindowsUiaFragmentProvider :
        IFlutterWindowsRawElementProviderSimple,
        IFlutterWindowsRawElementProviderFragment,
        IFlutterWindowsInvokeProvider,
        IFlutterWindowsValueProvider,
        IFlutterWindowsScrollProvider
    {
        private readonly FlutterWindowsUiaBridge _bridge;
        private readonly int _nodeId;

        internal FlutterWindowsUiaFragmentProvider(FlutterWindowsUiaBridge bridge, int nodeId)
        {
            _bridge = bridge;
            _nodeId = nodeId;
        }

        public FlutterWindowsUiaProviderOptions GetProviderOptions() => FlutterWindowsUiaProviderOptions.ServerSideProvider;

        public object? GetPatternProvider(int patternId) => (FlutterWindowsUiaPatternId)patternId switch
        {
            FlutterWindowsUiaPatternId.Invoke when _bridge.SupportsInvoke(_nodeId) => this,
            FlutterWindowsUiaPatternId.Value when _bridge.SupportsValue(_nodeId) => this,
            FlutterWindowsUiaPatternId.Scroll when _bridge.SupportsScroll(_nodeId) => this,
            _ => null,
        };

        public object? GetPropertyValue(int propertyId) => _bridge.GetFragmentPropertyValue(_nodeId, propertyId);

        public IFlutterWindowsRawElementProviderSimple? GetHostRawElementProvider() => null;

        public IFlutterWindowsRawElementProviderFragment? Navigate(FlutterWindowsUiaNavigateDirection direction) =>
            _bridge.NavigateFragment(_nodeId, direction);

        public int[] GetRuntimeId() => FlutterWindowsUiaRuntimeId.ForNode(_bridge._childHwnd, _nodeId);

        public FlutterWindowsUiaRect GetBoundingRectangle() =>
            _bridge.TryGetNode(_nodeId, out var node) ? _bridge.GetScreenBounds(node) : default;

        public IFlutterWindowsRawElementProviderFragmentRoot[]? GetEmbeddedFragmentRoots() => null;

        public void SetFocus() => _bridge.SetFocus(_nodeId);

        public IFlutterWindowsRawElementProviderFragmentRoot? GetFragmentRoot() => _bridge.RootProvider;

        public void Invoke() => _bridge.Invoke(_nodeId);

        public void SetValue([MarshalAs(UnmanagedType.LPWStr)] string? value) => _bridge.SetValue(_nodeId, value);

        [return: MarshalAs(UnmanagedType.BStr)]
        public string? GetValue() => _bridge.GetValue(_nodeId);

        [return: MarshalAs(UnmanagedType.Bool)]
        public bool GetIsReadOnly() => _bridge.IsValueReadOnly(_nodeId);

        public void Scroll(FlutterWindowsUiaScrollAmount horizontalAmount, FlutterWindowsUiaScrollAmount verticalAmount) =>
            _bridge.Scroll(_nodeId, horizontalAmount, verticalAmount);

        public void SetScrollPercent(double horizontalPercent, double verticalPercent) =>
            _bridge.SetScrollPercent(_nodeId, horizontalPercent, verticalPercent);

        public double GetHorizontalScrollPercent() => _bridge.GetScrollState(_nodeId).HorizontalPercent;

        public double GetVerticalScrollPercent() => _bridge.GetScrollState(_nodeId).VerticalPercent;

        public double GetHorizontalViewSize() => _bridge.GetScrollState(_nodeId).HorizontalViewSize;

        public double GetVerticalViewSize() => _bridge.GetScrollState(_nodeId).VerticalViewSize;

        [return: MarshalAs(UnmanagedType.Bool)]
        public bool GetHorizontallyScrollable() => _bridge.GetScrollState(_nodeId).CanScrollHorizontally;

        [return: MarshalAs(UnmanagedType.Bool)]
        public bool GetVerticallyScrollable() => _bridge.GetScrollState(_nodeId).CanScrollVertically;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FlutterWindowsUiaRect(double left, double top, double width, double height)
    {
        internal readonly double Left = left;
        internal readonly double Top = top;
        internal readonly double Width = width;
        internal readonly double Height = height;
        internal bool IsEmpty => Width <= 0 || Height <= 0;
        internal bool Contains(double x, double y) =>
            !IsEmpty && x >= Left && x < Left + Width && y >= Top && y < Top + Height;
    }

    private sealed record FlutterWindowsUiaGeometrySnapshot(
        int PhysicalWidth,
        int PhysicalHeight,
        double DevicePixelRatio,
        long ResizeGeneration,
        WindowsViewMetricsState State)
    {
        internal bool HasDrawableSize =>
            State == WindowsViewMetricsState.Active && PhysicalWidth > 0 && PhysicalHeight > 0;

        internal static FlutterWindowsUiaGeometrySnapshot From(WindowsViewMetrics metrics) => new(
            metrics.PhysicalWidth,
            metrics.PhysicalHeight,
            metrics.DevicePixelRatio,
            metrics.ResizeGeneration,
            metrics.State);

        internal bool TryGetScreenOrigin(nint childHwnd, out NativeMethods.NativePoint origin)
        {
            origin = default;
            return NativeMethods.ClientToScreen(childHwnd, ref origin);
        }
    }

    internal sealed record FlutterWindowsUiaScrollState(
        bool CanScrollHorizontally,
        bool CanScrollVertically,
        double? Position,
        double? ExtentMin,
        double? ExtentMax,
        double HorizontalViewport,
        double VerticalViewport)
    {
        internal const double NoScroll = -1.0;
        internal static FlutterWindowsUiaScrollState None { get; } = new(false, false, null, null, null, 0, 0);
        internal bool HasAnyAxis => CanScrollHorizontally || CanScrollVertically;
        internal double HorizontalPercent => PercentFor(CanScrollHorizontally);
        internal double VerticalPercent => PercentFor(CanScrollVertically);
        internal double HorizontalViewSize => ViewSizeFor(CanScrollHorizontally, HorizontalViewport);
        internal double VerticalViewSize => ViewSizeFor(CanScrollVertically, VerticalViewport);

        internal double OffsetForPercent(double percent, bool horizontal)
        {
            var canScroll = horizontal ? CanScrollHorizontally : CanScrollVertically;
            if (!canScroll || percent == NoScroll) return Position ?? 0;
            if (ExtentMin is not { } min || ExtentMax is not { } max || max < min)
                throw new InvalidOperationException("UIA cannot set a Flutter scroll percent without finite semantics extents.");
            return min + ((max - min) * (percent / 100.0));
        }

        private double PercentFor(bool canScroll)
        {
            if (!canScroll) return NoScroll;
            if (Position is not { } position || ExtentMin is not { } min || ExtentMax is not { } max || max <= min)
                return 0;
            return Math.Clamp(100.0 * ((position - min) / (max - min)), 0, 100);
        }

        private double ViewSizeFor(bool canScroll, double viewport)
        {
            if (!canScroll) return 100;
            if (ExtentMin is not { } min || ExtentMax is not { } max || max <= min || viewport <= 0)
                return 100;
            return Math.Clamp(100.0 * viewport / ((max - min) + viewport), 0, 100);
        }
    }

    private static class FlutterWindowsUiaRuntimeId
    {
        // AutomationInteropProvider.AppendRuntimeId. A child HWND has one
        // root provider; semantic node IDs extend that root without creating
        // an auxiliary native UIA tree.
        private const int AppendRuntimeId = 3;

        internal static int[] ForRoot(nint hwnd) => [AppendRuntimeId, unchecked((int)hwnd)];

        internal static int[] ForNode(nint hwnd, int nodeId) => [AppendRuntimeId, unchecked((int)hwnd), nodeId];
    }

    private enum FlutterWindowsUiaEventId
    {
        LayoutInvalidated = 20008,
    }

    private enum FlutterWindowsUiaPatternId
    {
        Invoke = 10000,
        Value = 10002,
        Scroll = 10004,
    }

    private enum FlutterWindowsUiaPropertyId
    {
        RuntimeId = 30000,
        BoundingRectangle = 30001,
        ProcessId = 30002,
        ControlType = 30003,
        Name = 30005,
        HasKeyboardFocus = 30008,
        IsKeyboardFocusable = 30009,
        IsEnabled = 30010,
        AutomationId = 30011,
        ClassName = 30012,
        IsControlElement = 30016,
        IsContentElement = 30017,
        NativeWindowHandle = 30020,
        IsOffscreen = 30022,
        FrameworkId = 30024,
        IsInvokePatternAvailable = 30031,
        IsScrollPatternAvailable = 30034,
        IsValuePatternAvailable = 30043,
        ValueValue = 30045,
        ValueIsReadOnly = 30046,
        ScrollHorizontalScrollPercent = 30054,
        ScrollHorizontalViewSize = 30055,
        ScrollVerticalScrollPercent = 30056,
        ScrollVerticalViewSize = 30057,
        ScrollHorizontallyScrollable = 30058,
        ScrollVerticallyScrollable = 30059,
    }

    private enum FlutterWindowsUiaControlTypeId
    {
        Button = 50000,
        ComboBox = 50003,
        Edit = 50004,
        Image = 50006,
        ListItem = 50007,
        List = 50008,
        Menu = 50009,
        MenuBar = 50010,
        MenuItem = 50011,
        ProgressBar = 50012,
        Slider = 50015,
        Spinner = 50016,
        StatusBar = 50017,
        Tab = 50018,
        TabItem = 50019,
        Text = 50020,
        ToolTip = 50022,
        Group = 50026,
        Thumb = 50027,
        DataItem = 50029,
        Window = 50032,
        Pane = 50033,
        HeaderItem = 50035,
        Table = 50036,
    }

    [Flags]
    internal enum FlutterWindowsUiaProviderOptions
    {
        ServerSideProvider = 0x0002,
    }

    [Guid("670c3006-bf4c-428b-8534-e1848f645122")]
    internal enum FlutterWindowsUiaNavigateDirection
    {
        Parent,
        NextSibling,
        PreviousSibling,
        FirstChild,
        LastChild,
    }

    internal enum FlutterWindowsUiaScrollAmount
    {
        LargeDecrement,
        SmallDecrement,
        NoAmount,
        LargeIncrement,
        SmallIncrement,
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("d6dd68d1-86fd-4332-8666-9abedea2d24c")]
    internal interface IFlutterWindowsRawElementProviderSimple
    {
        FlutterWindowsUiaProviderOptions GetProviderOptions();

        [return: MarshalAs(UnmanagedType.Interface)]
        object? GetPatternProvider(int patternId);

        [return: MarshalAs(UnmanagedType.Struct)]
        object? GetPropertyValue(int propertyId);

        IFlutterWindowsRawElementProviderSimple? GetHostRawElementProvider();
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("f7063da8-8359-439c-9297-bbc5299a7d87")]
    internal interface IFlutterWindowsRawElementProviderFragment
    {
        IFlutterWindowsRawElementProviderFragment? Navigate(FlutterWindowsUiaNavigateDirection direction);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)]
        int[]? GetRuntimeId();

        FlutterWindowsUiaRect GetBoundingRectangle();

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
        IFlutterWindowsRawElementProviderFragmentRoot[]? GetEmbeddedFragmentRoots();

        void SetFocus();

        IFlutterWindowsRawElementProviderFragmentRoot? GetFragmentRoot();
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("620ce2a5-ab8f-40a9-86cb-de3c75599b58")]
    internal interface IFlutterWindowsRawElementProviderFragmentRoot
    {
        IFlutterWindowsRawElementProviderFragment? ElementProviderFromPoint(double x, double y);

        IFlutterWindowsRawElementProviderFragment? GetFocus();
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("54fcb24b-e18e-47a2-b4d3-eccbe77599a2")]
    internal interface IFlutterWindowsInvokeProvider
    {
        void Invoke();
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("c7935180-6fb3-4201-b174-7df73adbf64a")]
    internal interface IFlutterWindowsValueProvider
    {
        void SetValue([MarshalAs(UnmanagedType.LPWStr)] string? value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string? GetValue();

        [return: MarshalAs(UnmanagedType.Bool)]
        bool GetIsReadOnly();
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("b38b8077-1fc3-42a5-8cae-d40c2215055a")]
    internal interface IFlutterWindowsScrollProvider
    {
        void Scroll(FlutterWindowsUiaScrollAmount horizontalAmount, FlutterWindowsUiaScrollAmount verticalAmount);

        void SetScrollPercent(double horizontalPercent, double verticalPercent);

        double GetHorizontalScrollPercent();

        double GetVerticalScrollPercent();

        double GetHorizontalViewSize();

        double GetVerticalViewSize();

        [return: MarshalAs(UnmanagedType.Bool)]
        bool GetHorizontallyScrollable();

        [return: MarshalAs(UnmanagedType.Bool)]
        bool GetVerticallyScrollable();
    }

    private static partial class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct NativePoint
        {
            internal int X;
            internal int Y;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ClientToScreen(nint hwnd, ref NativePoint point);

        [DllImport("user32.dll")]
        internal static extern nint GetFocus();

        [DllImport("UIAutomationCore.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UiaClientsAreListening();

        [DllImport("UIAutomationCore.dll")]
        internal static extern nint UiaReturnRawElementProvider(
            nint hwnd,
            nint wParam,
            nint lParam,
            [MarshalAs(UnmanagedType.Interface)]
            IFlutterWindowsRawElementProviderSimple provider);

        [DllImport("UIAutomationCore.dll")]
        internal static extern int UiaHostProviderFromHwnd(
            nint hwnd,
            [MarshalAs(UnmanagedType.Interface)]
            out IFlutterWindowsRawElementProviderSimple provider);

        [DllImport("UIAutomationCore.dll")]
        internal static extern int UiaRaiseAutomationEvent(
            [MarshalAs(UnmanagedType.Interface)]
            IFlutterWindowsRawElementProviderSimple provider,
            int eventId);

        [DllImport("UIAutomationCore.dll")]
        internal static extern int UiaDisconnectProvider(
            [MarshalAs(UnmanagedType.Interface)]
            IFlutterWindowsRawElementProviderSimple provider);
    }
}
