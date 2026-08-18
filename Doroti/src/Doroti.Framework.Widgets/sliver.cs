// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver.dart
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

public abstract class SliverWithKeepAliveWidget : RenderObjectWidget
{
    protected SliverWithKeepAliveWidget(global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public abstract override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context);
}

public abstract class SliverMultiBoxAdaptorWidget : SliverWithKeepAliveWidget
{
    public virtual SliverChildDelegate @delegate { get; private set; } = default!;

    protected SliverMultiBoxAdaptorWidget(global::Doroti.Framework.Foundation.Key? key = null, SliverChildDelegate @delegate = default!) : base(key: key)
    {
        this.@delegate = @delegate;
    }

    public override SliverMultiBoxAdaptorElement createElement() => new SliverMultiBoxAdaptorElement(this);
    public abstract override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context);
    public virtual double? estimateMaxScrollOffset(global::Doroti.Framework.Rendering.SliverConstraints? constraints, long firstIndex, long lastIndex, double leadingScrollOffset, double trailingScrollOffset)
    {
        DartRuntimePrimitives.Assert(() => (lastIndex >= firstIndex));
        return this.@delegate.estimateMaxScrollOffset(firstIndex, lastIndex, leadingScrollOffset, trailingScrollOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliverChildDelegate>("delegate", this.@delegate));
    }

}

public class SliverList : SliverMultiBoxAdaptorWidget
{
    public SliverList(global::Doroti.Framework.Foundation.Key? key = null, SliverChildDelegate @delegate = default!) : base(key: key, @delegate: @delegate)
    {
    }

    public static SliverList CreateBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, long semanticIndexOffset = 0)
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

    public static SliverList CreateSeparated(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findItemIndexCallback = null, global::System.Func<BuildContext, long, Widget?> separatorBuilder = default!, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true)
    {
        var __instance = new SliverList(default!, default!);
        return __instance;
    }

    public static SliverList CreateList(global::Doroti.Framework.Foundation.Key? key = null, List<Widget> children = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true)
    {
        return new SliverList(key, new SliverChildListDelegate(
            children ?? [],
            addAutomaticKeepAlives: addAutomaticKeepAlives,
            addRepaintBoundaries: addRepaintBoundaries,
            addSemanticIndexes: addSemanticIndexes));
    }

    public override SliverMultiBoxAdaptorElement createElement() => new SliverMultiBoxAdaptorElement(this, replaceMovedChildren: true);
    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var element__15163 = ((SliverMultiBoxAdaptorElement?)(object?)context)!;
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderSliverList(childManager: element__15163));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverFixedExtentList : SliverMultiBoxAdaptorWidget
{
    public virtual double itemExtent { get; private set; } = default!;

    public SliverFixedExtentList(global::Doroti.Framework.Foundation.Key? key = null, SliverChildDelegate @delegate = default!, double itemExtent = default!) : base(key: key, @delegate: @delegate)
    {
        this.itemExtent = itemExtent;
    }

    public static SliverFixedExtentList CreateBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, double itemExtent = default!, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, long semanticIndexOffset = 0)
    {
        var __instance = new SliverFixedExtentList(default!, default!, default!);
        __instance.itemExtent = itemExtent;
        return __instance;
    }

    public static SliverFixedExtentList CreateList(global::Doroti.Framework.Foundation.Key? key = null, List<Widget> children = default!, double itemExtent = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true)
    {
        var __instance = new SliverFixedExtentList(default!, default!, default!);
        __instance.itemExtent = itemExtent;
        return __instance;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var element__21968 = ((SliverMultiBoxAdaptorElement?)(object?)context)!;
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderSliverFixedExtentList(childManager: element__21968, itemExtent: this.itemExtent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderSliverFixedExtentList)(object)renderObject;
        __renderObject.itemExtent = this.itemExtent;
    }

}

public class SliverVariedExtentList : SliverMultiBoxAdaptorWidget
{
    public virtual ItemExtentBuilder itemExtentBuilder { get; private set; } = default!;

    public SliverVariedExtentList(global::Doroti.Framework.Foundation.Key? key = null, SliverChildDelegate @delegate = default!, ItemExtentBuilder itemExtentBuilder = default!) : base(key: key, @delegate: @delegate)
    {
        this.itemExtentBuilder = itemExtentBuilder;
    }

