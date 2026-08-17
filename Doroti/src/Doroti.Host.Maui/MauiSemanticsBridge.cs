using System.Diagnostics;
using Doroti.Ui;
using Microsoft.Maui.Controls;

namespace Doroti.Host.Maui;

internal sealed class MauiSemanticsBridge(AbsoluteLayout layer) : IMauiSemanticsBridge
{
    private static readonly TimeSpan MinimumApplyInterval = TimeSpan.FromMilliseconds(1000.0 / 15.0);
    private readonly AbsoluteLayout _layer = layer;
    private readonly object _gate = new();
    private readonly Dictionary<int, NativeElementState> _elements = [];
    private SemanticsUpdate? _pendingUpdate;
    private Action<int, SemanticsAction, object?>? _pendingAction;
    private bool _applyScheduled;
    private long _updatesReceived;
    private long _updatesApplied;
    private long _updatesCoalesced;
    private long _elementsCreated;
    private long _activeElements;
    private long _retainedNodes;
    private long _lastApplyTimestamp;

    public MauiSemanticsDiagnostics Diagnostics => new(
        Interlocked.Read(ref _updatesReceived),
        Interlocked.Read(ref _updatesApplied),
        Interlocked.Read(ref _updatesCoalesced),
        Interlocked.Read(ref _elementsCreated),
        Interlocked.Read(ref _activeElements),
        Interlocked.Read(ref _retainedNodes));

    public void Update(SemanticsUpdate update, Action<int, SemanticsAction, object?> performAction)
    {
        ArgumentNullException.ThrowIfNull(update);
        ArgumentNullException.ThrowIfNull(performAction);
        Interlocked.Increment(ref _updatesReceived);

        var schedule = false;
        lock (_gate)
        {
            _pendingUpdate = update;
            _pendingAction = performAction;
            if (_applyScheduled)
            {
                Interlocked.Increment(ref _updatesCoalesced);
            }
            else
            {
                _applyScheduled = true;
                schedule = true;
            }
        }

        if (schedule) ScheduleApply(RemainingApplyDelay());
    }

    private void ApplyLatest()
    {
        SemanticsUpdate? update;
        Action<int, SemanticsAction, object?>? performAction;
        lock (_gate)
        {
            update = _pendingUpdate;
            performAction = _pendingAction;
            _pendingUpdate = null;
            _pendingAction = null;
        }

        if (update is not null && performAction is not null)
        {
            Apply(update, performAction);
            Interlocked.Increment(ref _updatesApplied);
            Interlocked.Exchange(ref _lastApplyTimestamp, Stopwatch.GetTimestamp());
        }

        var scheduleAgain = false;
        lock (_gate)
        {
            if (_pendingUpdate is null)
            {
                _applyScheduled = false;
            }
            else
            {
                scheduleAgain = true;
            }
        }
        if (scheduleAgain) ScheduleApply(RemainingApplyDelay());
    }

    private TimeSpan RemainingApplyDelay()
    {
        var lastApply = Interlocked.Read(ref _lastApplyTimestamp);
        if (lastApply == 0) return TimeSpan.Zero;
        var elapsed = Stopwatch.GetElapsedTime(lastApply);
        return elapsed >= MinimumApplyInterval ? TimeSpan.Zero : MinimumApplyInterval - elapsed;
    }

    private void ScheduleApply(TimeSpan delay)
    {
        if (delay <= TimeSpan.Zero)
        {
            _layer.Dispatcher.Dispatch(ApplyLatest);
            return;
        }
        _ = DelayAndApplyLatest(delay);
    }

    private async Task DelayAndApplyLatest(TimeSpan delay)
    {
        await Task.Delay(delay).ConfigureAwait(false);
        _layer.Dispatcher.Dispatch(ApplyLatest);
    }

