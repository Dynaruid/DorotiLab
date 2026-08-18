// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/framework.dart
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
    void setState(global::System.Action fn);
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

public class ObjectKey : global::Doroti.Framework.Foundation.LocalKey
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is ObjectKey) && DartRuntimePrimitives.Identical(((ObjectKey)((ObjectKey)__other)).value, this.value));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.GetType(), Dart_coreLibrary.identityHashCode(this.value)));
    public override string ToString()
    {
        if ((object.Equals(this.GetType(), typeof(ObjectKey))))
        {
            return $"[{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this.value))}]";
        }
        return $"[{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ObjectKey"))} {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this.value))}]";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class GlobalKeyBase : global::Doroti.Framework.Foundation.Key
{
    internal virtual Element? _currentElement => WidgetsBinding.instance.buildOwner!._globalKeyRegistry.GetValueOrDefault(this);
}

public class GlobalKey<T> : GlobalKeyBase where T : IState
{
    public static GlobalKey<T> Create(string? debugLabel = null) => new LabeledGlobalKey<T>(debugLabel);

    public GlobalKey()
    {
    }

    public virtual BuildContext? currentContext => DartRuntimePrimitives.ConvertValue<BuildContext>(this._currentElement);
    public virtual Widget? currentWidget => this._currentElement?.widget;
    public virtual T? currentState => (this._currentElement switch { StatefulElement { state: T state__7650 } __object7625 => state__7650, _ => default });
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
        var label__8279 = ((this._debugLabel is not null) ? $" {this._debugLabel}" : "");
        if ((object.Equals(this.GetType(), typeof(LabeledGlobalKey<T>))))
        {
            return $"[GlobalKey#{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.shortHash(this))}{label__8279}]";
        }
        return $"[{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}{label__8279}]";
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is GlobalObjectKey<T>) && DartRuntimePrimitives.Identical(((GlobalObjectKey<T>)((GlobalObjectKey<T>)__other)).value, this.value));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(Dart_coreLibrary.identityHashCode(this.value));
    public override string ToString()
    {
        string selfType__9972 = global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "GlobalObjectKey");
        var suffix__10261 = "<State<StatefulWidget>>";
        if (selfType__9972.endsWith(suffix__10261))
        {
            selfType__9972 = selfType__9972.substring(0L, (selfType__9972.Length - suffix__10261.Length));
        }
        return $"[{selfType__9972} {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this.value))}]";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class Widget : global::Doroti.Framework.Foundation.DiagnosticableTree
{
    public virtual global::Doroti.Framework.Foundation.Key? key { get; private set; }
    public Widget() { }


    protected Widget(global::Doroti.Framework.Foundation.Key? key = null)
    {
        this.key = key;
    }

    public virtual string toStringShallow(string joiner = ", ", global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = global::Doroti.Framework.Foundation.DiagnosticLevel.debug) => throw new NotSupportedException();
    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = global::Doroti.Framework.Foundation.DiagnosticLevel.debug, long wrapWidth = 65) => throw new NotSupportedException();
    public virtual global::Doroti.Framework.Foundation.DiagnosticsNode toDiagnosticsNode(string? name = null, global::Doroti.Framework.Foundation.DiagnosticsTreeStyle? style = null) => throw new NotSupportedException();
    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren() => throw new NotSupportedException();
    public virtual string ToString(global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = global::Doroti.Framework.Foundation.DiagnosticLevel.info) => throw new NotSupportedException();
    public abstract Element createElement();
    public virtual string toStringShort()
    {
        string type__14140 = global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Widget");
        return ((this.key is null) ? type__14140 : $"{type__14140}-{this.key}");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.defaultDiagnosticsTreeStyle = global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.dense;
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
        return ((object.Equals(DartRuntimePrimitives.RuntimeType(oldWidget), DartRuntimePrimitives.RuntimeType(newWidget))) && (object.Equals(((Widget)oldWidget).key, ((Widget)newWidget).key)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _debugConcreteSubtype(Widget widget)
    {
        return ((widget is StatefulWidget) ? 1L : ((widget is StatelessWidget) ? 2L : 0L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class StatelessWidget : Widget
{
    protected StatelessWidget(global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public override StatelessElement createElement() => new StatelessElement(this);
    public abstract Widget build(BuildContext context);
}

public abstract class StatefulWidget : Widget
{
    protected StatefulWidget(global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
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

public delegate void StateSetter(global::System.Action fn);

public abstract class State<T> : IState, global::Doroti.Framework.Foundation.Diagnosticable where T : StatefulWidget
{
    internal virtual T? _widget { get; set; } = default;
    internal virtual _StateLifecycle__framework _debugLifecycleState { get; set; } = _StateLifecycle__framework.created;
    internal virtual StatefulElement? _element { get; set; } = default;

    public virtual T widget => DartRuntimePrimitives.ConvertValue<T>(this._widget!);
    internal virtual bool _debugTypesAreRight(Widget widget) => (widget is T);
    public virtual BuildContext context
    {
        get
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((this._element is null))
                    {
                        throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("This widget has been unmounted, so the State no longer has a context (and should be considered defunct). \n" + "Consider canceling any active work during \"dispose\" or using the \"mounted\" getter to determine if the State is still active."));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return ((BuildContext)(object?)this._element!);
            return default!;
        }
    }
    public virtual bool mounted => DartRuntimePrimitives.ConvertValue<bool>((this._element is not null));
    public virtual void initState()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._debugLifecycleState, _StateLifecycle__framework.created)));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchCreated("widgets", "State", this));
    }

    public virtual void didUpdateWidget(T oldWidget)
    {
    }

    public virtual void reassemble()
    {
    }

    public virtual void setState(global::System.Action fn)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((object.Equals(this._debugLifecycleState, _StateLifecycle__framework.defunct)))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"setState() called after dispose(): {this}"), new global::Doroti.Framework.Foundation.ErrorDescription("This error happens if you call setState() on a State object for a widget that " + "no longer appears in the widget tree (e.g., whose parent widget no longer " + "includes the widget in its build). This error can occur when code calls " + "setState() from a timer, from an animation callback, or after an " + "asynchronous operation (such as an awaited network request or other " + "Future) completes after the widget has been removed from the tree."), new global::Doroti.Framework.Foundation.ErrorHint("The preferred solution is " + "to cancel the timer or stop listening to the animation in the dispose() " + "callback. Another solution is to check the \"mounted\" property of this " + "object before calling setState() to ensure the object is still in the " + "tree."), new global::Doroti.Framework.Foundation.ErrorHint("This error might indicate a memory leak if setState() is being called " + "because another object is retaining a reference to this State object " + "after it has been removed from the tree. To avoid memory leaks, " + "consider breaking the reference to this object during dispose().") }));
                }
                if (((object.Equals(this._debugLifecycleState, _StateLifecycle__framework.created)) && !this.mounted))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"setState() called in constructor: {this}"), new global::Doroti.Framework.Foundation.ErrorHint("This happens when you call setState() on a State object for a widget that " + "hasn't been inserted into the widget tree yet. It is not necessary to call " + "setState() in the constructor, since the state is already assumed to be dirty " + "when it is initially created.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        object? result__53490 = DartRuntimePrimitives.CaptureVoid(() => fn());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__53490 is Future))
                {
                    Future result__53490__as53542 = (Future)result__53490;
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("setState() callback argument returned a Future."), new global::Doroti.Framework.Foundation.ErrorDescription($"The setState() method on {this} was called with a closure or method that " + "returned a Future. Maybe it is marked as \"async\"."), new global::Doroti.Framework.Foundation.ErrorHint("Instead of performing asynchronous work inside a call to setState(), first " + "execute the work (without updating the widget state), and then synchronously " + "update the state inside a call to setState().") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._element!.markNeedsBuild();
    }

    public virtual void deactivate()
    {
    }

    public virtual void activate()
    {
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._debugLifecycleState, _StateLifecycle__framework.ready)));
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLifecycleState = _StateLifecycle__framework.defunct;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
    }

    public abstract Widget build(BuildContext context);
    public virtual void didChangeDependencies()
    {
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                properties.add(new global::Doroti.Framework.Foundation.EnumProperty<_StateLifecycle__framework>("lifecycle state", this._debugLifecycleState, defaultValue: _StateLifecycle__framework.ready));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<T>("_widget", this._widget, ifNull: "no widget"));
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<StatefulElement>("_element", this._element, ifNull: "not mounted"));
    }

    StatefulWidget? IState._widget { get => _widget; set => _widget = (T?)value; }
    _StateLifecycle__framework IState._debugLifecycleState { get => _debugLifecycleState; set => _debugLifecycleState = value; }
    StatefulElement? IState._element { get => _element; set => _element = value; }
    StatefulWidget IState.widget => widget;
    void IState.didUpdateWidget(StatefulWidget oldWidget) => didUpdateWidget((T)oldWidget);
    public virtual void didChangeAppLifecycleState(AppLifecycleState state) { }
    public virtual void didChangeAccessibilityFeatures() { }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class ProxyWidget : Widget
{
    public virtual Widget child { get; private set; } = default!;

    protected ProxyWidget(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

}

public abstract class ParentDataWidget<T> : ProxyWidget
{
    protected ParentDataWidget(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public override ParentDataElement<T> createElement() => new ParentDataElement<T>(this);
    public virtual bool debugIsValidRenderObject(global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(typeof(T), typeof(object))));
        DartRuntimePrimitives.Assert(() => (!object.Equals(typeof(T), typeof(global::Doroti.Framework.Rendering.ParentData))));
        return (((global::Doroti.Framework.Rendering.RenderObject)renderObject).parentData is T);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Type debugTypicalAncestorWidgetClass { get; }
    public virtual string debugTypicalAncestorWidgetDescription => $"{this.debugTypicalAncestorWidgetClass}";
    internal virtual IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode> _debugDescribeIncorrectParentDataType(global::Doroti.Framework.Rendering.ParentData? parentData, RenderObjectWidget? parentDataCreator = null, global::Doroti.Framework.Foundation.DiagnosticsNode? ownershipChain = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(typeof(T), typeof(object))));
        DartRuntimePrimitives.Assert(() => (!object.Equals(typeof(T), typeof(global::Doroti.Framework.Rendering.ParentData))));
        var description__72780 = $"The ParentDataWidget {this} wants to apply ParentData of type {typeof(T)} to a RenderObject";
        return ((IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode>)(object?)new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorHint($"Usually, this means that the {this.GetType()} widget has the wrong ancestor RenderObjectWidget. " + $"Typically, {this.GetType()} widgets are placed directly inside {this.debugTypicalAncestorWidgetDescription} widgets.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract void applyParentData(global::Doroti.Framework.Rendering.RenderObject renderObject);
    public virtual bool debugCanApplyOutOfTurn() => false;
}

public abstract class InheritedWidget : ProxyWidget
{
    protected InheritedWidget(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public override InheritedElement createElement() => new InheritedElement(this);
    public abstract bool updateShouldNotify(InheritedWidget oldWidget);
    protected InheritedWidget(Widget child) : this(null, child) { }
}

public abstract class RenderObjectWidget : Widget
{
    protected RenderObjectWidget(global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public abstract override RenderObjectElement createElement();
    public abstract global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context);
    public virtual void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
    }

    public virtual void didUnmountRenderObject(global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
    }

}

public abstract class LeafRenderObjectWidget : RenderObjectWidget
{
    protected LeafRenderObjectWidget(global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public override LeafRenderObjectElement createElement() => new LeafRenderObjectElement(this);
}

public abstract class SingleChildRenderObjectWidget : RenderObjectWidget
{
    public virtual Widget? child { get; private set; }

    protected SingleChildRenderObjectWidget(global::Doroti.Framework.Foundation.Key? key = null, Widget? child = null) : base(key: key)
    {
        this.child = child;
    }

    public override SingleChildRenderObjectElement createElement() => new SingleChildRenderObjectElement(this);
}

public abstract class MultiChildRenderObjectWidget : RenderObjectWidget
{
    public virtual List<Widget> children { get; private set; } = default!;

    protected MultiChildRenderObjectWidget(global::Doroti.Framework.Foundation.Key? key = null, List<Widget> children = default!) : base(key: key)
    {
        List<Widget> __children = children ?? new List<Widget>();
        this.children = __children;
    }

    public override MultiChildRenderObjectElement createElement() => new MultiChildRenderObjectElement(this);
    protected MultiChildRenderObjectWidget(global::Doroti.Framework.Foundation.Key? key = null, IEnumerable<Widget> children = default!) : this(key, children.ToList()) { }
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
        DartRuntimePrimitives.Assert(() => (object.Equals(((Element)element)._lifecycleState, _ElementLifecycle__framework.inactive)));
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Widgets.DebugLibrary.debugPrintGlobalKeyedWidgetLifecycle)
                {
                    if ((((Element)element).widget.key is GlobalKeyBase))
                    {
                        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"Discarding {element} from inactive elements list.");
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        element.visitChildren(((global::System.Action<Element>)((child) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(((Element)child)._parent, element)));
            _InactiveElements__framework._unmount(child);
        })));
        element.unmount();
        DartRuntimePrimitives.Assert(() => (object.Equals(((Element)element)._lifecycleState, _ElementLifecycle__framework.defunct)));
    }

    internal virtual void _unmountAll()
    {
        _locked = true;
        List<Element> elements__91401 = ((Func<List<Element>>)(() =>
{
    var __cascade = this._elements.ToList();
    __cascade.sort(Element._sort);
    return __cascade;
}))().ToList();
        this._elements.Clear();
        try
        {
            System.Linq.Enumerable.Reverse(elements__91401).forEach((__arg0) => ((global::System.Action<Element>)_unmount)(__arg0));
        }
        finally
        {
            DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._elements));
            _locked = false;
        }
    }

    internal static void _deactivateRecursively(Element element)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((Element)element)._lifecycleState, _ElementLifecycle__framework.active)));
        try
        {
            element.deactivate();
        }
        catch
        {
            Element._deactivateFailedSubtreeRecursively(element);
            throw;
        }
        element.visitChildren((global::System.Action<Element>)_deactivateRecursively);
        DartRuntimePrimitives.Assert(() =>
            {
                element.debugDeactivated();
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual void add(Element element)
    {
        DartRuntimePrimitives.Assert(() => !this._locked);
        DartRuntimePrimitives.Assert(() => !this._elements.Contains(element));
        DartRuntimePrimitives.Assert(() => (((Element)element)._parent is null));
        switch (((Element)element)._lifecycleState)
        {
            case _ElementLifecycle__framework.active:
                {
                    _InactiveElements__framework._deactivateRecursively(element);
                    this._elements.Add(element);
                    break;
                }
            case _ElementLifecycle__framework.inactive:
                {
                    this._elements.Add(element);
                    break;
                }
            case _ElementLifecycle__framework.initial or _ElementLifecycle__framework.failed or _ElementLifecycle__framework.defunct:
                {
                    DartRuntimePrimitives.Assert(() => false, () => (object?)$"{element} must not be deactivated when in {((Element)element)._lifecycleState} state.");
                    break;
                }
        }
    }

    public virtual void remove(Element element)
    {
        DartRuntimePrimitives.Assert(() => !this._locked);
        DartRuntimePrimitives.Assert(() => this._elements.Contains(element));
        DartRuntimePrimitives.Assert(() => (((Element)element)._parent is null));
        this._elements.Remove(element);
        DartRuntimePrimitives.Assert(() => (object.Equals(((Element)element)._lifecycleState, _ElementLifecycle__framework.inactive)));
    }

    public virtual bool debugContains(Element element)
    {
        bool result__92966 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                result__92966 = this._elements.Contains(element);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__92966;
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
    public global::Doroti.Framework.Rendering.RenderObject? findRenderObject();
    public global::Doroti.Ui.Size? size { get; }
    public InheritedWidget dependOnInheritedElement(InheritedElement ancestor, object? aspect = null);
    public T? dependOnInheritedWidgetOfExactType<T>(object? aspect = null);
    public T? getInheritedWidgetOfExactType<T>();
    public InheritedElement? getElementForInheritedWidgetOfExactType<T>();
    public T? findAncestorWidgetOfExactType<T>();
    public T? findAncestorStateOfType<T>();
    public T? findRootAncestorStateOfType<T>();
    public T? findAncestorRenderObjectOfType<T>();
    public void visitAncestorElements(global::System.Func<Element, bool> visitor);
    public void visitChildElements(global::System.Action<Element> visitor);
    public void dispatchNotification(Notification notification);
    public global::Doroti.Framework.Foundation.DiagnosticsNode describeElement(string name, global::Doroti.Framework.Foundation.DiagnosticsTreeStyle style = global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty);
    public global::Doroti.Framework.Foundation.DiagnosticsNode describeWidget(string name, global::Doroti.Framework.Foundation.DiagnosticsTreeStyle style = global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty);
    public List<global::Doroti.Framework.Foundation.DiagnosticsNode> describeMissingAncestor(Type expectedAncestorType);
    public global::Doroti.Framework.Foundation.DiagnosticsNode describeOwnershipChain(string name);
}

public class BuildScope
{
    internal virtual bool _buildScheduled { get; set; } = false;
    internal virtual bool _building { get; set; } = false;
    public virtual global::System.Action? scheduleRebuild { get; private set; }
    internal virtual bool? _dirtyElementsNeedsResorting { get; set; } = default;
    internal virtual List<Element> _dirtyElements { get; private set; } = new List<Element>();

    public BuildScope(global::System.Action? scheduleRebuild = null)
    {
        this.scheduleRebuild = scheduleRebuild;
    }

    internal virtual void _scheduleBuildFor(Element element)
    {
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(((Element)element).buildScope, this));
        if (!((Element)element)._inDirtyList)
        {
            this._dirtyElements.Add(element);
            element._inDirtyList = true;
        }
        if ((!this._buildScheduled && !this._building))
        {
            _buildScheduled = true;
            this.scheduleRebuild?.Invoke();
        }
        if ((this._dirtyElementsNeedsResorting is not null))
        {
            _dirtyElementsNeedsResorting = true;
        }
    }

    internal virtual void _tryRebuild(Element element)
    {
        DartRuntimePrimitives.Assert(() => ((Element)element)._inDirtyList);
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(((Element)element).buildScope, this));
        bool isTimelineTracked__120372 = (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && FrameworkLibrary._isProfileBuildsEnabledFor(((Element)element).widget));
        if (isTimelineTracked__120372)
        {
            DartMap<string, string>? debugTimelineArguments__120509 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode && global::Doroti.Framework.Widgets.DebugLibrary.debugEnhanceBuildTimelineArguments))
                    {
                        debugTimelineArguments__120509 = ((Diagnosticable)((Element)element).widget).toDiagnosticsNode().toTimelineArguments();
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            FlutterTimeline.startSync($"{DartRuntimePrimitives.RuntimeType(((Element)element).widget)}", arguments: debugTimelineArguments__120509?.cast<string, object>());
        }
        try
        {
            element.rebuild();
        }
        catch (Exception e__120906)
        {
            var stack__120909 = new System.Diagnostics.StackTrace();
            FrameworkLibrary._reportException(new global::Doroti.Framework.Foundation.ErrorDescription("while rebuilding dirty elements"), e__120906, stack__120909, informationCollector: (() => new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { element.describeElement("The element being rebuilt at the time was") }));
        }
        if (isTimelineTracked__120372)
        {
            FlutterTimeline.finishSync();
        }
    }

    internal virtual bool _debugAssertElementInScope(Element element, Element debugBuildRoot)
    {
        bool isInScope__121432 = (element._debugIsDescendantOf(debugBuildRoot) || !((Element)element).debugIsActive);
        if (isInScope__121432)
        {
            return true;
        }
        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Tried to build dirty widget in the wrong build scope."), new global::Doroti.Framework.Foundation.ErrorDescription("A widget which was marked as dirty and is still active was scheduled to be built, " + "but the current build scope unexpectedly does not contain that widget."), new global::Doroti.Framework.Foundation.ErrorHint("Sometimes this is detected when an element is removed from the widget tree, but the " + "element somehow did not get marked as inactive. In that case, it might be caused by " + "an ancestor element failing to implement visitChildren correctly, thus preventing " + "some or all of its descendants from being correctly deactivated."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<Element>("The root of the build scope was", debugBuildRoot, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.DiagnosticsProperty<Element>("The offending element (which does not appear to be a descendant of the root of the build scope) was", element, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _flushDirtyElements(Element debugBuildRoot)
    {
        DartRuntimePrimitives.Assert(() => (this._dirtyElementsNeedsResorting is null), () => (object?)"_flushDirtyElements must be non-reentrant");
        this._dirtyElements.sort(Element._sort);
        _dirtyElementsNeedsResorting = false;
        try
        {
            for (var index__122995 = 0L; (index__122995 < checked((long)(this._dirtyElements.Count))); index__122995 = _dirtyElementIndexAfter(index__122995))
            {
                Element element__123101 = this._dirtyElements[(int)(index__122995)];
                if (DartRuntimePrimitives.Identical(((Element)element__123101).buildScope, this))
                {
                    DartRuntimePrimitives.Assert(() => _debugAssertElementInScope(element__123101, debugBuildRoot));
                    _tryRebuild(element__123101);
                }
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    IEnumerable<Element> missedElements__123356 = this._dirtyElements.where(((element) => ((((Element)element).debugIsActive && ((Element)element).dirty) && DartRuntimePrimitives.Identical(((Element)element).buildScope, this))));
                    if (System.Linq.Enumerable.Any(missedElements__123356))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("buildScope missed some dirty elements."), new global::Doroti.Framework.Foundation.ErrorHint("This probably indicates that the dirty list should have been resorted but was not."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<Element>("The context argument of the buildScope call was", debugBuildRoot, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), Element.describeElements("The list of missed elements at the end of the buildScope call was", missedElements__123356.Cast<Element>()) }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        finally
        {
            foreach (Element element__124308 in this._dirtyElements)
            {
                if (DartRuntimePrimitives.Identical(((Element)element__124308).buildScope, this))
                {
                    element__124308._inDirtyList = false;
                }
            }
            this._dirtyElements.Clear();
            _dirtyElementsNeedsResorting = null;
            _buildScheduled = false;
        }
    }

    internal virtual long _dirtyElementIndexAfter(long index)
    {
        if (!DartRuntimePrimitives.RequireValue(this._dirtyElementsNeedsResorting))
        {
            return (index + 1L);
        }
        index += 1L;
        this._dirtyElements.sort(Element._sort);
        _dirtyElementsNeedsResorting = false;
        while (((index > 0L) && this._dirtyElements[(int)((index - 1L))].dirty))
        {
            index -= 1L;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                for (long i__125572 = (index - 1L); (i__125572 >= 0L); i__125572 -= 1L)
                {
                    Element element__125627 = this._dirtyElements[(int)(i__125572)];
                    DartRuntimePrimitives.Assert(() => (!((Element)element__125627).dirty || (!object.Equals(((Element)element__125627)._lifecycleState, _ElementLifecycle__framework.active))));
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
    public virtual global::System.Action? onBuildScheduled { get; set; } = default;
    internal virtual _InactiveElements__framework _inactiveElements { get; private set; } = new _InactiveElements__framework();
    internal virtual bool _scheduledFlushDirtyElements { get; set; } = false;
    public virtual FocusManager focusManager { get; set; } = default!;
    internal virtual long _debugStateLockLevel { get; set; } = 0L;
    internal virtual bool _debugBuilding { get; set; } = false;
    internal virtual Element? _debugCurrentBuildTarget { get; set; } = default;
    internal virtual DartMap<Element, HashSet<GlobalKeyBase>>? _debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans { get; set; } = default;
    internal virtual DartMap<GlobalKeyBase, Element> _globalKeyRegistry { get; private set; } = new DartMap<GlobalKeyBase, Element>();
    internal virtual HashSet<Element>? _debugIllFatedElements { get; private set; } = (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? new HashSet<Element>() : null);
    internal virtual DartMap<Element, DartMap<Element, GlobalKeyBase>>? _debugGlobalKeyReservations { get; private set; } = (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? new DartMap<Element, DartMap<Element, GlobalKeyBase>>() : null);

    public BuildOwner(global::System.Action? onBuildScheduled = null, FocusManager? focusManager = null)
    {
        this.onBuildScheduled = onBuildScheduled;
        this.focusManager = (focusManager ?? (((Func<FocusManager>)(() =>
{
    var __cascade = new FocusManager();
    __cascade.registerGlobalHandlers();
    return __cascade;
}))()));
    }

    public virtual void scheduleBuildFor(Element element)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((Element)element).owner, this)));
        DartRuntimePrimitives.Assert(() => (((Element)element)._parentBuildScope is not null));
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Widgets.DebugLibrary.debugPrintScheduleBuildForStacks)
                {
                    global::Doroti.Framework.Foundation.AssertionsLibrary.debugPrintStack(label: $"scheduleBuildFor() called for {element}{(((Element)element).buildScope._dirtyElements.Contains(element) ? " (ALREADY IN LIST)" : "")}");
                }
                if (!((Element)element).dirty)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("scheduleBuildFor() called for a widget that is not marked as dirty."), element.describeElement("The method was called for the following element"), new global::Doroti.Framework.Foundation.ErrorDescription("This element is not current marked as dirty. Make sure to set the dirty flag before " + "calling scheduleBuildFor()."), new global::Doroti.Framework.Foundation.ErrorHint("If you did not attempt to call scheduleBuildFor() yourself, then this probably " + "indicates a bug in the widgets framework. Please report it:\n" + "  https://github.com/flutter/flutter/issues/new?template=02_bug.yml") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        BuildScope buildScope__129576 = ((Element)element).buildScope;
        DartRuntimePrimitives.Assert(() =>
            {
                if ((global::Doroti.Framework.Widgets.DebugLibrary.debugPrintScheduleBuildForStacks && ((Element)element)._inDirtyList))
                {
                    global::Doroti.Framework.Foundation.AssertionsLibrary.debugPrintStack(label: "BuildOwner.scheduleBuildFor() called; " + $"_dirtyElementsNeedsResorting was {((BuildScope)buildScope__129576)._dirtyElementsNeedsResorting} (now true); " + $"The dirty list for the current build scope is: {((BuildScope)buildScope__129576)._dirtyElements}");
                }
                if ((!this._debugBuilding && ((Element)element)._inDirtyList))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("BuildOwner.scheduleBuildFor() called inappropriately."), new global::Doroti.Framework.Foundation.ErrorHint("The BuildOwner.scheduleBuildFor() method called on an Element " + "that is already in the dirty list."), element.describeElement("the dirty Element was") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if ((!this._scheduledFlushDirtyElements && (this.onBuildScheduled is not null)))
        {
            _scheduledFlushDirtyElements = true;
            this.onBuildScheduled!();
        }
        buildScope__129576._scheduleBuildFor(element);
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Widgets.DebugLibrary.debugPrintScheduleBuildForStacks)
                {
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"...the build scope's dirty list is now: {((BuildScope)buildScope__129576)._dirtyElements}");
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual bool _debugStateLocked => DartRuntimePrimitives.ConvertValue<bool>((this._debugStateLockLevel > 0L));
    public virtual bool debugBuilding => this._debugBuilding;
    public virtual void lockState(global::System.Action callback)
    {
        DartRuntimePrimitives.Assert(() => (this._debugStateLockLevel >= 0L));
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
        DartRuntimePrimitives.Assert(() => (this._debugStateLockLevel >= 0L));
    }

    public virtual void buildScope(Element context, global::System.Action? callback = null)
    {
        BuildScope buildScope__133142 = ((Element)context).buildScope;
        if (((callback is null) && !System.Linq.Enumerable.Any(((BuildScope)buildScope__133142)._dirtyElements)))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (this._debugStateLockLevel >= 0L));
        DartRuntimePrimitives.Assert(() => !this._debugBuilding);
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Widgets.DebugLibrary.debugPrintBuildScope)
                {
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"buildScope called with context {context}; " + $"its build scope's dirty list is: {((BuildScope)buildScope__133142)._dirtyElements}");
                }
                _debugStateLockLevel += 1L;
                _debugBuilding = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            DartMap<string, string>? debugTimelineArguments__133689 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Framework.Widgets.DebugLibrary.debugEnhanceBuildTimelineArguments)
                    {
                        debugTimelineArguments__133689 = new DartMap<string, string> { ["build scope dirty count"] = $"{checked((long)(((BuildScope)buildScope__133142)._dirtyElements.Count))}", ["build scope dirty list"] = $"{((BuildScope)buildScope__133142)._dirtyElements}", ["lock level"] = $"{this._debugStateLockLevel}", ["scope context"] = $"{context}" }.cast<string, string>();
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            FlutterTimeline.startSync("BUILD", arguments: debugTimelineArguments__133689?.cast<string, object>());
        }
        try
        {
            _scheduledFlushDirtyElements = true;
            buildScope__133142._building = true;
            if ((callback is not null))
            {
                DartRuntimePrimitives.Assert(() => this._debugStateLocked);
                Element? debugPreviousBuildTarget__134383 = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        debugPreviousBuildTarget__134383 = this._debugCurrentBuildTarget;
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
                            DartRuntimePrimitives.Assert(() => (object.Equals(this._debugCurrentBuildTarget, context)));
                            _debugCurrentBuildTarget = debugPreviousBuildTarget__134383;
                            _debugElementWasRebuilt(context);
                            return true;
                            throw new InvalidOperationException("Dart closure completed without a value.");
                        });
                }
            }
            buildScope__133142._flushDirtyElements(debugBuildRoot: context);
        }
        finally
        {
            buildScope__133142._building = false;
            _scheduledFlushDirtyElements = false;
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
            DartRuntimePrimitives.Assert(() => this._debugBuilding);
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugBuilding = false;
                    _debugStateLockLevel -= 1L;
                    if (global::Doroti.Framework.Widgets.DebugLibrary.debugPrintBuildScope)
                    {
                        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint("buildScope finished");
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        DartRuntimePrimitives.Assert(() => (this._debugStateLockLevel >= 0L));
    }

    internal virtual void _debugTrackElementThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans(Element node, GlobalKeyBase key)
    {
        DartMap<Element, HashSet<GlobalKeyBase>> map__135655 = _debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans ??= new DartMap<Element, HashSet<GlobalKeyBase>>();
        HashSet<GlobalKeyBase> keys__135809 = map__135655.putIfAbsent(node, (() => new HashSet<GlobalKeyBase>()));
        keys__135809.Add(key);
    }

    internal virtual void _debugElementWasRebuilt(Element node)
    {
        this._debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans?.remove(node);
    }

    public virtual long globalKeyCount => checked((long)(this._globalKeyRegistry.Count));
    internal virtual void _debugRemoveGlobalKeyReservationFor(Element parent, Element child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugGlobalKeyReservations.GetValueOrDefault(parent)?.remove(child);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual void _registerGlobalKey(GlobalKeyBase key, Element element)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (this._globalKeyRegistry.ContainsKey(key))
                {
                    Element oldElement__137410 = this._globalKeyRegistry.GetValueOrDefault(key)!;
                    DartRuntimePrimitives.Assert(() => (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)element).widget), DartRuntimePrimitives.RuntimeType(((Element)oldElement__137410).widget))));
                    this._debugIllFatedElements?.Add(oldElement__137410);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._globalKeyRegistry[key] = element;
    }

    internal virtual void _unregisterGlobalKey(GlobalKeyBase key, Element element)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._globalKeyRegistry.ContainsKey(key) && (!object.Equals(this._globalKeyRegistry.GetValueOrDefault(key), element))))
                {
                    Element oldElement__137843 = this._globalKeyRegistry.GetValueOrDefault(key)!;
                    DartRuntimePrimitives.Assert(() => (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)element).widget), DartRuntimePrimitives.RuntimeType(((Element)oldElement__137843).widget))));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if ((object.Equals(this._globalKeyRegistry.GetValueOrDefault(key), element)))
        {
            this._globalKeyRegistry.remove(key);
        }
    }

    internal virtual void _debugReserveGlobalKeyFor(Element parent, Element child, GlobalKeyBase key)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugGlobalKeyReservations.putIfAbsent(parent, () => new DartMap<Element, GlobalKeyBase>());
                this._debugGlobalKeyReservations[parent]![child] = key;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual void _debugVerifyGlobalKeyReservation()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var keyToParent__138423 = new DartMap<GlobalKeyBase, Element>();
                this._debugGlobalKeyReservations?.forEach(((global::System.Action<Element, DartMap<Element, GlobalKeyBase>>)((parent, childToKey) =>
                {
                    if (((object.Equals(((Element)parent)._lifecycleState, _ElementLifecycle__framework.defunct)) || (((bool?)((dynamic)((Element)parent).renderObject)?.attached) == false)))
                    {
                        return;
                    }
                    childToKey.forEach(((global::System.Action<Element, GlobalKeyBase>)((child, key) =>
                    {
                        if ((((Element)child)._parent is null))
                        {
                            return;
                        }
                        if ((keyToParent__138423.ContainsKey(key) && (!object.Equals(keyToParent__138423.GetValueOrDefault(key), parent))))
                        {
                            Element older__139588 = keyToParent__138423.GetValueOrDefault(key)!;
                            var newer__139633 = parent;
                            global::Doroti.Framework.Foundation.FlutterError error__139680 = default!;
                            if ((older__139588.ToString() != newer__139633.ToString()))
                            {
                                error__139680 = new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Multiple widgets used the same GlobalKey."), new global::Doroti.Framework.Foundation.ErrorDescription($"The key {key} was used by multiple widgets. The parents of those widgets were:\n" + $"- {older__139588}\n" + $"- {newer__139633}\n" + "A GlobalKey can only be specified on one widget at a time in the widget tree.") });
                            }
                            else
                            {
                                error__139680 = new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Multiple widgets used the same GlobalKey."), new global::Doroti.Framework.Foundation.ErrorDescription($"The key {key} was used by multiple widgets. The parents of those widgets were " + "different widgets that both had the following description:\n" + $"  {parent}\n" + "A GlobalKey can only be specified on one widget at a time in the widget tree.") });
                            }
                            if ((!object.Equals(((Element)child)._parent, older__139588)))
                            {
                                older__139588.visitChildren(((global::System.Action<Element>)((currentChild) =>
                                {
                                    if ((object.Equals(currentChild, child)))
                                    {
                                        older__139588.forgetChild(child);
                                    }
                                })));
                            }
                            if ((!object.Equals(((Element)child)._parent, newer__139633)))
                            {
                                newer__139633.visitChildren(((global::System.Action<Element>)((currentChild) =>
                                {
                                    if ((object.Equals(currentChild, child)))
                                    {
                                        newer__139633.forgetChild(child);
                                    }
                                })));
                            }
                            throw DartRuntimePrimitives.AsException(error__139680);
                        }
                        else
                        {
                            keyToParent__138423[key] = parent;
                        }
                    })));
                })));
                this._debugGlobalKeyReservations.Clear();
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual void _debugVerifyIllFatedPopulation()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                DartMap<GlobalKeyBase, HashSet<Element>>? duplicates__141809 = default!;
                foreach (Element element__141846 in (this._debugIllFatedElements ?? new HashSet<Element>()))
                {
                    if ((!object.Equals(((Element)element__141846)._lifecycleState, _ElementLifecycle__framework.defunct)))
                    {
                        DartRuntimePrimitives.Assert(() => (((Element)element__141846).widget.key is not null));
                        var key__142034 = ((GlobalKeyBase?)(object?)((Element)element__141846).widget.key!)!;
                        DartRuntimePrimitives.Assert(() => this._globalKeyRegistry.ContainsKey(key__142034));
                        duplicates__141809 ??= new DartMap<GlobalKeyBase, HashSet<Element>>();
                        HashSet<Element> elements__142279 = duplicates__141809.putIfAbsent(key__142034, (() => new HashSet<Element>()));
                        elements__142279.Add(element__141846);
                        elements__142279.Add(this._globalKeyRegistry.GetValueOrDefault(key__142034)!);
                    }
                }
                this._debugIllFatedElements.Clear();
                if ((duplicates__141809 is not null))
                {
                    var information__142524 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
                    information__142524.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Multiple widgets used the same GlobalKey."));
                    foreach (GlobalKeyBase key__142672 in duplicates__141809.Keys)
                    {
                        HashSet<Element> elements__142727 = duplicates__141809.GetValueOrDefault(key__142672)!;
                        information__142524.Add(Element.describeElements($"The key {key__142672} was used by {checked((long)(elements__142727.Count))} widgets", elements__142727));
                    }
                    information__142524.Add(new global::Doroti.Framework.Foundation.ErrorDescription("A GlobalKey can only be specified on one widget at a time in the widget tree."));
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(information__142524));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual void finalizeTree()
    {
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync("FINALIZE TREE");
        }
        try
        {
            lockState(() => ((_InactiveElements__framework)this._inactiveElements)._unmountAll());
            DartRuntimePrimitives.Assert(() =>
                {
                    try
                    {
                        _debugVerifyGlobalKeyReservation();
                        _debugVerifyIllFatedPopulation();
                        if (((this._debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans is { } __items144242 ? System.Linq.Enumerable.Any(__items144242) : (bool?)null) ?? false))
                        {
                            HashSet<GlobalKeyBase> keys__144362 = new HashSet<GlobalKeyBase>();
                            foreach (Element element__144422 in this._debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans!.Keys)
                            {
                                if ((!object.Equals(((Element)element__144422)._lifecycleState, _ElementLifecycle__framework.defunct)))
                                {
                                    keys__144362.UnionWith(this._debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans!.GetValueOrDefault(element__144422)!);
                                }
                            }
                            if (System.Linq.Enumerable.Any(keys__144362))
                            {
                                DartMap<string, long> keyStringCount__144838 = new DartMap<string, long>();
                                foreach (string key__144911 in keys__144362.map<GlobalKeyBase, string>(((key) => key.ToString())))
                                {
                                    if (keyStringCount__144838.ContainsKey(key__144911))
                                    {
                                        keyStringCount__144838.update(key__144911, ((value) => (value + 1L)));
                                    }
                                    else
                                    {
                                        keyStringCount__144838[key__144911] = 1L;
                                    }
                                }
                                var keyLabels__145222 = new List<string>();
                                IEnumerable<Element> elements__145601 = this._debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans!.Keys;
                                DartMap<string, long> elementStringCount__145737 = new DartMap<string, long>();
                                foreach (string element__145814 in elements__145601.map<Element, string>(((element) => element.ToString())))
                                {
                                    if (elementStringCount__145737.ContainsKey(element__145814))
                                    {
                                        elementStringCount__145737.update(element__145814, ((value) => (value + 1L)));
                                    }
                                    else
                                    {
                                        elementStringCount__145737[element__145814] = 1L;
                                    }
                                }
                                var elementLabels__146196 = new List<string>();
                                DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(keyLabels__145222));
                                var the__146629 = ((checked((long)(keys__144362.Count)) == 1L) ? " the" : "");
                                var s__146687 = ((checked((long)(keys__144362.Count)) == 1L) ? "" : "s");
                                var were__146740 = ((checked((long)(keys__144362.Count)) == 1L) ? "was" : "were");
                                var their__146802 = ((checked((long)(keys__144362.Count)) == 1L) ? "its" : "their");
                                var respective__146866 = ((checked((long)(elementLabels__146196.Count)) == 1L) ? "" : " respective");
                                var those__146947 = ((checked((long)(keys__144362.Count)) == 1L) ? "that" : "those");
                                var s2__147012 = ((checked((long)(elementLabels__146196.Count)) == 1L) ? "" : "s");
                                var those2__147075 = ((checked((long)(elementLabels__146196.Count)) == 1L) ? "that" : "those");
                                var they__147150 = ((checked((long)(elementLabels__146196.Count)) == 1L) ? "it" : "they");
                                var think__147220 = ((checked((long)(elementLabels__146196.Count)) == 1L) ? "thinks" : "think");
                                var are__147296 = ((checked((long)(elementLabels__146196.Count)) == 1L) ? "is" : "are");
                                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"Duplicate GlobalKey{s__146687} detected in widget tree."), new global::Doroti.Framework.Foundation.ErrorDescription($"The following GlobalKey{s__146687} {were__146740} specified multiple times in the widget tree. This will lead to " + "parts of the widget tree being truncated unexpectedly, because the second time a key is seen, " + $"the previous instance is moved to the new location. The key{s__146687} {were__146740}:\n" + $"- {string.Join("\n  ", keyLabels__145222)}\n" + $"This was determined by noticing that after{the__146629} widget{s__146687} with the above global key{s__146687} {were__146740} moved " + $"out of {their__146802}{respective__146866} previous parent{s2__147012}, {those2__147075} previous parent{s2__147012} never updated during this frame, meaning " + $"that {they__147150} either did not update at all or updated before the widget{s__146687} {were__146740} moved, in either case " + $"implying that {they__147150} still {think__147220} that {they__147150} should have a child with {those__146947} global key{s__146687}.\n" + $"The specific parent{s2__147012} that did not update after having one or more children forcibly removed " + $"due to GlobalKey reparenting {are__147296}:\n" + $"- {string.Join("\n  ", elementLabels__146196)}" + "\nA GlobalKey can only be specified on one widget at a time in the widget tree.") }));
                            }
                        }
                    }
                    finally
                    {
                        this._debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans?.Clear();
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        catch (Exception e__149151)
        {
            var stack__149154 = new System.Diagnostics.StackTrace();
            FrameworkLibrary._reportException(new global::Doroti.Framework.Foundation.ErrorSummary("while finalizing the widget tree"), e__149151, stack__149154);
        }
    }

    public virtual void reassemble(Element root)
    {
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync("Preparing Hot Reload (widgets)");
        }
        try
        {
            DartRuntimePrimitives.Assert(() => (((Element)root)._parent is null));
            DartRuntimePrimitives.Assert(() => (object.Equals(((Element)root).owner, this)));
            root.reassemble();
        }
        finally
        {
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
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
        if ((this.current?.onNotification(notification) ?? true))
        {
            return;
        }
        this.parent?.dispatchNotification(notification);
    }

}