    public static SliverVariedExtentList CreateBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, ItemExtentBuilder itemExtentBuilder = default!, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true)
    {
        var __instance = new SliverVariedExtentList(default!, default!, default!);
        __instance.itemExtentBuilder = itemExtentBuilder;
        return __instance;
    }

    public static SliverVariedExtentList CreateList(global::Doroti.Framework.Foundation.Key? key = null, List<Widget> children = default!, ItemExtentBuilder itemExtentBuilder = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true)
    {
        var __instance = new SliverVariedExtentList(default!, default!, default!);
        __instance.itemExtentBuilder = itemExtentBuilder;
        return __instance;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var element__26574 = ((SliverMultiBoxAdaptorElement?)(object?)context)!;
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderSliverVariedExtentList(childManager: element__26574, itemExtentBuilder: (ItemExtentBuilder)this.itemExtentBuilder));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderSliverVariedExtentList)(object)renderObject;
        __renderObject.itemExtentBuilder = this.itemExtentBuilder;
    }

}

public class SliverGrid : SliverMultiBoxAdaptorWidget
{
    public virtual global::Doroti.Framework.Rendering.SliverGridDelegate gridDelegate { get; private set; } = default!;

    public SliverGrid(global::Doroti.Framework.Foundation.Key? key = null, SliverChildDelegate @delegate = default!, global::Doroti.Framework.Rendering.SliverGridDelegate gridDelegate = default!) : base(key: key, @delegate: @delegate)
    {
        this.gridDelegate = gridDelegate;
    }

