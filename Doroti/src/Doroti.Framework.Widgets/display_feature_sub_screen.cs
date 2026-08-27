// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/display_feature_sub_screen.dart
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

public class DisplayFeatureSubScreen : StatelessWidget
{
    public virtual Offset? anchorPoint { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public DisplayFeatureSubScreen(global::Doroti.Framework.Foundation.Key? key = null, Offset? anchorPoint = null, Widget child = default!) : base(key: key)
    {
        this.anchorPoint = anchorPoint;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => ((this.anchorPoint is not null) || global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context, why: "to determine which sub-screen DisplayFeatureSubScreen uses", alternative: "Alternatively, consider specifying the 'anchorPoint' argument on the DisplayFeatureSubScreen.")));
        MediaQueryData mediaQuery = ((MediaQueryData)(object?)MediaQuery.of(context));
        global::Doroti.Ui.Size parentSize = ((global::Doroti.Ui.Size)(object?)((MediaQueryData)mediaQuery).size);
        global::Doroti.Ui.Rect wantedBounds = ((global::Doroti.Ui.Rect)(object?)(Offset.zero & parentSize));
        global::Doroti.Ui.Offset resolvedAnchorPoint = ((global::Doroti.Ui.Offset)(object?)DisplayFeatureSubScreen._capOffset(((this.anchorPoint ?? (Offset)DisplayFeatureSubScreen._fallbackAnchorPoint(context))), parentSize));
        IEnumerable<global::Doroti.Ui.Rect> subScreens = ((IEnumerable<global::Doroti.Ui.Rect>)(object?)DisplayFeatureSubScreen.subScreensInBounds(wantedBounds, DisplayFeatureSubScreen.avoidBounds(mediaQuery)));
        global::Doroti.Ui.Rect closestSubScreen = ((global::Doroti.Ui.Rect)(object?)DisplayFeatureSubScreen._closestToAnchorPoint(subScreens.Cast<Rect>(), resolvedAnchorPoint));
        return ((Widget)(object?)new Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: closestSubScreen.left, top: closestSubScreen.top, right: (parentSize.width - closestSubScreen.right), bottom: (parentSize.height - closestSubScreen.bottom)), child: new MediaQuery(data: mediaQuery.removeDisplayFeatures(closestSubScreen), child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Offset _fallbackAnchorPoint(BuildContext context)
    {
        return (Directionality.of(context) switch { TextDirection.rtl => new global::Doroti.Ui.Offset(double.MaxValue, 0), TextDirection.ltr => Offset.zero, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static IEnumerable<global::Doroti.Ui.Rect> avoidBounds(MediaQueryData mediaQuery)
    {
        return ((IEnumerable<global::Doroti.Ui.Rect>)(object?)((MediaQueryData)mediaQuery).displayFeatures.where(((d) => ((d.bounds.shortestSide > 0L) || (object.Equals(d.state, DisplayFeatureState.postureHalfOpened))))).map<global::Doroti.Ui.DisplayFeature, Rect>(((d) => d.bounds)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Rect _closestToAnchorPoint(IEnumerable<Rect> subScreens, Offset anchorPoint)
    {
        global::Doroti.Ui.Rect closestScreen = ((global::Doroti.Ui.Rect)(object?)subScreens.First());
        double closestDistance = DisplayFeatureSubScreen._distanceFromPointToRect(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(anchorPoint)), closestScreen);
        foreach (var screen in subScreens)
        {
            double subScreenDistance = DisplayFeatureSubScreen._distanceFromPointToRect(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(anchorPoint)), screen);
            if ((subScreenDistance < closestDistance))
            {
                closestScreen = screen;
                closestDistance = subScreenDistance;
            }
        }
        return closestScreen;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _distanceFromPointToRect(Offset point, Rect rect)
    {
        if ((point.dx < rect.left))
        {
            if ((point.dy < rect.top))
            {
                return ((point - rect.topLeft)).distance;
            }
            else
            {
                if ((point.dy > rect.bottom))
                {
                    return ((point - rect.bottomLeft)).distance;
                }
                else
                {
                    return (rect.left - point.dx);
                }
            }
        }
        else
        {
            if ((point.dx > rect.right))
            {
                if ((point.dy < rect.top))
                {
                    return ((point - rect.topRight)).distance;
                }
                else
                {
                    if ((point.dy > rect.bottom))
                    {
                        return ((point - rect.bottomRight)).distance;
                    }
                    else
                    {
                        return (point.dx - rect.right);
                    }
                }
            }
            else
            {
                if ((point.dy < rect.top))
                {
                    return (rect.top - point.dy);
                }
                else
                {
                    if ((point.dy > rect.bottom))
                    {
                        return (point.dy - rect.bottom);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static IEnumerable<global::Doroti.Ui.Rect> subScreensInBounds(Rect wantedBounds, IEnumerable<Rect> avoidBounds)
    {
        IEnumerable<global::Doroti.Ui.Rect> subScreens = ((IEnumerable<global::Doroti.Ui.Rect>)(object?)new List<global::Doroti.Ui.Rect> { wantedBounds });
        foreach (var bounds in avoidBounds)
        {
            var newSubScreens = new List<global::Doroti.Ui.Rect>();
            foreach (var screen in subScreens)
            {
                if (((screen.top >= bounds.top) && (screen.bottom <= bounds.bottom)))
                {
                    if ((screen.left < bounds.left))
                    {
                        newSubScreens.Add(global::Doroti.Ui.Rect.fromLTWH(screen.left, screen.top, (bounds.left - screen.left), screen.height));
                    }
                    if ((screen.right > bounds.right))
                    {
                        newSubScreens.Add(global::Doroti.Ui.Rect.fromLTWH(bounds.right, screen.top, (screen.right - bounds.right), screen.height));
                    }
                }
                else
                {
                    if (((screen.left >= bounds.left) && (screen.right <= bounds.right)))
                    {
                        if ((screen.top < bounds.top))
                        {
                            newSubScreens.Add(global::Doroti.Ui.Rect.fromLTWH(screen.left, screen.top, screen.width, (bounds.top - screen.top)));
                        }
                        if ((screen.bottom > bounds.bottom))
                        {
                            newSubScreens.Add(global::Doroti.Ui.Rect.fromLTWH(screen.left, bounds.bottom, screen.width, (screen.bottom - bounds.bottom)));
                        }
                    }
                    else
                    {
                        newSubScreens.Add(screen);
                    }
                }
            }
            subScreens = DartRuntimePrimitives.ConvertValue<IEnumerable<Rect>>(newSubScreens);
        }
        return ((IEnumerable<global::Doroti.Ui.Rect>)(object?)subScreens);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Offset _capOffset(Offset offset, Size maximum)
    {
        if (((((offset.dx >= 0L) && (offset.dx <= maximum.width)) && (offset.dy >= 0L)) && (offset.dy <= maximum.height)))
        {
            return offset;
        }
        else
        {
            return new global::Doroti.Ui.Offset(Math.Min(Math.Max(0, offset.dx), maximum.width), Math.Min(Math.Max(0, offset.dy), maximum.height));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

