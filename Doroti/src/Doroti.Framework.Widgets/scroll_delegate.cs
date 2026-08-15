// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_delegate.dart
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

public delegate long? SemanticIndexCallback(Widget widget, long localIndex);

public static partial class Scroll_delegateLibrary
{
    internal static long _kDefaultSemanticIndexCallback(Widget __unused0, long localIndex) => localIndex;
}

public abstract class SliverChildDelegate
{
    protected SliverChildDelegate()
    {
    }

    public abstract Widget? build(BuildContext context, long index);
    public virtual long? estimatedChildCount => null;
    public virtual double? estimateMaxScrollOffset(long firstIndex, long lastIndex, double leadingScrollOffset, double trailingScrollOffset) => null;
    public virtual void didFinishLayout(long firstIndex, long lastIndex)
    {
    }

    public abstract bool shouldRebuild(SliverChildDelegate oldDelegate);
    public virtual long? findIndexByKey(global::Doroti.Generated.Framework.Foundation.Key key) => null;
    public override string ToString()
    {
        var description__9482 = new List<string>();
        debugFillDescription(description__9482);
        return $"{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({string.Join(", ", description__9482)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillDescription(List<string> description)
    {
        try
        {
            long? children__9813 = this.estimatedChildCount;
            if ((children__9813 is not null))
            {
                long children__9813__value9855 = DartRuntimePrimitives.RequireValue(children__9813);
                description.Add($"estimated child count: {DartRuntimePrimitives.RequireValue(children__9813__value9855)}");
            }
        }
        catch (Exception e__9957)
        {
            description.Add($"estimated child count: EXCEPTION ({DartRuntimePrimitives.RuntimeType(e__9957)})");
        }
    }

}

internal class _SaltedValueKey__scroll_delegate : global::Doroti.Generated.Framework.Foundation.ValueKey<global::Doroti.Generated.Framework.Foundation.Key>
{
    internal _SaltedValueKey__scroll_delegate(global::Doroti.Generated.Framework.Foundation.Key value) : base(value)
    {
    }

}

public delegate long? ChildIndexGetter(global::Doroti.Generated.Framework.Foundation.Key key);

public class SliverChildBuilderDelegate : SliverChildDelegate
{
    public virtual global::System.Func<BuildContext, long, Widget?> builder { get; private set; } = default!;
    public virtual long? childCount { get; private set; }
    public virtual bool addAutomaticKeepAlives { get; private set; } = default!;
    public virtual bool addRepaintBoundaries { get; private set; } = default!;
    public virtual bool addSemanticIndexes { get; private set; } = default!;
    public virtual long semanticIndexOffset { get; private set; } = default!;
    public virtual global::System.Func<Widget, long, long?> semanticIndexCallback { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findChildIndexCallback { get; private set; }

    public SliverChildBuilderDelegate(global::System.Func<BuildContext, long, Widget?> builder, global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long? childCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, global::System.Func<Widget, long, long?> semanticIndexCallback = default!, long semanticIndexOffset = 0)
    {
        global::System.Func<Widget, long, long?> __semanticIndexCallback = semanticIndexCallback ?? ((widget, index) => Scroll_delegateLibrary._kDefaultSemanticIndexCallback(widget, index));
        this.builder = builder;
        this.findChildIndexCallback = findChildIndexCallback;
        this.childCount = childCount;
        this.addAutomaticKeepAlives = addAutomaticKeepAlives;
        this.addRepaintBoundaries = addRepaintBoundaries;
        this.addSemanticIndexes = addSemanticIndexes;
        this.semanticIndexCallback = __semanticIndexCallback;
        this.semanticIndexOffset = semanticIndexOffset;
    }

    public override long? findIndexByKey(global::Doroti.Generated.Framework.Foundation.Key key)
    {
        if ((this.findChildIndexCallback is null))
        {
            return null;
        }
        global::Doroti.Generated.Framework.Foundation.Key childKey__22461 = default!;
        if ((key is _SaltedValueKey__scroll_delegate))
        {
            _SaltedValueKey__scroll_delegate key__as22479 = (_SaltedValueKey__scroll_delegate)key;
            _SaltedValueKey__scroll_delegate saltedValueKey__22533 = ((_SaltedValueKey__scroll_delegate)key__as22479);
            childKey__22461 = saltedValueKey__22533.value;
        }
        else
        {
            childKey__22461 = key;
        }
        return this.findChildIndexCallback!(childKey__22461);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget? build(BuildContext context, long index)
    {
        if (((index < 0L) || (((this.childCount is not null) && (index >= DartRuntimePrimitives.RequireValue(this.childCount))))))
        {
            return ((Widget)(object)null);
        }
        Widget? child__22900 = default!;
        try
        {
            child__22900 = this.builder(context, index);
        }
        catch (Exception exception__22969)
        {
            var stackTrace__22980 = new System.Diagnostics.StackTrace();
            child__22900 = Scroll_delegateLibrary._createErrorWidget(exception__22969, stackTrace__22980);
        }
        if ((child__22900 is null))
        {
            return ((Widget)(object)null);
        }
        global::Doroti.Generated.Framework.Foundation.Key? key__23122 = ((global::Doroti.Generated.Framework.Foundation.Key?)(object?)((((Widget)child__22900).key is not null) ? new _SaltedValueKey__scroll_delegate(((Widget)child__22900).key!) : null));
        if (this.addRepaintBoundaries)
        {
            child__22900 = DartRuntimePrimitives.ConvertValue<Widget>(new RepaintBoundary(child: child__22900));
        }
        if (this.addSemanticIndexes)
        {
            long? semanticIndex__23314 = this.semanticIndexCallback(child__22900, index);
            if ((semanticIndex__23314 is not null))
            {
                long semanticIndex__23314__value23377 = DartRuntimePrimitives.RequireValue(semanticIndex__23314);
                child__22900 = DartRuntimePrimitives.ConvertValue<Widget>(new IndexedSemantics(index: (DartRuntimePrimitives.RequireValue(semanticIndex__23314__value23377) + this.semanticIndexOffset), child: child__22900));
            }
        }
        if (this.addAutomaticKeepAlives)
        {
            child__22900 = DartRuntimePrimitives.ConvertValue<Widget>(new AutomaticKeepAlive(child: new _SelectionKeepAlive__scroll_delegate(child: child__22900)));
        }
        return ((Widget?)(object?)new KeyedSubtree(key: key__23122, child: child__22900));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? estimatedChildCount => this.childCount;
    public override bool shouldRebuild(SliverChildDelegate oldDelegate) => true;
}

public class SliverChildListDelegate : SliverChildDelegate
{
    public virtual bool addAutomaticKeepAlives { get; private set; } = default!;
    public virtual bool addRepaintBoundaries { get; private set; } = default!;
    public virtual bool addSemanticIndexes { get; private set; } = default!;
    public virtual long semanticIndexOffset { get; private set; } = default!;
    public virtual global::System.Func<Widget, long, long?> semanticIndexCallback { get; private set; } = default!;
    public virtual List<Widget> children { get; private set; } = default!;
    internal virtual DartMap<global::Doroti.Generated.Framework.Foundation.Key?, long>? _keyToIndex { get; private set; }

    public SliverChildListDelegate(List<Widget> children, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, global::System.Func<Widget, long, long?> semanticIndexCallback = default!, long semanticIndexOffset = 0)
    {
        global::System.Func<Widget, long, long?> __semanticIndexCallback = semanticIndexCallback ?? ((widget, index) => Scroll_delegateLibrary._kDefaultSemanticIndexCallback(widget, index));
        this.children = children;
        this.addAutomaticKeepAlives = addAutomaticKeepAlives;
        this.addRepaintBoundaries = addRepaintBoundaries;
        this.addSemanticIndexes = addSemanticIndexes;
        this.semanticIndexCallback = __semanticIndexCallback;
        this.semanticIndexOffset = semanticIndexOffset;
        this._keyToIndex = new DartMap<global::Doroti.Generated.Framework.Foundation.Key?, long> { [null] = 0L }.cast<global::Doroti.Generated.Framework.Foundation.Key?, long>();
    }

    public static SliverChildListDelegate CreateFixed(List<Widget> children, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, global::System.Func<Widget, long, long?> semanticIndexCallback = default!, long semanticIndexOffset = 0)
    {
        var __instance = new SliverChildListDelegate(default!, default!, default!, default!, default!, default!);
        global::System.Func<Widget, long, long?> __semanticIndexCallback = semanticIndexCallback ?? ((widget, index) => Scroll_delegateLibrary._kDefaultSemanticIndexCallback(widget, index));
        __instance.children = children;
        __instance.addAutomaticKeepAlives = addAutomaticKeepAlives;
        __instance.addRepaintBoundaries = addRepaintBoundaries;
        __instance.addSemanticIndexes = addSemanticIndexes;
        __instance.semanticIndexCallback = __semanticIndexCallback;
        __instance.semanticIndexOffset = semanticIndexOffset;
        __instance._keyToIndex = null;
        return __instance;
    }

    internal virtual bool _isConstantInstance => DartRuntimePrimitives.ConvertValue<bool>((this._keyToIndex is null));
    internal virtual long? _findChildIndex(global::Doroti.Generated.Framework.Foundation.Key key)
    {
        if (this._isConstantInstance)
        {
            return null;
        }
        if (!this._keyToIndex!.ContainsKey(key))
        {
            long index__30343 = DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<long>(this._keyToIndex, null));
            while ((index__30343 < checked((long)(this.children.Count))))
            {
                Widget child__30432 = this.children[(int)(index__30343)];
                if ((((Widget)child__30432).key is not null))
                {
                    this._keyToIndex[DartRuntimePrimitives.RequireReference(((Widget)child__30432).key)] = index__30343;
                }
                if ((object.Equals(((Widget)child__30432).key, key)))
                {
                    this._keyToIndex[null] = (index__30343 + 1L);
                    return index__30343;
                }
                index__30343 += 1L;
            }
            this._keyToIndex[null] = index__30343;
        }
        else
        {
            return DartCollectionRuntime.NullableMapValue<long>(this._keyToIndex, key);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? findIndexByKey(global::Doroti.Generated.Framework.Foundation.Key key)
    {
        global::Doroti.Generated.Framework.Foundation.Key childKey__30899 = default!;
        if ((key is _SaltedValueKey__scroll_delegate))
        {
            _SaltedValueKey__scroll_delegate key__as30917 = (_SaltedValueKey__scroll_delegate)key;
            _SaltedValueKey__scroll_delegate saltedValueKey__30971 = ((_SaltedValueKey__scroll_delegate)key__as30917);
            childKey__30899 = saltedValueKey__30971.value;
        }
        else
        {
            childKey__30899 = key;
        }
        return _findChildIndex(childKey__30899);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget? build(BuildContext context, long index)
    {
        if (((index < 0L) || (index >= checked((long)(this.children.Count)))))
        {
            return ((Widget)(object)null);
        }
        Widget child__31264 = this.children[(int)(index)];
        global::Doroti.Generated.Framework.Foundation.Key? key__31304 = ((global::Doroti.Generated.Framework.Foundation.Key?)(object?)((((Widget)child__31264).key is not null) ? new _SaltedValueKey__scroll_delegate(((Widget)child__31264).key!) : null));
        if (this.addRepaintBoundaries)
        {
            child__31264 = DartRuntimePrimitives.ConvertValue<Widget>(new RepaintBoundary(child: child__31264));
        }
        if (this.addSemanticIndexes)
        {
            long? semanticIndex__31496 = this.semanticIndexCallback(child__31264, index);
            if ((semanticIndex__31496 is not null))
            {
                long semanticIndex__31496__value31559 = DartRuntimePrimitives.RequireValue(semanticIndex__31496);
                child__31264 = DartRuntimePrimitives.ConvertValue<Widget>(new IndexedSemantics(index: (DartRuntimePrimitives.RequireValue(semanticIndex__31496__value31559) + this.semanticIndexOffset), child: child__31264));
            }
        }
        if (this.addAutomaticKeepAlives)
        {
            child__31264 = DartRuntimePrimitives.ConvertValue<Widget>(new AutomaticKeepAlive(child: new _SelectionKeepAlive__scroll_delegate(child: child__31264)));
        }
        return ((Widget?)(object?)new KeyedSubtree(key: key__31304, child: child__31264));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? estimatedChildCount => checked((long)(this.children.Count));
    public override bool shouldRebuild(SliverChildDelegate oldDelegate)
    {
        var __oldDelegate = (SliverChildListDelegate)(object)oldDelegate;
        return (!object.Equals(this.children, ((SliverChildListDelegate)__oldDelegate).children));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SelectionKeepAlive__scroll_delegate : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;

    internal _SelectionKeepAlive__scroll_delegate(Widget child)
    {
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectionKeepAliveState__scroll_delegate());
}

internal class _SelectionKeepAliveState__scroll_delegate : State<_SelectionKeepAlive__scroll_delegate>, AutomaticKeepAliveClientMixin<_SelectionKeepAlive__scroll_delegate>, global::Doroti.Generated.Framework.Rendering.SelectionRegistrar
{
    internal virtual HashSet<global::Doroti.Generated.Framework.Rendering.Selectable>? _selectablesWithSelections { get; set; } = default;
    internal virtual DartMap<global::Doroti.Generated.Framework.Rendering.Selectable, global::System.Action>? _selectableAttachments { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Rendering.SelectionRegistrar? _registrar { get; set; } = default;
    internal virtual bool _wantKeepAlive { get; set; } = false;
    public virtual KeepAliveHandle? _keepAliveHandle { get; set; } = default;

    public virtual bool wantKeepAlive
    {
        get => this._wantKeepAlive;
        set
        {
            var __value = value;
            if ((this._wantKeepAlive != __value))
            {
                _wantKeepAlive = __value;
                updateKeepAlive();
            }
        }
    }
    public virtual global::System.Action listensTo(global::Doroti.Generated.Framework.Rendering.Selectable selectable)
    {
        return ((global::System.Action)(() => {
if (selectable.value.hasSelection)
{
    _updateSelectablesWithSelections(selectable, add: true);
}
else
{
    _updateSelectablesWithSelections(selectable, add: false);
}
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateSelectablesWithSelections(global::Doroti.Generated.Framework.Rendering.Selectable selectable, bool add)
    {
        if (add)
        {
            DartRuntimePrimitives.Assert(() => selectable.value.hasSelection);
            _selectablesWithSelections ??= new HashSet<global::Doroti.Generated.Framework.Rendering.Selectable>();
            this._selectablesWithSelections!.Add(selectable);
        }
        else
        {
            this._selectablesWithSelections?.Remove(selectable);
        }
        wantKeepAlive = ((this._selectablesWithSelections is { } __items33618 ? System.Linq.Enumerable.Any(__items33618) : (bool?)null) ?? false);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Generated.Framework.Rendering.SelectionRegistrar? newRegistrar__33782 = ((global::Doroti.Generated.Framework.Rendering.SelectionRegistrar?)(object?)SelectionContainer.maybeOf(this.context));
        if ((!object.Equals(this._registrar, newRegistrar__33782)))
        {
            if ((this._registrar is not null))
            {
                this._selectableAttachments?.Keys.forEach((__arg0) => ((global::System.Action<global::Doroti.Generated.Framework.Rendering.Selectable>)this._registrar!.remove)(__arg0));
            }
            _registrar = newRegistrar__33782;
            if ((this._registrar is not null))
            {
                this._selectableAttachments?.Keys.forEach((__arg0) => ((global::System.Action<global::Doroti.Generated.Framework.Rendering.Selectable>)this._registrar!.add)(__arg0));
            }
        }
    }

    public virtual void add(global::Doroti.Generated.Framework.Rendering.Selectable selectable)
    {
        global::System.Action attachment__34196 = ((global::System.Action)(object?)listensTo(selectable));
        selectable.addListener(() => attachment__34196());
        _selectableAttachments ??= new DartMap<global::Doroti.Generated.Framework.Rendering.Selectable, global::System.Action>();
        this._selectableAttachments![selectable] = (global::System.Action)attachment__34196;
        this._registrar!.add(selectable);
        if (selectable.value.hasSelection)
        {
            _updateSelectablesWithSelections(selectable, add: true);
        }
    }

    public virtual void remove(global::Doroti.Generated.Framework.Rendering.Selectable selectable)
    {
        if ((this._selectableAttachments is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => this._selectableAttachments!.ContainsKey(selectable));
        global::System.Action attachment__34732 = this._selectableAttachments!.remove(selectable)!;
        selectable.removeListener(() => attachment__34732());
        this._registrar!.remove(selectable);
        _updateSelectablesWithSelections(selectable, add: false);
    }

    public override void dispose()
    {
        if ((this._selectableAttachments is not null))
        {
            foreach (global::Doroti.Generated.Framework.Rendering.Selectable selectable__35037 in this._selectableAttachments!.Keys)
            {
                this._registrar!.remove(selectable__35037);
                selectable__35037.removeListener(this._selectableAttachments!.GetValueOrDefault(selectable__35037)!);
            }
            _selectableAttachments = null;
        }
        _selectablesWithSelections = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if ((this.wantKeepAlive && (this._keepAliveHandle is null)))
        {
            _ensureKeepAlive();
        }
        if ((this._registrar is null))
        {
            return ((_SelectionKeepAlive__scroll_delegate)this.widget).child;
        }
        return ((Widget)(object?)new SelectionRegistrarScope(registrar: this, child: ((_SelectionKeepAlive__scroll_delegate)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _ensureKeepAlive()
    {
        DartRuntimePrimitives.Assert(() => (this._keepAliveHandle is null));
        this._keepAliveHandle = new KeepAliveHandle();
        new KeepAliveNotification(this._keepAliveHandle!).dispatch(this.context);
    }

    public virtual void _releaseKeepAlive()
    {
        this._keepAliveHandle!.dispose();
        this._keepAliveHandle = null;
    }

    public virtual void updateKeepAlive()
    {
        if (this.wantKeepAlive)
        {
            if ((this._keepAliveHandle is null))
            {
                _ensureKeepAlive();
            }
        }
        else
        {
            if ((this._keepAliveHandle is not null))
            {
                _releaseKeepAlive();
            }
        }
    }

    public override void initState()
    {
        base.initState();
        if (this.wantKeepAlive)
        {
            _ensureKeepAlive();
        }
    }

    public override void deactivate()
    {
        if ((this._keepAliveHandle is not null))
        {
            _releaseKeepAlive();
        }
        base.deactivate();
    }

}

public static partial class Scroll_delegateLibrary
{
    internal static Widget _createErrorWidget(object exception, global::System.Diagnostics.StackTrace stackTrace)
    {
        var details__35653 = new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stackTrace, library: "widgets library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("building"));
        FlutterError.reportError(details__35653);
        return ErrorWidget.builder(details__35653);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class TwoDimensionalChildDelegate : global::Doroti.Generated.Framework.Foundation.ChangeNotifier
{
    protected TwoDimensionalChildDelegate()
    {
    }

    public abstract Widget? build(BuildContext context, ChildVicinity vicinity);
    public abstract bool shouldRebuild(TwoDimensionalChildDelegate oldDelegate);
}

public class TwoDimensionalChildBuilderDelegate : TwoDimensionalChildDelegate
{
    public virtual global::System.Func<BuildContext, ChildVicinity, Widget?> builder { get; private set; } = default!;
    internal virtual long? _maxXIndex { get; set; } = default;
    internal virtual long? _maxYIndex { get; set; } = default;
    public virtual bool addRepaintBoundaries { get; private set; } = default!;
    public virtual bool addAutomaticKeepAlives { get; private set; } = default!;

    public TwoDimensionalChildBuilderDelegate(global::System.Func<BuildContext, ChildVicinity, Widget?> builder, long? maxXIndex = null, long? maxYIndex = null, bool addRepaintBoundaries = true, bool addAutomaticKeepAlives = true)
    {
        this.builder = builder;
        this.addRepaintBoundaries = addRepaintBoundaries;
        this.addAutomaticKeepAlives = addAutomaticKeepAlives;
        this._maxYIndex = maxYIndex;
        this._maxXIndex = maxXIndex;
        System.Diagnostics.Debug.Assert(((maxYIndex is null) || (maxYIndex >= -1L)));
        System.Diagnostics.Debug.Assert(((maxXIndex is null) || (maxXIndex >= -1L)));
    }

    public virtual long? maxXIndex
    {
        get => this._maxXIndex;
        set
        {
            var __value = value;
            if ((__value == this.maxXIndex))
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => ((__value is null) || (__value >= -1L)));
            _maxXIndex = __value;
            notifyListeners();
        }
    }
    public virtual long? maxYIndex
    {
        get => this._maxYIndex;
        set
        {
            var __value = value;
            if ((this.maxYIndex == __value))
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => ((__value is null) || (__value >= -1L)));
            _maxYIndex = __value;
            notifyListeners();
        }
    }
    public override Widget? build(BuildContext context, ChildVicinity vicinity)
    {
        if (((((ChildVicinity)vicinity).xIndex < 0L) || (((this.maxXIndex is not null) && (((ChildVicinity)vicinity).xIndex > DartRuntimePrimitives.RequireValue(this.maxXIndex))))))
        {
            return ((Widget)(object)null);
        }
        if (((((ChildVicinity)vicinity).yIndex < 0L) || (((this.maxYIndex is not null) && (((ChildVicinity)vicinity).yIndex > DartRuntimePrimitives.RequireValue(this.maxYIndex))))))
        {
            return ((Widget)(object)null);
        }
        Widget? child__43820 = default!;
        try
        {
            child__43820 = this.builder(context, vicinity);
        }
        catch (Exception exception__43892)
        {
            var stackTrace__43903 = new System.Diagnostics.StackTrace();
            child__43820 = Scroll_delegateLibrary._createErrorWidget(exception__43892, stackTrace__43903);
        }
        if ((child__43820 is null))
        {
            return ((Widget)(object)null);
        }
        if (this.addRepaintBoundaries)
        {
            child__43820 = DartRuntimePrimitives.ConvertValue<Widget>(new RepaintBoundary(child: child__43820));
        }
        if (this.addAutomaticKeepAlives)
        {
            child__43820 = DartRuntimePrimitives.ConvertValue<Widget>(new AutomaticKeepAlive(child: new _SelectionKeepAlive__scroll_delegate(child: child__43820)));
        }
        return child__43820;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(TwoDimensionalChildDelegate oldDelegate) => true;
}

public class TwoDimensionalChildListDelegate : TwoDimensionalChildDelegate
{
    public virtual List<List<Widget>> children { get; private set; } = default!;
    public virtual bool addRepaintBoundaries { get; private set; } = default!;
    public virtual bool addAutomaticKeepAlives { get; private set; } = default!;

    public TwoDimensionalChildListDelegate(bool addRepaintBoundaries = true, bool addAutomaticKeepAlives = true, List<List<Widget>> children = default!)
    {
        this.addRepaintBoundaries = addRepaintBoundaries;
        this.addAutomaticKeepAlives = addAutomaticKeepAlives;
        this.children = children;
    }

    public override Widget? build(BuildContext context, ChildVicinity vicinity)
    {
        if (((((ChildVicinity)vicinity).yIndex < 0L) || (((ChildVicinity)vicinity).yIndex >= checked((long)(this.children.Count)))))
        {
            return ((Widget)(object)null);
        }
        if (((((ChildVicinity)vicinity).xIndex < 0L) || (((ChildVicinity)vicinity).xIndex >= checked((long)(this.children[(int)(((ChildVicinity)vicinity).yIndex)].Count)))))
        {
            return ((Widget)(object)null);
        }
        Widget child__47473 = this.children[(int)(((ChildVicinity)vicinity).yIndex)][(int)(((ChildVicinity)vicinity).xIndex)];
        if (this.addRepaintBoundaries)
        {
            child__47473 = DartRuntimePrimitives.ConvertValue<Widget>(new RepaintBoundary(child: child__47473));
        }
        if (this.addAutomaticKeepAlives)
        {
            child__47473 = DartRuntimePrimitives.ConvertValue<Widget>(new AutomaticKeepAlive(child: new _SelectionKeepAlive__scroll_delegate(child: child__47473)));
        }
        return child__47473;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(TwoDimensionalChildDelegate oldDelegate)
    {
        var __oldDelegate = (TwoDimensionalChildListDelegate)(object)oldDelegate;
        return (!object.Equals(this.children, ((TwoDimensionalChildListDelegate)__oldDelegate).children));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
