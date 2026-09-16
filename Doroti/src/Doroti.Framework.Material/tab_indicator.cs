// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tab_indicator.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class UnderlineTabIndicator : global::Doroti.Framework.Painting.Decoration
{
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide borderSide { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry insets { get; private set; } = default!;

    public UnderlineTabIndicator(global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.BorderSide borderSide = default!, global::Doroti.Framework.Painting.EdgeInsetsGeometry insets = default!)
    {
        global::Doroti.Framework.Painting.BorderSide __borderSide = borderSide ?? new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: Colors.white);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __insets = insets ?? EdgeInsets.zero;
        this.borderRadius = borderRadius;
        this.borderSide = __borderSide;
        this.insets = __insets;
    }

    public override global::Doroti.Framework.Painting.Decoration? lerpFrom(global::Doroti.Framework.Painting.Decoration? a, double t)
    {
        if (a is UnderlineTabIndicator)
        {
            UnderlineTabIndicator a__as1729 = (UnderlineTabIndicator)a;
            return (global::Doroti.Framework.Painting.Decoration?)new UnderlineTabIndicator(borderSide: BorderSide.lerp(a__as1729.borderSide, borderSide, t), insets: EdgeInsetsGeometry.lerp(a__as1729.insets, insets, t)!);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Painting.Decoration? lerpTo(global::Doroti.Framework.Painting.Decoration? b, double t)
    {
        if (b is UnderlineTabIndicator)
        {
            UnderlineTabIndicator b__as2045 = (UnderlineTabIndicator)b;
            return (global::Doroti.Framework.Painting.Decoration?)new UnderlineTabIndicator(borderSide: BorderSide.lerp(borderSide, b__as2045.borderSide, t), insets: EdgeInsetsGeometry.lerp(insets, b__as2045.insets, t)!);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Painting.BoxPainter createBoxPainter(global::System.Action onChanged = default!)
    {
        return new _UnderlinePainter__tab_indicator(this, borderRadius, () => onChanged());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Rect _indicatorRectFor(Rect rect, TextDirection textDirection)
    {
        global::Doroti.Ui.Rect indicator = insets.resolve(textDirection).deflateRect(rect);
        return Rect.fromLTWH(indicator.left, indicator.bottom - borderSide.width, indicator.width, borderSide.width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getClipPath(Rect rect, TextDirection textDirection)
    {
        if (borderRadius is not null)
        {
            return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRRect(borderRadius!.toRRect(_indicatorRectFor(rect, textDirection)));
    return __cascade;
}))();
        }
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(_indicatorRectFor(rect, textDirection));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _UnderlinePainter__tab_indicator : global::Doroti.Framework.Painting.BoxPainter
{
    public virtual UnderlineTabIndicator decoration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius { get; private set; }

    internal _UnderlinePainter__tab_indicator(UnderlineTabIndicator decoration, global::Doroti.Framework.Painting.BorderRadius? borderRadius, global::System.Action? onChanged) : base(onChanged)
    {
        this.decoration = decoration;
        this.borderRadius = borderRadius;
    }

    public override void paint(Canvas canvas, Offset offset, global::Doroti.Framework.Painting.ImageConfiguration configuration)
    {
        DartRuntimePrimitives.Assert(() => configuration.size is not null);
        global::Doroti.Ui.Rect rect = offset & DartRuntimePrimitives.RequireValue(configuration.size);
        global::Doroti.Ui.TextDirection textDirectionLocal = DartRuntimePrimitives.RequireValue(configuration.textDirection);
        global::Doroti.Ui.Paint paintLocal = default!;
        if (borderRadius is not null)
        {
            paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = decoration.borderSide.color;
    return __cascade;
}))();
            global::Doroti.Ui.Rect indicator = decoration._indicatorRectFor(rect, textDirectionLocal);
            var rrect = RRect.fromRectAndCorners(indicator, topLeft: borderRadius!.topLeft, topRight: borderRadius!.topRight, bottomRight: borderRadius!.bottomRight, bottomLeft: borderRadius!.bottomLeft);
            canvas.drawRRect(rrect, paintLocal);
        }
        else
        {
            paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = decoration.borderSide.toPaint();
    __cascade.strokeCap = StrokeCap.square;
    return __cascade;
}))();
            global::Doroti.Ui.Rect indicatorLocal = decoration._indicatorRectFor(rect, textDirectionLocal).deflate(decoration.borderSide.width / 2.0);
            canvas.drawLine(indicatorLocal.bottomLeft, indicatorLocal.bottomRight, paintLocal);
        }
    }

}
