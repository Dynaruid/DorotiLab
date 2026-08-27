// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/debug.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Rendering;

public static partial class DebugLibrary
{
    internal static global::Doroti.Framework.Painting.HSVColor _kDebugDefaultRepaintColor = new global::Doroti.Framework.Painting.HSVColor(0.4, 60.0, 1.0, 1.0);
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
    public static global::Doroti.Framework.Painting.HSVColor debugCurrentRepaintColor = DebugLibrary._kDebugDefaultRepaintColor;
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
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.fillType = PathFillType.evenOdd;
    __cascade.addRect(outerRect);
    __cascade.addRect(innerRect);
    return __cascade;
}))();
        var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
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
                if (((innerRect is not null) && !DartRuntimePrimitives.RequireValue(innerRect).isEmpty))
                {
                    Rect innerRect__value12483 = DartRuntimePrimitives.RequireValue(innerRect);
                    DebugLibrary._debugDrawDoubleRect(canvas, outerRect, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(innerRect__value12483)), new global::Doroti.Ui.Color(2415956223L));
                    DebugLibrary._debugDrawDoubleRect(canvas, DartRuntimePrimitives.RequireValue(innerRect__value12483).inflate(outlineWidth).intersect(outerRect), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(innerRect__value12483)), new global::Doroti.Ui.Color(4278227199L));
                }
                else
                {
                    var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(2425393296L);
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
                if ((((((((((((((((((DebugLibrary.debugPaintSizeEnabled || DebugLibrary.debugPaintBaselinesEnabled) || DebugLibrary.debugPaintLayerBordersEnabled) || DebugLibrary.debugPaintTextLayoutBoxes) || DebugLibrary.debugPaintPointersEnabled) || DebugLibrary.debugRepaintRainbowEnabled) || DebugLibrary.debugRepaintTextRainbowEnabled) || (!object.Equals(DebugLibrary.debugCurrentRepaintColor, DebugLibrary._kDebugDefaultRepaintColor))) || DebugLibrary.debugPrintMarkNeedsLayoutStacks) || DebugLibrary.debugPrintMarkNeedsPaintStacks) || DebugLibrary.debugPrintLayouts) || (DebugLibrary.debugCheckIntrinsicSizes != debugCheckIntrinsicSizesOverride)) || DebugLibrary.debugProfileLayoutsEnabled) || DebugLibrary.debugProfilePaintsEnabled) || (DebugLibrary.debugOnProfilePaint is not null)) || DebugLibrary.debugDisableClipLayers) || DebugLibrary.debugDisablePhysicalShapeLayers) || DebugLibrary.debugDisableOpacityLayers))
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
    public static bool debugCheckHasBoundedAxis(global::Doroti.Framework.Painting.Axis axis, BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((!((BoxConstraints)constraints).hasBoundedHeight || !((BoxConstraints)constraints).hasBoundedWidth))
                {
                    switch (axis)
                    {
                        case global::Doroti.Framework.Painting.Axis.vertical:
                            {
                                if (!((BoxConstraints)constraints).hasBoundedHeight)
                                {
                                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Vertical viewport was given unbounded height."), new ErrorDescription("Viewports expand in the scrolling direction to fill their container. " + "In this case, a vertical viewport was given an unlimited amount of " + "vertical space in which to expand. This situation typically happens " + "when a scrollable widget is nested inside another scrollable widget."), new ErrorHint("If this widget is always nested in a scrollable widget there " + "is no need to use a viewport because there will always be enough " + "vertical space for the children. In this case, consider using a " + "Column or Wrap instead. Otherwise, consider using a " + "CustomScrollView to concatenate arbitrary slivers into a " + "single scrollable.") });
                                }
                                if (!((BoxConstraints)constraints).hasBoundedWidth)
                                {
                                    throw new FlutterError("Vertical viewport was given unbounded width.\n" + "Viewports expand in the cross axis to fill their container and " + "constrain their children to match their extent in the cross axis. " + "In this case, a vertical viewport was given an unlimited amount of " + "horizontal space in which to expand.");
                                }
                                break;
                            }
                        case global::Doroti.Framework.Painting.Axis.horizontal:
                            {
                                if (!((BoxConstraints)constraints).hasBoundedWidth)
                                {
                                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Horizontal viewport was given unbounded width."), new ErrorDescription("Viewports expand in the scrolling direction to fill their container. " + "In this case, a horizontal viewport was given an unlimited amount of " + "horizontal space in which to expand. This situation typically happens " + "when a scrollable widget is nested inside another scrollable widget."), new ErrorHint("If this widget is always nested in a scrollable widget there " + "is no need to use a viewport because there will always be enough " + "horizontal space for the children. In this case, consider using a " + "Row or Wrap instead. Otherwise, consider using a " + "CustomScrollView to concatenate arbitrary slivers into a " + "single scrollable.") });
                                }
                                if (!((BoxConstraints)constraints).hasBoundedHeight)
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

