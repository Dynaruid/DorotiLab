// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/framework.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

/// <summary>Non-generic CLR contract for Dart's raw State type.</summary>
public interface IState
{
    StatefulWidget? _widget { get; set; }
    _StateLifecycle__framework _debugLifecycleState { get; set; }
    StatefulElement? _element { get; set; }
    StatefulWidget widget { get; }
    BuildContext context { get; }
    bool mounted { get; }
    void initState();
    void didUpdateWidget(StatefulWidget oldWidget);
    void reassemble();
    void setState(Action fn);
    void deactivate();
    void activate();
    void dispose();
    Widget build(BuildContext context);
    void didChangeDependencies();
}

internal class _DebugOnly__framework
{
    internal _DebugOnly__framework()
    {
    }

}

public static partial class FrameworkLibrary
{
    internal static _DebugOnly__framework _debugOnly = new _DebugOnly__framework();
}

public class ObjectKey : LocalKey
{
    public virtual object? value { get; private set; }

    public ObjectKey(object? value)
    {
        this.value = value;
    }

    public override bool Equals(object? other)
    {
        var __other = other as ObjectKey;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ObjectKey) && DartRuntimePrimitives.Identical(__other.value, value);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(GetType(), Dart_coreLibrary.identityHashCode(value)));
    public override string ToString()
    {
        if (Equals(GetType(), typeof(ObjectKey)))
        {
            return $"[{DiagnosticsLibrary.describeIdentity(value)}]";
        }
        return $"[{objectRuntimeTypeFunctions.objectRuntimeType(this, "ObjectKey")} {DiagnosticsLibrary.describeIdentity(value)}]";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class GlobalKeyBase : Key
{
    internal virtual Element? _currentElement => WidgetsBinding.instance.buildOwner!._globalKeyRegistry.GetValueOrDefault(this);
}

public class GlobalKey<T> : GlobalKeyBase where T : IState
{
    public new static GlobalKey<T> Create(string? debugLabel = null) => new LabeledGlobalKey<T>(debugLabel);

    public GlobalKey()
    {
    }

    public virtual BuildContext? currentContext => DartRuntimePrimitives.ConvertValue<BuildContext>(_currentElement);
    public virtual Widget? currentWidget => _currentElement?.widget;
    public virtual T? currentState => _currentElement switch { StatefulElement { state: T stateLocal } __object7625 => stateLocal, _ => default };
    public GlobalKey(string? debugLabel) { _ = debugLabel; }
}

public class LabeledGlobalKey<T> : GlobalKey<T> where T : IState
{
    internal virtual string? _debugLabel { get; private set; }

    public LabeledGlobalKey(string? _debugLabel)
    {
        this._debugLabel = _debugLabel;
    }

    public override string ToString()
    {
        var label = (_debugLabel is not null) ? $" {_debugLabel}" : "";
        if (Equals(GetType(), typeof(LabeledGlobalKey<T>)))
        {
            return $"[GlobalKey#{DiagnosticsLibrary.shortHash(this)}{label}]";
        }
        return $"[{DiagnosticsLibrary.describeIdentity(this)}{label}]";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class GlobalObjectKey<T> : GlobalKey<T> where T : IState
{
    public virtual object value { get; private set; } = default!;

    public GlobalObjectKey(object value)
    {
        this.value = value;
    }

    public override bool Equals(object? other)
    {
        var __other = other as GlobalObjectKey<T>;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is GlobalObjectKey<T>) && DartRuntimePrimitives.Identical(__other.value, value);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(Dart_coreLibrary.identityHashCode(value));
    public override string ToString()
    {
        string selfType = objectRuntimeTypeFunctions.objectRuntimeType(this, "GlobalObjectKey");
        var suffix = "<State<StatefulWidget>>";
        if (selfType.endsWith(suffix))
        {
            selfType = selfType.substring(0L, selfType.Length - suffix.Length);
        }
        return $"[{selfType} {DiagnosticsLibrary.describeIdentity(value)}]";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class Widget : DiagnosticableTree
{
    public virtual Key? key { get; private set; }
    public Widget() { }


    protected Widget(Key? key = null)
    {
        this.key = key;
    }

    public virtual string toStringShallow(string joiner = ", ", DiagnosticLevel minLevel = DiagnosticLevel.debug) => throw new NotSupportedException();
    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, long wrapWidth = 65) => throw new NotSupportedException();
    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null) => throw new NotSupportedException();
    public virtual List<DiagnosticsNode> debugDescribeChildren() => throw new NotSupportedException();
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info) => toStringShort();
    public abstract Element createElement();
    public virtual string toStringShort()
    {
        string @type = objectRuntimeTypeFunctions.objectRuntimeType(this, "Widget");
        return (key is null) ? @type : $"{@type}-{key}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.defaultDiagnosticsTreeStyle = DiagnosticsTreeStyle.dense;
    }

    public override bool Equals(object? other)
    {
        var __other = other as Widget;
        if (__other is null) return false;
        return base.Equals(__other);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(base.GetHashCode());
    public static bool canUpdate(Widget oldWidget, Widget newWidget)
    {
        return Equals(DartRuntimePrimitives.RuntimeType(oldWidget), DartRuntimePrimitives.RuntimeType(newWidget)) && Equals(oldWidget.key, newWidget.key);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _debugConcreteSubtype(Widget widget)
    {
        return (widget is StatefulWidget) ? 1L : ((widget is StatelessWidget) ? 2L : 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class StatelessWidget : Widget
{
    protected StatelessWidget(Key? key = null) : base(key: key)
    {
    }

    public override StatelessElement createElement() => new StatelessElement(this);
    public abstract Widget build(BuildContext context);
}

public abstract class StatefulWidget : Widget
{
    protected StatefulWidget(Key? key = null) : base(key: key)
    {
    }

    public override StatefulElement createElement() => new StatefulElement(this);
    public abstract IState createState();
}

public enum _StateLifecycle__framework
{
    created,
    initialized,
    ready,
    defunct
}

public delegate void StateSetter(Action fn);

public abstract class State<T> : IState, Diagnosticable where T : StatefulWidget
{
    internal virtual T? _widget { get; set; } = default;
    internal virtual _StateLifecycle__framework _debugLifecycleState { get; set; } = _StateLifecycle__framework.created;
    internal virtual StatefulElement? _element { get; set; } = default;

    public virtual T widget => DartRuntimePrimitives.ConvertValue<T>(_widget!);
    internal virtual bool _debugTypesAreRight(Widget widget) => widget is T;
    public virtual BuildContext context
    {
        get
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (_element is null)
                    {
                        throw DartRuntimePrimitives.AsException(FlutterError.Create("This widget has been unmounted, so the State no longer has a context (and should be considered defunct). \n" + "Consider canceling any active work during \"dispose\" or using the \"mounted\" getter to determine if the State is still active."));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return _element!;
        }
    }
    public virtual bool mounted => DartRuntimePrimitives.ConvertValue<bool>(_element is not null);
    public virtual void initState()
    {
        DartRuntimePrimitives.Assert(() => Equals(_debugLifecycleState, _StateLifecycle__framework.created));
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchCreated("widgets", "State", this));
    }

    public virtual void didUpdateWidget(T oldWidget)
    {
    }

    public virtual void reassemble()
    {
    }

    public virtual void setState(Action fn)
    {
        FrameworkWorkCounters.Add(FrameworkWork.SetState);
        DartRuntimePrimitives.Assert(() =>
            {
                if (Equals(_debugLifecycleState, _StateLifecycle__framework.defunct))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"setState() called after dispose(): {this}"), new ErrorDescription("This error happens if you call setState() on a State object for a widget that " + "no longer appears in the widget tree (e.g., whose parent widget no longer " + "includes the widget in its build). This error can occur when code calls " + "setState() from a timer, from an animation callback, or after an " + "asynchronous operation (such as an awaited network request or other " + "Future) completes after the widget has been removed from the tree."), new ErrorHint("The preferred solution is " + "to cancel the timer or stop listening to the animation in the dispose() " + "callback. Another solution is to check the \"mounted\" property of this " + "object before calling setState() to ensure the object is still in the " + "tree."), new ErrorHint("This error might indicate a memory leak if setState() is being called " + "because another object is retaining a reference to this State object " + "after it has been removed from the tree. To avoid memory leaks, " + "consider breaking the reference to this object during dispose().") }));
                }
                if (Equals(_debugLifecycleState, _StateLifecycle__framework.created) && !mounted)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"setState() called in constructor: {this}"), new ErrorHint("This happens when you call setState() on a State object for a widget that " + "hasn't been inserted into the widget tree yet. It is not necessary to call " + "setState() in the constructor, since the state is already assumed to be dirty " + "when it is initially created.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        object? result = DartRuntimePrimitives.CaptureVoid(() => fn());
        DartRuntimePrimitives.Assert(() =>
            {
                if (result is Future)
                {
                    Future result__53490__as53542 = (Future)result;
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("setState() callback argument returned a Future."), new ErrorDescription($"The setState() method on {this} was called with a closure or method that " + "returned a Future. Maybe it is marked as \"async\"."), new ErrorHint("Instead of performing asynchronous work inside a call to setState(), first " + "execute the work (without updating the widget state), and then synchronously " + "update the state inside a call to setState().") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _element!.markNeedsBuild();
    }

    public virtual void deactivate()
    {
    }

    public virtual void activate()
    {
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => Equals(_debugLifecycleState, _StateLifecycle__framework.ready));
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLifecycleState = _StateLifecycle__framework.defunct;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
    }

    public abstract Widget build(BuildContext context);
    public virtual void didChangeDependencies()
    {
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                properties.add(new EnumProperty<_StateLifecycle__framework>("lifecycle state", _debugLifecycleState, defaultValue: _StateLifecycle__framework.ready));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        properties.add(new ObjectFlagProperty<T>("_widget", _widget, ifNull: "no widget"));
        properties.add(new ObjectFlagProperty<StatefulElement>("_element", _element, ifNull: "not mounted"));
    }

    StatefulWidget? IState._widget { get => _widget; set => _widget = (T?)value; }
    _StateLifecycle__framework IState._debugLifecycleState { get => _debugLifecycleState; set => _debugLifecycleState = value; }
    StatefulElement? IState._element { get => _element; set => _element = value; }
    StatefulWidget IState.widget => widget;
    void IState.didUpdateWidget(StatefulWidget oldWidget) => didUpdateWidget((T)oldWidget);
    public virtual void didChangeAppLifecycleState(AppLifecycleState state) { }
    public virtual void didChangeAccessibilityFeatures() { }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class ProxyWidget : Widget
{
    public virtual Widget child { get; private set; } = default!;

    protected ProxyWidget(Key? key = null, Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

}

public abstract class ParentDataWidget<T> : ProxyWidget
{
    protected ParentDataWidget(Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public override ParentDataElement<T> createElement() => new ParentDataElement<T>(this);
    public virtual bool debugIsValidRenderObject(RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => !Equals(typeof(T), typeof(object)));
        DartRuntimePrimitives.Assert(() => !Equals(typeof(T), typeof(ParentData)));
        return renderObject.parentData is T;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Type debugTypicalAncestorWidgetClass { get; }
    public virtual string debugTypicalAncestorWidgetDescription => $"{debugTypicalAncestorWidgetClass}";
    internal virtual IEnumerable<DiagnosticsNode> _debugDescribeIncorrectParentDataType(ParentData? parentData, RenderObjectWidget? parentDataCreator = null, DiagnosticsNode? ownershipChain = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(typeof(T), typeof(object)));
        DartRuntimePrimitives.Assert(() => !Equals(typeof(T), typeof(ParentData)));
        var description = $"The ParentDataWidget {this} wants to apply ParentData of type {typeof(T)} to a RenderObject";
        return new List<DiagnosticsNode> { new ErrorHint($"Usually, this means that the {GetType()} widget has the wrong ancestor RenderObjectWidget. " + $"Typically, {GetType()} widgets are placed directly inside {debugTypicalAncestorWidgetDescription} widgets.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract void applyParentData(RenderObject renderObject);
    public virtual bool debugCanApplyOutOfTurn() => false;
}

public abstract class InheritedWidget : ProxyWidget
{
    protected InheritedWidget(Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public override InheritedElement createElement() => new InheritedElement(this);
    public abstract bool updateShouldNotify(InheritedWidget oldWidget);
    protected InheritedWidget(Widget child) : this(null, child) { }
}

public abstract class RenderObjectWidget : Widget
{
    protected RenderObjectWidget(Key? key = null) : base(key: key)
    {
    }

    public abstract override RenderObjectElement createElement();
    public abstract RenderObject createRenderObject(BuildContext context);
    public virtual void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
    }

    public virtual void didUnmountRenderObject(RenderObject renderObject)
    {
    }

}

public abstract class LeafRenderObjectWidget : RenderObjectWidget
{
    protected LeafRenderObjectWidget(Key? key = null) : base(key: key)
    {
    }

    public override LeafRenderObjectElement createElement() => new LeafRenderObjectElement(this);
}

public abstract class SingleChildRenderObjectWidget : RenderObjectWidget
{
    public virtual Widget? child { get; private set; }

    protected SingleChildRenderObjectWidget(Key? key = null, Widget? child = null) : base(key: key)
    {
        this.child = child;
    }

    public override SingleChildRenderObjectElement createElement() => new SingleChildRenderObjectElement(this);
}

public abstract class MultiChildRenderObjectWidget : RenderObjectWidget
{
    public virtual List<Widget> children { get; private set; } = default!;

    protected MultiChildRenderObjectWidget(Key? key = null, List<Widget> children = default!) : base(key: key)
    {
        List<Widget> __children = children ?? new List<Widget>();
        this.children = __children;
    }

    public override MultiChildRenderObjectElement createElement() => new MultiChildRenderObjectElement(this);
    protected MultiChildRenderObjectWidget(Key? key = null, IEnumerable<Widget> children = default!) : this(key, children.ToList()) { }
}

internal enum _ElementLifecycle__framework
{
    initial,
    active,
    inactive,
    failed,
    defunct
}

internal class _InactiveElements__framework
{
    internal virtual bool _locked { get; set; } = false;
    internal virtual HashSet<Element> _elements { get; private set; } = new HashSet<Element>();

    internal static void _unmount(Element element)
    {
        DartRuntimePrimitives.Assert(() => Equals(element._lifecycleState, _ElementLifecycle__framework.inactive));
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPrintGlobalKeyedWidgetLifecycle)
                {
                    if (element.widget.key is GlobalKeyBase)
                    {
                        PrintLibrary.debugPrint($"Discarding {element} from inactive elements list.");
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        element.visitChildren((child) =>
        {
            DartRuntimePrimitives.Assert(() => Equals(child._parent, element));
            _unmount(child);
        });
        element.unmount();
        DartRuntimePrimitives.Assert(() => Equals(element._lifecycleState, _ElementLifecycle__framework.defunct));
    }

    internal virtual void _unmountAll()
    {
        _locked = true;
        List<Element> elements = ((Func<List<Element>>)(() =>
{
    var __cascade = _elements.ToList();
    __cascade.sort(Element._sort);
    return __cascade;
}))().ToList();
        _elements.Clear();
        try
        {
            Enumerable.Reverse(elements).forEach((__arg0) => ((Action<Element>)_unmount)(__arg0));
        }
        finally
        {
            DartRuntimePrimitives.Assert(() => !Enumerable.Any(_elements));
            _locked = false;
        }
    }

    internal static void _deactivateRecursively(Element element)
    {
        DartRuntimePrimitives.Assert(() => Equals(element._lifecycleState, _ElementLifecycle__framework.active));
        try
        {
            element.deactivate();
        }
        catch
        {
            Element._deactivateFailedSubtreeRecursively(element);
            throw;
        }
        element.visitChildren(_deactivateRecursively);
        DartRuntimePrimitives.Assert(() =>
            {
                element.debugDeactivated();
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual void add(Element element)
    {
        DartRuntimePrimitives.Assert(() => !_locked);
        DartRuntimePrimitives.Assert(() => !_elements.Contains(element));
        DartRuntimePrimitives.Assert(() => element._parent is null);
        switch (element._lifecycleState)
        {
            case _ElementLifecycle__framework.active:
                {
                    _deactivateRecursively(element);
                    _elements.Add(element);
                    break;
                }
            case _ElementLifecycle__framework.inactive:
                {
                    _elements.Add(element);
                    break;
                }
            case _ElementLifecycle__framework.initial or _ElementLifecycle__framework.failed or _ElementLifecycle__framework.defunct:
                {
                    DartRuntimePrimitives.Assert(() => false, () => (object?)$"{element} must not be deactivated when in {element._lifecycleState} state.");
                    break;
                }
        }
    }

    public virtual void remove(Element element)
    {
        DartRuntimePrimitives.Assert(() => !_locked);
        DartRuntimePrimitives.Assert(() => _elements.Contains(element));
        DartRuntimePrimitives.Assert(() => element._parent is null);
        _elements.Remove(element);
        DartRuntimePrimitives.Assert(() => Equals(element._lifecycleState, _ElementLifecycle__framework.inactive));
    }

    public virtual bool debugContains(Element element)
    {
        bool result = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                result = _elements.Contains(element);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate void ElementVisitor(Element element);

public delegate bool ConditionalElementVisitor(Element element);

public interface BuildContext
{
    public Widget widget { get; }
    public BuildOwner? owner { get; }
    public bool mounted { get; }
    public bool debugDoingBuild { get; }
    public RenderObject? findRenderObject();
    public Size? size { get; }
    public InheritedWidget dependOnInheritedElement(InheritedElement ancestor, object? aspect = null);
    public T? dependOnInheritedWidgetOfExactType<T>(object? aspect = null);
    public T? getInheritedWidgetOfExactType<T>();
    public InheritedElement? getElementForInheritedWidgetOfExactType<T>();
    public T? findAncestorWidgetOfExactType<T>();
    public T? findAncestorStateOfType<T>();
    public T? findRootAncestorStateOfType<T>();
    public T? findAncestorRenderObjectOfType<T>();
    public void visitAncestorElements(Func<Element, bool> visitor);
    public void visitChildElements(Action<Element> visitor);
    public void dispatchNotification(Notification notification);
    public DiagnosticsNode describeElement(string name, DiagnosticsTreeStyle style = DiagnosticsTreeStyle.errorProperty);
    public DiagnosticsNode describeWidget(string name, DiagnosticsTreeStyle style = DiagnosticsTreeStyle.errorProperty);
    public List<DiagnosticsNode> describeMissingAncestor(Type expectedAncestorType);
    public DiagnosticsNode describeOwnershipChain(string name);
}

public class BuildScope
{
    internal virtual bool _buildScheduled { get; set; } = false;
    internal virtual bool _building { get; set; } = false;
    public virtual Action? scheduleRebuild { get; private set; }
    internal virtual bool? _dirtyElementsNeedsResorting { get; set; } = default;
    internal virtual List<Element> _dirtyElements { get; private set; } = new List<Element>();

    public BuildScope(Action? scheduleRebuild = null)
    {
        this.scheduleRebuild = scheduleRebuild;
    }

    internal virtual void _scheduleBuildFor(Element element)
    {
        FrameworkWorkCounters.Add(FrameworkWork.BuildEnqueueAttempt);
        if (FrameworkWorkTrace.Enabled) FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.Enqueue, element, this, element._inDirtyList ? 1 : 0);
        if (_building) FrameworkWorkCounters.Add(FrameworkWork.BuildDuringFlush);
        if (element._inDirtyList) FrameworkWorkCounters.Add(FrameworkWork.BuildEnqueueDuplicate);
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(element.buildScope, this));
        if (!element._inDirtyList)
        {
            _dirtyElements.Add(element);
            FrameworkWorkCounters.Add(FrameworkWork.BuildEnqueued);
            element._inDirtyList = true;
        }
        if (!_buildScheduled && !_building)
        {
            _buildScheduled = true;
            scheduleRebuild?.Invoke();
        }
        if (_dirtyElementsNeedsResorting is not null)
        {
            _dirtyElementsNeedsResorting = true;
        }
    }

    internal virtual void _tryRebuild(Element element)
    {
        DartRuntimePrimitives.Assert(() => element._inDirtyList);
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(element.buildScope, this));
        bool isTimelineTracked = !Foundation.ConstantsLibrary.kReleaseMode && FrameworkLibrary._isProfileBuildsEnabledFor(element.widget);
        if (isTimelineTracked)
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (Foundation.ConstantsLibrary.kDebugMode && DebugLibrary.debugEnhanceBuildTimelineArguments)
                    {
                        debugTimelineArguments = ((Diagnosticable)element.widget).toDiagnosticsNode().toTimelineArguments();
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            FlutterTimeline.startSync($"{DartRuntimePrimitives.RuntimeType(element.widget)}", arguments: debugTimelineArguments);
        }
        try
        {
            element.rebuild();
        }
        catch (Exception e)
        {
            var stack = new System.Diagnostics.StackTrace();
            FrameworkLibrary._reportException(new ErrorDescription("while rebuilding dirty elements"), e, stack, informationCollector: () => new List<DiagnosticsNode> { element.describeElement("The element being rebuilt at the time was") });
        }
        if (isTimelineTracked)
        {
            FlutterTimeline.finishSync();
        }
    }

    internal virtual bool _debugAssertElementInScope(Element element, Element debugBuildRoot)
    {
        bool isInScope = element._debugIsDescendantOf(debugBuildRoot) || !element.debugIsActive;
        if (isInScope)
        {
            return true;
        }
        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Tried to build dirty widget in the wrong build scope."), new ErrorDescription("A widget which was marked as dirty and is still active was scheduled to be built, " + "but the current build scope unexpectedly does not contain that widget."), new ErrorHint("Sometimes this is detected when an element is removed from the widget tree, but the " + "element somehow did not get marked as inactive. In that case, it might be caused by " + "an ancestor element failing to implement visitChildren correctly, thus preventing " + "some or all of its descendants from being correctly deactivated."), new DiagnosticsProperty<Element>("The root of the build scope was", debugBuildRoot, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<Element>("The offending element (which does not appear to be a descendant of the root of the build scope) was", element, style: DiagnosticsTreeStyle.errorProperty) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _flushDirtyElements(Element debugBuildRoot)
    {
        FrameworkWorkCounters.Add(FrameworkWork.BuildSort);
        DartRuntimePrimitives.Assert(() => _dirtyElementsNeedsResorting is null, () => (object?)"_flushDirtyElements must be non-reentrant");
        _dirtyElements.sort(Element._sort);
        _dirtyElementsNeedsResorting = false;
        try
        {
            for (var index = 0L; index < checked(_dirtyElements.Count); index = _dirtyElementIndexAfter(index))
            {
                Element elementLocal = _dirtyElements[(int)index];
                if (DartRuntimePrimitives.Identical(elementLocal.buildScope, this))
                {
                    DartRuntimePrimitives.Assert(() => _debugAssertElementInScope(elementLocal, debugBuildRoot));
                    _tryRebuild(elementLocal);
                }
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    IEnumerable<Element> missedElements = _dirtyElements.where((element) => element.debugIsActive && element.dirty && DartRuntimePrimitives.Identical(element.buildScope, this));
                    if (Enumerable.Any(missedElements))
                    {
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("buildScope missed some dirty elements."), new ErrorHint("This probably indicates that the dirty list should have been resorted but was not."), new DiagnosticsProperty<Element>("The context argument of the buildScope call was", debugBuildRoot, style: DiagnosticsTreeStyle.errorProperty), Element.describeElements("The list of missed elements at the end of the buildScope call was", missedElements.Cast<Element>()) }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        finally
        {
            foreach (Element elementAlternate in _dirtyElements)
            {
                if (DartRuntimePrimitives.Identical(elementAlternate.buildScope, this))
                {
                    elementAlternate._inDirtyList = false;
                }
            }
            _dirtyElements.Clear();
            _dirtyElementsNeedsResorting = null;
            _buildScheduled = false;
        }
    }

    internal virtual long _dirtyElementIndexAfter(long index)
    {
        if (!DartRuntimePrimitives.RequireValue(_dirtyElementsNeedsResorting))
        {
            return index + 1L;
        }
        index += 1L;
        FrameworkWorkCounters.Add(FrameworkWork.BuildResort);
        _dirtyElements.sort(Element._sort);
        _dirtyElementsNeedsResorting = false;
        while ((index > 0L) && _dirtyElements[(int)(index - 1L)].dirty)
        {
            index -= 1L;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                for (long i = index - 1L; i >= 0L; i -= 1L)
                {
                    Element element = _dirtyElements[(int)i];
                    DartRuntimePrimitives.Assert(() => !element.dirty || (!Equals(element._lifecycleState, _ElementLifecycle__framework.active)));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return index;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BuildOwner
{
    public virtual Action? onBuildScheduled { get; set; } = default;
    internal virtual _InactiveElements__framework _inactiveElements { get; private set; } = new _InactiveElements__framework();
    internal virtual bool _scheduledFlushDirtyElements { get; set; } = false;
    public virtual FocusManager focusManager { get; set; } = default!;
    internal virtual long _debugStateLockLevel { get; set; } = 0L;
    internal virtual bool _debugBuilding { get; set; } = false;
    internal virtual Element? _debugCurrentBuildTarget { get; set; } = default;
    internal virtual DartMap<Element, HashSet<GlobalKeyBase>>? _debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans { get; set; } = default;
    internal virtual DartMap<GlobalKeyBase, Element> _globalKeyRegistry { get; private set; } = new DartMap<GlobalKeyBase, Element>();
    internal virtual HashSet<Element>? _debugIllFatedElements { get; private set; } = Foundation.ConstantsLibrary.kDebugMode ? new HashSet<Element>() : null;
    internal virtual DartMap<Element, DartMap<Element, GlobalKeyBase>>? _debugGlobalKeyReservations { get; private set; } = Foundation.ConstantsLibrary.kDebugMode ? new DartMap<Element, DartMap<Element, GlobalKeyBase>>() : null;

    public BuildOwner(Action? onBuildScheduled = null, FocusManager? focusManager = null)
    {
        this.onBuildScheduled = onBuildScheduled;
        this.focusManager = focusManager ?? ((Func<FocusManager>)(() =>
{
    var __cascade = new FocusManager();
    __cascade.registerGlobalHandlers();
    return __cascade;
}))();
    }

    public virtual void scheduleBuildFor(Element element)
    {
        DartRuntimePrimitives.Assert(() => Equals(element.owner, this));
        DartRuntimePrimitives.Assert(() => element._parentBuildScope is not null);
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPrintScheduleBuildForStacks)
                {
                    AssertionsLibrary.debugPrintStack(label: $"scheduleBuildFor() called for {element}{(element.buildScope._dirtyElements.Contains(element) ? " (ALREADY IN LIST)" : "")}");
                }
                if (!element.dirty)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("scheduleBuildFor() called for a widget that is not marked as dirty."), element.describeElement("The method was called for the following element"), new ErrorDescription("This element is not current marked as dirty. Make sure to set the dirty flag before " + "calling scheduleBuildFor()."), new ErrorHint("If you did not attempt to call scheduleBuildFor() yourself, then this probably " + "indicates a bug in the widgets framework. Please report it:\n" + "  https://github.com/flutter/flutter/issues/new?template=02_bug.yml") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        BuildScope buildScopeLocal = element.buildScope;
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPrintScheduleBuildForStacks && element._inDirtyList)
                {
                    AssertionsLibrary.debugPrintStack(label: "BuildOwner.scheduleBuildFor() called; " + $"_dirtyElementsNeedsResorting was {buildScopeLocal._dirtyElementsNeedsResorting} (now true); " + $"The dirty list for the current build scope is: {buildScopeLocal._dirtyElements}");
                }
                if (!_debugBuilding && element._inDirtyList)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("BuildOwner.scheduleBuildFor() called inappropriately."), new ErrorHint("The BuildOwner.scheduleBuildFor() method called on an Element " + "that is already in the dirty list."), element.describeElement("the dirty Element was") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (!_scheduledFlushDirtyElements && (onBuildScheduled is not null))
        {
            _scheduledFlushDirtyElements = true;
            onBuildScheduled!();
        }
        buildScopeLocal._scheduleBuildFor(element);
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPrintScheduleBuildForStacks)
                {
                    PrintLibrary.debugPrint($"...the build scope's dirty list is now: {buildScopeLocal._dirtyElements}");
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual bool _debugStateLocked => DartRuntimePrimitives.ConvertValue<bool>(_debugStateLockLevel > 0L);
    public virtual bool debugBuilding => _debugBuilding;
    public virtual void lockState(Action callback)
    {
        DartRuntimePrimitives.Assert(() => _debugStateLockLevel >= 0L);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugStateLockLevel += 1L;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        try
        {
            callback();
        }
        finally
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugStateLockLevel -= 1L;
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        DartRuntimePrimitives.Assert(() => _debugStateLockLevel >= 0L);
    }

    public virtual void buildScope(Element context, Action? callback = null)
    {
        BuildScope buildScopeLocal = context.buildScope;
        if ((callback is null) && !Enumerable.Any(buildScopeLocal._dirtyElements))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => _debugStateLockLevel >= 0L);
        DartRuntimePrimitives.Assert(() => !_debugBuilding);
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPrintBuildScope)
                {
                    PrintLibrary.debugPrint($"buildScope called with context {context}; " + $"its build scope's dirty list is: {buildScopeLocal._dirtyElements}");
                }
                _debugStateLockLevel += 1L;
                _debugBuilding = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (DebugLibrary.debugEnhanceBuildTimelineArguments)
                    {
                        debugTimelineArguments = new DartMap<string, string> { ["build scope dirty count"] = $"{checked((long)buildScopeLocal._dirtyElements.Count)}", ["build scope dirty list"] = $"{buildScopeLocal._dirtyElements}", ["lock level"] = $"{_debugStateLockLevel}", ["scope context"] = $"{context}" }.cast<string, string>();
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            FlutterTimeline.startSync("BUILD", arguments: debugTimelineArguments);
        }
        try
        {
            _scheduledFlushDirtyElements = true;
            buildScopeLocal._building = true;
            if (callback is not null)
            {
                DartRuntimePrimitives.Assert(() => _debugStateLocked);
                Element? debugPreviousBuildTarget = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        debugPreviousBuildTarget = _debugCurrentBuildTarget;
                        _debugCurrentBuildTarget = context;
                        return true;
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                try
                {
                    callback();
                }
                finally
                {
                    DartRuntimePrimitives.Assert(() =>
                        {
                            DartRuntimePrimitives.Assert(() => Equals(_debugCurrentBuildTarget, context));
                            _debugCurrentBuildTarget = debugPreviousBuildTarget;
                            _debugElementWasRebuilt(context);
                            return true;
                            throw new InvalidOperationException("Dart closure completed without a value.");
                        });
                }
            }
            buildScopeLocal._flushDirtyElements(debugBuildRoot: context);
        }
        finally
        {
            buildScopeLocal._building = false;
            _scheduledFlushDirtyElements = false;
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
            DartRuntimePrimitives.Assert(() => _debugBuilding);
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugBuilding = false;
                    _debugStateLockLevel -= 1L;
                    if (DebugLibrary.debugPrintBuildScope)
                    {
                        PrintLibrary.debugPrint("buildScope finished");
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        DartRuntimePrimitives.Assert(() => _debugStateLockLevel >= 0L);
    }

    internal virtual void _debugTrackElementThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans(Element node, GlobalKeyBase key)
    {
        DartMap<Element, HashSet<GlobalKeyBase>> map = _debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans ??= new DartMap<Element, HashSet<GlobalKeyBase>>();
        HashSet<GlobalKeyBase> keys = map.putIfAbsent(node, () => new HashSet<GlobalKeyBase>());
        keys.Add(key);
    }

    internal virtual void _debugElementWasRebuilt(Element node)
    {
        _debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans?.remove(node);
    }

    public virtual long globalKeyCount => checked(_globalKeyRegistry.Count);
    internal virtual void _debugRemoveGlobalKeyReservationFor(Element parent, Element child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugGlobalKeyReservations?.GetValueOrDefault(parent)?.remove(child);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual void _registerGlobalKey(GlobalKeyBase key, Element element)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_globalKeyRegistry.ContainsKey(key))
                {
                    Element oldElement = _globalKeyRegistry.GetValueOrDefault(key)!;
                    DartRuntimePrimitives.Assert(() => !Equals(DartRuntimePrimitives.RuntimeType(element.widget), DartRuntimePrimitives.RuntimeType(oldElement.widget)));
                    _debugIllFatedElements?.Add(oldElement);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _globalKeyRegistry[key] = element;
    }

    internal virtual void _unregisterGlobalKey(GlobalKeyBase key, Element element)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_globalKeyRegistry.ContainsKey(key) && (!Equals(_globalKeyRegistry.GetValueOrDefault(key), element)))
                {
                    Element oldElement = _globalKeyRegistry.GetValueOrDefault(key)!;
                    DartRuntimePrimitives.Assert(() => !Equals(DartRuntimePrimitives.RuntimeType(element.widget), DartRuntimePrimitives.RuntimeType(oldElement.widget)));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (Equals(_globalKeyRegistry.GetValueOrDefault(key), element))
        {
            _globalKeyRegistry.remove(key);
        }
    }

    internal virtual void _debugReserveGlobalKeyFor(Element parent, Element child, GlobalKeyBase key)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_debugGlobalKeyReservations is { } reservations)
                    reservations.putIfAbsent(parent, () => new DartMap<Element, GlobalKeyBase>())[child] = key;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual void _debugVerifyGlobalKeyReservation()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var keyToParent = new DartMap<GlobalKeyBase, Element>();
                _debugGlobalKeyReservations?.forEach((parent, childToKey) =>
                {
                    if (Equals(parent._lifecycleState, _ElementLifecycle__framework.defunct) || (parent.renderObject?.attached == false))
                    {
                        return;
                    }
                    childToKey.forEach((child, key) =>
                    {
                        if (child._parent is null)
                        {
                            return;
                        }
                        if (keyToParent.ContainsKey(key) && (!Equals(keyToParent.GetValueOrDefault(key), parent)))
                        {
                            Element older = keyToParent.GetValueOrDefault(key)!;
                            var newer = parent;
                            FlutterError error = default!;
                            if (older.ToString() != newer.ToString())
                            {
                                error = new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Multiple widgets used the same GlobalKey."), new ErrorDescription($"The key {key} was used by multiple widgets. The parents of those widgets were:\n" + $"- {older}\n" + $"- {newer}\n" + "A GlobalKey can only be specified on one widget at a time in the widget tree.") });
                            }
                            else
                            {
                                error = new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Multiple widgets used the same GlobalKey."), new ErrorDescription($"The key {key} was used by multiple widgets. The parents of those widgets were " + "different widgets that both had the following description:\n" + $"  {parent}\n" + "A GlobalKey can only be specified on one widget at a time in the widget tree.") });
                            }
                            if (!Equals(child._parent, older))
                            {
                                older.visitChildren((currentChild) =>
                                {
                                    if (Equals(currentChild, child))
                                    {
                                        older.forgetChild(child);
                                    }
                                });
                            }
                            if (!Equals(child._parent, newer))
                            {
                                newer.visitChildren((currentChild) =>
                                {
                                    if (Equals(currentChild, child))
                                    {
                                        newer.forgetChild(child);
                                    }
                                });
                            }
                            throw DartRuntimePrimitives.AsException(error);
                        }
                        else
                        {
                            keyToParent[key] = parent;
                        }
                    });
                });
                _debugGlobalKeyReservations?.Clear();
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual void _debugVerifyIllFatedPopulation()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                DartMap<GlobalKeyBase, HashSet<Element>>? duplicates = default!;
                foreach (Element element in _debugIllFatedElements ?? new HashSet<Element>())
                {
                    if (!Equals(element._lifecycleState, _ElementLifecycle__framework.defunct))
                    {
                        DartRuntimePrimitives.Assert(() => element.widget.key is not null);
                        var keyLocal = ((GlobalKeyBase?)element.widget.key!)!;
                        DartRuntimePrimitives.Assert(() => _globalKeyRegistry.ContainsKey(keyLocal));
                        duplicates ??= new DartMap<GlobalKeyBase, HashSet<Element>>();
                        HashSet<Element> elements = duplicates.putIfAbsent(keyLocal, () => new HashSet<Element>());
                        elements.Add(element);
                        elements.Add(_globalKeyRegistry.GetValueOrDefault(keyLocal)!);
                    }
                }
                _debugIllFatedElements?.Clear();
                if (duplicates is not null)
                {
                    var information = new List<DiagnosticsNode>();
                    information.Add(new ErrorSummary("Multiple widgets used the same GlobalKey."));
                    foreach (GlobalKeyBase keyAlternate in duplicates.Keys)
                    {
                        HashSet<Element> elementsLocal = duplicates.GetValueOrDefault(keyAlternate)!;
                        information.Add(Element.describeElements($"The key {keyAlternate} was used by {checked((long)elementsLocal.Count)} widgets", elementsLocal));
                    }
                    information.Add(new ErrorDescription("A GlobalKey can only be specified on one widget at a time in the widget tree."));
                    throw DartRuntimePrimitives.AsException(new FlutterError(information));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual void finalizeTree()
    {
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync("FINALIZE TREE");
        }
        try
        {
            lockState(() => _inactiveElements._unmountAll());
            DartRuntimePrimitives.Assert(() =>
                {
                    try
                    {
                        _debugVerifyGlobalKeyReservation();
                        _debugVerifyIllFatedPopulation();
                        if ((_debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans is { } __items144242 ? Enumerable.Any(__items144242) : (bool?)null) ?? false)
                        {
                            HashSet<GlobalKeyBase> keys = new HashSet<GlobalKeyBase>();
                            foreach (Element elementLocal in _debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans!.Keys)
                            {
                                if (!Equals(elementLocal._lifecycleState, _ElementLifecycle__framework.defunct))
                                {
                                    keys.UnionWith(_debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans!.GetValueOrDefault(elementLocal)!);
                                }
                            }
                            if (Enumerable.Any(keys))
                            {
                                DartMap<string?, long> keyStringCount = new DartMap<string?, long>();
                                foreach (string? keyLocal in keys.map<GlobalKeyBase, string?>((key) => key.ToString()))
                                {
                                    if (keyStringCount.ContainsKey(keyLocal))
                                    {
                                        keyStringCount.update(keyLocal, (value) => value + 1L);
                                    }
                                    else
                                    {
                                        keyStringCount[keyLocal] = 1L;
                                    }
                                }
                                var keyLabels = new List<string>();
                                IEnumerable<Element> elements = _debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans!.Keys;
                                DartMap<string, long> elementStringCount = new DartMap<string, long>();
                                foreach (string elementAlternate in elements.map((element) => element.ToString()))
                                {
                                    if (elementStringCount.ContainsKey(elementAlternate))
                                    {
                                        elementStringCount.update(elementAlternate, (value) => value + 1L);
                                    }
                                    else
                                    {
                                        elementStringCount[elementAlternate] = 1L;
                                    }
                                }
                                var elementLabels = new List<string>();
                                DartRuntimePrimitives.Assert(() => Enumerable.Any(keyLabels));
                                var the = (checked(keys.Count) == 1L) ? " the" : "";
                                var s = (checked(keys.Count) == 1L) ? "" : "s";
                                var were = (checked(keys.Count) == 1L) ? "was" : "were";
                                var their = (checked(keys.Count) == 1L) ? "its" : "their";
                                var respective = (checked(elementLabels.Count) == 1L) ? "" : " respective";
                                var those = (checked(keys.Count) == 1L) ? "that" : "those";
                                var s2 = (checked(elementLabels.Count) == 1L) ? "" : "s";
                                var those2 = (checked(elementLabels.Count) == 1L) ? "that" : "those";
                                var they = (checked(elementLabels.Count) == 1L) ? "it" : "they";
                                var think = (checked(elementLabels.Count) == 1L) ? "thinks" : "think";
                                var are = (checked(elementLabels.Count) == 1L) ? "is" : "are";
                                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"Duplicate GlobalKey{s} detected in widget tree."), new ErrorDescription($"The following GlobalKey{s} {were} specified multiple times in the widget tree. This will lead to " + "parts of the widget tree being truncated unexpectedly, because the second time a key is seen, " + $"the previous instance is moved to the new location. The key{s} {were}:\n" + $"- {string.Join("\n  ", keyLabels)}\n" + $"This was determined by noticing that after{the} widget{s} with the above global key{s} {were} moved " + $"out of {their}{respective} previous parent{s2}, {those2} previous parent{s2} never updated during this frame, meaning " + $"that {they} either did not update at all or updated before the widget{s} {were} moved, in either case " + $"implying that {they} still {think} that {they} should have a child with {those} global key{s}.\n" + $"The specific parent{s2} that did not update after having one or more children forcibly removed " + $"due to GlobalKey reparenting {are}:\n" + $"- {string.Join("\n  ", elementLabels)}" + "\nA GlobalKey can only be specified on one widget at a time in the widget tree.") }));
                            }
                        }
                    }
                    finally
                    {
                        _debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans?.Clear();
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        catch (Exception e)
        {
            var stack = new System.Diagnostics.StackTrace();
            FrameworkLibrary._reportException(new ErrorSummary("while finalizing the widget tree"), e, stack);
        }
    }

    public virtual void reassemble(Element root)
    {
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync("Preparing Hot Reload (widgets)");
        }
        try
        {
            DartRuntimePrimitives.Assert(() => root._parent is null);
            DartRuntimePrimitives.Assert(() => Equals(root.owner, this));
            root.reassemble();
        }
        finally
        {
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
    }

}

public interface NotifiableElementMixin
{
    public bool onNotification(Notification notification);
    public void attachNotificationTree();
}

internal class _NotificationNode__framework
{
    public virtual NotifiableElementMixin? current { get; set; } = default;
    public virtual _NotificationNode__framework? parent { get; set; } = default;

    internal _NotificationNode__framework(_NotificationNode__framework? parent, NotifiableElementMixin? current)
    {
        this.parent = parent;
        this.current = current;
    }

    public virtual void dispatchNotification(Notification notification)
    {
        if (current?.onNotification(notification) ?? true)
        {
            return;
        }
        parent?.dispatchNotification(notification);
    }

}

public static partial class FrameworkLibrary
{
    internal static bool _isProfileBuildsEnabledFor(Widget widget)
    {
        return DebugLibrary.debugProfileBuildsEnabled || DebugLibrary.debugProfileBuildsEnabledUserWidgets && Widget_inspectorLibrary.debugIsWidgetLocalCreation(widget);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class Element : DiagnosticableTree, BuildContext
{
    internal virtual Element? _parent { get; set; } = default;
    internal virtual _NotificationNode__framework? _notificationTree { get; set; } = default;
    internal virtual object? _slot { get; set; } = default;
    internal virtual long _depth { get; set; } = default!;
    internal virtual Widget? _widget { get; set; } = default;
    internal virtual BuildOwner? _owner { get; set; } = default;
    internal virtual BuildScope? _parentBuildScope { get; set; } = default;
    internal virtual _ElementLifecycle__framework _lifecycleState { get; set; } = _ElementLifecycle__framework.initial;
    internal virtual HashSet<Element>? _debugForgottenChildrenWithGlobalKey { get; private set; } = Foundation.ConstantsLibrary.kDebugMode ? new HashSet<Element>() : null;
    internal virtual PersistentHashMap<Type, InheritedElement>? _inheritedElements { get; set; } = default;
    internal virtual HashSet<InheritedElement>? _dependencies { get; set; } = default;
    internal virtual bool _hadUnsatisfiedDependencies { get; set; } = false;
    internal virtual bool _dirty { get; set; } = true;
    internal virtual bool _inDirtyList { get; set; } = false;
    internal virtual bool _debugBuiltOnce { get; set; } = false;

    protected Element(Widget widget)
    {
        _widget = widget;
    }

    public virtual string toStringShallow(string joiner = ", ", DiagnosticLevel minLevel = DiagnosticLevel.debug) => throw new NotSupportedException();
    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, long wrapWidth = 65) => throw new NotSupportedException();
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info) =>
        $"{GetType().Name}({_widget?.GetType().Name ?? "unmounted"})";
    public virtual bool debugDoingBuild => throw new NotSupportedException();
    public override bool Equals(object? other)
    {
        var __other = other as Element;
        if (__other is null) return false;
        return DartRuntimePrimitives.Identical(this, __other);
    }

    public override int GetHashCode() => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);

    public virtual object? slot => _slot;
    public virtual long depth
    {
        get
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (Equals(_lifecycleState, _ElementLifecycle__framework.initial))
                    {
                        throw DartRuntimePrimitives.AsException(FlutterError.Create("Depth is only available when element has been mounted."));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return _depth;
        }
    }
    internal static long _sort(Element a, Element b)
    {
        long diff = a.depth - b.depth;
        if (diff != 0L)
        {
            return diff;
        }
        bool isBDirty = b.dirty;
        if (a.dirty != isBDirty)
        {
            return isBDirty ? -1L : 1L;
        }
        return 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _debugConcreteSubtype(Element element)
    {
        return (element is StatefulElement) ? 1L : ((element is StatelessElement) ? 2L : 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget widget => DartRuntimePrimitives.ConvertValue<Widget>(_widget!);
    public virtual bool mounted => DartRuntimePrimitives.ConvertValue<bool>(_widget is not null);
    public virtual bool debugIsDefunct
    {
        get
        {
            var isDefunct = false;
            DartRuntimePrimitives.Assert(() =>
                {
                    isDefunct = Equals(_lifecycleState, _ElementLifecycle__framework.defunct);
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return isDefunct;
        }
    }
    public virtual bool debugIsActive
    {
        get
        {
            var isActive = false;
            DartRuntimePrimitives.Assert(() =>
                {
                    isActive = Equals(_lifecycleState, _ElementLifecycle__framework.active);
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return isActive;
        }
    }
    public virtual BuildOwner? owner => _owner;
    public virtual BuildScope buildScope => DartRuntimePrimitives.ConvertValue<BuildScope>(_parentBuildScope!);
    public virtual void reassemble()
    {
        markNeedsBuild();
        visitChildren((child) =>
        {
            child.reassemble();
        });
    }

    internal virtual bool _debugIsDescendantOf(Element target)
    {
        Element? element = this;
        while ((element is not null) && (element.depth > target.depth))
        {
            element = element._parent;
        }
        return Equals(element, target);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderObject? renderObject
    {
        get
        {
            Element? current = this;
            while (current is not null)
            {
                if (Equals(current._lifecycleState, _ElementLifecycle__framework.defunct))
                {
                    break;
                }
                else
                {
                    if (current is RenderObjectElement)
                    {
                        RenderObjectElement current__163649__as163793 = (RenderObjectElement)current;
                        return current__163649__as163793.renderObject;
                    }
                    else
                    {
                        current = current.renderObjectAttachingChild;
                    }
                }
            }
            return null;
        }
    }
    public virtual Element? renderObjectAttachingChild
    {
        get
        {
            Element? next = default!;
            visitChildren((child) =>
            {
                DartRuntimePrimitives.Assert(() => next is null);
                next = child;
            });
            return next;
        }
    }
    public virtual List<DiagnosticsNode> describeMissingAncestor(Type expectedAncestorType)
    {
        var information = new List<DiagnosticsNode>();
        var ancestors = new List<Element>();
        visitAncestorElements((element) =>
        {
            ancestors.Add(element);
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        information.Add(new DiagnosticsProperty<Element>($"The specific widget that could not find a {expectedAncestorType} ancestor was", this, style: DiagnosticsTreeStyle.errorProperty));
        if (Enumerable.Any(ancestors))
        {
            information.Add(describeElements("The ancestors of this widget were", ancestors.Cast<Element>()));
        }
        else
        {
            information.Add(new ErrorDescription("This widget is the root of the tree, so it has no " + $"ancestors, let alone a \"{expectedAncestorType}\" ancestor."));
        }
        return information;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DiagnosticsNode describeElements(string name, IEnumerable<Element> elements)
    {
        return new DiagnosticsBlock(name: name, children: elements.map<Element, DiagnosticsNode>((element) => new DiagnosticsProperty<Element>("", element)).ToList(), allowTruncate: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode describeElement(string name, DiagnosticsTreeStyle style = DiagnosticsTreeStyle.errorProperty)
    {
        return new DiagnosticsProperty<Element>(name, this, style: DartRuntimePrimitives.RequireValue(style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode describeWidget(string name, DiagnosticsTreeStyle style = DiagnosticsTreeStyle.errorProperty)
    {
        return new DiagnosticsProperty<Element>(name, this, style: DartRuntimePrimitives.RequireValue(style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode describeOwnershipChain(string name)
    {
        return new StringProperty(name, debugGetCreatorChain(10L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void visitChildren(Action<Element> visitor)
    {
    }

    public virtual void debugVisitOnstageChildren(Action<Element> visitor) => visitChildren(visitor);
    public virtual void visitChildElements(Action<Element> visitor)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((owner is null) || !owner!._debugStateLocked)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("visitChildElements() called during build."), new ErrorDescription("The BuildContext.visitChildElements() method can't be called during " + "build because the child list is still being updated at that point, " + "so the children might not be constructed yet, or might be old children " + "that are going to be replaced.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        visitChildren(visitor);
    }

    public virtual Element? updateChild(Element? child, Widget? newWidget, object? newSlot)
    {
        if (FrameworkWorkTrace.Enabled) FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.UpdateChild, this, child, newWidget is null ? 0 : 1);
        if (newWidget is null)
        {
            if (child is not null)
            {
                deactivateChild(child);
            }
            return null;
        }
        Element newChild = default!;
        if (child is not null)
        {
            var hasSameSuperclass = true;
            DartRuntimePrimitives.Assert(() =>
                {
                    long oldElementClass = _debugConcreteSubtype(child);
                    long newWidgetClass = Widget._debugConcreteSubtype(newWidget);
                    hasSameSuperclass = oldElementClass == newWidgetClass;
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            if (hasSameSuperclass && Equals(child.widget, newWidget))
            {
                if (!Equals(child.slot, newSlot))
                {
                    updateSlotForChild(child, newSlot);
                }
                newChild = child;
            }
            else
            {
                if (hasSameSuperclass && Widget.canUpdate(child.widget, newWidget))
                {
                    if (!Equals(child.slot, newSlot))
                    {
                        updateSlotForChild(child, newSlot);
                    }
                    bool isTimelineTracked = !Foundation.ConstantsLibrary.kReleaseMode && FrameworkLibrary._isProfileBuildsEnabledFor(newWidget);
                    if (isTimelineTracked)
                    {
                        DartMap<string, string>? debugTimelineArguments = default!;
                        DartRuntimePrimitives.Assert(() =>
                            {
                                if (Foundation.ConstantsLibrary.kDebugMode && DebugLibrary.debugEnhanceBuildTimelineArguments)
                                {
                                    debugTimelineArguments = ((Diagnosticable)newWidget).toDiagnosticsNode().toTimelineArguments();
                                }
                                return true;
                                throw new InvalidOperationException("Dart closure completed without a value.");
                            });
                        FlutterTimeline.startSync($"{DartRuntimePrimitives.RuntimeType(newWidget)}", arguments: debugTimelineArguments);
                    }
                    child.update(newWidget);
                    if (isTimelineTracked)
                    {
                        FlutterTimeline.finishSync();
                    }
                    DartRuntimePrimitives.Assert(() => Equals(child.widget, newWidget));
                    DartRuntimePrimitives.Assert(() =>
                        {
                            child.owner!._debugElementWasRebuilt(child);
                            return true;
                            throw new InvalidOperationException("Dart closure completed without a value.");
                        });
                    newChild = child;
                }
                else
                {
                    deactivateChild(child);
                    DartRuntimePrimitives.Assert(() => child._parent is null);
                    newChild = inflateWidget(newWidget, newSlot);
                }
            }
        }
        else
        {
            newChild = inflateWidget(newWidget, newSlot);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not null)
                {
                    _debugRemoveGlobalKeyReservation(child);
                }
                Key? keyLocal = newWidget.key;
                if (keyLocal is GlobalKeyBase)
                {
                    GlobalKeyBase key__175416__as175447 = (GlobalKeyBase)keyLocal;
                    DartRuntimePrimitives.Assert(() => owner is not null);
                    owner!._debugReserveGlobalKeyFor(this, newChild, key__175416__as175447);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return newChild;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<Element> updateChildren(List<Element> oldChildren, List<Widget> newWidgets, HashSet<Element>? forgottenChildren = null, List<object>? slots = null)
    {
        DartRuntimePrimitives.Assert(() => (slots is null) || (checked(newWidgets.Count) == checked((long)slots.Count)));
        Element? replaceWithNullIfForgotten(Element child)
        {
            return (forgottenChildren?.Contains(child) ?? false) ? null : child;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        object? slotFor(long newChildIndex, Element? previousChild)
        {
            return (slots is not null) ? slots[(int)newChildIndex] : new IndexedSlot<Element?>(newChildIndex, previousChild);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var newChildrenTop = 0L;
        var oldChildrenTop = 0L;
        long newChildrenBottom = checked(newWidgets.Count) - 1L;
        long oldChildrenBottom = checked(oldChildren.Count) - 1L;
        var newChildren = new List<Element>(Enumerable.Repeat<Element>(_NullElement__framework.instance, checked((int)checked((long)newWidgets.Count))));
        Element? previousChildLocal = default!;
        while (oldChildrenTop <= oldChildrenBottom && newChildrenTop <= newChildrenBottom)
        {
            Element? oldChild = replaceWithNullIfForgotten(oldChildren[(int)oldChildrenTop]);
            Widget newWidget = newWidgets[(int)newChildrenTop];
            DartRuntimePrimitives.Assert(() => (oldChild is null) || Equals(oldChild._lifecycleState, _ElementLifecycle__framework.active));
            if ((oldChild is null) || !Widget.canUpdate(oldChild.widget, newWidget))
            {
                break;
            }
            Element newChild = updateChild(oldChild, newWidget, slotFor(newChildrenTop, previousChildLocal))!;
            DartRuntimePrimitives.Assert(() => Equals(newChild._lifecycleState, _ElementLifecycle__framework.active));
            newChildren[(int)newChildrenTop] = newChild;
            previousChildLocal = newChild;
            newChildrenTop += 1L;
            oldChildrenTop += 1L;
        }
        while (oldChildrenTop <= oldChildrenBottom && newChildrenTop <= newChildrenBottom)
        {
            Element? oldChildLocal = replaceWithNullIfForgotten(oldChildren[(int)oldChildrenBottom]);
            Widget newWidgetLocal = newWidgets[(int)newChildrenBottom];
            DartRuntimePrimitives.Assert(() => (oldChildLocal is null) || Equals(oldChildLocal._lifecycleState, _ElementLifecycle__framework.active));
            if ((oldChildLocal is null) || !Widget.canUpdate(oldChildLocal.widget, newWidgetLocal))
            {
                break;
            }
            oldChildrenBottom -= 1L;
            newChildrenBottom -= 1L;
        }
        bool haveOldChildren = oldChildrenTop <= oldChildrenBottom;
        DartMap<Key, Element>? oldKeyedChildren = default!;
        if (haveOldChildren)
        {
            oldKeyedChildren = new DartMap<Key, Element>();
            while (oldChildrenTop <= oldChildrenBottom)
            {
                Element? oldChildAlternate = replaceWithNullIfForgotten(oldChildren[(int)oldChildrenTop]);
                DartRuntimePrimitives.Assert(() => (oldChildAlternate is null) || Equals(oldChildAlternate._lifecycleState, _ElementLifecycle__framework.active));
                if (oldChildAlternate is not null)
                {
                    if (oldChildAlternate.widget.key is not null)
                    {
                        oldKeyedChildren[oldChildAlternate.widget.key!] = oldChildAlternate;
                    }
                    else
                    {
                        deactivateChild(oldChildAlternate);
                    }
                }
                oldChildrenTop += 1L;
            }
        }
        while (newChildrenTop <= newChildrenBottom)
        {
            Element? oldChildNested = default!;
            Widget newWidgetAlternate = newWidgets[(int)newChildrenTop];
            if (haveOldChildren)
            {
                Key? keyLocal = newWidgetAlternate.key;
                if (keyLocal is not null)
                {
                    oldChildNested = oldKeyedChildren!.GetValueOrDefault(keyLocal);
                    if (oldChildNested is not null)
                    {
                        if (Widget.canUpdate(oldChildNested.widget, newWidgetAlternate))
                        {
                            oldKeyedChildren.remove(keyLocal);
                        }
                        else
                        {
                            oldChildNested = null;
                        }
                    }
                }
            }
            DartRuntimePrimitives.Assert(() => (oldChildNested is null) || Widget.canUpdate(oldChildNested.widget, newWidgetAlternate));
            Element newChildLocal = updateChild(oldChildNested, newWidgetAlternate, slotFor(newChildrenTop, previousChildLocal))!;
            DartRuntimePrimitives.Assert(() => Equals(newChildLocal._lifecycleState, _ElementLifecycle__framework.active));
            DartRuntimePrimitives.Assert(() => Equals(oldChildNested, newChildLocal) || (oldChildNested is null) || (!Equals(oldChildNested._lifecycleState, _ElementLifecycle__framework.active)));
            newChildren[(int)newChildrenTop] = newChildLocal;
            previousChildLocal = newChildLocal;
            newChildrenTop += 1L;
        }
        DartRuntimePrimitives.Assert(() => oldChildrenTop == (oldChildrenBottom + 1L));
        DartRuntimePrimitives.Assert(() => newChildrenTop == (newChildrenBottom + 1L));
        DartRuntimePrimitives.Assert(() => (checked(newWidgets.Count) - newChildrenTop) == (checked(oldChildren.Count) - oldChildrenTop));
        newChildrenBottom = checked(newWidgets.Count) - 1L;
        oldChildrenBottom = checked(oldChildren.Count) - 1L;
        while (oldChildrenTop <= oldChildrenBottom && newChildrenTop <= newChildrenBottom)
        {
            Element oldChildCurrent = oldChildren[(int)oldChildrenTop];
            DartRuntimePrimitives.Assert(() => replaceWithNullIfForgotten(oldChildCurrent) is not null);
            DartRuntimePrimitives.Assert(() => Equals(oldChildCurrent._lifecycleState, _ElementLifecycle__framework.active));
            Widget newWidgetNested = newWidgets[(int)newChildrenTop];
            DartRuntimePrimitives.Assert(() => Widget.canUpdate(oldChildCurrent.widget, newWidgetNested));
            Element newChildAlternate = updateChild(oldChildCurrent, newWidgetNested, slotFor(newChildrenTop, previousChildLocal))!;
            DartRuntimePrimitives.Assert(() => Equals(newChildAlternate._lifecycleState, _ElementLifecycle__framework.active));
            DartRuntimePrimitives.Assert(() => Equals(oldChildCurrent, newChildAlternate) || (!Equals(oldChildCurrent._lifecycleState, _ElementLifecycle__framework.active)));
            newChildren[(int)newChildrenTop] = newChildAlternate;
            previousChildLocal = newChildAlternate;
            newChildrenTop += 1L;
            oldChildrenTop += 1L;
        }
        if (haveOldChildren && Enumerable.Any(oldKeyedChildren!))
        {
            foreach (Element oldChildNext in oldKeyedChildren.Values)
            {
                if ((forgottenChildren is null) || !forgottenChildren.Contains(oldChildNext))
                {
                    deactivateChild(oldChildNext);
                }
            }
        }
        DartRuntimePrimitives.Assert(() => newChildren.All((element) => element is not _NullElement__framework));
        return newChildren;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void mount(Element? parent, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.initial), () => (object?)$"This element is no longer in its initial state ({_lifecycleState.ToString()})");
        DartRuntimePrimitives.Assert(() => _parent is null, () => (object?)$"This element already has a parent ({_parent}) and it shouldn't have one yet.");
        DartRuntimePrimitives.Assert(() => (parent is null) || Equals(parent._lifecycleState, _ElementLifecycle__framework.active), () => (object?)$"Parent ({parent}) should be null or in the active state ({parent?._lifecycleState.ToString()})");
        DartRuntimePrimitives.Assert(() => slot is null, () => (object?)$"This element already has a slot ({slot}) and it shouldn't");
        _parent = parent;
        _slot = newSlot;
        _lifecycleState = _ElementLifecycle__framework.active;
        _depth = 1L + (_parent?.depth ?? 0L);
        if (parent is not null)
        {
            _owner = parent.owner;
            _parentBuildScope = parent.buildScope;
        }
        DartRuntimePrimitives.Assert(() => owner is not null);
        Key? keyLocal = widget.key;
        if (keyLocal is GlobalKeyBase)
        {
            GlobalKeyBase key__188214__as188240 = (GlobalKeyBase)keyLocal;
            owner!._registerGlobalKey(key__188214__as188240, this);
        }
        _updateInheritance();
        attachNotificationTree();
    }

    internal virtual void _debugRemoveGlobalKeyReservation(Element child)
    {
        DartRuntimePrimitives.Assert(() => owner is not null);
        owner!._debugRemoveGlobalKeyReservationFor(this, child);
    }

    public virtual void update(Widget newWidget)
    {
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.active) && (!Equals(newWidget, widget)) && Widget.canUpdate(widget, newWidget));
        DartRuntimePrimitives.Assert(() =>
            {
                _debugForgottenChildrenWithGlobalKey?.forEach((__arg0) => ((Action<Element>)_debugRemoveGlobalKeyReservation)(__arg0));
                _debugForgottenChildrenWithGlobalKey?.Clear();
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _widget = newWidget;
    }

    public virtual void updateSlotForChild(Element child, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.active));
        DartRuntimePrimitives.Assert(() => Equals(child._parent, this));
        void visit(Element element)
        {
            element.updateSlot(newSlot);
            Element? descendant = element.renderObjectAttachingChild;
            if (descendant is not null)
            {
                visit(descendant);
            }
        }
        visit(child);
    }

    public virtual void updateSlot(object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.active));
        DartRuntimePrimitives.Assert(() => _parent is not null);
        DartRuntimePrimitives.Assert(() => Equals(_parent!._lifecycleState, _ElementLifecycle__framework.active));
        _slot = newSlot;
    }

    internal virtual void _updateDepth(long parentDepth)
    {
        long expectedDepth = parentDepth + 1L;
        if (_depth < expectedDepth)
        {
            _depth = expectedDepth;
            visitChildren((child) =>
            {
                child._updateDepth(expectedDepth);
            });
        }
    }

    internal virtual void _updateBuildScopeRecursively()
    {
        if (DartRuntimePrimitives.Identical(buildScope, _parent?.buildScope))
        {
            return;
        }
        _inDirtyList = false;
        _parentBuildScope = _parent?.buildScope;
        visitChildren((child) =>
        {
            child._updateBuildScopeRecursively();
        });
    }

    public virtual void detachRenderObject()
    {
        visitChildren((child) =>
        {
            child.detachRenderObject();
        });
        _slot = null;
    }

    public virtual void attachRenderObject(object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => slot is null);
        visitChildren((child) =>
        {
            child.attachRenderObject(newSlot);
        });
        _slot = newSlot;
    }

    internal virtual Element? _retakeInactiveElement(GlobalKeyBase key, Widget newWidget)
    {
        Element? element = key._currentElement;
        if (element is null)
        {
            return null;
        }
        if (!Widget.canUpdate(element.widget, newWidget))
        {
            return null;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPrintGlobalKeyedWidgetLifecycle)
                {
                    PrintLibrary.debugPrint($"Attempting to take {element} from {(object?)element._parent ?? (object?)"inactive elements list"} to put in {this}.");
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        Element? parent = element._parent;
        if (parent is not null)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (Equals(parent, this))
                    {
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A GlobalKey was used multiple times inside one widget's child list."), new DiagnosticsProperty<GlobalKeyBase>("The offending GlobalKey was", key), parent.describeElement("The parent of the widgets with that key was"), element.describeElement("The first child to get instantiated with that key became"), new DiagnosticsProperty<Widget>("The second child that was to get instantiated with that key was", widget, style: DiagnosticsTreeStyle.errorProperty), new ErrorDescription("A GlobalKey can only be specified on one widget at a time in the widget tree.") }));
                    }
                    parent.owner!._debugTrackElementThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans(parent, key);
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            parent.forgetChild(element);
            parent.deactivateChild(element);
        }
        DartRuntimePrimitives.Assert(() => element._parent is null);
        owner!._inactiveElements.remove(element);
        return element;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Element inflateWidget(Widget newWidget, object? newSlot)
    {
        bool isTimelineTracked = !Foundation.ConstantsLibrary.kReleaseMode && FrameworkLibrary._isProfileBuildsEnabledFor(newWidget);
        if (isTimelineTracked)
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (Foundation.ConstantsLibrary.kDebugMode && DebugLibrary.debugEnhanceBuildTimelineArguments)
                    {
                        debugTimelineArguments = ((Diagnosticable)newWidget).toDiagnosticsNode().toTimelineArguments();
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            FlutterTimeline.startSync($"{DartRuntimePrimitives.RuntimeType(newWidget)}", arguments: debugTimelineArguments);
        }
        try
        {
            Key? keyLocal = newWidget.key;
            Element? inactiveChild = (keyLocal is GlobalKeyBase globalKey) ? _retakeInactiveElement(globalKey, newWidget) : null;
            Element newChild = inactiveChild ?? newWidget.createElement();
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugCheckForCycles(newChild);
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            try
            {
                if (inactiveChild is not null)
                {
                    DartRuntimePrimitives.Assert(() => inactiveChild._parent is null);
                    inactiveChild._activateWithParent(this, newSlot);
                    Element? updatedChild = updateChild(inactiveChild, newWidget, newSlot);
                    DartRuntimePrimitives.Assert(() => Equals(inactiveChild, updatedChild));
                    return updatedChild!;
                }
                else
                {
                    newChild.mount(this, newSlot);
                    DartRuntimePrimitives.Assert(() => Equals(newChild._lifecycleState, _ElementLifecycle__framework.active));
                    return newChild;
                }
            }
            catch
            {
                _deactivateFailedChildSilently(newChild);
                throw;
            }
        }
        finally
        {
            if (isTimelineTracked)
            {
                FlutterTimeline.finishSync();
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _debugCheckForCycles(Element newChild)
    {
        DartRuntimePrimitives.Assert(() => newChild._parent is null);
        DartRuntimePrimitives.Assert(() =>
            {
                var node = this;
                while (node._parent is not null)
                {
                    node = node._parent!;
                }
                DartRuntimePrimitives.Assert(() => !Equals(node, newChild));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual void deactivateChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child._parent, this));
        child._parent = null;
        child.detachRenderObject();
        owner!._inactiveElements.add(child);
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPrintGlobalKeyedWidgetLifecycle)
                {
                    if (child.widget.key is GlobalKeyBase)
                    {
                        PrintLibrary.debugPrint($"Deactivated {child} (keyed child of {this})");
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual void _deactivateFailedChildSilently(Element child)
    {
        try
        {
            child._parent = null;
            child.detachRenderObject();
            _deactivateFailedSubtreeRecursively(child);
        }
        catch
        {
        }
    }

    internal static void _deactivateFailedSubtreeRecursively(Element element)
    {
        try
        {
            element.deactivate();
        }
        catch
        {
            element._ensureDeactivated();
        }
        element._lifecycleState = _ElementLifecycle__framework.failed;
        try
        {
            element.visitChildren(_deactivateFailedSubtreeRecursively);
        }
        catch
        {
        }
    }

    public virtual void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child.widget.key is GlobalKeyBase)
                {
                    _debugForgottenChildrenWithGlobalKey?.Add(child);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual void _activateWithParent(Element parent, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.inactive));
        _parent = parent;
        _owner = parent.owner;
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPrintGlobalKeyedWidgetLifecycle)
                {
                    PrintLibrary.debugPrint($"Reactivating {this} (now child of {_parent}).");
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _updateDepth(_parent!.depth);
        _updateBuildScopeRecursively();
        _activateRecursively(this);
        attachRenderObject(newSlot);
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.active));
    }

    internal static void _activateRecursively(Element element)
    {
        DartRuntimePrimitives.Assert(() => Equals(element._lifecycleState, _ElementLifecycle__framework.inactive));
        element.activate();
        DartRuntimePrimitives.Assert(() => Equals(element._lifecycleState, _ElementLifecycle__framework.active));
        element.visitChildren(_activateRecursively);
    }

    public virtual void activate()
    {
        if (FrameworkWorkTrace.Enabled) FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.Activate, this);
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.inactive));
        DartRuntimePrimitives.Assert(() => owner is not null);
        bool hadDependencies = ((_dependencies is { } __items203339 ? System.Linq.Enumerable.Any(__items203339) : (bool?)null) ?? false) || _hadUnsatisfiedDependencies;
        _lifecycleState = _ElementLifecycle__framework.active;
        _dependencies?.Clear();
        _hadUnsatisfiedDependencies = false;
        _updateInheritance();
        attachNotificationTree();
        if (_dirty)
        {
            owner!.scheduleBuildFor(this);
        }
        if (hadDependencies)
        {
            didChangeDependencies();
        }
    }

    public virtual void deactivate()
    {
        if (FrameworkWorkTrace.Enabled) FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.Deactivate, this);
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.active));
        DartRuntimePrimitives.Assert(() => _widget is not null);
        _ensureDeactivated();
    }

    internal virtual void _ensureDeactivated()
    {
        if (_dependencies is HashSet<InheritedElement> dependencies && Enumerable.Any(dependencies))
        {
            foreach (var dependency in dependencies)
            {
                if (FrameworkWorkTrace.Enabled) FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.RemoveDependency, this, dependency);
                dependency.removeDependent(this);
            }
        }
        _inheritedElements = null;
        _lifecycleState = _ElementLifecycle__framework.inactive;
    }

    public virtual void debugDeactivated()
    {
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.inactive));
    }

    public virtual void unmount()
    {
        if (FrameworkWorkTrace.Enabled) FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.Unmount, this);
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.inactive));
        DartRuntimePrimitives.Assert(() => _widget is not null);
        DartRuntimePrimitives.Assert(() => owner is not null);
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        Key? keyLocal = _widget?.key;
        if (keyLocal is GlobalKeyBase)
        {
            GlobalKeyBase key__207717__as207745 = (GlobalKeyBase)keyLocal;
            owner!._unregisterGlobalKey(key__207717__as207745, this);
        }
        _widget = null;
        _dependencies = null;
        _lifecycleState = _ElementLifecycle__framework.defunct;
    }

    public virtual bool debugExpectsRenderObjectForSlot(object? slot) => true;
    public virtual RenderObject? findRenderObject()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!Equals(_lifecycleState, _ElementLifecycle__framework.active))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot get renderObject of inactive element."), new ErrorDescription("In order for an element to have a valid renderObject, it must be " + "active, which means it is part of the tree.\n" + $"Instead, this element is in the {_lifecycleState} state.\n" + "If you called this method from a State object, consider guarding " + "it with State.mounted."), describeElement("The findRenderObject() method was called for the following element") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return renderObject;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size? size
    {
        get
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (!Equals(_lifecycleState, _ElementLifecycle__framework.active))
                    {
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot get size of inactive element."), new ErrorDescription("In order for an element to have a valid size, the element must be " + "active, which means it is part of the tree.\n" + $"Instead, this element is in the {_lifecycleState} state."), describeElement("The size getter was called for the following element") }));
                    }
                    if (owner!._debugBuilding)
                    {
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot get size during build."), new ErrorDescription("The size of this render object has not yet been determined because " + "the framework is still in the process of building widgets, which " + "means the render tree for this frame has not yet been determined. " + "The size getter should only be called from paint callbacks or " + "interaction event handlers (e.g. gesture callbacks)."), new ErrorSpacer(), new ErrorHint("If you need some sizing information during build to decide which " + "widgets to build, consider using a LayoutBuilder widget, which can " + "tell you the layout constraints at a given location in the tree. See " + "<https://api.flutter.dev/flutter/widgets/LayoutBuilder-class.html> " + "for more details."), new ErrorSpacer(), describeElement("The size getter was called for the following element") }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            RenderObject? renderObject = findRenderObject();
            DartRuntimePrimitives.Assert(() =>
                {
                    if (renderObject is null)
                    {
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot get size without a render object."), new ErrorHint("In order for an element to have a valid size, the element must have " + "an associated render object. This element does not have an associated " + "render object, which typically means that the size getter was called " + "too early in the pipeline (e.g., during the build phase) before the " + "framework has created the render tree."), describeElement("The size getter was called for the following element") }));
                    }
                    if (renderObject is RenderSliver)
                    {
                        RenderSliver renderObject__212317__as213062 = (RenderSliver)renderObject;
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot get size from a RenderSliver."), new ErrorHint("The render object associated with this element is a " + $"{DartRuntimePrimitives.RuntimeType(renderObject__212317__as213062)}, which is a subtype of RenderSliver. " + "Slivers do not have a size per se. They have a more elaborate " + "geometry description, which can be accessed by calling " + "findRenderObject and then using the \"geometry\" getter on the " + "resulting object."), describeElement("The size getter was called for the following element"), renderObject__212317__as213062.describeForError("The associated render sliver was") }));
                    }
                    if (renderObject is not RenderBox)
                    {
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot get size from a render object that is not a RenderBox."), new ErrorHint("Instead of being a subtype of RenderBox, the render object associated " + $"with this element is a {DartRuntimePrimitives.RuntimeType(renderObject)}. If this type of " + "render object does have a size, consider calling findRenderObject " + "and extracting its size manually."), describeElement("The size getter was called for the following element"), renderObject.describeForError("The associated render object was") }));
                    }
                    RenderBox box = DartRuntimePrimitives.ConvertValue<RenderBox>((RenderBox)renderObject);
                    if (!box.hasSize)
                    {
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot get size from a render object that has not been through layout."), new ErrorHint("The size of this render object has not yet been determined because " + "this render object has not yet been through layout, which typically " + "means that the size getter was called too early in the pipeline " + "(e.g., during the build phase) before the framework has determined " + "the size and position of the render objects during layout."), describeElement("The size getter was called for the following element"), box.describeForError("The render object from which the size was to be obtained was") }));
                    }
                    if (box.debugNeedsLayout)
                    {
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot get size from a render object that has been marked dirty for layout."), new ErrorHint("The size of this render object is ambiguous because this render object has " + "been modified since it was last laid out, which typically means that the size " + "getter was called too early in the pipeline (e.g., during the build phase) " + "before the framework has determined the size and position of the render " + "objects during layout."), describeElement("The size getter was called for the following element"), box.describeForError("The render object from which the size was to be obtained was"), new ErrorHint("Consider using debugPrintMarkNeedsLayoutStacks to determine why the render " + "object in question is dirty, if you did not expect this.") }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            if (renderObject is RenderBox)
            {
                RenderBox renderObject__212317__as216465 = (RenderBox)renderObject;
                return renderObject__212317__as216465.size;
            }
            return null;
        }
    }
    internal virtual bool _debugCheckStateIsActiveForAncestorLookup()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!Equals(_lifecycleState, _ElementLifecycle__framework.active))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Looking up a deactivated widget's ancestor is unsafe."), new ErrorDescription("At this point the state of the widget's element tree is no longer " + "stable."), new ErrorHint("To safely refer to a widget's ancestor in its dispose() method, " + "save a reference to the ancestor by calling dependOnInheritedWidgetOfExactType() " + "in the widget's didChangeDependencies() method.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool doesDependOnInheritedElement(InheritedElement ancestor)
    {
        return _dependencies?.Contains(ancestor) ?? false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InheritedWidget dependOnInheritedElement(InheritedElement ancestor, object? aspect = null)
    {
        if (FrameworkWorkTrace.Enabled) FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.RegisterDependency, this, ancestor);
        (_dependencies ??= new HashSet<InheritedElement>()).Add(ancestor);
        ancestor.updateDependencies(this, aspect);
        return ((InheritedWidget?)ancestor.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? dependOnInheritedWidgetOfExactType<T>(object? aspect = null)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        InheritedElement? ancestor = _inheritedElements?.GetValueOrDefault(typeof(T));
        if (ancestor is not null)
        {
            return ((T?)(object?)dependOnInheritedElement(ancestor, aspect: aspect))!;
        }
        _hadUnsatisfiedDependencies = true;
        return default;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? getInheritedWidgetOfExactType<T>()
    {
        return ((T?)(object?)getElementForInheritedWidgetOfExactType<T>()?.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InheritedElement? getElementForInheritedWidgetOfExactType<T>()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        return _inheritedElements?.GetValueOrDefault(typeof(T));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void attachNotificationTree()
    {
        _notificationTree = _parent?._notificationTree;
    }

    internal virtual void _updateInheritance()
    {
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.active));
        _inheritedElements = _parent?._inheritedElements ?? PersistentHashMap<Type, InheritedElement>.CreateEmpty();
    }

    public virtual T? findAncestorWidgetOfExactType<T>()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor = _parent;
        while ((ancestor is not null) && (!Equals(DartRuntimePrimitives.RuntimeType(ancestor.widget), typeof(T))))
        {
            ancestor = ancestor._parent;
        }
        return ((T?)(object?)ancestor?.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? findAncestorStateOfType<T>()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor = _parent;
        while (ancestor is not null)
        {
            if ((ancestor is StatefulElement) && (((StatefulElement)ancestor).state is T))
            {
                StatefulElement ancestor__219807__as219868 = (StatefulElement)ancestor;
                break;
            }
            ancestor = ancestor._parent;
        }
        var statefulAncestor = ((StatefulElement?)ancestor)!;
        return ((T?)(object?)statefulAncestor?.state)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? findRootAncestorStateOfType<T>()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor = _parent;
        StatefulElement? statefulAncestor = default!;
        while (ancestor is not null)
        {
            if ((ancestor is StatefulElement) && (((StatefulElement)ancestor).state is T))
            {
                StatefulElement ancestor__220244__as220344 = (StatefulElement)ancestor;
                statefulAncestor = ancestor__220244__as220344;
            }
            ancestor = ancestor._parent;
        }
        return ((T?)(object?)statefulAncestor?.state)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? findAncestorRenderObjectOfType<T>()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor = _parent;
        while (ancestor is not null)
        {
            if ((ancestor is RenderObjectElement) && (((RenderObjectElement)ancestor).renderObject is T))
            {
                RenderObjectElement ancestor__220677__as220738 = (RenderObjectElement)ancestor;
                return ((T?)(object?)ancestor__220677__as220738.renderObject)!;
            }
            ancestor = ancestor._parent;
        }
        return default;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void visitAncestorElements(Func<Element, bool> visitor)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor = _parent;
        while ((ancestor is not null) && visitor(ancestor))
        {
            ancestor = ancestor._parent;
        }
    }

    public virtual void didChangeDependencies()
    {
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.active));
        DartRuntimePrimitives.Assert(() => _debugCheckOwnerBuildTargetExists("didChangeDependencies"));
        FrameworkWorkCounters.Add(FrameworkWork.DependencyChanged);
        if (FrameworkWorkTrace.Enabled) FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.Dependency, this);
        markNeedsBuild();
    }

    internal virtual bool _debugCheckOwnerBuildTargetExists(string methodName)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (owner!._debugCurrentBuildTarget is null)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{methodName} for {DartRuntimePrimitives.RuntimeType(widget)} was called at an " + "inappropriate time."), new ErrorDescription("It may only be called while the widgets are being built."), new ErrorHint($"A possible cause of this error is when {methodName} is called during " + "one of:\n" + " * network I/O event\n" + " * file I/O event\n" + " * timer\n" + " * microtask (caused by Future.then, async/await, scheduleMicrotask)") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string debugGetCreatorChain(long limit)
    {
        var chain = new List<string>();
        Element? node = this;
        while ((checked(chain.Count) < limit) && (node is not null))
        {
            chain.Add(((Diagnosticable)node).toStringShort());
            node = node._parent;
        }
        if (node is not null)
        {
            chain.Add("⋯");
        }
        return string.Join(" ← ", chain);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<Element> debugGetDiagnosticChain()
    {
        var chain = new List<Element> { this };
        Element? node = _parent;
        while (node is not null)
        {
            chain.Add(node);
            node = node._parent;
        }
        return chain;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispatchNotification(Notification notification)
    {
        _notificationTree?.dispatchNotification(notification);
    }

    public virtual string toStringShort() => ((Diagnosticable?)_widget)?.toStringShort() ?? $"{DiagnosticsLibrary.describeIdentity(this)}(DEFUNCT)";
    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new _ElementDiagnosticableTreeNode__framework(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.defaultDiagnosticsTreeStyle = DiagnosticsTreeStyle.dense;
        if (!Equals(_lifecycleState, _ElementLifecycle__framework.initial))
        {
            properties.add(new ObjectFlagProperty<long>("depth", depth, ifNull: "no depth"));
        }
        properties.add(new ObjectFlagProperty<Widget>("widget", _widget, ifNull: "no widget"));
        properties.add(new DiagnosticsProperty<Key>("key", _widget?.key, showName: false, defaultValue: null, level: DiagnosticLevel.hidden));
        _widget?.debugFillProperties(properties);
        properties.add(new FlagProperty("dirty", value: dirty, ifTrue: "dirty"));
        HashSet<InheritedElement>? deps = _dependencies;
        if ((deps is not null) && Enumerable.Any(deps))
        {
            List<InheritedElement> sortedDependencies = ((Func<List<InheritedElement>>)(() =>
{
    var __cascade = deps.ToList();
    __cascade.sort((a, b) => ((Diagnosticable)a).toStringShort().CompareTo(((Diagnosticable)b).toStringShort()));
    return __cascade;
}))().ToList();
            List<DiagnosticsNode> diagnosticsDependencies = sortedDependencies.map((element) => ((Diagnosticable)element.widget).toDiagnosticsNode(style: DiagnosticsTreeStyle.sparse)).ToList().ToList();
            properties.add(new DiagnosticsProperty<HashSet<InheritedElement>>("dependencies", deps, description: diagnosticsDependencies.ToString()));
        }
    }

    public virtual List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        visitChildren((child) =>
        {
            children.Add(((Diagnosticable)child).toDiagnosticsNode());
        });
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool dirty => _dirty;
    public virtual void markNeedsBuild()
    {
        FrameworkWorkCounters.Add(FrameworkWork.MarkBuild);
        if (_dirty) FrameworkWorkCounters.Add(FrameworkWork.MarkBuildAlreadyDirty);
        DartRuntimePrimitives.Assert(() => !Equals(_lifecycleState, _ElementLifecycle__framework.defunct));
        if (!Equals(_lifecycleState, _ElementLifecycle__framework.active))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => owner is not null);
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.active));
        DartRuntimePrimitives.Assert(() =>
            {
                if (owner!._debugBuilding)
                {
                    DartRuntimePrimitives.Assert(() => owner!._debugCurrentBuildTarget is not null);
                    DartRuntimePrimitives.Assert(() => owner!._debugStateLocked);
                    if (_debugIsDescendantOf(owner!._debugCurrentBuildTarget!))
                    {
                        return true;
                    }
                    var information = new List<DiagnosticsNode> { new ErrorSummary("setState() or markNeedsBuild() called during build."), new ErrorDescription($"This {DartRuntimePrimitives.RuntimeType(widget)} widget cannot be marked as needing to build because the framework " + "is already in the process of building widgets. A widget can be marked as " + "needing to be built during the build phase only if one of its ancestors " + "is currently building. This exception is allowed because the framework " + "builds parent widgets before children, which means a dirty descendant " + "will always be built. Otherwise, the framework might not visit this " + "widget during this build phase."), describeElement("The widget on which setState() or markNeedsBuild() was called was") };
                    if (owner!._debugCurrentBuildTarget is not null)
                    {
                        information.Add(owner!._debugCurrentBuildTarget!.describeWidget("The widget which was currently being built when the offending call was made was"));
                    }
                    throw DartRuntimePrimitives.AsException(new FlutterError(information));
                }
                else
                {
                    if (owner!._debugStateLocked)
                    {
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("setState() or markNeedsBuild() called when widget tree was locked."), new ErrorDescription($"This {DartRuntimePrimitives.RuntimeType(widget)} widget cannot be marked as needing to build " + "because the framework is locked."), describeElement("The widget on which setState() or markNeedsBuild() was called was") }));
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (dirty)
        {
            return;
        }
        _dirty = true;
        owner!.scheduleBuildFor(this);
    }

    public virtual void rebuild(bool force = false)
    {
        DartRuntimePrimitives.Assert(() => !Equals(_lifecycleState, _ElementLifecycle__framework.initial));
        if ((!Equals(_lifecycleState, _ElementLifecycle__framework.active)) || !_dirty && !force)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                DebugLibrary.debugOnRebuildDirtyWidget?.Invoke(this, _debugBuiltOnce);
                if (DebugLibrary.debugPrintRebuildDirtyWidgets)
                {
                    if (!_debugBuiltOnce)
                    {
                        PrintLibrary.debugPrint($"Building {this}");
                        _debugBuiltOnce = true;
                    }
                    else
                    {
                        PrintLibrary.debugPrint($"Rebuilding {this}");
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.active));
        DartRuntimePrimitives.Assert(() => owner!._debugStateLocked);
        Element? debugPreviousBuildTarget = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                debugPreviousBuildTarget = owner!._debugCurrentBuildTarget;
                owner!._debugCurrentBuildTarget = this;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        try
        {
            FrameworkWorkCounters.Add(FrameworkWork.Rebuild);
            if (force) FrameworkWorkCounters.Add(FrameworkWork.ForcedRebuild);
            using var profile = FrameworkWorkCounters.Enabled ? FrameworkWorkProfile.Begin(widget.GetType()) : default;
            if (FrameworkWorkTrace.Enabled) FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.Build, this);
            performRebuild();
        }
        finally
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    owner!._debugElementWasRebuilt(this);
                    DartRuntimePrimitives.Assert(() => Equals(owner!._debugCurrentBuildTarget, this));
                    owner!._debugCurrentBuildTarget = debugPreviousBuildTarget;
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        DartRuntimePrimitives.Assert(() => !_dirty);
    }

    public virtual void performRebuild()
    {
        _dirty = false;
    }

}

internal class _ElementDiagnosticableTreeNode__framework : DiagnosticableTreeNode<DiagnosticableTree>
{
    public virtual bool stateful { get; private set; } = default!;

    internal _ElementDiagnosticableTreeNode__framework(string? name = null, Element value = default!, DiagnosticsTreeStyle? style = default!, bool stateful = false) : base(name: name, value: value, style: style)
    {
        this.stateful = stateful;
    }

    public override DartMap<string, object?> toJsonMap(DiagnosticsSerializationDelegate? @delegate = null)
    {
        DartMap<string, object?> json = base.toJsonMap(@delegate);
        var element = ((Element?)value)!;
        if (!element.debugIsDefunct)
        {
            json["widgetRuntimeType"] = DartRuntimePrimitives.RuntimeTypeName(element.widget);
        }
        json["stateful"] = stateful;
        return json;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate Widget ErrorWidgetBuilder(FlutterErrorDetails details);

public class ErrorWidget : LeafRenderObjectWidget
{
    public static Func<FlutterErrorDetails, Widget> builder = _defaultErrorWidgetBuilder;
    public virtual string message { get; private set; } = default!;
    internal virtual FlutterError? _flutterError { get; private set; }

    public ErrorWidget(object exception) : base(key: new UniqueKey())
    {
        message = _stringify(exception);
        _flutterError = (exception is FlutterError) ? ((FlutterError)exception) : null;
    }

    public static ErrorWidget CreateWithDetails(string message = "", FlutterError? error = null)
    {
        var __instance = new ErrorWidget(default!);
        __instance.message = message;
        __instance._flutterError = error;
        return __instance;
    }

    internal static Widget _defaultErrorWidgetBuilder(FlutterErrorDetails details)
    {
        var messageLocal = "";
        DartRuntimePrimitives.Assert(() =>
            {
                messageLocal = $"{_stringify(details.exception)}\nSee also: https://docs.flutter.dev/testing/errors";
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        object exceptionLocal = details.exception;
        return CreateWithDetails(message: messageLocal, error: (exceptionLocal is FlutterError) ? ((FlutterError)exceptionLocal) : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static string _stringify(object? exception)
    {
        try
        {
            return exception is null ? "Error" : exception.ToString()!;
        }
        catch (Exception)
        {
        }
        return "Error";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderErrorBox(message));
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        if (_flutterError is null)
        {
            properties.add(new StringProperty("message", message, quoted: false));
        }
        else
        {
            properties.add(((Diagnosticable)_flutterError).toDiagnosticsNode(style: DiagnosticsTreeStyle.whitespace));
        }
    }

}

public delegate Widget WidgetBuilder(BuildContext context);

public delegate Widget IndexedWidgetBuilder(BuildContext context, long index);

public delegate Widget? NullableIndexedWidgetBuilder(BuildContext context, long index);

public delegate Widget TransitionBuilder(BuildContext context, Widget? child);

public abstract class ComponentElement : Element
{
    internal virtual Element? _child { get; set; } = default;
    internal virtual bool _debugDoingBuild { get; set; } = false;

    protected ComponentElement(Widget widget) : base(widget)
    {
    }

    public override bool debugDoingBuild => _debugDoingBuild;
    public override Element? renderObjectAttachingChild => _child;
    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        DartRuntimePrimitives.Assert(() => _child is null);
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.active));
        _firstBuild();
        DartRuntimePrimitives.Assert(() => _child is not null);
    }

    internal virtual void _firstBuild()
    {
        rebuild();
    }

    public override void performRebuild()
    {
        Widget built = default!;
        try
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingBuild = true;
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            built = build();
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingBuild = false;
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            DebugLibrary.debugWidgetBuilderValue(widget, built);
        }
        catch (Exception e)
        {
            var stack = new System.Diagnostics.StackTrace();
            _debugDoingBuild = false;
            built = ErrorWidget.builder(FrameworkLibrary._reportException(new ErrorDescription($"building {this}"), e, stack, informationCollector: () => new List<DiagnosticsNode>()));
        }
        try
        {
            _child = updateChild(_child, built, slot);
            DartRuntimePrimitives.Assert(() => _child is not null);
        }
        catch (Exception eLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            built = ErrorWidget.builder(FrameworkLibrary._reportException(new ErrorDescription($"building {this}"), eLocal, stackLocal, informationCollector: () => new List<DiagnosticsNode>()));
            try
            {
                _child?.deactivate();
            }
            catch
            {
            }
            _child = updateChild(null, built, slot);
        }
        base.performRebuild();
    }

    public abstract Widget build();
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

}

public class StatelessElement : ComponentElement
{
    public StatelessElement(StatelessWidget widget) : base(widget)
    {
    }

    public override Widget build() => ((StatelessWidget?)widget)!.build(this);
    public override void update(Widget newWidget)
    {
        var __newWidget = (StatelessWidget)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => Equals(widget, __newWidget));
        rebuild(force: true);
    }

}

public class StatefulElement : ComponentElement
{
    internal virtual IState? _state { get; set; } = default;
    internal virtual bool _didChangeDependencies { get; set; } = false;

    public StatefulElement(StatefulWidget widget) : base(widget)
    {
        _state = widget.createState();
        _state!._element = this;
        _state!._widget = widget;
    }

    public override Widget build() => state.build(this);
    public virtual IState state => DartRuntimePrimitives.ConvertValue<IState>(_state!);
    public override void reassemble()
    {
        state.reassemble();
        base.reassemble();
    }

    internal override void _firstBuild()
    {
        DartRuntimePrimitives.Assert(() => Equals(state._debugLifecycleState, _StateLifecycle__framework.created));
        object? debugCheckForReturnedFuture = DartRuntimePrimitives.CaptureVoid(() => state.initState());
        DartRuntimePrimitives.Assert(() =>
            {
                if (debugCheckForReturnedFuture is Future)
                {
                    Future debugCheckForReturnedFuture__250900__as250986 = (Future)debugCheckForReturnedFuture;
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{DartRuntimePrimitives.RuntimeType(state)}.initState() returned a Future."), new ErrorDescription("State.initState() must be a void method without an `async` keyword."), new ErrorHint("Rather than awaiting on asynchronous work directly inside of initState, " + "call a separate method to do this work without awaiting it.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                state._debugLifecycleState = _StateLifecycle__framework.initialized;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        state.didChangeDependencies();
        DartRuntimePrimitives.Assert(() =>
            {
                state._debugLifecycleState = _StateLifecycle__framework.ready;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        base._firstBuild();
    }

    public override void performRebuild()
    {
        if (_didChangeDependencies)
        {
            state.didChangeDependencies();
            _didChangeDependencies = false;
        }
        base.performRebuild();
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (StatefulWidget)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => Equals(widget, __newWidget));
        StatefulWidget oldWidget = state._widget!;
        state._widget = ((StatefulWidget?)widget)!;
        if (FrameworkWorkTrace.Enabled) FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.DidUpdateWidget, this, state);
        object? debugCheckForReturnedFuture = DartRuntimePrimitives.CaptureVoid(() => state.didUpdateWidget(oldWidget));
        DartRuntimePrimitives.Assert(() =>
            {
                if (debugCheckForReturnedFuture is Future)
                {
                    Future debugCheckForReturnedFuture__252202__as252303 = (Future)debugCheckForReturnedFuture;
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{DartRuntimePrimitives.RuntimeType(state)}.didUpdateWidget() returned a Future."), new ErrorDescription("State.didUpdateWidget() must be a void method without an `async` keyword."), new ErrorHint("Rather than awaiting on asynchronous work directly inside of didUpdateWidget, " + "call a separate method to do this work without awaiting it.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        rebuild(force: true);
    }

    public override void activate()
    {
        base.activate();
        state.activate();
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.active));
        markNeedsBuild();
    }

    public override void deactivate()
    {
        state.deactivate();
        base.deactivate();
    }

    public override void unmount()
    {
        base.unmount();
        state.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (Equals(state._debugLifecycleState, _StateLifecycle__framework.defunct))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{DartRuntimePrimitives.RuntimeType(state)}.dispose failed to call super.dispose."), new ErrorDescription("dispose() implementations must always call their superclass dispose() method, to ensure " + "that all the resources used by the widget are fully released.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        state._element = null;
        _state = null;
    }

    public override InheritedWidget dependOnInheritedElement(InheritedElement ancestor, object? aspect = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                Type targetType = DartRuntimePrimitives.RuntimeType(ancestor.widget);
                if (Equals(state._debugLifecycleState, _StateLifecycle__framework.created))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"dependOnInheritedWidgetOfExactType<{targetType}>() or dependOnInheritedElement() was called before {DartRuntimePrimitives.RuntimeType(state)}.initState() completed."), new ErrorDescription("When an inherited widget changes, for example if the value of Theme.of() changes, " + "its dependent widgets are rebuilt. If the dependent widget's reference to " + "the inherited widget is in a constructor or an initState() method, " + "then the rebuilt dependent widget will not reflect the changes in the " + "inherited widget."), new ErrorHint("Typically references to inherited widgets should occur in widget build() methods. Alternatively, " + "initialization based on inherited widgets can be placed in the didChangeDependencies method, which " + "is called after initState and whenever the dependencies change thereafter.") }));
                }
                if (Equals(state._debugLifecycleState, _StateLifecycle__framework.defunct))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"dependOnInheritedWidgetOfExactType<{targetType}>() or dependOnInheritedElement() was called after dispose(): {this}"), new ErrorDescription("This error happens if you call dependOnInheritedWidgetOfExactType() on the " + "BuildContext for a widget that no longer appears in the widget tree " + "(e.g., whose parent widget no longer includes the widget in its " + "build). This error can occur when code calls " + "dependOnInheritedWidgetOfExactType() from a timer or an animation callback."), new ErrorHint("The preferred solution is to cancel the timer or stop listening to the " + "animation in the dispose() callback. Another solution is to check the " + "\"mounted\" property of this object before calling " + "dependOnInheritedWidgetOfExactType() to ensure the object is still in the " + "tree."), new ErrorHint("This error might indicate a memory leak if " + "dependOnInheritedWidgetOfExactType() is being called because another object " + "is retaining a reference to this State object after it has been " + "removed from the tree. To avoid memory leaks, consider breaking the " + "reference to this object during dispose().") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return base.dependOnInheritedElement(ancestor!, aspect: aspect);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _didChangeDependencies = true;
    }

    public override DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new _ElementDiagnosticableTreeNode__framework(name: name, value: this, style: style, stateful: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<IState>("state", _state, defaultValue: null));
    }

}

public abstract class ProxyElement : ComponentElement
{
    protected ProxyElement(ProxyWidget widget) : base(widget)
    {
    }

    public override Widget build() => ((ProxyWidget)widget).child;
    public override void update(Widget newWidget)
    {
        var __newWidget = (ProxyWidget)newWidget;
        var oldWidget = ((ProxyWidget?)widget)!;
        DartRuntimePrimitives.Assert(() => !Equals(widget, __newWidget));
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => Equals(widget, __newWidget));
        updated(oldWidget);
        rebuild(force: true);
    }

    public virtual void updated(ProxyWidget oldWidget)
    {
        notifyClients(oldWidget);
    }

    public abstract void notifyClients(ProxyWidget oldWidget);
}

internal interface IParentDataElement
{
    Type debugParentDataType { get; }
    void applyParentDataTo(RenderObjectElement child);
}

public class ParentDataElement<T> : ProxyElement, IParentDataElement
{
    public ParentDataElement(ParentDataWidget<T> widget) : base(widget)
    {
    }

    public virtual Type debugParentDataType
    {
        get
        {
            Type? @type = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    @type = typeof(T);
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            if (@type is not null)
            {
                return @type!;
            }
            throw new NotSupportedException("debugParentDataType is only supported in debug builds");
        }
    }
    internal virtual void _applyParentData(ParentDataWidget<T> widget)
    {
        void applyParentDataToChild(Element child)
        {
            if (child is RenderObjectElement)
            {
                ((RenderObjectElement)child)._updateParentData(widget);
            }
            else
            {
                if (child.renderObjectAttachingChild is not null)
                {
                    applyParentDataToChild(child.renderObjectAttachingChild!);
                }
            }
        }
        if (renderObjectAttachingChild is not null)
        {
            applyParentDataToChild(renderObjectAttachingChild!);
        }
    }

    void IParentDataElement.applyParentDataTo(RenderObjectElement child) =>
        child._updateParentData((ParentDataWidget<T>)widget);

    public virtual void applyWidgetOutOfTurn(ParentDataWidget<T> newWidget)
    {
        DartRuntimePrimitives.Assert(() => newWidget.debugCanApplyOutOfTurn());
        DartRuntimePrimitives.Assert(() => Equals(newWidget.child, ((ParentDataWidget<T>)widget).child));
        _applyParentData(newWidget);
    }

    public override void notifyClients(ProxyWidget oldWidget)
    {
        var __oldWidget = (ParentDataWidget<T>)oldWidget;
        _applyParentData(((ParentDataWidget<T>?)widget)!);
    }

}

public class InheritedElement : ProxyElement
{
    internal virtual DartMap<Element, object?> _dependents { get; private set; } = new DartMap<Element, object?>();

    public InheritedElement(InheritedWidget widget) : base(widget)
    {
    }

    internal override void _updateInheritance()
    {
        DartRuntimePrimitives.Assert(() => Equals(_lifecycleState, _ElementLifecycle__framework.active));
        PersistentHashMap<Type, InheritedElement> incomingWidgets = _parent?._inheritedElements ?? PersistentHashMap<Type, InheritedElement>.CreateEmpty();
        _inheritedElements = incomingWidgets.put(DartRuntimePrimitives.RuntimeType(widget), this);
    }

    public override void debugDeactivated()
    {
        DartRuntimePrimitives.Assert(() => !Enumerable.Any(_dependents));
        base.debugDeactivated();
    }

    public virtual object? getDependencies(Element dependent)
    {
        return _dependents.GetValueOrDefault(dependent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setDependencies(Element dependent, object? value)
    {
        _dependents[dependent] = value;
    }

    public virtual void updateDependencies(Element dependent, object? aspect)
    {
        setDependencies(dependent, null);
    }

    public virtual void notifyDependent(InheritedWidget oldWidget, Element dependent)
    {
        dependent.didChangeDependencies();
    }

    public virtual void removeDependent(Element dependent)
    {
        _dependents.remove(dependent);
    }

    public override void updated(ProxyWidget oldWidget)
    {
        var __oldWidget = (InheritedWidget)oldWidget;
        if (((InheritedWidget?)widget)!.updateShouldNotify(__oldWidget))
        {
            base.updated(__oldWidget);
        }
    }

    public override void notifyClients(ProxyWidget oldWidget)
    {
        var __oldWidget = (InheritedWidget)oldWidget;
        DartRuntimePrimitives.Assert(() => _debugCheckOwnerBuildTargetExists("notifyClients"));
        foreach (Element dependent in _dependents.Keys)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    Element? ancestor = dependent._parent;
                    while ((!Equals(ancestor, this)) && (ancestor is not null))
                    {
                        ancestor = ancestor._parent;
                    }
                    return Equals(ancestor, this);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            DartRuntimePrimitives.Assert(() => dependent._dependencies!.Contains(this));
            notifyDependent(__oldWidget, dependent);
        }
    }

}

public abstract class RenderObjectElement : Element
{
    internal virtual RenderObject? _renderObject { get; set; } = default;
    internal virtual bool _debugDoingBuild { get; set; } = false;
    internal virtual RenderObjectElement? _ancestorRenderObjectElement { get; set; } = default;

    protected RenderObjectElement(RenderObjectWidget widget) : base(widget)
    {
    }

    public override RenderObject renderObject
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _renderObject is not null, () => (object?)$"{GetType()} unmounted");
            return _renderObject!;
        }
    }
    public override Element? renderObjectAttachingChild => DartRuntimePrimitives.ConvertValue<Element>(null);
    public override bool debugDoingBuild => _debugDoingBuild;
    internal virtual RenderObjectElement? _findAncestorRenderObjectElement()
    {
        Element? ancestor = _parent;
        while ((ancestor is not null) && (ancestor is not RenderObjectElement))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (!ancestor!.debugExpectsRenderObjectForSlot(slot))
                    {
                        ancestor = null;
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            ancestor = ancestor?._parent;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (ancestor?.debugExpectsRenderObjectForSlot(slot) == false)
                {
                    ancestor = null;
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((RenderObjectElement?)ancestor)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _debugCheckCompetingAncestors(List<IParentDataElement> result, HashSet<Type> debugAncestorTypes, HashSet<Type> debugParentDataTypes, List<Type> debugAncestorCulprits)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((checked(debugAncestorTypes.Count) != checked((long)result.Count)) || (checked(debugParentDataTypes.Count) != checked((long)result.Count)))
                {
                    DartRuntimePrimitives.Assert(() => (checked(debugAncestorTypes.Count) < checked((long)result.Count)) || (checked(debugParentDataTypes.Count) < checked((long)result.Count)));
                    try
                    {
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Incorrect use of ParentDataWidget."), new ErrorDescription("Competing ParentDataWidgets are providing parent data to the " + "same RenderObject:"), new ErrorDescription("A RenderObject can receive parent data from multiple " + "ParentDataWidgets, but the Type of ParentData must be unique to " + "prevent one overwriting another."), new ErrorHint("Usually, this indicates that one or more of the offending " + "ParentDataWidgets listed above isn't placed inside a dedicated " + "compatible ancestor widget that it isn't sharing with another " + "ParentDataWidget of the same type."), new ErrorHint("Otherwise, separating aspects of ParentData to prevent " + "conflicts can be done using mixins, mixing them all in on the " + "full ParentData Object, such as KeepAlive does with " + "KeepAliveParentDataMixin."), new ErrorDescription("The ownership chain for the RenderObject that received the " + $"parent data was:\n  {debugGetCreatorChain(10L)}") }));
                    }
                    catch (FlutterError error)
                    {
                        FrameworkLibrary._reportException(new ErrorSummary("while looking for parent data."), error, error.stackTrace);
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual List<IParentDataElement> _findAncestorParentDataElements()
    {
        Element? ancestorLocal = _parent;
        var result = new List<IParentDataElement>();
        var debugAncestorTypes = new HashSet<Type>();
        var debugParentDataTypes = new HashSet<Type>();
        var debugAncestorCulprits = new List<Type>();
        while ((ancestorLocal is not null) && (ancestorLocal is not RenderObjectElement))
        {
            if (ancestorLocal is IParentDataElement)
            {
                IParentDataElement ancestor__283177__as284599 = (IParentDataElement)ancestorLocal;
                DartRuntimePrimitives.Assert(() =>
                    {
                        IParentDataElement ancestor = ancestor__283177__as284599;
                        if (!debugAncestorTypes.Add(DartRuntimePrimitives.RuntimeType(ancestor)) || !debugParentDataTypes.Add(ancestor.debugParentDataType))
                        {
                            debugAncestorCulprits.Add(DartRuntimePrimitives.RuntimeType(ancestor));
                        }
                        return true;
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                result.Add(ancestor__283177__as284599);
            }
            ancestorLocal = ancestorLocal._parent;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (!Enumerable.Any(result) || (ancestorLocal is null))
                {
                    return true;
                }
                _debugCheckCompetingAncestors(result, debugAncestorTypes, debugParentDataTypes, debugAncestorCulprits);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingBuild = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _renderObject = ((RenderObjectWidget?)widget)!.createRenderObject(this);
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.RequireValue(_renderObject!.debugDisposed));
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingBuild = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                _debugUpdateRenderObjectOwner();
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => Equals(slot, newSlot));
        attachRenderObject(newSlot);
        base.performRebuild();
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (RenderObjectWidget)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => Equals(widget, __newWidget));
        DartRuntimePrimitives.Assert(() =>
            {
                _debugUpdateRenderObjectOwner();
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _performRebuild();
    }

    internal virtual void _debugUpdateRenderObjectOwner()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                renderObject.debugCreator = new DebugCreator(this);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override void performRebuild()
    {
        _performRebuild();
    }

    internal virtual void _performRebuild()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingBuild = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        ((RenderObjectWidget?)widget)!.updateRenderObject(this, renderObject);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingBuild = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        base.performRebuild();
    }

    public override void deactivate()
    {
        base.deactivate();
        DartRuntimePrimitives.Assert(() => !renderObject.attached, () => (object?)"A RenderObject was still attached when attempting to deactivate its " + $"RenderObjectElement: {renderObject}");
    }

    public override void unmount()
    {
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.RequireValue(renderObject.debugDisposed), () => (object?)"A RenderObject was disposed prior to its owning element being unmounted: " + $"{renderObject}");
        var oldWidget = ((RenderObjectWidget?)widget)!;
        base.unmount();
        DartRuntimePrimitives.Assert(() => !renderObject.attached, () => (object?)"A RenderObject was still attached when attempting to unmount its " + $"RenderObjectElement: {renderObject}");
        oldWidget.didUnmountRenderObject(renderObject);
        _renderObject!.dispose();
        _renderObject = null;
    }

    internal virtual void _updateParentData<T>(ParentDataWidget<T> parentDataWidget)
    {
        var applyParentDataLocal = true;
        DartRuntimePrimitives.Assert(() =>
            {
                try
                {
                    if (!parentDataWidget.debugIsValidRenderObject(renderObject))
                    {
                        applyParentDataLocal = false;
                        throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Incorrect use of ParentDataWidget.") }));
                    }
                }
                catch (FlutterError e)
                {
                    FrameworkLibrary._reportException(new ErrorSummary("while applying parent data."), e, e.stackTrace);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (applyParentDataLocal)
        {
            parentDataWidget.applyParentData(renderObject);
        }
    }

    public override void updateSlot(object? newSlot)
    {
        object? oldSlot = slot;
        DartRuntimePrimitives.Assert(() => !Equals(oldSlot, newSlot));
        base.updateSlot(newSlot);
        DartRuntimePrimitives.Assert(() => Equals(slot, newSlot));
        DartRuntimePrimitives.Assert(() => Equals(_ancestorRenderObjectElement, _findAncestorRenderObjectElement()));
        _ancestorRenderObjectElement?.moveRenderObjectChild(renderObject, oldSlot, slot);
    }

    public override void attachRenderObject(object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => _ancestorRenderObjectElement is null);
        _slot = newSlot;
        _ancestorRenderObjectElement = _findAncestorRenderObjectElement();
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ancestorRenderObjectElement is null)
                {
                    FlutterError.reportError(new FlutterErrorDetails(exception: new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The render object for {toStringShort()} cannot find ancestor render object to attach to."), new ErrorDescription($"The ownership chain for the RenderObject in question was:\n  {debugGetCreatorChain(10L)}"), new ErrorHint("Try wrapping your widget in a View widget or any other widget that is backed by " + $"a {typeof(RenderTreeRootElement)} to serve as the root of the render tree.") })));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _ancestorRenderObjectElement?.insertRenderObjectChild(renderObject, newSlot);
        List<IParentDataElement> parentDataElements = _findAncestorParentDataElements();
        foreach (var parentDataElement in parentDataElements)
        {
            parentDataElement.applyParentDataTo(this);
        }
    }

    public override void detachRenderObject()
    {
        if (_ancestorRenderObjectElement is not null)
        {
            _ancestorRenderObjectElement!.removeRenderObjectChild(renderObject, slot);
            _ancestorRenderObjectElement = null;
        }
        _slot = null;
    }

    public abstract void insertRenderObjectChild(RenderObject child, object? slot);
    public abstract void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot);
    public abstract void removeRenderObjectChild(RenderObject child, object? slot);
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<RenderObject>("renderObject", _renderObject, defaultValue: null));
    }

}

public abstract class RootRenderObjectElement : RenderObjectElement, RootElementMixin
{

    protected RootRenderObjectElement(RenderObjectWidget widget) : base(widget)
    {
    }

    public virtual void assignOwner(BuildOwner owner)
    {
        _owner = owner;
        _parentBuildScope = new BuildScope();
    }

    public override void mount(Element? parent, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => parent is null);
        DartRuntimePrimitives.Assert(() => newSlot is null);
        base.mount(parent, newSlot);
    }

}

public interface RootElementMixin
{
    public void assignOwner(BuildOwner owner);
    public void mount(Element? parent, object? newSlot);
}

public class LeafRenderObjectElement : RenderObjectElement
{
    public LeafRenderObjectElement(LeafRenderObjectWidget widget) : base(widget)
    {
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => false);
        base.forgetChild(child);
    }

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return widget.debugDescribeChildren();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SingleChildRenderObjectElement : RenderObjectElement
{
    internal virtual Element? _child { get; set; } = default;

    public SingleChildRenderObjectElement(SingleChildRenderObjectWidget widget) : base(widget)
    {
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
        _child = updateChild(_child, ((SingleChildRenderObjectWidget)widget).child, null);
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (SingleChildRenderObjectWidget)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => Equals(widget, __newWidget));
        _child = updateChild(_child, ((SingleChildRenderObjectWidget)widget).child, null);
    }

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        var renderObjectLocal = (IRenderObjectWithChild)renderObject;
        DartRuntimePrimitives.Assert(() => slot is null);
        DartRuntimePrimitives.Assert(() => renderObjectLocal.debugValidateChild(child));
        renderObjectLocal.child = child;
        DartRuntimePrimitives.Assert(() => Equals(renderObjectLocal, renderObject));
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        var renderObjectLocal = (IRenderObjectWithChild)renderObject;
        DartRuntimePrimitives.Assert(() => slot is null);
        DartRuntimePrimitives.Assert(() => Equals(renderObjectLocal.child, child));
        renderObjectLocal.child = null;
        DartRuntimePrimitives.Assert(() => Equals(renderObjectLocal, renderObject));
    }

}

public class MultiChildRenderObjectElement : RenderObjectElement
{
    internal virtual List<Element> _children { get; set; } = default!;
    internal virtual HashSet<Element> _forgottenChildren { get; private set; } = new HashSet<Element>();

    public MultiChildRenderObjectElement(MultiChildRenderObjectWidget widget) : base(widget)
    {
        System.Diagnostics.Debug.Assert(!DebugLibrary.debugChildrenHaveDuplicateKeys(widget, widget.children.Cast<Widget>()));
    }

    public override RenderObject renderObject
    {
        get
        {
            return DartRuntimePrimitives.ConvertValue<RenderObject>(base.renderObject);
        }
    }
    public virtual IEnumerable<Element> children => _children.where((child) => !_forgottenChildren.Contains(child));
    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        var __slot = slot as IndexedSlot<Element?> ?? throw new ArgumentException("A multi-child render slot must be an IndexedSlot.", nameof(slot));
        var renderObjectLocal = (IContainerRenderObject)renderObject;
        DartRuntimePrimitives.Assert(() => renderObjectLocal.debugValidateChild(child));
        renderObjectLocal.insert(child, after: __slot.value?.renderObject);
        DartRuntimePrimitives.Assert(() => Equals(renderObjectLocal, renderObject));
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        var __oldSlot = oldSlot as IndexedSlot<Element?> ?? throw new ArgumentException("A multi-child render slot must be an IndexedSlot.", nameof(oldSlot));
        var __newSlot = newSlot as IndexedSlot<Element?> ?? throw new ArgumentException("A multi-child render slot must be an IndexedSlot.", nameof(newSlot));
        var renderObjectLocal = (IContainerRenderObject)renderObject;
        DartRuntimePrimitives.Assert(() => Equals(child.parent, renderObjectLocal));
        renderObjectLocal.move(child, after: __newSlot.value?.renderObject);
        DartRuntimePrimitives.Assert(() => Equals(renderObjectLocal, renderObject));
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        var renderObjectLocal = (IContainerRenderObject)renderObject;
        DartRuntimePrimitives.Assert(() => Equals(child.parent, renderObjectLocal));
        renderObjectLocal.remove(child);
        DartRuntimePrimitives.Assert(() => Equals(renderObjectLocal, renderObject));
    }

    public override void visitChildren(Action<Element> visitor)
    {
        foreach (Element child in _children)
        {
            if (!_forgottenChildren.Contains(child))
            {
                visitor(child);
            }
        }
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => _children.Contains(child));
        DartRuntimePrimitives.Assert(() => !_forgottenChildren.Contains(child));
        _forgottenChildren.Add(child);
        base.forgetChild(child);
    }

    internal virtual bool _debugCheckHasAssociatedRenderObject(Element newChild)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (newChild.renderObject is null)
                {
                    FlutterError.reportError(new FlutterErrorDetails(exception: new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("The children of `MultiChildRenderObjectElement` must each has an associated render object."), new ErrorHint($"This typically means that the `{newChild.widget}` or its children\n" + "are not a subtype of `RenderObjectWidget`."), newChild.describeElement("The following element does not have an associated render object"), new DiagnosticsDebugCreator(new DebugCreator(newChild)) })));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Element inflateWidget(Widget newWidget, object? newSlot)
    {
        Element newChild = base.inflateWidget(newWidget, newSlot);
        DartRuntimePrimitives.Assert(() => _debugCheckHasAssociatedRenderObject(newChild));
        return newChild;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        var multiChildRenderObjectWidget = ((MultiChildRenderObjectWidget?)widget)!;
        var childrenLocal = new List<Element>(Enumerable.Repeat<Element>(_NullElement__framework.instance, checked((int)checked((long)multiChildRenderObjectWidget.children.Count))));
        Element? previousChild = default!;
        for (var i = 0L; i < checked(childrenLocal.Count); i += 1L)
        {
            Element newChild = inflateWidget(multiChildRenderObjectWidget.children[(int)i], new IndexedSlot<Element?>(i, previousChild));
            childrenLocal[(int)i] = newChild;
            previousChild = newChild;
        }
        _children = childrenLocal;
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (MultiChildRenderObjectWidget)newWidget;
        base.update(__newWidget);
        var multiChildRenderObjectWidget = ((MultiChildRenderObjectWidget?)widget)!;
        DartRuntimePrimitives.Assert(() => Equals(widget, __newWidget));
        DartRuntimePrimitives.Assert(() => !DebugLibrary.debugChildrenHaveDuplicateKeys(widget, multiChildRenderObjectWidget.children.Cast<Widget>()));
        _children = updateChildren(_children, multiChildRenderObjectWidget.children, forgottenChildren: _forgottenChildren);
        _forgottenChildren.Clear();
    }

}

public abstract class RenderTreeRootElement : RenderObjectElement
{
    protected RenderTreeRootElement(RenderObjectWidget widget) : base(widget)
    {
    }

    public override void attachRenderObject(object? newSlot)
    {
        _slot = newSlot;
        DartRuntimePrimitives.Assert(() => _debugCheckMustNotAttachRenderObjectToAncestor());
    }

    public override void detachRenderObject()
    {
        _slot = null;
    }

    public override void updateSlot(object? newSlot)
    {
        base.updateSlot(newSlot);
        DartRuntimePrimitives.Assert(() => _debugCheckMustNotAttachRenderObjectToAncestor());
    }

    internal virtual bool _debugCheckMustNotAttachRenderObjectToAncestor()
    {
        if (!Foundation.ConstantsLibrary.kDebugMode)
        {
            return true;
        }
        if (_findAncestorRenderObjectElement() is not null)
        {
            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The RenderObject for {toStringShort()} cannot maintain an independent render tree at its current location."), new ErrorDescription($"The ownership chain for the RenderObject in question was:\n  {debugGetCreatorChain(10L)}"), new ErrorDescription("This RenderObject is the root of an independent render tree and it cannot " + "attach itself to an ancestor in an existing tree. The ancestor RenderObject, " + "however, expects that a child will be attached."), new ErrorHint($"Try moving the subtree that contains the {toStringShort()} widget " + "to a location where it is not expected to attach its RenderObject " + "to a parent. This could mean moving the subtree into the view " + "property of a \"ViewAnchor\" widget or - if the subtree is the root of " + "your widget tree - passing it to \"runWidget\" instead of \"runApp\"."), new ErrorHint("If you are seeing this error in a test and the subtree containing " + $"the {toStringShort()} widget is passed to \"WidgetTester.pumpWidget\", " + "consider setting the \"wrapWithView\" parameter of that method to false.") }));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DebugCreator
{
    public virtual Element element { get; private set; } = default!;

    public DebugCreator(Element element)
    {
        this.element = element;
    }

    public override string ToString() => element.debugGetCreatorChain(12L);
}

public static partial class FrameworkLibrary
{
    internal static FlutterErrorDetails _reportException(DiagnosticsNode context, object exception, System.Diagnostics.StackTrace? stack, InformationCollector? informationCollector = null)
    {
        var details = new FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: context, informationCollector: informationCollector);
        FlutterError.reportError(details);
        return details;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class IndexedSlot<T> where T : Element?
{
    public virtual T value { get; private set; } = default!;
    public virtual long index { get; private set; } = default!;

    public IndexedSlot(long index, T value)
    {
        this.index = index;
        this.value = value;
    }

    public override bool Equals(object? other)
    {
        var __other = other as IndexedSlot<T>;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is IndexedSlot<T>) && (index == __other.index) && Equals(value, __other.value);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(index, value));
}

internal class _NullElement__framework : Element
{
    public static _NullElement__framework instance = new _NullElement__framework();

    internal _NullElement__framework() : base(new _NullWidget__framework())
    {
    }

    public override bool debugDoingBuild => throw new NotImplementedException();
}

internal class _NullWidget__framework : Widget
{
    internal _NullWidget__framework()
    {
    }

    public override Element createElement() => throw new NotImplementedException();
}
