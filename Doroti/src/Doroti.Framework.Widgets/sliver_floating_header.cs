// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver_floating_header.dart
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

namespace Doroti.Generated.Framework.Widgets;

public enum FloatingHeaderSnapMode
{
    overlay,
    scroll
}

public class SliverFloatingHeader : StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.AnimationStyle? animationStyle { get; private set; }
    public virtual FloatingHeaderSnapMode? snapMode { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public SliverFloatingHeader(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Animation.AnimationStyle? animationStyle = null, FloatingHeaderSnapMode? snapMode = null, Widget child = default!) : base(key: key)
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
    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _SliverFloatingHeader__sliver_floating_header(vsync: this, animationStyle: ((SliverFloatingHeader)this.widget).animationStyle, snapMode: ((SliverFloatingHeader)this.widget).snapMode, child: new _SnapTrigger__sliver_floating_header(((SliverFloatingHeader)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._ticker = new global::Doroti.Generated.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        this._tickerModeNotifier = null;
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
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
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
        if ((this.position is not null))
        {
            this.position!.isScrollingNotifier.removeListener(() => this.isScrollingListener());
        }
        position = Scrollable.maybeOf(this.context)?.position;
        if ((this.position is not null))
        {
            this.position!.isScrollingNotifier.addListener(() => this.isScrollingListener());
        }
    }

    public override void dispose()
    {
        if ((this.position is not null))
        {
            this.position!.isScrollingNotifier.removeListener(() => this.isScrollingListener());
        }
        base.dispose();
    }

    public virtual void isScrollingListener()
    {
        DartRuntimePrimitives.Assert(() => (this.position is not null));
        _RenderSliverFloatingHeader__sliver_floating_header? renderer__4860 = ((_RenderSliverFloatingHeader__sliver_floating_header?)(object?)this.context.findAncestorRenderObjectOfType<_RenderSliverFloatingHeader__sliver_floating_header>());
        renderer__4860?.isScrollingUpdate(this.position!);
    }

    public override Widget build(BuildContext context) => ((_SnapTrigger__sliver_floating_header)this.widget).child;
}

internal class _SliverFloatingHeader__sliver_floating_header : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Scheduler.TickerProvider? vsync { get; private set; }
    public virtual global::Doroti.Generated.Framework.Animation.AnimationStyle? animationStyle { get; private set; }
    public virtual FloatingHeaderSnapMode? snapMode { get; private set; }

    internal _SliverFloatingHeader__sliver_floating_header(global::Doroti.Generated.Framework.Scheduler.TickerProvider? vsync = null, global::Doroti.Generated.Framework.Animation.AnimationStyle? animationStyle = null, FloatingHeaderSnapMode? snapMode = null, Widget? child = null) : base(child: child)
    {
        this.vsync = vsync;
        this.animationStyle = animationStyle;
        this.snapMode = snapMode;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderSliverFloatingHeader__sliver_floating_header(vsync: this.vsync, animationStyle: this.animationStyle, snapMode: this.snapMode));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverFloatingHeader__sliver_floating_header)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSliverFloatingHeader__sliver_floating_header>)(() =>
{            var __cascade = __renderObject;
            __cascade.vsync = this.vsync;
            __cascade.animationStyle = this.animationStyle;
            __cascade.snapMode = this.snapMode;
            return __cascade;        }))());
    }

}

