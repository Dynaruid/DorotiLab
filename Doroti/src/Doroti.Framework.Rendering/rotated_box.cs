// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/rotated_box.dart
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

public static partial class Rotated_boxLibrary
{
    internal static double _kQuarterTurnsInRadians = (Dart_mathLibrary.pi / 2.0);
}

public class RenderRotatedBox : RenderBox, RenderObjectWithChildMixin<RenderBox>
{
    internal virtual long _quarterTurns { get; set; } = default!;
    internal virtual Matrix4? _paintTransform { get; set; } = default;
    internal virtual LayerHandle<TransformLayer> _transformLayer { get; private set; } = new LayerHandle<TransformLayer>();
    public virtual RenderBox? _child { get; set; } = default;

    public RenderRotatedBox(long quarterTurns, RenderBox? child = null)
    {
        this._quarterTurns = quarterTurns;
    }

    public virtual long quarterTurns
    {
        get => this._quarterTurns;
        set
        {
            var __value = value;
            if ((this._quarterTurns == __value))
            {
                return;
            }
            _quarterTurns = __value;
            markNeedsLayout();
        }
    }
    internal virtual bool _isVertical => ((checked((long)(this.quarterTurns)) & 1L) != 0L);
    public override double computeMinIntrinsicWidth(double height)
    {
        if ((child is null))
        {
            return 0.0;
        }
        return (this._isVertical ? child!.getMinIntrinsicHeight(height) : child!.getMinIntrinsicWidth(height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if ((child is null))
        {
            return 0.0;
        }
        return (this._isVertical ? child!.getMaxIntrinsicHeight(height) : child!.getMaxIntrinsicWidth(height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if ((child is null))
        {
            return 0.0;
        }
        return (this._isVertical ? child!.getMinIntrinsicWidth(width) : child!.getMinIntrinsicHeight(width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if ((child is null))
        {
            return 0.0;
        }
        return (this._isVertical ? child!.getMaxIntrinsicWidth(width) : child!.getMaxIntrinsicHeight(width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if ((child is null))
        {
            return ((BoxConstraints)constraints).smallest;
        }
        global::Doroti.Ui.Size childSize = child!.getDryLayout((this._isVertical ? ((BoxConstraints)constraints).flipped : constraints));
        return (this._isVertical ? new global::Doroti.Ui.Size(childSize.height, childSize.width) : childSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        _paintTransform = null;
        if ((child is not null))
        {
            child!.layout((this._isVertical ? ((BoxConstraints)constraints).flipped : constraints), parentUsesSize: true);
            size = (this._isVertical ? new global::Doroti.Ui.Size(child!.size.height, child!.size.width) : child!.size);
            _paintTransform = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.translateByDouble((size.width / 2.0), (size.height / 2.0), 0, 1);
    __cascade.rotateZ((Rotated_boxLibrary._kQuarterTurnsInRadians * ((this.quarterTurns % 4L))));
    __cascade.translateByDouble((-child!.size.width / 2.0), (-child!.size.height / 2.0), 0, 1);
    return __cascade;
}))();
        }
        else
        {
            size = ((BoxConstraints)constraints).smallest;
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        DartRuntimePrimitives.Assert(() => (((this._paintTransform is not null) || debugNeedsLayout) || (child is null)));
        if (((child is null) || (this._paintTransform is null)))
        {
            return false;
        }
        return result.addWithPaintTransform(transform: this._paintTransform, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, position) =>
        {
            return child!.hitTest(result, position: position);
            return default;
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintChild(PaintingContext context, Offset offset)
    {
        context.paintChild(child!, offset);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null))
        {
            this._transformLayer.layer = context.pushTransform(needsCompositing, offset, this._paintTransform!, (Action<PaintingContext, Offset>)this._paintChild, oldLayer: ((LayerHandle<TransformLayer>)this._transformLayer).layer);
        }
        else
        {
            this._transformLayer.layer = null;
        }
    }

    public override void dispose()
    {
        this._transformLayer.layer = null;
        base.dispose();
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
        if ((this._paintTransform is not null))
        {
            transform.multiply(this._paintTransform!);
        }
        base.applyPaintTransform(__child, transform);
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

