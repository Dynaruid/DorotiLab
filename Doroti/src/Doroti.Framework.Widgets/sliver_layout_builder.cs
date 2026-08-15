// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/sliver_layout_builder.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public delegate Widget SliverLayoutWidgetBuilder(BuildContext context, global::Doroti.Generated.Framework.Rendering.SliverConstraints constraints);

public class SliverLayoutBuilder : ConstrainedLayoutBuilder<global::Doroti.Generated.Framework.Rendering.SliverConstraints>
{
    public SliverLayoutBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.SliverConstraints, Widget> builder = default!) : base(key: key, builder: builder)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new _RenderSliverLayoutBuilder__sliver_layout_builder());
}

internal class _RenderSliverLayoutBuilder__sliver_layout_builder : global::Doroti.Generated.Framework.Rendering.RenderSliver, global::Doroti.Generated.Framework.Rendering.RenderObjectWithChildMixin<global::Doroti.Generated.Framework.Rendering.RenderSliver>, RenderAbstractLayoutBuilderMixin<global::Doroti.Generated.Framework.Rendering.SliverConstraints, global::Doroti.Generated.Framework.Rendering.RenderSliver>, global::Doroti.Generated.Framework.Rendering.IRenderLayoutCallback
{
    public virtual RenderSliver? _child { get; set; } = default;
    public virtual global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints>? _callback { get; set; } = default;

    public override double childMainAxisPosition(global::Doroti.Generated.Framework.Rendering.RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child, ((global::Doroti.Generated.Framework.Rendering.RenderSliver?)((dynamic)this).child))));
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        runLayoutCallback();
        this.child?.layout(this.constraints, parentUsesSize: true);
        geometry = (this.child?.geometry ?? global::Doroti.Generated.Framework.Rendering.SliverGeometry.zero);
    }

    public override void applyPaintTransform(global::Doroti.Generated.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child, ((global::Doroti.Generated.Framework.Rendering.RenderSliver?)((dynamic)this).child))));
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if ((this.child?.geometry?.visible ?? false))
        {
            context.paintChild(this.child!, offset);
        }
    }

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        return (((this.child is not null) && (this.child!.geometry!.hitTestExtent > 0L)) && this.child!.hitTest(result, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderSliver))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderSliver)} child was created by", this.debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((dynamic)child).debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
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
        ((dynamic)this._child)?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        ((dynamic)this._child)?.detach();
    }

    public override void redepthChildren()
    {
        if ((this._child is not null))
        {
            redepthChild(this._child!);
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
    }

    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return ((this.child is not null) ? new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { ((Diagnosticable)this.child!).toDiagnosticsNode(name: "child") } : new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateCallback(global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints> value)
    {
        if ((object.Equals((global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints>)value, (global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints>?)this._callback)))
        {
            return;
        }
        this._callback = (global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints>)value;
        scheduleLayoutCallback();
    }

    public virtual void layoutCallback() => this._callback!(this.constraints);
    public virtual global::Doroti.Generated.Framework.Rendering.SliverConstraints layoutInfo => ((global::Doroti.Generated.Framework.Rendering.SliverConstraints?)(object?)this.constraints)!;
}
