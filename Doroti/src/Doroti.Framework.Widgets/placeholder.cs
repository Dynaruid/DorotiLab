// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/placeholder.dart
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

internal class _PlaceholderPainter__placeholder : global::Doroti.Framework.Rendering.CustomPainter
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
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = color;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = strokeWidth;
    return __cascade;
}))();
        global::Doroti.Ui.Rect rect = Offset.zero & size;
        var path = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(rect);
    __cascade.addPolygon(new List<global::Doroti.Ui.Offset> { rect.topRight, rect.bottomLeft }, false);
    __cascade.addPolygon(new List<global::Doroti.Ui.Offset> { rect.topLeft, rect.bottomRight }, false);
    return __cascade;
}))();
        canvas.drawPath(path, paintLocal);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_PlaceholderPainter__placeholder)oldDelegate;
        return (!Equals(__oldPainter.color, color)) || (__oldPainter.strokeWidth != strokeWidth);
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

    public Placeholder(global::Doroti.Framework.Foundation.Key? key = null, Color color = default!, double strokeWidth = 2.0, double fallbackWidth = 400.0, double fallbackHeight = 400.0, Widget? child = null) : base(key: key)
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
        return new LimitedBox(maxWidth: fallbackWidth, maxHeight: fallbackHeight, child: new CustomPaint(size: Size.infinite, painter: new _PlaceholderPainter__placeholder(color: color, strokeWidth: strokeWidth), child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", color, defaultValue: new global::Doroti.Ui.Color(4282735204L)));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("strokeWidth", strokeWidth, defaultValue: 2.0));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("fallbackWidth", fallbackWidth, defaultValue: 400.0));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("fallbackHeight", fallbackHeight, defaultValue: 400.0));
    }

}