    private void Apply(SemanticsUpdate update, Action<int, SemanticsAction, object?> performAction)
    {
        var visibleNodes = update.nodes.Where(node => node.flags?.isHidden != true).ToArray();
        var visibleIds = visibleNodes.Select(node => node.id).ToHashSet();
        var topologyChanged = false;

        foreach (var staleId in _elements.Keys.Where(id => !visibleIds.Contains(id)).ToArray())
        {
            _elements.Remove(staleId);
            topologyChanged = true;
        }

        foreach (var node in visibleNodes)
        {
            var kind = ElementKindFor(node);
            if (!_elements.TryGetValue(node.id, out var state) || state.Kind != kind)
            {
                state = CreateState(kind);
                _elements[node.id] = state;
                Interlocked.Increment(ref _elementsCreated);
                topologyChanged = true;
            }
            UpdateState(state, node, performAction);
        }

        if (topologyChanged)
        {
            _layer.Children.Clear();
            foreach (var node in visibleNodes)
                _layer.Children.Add(_elements[node.id].Element);
        }

        Interlocked.Exchange(ref _activeElements, _elements.Count);
        Interlocked.Exchange(ref _retainedNodes, update.nodes.Count);
        _layer.SetValue(SemanticProperties.DescriptionProperty,
            $"Doroti semantics generation {update.generation}");
    }

    private static NativeElementState CreateState(NativeElementKind kind)
    {
        View element = kind switch
        {
            NativeElementKind.TextField => new Entry { Opacity = 0.01 },
            NativeElementKind.Button => new Button { Opacity = 0.01 },
            _ => new Label { Opacity = 0.01, InputTransparent = true },
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
                {
                    state.PerformAction?.Invoke(node.id, SemanticsAction.setSelection, new Dictionary<string, long>
                    {
                        ["base"] = entry.CursorPosition,
                        ["extent"] = entry.CursorPosition + entry.SelectionLength,
                    });
                }
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

        element.Focused += (_, _) =>
        {
            var node = state.Node;
            if (node is not null && node.actions.HasFlag(SemanticsAction.focus))
                state.PerformAction?.Invoke(node.id, SemanticsAction.focus, null);
        };
        return state;
    }

    private static void UpdateState(
        NativeElementState state,
        SemanticsNodeUpdate node,
        Action<int, SemanticsAction, object?> performAction)
    {
        state.Node = node;
        state.PerformAction = performAction;
        state.Updating = true;
        try
        {
            state.Element.InputTransparent = PassThroughOrdinaryTouch();
            switch (state.Element)
            {
                case Entry entry:
                    var entryText = node.value ?? string.Empty;
                    if (!string.Equals(entry.Text, entryText, StringComparison.Ordinal)) entry.Text = entryText;
                    entry.IsReadOnly = node.flags?.isReadOnly == true;
                    break;
                case Button button:
                    var buttonText = node.label ?? node.value ?? string.Empty;
                    if (!string.Equals(button.Text, buttonText, StringComparison.Ordinal)) button.Text = buttonText;
                    break;
                case Label label:
                    var labelText = node.label ?? node.value ?? string.Empty;
                    if (!string.Equals(label.Text, labelText, StringComparison.Ordinal)) label.Text = labelText;
                    break;
            }

            SemanticProperties.SetDescription(state.Element, string.Join(" ",
                new[] { node.label, node.value }.Where(value => !string.IsNullOrWhiteSpace(value))));
            if (node.flags?.isHeader == true)
                SemanticProperties.SetHeadingLevel(state.Element, SemanticHeadingLevel.Level1);
            AbsoluteLayout.SetLayoutBounds(state.Element, new Microsoft.Maui.Graphics.Rect(
                node.rect.left,
                node.rect.top,
                Math.Max(0, node.rect.right - node.rect.left),
                Math.Max(0, node.rect.bottom - node.rect.top)));
            AbsoluteLayout.SetLayoutFlags(state.Element, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.None);
        }
        finally
        {
            state.Updating = false;
        }
    }

    private static bool PassThroughOrdinaryTouch()
    {
#if ANDROID
        var manager = Android.App.Application.Context.GetSystemService(Android.Content.Context.AccessibilityService)
            as Android.Views.Accessibility.AccessibilityManager;
        return manager?.IsTouchExplorationEnabled != true;
#else
        return false;
#endif
    }

    private static NativeElementKind ElementKindFor(SemanticsNodeUpdate node) =>
        node.flags?.isTextField == true
            ? NativeElementKind.TextField
            : node.actions.HasFlag(SemanticsAction.tap)
                ? NativeElementKind.Button
                : NativeElementKind.Label;

    private enum NativeElementKind { Label, Button, TextField }

    private sealed class NativeElementState(NativeElementKind kind, View element)
    {
        internal NativeElementKind Kind { get; } = kind;
        internal View Element { get; } = element;
        internal SemanticsNodeUpdate? Node { get; set; }
        internal Action<int, SemanticsAction, object?>? PerformAction { get; set; }
        internal bool Updating { get; set; }
    }
}