public static partial class FrameworkLibrary
{
    internal static bool _isProfileBuildsEnabledFor(Widget widget)
    {
        return (global::Doroti.Framework.Widgets.DebugLibrary.debugProfileBuildsEnabled || ((global::Doroti.Framework.Widgets.DebugLibrary.debugProfileBuildsEnabledUserWidgets && global::Doroti.Framework.Widgets.Widget_inspectorLibrary.debugIsWidgetLocalCreation(widget))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class Element : global::Doroti.Framework.Foundation.DiagnosticableTree, BuildContext
{
    internal virtual Element? _parent { get; set; } = default;
    internal virtual _NotificationNode__framework? _notificationTree { get; set; } = default;
    internal virtual object? _slot { get; set; } = default;
    internal virtual long _depth { get; set; } = default!;
    internal virtual Widget? _widget { get; set; } = default;
    internal virtual BuildOwner? _owner { get; set; } = default;
    internal virtual BuildScope? _parentBuildScope { get; set; } = default;
    internal virtual _ElementLifecycle__framework _lifecycleState { get; set; } = _ElementLifecycle__framework.initial;
    internal virtual HashSet<Element>? _debugForgottenChildrenWithGlobalKey { get; private set; } = (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? new HashSet<Element>() : null);
    internal virtual global::Doroti.Framework.Foundation.PersistentHashMap<Type, InheritedElement>? _inheritedElements { get; set; } = default;
    internal virtual HashSet<InheritedElement>? _dependencies { get; set; } = default;
    internal virtual bool _hadUnsatisfiedDependencies { get; set; } = false;
    internal virtual bool _dirty { get; set; } = true;
    internal virtual bool _inDirtyList { get; set; } = false;
    internal virtual bool _debugBuiltOnce { get; set; } = false;

    protected Element(Widget widget)
    {
        this._widget = widget;
    }

    public virtual string toStringShallow(string joiner = ", ", global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = global::Doroti.Framework.Foundation.DiagnosticLevel.debug) => throw new NotSupportedException();
    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = global::Doroti.Framework.Foundation.DiagnosticLevel.debug, long wrapWidth = 65) => throw new NotSupportedException();
    public virtual string ToString(global::Doroti.Framework.Foundation.DiagnosticLevel minLevel = global::Doroti.Framework.Foundation.DiagnosticLevel.info) => throw new NotSupportedException();
    public virtual bool debugDoingBuild => throw new NotSupportedException();
    public override bool Equals(object? other)
    {
        var __other = other as Element;
        if (__other is null) return false;
        return DartRuntimePrimitives.Identical(this, __other);
    }

    public virtual object? slot => this._slot;
    public virtual long depth
    {
        get
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((object.Equals(this._lifecycleState, _ElementLifecycle__framework.initial)))
                    {
                        throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Depth is only available when element has been mounted."));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return this._depth;
            return default!;
        }
    }
    internal static long _sort(Element a, Element b)
    {
        long diff__157156 = (((Element)a).depth - ((Element)b).depth);
        if ((diff__157156 != 0L))
        {
            return diff__157156;
        }
        bool isBDirty__157410 = ((Element)b).dirty;
        if ((((Element)a).dirty != isBDirty__157410))
        {
            return (isBDirty__157410 ? -1L : 1L);
        }
        return 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _debugConcreteSubtype(Element element)
    {
        return ((element is StatefulElement) ? 1L : ((element is StatelessElement) ? 2L : 0L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget widget => DartRuntimePrimitives.ConvertValue<Widget>(this._widget!);
    public virtual bool mounted => DartRuntimePrimitives.ConvertValue<bool>((this._widget is not null));
    public virtual bool debugIsDefunct
    {
        get
        {
            var isDefunct__158916 = false;
            DartRuntimePrimitives.Assert(() =>
                {
                    isDefunct__158916 = (object.Equals(this._lifecycleState, _ElementLifecycle__framework.defunct));
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return isDefunct__158916;
            return default!;
        }
    }
    public virtual bool debugIsActive
    {
        get
        {
            var isActive__159307 = false;
            DartRuntimePrimitives.Assert(() =>
                {
                    isActive__159307 = (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active));
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return isActive__159307;
            return default!;
        }
    }
    public virtual BuildOwner? owner => this._owner;
    public virtual BuildScope buildScope => DartRuntimePrimitives.ConvertValue<BuildScope>(this._parentBuildScope!);
    public virtual void reassemble()
    {
        markNeedsBuild();
        visitChildren(((global::System.Action<Element>)((child) =>
        {
            child.reassemble();
        })));
    }

    internal virtual bool _debugIsDescendantOf(Element target)
    {
        Element? element__162888 = this;
        while (((element__162888 is not null) && (((Element)element__162888).depth > ((Element)target).depth)))
        {
            element__162888 = ((Element)element__162888)._parent;
        }
        return (object.Equals(element__162888, target));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.RenderObject renderObject
    {
        get
        {
            Element? current__163649 = this;
            while ((current__163649 is not null))
            {
                if ((object.Equals(((Element)current__163649)._lifecycleState, _ElementLifecycle__framework.defunct)))
                {
                    break;
                }
                else
                {
                    if ((current__163649 is RenderObjectElement))
                    {
                        RenderObjectElement current__163649__as163793 = (RenderObjectElement)current__163649;
                        return ((RenderObjectElement)((RenderObjectElement)current__163649__as163793)).renderObject;
                    }
                    else
                    {
                        current__163649 = ((Element)current__163649).renderObjectAttachingChild;
                    }
                }
            }
            return ((global::Doroti.Framework.Rendering.RenderObject)(object)null);
            return default!;
        }
    }
    public virtual Element? renderObjectAttachingChild
    {
        get
        {
            Element? next__164695 = default!;
            visitChildren(((global::System.Action<Element>)((child) =>
            {
                DartRuntimePrimitives.Assert(() => (next__164695 is null));
                next__164695 = child;
            })));
            return next__164695;
            return default!;
        }
    }
    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> describeMissingAncestor(Type expectedAncestorType)
    {
        var information__164971 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        var ancestors__165016 = new List<Element>();
        visitAncestorElements(((global::System.Func<Element, bool>)((element) =>
        {
            ancestors__165016.Add(element);
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        information__164971.Add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Element>($"The specific widget that could not find a {expectedAncestorType} ancestor was", this, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty));
        if (System.Linq.Enumerable.Any(ancestors__165016))
        {
            information__164971.Add(Element.describeElements("The ancestors of this widget were", ancestors__165016.Cast<Element>()));
        }
        else
        {
            information__164971.Add(new global::Doroti.Framework.Foundation.ErrorDescription("This widget is the root of the tree, so it has no " + $"ancestors, let alone a \"{expectedAncestorType}\" ancestor."));
        }
        return information__164971;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Foundation.DiagnosticsNode describeElements(string name, IEnumerable<Element> elements)
    {
        return ((global::Doroti.Framework.Foundation.DiagnosticsNode)(object?)new global::Doroti.Framework.Foundation.DiagnosticsBlock(name: name, children: elements.map<Element, global::Doroti.Framework.Foundation.DiagnosticsNode>(((element) => new global::Doroti.Framework.Foundation.DiagnosticsProperty<Element>("", element))).ToList(), allowTruncate: true));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Foundation.DiagnosticsNode describeElement(string name, global::Doroti.Framework.Foundation.DiagnosticsTreeStyle style = global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty)
    {
        return ((global::Doroti.Framework.Foundation.DiagnosticsNode)(object?)new global::Doroti.Framework.Foundation.DiagnosticsProperty<Element>(name, this, style: DartRuntimePrimitives.RequireValue(style)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Foundation.DiagnosticsNode describeWidget(string name, global::Doroti.Framework.Foundation.DiagnosticsTreeStyle style = global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty)
    {
        return ((global::Doroti.Framework.Foundation.DiagnosticsNode)(object?)new global::Doroti.Framework.Foundation.DiagnosticsProperty<Element>(name, this, style: DartRuntimePrimitives.RequireValue(style)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Foundation.DiagnosticsNode describeOwnershipChain(string name)
    {
        return ((global::Doroti.Framework.Foundation.DiagnosticsNode)(object?)new global::Doroti.Framework.Foundation.StringProperty(name, debugGetCreatorChain(10L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void visitChildren(global::System.Action<Element> visitor)
    {
    }

    public virtual void debugVisitOnstageChildren(global::System.Action<Element> visitor) => visitChildren((global::System.Action<Element>)visitor);
    public virtual void visitChildElements(global::System.Action<Element> visitor)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this.owner is null) || !this.owner!._debugStateLocked))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("visitChildElements() called during build."), new global::Doroti.Framework.Foundation.ErrorDescription("The BuildContext.visitChildElements() method can't be called during " + "build because the child list is still being updated at that point, " + "so the children might not be constructed yet, or might be old children " + "that are going to be replaced.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        visitChildren((global::System.Action<Element>)visitor);
    }

    public virtual Element? updateChild(Element? child, Widget? newWidget, object? newSlot)
    {
        if ((newWidget is null))
        {
            if ((child is not null))
            {
                deactivateChild(child);
            }
            return ((Element)(object)null);
        }
        Element newChild__171942 = default!;
        if ((child is not null))
        {
            var hasSameSuperclass__171987 = true;
            DartRuntimePrimitives.Assert(() =>
                {
                    long oldElementClass__173067 = Element._debugConcreteSubtype(child);
                    long newWidgetClass__173141 = Widget._debugConcreteSubtype(newWidget);
                    hasSameSuperclass__171987 = (oldElementClass__173067 == newWidgetClass__173141);
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            if ((hasSameSuperclass__171987 && (object.Equals(((Element)child).widget, newWidget))))
            {
                if ((!object.Equals(((Element)child).slot, newSlot)))
                {
                    updateSlotForChild(child, newSlot);
                }
                newChild__171942 = child;
            }
            else
            {
                if ((hasSameSuperclass__171987 && Widget.canUpdate(((Element)child).widget, newWidget)))
                {
                    if ((!object.Equals(((Element)child).slot, newSlot)))
                    {
                        updateSlotForChild(child, newSlot);
                    }
                    bool isTimelineTracked__173867 = (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && FrameworkLibrary._isProfileBuildsEnabledFor(newWidget));
                    if (isTimelineTracked__173867)
                    {
                        DartMap<string, string>? debugTimelineArguments__174007 = default!;
                        DartRuntimePrimitives.Assert(() =>
                            {
                                if ((global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode && global::Doroti.Framework.Widgets.DebugLibrary.debugEnhanceBuildTimelineArguments))
                                {
                                    debugTimelineArguments__174007 = ((Diagnosticable)newWidget).toDiagnosticsNode().toTimelineArguments();
                                }
                                return true;
                                throw new InvalidOperationException("Dart closure completed without a value.");
                            });
                        FlutterTimeline.startSync($"{DartRuntimePrimitives.RuntimeType(newWidget)}", arguments: debugTimelineArguments__174007?.cast<string, object>());
                    }
                    child.update(newWidget);
                    if (isTimelineTracked__173867)
                    {
                        FlutterTimeline.finishSync();
                    }
                    DartRuntimePrimitives.Assert(() => (object.Equals(((Element)child).widget, newWidget)));
                    DartRuntimePrimitives.Assert(() =>
                        {
                            ((Element)child).owner!._debugElementWasRebuilt(child);
                            return true;
                            throw new InvalidOperationException("Dart closure completed without a value.");
                        });
                    newChild__171942 = child;
                }
                else
                {
                    deactivateChild(child);
                    DartRuntimePrimitives.Assert(() => (((Element)child)._parent is null));
                    newChild__171942 = inflateWidget(newWidget, newSlot);
                }
            }
        }
        else
        {
            newChild__171942 = inflateWidget(newWidget, newSlot);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not null))
                {
                    _debugRemoveGlobalKeyReservation(child);
                }
                global::Doroti.Framework.Foundation.Key? key__175416 = ((Widget)newWidget).key;
                if ((key__175416 is GlobalKeyBase))
                {
                    GlobalKeyBase key__175416__as175447 = (GlobalKeyBase)key__175416;
                    DartRuntimePrimitives.Assert(() => (this.owner is not null));
                    this.owner!._debugReserveGlobalKeyFor(this, newChild__171942, key__175416__as175447);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return newChild__171942;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<Element> updateChildren(List<Element> oldChildren, List<Widget> newWidgets, HashSet<Element>? forgottenChildren = null, List<object>? slots = null)
    {
        DartRuntimePrimitives.Assert(() => ((slots is null) || (checked((long)(newWidgets.Count)) == checked((long)(slots.Count)))));
        Element? replaceWithNullIfForgotten(Element child)
        {
            return (((forgottenChildren?.Contains(child) ?? false)) ? null : child);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        object? slotFor(long newChildIndex, Element? previousChild)
        {
            return ((slots is not null) ? slots[(int)(newChildIndex)] : new IndexedSlot<Element?>(newChildIndex, previousChild));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var newChildrenTop__180927 = 0L;
        var oldChildrenTop__180955 = 0L;
        long newChildrenBottom__180983 = (checked((long)(newWidgets.Count)) - 1L);
        long oldChildrenBottom__181034 = (checked((long)(oldChildren.Count)) - 1L);
        var newChildren__181089 = new List<Element>(System.Linq.Enumerable.Repeat<Element>(_NullElement__framework.instance, checked((int)checked((long)(newWidgets.Count)))));
        Element? previousChild__181181 = default!;
        while ((((oldChildrenTop__180955 <= oldChildrenBottom__181034)) && ((newChildrenTop__180927 <= newChildrenBottom__180983))))
        {
            Element? oldChild__181346 = replaceWithNullIfForgotten(oldChildren[(int)(oldChildrenTop__180955)]);
            Widget newWidget__181433 = newWidgets[(int)(newChildrenTop__180927)];
            DartRuntimePrimitives.Assert(() => ((oldChild__181346 is null) || (object.Equals(((Element)oldChild__181346)._lifecycleState, _ElementLifecycle__framework.active))));
            if (((oldChild__181346 is null) || !Widget.canUpdate(((Element)oldChild__181346).widget, newWidget__181433)))
            {
                break;
            }
            Element newChild__181683 = updateChild(oldChild__181346, newWidget__181433, slotFor(newChildrenTop__180927, previousChild__181181))!;
            DartRuntimePrimitives.Assert(() => (object.Equals(((Element)newChild__181683)._lifecycleState, _ElementLifecycle__framework.active)));
            newChildren__181089[(int)(newChildrenTop__180927)] = newChild__181683;
            previousChild__181181 = newChild__181683;
            newChildrenTop__180927 += 1L;
            oldChildrenTop__180955 += 1L;
        }
        while ((((oldChildrenTop__180955 <= oldChildrenBottom__181034)) && ((newChildrenTop__180927 <= newChildrenBottom__180983))))
        {
            Element? oldChild__182159 = replaceWithNullIfForgotten(oldChildren[(int)(oldChildrenBottom__181034)]);
            Widget newWidget__182249 = newWidgets[(int)(newChildrenBottom__180983)];
            DartRuntimePrimitives.Assert(() => ((oldChild__182159 is null) || (object.Equals(((Element)oldChild__182159)._lifecycleState, _ElementLifecycle__framework.active))));
            if (((oldChild__182159 is null) || !Widget.canUpdate(((Element)oldChild__182159).widget, newWidget__182249)))
            {
                break;
            }
            oldChildrenBottom__181034 -= 1L;
            newChildrenBottom__180983 -= 1L;
        }
        bool haveOldChildren__182620 = (oldChildrenTop__180955 <= oldChildrenBottom__181034);
        DartMap<global::Doroti.Framework.Foundation.Key, Element>? oldKeyedChildren__182698 = default!;
        if (haveOldChildren__182620)
        {
            oldKeyedChildren__182698 = new DartMap<global::Doroti.Framework.Foundation.Key, Element>();
            while ((oldChildrenTop__180955 <= oldChildrenBottom__181034))
            {
                Element? oldChild__182861 = replaceWithNullIfForgotten(oldChildren[(int)(oldChildrenTop__180955)]);
                DartRuntimePrimitives.Assert(() => ((oldChild__182861 is null) || (object.Equals(((Element)oldChild__182861)._lifecycleState, _ElementLifecycle__framework.active))));
                if ((oldChild__182861 is not null))
                {
                    if ((((Element)oldChild__182861).widget.key is not null))
                    {
                        oldKeyedChildren__182698[((Element)oldChild__182861).widget.key!] = oldChild__182861;
                    }
                    else
                    {
                        deactivateChild(oldChild__182861);
                    }
                }
                oldChildrenTop__180955 += 1L;
            }
        }
        while ((newChildrenTop__180927 <= newChildrenBottom__180983))
        {
            Element? oldChild__183386 = default!;
            Widget newWidget__183415 = newWidgets[(int)(newChildrenTop__180927)];
            if (haveOldChildren__182620)
            {
                global::Doroti.Framework.Foundation.Key? key__183503 = ((Widget)newWidget__183415).key;
                if ((key__183503 is not null))
                {
                    oldChild__183386 = oldKeyedChildren__182698!.GetValueOrDefault(key__183503);
                    if ((oldChild__183386 is not null))
                    {
                        if (Widget.canUpdate(((Element)oldChild__183386).widget, newWidget__183415))
                        {
                            oldKeyedChildren__182698.remove(key__183503);
                        }
                        else
                        {
                            oldChild__183386 = null;
                        }
                    }
                }
            }
            DartRuntimePrimitives.Assert(() => ((oldChild__183386 is null) || Widget.canUpdate(((Element)oldChild__183386).widget, newWidget__183415)));
            Element newChild__184116 = updateChild(oldChild__183386, newWidget__183415, slotFor(newChildrenTop__180927, previousChild__181181))!;
            DartRuntimePrimitives.Assert(() => (object.Equals(((Element)newChild__184116)._lifecycleState, _ElementLifecycle__framework.active)));
            DartRuntimePrimitives.Assert(() => (((object.Equals(oldChild__183386, newChild__184116)) || (oldChild__183386 is null)) || (!object.Equals(((Element)oldChild__183386)._lifecycleState, _ElementLifecycle__framework.active))));
            newChildren__181089[(int)(newChildrenTop__180927)] = newChild__184116;
            previousChild__181181 = newChild__184116;
            newChildrenTop__180927 += 1L;
        }
        DartRuntimePrimitives.Assert(() => (oldChildrenTop__180955 == (oldChildrenBottom__181034 + 1L)));
        DartRuntimePrimitives.Assert(() => (newChildrenTop__180927 == (newChildrenBottom__180983 + 1L)));
        DartRuntimePrimitives.Assert(() => ((checked((long)(newWidgets.Count)) - newChildrenTop__180927) == (checked((long)(oldChildren.Count)) - oldChildrenTop__180955)));
        newChildrenBottom__180983 = (checked((long)(newWidgets.Count)) - 1L);
        oldChildrenBottom__181034 = (checked((long)(oldChildren.Count)) - 1L);
        while ((((oldChildrenTop__180955 <= oldChildrenBottom__181034)) && ((newChildrenTop__180927 <= newChildrenBottom__180983))))
        {
            Element oldChild__185045 = oldChildren[(int)(oldChildrenTop__180955)];
            DartRuntimePrimitives.Assert(() => (replaceWithNullIfForgotten(oldChild__185045) is not null));
            DartRuntimePrimitives.Assert(() => (object.Equals(((Element)oldChild__185045)._lifecycleState, _ElementLifecycle__framework.active)));
            Widget newWidget__185232 = newWidgets[(int)(newChildrenTop__180927)];
            DartRuntimePrimitives.Assert(() => Widget.canUpdate(((Element)oldChild__185045).widget, newWidget__185232));
            Element newChild__185352 = updateChild(oldChild__185045, newWidget__185232, slotFor(newChildrenTop__180927, previousChild__181181))!;
            DartRuntimePrimitives.Assert(() => (object.Equals(((Element)newChild__185352)._lifecycleState, _ElementLifecycle__framework.active)));
            DartRuntimePrimitives.Assert(() => ((object.Equals(oldChild__185045, newChild__185352)) || (!object.Equals(((Element)oldChild__185045)._lifecycleState, _ElementLifecycle__framework.active))));
            newChildren__181089[(int)(newChildrenTop__180927)] = newChild__185352;
            previousChild__181181 = newChild__185352;
            newChildrenTop__180927 += 1L;
            oldChildrenTop__180955 += 1L;
        }
        if ((haveOldChildren__182620 && System.Linq.Enumerable.Any(oldKeyedChildren__182698!)))
        {
            foreach (Element oldChild__185923 in oldKeyedChildren__182698.Values)
            {
                if (((forgottenChildren is null) || !forgottenChildren.Contains(oldChild__185923)))
                {
                    deactivateChild(oldChild__185923);
                }
            }
        }
        DartRuntimePrimitives.Assert(() => newChildren__181089.All(((element) => (element is not _NullElement__framework))));
        return newChildren__181089;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void mount(Element? parent, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.initial)), () => (object?)$"This element is no longer in its initial state ({this._lifecycleState.ToString()})");
        DartRuntimePrimitives.Assert(() => (this._parent is null), () => (object?)$"This element already has a parent ({this._parent}) and it shouldn't have one yet.");
        DartRuntimePrimitives.Assert(() => ((parent is null) || (object.Equals(((Element)parent)._lifecycleState, _ElementLifecycle__framework.active))), () => (object?)$"Parent ({parent}) should be null or in the active state ({((Element)parent)._lifecycleState.ToString()})");
        DartRuntimePrimitives.Assert(() => (this.slot is null), () => (object?)$"This element already has a slot ({this.slot}) and it shouldn't");
        _parent = parent;
        _slot = newSlot;
        _lifecycleState = _ElementLifecycle__framework.active;
        _depth = (1L + ((this._parent?.depth ?? 0L)));
        if ((parent is not null))
        {
            _owner = ((Element)parent).owner;
            _parentBuildScope = ((Element)parent).buildScope;
        }
        DartRuntimePrimitives.Assert(() => (this.owner is not null));
        global::Doroti.Framework.Foundation.Key? key__188214 = ((Widget)this.widget).key;
        if ((key__188214 is GlobalKeyBase))
        {
            GlobalKeyBase key__188214__as188240 = (GlobalKeyBase)key__188214;
            this.owner!._registerGlobalKey(key__188214__as188240, this);
        }
        _updateInheritance();
        attachNotificationTree();
    }

    internal virtual void _debugRemoveGlobalKeyReservation(Element child)
    {
        DartRuntimePrimitives.Assert(() => (this.owner is not null));
        this.owner!._debugRemoveGlobalKeyReservationFor(this, child);
    }

    public virtual void update(Widget newWidget)
    {
        DartRuntimePrimitives.Assert(() => (((object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)) && (!object.Equals(newWidget, this.widget))) && Widget.canUpdate(this.widget, newWidget)));
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugForgottenChildrenWithGlobalKey?.forEach((__arg0) => ((global::System.Action<Element>)this._debugRemoveGlobalKeyReservation)(__arg0));
                this._debugForgottenChildrenWithGlobalKey.Clear();
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _widget = newWidget;
    }

    public virtual void updateSlotForChild(Element child, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)));
        DartRuntimePrimitives.Assert(() => (object.Equals(((Element)child)._parent, this)));
        void visit(Element element)
        {
            element.updateSlot(newSlot);
            Element? descendant__190307 = ((Element)element).renderObjectAttachingChild;
            if ((descendant__190307 is not null))
            {
                visit(descendant__190307);
            }
        }
        visit(child);
    }

    public virtual void updateSlot(object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)));
        DartRuntimePrimitives.Assert(() => (this._parent is not null));
        DartRuntimePrimitives.Assert(() => (object.Equals(this._parent!._lifecycleState, _ElementLifecycle__framework.active)));
        _slot = newSlot;
    }

    internal virtual void _updateDepth(long parentDepth)
    {
        long expectedDepth__190884 = (parentDepth + 1L);
        if ((this._depth < expectedDepth__190884))
        {
            _depth = expectedDepth__190884;
            visitChildren(((global::System.Action<Element>)((child) =>
            {
                child._updateDepth(expectedDepth__190884);
            })));
        }
    }

    internal virtual void _updateBuildScopeRecursively()
    {
        if (DartRuntimePrimitives.Identical(this.buildScope, this._parent?.buildScope))
        {
            return;
        }
        _inDirtyList = false;
        _parentBuildScope = this._parent?.buildScope;
        visitChildren(((global::System.Action<Element>)((child) =>
        {
            child._updateBuildScopeRecursively();
        })));
    }

    public virtual void detachRenderObject()
    {
        visitChildren(((global::System.Action<Element>)((child) =>
        {
            child.detachRenderObject();
        })));
        _slot = null;
    }

    public virtual void attachRenderObject(object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => (this.slot is null));
        visitChildren(((global::System.Action<Element>)((child) =>
        {
            child.attachRenderObject(newSlot);
        })));
        _slot = newSlot;
    }

    internal virtual Element? _retakeInactiveElement(GlobalKeyBase key, Widget newWidget)
    {
        Element? element__193149 = key._currentElement;
        if ((element__193149 is null))
        {
            return ((Element)(object)null);
        }
        if (!Widget.canUpdate(((Element)element__193149).widget, newWidget))
        {
            return ((Element)(object)null);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Widgets.DebugLibrary.debugPrintGlobalKeyedWidgetLifecycle)
                {
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"Attempting to take {element__193149} from {(((object?)((Element)element__193149)._parent ?? (object?)"inactive elements list"))} to put in {this}.");
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        Element? parent__193576 = ((Element)element__193149)._parent;
        if ((parent__193576 is not null))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((object.Equals(parent__193576, this)))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("A GlobalKey was used multiple times inside one widget's child list."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<GlobalKeyBase>("The offending GlobalKey was", key), parent__193576.describeElement("The parent of the widgets with that key was"), element__193149.describeElement("The first child to get instantiated with that key became"), new global::Doroti.Framework.Foundation.DiagnosticsProperty<Widget>("The second child that was to get instantiated with that key was", this.widget, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorDescription("A GlobalKey can only be specified on one widget at a time in the widget tree.") }));
                    }
                    ((Element)parent__193576).owner!._debugTrackElementThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans(parent__193576, key);
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            parent__193576.forgetChild(element__193149);
            parent__193576.deactivateChild(element__193149);
        }
        DartRuntimePrimitives.Assert(() => (((Element)element__193149)._parent is null));
        this.owner!._inactiveElements.remove(element__193149);
        return element__193149;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Element inflateWidget(Widget newWidget, object? newSlot)
    {
        bool isTimelineTracked__195828 = (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && FrameworkLibrary._isProfileBuildsEnabledFor(newWidget));
        if (isTimelineTracked__195828)
        {
            DartMap<string, string>? debugTimelineArguments__195960 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode && global::Doroti.Framework.Widgets.DebugLibrary.debugEnhanceBuildTimelineArguments))
                    {
                        debugTimelineArguments__195960 = ((Diagnosticable)newWidget).toDiagnosticsNode().toTimelineArguments();
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            FlutterTimeline.startSync($"{DartRuntimePrimitives.RuntimeType(newWidget)}", arguments: debugTimelineArguments__195960?.cast<string, object>());
        }
        try
        {
            global::Doroti.Framework.Foundation.Key? key__196327 = ((Widget)newWidget).key;
            Element? inactiveChild__196369 = ((key__196327 is GlobalKeyBase globalKey__196327) ? _retakeInactiveElement(globalKey__196327, newWidget) : null);
            Element newChild__196491 = ((inactiveChild__196369 ?? (Element)newWidget.createElement()));
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugCheckForCycles(newChild__196491);
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            try
            {
                if ((inactiveChild__196369 is not null))
                {
                    DartRuntimePrimitives.Assert(() => (((Element)inactiveChild__196369)._parent is null));
                    inactiveChild__196369._activateWithParent(this, newSlot);
                    Element? updatedChild__196820 = ((Element?)(object?)updateChild(inactiveChild__196369, newWidget, newSlot));
                    DartRuntimePrimitives.Assert(() => (object.Equals(inactiveChild__196369, updatedChild__196820)));
                    return updatedChild__196820!;
                }
                else
                {
                    newChild__196491.mount(this, newSlot);
                    DartRuntimePrimitives.Assert(() => (object.Equals(((Element)newChild__196491)._lifecycleState, _ElementLifecycle__framework.active)));
                    return newChild__196491;
                }
            }
            catch
            {
                _deactivateFailedChildSilently(newChild__196491);
                throw;
            }
        }
        finally
        {
            if (isTimelineTracked__195828)
            {
                FlutterTimeline.finishSync();
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _debugCheckForCycles(Element newChild)
    {
        DartRuntimePrimitives.Assert(() => (((Element)newChild)._parent is null));
        DartRuntimePrimitives.Assert(() =>
            {
                var node__197558 = this;
                while ((((Element)node__197558)._parent is not null))
                {
                    node__197558 = ((Element)node__197558)._parent!;
                }
                DartRuntimePrimitives.Assert(() => (!object.Equals(node__197558, newChild)));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual void deactivateChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((Element)child)._parent, this)));
        child._parent = null;
        child.detachRenderObject();
        this.owner!._inactiveElements.add(child);
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Widgets.DebugLibrary.debugPrintGlobalKeyedWidgetLifecycle)
                {
                    if ((((Element)child).widget.key is GlobalKeyBase))
                    {
                        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"Deactivated {child} (keyed child of {this})");
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
            Element._deactivateFailedSubtreeRecursively(child);
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
            element.visitChildren((global::System.Action<Element>)_deactivateFailedSubtreeRecursively);
        }
        catch
        {
        }
    }

    public virtual void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((Element)child).widget.key is GlobalKeyBase))
                {
                    this._debugForgottenChildrenWithGlobalKey?.Add(child);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual void _activateWithParent(Element parent, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.inactive)));
        _parent = parent;
        _owner = ((Element)parent).owner;
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Widgets.DebugLibrary.debugPrintGlobalKeyedWidgetLifecycle)
                {
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"Reactivating {this} (now child of {this._parent}).");
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _updateDepth(this._parent!.depth);
        _updateBuildScopeRecursively();
        Element._activateRecursively(this);
        attachRenderObject(newSlot);
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)));
    }

    internal static void _activateRecursively(Element element)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((Element)element)._lifecycleState, _ElementLifecycle__framework.inactive)));
        element.activate();
        DartRuntimePrimitives.Assert(() => (object.Equals(((Element)element)._lifecycleState, _ElementLifecycle__framework.active)));
        element.visitChildren((global::System.Action<Element>)_activateRecursively);
    }

    public virtual void activate()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.inactive)));
        DartRuntimePrimitives.Assert(() => (this.owner is not null));
        bool hadDependencies__203312 = ((((this._dependencies is { } __items203339 ? System.Linq.Enumerable.Any(__items203339) : (bool?)null) ?? false)) || this._hadUnsatisfiedDependencies);
        _lifecycleState = _ElementLifecycle__framework.active;
        this._dependencies.Clear();
        _hadUnsatisfiedDependencies = false;
        _updateInheritance();
        attachNotificationTree();
        if (this._dirty)
        {
            this.owner!.scheduleBuildFor(this);
        }
        if (hadDependencies__203312)
        {
            didChangeDependencies();
        }
    }

