// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/navigator.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class NavigatorLibrary
{
    internal static Duration _kAndroidRefocusingDelayDuration = Duration.Create(milliseconds: 300L);
}

public delegate dynamic RouteFactory(RouteSettings settings);

public delegate List<dynamic> RouteListFactory(NavigatorState navigator, string initialRoute);

public delegate Route<T> RestorableRouteBuilder<T>(BuildContext context, object? arguments);

public delegate bool RoutePredicate(dynamic route);

public delegate Future<bool> WillPopCallback();

public delegate bool PopPageCallback(dynamic route, dynamic result);

public delegate void DidRemovePageCallback(Page<object?> page);

public enum RoutePopDisposition
{
    pop,
    doNotPop,
    bubble
}

public abstract class RouteBase
{
    internal abstract NavigatorState? _navigator { get; set; }
    internal abstract bool _installed { get; }
    internal abstract bool _isInstalledIn(NavigatorState state);
    internal abstract bool _isPageBased { get; }
    internal abstract void _updateSettings(RouteSettings newSettings);
    internal abstract void _updateRestorationId(string? restorationId);
    internal abstract object? currentResultObject { get; }
    internal abstract Future disposalCompleted { get; }
    internal abstract bool popCompleted { get; }
    internal abstract bool didPopObject(object? result);
    internal abstract void didCompleteObject(object? result);
    internal abstract void onPopInvokedWithResultObject(bool didPop, object? result);
    internal abstract bool _debugCheckCanConsumeResult(object? result, string methodName);

    public abstract bool requestFocus { get; }
    public abstract NavigatorState? navigator { get; }
    public abstract RouteSettings settings { get; }
    public abstract global::Doroti.Framework.Foundation.ValueListenable<string?> restorationScopeId { get; }
    public abstract List<OverlayEntry> overlayEntries { get; }
    public abstract void install();
    public abstract global::Doroti.Framework.Scheduler.TickerFuture didPush();
    public abstract void didAdd();
    public abstract void didReplace(dynamic? oldRoute);
    public abstract Future<RoutePopDisposition> willPop();
    public abstract RoutePopDisposition popDisposition { get; }
    public abstract void onPopInvoked(bool didPop);
    public abstract bool willHandlePopInternally { get; }
    public abstract void didPopNext(dynamic nextRoute);
    public abstract void didChangeNext(dynamic? nextRoute);
    public abstract void didChangePrevious(dynamic? previousRoute);
    public abstract void changedInternalState();
    public abstract void changedExternalState();
    public abstract void dispose();
    public abstract bool isCurrent { get; }
    public abstract bool isFirst { get; }
    public abstract bool hasActiveRouteBelow { get; }
    public abstract bool isActive { get; }
}

public abstract class Route<T> : RouteBase
{
    internal virtual bool? _requestFocus { get; private set; }
    internal override NavigatorState? _navigator { get; set; } = default;
    internal virtual RouteSettings _settings { get; set; } = default!;
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<string?> _restorationScopeId { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<string?>(null);
    internal virtual Completer<T?> _popCompleter { get; private set; } = new Completer<T?>();
    internal virtual Completer<T?> _disposeCompleter { get; private set; } = new Completer<T?>();

    protected Route(RouteSettings? settings = null, bool? requestFocus = null)
    {
        _settings = settings ?? new RouteSettings();
        _requestFocus = requestFocus;
    }

    public override bool requestFocus => DartRuntimePrimitives.ConvertValue<bool>((_requestFocus ?? navigator?.widget.requestFocus) ?? false);
    public override NavigatorState? navigator => _navigator;
    internal override bool _installed => _navigator is not null;
    internal override bool _isInstalledIn(NavigatorState state) => DartRuntimePrimitives.ConvertValue<bool>(Equals(_navigator, state));
    public override RouteSettings settings => _settings;
    internal override bool _isPageBased => settings is Page<object?>;
    public override global::Doroti.Framework.Foundation.ValueListenable<string?> restorationScopeId => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Foundation.ValueListenable<string?>>(_restorationScopeId);
    internal override void _updateSettings(RouteSettings newSettings)
    {
        if (!Equals(_settings, newSettings))
        {
            _settings = newSettings;
            if (_installed)
            {
                changedInternalState();
            }
        }
    }

    internal override void _updateRestorationId(string? restorationId)
    {
        _restorationScopeId.value = restorationId;
    }

    public override List<OverlayEntry> overlayEntries => new List<OverlayEntry>();
    public override void install()
    {
    }

    public override global::Doroti.Framework.Scheduler.TickerFuture didPush()
    {
        return ((Func<global::Doroti.Framework.Scheduler.TickerFuture>)(() =>
{
    var __cascade = Scheduler.TickerFuture.CreateComplete();
    __cascade.then((_) =>
    {
        if (requestFocus)
        {
            navigator!.focusNode.enclosingScope?.requestFocus();
        }
        return default!;
    });
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didAdd()
    {
        if (requestFocus)
        {
            DartRuntimePrimitives.Ignore(Scheduler.TickerFuture.CreateComplete().then((_) =>
            {
                navigator?.focusNode.enclosingScope?.requestFocus();
                return default!;
            }));
        }
    }

    public override void didReplace(dynamic? oldRoute)
    {
    }

    public async override Future<RoutePopDisposition> willPop()
    {
        return isFirst ? RoutePopDisposition.bubble : RoutePopDisposition.pop;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RoutePopDisposition popDisposition
    {
        get
        {
            if (_isPageBased)
            {
                var page = ((Page<object?>?)settings)!;
                if (!page.canPop)
                {
                    return RoutePopDisposition.doNotPop;
                }
            }
            return isFirst ? RoutePopDisposition.bubble : RoutePopDisposition.pop;
        }
    }
    public override void onPopInvoked(bool didPop)
    {
    }

    public virtual void onPopInvokedWithResult(bool didPop, T? result)
    {
        if (_isPageBased)
        {
            var page = ((Page<T>?)settings)!;
            page.onPopInvoked(didPop, result);
        }
    }

    public override bool willHandlePopInternally => false;
    public virtual T? currentResult => DartRuntimePrimitives.ConvertValue<T>(null);
    internal override object? currentResultObject => currentResult;
    internal override Future disposalCompleted => _disposeCompleter.future;
    internal override bool popCompleted => _popCompleter.isCompleted;
    public virtual Future<T?> popped => _popCompleter.future;
    public virtual bool didPop(T? result)
    {
        didComplete(result);
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didComplete(T? result)
    {
        _popCompleter.complete(result ?? currentResult);
    }

    internal override bool didPopObject(object? result)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckCanConsumeResult(result, "pop"));
        return didPop(DartRuntimePrimitives.ConvertValue<T>(result));
    }

    internal override void didCompleteObject(object? result)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckCanConsumeResult(result, "complete"));
        didComplete(DartRuntimePrimitives.ConvertValue<T>(result));
    }

    internal override void onPopInvokedWithResultObject(bool didPop, object? result)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckCanConsumeResult(result, "pop"));
        onPopInvokedWithResult(didPop, DartRuntimePrimitives.ConvertValue<T>(result));
    }

    public override void didPopNext(dynamic nextRoute)
    {
    }

    public override void didChangeNext(dynamic? nextRoute)
    {
    }

    public override void didChangePrevious(dynamic? previousRoute)
    {
    }

    public override void changedInternalState()
    {
    }

    public override void changedExternalState()
    {
    }

    public override void dispose()
    {
        _navigator = null;
        _restorationScopeId.dispose();
        _disposeCompleter.complete();
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
    }

    public override bool isCurrent
    {
        get
        {
            if (!_installed)
            {
                return false;
            }
            _RouteEntry__navigator? currentRouteEntry = _navigator!._lastRouteEntryWhereOrNull(_RouteEntry__navigator.isPresentPredicate);
            if (currentRouteEntry is null)
            {
                return false;
            }
            return Equals(currentRouteEntry.route, this);
        }
    }
    public override bool isFirst
    {
        get
        {
            if (!_installed)
            {
                return false;
            }
            _RouteEntry__navigator? currentRouteEntry = _navigator!._firstRouteEntryWhereOrNull(_RouteEntry__navigator.isPresentPredicate);
            if (currentRouteEntry is null)
            {
                return false;
            }
            return Equals(currentRouteEntry.route, this);
        }
    }
    public override bool hasActiveRouteBelow
    {
        get
        {
            if (!_installed)
            {
                return false;
            }
            foreach (_RouteEntry__navigator entry in _navigator!._history)
            {
                if (Equals(entry.route, this))
                {
                    return false;
                }
                if (_RouteEntry__navigator.isPresentPredicate(entry))
                {
                    return true;
                }
            }
            return false;
        }
    }
    public override bool isActive
    {
        get
        {
            return _navigator?._firstRouteEntryWhereOrNull(_RouteEntry__navigator.isRoutePredicate(this))?.isPresent ?? false;
        }
    }
    internal override bool _debugCheckCanConsumeResult(object? result, string methodName)
    {
        // Flutter checks T?: dismissing a dialog without a result is valid for
        // every route result type, including a non-nullable value type.
        if (result is not null && result is not T)
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A request was made to pop a route with a result of type {DartRuntimePrimitives.RuntimeType(result)}, but the route expected a value of type {typeof(T)}."), new global::Doroti.Framework.Foundation.ErrorDescription($"This usually happens when the type provided to Navigator.{methodName}() " + "is not a subtype of the type expected by the Route (e.g. DialogRoute<Null>), " + "or when a generic type is explicitly provided to a route creation method " + "(such as showDialog<T>()) but the popped value does not match this type."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object>("The route was", this), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>("The provided result was", result) }));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RouteSettings
{
    public virtual string? name { get; private set; }
    public virtual object? arguments { get; private set; }

    public RouteSettings(string? name = null, object? arguments = null)
    {
        this.name = name;
        this.arguments = arguments;
    }

    public override string ToString() => $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "RouteSettings")}({((name is null) ? "none" : $"\"{name}\"")}, {arguments})";
}

public abstract class Page<T> : RouteSettings
{
    public virtual global::Doroti.Framework.Foundation.LocalKey? key { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual global::System.Action<bool, T?> onPopInvoked { get; private set; } = default!;
    public virtual bool canPop { get; private set; } = default!;

    protected Page(global::Doroti.Framework.Foundation.LocalKey? key = null, string? name = null, object? arguments = null, string? restorationId = null, bool canPop = true, global::System.Action<bool, T?> onPopInvoked = default!) : base(name: name, arguments: arguments)
    {
        global::System.Action<bool, T?> __onPopInvoked = onPopInvoked ?? ((didPop, result) => _defaultPopInvokedHandler(didPop, result));
        this.key = key;
        this.restorationId = restorationId;
        this.canPop = canPop;
        this.onPopInvoked = __onPopInvoked;
    }

    public static void _defaultPopInvokedHandler(bool didPop, object? result)
    {
    }

    public virtual bool canUpdate(Page<object?> other)
    {
        return Equals(DartRuntimePrimitives.RuntimeType(other), GetType()) && Equals(other.key, key);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Route<T> createRoute(BuildContext context);
    public override string ToString() => $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "Page")}(\"{name}\", {key}, {arguments})";
}

public class NavigatorObserver
{
    internal static Expando<NavigatorState> _navigators = new Expando<NavigatorState>();

    public virtual NavigatorState? navigator => _navigators[this];
    public virtual void didPush(dynamic route, dynamic? previousRoute)
    {
    }

    public virtual void didPop(dynamic route, dynamic? previousRoute)
    {
    }

    public virtual void didRemove(dynamic route, dynamic? previousRoute)
    {
    }

    public virtual void didReplace(dynamic? newRoute = null, dynamic? oldRoute = null)
    {
    }

    public virtual void didChangeTop(dynamic topRoute, dynamic? previousTopRoute)
    {
    }

    public virtual void didStartUserGesture(dynamic route, dynamic? previousRoute)
    {
    }

    public virtual void didStopUserGesture()
    {
    }

}

public class HeroControllerScope : InheritedWidget
{
    public virtual HeroController? controller { get; private set; }

    public HeroControllerScope(global::Doroti.Framework.Foundation.Key? key = null, HeroController controller = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.controller = controller;
    }

    public static HeroControllerScope CreateNone(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!)
    {
        var __instance = new HeroControllerScope(key, default!, child);
        __instance.controller = null;
        return __instance;
    }

