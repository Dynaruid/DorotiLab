// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/selection_container.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class SelectionContainer : StatefulWidget
{
    public virtual global::Doroti.Framework.Rendering.SelectionRegistrar? registrar { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual SelectionContainerDelegate? @delegate { get; private set; }

    public SelectionContainer(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Rendering.SelectionRegistrar? registrar = null, SelectionContainerDelegate @delegate = default!, Widget child = default!) : base(key: key)
    {
        this.registrar = registrar;
        this.@delegate = @delegate;
        this.child = child;
    }

    public static SelectionContainer CreateDisabled(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!)
    {
        var __instance = new SelectionContainer(key, default!, default!, child);
        __instance.child = child;
        __instance.registrar = null;
        __instance.@delegate = null;
        return __instance;
    }

    public static global::Doroti.Framework.Rendering.SelectionRegistrar? maybeOf(BuildContext context)
    {
        SelectionRegistrarScope? scope = context.dependOnInheritedWidgetOfExactType<SelectionRegistrarScope>();
        return scope?.registrar;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _disabled => DartRuntimePrimitives.ConvertValue<bool>(@delegate is null);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectionContainerState__selection_container());
}

internal class _SelectionContainerState__selection_container : State<SelectionContainer>, global::Doroti.Framework.Rendering.Selectable, global::Doroti.Framework.Rendering.SelectionRegistrant
{
    internal virtual HashSet<global::System.Action> _listeners { get; private set; } = new HashSet<global::System.Action>();
    internal static global::Doroti.Framework.Rendering.SelectionGeometry _disabledGeometry = new global::Doroti.Framework.Rendering.SelectionGeometry(status: SelectionStatus.none, hasContent: true);
    public virtual SelectionRegistrar? _registrar { get; set; } = default;
    public virtual bool _subscribedToSelectionRegistrar { get; set; } = false;

    public override void initState()
    {
        base.initState();
        if (!widget._disabled)
        {
            widget.@delegate!._selectionContainerContext = context;
            if (widget.registrar is not null)
            {
                registrar = widget.registrar;
            }
        }
    }

    public override void didUpdateWidget(SelectionContainer oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.@delegate, widget.@delegate))
        {
            if (!oldWidget._disabled)
            {
                oldWidget.@delegate!._selectionContainerContext = null;
                _listeners.forEach((__arg0) => ((global::System.Action<global::System.Action>)oldWidget.@delegate!.removeListener)(__arg0));
            }
            if (!widget._disabled)
            {
                widget.@delegate!._selectionContainerContext = context;
                _listeners.forEach((__arg0) => ((global::System.Action<global::System.Action>)widget.@delegate!.addListener)(__arg0));
            }
            if (!Equals(oldWidget.@delegate?.value, widget.@delegate?.value))
            {
                foreach (global::System.Action listener in _listeners.ToList())
                {
                    listener();
                }
            }
        }
        if (widget._disabled)
        {
            registrar = null;
        }
        else
        {
            if (widget.registrar is not null)
            {
                registrar = widget.registrar;
            }
        }
        DartRuntimePrimitives.Assert(() => !widget._disabled || (registrar is null));
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if ((widget.registrar is null) && !widget._disabled)
        {
            registrar = SelectionContainer.maybeOf(context);
        }
        DartRuntimePrimitives.Assert(() => !widget._disabled || (registrar is null));
    }

    public virtual void addListener(global::System.Action listener)
    {
        DartRuntimePrimitives.Assert(() => !widget._disabled);
        widget.@delegate!.addListener(listener);
        _listeners.Add(() => listener());
    }

    public virtual void removeListener(global::System.Action listener)
    {
        widget.@delegate?.removeListener(listener);
        _listeners.Remove(listener);
    }

    public virtual void pushHandleLayers(global::Doroti.Framework.Rendering.LayerLink? startHandle, global::Doroti.Framework.Rendering.LayerLink? endHandle)
    {
        DartRuntimePrimitives.Assert(() => !widget._disabled);
        widget.@delegate!.pushHandleLayers(startHandle, endHandle);
    }

    public virtual global::Doroti.Framework.Rendering.SelectedContent? getSelectedContent()
    {
        DartRuntimePrimitives.Assert(() => !widget._disabled);
        return widget.@delegate!.getSelectedContent();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.SelectedContentRange? getSelection()
    {
        DartRuntimePrimitives.Assert(() => !widget._disabled);
        return widget.@delegate!.getSelection();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.SelectionResult dispatchSelectionEvent(global::Doroti.Framework.Rendering.SelectionEvent @event)
    {
        DartRuntimePrimitives.Assert(() => !widget._disabled);
        return widget.@delegate!.dispatchSelectionEvent(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.SelectionGeometry value
    {
        get
        {
            if (widget._disabled)
            {
                return _disabledGeometry;
            }
            return widget.@delegate!.value;
        }
    }
    public virtual Matrix4 getTransformTo(global::Doroti.Framework.Rendering.RenderObject? ancestor)
    {
        DartRuntimePrimitives.Assert(() => !widget._disabled);
        return context.findRenderObject()!.getTransformTo(ancestor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long contentLength => widget.@delegate!.contentLength;
    public virtual Size size => ((global::Doroti.Framework.Rendering.RenderBox?)context.findRenderObject()!)!.size;
    public virtual List<Rect> boundingBoxes => new List<global::Doroti.Ui.Rect> { ((global::Doroti.Framework.Rendering.RenderBox?)context.findRenderObject()!)!.paintBounds };
    public override void dispose()
    {
        if (!widget._disabled)
        {
            widget.@delegate!._selectionContainerContext = null;
            _listeners.forEach((__arg0) => ((global::System.Action<global::System.Action>)widget.@delegate!.removeListener)(__arg0));
        }
        _removeSelectionRegistrarSubscription();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (widget._disabled)
        {
            return SelectionRegistrarScope.Create_disabled(child: widget.child);
        }
        return new SelectionRegistrarScope(registrar: widget.@delegate!, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectionRegistrar? registrar
    {
        get => _registrar;
        set
        {
            var __value = value;
            if (Equals(__value, _registrar))
            {
                return;
            }
            if (__value is null)
            {
                removeListener(_updateSelectionRegistrarSubscription);
            }
            else
            {
                if (_registrar is null)
                {
                    addListener(_updateSelectionRegistrarSubscription);
                }
            }
            _removeSelectionRegistrarSubscription();
            _registrar = __value;
            _updateSelectionRegistrarSubscription();
        }
    }
    public virtual void _updateSelectionRegistrarSubscription()
    {
        if (_registrar is null)
        {
            _subscribedToSelectionRegistrar = false;
            return;
        }
        if (_subscribedToSelectionRegistrar && !value.hasContent)
        {
            _registrar!.remove(this);
            _subscribedToSelectionRegistrar = false;
        }
        else
        {
            if (!_subscribedToSelectionRegistrar && value.hasContent)
            {
                _registrar!.add(this);
                _subscribedToSelectionRegistrar = true;
            }
        }
    }

    public virtual void _removeSelectionRegistrarSubscription()
    {
        if (_subscribedToSelectionRegistrar)
        {
            _registrar!.remove(this);
            _subscribedToSelectionRegistrar = false;
        }
    }

}

public class SelectionRegistrarScope : InheritedWidget
{
    public virtual global::Doroti.Framework.Rendering.SelectionRegistrar? registrar { get; private set; }

    public SelectionRegistrarScope(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Rendering.SelectionRegistrar registrar = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.registrar = registrar;
    }

    public static SelectionRegistrarScope Create_disabled(Widget child)
    {
        var __instance = new SelectionRegistrarScope(default!, default!, child);
        __instance.registrar = null;
        return __instance;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (SelectionRegistrarScope)oldWidget;
        return !Equals(__oldWidget.registrar, registrar);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class SelectionContainerDelegate : global::Doroti.Framework.Rendering.SelectionHandler, global::Doroti.Framework.Rendering.SelectionRegistrar
{
    internal virtual BuildContext? _selectionContainerContext { get; set; } = default;

    public virtual void pushHandleLayers(global::Doroti.Framework.Rendering.LayerLink? startHandle, global::Doroti.Framework.Rendering.LayerLink? endHandle) => throw new NotSupportedException();
    public virtual global::Doroti.Framework.Rendering.SelectedContent? getSelectedContent() => throw new NotSupportedException();
    public virtual global::Doroti.Framework.Rendering.SelectedContentRange? getSelection() => throw new NotSupportedException();
    public virtual global::Doroti.Framework.Rendering.SelectionResult dispatchSelectionEvent(global::Doroti.Framework.Rendering.SelectionEvent @event) => throw new NotSupportedException();
    public virtual long contentLength => throw new NotSupportedException();
    public virtual global::Doroti.Framework.Rendering.SelectionGeometry value => throw new NotSupportedException();
    public virtual void add(global::Doroti.Framework.Rendering.Selectable selectable) => throw new NotSupportedException();
    public virtual void remove(global::Doroti.Framework.Rendering.Selectable selectable) => throw new NotSupportedException();
    public virtual Matrix4 getTransformFrom(global::Doroti.Framework.Rendering.Selectable child)
    {
        DartRuntimePrimitives.Assert(() => _selectionContainerContext?.findRenderObject() is not null, () => (object?)"getTransformFrom cannot be called before SelectionContainer is laid out.");
        return child.getTransformTo(((global::Doroti.Framework.Rendering.RenderBox?)_selectionContainerContext!.findRenderObject()!)!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Matrix4 getTransformTo(global::Doroti.Framework.Rendering.RenderObject? ancestor)
    {
        DartRuntimePrimitives.Assert(() => _selectionContainerContext?.findRenderObject() is not null, () => (object?)"getTransformTo cannot be called before SelectionContainer is laid out.");
        var box = ((global::Doroti.Framework.Rendering.RenderBox?)_selectionContainerContext!.findRenderObject()!)!;
        return box.getTransformTo(ancestor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hasSize
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _selectionContainerContext?.findRenderObject() is not null, () => (object?)"The _selectionContainerContext must have a renderObject, such as after the first build has completed.");
            var box = ((global::Doroti.Framework.Rendering.RenderBox?)_selectionContainerContext!.findRenderObject()!)!;
            return box.hasSize;
        }
    }
    public virtual global::Doroti.Ui.Size containerSize
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize, () => (object?)"containerSize cannot be called before SelectionContainer is laid out.");
            var box = ((global::Doroti.Framework.Rendering.RenderBox?)_selectionContainerContext!.findRenderObject()!)!;
            return box.size;
        }
    }
    private readonly HashSet<global::System.Action> __listeners = new();
    public virtual bool hasListeners => __listeners.Count != 0;
    public virtual void addListener(global::System.Action listener) => __listeners.Add(listener);
    public virtual void removeListener(global::System.Action listener) => __listeners.Remove(listener);
    public virtual void notifyListeners() { foreach (var listener in __listeners.ToArray()) listener(); }
    public virtual void dispose() => __listeners.Clear();
}

