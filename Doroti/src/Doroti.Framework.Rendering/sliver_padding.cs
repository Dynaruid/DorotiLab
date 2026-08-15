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

namespace Doroti.Generated.Framework.Rendering;

public abstract class RenderSliverEdgeInsetsPadding : RenderSliver, RenderObjectWithChildMixin<RenderSliver>
{
    public virtual RenderSliver? _child { get; set; } = default;

    public abstract global::Doroti.Generated.Framework.Painting.EdgeInsets? resolvedPadding { get; }
    public virtual double beforePadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.resolvedPadding is not null));
            return (global::Doroti.Generated.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection) switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => this.resolvedPadding!.bottom, global::Doroti.Generated.Framework.Painting.AxisDirection.right => this.resolvedPadding!.left, global::Doroti.Generated.Framework.Painting.AxisDirection.down => this.resolvedPadding!.top, global::Doroti.Generated.Framework.Painting.AxisDirection.left => this.resolvedPadding!.right, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public virtual double afterPadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.resolvedPadding is not null));
            return (global::Doroti.Generated.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection) switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => this.resolvedPadding!.top, global::Doroti.Generated.Framework.Painting.AxisDirection.right => this.resolvedPadding!.right, global::Doroti.Generated.Framework.Painting.AxisDirection.down => this.resolvedPadding!.bottom, global::Doroti.Generated.Framework.Painting.AxisDirection.left => this.resolvedPadding!.left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
            return (((SliverConstraints)constraints).axis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => this.resolvedPadding!.vertical, global::Doroti.Generated.Framework.Painting.Axis.vertical => this.resolvedPadding!.horizontal, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
        SliverConstraints constraints__3988 = this.constraints;
        double paintOffset(double from, double to)
        {
            return calculatePaintOffset(constraints__3988, from: from, to: to);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double cacheOffset(double from, double to)
        {
            return calculateCacheOffset(constraints__3988, from: from, to: to);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        DartRuntimePrimitives.Assert(() => (this.resolvedPadding is not null));
        global::Doroti.Generated.Framework.Painting.EdgeInsets resolvedPadding__4350 = this.resolvedPadding!;
        double beforePadding__4408 = this.beforePadding;
        double afterPadding__4461 = this.afterPadding;
        double mainAxisPadding__4512 = this.mainAxisPadding;
        double crossAxisPadding__4569 = this.crossAxisPadding;
        if ((child is null))
        {
            double paintExtent__4655 = paintOffset(from: 0.0, to: mainAxisPadding__4512);
            double cacheExtent__4733 = cacheOffset(from: 0.0, to: mainAxisPadding__4512);
            geometry = new SliverGeometry(scrollExtent: mainAxisPadding__4512, paintExtent: Math.Min(paintExtent__4655, ((SliverConstraints)constraints__3988).remainingPaintExtent), maxPaintExtent: mainAxisPadding__4512, cacheExtent: cacheExtent__4733);
            return;
        }
        double beforePaddingPaintExtent__5063 = paintOffset(from: 0.0, to: beforePadding__4408);
        double overlap__5144 = ((SliverConstraints)constraints__3988).overlap;
        if ((overlap__5144 > 0L))
        {
            overlap__5144 = Math.Max(0.0, (((SliverConstraints)constraints__3988).overlap - beforePaddingPaintExtent__5063));
        }
        child!.layout(constraints__3988.copyWith(scrollOffset: Math.Max(0.0, (((SliverConstraints)constraints__3988).scrollOffset - beforePadding__4408)), cacheOrigin: Math.Min(0.0, (((SliverConstraints)constraints__3988).cacheOrigin + beforePadding__4408)), overlap: overlap__5144, remainingPaintExtent: (((SliverConstraints)constraints__3988).remainingPaintExtent - paintOffset(from: 0.0, to: beforePadding__4408)), remainingCacheExtent: (((SliverConstraints)constraints__3988).remainingCacheExtent - cacheOffset(from: 0.0, to: beforePadding__4408)), crossAxisExtent: Math.Max(0.0, (((SliverConstraints)constraints__3988).crossAxisExtent - crossAxisPadding__4569)), precedingScrollExtent: (beforePadding__4408 + ((SliverConstraints)constraints__3988).precedingScrollExtent)), parentUsesSize: true);
        SliverGeometry childLayoutGeometry__5991 = child!.geometry!;
        if ((((SliverGeometry)childLayoutGeometry__5991).scrollOffsetCorrection is not null))
        {
            geometry = new SliverGeometry(scrollOffsetCorrection: ((SliverGeometry)childLayoutGeometry__5991).scrollOffsetCorrection);
            return;
        }
        double scrollExtent__6231 = ((SliverGeometry)childLayoutGeometry__5991).scrollExtent;
        double beforePaddingCacheExtent__6297 = cacheOffset(from: 0.0, to: beforePadding__4408);
        double afterPaddingCacheExtent__6384 = cacheOffset(from: (beforePadding__4408 + scrollExtent__6231), to: (mainAxisPadding__4512 + scrollExtent__6231));
        double afterPaddingPaintExtent__6531 = paintOffset(from: (beforePadding__4408 + scrollExtent__6231), to: (mainAxisPadding__4512 + scrollExtent__6231));
        double mainAxisPaddingCacheExtent__6678 = (beforePaddingCacheExtent__6297 + afterPaddingCacheExtent__6384);
        double mainAxisPaddingPaintExtent__6776 = (beforePaddingPaintExtent__5063 + afterPaddingPaintExtent__6531);
        double paintExtent__6874 = Math.Min((beforePaddingPaintExtent__5063 + Math.Max(((SliverGeometry)childLayoutGeometry__5991).paintExtent, (((SliverGeometry)childLayoutGeometry__5991).layoutExtent + afterPaddingPaintExtent__6531))), ((SliverConstraints)constraints__3988).remainingPaintExtent);
        geometry = new SliverGeometry(paintOrigin: ((SliverGeometry)childLayoutGeometry__5991).paintOrigin, scrollExtent: (mainAxisPadding__4512 + scrollExtent__6231), paintExtent: paintExtent__6874, layoutExtent: Math.Min((mainAxisPaddingPaintExtent__6776 + ((SliverGeometry)childLayoutGeometry__5991).layoutExtent), paintExtent__6874), cacheExtent: Math.Min((mainAxisPaddingCacheExtent__6678 + ((SliverGeometry)childLayoutGeometry__5991).cacheExtent), ((SliverConstraints)constraints__3988).remainingCacheExtent), maxPaintExtent: (mainAxisPadding__4512 + ((SliverGeometry)childLayoutGeometry__5991).maxPaintExtent), hitTestExtent: Math.Max((mainAxisPaddingPaintExtent__6776 + ((SliverGeometry)childLayoutGeometry__5991).paintExtent), (beforePaddingPaintExtent__5063 + ((SliverGeometry)childLayoutGeometry__5991).hitTestExtent)), hasVisualOverflow: ((SliverGeometry)childLayoutGeometry__5991).hasVisualOverflow);
        double calculatedOffset__7920 = (global::Doroti.Generated.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints__3988).axisDirection, ((SliverConstraints)constraints__3988).growthDirection) switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => paintOffset(from: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)resolvedPadding__4350).bottom + scrollExtent__6231), to: (resolvedPadding__4350.vertical + scrollExtent__6231)), global::Doroti.Generated.Framework.Painting.AxisDirection.left => paintOffset(from: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)resolvedPadding__4350).right + scrollExtent__6231), to: (resolvedPadding__4350.horizontal + scrollExtent__6231)), global::Doroti.Generated.Framework.Painting.AxisDirection.right => paintOffset(from: 0.0, to: ((global::Doroti.Generated.Framework.Painting.EdgeInsets)resolvedPadding__4350).left), global::Doroti.Generated.Framework.Painting.AxisDirection.down => paintOffset(from: 0.0, to: ((global::Doroti.Generated.Framework.Painting.EdgeInsets)resolvedPadding__4350).top), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var childParentData__8545 = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
        childParentData__8545.paintOffset = (((SliverConstraints)constraints__3988).axis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset(calculatedOffset__7920, ((global::Doroti.Generated.Framework.Painting.EdgeInsets)resolvedPadding__4350).top), global::Doroti.Generated.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(((global::Doroti.Generated.Framework.Painting.EdgeInsets)resolvedPadding__4350).left, calculatedOffset__7920), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        DartRuntimePrimitives.Assert(() => (beforePadding__4408 == this.beforePadding));
        DartRuntimePrimitives.Assert(() => (afterPadding__4461 == this.afterPadding));
        DartRuntimePrimitives.Assert(() => (mainAxisPadding__4512 == this.mainAxisPadding));
        DartRuntimePrimitives.Assert(() => (crossAxisPadding__4569 == this.crossAxisPadding));
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        if (((child is not null) && (child!.geometry!.hitTestExtent > 0.0)))
        {
            var childParentData__9263 = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
            return result.addWithAxisOffset(mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition, mainAxisOffset: childMainAxisPosition(child!), crossAxisOffset: childCrossAxisPosition(child!), paintOffset: ((SliverPhysicalParentData)childParentData__9263).paintOffset, hitTest: (Func<SliverHitTestResult, double, double, bool>)child!.hitTest);
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
        return (((SliverConstraints)constraints).axis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => this.resolvedPadding!.top, global::Doroti.Generated.Framework.Painting.Axis.vertical => this.resolvedPadding!.left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
        var childParentData__10400 = ((SliverPhysicalParentData?)(object?)((RenderObject)child).parentData!)!;
        childParentData__10400.applyPaintTransform(transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (((child is not null) && child!.geometry!.visible))
        {
            var childParentData__10654 = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
            context.paintChild(child!, (offset + ((SliverPhysicalParentData)childParentData__10654).paintOffset));
        }
    }

    public override void debugPaint(PaintingContext context, Offset offset)
    {
        base.debugPaint(context, offset);
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled)
                {
                    global::Doroti.Ui.Size parentSize__10984 = getAbsoluteSize();
                    global::Doroti.Ui.Rect outerRect__11035 = (offset & parentSize__10984);
                    global::Doroti.Ui.Rect? innerRect__11082 = default!;
                    if ((child is not null))
                    {
                        global::Doroti.Ui.Size childSize__11143 = child!.getAbsoluteSize();
                        var childParentData__11197 = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
                        innerRect__11082 = (((offset + ((SliverPhysicalParentData)childParentData__11197).paintOffset)) & childSize__11143);
                        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(innerRect__11082).top >= outerRect__11035.top));
                        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(innerRect__11082).left >= outerRect__11035.left));
                        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(innerRect__11082).right <= outerRect__11035.right));
                        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(innerRect__11082).bottom <= outerRect__11035.bottom));
                    }
                    global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPaintPadding(((PaintingContext)context).canvas, outerRect__11035, innerRect__11082);
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
    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? _resolvedPadding { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry _padding { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;

    public RenderSliverPadding(global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding, TextDirection? textDirection = null, RenderSliver? child = null)
    {
        this._padding = padding;
        this._textDirection = textDirection;
        System.Diagnostics.Debug.Assert(((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)padding).isNonNegative);
    }

    public override global::Doroti.Generated.Framework.Painting.EdgeInsets? resolvedPadding => this._resolvedPadding;
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
        properties.add(new DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
    }

}

