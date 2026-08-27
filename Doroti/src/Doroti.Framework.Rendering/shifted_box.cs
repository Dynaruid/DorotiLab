// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/shifted_box.dart
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

public delegate BoxConstraints BoxConstraintsTransform(BoxConstraints constraints);

public abstract class RenderShiftedBox : RenderBox, RenderObjectWithChildMixin<RenderBox>
{
    public virtual RenderBox? _child { get; set; } = default;

    protected RenderShiftedBox(RenderBox? child)
    {
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return (child?.getMinIntrinsicWidth(height) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return (child?.getMaxIntrinsicWidth(height) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return (child?.getMinIntrinsicHeight(width) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return (child?.getMaxIntrinsicHeight(width) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        double? result = default!;
        RenderBox? childLocal = this.child;
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        if ((childLocal is not null))
        {
            DartRuntimePrimitives.Assert(() => !childLocal.debugNeedsLayout);
            result = childLocal.getDistanceToActualBaseline(baseline);
            var childParentData = ((BoxParentData?)(object?)childLocal.parentData!)!;
            if ((result is not null))
            {
                double result__1798__value2078 = DartRuntimePrimitives.RequireValue(result);
                result__1798__value2078 += ((BoxParentData)childParentData).offset.dy;
            }
        }
        else
        {
            result = base.computeDistanceToActualBaseline(baseline);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = this.child;
        if ((childLocal is null))
        {
            return null;
        }
        double? childBaseline = childLocal.getDryBaseline(constraints, baseline);
        if ((childBaseline is null))
        {
            return null;
        }
        return DartRuntimePrimitives.RequireValue(childBaseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderBox? childLocal = this.child;
        if ((childLocal is not null))
        {
            var childParentData = ((BoxParentData?)(object?)childLocal.parentData!)!;
            context.paintChild(childLocal, (((BoxParentData)childParentData).offset + offset));
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? childLocal = this.child;
        if ((childLocal is not null))
        {
            var childParentData = ((BoxParentData?)(object?)childLocal.parentData!)!;
            return result.addWithPaintOffset(offset: ((BoxParentData)childParentData).offset, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - ((BoxParentData)childParentData).offset))));
                return childLocal.hitTest(result, position: transformed);
                return default;
            })));
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

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

    public virtual RenderBox? child
    {
        get => this._child;
        set
        {
            var __value = value;
            if ((this._child is not null))
            {
                dropChild(this._child!);
            }
            this._child = __value;
            if ((this._child is not null))
            {
                adoptChild(this._child!);
            }
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        this._child?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        this._child?.detach();
    }

    public override void redepthChildren()
    {
        if ((this._child is not null))
        {
            redepthChild(this._child!);
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return ((this.child is not null) ? new List<DiagnosticsNode> { ((Diagnosticable)this.child!).toDiagnosticsNode(name: "child") } : new List<DiagnosticsNode>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RenderPadding : RenderShiftedBox
{
    internal virtual global::Doroti.Framework.Painting.EdgeInsets? _resolvedPaddingCache { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry _padding { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;

    public RenderPadding(global::Doroti.Framework.Painting.EdgeInsetsGeometry padding, TextDirection? textDirection = null, RenderBox? child = null) : base(child)
    {
        this._textDirection = textDirection;
        this._padding = padding;
        System.Diagnostics.Debug.Assert(((global::Doroti.Framework.Painting.EdgeInsetsGeometry)padding).isNonNegative);
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsets _resolvedPadding
    {
        get
        {
            global::Doroti.Framework.Painting.EdgeInsets returnValue = _resolvedPaddingCache ??= this.padding.resolve(this.textDirection);
            DartRuntimePrimitives.Assert(() => returnValue.isNonNegative);
            return returnValue;
            return default!;
        }
    }
    internal virtual void _markNeedResolution()
    {
        _resolvedPaddingCache = null;
        markNeedsLayout();
    }

    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding
    {
        get => this._padding;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)__value).isNonNegative);
            if ((object.Equals(this._padding, __value)))
            {
                return;
            }
            _padding = __value;
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
    public override double computeMinIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Painting.EdgeInsets padding = this._resolvedPadding;
        if ((child is not null))
        {
            return (child!.getMinIntrinsicWidth(Math.Max(0.0, (height - padding.vertical))) + padding.horizontal);
        }
        return padding.horizontal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Painting.EdgeInsets padding = this._resolvedPadding;
        if ((child is not null))
        {
            return (child!.getMaxIntrinsicWidth(Math.Max(0.0, (height - padding.vertical))) + padding.horizontal);
        }
        return padding.horizontal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Painting.EdgeInsets padding = this._resolvedPadding;
        if ((child is not null))
        {
            return (child!.getMinIntrinsicHeight(Math.Max(0.0, (width - padding.horizontal))) + padding.vertical);
        }
        return padding.vertical;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Painting.EdgeInsets padding = this._resolvedPadding;
        if ((child is not null))
        {
            return (child!.getMaxIntrinsicHeight(Math.Max(0.0, (width - padding.horizontal))) + padding.vertical);
        }
        return padding.vertical;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        global::Doroti.Framework.Painting.EdgeInsets padding = this._resolvedPadding;
        if ((child is null))
        {
            return constraints.constrain(new global::Doroti.Ui.Size(padding.horizontal, padding.vertical));
        }
        BoxConstraints innerConstraints = constraints.deflate(padding);
        global::Doroti.Ui.Size childSize = child!.getDryLayout(innerConstraints);
        return constraints.constrain(new global::Doroti.Ui.Size((padding.horizontal + childSize.width), (padding.vertical + childSize.height)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = this.child;
        if ((childLocal is null))
        {
            return null;
        }
        global::Doroti.Framework.Painting.EdgeInsets padding = this._resolvedPadding;
        BoxConstraints innerConstraints = constraints.deflate(padding);
        double? childBaseline = childLocal.getDryBaseline(innerConstraints, baseline);
        if ((childBaseline is null))
        {
            return null;
        }
        return (DartRuntimePrimitives.RequireValue(childBaseline) + ((global::Doroti.Framework.Painting.EdgeInsets)padding).top);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = this.constraints;
        global::Doroti.Framework.Painting.EdgeInsets padding = this._resolvedPadding;
        if ((child is null))
        {
            size = constraintsLocal.constrain(new global::Doroti.Ui.Size(padding.horizontal, padding.vertical));
            return;
        }
        BoxConstraints innerConstraints = constraintsLocal.deflate(padding);
        child!.layout(innerConstraints, parentUsesSize: true);
        var childParentData = ((BoxParentData?)(object?)child!.parentData!)!;
        childParentData.offset = new global::Doroti.Ui.Offset(((global::Doroti.Framework.Painting.EdgeInsets)padding).left, ((global::Doroti.Framework.Painting.EdgeInsets)padding).top);
        size = constraintsLocal.constrain(new global::Doroti.Ui.Size((padding.horizontal + child!.size.width), (padding.vertical + child!.size.height)));
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        base.debugPaintSize(context, offset);
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Ui.Rect outerRect = (offset & size);
                global::Doroti.Framework.Rendering.DebugLibrary.debugPaintPadding(((PaintingContext)context).canvas, outerRect, ((child is not null) ? this._resolvedPaddingCache!.deflateRect(outerRect) : null));
                return true;
            });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
    }

}

public abstract class RenderAligningShiftedBox : RenderShiftedBox
{
    internal virtual global::Doroti.Framework.Painting.Alignment? _resolvedAlignment { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.AlignmentGeometry _alignment { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;

    protected RenderAligningShiftedBox(global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = default!, RenderBox? child = null) : base(child)
    {
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Framework.Painting.Alignment.center;
        this._alignment = __alignment;
        this._textDirection = textDirection;
    }

    public virtual global::Doroti.Framework.Painting.Alignment resolvedAlignment => _resolvedAlignment ??= this.alignment.resolve(this.textDirection);
    internal virtual void _markNeedResolution()
    {
        _resolvedAlignment = null;
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
    public virtual void alignChild()
    {
        DartRuntimePrimitives.Assert(() => (child is not null));
        DartRuntimePrimitives.Assert(() => !child!.debugNeedsLayout);
        DartRuntimePrimitives.Assert(() => child!.hasSize);
        DartRuntimePrimitives.Assert(() => hasSize);
        var childParentData = ((BoxParentData?)(object?)child!.parentData!)!;
        childParentData.offset = this.resolvedAlignment.alongOffset((size - child!.size));
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
    }

}

public class RenderPositionedBox : RenderAligningShiftedBox
{
    internal virtual double? _widthFactor { get; set; } = default;
    internal virtual double? _heightFactor { get; set; } = default;

    public RenderPositionedBox(RenderBox? child = null, double? widthFactor = null, double? heightFactor = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null) : base(child: child, alignment: alignment ?? global::Doroti.Framework.Painting.Alignment.center, textDirection: DartRuntimePrimitives.RequireValue(textDirection))
    {
        this._widthFactor = widthFactor;
        this._heightFactor = heightFactor;
        System.Diagnostics.Debug.Assert(((widthFactor is null) || (widthFactor >= 0.0)));
        System.Diagnostics.Debug.Assert(((heightFactor is null) || (heightFactor >= 0.0)));
    }

    public virtual double? widthFactor
    {
        get => this._widthFactor;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || (__value >= 0.0)));
            if ((this._widthFactor == __value))
            {
                return;
            }
            _widthFactor = __value;
            markNeedsLayout();
        }
    }
    public virtual double? heightFactor
    {
        get => this._heightFactor;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || (__value >= 0.0)));
            if ((this._heightFactor == __value))
            {
                return;
            }
            _heightFactor = __value;
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        return (base.computeMinIntrinsicWidth(height) * ((this._widthFactor ?? 1L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return (base.computeMaxIntrinsicWidth(height) * ((this._widthFactor ?? 1L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return (base.computeMinIntrinsicHeight(width) * ((this._heightFactor ?? 1L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return (base.computeMaxIntrinsicHeight(width) * ((this._heightFactor ?? 1L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        bool shrinkWrapWidth = ((this._widthFactor is not null) || (((BoxConstraints)constraints).maxWidth == double.PositiveInfinity));
        bool shrinkWrapHeight = ((this._heightFactor is not null) || (((BoxConstraints)constraints).maxHeight == double.PositiveInfinity));
        if ((child is not null))
        {
            global::Doroti.Ui.Size childSize = child!.getDryLayout(constraints.loosen());
            return constraints.constrain(new global::Doroti.Ui.Size((shrinkWrapWidth ? (childSize.width * ((this._widthFactor ?? 1.0))) : double.PositiveInfinity), (shrinkWrapHeight ? (childSize.height * ((this._heightFactor ?? 1.0))) : double.PositiveInfinity)));
        }
        return constraints.constrain(new global::Doroti.Ui.Size((shrinkWrapWidth ? 0.0 : double.PositiveInfinity), (shrinkWrapHeight ? 0.0 : double.PositiveInfinity)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = this.constraints;
        bool shrinkWrapWidth = ((this._widthFactor is not null) || (((BoxConstraints)constraintsLocal).maxWidth == double.PositiveInfinity));
        bool shrinkWrapHeight = ((this._heightFactor is not null) || (((BoxConstraints)constraintsLocal).maxHeight == double.PositiveInfinity));
        if ((child is not null))
        {
            child!.layout(constraintsLocal.loosen(), parentUsesSize: true);
            size = constraintsLocal.constrain(new global::Doroti.Ui.Size((shrinkWrapWidth ? (child!.size.width * ((this._widthFactor ?? 1.0))) : double.PositiveInfinity), (shrinkWrapHeight ? (child!.size.height * ((this._heightFactor ?? 1.0))) : double.PositiveInfinity)));
            alignChild();
        }
        else
        {
            size = constraintsLocal.constrain(new global::Doroti.Ui.Size((shrinkWrapWidth ? 0.0 : double.PositiveInfinity), (shrinkWrapHeight ? 0.0 : double.PositiveInfinity)));
        }
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        base.debugPaintSize(context, offset);
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Ui.Paint paint = default!;
                if (((child is not null) && !child!.size.isEmpty))
                {
                    global::Doroti.Ui.Path path = default!;
                    paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 1.0;
    __cascade.color = new global::Doroti.Ui.Color(4294967040L);
    return __cascade;
}))();
                    path = new global::Doroti.Ui.Path();
                    var childParentData = ((BoxParentData?)(object?)child!.parentData!)!;
                    if ((((BoxParentData)childParentData).offset.dy > 0.0))
                    {
                        double headSize = Math.Min((((BoxParentData)childParentData).offset.dy * 0.2), 10.0);
                        ((Func<Path>)(() =>
{
    var __cascade = path;
    __cascade.moveTo((offset.dx + (size.width / 2.0)), offset.dy);
    __cascade.relativeLineTo(0.0, (((BoxParentData)childParentData).offset.dy - headSize));
    __cascade.relativeLineTo(headSize, 0.0);
    __cascade.relativeLineTo(-headSize, headSize);
    __cascade.relativeLineTo(-headSize, -headSize);
    __cascade.relativeLineTo(headSize, 0.0);
    __cascade.moveTo((offset.dx + (size.width / 2.0)), (offset.dy + size.height));
    __cascade.relativeLineTo(0.0, (-((BoxParentData)childParentData).offset.dy + headSize));
    __cascade.relativeLineTo(headSize, 0.0);
    __cascade.relativeLineTo(-headSize, -headSize);
    __cascade.relativeLineTo(-headSize, headSize);
    __cascade.relativeLineTo(headSize, 0.0);
    return __cascade;
}))();
                        ((PaintingContext)context).canvas.drawPath(path, paint);
                    }
                    if ((((BoxParentData)childParentData).offset.dx > 0.0))
                    {
                        double headSizeLocal = Math.Min((((BoxParentData)childParentData).offset.dx * 0.2), 10.0);
                        ((Func<Path>)(() =>
{
    var __cascade = path;
    __cascade.moveTo(offset.dx, (offset.dy + (size.height / 2.0)));
    __cascade.relativeLineTo((((BoxParentData)childParentData).offset.dx - headSizeLocal), 0.0);
    __cascade.relativeLineTo(0.0, headSizeLocal);
    __cascade.relativeLineTo(headSizeLocal, -headSizeLocal);
    __cascade.relativeLineTo(-headSizeLocal, -headSizeLocal);
    __cascade.relativeLineTo(0.0, headSizeLocal);
    __cascade.moveTo((offset.dx + size.width), (offset.dy + (size.height / 2.0)));
    __cascade.relativeLineTo((-((BoxParentData)childParentData).offset.dx + headSizeLocal), 0.0);
    __cascade.relativeLineTo(0.0, headSizeLocal);
    __cascade.relativeLineTo(-headSizeLocal, -headSizeLocal);
    __cascade.relativeLineTo(headSizeLocal, -headSizeLocal);
    __cascade.relativeLineTo(0.0, headSizeLocal);
    return __cascade;
}))();
                        ((PaintingContext)context).canvas.drawPath(path, paint);
                    }
                }
                else
                {
                    paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(2425393296L);
    return __cascade;
}))();
                    ((PaintingContext)context).canvas.drawRect((offset & size), paint);
                }
                return true;
            });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("widthFactor", this._widthFactor, ifNull: "expand"));
        properties.add(new DoubleProperty("heightFactor", this._heightFactor, ifNull: "expand"));
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = this.child;
        if ((childLocal is null))
        {
            return null;
        }
        BoxConstraints childConstraints = constraints.loosen();
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if ((result is null))
        {
            return null;
        }
        global::Doroti.Ui.Size childSize = childLocal.getDryLayout(childConstraints);
        bool shrinkWrapWidth = ((this._widthFactor is not null) || (((BoxConstraints)constraints).maxWidth == double.PositiveInfinity));
        bool shrinkWrapHeight = ((this._heightFactor is not null) || (((BoxConstraints)constraints).maxHeight == double.PositiveInfinity));
        global::Doroti.Ui.Size size = constraints.constrain(new global::Doroti.Ui.Size((shrinkWrapWidth ? (childSize.width * ((this._widthFactor ?? 1.0))) : double.PositiveInfinity), (shrinkWrapHeight ? (childSize.height * ((this._heightFactor ?? 1.0))) : double.PositiveInfinity)));
        global::Doroti.Ui.Offset childOffset = resolvedAlignment.alongOffset((size - childSize));
        return (DartRuntimePrimitives.RequireValue(result) + childOffset.dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum OverflowBoxFit
{
    max,
    deferToChild
}

public class RenderConstrainedOverflowBox : RenderAligningShiftedBox
{
    internal virtual double? _minWidth { get; set; } = default;
    internal virtual double? _maxWidth { get; set; } = default;
    internal virtual double? _minHeight { get; set; } = default;
    internal virtual double? _maxHeight { get; set; } = default;
    internal virtual OverflowBoxFit _fit { get; set; } = default!;

    public RenderConstrainedOverflowBox(RenderBox? child = null, double? minWidth = null, double? maxWidth = null, double? minHeight = null, double? maxHeight = null, OverflowBoxFit fit = OverflowBoxFit.max, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null) : base(child: child, alignment: alignment ?? global::Doroti.Framework.Painting.Alignment.center, textDirection: DartRuntimePrimitives.RequireValue(textDirection))
    {
        this._minWidth = minWidth;
        this._maxWidth = maxWidth;
        this._minHeight = minHeight;
        this._maxHeight = maxHeight;
        this._fit = fit;
    }

    public virtual double? minWidth
    {
        get => this._minWidth;
        set
        {
            var __value = value;
            if ((this._minWidth == __value))
            {
                return;
            }
            _minWidth = __value;
            markNeedsLayout();
        }
    }
    public virtual double? maxWidth
    {
        get => this._maxWidth;
        set
        {
            var __value = value;
            if ((this._maxWidth == __value))
            {
                return;
            }
            _maxWidth = __value;
            markNeedsLayout();
        }
    }
    public virtual double? minHeight
    {
        get => this._minHeight;
        set
        {
            var __value = value;
            if ((this._minHeight == __value))
            {
                return;
            }
            _minHeight = __value;
            markNeedsLayout();
        }
    }
    public virtual double? maxHeight
    {
        get => this._maxHeight;
        set
        {
            var __value = value;
            if ((this._maxHeight == __value))
            {
                return;
            }
            _maxHeight = __value;
            markNeedsLayout();
        }
    }
    public virtual OverflowBoxFit fit
    {
        get => this._fit;
        set
        {
            var __value = value;
            if ((object.Equals(this._fit, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _fit = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayoutForSizedByParentChange();
        }
    }
    internal virtual BoxConstraints _getInnerConstraints(BoxConstraints constraints)
    {
        return new BoxConstraints(minWidth: (this._minWidth ?? ((BoxConstraints)constraints).minWidth), maxWidth: (this._maxWidth ?? ((BoxConstraints)constraints).maxWidth), minHeight: (this._minHeight ?? ((BoxConstraints)constraints).minHeight), maxHeight: (this._maxHeight ?? ((BoxConstraints)constraints).maxHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool sizedByParent => (this.fit switch { OverflowBoxFit.max => true, OverflowBoxFit.deferToChild => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return (this.fit switch { OverflowBoxFit.max => ((BoxConstraints)constraints).biggest, OverflowBoxFit.deferToChild => (child?.getDryLayout(constraints) ?? ((BoxConstraints)constraints).smallest), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = this.child;
        if ((childLocal is null))
        {
            return null;
        }
        BoxConstraints childConstraints = _getInnerConstraints(constraints);
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if ((result is null))
        {
            return null;
        }
        global::Doroti.Ui.Size childSize = childLocal.getDryLayout(childConstraints);
        global::Doroti.Ui.Size size = getDryLayout(constraints);
        return (DartRuntimePrimitives.RequireValue(result) + resolvedAlignment.alongOffset((size - childSize)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        if ((child is not null))
        {
            child!.layout(_getInnerConstraints(constraints), parentUsesSize: true);
            switch (this.fit)
            {
                case OverflowBoxFit.max:
                    {
                        DartRuntimePrimitives.Assert(() => this.sizedByParent);
                        break;
                    }
                case OverflowBoxFit.deferToChild:
                    {
                        size = constraints.constrain(child!.size);
                        break;
                    }
            }
            alignChild();
        }
        else
        {
            switch (this.fit)
            {
                case OverflowBoxFit.max:
                    {
                        DartRuntimePrimitives.Assert(() => this.sizedByParent);
                        break;
                    }
                case OverflowBoxFit.deferToChild:
                    {
                        size = ((BoxConstraints)constraints).smallest;
                        break;
                    }
            }
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("minWidth", this.minWidth, ifNull: "use parent minWidth constraint"));
        properties.add(new DoubleProperty("maxWidth", this.maxWidth, ifNull: "use parent maxWidth constraint"));
        properties.add(new DoubleProperty("minHeight", this.minHeight, ifNull: "use parent minHeight constraint"));
        properties.add(new DoubleProperty("maxHeight", this.maxHeight, ifNull: "use parent maxHeight constraint"));
        properties.add(new EnumProperty<OverflowBoxFit>("fit", this.fit));
    }

}

public class RenderConstraintsTransformBox : RenderAligningShiftedBox, DebugOverflowIndicatorMixin
{
    internal virtual Func<BoxConstraints, BoxConstraints> _constraintsTransform { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = default!;
    internal virtual Rect _overflowContainerRect { get; set; } = Rect.zero;
    internal virtual Rect _overflowChildRect { get; set; } = Rect.zero;
    internal virtual bool _isOverflowing { get; set; } = false;
    internal virtual BoxConstraints? _childConstraints { get; set; } = default;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();
    public virtual List<global::Doroti.Framework.Painting.TextPainter> _indicatorLabel { get; set; } = new List<global::Doroti.Framework.Painting.TextPainter>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(System.Enum.GetValues<_OverflowSide__debug_overflow_indicator>().ToList().Count)))), ((i) => new global::Doroti.Framework.Painting.TextPainter(textDirection: TextDirection.ltr))));
    public virtual bool _overflowReportNeeded { get; set; } = true;

    public RenderConstraintsTransformBox(global::Doroti.Framework.Painting.AlignmentGeometry alignment, TextDirection? textDirection, Func<BoxConstraints, BoxConstraints> constraintsTransform, RenderBox? child = null, Clip clipBehavior = Clip.none) : base(alignment: alignment, textDirection: DartRuntimePrimitives.RequireValue(textDirection), child: child)
    {
        this._constraintsTransform = constraintsTransform;
        this._clipBehavior = clipBehavior;
    }

    public virtual Func<BoxConstraints, BoxConstraints> constraintsTransform
    {
        get => this._constraintsTransform;
        set
        {
            var __value = value;
            if ((object.Equals((Func<BoxConstraints, BoxConstraints>)this._constraintsTransform, (Func<BoxConstraints, BoxConstraints>)__value)))
            {
                return;
            }
            _constraintsTransform = __value;
            bool needsLayout = ((this._childConstraints is null) || (!object.Equals(this._childConstraints, __value(constraints))));
            if (needsLayout)
            {
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
            if ((!object.Equals(__value, this._clipBehavior)))
            {
                _clipBehavior = __value;
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public override double computeMinIntrinsicHeight(double width)
    {
        return base.computeMinIntrinsicHeight(this.constraintsTransform(new BoxConstraints(maxWidth: width)).maxWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return base.computeMaxIntrinsicHeight(this.constraintsTransform(new BoxConstraints(maxWidth: width)).maxWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return base.computeMinIntrinsicWidth(this.constraintsTransform(new BoxConstraints(maxHeight: height)).maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return base.computeMaxIntrinsicWidth(this.constraintsTransform(new BoxConstraints(maxHeight: height)).maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        global::Doroti.Ui.Size? childSize = child?.getDryLayout(this.constraintsTransform(constraints));
        return ((childSize is null) ? ((BoxConstraints)constraints).smallest : constraints.constrain(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(childSize))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = this.child;
        if ((childLocal is null))
        {
            return null;
        }
        BoxConstraints childConstraints = this.constraintsTransform(constraints);
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if ((result is null))
        {
            return null;
        }
        global::Doroti.Ui.Size childSize = childLocal.getDryLayout(childConstraints);
        global::Doroti.Ui.Size size = constraints.constrain(childSize);
        return (DartRuntimePrimitives.RequireValue(result) + resolvedAlignment.alongOffset((size - childSize)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = this.constraints;
        RenderBox? childLocal = this.child;
        if ((childLocal is not null))
        {
            BoxConstraints childConstraints = this.constraintsTransform(constraintsLocal);
            DartRuntimePrimitives.Assert(() => ((BoxConstraints)childConstraints).isNormalized);
            _childConstraints = childConstraints;
            childLocal.layout(childConstraints, parentUsesSize: true);
            size = constraintsLocal.constrain(((RenderBox)childLocal).size);
            alignChild();
            var childParentData = ((BoxParentData?)(object?)childLocal.parentData!)!;
            _overflowContainerRect = (Offset.zero & size);
            _overflowChildRect = (((BoxParentData)childParentData).offset & ((RenderBox)childLocal).size);
        }
        else
        {
            size = ((BoxConstraints)constraintsLocal).smallest;
            _overflowContainerRect = Rect.zero;
            _overflowChildRect = Rect.zero;
        }
        _isOverflowing = RelativeRect.CreateFromRect(this._overflowContainerRect, this._overflowChildRect).hasInsets;
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is null))
        {
            return;
        }
        if (!this._isOverflowing)
        {
            base.paint(context, offset);
            return;
        }
        this._clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, (Offset.zero & size), (Action<PaintingContext, Offset>)base.paint, clipBehavior: this.clipBehavior, oldLayer: ((LayerHandle<ClipRectLayer>)this._clipRectLayer).layer);
        DartRuntimePrimitives.Assert(() =>
            {
                if (size.isEmpty)
                {
                    return true;
                }
                switch (this.clipBehavior)
                {
                    case Clip.none:
                        {
                            paintOverflowIndicator(context, offset, this._overflowContainerRect, this._overflowChildRect);
                            break;
                        }
                    case Clip.hardEdge:
                    case Clip.antiAlias:
                    case Clip.antiAliasWithSaveLayer:
                        {
                            break;
                        }
                }
                return true;
            });
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        foreach (global::Doroti.Framework.Painting.TextPainter painter in this._indicatorLabel)
        {
            painter.dispose();
        }
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
                    return (this._isOverflowing ? (Offset.zero & size) : null);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShort()
    {
        string header = base.toStringShort();
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            if (this._isOverflowing)
            {
                header += " OVERFLOWING";
            }
        }
        return header;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string _formatPixels(double value)
    {
        DartRuntimePrimitives.Assert(() => (value > 0.0));
        return (value switch { > 10.0 => value.toStringAsFixed(0L), > 1.0 => value.toStringAsFixed(1L), _ => value.toStringAsPrecision(3L) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<_OverflowRegionData__debug_overflow_indicator> _calculateOverflowRegions(RelativeRect overflow, Rect containerRect)
    {
        var regions = new List<_OverflowRegionData__debug_overflow_indicator>();
        if ((((RelativeRect)overflow).left > 0.0))
        {
            var markerRect = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, (containerRect.width * DebugOverflowIndicatorMixin._indicatorFraction), containerRect.height);
            regions.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRect, label: $"LEFT OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).left)} PIXELS", labelOffset: (markerRect.centerLeft + new global::Doroti.Ui.Offset((DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels), 0.0)), rotation: (Dart_mathLibrary.pi / 2.0), side: _OverflowSide__debug_overflow_indicator.left));
        }
        if ((((RelativeRect)overflow).right > 0.0))
        {
            var markerRectLocal = global::Doroti.Ui.Rect.fromLTWH((containerRect.width * ((1.0 - DebugOverflowIndicatorMixin._indicatorFraction))), 0.0, (containerRect.width * DebugOverflowIndicatorMixin._indicatorFraction), containerRect.height);
            regions.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRectLocal, label: $"RIGHT OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).right)} PIXELS", labelOffset: (markerRectLocal.centerRight - new global::Doroti.Ui.Offset((DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels), 0.0)), rotation: (-Dart_mathLibrary.pi / 2.0), side: _OverflowSide__debug_overflow_indicator.right));
        }
        if ((((RelativeRect)overflow).top > 0.0))
        {
            var markerRectAlternate = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, containerRect.width, (containerRect.height * DebugOverflowIndicatorMixin._indicatorFraction));
            regions.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRectAlternate, label: $"TOP OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).top)} PIXELS", labelOffset: (markerRectAlternate.topCenter + new global::Doroti.Ui.Offset(0.0, DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels)), side: _OverflowSide__debug_overflow_indicator.top));
        }
        if ((((RelativeRect)overflow).bottom > 0.0))
        {
            var markerRectNested = global::Doroti.Ui.Rect.fromLTWH(0.0, (containerRect.height * ((1.0 - DebugOverflowIndicatorMixin._indicatorFraction))), containerRect.width, (containerRect.height * DebugOverflowIndicatorMixin._indicatorFraction));
            regions.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRectNested, label: $"BOTTOM OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).bottom)} PIXELS", labelOffset: (markerRectNested.bottomCenter - new global::Doroti.Ui.Offset(0.0, (DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels))), side: _OverflowSide__debug_overflow_indicator.bottom));
        }
        return regions;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _reportOverflow(RelativeRect overflow, List<DiagnosticsNode>? overflowHints)
    {
        overflowHints ??= new List<DiagnosticsNode>();
        if ((checked((long)(overflowHints.Count)) == 0))
        {
            overflowHints.Add(new ErrorDescription($"The edge of the {this.GetType()} that is " + "overflowing has been marked in the rendering with a yellow and black " + "striped pattern. This is usually caused by the contents being too big " + $"for the {this.GetType()}."));
            overflowHints.Add(new ErrorHint("This is considered an error condition because it indicates that there " + "is content that cannot be seen. If the content is legitimately bigger " + "than the available space, consider clipping it with a ClipRect widget " + $"before putting it in the {this.GetType()}, or using a scrollable " + "container, like a ListView."));
        }
        var overflows = new List<string>();
        var overflowText = "";
        DartRuntimePrimitives.Assert(() => (checked((long)(overflows.Count)) != 0));
        switch (checked((long)(overflows.Count)))
        {
            case 1L:
                {
                    overflowText = overflows.First();
                    break;
                }
            case 2L:
                {
                    overflowText = $"{overflows.First()} and {overflows.Last()}";
                    break;
                }
            default:
                {
                    overflows[(int)((checked((long)(overflows.Count)) - 1L))] = $"and {overflows[(int)((checked((long)(overflows.Count)) - 1L))]}";
                    overflowText = string.Join(", ", overflows);
                    break;
                }
        }
        FlutterError.reportError(new FlutterErrorDetails(exception: new FlutterError($"A {this.GetType()} overflowed by {overflowText}."), library: "rendering library", context: new ErrorDescription("during layout"), informationCollector: (() => new List<DiagnosticsNode> { describeForError($"The specific {this.GetType()} in question is"), new DiagnosticsNode(DartCoreExtensions.repeat("◢◤", ((checked((long)(FlutterError.wrapWidth / 2L))))), allowWrap: false) })));
    }

    public virtual void paintOverflowIndicator(PaintingContext context, Offset offset, Rect containerRect, Rect childRect, List<DiagnosticsNode>? overflowHints = null)
    {
        var overflow = RelativeRect.CreateFromRect(containerRect, childRect);
        if (((((((RelativeRect)overflow).left <= 0.0) && (((RelativeRect)overflow).right <= 0.0)) && (((RelativeRect)overflow).top <= 0.0)) && (((RelativeRect)overflow).bottom <= 0.0)))
        {
            return;
        }
        List<_OverflowRegionData__debug_overflow_indicator> overflowRegions = _calculateOverflowRegions(overflow, containerRect);
        foreach (var region in overflowRegions)
        {
            ((PaintingContext)context).canvas.drawRect(((_OverflowRegionData__debug_overflow_indicator)region).rect.shift(offset), DebugOverflowIndicatorMixin._indicatorPaint);
            var textSpan = ((global::Doroti.Framework.Painting.TextSpan?)(object?)this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region).side))].text)!;
            if ((textSpan?.text != ((_OverflowRegionData__debug_overflow_indicator)region).label))
            {
                this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region).side))].text = new global::Doroti.Framework.Painting.TextSpan(text: ((_OverflowRegionData__debug_overflow_indicator)region).label, style: DebugOverflowIndicatorMixin._indicatorTextStyle);
                this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region).side))].layout();
            }
            global::Doroti.Ui.Offset labelOffsetLocal = (((_OverflowRegionData__debug_overflow_indicator)region).labelOffset + offset);
            var centerOffset = new global::Doroti.Ui.Offset((-this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region).side))].width / 2.0), 0.0);
            global::Doroti.Ui.Rect textBackgroundRect = (centerOffset & this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region).side))].size);
            ((PaintingContext)context).canvas.save();
            ((PaintingContext)context).canvas.translate(labelOffsetLocal.dx, labelOffsetLocal.dy);
            ((PaintingContext)context).canvas.rotate(((_OverflowRegionData__debug_overflow_indicator)region).rotation);
            ((PaintingContext)context).canvas.drawRect(textBackgroundRect, DebugOverflowIndicatorMixin._labelBackgroundPaint);
            this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region).side))].paint(((PaintingContext)context).canvas, centerOffset);
            ((PaintingContext)context).canvas.restore();
        }
        if (this._overflowReportNeeded)
        {
            this._overflowReportNeeded = false;
            _reportOverflow(overflow, overflowHints);
        }
    }

    public override void reassemble()
    {
        base.reassemble();
        DartRuntimePrimitives.Assert(() =>
            {
                this._overflowReportNeeded = true;
                return true;
            });
    }

}

