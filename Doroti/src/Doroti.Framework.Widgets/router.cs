// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/router.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class RouteInformation
{
    internal virtual string? _location { get; private set; }
    internal virtual DartUri? _uri { get; private set; }
    public virtual object? state { get; private set; }

    public RouteInformation(string? location = null, DartUri? uri = null, object? state = null)
    {
        this.state = state;
        _location = location;
        _uri = uri;
        System.Diagnostics.Debug.Assert((location is not null) != (uri is not null));
    }

    public virtual string location
    {
        get
        {
            return _location
                ?? Dart_coreLibrary.decodeComponent(
                    new DartUri(
                        path: (uri.path.Length == 0) ? "/" : uri.path,
                        queryParameters: !Enumerable.Any(uri.queryParametersAll)
                            ? null
                            : uri.queryParametersAll,
                        fragment: (uri.fragment.Length == 0) ? null : uri.fragment
                    ).ToString()
                );
        }
    }
    public virtual DartUri uri
    {
        get
        {
            if (_uri is not null)
            {
                return _uri;
            }
            return DartUri.parse(_location!);
        }
    }
}

public interface IRouterConfig
{
    Widget createRouterWidget(Key? key = null, string? restorationScopeId = null);
}

public interface IRouterDelegate : Listenable
{
    Widget createRouterWidget(
        RouteInformationProvider? provider,
        object? parser,
        BackButtonDispatcher? dispatcher,
        string? restorationScopeId
    );
}

public interface IRouter
{
    BackButtonDispatcher? backButtonDispatcher { get; }
}

internal interface IRouterState : IState
{
    void setReportingStatus(RouteInformationReportingType status, Action action);
}

public class RouterConfig<T> : IRouterConfig
{
    public virtual RouteInformationProvider? routeInformationProvider { get; private set; }
    public virtual RouteInformationParser<T>? routeInformationParser { get; private set; }
    public virtual RouterDelegate<T> routerDelegate { get; private set; } = default!;
    public virtual BackButtonDispatcher? backButtonDispatcher { get; private set; }

    public Widget createRouterWidget(Key? key = null, string? restorationScopeId = null) =>
        Router<T>.CreateWithConfig(key: key, config: this, restorationScopeId: restorationScopeId);

    public RouterConfig(
        RouteInformationProvider? routeInformationProvider = null,
        RouteInformationParser<T>? routeInformationParser = null,
        RouterDelegate<T> routerDelegate = default!,
        BackButtonDispatcher? backButtonDispatcher = null
    )
    {
        this.routeInformationProvider = routeInformationProvider;
        this.routeInformationParser = routeInformationParser;
        this.routerDelegate = routerDelegate;
        this.backButtonDispatcher = backButtonDispatcher;
        System.Diagnostics.Debug.Assert(
            (routeInformationProvider is null) == (routeInformationParser is null)
        );
    }
}

public class Router<T> : StatefulWidget, IRouter
{
    public virtual RouteInformationProvider? routeInformationProvider { get; private set; }
    public virtual RouteInformationParser<T>? routeInformationParser { get; private set; }
    public virtual RouterDelegate<T> routerDelegate { get; private set; } = default!;
    public virtual BackButtonDispatcher? backButtonDispatcher { get; private set; }
    public virtual string? restorationScopeId { get; private set; }

    public Router(
        Key? key = null,
        RouteInformationProvider? routeInformationProvider = null,
        RouteInformationParser<T>? routeInformationParser = null,
        RouterDelegate<T> routerDelegate = default!,
        BackButtonDispatcher? backButtonDispatcher = null,
        string? restorationScopeId = null
    )
        : base(key: key)
    {
        this.routeInformationProvider = routeInformationProvider;
        this.routeInformationParser = routeInformationParser;
        this.routerDelegate = routerDelegate;
        this.backButtonDispatcher = backButtonDispatcher;
        this.restorationScopeId = restorationScopeId;
        System.Diagnostics.Debug.Assert(
            (routeInformationProvider is null) || (routeInformationParser is not null)
        );
    }

