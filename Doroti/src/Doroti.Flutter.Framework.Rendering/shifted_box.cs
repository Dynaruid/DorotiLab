// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/shifted_box.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Rendering;

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
        double? result__1798 = default!;
        RenderBox? child__1827 = this.child;
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        if ((child__1827 is not null))
        {
            DartRuntimePrimitives.Assert(() => !child__1827.debugNeedsLayout);
            result__1798 = child__1827.getDistanceToActualBaseline(baseline);
            var childParentData__2014 = ((BoxParentData?)(object?)child__1827.parentData!)!;
            if ((result__1798 is not null))
            {
                double result__1798__value2078 = DartRuntimePrimitives.RequireValue(result__1798);
                result__1798__value2078 += ((BoxParentData)childParentData__2014).offset.dy;
            }
        }
        else
        {
            result__1798 = base.computeDistanceToActualBaseline(baseline);
        }
        return result__1798;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? child__2381 = this.child;
        if ((child__2381 is null))
        {
            return null;
        }
        double? childBaseline__2561 = child__2381.getDryBaseline(constraints, baseline);
        if ((childBaseline__2561 is null))
        {
            return null;
        }
        return DartRuntimePrimitives.RequireValue(childBaseline__2561);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderBox? child__2880 = this.child;
        if ((child__2880 is not null))
        {
            var childParentData__2937 = ((BoxParentData?)(object?)child__2880.parentData!)!;
            context.paintChild(child__2880, (((BoxParentData)childParentData__2937).offset + offset));
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child__3179 = this.child;
        if ((child__3179 is not null))
        {
            var childParentData__3236 = ((BoxParentData?)(object?)child__3179.parentData!)!;
            return result.addWithPaintOffset(offset: ((BoxParentData)childParentData__3236).offset, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - ((BoxParentData)childParentData__3236).offset))));
                return child__3179.hitTest(result, position: transformed);
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
    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? _resolvedPaddingCache { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry _padding { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;

    public RenderPadding(global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding, TextDirection? textDirection = null, RenderBox? child = null) : base(child)
    {
        this._textDirection = textDirection;
        this._padding = padding;
        System.Diagnostics.Debug.Assert(((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)padding).isNonNegative);
    }

    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsets _resolvedPadding
    {
        get
        {
            global::Doroti.Generated.Framework.Painting.EdgeInsets returnValue__4479 = _resolvedPaddingCache ??= this.padding.resolve(this.textDirection);
            DartRuntimePrimitives.Assert(() => returnValue__4479.isNonNegative);
            return returnValue__4479;
            return default!;
        }
    }
    internal virtual void _markNeedResolution()
    {
        _resolvedPaddingCache = null;
        markNeedsLayout();
    }

    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding
    {
        get => this._padding;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)__value).isNonNegative);
            if ((object.Equals(this._padding, __value)))
            {
                return;
            }
            _padding = __value;
            _markNeedResolution();
        }
    }
    public virtual global::Doroti.Flutter.Ui.TextDirection? textDirection
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
        global::Doroti.Generated.Framework.Painting.EdgeInsets padding__5661 = this._resolvedPadding;
        if ((child is not null))
        {
            return (child!.getMinIntrinsicWidth(Math.Max(0.0, (height - padding__5661.vertical))) + padding__5661.horizontal);
        }
        return padding__5661.horizontal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsets padding__6002 = this._resolvedPadding;
        if ((child is not null))
        {
            return (child!.getMaxIntrinsicWidth(Math.Max(0.0, (height - padding__6002.vertical))) + padding__6002.horizontal);
        }
        return padding__6002.horizontal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsets padding__6343 = this._resolvedPadding;
        if ((child is not null))
        {
            return (child!.getMinIntrinsicHeight(Math.Max(0.0, (width - padding__6343.horizontal))) + padding__6343.vertical);
        }
        return padding__6343.vertical;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsets padding__6682 = this._resolvedPadding;
        if ((child is not null))
        {
            return (child!.getMaxIntrinsicHeight(Math.Max(0.0, (width - padding__6682.horizontal))) + padding__6682.vertical);
        }
        return padding__6682.vertical;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsets padding__7047 = this._resolvedPadding;
        if ((child is null))
        {
            return constraints.constrain(new global::Doroti.Flutter.Ui.Size(padding__7047.horizontal, padding__7047.vertical));
        }
        BoxConstraints innerConstraints__7211 = constraints.deflate(padding__7047);
        global::Doroti.Flutter.Ui.Size childSize__7275 = child!.getDryLayout(innerConstraints__7211);
        return constraints.constrain(new global::Doroti.Flutter.Ui.Size((padding__7047.horizontal + childSize__7275.width), (padding__7047.vertical + childSize__7275.height)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? child__7584 = this.child;
        if ((child__7584 is null))
        {
            return null;
        }
        global::Doroti.Generated.Framework.Painting.EdgeInsets padding__7675 = this._resolvedPadding;
        BoxConstraints innerConstraints__7728 = constraints.deflate(padding__7675);
        double? childBaseline__7795 = child__7584.getDryBaseline(innerConstraints__7728, baseline);
        if ((childBaseline__7795 is null))
        {
            return null;
        }
        return (DartRuntimePrimitives.RequireValue(childBaseline__7795) + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__7675).top);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraints__8026 = this.constraints;
        global::Doroti.Generated.Framework.Painting.EdgeInsets padding__8079 = this._resolvedPadding;
        if ((child is null))
        {
            size = constraints__8026.constrain(new global::Doroti.Flutter.Ui.Size(padding__8079.horizontal, padding__8079.vertical));
            return;
        }
        BoxConstraints innerConstraints__8257 = constraints__8026.deflate(padding__8079);
        child!.layout(innerConstraints__8257, parentUsesSize: true);
        var childParentData__8375 = ((BoxParentData?)(object?)child!.parentData!)!;
        childParentData__8375.offset = new global::Doroti.Flutter.Ui.Offset(((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__8079).left, ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__8079).top);
        size = constraints__8026.constrain(new global::Doroti.Flutter.Ui.Size((padding__8079.horizontal + child!.size.width), (padding__8079.vertical + child!.size.height)));
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        base.debugPaintSize(context, offset);
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Flutter.Ui.Rect outerRect__8783 = (offset & size);
                global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPaintPadding(((PaintingContext)context).canvas, outerRect__8783, ((child is not null) ? this._resolvedPaddingCache!.deflateRect(outerRect__8783) : null));
                return true;
            });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding));
        properties.add(new EnumProperty<global::Doroti.Flutter.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
    }

}

