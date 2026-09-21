// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/circle_border.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public class CircleBorder : OutlinedBorder
{
    public virtual double eccentricity { get; private set; } = default!;

    public CircleBorder(BorderSide side = default!, double eccentricity = 0.0)
        : base(side: side ?? BorderSide.none)
    {
        this.eccentricity = eccentricity;
        System.Diagnostics.Debug.Assert(eccentricity >= 0.0);
        System.Diagnostics.Debug.Assert(eccentricity <= 1.0);
    }

    public override ShapeBorder scale(double t) =>
        new CircleBorder(side: side.scale(t), eccentricity: eccentricity);

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is CircleBorder)
        {
            CircleBorder a__as2194 = (CircleBorder)a;
            return new CircleBorder(
                side: BorderSide.lerp(a__as2194.side, side, t),
                eccentricity: Dart_uiLibrary.clampDouble(
                    (
                        Dart_uiLibrary.lerpDouble(a__as2194.eccentricity, eccentricity, t)
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    0.0,
                    1.0
                )
            );
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is CircleBorder)
        {
            CircleBorder b__as2507 = (CircleBorder)b;
            return new CircleBorder(
                side: BorderSide.lerp(side, b__as2507.side, t),
                eccentricity: Dart_uiLibrary.clampDouble(
                    (
                        Dart_uiLibrary.lerpDouble(eccentricity, b__as2507.eccentricity, t)
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    0.0,
                    1.0
                )
            );
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addOval(_adjustRect(rect).deflate(side.strokeInset));
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addOval(_adjustRect(rect));
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        Rect adjustedRect = _adjustRect(rect);
        return RRect
            .fromRectAndRadius(
                adjustedRect,
                Radius.elliptical(adjustedRect.width / 2.0, adjustedRect.height / 2.0)
            )
            .contains(position);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void paintInterior(
        Canvas canvas,
        Rect rect,
        Paint paint,
        TextDirection? textDirection = null
    )
    {
        if (eccentricity == 0.0)
        {
            canvas.drawCircle(rect.center, rect.shortestSide / 2.0, paint);
        }
        else
        {
            canvas.drawOval(_adjustRect(rect), paint);
        }
    }

    public override bool preferPaintInterior => true;

    public override CircleBorder copyWith(
        BorderSide? side = null,
        BorderRadiusGeometry? borderRadius = null,
        double? eccentricity = null,
        LinearBorderEdge? start = null,
        LinearBorderEdge? end = null,
        LinearBorderEdge? top = null,
        LinearBorderEdge? bottom = null,
        double? circularity = null,
        double? rectilinearity = null,
        double? points = null,
        double? innerRadiusRatio = null,
        double? pointRounding = null,
        double? valleyRounding = null,
        double? rotation = null,
        double? squash = null
    )
    {
        return new CircleBorder(
            side: side ?? this.side,
            eccentricity: eccentricity ?? this.eccentricity
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    )
    {
        switch (side.style)
        {
            case BorderStyle.none:
            {
                break;
            }
            case BorderStyle.solid:
            {
                if (eccentricity == 0.0)
                {
                    canvas.drawCircle(
                        rect.center,
                        (rect.shortestSide + side.strokeOffset) / 2L,
                        side.toPaint()
                    );
                }
                else
                {
                    Rect borderRect = _adjustRect(rect);
                    canvas.drawOval(borderRect.inflate(side.strokeOffset / 2L), side.toPaint());
                }
                break;
            }
        }
    }

    internal virtual Rect _adjustRect(Rect rect)
    {
        if ((eccentricity == 0.0) || (rect.width == rect.height))
        {
            return Rect.fromCircle(center: rect.center, radius: rect.shortestSide / 2.0);
        }
        if (rect.width < rect.height)
        {
            double delta = (1.0 - eccentricity) * (rect.height - rect.width) / 2.0;
            return Rect.fromLTRB(rect.left, rect.top + delta, rect.right, rect.bottom - delta);
        }
        else
        {
            double deltaLocal = (1.0 - eccentricity) * (rect.width - rect.height) / 2.0;
            return Rect.fromLTRB(
                rect.left + deltaLocal,
                rect.top,
                rect.right - deltaLocal,
                rect.bottom
            );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as CircleBorder;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is CircleBorder)
            && Equals(__other.side, side)
            && (__other.eccentricity == eccentricity);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, eccentricity);

    public override string ToString()
    {
        if (eccentricity != 0.0)
        {
            return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "CircleBorder")}({side}, eccentricity: {eccentricity})";
        }
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "CircleBorder")}({side})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
