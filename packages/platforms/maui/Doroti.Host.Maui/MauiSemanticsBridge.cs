using System.Diagnostics;
using Doroti.Ui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;

namespace Doroti.Host.Maui;

/// <summary>Mirrors retained semantics into native accessibility views without creating a second touch tree.</summary>
internal sealed class MauiSemanticsBridge(AbsoluteLayout layer, Action<Action, TimeSpan>? schedule = null) : IMauiSemanticsBridge, IDisposable
{
    private static readonly TimeSpan MinimumApplyInterval = TimeSpan.FromMilliseconds(
#if IOS && !MACCATALYST
        67
#else
        1000.0 / 15.0
#endif
    );
    private readonly AbsoluteLayout _layer = layer;
    private readonly object _gate = new();
    private readonly Dictionary<int, NativeElementState> _elements = [];
    private readonly Dictionary<NativeElementKind, Stack<NativeElementState>> _recycledElements =
    [];
    private readonly SemanticsVisibleTree _visible = new(SemanticsPlatformViewExposure.ExcludeSubtree);
    private SemanticsTreeSnapshot? _tree;
    private Action<int, SemanticsAction, object?>? _performAction;
    private readonly CancellationTokenSource _lifetime = new();
    private PendingUpdate? _pending;
    private bool _applyScheduled;
    private bool _disposed;
    private int _projectionDepth;
    private long _scheduleGeneration;
    private long _lastReceivedGeneration = -1;
    private long _updatesReceived;
    private long _updatesApplied;
    private long _updatesCoalesced;
    private long _elementsCreated;
    private long _activeElements;
    private long _retainedNodes;
    private long _nativePropertyWrites;
    private long _immediateFlushes;
    private long _staleCallbacksSuppressed;
    private long _updatesSuppressed;
    private long _elementsReused;
    private long _topologyUpdatesApplied;
    private long _applyWorkMicroseconds;
    private long _maxApplyWorkMicroseconds;
    private long _lastApplyTimestamp;
    private DorotiFrameTrace? _frameTrace;
    private ulong _viewId;

    public MauiSemanticsDiagnostics Diagnostics =>
        new(
            Interlocked.Read(ref _updatesReceived),
            Interlocked.Read(ref _updatesApplied),
            Interlocked.Read(ref _updatesCoalesced),
            Interlocked.Read(ref _elementsCreated),
            Interlocked.Read(ref _activeElements),
            Interlocked.Read(ref _retainedNodes),
            Interlocked.Read(ref _nativePropertyWrites),
            Interlocked.Read(ref _immediateFlushes),
            Interlocked.Read(ref _staleCallbacksSuppressed),
            Interlocked.Read(ref _updatesSuppressed),
            Interlocked.Read(ref _elementsReused),
            Interlocked.Read(ref _topologyUpdatesApplied),
            Interlocked.Read(ref _applyWorkMicroseconds),
            Interlocked.Read(ref _maxApplyWorkMicroseconds)
        );

    public void AttachFrameTrace(DorotiFrameTrace trace, ulong viewId)
    {
        ArgumentNullException.ThrowIfNull(trace);
        _frameTrace = trace;
        _viewId = viewId;
    }

