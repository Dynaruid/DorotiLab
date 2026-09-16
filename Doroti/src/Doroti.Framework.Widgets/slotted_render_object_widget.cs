// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/slotted_render_object_widget.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public abstract class SlottedMultiChildRenderObjectWidget<SlotType, ChildType> : RenderObjectWidget, SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> where SlotType : notnull where ChildType : RenderObject
{

    protected SlottedMultiChildRenderObjectWidget(Key? key = null) : base(key: key)
    {
    }

    public abstract IEnumerable<SlotType> slots { get; }
    public abstract Widget? childForSlot(SlotType slot);

    public override SlottedRenderObjectElement<SlotType, ChildType> createElement() => new SlottedRenderObjectElement<SlotType, ChildType>(this);
}

public interface SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> where SlotType : notnull where ChildType : RenderObject
{
    public IEnumerable<SlotType> slots { get; }
    public Widget? childForSlot(SlotType slot);
    public RenderObject createRenderObject(BuildContext context);
    public void updateRenderObject(BuildContext context, RenderObject renderObject);
    public SlottedRenderObjectElement<SlotType, ChildType> createElement();
}

public interface SlottedContainerRenderObjectMixin<SlotType, ChildType> where SlotType : notnull where ChildType : RenderObject
{
    DartMap<SlotType, ChildType> _slotToChild { get; }

    public ChildType? childForSlot(SlotType slot);
    public IEnumerable<ChildType> children { get; }
    public string debugNameForSlot(SlotType slot);
    public void attach(PipelineOwner owner);
    public void detach();
    public void redepthChildren();
    public void visitChildren(System.Action<RenderObject> visitor);
    public List<DiagnosticsNode> debugDescribeChildren();
    public void _addDiagnostics(ChildType child, List<DiagnosticsNode> value, string name);
    public void _setChild(ChildType? child, SlotType slot);
    public void _moveChild(ChildType child, SlotType slot, SlotType oldSlot);
}

public class SlottedRenderObjectElement<SlotType, ChildType> : RenderObjectElement where SlotType : notnull where ChildType : RenderObject
{
    internal virtual DartMap<SlotType, Element> _slotToChild { get; set; } = new DartMap<SlotType, Element>();
    internal virtual DartMap<Key, Element> _keyedChildren { get; set; } = new DartMap<Key, Element>();
    internal virtual List<SlotType>? _debugPreviousSlots { get; set; } = default;

    public SlottedRenderObjectElement(SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType> widget) : base((RenderObjectWidget)widget)
    {
    }

