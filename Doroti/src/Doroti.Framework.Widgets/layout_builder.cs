// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/layout_builder.dart
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

public delegate Widget LayoutWidgetBuilder(BuildContext context, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints);

public abstract class AbstractLayoutBuilder<LayoutInfoType> : RenderObjectWidget
{
    protected AbstractLayoutBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public abstract global::System.Func<BuildContext, LayoutInfoType, Widget> builder { get; }
    public override RenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<RenderObjectElement>(new _LayoutBuilderElement__layout_builder<LayoutInfoType>(this));
    public virtual bool updateShouldRebuild(AbstractLayoutBuilder<LayoutInfoType> oldWidget) => true;
    public abstract override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context);
}

public abstract class ConstrainedLayoutBuilder<ConstraintType> : AbstractLayoutBuilder<ConstraintType> where ConstraintType : global::Doroti.Generated.Framework.Rendering.Constraints
{
    private global::System.Func<BuildContext, ConstraintType, Widget> __field_builder = default!;
    public override global::System.Func<BuildContext, ConstraintType, Widget> builder { get => __field_builder; }

    protected ConstrainedLayoutBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, ConstraintType, Widget> builder = default!) : base(key: key)
    {
        this.__field_builder = builder;
    }

}

internal class _LayoutBuilderElement__layout_builder<LayoutInfoType> : RenderObjectElement
{
    internal virtual Element? _child { get; set; } = default;
    private bool __late__buildScope_initialized;
    private BuildScope __late__buildScope = default!;
    internal virtual BuildScope _buildScope
    {
        get
        {
            if (!__late__buildScope_initialized)
            {
                __late__buildScope = new BuildScope(scheduleRebuild: () => this._scheduleRebuild());
                __late__buildScope_initialized = true;
            }
            return __late__buildScope;
        }
    }
    internal virtual bool _deferredCallbackScheduled { get; set; } = false;
    internal virtual LayoutInfoType? _previousLayoutInfo { get; set; } = default;
    internal virtual bool _needsBuild { get; set; } = true;

    internal _LayoutBuilderElement__layout_builder(AbstractLayoutBuilder<LayoutInfoType> widget) : base(widget)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject renderObject => base.renderObject;
    public override BuildScope buildScope => this._buildScope;
    internal virtual void _scheduleRebuild()
    {
        if (this._deferredCallbackScheduled)
        {
            return;
        }
        bool deferMarkNeedsLayout__5728 = (global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase switch { global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.idle => true, global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.postFrameCallbacks => true, global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.transientCallbacks or global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.midFrameMicrotasks => false, global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.persistentCallbacks => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if (!deferMarkNeedsLayout__5728)
        {
            ((dynamic)this.renderObject).scheduleLayoutCallback();
            return;
        }
        _deferredCallbackScheduled = true;
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.scheduleFrameCallback((global::System.Action<Duration>)this._frameCallback);
    }

    internal virtual void _frameCallback(Duration timestamp)
    {
        _deferredCallbackScheduled = false;
        if (this.mounted)
        {
            ((dynamic)this.renderObject).scheduleLayoutCallback();
        }
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child, this._child)));
        _child = null;
        base.forgetChild(child);
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        ((dynamic)this.renderObject)._updateCallback((global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints>)this._rebuildWithConstraints);
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (AbstractLayoutBuilder<LayoutInfoType>)(object)newWidget;
        DartRuntimePrimitives.Assert(() => (!object.Equals(this.widget, __newWidget)));
        var oldWidget__7101 = ((AbstractLayoutBuilder<LayoutInfoType>?)(object?)this.widget)!;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        ((dynamic)this.renderObject)._updateCallback((global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints>)this._rebuildWithConstraints);
        if (__newWidget.updateShouldRebuild(oldWidget__7101))
        {
            _needsBuild = true;
            ((dynamic)this.renderObject).scheduleLayoutCallback();
        }
    }

    public override void markNeedsBuild()
    {
        ((dynamic)this.renderObject).scheduleLayoutCallback();
        _needsBuild = true;
    }

    public override void performRebuild()
    {
        ((dynamic)this.renderObject).scheduleLayoutCallback();
        _needsBuild = true;
        base.performRebuild();
    }

    public override void unmount()
    {
        ((dynamic)this.renderObject)._callback = null;
        base.unmount();
    }

    internal virtual void _rebuildWithConstraints(global::Doroti.Generated.Framework.Rendering.Constraints __unused0)
    {
        LayoutInfoType layoutInfo__8823 = DartRuntimePrimitives.ConvertValue<LayoutInfoType>(((dynamic)this.renderObject).layoutInfo);
        void updateChildCallback()
        {
            Widget built__8954 = default!;
            try
            {
                DartRuntimePrimitives.Assert(() => EqualityComparer<LayoutInfoType>.Default.Equals(layoutInfo__8823, ((dynamic)this.renderObject).layoutInfo));
                built__8954 = (((AbstractLayoutBuilder<LayoutInfoType>?)(object?)this.widget)!).builder(this, layoutInfo__8823);
                global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugWidgetBuilderValue(this.widget, built__8954);
            }
            catch (Exception e__9184)
            {
                var stack__9187 = new System.Diagnostics.StackTrace();
                built__8954 = ErrorWidget.builder(Layout_builderLibrary._reportException(new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"building {this.widget}"), e__9184, stack__9187, informationCollector: (() => new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>())));
            }
            try
            {
                _child = updateChild(this._child, built__8954, null);
                DartRuntimePrimitives.Assert(() => (this._child is not null));
            }
            catch (Exception e__9636)
            {
                var stack__9639 = new System.Diagnostics.StackTrace();
                built__8954 = ErrorWidget.builder(Layout_builderLibrary._reportException(new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"building {this.widget}"), e__9636, stack__9639, informationCollector: (() => new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>())));
                _child = updateChild(((Element)(object)null), built__8954, this.slot);
            }
        }
        global::System.Action? callback__10147 = ((global::System.Action)((this._needsBuild || (!EqualityComparer<LayoutInfoType>.Default.Equals(layoutInfo__8823, this._previousLayoutInfo))) ? updateChildCallback : null));
        this.owner!.buildScope(this, () => callback__10147());
    }

    public override void insertRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        var renderObject__10429 = this.renderObject;
        var childProperty__10430 = renderObject__10429.GetType().GetProperty("child")
            ?? throw new InvalidOperationException($"{renderObject__10429.GetType().FullName} does not expose a layout-builder child property.");
        DartRuntimePrimitives.Assert(() => (slot is null));
        childProperty__10430.SetValue(renderObject__10429, child);
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObject__10429, this.renderObject)));
    }

    public override void moveRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        var renderObject__10898 = this.renderObject;
        var childProperty__10899 = renderObject__10898.GetType().GetProperty("child")
            ?? throw new InvalidOperationException($"{renderObject__10898.GetType().FullName} does not expose a layout-builder child property.");
        DartRuntimePrimitives.Assert(() => (object.Equals((global::Doroti.Generated.Framework.Rendering.RenderObject?)childProperty__10899.GetValue(renderObject__10898), child)));
        childProperty__10899.SetValue(renderObject__10898, null);
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObject__10898, this.renderObject)));
    }

}