    public void Update(SemanticsUpdate update, Action<int, SemanticsAction, object?> performAction)
    {
        using var allocationProfile = FrameworkWorkProfile.AllocationEnabled
            ? FrameworkWorkProfile.Begin(GetType(), 16)
            : default;
        ArgumentNullException.ThrowIfNull(update);
        ArgumentNullException.ThrowIfNull(performAction);
        Interlocked.Increment(ref _updatesReceived);
        var throttle = ShouldThrottleScrollUpdate(update);
        long scheduleId;
        TimeSpan delay;
        lock (_gate)
        {
            if (_disposed || update.generation < _lastReceivedGeneration)
            {
                Interlocked.Increment(ref _staleCallbacksSuppressed);
                return;
            }
            _lastReceivedGeneration = update.generation;
            _performAction = performAction;
            var immediate = false;
            if (throttle)
            {
                // The renderer sends a complete snapshot. Only filter and diff
                // the latest snapshot when it reaches the UIKit dispatcher.
                _pending = new(update, null);
            }
            else
            {
                var delta = _visible.Prepare(update);
                // Even an applied-equivalent update replaces older pending work and
                // refreshes its action owner on the native dispatcher.
                if (!delta.HasChanges) Interlocked.Increment(ref _updatesSuppressed);
                _pending = new(update, delta);
                immediate =
                    update.urgency is SemanticsUpdateUrgency.immediate or SemanticsUpdateUrgency.scrollEnd
                    || delta.Changes.Any(change => change.Previous is not null
                        && (change.Properties & ~(SemanticsNodeProperty.bounds | SemanticsNodeProperty.scroll
                            | SemanticsNodeProperty.textGeometry | SemanticsNodeProperty.children
                            | SemanticsNodeProperty.traversal)) != 0)
                    || !delta.HasChanges;
            }

            if (_applyScheduled)
            {
                Interlocked.Increment(ref _updatesCoalesced);
                if (!immediate)
                {
                    // Keep the existing deadline and let its callback take the latest tree.
                    return;
                }
            }
            if (immediate)
            {
                Interlocked.Increment(ref _immediateFlushes);
            }

            _applyScheduled = true;
            scheduleId = ++_scheduleGeneration;
            delay = immediate ? TimeSpan.Zero : RemainingApplyDelay();
        }
        ScheduleApply(scheduleId, delay);
    }

