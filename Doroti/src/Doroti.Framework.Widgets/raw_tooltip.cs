// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/raw_tooltip.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class Raw_tooltipLibrary
{
    internal static AnimationStyle _kDefaultAnimationStyle = new AnimationStyle(
        curve: Curves.fastOutSlowIn,
        duration: Duration.Create(milliseconds: 150L),
        reverseDuration: Duration.Create(milliseconds: 75L)
    );
}

public delegate Widget TooltipComponentBuilder(BuildContext context, Animation<double> animation);

public delegate Offset TooltipPositionDelegate(TooltipPositionContext context);

public class TooltipPositionContext
{
    public virtual Offset target { get; private set; } = default!;
    public virtual Size targetSize { get; private set; } = default!;
    public virtual Size tooltipSize { get; private set; } = default!;
    public virtual double verticalOffset { get; private set; } = default!;
    public virtual bool preferBelow { get; private set; } = default!;
    public virtual Size overlaySize { get; private set; } = default!;

    public TooltipPositionContext(
        Offset target,
        Size targetSize,
        Size tooltipSize,
        double verticalOffset,
        bool preferBelow = true,
        Size? overlaySize = null
    )
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
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is TooltipPositionContext)
            && Equals(__other.target, target)
            && Equals(__other.targetSize, targetSize)
            && Equals(__other.tooltipSize, tooltipSize)
            && Equals(__other.overlaySize, overlaySize)
            && (__other.verticalOffset == verticalOffset)
            && (__other.preferBelow == preferBelow);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                target,
                targetSize,
                tooltipSize,
                (
                    overlaySize
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                verticalOffset,
                preferBelow
            )
        );
}

public enum TooltipTriggerMode
{
    manual,
    longPress,
    tap,
}

public delegate void TooltipTriggeredCallback();

