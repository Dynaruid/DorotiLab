// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/proxy_box.dart
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

public class RenderProxyBox : RenderBox, RenderObjectWithChildMixin<RenderBox>, RenderProxyBoxMixin<RenderBox>
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

    public override void setupParentData(RenderObject child)
    {
        if ((((RenderObject)child).parentData is not ParentData))
        {
            child.parentData = new ParentData();
        }
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
        return (child?.getDistanceToActualBaseline(baseline) ?? base.computeDistanceToActualBaseline(baseline));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? child__3582 = this.child;
        return ((child__3582 is null) ? base.computeDryBaseline(constraints, baseline) : child__3582.getDryBaseline(constraints, baseline));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return (child?.getDryLayout(constraints) ?? computeSizeForNoChild(constraints));
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
        return ((BoxConstraints)constraints).smallest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        return (child?.hitTest(result, position: position) ?? false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderBox? child__4667 = this.child;
        if ((child__4667 is null))
        {
            return;
        }
        context.paintChild(child__4667, offset);
    }

}

public interface RenderProxyBoxMixin<T> where T : RenderBox
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
    public global::Doroti.Ui.Size computeSizeForNoChild(BoxConstraints constraints);
    public bool hitTestChildren(BoxHitTestResult result, Offset position);
    public void applyPaintTransform(RenderObject child, Matrix4 transform);
    public void paint(PaintingContext context, Offset offset);
}

public enum HitTestBehavior
{
    deferToChild,
    opaque,
    translucent
}

public abstract class RenderProxyBoxWithHitTestBehavior : RenderProxyBox
{
    public virtual HitTestBehavior behavior { get; set; } = default!;

    protected RenderProxyBoxWithHitTestBehavior(HitTestBehavior behavior = HitTestBehavior.deferToChild, RenderBox? child = null) : base(child)
    {
        this.behavior = behavior;
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        var hitTarget__6160 = false;
        if (size.contains(position))
        {
            hitTarget__6160 = (hitTestChildren(result, position: position) || hitTestSelf(position));
            if ((hitTarget__6160 || (object.Equals(this.behavior, HitTestBehavior.translucent))))
            {
                result.add(new BoxHitTestEntry(this, position));
            }
        }
        return hitTarget__6160;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position) => (object.Equals(this.behavior, HitTestBehavior.opaque));
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<HitTestBehavior>("behavior", this.behavior, defaultValue: null));
    }

}

public class RenderConstrainedBox : RenderProxyBox
{
    internal virtual BoxConstraints _additionalConstraints { get; set; } = default!;

    public RenderConstrainedBox(RenderBox? child = null, BoxConstraints additionalConstraints = default!) : base(child)
    {
        this._additionalConstraints = additionalConstraints;
        System.Diagnostics.Debug.Assert(additionalConstraints.debugAssertIsValid());
    }

    public virtual BoxConstraints additionalConstraints
    {
        get => this._additionalConstraints;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value.debugAssertIsValid());
            if ((object.Equals(this._additionalConstraints, __value)))
            {
                return;
            }
            _additionalConstraints = __value;
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        if ((((BoxConstraints)this._additionalConstraints).hasBoundedWidth && ((BoxConstraints)this._additionalConstraints).hasTightWidth))
        {
            return ((BoxConstraints)this._additionalConstraints).minWidth;
        }
        double width__8267 = base.computeMinIntrinsicWidth(height);
        DartRuntimePrimitives.Assert(() => double.IsFinite(width__8267));
        if (!((BoxConstraints)this._additionalConstraints).hasInfiniteWidth)
        {
            return this._additionalConstraints.constrainWidth(width__8267);
        }
        return width__8267;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if ((((BoxConstraints)this._additionalConstraints).hasBoundedWidth && ((BoxConstraints)this._additionalConstraints).hasTightWidth))
        {
            return ((BoxConstraints)this._additionalConstraints).minWidth;
        }
        double width__8705 = base.computeMaxIntrinsicWidth(height);
        DartRuntimePrimitives.Assert(() => double.IsFinite(width__8705));
        if (!((BoxConstraints)this._additionalConstraints).hasInfiniteWidth)
        {
            return this._additionalConstraints.constrainWidth(width__8705);
        }
        return width__8705;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if ((((BoxConstraints)this._additionalConstraints).hasBoundedHeight && ((BoxConstraints)this._additionalConstraints).hasTightHeight))
        {
            return ((BoxConstraints)this._additionalConstraints).minHeight;
        }
        double height__9146 = base.computeMinIntrinsicHeight(width);
        DartRuntimePrimitives.Assert(() => double.IsFinite(height__9146));
        if (!((BoxConstraints)this._additionalConstraints).hasInfiniteHeight)
        {
            return this._additionalConstraints.constrainHeight(height__9146);
        }
        return height__9146;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if ((((BoxConstraints)this._additionalConstraints).hasBoundedHeight && ((BoxConstraints)this._additionalConstraints).hasTightHeight))
        {
            return ((BoxConstraints)this._additionalConstraints).minHeight;
        }
        double height__9593 = base.computeMaxIntrinsicHeight(width);
        DartRuntimePrimitives.Assert(() => double.IsFinite(height__9593));
        if (!((BoxConstraints)this._additionalConstraints).hasInfiniteHeight)
        {
            return this._additionalConstraints.constrainHeight(height__9593);
        }
        return height__9593;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        return child?.getDryBaseline(this._additionalConstraints.enforce(constraints), baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraints__10075 = this.constraints;
        if ((child is not null))
        {
            child!.layout(this._additionalConstraints.enforce(constraints__10075), parentUsesSize: true);
            size = child!.size;
        }
        else
        {
            size = this._additionalConstraints.enforce(constraints__10075).constrain(Size.zero);
        }
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return (child?.getDryLayout(this._additionalConstraints.enforce(constraints)) ?? this._additionalConstraints.enforce(constraints).constrain(Size.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        base.debugPaintSize(context, offset);
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Ui.Paint paint__10749 = default!;
                if (((child is null) || child!.size.isEmpty))
                {
                    paint__10749 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(2425393296L);
    return __cascade;
}))();
                    ((PaintingContext)context).canvas.drawRect((offset & size), paint__10749);
                }
                return true;
            });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<BoxConstraints>("additionalConstraints", this.additionalConstraints));
    }

}

public class RenderLimitedBox : RenderProxyBox
{
    internal virtual double _maxWidth { get; set; } = default!;
    internal virtual double _maxHeight { get; set; } = default!;

    public RenderLimitedBox(RenderBox? child = null, double maxWidth = double.PositiveInfinity, double maxHeight = double.PositiveInfinity) : base(child)
    {
        this._maxWidth = maxWidth;
        this._maxHeight = maxHeight;
        System.Diagnostics.Debug.Assert((maxWidth >= 0.0));
        System.Diagnostics.Debug.Assert((maxHeight >= 0.0));
    }

