// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/tab_scaffold.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public class CupertinoTabController : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual bool _isDisposed { get; set; } = false;
    internal virtual long _index { get; set; } = default!;

    public CupertinoTabController(long initialIndex = 0)
    {
        this._index = initialIndex;
        System.Diagnostics.Debug.Assert((initialIndex >= 0L));
    }

    public virtual long index
    {
        get => this._index;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0L));
            if ((this._index == __value))
            {
                return;
            }
            _index = __value;
            notifyListeners();
        }
    }
    public virtual void dispose()
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
        System.Diagnostics.Debug.Assert(((controller is null) || (((CupertinoTabController)controller).index < checked((long)(((CupertinoTabBar)tabBar).items.Count)))));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTabScaffoldState__tab_scaffold());
}

internal class _CupertinoTabScaffoldState__tab_scaffold : global::Doroti.Framework.Widgets.State<CupertinoTabScaffold>, global::Doroti.Framework.Widgets.RestorationMixin<CupertinoTabScaffold>
{
    internal virtual RestorableCupertinoTabController? _internalController { get; set; } = default;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    internal virtual CupertinoTabController _controller => DartRuntimePrimitives.ConvertValue<CupertinoTabController>((((CupertinoTabScaffold)this.widget).controller ?? this._internalController!.value));
    public virtual string? restorationId => ((CupertinoTabScaffold)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        _restoreInternalController();
    }

