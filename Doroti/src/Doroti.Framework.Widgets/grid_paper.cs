// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/grid_paper.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

internal class _GridPaperPainter__grid_paper : CustomPainter
{
    public virtual Color color { get; private set; } = default!;
    public virtual double interval { get; private set; } = default!;
    public virtual long divisions { get; private set; } = default!;
    public virtual long subdivisions { get; private set; } = default!;

    internal _GridPaperPainter__grid_paper(
        Color color,
        double interval,
        long divisions,
        long subdivisions
    )
    {
        this.color = color;
        this.interval = interval;
        this.divisions = divisions;
        this.subdivisions = subdivisions;
    }

    public override void paint(Canvas canvas, Size size)
    {
        var linePaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = color;
                    return __cascade;
                }
            )
        )();
        double allDivisions = (divisions * subdivisions).toDouble();
        for (var x = 0.0; x <= size.width; x += interval / allDivisions)
        {
            linePaint.strokeWidth =
                ((x % interval) == 0.0)
                    ? 1.0
                    : (((x % (interval / subdivisions)) == 0.0) ? 0.5 : 0.25);
            canvas.drawLine(new Offset(x, 0.0), new Offset(x, size.height), linePaint);
        }
        for (var y = 0.0; y <= size.height; y += interval / allDivisions)
        {
            linePaint.strokeWidth =
                ((y % interval) == 0.0)
                    ? 1.0
                    : (((y % (interval / subdivisions)) == 0.0) ? 0.5 : 0.25);
            canvas.drawLine(new Offset(0.0, y), new Offset(size.width, y), linePaint);
        }
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldPainter = (_GridPaperPainter__grid_paper)oldDelegate;
        return (!Equals(__oldPainter.color, color))
            || (__oldPainter.interval != interval)
            || (__oldPainter.divisions != divisions)
            || (__oldPainter.subdivisions != subdivisions);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool? hitTest(Offset position) => false;
}

public class GridPaper : StatelessWidget
{
    public virtual Color color { get; private set; } = default!;
    public virtual double interval { get; private set; } = default!;
    public virtual long divisions { get; private set; } = default!;
    public virtual long subdivisions { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public GridPaper(
        Key? key = null,
        Color color = default!,
        double interval = 100.0,
        long divisions = 2,
        long subdivisions = 5,
        Widget? child = null
    )
        : base(key: key)
    {
        Color __color = color ?? new Color(0x7FC3E8F3);
        this.color = __color;
        this.interval = interval;
        this.divisions = divisions;
        this.subdivisions = subdivisions;
        this.child = child;
        System.Diagnostics.Debug.Assert(divisions > 0L);
        System.Diagnostics.Debug.Assert(subdivisions > 0L);
    }

    public override Widget build(BuildContext context)
    {
        return new CustomPaint(
            foregroundPainter: new _GridPaperPainter__grid_paper(
                color: color,
                interval: interval,
                divisions: divisions,
                subdivisions: subdivisions
            ),
            child: child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