    public virtual double maxWidth
    {
        get => this._maxWidth;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0.0));
            if ((this._maxWidth == __value))
            {
                return;
            }
            _maxWidth = __value;
            markNeedsLayout();
        }
    }
    public virtual double maxHeight
    {
        get => this._maxHeight;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0.0));
            if ((this._maxHeight == __value))
            {
                return;
            }
            _maxHeight = __value;
            markNeedsLayout();
        }
    }
    internal virtual BoxConstraints _limitConstraints(BoxConstraints constraints)
    {
        return new BoxConstraints(minWidth: ((BoxConstraints)constraints).minWidth, maxWidth: (((BoxConstraints)constraints).hasBoundedWidth ? ((BoxConstraints)constraints).maxWidth : constraints.constrainWidth(this.maxWidth)), minHeight: ((BoxConstraints)constraints).minHeight, maxHeight: (((BoxConstraints)constraints).hasBoundedHeight ? ((BoxConstraints)constraints).maxHeight : constraints.constrainHeight(this.maxHeight)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeSize(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild)
    {
        if ((child is not null))
        {
            global::Doroti.Ui.Size childSize__13568 = layoutChild(child!, _limitConstraints(constraints));
            return constraints.constrain(childSize__13568);
        }
        return _limitConstraints(constraints).constrain(Size.zero);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: constraints, layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("maxWidth", this.maxWidth, defaultValue: double.PositiveInfinity));
        properties.add(new DoubleProperty("maxHeight", this.maxHeight, defaultValue: double.PositiveInfinity));
    }

}

public class RenderAspectRatio : RenderProxyBox
{
    internal virtual double _aspectRatio { get; set; } = default!;

    public RenderAspectRatio(RenderBox? child = null, double aspectRatio = default!) : base(child)
    {
        this._aspectRatio = aspectRatio;
        System.Diagnostics.Debug.Assert((aspectRatio > 0.0));
        System.Diagnostics.Debug.Assert(double.IsFinite(aspectRatio));
    }

    public virtual double aspectRatio
    {
        get => this._aspectRatio;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value > 0.0));
            DartRuntimePrimitives.Assert(() => double.IsFinite(__value));
            if ((this._aspectRatio == __value))
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
            return (height * this._aspectRatio);
        }
        return (child?.getMinIntrinsicWidth(height) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if (double.IsFinite(height))
        {
            return (height * this._aspectRatio);
        }
        return (child?.getMaxIntrinsicWidth(height) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if (double.IsFinite(width))
        {
            return (width / this._aspectRatio);
        }
        return (child?.getMinIntrinsicHeight(width) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if (double.IsFinite(width))
        {
            return (width / this._aspectRatio);
        }
        return (child?.getMaxIntrinsicHeight(width) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _applyAspectRatio(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((!((BoxConstraints)constraints).hasBoundedWidth && !((BoxConstraints)constraints).hasBoundedHeight))
                {
                    throw new FlutterError($"{this.GetType()} has unbounded constraints.\n" + $"This {this.GetType()} was given an aspect ratio of {this.aspectRatio} but was given " + "both unbounded width and unbounded height constraints. Because both " + "constraints were unbounded, this render object doesn't know how much " + "size to consume.");
                }
                return true;
            });
        if (((BoxConstraints)constraints).isTight)
        {
            return ((BoxConstraints)constraints).smallest;
        }
        double width__18340 = ((BoxConstraints)constraints).maxWidth;
        double height__18381 = default!;
        if (double.IsFinite(width__18340))
        {
            height__18381 = (width__18340 / this._aspectRatio);
        }
        else
        {
            height__18381 = ((BoxConstraints)constraints).maxHeight;
            width__18340 = (height__18381 * this._aspectRatio);
        }
        if ((width__18340 > ((BoxConstraints)constraints).maxWidth))
        {
            width__18340 = ((BoxConstraints)constraints).maxWidth;
            height__18381 = (width__18340 / this._aspectRatio);
        }
        if ((height__18381 > ((BoxConstraints)constraints).maxHeight))
        {
            height__18381 = ((BoxConstraints)constraints).maxHeight;
            width__18340 = (height__18381 * this._aspectRatio);
        }
        if ((width__18340 < ((BoxConstraints)constraints).minWidth))
        {
            width__18340 = ((BoxConstraints)constraints).minWidth;
            height__18381 = (width__18340 / this._aspectRatio);
        }
        if ((height__18381 < ((BoxConstraints)constraints).minHeight))
        {
            height__18381 = ((BoxConstraints)constraints).minHeight;
            width__18340 = (height__18381 * this._aspectRatio);
        }
        return constraints.constrain(new global::Doroti.Ui.Size(width__18340, height__18381));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _applyAspectRatio(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        return base.computeDryBaseline(BoxConstraints.CreateTight(getDryLayout(constraints)), baseline);
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
        properties.add(new DoubleProperty("aspectRatio", this.aspectRatio));
    }

}

public class RenderIntrinsicWidth : RenderProxyBox
{
    internal virtual double? _stepWidth { get; set; } = default;
    internal virtual double? _stepHeight { get; set; } = default;

    public RenderIntrinsicWidth(double? stepWidth = null, double? stepHeight = null, RenderBox? child = null) : base(child)
    {
        this._stepWidth = stepWidth;
        this._stepHeight = stepHeight;
        System.Diagnostics.Debug.Assert(((stepWidth is null) || (DartRuntimePrimitives.RequireValue(stepWidth) > 0.0)));
        System.Diagnostics.Debug.Assert(((stepHeight is null) || (DartRuntimePrimitives.RequireValue(stepHeight) > 0.0)));
    }

    public virtual double? stepWidth
    {
        get => this._stepWidth;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || (DartRuntimePrimitives.RequireValue(__value) > 0.0)));
            if ((__value == this._stepWidth))
            {
                return;
            }
            _stepWidth = __value;
            markNeedsLayout();
        }
    }
    public virtual double? stepHeight
    {
        get => this._stepHeight;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || (DartRuntimePrimitives.RequireValue(__value) > 0.0)));
            if ((__value == this._stepHeight))
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
        if ((step is null))
        {
            return input;
        }
        return (((input / DartRuntimePrimitives.RequireValue(step))).ceil() * DartRuntimePrimitives.RequireValue(step));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return getMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if ((child is null))
        {
            return 0.0;
        }
        double width__23570 = child!.getMaxIntrinsicWidth(height);
        return _applyStep(width__23570, this._stepWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if ((child is null))
        {
            return 0.0;
        }
        if (!double.IsFinite(width))
        {
            width = getMaxIntrinsicWidth(double.PositiveInfinity);
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(width));
        double height__23905 = child!.getMinIntrinsicHeight(width);
        return _applyStep(height__23905, this._stepHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if ((child is null))
        {
            return 0.0;
        }
        if (!double.IsFinite(width))
        {
            width = getMaxIntrinsicWidth(double.PositiveInfinity);
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(width));
        double height__24243 = child!.getMaxIntrinsicHeight(width);
        return _applyStep(height__24243, this._stepHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BoxConstraints _childConstraints(RenderBox child, BoxConstraints constraints)
    {
        return constraints.tighten(width: (((BoxConstraints)constraints).hasTightWidth ? null : _applyStep(child.getMaxIntrinsicWidth(((BoxConstraints)constraints).maxHeight), this._stepWidth)), height: ((this.stepHeight is null) ? null : _applyStep(child.getMaxIntrinsicHeight(((BoxConstraints)constraints).maxWidth), this._stepHeight)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeSize(Func<RenderBox, BoxConstraints, Size> layoutChild, BoxConstraints constraints)
    {
        RenderBox? child__24863 = this.child;
        return ((child__24863 is null) ? ((BoxConstraints)constraints).smallest : layoutChild(child__24863, _childConstraints(child__24863, constraints)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, constraints: constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? child__25320 = this.child;
        return child__25320?.getDryBaseline(_childConstraints(child__25320, constraints), baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild, constraints: constraints);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("stepWidth", this.stepWidth));
        properties.add(new DoubleProperty("stepHeight", this.stepHeight));
    }

}

public class RenderIntrinsicHeight : RenderProxyBox
{
    public RenderIntrinsicHeight(RenderBox? child = null) : base(child)
    {
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        if ((child is null))
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
        if ((child is null))
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
        return (((BoxConstraints)constraints).hasTightHeight ? constraints : constraints.tighten(height: child.getMaxIntrinsicHeight(((BoxConstraints)constraints).maxWidth)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeSize(Func<RenderBox, BoxConstraints, Size> layoutChild, BoxConstraints constraints)
    {
        RenderBox? child__28568 = this.child;
        return ((child__28568 is null) ? ((BoxConstraints)constraints).smallest : layoutChild(child__28568, _childConstraints(child__28568, constraints)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, constraints: constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? child__29025 = this.child;
        return child__29025?.getDryBaseline(_childConstraints(child__29025, constraints), baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild, constraints: constraints);
    }

}

public class RenderIgnoreBaseline : RenderProxyBox
{
    public RenderIgnoreBaseline(RenderBox? child = null) : base(child)
    {
    }

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

    public RenderOpacity(double opacity = 1.0, bool alwaysIncludeSemantics = false, RenderBox? child = null) : base(child)
    {
        this._opacity = opacity;
        this._alwaysIncludeSemantics = alwaysIncludeSemantics;
        this._alpha = Dart_uiLibrary.Color.getAlphaFromOpacity(opacity);
        System.Diagnostics.Debug.Assert(((opacity >= 0.0) && (opacity <= 1.0)));
    }

    public override bool alwaysNeedsCompositing => ((child is not null) && (this._alpha > 0L));
    public override bool isRepaintBoundary => this.alwaysNeedsCompositing;
    public virtual double opacity
    {
        get => this._opacity;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value >= 0.0) && (__value <= 1.0)));
            if ((this._opacity == __value))
            {
                return;
            }
            bool didNeedCompositing__31407 = this.alwaysNeedsCompositing;
            var wasVisible__31462 = (this._alpha != 0L);
            _opacity = __value;
            _alpha = Dart_uiLibrary.Color.getAlphaFromOpacity(this._opacity);
            if ((didNeedCompositing__31407 != this.alwaysNeedsCompositing))
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsCompositedLayerUpdate();
            if (((wasVisible__31462 != ((this._alpha != 0L))) && !this.alwaysIncludeSemantics))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool alwaysIncludeSemantics
    {
        get => this._alwaysIncludeSemantics;
        set
        {
            var __value = value;
            if ((__value == this._alwaysIncludeSemantics))
            {
                return;
            }
            _alwaysIncludeSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override bool paintsChild(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => (object.Equals(__child.parent, this)));
        return (this._alpha > 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override OffsetLayer updateCompositedLayer(OffsetLayer? oldLayer)
    {
        var __oldLayer = oldLayer is null ? null : (OpacityLayer)(object)oldLayer;
        OpacityLayer layer__32488 = (__oldLayer ?? new OpacityLayer());
        layer__32488.alpha = this._alpha;
        return layer__32488;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (((child is null) || (this._alpha == 0L)))
        {
            return;
        }
        base.paint(context, offset);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (((child is not null) && (((this._alpha != 0L) || this.alwaysIncludeSemantics))))
        {
            visitor(child!);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("opacity", this.opacity));
        properties.add(new FlagProperty("alwaysIncludeSemantics", value: this.alwaysIncludeSemantics, ifTrue: "alwaysIncludeSemantics"));
    }

}

public interface RenderAnimatedOpacityMixin<T> where T : RenderObject
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

    public RenderAnimatedOpacity(Animation<double> opacity, bool alwaysIncludeSemantics = false, RenderBox? child = null) : base(child)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
    }

    public override bool isRepaintBoundary => ((child is not null) && DartRuntimePrimitives.RequireValue(this._currentlyIsRepaintBoundary));
    public override OffsetLayer updateCompositedLayer(OffsetLayer? oldLayer)
    {
        var __oldLayer = oldLayer is null ? null : (OpacityLayer)(object)oldLayer;
        OpacityLayer updatedLayer__33945 = (__oldLayer ?? new OpacityLayer());
        updatedLayer__33945.alpha = this._alpha;
        return updatedLayer__33945;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Animation<double> opacity
    {
        get => this._opacity!;
        set
        {
            var __value = value;
            if ((object.Equals(this._opacity, __value)))
            {
                return;
            }
            if ((attached && (this._opacity is not null)))
            {
                this.opacity.removeListener(this._updateOpacity);
            }
            this._opacity = __value;
            if (attached)
            {
                this.opacity.addListener(this._updateOpacity);
            }
            _updateOpacity();
        }
    }
    public virtual bool alwaysIncludeSemantics
    {
        get => DartRuntimePrimitives.RequireValue(this._alwaysIncludeSemantics);
        set
        {
            var __value = value;
            if ((__value == this._alwaysIncludeSemantics))
            {
                return;
            }
            this._alwaysIncludeSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        this.opacity.addListener(this._updateOpacity);
        _updateOpacity();
    }

    public override void detach()
    {
        this.opacity.removeListener(this._updateOpacity);
        base.detach();
    }

    public virtual void _updateOpacity()
    {
        long? oldAlpha__35811 = this._alpha;
        this._alpha = Dart_uiLibrary.Color.getAlphaFromOpacity(this.opacity.value);
        if ((oldAlpha__35811 != this._alpha))
        {
            bool? wasRepaintBoundary__35936 = this._currentlyIsRepaintBoundary;
            this._currentlyIsRepaintBoundary = (DartRuntimePrimitives.RequireValue(this._alpha) > 0L);
            if (((child is not null) && (wasRepaintBoundary__35936 != this._currentlyIsRepaintBoundary)))
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsCompositedLayerUpdate();
            if (((oldAlpha__35811 == 0L) || (this._alpha == 0L)))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }

    public override bool paintsChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderObject)child).parent, this)));
        return (this.opacity.value > 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((this._alpha == 0L))
        {
            return;
        }
        base.paint(context, offset);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (((child is not null) && (((this._alpha != 0L) || this.alwaysIncludeSemantics))))
        {
            visitor(child!);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Animation<double>>("opacity", this.opacity));
        properties.add(new FlagProperty("alwaysIncludeSemantics", value: this.alwaysIncludeSemantics, ifTrue: "alwaysIncludeSemantics"));
    }

}

public delegate Shader ShaderCallback(Rect bounds);

public class RenderShaderMask : RenderProxyBox
{
    internal virtual Func<Rect, Shader> _shaderCallback { get; set; } = default!;
    internal virtual BlendMode _blendMode { get; set; } = default!;

    public RenderShaderMask(RenderBox? child = null, Func<Rect, Shader> shaderCallback = default!, BlendMode blendMode = BlendMode.modulate) : base(child)
    {
        this._shaderCallback = shaderCallback;
        this._blendMode = blendMode;
    }

    public override ShaderMaskLayer? layer => ((ShaderMaskLayer?)(object?)base.layer)!;
    public virtual Func<Rect, Shader> shaderCallback
    {
        get => this._shaderCallback;
        set
        {
            var __value = value;
            if ((object.Equals((Func<Rect, Shader>)this._shaderCallback, (Func<Rect, Shader>)__value)))
            {
                return;
            }
            _shaderCallback = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.BlendMode blendMode
    {
        get => this._blendMode;
        set
        {
            var __value = value;
            if ((object.Equals(this._blendMode, __value)))
            {
                return;
            }
            _blendMode = __value;
            markNeedsPaint();
        }
    }
    public override bool alwaysNeedsCompositing => (child is not null);
    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null))
        {
            DartRuntimePrimitives.Assert(() => needsCompositing);
            layer ??= new ShaderMaskLayer();
            ((Func<ShaderMaskLayer>)(() =>
{
    var __cascade = this.layer!;
    __cascade.shader = this._shaderCallback((Offset.zero & size));
    __cascade.maskRect = (offset & size);
    __cascade.blendMode = this._blendMode;
    return __cascade;
}))();
            context.pushLayer(this.layer!, (Action<PaintingContext, Offset>)base.paint, offset);
            DartRuntimePrimitives.Assert(() =>
                {
                    this.layer!.debugCreator = debugCreator;
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

    public RenderBackdropFilter(RenderBox? child = null, ImageFilter? filter = null, ImageFilterConfig? filterConfig = null, BlendMode blendMode = BlendMode.srcOver, bool enabled = true, BackdropKey? backdropKey = null) : base(child)
    {
        this._filterConfig = (filterConfig ?? ImageFilterConfig.Create(filter!));
        this._enabled = enabled;
        this._blendMode = blendMode;
        this._backdropKey = backdropKey;
        System.Diagnostics.Debug.Assert(((filter is not null) || (filterConfig is not null)));
        System.Diagnostics.Debug.Assert(((filter is null) || (filterConfig is null)));
    }

    public override BackdropFilterLayer? layer => ((BackdropFilterLayer?)(object?)base.layer)!;
    public virtual bool enabled
    {
        get => this._enabled;
        set
        {
            var __value = value;
            if ((this.enabled == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _enabled = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.ImageFilter filter
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (((ImageFilterConfig)this.filterConfig).filter is not null));
            return ((ImageFilterConfig)this.filterConfig).filter!;
            return default!;
        }
        set
        {
            var __value = (ImageFilter)(object)value;
            filterConfig = ImageFilterConfig.Create(__value);
        }
    }
    public virtual ImageFilterConfig filterConfig
    {
        get => this._filterConfig;
        set
        {
            var __value = value;
            if ((object.Equals(this._filterConfig, __value)))
            {
                return;
            }
            _filterConfig = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.BlendMode blendMode
    {
        get => this._blendMode;
        set
        {
            var __value = value;
            if ((object.Equals(this._blendMode, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _blendMode = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual BackdropKey? backdropKey
    {
        get => this._backdropKey;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._backdropKey)))
            {
                return;
            }
            _backdropKey = __value;
            markNeedsPaint();
        }
    }
    public override bool alwaysNeedsCompositing => (child is not null);
    public override void paint(PaintingContext context, Offset offset)
    {
        if (!this._enabled)
        {
            base.paint(context, offset);
            return;
        }
        global::Doroti.Ui.ImageFilter effectiveFilter__44613 = this._filterConfig.resolve(new ImageFilterContext(bounds: (offset & size)));
        if ((child is not null))
        {
            DartRuntimePrimitives.Assert(() => needsCompositing);
            layer ??= new BackdropFilterLayer();
            this.layer!.filter = effectiveFilter__44613;
            this.layer!.blendMode = this._blendMode;
            this.layer!.backdropKey = this._backdropKey;
            context.pushLayer(this.layer!, (Action<PaintingContext, Offset>)base.paint, offset);
            DartRuntimePrimitives.Assert(() =>
                {
                    this.layer!.debugCreator = debugCreator;
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
        properties.add(new DiagnosticsProperty<ImageFilterConfig>("filterConfig", this.filterConfig, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Ui.BlendMode>("blendMode", this.blendMode));
        properties.add(new FlagProperty("enabled", value: this.enabled, ifTrue: "enabled"));
    }

}

public abstract class CustomClipper<T> : Listenable
{
    internal virtual Listenable? _reclip { get; private set; }

    protected CustomClipper(Listenable? reclip = null)
    {
        this._reclip = reclip;
    }

    public virtual void addListener(Action listener) => this._reclip?.addListener(listener);
    public virtual void removeListener(Action listener) => this._reclip?.removeListener(listener);
    public abstract T getClip(Size size);
    public virtual global::Doroti.Ui.Rect getApproximateClipRect(Size size) => (Offset.zero & size);
    public abstract bool shouldReclip(CustomClipper<T> oldClipper);
    public override string ToString() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "CustomClipper");
}

public class ShapeBorderClipper : CustomClipper<Path>
{
    public virtual global::Doroti.Framework.Painting.ShapeBorder shape { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }

    public ShapeBorderClipper(global::Doroti.Framework.Painting.ShapeBorder shape, TextDirection? textDirection = null)
    {
        this.shape = shape;
        this.textDirection = textDirection;
    }

    public override global::Doroti.Ui.Path getClip(Size size)
    {
        return this.shape.getOuterPath((Offset.zero & size), textDirection: this.textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldReclip(CustomClipper<Path> oldClipper)
    {
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(oldClipper), typeof(ShapeBorderClipper))))
        {
            return true;
        }
        var typedOldClipper__50451 = ((ShapeBorderClipper?)(object?)oldClipper)!;
        return ((!object.Equals(((ShapeBorderClipper)typedOldClipper__50451).shape, this.shape)) || (!object.Equals(((ShapeBorderClipper)typedOldClipper__50451).textDirection, this.textDirection)));
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
    internal virtual global::Doroti.Framework.Painting.TextPainter? _debugText { get; set; } = default;

    internal _RenderCustomClip__proxy_box(RenderBox? child = null, CustomClipper<T>? clipper = null, Clip clipBehavior = Clip.antiAlias) : base(child)
    {
        this._clipper = clipper;
        this._clipBehavior = clipBehavior;
    }

    public virtual CustomClipper<T>? clipper
    {
        get => this._clipper;
        set
        {
            var newClipper = value;
            if ((object.Equals(this._clipper, newClipper)))
            {
                return;
            }
            CustomClipper<T>? oldClipper__51130 = this._clipper;
            _clipper = newClipper;
            DartRuntimePrimitives.Assert(() => ((newClipper is not null) || (oldClipper__51130 is not null)));
            if (((((newClipper is null) || (oldClipper__51130 is null)) || (!object.Equals(DartRuntimePrimitives.RuntimeType(newClipper), DartRuntimePrimitives.RuntimeType(oldClipper__51130)))) || newClipper.shouldReclip(oldClipper__51130)))
            {
                _markNeedsClip();
            }
            if (attached)
            {
                oldClipper__51130?.removeListener(this._markNeedsClip);
                newClipper?.addListener(this._markNeedsClip);
            }
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        this._clipper?.addListener(this._markNeedsClip);
    }

    public override void detach()
    {
        this._clipper?.removeListener(this._markNeedsClip);
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
            }
        }
    }
    public override void performLayout()
    {
        global::Doroti.Ui.Size? oldSize__52169 = (hasSize ? size : null);
        base.performLayout();
        if ((!object.Equals(oldSize__52169, size)))
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
                    return (this._clipper?.getApproximateClipRect(size) ?? (Offset.zero & size));
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPaint ??= ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.shader = global::Doroti.Ui.Gradient.linear(Offset.zero, new global::Doroti.Ui.Offset(10.0, 10.0), new List<global::Doroti.Ui.Color> { new global::Doroti.Ui.Color(0L), new global::Doroti.Ui.Color(4294902015L), new global::Doroti.Ui.Color(4294902015L), new global::Doroti.Ui.Color(0L) }, new List<double> { 0.25, 0.25, 0.75, 0.75 }, TileMode.repeated);
    __cascade.strokeWidth = 2.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
                _debugText ??= ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = new global::Doroti.Framework.Painting.TextPainter(text: new global::Doroti.Framework.Painting.TextSpan(text: "✂", style: new global::Doroti.Framework.Painting.TextStyle(color: new global::Doroti.Ui.Color(4294902015L), fontSize: 14.0)), textDirection: TextDirection.rtl);
    __cascade.layout();
    return __cascade;
}))();
                return true;
            });
    }

    public override void dispose()
    {
        this._debugText?.dispose();
        _debugText = null;
        base.dispose();
    }

}

public class RenderClipRect : _RenderCustomClip__proxy_box<Rect>
{
    public RenderClipRect(RenderBox? child = null, CustomClipper<Rect>? clipper = null, Clip clipBehavior = Clip.antiAlias) : base(child: child, clipper: clipper, clipBehavior: clipBehavior)
    {
    }

    internal override global::Doroti.Ui.Rect _defaultClip => (Offset.zero & size);
    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if ((_clipper is not null))
        {
            _updateClip();
            DartRuntimePrimitives.Assert(() => !object.Equals(_clip, null));
            if (!DartRuntimePrimitives.RequireValue(_clip).contains(position))
            {
                return false;
            }
        }
        return base.hitTest(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null))
        {
            if ((!object.Equals(clipBehavior, Clip.none)))
            {
                _updateClip();
                layer = context.pushClipRect(needsCompositing, offset, DartRuntimePrimitives.RequireValue(_clip), (Action<PaintingContext, Offset>)base.paint, clipBehavior: clipBehavior, oldLayer: ((ClipRectLayer?)(object?)layer)!);
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
                if ((child is not null))
                {
                    base.debugPaintSize(context, offset);
                    if ((!object.Equals(clipBehavior, Clip.none)))
                    {
                        ((PaintingContext)context).canvas.drawRect(DartRuntimePrimitives.RequireValue(_clip).shift(offset), _debugPaint!);
                        _debugText!.paint(((PaintingContext)context).canvas, (offset + new global::Doroti.Ui.Offset((DartRuntimePrimitives.RequireValue(_clip).width / 8.0), (-DartRuntimePrimitives.RequireValue(_debugText!.text!.style!.fontSize) * 1.1))));
                    }
                }
                return true;
            });
    }

}

public class RenderClipRRect : _RenderCustomClip__proxy_box<RRect>
{
    internal virtual global::Doroti.Framework.Painting.BorderRadiusGeometry _borderRadius { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;

    public RenderClipRRect(RenderBox? child = null, global::Doroti.Framework.Painting.BorderRadiusGeometry borderRadius = default!, CustomClipper<RRect>? clipper = null, Clip clipBehavior = Clip.antiAlias, TextDirection? textDirection = null) : base(child: child, clipper: clipper, clipBehavior: clipBehavior)
    {
        global::Doroti.Framework.Painting.BorderRadiusGeometry __borderRadius = borderRadius ?? global::Doroti.Framework.Painting.BorderRadius.zero;
        this._borderRadius = __borderRadius;
        this._textDirection = textDirection;
    }

    public virtual global::Doroti.Framework.Painting.BorderRadiusGeometry borderRadius
    {
        get => this._borderRadius;
        set
        {
            var __value = value;
            if ((object.Equals(this._borderRadius, __value)))
            {
                return;
            }
            _borderRadius = __value;
            _markNeedsClip();
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
            _markNeedsClip();
        }
    }
    internal override global::Doroti.Ui.RRect _defaultClip => this._borderRadius.resolve(this.textDirection).toRRect((Offset.zero & size));
    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if ((_clipper is not null))
        {
            _updateClip();
            DartRuntimePrimitives.Assert(() => !object.Equals(_clip, null));
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
        if ((child is not null))
        {
            if ((!object.Equals(clipBehavior, Clip.none)))
            {
                _updateClip();
                layer = context.pushClipRRect(needsCompositing, offset, _clip!.outerRect, _clip!, (Action<PaintingContext, Offset>)base.paint, clipBehavior: clipBehavior, oldLayer: ((ClipRRectLayer?)(object?)layer)!);
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
                if ((child is not null))
                {
                    base.debugPaintSize(context, offset);
                    if ((!object.Equals(clipBehavior, Clip.none)))
                    {
                        ((PaintingContext)context).canvas.drawRRect(_clip!.shift(offset), _debugPaint!);
                        _debugText!.paint(((PaintingContext)context).canvas, (offset + new global::Doroti.Ui.Offset(_clip!.tlRadiusX, (-DartRuntimePrimitives.RequireValue(_debugText!.text!.style!.fontSize) * 1.1))));
                    }
                }
                return true;
            });
    }

}

public class RenderClipRSuperellipse : _RenderCustomClip__proxy_box<RSuperellipse>
{
    internal virtual global::Doroti.Framework.Painting.BorderRadiusGeometry _borderRadius { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;

    public RenderClipRSuperellipse(RenderBox? child = null, global::Doroti.Framework.Painting.BorderRadiusGeometry borderRadius = default!, CustomClipper<RSuperellipse>? clipper = null, Clip clipBehavior = Clip.antiAlias, TextDirection? textDirection = null) : base(child: child, clipper: clipper, clipBehavior: clipBehavior)
    {
        global::Doroti.Framework.Painting.BorderRadiusGeometry __borderRadius = borderRadius ?? global::Doroti.Framework.Painting.BorderRadius.zero;
        this._borderRadius = __borderRadius;
        this._textDirection = textDirection;
    }

    public virtual global::Doroti.Framework.Painting.BorderRadiusGeometry borderRadius
    {
        get => this._borderRadius;
        set
        {
            var __value = value;
            if ((object.Equals(this._borderRadius, __value)))
            {
                return;
            }
            _borderRadius = __value;
            _markNeedsClip();
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
            _markNeedsClip();
        }
    }
    internal override global::Doroti.Ui.RSuperellipse _defaultClip => this._borderRadius.resolve(this.textDirection).toRSuperellipse((Offset.zero & size));
    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if ((_clipper is not null))
        {
            _updateClip();
            DartRuntimePrimitives.Assert(() => !object.Equals(_clip, null));
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
        if ((child is not null))
        {
            if ((!object.Equals(clipBehavior, Clip.none)))
            {
                _updateClip();
                layer = context.pushClipRSuperellipse(needsCompositing, offset, _clip!.outerRect, _clip!, (Action<PaintingContext, Offset>)base.paint, clipBehavior: clipBehavior, oldLayer: ((ClipRSuperellipseLayer?)(object?)layer)!);
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
                if ((child is not null))
                {
                    base.debugPaintSize(context, offset);
                    if ((!object.Equals(clipBehavior, Clip.none)))
                    {
                        ((PaintingContext)context).canvas.drawRSuperellipse(_clip!.shift(offset), _debugPaint!);
                        _debugText!.paint(((PaintingContext)context).canvas, (offset + new global::Doroti.Ui.Offset(_clip!.tlRadiusX, (-DartRuntimePrimitives.RequireValue(_debugText!.text!.style!.fontSize) * 1.1))));
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

    public RenderClipOval(RenderBox? child = null, CustomClipper<Rect>? clipper = null, Clip clipBehavior = Clip.antiAlias) : base(child: child, clipper: clipper, clipBehavior: clipBehavior)
    {
    }

    internal virtual global::Doroti.Ui.Path _getClipPath(Rect rect)
    {
        if ((!object.Equals(rect, this._cachedRect)))
        {
            _cachedRect = rect;
            _cachedPath = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addOval(DartRuntimePrimitives.RequireValue(this._cachedRect));
    return __cascade;
}))();
        }
        return this._cachedPath;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override global::Doroti.Ui.Rect _defaultClip => (Offset.zero & size);
    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        _updateClip();
        DartRuntimePrimitives.Assert(() => !object.Equals(_clip, null));
        global::Doroti.Ui.Offset center__62845 = DartRuntimePrimitives.RequireValue(_clip).center;
        var offset__62955 = new global::Doroti.Ui.Offset((((position.dx - center__62845.dx)) / DartRuntimePrimitives.RequireValue(_clip).width), (((position.dy - center__62845.dy)) / DartRuntimePrimitives.RequireValue(_clip).height));
        if ((offset__62955.distanceSquared > 0.25))
        {
            return false;
        }
        return base.hitTest(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null))
        {
            if ((!object.Equals(clipBehavior, Clip.none)))
            {
                _updateClip();
                layer = context.pushClipPath(needsCompositing, offset, DartRuntimePrimitives.RequireValue(_clip), _getClipPath(DartRuntimePrimitives.RequireValue(_clip)), (Action<PaintingContext, Offset>)base.paint, clipBehavior: clipBehavior, oldLayer: ((ClipPathLayer?)(object?)layer)!);
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
                if ((child is not null))
                {
                    base.debugPaintSize(context, offset);
                    if ((!object.Equals(clipBehavior, Clip.none)))
                    {
                        ((PaintingContext)context).canvas.drawPath(_getClipPath(DartRuntimePrimitives.RequireValue(_clip)).shift(offset), _debugPaint!);
                        _debugText!.paint(((PaintingContext)context).canvas, (offset + new global::Doroti.Ui.Offset((((DartRuntimePrimitives.RequireValue(_clip).width - _debugText!.width)) / 2.0), (-DartRuntimePrimitives.RequireValue(_debugText!.text!.style!.fontSize) * 1.1))));
                    }
                }
                return true;
            });
    }

}

public class RenderClipPath : _RenderCustomClip__proxy_box<Path>
{
    public RenderClipPath(RenderBox? child = null, CustomClipper<Path>? clipper = null, Clip clipBehavior = Clip.antiAlias) : base(child: child, clipper: clipper, clipBehavior: clipBehavior)
    {
    }

    internal override global::Doroti.Ui.Path _defaultClip => ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect((Offset.zero & size));
    return __cascade;
}))();
    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if ((_clipper is not null))
        {
            _updateClip();
            DartRuntimePrimitives.Assert(() => !object.Equals(_clip, null));
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
        if ((child is not null))
        {
            if ((!object.Equals(clipBehavior, Clip.none)))
            {
                _updateClip();
                layer = context.pushClipPath(needsCompositing, offset, (Offset.zero & size), _clip!, (Action<PaintingContext, Offset>)base.paint, clipBehavior: clipBehavior, oldLayer: ((ClipPathLayer?)(object?)layer)!);
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
                if ((child is not null))
                {
                    base.debugPaintSize(context, offset);
                    if ((!object.Equals(clipBehavior, Clip.none)))
                    {
                        ((PaintingContext)context).canvas.drawPath(_clip!.shift(offset), _debugPaint!);
                        _debugText!.paint(((PaintingContext)context).canvas, offset);
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

    internal _RenderPhysicalModelBase__proxy_box(RenderBox? child, double elevation, Color color, Color shadowColor, Clip clipBehavior = Clip.none, CustomClipper<T>? clipper = null) : base(child: child, clipBehavior: clipBehavior, clipper: clipper)
    {
        this._elevation = elevation;
        this._color = color;
        this._shadowColor = shadowColor;
        System.Diagnostics.Debug.Assert((elevation >= 0.0));
    }

    public virtual double elevation
    {
        get => this._elevation;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0.0));
            if ((this.elevation == __value))
            {
                return;
            }
            bool didNeedCompositing__67749 = alwaysNeedsCompositing;
            _elevation = __value;
            if ((didNeedCompositing__67749 != alwaysNeedsCompositing))
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.Color shadowColor
    {
        get => this._shadowColor;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this.shadowColor, __value)))
            {
                return;
            }
            _shadowColor = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.Color color
    {
        get => this._color;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this.color, __value)))
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
        properties.add(new DoubleProperty("elevation", this.elevation));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", this.color));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", this.color));
    }

}

public class RenderPhysicalModel : _RenderPhysicalModelBase__proxy_box<RRect>
{
    internal virtual global::Doroti.Framework.Painting.BoxShape _shape { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.BorderRadius? _borderRadius { get; set; } = default;

    public RenderPhysicalModel(RenderBox? child = null, global::Doroti.Framework.Painting.BoxShape shape = BoxShape.rectangle, Clip clipBehavior = Clip.none, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, double elevation = 0.0, Color color = default!, Color shadowColor = default!) : base(child: child, clipBehavior: clipBehavior, elevation: elevation, color: color, shadowColor: shadowColor ?? new Color(0xFF000000))
    {
        this._shape = shape;
        this._borderRadius = borderRadius;
        System.Diagnostics.Debug.Assert((elevation >= 0.0));
    }

    public virtual global::Doroti.Framework.Painting.BoxShape shape
    {
        get => this._shape;
        set
        {
            var __value = value;
            if ((object.Equals(this.shape, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _shape = DartRuntimePrimitives.RequireValue(__value);
            _markNeedsClip();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius
    {
        get => this._borderRadius;
        set
        {
            var __value = value;
            if ((object.Equals(this.borderRadius, __value)))
            {
                return;
            }
            _borderRadius = __value;
            _markNeedsClip();
        }
    }
    internal override global::Doroti.Ui.RRect _defaultClip
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            global::Doroti.Ui.Rect rect__70286 = (Offset.zero & size);
            return (this._shape switch { global::Doroti.Framework.Painting.BoxShape.rectangle => ((this.borderRadius ?? global::Doroti.Framework.Painting.BorderRadius.zero)).toRRect(rect__70286), global::Doroti.Framework.Painting.BoxShape.circle => global::Doroti.Ui.RRect.fromRectXY(rect__70286, (rect__70286.width / 2L), (rect__70286.height / 2L)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if ((_clipper is not null))
        {
            _updateClip();
            DartRuntimePrimitives.Assert(() => !object.Equals(_clip, null));
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
        if ((child is null))
        {
            layer = null;
            return;
        }
        _updateClip();
        global::Doroti.Ui.RRect offsetRRect__70978 = _clip!.shift(offset);
        var paintShadows__71022 = true;
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Painting.DebugLibrary.debugDisableShadows)
                {
                    if ((elevation > 0.0))
                    {
                        ((PaintingContext)context).canvas.drawRRect(offsetRRect__70978, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = shadowColor;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = (elevation * 2.0);
    return __cascade;
}))());
                    }
                    paintShadows__71022 = false;
                }
                return true;
            });
        global::Doroti.Ui.Canvas canvas__71440 = ((PaintingContext)context).canvas;
        if (((elevation != 0.0) && paintShadows__71022))
        {
            var offsetRRectAsPath__71521 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRRect(offsetRRect__70978);
    return __cascade;
}))();
            canvas__71440.drawShadow(offsetRRectAsPath__71521, shadowColor, elevation, (color.alpha != 255L));
        }
        var usesSaveLayer__71677 = (object.Equals(clipBehavior, Clip.antiAliasWithSaveLayer));
        if (!usesSaveLayer__71677)
        {
            canvas__71440.drawRRect(offsetRRect__70978, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = color;
    return __cascade;
}))());
        }
        layer = context.pushClipRRect(needsCompositing, offset, (Offset.zero & size), _clip!, ((Action<PaintingContext, Offset>)((context, offset) =>
        {
            if (usesSaveLayer__71677)
            {
                ((PaintingContext)context).canvas.drawPaint(((Func<Paint>)(() =>
            {
                var __cascade = new global::Doroti.Ui.Paint();
                __cascade.color = color;
                return __cascade;
            }))());
            }
            base.paint(context, offset);
        })), oldLayer: ((ClipRRectLayer?)(object?)layer)!, clipBehavior: clipBehavior);
        DartRuntimePrimitives.Assert(() =>
            {
                layer?.debugCreator = debugCreator;
                return true;
            });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.BoxShape>("shape", this.shape));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.BorderRadius>("borderRadius", this.borderRadius));
    }

}

public class RenderPhysicalShape : _RenderPhysicalModelBase__proxy_box<Path>
{
    public RenderPhysicalShape(RenderBox? child = null, CustomClipper<Path> clipper = default!, Clip clipBehavior = Clip.none, double elevation = 0.0, Color color = default!, Color shadowColor = default!) : base(child: child, clipper: clipper, clipBehavior: clipBehavior, elevation: elevation, color: color, shadowColor: shadowColor ?? new Color(0xFF000000))
    {
        System.Diagnostics.Debug.Assert((elevation >= 0.0));
    }

    internal override global::Doroti.Ui.Path _defaultClip => ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect((Offset.zero & size));
    return __cascade;
}))();
    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if ((_clipper is not null))
        {
            _updateClip();
            DartRuntimePrimitives.Assert(() => !object.Equals(_clip, null));
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
        if ((child is null))
        {
            layer = null;
            return;
        }
        _updateClip();
        global::Doroti.Ui.Path offsetPath__74160 = _clip!.shift(offset);
        var paintShadows__74203 = true;
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Painting.DebugLibrary.debugDisableShadows)
                {
                    if ((elevation > 0.0))
                    {
                        ((PaintingContext)context).canvas.drawPath(offsetPath__74160, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = shadowColor;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = (elevation * 2.0);
    return __cascade;
}))());
                    }
                    paintShadows__74203 = false;
                }
                return true;
            });
        global::Doroti.Ui.Canvas canvas__74619 = ((PaintingContext)context).canvas;
        if (((elevation != 0.0) && paintShadows__74203))
        {
            canvas__74619.drawShadow(offsetPath__74160, shadowColor, elevation, (color.alpha != 255L));
        }
        var usesSaveLayer__74786 = (object.Equals(clipBehavior, Clip.antiAliasWithSaveLayer));
        if (!usesSaveLayer__74786)
        {
            canvas__74619.drawPath(offsetPath__74160, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = color;
    return __cascade;
}))());
        }
        layer = context.pushClipPath(needsCompositing, offset, (Offset.zero & size), _clip!, ((Action<PaintingContext, Offset>)((context, offset) =>
        {
            if (usesSaveLayer__74786)
            {
                ((PaintingContext)context).canvas.drawPaint(((Func<Paint>)(() =>
            {
                var __cascade = new global::Doroti.Ui.Paint();
                __cascade.color = color;
                return __cascade;
            }))());
            }
            base.paint(context, offset);
        })), oldLayer: ((ClipPathLayer?)(object?)layer)!, clipBehavior: clipBehavior);
        DartRuntimePrimitives.Assert(() =>
            {
                layer?.debugCreator = debugCreator;
                return true;
            });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<CustomClipper<global::Doroti.Ui.Path>>("clipper", clipper));
    }

}

