// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/router.dart
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

public class RouteInformation
{
    internal virtual string? _location { get; private set; }
    internal virtual DartUri? _uri { get; private set; }
    public virtual object? state { get; private set; }

    public RouteInformation(string? location = null, DartUri? uri = null, object? state = null)
    {
        this.state = state;
        this._location = location;
        this._uri = uri;
        System.Diagnostics.Debug.Assert((((location is not null)) != ((uri is not null))));
    }

    public virtual string location
    {
        get
        {
            return (this._location ?? Dart_coreLibrary.decodeComponent(new DartUri(path: ((this.uri.path.Length == 0) ? "/" : this.uri.path), queryParameters: (!System.Linq.Enumerable.Any(this.uri.queryParametersAll) ? null : this.uri.queryParametersAll), fragment: ((this.uri.fragment.Length == 0) ? null : this.uri.fragment)).ToString()));
            return default!;
        }
    }
    public virtual DartUri uri
    {
        get
        {
            if ((this._uri is not null))
            {
                return this._uri;
            }
            return DartUri.parse(this._location!);
            return default!;
        }
    }
}

public class RouterConfig<T>
{
    public virtual RouteInformationProvider? routeInformationProvider { get; private set; }
    public virtual RouteInformationParser<T>? routeInformationParser { get; private set; }
    public virtual RouterDelegate<T> routerDelegate { get; private set; } = default!;
    public virtual BackButtonDispatcher? backButtonDispatcher { get; private set; }

    public RouterConfig(RouteInformationProvider? routeInformationProvider = null, RouteInformationParser<T>? routeInformationParser = null, RouterDelegate<T> routerDelegate = default!, BackButtonDispatcher? backButtonDispatcher = null)
    {
        this.routeInformationProvider = routeInformationProvider;
        this.routeInformationParser = routeInformationParser;
        this.routerDelegate = routerDelegate;
        this.backButtonDispatcher = backButtonDispatcher;
        System.Diagnostics.Debug.Assert((((routeInformationProvider is null)) == ((routeInformationParser is null))));
    }

}

public class Router<T> : StatefulWidget
{
    public virtual RouteInformationProvider? routeInformationProvider { get; private set; }
    public virtual RouteInformationParser<T>? routeInformationParser { get; private set; }
    public virtual RouterDelegate<T> routerDelegate { get; private set; } = default!;
    public virtual BackButtonDispatcher? backButtonDispatcher { get; private set; }
    public virtual string? restorationScopeId { get; private set; }

    public Router(global::Doroti.Generated.Framework.Foundation.Key? key = null, RouteInformationProvider? routeInformationProvider = null, RouteInformationParser<T>? routeInformationParser = null, RouterDelegate<T> routerDelegate = default!, BackButtonDispatcher? backButtonDispatcher = null, string? restorationScopeId = null) : base(key: key)
    {
        this.routeInformationProvider = routeInformationProvider;
        this.routeInformationParser = routeInformationParser;
        this.routerDelegate = routerDelegate;
        this.backButtonDispatcher = backButtonDispatcher;
        this.restorationScopeId = restorationScopeId;
        System.Diagnostics.Debug.Assert(((routeInformationProvider is null) || (routeInformationParser is not null)));
    }

    public static Router<T> CreateWithConfig(global::Doroti.Generated.Framework.Foundation.Key? key = null, RouterConfig<T> config = default!, string? restorationScopeId = null)
    {
        return new Router<T>(key: key, routeInformationProvider: ((RouterConfig<T>)config).routeInformationProvider, routeInformationParser: ((RouterConfig<T>)config).routeInformationParser, routerDelegate: ((RouterConfig<T>)config).routerDelegate, backButtonDispatcher: ((RouterConfig<T>)config).backButtonDispatcher, restorationScopeId: restorationScopeId);
    }