    public virtual void deactivate()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)));
        DartRuntimePrimitives.Assert(() => (this._widget is not null));
        _ensureDeactivated();
    }

    internal virtual void _ensureDeactivated()
    {
        if (this._dependencies is HashSet<InheritedElement> dependencies__205539 && (System.Linq.Enumerable.Any(dependencies__205539)))
        {
            foreach (var dependency__205602 in dependencies__205539)
            {
                dependency__205602.removeDependent(this);
            }
        }
        _inheritedElements = null;
        _lifecycleState = _ElementLifecycle__framework.inactive;
    }

    public virtual void debugDeactivated()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.inactive)));
    }

    public virtual void unmount()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.inactive)));
        DartRuntimePrimitives.Assert(() => (this._widget is not null));
        DartRuntimePrimitives.Assert(() => (this.owner is not null));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        global::Doroti.Framework.Foundation.Key? key__207717 = this._widget?.key;
        if ((key__207717 is GlobalKeyBase))
        {
            GlobalKeyBase key__207717__as207745 = (GlobalKeyBase)key__207717;
            this.owner!._unregisterGlobalKey(key__207717__as207745, this);
        }
        _widget = null;
        _dependencies = null;
        _lifecycleState = _ElementLifecycle__framework.defunct;
    }

    public virtual bool debugExpectsRenderObjectForSlot(object? slot) => true;
    public virtual global::Doroti.Framework.Rendering.RenderObject? findRenderObject()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((!object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get renderObject of inactive element."), new global::Doroti.Framework.Foundation.ErrorDescription("In order for an element to have a valid renderObject, it must be " + "active, which means it is part of the tree.\n" + $"Instead, this element is in the {this._lifecycleState} state.\n" + "If you called this method from a State object, consider guarding " + "it with State.mounted."), describeElement("The findRenderObject() method was called for the following element") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return this.renderObject;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size? size
    {
        get
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((!object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get size of inactive element."), new global::Doroti.Framework.Foundation.ErrorDescription("In order for an element to have a valid size, the element must be " + "active, which means it is part of the tree.\n" + $"Instead, this element is in the {this._lifecycleState} state."), describeElement("The size getter was called for the following element") }));
                    }
                    if (this.owner!._debugBuilding)
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get size during build."), new global::Doroti.Framework.Foundation.ErrorDescription("The size of this render object has not yet been determined because " + "the framework is still in the process of building widgets, which " + "means the render tree for this frame has not yet been determined. " + "The size getter should only be called from paint callbacks or " + "interaction event handlers (e.g. gesture callbacks)."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.ErrorHint("If you need some sizing information during build to decide which " + "widgets to build, consider using a LayoutBuilder widget, which can " + "tell you the layout constraints at a given location in the tree. See " + "<https://api.flutter.dev/flutter/widgets/LayoutBuilder-class.html> " + "for more details."), new global::Doroti.Framework.Foundation.ErrorSpacer(), describeElement("The size getter was called for the following element") }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            global::Doroti.Framework.Rendering.RenderObject? renderObject__212317 = ((global::Doroti.Framework.Rendering.RenderObject?)(object?)findRenderObject());
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((renderObject__212317 is null))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get size without a render object."), new global::Doroti.Framework.Foundation.ErrorHint("In order for an element to have a valid size, the element must have " + "an associated render object. This element does not have an associated " + "render object, which typically means that the size getter was called " + "too early in the pipeline (e.g., during the build phase) before the " + "framework has created the render tree."), describeElement("The size getter was called for the following element") }));
                    }
                    if ((renderObject__212317 is global::Doroti.Framework.Rendering.RenderSliver))
                    {
                        global::Doroti.Framework.Rendering.RenderSliver renderObject__212317__as213062 = (global::Doroti.Framework.Rendering.RenderSliver)renderObject__212317;
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get size from a RenderSliver."), new global::Doroti.Framework.Foundation.ErrorHint("The render object associated with this element is a " + $"{(DartRuntimePrimitives.RuntimeType(((global::Doroti.Framework.Rendering.RenderSliver)renderObject__212317__as213062)))}, which is a subtype of RenderSliver. " + "Slivers do not have a size per se. They have a more elaborate " + "geometry description, which can be accessed by calling " + "findRenderObject and then using the \"geometry\" getter on the " + "resulting object."), describeElement("The size getter was called for the following element"), ((global::Doroti.Framework.Rendering.RenderSliver)renderObject__212317__as213062).describeForError("The associated render sliver was") }));
                    }
                    if ((renderObject__212317 is not global::Doroti.Framework.Rendering.RenderBox))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get size from a render object that is not a RenderBox."), new global::Doroti.Framework.Foundation.ErrorHint("Instead of being a subtype of RenderBox, the render object associated " + $"with this element is a {DartRuntimePrimitives.RuntimeType(renderObject__212317)}. If this type of " + "render object does have a size, consider calling findRenderObject " + "and extracting its size manually."), describeElement("The size getter was called for the following element"), ((global::Doroti.Framework.Foundation.DiagnosticsNode)((dynamic)renderObject__212317).describeForError("The associated render object was")) }));
                    }
                    global::Doroti.Framework.Rendering.RenderBox box__214550 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderBox>(((global::Doroti.Framework.Rendering.RenderBox)renderObject__212317));
                    if (!((global::Doroti.Framework.Rendering.RenderBox)box__214550).hasSize)
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get size from a render object that has not been through layout."), new global::Doroti.Framework.Foundation.ErrorHint("The size of this render object has not yet been determined because " + "this render object has not yet been through layout, which typically " + "means that the size getter was called too early in the pipeline " + "(e.g., during the build phase) before the framework has determined " + "the size and position of the render objects during layout."), describeElement("The size getter was called for the following element"), box__214550.describeForError("The render object from which the size was to be obtained was") }));
                    }
                    if (box__214550.debugNeedsLayout)
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get size from a render object that has been marked dirty for layout."), new global::Doroti.Framework.Foundation.ErrorHint("The size of this render object is ambiguous because this render object has " + "been modified since it was last laid out, which typically means that the size " + "getter was called too early in the pipeline (e.g., during the build phase) " + "before the framework has determined the size and position of the render " + "objects during layout."), describeElement("The size getter was called for the following element"), box__214550.describeForError("The render object from which the size was to be obtained was"), new global::Doroti.Framework.Foundation.ErrorHint("Consider using debugPrintMarkNeedsLayoutStacks to determine why the render " + "object in question is dirty, if you did not expect this.") }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            if ((renderObject__212317 is global::Doroti.Framework.Rendering.RenderBox))
            {
                global::Doroti.Framework.Rendering.RenderBox renderObject__212317__as216465 = (global::Doroti.Framework.Rendering.RenderBox)renderObject__212317;
                return ((global::Doroti.Framework.Rendering.RenderBox)((global::Doroti.Framework.Rendering.RenderBox)renderObject__212317__as216465)).size;
            }
            return ((Size)(object)null);
            return default!;
        }
    }
    internal virtual bool _debugCheckStateIsActiveForAncestorLookup()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((!object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Looking up a deactivated widget's ancestor is unsafe."), new global::Doroti.Framework.Foundation.ErrorDescription("At this point the state of the widget's element tree is no longer " + "stable."), new global::Doroti.Framework.Foundation.ErrorHint("To safely refer to a widget's ancestor in its dispose() method, " + "save a reference to the ancestor by calling dependOnInheritedWidgetOfExactType() " + "in the widget's didChangeDependencies() method.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool doesDependOnInheritedElement(InheritedElement ancestor)
    {
        return (this._dependencies?.Contains(ancestor) ?? false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InheritedWidget dependOnInheritedElement(InheritedElement ancestor, object? aspect = null)
    {
        (_dependencies ??= new HashSet<InheritedElement>()).Add(ancestor);
        ancestor.updateDependencies(this, aspect);
        return ((InheritedWidget?)(object?)ancestor.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? dependOnInheritedWidgetOfExactType<T>(object? aspect = null)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        InheritedElement? ancestor__218135 = this._inheritedElements.GetValueOrDefault(typeof(T));
        if ((ancestor__218135 is not null))
        {
            return ((T?)(object?)dependOnInheritedElement(ancestor__218135, aspect: aspect))!;
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
        return this._inheritedElements.GetValueOrDefault(typeof(T));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void attachNotificationTree()
    {
        _notificationTree = this._parent?._notificationTree;
    }

    internal virtual void _updateInheritance()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)));
        _inheritedElements = this._parent?._inheritedElements ?? global::Doroti.Framework.Foundation.PersistentHashMap<Type, InheritedElement>.CreateEmpty();
    }

    public virtual T? findAncestorWidgetOfExactType<T>()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor__219491 = this._parent;
        while (((ancestor__219491 is not null) && (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor__219491).widget), typeof(T)))))
        {
            ancestor__219491 = ((Element)ancestor__219491)._parent;
        }
        return ((T?)(object?)ancestor__219491?.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? findAncestorStateOfType<T>()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor__219807 = this._parent;
        while ((ancestor__219807 is not null))
        {
            if (((ancestor__219807 is StatefulElement) && (((StatefulElement)((StatefulElement)ancestor__219807)).state is T)))
            {
                StatefulElement ancestor__219807__as219868 = (StatefulElement)ancestor__219807;
                break;
            }
            ancestor__219807 = ((Element)ancestor__219807)._parent;
        }
        var statefulAncestor__219996 = ((StatefulElement?)(object?)ancestor__219807)!;
        return ((T?)(object?)statefulAncestor__219996?.state)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? findRootAncestorStateOfType<T>()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor__220244 = this._parent;
        StatefulElement? statefulAncestor__220285 = default!;
        while ((ancestor__220244 is not null))
        {
            if (((ancestor__220244 is StatefulElement) && (((StatefulElement)((StatefulElement)ancestor__220244)).state is T)))
            {
                StatefulElement ancestor__220244__as220344 = (StatefulElement)ancestor__220244;
                statefulAncestor__220285 = ((StatefulElement)ancestor__220244__as220344);
            }
            ancestor__220244 = ((Element)ancestor__220244)._parent;
        }
        return ((T?)(object?)statefulAncestor__220285?.state)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? findAncestorRenderObjectOfType<T>()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor__220677 = this._parent;
        while ((ancestor__220677 is not null))
        {
            if (((ancestor__220677 is RenderObjectElement) && (((RenderObjectElement)((RenderObjectElement)ancestor__220677)).renderObject is T)))
            {
                RenderObjectElement ancestor__220677__as220738 = (RenderObjectElement)ancestor__220677;
                return ((T?)(object?)((RenderObjectElement)((RenderObjectElement)ancestor__220677__as220738)).renderObject)!;
            }
            ancestor__220677 = ((Element)ancestor__220677)._parent;
        }
        return default;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void visitAncestorElements(global::System.Func<Element, bool> visitor)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor__221065 = this._parent;
        while (((ancestor__221065 is not null) && visitor(ancestor__221065)))
        {
            ancestor__221065 = ((Element)ancestor__221065)._parent;
        }
    }

    public virtual void didChangeDependencies()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)));
        DartRuntimePrimitives.Assert(() => _debugCheckOwnerBuildTargetExists("didChangeDependencies"));
        markNeedsBuild();
    }

    internal virtual bool _debugCheckOwnerBuildTargetExists(string methodName)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this.owner!._debugCurrentBuildTarget is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{methodName} for {DartRuntimePrimitives.RuntimeType(this.widget)} was called at an " + "inappropriate time."), new global::Doroti.Framework.Foundation.ErrorDescription("It may only be called while the widgets are being built."), new global::Doroti.Framework.Foundation.ErrorHint($"A possible cause of this error is when {methodName} is called during " + "one of:\n" + " * network I/O event\n" + " * file I/O event\n" + " * timer\n" + " * microtask (caused by Future.then, async/await, scheduleMicrotask)") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string debugGetCreatorChain(long limit)
    {
        var chain__222912 = new List<string>();
        Element? node__222945 = this;
        while (((checked((long)(chain__222912.Count)) < limit) && (node__222945 is not null)))
        {
            chain__222912.Add(((Diagnosticable)node__222945).toStringShort());
            node__222945 = ((Element)node__222945)._parent;
        }
        if ((node__222945 is not null))
        {
            chain__222912.Add("⋯");
        }
        return string.Join(" ← ", chain__222912);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<Element> debugGetDiagnosticChain()
    {
        var chain__223444 = new List<Element> { this };
        Element? node__223482 = this._parent;
        while ((node__223482 is not null))
        {
            chain__223444.Add(node__223482);
            node__223482 = ((Element)node__223482)._parent;
        }
        return chain__223444;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispatchNotification(Notification notification)
    {
        this._notificationTree?.dispatchNotification(notification);
    }

    public virtual string toStringShort() => DartRuntimePrimitives.ConvertValue<string>((((Diagnosticable)this._widget).toStringShort() ?? $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}(DEFUNCT)"));
    public virtual global::Doroti.Framework.Foundation.DiagnosticsNode toDiagnosticsNode(string? name = null, global::Doroti.Framework.Foundation.DiagnosticsTreeStyle? style = null)
    {
        return ((global::Doroti.Framework.Foundation.DiagnosticsNode)(object?)new _ElementDiagnosticableTreeNode__framework(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.defaultDiagnosticsTreeStyle = global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.dense;
        if ((!object.Equals(this._lifecycleState, _ElementLifecycle__framework.initial)))
        {
            properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<long>("depth", this.depth, ifNull: "no depth"));
        }
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<Widget>("widget", this._widget, ifNull: "no widget"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Foundation.Key>("key", this._widget?.key, showName: false, defaultValue: null, level: global::Doroti.Framework.Foundation.DiagnosticLevel.hidden));
        this._widget?.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("dirty", value: this.dirty, ifTrue: "dirty"));
        HashSet<InheritedElement>? deps__224858 = this._dependencies;
        if (((deps__224858 is not null) && System.Linq.Enumerable.Any(deps__224858)))
        {
            List<InheritedElement> sortedDependencies__224958 = ((Func<List<InheritedElement>>)(() =>
{
    var __cascade = deps__224858.ToList();
    __cascade.sort(((a, b) => ((Diagnosticable)a).toStringShort().CompareTo(((Diagnosticable)b).toStringShort())));
    return __cascade;
}))().ToList();
            List<global::Doroti.Framework.Foundation.DiagnosticsNode> diagnosticsDependencies__225170 = sortedDependencies__224958.map<InheritedElement, global::Doroti.Framework.Foundation.DiagnosticsNode>(((element) => ((Diagnosticable)((InheritedElement)element).widget).toDiagnosticsNode(style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.sparse))).ToList().ToList();
            properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<InheritedElement>>("dependencies", deps__224858, description: diagnosticsDependencies__225170.ToString()));
        }
    }

    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children__225670 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        visitChildren(((global::System.Action<Element>)((child) =>
        {
            children__225670.Add(((Diagnosticable)child).toDiagnosticsNode());
        })));
        return children__225670;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool dirty => this._dirty;
    public virtual void markNeedsBuild()
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._lifecycleState, _ElementLifecycle__framework.defunct)));
        if ((!object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (this.owner is not null));
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)));
        DartRuntimePrimitives.Assert(() =>
            {
                if (this.owner!._debugBuilding)
                {
                    DartRuntimePrimitives.Assert(() => (this.owner!._debugCurrentBuildTarget is not null));
                    DartRuntimePrimitives.Assert(() => this.owner!._debugStateLocked);
                    if (_debugIsDescendantOf(this.owner!._debugCurrentBuildTarget!))
                    {
                        return true;
                    }
                    var information__227480 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("setState() or markNeedsBuild() called during build."), new global::Doroti.Framework.Foundation.ErrorDescription($"This {DartRuntimePrimitives.RuntimeType(this.widget)} widget cannot be marked as needing to build because the framework " + "is already in the process of building widgets. A widget can be marked as " + "needing to be built during the build phase only if one of its ancestors " + "is currently building. This exception is allowed because the framework " + "builds parent widgets before children, which means a dirty descendant " + "will always be built. Otherwise, the framework might not visit this " + "widget during this build phase."), describeElement("The widget on which setState() or markNeedsBuild() was called was") };
                    if ((this.owner!._debugCurrentBuildTarget is not null))
                    {
                        information__227480.Add(this.owner!._debugCurrentBuildTarget!.describeWidget("The widget which was currently being built when the offending call was made was"));
                    }
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(information__227480));
                }
                else
                {
                    if (this.owner!._debugStateLocked)
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("setState() or markNeedsBuild() called when widget tree was locked."), new global::Doroti.Framework.Foundation.ErrorDescription($"This {DartRuntimePrimitives.RuntimeType(this.widget)} widget cannot be marked as needing to build " + "because the framework is locked."), describeElement("The widget on which setState() or markNeedsBuild() was called was") }));
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (this.dirty)
        {
            return;
        }
        _dirty = true;
        this.owner!.scheduleBuildFor(this);
    }

    public virtual void rebuild(bool force = false)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._lifecycleState, _ElementLifecycle__framework.initial)));
        if (((!object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)) || ((!this._dirty && !force))))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Framework.Widgets.DebugLibrary.debugOnRebuildDirtyWidget?.Invoke(this, this._debugBuiltOnce);
                if (global::Doroti.Framework.Widgets.DebugLibrary.debugPrintRebuildDirtyWidgets)
                {
                    if (!this._debugBuiltOnce)
                    {
                        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"Building {this}");
                        _debugBuiltOnce = true;
                    }
                    else
                    {
                        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"Rebuilding {this}");
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)));
        DartRuntimePrimitives.Assert(() => this.owner!._debugStateLocked);
        Element? debugPreviousBuildTarget__235766 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                debugPreviousBuildTarget__235766 = this.owner!._debugCurrentBuildTarget;
                this.owner!._debugCurrentBuildTarget = this;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        try
        {
            performRebuild();
        }
        finally
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    this.owner!._debugElementWasRebuilt(this);
                    DartRuntimePrimitives.Assert(() => (object.Equals(this.owner!._debugCurrentBuildTarget, this)));
                    this.owner!._debugCurrentBuildTarget = debugPreviousBuildTarget__235766;
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        DartRuntimePrimitives.Assert(() => !this._dirty);
    }

    public virtual void performRebuild()
    {
        _dirty = false;
    }

}