public enum DecorationPosition
{
    background,
    foreground
}

public class RenderDecoratedBox : RenderProxyBox
{
    internal virtual global::Doroti.Framework.Painting.BoxPainter? _painter { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.Decoration _decoration { get; set; } = default!;
    internal virtual DecorationPosition _position { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.ImageConfiguration _configuration { get; set; } = default!;

    public RenderDecoratedBox(global::Doroti.Framework.Painting.Decoration decoration, DecorationPosition position = DecorationPosition.background, global::Doroti.Framework.Painting.ImageConfiguration configuration = default!, RenderBox? child = null) : base(child)
    {
        global::Doroti.Framework.Painting.ImageConfiguration __configuration = configuration ?? global::Doroti.Framework.Painting.ImageConfiguration.empty;
        this._decoration = decoration;
        this._position = position;
        this._configuration = __configuration;
    }

    public virtual global::Doroti.Framework.Painting.Decoration decoration
    {
        get => this._decoration;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._decoration)))
            {
                return;
            }
            this._painter?.dispose();
            _painter = null;
            _decoration = __value;
            markNeedsPaint();
        }
    }
    public virtual DecorationPosition position
    {
        get => this._position;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._position)))
            {
                return;
            }
            _position = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Framework.Painting.ImageConfiguration configuration
    {
        get => this._configuration;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._configuration)))
            {
                return;
            }
            _configuration = __value;
            markNeedsPaint();
        }
    }
    public override void detach()
    {
        this._painter?.dispose();
        _painter = null;
        base.detach();
        markNeedsPaint();
    }

    public override void dispose()
    {
        this._painter?.dispose();
        base.dispose();
    }

    public override bool hitTestSelf(Offset position)
    {
        return this._decoration.hitTest(size, position, textDirection: ((global::Doroti.Framework.Painting.ImageConfiguration)this.configuration).textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        _painter ??= this._decoration.createBoxPainter((Action)markNeedsPaint);
        global::Doroti.Framework.Painting.ImageConfiguration filledConfiguration__78964 = this.configuration.copyWith(size: size);
        if ((object.Equals(this.position, DecorationPosition.background)))
        {
            long? debugSaveCount__79086 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    debugSaveCount__79086 = ((PaintingContext)context).canvas.getSaveCount();
                    return true;
                });
            this._painter!.paint(((PaintingContext)context).canvas, offset, filledConfiguration__78964);
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((debugSaveCount__79086 != ((PaintingContext)context).canvas.getSaveCount()))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{DartRuntimePrimitives.RuntimeType(this._decoration)} painter had mismatching save and restore calls."), new ErrorDescription($"Before painting the decoration, the canvas save count was {debugSaveCount__79086}. " + $"After painting it, the canvas save count was {((PaintingContext)context).canvas.getSaveCount()}. " + "Every call to save() or saveLayer() must be matched by a call to restore()."), new DiagnosticsProperty<global::Doroti.Framework.Painting.Decoration>("The decoration was", this.decoration, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<global::Doroti.Framework.Painting.BoxPainter>("The painter was", this._painter, style: DiagnosticsTreeStyle.errorProperty) });
                    }
                    return true;
                });
            if (((global::Doroti.Framework.Painting.Decoration)this.decoration).isComplex)
            {
                context.setIsComplexHint();
            }
        }
        base.paint(context, offset);
        if ((object.Equals(this.position, DecorationPosition.foreground)))
        {
            this._painter!.paint(((PaintingContext)context).canvas, offset, filledConfiguration__78964);
            if (((global::Doroti.Framework.Painting.Decoration)this.decoration).isComplex)
            {
                context.setIsComplexHint();
            }
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(((Diagnosticable)this._decoration).toDiagnosticsNode(name: "decoration"));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.ImageConfiguration>("configuration", this.configuration));
    }

}