public interface RenderAbstractLayoutBuilderMixin<LayoutInfoType, ChildType> where ChildType : global::Doroti.Generated.Framework.Rendering.RenderObject
{
    global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints>? _callback { get; set; }

    public void _updateCallback(global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints> value);
    public void layoutCallback();
    public LayoutInfoType layoutInfo { get; }
}

public delegate void RenderConstrainedLayoutBuilder<LayoutInfoType, ChildType>() where ChildType : global::Doroti.Generated.Framework.Rendering.RenderObject;

public class LayoutBuilder : ConstrainedLayoutBuilder<global::Doroti.Generated.Framework.Rendering.BoxConstraints>
{
    public LayoutBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, Widget> builder = default!) : base(key: key, builder: builder)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new _RenderLayoutBuilder__layout_builder());
}

internal class _RenderLayoutBuilder__layout_builder : global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.RenderObjectWithChildMixin<global::Doroti.Generated.Framework.Rendering.RenderBox>, RenderAbstractLayoutBuilderMixin<global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Rendering.RenderBox>, global::Doroti.Generated.Framework.Rendering.IRenderLayoutCallback
{
    public virtual RenderBox? _child { get; set; } = default;
    public virtual global::System.Action<global::Doroti.Generated.Framework.Rendering.Constraints>? _callback { get; set; } = default;

    public override double computeMinIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => _debugThrowIfNotCheckingIntrinsics());
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => _debugThrowIfNotCheckingIntrinsics());
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => _debugThrowIfNotCheckingIntrinsics());
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => _debugThrowIfNotCheckingIntrinsics());
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: "Calculating the dry layout would require running the layout callback " + "speculatively, which might mutate the live render object tree."));
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: "Calculating the dry baseline would require running the layout callback " + "speculatively, which might mutate the live render object tree."));
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints__16670 = this.constraints;
        runLayoutCallback();
        if ((this.child is not null))
        {
            this.child!.layout(constraints__16670, parentUsesSize: true);
            size = constraints__16670.constrain(this.child!.size);
        }
        else
        {
            size = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__16670).biggest;
        }
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return ((this.child?.getDistanceToActualBaseline(baseline) ?? (double)base.computeDistanceToActualBaseline(baseline)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        return (this.child?.hitTest(result, position: position) ?? false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if ((this.child is not null))
        {
            context.paintChild(this.child!, offset);
        }
    }

    internal virtual bool _debugThrowIfNotCheckingIntrinsics()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!global::Doroti.Generated.Framework.Rendering.RenderObject.debugCheckingIntrinsics)
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("LayoutBuilder does not support returning intrinsic dimensions.\n" + "Calculating the intrinsic dimensions would require running the layout " + "callback speculatively, which might mutate the live render object tree."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderBox))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((dynamic)child).debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
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
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints layoutInfo => ((global::Doroti.Generated.Framework.Rendering.BoxConstraints?)(object?)this.constraints)!;
}

public static partial class Layout_builderLibrary
{
    internal static global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails _reportException(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode context, object exception, global::System.Diagnostics.StackTrace stack, InformationCollector? informationCollector = null)
    {
        var details__18044 = new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: context, informationCollector: (InformationCollector?)informationCollector);
        FlutterError.reportError(details__18044);
        return details__18044;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
