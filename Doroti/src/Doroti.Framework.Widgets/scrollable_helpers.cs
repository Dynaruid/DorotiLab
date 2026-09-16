// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scrollable_helpers.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class ScrollableDetails
{
    public virtual AxisDirection direction { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual Clip? decorationClipBehavior { get; private set; }

    public ScrollableDetails(AxisDirection direction, ScrollController? controller = null, ScrollPhysics? physics = null, Clip? clipBehavior = null, Clip? decorationClipBehavior = null)
    {
        this.direction = direction;
        this.controller = controller;
        this.physics = physics;
        this.decorationClipBehavior = clipBehavior ?? decorationClipBehavior;
    }

    public static ScrollableDetails CreateVertical(bool reverse = false, ScrollController? controller = null, ScrollPhysics? physics = null, Clip? decorationClipBehavior = null)
    {
        var __instance = new ScrollableDetails(default!, controller, physics, default!, decorationClipBehavior);
        __instance.controller = controller;
        __instance.physics = physics;
        __instance.decorationClipBehavior = decorationClipBehavior;
        __instance.direction = reverse ? AxisDirection.up : AxisDirection.down;
        return __instance;
    }

    public static ScrollableDetails CreateHorizontal(bool reverse = false, ScrollController? controller = null, ScrollPhysics? physics = null, Clip? decorationClipBehavior = null)
    {
        var __instance = new ScrollableDetails(default!, controller, physics, default!, decorationClipBehavior);
        __instance.controller = controller;
        __instance.physics = physics;
        __instance.decorationClipBehavior = decorationClipBehavior;
        __instance.direction = reverse ? AxisDirection.left : AxisDirection.right;
        return __instance;
    }

    public virtual Clip? clipBehavior => DartRuntimePrimitives.ConvertValue<Clip>(decorationClipBehavior);
    public virtual ScrollableDetails copyWith(AxisDirection? direction = null, ScrollController? controller = null, ScrollPhysics? physics = null, Clip? decorationClipBehavior = null)
    {
        return new ScrollableDetails(direction: direction ?? this.direction, controller: controller ?? this.controller, physics: physics ?? this.physics, decorationClipBehavior: decorationClipBehavior ?? this.decorationClipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        var description = new List<string>();
        description.Add($"axisDirection: {direction}");
        void addIfNonNull(string prefix, object? value)
        {
            if (value is not null)
            {
                description.Add(prefix + value.ToString());
            }
        }
        addIfNonNull("scroll controller: ", controller);
        addIfNonNull("scroll physics: ", physics);
        addIfNonNull("decorationClipBehavior: ", decorationClipBehavior);
        return $"{DiagnosticsLibrary.describeIdentity(this)}({string.Join(", ", description)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(direction, controller, physics, decorationClipBehavior));
    public override bool Equals(object? other)
    {
        var __other = other as ScrollableDetails;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ScrollableDetails) && Equals(__other.direction, direction) && Equals(__other.controller, controller) && Equals(__other.physics, physics) && Equals(__other.decorationClipBehavior, decorationClipBehavior);
    }

}

public class EdgeDraggingAutoScroller
{
    public virtual ScrollableState scrollable { get; private set; } = default!;
    public virtual Action? onScrollViewScrolled { get; private set; }
    public virtual double velocityScalar { get; private set; } = default!;
    internal virtual Rect _dragTargetRelatedToScrollOrigin { get; set; } = default!;
    internal virtual bool _scrolling { get; set; } = false;

    public EdgeDraggingAutoScroller(ScrollableState scrollable, Action? onScrollViewScrolled = null, double velocityScalar = default!)
    {
        this.scrollable = scrollable;
        this.onScrollViewScrolled = onScrollViewScrolled;
        this.velocityScalar = velocityScalar;
    }

    public virtual bool scrolling => _scrolling;
    internal virtual double _offsetExtent(Offset offset, Axis scrollDirection)
    {
        return scrollDirection switch { Axis.horizontal => offset.dx, Axis.vertical => offset.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _sizeExtent(Size size, Axis scrollDirection)
    {
        return scrollDirection switch { Axis.horizontal => size.width, Axis.vertical => size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual AxisDirection _axisDirection => scrollable.axisDirection;
    internal virtual Axis _scrollDirection => Basic_typesLibrary.axisDirectionToAxis(_axisDirection);
    public virtual void startAutoScrollIfNecessary(Rect dragTarget)
    {
        ScrollPhysics? physics = scrollable.resolvedPhysics;
        if ((physics is not null) && !physics.shouldAcceptUserOffset(scrollable.position))
        {
            stopAutoScroll();
            return;
        }
        Offset deltaToOrigin = scrollable.deltaToScrollOrigin;
        _dragTargetRelatedToScrollOrigin = dragTarget.translate(deltaToOrigin.dx, deltaToOrigin.dy);
        if (_scrolling)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !_scrolling);
        DartRuntimePrimitives.Ignore(_scroll());
    }

    public virtual void stopAutoScroll()
    {
        _scrolling = false;
    }

    internal async virtual Future _scroll()
    {
        var scrollRenderBox = ((RenderBox?)scrollable.context.findRenderObject()!)!;
        Matrix4 transform = scrollRenderBox.getTransformTo(null);
        Rect globalRect = MatrixUtils.transformRect(transform, Rect.fromLTWH(0, 0, scrollRenderBox.size.width, scrollRenderBox.size.height));
        Rect transformedDragTarget = MatrixUtils.transformRect(transform, _dragTargetRelatedToScrollOrigin);
        DartRuntimePrimitives.Assert(() => (globalRect.size.width + Foundation.ConstantsLibrary.precisionErrorTolerance >= transformedDragTarget.size.width) && (globalRect.size.height + Foundation.ConstantsLibrary.precisionErrorTolerance >= transformedDragTarget.size.height), () => (object?)"Drag target size is larger than scrollable size, which may cause bouncing");
        _scrolling = true;
        double? newOffset = default!;
        var overDragMax = 20.0;
        Offset deltaToOrigin = scrollable.deltaToScrollOrigin;
        Offset viewportOrigin = globalRect.topLeft.translate(deltaToOrigin.dx, deltaToOrigin.dy);
        double viewportStart = _offsetExtent(viewportOrigin, _scrollDirection);
        double viewportEnd = viewportStart + _sizeExtent(globalRect.size, _scrollDirection);
        double proxyStart = _offsetExtent(_dragTargetRelatedToScrollOrigin.topLeft, _scrollDirection);
        double proxyEnd = _offsetExtent(_dragTargetRelatedToScrollOrigin.bottomRight, _scrollDirection);
        switch (_axisDirection)
        {
            case AxisDirection.up:
            case AxisDirection.left:
                {
                    if ((proxyEnd > viewportEnd) && (scrollable.position.pixels > scrollable.position.minScrollExtent))
                    {
                        double overDrag = Math.Min(proxyEnd - viewportEnd, overDragMax);
                        newOffset = Math.Max(scrollable.position.minScrollExtent, scrollable.position.pixels - overDrag);
                    }
                    else
                    {
                        if ((proxyStart < viewportStart) && (scrollable.position.pixels < scrollable.position.maxScrollExtent))
                        {
                            double overDragLocal = Math.Min(viewportStart - proxyStart, overDragMax);
                            newOffset = Math.Min(scrollable.position.maxScrollExtent, scrollable.position.pixels + overDragLocal);
                        }
                    }
                    break;
                }
            case AxisDirection.right:
            case AxisDirection.down:
                {
                    if ((proxyStart < viewportStart) && (scrollable.position.pixels > scrollable.position.minScrollExtent))
                    {
                        double overDragAlternate = Math.Min(viewportStart - proxyStart, overDragMax);
                        newOffset = Math.Max(scrollable.position.minScrollExtent, scrollable.position.pixels - overDragAlternate);
                    }
                    else
                    {
                        if ((proxyEnd > viewportEnd) && (scrollable.position.pixels < scrollable.position.maxScrollExtent))
                        {
                            double overDragNested = Math.Min(proxyEnd - viewportEnd, overDragMax);
                            newOffset = Math.Min(scrollable.position.maxScrollExtent, scrollable.position.pixels + overDragNested);
                        }
                    }
                    break;
                }
        }
        if ((newOffset is null) || ((DartRuntimePrimitives.RequireValue(newOffset) - scrollable.position.pixels).abs() < 1.0))
        {
            _scrolling = false;
            return;
        }
        var durationLocal = Duration.Create(milliseconds: (1000L / velocityScalar).round());
        await scrollable.position.animateTo(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(newOffset)), duration: durationLocal, curve: Curves.linear);
        onScrollViewScrolled?.Invoke();
        if (_scrolling)
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
    public virtual AxisDirection direction { get; private set; } = default!;
    public virtual ScrollIncrementType type { get; private set; } = default!;

    public ScrollIntent(AxisDirection direction, ScrollIncrementType type = ScrollIncrementType.line)
    {
        this.direction = direction;
        this.type = type;
    }

}

public class ScrollAction : ContextAction<ScrollIntent>
{
    public override bool isEnabled(ScrollIntent intent, BuildContext? context = null)
    {
        if (context is null)
        {
            return false;
        }
        if (Scrollable.maybeOf(context) is not null)
        {
            return true;
        }
        ScrollController? primaryScrollController = PrimaryScrollController.maybeOf(context);
        return primaryScrollController is not null && primaryScrollController.hasClients;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _calculateScrollIncrement(ScrollableState state, ScrollIncrementType type = ScrollIncrementType.line)
    {
        DartRuntimePrimitives.Assert(() => state.position.hasPixels);
        DartRuntimePrimitives.Assert(() => (state.resolvedPhysics is null) || state.resolvedPhysics!.shouldAcceptUserOffset(state.position));
        if (state.widget.incrementCalculator is not null)
        {
            return state.widget.incrementCalculator!(new ScrollIncrementDetails(type: type, metrics: state.position));
        }
        return type switch { ScrollIncrementType.line => 50.0, ScrollIncrementType.page => 0.8 * state.position.viewportDimension, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static double getDirectionalIncrement(ScrollableState state, ScrollIntent intent)
    {
        if (Equals(Basic_typesLibrary.axisDirectionToAxis(intent.direction), Basic_typesLibrary.axisDirectionToAxis(state.axisDirection)))
        {
            double increment = _calculateScrollIncrement(state, type: intent.type);
            return Equals(intent.direction, state.axisDirection) ? increment : -increment;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? invoke(ScrollIntent intent, BuildContext? context = null)
    {
        DartRuntimePrimitives.Assert(() => context is not null, () => (object?)"Cannot scroll without a context.");
        ArgumentNullException.ThrowIfNull(context);
        ScrollableState? state = Scrollable.maybeOf(context);
        if (state is null)
        {
            ScrollController primaryScrollController = PrimaryScrollController.of(context);
            DartRuntimePrimitives.Assert(() =>
                {
                    if (primaryScrollController.positions.Count() != 1L)
                    {
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A ScrollAction was invoked with the PrimaryScrollController, but " + "more than one ScrollPosition is attached."), new ErrorDescription("Only one ScrollPosition can be manipulated by a ScrollAction at " + "a time."), new ErrorHint("The PrimaryScrollController can be inherited automatically by " + "descendant ScrollViews based on the TargetPlatform and scroll " + "direction. By default, the PrimaryScrollController is " + "automatically inherited on mobile platforms for vertical " + "ScrollViews. ScrollView.primary can also override this behavior.") }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            BuildContext? notificationContextLocal = primaryScrollController.position.context.notificationContext;
            if (notificationContextLocal is not null)
            {
                state = Scrollable.maybeOf(notificationContextLocal);
            }
            if (state is null)
            {
                return default!;
            }
        }
        DartRuntimePrimitives.Assert(() => state.position.hasPixels, () => (object?)"Scrollable must be laid out before it can be scrolled via a ScrollAction");
        if ((state.resolvedPhysics is not null) && !state.resolvedPhysics!.shouldAcceptUserOffset(state.position))
        {
            return default!;
        }
        double increment = getDirectionalIncrement(state, intent);
        if (increment == 0.0)
        {
            return default!;
        }
        DartRuntimePrimitives.Ignore(state.position.moveTo(state.position.pixels + increment, duration: Duration.Create(milliseconds: 100L), curve: Curves.easeInOut));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

