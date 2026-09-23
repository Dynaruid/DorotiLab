// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/shifted_box.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public delegate BoxConstraints BoxConstraintsTransform(BoxConstraints constraints);

public abstract class RenderShiftedBox : RenderBox, RenderObjectWithChildMixin<RenderBox>
{
    public virtual RenderBox? _child { get; set; } = default;

    protected RenderShiftedBox(RenderBox? child) { }

    public override double computeMinIntrinsicWidth(double height)
    {
        return child?.getMinIntrinsicWidth(height) ?? 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return child?.getMaxIntrinsicWidth(height) ?? 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return child?.getMinIntrinsicHeight(width) ?? 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return child?.getMaxIntrinsicHeight(width) ?? 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        double? result = default!;
        RenderBox? childLocal = child;
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        if (childLocal is not null)
        {
            DartRuntimePrimitives.Assert(() => !childLocal.debugNeedsLayout);
            result = childLocal.getDistanceToActualBaseline(baseline);
            var childParentData = ((BoxParentData?)(object?)childLocal.parentData!)!;
            if (result is not null)
            {
                double result__1798__value2078 = (
                    result
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                result__1798__value2078 += childParentData.offset.dy;
            }
        }
        else
        {
            result = base.computeDistanceToActualBaseline(baseline);
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        double? childBaseline = childLocal.getDryBaseline(constraints, baseline);
        if (childBaseline is null)
        {
            return null;
        }
        return (
            childBaseline
            ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderBox? childLocal = child;
        if (childLocal is not null)
        {
            var childParentData = ((BoxParentData?)(object?)childLocal.parentData!)!;
            context.paintChild(childLocal, childParentData.offset + offset);
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? childLocal = child;
        if (childLocal is not null)
        {
            var childParentData = ((BoxParentData?)(object?)childLocal.parentData!)!;
            return result.addWithPaintOffset(
                offset: childParentData.offset,
                position: position,
                hitTest: (result, transformed) =>
                {
                    DartRuntimePrimitives.Assert(() =>
                        Equals(transformed, position - childParentData.offset)
                    );
                    return childLocal.hitTest(result, position: transformed);
                }
            );
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (child is not RenderBox)
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"A {GetType()} expected a child of type {typeof(RenderBox)} but received a "
                                + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."
                        ),
                        new ErrorDescription(
                            "RenderObjects expect specific types of children because they "
                                + "coordinate with their children during layout and paint. For "
                                + "example, a RenderSliver cannot be the child of a RenderBox because "
                                + "a RenderSliver does not understand the RenderBox layout protocol."
                        ),
                        new ErrorSpacer(),
                        new DiagnosticsProperty<object?>(
                            $"The {GetType()} that expected a {typeof(RenderBox)} child was created by",
                            debugCreator,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                        new ErrorSpacer(),
                        new DiagnosticsProperty<object?>(
                            $"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type "
                                + "was created by",
                            child.debugCreator,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                    }
                );
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual RenderBox? child
    {
        get => _child;
        set
        {
            var __value = value;
            if (_child is not null)
            {
                dropChild(_child!);
            }
            _child = __value;
            if (_child is not null)
            {
                adoptChild(_child!);
            }
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _child?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        _child?.detach();
    }

    public override void redepthChildren()
    {
        if (_child is not null)
        {
            redepthChild(_child!);
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        if (_child is not null)
        {
            visitor(_child!);
        }
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return (child is not null)
            ? new List<DiagnosticsNode>
            {
                ((Diagnosticable)child!).toDiagnosticsNode(name: "child"),
            }
            : new List<DiagnosticsNode>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class RenderPadding : RenderShiftedBox
{
    internal virtual EdgeInsets? _resolvedPaddingCache { get; set; } = default;
    internal virtual EdgeInsetsGeometry _padding { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;

    public RenderPadding(
        EdgeInsetsGeometry padding,
        TextDirection? textDirection = null,
        RenderBox? child = null
    )
        : base(child)
    {
        _textDirection = textDirection;
        _padding = padding;
        System.Diagnostics.Debug.Assert(padding.isNonNegative);
    }

    internal virtual EdgeInsets _resolvedPadding
    {
        get
        {
            EdgeInsets returnValue = _resolvedPaddingCache ??= padding.resolve(textDirection);
            DartRuntimePrimitives.Assert(() => returnValue.isNonNegative);
            return returnValue;
        }
    }

    internal virtual void _markNeedResolution()
    {
        _resolvedPaddingCache = null;
        markNeedsLayout();
    }

    public virtual EdgeInsetsGeometry padding
    {
        get => _padding;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value.isNonNegative);
            if (Equals(_padding, __value))
            {
                return;
            }
            _padding = __value;
            _markNeedResolution();
        }
    }
    public virtual TextDirection? textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, __value))
            {
                return;
            }
            _textDirection = __value;
            _markNeedResolution();
        }
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        EdgeInsets padding = _resolvedPadding;
        if (child is not null)
        {
            return child!.getMinIntrinsicWidth(Math.Max(0.0, height - padding.vertical))
                + padding.horizontal;
        }
        return padding.horizontal;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        EdgeInsets padding = _resolvedPadding;
        if (child is not null)
        {
            return child!.getMaxIntrinsicWidth(Math.Max(0.0, height - padding.vertical))
                + padding.horizontal;
        }
        return padding.horizontal;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        EdgeInsets padding = _resolvedPadding;
        if (child is not null)
        {
            return child!.getMinIntrinsicHeight(Math.Max(0.0, width - padding.horizontal))
                + padding.vertical;
        }
        return padding.vertical;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        EdgeInsets padding = _resolvedPadding;
        if (child is not null)
        {
            return child!.getMaxIntrinsicHeight(Math.Max(0.0, width - padding.horizontal))
                + padding.vertical;
        }
        return padding.vertical;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        EdgeInsets padding = _resolvedPadding;
        if (child is null)
        {
            return constraints.constrain(new Size(padding.horizontal, padding.vertical));
        }
        BoxConstraints innerConstraints = constraints.deflate(padding);
        Size childSize = child!.getDryLayout(innerConstraints);
        return constraints.constrain(
            new Size(padding.horizontal + childSize.width, padding.vertical + childSize.height)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        EdgeInsets padding = _resolvedPadding;
        BoxConstraints innerConstraints = constraints.deflate(padding);
        double? childBaseline = childLocal.getDryBaseline(innerConstraints, baseline);
        if (childBaseline is null)
        {
            return null;
        }
        return (
                childBaseline
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) + padding.top;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        EdgeInsets padding = _resolvedPadding;
        if (child is null)
        {
            size = constraintsLocal.constrain(new Size(padding.horizontal, padding.vertical));
            return;
        }
        BoxConstraints innerConstraints = constraintsLocal.deflate(padding);
        child!.layout(innerConstraints, parentUsesSize: true);
        var childParentData = ((BoxParentData?)(object?)child!.parentData!)!;
        childParentData.offset = new Offset(padding.left, padding.top);
        size = constraintsLocal.constrain(
            new Size(padding.horizontal + child!.size.width, padding.vertical + child!.size.height)
        );
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        base.debugPaintSize(context, offset);
        DartRuntimePrimitives.Assert(() =>
        {
            Rect outerRect = offset & size;
            DebugLibrary.debugPaintPadding(
                context.canvas,
                outerRect,
                (child is not null) ? _resolvedPaddingCache!.deflateRect(outerRect) : null
            );
            return true;
        });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding));
        properties.add(
            new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null)
        );
    }
}

public abstract class RenderAligningShiftedBox : RenderShiftedBox
{
    internal virtual Alignment? _resolvedAlignment { get; set; } = default;
    internal virtual AlignmentGeometry _alignment { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;

    protected RenderAligningShiftedBox(
        AlignmentGeometry alignment = default!,
        TextDirection? textDirection = default!,
        RenderBox? child = null
    )
        : base(child)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        _alignment = __alignment;
        _textDirection = textDirection;
    }

    public virtual Alignment resolvedAlignment =>
        _resolvedAlignment ??= alignment.resolve(textDirection);

    internal virtual void _markNeedResolution()
    {
        _resolvedAlignment = null;
        markNeedsLayout();
    }

    public virtual AlignmentGeometry alignment
    {
        get => _alignment;
        set
        {
            var __value = value;
            if (Equals(_alignment, __value))
            {
                return;
            }
            _alignment = __value;
            _markNeedResolution();
        }
    }
    public virtual TextDirection? textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, __value))
            {
                return;
            }
            _textDirection = __value;
            _markNeedResolution();
        }
    }

    public virtual void alignChild()
    {
        DartRuntimePrimitives.Assert(() => child is not null);
        DartRuntimePrimitives.Assert(() => !child!.debugNeedsLayout);
        DartRuntimePrimitives.Assert(() => child!.hasSize);
        DartRuntimePrimitives.Assert(() => hasSize);
        var childParentData = ((BoxParentData?)(object?)child!.parentData!)!;
        childParentData.offset = resolvedAlignment.alongOffset(size - child!.size);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
        properties.add(
            new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null)
        );
    }
}

