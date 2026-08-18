// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/raw_tooltip.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public static partial class Raw_tooltipLibrary
{
    internal static global::Doroti.Framework.Animation.AnimationStyle _kDefaultAnimationStyle = new global::Doroti.Framework.Animation.AnimationStyle(curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, duration: Duration.Create(milliseconds: 150L), reverseDuration: Duration.Create(milliseconds: 75L));
}

public delegate Widget TooltipComponentBuilder(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation);

public delegate Offset TooltipPositionDelegate(TooltipPositionContext context);

public class TooltipPositionContext
{
    public virtual Offset target { get; private set; } = default!;
    public virtual Size targetSize { get; private set; } = default!;
    public virtual Size tooltipSize { get; private set; } = default!;
    public virtual double verticalOffset { get; private set; } = default!;
    public virtual bool preferBelow { get; private set; } = default!;
    public virtual Size overlaySize { get; private set; } = default!;

    public TooltipPositionContext(Offset target, Size targetSize, Size tooltipSize, double verticalOffset, bool preferBelow = true, Size? overlaySize = null)
    {
        Size __overlaySize = overlaySize ?? Size.infinite;
        this.target = target;
        this.targetSize = targetSize;
        this.tooltipSize = tooltipSize;
        this.verticalOffset = verticalOffset;
        this.preferBelow = preferBelow;
        this.overlaySize = __overlaySize;
    }

    public override bool Equals(object? other)
    {
        var __other = other as TooltipPositionContext;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((__other is TooltipPositionContext) && (object.Equals(((TooltipPositionContext)((TooltipPositionContext)__other)).target, this.target))) && (object.Equals(((TooltipPositionContext)((TooltipPositionContext)__other)).targetSize, this.targetSize))) && (object.Equals(((TooltipPositionContext)((TooltipPositionContext)__other)).tooltipSize, this.tooltipSize))) && (object.Equals(((TooltipPositionContext)((TooltipPositionContext)__other)).overlaySize, this.overlaySize))) && (((TooltipPositionContext)((TooltipPositionContext)__other)).verticalOffset == this.verticalOffset)) && (((TooltipPositionContext)((TooltipPositionContext)__other)).preferBelow == this.preferBelow));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.target, this.targetSize, this.tooltipSize, DartRuntimePrimitives.RequireValue(this.overlaySize), this.verticalOffset, this.preferBelow));
}

public enum TooltipTriggerMode
{
    manual,
    longPress,
    tap
}

public delegate void TooltipTriggeredCallback();

