// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/layout_builder.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Widget LayoutWidgetBuilder(BuildContext context, BoxConstraints constraints);

public abstract class AbstractLayoutBuilder<LayoutInfoType> : RenderObjectWidget
{
    protected AbstractLayoutBuilder(Key? key = null) : base(key: key)
    {
    }

    public abstract Func<BuildContext, LayoutInfoType, Widget> builder { get; }
    public override RenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<RenderObjectElement>(new _LayoutBuilderElement__layout_builder<LayoutInfoType>(this));
    public virtual bool updateShouldRebuild(AbstractLayoutBuilder<LayoutInfoType> oldWidget) => true;
    public abstract override RenderObject createRenderObject(BuildContext context);
}

public abstract class ConstrainedLayoutBuilder<ConstraintType> : AbstractLayoutBuilder<ConstraintType> where ConstraintType : Constraints
{
    private Func<BuildContext, ConstraintType, Widget> __field_builder = default!;
    public override Func<BuildContext, ConstraintType, Widget> builder { get => __field_builder; }

    protected ConstrainedLayoutBuilder(Key? key = null, Func<BuildContext, ConstraintType, Widget> builder = default!) : base(key: key)
    {
        __field_builder = builder;
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
                __late__buildScope = new BuildScope(scheduleRebuild: () => _scheduleRebuild());
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

    public override RenderObject renderObject => base.renderObject;
    private RenderAbstractLayoutBuilderMixin<LayoutInfoType, RenderObject> layoutBuilder =>
        (RenderAbstractLayoutBuilderMixin<LayoutInfoType, RenderObject>)renderObject;
    public override BuildScope buildScope => _buildScope;
    internal virtual void _scheduleRebuild()
    {
        if (_deferredCallbackScheduled)
        {
            return;
        }
        bool deferMarkNeedsLayout = Scheduler.SchedulerBinding.instance.schedulerPhase switch { Scheduler.SchedulerPhase.idle => true, Scheduler.SchedulerPhase.postFrameCallbacks => true, Scheduler.SchedulerPhase.transientCallbacks or Scheduler.SchedulerPhase.midFrameMicrotasks => false, Scheduler.SchedulerPhase.persistentCallbacks => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        if (!deferMarkNeedsLayout)
        {
            renderObject.scheduleLayoutCallback();
            return;
        }
        _deferredCallbackScheduled = true;
        Scheduler.SchedulerBinding.instance.scheduleFrameCallback(_frameCallback);
    }

    internal virtual void _frameCallback(Duration timestamp)
    {
        _deferredCallbackScheduled = false;
        if (mounted)
        {
            renderObject.scheduleLayoutCallback();
        }
    }

    public override void visitChildren(Action<Element> visitor)
    {
        if (_child is not null)
        {
            visitor(_child!);
        }
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child, _child));
        _child = null;
        base.forgetChild(child);
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        layoutBuilder._updateCallback(_rebuildWithConstraints);
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (AbstractLayoutBuilder<LayoutInfoType>)newWidget;
        DartRuntimePrimitives.Assert(() => !Equals(widget, __newWidget));
        var oldWidget = ((AbstractLayoutBuilder<LayoutInfoType>?)widget)!;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => Equals(widget, __newWidget));
        layoutBuilder._updateCallback(_rebuildWithConstraints);
        if (__newWidget.updateShouldRebuild(oldWidget))
        {
            _needsBuild = true;
            renderObject.scheduleLayoutCallback();
        }
    }

    public override void markNeedsBuild()
    {
        renderObject.scheduleLayoutCallback();
        _needsBuild = true;
    }

    public override void performRebuild()
    {
        renderObject.scheduleLayoutCallback();
        _needsBuild = true;
        base.performRebuild();
    }

    public override void unmount()
    {
        layoutBuilder._callback = null;
        base.unmount();
    }

    internal virtual void _rebuildWithConstraints(Constraints __unused0)
    {
        LayoutInfoType layoutInfoLocal = DartRuntimePrimitives.ConvertValue<LayoutInfoType>(layoutBuilder.layoutInfo);
        void updateChildCallback()
        {
            Widget built = default!;
            try
            {
                DartRuntimePrimitives.Assert(() => EqualityComparer<LayoutInfoType>.Default.Equals(layoutInfoLocal, layoutBuilder.layoutInfo));
                built = ((AbstractLayoutBuilder<LayoutInfoType>?)widget)!.builder(this, layoutInfoLocal);
                DebugLibrary.debugWidgetBuilderValue(widget, built);
            }
            catch (Exception e)
            {
                var stack = new System.Diagnostics.StackTrace();
                built = ErrorWidget.builder(Layout_builderLibrary._reportException(new ErrorDescription($"building {widget}"), e, stack, informationCollector: () => new List<DiagnosticsNode>()));
            }
            try
            {
                _child = updateChild(_child, built, null);
                DartRuntimePrimitives.Assert(() => _child is not null);
            }
            catch (Exception eLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                built = ErrorWidget.builder(Layout_builderLibrary._reportException(new ErrorDescription($"building {widget}"), eLocal, stackLocal, informationCollector: () => new List<DiagnosticsNode>()));
                _child = updateChild(null, built, slot);
            }
        }
        Action? callback = (_needsBuild || (!EqualityComparer<LayoutInfoType>.Default.Equals(layoutInfoLocal, _previousLayoutInfo))) ? updateChildCallback : null;
        owner!.buildScope(this, callback);
    }

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        var renderObjectLocal = renderObject;
        var childOwner = (IRenderObjectWithChild)renderObjectLocal;
        DartRuntimePrimitives.Assert(() => slot is null);
        childOwner.child = child;
        DartRuntimePrimitives.Assert(() => Equals(renderObjectLocal, renderObject));
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        var renderObjectLocal = renderObject;
        var childOwner = (IRenderObjectWithChild)renderObjectLocal;
        DartRuntimePrimitives.Assert(() => Equals(childOwner.child, child));
        childOwner.child = null;
        DartRuntimePrimitives.Assert(() => Equals(renderObjectLocal, renderObject));
    }

}