public class RenderPositionedBox : RenderAligningShiftedBox
{
    internal virtual double? _widthFactor { get; set; } = default;
    internal virtual double? _heightFactor { get; set; } = default;

    public RenderPositionedBox(
        RenderBox? child = null,
        double? widthFactor = null,
        double? heightFactor = null,
        AlignmentGeometry alignment = default!,
        TextDirection? textDirection = null
    )
        : base(
            child: child,
            alignment: alignment ?? Alignment.center,
            textDirection: (
                textDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        )
    {
        _widthFactor = widthFactor;
        _heightFactor = heightFactor;
        System.Diagnostics.Debug.Assert((widthFactor is null) || (widthFactor >= 0.0));
        System.Diagnostics.Debug.Assert((heightFactor is null) || (heightFactor >= 0.0));
    }

    public virtual double? widthFactor
    {
        get => _widthFactor;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is null) || (__value >= 0.0));
            if (_widthFactor == __value)
            {
                return;
            }
            _widthFactor = __value;
            markNeedsLayout();
        }
    }
    public virtual double? heightFactor
    {
        get => _heightFactor;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is null) || (__value >= 0.0));
            if (_heightFactor == __value)
            {
                return;
            }
            _heightFactor = __value;
            markNeedsLayout();
        }
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return base.computeMinIntrinsicWidth(height) * (_widthFactor ?? 1L);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return base.computeMaxIntrinsicWidth(height) * (_widthFactor ?? 1L);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return base.computeMinIntrinsicHeight(width) * (_heightFactor ?? 1L);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return base.computeMaxIntrinsicHeight(width) * (_heightFactor ?? 1L);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        bool shrinkWrapWidth =
            (_widthFactor is not null) || (constraints.maxWidth == double.PositiveInfinity);
        bool shrinkWrapHeight =
            (_heightFactor is not null) || (constraints.maxHeight == double.PositiveInfinity);
        if (child is not null)
        {
            Size childSize = child!.getDryLayout(constraints.loosen());
            return constraints.constrain(
                new Size(
                    shrinkWrapWidth
                        ? (childSize.width * (_widthFactor ?? 1.0))
                        : double.PositiveInfinity,
                    shrinkWrapHeight
                        ? (childSize.height * (_heightFactor ?? 1.0))
                        : double.PositiveInfinity
                )
            );
        }
        return constraints.constrain(
            new Size(
                shrinkWrapWidth ? 0.0 : double.PositiveInfinity,
                shrinkWrapHeight ? 0.0 : double.PositiveInfinity
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        bool shrinkWrapWidth =
            (_widthFactor is not null) || (constraintsLocal.maxWidth == double.PositiveInfinity);
        bool shrinkWrapHeight =
            (_heightFactor is not null) || (constraintsLocal.maxHeight == double.PositiveInfinity);
        if (child is not null)
        {
            child!.layout(constraintsLocal.loosen(), parentUsesSize: true);
            size = constraintsLocal.constrain(
                new Size(
                    shrinkWrapWidth
                        ? (child!.size.width * (_widthFactor ?? 1.0))
                        : double.PositiveInfinity,
                    shrinkWrapHeight
                        ? (child!.size.height * (_heightFactor ?? 1.0))
                        : double.PositiveInfinity
                )
            );
            alignChild();
        }
        else
        {
            size = constraintsLocal.constrain(
                new Size(
                    shrinkWrapWidth ? 0.0 : double.PositiveInfinity,
                    shrinkWrapHeight ? 0.0 : double.PositiveInfinity
                )
            );
        }
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        base.debugPaintSize(context, offset);
        DartRuntimePrimitives.Assert(() =>
        {
            Paint paint = default!;
            if ((child is not null) && !child!.size.isEmpty)
            {
                Path path = default!;
                paint = (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.style = PaintingStyle.stroke;
                            __cascade.strokeWidth = 1.0;
                            __cascade.color = new Color(4294967040L);
                            return __cascade;
                        }
                    )
                )();
                path = new Path();
                var childParentData = ((BoxParentData?)(object?)child!.parentData!)!;
                if (childParentData.offset.dy > 0.0)
                {
                    double headSize = Math.Min(childParentData.offset.dy * 0.2, 10.0);
                    (
                        (Func<Path>)(
                            () =>
                            {
                                var __cascade = path;
                                __cascade.moveTo(offset.dx + (size.width / 2.0), offset.dy);
                                __cascade.relativeLineTo(0.0, childParentData.offset.dy - headSize);
                                __cascade.relativeLineTo(headSize, 0.0);
                                __cascade.relativeLineTo(-headSize, headSize);
                                __cascade.relativeLineTo(-headSize, -headSize);
                                __cascade.relativeLineTo(headSize, 0.0);
                                __cascade.moveTo(
                                    offset.dx + (size.width / 2.0),
                                    offset.dy + size.height
                                );
                                __cascade.relativeLineTo(
                                    0.0,
                                    -childParentData.offset.dy + headSize
                                );
                                __cascade.relativeLineTo(headSize, 0.0);
                                __cascade.relativeLineTo(-headSize, -headSize);
                                __cascade.relativeLineTo(-headSize, headSize);
                                __cascade.relativeLineTo(headSize, 0.0);
                                return __cascade;
                            }
                        )
                    )();
                    context.canvas.drawPath(path, paint);
                }
                if (childParentData.offset.dx > 0.0)
                {
                    double headSizeLocal = Math.Min(childParentData.offset.dx * 0.2, 10.0);
                    (
                        (Func<Path>)(
                            () =>
                            {
                                var __cascade = path;
                                __cascade.moveTo(offset.dx, offset.dy + (size.height / 2.0));
                                __cascade.relativeLineTo(
                                    childParentData.offset.dx - headSizeLocal,
                                    0.0
                                );
                                __cascade.relativeLineTo(0.0, headSizeLocal);
                                __cascade.relativeLineTo(headSizeLocal, -headSizeLocal);
                                __cascade.relativeLineTo(-headSizeLocal, -headSizeLocal);
                                __cascade.relativeLineTo(0.0, headSizeLocal);
                                __cascade.moveTo(
                                    offset.dx + size.width,
                                    offset.dy + (size.height / 2.0)
                                );
                                __cascade.relativeLineTo(
                                    -childParentData.offset.dx + headSizeLocal,
                                    0.0
                                );
                                __cascade.relativeLineTo(0.0, headSizeLocal);
                                __cascade.relativeLineTo(-headSizeLocal, -headSizeLocal);
                                __cascade.relativeLineTo(headSizeLocal, -headSizeLocal);
                                __cascade.relativeLineTo(0.0, headSizeLocal);
                                return __cascade;
                            }
                        )
                    )();
                    context.canvas.drawPath(path, paint);
                }
            }
            else
            {
                paint = (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.color = new Color(2425393296L);
                            return __cascade;
                        }
                    )
                )();
                context.canvas.drawRect(offset & size, paint);
            }
            return true;
        });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("widthFactor", _widthFactor, ifNull: "expand"));
        properties.add(new DoubleProperty("heightFactor", _heightFactor, ifNull: "expand"));
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        BoxConstraints childConstraints = constraints.loosen();
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if (result is null)
        {
            return null;
        }
        Size childSize = childLocal.getDryLayout(childConstraints);
        bool shrinkWrapWidth =
            (_widthFactor is not null) || (constraints.maxWidth == double.PositiveInfinity);
        bool shrinkWrapHeight =
            (_heightFactor is not null) || (constraints.maxHeight == double.PositiveInfinity);
        Size size = constraints.constrain(
            new Size(
                shrinkWrapWidth
                    ? (childSize.width * (_widthFactor ?? 1.0))
                    : double.PositiveInfinity,
                shrinkWrapHeight
                    ? (childSize.height * (_heightFactor ?? 1.0))
                    : double.PositiveInfinity
            )
        );
        Offset childOffset = resolvedAlignment.alongOffset(size - childSize);
        return (
                result
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) + childOffset.dy;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public enum OverflowBoxFit
{
    max,
    deferToChild,
}