public class RenderTransform : RenderProxyBox
{
    internal virtual Offset? _origin { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.AlignmentGeometry? _alignment { get; set; } = default;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    public virtual bool transformHitTests { get; set; } = default!;
    internal virtual Matrix4? _transform { get; set; } = default;
    internal virtual FilterQuality? _filterQuality { get; set; } = default;

    public RenderTransform(Matrix4 transform, Offset? origin = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, TextDirection? textDirection = null, bool transformHitTests = true, FilterQuality? filterQuality = null, RenderBox? child = null) : base(child)
    {
        this._transform = Matrix4.copy(transform);
        this._origin = origin;
        this._alignment = alignment;
        this._textDirection = textDirection;
        this.transformHitTests = transformHitTests;
        this._filterQuality = filterQuality;
    }

    public virtual global::Doroti.Ui.Offset? origin
    {
        get => this._origin;
        set
        {
            var __value = value;
            if ((object.Equals(this._origin, __value)))
            {
                return;
            }
            _origin = __value;
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment
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
            markNeedsPaint();
            markNeedsSemanticsUpdate();
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
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }
    public override bool alwaysNeedsCompositing => ((child is not null) && (this._filterQuality is not null));
    public virtual Matrix4 transform
    {
        set
        {
            var __value = value;
            if ((object.Equals(this._transform, __value)))
            {
                return;
            }
            _transform = Matrix4.copy(__value);
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual global::Doroti.Ui.FilterQuality? filterQuality
    {
        get => this._filterQuality;
        set
        {
            var __value = value;
            if ((object.Equals(this._filterQuality, __value)))
            {
                return;
            }
            bool didNeedCompositing__84785 = this.alwaysNeedsCompositing;
            _filterQuality = __value;
            if ((didNeedCompositing__84785 != this.alwaysNeedsCompositing))
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsPaint();
        }
    }
    public virtual void setIdentity()
    {
        this._transform!.setIdentity();
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    public virtual void rotateX(double radians)
    {
        this._transform!.rotateX(radians);
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    public virtual void rotateY(double radians)
    {
        this._transform!.rotateY(radians);
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    public virtual void rotateZ(double radians)
    {
        this._transform!.rotateZ(radians);
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    public virtual void translate(double x, double y = 0.0, double z = 0.0)
    {
        this._transform!.translateByDouble(x, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(y)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(z)), 1);
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    public virtual void scale(double x, double? y = null, double? z = null)
    {
        this._transform!.scaleByDouble(x, (y ?? x), (z ?? x), 1);
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    internal virtual Matrix4? _effectiveTransform
    {
        get
        {
            global::Doroti.Framework.Painting.Alignment? resolvedAlignment__86229 = this.alignment?.resolve(this.textDirection);
            if (((this._origin is null) && (resolvedAlignment__86229 is null)))
            {
                return this._transform;
            }
            var result__86381 = Matrix4.identity();
            if ((this._origin is not null))
            {
                result__86381.translateByDouble(DartRuntimePrimitives.RequireValue(this._origin).dx, DartRuntimePrimitives.RequireValue(this._origin).dy, 0, 1);
            }
            global::Doroti.Ui.Offset? translation__86519 = default!;
            if ((resolvedAlignment__86229 is not null))
            {
                translation__86519 = resolvedAlignment__86229.alongSize(size);
                result__86381.translateByDouble(DartRuntimePrimitives.RequireValue(translation__86519).dx, DartRuntimePrimitives.RequireValue(translation__86519).dy, 0, 1);
            }
            result__86381.multiply(this._transform!);
            if ((resolvedAlignment__86229 is not null))
            {
                result__86381.translateByDouble(-DartRuntimePrimitives.RequireValue(translation__86519).dx, -DartRuntimePrimitives.RequireValue(translation__86519).dy, 0, 1);
            }
            if ((this._origin is not null))
            {
                result__86381.translateByDouble(-DartRuntimePrimitives.RequireValue(this._origin).dx, -DartRuntimePrimitives.RequireValue(this._origin).dy, 0, 1);
            }
            return result__86381;
            return default!;
        }
    }
    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        return hitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        DartRuntimePrimitives.Assert(() => (!this.transformHitTests || (this._effectiveTransform is not null)));
        return result.addWithPaintTransform(transform: (this.transformHitTests ? this._effectiveTransform : null), position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, position) =>
        {
            return base.hitTestChildren(result, position: position);
            return default;
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null))
        {
            Matrix4 transform__87864 = this._effectiveTransform!;
            if ((this.filterQuality is null))
            {
                global::Doroti.Ui.Offset? childOffset__87955 = MatrixUtils.getAsTranslation(transform__87864);
                if ((childOffset__87955 is null))
                {
                    double det__88221 = transform__87864.determinant;
                    if (((det__88221 == 0L) || !double.IsFinite(det__88221)))
                    {
                        layer = null;
                        return;
                    }
                    layer = context.pushTransform(needsCompositing, offset, transform__87864, (Action<PaintingContext, Offset>)base.paint, oldLayer: ((layer is TransformLayer) ? ((TransformLayer?)(object?)layer)! : null));
                }
                else
                {
                    base.paint(context, (offset + DartRuntimePrimitives.RequireValue(childOffset__87955)));
                    layer = null;
                }
            }
            else
            {
                var effectiveTransform__88720 = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.translationValues(offset.dx, offset.dy, 0.0);
    __cascade.multiply(transform__87864);
    __cascade.translateByDouble(-offset.dx, -offset.dy, 0, 1);
    return __cascade;
}))();
                var filter__88901 = new global::Doroti.Ui.ImageFilter(effectiveTransform__88720.storage, filterQuality: DartRuntimePrimitives.RequireValue(this.filterQuality));
                if (layer is ImageFilterLayer filterLayer__89069)
                {
                    filterLayer__89069.imageFilter = filter__88901;
                }
                else
                {
                    layer = new ImageFilterLayer(imageFilter: filter__88901);
                }
                context.pushLayer(layer!, (Action<PaintingContext, Offset>)base.paint, offset);
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
        var __child = (RenderBox)(object)child;
        transform.multiply(this._effectiveTransform!);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Painting.TransformProperty("transform matrix", this._transform));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("origin", this.origin));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("transformHitTests", this.transformHitTests));
    }

}

public class RenderFittedBox : RenderProxyBox
{
    internal virtual global::Doroti.Framework.Painting.Alignment? _resolvedAlignment { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.BoxFit _fit { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.AlignmentGeometry _alignment { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual bool? _hasVisualOverflow { get; set; } = default;
    internal virtual Matrix4? _transform { get; set; } = default;
    internal virtual Clip _clipBehavior { get; set; } = Clip.none;

    public RenderFittedBox(global::Doroti.Framework.Painting.BoxFit fit = BoxFit.contain, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, TextDirection? textDirection = null, RenderBox? child = null, Clip clipBehavior = Clip.none) : base(child)
    {
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Framework.Painting.Alignment.center;
        this._fit = fit;
        this._alignment = __alignment;
        this._textDirection = textDirection;
        this._clipBehavior = clipBehavior;
    }

    internal virtual global::Doroti.Framework.Painting.Alignment _resolve() => _resolvedAlignment ??= this.alignment.resolve(this.textDirection);
    internal virtual void _markNeedResolution()
    {
        _resolvedAlignment = null;
        markNeedsPaint();
    }

    internal virtual bool _fitAffectsLayout(global::Doroti.Framework.Painting.BoxFit fit)
    {
        switch (fit)
        {
            case global::Doroti.Framework.Painting.BoxFit.scaleDown:
                {
                    return true;
                }
            case global::Doroti.Framework.Painting.BoxFit.contain:
            case global::Doroti.Framework.Painting.BoxFit.cover:
            case global::Doroti.Framework.Painting.BoxFit.fill:
            case global::Doroti.Framework.Painting.BoxFit.fitHeight:
            case global::Doroti.Framework.Painting.BoxFit.fitWidth:
            case global::Doroti.Framework.Painting.BoxFit.none:
                {
                    return false;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Painting.BoxFit fit
    {
        get => this._fit;
        set
        {
            var __value = value;
            if ((object.Equals(this._fit, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            global::Doroti.Framework.Painting.BoxFit lastFit__91276 = this._fit;
            _fit = DartRuntimePrimitives.RequireValue(__value);
            if ((_fitAffectsLayout(lastFit__91276) || _fitAffectsLayout(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(__value)))))
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
            _clearPaintData();
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
            _clearPaintData();
            _markNeedResolution();
        }
    }
    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if ((child is not null))
        {
            global::Doroti.Ui.Size childSize__92776 = child!.getDryLayout(new BoxConstraints());
            switch (this.fit)
            {
                case global::Doroti.Framework.Painting.BoxFit.scaleDown:
                    {
                        BoxConstraints sizeConstraints__92917 = constraints.loosen();
                        global::Doroti.Ui.Size unconstrainedSize__92978 = sizeConstraints__92917.constrainSizeAndAttemptToPreserveAspectRatio(childSize__92776);
                        return constraints.constrain(unconstrainedSize__92978);
                    }
                case global::Doroti.Framework.Painting.BoxFit.contain:
                case global::Doroti.Framework.Painting.BoxFit.cover:
                case global::Doroti.Framework.Painting.BoxFit.fill:
                case global::Doroti.Framework.Painting.BoxFit.fitHeight:
                case global::Doroti.Framework.Painting.BoxFit.fitWidth:
                case global::Doroti.Framework.Painting.BoxFit.none:
                    {
                        return constraints.constrainSizeAndAttemptToPreserveAspectRatio(childSize__92776);
                    }
            }
        }
        else
        {
            return ((BoxConstraints)constraints).smallest;
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
        if ((child is not null))
        {
            child!.layout(new BoxConstraints(), parentUsesSize: true);
            switch (this.fit)
            {
                case global::Doroti.Framework.Painting.BoxFit.scaleDown:
                    {
                        BoxConstraints sizeConstraints__94010 = constraints.loosen();
                        global::Doroti.Ui.Size unconstrainedSize__94071 = sizeConstraints__94010.constrainSizeAndAttemptToPreserveAspectRatio(child!.size);
                        size = constraints.constrain(unconstrainedSize__94071);
                        break;
                    }
                case global::Doroti.Framework.Painting.BoxFit.contain:
                case global::Doroti.Framework.Painting.BoxFit.cover:
                case global::Doroti.Framework.Painting.BoxFit.fill:
                case global::Doroti.Framework.Painting.BoxFit.fitHeight:
                case global::Doroti.Framework.Painting.BoxFit.fitWidth:
                case global::Doroti.Framework.Painting.BoxFit.none:
                    {
                        size = constraints.constrainSizeAndAttemptToPreserveAspectRatio(child!.size);
                        break;
                    }
            }
            _clearPaintData();
        }
        else
        {
            size = ((BoxConstraints)constraints).smallest;
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
    internal virtual void _clearPaintData()
    {
        _hasVisualOverflow = null;
        _transform = null;
    }

    internal virtual void _updatePaintData()
    {
        if ((this._transform is not null))
        {
            return;
        }
        if ((child is null))
        {
            _hasVisualOverflow = false;
            _transform = Matrix4.identity();
        }
        else
        {
            global::Doroti.Framework.Painting.Alignment resolvedAlignment__95271 = _resolve();
            global::Doroti.Ui.Size childSize__95320 = child!.size;
            global::Doroti.Framework.Painting.FittedSizes sizes__95369 = global::Doroti.Framework.Painting.Box_fitLibrary.applyBoxFit(this._fit, childSize__95320, size);
            double scaleX__95432 = (((global::Doroti.Framework.Painting.FittedSizes)sizes__95369).destination.width / ((global::Doroti.Framework.Painting.FittedSizes)sizes__95369).source.width);
            double scaleY__95506 = (((global::Doroti.Framework.Painting.FittedSizes)sizes__95369).destination.height / ((global::Doroti.Framework.Painting.FittedSizes)sizes__95369).source.height);
            global::Doroti.Ui.Rect sourceRect__95580 = resolvedAlignment__95271.inscribe(((global::Doroti.Framework.Painting.FittedSizes)sizes__95369).source, (Offset.zero & childSize__95320));
            global::Doroti.Ui.Rect destinationRect__95677 = resolvedAlignment__95271.inscribe(((global::Doroti.Framework.Painting.FittedSizes)sizes__95369).destination, (Offset.zero & size));
            _hasVisualOverflow = ((sourceRect__95580.width < childSize__95320.width) || (sourceRect__95580.height < childSize__95320.height));
            DartRuntimePrimitives.Assert(() => (double.IsFinite(scaleX__95432) && double.IsFinite(scaleY__95506)));
            _transform = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.translationValues(destinationRect__95677.left, destinationRect__95677.top, 0.0);
    __cascade.scaleByDouble(scaleX__95432, scaleY__95506, 1.0, 1);
    __cascade.translateByDouble(-sourceRect__95580.left, -sourceRect__95580.top, 0, 1);
    return __cascade;
}))();
            DartRuntimePrimitives.Assert(() => this._transform!.storage.All(((value) => double.IsFinite(DartRuntimePrimitives.RequireValue(value)))));
        }
    }

    internal virtual TransformLayer? _paintChildWithTransform(PaintingContext context, Offset offset)
    {
        global::Doroti.Ui.Offset? childOffset__96350 = MatrixUtils.getAsTranslation(this._transform!);
        if ((childOffset__96350 is null))
        {
            return context.pushTransform(needsCompositing, offset, this._transform!, (Action<PaintingContext, Offset>)base.paint, oldLayer: ((layer is TransformLayer) ? ((TransformLayer?)(object?)layer!)! : null));
        }
        else
        {
            base.paint(context, (offset + DartRuntimePrimitives.RequireValue(childOffset__96350)));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((((child is null) || size.isEmpty) || child!.size.isEmpty))
        {
            return;
        }
        _updatePaintData();
        DartRuntimePrimitives.Assert(() => (child is not null));
        if ((DartRuntimePrimitives.RequireValue(this._hasVisualOverflow) && (!object.Equals(this.clipBehavior, Clip.none))))
        {
            layer = context.pushClipRect(needsCompositing, offset, (Offset.zero & size), (__arg0, __arg1) => { _ = ((Func<PaintingContext, Offset, TransformLayer?>)this._paintChildWithTransform)(__arg0, __arg1); }, oldLayer: ((layer is ClipRectLayer) ? ((ClipRectLayer?)(object?)layer!)! : null), clipBehavior: this.clipBehavior);
        }
        else
        {
            layer = _paintChildWithTransform(context, offset);
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        if ((size.isEmpty || ((child?.size.isEmpty ?? false))))
        {
            return false;
        }
        _updatePaintData();
        return result.addWithPaintTransform(transform: this._transform, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, position) =>
        {
            return base.hitTestChildren(result, position: position);
            return default;
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool paintsChild(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => (object.Equals(__child.parent, this)));
        return (!size.isEmpty && !((RenderBox)__child).size.isEmpty);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
        if (!paintsChild(__child))
        {
            transform.setZero();
        }
        else
        {
            _updatePaintData();
            transform.multiply(this._transform!);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.BoxFit>("fit", this.fit));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
    }

}

public class RenderFractionalTranslation : RenderProxyBox
{
    internal virtual Offset _translation { get; set; } = default!;
    public virtual bool transformHitTests { get; set; } = default!;

    public RenderFractionalTranslation(Offset translation, bool transformHitTests = true, RenderBox? child = null) : base(child)
    {
        this.transformHitTests = transformHitTests;
        this._translation = translation;
    }

    public virtual global::Doroti.Ui.Offset translation
    {
        get => this._translation;
        set
        {
            var __value = value;
            if ((object.Equals(this._translation, __value)))
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
        return result.addWithPaintOffset(offset: (this.transformHitTests ? new global::Doroti.Ui.Offset((this.translation.dx * size.width), (this.translation.dy * size.height)) : null), position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, position) =>
        {
            return base.hitTestChildren(result, position: position);
            return default;
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        if ((child is not null))
        {
            base.paint(context, new global::Doroti.Ui.Offset((offset.dx + (this.translation.dx * size.width)), (offset.dy + (this.translation.dy * size.height))));
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
        transform.translateByDouble((this.translation.dx * size.width), (this.translation.dy * size.height), 0, 1);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("translation", this.translation));
        properties.add(new DiagnosticsProperty<bool>("transformHitTests", this.transformHitTests));
    }

}

public delegate void PointerDownEventListener(global::Doroti.Framework.Gestures.PointerDownEvent @event);

public delegate void PointerMoveEventListener(global::Doroti.Framework.Gestures.PointerMoveEvent @event);

public delegate void PointerUpEventListener(global::Doroti.Framework.Gestures.PointerUpEvent @event);

public delegate void PointerCancelEventListener(global::Doroti.Framework.Gestures.PointerCancelEvent @event);

public delegate void PointerPanZoomStartEventListener(global::Doroti.Framework.Gestures.PointerPanZoomStartEvent @event);

public delegate void PointerPanZoomUpdateEventListener(global::Doroti.Framework.Gestures.PointerPanZoomUpdateEvent @event);

public delegate void PointerPanZoomEndEventListener(global::Doroti.Framework.Gestures.PointerPanZoomEndEvent @event);

public delegate void PointerSignalEventListener(global::Doroti.Framework.Gestures.PointerSignalEvent @event);

public class RenderPointerListener : RenderProxyBoxWithHitTestBehavior
{
    public virtual Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onPointerDown { get; set; } = default;
    public virtual Action<global::Doroti.Framework.Gestures.PointerMoveEvent>? onPointerMove { get; set; } = default;
    public virtual Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onPointerUp { get; set; } = default;
    public virtual Action<global::Doroti.Framework.Gestures.PointerHoverEvent>? onPointerHover { get; set; } = default;
    public virtual Action<global::Doroti.Framework.Gestures.PointerCancelEvent>? onPointerCancel { get; set; } = default;
    public virtual Action<global::Doroti.Framework.Gestures.PointerPanZoomStartEvent>? onPointerPanZoomStart { get; set; } = default;
    public virtual Action<global::Doroti.Framework.Gestures.PointerPanZoomUpdateEvent>? onPointerPanZoomUpdate { get; set; } = default;
    public virtual Action<global::Doroti.Framework.Gestures.PointerPanZoomEndEvent>? onPointerPanZoomEnd { get; set; } = default;
    public virtual Action<global::Doroti.Framework.Gestures.PointerSignalEvent>? onPointerSignal { get; set; } = default;

    public RenderPointerListener(Action<global::Doroti.Framework.Gestures.PointerDownEvent>? onPointerDown = null, Action<global::Doroti.Framework.Gestures.PointerMoveEvent>? onPointerMove = null, Action<global::Doroti.Framework.Gestures.PointerUpEvent>? onPointerUp = null, Action<global::Doroti.Framework.Gestures.PointerHoverEvent>? onPointerHover = null, Action<global::Doroti.Framework.Gestures.PointerCancelEvent>? onPointerCancel = null, Action<global::Doroti.Framework.Gestures.PointerPanZoomStartEvent>? onPointerPanZoomStart = null, Action<global::Doroti.Framework.Gestures.PointerPanZoomUpdateEvent>? onPointerPanZoomUpdate = null, Action<global::Doroti.Framework.Gestures.PointerPanZoomEndEvent>? onPointerPanZoomEnd = null, Action<global::Doroti.Framework.Gestures.PointerSignalEvent>? onPointerSignal = null, HitTestBehavior behavior = HitTestBehavior.deferToChild, RenderBox? child = null) : base(behavior: behavior, child: child)
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
        return ((BoxConstraints)constraints).biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        switch (@event)
        {
            case global::Doroti.Framework.Gestures.PointerDownEvent __object105825:
                this.onPointerDown?.Invoke(__object105825);
                break;
            case global::Doroti.Framework.Gestures.PointerMoveEvent __object105881:
                this.onPointerMove?.Invoke(__object105881);
                break;
            case global::Doroti.Framework.Gestures.PointerUpEvent __object105937:
                this.onPointerUp?.Invoke(__object105937);
                break;
            case global::Doroti.Framework.Gestures.PointerHoverEvent __object105989:
                this.onPointerHover?.Invoke(__object105989);
                break;
            case global::Doroti.Framework.Gestures.PointerCancelEvent __object106047:
                this.onPointerCancel?.Invoke(__object106047);
                break;
            case global::Doroti.Framework.Gestures.PointerPanZoomStartEvent __object106107:
                this.onPointerPanZoomStart?.Invoke(__object106107);
                break;
            case global::Doroti.Framework.Gestures.PointerPanZoomUpdateEvent __object106179:
                this.onPointerPanZoomUpdate?.Invoke(__object106179);
                break;
            case global::Doroti.Framework.Gestures.PointerPanZoomEndEvent __object106253:
                this.onPointerPanZoomEnd?.Invoke(__object106253);
                break;
            case global::Doroti.Framework.Gestures.PointerSignalEvent __object106321:
                this.onPointerSignal?.Invoke(__object106321);
                break;
            default:
                break;
        }
        return;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagsSummary<Delegate?>("listeners", new DartMap<string, Delegate?> { ["down"] = this.onPointerDown, ["move"] = this.onPointerMove, ["up"] = this.onPointerUp, ["hover"] = this.onPointerHover, ["cancel"] = this.onPointerCancel, ["panZoomStart"] = this.onPointerPanZoomStart, ["panZoomUpdate"] = this.onPointerPanZoomUpdate, ["panZoomEnd"] = this.onPointerPanZoomEnd, ["signal"] = this.onPointerSignal }, ifEmpty: "<none>"));
    }

}

public class RenderMouseRegion : RenderProxyBoxWithHitTestBehavior, global::Doroti.Framework.Services.IMouseTrackerAnnotation
{
    internal virtual bool _opaque { get; set; } = default!;
    public virtual Action<global::Doroti.Framework.Gestures.PointerEnterEvent>? onEnter { get; set; } = default;
    public virtual Action<global::Doroti.Framework.Gestures.PointerHoverEvent>? onHover { get; set; } = default;
    public virtual Action<global::Doroti.Framework.Gestures.PointerExitEvent>? onExit { get; set; } = default;
    internal virtual MouseCursor _cursor { get; set; } = default!;
    internal virtual bool _validForMouseTracker { get; set; } = default!;
    dynamic global::Doroti.Framework.Services.IMouseTrackerAnnotation.onEnter => this.onEnter;
    dynamic global::Doroti.Framework.Services.IMouseTrackerAnnotation.onExit => this.onExit;

    public RenderMouseRegion(Action<global::Doroti.Framework.Gestures.PointerEnterEvent>? onEnter = null, Action<global::Doroti.Framework.Gestures.PointerHoverEvent>? onHover = null, Action<global::Doroti.Framework.Gestures.PointerExitEvent>? onExit = null, MouseCursor cursor = default!, bool validForMouseTracker = true, bool opaque = true, RenderBox? child = null, HitTestBehavior? hitTestBehavior = HitTestBehavior.opaque) : base(child: child, behavior: (hitTestBehavior ?? HitTestBehavior.opaque))
    {
        MouseCursor __cursor = cursor ?? MouseCursor.defer;
        this.onEnter = onEnter;
        this.onHover = onHover;
        this.onExit = onExit;
        this._cursor = __cursor;
        this._validForMouseTracker = validForMouseTracker;
        this._opaque = opaque;
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        return (base.hitTest(result, position: position) && this._opaque);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if ((@event is global::Doroti.Framework.Gestures.PointerHoverEvent))
        {
            global::Doroti.Framework.Gestures.PointerHoverEvent @event__as108765 = (global::Doroti.Framework.Gestures.PointerHoverEvent)@event;
            this.onHover?.Invoke(@event__as108765);
        }
    }

    public virtual bool opaque
    {
        get => this._opaque;
        set
        {
            var __value = value;
            if ((this._opaque != DartRuntimePrimitives.RequireValue(__value)))
            {
                _opaque = DartRuntimePrimitives.RequireValue(__value);
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
            HitTestBehavior newValue__110004 = (__value ?? HitTestBehavior.opaque);
            if ((!object.Equals(behavior, newValue__110004)))
            {
                behavior = newValue__110004;
                markNeedsPaint();
            }
        }
    }
    public virtual MouseCursor cursor
    {
        get => this._cursor;
        set
        {
            var __value = value;
            if ((!object.Equals(this._cursor, __value)))
            {
                _cursor = __value;
                markNeedsPaint();
            }
        }
    }
    public virtual bool validForMouseTracker => this._validForMouseTracker;
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
        return ((BoxConstraints)constraints).biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagsSummary<Delegate?>("listeners", new DartMap<string, Delegate?> { ["enter"] = this.onEnter, ["hover"] = this.onHover, ["exit"] = this.onExit }, ifEmpty: "<none>"));
        properties.add(new DiagnosticsProperty<MouseCursor>("cursor", this.cursor, defaultValue: MouseCursor.defer));
        properties.add(new DiagnosticsProperty<bool>("opaque", this.opaque, defaultValue: true));
        properties.add(new FlagProperty("validForMouseTracker", value: this.validForMouseTracker, defaultValue: true, ifFalse: "invalid for MouseTracker"));
    }

}

public class RenderRepaintBoundary : RenderProxyBox
{
    internal virtual long _debugSymmetricPaintCount { get; set; } = 0L;
    internal virtual long _debugAsymmetricPaintCount { get; set; } = 0L;

    public RenderRepaintBoundary(RenderBox? child = null) : base(child)
    {
    }

    public override bool isRepaintBoundary => true;
    public virtual Future<global::Doroti.Ui.Image> toImage(double pixelRatio = 1.0)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsPaint);
        var offsetLayer__116270 = ((OffsetLayer?)(object?)layer!)!;
        return offsetLayer__116270.toImage((Offset.zero & size), pixelRatio: pixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Image toImageSync(double pixelRatio = 1.0)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsPaint);
        var offsetLayer__118747 = ((OffsetLayer?)(object?)layer!)!;
        return offsetLayer__118747.toImageSync((Offset.zero & size), pixelRatio: pixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long debugSymmetricPaintCount => this._debugSymmetricPaintCount;
    public virtual long debugAsymmetricPaintCount => this._debugAsymmetricPaintCount;
    public virtual void debugResetMetrics()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugSymmetricPaintCount = 0L;
                _debugAsymmetricPaintCount = 0L;
                return true;
            });
    }

    public override void debugRegisterRepaintBoundaryPaint(bool includedParent = true, bool includedChild = false)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((includedParent && includedChild))
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
        var inReleaseMode__121161 = true;
        DartRuntimePrimitives.Assert(() =>
            {
                inReleaseMode__121161 = false;
                long totalPaints__121244 = (this.debugSymmetricPaintCount + this.debugAsymmetricPaintCount);
                if ((totalPaints__121244 == 0L))
                {
                    properties.add(new MessageProperty("usefulness ratio", "no metrics collected yet (never painted)"));
                }
                else
                {
                    double fraction__121504 = (this.debugAsymmetricPaintCount / totalPaints__121244);
                    string diagnosis__121577 = (fraction__121504 switch { _ when (totalPaints__121244 < 5L) => "insufficient data to draw conclusion (less than five repaints)", > 0.9 => "this is an outstandingly useful repaint boundary and should definitely be kept", > 0.5 => "this is a useful repaint boundary and should be kept", > 0.3 => "this repaint boundary is probably useful, but maybe it would be more useful in tandem with adding more repaint boundaries elsewhere", > 0.1 => "this repaint boundary does sometimes show value, though currently not that often", _ when (this.debugAsymmetricPaintCount > 0L) => "this repaint boundary is not very effective and should probably be removed", _ => "this repaint boundary is astoundingly ineffectual and should be removed" });
                    properties.add(new PercentProperty("metrics", fraction__121504, unit: "useful", tooltip: $"{this.debugSymmetricPaintCount} bad vs {this.debugAsymmetricPaintCount} good"));
                    properties.add(new MessageProperty("diagnosis", diagnosis__121577));
                }
                return true;
            });
        if (inReleaseMode__121161)
        {
            properties.add(new DiagnosticsNode("(run in debug mode to collect repaint boundary statistics)"));
        }
    }

}

public class RenderIgnorePointer : RenderProxyBox
{
    internal virtual bool _ignoring { get; set; } = default!;
    internal virtual bool? _ignoringSemantics { get; set; } = default;

    public RenderIgnorePointer(RenderBox? child = null, bool ignoring = true, bool? ignoringSemantics = null) : base(child)
    {
        this._ignoring = ignoring;
        this._ignoringSemantics = ignoringSemantics;
    }

    public virtual bool ignoring
    {
        get => this._ignoring;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._ignoring))
            {
                return;
            }
            _ignoring = DartRuntimePrimitives.RequireValue(__value);
            if ((this.ignoringSemantics is null))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool? ignoringSemantics
    {
        get => this._ignoringSemantics;
        set
        {
            var __value = value;
            if ((__value == this._ignoringSemantics))
            {
                return;
            }
            _ignoringSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        return (!this.ignoring && base.hitTest(result, position: position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if ((this._ignoringSemantics ?? false))
        {
            return;
        }
        base.visitChildrenForSemantics((Action<RenderObject>)visitor);
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isBlockingUserActions = (this._ignoring && ((this._ignoringSemantics ?? true)));
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("ignoring", this._ignoring));
        properties.add(new DiagnosticsProperty<bool>("ignoringSemantics", this._ignoringSemantics, description: ((this._ignoringSemantics is null) ? null : $"implicitly {this._ignoringSemantics}")));
    }

}

public class RenderOffstage : RenderProxyBox
{
    internal virtual bool _offstage { get; set; } = default!;

    public RenderOffstage(bool offstage = true, RenderBox? child = null) : base(child)
    {
        this._offstage = offstage;
    }

    public virtual bool offstage
    {
        get => this._offstage;
        set
        {
            var __value = value;
            if ((__value == this._offstage))
            {
                return;
            }
            _offstage = __value;
            markNeedsLayoutForSizedByParentChange();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        if (this.offstage)
        {
            return 0.0;
        }
        return base.computeMinIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if (this.offstage)
        {
            return 0.0;
        }
        return base.computeMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if (this.offstage)
        {
            return 0.0;
        }
        return base.computeMinIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if (this.offstage)
        {
            return 0.0;
        }
        return base.computeMaxIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        if (this.offstage)
        {
            return null;
        }
        return base.computeDistanceToActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool sizedByParent => this.offstage;
    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        return (this.offstage ? null : base.computeDryBaseline(constraints, baseline));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if (this.offstage)
        {
            return ((BoxConstraints)constraints).smallest;
        }
        return base.computeDryLayout(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performResize()
    {
        DartRuntimePrimitives.Assert(() => this.offstage);
        base.performResize();
    }

    public override void performLayout()
    {
        if (this.offstage)
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
        return (!this.offstage && base.hitTest(result, position: position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool paintsChild(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => (object.Equals(__child.parent, this)));
        return !this.offstage;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (this.offstage)
        {
            return;
        }
        base.paint(context, offset);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (this.offstage)
        {
            return;
        }
        base.visitChildrenForSemantics((Action<RenderObject>)visitor);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("offstage", this.offstage));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        if ((child is null))
        {
            return new List<DiagnosticsNode>();
        }
        return new List<DiagnosticsNode> { ((Diagnosticable)child!).toDiagnosticsNode(name: "child", style: (this.offstage ? DiagnosticsTreeStyle.offstage : DiagnosticsTreeStyle.sparse)) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RenderAbsorbPointer : RenderProxyBox
{
    internal virtual bool _absorbing { get; set; } = default!;
    internal virtual bool? _ignoringSemantics { get; set; } = default;

    public RenderAbsorbPointer(RenderBox? child = null, bool absorbing = true, bool? ignoringSemantics = null) : base(child)
    {
        this._absorbing = absorbing;
        this._ignoringSemantics = ignoringSemantics;
    }

    public virtual bool absorbing
    {
        get => this._absorbing;
        set
        {
            var __value = value;
            if ((this._absorbing == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _absorbing = DartRuntimePrimitives.RequireValue(__value);
            if ((this.ignoringSemantics is null))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool? ignoringSemantics
    {
        get => this._ignoringSemantics;
        set
        {
            var __value = value;
            if ((__value == this._ignoringSemantics))
            {
                return;
            }
            _ignoringSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        return (this.absorbing ? size.contains(position) : base.hitTest(result, position: position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if ((this._ignoringSemantics ?? false))
        {
            return;
        }
        base.visitChildrenForSemantics((Action<RenderObject>)visitor);
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isBlockingUserActions = (this.absorbing && ((this._ignoringSemantics ?? true)));
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("absorbing", this.absorbing));
        properties.add(new DiagnosticsProperty<bool>("ignoringSemantics", this.ignoringSemantics, description: ((this.ignoringSemantics is null) ? null : $"implicitly {this.ignoringSemantics}")));
    }

}

public class RenderMetaData : RenderProxyBoxWithHitTestBehavior
{
    public virtual object metaData { get; set; } = default!;

    public RenderMetaData(object metaData = default!, HitTestBehavior behavior = HitTestBehavior.deferToChild, RenderBox? child = null) : base(behavior: behavior, child: child)
    {
        this.metaData = metaData;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<object>("metaData", this.metaData));
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

    public RenderSemanticsGestureHandler(RenderBox? child = null, Action? onTap = null, Action? onLongPress = null, Action<DragUpdateDetails>? onHorizontalDragUpdate = null, Action<DragUpdateDetails>? onVerticalDragUpdate = null, double scrollFactor = 0.8, HitTestBehavior behavior = HitTestBehavior.deferToChild) : base(child: child, behavior: behavior)
    {
        this.scrollFactor = scrollFactor;
        this._onTap = onTap;
        this._onLongPress = onLongPress;
        this._onHorizontalDragUpdate = onHorizontalDragUpdate;
        this._onVerticalDragUpdate = onVerticalDragUpdate;
    }

    public virtual HashSet<global::Doroti.Ui.SemanticsAction>? validActions
    {
        get => this._validActions;
        set
        {
            var __value = value is null ? null : (HashSet<SemanticsAction>)(object)value;
            if (global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals<global::Doroti.Ui.SemanticsAction>(__value, this._validActions))
            {
                return;
            }
            _validActions = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual Action? onTap
    {
        get => this._onTap;
        set
        {
            var __value = value;
            if ((object.Equals((Action?)this._onTap, (Action?)__value)))
            {
                return;
            }
            var hadHandler__136347 = (this._onTap is not null);
            _onTap = __value;
            if ((((__value is not null)) != hadHandler__136347))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual Action? onLongPress
    {
        get => this._onLongPress;
        set
        {
            var __value = value;
            if ((object.Equals((Action?)this._onLongPress, (Action?)__value)))
            {
                return;
            }
            var hadHandler__136784 = (this._onLongPress is not null);
            _onLongPress = __value;
            if ((((__value is not null)) != hadHandler__136784))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onHorizontalDragUpdate
    {
        get => this._onHorizontalDragUpdate;
        set
        {
            var __value = value is null ? null : (Action<DragUpdateDetails>)(object)value;
            if ((object.Equals((Action<DragUpdateDetails>?)this._onHorizontalDragUpdate, (Action<DragUpdateDetails>?)__value)))
            {
                return;
            }
            var hadHandler__137272 = (this._onHorizontalDragUpdate is not null);
            _onHorizontalDragUpdate = __value;
            if ((((__value is not null)) != hadHandler__137272))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onVerticalDragUpdate
    {
        get => this._onVerticalDragUpdate;
        set
        {
            var __value = value is null ? null : (Action<DragUpdateDetails>)(object)value;
            if ((object.Equals((Action<DragUpdateDetails>?)this._onVerticalDragUpdate, (Action<DragUpdateDetails>?)__value)))
            {
                return;
            }
            var hadHandler__137755 = (this._onVerticalDragUpdate is not null);
            _onVerticalDragUpdate = __value;
            if ((((__value is not null)) != hadHandler__137755))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        if (((this.onTap is not null) && _isValidAction(SemanticsAction.tap)))
        {
            config.onTap = this.onTap;
        }
        if (((this.onLongPress is not null) && _isValidAction(SemanticsAction.longPress)))
        {
            config.onLongPress = this.onLongPress;
        }
        if ((this.onHorizontalDragUpdate is not null))
        {
            if (_isValidAction(SemanticsAction.scrollRight))
            {
                config.onScrollRight = this._performSemanticScrollRight;
            }
            if (_isValidAction(SemanticsAction.scrollLeft))
            {
                config.onScrollLeft = this._performSemanticScrollLeft;
            }
        }
        if ((this.onVerticalDragUpdate is not null))
        {
            if (_isValidAction(SemanticsAction.scrollUp))
            {
                config.onScrollUp = this._performSemanticScrollUp;
            }
            if (_isValidAction(SemanticsAction.scrollDown))
            {
                config.onScrollDown = this._performSemanticScrollDown;
            }
        }
    }

    internal virtual bool _isValidAction(SemanticsAction action)
    {
        return ((this.validActions is null) || this.validActions!.Contains(action));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _performSemanticScrollLeft()
    {
        if ((this.onHorizontalDragUpdate is not null))
        {
            double primaryDelta__139388 = (size.width * -this.scrollFactor);
            this.onHorizontalDragUpdate!(new DragUpdateDetails(delta: new global::Doroti.Ui.Offset(primaryDelta__139388, 0.0), primaryDelta: primaryDelta__139388, globalPosition: localToGlobal(size.center(Offset.zero))));
        }
    }

    internal virtual void _performSemanticScrollRight()
    {
        if ((this.onHorizontalDragUpdate is not null))
        {
            double primaryDelta__139769 = (size.width * this.scrollFactor);
            this.onHorizontalDragUpdate!(new DragUpdateDetails(delta: new global::Doroti.Ui.Offset(primaryDelta__139769, 0.0), primaryDelta: primaryDelta__139769, globalPosition: localToGlobal(size.center(Offset.zero))));
        }
    }

    internal virtual void _performSemanticScrollUp()
    {
        if ((this.onVerticalDragUpdate is not null))
        {
            double primaryDelta__140144 = (size.height * -this.scrollFactor);
            this.onVerticalDragUpdate!(new DragUpdateDetails(delta: new global::Doroti.Ui.Offset(0.0, primaryDelta__140144), primaryDelta: primaryDelta__140144, globalPosition: localToGlobal(size.center(Offset.zero))));
        }
    }

    internal virtual void _performSemanticScrollDown()
    {
        if ((this.onVerticalDragUpdate is not null))
        {
            double primaryDelta__140521 = (size.height * this.scrollFactor);
            this.onVerticalDragUpdate!(new DragUpdateDetails(delta: new global::Doroti.Ui.Offset(0.0, primaryDelta__140521), primaryDelta: primaryDelta__140521, globalPosition: localToGlobal(size.center(Offset.zero))));
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var gestures__140934 = new List<string>();
        if ((checked((long)(gestures__140934.Count)) == 0))
        {
            gestures__140934.Add("<none>");
        }
        properties.add(new IterableProperty<string>("gestures", gestures__140934));
    }

}

public class RenderSemanticsAnnotations : RenderProxyBox, SemanticsAnnotationsMixin
{
    public virtual global::Doroti.Framework.Semantics.SemanticsProperties _properties { get; set; } = default!;
    public virtual bool _container { get; set; } = default!;
    public virtual bool _explicitChildNodes { get; set; } = default!;
    public virtual bool _excludeSemantics { get; set; } = default!;
    public virtual bool _blockUserActions { get; set; } = default!;
    public virtual Locale? _localeForSubtree { get; set; } = default;
    public virtual global::Doroti.Framework.Semantics.AttributedString? _attributedLabel { get; set; } = default;
    public virtual global::Doroti.Framework.Semantics.AttributedString? _attributedValue { get; set; } = default;
    public virtual global::Doroti.Framework.Semantics.AttributedString? _attributedIncreasedValue { get; set; } = default;
    public virtual global::Doroti.Framework.Semantics.AttributedString? _attributedDecreasedValue { get; set; } = default;
    public virtual global::Doroti.Framework.Semantics.AttributedString? _attributedHint { get; set; } = default;
    public virtual TextDirection? _textDirection { get; set; } = default;

    public RenderSemanticsAnnotations(RenderBox? child = null, global::Doroti.Framework.Semantics.SemanticsProperties properties = default!, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, Locale? localeForSubtree = null, TextDirection? textDirection = null) : base(child)
    {
        initSemanticsAnnotations(
            properties,
            container,
            explicitChildNodes,
            excludeSemantics,
            blockUserActions,
            localeForSubtree,
            textDirection);
    }

    public virtual void initSemanticsAnnotations(global::Doroti.Framework.Semantics.SemanticsProperties properties, bool container, bool explicitChildNodes, bool excludeSemantics, bool blockUserActions, Locale? localeForSubtree, TextDirection? textDirection)
    {
        this._properties = properties;
        this._container = container;
        this._explicitChildNodes = explicitChildNodes;
        this._excludeSemantics = excludeSemantics;
        this._blockUserActions = blockUserActions;
        this._localeForSubtree = localeForSubtree;
        this._textDirection = textDirection;
        _updateAttributedFields(this._properties);
    }

    public virtual global::Doroti.Framework.Semantics.SemanticsProperties properties
    {
        get => this._properties;
        set
        {
            var __value = value;
            if ((object.Equals(this._properties, __value)))
            {
                return;
            }
            this._properties = __value;
            _updateAttributedFields(this._properties);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool container
    {
        get => this._container;
        set
        {
            var __value = value;
            if ((this.container == __value))
            {
                return;
            }
            this._container = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool explicitChildNodes
    {
        get => this._explicitChildNodes;
        set
        {
            var __value = value;
            if ((this._explicitChildNodes == __value))
            {
                return;
            }
            this._explicitChildNodes = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool excludeSemantics
    {
        get => this._excludeSemantics;
        set
        {
            var __value = value;
            if ((this._excludeSemantics == __value))
            {
                return;
            }
            this._excludeSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool blockUserActions
    {
        get => this._blockUserActions;
        set
        {
            var __value = value;
            if ((this._blockUserActions == __value))
            {
                return;
            }
            this._blockUserActions = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual Locale? localeForSubtree
    {
        get => this._localeForSubtree;
        set
        {
            var __value = value;
            if ((object.Equals(this._localeForSubtree, __value)))
            {
                return;
            }
            this._localeForSubtree = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual void _updateAttributedFields(global::Doroti.Framework.Semantics.SemanticsProperties value)
    {
        this._attributedLabel = _effectiveAttributedLabel(value);
        this._attributedValue = _effectiveAttributedValue(value);
        this._attributedIncreasedValue = _effectiveAttributedIncreasedValue(value);
        this._attributedDecreasedValue = _effectiveAttributedDecreasedValue(value);
        this._attributedHint = _effectiveAttributedHint(value);
    }

    public virtual global::Doroti.Framework.Semantics.AttributedString? _effectiveAttributedLabel(global::Doroti.Framework.Semantics.SemanticsProperties value)
    {
        return (((global::Doroti.Framework.Semantics.SemanticsProperties)value).attributedLabel ?? (((((global::Doroti.Framework.Semantics.SemanticsProperties)value).label is null) ? null : new global::Doroti.Framework.Semantics.AttributedString(((global::Doroti.Framework.Semantics.SemanticsProperties)value).label!))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Semantics.AttributedString? _effectiveAttributedValue(global::Doroti.Framework.Semantics.SemanticsProperties value)
    {
        return (((global::Doroti.Framework.Semantics.SemanticsProperties)value).attributedValue ?? (((((global::Doroti.Framework.Semantics.SemanticsProperties)value).value is null) ? null : new global::Doroti.Framework.Semantics.AttributedString(((global::Doroti.Framework.Semantics.SemanticsProperties)value).value!))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Semantics.AttributedString? _effectiveAttributedIncreasedValue(global::Doroti.Framework.Semantics.SemanticsProperties value)
    {
        return (((global::Doroti.Framework.Semantics.SemanticsProperties)value).attributedIncreasedValue ?? (((((global::Doroti.Framework.Semantics.SemanticsProperties)value).increasedValue is null) ? null : new global::Doroti.Framework.Semantics.AttributedString(((global::Doroti.Framework.Semantics.SemanticsProperties)value).increasedValue!))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Semantics.AttributedString? _effectiveAttributedDecreasedValue(global::Doroti.Framework.Semantics.SemanticsProperties value)
    {
        return (((global::Doroti.Framework.Semantics.SemanticsProperties)this.properties).attributedDecreasedValue ?? (((((global::Doroti.Framework.Semantics.SemanticsProperties)value).decreasedValue is null) ? null : new global::Doroti.Framework.Semantics.AttributedString(((global::Doroti.Framework.Semantics.SemanticsProperties)value).decreasedValue!))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Semantics.AttributedString? _effectiveAttributedHint(global::Doroti.Framework.Semantics.SemanticsProperties value)
    {
        return (((global::Doroti.Framework.Semantics.SemanticsProperties)value).attributedHint ?? (((((global::Doroti.Framework.Semantics.SemanticsProperties)value).hint is null) ? null : new global::Doroti.Framework.Semantics.AttributedString(((global::Doroti.Framework.Semantics.SemanticsProperties)value).hint!))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextDirection? textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this.textDirection, __value)))
            {
                return;
            }
            this._textDirection = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (this.excludeSemantics)
        {
            return;
        }
        base.visitChildrenForSemantics((Action<RenderObject>)visitor);
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = (this.container || ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).identifier is not null)));
        config.explicitChildNodes = this.explicitChildNodes;
        config.isBlockingUserActions = this.blockUserActions;
        if ((this.localeForSubtree is not null))
        {
            config.localeForSubtree = this.localeForSubtree;
        }
        DartRuntimePrimitives.Assert(() => (((((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).scopesRoute ?? false)) && this.explicitChildNodes)) || !((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).scopesRoute ?? false))));
        DartRuntimePrimitives.Assert(() => !((((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).toggled ?? false)) && ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).@checked ?? false)))));
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).enabled is not null))
        {
            config.isEnabled = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).enabled;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).@checked is not null))
        {
            config.isChecked = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).@checked;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).mixed is not null))
        {
            config.isCheckStateMixed = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).mixed;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).toggled is not null))
        {
            config.isToggled = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).toggled;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).selected is not null))
        {
            config.isSelected = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).selected);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).button is not null))
        {
            config.isButton = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).button);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).expanded is not null))
        {
            config.isExpanded = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).expanded;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).link is not null))
        {
            config.isLink = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).link);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).linkUrl is not null))
        {
            config.linkUrl = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).linkUrl;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).slider is not null))
        {
            config.isSlider = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).slider);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).keyboardKey is not null))
        {
            config.isKeyboardKey = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).keyboardKey);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).header is not null))
        {
            config.isHeader = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).header);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).headingLevel is not null))
        {
            config.headingLevel = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).headingLevel);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).textField is not null))
        {
            config.isTextField = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).textField);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).readOnly is not null))
        {
            config.isReadOnly = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).readOnly);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).focusable is not null))
        {
            config.isFocusable = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).focusable);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).focused is not null))
        {
            config.isFocused = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).focused;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).accessibilityFocusBlockType is not null))
        {
            config.accessibilityFocusBlockType = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).accessibilityFocusBlockType);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).inMutuallyExclusiveGroup is not null))
        {
            config.isInMutuallyExclusiveGroup = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).inMutuallyExclusiveGroup);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).obscured is not null))
        {
            config.isObscured = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).obscured);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).multiline is not null))
        {
            config.isMultiline = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).multiline);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).hidden is not null))
        {
            config.isHidden = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).hidden);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).image is not null))
        {
            config.isImage = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).image);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).isRequired is not null))
        {
            config.isRequired = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).isRequired;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).identifier is not null))
        {
            config.identifier = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).identifier!;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).traversalParentIdentifier is not null))
        {
            config.traversalParentIdentifier = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).traversalParentIdentifier;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).traversalChildIdentifier is not null))
        {
            config.traversalChildIdentifier = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).traversalChildIdentifier;
        }
        if ((this._attributedLabel is not null))
        {
            config.attributedLabel = this._attributedLabel!;
        }
        if ((this._attributedValue is not null))
        {
            config.attributedValue = this._attributedValue!;
        }
        if ((this._attributedIncreasedValue is not null))
        {
            config.attributedIncreasedValue = this._attributedIncreasedValue!;
        }
        if ((this._attributedDecreasedValue is not null))
        {
            config.attributedDecreasedValue = this._attributedDecreasedValue!;
        }
        if ((this._attributedHint is not null))
        {
            config.attributedHint = this._attributedHint!;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).tooltip is not null))
        {
            config.tooltip = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).tooltip!;
        }
        if (((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).hintOverrides is not null) && ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).hintOverrides!.isNotEmpty))
        {
            config.hintOverrides = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).hintOverrides;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).scopesRoute is not null))
        {
            config.scopesRoute = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).scopesRoute);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).namesRoute is not null))
        {
            config.namesRoute = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).namesRoute);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).liveRegion is not null))
        {
            config.liveRegion = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).liveRegion);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).maxValueLength is not null))
        {
            config.maxValueLength = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).maxValueLength;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).currentValueLength is not null))
        {
            config.currentValueLength = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).currentValueLength;
        }
        if ((this.textDirection is not null))
        {
            config.textDirection = this.textDirection;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).sortKey is not null))
        {
            config.sortKey = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).sortKey;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).tagForChildren is not null))
        {
            config.addTagForChildren(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).tagForChildren!);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this.properties).role is not null))
        {
            config.role = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).role);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).controlsNodes is not null))
        {
            config.controlsNodes = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).controlsNodes;
        }
        if ((!object.Equals(((global::Doroti.Framework.Semantics.SemanticsConfiguration)config).validationResult, ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).validationResult)))
        {
            config.validationResult = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).validationResult;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).hitTestBehavior is not null))
        {
            config.hitTestBehavior = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).hitTestBehavior);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).inputType is not null))
        {
            config.inputType = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).inputType);
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).minValue is not null))
        {
            config.minValue = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).minValue;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).maxValue is not null))
        {
            config.maxValue = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).maxValue;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onTap is not null))
        {
            config.onTap = this._performTap;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onLongPress is not null))
        {
            config.onLongPress = this._performLongPress;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onDismiss is not null))
        {
            config.onDismiss = this._performDismiss;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onScrollLeft is not null))
        {
            config.onScrollLeft = this._performScrollLeft;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onScrollRight is not null))
        {
            config.onScrollRight = this._performScrollRight;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onScrollUp is not null))
        {
            config.onScrollUp = this._performScrollUp;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onScrollDown is not null))
        {
            config.onScrollDown = this._performScrollDown;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onIncrease is not null))
        {
            config.onIncrease = this._performIncrease;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onDecrease is not null))
        {
            config.onDecrease = this._performDecrease;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onCopy is not null))
        {
            config.onCopy = this._performCopy;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onCut is not null))
        {
            config.onCut = this._performCut;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onPaste is not null))
        {
            config.onPaste = this._performPaste;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorForwardByCharacter is not null))
        {
            config.onMoveCursorForwardByCharacter = this._performMoveCursorForwardByCharacter;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorBackwardByCharacter is not null))
        {
            config.onMoveCursorBackwardByCharacter = this._performMoveCursorBackwardByCharacter;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorForwardByWord is not null))
        {
            config.onMoveCursorForwardByWord = this._performMoveCursorForwardByWord;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorBackwardByWord is not null))
        {
            config.onMoveCursorBackwardByWord = this._performMoveCursorBackwardByWord;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onSetSelection is not null))
        {
            config.onSetSelection = this._performSetSelection;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onSetText is not null))
        {
            config.onSetText = this._performSetText;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onDidGainAccessibilityFocus is not null))
        {
            config.onDidGainAccessibilityFocus = this._performDidGainAccessibilityFocus;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onDidLoseAccessibilityFocus is not null))
        {
            config.onDidLoseAccessibilityFocus = this._performDidLoseAccessibilityFocus;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onFocus is not null))
        {
            config.onFocus = this._performFocus;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onExpand is not null))
        {
            config.onExpand = this._performExpand;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onCollapse is not null))
        {
            config.onCollapse = this._performCollapse;
        }
        if ((((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).customSemanticsActions is not null))
        {
            config.customSemanticsActions = ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).customSemanticsActions!;
        }
    }

    public virtual void _performTap()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onTap?.Invoke();
    }

    public virtual void _performLongPress()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onLongPress?.Invoke();
    }

    public virtual void _performDismiss()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onDismiss?.Invoke();
    }

    public virtual void _performScrollLeft()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onScrollLeft?.Invoke();
    }

    public virtual void _performScrollRight()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onScrollRight?.Invoke();
    }

    public virtual void _performScrollUp()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onScrollUp?.Invoke();
    }

    public virtual void _performScrollDown()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onScrollDown?.Invoke();
    }

    public virtual void _performIncrease()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onIncrease?.Invoke();
    }

    public virtual void _performDecrease()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onDecrease?.Invoke();
    }

    public virtual void _performCopy()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onCopy?.Invoke();
    }

    public virtual void _performCut()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onCut?.Invoke();
    }

    public virtual void _performPaste()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onPaste?.Invoke();
    }

    public virtual void _performMoveCursorForwardByCharacter(bool extendSelection)
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorForwardByCharacter?.Invoke(extendSelection);
    }

    public virtual void _performMoveCursorBackwardByCharacter(bool extendSelection)
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorBackwardByCharacter?.Invoke(extendSelection);
    }

    public virtual void _performMoveCursorForwardByWord(bool extendSelection)
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorForwardByWord?.Invoke(extendSelection);
    }

    public virtual void _performMoveCursorBackwardByWord(bool extendSelection)
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorBackwardByWord?.Invoke(extendSelection);
    }

    public virtual void _performSetSelection(TextSelection selection)
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onSetSelection?.Invoke(selection);
    }

    public virtual void _performSetText(string text)
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onSetText?.Invoke(text);
    }

    public virtual void _performDidGainAccessibilityFocus()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onDidGainAccessibilityFocus?.Invoke();
    }

    public virtual void _performDidLoseAccessibilityFocus()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onDidLoseAccessibilityFocus?.Invoke();
    }

    public virtual void _performFocus()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onFocus?.Invoke();
    }

    public virtual void _performExpand()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onExpand?.Invoke();
    }

    public virtual void _performCollapse()
    {
        ((global::Doroti.Framework.Semantics.SemanticsProperties)this._properties).onCollapse?.Invoke();
    }

}