internal class _ElementDiagnosticableTreeNode__framework : global::Doroti.Framework.Foundation.DiagnosticableTreeNode<global::Doroti.Framework.Foundation.DiagnosticableTree>
{
    public virtual bool stateful { get; private set; } = default!;

    internal _ElementDiagnosticableTreeNode__framework(string? name = null, Element value = default!, global::Doroti.Framework.Foundation.DiagnosticsTreeStyle? style = default!, bool stateful = false) : base(name: name, value: value, style: DartRuntimePrimitives.RequireValue(style))
    {
        this.stateful = stateful;
    }

    public override DartMap<string, object> toJsonMap(global::Doroti.Framework.Foundation.DiagnosticsSerializationDelegate @delegate)
    {
        DartMap<string, object?> json__236880 = ((DartMap<string, object?>)(object?)base.toJsonMap(@delegate));
        var element__236924 = ((Element?)(object?)this.value)!;
        if (!((Element)element__236924).debugIsDefunct)
        {
            json__236880["widgetRuntimeType"] = DartRuntimePrimitives.RuntimeTypeName(((Element)element__236924).widget);
        }
        json__236880["stateful"] = this.stateful;
        return json__236880;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate Widget ErrorWidgetBuilder(global::Doroti.Framework.Foundation.FlutterErrorDetails details);

public class ErrorWidget : LeafRenderObjectWidget
{
    public static global::System.Func<global::Doroti.Framework.Foundation.FlutterErrorDetails, Widget> builder = _defaultErrorWidgetBuilder;
    public virtual string message { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Foundation.FlutterError? _flutterError { get; private set; }

    public ErrorWidget(object exception) : base(key: new global::Doroti.Framework.Foundation.UniqueKey())
    {
        this.message = ErrorWidget._stringify(exception);
        this._flutterError = ((exception is global::Doroti.Framework.Foundation.FlutterError) ? ((global::Doroti.Framework.Foundation.FlutterError)exception) : null);
    }

    public static ErrorWidget CreateWithDetails(string message = "", global::Doroti.Framework.Foundation.FlutterError? error = null)
    {
        var __instance = new ErrorWidget(default!);
        __instance.message = message;
        __instance._flutterError = error;
        return __instance;
    }

    internal static Widget _defaultErrorWidgetBuilder(global::Doroti.Framework.Foundation.FlutterErrorDetails details)
    {
        var message__241341 = "";
        DartRuntimePrimitives.Assert(() =>
            {
                message__241341 = $"{(ErrorWidget._stringify(((global::Doroti.Framework.Foundation.FlutterErrorDetails)details).exception))}\nSee also: https://docs.flutter.dev/testing/errors";
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        object exception__241530 = ((global::Doroti.Framework.Foundation.FlutterErrorDetails)details).exception;
        return ((Widget)(object?)ErrorWidget.CreateWithDetails(message: message__241341, error: ((exception__241530 is global::Doroti.Framework.Foundation.FlutterError) ? ((global::Doroti.Framework.Foundation.FlutterError)exception__241530) : null)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static string _stringify(object? exception)
    {
        try
        {
            return ((string)((dynamic)exception).ToString());
        }
        catch (Exception error__241798)
        {
        }
        return "Error";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new global::Doroti.Framework.Rendering.RenderErrorBox(this.message));
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        if ((this._flutterError is null))
        {
            properties.add(new global::Doroti.Framework.Foundation.StringProperty("message", this.message, quoted: false));
        }
        else
        {
            properties.add(((Diagnosticable)this._flutterError).toDiagnosticsNode(style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.whitespace));
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

    public virtual bool debugDoingBuild => this._debugDoingBuild;
    public override Element? renderObjectAttachingChild => this._child;
    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        DartRuntimePrimitives.Assert(() => (this._child is null));
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)));
        _firstBuild();
        DartRuntimePrimitives.Assert(() => (this._child is not null));
    }

    internal virtual void _firstBuild()
    {
        rebuild();
    }

    public override void performRebuild()
    {
        Widget built__246655 = default!;
        try
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingBuild = true;
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            built__246655 = build();
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingBuild = false;
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            global::Doroti.Framework.Widgets.DebugLibrary.debugWidgetBuilderValue(this.widget, built__246655);
        }
        catch (Exception e__246923)
        {
            var stack__246926 = new System.Diagnostics.StackTrace();
            _debugDoingBuild = false;
            built__246655 = ErrorWidget.builder(FrameworkLibrary._reportException(new global::Doroti.Framework.Foundation.ErrorDescription($"building {this}"), e__246923, stack__246926, informationCollector: (() => new List<global::Doroti.Framework.Foundation.DiagnosticsNode>())));
        }
        try
        {
            _child = updateChild(this._child, built__246655, this.slot);
            DartRuntimePrimitives.Assert(() => (this._child is not null));
        }
        catch (Exception e__247601)
        {
            var stack__247604 = new System.Diagnostics.StackTrace();
            built__246655 = ErrorWidget.builder(FrameworkLibrary._reportException(new global::Doroti.Framework.Foundation.ErrorDescription($"building {this}"), e__247601, stack__247604, informationCollector: (() => new List<global::Doroti.Framework.Foundation.DiagnosticsNode>())));
            try
            {
                this._child?.deactivate();
            }
            catch
            {
            }
            _child = updateChild(((Element)(object)null), built__246655, this.slot);
        }
        base.performRebuild();
    }

    public abstract Widget build();
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

}

public class StatelessElement : ComponentElement
{
    public StatelessElement(StatelessWidget widget) : base(widget)
    {
    }

    public override Widget build() => (((StatelessWidget?)(object?)this.widget)!).build(this);
    public override void update(Widget newWidget)
    {
        var __newWidget = (StatelessWidget)(object)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        rebuild(force: true);
    }

}

public class StatefulElement : ComponentElement
{
    internal virtual IState? _state { get; set; } = default;
    internal virtual bool _didChangeDependencies { get; set; } = false;

    public StatefulElement(StatefulWidget widget) : base(widget)
    {
        this._state = widget.createState();
        this._state!._element = this;
        this._state!._widget = widget;
    }

    public override Widget build() => this.state.build(this);
    public virtual IState state => DartRuntimePrimitives.ConvertValue<IState>(this._state!);
    public override void reassemble()
    {
        this.state.reassemble();
        base.reassemble();
    }

    internal override void _firstBuild()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this.state._debugLifecycleState, _StateLifecycle__framework.created)));
        object? debugCheckForReturnedFuture__250900 = DartRuntimePrimitives.CaptureVoid(() => this.state.initState());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((debugCheckForReturnedFuture__250900 is Future))
                {
                    Future debugCheckForReturnedFuture__250900__as250986 = (Future)debugCheckForReturnedFuture__250900;
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{DartRuntimePrimitives.RuntimeType(this.state)}.initState() returned a Future."), new global::Doroti.Framework.Foundation.ErrorDescription("State.initState() must be a void method without an `async` keyword."), new global::Doroti.Framework.Foundation.ErrorHint("Rather than awaiting on asynchronous work directly inside of initState, " + "call a separate method to do this work without awaiting it.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                this.state._debugLifecycleState = _StateLifecycle__framework.initialized;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this.state.didChangeDependencies();
        DartRuntimePrimitives.Assert(() =>
            {
                this.state._debugLifecycleState = _StateLifecycle__framework.ready;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        base._firstBuild();
    }

    public override void performRebuild()
    {
        if (this._didChangeDependencies)
        {
            this.state.didChangeDependencies();
            _didChangeDependencies = false;
        }
        base.performRebuild();
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (StatefulWidget)(object)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        StatefulWidget oldWidget__252110 = this.state._widget!;
        this.state._widget = ((StatefulWidget?)(object?)this.widget)!;
        object? debugCheckForReturnedFuture__252202 = DartRuntimePrimitives.CaptureVoid(() => this.state.didUpdateWidget(oldWidget__252110));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((debugCheckForReturnedFuture__252202 is Future))
                {
                    Future debugCheckForReturnedFuture__252202__as252303 = (Future)debugCheckForReturnedFuture__252202;
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{DartRuntimePrimitives.RuntimeType(this.state)}.didUpdateWidget() returned a Future."), new global::Doroti.Framework.Foundation.ErrorDescription("State.didUpdateWidget() must be a void method without an `async` keyword."), new global::Doroti.Framework.Foundation.ErrorHint("Rather than awaiting on asynchronous work directly inside of didUpdateWidget, " + "call a separate method to do this work without awaiting it.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        rebuild(force: true);
    }

    public override void activate()
    {
        base.activate();
        this.state.activate();
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)));
        markNeedsBuild();
    }

    public override void deactivate()
    {
        this.state.deactivate();
        base.deactivate();
    }

    public override void unmount()
    {
        base.unmount();
        this.state.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((object.Equals(this.state._debugLifecycleState, _StateLifecycle__framework.defunct)))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{DartRuntimePrimitives.RuntimeType(this.state)}.dispose failed to call super.dispose."), new global::Doroti.Framework.Foundation.ErrorDescription("dispose() implementations must always call their superclass dispose() method, to ensure " + "that all the resources used by the widget are fully released.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this.state._element = null;
        _state = null;
    }

    public override InheritedWidget dependOnInheritedElement(InheritedElement ancestor, object? aspect = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                Type targetType__254256 = DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget);
                if ((object.Equals(this.state._debugLifecycleState, _StateLifecycle__framework.created)))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"dependOnInheritedWidgetOfExactType<{targetType__254256}>() or dependOnInheritedElement() was called before {DartRuntimePrimitives.RuntimeType(this.state)}.initState() completed."), new global::Doroti.Framework.Foundation.ErrorDescription("When an inherited widget changes, for example if the value of Theme.of() changes, " + "its dependent widgets are rebuilt. If the dependent widget's reference to " + "the inherited widget is in a constructor or an initState() method, " + "then the rebuilt dependent widget will not reflect the changes in the " + "inherited widget."), new global::Doroti.Framework.Foundation.ErrorHint("Typically references to inherited widgets should occur in widget build() methods. Alternatively, " + "initialization based on inherited widgets can be placed in the didChangeDependencies method, which " + "is called after initState and whenever the dependencies change thereafter.") }));
                }
                if ((object.Equals(this.state._debugLifecycleState, _StateLifecycle__framework.defunct)))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"dependOnInheritedWidgetOfExactType<{targetType__254256}>() or dependOnInheritedElement() was called after dispose(): {this}"), new global::Doroti.Framework.Foundation.ErrorDescription("This error happens if you call dependOnInheritedWidgetOfExactType() on the " + "BuildContext for a widget that no longer appears in the widget tree " + "(e.g., whose parent widget no longer includes the widget in its " + "build). This error can occur when code calls " + "dependOnInheritedWidgetOfExactType() from a timer or an animation callback."), new global::Doroti.Framework.Foundation.ErrorHint("The preferred solution is to cancel the timer or stop listening to the " + "animation in the dispose() callback. Another solution is to check the " + "\"mounted\" property of this object before calling " + "dependOnInheritedWidgetOfExactType() to ensure the object is still in the " + "tree."), new global::Doroti.Framework.Foundation.ErrorHint("This error might indicate a memory leak if " + "dependOnInheritedWidgetOfExactType() is being called because another object " + "is retaining a reference to this State object after it has been " + "removed from the tree. To avoid memory leaks, consider breaking the " + "reference to this object during dispose().") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((InheritedWidget)(object?)base.dependOnInheritedElement(((InheritedElement?)(object?)ancestor)!, aspect: aspect));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _didChangeDependencies = true;
    }

    public override global::Doroti.Framework.Foundation.DiagnosticsNode toDiagnosticsNode(string? name = null, global::Doroti.Framework.Foundation.DiagnosticsTreeStyle? style = null)
    {
        return ((global::Doroti.Framework.Foundation.DiagnosticsNode)(object?)new _ElementDiagnosticableTreeNode__framework(name: name, value: this, style: style, stateful: true));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<IState>("state", this._state, defaultValue: null));
    }

}