internal class _ExclusiveMouseRegion__raw_tooltip : MouseRegion
{
    internal _ExclusiveMouseRegion__raw_tooltip(
        Action<Gestures.PointerEnterEvent>? onEnter = null,
        Action<Gestures.PointerExitEvent>? onExit = null,
        Widget? child = null
    )
        : base(onEnter: onEnter, onExit: onExit, child: child) { }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderExclusiveMouseRegion__raw_tooltip(onEnter: onEnter, onExit: onExit);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class _RenderExclusiveMouseRegion__raw_tooltip : RenderMouseRegion
{
    public static bool isOutermostMouseRegion = true;
    public static bool foundInnermostMouseRegion = false;

    internal _RenderExclusiveMouseRegion__raw_tooltip(
        Action<Gestures.PointerEnterEvent>? onEnter = null,
        Action<Gestures.PointerExitEvent>? onExit = null
    )
        : base(onEnter: onEnter, onExit: onExit) { }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        var isHit = false;
        bool outermost = isOutermostMouseRegion;
        isOutermostMouseRegion = false;
        if (size.contains(position))
        {
            isHit = hitTestChildren(result, position: position) || hitTestSelf(position);
            if (
                (isHit || Equals(behavior, HitTestBehavior.translucent))
                && !foundInnermostMouseRegion
            )
            {
                foundInnermostMouseRegion = true;
                result.add(new BoxHitTestEntry(this, position));
            }
        }
        if (outermost)
        {
            isOutermostMouseRegion = true;
            foundInnermostMouseRegion = false;
        }
        return isHit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RawTooltip : StatefulWidget
{
    public virtual string? semanticsTooltip { get; private set; }
    public virtual Func<BuildContext, Animation<double>, Widget> tooltipBuilder
    {
        get;
        private set;
    } = default!;
    public virtual Duration hoverDelay { get; private set; } = default!;
    public virtual Duration touchDelay { get; private set; } = default!;
    public virtual Duration dismissDelay { get; private set; } = default!;
    public virtual bool enableTapToDismiss { get; private set; } = default!;
    public virtual TooltipTriggerMode triggerMode { get; private set; } = default!;
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual Action? onTriggered { get; private set; }
    public virtual AnimationStyle animationStyle { get; private set; } = default!;
    public virtual Func<TooltipPositionContext, Offset>? positionDelegate { get; private set; }
    public virtual bool ignorePointer { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    internal static List<RawTooltipState> _openedTooltips = new List<RawTooltipState>();

    public RawTooltip(
        Key? key = null,
        string? semanticsTooltip = default!,
        Func<BuildContext, Animation<double>, Widget> tooltipBuilder = default!,
        Duration hoverDelay = default,
        Duration? touchDelay = null,
        Duration? dismissDelay = null,
        bool enableTapToDismiss = true,
        TooltipTriggerMode triggerMode = TooltipTriggerMode.longPress,
        bool enableFeedback = true,
        Action? onTriggered = null,
        AnimationStyle animationStyle = default!,
        Func<TooltipPositionContext, Offset>? positionDelegate = null,
        bool ignorePointer = false,
        Widget child = default!
    )
        : base(key: key)
    {
        Duration __touchDelay = touchDelay ?? Duration.Create(milliseconds: 1500);
        Duration __dismissDelay = dismissDelay ?? Duration.Create(milliseconds: 100);
        AnimationStyle __animationStyle =
            animationStyle ?? Raw_tooltipLibrary._kDefaultAnimationStyle;
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
        if (!Enumerable.Any(_openedTooltips))
        {
            return false;
        }
        List<RawTooltipState> openedTooltips = _openedTooltips.ToList().ToList();
        foreach (var state in openedTooltips)
        {
            DartRuntimePrimitives.Assert(() => state.mounted);
            state._scheduleDismissTooltip();
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new RawTooltipState());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new StringProperty(
                "semantics",
                semanticsTooltip,
                showName: (semanticsTooltip is null) || (semanticsTooltip!.Length == 0),
                defaultValue: ((semanticsTooltip is null) || (semanticsTooltip!.Length == 0))
                    ? null
                    : DiagnosticsLibrary.kNoDefaultValue.ToString()
            )
        );
        properties.add(
            new DiagnosticsProperty<Duration>("hover delay", hoverDelay, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Duration>("touch delay", (touchDelay), defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Duration>("dismiss delay", (dismissDelay), defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TooltipTriggerMode>(
                "triggerMode",
                triggerMode,
                defaultValue: null
            )
        );
        properties.add(
            new FlagProperty(
                "enableFeedback",
                value: enableFeedback,
                ifTrue: "true",
                showName: true
            )
        );
        properties.add(
            new DiagnosticsProperty<Func<TooltipPositionContext, Offset>>(
                "positionDelegate",
                positionDelegate,
                defaultValue: null
            )
        );
    }
}

public class RawTooltipState : State<RawTooltip>, SingleTickerProviderStateMixin<RawTooltip>
{
    internal virtual OverlayPortalController _overlayController { get; private set; } =
        new OverlayPortalController();
    internal virtual Timer? _timer { get; set; } = default;
    internal virtual AnimationController? _backingController { get; set; } = default;
    internal virtual CurvedAnimation? _backingOverlayAnimation { get; set; } = default;
    internal virtual LongPressGestureRecognizer? _longPressRecognizer { get; set; } = default;
    internal virtual TapGestureRecognizer? _tapRecognizer { get; set; } = default;
    internal virtual HashSet<long> _activeHoveringPointerDevices { get; private set; } =
        new HashSet<long>();
    internal virtual AnimationStatus _animationStatus { get; set; } = AnimationStatus.dismissed;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual AnimationController _controller
    {
        get
        {
            return _backingController ??= (
                (Func<AnimationController>)(
                    () =>
                    {
                        var __cascade = new AnimationController(
                            duration: widget.animationStyle.duration,
                            reverseDuration: widget.animationStyle.reverseDuration,
                            vsync: this
                        );
                        __cascade.addStatusListener(_handleStatusChanged);
                        return __cascade;
                    }
                )
            )();
        }
    }
    internal virtual CurvedAnimation _overlayAnimation
    {
        get
        {
            return _backingOverlayAnimation ??= new CurvedAnimation(
                parent: _controller,
                curve: widget.animationStyle.curve
                    ?? Raw_tooltipLibrary._kDefaultAnimationStyle.curve!
            );
        }
    }

    internal virtual void _handleStatusChanged(AnimationStatus status)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        switch (
            (
                AnimationStatusMembers.isDismissed(_animationStatus),
                AnimationStatusMembers.isDismissed(status)
            )
        )
        {
            case (false, true):
            {
                RawTooltip._openedTooltips.Remove(this);
                _overlayController.hide();
                break;
            }
            case (true, false):
            {
                _overlayController.show();
                RawTooltip._openedTooltips.Add(this);
                DartRuntimePrimitives.Ignore(
                    SemanticsService.tooltip(widget.semanticsTooltip ?? "")
                );
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
        DartRuntimePrimitives.Assert(() => mounted);
        void show()
        {
            DartRuntimePrimitives.Assert(() => mounted);
            _controller.forward();
            _timer?.cancel();
            _timer =
                (touchDelay is null)
                    ? null
                    : new Timer(
                        (
                            touchDelay
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ),
                        () =>
                        {
                            _ = ((Func<double?, Scheduler.TickerFuture>)_controller.reverse)(
                                default
                            );
                        }
                    );
        }
        DartRuntimePrimitives.Assert(
            () =>
                !(_timer?.isActive ?? false)
                || (!Equals(_controller.status, AnimationStatus.reverse)),
            () => (object?)"timer must not be active when the tooltip is animating out"
        );
        if (_controller.isDismissed && (withDelay.inMicroseconds > 0L))
        {
            _timer?.cancel();
            _timer = new Timer(withDelay, show);
        }
        else
        {
            show();
        }
    }

    internal virtual void _scheduleDismissTooltip(Duration withDelay = default)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        DartRuntimePrimitives.Assert(
            () =>
                !(_timer?.isActive ?? false)
                || (!Equals(_backingController?.status, AnimationStatus.reverse)),
            () => (object?)"timer must not be active when the tooltip is animating out"
        );
        _timer?.cancel();
        _timer = null;
        if (_backingController?.isForwardOrCompleted ?? false)
        {
            if (withDelay.inMicroseconds > 0L)
            {
                _timer = new Timer(
                    withDelay,
                    () =>
                    {
                        _ = ((Func<double?, Scheduler.TickerFuture>)_controller.reverse)(default);
                    }
                );
            }
            else
            {
                _controller.reverse();
            }
        }
    }

    internal virtual void _handlePointerDown(Gestures.PointerDownEvent @event)
    {
        var triggerModeDeviceKinds = new HashSet<PointerDeviceKind>
        {
            PointerDeviceKind.invertedStylus,
            PointerDeviceKind.stylus,
            PointerDeviceKind.touch,
            PointerDeviceKind.unknown,
            PointerDeviceKind.trackpad,
        };
        switch (widget.triggerMode)
        {
            case TooltipTriggerMode.longPress:
            {
                LongPressGestureRecognizer recognizer = _longPressRecognizer ??=
                    new LongPressGestureRecognizer(
                        debugOwner: this,
                        supportedDevices: triggerModeDeviceKinds
                    );
                DartRuntimePrimitives.Ignore(
                    (
                        (Func<LongPressGestureRecognizer>)(
                            () =>
                            {
                                var __cascade = recognizer;
                                __cascade.onLongPressCancel = _handleTapToDismiss;
                                __cascade.onLongPress = _handleLongPress;
                                __cascade.onLongPressUp = _handlePressUp;
                                __cascade.addPointer(@event);
                                return __cascade;
                            }
                        )
                    )()
                );
                break;
            }
            case TooltipTriggerMode.tap:
            {
                TapGestureRecognizer recognizerLocal = _tapRecognizer ??= new TapGestureRecognizer(
                    debugOwner: this,
                    supportedDevices: triggerModeDeviceKinds
                );
                DartRuntimePrimitives.Ignore(
                    (
                        (Func<TapGestureRecognizer>)(
                            () =>
                            {
                                var __cascade = recognizerLocal;
                                __cascade.onTapCancel = _handleTapToDismiss;
                                __cascade.onTap = _handleTap;
                                __cascade.addPointer(@event);
                                return __cascade;
                            }
                        )
                    )()
                );
                break;
            }
            case TooltipTriggerMode.manual:
            {
                break;
            }
        }
    }

    internal virtual void _handleGlobalPointerEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        if (
            (_tapRecognizer?.primaryPointer == @event.pointer)
            || (_longPressRecognizer?.primaryPointer == @event.pointer)
        )
        {
            return;
        }
        if (
            ((_timer is null) && _controller.isDismissed)
            || (@event is not Gestures.PointerDownEvent)
        )
        {
            return;
        }
        _handleTapToDismiss();
    }

    internal virtual void _handleTapToDismiss()
    {
        if (!widget.enableTapToDismiss)
        {
            return;
        }
        _scheduleDismissTooltip();
        _activeHoveringPointerDevices.Clear();
    }

    internal virtual void _handleTap()
    {
        bool tooltipCreated = _controller.isDismissed;
        if (tooltipCreated && widget.enableFeedback)
        {
            DartRuntimePrimitives.Assert(() => Equals(widget.triggerMode, TooltipTriggerMode.tap));
            DartRuntimePrimitives.Ignore(Feedback.forTap(context));
        }
        widget.onTriggered?.Invoke();
        _scheduleShowTooltip(
            withDelay: Duration.zero,
            touchDelay: !Enumerable.Any(_activeHoveringPointerDevices) ? widget.touchDelay : null
        );
    }

    internal virtual void _handleLongPress()
    {
        bool tooltipCreated = _controller.isDismissed;
        if (tooltipCreated && widget.enableFeedback)
        {
            DartRuntimePrimitives.Assert(() =>
                Equals(widget.triggerMode, TooltipTriggerMode.longPress)
            );
            DartRuntimePrimitives.Ignore(Feedback.forLongPress(context));
        }
        widget.onTriggered?.Invoke();
        _scheduleShowTooltip(withDelay: Duration.zero);
    }

    internal virtual void _handlePressUp()
    {
        if (Enumerable.Any(_activeHoveringPointerDevices))
        {
            return;
        }
        _scheduleDismissTooltip(withDelay: (widget.touchDelay));
    }

    internal virtual void _handleMouseEnter(Gestures.PointerEnterEvent @event)
    {
        _activeHoveringPointerDevices.Add(@event.device);
        List<RawTooltipState> tooltipsToDismiss = RawTooltip
            ._openedTooltips.where(
                (tooltip) => !Enumerable.Any(tooltip._activeHoveringPointerDevices)
            )
            .ToList()
            .ToList();
        foreach (var tooltipLocal in tooltipsToDismiss)
        {
            DartRuntimePrimitives.Assert(() => tooltipLocal.mounted);
            tooltipLocal._scheduleDismissTooltip();
        }
        _scheduleShowTooltip(
            withDelay: Enumerable.Any(tooltipsToDismiss) ? Duration.zero : widget.hoverDelay
        );
    }

    internal virtual void _handleMouseExit(Gestures.PointerExitEvent @event)
    {
        if (!Enumerable.Any(_activeHoveringPointerDevices))
        {
            return;
        }
        _activeHoveringPointerDevices.Remove(@event.device);
        if (!Enumerable.Any(_activeHoveringPointerDevices))
        {
            _scheduleDismissTooltip(withDelay: widget.dismissDelay);
        }
    }

    public virtual bool ensureTooltipVisible()
    {
        _timer?.cancel();
        _timer = null;
        if (_controller.isForwardOrCompleted)
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
        GestureBinding.instance.pointerRouter.addGlobalRoute(_handleGlobalPointerEvent);
    }

    internal virtual Widget _buildTooltipOverlay(
        BuildContext context,
        OverlayChildLayoutInfo layoutInfo
    )
    {
        if (layoutInfo.childPaintTransform.determinant == 0.0)
        {
            return SizedBox.CreateShrink();
        }
        Offset targetLocal = MatrixUtils.transformPoint(
            layoutInfo.childPaintTransform,
            layoutInfo.childSize.center(Offset.zero)
        );
        Widget tooltip = new IgnorePointer(
            ignoring: widget.ignorePointer,
            child: new _ExclusiveMouseRegion__raw_tooltip(
                onEnter: _handleMouseEnter,
                onExit: _handleMouseExit,
                child: widget.tooltipBuilder(context, _overlayAnimation)
            )
        );
        Widget overlayChild = Positioned.CreateFill(
            bottom: MediaQuery.maybeViewInsetsOf(context)?.bottom ?? 0.0,
            child: new CustomSingleChildLayout(
                @delegate: new _TooltipPositionDelegate__raw_tooltip(
                    target: targetLocal,
                    targetSize: layoutInfo.childSize,
                    positionDelegate: widget.positionDelegate
                ),
                child: tooltip
            )
        );
        return (SelectionContainer.maybeOf(context) is null)
            ? overlayChild
            : SelectionContainer.CreateDisabled(child: overlayChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        GestureBinding.instance.pointerRouter.removeGlobalRoute(_handleGlobalPointerEvent);
        RawTooltip._openedTooltips.Remove(this);
        _longPressRecognizer?.onLongPressCancel = null;
        _longPressRecognizer?.dispose();
        _tapRecognizer?.onTapCancel = null;
        _tapRecognizer?.dispose();
        _timer?.cancel();
        _backingController?.dispose();
        _backingOverlayAnimation?.dispose();
        DartRuntimePrimitives.Assert(() =>
        {
            if ((_ticker is null) || !_ticker!.isActive)
            {
                return true;
            }
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary($"{this} was disposed with an active Ticker."),
                        new ErrorDescription(
                            $"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time "
                                + "dispose() was called on the mixin, that Ticker was still active. The Ticker must "
                                + "be disposed before calling super.dispose()."
                        ),
                        new ErrorHint(
                            "Tickers used by AnimationControllers "
                                + "should be disposed by calling dispose() on the AnimationController itself. "
                                + "Otherwise, the ticker will leak."
                        ),
                        _ticker!.describeForError("The offending ticker was"),
                    }
                )
            );
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (
            (widget.semanticsTooltip is null ? (bool?)null : widget.semanticsTooltip.Length == 0)
            ?? false
        )
        {
            return widget.child;
        }
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasOverlay(context));
        bool excludeFromSemantics =
            (widget.semanticsTooltip is null) || (widget.semanticsTooltip!.Length == 0);
        Widget result = new Semantics(
            tooltip: excludeFromSemantics ? null : widget.semanticsTooltip,
            child: widget.child
        );
        result = DartRuntimePrimitives.ConvertValue<Widget>(
            new _ExclusiveMouseRegion__raw_tooltip(
                onEnter: _handleMouseEnter,
                onExit: _handleMouseExit,
                child: new Listener(
                    onPointerDown: _handlePointerDown,
                    behavior: HitTestBehavior.opaque,
                    child: result
                )
            )
        );
        return OverlayPortal.CreateOverlayChildLayoutBuilder(
            controller: _overlayController,
            overlayChildBuilder: _buildTooltipOverlay,
            child: result
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_ticker is null)
            {
                return true;
            }
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."
                        ),
                        new ErrorDescription(
                            "A SingleTickerProviderStateMixin can only be used as a TickerProvider once."
                        ),
                        new ErrorHint(
                            "If a State is used for multiple AnimationController objects, or if it is passed to other "
                                + "objects and those objects might use it more than one time in total, then instead of "
                                + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin."
                        ),
                    }
                )
            );
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        _ticker = new Scheduler.Ticker(
            onTick,
            debugLabel: Foundation.ConstantsLibrary.kDebugMode
                ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                : null
        );
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
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
        TickerModeData values = _tickerModeNotifier!.value;
        if (_ticker is not null)
        {
            _ticker!.muted = !values.enabled;
            _ticker!.forceFrames = values.forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch
        {
            (true, true) => "active but muted",
            (true, _) => "active",
            (false, true) => "inactive and muted",
            (false, _) => "inactive",
            (null, _) => DartRuntimePrimitives.ConvertValue<string>(null),
        };
        properties.add(
            new DiagnosticsProperty<Scheduler.Ticker>(
                "ticker",
                _ticker,
                description: tickerDescription,
                showSeparator: false,
                defaultValue: default
            )
        );
    }
}

