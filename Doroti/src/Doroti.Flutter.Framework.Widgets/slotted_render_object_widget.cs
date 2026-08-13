// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/slotted_render_object_widget.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public abstract class SlottedMultiChildRenderObjectWidget<SlotType, ChildType> : RenderObjectWidget, SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> where ChildType : global::Doroti.Generated.Framework.Rendering.RenderObject
{

    protected SlottedMultiChildRenderObjectWidget(global::Doroti.Generated.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public abstract IEnumerable<SlotType> slots { get; }
    public abstract Widget? childForSlot(SlotType slot);

    public override SlottedRenderObjectElement<SlotType, ChildType> createElement() => new SlottedRenderObjectElement<SlotType, ChildType>(this);
}

public interface SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> where ChildType : global::Doroti.Generated.Framework.Rendering.RenderObject
{
    public IEnumerable<SlotType> slots { get; }
    public Widget? childForSlot(SlotType slot);
    public global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context);
    public void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject);
    public SlottedRenderObjectElement<SlotType, ChildType> createElement();
}

public interface SlottedContainerRenderObjectMixin<SlotType, ChildType> where SlotType : notnull where ChildType : global::Doroti.Generated.Framework.Rendering.RenderObject
{
    DartMap<SlotType, ChildType> _slotToChild { get; }

    public ChildType? childForSlot(SlotType slot);
    public IEnumerable<ChildType> children { get; }
    public string debugNameForSlot(SlotType slot);
    public void attach(global::Doroti.Generated.Framework.Rendering.PipelineOwner owner);
    public void detach();
    public void redepthChildren();
    public void visitChildren(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor);
    public List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren();
    public void _addDiagnostics(ChildType child, List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> value, string name);
    public void _setChild(ChildType? child, SlotType slot);
    public void _moveChild(ChildType child, SlotType slot, SlotType oldSlot);
}

public class SlottedRenderObjectElement<SlotType, ChildType> : RenderObjectElement where SlotType : notnull where ChildType : global::Doroti.Generated.Framework.Rendering.RenderObject
{
    internal virtual DartMap<SlotType, Element> _slotToChild { get; set; } = new DartMap<SlotType, Element>();
    internal virtual DartMap<global::Doroti.Generated.Framework.Foundation.Key, Element> _keyedChildren { get; set; } = new DartMap<global::Doroti.Generated.Framework.Foundation.Key, Element>();
    internal virtual List<SlotType>? _debugPreviousSlots { get; set; } = default;

