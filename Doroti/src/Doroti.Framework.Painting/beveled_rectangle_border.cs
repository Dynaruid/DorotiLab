// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/beveled_rectangle_border.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public class BeveledRectangleBorder : OutlinedBorder
{
    public virtual BorderRadiusGeometry borderRadius { get; private set; } = default!;

    public BeveledRectangleBorder(
        BorderSide side = default!,
        BorderRadiusGeometry borderRadius = default!
    )
        : base(side: side ?? BorderSide.none)
    {
        BorderRadiusGeometry __borderRadius = borderRadius ?? BorderRadius.zero;
        this.borderRadius = __borderRadius;
    }

    public override ShapeBorder scale(double t)
    {
        return new BeveledRectangleBorder(
            side: side.scale(t),
            borderRadius: borderRadius.op_Multiply(t)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is BeveledRectangleBorder)
        {
            BeveledRectangleBorder a__as1688 = (BeveledRectangleBorder)a;
            return new BeveledRectangleBorder(
                side: BorderSide.lerp(a__as1688.side, side, t),
                borderRadius: BorderRadiusGeometry.lerp(a__as1688.borderRadius, borderRadius, t)!
            );
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is BeveledRectangleBorder)
        {
            BeveledRectangleBorder b__as2010 = (BeveledRectangleBorder)b;
            return new BeveledRectangleBorder(
                side: BorderSide.lerp(side, b__as2010.side, t),
                borderRadius: BorderRadiusGeometry.lerp(borderRadius, b__as2010.borderRadius, t)!
            );
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override BeveledRectangleBorder copyWith(
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
        return new BeveledRectangleBorder(
            side: side ?? this.side,
            borderRadius: borderRadius ?? this.borderRadius
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Path _getPath(RRect rrect)
    {
        var centerLeft = new Offset(rrect.left, rrect.center.dy);
        var centerRight = new Offset(rrect.right, rrect.center.dy);
        var centerTop = new Offset(rrect.center.dx, rrect.top);
        var centerBottom = new Offset(rrect.center.dx, rrect.bottom);
        double tlRadiusXLocal = Math.Max(0.0, rrect.tlRadiusX);
        double tlRadiusYLocal = Math.Max(0.0, rrect.tlRadiusY);
        double trRadiusXLocal = Math.Max(0.0, rrect.trRadiusX);
        double trRadiusYLocal = Math.Max(0.0, rrect.trRadiusY);
        double blRadiusXLocal = Math.Max(0.0, rrect.blRadiusX);
        double blRadiusYLocal = Math.Max(0.0, rrect.blRadiusY);
        double brRadiusXLocal = Math.Max(0.0, rrect.brRadiusX);
        double brRadiusYLocal = Math.Max(0.0, rrect.brRadiusY);
        var vertices = new List<Offset>
        {
            new Offset(rrect.left, Math.Min(centerLeft.dy, rrect.top + tlRadiusYLocal)),
            new Offset(Math.Min(centerTop.dx, rrect.left + tlRadiusXLocal), rrect.top),
            new Offset(Math.Max(centerTop.dx, rrect.right - trRadiusXLocal), rrect.top),
            new Offset(rrect.right, Math.Min(centerRight.dy, rrect.top + trRadiusYLocal)),
            new Offset(rrect.right, Math.Max(centerRight.dy, rrect.bottom - brRadiusYLocal)),
            new Offset(Math.Max(centerBottom.dx, rrect.right - brRadiusXLocal), rrect.bottom),
            new Offset(Math.Min(centerBottom.dx, rrect.left + blRadiusXLocal), rrect.bottom),
            new Offset(rrect.left, Math.Max(centerLeft.dy, rrect.bottom - blRadiusYLocal)),
        };
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addPolygon(vertices, true);
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return _getPath(
            borderRadius.resolve(textDirection).toRRect(rect).deflate(side.strokeInset)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return _getPath(borderRadius.resolve(textDirection).toRRect(rect));
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
        if (rect.isEmpty)
        {
            return;
        }
        switch (side.style)
        {
            case BorderStyle.none:
            {
                break;
            }
            case BorderStyle.solid:
            {
                RRect borderRect = this.borderRadius.resolve(textDirection).toRRect(rect);
                RRect adjustedRect = borderRect.inflate(side.strokeOutset);
                Path path = (
                    (Func<Path>)(
                        () =>
                        {
                            var __cascade = _getPath(adjustedRect);
                            __cascade.addPath(
                                getInnerPath(rect, textDirection: textDirection),
                                Offset.zero
                            );
                            return __cascade;
                        }
                    )
                )();
                canvas.drawPath(path, side.toPaint());
                break;
            }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as BeveledRectangleBorder;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is BeveledRectangleBorder)
            && Equals(__other.side, side)
            && Equals(__other.borderRadius, borderRadius);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, borderRadius);

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "BeveledRectangleBorder")}({side}, {borderRadius})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