    public static SliverGrid CreateBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Rendering.SliverGridDelegate gridDelegate = default!, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, long semanticIndexOffset = 0)
    {
        var __instance = new SliverGrid(default!, default!, default!);
        __instance.gridDelegate = gridDelegate;
        return __instance;
    }

    public static SliverGrid CreateCount(global::Doroti.Framework.Foundation.Key? key = null, long crossAxisCount = default!, double mainAxisSpacing = 0.0, double crossAxisSpacing = 0.0, double childAspectRatio = 1.0, List<Widget> children = default!)
    {
        var __instance = new SliverGrid(default!, default!, default!);
        List<Widget> __children = children ?? new List<Widget>();
        __instance.gridDelegate = new global::Doroti.Framework.Rendering.SliverGridDelegateWithFixedCrossAxisCount(crossAxisCount: crossAxisCount, mainAxisSpacing: mainAxisSpacing, crossAxisSpacing: crossAxisSpacing, childAspectRatio: childAspectRatio);
        return __instance;
    }

    public static SliverGrid CreateExtent(global::Doroti.Framework.Foundation.Key? key = null, double maxCrossAxisExtent = default!, double mainAxisSpacing = 0.0, double crossAxisSpacing = 0.0, double childAspectRatio = 1.0, List<Widget> children = default!)
    {
        var __instance = new SliverGrid(default!, default!, default!);
        List<Widget> __children = children ?? new List<Widget>();
        __instance.gridDelegate = new global::Doroti.Framework.Rendering.SliverGridDelegateWithMaxCrossAxisExtent(maxCrossAxisExtent: maxCrossAxisExtent, mainAxisSpacing: mainAxisSpacing, crossAxisSpacing: crossAxisSpacing, childAspectRatio: childAspectRatio);
        return __instance;
    }

    public static SliverGrid CreateList(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Rendering.SliverGridDelegate gridDelegate = default!, List<Widget> children = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, long semanticIndexOffset = 0)
    {
        var __instance = new SliverGrid(default!, default!, default!);
        __instance.gridDelegate = gridDelegate;
        return __instance;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var element__34723 = ((SliverMultiBoxAdaptorElement?)(object?)context)!;
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderSliverGrid(childManager: element__34723, gridDelegate: this.gridDelegate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderSliverGrid)(object)renderObject;
        __renderObject.gridDelegate = this.gridDelegate;
    }

    public override double? estimateMaxScrollOffset(global::Doroti.Framework.Rendering.SliverConstraints? constraints, long firstIndex, long lastIndex, double leadingScrollOffset, double trailingScrollOffset)
    {
        return ((base.estimateMaxScrollOffset(constraints, firstIndex, lastIndex, leadingScrollOffset, trailingScrollOffset) ?? (double)this.gridDelegate.getLayout(constraints!).computeMaxScrollOffset(DartRuntimePrimitives.RequireValue(((SliverChildDelegate)this.@delegate).estimatedChildCount))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverMultiBoxAdaptorElement : RenderObjectElement, global::Doroti.Framework.Rendering.RenderSliverBoxChildManager
{
    internal virtual bool _replaceMovedChildren { get; private set; } = default!;
    internal virtual SortedDictionary<long, Element?> _childElements { get; private set; } = new SortedDictionary<long, Element?>();
    internal virtual global::Doroti.Framework.Rendering.RenderBox? _currentBeforeChild { get; set; } = default;
    internal virtual long? _currentlyUpdatingChildIndex { get; set; } = default;
    internal virtual bool _didUnderflow { get; set; } = false;

    public SliverMultiBoxAdaptorElement(SliverMultiBoxAdaptorWidget widget, bool replaceMovedChildren = false) : base(widget)
    {
        this._replaceMovedChildren = replaceMovedChildren;
    }

    public override global::Doroti.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(((global::Doroti.Framework.Rendering.RenderSliverMultiBoxAdaptor?)(object?)base.renderObject)!);
    public override void update(Widget newWidget)
    {
        var __newWidget = (SliverMultiBoxAdaptorWidget)(object)newWidget;
        var oldWidget__36992 = ((SliverMultiBoxAdaptorWidget?)(object?)this.widget)!;
        base.update(__newWidget);
        SliverChildDelegate newDelegate__37102 = ((SliverMultiBoxAdaptorWidget)__newWidget).@delegate;
        SliverChildDelegate oldDelegate__37166 = ((SliverMultiBoxAdaptorWidget)oldWidget__36992).@delegate;
        if (((!object.Equals(newDelegate__37102, oldDelegate__37166)) && (((!object.Equals(DartRuntimePrimitives.RuntimeType(newDelegate__37102), DartRuntimePrimitives.RuntimeType(oldDelegate__37166))) || newDelegate__37102.shouldRebuild(oldDelegate__37166)))))
        {
            performRebuild();
        }
    }

    public override void performRebuild()
    {
        base.performRebuild();
        _currentBeforeChild = null;
        var childrenUpdated__37616 = false;
        DartRuntimePrimitives.Assert(() => (this._currentlyUpdatingChildIndex is null));
        try
        {
            var newChildren__37713 = new SortedDictionary<long, Element?>();
            DartMap<long, double> indexToLayoutOffset__37787 = new DartMap<long, double>();
            var adaptorWidget__37845 = ((SliverMultiBoxAdaptorWidget?)(object?)this.widget)!;
            void processElement(long index)
            {
                _currentlyUpdatingChildIndex = index;
                if (((this._childElements.ContainsKey(index)) && (!object.Equals(this._childElements.GetValueOrDefault(index), newChildren__37713.GetValueOrDefault(index)))))
                {
                    this._childElements[index] = updateChild(this._childElements.GetValueOrDefault(index), ((Widget)(object)null), index);
                    childrenUpdated__37616 = true;
                }
                Element? newChild__38320 = ((Element?)(object?)updateChild(newChildren__37713.GetValueOrDefault(index), _build(index, adaptorWidget__37845), index));
                if ((newChild__38320 is not null))
                {
                    childrenUpdated__37616 = (childrenUpdated__37616 || (!object.Equals(this._childElements.GetValueOrDefault(index), newChild__38320)));
                    this._childElements[index] = newChild__38320;
                    var parentData__38616 = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)((global::Doroti.Framework.Rendering.ParentData?)((dynamic)((Element)newChild__38320).renderObject!).parentData)!)!;
                    if ((index == 0L))
                    {
                        parentData__38616.layoutOffset = 0.0;
                    }
                    else
                    {
                        if (indexToLayoutOffset__37787.ContainsKey(index))
                        {
                            parentData__38616.layoutOffset = DartCollectionRuntime.NullableMapValue<double>(indexToLayoutOffset__37787, index);
                        }
                    }
                    if (!((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData)parentData__38616).keptAlive)
                    {
                        _currentBeforeChild = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((Element)newChild__38320).renderObject)!;
                    }
                }
                else
                {
                    childrenUpdated__37616 = true;
                    this._childElements.Remove(index);
                }
            }
            foreach (long index__39165 in this._childElements.Keys.ToList())
            {
                global::Doroti.Framework.Foundation.Key? key__39225 = this._childElements.GetValueOrDefault(index__39165)!.widget.key;
                long? newIndex__39285 = ((key__39225 is null) ? null : ((SliverMultiBoxAdaptorWidget)adaptorWidget__37845).@delegate.findIndexByKey(key__39225));
                var childParentData__39375 = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)((global::Doroti.Framework.Rendering.ParentData?)((dynamic)this._childElements.GetValueOrDefault(index__39165)!.renderObject)?.parentData))!;
                if (((childParentData__39375 is not null) && (childParentData__39375.layoutOffset is not null)))
                {
                    indexToLayoutOffset__37787[index__39165] = DartRuntimePrimitives.RequireValue(childParentData__39375.layoutOffset);
                }
                if (((newIndex__39285 is not null) && (DartRuntimePrimitives.RequireValue(newIndex__39285) != index__39165)))
                {
                    long newIndex__39285__value39663 = DartRuntimePrimitives.RequireValue(newIndex__39285);
                    if ((childParentData__39375 is not null))
                    {
                        childParentData__39375.layoutOffset = null;
                    }
                    newChildren__37713[DartRuntimePrimitives.RequireValue(newIndex__39285__value39663)] = this._childElements.GetValueOrDefault(index__39165);
                    if (this._replaceMovedChildren)
                    {
                        newChildren__37713.putIfAbsent(index__39165, (() => default!));
                    }
                    this._childElements.Remove(index__39165);
                }
                else
                {
                    newChildren__37713.putIfAbsent(index__39165, (() => this._childElements.GetValueOrDefault(index__39165)));
                }
            }
            ((dynamic)((dynamic)this.renderObject)).debugChildIntegrityEnabled = false;
            newChildren__37713.Keys.forEach((__arg0) => ((global::System.Action<long>)processElement)(__arg0));
            if ((!childrenUpdated__37616 && this._didUnderflow))
            {
                long lastKey__41166 = (DartCollectionRuntime.LastKeyOrNull<long, Element?>(this._childElements) ?? -1L);
                long rightBoundary__41226 = (lastKey__41166 + 1L);
                newChildren__37713[rightBoundary__41226] = this._childElements.GetValueOrDefault(rightBoundary__41226);
                processElement(rightBoundary__41226);
            }
        }
        finally
        {
            _currentlyUpdatingChildIndex = null;
            ((dynamic)((dynamic)this.renderObject)).debugChildIntegrityEnabled = true;
        }
    }

    internal virtual Widget? _build(long index, SliverMultiBoxAdaptorWidget widget)
    {
        return ((Widget?)(object?)((SliverMultiBoxAdaptorWidget)widget).@delegate.build(this, index));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void createChild(long index, global::Doroti.Framework.Rendering.RenderBox? after)
    {
        DartRuntimePrimitives.Assert(() => (this._currentlyUpdatingChildIndex is null));
        this.owner!.buildScope(this, ((global::System.Action)(() =>
        {
            var insertFirst__41780 = (after is null);
            DartRuntimePrimitives.Assert(() => (insertFirst__41780 || (this._childElements.ContainsKey((index - 1L)))));
            _currentBeforeChild = (insertFirst__41780 ? null : (((global::Doroti.Framework.Rendering.RenderBox?)(object?)this._childElements.GetValueOrDefault((index - 1L))!.renderObject)!));
            Element? newChild__42014 = default!;
            try
            {
                var adaptorWidget__42050 = ((SliverMultiBoxAdaptorWidget?)(object?)this.widget)!;
                _currentlyUpdatingChildIndex = index;
                newChild__42014 = updateChild(this._childElements.GetValueOrDefault(index), _build(index, adaptorWidget__42050), index);
            }
            finally
            {
                _currentlyUpdatingChildIndex = null;
            }
            if ((newChild__42014 is not null))
            {
                this._childElements[index] = newChild__42014;
            }
            else
            {
                this._childElements.Remove(index);
            }
        })));
    }

    public override Element? updateChild(Element? child, Widget? newWidget, object? newSlot)
    {
        var oldParentData__42559 = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)((global::Doroti.Framework.Rendering.ParentData?)((dynamic)child?.renderObject)?.parentData))!;
        Element? newChild__42663 = ((Element?)(object?)base.updateChild(child, newWidget, newSlot));
        var newParentData__42730 = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)((global::Doroti.Framework.Rendering.ParentData?)((dynamic)newChild__42663?.renderObject)?.parentData))!;
        if ((((!object.Equals(oldParentData__42559, newParentData__42730)) && (oldParentData__42559 is not null)) && (newParentData__42730 is not null)))
        {
            newParentData__42730.layoutOffset = oldParentData__42559.layoutOffset;
        }
        return newChild__42663;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => (((Element)child).slot is not null));
        DartRuntimePrimitives.Assert(() => this._childElements.ContainsKey(DartRuntimePrimitives.ConvertValue<long>(((Element)child).slot)));
        this._childElements.Remove(DartRuntimePrimitives.ConvertValue<long>(((Element)child).slot));
        base.forgetChild(child);
    }

    public virtual void removeChild(global::Doroti.Framework.Rendering.RenderBox child)
    {
        long index__43350 = DartRuntimePrimitives.ConvertValue<long>(((long)((dynamic)((dynamic)this.renderObject)).indexOf(child)));
        DartRuntimePrimitives.Assert(() => (this._currentlyUpdatingChildIndex is null));
        DartRuntimePrimitives.Assert(() => (index__43350 >= 0L));
        this.owner!.buildScope(this, ((global::System.Action)(() =>
        {
            DartRuntimePrimitives.Assert(() => this._childElements.ContainsKey(index__43350));
            try
            {
                _currentlyUpdatingChildIndex = index__43350;
                Element? result__43624 = ((Element?)(object?)updateChild(this._childElements.GetValueOrDefault(index__43350), ((Widget)(object)null), index__43350));
                DartRuntimePrimitives.Assert(() => (result__43624 is null));
            }
            finally
            {
                _currentlyUpdatingChildIndex = null;
            }
            this._childElements.Remove(index__43350);
            DartRuntimePrimitives.Assert(() => !this._childElements.ContainsKey(index__43350));
        })));
    }

    internal static double _extrapolateMaxScrollOffset(long firstIndex, long lastIndex, double leadingScrollOffset, double trailingScrollOffset, long childCount)
    {
        if ((DartRuntimePrimitives.RequireValue(lastIndex) == (childCount - 1L)))
        {
            return DartRuntimePrimitives.RequireValue(trailingScrollOffset);
        }
        long reifiedCount__44153 = ((DartRuntimePrimitives.RequireValue(lastIndex) - DartRuntimePrimitives.RequireValue(firstIndex)) + 1L);
        double averageExtent__44213 = (((DartRuntimePrimitives.RequireValue(trailingScrollOffset) - DartRuntimePrimitives.RequireValue(leadingScrollOffset))) / reifiedCount__44153);
        long remainingCount__44304 = ((childCount - DartRuntimePrimitives.RequireValue(lastIndex)) - 1L);
        return (DartRuntimePrimitives.RequireValue(trailingScrollOffset) + (averageExtent__44213 * remainingCount__44304));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double estimateMaxScrollOffset(global::Doroti.Framework.Rendering.SliverConstraints constraints, long? firstIndex = null, long? lastIndex = null, double? leadingScrollOffset = null, double? trailingScrollOffset = null)
    {
        long? childCount__44634 = this.estimatedChildCount;
        if ((childCount__44634 is null))
        {
            return double.PositiveInfinity;
        }
        return (((((SliverMultiBoxAdaptorWidget?)(object?)this.widget)!).estimateMaxScrollOffset(constraints, DartRuntimePrimitives.RequireValue(firstIndex), DartRuntimePrimitives.RequireValue(lastIndex), DartRuntimePrimitives.RequireValue(leadingScrollOffset), DartRuntimePrimitives.RequireValue(trailingScrollOffset)) ?? (double)SliverMultiBoxAdaptorElement._extrapolateMaxScrollOffset(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndex)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(lastIndex)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(leadingScrollOffset)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(trailingScrollOffset)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(childCount__44634)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long? estimatedChildCount => (((SliverMultiBoxAdaptorWidget?)(object?)this.widget)!).@delegate.estimatedChildCount;
    public virtual long childCount
    {
        get
        {
            long? result__45304 = this.estimatedChildCount;
            if ((result__45304 is null))
            {
                var lo__45633 = 0L;
                var hi__45651 = 1L;
                var adaptorWidget__45671 = ((SliverMultiBoxAdaptorWidget?)(object?)this.widget)!;
                long max__45742 = (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? 9007199254740992L : ((long.MaxValue)));
                while ((_build((hi__45651 - 1L), adaptorWidget__45671) is not null))
                {
                    lo__45633 = (hi__45651 - 1L);
                    if ((hi__45651 < (checked((long)(max__45742 / 2L)))))
                    {
                        hi__45651 *= 2L;
                    }
                    else
                    {
                        if ((hi__45651 < max__45742))
                        {
                            hi__45651 = max__45742;
                        }
                        else
                        {
                            throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"Could not find the number of children in {((SliverMultiBoxAdaptorWidget)adaptorWidget__45671).@delegate}.\n" + "The childCount getter was called (implying that the delegate's builder returned null " + $"for a positive index), but even building the child with index {hi__45651} (the maximum " + "possible integer) did not return null. Consider implementing childCount to avoid " + "the cost of searching for the final child."));
                        }
                    }
                }
                while (((hi__45651 - lo__45633) > 1L))
                {
                    long mid__46602 = ((checked((long)(((hi__45651 - lo__45633)) / 2L))) + lo__45633);
                    if ((_build((mid__46602 - 1L), adaptorWidget__45671) is null))
                    {
                        hi__45651 = mid__46602;
                    }
                    else
                    {
                        lo__45633 = mid__46602;
                    }
                }
                result__45304 = lo__45633;
            }
            return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(result__45304));
            return default!;
        }
    }
    public virtual void didStartLayout()
    {
        DartRuntimePrimitives.Assert(() => debugAssertChildListLocked());
    }

    public virtual void didFinishLayout()
    {
        DartRuntimePrimitives.Assert(() => debugAssertChildListLocked());
        long firstIndex__46987 = (DartCollectionRuntime.FirstKeyOrNull<long, Element?>(this._childElements) ?? 0L);
        long lastIndex__47046 = (DartCollectionRuntime.LastKeyOrNull<long, Element?>(this._childElements) ?? 0L);
        (((SliverMultiBoxAdaptorWidget?)(object?)this.widget)!).@delegate.didFinishLayout(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndex__46987)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(lastIndex__47046)));
    }

    public virtual bool debugAssertChildListLocked()
    {
        DartRuntimePrimitives.Assert(() => (this._currentlyUpdatingChildIndex is null));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didAdoptChild(global::Doroti.Framework.Rendering.RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (this._currentlyUpdatingChildIndex is not null));
        var childParentData__47459 = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        childParentData__47459.index = this._currentlyUpdatingChildIndex;
    }

    public virtual void setDidUnderflow(bool value)
    {
        _didUnderflow = value;
    }

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        long __slot = DartRuntimePrimitives.ConvertValue<long>(slot);
        DartRuntimePrimitives.Assert(() => (this._currentlyUpdatingChildIndex == __slot));
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)((dynamic)this.renderObject)).debugValidateChild(child)));
        ((dynamic)this.renderObject).insert(((global::Doroti.Framework.Rendering.RenderBox?)(object?)child)!, after: this._currentBeforeChild);
        DartRuntimePrimitives.Assert(() =>
            {
                var childParentData__47994 = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)((global::Doroti.Framework.Rendering.RenderBox)child).parentData!)!;
                DartRuntimePrimitives.Assert(() => (__slot == ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData)childParentData__47994).index));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        long __oldSlot = DartRuntimePrimitives.ConvertValue<long>(oldSlot);
        long __newSlot = DartRuntimePrimitives.ConvertValue<long>(newSlot);
        DartRuntimePrimitives.Assert(() => (this._currentlyUpdatingChildIndex == DartRuntimePrimitives.RequireValue(__newSlot)));
        ((dynamic)((dynamic)this.renderObject)).move(((global::Doroti.Framework.Rendering.RenderBox?)(object?)child)!, after: this._currentBeforeChild);
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        long __slot = DartRuntimePrimitives.ConvertValue<long>(slot);
        DartRuntimePrimitives.Assert(() => (this._currentlyUpdatingChildIndex is not null));
        ((dynamic)((dynamic)this.renderObject)).remove(((global::Doroti.Framework.Rendering.RenderBox?)(object?)child)!);
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        DartRuntimePrimitives.Assert(() => !this._childElements.Values.any(((child) => (child is null))));
        this._childElements.Values.cast<Element>().ToList().forEach((__arg0) => ((global::System.Action<Element>)visitor)(__arg0));
    }

    public override void debugVisitOnstageChildren(global::System.Action<Element> visitor)
    {
        this._childElements.Values.cast<Element>().where(((child) =>
        {
            var parentData__49042 = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)((global::Doroti.Framework.Rendering.ParentData?)((dynamic)((Element)child).renderObject!).parentData)!)!;
            double itemExtent__49146 = DartRuntimePrimitives.ConvertValue<double>((((dynamic)((dynamic)this.renderObject).constraints).axis switch { global::Doroti.Framework.Painting.Axis.horizontal => ((Rect)((dynamic)((Element)child).renderObject!).paintBounds).width, global::Doroti.Framework.Painting.Axis.vertical => ((Rect)((dynamic)((Element)child).renderObject!).paintBounds).height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            return (((parentData__49042.layoutOffset is not null) && (DartRuntimePrimitives.RequireValue(parentData__49042.layoutOffset) < (((dynamic)((dynamic)this.renderObject).constraints).scrollOffset + ((dynamic)((dynamic)this.renderObject).constraints).remainingPaintExtent))) && ((DartRuntimePrimitives.RequireValue(parentData__49042.layoutOffset) + itemExtent__49146) > ((dynamic)((dynamic)this.renderObject).constraints).scrollOffset));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).forEach((__arg0) => ((global::System.Action<Element>)visitor)(__arg0));
    }

}

public class SliverOpacity : SingleChildRenderObjectWidget
{
    public virtual double opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    public SliverOpacity(global::Doroti.Framework.Foundation.Key? key = null, double opacity = default!, bool alwaysIncludeSemantics = false, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
        System.Diagnostics.Debug.Assert(((opacity >= 0.0) && (opacity <= 1.0)));
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderSliverOpacity(opacity: this.opacity, alwaysIncludeSemantics: this.alwaysIncludeSemantics));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderSliverOpacity)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderSliverOpacity>)(() =>
{
    var __cascade = __renderObject;
    __cascade.opacity = this.opacity;
    __cascade.alwaysIncludeSemantics = this.alwaysIncludeSemantics;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<double>("opacity", this.opacity));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("alwaysIncludeSemantics", value: this.alwaysIncludeSemantics, ifTrue: "alwaysIncludeSemantics"));
    }

}