    public static HeroController? maybeOf(BuildContext context)
    {
        HeroControllerScope? host = context.dependOnInheritedWidgetOfExactType<HeroControllerScope>();
        return host?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static HeroController of(BuildContext context)
    {
        HeroController? controller = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
            {
                if (controller is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("HeroControllerScope.of() was called with a context that does not contain a " + "HeroControllerScope widget.\n" + "No HeroControllerScope widget ancestor could be found starting from the " + "context that was passed to HeroControllerScope.of(). This can happen " + "because you are using a widget that looks for a HeroControllerScope " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return controller!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (HeroControllerScope)oldWidget;
        return !Equals(__oldWidget.controller, controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class RouteTransitionRecord
{
    public abstract RouteBase route { get; }
    public abstract bool isWaitingForEnteringDecision { get; }
    public abstract bool isWaitingForExitingDecision { get; }
    public abstract void markForPush();
    public abstract void markForAdd();
    public abstract void markForPop(dynamic? result = null);
    public abstract void markForComplete(dynamic? result = null);
    public virtual void markForRemove() => markForComplete();
}

public abstract class TransitionDelegate<T>
{
    protected TransitionDelegate()
    {
    }

    internal virtual IEnumerable<RouteTransitionRecord> _transition(List<RouteTransitionRecord> newPageRouteHistory, DartMap<RouteTransitionRecord?, RouteTransitionRecord> locationToExitingPageRoute, DartMap<RouteTransitionRecord?, List<RouteTransitionRecord>> pageRouteToPagelessRoutes)
    {
        IEnumerable<RouteTransitionRecord> results = resolve(newPageRouteHistory: newPageRouteHistory, locationToExitingPageRoute: locationToExitingPageRoute, pageRouteToPagelessRoutes: pageRouteToPagelessRoutes);
        DartRuntimePrimitives.Assert(() =>
            {
                List<RouteTransitionRecord> resultsToVerify = results.ToList().ToList();
                HashSet<RouteTransitionRecord> exitingPageRoutes = locationToExitingPageRoute.Values.toSet();
                foreach (var exitingPageRoute in exitingPageRoutes)
                {
                    DartRuntimePrimitives.Assert(() => !exitingPageRoute.isWaitingForExitingDecision);
                    if (pageRouteToPagelessRoutes.ContainsKey(exitingPageRoute))
                    {
                        foreach (RouteTransitionRecord pagelessRoute in pageRouteToPagelessRoutes.GetValueOrDefault(exitingPageRoute)!)
                        {
                            DartRuntimePrimitives.Assert(() => !pagelessRoute.isWaitingForExitingDecision);
                        }
                    }
                }
                var indexOfNextRouteInNewHistory = 0L;
                foreach (_RouteEntry__navigator routeEntry in resultsToVerify.cast<_RouteEntry__navigator>())
                {
                    DartRuntimePrimitives.Assert(() => !routeEntry.isWaitingForEnteringDecision && !routeEntry.isWaitingForExitingDecision);
                    if ((indexOfNextRouteInNewHistory >= checked(newPageRouteHistory.Count)) || (!Equals(routeEntry, newPageRouteHistory[(int)indexOfNextRouteInNewHistory])))
                    {
                        DartRuntimePrimitives.Assert(() => exitingPageRoutes.Contains(routeEntry));
                        exitingPageRoutes.Remove(routeEntry);
                    }
                    else
                    {
                        indexOfNextRouteInNewHistory += 1L;
                    }
                }
                DartRuntimePrimitives.Assert(() => (indexOfNextRouteInNewHistory == checked(newPageRouteHistory.Count)) && !Enumerable.Any(exitingPageRoutes), () => (object?)$"The merged result from the {GetType()}.resolve does not include all " + "required routes. Do you remember to merge all exiting routes?");
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return results;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract IEnumerable<RouteTransitionRecord> resolve(List<RouteTransitionRecord> newPageRouteHistory, DartMap<RouteTransitionRecord?, RouteTransitionRecord> locationToExitingPageRoute, DartMap<RouteTransitionRecord?, List<RouteTransitionRecord>> pageRouteToPagelessRoutes);
}

public class DefaultTransitionDelegate<T> : TransitionDelegate<T>
{
    public DefaultTransitionDelegate()
    {
    }

    public override IEnumerable<RouteTransitionRecord> resolve(List<RouteTransitionRecord> newPageRouteHistory, DartMap<RouteTransitionRecord?, RouteTransitionRecord> locationToExitingPageRoute, DartMap<RouteTransitionRecord?, List<RouteTransitionRecord>> pageRouteToPagelessRoutes)
    {
        var results = new List<RouteTransitionRecord>();
        void handleExitingRoute(RouteTransitionRecord? location, bool isLast)
        {
            RouteTransitionRecord? exitingPageRoute = locationToExitingPageRoute.GetValueOrDefault(DartRuntimePrimitives.RequireReference(location));
            if (exitingPageRoute is null)
            {
                return;
            }
            if (exitingPageRoute.isWaitingForExitingDecision)
            {
                bool hasPagelessRoute = pageRouteToPagelessRoutes.ContainsKey(exitingPageRoute);
                bool isLastExitingPageRoute = isLast && !locationToExitingPageRoute.ContainsKey(exitingPageRoute);
                if (isLastExitingPageRoute && !hasPagelessRoute)
                {
                    exitingPageRoute.markForPop(exitingPageRoute.route.currentResultObject);
                }
                else
                {
                    exitingPageRoute.markForComplete(exitingPageRoute.route.currentResultObject);
                }
                if (hasPagelessRoute)
                {
                    List<RouteTransitionRecord> pagelessRoutes = pageRouteToPagelessRoutes.GetValueOrDefault(exitingPageRoute)!.ToList();
                    foreach (var pagelessRoute in pagelessRoutes)
                    {
                        if (pagelessRoute.isWaitingForExitingDecision)
                        {
                            if (isLastExitingPageRoute && Equals(pagelessRoute, pagelessRoutes.Last()))
                            {
                                pagelessRoute.markForPop(pagelessRoute.route.currentResultObject);
                            }
                            else
                            {
                                pagelessRoute.markForComplete(pagelessRoute.route.currentResultObject);
                            }
                        }
                    }
                }
            }
            results.Add(exitingPageRoute);
            handleExitingRoute(exitingPageRoute, isLast);
        }
        handleExitingRoute(null, !Enumerable.Any(newPageRouteHistory));
        foreach (var pageRoute in newPageRouteHistory)
        {
            var isLastIteration = Equals(newPageRouteHistory.Last(), pageRoute);
            if (pageRoute.isWaitingForEnteringDecision)
            {
                if (!locationToExitingPageRoute.ContainsKey(pageRoute) && isLastIteration)
                {
                    pageRoute.markForPush();
                }
                else
                {
                    pageRoute.markForAdd();
                }
            }
            results.Add(pageRoute);
            handleExitingRoute(pageRoute, isLastIteration);
        }
        return results;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class NavigatorLibrary
{
    public static TraversalEdgeBehavior kDefaultRouteTraversalEdgeBehavior = TraversalEdgeBehavior.parentScope;
}

public static partial class NavigatorLibrary
{
    public static TraversalEdgeBehavior kDefaultRouteDirectionalTraversalEdgeBehavior = TraversalEdgeBehavior.stop;
}

public class Navigator : StatefulWidget
{
    internal static RouteBase _requireRoute(object? route) => route as RouteBase ?? throw new ArgumentException("Navigator route callbacks must return a Route<T> instance.", nameof(route));

    internal static readonly List<Page<object?>> _defaultPages = new();
    public virtual List<Page<object?>> pages { get; private set; } = default!;
    public virtual global::System.Func<dynamic, object?, bool>? onPopPage { get; private set; }
    public virtual global::System.Action<Page<object?>>? onDidRemovePage { get; private set; }
    public virtual TransitionDelegate<object> transitionDelegate { get; private set; } = default!;
    public virtual string? initialRoute { get; private set; }
    public virtual global::System.Func<RouteSettings, dynamic?>? onGenerateRoute { get; private set; }
    public virtual global::System.Func<RouteSettings, dynamic?>? onUnknownRoute { get; private set; }
    public virtual List<NavigatorObserver> observers { get; private set; } = default!;
    public virtual string? restorationScopeId { get; private set; }
    public virtual TraversalEdgeBehavior routeTraversalEdgeBehavior { get; private set; } = default!;
    public virtual TraversalEdgeBehavior routeDirectionalTraversalEdgeBehavior { get; private set; } = default!;
    public const string defaultRouteName = "/";
    public virtual global::System.Func<NavigatorState, string, List<dynamic>> onGenerateInitialRoutes { get; private set; } = default!;
    public virtual bool reportsRouteUpdateToEngine { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool requestFocus { get; private set; } = default!;

    public Navigator(global::Doroti.Framework.Foundation.Key? key = null, List<Page<object?>> pages = default!, global::System.Func<dynamic, object?, bool>? onPopPage = null, string? initialRoute = null, global::System.Func<NavigatorState, string, List<dynamic>> onGenerateInitialRoutes = default!, global::System.Func<RouteSettings, dynamic?>? onGenerateRoute = null, global::System.Func<RouteSettings, dynamic?>? onUnknownRoute = null, TransitionDelegate<object> transitionDelegate = default!, bool reportsRouteUpdateToEngine = false, Clip clipBehavior = Clip.hardEdge, List<NavigatorObserver> observers = default!, bool requestFocus = true, string? restorationScopeId = null, TraversalEdgeBehavior? routeTraversalEdgeBehavior = null, TraversalEdgeBehavior? routeDirectionalTraversalEdgeBehavior = null, global::System.Action<Page<object?>>? onDidRemovePage = null) : base(key: key)
    {
        List<Page<object?>> __pages = pages ?? _defaultPages;
        global::System.Func<NavigatorState, string, List<dynamic>> __onGenerateInitialRoutes = onGenerateInitialRoutes ?? defaultGenerateInitialRoutes;
        TransitionDelegate<object> __transitionDelegate = transitionDelegate ?? new DefaultTransitionDelegate<object>();
        List<NavigatorObserver> __observers = observers ?? new List<NavigatorObserver>();
        TraversalEdgeBehavior __routeTraversalEdgeBehavior = routeTraversalEdgeBehavior ?? NavigatorLibrary.kDefaultRouteTraversalEdgeBehavior;
        TraversalEdgeBehavior __routeDirectionalTraversalEdgeBehavior = routeDirectionalTraversalEdgeBehavior ?? NavigatorLibrary.kDefaultRouteDirectionalTraversalEdgeBehavior;
        this.pages = __pages;
        this.onPopPage = onPopPage;
        this.initialRoute = initialRoute;
        this.onGenerateInitialRoutes = __onGenerateInitialRoutes;
        this.onGenerateRoute = onGenerateRoute;
        this.onUnknownRoute = onUnknownRoute;
        this.transitionDelegate = __transitionDelegate;
        this.reportsRouteUpdateToEngine = reportsRouteUpdateToEngine;
        this.clipBehavior = clipBehavior;
        this.observers = __observers;
        this.requestFocus = requestFocus;
        this.restorationScopeId = restorationScopeId;
        this.routeTraversalEdgeBehavior = __routeTraversalEdgeBehavior;
        this.routeDirectionalTraversalEdgeBehavior = __routeDirectionalTraversalEdgeBehavior;
        this.onDidRemovePage = onDidRemovePage;
    }

    public static Future<T?> pushNamed<T>(BuildContext context, string routeName, object? arguments = null)
    {
        return of(context).pushNamed<T>(routeName, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePushNamed<T>(BuildContext context, string routeName, object? arguments = null)
    {
        return of(context).restorablePushNamed<T>(routeName, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<T?> pushReplacementNamed<T, TO>(BuildContext context, string routeName, TO? result = default, object? arguments = null)
    {
        return of(context).pushReplacementNamed<T, TO>(routeName, arguments: arguments, result: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePushReplacementNamed<T, TO>(BuildContext context, string routeName, TO? result = default, object? arguments = null)
    {
        return of(context).restorablePushReplacementNamed<T, TO>(routeName, arguments: arguments, result: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<T?> popAndPushNamed<T, TO>(BuildContext context, string routeName, TO? result = default, object? arguments = null)
    {
        return of(context).popAndPushNamed<T, TO>(routeName, arguments: arguments, result: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePopAndPushNamed<T, TO>(BuildContext context, string routeName, TO? result = default, object? arguments = null)
    {
        return of(context).restorablePopAndPushNamed<T, TO>(routeName, arguments: arguments, result: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<T?> pushNamedAndRemoveUntil<T>(BuildContext context, string newRouteName, global::System.Func<dynamic, bool> predicate, object? arguments = null)
    {
        return of(context).pushNamedAndRemoveUntil<T>(newRouteName, predicate, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePushNamedAndRemoveUntil<T>(BuildContext context, string newRouteName, global::System.Func<dynamic, bool> predicate, object? arguments = null)
    {
        return of(context).restorablePushNamedAndRemoveUntil<T>(newRouteName, predicate, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<T?> push<T>(BuildContext context, Route<T> route)
    {
        return of(context).push(route);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePush<T>(BuildContext context, global::System.Func<BuildContext, object?, Route<T>> routeBuilder, object? arguments = null)
    {
        return of(context).restorablePush(routeBuilder, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<T?> pushReplacement<T, TO>(BuildContext context, Route<T> newRoute, TO? result = default)
    {
        return of(context).pushReplacement<T, TO>(newRoute, result: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePushReplacement<T, TO>(BuildContext context, global::System.Func<BuildContext, object?, Route<T>> routeBuilder, TO? result = default, object? arguments = null)
    {
        return of(context).restorablePushReplacement<T, TO>(routeBuilder, result: result, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<T?> pushAndRemoveUntil<T>(BuildContext context, Route<T> newRoute, global::System.Func<dynamic, bool> predicate)
    {
        return of(context).pushAndRemoveUntil<T>(newRoute, predicate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePushAndRemoveUntil<T>(BuildContext context, global::System.Func<BuildContext, object?, Route<T>> newRouteBuilder, global::System.Func<dynamic, bool> predicate, object? arguments = null)
    {
        return of(context).restorablePushAndRemoveUntil<T>(newRouteBuilder, predicate, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void replace<T>(BuildContext context, dynamic oldRoute, Route<T> newRoute)
    {
        of(context).replace<T>(oldRoute: _requireRoute((object?)oldRoute), newRoute: newRoute);
        return;
    }

    public static string restorableReplace<T>(BuildContext context, dynamic oldRoute, global::System.Func<BuildContext, object?, Route<T>> newRouteBuilder, object? arguments = null)
    {
        return of(context).restorableReplace<T>(oldRoute: _requireRoute((object?)oldRoute), newRouteBuilder: newRouteBuilder, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void replaceRouteBelow<T>(BuildContext context, dynamic anchorRoute, Route<T> newRoute)
    {
        of(context).replaceRouteBelow<T>(anchorRoute: _requireRoute((object?)anchorRoute), newRoute: newRoute);
        return;
    }

    public static string restorableReplaceRouteBelow<T>(BuildContext context, dynamic anchorRoute, global::System.Func<BuildContext, object?, Route<T>> newRouteBuilder, object? arguments = null)
    {
        return of(context).restorableReplaceRouteBelow<T>(anchorRoute: _requireRoute((object?)anchorRoute), newRouteBuilder: newRouteBuilder, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool canPop(BuildContext context)
    {
        NavigatorState? navigator = maybeOf(context);
        return (navigator is not null) && navigator.canPop();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<bool> maybePop<T>(BuildContext context, T? result = default)
    {
        return of(context).maybePop<T>(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void pop<T>(BuildContext context, T? result = default)
    {
        of(context).pop<T>(result);
    }

    public static void popUntil(BuildContext context, global::System.Func<dynamic, bool> predicate)
    {
        of(context).popUntil(predicate);
    }

    public static void popUntilWithResult<T>(BuildContext context, global::System.Func<dynamic, bool> predicate, T? result)
    {
        of(context).popUntilWithResult<T>(predicate, result);
    }

    public static void removeRoute<T>(BuildContext context, Route<T> route, T? result = default)
    {
        of(context).removeRoute<T>(route, result);
        return;
    }

    public static void removeRouteBelow<T>(BuildContext context, Route<T> anchorRoute, T? result = default)
    {
        of(context).removeRouteBelow<T>(anchorRoute, result);
        return;
    }

    public static NavigatorState of(BuildContext context, bool rootNavigator = false)
    {
        NavigatorState? navigator = default!;
        if (context is StatefulElement { state: NavigatorState stateLocal } __object119923)
        {
            navigator = stateLocal;
        }
        navigator = rootNavigator ? (context.findRootAncestorStateOfType<NavigatorState>() ?? navigator) : (navigator ?? context.findAncestorStateOfType<NavigatorState>());
        DartRuntimePrimitives.Assert(() =>
            {
                if (navigator is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("Navigator operation requested with a context that does not include a Navigator.\n" + "The context used to push or pop routes from the Navigator must be that of a " + "widget that is a descendant of a Navigator widget."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return navigator!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static NavigatorState? maybeOf(BuildContext context, bool rootNavigator = false)
    {
        NavigatorState? navigator = default!;
        if (context is StatefulElement { state: NavigatorState stateLocal } __object121458)
        {
            navigator = stateLocal;
        }
        return rootNavigator ? (context.findRootAncestorStateOfType<NavigatorState>() ?? navigator) : (navigator ?? context.findAncestorStateOfType<NavigatorState>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static List<object> defaultGenerateInitialRoutes(NavigatorState navigator, string initialRouteName)
    {
        var result = new List<object?>();
        if (initialRouteName.startsWith("/") && (initialRouteName.Length > 1L))
        {
            initialRouteName = initialRouteName.substring(1L);
            DartRuntimePrimitives.Assert(() => defaultRouteName == "/");
            List<string>? debugRouteNames = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    debugRouteNames = new List<string> { defaultRouteName };
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            result.Add(navigator._routeNamed<object>(defaultRouteName, arguments: null, allowNull: true));
            List<string> routeParts = initialRouteName.split("/").ToList();
            if (initialRouteName.Length != 0)
            {
                var routeName = "";
                foreach (var part in routeParts)
                {
                    routeName += $"/{part}";
                    DartRuntimePrimitives.Assert(() =>
                        {
                            debugRouteNames!.Add(routeName);
                            return true;
                            throw new InvalidOperationException("Dart closure completed without a value.");
                        });
                    result.Add(navigator._routeNamed<object>(routeName, arguments: null, allowNull: true));
                }
            }
            if (result.Last() is null)
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: "Could not navigate to initial route.\n" + $"The requested route name was: \"/{initialRouteName}\"\n" + "There was no corresponding route in the app, and therefore the initial route specified will be " + $"ignored and \"{defaultRouteName}\" will be used instead."));
                        return true;
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                foreach (var routeLocal in result)
                {
                    if (routeLocal is not null)
                    {
                        _requireRoute(routeLocal).dispose();
                    }
                }
                result.Clear();
            }
        }
        else
        {
            if (initialRouteName != defaultRouteName)
            {
                result.Add(navigator._routeNamed<object>(initialRouteName, arguments: null, allowNull: true));
            }
        }
        result.removeWhere((route) => route is null);
        if (!Enumerable.Any(result))
        {
            result.Add(navigator._routeNamed<object>(defaultRouteName, arguments: null) ?? throw new InvalidOperationException("The navigator must provide the default route."));
        }
        return result.OfType<object>().ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new NavigatorState());
}

public enum _RouteLifecycle__navigator
{
    staging,
    add,
    adding,
    push,
    pushReplace,
    pushing,
    replace,
    idle,
    pop,
    complete,
    remove,
    popping,
    removing,
    dispose,
    disposing,
    disposed
}

internal delegate bool _RouteEntryPredicate__navigator(_RouteEntry__navigator entry);

public class _RoutePlaceholder__navigator
{
    internal _RoutePlaceholder__navigator()
    {
    }

}

public class _RouteEntry__navigator : RouteTransitionRecord
{
    private RouteBase __field_route = default!;
    public override RouteBase route { get => __field_route; }
    public virtual _RestorationInformation__navigator? restorationInformation { get; private set; }
    public virtual bool pageBased { get; private set; } = default!;
    public const long kDebugPopAttemptLimit = 100L;
    public static object notAnnounced = new _RoutePlaceholder__navigator();
    public virtual _RouteLifecycle__navigator currentState { get; set; } = default!;
    public virtual object? lastAnnouncedPreviousRoute { get; set; } = notAnnounced;
    public virtual WeakReference<object> lastAnnouncedPoppedNextRoute { get; set; } = new WeakReference<object>(notAnnounced);
    public virtual object? lastAnnouncedNextRoute { get; set; } = notAnnounced;
    public virtual long? lastFocusNode { get; set; } = default;
    public virtual bool imperativeRemoval { get; set; } = false;
    public virtual object? pendingResult { get; set; } = default;
    internal virtual bool _reportRemovalToObserver { get; set; } = true;
    internal virtual bool _isWaitingForExitingDecision { get; set; } = false;

    internal _RouteEntry__navigator(RouteBase route, _RouteLifecycle__navigator initialState, bool pageBased, _RestorationInformation__navigator? restorationInformation = null)
    {
        __field_route = route;
        this.pageBased = pageBased;
        this.restorationInformation = restorationInformation;
        currentState = initialState;
        System.Diagnostics.Debug.Assert(!pageBased || (route.settings is Page<object?>));
        System.Diagnostics.Debug.Assert(Equals(initialState, _RouteLifecycle__navigator.staging) || Equals(initialState, _RouteLifecycle__navigator.add) || Equals(initialState, _RouteLifecycle__navigator.push) || Equals(initialState, _RouteLifecycle__navigator.pushReplace) || Equals(initialState, _RouteLifecycle__navigator.replace));
    }

    public virtual string? restorationId
    {
        get
        {
            if (pageBased)
            {
                var page = ((Page<object?>?)route.settings)!;
                return (page.restorationId is not null) ? $"p+{page.restorationId}" : null;
            }
            if (restorationInformation is not null)
            {
                return $"r+{restorationInformation!.restorationScopeId}";
            }
            return null;
        }
    }
    public virtual bool canUpdateFrom(Page<object?> page)
    {
        if (!willBePresent)
        {
            return false;
        }
        if (!pageBased)
        {
            return false;
        }
        var routePage = ((Page<object?>?)route.settings)!;
        return page.canUpdate(routePage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleAdd(NavigatorState navigator, RouteBase? previousPresent)
    {
        DartRuntimePrimitives.Assert(() => Equals(currentState, _RouteLifecycle__navigator.add));
        DartRuntimePrimitives.Assert(() => navigator._debugLocked);
        currentState = _RouteLifecycle__navigator.adding;
        navigator._observedRouteAdditions.Enqueue(new _NavigatorPushObservation__navigator(route, previousPresent));
    }

    public virtual void handlePush(NavigatorState navigator, bool isNewFirst, RouteBase? previous, RouteBase? previousPresent)
    {
        DartRuntimePrimitives.Assert(() => Equals(currentState, _RouteLifecycle__navigator.push) || Equals(currentState, _RouteLifecycle__navigator.pushReplace) || Equals(currentState, _RouteLifecycle__navigator.replace));
        DartRuntimePrimitives.Assert(() => navigator._debugLocked);
        DartRuntimePrimitives.Assert(() => !route._installed, () => (object?)"The pushed route has already been used. When pushing a route, a new " + "Route object must be provided.");
        _RouteLifecycle__navigator previousState = currentState;
        route._navigator = navigator;
        route.install();
        DartRuntimePrimitives.Assert(() => Enumerable.Any(route.overlayEntries));
        if (Equals(currentState, _RouteLifecycle__navigator.push) || Equals(currentState, _RouteLifecycle__navigator.pushReplace))
        {
            global::Doroti.Framework.Scheduler.TickerFuture routeFuture = route.didPush();
            currentState = _RouteLifecycle__navigator.pushing;
            routeFuture.whenCompleteOrCancel(() =>
            {
                if (Equals(currentState, _RouteLifecycle__navigator.pushing))
                {
                    currentState = _RouteLifecycle__navigator.idle;
                    DartRuntimePrimitives.Assert(() => !navigator._debugLocked);
                    DartRuntimePrimitives.Assert(() =>
                        {
                            navigator._debugLocked = true;
                            return true;
                            throw new InvalidOperationException("Dart closure completed without a value.");
                        });
                    navigator._flushHistoryUpdates();
                    DartRuntimePrimitives.Assert(() =>
                        {
                            navigator._debugLocked = false;
                            return true;
                            throw new InvalidOperationException("Dart closure completed without a value.");
                        });
                }
            });
        }
        else
        {
            DartRuntimePrimitives.Assert(() => Equals(currentState, _RouteLifecycle__navigator.replace));
            route.didReplace(previous);
            currentState = _RouteLifecycle__navigator.idle;
        }
        if (isNewFirst)
        {
            route.didChangeNext(null);
        }
        if (Equals(previousState, _RouteLifecycle__navigator.replace) || Equals(previousState, _RouteLifecycle__navigator.pushReplace))
        {
            navigator._observedRouteAdditions.Enqueue(new _NavigatorReplaceObservation__navigator(route, previousPresent));
            if ((previousPresent is not null) && previousPresent._isPageBased)
            {
                var page = ((Page<object?>?)previousPresent.settings)!;
                navigator.widget.onDidRemovePage?.Invoke(page);
            }
        }
        else
        {
            DartRuntimePrimitives.Assert(() => Equals(previousState, _RouteLifecycle__navigator.push));
            navigator._observedRouteAdditions.Enqueue(new _NavigatorPushObservation__navigator(route, previousPresent));
        }
    }

    public virtual void handleDidPopNext(RouteBase poppedRoute)
    {
        route.didPopNext(poppedRoute);
        lastAnnouncedPoppedNextRoute = new WeakReference<object>(poppedRoute);
        if (lastFocusNode is not null)
        {
            DartRuntimePrimitives.Ignore(poppedRoute.disposalCompleted.then((global::System.Func<object?, Future<object?>>)(async (result) =>
            {
                switch (PlatformLibrary.defaultTargetPlatform)
                {
                    case TargetPlatform.android:
                        {
                            long? reFocusNode = lastFocusNode;
                            await new Future(NavigatorLibrary._kAndroidRefocusingDelayDuration);
                            await SystemChannels.accessibility.send(new global::Doroti.Framework.Semantics.FocusSemanticEvent().toMap(nodeId: reFocusNode));
                            break;
                        }
                    case TargetPlatform.iOS:
                        {
                            await SystemChannels.accessibility.send(new global::Doroti.Framework.Semantics.FocusSemanticEvent().toMap(nodeId: lastFocusNode));
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            })).catchError((error, stackTrace) =>
            {
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stackTrace, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while restoring focus in the navigator")));
            }));
        }
    }

    public virtual bool handlePop(NavigatorState navigator, RouteBase? previousPresent)
    {
        DartRuntimePrimitives.Assert(() => navigator._debugLocked);
        DartRuntimePrimitives.Assert(() => route._isInstalledIn(navigator));
        currentState = _RouteLifecycle__navigator.popping;
        if (route.popCompleted)
        {
            DartRuntimePrimitives.Assert(() => pageBased);
            DartRuntimePrimitives.Assert(() => pendingResult is null);
            return true;
        }
        if (!route.didPopObject(pendingResult))
        {
            currentState = _RouteLifecycle__navigator.idle;
            return false;
        }
        route.onPopInvokedWithResultObject(true, pendingResult);
        if (pageBased && imperativeRemoval)
        {
            var page = ((Page<object?>?)route.settings)!;
            navigator.widget.onDidRemovePage?.Invoke(page);
        }
        pendingResult = null;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleComplete()
    {
        route.didCompleteObject(pendingResult);
        pendingResult = null;
        DartRuntimePrimitives.Assert(() => route.popCompleted);
        currentState = _RouteLifecycle__navigator.remove;
    }

    public virtual void handleRemoval(NavigatorState navigator, RouteBase? previousPresent)
    {
        DartRuntimePrimitives.Assert(() => navigator._debugLocked);
        if (route._isInstalledIn(navigator))
        {
            currentState = _RouteLifecycle__navigator.removing;
        }
        else
        {
            currentState = _RouteLifecycle__navigator.dispose;
        }
        if (_reportRemovalToObserver)
        {
            navigator._observedRouteDeletions.Enqueue(new _NavigatorRemoveObservation__navigator(route, previousPresent));
        }
    }

    public virtual void didAdd(NavigatorState navigator, bool isNewFirst)
    {
        DartRuntimePrimitives.Assert(() => !route._installed);
        route._navigator = navigator;
        route.install();
        DartRuntimePrimitives.Assert(() => Enumerable.Any(route.overlayEntries));
        route.didAdd();
        currentState = _RouteLifecycle__navigator.idle;
        if (isNewFirst)
        {
            route.didChangeNext(null);
        }
    }

    public virtual void pop<T>(T? result, bool imperativeRemoval)
    {
        DartRuntimePrimitives.Assert(() => isPresent);
        pendingResult = result;
        currentState = _RouteLifecycle__navigator.pop;
        this.imperativeRemoval = imperativeRemoval;
    }

    public virtual void complete<T>(T? result, bool isReplaced, bool imperativeRemoval)
    {
        if (FoundationRuntimePorts.EnumIndex(currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.remove))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => isPresent);
        _reportRemovalToObserver = !isReplaced;
        pendingResult = result;
        currentState = _RouteLifecycle__navigator.complete;
        this.imperativeRemoval = imperativeRemoval;
    }

    public virtual void finalize()
    {
        DartRuntimePrimitives.Assert(() => FoundationRuntimePorts.EnumIndex(currentState) < FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.dispose));
        currentState = _RouteLifecycle__navigator.dispose;
    }

    public virtual void forcedDispose()
    {
        DartRuntimePrimitives.Assert(() => FoundationRuntimePorts.EnumIndex(currentState) < FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.disposed));
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        currentState = _RouteLifecycle__navigator.disposed;
        route.dispose();
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => FoundationRuntimePorts.EnumIndex(currentState) < FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.disposing));
        currentState = _RouteLifecycle__navigator.disposing;
        IEnumerable<OverlayEntry> mountedEntries = route.overlayEntries.where((e) => e.mounted);
        if (!Enumerable.Any(mountedEntries))
        {
            forcedDispose();
            return;
        }
        long mountedLocal = mountedEntries.Count();
        DartRuntimePrimitives.Assert(() => mountedLocal > 0L);
        NavigatorState navigator = route._navigator!;
        navigator._entryWaitingForSubTreeDisposal.Add(this);
        foreach (var entry in mountedEntries)
        {
            global::System.Action listener = default!;
            listener = () =>
            {
                DartRuntimePrimitives.Assert(() => mountedLocal > 0L);
                DartRuntimePrimitives.Assert(() => !entry.mounted);
                mountedLocal--;
                entry.removeListener(listener);
                if (mountedLocal == 0L)
                {
                    DartRuntimePrimitives.Assert(() => route.overlayEntries.All((e) => !e.mounted));
                    DartAsyncRuntime.scheduleMicrotask(() =>
                    {
                        if (!navigator._entryWaitingForSubTreeDisposal.Remove(this))
                        {
                            DartRuntimePrimitives.Assert(() => !route._installed && !navigator.mounted);
                            return;
                        }
                        DartRuntimePrimitives.Assert(() => Equals(currentState, _RouteLifecycle__navigator.disposing));
                        forcedDispose();
                    });
                    return;
                }
            };
            entry.addListener(listener);
        }
    }

    public virtual bool willBePresent
    {
        get
        {
            return (FoundationRuntimePorts.EnumIndex(currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.idle)) && (FoundationRuntimePorts.EnumIndex(currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.add));
        }
    }
    public virtual bool isPresent
    {
        get
        {
            return (FoundationRuntimePorts.EnumIndex(currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.remove)) && (FoundationRuntimePorts.EnumIndex(currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.add));
        }
    }
    public virtual bool isPresentForRestoration => DartRuntimePrimitives.ConvertValue<bool>(FoundationRuntimePorts.EnumIndex(currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.idle));
    public virtual bool suitableForAnnouncement
    {
        get
        {
            return (FoundationRuntimePorts.EnumIndex(currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.removing)) && (FoundationRuntimePorts.EnumIndex(currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.push));
        }
    }
    public virtual bool suitableForTransitionAnimation
    {
        get
        {
            return (FoundationRuntimePorts.EnumIndex(currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.remove)) && (FoundationRuntimePorts.EnumIndex(currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.push));
        }
    }
    public virtual bool shouldAnnounceChangeToNext(object? nextRoute)
    {
        DartRuntimePrimitives.Assert(() => !Equals(nextRoute, lastAnnouncedNextRoute));
        return !((nextRoute is null) && Equals(DartCoreExtensions.weakTarget(lastAnnouncedPoppedNextRoute), lastAnnouncedNextRoute));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isPresentPredicate(_RouteEntry__navigator entry) => entry.isPresent;
    public static bool suitableForTransitionAnimationPredicate(_RouteEntry__navigator entry) => entry.suitableForTransitionAnimation;
    public static bool willBePresentPredicate(_RouteEntry__navigator entry) => entry.willBePresent;
    public static global::System.Func<_RouteEntry__navigator, bool> isRoutePredicate(RouteBase route)
    {
        return (entry) => Equals(entry.route, route);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isWaitingForEnteringDecision => DartRuntimePrimitives.ConvertValue<bool>(Equals(currentState, _RouteLifecycle__navigator.staging));
    public override bool isWaitingForExitingDecision => _isWaitingForExitingDecision;
    public virtual void markNeedsExitingDecision() => _isWaitingForExitingDecision = true;
    public override void markForPush()
    {
        DartRuntimePrimitives.Assert(() => isWaitingForEnteringDecision && !isWaitingForExitingDecision, () => (object?)"This route cannot be marked for push. Either a decision has already been " + "made or it does not require an explicit decision on how to transition in.");
        currentState = _RouteLifecycle__navigator.push;
    }

    public override void markForAdd()
    {
        DartRuntimePrimitives.Assert(() => isWaitingForEnteringDecision && !isWaitingForExitingDecision, () => (object?)"This route cannot be marked for add. Either a decision has already been " + "made or it does not require an explicit decision on how to transition in.");
        currentState = _RouteLifecycle__navigator.add;
    }

    public override void markForPop(dynamic? result = null)
    {
        DartRuntimePrimitives.Assert(() => !isWaitingForEnteringDecision && isWaitingForExitingDecision && isPresent, () => (object?)"This route cannot be marked for pop. Either a decision has already been " + "made or it does not require an explicit decision on how to transition out.");
        var attempt = 0L;
        while (route.willHandlePopInternally)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    attempt += 1L;
                    return attempt < kDebugPopAttemptLimit;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }, () => (object?)$"Attempted to pop {route} {kDebugPopAttemptLimit} times, but still failed");
            bool popResult = route.didPopObject((object?)result);
            DartRuntimePrimitives.Assert(() => !popResult);
        }
        pop<object>((object?)result, imperativeRemoval: false);
        _isWaitingForExitingDecision = false;
    }

    public override void markForComplete(dynamic? result = null)
    {
        DartRuntimePrimitives.Assert(() => !isWaitingForEnteringDecision && isWaitingForExitingDecision && isPresent, () => (object?)"This route cannot be marked for complete. Either a decision has already " + "been made or it does not require an explicit decision on how to transition " + "out.");
        complete<object>((object?)result, isReplaced: false, imperativeRemoval: false);
        _isWaitingForExitingDecision = false;
    }

    public virtual bool restorationEnabled
    {
        get => route.restorationScopeId.value is not null;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !__value || (restorationId is not null));
            route._updateRestorationId(__value ? restorationId : null);
        }
    }
}

internal abstract class _NavigatorObservation__navigator
{
    public virtual RouteBase primaryRoute { get; private set; } = default!;
    public virtual RouteBase? secondaryRoute { get; private set; }

    internal _NavigatorObservation__navigator(RouteBase primaryRoute, RouteBase? secondaryRoute)
    {
        this.primaryRoute = primaryRoute;
        this.secondaryRoute = secondaryRoute;
    }

    public abstract void notify(NavigatorObserver observer);
}

internal class _NavigatorPushObservation__navigator : _NavigatorObservation__navigator
{
    internal _NavigatorPushObservation__navigator(RouteBase primaryRoute, RouteBase? secondaryRoute) : base(primaryRoute, secondaryRoute)
    {
    }

    public override void notify(NavigatorObserver observer)
    {
        observer.didPush(primaryRoute, secondaryRoute);
    }

}

internal class _NavigatorPopObservation__navigator : _NavigatorObservation__navigator
{
    internal _NavigatorPopObservation__navigator(RouteBase primaryRoute, RouteBase? secondaryRoute) : base(primaryRoute, secondaryRoute)
    {
    }

    public override void notify(NavigatorObserver observer)
    {
        observer.didPop(primaryRoute, secondaryRoute);
    }

}

internal class _NavigatorRemoveObservation__navigator : _NavigatorObservation__navigator
{
    internal _NavigatorRemoveObservation__navigator(RouteBase primaryRoute, RouteBase? secondaryRoute) : base(primaryRoute, secondaryRoute)
    {
    }

    public override void notify(NavigatorObserver observer)
    {
        observer.didRemove(primaryRoute, secondaryRoute);
    }

}

internal class _NavigatorReplaceObservation__navigator : _NavigatorObservation__navigator
{
    internal _NavigatorReplaceObservation__navigator(RouteBase primaryRoute, RouteBase? secondaryRoute) : base(primaryRoute, secondaryRoute)
    {
    }

    public override void notify(NavigatorObserver observer)
    {
        observer.didReplace(newRoute: primaryRoute, oldRoute: secondaryRoute);
    }

}

internal delegate bool _IndexWhereCallback__navigator(_RouteEntry__navigator element);

public class _History__navigator : ChangeNotifier, IEnumerable<_RouteEntry__navigator>
{
    internal virtual List<_RouteEntry__navigator> _value { get; private set; } = new List<_RouteEntry__navigator>();

    internal _History__navigator()
    {
    }

    public virtual long indexWhere(global::System.Func<_RouteEntry__navigator, bool> test, long start = 0)
    {
        return _value.indexWhere(test, start);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void add(_RouteEntry__navigator element)
    {
        _value.Add(element);
        notifyListeners();
    }

    public virtual void addAll(IEnumerable<_RouteEntry__navigator> elements)
    {
        _value.AddRange(elements.Cast<_RouteEntry__navigator>());
        if (Enumerable.Any(elements))
        {
            notifyListeners();
        }
    }

    public virtual void clear()
    {
        bool valueWasEmpty = !Enumerable.Any(_value);
        _value.Clear();
        if (!valueWasEmpty)
        {
            notifyListeners();
        }
    }

    public virtual void insert(long index, _RouteEntry__navigator element)
    {
        _value.Insert(checked((int)index), element);
        notifyListeners();
    }

    public virtual _RouteEntry__navigator removeAt(long index)
    {
        _RouteEntry__navigator entry = _value.removeAt(index);
        notifyListeners();
        return entry;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _RouteEntry__navigator removeLast()
    {
        _RouteEntry__navigator entry = _value.removeLast<_RouteEntry__navigator>();
        notifyListeners();
        return entry;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public _RouteEntry__navigator this[long index]
    {
        get
        {
            return _value[(int)index];
        }
    }

    public virtual IEnumerator<_RouteEntry__navigator> GetEnumerator()
    {
        return _value.GetEnumerator();
    }
    public override string ToString()
    {
        return $"[{string.Join(", ", _value)}]";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

public class NavigatorState : State<Navigator>, TickerProviderStateMixin<Navigator>, RestorationMixin<Navigator>
{
    internal virtual GlobalKey<OverlayState> _overlayKey { get; set; } = default!;
    internal virtual _History__navigator _history { get; private set; } = new _History__navigator();
    internal virtual HashSet<_RouteEntry__navigator> _entryWaitingForSubTreeDisposal { get; private set; } = new HashSet<_RouteEntry__navigator>();
    internal virtual _HistoryProperty__navigator _serializableHistory { get; private set; } = new _HistoryProperty__navigator();
    internal virtual Queue<_NavigatorObservation__navigator> _observedRouteAdditions { get; private set; } = new Queue<_NavigatorObservation__navigator>();
    internal virtual Queue<_NavigatorObservation__navigator> _observedRouteDeletions { get; private set; } = new Queue<_NavigatorObservation__navigator>();
    public virtual FocusNode focusNode { get; private set; } = new FocusNode(debugLabel: "Navigator");
    internal virtual bool _debugLocked { get; set; } = false;
    internal virtual HeroController? _heroControllerFromScope { get; set; } = default;
    internal virtual List<NavigatorObserver> _effectiveObservers { get; set; } = default!;
    internal virtual RestorableNum<long> _rawNextPagelessRestorationScopeId { get; private set; } = new RestorableNum<long>(0L);
    internal virtual _RouteEntry__navigator? _lastTopmostRoute { get; set; } = default;
    internal virtual string? _lastAnnouncedRouteName { get; set; } = default;
    internal virtual bool _debugUpdatingPage { get; set; } = false;
    internal virtual bool _flushingHistory { get; set; } = false;
    internal virtual long _userGesturesInProgressCount { get; set; } = 0L;
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> userGestureInProgressNotifier { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<bool>(false);
    internal virtual HashSet<long> _activePointers { get; private set; } = new HashSet<long>();
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action> _properties { get; set; } = new DartMap<global::Doroti.Framework.Widgets.IRestorableProperty, global::System.Action>();
    public virtual List<global::Doroti.Framework.Widgets.IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    internal virtual bool _usingPagesAPI => !ReferenceEquals(widget.pages, Navigator._defaultPages);
    internal virtual void _handleHistoryChanged()
    {
        switch (Scheduler.SchedulerBinding.instance.schedulerPhase)
        {
            case Scheduler.SchedulerPhase.postFrameCallbacks:
                {
                    new NavigationNotification(canHandlePop: _getNavigatorCanHandlePop()).dispatch(context);
                    break;
                }
            case Scheduler.SchedulerPhase.idle:
            case Scheduler.SchedulerPhase.midFrameMicrotasks:
            case Scheduler.SchedulerPhase.persistentCallbacks:
            case Scheduler.SchedulerPhase.transientCallbacks:
                {
                    Scheduler.SchedulerBinding.instance.addPostFrameCallback((timeStamp) =>
                    {
                        if (!mounted)
                        {
                            return;
                        }
                        new NavigationNotification(canHandlePop: _getNavigatorCanHandlePop()).dispatch(context);
                    }, debugLabel: "Navigator.dispatchNotification");
                    break;
                }
        }
    }

    internal virtual bool _getNavigatorCanHandlePop()
    {
        if (canPop())
        {
            return true;
        }
        _RouteEntry__navigator? lastEntry = _lastRouteEntryWhereOrNull(_RouteEntry__navigator.isPresentPredicate);
        return (lastEntry is not null) && Equals(lastEntry.route.popDisposition, RoutePopDisposition.doNotPop);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckPageApiParameters()
    {
        if (!_usingPagesAPI)
        {
            return true;
        }
        if (!Enumerable.Any(widget.pages))
        {
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: FlutterError.Create("The Navigator.pages must not be empty to use the " + "Navigator.pages API"), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
        }
        else
        {
            if (widget.onDidRemovePage is null == widget.onPopPage is null)
            {
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: FlutterError.Create("Either onDidRemovePage or onPopPage must be provided to use the " + "Navigator.pages API but not both."), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        base.initState();
        DartRuntimePrimitives.Assert(() => _debugCheckPageApiParameters());
        foreach (NavigatorObserver observer in widget.observers)
        {
            DartRuntimePrimitives.Assert(() => observer.navigator is null);
            NavigatorObserver._navigators[observer] = this;
        }
        _effectiveObservers = widget.observers;
        var heroControllerScope = ((HeroControllerScope?)context.getElementForInheritedWidgetOfExactType<HeroControllerScope>()?.widget)!;
        _updateHeroController(heroControllerScope?.controller);
        if (widget.reportsRouteUpdateToEngine)
        {
            DartRuntimePrimitives.Ignore(SystemNavigator.selectSingleEntryHistory());
        }
        ServicesBinding.instance.accessibilityFocus.addListener(_recordLastFocus);
        _history.addListener(_handleHistoryChanged);
    }

    internal virtual void _recordLastFocus()
    {
        _RouteEntry__navigator? entry = _history.where(_RouteEntry__navigator.isPresentPredicate).LastOrDefault();
        entry?.lastFocusNode = ServicesBinding.instance.accessibilityFocus.value;
    }

    internal virtual long _nextPagelessRestorationScopeId => DartRuntimePrimitives.ConvertValue<long>(_rawNextPagelessRestorationScopeId.value++);
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(_rawNextPagelessRestorationScopeId, "id");
        registerForRestoration(_serializableHistory, "history");
        _forcedDisposeAllRouteEntries();
        DartRuntimePrimitives.Assert(() => !Enumerable.Any(_history));
        _overlayKey = new GlobalKey<OverlayState>();
        _history.addAll(_serializableHistory.restoreEntriesForPage(null, this));
        foreach (Page<object?> page in widget.pages)
        {
            var entry = new _RouteEntry__navigator(page.createRoute(context), pageBased: true, initialState: _RouteLifecycle__navigator.add);
            DartRuntimePrimitives.Assert(() => Equals(entry.route.settings, page), () => (object?)"The settings getter of a page-based Route must return a Page object. " + "Please set the settings to the Page in the Page.createRoute method.");
            _history.add(entry);
            _history.addAll(_serializableHistory.restoreEntriesForPage(entry, this));
        }
        if (!_serializableHistory.hasData)
        {
            string? initialRouteLocal = widget.initialRoute;
            if (!Enumerable.Any(widget.pages))
            {
                initialRouteLocal ??= Navigator.defaultRouteName;
            }
            if (initialRouteLocal is not null)
            {
                _history.addAll(widget.onGenerateInitialRoutes(this, widget.initialRoute ?? Navigator.defaultRouteName).map<dynamic, _RouteEntry__navigator>((route) =>
                {
                    RouteBase typedRoute = Navigator._requireRoute((object?)route);
                    return new _RouteEntry__navigator(typedRoute, pageBased: false, initialState: _RouteLifecycle__navigator.add, restorationInformation: (typedRoute.settings.ToString() is not null) ? _RestorationInformation__navigator.CreateNamed(name: typedRoute.settings.ToString()!, arguments: null, restorationScopeId: _nextPagelessRestorationScopeId) : null);
                }).Cast<_RouteEntry__navigator>());
            }
        }
        DartRuntimePrimitives.Assert(() => Enumerable.Any(_history), () => (object?)"All routes returned by onGenerateInitialRoutes are not restorable. " + "Please make sure that all routes returned by onGenerateInitialRoutes " + "have their RouteSettings defined with names that are defined in the " + "app's routes table.");
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _flushHistoryUpdates();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
        if (bucket is not null)
        {
            _serializableHistory.update(_history);
        }
        else
        {
            _serializableHistory.clear();
        }
    }

    public virtual string? restorationId => widget.restorationScopeId;
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
        _updateHeroController(HeroControllerScope.maybeOf(context));
        foreach (_RouteEntry__navigator entry in _history)
        {
            if (Equals(entry.route.navigator, this))
            {
                entry.route.changedExternalState();
            }
        }
    }

    internal virtual void _forcedDisposeAllRouteEntries()
    {
        _entryWaitingForSubTreeDisposal.removeWhere((entry) =>
        {
            entry.forcedDispose();
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        while (Enumerable.Any(_history))
        {
            _disposeRouteEntry(_history.removeLast(), graceful: false);
        }
    }

    internal static void _disposeRouteEntry(_RouteEntry__navigator entry, bool graceful)
    {
        foreach (OverlayEntry overlayEntry in entry.route.overlayEntries)
        {
            if (overlayEntry._overlay is not null)
            {
                overlayEntry.remove();
            }
        }
        if (graceful)
        {
            entry.dispose();
        }
        else
        {
            entry.forcedDispose();
        }
    }

    internal virtual void _updateHeroController(HeroController? newHeroController)
    {
        if (!Equals(_heroControllerFromScope, newHeroController))
        {
            if (newHeroController is not null)
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        if (newHeroController.navigator is not null)
                        {
                            NavigatorState previousOwner = newHeroController.navigator!;
                            ServicesBinding.instance.addPostFrameCallback((timestamp) =>
                            {
                                if (Equals(_heroControllerFromScope, newHeroController))
                                {
                                    var hasHeroControllerOwnerShip = Equals(_heroControllerFromScope!.navigator, this);
                                    if (!hasHeroControllerOwnerShip || Equals(previousOwner._heroControllerFromScope, newHeroController))
                                    {
                                        NavigatorState otherOwner = hasHeroControllerOwnerShip ? previousOwner : _heroControllerFromScope!.navigator!;
                                        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: FlutterError.Create("A HeroController can not be shared by multiple Navigators. " + "The Navigators that share the same HeroController are:\n" + $"- {this}\n" + $"- {otherOwner}\n" + "Please create a HeroControllerScope for each Navigator or " + "use a HeroControllerScope.none to prevent subtree from " + "receiving a HeroController."), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
                                    }
                                }
                            }, debugLabel: "Navigator.checkHeroControllerOwnership");
                        }
                        return true;
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                NavigatorObserver._navigators[newHeroController] = this;
            }
            if (Equals(_heroControllerFromScope?.navigator, this))
            {
                NavigatorObserver._navigators[_heroControllerFromScope!] = DartRuntimePrimitives.ConvertValue<NavigatorState>(null);
            }
            _heroControllerFromScope = newHeroController;
            _updateEffectiveObservers();
        }
    }

    internal virtual void _updateEffectiveObservers()
    {
        if (_heroControllerFromScope is not null)
        {
            _effectiveObservers = widget.observers.Concat(new List<NavigatorObserver> { _heroControllerFromScope! }).ToList();
        }
        else
        {
            _effectiveObservers = widget.observers;
        }
    }

    public override void didUpdateWidget(Navigator oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        DartRuntimePrimitives.Assert(() => _debugCheckPageApiParameters());
        if (!Equals(oldWidget.observers, widget.observers))
        {
            foreach (NavigatorObserver observer in oldWidget.observers)
            {
                NavigatorObserver._navigators[observer] = DartRuntimePrimitives.ConvertValue<NavigatorState>(null);
            }
            foreach (NavigatorObserver observerLocal in widget.observers)
            {
                DartRuntimePrimitives.Assert(() => observerLocal.navigator is null);
                NavigatorObserver._navigators[observerLocal] = this;
            }
            _updateEffectiveObservers();
        }
        if ((!Equals(oldWidget.pages, widget.pages)) && !restorePending)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (!Enumerable.Any(widget.pages))
                    {
                        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: FlutterError.Create("The Navigator.pages must not be empty to use the " + "Navigator.pages API"), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            _updatePages();
        }
        foreach (_RouteEntry__navigator entry in _history)
        {
            if (Equals(entry.route.navigator, this))
            {
                entry.route.changedExternalState();
            }
        }
    }

    internal virtual void _debugCheckDuplicatedPageKeys()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var keyReservation = new HashSet<global::Doroti.Framework.Foundation.Key>();
                foreach (Page<object?> page in widget.pages)
                {
                    global::Doroti.Framework.Foundation.LocalKey? keyLocal = page.key;
                    if (keyLocal is not null)
                    {
                        DartRuntimePrimitives.Assert(() => !keyReservation.Contains(keyLocal));
                        keyReservation.Add(keyLocal);
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override void deactivate()
    {
        foreach (NavigatorObserver observer in _effectiveObservers)
        {
            NavigatorObserver._navigators[observer] = DartRuntimePrimitives.ConvertValue<NavigatorState>(null);
        }
        _effectiveObservers = new List<NavigatorObserver>();
        base.deactivate();
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
        _updateEffectiveObservers();
        foreach (NavigatorObserver observer in _effectiveObservers)
        {
            DartRuntimePrimitives.Assert(() => observer.navigator is null);
            NavigatorObserver._navigators[observer] = this;
        }
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => !Enumerable.Any(_effectiveObservers));
        _updateHeroController(null);
        focusNode.dispose();
        _forcedDisposeAllRouteEntries();
        _rawNextPagelessRestorationScopeId.dispose();
        _serializableHistory.dispose();
        userGestureInProgressNotifier.dispose();
        ServicesBinding.instance.accessibilityFocus.removeListener(_recordLastFocus);
        _history.removeListener(_handleHistoryChanged);
        _history.dispose();
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
        DartRuntimePrimitives.Assert(() => _debugLocked);
    }

    public virtual OverlayState? overlay => _overlayKey.currentState;
    internal virtual IEnumerable<OverlayEntry> _allRouteOverlayEntries
    {
        get
        {
            return _history
                .where(_RouteEntry__navigator.isPresentPredicate)
                .SelectMany(entry => entry.route.overlayEntries);
        }
    }
    internal virtual void _updatePages()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => !_debugUpdatingPage);
                _debugCheckDuplicatedPageKeys();
                _debugUpdatingPage = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        var needsExplicitDecision = false;
        var newPagesBottom = 0L;
        var oldEntriesBottom = 0L;
        long newPagesTop = checked(widget.pages.Count) - 1L;
        long oldEntriesTop = _history.Count() - 1L;
        var newHistory = new List<_RouteEntry__navigator>();
        var pageRouteToPagelessRoutesLocal = new DartMap<_RouteEntry__navigator?, List<_RouteEntry__navigator>>();
        _RouteEntry__navigator? previousOldPageRouteEntry = default!;
        while (oldEntriesBottom <= oldEntriesTop)
        {
            _RouteEntry__navigator oldEntry = _history[oldEntriesBottom];
            DartRuntimePrimitives.Assert(() => !Equals(oldEntry.currentState, _RouteLifecycle__navigator.disposed));
            if (!oldEntry.pageBased)
            {
                List<_RouteEntry__navigator> pagelessRoutes = pageRouteToPagelessRoutesLocal.putIfAbsent(previousOldPageRouteEntry, () => new List<_RouteEntry__navigator>()).ToList();
                pagelessRoutes.Add(oldEntry);
                oldEntriesBottom += 1L;
                continue;
            }
            if (newPagesBottom > newPagesTop)
            {
                break;
            }
            Page<object?> newPage = widget.pages[(int)newPagesBottom];
            if (!oldEntry.canUpdateFrom(newPage))
            {
                break;
            }
            previousOldPageRouteEntry = oldEntry;
            oldEntry.route._updateSettings(newPage);
            newHistory.Add(oldEntry);
            newPagesBottom += 1L;
            oldEntriesBottom += 1L;
        }
        var unattachedPagelessRoutes = new List<_RouteEntry__navigator>();
        while (oldEntriesBottom <= oldEntriesTop && newPagesBottom <= newPagesTop)
        {
            _RouteEntry__navigator oldEntryLocal = _history[oldEntriesTop];
            DartRuntimePrimitives.Assert(() => !Equals(oldEntryLocal.currentState, _RouteLifecycle__navigator.disposed));
            if (!oldEntryLocal.pageBased)
            {
                unattachedPagelessRoutes.Add(oldEntryLocal);
                oldEntriesTop -= 1L;
                continue;
            }
            Page<object?> newPageLocal = widget.pages[(int)newPagesTop];
            if (!oldEntryLocal.canUpdateFrom(newPageLocal))
            {
                break;
            }
            if (Enumerable.Any(unattachedPagelessRoutes))
            {
                pageRouteToPagelessRoutesLocal.putIfAbsent(oldEntryLocal, () => new List<_RouteEntry__navigator>(DartRuntimePrimitives.ConvertEnumerable<_RouteEntry__navigator>(unattachedPagelessRoutes)));
                unattachedPagelessRoutes.Clear();
            }
            oldEntriesTop -= 1L;
            newPagesTop -= 1L;
        }
        oldEntriesTop += checked(unattachedPagelessRoutes.Count);
        var oldEntriesBottomToScan = oldEntriesBottom;
        var pageKeyToOldEntry = new DartMap<global::Doroti.Framework.Foundation.LocalKey, _RouteEntry__navigator>();
        var phantomEntries = new HashSet<_RouteEntry__navigator>();
        while (oldEntriesBottomToScan <= oldEntriesTop)
        {
            _RouteEntry__navigator oldEntryAlternate = _history[oldEntriesBottomToScan];
            oldEntriesBottomToScan += 1L;
            DartRuntimePrimitives.Assert(() => !Equals(oldEntryAlternate.currentState, _RouteLifecycle__navigator.disposed));
            if (!oldEntryAlternate.pageBased)
            {
                continue;
            }
            var page = ((Page<object?>?)oldEntryAlternate.route.settings)!;
            if (page.key is null)
            {
                continue;
            }
            if (!oldEntryAlternate.willBePresent)
            {
                phantomEntries.Add(oldEntryAlternate);
                continue;
            }
            DartRuntimePrimitives.Assert(() => !pageKeyToOldEntry.ContainsKey(page.key));
            pageKeyToOldEntry[page.key!] = oldEntryAlternate;
        }
        while (newPagesBottom <= newPagesTop)
        {
            Page<object?> nextPage = widget.pages[(int)newPagesBottom];
            newPagesBottom += 1L;
            if ((nextPage.key is null) || !pageKeyToOldEntry.ContainsKey(nextPage.key) || !pageKeyToOldEntry.GetValueOrDefault(DartRuntimePrimitives.RequireReference(nextPage.key))!.canUpdateFrom(nextPage))
            {
                var newEntry = new _RouteEntry__navigator(nextPage.createRoute(context), pageBased: true, initialState: _RouteLifecycle__navigator.staging);
                needsExplicitDecision = true;
                DartRuntimePrimitives.Assert(() => Equals(newEntry.route.settings, nextPage), () => (object?)"The settings getter of a page-based Route must return a Page object. " + "Please set the settings to the Page in the Page.createRoute method.");
                newHistory.Add(newEntry);
            }
            else
            {
                _RouteEntry__navigator matchingEntry = pageKeyToOldEntry.remove(nextPage.key)!;
                DartRuntimePrimitives.Assert(() => matchingEntry.canUpdateFrom(nextPage));
                matchingEntry.route._updateSettings(nextPage);
                newHistory.Add(matchingEntry);
            }
        }
        var locationToExitingPageRouteLocal = new DartMap<RouteTransitionRecord?, RouteTransitionRecord>();
        while (oldEntriesBottom <= oldEntriesTop)
        {
            _RouteEntry__navigator potentialEntryToRemove = _history[oldEntriesBottom];
            oldEntriesBottom += 1L;
            if (!potentialEntryToRemove.pageBased)
            {
                DartRuntimePrimitives.Assert(() => previousOldPageRouteEntry is not null);
                List<_RouteEntry__navigator> pagelessRoutesLocal = pageRouteToPagelessRoutesLocal.putIfAbsent(previousOldPageRouteEntry, () => new List<_RouteEntry__navigator>()).ToList();
                pagelessRoutesLocal.Add(potentialEntryToRemove);
                if (previousOldPageRouteEntry!.isWaitingForExitingDecision && potentialEntryToRemove.willBePresent)
                {
                    potentialEntryToRemove.markNeedsExitingDecision();
                }
                continue;
            }
            var potentialPageToRemove = ((Page<object?>?)potentialEntryToRemove.route.settings)!;
            if ((potentialPageToRemove.key is null) || pageKeyToOldEntry.ContainsKey(potentialPageToRemove.key) || phantomEntries.Contains(potentialEntryToRemove))
            {
                locationToExitingPageRouteLocal[DartRuntimePrimitives.RequireReference(previousOldPageRouteEntry)] = DartRuntimePrimitives.ConvertValue<RouteTransitionRecord>(potentialEntryToRemove);
                if (potentialEntryToRemove.willBePresent)
                {
                    potentialEntryToRemove.markNeedsExitingDecision();
                }
            }
            previousOldPageRouteEntry = potentialEntryToRemove;
        }
        DartRuntimePrimitives.Assert(() => oldEntriesBottom == (oldEntriesTop + 1L));
        DartRuntimePrimitives.Assert(() => newPagesBottom == (newPagesTop + 1L));
        newPagesTop = checked(widget.pages.Count) - 1L;
        oldEntriesTop = _history.Count() - 1L;
        DartRuntimePrimitives.Assert(() =>
            {
                if (oldEntriesBottom <= oldEntriesTop)
                {
                    return (newPagesBottom <= newPagesTop) && _history[oldEntriesBottom].pageBased && _history[oldEntriesBottom].canUpdateFrom(widget.pages[(int)newPagesBottom]);
                }
                else
                {
                    return newPagesBottom > newPagesTop;
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        while (oldEntriesBottom <= oldEntriesTop && newPagesBottom <= newPagesTop)
        {
            _RouteEntry__navigator oldEntryNested = _history[oldEntriesBottom];
            DartRuntimePrimitives.Assert(() => !Equals(oldEntryNested.currentState, _RouteLifecycle__navigator.disposed));
            if (!oldEntryNested.pageBased)
            {
                DartRuntimePrimitives.Assert(() => previousOldPageRouteEntry is not null);
                List<_RouteEntry__navigator> pagelessRoutesAlternate = pageRouteToPagelessRoutesLocal.putIfAbsent(previousOldPageRouteEntry, () => new List<_RouteEntry__navigator>()).ToList();
                pagelessRoutesAlternate.Add(oldEntryNested);
                continue;
            }
            previousOldPageRouteEntry = oldEntryNested;
            Page<object?> newPageAlternate = widget.pages[(int)newPagesBottom];
            DartRuntimePrimitives.Assert(() => oldEntryNested.canUpdateFrom(newPageAlternate));
            oldEntryNested.route._updateSettings(newPageAlternate);
            newHistory.Add(oldEntryNested);
            oldEntriesBottom += 1L;
            newPagesBottom += 1L;
        }
        needsExplicitDecision = needsExplicitDecision || Enumerable.Any(locationToExitingPageRouteLocal);
        IEnumerable<_RouteEntry__navigator> results = newHistory;
        if (needsExplicitDecision)
        {
            results = widget.transitionDelegate._transition(newPageRouteHistory: newHistory.Cast<RouteTransitionRecord>().ToList(), locationToExitingPageRoute: locationToExitingPageRouteLocal, pageRouteToPagelessRoutes: pageRouteToPagelessRoutesLocal.cast<RouteTransitionRecord?, List<RouteTransitionRecord>>()).cast<_RouteEntry__navigator>();
        }
        _history.clear();
        if (pageRouteToPagelessRoutesLocal.ContainsKey(null))
        {
            _history.addAll(pageRouteToPagelessRoutesLocal.GetValueOrDefault(null)!.Cast<_RouteEntry__navigator>());
        }
        foreach (var result in results)
        {
            _history.add(result);
            if (pageRouteToPagelessRoutesLocal.ContainsKey(result))
            {
                _history.addAll(pageRouteToPagelessRoutesLocal.GetValueOrDefault(result)!.Cast<_RouteEntry__navigator>());
            }
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugUpdatingPage = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _flushHistoryUpdates();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual void _flushHistoryUpdates(bool rearrangeOverlay = true)
    {
        DartRuntimePrimitives.Assert(() => _debugLocked && !_debugUpdatingPage);
        _flushingHistory = true;
        long index = _history.Count() - 1L;
        _RouteEntry__navigator? next = default!;
        _RouteEntry__navigator? entry = _history[index];
        _RouteEntry__navigator? previousLocal = (index > 0L) ? _history[index - 1L] : null;
        var canRemoveOrAdd = false;
        RouteBase? poppedRoute = default;
        var seenTopActiveRoute = false;
        var toBeDisposed = new List<_RouteEntry__navigator>();
        while (index >= 0L)
        {
            switch (entry!.currentState)
            {
                case _RouteLifecycle__navigator.add:
                    {
                        DartRuntimePrimitives.Assert(() => rearrangeOverlay);
                        entry.handleAdd(navigator: this, previousPresent: _getRouteBefore(index - 1L, _RouteEntry__navigator.isPresentPredicate)?.route);
                        DartRuntimePrimitives.Assert(() => Equals(entry.currentState, _RouteLifecycle__navigator.adding));
                        continue;
                    }
                case _RouteLifecycle__navigator.adding:
                    {
                        if (canRemoveOrAdd || (next is null))
                        {
                            entry.didAdd(navigator: this, isNewFirst: next is null);
                            DartRuntimePrimitives.Assert(() => Equals(entry.currentState, _RouteLifecycle__navigator.idle));
                            continue;
                        }
                        break;
                    }
                case _RouteLifecycle__navigator.push:
                case _RouteLifecycle__navigator.pushReplace:
                case _RouteLifecycle__navigator.replace:
                    {
                        DartRuntimePrimitives.Assert(() => rearrangeOverlay);
                        entry.handlePush(navigator: this, previous: previousLocal?.route, previousPresent: _getRouteBefore(index - 1L, _RouteEntry__navigator.isPresentPredicate)?.route, isNewFirst: next is null);
                        DartRuntimePrimitives.Assert(() => !Equals(entry.currentState, _RouteLifecycle__navigator.push));
                        DartRuntimePrimitives.Assert(() => !Equals(entry.currentState, _RouteLifecycle__navigator.pushReplace));
                        DartRuntimePrimitives.Assert(() => !Equals(entry.currentState, _RouteLifecycle__navigator.replace));
                        if (Equals(entry.currentState, _RouteLifecycle__navigator.idle))
                        {
                            continue;
                        }
                        break;
                    }
                case _RouteLifecycle__navigator.pushing:
                    {
                        if (!seenTopActiveRoute && (poppedRoute is not null))
                        {
                            entry.handleDidPopNext(poppedRoute);
                        }
                        seenTopActiveRoute = true;
                        break;
                    }
                case _RouteLifecycle__navigator.idle:
                    {
                        if (!seenTopActiveRoute && (poppedRoute is not null))
                        {
                            entry.handleDidPopNext(poppedRoute);
                        }
                        seenTopActiveRoute = true;
                        canRemoveOrAdd = true;
                        break;
                    }
                case _RouteLifecycle__navigator.pop:
                    {
                        if (!entry.handlePop(navigator: this, previousPresent: _getRouteBefore(index, _RouteEntry__navigator.willBePresentPredicate)?.route))
                        {
                            DartRuntimePrimitives.Assert(() => Equals(entry.currentState, _RouteLifecycle__navigator.idle));
                            continue;
                        }
                        if (!seenTopActiveRoute)
                        {
                            if (poppedRoute is not null)
                            {
                                entry.handleDidPopNext(poppedRoute);
                            }
                            poppedRoute = entry.route;
                        }
                        _observedRouteDeletions.Enqueue(new _NavigatorPopObservation__navigator(entry.route, _getRouteBefore(index, _RouteEntry__navigator.willBePresentPredicate)?.route));
                        if (Equals(entry.currentState, _RouteLifecycle__navigator.dispose))
                        {
                            continue;
                        }
                        DartRuntimePrimitives.Assert(() => Equals(entry.currentState, _RouteLifecycle__navigator.popping));
                        canRemoveOrAdd = true;
                        break;
                    }
                case _RouteLifecycle__navigator.popping:
                    {
                        break;
                    }
                case _RouteLifecycle__navigator.complete:
                    {
                        entry.handleComplete();
                        DartRuntimePrimitives.Assert(() => Equals(entry.currentState, _RouteLifecycle__navigator.remove));
                        continue;
                    }
                case _RouteLifecycle__navigator.remove:
                    {
                        if (!seenTopActiveRoute && entry.route._installed)
                        {
                            if (poppedRoute is not null)
                            {
                                entry.handleDidPopNext(poppedRoute);
                            }
                            poppedRoute = null;
                        }
                        entry.handleRemoval(navigator: this, previousPresent: _getRouteBefore(index, _RouteEntry__navigator.willBePresentPredicate)?.route);
                        DartRuntimePrimitives.Assert(() => FoundationRuntimePorts.EnumIndex(entry.currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.removing));
                        continue;
                    }
                case _RouteLifecycle__navigator.removing:
                    {
                        if (!canRemoveOrAdd && (next is not null))
                        {
                            break;
                        }
                        entry.currentState = _RouteLifecycle__navigator.dispose;
                        continue;
                    }
                case _RouteLifecycle__navigator.dispose:
                    {
                        toBeDisposed.Add(_history.removeAt(index));
                        entry = next;
                        break;
                    }
                case _RouteLifecycle__navigator.disposing:
                case _RouteLifecycle__navigator.disposed:
                case _RouteLifecycle__navigator.staging:
                    {
                        DartRuntimePrimitives.Assert(() => false);
                        break;
                    }
            }
            index -= 1L;
            next = entry;
            entry = previousLocal;
            previousLocal = (index > 0L) ? _history[index - 1L] : null;
        }
        _flushObserverNotifications();
        _flushRouteAnnouncement();
        _RouteEntry__navigator? lastEntry = _lastRouteEntryWhereOrNull(_RouteEntry__navigator.isPresentPredicate);
        if ((lastEntry is not null) && (!Equals(_lastTopmostRoute, lastEntry)))
        {
            foreach (NavigatorObserver observer in _effectiveObservers)
            {
                observer.didChangeTop(lastEntry.route, _lastTopmostRoute?.route);
            }
        }
        _lastTopmostRoute = lastEntry;
        if (widget.reportsRouteUpdateToEngine)
        {
            string? routeName = lastEntry?.route.settings.ToString();
            if ((routeName is not null) && (routeName != _lastAnnouncedRouteName))
            {
                DartRuntimePrimitives.Ignore(SystemNavigator.routeInformationUpdated(uri: DartUri.parse(routeName)));
                _lastAnnouncedRouteName = routeName;
            }
        }
        foreach (var entryLocal in toBeDisposed)
        {
            _disposeRouteEntry(entryLocal, graceful: true);
        }
        if (rearrangeOverlay)
        {
            overlay?.rearrange(_allRouteOverlayEntries.Cast<OverlayEntry>());
        }
        if (bucket is not null)
        {
            _serializableHistory.update(_history);
        }
        _flushingHistory = false;
    }

    internal virtual void _flushObserverNotifications()
    {
        if (!Enumerable.Any(_effectiveObservers))
        {
            _observedRouteDeletions.Clear();
            _observedRouteAdditions.Clear();
            return;
        }
        while (Enumerable.Any(_observedRouteAdditions))
        {
            _NavigatorObservation__navigator observation = _observedRouteAdditions.removeLast<_NavigatorObservation__navigator>();
            _effectiveObservers.forEach((__arg0) => ((global::System.Action<NavigatorObserver>)observation.notify)(__arg0));
        }
        while (Enumerable.Any(_observedRouteDeletions))
        {
            _NavigatorObservation__navigator observationLocal = _observedRouteDeletions.Dequeue();
            _effectiveObservers.forEach((__arg0) => ((global::System.Action<NavigatorObserver>)observationLocal.notify)(__arg0));
        }
    }

    internal virtual void _flushRouteAnnouncement()
    {
        long index = _history.Count() - 1L;
        while (index >= 0L)
        {
            _RouteEntry__navigator entry = _history[index];
            if (!entry.suitableForAnnouncement)
            {
                index -= 1L;
                continue;
            }
            _RouteEntry__navigator? next = _getRouteAfter(index + 1L, _RouteEntry__navigator.suitableForTransitionAnimationPredicate);
            if (!Equals(next?.route, entry.lastAnnouncedNextRoute))
            {
                if (entry.shouldAnnounceChangeToNext(next?.route))
                {
                    entry.route.didChangeNext(next?.route);
                }
                entry.lastAnnouncedNextRoute = next?.route;
            }
            _RouteEntry__navigator? previous = _getRouteBefore(index - 1L, _RouteEntry__navigator.suitableForTransitionAnimationPredicate);
            if (!Equals(previous?.route, entry.lastAnnouncedPreviousRoute))
            {
                entry.route.didChangePrevious(previous?.route);
                entry.lastAnnouncedPreviousRoute = previous?.route;
            }
            index -= 1L;
        }
    }

    internal virtual _RouteEntry__navigator? _getRouteBefore(long index, global::System.Func<_RouteEntry__navigator, bool> predicate)
    {
        index = _getIndexBefore(index, predicate);
        return (index >= 0L) ? _history[index] : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _getIndexBefore(long index, global::System.Func<_RouteEntry__navigator, bool> predicate)
    {
        while ((index >= 0L) && !predicate(_history[index]))
        {
            index -= 1L;
        }
        return index;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _RouteEntry__navigator? _getRouteAfter(long index, global::System.Func<_RouteEntry__navigator, bool> predicate)
    {
        while ((index < _history.Count()) && !predicate(_history[index]))
        {
            index += 1L;
        }
        return (index < _history.Count()) ? _history[index] : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Route<T?>? _routeNamed<T>(string name, object? arguments, bool allowNull = false)
    {
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        if (allowNull && (widget.onGenerateRoute is null))
        {
            return default;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (widget.onGenerateRoute is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create($"Navigator.onGenerateRoute was null, but the route named \"{name}\" was referenced.\n" + "To use the Navigator API with named routes (pushNamed, pushReplacementNamed, or " + "pushNamedAndRemoveUntil), the Navigator must be provided with an " + "onGenerateRoute handler.\n" + "The Navigator was:\n" + $"  {this}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        var settings = new RouteSettings(name: name, arguments: arguments);
        var route = ((Route<T?>?)(object?)widget.onGenerateRoute!(settings))!;
        if ((route is null) && !allowNull)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (widget.onUnknownRoute is null)
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"Navigator.onGenerateRoute returned null when requested to build route \"{name}\"."), new global::Doroti.Framework.Foundation.ErrorDescription("The onGenerateRoute callback must never return null, unless an onUnknownRoute " + "callback is provided as well."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<NavigatorState>("The Navigator was", this, style: DiagnosticsTreeStyle.errorProperty) }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            route = ((Route<T?>?)(object?)widget.onUnknownRoute!(settings))!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (route is null)
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"Navigator.onUnknownRoute returned null when requested to build route \"{name}\"."), new global::Doroti.Framework.Foundation.ErrorDescription("The onUnknownRoute callback must never return null."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<NavigatorState>("The Navigator was", this, style: DiagnosticsTreeStyle.errorProperty) }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        DartRuntimePrimitives.Assert(() => (route is not null) || allowNull);
        return route;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T?> pushNamed<T>(string routeName, object? arguments = null)
    {
        return push<T?>(_routeNamed<T>(routeName, arguments: arguments)!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePushNamed<T>(string routeName, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = _RestorationInformation__navigator.CreateNamed(name: routeName, arguments: arguments, restorationScopeId: _nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.push);
        _pushEntry(entry);
        return entry.restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T?> pushReplacementNamed<T, TO>(string routeName, TO? result = default, object? arguments = null)
    {
        return pushReplacement<T?, TO>(_routeNamed<T>(routeName, arguments: arguments)!, result: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePushReplacementNamed<T, TO>(string routeName, TO? result = default, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = _RestorationInformation__navigator.CreateNamed(name: routeName, arguments: arguments, restorationScopeId: _nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.pushReplace);
        _pushReplacementEntry(entry, result);
        return entry.restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T?> popAndPushNamed<T, TO>(string routeName, TO? result = default, object? arguments = null)
    {
        pop<TO>(result);
        return pushNamed<T>(routeName, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePopAndPushNamed<T, TO>(string routeName, TO? result = default, object? arguments = null)
    {
        pop<TO>(result);
        return restorablePushNamed<object>(routeName, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T?> pushNamedAndRemoveUntil<T>(string newRouteName, global::System.Func<dynamic, bool> predicate, object? arguments = null)
    {
        return pushAndRemoveUntil<T?>(_routeNamed<T>(newRouteName, arguments: arguments)!, predicate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePushNamedAndRemoveUntil<T>(string newRouteName, global::System.Func<dynamic, bool> predicate, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = _RestorationInformation__navigator.CreateNamed(name: newRouteName, arguments: arguments, restorationScopeId: _nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.push);
        _pushEntryAndRemoveUntil(entry, predicate);
        return entry.restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T?> push<T>(Route<T> route)
    {
        _pushEntry(new _RouteEntry__navigator(route, pageBased: false, initialState: _RouteLifecycle__navigator.push));
        return route.popped;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugIsStaticCallback(Delegate callback)
    {
        var result = false;
        DartRuntimePrimitives.Assert(() =>
            {
                result = Foundation.ConstantsLibrary.kIsWeb || (Dart_uiLibrary.PluginUtilities.getCallbackHandle(callback) is not null);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return DartRuntimePrimitives.RequireValue(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePush<T>(global::System.Func<BuildContext, object?, Route<T>> routeBuilder, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback(routeBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = _RestorationInformation__navigator.CreateAnonymous(routeBuilder: routeBuilder, arguments: arguments, restorationScopeId: _nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.push);
        _pushEntry(entry);
        return entry.restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _pushEntry(_RouteEntry__navigator entry)
    {
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => !entry.route._installed);
        DartRuntimePrimitives.Assert(() => Equals(entry.currentState, _RouteLifecycle__navigator.push));
        _history.add(entry);
        _flushHistoryUpdates();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _afterNavigation(entry.route);
    }

    internal virtual void _afterNavigation(RouteBase? route)
    {
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            DartMap<string, object>? routeJsonable = default!;
            if (route is not null)
            {
                routeJsonable = new DartMap<string, object>();
                string description = default!;
                if (route is ITransitionRoute transitionRoute)
                {
                    description = transitionRoute.debugLabel;
                }
                else
                {
                    description = $"{route}";
                }
                routeJsonable["description"] = description;
                RouteSettings settingsLocal = route.settings;
                var settingsJsonable = new DartMap<string, object?> { ["name"] = settingsLocal.name };
                if (settingsLocal.arguments is not null)
                {
                    settingsJsonable["arguments"] = Dart_convertLibrary.jsonEncode(settingsLocal.arguments, toEncodable: (@object) => $"{@object}");
                }
                routeJsonable["settings"] = settingsJsonable;
            }
            Dart_developerLibrary.postEvent("Flutter.Navigation", new DartMap<string, object> { ["route"] = routeJsonable });
        }
        _cancelActivePointers();
    }

    public virtual Future<T?> pushReplacement<T, TO>(Route<T> newRoute, TO? result = default)
    {
        DartRuntimePrimitives.Assert(() => !newRoute._installed);
        _pushReplacementEntry(new _RouteEntry__navigator(newRoute, pageBased: false, initialState: _RouteLifecycle__navigator.pushReplace), result);
        return newRoute.popped;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePushReplacement<T, TO>(global::System.Func<BuildContext, object?, Route<T>> routeBuilder, TO? result = default, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback(routeBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = _RestorationInformation__navigator.CreateAnonymous(routeBuilder: routeBuilder, arguments: arguments, restorationScopeId: _nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.pushReplace);
        _pushReplacementEntry(entry, result);
        return entry.restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _pushReplacementEntry<TO>(_RouteEntry__navigator entry, TO? result)
    {
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => !entry.route._installed);
        DartRuntimePrimitives.Assert(() => Enumerable.Any(_history));
        DartRuntimePrimitives.Assert(() => _history.any(__item => _RouteEntry__navigator.isPresentPredicate(__item)), () => (object?)"Navigator has no active routes to replace.");
        DartRuntimePrimitives.Assert(() => Equals(entry.currentState, _RouteLifecycle__navigator.pushReplace));
        _history.lastWhere(_RouteEntry__navigator.isPresentPredicate).complete(result, isReplaced: true, imperativeRemoval: true);
        _history.add(entry);
        _flushHistoryUpdates();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _afterNavigation(entry.route);
    }

    public virtual Future<T?> pushAndRemoveUntil<T>(Route<T> newRoute, global::System.Func<dynamic, bool> predicate)
    {
        DartRuntimePrimitives.Assert(() => !newRoute._installed);
        DartRuntimePrimitives.Assert(() => !Enumerable.Any(newRoute.overlayEntries));
        _pushEntryAndRemoveUntil(new _RouteEntry__navigator(newRoute, pageBased: false, initialState: _RouteLifecycle__navigator.push), predicate);
        return newRoute.popped;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePushAndRemoveUntil<T>(global::System.Func<BuildContext, object?, Route<T>> newRouteBuilder, global::System.Func<dynamic, bool> predicate, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback(newRouteBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = _RestorationInformation__navigator.CreateAnonymous(routeBuilder: newRouteBuilder, arguments: arguments, restorationScopeId: _nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.push);
        _pushEntryAndRemoveUntil(entry, predicate);
        return entry.restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _pushEntryAndRemoveUntil(_RouteEntry__navigator entry, global::System.Func<dynamic, bool> predicate)
    {
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => !entry.route._installed);
        DartRuntimePrimitives.Assert(() => !Enumerable.Any(entry.route.overlayEntries));
        DartRuntimePrimitives.Assert(() => Equals(entry.currentState, _RouteLifecycle__navigator.push));
        long index = _history.Count() - 1L;
        _history.add(entry);
        while ((index >= 0L) && !predicate(_history[index].route))
        {
            if (_history[index].isPresent)
            {
                _history[index].complete((Navigator?)null, isReplaced: false, imperativeRemoval: true);
            }
            index -= 1L;
        }
        _flushHistoryUpdates();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _afterNavigation(entry.route);
    }

    public virtual void replace<T>(dynamic oldRoute, Route<T> newRoute)
    {
        RouteBase typedOldRoute = Navigator._requireRoute((object?)oldRoute);
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        DartRuntimePrimitives.Assert(() => typedOldRoute._isInstalledIn(this));
        _replaceEntry(new _RouteEntry__navigator(newRoute, pageBased: false, initialState: _RouteLifecycle__navigator.replace), typedOldRoute);
    }

    public virtual string restorableReplace<T>(dynamic oldRoute, global::System.Func<BuildContext, object?, Route<T>> newRouteBuilder, object? arguments = null)
    {
        RouteBase typedOldRoute = Navigator._requireRoute((object?)oldRoute);
        DartRuntimePrimitives.Assert(() => typedOldRoute._isInstalledIn(this));
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback(newRouteBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = _RestorationInformation__navigator.CreateAnonymous(routeBuilder: newRouteBuilder, arguments: arguments, restorationScopeId: _nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.replace);
        _replaceEntry(entry, typedOldRoute);
        return entry.restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _replaceEntry(_RouteEntry__navigator entry, RouteBase oldRoute)
    {
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        if (Equals(oldRoute, entry.route))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => Equals(entry.currentState, _RouteLifecycle__navigator.replace));
        DartRuntimePrimitives.Assert(() => !entry.route._installed);
        long index = _history.indexWhere(_RouteEntry__navigator.isRoutePredicate(oldRoute));
        DartRuntimePrimitives.Assert(() => index >= 0L, () => (object?)"This Navigator does not contain the specified oldRoute.");
        DartRuntimePrimitives.Assert(() => _history[index].isPresent, () => (object?)"The specified oldRoute has already been removed from the Navigator.");
        bool wasCurrent = oldRoute.isCurrent;
        _history.insert(index + 1L, entry);
        _history[index].complete((Navigator?)null, isReplaced: true, imperativeRemoval: true);
        _flushHistoryUpdates();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (wasCurrent)
        {
            _afterNavigation(entry.route);
        }
    }

    public virtual void replaceRouteBelow<T>(dynamic anchorRoute, Route<T> newRoute)
    {
        RouteBase typedAnchorRoute = Navigator._requireRoute((object?)anchorRoute);
        DartRuntimePrimitives.Assert(() => !newRoute._installed);
        DartRuntimePrimitives.Assert(() => typedAnchorRoute._isInstalledIn(this));
        _replaceEntryBelow(new _RouteEntry__navigator(newRoute, pageBased: false, initialState: _RouteLifecycle__navigator.replace), typedAnchorRoute);
    }

    public virtual string restorableReplaceRouteBelow<T>(dynamic anchorRoute, global::System.Func<BuildContext, object?, Route<T>> newRouteBuilder, object? arguments = null)
    {
        RouteBase typedAnchorRoute = Navigator._requireRoute((object?)anchorRoute);
        DartRuntimePrimitives.Assert(() => typedAnchorRoute._isInstalledIn(this));
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback(newRouteBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = _RestorationInformation__navigator.CreateAnonymous(routeBuilder: newRouteBuilder, arguments: arguments, restorationScopeId: _nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.replace);
        _replaceEntryBelow(entry, typedAnchorRoute);
        return entry.restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _replaceEntryBelow(_RouteEntry__navigator entry, RouteBase anchorRoute)
    {
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        long anchorIndex = _history.indexWhere(_RouteEntry__navigator.isRoutePredicate(anchorRoute));
        DartRuntimePrimitives.Assert(() => anchorIndex >= 0L, () => (object?)"This Navigator does not contain the specified anchorRoute.");
        DartRuntimePrimitives.Assert(() => _history[anchorIndex].isPresent, () => (object?)"The specified anchorRoute has already been removed from the Navigator.");
        long index = anchorIndex - 1L;
        while (index >= 0L)
        {
            if (_history[index].isPresent)
            {
                break;
            }
            index -= 1L;
        }
        DartRuntimePrimitives.Assert(() => index >= 0L, () => (object?)"There are no routes below the specified anchorRoute.");
        _history.insert(index + 1L, entry);
        _history[index].complete((Navigator?)null, isReplaced: true, imperativeRemoval: true);
        _flushHistoryUpdates();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual bool canPop()
    {
        IEnumerator<_RouteEntry__navigator> iterator = _history.where(_RouteEntry__navigator.isPresentPredicate).GetEnumerator();
        if (!iterator.MoveNext())
        {
            return false;
        }
        if (iterator.Current.route.willHandlePopInternally)
        {
            return true;
        }
        if (!iterator.MoveNext())
        {
            return false;
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<bool> maybePop<T>(T? result = default)
    {
        _RouteEntry__navigator? lastEntry = _lastRouteEntryWhereOrNull(_RouteEntry__navigator.isPresentPredicate);
        if (lastEntry is null)
        {
            return false;
        }
        DartRuntimePrimitives.Assert(() => lastEntry.route._isInstalledIn(this));
        DartRuntimePrimitives.Assert(() => lastEntry.route._debugCheckCanConsumeResult(result, methodName: "maybePop"));
        if (Equals(await lastEntry.route.willPop(), RoutePopDisposition.doNotPop))
        {
            return true;
        }
        if (!mounted)
        {
            return true;
        }
        _RouteEntry__navigator? newLastEntry = _lastRouteEntryWhereOrNull(_RouteEntry__navigator.isPresentPredicate);
        if (!Equals(lastEntry, newLastEntry))
        {
            return true;
        }
        switch (lastEntry.route.popDisposition)
        {
            case RoutePopDisposition.bubble:
                {
                    return false;
                }
            case RoutePopDisposition.pop:
                {
                    pop<object>(result);
                    return true;
                }
            case RoutePopDisposition.doNotPop:
                {
                    lastEntry.route.onPopInvokedWithResultObject(false, result);
                    return true;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void pop<T>(T? result = default)
    {
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _RouteEntry__navigator? entry = _lastRouteEntryWhereOrNull(_RouteEntry__navigator.isPresentPredicate);
                return entry?.route._debugCheckCanConsumeResult(result, methodName: "pop") ?? true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _RouteEntry__navigator entryLocal = _history.lastWhere(_RouteEntry__navigator.isPresentPredicate);
        if (entryLocal.pageBased && (widget.onPopPage is not null))
        {
            if (widget.onPopPage!(entryLocal.route, result))
            {
                if (FoundationRuntimePorts.EnumIndex(entryLocal.currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.idle))
                {
                    DartRuntimePrimitives.Assert(() => entryLocal.route.popCompleted);
                    entryLocal.currentState = _RouteLifecycle__navigator.pop;
                }
                entryLocal.route.onPopInvokedWithResultObject(true, result);
            }
        }
        else
        {
            entryLocal.pop<T>(result, imperativeRemoval: true);
            DartRuntimePrimitives.Assert(() => Equals(entryLocal.currentState, _RouteLifecycle__navigator.pop));
        }
        if (Equals(entryLocal.currentState, _RouteLifecycle__navigator.pop))
        {
            _flushHistoryUpdates(rearrangeOverlay: false);
        }
        DartRuntimePrimitives.Assert(() => Equals(entryLocal.currentState, _RouteLifecycle__navigator.idle) || entryLocal.route.popCompleted);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _afterNavigation(entryLocal.route);
    }

    public virtual void popUntil(global::System.Func<dynamic, bool> predicate)
    {
        _RouteEntry__navigator? candidate = _lastRouteEntryWhereOrNull(_RouteEntry__navigator.isPresentPredicate);
        while (candidate is not null)
        {
            if (predicate(candidate.route))
            {
                return;
            }
            pop<object>();
            candidate = _lastRouteEntryWhereOrNull(_RouteEntry__navigator.isPresentPredicate);
        }
    }

    public virtual void popUntilWithResult<T>(global::System.Func<dynamic, bool> predicate, T? result)
    {
        _RouteEntry__navigator? candidate = _lastRouteEntryWhereOrNull(_RouteEntry__navigator.isPresentPredicate);
        while (candidate is not null)
        {
            if (predicate(candidate.route))
            {
                return;
            }
            _RouteEntry__navigator? next = _lastRouteEntryWhereOrNull((e) => _RouteEntry__navigator.isPresentPredicate(e) && (!Equals(e, candidate)));
            if ((next is not null) && !next.route.willHandlePopInternally && predicate(next.route))
            {
                pop<T>(result);
            }
            else
            {
                pop<object>();
            }
            candidate = _lastRouteEntryWhereOrNull(_RouteEntry__navigator.isPresentPredicate);
        }
    }

    public virtual void removeRoute<T>(Route<T> route, T? result = default)
    {
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => route._isInstalledIn(this));
        bool wasCurrent = route.isCurrent;
        _RouteEntry__navigator entry = _history.firstWhere(_RouteEntry__navigator.isRoutePredicate(route));
        entry.complete(result, isReplaced: false, imperativeRemoval: true);
        _flushHistoryUpdates(rearrangeOverlay: false);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (wasCurrent)
        {
            _afterNavigation(_lastRouteEntryWhereOrNull(_RouteEntry__navigator.isPresentPredicate)?.route);
        }
    }

    public virtual void removeRouteBelow<T>(Route<T> anchorRoute, T? result = default)
    {
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => anchorRoute._isInstalledIn(this));
        long anchorIndex = _history.indexWhere(_RouteEntry__navigator.isRoutePredicate(anchorRoute));
        DartRuntimePrimitives.Assert(() => anchorIndex >= 0L, () => (object?)"This Navigator does not contain the specified anchorRoute.");
        DartRuntimePrimitives.Assert(() => _history[anchorIndex].isPresent, () => (object?)"The specified anchorRoute has already been removed from the Navigator.");
        long index = anchorIndex - 1L;
        while (index >= 0L)
        {
            if (_history[index].isPresent)
            {
                break;
            }
            index -= 1L;
        }
        DartRuntimePrimitives.Assert(() => index >= 0L, () => (object?)"There are no routes below the specified anchorRoute.");
        _history[index].complete(result, isReplaced: false, imperativeRemoval: true);
        _flushHistoryUpdates(rearrangeOverlay: false);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual void finalizeRoute(dynamic route)
    {
        RouteBase typedRoute = Navigator._requireRoute((object?)route);
        bool? wasDebugLocked = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                wasDebugLocked = _debugLocked;
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => _history.where((entry) => _RouteEntry__navigator.isRoutePredicate(typedRoute)(DartRuntimePrimitives.ConvertValue<_RouteEntry__navigator>(entry))).Count() == 1L);
        long index = _history.indexWhere(_RouteEntry__navigator.isRoutePredicate(typedRoute));
        _RouteEntry__navigator entryLocal = _history[index];
        if (entryLocal.pageBased && (FoundationRuntimePorts.EnumIndex(entryLocal.currentState) < FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.pop)))
        {
            _observedRouteDeletions.Enqueue(new _NavigatorPopObservation__navigator(typedRoute, _getRouteBefore(index - 1L, _RouteEntry__navigator.willBePresentPredicate)?.route));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => Equals(entryLocal.currentState, _RouteLifecycle__navigator.popping));
        }
        entryLocal.finalize();
        if (!_flushingHistory)
        {
            _flushHistoryUpdates(rearrangeOverlay: false);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = DartRuntimePrimitives.RequireValue(wasDebugLocked);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual Route<T>? _getRouteById<T>(string id)
    {
        return ((Route<T>?)_firstRouteEntryWhereOrNull((entry) => entry.restorationId == id)?.route)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _userGesturesInProgress
    {
        get => _userGesturesInProgressCount;
        set
        {
            var __value = value;
            _userGesturesInProgressCount = __value;
            userGestureInProgressNotifier.value = _userGesturesInProgress > 0L;
        }
    }
    public virtual bool userGestureInProgress => userGestureInProgressNotifier.value;
    public virtual void didStartUserGesture()
    {
        _userGesturesInProgress += 1L;
        if (_userGesturesInProgress == 1L)
        {
            long routeIndex = _getIndexBefore(_history.Count() - 1L, _RouteEntry__navigator.willBePresentPredicate);
            RouteBase routeLocal = _history[routeIndex].route;
            RouteBase? previousRoute = default;
            if (!routeLocal.willHandlePopInternally && (routeIndex > 0L))
            {
                previousRoute = _getRouteBefore(routeIndex - 1L, _RouteEntry__navigator.willBePresentPredicate)!.route;
            }
            foreach (NavigatorObserver observer in _effectiveObservers)
            {
                observer.didStartUserGesture(routeLocal, previousRoute);
            }
        }
    }

    public virtual void didStopUserGesture()
    {
        DartRuntimePrimitives.Assert(() => _userGesturesInProgress > 0L);
        _userGesturesInProgress -= 1L;
        if (_userGesturesInProgress == 0L)
        {
            foreach (NavigatorObserver observer in _effectiveObservers)
            {
                observer.didStopUserGesture();
            }
        }
    }

    internal virtual void _handlePointerDown(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        _activePointers.Add(@event.pointer);
    }

    internal virtual void _handlePointerUpOrCancel(global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        _activePointers.Remove(@event.pointer);
    }

    internal virtual void _cancelActivePointers()
    {
        if (Equals(Scheduler.SchedulerBinding.instance.schedulerPhase, Scheduler.SchedulerPhase.idle))
        {
            global::Doroti.Framework.Rendering.RenderAbsorbPointer? absorber = _overlayKey.currentContext?.findAncestorRenderObjectOfType<global::Doroti.Framework.Rendering.RenderAbsorbPointer>();
            setState(() =>
            {
                absorber?.absorbing = true;
            });
        }
        _activePointers.ToList().forEach((__arg0) => ((global::System.Action<long>)WidgetsBinding.instance.cancelPointer)(__arg0));
    }

    internal virtual _RouteEntry__navigator? _firstRouteEntryWhereOrNull(global::System.Func<_RouteEntry__navigator, bool> test)
    {
        foreach (_RouteEntry__navigator element in _history)
        {
            if (test(element))
            {
                return element;
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _RouteEntry__navigator? _lastRouteEntryWhereOrNull(global::System.Func<_RouteEntry__navigator, bool> test)
    {
        _RouteEntry__navigator? result = default!;
        foreach (_RouteEntry__navigator element in _history)
        {
            if (test(element))
            {
                result = element;
            }
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => !_debugLocked);
        DartRuntimePrimitives.Assert(() => Enumerable.Any(_history));
        return new Overlay(
            key: _overlayKey,
            clipBehavior: widget.clipBehavior,
            initialEntries: overlay is null ? _allRouteOverlayEntries.ToList() : new List<OverlayEntry>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => _bucket;
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
                throw new InvalidOperationException("Dart closure completed without a value.");
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
    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\"."), new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:") }));
                }
                _debugPropertiesWaitingForReregistration = null;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
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
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        property.removeListener(listener);
        property._unregister();
    }

}

public enum _RouteRestorationType__navigator
{
    named,
    anonymous
}

public abstract class _RestorationInformation__navigator
{
    public virtual _RouteRestorationType__navigator type { get; private set; } = default!;
    internal virtual object? _serializableData { get; set; } = default;

    internal _RestorationInformation__navigator(_RouteRestorationType__navigator type)
    {
        this.type = type;
    }

    internal static _RestorationInformation__navigator CreateNamed(string name, object? arguments, long restorationScopeId)
        => new _NamedRestorationInformation__navigator(name, arguments, restorationScopeId);

    internal static _RestorationInformation__navigator CreateAnonymous(global::System.Func<BuildContext, object?, dynamic> routeBuilder, object? arguments, long restorationScopeId)
        => new _AnonymousRestorationInformation__navigator(routeBuilder, arguments, restorationScopeId);

    internal static _RestorationInformation__navigator CreateFromSerializableData(object data)
    {
        var casted = ((List<object?>?)data)!;
        DartRuntimePrimitives.Assert(() => Enumerable.Any(casted));
        _RouteRestorationType__navigator @type = Enum.GetValues<_RouteRestorationType__navigator>().ToList()[(int)(long)casted[(int)0L]!];
        switch (@type)
        {
            case _RouteRestorationType__navigator.named:
                {
                    return _NamedRestorationInformation__navigator.CreateFromSerializableData(casted.Skip(checked((int)1L)).ToList());
                }
            case _RouteRestorationType__navigator.anonymous:
                {
                    return _AnonymousRestorationInformation__navigator.CreateFromSerializableData(casted.Skip(checked((int)1L)).ToList());
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
    }

    public abstract long restorationScopeId { get; }
    public virtual bool isRestorable => true;
    public virtual object getSerializableData()
    {
        _serializableData ??= computeSerializableData();
        return _serializableData!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> computeSerializableData()
    {
        return new List<object> { FoundationRuntimePorts.EnumIndex(type) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract RouteBase createRoute(NavigatorState navigator);
    public virtual _RouteEntry__navigator toRouteEntry(NavigatorState navigator, _RouteLifecycle__navigator initialState = _RouteLifecycle__navigator.add)
    {
        RouteBase route = createRoute(navigator);
        return new _RouteEntry__navigator(route, pageBased: false, initialState: initialState, restorationInformation: this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NamedRestorationInformation__navigator : _RestorationInformation__navigator
{
    private long __field_restorationScopeId = default!;
    public override long restorationScopeId { get => __field_restorationScopeId; }
    public virtual string name { get; private set; } = default!;
    public virtual object? arguments { get; private set; }

    internal _NamedRestorationInformation__navigator(string name, object? arguments, long restorationScopeId) : base(_RouteRestorationType__navigator.named)
    {
        this.name = name;
        this.arguments = arguments;
        __field_restorationScopeId = restorationScopeId;
    }

    internal static _NamedRestorationInformation__navigator CreateFromSerializableData(List<object?> data)
    {
        var __instance = new _NamedRestorationInformation__navigator(default!, default!, default!);
        __instance.__field_restorationScopeId = (long)data[(int)0L]!;
        __instance.name = ((string?)data[(int)1L]!)!;
        __instance.arguments = data.elementAtOrNull(2L);
        return __instance;
    }

    public override List<object> computeSerializableData()
    {
        return ((Func<List<object>>)(() =>
{
    var __cascade = base.computeSerializableData();
    __cascade.AddRange(new List<object> { restorationScopeId, name });
    if (arguments is not null) __cascade.Add(arguments);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RouteBase createRoute(NavigatorState navigator)
    {
        RouteBase route = navigator._routeNamed<object>(name, arguments: arguments)!;
        return route;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AnonymousRestorationInformation__navigator : _RestorationInformation__navigator
{
    private long __field_restorationScopeId = default!;
    public override long restorationScopeId { get => __field_restorationScopeId; }
    public virtual global::System.Func<BuildContext, object?, dynamic> routeBuilder { get; private set; } = default!;
    public virtual object? arguments { get; private set; }

    internal _AnonymousRestorationInformation__navigator(global::System.Func<BuildContext, object?, dynamic> routeBuilder, object? arguments, long restorationScopeId) : base(_RouteRestorationType__navigator.anonymous)
    {
        this.routeBuilder = routeBuilder;
        this.arguments = arguments;
        __field_restorationScopeId = restorationScopeId;
    }

    internal static _AnonymousRestorationInformation__navigator CreateFromSerializableData(List<object?> data)
    {
        var __instance = new _AnonymousRestorationInformation__navigator(default!, default!, default!);
        __instance.__field_restorationScopeId = (long)data[(int)0L]!;
        __instance.routeBuilder = ((global::System.Func<BuildContext, object?, Route<object>>?)Dart_uiLibrary.PluginUtilities.getCallbackFromHandle(new global::Doroti.Ui.CallbackHandle((long)data[(int)1L]!))!)!;
        __instance.arguments = data.elementAtOrNull(2L);
        return __instance;
    }

    public override bool isRestorable => !Foundation.ConstantsLibrary.kIsWeb;
    public override List<object> computeSerializableData()
    {
        DartRuntimePrimitives.Assert(() => isRestorable);
        global::Doroti.Ui.CallbackHandle? handle = Dart_uiLibrary.PluginUtilities.getCallbackHandle(routeBuilder);
        DartRuntimePrimitives.Assert(() => handle is not null);
        return ((Func<List<object>>)(() =>
{
    var __cascade = base.computeSerializableData();
    __cascade.AddRange(new List<object> { restorationScopeId, handle!.toRawHandle() });
    if (arguments is not null) __cascade.Add(arguments);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RouteBase createRoute(NavigatorState navigator)
    {
        object? result = routeBuilder(navigator.context, arguments);
        return Navigator._requireRoute(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _HistoryProperty__navigator : RestorableProperty<DartMap<string?, List<object>>?>
{
    internal virtual DartMap<string?, List<object>>? _pageToPagelessRoutes { get; set; } = default;

    public virtual void update(_History__navigator history)
    {
        DartRuntimePrimitives.Assert(() => isRegistered);
        var wasUninitialized = _pageToPagelessRoutes is null;
        var needsSerialization = wasUninitialized;
        _pageToPagelessRoutes ??= new DartMap<string?, List<object>>();
        _RouteEntry__navigator? currentPage = default!;
        var newRoutesForCurrentPage = new List<object>();
        List<object> oldRoutesForCurrentPage = (_pageToPagelessRoutes!.GetValueOrDefault(null) ?? new List<object>()).ToList();
        var restorationEnabledLocal = true;
        var newMap = new DartMap<string?, List<object>>();
        HashSet<string?> removedPages = _pageToPagelessRoutes!.Keys.toSet();
        foreach (var entry in history)
        {
            if (!entry.isPresentForRestoration)
            {
                entry.restorationEnabled = false;
                continue;
            }
            DartRuntimePrimitives.Assert(() => entry.isPresentForRestoration);
            if (entry.pageBased)
            {
                needsSerialization = needsSerialization || (checked(newRoutesForCurrentPage.Count) != checked((long)oldRoutesForCurrentPage.Count));
                _finalizeEntry(newRoutesForCurrentPage, currentPage, newMap, removedPages);
                currentPage = entry;
                restorationEnabledLocal = entry.restorationId is not null;
                entry.restorationEnabled = restorationEnabledLocal;
                if (restorationEnabledLocal)
                {
                    DartRuntimePrimitives.Assert(() => entry.restorationId is not null);
                    newRoutesForCurrentPage = new List<object>();
                    oldRoutesForCurrentPage = _pageToPagelessRoutes!.GetValueOrDefault(DartRuntimePrimitives.RequireReference(entry.restorationId)) ?? new List<object>();
                }
                else
                {
                    newRoutesForCurrentPage = new List<object>();
                    oldRoutesForCurrentPage = new List<object>();
                }
                continue;
            }
            DartRuntimePrimitives.Assert(() => !entry.pageBased);
            restorationEnabledLocal = restorationEnabledLocal && (entry.restorationInformation?.isRestorable ?? false);
            entry.restorationEnabled = restorationEnabledLocal;
            if (restorationEnabledLocal)
            {
                DartRuntimePrimitives.Assert(() => entry.restorationId is not null);
                DartRuntimePrimitives.Assert(() => (currentPage is null) || (currentPage.restorationId is not null));
                DartRuntimePrimitives.Assert(() => entry.restorationInformation is not null);
                object serializedData = entry.restorationInformation!.getSerializableData();
                needsSerialization = needsSerialization || (checked(oldRoutesForCurrentPage.Count) <= checked((long)newRoutesForCurrentPage.Count)) || (!Equals(oldRoutesForCurrentPage[(int)checked((long)newRoutesForCurrentPage.Count)], serializedData));
                newRoutesForCurrentPage.Add(serializedData);
            }
        }
        needsSerialization = needsSerialization || (checked(newRoutesForCurrentPage.Count) != checked((long)oldRoutesForCurrentPage.Count));
        _finalizeEntry(newRoutesForCurrentPage, currentPage, newMap, removedPages);
        needsSerialization = needsSerialization || Enumerable.Any(removedPages);
        DartRuntimePrimitives.Assert(() => wasUninitialized || (_debugMapsEqual(_pageToPagelessRoutes!, newMap) != needsSerialization));
        if (needsSerialization)
        {
            _pageToPagelessRoutes = newMap.cast<string?, List<object>>();
            notifyListeners();
        }
    }

    internal virtual void _finalizeEntry(List<object> routes, _RouteEntry__navigator? page, DartMap<string?, List<object>> pageToRoutes, HashSet<string?> pagesToRemove)
    {
        DartRuntimePrimitives.Assert(() => (page is null) || page.pageBased);
        DartRuntimePrimitives.Assert(() => !pageToRoutes.ContainsKey(page?.restorationId));
        if (Enumerable.Any(routes))
        {
            DartRuntimePrimitives.Assert(() => (page is null) || (page.restorationId is not null));
            string? restorationIdLocal = page?.restorationId;
            pageToRoutes[restorationIdLocal] = routes;
            pagesToRemove.Remove(restorationIdLocal);
        }
    }

    internal virtual bool _debugMapsEqual(DartMap<string?, List<object>> a, DartMap<string?, List<object>> b)
    {
        if (!CollectionsLibrary.setEquals(a.Keys.toSet(), b.Keys.toSet()))
        {
            return false;
        }
        foreach (string? key in a.Keys)
        {
            if (!CollectionsLibrary.listEquals(a.GetValueOrDefault(key), b.GetValueOrDefault(key)))
            {
                return false;
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void clear()
    {
        DartRuntimePrimitives.Assert(() => isRegistered);
        if (_pageToPagelessRoutes is null)
        {
            return;
        }
        _pageToPagelessRoutes = null;
        notifyListeners();
    }

    public virtual bool hasData => DartRuntimePrimitives.ConvertValue<bool>(_pageToPagelessRoutes is not null);
    public virtual List<_RouteEntry__navigator> restoreEntriesForPage(_RouteEntry__navigator? page, NavigatorState navigator)
    {
        DartRuntimePrimitives.Assert(() => isRegistered);
        DartRuntimePrimitives.Assert(() => (page is null) || page.pageBased);
        var result = new List<_RouteEntry__navigator>();
        if ((_pageToPagelessRoutes is null) || (page is not null) && (page.restorationId is null))
        {
            return result;
        }
        List<object>? serializedData = _pageToPagelessRoutes!.GetValueOrDefault(page?.restorationId)?.ToList();
        if (serializedData is null)
        {
            return result;
        }
        foreach (object data in serializedData)
        {
            result.Add(_RestorationInformation__navigator.CreateFromSerializableData(data).toRouteEntry(navigator));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DartMap<string?, List<object>>? createDefaultValue()
    {
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DartMap<string?, List<object>>? fromPrimitives(object? data)
    {
        var casted = DartRuntimePrimitives.ConvertMap<object, object>((System.Collections.IDictionary)data!);
        return casted.map<object, object, string?, List<object>>((key, value) => new MapEntry<string?, List<object>>(((string?)key)!, new List<object>(DartRuntimePrimitives.ConvertEnumerable<object>(((List<object>?)value)!))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initWithValue(DartMap<string?, List<object>>? value)
    {
        _pageToPagelessRoutes = value;
    }

    public override object? toPrimitives()
    {
        return _pageToPagelessRoutes;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool enabled => hasData;
}

public delegate NavigatorState NavigatorFinderCallback(BuildContext context);

public delegate string RoutePresentationCallback(NavigatorState navigator, object? arguments);

public delegate void RouteCompletionCallback<T>(T result);

public class RestorableRouteFuture<T> : RestorableProperty<string?>
{
    public virtual global::System.Func<BuildContext, NavigatorState> navigatorFinder { get; private set; } = default!;
    public virtual global::System.Func<NavigatorState, object?, string> onPresent { get; private set; } = default!;
    public virtual global::System.Action<T>? onComplete { get; private set; }
    internal virtual Route<T>? _route { get; set; } = default;
    // Dart library-private member: distinct from the same name in the base library.
    internal new virtual bool _disposed { get; set; } = false;

    public RestorableRouteFuture(global::System.Func<BuildContext, NavigatorState> navigatorFinder = default!, global::System.Func<NavigatorState, object?, string> onPresent = default!, global::System.Action<T>? onComplete = null)
    {
        global::System.Func<BuildContext, NavigatorState> __navigatorFinder = navigatorFinder ?? _defaultNavigatorFinder;
        this.navigatorFinder = __navigatorFinder;
        this.onPresent = onPresent;
        this.onComplete = onComplete;
    }

    public virtual void present(object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => !isPresent);
        DartRuntimePrimitives.Assert(() => isRegistered);
        string routeId = onPresent(_navigator, arguments);
        _hookOntoRouteFuture(routeId);
        notifyListeners();
    }

    public virtual bool isPresent => DartRuntimePrimitives.ConvertValue<bool>(route is not null);
    public virtual Route<T>? route => _route;
    public override string? createDefaultValue() => DartRuntimePrimitives.ConvertValue<string>(null);
    public override void initWithValue(string? value)
    {
        if (value is not null)
        {
            _hookOntoRouteFuture(value);
        }
    }

    public override object? toPrimitives()
    {
        DartRuntimePrimitives.Assert(() => route is not null);
        DartRuntimePrimitives.Assert(() => enabled);
        return route?.restorationScopeId.value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string? fromPrimitives(object? data)
    {
        DartRuntimePrimitives.Assert(() => data is not null);
        return ((string?)data!)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        base.dispose();
        _route?.restorationScopeId.removeListener(notifyListeners);
        _disposed = true;
    }

    public override bool enabled => DartRuntimePrimitives.ConvertValue<bool>(route?.restorationScopeId.value is not null);
    internal virtual NavigatorState _navigator
    {
        get
        {
            NavigatorState navigator = navigatorFinder(state.context);
            return navigator;
        }
    }
    internal virtual void _hookOntoRouteFuture(string id)
    {
        _route = _navigator._getRouteById<T>(id);
        DartRuntimePrimitives.Assert(() => _route is not null);
        route!.restorationScopeId.addListener(notifyListeners);
        DartRuntimePrimitives.Ignore(route!.popped.then((global::System.Action<object>)((result) =>
        {
            if (_disposed)
            {
                return;
            }
            _route?.restorationScopeId.removeListener(notifyListeners);
            _route = null;
            notifyListeners();
            onComplete?.Invoke(((T?)(object?)result)!);
        })));
    }

    internal static NavigatorState _defaultNavigatorFinder(BuildContext context) => Navigator.of(context);
}

public class NavigationNotification : Notification
{
    public virtual bool canHandlePop { get; private set; } = default!;

    public NavigationNotification(bool canHandlePop)
    {
        this.canHandlePop = canHandlePop;
    }

    public override string ToString()
    {
        return $"NavigationNotification canHandlePop: {canHandlePop}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
