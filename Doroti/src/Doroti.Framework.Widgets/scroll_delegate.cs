// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_delegate.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

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
    public virtual long? findIndexByKey(global::Doroti.Framework.Foundation.Key key) => null;
    public override string ToString()
    {
        var description = new List<string>();
        debugFillDescription(description);
        return $"{DiagnosticsLibrary.describeIdentity(this)}({string.Join(", ", description)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillDescription(List<string> description)
    {
        try
        {
            long? children = estimatedChildCount;
            if (children is not null)
            {
                long children__9813__value9855 = DartRuntimePrimitives.RequireValue(children);
                description.Add($"estimated child count: {DartRuntimePrimitives.RequireValue(children__9813__value9855)}");
            }
        }
        catch (Exception e)
        {
            description.Add($"estimated child count: EXCEPTION ({DartRuntimePrimitives.RuntimeType(e)})");
        }
    }

}

internal class _SaltedValueKey__scroll_delegate : global::Doroti.Framework.Foundation.ValueKey<global::Doroti.Framework.Foundation.Key>
{
    internal _SaltedValueKey__scroll_delegate(global::Doroti.Framework.Foundation.Key value) : base(value)
    {
    }

}

public delegate long? ChildIndexGetter(global::Doroti.Framework.Foundation.Key key);