    internal virtual void _restoreInternalController()
    {
        if ((this._internalController is not null))
        {
            registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._internalController!), "controller");
            this._internalController!.value.addListener(() => this._onCurrentIndexChange());
        }
    }

    public override void initState()
    {
        base.initState();
        _updateTabController();
    }

    internal virtual void _updateTabController(CupertinoTabController? oldWidgetController = null)
    {
        if (((((CupertinoTabScaffold)this.widget).controller is null) && (this._internalController is null)))
        {
            _internalController = new RestorableCupertinoTabController(initialIndex: ((CupertinoTabScaffold)this.widget).tabBar.currentIndex);
            if (!this.restorePending)
            {
                _restoreInternalController();
            }
        }
        if (((((CupertinoTabScaffold)this.widget).controller is not null) && (this._internalController is not null)))
        {
            unregisterFromRestoration(this._internalController!);
            this._internalController!.dispose();
            _internalController = null;
        }
        if ((!object.Equals(oldWidgetController, ((CupertinoTabScaffold)this.widget).controller)))
        {
            if ((oldWidgetController?._isDisposed == false))
            {
                oldWidgetController!.removeListener(() => this._onCurrentIndexChange());
            }
            ((CupertinoTabScaffold)this.widget).controller?.addListener(() => this._onCurrentIndexChange());
        }
    }

    internal virtual void _onCurrentIndexChange()
    {
        DartRuntimePrimitives.Assert(() => ((((CupertinoTabController)this._controller).index >= 0L) && (((CupertinoTabController)this._controller).index < checked((long)(((CupertinoTabScaffold)this.widget).tabBar.items.Count)))), () => (object?)$"The {this.GetType()}'s current index {((CupertinoTabController)this._controller).index} is " + $"out of bounds for the tab bar with {checked((long)(((CupertinoTabScaffold)this.widget).tabBar.items.Count))} tabs");
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public override void didUpdateWidget(CupertinoTabScaffold oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if ((!object.Equals(((CupertinoTabScaffold)this.widget).controller, ((CupertinoTabScaffold)oldWidget).controller)))
        {
            _updateTabController(((CupertinoTabScaffold)oldWidget).controller);
        }
        else
        {
            if ((((CupertinoTabController)this._controller).index >= checked((long)(((CupertinoTabScaffold)this.widget).tabBar.items.Count))))
            {
                this._controller.index = (checked((long)(((CupertinoTabScaffold)this.widget).tabBar.items.Count)) - 1L);
            }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.MediaQueryData existingMediaQuery = ((global::Doroti.Framework.Widgets.MediaQueryData)(object?)MediaQuery.of(context));
        global::Doroti.Framework.Widgets.MediaQueryData newMediaQuery = ((global::Doroti.Framework.Widgets.MediaQueryData)(object?)MediaQuery.of(context));
        global::Doroti.Framework.Widgets.Widget content = ((global::Doroti.Framework.Widgets.Widget)(object?)new _TabSwitchingView__tab_scaffold(currentTabIndex: ((CupertinoTabController)this._controller).index, tabCount: checked((long)(((CupertinoTabScaffold)this.widget).tabBar.items.Count)), tabBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget>)((CupertinoTabScaffold)this.widget).tabBuilder));
        global::Doroti.Framework.Painting.EdgeInsets contentPadding = global::Doroti.Framework.Painting.EdgeInsets.zero;
        if (((CupertinoTabScaffold)this.widget).resizeToAvoidBottomInset)
        {
            newMediaQuery = newMediaQuery.removeViewInsets(removeBottom: true);
            contentPadding = global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: ((global::Doroti.Framework.Widgets.MediaQueryData)existingMediaQuery).viewInsets.bottom);
        }
        if ((!((CupertinoTabScaffold)this.widget).resizeToAvoidBottomInset || (((CupertinoTabScaffold)this.widget).tabBar.preferredSize.height > ((global::Doroti.Framework.Widgets.MediaQueryData)existingMediaQuery).viewInsets.bottom)))
        {
            double bottomPadding = (((CupertinoTabScaffold)this.widget).tabBar.preferredSize.height + ((global::Doroti.Framework.Widgets.MediaQueryData)existingMediaQuery).padding.bottom);
            if (((CupertinoTabScaffold)this.widget).tabBar.opaque(context))
            {
                contentPadding = global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: bottomPadding);
                newMediaQuery = newMediaQuery.removePadding(removeBottom: true);
            }
            else
            {
                newMediaQuery = newMediaQuery.copyWith(padding: ((global::Doroti.Framework.Widgets.MediaQueryData)newMediaQuery).padding.copyWith(bottom: bottomPadding));
            }
        }
        content = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MediaQuery(data: newMediaQuery, child: new global::Doroti.Framework.Widgets.Padding(padding: contentPadding, child: content)));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: (CupertinoDynamicColor.maybeResolve(((CupertinoTabScaffold)this.widget).backgroundColor, context) ?? CupertinoTheme.of(context).scaffoldBackgroundColor)), child: new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(content), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(MediaQuery.withNoTextScaling(child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.bottomCenter, child: ((CupertinoTabScaffold)this.widget).tabBar.copyWith(currentIndex: ((CupertinoTabController)this._controller).index, onTap: ((global::System.Action<long>)((newIndex) => {
this._controller.index = newIndex;
((CupertinoTabScaffold)this.widget).tabBar.onTap?.Invoke(newIndex);
})))))) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        if ((((CupertinoTabScaffold)this.widget).controller?._isDisposed == false))
        {
            this._controller.removeListener(() => this._onCurrentIndexChange());
        }
        this._internalController?.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) =>
        {
            if (!((dynamic)property)._disposed)
            {
                property.removeListener((global::System.Action)(() => listener()));
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue = (hasSerializedValue ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
        if (!((dynamic)property).isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if ((this.bucket is null))
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener((global::System.Action)(() => listener()));
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue);
        if (((!hasSerializedValue && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(dynamic property)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)property)._owner, this)));
        this._bucket?.remove<object?>(((dynamic)property)._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket is null)));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (this._firstRestorePending)
            {
                return true;
            }
            if ((this.restorationId is null))
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = ((global::Doroti.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent, this._currentParent)) && ((potentialNewParent?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        this._currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            oldBucket?.dispose();
        }
    }

    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = ((global::Doroti.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucketLocal)));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(dynamic property)
    {
        if (((dynamic)property).enabled)
        {
            this._bucket?.write(((dynamic)property)._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(((dynamic)property)._restorationId!);
        }
    }

    public virtual void _unregister(dynamic property)
    {
        global::System.Action listener = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener((global::System.Action)(() => listener()));
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
        System.Diagnostics.Debug.Assert((tabCount > 0L));
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
        this.shouldBuildTab.AddRange(new List<bool>(System.Linq.Enumerable.Repeat<bool>(false, checked((int)((_TabSwitchingView__tab_scaffold)this.widget).tabCount))).Cast<bool>());
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _focusActiveTab();
    }

    public override void didUpdateWidget(_TabSwitchingView__tab_scaffold oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        long lengthDiff = (((_TabSwitchingView__tab_scaffold)this.widget).tabCount - checked((long)(this.shouldBuildTab.Count)));
        if ((lengthDiff > 0L))
        {
            this.shouldBuildTab.AddRange(new List<bool>(System.Linq.Enumerable.Repeat<bool>(false, checked((int)lengthDiff))).Cast<bool>());
        }
        else
        {
            if ((lengthDiff < 0L))
            {
                this.shouldBuildTab.RemoveRange(checked((int)((_TabSwitchingView__tab_scaffold)this.widget).tabCount), checked((int)checked((long)(this.shouldBuildTab.Count))));
            }
        }
        _focusActiveTab();
    }

    internal virtual void _focusActiveTab()
    {
        if ((checked((long)(this.tabFocusNodes.Count)) != ((_TabSwitchingView__tab_scaffold)this.widget).tabCount))
        {
            if ((checked((long)(this.tabFocusNodes.Count)) > ((_TabSwitchingView__tab_scaffold)this.widget).tabCount))
            {
                this.discardedNodes.AddRange(this.tabFocusNodes.Skip(checked((int)((_TabSwitchingView__tab_scaffold)this.widget).tabCount)).ToList().Cast<global::Doroti.Framework.Widgets.FocusScopeNode>());
                this.tabFocusNodes.RemoveRange(checked((int)((_TabSwitchingView__tab_scaffold)this.widget).tabCount), checked((int)checked((long)(this.tabFocusNodes.Count))));
            }
            else
            {
                this.tabFocusNodes.AddRange(new List<global::Doroti.Framework.Widgets.FocusScopeNode>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)(((_TabSwitchingView__tab_scaffold)this.widget).tabCount - checked((long)(this.tabFocusNodes.Count))))), ((index) => new global::Doroti.Framework.Widgets.FocusScopeNode(debugLabel: $"{typeof(CupertinoTabScaffold)} Tab {(index + checked((long)(this.tabFocusNodes.Count)))}")))).Cast<global::Doroti.Framework.Widgets.FocusScopeNode>());
            }
        }
        FocusScope.of(this.context).setFirstFocus(this.tabFocusNodes[(int)(((_TabSwitchingView__tab_scaffold)this.widget).currentTabIndex)]);
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Widgets.FocusScopeNode focusScopeNode in this.tabFocusNodes)
        {
            focusScopeNode.dispose();
        }
        foreach (global::Doroti.Framework.Widgets.FocusScopeNode focusScopeNodeLocal in this.discardedNodes)
        {
            focusScopeNodeLocal.dispose();
        }
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(fit: global::Doroti.Framework.Rendering.StackFit.expand, children: new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)((_TabSwitchingView__tab_scaffold)this.widget).tabCount)), ((index) =>
        {
            var active = (index == ((_TabSwitchingView__tab_scaffold)this.widget).currentTabIndex);
            this.shouldBuildTab[(int)(index)] = (active || this.shouldBuildTab[(int)(index)]);
            return new global::Doroti.Framework.Widgets.HeroMode(enabled: active, child: new global::Doroti.Framework.Widgets.Offstage(offstage: !active, child: new global::Doroti.Framework.Widgets.TickerMode(enabled: active, child: new global::Doroti.Framework.Widgets.FocusScope(node: this.tabFocusNodes[(int)(index)], child: new global::Doroti.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
            {
                return (this.shouldBuildTab[(int)(index)] ? this.widget.tabBuilder(context, index) : global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
                throw new InvalidOperationException("Dart closure completed without a value.");
            })))))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RestorableCupertinoTabController : global::Doroti.Framework.Widgets.RestorableChangeNotifier<CupertinoTabController>
{
    internal virtual long _initialIndex { get; private set; } = default!;

    public RestorableCupertinoTabController(long initialIndex = 0)
    {
        this._initialIndex = initialIndex;
        System.Diagnostics.Debug.Assert((initialIndex >= 0L));
    }

    public override CupertinoTabController createDefaultValue()
    {
        return new CupertinoTabController(initialIndex: this._initialIndex);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override CupertinoTabController fromPrimitives(object? data)
    {
        DartRuntimePrimitives.Assert(() => (data is not null));
        return new CupertinoTabController(initialIndex: ((long)data!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? toPrimitives()
    {
        return ((CupertinoTabController)this.value).index;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
