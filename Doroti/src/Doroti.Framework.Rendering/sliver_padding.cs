// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_padding.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public abstract class RenderSliverEdgeInsetsPadding : RenderSliver, RenderObjectWithChildMixin<RenderSliver>
{
    public virtual RenderSliver? _child { get; set; } = default;

    public abstract global::Doroti.Framework.Painting.EdgeInsets? resolvedPadding { get; }
    public virtual double beforePadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => resolvedPadding is not null);
            return SliverLibrary.applyGrowthDirectionToAxisDirection(constraints.axisDirection, constraints.growthDirection) switch { AxisDirection.up => resolvedPadding!.bottom, AxisDirection.right => resolvedPadding!.left, AxisDirection.down => resolvedPadding!.top, AxisDirection.left => resolvedPadding!.right, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    public virtual double afterPadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => resolvedPadding is not null);
            return SliverLibrary.applyGrowthDirectionToAxisDirection(constraints.axisDirection, constraints.growthDirection) switch { AxisDirection.up => resolvedPadding!.top, AxisDirection.right => resolvedPadding!.right, AxisDirection.down => resolvedPadding!.bottom, AxisDirection.left => resolvedPadding!.left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    public virtual double mainAxisPadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => resolvedPadding is not null);
            return resolvedPadding!.along(constraints.axis);
        }
    }
    public virtual double crossAxisPadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => resolvedPadding is not null);
            return constraints.axis switch { Axis.horizontal => resolvedPadding!.vertical, Axis.vertical => resolvedPadding!.horizontal, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    public override void setupParentData(RenderObject child)
    {
        if (child.parentData is not SliverPhysicalParentData)
        {
            child.parentData = new SliverPhysicalParentData();
        }
    }

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = constraints;
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
        DartRuntimePrimitives.Assert(() => resolvedPadding is not null);
        global::Doroti.Framework.Painting.EdgeInsets resolvedPaddingLocal = resolvedPadding!;
        double beforePaddingLocal = beforePadding;
        double afterPaddingLocal = afterPadding;
        double mainAxisPaddingLocal = mainAxisPadding;
        double crossAxisPaddingLocal = crossAxisPadding;
        if (child is null)
        {
            double paintExtentLocal = paintOffset(from: 0.0, to: mainAxisPaddingLocal);
            double cacheExtentLocal = cacheOffset(from: 0.0, to: mainAxisPaddingLocal);
            geometry = new SliverGeometry(scrollExtent: mainAxisPaddingLocal, paintExtent: Math.Min(paintExtentLocal, constraintsLocal.remainingPaintExtent), maxPaintExtent: mainAxisPaddingLocal, cacheExtent: cacheExtentLocal);
            return;
        }
        double beforePaddingPaintExtent = paintOffset(from: 0.0, to: beforePaddingLocal);
        double overlapLocal = constraintsLocal.overlap;
        if (overlapLocal > 0L)
        {
            overlapLocal = Math.Max(0.0, constraintsLocal.overlap - beforePaddingPaintExtent);
        }
        child!.layout(constraintsLocal.copyWith(scrollOffset: Math.Max(0.0, constraintsLocal.scrollOffset - beforePaddingLocal), cacheOrigin: Math.Min(0.0, constraintsLocal.cacheOrigin + beforePaddingLocal), overlap: overlapLocal, remainingPaintExtent: constraintsLocal.remainingPaintExtent - paintOffset(from: 0.0, to: beforePaddingLocal), remainingCacheExtent: constraintsLocal.remainingCacheExtent - cacheOffset(from: 0.0, to: beforePaddingLocal), crossAxisExtent: Math.Max(0.0, constraintsLocal.crossAxisExtent - crossAxisPaddingLocal), precedingScrollExtent: beforePaddingLocal + constraintsLocal.precedingScrollExtent), parentUsesSize: true);
        SliverGeometry childLayoutGeometry = child!.geometry!;
        if (childLayoutGeometry.scrollOffsetCorrection is not null)
        {
            geometry = new SliverGeometry(scrollOffsetCorrection: childLayoutGeometry.scrollOffsetCorrection);
            return;
        }
        double scrollExtentLocal = childLayoutGeometry.scrollExtent;
        double beforePaddingCacheExtent = cacheOffset(from: 0.0, to: beforePaddingLocal);
        double afterPaddingCacheExtent = cacheOffset(from: beforePaddingLocal + scrollExtentLocal, to: mainAxisPaddingLocal + scrollExtentLocal);
        double afterPaddingPaintExtent = paintOffset(from: beforePaddingLocal + scrollExtentLocal, to: mainAxisPaddingLocal + scrollExtentLocal);
        double mainAxisPaddingCacheExtent = beforePaddingCacheExtent + afterPaddingCacheExtent;
        double mainAxisPaddingPaintExtent = beforePaddingPaintExtent + afterPaddingPaintExtent;
        double paintExtentAlternate = Math.Min(beforePaddingPaintExtent + Math.Max(childLayoutGeometry.paintExtent, childLayoutGeometry.layoutExtent + afterPaddingPaintExtent), constraintsLocal.remainingPaintExtent);
        geometry = new SliverGeometry(paintOrigin: childLayoutGeometry.paintOrigin, scrollExtent: mainAxisPaddingLocal + scrollExtentLocal, paintExtent: paintExtentAlternate, layoutExtent: Math.Min(mainAxisPaddingPaintExtent + childLayoutGeometry.layoutExtent, paintExtentAlternate), cacheExtent: Math.Min(mainAxisPaddingCacheExtent + childLayoutGeometry.cacheExtent, constraintsLocal.remainingCacheExtent), maxPaintExtent: mainAxisPaddingLocal + childLayoutGeometry.maxPaintExtent, hitTestExtent: Math.Max(mainAxisPaddingPaintExtent + childLayoutGeometry.paintExtent, beforePaddingPaintExtent + childLayoutGeometry.hitTestExtent), hasVisualOverflow: childLayoutGeometry.hasVisualOverflow);
        double calculatedOffset = SliverLibrary.applyGrowthDirectionToAxisDirection(constraintsLocal.axisDirection, constraintsLocal.growthDirection) switch { AxisDirection.up => paintOffset(from: resolvedPaddingLocal.bottom + scrollExtentLocal, to: resolvedPaddingLocal.vertical + scrollExtentLocal), AxisDirection.left => paintOffset(from: resolvedPaddingLocal.right + scrollExtentLocal, to: resolvedPaddingLocal.horizontal + scrollExtentLocal), AxisDirection.right => paintOffset(from: 0.0, to: resolvedPaddingLocal.left), AxisDirection.down => paintOffset(from: 0.0, to: resolvedPaddingLocal.top), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        var childParentData = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
        childParentData.paintOffset = constraintsLocal.axis switch { Axis.horizontal => new global::Doroti.Ui.Offset(calculatedOffset, resolvedPaddingLocal.top), Axis.vertical => new global::Doroti.Ui.Offset(resolvedPaddingLocal.left, calculatedOffset), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        DartRuntimePrimitives.Assert(() => beforePaddingLocal == beforePadding);
        DartRuntimePrimitives.Assert(() => afterPaddingLocal == afterPadding);
        DartRuntimePrimitives.Assert(() => mainAxisPaddingLocal == mainAxisPadding);
        DartRuntimePrimitives.Assert(() => crossAxisPaddingLocal == crossAxisPadding);
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        if ((child is not null) && (child!.geometry!.hitTestExtent > 0.0))
        {
            var childParentData = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
            return result.addWithAxisOffset(mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition, mainAxisOffset: childMainAxisPosition(child!), crossAxisOffset: childCrossAxisPosition(child!), paintOffset: childParentData.paintOffset, hitTest: child!.hitTest);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderSliver)(object)child;
        DartRuntimePrimitives.Assert(() => Equals(__child, this.child));
        return calculatePaintOffset(constraints, from: 0.0, to: beforePadding);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double childCrossAxisPosition(RenderObject child)
    {
        var __child = (RenderSliver)(object)child;
        DartRuntimePrimitives.Assert(() => Equals(__child, this.child));
        DartRuntimePrimitives.Assert(() => resolvedPadding is not null);
        return constraints.axis switch { Axis.horizontal => resolvedPadding!.top, Axis.vertical => resolvedPadding!.left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? childScrollOffset(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        return beforePadding;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => Equals(child, this.child));
        var childParentData = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
        childParentData.applyPaintTransform(transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null) && child!.geometry!.visible)
        {
            var childParentData = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
            context.paintChild(child!, offset + childParentData.paintOffset);
        }
    }

    public override void debugPaint(PaintingContext context, Offset offset)
    {
        base.debugPaint(context, offset);
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPaintSizeEnabled)
                {
                    global::Doroti.Ui.Size parentSize = getAbsoluteSize();
                    global::Doroti.Ui.Rect outerRect = offset & parentSize;
                    global::Doroti.Ui.Rect? innerRect = default!;
                    if (child is not null)
                    {
                        global::Doroti.Ui.Size childSize = child!.getAbsoluteSize();
                        var childParentData = ((SliverPhysicalParentData?)(object?)child!.parentData!)!;
                        innerRect = offset + childParentData.paintOffset & childSize;
                        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.RequireValue(innerRect).top >= outerRect.top);
                        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.RequireValue(innerRect).left >= outerRect.left);
                        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.RequireValue(innerRect).right <= outerRect.right);
                        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.RequireValue(innerRect).bottom <= outerRect.bottom);
                    }
                    DebugLibrary.debugPaintPadding(context.canvas, outerRect, innerRect);
                }
                return true;
            });
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderSliver)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderSliver)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderSliver? child
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
        return (child is not null) ? new List<DiagnosticsNode> { ((Diagnosticable)child!).toDiagnosticsNode(name: "child") } : new List<DiagnosticsNode>();
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
        _padding = padding;
        _textDirection = textDirection;
        System.Diagnostics.Debug.Assert(padding.isNonNegative);
    }

    public override global::Doroti.Framework.Painting.EdgeInsets? resolvedPadding => _resolvedPadding;
    internal virtual void _resolve()
    {
        if (resolvedPadding is not null)
        {
            return;
        }
        _resolvedPadding = padding.resolve(textDirection);
        DartRuntimePrimitives.Assert(() => resolvedPadding!.isNonNegative);
    }

    internal virtual void _markNeedsResolution()
    {
        _resolvedPadding = null;
        markNeedsLayout();
    }

    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding
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
            _markNeedsResolution();
        }
    }
    public virtual global::Doroti.Ui.TextDirection? textDirection
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
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", padding));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", textDirection, defaultValue: null));
    }

}