public class RenderBlockSemantics : RenderProxyBox
{
    internal virtual bool _blocking { get; set; } = default!;

    public RenderBlockSemantics(RenderBox? child = null, bool blocking = true) : base(child)
    {
        this._blocking = blocking;
    }

    public virtual bool blocking
    {
        get => this._blocking;
        set
        {
            var __value = value;
            if ((__value == this._blocking))
            {
                return;
            }
            _blocking = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isBlockingSemanticsOfPreviouslyPaintedNodes = this.blocking;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("blocking", this.blocking));
    }

}

public class RenderMergeSemantics : RenderProxyBox
{
    public RenderMergeSemantics(RenderBox? child = null) : base(child)
    {
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = config;
    __cascade.isSemanticBoundary = true;
    __cascade.isMergingSemanticsOfDescendants = true;
    return __cascade;
}))();
    }

}

public class RenderExcludeSemantics : RenderProxyBox
{
    internal virtual bool _excluding { get; set; } = default!;

    public RenderExcludeSemantics(RenderBox? child = null, bool excluding = true) : base(child)
    {
        this._excluding = excluding;
    }

    public virtual bool excluding
    {
        get => this._excluding;
        set
        {
            var __value = value;
            if ((__value == this._excluding))
            {
                return;
            }
            _excluding = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (this.excluding)
        {
            return;
        }
        base.visitChildrenForSemantics((Action<RenderObject>)visitor);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("excluding", this.excluding));
    }

}