internal class _ExclusiveMouseRegion__raw_tooltip : MouseRegion
{
    internal _ExclusiveMouseRegion__raw_tooltip(global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>? onEnter = null, global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>? onExit = null, Widget? child = null) : base(onEnter: onEnter, onExit: onExit, child: child)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderExclusiveMouseRegion__raw_tooltip(onEnter: (global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>?)this.onEnter, onExit: (global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>?)this.onExit));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _RenderExclusiveMouseRegion__raw_tooltip : global::Doroti.Framework.Rendering.RenderMouseRegion
{
    public static bool isOutermostMouseRegion = true;
    public static bool foundInnermostMouseRegion = false;

    internal _RenderExclusiveMouseRegion__raw_tooltip(global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>? onEnter = null, global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>? onExit = null) : base(onEnter: onEnter, onExit: onExit)
    {
    }

    public override bool hitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        var isHit__6599 = false;
        bool outermost__6629 = isOutermostMouseRegion;
        isOutermostMouseRegion = false;
        if (this.size.contains(position))
        {
            isHit__6599 = (hitTestChildren(result, position: position) || hitTestSelf(position));
            if ((((isHit__6599 || (object.Equals(this.behavior, global::Doroti.Framework.Rendering.HitTestBehavior.translucent)))) && !foundInnermostMouseRegion))
            {
                foundInnermostMouseRegion = true;
                result.add(new global::Doroti.Framework.Rendering.BoxHitTestEntry(this, position));
            }
        }
        if (outermost__6629)
        {
            isOutermostMouseRegion = true;
            foundInnermostMouseRegion = false;
        }
        return isHit__6599;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RawTooltip : StatefulWidget
{
    public virtual string? semanticsTooltip { get; private set; }
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> tooltipBuilder { get; private set; } = default!;
    public virtual Duration hoverDelay { get; private set; } = default!;
    public virtual Duration touchDelay { get; private set; } = default!;
    public virtual Duration dismissDelay { get; private set; } = default!;
    public virtual bool enableTapToDismiss { get; private set; } = default!;
    public virtual TooltipTriggerMode triggerMode { get; private set; } = default!;
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual global::System.Action? onTriggered { get; private set; }
    public virtual global::Doroti.Framework.Animation.AnimationStyle animationStyle { get; private set; } = default!;
    public virtual global::System.Func<TooltipPositionContext, Offset>? positionDelegate { get; private set; }
    public virtual bool ignorePointer { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    internal static List<RawTooltipState> _openedTooltips = new List<RawTooltipState>();

    public RawTooltip(global::Doroti.Framework.Foundation.Key? key = null, string? semanticsTooltip = default!, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> tooltipBuilder = default!, Duration hoverDelay = default, Duration? touchDelay = null, Duration? dismissDelay = null, bool enableTapToDismiss = true, TooltipTriggerMode triggerMode = TooltipTriggerMode.longPress, bool enableFeedback = true, global::System.Action? onTriggered = null, global::Doroti.Framework.Animation.AnimationStyle animationStyle = default!, global::System.Func<TooltipPositionContext, Offset>? positionDelegate = null, bool ignorePointer = false, Widget child = default!) : base(key: key)
    {
        Duration __touchDelay = touchDelay ?? Duration.Create(milliseconds: 1500);
        Duration __dismissDelay = dismissDelay ?? Duration.Create(milliseconds: 100);
        global::Doroti.Framework.Animation.AnimationStyle __animationStyle = animationStyle ?? Raw_tooltipLibrary._kDefaultAnimationStyle;
        this.semanticsTooltip = semanticsTooltip;
        this.tooltipBuilder = tooltipBuilder;
        this.hoverDelay = hoverDelay;
        this.touchDelay = __touchDelay;
        this.dismissDelay = __dismissDelay;
        this.enableTapToDismiss = enableTapToDismiss;
        this.triggerMode = triggerMode;
        this.enableFeedback = enableFeedback;
        this.onTriggered = onTriggered;
        this.animationStyle = __animationStyle;
        this.positionDelegate = positionDelegate;
        this.ignorePointer = ignorePointer;
        this.child = child;
    }

    public static bool dismissAllToolTips()
    {
        if (!System.Linq.Enumerable.Any(_openedTooltips))
        {
            return false;
        }
        List<RawTooltipState> openedTooltips__17670 = _openedTooltips.ToList().ToList();
        foreach (var state__17728 in openedTooltips__17670)
        {
            DartRuntimePrimitives.Assert(() => state__17728.mounted);
            state__17728._scheduleDismissTooltip();
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new RawTooltipState());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("semantics", this.semanticsTooltip, showName: ((this.semanticsTooltip is null) || (this.semanticsTooltip!.Length == 0)), defaultValue: (((this.semanticsTooltip is null) || (this.semanticsTooltip!.Length == 0)) ? null : global::Doroti.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue.ToString())));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("hover delay", this.hoverDelay, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("touch delay", DartRuntimePrimitives.RequireValue(this.touchDelay), defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("dismiss delay", DartRuntimePrimitives.RequireValue(this.dismissDelay), defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TooltipTriggerMode>("triggerMode", this.triggerMode, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enableFeedback", value: this.enableFeedback, ifTrue: "true", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::System.Func<TooltipPositionContext, Offset>>("positionDelegate", this.positionDelegate, defaultValue: null));
    }

}

public class RawTooltipState : State<RawTooltip>, SingleTickerProviderStateMixin<RawTooltip>
{
    internal virtual OverlayPortalController _overlayController { get; private set; } = new OverlayPortalController();
    internal virtual Timer? _timer { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.AnimationController? _backingController { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _backingOverlayAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Gestures.LongPressGestureRecognizer? _longPressRecognizer { get; set; } = default;
    internal virtual global::Doroti.Framework.Gestures.TapGestureRecognizer? _tapRecognizer { get; set; } = default;
    internal virtual HashSet<long> _activeHoveringPointerDevices { get; private set; } = new HashSet<long>();
    internal virtual global::Doroti.Framework.Animation.AnimationStatus _animationStatus { get; set; } = global::Doroti.Framework.Animation.AnimationStatus.dismissed;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual global::Doroti.Framework.Animation.AnimationController _controller
    {
        get
        {
            return _backingController ??= ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: ((RawTooltip)this.widget).animationStyle.duration, reverseDuration: ((RawTooltip)this.widget).animationStyle.reverseDuration, vsync: this);
    __cascade.addStatusListener((AnimationStatusListener)this._handleStatusChanged);
    return __cascade;
}))();
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _overlayAnimation
    {
        get
        {
            return _backingOverlayAnimation ??= new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._controller, curve: (((RawTooltip)this.widget).animationStyle.curve ?? ((global::Doroti.Framework.Animation.AnimationStyle)Raw_tooltipLibrary._kDefaultAnimationStyle).curve!));
            return default!;
        }
    }
    internal virtual void _handleStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        switch ((global::Doroti.Framework.Animation.AnimationStatusMembers.isDismissed(this._animationStatus), global::Doroti.Framework.Animation.AnimationStatusMembers.isDismissed(status)))
        {
            case (false, true):
                {
                    RawTooltip._openedTooltips.Remove(this);
                    this._overlayController.hide();
                    break;
                }
            case (true, false):
                {
                    this._overlayController.show();
                    RawTooltip._openedTooltips.Add(this);
                    DartRuntimePrimitives.Ignore(SemanticsService.tooltip((((RawTooltip)this.widget).semanticsTooltip ?? "")));
                    break;
                }
            case (true, true) or (false, false):
                {
                    break;
                }
        }
        _animationStatus = status;
    }

    internal virtual void _scheduleShowTooltip(Duration withDelay, Duration? touchDelay = null)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        void show()
        {
            DartRuntimePrimitives.Assert(() => this.mounted);
            this._controller.forward();
            this._timer?.cancel();
            _timer = ((touchDelay is null) ? null : new Timer(DartRuntimePrimitives.RequireValue(touchDelay), () => { _ = ((global::System.Func<double?, global::Doroti.Framework.Scheduler.TickerFuture>)((global::Doroti.Framework.Animation.AnimationController)this._controller).reverse)(default); }));
        }
        DartRuntimePrimitives.Assert(() => (!((this._timer?.isActive ?? false)) || (!object.Equals(((global::Doroti.Framework.Animation.AnimationController)this._controller).status, global::Doroti.Framework.Animation.AnimationStatus.reverse))), () => (object?)"timer must not be active when the tooltip is animating out");
        if ((this._controller.isDismissed && (withDelay.inMicroseconds > 0L)))
        {
            this._timer?.cancel();
            _timer = new Timer(withDelay, show);
        }
        else
        {
            show();
        }
    }

    internal virtual void _scheduleDismissTooltip(Duration withDelay = default)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        DartRuntimePrimitives.Assert(() => (!((this._timer?.isActive ?? false)) || (!object.Equals(this._backingController?.status, global::Doroti.Framework.Animation.AnimationStatus.reverse))), () => (object?)"timer must not be active when the tooltip is animating out");
        this._timer?.cancel();
        _timer = null;
        if ((this._backingController?.isForwardOrCompleted ?? false))
        {
            if ((withDelay.inMicroseconds > 0L))
            {
                _timer = new Timer(withDelay, () => { _ = ((global::System.Func<double?, global::Doroti.Framework.Scheduler.TickerFuture>)((global::Doroti.Framework.Animation.AnimationController)this._controller).reverse)(default); });
            }
            else
            {
                this._controller.reverse();
            }
        }
    }

    internal virtual void _handlePointerDown(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        var triggerModeDeviceKinds__22757 = new HashSet<PointerDeviceKind> { PointerDeviceKind.invertedStylus, PointerDeviceKind.stylus, PointerDeviceKind.touch, PointerDeviceKind.unknown, PointerDeviceKind.trackpad };
        switch (((RawTooltip)this.widget).triggerMode)
        {
            case TooltipTriggerMode.longPress:
                {
                    global::Doroti.Framework.Gestures.LongPressGestureRecognizer recognizer__23157 = _longPressRecognizer ??= new global::Doroti.Framework.Gestures.LongPressGestureRecognizer(debugOwner: this, supportedDevices: triggerModeDeviceKinds__22757);
                    DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)(() =>
{
    var __cascade = recognizer__23157;
    __cascade.onLongPressCancel = this._handleTapToDismiss;
    __cascade.onLongPress = this._handleLongPress;
    __cascade.onLongPressUp = this._handlePressUp;
    __cascade.addPointer(@event);
    return __cascade;
}))());
                    break;
                }
            case TooltipTriggerMode.tap:
                {
                    global::Doroti.Framework.Gestures.TapGestureRecognizer recognizer__23553 = _tapRecognizer ??= new global::Doroti.Framework.Gestures.TapGestureRecognizer(debugOwner: this, supportedDevices: triggerModeDeviceKinds__22757);
                    DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() =>
{
    var __cascade = recognizer__23553;
    __cascade.onTapCancel = this._handleTapToDismiss;
    __cascade.onTap = this._handleTap;
    __cascade.addPointer(@event);
    return __cascade;
}))());
                    break;
                }
            case TooltipTriggerMode.manual:
                {
                    break;
                }
        }
    }

    internal virtual void _handleGlobalPointerEvent(global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        if (((this._tapRecognizer?.primaryPointer == ((global::Doroti.Framework.Gestures.PointerEvent)@event).pointer) || (this._longPressRecognizer?.primaryPointer == ((global::Doroti.Framework.Gestures.PointerEvent)@event).pointer)))
        {
            return;
        }
        if (((((this._timer is null) && this._controller.isDismissed)) || (@event is not global::Doroti.Framework.Gestures.PointerDownEvent)))
        {
            return;
        }
        _handleTapToDismiss();
    }

    internal virtual void _handleTapToDismiss()
    {
        if (!((RawTooltip)this.widget).enableTapToDismiss)
        {
            return;
        }
        _scheduleDismissTooltip();
        this._activeHoveringPointerDevices.Clear();
    }

    internal virtual void _handleTap()
    {
        bool tooltipCreated__25138 = this._controller.isDismissed;
        if ((tooltipCreated__25138 && ((RawTooltip)this.widget).enableFeedback))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(((RawTooltip)this.widget).triggerMode, TooltipTriggerMode.tap)));
            DartRuntimePrimitives.Ignore(Feedback.forTap(this.context));
        }
        ((RawTooltip)this.widget).onTriggered?.Invoke();
        _scheduleShowTooltip(withDelay: Duration.zero, touchDelay: (!System.Linq.Enumerable.Any(this._activeHoveringPointerDevices) ? ((RawTooltip)this.widget).touchDelay : null));
    }

    internal virtual void _handleLongPress()
    {
        bool tooltipCreated__25715 = this._controller.isDismissed;
        if ((tooltipCreated__25715 && ((RawTooltip)this.widget).enableFeedback))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(((RawTooltip)this.widget).triggerMode, TooltipTriggerMode.longPress)));
            DartRuntimePrimitives.Ignore(Feedback.forLongPress(this.context));
        }
        ((RawTooltip)this.widget).onTriggered?.Invoke();
        _scheduleShowTooltip(withDelay: Duration.zero);
    }

    internal virtual void _handlePressUp()
    {
        if (System.Linq.Enumerable.Any(this._activeHoveringPointerDevices))
        {
            return;
        }
        _scheduleDismissTooltip(withDelay: DartRuntimePrimitives.RequireValue(((RawTooltip)this.widget).touchDelay));
    }

    internal virtual void _handleMouseEnter(global::Doroti.Framework.Gestures.PointerEnterEvent @event)
    {
        this._activeHoveringPointerDevices.Add(@event.device);
        List<RawTooltipState> tooltipsToDismiss__27539 = RawTooltip._openedTooltips.where(((tooltip) => !System.Linq.Enumerable.Any(((RawTooltipState)tooltip)._activeHoveringPointerDevices))).ToList().ToList();
        foreach (var tooltip__27711 in tooltipsToDismiss__27539)
        {
            DartRuntimePrimitives.Assert(() => tooltip__27711.mounted);
            tooltip__27711._scheduleDismissTooltip();
        }
        _scheduleShowTooltip(withDelay: (System.Linq.Enumerable.Any(tooltipsToDismiss__27539) ? Duration.zero : ((RawTooltip)this.widget).hoverDelay));
    }

    internal virtual void _handleMouseExit(global::Doroti.Framework.Gestures.PointerExitEvent @event)
    {
        if (!System.Linq.Enumerable.Any(this._activeHoveringPointerDevices))
        {
            return;
        }
        this._activeHoveringPointerDevices.Remove(@event.device);
        if (!System.Linq.Enumerable.Any(this._activeHoveringPointerDevices))
        {
            _scheduleDismissTooltip(withDelay: ((RawTooltip)this.widget).dismissDelay);
        }
    }

    public virtual bool ensureTooltipVisible()
    {
        this._timer?.cancel();
        _timer = null;
        if (this._controller.isForwardOrCompleted)
        {
            return false;
        }
        _scheduleShowTooltip(withDelay: Duration.zero);
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        base.initState();
        global::Doroti.Framework.Gestures.GestureBinding.instance.pointerRouter.addGlobalRoute((global::System.Action<global::Doroti.Framework.Gestures.PointerEvent>)this._handleGlobalPointerEvent);
    }

    internal virtual Widget _buildTooltipOverlay(BuildContext context, OverlayChildLayoutInfo layoutInfo)
    {
        if ((((OverlayChildLayoutInfo)layoutInfo).childPaintTransform.determinant == 0.0))
        {
            return ((Widget)(object?)SizedBox.CreateShrink());
        }
        global::Doroti.Ui.Offset target__29543 = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(((OverlayChildLayoutInfo)layoutInfo).childPaintTransform, ((OverlayChildLayoutInfo)layoutInfo).childSize.center(Offset.zero)));
        Widget tooltip__29759 = ((Widget)(object?)new IgnorePointer(ignoring: ((RawTooltip)this.widget).ignorePointer, child: new _ExclusiveMouseRegion__raw_tooltip(onEnter: (global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>)this._handleMouseEnter, onExit: (global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>)this._handleMouseExit, child: this.widget.tooltipBuilder(context, this._overlayAnimation))));
        Widget overlayChild__30028 = ((Widget)(object?)Positioned.CreateFill(bottom: (MediaQuery.maybeViewInsetsOf(context)?.bottom ?? 0.0), child: new CustomSingleChildLayout(@delegate: new _TooltipPositionDelegate__raw_tooltip(target: target__29543, targetSize: ((OverlayChildLayoutInfo)layoutInfo).childSize, positionDelegate: (global::System.Func<TooltipPositionContext, Offset>?)((RawTooltip)this.widget).positionDelegate), child: tooltip__29759)));
        return ((SelectionContainer.maybeOf(context) is null) ? overlayChild__30028 : SelectionContainer.CreateDisabled(child: overlayChild__30028));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        global::Doroti.Framework.Gestures.GestureBinding.instance.pointerRouter.removeGlobalRoute((global::System.Action<global::Doroti.Framework.Gestures.PointerEvent>)this._handleGlobalPointerEvent);
        RawTooltip._openedTooltips.Remove(this);
        this._longPressRecognizer?.onLongPressCancel = null;
        this._longPressRecognizer?.dispose();
        this._tapRecognizer?.onTapCancel = null;
        this._tapRecognizer?.dispose();
        this._timer?.cancel();
        this._backingController?.dispose();
        this._backingOverlayAnimation?.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (((((RawTooltip)this.widget).semanticsTooltip is null ? (bool?)null : ((RawTooltip)this.widget).semanticsTooltip.Length == 0) ?? false))
        {
            return ((RawTooltip)this.widget).child;
        }
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasOverlay(context));
        bool excludeFromSemantics__31681 = ((((RawTooltip)this.widget).semanticsTooltip is null) || (((RawTooltip)this.widget).semanticsTooltip!.Length == 0));
        Widget result__31901 = ((Widget)(object?)new Semantics(tooltip: (excludeFromSemantics__31681 ? null : ((RawTooltip)this.widget).semanticsTooltip), child: ((RawTooltip)this.widget).child));
        result__31901 = DartRuntimePrimitives.ConvertValue<Widget>(new _ExclusiveMouseRegion__raw_tooltip(onEnter: (global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>)this._handleMouseEnter, onExit: (global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>)this._handleMouseExit, child: new Listener(onPointerDown: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)this._handlePointerDown, behavior: global::Doroti.Framework.Rendering.HitTestBehavior.opaque, child: result__31901)));
        return ((Widget)(object?)OverlayPortal.CreateOverlayChildLayoutBuilder(controller: this._overlayController, overlayChildBuilder: this._buildTooltipOverlay, child: result__31901));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._ticker = new global::Doroti.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
    }

}

