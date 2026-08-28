using System.Diagnostics;
using Doroti.Ui;
using Microsoft.Maui.Controls;

namespace Doroti.Host.Maui;

/// <summary>Mirrors retained semantics into native accessibility views without creating a second touch tree.</summary>
internal sealed class MauiSemanticsBridge(AbsoluteLayout layer) : IMauiSemanticsBridge, IDisposable
{
    private static readonly TimeSpan MinimumApplyInterval = TimeSpan.FromMilliseconds(1000.0 / 15.0);
    private readonly AbsoluteLayout _layer = layer;
    private readonly object _gate = new();
    private readonly Dictionary<int, NativeElementState> _elements = [];
    private readonly Dictionary<NativeElementKind, Stack<NativeElementState>> _recycledElements = [];
    private readonly Dictionary<int, SemanticsNodeUpdate> _appliedNodes = [];
    private readonly CancellationTokenSource _lifetime = new();
    private PendingUpdate? _pending;
    private bool _applyScheduled;
    private bool _disposed;
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

    public MauiSemanticsDiagnostics Diagnostics => new(
        Interlocked.Read(ref _updatesReceived), Interlocked.Read(ref _updatesApplied),
        Interlocked.Read(ref _updatesCoalesced), Interlocked.Read(ref _elementsCreated),
        Interlocked.Read(ref _activeElements), Interlocked.Read(ref _retainedNodes),
        Interlocked.Read(ref _nativePropertyWrites), Interlocked.Read(ref _immediateFlushes),
        Interlocked.Read(ref _staleCallbacksSuppressed), Interlocked.Read(ref _updatesSuppressed),
        Interlocked.Read(ref _elementsReused), Interlocked.Read(ref _topologyUpdatesApplied),
        Interlocked.Read(ref _applyWorkMicroseconds), Interlocked.Read(ref _maxApplyWorkMicroseconds));

    public void AttachFrameTrace(DorotiFrameTrace trace, ulong viewId)
    {
        ArgumentNullException.ThrowIfNull(trace);
        _frameTrace = trace;
        _viewId = viewId;
    }