public class SliverIgnorePointer : SingleChildRenderObjectWidget
{
    public virtual bool ignoring { get; private set; } = default!;
    public virtual bool? ignoringSemantics { get; private set; }

    public SliverIgnorePointer(global::Doroti.Framework.Foundation.Key? key = null, bool ignoring = true, bool? ignoringSemantics = null, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.ignoring = ignoring;
        this.ignoringSemantics = ignoringSemantics;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderSliverIgnorePointer(ignoring: this.ignoring, ignoringSemantics: this.ignoringSemantics));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderSliverIgnorePointer)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderSliverIgnorePointer>)(() =>
{
    var __cascade = __renderObject;
    __cascade.ignoring = this.ignoring;
    __cascade.ignoringSemantics = this.ignoringSemantics;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("ignoring", this.ignoring));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("ignoringSemantics", this.ignoringSemantics, defaultValue: null));
    }

}

public class SliverOffstage : SingleChildRenderObjectWidget
{
    public virtual bool offstage { get; private set; } = default!;

    public SliverOffstage(global::Doroti.Framework.Foundation.Key? key = null, bool offstage = true, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.offstage = offstage;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new global::Doroti.Framework.Rendering.RenderSliverOffstage(offstage: this.offstage));
    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderSliverOffstage)(object)renderObject;
        __renderObject.offstage = this.offstage;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("offstage", this.offstage));
    }

    public override SingleChildRenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<SingleChildRenderObjectElement>(new _SliverOffstageElement__sliver(this));
}

