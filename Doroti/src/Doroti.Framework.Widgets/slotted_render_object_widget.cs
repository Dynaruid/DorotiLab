// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/slotted_render_object_widget.dart
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

public abstract class SlottedMultiChildRenderObjectWidget<SlotType, ChildType> : RenderObjectWidget, SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> where ChildType : global::Doroti.Framework.Rendering.RenderObject
{

    protected SlottedMultiChildRenderObjectWidget(global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public abstract IEnumerable<SlotType> slots { get; }
    public abstract Widget? childForSlot(SlotType slot);

    public override SlottedRenderObjectElement<SlotType, ChildType> createElement() => new SlottedRenderObjectElement<SlotType, ChildType>(this);
}

public interface SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> where ChildType : global::Doroti.Framework.Rendering.RenderObject
{
    public IEnumerable<SlotType> slots { get; }
    public Widget? childForSlot(SlotType slot);
    public global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context);
    public void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject);
    public SlottedRenderObjectElement<SlotType, ChildType> createElement();
}

public interface SlottedContainerRenderObjectMixin<SlotType, ChildType> where SlotType : notnull where ChildType : global::Doroti.Framework.Rendering.RenderObject
{
    DartMap<SlotType, ChildType> _slotToChild { get; }

    public ChildType? childForSlot(SlotType slot);
    public IEnumerable<ChildType> children { get; }
    public string debugNameForSlot(SlotType slot);
    public void attach(global::Doroti.Framework.Rendering.PipelineOwner owner);
    public void detach();
    public void redepthChildren();
    public void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor);
    public List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren();
    public void _addDiagnostics(ChildType child, List<global::Doroti.Framework.Foundation.DiagnosticsNode> value, string name);
    public void _setChild(ChildType? child, SlotType slot);
    public void _moveChild(ChildType child, SlotType slot, SlotType oldSlot);
}

public class SlottedRenderObjectElement<SlotType, ChildType> : RenderObjectElement where SlotType : notnull where ChildType : global::Doroti.Framework.Rendering.RenderObject
{
    internal virtual DartMap<SlotType, Element> _slotToChild { get; set; } = new DartMap<SlotType, Element>();
    internal virtual DartMap<global::Doroti.Framework.Foundation.Key, Element> _keyedChildren { get; set; } = new DartMap<global::Doroti.Framework.Foundation.Key, Element>();
    internal virtual List<SlotType>? _debugPreviousSlots { get; set; } = default;

