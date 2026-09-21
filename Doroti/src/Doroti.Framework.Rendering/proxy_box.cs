// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/proxy_box.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public class RenderProxyBox
    : RenderBox,
        RenderObjectWithChildMixin<RenderBox>,
        RenderProxyBoxMixin<RenderBox>
{
    public virtual RenderBox? _child { get; set; } = default;

    public RenderProxyBox(RenderBox? child = null)
    {
        this.child = child;
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void setupParentData(RenderObject child)
    {
        if (child.parentData is not ParentData)
        {
            child.parentData = new ParentData();
        }
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return child?.getMinIntrinsicWidth(height) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return child?.getMaxIntrinsicWidth(height) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return child?.getMinIntrinsicHeight(width) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return child?.getMaxIntrinsicHeight(width) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return child?.getDistanceToActualBaseline(baseline)
            ?? base.computeDistanceToActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        return (childLocal is null)
            ? base.computeDryBaseline(constraints, baseline)
            : childLocal.getDryBaseline(constraints, baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return child?.getDryLayout(constraints) ?? computeSizeForNoChild(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        child?.layout(constraints, parentUsesSize: true);
        size = child?.size ?? computeSizeForNoChild(constraints);
        return;
    }

    public virtual Size computeSizeForNoChild(BoxConstraints constraints)
    {
        return constraints.smallest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        return child?.hitTest(result, position: position) ?? false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform) { }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return;
        }
        context.paintChild(childLocal, offset);
    }
}

public interface RenderProxyBoxMixin<T>
    where T : RenderBox
{
    public void setupParentData(RenderObject child);
    public double computeMinIntrinsicWidth(double height);
    public double computeMaxIntrinsicWidth(double height);
    public double computeMinIntrinsicHeight(double width);
    public double computeMaxIntrinsicHeight(double width);
    public double? computeDistanceToActualBaseline(TextBaseline baseline);
    public double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline);
    public Size computeDryLayout(BoxConstraints constraints);
    public void performLayout();
    public Size computeSizeForNoChild(BoxConstraints constraints);
    public bool hitTestChildren(BoxHitTestResult result, Offset position);
    public void applyPaintTransform(RenderObject child, Matrix4 transform);
    public void paint(PaintingContext context, Offset offset);
}

public enum HitTestBehavior
{
    deferToChild,
    opaque,
    translucent,
}

public abstract class RenderProxyBoxWithHitTestBehavior : RenderProxyBox
{
    public virtual HitTestBehavior behavior { get; set; } = default!;

    protected RenderProxyBoxWithHitTestBehavior(
        HitTestBehavior behavior = HitTestBehavior.deferToChild,
        RenderBox? child = null
    )
        : base(child)
    {
        this.behavior = behavior;
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        var hitTarget = false;
        if (size.contains(position))
        {
            hitTarget = hitTestChildren(result, position: position) || hitTestSelf(position);
            if (hitTarget || Equals(behavior, HitTestBehavior.translucent))
            {
                result.add(new BoxHitTestEntry(this, position));
            }
        }
        return hitTarget;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position) => Equals(behavior, HitTestBehavior.opaque);

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<HitTestBehavior>("behavior", behavior, defaultValue: null));
    }
}

public class RenderConstrainedBox : RenderProxyBox
{
    internal virtual BoxConstraints _additionalConstraints { get; set; } = default!;

    public RenderConstrainedBox(
        RenderBox? child = null,
        BoxConstraints additionalConstraints = default!
    )
        : base(child)
    {
        _additionalConstraints = additionalConstraints;
        System.Diagnostics.Debug.Assert(additionalConstraints.debugAssertIsValid());
    }

    public virtual BoxConstraints additionalConstraints
    {
        get => _additionalConstraints;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value.debugAssertIsValid());
            if (Equals(_additionalConstraints, __value))
            {
                return;
            }
            _additionalConstraints = __value;
            markNeedsLayout();
        }
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        if (_additionalConstraints.hasBoundedWidth && _additionalConstraints.hasTightWidth)
        {
            return _additionalConstraints.minWidth;
        }
        double width = base.computeMinIntrinsicWidth(height);
        DartRuntimePrimitives.Assert(() => double.IsFinite(width));
        if (!_additionalConstraints.hasInfiniteWidth)
        {
            return _additionalConstraints.constrainWidth(width);
        }
        return width;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if (_additionalConstraints.hasBoundedWidth && _additionalConstraints.hasTightWidth)
        {
            return _additionalConstraints.minWidth;
        }
        double width = base.computeMaxIntrinsicWidth(height);
        DartRuntimePrimitives.Assert(() => double.IsFinite(width));
        if (!_additionalConstraints.hasInfiniteWidth)
        {
            return _additionalConstraints.constrainWidth(width);
        }
        return width;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if (_additionalConstraints.hasBoundedHeight && _additionalConstraints.hasTightHeight)
        {
            return _additionalConstraints.minHeight;
        }
        double height = base.computeMinIntrinsicHeight(width);
        DartRuntimePrimitives.Assert(() => double.IsFinite(height));
        if (!_additionalConstraints.hasInfiniteHeight)
        {
            return _additionalConstraints.constrainHeight(height);
        }
        return height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if (_additionalConstraints.hasBoundedHeight && _additionalConstraints.hasTightHeight)
        {
            return _additionalConstraints.minHeight;
        }
        double height = base.computeMaxIntrinsicHeight(width);
        DartRuntimePrimitives.Assert(() => double.IsFinite(height));
        if (!_additionalConstraints.hasInfiniteHeight)
        {
            return _additionalConstraints.constrainHeight(height);
        }
        return height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        return child?.getDryBaseline(_additionalConstraints.enforce(constraints), baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        if (child is not null)
        {
            child!.layout(_additionalConstraints.enforce(constraintsLocal), parentUsesSize: true);
            size = child!.size;
        }
        else
        {
            size = _additionalConstraints.enforce(constraintsLocal).constrain(Size.zero);
        }
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return child?.getDryLayout(_additionalConstraints.enforce(constraints))
            ?? _additionalConstraints.enforce(constraints).constrain(Size.zero);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        base.debugPaintSize(context, offset);
        DartRuntimePrimitives.Assert(() =>
        {
            Paint paint = default!;
            if ((child is null) || child!.size.isEmpty)
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
        properties.add(
            new DiagnosticsProperty<BoxConstraints>("additionalConstraints", additionalConstraints)
        );
    }
}

public class RenderLimitedBox : RenderProxyBox
{
    internal virtual double _maxWidth { get; set; } = default!;
    internal virtual double _maxHeight { get; set; } = default!;

    public RenderLimitedBox(
        RenderBox? child = null,
        double maxWidth = double.PositiveInfinity,
        double maxHeight = double.PositiveInfinity
    )
        : base(child)
    {
        _maxWidth = maxWidth;
        _maxHeight = maxHeight;
        System.Diagnostics.Debug.Assert(maxWidth >= 0.0);
        System.Diagnostics.Debug.Assert(maxHeight >= 0.0);
    }

    public virtual double maxWidth
    {
        get => _maxWidth;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value >= 0.0);
            if (_maxWidth == __value)
            {
                return;
            }
            _maxWidth = __value;
            markNeedsLayout();
        }
    }
    public virtual double maxHeight
    {
        get => _maxHeight;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value >= 0.0);
            if (_maxHeight == __value)
            {
                return;
            }
            _maxHeight = __value;
            markNeedsLayout();
        }
    }

    internal virtual BoxConstraints _limitConstraints(BoxConstraints constraints)
    {
        return new BoxConstraints(
            minWidth: constraints.minWidth,
            maxWidth: constraints.hasBoundedWidth
                ? constraints.maxWidth
                : constraints.constrainWidth(maxWidth),
            minHeight: constraints.minHeight,
            maxHeight: constraints.hasBoundedHeight
                ? constraints.maxHeight
                : constraints.constrainHeight(maxHeight)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _computeSize(
        BoxConstraints constraints,
        Func<RenderBox, BoxConstraints, Size> layoutChild
    )
    {
        if (child is not null)
        {
            Size childSize = layoutChild(child!, _limitConstraints(constraints));
            return constraints.constrain(childSize);
        }
        return _limitConstraints(constraints).constrain(Size.zero);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(
            constraints: constraints,
            layoutChild: ChildLayoutHelper.dryLayoutChild
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: constraints, layoutChild: ChildLayoutHelper.layoutChild);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DoubleProperty("maxWidth", maxWidth, defaultValue: double.PositiveInfinity)
        );
        properties.add(
            new DoubleProperty("maxHeight", maxHeight, defaultValue: double.PositiveInfinity)
        );
    }
}

public class RenderAspectRatio : RenderProxyBox
{
    internal virtual double _aspectRatio { get; set; } = default!;

    public RenderAspectRatio(RenderBox? child = null, double aspectRatio = default!)
        : base(child)
    {
        _aspectRatio = aspectRatio;
        System.Diagnostics.Debug.Assert(aspectRatio > 0.0);
        System.Diagnostics.Debug.Assert(double.IsFinite(aspectRatio));
    }

