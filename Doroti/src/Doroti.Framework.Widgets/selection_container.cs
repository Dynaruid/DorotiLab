// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/selection_container.dart
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

public class SelectionContainer : StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.SelectionRegistrar? registrar { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual SelectionContainerDelegate? @delegate { get; private set; }

    public SelectionContainer(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.SelectionRegistrar? registrar = null, SelectionContainerDelegate @delegate = default!, Widget child = default!) : base(key: key)
    {
        this.registrar = registrar;
        this.@delegate = @delegate;
        this.child = child;
    }

    public static SelectionContainer CreateDisabled(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!)
    {
        var __instance = new SelectionContainer(default!, default!, default!, default!);
        __instance.child = child;
        __instance.registrar = null;
        __instance.@delegate = null;
        return __instance;
    }

    public static global::Doroti.Generated.Framework.Rendering.SelectionRegistrar? maybeOf(BuildContext context)
    {
        SelectionRegistrarScope? scope__3536 = ((SelectionRegistrarScope?)(object?)context.dependOnInheritedWidgetOfExactType<SelectionRegistrarScope>());
        return scope__3536?.registrar;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _disabled => DartRuntimePrimitives.ConvertValue<bool>((this.@delegate is null));
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectionContainerState__selection_container());
}

internal class _SelectionContainerState__selection_container : State<SelectionContainer>, global::Doroti.Generated.Framework.Rendering.Selectable, global::Doroti.Generated.Framework.Rendering.SelectionRegistrant
{
    internal virtual HashSet<global::System.Action> _listeners { get; private set; } = new HashSet<global::System.Action>();
    internal static global::Doroti.Generated.Framework.Rendering.SelectionGeometry _disabledGeometry = new global::Doroti.Generated.Framework.Rendering.SelectionGeometry(status: global::Doroti.Generated.Framework.Rendering.SelectionStatus.none, hasContent: true);
    public virtual SelectionRegistrar? _registrar { get; set; } = default;
    public virtual bool _subscribedToSelectionRegistrar { get; set; } = false;

    public override void initState()
    {
        base.initState();
        if (!((SelectionContainer)this.widget)._disabled)
        {
            ((SelectionContainer)this.widget).@delegate!._selectionContainerContext = this.context;
            if ((((SelectionContainer)this.widget).registrar is not null))
            {
                registrar = ((SelectionContainer)this.widget).registrar;
            }
        }
    }