public abstract class ProxyElement : ComponentElement
{
    protected ProxyElement(ProxyWidget widget) : base(widget)
    {
    }

    public override Widget build() => ((Widget)((dynamic)(((ProxyWidget?)(object?)this.widget)!)).child);
    public override void update(Widget newWidget)
    {
        var __newWidget = (ProxyWidget)(object)newWidget;
        var oldWidget__258554 = ((ProxyWidget?)(object?)this.widget)!;
        DartRuntimePrimitives.Assert(() => (!object.Equals(this.widget, __newWidget)));
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        updated(oldWidget__258554);
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
            Type? type__259898 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    type__259898 = typeof(T);
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            if ((type__259898 is not null))
            {
                return type__259898!;
            }
            throw new NotSupportedException("debugParentDataType is only supported in debug builds");
            return default!;
        }
    }
    internal virtual void _applyParentData(ParentDataWidget<T> widget)
    {
        void applyParentDataToChild(Element child)
        {
            if ((child is RenderObjectElement))
            {
                ((RenderObjectElement)child)._updateParentData(widget);
            }
            else
            {
                if ((((Element)child).renderObjectAttachingChild is not null))
                {
                    applyParentDataToChild(((Element)child).renderObjectAttachingChild!);
                }
            }
        }
        if ((this.renderObjectAttachingChild is not null))
        {
            applyParentDataToChild(this.renderObjectAttachingChild!);
        }
    }

    void IParentDataElement.applyParentDataTo(RenderObjectElement child) =>
        child._updateParentData(((ParentDataWidget<T>)(object)this.widget));

    public virtual void applyWidgetOutOfTurn(ParentDataWidget<T> newWidget)
    {
        DartRuntimePrimitives.Assert(() => newWidget.debugCanApplyOutOfTurn());
        DartRuntimePrimitives.Assert(() => (object.Equals(((Widget)((dynamic)newWidget).child), ((Widget)((dynamic)(((ParentDataWidget<T>?)(object?)this.widget)!)).child))));
        _applyParentData(newWidget);
    }

    public override void notifyClients(ProxyWidget oldWidget)
    {
        var __oldWidget = (ParentDataWidget<T>)(object)oldWidget;
        _applyParentData(((ParentDataWidget<T>?)(object?)this.widget)!);
    }

}

