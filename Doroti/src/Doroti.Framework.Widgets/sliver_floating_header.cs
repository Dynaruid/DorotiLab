// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver_floating_header.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public enum FloatingHeaderSnapMode
{
    overlay,
    scroll
}

public class SliverFloatingHeader : StatefulWidget
{
    public virtual AnimationStyle? animationStyle { get; private set; }
    public virtual FloatingHeaderSnapMode? snapMode { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public SliverFloatingHeader(Key? key = null, AnimationStyle? animationStyle = null, FloatingHeaderSnapMode? snapMode = null, Widget child = default!) : base(key: key)
    {
        this.animationStyle = animationStyle;
        this.snapMode = snapMode;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SliverFloatingHeaderState__sliver_floating_header());
}

internal class _SliverFloatingHeaderState__sliver_floating_header : State<SliverFloatingHeader>, SingleTickerProviderStateMixin<SliverFloatingHeader>
{
    public virtual ScrollPosition? position { get; set; } = default;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override Widget build(BuildContext context)
    {
        return new _SliverFloatingHeader__sliver_floating_header(vsync: this, animationStyle: widget.animationStyle, snapMode: widget.snapMode, child: new _SnapTrigger__sliver_floating_header(widget.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _ticker = new Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
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
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new DiagnosticsProperty<Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

internal class _SnapTrigger__sliver_floating_header : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;

    internal _SnapTrigger__sliver_floating_header(Widget child)
    {
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SnapTriggerState__sliver_floating_header());
}

internal class _SnapTriggerState__sliver_floating_header : State<_SnapTrigger__sliver_floating_header>
{
    public virtual ScrollPosition? position { get; set; } = default;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (position is not null)
        {
            position!.isScrollingNotifier.removeListener(isScrollingListener);
        }
        position = Scrollable.maybeOf(context)?.position;
        if (position is not null)
        {
            position!.isScrollingNotifier.addListener(isScrollingListener);
        }
    }

    public override void dispose()
    {
        if (position is not null)
        {
            position!.isScrollingNotifier.removeListener(isScrollingListener);
        }
        base.dispose();
    }

    public virtual void isScrollingListener()
    {
        DartRuntimePrimitives.Assert(() => position is not null);
        _RenderSliverFloatingHeader__sliver_floating_header? renderer = context.findAncestorRenderObjectOfType<_RenderSliverFloatingHeader__sliver_floating_header>();
        renderer?.isScrollingUpdate(position!);
    }

    public override Widget build(BuildContext context) => widget.child;
}

internal class _SliverFloatingHeader__sliver_floating_header : SingleChildRenderObjectWidget
{
    public virtual Scheduler.TickerProvider? vsync { get; private set; }
    public virtual AnimationStyle? animationStyle { get; private set; }
    public virtual FloatingHeaderSnapMode? snapMode { get; private set; }

    internal _SliverFloatingHeader__sliver_floating_header(Scheduler.TickerProvider? vsync = null, AnimationStyle? animationStyle = null, FloatingHeaderSnapMode? snapMode = null, Widget? child = null) : base(child: child)
    {
        this.vsync = vsync;
        this.animationStyle = animationStyle;
        this.snapMode = snapMode;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSliverFloatingHeader__sliver_floating_header(vsync: vsync, animationStyle: animationStyle, snapMode: snapMode);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverFloatingHeader__sliver_floating_header)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSliverFloatingHeader__sliver_floating_header>)(() =>
{
    var __cascade = __renderObject;
    __cascade.vsync = vsync;
    __cascade.animationStyle = animationStyle;
    __cascade.snapMode = snapMode;
    return __cascade;
}))());
    }

}

public class _RenderSliverFloatingHeader__sliver_floating_header : RenderSliverSingleBoxAdapter
{
    public virtual Animation<double> snapAnimation { get; set; } = default!;
    public virtual AnimationController? snapController { get; set; } = default;
    public virtual double? lastScrollOffset { get; set; } = default;
    public virtual double effectiveScrollOffset { get; set; } = default!;
    internal virtual Scheduler.TickerProvider? _vsync { get; set; } = default;
    public virtual AnimationStyle? animationStyle { get; set; } = default;
    public virtual FloatingHeaderSnapMode? snapMode { get; set; } = default;

    internal _RenderSliverFloatingHeader__sliver_floating_header(Scheduler.TickerProvider? vsync = null, AnimationStyle? animationStyle = null, FloatingHeaderSnapMode? snapMode = null)
    {
        this.animationStyle = animationStyle;
        this.snapMode = snapMode;
        _vsync = vsync;
    }

