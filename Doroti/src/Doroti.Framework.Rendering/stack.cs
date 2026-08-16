// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/stack.dart
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

public class RelativeRect
{
    public static RelativeRect fill = new RelativeRect(0.0, 0.0, 0.0, 0.0);
    public virtual double left { get; private set; } = default!;
    public virtual double top { get; private set; } = default!;
    public virtual double right { get; private set; } = default!;
    public virtual double bottom { get; private set; } = default!;

    public RelativeRect(double left, double top, double right, double bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }

    public static RelativeRect CreateFromSize(Rect rect, Size container)
    {
        var __instance = new RelativeRect(default!, default!, default!, default!);
        __instance.left = rect.left;
        __instance.top = rect.top;
        __instance.right = (container.width - rect.right);
        __instance.bottom = (container.height - rect.bottom);
        return __instance;
    }

    public static RelativeRect CreateFromRect(Rect rect, Rect container)
    {
        var __instance = new RelativeRect(default!, default!, default!, default!);
        __instance.left = (rect.left - container.left);
        __instance.top = (rect.top - container.top);
        __instance.right = (container.right - rect.right);
        __instance.bottom = (container.bottom - rect.bottom);
        return __instance;
    }

    public static RelativeRect CreateFromDirectional(TextDirection textDirection, double start, double top, double end, double bottom)
    {
        var (left__3056, right__3069) = (textDirection switch { TextDirection.rtl => (((double, double))((end, start))), TextDirection.ltr => (((double, double))((start, end))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return new RelativeRect(left__3056, top, right__3069, bottom);
    }

    public virtual bool hasInsets => ((((this.left > 0.0) || (this.top > 0.0)) || (this.right > 0.0)) || (this.bottom > 0.0));
    public virtual RelativeRect shift(Offset offset)
    {
        return new RelativeRect((this.left + offset.dx), (this.top + offset.dy), (this.right - offset.dx), (this.bottom - offset.dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RelativeRect inflate(double delta)
    {
        return new RelativeRect((this.left - delta), (this.top - delta), (this.right - delta), (this.bottom - delta));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RelativeRect deflate(double delta)
    {
        return inflate(-delta);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RelativeRect intersect(RelativeRect other)
    {
        return new RelativeRect(Math.Max(this.left, ((RelativeRect)other).left), Math.Max(this.top, ((RelativeRect)other).top), Math.Max(this.right, ((RelativeRect)other).right), Math.Max(this.bottom, ((RelativeRect)other).bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Rect toRect(Rect container)
    {
        return global::Doroti.Ui.Rect.fromLTRB(this.left, this.top, (container.width - this.right), (container.height - this.bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size toSize(Size container)
    {
        return new global::Doroti.Ui.Size(((container.width - this.left) - this.right), ((container.height - this.top) - this.bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static RelativeRect? lerp(RelativeRect? a, RelativeRect? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return new RelativeRect((b!.left * t), (((RelativeRect)b).top * t), (((RelativeRect)b).right * t), (((RelativeRect)b).bottom * t));
        }
        if ((b is null))
        {
            double k__6540 = (1.0 - t);
            return new RelativeRect((b!.left * k__6540), (((RelativeRect)b).top * k__6540), (((RelativeRect)b).right * k__6540), (((RelativeRect)b).bottom * k__6540));
        }
        return new RelativeRect(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((RelativeRect)a).left, ((RelativeRect)b).left, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((RelativeRect)a).top, ((RelativeRect)b).top, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((RelativeRect)a).right, ((RelativeRect)b).right, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((RelativeRect)a).bottom, ((RelativeRect)b).bottom, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as RelativeRect;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (((((__other is RelativeRect) && (((RelativeRect)((RelativeRect)__other)).left == this.left)) && (((RelativeRect)((RelativeRect)__other)).top == this.top)) && (((RelativeRect)((RelativeRect)__other)).right == this.right)) && (((RelativeRect)((RelativeRect)__other)).bottom == this.bottom));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.left, this.top, this.right, this.bottom);
    public override string ToString() => $"RelativeRect.fromLTRB({this.left.toStringAsFixed(1L)}, {this.top.toStringAsFixed(1L)}, {this.right.toStringAsFixed(1L)}, {this.bottom.toStringAsFixed(1L)})";
}

public class StackParentData : ContainerBoxParentData<RenderBox>
{
    public virtual double? top { get; set; } = default;
    public virtual double? right { get; set; } = default;
    public virtual double? bottom { get; set; } = default;
    public virtual double? left { get; set; } = default;
    public virtual double? width { get; set; } = default;
    public virtual double? height { get; set; } = default;

    public virtual RelativeRect rect
    {
        get => new RelativeRect(DartRuntimePrimitives.RequireValue(this.left), DartRuntimePrimitives.RequireValue(this.top), DartRuntimePrimitives.RequireValue(this.right), DartRuntimePrimitives.RequireValue(this.bottom));
        set
        {
            var __value = value;
            top = ((RelativeRect)__value).top;
            right = ((RelativeRect)__value).right;
            bottom = ((RelativeRect)__value).bottom;
            left = ((RelativeRect)__value).left;
        }
    }
    public virtual bool isPositioned => ((((((this.top is not null) || (this.right is not null)) || (this.bottom is not null)) || (this.left is not null)) || (this.width is not null)) || (this.height is not null));
    public virtual BoxConstraints positionedChildConstraints(Size stackSize)
    {
        DartRuntimePrimitives.Assert(() => this.isPositioned);
        double? width__9197 = ((this.left, this.right) switch { (double left__9250, double right__9270) => ((stackSize.width - right__9270) - left__9250), (_, _) => this.width });
        double? height__9367 = ((this.top, this.bottom) switch { (double top__9421, double bottom__9440) => ((stackSize.height - bottom__9440) - top__9421), (_, _) => this.height });
        DartRuntimePrimitives.Assert(() => ((height__9367 is null) || !double.IsNaN(DartRuntimePrimitives.RequireValue(height__9367))));
        DartRuntimePrimitives.Assert(() => ((width__9197 is null) || !double.IsNaN(DartRuntimePrimitives.RequireValue(width__9197))));
        return BoxConstraints.CreateTightFor(width: ((width__9197 is null) ? null : Math.Max(0.0, DartRuntimePrimitives.RequireValue(width__9197))), height: ((height__9367 is null) ? null : Math.Max(0.0, DartRuntimePrimitives.RequireValue(height__9367))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        var values__9820 = new List<string>();
        if ((checked((long)(values__9820.Count)) == 0))
        {
            values__9820.Add("not positioned");
        }
        values__9820.Add(base.ToString());
        return string.Join("; ", values__9820);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum StackFit
{
    loose,
    expand,
    passthrough
}

public class RenderStack : RenderBox, ContainerRenderObjectMixin<RenderBox, StackParentData>, RenderBoxContainerDefaultsMixin<RenderBox, StackParentData>
{
    internal virtual bool _hasVisualOverflow { get; set; } = false;
    internal virtual global::Doroti.Framework.Painting.Alignment? _resolvedAlignmentCache { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.AlignmentGeometry _alignment { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual StackFit _fit { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    public RenderStack(List<RenderBox>? children = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null, StackFit fit = StackFit.loose, Clip clipBehavior = Clip.hardEdge)
    {
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Framework.Painting.AlignmentDirectional.topStart;
        this._alignment = __alignment;
        this._textDirection = textDirection;
        this._fit = fit;
        this._clipBehavior = clipBehavior;
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        if ((__child.parentData is not StackParentData))
        {
            __child.parentData = new StackParentData();
        }
    }

    internal virtual global::Doroti.Framework.Painting.Alignment _resolvedAlignment => _resolvedAlignmentCache ??= this.alignment.resolve(this.textDirection);
    internal virtual void _markNeedResolution()
    {
        _resolvedAlignmentCache = null;
        markNeedsLayout();
    }

    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment
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
            _markNeedResolution();
        }
    }
    public virtual global::Doroti.Ui.TextDirection? textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, __value)))
            {
                return;
            }
            _textDirection = __value;
            _markNeedResolution();
        }
    }
    public virtual StackFit fit
    {
        get => this._fit;
        set
        {
            var __value = value;
            if ((!object.Equals(this._fit, DartRuntimePrimitives.RequireValue(__value))))
            {
                _fit = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(__value), this._clipBehavior)))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public static double getIntrinsicDimension(RenderBox? firstChild, Func<RenderBox, double> mainChildSizeGetter)
    {
        var extent__18104 = 0.0;
        var child__18126 = firstChild;
        while ((child__18126 is not null))
        {
            var childParentData__18186 = ((StackParentData?)(object?)child__18126.parentData!)!;
            if (!((StackParentData)childParentData__18186).isPositioned)
            {
                extent__18104 = Math.Max(extent__18104, mainChildSizeGetter(child__18126));
            }
            DartRuntimePrimitives.Assert(() => (object.Equals(child__18126.parentData, childParentData__18186)));
            child__18126 = childParentData__18186.nextSibling;
        }
        return extent__18104;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return getIntrinsicDimension(firstChild, ((Func<RenderBox, double>)((child) => child.getMinIntrinsicWidth(height))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return getIntrinsicDimension(firstChild, ((Func<RenderBox, double>)((child) => child.getMaxIntrinsicWidth(height))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return getIntrinsicDimension(firstChild, ((Func<RenderBox, double>)((child) => child.getMinIntrinsicHeight(width))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return getIntrinsicDimension(firstChild, ((Func<RenderBox, double>)((child) => child.getMaxIntrinsicHeight(width))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return defaultComputeDistanceToHighestActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool layoutPositionedChild(RenderBox child, StackParentData childParentData, Size size, global::Doroti.Framework.Painting.Alignment alignment)
    {
        DartRuntimePrimitives.Assert(() => ((StackParentData)childParentData).isPositioned);
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentData)));
        BoxConstraints childConstraints__19799 = childParentData.positionedChildConstraints(size);
        child.layout(childConstraints__19799, parentUsesSize: true);
        double x__19944 = (childParentData switch { StackParentData { left: double left__20011 } __object19981 => left__20011, StackParentData { right: double right__20063 } __object20033 => ((size.width - right__20063) - ((RenderBox)child).size.width), StackParentData __object20119 => alignment.alongOffset((size - ((RenderBox)child).size)).dx, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double y__20220 = (childParentData switch { StackParentData { top: double top__20287 } __object20257 => top__20287, StackParentData { bottom: double bottom__20337 } __object20307 => ((size.height - bottom__20337) - ((RenderBox)child).size.height), StackParentData __object20397 => alignment.alongOffset((size - ((RenderBox)child).size)).dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        childParentData.offset = new global::Doroti.Ui.Offset(x__19944, y__20220);
        return ((((x__19944 < 0.0) || ((x__19944 + ((RenderBox)child).size.width) > size.width)) || (y__20220 < 0.0)) || ((y__20220 + ((RenderBox)child).size.height) > size.height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double? _baselineForChild(RenderBox child, Size stackSize, BoxConstraints nonPositionedChildConstraints, global::Doroti.Framework.Painting.Alignment alignment, TextBaseline baseline)
    {
        var childParentData__20855 = ((StackParentData?)(object?)child.parentData!)!;
        BoxConstraints childConstraints__20936 = (((StackParentData)childParentData__20855).isPositioned ? childParentData__20855.positionedChildConstraints(stackSize) : nonPositionedChildConstraints);
        double? baselineOffset__21107 = child.getDryBaseline(childConstraints__20936, baseline);
        if ((baselineOffset__21107 is null))
        {
            return null;
        }
        double y__21250 = (childParentData__20855 switch { StackParentData { top: double top__21317 } __object21287 => top__21317, StackParentData { bottom: double bottom__21367 } __object21337 => ((stackSize.height - bottom__21367) - child.getDryLayout(childConstraints__20936).height), StackParentData __object21466 => alignment.alongOffset((stackSize - child.getDryLayout(childConstraints__20936))).dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return (DartRuntimePrimitives.RequireValue(baselineOffset__21107) + y__21250);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        BoxConstraints nonPositionedChildConstraints__21743 = (this.fit switch { StackFit.loose => constraints.loosen(), StackFit.expand => BoxConstraints.CreateTight(((BoxConstraints)constraints).biggest), StackFit.passthrough => constraints, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Painting.Alignment alignment__21975 = this._resolvedAlignment;
        global::Doroti.Ui.Size size__22022 = getDryLayout(constraints);
        BaselineOffset baselineOffset__22076 = BaselineOffset.noBaseline;
        for (RenderBox? child__22140 = firstChild; (child__22140 is not null); child__22140 = childAfter(child__22140))
        {
            baselineOffset__22076 = baselineOffset__22076.minOf(new BaselineOffset(_baselineForChild(child__22140, size__22022, nonPositionedChildConstraints__21743, alignment__21975, baseline)));
        }
        return baselineOffset__22076.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeSize(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild)
    {
        var hasNonPositionedChildren__22729 = false;
        if ((childCount == 0L))
        {
            return (((BoxConstraints)constraints).biggest.isFinite ? ((BoxConstraints)constraints).biggest : ((BoxConstraints)constraints).smallest);
        }
        double width__22896 = ((BoxConstraints)constraints).minWidth;
        double height__22937 = ((BoxConstraints)constraints).minHeight;
        BoxConstraints nonPositionedConstraints__22995 = (this.fit switch { StackFit.loose => constraints.loosen(), StackFit.expand => BoxConstraints.CreateTight(((BoxConstraints)constraints).biggest), StackFit.passthrough => constraints, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        RenderBox? child__23217 = firstChild;
        while ((child__23217 is not null))
        {
            var childParentData__23277 = ((StackParentData?)(object?)child__23217.parentData!)!;
            if (!((StackParentData)childParentData__23277).isPositioned)
            {
                hasNonPositionedChildren__22729 = true;
                global::Doroti.Ui.Size childSize__23438 = layoutChild(child__23217, nonPositionedConstraints__22995);
                width__22896 = Math.Max(width__22896, childSize__23438.width);
                height__22937 = Math.Max(height__22937, childSize__23438.height);
            }
            child__23217 = childParentData__23277.nextSibling;
        }
        global::Doroti.Ui.Size size__23674 = default!;
        if (hasNonPositionedChildren__22729)
        {
            size__23674 = new global::Doroti.Ui.Size(width__22896, height__22937);
            DartRuntimePrimitives.Assert(() => (size__23674.width == constraints.constrainWidth(width__22896)));
            DartRuntimePrimitives.Assert(() => (size__23674.height == constraints.constrainHeight(height__22937)));
        }
        else
        {
            size__23674 = ((BoxConstraints)constraints).biggest;
        }
        DartRuntimePrimitives.Assert(() => size__23674.isFinite);
        return size__23674;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraints__24507 = this.constraints;
        _hasVisualOverflow = false;
        size = _computeSize(constraints: constraints__24507, layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild);
        global::Doroti.Framework.Painting.Alignment resolvedAlignment__24688 = this._resolvedAlignment;
        RenderBox? child__24743 = firstChild;
        while ((child__24743 is not null))
        {
            var childParentData__24803 = ((StackParentData?)(object?)child__24743.parentData!)!;
            if (!((StackParentData)childParentData__24803).isPositioned)
            {
                childParentData__24803.offset = resolvedAlignment__24688.alongOffset((size - ((RenderBox)child__24743).size));
            }
            else
            {
                _hasVisualOverflow = (layoutPositionedChild(child__24743, childParentData__24803, size, resolvedAlignment__24688) || this._hasVisualOverflow);
            }
            DartRuntimePrimitives.Assert(() => (object.Equals(child__24743.parentData, childParentData__24803)));
            child__24743 = childParentData__24803.nextSibling;
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        return defaultHitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paintStack(PaintingContext context, Offset offset)
    {
        defaultPaint(context, offset);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (((!object.Equals(this.clipBehavior, Clip.none)) && this._hasVisualOverflow))
        {
            this._clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, (Offset.zero & size), (Action<PaintingContext, Offset>)this.paintStack, clipBehavior: this.clipBehavior, oldLayer: ((LayerHandle<ClipRectLayer>)this._clipRectLayer).layer);
        }
        else
        {
            this._clipRectLayer.layer = null;
            paintStack(context, offset);
        }
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        base.dispose();
    }

    public override Rect? describeApproximatePaintClip(RenderObject child)
    {
        switch (this.clipBehavior)
        {
            case Clip.none:
                {
                    return null;
                }
            case Clip.hardEdge:
            case Clip.antiAlias:
            case Clip.antiAliasWithSaveLayer:
                {
                    return (this._hasVisualOverflow ? (Offset.zero & size) : null);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection));
        properties.add(new EnumProperty<StackFit>("fit", this.fit));
        properties.add(new EnumProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: Clip.hardEdge));
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((StackParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((StackParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((StackParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((StackParentData?)(object?)child.parentData!)!;
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
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData__175971 = ((StackParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((StackParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData__176766 = ((StackParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData__177424 = ((StackParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((StackParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is StackParentData));
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach(this.add);
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData__179226 = ((StackParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData__179613 = ((StackParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((StackParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
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
            var childParentData__180684 = ((StackParentData?)(object?)child__180623.parentData!)!;
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
        var childParentData__181479 = ((StackParentData?)(object?)child.parentData!)!;
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
            child__181803.attach(owner);
            var childParentData__181891 = ((StackParentData?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((StackParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((StackParentData?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((StackParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((StackParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((StackParentData?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children__183528 = new List<DiagnosticsNode>();
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
                var childParentData__183833 = ((StackParentData?)(object?)child__183606.parentData!)!;
                child__183606 = childParentData__183833.nextSibling!;
            }
        }
        return children__183528;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        RenderBox? child__138717 = firstChild;
        while ((child__138717 is not null))
        {
            var childParentData__138777 = ((StackParentData?)(object?)child__138717.parentData!)!;
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
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset minBaseline__139372 = BaselineOffset.noBaseline;
        RenderBox? child__139428 = firstChild;
        while ((child__139428 is not null))
        {
            var childParentData__139488 = ((StackParentData?)(object?)child__139428.parentData!)!;
            BaselineOffset candidate__139570 = (new BaselineOffset(child__139428.getDistanceToActualBaseline(baseline)).op_Add(childParentData__139488.offset.dy));
            minBaseline__139372 = minBaseline__139372.minOf(candidate__139570);
            child__139428 = childParentData__139488.nextSibling;
        }
        return minBaseline__139372.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child__140279 = lastChild;
        while ((child__140279 is not null))
        {
            var childParentData__140418 = ((StackParentData?)(object?)child__140279.parentData!)!;
            bool isHit__140490 = result.addWithPaintOffset(offset: childParentData__140418.offset, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData__140418.offset))));
                return child__140279!.hitTest(result, position: transformed);
                return default;
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
        RenderBox? child__141240 = firstChild;
        while ((child__141240 is not null))
        {
            var childParentData__141300 = ((StackParentData?)(object?)child__141240.parentData!)!;
            context.paintChild(child__141240, (childParentData__141300.offset + offset));
            child__141240 = childParentData__141300.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result__141793 = new List<RenderBox>();
        RenderBox? child__141832 = firstChild;
        while ((child__141832 is not null))
        {
            var childParentData__141892 = ((StackParentData?)(object?)child__141832.parentData!)!;
            result__141793.Add(((RenderBox?)(object?)child__141832)!);
            child__141832 = childParentData__141892.nextSibling;
        }
        return result__141793;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RenderIndexedStack : RenderStack
{
    internal virtual long? _index { get; set; } = default;

    public RenderIndexedStack(List<RenderBox>? children = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null, StackFit fit = StackFit.loose, Clip clipBehavior = Clip.hardEdge, long? index = 0) : base(children: children, alignment: alignment ?? global::Doroti.Framework.Painting.AlignmentDirectional.topStart, textDirection: DartRuntimePrimitives.RequireValue(textDirection), fit: fit, clipBehavior: clipBehavior)
    {
        this._index = index;
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        RenderBox? displayedChild__27820 = _childAtIndex();
        if ((displayedChild__27820 is not null))
        {
            visitor(displayedChild__27820);
        }
    }

    public virtual long? index
    {
        get => this._index;
        set
        {
            var __value = value;
            if ((this._index != __value))
            {
                _index = __value;
                markNeedsLayout();
            }
        }
    }
    internal virtual RenderBox? _childAtIndex()
    {
        long? index__28204 = FoundationRuntimePorts.EnumIndex(this);
        if ((index__28204 is null))
        {
            return null;
        }
        RenderBox? child__28289 = firstChild;
        for (var i__28322 = 0L; ((i__28322 < DartRuntimePrimitives.RequireValue(index__28204)) && (child__28289 is not null)); i__28322 += 1L)
        {
            child__28289 = childAfter(child__28289);
        }
        DartRuntimePrimitives.Assert(() => ((firstChild is null) || (child__28289 is not null)));
        return child__28289;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        RenderBox? displayedChild__28578 = _childAtIndex();
        if ((displayedChild__28578 is null))
        {
            return null;
        }
        var childParentData__28681 = ((StackParentData?)(object?)displayedChild__28578.parentData!)!;
        BaselineOffset offset__28771 = (new BaselineOffset(displayedChild__28578.getDistanceToActualBaseline(baseline)).op_Add(childParentData__28681.offset.dy));
        return offset__28771.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? displayedChild__29040 = _childAtIndex();
        if ((displayedChild__29040 is null))
        {
            return null;
        }
        BoxConstraints nonPositionedChildConstraints__29158 = (fit switch { StackFit.loose => constraints.loosen(), StackFit.expand => BoxConstraints.CreateTight(((BoxConstraints)constraints).biggest), StackFit.passthrough => constraints, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Painting.Alignment alignment__29390 = _resolvedAlignment;
        global::Doroti.Ui.Size size__29437 = getDryLayout(constraints);
        return RenderStack._baselineForChild(displayedChild__29040, size__29437, nonPositionedChildConstraints__29158, alignment__29390, baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? displayedChild__29741 = _childAtIndex();
        if ((displayedChild__29741 is null))
        {
            return false;
        }
        var childParentData__29845 = ((StackParentData?)(object?)displayedChild__29741.parentData!)!;
        return result.addWithPaintOffset(offset: childParentData__29845.offset, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData__29845.offset))));
            return displayedChild__29741.hitTest(result, position: transformed);
            return default;
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintStack(PaintingContext context, Offset offset)
    {
        RenderBox? displayedChild__30325 = _childAtIndex();
        if ((displayedChild__30325 is null))
        {
            return;
        }
        var childParentData__30423 = ((StackParentData?)(object?)displayedChild__30325.parentData!)!;
        context.paintChild(displayedChild__30325, (childParentData__30423.offset + offset));
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("index", this.index));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children__30816 = new List<DiagnosticsNode>();
        var i__30856 = 0L;
        RenderObject? child__30881 = firstChild;
        while ((child__30881 is not null))
        {
            children__30816.Add(((Diagnosticable)child__30881).toDiagnosticsNode(name: $"child {(i__30856 + 1L)}", style: ((i__30856 != this.index) ? DiagnosticsTreeStyle.offstage : null)));
            child__30881 = (((StackParentData?)(object?)((RenderObject)child__30881).parentData!)!).nextSibling;
            i__30856 += 1L;
        }
        return children__30816;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