internal class _SliverOffstageElement__sliver : SingleChildRenderObjectElement
{
    internal _SliverOffstageElement__sliver(SliverOffstage widget) : base(widget)
    {
    }

    public override void debugVisitOnstageChildren(global::System.Action<Element> visitor)
    {
        if (!(((SliverOffstage?)(object?)this.widget)!).offstage)
        {
            base.debugVisitOnstageChildren((global::System.Action<Element>)visitor);
        }
    }

}

public class KeepAlive : ParentDataWidget<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>
{
    public virtual bool keepAlive { get; private set; } = default!;

    public KeepAlive(global::Doroti.Framework.Foundation.Key? key = null, bool keepAlive = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.keepAlive = keepAlive;
    }

    public override void applyParentData(global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderObject)renderObject).parentData is global::Doroti.Framework.Rendering.KeepAliveParentDataMixin));
        var parentData__60822 = ((global::Doroti.Framework.Rendering.KeepAliveParentDataMixin?)(object?)((global::Doroti.Framework.Rendering.RenderObject)renderObject).parentData!)!;
        if ((((global::Doroti.Framework.Rendering.KeepAliveParentDataMixin)parentData__60822).keepAlive != this.keepAlive))
        {
            parentData__60822.keepAlive = this.keepAlive;
            if (!this.keepAlive)
            {
                ((dynamic)((global::Doroti.Framework.Rendering.RenderObject)renderObject).parent)?.markNeedsLayout();
            }
        }
    }

    public override bool debugCanApplyOutOfTurn() => this.keepAlive;
    public override Type debugTypicalAncestorWidgetClass => throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Multiple Types are supported, use debugTypicalAncestorWidgetDescription."));
    public override string debugTypicalAncestorWidgetDescription => "SliverWithKeepAliveWidget or TwoDimensionalViewport";
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("keepAlive", this.keepAlive));
    }

}