public class InheritedElement : ProxyElement
{
    internal virtual DartMap<Element, object> _dependents { get; private set; } = new DartMap<Element, object>();

    public InheritedElement(InheritedWidget widget) : base(widget)
    {
    }

    internal override void _updateInheritance()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active)));
        global::Doroti.Framework.Foundation.PersistentHashMap<Type, InheritedElement> incomingWidgets__263141 = (this._parent?._inheritedElements ?? global::Doroti.Framework.Foundation.PersistentHashMap<Type, InheritedElement>.CreateEmpty());
        _inheritedElements = incomingWidgets__263141.put(DartRuntimePrimitives.RuntimeType(this.widget), this);
    }

    public override void debugDeactivated()
    {
        DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._dependents));
        base.debugDeactivated();
    }

    public virtual object? getDependencies(Element dependent)
    {
        return this._dependents.GetValueOrDefault(dependent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setDependencies(Element dependent, object? value)
    {
        this._dependents[dependent] = value;
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
        this._dependents.remove(dependent);
    }

    public override void updated(ProxyWidget oldWidget)
    {
        var __oldWidget = (InheritedWidget)(object)oldWidget;
        if ((((InheritedWidget?)(object?)this.widget)!).updateShouldNotify(__oldWidget))
        {
            base.updated(__oldWidget);
        }
    }

    public override void notifyClients(ProxyWidget oldWidget)
    {
        var __oldWidget = (InheritedWidget)(object)oldWidget;
        DartRuntimePrimitives.Assert(() => _debugCheckOwnerBuildTargetExists("notifyClients"));
        foreach (Element dependent__269504 in this._dependents.Keys)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    Element? ancestor__269622 = ((Element)dependent__269504)._parent;
                    while (((!object.Equals(ancestor__269622, this)) && (ancestor__269622 is not null)))
                    {
                        ancestor__269622 = ((Element)ancestor__269622)._parent;
                    }
                    return (object.Equals(ancestor__269622, this));
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            DartRuntimePrimitives.Assert(() => ((Element)dependent__269504)._dependencies!.Contains(this));
            notifyDependent(__oldWidget, dependent__269504);
        }
    }

}

