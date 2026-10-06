// Adapted from flutter_custom_carousel, Copyright (c) 2024, gskinner.com, inc.
using Doroti.Framework.Foundation;
using Doroti.Framework.Gestures;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;

namespace Doroti.CustomCarousel;

public sealed class CustomCarouselScrollable : Scrollable
{
    public double itemExtent { get; }
    public int itemCount { get; }
    public bool loop { get; }
    public bool verticalWheelScroll { get; }

    public CustomCarouselScrollable(double itemExtent, int itemCount,
        Func<BuildContext, ViewportOffset, Widget> viewportBuilder,
        CustomCarouselScrollController controller, ScrollPhysics physics, bool loop = false,
        AxisDirection axisDirection = AxisDirection.down, ScrollBehavior? scrollBehavior = null,
        string? restorationId = null, bool excludeFromSemantics = false, Key? key = null, bool verticalWheelScroll = true)
        : base(key: key, controller: controller, physics: physics, viewportBuilder: viewportBuilder,
            axisDirection: axisDirection, scrollBehavior: scrollBehavior, restorationId: restorationId,
            excludeFromSemantics: excludeFromSemantics, semanticChildCount: itemCount)
    {
        if (!double.IsFinite(itemExtent) || itemExtent <= 0) throw new ArgumentOutOfRangeException(nameof(itemExtent));
        if (itemCount <= 0) throw new ArgumentOutOfRangeException(nameof(itemCount));
        this.itemExtent = itemExtent;
        this.itemCount = itemCount;
        this.loop = loop;
        this.verticalWheelScroll = verticalWheelScroll;
    }

    public override IState createState() => new CustomCarouselScrollableState();
}

public sealed class CustomCarouselScrollableState : ScrollableState
{
    private CustomCarouselScrollable carousel => (CustomCarouselScrollable)widget;
    public double itemExtent => carousel.itemExtent;
    public int itemCount => carousel.itemCount;
    public bool loop => carousel.loop;

    public override Widget build(BuildContext context) => new Listener(
        onPointerSignal: input =>
        {
            if (!carousel.verticalWheelScroll || carousel.axis != Axis.horizontal ||
                input is not PointerScrollEvent wheel || wheel.scrollDelta.dx != 0 || wheel.scrollDelta.dy == 0 ||
                resolvedPhysics?.shouldAcceptUserOffset(position) != true) return;
            var delta = carousel.axisDirection == AxisDirection.left ? -wheel.scrollDelta.dy : wheel.scrollDelta.dy;
            if (Math.Clamp(position.pixels + delta, position.minScrollExtent, position.maxScrollExtent) == position.pixels) return;
            // The inner Scrollable (or a nested child) gets first refusal. This
            // fallback makes an ordinary vertical mouse wheel useful on a desktop
            // horizontal carousel without dispatching the same packet twice.
            GestureBinding.instance.pointerSignalResolver.register(wheel, signal =>
            {
                position.pointerScroll(delta);
                ((PointerScrollEvent)signal).respond(allowPlatformDefault: false);
            });
        }, child: base.build(context));
}
