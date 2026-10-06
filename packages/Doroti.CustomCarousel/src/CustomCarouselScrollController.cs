// Adapted from flutter_custom_carousel, Copyright (c) 2024, gskinner.com, inc.
// Distributed under the BSD 3-Clause license; see ../LICENSE.
using Doroti.Framework.Animation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Runtime;

namespace Doroti.CustomCarousel;

public sealed class CustomCarouselScrollController(int initialItem = 0) : ScrollController
{
    public int initialItem { get; } = initialItem;
    public override CustomCarouselScrollPosition position => (CustomCarouselScrollPosition)base.position;
    public int selectedItem => position.itemIndex;

    public Task animateToItem(int itemIndex, Duration? duration = null, Curve? curve = null) =>
        hasClients ? animateTo(itemIndex * position.itemExtent,
            duration ?? position.defaultDuration(itemIndex * position.itemExtent),
            curve ?? Curves.easeInOutSine).asTask() : Task.CompletedTask;

    public Task nextItem(Duration? duration = null, Curve? curve = null) =>
        hasClients ? animateToItem(selectedItem + 1, duration, curve) : Task.CompletedTask;

    public Task previousItem(Duration? duration = null, Curve? curve = null) =>
        hasClients ? animateToItem(selectedItem - 1, duration, curve) : Task.CompletedTask;

    public void jumpToItem(int itemIndex)
    {
        if (hasClients) position.jumpTo(itemIndex * position.itemExtent);
    }

    public override Future animateTo(double offset, Duration duration, Curve curve) =>
        position.animateTo(offset, duration, curve);

    public override ScrollPosition createScrollPosition(ScrollPhysics physics, ScrollContext context,
        ScrollPosition? oldPosition) => new CustomCarouselScrollPosition(physics, context, initialItem, oldPosition);
}

public sealed class CustomCarouselScrollPosition : ScrollPositionWithSingleContext
{
    private double _lastItemExtent;
    private CustomCarouselScrollableState carouselContext => (CustomCarouselScrollableState)context;
    public double itemExtent => carouselContext.itemExtent;
    public bool loop => carouselContext.loop;
    public int itemIndex => normalizeItem(getItemFromOffset(pixels));

    public CustomCarouselScrollPosition(ScrollPhysics physics, ScrollContext context,
        int initialItem, ScrollPosition? oldPosition = null)
        : base(physics, context, initialPixels: InitialPixels(context, initialItem), oldPosition: oldPosition)
    {
        _lastItemExtent = oldPosition is CustomCarouselScrollPosition old ? old._lastItemExtent : itemExtent;
    }

    private static double InitialPixels(ScrollContext context, int index)
    {
        if (context is not CustomCarouselScrollableState carousel)
            throw new ArgumentException("Use CustomCarouselScrollController with CustomCarouselScrollable.", nameof(context));
        index = carousel.loop ? Modulo(index, carousel.itemCount) : Math.Clamp(index, 0, carousel.itemCount - 1);
        return index * carousel.itemExtent;
    }

    internal static int Modulo(int index, int count) => ((index % count) + count) % count;
    public int normalizeItem(int index) => carouselContext.loop ? Modulo(index, carouselContext.itemCount) : index;
    public double normalizePixels(double value)
    {
        if (!carouselContext.loop) return value;
        var range = itemExtent * carouselContext.itemCount;
        return ((value % range) + range) % range;
    }

    public int getItemFromOffset(double value) => checked((int)Math.Floor(
        (carouselContext.loop ? value : Math.Clamp(value, 0, itemExtent * (carouselContext.itemCount - 1))) / itemExtent + 0.5));

    private double targetPixels(double value)
    {
        if (!double.IsFinite(value)) throw new ArgumentOutOfRangeException(nameof(value));
        if (!carouselContext.loop) return Math.Clamp(value, 0, itemExtent * (carouselContext.itemCount - 1));
        var delta = normalizePixels(value) - normalizePixels(pixels);
        var range = itemExtent * carouselContext.itemCount;
        if (Math.Abs(delta) > range / 2) delta += delta > 0 ? -range : range;
        return pixels + delta;
    }

    public Duration defaultDuration(double to) => Duration.Create(milliseconds:
        (long)Math.Round(Math.Pow(Math.Abs(pixels - targetPixels(to)), 0.6) * 5 + 150));

    public override Future animateTo(double to, Duration duration, Curve curve) =>
        base.animateTo(targetPixels(to), duration, curve);

    // Keep the activity's coordinate continuous across the loop seam. Rendering
    // and itemIndex normalize independently; rebase only after scrolling stops.
    public override void goIdle()
    {
        base.goIdle();
        if (hasPixels && carouselContext.loop) correctPixels(normalizePixels(pixels));
    }

    public override void pointerScroll(double delta)
    {
        if (!double.IsFinite(delta)) throw new ArgumentOutOfRangeException(nameof(delta));
        if (carouselContext.itemCount <= 1) return;
        if (delta == 0) { goBallistic(0); return; }
        var wheel = activity as CarouselWheelScrollActivity;
        var from = wheel?.target ?? pixels;
        var to = carouselContext.loop ? from + delta : Math.Clamp(from + delta, minScrollExtent, maxScrollExtent);
        if (to == from) return;
        if (wheel is null)
        {
            wheel = new CarouselWheelScrollActivity(this, context.vsync);
            beginActivity(wheel);
        }
        updateUserScrollDirection(delta > 0 ? ScrollDirection.reverse : ScrollDirection.forward);
        wheel.updateTarget(to, smooth: Math.Abs(delta) >= 60);
    }
    public override void jumpTo(double value) => base.jumpTo(carouselContext.loop
        ? normalizePixels(value) : Math.Clamp(value, 0, itemExtent * (carouselContext.itemCount - 1)));

    public override bool applyViewportDimension(double viewportDimension)
    {
        if (_lastItemExtent > 0 && itemExtent != _lastItemExtent)
            correctPixels(pixels / _lastItemExtent * itemExtent);
        _lastItemExtent = itemExtent;
        return base.applyViewportDimension(viewportDimension);
    }

    public override FixedExtentMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null,
        double? pixels = null, double? viewportDimension = null, AxisDirection? axisDirection = null,
        double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null,
        double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null) => new(
        minScrollExtent ?? (hasContentDimensions ? this.minScrollExtent : 0),
        maxScrollExtent ?? (hasContentDimensions ? this.maxScrollExtent : 0),
        pixels ?? this.pixels, viewportDimension ?? (hasViewportDimension ? this.viewportDimension : itemExtent),
        axisDirection ?? this.axisDirection, itemIndex ?? this.itemIndex, devicePixelRatio ?? this.devicePixelRatio);
}
