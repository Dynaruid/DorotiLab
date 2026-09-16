// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/debug.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public static partial class DebugLibrary
{
    internal static HSVColor _kDebugDefaultRepaintColor = new HSVColor(0.4, 60.0, 1.0, 1.0);
}

public static partial class DebugLibrary
{
    public static bool debugPaintSizeEnabled = false;
}

public static partial class DebugLibrary
{
    public static bool debugPaintBaselinesEnabled = false;
}

public static partial class DebugLibrary
{
    public static bool debugPaintTextLayoutBoxes = false;
}

public static partial class DebugLibrary
{
    public static bool debugPaintLayerBordersEnabled = false;
}

public static partial class DebugLibrary
{
    public static bool debugPaintPointersEnabled = false;
}

public static partial class DebugLibrary
{
    public static bool debugRepaintRainbowEnabled = false;
}

public static partial class DebugLibrary
{
    public static bool debugRepaintTextRainbowEnabled = false;
}

public static partial class DebugLibrary
{
    public static HSVColor debugCurrentRepaintColor = _kDebugDefaultRepaintColor;
}

public static partial class DebugLibrary
{
    public static bool debugPrintMarkNeedsLayoutStacks = false;
}

public static partial class DebugLibrary
{
    public static bool debugPrintMarkNeedsPaintStacks = false;
}

public static partial class DebugLibrary
{
    public static bool debugPrintLayouts = false;
}

public static partial class DebugLibrary
{
    public static bool debugCheckIntrinsicSizes = false;
}

public static partial class DebugLibrary
{
    public static bool debugProfileLayoutsEnabled = false;
}

public static partial class DebugLibrary
{
    public static bool debugProfilePaintsEnabled = false;
}

public static partial class DebugLibrary
{
    public static bool debugEnhanceLayoutTimelineArguments = false;
}

public static partial class DebugLibrary
{
    public static bool debugEnhancePaintTimelineArguments = false;
}

public delegate void ProfilePaintCallback(RenderObject renderObject);

public static partial class DebugLibrary
{
    public static Action<RenderObject>? debugOnProfilePaint;
}

public static partial class DebugLibrary
{
    public static bool debugDisableClipLayers = false;
}

public static partial class DebugLibrary
{
    public static bool debugDisablePhysicalShapeLayers = false;
}

public static partial class DebugLibrary
{
    public static bool debugDisableOpacityLayers = false;
}

public static partial class DebugLibrary
{
    internal static void _debugDrawDoubleRect(Canvas canvas, Rect outerRect, Rect innerRect, Color color)
    {
        var path = ((Func<Path>)(() =>
{
    var __cascade = new Path();
    __cascade.fillType = PathFillType.evenOdd;
    __cascade.addRect(outerRect);
    __cascade.addRect(innerRect);
    return __cascade;
}))();
        var paint = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.color = color;
    return __cascade;
}))();
        canvas.drawPath(path, paint);
    }
}

public static partial class DebugLibrary
{
    public static void debugPaintPadding(Canvas canvas, Rect outerRect, Rect? innerRect, double outlineWidth = 2.0)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((innerRect is not null) && !DartRuntimePrimitives.RequireValue(innerRect).isEmpty)
                {
                    Rect innerRect__value12483 = DartRuntimePrimitives.RequireValue(innerRect);
                    _debugDrawDoubleRect(canvas, outerRect, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(innerRect__value12483)), new Color(2415956223L));
                    _debugDrawDoubleRect(canvas, DartRuntimePrimitives.RequireValue(innerRect__value12483).inflate(outlineWidth).intersect(outerRect), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(innerRect__value12483)), new Color(4278227199L));
                }
                else
                {
                    var paint = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.color = new Color(2425393296L);
    return __cascade;
}))();
                    canvas.drawRect(outerRect, paint);
                }
                return true;
            });
    }
}

public static partial class DebugLibrary
{
    public static bool debugAssertAllRenderVarsUnset(string reason, bool debugCheckIntrinsicSizesOverride = false)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (debugPaintSizeEnabled || debugPaintBaselinesEnabled || debugPaintLayerBordersEnabled || debugPaintTextLayoutBoxes || debugPaintPointersEnabled || debugRepaintRainbowEnabled || debugRepaintTextRainbowEnabled || (!Equals(debugCurrentRepaintColor, _kDebugDefaultRepaintColor)) || debugPrintMarkNeedsLayoutStacks || debugPrintMarkNeedsPaintStacks || debugPrintLayouts || (debugCheckIntrinsicSizes != debugCheckIntrinsicSizesOverride) || debugProfileLayoutsEnabled || debugProfilePaintsEnabled || (debugOnProfilePaint is not null) || debugDisableClipLayers || debugDisablePhysicalShapeLayers || debugDisableOpacityLayers)
                {
                    throw new FlutterError(reason);
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugCheckHasBoundedAxis(Axis axis, BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!constraints.hasBoundedHeight || !constraints.hasBoundedWidth)
                {
                    switch (axis)
                    {
                        case Axis.vertical:
                            {
                                if (!constraints.hasBoundedHeight)
                                {
                                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Vertical viewport was given unbounded height."), new ErrorDescription("Viewports expand in the scrolling direction to fill their container. " + "In this case, a vertical viewport was given an unlimited amount of " + "vertical space in which to expand. This situation typically happens " + "when a scrollable widget is nested inside another scrollable widget."), new ErrorHint("If this widget is always nested in a scrollable widget there " + "is no need to use a viewport because there will always be enough " + "vertical space for the children. In this case, consider using a " + "Column or Wrap instead. Otherwise, consider using a " + "CustomScrollView to concatenate arbitrary slivers into a " + "single scrollable.") });
                                }
                                if (!constraints.hasBoundedWidth)
                                {
                                    throw new FlutterError("Vertical viewport was given unbounded width.\n" + "Viewports expand in the cross axis to fill their container and " + "constrain their children to match their extent in the cross axis. " + "In this case, a vertical viewport was given an unlimited amount of " + "horizontal space in which to expand.");
                                }
                                break;
                            }
                        case Axis.horizontal:
                            {
                                if (!constraints.hasBoundedWidth)
                                {
                                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Horizontal viewport was given unbounded width."), new ErrorDescription("Viewports expand in the scrolling direction to fill their container. " + "In this case, a horizontal viewport was given an unlimited amount of " + "horizontal space in which to expand. This situation typically happens " + "when a scrollable widget is nested inside another scrollable widget."), new ErrorHint("If this widget is always nested in a scrollable widget there " + "is no need to use a viewport because there will always be enough " + "horizontal space for the children. In this case, consider using a " + "Row or Wrap instead. Otherwise, consider using a " + "CustomScrollView to concatenate arbitrary slivers into a " + "single scrollable.") });
                                }
                                if (!constraints.hasBoundedHeight)
                                {
                                    throw new FlutterError("Horizontal viewport was given unbounded height.\n" + "Viewports expand in the cross axis to fill their container and " + "constrain their children to match their extent in the cross axis. " + "In this case, a horizontal viewport was given an unlimited amount of " + "vertical space in which to expand.");
                                }
                                break;
                            }
                    }
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

