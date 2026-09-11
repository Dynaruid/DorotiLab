#if ANDROID
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.View;
using AndroidX.Core.View.Accessibility;
using AndroidX.CustomView.Widget;
using Doroti.Ui;
using NativeAction = Android.Views.Accessibility.Action;

namespace Doroti.Host.Maui;

/// <summary>Exposes retained semantics as virtual Android nodes, without hidden native controls.</summary>
internal sealed partial class MauiAndroidSemanticsBridge : IMauiSemanticsBridge, IDisposable
{
    private readonly View _element;
    private DorotiAndroidVulkanView? _native;
    private VirtualNodes? _helper;
    private Dictionary<int, SemanticsNodeUpdate> _nodes = [];
    private int[] _order = [];
    private Action<int, SemanticsAction, object?>? _performAction;
    private long _generation = -1, _received, _applied, _suppressed, _applyMicros, _maxApplyMicros;
    private bool _disposed;

    internal MauiAndroidSemanticsBridge(View element)
    {
        _element = element;
        element.HandlerChanged += Attach;
        Attach(null, EventArgs.Empty);
    }

    public MauiSemanticsDiagnostics Diagnostics => new(_received, _applied, 0, 0, 0, _nodes.Count,
        UpdatesSuppressed: _suppressed, ApplyWorkMicroseconds: _applyMicros, MaxApplyWorkMicroseconds: _maxApplyMicros);

    public void AttachFrameTrace(DorotiFrameTrace trace, ulong viewId) { }

    public void Update(SemanticsUpdate update, Action<int, SemanticsAction, object?> performAction)
    {
        if (_disposed || update.generation < _generation) return;
        // Framework dispatch, provider callbacks and SurfaceView input share the UI thread.
        if (_element.Dispatcher.IsDispatchRequired)
        {
            _element.Dispatcher.Dispatch(() => Update(update, performAction));
            return;
        }
        _received++;
        _generation = update.generation;
        _performAction = performAction;
        var started = Stopwatch.GetTimestamp();
        var nodes = update.nodes.ToDictionary(node => node.id);
        var hidden = new HashSet<int>();
        var pending = new Stack<int>(update.nodes.Where(node => node.flags?.isHidden == true).Select(node => node.id));
        while (pending.TryPop(out var id))
        {
            if (!hidden.Add(id) || !nodes.TryGetValue(id, out var node)) continue;
            foreach (var child in node.children) pending.Push(child);
        }
        foreach (var id in hidden) nodes.Remove(id);
        var visible = update.nodes.Where(node => nodes.ContainsKey(node.id)).ToArray();
        if (!SemanticsUpdateDiffer.Diff(_nodes, visible).HasChanges) { _suppressed++; return; }
        ClearRemovedFocus(nodes);
        _nodes = nodes;
        _order = visible.Select(node => node.id).ToArray();
        _applied++;
        // Android's helper owns accessibility/keyboard focus and emits one tree
        // change. NodeInfo objects are created only when a service queries them.
        _helper?.InvalidateRoot();
        var micros = Stopwatch.GetElapsedTime(started).Ticks / 10;
        _applyMicros += micros;
        _maxApplyMicros = Math.Max(_maxApplyMicros, micros);
    }

    public void Clear()
    {
        ClearRemovedFocus(new Dictionary<int, SemanticsNodeUpdate>());
        _nodes.Clear(); _order = []; _performAction = null; _generation = -1;
        _helper?.InvalidateRoot();
    }

    private void ClearRemovedFocus(IReadOnlyDictionary<int, SemanticsNodeUpdate> nodes)
    {
        if (_helper is null || _native is null) return;
        var keyboard = _helper.KeyboardFocusedVirtualViewId;
        if (keyboard != ExploreByTouchHelper.InvalidId && !nodes.ContainsKey(keyboard))
            _helper.ClearKeyboardFocusForVirtualView(keyboard);
        var accessibility = _helper.AccessibilityFocusedVirtualViewId;
        if (accessibility != ExploreByTouchHelper.InvalidId && !nodes.ContainsKey(accessibility))
            _helper.GetAccessibilityNodeProvider(_native)?.PerformAction(accessibility, (int)NativeAction.ClearAccessibilityFocus, null);
    }