public class SliverConstrainedCrossAxis : StatelessWidget
{
    public virtual double maxExtent { get; private set; } = default!;
    public virtual Widget sliver { get; private set; } = default!;

    public SliverConstrainedCrossAxis(global::Doroti.Framework.Foundation.Key? key = null, double maxExtent = default!, Widget sliver = default!) : base(key: key)
    {
        this.maxExtent = maxExtent;
        this.sliver = sliver;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _SliverZeroFlexParentDataWidget__sliver(sliver: new _SliverConstrainedCrossAxis__sliver(maxExtent: this.maxExtent, sliver: this.sliver)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SliverZeroFlexParentDataWidget__sliver : ParentDataWidget<global::Doroti.Framework.Rendering.SliverPhysicalParentData>
{
    internal _SliverZeroFlexParentDataWidget__sliver(Widget sliver) : base(child: sliver)
    {
    }

    public override void applyParentData(global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderObject)renderObject).parentData is global::Doroti.Framework.Rendering.SliverPhysicalParentData));
        var parentData__64073 = ((global::Doroti.Framework.Rendering.SliverPhysicalParentData?)(object?)((global::Doroti.Framework.Rendering.RenderObject)renderObject).parentData!)!;
        var needsLayout__64148 = false;
        if ((((global::Doroti.Framework.Rendering.SliverPhysicalParentData)parentData__64073).crossAxisFlex != 0L))
        {
            parentData__64073.crossAxisFlex = 0L;
            needsLayout__64148 = true;
        }
        if (needsLayout__64148)
        {
            ((dynamic)((global::Doroti.Framework.Rendering.RenderObject)renderObject).parent)?.markNeedsLayout();
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
        System.Diagnostics.Debug.Assert((maxExtent >= 0.0));
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderSliverConstrainedCrossAxis(maxExtent: this.maxExtent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderSliverConstrainedCrossAxis)(object)renderObject;
        __renderObject.maxExtent = this.maxExtent;
    }

}

