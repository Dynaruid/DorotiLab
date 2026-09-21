// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tab_indicator.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class UnderlineTabIndicator : Decoration
{
    public virtual BorderRadius? borderRadius { get; private set; }
    public virtual BorderSide borderSide { get; private set; } = default!;
    public virtual EdgeInsetsGeometry insets { get; private set; } = default!;

    public UnderlineTabIndicator(
        BorderRadius? borderRadius = null,
        BorderSide borderSide = default!,
        EdgeInsetsGeometry insets = default!
    )
    {
        BorderSide __borderSide = borderSide ?? new BorderSide(width: 2.0, color: Colors.white);
        EdgeInsetsGeometry __insets = insets ?? EdgeInsets.zero;
        this.borderRadius = borderRadius;
        this.borderSide = __borderSide;
        this.insets = __insets;
    }

    public override Decoration? lerpFrom(Decoration? a, double t)
    {
        if (a is UnderlineTabIndicator)
        {
            UnderlineTabIndicator a__as1729 = (UnderlineTabIndicator)a;
            return (Decoration?)
                new UnderlineTabIndicator(
                    borderSide: BorderSide.lerp(a__as1729.borderSide, borderSide, t),
                    insets: EdgeInsetsGeometry.lerp(a__as1729.insets, insets, t)!
                );
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Decoration? lerpTo(Decoration? b, double t)
    {
        if (b is UnderlineTabIndicator)
        {
            UnderlineTabIndicator b__as2045 = (UnderlineTabIndicator)b;
            return (Decoration?)
                new UnderlineTabIndicator(
                    borderSide: BorderSide.lerp(borderSide, b__as2045.borderSide, t),
                    insets: EdgeInsetsGeometry.lerp(insets, b__as2045.insets, t)!
                );
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override BoxPainter createBoxPainter(Action onChanged = default!)
    {
        return new _UnderlinePainter__tab_indicator(this, borderRadius, () => onChanged());
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Rect _indicatorRectFor(Rect rect, TextDirection textDirection)
    {
        Rect indicator = insets.resolve(textDirection).deflateRect(rect);
        return Rect.fromLTWH(
            indicator.left,
            indicator.bottom - borderSide.width,
            indicator.width,
            borderSide.width
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Path getClipPath(Rect rect, TextDirection textDirection)
    {
        if (borderRadius is not null)
        {
            return (
                (Func<Path>)(
                    () =>
                    {
                        var __cascade = new Path();
                        __cascade.addRRect(
                            borderRadius!.toRRect(_indicatorRectFor(rect, textDirection))
                        );
                        return __cascade;
                    }
                )
            )();
        }
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRect(_indicatorRectFor(rect, textDirection));
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _UnderlinePainter__tab_indicator : BoxPainter
{
    public virtual UnderlineTabIndicator decoration { get; private set; } = default!;
    public virtual BorderRadius? borderRadius { get; private set; }

    internal _UnderlinePainter__tab_indicator(
        UnderlineTabIndicator decoration,
        BorderRadius? borderRadius,
        Action? onChanged
    )
        : base(onChanged)
    {
        this.decoration = decoration;
        this.borderRadius = borderRadius;
    }

    public override void paint(Canvas canvas, Offset offset, ImageConfiguration configuration)
    {
        DartRuntimePrimitives.Assert(() => configuration.size is not null);
        Rect rect =
            offset
            & (
                configuration.size
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        TextDirection textDirectionLocal = (
            configuration.textDirection
            ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        Paint paintLocal = default!;
        if (borderRadius is not null)
        {
            paintLocal = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = decoration.borderSide.color;
                        return __cascade;
                    }
                )
            )();
            Rect indicator = decoration._indicatorRectFor(rect, textDirectionLocal);
            var rrect = RRect.fromRectAndCorners(
                indicator,
                topLeft: borderRadius!.topLeft,
                topRight: borderRadius!.topRight,
                bottomRight: borderRadius!.bottomRight,
                bottomLeft: borderRadius!.bottomLeft
            );
            canvas.drawRRect(rrect, paintLocal);
        }
        else
        {
            paintLocal = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = decoration.borderSide.toPaint();
                        __cascade.strokeCap = StrokeCap.square;
                        return __cascade;
                    }
                )
            )();
            Rect indicatorLocal = decoration
                ._indicatorRectFor(rect, textDirectionLocal)
                .deflate(decoration.borderSide.width / 2.0);
            canvas.drawLine(indicatorLocal.bottomLeft, indicatorLocal.bottomRight, paintLocal);
        }
    }
}