internal class _TooltipPositionDelegate__raw_tooltip : global::Doroti.Framework.Rendering.SingleChildLayoutDelegate
{
    public virtual Offset target { get; private set; } = default!;
    public virtual Size targetSize { get; private set; } = default!;
    public virtual global::System.Func<TooltipPositionContext, Offset>? positionDelegate { get; private set; }

    internal _TooltipPositionDelegate__raw_tooltip(Offset target, Size targetSize, global::System.Func<TooltipPositionContext, Offset>? positionDelegate = null)
    {
        this.target = target;
        this.targetSize = targetSize;
        this.positionDelegate = positionDelegate;
    }

    public override global::Doroti.Framework.Rendering.BoxConstraints getConstraintsForChild(global::Doroti.Framework.Rendering.BoxConstraints constraints) => constraints.loosen();
    public override Offset getPositionForChild(Size size, Size childSize)
    {
        if ((this.positionDelegate is not null))
        {
            return this.positionDelegate!(new TooltipPositionContext(target: this.target, targetSize: this.targetSize, tooltipSize: childSize, overlaySize: size, verticalOffset: 0.0));
        }
        return global::Doroti.Framework.Painting.GeometryLibrary.positionDependentBox(size: size, childSize: childSize, target: this.target, preferBelow: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_TooltipPositionDelegate__raw_tooltip)(object)oldDelegate;
        return (((!object.Equals(this.target, ((_TooltipPositionDelegate__raw_tooltip)__oldDelegate).target)) || (!object.Equals(this.targetSize, ((_TooltipPositionDelegate__raw_tooltip)__oldDelegate).targetSize))) || (!object.Equals((global::System.Func<TooltipPositionContext, Offset>?)this.positionDelegate, (global::System.Func<TooltipPositionContext, Offset>?)((_TooltipPositionDelegate__raw_tooltip)__oldDelegate).positionDelegate)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

