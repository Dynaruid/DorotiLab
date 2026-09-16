// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/scrollbar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class ScrollbarLibrary
{
    internal static double _kScrollbarMinLength = 36.0;
}

public static partial class ScrollbarLibrary
{
    internal static double _kScrollbarMinOverscrollLength = 8.0;
}

public static partial class ScrollbarLibrary
{
    internal static Duration _kScrollbarTimeToFade = Duration.Create(milliseconds: 1200L);
}

public static partial class ScrollbarLibrary
{
    internal static Duration _kScrollbarFadeDuration = Duration.Create(milliseconds: 250L);
}

public static partial class ScrollbarLibrary
{
    internal static Duration _kScrollbarResizeDuration = Duration.Create(milliseconds: 100L);
}

public static partial class ScrollbarLibrary
{
    internal static Color _kScrollbarColor = new CupertinoDynamicColor(color: new Color(1493172224L), darkColor: new Color(2164260863L));
}

public static partial class ScrollbarLibrary
{
    internal static double _kScrollbarMainAxisMargin = 3.0;
}

public static partial class ScrollbarLibrary
{
    internal static double _kScrollbarCrossAxisMargin = 3.0;
}

public class CupertinoScrollbar : RawScrollbar
{
    public const double defaultThickness = 3;
    public const double defaultThicknessWhileDragging = 8.0;
    public static Radius defaultRadius = Radius.circular(1.5);
    public static Radius defaultRadiusWhileDragging = Radius.circular(4.0);
    public virtual double thicknessWhileDragging { get; private set; } = default!;
    public virtual Radius radiusWhileDragging { get; private set; } = default!;

    public CupertinoScrollbar(Key? key = null, Widget child = default!, ScrollController? controller = null, bool? thumbVisibility = null, double? thickness = null, double? thicknessWhileDragging = null, Radius? radius = null, Radius? radiusWhileDragging = null, Func<ScrollNotification, bool>? notificationPredicate = null, ScrollbarOrientation? scrollbarOrientation = null, double? mainAxisMargin = null) : base(key: key, child: child, controller: controller, thickness: thickness ?? defaultThickness, radius: radius ?? defaultRadius, scrollbarOrientation: scrollbarOrientation, mainAxisMargin: mainAxisMargin ?? ScrollbarLibrary._kScrollbarMainAxisMargin, thumbVisibility: thumbVisibility ?? false, fadeDuration: ScrollbarLibrary._kScrollbarFadeDuration, timeToFade: ScrollbarLibrary._kScrollbarTimeToFade, pressDuration: Duration.Create(milliseconds: 100L), notificationPredicate: notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate)
    {
        double __thicknessWhileDragging = thicknessWhileDragging ?? defaultThicknessWhileDragging;
        Radius __radiusWhileDragging = radiusWhileDragging ?? defaultRadiusWhileDragging;
        this.thicknessWhileDragging = __thicknessWhileDragging;
        this.radiusWhileDragging = __radiusWhileDragging;
        System.Diagnostics.Debug.Assert(thickness < double.PositiveInfinity);
        System.Diagnostics.Debug.Assert(__thicknessWhileDragging < double.PositiveInfinity);
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoScrollbarState__scrollbar());
}

internal class _CupertinoScrollbarState__scrollbar : RawScrollbarState<CupertinoScrollbar>
{
    internal virtual AnimationController _thicknessAnimationController { get; set; } = default!;
    internal virtual double _pressStartAxisPosition { get; set; } = 0.0;

    internal virtual double _thickness
    {
        get
        {
            return DartRuntimePrimitives.RequireValue(widget.thickness) + (_thicknessAnimationController.value * (widget.thicknessWhileDragging - DartRuntimePrimitives.RequireValue(widget.thickness)));
        }
    }
    internal virtual Radius _radius
    {
        get
        {
            return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(widget.radius, widget.radiusWhileDragging, _thicknessAnimationController.value));
        }
    }
    public override void initState()
    {
        base.initState();
        _thicknessAnimationController = new AnimationController(vsync: this, duration: ScrollbarLibrary._kScrollbarResizeDuration);
        _thicknessAnimationController.addListener(() =>
        {
            updateScrollbarPainter();
        });
    }

    public override void updateScrollbarPainter()
    {
        DartRuntimePrimitives.Ignore(((Func<ScrollbarPainter>)(() =>
{
    var __cascade = scrollbarPainter;
    __cascade.color = CupertinoDynamicColor.resolve(ScrollbarLibrary._kScrollbarColor, context);
    __cascade.textDirection = Directionality.of(context);
    __cascade.thickness = _thickness;
    __cascade.mainAxisMargin = widget.mainAxisMargin;
    __cascade.crossAxisMargin = ScrollbarLibrary._kScrollbarCrossAxisMargin;
    __cascade.radius = _radius;
    __cascade.padding = MediaQuery.paddingOf(context);
    __cascade.minLength = ScrollbarLibrary._kScrollbarMinLength;
    __cascade.minOverscrollLength = ScrollbarLibrary._kScrollbarMinOverscrollLength;
    __cascade.scrollbarOrientation = widget.scrollbarOrientation;
    return __cascade;
}))());
    }

    public override void handleThumbPressStart(Offset localPosition)
    {
        base.handleThumbPressStart(localPosition);
        Axis? direction = getScrollbarDirection();
        if (direction is null)
        {
            return;
        }
        _pressStartAxisPosition = DartRuntimePrimitives.RequireValue(direction) switch { Axis.vertical => localPosition.dy, Axis.horizontal => localPosition.dx, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    }

    public override void handleThumbPress()
    {
        if (getScrollbarDirection() is null)
        {
            return;
        }
        base.handleThumbPress();
        DartRuntimePrimitives.Ignore(_thicknessAnimationController.forward().then((_) => HapticFeedback.mediumImpact()));
    }

    public override void handleThumbPressEnd(Offset localPosition, Gestures.Velocity velocity)
    {
        Axis? direction = getScrollbarDirection();
        if (direction is null)
        {
            return;
        }
        _thicknessAnimationController.reverse();
        base.handleThumbPressEnd(localPosition, velocity);
        var (axisPosition, axisVelocity) = DartRuntimePrimitives.RequireValue(direction) switch { Axis.horizontal => (localPosition.dx, velocity.pixelsPerSecond.dx), Axis.vertical => (localPosition.dy, velocity.pixelsPerSecond.dy), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        if ((axisPosition != _pressStartAxisPosition) && (axisVelocity.abs() < 10L))
        {
            DartRuntimePrimitives.Ignore(HapticFeedback.mediumImpact());
        }
    }

    public override void handleTrackTapDown(Gestures.TapDownDetails details)
    {
        if (!Equals(ScrollConfiguration.of(context).getPlatform(context), TargetPlatform.iOS))
        {
            base.handleTrackTapDown(details);
        }
    }

    public override void dispose()
    {
        _thicknessAnimationController.dispose();
        base.dispose();
    }

}