    private void Attach(object? sender, EventArgs args)
    {
        if (_disposed || ReferenceEquals(_native, _element.Handler?.PlatformView)) return;
        Detach();
        if (_element.Handler?.PlatformView is not DorotiAndroidVulkanView native) return;
        _native = native;
        _helper = new VirtualNodes(native, this);
        native.SemanticsHelper = _helper;
        ViewCompat.SetAccessibilityDelegate(native, _helper);
    }

    private void Detach()
    {
        if (_native is not null)
        {
            _native.SemanticsHelper = null;
            ViewCompat.SetAccessibilityDelegate(_native, null);
        }
        _helper?.Dispose(); _helper = null; _native = null;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _element.HandlerChanged -= Attach;
        Detach(); Clear();
    }

    private sealed class VirtualNodes : ExploreByTouchHelper
    {
        private readonly MauiAndroidSemanticsBridge? _owner;
        private readonly DorotiAndroidVulkanView? _host;

        [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(VirtualNodes))]
        internal VirtualNodes(DorotiAndroidVulkanView host, MauiAndroidSemanticsBridge owner) : base(host)
        { _host = host; _owner = owner; }
        private VirtualNodes(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership) { }

        protected override int GetVirtualViewAt(float x, float y)
        {
            if (_owner is null || _host is null) return InvalidId;
            var scale = _host.SemanticsDensity;
            var point = new Offset(x / scale, y / scale);
            // Flat view-space projection: prefer the smallest containing node,
            // so an actionable child wins over its scroll/container bounds.
            var best = InvalidId;
            var area = double.PositiveInfinity;
            foreach (var id in _owner._order)
            {
                var node = _owner._nodes[id];
                if (!IsExposed(node)) continue;
                if (node.rect.contains(point) && node.rect.width * node.rect.height < area)
                { best = id; area = node.rect.width * node.rect.height; }
            }
            return best;
        }

        protected override void GetVisibleVirtualViews(IList<Java.Lang.Integer>? virtualViewIds)
        {
            if (_owner is null || virtualViewIds is null) return;
            foreach (var id in _owner._order)
            {
                if (!IsExposed(_owner._nodes[id])) continue;
                using var value = Java.Lang.Integer.ValueOf(id)!;
                virtualViewIds.Add(value);
            }
        }

        private static bool IsExposed(SemanticsNodeUpdate node) =>
            !string.IsNullOrWhiteSpace(node.label) || !string.IsNullOrWhiteSpace(node.value) ||
            node.actions != SemanticsAction.none || node.flags is { isFocused: not Tristate.none };

