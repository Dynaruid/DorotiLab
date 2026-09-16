// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/slotted_render_object_widget.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public abstract class SlottedMultiChildRenderObjectWidget<SlotType, ChildType> : RenderObjectWidget, SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> where SlotType : notnull where ChildType : global::Doroti.Framework.Rendering.RenderObject
{

    protected SlottedMultiChildRenderObjectWidget(global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public abstract IEnumerable<SlotType> slots { get; }
    public abstract Widget? childForSlot(SlotType slot);

    public override SlottedRenderObjectElement<SlotType, ChildType> createElement() => new SlottedRenderObjectElement<SlotType, ChildType>(this);
}

public interface SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> where SlotType : notnull where ChildType : global::Doroti.Framework.Rendering.RenderObject
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

    public override global::Doroti.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(((SlottedContainerRenderObjectMixin<SlotType, ChildType>?)base.renderObject)!);
    public override void visitChildren(global::System.Action<Element> visitor)
    {
        this._slotToChild.Values.forEach((__arg0) => ((global::System.Action<Element>)visitor)(__arg0));
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => this._slotToChild.containsValue(child));
        DartRuntimePrimitives.Assert(() => (((Element)child).slot is SlotType));
        DartRuntimePrimitives.Assert(() => this._slotToChild.ContainsKey((child.slot is SlotType childSlot ? childSlot : throw new ArgumentException("A slotted child requires its declared slot type.", nameof(child)))));
        this._slotToChild.remove((child.slot is SlotType childSlot ? childSlot : throw new ArgumentException("A slotted child requires its declared slot type.", nameof(child))));
        base.forgetChild(child);
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        _updateChildren();
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)newWidget;
        base.update((Widget)__newWidget);
        DartRuntimePrimitives.Assert(() => (Equals(this.widget, __newWidget)));
        _updateChildren();
    }

    internal virtual void _updateChildren()
    {
        var slottedMultiChildRenderObjectWidgetMixin = ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>?)this.widget)!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPreviousSlots ??= ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)slottedMultiChildRenderObjectWidgetMixin).slots.ToList();
                return CollectionsLibrary.listEquals(this._debugPreviousSlots, ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>)slottedMultiChildRenderObjectWidgetMixin).slots.ToList());
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
            Widget? widgetLocal = ((Widget?)slottedMultiChildRenderObjectWidgetMixin.childForSlot(slotLocal));
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
                    DartRuntimePrimitives.Assert(() => (!Equals(oldSlotChild!.widget.key, newWidgetKey)));
                    fromElement = null;
                }
            }
            Element? newChild = ((Element?)updateChild(fromElement, widgetLocal, slotLocal));
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
        var __child = (ChildType)child;
        var __slot = slot is SlotType typedslot ? typedslot : throw new ArgumentException("A slotted child requires its declared slot type.", nameof(slot));
        ((SlottedContainerRenderObjectMixin<SlotType, ChildType>)this.renderObject)._setChild(__child, __slot);
        DartRuntimePrimitives.Assert(() => EqualityComparer<ChildType>.Default.Equals(((SlottedContainerRenderObjectMixin<SlotType, ChildType>)this.renderObject)._slotToChild.GetValueOrDefault(__slot), __child));
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (ChildType)child;
        var __slot = slot is SlotType typedslot ? typedslot : throw new ArgumentException("A slotted child requires its declared slot type.", nameof(slot));
        if (EqualityComparer<ChildType>.Default.Equals(((SlottedContainerRenderObjectMixin<SlotType, ChildType>)this.renderObject)._slotToChild.GetValueOrDefault(__slot), __child))
        {
            ((SlottedContainerRenderObjectMixin<SlotType, ChildType>)this.renderObject)._setChild(default(ChildType)!, __slot);
            DartRuntimePrimitives.Assert(() => (!((SlottedContainerRenderObjectMixin<SlotType, ChildType>)this.renderObject)._slotToChild.ContainsKey(__slot)));
        }
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (ChildType)child;
        var __oldSlot = oldSlot is SlotType typedoldSlot ? typedoldSlot : throw new ArgumentException("A slotted child requires its declared slot type.", nameof(oldSlot));
        var __newSlot = newSlot is SlotType typednewSlot ? typednewSlot : throw new ArgumentException("A slotted child requires its declared slot type.", nameof(newSlot));
        ((SlottedContainerRenderObjectMixin<SlotType, ChildType>)this.renderObject)._moveChild(__child, __newSlot, __oldSlot);
    }

}