public class RenderConstrainedOverflowBox : RenderAligningShiftedBox
{
    internal virtual double? _minWidth { get; set; } = default;
    internal virtual double? _maxWidth { get; set; } = default;
    internal virtual double? _minHeight { get; set; } = default;
    internal virtual double? _maxHeight { get; set; } = default;
    internal virtual OverflowBoxFit _fit { get; set; } = default!;

    public RenderConstrainedOverflowBox(
        RenderBox? child = null,
        double? minWidth = null,
        double? maxWidth = null,
        double? minHeight = null,
        double? maxHeight = null,
        OverflowBoxFit fit = OverflowBoxFit.max,
        AlignmentGeometry alignment = default!,
        TextDirection? textDirection = null
    )
        : base(
            child: child,
            alignment: alignment ?? Alignment.center,
            textDirection: (
                textDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        )
    {
        _minWidth = minWidth;
        _maxWidth = maxWidth;
        _minHeight = minHeight;
        _maxHeight = maxHeight;
        _fit = fit;
    }

    public virtual double? minWidth
    {
        get => _minWidth;
        set
        {
            var __value = value;
            if (_minWidth == __value)
            {
                return;
            }
            _minWidth = __value;
            markNeedsLayout();
        }
    }
    public virtual double? maxWidth
    {
        get => _maxWidth;
        set
        {
            var __value = value;
            if (_maxWidth == __value)
            {
                return;
            }
            _maxWidth = __value;
            markNeedsLayout();
        }
    }
    public virtual double? minHeight
    {
        get => _minHeight;
        set
        {
            var __value = value;
            if (_minHeight == __value)
            {
                return;
            }
            _minHeight = __value;
            markNeedsLayout();
        }
    }
    public virtual double? maxHeight
    {
        get => _maxHeight;
        set
        {
            var __value = value;
            if (_maxHeight == __value)
            {
                return;
            }
            _maxHeight = __value;
            markNeedsLayout();
        }
    }
    public virtual OverflowBoxFit fit
    {
        get => _fit;
        set
        {
            var __value = value;
            if (Equals(_fit, (__value)))
            {
                return;
            }
            _fit = (__value);
            markNeedsLayoutForSizedByParentChange();
        }
    }

    internal virtual BoxConstraints _getInnerConstraints(BoxConstraints constraints)
    {
        return new BoxConstraints(
            minWidth: _minWidth ?? constraints.minWidth,
            maxWidth: _maxWidth ?? constraints.maxWidth,
            minHeight: _minHeight ?? constraints.minHeight,
            maxHeight: _maxHeight ?? constraints.maxHeight
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool sizedByParent =>
        fit switch
        {
            OverflowBoxFit.max => true,
            OverflowBoxFit.deferToChild => false,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return fit switch
        {
            OverflowBoxFit.max => constraints.biggest,
            OverflowBoxFit.deferToChild => child?.getDryLayout(constraints) ?? constraints.smallest,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        BoxConstraints childConstraints = _getInnerConstraints(constraints);
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if (result is null)
        {
            return null;
        }
        Size childSize = childLocal.getDryLayout(childConstraints);
        Size size = getDryLayout(constraints);
        return (
                result
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) + resolvedAlignment.alongOffset(size - childSize).dy;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        if (child is not null)
        {
            child!.layout(_getInnerConstraints(constraints), parentUsesSize: true);
            switch (fit)
            {
                case OverflowBoxFit.max:
                {
                    DartRuntimePrimitives.Assert(() => sizedByParent);
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
            switch (fit)
            {
                case OverflowBoxFit.max:
                {
                    DartRuntimePrimitives.Assert(() => sizedByParent);
                    break;
                }
                case OverflowBoxFit.deferToChild:
                {
                    size = constraints.smallest;
                    break;
                }
            }
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DoubleProperty("minWidth", minWidth, ifNull: "use parent minWidth constraint")
        );
        properties.add(
            new DoubleProperty("maxWidth", maxWidth, ifNull: "use parent maxWidth constraint")
        );
        properties.add(
            new DoubleProperty("minHeight", minHeight, ifNull: "use parent minHeight constraint")
        );
        properties.add(
            new DoubleProperty("maxHeight", maxHeight, ifNull: "use parent maxHeight constraint")
        );
        properties.add(new EnumProperty<OverflowBoxFit>("fit", fit));
    }
}

public class RenderConstraintsTransformBox : RenderAligningShiftedBox, DebugOverflowIndicatorMixin
{
    internal virtual Func<BoxConstraints, BoxConstraints> _constraintsTransform { get; set; } =
        default!;
    internal virtual Clip _clipBehavior { get; set; } = default!;
    internal virtual Rect _overflowContainerRect { get; set; } = Rect.zero;
    internal virtual Rect _overflowChildRect { get; set; } = Rect.zero;
    internal virtual bool _isOverflowing { get; set; } = false;
    internal virtual BoxConstraints? _childConstraints { get; set; } = default;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } =
        new LayerHandle<ClipRectLayer>();
    public virtual List<TextPainter> _indicatorLabel { get; set; } =
        new List<TextPainter>(
            Enumerable.Select(
                Enumerable.Range(
                    0,
                    checked(
                        (int)
                            checked(
                                (long)
                                    Enum.GetValues<_OverflowSide__debug_overflow_indicator>()
                                        .ToList()
                                        .Count
                            )
                    )
                ),
                (i) => new TextPainter(textDirection: TextDirection.ltr)
            )
        );
    public virtual bool _overflowReportNeeded { get; set; } = true;

    public RenderConstraintsTransformBox(
        AlignmentGeometry alignment,
        TextDirection? textDirection,
        Func<BoxConstraints, BoxConstraints> constraintsTransform,
        RenderBox? child = null,
        Clip clipBehavior = Clip.none
    )
        : base(
            alignment: alignment,
            textDirection: (
                textDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            child: child
        )
    {
        _constraintsTransform = constraintsTransform;
        _clipBehavior = clipBehavior;
    }

    public virtual Func<BoxConstraints, BoxConstraints> constraintsTransform
    {
        get => _constraintsTransform;
        set
        {
            var __value = value;
            if (Equals(_constraintsTransform, __value))
            {
                return;
            }
            _constraintsTransform = __value;
            bool needsLayout =
                (_childConstraints is null) || (!Equals(_childConstraints, __value(constraints)));
            if (needsLayout)
            {
                markNeedsLayout();
            }
        }
    }
    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            if (!Equals(__value, _clipBehavior))
            {
                _clipBehavior = __value;
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return base.computeMinIntrinsicHeight(
            constraintsTransform(new BoxConstraints(maxWidth: width)).maxWidth
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return base.computeMaxIntrinsicHeight(
            constraintsTransform(new BoxConstraints(maxWidth: width)).maxWidth
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return base.computeMinIntrinsicWidth(
            constraintsTransform(new BoxConstraints(maxHeight: height)).maxHeight
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return base.computeMaxIntrinsicWidth(
            constraintsTransform(new BoxConstraints(maxHeight: height)).maxHeight
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        Size? childSize = child?.getDryLayout(constraintsTransform(constraints));
        return (childSize is null)
            ? constraints.smallest
            : constraints.constrain(
                (
                    (
                        childSize
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        BoxConstraints childConstraints = constraintsTransform(constraints);
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if (result is null)
        {
            return null;
        }
        Size childSize = childLocal.getDryLayout(childConstraints);
        Size size = constraints.constrain(childSize);
        return (
                result
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) + resolvedAlignment.alongOffset(size - childSize).dy;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        RenderBox? childLocal = child;
        if (childLocal is not null)
        {
            BoxConstraints childConstraints = constraintsTransform(constraintsLocal);
            DartRuntimePrimitives.Assert(() => childConstraints.isNormalized);
            _childConstraints = childConstraints;
            childLocal.layout(childConstraints, parentUsesSize: true);
            size = constraintsLocal.constrain(childLocal.size);
            alignChild();
            var childParentData = ((BoxParentData?)(object?)childLocal.parentData!)!;
            _overflowContainerRect = Offset.zero & size;
            _overflowChildRect = childParentData.offset & childLocal.size;
        }
        else
        {
            size = constraintsLocal.smallest;
            _overflowContainerRect = Rect.zero;
            _overflowChildRect = Rect.zero;
        }
        _isOverflowing = RelativeRect
            .CreateFromRect(_overflowContainerRect, _overflowChildRect)
            .hasInsets;
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is null)
        {
            return;
        }
        if (!_isOverflowing)
        {
            base.paint(context, offset);
            return;
        }
        _clipRectLayer.layer = context.pushClipRect(
            needsCompositing,
            offset,
            Offset.zero & size,
            base.paint,
            clipBehavior: clipBehavior,
            oldLayer: _clipRectLayer.layer
        );
        DartRuntimePrimitives.Assert(() =>
        {
            if (size.isEmpty)
            {
                return true;
            }
            switch (clipBehavior)
            {
                case Clip.none:
                {
                    paintOverflowIndicator(
                        context,
                        offset,
                        _overflowContainerRect,
                        _overflowChildRect
                    );
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
        _clipRectLayer.layer = null;
        foreach (TextPainter painter in _indicatorLabel)
        {
            painter.dispose();
        }
        base.dispose();
    }

    public override Rect? describeApproximatePaintClip(RenderObject child)
    {
        switch (clipBehavior)
        {
            case Clip.none:
            {
                return null;
            }
            case Clip.hardEdge:
            case Clip.antiAlias:
            case Clip.antiAliasWithSaveLayer:
            {
                return _isOverflowing ? (Offset.zero & size) : null;
            }
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string toStringShort()
    {
        string header = base.toStringShort();
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            if (_isOverflowing)
            {
                header += " OVERFLOWING";
            }
        }
        return header;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual string _formatPixels(double value)
    {
        DartRuntimePrimitives.Assert(() => value > 0.0);
        return value switch
        {
            > 10.0 => value.toStringAsFixed(0L),
            > 1.0 => value.toStringAsFixed(1L),
            _ => value.toStringAsPrecision(3L),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual List<_OverflowRegionData__debug_overflow_indicator> _calculateOverflowRegions(
        RelativeRect overflow,
        Rect containerRect
    )
    {
        var regions = new List<_OverflowRegionData__debug_overflow_indicator>();
        if (overflow.left > 0.0)
        {
            var markerRect = Rect.fromLTWH(
                0.0,
                0.0,
                containerRect.width * DebugOverflowIndicatorMixin._indicatorFraction,
                containerRect.height
            );
            regions.Add(
                new _OverflowRegionData__debug_overflow_indicator(
                    rect: markerRect,
                    label: $"LEFT OVERFLOWED BY {_formatPixels(overflow.left)} PIXELS",
                    labelOffset: markerRect.centerLeft
                        + new Offset(
                            DebugOverflowIndicatorMixin._indicatorFontSizePixels
                                + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels,
                            0.0
                        ),
                    rotation: Math.PI / 2.0,
                    side: _OverflowSide__debug_overflow_indicator.left
                )
            );
        }
        if (overflow.right > 0.0)
        {
            var markerRectLocal = Rect.fromLTWH(
                containerRect.width * (1.0 - DebugOverflowIndicatorMixin._indicatorFraction),
                0.0,
                containerRect.width * DebugOverflowIndicatorMixin._indicatorFraction,
                containerRect.height
            );
            regions.Add(
                new _OverflowRegionData__debug_overflow_indicator(
                    rect: markerRectLocal,
                    label: $"RIGHT OVERFLOWED BY {_formatPixels(overflow.right)} PIXELS",
                    labelOffset: markerRectLocal.centerRight
                        - new Offset(
                            DebugOverflowIndicatorMixin._indicatorFontSizePixels
                                + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels,
                            0.0
                        ),
                    rotation: -Math.PI / 2.0,
                    side: _OverflowSide__debug_overflow_indicator.right
                )
            );
        }
        if (overflow.top > 0.0)
        {
            var markerRectAlternate = Rect.fromLTWH(
                0.0,
                0.0,
                containerRect.width,
                containerRect.height * DebugOverflowIndicatorMixin._indicatorFraction
            );
            regions.Add(
                new _OverflowRegionData__debug_overflow_indicator(
                    rect: markerRectAlternate,
                    label: $"TOP OVERFLOWED BY {_formatPixels(overflow.top)} PIXELS",
                    labelOffset: markerRectAlternate.topCenter
                        + new Offset(0.0, DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels),
                    side: _OverflowSide__debug_overflow_indicator.top
                )
            );
        }
        if (overflow.bottom > 0.0)
        {
            var markerRectNested = Rect.fromLTWH(
                0.0,
                containerRect.height * (1.0 - DebugOverflowIndicatorMixin._indicatorFraction),
                containerRect.width,
                containerRect.height * DebugOverflowIndicatorMixin._indicatorFraction
            );
            regions.Add(
                new _OverflowRegionData__debug_overflow_indicator(
                    rect: markerRectNested,
                    label: $"BOTTOM OVERFLOWED BY {_formatPixels(overflow.bottom)} PIXELS",
                    labelOffset: markerRectNested.bottomCenter
                        - new Offset(
                            0.0,
                            DebugOverflowIndicatorMixin._indicatorFontSizePixels
                                + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels
                        ),
                    side: _OverflowSide__debug_overflow_indicator.bottom
                )
            );
        }
        return regions;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _reportOverflow(RelativeRect overflow, List<DiagnosticsNode>? overflowHints)
    {
        overflowHints ??= new List<DiagnosticsNode>();
        if (checked((long)overflowHints.Count) == 0)
        {
            overflowHints.Add(
                new ErrorDescription(
                    $"The edge of the {GetType()} that is "
                        + "overflowing has been marked in the rendering with a yellow and black "
                        + "striped pattern. This is usually caused by the contents being too big "
                        + $"for the {GetType()}."
                )
            );
            overflowHints.Add(
                new ErrorHint(
                    "This is considered an error condition because it indicates that there "
                        + "is content that cannot be seen. If the content is legitimately bigger "
                        + "than the available space, consider clipping it with a ClipRect widget "
                        + $"before putting it in the {GetType()}, or using a scrollable "
                        + "container, like a ListView."
                )
            );
        }
        var overflows = new List<string>();
        var overflowText = "";
        DartRuntimePrimitives.Assert(() => checked((long)overflows.Count) != 0);
        switch (checked((long)overflows.Count))
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
                overflows[(int)(checked(overflows.Count) - 1L)] =
                    $"and {overflows[(int)(checked(overflows.Count) - 1L)]}";
                overflowText = string.Join(", ", overflows);
                break;
            }
        }
        FlutterError.reportError(
            new FlutterErrorDetails(
                exception: new FlutterError($"A {GetType()} overflowed by {overflowText}."),
                library: "rendering library",
                context: new ErrorDescription("during layout"),
                informationCollector: () =>
                    new List<DiagnosticsNode>
                    {
                        describeForError($"The specific {GetType()} in question is"),
                        new DiagnosticsNode(
                            DartCoreExtensions.repeat("◢◤", checked(FlutterError.wrapWidth / 2L)),
                            allowWrap: false
                        ),
                    }
            )
        );
    }

    public virtual void paintOverflowIndicator(
        PaintingContext context,
        Offset offset,
        Rect containerRect,
        Rect childRect,
        List<DiagnosticsNode>? overflowHints = null
    )
    {
        var overflow = RelativeRect.CreateFromRect(containerRect, childRect);
        if (
            (overflow.left <= 0.0)
            && (overflow.right <= 0.0)
            && (overflow.top <= 0.0)
            && (overflow.bottom <= 0.0)
        )
        {
            return;
        }
        List<_OverflowRegionData__debug_overflow_indicator> overflowRegions =
            _calculateOverflowRegions(overflow, containerRect);
        foreach (var region in overflowRegions)
        {
            context.canvas.drawRect(
                region.rect.shift(offset),
                DebugOverflowIndicatorMixin._indicatorPaint
            );
            var textSpan = (
                (TextSpan?)
                    (object?)
                        _indicatorLabel[(int)FoundationRuntimePorts.EnumIndex(region.side)].text
            )!;
            if (textSpan?.text != region.label)
            {
                _indicatorLabel[(int)FoundationRuntimePorts.EnumIndex(region.side)].text =
                    new TextSpan(
                        text: region.label,
                        style: DebugOverflowIndicatorMixin._indicatorTextStyle
                    );
                _indicatorLabel[(int)FoundationRuntimePorts.EnumIndex(region.side)].layout();
            }
            Offset labelOffsetLocal = region.labelOffset + offset;
            var centerOffset = new Offset(
                -_indicatorLabel[(int)FoundationRuntimePorts.EnumIndex(region.side)].width / 2.0,
                0.0
            );
            Rect textBackgroundRect =
                centerOffset
                & _indicatorLabel[(int)FoundationRuntimePorts.EnumIndex(region.side)].size;
            context.canvas.save();
            context.canvas.translate(labelOffsetLocal.dx, labelOffsetLocal.dy);
            context.canvas.rotate(region.rotation);
            context.canvas.drawRect(
                textBackgroundRect,
                DebugOverflowIndicatorMixin._labelBackgroundPaint
            );
            _indicatorLabel[(int)FoundationRuntimePorts.EnumIndex(region.side)]
                .paint(context.canvas, centerOffset);
            context.canvas.restore();
        }
        if (_overflowReportNeeded)
        {
            _overflowReportNeeded = false;
            _reportOverflow(overflow, overflowHints);
        }
    }

    public override void reassemble()
    {
        base.reassemble();
        DartRuntimePrimitives.Assert(() =>
        {
            _overflowReportNeeded = true;
            return true;
        });
    }
}

public class RenderSizedOverflowBox : RenderAligningShiftedBox
{
    internal virtual Size _requestedSize { get; set; } = default!;

    public RenderSizedOverflowBox(
        RenderBox? child = null,
        Size requestedSize = default!,
        AlignmentGeometry alignment = default!,
        TextDirection? textDirection = null
    )
        : base(
            child: child,
            alignment: alignment ?? Alignment.center,
            textDirection: (
                textDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        )
    {
        _requestedSize = requestedSize;
    }

    public virtual Size requestedSize
    {
        get => _requestedSize;
        set
        {
            var __value = value;
            if (Equals(_requestedSize, __value))
            {
                return;
            }
            _requestedSize = __value;
            markNeedsLayout();
        }
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return _requestedSize.width;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return _requestedSize.width;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return _requestedSize.height;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return _requestedSize.height;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return base.computeDistanceToActualBaseline(baseline);
        }
        double? result = childLocal.getDistanceToActualBaseline(baseline);
        if (result is null)
        {
            return base.computeDistanceToActualBaseline(baseline);
        }
        var childParentData = ((BoxParentData?)(object?)childLocal.parentData!)!;
        return (
                result
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) + childParentData.offset.dy;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        double? result = childLocal.getDryBaseline(constraints, baseline);
        if (result is null)
        {
            return null;
        }
        Size childSize = childLocal.getDryLayout(constraints);
        Size size = getDryLayout(constraints);
        return (
                result
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) + resolvedAlignment.alongOffset(size - childSize).dy;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return constraints.constrain(_requestedSize);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        size = constraints.constrain(_requestedSize);
        if (child is not null)
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

    public RenderFractionallySizedOverflowBox(
        RenderBox? child = null,
        double? widthFactor = null,
        double? heightFactor = null,
        AlignmentGeometry alignment = default!,
        TextDirection? textDirection = null
    )
        : base(
            child: child,
            alignment: alignment ?? Alignment.center,
            textDirection: (
                textDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        )
    {
        _widthFactor = widthFactor;
        _heightFactor = heightFactor;
    }

    public virtual double? widthFactor
    {
        get => _widthFactor;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is null) || (__value >= 0.0));
            if (_widthFactor == __value)
            {
                return;
            }
            _widthFactor = __value;
            markNeedsLayout();
        }
    }
    public virtual double? heightFactor
    {
        get => _heightFactor;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is null) || (__value >= 0.0));
            if (_heightFactor == __value)
            {
                return;
            }
            _heightFactor = __value;
            markNeedsLayout();
        }
    }

    internal virtual BoxConstraints _getInnerConstraints(BoxConstraints constraints)
    {
        double minWidthLocal = constraints.minWidth;
        double maxWidthLocal = constraints.maxWidth;
        if (_widthFactor is not null)
        {
            double width =
                maxWidthLocal
                * (
                    _widthFactor
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
            minWidthLocal = width;
            maxWidthLocal = width;
        }
        double minHeightLocal = constraints.minHeight;
        double maxHeightLocal = constraints.maxHeight;
        if (_heightFactor is not null)
        {
            double height =
                maxHeightLocal
                * (
                    _heightFactor
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
            minHeightLocal = height;
            maxHeightLocal = height;
        }
        return new BoxConstraints(
            minWidth: minWidthLocal,
            maxWidth: maxWidthLocal,
            minHeight: minHeightLocal,
            maxHeight: maxHeightLocal
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        double result = default!;
        if (child is null)
        {
            result = base.computeMinIntrinsicWidth(height);
        }
        else
        {
            result = child!.getMinIntrinsicWidth(height * (_heightFactor ?? 1.0));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(result));
        return result / (_widthFactor ?? 1.0);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double result = default!;
        if (child is null)
        {
            result = base.computeMaxIntrinsicWidth(height);
        }
        else
        {
            result = child!.getMaxIntrinsicWidth(height * (_heightFactor ?? 1.0));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(result));
        return result / (_widthFactor ?? 1.0);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double result = default!;
        if (child is null)
        {
            result = base.computeMinIntrinsicHeight(width);
        }
        else
        {
            result = child!.getMinIntrinsicHeight(width * (_widthFactor ?? 1.0));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(result));
        return result / (_heightFactor ?? 1.0);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        double result = default!;
        if (child is null)
        {
            result = base.computeMaxIntrinsicHeight(width);
        }
        else
        {
            result = child!.getMaxIntrinsicHeight(width * (_widthFactor ?? 1.0));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(result));
        return result / (_heightFactor ?? 1.0);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if (child is not null)
        {
            Size childSize = child!.getDryLayout(_getInnerConstraints(constraints));
            return constraints.constrain(childSize);
        }
        return constraints.constrain(_getInnerConstraints(constraints).constrain(Size.zero));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        BoxConstraints childConstraints = _getInnerConstraints(constraints);
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if (result is null)
        {
            return null;
        }
        Size childSize = childLocal.getDryLayout(childConstraints);
        Size size = getDryLayout(constraints);
        return (
                result
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) + resolvedAlignment.alongOffset(size - childSize).dy;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        if (child is not null)
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
        properties.add(new DoubleProperty("widthFactor", _widthFactor, ifNull: "pass-through"));
        properties.add(new DoubleProperty("heightFactor", _heightFactor, ifNull: "pass-through"));
    }
}

public abstract class SingleChildLayoutDelegate
{
    internal virtual Listenable? _relayout { get; private set; }

    protected SingleChildLayoutDelegate(Listenable? relayout = null)
    {
        _relayout = relayout;
    }

    public virtual Size getSize(BoxConstraints constraints) => constraints.biggest;

    public virtual BoxConstraints getConstraintsForChild(BoxConstraints constraints) => constraints;

    public virtual Offset getPositionForChild(Size size, Size childSize) => Offset.zero;

    public abstract bool shouldRelayout(SingleChildLayoutDelegate oldDelegate);
}

public class RenderCustomSingleChildLayoutBox : RenderShiftedBox
{
    internal virtual SingleChildLayoutDelegate _delegate { get; set; } = default!;

    public RenderCustomSingleChildLayoutBox(
        RenderBox? child = null,
        SingleChildLayoutDelegate @delegate = default!
    )
        : base(child)
    {
        _delegate = @delegate;
    }

    public virtual SingleChildLayoutDelegate @delegate
    {
        get => _delegate;
        set
        {
            var newDelegate = value;
            if (Equals(_delegate, newDelegate))
            {
                return;
            }
            SingleChildLayoutDelegate oldDelegate = _delegate;
            if (
                (
                    !Equals(
                        DartRuntimePrimitives.RuntimeType(newDelegate),
                        DartRuntimePrimitives.RuntimeType(oldDelegate)
                    )
                ) || newDelegate.shouldRelayout(oldDelegate)
            )
            {
                markNeedsLayout();
            }
            _delegate = newDelegate;
            if (attached)
            {
                oldDelegate._relayout?.removeListener(markNeedsLayout);
                newDelegate._relayout?.addListener(markNeedsLayout);
            }
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _delegate._relayout?.addListener(markNeedsLayout);
    }

    public override void detach()
    {
        _delegate._relayout?.removeListener(markNeedsLayout);
        base.detach();
    }

    internal virtual Size _getSize(BoxConstraints constraints)
    {
        return constraints.constrain(_delegate.getSize(constraints));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        double widthLocal = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(widthLocal))
        {
            return widthLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double widthLocal = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(widthLocal))
        {
            return widthLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double heightLocal = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(heightLocal))
        {
            return heightLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        double heightLocal = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(heightLocal))
        {
            return heightLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _getSize(constraints);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        BoxConstraints childConstraints = @delegate.getConstraintsForChild(constraints);
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if (result is null)
        {
            return null;
        }
        return (
                result
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
            + @delegate
                .getPositionForChild(
                    _getSize(constraints),
                    childConstraints.isTight
                        ? childConstraints.smallest
                        : childLocal.getDryLayout(childConstraints)
                )
                .dy;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        size = _getSize(constraints);
        if (child is not null)
        {
            BoxConstraints childConstraints = @delegate.getConstraintsForChild(constraints);
            DartRuntimePrimitives.Assert(() =>
                childConstraints.debugAssertIsValid(isAppliedConstraint: true)
            );
            child!.layout(childConstraints, parentUsesSize: !childConstraints.isTight);
            var childParentData = ((BoxParentData?)(object?)child!.parentData!)!;
            childParentData.offset = @delegate.getPositionForChild(
                size,
                childConstraints.isTight ? childConstraints.smallest : child!.size
            );
        }
    }
}

public class RenderBaseline : RenderShiftedBox
{
    internal virtual double _baseline { get; set; } = default!;
    internal virtual TextBaseline _baselineType { get; set; } = default!;

    public RenderBaseline(
        RenderBox? child = null,
        double baseline = default!,
        TextBaseline baselineType = default!
    )
        : base(child)
    {
        _baseline = baseline;
        _baselineType = baselineType;
    }

    public virtual double baseline
    {
        get => _baseline;
        set
        {
            var __value = value;
            if (_baseline == __value)
            {
                return;
            }
            _baseline = __value;
            markNeedsLayout();
        }
    }
    public virtual TextBaseline baselineType
    {
        get => _baselineType;
        set
        {
            var __value = value;
            if (Equals(_baselineType, __value))
            {
                return;
            }
            _baselineType = __value;
            markNeedsLayout();
        }
    }

    internal virtual (Size size, double top) _computeSizes(
        BoxConstraints constraints,
        Func<RenderBox, BoxConstraints, Size> layoutChild,
        Func<RenderBox, BoxConstraints, TextBaseline, double?> getBaseline
    )
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return (size: constraints.smallest, top: 0);
        }
        BoxConstraints childConstraints = constraints.loosen();
        Size childSize = layoutChild(childLocal, childConstraints);
        double childBaseline =
            getBaseline(childLocal, childConstraints, baselineType) ?? childSize.height;
        double topLocal = baseline - childBaseline;
        return (
            size: constraints.constrain(new Size(childSize.width, topLocal + childSize.height)),
            top: topLocal
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSizes(
            constraints,
            ChildLayoutHelper.dryLayoutChild,
            ChildLayoutHelper.getDryBaseline
        ).size;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        double? result1 = childLocal?.getDryBaseline(constraints.loosen(), baseline);
        double? result2 = childLocal?.getDryBaseline(constraints.loosen(), baselineType);
        if ((result1 is null) || (result2 is null))
        {
            return null;
        }
        return this.baseline
            + (
                result1
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
            - (
                result2
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        var (sizeLocal, top) = _computeSizes(
            constraints,
            ChildLayoutHelper.layoutChild,
            ChildLayoutHelper.getBaseline
        );
        size = sizeLocal;
        ((BoxParentData?)(object?)child?.parentData)!?.offset = new Offset(0.0, top);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("baseline", baseline));
        properties.add(new EnumProperty<TextBaseline>("baselineType", baselineType));
    }
}
