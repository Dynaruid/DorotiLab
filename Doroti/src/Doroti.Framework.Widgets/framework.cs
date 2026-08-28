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
    public virtual T? currentState => (this._currentElement switch { StatefulElement { state: T stateLocal } __object7625 => stateLocal, _ => default });
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
        var label = ((this._debugLabel is not null) ? $" {this._debugLabel}" : "");
        if ((object.Equals(this.GetType(), typeof(LabeledGlobalKey<T>))))
        {
            return $"[GlobalKey#{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.shortHash(this))}{label}]";
        }
        return $"[{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}{label}]";
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
        string selfType = global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "GlobalObjectKey");
        var suffix = "<State<StatefulWidget>>";
        if (selfType.endsWith(suffix))
        {
            selfType = selfType.substring(0L, (selfType.Length - suffix.Length));
        }
        return $"[{selfType} {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this.value))}]";
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
        string @type = global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Widget");
        return ((this.key is null) ? @type : $"{@type}-{this.key}");
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
        object? result = DartRuntimePrimitives.CaptureVoid(() => fn());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result is Future))
                {
                    Future result__53490__as53542 = (Future)result;
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
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString ?? (string)toStringShort()));
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
        var description = $"The ParentDataWidget {this} wants to apply ParentData of type {typeof(T)} to a RenderObject";
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
        List<Element> elements = ((Func<List<Element>>)(() =>
{
    var __cascade = this._elements.ToList();
    __cascade.sort(Element._sort);
    return __cascade;
}))().ToList();
        this._elements.Clear();
        try
        {
            System.Linq.Enumerable.Reverse(elements).forEach((__arg0) => ((global::System.Action<Element>)_unmount)(__arg0));
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
        bool result = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                result = this._elements.Contains(element);
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
        bool isTimelineTracked = (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && FrameworkLibrary._isProfileBuildsEnabledFor(((Element)element).widget));
        if (isTimelineTracked)
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode && global::Doroti.Framework.Widgets.DebugLibrary.debugEnhanceBuildTimelineArguments))
                    {
                        debugTimelineArguments = ((Diagnosticable)((Element)element).widget).toDiagnosticsNode().toTimelineArguments();
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            FlutterTimeline.startSync($"{DartRuntimePrimitives.RuntimeType(((Element)element).widget)}", arguments: debugTimelineArguments?.cast<string, object>());
        }
        try
        {
            element.rebuild();
        }
        catch (Exception e)
        {
            var stack = new System.Diagnostics.StackTrace();
            FrameworkLibrary._reportException(new global::Doroti.Framework.Foundation.ErrorDescription("while rebuilding dirty elements"), e, stack, informationCollector: (() => new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { element.describeElement("The element being rebuilt at the time was") }));
        }
        if (isTimelineTracked)
        {
            FlutterTimeline.finishSync();
        }
    }

    internal virtual bool _debugAssertElementInScope(Element element, Element debugBuildRoot)
    {
        bool isInScope = (element._debugIsDescendantOf(debugBuildRoot) || !((Element)element).debugIsActive);
        if (isInScope)
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
            for (var index = 0L; (index < checked((long)(this._dirtyElements.Count))); index = _dirtyElementIndexAfter(index))
            {
                Element elementLocal = this._dirtyElements[(int)(index)];
                if (DartRuntimePrimitives.Identical(((Element)elementLocal).buildScope, this))
                {
                    DartRuntimePrimitives.Assert(() => _debugAssertElementInScope(elementLocal, debugBuildRoot));
                    _tryRebuild(elementLocal);
                }
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    IEnumerable<Element> missedElements = this._dirtyElements.where(((element) => ((((Element)element).debugIsActive && ((Element)element).dirty) && DartRuntimePrimitives.Identical(((Element)element).buildScope, this))));
                    if (System.Linq.Enumerable.Any(missedElements))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("buildScope missed some dirty elements."), new global::Doroti.Framework.Foundation.ErrorHint("This probably indicates that the dirty list should have been resorted but was not."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<Element>("The context argument of the buildScope call was", debugBuildRoot, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), Element.describeElements("The list of missed elements at the end of the buildScope call was", missedElements.Cast<Element>()) }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        finally
        {
            foreach (Element elementAlternate in this._dirtyElements)
            {
                if (DartRuntimePrimitives.Identical(((Element)elementAlternate).buildScope, this))
                {
                    elementAlternate._inDirtyList = false;
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
                for (long i = (index - 1L); (i >= 0L); i -= 1L)
                {
                    Element element = this._dirtyElements[(int)(i)];
                    DartRuntimePrimitives.Assert(() => (!((Element)element).dirty || (!object.Equals(((Element)element)._lifecycleState, _ElementLifecycle__framework.active))));
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
        BuildScope buildScopeLocal = ((Element)element).buildScope;
        DartRuntimePrimitives.Assert(() =>
            {
                if ((global::Doroti.Framework.Widgets.DebugLibrary.debugPrintScheduleBuildForStacks && ((Element)element)._inDirtyList))
                {
                    global::Doroti.Framework.Foundation.AssertionsLibrary.debugPrintStack(label: "BuildOwner.scheduleBuildFor() called; " + $"_dirtyElementsNeedsResorting was {((BuildScope)buildScopeLocal)._dirtyElementsNeedsResorting} (now true); " + $"The dirty list for the current build scope is: {((BuildScope)buildScopeLocal)._dirtyElements}");
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
        buildScopeLocal._scheduleBuildFor(element);
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Widgets.DebugLibrary.debugPrintScheduleBuildForStacks)
                {
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"...the build scope's dirty list is now: {((BuildScope)buildScopeLocal)._dirtyElements}");
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
        BuildScope buildScopeLocal = ((Element)context).buildScope;
        if (((callback is null) && !System.Linq.Enumerable.Any(((BuildScope)buildScopeLocal)._dirtyElements)))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (this._debugStateLockLevel >= 0L));
        DartRuntimePrimitives.Assert(() => !this._debugBuilding);
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Widgets.DebugLibrary.debugPrintBuildScope)
                {
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"buildScope called with context {context}; " + $"its build scope's dirty list is: {((BuildScope)buildScopeLocal)._dirtyElements}");
                }
                _debugStateLockLevel += 1L;
                _debugBuilding = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Framework.Widgets.DebugLibrary.debugEnhanceBuildTimelineArguments)
                    {
                        debugTimelineArguments = new DartMap<string, string> { ["build scope dirty count"] = $"{checked((long)(((BuildScope)buildScopeLocal)._dirtyElements.Count))}", ["build scope dirty list"] = $"{((BuildScope)buildScopeLocal)._dirtyElements}", ["lock level"] = $"{this._debugStateLockLevel}", ["scope context"] = $"{context}" }.cast<string, string>();
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            FlutterTimeline.startSync("BUILD", arguments: debugTimelineArguments?.cast<string, object>());
        }
        try
        {
            _scheduledFlushDirtyElements = true;
            buildScopeLocal._building = true;
            if ((callback is not null))
            {
                DartRuntimePrimitives.Assert(() => this._debugStateLocked);
                Element? debugPreviousBuildTarget = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        debugPreviousBuildTarget = this._debugCurrentBuildTarget;
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
        DartMap<Element, HashSet<GlobalKeyBase>> map = _debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans ??= new DartMap<Element, HashSet<GlobalKeyBase>>();
        HashSet<GlobalKeyBase> keys = map.putIfAbsent(node, (() => new HashSet<GlobalKeyBase>()));
        keys.Add(key);
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
                    Element oldElement = this._globalKeyRegistry.GetValueOrDefault(key)!;
                    DartRuntimePrimitives.Assert(() => (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)element).widget), DartRuntimePrimitives.RuntimeType(((Element)oldElement).widget))));
                    this._debugIllFatedElements?.Add(oldElement);
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
                    Element oldElement = this._globalKeyRegistry.GetValueOrDefault(key)!;
                    DartRuntimePrimitives.Assert(() => (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)element).widget), DartRuntimePrimitives.RuntimeType(((Element)oldElement).widget))));
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
                var keyToParent = new DartMap<GlobalKeyBase, Element>();
                this._debugGlobalKeyReservations?.forEach(((global::System.Action<Element, DartMap<Element, GlobalKeyBase>>)((parent, childToKey) =>
                {
                    if (((object.Equals(((Element)parent)._lifecycleState, _ElementLifecycle__framework.defunct)) || (((Element)parent).renderObject?.attached == false)))
                    {
                        return;
                    }
                    childToKey.forEach(((global::System.Action<Element, GlobalKeyBase>)((child, key) =>
                    {
                        if ((((Element)child)._parent is null))
                        {
                            return;
                        }
                        if ((keyToParent.ContainsKey(key) && (!object.Equals(keyToParent.GetValueOrDefault(key), parent))))
                        {
                            Element older = keyToParent.GetValueOrDefault(key)!;
                            var newer = parent;
                            global::Doroti.Framework.Foundation.FlutterError error = default!;
                            if ((older.ToString() != newer.ToString()))
                            {
                                error = new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Multiple widgets used the same GlobalKey."), new global::Doroti.Framework.Foundation.ErrorDescription($"The key {key} was used by multiple widgets. The parents of those widgets were:\n" + $"- {older}\n" + $"- {newer}\n" + "A GlobalKey can only be specified on one widget at a time in the widget tree.") });
                            }
                            else
                            {
                                error = new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Multiple widgets used the same GlobalKey."), new global::Doroti.Framework.Foundation.ErrorDescription($"The key {key} was used by multiple widgets. The parents of those widgets were " + "different widgets that both had the following description:\n" + $"  {parent}\n" + "A GlobalKey can only be specified on one widget at a time in the widget tree.") });
                            }
                            if ((!object.Equals(((Element)child)._parent, older)))
                            {
                                older.visitChildren(((global::System.Action<Element>)((currentChild) =>
                                {
                                    if ((object.Equals(currentChild, child)))
                                    {
                                        older.forgetChild(child);
                                    }
                                })));
                            }
                            if ((!object.Equals(((Element)child)._parent, newer)))
                            {
                                newer.visitChildren(((global::System.Action<Element>)((currentChild) =>
                                {
                                    if ((object.Equals(currentChild, child)))
                                    {
                                        newer.forgetChild(child);
                                    }
                                })));
                            }
                            throw DartRuntimePrimitives.AsException(error);
                        }
                        else
                        {
                            keyToParent[key] = parent;
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
                DartMap<GlobalKeyBase, HashSet<Element>>? duplicates = default!;
                foreach (Element element in (this._debugIllFatedElements ?? new HashSet<Element>()))
                {
                    if ((!object.Equals(((Element)element)._lifecycleState, _ElementLifecycle__framework.defunct)))
                    {
                        DartRuntimePrimitives.Assert(() => (((Element)element).widget.key is not null));
                        var keyLocal = ((GlobalKeyBase?)(object?)((Element)element).widget.key!)!;
                        DartRuntimePrimitives.Assert(() => this._globalKeyRegistry.ContainsKey(keyLocal));
                        duplicates ??= new DartMap<GlobalKeyBase, HashSet<Element>>();
                        HashSet<Element> elements = duplicates.putIfAbsent(keyLocal, (() => new HashSet<Element>()));
                        elements.Add(element);
                        elements.Add(this._globalKeyRegistry.GetValueOrDefault(keyLocal)!);
                    }
                }
                this._debugIllFatedElements.Clear();
                if ((duplicates is not null))
                {
                    var information = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
                    information.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Multiple widgets used the same GlobalKey."));
                    foreach (GlobalKeyBase keyAlternate in duplicates.Keys)
                    {
                        HashSet<Element> elementsLocal = duplicates.GetValueOrDefault(keyAlternate)!;
                        information.Add(Element.describeElements($"The key {keyAlternate} was used by {checked((long)(elementsLocal.Count))} widgets", elementsLocal));
                    }
                    information.Add(new global::Doroti.Framework.Foundation.ErrorDescription("A GlobalKey can only be specified on one widget at a time in the widget tree."));
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(information));
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
                            HashSet<GlobalKeyBase> keys = new HashSet<GlobalKeyBase>();
                            foreach (Element elementLocal in this._debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans!.Keys)
                            {
                                if ((!object.Equals(((Element)elementLocal)._lifecycleState, _ElementLifecycle__framework.defunct)))
                                {
                                    keys.UnionWith(this._debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans!.GetValueOrDefault(elementLocal)!);
                                }
                            }
                            if (System.Linq.Enumerable.Any(keys))
                            {
                                DartMap<string, long> keyStringCount = new DartMap<string, long>();
                                foreach (string keyLocal in keys.map<GlobalKeyBase, string>(((key) => key.ToString())))
                                {
                                    if (keyStringCount.ContainsKey(keyLocal))
                                    {
                                        keyStringCount.update(keyLocal, ((value) => (value + 1L)));
                                    }
                                    else
                                    {
                                        keyStringCount[keyLocal] = 1L;
                                    }
                                }
                                var keyLabels = new List<string>();
                                IEnumerable<Element> elements = this._debugElementsThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans!.Keys;
                                DartMap<string, long> elementStringCount = new DartMap<string, long>();
                                foreach (string elementAlternate in elements.map<Element, string>(((element) => element.ToString())))
                                {
                                    if (elementStringCount.ContainsKey(elementAlternate))
                                    {
                                        elementStringCount.update(elementAlternate, ((value) => (value + 1L)));
                                    }
                                    else
                                    {
                                        elementStringCount[elementAlternate] = 1L;
                                    }
                                }
                                var elementLabels = new List<string>();
                                DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(keyLabels));
                                var the = ((checked((long)(keys.Count)) == 1L) ? " the" : "");
                                var s = ((checked((long)(keys.Count)) == 1L) ? "" : "s");
                                var were = ((checked((long)(keys.Count)) == 1L) ? "was" : "were");
                                var their = ((checked((long)(keys.Count)) == 1L) ? "its" : "their");
                                var respective = ((checked((long)(elementLabels.Count)) == 1L) ? "" : " respective");
                                var those = ((checked((long)(keys.Count)) == 1L) ? "that" : "those");
                                var s2 = ((checked((long)(elementLabels.Count)) == 1L) ? "" : "s");
                                var those2 = ((checked((long)(elementLabels.Count)) == 1L) ? "that" : "those");
                                var they = ((checked((long)(elementLabels.Count)) == 1L) ? "it" : "they");
                                var think = ((checked((long)(elementLabels.Count)) == 1L) ? "thinks" : "think");
                                var are = ((checked((long)(elementLabels.Count)) == 1L) ? "is" : "are");
                                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"Duplicate GlobalKey{s} detected in widget tree."), new global::Doroti.Framework.Foundation.ErrorDescription($"The following GlobalKey{s} {were} specified multiple times in the widget tree. This will lead to " + "parts of the widget tree being truncated unexpectedly, because the second time a key is seen, " + $"the previous instance is moved to the new location. The key{s} {were}:\n" + $"- {string.Join("\n  ", keyLabels)}\n" + $"This was determined by noticing that after{the} widget{s} with the above global key{s} {were} moved " + $"out of {their}{respective} previous parent{s2}, {those2} previous parent{s2} never updated during this frame, meaning " + $"that {they} either did not update at all or updated before the widget{s} {were} moved, in either case " + $"implying that {they} still {think} that {they} should have a child with {those} global key{s}.\n" + $"The specific parent{s2} that did not update after having one or more children forcibly removed " + $"due to GlobalKey reparenting {are}:\n" + $"- {string.Join("\n  ", elementLabels)}" + "\nA GlobalKey can only be specified on one widget at a time in the widget tree.") }));
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
        catch (Exception e)
        {
            var stack = new System.Diagnostics.StackTrace();
            FrameworkLibrary._reportException(new global::Doroti.Framework.Foundation.ErrorSummary("while finalizing the widget tree"), e, stack);
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
        long diff = (((Element)a).depth - ((Element)b).depth);
        if ((diff != 0L))
        {
            return diff;
        }
        bool isBDirty = ((Element)b).dirty;
        if ((((Element)a).dirty != isBDirty))
        {
            return (isBDirty ? -1L : 1L);
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
            var isDefunct = false;
            DartRuntimePrimitives.Assert(() =>
                {
                    isDefunct = (object.Equals(this._lifecycleState, _ElementLifecycle__framework.defunct));
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return isDefunct;
            return default!;
        }
    }
    public virtual bool debugIsActive
    {
        get
        {
            var isActive = false;
            DartRuntimePrimitives.Assert(() =>
                {
                    isActive = (object.Equals(this._lifecycleState, _ElementLifecycle__framework.active));
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return isActive;
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
        Element? element = this;
        while (((element is not null) && (((Element)element).depth > ((Element)target).depth)))
        {
            element = ((Element)element)._parent;
        }
        return (object.Equals(element, target));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.RenderObject renderObject
    {
        get
        {
            Element? current = this;
            while ((current is not null))
            {
                if ((object.Equals(((Element)current)._lifecycleState, _ElementLifecycle__framework.defunct)))
                {
                    break;
                }
                else
                {
                    if ((current is RenderObjectElement))
                    {
                        RenderObjectElement current__163649__as163793 = (RenderObjectElement)current;
                        return ((RenderObjectElement)((RenderObjectElement)current__163649__as163793)).renderObject;
                    }
                    else
                    {
                        current = ((Element)current).renderObjectAttachingChild;
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
            Element? next = default!;
            visitChildren(((global::System.Action<Element>)((child) =>
            {
                DartRuntimePrimitives.Assert(() => (next is null));
                next = child;
            })));
            return next;
            return default!;
        }
    }
    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> describeMissingAncestor(Type expectedAncestorType)
    {
        var information = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        var ancestors = new List<Element>();
        visitAncestorElements(((global::System.Func<Element, bool>)((element) =>
        {
            ancestors.Add(element);
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        information.Add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Element>($"The specific widget that could not find a {expectedAncestorType} ancestor was", this, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty));
        if (System.Linq.Enumerable.Any(ancestors))
        {
            information.Add(Element.describeElements("The ancestors of this widget were", ancestors.Cast<Element>()));
        }
        else
        {
            information.Add(new global::Doroti.Framework.Foundation.ErrorDescription("This widget is the root of the tree, so it has no " + $"ancestors, let alone a \"{expectedAncestorType}\" ancestor."));
        }
        return information;
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
        Element newChild = default!;
        if ((child is not null))
        {
            var hasSameSuperclass = true;
            DartRuntimePrimitives.Assert(() =>
                {
                    long oldElementClass = Element._debugConcreteSubtype(child);
                    long newWidgetClass = Widget._debugConcreteSubtype(newWidget);
                    hasSameSuperclass = (oldElementClass == newWidgetClass);
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            if ((hasSameSuperclass && (object.Equals(((Element)child).widget, newWidget))))
            {
                if ((!object.Equals(((Element)child).slot, newSlot)))
                {
                    updateSlotForChild(child, newSlot);
                }
                newChild = child;
            }
            else
            {
                if ((hasSameSuperclass && Widget.canUpdate(((Element)child).widget, newWidget)))
                {
                    if ((!object.Equals(((Element)child).slot, newSlot)))
                    {
                        updateSlotForChild(child, newSlot);
                    }
                    bool isTimelineTracked = (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && FrameworkLibrary._isProfileBuildsEnabledFor(newWidget));
                    if (isTimelineTracked)
                    {
                        DartMap<string, string>? debugTimelineArguments = default!;
                        DartRuntimePrimitives.Assert(() =>
                            {
                                if ((global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode && global::Doroti.Framework.Widgets.DebugLibrary.debugEnhanceBuildTimelineArguments))
                                {
                                    debugTimelineArguments = ((Diagnosticable)newWidget).toDiagnosticsNode().toTimelineArguments();
                                }
                                return true;
                                throw new InvalidOperationException("Dart closure completed without a value.");
                            });
                        FlutterTimeline.startSync($"{DartRuntimePrimitives.RuntimeType(newWidget)}", arguments: debugTimelineArguments?.cast<string, object>());
                    }
                    child.update(newWidget);
                    if (isTimelineTracked)
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
                    newChild = child;
                }
                else
                {
                    deactivateChild(child);
                    DartRuntimePrimitives.Assert(() => (((Element)child)._parent is null));
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
                if ((child is not null))
                {
                    _debugRemoveGlobalKeyReservation(child);
                }
                global::Doroti.Framework.Foundation.Key? keyLocal = ((Widget)newWidget).key;
                if ((keyLocal is GlobalKeyBase))
                {
                    GlobalKeyBase key__175416__as175447 = (GlobalKeyBase)keyLocal;
                    DartRuntimePrimitives.Assert(() => (this.owner is not null));
                    this.owner!._debugReserveGlobalKeyFor(this, newChild, key__175416__as175447);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return newChild;
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
        var newChildrenTop = 0L;
        var oldChildrenTop = 0L;
        long newChildrenBottom = (checked((long)(newWidgets.Count)) - 1L);
        long oldChildrenBottom = (checked((long)(oldChildren.Count)) - 1L);
        var newChildren = new List<Element>(System.Linq.Enumerable.Repeat<Element>(_NullElement__framework.instance, checked((int)checked((long)(newWidgets.Count)))));
        Element? previousChildLocal = default!;
        while ((((oldChildrenTop <= oldChildrenBottom)) && ((newChildrenTop <= newChildrenBottom))))
        {
            Element? oldChild = replaceWithNullIfForgotten(oldChildren[(int)(oldChildrenTop)]);
            Widget newWidget = newWidgets[(int)(newChildrenTop)];
            DartRuntimePrimitives.Assert(() => ((oldChild is null) || (object.Equals(((Element)oldChild)._lifecycleState, _ElementLifecycle__framework.active))));
            if (((oldChild is null) || !Widget.canUpdate(((Element)oldChild).widget, newWidget)))
            {
                break;
            }
            Element newChild = updateChild(oldChild, newWidget, slotFor(newChildrenTop, previousChildLocal))!;
            DartRuntimePrimitives.Assert(() => (object.Equals(((Element)newChild)._lifecycleState, _ElementLifecycle__framework.active)));
            newChildren[(int)(newChildrenTop)] = newChild;
            previousChildLocal = newChild;
            newChildrenTop += 1L;
            oldChildrenTop += 1L;
        }
        while ((((oldChildrenTop <= oldChildrenBottom)) && ((newChildrenTop <= newChildrenBottom))))
        {
            Element? oldChildLocal = replaceWithNullIfForgotten(oldChildren[(int)(oldChildrenBottom)]);
            Widget newWidgetLocal = newWidgets[(int)(newChildrenBottom)];
            DartRuntimePrimitives.Assert(() => ((oldChildLocal is null) || (object.Equals(((Element)oldChildLocal)._lifecycleState, _ElementLifecycle__framework.active))));
            if (((oldChildLocal is null) || !Widget.canUpdate(((Element)oldChildLocal).widget, newWidgetLocal)))
            {
                break;
            }
            oldChildrenBottom -= 1L;
            newChildrenBottom -= 1L;
        }
        bool haveOldChildren = (oldChildrenTop <= oldChildrenBottom);
        DartMap<global::Doroti.Framework.Foundation.Key, Element>? oldKeyedChildren = default!;
        if (haveOldChildren)
        {
            oldKeyedChildren = new DartMap<global::Doroti.Framework.Foundation.Key, Element>();
            while ((oldChildrenTop <= oldChildrenBottom))
            {
                Element? oldChildAlternate = replaceWithNullIfForgotten(oldChildren[(int)(oldChildrenTop)]);
                DartRuntimePrimitives.Assert(() => ((oldChildAlternate is null) || (object.Equals(((Element)oldChildAlternate)._lifecycleState, _ElementLifecycle__framework.active))));
                if ((oldChildAlternate is not null))
                {
                    if ((((Element)oldChildAlternate).widget.key is not null))
                    {
                        oldKeyedChildren[((Element)oldChildAlternate).widget.key!] = oldChildAlternate;
                    }
                    else
                    {
                        deactivateChild(oldChildAlternate);
                    }
                }
                oldChildrenTop += 1L;
            }
        }
        while ((newChildrenTop <= newChildrenBottom))
        {
            Element? oldChildNested = default!;
            Widget newWidgetAlternate = newWidgets[(int)(newChildrenTop)];
            if (haveOldChildren)
            {
                global::Doroti.Framework.Foundation.Key? keyLocal = ((Widget)newWidgetAlternate).key;
                if ((keyLocal is not null))
                {
                    oldChildNested = oldKeyedChildren!.GetValueOrDefault(keyLocal);
                    if ((oldChildNested is not null))
                    {
                        if (Widget.canUpdate(((Element)oldChildNested).widget, newWidgetAlternate))
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
            DartRuntimePrimitives.Assert(() => ((oldChildNested is null) || Widget.canUpdate(((Element)oldChildNested).widget, newWidgetAlternate)));
            Element newChildLocal = updateChild(oldChildNested, newWidgetAlternate, slotFor(newChildrenTop, previousChildLocal))!;
            DartRuntimePrimitives.Assert(() => (object.Equals(((Element)newChildLocal)._lifecycleState, _ElementLifecycle__framework.active)));
            DartRuntimePrimitives.Assert(() => (((object.Equals(oldChildNested, newChildLocal)) || (oldChildNested is null)) || (!object.Equals(((Element)oldChildNested)._lifecycleState, _ElementLifecycle__framework.active))));
            newChildren[(int)(newChildrenTop)] = newChildLocal;
            previousChildLocal = newChildLocal;
            newChildrenTop += 1L;
        }
        DartRuntimePrimitives.Assert(() => (oldChildrenTop == (oldChildrenBottom + 1L)));
        DartRuntimePrimitives.Assert(() => (newChildrenTop == (newChildrenBottom + 1L)));
        DartRuntimePrimitives.Assert(() => ((checked((long)(newWidgets.Count)) - newChildrenTop) == (checked((long)(oldChildren.Count)) - oldChildrenTop)));
        newChildrenBottom = (checked((long)(newWidgets.Count)) - 1L);
        oldChildrenBottom = (checked((long)(oldChildren.Count)) - 1L);
        while ((((oldChildrenTop <= oldChildrenBottom)) && ((newChildrenTop <= newChildrenBottom))))
        {
            Element oldChildCurrent = oldChildren[(int)(oldChildrenTop)];
            DartRuntimePrimitives.Assert(() => (replaceWithNullIfForgotten(oldChildCurrent) is not null));
            DartRuntimePrimitives.Assert(() => (object.Equals(((Element)oldChildCurrent)._lifecycleState, _ElementLifecycle__framework.active)));
            Widget newWidgetNested = newWidgets[(int)(newChildrenTop)];
            DartRuntimePrimitives.Assert(() => Widget.canUpdate(((Element)oldChildCurrent).widget, newWidgetNested));
            Element newChildAlternate = updateChild(oldChildCurrent, newWidgetNested, slotFor(newChildrenTop, previousChildLocal))!;
            DartRuntimePrimitives.Assert(() => (object.Equals(((Element)newChildAlternate)._lifecycleState, _ElementLifecycle__framework.active)));
            DartRuntimePrimitives.Assert(() => ((object.Equals(oldChildCurrent, newChildAlternate)) || (!object.Equals(((Element)oldChildCurrent)._lifecycleState, _ElementLifecycle__framework.active))));
            newChildren[(int)(newChildrenTop)] = newChildAlternate;
            previousChildLocal = newChildAlternate;
            newChildrenTop += 1L;
            oldChildrenTop += 1L;
        }
        if ((haveOldChildren && System.Linq.Enumerable.Any(oldKeyedChildren!)))
        {
            foreach (Element oldChildNext in oldKeyedChildren.Values)
            {
                if (((forgottenChildren is null) || !forgottenChildren.Contains(oldChildNext)))
                {
                    deactivateChild(oldChildNext);
                }
            }
        }
        DartRuntimePrimitives.Assert(() => newChildren.All(((element) => (element is not _NullElement__framework))));
        return newChildren;
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
        global::Doroti.Framework.Foundation.Key? keyLocal = ((Widget)this.widget).key;
        if ((keyLocal is GlobalKeyBase))
        {
            GlobalKeyBase key__188214__as188240 = (GlobalKeyBase)keyLocal;
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
            Element? descendant = ((Element)element).renderObjectAttachingChild;
            if ((descendant is not null))
            {
                visit(descendant);
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
        long expectedDepth = (parentDepth + 1L);
        if ((this._depth < expectedDepth))
        {
            _depth = expectedDepth;
            visitChildren(((global::System.Action<Element>)((child) =>
            {
                child._updateDepth(expectedDepth);
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
        Element? element = key._currentElement;
        if ((element is null))
        {
            return ((Element)(object)null);
        }
        if (!Widget.canUpdate(((Element)element).widget, newWidget))
        {
            return ((Element)(object)null);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Widgets.DebugLibrary.debugPrintGlobalKeyedWidgetLifecycle)
                {
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"Attempting to take {element} from {(((object?)((Element)element)._parent ?? (object?)"inactive elements list"))} to put in {this}.");
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        Element? parent = ((Element)element)._parent;
        if ((parent is not null))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((object.Equals(parent, this)))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("A GlobalKey was used multiple times inside one widget's child list."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<GlobalKeyBase>("The offending GlobalKey was", key), parent.describeElement("The parent of the widgets with that key was"), element.describeElement("The first child to get instantiated with that key became"), new global::Doroti.Framework.Foundation.DiagnosticsProperty<Widget>("The second child that was to get instantiated with that key was", this.widget, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorDescription("A GlobalKey can only be specified on one widget at a time in the widget tree.") }));
                    }
                    ((Element)parent).owner!._debugTrackElementThatWillNeedToBeRebuiltDueToGlobalKeyShenanigans(parent, key);
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            parent.forgetChild(element);
            parent.deactivateChild(element);
        }
        DartRuntimePrimitives.Assert(() => (((Element)element)._parent is null));
        this.owner!._inactiveElements.remove(element);
        return element;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Element inflateWidget(Widget newWidget, object? newSlot)
    {
        bool isTimelineTracked = (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && FrameworkLibrary._isProfileBuildsEnabledFor(newWidget));
        if (isTimelineTracked)
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode && global::Doroti.Framework.Widgets.DebugLibrary.debugEnhanceBuildTimelineArguments))
                    {
                        debugTimelineArguments = ((Diagnosticable)newWidget).toDiagnosticsNode().toTimelineArguments();
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            FlutterTimeline.startSync($"{DartRuntimePrimitives.RuntimeType(newWidget)}", arguments: debugTimelineArguments?.cast<string, object>());
        }
        try
        {
            global::Doroti.Framework.Foundation.Key? keyLocal = ((Widget)newWidget).key;
            Element? inactiveChild = ((keyLocal is GlobalKeyBase globalKey) ? _retakeInactiveElement(globalKey, newWidget) : null);
            Element newChild = ((inactiveChild ?? (Element)newWidget.createElement()));
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugCheckForCycles(newChild);
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            try
            {
                if ((inactiveChild is not null))
                {
                    DartRuntimePrimitives.Assert(() => (((Element)inactiveChild)._parent is null));
                    inactiveChild._activateWithParent(this, newSlot);
                    Element? updatedChild = ((Element?)(object?)updateChild(inactiveChild, newWidget, newSlot));
                    DartRuntimePrimitives.Assert(() => (object.Equals(inactiveChild, updatedChild)));
                    return updatedChild!;
                }
                else
                {
                    newChild.mount(this, newSlot);
                    DartRuntimePrimitives.Assert(() => (object.Equals(((Element)newChild)._lifecycleState, _ElementLifecycle__framework.active)));
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
        DartRuntimePrimitives.Assert(() => (((Element)newChild)._parent is null));
        DartRuntimePrimitives.Assert(() =>
            {
                var node = this;
                while ((((Element)node)._parent is not null))
                {
                    node = ((Element)node)._parent!;
                }
                DartRuntimePrimitives.Assert(() => (!object.Equals(node, newChild)));
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
        bool hadDependencies = ((((this._dependencies is { } __items203339 ? System.Linq.Enumerable.Any(__items203339) : (bool?)null) ?? false)) || this._hadUnsatisfiedDependencies);
        _lifecycleState = _ElementLifecycle__framework.active;
        this._dependencies.Clear();
        _hadUnsatisfiedDependencies = false;
        _updateInheritance();
        attachNotificationTree();
        if (this._dirty)
        {
            this.owner!.scheduleBuildFor(this);
        }
        if (hadDependencies)
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
        if (this._dependencies is HashSet<InheritedElement> dependencies && (System.Linq.Enumerable.Any(dependencies)))
        {
            foreach (var dependency in dependencies)
            {
                dependency.removeDependent(this);
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
        global::Doroti.Framework.Foundation.Key? keyLocal = this._widget?.key;
        if ((keyLocal is GlobalKeyBase))
        {
            GlobalKeyBase key__207717__as207745 = (GlobalKeyBase)keyLocal;
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
            global::Doroti.Framework.Rendering.RenderObject? renderObject = ((global::Doroti.Framework.Rendering.RenderObject?)(object?)findRenderObject());
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((renderObject is null))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get size without a render object."), new global::Doroti.Framework.Foundation.ErrorHint("In order for an element to have a valid size, the element must have " + "an associated render object. This element does not have an associated " + "render object, which typically means that the size getter was called " + "too early in the pipeline (e.g., during the build phase) before the " + "framework has created the render tree."), describeElement("The size getter was called for the following element") }));
                    }
                    if ((renderObject is global::Doroti.Framework.Rendering.RenderSliver))
                    {
                        global::Doroti.Framework.Rendering.RenderSliver renderObject__212317__as213062 = (global::Doroti.Framework.Rendering.RenderSliver)renderObject;
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get size from a RenderSliver."), new global::Doroti.Framework.Foundation.ErrorHint("The render object associated with this element is a " + $"{(DartRuntimePrimitives.RuntimeType(((global::Doroti.Framework.Rendering.RenderSliver)renderObject__212317__as213062)))}, which is a subtype of RenderSliver. " + "Slivers do not have a size per se. They have a more elaborate " + "geometry description, which can be accessed by calling " + "findRenderObject and then using the \"geometry\" getter on the " + "resulting object."), describeElement("The size getter was called for the following element"), ((global::Doroti.Framework.Rendering.RenderSliver)renderObject__212317__as213062).describeForError("The associated render sliver was") }));
                    }
                    if ((renderObject is not global::Doroti.Framework.Rendering.RenderBox))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get size from a render object that is not a RenderBox."), new global::Doroti.Framework.Foundation.ErrorHint("Instead of being a subtype of RenderBox, the render object associated " + $"with this element is a {DartRuntimePrimitives.RuntimeType(renderObject)}. If this type of " + "render object does have a size, consider calling findRenderObject " + "and extracting its size manually."), describeElement("The size getter was called for the following element"), renderObject.describeForError("The associated render object was") }));
                    }
                    global::Doroti.Framework.Rendering.RenderBox box = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderBox>(((global::Doroti.Framework.Rendering.RenderBox)renderObject));
                    if (!((global::Doroti.Framework.Rendering.RenderBox)box).hasSize)
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get size from a render object that has not been through layout."), new global::Doroti.Framework.Foundation.ErrorHint("The size of this render object has not yet been determined because " + "this render object has not yet been through layout, which typically " + "means that the size getter was called too early in the pipeline " + "(e.g., during the build phase) before the framework has determined " + "the size and position of the render objects during layout."), describeElement("The size getter was called for the following element"), box.describeForError("The render object from which the size was to be obtained was") }));
                    }
                    if (box.debugNeedsLayout)
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Cannot get size from a render object that has been marked dirty for layout."), new global::Doroti.Framework.Foundation.ErrorHint("The size of this render object is ambiguous because this render object has " + "been modified since it was last laid out, which typically means that the size " + "getter was called too early in the pipeline (e.g., during the build phase) " + "before the framework has determined the size and position of the render " + "objects during layout."), describeElement("The size getter was called for the following element"), box.describeForError("The render object from which the size was to be obtained was"), new global::Doroti.Framework.Foundation.ErrorHint("Consider using debugPrintMarkNeedsLayoutStacks to determine why the render " + "object in question is dirty, if you did not expect this.") }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            if ((renderObject is global::Doroti.Framework.Rendering.RenderBox))
            {
                global::Doroti.Framework.Rendering.RenderBox renderObject__212317__as216465 = (global::Doroti.Framework.Rendering.RenderBox)renderObject;
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
        InheritedElement? ancestor = this._inheritedElements.GetValueOrDefault(typeof(T));
        if ((ancestor is not null))
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
        Element? ancestor = this._parent;
        while (((ancestor is not null) && (!object.Equals(DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget), typeof(T)))))
        {
            ancestor = ((Element)ancestor)._parent;
        }
        return ((T?)(object?)ancestor?.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? findAncestorStateOfType<T>()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor = this._parent;
        while ((ancestor is not null))
        {
            if (((ancestor is StatefulElement) && (((StatefulElement)((StatefulElement)ancestor)).state is T)))
            {
                StatefulElement ancestor__219807__as219868 = (StatefulElement)ancestor;
                break;
            }
            ancestor = ((Element)ancestor)._parent;
        }
        var statefulAncestor = ((StatefulElement?)(object?)ancestor)!;
        return ((T?)(object?)statefulAncestor?.state)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? findRootAncestorStateOfType<T>()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor = this._parent;
        StatefulElement? statefulAncestor = default!;
        while ((ancestor is not null))
        {
            if (((ancestor is StatefulElement) && (((StatefulElement)((StatefulElement)ancestor)).state is T)))
            {
                StatefulElement ancestor__220244__as220344 = (StatefulElement)ancestor;
                statefulAncestor = ((StatefulElement)ancestor__220244__as220344);
            }
            ancestor = ((Element)ancestor)._parent;
        }
        return ((T?)(object?)statefulAncestor?.state)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? findAncestorRenderObjectOfType<T>()
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor = this._parent;
        while ((ancestor is not null))
        {
            if (((ancestor is RenderObjectElement) && (((RenderObjectElement)((RenderObjectElement)ancestor)).renderObject is T)))
            {
                RenderObjectElement ancestor__220677__as220738 = (RenderObjectElement)ancestor;
                return ((T?)(object?)((RenderObjectElement)((RenderObjectElement)ancestor__220677__as220738)).renderObject)!;
            }
            ancestor = ((Element)ancestor)._parent;
        }
        return default;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void visitAncestorElements(global::System.Func<Element, bool> visitor)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckStateIsActiveForAncestorLookup());
        Element? ancestor = this._parent;
        while (((ancestor is not null) && visitor(ancestor)))
        {
            ancestor = ((Element)ancestor)._parent;
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
        var chain = new List<string>();
        Element? node = this;
        while (((checked((long)(chain.Count)) < limit) && (node is not null)))
        {
            chain.Add(((Diagnosticable)node).toStringShort());
            node = ((Element)node)._parent;
        }
        if ((node is not null))
        {
            chain.Add("⋯");
        }
        return string.Join(" ← ", chain);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<Element> debugGetDiagnosticChain()
    {
        var chain = new List<Element> { this };
        Element? node = this._parent;
        while ((node is not null))
        {
            chain.Add(node);
            node = ((Element)node)._parent;
        }
        return chain;
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
        HashSet<InheritedElement>? deps = this._dependencies;
        if (((deps is not null) && System.Linq.Enumerable.Any(deps)))
        {
            List<InheritedElement> sortedDependencies = ((Func<List<InheritedElement>>)(() =>
{
    var __cascade = deps.ToList();
    __cascade.sort(((a, b) => ((Diagnosticable)a).toStringShort().CompareTo(((Diagnosticable)b).toStringShort())));
    return __cascade;
}))().ToList();
            List<global::Doroti.Framework.Foundation.DiagnosticsNode> diagnosticsDependencies = sortedDependencies.map<InheritedElement, global::Doroti.Framework.Foundation.DiagnosticsNode>(((element) => ((Diagnosticable)((InheritedElement)element).widget).toDiagnosticsNode(style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.sparse))).ToList().ToList();
            properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<InheritedElement>>("dependencies", deps, description: diagnosticsDependencies.ToString()));
        }
    }

    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        visitChildren(((global::System.Action<Element>)((child) =>
        {
            children.Add(((Diagnosticable)child).toDiagnosticsNode());
        })));
        return children;
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
                    var information = new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("setState() or markNeedsBuild() called during build."), new global::Doroti.Framework.Foundation.ErrorDescription($"This {DartRuntimePrimitives.RuntimeType(this.widget)} widget cannot be marked as needing to build because the framework " + "is already in the process of building widgets. A widget can be marked as " + "needing to be built during the build phase only if one of its ancestors " + "is currently building. This exception is allowed because the framework " + "builds parent widgets before children, which means a dirty descendant " + "will always be built. Otherwise, the framework might not visit this " + "widget during this build phase."), describeElement("The widget on which setState() or markNeedsBuild() was called was") };
                    if ((this.owner!._debugCurrentBuildTarget is not null))
                    {
                        information.Add(this.owner!._debugCurrentBuildTarget!.describeWidget("The widget which was currently being built when the offending call was made was"));
                    }
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(information));
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
        Element? debugPreviousBuildTarget = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                debugPreviousBuildTarget = this.owner!._debugCurrentBuildTarget;
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
                    this.owner!._debugCurrentBuildTarget = debugPreviousBuildTarget;
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
        DartMap<string, object?> json = ((DartMap<string, object?>)(object?)base.toJsonMap(@delegate));
        var element = ((Element?)(object?)this.value)!;
        if (!((Element)element).debugIsDefunct)
        {
            json["widgetRuntimeType"] = DartRuntimePrimitives.RuntimeTypeName(((Element)element).widget);
        }
        json["stateful"] = this.stateful;
        return json;
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
        var messageLocal = "";
        DartRuntimePrimitives.Assert(() =>
            {
                messageLocal = $"{(ErrorWidget._stringify(((global::Doroti.Framework.Foundation.FlutterErrorDetails)details).exception))}\nSee also: https://docs.flutter.dev/testing/errors";
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        object exceptionLocal = ((global::Doroti.Framework.Foundation.FlutterErrorDetails)details).exception;
        return ((Widget)(object?)ErrorWidget.CreateWithDetails(message: messageLocal, error: ((exceptionLocal is global::Doroti.Framework.Foundation.FlutterError) ? ((global::Doroti.Framework.Foundation.FlutterError)exceptionLocal) : null)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static string _stringify(object? exception)
    {
        try
        {
            return ((string)((dynamic)exception).ToString());
        }
        catch (Exception error)
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
            global::Doroti.Framework.Widgets.DebugLibrary.debugWidgetBuilderValue(this.widget, built);
        }
        catch (Exception e)
        {
            var stack = new System.Diagnostics.StackTrace();
            _debugDoingBuild = false;
            built = ErrorWidget.builder(FrameworkLibrary._reportException(new global::Doroti.Framework.Foundation.ErrorDescription($"building {this}"), e, stack, informationCollector: (() => new List<global::Doroti.Framework.Foundation.DiagnosticsNode>())));
        }
        try
        {
            _child = updateChild(this._child, built, this.slot);
            DartRuntimePrimitives.Assert(() => (this._child is not null));
        }
        catch (Exception eLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            built = ErrorWidget.builder(FrameworkLibrary._reportException(new global::Doroti.Framework.Foundation.ErrorDescription($"building {this}"), eLocal, stackLocal, informationCollector: (() => new List<global::Doroti.Framework.Foundation.DiagnosticsNode>())));
            try
            {
                this._child?.deactivate();
            }
            catch
            {
            }
            _child = updateChild(((Element)(object)null), built, this.slot);
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
        object? debugCheckForReturnedFuture = DartRuntimePrimitives.CaptureVoid(() => this.state.initState());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((debugCheckForReturnedFuture is Future))
                {
                    Future debugCheckForReturnedFuture__250900__as250986 = (Future)debugCheckForReturnedFuture;
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
        StatefulWidget oldWidget = this.state._widget!;
        this.state._widget = ((StatefulWidget?)(object?)this.widget)!;
        object? debugCheckForReturnedFuture = DartRuntimePrimitives.CaptureVoid(() => this.state.didUpdateWidget(oldWidget));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((debugCheckForReturnedFuture is Future))
                {
                    Future debugCheckForReturnedFuture__252202__as252303 = (Future)debugCheckForReturnedFuture;
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
                Type targetType = DartRuntimePrimitives.RuntimeType(((Element)ancestor).widget);
                if ((object.Equals(this.state._debugLifecycleState, _StateLifecycle__framework.created)))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"dependOnInheritedWidgetOfExactType<{targetType}>() or dependOnInheritedElement() was called before {DartRuntimePrimitives.RuntimeType(this.state)}.initState() completed."), new global::Doroti.Framework.Foundation.ErrorDescription("When an inherited widget changes, for example if the value of Theme.of() changes, " + "its dependent widgets are rebuilt. If the dependent widget's reference to " + "the inherited widget is in a constructor or an initState() method, " + "then the rebuilt dependent widget will not reflect the changes in the " + "inherited widget."), new global::Doroti.Framework.Foundation.ErrorHint("Typically references to inherited widgets should occur in widget build() methods. Alternatively, " + "initialization based on inherited widgets can be placed in the didChangeDependencies method, which " + "is called after initState and whenever the dependencies change thereafter.") }));
                }
                if ((object.Equals(this.state._debugLifecycleState, _StateLifecycle__framework.defunct)))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"dependOnInheritedWidgetOfExactType<{targetType}>() or dependOnInheritedElement() was called after dispose(): {this}"), new global::Doroti.Framework.Foundation.ErrorDescription("This error happens if you call dependOnInheritedWidgetOfExactType() on the " + "BuildContext for a widget that no longer appears in the widget tree " + "(e.g., whose parent widget no longer includes the widget in its " + "build). This error can occur when code calls " + "dependOnInheritedWidgetOfExactType() from a timer or an animation callback."), new global::Doroti.Framework.Foundation.ErrorHint("The preferred solution is to cancel the timer or stop listening to the " + "animation in the dispose() callback. Another solution is to check the " + "\"mounted\" property of this object before calling " + "dependOnInheritedWidgetOfExactType() to ensure the object is still in the " + "tree."), new global::Doroti.Framework.Foundation.ErrorHint("This error might indicate a memory leak if " + "dependOnInheritedWidgetOfExactType() is being called because another object " + "is retaining a reference to this State object after it has been " + "removed from the tree. To avoid memory leaks, consider breaking the " + "reference to this object during dispose().") }));
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

    public override Widget build() => ((ProxyWidget)this.widget).child;
    public override void update(Widget newWidget)
    {
        var __newWidget = (ProxyWidget)(object)newWidget;
        var oldWidget = ((ProxyWidget?)(object?)this.widget)!;
        DartRuntimePrimitives.Assert(() => (!object.Equals(this.widget, __newWidget)));
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
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
            if ((@type is not null))
            {
                return @type!;
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
        DartRuntimePrimitives.Assert(() => (object.Equals(newWidget.child, ((ParentDataWidget<T>)this.widget).child)));
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
        global::Doroti.Framework.Foundation.PersistentHashMap<Type, InheritedElement> incomingWidgets = (this._parent?._inheritedElements ?? global::Doroti.Framework.Foundation.PersistentHashMap<Type, InheritedElement>.CreateEmpty());
        _inheritedElements = incomingWidgets.put(DartRuntimePrimitives.RuntimeType(this.widget), this);
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
        foreach (Element dependent in this._dependents.Keys)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    Element? ancestor = ((Element)dependent)._parent;
                    while (((!object.Equals(ancestor, this)) && (ancestor is not null)))
                    {
                        ancestor = ((Element)ancestor)._parent;
                    }
                    return (object.Equals(ancestor, this));
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            DartRuntimePrimitives.Assert(() => ((Element)dependent)._dependencies!.Contains(this));
            notifyDependent(__oldWidget, dependent);
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
        Element? ancestor = this._parent;
        while (((ancestor is not null) && (ancestor is not RenderObjectElement)))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (!ancestor!.debugExpectsRenderObjectForSlot(this.slot))
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
                if ((ancestor?.debugExpectsRenderObjectForSlot(this.slot) == false))
                {
                    ancestor = null;
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((RenderObjectElement?)(object?)ancestor)!;
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
                    catch (global::Doroti.Framework.Foundation.FlutterError error)
                    {
                        FrameworkLibrary._reportException(new global::Doroti.Framework.Foundation.ErrorSummary("while looking for parent data."), error, error.stackTrace);
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual List<IParentDataElement> _findAncestorParentDataElements()
    {
        Element? ancestorLocal = this._parent;
        var result = new List<IParentDataElement>();
        var debugAncestorTypes = new HashSet<Type>();
        var debugParentDataTypes = new HashSet<Type>();
        var debugAncestorCulprits = new List<Type>();
        while (((ancestorLocal is not null) && (ancestorLocal is not RenderObjectElement)))
        {
            if ((ancestorLocal is IParentDataElement))
            {
                IParentDataElement ancestor__283177__as284599 = (IParentDataElement)ancestorLocal;
                DartRuntimePrimitives.Assert(() =>
                    {
                        IParentDataElement ancestor = ancestor__283177__as284599;
                        if ((!debugAncestorTypes.Add(DartRuntimePrimitives.RuntimeType(ancestor)) || !debugParentDataTypes.Add(ancestor.debugParentDataType)))
                        {
                            debugAncestorCulprits.Add(DartRuntimePrimitives.RuntimeType(ancestor));
                        }
                        return true;
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                result.Add(ancestor__283177__as284599);
            }
            ancestorLocal = ((Element)ancestorLocal)._parent;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((!System.Linq.Enumerable.Any(result) || (ancestorLocal is null)))
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
        _renderObject = (((RenderObjectWidget?)(object?)this.widget)!).createRenderObject(this);
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.RequireValue(this._renderObject!.debugDisposed));
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
                this.renderObject.debugCreator = new DebugCreator(this);
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
        var oldWidget = ((RenderObjectWidget?)(object?)this.widget)!;
        base.unmount();
        DartRuntimePrimitives.Assert(() => !((global::Doroti.Framework.Rendering.RenderObject)this.renderObject).attached, () => (object?)"A RenderObject was still attached when attempting to unmount its " + $"RenderObjectElement: {this.renderObject}");
        oldWidget.didUnmountRenderObject(this.renderObject);
        this._renderObject!.dispose();
        _renderObject = null;
    }

    internal virtual void _updateParentData<T>(ParentDataWidget<T> parentDataWidget)
    {
        var applyParentDataLocal = true;
        DartRuntimePrimitives.Assert(() =>
            {
                try
                {
                    if (!parentDataWidget.debugIsValidRenderObject(this.renderObject))
                    {
                        applyParentDataLocal = false;
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Incorrect use of ParentDataWidget.") }));
                    }
                }
                catch (global::Doroti.Framework.Foundation.FlutterError e)
                {
                    FrameworkLibrary._reportException(new global::Doroti.Framework.Foundation.ErrorSummary("while applying parent data."), e, e.stackTrace);
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (applyParentDataLocal)
        {
            parentDataWidget.applyParentData(this.renderObject);
        }
    }

    public override void updateSlot(object? newSlot)
    {
        object? oldSlot = this.slot;
        DartRuntimePrimitives.Assert(() => (!object.Equals(oldSlot, newSlot)));
        base.updateSlot(newSlot);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.slot, newSlot)));
        DartRuntimePrimitives.Assert(() => (object.Equals(this._ancestorRenderObjectElement, _findAncestorRenderObjectElement())));
        this._ancestorRenderObjectElement?.moveRenderObjectChild(this.renderObject, oldSlot, this.slot);
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
        List<IParentDataElement> parentDataElements = _findAncestorParentDataElements();
        foreach (var parentDataElement in parentDataElements)
        {
            parentDataElement.applyParentDataTo(this);
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
        _child = updateChild(this._child, ((SingleChildRenderObjectWidget)this.widget).child, null);
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (SingleChildRenderObjectWidget)(object)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        _child = updateChild(this._child, ((SingleChildRenderObjectWidget)this.widget).child, null);
    }

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var renderObjectLocal = (global::Doroti.Framework.Rendering.IRenderObjectWithChild)this.renderObject;
        DartRuntimePrimitives.Assert(() => (slot is null));
        DartRuntimePrimitives.Assert(() => renderObjectLocal.debugValidateChild(child));
        renderObjectLocal.child = child;
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObjectLocal, this.renderObject)));
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var renderObjectLocal = (global::Doroti.Framework.Rendering.IRenderObjectWithChild)this.renderObject;
        DartRuntimePrimitives.Assert(() => (slot is null));
        DartRuntimePrimitives.Assert(() => object.Equals(renderObjectLocal.child, child));
        renderObjectLocal.child = null;
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObjectLocal, this.renderObject)));
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
        var renderObjectLocal = (global::Doroti.Framework.Rendering.IContainerRenderObject)this.renderObject;
        DartRuntimePrimitives.Assert(() => renderObjectLocal.debugValidateChild(child));
        renderObjectLocal.insert(child, after: ((IndexedSlot<Element?>)__slot).value?.renderObject);
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObjectLocal, this.renderObject)));
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __oldSlot = (IndexedSlot<Element?>)(object)oldSlot;
        var __newSlot = (IndexedSlot<Element?>)(object)newSlot;
        var renderObjectLocal = (global::Doroti.Framework.Rendering.IContainerRenderObject)this.renderObject;
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Framework.Rendering.RenderObject)child).parent, renderObjectLocal)));
        renderObjectLocal.move(child, after: ((IndexedSlot<Element?>)__newSlot).value?.renderObject);
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObjectLocal, this.renderObject)));
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var renderObjectLocal = (global::Doroti.Framework.Rendering.IContainerRenderObject)this.renderObject;
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Framework.Rendering.RenderObject)child).parent, renderObjectLocal)));
        renderObjectLocal.remove(child);
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObjectLocal, this.renderObject)));
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        foreach (Element child in this._children)
        {
            if (!this._forgottenChildren.Contains(child))
            {
                visitor(child);
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
        Element newChild = ((Element)(object?)base.inflateWidget(newWidget, newSlot));
        DartRuntimePrimitives.Assert(() => _debugCheckHasAssociatedRenderObject(newChild));
        return newChild;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        var multiChildRenderObjectWidget = ((MultiChildRenderObjectWidget?)(object?)this.widget)!;
        var childrenLocal = new List<Element>(System.Linq.Enumerable.Repeat<Element>(_NullElement__framework.instance, checked((int)checked((long)(((MultiChildRenderObjectWidget)multiChildRenderObjectWidget).children.Count)))));
        Element? previousChild = default!;
        for (var i = 0L; (i < checked((long)(childrenLocal.Count))); i += 1L)
        {
            Element newChild = ((Element)(object?)inflateWidget(((MultiChildRenderObjectWidget)multiChildRenderObjectWidget).children[(int)(i)], new IndexedSlot<Element?>(i, previousChild)));
            childrenLocal[(int)(i)] = newChild;
            previousChild = newChild;
        }
        _children = childrenLocal;
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (MultiChildRenderObjectWidget)(object)newWidget;
        base.update(__newWidget);
        var multiChildRenderObjectWidget = ((MultiChildRenderObjectWidget?)(object?)this.widget)!;
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        DartRuntimePrimitives.Assert(() => !global::Doroti.Framework.Widgets.DebugLibrary.debugChildrenHaveDuplicateKeys(this.widget, ((MultiChildRenderObjectWidget)multiChildRenderObjectWidget).children.Cast<Widget>()));
        _children = updateChildren(this._children, ((MultiChildRenderObjectWidget)multiChildRenderObjectWidget).children, forgottenChildren: this._forgottenChildren);
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
        var details = new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: context, informationCollector: (InformationCollector?)informationCollector);
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