        protected override void OnPopulateNodeForVirtualView(int virtualViewId, AccessibilityNodeInfoCompat? info)
        {
            if (info is null) return;
            if (_owner is null || _host is null || !_owner._nodes.TryGetValue(virtualViewId, out var node))
            {
                info.ContentDescription = string.Empty;
                using var empty = new Android.Graphics.Rect(); SetBoundsInScreenFromBoundsInParent(info, empty);
                return;
            }
            var flags = node.flags;
            // Establish virtual identity before selection: Android selection
            // anchors refer to the source node, which the helper also sets later.
            info.SetSource(_host, virtualViewId);
            var textField = flags?.isTextField == true;
            var toggled = flags is { isToggled: not Tristate.none };
            var checkable = toggled || flags is { isChecked: not CheckedState.none };
            info.ClassName = textField ? "android.widget.EditText" : flags?.isSlider == true ? "android.widget.SeekBar" :
                flags?.isInMutuallyExclusiveGroup == true ? "android.widget.RadioButton" : toggled ? "android.widget.Switch" :
                checkable ? "android.widget.CheckBox" : node.actions.HasFlag(SemanticsAction.tap) ? "android.widget.Button" : "android.view.View";
            info.Text = textField ? node.value ?? string.Empty : node.label ?? string.Empty;
            info.ContentDescription = textField ? node.label ?? string.Empty :
                string.Join(" ", new[] { node.label, node.value }.Where(value => !string.IsNullOrWhiteSpace(value)));
            info.HintText = node.hint;
            info.TooltipText = node.tooltip;
            info.Enabled = flags?.isEnabled != Tristate.isFalse;
            info.Focusable = flags?.isAccessibilityFocusBlocked != true;
            info.Focused = flags?.isFocused == Tristate.isTrue;
            info.Checkable = checkable;
            info.Checked = toggled ? flags?.isToggled == Tristate.isTrue : flags?.isChecked is CheckedState.isTrue or CheckedState.mixed;
            info.Selected = flags?.isSelected == Tristate.isTrue;
            info.Password = flags?.isObscured == true;
            info.Editable = textField && flags?.isReadOnly != true;
            info.Heading = flags?.isHeader == true || node.headingLevel is > 0;
            info.LiveRegion = flags?.isLiveRegion == true ? 1 : 0;
            info.Clickable = node.actions.HasFlag(SemanticsAction.tap);
            info.LongClickable = node.actions.HasFlag(SemanticsAction.longPress);
            info.Scrollable = (node.actions & (SemanticsAction.scrollUp | SemanticsAction.scrollDown | SemanticsAction.scrollLeft | SemanticsAction.scrollRight)) != 0;
            info.MovementGranularities =
                ((node.actions & (SemanticsAction.moveCursorForwardByCharacter | SemanticsAction.moveCursorBackwardByCharacter)) != 0 ? 1 : 0) |
                ((node.actions & (SemanticsAction.moveCursorForwardByWord | SemanticsAction.moveCursorBackwardByWord)) != 0 ? 2 : 0);
            var scale = _host.SemanticsDensity;
            using var bounds = new Android.Graphics.Rect((int)Math.Floor(node.rect.left * scale), (int)Math.Floor(node.rect.top * scale),
                (int)Math.Ceiling(node.rect.right * scale), (int)Math.Ceiling(node.rect.bottom * scale));
            SetBoundsInScreenFromBoundsInParent(info, bounds);
            if (textField && node.textSelectionBase >= 0 && node.textSelectionExtent >= 0)
            {
                var length = (node.value ?? string.Empty).Length;
                info.SetTextSelection((int)Math.Clamp(node.textSelectionBase, 0, length), (int)Math.Clamp(node.textSelectionExtent, 0, length));
            }
            if (flags?.isSlider == true && float.TryParse(node.minValue, CultureInfo.InvariantCulture, out var min) &&
                float.TryParse(node.maxValue, CultureInfo.InvariantCulture, out var max) && max >= min &&
                float.TryParse(node.value, CultureInfo.InvariantCulture, out var current) && float.IsFinite(min) && float.IsFinite(max) && float.IsFinite(current))
            {
                using var range = AccessibilityNodeInfoCompat.RangeInfoCompat.Obtain(1, min, max, Math.Clamp(current, min, max));
                info.RangeInfo = range;
            }
            foreach (var (native, action) in Actions)
                if ((node.actions & action) != 0)
                {
                    if ((int)native <= (int)NativeAction.SetText) info.AddAction((int)native);
                    else
                    {
                        using var custom = new AccessibilityNodeInfoCompat.AccessibilityActionCompat((int)native, (string?)null);
                        info.AddAction(custom);
                    }
                }
        }