public class RenderIndexedSemantics : RenderProxyBox
{
    internal virtual long _index { get; set; } = default!;

    public RenderIndexedSemantics(RenderBox? child = null, long index = default!) : base(child)
    {
        this._index = index;
    }

    public virtual long index
    {
        get => this._index;
        set
        {
            var __value = value;
            if ((__value == this.index))
            {
                return;
            }
            _index = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.indexInParent = this.index;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<long>("index", this.index));
    }

}

public class RenderLeaderLayer : RenderProxyBox
{
    internal virtual LayerLink _link { get; set; } = default!;
    internal virtual Size? _previousLayoutSize { get; set; } = default;

    public RenderLeaderLayer(LayerLink link, RenderBox? child = null) : base(child)
    {
        this._link = link;
    }

    public virtual LayerLink link
    {
        get => this._link;
        set
        {
            var __value = value;
            if ((object.Equals(this._link, __value)))
            {
                return;
            }
            this._link.leaderSize = null;
            _link = __value;
            if ((this._previousLayoutSize is not null))
            {
                this._link.leaderSize = this._previousLayoutSize;
            }
            markNeedsPaint();
        }
    }
    public override bool alwaysNeedsCompositing => true;
    public override void performLayout()
    {
        base.performLayout();
        _previousLayoutSize = size;
        this.link.leaderSize = size;
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((layer is null))
        {
            layer = new LeaderLayer(link: this.link, offset: offset);
        }
        else
        {
            var leaderLayer__148197 = ((LeaderLayer?)(object?)layer!)!;
            ((Func<LeaderLayer>)(() =>
{
    var __cascade = leaderLayer__148197;
    __cascade.link = this.link;
    __cascade.offset = offset;
    return __cascade;
}))();
        }
        context.pushLayer(layer!, (Action<PaintingContext, Offset>)base.paint, Offset.zero);
        DartRuntimePrimitives.Assert(() =>
            {
                layer!.debugCreator = debugCreator;
                return true;
            });
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<LayerLink>("link", this.link));
    }

}

