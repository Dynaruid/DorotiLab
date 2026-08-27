// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/scrollbar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
    internal static Color _kScrollbarColor = ((Color)(object?)new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(1493172224L), darkColor: new global::Doroti.Ui.Color(2164260863L)));
}

public static partial class ScrollbarLibrary
{
    internal static double _kScrollbarMainAxisMargin = 3.0;
}

public static partial class ScrollbarLibrary
{
    internal static double _kScrollbarCrossAxisMargin = 3.0;
}

public class CupertinoScrollbar : global::Doroti.Framework.Widgets.RawScrollbar
{
    public const double defaultThickness = 3;
    public const double defaultThicknessWhileDragging = 8.0;
    public static Radius defaultRadius = global::Doroti.Ui.Radius.circular(1.5);
    public static Radius defaultRadiusWhileDragging = global::Doroti.Ui.Radius.circular(4.0);
    public virtual double thicknessWhileDragging { get; private set; } = default!;
    public virtual Radius radiusWhileDragging { get; private set; } = default!;

    public CupertinoScrollbar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget child = default!, global::Doroti.Framework.Widgets.ScrollController? controller = null, bool? thumbVisibility = null, double? thickness = null, double? thicknessWhileDragging = null, Radius? radius = null, Radius? radiusWhileDragging = null, global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool>? notificationPredicate = null, global::Doroti.Framework.Widgets.ScrollbarOrientation? scrollbarOrientation = null, double? mainAxisMargin = null) : base(key: key, child: child, controller: controller, thickness: thickness ?? defaultThickness, radius: radius ?? defaultRadius, scrollbarOrientation: scrollbarOrientation, mainAxisMargin: mainAxisMargin ?? ScrollbarLibrary._kScrollbarMainAxisMargin, thumbVisibility: (thumbVisibility ?? false), fadeDuration: ScrollbarLibrary._kScrollbarFadeDuration, timeToFade: ScrollbarLibrary._kScrollbarTimeToFade, pressDuration: Duration.Create(milliseconds: 100L), notificationPredicate: ((notificationPredicate ?? (global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool>)global::Doroti.Framework.Widgets.Scroll_notificationLibrary.defaultScrollNotificationPredicate)))
    {
        double __thicknessWhileDragging = thicknessWhileDragging ?? defaultThicknessWhileDragging;
        Radius __radiusWhileDragging = radiusWhileDragging ?? defaultRadiusWhileDragging;
        this.thicknessWhileDragging = __thicknessWhileDragging;
        this.radiusWhileDragging = __radiusWhileDragging;
        System.Diagnostics.Debug.Assert((thickness < double.PositiveInfinity));
        System.Diagnostics.Debug.Assert((__thicknessWhileDragging < double.PositiveInfinity));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoScrollbarState__scrollbar());
}

internal class _CupertinoScrollbarState__scrollbar : global::Doroti.Framework.Widgets.RawScrollbarState<CupertinoScrollbar>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _thicknessAnimationController { get; set; } = default!;
    internal virtual double _pressStartAxisPosition { get; set; } = 0.0;

    internal virtual double _thickness
    {
        get
        {
            return (DartRuntimePrimitives.RequireValue(this.widget.thickness) + (((global::Doroti.Framework.Animation.AnimationController)this._thicknessAnimationController).value * ((((CupertinoScrollbar)this.widget).thicknessWhileDragging - DartRuntimePrimitives.RequireValue(this.widget.thickness)))));
            return default!;
        }
    }
    internal virtual global::Doroti.Ui.Radius _radius
    {
        get
        {
            return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(this.widget.radius, ((CupertinoScrollbar)this.widget).radiusWhileDragging, ((global::Doroti.Framework.Animation.AnimationController)this._thicknessAnimationController).value));
            return default!;
        }
    }
    public override void initState()
    {
        base.initState();
        _thicknessAnimationController = new global::Doroti.Framework.Animation.AnimationController(vsync: this, duration: ScrollbarLibrary._kScrollbarResizeDuration);
        this._thicknessAnimationController.addListener(((global::System.Action)(() =>
        {
            updateScrollbarPainter();
        })));
    }

    public override void updateScrollbarPainter()
    {
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Widgets.ScrollbarPainter>)(() =>
{
    var __cascade = this.scrollbarPainter;
    __cascade.color = CupertinoDynamicColor.resolve(ScrollbarLibrary._kScrollbarColor, this.context);
    __cascade.textDirection = Directionality.of(this.context);
    __cascade.thickness = this._thickness;
    __cascade.mainAxisMargin = this.widget.mainAxisMargin;
    __cascade.crossAxisMargin = ScrollbarLibrary._kScrollbarCrossAxisMargin;
    __cascade.radius = this._radius;
    __cascade.padding = MediaQuery.paddingOf(this.context);
    __cascade.minLength = ScrollbarLibrary._kScrollbarMinLength;
    __cascade.minOverscrollLength = ScrollbarLibrary._kScrollbarMinOverscrollLength;
    __cascade.scrollbarOrientation = this.widget.scrollbarOrientation;
    return __cascade;
}))());
    }

    public override void handleThumbPressStart(Offset localPosition)
    {
        base.handleThumbPressStart(localPosition);
        global::Doroti.Framework.Painting.Axis? direction = getScrollbarDirection();
        if ((direction is null))
        {
            return;
        }
        _pressStartAxisPosition = (DartRuntimePrimitives.RequireValue(direction) switch { global::Doroti.Framework.Painting.Axis.vertical => localPosition.dy, global::Doroti.Framework.Painting.Axis.horizontal => localPosition.dx, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    }

    public override void handleThumbPress()
    {
        if ((getScrollbarDirection() is null))
        {
            return;
        }
        base.handleThumbPress();
        DartRuntimePrimitives.Ignore(this._thicknessAnimationController.forward().then(((global::System.Func<object?, object>)((_) => HapticFeedback.mediumImpact()))));
    }

    public override void handleThumbPressEnd(Offset localPosition, global::Doroti.Framework.Gestures.Velocity velocity)
    {
        global::Doroti.Framework.Painting.Axis? direction = getScrollbarDirection();
        if ((direction is null))
        {
            return;
        }
        this._thicknessAnimationController.reverse();
        base.handleThumbPressEnd(localPosition, velocity);
        var (axisPosition, axisVelocity) = (DartRuntimePrimitives.RequireValue(direction) switch { global::Doroti.Framework.Painting.Axis.horizontal => (((double, double))((localPosition.dx, ((global::Doroti.Framework.Gestures.Velocity)velocity).pixelsPerSecond.dx))), global::Doroti.Framework.Painting.Axis.vertical => (((double, double))((localPosition.dy, ((global::Doroti.Framework.Gestures.Velocity)velocity).pixelsPerSecond.dy))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if (((axisPosition != this._pressStartAxisPosition) && (axisVelocity.abs() < 10L)))
        {
            DartRuntimePrimitives.Ignore(HapticFeedback.mediumImpact());
        }
    }

    public override void handleTrackTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        if ((!object.Equals(ScrollConfiguration.of(this.context).getPlatform(this.context), global::Doroti.Framework.Foundation.TargetPlatform.iOS)))
        {
            base.handleTrackTapDown(details);
        }
    }

    public override void dispose()
    {
        this._thicknessAnimationController.dispose();
        base.dispose();
    }

}
