// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public abstract class SliverWithKeepAliveWidget : RenderObjectWidget
{
    protected SliverWithKeepAliveWidget(Key? key = null) : base(key: key)
    {
    }

    public abstract override RenderObject createRenderObject(BuildContext context);
}

public abstract class SliverMultiBoxAdaptorWidget : SliverWithKeepAliveWidget
{
    public virtual SliverChildDelegate @delegate { get; private set; } = default!;

    protected SliverMultiBoxAdaptorWidget(Key? key = null, SliverChildDelegate @delegate = default!) : base(key: key)
    {
        this.@delegate = @delegate;
    }

    public override SliverMultiBoxAdaptorElement createElement() => new SliverMultiBoxAdaptorElement(this);
    public abstract override RenderObject createRenderObject(BuildContext context);
    public virtual double? estimateMaxScrollOffset(SliverConstraints? constraints, long firstIndex, long lastIndex, double leadingScrollOffset, double trailingScrollOffset)
    {
        DartRuntimePrimitives.Assert(() => lastIndex >= firstIndex);
        return @delegate.estimateMaxScrollOffset(firstIndex, lastIndex, leadingScrollOffset, trailingScrollOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<SliverChildDelegate>("delegate", @delegate));
    }

}

public class SliverList : SliverMultiBoxAdaptorWidget
{
    public SliverList(Key? key = null, SliverChildDelegate @delegate = default!) : base(key: key, @delegate: @delegate)
    {
    }

    public static SliverList CreateBuilder(Key? key = null, Func<BuildContext, long, Widget?> itemBuilder = default!, Func<Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, long semanticIndexOffset = 0)
    {
        return new SliverList(key, new SliverChildBuilderDelegate(
            itemBuilder,
            findChildIndexCallback,
            itemCount,
            addAutomaticKeepAlives,
            addRepaintBoundaries,
            addSemanticIndexes,
            semanticIndexOffset: semanticIndexOffset));
    }

    public static SliverList CreateSeparated(Key? key = null, Func<BuildContext, long, Widget?> itemBuilder = default!, Func<Key, long?>? findChildIndexCallback = null, Func<Key, long?>? findItemIndexCallback = null, Func<BuildContext, long, Widget?> separatorBuilder = default!, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true)
    {
        return new SliverList(key, new SliverChildBuilderDelegate(
            (context, index) => (index & 1L) == 0L
                ? itemBuilder(context, index / 2L)
                : separatorBuilder(context, index / 2L),
            findChildIndexCallback: findItemIndexCallback is null
                ? findChildIndexCallback
                : childKey => findItemIndexCallback(childKey) is { } itemIndex ? itemIndex * 2L : null,
            childCount: itemCount is { } count ? Math.Max(0L, count * 2L - 1L) : null,
            addAutomaticKeepAlives: addAutomaticKeepAlives,
            addRepaintBoundaries: addRepaintBoundaries,
            addSemanticIndexes: addSemanticIndexes,
            semanticIndexCallback: (_, index) => (index & 1L) == 0L ? index / 2L : null));
    }

    public static SliverList CreateList(Key? key = null, List<Widget> children = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true)
    {
        return new SliverList(key, new SliverChildListDelegate(
            children ?? [],
            addAutomaticKeepAlives: addAutomaticKeepAlives,
            addRepaintBoundaries: addRepaintBoundaries,
            addSemanticIndexes: addSemanticIndexes));
    }