    public virtual Scheduler.TickerProvider? vsync
    {
        get => _vsync;
        set
        {
            var __value = value;
            if (Equals(__value, _vsync))
            {
                return;
            }
            _vsync = __value;
            if (__value is null)
            {
                snapController?.dispose();
                snapController = null;
            }
            else
            {
                snapController?.resync(__value);
            }
        }
    }
    public virtual void isScrollingUpdate(ScrollPosition position)
    {
        if (position.isScrollingNotifier.value)
        {
            snapController?.stop();
        }
        else
        {
            ScrollDirection direction = position.userScrollDirection;
            bool headerIsPartiallyVisible = direction switch { ScrollDirection.forward when effectiveScrollOffset <= 0L => false, ScrollDirection.reverse when effectiveScrollOffset >= childExtent => false, _ => true };
            if (headerIsPartiallyVisible)
            {
                snapController ??= ((Func<AnimationController>)(() =>
{
    var __cascade = new AnimationController(vsync: vsync!);
    __cascade.addListener(() =>
    {
        if (effectiveScrollOffset != snapAnimation.value)
        {
            effectiveScrollOffset = snapAnimation.value;
            markNeedsLayout();
        }
    });
    return __cascade;
}))();
                snapController!.duration = direction switch { ScrollDirection.forward => animationStyle?.duration ?? Duration.Create(milliseconds: 300L), _ => animationStyle?.reverseDuration ?? Duration.Create(milliseconds: 300L) };
                snapAnimation = snapController!.drive(new Tween<double>(begin: effectiveScrollOffset, end: direction switch { ScrollDirection.forward => 0, _ => childExtent }).chain(new CurveTween(curve: direction switch { ScrollDirection.forward => animationStyle?.curve ?? Curves.easeInOut, _ => animationStyle?.reverseCurve ?? Curves.easeInOut })));
                snapController!.forward(from: 0.0);
            }
        }
    }

    public virtual double childExtent
    {
        get
        {
            if (child is null)
            {
                return 0.0;
            }
            DartRuntimePrimitives.Assert(() => child!.hasSize);
            return constraints.axis switch { Axis.vertical => child!.size.height, Axis.horizontal => child!.size.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    public override void detach()
    {
        snapController?.dispose();
        snapController = null;
        base.detach();
    }

    public virtual bool floatingHeaderNeedsToBeUpdated
    {
        get
        {
            return (lastScrollOffset is not null) && ((constraints.scrollOffset < DartRuntimePrimitives.RequireValue(lastScrollOffset)) || (effectiveScrollOffset < childExtent));
        }
    }
    public override void performLayout()
    {
        if (!floatingHeaderNeedsToBeUpdated)
        {
            effectiveScrollOffset = constraints.scrollOffset;
        }
        else
        {
            double delta = DartRuntimePrimitives.RequireValue(lastScrollOffset) - constraints.scrollOffset;
            if (Equals(constraints.userScrollDirection, ScrollDirection.forward))
            {
                if (effectiveScrollOffset > childExtent)
                {
                    effectiveScrollOffset = childExtent;
                }
            }
            else
            {
                delta = Dart_uiLibrary.clampDouble(delta, -double.PositiveInfinity, 0);
            }
            effectiveScrollOffset = Dart_uiLibrary.clampDouble(effectiveScrollOffset - delta, 0.0, constraints.scrollOffset);
        }
        child?.layout(constraints.asBoxConstraints(), parentUsesSize: true);
        double paintExtentLocal = childExtent - effectiveScrollOffset;
        double layoutExtentLocal = (snapMode ?? FloatingHeaderSnapMode.overlay) switch { FloatingHeaderSnapMode.overlay => childExtent - constraints.scrollOffset, FloatingHeaderSnapMode.scroll => paintExtentLocal, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        geometry = new SliverGeometry(paintOrigin: Math.Min(constraints.overlap, 0.0), scrollExtent: childExtent, paintExtent: Dart_uiLibrary.clampDouble(paintExtentLocal, 0.0, constraints.remainingPaintExtent), layoutExtent: Dart_uiLibrary.clampDouble(layoutExtentLocal, 0.0, constraints.remainingPaintExtent), maxPaintExtent: childExtent, hasVisualOverflow: true);
        lastScrollOffset = constraints.scrollOffset;
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        return (geometry is null) ? 0 : Math.Min(0, geometry!.paintExtent - childExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => Equals(child, this.child));
        applyPaintTransformForBoxChild(((RenderBox?)child)!, transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null) && geometry!.visible)
        {
            offset += SliverLibrary.applyGrowthDirectionToAxisDirection(constraints.axisDirection, constraints.growthDirection) switch { AxisDirection.up => new Offset(0.0, geometry!.paintExtent - childMainAxisPosition(child!) - childExtent), AxisDirection.left => new Offset(geometry!.paintExtent - childMainAxisPosition(child!) - childExtent, 0.0), AxisDirection.right => new Offset(childMainAxisPosition(child!), 0.0), AxisDirection.down => new Offset(0.0, childMainAxisPosition(child!)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            context.paintChild(child!, offset);
        }
    }

}

