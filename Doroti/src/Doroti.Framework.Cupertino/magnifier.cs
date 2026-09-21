// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/magnifier.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public class CupertinoTextMagnifier : StatefulWidget
{
    public virtual Curve animationCurve { get; private set; } = default!;
    public virtual MagnifierController controller { get; private set; } = default!;
    public virtual double dragResistance { get; private set; } = default!;
    public virtual double hideBelowThreshold { get; private set; } = default!;
    public virtual double horizontalScreenEdgePadding { get; private set; } = default!;
    public virtual ValueNotifier<MagnifierInfo> magnifierInfo { get; private set; } = default!;
    internal static Duration _kDragAnimationDuration = Duration.Create(milliseconds: 45L);

    public CupertinoTextMagnifier(
        Key? key = null,
        Curve animationCurve = default!,
        MagnifierController controller = default!,
        double dragResistance = 10.0,
        double hideBelowThreshold = 48.0,
        double horizontalScreenEdgePadding = 10.0,
        ValueNotifier<MagnifierInfo> magnifierInfo = default!
    )
        : base(key: key)
    {
        Curve __animationCurve = animationCurve ?? Curves.easeOut;
        this.animationCurve = __animationCurve;
        this.controller = controller;
        this.dragResistance = dragResistance;
        this.hideBelowThreshold = hideBelowThreshold;
        this.horizontalScreenEdgePadding = horizontalScreenEdgePadding;
        this.magnifierInfo = magnifierInfo;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTextMagnifierState__magnifier());
}