public class RenderSizedOverflowBox : RenderAligningShiftedBox
{
    internal virtual Size _requestedSize { get; set; } = default!;

    public RenderSizedOverflowBox(RenderBox? child = null, Size requestedSize = default!, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null) : base(child: child, alignment: alignment ?? global::Doroti.Framework.Painting.Alignment.center, textDirection: DartRuntimePrimitives.RequireValue(textDirection))
    {
        this._requestedSize = requestedSize;
    }

    public virtual global::Doroti.Ui.Size requestedSize
    {
        get => this._requestedSize;
        set
        {
            var __value = value;
            if ((object.Equals(this._requestedSize, __value)))
            {
                return;
            }
            _requestedSize = __value;
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        return this._requestedSize.width;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return this._requestedSize.width;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return this._requestedSize.height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return this._requestedSize.height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        RenderBox? childLocal = this.child;
        if ((childLocal is null))
        {
            return base.computeDistanceToActualBaseline(baseline);
        }
        double? result = childLocal.getDistanceToActualBaseline(baseline);
        if ((result is null))
        {
            return base.computeDistanceToActualBaseline(baseline);
        }
        var childParentData = ((BoxParentData?)(object?)childLocal.parentData!)!;
        return (DartRuntimePrimitives.RequireValue(result) + ((BoxParentData)childParentData).offset.dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = this.child;
        if ((childLocal is null))
        {
            return null;
        }
        double? result = childLocal.getDryBaseline(constraints, baseline);
        if ((result is null))
        {
            return null;
        }
        global::Doroti.Ui.Size childSize = childLocal.getDryLayout(constraints);
        global::Doroti.Ui.Size size = getDryLayout(constraints);
        return (DartRuntimePrimitives.RequireValue(result) + resolvedAlignment.alongOffset((size - childSize)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return constraints.constrain(this._requestedSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = constraints.constrain(this._requestedSize);
        if ((child is not null))
        {
            child!.layout(constraints, parentUsesSize: true);
            alignChild();
        }
    }

}

public class RenderFractionallySizedOverflowBox : RenderAligningShiftedBox
{
    internal virtual double? _widthFactor { get; set; } = default;
    internal virtual double? _heightFactor { get; set; } = default;

    public RenderFractionallySizedOverflowBox(RenderBox? child = null, double? widthFactor = null, double? heightFactor = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null) : base(child: child, alignment: alignment ?? global::Doroti.Framework.Painting.Alignment.center, textDirection: DartRuntimePrimitives.RequireValue(textDirection))
    {
        this._widthFactor = widthFactor;
        this._heightFactor = heightFactor;
    }

    public virtual double? widthFactor
    {
        get => this._widthFactor;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || (__value >= 0.0)));
            if ((this._widthFactor == __value))
            {
                return;
            }
            _widthFactor = __value;
            markNeedsLayout();
        }
    }
    public virtual double? heightFactor
    {
        get => this._heightFactor;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || (__value >= 0.0)));
            if ((this._heightFactor == __value))
            {
                return;
            }
            _heightFactor = __value;
            markNeedsLayout();
        }
    }
    internal virtual BoxConstraints _getInnerConstraints(BoxConstraints constraints)
    {
        double minWidthLocal = ((BoxConstraints)constraints).minWidth;
        double maxWidthLocal = ((BoxConstraints)constraints).maxWidth;
        if ((this._widthFactor is not null))
        {
            double width = (maxWidthLocal * DartRuntimePrimitives.RequireValue(this._widthFactor));
            minWidthLocal = width;
            maxWidthLocal = width;
        }
        double minHeightLocal = ((BoxConstraints)constraints).minHeight;
        double maxHeightLocal = ((BoxConstraints)constraints).maxHeight;
        if ((this._heightFactor is not null))
        {
            double height = (maxHeightLocal * DartRuntimePrimitives.RequireValue(this._heightFactor));
            minHeightLocal = height;
            maxHeightLocal = height;
        }
        return new BoxConstraints(minWidth: minWidthLocal, maxWidth: maxWidthLocal, minHeight: minHeightLocal, maxHeight: maxHeightLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        double result = default!;
        if ((child is null))
        {
            result = base.computeMinIntrinsicWidth(height);
        }
        else
        {
            result = child!.getMinIntrinsicWidth((height * ((this._heightFactor ?? 1.0))));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(result));
        return (result / ((this._widthFactor ?? 1.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double result = default!;
        if ((child is null))
        {
            result = base.computeMaxIntrinsicWidth(height);
        }
        else
        {
            result = child!.getMaxIntrinsicWidth((height * ((this._heightFactor ?? 1.0))));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(result));
        return (result / ((this._widthFactor ?? 1.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double result = default!;
        if ((child is null))
        {
            result = base.computeMinIntrinsicHeight(width);
        }
        else
        {
            result = child!.getMinIntrinsicHeight((width * ((this._widthFactor ?? 1.0))));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(result));
        return (result / ((this._heightFactor ?? 1.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        double result = default!;
        if ((child is null))
        {
            result = base.computeMaxIntrinsicHeight(width);
        }
        else
        {
            result = child!.getMaxIntrinsicHeight((width * ((this._widthFactor ?? 1.0))));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(result));
        return (result / ((this._heightFactor ?? 1.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if ((child is not null))
        {
            global::Doroti.Ui.Size childSize = child!.getDryLayout(_getInnerConstraints(constraints));
            return constraints.constrain(childSize);
        }
        return constraints.constrain(_getInnerConstraints(constraints).constrain(Size.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = this.child;
        if ((childLocal is null))
        {
            return null;
        }
        BoxConstraints childConstraints = _getInnerConstraints(constraints);
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if ((result is null))
        {
            return null;
        }
        global::Doroti.Ui.Size childSize = childLocal.getDryLayout(childConstraints);
        global::Doroti.Ui.Size size = getDryLayout(constraints);
        return (DartRuntimePrimitives.RequireValue(result) + resolvedAlignment.alongOffset((size - childSize)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        if ((child is not null))
        {
            child!.layout(_getInnerConstraints(constraints), parentUsesSize: true);
            size = constraints.constrain(child!.size);
            alignChild();
        }
        else
        {
            size = constraints.constrain(_getInnerConstraints(constraints).constrain(Size.zero));
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("widthFactor", this._widthFactor, ifNull: "pass-through"));
        properties.add(new DoubleProperty("heightFactor", this._heightFactor, ifNull: "pass-through"));
    }

}

public abstract class SingleChildLayoutDelegate
{
    internal virtual Listenable? _relayout { get; private set; }

    protected SingleChildLayoutDelegate(Listenable? relayout = null)
    {
        this._relayout = relayout;
    }

    public virtual global::Doroti.Ui.Size getSize(BoxConstraints constraints) => ((BoxConstraints)constraints).biggest;
    public virtual BoxConstraints getConstraintsForChild(BoxConstraints constraints) => constraints;
    public virtual global::Doroti.Ui.Offset getPositionForChild(Size size, Size childSize) => Offset.zero;
    public abstract bool shouldRelayout(SingleChildLayoutDelegate oldDelegate);
}

public class RenderCustomSingleChildLayoutBox : RenderShiftedBox
{
    internal virtual SingleChildLayoutDelegate _delegate { get; set; } = default!;

    public RenderCustomSingleChildLayoutBox(RenderBox? child = null, SingleChildLayoutDelegate @delegate = default!) : base(child)
    {
        this._delegate = @delegate;
    }

    public virtual SingleChildLayoutDelegate @delegate
    {
        get => this._delegate;
        set
        {
            var newDelegate = value;
            if ((object.Equals(this._delegate, newDelegate)))
            {
                return;
            }
            SingleChildLayoutDelegate oldDelegate = this._delegate;
            if (((!object.Equals(DartRuntimePrimitives.RuntimeType(newDelegate), DartRuntimePrimitives.RuntimeType(oldDelegate))) || newDelegate.shouldRelayout(oldDelegate)))
            {
                markNeedsLayout();
            }
            _delegate = newDelegate;
            if (attached)
            {
                ((SingleChildLayoutDelegate)oldDelegate)._relayout?.removeListener(markNeedsLayout);
                ((SingleChildLayoutDelegate)newDelegate)._relayout?.addListener(markNeedsLayout);
            }
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        ((SingleChildLayoutDelegate)this._delegate)._relayout?.addListener(markNeedsLayout);
    }

    public override void detach()
    {
        ((SingleChildLayoutDelegate)this._delegate)._relayout?.removeListener(markNeedsLayout);
        base.detach();
    }

    internal virtual global::Doroti.Ui.Size _getSize(BoxConstraints constraints)
    {
        return constraints.constrain(this._delegate.getSize(constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        double widthLocal = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(widthLocal))
        {
            return widthLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double widthLocal = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(widthLocal))
        {
            return widthLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double heightLocal = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(heightLocal))
        {
            return heightLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        double heightLocal = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(heightLocal))
        {
            return heightLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _getSize(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = this.child;
        if ((childLocal is null))
        {
            return null;
        }
        BoxConstraints childConstraints = this.@delegate.getConstraintsForChild(constraints);
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if ((result is null))
        {
            return null;
        }
        return (DartRuntimePrimitives.RequireValue(result) + this.@delegate.getPositionForChild(_getSize(constraints), (((BoxConstraints)childConstraints).isTight ? ((BoxConstraints)childConstraints).smallest : childLocal.getDryLayout(childConstraints))).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _getSize(constraints);
        if ((child is not null))
        {
            BoxConstraints childConstraints = this.@delegate.getConstraintsForChild(constraints);
            DartRuntimePrimitives.Assert(() => childConstraints.debugAssertIsValid(isAppliedConstraint: true));
            child!.layout(childConstraints, parentUsesSize: !((BoxConstraints)childConstraints).isTight);
            var childParentData = ((BoxParentData?)(object?)child!.parentData!)!;
            childParentData.offset = this.@delegate.getPositionForChild(size, (((BoxConstraints)childConstraints).isTight ? ((BoxConstraints)childConstraints).smallest : child!.size));
        }
    }

}

public class RenderBaseline : RenderShiftedBox
{
    internal virtual double _baseline { get; set; } = default!;
    internal virtual TextBaseline _baselineType { get; set; } = default!;

    public RenderBaseline(RenderBox? child = null, double baseline = default!, TextBaseline baselineType = default!) : base(child)
    {
        this._baseline = baseline;
        this._baselineType = baselineType;
    }

    public virtual double baseline
    {
        get => this._baseline;
        set
        {
            var __value = value;
            if ((this._baseline == __value))
            {
                return;
            }
            _baseline = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextBaseline baselineType
    {
        get => this._baselineType;
        set
        {
            var __value = value;
            if ((object.Equals(this._baselineType, __value)))
            {
                return;
            }
            _baselineType = __value;
            markNeedsLayout();
        }
    }
    internal virtual (global::Doroti.Ui.Size size, double top) _computeSizes(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild, Func<RenderBox, BoxConstraints, TextBaseline, double?> getBaseline)
    {
        RenderBox? childLocal = this.child;
        if ((childLocal is null))
        {
            return (size: ((BoxConstraints)constraints).smallest, top: 0);
        }
        BoxConstraints childConstraints = constraints.loosen();
        global::Doroti.Ui.Size childSize = layoutChild(childLocal, childConstraints);
        double childBaseline = (getBaseline(childLocal, childConstraints, this.baselineType) ?? childSize.height);
        double topLocal = (this.baseline - childBaseline);
        return (size: constraints.constrain(new global::Doroti.Ui.Size(childSize.width, (topLocal + childSize.height))), top: topLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSizes(constraints, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline).size;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = this.child;
        double? result1 = childLocal?.getDryBaseline(constraints.loosen(), baseline);
        double? result2 = childLocal?.getDryBaseline(constraints.loosen(), this.baselineType);
        if (((result1 is null) || (result2 is null)))
        {
            return null;
        }
        return ((this.baseline + DartRuntimePrimitives.RequireValue(result1)) - DartRuntimePrimitives.RequireValue(result2));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        var (sizeLocal, top) = _computeSizes(constraints, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getBaseline);
        this.size = sizeLocal;
        (((BoxParentData?)(object?)child?.parentData)!)?.offset = new global::Doroti.Ui.Offset(0.0, top);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("baseline", this.baseline));
        properties.add(new EnumProperty<global::Doroti.Ui.TextBaseline>("baselineType", this.baselineType));
    }

}

