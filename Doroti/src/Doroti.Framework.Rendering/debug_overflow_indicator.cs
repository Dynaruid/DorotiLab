// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/debug_overflow_indicator.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Rendering;

public enum _OverflowSide__debug_overflow_indicator
{
    left,
    top,
    bottom,
    right
}

public class _OverflowRegionData__debug_overflow_indicator
{
    public virtual Rect rect { get; private set; } = default!;
    public virtual string label { get; private set; } = default!;
    public virtual Offset labelOffset { get; private set; } = default!;
    public virtual double rotation { get; private set; } = default!;
    public virtual _OverflowSide__debug_overflow_indicator side { get; private set; } = default!;

    internal _OverflowRegionData__debug_overflow_indicator(Rect rect, string label = "", Offset labelOffset = default, double rotation = 0.0, _OverflowSide__debug_overflow_indicator side = default!)
    {
        this.rect = rect;
        this.label = label;
        this.labelOffset = labelOffset;
        this.rotation = rotation;
        this.side = side;
    }

}

public interface DebugOverflowIndicatorMixin
{
    internal static Color _black = new global::Doroti.Ui.Color(3204448256L);
    internal static Color _yellow = new global::Doroti.Ui.Color(3221225216L);
    internal const double _indicatorFraction = 0.1;
    internal const double _indicatorFontSizePixels = 7.5;
    internal const double _indicatorLabelPaddingPixels = 1.0;
    internal static global::Doroti.Framework.Painting.TextStyle _indicatorTextStyle = new global::Doroti.Framework.Painting.TextStyle(color: new global::Doroti.Ui.Color(4287627264L), fontSize: _indicatorFontSizePixels, fontWeight: FontWeight.w800);
    internal static Paint _indicatorPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.shader = global::Doroti.Ui.Gradient.linear(Offset.zero, new global::Doroti.Ui.Offset(10.0, 10.0), new List<global::Doroti.Ui.Color> { _black, _yellow, _yellow, _black }, new List<double> { 0.25, 0.25, 0.75, 0.75 }, TileMode.repeated);
    return __cascade;
}))();
    internal static Paint _labelBackgroundPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(4294967295L);
    return __cascade;
}))();
    List<global::Doroti.Framework.Painting.TextPainter> _indicatorLabel { get; }
    bool _overflowReportNeeded { get; set; }

    public void dispose();
    public string _formatPixels(double value);
    public List<_OverflowRegionData__debug_overflow_indicator> _calculateOverflowRegions(RelativeRect overflow, Rect containerRect);
    public void _reportOverflow(RelativeRect overflow, List<DiagnosticsNode>? overflowHints);
    public void paintOverflowIndicator(PaintingContext context, Offset offset, Rect containerRect, Rect childRect, List<DiagnosticsNode>? overflowHints = null);
    public void reassemble();
}