    public void Update(SemanticsUpdate update, Action<int, SemanticsAction, object?> performAction)
    {
        ArgumentNullException.ThrowIfNull(update);
        ArgumentNullException.ThrowIfNull(performAction);
        Interlocked.Increment(ref _updatesReceived);
        var visibleNodes = VisibleNodes(update.nodes);
        var schedule = false;
        var scheduleId = 0L;
        var delay = TimeSpan.Zero;
        lock (_gate)
        {
            if (_disposed || update.generation < _lastReceivedGeneration)
            {
                Interlocked.Increment(ref _staleCallbacksSuppressed);
                return;
            }
            _lastReceivedGeneration = update.generation;
            var delta = SemanticsUpdateDiffer.Diff(_appliedNodes, visibleNodes);
            if (!delta.HasChanges)
            {
                Interlocked.Increment(ref _updatesSuppressed);
                return;
            }

            _pending = new(update with { nodes = visibleNodes }, performAction);
            var immediate = update.urgency is SemanticsUpdateUrgency.immediate or SemanticsUpdateUrgency.scrollEnd ||
                            delta.RequiresImmediateFlush;
            if (_applyScheduled)
            {
                Interlocked.Increment(ref _updatesCoalesced);
                if (!immediate) return;
                // A critical update invalidates a previously scheduled scroll callback.
                _scheduleGeneration++;
            }
            else
            {
                _applyScheduled = true;
                _scheduleGeneration++;
            }
            if (immediate) Interlocked.Increment(ref _immediateFlushes);
            scheduleId = _scheduleGeneration;
            delay = immediate ? TimeSpan.Zero : RemainingApplyDelay();
            schedule = true;
        }
        if (schedule) ScheduleApply(scheduleId, delay);
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            _pending = null;
            _applyScheduled = false;
            _scheduleGeneration++;
        }
        _lifetime.Cancel();
        _lifetime.Dispose();
    }

    public void Clear()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _pending = null;
            _applyScheduled = false;
            _scheduleGeneration++;
            _appliedNodes.Clear();
            _elements.Clear();
            _recycledElements.Clear();
            _lastReceivedGeneration = -1;
        }
        _layer.Dispatcher.Dispatch(() => _layer.Children.Clear());
        Interlocked.Exchange(ref _activeElements, 0);
        Interlocked.Exchange(ref _retainedNodes, 0);
    }

    private static IReadOnlyList<SemanticsNodeUpdate> VisibleNodes(IReadOnlyList<SemanticsNodeUpdate> nodes)
    {
        var nodesById = nodes.ToDictionary(node => node.id);
        var hidden = nodes.Where(node => node.flags?.isHidden == true).Select(node => node.id).ToHashSet();
        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var hiddenId in hidden.ToArray())
            {
                if (!nodesById.TryGetValue(hiddenId, out var node)) continue;
                foreach (var childId in node.children) changed |= hidden.Add(childId);
            }
        }
        var visible = nodesById.Keys.Where(id => !hidden.Contains(id)).ToHashSet();
        return nodes.Where(node => visible.Contains(node.id))
            .Select(node => node with { children = node.children.Where(visible.Contains).ToArray() })
            .OrderBy(node => node.id)
            .ToArray();
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
        if (pending is null) return;
        var started = DorotiFrameClock.Now;
        _frameTrace?.Record(DorotiFramePhase.semanticsApply, _viewId, started,
            reason: $"generation {pending.Update.generation}");
        try
        {
            Apply(pending.Update, pending.PerformAction);
            Interlocked.Increment(ref _updatesApplied);
        }
        finally
        {
            var finished = DorotiFrameClock.Now;
            var elapsedMicroseconds = Math.Max(0, (finished - started).Ticks / 10);
            Interlocked.Add(ref _applyWorkMicroseconds, elapsedMicroseconds);
            UpdateMaximum(ref _maxApplyWorkMicroseconds, elapsedMicroseconds);
            _frameTrace?.Record(DorotiFramePhase.semanticsApplyEnd, _viewId, finished,
                reason: $"generation {pending.Update.generation}");
            Interlocked.Exchange(ref _lastApplyTimestamp, Stopwatch.GetTimestamp());
        }
    }

    private TimeSpan RemainingApplyDelay()
    {
        var timestamp = Interlocked.Read(ref _lastApplyTimestamp);
        if (timestamp == 0) return TimeSpan.Zero;
        var elapsed = Stopwatch.GetElapsedTime(timestamp);
        return elapsed >= MinimumApplyInterval ? TimeSpan.Zero : MinimumApplyInterval - elapsed;
    }

    private void ScheduleApply(long scheduleId, TimeSpan delay)
    {
        if (delay <= TimeSpan.Zero)
        {
            _layer.Dispatcher.Dispatch(() => ApplyLatest(scheduleId));
            return;
        }
        _ = DelayAndApplyLatest(scheduleId, delay, _lifetime.Token);
    }

    private async Task DelayAndApplyLatest(long scheduleId, TimeSpan delay, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            if (!cancellationToken.IsCancellationRequested)
                _layer.Dispatcher.Dispatch(() => ApplyLatest(scheduleId));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Interlocked.Increment(ref _staleCallbacksSuppressed);
        }
    }

    private void Apply(SemanticsUpdate update, Action<int, SemanticsAction, object?> performAction)
    {
        var delta = SemanticsUpdateDiffer.Diff(_appliedNodes, update.nodes);
        var changedById = delta.changedNodes.ToDictionary(node => node.id);
        var rebuildOrder = delta.HasTopologyChange;
        if (rebuildOrder) Interlocked.Increment(ref _topologyUpdatesApplied);
        _layer.BatchBegin();
        try
        {
            foreach (var staleId in delta.removedNodeIds)
            {
                if (_elements.TryGetValue(staleId, out var staleState))
                {
                    _layer.Children.Remove(staleState.Element);
                    RecycleState(staleState);
                }
                _elements.Remove(staleId);
                _appliedNodes.Remove(staleId);
            }

            foreach (var node in update.nodes)
            {
                var kind = ElementKindFor(node);
                if (!_elements.TryGetValue(node.id, out var state) || state.Kind != kind)
                {
                    if (state is not null)
                    {
                        _layer.Children.Remove(state.Element);
                        RecycleState(state);
                    }
                    state = AcquireState(kind);
                    _elements[node.id] = state;
                    rebuildOrder = true;
                    UpdateState(state, node, performAction, AllProperties);
                }
                else if (changedById.TryGetValue(node.id, out var nodeDelta))
                {
                    UpdateState(state, node, performAction, nodeDelta.changedProperties);
                }
                _appliedNodes[node.id] = node;
            }

            if (rebuildOrder)
            {
                SynchronizeChildOrder(update.nodes);
            }
            Interlocked.Exchange(ref _activeElements, _elements.Count);
            Interlocked.Exchange(ref _retainedNodes, _appliedNodes.Count);
        }
        finally
        {
            _layer.BatchCommit();
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
        state.Node = null;
        state.PerformAction = null;
        if (!_recycledElements.TryGetValue(state.Kind, out var recycled))
        {
            recycled = new Stack<NativeElementState>();
            _recycledElements.Add(state.Kind, recycled);
        }
        recycled.Push(state);
    }

    private void SynchronizeChildOrder(IReadOnlyList<SemanticsNodeUpdate> nodes)
    {
        var index = 0;
        foreach (var node in nodes.OrderBy(node => node.indexInParent ?? int.MaxValue).ThenBy(node => node.id))
        {
            var element = _elements[node.id].Element;
            var currentIndex = _layer.Children.IndexOf(element);
            if (currentIndex != index)
            {
                if (currentIndex >= 0) _layer.Children.RemoveAt(currentIndex);
                _layer.Children.Insert(index, element);
            }
            index++;
        }
        while (_layer.Children.Count > index) _layer.Children.RemoveAt(_layer.Children.Count - 1);
    }

    private static NativeElementState CreateState(NativeElementKind kind)
    {
        View element = kind switch
        {
            NativeElementKind.TextField => new Entry { Opacity = 0 },
            NativeElementKind.Checkbox => new CheckBox { Opacity = 0 },
            NativeElementKind.Radio => new RadioButton { Opacity = 0 },
            NativeElementKind.Toggle => new Microsoft.Maui.Controls.Switch { Opacity = 0 },
            NativeElementKind.Slider => new Slider { Opacity = 0 },
            NativeElementKind.Button => new Button { Opacity = 0 },
            _ => new Label { Opacity = 0 },
        };
        var state = new NativeElementState(kind, element);
        if (element is Entry entry)
        {
            entry.TextChanged += (_, args) =>
            {
                var node = state.Node;
                if (!state.Updating && node is not null && node.actions.HasFlag(SemanticsAction.setText))
                    state.PerformAction?.Invoke(node.id, SemanticsAction.setText, args.NewTextValue ?? string.Empty);
            };
            entry.PropertyChanged += (_, args) =>
            {
                var node = state.Node;
                if (!state.Updating && node is not null && node.actions.HasFlag(SemanticsAction.setSelection) &&
                    args.PropertyName is nameof(InputView.CursorPosition) or nameof(InputView.SelectionLength))
                    state.PerformAction?.Invoke(node.id, SemanticsAction.setSelection, new Dictionary<string, long>
                    { ["base"] = entry.CursorPosition, ["extent"] = entry.CursorPosition + entry.SelectionLength });
            };
        }
        else if (element is Button button)
        {
            button.Clicked += (_, _) =>
            {
                var node = state.Node;
                if (node is not null && node.actions.HasFlag(SemanticsAction.tap))
                    state.PerformAction?.Invoke(node.id, SemanticsAction.tap, null);
            };
        }
        else if (element is CheckBox checkbox)
        {
            checkbox.CheckedChanged += (_, _) => PerformTap(state);
        }
        else if (element is RadioButton radio)
        {
            radio.CheckedChanged += (_, _) => PerformTap(state);
        }
        else if (element is Microsoft.Maui.Controls.Switch toggle)
        {
            toggle.Toggled += (_, _) => PerformTap(state);
        }
        else if (element is Slider slider)
        {
            slider.ValueChanged += (_, args) =>
            {
                if (state.Updating || state.Node is not { } node) return;
                var action = args.NewValue >= args.OldValue ? SemanticsAction.increase : SemanticsAction.decrease;
                if (node.actions.HasFlag(action)) state.PerformAction?.Invoke(node.id, action, null);
            };
        }
        element.Focused += (_, _) =>
        {
            var node = state.Node;
            if (node is not null && node.actions.HasFlag(SemanticsAction.focus))
                state.PerformAction?.Invoke(node.id, SemanticsAction.focus, null);
        };
        return state;
    }

    private static void PerformTap(NativeElementState state)
    {
        if (state.Updating || state.Node is not { } node || !node.actions.HasFlag(SemanticsAction.tap)) return;
        state.PerformAction?.Invoke(node.id, SemanticsAction.tap, null);
    }

    private void UpdateState(NativeElementState state, SemanticsNodeUpdate node,
        Action<int, SemanticsAction, object?> performAction, SemanticsNodeProperty properties)
    {
        state.Updating = true;
        try
        {
            state.Node = node;
            state.PerformAction = performAction;
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
            if ((properties & (SemanticsNodeProperty.value | SemanticsNodeProperty.flags)) != 0 && state.Element is Entry entry)
            {
                var text = node.value ?? string.Empty;
                if (!string.Equals(entry.Text, text, StringComparison.Ordinal)) { entry.Text = text; Interlocked.Increment(ref _nativePropertyWrites); }
                var readOnly = node.flags?.isReadOnly == true;
                if (entry.IsReadOnly != readOnly) { entry.IsReadOnly = readOnly; Interlocked.Increment(ref _nativePropertyWrites); }
            }
            if ((properties & (SemanticsNodeProperty.label | SemanticsNodeProperty.value | SemanticsNodeProperty.flags)) != 0 && state.Element is Button button)
            {
                var text = node.label ?? node.value ?? string.Empty;
                if (!string.Equals(button.Text, text, StringComparison.Ordinal)) { button.Text = text; Interlocked.Increment(ref _nativePropertyWrites); }
            }
            if ((properties & (SemanticsNodeProperty.label | SemanticsNodeProperty.value)) != 0 && state.Element is Label label)
            {
                var text = node.label ?? node.value ?? string.Empty;
                if (!string.Equals(label.Text, text, StringComparison.Ordinal)) { label.Text = text; Interlocked.Increment(ref _nativePropertyWrites); }
            }
            if ((properties & (SemanticsNodeProperty.label | SemanticsNodeProperty.value | SemanticsNodeProperty.flags |
                               SemanticsNodeProperty.role | SemanticsNodeProperty.metadata)) != 0)
            {
                var description = string.Join(" ", new[] { node.label, node.flags?.isTextField == true ? null : node.value }
                    .Where(value => !string.IsNullOrWhiteSpace(value)));
                if (!string.Equals(state.Description, description, StringComparison.Ordinal))
                {
                    SemanticProperties.SetDescription(state.Element, description);
                    state.Description = description;
                    Interlocked.Increment(ref _nativePropertyWrites);
                }
                var hint = string.Join(" ", new[] { node.hint, node.tooltip }
                    .Where(value => !string.IsNullOrWhiteSpace(value)));
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
            if ((properties & (SemanticsNodeProperty.flags | SemanticsNodeProperty.value |
                               SemanticsNodeProperty.metadata)) != 0)
            {
                if (state.Element is CheckBox checkbox)
                {
                    var isChecked = node.flags?.isChecked is CheckedState.isTrue or CheckedState.mixed;
                    if (checkbox.IsChecked != isChecked) { checkbox.IsChecked = isChecked; Interlocked.Increment(ref _nativePropertyWrites); }
                }
                else if (state.Element is RadioButton radio)
                {
                    var isChecked = node.flags?.isChecked == CheckedState.isTrue || node.flags?.isSelected == Tristate.isTrue;
                    if (radio.IsChecked != isChecked) { radio.IsChecked = isChecked; Interlocked.Increment(ref _nativePropertyWrites); }
                }
                else if (state.Element is Microsoft.Maui.Controls.Switch toggle)
                {
                    var isToggled = node.flags?.isToggled == Tristate.isTrue;
                    if (toggle.IsToggled != isToggled) { toggle.IsToggled = isToggled; Interlocked.Increment(ref _nativePropertyWrites); }
                }
                else if (state.Element is Slider slider)
                {
                    var minimum = TryParseDouble(node.minValue, out var parsedMinimum) ? parsedMinimum : slider.Minimum;
                    var maximum = TryParseDouble(node.maxValue, out var parsedMaximum) ? parsedMaximum : slider.Maximum;
                    if (minimum <= maximum)
                    {
                        if (minimum > slider.Maximum && slider.Maximum != maximum)
                        { slider.Maximum = maximum; Interlocked.Increment(ref _nativePropertyWrites); }
                        if (maximum < slider.Minimum && slider.Minimum != minimum)
                        { slider.Minimum = minimum; Interlocked.Increment(ref _nativePropertyWrites); }
                        if (slider.Minimum != minimum) { slider.Minimum = minimum; Interlocked.Increment(ref _nativePropertyWrites); }
                        if (slider.Maximum != maximum) { slider.Maximum = maximum; Interlocked.Increment(ref _nativePropertyWrites); }
                    }
                    if (TryParseDouble(node.value, out var value))
                    {
                        value = Math.Clamp(value, slider.Minimum, slider.Maximum);
                        if (slider.Value != value) { slider.Value = value; Interlocked.Increment(ref _nativePropertyWrites); }
                    }
                }
            }
            if ((properties & SemanticsNodeProperty.bounds) != 0)
            {
                var bounds = new Microsoft.Maui.Graphics.Rect(node.rect.left, node.rect.top,
                    Math.Max(0, node.rect.right - node.rect.left), Math.Max(0, node.rect.bottom - node.rect.top));
                if (state.LayoutBounds != bounds)
                {
                    AbsoluteLayout.SetLayoutBounds(state.Element, bounds);
                    state.LayoutBounds = bounds;
                    Interlocked.Increment(ref _nativePropertyWrites);
                }
                if (!state.LayoutFlagsInitialized)
                {
                    AbsoluteLayout.SetLayoutFlags(state.Element, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.None);
                    state.LayoutFlagsInitialized = true;
                }
            }
        }
        finally { state.Updating = false; }
    }

    private static NativeElementKind ElementKindFor(SemanticsNodeUpdate node) =>
        node.flags?.isTextField == true ? NativeElementKind.TextField :
        node.flags?.isSlider == true ? NativeElementKind.Slider :
        node.flags?.isToggled != Tristate.none ? NativeElementKind.Toggle :
        node.flags?.isInMutuallyExclusiveGroup == true &&
            (node.flags.isChecked != CheckedState.none || node.flags.isSelected != Tristate.none) ? NativeElementKind.Radio :
        node.flags?.isChecked != CheckedState.none ? NativeElementKind.Checkbox :
        node.actions.HasFlag(SemanticsAction.tap) ? NativeElementKind.Button : NativeElementKind.Label;

    private static bool TryParseDouble(string? value, out double result) =>
        double.TryParse(value, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out result) && double.IsFinite(result);

    private static SemanticHeadingLevel HeadingLevelFor(SemanticsNodeUpdate node) => node.headingLevel switch
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
        SemanticsNodeProperty.bounds | SemanticsNodeProperty.label | SemanticsNodeProperty.value |
        SemanticsNodeProperty.actions | SemanticsNodeProperty.flags | SemanticsNodeProperty.role |
        SemanticsNodeProperty.children | SemanticsNodeProperty.traversal | SemanticsNodeProperty.selection |
        SemanticsNodeProperty.metadata;

    private static void UpdateMaximum(ref long target, long candidate)
    {
        var current = Interlocked.Read(ref target);
        while (candidate > current)
        {
            var observed = Interlocked.CompareExchange(ref target, candidate, current);
            if (observed == current) return;
            current = observed;
        }
    }

    private sealed record PendingUpdate(SemanticsUpdate Update, Action<int, SemanticsAction, object?> PerformAction);
    private enum NativeElementKind { Label, Button, TextField, Checkbox, Radio, Toggle, Slider }
    private sealed class NativeElementState(NativeElementKind kind, View element)
    {
        internal NativeElementKind Kind { get; } = kind;
        internal View Element { get; } = element;
        internal SemanticsNodeUpdate? Node { get; set; }
        internal Action<int, SemanticsAction, object?>? PerformAction { get; set; }
        internal string? Description { get; set; }
        internal string? Hint { get; set; }
        internal SemanticHeadingLevel Heading { get; set; } = SemanticHeadingLevel.None;
        internal Microsoft.Maui.Graphics.Rect? LayoutBounds { get; set; }
        internal bool LayoutFlagsInitialized { get; set; }
        internal bool Updating { get; set; }
    }
}