    public virtual double aspectRatio
    {
        get => _aspectRatio;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value > 0.0);
            DartRuntimePrimitives.Assert(() => double.IsFinite(__value));
            if (_aspectRatio == __value)
            {
                return;
            }
            _aspectRatio = __value;
            markNeedsLayout();
        }
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        if (double.IsFinite(height))
        {
            return height * _aspectRatio;
        }
        return child?.getMinIntrinsicWidth(height) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if (double.IsFinite(height))
        {
            return height * _aspectRatio;
        }
        return child?.getMaxIntrinsicWidth(height) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if (double.IsFinite(width))
        {
            return width / _aspectRatio;
        }
        return child?.getMinIntrinsicHeight(width) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if (double.IsFinite(width))
        {
            return width / _aspectRatio;
        }
        return child?.getMaxIntrinsicHeight(width) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _applyAspectRatio(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid());
        DartRuntimePrimitives.Assert(() =>
        {
            if (!constraints.hasBoundedWidth && !constraints.hasBoundedHeight)
            {
                throw new FlutterError(
                    $"{GetType()} has unbounded constraints.\n"
                        + $"This {GetType()} was given an aspect ratio of {aspectRatio} but was given "
                        + "both unbounded width and unbounded height constraints. Because both "
                        + "constraints were unbounded, this render object doesn't know how much "
                        + "size to consume."
                );
            }
            return true;
        });
        if (constraints.isTight)
        {
            return constraints.smallest;
        }
        double width = constraints.maxWidth;
        double height = default!;
        if (double.IsFinite(width))
        {
            height = width / _aspectRatio;
        }
        else
        {
            height = constraints.maxHeight;
            width = height * _aspectRatio;
        }
        if (width > constraints.maxWidth)
        {
            width = constraints.maxWidth;
            height = width / _aspectRatio;
        }
        if (height > constraints.maxHeight)
        {
            height = constraints.maxHeight;
            width = height * _aspectRatio;
        }
        if (width < constraints.minWidth)
        {
            width = constraints.minWidth;
            height = width / _aspectRatio;
        }
        if (height < constraints.minHeight)
        {
            height = constraints.minHeight;
            width = height * _aspectRatio;
        }
        return constraints.constrain(new Size(width, height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _applyAspectRatio(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        return base.computeDryBaseline(
            BoxConstraints.CreateTight(getDryLayout(constraints)),
            baseline
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = getDryLayout(constraints);
        child?.layout(BoxConstraints.CreateTight(size));
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("aspectRatio", aspectRatio));
    }
}

public class RenderIntrinsicWidth : RenderProxyBox
{
    internal virtual double? _stepWidth { get; set; } = default;
    internal virtual double? _stepHeight { get; set; } = default;

    public RenderIntrinsicWidth(
        double? stepWidth = null,
        double? stepHeight = null,
        RenderBox? child = null
    )
        : base(child)
    {
        _stepWidth = stepWidth;
        _stepHeight = stepHeight;
        System.Diagnostics.Debug.Assert(
            (stepWidth is null)
                || (
                    (
                        stepWidth
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) > 0.0
                )
        );
        System.Diagnostics.Debug.Assert(
            (stepHeight is null)
                || (
                    (
                        stepHeight
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) > 0.0
                )
        );
    }

    public virtual double? stepWidth
    {
        get => _stepWidth;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() =>
                (__value is null)
                || (
                    (
                        __value
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) > 0.0
                )
            );
            if (__value == _stepWidth)
            {
                return;
            }
            _stepWidth = __value;
            markNeedsLayout();
        }
    }
    public virtual double? stepHeight
    {
        get => _stepHeight;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() =>
                (__value is null)
                || (
                    (
                        __value
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) > 0.0
                )
            );
            if (__value == _stepHeight)
            {
                return;
            }
            _stepHeight = __value;
            markNeedsLayout();
        }
    }

    internal static double _applyStep(double input, double? step)
    {
        DartRuntimePrimitives.Assert(() => double.IsFinite(input));
        if (step is null)
        {
            return input;
        }
        return (
                input
                / (
                    step
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            ).ceil()
            * (
                step
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return getMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if (child is null)
        {
            return 0.0;
        }
        double width = child!.getMaxIntrinsicWidth(height);
        return _applyStep(width, _stepWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if (child is null)
        {
            return 0.0;
        }
        if (!double.IsFinite(width))
        {
            width = getMaxIntrinsicWidth(double.PositiveInfinity);
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(width));
        double height = child!.getMinIntrinsicHeight(width);
        return _applyStep(height, _stepHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if (child is null)
        {
            return 0.0;
        }
        if (!double.IsFinite(width))
        {
            width = getMaxIntrinsicWidth(double.PositiveInfinity);
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(width));
        double height = child!.getMaxIntrinsicHeight(width);
        return _applyStep(height, _stepHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BoxConstraints _childConstraints(RenderBox child, BoxConstraints constraints)
    {
        return constraints.tighten(
            width: constraints.hasTightWidth
                ? null
                : _applyStep(child.getMaxIntrinsicWidth(constraints.maxHeight), _stepWidth),
            height: (stepHeight is null)
                ? null
                : _applyStep(child.getMaxIntrinsicHeight(constraints.maxWidth), _stepHeight)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _computeSize(
        Func<RenderBox, BoxConstraints, Size> layoutChild,
        BoxConstraints constraints
    )
    {
        RenderBox? childLocal = child;
        return (childLocal is null)
            ? constraints.smallest
            : layoutChild(childLocal, _childConstraints(childLocal, constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(
            layoutChild: ChildLayoutHelper.dryLayoutChild,
            constraints: constraints
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        return childLocal?.getDryBaseline(_childConstraints(childLocal, constraints), baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(layoutChild: ChildLayoutHelper.layoutChild, constraints: constraints);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("stepWidth", stepWidth));
        properties.add(new DoubleProperty("stepHeight", stepHeight));
    }
}

public class RenderIntrinsicHeight : RenderProxyBox
{
    public RenderIntrinsicHeight(RenderBox? child = null)
        : base(child) { }

    public override double computeMinIntrinsicWidth(double height)
    {
        if (child is null)
        {
            return 0.0;
        }
        if (!double.IsFinite(height))
        {
            height = child!.getMaxIntrinsicHeight(double.PositiveInfinity);
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(height));
        return child!.getMinIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if (child is null)
        {
            return 0.0;
        }
        if (!double.IsFinite(height))
        {
            height = child!.getMaxIntrinsicHeight(double.PositiveInfinity);
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(height));
        return child!.getMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return getMaxIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BoxConstraints _childConstraints(RenderBox child, BoxConstraints constraints)
    {
        return constraints.hasTightHeight
            ? constraints
            : constraints.tighten(height: child.getMaxIntrinsicHeight(constraints.maxWidth));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _computeSize(
        Func<RenderBox, BoxConstraints, Size> layoutChild,
        BoxConstraints constraints
    )
    {
        RenderBox? childLocal = child;
        return (childLocal is null)
            ? constraints.smallest
            : layoutChild(childLocal, _childConstraints(childLocal, constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(
            layoutChild: ChildLayoutHelper.dryLayoutChild,
            constraints: constraints
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        return childLocal?.getDryBaseline(_childConstraints(childLocal, constraints), baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(layoutChild: ChildLayoutHelper.layoutChild, constraints: constraints);
    }
}

public class RenderIgnoreBaseline : RenderProxyBox
{
    public RenderIgnoreBaseline(RenderBox? child = null)
        : base(child) { }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RenderOpacity : RenderProxyBox
{
    internal virtual long _alpha { get; set; } = default!;
    internal virtual double _opacity { get; set; } = default!;
    internal virtual bool _alwaysIncludeSemantics { get; set; } = default!;

    public RenderOpacity(
        double opacity = 1.0,
        bool alwaysIncludeSemantics = false,
        RenderBox? child = null
    )
        : base(child)
    {
        _opacity = opacity;
        _alwaysIncludeSemantics = alwaysIncludeSemantics;
        _alpha = Dart_uiLibrary.Color.getAlphaFromOpacity(opacity);
        System.Diagnostics.Debug.Assert((opacity >= 0.0) && (opacity <= 1.0));
    }

    public override bool alwaysNeedsCompositing => (child is not null) && (_alpha > 0L);
    public override bool isRepaintBoundary => alwaysNeedsCompositing;
    public virtual double opacity
    {
        get => _opacity;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0.0) && (__value <= 1.0));
            if (_opacity == __value)
            {
                return;
            }
            bool didNeedCompositing = alwaysNeedsCompositing;
            var wasVisible = _alpha != 0L;
            _opacity = __value;
            _alpha = Dart_uiLibrary.Color.getAlphaFromOpacity(_opacity);
            if (didNeedCompositing != alwaysNeedsCompositing)
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsCompositedLayerUpdate();
            if ((wasVisible != (_alpha != 0L)) && !alwaysIncludeSemantics)
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool alwaysIncludeSemantics
    {
        get => _alwaysIncludeSemantics;
        set
        {
            var __value = value;
            if (__value == _alwaysIncludeSemantics)
            {
                return;
            }
            _alwaysIncludeSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override bool paintsChild(RenderObject child)
    {
        var __child = (RenderBox)child;
        DartRuntimePrimitives.Assert(() => Equals(__child.parent, this));
        return _alpha > 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override OffsetLayer updateCompositedLayer(OffsetLayer? oldLayer)
    {
        var __oldLayer = oldLayer is null ? null : (OpacityLayer)oldLayer;
        OpacityLayer layer = __oldLayer ?? new OpacityLayer();
        layer.alpha = _alpha;
        return layer;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is null) || (_alpha == 0L))
        {
            return;
        }
        base.paint(context, offset);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if ((child is not null) && ((_alpha != 0L) || alwaysIncludeSemantics))
        {
            visitor(child!);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("opacity", opacity));
        properties.add(
            new FlagProperty(
                "alwaysIncludeSemantics",
                value: alwaysIncludeSemantics,
                ifTrue: "alwaysIncludeSemantics"
            )
        );
    }
}

public interface RenderAnimatedOpacityMixin<T>
    where T : RenderObject
{
    long? _alpha { get; set; }
    bool? _currentlyIsRepaintBoundary { get; set; }
    Animation<double>? _opacity { get; set; }
    bool? _alwaysIncludeSemantics { get; set; }

    public bool isRepaintBoundary { get; }
    public OffsetLayer updateCompositedLayer(OffsetLayer? oldLayer);
    public Animation<double> opacity { get; set; }
    public bool alwaysIncludeSemantics { get; set; }
    public void attach(PipelineOwner owner);
    public void detach();
    public void _updateOpacity();
    public bool paintsChild(RenderObject child);
    public void paint(PaintingContext context, Offset offset);
    public void visitChildrenForSemantics(Action<RenderObject> visitor);
    public void debugFillProperties(DiagnosticPropertiesBuilder properties);
}

public class RenderAnimatedOpacity : RenderProxyBox, RenderAnimatedOpacityMixin<RenderBox>
{
    public virtual long? _alpha { get; set; } = default;
    public virtual bool? _currentlyIsRepaintBoundary { get; set; } = default;
    public virtual Animation<double>? _opacity { get; set; } = default;
    public virtual bool? _alwaysIncludeSemantics { get; set; } = default;

    public RenderAnimatedOpacity(
        Animation<double> opacity,
        bool alwaysIncludeSemantics = false,
        RenderBox? child = null
    )
        : base(child)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
    }

    public override bool isRepaintBoundary =>
        (child is not null)
        && (
            _currentlyIsRepaintBoundary
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );

    public override OffsetLayer updateCompositedLayer(OffsetLayer? oldLayer)
    {
        var __oldLayer = oldLayer is null ? null : (OpacityLayer)oldLayer;
        OpacityLayer updatedLayer = __oldLayer ?? new OpacityLayer();
        updatedLayer.alpha = _alpha;
        return updatedLayer;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Animation<double> opacity
    {
        get => _opacity!;
        set
        {
            var __value = value;
            if (Equals(_opacity, __value))
            {
                return;
            }
            if (attached && (_opacity is not null))
            {
                opacity.removeListener(_updateOpacity);
            }
            _opacity = __value;
            if (attached)
            {
                opacity.addListener(_updateOpacity);
            }
            _updateOpacity();
        }
    }
    public virtual bool alwaysIncludeSemantics
    {
        get =>
            (
                _alwaysIncludeSemantics
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        set
        {
            var __value = value;
            if (__value == _alwaysIncludeSemantics)
            {
                return;
            }
            _alwaysIncludeSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        opacity.addListener(_updateOpacity);
        _updateOpacity();
    }

    public override void detach()
    {
        opacity.removeListener(_updateOpacity);
        base.detach();
    }

    public virtual void _updateOpacity()
    {
        long? oldAlpha = _alpha;
        _alpha = Dart_uiLibrary.Color.getAlphaFromOpacity(opacity.value);
        if (oldAlpha != _alpha)
        {
            bool? wasRepaintBoundary = _currentlyIsRepaintBoundary;
            _currentlyIsRepaintBoundary =
                (
                    _alpha
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) > 0L;
            if ((child is not null) && (wasRepaintBoundary != _currentlyIsRepaintBoundary))
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsCompositedLayerUpdate();
            if ((oldAlpha == 0L) || (_alpha == 0L))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }

    public override bool paintsChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        return opacity.value > 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (_alpha == 0L)
        {
            return;
        }
        base.paint(context, offset);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if ((child is not null) && ((_alpha != 0L) || alwaysIncludeSemantics))
        {
            visitor(child!);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Animation<double>>("opacity", opacity));
        properties.add(
            new FlagProperty(
                "alwaysIncludeSemantics",
                value: alwaysIncludeSemantics,
                ifTrue: "alwaysIncludeSemantics"
            )
        );
    }
}

public delegate Shader ShaderCallback(Rect bounds);

public class RenderShaderMask : RenderProxyBox
{
    internal virtual Func<Rect, Shader> _shaderCallback { get; set; } = default!;
    internal virtual BlendMode _blendMode { get; set; } = default!;

    public RenderShaderMask(
        RenderBox? child = null,
        Func<Rect, Shader> shaderCallback = default!,
        BlendMode blendMode = BlendMode.modulate
    )
        : base(child)
    {
        _shaderCallback = shaderCallback;
        _blendMode = blendMode;
    }

    public override ShaderMaskLayer? layer => ((ShaderMaskLayer?)base.layer)!;
    public virtual Func<Rect, Shader> shaderCallback
    {
        get => _shaderCallback;
        set
        {
            var __value = value;
            if (Equals(_shaderCallback, __value))
            {
                return;
            }
            _shaderCallback = __value;
            markNeedsPaint();
        }
    }
    public virtual BlendMode blendMode
    {
        get => _blendMode;
        set
        {
            var __value = value;
            if (Equals(_blendMode, __value))
            {
                return;
            }
            _blendMode = __value;
            markNeedsPaint();
        }
    }
    public override bool alwaysNeedsCompositing => child is not null;

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            DartRuntimePrimitives.Assert(() => needsCompositing);
            layer ??= new ShaderMaskLayer();
            (
                (Func<ShaderMaskLayer>)(
                    () =>
                    {
                        var __cascade = layer!;
                        __cascade.shader = _shaderCallback(Offset.zero & size);
                        __cascade.maskRect = offset & size;
                        __cascade.blendMode = _blendMode;
                        return __cascade;
                    }
                )
            )();
            context.pushLayer(layer!, base.paint, offset);
            DartRuntimePrimitives.Assert(() =>
            {
                layer!.debugCreator = debugCreator;
                return true;
            });
        }
        else
        {
            layer = null;
        }
    }
}

public class RenderBackdropFilter : RenderProxyBox
{
    internal virtual bool _enabled { get; set; } = default!;
    internal virtual ImageFilterConfig _filterConfig { get; set; } = default!;
    internal virtual BlendMode _blendMode { get; set; } = default!;
    internal virtual BackdropKey? _backdropKey { get; set; } = default;

    public RenderBackdropFilter(
        RenderBox? child = null,
        ImageFilter? filter = null,
        ImageFilterConfig? filterConfig = null,
        BlendMode blendMode = BlendMode.srcOver,
        bool enabled = true,
        BackdropKey? backdropKey = null
    )
        : base(child)
    {
        _filterConfig = filterConfig ?? ImageFilterConfig.Create(filter!);
        _enabled = enabled;
        _blendMode = blendMode;
        _backdropKey = backdropKey;
        System.Diagnostics.Debug.Assert((filter is not null) || (filterConfig is not null));
        System.Diagnostics.Debug.Assert((filter is null) || (filterConfig is null));
    }

    public override BackdropFilterLayer? layer => ((BackdropFilterLayer?)base.layer)!;
    public virtual bool enabled
    {
        get => _enabled;
        set
        {
            var __value = value;
            if (enabled == (__value))
            {
                return;
            }
            _enabled = (__value);
            markNeedsPaint();
        }
    }
    public virtual ImageFilter filter
    {
        get
        {
            DartRuntimePrimitives.Assert(() => filterConfig.filter is not null);
            return filterConfig.filter!;
        }
        set
        {
            var __value = value;
            filterConfig = ImageFilterConfig.Create(__value);
        }
    }
    public virtual ImageFilterConfig filterConfig
    {
        get => _filterConfig;
        set
        {
            var __value = value;
            if (Equals(_filterConfig, __value))
            {
                return;
            }
            _filterConfig = __value;
            markNeedsPaint();
        }
    }
    public virtual BlendMode blendMode
    {
        get => _blendMode;
        set
        {
            var __value = value;
            if (Equals(_blendMode, (__value)))
            {
                return;
            }
            _blendMode = (__value);
            markNeedsPaint();
        }
    }
    public virtual BackdropKey? backdropKey
    {
        get => _backdropKey;
        set
        {
            var __value = value;
            if (Equals(__value, _backdropKey))
            {
                return;
            }
            _backdropKey = __value;
            markNeedsPaint();
        }
    }
    public override bool alwaysNeedsCompositing => child is not null;

    public override void paint(PaintingContext context, Offset offset)
    {
        if (!_enabled)
        {
            base.paint(context, offset);
            return;
        }
        ImageFilter effectiveFilter = _filterConfig.resolve(
            new ImageFilterContext(bounds: offset & size)
        );
        if (child is not null)
        {
            DartRuntimePrimitives.Assert(() => needsCompositing);
            layer ??= new BackdropFilterLayer();
            layer!.filter = effectiveFilter;
            layer!.blendMode = _blendMode;
            layer!.backdropKey = _backdropKey;
            context.pushLayer(layer!, base.paint, offset);
            DartRuntimePrimitives.Assert(() =>
            {
                layer!.debugCreator = debugCreator;
                return true;
            });
        }
        else
        {
            layer = null;
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<ImageFilterConfig>(
                "filterConfig",
                filterConfig,
                defaultValue: null
            )
        );
        properties.add(new EnumProperty<BlendMode>("blendMode", blendMode));
        properties.add(new FlagProperty("enabled", value: enabled, ifTrue: "enabled"));
    }
}

public abstract class CustomClipper<T> : Listenable
{
    internal virtual Listenable? _reclip { get; private set; }

    protected CustomClipper(Listenable? reclip = null)
    {
        _reclip = reclip;
    }

    public virtual void addListener(Action listener) => _reclip?.addListener(listener);

    public virtual void removeListener(Action listener) => _reclip?.removeListener(listener);

    public abstract T getClip(Size size);

    public virtual Rect getApproximateClipRect(Size size) => Offset.zero & size;

    public abstract bool shouldReclip(CustomClipper<T> oldClipper);

    public override string ToString() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "CustomClipper");
}

public class ShapeBorderClipper : CustomClipper<Path>
{
    public virtual ShapeBorder shape { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }

    public ShapeBorderClipper(ShapeBorder shape, TextDirection? textDirection = null)
    {
        this.shape = shape;
        this.textDirection = textDirection;
    }

    public override Path getClip(Size size)
    {
        return shape.getOuterPath(Offset.zero & size, textDirection: textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldReclip(CustomClipper<Path> oldClipper)
    {
        if (!Equals(DartRuntimePrimitives.RuntimeType(oldClipper), typeof(ShapeBorderClipper)))
        {
            return true;
        }
        var typedOldClipper = ((ShapeBorderClipper?)oldClipper)!;
        return (!Equals(typedOldClipper.shape, shape))
            || (!Equals(typedOldClipper.textDirection, textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class _RenderCustomClip__proxy_box<T> : RenderProxyBox
{
    internal virtual CustomClipper<T>? _clipper { get; set; } = default;
    internal virtual T? _clip { get; set; } = default;

    // `T?` on an unconstrained C# generic is only a nullable annotation. When T is
    // Rect or RRect, default(T) is an empty value rather than Dart's null sentinel.
    // Track validity explicitly so the first paint computes the actual clip.
    internal virtual bool _clipIsValid { get; set; }
    internal virtual Clip _clipBehavior { get; set; } = default!;
    internal virtual Paint? _debugPaint { get; set; } = default;
    internal virtual TextPainter? _debugText { get; set; } = default;

    internal _RenderCustomClip__proxy_box(
        RenderBox? child = null,
        CustomClipper<T>? clipper = null,
        Clip clipBehavior = Clip.antiAlias
    )
        : base(child)
    {
        _clipper = clipper;
        _clipBehavior = clipBehavior;
    }

    public virtual CustomClipper<T>? clipper
    {
        get => _clipper;
        set
        {
            var newClipper = value;
            if (Equals(_clipper, newClipper))
            {
                return;
            }
            CustomClipper<T>? oldClipper = _clipper;
            _clipper = newClipper;
            DartRuntimePrimitives.Assert(() =>
                (newClipper is not null) || (oldClipper is not null)
            );
            if (
                (newClipper is null)
                || (oldClipper is null)
                || (
                    !Equals(
                        DartRuntimePrimitives.RuntimeType(newClipper),
                        DartRuntimePrimitives.RuntimeType(oldClipper)
                    )
                )
                || newClipper.shouldReclip(oldClipper)
            )
            {
                _markNeedsClip();
            }
            if (attached)
            {
                oldClipper?.removeListener(_markNeedsClip);
                newClipper?.addListener(_markNeedsClip);
            }
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _clipper?.addListener(_markNeedsClip);
    }

    public override void detach()
    {
        _clipper?.removeListener(_markNeedsClip);
        base.detach();
    }

    internal virtual void _markNeedsClip()
    {
        _clip = default;
        _clipIsValid = false;
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    internal abstract T _defaultClip { get; }
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
            }
        }
    }

    public override void performLayout()
    {
        Size? oldSize = hasSize ? size : null;
        base.performLayout();
        if (!Equals(oldSize, size))
        {
            _clip = default;
            _clipIsValid = false;
        }
    }

    internal virtual void _updateClip()
    {
        if (!_clipIsValid)
        {
            if (_clipper is null)
            {
                _clip = _defaultClip;
            }
            else
            {
                _clip = _clipper.getClip(size);
            }
            _clipIsValid = true;
        }
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
                return _clipper?.getApproximateClipRect(size) ?? (Offset.zero & size);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPaint ??= (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.shader = Ui.Gradient.linear(
                            Offset.zero,
                            new Offset(10.0, 10.0),
                            new List<Color>
                            {
                                new Color(0L),
                                new Color(4294902015L),
                                new Color(4294902015L),
                                new Color(0L),
                            },
                            new List<double> { 0.25, 0.25, 0.75, 0.75 },
                            TileMode.repeated
                        );
                        __cascade.strokeWidth = 2.0;
                        __cascade.style = PaintingStyle.stroke;
                        return __cascade;
                    }
                )
            )();
            _debugText ??= (
                (Func<TextPainter>)(
                    () =>
                    {
                        var __cascade = new TextPainter(
                            text: new TextSpan(
                                text: "✂",
                                style: new Painting.TextStyle(
                                    color: new Color(4294902015L),
                                    fontSize: 14.0
                                )
                            ),
                            textDirection: TextDirection.rtl
                        );
                        __cascade.layout();
                        return __cascade;
                    }
                )
            )();
            return true;
        });
    }

    public override void dispose()
    {
        _debugText?.dispose();
        _debugText = null;
        base.dispose();
    }
}

public class RenderClipRect : _RenderCustomClip__proxy_box<Rect>
{
    public RenderClipRect(
        RenderBox? child = null,
        CustomClipper<Rect>? clipper = null,
        Clip clipBehavior = Clip.antiAlias
    )
        : base(child: child, clipper: clipper, clipBehavior: clipBehavior) { }

    internal override Rect _defaultClip => Offset.zero & size;

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (_clipper is not null)
        {
            _updateClip();
            DartRuntimePrimitives.Assert(() => !Equals(_clip, null));
            if (!(_clip).contains(position))
            {
                return false;
            }
        }
        return base.hitTest(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            if (!Equals(clipBehavior, Clip.none))
            {
                _updateClip();
                layer = context.pushClipRect(
                    needsCompositing,
                    offset,
                    (_clip),
                    base.paint,
                    clipBehavior: clipBehavior,
                    oldLayer: ((ClipRectLayer?)layer)!
                );
            }
            else
            {
                context.paintChild(child!, offset);
                layer = null;
            }
        }
        else
        {
            layer = null;
        }
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (child is not null)
            {
                base.debugPaintSize(context, offset);
                if (!Equals(clipBehavior, Clip.none))
                {
                    context.canvas.drawRect((_clip).shift(offset), _debugPaint!);
                    _debugText!.paint(
                        context.canvas,
                        offset
                            + new Offset(
                                (_clip).width / 8.0,
                                -(
                                    _debugText!.text!.style!.fontSize
                                    ?? throw new global::System.NullReferenceException(
                                        "Dart null assertion failed."
                                    )
                                ) * 1.1
                            )
                    );
                }
            }
            return true;
        });
    }
}

public class RenderClipRRect : _RenderCustomClip__proxy_box<RRect>
{
    internal virtual BorderRadiusGeometry _borderRadius { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;

    public RenderClipRRect(
        RenderBox? child = null,
        BorderRadiusGeometry borderRadius = default!,
        CustomClipper<RRect>? clipper = null,
        Clip clipBehavior = Clip.antiAlias,
        TextDirection? textDirection = null
    )
        : base(child: child, clipper: clipper, clipBehavior: clipBehavior)
    {
        BorderRadiusGeometry __borderRadius = borderRadius ?? BorderRadius.zero;
        _borderRadius = __borderRadius;
        _textDirection = textDirection;
    }

    public virtual BorderRadiusGeometry borderRadius
    {
        get => _borderRadius;
        set
        {
            var __value = value;
            if (Equals(_borderRadius, __value))
            {
                return;
            }
            _borderRadius = __value;
            _markNeedsClip();
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
            _markNeedsClip();
        }
    }
    internal override RRect _defaultClip =>
        _borderRadius.resolve(textDirection).toRRect(Offset.zero & size);

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (_clipper is not null)
        {
            _updateClip();
            DartRuntimePrimitives.Assert(() => !Equals(_clip, null));
            if (!_clip!.contains(position))
            {
                return false;
            }
        }
        return base.hitTest(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            if (!Equals(clipBehavior, Clip.none))
            {
                _updateClip();
                layer = context.pushClipRRect(
                    needsCompositing,
                    offset,
                    _clip!.outerRect,
                    _clip!,
                    base.paint,
                    clipBehavior: clipBehavior,
                    oldLayer: ((ClipRRectLayer?)layer)!
                );
            }
            else
            {
                context.paintChild(child!, offset);
                layer = null;
            }
        }
        else
        {
            layer = null;
        }
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (child is not null)
            {
                base.debugPaintSize(context, offset);
                if (!Equals(clipBehavior, Clip.none))
                {
                    context.canvas.drawRRect(_clip!.shift(offset), _debugPaint!);
                    _debugText!.paint(
                        context.canvas,
                        offset
                            + new Offset(
                                _clip!.tlRadiusX,
                                -(
                                    _debugText!.text!.style!.fontSize
                                    ?? throw new global::System.NullReferenceException(
                                        "Dart null assertion failed."
                                    )
                                ) * 1.1
                            )
                    );
                }
            }
            return true;
        });
    }
}

public class RenderClipRSuperellipse : _RenderCustomClip__proxy_box<RSuperellipse>
{
    internal virtual BorderRadiusGeometry _borderRadius { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;

    public RenderClipRSuperellipse(
        RenderBox? child = null,
        BorderRadiusGeometry borderRadius = default!,
        CustomClipper<RSuperellipse>? clipper = null,
        Clip clipBehavior = Clip.antiAlias,
        TextDirection? textDirection = null
    )
        : base(child: child, clipper: clipper, clipBehavior: clipBehavior)
    {
        BorderRadiusGeometry __borderRadius = borderRadius ?? BorderRadius.zero;
        _borderRadius = __borderRadius;
        _textDirection = textDirection;
    }

    public virtual BorderRadiusGeometry borderRadius
    {
        get => _borderRadius;
        set
        {
            var __value = value;
            if (Equals(_borderRadius, __value))
            {
                return;
            }
            _borderRadius = __value;
            _markNeedsClip();
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
            _markNeedsClip();
        }
    }
    internal override RSuperellipse _defaultClip =>
        _borderRadius.resolve(textDirection).toRSuperellipse(Offset.zero & size);

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (_clipper is not null)
        {
            _updateClip();
            DartRuntimePrimitives.Assert(() => !Equals(_clip, null));
            if (!_clip!.outerRect.contains(position))
            {
                return false;
            }
        }
        return base.hitTest(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            if (!Equals(clipBehavior, Clip.none))
            {
                _updateClip();
                layer = context.pushClipRSuperellipse(
                    needsCompositing,
                    offset,
                    _clip!.outerRect,
                    _clip!,
                    base.paint,
                    clipBehavior: clipBehavior,
                    oldLayer: ((ClipRSuperellipseLayer?)layer)!
                );
            }
            else
            {
                context.paintChild(child!, offset);
                layer = null;
            }
        }
        else
        {
            layer = null;
        }
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (child is not null)
            {
                base.debugPaintSize(context, offset);
                if (!Equals(clipBehavior, Clip.none))
                {
                    context.canvas.drawRSuperellipse(_clip!.shift(offset), _debugPaint!);
                    _debugText!.paint(
                        context.canvas,
                        offset
                            + new Offset(
                                _clip!.tlRadiusX,
                                -(
                                    _debugText!.text!.style!.fontSize
                                    ?? throw new global::System.NullReferenceException(
                                        "Dart null assertion failed."
                                    )
                                ) * 1.1
                            )
                    );
                }
            }
            return true;
        });
    }
}

public class RenderClipOval : _RenderCustomClip__proxy_box<Rect>
{
    internal virtual Rect? _cachedRect { get; set; } = default;
    internal virtual Path _cachedPath { get; set; } = default!;

    public RenderClipOval(
        RenderBox? child = null,
        CustomClipper<Rect>? clipper = null,
        Clip clipBehavior = Clip.antiAlias
    )
        : base(child: child, clipper: clipper, clipBehavior: clipBehavior) { }

    internal virtual Path _getClipPath(Rect rect)
    {
        if (!Equals(rect, _cachedRect))
        {
            _cachedRect = rect;
            _cachedPath = (
                (Func<Path>)(
                    () =>
                    {
                        var __cascade = new Path();
                        __cascade.addOval(
                            (
                                _cachedRect
                                ?? throw new global::System.NullReferenceException(
                                    "Dart null assertion failed."
                                )
                            )
                        );
                        return __cascade;
                    }
                )
            )();
        }
        return _cachedPath;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Rect _defaultClip => Offset.zero & size;

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        _updateClip();
        DartRuntimePrimitives.Assert(() => !Equals(_clip, null));
        Offset centerLocal = (_clip).center;
        var offset = new Offset(
            (position.dx - centerLocal.dx) / (_clip).width,
            (position.dy - centerLocal.dy) / (_clip).height
        );
        if (offset.distanceSquared > 0.25)
        {
            return false;
        }
        return base.hitTest(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            if (!Equals(clipBehavior, Clip.none))
            {
                _updateClip();
                layer = context.pushClipPath(
                    needsCompositing,
                    offset,
                    (_clip),
                    _getClipPath((_clip)),
                    base.paint,
                    clipBehavior: clipBehavior,
                    oldLayer: ((ClipPathLayer?)layer)!
                );
            }
            else
            {
                context.paintChild(child!, offset);
                layer = null;
            }
        }
        else
        {
            layer = null;
        }
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (child is not null)
            {
                base.debugPaintSize(context, offset);
                if (!Equals(clipBehavior, Clip.none))
                {
                    context.canvas.drawPath(_getClipPath((_clip)).shift(offset), _debugPaint!);
                    _debugText!.paint(
                        context.canvas,
                        offset
                            + new Offset(
                                ((_clip).width - _debugText!.width) / 2.0,
                                -(
                                    _debugText!.text!.style!.fontSize
                                    ?? throw new global::System.NullReferenceException(
                                        "Dart null assertion failed."
                                    )
                                ) * 1.1
                            )
                    );
                }
            }
            return true;
        });
    }
}

public class RenderClipPath : _RenderCustomClip__proxy_box<Path>
{
    public RenderClipPath(
        RenderBox? child = null,
        CustomClipper<Path>? clipper = null,
        Clip clipBehavior = Clip.antiAlias
    )
        : base(child: child, clipper: clipper, clipBehavior: clipBehavior) { }

    internal override Path _defaultClip =>
        (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRect(Offset.zero & size);
                    return __cascade;
                }
            )
        )();

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (_clipper is not null)
        {
            _updateClip();
            DartRuntimePrimitives.Assert(() => !Equals(_clip, null));
            if (!_clip!.contains(position))
            {
                return false;
            }
        }
        return base.hitTest(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            if (!Equals(clipBehavior, Clip.none))
            {
                _updateClip();
                layer = context.pushClipPath(
                    needsCompositing,
                    offset,
                    Offset.zero & size,
                    _clip!,
                    base.paint,
                    clipBehavior: clipBehavior,
                    oldLayer: ((ClipPathLayer?)layer)!
                );
            }
            else
            {
                context.paintChild(child!, offset);
                layer = null;
            }
        }
        else
        {
            layer = null;
        }
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (child is not null)
            {
                base.debugPaintSize(context, offset);
                if (!Equals(clipBehavior, Clip.none))
                {
                    context.canvas.drawPath(_clip!.shift(offset), _debugPaint!);
                    _debugText!.paint(context.canvas, offset);
                }
            }
            return true;
        });
    }
}

public abstract class _RenderPhysicalModelBase__proxy_box<T> : _RenderCustomClip__proxy_box<T>
{
    internal virtual double _elevation { get; set; } = default!;
    internal virtual Color _shadowColor { get; set; } = default!;
    internal virtual Color _color { get; set; } = default!;

    internal _RenderPhysicalModelBase__proxy_box(
        RenderBox? child,
        double elevation,
        Color color,
        Color shadowColor,
        Clip clipBehavior = Clip.none,
        CustomClipper<T>? clipper = null
    )
        : base(child: child, clipBehavior: clipBehavior, clipper: clipper)
    {
        _elevation = elevation;
        _color = color;
        _shadowColor = shadowColor;
        System.Diagnostics.Debug.Assert(elevation >= 0.0);
    }

    public virtual double elevation
    {
        get => _elevation;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value >= 0.0);
            if (elevation == __value)
            {
                return;
            }
            bool didNeedCompositing = alwaysNeedsCompositing;
            _elevation = __value;
            if (didNeedCompositing != alwaysNeedsCompositing)
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsPaint();
        }
    }
    public virtual Color shadowColor
    {
        get => _shadowColor;
        set
        {
            var __value = value;
            if (Equals(shadowColor, __value))
            {
                return;
            }
            _shadowColor = __value;
            markNeedsPaint();
        }
    }
    public virtual Color color
    {
        get => _color;
        set
        {
            var __value = value;
            if (Equals(color, __value))
            {
                return;
            }
            _color = __value;
            markNeedsPaint();
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("elevation", elevation));
        properties.add(new ColorProperty("color", color));
        properties.add(new ColorProperty("shadowColor", color));
    }
}

public class RenderPhysicalModel : _RenderPhysicalModelBase__proxy_box<RRect>
{
    internal virtual BoxShape _shape { get; set; } = default!;
    internal virtual BorderRadius? _borderRadius { get; set; } = default;

    public RenderPhysicalModel(
        RenderBox? child = null,
        BoxShape shape = BoxShape.rectangle,
        Clip clipBehavior = Clip.none,
        BorderRadius? borderRadius = null,
        double elevation = 0.0,
        Color color = default!,
        Color shadowColor = default!
    )
        : base(
            child: child,
            clipBehavior: clipBehavior,
            elevation: elevation,
            color: color,
            shadowColor: shadowColor ?? new Color(0xFF000000)
        )
    {
        _shape = shape;
        _borderRadius = borderRadius;
        System.Diagnostics.Debug.Assert(elevation >= 0.0);
    }

    public virtual BoxShape shape
    {
        get => _shape;
        set
        {
            var __value = value;
            if (Equals(shape, (__value)))
            {
                return;
            }
            _shape = (__value);
            _markNeedsClip();
        }
    }
    public virtual BorderRadius? borderRadius
    {
        get => _borderRadius;
        set
        {
            var __value = value;
            if (Equals(borderRadius, __value))
            {
                return;
            }
            _borderRadius = __value;
            _markNeedsClip();
        }
    }
    internal override RRect _defaultClip
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            Rect rect = Offset.zero & size;
            return _shape switch
            {
                BoxShape.rectangle => (borderRadius ?? BorderRadius.zero).toRRect(rect),
                BoxShape.circle => RRect.fromRectXY(rect, rect.width / 2L, rect.height / 2L),
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
        }
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (_clipper is not null)
        {
            _updateClip();
            DartRuntimePrimitives.Assert(() => !Equals(_clip, null));
            if (!_clip!.contains(position))
            {
                return false;
            }
        }
        return base.hitTest(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is null)
        {
            layer = null;
            return;
        }
        _updateClip();
        RRect offsetRRect = _clip!.shift(offset);
        var paintShadows = true;
        DartRuntimePrimitives.Assert(() =>
        {
            if (Painting.DebugLibrary.debugDisableShadows)
            {
                if (elevation > 0.0)
                {
                    context.canvas.drawRRect(
                        offsetRRect,
                        (
                            (Func<Paint>)(
                                () =>
                                {
                                    var __cascade = new Paint();
                                    __cascade.color = shadowColor;
                                    __cascade.style = PaintingStyle.stroke;
                                    __cascade.strokeWidth = elevation * 2.0;
                                    return __cascade;
                                }
                            )
                        )()
                    );
                }
                paintShadows = false;
            }
            return true;
        });
        Canvas canvasLocal = context.canvas;
        if ((elevation != 0.0) && paintShadows)
        {
            var offsetRRectAsPath = (
                (Func<Path>)(
                    () =>
                    {
                        var __cascade = new Path();
                        __cascade.addRRect(offsetRRect);
                        return __cascade;
                    }
                )
            )();
            canvasLocal.drawShadow(offsetRRectAsPath, shadowColor, elevation, color.alpha != 255L);
        }
        var usesSaveLayer = Equals(clipBehavior, Clip.antiAliasWithSaveLayer);
        if (!usesSaveLayer)
        {
            canvasLocal.drawRRect(
                offsetRRect,
                (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.color = color;
                            return __cascade;
                        }
                    )
                )()
            );
        }
        layer = context.pushClipRRect(
            needsCompositing,
            offset,
            Offset.zero & size,
            _clip!,
            (context, offset) =>
            {
                if (usesSaveLayer)
                {
                    context.canvas.drawPaint(
                        (
                            (Func<Paint>)(
                                () =>
                                {
                                    var __cascade = new Paint();
                                    __cascade.color = color;
                                    return __cascade;
                                }
                            )
                        )()
                    );
                }
                base.paint(context, offset);
            },
            oldLayer: ((ClipRRectLayer?)layer)!,
            clipBehavior: clipBehavior
        );
        DartRuntimePrimitives.Assert(() =>
        {
            layer?.debugCreator = debugCreator;
            return true;
        });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<BoxShape>("shape", shape));
        properties.add(new DiagnosticsProperty<BorderRadius>("borderRadius", borderRadius));
    }
}

