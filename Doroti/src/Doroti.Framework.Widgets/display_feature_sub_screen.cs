// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/display_feature_sub_screen.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class DisplayFeatureSubScreen : StatelessWidget
{
    public virtual Offset? anchorPoint { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public DisplayFeatureSubScreen(
        Key? key = null,
        Offset? anchorPoint = null,
        Widget child = default!
    )
        : base(key: key)
    {
        this.anchorPoint = anchorPoint;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            (anchorPoint is not null)
            || DebugLibrary.debugCheckHasDirectionality(
                context,
                why: "to determine which sub-screen DisplayFeatureSubScreen uses",
                alternative: "Alternatively, consider specifying the 'anchorPoint' argument on the DisplayFeatureSubScreen."
            )
        );
        MediaQueryData mediaQuery = MediaQuery.of(context);
        Size parentSize = mediaQuery.size;
        Rect wantedBounds = Offset.zero & parentSize;
        Offset resolvedAnchorPoint = _capOffset(
            anchorPoint ?? _fallbackAnchorPoint(context),
            parentSize
        );
        IEnumerable<Rect> subScreens = subScreensInBounds(wantedBounds, avoidBounds(mediaQuery));
        Rect closestSubScreen = _closestToAnchorPoint(subScreens.Cast<Rect>(), resolvedAnchorPoint);
        return new Padding(
            padding: EdgeInsets.CreateOnly(
                left: closestSubScreen.left,
                top: closestSubScreen.top,
                right: parentSize.width - closestSubScreen.right,
                bottom: parentSize.height - closestSubScreen.bottom
            ),
            child: new MediaQuery(
                data: mediaQuery.removeDisplayFeatures(closestSubScreen),
                child: child
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static Offset _fallbackAnchorPoint(BuildContext context)
    {
        return Directionality.of(context) switch
        {
            TextDirection.rtl => new Offset(double.MaxValue, 0),
            TextDirection.ltr => Offset.zero,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static IEnumerable<Rect> avoidBounds(MediaQueryData mediaQuery)
    {
        return mediaQuery
            .displayFeatures.where(
                (d) =>
                    (d.bounds.shortestSide > 0L)
                    || Equals(d.state, DisplayFeatureState.postureHalfOpened)
            )
            .map((d) => d.bounds);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static Rect _closestToAnchorPoint(IEnumerable<Rect> subScreens, Offset anchorPoint)
    {
        Rect closestScreen = subScreens.First();
        double closestDistance = _distanceFromPointToRect(((anchorPoint)), closestScreen);
        foreach (var screen in subScreens)
        {
            double subScreenDistance = _distanceFromPointToRect(((anchorPoint)), screen);
            if (subScreenDistance < closestDistance)
            {
                closestScreen = screen;
                closestDistance = subScreenDistance;
            }
        }
        return closestScreen;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static double _distanceFromPointToRect(Offset point, Rect rect)
    {
        if (point.dx < rect.left)
        {
            if (point.dy < rect.top)
            {
                return (point - rect.topLeft).distance;
            }
            else
            {
                if (point.dy > rect.bottom)
                {
                    return (point - rect.bottomLeft).distance;
                }
                else
                {
                    return rect.left - point.dx;
                }
            }
        }
        else
        {
            if (point.dx > rect.right)
            {
                if (point.dy < rect.top)
                {
                    return (point - rect.topRight).distance;
                }
                else
                {
                    if (point.dy > rect.bottom)
                    {
                        return (point - rect.bottomRight).distance;
                    }
                    else
                    {
                        return point.dx - rect.right;
                    }
                }
            }
            else
            {
                if (point.dy < rect.top)
                {
                    return rect.top - point.dy;
                }
                else
                {
                    if (point.dy > rect.bottom)
                    {
                        return point.dy - rect.bottom;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static IEnumerable<Rect> subScreensInBounds(
        Rect wantedBounds,
        IEnumerable<Rect> avoidBounds
    )
    {
        IEnumerable<Rect> subScreens = new List<Rect> { wantedBounds };
        foreach (var bounds in avoidBounds)
        {
            var newSubScreens = new List<Rect>();
            foreach (var screen in subScreens)
            {
                if ((screen.top >= bounds.top) && (screen.bottom <= bounds.bottom))
                {
                    if (screen.left < bounds.left)
                    {
                        newSubScreens.Add(
                            Rect.fromLTWH(
                                screen.left,
                                screen.top,
                                bounds.left - screen.left,
                                screen.height
                            )
                        );
                    }
                    if (screen.right > bounds.right)
                    {
                        newSubScreens.Add(
                            Rect.fromLTWH(
                                bounds.right,
                                screen.top,
                                screen.right - bounds.right,
                                screen.height
                            )
                        );
                    }
                }
                else
                {
                    if ((screen.left >= bounds.left) && (screen.right <= bounds.right))
                    {
                        if (screen.top < bounds.top)
                        {
                            newSubScreens.Add(
                                Rect.fromLTWH(
                                    screen.left,
                                    screen.top,
                                    screen.width,
                                    bounds.top - screen.top
                                )
                            );
                        }
                        if (screen.bottom > bounds.bottom)
                        {
                            newSubScreens.Add(
                                Rect.fromLTWH(
                                    screen.left,
                                    bounds.bottom,
                                    screen.width,
                                    screen.bottom - bounds.bottom
                                )
                            );
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
        return subScreens;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static Offset _capOffset(Offset offset, Size maximum)
    {
        if (
            (offset.dx >= 0L)
            && (offset.dx <= maximum.width)
            && (offset.dy >= 0L)
            && (offset.dy <= maximum.height)
        )
        {
            return offset;
        }
        else
        {
            return new Offset(
                Math.Min(Math.Max(0, offset.dx), maximum.width),
                Math.Min(Math.Max(0, offset.dy), maximum.height)
            );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
