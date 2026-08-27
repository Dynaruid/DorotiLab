// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_padding.dart
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

public abstract class RenderSliverEdgeInsetsPadding : RenderSliver, RenderObjectWithChildMixin<RenderSliver>
{
    public virtual RenderSliver? _child { get; set; } = default;

    public abstract global::Doroti.Framework.Painting.EdgeInsets? resolvedPadding { get; }
    public virtual double beforePadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.resolvedPadding is not null));
            return (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection) switch { global::Doroti.Framework.Painting.AxisDirection.up => this.resolvedPadding!.bottom, global::Doroti.Framework.Painting.AxisDirection.right => this.resolvedPadding!.left, global::Doroti.Framework.Painting.AxisDirection.down => this.resolvedPadding!.top, global::Doroti.Framework.Painting.AxisDirection.left => this.resolvedPadding!.right, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public virtual double afterPadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.resolvedPadding is not null));
            return (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection) switch { global::Doroti.Framework.Painting.AxisDirection.up => this.resolvedPadding!.top, global::Doroti.Framework.Painting.AxisDirection.right => this.resolvedPadding!.right, global::Doroti.Framework.Painting.AxisDirection.down => this.resolvedPadding!.bottom, global::Doroti.Framework.Painting.AxisDirection.left => this.resolvedPadding!.left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public virtual double mainAxisPadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.resolvedPadding is not null));
            return this.resolvedPadding!.along(((SliverConstraints)constraints).axis);
            return default!;
        }
    }
    public virtual double crossAxisPadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.resolvedPadding is not null));
            return (((SliverConstraints)constraints).axis switch { global::Doroti.Framework.Painting.Axis.horizontal => this.resolvedPadding!.vertical, global::Doroti.Framework.Painting.Axis.vertical => this.resolvedPadding!.horizontal, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public override void setupParentData(RenderObject child)
    {
        if ((((RenderObject)child).parentData is not SliverPhysicalParentData))
        {
            child.parentData = new SliverPhysicalParentData();
        }
    }

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = this.constraints;
        double paintOffset(double from, double to)
        {
            return calculatePaintOffset(constraintsLocal, from: from, to: to);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double cacheOffset(double from, double to)
        {
            return calculateCacheOffset(constraintsLocal, from: from, to: to);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        DartRuntimePrimitives.Assert(() => (this.resolvedPadding is not null));
        global::Doroti.Framework.Painting.EdgeInsets resolvedPaddingLocal = this.resolvedPadding!;
        double beforePaddingLocal = this.beforePadding;
        double afterPaddingLocal = this.afterPadding;
        double mainAxisPaddingLocal = this.mainAxisPadding;
        double crossAxisPaddingLocal = this.crossAxisPadding;
        if ((child is null))
        {
            double paintExtentLocal = paintOffset(from: 0.0, to: mainAxisPaddingLocal);
            double cacheExtentLocal = cacheOffset(from: 0.0, to: mainAxisPaddingLocal);
            geometry = new SliverGeometry(scrollExtent: mainAxisPaddingLocal, paintExtent: Math.Min(paintExtentLocal, ((SliverConstraints)constraintsLocal).remainingPaintExtent), maxPaintExtent: mainAxisPaddingLocal, cacheExtent: cacheExtentLocal);
            return;
        }
        double beforePaddingPaintExtent = paintOffset(from: 0.0, to: beforePaddingLocal);
        double overlapLocal = ((SliverConstraints)constraintsLocal).overlap;
        if ((overlapLocal > 0L))
        {
            overlapLocal = Math.Max(0.0, (((SliverConstraints)constraintsLocal).overlap - beforePaddingPaintExtent));
        }
        child!.layout(constraintsLocal.copyWith(scrollOffset: Math.Max(0.0, (((SliverConstraints)constraintsLocal).scrollOffset - beforePaddingLocal)), cacheOrigin: Math.Min(0.0, (((SliverConstraints)constraintsLocal).cacheOrigin + beforePaddingLocal)), overlap: overlapLocal, remainingPaintExtent: (((SliverConstraints)constraintsLocal).remainingPaintExtent - paintOffset(from: 0.0, to: beforePaddingLocal)), remainingCacheExtent: (((SliverConstraints)constraintsLocal).remainingCacheExtent - cacheOffset(from: 0.0, to: beforePaddingLocal)), crossAxisExtent: Math.Max(0.0, (((SliverConstraints)constraintsLocal).crossAxisExtent - crossAxisPaddingLocal)), precedingScrollExtent: (beforePaddingLocal + ((SliverConstraints)constraintsLocal).precedingScrollExtent)), parentUsesSize: true);
        SliverGeometry childLayoutGeometry = child!.geometry!;
        if ((((SliverGeometry)childLayoutGeometry).scrollOffsetCorrection is not null))
        {
            geometry = new SliverGeometry(scrollOffsetCorrection: ((SliverGeometry)childLayoutGeometry).scrollOffsetCorrection);
            return;
        }
        double scrollExtentLocal = ((SliverGeometry)childLayoutGeometry).scrollExtent;
        double beforePaddingCacheExtent = cacheOffset(from: 0.0, to: beforePaddingLocal);
        double afterPaddingCacheExtent = cacheOffset(from: (beforePaddingLocal + scrollExtentLocal), to: (mainAxisPaddingLocal + scrollExtentLocal));
        double afterPaddingPaintExtent = paintOffset(from: (beforePaddingLocal + scrollExtentLocal), to: (mainAxisPaddingLocal + scrollExtentLocal));
        double mainAxisPaddingCacheExtent = (beforePaddingCacheExtent + afterPaddingCacheExtent);
        double mainAxisPaddingPaintExtent = (beforePaddingPaintExtent + afterPaddingPaintExtent);
        double paintExtentAlternate = Math.Min((beforePaddingPaintExtent + Math.Max(((SliverGeometry)childLayoutGeometry).paintExtent, (((SliverGeometry)childLayoutGeometry).layoutExtent + afterPaddingPaintExtent))), ((SliverConstraints)constraintsLocal).remainingPaintExtent);
        geometry = new SliverGeometry(paintOrigin: ((SliverGeometry)childLayoutGeometry).paintOrigin, scrollExtent: (mainAxisPaddingLocal + scrollExtentLocal), paintExtent: paintExtentAlternate, layoutExtent: Math.Min((mainAxisPaddingPaintExtent + ((SliverGeometry)childLayoutGeometry).layoutExtent), paintExtentAlternate), cacheExtent: Math.Min((mainAxisPaddingCacheExtent + ((SliverGeometry)childLayoutGeometry).cacheExtent), ((SliverConstraints)constraintsLocal).remainingCacheExtent), maxPaintExtent: (mainAxisPaddingLocal + ((SliverGeometry)childLayoutGeometry).maxPaintExtent), hitTestExtent: Math.Max((mainAxisPaddingPaintExtent + ((SliverGeometry)childLayoutGeometry).paintExtent), (beforePaddingPaintExtent + ((SliverGeometry)childLayoutGeometry).hitTestExtent)), hasVisualOverflow: ((SliverGeometry)childLayoutGeometry).hasVisualOverflow);
        double calculatedOffset = (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraintsLocal).axisDirection, ((SliverConstraints)constraintsLocal).growthDirection) switch { global::Doroti.Framework.Painting.AxisDirection.up => paintOffset(from: (((global::Doroti.Framework.Painting.EdgeInsets)resolvedPaddingLocal).bottom + scrollExtentLocal), to: (resolvedPaddingLocal.vertical + scrollExtentLocal)), global::Doroti.Framework.Painting.AxisDirection.left => paintOffset(from: (((global::Doroti.Framework.Painting.EdgeInsets)resolvedPaddingLocal).right + scrollExtentLocal), to: (resolvedPaddingLocal.horizontal + scrollExtentLocal)), global::Doroti.Framework.Painting.AxisDirection.right => paintOffset(from: 0.0, to: ((global::Doroti.Framework.Painting.EdgeInsets)resolvedPaddingLocal).left), global::Doroti.Framework.Painting.AxisDirection.down => paintOffset(from: 0.0, to: ((global::Doroti.Framework.Painting.EdgeInsets)resolvedPaddingLocal).top), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var childParentData = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
        childParentData.paintOffset = (((SliverConstraints)constraintsLocal).axis switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset(calculatedOffset, ((global::Doroti.Framework.Painting.EdgeInsets)resolvedPaddingLocal).top), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(((global::Doroti.Framework.Painting.EdgeInsets)resolvedPaddingLocal).left, calculatedOffset), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        DartRuntimePrimitives.Assert(() => (beforePaddingLocal == this.beforePadding));
        DartRuntimePrimitives.Assert(() => (afterPaddingLocal == this.afterPadding));
        DartRuntimePrimitives.Assert(() => (mainAxisPaddingLocal == this.mainAxisPadding));
        DartRuntimePrimitives.Assert(() => (crossAxisPaddingLocal == this.crossAxisPadding));
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        if (((child is not null) && (child!.geometry!.hitTestExtent > 0.0)))
        {
            var childParentData = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
            return result.addWithAxisOffset(mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition, mainAxisOffset: childMainAxisPosition(child!), crossAxisOffset: childCrossAxisPosition(child!), paintOffset: ((SliverPhysicalParentData)childParentData).paintOffset, hitTest: (Func<SliverHitTestResult, double, double, bool>)child!.hitTest);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderSliver)(object)child;
        DartRuntimePrimitives.Assert(() => (object.Equals(__child, this.child)));
        return calculatePaintOffset(constraints, from: 0.0, to: this.beforePadding);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double childCrossAxisPosition(RenderObject child)
    {
        var __child = (RenderSliver)(object)child;
        DartRuntimePrimitives.Assert(() => (object.Equals(__child, this.child)));
        DartRuntimePrimitives.Assert(() => (this.resolvedPadding is not null));
        return (((SliverConstraints)constraints).axis switch { global::Doroti.Framework.Painting.Axis.horizontal => this.resolvedPadding!.top, global::Doroti.Framework.Painting.Axis.vertical => this.resolvedPadding!.left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? childScrollOffset(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderObject)child).parent, this)));
        return this.beforePadding;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child, this.child)));
        var childParentData = ((SliverPhysicalParentData?)(object?)((RenderObject)child).parentData!)!;
        childParentData.applyPaintTransform(transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (((child is not null) && child!.geometry!.visible))
        {
            var childParentData = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
            context.paintChild(child!, (offset + ((SliverPhysicalParentData)childParentData).paintOffset));
        }
    }

    public override void debugPaint(PaintingContext context, Offset offset)
    {
        base.debugPaint(context, offset);
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled)
                {
                    global::Doroti.Ui.Size parentSize = getAbsoluteSize();
                    global::Doroti.Ui.Rect outerRect = (offset & parentSize);
                    global::Doroti.Ui.Rect? innerRect = default!;
                    if ((child is not null))
                    {
                        global::Doroti.Ui.Size childSize = child!.getAbsoluteSize();
                        var childParentData = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
                        innerRect = (((offset + ((SliverPhysicalParentData)childParentData).paintOffset)) & childSize);
                        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(innerRect).top >= outerRect.top));
                        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(innerRect).left >= outerRect.left));
                        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(innerRect).right <= outerRect.right));
                        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(innerRect).bottom <= outerRect.bottom));
                    }
                    global::Doroti.Framework.Rendering.DebugLibrary.debugPaintPadding(((PaintingContext)context).canvas, outerRect, innerRect);
                }
                return true;
            });
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderSliver))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderSliver)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderSliver? child
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

public class RenderSliverPadding : RenderSliverEdgeInsetsPadding
{
    internal virtual global::Doroti.Framework.Painting.EdgeInsets? _resolvedPadding { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry _padding { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;

    public RenderSliverPadding(global::Doroti.Framework.Painting.EdgeInsetsGeometry padding, TextDirection? textDirection = null, RenderSliver? child = null)
    {
        this._padding = padding;
        this._textDirection = textDirection;
        System.Diagnostics.Debug.Assert(((global::Doroti.Framework.Painting.EdgeInsetsGeometry)padding).isNonNegative);
    }

    public override global::Doroti.Framework.Painting.EdgeInsets? resolvedPadding => this._resolvedPadding;
    internal virtual void _resolve()
    {
        if ((this.resolvedPadding is not null))
        {
            return;
        }
        _resolvedPadding = this.padding.resolve(this.textDirection);
        DartRuntimePrimitives.Assert(() => this.resolvedPadding!.isNonNegative);
    }

    internal virtual void _markNeedsResolution()
    {
        _resolvedPadding = null;
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
            _markNeedsResolution();
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
            _markNeedsResolution();
        }
    }
    public override void performLayout()
    {
        _resolve();
        base.performLayout();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
    }

}