internal class _TooltipPositionDelegate__raw_tooltip : SingleChildLayoutDelegate
{
    public virtual Offset target { get; private set; } = default!;
    public virtual Size targetSize { get; private set; } = default!;
    public virtual Func<TooltipPositionContext, Offset>? positionDelegate { get; private set; }

    internal _TooltipPositionDelegate__raw_tooltip(
        Offset target,
        Size targetSize,
        Func<TooltipPositionContext, Offset>? positionDelegate = null
    )
    {
        this.target = target;
        this.targetSize = targetSize;
        this.positionDelegate = positionDelegate;
    }

    public override BoxConstraints getConstraintsForChild(BoxConstraints constraints) =>
        constraints.loosen();

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        if (positionDelegate is not null)
        {
            return positionDelegate!(
                new TooltipPositionContext(
                    target: target,
                    targetSize: targetSize,
                    tooltipSize: childSize,
                    overlaySize: size,
                    verticalOffset: 0.0
                )
            );
        }
        return GeometryLibrary.positionDependentBox(
            size: size,
            childSize: childSize,
            target: target,
            preferBelow: true
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_TooltipPositionDelegate__raw_tooltip)oldDelegate;
        return (!Equals(target, __oldDelegate.target))
            || (!Equals(targetSize, __oldDelegate.targetSize))
            || (!Equals(positionDelegate, __oldDelegate.positionDelegate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
