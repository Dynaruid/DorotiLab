// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/display_feature_sub_screen.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class DisplayFeatureSubScreen : StatelessWidget
{
    public virtual Offset? anchorPoint { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public DisplayFeatureSubScreen(global::Doroti.Generated.Framework.Foundation.Key? key = null, Offset? anchorPoint = null, Widget child = default!) : base(key: key)
    {
        this.anchorPoint = anchorPoint;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => ((this.anchorPoint is not null) || global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context, why: "to determine which sub-screen DisplayFeatureSubScreen uses", alternative: "Alternatively, consider specifying the 'anchorPoint' argument on the DisplayFeatureSubScreen.")));
        MediaQueryData mediaQuery__4178 = ((MediaQueryData)(object?)MediaQuery.of(context));
        global::Doroti.Flutter.Ui.Size parentSize__4230 = ((global::Doroti.Flutter.Ui.Size)(object?)((MediaQueryData)mediaQuery__4178).size);
        global::Doroti.Flutter.Ui.Rect wantedBounds__4275 = ((global::Doroti.Flutter.Ui.Rect)(object?)(Offset.zero & parentSize__4230));
        global::Doroti.Flutter.Ui.Offset resolvedAnchorPoint__4333 = ((global::Doroti.Flutter.Ui.Offset)(object?)DisplayFeatureSubScreen._capOffset(((this.anchorPoint ?? (Offset)DisplayFeatureSubScreen._fallbackAnchorPoint(context))), parentSize__4230));
        IEnumerable<global::Doroti.Flutter.Ui.Rect> subScreens__4469 = ((IEnumerable<global::Doroti.Flutter.Ui.Rect>)(object?)DisplayFeatureSubScreen.subScreensInBounds(wantedBounds__4275, DisplayFeatureSubScreen.avoidBounds(mediaQuery__4178)));
        global::Doroti.Flutter.Ui.Rect closestSubScreen__4556 = ((global::Doroti.Flutter.Ui.Rect)(object?)DisplayFeatureSubScreen._closestToAnchorPoint(subScreens__4469.Cast<Rect>(), resolvedAnchorPoint__4333));
        return ((Widget)(object?)new Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: closestSubScreen__4556.left, top: closestSubScreen__4556.top, right: (parentSize__4230.width - closestSubScreen__4556.right), bottom: (parentSize__4230.height - closestSubScreen__4556.bottom)), child: new MediaQuery(data: mediaQuery__4178.removeDisplayFeatures(closestSubScreen__4556), child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Flutter.Ui.Offset _fallbackAnchorPoint(BuildContext context)
    {
        return (Directionality.of(context) switch { TextDirection.rtl => new global::Doroti.Flutter.Ui.Offset(double.MaxValue, 0), TextDirection.ltr => Offset.zero, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static IEnumerable<global::Doroti.Flutter.Ui.Rect> avoidBounds(MediaQueryData mediaQuery)
    {
        return ((IEnumerable<global::Doroti.Flutter.Ui.Rect>)(object?)((MediaQueryData)mediaQuery).displayFeatures.where(((d) => ((d.bounds.shortestSide > 0L) || (object.Equals(d.state, DisplayFeatureState.postureHalfOpened))))).map<global::Doroti.Flutter.Ui.DisplayFeature, Rect>(((d) => d.bounds)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Flutter.Ui.Rect _closestToAnchorPoint(IEnumerable<Rect> subScreens, Offset anchorPoint)
    {
        global::Doroti.Flutter.Ui.Rect closestScreen__5906 = ((global::Doroti.Flutter.Ui.Rect)(object?)subScreens.First());
        double closestDistance__5951 = DisplayFeatureSubScreen._distanceFromPointToRect(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(anchorPoint)), closestScreen__5906);
        foreach (var screen__6038 in subScreens)
        {
            double subScreenDistance__6081 = DisplayFeatureSubScreen._distanceFromPointToRect(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(anchorPoint)), screen__6038);
            if ((subScreenDistance__6081 < closestDistance__5951))
            {
                closestScreen__5906 = screen__6038;
                closestDistance__5951 = subScreenDistance__6081;
            }
        }
        return closestScreen__5906;
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

    public static IEnumerable<global::Doroti.Flutter.Ui.Rect> subScreensInBounds(Rect wantedBounds, IEnumerable<Rect> avoidBounds)
    {
        IEnumerable<global::Doroti.Flutter.Ui.Rect> subScreens__7651 = ((IEnumerable<global::Doroti.Flutter.Ui.Rect>)(object?)new List<global::Doroti.Flutter.Ui.Rect> { wantedBounds });
        foreach (var bounds__7701 in avoidBounds)
        {
            var newSubScreens__7738 = new List<global::Doroti.Flutter.Ui.Rect>();
            foreach (var screen__7781 in subScreens__7651)
            {
                if (((screen__7781.top >= bounds__7701.top) && (screen__7781.bottom <= bounds__7701.bottom)))
                {
                    if ((screen__7781.left < bounds__7701.left))
                    {
                        newSubScreens__7738.Add(global::Doroti.Flutter.Ui.Rect.fromLTWH(screen__7781.left, screen__7781.top, (bounds__7701.left - screen__7781.left), screen__7781.height));
                    }
                    if ((screen__7781.right > bounds__7701.right))
                    {
                        newSubScreens__7738.Add(global::Doroti.Flutter.Ui.Rect.fromLTWH(bounds__7701.right, screen__7781.top, (screen__7781.right - bounds__7701.right), screen__7781.height));
                    }
                }
                else
                {
                    if (((screen__7781.left >= bounds__7701.left) && (screen__7781.right <= bounds__7701.right)))
                    {
                        if ((screen__7781.top < bounds__7701.top))
                        {
                            newSubScreens__7738.Add(global::Doroti.Flutter.Ui.Rect.fromLTWH(screen__7781.left, screen__7781.top, screen__7781.width, (bounds__7701.top - screen__7781.top)));
                        }
                        if ((screen__7781.bottom > bounds__7701.bottom))
                        {
                            newSubScreens__7738.Add(global::Doroti.Flutter.Ui.Rect.fromLTWH(screen__7781.left, bounds__7701.bottom, screen__7781.width, (screen__7781.bottom - bounds__7701.bottom)));
                        }
                    }
                    else
                    {
                        newSubScreens__7738.Add(screen__7781);
                    }
                }
            }
            subScreens__7651 = DartRuntimePrimitives.ConvertValue<IEnumerable<Rect>>(newSubScreens__7738);
        }
        return ((IEnumerable<global::Doroti.Flutter.Ui.Rect>)(object?)subScreens__7651);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Flutter.Ui.Offset _capOffset(Offset offset, Size maximum)
    {
        if (((((offset.dx >= 0L) && (offset.dx <= maximum.width)) && (offset.dy >= 0L)) && (offset.dy <= maximum.height)))
        {
            return offset;
        }
        else
        {
            return new global::Doroti.Flutter.Ui.Offset(Math.Min(Math.Max(0, offset.dx), maximum.width), Math.Min(Math.Max(0, offset.dy), maximum.height));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

