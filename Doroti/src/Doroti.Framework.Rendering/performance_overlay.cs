// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/performance_overlay.dart
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

public enum PerformanceOverlayOption
{
    displayRasterizerStatistics,
    visualizeRasterizerStatistics,
    displayEngineStatistics,
    visualizeEngineStatistics
}

public class RenderPerformanceOverlay : RenderBox
{
    internal static long _rasterizerMask = (((1L << (int)(FoundationRuntimePorts.EnumIndex(PerformanceOverlayOption.displayRasterizerStatistics)))) | ((1L << (int)(FoundationRuntimePorts.EnumIndex(PerformanceOverlayOption.visualizeRasterizerStatistics)))));
    internal static long _engineMask = (((1L << (int)(FoundationRuntimePorts.EnumIndex(PerformanceOverlayOption.displayEngineStatistics)))) | ((1L << (int)(FoundationRuntimePorts.EnumIndex(PerformanceOverlayOption.visualizeEngineStatistics)))));
    internal virtual long _optionsMask { get; set; } = default!;

    public RenderPerformanceOverlay(long optionsMask = 0)
    {
        this._optionsMask = optionsMask;
    }

    public virtual long optionsMask
    {
        get => this._optionsMask;
        set
        {
            var __value = value;
            if ((__value == this._optionsMask))
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
            var kDefaultGraphHeight__4012 = 80.0;
            var result__4048 = 0.0;
            if ((((this.optionsMask & _rasterizerMask)) != 0L))
            {
                result__4048 += kDefaultGraphHeight__4012;
            }
            if ((((this.optionsMask & _engineMask)) != 0L))
            {
                result__4048 += kDefaultGraphHeight__4012;
            }
            return result__4048;
            return default!;
        }
    }
    public override double computeMinIntrinsicHeight(double width)
    {
        return this._intrinsicHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return this._intrinsicHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return constraints.constrain(new global::Doroti.Ui.Size(double.PositiveInfinity, this._intrinsicHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => needsCompositing);
        context.addLayer(new PerformanceOverlayLayer(overlayRect: global::Doroti.Ui.Rect.fromLTWH(offset.dx, offset.dy, size.width, size.height), optionsMask: this.optionsMask));
    }

}

