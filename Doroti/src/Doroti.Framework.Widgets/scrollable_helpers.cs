// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scrollable_helpers.dart
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

public class ScrollableDetails
{
    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection direction { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual Clip? decorationClipBehavior { get; private set; }

    public ScrollableDetails(global::Doroti.Generated.Framework.Painting.AxisDirection direction, ScrollController? controller = null, ScrollPhysics? physics = null, Clip? clipBehavior = null, Clip? decorationClipBehavior = null)
    {
        this.direction = direction;
        this.controller = controller;
        this.physics = physics;
        this.decorationClipBehavior = (clipBehavior ?? decorationClipBehavior);
    }

    public static ScrollableDetails CreateVertical(bool reverse = false, ScrollController? controller = null, ScrollPhysics? physics = null, Clip? decorationClipBehavior = null)
    {
        var __instance = new ScrollableDetails(default!, default!, default!, default!, default!);
        __instance.controller = controller;
        __instance.physics = physics;
        __instance.decorationClipBehavior = decorationClipBehavior;
        __instance.direction = (reverse ? global::Doroti.Generated.Framework.Painting.AxisDirection.up : global::Doroti.Generated.Framework.Painting.AxisDirection.down);
        return __instance;
    }

    public static ScrollableDetails CreateHorizontal(bool reverse = false, ScrollController? controller = null, ScrollPhysics? physics = null, Clip? decorationClipBehavior = null)
    {
        var __instance = new ScrollableDetails(default!, default!, default!, default!, default!);
        __instance.controller = controller;
        __instance.physics = physics;
        __instance.decorationClipBehavior = decorationClipBehavior;
        __instance.direction = (reverse ? global::Doroti.Generated.Framework.Painting.AxisDirection.left : global::Doroti.Generated.Framework.Painting.AxisDirection.right);
        return __instance;
    }

    public virtual global::Doroti.Ui.Clip? clipBehavior => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Clip>(this.decorationClipBehavior);
    public virtual ScrollableDetails copyWith(global::Doroti.Generated.Framework.Painting.AxisDirection? direction = null, ScrollController? controller = null, ScrollPhysics? physics = null, Clip? decorationClipBehavior = null)
    {
        return new ScrollableDetails(direction: (direction ?? this.direction), controller: (controller ?? this.controller), physics: (physics ?? this.physics), decorationClipBehavior: (decorationClipBehavior ?? this.decorationClipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        var description__3940 = new List<string>();
        description__3940.Add($"axisDirection: {this.direction}");
        void addIfNonNull(string prefix, object? value)
        {
            if ((value is not null))
            {
                description__3940.Add((prefix + ((string)((dynamic)value).ToString())));
            }
        }
        addIfNonNull("scroll controller: ", this.controller);
        addIfNonNull("scroll physics: ", this.physics);
        addIfNonNull("decorationClipBehavior: ", this.decorationClipBehavior);
        return $"{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({string.Join(", ", description__3940)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.direction, this.controller, this.physics, this.decorationClipBehavior));
    public override bool Equals(object? other)
    {
        var __other = other as ScrollableDetails;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is ScrollableDetails) && (object.Equals(((ScrollableDetails)((ScrollableDetails)__other)).direction, this.direction))) && (object.Equals(((ScrollableDetails)((ScrollableDetails)__other)).controller, this.controller))) && (object.Equals(((ScrollableDetails)((ScrollableDetails)__other)).physics, this.physics))) && (object.Equals(((ScrollableDetails)((ScrollableDetails)__other)).decorationClipBehavior, this.decorationClipBehavior)));
    }

}

public class EdgeDraggingAutoScroller
{
    public virtual ScrollableState scrollable { get; private set; } = default!;
    public virtual global::System.Action? onScrollViewScrolled { get; private set; }
    public virtual double velocityScalar { get; private set; } = default!;
    internal virtual Rect _dragTargetRelatedToScrollOrigin { get; set; } = default!;
    internal virtual bool _scrolling { get; set; } = false;

