// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/placeholder.dart
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

namespace Doroti.Generated.Framework.Widgets;

internal class _PlaceholderPainter__placeholder : global::Doroti.Generated.Framework.Rendering.CustomPainter
{
    public virtual Color color { get; private set; } = default!;
    public virtual double strokeWidth { get; private set; } = default!;

    internal _PlaceholderPainter__placeholder(Color color, double strokeWidth)
    {
        this.color = color;
        this.strokeWidth = strokeWidth;
    }

    public override void paint(Canvas canvas, Size size)
    {
        var paint__498 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.color;
            __cascade.style = PaintingStyle.stroke;
            __cascade.strokeWidth = this.strokeWidth;
            return __cascade;        }))();
        global::Doroti.Ui.Rect rect__623 = ((global::Doroti.Ui.Rect)(object?)(Offset.zero & size));
        var path__660 = ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Ui.Path();
            __cascade.addRect(rect__623);
            __cascade.addPolygon(new List<global::Doroti.Ui.Offset> { rect__623.topRight, rect__623.bottomLeft }, false);
            __cascade.addPolygon(new List<global::Doroti.Ui.Offset> { rect__623.topLeft, rect__623.bottomRight }, false);
            return __cascade;        }))();
        canvas.drawPath(path__660, paint__498);
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_PlaceholderPainter__placeholder)(object)oldDelegate;
        return ((!object.Equals(((_PlaceholderPainter__placeholder)__oldPainter).color, this.color)) || (((_PlaceholderPainter__placeholder)__oldPainter).strokeWidth != this.strokeWidth));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool? hitTest(Offset position) => false;
}

public class Placeholder : StatelessWidget
{
    public virtual Color color { get; private set; } = default!;
    public virtual double strokeWidth { get; private set; } = default!;
    public virtual double fallbackWidth { get; private set; } = default!;
    public virtual double fallbackHeight { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public Placeholder(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color color = default!, double strokeWidth = 2.0, double fallbackWidth = 400.0, double fallbackHeight = 400.0, Widget? child = null) : base(key: key)
    {
        Color __color = color ?? new Color(0xFF455A64);
        this.color = __color;
        this.strokeWidth = strokeWidth;
        this.fallbackWidth = fallbackWidth;
        this.fallbackHeight = fallbackHeight;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new LimitedBox(maxWidth: this.fallbackWidth, maxHeight: this.fallbackHeight, child: new CustomPaint(size: Size.infinite, painter: new _PlaceholderPainter__placeholder(color: this.color, strokeWidth: this.strokeWidth), child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: new global::Doroti.Ui.Color(4282735204L)));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("strokeWidth", this.strokeWidth, defaultValue: 2.0));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("fallbackWidth", this.fallbackWidth, defaultValue: 400.0));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("fallbackHeight", this.fallbackHeight, defaultValue: 400.0));
    }

}

