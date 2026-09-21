// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/debug_overflow_indicator.dart
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public enum _OverflowSide__debug_overflow_indicator
{
    left,
    top,
    bottom,
    right,
}

public class _OverflowRegionData__debug_overflow_indicator
{
    public virtual Rect rect { get; private set; } = default!;
    public virtual string label { get; private set; } = default!;
    public virtual Offset labelOffset { get; private set; } = default!;
    public virtual double rotation { get; private set; } = default!;
    public virtual _OverflowSide__debug_overflow_indicator side { get; private set; } = default!;

    internal _OverflowRegionData__debug_overflow_indicator(
        Rect rect,
        string label = "",
        Offset labelOffset = default,
        double rotation = 0.0,
        _OverflowSide__debug_overflow_indicator side = default!
    )
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
    internal static Color _black = new Color(3204448256L);
    internal static Color _yellow = new Color(3221225216L);
    internal const double _indicatorFraction = 0.1;
    internal const double _indicatorFontSizePixels = 7.5;
    internal const double _indicatorLabelPaddingPixels = 1.0;
    internal static Painting.TextStyle _indicatorTextStyle = new Painting.TextStyle(
        color: new Color(4287627264L),
        fontSize: _indicatorFontSizePixels,
        fontWeight: FontWeight.w800
    );
    internal static Paint _indicatorPaint = (
        (Func<Paint>)(
            () =>
            {
                var __cascade = new Paint();
                __cascade.shader = Ui.Gradient.linear(
                    Offset.zero,
                    new Offset(10.0, 10.0),
                    new List<Color> { _black, _yellow, _yellow, _black },
                    new List<double> { 0.25, 0.25, 0.75, 0.75 },
                    TileMode.repeated
                );
                return __cascade;
            }
        )
    )();
    internal static Paint _labelBackgroundPaint = (
        (Func<Paint>)(
            () =>
            {
                var __cascade = new Paint();
                __cascade.color = new Color(4294967295L);
                return __cascade;
            }
        )
    )();
    List<TextPainter> _indicatorLabel { get; }
    bool _overflowReportNeeded { get; set; }

    public void dispose();
    public string _formatPixels(double value);
    public List<_OverflowRegionData__debug_overflow_indicator> _calculateOverflowRegions(
        RelativeRect overflow,
        Rect containerRect
    );
    public void _reportOverflow(RelativeRect overflow, List<DiagnosticsNode>? overflowHints);
    public void paintOverflowIndicator(
        PaintingContext context,
        Offset offset,
        Rect containerRect,
        Rect childRect,
        List<DiagnosticsNode>? overflowHints = null
    );
    public void reassemble();
}