    public EdgeDraggingAutoScroller(ScrollableState scrollable, global::System.Action? onScrollViewScrolled = null, double velocityScalar = default!)
    {
        this.scrollable = scrollable;
        this.onScrollViewScrolled = onScrollViewScrolled;
        this.velocityScalar = velocityScalar;
    }

    public virtual bool scrolling => this._scrolling;
    internal virtual double _offsetExtent(Offset offset, global::Doroti.Generated.Framework.Painting.Axis scrollDirection)
    {
        return (scrollDirection switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => offset.dx, global::Doroti.Generated.Framework.Painting.Axis.vertical => offset.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _sizeExtent(Size size, global::Doroti.Generated.Framework.Painting.Axis scrollDirection)
    {
        return (scrollDirection switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => size.width, global::Doroti.Generated.Framework.Painting.Axis.vertical => size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.AxisDirection _axisDirection => ((ScrollableState)this.scrollable).axisDirection;
    internal virtual global::Doroti.Generated.Framework.Painting.Axis _scrollDirection => global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(this._axisDirection);
    public virtual void startAutoScrollIfNecessary(Rect dragTarget)
    {
        ScrollPhysics? physics__7416 = ((ScrollableState)this.scrollable).resolvedPhysics;
        if (((physics__7416 is not null) && !physics__7416.shouldAcceptUserOffset(((ScrollableState)this.scrollable).position)))
        {
            stopAutoScroll();
            return;
        }
        global::Doroti.Ui.Offset deltaToOrigin__7598 = ((global::Doroti.Ui.Offset)(object?)((ScrollableState)this.scrollable).deltaToScrollOrigin);
        _dragTargetRelatedToScrollOrigin = dragTarget.translate(deltaToOrigin__7598.dx, deltaToOrigin__7598.dy);
        if (this._scrolling)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !this._scrolling);
        DartRuntimePrimitives.Ignore(_scroll());
    }

    public virtual void stopAutoScroll()
    {
        _scrolling = false;
    }

    internal async virtual Future _scroll()
    {
        var scrollRenderBox__8025 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.scrollable.context.findRenderObject()!)!;
        Matrix4 transform__8114 = ((Matrix4)(object?)scrollRenderBox__8025.getTransformTo(((global::Doroti.Generated.Framework.Rendering.RenderObject)(object)null)));
        global::Doroti.Ui.Rect globalRect__8179 = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform__8114, global::Doroti.Ui.Rect.fromLTWH(0, 0, ((global::Doroti.Generated.Framework.Rendering.RenderBox)scrollRenderBox__8025).size.width, ((global::Doroti.Generated.Framework.Rendering.RenderBox)scrollRenderBox__8025).size.height)));
        global::Doroti.Ui.Rect transformedDragTarget__8342 = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform__8114, this._dragTargetRelatedToScrollOrigin));
        DartRuntimePrimitives.Assert(() => ((((globalRect__8179.size.width + global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)) >= transformedDragTarget__8342.size.width) && (((globalRect__8179.size.height + global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)) >= transformedDragTarget__8342.size.height)), () => (object?)"Drag target size is larger than scrollable size, which may cause bouncing");
        _scrolling = true;
        double? newOffset__8789 = default!;
        var overDragMax__8810 = 20.0;
        global::Doroti.Ui.Offset deltaToOrigin__8848 = ((global::Doroti.Ui.Offset)(object?)((ScrollableState)this.scrollable).deltaToScrollOrigin);
        global::Doroti.Ui.Offset viewportOrigin__8913 = ((global::Doroti.Ui.Offset)(object?)globalRect__8179.topLeft.translate(deltaToOrigin__8848.dx, deltaToOrigin__8848.dy));
        double viewportStart__9013 = _offsetExtent(viewportOrigin__8913, this._scrollDirection);
        double viewportEnd__9095 = (viewportStart__9013 + _sizeExtent(globalRect__8179.size, this._scrollDirection));
        double proxyStart__9191 = _offsetExtent(this._dragTargetRelatedToScrollOrigin.topLeft, this._scrollDirection);
        double proxyEnd__9315 = _offsetExtent(this._dragTargetRelatedToScrollOrigin.bottomRight, this._scrollDirection);
        switch (this._axisDirection)
        {
            case global::Doroti.Generated.Framework.Painting.AxisDirection.up:
            case global::Doroti.Generated.Framework.Painting.AxisDirection.left:
                {
                    if (((proxyEnd__9315 > viewportEnd__9095) && (((ScrollableState)this.scrollable).position.pixels > ((ScrollableState)this.scrollable).position.minScrollExtent)))
                    {
                        double overDrag__9655 = Math.Min((proxyEnd__9315 - viewportEnd__9095), overDragMax__8810);
                        newOffset__8789 = Math.Max(((ScrollableState)this.scrollable).position.minScrollExtent, (((ScrollableState)this.scrollable).position.pixels - overDrag__9655));
                    }
                    else
                    {
                        if (((proxyStart__9191 < viewportStart__9013) && (((ScrollableState)this.scrollable).position.pixels < ((ScrollableState)this.scrollable).position.maxScrollExtent)))
                        {
                            double overDrag__10010 = Math.Min((viewportStart__9013 - proxyStart__9191), overDragMax__8810);
                            newOffset__8789 = Math.Min(((ScrollableState)this.scrollable).position.maxScrollExtent, (((ScrollableState)this.scrollable).position.pixels + overDrag__10010));
                        }
                    }
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.right:
            case global::Doroti.Generated.Framework.Painting.AxisDirection.down:
                {
                    if (((proxyStart__9191 < viewportStart__9013) && (((ScrollableState)this.scrollable).position.pixels > ((ScrollableState)this.scrollable).position.minScrollExtent)))
                    {
                        double overDrag__10435 = Math.Min((viewportStart__9013 - proxyStart__9191), overDragMax__8810);
                        newOffset__8789 = Math.Max(((ScrollableState)this.scrollable).position.minScrollExtent, (((ScrollableState)this.scrollable).position.pixels - overDrag__10435));
                    }
                    else
                    {
                        if (((proxyEnd__9315 > viewportEnd__9095) && (((ScrollableState)this.scrollable).position.pixels < ((ScrollableState)this.scrollable).position.maxScrollExtent)))
                        {
                            double overDrag__10790 = Math.Min((proxyEnd__9315 - viewportEnd__9095), overDragMax__8810);
                            newOffset__8789 = Math.Min(((ScrollableState)this.scrollable).position.maxScrollExtent, (((ScrollableState)this.scrollable).position.pixels + overDrag__10790));
                        }
                    }
                    break;
                }
        }
        if (((newOffset__8789 is null) || (((DartRuntimePrimitives.RequireValue(newOffset__8789) - ((ScrollableState)this.scrollable).position.pixels)).abs() < 1.0)))
        {
            _scrolling = false;
            return;
        }
        var duration__11192 = Duration.Create(milliseconds: ((1000L / this.velocityScalar)).round());
        await ((ScrollableState)this.scrollable).position.animateTo(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(newOffset__8789)), duration: duration__11192, curve: global::Doroti.Generated.Framework.Animation.Curves.linear);
        this.onScrollViewScrolled?.Invoke();
        if (this._scrolling)
        {
            await _scroll();
        }
    }

}