public abstract class RenderObjectElement : Element
{
    internal virtual global::Doroti.Framework.Rendering.RenderObject? _renderObject { get; set; } = default;
    internal virtual bool _debugDoingBuild { get; set; } = false;
    internal virtual RenderObjectElement? _ancestorRenderObjectElement { get; set; } = default;

    protected RenderObjectElement(RenderObjectWidget widget) : base(widget)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject renderObject
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this._renderObject is not null), () => (object?)$"{this.GetType()} unmounted");
            return this._renderObject!;
            return default!;
        }
    }
    public override Element? renderObjectAttachingChild => DartRuntimePrimitives.ConvertValue<Element>(null);
    public virtual bool debugDoingBuild => this._debugDoingBuild;
    internal virtual RenderObjectElement? _findAncestorRenderObjectElement()
    {
        Element? ancestor__279246 = this._parent;
        while (((ancestor__279246 is not null) && (ancestor__279246 is not RenderObjectElement)))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (!ancestor__279246!.debugExpectsRenderObjectForSlot(this.slot))
                    {
                        ancestor__279246 = null;
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            ancestor__279246 = ancestor__279246?._parent;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((ancestor__279246?.debugExpectsRenderObjectForSlot(this.slot) == false))
                {
                    ancestor__279246 = null;
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((RenderObjectElement?)(object?)ancestor__279246)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _debugCheckCompetingAncestors(List<IParentDataElement> result, HashSet<Type> debugAncestorTypes, HashSet<Type> debugParentDataTypes, List<Type> debugAncestorCulprits)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (((checked((long)(debugAncestorTypes.Count)) != checked((long)(result.Count))) || (checked((long)(debugParentDataTypes.Count)) != checked((long)(result.Count)))))
                {
                    DartRuntimePrimitives.Assert(() => ((checked((long)(debugAncestorTypes.Count)) < checked((long)(result.Count))) || (checked((long)(debugParentDataTypes.Count)) < checked((long)(result.Count)))));
                    try
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Incorrect use of ParentDataWidget."), new global::Doroti.Framework.Foundation.ErrorDescription("Competing ParentDataWidgets are providing parent data to the " + "same RenderObject:"), new global::Doroti.Framework.Foundation.ErrorDescription("A RenderObject can receive parent data from multiple " + "ParentDataWidgets, but the Type of ParentData must be unique to " + "prevent one overwriting another."), new global::Doroti.Framework.Foundation.ErrorHint("Usually, this indicates that one or more of the offending " + "ParentDataWidgets listed above isn't placed inside a dedicated " + "compatible ancestor widget that it isn't sharing with another " + "ParentDataWidget of the same type."), new global::Doroti.Framework.Foundation.ErrorHint("Otherwise, separating aspects of ParentData to prevent " + "conflicts can be done using mixins, mixing them all in on the " + "full ParentData Object, such as KeepAlive does with " + "KeepAliveParentDataMixin."), new global::Doroti.Framework.Foundation.ErrorDescription("The ownership chain for the RenderObject that received the " + $"parent data was:\n  {debugGetCreatorChain(10L)}") }));
                    }
                    catch (global::Doroti.Framework.Foundation.FlutterError error__282928)
                    {
                        FrameworkLibrary._reportException(new global::Doroti.Framework.Foundation.ErrorSummary("while looking for parent data."), error__282928, error__282928.stackTrace);
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual List<IParentDataElement> _findAncestorParentDataElements()
    {
        Element? ancestor__283177 = this._parent;
        var result__283207 = new List<IParentDataElement>();
        var debugAncestorTypes__283261 = new HashSet<Type>();
        var debugParentDataTypes__283302 = new HashSet<Type>();
        var debugAncestorCulprits__283345 = new List<Type>();
        while (((ancestor__283177 is not null) && (ancestor__283177 is not RenderObjectElement)))
        {
            if ((ancestor__283177 is IParentDataElement))
            {
                IParentDataElement ancestor__283177__as284599 = (IParentDataElement)ancestor__283177;
                DartRuntimePrimitives.Assert(() =>
                    {
                        IParentDataElement ancestor = ancestor__283177__as284599;
                        if ((!debugAncestorTypes__283261.Add(DartRuntimePrimitives.RuntimeType(ancestor)) || !debugParentDataTypes__283302.Add(ancestor.debugParentDataType)))
                        {
                            debugAncestorCulprits__283345.Add(DartRuntimePrimitives.RuntimeType(ancestor));
                        }
                        return true;
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                result__283207.Add(ancestor__283177__as284599);
            }
            ancestor__283177 = ((Element)ancestor__283177)._parent;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((!System.Linq.Enumerable.Any(result__283207) || (ancestor__283177 is null)))
                {
                    return true;
                }
                _debugCheckCompetingAncestors(result__283207, debugAncestorTypes__283261, debugParentDataTypes__283302, debugAncestorCulprits__283345);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__283207;
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
        _renderObject = (((RenderObjectWidget?)(object?)this.widget)!).createRenderObject(this);
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.RequireValue(((bool?)((dynamic)this._renderObject!).debugDisposed)));
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
        DartRuntimePrimitives.Assert(() => (object.Equals(this.slot, newSlot)));
        attachRenderObject(newSlot);
        base.performRebuild();
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (RenderObjectWidget)(object)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
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
                ((dynamic)this.renderObject).debugCreator = new DebugCreator(this);
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
        (((RenderObjectWidget?)(object?)this.widget)!).updateRenderObject(this, this.renderObject);
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
        DartRuntimePrimitives.Assert(() => !((global::Doroti.Framework.Rendering.RenderObject)this.renderObject).attached, () => (object?)"A RenderObject was still attached when attempting to deactivate its " + $"RenderObjectElement: {this.renderObject}");
    }

    public override void unmount()
    {
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Rendering.RenderObject)this.renderObject).debugDisposed), () => (object?)"A RenderObject was disposed prior to its owning element being unmounted: " + $"{this.renderObject}");
        var oldWidget__287428 = ((RenderObjectWidget?)(object?)this.widget)!;
        base.unmount();
        DartRuntimePrimitives.Assert(() => !((global::Doroti.Framework.Rendering.RenderObject)this.renderObject).attached, () => (object?)"A RenderObject was still attached when attempting to unmount its " + $"RenderObjectElement: {this.renderObject}");
        oldWidget__287428.didUnmountRenderObject(this.renderObject);
        ((dynamic)this._renderObject!).dispose();
        _renderObject = null;
    }

    internal virtual void _updateParentData<T>(ParentDataWidget<T> parentDataWidget)
    {
        var applyParentData__287853 = true;
        DartRuntimePrimitives.Assert(() =>
            {
                try
                {
                    if (!parentDataWidget.debugIsValidRenderObject(this.renderObject))
                    {
                        applyParentData__287853 = false;
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Incorrect use of ParentDataWidget.") }));
                    }
                }
                catch (global::Doroti.Framework.Foundation.FlutterError e__288494)
                {
                    FrameworkLibrary._reportException(new global::Doroti.Framework.Foundation.ErrorSummary("while applying parent data."), e__288494, e__288494.stackTrace);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (applyParentData__287853)
        {
            parentDataWidget.applyParentData(this.renderObject);
        }
    }

    public override void updateSlot(object? newSlot)
    {
        object? oldSlot__289094 = this.slot;
        DartRuntimePrimitives.Assert(() => (!object.Equals(oldSlot__289094, newSlot)));
        base.updateSlot(newSlot);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.slot, newSlot)));
        DartRuntimePrimitives.Assert(() => (object.Equals(this._ancestorRenderObjectElement, _findAncestorRenderObjectElement())));
        this._ancestorRenderObjectElement?.moveRenderObjectChild(this.renderObject, oldSlot__289094, this.slot);
    }

    public override void attachRenderObject(object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => (this._ancestorRenderObjectElement is null));
        _slot = newSlot;
        _ancestorRenderObjectElement = _findAncestorRenderObjectElement();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ancestorRenderObjectElement is null))
                {
                    FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"The render object for {toStringShort()} cannot find ancestor render object to attach to."), new global::Doroti.Framework.Foundation.ErrorDescription($"The ownership chain for the RenderObject in question was:\n  {debugGetCreatorChain(10L)}"), new global::Doroti.Framework.Foundation.ErrorHint("Try wrapping your widget in a View widget or any other widget that is backed by " + $"a {typeof(RenderTreeRootElement)} to serve as the root of the render tree.") })));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._ancestorRenderObjectElement?.insertRenderObjectChild(this.renderObject, newSlot);
        List<IParentDataElement> parentDataElements__290510 = _findAncestorParentDataElements();
        foreach (var parentDataElement__290589 in parentDataElements__290510)
        {
            parentDataElement__290589.applyParentDataTo(this);
        }
    }

    public override void detachRenderObject()
    {
        if ((this._ancestorRenderObjectElement is not null))
        {
            this._ancestorRenderObjectElement!.removeRenderObjectChild(this.renderObject, this.slot);
            _ancestorRenderObjectElement = null;
        }
        _slot = null;
    }

    public abstract void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot);
    public abstract void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot);
    public abstract void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot);
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.RenderObject>("renderObject", this._renderObject, defaultValue: null));
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
        DartRuntimePrimitives.Assert(() => (parent is null));
        DartRuntimePrimitives.Assert(() => (newSlot is null));
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

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return ((List<global::Doroti.Framework.Foundation.DiagnosticsNode>)(object?)this.widget.debugDescribeChildren());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SingleChildRenderObjectElement : RenderObjectElement
{
    internal virtual Element? _child { get; set; } = default;

    public SingleChildRenderObjectElement(SingleChildRenderObjectWidget widget) : base(widget)
    {
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
        _child = updateChild(this._child, ((Widget?)((dynamic)(((SingleChildRenderObjectWidget?)(object?)this.widget)!)).child), null);
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (SingleChildRenderObjectWidget)(object)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        _child = updateChild(this._child, ((Widget?)((dynamic)(((SingleChildRenderObjectWidget?)(object?)this.widget)!)).child), null);
    }

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        dynamic renderObject__296838 = this.renderObject;
        DartRuntimePrimitives.Assert(() => (slot is null));
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)renderObject__296838).debugValidateChild(child)));
        ((dynamic)renderObject__296838).child = (dynamic)child;
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObject__296838, this.renderObject)));
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        dynamic renderObject__297288 = this.renderObject;
        DartRuntimePrimitives.Assert(() => (slot is null));
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)renderObject__297288).child, child)));
        ((dynamic)renderObject__297288).child = null;
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObject__297288, this.renderObject)));
    }

}