public class _RenderSliverFloatingHeader__sliver_floating_header : global::Doroti.Generated.Framework.Rendering.RenderSliverSingleBoxAdapter
{
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> snapAnimation { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController? snapController { get; set; } = default;
    public virtual double? lastScrollOffset { get; set; } = default;
    public virtual double effectiveScrollOffset { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Scheduler.TickerProvider? _vsync { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationStyle? animationStyle { get; set; } = default;
    public virtual FloatingHeaderSnapMode? snapMode { get; set; } = default;

    internal _RenderSliverFloatingHeader__sliver_floating_header(global::Doroti.Generated.Framework.Scheduler.TickerProvider? vsync = null, global::Doroti.Generated.Framework.Animation.AnimationStyle? animationStyle = null, FloatingHeaderSnapMode? snapMode = null)
    {
        this.animationStyle = animationStyle;
        this.snapMode = snapMode;
        this._vsync = vsync;
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.TickerProvider? vsync
    {
        get => this._vsync;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._vsync)))
            {
                return;
            }
            _vsync = __value;
            if ((__value is null))
            {
                this.snapController?.dispose();
                snapController = null;
            }
            else
            {
                this.snapController?.resync(__value);
            }
        }
    }
    public virtual void isScrollingUpdate(ScrollPosition position)
    {
        if (((ScrollPosition)position).isScrollingNotifier.value)
        {
            this.snapController?.stop();
        }
        else
        {
            global::Doroti.Generated.Framework.Rendering.ScrollDirection direction__7176 = position.userScrollDirection;
            bool headerIsPartiallyVisible__7235 = (direction__7176 switch { global::Doroti.Generated.Framework.Rendering.ScrollDirection.forward when ((this.effectiveScrollOffset <= 0L)) => false, global::Doroti.Generated.Framework.Rendering.ScrollDirection.reverse when ((this.effectiveScrollOffset >= this.childExtent)) => false, _ => true });
            if (headerIsPartiallyVisible__7235)
            {
                snapController ??= ((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Animation.AnimationController(vsync: this.vsync!);
            __cascade.addListener(((global::System.Action)(() => {
if ((this.effectiveScrollOffset != ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.snapAnimation).value))
{
    effectiveScrollOffset = ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.snapAnimation).value;
    markNeedsLayout();
}
})));
            return __cascade;        }))();
                this.snapController!.duration = (direction__7176 switch { global::Doroti.Generated.Framework.Rendering.ScrollDirection.forward => (this.animationStyle?.duration ?? Duration.Create(milliseconds: 300L)), _ => (this.animationStyle?.reverseDuration ?? Duration.Create(milliseconds: 300L)) });
                snapAnimation = this.snapController!.drive(new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: this.effectiveScrollOffset, end: (direction__7176 switch { global::Doroti.Generated.Framework.Rendering.ScrollDirection.forward => 0, _ => this.childExtent })).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: (direction__7176 switch { global::Doroti.Generated.Framework.Rendering.ScrollDirection.forward => (this.animationStyle?.curve ?? global::Doroti.Generated.Framework.Animation.Curves.easeInOut), _ => (this.animationStyle?.reverseCurve ?? global::Doroti.Generated.Framework.Animation.Curves.easeInOut) }))));
                this.snapController!.forward(from: 0.0);
            }
        }
    }

    public virtual double childExtent
    {
        get
        {
            if ((this.child is null))
            {
                return 0.0;
            }
            DartRuntimePrimitives.Assert(() => this.child!.hasSize);
            return (((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).axis switch { global::Doroti.Generated.Framework.Painting.Axis.vertical => this.child!.size.height, global::Doroti.Generated.Framework.Painting.Axis.horizontal => this.child!.size.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public virtual void detach()
    {
        this.snapController?.dispose();
        snapController = null;
        base.detach();
    }

    public virtual bool floatingHeaderNeedsToBeUpdated
    {
        get
        {
            return ((this.lastScrollOffset is not null) && (((((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset < DartRuntimePrimitives.RequireValue(this.lastScrollOffset)) || (this.effectiveScrollOffset < this.childExtent))));
            return default!;
        }
    }
    public override void performLayout()
    {
        if (!this.floatingHeaderNeedsToBeUpdated)
        {
            effectiveScrollOffset = ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset;
        }
        else
        {
            double delta__9782 = (DartRuntimePrimitives.RequireValue(this.lastScrollOffset) - ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset);
            if ((object.Equals(((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).userScrollDirection, global::Doroti.Generated.Framework.Rendering.ScrollDirection.forward)))
            {
                if ((this.effectiveScrollOffset > this.childExtent))
                {
                    effectiveScrollOffset = this.childExtent;
                }
            }
            else
            {
                delta__9782 = Dart_uiLibrary.clampDouble(delta__9782, -double.PositiveInfinity, 0);
            }
            effectiveScrollOffset = Dart_uiLibrary.clampDouble((this.effectiveScrollOffset - delta__9782), 0.0, ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset);
        }
        this.child?.layout(this.constraints.asBoxConstraints(), parentUsesSize: true);
        double paintExtent__10543 = (this.childExtent - this.effectiveScrollOffset);
        double layoutExtent__10611 = ((this.snapMode ?? FloatingHeaderSnapMode.overlay) switch { FloatingHeaderSnapMode.overlay => (this.childExtent - ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset), FloatingHeaderSnapMode.scroll => paintExtent__10543, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        geometry = new global::Doroti.Generated.Framework.Rendering.SliverGeometry(paintOrigin: Math.Min(((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).overlap, 0.0), scrollExtent: this.childExtent, paintExtent: Dart_uiLibrary.clampDouble(paintExtent__10543, 0.0, ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).remainingPaintExtent), layoutExtent: Dart_uiLibrary.clampDouble(layoutExtent__10611, 0.0, ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).remainingPaintExtent), maxPaintExtent: this.childExtent, hasVisualOverflow: true);
        lastScrollOffset = ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset;
    }

    public override double childMainAxisPosition(global::Doroti.Generated.Framework.Rendering.RenderObject child)
    {
        return ((this.geometry is null) ? 0 : Math.Min(0, (this.geometry!.paintExtent - this.childExtent)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(global::Doroti.Generated.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child, ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child))));
        applyPaintTransformForBoxChild(((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)child)!, transform);
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (((this.child is not null) && this.geometry!.visible))
        {
            offset += (global::Doroti.Generated.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).axisDirection, ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).growthDirection) switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => new global::Doroti.Ui.Offset(0.0, ((this.geometry!.paintExtent - childMainAxisPosition(this.child!)) - this.childExtent)), global::Doroti.Generated.Framework.Painting.AxisDirection.left => new global::Doroti.Ui.Offset(((this.geometry!.paintExtent - childMainAxisPosition(this.child!)) - this.childExtent), 0.0), global::Doroti.Generated.Framework.Painting.AxisDirection.right => new global::Doroti.Ui.Offset(childMainAxisPosition(this.child!), 0.0), global::Doroti.Generated.Framework.Painting.AxisDirection.down => new global::Doroti.Ui.Offset(0.0, childMainAxisPosition(this.child!)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            context.paintChild(this.child!, offset);
        }
    }

}

