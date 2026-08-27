// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/grid_paper.dart
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

namespace Doroti.Framework.Widgets;

internal class _GridPaperPainter__grid_paper : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual Color color { get; private set; } = default!;
    public virtual double interval { get; private set; } = default!;
    public virtual long divisions { get; private set; } = default!;
    public virtual long subdivisions { get; private set; } = default!;

    internal _GridPaperPainter__grid_paper(Color color, double interval, long divisions, long subdivisions)
    {
        this.color = color;
        this.interval = interval;
        this.divisions = divisions;
        this.subdivisions = subdivisions;
    }

    public override void paint(Canvas canvas, Size size)
    {
        var linePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.color;
    return __cascade;
}))();
        double allDivisions = ((this.divisions * this.subdivisions)).toDouble();
        for (var x = 0.0; (x <= size.width); x += (this.interval / allDivisions))
        {
            linePaint.strokeWidth = ((((x % this.interval) == 0.0)) ? 1.0 : ((((x % ((this.interval / this.subdivisions))) == 0.0)) ? 0.5 : 0.25));
            canvas.drawLine(new global::Doroti.Ui.Offset(x, 0.0), new global::Doroti.Ui.Offset(x, size.height), linePaint);
        }
        for (var y = 0.0; (y <= size.height); y += (this.interval / allDivisions))
        {
            linePaint.strokeWidth = ((((y % this.interval) == 0.0)) ? 1.0 : ((((y % ((this.interval / this.subdivisions))) == 0.0)) ? 0.5 : 0.25));
            canvas.drawLine(new global::Doroti.Ui.Offset(0.0, y), new global::Doroti.Ui.Offset(size.width, y), linePaint);
        }
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_GridPaperPainter__grid_paper)(object)oldDelegate;
        return ((((!object.Equals(((_GridPaperPainter__grid_paper)__oldPainter).color, this.color)) || (((_GridPaperPainter__grid_paper)__oldPainter).interval != this.interval)) || (((_GridPaperPainter__grid_paper)__oldPainter).divisions != this.divisions)) || (((_GridPaperPainter__grid_paper)__oldPainter).subdivisions != this.subdivisions));
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

    public GridPaper(global::Doroti.Framework.Foundation.Key? key = null, Color color = default!, double interval = 100.0, long divisions = 2, long subdivisions = 5, Widget? child = null) : base(key: key)
    {
        Color __color = color ?? new Color(0x7FC3E8F3);
        this.color = __color;
        this.interval = interval;
        this.divisions = divisions;
        this.subdivisions = subdivisions;
        this.child = child;
        System.Diagnostics.Debug.Assert((divisions > 0L));
        System.Diagnostics.Debug.Assert((subdivisions > 0L));
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new CustomPaint(foregroundPainter: new _GridPaperPainter__grid_paper(color: this.color, interval: this.interval, divisions: this.divisions, subdivisions: this.subdivisions), child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