public class MultiChildRenderObjectElement : RenderObjectElement
{
    internal virtual List<Element> _children { get; set; } = default!;
    internal virtual HashSet<Element> _forgottenChildren { get; private set; } = new HashSet<Element>();

    public MultiChildRenderObjectElement(MultiChildRenderObjectWidget widget) : base(widget)
    {
        System.Diagnostics.Debug.Assert(!global::Doroti.Framework.Widgets.DebugLibrary.debugChildrenHaveDuplicateKeys(widget, ((MultiChildRenderObjectWidget)widget).children.Cast<Widget>()));
    }

    public override global::Doroti.Framework.Rendering.RenderObject renderObject
    {
        get
        {
            return DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(base.renderObject);
            return default!;
        }
    }
    public virtual IEnumerable<Element> children => this._children.where(((child) => !this._forgottenChildren.Contains(child)));
    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __slot = (IndexedSlot<Element?>)(object)slot;
        dynamic renderObject__299399 = this.renderObject;
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)renderObject__299399).debugValidateChild(child)));
        renderObject__299399.insert((dynamic)child, after: (dynamic?)((IndexedSlot<Element?>)__slot).value?.renderObject);
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObject__299399, this.renderObject)));
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __oldSlot = (IndexedSlot<Element?>)(object)oldSlot;
        var __newSlot = (IndexedSlot<Element?>)(object)newSlot;
        dynamic renderObject__299839 = this.renderObject;
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Framework.Rendering.RenderObject)child).parent, renderObject__299839)));
        ((dynamic)renderObject__299839).move((dynamic)child, after: (dynamic?)((IndexedSlot<Element?>)__newSlot).value?.renderObject);
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObject__299839, this.renderObject)));
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        dynamic renderObject__300207 = this.renderObject;
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Framework.Rendering.RenderObject)child).parent, renderObject__300207)));
        ((dynamic)renderObject__300207).remove((dynamic)child);
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObject__300207, this.renderObject)));
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        foreach (Element child__300449 in this._children)
        {
            if (!this._forgottenChildren.Contains(child__300449))
            {
                visitor(child__300449);
            }
        }
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => this._children.Contains(child));
        DartRuntimePrimitives.Assert(() => !this._forgottenChildren.Contains(child));
        this._forgottenChildren.Add(child);
        base.forgetChild(child);
    }

    internal virtual bool _debugCheckHasAssociatedRenderObject(Element newChild)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((Element)newChild).renderObject is null))
                {
                    FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("The children of `MultiChildRenderObjectElement` must each has an associated render object."), new global::Doroti.Framework.Foundation.ErrorHint($"This typically means that the `{((Element)newChild).widget}` or its children\n" + "are not a subtype of `RenderObjectWidget`."), newChild.describeElement("The following element does not have an associated render object"), new global::Doroti.Framework.Rendering.DiagnosticsDebugCreator(new DebugCreator(newChild)) })));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Element inflateWidget(Widget newWidget, object? newSlot)
    {
        Element newChild__301761 = ((Element)(object?)base.inflateWidget(newWidget, newSlot));
        DartRuntimePrimitives.Assert(() => _debugCheckHasAssociatedRenderObject(newChild__301761));
        return newChild__301761;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        var multiChildRenderObjectWidget__302004 = ((MultiChildRenderObjectWidget?)(object?)this.widget)!;
        var children__302085 = new List<Element>(System.Linq.Enumerable.Repeat<Element>(_NullElement__framework.instance, checked((int)checked((long)(((MultiChildRenderObjectWidget)multiChildRenderObjectWidget__302004).children.Count)))));
        Element? previousChild__302219 = default!;
        for (var i__302247 = 0L; (i__302247 < checked((long)(children__302085.Count))); i__302247 += 1L)
        {
            Element newChild__302305 = ((Element)(object?)inflateWidget(((MultiChildRenderObjectWidget)multiChildRenderObjectWidget__302004).children[(int)(i__302247)], new IndexedSlot<Element?>(i__302247, previousChild__302219)));
            children__302085[(int)(i__302247)] = newChild__302305;
            previousChild__302219 = newChild__302305;
        }
        _children = children__302085;
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (MultiChildRenderObjectWidget)(object)newWidget;
        base.update(__newWidget);
        var multiChildRenderObjectWidget__302645 = ((MultiChildRenderObjectWidget?)(object?)this.widget)!;
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        DartRuntimePrimitives.Assert(() => !global::Doroti.Framework.Widgets.DebugLibrary.debugChildrenHaveDuplicateKeys(this.widget, ((MultiChildRenderObjectWidget)multiChildRenderObjectWidget__302645).children.Cast<Widget>()));
        _children = updateChildren(this._children, ((MultiChildRenderObjectWidget)multiChildRenderObjectWidget__302645).children, forgottenChildren: this._forgottenChildren);
        this._forgottenChildren.Clear();
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
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode)
        {
            return true;
        }
        if ((_findAncestorRenderObjectElement() is not null))
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"The RenderObject for {toStringShort()} cannot maintain an independent render tree at its current location."), new global::Doroti.Framework.Foundation.ErrorDescription($"The ownership chain for the RenderObject in question was:\n  {debugGetCreatorChain(10L)}"), new global::Doroti.Framework.Foundation.ErrorDescription("This RenderObject is the root of an independent render tree and it cannot " + "attach itself to an ancestor in an existing tree. The ancestor RenderObject, " + "however, expects that a child will be attached."), new global::Doroti.Framework.Foundation.ErrorHint($"Try moving the subtree that contains the {toStringShort()} widget " + "to a location where it is not expected to attach its RenderObject " + "to a parent. This could mean moving the subtree into the view " + "property of a \"ViewAnchor\" widget or - if the subtree is the root of " + "your widget tree - passing it to \"runWidget\" instead of \"runApp\"."), new global::Doroti.Framework.Foundation.ErrorHint("If you are seeing this error in a test and the subtree containing " + $"the {toStringShort()} widget is passed to \"WidgetTester.pumpWidget\", " + "consider setting the \"wrapWithView\" parameter of that method to false.") }));
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

    public override string ToString() => this.element.debugGetCreatorChain(12L);
}

public static partial class FrameworkLibrary
{
    internal static global::Doroti.Framework.Foundation.FlutterErrorDetails _reportException(global::Doroti.Framework.Foundation.DiagnosticsNode context, object exception, global::System.Diagnostics.StackTrace? stack, InformationCollector? informationCollector = null)
    {
        var details__306179 = new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: context, informationCollector: (InformationCollector?)informationCollector);
        FlutterError.reportError(details__306179);
        return details__306179;
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is IndexedSlot<T>) && (this.index == ((IndexedSlot<T>)(object)__other).index)) && object.Equals(this.value, ((IndexedSlot<T>)(object)__other).value));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.index, this.value));
}

internal class _NullElement__framework : Element
{
    public static _NullElement__framework instance = new _NullElement__framework();

    internal _NullElement__framework() : base(new _NullWidget__framework())
    {
    }

    public virtual bool debugDoingBuild => throw new NotImplementedException();
}

internal class _NullWidget__framework : Widget
{
    internal _NullWidget__framework()
    {
    }

    public override Element createElement() => throw new NotImplementedException();
}