    public override RenderObject renderObject => DartRuntimePrimitives.ConvertValue<RenderObject>(((SlottedContainerRenderObjectMixin<SlotType, ChildType>?)base.renderObject)!);
    public override void visitChildren(System.Action<Element> visitor)
    {
        _slotToChild.Values.forEach((__arg0) => visitor(__arg0));
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => _slotToChild.containsValue(child));
        DartRuntimePrimitives.Assert(() => child.slot is SlotType);
        DartRuntimePrimitives.Assert(() => _slotToChild.ContainsKey(child.slot is SlotType childSlot ? childSlot : throw new ArgumentException("A slotted child requires its declared slot type.", nameof(child))));
        _slotToChild.remove(child.slot is SlotType childSlot ? childSlot : throw new ArgumentException("A slotted child requires its declared slot type.", nameof(child)));
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
        DartRuntimePrimitives.Assert(() => Equals(widget, __newWidget));
        _updateChildren();
    }

    internal virtual void _updateChildren()
    {
        var slottedMultiChildRenderObjectWidgetMixin = ((SlottedMultiChildRenderObjectWidgetMixin<SlotType, ChildType>?)widget)!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPreviousSlots ??= slottedMultiChildRenderObjectWidgetMixin.slots.ToList();
                return CollectionsLibrary.listEquals(_debugPreviousSlots, slottedMultiChildRenderObjectWidgetMixin.slots.ToList());
                throw new InvalidOperationException("Dart closure completed without a value.");
            }, () => (object?)$"{DartRuntimePrimitives.RuntimeType(widget)}.slots must not change.");
        DartRuntimePrimitives.Assert(() => checked((long)slottedMultiChildRenderObjectWidgetMixin.slots.toSet().Count) == slottedMultiChildRenderObjectWidgetMixin.slots.Count(), () => (object?)"slots must be unique");
        DartMap<Key, Element> oldKeyedElements = _keyedChildren;
        _keyedChildren = new DartMap<Key, Element>();
        DartMap<SlotType, Element> oldSlotToChild = _slotToChild;
        _slotToChild = new DartMap<SlotType, Element>();
        DartMap<Key, List<Element>>? debugDuplicateKeys = default!;
        foreach (SlotType slotLocal in slottedMultiChildRenderObjectWidgetMixin.slots)
        {
            Widget? widgetLocal = slottedMultiChildRenderObjectWidgetMixin.childForSlot(slotLocal);
            Key? newWidgetKey = widgetLocal?.key;
            Element? oldSlotChild = oldSlotToChild.GetValueOrDefault(slotLocal);
            Element? oldKeyChild = oldKeyedElements.GetValueOrDefault(newWidgetKey);
            Element? fromElement = default!;
            if (oldKeyChild is not null)
            {
                fromElement = oldSlotToChild.remove(((SlotType?)oldKeyChild.slot)!);
            }
            else
            {
                if (oldSlotChild?.widget.key is null)
                {
                    fromElement = oldSlotToChild.remove(slotLocal);
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => !Equals(oldSlotChild!.widget.key, newWidgetKey));
                    fromElement = null;
                }
            }
            Element? newChild = updateChild(fromElement, widgetLocal, slotLocal);
            if (newChild is not null)
            {
                _slotToChild[slotLocal] = newChild;
                if (newWidgetKey is not null)
                {
                    DartRuntimePrimitives.Assert(() =>
                        {
                            Element? existingElement = _keyedChildren.GetValueOrDefault(newWidgetKey);
                            if (existingElement is not null)
                            {
                                (debugDuplicateKeys ??= new DartMap<Key, List<Element>>()).putIfAbsent(newWidgetKey, () => new List<Element> { existingElement }).Add(newChild);
                            }
                            return true;
                            throw new InvalidOperationException("Dart closure completed without a value.");
                        });
                    _keyedChildren[DartRuntimePrimitives.RequireReference(newWidgetKey)] = newChild;
                }
            }
        }
        oldSlotToChild.Values.forEach((__arg0) => ((System.Action<Element>)deactivateChild)(__arg0));
        DartRuntimePrimitives.Assert(() => _debugDuplicateKeys(debugDuplicateKeys));
        DartRuntimePrimitives.Assert(() => _keyedChildren.Values.All(_slotToChild.Values.contains), () => (object?)$"_keyedChildren {_keyedChildren.Values} should be a subset of {_slotToChild.Values}");
    }

    internal virtual bool _debugDuplicateKeys(DartMap<Key, List<Element>>? debugDuplicateKeys)
    {
        if (debugDuplicateKeys is null)
        {
            return true;
        }
        foreach (MapEntry<Key, List<Element>> duplicateKey in debugDuplicateKeys.entries)
        {
            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"Multiple widgets used the same key in {DartRuntimePrimitives.RuntimeType(widget)}."), new ErrorDescription($"The key {duplicateKey.key} was used by multiple widgets. The offending widgets were:\n"), new ErrorDescription("A key can only be specified on one widget at a time in the same parent widget.") }));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        var __child = (ChildType)child;
        var __slot = slot is SlotType typedslot ? typedslot : throw new ArgumentException("A slotted child requires its declared slot type.", nameof(slot));
        ((SlottedContainerRenderObjectMixin<SlotType, ChildType>)renderObject)._setChild(__child, __slot);
        DartRuntimePrimitives.Assert(() => EqualityComparer<ChildType>.Default.Equals(((SlottedContainerRenderObjectMixin<SlotType, ChildType>)renderObject)._slotToChild.GetValueOrDefault(__slot), __child));
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        var __child = (ChildType)child;
        var __slot = slot is SlotType typedslot ? typedslot : throw new ArgumentException("A slotted child requires its declared slot type.", nameof(slot));
        if (EqualityComparer<ChildType>.Default.Equals(((SlottedContainerRenderObjectMixin<SlotType, ChildType>)renderObject)._slotToChild.GetValueOrDefault(__slot), __child))
        {
            ((SlottedContainerRenderObjectMixin<SlotType, ChildType>)renderObject)._setChild(default(ChildType)!, __slot);
            DartRuntimePrimitives.Assert(() => !((SlottedContainerRenderObjectMixin<SlotType, ChildType>)renderObject)._slotToChild.ContainsKey(__slot));
        }
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (ChildType)child;
        var __oldSlot = oldSlot is SlotType typedoldSlot ? typedoldSlot : throw new ArgumentException("A slotted child requires its declared slot type.", nameof(oldSlot));
        var __newSlot = newSlot is SlotType typednewSlot ? typednewSlot : throw new ArgumentException("A slotted child requires its declared slot type.", nameof(newSlot));
        ((SlottedContainerRenderObjectMixin<SlotType, ChildType>)renderObject)._moveChild(__child, __newSlot, __oldSlot);
    }

}