    public override SliverMultiBoxAdaptorElement createElement() => new SliverMultiBoxAdaptorElement(this, replaceMovedChildren: true);
    public override RenderObject createRenderObject(BuildContext context)
    {
        var element = ((SliverMultiBoxAdaptorElement?)context)!;
        return new RenderSliverList(childManager: element);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverFixedExtentList : SliverMultiBoxAdaptorWidget
{
    public virtual double itemExtent { get; private set; } = default!;

    public SliverFixedExtentList(Key? key = null, SliverChildDelegate @delegate = default!, double itemExtent = default!) : base(key: key, @delegate: @delegate)
    {
        this.itemExtent = itemExtent;
    }

    public static SliverFixedExtentList CreateBuilder(Key? key = null, Func<BuildContext, long, Widget?> itemBuilder = default!, double itemExtent = default!, Func<Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, long semanticIndexOffset = 0)
    {
        return new SliverFixedExtentList(key, new SliverChildBuilderDelegate(
            itemBuilder,
            findChildIndexCallback,
            itemCount,
            addAutomaticKeepAlives,
            addRepaintBoundaries,
            addSemanticIndexes,
            semanticIndexOffset: semanticIndexOffset), itemExtent);
    }

    public static SliverFixedExtentList CreateList(Key? key = null, List<Widget> children = default!, double itemExtent = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true)
    {
        return new SliverFixedExtentList(key, new SliverChildListDelegate(
            children ?? [],
            addAutomaticKeepAlives: addAutomaticKeepAlives,
            addRepaintBoundaries: addRepaintBoundaries,
            addSemanticIndexes: addSemanticIndexes), itemExtent);
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        var element = ((SliverMultiBoxAdaptorElement?)context)!;
        return new RenderSliverFixedExtentList(childManager: element, itemExtent: itemExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSliverFixedExtentList)renderObject;
        __renderObject.itemExtent = itemExtent;
    }

}

public class SliverVariedExtentList : SliverMultiBoxAdaptorWidget
{
    public virtual ItemExtentBuilder itemExtentBuilder { get; private set; } = default!;

    public SliverVariedExtentList(Key? key = null, SliverChildDelegate @delegate = default!, ItemExtentBuilder itemExtentBuilder = default!) : base(key: key, @delegate: @delegate)
    {
        this.itemExtentBuilder = itemExtentBuilder;
    }

    public static SliverVariedExtentList CreateBuilder(Key? key = null, Func<BuildContext, long, Widget?> itemBuilder = default!, ItemExtentBuilder itemExtentBuilder = default!, Func<Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true)
    {
        return new SliverVariedExtentList(key, new SliverChildBuilderDelegate(
            itemBuilder,
            findChildIndexCallback,
            itemCount,
            addAutomaticKeepAlives,
            addRepaintBoundaries,
            addSemanticIndexes), itemExtentBuilder);
    }

    public static SliverVariedExtentList CreateList(Key? key = null, List<Widget> children = default!, ItemExtentBuilder itemExtentBuilder = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true)
    {
        return new SliverVariedExtentList(key, new SliverChildListDelegate(
            children ?? [],
            addAutomaticKeepAlives: addAutomaticKeepAlives,
            addRepaintBoundaries: addRepaintBoundaries,
            addSemanticIndexes: addSemanticIndexes), itemExtentBuilder);
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        var element = ((SliverMultiBoxAdaptorElement?)context)!;
        return new RenderSliverVariedExtentList(childManager: element, itemExtentBuilder: itemExtentBuilder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSliverVariedExtentList)renderObject;
        __renderObject.itemExtentBuilder = itemExtentBuilder;
    }

}

public class SliverGrid : SliverMultiBoxAdaptorWidget
{
    public virtual SliverGridDelegate gridDelegate { get; private set; } = default!;

    public SliverGrid(Key? key = null, SliverChildDelegate @delegate = default!, SliverGridDelegate gridDelegate = default!) : base(key: key, @delegate: @delegate)
    {
        this.gridDelegate = gridDelegate;
    }

    public static SliverGrid CreateBuilder(Key? key = null, SliverGridDelegate gridDelegate = default!, Func<BuildContext, long, Widget?> itemBuilder = default!, Func<Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, long semanticIndexOffset = 0)
    {
        return new SliverGrid(key, new SliverChildBuilderDelegate(
            itemBuilder,
            findChildIndexCallback,
            itemCount,
            addAutomaticKeepAlives,
            addRepaintBoundaries,
            addSemanticIndexes,
            semanticIndexOffset: semanticIndexOffset), gridDelegate);
    }

    public static SliverGrid CreateCount(Key? key = null, long crossAxisCount = default!, double mainAxisSpacing = 0.0, double crossAxisSpacing = 0.0, double childAspectRatio = 1.0, List<Widget> children = default!)
    {
        return new SliverGrid(key, new SliverChildListDelegate(children ?? []),
            new SliverGridDelegateWithFixedCrossAxisCount(crossAxisCount: crossAxisCount, mainAxisSpacing: mainAxisSpacing, crossAxisSpacing: crossAxisSpacing, childAspectRatio: childAspectRatio));
    }

    public static SliverGrid CreateExtent(Key? key = null, double maxCrossAxisExtent = default!, double mainAxisSpacing = 0.0, double crossAxisSpacing = 0.0, double childAspectRatio = 1.0, List<Widget> children = default!)
    {
        return new SliverGrid(key, new SliverChildListDelegate(children ?? []),
            new SliverGridDelegateWithMaxCrossAxisExtent(maxCrossAxisExtent: maxCrossAxisExtent, mainAxisSpacing: mainAxisSpacing, crossAxisSpacing: crossAxisSpacing, childAspectRatio: childAspectRatio));
    }

    public static SliverGrid CreateList(Key? key = null, SliverGridDelegate gridDelegate = default!, List<Widget> children = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, long semanticIndexOffset = 0)
    {
        return new SliverGrid(key, new SliverChildListDelegate(
            children ?? [],
            addAutomaticKeepAlives: addAutomaticKeepAlives,
            addRepaintBoundaries: addRepaintBoundaries,
            addSemanticIndexes: addSemanticIndexes,
            semanticIndexOffset: semanticIndexOffset), gridDelegate);
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        var element = ((SliverMultiBoxAdaptorElement?)context)!;
        return new RenderSliverGrid(childManager: element, gridDelegate: gridDelegate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSliverGrid)renderObject;
        __renderObject.gridDelegate = gridDelegate;
    }

    public override double? estimateMaxScrollOffset(SliverConstraints? constraints, long firstIndex, long lastIndex, double leadingScrollOffset, double trailingScrollOffset)
    {
        return base.estimateMaxScrollOffset(constraints, firstIndex, lastIndex, leadingScrollOffset, trailingScrollOffset) ?? (double)gridDelegate.getLayout(constraints!).computeMaxScrollOffset(DartRuntimePrimitives.RequireValue(@delegate.estimatedChildCount));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverMultiBoxAdaptorElement : RenderObjectElement, RenderSliverBoxChildManager
{
    internal virtual bool _replaceMovedChildren { get; private set; } = default!;
    internal virtual SortedDictionary<long, Element?> _childElements { get; private set; } = new SortedDictionary<long, Element?>();
    internal virtual RenderBox? _currentBeforeChild { get; set; } = default;
    internal virtual long? _currentlyUpdatingChildIndex { get; set; } = default;
    internal virtual bool _didUnderflow { get; set; } = false;

    public SliverMultiBoxAdaptorElement(SliverMultiBoxAdaptorWidget widget, bool replaceMovedChildren = false) : base(widget)
    {
        _replaceMovedChildren = replaceMovedChildren;
    }

    public override RenderSliverMultiBoxAdaptor renderObject => (RenderSliverMultiBoxAdaptor)base.renderObject;
    public override void update(Widget newWidget)
    {
        var __newWidget = (SliverMultiBoxAdaptorWidget)newWidget;
        var oldWidget = ((SliverMultiBoxAdaptorWidget?)widget)!;
        base.update(__newWidget);
        FrameworkWorkCounters.Add(FrameworkWork.DelegateUpdate);
        SliverChildDelegate newDelegate = __newWidget.@delegate;
        SliverChildDelegate oldDelegate = oldWidget.@delegate;
        if ((!Equals(newDelegate, oldDelegate)) && ((!Equals(DartRuntimePrimitives.RuntimeType(newDelegate), DartRuntimePrimitives.RuntimeType(oldDelegate))) || newDelegate.shouldRebuild(oldDelegate)))
        {
            performRebuild();
        }
    }

    public override void performRebuild()
    {
        base.performRebuild();
        FrameworkWorkCounters.Add(FrameworkWork.DelegateRebuild);
        _currentBeforeChild = null;
        var childrenUpdated = false;
        DartRuntimePrimitives.Assert(() => _currentlyUpdatingChildIndex is null);
        try
        {
            var newChildren = new SortedDictionary<long, Element?>();
            DartMap<long, double> indexToLayoutOffset = new DartMap<long, double>();
            var adaptorWidget = ((SliverMultiBoxAdaptorWidget?)widget)!;
            void processElement(long index)
            {
                _currentlyUpdatingChildIndex = index;
                if (_childElements.ContainsKey(index) && (!Equals(_childElements.GetValueOrDefault(index), newChildren.GetValueOrDefault(index))))
                {
                    _childElements[index] = updateChild(_childElements.GetValueOrDefault(index), null, index);
                    childrenUpdated = true;
                }
                Element? newChild = updateChild(newChildren.GetValueOrDefault(index), _build(index, adaptorWidget), index);
                if (newChild is not null)
                {
                    childrenUpdated = childrenUpdated || (!Equals(_childElements.GetValueOrDefault(index), newChild));
                    _childElements[index] = newChild;
                    var parentDataLocal = ((SliverMultiBoxAdaptorParentData?)newChild.renderObject!.parentData!)!;
                    if (index == 0L)
                    {
                        parentDataLocal.layoutOffset = 0.0;
                    }
                    else
                    {
                        if (indexToLayoutOffset.ContainsKey(index))
                        {
                            parentDataLocal.layoutOffset = DartCollectionRuntime.NullableMapValue<double>(indexToLayoutOffset, index);
                        }
                    }
                    if (!parentDataLocal.keptAlive)
                    {
                        _currentBeforeChild = ((RenderBox?)newChild.renderObject)!;
                    }
                }
                else
                {
                    childrenUpdated = true;
                    _childElements.Remove(index);
                }
            }
            foreach (long indexLocal in _childElements.Keys.ToList())
            {
                FrameworkWorkCounters.Add(FrameworkWork.RetainedChildVisit);
                Key? keyLocal = _childElements.GetValueOrDefault(indexLocal)!.widget.key;
                long? newIndex = (keyLocal is null) ? null : adaptorWidget.@delegate.findIndexByKey(keyLocal);
                var childParentData = ((SliverMultiBoxAdaptorParentData?)(_childElements.GetValueOrDefault(indexLocal)!.renderObject?.parentData))!;
                if ((childParentData is not null) && (childParentData.layoutOffset is not null))
                {
                    indexToLayoutOffset[indexLocal] = DartRuntimePrimitives.RequireValue(childParentData.layoutOffset);
                }
                if ((newIndex is not null) && (DartRuntimePrimitives.RequireValue(newIndex) != indexLocal))
                {
                    long newIndex__39285__value39663 = DartRuntimePrimitives.RequireValue(newIndex);
                    if (childParentData is not null)
                    {
                        childParentData.layoutOffset = null;
                    }
                    newChildren[DartRuntimePrimitives.RequireValue(newIndex__39285__value39663)] = _childElements.GetValueOrDefault(indexLocal);
                    if (_replaceMovedChildren)
                    {
                        newChildren.putIfAbsent(indexLocal, () => default!);
                    }
                    _childElements.Remove(indexLocal);
                }
                else
                {
                    newChildren.putIfAbsent(indexLocal, () => _childElements.GetValueOrDefault(indexLocal));
                }
            }
            renderObject.debugChildIntegrityEnabled = false;
            newChildren.Keys.forEach((__arg0) => ((System.Action<long>)processElement)(__arg0));
            if (!childrenUpdated && _didUnderflow)
            {
                long lastKey = DartCollectionRuntime.LastKeyOrNull(_childElements) ?? -1L;
                long rightBoundary = lastKey + 1L;
                newChildren[rightBoundary] = _childElements.GetValueOrDefault(rightBoundary);
                processElement(rightBoundary);
            }
        }
        finally
        {
            _currentlyUpdatingChildIndex = null;
            renderObject.debugChildIntegrityEnabled = true;
        }
    }

    internal virtual Widget? _build(long index, SliverMultiBoxAdaptorWidget widget)
    {
        return widget.@delegate.build(this, index);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void createChild(long index, RenderBox? after)
    {
        DartRuntimePrimitives.Assert(() => _currentlyUpdatingChildIndex is null);
        owner!.buildScope(this, () =>
        {
            var insertFirst = after is null;
            DartRuntimePrimitives.Assert(() => insertFirst || _childElements.ContainsKey(index - 1L));
            _currentBeforeChild = insertFirst ? null : ((RenderBox?)_childElements.GetValueOrDefault(index - 1L)!.renderObject)!;
            Element? newChild = default!;
            try
            {
                var adaptorWidget = ((SliverMultiBoxAdaptorWidget?)widget)!;
                _currentlyUpdatingChildIndex = index;
                newChild = updateChild(_childElements.GetValueOrDefault(index), _build(index, adaptorWidget), index);
            }
            finally
            {
                _currentlyUpdatingChildIndex = null;
            }
            if (newChild is not null)
            {
                _childElements[index] = newChild;
            }
            else
            {
                _childElements.Remove(index);
            }
        });
    }

    public override Element? updateChild(Element? child, Widget? newWidget, object? newSlot)
    {
        var oldParentData = ((SliverMultiBoxAdaptorParentData?)((child?.renderObject)?.parentData))!;
        Element? newChild = base.updateChild(child, newWidget, newSlot);
        var newParentData = ((SliverMultiBoxAdaptorParentData?)((newChild?.renderObject)?.parentData))!;
        if ((!Equals(oldParentData, newParentData)) && (oldParentData is not null) && (newParentData is not null))
        {
            newParentData.layoutOffset = oldParentData.layoutOffset;
        }
        return newChild;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => child.slot is not null);
        DartRuntimePrimitives.Assert(() => _childElements.ContainsKey(DartRuntimePrimitives.ConvertValue<long>(child.slot)));
        _childElements.Remove(DartRuntimePrimitives.ConvertValue<long>(child.slot));
        base.forgetChild(child);
    }

    public virtual void removeChild(RenderBox child)
    {
        long index = DartRuntimePrimitives.ConvertValue<long>(renderObject.indexOf(child));
        DartRuntimePrimitives.Assert(() => _currentlyUpdatingChildIndex is null);
        DartRuntimePrimitives.Assert(() => index >= 0L);
        owner!.buildScope(this, () =>
        {
            DartRuntimePrimitives.Assert(() => _childElements.ContainsKey(index));
            try
            {
                _currentlyUpdatingChildIndex = index;
                Element? result = updateChild(_childElements.GetValueOrDefault(index), null, index);
                DartRuntimePrimitives.Assert(() => result is null);
            }
            finally
            {
                _currentlyUpdatingChildIndex = null;
            }
            _childElements.Remove(index);
            DartRuntimePrimitives.Assert(() => !_childElements.ContainsKey(index));
        });
    }

    internal static double _extrapolateMaxScrollOffset(long firstIndex, long lastIndex, double leadingScrollOffset, double trailingScrollOffset, long childCount)
    {
        if (DartRuntimePrimitives.RequireValue(lastIndex) == (childCount - 1L))
        {
            return DartRuntimePrimitives.RequireValue(trailingScrollOffset);
        }
        long reifiedCount = DartRuntimePrimitives.RequireValue(lastIndex) - DartRuntimePrimitives.RequireValue(firstIndex) + 1L;
        double averageExtent = (DartRuntimePrimitives.RequireValue(trailingScrollOffset) - DartRuntimePrimitives.RequireValue(leadingScrollOffset)) / reifiedCount;
        long remainingCount = childCount - DartRuntimePrimitives.RequireValue(lastIndex) - 1L;
        return DartRuntimePrimitives.RequireValue(trailingScrollOffset) + (averageExtent * remainingCount);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double estimateMaxScrollOffset(SliverConstraints constraints, long? firstIndex = null, long? lastIndex = null, double? leadingScrollOffset = null, double? trailingScrollOffset = null)
    {
        long? childCount = estimatedChildCount;
        if (childCount is null)
        {
            return double.PositiveInfinity;
        }
        return ((SliverMultiBoxAdaptorWidget?)widget)!.estimateMaxScrollOffset(constraints, DartRuntimePrimitives.RequireValue(firstIndex), DartRuntimePrimitives.RequireValue(lastIndex), DartRuntimePrimitives.RequireValue(leadingScrollOffset), DartRuntimePrimitives.RequireValue(trailingScrollOffset)) ?? (double)_extrapolateMaxScrollOffset(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndex)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(lastIndex)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(leadingScrollOffset)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(trailingScrollOffset)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(childCount)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long? estimatedChildCount => ((SliverMultiBoxAdaptorWidget?)widget)!.@delegate.estimatedChildCount;
    public virtual long childCount
    {
        get
        {
            long? result = estimatedChildCount;
            if (result is null)
            {
                var lo = 0L;
                var hi = 1L;
                var adaptorWidget = ((SliverMultiBoxAdaptorWidget?)widget)!;
                long max = Foundation.ConstantsLibrary.kIsWeb ? 9007199254740992L : long.MaxValue;
                while (_build(hi - 1L, adaptorWidget) is not null)
                {
                    lo = hi - 1L;
                    if (hi < checked(max / 2L))
                    {
                        hi *= 2L;
                    }
                    else
                    {
                        if (hi < max)
                        {
                            hi = max;
                        }
                        else
                        {
                            throw DartRuntimePrimitives.AsException(FlutterError.Create($"Could not find the number of children in {adaptorWidget.@delegate}.\n" + "The childCount getter was called (implying that the delegate's builder returned null " + $"for a positive index), but even building the child with index {hi} (the maximum " + "possible integer) did not return null. Consider implementing childCount to avoid " + "the cost of searching for the final child."));
                        }
                    }
                }
                while ((hi - lo) > 1L)
                {
                    long mid = checked((hi - lo) / 2L) + lo;
                    if (_build(mid - 1L, adaptorWidget) is null)
                    {
                        hi = mid;
                    }
                    else
                    {
                        lo = mid;
                    }
                }
                result = lo;
            }
            return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(result));
        }
    }
    public virtual void didStartLayout()
    {
        DartRuntimePrimitives.Assert(() => debugAssertChildListLocked());
    }

    public virtual void didFinishLayout()
    {
        DartRuntimePrimitives.Assert(() => debugAssertChildListLocked());
        long firstIndex = DartCollectionRuntime.FirstKeyOrNull(_childElements) ?? 0L;
        long lastIndex = DartCollectionRuntime.LastKeyOrNull(_childElements) ?? 0L;
        ((SliverMultiBoxAdaptorWidget?)widget)!.@delegate.didFinishLayout(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndex)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(lastIndex)));
    }

    public virtual bool debugAssertChildListLocked()
    {
        DartRuntimePrimitives.Assert(() => _currentlyUpdatingChildIndex is null);
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didAdoptChild(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => _currentlyUpdatingChildIndex is not null);
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        childParentData.index = _currentlyUpdatingChildIndex;
    }

    public virtual void setDidUnderflow(bool value)
    {
        _didUnderflow = value;
    }

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        long __slot = DartRuntimePrimitives.ConvertValue<long>(slot);
        DartRuntimePrimitives.Assert(() => _currentlyUpdatingChildIndex == __slot);
        DartRuntimePrimitives.Assert(() => renderObject.debugValidateChild(child));
        renderObject.insert(((RenderBox?)child)!, after: _currentBeforeChild);
        DartRuntimePrimitives.Assert(() =>
            {
                var childParentData = ((SliverMultiBoxAdaptorParentData?)((RenderBox)child).parentData!)!;
                DartRuntimePrimitives.Assert(() => __slot == childParentData.index);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        long __oldSlot = DartRuntimePrimitives.ConvertValue<long>(oldSlot);
        long __newSlot = DartRuntimePrimitives.ConvertValue<long>(newSlot);
        DartRuntimePrimitives.Assert(() => _currentlyUpdatingChildIndex == DartRuntimePrimitives.RequireValue(__newSlot));
        renderObject.move(((RenderBox?)child)!, after: _currentBeforeChild);
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        long __slot = DartRuntimePrimitives.ConvertValue<long>(slot);
        DartRuntimePrimitives.Assert(() => _currentlyUpdatingChildIndex is not null);
        renderObject.remove(((RenderBox?)child)!);
    }

    public override void visitChildren(System.Action<Element> visitor)
    {
        DartRuntimePrimitives.Assert(() => !_childElements.Values.any((child) => child is null));
        _childElements.Values.cast<Element>().ToList().forEach((__arg0) => visitor(__arg0));
    }

    public override void debugVisitOnstageChildren(System.Action<Element> visitor)
    {
        _childElements.Values.cast<Element>().where((child) =>
        {
            var parentDataLocal = ((SliverMultiBoxAdaptorParentData?)child.renderObject!.parentData!)!;
            double itemExtent = DartRuntimePrimitives.ConvertValue<double>(renderObject.constraints.axis switch { Axis.horizontal => child.renderObject!.paintBounds.width, Axis.vertical => child.renderObject!.paintBounds.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return (parentDataLocal.layoutOffset is not null) && (DartRuntimePrimitives.RequireValue(parentDataLocal.layoutOffset) < (renderObject.constraints.scrollOffset + renderObject.constraints.remainingPaintExtent)) && ((DartRuntimePrimitives.RequireValue(parentDataLocal.layoutOffset) + itemExtent) > renderObject.constraints.scrollOffset);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).forEach((__arg0) => visitor(__arg0));
    }

}

public class SliverOpacity : SingleChildRenderObjectWidget
{
    public virtual double opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    public SliverOpacity(Key? key = null, double opacity = default!, bool alwaysIncludeSemantics = false, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
        System.Diagnostics.Debug.Assert((opacity >= 0.0) && (opacity <= 1.0));
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderSliverOpacity(opacity: opacity, alwaysIncludeSemantics: alwaysIncludeSemantics);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSliverOpacity)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderSliverOpacity>)(() =>
{
    var __cascade = __renderObject;
    __cascade.opacity = opacity;
    __cascade.alwaysIncludeSemantics = alwaysIncludeSemantics;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<double>("opacity", opacity));
        properties.add(new FlagProperty("alwaysIncludeSemantics", value: alwaysIncludeSemantics, ifTrue: "alwaysIncludeSemantics"));
    }

}

public class SliverIgnorePointer : SingleChildRenderObjectWidget
{
    public virtual bool ignoring { get; private set; } = default!;
    public virtual bool? ignoringSemantics { get; private set; }

    public SliverIgnorePointer(Key? key = null, bool ignoring = true, bool? ignoringSemantics = null, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.ignoring = ignoring;
        this.ignoringSemantics = ignoringSemantics;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderSliverIgnorePointer(ignoring: ignoring, ignoringSemantics: ignoringSemantics);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSliverIgnorePointer)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderSliverIgnorePointer>)(() =>
{
    var __cascade = __renderObject;
    __cascade.ignoring = ignoring;
    __cascade.ignoringSemantics = ignoringSemantics;
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("ignoring", ignoring));
        properties.add(new DiagnosticsProperty<bool>("ignoringSemantics", ignoringSemantics, defaultValue: null));
    }

}

public class SliverOffstage : SingleChildRenderObjectWidget
{
    public virtual bool offstage { get; private set; } = default!;

    public SliverOffstage(Key? key = null, bool offstage = true, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.offstage = offstage;
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderSliverOffstage(offstage: offstage));
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSliverOffstage)renderObject;
        __renderObject.offstage = offstage;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("offstage", offstage));
    }

    public override SingleChildRenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<SingleChildRenderObjectElement>(new _SliverOffstageElement__sliver(this));
}

internal class _SliverOffstageElement__sliver : SingleChildRenderObjectElement
{
    internal _SliverOffstageElement__sliver(SliverOffstage widget) : base(widget)
    {
    }

    public override void debugVisitOnstageChildren(System.Action<Element> visitor)
    {
        if (!((SliverOffstage?)widget)!.offstage)
        {
            base.debugVisitOnstageChildren(visitor);
        }
    }

}

public class KeepAlive : ParentDataWidget<KeepAliveParentDataMixin>
{
    public virtual bool keepAlive { get; private set; } = default!;

    public KeepAlive(Key? key = null, bool keepAlive = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.keepAlive = keepAlive;
    }

    public override void applyParentData(RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => renderObject.parentData is KeepAliveParentDataMixin);
        var parentDataLocal = ((KeepAliveParentDataMixin?)renderObject.parentData!)!;
        if (parentDataLocal.keepAlive != keepAlive)
        {
            parentDataLocal.keepAlive = keepAlive;
            if (!keepAlive)
            {
                if (renderObject.parent is KeepAliveReleaseListener listener && renderObject is RenderBox box)
                    listener.ReleaseKeepAlive(box);
                else renderObject.parent?.markNeedsLayout();
            }
        }
    }

    public override bool debugCanApplyOutOfTurn() => keepAlive;
    public override Type debugTypicalAncestorWidgetClass => throw DartRuntimePrimitives.AsException(FlutterError.Create("Multiple Types are supported, use debugTypicalAncestorWidgetDescription."));
    public override string debugTypicalAncestorWidgetDescription => "SliverWithKeepAliveWidget or TwoDimensionalViewport";
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("keepAlive", keepAlive));
    }

}