public class RenderPhysicalShape : _RenderPhysicalModelBase__proxy_box<Path>
{
    public RenderPhysicalShape(
        RenderBox? child = null,
        CustomClipper<Path> clipper = default!,
        Clip clipBehavior = Clip.none,
        double elevation = 0.0,
        Color color = default!,
        Color shadowColor = default!
    )
        : base(
            child: child,
            clipper: clipper,
            clipBehavior: clipBehavior,
            elevation: elevation,
            color: color,
            shadowColor: shadowColor ?? new Color(0xFF000000)
        )
    {
        System.Diagnostics.Debug.Assert(elevation >= 0.0);
    }

    internal override Path _defaultClip =>
        (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRect(Offset.zero & size);
                    return __cascade;
                }
            )
        )();

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (_clipper is not null)
        {
            _updateClip();
            DartRuntimePrimitives.Assert(() => !Equals(_clip, null));
            if (!_clip!.contains(position))
            {
                return false;
            }
        }
        return base.hitTest(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is null)
        {
            layer = null;
            return;
        }
        _updateClip();
        Path offsetPath = _clip!.shift(offset);
        var paintShadows = true;
        DartRuntimePrimitives.Assert(() =>
        {
            if (Painting.DebugLibrary.debugDisableShadows)
            {
                if (elevation > 0.0)
                {
                    context.canvas.drawPath(
                        offsetPath,
                        (
                            (Func<Paint>)(
                                () =>
                                {
                                    var __cascade = new Paint();
                                    __cascade.color = shadowColor;
                                    __cascade.style = PaintingStyle.stroke;
                                    __cascade.strokeWidth = elevation * 2.0;
                                    return __cascade;
                                }
                            )
                        )()
                    );
                }
                paintShadows = false;
            }
            return true;
        });
        Canvas canvasLocal = context.canvas;
        if ((elevation != 0.0) && paintShadows)
        {
            canvasLocal.drawShadow(offsetPath, shadowColor, elevation, color.alpha != 255L);
        }
        var usesSaveLayer = Equals(clipBehavior, Clip.antiAliasWithSaveLayer);
        if (!usesSaveLayer)
        {
            canvasLocal.drawPath(
                offsetPath,
                (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.color = color;
                            return __cascade;
                        }
                    )
                )()
            );
        }
        layer = context.pushClipPath(
            needsCompositing,
            offset,
            Offset.zero & size,
            _clip!,
            (context, offset) =>
            {
                if (usesSaveLayer)
                {
                    context.canvas.drawPaint(
                        (
                            (Func<Paint>)(
                                () =>
                                {
                                    var __cascade = new Paint();
                                    __cascade.color = color;
                                    return __cascade;
                                }
                            )
                        )()
                    );
                }
                base.paint(context, offset);
            },
            oldLayer: ((ClipPathLayer?)layer)!,
            clipBehavior: clipBehavior
        );
        DartRuntimePrimitives.Assert(() =>
        {
            layer?.debugCreator = debugCreator;
            return true;
        });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<CustomClipper<Path>>("clipper", clipper));
    }
}