    public override void didUpdateWidget(SelectionContainer oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((SelectionContainer)oldWidget).@delegate, ((SelectionContainer)this.widget).@delegate)))
        {
            if (!((SelectionContainer)oldWidget)._disabled)
            {
                ((SelectionContainer)oldWidget).@delegate!._selectionContainerContext = null;
                this._listeners.forEach((__arg0) => ((global::System.Action<global::System.Action>)((global::System.Action<global::System.Action>)((dynamic)((SelectionContainer)oldWidget).@delegate!).removeListener))(__arg0));
            }
            if (!((SelectionContainer)this.widget)._disabled)
            {
                ((SelectionContainer)this.widget).@delegate!._selectionContainerContext = this.context;
                this._listeners.forEach((__arg0) => ((global::System.Action<global::System.Action>)((global::System.Action<global::System.Action>)((dynamic)((SelectionContainer)this.widget).@delegate!).addListener))(__arg0));
            }
            if ((!object.Equals(((global::Doroti.Generated.Framework.Rendering.SelectionGeometry?)((dynamic)((SelectionContainer)oldWidget).@delegate)?.value), ((global::Doroti.Generated.Framework.Rendering.SelectionGeometry?)((dynamic)((SelectionContainer)this.widget).@delegate)?.value))))
            {
                foreach (global::System.Action listener__4952 in this._listeners.ToList())
                {
                    listener__4952();
                }
            }
        }
        if (((SelectionContainer)this.widget)._disabled)
        {
            registrar = null;
        }
        else
        {
            if ((((SelectionContainer)this.widget).registrar is not null))
            {
                registrar = ((SelectionContainer)this.widget).registrar;
            }
        }
        DartRuntimePrimitives.Assert(() => (!((SelectionContainer)this.widget)._disabled || (this.registrar is null)));
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (((((SelectionContainer)this.widget).registrar is null) && !((SelectionContainer)this.widget)._disabled))
        {
            registrar = SelectionContainer.maybeOf(this.context);
        }
        DartRuntimePrimitives.Assert(() => (!((SelectionContainer)this.widget)._disabled || (this.registrar is null)));
    }

    public virtual void addListener(global::System.Action listener)
    {
        DartRuntimePrimitives.Assert(() => !((SelectionContainer)this.widget)._disabled);
        ((SelectionContainer)this.widget).@delegate!.addListener(() => listener());
        this._listeners.Add(() => listener());
    }

    public virtual void removeListener(global::System.Action listener)
    {
        ((SelectionContainer)this.widget).@delegate?.removeListener(() => listener());
        this._listeners.Remove(listener);
    }

    public virtual void pushHandleLayers(global::Doroti.Generated.Framework.Rendering.LayerLink? startHandle, global::Doroti.Generated.Framework.Rendering.LayerLink? endHandle)
    {
        DartRuntimePrimitives.Assert(() => !((SelectionContainer)this.widget)._disabled);
        ((SelectionContainer)this.widget).@delegate!.pushHandleLayers(startHandle, endHandle);
    }

    public virtual global::Doroti.Generated.Framework.Rendering.SelectedContent? getSelectedContent()
    {
        DartRuntimePrimitives.Assert(() => !((SelectionContainer)this.widget)._disabled);
        return ((global::Doroti.Generated.Framework.Rendering.SelectedContent?)(object?)((SelectionContainer)this.widget).@delegate!.getSelectedContent());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Rendering.SelectedContentRange? getSelection()
    {
        DartRuntimePrimitives.Assert(() => !((SelectionContainer)this.widget)._disabled);
        return ((global::Doroti.Generated.Framework.Rendering.SelectedContentRange?)(object?)((SelectionContainer)this.widget).@delegate!.getSelection());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Rendering.SelectionResult dispatchSelectionEvent(global::Doroti.Generated.Framework.Rendering.SelectionEvent @event)
    {
        DartRuntimePrimitives.Assert(() => !((SelectionContainer)this.widget)._disabled);
        return ((SelectionContainer)this.widget).@delegate!.dispatchSelectionEvent(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Rendering.SelectionGeometry value
    {
        get
        {
            if (((SelectionContainer)this.widget)._disabled)
            {
                return _SelectionContainerState__selection_container._disabledGeometry;
            }
            return ((global::Doroti.Generated.Framework.Rendering.SelectionGeometry)((dynamic)((SelectionContainer)this.widget).@delegate!).value);
            return default!;
        }
    }
    public virtual Matrix4 getTransformTo(global::Doroti.Generated.Framework.Rendering.RenderObject? ancestor)
    {
        DartRuntimePrimitives.Assert(() => !((SelectionContainer)this.widget)._disabled);
        return ((Matrix4)(object?)((Matrix4)((dynamic)this.context.findRenderObject()!).getTransformTo(ancestor)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long contentLength => ((long)((dynamic)((SelectionContainer)this.widget).@delegate!).contentLength);
    public virtual Size size => (((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!).size;
    public virtual List<Rect> boundingBoxes => new List<global::Doroti.Ui.Rect> { (((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!).paintBounds };
    public override void dispose()
    {
        if (!((SelectionContainer)this.widget)._disabled)
        {
            ((SelectionContainer)this.widget).@delegate!._selectionContainerContext = null;
            this._listeners.forEach((__arg0) => ((global::System.Action<global::System.Action>)((global::System.Action<global::System.Action>)((dynamic)((SelectionContainer)this.widget).@delegate!).removeListener))(__arg0));
        }
        _removeSelectionRegistrarSubscription();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (((SelectionContainer)this.widget)._disabled)
        {
            return ((Widget)(object?)SelectionRegistrarScope.Create_disabled(child: ((SelectionContainer)this.widget).child));
        }
        return ((Widget)(object?)new SelectionRegistrarScope(registrar: ((SelectionContainer)this.widget).@delegate!, child: ((SelectionContainer)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectionRegistrar? registrar
    {
        get => this._registrar;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._registrar)))
            {
                return;
            }
            if ((__value is null))
            {
                removeListener(() => this._updateSelectionRegistrarSubscription());
            }
            else
            {
                if ((this._registrar is null))
                {
                    addListener(() => this._updateSelectionRegistrarSubscription());
                }
            }
            _removeSelectionRegistrarSubscription();
            this._registrar = __value;
            _updateSelectionRegistrarSubscription();
        }
    }
    public virtual void _updateSelectionRegistrarSubscription()
    {
        if ((this._registrar is null))
        {
            this._subscribedToSelectionRegistrar = false;
            return;
        }
        if ((this._subscribedToSelectionRegistrar && !this.value.hasContent))
        {
            this._registrar!.remove(this);
            this._subscribedToSelectionRegistrar = false;
        }
        else
        {
            if ((!this._subscribedToSelectionRegistrar && this.value.hasContent))
            {
                this._registrar!.add(this);
                this._subscribedToSelectionRegistrar = true;
            }
        }
    }

    public virtual void _removeSelectionRegistrarSubscription()
    {
        if (this._subscribedToSelectionRegistrar)
        {
            this._registrar!.remove(this);
            this._subscribedToSelectionRegistrar = false;
        }
    }

}

public class SelectionRegistrarScope : InheritedWidget
{
    public virtual global::Doroti.Generated.Framework.Rendering.SelectionRegistrar? registrar { get; private set; }

    public SelectionRegistrarScope(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Rendering.SelectionRegistrar registrar = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.registrar = registrar;
    }

    public static SelectionRegistrarScope Create_disabled(Widget child)
    {
        var __instance = new SelectionRegistrarScope(default!, default!, default!);
        __instance.registrar = null;
        return __instance;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (SelectionRegistrarScope)(object)oldWidget;
        return (!object.Equals(((SelectionRegistrarScope)__oldWidget).registrar, this.registrar));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class SelectionContainerDelegate : global::Doroti.Generated.Framework.Rendering.SelectionHandler, global::Doroti.Generated.Framework.Rendering.SelectionRegistrar
{
    internal virtual BuildContext? _selectionContainerContext { get; set; } = default;

    public virtual void pushHandleLayers(global::Doroti.Generated.Framework.Rendering.LayerLink? startHandle, global::Doroti.Generated.Framework.Rendering.LayerLink? endHandle) => throw new NotSupportedException();
    public virtual global::Doroti.Generated.Framework.Rendering.SelectedContent? getSelectedContent() => throw new NotSupportedException();
    public virtual global::Doroti.Generated.Framework.Rendering.SelectedContentRange? getSelection() => throw new NotSupportedException();
    public virtual global::Doroti.Generated.Framework.Rendering.SelectionResult dispatchSelectionEvent(global::Doroti.Generated.Framework.Rendering.SelectionEvent @event) => throw new NotSupportedException();
    public virtual long contentLength => throw new NotSupportedException();
    public virtual global::Doroti.Generated.Framework.Rendering.SelectionGeometry value => throw new NotSupportedException();
    public virtual void add(global::Doroti.Generated.Framework.Rendering.Selectable selectable) => throw new NotSupportedException();
    public virtual void remove(global::Doroti.Generated.Framework.Rendering.Selectable selectable) => throw new NotSupportedException();
    public virtual Matrix4 getTransformFrom(global::Doroti.Generated.Framework.Rendering.Selectable child)
    {
        DartRuntimePrimitives.Assert(() => (this._selectionContainerContext?.findRenderObject() is not null), () => (object?)"getTransformFrom cannot be called before SelectionContainer is laid out.");
        return ((Matrix4)(object?)child.getTransformTo(((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this._selectionContainerContext!.findRenderObject()!)!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Matrix4 getTransformTo(global::Doroti.Generated.Framework.Rendering.RenderObject? ancestor)
    {
        DartRuntimePrimitives.Assert(() => (this._selectionContainerContext?.findRenderObject() is not null), () => (object?)"getTransformTo cannot be called before SelectionContainer is laid out.");
        var box__10212 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this._selectionContainerContext!.findRenderObject()!)!;
        return ((Matrix4)(object?)box__10212.getTransformTo(ancestor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hasSize
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this._selectionContainerContext?.findRenderObject() is not null), () => (object?)"The _selectionContainerContext must have a renderObject, such as after the first build has completed.");
            var box__10724 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this._selectionContainerContext!.findRenderObject()!)!;
            return ((global::Doroti.Generated.Framework.Rendering.RenderBox)box__10724).hasSize;
            return default!;
        }
    }
    public virtual global::Doroti.Ui.Size containerSize
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this.hasSize, () => (object?)"containerSize cannot be called before SelectionContainer is laid out.");
            var box__11089 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this._selectionContainerContext!.findRenderObject()!)!;
            return ((global::Doroti.Generated.Framework.Rendering.RenderBox)box__11089).size;
            return default!;
        }
    }
    private readonly HashSet<global::System.Action> __listeners = new();
    public virtual bool hasListeners => __listeners.Count != 0;
    public virtual void addListener(global::System.Action listener) => __listeners.Add(listener);
    public virtual void removeListener(global::System.Action listener) => __listeners.Remove(listener);
    public virtual void notifyListeners() { foreach (var listener in __listeners.ToArray()) listener(); }
    public virtual void dispose() => __listeners.Clear();
}