public class SliverCrossAxisExpanded : ParentDataWidget<global::Doroti.Framework.Rendering.SliverPhysicalContainerParentData>
{
    public virtual long flex { get; private set; } = default!;

    public SliverCrossAxisExpanded(global::Doroti.Framework.Foundation.Key? key = null, long flex = default!, Widget sliver = default!) : base(key: key, child: sliver)
    {
        this.flex = flex;
        System.Diagnostics.Debug.Assert(((flex > 0L) && (flex < double.PositiveInfinity)));
    }

    public override void applyParentData(global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderObject)renderObject).parentData is global::Doroti.Framework.Rendering.SliverPhysicalContainerParentData));
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderObject)renderObject).parent is global::Doroti.Framework.Rendering.RenderSliverCrossAxisGroup));
        var parentData__66457 = ((global::Doroti.Framework.Rendering.SliverPhysicalParentData?)(object?)((global::Doroti.Framework.Rendering.RenderObject)renderObject).parentData!)!;
        var needsLayout__66532 = false;
        if ((((global::Doroti.Framework.Rendering.SliverPhysicalParentData)parentData__66457).crossAxisFlex != this.flex))
        {
            parentData__66457.crossAxisFlex = this.flex;
            needsLayout__66532 = true;
        }
        if (needsLayout__66532)
        {
            ((dynamic)((global::Doroti.Framework.Rendering.RenderObject)renderObject).parent)?.markNeedsLayout();
        }
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(SliverCrossAxisGroup);
}