public class RenderFollowerLayer : RenderProxyBox
{
    internal virtual LayerLink _link { get; set; } = default!;
    internal virtual bool _showWhenUnlinked { get; set; } = default!;
    internal virtual Offset _offset { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.Alignment _leaderAnchor { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.Alignment _followerAnchor { get; set; } = default!;

    public RenderFollowerLayer(LayerLink link, bool showWhenUnlinked = true, Offset offset = default, global::Doroti.Framework.Painting.Alignment leaderAnchor = default!, global::Doroti.Framework.Painting.Alignment followerAnchor = default!, RenderBox? child = null) : base(child)
    {
        global::Doroti.Framework.Painting.Alignment __leaderAnchor = leaderAnchor ?? global::Doroti.Framework.Painting.Alignment.topLeft;
        global::Doroti.Framework.Painting.Alignment __followerAnchor = followerAnchor ?? global::Doroti.Framework.Painting.Alignment.topLeft;
        this._link = link;
        this._showWhenUnlinked = showWhenUnlinked;
        this._offset = offset;
        this._leaderAnchor = __leaderAnchor;
        this._followerAnchor = __followerAnchor;
    }

    public virtual LayerLink link
    {
        get => this._link;
        set
        {
            var __value = value;
            if ((object.Equals(this._link, __value)))
            {
                return;
            }
            _link = __value;
            markNeedsPaint();
        }
    }
    public virtual bool showWhenUnlinked
    {
        get => this._showWhenUnlinked;
        set
        {
            var __value = value;
            if ((this._showWhenUnlinked == __value))
            {
                return;
            }
            _showWhenUnlinked = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.Offset offset
    {
        get => this._offset;
        set
        {
            var __value = value;
            if ((object.Equals(this._offset, __value)))
            {
                return;
            }
            _offset = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Framework.Painting.Alignment leaderAnchor
    {
        get => this._leaderAnchor;
        set
        {
            var __value = value;
            if ((object.Equals(this._leaderAnchor, __value)))
            {
                return;
            }
            _leaderAnchor = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Framework.Painting.Alignment followerAnchor
    {
        get => this._followerAnchor;
        set
        {
            var __value = value;
            if ((object.Equals(this._followerAnchor, __value)))
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
    public override FollowerLayer? layer => ((FollowerLayer?)(object?)base.layer)!;
    public virtual Matrix4 getCurrentTransform()
    {
        return (this.layer?.getLastTransform() ?? Matrix4.identity());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (((((LayerLink)this.link).leader is null) && !this.showWhenUnlinked))
        {
            return false;
        }
        return hitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        return result.addWithPaintTransform(transform: getCurrentTransform(), position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, position) =>
        {
            return base.hitTestChildren(result, position: position);
            return default;
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        global::Doroti.Ui.Size? leaderSize__154066 = ((LayerLink)this.link).leaderSize;
        DartRuntimePrimitives.Assert(() => (((((LayerLink)this.link).leaderSize is not null) || (((LayerLink)this.link).leader is null)) || (object.Equals(this.leaderAnchor, global::Doroti.Framework.Painting.Alignment.topLeft))));
        global::Doroti.Ui.Offset effectiveLinkedOffset__154426 = ((leaderSize__154066 is null) ? this.offset : ((this.leaderAnchor.alongSize(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(leaderSize__154066))) - this.followerAnchor.alongSize(size)) + this.offset));
        if ((this.layer is null))
        {
            layer = new FollowerLayer(link: this.link, showWhenUnlinked: this.showWhenUnlinked, linkedOffset: effectiveLinkedOffset__154426, unlinkedOffset: offset);
        }
        else
        {
            ((Func<FollowerLayer?>)(() =>
{
    var __cascade = this.layer;
    __cascade.link = this.link;
    __cascade.showWhenUnlinked = this.showWhenUnlinked;
    __cascade.linkedOffset = effectiveLinkedOffset__154426;
    __cascade.unlinkedOffset = offset;
    return __cascade;
}))();
        }
        context.pushLayer(this.layer!, (Action<PaintingContext, Offset>)base.paint, Offset.zero, childPaintBounds: global::Doroti.Ui.Rect.fromLTRB(double.NegativeInfinity, double.NegativeInfinity, double.PositiveInfinity, double.PositiveInfinity));
        DartRuntimePrimitives.Assert(() =>
            {
                this.layer!.debugCreator = debugCreator;
                return true;
            });
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
        transform.multiply(getCurrentTransform());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<LayerLink>("link", this.link));
        properties.add(new DiagnosticsProperty<bool>("showWhenUnlinked", this.showWhenUnlinked));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("offset", this.offset));
        properties.add(new global::Doroti.Framework.Painting.TransformProperty("current transform matrix", getCurrentTransform()));
    }

}

public class RenderAnnotatedRegion<T> : RenderProxyBox
{
    internal virtual T _value { get; set; } = default!;
    internal virtual bool _sized { get; set; } = default!;
    public virtual LayerHandle<AnnotatedRegionLayer<T>> _layerHandle { get; private set; } = default!;
    private bool __field_alwaysNeedsCompositing = true;
    public override bool alwaysNeedsCompositing { get => __field_alwaysNeedsCompositing; }

    public RenderAnnotatedRegion(T value, bool sized, RenderBox? child = null) : base(child)
    {
        this._value = value;
        this._sized = sized;
        this._layerHandle = new LayerHandle<AnnotatedRegionLayer<T>>();
    }

    public virtual T value
    {
        get => this._value;
        set
        {
            var newValue = value;
            if (EqualityComparer<T>.Default.Equals(this._value, newValue))
            {
                return;
            }
            _value = newValue;
            markNeedsPaint();
        }
    }
    public virtual bool sized
    {
        get => this._sized;
        set
        {
            var __value = value;
            if ((this._sized == __value))
            {
                return;
            }
            _sized = __value;
            markNeedsPaint();
        }
    }
    public override void paint(PaintingContext context, Offset offset)
    {
        var layer__157520 = new AnnotatedRegionLayer<T>(this.value, size: (this.sized ? size : null), offset: (this.sized ? offset : null));
        this._layerHandle.layer = layer__157520;
        context.pushLayer(layer__157520, (Action<PaintingContext, Offset>)base.paint, offset);
    }

    public override void dispose()
    {
        this._layerHandle.layer = null;
        base.dispose();
    }

}