public enum DecorationPosition
{
    background,
    foreground,
}

public class RenderDecoratedBox : RenderProxyBox
{
    internal virtual BoxPainter? _painter { get; set; } = default;
    internal virtual Decoration _decoration { get; set; } = default!;
    internal virtual DecorationPosition _position { get; set; } = default!;
    internal virtual ImageConfiguration _configuration { get; set; } = default!;

    public RenderDecoratedBox(
        Decoration decoration,
        DecorationPosition position = DecorationPosition.background,
        ImageConfiguration configuration = default!,
        RenderBox? child = null
    )
        : base(child)
    {
        ImageConfiguration __configuration = configuration ?? ImageConfiguration.empty;
        _decoration = decoration;
        _position = position;
        _configuration = __configuration;
    }

    public virtual Decoration decoration
    {
        get => _decoration;
        set
        {
            var __value = value;
            if (Equals(__value, _decoration))
            {
                return;
            }
            _painter?.dispose();
            _painter = null;
            _decoration = __value;
            markNeedsPaint();
        }
    }
    public virtual DecorationPosition position
    {
        get => _position;
        set
        {
            var __value = value;
            if (Equals(__value, _position))
            {
                return;
            }
            _position = __value;
            markNeedsPaint();
        }
    }
    public virtual ImageConfiguration configuration
    {
        get => _configuration;
        set
        {
            var __value = value;
            if (Equals(__value, _configuration))
            {
                return;
            }
            _configuration = __value;
            markNeedsPaint();
        }
    }

    public override void detach()
    {
        _painter?.dispose();
        _painter = null;
        base.detach();
        markNeedsPaint();
    }

    public override void dispose()
    {
        _painter?.dispose();
        base.dispose();
    }

    public override bool hitTestSelf(Offset position)
    {
        return _decoration.hitTest(size, position, textDirection: configuration.textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        _painter ??= _decoration.createBoxPainter(markNeedsPaint);
        ImageConfiguration filledConfiguration = configuration.copyWith(size: size);
        if (Equals(position, DecorationPosition.background))
        {
            long? debugSaveCount = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                debugSaveCount = context.canvas.getSaveCount();
                return true;
            });
            _painter!.paint(context.canvas, offset, filledConfiguration);
            DartRuntimePrimitives.Assert(() =>
            {
                if (debugSaveCount != context.canvas.getSaveCount())
                {
                    throw new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                $"{DartRuntimePrimitives.RuntimeType(_decoration)} painter had mismatching save and restore calls."
                            ),
                            new ErrorDescription(
                                $"Before painting the decoration, the canvas save count was {debugSaveCount}. "
                                    + $"After painting it, the canvas save count was {context.canvas.getSaveCount()}. "
                                    + "Every call to save() or saveLayer() must be matched by a call to restore()."
                            ),
                            new DiagnosticsProperty<Decoration>(
                                "The decoration was",
                                decoration,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                            new DiagnosticsProperty<BoxPainter>(
                                "The painter was",
                                _painter,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        }
                    );
                }
                return true;
            });
            if (decoration.isComplex)
            {
                context.setIsComplexHint();
            }
        }
        base.paint(context, offset);
        if (Equals(position, DecorationPosition.foreground))
        {
            _painter!.paint(context.canvas, offset, filledConfiguration);
            if (decoration.isComplex)
            {
                context.setIsComplexHint();
            }
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(((Diagnosticable)_decoration).toDiagnosticsNode(name: "decoration"));
        properties.add(new DiagnosticsProperty<ImageConfiguration>("configuration", configuration));
    }
}

public class RenderTransform : RenderProxyBox
{
    internal virtual Offset? _origin { get; set; } = default;
    internal virtual AlignmentGeometry? _alignment { get; set; } = default;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    public virtual bool transformHitTests { get; set; } = default!;
    internal virtual Matrix4? _transform { get; set; } = default;
    internal virtual FilterQuality? _filterQuality { get; set; } = default;

    public RenderTransform(
        Matrix4 transform,
        Offset? origin = null,
        AlignmentGeometry? alignment = null,
        TextDirection? textDirection = null,
        bool transformHitTests = true,
        FilterQuality? filterQuality = null,
        RenderBox? child = null
    )
        : base(child)
    {
        _transform = Matrix4.copy(transform);
        _origin = origin;
        _alignment = alignment;
        _textDirection = textDirection;
        this.transformHitTests = transformHitTests;
        _filterQuality = filterQuality;
    }

