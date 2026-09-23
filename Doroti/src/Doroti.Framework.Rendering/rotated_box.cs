// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/rotated_box.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public static partial class Rotated_boxLibrary
{
    internal static double _kQuarterTurnsInRadians = Math.PI / 2.0;
}

public class RenderRotatedBox : RenderBox, RenderObjectWithChildMixin<RenderBox>
{
    internal virtual long _quarterTurns { get; set; } = default!;
    internal virtual Matrix4? _paintTransform { get; set; } = default;
    internal virtual LayerHandle<TransformLayer> _transformLayer { get; private set; } =
        new LayerHandle<TransformLayer>();
    public virtual RenderBox? _child { get; set; } = default;

    public RenderRotatedBox(long quarterTurns, RenderBox? child = null)
    {
        _quarterTurns = quarterTurns;
    }

    public virtual long quarterTurns
    {
        get => _quarterTurns;
        set
        {
            var __value = value;
            if (_quarterTurns == __value)
            {
                return;
            }
            _quarterTurns = __value;
            markNeedsLayout();
        }
    }
    internal virtual bool _isVertical => (checked(quarterTurns) & 1L) != 0L;

    public override double computeMinIntrinsicWidth(double height)
    {
        if (child is null)
        {
            return 0.0;
        }
        return _isVertical
            ? child!.getMinIntrinsicHeight(height)
            : child!.getMinIntrinsicWidth(height);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if (child is null)
        {
            return 0.0;
        }
        return _isVertical
            ? child!.getMaxIntrinsicHeight(height)
            : child!.getMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if (child is null)
        {
            return 0.0;
        }
        return _isVertical
            ? child!.getMinIntrinsicWidth(width)
            : child!.getMinIntrinsicHeight(width);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if (child is null)
        {
            return 0.0;
        }
        return _isVertical
            ? child!.getMaxIntrinsicWidth(width)
            : child!.getMaxIntrinsicHeight(width);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if (child is null)
        {
            return constraints.smallest;
        }
        Size childSize = child!.getDryLayout(_isVertical ? constraints.flipped : constraints);
        return _isVertical ? new Size(childSize.height, childSize.width) : childSize;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        _paintTransform = null;
        if (child is not null)
        {
            child!.layout(_isVertical ? constraints.flipped : constraints, parentUsesSize: true);
            size = _isVertical ? new Size(child!.size.height, child!.size.width) : child!.size;
            _paintTransform = (
                (Func<Matrix4>)(
                    () =>
                    {
                        var __cascade = Matrix4.identity();
                        __cascade.translateByDouble(size.width / 2.0, size.height / 2.0, 0, 1);
                        __cascade.rotateZ(
                            Rotated_boxLibrary._kQuarterTurnsInRadians * (quarterTurns % 4L)
                        );
                        __cascade.translateByDouble(
                            -child!.size.width / 2.0,
                            -child!.size.height / 2.0,
                            0,
                            1
                        );
                        return __cascade;
                    }
                )
            )();
        }
        else
        {
            size = constraints.smallest;
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        DartRuntimePrimitives.Assert(() =>
            (_paintTransform is not null) || debugNeedsLayout || (child is null)
        );
        if ((child is null) || (_paintTransform is null))
        {
            return false;
        }
        return result.addWithPaintTransform(
            transform: _paintTransform,
            position: position,
            hitTest: (result, position) =>
            {
                return child!.hitTest(result, position: position);
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _paintChild(PaintingContext context, Offset offset)
    {
        context.paintChild(child!, offset);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            _transformLayer.layer = context.pushTransform(
                needsCompositing,
                offset,
                _paintTransform!,
                _paintChild,
                oldLayer: _transformLayer.layer
            );
        }
        else
        {
            _transformLayer.layer = null;
        }
    }

    public override void dispose()
    {
        _transformLayer.layer = null;
        base.dispose();
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
        if (_paintTransform is not null)
        {
            transform.multiply(_paintTransform!);
        }
        base.applyPaintTransform(__child, transform);
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