public abstract class RenderAligningShiftedBox : RenderShiftedBox
{
    internal virtual global::Doroti.Generated.Framework.Painting.Alignment? _resolvedAlignment { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry _alignment { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;

    protected RenderAligningShiftedBox(global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = default!, RenderBox? child = null) : base(child)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this._alignment = __alignment;
        this._textDirection = textDirection;
    }

    public virtual global::Doroti.Generated.Framework.Painting.Alignment resolvedAlignment => _resolvedAlignment ??= this.alignment.resolve(this.textDirection);
    internal virtual void _markNeedResolution()
    {
        _resolvedAlignment = null;
        markNeedsLayout();
    }

    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment
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
    public virtual global::Doroti.Flutter.Ui.TextDirection? textDirection
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
        var childParentData__12344 = ((BoxParentData?)(object?)child!.parentData!)!;
        childParentData__12344.offset = this.resolvedAlignment.alongOffset((size - child!.size));
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new EnumProperty<global::Doroti.Flutter.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
    }

}

public class RenderPositionedBox : RenderAligningShiftedBox
{
    internal virtual double? _widthFactor { get; set; } = default;
    internal virtual double? _heightFactor { get; set; } = default;

    public RenderPositionedBox(RenderBox? child = null, double? widthFactor = null, double? heightFactor = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null) : base(child: child, alignment: alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center, textDirection: DartRuntimePrimitives.RequireValue(textDirection))
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
        bool shrinkWrapWidth__15265 = ((this._widthFactor is not null) || (((BoxConstraints)constraints).maxWidth == double.PositiveInfinity));
        bool shrinkWrapHeight__15363 = ((this._heightFactor is not null) || (((BoxConstraints)constraints).maxHeight == double.PositiveInfinity));
        if ((child is not null))
        {
            global::Doroti.Flutter.Ui.Size childSize__15491 = child!.getDryLayout(constraints.loosen());
            return constraints.constrain(new global::Doroti.Flutter.Ui.Size((shrinkWrapWidth__15265 ? (childSize__15491.width * ((this._widthFactor ?? 1.0))) : double.PositiveInfinity), (shrinkWrapHeight__15363 ? (childSize__15491.height * ((this._heightFactor ?? 1.0))) : double.PositiveInfinity)));
        }
        return constraints.constrain(new global::Doroti.Flutter.Ui.Size((shrinkWrapWidth__15265 ? 0.0 : double.PositiveInfinity), (shrinkWrapHeight__15363 ? 0.0 : double.PositiveInfinity)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraints__16002 = this.constraints;
        bool shrinkWrapWidth__16049 = ((this._widthFactor is not null) || (((BoxConstraints)constraints__16002).maxWidth == double.PositiveInfinity));
        bool shrinkWrapHeight__16147 = ((this._heightFactor is not null) || (((BoxConstraints)constraints__16002).maxHeight == double.PositiveInfinity));
        if ((child is not null))
        {
            child!.layout(constraints__16002.loosen(), parentUsesSize: true);
            size = constraints__16002.constrain(new global::Doroti.Flutter.Ui.Size((shrinkWrapWidth__16049 ? (child!.size.width * ((this._widthFactor ?? 1.0))) : double.PositiveInfinity), (shrinkWrapHeight__16147 ? (child!.size.height * ((this._heightFactor ?? 1.0))) : double.PositiveInfinity)));
            alignChild();
        }
        else
        {
            size = constraints__16002.constrain(new global::Doroti.Flutter.Ui.Size((shrinkWrapWidth__16049 ? 0.0 : double.PositiveInfinity), (shrinkWrapHeight__16147 ? 0.0 : double.PositiveInfinity)));
        }
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        base.debugPaintSize(context, offset);
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Flutter.Ui.Paint paint__16914 = default!;
                if (((child is not null) && !child!.size.isEmpty))
                {
                    global::Doroti.Flutter.Ui.Path path__16991 = default!;
                    paint__16914 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 1.0;
    __cascade.color = new global::Doroti.Flutter.Ui.Color(4294967040L);
    return __cascade;
}))();
                    path__16991 = new global::Doroti.Flutter.Ui.Path();
                    var childParentData__17174 = ((BoxParentData?)(object?)child!.parentData!)!;
                    if ((((BoxParentData)childParentData__17174).offset.dy > 0.0))
                    {
                        double headSize__17338 = Math.Min((((BoxParentData)childParentData__17174).offset.dy * 0.2), 10.0);
                        ((Func<Path>)(() =>
{
    var __cascade = path__16991;
    __cascade.moveTo((offset.dx + (size.width / 2.0)), offset.dy);
    __cascade.relativeLineTo(0.0, (((BoxParentData)childParentData__17174).offset.dy - headSize__17338));
    __cascade.relativeLineTo(headSize__17338, 0.0);
    __cascade.relativeLineTo(-headSize__17338, headSize__17338);
    __cascade.relativeLineTo(-headSize__17338, -headSize__17338);
    __cascade.relativeLineTo(headSize__17338, 0.0);
    __cascade.moveTo((offset.dx + (size.width / 2.0)), (offset.dy + size.height));
    __cascade.relativeLineTo(0.0, (-((BoxParentData)childParentData__17174).offset.dy + headSize__17338));
    __cascade.relativeLineTo(headSize__17338, 0.0);
    __cascade.relativeLineTo(-headSize__17338, -headSize__17338);
    __cascade.relativeLineTo(-headSize__17338, headSize__17338);
    __cascade.relativeLineTo(headSize__17338, 0.0);
    return __cascade;
}))();
                        ((PaintingContext)context).canvas.drawPath(path__16991, paint__16914);
                    }
                    if ((((BoxParentData)childParentData__17174).offset.dx > 0.0))
                    {
                        double headSize__18244 = Math.Min((((BoxParentData)childParentData__17174).offset.dx * 0.2), 10.0);
                        ((Func<Path>)(() =>
{
    var __cascade = path__16991;
    __cascade.moveTo(offset.dx, (offset.dy + (size.height / 2.0)));
    __cascade.relativeLineTo((((BoxParentData)childParentData__17174).offset.dx - headSize__18244), 0.0);
    __cascade.relativeLineTo(0.0, headSize__18244);
    __cascade.relativeLineTo(headSize__18244, -headSize__18244);
    __cascade.relativeLineTo(-headSize__18244, -headSize__18244);
    __cascade.relativeLineTo(0.0, headSize__18244);
    __cascade.moveTo((offset.dx + size.width), (offset.dy + (size.height / 2.0)));
    __cascade.relativeLineTo((-((BoxParentData)childParentData__17174).offset.dx + headSize__18244), 0.0);
    __cascade.relativeLineTo(0.0, headSize__18244);
    __cascade.relativeLineTo(-headSize__18244, -headSize__18244);
    __cascade.relativeLineTo(headSize__18244, -headSize__18244);
    __cascade.relativeLineTo(0.0, headSize__18244);
    return __cascade;
}))();
                        ((PaintingContext)context).canvas.drawPath(path__16991, paint__16914);
                    }
                }
                else
                {
                    paint__16914 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Paint();
    __cascade.color = new global::Doroti.Flutter.Ui.Color(2425393296L);
    return __cascade;
}))();
                    ((PaintingContext)context).canvas.drawRect((offset & size), paint__16914);
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
        RenderBox? child__19632 = this.child;
        if ((child__19632 is null))
        {
            return null;
        }
        BoxConstraints childConstraints__19727 = constraints.loosen();
        double? result__19786 = child__19632.getDryBaseline(childConstraints__19727, baseline);
        if ((result__19786 is null))
        {
            return null;
        }
        global::Doroti.Flutter.Ui.Size childSize__19911 = child__19632.getDryLayout(childConstraints__19727);
        bool shrinkWrapWidth__20067 = ((this._widthFactor is not null) || (((BoxConstraints)constraints).maxWidth == double.PositiveInfinity));
        bool shrinkWrapHeight__20165 = ((this._heightFactor is not null) || (((BoxConstraints)constraints).maxHeight == double.PositiveInfinity));
        global::Doroti.Flutter.Ui.Size size__20266 = constraints.constrain(new global::Doroti.Flutter.Ui.Size((shrinkWrapWidth__20067 ? (childSize__19911.width * ((this._widthFactor ?? 1.0))) : double.PositiveInfinity), (shrinkWrapHeight__20165 ? (childSize__19911.height * ((this._heightFactor ?? 1.0))) : double.PositiveInfinity)));
        global::Doroti.Flutter.Ui.Offset childOffset__20515 = resolvedAlignment.alongOffset((size__20266 - childSize__19911));
        return (DartRuntimePrimitives.RequireValue(result__19786) + childOffset__20515.dy);
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

    public RenderConstrainedOverflowBox(RenderBox? child = null, double? minWidth = null, double? maxWidth = null, double? minHeight = null, double? maxHeight = null, OverflowBoxFit fit = OverflowBoxFit.max, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null) : base(child: child, alignment: alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center, textDirection: DartRuntimePrimitives.RequireValue(textDirection))
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
        RenderBox? child__26299 = this.child;
        if ((child__26299 is null))
        {
            return null;
        }
        BoxConstraints childConstraints__26394 = _getInnerConstraints(constraints);
        double? result__26466 = child__26299.getDryBaseline(childConstraints__26394, baseline);
        if ((result__26466 is null))
        {
            return null;
        }
        global::Doroti.Flutter.Ui.Size childSize__26591 = child__26299.getDryLayout(childConstraints__26394);
        global::Doroti.Flutter.Ui.Size size__26656 = getDryLayout(constraints);
        return (DartRuntimePrimitives.RequireValue(result__26466) + resolvedAlignment.alongOffset((size__26656 - childSize__26591)).dy);
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
    public virtual List<global::Doroti.Generated.Framework.Painting.TextPainter> _indicatorLabel { get; set; } = new List<global::Doroti.Generated.Framework.Painting.TextPainter>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(System.Enum.GetValues<_OverflowSide__debug_overflow_indicator>().ToList().Count)))), ((i) => new global::Doroti.Generated.Framework.Painting.TextPainter(textDirection: TextDirection.ltr))));
    public virtual bool _overflowReportNeeded { get; set; } = true;

    public RenderConstraintsTransformBox(global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment, TextDirection? textDirection, Func<BoxConstraints, BoxConstraints> constraintsTransform, RenderBox? child = null, Clip clipBehavior = Clip.none) : base(alignment: alignment, textDirection: DartRuntimePrimitives.RequireValue(textDirection), child: child)
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
            bool needsLayout__31239 = ((this._childConstraints is null) || (!object.Equals(this._childConstraints, __value(constraints))));
            if (needsLayout__31239)
            {
                markNeedsLayout();
            }
        }
    }
    public virtual global::Doroti.Flutter.Ui.Clip clipBehavior
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
        global::Doroti.Flutter.Ui.Size? childSize__32641 = child?.getDryLayout(this.constraintsTransform(constraints));
        return ((childSize__32641 is null) ? ((BoxConstraints)constraints).smallest : constraints.constrain(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(childSize__32641))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? child__32927 = this.child;
        if ((child__32927 is null))
        {
            return null;
        }
        BoxConstraints childConstraints__33022 = this.constraintsTransform(constraints);
        double? result__33094 = child__32927.getDryBaseline(childConstraints__33022, baseline);
        if ((result__33094 is null))
        {
            return null;
        }
        global::Doroti.Flutter.Ui.Size childSize__33219 = child__32927.getDryLayout(childConstraints__33022);
        global::Doroti.Flutter.Ui.Size size__33284 = constraints.constrain(childSize__33219);
        return (DartRuntimePrimitives.RequireValue(result__33094) + resolvedAlignment.alongOffset((size__33284 - childSize__33219)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraints__33626 = this.constraints;
        RenderBox? child__33679 = this.child;
        if ((child__33679 is not null))
        {
            BoxConstraints childConstraints__33751 = this.constraintsTransform(constraints__33626);
            DartRuntimePrimitives.Assert(() => ((BoxConstraints)childConstraints__33751).isNormalized);
            _childConstraints = childConstraints__33751;
            child__33679.layout(childConstraints__33751, parentUsesSize: true);
            size = constraints__33626.constrain(((RenderBox)child__33679).size);
            alignChild();
            var childParentData__34073 = ((BoxParentData?)(object?)child__33679.parentData!)!;
            _overflowContainerRect = (Offset.zero & size);
            _overflowChildRect = (((BoxParentData)childParentData__34073).offset & ((RenderBox)child__33679).size);
        }
        else
        {
            size = ((BoxConstraints)constraints__33626).smallest;
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
        foreach (global::Doroti.Generated.Framework.Painting.TextPainter painter__3670 in this._indicatorLabel)
        {
            painter__3670.dispose();
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
        string header__35928 = base.toStringShort();
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            if (this._isOverflowing)
            {
                header__35928 += " OVERFLOWING";
            }
        }
        return header__35928;
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
        var regions__4254 = new List<_OverflowRegionData__debug_overflow_indicator>();
        if ((((RelativeRect)overflow).left > 0.0))
        {
            var markerRect__4332 = global::Doroti.Flutter.Ui.Rect.fromLTWH(0.0, 0.0, (containerRect.width * DebugOverflowIndicatorMixin._indicatorFraction), containerRect.height);
            regions__4254.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRect__4332, label: $"LEFT OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).left)} PIXELS", labelOffset: (markerRect__4332.centerLeft + new global::Doroti.Flutter.Ui.Offset((DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels), 0.0)), rotation: (Dart_mathLibrary.pi / 2.0), side: _OverflowSide__debug_overflow_indicator.left));
        }
        if ((((RelativeRect)overflow).right > 0.0))
        {
            var markerRect__4921 = global::Doroti.Flutter.Ui.Rect.fromLTWH((containerRect.width * ((1.0 - DebugOverflowIndicatorMixin._indicatorFraction))), 0.0, (containerRect.width * DebugOverflowIndicatorMixin._indicatorFraction), containerRect.height);
            regions__4254.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRect__4921, label: $"RIGHT OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).right)} PIXELS", labelOffset: (markerRect__4921.centerRight - new global::Doroti.Flutter.Ui.Offset((DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels), 0.0)), rotation: (-Dart_mathLibrary.pi / 2.0), side: _OverflowSide__debug_overflow_indicator.right));
        }
        if ((((RelativeRect)overflow).top > 0.0))
        {
            var markerRect__5558 = global::Doroti.Flutter.Ui.Rect.fromLTWH(0.0, 0.0, containerRect.width, (containerRect.height * DebugOverflowIndicatorMixin._indicatorFraction));
            regions__4254.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRect__5558, label: $"TOP OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).top)} PIXELS", labelOffset: (markerRect__5558.topCenter + new global::Doroti.Flutter.Ui.Offset(0.0, DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels)), side: _OverflowSide__debug_overflow_indicator.top));
        }
        if ((((RelativeRect)overflow).bottom > 0.0))
        {
            var markerRect__6054 = global::Doroti.Flutter.Ui.Rect.fromLTWH(0.0, (containerRect.height * ((1.0 - DebugOverflowIndicatorMixin._indicatorFraction))), containerRect.width, (containerRect.height * DebugOverflowIndicatorMixin._indicatorFraction));
            regions__4254.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRect__6054, label: $"BOTTOM OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).bottom)} PIXELS", labelOffset: (markerRect__6054.bottomCenter - new global::Doroti.Flutter.Ui.Offset(0.0, (DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels))), side: _OverflowSide__debug_overflow_indicator.bottom));
        }
        return regions__4254;
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
        var overflows__7571 = new List<string>();
        var overflowText__7954 = "";
        DartRuntimePrimitives.Assert(() => (checked((long)(overflows__7571.Count)) != 0));
        switch (checked((long)(overflows__7571.Count)))
        {
            case 1L:
                {
                    overflowText__7954 = overflows__7571.First();
                    break;
                }
            case 2L:
                {
                    overflowText__7954 = $"{overflows__7571.First()} and {overflows__7571.Last()}";
                    break;
                }
            default:
                {
                    overflows__7571[(int)((checked((long)(overflows__7571.Count)) - 1L))] = $"and {overflows__7571[(int)((checked((long)(overflows__7571.Count)) - 1L))]}";
                    overflowText__7954 = string.Join(", ", overflows__7571);
                    break;
                }
        }
        FlutterError.reportError(new FlutterErrorDetails(exception: new FlutterError($"A {this.GetType()} overflowed by {overflowText__7954}."), library: "rendering library", context: new ErrorDescription("during layout"), informationCollector: (() => new List<DiagnosticsNode> { describeForError($"The specific {this.GetType()} in question is"), new DiagnosticsNode(DartCoreExtensions.repeat("◢◤", ((checked((long)(FlutterError.wrapWidth / 2L))))), allowWrap: false) })));
    }

    public virtual void paintOverflowIndicator(PaintingContext context, Offset offset, Rect containerRect, Rect childRect, List<DiagnosticsNode>? overflowHints = null)
    {
        var overflow__9849 = RelativeRect.CreateFromRect(containerRect, childRect);
        if (((((((RelativeRect)overflow__9849).left <= 0.0) && (((RelativeRect)overflow__9849).right <= 0.0)) && (((RelativeRect)overflow__9849).top <= 0.0)) && (((RelativeRect)overflow__9849).bottom <= 0.0)))
        {
            return;
        }
        List<_OverflowRegionData__debug_overflow_indicator> overflowRegions__10097 = _calculateOverflowRegions(overflow__9849, containerRect);
        foreach (var region__10201 in overflowRegions__10097)
        {
            ((PaintingContext)context).canvas.drawRect(((_OverflowRegionData__debug_overflow_indicator)region__10201).rect.shift(offset), DebugOverflowIndicatorMixin._indicatorPaint);
            var textSpan__10317 = ((global::Doroti.Generated.Framework.Painting.TextSpan?)(object?)this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region__10201).side))].text)!;
            if ((textSpan__10317?.text != ((_OverflowRegionData__debug_overflow_indicator)region__10201).label))
            {
                this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region__10201).side))].text = new global::Doroti.Generated.Framework.Painting.TextSpan(text: ((_OverflowRegionData__debug_overflow_indicator)region__10201).label, style: DebugOverflowIndicatorMixin._indicatorTextStyle);
                this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region__10201).side))].layout();
            }
            global::Doroti.Flutter.Ui.Offset labelOffset__10646 = (((_OverflowRegionData__debug_overflow_indicator)region__10201).labelOffset + offset);
            var centerOffset__10701 = new global::Doroti.Flutter.Ui.Offset((-this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region__10201).side))].width / 2.0), 0.0);
            global::Doroti.Flutter.Ui.Rect textBackgroundRect__10795 = (centerOffset__10701 & this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region__10201).side))].size);
            ((PaintingContext)context).canvas.save();
            ((PaintingContext)context).canvas.translate(labelOffset__10646.dx, labelOffset__10646.dy);
            ((PaintingContext)context).canvas.rotate(((_OverflowRegionData__debug_overflow_indicator)region__10201).rotation);
            ((PaintingContext)context).canvas.drawRect(textBackgroundRect__10795, DebugOverflowIndicatorMixin._labelBackgroundPaint);
            this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region__10201).side))].paint(((PaintingContext)context).canvas, centerOffset__10701);
            ((PaintingContext)context).canvas.restore();
        }
        if (this._overflowReportNeeded)
        {
            this._overflowReportNeeded = false;
            _reportOverflow(overflow__9849, overflowHints);
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

    public RenderSizedOverflowBox(RenderBox? child = null, Size requestedSize = default!, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null) : base(child: child, alignment: alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center, textDirection: DartRuntimePrimitives.RequireValue(textDirection))
    {
        this._requestedSize = requestedSize;
    }

    public virtual global::Doroti.Flutter.Ui.Size requestedSize
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
        RenderBox? child__37982 = this.child;
        if ((child__37982 is null))
        {
            return base.computeDistanceToActualBaseline(baseline);
        }
        double? result__38113 = child__37982.getDistanceToActualBaseline(baseline);
        if ((result__38113 is null))
        {
            return base.computeDistanceToActualBaseline(baseline);
        }
        var childParentData__38271 = ((BoxParentData?)(object?)child__37982.parentData!)!;
        return (DartRuntimePrimitives.RequireValue(result__38113) + ((BoxParentData)childParentData__38271).offset.dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? child__38502 = this.child;
        if ((child__38502 is null))
        {
            return null;
        }
        double? result__38590 = child__38502.getDryBaseline(constraints, baseline);
        if ((result__38590 is null))
        {
            return null;
        }
        global::Doroti.Flutter.Ui.Size childSize__38710 = child__38502.getDryLayout(constraints);
        global::Doroti.Flutter.Ui.Size size__38770 = getDryLayout(constraints);
        return (DartRuntimePrimitives.RequireValue(result__38590) + resolvedAlignment.alongOffset((size__38770 - childSize__38710)).dy);
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

    public RenderFractionallySizedOverflowBox(RenderBox? child = null, double? widthFactor = null, double? heightFactor = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null) : base(child: child, alignment: alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center, textDirection: DartRuntimePrimitives.RequireValue(textDirection))
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
        double minWidth__41692 = ((BoxConstraints)constraints).minWidth;
        double maxWidth__41736 = ((BoxConstraints)constraints).maxWidth;
        if ((this._widthFactor is not null))
        {
            double width__41820 = (maxWidth__41736 * DartRuntimePrimitives.RequireValue(this._widthFactor));
            minWidth__41692 = width__41820;
            maxWidth__41736 = width__41820;
        }
        double minHeight__41919 = ((BoxConstraints)constraints).minHeight;
        double maxHeight__41965 = ((BoxConstraints)constraints).maxHeight;
        if ((this._heightFactor is not null))
        {
            double height__42052 = (maxHeight__41965 * DartRuntimePrimitives.RequireValue(this._heightFactor));
            minHeight__41919 = height__42052;
            maxHeight__41965 = height__42052;
        }
        return new BoxConstraints(minWidth: minWidth__41692, maxWidth: maxWidth__41736, minHeight: minHeight__41919, maxHeight: maxHeight__41965);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        double result__42374 = default!;
        if ((child is null))
        {
            result__42374 = base.computeMinIntrinsicWidth(height);
        }
        else
        {
            result__42374 = child!.getMinIntrinsicWidth((height * ((this._heightFactor ?? 1.0))));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(result__42374));
        return (result__42374 / ((this._widthFactor ?? 1.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double result__42780 = default!;
        if ((child is null))
        {
            result__42780 = base.computeMaxIntrinsicWidth(height);
        }
        else
        {
            result__42780 = child!.getMaxIntrinsicWidth((height * ((this._heightFactor ?? 1.0))));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(result__42780));
        return (result__42780 / ((this._widthFactor ?? 1.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double result__43186 = default!;
        if ((child is null))
        {
            result__43186 = base.computeMinIntrinsicHeight(width);
        }
        else
        {
            result__43186 = child!.getMinIntrinsicHeight((width * ((this._widthFactor ?? 1.0))));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(result__43186));
        return (result__43186 / ((this._heightFactor ?? 1.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        double result__43592 = default!;
        if ((child is null))
        {
            result__43592 = base.computeMaxIntrinsicHeight(width);
        }
        else
        {
            result__43592 = child!.getMaxIntrinsicHeight((width * ((this._widthFactor ?? 1.0))));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(result__43592));
        return (result__43592 / ((this._heightFactor ?? 1.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if ((child is not null))
        {
            global::Doroti.Flutter.Ui.Size childSize__44049 = child!.getDryLayout(_getInnerConstraints(constraints));
            return constraints.constrain(childSize__44049);
        }
        return constraints.constrain(_getInnerConstraints(constraints).constrain(Size.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? child__44390 = this.child;
        if ((child__44390 is null))
        {
            return null;
        }
        BoxConstraints childConstraints__44485 = _getInnerConstraints(constraints);
        double? result__44557 = child__44390.getDryBaseline(childConstraints__44485, baseline);
        if ((result__44557 is null))
        {
            return null;
        }
        global::Doroti.Flutter.Ui.Size childSize__44682 = child__44390.getDryLayout(childConstraints__44485);
        global::Doroti.Flutter.Ui.Size size__44747 = getDryLayout(constraints);
        return (DartRuntimePrimitives.RequireValue(result__44557) + resolvedAlignment.alongOffset((size__44747 - childSize__44682)).dy);
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

    public virtual global::Doroti.Flutter.Ui.Size getSize(BoxConstraints constraints) => ((BoxConstraints)constraints).biggest;
    public virtual BoxConstraints getConstraintsForChild(BoxConstraints constraints) => constraints;
    public virtual global::Doroti.Flutter.Ui.Offset getPositionForChild(Size size, Size childSize) => Offset.zero;
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
            SingleChildLayoutDelegate oldDelegate__50258 = this._delegate;
            if (((!object.Equals(DartRuntimePrimitives.RuntimeType(newDelegate), DartRuntimePrimitives.RuntimeType(oldDelegate__50258))) || newDelegate.shouldRelayout(oldDelegate__50258)))
            {
                markNeedsLayout();
            }
            _delegate = newDelegate;
            if (attached)
            {
                ((SingleChildLayoutDelegate)oldDelegate__50258)._relayout?.removeListener(markNeedsLayout);
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

    internal virtual global::Doroti.Flutter.Ui.Size _getSize(BoxConstraints constraints)
    {
        return constraints.constrain(this._delegate.getSize(constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        double width__51325 = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(width__51325))
        {
            return width__51325;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double width__51549 = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(width__51549))
        {
            return width__51549;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double height__51773 = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(height__51773))
        {
            return height__51773;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        double height__51999 = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(height__51999))
        {
            return height__51999;
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
        RenderBox? child__52398 = this.child;
        if ((child__52398 is null))
        {
            return null;
        }
        BoxConstraints childConstraints__52493 = this.@delegate.getConstraintsForChild(constraints);
        double? result__52576 = child__52398.getDryBaseline(childConstraints__52493, baseline);
        if ((result__52576 is null))
        {
            return null;
        }
        return (DartRuntimePrimitives.RequireValue(result__52576) + this.@delegate.getPositionForChild(_getSize(constraints), (((BoxConstraints)childConstraints__52493).isTight ? ((BoxConstraints)childConstraints__52493).smallest : child__52398.getDryLayout(childConstraints__52493))).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _getSize(constraints);
        if ((child is not null))
        {
            BoxConstraints childConstraints__53096 = this.@delegate.getConstraintsForChild(constraints);
            DartRuntimePrimitives.Assert(() => childConstraints__53096.debugAssertIsValid(isAppliedConstraint: true));
            child!.layout(childConstraints__53096, parentUsesSize: !((BoxConstraints)childConstraints__53096).isTight);
            var childParentData__53333 = ((BoxParentData?)(object?)child!.parentData!)!;
            childParentData__53333.offset = this.@delegate.getPositionForChild(size, (((BoxConstraints)childConstraints__53096).isTight ? ((BoxConstraints)childConstraints__53096).smallest : child!.size));
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
    public virtual global::Doroti.Flutter.Ui.TextBaseline baselineType
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
    internal virtual (global::Doroti.Flutter.Ui.Size size, double top) _computeSizes(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild, Func<RenderBox, BoxConstraints, TextBaseline, double?> getBaseline)
    {
        RenderBox? child__55479 = this.child;
        if ((child__55479 is null))
        {
            return (size: ((BoxConstraints)constraints).smallest, top: 0);
        }
        BoxConstraints childConstraints__55606 = constraints.loosen();
        global::Doroti.Flutter.Ui.Size childSize__55662 = layoutChild(child__55479, childConstraints__55606);
        double childBaseline__55729 = (getBaseline(child__55479, childConstraints__55606, this.baselineType) ?? childSize__55662.height);
        double top__55842 = (this.baseline - childBaseline__55729);
        return (size: constraints.constrain(new global::Doroti.Flutter.Ui.Size(childSize__55662.width, (top__55842 + childSize__55662.height))), top: top__55842);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSizes(constraints, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline).size;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? child__56334 = this.child;
        double? result1__56372 = child__56334?.getDryBaseline(constraints.loosen(), baseline);
        double? result2__56455 = child__56334?.getDryBaseline(constraints.loosen(), this.baselineType);
        if (((result1__56372 is null) || (result2__56455 is null)))
        {
            return null;
        }
        return ((this.baseline + DartRuntimePrimitives.RequireValue(result1__56372)) - DartRuntimePrimitives.RequireValue(result2__56455));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        var (size__56700, top__56714) = _computeSizes(constraints, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getBaseline);
        this.size = size__56700;
        (((BoxParentData?)(object?)child?.parentData)!)?.offset = new global::Doroti.Flutter.Ui.Offset(0.0, top__56714);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("baseline", this.baseline));
        properties.add(new EnumProperty<global::Doroti.Flutter.Ui.TextBaseline>("baselineType", this.baselineType));
    }

}