public class SliverConstrainedCrossAxis : StatelessWidget
{
    public virtual double maxExtent { get; private set; } = default!;
    public virtual Widget sliver { get; private set; } = default!;

    public SliverConstrainedCrossAxis(Key? key = null, double maxExtent = default!, Widget sliver = default!) : base(key: key)
    {
        this.maxExtent = maxExtent;
        this.sliver = sliver;
    }

    public override Widget build(BuildContext context)
    {
        return new _SliverZeroFlexParentDataWidget__sliver(sliver: new _SliverConstrainedCrossAxis__sliver(maxExtent: maxExtent, sliver: sliver));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SliverZeroFlexParentDataWidget__sliver : ParentDataWidget<SliverPhysicalParentData>
{
    internal _SliverZeroFlexParentDataWidget__sliver(Widget sliver) : base(child: sliver)
    {
    }

    public override void applyParentData(RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => renderObject.parentData is SliverPhysicalParentData);
        var parentDataLocal = ((SliverPhysicalParentData?)renderObject.parentData!)!;
        var needsLayout = false;
        if (parentDataLocal.crossAxisFlex != 0L)
        {
            parentDataLocal.crossAxisFlex = 0L;
            needsLayout = true;
        }
        if (needsLayout)
        {
            renderObject.parent?.markNeedsLayout();
        }
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(SliverCrossAxisGroup);
}

internal class _SliverConstrainedCrossAxis__sliver : SingleChildRenderObjectWidget
{
    public virtual double maxExtent { get; private set; } = default!;

    internal _SliverConstrainedCrossAxis__sliver(double maxExtent, Widget sliver) : base(child: sliver)
    {
        this.maxExtent = maxExtent;
        System.Diagnostics.Debug.Assert(maxExtent >= 0.0);
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderSliverConstrainedCrossAxis(maxExtent: maxExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSliverConstrainedCrossAxis)renderObject;
        __renderObject.maxExtent = maxExtent;
    }

}

public class SliverCrossAxisExpanded : ParentDataWidget<SliverPhysicalContainerParentData>
{
    public virtual long flex { get; private set; } = default!;

    public SliverCrossAxisExpanded(Key? key = null, long flex = default!, Widget sliver = default!) : base(key: key, child: sliver)
    {
        this.flex = flex;
        System.Diagnostics.Debug.Assert((flex > 0L) && (flex < double.PositiveInfinity));
    }

    public override void applyParentData(RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => renderObject.parentData is SliverPhysicalContainerParentData);
        DartRuntimePrimitives.Assert(() => renderObject.parent is RenderSliverCrossAxisGroup);
        var parentDataLocal = ((SliverPhysicalParentData?)renderObject.parentData!)!;
        var needsLayout = false;
        if (parentDataLocal.crossAxisFlex != flex)
        {
            parentDataLocal.crossAxisFlex = flex;
            needsLayout = true;
        }
        if (needsLayout)
        {
            renderObject.parent?.markNeedsLayout();
        }
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(SliverCrossAxisGroup);
}

public class SliverCrossAxisGroup : MultiChildRenderObjectWidget
{
    public SliverCrossAxisGroup(Key? key = null, List<Widget> slivers = default!) : base(key: key, children: slivers)
    {
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderSliverCrossAxisGroup();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverMainAxisGroup : MultiChildRenderObjectWidget
{
    public SliverMainAxisGroup(Key? key = null, List<Widget> slivers = default!) : base(key: key, children: slivers)
    {
    }

    public override MultiChildRenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<MultiChildRenderObjectElement>(new _SliverMainAxisGroupElement__sliver(this));
    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderSliverMainAxisGroup();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SliverMainAxisGroupElement__sliver : MultiChildRenderObjectElement
{
    internal _SliverMainAxisGroupElement__sliver(SliverMainAxisGroup widget) : base(widget)
    {
    }

    public override void debugVisitOnstageChildren(System.Action<Element> visitor)
    {
        children.where((e) =>
        {
            var renderSliver = ((RenderSliver?)e.renderObject!)!;
            return renderSliver.geometry!.visible;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).forEach((__arg0) => visitor(__arg0));
    }

}

public class SliverEnsureSemantics : SingleChildRenderObjectWidget
{
    public SliverEnsureSemantics(Key? key = null, Widget sliver = default!) : base(key: key, child: sliver)
    {
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new _RenderSliverEnsureSemantics__sliver());
}

internal class _RenderSliverEnsureSemantics__sliver : RenderProxySliver
{
    public override bool ensureSemantics => true;
}