    public SlottedRenderObjectElement(SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> widget) : base((RenderObjectWidget)widget)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(((SlottedContainerRenderObjectMixin<SlotType, ChildType>?)(object?)base.renderObject)!);
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
        var slottedMultiChildRenderObjectWidgetMixin__9884 = ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>?)(object?)this.widget)!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPreviousSlots ??= ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)slottedMultiChildRenderObjectWidgetMixin__9884).slots.ToList();
                return global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(this._debugPreviousSlots, ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)slottedMultiChildRenderObjectWidgetMixin__9884).slots.ToList());
                throw new InvalidOperationException("Dart closure completed without a value.");
            }, () => (object?)$"{DartRuntimePrimitives.RuntimeType(this.widget)}.slots must not change.");
        DartRuntimePrimitives.Assert(() => (checked((long)(((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)slottedMultiChildRenderObjectWidgetMixin__9884).slots.toSet().Count)) == ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)slottedMultiChildRenderObjectWidgetMixin__9884).slots.Count()), () => (object?)"slots must be unique");
        DartMap<global::Doroti.Generated.Framework.Foundation.Key, Element> oldKeyedElements__10511 = this._keyedChildren;
        _keyedChildren = new DartMap<global::Doroti.Generated.Framework.Foundation.Key, Element>();
        DartMap<SlotType, Element> oldSlotToChild__10618 = this._slotToChild;
        _slotToChild = new DartMap<SlotType, Element>();
        DartMap<global::Doroti.Generated.Framework.Foundation.Key, List<Element>>? debugDuplicateKeys__10721 = default!;
        foreach (SlotType slot__10766 in ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)slottedMultiChildRenderObjectWidgetMixin__9884).slots)
        {
            Widget? widget__10844 = ((Widget?)(object?)slottedMultiChildRenderObjectWidgetMixin__9884.childForSlot(slot__10766));
            global::Doroti.Generated.Framework.Foundation.Key? newWidgetKey__10931 = widget__10844?.key;
            Element? oldSlotChild__10981 = oldSlotToChild__10618.GetValueOrDefault(slot__10766);
            Element? oldKeyChild__11039 = oldKeyedElements__10511.GetValueOrDefault(newWidgetKey__10931);
            Element? fromElement__11267 = default!;
            if ((oldKeyChild__11039 is not null))
            {
                fromElement__11267 = oldSlotToChild__10618.remove(((SlotType?)(object?)((Element)oldKeyChild__11039).slot)!);
            }
            else
            {
                if ((oldSlotChild__10981?.widget.key is null))
                {
                    fromElement__11267 = oldSlotToChild__10618.remove(slot__10766);
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => (!object.Equals(oldSlotChild__10981!.widget.key, newWidgetKey__10931)));
                    fromElement__11267 = null;
                }
            }
            Element? newChild__11705 = ((Element?)(object?)updateChild(fromElement__11267, widget__10844, slot__10766));
            if ((newChild__11705 is not null))
            {
                this._slotToChild[slot__10766] = newChild__11705;
                if ((newWidgetKey__10931 is not null))
                {
                    DartRuntimePrimitives.Assert(() =>
                        {
                            Element? existingElement__11912 = this._keyedChildren.GetValueOrDefault(newWidgetKey__10931);
                            if ((existingElement__11912 is not null))
                            {
                                (debugDuplicateKeys__10721 ??= new DartMap<global::Doroti.Generated.Framework.Foundation.Key, List<Element>>()).putIfAbsent(newWidgetKey__10931, (() => new List<Element> { existingElement__11912 })).Add(newChild__11705);
                            }
                            return true;
                            throw new InvalidOperationException("Dart closure completed without a value.");
                        });
                    this._keyedChildren[DartRuntimePrimitives.RequireReference(newWidgetKey__10931)] = newChild__11705;
                }
            }
        }
        oldSlotToChild__10618.Values.forEach((__arg0) => ((global::System.Action<Element>)this.deactivateChild)(__arg0));
        DartRuntimePrimitives.Assert(() => _debugDuplicateKeys(debugDuplicateKeys__10721));
        DartRuntimePrimitives.Assert(() => this._keyedChildren.Values.All(this._slotToChild.Values.contains), () => (object?)$"_keyedChildren {this._keyedChildren.Values} should be a subset of {this._slotToChild.Values}");
    }

    internal virtual bool _debugDuplicateKeys(DartMap<global::Doroti.Generated.Framework.Foundation.Key, List<Element>>? debugDuplicateKeys)
    {
        if ((debugDuplicateKeys is null))
        {
            return true;
        }
        foreach (MapEntry<global::Doroti.Generated.Framework.Foundation.Key, List<Element>> duplicateKey__12777 in debugDuplicateKeys.entries)
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"Multiple widgets used the same key in {DartRuntimePrimitives.RuntimeType(this.widget)}."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"The key {duplicateKey__12777.key} was used by multiple widgets. The offending widgets were:\n"), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("A key can only be specified on one widget at a time in the same parent widget.") }));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void insertRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (ChildType)(object)child;
        var __slot = (SlotType)(object)slot;
        ((dynamic)this.renderObject)._setChild(__child, __slot);
        DartRuntimePrimitives.Assert(() => EqualityComparer<ChildType>.Default.Equals(((SlottedContainerRenderObjectMixin<SlotType, ChildType>)this.renderObject)._slotToChild.GetValueOrDefault(__slot), __child));
    }

    public override void removeRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (ChildType)(object)child;
        var __slot = (SlotType)(object)slot;
        if (EqualityComparer<ChildType>.Default.Equals(((SlottedContainerRenderObjectMixin<SlotType, ChildType>)this.renderObject)._slotToChild.GetValueOrDefault(__slot), __child))
        {
            ((dynamic)this.renderObject)._setChild(default(ChildType)!, __slot);
            DartRuntimePrimitives.Assert(() => (!((SlottedContainerRenderObjectMixin<SlotType, ChildType>)this.renderObject)._slotToChild.ContainsKey(__slot)));
        }
    }

    public override void moveRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (ChildType)(object)child;
        var __oldSlot = (SlotType)(object)oldSlot;
        var __newSlot = (SlotType)(object)newSlot;
        ((dynamic)this.renderObject)._moveChild(__child, __newSlot, __oldSlot);
    }

}