        protected override bool OnPerformActionForVirtualView(int virtualViewId, int action, Bundle? arguments)
        {
            if (_owner is null || !_owner._nodes.TryGetValue(virtualViewId, out var node) || node.flags?.isEnabled == Tristate.isFalse) return false;
            foreach (var (native, candidates) in Actions)
            {
                if ((int)native != action) continue;
                var available = candidates & node.actions;
                if (available == 0) return false;
                // A range action takes precedence over scrolling, then choose
                // the supported axis. Never dispatch a combined action mask.
                var chosen = available.HasFlag(SemanticsAction.increase) ? SemanticsAction.increase :
                    available.HasFlag(SemanticsAction.decrease) ? SemanticsAction.decrease :
                    (SemanticsAction)((long)available & -(long)available);
                object? value = chosen == SemanticsAction.setText ? arguments?.GetCharSequence("ACTION_ARGUMENT_SET_TEXT_CHARSEQUENCE") ?? string.Empty : null;
                if (native is NativeAction.NextAtMovementGranularity or NativeAction.PreviousAtMovementGranularity)
                {
                    var forward = native == NativeAction.NextAtMovementGranularity;
                    chosen = arguments?.GetInt("ACTION_ARGUMENT_MOVEMENT_GRANULARITY_INT", 1) switch
                    {
                        null or 1 => forward ? SemanticsAction.moveCursorForwardByCharacter : SemanticsAction.moveCursorBackwardByCharacter,
                        2 => forward ? SemanticsAction.moveCursorForwardByWord : SemanticsAction.moveCursorBackwardByWord,
                        _ => SemanticsAction.none,
                    };
                    if (chosen == SemanticsAction.none || !node.actions.HasFlag(chosen)) return false;
                    value = arguments?.GetBoolean("ACTION_ARGUMENT_EXTEND_SELECTION_BOOLEAN", false) ?? false;
                }
                if (chosen == SemanticsAction.setSelection)
                    value = new Dictionary<string, long> { ["base"] = arguments?.GetInt("ACTION_ARGUMENT_SELECTION_START_INT", -1) ?? -1,
                        ["extent"] = arguments?.GetInt("ACTION_ARGUMENT_SELECTION_END_INT", -1) ?? -1 };
                _owner._performAction?.Invoke(virtualViewId, chosen, value);
                return true;
            }
            return false;
        }

        protected override void OnPopulateEventForVirtualView(int virtualViewId, Android.Views.Accessibility.AccessibilityEvent? e)
        {
            base.OnPopulateEventForVirtualView(virtualViewId, e);
            var action = e?.EventType switch
            {
                Android.Views.Accessibility.EventTypes.ViewAccessibilityFocused => SemanticsAction.didGainAccessibilityFocus,
                Android.Views.Accessibility.EventTypes.ViewAccessibilityFocusCleared => SemanticsAction.didLoseAccessibilityFocus,
                _ => SemanticsAction.none,
            };
            if (action != SemanticsAction.none && _owner?._nodes.TryGetValue(virtualViewId, out var node) == true && node.actions.HasFlag(action))
                _owner._performAction?.Invoke(virtualViewId, action, null);
        }

        protected override void OnVirtualViewKeyboardFocusChanged(int virtualViewId, bool hasFocus)
        {
            base.OnVirtualViewKeyboardFocusChanged(virtualViewId, hasFocus);
            if (hasFocus && _owner?._nodes.TryGetValue(virtualViewId, out var node) == true && node.actions.HasFlag(SemanticsAction.focus))
                _owner._performAction?.Invoke(virtualViewId, SemanticsAction.focus, null);
        }

        private static readonly (NativeAction Native, SemanticsAction Action)[] Actions = [
            (NativeAction.Click, SemanticsAction.tap), (NativeAction.LongClick, SemanticsAction.longPress),
            (NativeAction.ScrollForward, SemanticsAction.increase | SemanticsAction.scrollUp | SemanticsAction.scrollLeft),
            (NativeAction.ScrollBackward, SemanticsAction.decrease | SemanticsAction.scrollDown | SemanticsAction.scrollRight),
            (NativeAction.SetText, SemanticsAction.setText), (NativeAction.SetSelection, SemanticsAction.setSelection),
            (NativeAction.NextAtMovementGranularity, SemanticsAction.moveCursorForwardByCharacter | SemanticsAction.moveCursorForwardByWord),
            (NativeAction.PreviousAtMovementGranularity, SemanticsAction.moveCursorBackwardByCharacter | SemanticsAction.moveCursorBackwardByWord),
            (NativeAction.Copy, SemanticsAction.copy), (NativeAction.Cut, SemanticsAction.cut), (NativeAction.Paste, SemanticsAction.paste),
            (NativeAction.Expand, SemanticsAction.expand), (NativeAction.Collapse, SemanticsAction.collapse),
            (NativeAction.Dismiss, SemanticsAction.dismiss), ((NativeAction)16908342, SemanticsAction.showOnScreen)];
    }
}
#endif