public class SliverCrossAxisGroup : MultiChildRenderObjectWidget
{
    public SliverCrossAxisGroup(global::Doroti.Framework.Foundation.Key? key = null, List<Widget> slivers = default!) : base(key: key, children: slivers)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderSliverCrossAxisGroup());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverMainAxisGroup : MultiChildRenderObjectWidget
{
    public SliverMainAxisGroup(global::Doroti.Framework.Foundation.Key? key = null, List<Widget> slivers = default!) : base(key: key, children: slivers)
    {
    }

    public override MultiChildRenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<MultiChildRenderObjectElement>(new _SliverMainAxisGroupElement__sliver(this));
    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderSliverMainAxisGroup());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SliverMainAxisGroupElement__sliver : MultiChildRenderObjectElement
{
    internal _SliverMainAxisGroupElement__sliver(SliverMainAxisGroup widget) : base(widget)
    {
    }

    public override void debugVisitOnstageChildren(global::System.Action<Element> visitor)
    {
        this.children.where(((e) =>
        {
            var renderSliver__72686 = ((global::Doroti.Framework.Rendering.RenderSliver?)(object?)((Element)e).renderObject!)!;
            return ((global::Doroti.Framework.Rendering.RenderSliver)renderSliver__72686).geometry!.visible;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).forEach((__arg0) => ((global::System.Action<Element>)visitor)(__arg0));
    }

}

public class SliverEnsureSemantics : SingleChildRenderObjectWidget
{
    public SliverEnsureSemantics(global::Doroti.Framework.Foundation.Key? key = null, Widget sliver = default!) : base(key: key, child: sliver)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new _RenderSliverEnsureSemantics__sliver());
}

internal class _RenderSliverEnsureSemantics__sliver : global::Doroti.Framework.Rendering.RenderProxySliver
{
    public override bool ensureSemantics => true;
}