    public static Router<T> CreateWithConfig(
        Key? key = null,
        RouterConfig<T> config = default!,
        string? restorationScopeId = null
    )
    {
        return new Router<T>(
            key: key,
            routeInformationProvider: config.routeInformationProvider,
            routeInformationParser: config.routeInformationParser,
            routerDelegate: config.routerDelegate,
            backButtonDispatcher: config.backButtonDispatcher,
            restorationScopeId: restorationScopeId
        );
    }

    public static IRouter? untypedOf(BuildContext context) =>
        context.dependOnInheritedWidgetOfExactType<_RouterScope__router>()?.routerState.widget
        as IRouter;

    public static Router<TConfiguration> of<TConfiguration>(BuildContext context)
    {
        _RouterScope__router? scope =
            context.dependOnInheritedWidgetOfExactType<_RouterScope__router>();
        DartRuntimePrimitives.Assert(() =>
        {
            if (scope is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "Router operation requested with a context that does not include a Router.\n"
                            + "The context used to retrieve the Router must be that of a widget that "
                            + "is a descendant of a Router widget."
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return ((Router<TConfiguration>?)scope!.routerState.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Router<TConfiguration>? maybeOf<TConfiguration>(BuildContext context)
    {
        _RouterScope__router? scope =
            context.dependOnInheritedWidgetOfExactType<_RouterScope__router>();
        return ((Router<TConfiguration>?)scope?.routerState.widget)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void navigate(BuildContext context, Action callback)
    {
        var scope = (
            (_RouterScope__router?)
                context.getElementForInheritedWidgetOfExactType<_RouterScope__router>()!.widget
        )!;
        scope.routerState.setReportingStatus(
            RouteInformationReportingType.navigate,
            () => callback()
        );
    }

    public static void neglect(BuildContext context, Action callback)
    {
        var scope = (
            (_RouterScope__router?)
                context.getElementForInheritedWidgetOfExactType<_RouterScope__router>()!.widget
        )!;
        scope.routerState.setReportingStatus(
            RouteInformationReportingType.neglect,
            () => callback()
        );
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _RouterState__router<T>());
}

internal delegate Future<Q> _AsyncPassthrough__router<Q>(Q __unused0);

internal delegate Future _RouteSetter__router<T>(T __unused0);

public enum RouteInformationReportingType
{
    none,
    neglect,
    navigate,
}

internal class _RouterState__router<T> : State<Router<T>>, RestorationMixin<Router<T>>, IRouterState
{
    internal virtual object? _currentRouterTransaction { get; set; } = default;
    internal virtual RouteInformationReportingType? _currentIntentionToReport { get; set; } =
        default;
    internal virtual _RestorableRouteInformation__router _routeInformation { get; private set; } =
        new _RestorableRouteInformation__router();
    internal virtual bool _routeParsePending { get; set; } = default!;
    internal virtual bool _routeInformationReportingTaskScheduled { get; set; } = false;
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } =
        new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } =
        default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

    public virtual string? restorationId => widget.restorationScopeId;

    public override void initState()
    {
        base.initState();
        widget.routeInformationProvider?.addListener(_handleRouteInformationProviderNotification);
        widget.backButtonDispatcher?.addCallback(_handleBackButtonDispatcherNotification);
        widget.routerDelegate.addListener(_handleRouterDelegateNotification);
    }

    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(_routeInformation, "route");
        if (_routeInformation.value is not null)
        {
            DartRuntimePrimitives.Assert(() => widget.routeInformationParser is not null);
            _processRouteInformation(
                _routeInformation.value!,
                () => widget.routerDelegate.setRestoredRoutePath
            );
        }
        else
        {
            if (widget.routeInformationProvider is not null)
            {
                _processRouteInformation(
                    widget.routeInformationProvider!.value,
                    () => widget.routerDelegate.setInitialRoutePath
                );
            }
        }
    }

    internal virtual void _scheduleRouteInformationReportingTask()
    {
        if (_routeInformationReportingTaskScheduled || (widget.routeInformationProvider is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => _currentIntentionToReport is not null);
        _routeInformationReportingTaskScheduled = true;
        Scheduler.SchedulerBinding.instance.addPostFrameCallback(
            (__arg0) => ((Action<Duration>)_reportRouteInformation)(__arg0),
            debugLabel: "Router.reportRouteInfo"
        );
    }

    internal virtual void _reportRouteInformation(Duration timestamp)
    {
        if (!mounted)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => _routeInformationReportingTaskScheduled);
        _routeInformationReportingTaskScheduled = false;
        if (_routeInformation.value is not null)
        {
            RouteInformation currentRouteInformation = _routeInformation.value!;
            DartRuntimePrimitives.Assert(() => _currentIntentionToReport is not null);
            widget.routeInformationProvider!.routerReportsNewRouteInformation(
                currentRouteInformation,
                type: (
                    _currentIntentionToReport
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            );
        }
        _currentIntentionToReport = RouteInformationReportingType.none;
    }

    internal virtual RouteInformation? _retrieveNewRouteInformation()
    {
        T? configuration = widget.routerDelegate.currentConfiguration;
        if (configuration is null)
        {
            return null;
        }
        return widget.routeInformationParser?.restoreRouteInformation(configuration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    void IRouterState.setReportingStatus(RouteInformationReportingType status, Action action) =>
        _setStateWithExplicitReportStatus(status, action);

    internal virtual void _setStateWithExplicitReportStatus(
        RouteInformationReportingType status,
        Action fn
    )
    {
        DartRuntimePrimitives.Assert(() =>
            FoundationRuntimePorts.EnumIndex(status)
            >= FoundationRuntimePorts.EnumIndex(RouteInformationReportingType.neglect)
        );
        DartRuntimePrimitives.Assert(() =>
        {
            if (
                (_currentIntentionToReport is not null)
                && (!Equals(_currentIntentionToReport, RouteInformationReportingType.none))
                && (!Equals(_currentIntentionToReport, status))
            )
            {
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: "Both Router.navigate and Router.neglect have been called in this "
                            + "build cycle, and the Router cannot decide whether to report the "
                            + "route information. Please make sure only one of them is called "
                            + "within the same build cycle."
                    )
                );
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
        _routeInformation.value = _retrieveNewRouteInformation();
        _currentIntentionToReport ??= RouteInformationReportingType.none;
        _scheduleRouteInformationReportingTask();
    }

    public override void didChangeDependencies()
    {
        _routeParsePending = true;
        base.didChangeDependencies();
        RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: needsRestore
        );
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
        RouteInformation? currentRouteInformation =
            _routeInformation.value ?? widget.routeInformationProvider?.value;
        if ((currentRouteInformation is not null) && _routeParsePending)
        {
            _processRouteInformation(
                currentRouteInformation,
                () => widget.routerDelegate.setNewRoutePath
            );
        }
        _routeParsePending = false;
        _maybeNeedToReportRouteInformation();
    }

    public override void didUpdateWidget(Router<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if (
            (!Equals(widget.routeInformationProvider, oldWidget.routeInformationProvider))
            || (!Equals(widget.backButtonDispatcher, oldWidget.backButtonDispatcher))
            || (!Equals(widget.routeInformationParser, oldWidget.routeInformationParser))
            || (!Equals(widget.routerDelegate, oldWidget.routerDelegate))
        )
        {
            _currentRouterTransaction = new object();
        }
        if (!Equals(widget.routeInformationProvider, oldWidget.routeInformationProvider))
        {
            oldWidget.routeInformationProvider?.removeListener(
                _handleRouteInformationProviderNotification
            );
            widget.routeInformationProvider?.addListener(
                _handleRouteInformationProviderNotification
            );
            if (
                !Equals(
                    oldWidget.routeInformationProvider?.value,
                    widget.routeInformationProvider?.value
                )
            )
            {
                _handleRouteInformationProviderNotification();
            }
        }
        if (!Equals(widget.backButtonDispatcher, oldWidget.backButtonDispatcher))
        {
            oldWidget.backButtonDispatcher?.removeCallback(_handleBackButtonDispatcherNotification);
            widget.backButtonDispatcher?.addCallback(_handleBackButtonDispatcherNotification);
        }
        if (!Equals(widget.routerDelegate, oldWidget.routerDelegate))
        {
            oldWidget.routerDelegate.removeListener(_handleRouterDelegateNotification);
            widget.routerDelegate.addListener(_handleRouterDelegateNotification);
            _maybeNeedToReportRouteInformation();
        }
    }

    public override void dispose()
    {
        _routeInformation.dispose();
        widget.routeInformationProvider?.removeListener(
            _handleRouteInformationProviderNotification
        );
        widget.backButtonDispatcher?.removeCallback(_handleBackButtonDispatcherNotification);
        widget.routerDelegate.removeListener(_handleRouterDelegateNotification);
        _currentRouterTransaction = null;
        _properties.forEach(
            (property, listener) =>
            {
                if (!property._disposed)
                {
                    property.removeListener(listener);
                }
            }
        );
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual void _processRouteInformation(
        RouteInformation information,
        Func<Func<T, Future>> delegateRouteSetter
    )
    {
        DartRuntimePrimitives.Assert(() => _routeParsePending);
        _routeParsePending = false;
        _currentRouterTransaction = new object();
        DartRuntimePrimitives.Ignore(
            widget
                .routeInformationParser!.parseRouteInformationWithDependencies(information, context)
                .then(
                    _processParsedRouteInformation(_currentRouterTransaction, delegateRouteSetter)
                )
        );
    }

    internal virtual Func<object?, Future> _processParsedRouteInformation(
        object? transaction,
        Func<Func<T, Future>> delegateRouteSetter
    )
    {
        return async (data) =>
        {
            if (!Equals(_currentRouterTransaction, transaction))
            {
                return;
            }
            await delegateRouteSetter()(DartRuntimePrimitives.ConvertValue<T>(data));
            if (Equals(_currentRouterTransaction, transaction))
            {
                _rebuild();
            }
            throw new InvalidOperationException("Dart closure completed without a value.");
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleRouteInformationProviderNotification()
    {
        _routeParsePending = true;
        _processRouteInformation(
            widget.routeInformationProvider!.value,
            () => widget.routerDelegate.setNewRoutePath
        );
    }

    internal virtual Future<bool> _handleBackButtonDispatcherNotification()
    {
        _currentRouterTransaction = new object();
        return widget
            .routerDelegate.popRoute()
            .then<bool>(_handleRoutePopped(_currentRouterTransaction));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Func<bool, Future<bool>> _handleRoutePopped(object? transaction)
    {
        return (data) =>
        {
            if (!Equals(transaction, _currentRouterTransaction))
            {
                return new SynchronousFuture<bool>(true);
            }
            _rebuild();
            return new SynchronousFuture<bool>(data);
            throw new InvalidOperationException("Dart closure completed without a value.");
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _rebuild()
    {
        setState(() => { });
        _maybeNeedToReportRouteInformation();
    }

    internal virtual void _handleRouterDelegateNotification()
    {
        setState(() => { });
        _maybeNeedToReportRouteInformation();
    }

    public override Widget build(BuildContext context)
    {
        return new UnmanagedRestorationScope(
            bucket: bucket,
            child: new _RouterScope__router(
                routeInformationProvider: widget.routeInformationProvider,
                backButtonDispatcher: widget.backButtonDispatcher,
                routeInformationParser: widget.routeInformationParser,
                routerDelegate: widget.routerDelegate,
                routerState: this,
                child: new Builder(builder: widget.routerDelegate.build)
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RestorationBucket? bucket => _bucket;

    public virtual void didToggleBucket(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(
            () =>
                (property._restorationId is null)
                || (_debugDoingRestore && (property._restorationId == restorationId)),
            () => (object?)$"Property is already registered under {property._restorationId}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                _debugDoingRestore
                || !_properties.Keys.map((r) => r._restorationId).contains(restorationId),
            () => (object?)$"\"{restorationId}\" is already registered to another property."
        );
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue
            ? property.fromPrimitivesObject(bucket!.read<object>(restorationId))
            : property.createDefaultValueObject();
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
        DartRuntimePrimitives.Assert(() =>
            (property._restorationId == restorationId)
            && Equals(property._owner, this)
            && _properties.ContainsKey(property)
        );
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
    }

    public virtual void unregisterFromRestoration(IRestorableProperty property)
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
        RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: false
        );
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
            RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent))
                && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore =>
        DartRuntimePrimitives.ConvertValue<bool>(
            _debugPropertiesWaitingForReregistration is not null
        );

    public virtual void _doRestore(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
        {
            if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "Previously registered RestorableProperties must be re-registered in \"restoreState\"."
                            ),
                            new ErrorDescription(
                                $"The RestorableProperties with the following IDs were not re-registered to {this} when "
                                    + "\"restoreState\" was called:"
                            ),
                        }
                    )
                );
            }
            _debugPropertiesWaitingForReregistration = null;
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
    }

    public virtual bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(
                newBucket: null,
                restorePending: restorePending
            );
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(
                newBucket: newBucketLocal,
                restorePending: restorePending
            );
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

    public virtual bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach(
                    (__arg0) => ((Action<IRestorableProperty>)_updateProperty)(__arg0)
                );
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(IRestorableProperty property)
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

    public virtual void _unregister(IRestorableProperty property)
    {
        Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        property.removeListener(listener);
        property._unregister();
    }
}

internal class _RouterScope__router : InheritedWidget
{
    public virtual ValueListenable<RouteInformation>? routeInformationProvider { get; private set; }
    public virtual BackButtonDispatcher? backButtonDispatcher { get; private set; }
    public virtual object? routeInformationParser { get; private set; } = default!;
    public virtual object routerDelegate { get; private set; } = default!;
    public virtual IRouterState routerState { get; private set; } = default!;

    internal _RouterScope__router(
        ValueListenable<RouteInformation>? routeInformationProvider,
        BackButtonDispatcher? backButtonDispatcher,
        object? routeInformationParser,
        object routerDelegate,
        IRouterState routerState,
        Widget child
    )
        : base(child: child)
    {
        this.routeInformationProvider = routeInformationProvider;
        this.backButtonDispatcher = backButtonDispatcher;
        this.routeInformationParser = routeInformationParser;
        this.routerDelegate = routerDelegate;
        this.routerState = routerState;
        System.Diagnostics.Debug.Assert(
            (routeInformationProvider is null) || (routeInformationParser is not null)
        );
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_RouterScope__router)oldWidget;
        return (!Equals(routeInformationProvider, __oldWidget.routeInformationProvider))
            || (!Equals(backButtonDispatcher, __oldWidget.backButtonDispatcher))
            || (!Equals(routeInformationParser, __oldWidget.routeInformationParser))
            || (!Equals(routerDelegate, __oldWidget.routerDelegate))
            || (!Equals(routerState, __oldWidget.routerState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class _CallbackHookProvider__router<T>
{
    internal virtual ObserverList<Func<T>> _callbacks { get; private set; } =
        new ObserverList<Func<T>>();

    public virtual bool hasCallbacks => Enumerable.Any(_callbacks);

    public virtual void addCallback(Func<T> callback) => _callbacks.add(callback);

    public virtual void removeCallback(Func<T> callback) => _callbacks.remove(callback);

    public virtual T invokeCallback(T defaultValue)
    {
        if (!Enumerable.Any(_callbacks))
        {
            return defaultValue;
        }
        try
        {
            return _callbacks.Single()();
        }
        catch (Exception exceptionLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            FlutterError.reportError(
                new FlutterErrorDetails(
                    exception: exceptionLocal,
                    stack: stackLocal,
                    library: "widget library",
                    context: new ErrorDescription($"while invoking the callback for {GetType()}"),
                    informationCollector: () =>
                        new List<DiagnosticsNode>
                        {
                            new DiagnosticsProperty<_CallbackHookProvider__router<T>>(
                                $"The {GetType()} that invoked the callback was",
                                this,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        }
                )
            );
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
                __late__children = new HashSet<ChildBackButtonDispatcher>()!;
                __late__children_initialized = true;
            }
            return __late__children;
        }
    }

    public override bool hasCallbacks =>
        DartRuntimePrimitives.ConvertValue<bool>(base.hasCallbacks || Enumerable.Any(_children));

    public override Future<bool> invokeCallback(Future<bool> defaultValue)
    {
        if (Enumerable.Any(_children))
        {
            List<ChildBackButtonDispatcher> children = _children.ToList().ToList();
            long childIndex = checked(children.Count) - 1L;
            Future<bool> notifyNextChild(bool result)
            {
                if (result)
                {
                    return new SynchronousFuture<bool>(result);
                }
                if (childIndex > 0L)
                {
                    childIndex -= 1L;
                    return children[(int)childIndex]
                        .notifiedByParent(defaultValue)
                        .then<bool>(notifyNextChild);
                }
                return base.invokeCallback(defaultValue);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            return children[(int)childIndex]
                .notifiedByParent(defaultValue)
                .then<bool>(notifyNextChild);
        }
        return base.invokeCallback(defaultValue);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ChildBackButtonDispatcher createChildBackButtonDispatcher()
    {
        return new ChildBackButtonDispatcher(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void takePriority() => _children.Clear();

    public virtual void deferTo(ChildBackButtonDispatcher child)
    {
        DartRuntimePrimitives.Assert(() => hasCallbacks);
        _children.remove(child);
        _children.add(child);
    }

    public virtual void forget(ChildBackButtonDispatcher child) => _children.remove(child);
}

public class RootBackButtonDispatcher : BackButtonDispatcher, WidgetsBindingObserver
{
    public RootBackButtonDispatcher() { }

    public override void addCallback(Func<Future<bool>> callback)
    {
        if (!hasCallbacks)
        {
            WidgetsBinding.instance.addObserver(this);
        }
        base.addCallback(callback);
    }

    public override void removeCallback(Func<Future<bool>> callback)
    {
        base.removeCallback(callback);
        if (!hasCallbacks)
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
        return invokeCallback(defaultValue);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void takePriority()
    {
        parent.deferTo(this);
        base.takePriority();
    }

    public override void deferTo(ChildBackButtonDispatcher child)
    {
        DartRuntimePrimitives.Assert(() => hasCallbacks);
        parent.deferTo(this);
        base.deferTo(child);
    }

    public override void removeCallback(Func<Future<bool>> callback)
    {
        base.removeCallback(callback);
        if (!hasCallbacks)
        {
            parent.forget(this);
        }
    }
}

public class BackButtonListener : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Func<Future<bool>> onBackButtonPressed { get; private set; } = default!;

    public BackButtonListener(
        Key? key = null,
        Widget child = default!,
        Func<Future<bool>> onBackButtonPressed = default!
    )
        : base(key: key)
    {
        this.child = child;
        this.onBackButtonPressed = onBackButtonPressed;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _BackButtonListenerState__router());
}

internal class _BackButtonListenerState__router : State<BackButtonListener>
{
    public virtual BackButtonDispatcher? dispatcher { get; set; } = default;

    public override void didChangeDependencies()
    {
        dispatcher?.removeCallback(widget.onBackButtonPressed);
        BackButtonDispatcher? rootBackDispatcher = Router<object>
            .untypedOf(context)!
            .backButtonDispatcher;
        DartRuntimePrimitives.Assert(
            () => rootBackDispatcher is not null,
            () => (object?)"The parent router must have a backButtonDispatcher to use this widget"
        );
        dispatcher = DartRuntimePrimitives.ConvertValue<BackButtonDispatcher>(
            (
                (Func<ChildBackButtonDispatcher>)(
                    () =>
                    {
                        var __cascade = rootBackDispatcher!.createChildBackButtonDispatcher();
                        __cascade.addCallback(widget.onBackButtonPressed);
                        __cascade.takePriority();
                        return __cascade;
                    }
                )
            )()
        );
        base.didChangeDependencies();
    }

    public override void didUpdateWidget(BackButtonListener oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.onBackButtonPressed, widget.onBackButtonPressed))
        {
            dispatcher?.removeCallback(oldWidget.onBackButtonPressed);
            dispatcher?.addCallback(widget.onBackButtonPressed);
            dispatcher?.takePriority();
        }
    }

    public override void dispose()
    {
        dispatcher?.removeCallback(widget.onBackButtonPressed);
        base.dispose();
    }

    public override Widget build(BuildContext context) => widget.child;
}

public abstract class RouteInformationParser<T>
{
    protected RouteInformationParser() { }

    public virtual Future<T> parseRouteInformation(RouteInformation routeInformation)
    {
        throw new NotImplementedException(
            "One of the parseRouteInformation or "
                + "parseRouteInformationWithDependencies must be implemented"
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T> parseRouteInformationWithDependencies(
        RouteInformation routeInformation,
        BuildContext context
    )
    {
        return parseRouteInformation(routeInformation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RouteInformation? restoreRouteInformation(T configuration) =>
        DartRuntimePrimitives.ConvertValue<RouteInformation>(null);
}

public abstract class RouterDelegate<T> : IRouterDelegate
{
    Widget IRouterDelegate.createRouterWidget(
        RouteInformationProvider? provider,
        object? parser,
        BackButtonDispatcher? dispatcher,
        string? restorationScopeId
    )
    {
        if (parser is not null && parser is not RouteInformationParser<T>)
        {
            throw new ArgumentException(
                $"The route information parser must use {typeof(T)}.",
                nameof(parser)
            );
        }

        return new Router<T>(
            routeInformationProvider: provider,
            routeInformationParser: (RouteInformationParser<T>?)parser,
            routerDelegate: this,
            backButtonDispatcher: dispatcher,
            restorationScopeId: restorationScopeId
        );
    }

    public virtual void addListener(Action listener) => throw new NotSupportedException();

    public virtual void removeListener(Action listener) => throw new NotSupportedException();

    public virtual Future setInitialRoutePath(T configuration)
    {
        return setNewRoutePath(configuration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future setRestoredRoutePath(T configuration)
    {
        return setNewRoutePath(configuration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Future setNewRoutePath(T configuration);
    public abstract Future<bool> popRoute();
    public virtual T? currentConfiguration => DartRuntimePrimitives.ConvertValue<T>(null);
    public abstract Widget build(BuildContext context);
}

public abstract class RouteInformationProvider : ValueListenable<RouteInformation>
{
    public virtual RouteInformation value => throw new NotSupportedException();

    public virtual void routerReportsNewRouteInformation(
        RouteInformation routeInformation,
        RouteInformationReportingType type = RouteInformationReportingType.none
    ) { }

    private readonly HashSet<Action> __listeners = new();
    public virtual bool hasListeners => __listeners.Count != 0;

    public virtual void addListener(Action listener) => __listeners.Add(listener);

    public virtual void removeListener(Action listener) => __listeners.Remove(listener);

    public virtual void notifyListeners()
    {
        foreach (var listener in __listeners.ToArray())
        {
            listener();
        }
    }

    public virtual void dispose() => __listeners.Clear();
}

public class PlatformRouteInformationProvider : RouteInformationProvider, WidgetsBindingObserver
{
    internal virtual RouteInformation _value { get; set; } = default!;
    internal virtual RouteInformation _valueInEngine { get; set; } =
        new RouteInformation(
            uri: DartUri.parse(WidgetsBinding.instance.platformDispatcher.defaultRouteName)
        );

    public PlatformRouteInformationProvider(RouteInformation initialRouteInformation)
    {
        _value = initialRouteInformation;
    }

    internal static bool _equals(DartUri a, DartUri b)
    {
        return (a.path == b.path)
            && (a.fragment == b.fragment)
            && new DeepCollectionEquality().equals(a.queryParametersAll, b.queryParametersAll);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void routerReportsNewRouteInformation(
        RouteInformation routeInformation,
        RouteInformationReportingType type = RouteInformationReportingType.none
    )
    {
        DartRuntimePrimitives.Ignore(SystemNavigator.selectMultiEntryHistory());
        DartRuntimePrimitives.Ignore(
            SystemNavigator.routeInformationUpdated(
                uri: routeInformation.uri,
                state: routeInformation.state,
                replace: type switch
                {
                    RouteInformationReportingType.neglect => true,
                    RouteInformationReportingType.navigate => false,
                    RouteInformationReportingType.none => _equals(
                        _valueInEngine.uri,
                        routeInformation.uri
                    ),
                    _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
                }
            )
        );
        _value = routeInformation;
        _valueInEngine = routeInformation;
    }

    public override RouteInformation value => _value;

    internal virtual void _platformReportsNewRouteInformation(RouteInformation routeInformation)
    {
        if (Equals(_value, routeInformation))
        {
            return;
        }
        _value = routeInformation;
        _valueInEngine = routeInformation;
        notifyListeners();
    }

    public override void addListener(Action listener)
    {
        if (!hasListeners)
        {
            WidgetsBinding.instance.addObserver(this);
        }
        base.addListener(listener);
    }

    public override void removeListener(Action listener)
    {
        base.removeListener(listener);
        if (!hasListeners)
        {
            WidgetsBinding.instance.removeObserver(this);
        }
    }

    public override void dispose()
    {
        if (hasListeners)
        {
            WidgetsBinding.instance.removeObserver(this);
        }
        base.dispose();
    }

    public virtual async Future<bool> didPushRouteInformation(RouteInformation routeInformation)
    {
        DartRuntimePrimitives.Assert(() => hasListeners);
        _platformReportsNewRouteInformation(routeInformation);
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class PopNavigatorRouterDelegateMixin<T> : RouterDelegate<T>
{
    public abstract override void addListener(Action listener);
    public abstract override void removeListener(Action listener);
    public abstract GlobalKey<NavigatorState>? navigatorKey { get; }

    public override Future<bool> popRoute()
    {
        NavigatorState? navigator = navigatorKey?.currentState;
        return navigator?.maybePop<object>() ?? new SynchronousFuture<bool>(false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _RestorableRouteInformation__router : RestorableValue<RouteInformation?>
{
    public override RouteInformation? createDefaultValue() =>
        DartRuntimePrimitives.ConvertValue<RouteInformation>(null);

    public override void didUpdateValue(RouteInformation? oldValue)
    {
        notifyListeners();
    }

    public override RouteInformation? fromPrimitives(object? data)
    {
        if (data is null)
        {
            return null;
        }
        DartRuntimePrimitives.Assert(() =>
            (data is List<object?>) && (checked(((List<object>)data).Count) == 2L)
        );
        var castedData = ((List<object?>?)data)!;
        var uriLocal = ((string?)castedData.First())!;
        if (uriLocal is null)
        {
            return null;
        }
        return new RouteInformation(uri: DartUri.parse(uriLocal), state: castedData.Last());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? toPrimitives()
    {
        return (value is null) ? null : new List<object?> { value!.uri.ToString(), value!.state };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
