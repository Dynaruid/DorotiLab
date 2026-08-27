// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tab_indicator.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class UnderlineTabIndicator : global::Doroti.Framework.Painting.Decoration
{
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide borderSide { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry insets { get; private set; } = default!;

    public UnderlineTabIndicator(global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.BorderSide borderSide = default!, global::Doroti.Framework.Painting.EdgeInsetsGeometry insets = default!)
    {
        global::Doroti.Framework.Painting.BorderSide __borderSide = borderSide ?? new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: Colors.white);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __insets = insets ?? global::Doroti.Framework.Painting.EdgeInsets.zero;
        this.borderRadius = borderRadius;
        this.borderSide = __borderSide;
        this.insets = __insets;
    }

    public override global::Doroti.Framework.Painting.Decoration? lerpFrom(global::Doroti.Framework.Painting.Decoration? a, double t)
    {
        if ((a is UnderlineTabIndicator))
        {
            UnderlineTabIndicator a__as1729 = (UnderlineTabIndicator)a;
            return ((global::Doroti.Framework.Painting.Decoration?)(object?)new UnderlineTabIndicator(borderSide: BorderSide.lerp(((UnderlineTabIndicator)((UnderlineTabIndicator)a__as1729)).borderSide, this.borderSide, t), insets: EdgeInsetsGeometry.lerp(((UnderlineTabIndicator)((UnderlineTabIndicator)a__as1729)).insets, this.insets, t)!));
        }
        return ((global::Doroti.Framework.Painting.Decoration?)(object?)base.lerpFrom(a, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Painting.Decoration? lerpTo(global::Doroti.Framework.Painting.Decoration? b, double t)
    {
        if ((b is UnderlineTabIndicator))
        {
            UnderlineTabIndicator b__as2045 = (UnderlineTabIndicator)b;
            return ((global::Doroti.Framework.Painting.Decoration?)(object?)new UnderlineTabIndicator(borderSide: BorderSide.lerp(this.borderSide, ((UnderlineTabIndicator)((UnderlineTabIndicator)b__as2045)).borderSide, t), insets: EdgeInsetsGeometry.lerp(this.insets, ((UnderlineTabIndicator)((UnderlineTabIndicator)b__as2045)).insets, t)!));
        }
        return ((global::Doroti.Framework.Painting.Decoration?)(object?)base.lerpTo(b, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Painting.BoxPainter createBoxPainter(global::System.Action onChanged = default!)
    {
        return ((global::Doroti.Framework.Painting.BoxPainter)(object?)new _UnderlinePainter__tab_indicator(this, this.borderRadius, () => onChanged()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Rect _indicatorRectFor(Rect rect, TextDirection textDirection)
    {
        global::Doroti.Ui.Rect indicator = ((global::Doroti.Ui.Rect)(object?)this.insets.resolve(textDirection).deflateRect(rect));
        return global::Doroti.Ui.Rect.fromLTWH(indicator.left, (indicator.bottom - ((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width), indicator.width, ((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getClipPath(Rect rect, TextDirection textDirection)
    {
        if ((this.borderRadius is not null))
        {
            return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRRect(this.borderRadius!.toRRect(_indicatorRectFor(rect, textDirection)));
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
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Painting.ImageConfiguration)configuration).size is not null));
        global::Doroti.Ui.Rect rect = ((global::Doroti.Ui.Rect)(object?)(offset & DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Painting.ImageConfiguration)configuration).size)));
        global::Doroti.Ui.TextDirection textDirectionLocal = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Painting.ImageConfiguration)configuration).textDirection);
        global::Doroti.Ui.Paint paintLocal = default!;
        if ((this.borderRadius is not null))
        {
            paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = ((UnderlineTabIndicator)this.decoration).borderSide.color;
    return __cascade;
}))();
            global::Doroti.Ui.Rect indicator = ((global::Doroti.Ui.Rect)(object?)this.decoration._indicatorRectFor(rect, textDirectionLocal));
            var rrect = global::Doroti.Ui.RRect.fromRectAndCorners(indicator, topLeft: this.borderRadius!.topLeft, topRight: this.borderRadius!.topRight, bottomRight: this.borderRadius!.bottomRight, bottomLeft: this.borderRadius!.bottomLeft);
            canvas.drawRRect(rrect, paintLocal);
        }
        else
        {
            paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = ((UnderlineTabIndicator)this.decoration).borderSide.toPaint();
    __cascade.strokeCap = StrokeCap.square;
    return __cascade;
}))();
            global::Doroti.Ui.Rect indicatorLocal = ((global::Doroti.Ui.Rect)(object?)this.decoration._indicatorRectFor(rect, textDirectionLocal).deflate((((UnderlineTabIndicator)this.decoration).borderSide.width / 2.0)));
            canvas.drawLine(indicatorLocal.bottomLeft, indicatorLocal.bottomRight, paintLocal);
        }
    }

}
