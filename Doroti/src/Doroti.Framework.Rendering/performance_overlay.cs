// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/performance_overlay.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public enum PerformanceOverlayOption
{
    displayRasterizerStatistics,
    visualizeRasterizerStatistics,
    displayEngineStatistics,
    visualizeEngineStatistics,
}

public class RenderPerformanceOverlay : RenderBox
{
    internal static long _rasterizerMask =
        (
            1L
            << (int)
                FoundationRuntimePorts.EnumIndex(
                    PerformanceOverlayOption.displayRasterizerStatistics
                )
        )
        | (
            1L
            << (int)
                FoundationRuntimePorts.EnumIndex(
                    PerformanceOverlayOption.visualizeRasterizerStatistics
                )
        );
    internal static long _engineMask =
        (
            1L
            << (int)
                FoundationRuntimePorts.EnumIndex(PerformanceOverlayOption.displayEngineStatistics)
        )
        | (
            1L
            << (int)
                FoundationRuntimePorts.EnumIndex(PerformanceOverlayOption.visualizeEngineStatistics)
        );
    internal virtual long _optionsMask { get; set; } = default!;

    public RenderPerformanceOverlay(long optionsMask = 0)
    {
        _optionsMask = optionsMask;
    }

    public virtual long optionsMask
    {
        get => _optionsMask;
        set
        {
            var __value = value;
            if (__value == _optionsMask)
            {
                return;
            }
            _optionsMask = __value;
            markNeedsPaint();
        }
    }
    public override bool sizedByParent => true;
    public override bool alwaysNeedsCompositing => true;

    public override double computeMinIntrinsicWidth(double height)
    {
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _intrinsicHeight
    {
        get
        {
            var kDefaultGraphHeight = 80.0;
            var result = 0.0;
            if ((optionsMask & _rasterizerMask) != 0L)
            {
                result += kDefaultGraphHeight;
            }
            if ((optionsMask & _engineMask) != 0L)
            {
                result += kDefaultGraphHeight;
            }
            return result;
        }
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return _intrinsicHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return _intrinsicHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return constraints.constrain(new Size(double.PositiveInfinity, _intrinsicHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => needsCompositing);
        context.addLayer(
            new PerformanceOverlayLayer(
                overlayRect: Rect.fromLTWH(offset.dx, offset.dy, size.width, size.height),
                optionsMask: optionsMask
            )
        );
    }
}