internal class _CupertinoTextMagnifierState__magnifier
    : State<CupertinoTextMagnifier>,
        SingleTickerProviderStateMixin<CupertinoTextMagnifier>
{
    internal virtual Offset _currentAdjustedMagnifierPosition { get; set; } = Offset.zero;
    internal virtual double _verticalFocalPointAdjustment { get; set; } = 0;
    internal virtual AnimationController _ioAnimationController { get; private set; } = default!;
    internal virtual Animation<double> _ioAnimation { get; private set; } = default!;
    internal virtual CurvedAnimation _ioCurvedAnimation { get; private set; } = default!;
    internal virtual Action _magnifierInfoListener { get; set; } = default!;
    internal virtual Action _tickerModeListener { get; set; } = default!;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _magnifierInfoListener = _determineMagnifierPositionAndFocalPoint;
        _tickerModeListener = _updateTicker;
        _ioAnimationController = (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = new AnimationController(
                        value: 0,
                        vsync: this,
                        duration: CupertinoMagnifier._kInOutAnimationDuration
                    );
                    __cascade.addListener(() =>
                    {
                        setState(() => { });
                    });
                    return __cascade;
                }
            )
        )();
        widget.controller.animationController = _ioAnimationController;
        widget.magnifierInfo.addListener(_magnifierInfoListener);
        _ioCurvedAnimation = new CurvedAnimation(
            parent: _ioAnimationController,
            curve: widget.animationCurve
        );
        _ioAnimation = new Tween<double>(begin: 0.0, end: 1.0).animate(_ioCurvedAnimation);
    }

    public override void dispose()
    {
        widget.magnifierInfo.removeListener(_magnifierInfoListener);
        _tickerModeNotifier?.removeListener(_tickerModeListener);
        widget.controller.animationController = null;
        _ioAnimationController.dispose();
        _ioCurvedAnimation.dispose();
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
        });
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(CupertinoTextMagnifier oldWidget)
    {
        if (!Equals(oldWidget.magnifierInfo, widget.magnifierInfo))
        {
            oldWidget.magnifierInfo.removeListener(_magnifierInfoListener);
            widget.magnifierInfo.addListener(_magnifierInfoListener);
        }
        base.didUpdateWidget(oldWidget);
    }

    public override void didChangeDependencies()
    {
        _determineMagnifierPositionAndFocalPoint();
        base.didChangeDependencies();
    }

    internal virtual void _determineMagnifierPositionAndFocalPoint()
    {
        MagnifierInfo textEditingContext = widget.magnifierInfo.value;
        double verticalCenterOfCurrentLine = textEditingContext.caretRect.center.dy;
        if (
            (verticalCenterOfCurrentLine - textEditingContext.globalGesturePosition.dy)
            < -widget.hideBelowThreshold
        )
        {
            if (widget.controller.shown)
            {
                DartRuntimePrimitives.Ignore(widget.controller.hide(removeFromOverlay: false));
            }
            return;
        }
        if (!widget.controller.shown)
        {
            _ioAnimationController.forward();
        }
        double verticalPositionOfLens = Math.Max(
            verticalCenterOfCurrentLine,
            verticalCenterOfCurrentLine
                - (
                    (verticalCenterOfCurrentLine - textEditingContext.globalGesturePosition.dy)
                    / widget.dragResistance
                )
        );
        var rawMagnifierPosition = new Offset(
            textEditingContext.globalGesturePosition.dx
                - (CupertinoMagnifier.kDefaultSize.width / 2L),
            verticalPositionOfLens
                - (
                    CupertinoMagnifier.kDefaultSize.height
                    - CupertinoMagnifier.kMagnifierAboveFocalPoint
                )
        );
        Rect screenRect = Offset.zero & MediaQuery.sizeOf(context);
        Offset adjustedMagnifierPosition = MagnifierController
            .shiftWithinBounds(
                bounds: Rect.fromLTRB(
                    screenRect.left + widget.horizontalScreenEdgePadding,
                    screenRect.top
                        - (
                            CupertinoMagnifier.kDefaultSize.height
                            + CupertinoMagnifier.kMagnifierAboveFocalPoint
                        ),
                    screenRect.right - widget.horizontalScreenEdgePadding,
                    screenRect.bottom
                        + (
                            CupertinoMagnifier.kDefaultSize.height
                            + CupertinoMagnifier.kMagnifierAboveFocalPoint
                        )
                ),
                rect: rawMagnifierPosition & CupertinoMagnifier.kDefaultSize
            )
            .topLeft;
        setState(() =>
        {
            _currentAdjustedMagnifierPosition = adjustedMagnifierPosition;
            _verticalFocalPointAdjustment = verticalCenterOfCurrentLine - verticalPositionOfLens;
        });
    }

    public override Widget build(BuildContext context)
    {
        CupertinoThemeData themeData = CupertinoTheme.of(context);
        return new AnimatedPositioned(
            duration: CupertinoTextMagnifier._kDragAnimationDuration,
            curve: widget.animationCurve,
            left: _currentAdjustedMagnifierPosition.dx,
            top: _currentAdjustedMagnifierPosition.dy,
            child: new CupertinoMagnifier(
                inOutAnimation: _ioAnimation,
                additionalFocalPointOffset: new Offset(0, _verticalFocalPointAdjustment),
                borderSide: new BorderSide(color: themeData.primaryColor, width: 2.0)
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        _tickerModeNotifier?.removeListener(_tickerModeListener);
        newNotifier.addListener(_tickerModeListener);
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

public class CupertinoMagnifier : StatelessWidget
{
    public virtual List<BoxShadow> shadows { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual BorderSide borderSide { get; private set; } = default!;
    public static double kMagnifierAboveFocalPoint = -26;
    public static Size kDefaultSize = new Size(80, 47.5);
    internal static Duration _kInOutAnimationDuration = Duration.Create(milliseconds: 150L);
    public virtual Size size { get; private set; } = default!;
    public virtual BorderRadius borderRadius { get; private set; } = default!;
    public virtual Animation<double>? inOutAnimation { get; private set; }
    public virtual Offset additionalFocalPointOffset { get; private set; } = default!;
    public virtual double magnificationScale { get; private set; } = default!;

    public CupertinoMagnifier(
        Key? key = null,
        Size? size = null,
        BorderRadius borderRadius = default!,
        Offset additionalFocalPointOffset = default,
        List<BoxShadow> shadows = default!,
        Clip clipBehavior = Clip.none,
        BorderSide borderSide = default!,
        Animation<double>? inOutAnimation = null,
        double magnificationScale = 1.0
    )
        : base(key: key)
    {
        Size __size = size ?? kDefaultSize;
        BorderRadius __borderRadius =
            borderRadius ?? BorderRadius.CreateAll(Radius.elliptical(60, 50));
        List<BoxShadow> __shadows =
            shadows
            ?? new List<BoxShadow>
            {
                new BoxShadow(
                    color: Color.fromARGB(25, 0, 0, 0),
                    blurRadius: 11,
                    spreadRadius: 0.2,
                    blurStyle: BlurStyle.outer
                ),
            };
        BorderSide __borderSide =
            borderSide ?? new BorderSide(color: Color.fromARGB(255, 0, 124, 255), width: 2.0);
        this.size = __size;
        this.borderRadius = __borderRadius;
        this.additionalFocalPointOffset = additionalFocalPointOffset;
        this.shadows = __shadows;
        this.clipBehavior = clipBehavior;
        this.borderSide = __borderSide;
        this.inOutAnimation = inOutAnimation;
        this.magnificationScale = magnificationScale;
        System.Diagnostics.Debug.Assert(magnificationScale > 0L);
    }

    public override Widget build(BuildContext context)
    {
        var focalPointOffsetLocal = new Offset(
            0,
            (kDefaultSize.height / 2L) - kMagnifierAboveFocalPoint
        );
        focalPointOffsetLocal.scale(1, inOutAnimation?.value ?? 1);
        focalPointOffsetLocal += additionalFocalPointOffset;
        return Transform.CreateTranslate(
            offset: (
                Dart_uiLibrary.Offset.lerp(
                    new Offset(0, -kMagnifierAboveFocalPoint),
                    Offset.zero,
                    inOutAnimation?.value ?? 1
                ) ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            child: new RawMagnifier(
                size: (
                    size
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                focalPointOffset: focalPointOffsetLocal,
                decoration: new MagnifierDecoration(
                    opacity: inOutAnimation?.value ?? 1,
                    shape: new RoundedRectangleBorder(borderRadius: borderRadius, side: borderSide),
                    shadows: shadows
                ),
                clipBehavior: clipBehavior,
                magnificationScale: magnificationScale
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