    private bool ShouldThrottleScrollUpdate(SemanticsUpdate update) =>
#if IOS && !MACCATALYST
        update.urgency == SemanticsUpdateUrgency.automatic
        && _frameTrace?.HasActiveScrollActivity == true
        && !UIKit.UIAccessibility.IsVoiceOverRunning
        && !UIKit.UIAccessibility.IsSwitchControlRunning;
#else
        false;
#endif

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Reset();
        }
        _lifetime.Cancel();
        _lifetime.Dispose();
    }

    public void Clear()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            Reset();
        }
    }

    private void Reset()
    {
        var retired = _elements.Values.ToArray();
        _pending = null;
        _applyScheduled = false;
        _scheduleGeneration++;
        _performAction = null;
        _lastReceivedGeneration = -1;
        _lastApplyTimestamp = 0;
        _visible.Clear();
        _tree = null;
        _elements.Clear();
        _recycledElements.Clear();
        Interlocked.Exchange(ref _activeElements, 0);
        Interlocked.Exchange(ref _retainedNodes, 0);
        _layer.Dispatcher.Dispatch(() =>
        {
            // A new snapshot may already have attached other views by the time this runs.
            foreach (var state in retired)
            {
                state.Container.UpdateNode(null);
                DetachContainer(state);
            }
        });
    }

    private void ApplyLatest(long scheduleId)
    {
        PendingUpdate? pending;
        lock (_gate)
        {
            if (_disposed || scheduleId != _scheduleGeneration)
            {
                Interlocked.Increment(ref _staleCallbacksSuppressed);
                return;
            }
            pending = _pending;
            _pending = null;
            _applyScheduled = false;
        }
        if (pending is null)
        {
            return;
        }

        var started = DorotiFrameClock.Now;
        _frameTrace?.Record(
            DorotiFramePhase.semanticsApply,
            _viewId,
            started,
            reason: $"generation {pending.Update.generation}"
        );
        try
        {
            lock (_gate)
            {
                if (_disposed || scheduleId != _scheduleGeneration) return;
                if (Apply(pending.Delta ?? _visible.Prepare(pending.Update)))
                    Interlocked.Increment(ref _updatesApplied);
                _lastApplyTimestamp = Stopwatch.GetTimestamp();
            }
        }
        finally
        {
            var finished = DorotiFrameClock.Now;
            var elapsedMicroseconds = Math.Max(0, (finished - started).Ticks / 10);
            Interlocked.Add(ref _applyWorkMicroseconds, elapsedMicroseconds);
            UpdateMaximum(ref _maxApplyWorkMicroseconds, elapsedMicroseconds);
            _frameTrace?.Record(
                DorotiFramePhase.semanticsApplyEnd,
                _viewId,
                finished,
                reason: $"generation {pending.Update.generation}"
            );
        }
    }

    private TimeSpan RemainingApplyDelay()
    {
        if (_lastApplyTimestamp == 0)
        {
            return TimeSpan.Zero;
        }

        var remaining = MinimumApplyInterval - Stopwatch.GetElapsedTime(_lastApplyTimestamp);
        return remaining <= TimeSpan.Zero
            ? TimeSpan.Zero
            : TimeSpan.FromMilliseconds(Math.Ceiling(remaining.TotalMilliseconds));
    }

    private void ScheduleApply(long scheduleId, TimeSpan delay)
    {
        if (schedule is not null) { schedule(() => ApplyLatest(scheduleId), delay); return; }
        if (delay <= TimeSpan.Zero)
        {
            _layer.Dispatcher.Dispatch(() => ApplyLatest(scheduleId));
            return;
        }
        lock (_gate)
        {
            if (_disposed || scheduleId != _scheduleGeneration) return;
            _ = DelayAndApplyLatest(scheduleId, delay, _lifetime.Token);
        }
    }

    private async Task DelayAndApplyLatest(
        long scheduleId,
        TimeSpan delay,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            if (!cancellationToken.IsCancellationRequested)
            {
                _layer.Dispatcher.Dispatch(() => ApplyLatest(scheduleId));
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Interlocked.Increment(ref _staleCallbacksSuppressed);
        }
    }

    private bool Apply(SemanticsVisibleUpdate delta)
    {
        using var allocationProfile = FrameworkWorkProfile.AllocationEnabled
            ? FrameworkWorkProfile.Begin(GetType(), 17)
            : default;
        _visible.Commit(delta);
        if (!delta.HasChanges) return false;

        var rebuildOrder = delta.TopologyChanged;
        _layer.BatchBegin();
        _projectionDepth++;
        try
        {
            foreach (var staleId in delta.Removed)
            {
                if (_elements.TryGetValue(staleId, out var staleState))
                {
                    DetachContainer(staleState);
                    RecycleState(staleState);
                }
                _elements.Remove(staleId);
            }

            foreach (var change in delta.Changes)
            {
                var node = change.Current;
                var kind = ElementKindFor(node);
                if (!_elements.TryGetValue(node.id, out var state) || state.Kind != kind)
                {
                    if (state is not null)
                    {
                        DetachContainer(state);
                        RecycleState(state);
                    }
                    state = AcquireState(kind);
                    _elements[node.id] = state;
                    rebuildOrder = true;
                    UpdateState(state, node, AllProperties);
                }
                else UpdateState(state, node, change.Properties);
            }

            var tree = _tree = delta.TopologyChanged || _tree is null
                ? SemanticsTreeSnapshot.IndexRetainedNodes(_visible.Nodes) : _tree;
            if (rebuildOrder)
            {
                Interlocked.Increment(ref _topologyUpdatesApplied);
                SynchronizeChildOrder(tree);
                foreach (var node in tree.Nodes.Values) LayoutNode(tree, node);
            }
            else
            {
                HashSet<int>? affected = null;
                foreach (var change in delta.Changes)
                {
                    if (change.Previous?.rect == change.Current.rect) continue;
                    (affected ??= []).Add(change.Current.id);
                    if (change.Previous?.rect.topLeft != change.Current.rect.topLeft)
                        affected.UnionWith(tree.Children(change.Current.id));
                }
                if (affected is not null)
                    foreach (var id in affected) LayoutNode(tree, tree.Nodes[id]);
            }
            Interlocked.Exchange(ref _activeElements, _elements.Count);
            Interlocked.Exchange(ref _retainedNodes, _visible.Nodes.Count);
            return true;
        }
        finally
        {
            try
            {
                _layer.BatchCommit();
            }
            finally
            {
                _projectionDepth--;
            }
        }
    }

    private NativeElementState AcquireState(NativeElementKind kind)
    {
        if (_recycledElements.TryGetValue(kind, out var recycled) && recycled.TryPop(out var state))
        {
            Interlocked.Increment(ref _elementsReused);
            return state;
        }
        Interlocked.Increment(ref _elementsCreated);
        return CreateState(kind);
    }

    private void RecycleState(NativeElementState state)
    {
        state.Container.UpdateNode(null);
        foreach (var child in state.Container.Children.OfType<MauiSemanticsLayout>().ToArray()) state.Container.Children.Remove(child);
        state.Node = null;
        state.LayoutBounds = null;
        // An exposed native peer must never become a different node through pooling.
        if (state.Container.Handler is not null) return;
        if (!_recycledElements.TryGetValue(state.Kind, out var recycled))
        {
            recycled = new Stack<NativeElementState>();
            _recycledElements.Add(state.Kind, recycled);
        }
        recycled.Push(state);
    }

    private void SynchronizeChildOrder(SemanticsTreeSnapshot tree)
    {
        // Remove old ownership links before inserting their replacements, so a tree
        // that inverts a parent/child relationship never creates a transient cycle.
        foreach (var (id, state) in _elements)
        {
            Layout owner = tree.Parents.TryGetValue(id, out var parentId)
                ? _elements[parentId].Container
                : _layer;
            if (state.Container.Parent is Layout previous && !ReferenceEquals(previous, owner))
            {
                previous.Remove(state.Container);
            }
        }

        SynchronizeChildren(_layer, tree.Roots, 0);
        foreach (var node in tree.Nodes.Values)
        {
            var owner = _elements[node.id].Container;
            if (node.children.Count != 0 || owner.Count > 1)
            {
                // The projected control occupies the first slot in each container.
                SynchronizeChildren(owner, tree.Children(node.id), 1);
            }
        }
    }

    private void SynchronizeChildren(Layout owner, IReadOnlyList<int> childIds, int index)
    {
        foreach (var id in childIds)
        {
            var child = _elements[id].Container;
            if (index >= owner.Count || !ReferenceEquals(owner[index], child))
            {
                var previousIndex = owner.IndexOf(child);
                if (previousIndex >= 0)
                {
                    if (previousIndex < index) continue; // This child was already listed.
                    owner.RemoveAt(previousIndex);
                }
                owner.Insert(index, child);
            }
            index++;
        }
        while (owner.Count > index) owner.RemoveAt(owner.Count - 1);
    }

    private static void DetachContainer(NativeElementState state)
    {
        if (state.Container.Parent is Layout parent) parent.Children.Remove(state.Container);
    }

    private void LayoutNode(SemanticsTreeSnapshot tree, SemanticsNodeUpdate node)
    {
        var state = _elements[node.id];
        var origin = tree.Parents.TryGetValue(node.id, out var parent)
            ? tree.Nodes[parent].rect.topLeft : Doroti.Ui.Offset.zero;
        var bounds = new Microsoft.Maui.Graphics.Rect(
            node.rect.left - origin.dx,
            node.rect.top - origin.dy,
            Math.Max(0, node.rect.width),
            Math.Max(0, node.rect.height)
        );
        if (state.LayoutBounds == bounds) return;
        AbsoluteLayout.SetLayoutBounds(state.Container, bounds);
        state.LayoutBounds = bounds;
        Interlocked.Increment(ref _nativePropertyWrites);
    }

    private NativeElementState CreateState(NativeElementKind kind)
    {
        View element = kind switch
        {
            NativeElementKind.TextField => new Entry { Opacity = 0 },
            NativeElementKind.Checkbox => new CheckBox { Opacity = 0 },
            // These are projections of framework-owned groups. MAUI otherwise
            // groups every RadioButton in this flat layer together and unchecks
            // siblings during an update, emitting spurious actions for them.
            NativeElementKind.Radio => new RadioButton
            {
                Opacity = 0,
                GroupName = Guid.NewGuid().ToString("N"),
            },
            NativeElementKind.Toggle => new Microsoft.Maui.Controls.Switch { Opacity = 0 },
            NativeElementKind.Slider => new Slider { Opacity = 0 },
            NativeElementKind.Button => new Button { Opacity = 0 },
            _ => new Label { Opacity = 0 },
        };
        var state = new NativeElementState(kind, element);
        state.Container.DispatchAction = (action, arguments) => PerformAction(state, action, arguments);
        AbsoluteLayout.SetLayoutFlags(state.Container, AbsoluteLayoutFlags.None);
        state.Container.Children.Add(element);
#if ANDROID
        state.Container.ObserveAndroidProxy(element);
#endif
        AbsoluteLayout.SetLayoutBounds(element, new(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(element, AbsoluteLayoutFlags.All);
        if (element is Entry entry)
        {
            entry.TextChanged += (_, args) =>
            {
                var node = state.Node;
                var text = args.NewTextValue ?? string.Empty;
                if (
                    node is not null
                    && text == (entry.Text ?? string.Empty)
                    && ObserveValueChange(state, text)
                )
                {
                    PerformAction(state, SemanticsAction.setText, text);
                }
            };
            entry.PropertyChanged += (_, args) =>
            {
                var node = state.Node;
                if (
                    node is not null
                    && args.PropertyName
                        is nameof(InputView.CursorPosition)
                            or nameof(InputView.SelectionLength)
                )
                {
                    var expected = state.ObservedSelection;
                    var start = entry.CursorPosition;
                    var end = start + entry.SelectionLength;
                    state.ObservedSelection = (start, end);
                    if (start != expected.Start || end != expected.End)
                    {
                        PerformAction(
                            state,
                            SemanticsAction.setSelection,
                            new Dictionary<string, long> { ["base"] = start, ["extent"] = end }
                        );
                    }
                }
            };
        }
        else if (element is Button button)
        {
            button.Clicked += (_, _) => PerformAction(state, SemanticsAction.tap);
        }
        else if (element is CheckBox checkbox)
        {
            checkbox.CheckedChanged += (_, args) =>
            {
                if (
                    state.Node is { } node
                    && args.Value == checkbox.IsChecked
                    && ObserveValueChange(state, args.Value)
                )
                {
                    PerformAction(state, SemanticsAction.tap);
                }
            };
        }
        else if (element is RadioButton radio)
        {
            // Selecting a radio is an activation; deselection is a consequence
            // of another selection and must never re-activate the old option.
            radio.CheckedChanged += (_, args) =>
            {
                if (
                    args.Value == radio.IsChecked
                    && ObserveValueChange(state, args.Value)
                    && args.Value
                )
                {
                    PerformAction(state, SemanticsAction.tap);
                }
            };
        }
        else if (element is Microsoft.Maui.Controls.Switch toggle)
        {
            toggle.Toggled += (_, args) =>
            {
                if (
                    state.Node is { } node
                    && args.Value == toggle.IsToggled
                    && ObserveValueChange(state, args.Value)
                )
                {
                    PerformAction(state, SemanticsAction.tap);
                }
            };
        }
        else if (element is Slider slider)
        {
            slider.ValueChanged += (_, args) =>
            {
                if (
                    state.Node is not { } node
                    || args.NewValue != slider.Value
                    || !double.IsFinite(args.NewValue)
                )
                {
                    return;
                }

                var current = state.ObservedValue is double value ? value : args.OldValue;
                if (!ObserveValueChange(state, args.NewValue))
                {
                    return;
                }

                PerformAction(
                    state,
                    args.NewValue > current ? SemanticsAction.increase : SemanticsAction.decrease
                );
            };
        }
        element.Focused += (_, _) =>
        {
            var node = state.Node;
            if (element.IsFocused && node?.flags?.isFocused != Tristate.isTrue)
            {
                PerformAction(state, SemanticsAction.focus);
            }
        };
        return state;
    }

    // Native property notifications are not themselves accessibility actions.
    // All controls share this boundary: publishing an entire tree (including
    // reparenting/focus changes) must never feed actions back into the framework.
    // The value comparisons above also reject delayed/no-op projection echoes.
    private bool PerformAction(
        NativeElementState state,
        SemanticsAction action,
        object? arguments = null
    )
    {
        if (
            _disposed
            || _projectionDepth != 0
            || state.Updating
            || state.Node is not { } node
            || !_elements.TryGetValue(node.id, out var current)
            || !ReferenceEquals(current, state)
            || !SemanticsActionPolicy.Allows(node, action)
        )
        {
            return false;
        }

        if (
            node.flags?.isReadOnly == true
            && action
                is SemanticsAction.setText
                    or SemanticsAction.cut
                    or SemanticsAction.paste
                    or SemanticsAction.increase
                    or SemanticsAction.decrease
        )
        {
            return false;
        }

        if (_performAction is not { } owner) return false;
        owner(node.id, action, arguments);
        return true;
    }

    // Advance the baseline on real native changes too: a second action may
    // restore the projected value before the framework publishes another tree.
    private static bool ObserveValueChange(NativeElementState state, object value)
    {
        var changed = !Equals(state.ObservedValue, value);
        state.ObservedValue = value;
        return changed;
    }

    private static bool RadioChecked(SemanticsNodeUpdate node) =>
        node.flags?.isChecked is { } checkedState && checkedState != CheckedState.none
            ? checkedState == CheckedState.isTrue
            : node.flags?.isSelected == Tristate.isTrue;

    internal static (int Start, int End) ProjectedSelection(SemanticsNodeUpdate node)
    {
        var length = (node.value ?? string.Empty).Length;
        return (
            (int)Math.Clamp(Math.Min(node.textSelectionBase, node.textSelectionExtent), 0, length),
            (int)Math.Clamp(Math.Max(node.textSelectionBase, node.textSelectionExtent), 0, length)
        );
    }

    private void UpdateState(
        NativeElementState state,
        SemanticsNodeUpdate node,
        SemanticsNodeProperty properties
    )
    {
        state.Node = node;
        state.Container.UpdateNode(node);
        if ((properties & ~(SemanticsNodeProperty.bounds | SemanticsNodeProperty.scroll
            | SemanticsNodeProperty.textGeometry | SemanticsNodeProperty.children
            | SemanticsNodeProperty.traversal)) == SemanticsNodeProperty.none) return;
        state.Updating = true;
        try
        {
            if (!state.Element.InputTransparent)
            {
                state.Element.InputTransparent = true;
                Interlocked.Increment(ref _nativePropertyWrites);
            }
            var enabled = node.flags?.isEnabled != Tristate.isFalse;
            if (state.Element.IsEnabled != enabled)
            {
                state.Element.IsEnabled = enabled;
                Interlocked.Increment(ref _nativePropertyWrites);
            }
            if (
                (properties & (SemanticsNodeProperty.value | SemanticsNodeProperty.flags)) != 0
                && state.Element is Entry entry
            )
            {
                var text = node.flags?.isObscured == true ? string.Empty : node.value ?? string.Empty;
                if (!string.Equals(entry.Text, text, StringComparison.Ordinal))
                {
                    entry.Text = text;
                    Interlocked.Increment(ref _nativePropertyWrites);
                }
                var readOnly = node.flags?.isReadOnly == true;
                if (entry.IsReadOnly != readOnly)
                {
                    entry.IsReadOnly = readOnly;
                    Interlocked.Increment(ref _nativePropertyWrites);
                }
            }
            if (
                (properties & (SemanticsNodeProperty.selection | SemanticsNodeProperty.value)) != 0
                && state.Element is Entry selectionEntry
                && node.textSelectionBase >= 0
                && node.textSelectionExtent >= 0
            )
            {
                var selection = ProjectedSelection(node);
                if (selectionEntry.CursorPosition != selection.Start)
                {
                    selectionEntry.CursorPosition = selection.Start;
                    Interlocked.Increment(ref _nativePropertyWrites);
                }
                if (selectionEntry.SelectionLength != selection.End - selection.Start)
                {
                    selectionEntry.SelectionLength = selection.End - selection.Start;
                    Interlocked.Increment(ref _nativePropertyWrites);
                }
            }
            if (
                (
                    properties
                    & (
                        SemanticsNodeProperty.label
                        | SemanticsNodeProperty.value
                        | SemanticsNodeProperty.flags
                    )
                ) != 0
                && state.Element is Button button
            )
            {
                var text = node.label ?? (node.flags?.isObscured == true ? null : node.value) ?? string.Empty;
                if (!string.Equals(button.Text, text, StringComparison.Ordinal))
                {
                    button.Text = text;
                    Interlocked.Increment(ref _nativePropertyWrites);
                }
            }
            if (
                (properties & (SemanticsNodeProperty.label | SemanticsNodeProperty.value | SemanticsNodeProperty.flags)) != 0
                && state.Element is Label label
            )
            {
                var text = node.label ?? (node.flags?.isObscured == true ? null : node.value) ?? string.Empty;
                if (!string.Equals(label.Text, text, StringComparison.Ordinal))
                {
                    label.Text = text;
                    Interlocked.Increment(ref _nativePropertyWrites);
                }
            }
            if (
                (
                    properties
                    & (
                        SemanticsNodeProperty.label
                        | SemanticsNodeProperty.value
                        | SemanticsNodeProperty.flags
                        | SemanticsNodeProperty.role
                        | SemanticsNodeProperty.metadata
                    )
                ) != 0
            )
            {
                var description = string.Join(
                    " ",
                    new[] { node.label, node.flags?.isTextField == true || node.flags?.isObscured == true ? null : node.value }.Where(
                        value => !string.IsNullOrWhiteSpace(value)
                    )
                );
                if (!string.Equals(state.Description, description, StringComparison.Ordinal))
                {
                    SemanticProperties.SetDescription(state.Element, description);
                    state.Description = description;
                    Interlocked.Increment(ref _nativePropertyWrites);
                }
                var hint = string.Join(
                    " ",
                    new[] { node.hint, node.tooltip }.Where(value =>
                        !string.IsNullOrWhiteSpace(value)
                    )
                );
                if (!string.Equals(state.Hint, hint, StringComparison.Ordinal))
                {
                    SemanticProperties.SetHint(state.Element, hint);
                    state.Hint = hint;
                    Interlocked.Increment(ref _nativePropertyWrites);
                }
                var heading = HeadingLevelFor(node);
                if (state.Heading != heading)
                {
                    SemanticProperties.SetHeadingLevel(state.Element, heading);
                    state.Heading = heading;
                    Interlocked.Increment(ref _nativePropertyWrites);
                }
            }
            if (
                (
                    properties
                    & (
                        SemanticsNodeProperty.flags
                        | SemanticsNodeProperty.value
                        | SemanticsNodeProperty.metadata
                    )
                ) != 0
            )
            {
                if (state.Element is CheckBox checkbox)
                {
                    var isChecked =
                        node.flags?.isChecked is CheckedState.isTrue or CheckedState.mixed;
                    if (checkbox.IsChecked != isChecked)
                    {
                        checkbox.IsChecked = isChecked;
                        Interlocked.Increment(ref _nativePropertyWrites);
                    }
                }
                else if (state.Element is RadioButton radio)
                {
                    var isChecked = RadioChecked(node);
                    if (radio.IsChecked != isChecked)
                    {
                        radio.IsChecked = isChecked;
                        Interlocked.Increment(ref _nativePropertyWrites);
                    }
                }
                else if (state.Element is Microsoft.Maui.Controls.Switch toggle)
                {
                    var isToggled = node.flags?.isToggled == Tristate.isTrue;
                    if (toggle.IsToggled != isToggled)
                    {
                        toggle.IsToggled = isToggled;
                        Interlocked.Increment(ref _nativePropertyWrites);
                    }
                }
                else if (state.Element is Slider slider)
                {
                    var minimum = TryParseDouble(node.minValue, out var parsedMinimum)
                        ? parsedMinimum
                        : slider.Minimum;
                    var maximum = TryParseDouble(node.maxValue, out var parsedMaximum)
                        ? parsedMaximum
                        : slider.Maximum;
                    if (minimum <= maximum)
                    {
                        if (minimum > slider.Maximum && slider.Maximum != maximum)
                        {
                            slider.Maximum = maximum;
                            Interlocked.Increment(ref _nativePropertyWrites);
                        }
                        if (maximum < slider.Minimum && slider.Minimum != minimum)
                        {
                            slider.Minimum = minimum;
                            Interlocked.Increment(ref _nativePropertyWrites);
                        }
                        if (slider.Minimum != minimum)
                        {
                            slider.Minimum = minimum;
                            Interlocked.Increment(ref _nativePropertyWrites);
                        }
                        if (slider.Maximum != maximum)
                        {
                            slider.Maximum = maximum;
                            Interlocked.Increment(ref _nativePropertyWrites);
                        }
                    }
                    if (TryParseDouble(node.value, out var value))
                    {
                        value = Math.Clamp(value, slider.Minimum, slider.Maximum);
                        if (slider.Value != value)
                        {
                            slider.Value = value;
                            Interlocked.Increment(ref _nativePropertyWrites);
                        }
                    }
                }
            }
        }
        finally
        {
            state.ObservedValue = state.Element switch
            {
                Entry entry => entry.Text ?? string.Empty,
                CheckBox checkbox => checkbox.IsChecked,
                RadioButton radio => radio.IsChecked,
                Microsoft.Maui.Controls.Switch toggle => toggle.IsToggled,
                Slider slider => slider.Value,
                _ => null,
            };
            if (state.Element is Entry entrySelection)
            {
                state.ObservedSelection = (
                    entrySelection.CursorPosition,
                    entrySelection.CursorPosition + entrySelection.SelectionLength
                );
            }

            state.Updating = false;
        }
    }

    private static NativeElementKind ElementKindFor(SemanticsNodeUpdate node) =>
        node.flags?.isTextField == true ? NativeElementKind.TextField
        : node.flags?.isSlider == true ? NativeElementKind.Slider
        : node.flags?.isToggled is Tristate.isTrue or Tristate.isFalse ? NativeElementKind.Toggle
        : node.flags?.isInMutuallyExclusiveGroup == true
        && (node.flags.isChecked != CheckedState.none || node.flags.isSelected != Tristate.none)
            ? NativeElementKind.Radio
        : node.flags?.isChecked is CheckedState.isTrue or CheckedState.isFalse or CheckedState.mixed ? NativeElementKind.Checkbox
        : node.actions.HasFlag(SemanticsAction.tap) ? NativeElementKind.Button
        : NativeElementKind.Label;

    private static bool TryParseDouble(string? value, out double result) =>
        double.TryParse(
            value,
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out result
        ) && double.IsFinite(result);

    private static SemanticHeadingLevel HeadingLevelFor(SemanticsNodeUpdate node) =>
        node.headingLevel switch
        {
            1 => SemanticHeadingLevel.Level1,
            2 => SemanticHeadingLevel.Level2,
            3 => SemanticHeadingLevel.Level3,
            4 => SemanticHeadingLevel.Level4,
            5 => SemanticHeadingLevel.Level5,
            6 => SemanticHeadingLevel.Level6,
            7 => SemanticHeadingLevel.Level7,
            8 => SemanticHeadingLevel.Level8,
            9 => SemanticHeadingLevel.Level9,
            _ when node.flags?.isHeader == true => SemanticHeadingLevel.Level1,
            _ => SemanticHeadingLevel.None,
        };

    private const SemanticsNodeProperty AllProperties =
        SemanticsNodeProperty.bounds
        | SemanticsNodeProperty.label
        | SemanticsNodeProperty.value
        | SemanticsNodeProperty.actions
        | SemanticsNodeProperty.flags
        | SemanticsNodeProperty.role
        | SemanticsNodeProperty.children
        | SemanticsNodeProperty.traversal
        | SemanticsNodeProperty.selection
        | SemanticsNodeProperty.metadata;

    private static void UpdateMaximum(ref long target, long candidate)
    {
        var current = Interlocked.Read(ref target);
        while (candidate > current)
        {
            var observed = Interlocked.CompareExchange(ref target, candidate, current);
            if (observed == current)
            {
                return;
            }

            current = observed;
        }
    }

    private sealed record PendingUpdate(
        SemanticsUpdate Update,
        SemanticsVisibleUpdate? Delta
    );

    private enum NativeElementKind
    {
        Label,
        Button,
        TextField,
        Checkbox,
        Radio,
        Toggle,
        Slider,
    }

    private sealed class NativeElementState(NativeElementKind kind, View element)
    {
        internal NativeElementKind Kind { get; } = kind;
        internal View Element { get; } = element;
        internal MauiSemanticsLayout Container { get; } = new() { InputTransparent = true, CascadeInputTransparent = false };
        internal SemanticsNodeUpdate? Node { get; set; }
        internal string? Description { get; set; }
        internal string? Hint { get; set; }
        internal SemanticHeadingLevel Heading { get; set; } = SemanticHeadingLevel.None;
        internal Microsoft.Maui.Graphics.Rect? LayoutBounds { get; set; }
        internal bool Updating { get; set; }
        internal object? ObservedValue { get; set; }
        internal (int Start, int End) ObservedSelection { get; set; }
    }
}
