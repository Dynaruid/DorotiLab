// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/tab_scaffold.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public class CupertinoTabController : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual bool _isDisposed { get; set; } = false;
    internal virtual long _index { get; set; } = default!;

    public CupertinoTabController(long initialIndex = 0)
    {
        _index = initialIndex;
        System.Diagnostics.Debug.Assert(initialIndex >= 0L);
    }

    public virtual long index
    {
        get => _index;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value >= 0L);
            if (_index == __value)
            {
                return;
            }
            _index = __value;
            notifyListeners();
        }
    }
    public override void dispose()
    {
        base.dispose();
        _isDisposed = true;
    }

}

public class CupertinoTabScaffold : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual CupertinoTabBar tabBar { get; private set; } = default!;
    public virtual CupertinoTabController? controller { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget> tabBuilder { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool resizeToAvoidBottomInset { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }

    public CupertinoTabScaffold(global::Doroti.Framework.Foundation.Key? key = null, CupertinoTabBar tabBar = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget> tabBuilder = default!, CupertinoTabController? controller = null, Color? backgroundColor = null, bool resizeToAvoidBottomInset = true, string? restorationId = null) : base(key: key)
    {
        this.tabBar = tabBar;
        this.tabBuilder = tabBuilder;
        this.controller = controller;
        this.backgroundColor = backgroundColor;
        this.resizeToAvoidBottomInset = resizeToAvoidBottomInset;
        this.restorationId = restorationId;
        System.Diagnostics.Debug.Assert((controller is null) || (controller.index < checked(tabBar.items.Count)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTabScaffoldState__tab_scaffold());
}

internal class _CupertinoTabScaffoldState__tab_scaffold : global::Doroti.Framework.Widgets.State<CupertinoTabScaffold>, global::Doroti.Framework.Widgets.RestorationMixin<CupertinoTabScaffold>
{
    internal virtual RestorableCupertinoTabController? _internalController { get; set; } = default;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action> _properties { get; set; } = new DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action>();
    public virtual List<global::Doroti.Framework.Widgets.IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    internal virtual CupertinoTabController _controller => DartRuntimePrimitives.ConvertValue<CupertinoTabController>(widget.controller ?? _internalController!.value);
    public virtual string? restorationId => widget.restorationId;
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        _restoreInternalController();
    }

    internal virtual void _restoreInternalController()
    {
        if (_internalController is not null)
        {
            registerForRestoration(_internalController!, "controller");
            _internalController!.value.addListener(_onCurrentIndexChange);
        }
    }

    public override void initState()
    {
        base.initState();
        _updateTabController();
    }

    internal virtual void _updateTabController(CupertinoTabController? oldWidgetController = null)
    {
        if ((widget.controller is null) && (_internalController is null))
        {
            _internalController = new RestorableCupertinoTabController(initialIndex: widget.tabBar.currentIndex);
            if (!restorePending)
            {
                _restoreInternalController();
            }
        }
        if ((widget.controller is not null) && (_internalController is not null))
        {
            unregisterFromRestoration(_internalController!);
            _internalController!.dispose();
            _internalController = null;
        }
        if (!Equals(oldWidgetController, widget.controller))
        {
            if (oldWidgetController?._isDisposed == false)
            {
                oldWidgetController!.removeListener(_onCurrentIndexChange);
            }
            widget.controller?.addListener(_onCurrentIndexChange);
        }
    }

    internal virtual void _onCurrentIndexChange()
    {
        DartRuntimePrimitives.Assert(() => (_controller.index >= 0L) && (_controller.index < checked(widget.tabBar.items.Count)), () => (object?)$"The {GetType()}'s current index {_controller.index} is " + $"out of bounds for the tab bar with {checked((long)widget.tabBar.items.Count)} tabs");
        setState(() =>
        {
        });
    }

    public override void didUpdateWidget(CupertinoTabScaffold oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if (!Equals(widget.controller, oldWidget.controller))
        {
            _updateTabController(oldWidget.controller);
        }
        else
        {
            if (_controller.index >= checked(widget.tabBar.items.Count))
            {
                _controller.index = checked(widget.tabBar.items.Count) - 1L;
            }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.MediaQueryData existingMediaQuery = MediaQuery.of(context);
        global::Doroti.Framework.Widgets.MediaQueryData newMediaQuery = MediaQuery.of(context);
        global::Doroti.Framework.Widgets.Widget content = new _TabSwitchingView__tab_scaffold(currentTabIndex: _controller.index, tabCount: checked(widget.tabBar.items.Count), tabBuilder: widget.tabBuilder);
        global::Doroti.Framework.Painting.EdgeInsets contentPadding = EdgeInsets.zero;
        if (widget.resizeToAvoidBottomInset)
        {
            newMediaQuery = newMediaQuery.removeViewInsets(removeBottom: true);
            contentPadding = EdgeInsets.CreateOnly(bottom: existingMediaQuery.viewInsets.bottom);
        }
        if (!widget.resizeToAvoidBottomInset || (widget.tabBar.preferredSize.height > existingMediaQuery.viewInsets.bottom))
        {
            double bottomPadding = widget.tabBar.preferredSize.height + existingMediaQuery.padding.bottom;
            if (widget.tabBar.opaque(context))
            {
                contentPadding = EdgeInsets.CreateOnly(bottom: bottomPadding);
                newMediaQuery = newMediaQuery.removePadding(removeBottom: true);
            }
            else
            {
                newMediaQuery = newMediaQuery.copyWith(padding: newMediaQuery.padding.copyWith(bottom: bottomPadding));
            }
        }
        content = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MediaQuery(data: newMediaQuery, child: new global::Doroti.Framework.Widgets.Padding(padding: contentPadding, child: content)));
        return new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: CupertinoDynamicColor.maybeResolve(widget.backgroundColor, context) ?? CupertinoTheme.of(context).scaffoldBackgroundColor), child: new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(content), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(MediaQuery.withNoTextScaling(child: new global::Doroti.Framework.Widgets.Align(alignment: Alignment.bottomCenter, child: widget.tabBar.copyWith(currentIndex: _controller.index, onTap: (newIndex) => {
_controller.index = newIndex;
widget.tabBar.onTap?.Invoke(newIndex);
})))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        if (widget.controller?._isDisposed == false)
        {
            _controller.removeListener(_onCurrentIndexChange);
        }
        _internalController?.dispose();
        _properties.forEach((property, listener) =>
        {
            if (!property._disposed)
            {
                property.removeListener(listener);
            }
        });
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => _bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(global::Doroti.Framework.Widgets.IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => (property._restorationId is null) || _debugDoingRestore && (property._restorationId == restorationId), () => (object?)$"Property is already registered under {property._restorationId}.");
        DartRuntimePrimitives.Assert(() => _debugDoingRestore || !_properties.Keys.map<global::Doroti.Framework.Widgets.IRestorableProperty, string?>((r) => r._restorationId).contains(restorationId), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue ? property.fromPrimitivesObject(bucket!.read<object>(restorationId)) : property.createDefaultValueObject();
        if (!property.isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if (bucket is null)
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener(listener);
            _properties[property] = listener;
        }
        DartRuntimePrimitives.Assert(() => (property._restorationId == restorationId) && Equals(property._owner, this) && _properties.ContainsKey(property));
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(global::Doroti.Framework.Widgets.IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => Equals(property._owner, this));
        _bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((_currentParent is null) || (_bucket?.restorationId == restorationId) || restorePending)
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            DartRuntimePrimitives.Assert(() => (_bucket is null) || (oldBucket is null));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (_firstRestorePending)
            {
                return true;
            }
            if (restorationId is null)
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent)) && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>(_debugPropertiesWaitingForReregistration is not null);
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
    }

    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(_debugPropertiesWaitingForReregistration!.map<global::Doroti.Framework.Widgets.IRestorableProperty, global::Doroti.Framework.Foundation.DiagnosticsNode>((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {property._restorationId}"))); return __collection41817; }))()));
                }
                _debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => Equals(_bucket, newBucketLocal));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => _bucket is not null);
        DartRuntimePrimitives.Assert(() => !restorePending);
        _bucket!.rename(restorationId!);
        parent.adoptChild(_bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Widgets.IRestorableProperty>)_updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(global::Doroti.Framework.Widgets.IRestorableProperty property)
    {
        if (property.enabled)
        {
            _bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            _bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(global::Doroti.Framework.Widgets.IRestorableProperty property)
    {
        global::System.Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
    }

}

public class _TabSwitchingView__tab_scaffold : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual long currentTabIndex { get; private set; } = default!;
    public virtual long tabCount { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget> tabBuilder { get; private set; } = default!;

    internal _TabSwitchingView__tab_scaffold(long currentTabIndex, long tabCount, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget> tabBuilder)
    {
        this.currentTabIndex = currentTabIndex;
        this.tabCount = tabCount;
        this.tabBuilder = tabBuilder;
        System.Diagnostics.Debug.Assert(tabCount > 0L);
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TabSwitchingViewState__tab_scaffold());
}

public class _TabSwitchingViewState__tab_scaffold : global::Doroti.Framework.Widgets.State<_TabSwitchingView__tab_scaffold>
{
    public virtual List<bool> shouldBuildTab { get; private set; } = new List<bool>();
    public virtual List<global::Doroti.Framework.Widgets.FocusScopeNode> tabFocusNodes { get; private set; } = new List<global::Doroti.Framework.Widgets.FocusScopeNode>();
    public virtual List<global::Doroti.Framework.Widgets.FocusScopeNode> discardedNodes { get; private set; } = new List<global::Doroti.Framework.Widgets.FocusScopeNode>();

    public override void initState()
    {
        base.initState();
        shouldBuildTab.AddRange(new List<bool>(Enumerable.Repeat<bool>(false, checked((int)widget.tabCount))).Cast<bool>());
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _focusActiveTab();
    }

    public override void didUpdateWidget(_TabSwitchingView__tab_scaffold oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        long lengthDiff = widget.tabCount - checked(shouldBuildTab.Count);
        if (lengthDiff > 0L)
        {
            shouldBuildTab.AddRange(new List<bool>(Enumerable.Repeat<bool>(false, checked((int)lengthDiff))).Cast<bool>());
        }
        else
        {
            if (lengthDiff < 0L)
            {
                shouldBuildTab.RemoveRange(checked((int)widget.tabCount), checked((int)checked((long)shouldBuildTab.Count)));
            }
        }
        _focusActiveTab();
    }

    internal virtual void _focusActiveTab()
    {
        if (checked(tabFocusNodes.Count) != widget.tabCount)
        {
            if (checked(tabFocusNodes.Count) > widget.tabCount)
            {
                discardedNodes.AddRange(tabFocusNodes.Skip(checked((int)widget.tabCount)).ToList().Cast<global::Doroti.Framework.Widgets.FocusScopeNode>());
                tabFocusNodes.RemoveRange(checked((int)widget.tabCount), checked((int)checked((long)tabFocusNodes.Count)));
            }
            else
            {
                tabFocusNodes.AddRange(new List<global::Doroti.Framework.Widgets.FocusScopeNode>(Enumerable.Select(Enumerable.Range(0, checked((int)(widget.tabCount - checked(tabFocusNodes.Count)))), (index) => new global::Doroti.Framework.Widgets.FocusScopeNode(debugLabel: $"{typeof(CupertinoTabScaffold)} Tab {index + checked((long)tabFocusNodes.Count)}"))).Cast<global::Doroti.Framework.Widgets.FocusScopeNode>());
            }
        }
        FocusScope.of(context).setFirstFocus(tabFocusNodes[(int)widget.currentTabIndex]);
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Widgets.FocusScopeNode focusScopeNode in tabFocusNodes)
        {
            focusScopeNode.dispose();
        }
        foreach (global::Doroti.Framework.Widgets.FocusScopeNode focusScopeNodeLocal in discardedNodes)
        {
            focusScopeNodeLocal.dispose();
        }
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.Stack(fit: StackFit.expand, children: new List<global::Doroti.Framework.Widgets.Widget>(Enumerable.Select(Enumerable.Range(0, checked((int)widget.tabCount)), (index) =>
        {
            var active = index == widget.currentTabIndex;
            shouldBuildTab[index] = active || shouldBuildTab[index];
            return new global::Doroti.Framework.Widgets.HeroMode(enabled: active, child: new global::Doroti.Framework.Widgets.Offstage(offstage: !active, child: new global::Doroti.Framework.Widgets.TickerMode(enabled: active, child: new global::Doroti.Framework.Widgets.FocusScope(node: tabFocusNodes[index], child: new global::Doroti.Framework.Widgets.Builder(builder: (context) =>
            {
                return shouldBuildTab[index] ? widget.tabBuilder(context, index) : SizedBox.CreateShrink();
                throw new InvalidOperationException("Dart closure completed without a value.");
            })))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RestorableCupertinoTabController : global::Doroti.Framework.Widgets.RestorableChangeNotifier<CupertinoTabController>
{
    internal virtual long _initialIndex { get; private set; } = default!;

    public RestorableCupertinoTabController(long initialIndex = 0)
    {
        _initialIndex = initialIndex;
        System.Diagnostics.Debug.Assert(initialIndex >= 0L);
    }

    public override CupertinoTabController createDefaultValue()
    {
        return new CupertinoTabController(initialIndex: _initialIndex);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override CupertinoTabController fromPrimitives(object? data)
    {
        DartRuntimePrimitives.Assert(() => data is not null);
        return new CupertinoTabController(initialIndex: (long)data!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? toPrimitives()
    {
        return value.index;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