public class SliverChildBuilderDelegate : SliverChildDelegate
{
    public virtual global::System.Func<BuildContext, long, Widget?> builder { get; private set; } = default!;
    public virtual long? childCount { get; private set; }
    public virtual bool addAutomaticKeepAlives { get; private set; } = default!;
    public virtual bool addRepaintBoundaries { get; private set; } = default!;
    public virtual bool addSemanticIndexes { get; private set; } = default!;
    public virtual long semanticIndexOffset { get; private set; } = default!;
    public virtual global::System.Func<Widget, long, long?> semanticIndexCallback { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback { get; private set; }

    public SliverChildBuilderDelegate(global::System.Func<BuildContext, long, Widget?> builder, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long? childCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, global::System.Func<Widget, long, long?> semanticIndexCallback = default!, long semanticIndexOffset = 0)
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

    public override long? findIndexByKey(global::Doroti.Framework.Foundation.Key key)
    {
        if (findChildIndexCallback is null)
        {
            return null;
        }
        global::Doroti.Framework.Foundation.Key childKey = default!;
        if (key is _SaltedValueKey__scroll_delegate)
        {
            _SaltedValueKey__scroll_delegate key__as22479 = (_SaltedValueKey__scroll_delegate)key;
            _SaltedValueKey__scroll_delegate saltedValueKey = key__as22479;
            childKey = saltedValueKey.value;
        }
        else
        {
            childKey = key;
        }
        return findChildIndexCallback!(childKey);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget? build(BuildContext context, long index)
    {
        if ((index < 0L) || (childCount is not null) && (index >= DartRuntimePrimitives.RequireValue(childCount)))
        {
            return null;
        }
        Widget? childLocal = default!;
        try
        {
            childLocal = builder(context, index);
        }
        catch (Exception exception)
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            childLocal = Scroll_delegateLibrary._createErrorWidget(exception, stackTrace);
        }
        if (childLocal is null)
        {
            return null;
        }
        global::Doroti.Framework.Foundation.Key? keyLocal = (childLocal.key is not null) ? new _SaltedValueKey__scroll_delegate(childLocal.key!) : null;
        if (addRepaintBoundaries)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new RepaintBoundary(child: childLocal));
        }
        if (addSemanticIndexes)
        {
            long? semanticIndex = semanticIndexCallback(childLocal, index);
            if (semanticIndex is not null)
            {
                long semanticIndex__23314__value23377 = DartRuntimePrimitives.RequireValue(semanticIndex);
                childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new IndexedSemantics(index: DartRuntimePrimitives.RequireValue(semanticIndex__23314__value23377) + semanticIndexOffset, child: childLocal));
            }
        }
        if (addAutomaticKeepAlives)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new AutomaticKeepAlive(child: new _SelectionKeepAlive__scroll_delegate(child: childLocal)));
        }
        return (Widget?)new KeyedSubtree(key: keyLocal, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? estimatedChildCount => childCount;
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
    internal virtual DartMap<global::Doroti.Framework.Foundation.Key?, long>? _keyToIndex { get; private set; }

    public SliverChildListDelegate(List<Widget> children, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, global::System.Func<Widget, long, long?> semanticIndexCallback = default!, long semanticIndexOffset = 0)
    {
        global::System.Func<Widget, long, long?> __semanticIndexCallback = semanticIndexCallback ?? ((widget, index) => Scroll_delegateLibrary._kDefaultSemanticIndexCallback(widget, index));
        this.children = children;
        this.addAutomaticKeepAlives = addAutomaticKeepAlives;
        this.addRepaintBoundaries = addRepaintBoundaries;
        this.addSemanticIndexes = addSemanticIndexes;
        this.semanticIndexCallback = __semanticIndexCallback;
        this.semanticIndexOffset = semanticIndexOffset;
        _keyToIndex = new DartMap<global::Doroti.Framework.Foundation.Key?, long> { [null] = 0L }.cast<global::Doroti.Framework.Foundation.Key?, long>();
    }

    public static SliverChildListDelegate CreateFixed(List<Widget> children, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, global::System.Func<Widget, long, long?> semanticIndexCallback = default!, long semanticIndexOffset = 0)
    {
        var __instance = new SliverChildListDelegate(children, addAutomaticKeepAlives, addRepaintBoundaries, addSemanticIndexes, semanticIndexCallback, semanticIndexOffset);
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

    internal virtual bool _isConstantInstance => DartRuntimePrimitives.ConvertValue<bool>(_keyToIndex is null);
    internal virtual long? _findChildIndex(global::Doroti.Framework.Foundation.Key key)
    {
        if (_isConstantInstance)
        {
            return null;
        }
        if (!_keyToIndex!.ContainsKey(key))
        {
            long index = DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<long>(_keyToIndex, null));
            while (index < checked(children.Count))
            {
                Widget child = children[(int)index];
                if (child.key is not null)
                {
                    _keyToIndex[DartRuntimePrimitives.RequireReference(child.key)] = index;
                }
                if (Equals(child.key, key))
                {
                    _keyToIndex[null] = index + 1L;
                    return index;
                }
                index += 1L;
            }
            _keyToIndex[null] = index;
        }
        else
        {
            return DartCollectionRuntime.NullableMapValue<long>(_keyToIndex, key);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? findIndexByKey(global::Doroti.Framework.Foundation.Key key)
    {
        global::Doroti.Framework.Foundation.Key childKey = default!;
        if (key is _SaltedValueKey__scroll_delegate)
        {
            _SaltedValueKey__scroll_delegate key__as30917 = (_SaltedValueKey__scroll_delegate)key;
            _SaltedValueKey__scroll_delegate saltedValueKey = key__as30917;
            childKey = saltedValueKey.value;
        }
        else
        {
            childKey = key;
        }
        return _findChildIndex(childKey);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget? build(BuildContext context, long index)
    {
        if ((index < 0L) || (index >= checked(children.Count)))
        {
            return null;
        }
        Widget childLocal = children[(int)index];
        global::Doroti.Framework.Foundation.Key? keyLocal = (childLocal.key is not null) ? new _SaltedValueKey__scroll_delegate(childLocal.key!) : null;
        if (addRepaintBoundaries)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new RepaintBoundary(child: childLocal));
        }
        if (addSemanticIndexes)
        {
            long? semanticIndex = semanticIndexCallback(childLocal, index);
            if (semanticIndex is not null)
            {
                long semanticIndex__31496__value31559 = DartRuntimePrimitives.RequireValue(semanticIndex);
                childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new IndexedSemantics(index: DartRuntimePrimitives.RequireValue(semanticIndex__31496__value31559) + semanticIndexOffset, child: childLocal));
            }
        }
        if (addAutomaticKeepAlives)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new AutomaticKeepAlive(child: new _SelectionKeepAlive__scroll_delegate(child: childLocal)));
        }
        return (Widget?)new KeyedSubtree(key: keyLocal, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? estimatedChildCount => checked(children.Count);
    public override bool shouldRebuild(SliverChildDelegate oldDelegate)
    {
        var __oldDelegate = (SliverChildListDelegate)oldDelegate;
        return !Equals(children, __oldDelegate.children);
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

internal class _SelectionKeepAliveState__scroll_delegate : State<_SelectionKeepAlive__scroll_delegate>, AutomaticKeepAliveClientMixin<_SelectionKeepAlive__scroll_delegate>, global::Doroti.Framework.Rendering.SelectionRegistrar
{
    internal virtual HashSet<global::Doroti.Framework.Rendering.Selectable>? _selectablesWithSelections { get; set; } = default;
    internal virtual DartMap<global::Doroti.Framework.Rendering.Selectable, global::System.Action>? _selectableAttachments { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.SelectionRegistrar? _registrar { get; set; } = default;
    internal virtual bool _wantKeepAlive { get; set; } = false;
    public virtual KeepAliveHandle? _keepAliveHandle { get; set; } = default;

    public virtual bool wantKeepAlive
    {
        get => _wantKeepAlive;
        set
        {
            var __value = value;
            if (_wantKeepAlive != __value)
            {
                _wantKeepAlive = __value;
                updateKeepAlive();
            }
        }
    }
    public virtual global::System.Action listensTo(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        return () =>
        {
            if (selectable.value.hasSelection)
            {
                _updateSelectablesWithSelections(selectable, add: true);
            }
            else
            {
                _updateSelectablesWithSelections(selectable, add: false);
            }
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateSelectablesWithSelections(global::Doroti.Framework.Rendering.Selectable selectable, bool add)
    {
        if (add)
        {
            DartRuntimePrimitives.Assert(() => selectable.value.hasSelection);
            _selectablesWithSelections ??= new HashSet<global::Doroti.Framework.Rendering.Selectable>();
            _selectablesWithSelections!.Add(selectable);
        }
        else
        {
            _selectablesWithSelections?.Remove(selectable);
        }
        wantKeepAlive = (_selectablesWithSelections is { } __items33618 ? System.Linq.Enumerable.Any(__items33618) : (bool?)null) ?? false;
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Rendering.SelectionRegistrar? newRegistrar = SelectionContainer.maybeOf(context);
        if (!Equals(_registrar, newRegistrar))
        {
            if (_registrar is not null)
            {
                _selectableAttachments?.Keys.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.Selectable>)_registrar!.remove)(__arg0));
            }
            _registrar = newRegistrar;
            if (_registrar is not null)
            {
                _selectableAttachments?.Keys.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.Selectable>)_registrar!.add)(__arg0));
            }
        }
    }

    public virtual void add(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        global::System.Action attachment = listensTo(selectable);
        selectable.addListener(attachment);
        _selectableAttachments ??= new DartMap<global::Doroti.Framework.Rendering.Selectable, global::System.Action>();
        _selectableAttachments![selectable] = attachment;
        _registrar!.add(selectable);
        if (selectable.value.hasSelection)
        {
            _updateSelectablesWithSelections(selectable, add: true);
        }
    }

    public virtual void remove(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        if (_selectableAttachments is null)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => _selectableAttachments!.ContainsKey(selectable));
        global::System.Action attachment = _selectableAttachments!.remove(selectable)!;
        selectable.removeListener(attachment);
        _registrar!.remove(selectable);
        _updateSelectablesWithSelections(selectable, add: false);
    }

    public override void dispose()
    {
        if (_selectableAttachments is not null)
        {
            foreach (global::Doroti.Framework.Rendering.Selectable selectable in _selectableAttachments!.Keys)
            {
                _registrar!.remove(selectable);
                selectable.removeListener(_selectableAttachments!.GetValueOrDefault(selectable)!);
            }
            _selectableAttachments = null;
        }
        _selectablesWithSelections = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (wantKeepAlive && (_keepAliveHandle is null))
        {
            _ensureKeepAlive();
        }
        if (_registrar is null)
        {
            return widget.child;
        }
        return new SelectionRegistrarScope(registrar: this, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _ensureKeepAlive()
    {
        DartRuntimePrimitives.Assert(() => _keepAliveHandle is null);
        _keepAliveHandle = new KeepAliveHandle();
        new KeepAliveNotification(_keepAliveHandle!).dispatch(context);
    }

    public virtual void _releaseKeepAlive()
    {
        _keepAliveHandle!.dispose();
        _keepAliveHandle = null;
    }

    public virtual void updateKeepAlive()
    {
        if (wantKeepAlive)
        {
            if (_keepAliveHandle is null)
            {
                _ensureKeepAlive();
            }
        }
        else
        {
            if (_keepAliveHandle is not null)
            {
                _releaseKeepAlive();
            }
        }
    }

    public override void initState()
    {
        base.initState();
        if (wantKeepAlive)
        {
            _ensureKeepAlive();
        }
    }

    public override void deactivate()
    {
        if (_keepAliveHandle is not null)
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
        var details = new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stackTrace, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("building"));
        FlutterError.reportError(details);
        return ErrorWidget.builder(details);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class TwoDimensionalChildDelegate : global::Doroti.Framework.Foundation.ChangeNotifier
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
        _maxYIndex = maxYIndex;
        _maxXIndex = maxXIndex;
        System.Diagnostics.Debug.Assert((maxYIndex is null) || (maxYIndex >= -1L));
        System.Diagnostics.Debug.Assert((maxXIndex is null) || (maxXIndex >= -1L));
    }

    public virtual long? maxXIndex
    {
        get => _maxXIndex;
        set
        {
            var __value = value;
            if (__value == maxXIndex)
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => (__value is null) || (__value >= -1L));
            _maxXIndex = __value;
            notifyListeners();
        }
    }
    public virtual long? maxYIndex
    {
        get => _maxYIndex;
        set
        {
            var __value = value;
            if (maxYIndex == __value)
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => (__value is null) || (__value >= -1L));
            _maxYIndex = __value;
            notifyListeners();
        }
    }
    public override Widget? build(BuildContext context, ChildVicinity vicinity)
    {
        if ((vicinity.xIndex < 0L) || (maxXIndex is not null) && (vicinity.xIndex > DartRuntimePrimitives.RequireValue(maxXIndex)))
        {
            return null;
        }
        if ((vicinity.yIndex < 0L) || (maxYIndex is not null) && (vicinity.yIndex > DartRuntimePrimitives.RequireValue(maxYIndex)))
        {
            return null;
        }
        Widget? childLocal = default!;
        try
        {
            childLocal = builder(context, vicinity);
        }
        catch (Exception exception)
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            childLocal = Scroll_delegateLibrary._createErrorWidget(exception, stackTrace);
        }
        if (childLocal is null)
        {
            return null;
        }
        if (addRepaintBoundaries)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new RepaintBoundary(child: childLocal));
        }
        if (addAutomaticKeepAlives)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new AutomaticKeepAlive(child: new _SelectionKeepAlive__scroll_delegate(child: childLocal)));
        }
        return childLocal;
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
        if ((vicinity.yIndex < 0L) || (vicinity.yIndex >= checked(children.Count)))
        {
            return null;
        }
        if ((vicinity.xIndex < 0L) || (vicinity.xIndex >= checked(children[(int)vicinity.yIndex].Count)))
        {
            return null;
        }
        Widget childLocal = children[(int)vicinity.yIndex][(int)vicinity.xIndex];
        if (addRepaintBoundaries)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new RepaintBoundary(child: childLocal));
        }
        if (addAutomaticKeepAlives)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new AutomaticKeepAlive(child: new _SelectionKeepAlive__scroll_delegate(child: childLocal)));
        }
        return childLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(TwoDimensionalChildDelegate oldDelegate)
    {
        var __oldDelegate = (TwoDimensionalChildListDelegate)oldDelegate;
        return !Equals(children, __oldDelegate.children);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