public delegate double ScrollIncrementCalculator(ScrollIncrementDetails details);

public enum ScrollIncrementType
{
    line,
    page
}

public class ScrollIncrementDetails
{
    public virtual ScrollIncrementType type { get; private set; } = default!;
    public virtual ScrollMetrics metrics { get; private set; } = default!;

    public ScrollIncrementDetails(ScrollIncrementType type, ScrollMetrics metrics)
    {
        this.type = type;
        this.metrics = metrics;
    }

}

public class ScrollIntent : Intent
{
    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection direction { get; private set; } = default!;
    public virtual ScrollIncrementType type { get; private set; } = default!;

    public ScrollIntent(global::Doroti.Generated.Framework.Painting.AxisDirection direction, ScrollIncrementType type = ScrollIncrementType.line)
    {
        this.direction = direction;
        this.type = type;
    }

}

public class ScrollAction : ContextAction<ScrollIntent>
{
    public override bool isEnabled(ScrollIntent intent, BuildContext? context = null)
    {
        if ((context is null))
        {
            return false;
        }
        if ((Scrollable.maybeOf(context) is not null))
        {
            return true;
        }
        ScrollController? primaryScrollController__15625 = ((ScrollController?)(object?)PrimaryScrollController.maybeOf(context));
        return (((primaryScrollController__15625 is not null)) && ((ScrollController)primaryScrollController__15625).hasClients);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _calculateScrollIncrement(ScrollableState state, ScrollIncrementType type = ScrollIncrementType.line)
    {
        DartRuntimePrimitives.Assert(() => ((ScrollableState)state).position.hasPixels);
        DartRuntimePrimitives.Assert(() => ((((ScrollableState)state).resolvedPhysics is null) || ((ScrollableState)state).resolvedPhysics!.shouldAcceptUserOffset(((ScrollableState)state).position)));
        if ((state.widget.incrementCalculator is not null))
        {
            return state.widget.incrementCalculator!(new ScrollIncrementDetails(type: type, metrics: ((ScrollableState)state).position));
        }
        return (type switch { ScrollIncrementType.line => 50.0, ScrollIncrementType.page => (0.8 * ((ScrollableState)state).position.viewportDimension), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static double getDirectionalIncrement(ScrollableState state, ScrollIntent intent)
    {
        if ((object.Equals(global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((ScrollIntent)intent).direction), global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((ScrollableState)state).axisDirection))))
        {
            double increment__17112 = ScrollAction._calculateScrollIncrement(state, type: ((ScrollIntent)intent).type);
            return ((object.Equals(((ScrollIntent)intent).direction, ((ScrollableState)state).axisDirection)) ? increment__17112 : -increment__17112);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? invoke(ScrollIntent intent, BuildContext? context = null)
    {
        DartRuntimePrimitives.Assert(() => (context is not null), () => (object?)"Cannot scroll without a context.");
        ScrollableState? state__17443 = ((ScrollableState?)(object?)Scrollable.maybeOf(context!));
        if ((state__17443 is null))
        {
            ScrollController primaryScrollController__17535 = ((ScrollController)(object?)PrimaryScrollController.of(context));
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((((ScrollController)primaryScrollController__17535).positions.Count() != 1L))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("A ScrollAction was invoked with the PrimaryScrollController, but " + "more than one ScrollPosition is attached."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("Only one ScrollPosition can be manipulated by a ScrollAction at " + "a time."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("The PrimaryScrollController can be inherited automatically by " + "descendant ScrollViews based on the TargetPlatform and scroll " + "direction. By default, the PrimaryScrollController is " + "automatically inherited on mobile platforms for vertical " + "ScrollViews. ScrollView.primary can also override this behavior.") }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            BuildContext? notificationContext__18575 = ((ScrollController)primaryScrollController__17535).position.context.notificationContext;
            if ((notificationContext__18575 is not null))
            {
                state__17443 = Scrollable.maybeOf(notificationContext__18575);
            }
            if ((state__17443 is null))
            {
                return default!;
            }
        }
        DartRuntimePrimitives.Assert(() => ((ScrollableState)state__17443).position.hasPixels, () => (object?)"Scrollable must be laid out before it can be scrolled via a ScrollAction");
        if (((((ScrollableState)state__17443).resolvedPhysics is not null) && !((ScrollableState)state__17443).resolvedPhysics!.shouldAcceptUserOffset(((ScrollableState)state__17443).position)))
        {
            return default!;
        }
        double increment__19180 = ScrollAction.getDirectionalIncrement(state__17443, intent);
        if ((increment__19180 == 0.0))
        {
            return default!;
        }
        DartRuntimePrimitives.Ignore(((ScrollableState)state__17443).position.moveTo((((ScrollableState)state__17443).position.pixels + increment__19180), duration: Duration.Create(milliseconds: 100L), curve: global::Doroti.Generated.Framework.Animation.Curves.easeInOut));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

