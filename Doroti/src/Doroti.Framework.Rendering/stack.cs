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
        var (left, right) = (textDirection switch { TextDirection.rtl => (((double, double))((end, start))), TextDirection.ltr => (((double, double))((start, end))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return new RelativeRect(left, top, right, bottom);
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
            double k = (1.0 - t);
            return new RelativeRect((b!.left * k), (((RelativeRect)b).top * k), (((RelativeRect)b).right * k), (((RelativeRect)b).bottom * k));
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
        double? widthLocal = ((this.left, this.right) switch { (double leftLocal, double rightLocal) => ((stackSize.width - rightLocal) - leftLocal), (_, _) => this.width });
        double? heightLocal = ((this.top, this.bottom) switch { (double topLocal, double bottomLocal) => ((stackSize.height - bottomLocal) - topLocal), (_, _) => this.height });
        DartRuntimePrimitives.Assert(() => ((heightLocal is null) || !double.IsNaN(DartRuntimePrimitives.RequireValue(heightLocal))));
        DartRuntimePrimitives.Assert(() => ((widthLocal is null) || !double.IsNaN(DartRuntimePrimitives.RequireValue(widthLocal))));
        return BoxConstraints.CreateTightFor(width: ((widthLocal is null) ? null : Math.Max(0.0, DartRuntimePrimitives.RequireValue(widthLocal))), height: ((heightLocal is null) ? null : Math.Max(0.0, DartRuntimePrimitives.RequireValue(heightLocal))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        var values = new List<string>();
        if ((checked((long)(values.Count)) == 0))
        {
            values.Add("not positioned");
        }
        values.Add(base.ToString());
        return string.Join("; ", values);
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
        var extent = 0.0;
        var child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            if (!((StackParentData)childParentData).isPositioned)
            {
                extent = Math.Max(extent, mainChildSizeGetter(child));
            }
            DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentData)));
            child = childParentData.nextSibling;
        }
        return extent;
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
        BoxConstraints childConstraints = childParentData.positionedChildConstraints(size);
        child.layout(childConstraints, parentUsesSize: true);
        double x = (childParentData switch { StackParentData { left: double leftLocal } __object19981 => leftLocal, StackParentData { right: double rightLocal } __object20033 => ((size.width - rightLocal) - ((RenderBox)child).size.width), StackParentData __object20119 => alignment.alongOffset((size - ((RenderBox)child).size)).dx, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double y = (childParentData switch { StackParentData { top: double topLocal } __object20257 => topLocal, StackParentData { bottom: double bottomLocal } __object20307 => ((size.height - bottomLocal) - ((RenderBox)child).size.height), StackParentData __object20397 => alignment.alongOffset((size - ((RenderBox)child).size)).dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        childParentData.offset = new global::Doroti.Ui.Offset(x, y);
        return ((((x < 0.0) || ((x + ((RenderBox)child).size.width) > size.width)) || (y < 0.0)) || ((y + ((RenderBox)child).size.height) > size.height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double? _baselineForChild(RenderBox child, Size stackSize, BoxConstraints nonPositionedChildConstraints, global::Doroti.Framework.Painting.Alignment alignment, TextBaseline baseline)
    {
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        BoxConstraints childConstraints = (((StackParentData)childParentData).isPositioned ? childParentData.positionedChildConstraints(stackSize) : nonPositionedChildConstraints);
        double? baselineOffset = child.getDryBaseline(childConstraints, baseline);
        if ((baselineOffset is null))
        {
            return null;
        }
        double y = (childParentData switch { StackParentData { top: double topLocal } __object21287 => topLocal, StackParentData { bottom: double bottomLocal } __object21337 => ((stackSize.height - bottomLocal) - child.getDryLayout(childConstraints).height), StackParentData __object21466 => alignment.alongOffset((stackSize - child.getDryLayout(childConstraints))).dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return (DartRuntimePrimitives.RequireValue(baselineOffset) + y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        BoxConstraints nonPositionedChildConstraints = (this.fit switch { StackFit.loose => constraints.loosen(), StackFit.expand => BoxConstraints.CreateTight(((BoxConstraints)constraints).biggest), StackFit.passthrough => constraints, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Painting.Alignment alignment = this._resolvedAlignment;
        global::Doroti.Ui.Size size = getDryLayout(constraints);
        BaselineOffset baselineOffset = BaselineOffset.noBaseline;
        for (RenderBox? child = firstChild; (child is not null); child = childAfter(child))
        {
            baselineOffset = baselineOffset.minOf(new BaselineOffset(_baselineForChild(child, size, nonPositionedChildConstraints, alignment, baseline)));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeSize(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild)
    {
        var hasNonPositionedChildren = false;
        if ((childCount == 0L))
        {
            return (((BoxConstraints)constraints).biggest.isFinite ? ((BoxConstraints)constraints).biggest : ((BoxConstraints)constraints).smallest);
        }
        double widthLocal = ((BoxConstraints)constraints).minWidth;
        double heightLocal = ((BoxConstraints)constraints).minHeight;
        BoxConstraints nonPositionedConstraints = (this.fit switch { StackFit.loose => constraints.loosen(), StackFit.expand => BoxConstraints.CreateTight(((BoxConstraints)constraints).biggest), StackFit.passthrough => constraints, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            if (!((StackParentData)childParentData).isPositioned)
            {
                hasNonPositionedChildren = true;
                global::Doroti.Ui.Size childSize = layoutChild(child, nonPositionedConstraints);
                widthLocal = Math.Max(widthLocal, childSize.width);
                heightLocal = Math.Max(heightLocal, childSize.height);
            }
            child = childParentData.nextSibling;
        }
        global::Doroti.Ui.Size size = default!;
        if (hasNonPositionedChildren)
        {
            size = new global::Doroti.Ui.Size(widthLocal, heightLocal);
            DartRuntimePrimitives.Assert(() => (size.width == constraints.constrainWidth(widthLocal)));
            DartRuntimePrimitives.Assert(() => (size.height == constraints.constrainHeight(heightLocal)));
        }
        else
        {
            size = ((BoxConstraints)constraints).biggest;
        }
        DartRuntimePrimitives.Assert(() => size.isFinite);
        return size;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = this.constraints;
        _hasVisualOverflow = false;
        size = _computeSize(constraints: constraintsLocal, layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild);
        global::Doroti.Framework.Painting.Alignment resolvedAlignment = this._resolvedAlignment;
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            if (!((StackParentData)childParentData).isPositioned)
            {
                childParentData.offset = resolvedAlignment.alongOffset((size - ((RenderBox)child).size));
            }
            else
            {
                _hasVisualOverflow = (layoutPositionedChild(child, childParentData, size, resolvedAlignment) || this._hasVisualOverflow);
            }
            DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentData)));
            child = childParentData.nextSibling;
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
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((StackParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((StackParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((StackParentData?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
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
            var afterParentData = ((StackParentData?)(object?)after.parentData!)!;
            if ((afterParentData.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((StackParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((StackParentData?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData.nextSibling, child)));
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
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((StackParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((StackParentData?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
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
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData.previousSibling, after)))
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
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.attach(owner);
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.detach();
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((StackParentData?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child = this.firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if ((object.Equals(child, this.lastChild)))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((StackParentData?)(object?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if ((result is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy);
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            BaselineOffset candidate = (new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy));
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while ((child is not null))
        {
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData.offset))));
                return child!.hitTest(result, position: transformed);
                return default;
            })));
            if (isHit)
            {
                return true;
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            context.paintChild(child, (childParentData.offset + offset));
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((StackParentData?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
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
        RenderBox? displayedChild = _childAtIndex();
        if ((displayedChild is not null))
        {
            visitor(displayedChild);
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
        long? index = FoundationRuntimePorts.EnumIndex(this);
        if ((index is null))
        {
            return null;
        }
        RenderBox? child = firstChild;
        for (var i = 0L; ((i < DartRuntimePrimitives.RequireValue(index)) && (child is not null)); i += 1L)
        {
            child = childAfter(child);
        }
        DartRuntimePrimitives.Assert(() => ((firstChild is null) || (child is not null)));
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        RenderBox? displayedChild = _childAtIndex();
        if ((displayedChild is null))
        {
            return null;
        }
        var childParentData = ((StackParentData?)(object?)displayedChild.parentData!)!;
        BaselineOffset offsetLocal = (new BaselineOffset(displayedChild.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy));
        return offsetLocal.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? displayedChild = _childAtIndex();
        if ((displayedChild is null))
        {
            return null;
        }
        BoxConstraints nonPositionedChildConstraints = (fit switch { StackFit.loose => constraints.loosen(), StackFit.expand => BoxConstraints.CreateTight(((BoxConstraints)constraints).biggest), StackFit.passthrough => constraints, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Painting.Alignment alignment = _resolvedAlignment;
        global::Doroti.Ui.Size size = getDryLayout(constraints);
        return RenderStack._baselineForChild(displayedChild, size, nonPositionedChildConstraints, alignment, baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? displayedChild = _childAtIndex();
        if ((displayedChild is null))
        {
            return false;
        }
        var childParentData = ((StackParentData?)(object?)displayedChild.parentData!)!;
        return result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData.offset))));
            return displayedChild.hitTest(result, position: transformed);
            return default;
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintStack(PaintingContext context, Offset offset)
    {
        RenderBox? displayedChild = _childAtIndex();
        if ((displayedChild is null))
        {
            return;
        }
        var childParentData = ((StackParentData?)(object?)displayedChild.parentData!)!;
        context.paintChild(displayedChild, (childParentData.offset + offset));
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("index", this.index));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        var i = 0L;
        RenderObject? child = firstChild;
        while ((child is not null))
        {
            children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child {(i + 1L)}", style: ((i != this.index) ? DiagnosticsTreeStyle.offstage : null)));
            child = (((StackParentData?)(object?)((RenderObject)child).parentData!)!).nextSibling;
            i += 1L;
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

