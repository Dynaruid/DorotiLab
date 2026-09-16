// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver_layout_builder.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Widget SliverLayoutWidgetBuilder(BuildContext context, SliverConstraints constraints);

public class SliverLayoutBuilder : ConstrainedLayoutBuilder<SliverConstraints>
{
    public SliverLayoutBuilder(Key? key = null, Func<BuildContext, SliverConstraints, Widget> builder = default!) : base(key: key, builder: builder)
    {
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new _RenderSliverLayoutBuilder__sliver_layout_builder());
}

internal class _RenderSliverLayoutBuilder__sliver_layout_builder : RenderSliver, RenderObjectWithChildMixin<RenderSliver>, RenderAbstractLayoutBuilderMixin<SliverConstraints, RenderSliver>, IRenderLayoutCallback
{
    public virtual RenderSliver? _child { get; set; } = default;
    public virtual Action<Constraints>? _callback { get; set; } = default;

    public override double childMainAxisPosition(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child, this.child));
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        runLayoutCallback();
        child?.layout(constraints, parentUsesSize: true);
        geometry = child?.geometry ?? SliverGeometry.zero;
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => Equals(child, this.child));
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child?.geometry?.visible ?? false)
        {
            context.paintChild(child!, offset);
        }
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        return (child is not null) && (child!.geometry!.hitTestExtent > 0L) && child!.hitTest(result, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderSliver)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderSliver)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
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

    public virtual void _updateCallback(Action<Constraints> value)
    {
        if (Equals(value, _callback))
        {
            return;
        }
        _callback = value;
        scheduleLayoutCallback();
    }

    public virtual void layoutCallback() => _callback!(constraints);
    public virtual SliverConstraints layoutInfo => constraints!;
}
