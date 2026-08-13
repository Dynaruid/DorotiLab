// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/overflow_bar.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public enum OverflowBarAlignment
{
    start,
    end,
    center
}

public class OverflowBar : MultiChildRenderObjectWidget
{
    public virtual double spacing { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.MainAxisAlignment? alignment { get; private set; }
    public virtual double overflowSpacing { get; private set; } = default!;
    public virtual OverflowBarAlignment overflowAlignment { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.VerticalDirection overflowDirection { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }

    public OverflowBar(global::Doroti.Generated.Framework.Foundation.Key? key = null, double spacing = 0.0, global::Doroti.Generated.Framework.Rendering.MainAxisAlignment? alignment = null, double overflowSpacing = 0.0, OverflowBarAlignment overflowAlignment = OverflowBarAlignment.start, global::Doroti.Generated.Framework.Painting.VerticalDirection overflowDirection = global::Doroti.Generated.Framework.Painting.VerticalDirection.down, TextDirection? textDirection = null, List<Widget> children = default!) : base(key: key, children: children ?? new List<Widget>())
    {
        this.spacing = spacing;
        this.alignment = alignment;
        this.overflowSpacing = overflowSpacing;
        this.overflowAlignment = overflowAlignment;
        this.overflowDirection = overflowDirection;
        this.textDirection = textDirection;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderOverflowBar__overflow_bar(spacing: this.spacing, alignment: this.alignment, overflowSpacing: this.overflowSpacing, overflowAlignment: this.overflowAlignment, overflowDirection: this.overflowDirection, textDirection: ((this.textDirection ?? (TextDirection)Directionality.of(context)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Ignore(((Func<_RenderOverflowBar__overflow_bar>)(() =>
{            var __cascade = (((_RenderOverflowBar__overflow_bar?)(object?)renderObject)!);
            __cascade.spacing = this.spacing;
            __cascade.alignment = this.alignment;
            __cascade.overflowSpacing = this.overflowSpacing;
            __cascade.overflowAlignment = this.overflowAlignment;
            __cascade.overflowDirection = this.overflowDirection;
            __cascade.textDirection = ((this.textDirection ?? (TextDirection)Directionality.of(context)));
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("spacing", this.spacing, defaultValue: 0L));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.MainAxisAlignment>("alignment", this.alignment, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("overflowSpacing", this.overflowSpacing, defaultValue: 0L));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<OverflowBarAlignment>("overflowAlignment", this.overflowAlignment, defaultValue: OverflowBarAlignment.start));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.VerticalDirection>("overflowDirection", this.overflowDirection, defaultValue: global::Doroti.Generated.Framework.Painting.VerticalDirection.down));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Flutter.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
    }

}

internal class _OverflowBarParentData__overflow_bar : global::Doroti.Generated.Framework.Rendering.ContainerBoxParentData<global::Doroti.Generated.Framework.Rendering.RenderBox>
{
}

internal class _RenderOverflowBar__overflow_bar : global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Generated.Framework.Rendering.RenderBox, _OverflowBarParentData__overflow_bar>, global::Doroti.Generated.Framework.Rendering.RenderBoxContainerDefaultsMixin<global::Doroti.Generated.Framework.Rendering.RenderBox, _OverflowBarParentData__overflow_bar>
{
    internal virtual double _spacing { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Rendering.MainAxisAlignment? _alignment { get; set; } = default;
    internal virtual double _overflowSpacing { get; set; } = default!;
    internal virtual OverflowBarAlignment _overflowAlignment { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.VerticalDirection _overflowDirection { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderOverflowBar__overflow_bar(List<global::Doroti.Generated.Framework.Rendering.RenderBox>? children = null, double spacing = 0.0, global::Doroti.Generated.Framework.Rendering.MainAxisAlignment? alignment = null, double overflowSpacing = 0.0, OverflowBarAlignment overflowAlignment = OverflowBarAlignment.start, global::Doroti.Generated.Framework.Painting.VerticalDirection overflowDirection = global::Doroti.Generated.Framework.Painting.VerticalDirection.down, TextDirection textDirection = default!)
    {
        this._spacing = spacing;
        this._alignment = alignment;
        this._overflowSpacing = overflowSpacing;
        this._overflowAlignment = overflowAlignment;
        this._overflowDirection = overflowDirection;
        this._textDirection = textDirection;
    }

    public virtual double spacing
    {
        get => this._spacing;
        set
        {
            var __value = value;
            if ((this._spacing == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _spacing = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Rendering.MainAxisAlignment? alignment
    {
        get => this._alignment;
        set
        {
            var __value = value;
            if ((object.Equals(this._alignment, __value)))
            {
                return;
            }
            _alignment = __value;
            markNeedsLayout();
        }
    }
    public virtual double overflowSpacing
    {
        get => this._overflowSpacing;
        set
        {
            var __value = value;
            if ((this._overflowSpacing == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _overflowSpacing = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual OverflowBarAlignment overflowAlignment
    {
        get => this._overflowAlignment;
        set
        {
            var __value = value;
            if ((object.Equals(this._overflowAlignment, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _overflowAlignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.VerticalDirection overflowDirection
    {
        get => this._overflowDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._overflowDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _overflowDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Flutter.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public override void setupParentData(global::Doroti.Generated.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not _OverflowBarParentData__overflow_bar))
        {
            __child.parentData = new _OverflowBarParentData__overflow_bar();
        }
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__11926 = this.firstChild;
        if ((child__11926 is null))
        {
            return 0;
        }
        var barWidth__12001 = 0.0;
        while ((child__11926 is not null))
        {
            barWidth__12001 += child__11926.getMinIntrinsicWidth(double.PositiveInfinity);
            child__11926 = childAfter(child__11926);
        }
        barWidth__12001 += (this.spacing * ((this.childCount - 1L)));
        var height__12200 = 0.0;
        if ((barWidth__12001 > width))
        {
            child__11926 = this.firstChild;
            while ((child__11926 is not null))
            {
                height__12200 += child__11926.getMinIntrinsicHeight(width);
                child__11926 = childAfter(child__11926);
            }
            return (height__12200 + (this.overflowSpacing * ((this.childCount - 1L))));
        }
        else
        {
            child__11926 = this.firstChild;
            while ((child__11926 is not null))
            {
                height__12200 = Math.Max(height__12200, child__11926.getMinIntrinsicHeight(width));
                child__11926 = childAfter(child__11926);
            }
            return height__12200;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__12746 = this.firstChild;
        if ((child__12746 is null))
        {
            return 0;
        }
        var barWidth__12821 = 0.0;
        while ((child__12746 is not null))
        {
            barWidth__12821 += child__12746.getMinIntrinsicWidth(double.PositiveInfinity);
            child__12746 = childAfter(child__12746);
        }
        barWidth__12821 += (this.spacing * ((this.childCount - 1L)));
        var height__13020 = 0.0;
        if ((barWidth__12821 > width))
        {
            child__12746 = this.firstChild;
            while ((child__12746 is not null))
            {
                height__13020 += child__12746.getMaxIntrinsicHeight(width);
                child__12746 = childAfter(child__12746);
            }
            return (height__13020 + (this.overflowSpacing * ((this.childCount - 1L))));
        }
        else
        {
            child__12746 = this.firstChild;
            while ((child__12746 is not null))
            {
                height__13020 = Math.Max(height__13020, child__12746.getMaxIntrinsicHeight(width));
                child__12746 = childAfter(child__12746);
            }
            return height__13020;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__13566 = this.firstChild;
        if ((child__13566 is null))
        {
            return 0;
        }
        var width__13641 = 0.0;
        while ((child__13566 is not null))
        {
            width__13641 += child__13566.getMinIntrinsicWidth(double.PositiveInfinity);
            child__13566 = childAfter(child__13566);
        }
        return (width__13641 + (this.spacing * ((this.childCount - 1L))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__13911 = this.firstChild;
        if ((child__13911 is null))
        {
            return 0;
        }
        var width__13986 = 0.0;
        while ((child__13911 is not null))
        {
            width__13986 += child__13911.getMaxIntrinsicWidth(double.PositiveInfinity);
            child__13911 = childAfter(child__13911);
        }
        return (width__13986 + (this.spacing * ((this.childCount - 1L))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return defaultComputeDistanceToHighestActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Generated.Framework.Rendering.BoxConstraints childConstraints__14449 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)constraints.loosen());
        var (next__14540, startChild__14563) = (this.overflowDirection switch { global::Doroti.Generated.Framework.Painting.VerticalDirection.down => (((global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.RenderBox?>, global::Doroti.Generated.Framework.Rendering.RenderBox?))((this.childAfter, this.firstChild))), global::Doroti.Generated.Framework.Painting.VerticalDirection.up => (((global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.RenderBox?>, global::Doroti.Generated.Framework.Rendering.RenderBox?))((this.childBefore, this.lastChild))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var maxChildHeight__14742 = 0.0;
        var y__14772 = 0.0;
        var childrenWidth__14789 = 0.0;
        global::Doroti.Generated.Framework.Rendering.BaselineOffset minHorizontalBaseline__14829 = global::Doroti.Generated.Framework.Rendering.BaselineOffset.noBaseline;
        global::Doroti.Generated.Framework.Rendering.BaselineOffset verticalBaseline__14899 = global::Doroti.Generated.Framework.Rendering.BaselineOffset.noBaseline;
        for (var child__14959 = startChild__14563; (child__14959 is not null); child__14959 = next__14540(child__14959))
        {
            global::Doroti.Flutter.Ui.Size childSize__15034 = ((global::Doroti.Flutter.Ui.Size)(object?)child__14959.getDryLayout(childConstraints__14449));
            double heightDiff__15103 = (childSize__15034.height - maxChildHeight__14742);
            if ((heightDiff__15103 > 0L))
            {
                minHorizontalBaseline__14829 = minHorizontalBaseline__14829.op_Add((heightDiff__15103 / 2L));
                maxChildHeight__14742 = childSize__15034.height;
            }
            var baselineOffset__15292 = new global::Doroti.Generated.Framework.Rendering.BaselineOffset(child__14959.getDryBaseline(childConstraints__14449, baseline));
            if ((baselineOffset__15292 is not null))
            {
                verticalBaseline__14899 ??= (baselineOffset__15292.op_Add(y__14772));
                minHorizontalBaseline__14829 = minHorizontalBaseline__14829.minOf((baselineOffset__15292.op_Add(((maxChildHeight__14742 - childSize__15034.height)))));
            }
            y__14772 += (childSize__15034.height + this.overflowSpacing);
            childrenWidth__14789 += childSize__15034.width;
        }
        DartRuntimePrimitives.Assert(() => (((verticalBaseline__14899 is null)) == ((minHorizontalBaseline__14829 is null))));
        return (((childrenWidth__14789 + (this.spacing * ((this.childCount - 1L)))) > ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth) ? verticalBaseline__14899.offset : minHorizontalBaseline__14829.offset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__16010 = this.firstChild;
        if ((child__16010 is null))
        {
            return ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).smallest;
        }
        global::Doroti.Generated.Framework.Rendering.BoxConstraints childConstraints__16121 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)constraints.loosen());
        var childrenWidth__16170 = 0.0;
        var maxChildHeight__16199 = 0.0;
        var y__16229 = 0.0;
        while ((child__16010 is not null))
        {
            global::Doroti.Flutter.Ui.Size childSize__16283 = ((global::Doroti.Flutter.Ui.Size)(object?)child__16010.getDryLayout(childConstraints__16121));
            childrenWidth__16170 += childSize__16283.width;
            maxChildHeight__16199 = Math.Max(maxChildHeight__16199, childSize__16283.height);
            y__16229 += (childSize__16283.height + this.overflowSpacing);
            child__16010 = childAfter(child__16010);
        }
        double actualWidth__16543 = (childrenWidth__16170 + (this.spacing * ((this.childCount - 1L))));
        if ((actualWidth__16543 > ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth))
        {
            return constraints.constrain(new global::Doroti.Flutter.Ui.Size(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth, (y__16229 - this.overflowSpacing)));
        }
        else
        {
            double overallWidth__16764 = ((this.alignment is null) ? actualWidth__16543 : ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth);
            return constraints.constrain(new global::Doroti.Flutter.Ui.Size(overallWidth__16764, maxChildHeight__16199));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__16970 = this.firstChild;
        if ((child__16970 is null))
        {
            size = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).smallest;
            return;
        }
        global::Doroti.Generated.Framework.Rendering.BoxConstraints childConstraints__17096 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)this.constraints.loosen());
        double childrenWidth__17148 = 0;
        double maxChildHeight__17178 = 0;
        double maxChildWidth__17209 = 0;
        while ((child__16970 is not null))
        {
            child__16970.layout(childConstraints__17096, parentUsesSize: true);
            childrenWidth__17148 += ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__16970).size.width;
            maxChildHeight__17178 = Math.Max(maxChildHeight__17178, ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__16970).size.height);
            maxChildWidth__17209 = Math.Max(maxChildWidth__17209, ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__16970).size.width);
            child__16970 = childAfter(child__16970);
        }
        var rtl__17541 = (object.Equals(this.textDirection, TextDirection.rtl));
        double actualWidth__17600 = (childrenWidth__17148 + (this.spacing * ((this.childCount - 1L))));
        if ((actualWidth__17600 > ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth))
        {
            child__16970 = ((object.Equals(this.overflowDirection, global::Doroti.Generated.Framework.Painting.VerticalDirection.down)) ? this.firstChild : this.lastChild);
            global::Doroti.Generated.Framework.Rendering.RenderBox? nextChild()
            {
                return ((object.Equals(this.overflowDirection, global::Doroti.Generated.Framework.Painting.VerticalDirection.down)) ? childAfter(child__16970!) : childBefore(child__16970!));
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            double y__17966 = 0;
            while ((child__16970 is not null))
            {
                var childParentData__18017 = ((_OverflowBarParentData__overflow_bar?)(object?)child__16970.parentData!)!;
                double x__18101 = (this.overflowAlignment switch { OverflowBarAlignment.center => (((((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth - ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__16970).size.width)) / 2L), OverflowBarAlignment.start => (rtl__17541 ? (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth - ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__16970).size.width) : 0), OverflowBarAlignment.end => (rtl__17541 ? 0 : (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth - ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__16970).size.width)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                childParentData__18017.offset = new global::Doroti.Flutter.Ui.Offset(x__18101, y__17966);
                y__17966 += (((global::Doroti.Generated.Framework.Rendering.RenderBox)child__16970).size.height + this.overflowSpacing);
                child__16970 = nextChild();
            }
            size = this.constraints.constrain(new global::Doroti.Flutter.Ui.Size(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth, (y__17966 - this.overflowSpacing)));
        }
        else
        {
            child__16970 = this.firstChild;
            double firstChildWidth__18725 = child__16970!.size.width;
            double overallWidth__18781 = ((this.alignment is null) ? actualWidth__17600 : ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth);
            size = this.constraints.constrain(new global::Doroti.Flutter.Ui.Size(overallWidth__18781, maxChildHeight__17178));
            double x__18943 = default!;
            double layoutSpacing__19003 = this.spacing;
            switch (this.alignment)
            {
                case null:
                    {
                        x__18943 = (rtl__17541 ? (this.size.width - firstChildWidth__18725) : 0);
                        break;
                    }
                case global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.start:
                    {
                        x__18943 = (rtl__17541 ? (this.size.width - firstChildWidth__18725) : 0);
                        break;
                    }
                case global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.center:
                    {
                        double halfRemainingWidth__19308 = (((this.size.width - actualWidth__17600)) / 2L);
                        x__18943 = (rtl__17541 ? ((this.size.width - halfRemainingWidth__19308) - firstChildWidth__18725) : halfRemainingWidth__19308);
                        break;
                    }
                case global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.end:
                    {
                        x__18943 = (rtl__17541 ? (actualWidth__17600 - firstChildWidth__18725) : (this.size.width - actualWidth__17600));
                        break;
                    }
                case global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.spaceBetween:
                    {
                        layoutSpacing__19003 = (((this.size.width - childrenWidth__17148)) / ((this.childCount - 1L)));
                        x__18943 = (rtl__17541 ? (this.size.width - firstChildWidth__18725) : 0);
                        break;
                    }
                case global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.spaceAround:
                    {
                        layoutSpacing__19003 = ((this.childCount > 0L) ? (((this.size.width - childrenWidth__17148)) / this.childCount) : 0);
                        x__18943 = (rtl__17541 ? ((this.size.width - (layoutSpacing__19003 / 2L)) - firstChildWidth__18725) : (layoutSpacing__19003 / 2L));
                        break;
                    }
                case global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.spaceEvenly:
                    {
                        layoutSpacing__19003 = (((this.size.width - childrenWidth__17148)) / ((this.childCount + 1L)));
                        x__18943 = (rtl__17541 ? ((this.size.width - layoutSpacing__19003) - firstChildWidth__18725) : layoutSpacing__19003);
                        break;
                    }
            }
            while ((child__16970 is not null))
            {
                var childParentData__20219 = ((_OverflowBarParentData__overflow_bar?)(object?)child__16970.parentData!)!;
                childParentData__20219.offset = new global::Doroti.Flutter.Ui.Offset(x__18943, (((maxChildHeight__17178 - ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__16970).size.height)) / 2L));
                if (!rtl__17541)
                {
                    x__18943 += (((global::Doroti.Generated.Framework.Rendering.RenderBox)child__16970).size.width + layoutSpacing__19003);
                }
                child__16970 = childAfter(child__16970);
                if ((rtl__17541 && (child__16970 is not null)))
                {
                    x__18943 -= (((global::Doroti.Generated.Framework.Rendering.RenderBox)child__16970).size.width + layoutSpacing__19003);
                }
            }
        }
    }

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        return defaultHitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        defaultPaint(context, offset);
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("spacing", this.spacing, defaultValue: 0L));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("overflowSpacing", this.overflowSpacing, defaultValue: 0L));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<OverflowBarAlignment>("overflowAlignment", this.overflowAlignment, defaultValue: OverflowBarAlignment.start));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.VerticalDirection>("overflowDirection", this.overflowDirection, defaultValue: global::Doroti.Generated.Framework.Painting.VerticalDirection.down));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Flutter.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((_OverflowBarParentData__overflow_bar?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((_OverflowBarParentData__overflow_bar?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((_OverflowBarParentData__overflow_bar?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((_OverflowBarParentData__overflow_bar?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => this._childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderBox))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData__175971 = ((_OverflowBarParentData__overflow_bar?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((_OverflowBarParentData__overflow_bar?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData__176343.previousSibling = child;
            }
            this._firstChild = child;
            this._lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this._firstChild is not null));
            DartRuntimePrimitives.Assert(() => (this._lastChild is not null));
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: this._firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: this._lastChild));
            var afterParentData__176766 = ((_OverflowBarParentData__overflow_bar?)(object?)after.parentData!)!;
            if ((afterParentData__176766.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData__175971.previousSibling = after;
                afterParentData__176766.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData__175971.nextSibling = afterParentData__176766.nextSibling;
                childParentData__175971.previousSibling = after;
                var childPreviousSiblingParentData__177424 = ((_OverflowBarParentData__overflow_bar?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((_OverflowBarParentData__overflow_bar?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is _OverflowBarParentData__overflow_bar), () => (object?)$"A child of {this.GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {typeof(_OverflowBarParentData__overflow_bar)}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {typeof(_OverflowBarParentData__overflow_bar)}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((global::System.Action<RenderBox>)this.add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData__179226 = ((_OverflowBarParentData__overflow_bar?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData__179226.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData__179226.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData__179613 = ((_OverflowBarParentData__overflow_bar?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((_OverflowBarParentData__overflow_bar?)(object?)childParentData__179226.nextSibling!.parentData!)!;
            childNextSiblingParentData__179965.previousSibling = childParentData__179226.previousSibling;
        }
        childParentData__179226.previousSibling = null;
        childParentData__179226.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child__180623 = this._firstChild;
        while ((child__180623 is not null))
        {
            var childParentData__180684 = ((_OverflowBarParentData__overflow_bar?)(object?)child__180623.parentData!)!;
            RenderBox? next__180762 = childParentData__180684.nextSibling;
            childParentData__180684.previousSibling = null;
            childParentData__180684.nextSibling = null;
            dropChild(child__180623);
            child__180623 = next__180762;
        }
        this._firstChild = null;
        this._lastChild = null;
        this._childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__181479 = ((_OverflowBarParentData__overflow_bar?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData__181479.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            ((dynamic)child__181803).attach(owner);
            var childParentData__181891 = ((_OverflowBarParentData__overflow_bar?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            ((dynamic)child__182065).detach();
            var childParentData__182148 = ((_OverflowBarParentData__overflow_bar?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((_OverflowBarParentData__overflow_bar?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((_OverflowBarParentData__overflow_bar?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((_OverflowBarParentData__overflow_bar?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((_OverflowBarParentData__overflow_bar?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children__183528 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child__183606 = this.firstChild!;
            var count__183637 = 1L;
            while (true)
            {
                children__183528.Add(((Diagnosticable)child__183606).toDiagnosticsNode(name: $"child__183606 {count__183637}"));
                if ((object.Equals(child__183606, this.lastChild)))
                {
                    break;
                }
                count__183637 += 1L;
                var childParentData__183833 = ((_OverflowBarParentData__overflow_bar?)(object?)child__183606.parentData!)!;
                child__183606 = childParentData__183833.nextSibling!;
            }
        }
        return children__183528;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        RenderBox? child__138717 = this.firstChild;
        while ((child__138717 is not null))
        {
            var childParentData__138777 = ((_OverflowBarParentData__overflow_bar?)(object?)child__138717.parentData!)!;
            double? result__138852 = child__138717.getDistanceToActualBaseline(baseline);
            if ((result__138852 is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result__138852);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData__138777.offset.dy);
            }
            child__138717 = childParentData__138777.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        BaselineOffset minBaseline__139372 = BaselineOffset.noBaseline;
        RenderBox? child__139428 = this.firstChild;
        while ((child__139428 is not null))
        {
            var childParentData__139488 = ((_OverflowBarParentData__overflow_bar?)(object?)child__139428.parentData!)!;
            BaselineOffset candidate__139570 = (new BaselineOffset(child__139428.getDistanceToActualBaseline(baseline)).op_Add(childParentData__139488.offset.dy));
            minBaseline__139372 = minBaseline__139372.minOf(candidate__139570);
            child__139428 = childParentData__139488.nextSibling;
        }
        return minBaseline__139372.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child__140279 = this.lastChild;
        while ((child__140279 is not null))
        {
            var childParentData__140418 = ((_OverflowBarParentData__overflow_bar?)(object?)child__140279.parentData!)!;
            bool isHit__140490 = result.addWithPaintOffset(offset: childParentData__140418.offset, position: position, hitTest: ((global::System.Func<BoxHitTestResult, Offset, bool>)((result, transformed) => {
DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData__140418.offset))));
return child__140279!.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            if (isHit__140490)
            {
                return true;
            }
            child__140279 = childParentData__140418.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child__141240 = this.firstChild;
        while ((child__141240 is not null))
        {
            var childParentData__141300 = ((_OverflowBarParentData__overflow_bar?)(object?)child__141240.parentData!)!;
            context.paintChild(child__141240, (childParentData__141300.offset + offset));
            child__141240 = childParentData__141300.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result__141793 = new List<RenderBox>();
        RenderBox? child__141832 = this.firstChild;
        while ((child__141832 is not null))
        {
            var childParentData__141892 = ((_OverflowBarParentData__overflow_bar?)(object?)child__141832.parentData!)!;
            result__141793.Add(((RenderBox?)(object?)child__141832)!);
            child__141832 = childParentData__141892.nextSibling;
        }
        return result__141793;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

