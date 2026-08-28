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

namespace Doroti.Framework.Widgets;

public class ScrollableDetails
{
    public virtual global::Doroti.Framework.Painting.AxisDirection direction { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual Clip? decorationClipBehavior { get; private set; }

    public ScrollableDetails(global::Doroti.Framework.Painting.AxisDirection direction, ScrollController? controller = null, ScrollPhysics? physics = null, Clip? clipBehavior = null, Clip? decorationClipBehavior = null)
    {
        this.direction = direction;
        this.controller = controller;
        this.physics = physics;
        this.decorationClipBehavior = (clipBehavior ?? decorationClipBehavior);
    }

    public static ScrollableDetails CreateVertical(bool reverse = false, ScrollController? controller = null, ScrollPhysics? physics = null, Clip? decorationClipBehavior = null)
    {
        var __instance = new ScrollableDetails(default!, controller, physics, default!, decorationClipBehavior);
        __instance.controller = controller;
        __instance.physics = physics;
        __instance.decorationClipBehavior = decorationClipBehavior;
        __instance.direction = (reverse ? global::Doroti.Framework.Painting.AxisDirection.up : global::Doroti.Framework.Painting.AxisDirection.down);
        return __instance;
    }

    public static ScrollableDetails CreateHorizontal(bool reverse = false, ScrollController? controller = null, ScrollPhysics? physics = null, Clip? decorationClipBehavior = null)
    {
        var __instance = new ScrollableDetails(default!, controller, physics, default!, decorationClipBehavior);
        __instance.controller = controller;
        __instance.physics = physics;
        __instance.decorationClipBehavior = decorationClipBehavior;
        __instance.direction = (reverse ? global::Doroti.Framework.Painting.AxisDirection.left : global::Doroti.Framework.Painting.AxisDirection.right);
        return __instance;
    }

    public virtual global::Doroti.Ui.Clip? clipBehavior => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Clip>(this.decorationClipBehavior);
    public virtual ScrollableDetails copyWith(global::Doroti.Framework.Painting.AxisDirection? direction = null, ScrollController? controller = null, ScrollPhysics? physics = null, Clip? decorationClipBehavior = null)
    {
        return new ScrollableDetails(direction: (direction ?? this.direction), controller: (controller ?? this.controller), physics: (physics ?? this.physics), decorationClipBehavior: (decorationClipBehavior ?? this.decorationClipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        var description = new List<string>();
        description.Add($"axisDirection: {this.direction}");
        void addIfNonNull(string prefix, object? value)
        {
            if ((value is not null))
            {
                description.Add((prefix + ((string)((dynamic)value).ToString())));
            }
        }
        addIfNonNull("scroll controller: ", this.controller);
        addIfNonNull("scroll physics: ", this.physics);
        addIfNonNull("decorationClipBehavior: ", this.decorationClipBehavior);
        return $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({string.Join(", ", description)})";
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
    internal virtual double _offsetExtent(Offset offset, global::Doroti.Framework.Painting.Axis scrollDirection)
    {
        return (scrollDirection switch { global::Doroti.Framework.Painting.Axis.horizontal => offset.dx, global::Doroti.Framework.Painting.Axis.vertical => offset.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _sizeExtent(Size size, global::Doroti.Framework.Painting.Axis scrollDirection)
    {
        return (scrollDirection switch { global::Doroti.Framework.Painting.Axis.horizontal => size.width, global::Doroti.Framework.Painting.Axis.vertical => size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.AxisDirection _axisDirection => ((ScrollableState)this.scrollable).axisDirection;
    internal virtual global::Doroti.Framework.Painting.Axis _scrollDirection => global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(this._axisDirection);
    public virtual void startAutoScrollIfNecessary(Rect dragTarget)
    {
        ScrollPhysics? physics = ((ScrollableState)this.scrollable).resolvedPhysics;
        if (((physics is not null) && !physics.shouldAcceptUserOffset(((ScrollableState)this.scrollable).position)))
        {
            stopAutoScroll();
            return;
        }
        global::Doroti.Ui.Offset deltaToOrigin = ((global::Doroti.Ui.Offset)(object?)((ScrollableState)this.scrollable).deltaToScrollOrigin);
        _dragTargetRelatedToScrollOrigin = dragTarget.translate(deltaToOrigin.dx, deltaToOrigin.dy);
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
        var scrollRenderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.scrollable.context.findRenderObject()!)!;
        Matrix4 transform = ((Matrix4)(object?)scrollRenderBox.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
        global::Doroti.Ui.Rect globalRect = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform, global::Doroti.Ui.Rect.fromLTWH(0, 0, ((global::Doroti.Framework.Rendering.RenderBox)scrollRenderBox).size.width, ((global::Doroti.Framework.Rendering.RenderBox)scrollRenderBox).size.height)));
        global::Doroti.Ui.Rect transformedDragTarget = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform, this._dragTargetRelatedToScrollOrigin));
        DartRuntimePrimitives.Assert(() => ((((globalRect.size.width + global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)) >= transformedDragTarget.size.width) && (((globalRect.size.height + global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)) >= transformedDragTarget.size.height)), () => (object?)"Drag target size is larger than scrollable size, which may cause bouncing");
        _scrolling = true;
        double? newOffset = default!;
        var overDragMax = 20.0;
        global::Doroti.Ui.Offset deltaToOrigin = ((global::Doroti.Ui.Offset)(object?)((ScrollableState)this.scrollable).deltaToScrollOrigin);
        global::Doroti.Ui.Offset viewportOrigin = ((global::Doroti.Ui.Offset)(object?)globalRect.topLeft.translate(deltaToOrigin.dx, deltaToOrigin.dy));
        double viewportStart = _offsetExtent(viewportOrigin, this._scrollDirection);
        double viewportEnd = (viewportStart + _sizeExtent(globalRect.size, this._scrollDirection));
        double proxyStart = _offsetExtent(this._dragTargetRelatedToScrollOrigin.topLeft, this._scrollDirection);
        double proxyEnd = _offsetExtent(this._dragTargetRelatedToScrollOrigin.bottomRight, this._scrollDirection);
        switch (this._axisDirection)
        {
            case global::Doroti.Framework.Painting.AxisDirection.up:
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    if (((proxyEnd > viewportEnd) && (((ScrollableState)this.scrollable).position.pixels > ((ScrollableState)this.scrollable).position.minScrollExtent)))
                    {
                        double overDrag = Math.Min((proxyEnd - viewportEnd), overDragMax);
                        newOffset = Math.Max(((ScrollableState)this.scrollable).position.minScrollExtent, (((ScrollableState)this.scrollable).position.pixels - overDrag));
                    }
                    else
                    {
                        if (((proxyStart < viewportStart) && (((ScrollableState)this.scrollable).position.pixels < ((ScrollableState)this.scrollable).position.maxScrollExtent)))
                        {
                            double overDragLocal = Math.Min((viewportStart - proxyStart), overDragMax);
                            newOffset = Math.Min(((ScrollableState)this.scrollable).position.maxScrollExtent, (((ScrollableState)this.scrollable).position.pixels + overDragLocal));
                        }
                    }
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.right:
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    if (((proxyStart < viewportStart) && (((ScrollableState)this.scrollable).position.pixels > ((ScrollableState)this.scrollable).position.minScrollExtent)))
                    {
                        double overDragAlternate = Math.Min((viewportStart - proxyStart), overDragMax);
                        newOffset = Math.Max(((ScrollableState)this.scrollable).position.minScrollExtent, (((ScrollableState)this.scrollable).position.pixels - overDragAlternate));
                    }
                    else
                    {
                        if (((proxyEnd > viewportEnd) && (((ScrollableState)this.scrollable).position.pixels < ((ScrollableState)this.scrollable).position.maxScrollExtent)))
                        {
                            double overDragNested = Math.Min((proxyEnd - viewportEnd), overDragMax);
                            newOffset = Math.Min(((ScrollableState)this.scrollable).position.maxScrollExtent, (((ScrollableState)this.scrollable).position.pixels + overDragNested));
                        }
                    }
                    break;
                }
        }
        if (((newOffset is null) || (((DartRuntimePrimitives.RequireValue(newOffset) - ((ScrollableState)this.scrollable).position.pixels)).abs() < 1.0)))
        {
            _scrolling = false;
            return;
        }
        var durationLocal = Duration.Create(milliseconds: ((1000L / this.velocityScalar)).round());
        await ((ScrollableState)this.scrollable).position.animateTo(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(newOffset)), duration: durationLocal, curve: global::Doroti.Framework.Animation.Curves.linear);
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
    public virtual global::Doroti.Framework.Painting.AxisDirection direction { get; private set; } = default!;
    public virtual ScrollIncrementType type { get; private set; } = default!;

    public ScrollIntent(global::Doroti.Framework.Painting.AxisDirection direction, ScrollIncrementType type = ScrollIncrementType.line)
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
        ScrollController? primaryScrollController = ((ScrollController?)(object?)PrimaryScrollController.maybeOf(context));
        return (((primaryScrollController is not null)) && ((ScrollController)primaryScrollController).hasClients);
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
        if ((object.Equals(global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((ScrollIntent)intent).direction), global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((ScrollableState)state).axisDirection))))
        {
            double increment = ScrollAction._calculateScrollIncrement(state, type: ((ScrollIntent)intent).type);
            return ((object.Equals(((ScrollIntent)intent).direction, ((ScrollableState)state).axisDirection)) ? increment : -increment);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? invoke(ScrollIntent intent, BuildContext? context = null)
    {
        DartRuntimePrimitives.Assert(() => (context is not null), () => (object?)"Cannot scroll without a context.");
        ScrollableState? state = ((ScrollableState?)(object?)Scrollable.maybeOf(context!));
        if ((state is null))
        {
            ScrollController primaryScrollController = ((ScrollController)(object?)PrimaryScrollController.of(context));
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((((ScrollController)primaryScrollController).positions.Count() != 1L))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("A ScrollAction was invoked with the PrimaryScrollController, but " + "more than one ScrollPosition is attached."), new global::Doroti.Framework.Foundation.ErrorDescription("Only one ScrollPosition can be manipulated by a ScrollAction at " + "a time."), new global::Doroti.Framework.Foundation.ErrorHint("The PrimaryScrollController can be inherited automatically by " + "descendant ScrollViews based on the TargetPlatform and scroll " + "direction. By default, the PrimaryScrollController is " + "automatically inherited on mobile platforms for vertical " + "ScrollViews. ScrollView.primary can also override this behavior.") }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            BuildContext? notificationContextLocal = ((ScrollController)primaryScrollController).position.context.notificationContext;
            if ((notificationContextLocal is not null))
            {
                state = Scrollable.maybeOf(notificationContextLocal);
            }
            if ((state is null))
            {
                return default!;
            }
        }
        DartRuntimePrimitives.Assert(() => ((ScrollableState)state).position.hasPixels, () => (object?)"Scrollable must be laid out before it can be scrolled via a ScrollAction");
        if (((((ScrollableState)state).resolvedPhysics is not null) && !((ScrollableState)state).resolvedPhysics!.shouldAcceptUserOffset(((ScrollableState)state).position)))
        {
            return default!;
        }
        double increment = ScrollAction.getDirectionalIncrement(state, intent);
        if ((increment == 0.0))
        {
            return default!;
        }
        DartRuntimePrimitives.Ignore(((ScrollableState)state).position.moveTo((((ScrollableState)state).position.pixels + increment), duration: Duration.Create(milliseconds: 100L), curve: global::Doroti.Framework.Animation.Curves.easeInOut));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