    public virtual Offset? origin
    {
        get => _origin;
        set
        {
            var __value = value;
            if (Equals(_origin, __value))
            {
                return;
            }
            _origin = __value;
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual AlignmentGeometry? alignment
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
            markNeedsPaint();
            markNeedsSemanticsUpdate();
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
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }
    public override bool alwaysNeedsCompositing =>
        (child is not null) && (_filterQuality is not null);
    public virtual Matrix4 transform
    {
        set
        {
            var __value = value;
            if (Equals(_transform, __value))
            {
                return;
            }
            _transform = Matrix4.copy(__value);
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual FilterQuality? filterQuality
    {
        get => _filterQuality;
        set
        {
            var __value = value;
            if (Equals(_filterQuality, __value))
            {
                return;
            }
            bool didNeedCompositing = alwaysNeedsCompositing;
            _filterQuality = __value;
            if (didNeedCompositing != alwaysNeedsCompositing)
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsPaint();
        }
    }

    public virtual void setIdentity()
    {
        _transform!.setIdentity();
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    public virtual void rotateX(double radians)
    {
        _transform!.rotateX(radians);
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    public virtual void rotateY(double radians)
    {
        _transform!.rotateY(radians);
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    public virtual void rotateZ(double radians)
    {
        _transform!.rotateZ(radians);
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    public virtual void translate(double x, double y = 0.0, double z = 0.0)
    {
        _transform!.translateByDouble(x, ((y)), ((z)), 1);
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    public virtual void scale(double x, double? y = null, double? z = null)
    {
        _transform!.scaleByDouble(x, y ?? x, z ?? x, 1);
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    internal virtual Matrix4? _effectiveTransform
    {
        get
        {
            Alignment? resolvedAlignment = alignment?.resolve(textDirection);
            if ((_origin is null) && (resolvedAlignment is null))
            {
                return _transform;
            }
            var result = Matrix4.identity();
            if (_origin is not null)
            {
                result.translateByDouble(
                    (
                        _origin
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dx,
                    (
                        _origin
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dy,
                    0,
                    1
                );
            }
            Offset? translation = default!;
            if (resolvedAlignment is not null)
            {
                translation = resolvedAlignment.alongSize(size);
                result.translateByDouble(
                    (
                        translation
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dx,
                    (
                        translation
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dy,
                    0,
                    1
                );
            }
            result.multiply(_transform!);
            if (resolvedAlignment is not null)
            {
                result.translateByDouble(
                    -(
                        translation
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dx,
                    -(
                        translation
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dy,
                    0,
                    1
                );
            }
            if (_origin is not null)
            {
                result.translateByDouble(
                    -(
                        _origin
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dx,
                    -(
                        _origin
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dy,
                    0,
                    1
                );
            }
            return result;
        }
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        return hitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        DartRuntimePrimitives.Assert(() => !transformHitTests || (_effectiveTransform is not null));
        return result.addWithPaintTransform(
            transform: transformHitTests ? _effectiveTransform : null,
            position: position,
            hitTest: (result, position) =>
            {
                return base.hitTestChildren(result, position: position);
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            Matrix4 transform = _effectiveTransform!;
            if (filterQuality is null)
            {
                Offset? childOffset = MatrixUtils.getAsTranslation(transform);
                if (childOffset is null)
                {
                    double det = transform.determinant;
                    if ((det == 0L) || !double.IsFinite(det))
                    {
                        layer = null;
                        return;
                    }
                    layer = context.pushTransform(
                        needsCompositing,
                        offset,
                        transform,
                        base.paint,
                        oldLayer: (layer is TransformLayer) ? ((TransformLayer?)layer)! : null
                    );
                }
                else
                {
                    base.paint(
                        context,
                        offset
                            + (
                                childOffset
                                ?? throw new global::System.NullReferenceException(
                                    "Dart null assertion failed."
                                )
                            )
                    );
                    layer = null;
                }
            }
            else
            {
                var effectiveTransform = (
                    (Func<Matrix4>)(
                        () =>
                        {
                            var __cascade = Matrix4.translationValues(offset.dx, offset.dy, 0.0);
                            __cascade.multiply(transform);
                            __cascade.translateByDouble(-offset.dx, -offset.dy, 0, 1);
                            return __cascade;
                        }
                    )
                )();
                var filter = new ImageFilter(
                    effectiveTransform.storage,
                    filterQuality: (
                        filterQuality
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                );
                if (layer is ImageFilterLayer filterLayer)
                {
                    filterLayer.imageFilter = filter;
                }
                else
                {
                    layer = new ImageFilterLayer(imageFilter: filter);
                }
                context.pushLayer(layer!, base.paint, offset);
                DartRuntimePrimitives.Assert(() =>
                {
                    layer!.debugCreator = debugCreator;
                    return true;
                });
            }
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)child;
        transform.multiply(_effectiveTransform!);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new TransformProperty("transform matrix", _transform));
        properties.add(new DiagnosticsProperty<Offset>("origin", origin));
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
        properties.add(
            new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null)
        );
        properties.add(new DiagnosticsProperty<bool>("transformHitTests", transformHitTests));
    }
}

public class RenderFittedBox : RenderProxyBox
{
    internal virtual Alignment? _resolvedAlignment { get; set; } = default;
    internal virtual BoxFit _fit { get; set; } = default!;
    internal virtual AlignmentGeometry _alignment { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual bool? _hasVisualOverflow { get; set; } = default;
    internal virtual Matrix4? _transform { get; set; } = default;
    internal virtual Clip _clipBehavior { get; set; } = Clip.none;

    public RenderFittedBox(
        BoxFit fit = BoxFit.contain,
        AlignmentGeometry alignment = default!,
        TextDirection? textDirection = null,
        RenderBox? child = null,
        Clip clipBehavior = Clip.none
    )
        : base(child)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        _fit = fit;
        _alignment = __alignment;
        _textDirection = textDirection;
        _clipBehavior = clipBehavior;
    }

    internal virtual Alignment _resolve() =>
        _resolvedAlignment ??= alignment.resolve(textDirection);

    internal virtual void _markNeedResolution()
    {
        _resolvedAlignment = null;
        markNeedsPaint();
    }

    internal virtual bool _fitAffectsLayout(BoxFit fit)
    {
        switch (fit)
        {
            case BoxFit.scaleDown:
            {
                return true;
            }
            case BoxFit.contain:
            case BoxFit.cover:
            case BoxFit.fill:
            case BoxFit.fitHeight:
            case BoxFit.fitWidth:
            case BoxFit.none:
            {
                return false;
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxFit fit
    {
        get => _fit;
        set
        {
            var __value = value;
            if (Equals(_fit, (__value)))
            {
                return;
            }
            BoxFit lastFit = _fit;
            _fit = (__value);
            if (_fitAffectsLayout(lastFit) || _fitAffectsLayout(((__value))))
            {
                markNeedsLayout();
            }
            else
            {
                _clearPaintData();
                markNeedsPaint();
            }
        }
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
            _clearPaintData();
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
            _clearPaintData();
            _markNeedResolution();
        }
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if (child is not null)
        {
            Size childSize = child!.getDryLayout(new BoxConstraints());
            switch (fit)
            {
                case BoxFit.scaleDown:
                {
                    BoxConstraints sizeConstraints = constraints.loosen();
                    Size unconstrainedSize =
                        sizeConstraints.constrainSizeAndAttemptToPreserveAspectRatio(childSize);
                    return constraints.constrain(unconstrainedSize);
                }
                case BoxFit.contain:
                case BoxFit.cover:
                case BoxFit.fill:
                case BoxFit.fitHeight:
                case BoxFit.fitWidth:
                case BoxFit.none:
                {
                    return constraints.constrainSizeAndAttemptToPreserveAspectRatio(childSize);
                }
            }
        }
        else
        {
            return constraints.smallest;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        return child?.getDryBaseline(new BoxConstraints(), baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        if (child is not null)
        {
            child!.layout(new BoxConstraints(), parentUsesSize: true);
            switch (fit)
            {
                case BoxFit.scaleDown:
                {
                    BoxConstraints sizeConstraints = constraints.loosen();
                    Size unconstrainedSize =
                        sizeConstraints.constrainSizeAndAttemptToPreserveAspectRatio(child!.size);
                    size = constraints.constrain(unconstrainedSize);
                    break;
                }
                case BoxFit.contain:
                case BoxFit.cover:
                case BoxFit.fill:
                case BoxFit.fitHeight:
                case BoxFit.fitWidth:
                case BoxFit.none:
                {
                    size = constraints.constrainSizeAndAttemptToPreserveAspectRatio(child!.size);
                    break;
                }
            }
            _clearPaintData();
        }
        else
        {
            size = constraints.smallest;
        }
    }

    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            if (!Equals((__value), _clipBehavior))
            {
                _clipBehavior = (__value);
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }

    internal virtual void _clearPaintData()
    {
        _hasVisualOverflow = null;
        _transform = null;
    }

    internal virtual void _updatePaintData()
    {
        if (_transform is not null)
        {
            return;
        }
        if (child is null)
        {
            _hasVisualOverflow = false;
            _transform = Matrix4.identity();
        }
        else
        {
            Alignment resolvedAlignment = _resolve();
            Size childSize = child!.size;
            FittedSizes sizes = Box_fitLibrary.applyBoxFit(_fit, childSize, size);
            double scaleX = sizes.destination.width / sizes.source.width;
            double scaleY = sizes.destination.height / sizes.source.height;
            Rect sourceRect = resolvedAlignment.inscribe(sizes.source, Offset.zero & childSize);
            Rect destinationRect = resolvedAlignment.inscribe(
                sizes.destination,
                Offset.zero & size
            );
            _hasVisualOverflow =
                (sourceRect.width < childSize.width) || (sourceRect.height < childSize.height);
            DartRuntimePrimitives.Assert(() => double.IsFinite(scaleX) && double.IsFinite(scaleY));
            _transform = (
                (Func<Matrix4>)(
                    () =>
                    {
                        var __cascade = Matrix4.translationValues(
                            destinationRect.left,
                            destinationRect.top,
                            0.0
                        );
                        __cascade.scaleByDouble(scaleX, scaleY, 1.0, 1);
                        __cascade.translateByDouble(-sourceRect.left, -sourceRect.top, 0, 1);
                        return __cascade;
                    }
                )
            )();
            DartRuntimePrimitives.Assert(() =>
                _transform!.storage.All((value) => double.IsFinite((value)))
            );
        }
    }

    internal virtual TransformLayer? _paintChildWithTransform(
        PaintingContext context,
        Offset offset
    )
    {
        Offset? childOffset = MatrixUtils.getAsTranslation(_transform!);
        if (childOffset is null)
        {
            return context.pushTransform(
                needsCompositing,
                offset,
                _transform!,
                base.paint,
                oldLayer: (layer is TransformLayer) ? ((TransformLayer?)layer!)! : null
            );
        }
        else
        {
            base.paint(
                context,
                offset
                    + (
                        childOffset
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
            );
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is null) || size.isEmpty || child!.size.isEmpty)
        {
            return;
        }
        _updatePaintData();
        DartRuntimePrimitives.Assert(() => child is not null);
        if (
            (
                _hasVisualOverflow
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ) && (!Equals(clipBehavior, Clip.none))
        )
        {
            layer = context.pushClipRect(
                needsCompositing,
                offset,
                Offset.zero & size,
                (__arg0, __arg1) =>
                {
                    _ = ((Func<PaintingContext, Offset, TransformLayer?>)_paintChildWithTransform)(
                        __arg0,
                        __arg1
                    );
                },
                oldLayer: (layer is ClipRectLayer) ? ((ClipRectLayer?)layer!)! : null,
                clipBehavior: clipBehavior
            );
        }
        else
        {
            layer = _paintChildWithTransform(context, offset);
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        if (size.isEmpty || (child?.size.isEmpty ?? false))
        {
            return false;
        }
        _updatePaintData();
        return result.addWithPaintTransform(
            transform: _transform,
            position: position,
            hitTest: (result, position) =>
            {
                return base.hitTestChildren(result, position: position);
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool paintsChild(RenderObject child)
    {
        var __child = (RenderBox)child;
        DartRuntimePrimitives.Assert(() => Equals(__child.parent, this));
        return !size.isEmpty && !__child.size.isEmpty;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)child;
        if (!paintsChild(__child))
        {
            transform.setZero();
        }
        else
        {
            _updatePaintData();
            transform.multiply(_transform!);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<BoxFit>("fit", fit));
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
        properties.add(
            new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null)
        );
    }
}

public class RenderFractionalTranslation : RenderProxyBox
{
    internal virtual Offset _translation { get; set; } = default!;
    public virtual bool transformHitTests { get; set; } = default!;

    public RenderFractionalTranslation(
        Offset translation,
        bool transformHitTests = true,
        RenderBox? child = null
    )
        : base(child)
    {
        this.transformHitTests = transformHitTests;
        _translation = translation;
    }

    public virtual Offset translation
    {
        get => _translation;
        set
        {
            var __value = value;
            if (Equals(_translation, __value))
            {
                return;
            }
            _translation = __value;
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        return hitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        return result.addWithPaintOffset(
            offset: transformHitTests
                ? new global::Doroti.Ui.Offset(
                    translation.dx * size.width,
                    translation.dy * size.height
                )
                : null,
            position: position,
            hitTest: (result, position) =>
            {
                return base.hitTestChildren(result, position: position);
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        if (child is not null)
        {
            base.paint(
                context,
                new Offset(
                    offset.dx + (translation.dx * size.width),
                    offset.dy + (translation.dy * size.height)
                )
            );
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)child;
        transform.translateByDouble(
            translation.dx * size.width,
            translation.dy * size.height,
            0,
            1
        );
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("translation", translation));
        properties.add(new DiagnosticsProperty<bool>("transformHitTests", transformHitTests));
    }
}

public delegate void PointerDownEventListener(Gestures.PointerDownEvent @event);

public delegate void PointerMoveEventListener(Gestures.PointerMoveEvent @event);

public delegate void PointerUpEventListener(Gestures.PointerUpEvent @event);

public delegate void PointerCancelEventListener(Gestures.PointerCancelEvent @event);

public delegate void PointerPanZoomStartEventListener(PointerPanZoomStartEvent @event);

public delegate void PointerPanZoomUpdateEventListener(PointerPanZoomUpdateEvent @event);

public delegate void PointerPanZoomEndEventListener(PointerPanZoomEndEvent @event);

public delegate void PointerSignalEventListener(PointerSignalEvent @event);

public class RenderPointerListener : RenderProxyBoxWithHitTestBehavior
{
    public virtual Action<Gestures.PointerDownEvent>? onPointerDown { get; set; } = default;
    public virtual Action<Gestures.PointerMoveEvent>? onPointerMove { get; set; } = default;
    public virtual Action<Gestures.PointerUpEvent>? onPointerUp { get; set; } = default;
    public virtual Action<Gestures.PointerHoverEvent>? onPointerHover { get; set; } = default;
    public virtual Action<Gestures.PointerCancelEvent>? onPointerCancel { get; set; } = default;
    public virtual Action<PointerPanZoomStartEvent>? onPointerPanZoomStart { get; set; } = default;
    public virtual Action<PointerPanZoomUpdateEvent>? onPointerPanZoomUpdate { get; set; } =
        default;
    public virtual Action<PointerPanZoomEndEvent>? onPointerPanZoomEnd { get; set; } = default;
    public virtual Action<PointerSignalEvent>? onPointerSignal { get; set; } = default;

    public RenderPointerListener(
        Action<Gestures.PointerDownEvent>? onPointerDown = null,
        Action<Gestures.PointerMoveEvent>? onPointerMove = null,
        Action<Gestures.PointerUpEvent>? onPointerUp = null,
        Action<Gestures.PointerHoverEvent>? onPointerHover = null,
        Action<Gestures.PointerCancelEvent>? onPointerCancel = null,
        Action<PointerPanZoomStartEvent>? onPointerPanZoomStart = null,
        Action<PointerPanZoomUpdateEvent>? onPointerPanZoomUpdate = null,
        Action<PointerPanZoomEndEvent>? onPointerPanZoomEnd = null,
        Action<PointerSignalEvent>? onPointerSignal = null,
        HitTestBehavior behavior = HitTestBehavior.deferToChild,
        RenderBox? child = null
    )
        : base(behavior: behavior, child: child)
    {
        this.onPointerDown = onPointerDown;
        this.onPointerMove = onPointerMove;
        this.onPointerUp = onPointerUp;
        this.onPointerHover = onPointerHover;
        this.onPointerCancel = onPointerCancel;
        this.onPointerPanZoomStart = onPointerPanZoomStart;
        this.onPointerPanZoomUpdate = onPointerPanZoomUpdate;
        this.onPointerPanZoomEnd = onPointerPanZoomEnd;
        this.onPointerSignal = onPointerSignal;
    }

    public override Size computeSizeForNoChild(BoxConstraints constraints)
    {
        return constraints.biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        switch (@event)
        {
            case Gestures.PointerDownEvent __object105825:
                onPointerDown?.Invoke(__object105825);
                break;
            case Gestures.PointerMoveEvent __object105881:
                onPointerMove?.Invoke(__object105881);
                break;
            case Gestures.PointerUpEvent __object105937:
                onPointerUp?.Invoke(__object105937);
                break;
            case Gestures.PointerHoverEvent __object105989:
                onPointerHover?.Invoke(__object105989);
                break;
            case Gestures.PointerCancelEvent __object106047:
                onPointerCancel?.Invoke(__object106047);
                break;
            case PointerPanZoomStartEvent __object106107:
                onPointerPanZoomStart?.Invoke(__object106107);
                break;
            case PointerPanZoomUpdateEvent __object106179:
                onPointerPanZoomUpdate?.Invoke(__object106179);
                break;
            case PointerPanZoomEndEvent __object106253:
                onPointerPanZoomEnd?.Invoke(__object106253);
                break;
            case PointerSignalEvent __object106321:
                onPointerSignal?.Invoke(__object106321);
                break;
            default:
                break;
        }
        return;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new FlagsSummary<Delegate?>(
                "listeners",
                new DartMap<string, Delegate?>
                {
                    ["down"] = onPointerDown,
                    ["move"] = onPointerMove,
                    ["up"] = onPointerUp,
                    ["hover"] = onPointerHover,
                    ["cancel"] = onPointerCancel,
                    ["panZoomStart"] = onPointerPanZoomStart,
                    ["panZoomUpdate"] = onPointerPanZoomUpdate,
                    ["panZoomEnd"] = onPointerPanZoomEnd,
                    ["signal"] = onPointerSignal,
                },
                ifEmpty: "<none>"
            )
        );
    }
}

public class RenderMouseRegion : RenderProxyBoxWithHitTestBehavior, IMouseTrackerAnnotation
{
    internal virtual bool _opaque { get; set; } = default!;
    public virtual Action<Gestures.PointerEnterEvent>? onEnter { get; set; } = default;
    public virtual Action<Gestures.PointerHoverEvent>? onHover { get; set; } = default;
    public virtual Action<Gestures.PointerExitEvent>? onExit { get; set; } = default;
    internal virtual MouseCursor _cursor { get; set; } = default!;
    internal virtual bool _validForMouseTracker { get; set; } = default!;
    IMouseTrackerCallback? IMouseTrackerAnnotation.onEnter =>
        onEnter is null ? null : new MouseTrackerCallback<Gestures.PointerEnterEvent>(onEnter);
    IMouseTrackerCallback? IMouseTrackerAnnotation.onExit =>
        onExit is null ? null : new MouseTrackerCallback<Gestures.PointerExitEvent>(onExit);

    public RenderMouseRegion(
        Action<Gestures.PointerEnterEvent>? onEnter = null,
        Action<Gestures.PointerHoverEvent>? onHover = null,
        Action<Gestures.PointerExitEvent>? onExit = null,
        MouseCursor cursor = default!,
        bool validForMouseTracker = true,
        bool opaque = true,
        RenderBox? child = null,
        HitTestBehavior? hitTestBehavior = HitTestBehavior.opaque
    )
        : base(child: child, behavior: hitTestBehavior ?? HitTestBehavior.opaque)
    {
        MouseCursor __cursor = cursor ?? MouseCursor.defer;
        this.onEnter = onEnter;
        this.onHover = onHover;
        this.onExit = onExit;
        _cursor = __cursor;
        _validForMouseTracker = validForMouseTracker;
        _opaque = opaque;
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        return base.hitTest(result, position: position) && _opaque;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if (@event is Gestures.PointerHoverEvent)
        {
            Gestures.PointerHoverEvent @event__as108765 = (Gestures.PointerHoverEvent)@event;
            onHover?.Invoke(@event__as108765);
        }
    }

    public virtual bool opaque
    {
        get => _opaque;
        set
        {
            var __value = value;
            if (_opaque != (__value))
            {
                _opaque = (__value);
                markNeedsPaint();
            }
        }
    }
    public virtual HitTestBehavior? hitTestBehavior
    {
        get => behavior;
        set
        {
            var __value = value;
            HitTestBehavior newValue = __value ?? HitTestBehavior.opaque;
            if (!Equals(behavior, newValue))
            {
                behavior = newValue;
                markNeedsPaint();
            }
        }
    }
    public virtual MouseCursor cursor
    {
        get => _cursor;
        set
        {
            var __value = value;
            if (!Equals(_cursor, __value))
            {
                _cursor = __value;
                markNeedsPaint();
            }
        }
    }
    public virtual bool validForMouseTracker => _validForMouseTracker;

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _validForMouseTracker = true;
    }

    public override void detach()
    {
        _validForMouseTracker = false;
        base.detach();
    }

    public override Size computeSizeForNoChild(BoxConstraints constraints)
    {
        return constraints.biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new FlagsSummary<Delegate?>(
                "listeners",
                new DartMap<string, Delegate?>
                {
                    ["enter"] = onEnter,
                    ["hover"] = onHover,
                    ["exit"] = onExit,
                },
                ifEmpty: "<none>"
            )
        );
        properties.add(
            new DiagnosticsProperty<MouseCursor>("cursor", cursor, defaultValue: MouseCursor.defer)
        );
        properties.add(new DiagnosticsProperty<bool>("opaque", opaque, defaultValue: true));
        properties.add(
            new FlagProperty(
                "validForMouseTracker",
                value: validForMouseTracker,
                defaultValue: true,
                ifFalse: "invalid for MouseTracker"
            )
        );
    }
}

public class RenderRepaintBoundary : RenderProxyBox
{
    internal virtual long _debugSymmetricPaintCount { get; set; } = 0L;
    internal virtual long _debugAsymmetricPaintCount { get; set; } = 0L;

    public RenderRepaintBoundary(RenderBox? child = null)
        : base(child) { }

    public override bool isRepaintBoundary => true;

    public virtual Future<Image> toImage(double pixelRatio = 1.0)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsPaint);
        var offsetLayer = ((OffsetLayer?)layer!)!;
        return offsetLayer.toImage(Offset.zero & size, pixelRatio: pixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Image toImageSync(double pixelRatio = 1.0)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsPaint);
        var offsetLayer = ((OffsetLayer?)layer!)!;
        return offsetLayer.toImageSync(Offset.zero & size, pixelRatio: pixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long debugSymmetricPaintCount => _debugSymmetricPaintCount;
    public virtual long debugAsymmetricPaintCount => _debugAsymmetricPaintCount;

    public virtual void debugResetMetrics()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _debugSymmetricPaintCount = 0L;
            _debugAsymmetricPaintCount = 0L;
            return true;
        });
    }

    public override void debugRegisterRepaintBoundaryPaint(
        bool includedParent = true,
        bool includedChild = false
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (includedParent && includedChild)
            {
                _debugSymmetricPaintCount += 1L;
            }
            else
            {
                _debugAsymmetricPaintCount += 1L;
            }
            return true;
        });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var inReleaseMode = true;
        DartRuntimePrimitives.Assert(() =>
        {
            inReleaseMode = false;
            long totalPaints = debugSymmetricPaintCount + debugAsymmetricPaintCount;
            if (totalPaints == 0L)
            {
                properties.add(
                    new MessageProperty(
                        "usefulness ratio",
                        "no metrics collected yet (never painted)"
                    )
                );
            }
            else
            {
                double fraction = debugAsymmetricPaintCount / totalPaints;
                string diagnosis = fraction switch
                {
                    _ when totalPaints < 5L =>
                        "insufficient data to draw conclusion (less than five repaints)",
                    > 0.9 =>
                        "this is an outstandingly useful repaint boundary and should definitely be kept",
                    > 0.5 => "this is a useful repaint boundary and should be kept",
                    > 0.3 =>
                        "this repaint boundary is probably useful, but maybe it would be more useful in tandem with adding more repaint boundaries elsewhere",
                    > 0.1 =>
                        "this repaint boundary does sometimes show value, though currently not that often",
                    _ when debugAsymmetricPaintCount > 0L =>
                        "this repaint boundary is not very effective and should probably be removed",
                    _ => "this repaint boundary is astoundingly ineffectual and should be removed",
                };
                properties.add(
                    new PercentProperty(
                        "metrics",
                        fraction,
                        unit: "useful",
                        tooltip: $"{debugSymmetricPaintCount} bad vs {debugAsymmetricPaintCount} good"
                    )
                );
                properties.add(new MessageProperty("diagnosis", diagnosis));
            }
            return true;
        });
        if (inReleaseMode)
        {
            properties.add(
                new DiagnosticsNode("(run in debug mode to collect repaint boundary statistics)")
            );
        }
    }
}

public class RenderIgnorePointer : RenderProxyBox
{
    internal virtual bool _ignoring { get; set; } = default!;
    internal virtual bool? _ignoringSemantics { get; set; } = default;

    public RenderIgnorePointer(
        RenderBox? child = null,
        bool ignoring = true,
        bool? ignoringSemantics = null
    )
        : base(child)
    {
        _ignoring = ignoring;
        _ignoringSemantics = ignoringSemantics;
    }

    public virtual bool ignoring
    {
        get => _ignoring;
        set
        {
            var __value = value;
            if ((__value) == _ignoring)
            {
                return;
            }
            _ignoring = (__value);
            if (ignoringSemantics is null)
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool? ignoringSemantics
    {
        get => _ignoringSemantics;
        set
        {
            var __value = value;
            if (__value == _ignoringSemantics)
            {
                return;
            }
            _ignoringSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        return !ignoring && base.hitTest(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (_ignoringSemantics ?? false)
        {
            return;
        }
        base.visitChildrenForSemantics(visitor);
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isBlockingUserActions = _ignoring && (_ignoringSemantics ?? true);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("ignoring", _ignoring));
        properties.add(
            new DiagnosticsProperty<bool?>(
                "ignoringSemantics",
                _ignoringSemantics,
                description: (_ignoringSemantics is null)
                    ? null
                    : $"implicitly {_ignoringSemantics}"
            )
        );
    }
}

public class RenderOffstage : RenderProxyBox
{
    internal virtual bool _offstage { get; set; } = default!;

    public RenderOffstage(bool offstage = true, RenderBox? child = null)
        : base(child)
    {
        _offstage = offstage;
    }

    public virtual bool offstage
    {
        get => _offstage;
        set
        {
            var __value = value;
            if (__value == _offstage)
            {
                return;
            }
            _offstage = __value;
            markNeedsLayoutForSizedByParentChange();
        }
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        if (offstage)
        {
            return 0.0;
        }
        return base.computeMinIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if (offstage)
        {
            return 0.0;
        }
        return base.computeMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if (offstage)
        {
            return 0.0;
        }
        return base.computeMinIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if (offstage)
        {
            return 0.0;
        }
        return base.computeMaxIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        if (offstage)
        {
            return null;
        }
        return base.computeDistanceToActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool sizedByParent => offstage;

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        return offstage ? null : base.computeDryBaseline(constraints, baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if (offstage)
        {
            return constraints.smallest;
        }
        return base.computeDryLayout(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performResize()
    {
        DartRuntimePrimitives.Assert(() => offstage);
        base.performResize();
    }

    public override void performLayout()
    {
        if (offstage)
        {
            child?.layout(constraints);
        }
        else
        {
            base.performLayout();
        }
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        return !offstage && base.hitTest(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool paintsChild(RenderObject child)
    {
        var __child = (RenderBox)child;
        DartRuntimePrimitives.Assert(() => Equals(__child.parent, this));
        return !offstage;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (offstage)
        {
            return;
        }
        base.paint(context, offset);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (offstage)
        {
            return;
        }
        base.visitChildrenForSemantics(visitor);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("offstage", offstage));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        if (child is null)
        {
            return new List<DiagnosticsNode>();
        }
        return new List<DiagnosticsNode>
        {
            ((Diagnosticable)child!).toDiagnosticsNode(
                name: "child",
                style: offstage ? DiagnosticsTreeStyle.offstage : DiagnosticsTreeStyle.sparse
            ),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RenderAbsorbPointer : RenderProxyBox
{
    internal virtual bool _absorbing { get; set; } = default!;
    internal virtual bool? _ignoringSemantics { get; set; } = default;

    public RenderAbsorbPointer(
        RenderBox? child = null,
        bool absorbing = true,
        bool? ignoringSemantics = null
    )
        : base(child)
    {
        _absorbing = absorbing;
        _ignoringSemantics = ignoringSemantics;
    }

    public virtual bool absorbing
    {
        get => _absorbing;
        set
        {
            var __value = value;
            if (_absorbing == (__value))
            {
                return;
            }
            _absorbing = (__value);
            if (ignoringSemantics is null)
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool? ignoringSemantics
    {
        get => _ignoringSemantics;
        set
        {
            var __value = value;
            if (__value == _ignoringSemantics)
            {
                return;
            }
            _ignoringSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        return absorbing ? size.contains(position) : base.hitTest(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (_ignoringSemantics ?? false)
        {
            return;
        }
        base.visitChildrenForSemantics(visitor);
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isBlockingUserActions = absorbing && (_ignoringSemantics ?? true);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("absorbing", absorbing));
        properties.add(
            new DiagnosticsProperty<bool?>(
                "ignoringSemantics",
                ignoringSemantics,
                description: (ignoringSemantics is null) ? null : $"implicitly {ignoringSemantics}"
            )
        );
    }
}

public class RenderMetaData : RenderProxyBoxWithHitTestBehavior
{
    public virtual object? metaData { get; set; } = default!;

    public RenderMetaData(
        object? metaData = null,
        HitTestBehavior behavior = HitTestBehavior.deferToChild,
        RenderBox? child = null
    )
        : base(behavior: behavior, child: child)
    {
        this.metaData = metaData;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<object>("metaData", metaData));
    }
}

public class RenderSemanticsGestureHandler : RenderProxyBoxWithHitTestBehavior
{
    internal virtual HashSet<SemanticsAction>? _validActions { get; set; } = default;
    internal virtual Action? _onTap { get; set; } = default;
    internal virtual Action? _onLongPress { get; set; } = default;
    internal virtual Action<DragUpdateDetails>? _onHorizontalDragUpdate { get; set; } = default;
    internal virtual Action<DragUpdateDetails>? _onVerticalDragUpdate { get; set; } = default;
    public virtual double scrollFactor { get; set; } = default!;

    public RenderSemanticsGestureHandler(
        RenderBox? child = null,
        Action? onTap = null,
        Action? onLongPress = null,
        Action<DragUpdateDetails>? onHorizontalDragUpdate = null,
        Action<DragUpdateDetails>? onVerticalDragUpdate = null,
        double scrollFactor = 0.8,
        HitTestBehavior behavior = HitTestBehavior.deferToChild
    )
        : base(child: child, behavior: behavior)
    {
        this.scrollFactor = scrollFactor;
        _onTap = onTap;
        _onLongPress = onLongPress;
        _onHorizontalDragUpdate = onHorizontalDragUpdate;
        _onVerticalDragUpdate = onVerticalDragUpdate;
    }

    public virtual HashSet<SemanticsAction>? validActions
    {
        get => _validActions;
        set
        {
            var __value = value is null ? null : value;
            if (CollectionsLibrary.setEquals(__value, _validActions))
            {
                return;
            }
            _validActions = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual Action? onTap
    {
        get => _onTap;
        set
        {
            var __value = value;
            if (Equals(_onTap, __value))
            {
                return;
            }
            var hadHandler = _onTap is not null;
            _onTap = __value;
            if ((__value is not null) != hadHandler)
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual Action? onLongPress
    {
        get => _onLongPress;
        set
        {
            var __value = value;
            if (Equals(_onLongPress, __value))
            {
                return;
            }
            var hadHandler = _onLongPress is not null;
            _onLongPress = __value;
            if ((__value is not null) != hadHandler)
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual Action<DragUpdateDetails>? onHorizontalDragUpdate
    {
        get => _onHorizontalDragUpdate;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(_onHorizontalDragUpdate, __value))
            {
                return;
            }
            var hadHandler = _onHorizontalDragUpdate is not null;
            _onHorizontalDragUpdate = __value;
            if ((__value is not null) != hadHandler)
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual Action<DragUpdateDetails>? onVerticalDragUpdate
    {
        get => _onVerticalDragUpdate;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(_onVerticalDragUpdate, __value))
            {
                return;
            }
            var hadHandler = _onVerticalDragUpdate is not null;
            _onVerticalDragUpdate = __value;
            if ((__value is not null) != hadHandler)
            {
                markNeedsSemanticsUpdate();
            }
        }
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        if ((onTap is not null) && _isValidAction(SemanticsAction.tap))
        {
            config.onTap = onTap;
        }
        if ((onLongPress is not null) && _isValidAction(SemanticsAction.longPress))
        {
            config.onLongPress = onLongPress;
        }
        if (onHorizontalDragUpdate is not null)
        {
            if (_isValidAction(SemanticsAction.scrollRight))
            {
                config.onScrollRight = _performSemanticScrollRight;
            }
            if (_isValidAction(SemanticsAction.scrollLeft))
            {
                config.onScrollLeft = _performSemanticScrollLeft;
            }
        }
        if (onVerticalDragUpdate is not null)
        {
            if (_isValidAction(SemanticsAction.scrollUp))
            {
                config.onScrollUp = _performSemanticScrollUp;
            }
            if (_isValidAction(SemanticsAction.scrollDown))
            {
                config.onScrollDown = _performSemanticScrollDown;
            }
        }
    }

    internal virtual bool _isValidAction(SemanticsAction action)
    {
        return (validActions is null) || validActions!.Contains(action);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _performSemanticScrollLeft()
    {
        if (onHorizontalDragUpdate is not null)
        {
            double primaryDeltaLocal = size.width * -scrollFactor;
            onHorizontalDragUpdate!(
                new DragUpdateDetails(
                    delta: new Offset(primaryDeltaLocal, 0.0),
                    primaryDelta: primaryDeltaLocal,
                    globalPosition: localToGlobal(size.center(Offset.zero))
                )
            );
        }
    }

    internal virtual void _performSemanticScrollRight()
    {
        if (onHorizontalDragUpdate is not null)
        {
            double primaryDeltaLocal = size.width * scrollFactor;
            onHorizontalDragUpdate!(
                new DragUpdateDetails(
                    delta: new Offset(primaryDeltaLocal, 0.0),
                    primaryDelta: primaryDeltaLocal,
                    globalPosition: localToGlobal(size.center(Offset.zero))
                )
            );
        }
    }

    internal virtual void _performSemanticScrollUp()
    {
        if (onVerticalDragUpdate is not null)
        {
            double primaryDeltaLocal = size.height * -scrollFactor;
            onVerticalDragUpdate!(
                new DragUpdateDetails(
                    delta: new Offset(0.0, primaryDeltaLocal),
                    primaryDelta: primaryDeltaLocal,
                    globalPosition: localToGlobal(size.center(Offset.zero))
                )
            );
        }
    }

    internal virtual void _performSemanticScrollDown()
    {
        if (onVerticalDragUpdate is not null)
        {
            double primaryDeltaLocal = size.height * scrollFactor;
            onVerticalDragUpdate!(
                new DragUpdateDetails(
                    delta: new Offset(0.0, primaryDeltaLocal),
                    primaryDelta: primaryDeltaLocal,
                    globalPosition: localToGlobal(size.center(Offset.zero))
                )
            );
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var gestures = new List<string>();
        if (checked((long)gestures.Count) == 0)
        {
            gestures.Add("<none>");
        }
        properties.add(new IterableProperty<string>("gestures", gestures));
    }
}

public class RenderSemanticsAnnotations : RenderProxyBox, SemanticsAnnotationsMixin
{
    public virtual SemanticsProperties _properties { get; set; } = default!;
    public virtual bool _container { get; set; } = default!;
    public virtual bool _explicitChildNodes { get; set; } = default!;
    public virtual bool _excludeSemantics { get; set; } = default!;
    public virtual bool _blockUserActions { get; set; } = default!;
    public virtual Locale? _localeForSubtree { get; set; } = default;
    public virtual AttributedString? _attributedLabel { get; set; } = default;
    public virtual AttributedString? _attributedValue { get; set; } = default;
    public virtual AttributedString? _attributedIncreasedValue { get; set; } = default;
    public virtual AttributedString? _attributedDecreasedValue { get; set; } = default;
    public virtual AttributedString? _attributedHint { get; set; } = default;
    public virtual TextDirection? _textDirection { get; set; } = default;

    public RenderSemanticsAnnotations(
        RenderBox? child = null,
        SemanticsProperties properties = default!,
        bool container = false,
        bool explicitChildNodes = false,
        bool excludeSemantics = false,
        bool blockUserActions = false,
        Locale? localeForSubtree = null,
        TextDirection? textDirection = null
    )
        : base(child)
    {
        initSemanticsAnnotations(
            properties,
            container,
            explicitChildNodes,
            excludeSemantics,
            blockUserActions,
            localeForSubtree,
            textDirection
        );
    }

    public virtual void initSemanticsAnnotations(
        SemanticsProperties properties,
        bool container,
        bool explicitChildNodes,
        bool excludeSemantics,
        bool blockUserActions,
        Locale? localeForSubtree,
        TextDirection? textDirection
    )
    {
        _properties = properties;
        _container = container;
        _explicitChildNodes = explicitChildNodes;
        _excludeSemantics = excludeSemantics;
        _blockUserActions = blockUserActions;
        _localeForSubtree = localeForSubtree;
        _textDirection = textDirection;
        _updateAttributedFields(_properties);
    }

    public virtual SemanticsProperties properties
    {
        get => _properties;
        set
        {
            var __value = value;
            if (Equals(_properties, __value))
            {
                return;
            }
            _properties = __value;
            _updateAttributedFields(_properties);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool container
    {
        get => _container;
        set
        {
            var __value = value;
            if (container == __value)
            {
                return;
            }
            _container = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool explicitChildNodes
    {
        get => _explicitChildNodes;
        set
        {
            var __value = value;
            if (_explicitChildNodes == __value)
            {
                return;
            }
            _explicitChildNodes = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool excludeSemantics
    {
        get => _excludeSemantics;
        set
        {
            var __value = value;
            if (_excludeSemantics == __value)
            {
                return;
            }
            _excludeSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool blockUserActions
    {
        get => _blockUserActions;
        set
        {
            var __value = value;
            if (_blockUserActions == __value)
            {
                return;
            }
            _blockUserActions = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual Locale? localeForSubtree
    {
        get => _localeForSubtree;
        set
        {
            var __value = value;
            if (Equals(_localeForSubtree, __value))
            {
                return;
            }
            _localeForSubtree = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public virtual void _updateAttributedFields(SemanticsProperties value)
    {
        _attributedLabel = _effectiveAttributedLabel(value);
        _attributedValue = _effectiveAttributedValue(value);
        _attributedIncreasedValue = _effectiveAttributedIncreasedValue(value);
        _attributedDecreasedValue = _effectiveAttributedDecreasedValue(value);
        _attributedHint = _effectiveAttributedHint(value);
    }

    public virtual AttributedString? _effectiveAttributedLabel(SemanticsProperties value)
    {
        return value.attributedLabel
            ?? ((value.label is null) ? null : new AttributedString(value.label!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AttributedString? _effectiveAttributedValue(SemanticsProperties value)
    {
        return value.attributedValue
            ?? ((value.value is null) ? null : new AttributedString(value.value!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AttributedString? _effectiveAttributedIncreasedValue(SemanticsProperties value)
    {
        return value.attributedIncreasedValue
            ?? (
                (value.increasedValue is null) ? null : new AttributedString(value.increasedValue!)
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AttributedString? _effectiveAttributedDecreasedValue(SemanticsProperties value)
    {
        return properties.attributedDecreasedValue
            ?? (
                (value.decreasedValue is null) ? null : new AttributedString(value.decreasedValue!)
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AttributedString? _effectiveAttributedHint(SemanticsProperties value)
    {
        return value.attributedHint
            ?? ((value.hint is null) ? null : new AttributedString(value.hint!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextDirection? textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(textDirection, __value))
            {
                return;
            }
            _textDirection = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (excludeSemantics)
        {
            return;
        }
        base.visitChildrenForSemantics(visitor);
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = container || _properties.identifier is not null;
        config.explicitChildNodes = explicitChildNodes;
        config.isBlockingUserActions = blockUserActions;
        if (localeForSubtree is not null)
        {
            config.localeForSubtree = localeForSubtree;
        }
        DartRuntimePrimitives.Assert(() =>
            ((_properties.scopesRoute ?? false) && explicitChildNodes)
            || !(_properties.scopesRoute ?? false)
        );
        DartRuntimePrimitives.Assert(() =>
            !((_properties.toggled ?? false) && (_properties.@checked ?? false))
        );
        if (_properties.enabled is not null)
        {
            config.isEnabled = _properties.enabled;
        }
        if (_properties.@checked is not null)
        {
            config.isChecked = _properties.@checked;
        }
        if (_properties.mixed is not null)
        {
            config.isCheckStateMixed = _properties.mixed;
        }
        if (_properties.toggled is not null)
        {
            config.isToggled = _properties.toggled;
        }
        if (_properties.selected is not null)
        {
            config.isSelected = (
                _properties.selected
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.button is not null)
        {
            config.isButton = (
                _properties.button
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.expanded is not null)
        {
            config.isExpanded = _properties.expanded;
        }
        if (_properties.link is not null)
        {
            config.isLink = (
                _properties.link
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.linkUrl is not null)
        {
            config.linkUrl = _properties.linkUrl;
        }
        if (_properties.slider is not null)
        {
            config.isSlider = (
                _properties.slider
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.keyboardKey is not null)
        {
            config.isKeyboardKey = (
                _properties.keyboardKey
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.header is not null)
        {
            config.isHeader = (
                _properties.header
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.headingLevel is not null)
        {
            config.headingLevel = (
                _properties.headingLevel
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.textField is not null)
        {
            config.isTextField = (
                _properties.textField
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.readOnly is not null)
        {
            config.isReadOnly = (
                _properties.readOnly
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.focusable is not null)
        {
            config.isFocusable = (
                _properties.focusable
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.focused is not null)
        {
            config.isFocused = _properties.focused;
        }
        if (_properties.accessibilityFocusBlockType is not null)
        {
            config.accessibilityFocusBlockType = (
                _properties.accessibilityFocusBlockType
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.inMutuallyExclusiveGroup is not null)
        {
            config.isInMutuallyExclusiveGroup = (
                _properties.inMutuallyExclusiveGroup
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.obscured is not null)
        {
            config.isObscured = (
                _properties.obscured
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.multiline is not null)
        {
            config.isMultiline = (
                _properties.multiline
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.hidden is not null)
        {
            config.isHidden = (
                _properties.hidden
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.image is not null)
        {
            config.isImage = (
                _properties.image
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.isRequired is not null)
        {
            config.isRequired = _properties.isRequired;
        }
        if (_properties.identifier is not null)
        {
            config.identifier = _properties.identifier!;
        }
        if (_properties.traversalParentIdentifier is not null)
        {
            config.traversalParentIdentifier = _properties.traversalParentIdentifier;
        }
        if (_properties.traversalChildIdentifier is not null)
        {
            config.traversalChildIdentifier = _properties.traversalChildIdentifier;
        }
        if (_attributedLabel is not null)
        {
            config.attributedLabel = _attributedLabel!;
        }
        if (_attributedValue is not null)
        {
            config.attributedValue = _attributedValue!;
        }
        if (_attributedIncreasedValue is not null)
        {
            config.attributedIncreasedValue = _attributedIncreasedValue!;
        }
        if (_attributedDecreasedValue is not null)
        {
            config.attributedDecreasedValue = _attributedDecreasedValue!;
        }
        if (_attributedHint is not null)
        {
            config.attributedHint = _attributedHint!;
        }
        if (_properties.tooltip is not null)
        {
            config.tooltip = _properties.tooltip!;
        }
        if ((_properties.hintOverrides is not null) && _properties.hintOverrides!.isNotEmpty)
        {
            config.hintOverrides = _properties.hintOverrides;
        }
        if (_properties.scopesRoute is not null)
        {
            config.scopesRoute = (
                _properties.scopesRoute
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.namesRoute is not null)
        {
            config.namesRoute = (
                _properties.namesRoute
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.liveRegion is not null)
        {
            config.liveRegion = (
                _properties.liveRegion
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.maxValueLength is not null)
        {
            config.maxValueLength = _properties.maxValueLength;
        }
        if (_properties.currentValueLength is not null)
        {
            config.currentValueLength = _properties.currentValueLength;
        }
        if (textDirection is not null)
        {
            config.textDirection = textDirection;
        }
        if (_properties.sortKey is not null)
        {
            config.sortKey = _properties.sortKey;
        }
        if (_properties.tagForChildren is not null)
        {
            config.addTagForChildren(_properties.tagForChildren!);
        }
        if (properties.role is not null)
        {
            config.role = (
                _properties.role
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.controlsNodes is not null)
        {
            config.controlsNodes = _properties.controlsNodes;
        }
        if (!Equals(config.validationResult, _properties.validationResult))
        {
            config.validationResult = _properties.validationResult;
        }
        if (_properties.hitTestBehavior is not null)
        {
            config.hitTestBehavior = (
                _properties.hitTestBehavior
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.inputType is not null)
        {
            config.inputType = (
                _properties.inputType
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        }
        if (_properties.minValue is not null)
        {
            config.minValue = _properties.minValue;
        }
        if (_properties.maxValue is not null)
        {
            config.maxValue = _properties.maxValue;
        }
        if (_properties.onTap is not null)
        {
            config.onTap = _performTap;
        }
        if (_properties.onLongPress is not null)
        {
            config.onLongPress = _performLongPress;
        }
        if (_properties.onDismiss is not null)
        {
            config.onDismiss = _performDismiss;
        }
        if (_properties.onScrollLeft is not null)
        {
            config.onScrollLeft = _performScrollLeft;
        }
        if (_properties.onScrollRight is not null)
        {
            config.onScrollRight = _performScrollRight;
        }
        if (_properties.onScrollUp is not null)
        {
            config.onScrollUp = _performScrollUp;
        }
        if (_properties.onScrollDown is not null)
        {
            config.onScrollDown = _performScrollDown;
        }
        if (_properties.onIncrease is not null)
        {
            config.onIncrease = _performIncrease;
        }
        if (_properties.onDecrease is not null)
        {
            config.onDecrease = _performDecrease;
        }
        if (_properties.onCopy is not null)
        {
            config.onCopy = _performCopy;
        }
        if (_properties.onCut is not null)
        {
            config.onCut = _performCut;
        }
        if (_properties.onPaste is not null)
        {
            config.onPaste = _performPaste;
        }
        if (_properties.onMoveCursorForwardByCharacter is not null)
        {
            config.onMoveCursorForwardByCharacter = _performMoveCursorForwardByCharacter;
        }
        if (_properties.onMoveCursorBackwardByCharacter is not null)
        {
            config.onMoveCursorBackwardByCharacter = _performMoveCursorBackwardByCharacter;
        }
        if (_properties.onMoveCursorForwardByWord is not null)
        {
            config.onMoveCursorForwardByWord = _performMoveCursorForwardByWord;
        }
        if (_properties.onMoveCursorBackwardByWord is not null)
        {
            config.onMoveCursorBackwardByWord = _performMoveCursorBackwardByWord;
        }
        if (_properties.onSetSelection is not null)
        {
            config.onSetSelection = _performSetSelection;
        }
        if (_properties.onSetText is not null)
        {
            config.onSetText = _performSetText;
        }
        if (_properties.onDidGainAccessibilityFocus is not null)
        {
            config.onDidGainAccessibilityFocus = _performDidGainAccessibilityFocus;
        }
        if (_properties.onDidLoseAccessibilityFocus is not null)
        {
            config.onDidLoseAccessibilityFocus = _performDidLoseAccessibilityFocus;
        }
        if (_properties.onFocus is not null)
        {
            config.onFocus = _performFocus;
        }
        if (_properties.onExpand is not null)
        {
            config.onExpand = _performExpand;
        }
        if (_properties.onCollapse is not null)
        {
            config.onCollapse = _performCollapse;
        }
        if (_properties.customSemanticsActions is not null)
        {
            config.customSemanticsActions = _properties.customSemanticsActions!;
        }
    }

    public virtual void _performTap()
    {
        _properties.onTap?.Invoke();
    }

    public virtual void _performLongPress()
    {
        _properties.onLongPress?.Invoke();
    }

    public virtual void _performDismiss()
    {
        _properties.onDismiss?.Invoke();
    }

    public virtual void _performScrollLeft()
    {
        _properties.onScrollLeft?.Invoke();
    }

    public virtual void _performScrollRight()
    {
        _properties.onScrollRight?.Invoke();
    }

    public virtual void _performScrollUp()
    {
        _properties.onScrollUp?.Invoke();
    }

    public virtual void _performScrollDown()
    {
        _properties.onScrollDown?.Invoke();
    }

    public virtual void _performIncrease()
    {
        _properties.onIncrease?.Invoke();
    }

    public virtual void _performDecrease()
    {
        _properties.onDecrease?.Invoke();
    }

    public virtual void _performCopy()
    {
        _properties.onCopy?.Invoke();
    }

    public virtual void _performCut()
    {
        _properties.onCut?.Invoke();
    }

    public virtual void _performPaste()
    {
        _properties.onPaste?.Invoke();
    }

    public virtual void _performMoveCursorForwardByCharacter(bool extendSelection)
    {
        _properties.onMoveCursorForwardByCharacter?.Invoke(extendSelection);
    }

    public virtual void _performMoveCursorBackwardByCharacter(bool extendSelection)
    {
        _properties.onMoveCursorBackwardByCharacter?.Invoke(extendSelection);
    }

    public virtual void _performMoveCursorForwardByWord(bool extendSelection)
    {
        _properties.onMoveCursorForwardByWord?.Invoke(extendSelection);
    }

    public virtual void _performMoveCursorBackwardByWord(bool extendSelection)
    {
        _properties.onMoveCursorBackwardByWord?.Invoke(extendSelection);
    }

    public virtual void _performSetSelection(TextSelection selection)
    {
        _properties.onSetSelection?.Invoke(selection);
    }

    public virtual void _performSetText(string text)
    {
        _properties.onSetText?.Invoke(text);
    }

    public virtual void _performDidGainAccessibilityFocus()
    {
        _properties.onDidGainAccessibilityFocus?.Invoke();
    }

    public virtual void _performDidLoseAccessibilityFocus()
    {
        _properties.onDidLoseAccessibilityFocus?.Invoke();
    }

    public virtual void _performFocus()
    {
        _properties.onFocus?.Invoke();
    }

    public virtual void _performExpand()
    {
        _properties.onExpand?.Invoke();
    }

    public virtual void _performCollapse()
    {
        _properties.onCollapse?.Invoke();
    }
}

public class RenderBlockSemantics : RenderProxyBox
{
    internal virtual bool _blocking { get; set; } = default!;

    public RenderBlockSemantics(RenderBox? child = null, bool blocking = true)
        : base(child)
    {
        _blocking = blocking;
    }

    public virtual bool blocking
    {
        get => _blocking;
        set
        {
            var __value = value;
            if (__value == _blocking)
            {
                return;
            }
            _blocking = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isBlockingSemanticsOfPreviouslyPaintedNodes = blocking;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("blocking", blocking));
    }
}

public class RenderMergeSemantics : RenderProxyBox
{
    public RenderMergeSemantics(RenderBox? child = null)
        : base(child) { }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        (
            (Func<SemanticsConfiguration>)(
                () =>
                {
                    var __cascade = config;
                    __cascade.isSemanticBoundary = true;
                    __cascade.isMergingSemanticsOfDescendants = true;
                    return __cascade;
                }
            )
        )();
    }
}

public class RenderExcludeSemantics : RenderProxyBox
{
    internal virtual bool _excluding { get; set; } = default!;

    public RenderExcludeSemantics(RenderBox? child = null, bool excluding = true)
        : base(child)
    {
        _excluding = excluding;
    }

    public virtual bool excluding
    {
        get => _excluding;
        set
        {
            var __value = value;
            if (__value == _excluding)
            {
                return;
            }
            _excluding = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (excluding)
        {
            return;
        }
        base.visitChildrenForSemantics(visitor);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("excluding", excluding));
    }
}

public class RenderIndexedSemantics : RenderProxyBox
{
    internal virtual long _index { get; set; } = default!;

    public RenderIndexedSemantics(RenderBox? child = null, long index = default!)
        : base(child)
    {
        _index = index;
    }

    public virtual long index
    {
        get => _index;
        set
        {
            var __value = value;
            if (__value == index)
            {
                return;
            }
            _index = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.indexInParent = index;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<long>("index", index));
    }
}

public class RenderLeaderLayer : RenderProxyBox
{
    internal virtual LayerLink _link { get; set; } = default!;
    internal virtual Size? _previousLayoutSize { get; set; } = default;

    public RenderLeaderLayer(LayerLink link, RenderBox? child = null)
        : base(child)
    {
        _link = link;
    }

    public virtual LayerLink link
    {
        get => _link;
        set
        {
            var __value = value;
            if (Equals(_link, __value))
            {
                return;
            }
            _link.leaderSize = null;
            _link = __value;
            if (_previousLayoutSize is not null)
            {
                _link.leaderSize = _previousLayoutSize;
            }
            markNeedsPaint();
        }
    }
    public override bool alwaysNeedsCompositing => true;

    public override void performLayout()
    {
        base.performLayout();
        _previousLayoutSize = size;
        link.leaderSize = size;
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (layer is null)
        {
            layer = new LeaderLayer(link: link, offset: offset);
        }
        else
        {
            var leaderLayer = ((LeaderLayer?)layer!)!;
            (
                (Func<LeaderLayer>)(
                    () =>
                    {
                        var __cascade = leaderLayer;
                        __cascade.link = link;
                        __cascade.offset = offset;
                        return __cascade;
                    }
                )
            )();
        }
        context.pushLayer(layer!, base.paint, Offset.zero);
        DartRuntimePrimitives.Assert(() =>
        {
            layer!.debugCreator = debugCreator;
            return true;
        });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<LayerLink>("link", link));
    }
}

public class RenderFollowerLayer : RenderProxyBox
{
    internal virtual LayerLink _link { get; set; } = default!;
    internal virtual bool _showWhenUnlinked { get; set; } = default!;
    internal virtual Offset _offset { get; set; } = default!;
    internal virtual Alignment _leaderAnchor { get; set; } = default!;
    internal virtual Alignment _followerAnchor { get; set; } = default!;

    public RenderFollowerLayer(
        LayerLink link,
        bool showWhenUnlinked = true,
        Offset offset = default,
        Alignment leaderAnchor = default!,
        Alignment followerAnchor = default!,
        RenderBox? child = null
    )
        : base(child)
    {
        Alignment __leaderAnchor = leaderAnchor ?? Alignment.topLeft;
        Alignment __followerAnchor = followerAnchor ?? Alignment.topLeft;
        _link = link;
        _showWhenUnlinked = showWhenUnlinked;
        _offset = offset;
        _leaderAnchor = __leaderAnchor;
        _followerAnchor = __followerAnchor;
    }

    public virtual LayerLink link
    {
        get => _link;
        set
        {
            var __value = value;
            if (Equals(_link, __value))
            {
                return;
            }
            _link = __value;
            markNeedsPaint();
        }
    }
    public virtual bool showWhenUnlinked
    {
        get => _showWhenUnlinked;
        set
        {
            var __value = value;
            if (_showWhenUnlinked == __value)
            {
                return;
            }
            _showWhenUnlinked = __value;
            markNeedsPaint();
        }
    }
    public virtual Offset offset
    {
        get => _offset;
        set
        {
            var __value = value;
            if (Equals(_offset, __value))
            {
                return;
            }
            _offset = __value;
            markNeedsPaint();
        }
    }
    public virtual Alignment leaderAnchor
    {
        get => _leaderAnchor;
        set
        {
            var __value = value;
            if (Equals(_leaderAnchor, __value))
            {
                return;
            }
            _leaderAnchor = __value;
            markNeedsPaint();
        }
    }
    public virtual Alignment followerAnchor
    {
        get => _followerAnchor;
        set
        {
            var __value = value;
            if (Equals(_followerAnchor, __value))
            {
                return;
            }
            _followerAnchor = __value;
            markNeedsPaint();
        }
    }

    public override void detach()
    {
        layer = null;
        base.detach();
    }

    public override bool alwaysNeedsCompositing => true;
    public override FollowerLayer? layer => ((FollowerLayer?)base.layer)!;

    public virtual Matrix4 getCurrentTransform()
    {
        return layer?.getLastTransform() ?? Matrix4.identity();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if ((link.leader is null) && !showWhenUnlinked)
        {
            return false;
        }
        return hitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        return result.addWithPaintTransform(
            transform: getCurrentTransform(),
            position: position,
            hitTest: (result, position) =>
            {
                return base.hitTestChildren(result, position: position);
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        Size? leaderSizeLocal = link.leaderSize;
        DartRuntimePrimitives.Assert(() =>
            (link.leaderSize is not null)
            || (link.leader is null)
            || Equals(leaderAnchor, Alignment.topLeft)
        );
        Offset effectiveLinkedOffset =
            (leaderSizeLocal is null)
                ? this.offset
                : (
                    leaderAnchor.alongSize(
                        (
                            (
                                leaderSizeLocal
                                ?? throw new global::System.NullReferenceException(
                                    "Dart null assertion failed."
                                )
                            )
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        )
                    )
                    - followerAnchor.alongSize(size)
                    + this.offset
                );
        if (layer is null)
        {
            layer = new FollowerLayer(
                link: link,
                showWhenUnlinked: showWhenUnlinked,
                linkedOffset: effectiveLinkedOffset,
                unlinkedOffset: offset
            );
        }
        else
        {
            (
                (Func<FollowerLayer?>)(
                    () =>
                    {
                        var __cascade = layer;
                        __cascade.link = link;
                        __cascade.showWhenUnlinked = showWhenUnlinked;
                        __cascade.linkedOffset = effectiveLinkedOffset;
                        __cascade.unlinkedOffset = offset;
                        return __cascade;
                    }
                )
            )();
        }
        context.pushLayer(
            layer!,
            base.paint,
            Offset.zero,
            childPaintBounds: Rect.fromLTRB(
                double.NegativeInfinity,
                double.NegativeInfinity,
                double.PositiveInfinity,
                double.PositiveInfinity
            )
        );
        DartRuntimePrimitives.Assert(() =>
        {
            layer!.debugCreator = debugCreator;
            return true;
        });
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)child;
        transform.multiply(getCurrentTransform());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<LayerLink>("link", link));
        properties.add(new DiagnosticsProperty<bool>("showWhenUnlinked", showWhenUnlinked));
        properties.add(new DiagnosticsProperty<Offset>("offset", offset));
        properties.add(new TransformProperty("current transform matrix", getCurrentTransform()));
    }
}

public class RenderAnnotatedRegion<T> : RenderProxyBox
{
    internal virtual T _value { get; set; } = default!;
    internal virtual bool _sized { get; set; } = default!;

    // Dart library-private member: distinct from the same name in the base library.
    public new virtual LayerHandle<AnnotatedRegionLayer<T>> _layerHandle { get; private set; } =
        default!;
    private bool __field_alwaysNeedsCompositing = true;
    public override bool alwaysNeedsCompositing
    {
        get => __field_alwaysNeedsCompositing;
    }

    public RenderAnnotatedRegion(T value, bool sized, RenderBox? child = null)
        : base(child)
    {
        _value = value;
        _sized = sized;
        _layerHandle = new LayerHandle<AnnotatedRegionLayer<T>>();
    }

    public virtual T value
    {
        get => _value;
        set
        {
            var newValue = value;
            if (EqualityComparer<T>.Default.Equals(_value, newValue))
            {
                return;
            }
            _value = newValue;
            markNeedsPaint();
        }
    }
    public virtual bool sized
    {
        get => _sized;
        set
        {
            var __value = value;
            if (_sized == __value)
            {
                return;
            }
            _sized = __value;
            markNeedsPaint();
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        var layerLocal = new AnnotatedRegionLayer<T>(
            value,
            size: sized ? size : null,
            offset: sized ? offset : null
        );
        _layerHandle.layer = layerLocal;
        context.pushLayer(layerLocal, base.paint, offset);
    }

    public override void dispose()
    {
        _layerHandle.layer = null;
        base.dispose();
    }
}