    public static Router<T> of<T>(BuildContext context)
    {
        _RouterScope__router? scope__22431 = ((_RouterScope__router?)(object?)context.dependOnInheritedWidgetOfExactType<_RouterScope__router>());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((scope__22431 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("Router operation requested with a context that does not include a Router.\n" + "The context used to retrieve the Router must be that of a widget that " + "is a descendant of a Router widget."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((Router<T>?)(object?)((dynamic)((dynamic)scope__22431!.routerState).widget))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Router<T>? maybeOf<T>(BuildContext context)
    {
        _RouterScope__router? scope__23548 = ((_RouterScope__router?)(object?)context.dependOnInheritedWidgetOfExactType<_RouterScope__router>());
        return ((Router<T>?)(object?)((dynamic)((dynamic)scope__23548?.routerState).widget))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void navigate(BuildContext context, global::System.Action callback)
    {
        var scope__25141 = ((_RouterScope__router?)(object?)context.getElementForInheritedWidgetOfExactType<_RouterScope__router>()!.widget)!;
        ((dynamic)((_RouterScope__router)scope__25141).routerState)._setStateWithExplicitReportStatus(RouteInformationReportingType.navigate, (global::System.Action)(() => callback()));
    }

    public static void neglect(BuildContext context, global::System.Action callback)
    {
        var scope__26516 = ((_RouterScope__router?)(object?)context.getElementForInheritedWidgetOfExactType<_RouterScope__router>()!.widget)!;
        ((dynamic)((_RouterScope__router)scope__26516).routerState)._setStateWithExplicitReportStatus(RouteInformationReportingType.neglect, (global::System.Action)(() => callback()));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RouterState__router<T>());
}

internal delegate Future<Q> _AsyncPassthrough__router<Q>(Q __unused0);

internal delegate Future _RouteSetter__router<T>(T __unused0);

public enum RouteInformationReportingType
{
    none,
    neglect,
    navigate
}

internal class _RouterState__router<T> : State<Router<T>>, RestorationMixin<Router<T>>
{
    internal virtual object? _currentRouterTransaction { get; set; } = default;
    internal virtual RouteInformationReportingType? _currentIntentionToReport { get; set; } = default;
    internal virtual _RestorableRouteInformation__router _routeInformation { get; private set; } = new _RestorableRouteInformation__router();
    internal virtual bool _routeParsePending { get; set; } = default!;
    internal virtual bool _routeInformationReportingTaskScheduled { get; set; } = false;
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public virtual string? restorationId => ((Router<T>)(object)this.widget).restorationScopeId;
    public override void initState()
    {
        base.initState();
        ((Router<T>)(object)this.widget).routeInformationProvider?.addListener(() => this._handleRouteInformationProviderNotification());
        ((Router<T>)(object)this.widget).backButtonDispatcher?.addCallback((global::System.Func<Future<bool>>)this._handleBackButtonDispatcherNotification);
        ((Router<T>)(object)this.widget).routerDelegate.addListener(() => this._handleRouterDelegateNotification());
    }

    public virtual void restoreState(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(this._routeInformation, "route");
        if ((this._routeInformation.value is not null))
        {
            DartRuntimePrimitives.Assert(() => (((Router<T>)(object)this.widget).routeInformationParser is not null));
            _processRouteInformation(this._routeInformation.value!, ((global::System.Func<global::System.Func<T, Future>>)(() => ((Router<T>)(object)this.widget).routerDelegate.setRestoredRoutePath)));
        }
        else
        {
            if ((((Router<T>)(object)this.widget).routeInformationProvider is not null))
            {
                _processRouteInformation(((Router<T>)(object)this.widget).routeInformationProvider!.value, ((global::System.Func<global::System.Func<T, Future>>)(() => ((Router<T>)(object)this.widget).routerDelegate.setInitialRoutePath)));
            }
        }
    }

    internal virtual void _scheduleRouteInformationReportingTask()
    {
        if ((this._routeInformationReportingTaskScheduled || (((Router<T>)(object)this.widget).routeInformationProvider is null)))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (this._currentIntentionToReport is not null));
        _routeInformationReportingTaskScheduled = true;
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback((__arg0) => ((global::System.Action<Duration>)this._reportRouteInformation)(__arg0), debugLabel: "Router.reportRouteInfo");
    }

    internal virtual void _reportRouteInformation(Duration timestamp)
    {
        if (!this.mounted)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => this._routeInformationReportingTaskScheduled);
        _routeInformationReportingTaskScheduled = false;
        if ((this._routeInformation.value is not null))
        {
            RouteInformation currentRouteInformation__29770 = this._routeInformation.value!;
            DartRuntimePrimitives.Assert(() => (this._currentIntentionToReport is not null));
            ((Router<T>)(object)this.widget).routeInformationProvider!.routerReportsNewRouteInformation(currentRouteInformation__29770, type: DartRuntimePrimitives.RequireValue(this._currentIntentionToReport));
        }
        _currentIntentionToReport = RouteInformationReportingType.none;
    }

    internal virtual RouteInformation? _retrieveNewRouteInformation()
    {
        T? configuration__30173 = ((Router<T>)(object)this.widget).routerDelegate.currentConfiguration;
        if ((configuration__30173 is null))
        {
            return ((RouteInformation)(object)null);
        }
        return ((RouteInformation?)(object?)((Router<T>)(object)this.widget).routeInformationParser?.restoreRouteInformation(configuration__30173));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _setStateWithExplicitReportStatus(RouteInformationReportingType status, global::System.Action fn)
    {
        DartRuntimePrimitives.Assert(() => (FoundationRuntimePorts.EnumIndex(status) >= FoundationRuntimePorts.EnumIndex(RouteInformationReportingType.neglect)));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((this._currentIntentionToReport is not null) && (!object.Equals(this._currentIntentionToReport, RouteInformationReportingType.none))) && (!object.Equals(this._currentIntentionToReport, status))))
                {
                    FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: "Both Router.navigate and Router.neglect have been called in this " + "build cycle, and the Router cannot decide whether to report the " + "route information. Please make sure only one of them is called " + "within the same build cycle."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _currentIntentionToReport = status;
        _scheduleRouteInformationReportingTask();
        fn();
    }

    internal virtual void _maybeNeedToReportRouteInformation()
    {
        this._routeInformation.value = _retrieveNewRouteInformation();
        _currentIntentionToReport ??= RouteInformationReportingType.none;
        _scheduleRouteInformationReportingTask();
    }

    public override void didChangeDependencies()
    {
        _routeParsePending = true;
        base.didChangeDependencies();
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__41020 = this._bucket;
        bool needsRestore__41056 = this.restorePending;
        _currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket__41159 = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore__41056);
        if (needsRestore__41056)
        {
            _doRestore(oldBucket__41020);
        }
        if (didReplaceBucket__41159)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket__41020, this._bucket)));
            oldBucket__41020?.dispose();
        }
        RouteInformation? currentRouteInformation__31846 = (this._routeInformation.value ?? ((Router<T>)(object)this.widget).routeInformationProvider?.value);
        if (((currentRouteInformation__31846 is not null) && this._routeParsePending))
        {
            _processRouteInformation(currentRouteInformation__31846, ((global::System.Func<global::System.Func<T, Future>>)(() => ((Router<T>)(object)this.widget).routerDelegate.setNewRoutePath)));
        }
        _routeParsePending = false;
        _maybeNeedToReportRouteInformation();
    }

    public override void didUpdateWidget(Router<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if (((((!object.Equals(((Router<T>)(object)this.widget).routeInformationProvider, ((Router<T>)oldWidget).routeInformationProvider)) || (!object.Equals(((Router<T>)(object)this.widget).backButtonDispatcher, ((Router<T>)oldWidget).backButtonDispatcher))) || (!object.Equals(((Router<T>)(object)this.widget).routeInformationParser, ((Router<T>)oldWidget).routeInformationParser))) || (!object.Equals(((Router<T>)(object)this.widget).routerDelegate, ((Router<T>)oldWidget).routerDelegate))))
        {
            _currentRouterTransaction = new object();
        }
        if ((!object.Equals(((Router<T>)(object)this.widget).routeInformationProvider, ((Router<T>)oldWidget).routeInformationProvider)))
        {
            ((Router<T>)oldWidget).routeInformationProvider?.removeListener(() => this._handleRouteInformationProviderNotification());
            ((Router<T>)(object)this.widget).routeInformationProvider?.addListener(() => this._handleRouteInformationProviderNotification());
            if ((!object.Equals(((Router<T>)oldWidget).routeInformationProvider?.value, ((Router<T>)(object)this.widget).routeInformationProvider?.value)))
            {
                _handleRouteInformationProviderNotification();
            }
        }
        if ((!object.Equals(((Router<T>)(object)this.widget).backButtonDispatcher, ((Router<T>)oldWidget).backButtonDispatcher)))
        {
            ((Router<T>)oldWidget).backButtonDispatcher?.removeCallback((global::System.Func<Future<bool>>)this._handleBackButtonDispatcherNotification);
            ((Router<T>)(object)this.widget).backButtonDispatcher?.addCallback((global::System.Func<Future<bool>>)this._handleBackButtonDispatcherNotification);
        }
        if ((!object.Equals(((Router<T>)(object)this.widget).routerDelegate, ((Router<T>)oldWidget).routerDelegate)))
        {
            ((Router<T>)oldWidget).routerDelegate.removeListener(() => this._handleRouterDelegateNotification());
            ((Router<T>)(object)this.widget).routerDelegate.addListener(() => this._handleRouterDelegateNotification());
            _maybeNeedToReportRouteInformation();
        }
    }

    public override void dispose()
    {
        this._routeInformation.dispose();
        ((Router<T>)(object)this.widget).routeInformationProvider?.removeListener(() => this._handleRouteInformationProviderNotification());
        ((Router<T>)(object)this.widget).backButtonDispatcher?.removeCallback((global::System.Func<Future<bool>>)this._handleBackButtonDispatcherNotification);
        ((Router<T>)(object)this.widget).routerDelegate.removeListener(() => this._handleRouterDelegateNotification());
        _currentRouterTransaction = null;
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) => {
if (!((dynamic)property)._disposed)
{
    property.removeListener((global::System.Action)(() => listener()));
}
})));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual void _processRouteInformation(RouteInformation information, global::System.Func<global::System.Func<T, Future>> delegateRouteSetter)
    {
        DartRuntimePrimitives.Assert(() => this._routeParsePending);
        _routeParsePending = false;
        _currentRouterTransaction = new object();
        DartRuntimePrimitives.Ignore(((Router<T>)(object)this.widget).routeInformationParser!.parseRouteInformationWithDependencies(information, this.context).then(_processParsedRouteInformation(this._currentRouterTransaction, (global::System.Func<global::System.Func<T, Future>>)delegateRouteSetter)));
    }

    internal virtual global::System.Func<object, Future> _processParsedRouteInformation(object? transaction, global::System.Func<global::System.Func<T, Future>> delegateRouteSetter)
    {
        return ((global::System.Func<object, Future>)(async (data) => {
if ((!object.Equals(this._currentRouterTransaction, transaction)))
{
    return;
}
await delegateRouteSetter()(DartRuntimePrimitives.ConvertValue<T>(data));
if ((object.Equals(this._currentRouterTransaction, transaction)))
{
    _rebuild();
}
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleRouteInformationProviderNotification()
    {
        _routeParsePending = true;
        _processRouteInformation(((Router<T>)(object)this.widget).routeInformationProvider!.value, ((global::System.Func<global::System.Func<T, Future>>)(() => ((Router<T>)(object)this.widget).routerDelegate.setNewRoutePath)));
    }

    internal virtual Future<bool> _handleBackButtonDispatcherNotification()
    {
        _currentRouterTransaction = new object();
        return ((Router<T>)(object)this.widget).routerDelegate.popRoute().then<bool>(_handleRoutePopped(this._currentRouterTransaction));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::System.Func<bool, Future<bool>> _handleRoutePopped(object? transaction)
    {
        return ((global::System.Func<bool, Future<bool>>)((data) => {
if ((!object.Equals(transaction, this._currentRouterTransaction)))
{
    return new global::Doroti.Generated.Framework.Foundation.SynchronousFuture<bool>(true);
}
_rebuild();
return new global::Doroti.Generated.Framework.Foundation.SynchronousFuture<bool>(data);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _rebuild()
    {
        setState(((global::System.Action)(() => {
})));
        _maybeNeedToReportRouteInformation();
    }

    internal virtual void _handleRouterDelegateNotification()
    {
        setState(((global::System.Action)(() => {
})));
        _maybeNeedToReportRouteInformation();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new UnmanagedRestorationScope(bucket: this.bucket, child: new _RouterScope__router(routeInformationProvider: ((Router<T>)(object)this.widget).routeInformationProvider, backButtonDispatcher: ((Router<T>)(object)this.widget).backButtonDispatcher, routeInformationParser: ((Router<T>)(object)this.widget).routeInformationParser, routerDelegate: ((Router<T>)(object)this.widget).routerDelegate, routerState: this, child: new Builder(builder: (global::System.Func<BuildContext, Widget>)((Router<T>)(object)this.widget).routerDelegate.build))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue__36723 = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue__36804 = (hasSerializedValue__36723 ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
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
        property.initWithValue((dynamic)initialValue__36804);
        if (((!hasSerializedValue__36723 && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
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
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__39230 = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket__39295 = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket__39295)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket__39230, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket__39230 is null)));
            oldBucket__39230?.dispose();
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
            global::Doroti.Generated.Framework.Services.RestorationBucket? potentialNewParent__40517 = ((global::Doroti.Generated.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent__40517, this._currentParent)) && ((potentialNewParent__40517?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public virtual void _doRestore(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\"."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:") }));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace__42801 = _setNewBucketIfNecessary(newBucket: ((global::Doroti.Generated.Framework.Services.RestorationBucket)(object)null), restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace__42801;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Generated.Framework.Services.RestorationBucket newBucket__43086 = ((global::Doroti.Generated.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplace__43168 = _setNewBucketIfNecessary(newBucket: newBucket__43086, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucket__43086)));
            return didReplace__43168;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__43946 = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket__43946);
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
        global::System.Action listener__44576 = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        property.removeListener((global::System.Action)(() => listener__44576()));
        property._unregister();
    }

}

internal class _RouterScope__router : InheritedWidget
{
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<RouteInformation?>? routeInformationProvider { get; private set; }
    public virtual BackButtonDispatcher? backButtonDispatcher { get; private set; }
    public virtual dynamic routeInformationParser { get; private set; } = default!;
    public virtual dynamic routerDelegate { get; private set; } = default!;
    public virtual dynamic routerState { get; private set; } = default!;

    internal _RouterScope__router(global::Doroti.Generated.Framework.Foundation.ValueListenable<RouteInformation?>? routeInformationProvider, BackButtonDispatcher? backButtonDispatcher, dynamic routeInformationParser, dynamic routerDelegate, dynamic routerState, Widget child) : base(child: child)
    {
        this.routeInformationProvider = routeInformationProvider;
        this.backButtonDispatcher = backButtonDispatcher;
        this.routeInformationParser = routeInformationParser;
        this.routerDelegate = routerDelegate;
        this.routerState = routerState;
        System.Diagnostics.Debug.Assert(((routeInformationProvider is null) || (routeInformationParser is not null)));
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_RouterScope__router)(object)oldWidget;
        return (((((!object.Equals(this.routeInformationProvider, ((_RouterScope__router)__oldWidget).routeInformationProvider)) || (!object.Equals(this.backButtonDispatcher, ((_RouterScope__router)__oldWidget).backButtonDispatcher))) || (!object.Equals(this.routeInformationParser, ((_RouterScope__router)__oldWidget).routeInformationParser))) || (!object.Equals(this.routerDelegate, ((_RouterScope__router)__oldWidget).routerDelegate))) || (!object.Equals(this.routerState, ((_RouterScope__router)__oldWidget).routerState)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _CallbackHookProvider__router<T>
{
    internal virtual global::Doroti.Generated.Framework.Foundation.ObserverList<global::System.Func<T>> _callbacks { get; private set; } = new global::Doroti.Generated.Framework.Foundation.ObserverList<global::System.Func<T>>();

    public virtual bool hasCallbacks => System.Linq.Enumerable.Any(this._callbacks);
    public virtual void addCallback(global::System.Func<T> callback) => this._callbacks.add((global::System.Func<T>)callback);
    public virtual void removeCallback(global::System.Func<T> callback) => this._callbacks.remove((global::System.Func<T>)callback);
    public virtual T invokeCallback(T defaultValue)
    {
        if (!System.Linq.Enumerable.Any(this._callbacks))
        {
            return defaultValue;
        }
        try
        {
            return this._callbacks.Single()();
        }
        catch (Exception exception__39698)
        {
            var stack__39709 = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: exception__39698, stack: stack__39709, library: "widget library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"while invoking the callback for {this.GetType()}"), informationCollector: ((InformationCollector)(() => new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<_CallbackHookProvider__router<T>>($"The {this.GetType()} that invoked the callback was", this, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }))));
            return defaultValue;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class BackButtonDispatcher : _CallbackHookProvider__router<Future<bool>>
{
    private bool __late__children_initialized;
    private HashSet<ChildBackButtonDispatcher> __late__children = default!;
    internal virtual HashSet<ChildBackButtonDispatcher> _children
    {
        get
        {
            if (!__late__children_initialized)
            {
                __late__children = ((HashSet<ChildBackButtonDispatcher>?)(object?)new HashSet<ChildBackButtonDispatcher>())!;
                __late__children_initialized = true;
            }
            return __late__children;
        }
    }

    public override bool hasCallbacks => DartRuntimePrimitives.ConvertValue<bool>((base.hasCallbacks || System.Linq.Enumerable.Any(this._children)));
    public override Future<bool> invokeCallback(Future<bool> defaultValue)
    {
        if (System.Linq.Enumerable.Any(this._children))
        {
            List<ChildBackButtonDispatcher> children__42640 = this._children.ToList().ToList();
            long childIndex__42681 = (checked((long)(children__42640.Count)) - 1L);
            Future<bool> notifyNextChild(bool result)
            {
                if (result)
                {
                    return ((Future<bool>)(object?)new global::Doroti.Generated.Framework.Foundation.SynchronousFuture<bool>(result));
                }
                if ((childIndex__42681 > 0L))
                {
                    childIndex__42681 -= 1L;
                    return children__42640[(int)(childIndex__42681)].notifiedByParent(defaultValue).then<bool>(notifyNextChild);
                }
                return ((Future<bool>)(object?)base.invokeCallback(defaultValue));
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            return children__42640[(int)(childIndex__42681)].notifiedByParent(defaultValue).then<bool>(notifyNextChild);
        }
        return ((Future<bool>)(object?)base.invokeCallback(defaultValue));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ChildBackButtonDispatcher createChildBackButtonDispatcher()
    {
        return new ChildBackButtonDispatcher(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void takePriority() => ((dynamic)this._children).clear();
    public virtual void deferTo(ChildBackButtonDispatcher child)
    {
        DartRuntimePrimitives.Assert(() => this.hasCallbacks);
        this._children.remove(child);
        this._children.add(child);
    }

    public virtual void forget(ChildBackButtonDispatcher child) => this._children.remove(child);
}

public class RootBackButtonDispatcher : BackButtonDispatcher, WidgetsBindingObserver
{

    public RootBackButtonDispatcher()
    {
    }

    public override void addCallback(global::System.Func<Future<bool>> callback)
    {
        if (!this.hasCallbacks)
        {
            WidgetsBinding.instance.addObserver(this);
        }
        base.addCallback((global::System.Func<Future<bool>>)callback);
    }

    public override void removeCallback(global::System.Func<Future<bool>> callback)
    {
        base.removeCallback((global::System.Func<Future<bool>>)callback);
        if (!this.hasCallbacks)
        {
            WidgetsBinding.instance.removeObserver(this);
        }
    }

    public virtual Future<bool> didPopRoute() => invokeCallback(Future<bool>.value(false));
}

public class ChildBackButtonDispatcher : BackButtonDispatcher
{
    public virtual BackButtonDispatcher parent { get; private set; } = default!;

    public ChildBackButtonDispatcher(BackButtonDispatcher parent)
    {
        this.parent = parent;
    }

    public virtual Future<bool> notifiedByParent(Future<bool> defaultValue)
    {
        return ((Future<bool>)(object?)invokeCallback(defaultValue));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void takePriority()
    {
        this.parent.deferTo(this);
        base.takePriority();
    }

    public override void deferTo(ChildBackButtonDispatcher child)
    {
        DartRuntimePrimitives.Assert(() => this.hasCallbacks);
        this.parent.deferTo(this);
        base.deferTo(child);
    }

    public override void removeCallback(global::System.Func<Future<bool>> callback)
    {
        base.removeCallback((global::System.Func<Future<bool>>)callback);
        if (!this.hasCallbacks)
        {
            this.parent.forget(this);
        }
    }

}

public class BackButtonListener : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual global::System.Func<Future<bool>> onBackButtonPressed { get; private set; } = default!;

    public BackButtonListener(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, global::System.Func<Future<bool>> onBackButtonPressed = default!) : base(key: key)
    {
        this.child = child;
        this.onBackButtonPressed = onBackButtonPressed;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _BackButtonListenerState__router());
}

internal class _BackButtonListenerState__router : State<BackButtonListener>
{
    public virtual BackButtonDispatcher? dispatcher { get; set; } = default;

    public override void didChangeDependencies()
    {
        this.dispatcher?.removeCallback((global::System.Func<Future<bool>>)((BackButtonListener)(object)this.widget).onBackButtonPressed);
        BackButtonDispatcher? rootBackDispatcher__50813 = ((BackButtonDispatcher?)((dynamic)Router<object>.of<object>(this.context)).backButtonDispatcher);
        DartRuntimePrimitives.Assert(() => (rootBackDispatcher__50813 is not null), () => (object?)"The parent router must have a backButtonDispatcher to use this widget");
        dispatcher = DartRuntimePrimitives.ConvertValue<BackButtonDispatcher>(((Func<ChildBackButtonDispatcher>)(() =>
{            var __cascade = rootBackDispatcher__50813!.createChildBackButtonDispatcher();
            __cascade.addCallback((global::System.Func<Future<bool>>)((BackButtonListener)(object)this.widget).onBackButtonPressed);
            __cascade.takePriority();
            return __cascade;        }))());
        base.didChangeDependencies();
    }

    public override void didUpdateWidget(BackButtonListener oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals((global::System.Func<Future<bool>>)((BackButtonListener)oldWidget).onBackButtonPressed, (global::System.Func<Future<bool>>)((BackButtonListener)(object)this.widget).onBackButtonPressed)))
        {
            this.dispatcher?.removeCallback((global::System.Func<Future<bool>>)((BackButtonListener)oldWidget).onBackButtonPressed);
            this.dispatcher?.addCallback((global::System.Func<Future<bool>>)((BackButtonListener)(object)this.widget).onBackButtonPressed);
            this.dispatcher?.takePriority();
        }
    }

    public override void dispose()
    {
        this.dispatcher?.removeCallback((global::System.Func<Future<bool>>)((BackButtonListener)(object)this.widget).onBackButtonPressed);
        base.dispose();
    }

    public override Widget build(BuildContext context) => ((BackButtonListener)(object)this.widget).child;
}

public abstract class RouteInformationParser<T>
{
    protected RouteInformationParser()
    {
    }

    public virtual Future<T> parseRouteInformation(RouteInformation routeInformation)
    {
        throw new NotImplementedException("One of the parseRouteInformation or " + "parseRouteInformationWithDependencies must be implemented");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T> parseRouteInformationWithDependencies(RouteInformation routeInformation, BuildContext context)
    {
        return ((Future<T>)(object?)parseRouteInformation(routeInformation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RouteInformation? restoreRouteInformation(T configuration) => DartRuntimePrimitives.ConvertValue<RouteInformation>(null);
}

public abstract class RouterDelegate<T> : global::Doroti.Generated.Framework.Foundation.Listenable
{
    public virtual void addListener(global::System.Action listener) => throw new NotSupportedException();
    public virtual void removeListener(global::System.Action listener) => throw new NotSupportedException();
    public virtual Future setInitialRoutePath(T configuration)
    {
        return ((Future)(object?)setNewRoutePath(configuration));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future setRestoredRoutePath(T configuration)
    {
        return ((Future)(object?)setNewRoutePath(configuration));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Future setNewRoutePath(T configuration);
    public abstract Future<bool> popRoute();
    public virtual T? currentConfiguration => DartRuntimePrimitives.ConvertValue<T>(null);
    public abstract Widget build(BuildContext context);
}

public abstract class RouteInformationProvider : global::Doroti.Generated.Framework.Foundation.ValueListenable<RouteInformation>
{
    public virtual RouteInformation value => throw new NotSupportedException();
    public virtual void routerReportsNewRouteInformation(RouteInformation routeInformation, RouteInformationReportingType type = RouteInformationReportingType.none)
    {
    }

    private readonly HashSet<global::System.Action> __listeners = new();
    public virtual bool hasListeners => __listeners.Count != 0;
    public virtual void addListener(global::System.Action listener) => __listeners.Add(listener);
    public virtual void removeListener(global::System.Action listener) => __listeners.Remove(listener);
    public virtual void notifyListeners() { foreach (var listener in __listeners.ToArray()) listener(); }
    public virtual void dispose() => __listeners.Clear();
}

public class PlatformRouteInformationProvider : RouteInformationProvider, WidgetsBindingObserver
{
    internal virtual RouteInformation _value { get; set; } = default!;
    internal virtual RouteInformation _valueInEngine { get; set; } = new RouteInformation(uri: DartUri.parse(WidgetsBinding.instance.platformDispatcher.defaultRouteName));

    public PlatformRouteInformationProvider(RouteInformation initialRouteInformation)
    {
        this._value = initialRouteInformation;
    }

    internal static bool _equals(DartUri a, DartUri b)
    {
        return (((a.path == b.path) && (a.fragment == b.fragment)) && new DeepCollectionEquality().equals(a.queryParametersAll, b.queryParametersAll));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void routerReportsNewRouteInformation(RouteInformation routeInformation, RouteInformationReportingType type = RouteInformationReportingType.none)
    {
        DartRuntimePrimitives.Ignore(SystemNavigator.selectMultiEntryHistory());
        DartRuntimePrimitives.Ignore(SystemNavigator.routeInformationUpdated(uri: ((RouteInformation)routeInformation).uri, state: ((RouteInformation)routeInformation).state, replace: (type switch { RouteInformationReportingType.neglect => true, RouteInformationReportingType.navigate => false, RouteInformationReportingType.none => PlatformRouteInformationProvider._equals(((RouteInformation)this._valueInEngine).uri, ((RouteInformation)routeInformation).uri), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
        _value = routeInformation;
        _valueInEngine = routeInformation;
    }

    public virtual RouteInformation value => this._value;
    internal virtual void _platformReportsNewRouteInformation(RouteInformation routeInformation)
    {
        if ((object.Equals(this._value, routeInformation)))
        {
            return;
        }
        _value = routeInformation;
        _valueInEngine = routeInformation;
        notifyListeners();
    }

    public virtual void addListener(global::System.Action listener)
    {
        if (!this.hasListeners)
        {
            WidgetsBinding.instance.addObserver(this);
        }
        base.addListener(() => listener());
    }

    public virtual void removeListener(global::System.Action listener)
    {
        base.removeListener(() => listener());
        if (!this.hasListeners)
        {
            WidgetsBinding.instance.removeObserver(this);
        }
    }

    public virtual void dispose()
    {
        if (this.hasListeners)
        {
            WidgetsBinding.instance.removeObserver(this);
        }
        base.dispose();
    }

    public async virtual Future<bool> didPushRouteInformation(RouteInformation routeInformation)
    {
        DartRuntimePrimitives.Assert(() => this.hasListeners);
        _platformReportsNewRouteInformation(routeInformation);
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class PopNavigatorRouterDelegateMixin<T> : RouterDelegate<T>
{
    public abstract void addListener(global::System.Action listener);
    public abstract void removeListener(global::System.Action listener);
    public abstract GlobalKey<NavigatorState>? navigatorKey { get; }
    public override Future<bool> popRoute()
    {
        NavigatorState? navigator__68731 = this.navigatorKey?.currentState;
        return (navigator__68731?.maybePop<object>() ?? new global::Doroti.Generated.Framework.Foundation.SynchronousFuture<bool>(false));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RestorableRouteInformation__router : RestorableValue<RouteInformation?>
{
    public override RouteInformation? createDefaultValue() => DartRuntimePrimitives.ConvertValue<RouteInformation>(null);
    public override void didUpdateValue(RouteInformation? oldValue)
    {
        notifyListeners();
    }

    public override RouteInformation? fromPrimitives(object? data)
    {
        if ((data is null))
        {
            return ((RouteInformation)(object)null);
        }
        DartRuntimePrimitives.Assert(() => ((data is List<object?>) && (checked((long)(((List<object>)data).Count)) == 2L)));
        var castedData__69257 = ((List<object?>?)(object?)data)!;
        var uri__69303 = ((string?)(object?)castedData__69257.First())!;
        if ((uri__69303 is null))
        {
            return ((RouteInformation)(object)null);
        }
        return new RouteInformation(uri: DartUri.parse(uri__69303), state: castedData__69257.Last());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? toPrimitives()
    {
        return ((this.value is null) ? null : new List<object?> { this.value!.uri.ToString(), this.value!.state });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