public interface RenderAbstractLayoutBuilderMixin<LayoutInfoType, out ChildType> where ChildType : RenderObject
{
    Action<Constraints>? _callback { get; set; }

    public void _updateCallback(Action<Constraints> value);
    public void layoutCallback();
    public LayoutInfoType layoutInfo { get; }
}

public delegate void RenderConstrainedLayoutBuilder<LayoutInfoType, ChildType>() where ChildType : RenderObject;

public class LayoutBuilder : ConstrainedLayoutBuilder<BoxConstraints>
{
    public LayoutBuilder(Key? key = null, Func<BuildContext, BoxConstraints, Widget> builder = default!) : base(key: key, builder: builder)
    {
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new _RenderLayoutBuilder__layout_builder());
}

internal class _RenderLayoutBuilder__layout_builder : RenderBox, RenderObjectWithChildMixin<RenderBox>, RenderAbstractLayoutBuilderMixin<BoxConstraints, RenderBox>, IRenderLayoutCallback
{
    public virtual RenderBox? _child { get; set; } = default;
    public virtual Action<Constraints>? _callback { get; set; } = default;

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

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: "Calculating the dry layout would require running the layout callback " + "speculatively, which might mutate the live render object tree."));
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: "Calculating the dry baseline would require running the layout callback " + "speculatively, which might mutate the live render object tree."));
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        runLayoutCallback();
        if (child is not null)
        {
            child!.layout(constraintsLocal, parentUsesSize: true);
            size = constraintsLocal.constrain(child!.size);
        }
        else
        {
            size = constraintsLocal.biggest;
        }
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return child?.getDistanceToActualBaseline(baseline) ?? base.computeDistanceToActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        return child?.hitTest(result, position: position) ?? false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            context.paintChild(child!, offset);
        }
    }

    internal virtual bool _debugThrowIfNotCheckingIntrinsics()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!debugCheckingIntrinsics)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("LayoutBuilder does not support returning intrinsic dimensions.\n" + "Calculating the intrinsic dimensions would require running the layout " + "callback speculatively, which might mutate the live render object tree."));
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
                if (child is not RenderBox)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
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
    public virtual BoxConstraints layoutInfo => constraints!;
}

public static partial class Layout_builderLibrary
{
    internal static FlutterErrorDetails _reportException(DiagnosticsNode context, object exception, System.Diagnostics.StackTrace stack, InformationCollector? informationCollector = null)
    {
        var details = new FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: context, informationCollector: informationCollector);
        FlutterError.reportError(details);
        return details;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