    public SlottedRenderObjectElement(SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> widget) : base((RenderObjectWidget)widget)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(((SlottedContainerRenderObjectMixin<SlotType, ChildType>?)(object?)base.renderObject)!);
    public override void visitChildren(global::System.Action<Element> visitor)
    {
        this._slotToChild.Values.forEach((__arg0) => ((global::System.Action<Element>)visitor)(__arg0));
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => this._slotToChild.containsValue(child));
        DartRuntimePrimitives.Assert(() => (((Element)child).slot is SlotType));
        DartRuntimePrimitives.Assert(() => this._slotToChild.ContainsKey(((SlotType)(object)((Element)child).slot)));
        this._slotToChild.remove(((SlotType)(object)((Element)child).slot));
        base.forgetChild(child);
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        _updateChildren();
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)(object)newWidget;
        base.update((Widget)__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        _updateChildren();
    }

    internal virtual void _updateChildren()
    {
        var slottedMultiChildRenderObjectWidgetMixin = ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>?)(object?)this.widget)!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPreviousSlots ??= ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)slottedMultiChildRenderObjectWidgetMixin).slots.ToList();
                return global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(this._debugPreviousSlots, ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)slottedMultiChildRenderObjectWidgetMixin).slots.ToList());
                throw new InvalidOperationException("Dart closure completed without a value.");
            }, () => (object?)$"{DartRuntimePrimitives.RuntimeType(this.widget)}.slots must not change.");
        DartRuntimePrimitives.Assert(() => (checked((long)(((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)slottedMultiChildRenderObjectWidgetMixin).slots.toSet().Count)) == ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)slottedMultiChildRenderObjectWidgetMixin).slots.Count()), () => (object?)"slots must be unique");
        DartMap<global::Doroti.Framework.Foundation.Key, Element> oldKeyedElements = this._keyedChildren;
        _keyedChildren = new DartMap<global::Doroti.Framework.Foundation.Key, Element>();
        DartMap<SlotType, Element> oldSlotToChild = this._slotToChild;
        _slotToChild = new DartMap<SlotType, Element>();
        DartMap<global::Doroti.Framework.Foundation.Key, List<Element>>? debugDuplicateKeys = default!;
        foreach (SlotType slotLocal in ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)slottedMultiChildRenderObjectWidgetMixin).slots)
        {
            Widget? widgetLocal = ((Widget?)(object?)slottedMultiChildRenderObjectWidgetMixin.childForSlot(slotLocal));
            global::Doroti.Framework.Foundation.Key? newWidgetKey = widgetLocal?.key;
            Element? oldSlotChild = oldSlotToChild.GetValueOrDefault(slotLocal);
            Element? oldKeyChild = oldKeyedElements.GetValueOrDefault(newWidgetKey);
            Element? fromElement = default!;
            if ((oldKeyChild is not null))
            {
                fromElement = oldSlotToChild.remove(((SlotType?)(object?)((Element)oldKeyChild).slot)!);
            }
            else
            {
                if ((oldSlotChild?.widget.key is null))
                {
                    fromElement = oldSlotToChild.remove(slotLocal);
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => (!object.Equals(oldSlotChild!.widget.key, newWidgetKey)));
                    fromElement = null;
                }
            }
            Element? newChild = ((Element?)(object?)updateChild(fromElement, widgetLocal, slotLocal));
            if ((newChild is not null))
            {
                this._slotToChild[slotLocal] = newChild;
                if ((newWidgetKey is not null))
                {
                    DartRuntimePrimitives.Assert(() =>
                        {
                            Element? existingElement = this._keyedChildren.GetValueOrDefault(newWidgetKey);
                            if ((existingElement is not null))
                            {
                                (debugDuplicateKeys ??= new DartMap<global::Doroti.Framework.Foundation.Key, List<Element>>()).putIfAbsent(newWidgetKey, (() => new List<Element> { existingElement })).Add(newChild);
                            }
                            return true;
                            throw new InvalidOperationException("Dart closure completed without a value.");
                        });
                    this._keyedChildren[DartRuntimePrimitives.RequireReference(newWidgetKey)] = newChild;
                }
            }
        }
        oldSlotToChild.Values.forEach((__arg0) => ((global::System.Action<Element>)this.deactivateChild)(__arg0));
        DartRuntimePrimitives.Assert(() => _debugDuplicateKeys(debugDuplicateKeys));
        DartRuntimePrimitives.Assert(() => this._keyedChildren.Values.All(this._slotToChild.Values.contains), () => (object?)$"_keyedChildren {this._keyedChildren.Values} should be a subset of {this._slotToChild.Values}");
    }

    internal virtual bool _debugDuplicateKeys(DartMap<global::Doroti.Framework.Foundation.Key, List<Element>>? debugDuplicateKeys)
    {
        if ((debugDuplicateKeys is null))
        {
            return true;
        }
        foreach (MapEntry<global::Doroti.Framework.Foundation.Key, List<Element>> duplicateKey in debugDuplicateKeys.entries)
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"Multiple widgets used the same key in {DartRuntimePrimitives.RuntimeType(this.widget)}."), new global::Doroti.Framework.Foundation.ErrorDescription($"The key {duplicateKey.key} was used by multiple widgets. The offending widgets were:\n"), new global::Doroti.Framework.Foundation.ErrorDescription("A key can only be specified on one widget at a time in the same parent widget.") }));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (ChildType)(object)child;
        var __slot = (SlotType)(object)slot;
        ((dynamic)this.renderObject)._setChild(__child, __slot);
        DartRuntimePrimitives.Assert(() => EqualityComparer<ChildType>.Default.Equals(((SlottedContainerRenderObjectMixin<SlotType, ChildType>)this.renderObject)._slotToChild.GetValueOrDefault(__slot), __child));
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (ChildType)(object)child;
        var __slot = (SlotType)(object)slot;
        if (EqualityComparer<ChildType>.Default.Equals(((SlottedContainerRenderObjectMixin<SlotType, ChildType>)this.renderObject)._slotToChild.GetValueOrDefault(__slot), __child))
        {
            ((dynamic)this.renderObject)._setChild(default(ChildType)!, __slot);
            DartRuntimePrimitives.Assert(() => (!((SlottedContainerRenderObjectMixin<SlotType, ChildType>)this.renderObject)._slotToChild.ContainsKey(__slot)));
        }
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (ChildType)(object)child;
        var __oldSlot = (SlotType)(object)oldSlot;
        var __newSlot = (SlotType)(object)newSlot;
        ((dynamic)this.renderObject)._moveChild(__child, __newSlot, __oldSlot);
    }

}
