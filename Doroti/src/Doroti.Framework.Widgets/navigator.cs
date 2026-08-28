// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/navigator.dart
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

public delegate void DidRemovePageCallback(Page<object> page);

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
    public abstract void didReplace(dynamic oldRoute);
    public abstract Future<RoutePopDisposition> willPop();
    public abstract RoutePopDisposition popDisposition { get; }
    public abstract void onPopInvoked(bool didPop);
    public abstract bool willHandlePopInternally { get; }
    public abstract void didPopNext(dynamic nextRoute);
    public abstract void didChangeNext(dynamic nextRoute);
    public abstract void didChangePrevious(dynamic previousRoute);
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
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<string?> _restorationScopeId { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<string?>(((string)(object)null));
    internal virtual Completer<T?> _popCompleter { get; private set; } = new Completer<T?>();
    internal virtual Completer<T?> _disposeCompleter { get; private set; } = new Completer<T?>();

    protected Route(RouteSettings? settings = null, bool? requestFocus = null)
    {
        this._settings = (settings ?? new RouteSettings());
        this._requestFocus = requestFocus;
    }

    public override bool requestFocus => DartRuntimePrimitives.ConvertValue<bool>(((this._requestFocus ?? this.navigator?.widget.requestFocus) ?? false));
    public override NavigatorState? navigator => this._navigator;
    internal override bool _installed => this._navigator is not null;
    internal override bool _isInstalledIn(NavigatorState state) => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(this._navigator, state)));
    public override RouteSettings settings => this._settings;
    internal override bool _isPageBased => (this.settings is Page<object?>);
    public override global::Doroti.Framework.Foundation.ValueListenable<string?> restorationScopeId => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Foundation.ValueListenable<string?>>(this._restorationScopeId);
    internal override void _updateSettings(RouteSettings newSettings)
    {
        if ((!object.Equals(this._settings, newSettings)))
        {
            _settings = newSettings;
            if (this._installed)
            {
                changedInternalState();
            }
        }
    }

    internal override void _updateRestorationId(string? restorationId)
    {
        this._restorationScopeId.value = restorationId;
    }

    public override List<OverlayEntry> overlayEntries => new List<OverlayEntry>();
    public override void install()
    {
    }

    public override global::Doroti.Framework.Scheduler.TickerFuture didPush()
    {
        return ((Func<global::Doroti.Framework.Scheduler.TickerFuture>)(() =>
{
    var __cascade = global::Doroti.Framework.Scheduler.TickerFuture.CreateComplete();
    __cascade.then(((global::System.Func<object?, object>)((_) =>
    {
        if (this.requestFocus)
        {
            this.navigator!.focusNode.enclosingScope?.requestFocus();
        }
        return default!;
    })));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didAdd()
    {
        if (this.requestFocus)
        {
            DartRuntimePrimitives.Ignore(global::Doroti.Framework.Scheduler.TickerFuture.CreateComplete().then(((global::System.Func<object?, object>)((_) =>
            {
                this.navigator?.focusNode.enclosingScope?.requestFocus();
                return default!;
            }))));
        }
    }

    public override void didReplace(dynamic oldRoute)
    {
    }

    public async override Future<RoutePopDisposition> willPop()
    {
        return (this.isFirst ? RoutePopDisposition.bubble : RoutePopDisposition.pop);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RoutePopDisposition popDisposition
    {
        get
        {
            if (this._isPageBased)
            {
                var page = ((Page<object?>?)(object?)this.settings)!;
                if (!((Page<object>)page).canPop)
                {
                    return RoutePopDisposition.doNotPop;
                }
            }
            return (this.isFirst ? RoutePopDisposition.bubble : RoutePopDisposition.pop);
            return default!;
        }
    }
    public override void onPopInvoked(bool didPop)
    {
    }

    public virtual void onPopInvokedWithResult(bool didPop, T? result)
    {
        if (this._isPageBased)
        {
            var page = ((Page<T>?)(object?)this.settings)!;
            page.onPopInvoked(didPop, result);
        }
    }

    public override bool willHandlePopInternally => false;
    public virtual T? currentResult => DartRuntimePrimitives.ConvertValue<T>(null);
    internal override object? currentResultObject => currentResult;
    internal override Future disposalCompleted => this._disposeCompleter.future;
    internal override bool popCompleted => this._popCompleter.isCompleted;
    public virtual Future<T?> popped => this._popCompleter.future;
    public virtual bool didPop(T? result)
    {
        didComplete(result);
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didComplete(T? result)
    {
        this._popCompleter.complete(((result ?? (T)this.currentResult)));
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

    public override void didChangeNext(dynamic nextRoute)
    {
    }

    public override void didChangePrevious(dynamic previousRoute)
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
        this._restorationScopeId.dispose();
        this._disposeCompleter.complete();
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
    }

    public override bool isCurrent
    {
        get
        {
            if (!this._installed)
            {
                return false;
            }
            _RouteEntry__navigator? currentRouteEntry = ((_RouteEntry__navigator?)(object?)this._navigator!._lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
            if ((currentRouteEntry is null))
            {
                return false;
            }
            return (object.Equals(((_RouteEntry__navigator)currentRouteEntry).route, this));
            return default!;
        }
    }
    public override bool isFirst
    {
        get
        {
            if (!this._installed)
            {
                return false;
            }
            _RouteEntry__navigator? currentRouteEntry = ((_RouteEntry__navigator?)(object?)this._navigator!._firstRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
            if ((currentRouteEntry is null))
            {
                return false;
            }
            return (object.Equals(((_RouteEntry__navigator)currentRouteEntry).route, this));
            return default!;
        }
    }
    public override bool hasActiveRouteBelow
    {
        get
        {
            if (!this._installed)
            {
                return false;
            }
            foreach (_RouteEntry__navigator entry in this._navigator!._history)
            {
                if ((object.Equals(((_RouteEntry__navigator)entry).route, this)))
                {
                    return false;
                }
                if (_RouteEntry__navigator.isPresentPredicate(entry))
                {
                    return true;
                }
            }
            return false;
            return default!;
        }
    }
    public override bool isActive
    {
        get
        {
            return (this._navigator?._firstRouteEntryWhereOrNull(_RouteEntry__navigator.isRoutePredicate(this))?.isPresent ?? false);
            return default!;
        }
    }
    internal override bool _debugCheckCanConsumeResult(object? result, string methodName)
    {
        if ((result is not T))
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

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RouteSettings"))}({((this.name is null) ? "none" : $"\"{this.name}\"")}, {this.arguments})";
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

    public virtual bool canUpdate(Page<object> other)
    {
        return ((object.Equals(DartRuntimePrimitives.RuntimeType(other), this.GetType())) && (object.Equals(((Page<object>)other).key, this.key)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Route<T> createRoute(BuildContext context);
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Page"))}(\"{this.name}\", {this.key}, {this.arguments})";
}

public class NavigatorObserver
{
    internal static Expando<NavigatorState> _navigators = new Expando<NavigatorState>();

    public virtual NavigatorState? navigator => _navigators[this];
    public virtual void didPush(dynamic route, dynamic previousRoute)
    {
    }

    public virtual void didPop(dynamic route, dynamic previousRoute)
    {
    }

    public virtual void didRemove(dynamic route, dynamic previousRoute)
    {
    }

    public virtual void didReplace(dynamic newRoute = null, dynamic oldRoute = null)
    {
    }

    public virtual void didChangeTop(dynamic topRoute, dynamic previousTopRoute)
    {
    }

    public virtual void didStartUserGesture(dynamic route, dynamic previousRoute)
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
        HeroControllerScope? host = ((HeroControllerScope?)(object?)context.dependOnInheritedWidgetOfExactType<HeroControllerScope>());
        return host?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static HeroController of(BuildContext context)
    {
        HeroController? controller = ((HeroController?)(object?)HeroControllerScope.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((controller is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("HeroControllerScope.of() was called with a context that does not contain a " + "HeroControllerScope widget.\n" + "No HeroControllerScope widget ancestor could be found starting from the " + "context that was passed to HeroControllerScope.of(). This can happen " + "because you are using a widget that looks for a HeroControllerScope " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return controller!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (HeroControllerScope)(object)oldWidget;
        return (!object.Equals(((HeroControllerScope)__oldWidget).controller, this.controller));
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
    public abstract void markForPop(dynamic result = default!);
    public abstract void markForComplete(dynamic result = default!);
    public virtual void markForRemove() => markForComplete();
}

public abstract class TransitionDelegate<T>
{
    protected TransitionDelegate()
    {
    }

    internal virtual IEnumerable<RouteTransitionRecord> _transition(List<RouteTransitionRecord> newPageRouteHistory, DartMap<RouteTransitionRecord?, RouteTransitionRecord> locationToExitingPageRoute, DartMap<RouteTransitionRecord?, List<RouteTransitionRecord>> pageRouteToPagelessRoutes)
    {
        IEnumerable<RouteTransitionRecord> results = ((IEnumerable<RouteTransitionRecord>)(object?)resolve(newPageRouteHistory: newPageRouteHistory, locationToExitingPageRoute: locationToExitingPageRoute, pageRouteToPagelessRoutes: pageRouteToPagelessRoutes));
        DartRuntimePrimitives.Assert(() =>
            {
                List<RouteTransitionRecord> resultsToVerify = results.ToList().ToList();
                HashSet<RouteTransitionRecord> exitingPageRoutes = locationToExitingPageRoute.Values.toSet();
                foreach (var exitingPageRoute in exitingPageRoutes)
                {
                    DartRuntimePrimitives.Assert(() => !((RouteTransitionRecord)exitingPageRoute).isWaitingForExitingDecision);
                    if (pageRouteToPagelessRoutes.ContainsKey(exitingPageRoute))
                    {
                        foreach (RouteTransitionRecord pagelessRoute in pageRouteToPagelessRoutes.GetValueOrDefault(exitingPageRoute)!)
                        {
                            DartRuntimePrimitives.Assert(() => !((RouteTransitionRecord)pagelessRoute).isWaitingForExitingDecision);
                        }
                    }
                }
                var indexOfNextRouteInNewHistory = 0L;
                foreach (_RouteEntry__navigator routeEntry in resultsToVerify.cast<_RouteEntry__navigator>())
                {
                    DartRuntimePrimitives.Assert(() => (!((_RouteEntry__navigator)routeEntry).isWaitingForEnteringDecision && !((_RouteEntry__navigator)routeEntry).isWaitingForExitingDecision));
                    if (((indexOfNextRouteInNewHistory >= checked((long)(newPageRouteHistory.Count))) || (!object.Equals(routeEntry, newPageRouteHistory[(int)(indexOfNextRouteInNewHistory)]))))
                    {
                        DartRuntimePrimitives.Assert(() => exitingPageRoutes.Contains(routeEntry));
                        exitingPageRoutes.Remove(routeEntry);
                    }
                    else
                    {
                        indexOfNextRouteInNewHistory += 1L;
                    }
                }
                DartRuntimePrimitives.Assert(() => ((indexOfNextRouteInNewHistory == checked((long)(newPageRouteHistory.Count))) && !System.Linq.Enumerable.Any(exitingPageRoutes)), () => (object?)$"The merged result from the {this.GetType()}.resolve does not include all " + "required routes. Do you remember to merge all exiting routes?");
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
            if ((exitingPageRoute is null))
            {
                return;
            }
            if (((RouteTransitionRecord)exitingPageRoute).isWaitingForExitingDecision)
            {
                bool hasPagelessRoute = pageRouteToPagelessRoutes.ContainsKey(exitingPageRoute);
                bool isLastExitingPageRoute = (isLast && !locationToExitingPageRoute.ContainsKey(exitingPageRoute));
                if ((isLastExitingPageRoute && !hasPagelessRoute))
                {
                    exitingPageRoute.markForPop(((RouteTransitionRecord)exitingPageRoute).route.currentResultObject);
                }
                else
                {
                    exitingPageRoute.markForComplete(((RouteTransitionRecord)exitingPageRoute).route.currentResultObject);
                }
                if (hasPagelessRoute)
                {
                    List<RouteTransitionRecord> pagelessRoutes = pageRouteToPagelessRoutes.GetValueOrDefault(exitingPageRoute)!.ToList();
                    foreach (var pagelessRoute in pagelessRoutes)
                    {
                        if (((RouteTransitionRecord)pagelessRoute).isWaitingForExitingDecision)
                        {
                            if ((isLastExitingPageRoute && (object.Equals(pagelessRoute, pagelessRoutes.Last()))))
                            {
                                pagelessRoute.markForPop(((RouteTransitionRecord)pagelessRoute).route.currentResultObject);
                            }
                            else
                            {
                                pagelessRoute.markForComplete(((RouteTransitionRecord)pagelessRoute).route.currentResultObject);
                            }
                        }
                    }
                }
            }
            results.Add(exitingPageRoute);
            handleExitingRoute(exitingPageRoute, isLast);
        }
        handleExitingRoute(((RouteTransitionRecord)(object)null), !System.Linq.Enumerable.Any(newPageRouteHistory));
        foreach (var pageRoute in newPageRouteHistory)
        {
            var isLastIteration = (object.Equals(newPageRouteHistory.Last(), pageRoute));
            if (((RouteTransitionRecord)pageRoute).isWaitingForEnteringDecision)
            {
                if ((!locationToExitingPageRoute.ContainsKey(pageRoute) && isLastIteration))
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
        return ((IEnumerable<RouteTransitionRecord>)(object?)results);
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

    internal static readonly List<Page<object>> _defaultPages = new();
    public virtual List<Page<object>> pages { get; private set; } = default!;
    public virtual global::System.Func<dynamic, object, bool>? onPopPage { get; private set; }
    public virtual global::System.Action<Page<object>>? onDidRemovePage { get; private set; }
    public virtual TransitionDelegate<object> transitionDelegate { get; private set; } = default!;
    public virtual string? initialRoute { get; private set; }
    public virtual global::System.Func<RouteSettings, dynamic>? onGenerateRoute { get; private set; }
    public virtual global::System.Func<RouteSettings, dynamic>? onUnknownRoute { get; private set; }
    public virtual List<NavigatorObserver> observers { get; private set; } = default!;
    public virtual string? restorationScopeId { get; private set; }
    public virtual TraversalEdgeBehavior routeTraversalEdgeBehavior { get; private set; } = default!;
    public virtual TraversalEdgeBehavior routeDirectionalTraversalEdgeBehavior { get; private set; } = default!;
    public const string defaultRouteName = "/";
    public virtual global::System.Func<NavigatorState, string, List<dynamic>> onGenerateInitialRoutes { get; private set; } = default!;
    public virtual bool reportsRouteUpdateToEngine { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool requestFocus { get; private set; } = default!;

    public Navigator(global::Doroti.Framework.Foundation.Key? key = null, List<Page<object>> pages = default!, global::System.Func<dynamic, object, bool>? onPopPage = null, string? initialRoute = null, global::System.Func<NavigatorState, string, List<dynamic>> onGenerateInitialRoutes = default!, global::System.Func<RouteSettings, dynamic>? onGenerateRoute = null, global::System.Func<RouteSettings, dynamic>? onUnknownRoute = null, TransitionDelegate<object> transitionDelegate = default!, bool reportsRouteUpdateToEngine = false, Clip clipBehavior = Clip.hardEdge, List<NavigatorObserver> observers = default!, bool requestFocus = true, string? restorationScopeId = null, TraversalEdgeBehavior? routeTraversalEdgeBehavior = null, TraversalEdgeBehavior? routeDirectionalTraversalEdgeBehavior = null, global::System.Action<Page<object>>? onDidRemovePage = null) : base(key: key)
    {
        List<Page<object>> __pages = pages ?? _defaultPages;
        global::System.Func<NavigatorState, string, List<dynamic>> __onGenerateInitialRoutes = onGenerateInitialRoutes ?? Navigator.defaultGenerateInitialRoutes;
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
        return ((Future<T?>)(object?)Navigator.of(context).pushNamed<T>(routeName, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePushNamed<T>(BuildContext context, string routeName, object? arguments = null)
    {
        return ((string)(object?)Navigator.of(context).restorablePushNamed<T>(routeName, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<T?> pushReplacementNamed<T, TO>(BuildContext context, string routeName, TO? result = default, object? arguments = null)
    {
        return ((Future<T?>)(object?)Navigator.of(context).pushReplacementNamed<T, TO>(routeName, arguments: arguments, result: result));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePushReplacementNamed<T, TO>(BuildContext context, string routeName, TO? result = default, object? arguments = null)
    {
        return ((string)(object?)Navigator.of(context).restorablePushReplacementNamed<T, TO>(routeName, arguments: arguments, result: result));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<T?> popAndPushNamed<T, TO>(BuildContext context, string routeName, TO? result = default, object? arguments = null)
    {
        return ((Future<T?>)(object?)Navigator.of(context).popAndPushNamed<T, TO>(routeName, arguments: arguments, result: result));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePopAndPushNamed<T, TO>(BuildContext context, string routeName, TO? result = default, object? arguments = null)
    {
        return ((string)(object?)Navigator.of(context).restorablePopAndPushNamed<T, TO>(routeName, arguments: arguments, result: result));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<T?> pushNamedAndRemoveUntil<T>(BuildContext context, string newRouteName, global::System.Func<dynamic, bool> predicate, object? arguments = null)
    {
        return ((Future<T?>)(object?)Navigator.of(context).pushNamedAndRemoveUntil<T>(newRouteName, (global::System.Func<dynamic, bool>)predicate, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePushNamedAndRemoveUntil<T>(BuildContext context, string newRouteName, global::System.Func<dynamic, bool> predicate, object? arguments = null)
    {
        return ((string)(object?)Navigator.of(context).restorablePushNamedAndRemoveUntil<T>(newRouteName, (global::System.Func<dynamic, bool>)predicate, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<T?> push<T>(BuildContext context, Route<T> route)
    {
        return ((Future<T?>)(object?)Navigator.of(context).push(route));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePush<T>(BuildContext context, global::System.Func<BuildContext, object, Route<T>> routeBuilder, object? arguments = null)
    {
        return ((string)(object?)Navigator.of(context).restorablePush((global::System.Func<BuildContext, object, Route<T>>)routeBuilder, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<T?> pushReplacement<T, TO>(BuildContext context, Route<T> newRoute, TO? result = default)
    {
        return ((Future<T?>)(object?)Navigator.of(context).pushReplacement<T, TO>(newRoute, result: result));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePushReplacement<T, TO>(BuildContext context, global::System.Func<BuildContext, object, Route<T>> routeBuilder, TO? result = default, object? arguments = null)
    {
        return ((string)(object?)Navigator.of(context).restorablePushReplacement<T, TO>((global::System.Func<BuildContext, object, Route<T>>)routeBuilder, result: result, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<T?> pushAndRemoveUntil<T>(BuildContext context, Route<T> newRoute, global::System.Func<dynamic, bool> predicate)
    {
        return ((Future<T?>)(object?)Navigator.of(context).pushAndRemoveUntil<T>(newRoute, (global::System.Func<dynamic, bool>)predicate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static string restorablePushAndRemoveUntil<T>(BuildContext context, global::System.Func<BuildContext, object, Route<T>> newRouteBuilder, global::System.Func<dynamic, bool> predicate, object? arguments = null)
    {
        return ((string)(object?)Navigator.of(context).restorablePushAndRemoveUntil<T>((global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder, (global::System.Func<dynamic, bool>)predicate, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void replace<T>(BuildContext context, dynamic oldRoute, Route<T> newRoute)
    {
        Navigator.of(context).replace<T>(oldRoute: Navigator._requireRoute((object?)oldRoute), newRoute: newRoute);
        return;
    }

    public static string restorableReplace<T>(BuildContext context, dynamic oldRoute, global::System.Func<BuildContext, object, Route<T>> newRouteBuilder, object? arguments = null)
    {
        return ((string)(object?)Navigator.of(context).restorableReplace<T>(oldRoute: Navigator._requireRoute((object?)oldRoute), newRouteBuilder: (global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void replaceRouteBelow<T>(BuildContext context, dynamic anchorRoute, Route<T> newRoute)
    {
        Navigator.of(context).replaceRouteBelow<T>(anchorRoute: Navigator._requireRoute((object?)anchorRoute), newRoute: newRoute);
        return;
    }

    public static string restorableReplaceRouteBelow<T>(BuildContext context, dynamic anchorRoute, global::System.Func<BuildContext, object, Route<T>> newRouteBuilder, object? arguments = null)
    {
        return ((string)(object?)Navigator.of(context).restorableReplaceRouteBelow<T>(anchorRoute: Navigator._requireRoute((object?)anchorRoute), newRouteBuilder: (global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool canPop(BuildContext context)
    {
        NavigatorState? navigator = ((NavigatorState?)(object?)Navigator.maybeOf(context));
        return ((navigator is not null) && navigator.canPop());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future<bool> maybePop<T>(BuildContext context, T? result = default)
    {
        return ((Future<bool>)(object?)Navigator.of(context).maybePop<T>(result));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void pop<T>(BuildContext context, T? result = default)
    {
        Navigator.of(context).pop<T>(result);
    }

    public static void popUntil(BuildContext context, global::System.Func<dynamic, bool> predicate)
    {
        Navigator.of(context).popUntil((global::System.Func<dynamic, bool>)predicate);
    }

    public static void popUntilWithResult<T>(BuildContext context, global::System.Func<dynamic, bool> predicate, T? result)
    {
        Navigator.of(context).popUntilWithResult<T>((global::System.Func<dynamic, bool>)predicate, result);
    }

    public static void removeRoute<T>(BuildContext context, Route<T> route, T? result = default)
    {
        Navigator.of(context).removeRoute<T>(route, result);
        return;
    }

    public static void removeRouteBelow<T>(BuildContext context, Route<T> anchorRoute, T? result = default)
    {
        Navigator.of(context).removeRouteBelow<T>(anchorRoute, result);
        return;
    }

    public static NavigatorState of(BuildContext context, bool rootNavigator = false)
    {
        NavigatorState? navigator = default!;
        if (context is StatefulElement { state: NavigatorState stateLocal } __object119923)
        {
            navigator = stateLocal;
        }
        navigator = (rootNavigator ? (context.findRootAncestorStateOfType<NavigatorState>() ?? navigator) : ((navigator ?? (NavigatorState)context.findAncestorStateOfType<NavigatorState>())));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((navigator is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Navigator operation requested with a context that does not include a Navigator.\n" + "The context used to push or pop routes from the Navigator must be that of a " + "widget that is a descendant of a Navigator widget."));
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
        return (rootNavigator ? (context.findRootAncestorStateOfType<NavigatorState>() ?? navigator) : ((navigator ?? (NavigatorState)context.findAncestorStateOfType<NavigatorState>())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static List<object> defaultGenerateInitialRoutes(NavigatorState navigator, string initialRouteName)
    {
        var result = new List<object>();
        if ((initialRouteName.startsWith("/") && (initialRouteName.Length > 1L)))
        {
            initialRouteName = initialRouteName.substring(1L);
            DartRuntimePrimitives.Assert(() => (Navigator.defaultRouteName == "/"));
            List<string>? debugRouteNames = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    debugRouteNames = new List<string> { Navigator.defaultRouteName };
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            result.Add(navigator._routeNamed<object>(Navigator.defaultRouteName, arguments: null, allowNull: true));
            List<string> routeParts = initialRouteName.split("/").ToList();
            if ((initialRouteName.Length != 0))
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
            if ((result.Last() is null))
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: "Could not navigate to initial route.\n" + $"The requested route name was: \"/{initialRouteName}\"\n" + "There was no corresponding route in the app, and therefore the initial route specified will be " + $"ignored and \"{Navigator.defaultRouteName}\" will be used instead."));
                        return true;
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                foreach (var routeLocal in result)
                {
                    if (routeLocal is not null)
                    {
                        Navigator._requireRoute(routeLocal).dispose();
                    }
                }
                result.Clear();
            }
        }
        else
        {
            if ((initialRouteName != Navigator.defaultRouteName))
            {
                result.Add(navigator._routeNamed<object>(initialRouteName, arguments: null, allowNull: true));
            }
        }
        result.removeWhere(((route) => (route is null)));
        if (!System.Linq.Enumerable.Any(result))
        {
            result.Add(navigator._routeNamed<object>(Navigator.defaultRouteName, arguments: null));
        }
        return ((List<object>)(object?)result.cast<object>());
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
        this.__field_route = route;
        this.pageBased = pageBased;
        this.restorationInformation = restorationInformation;
        this.currentState = initialState;
        System.Diagnostics.Debug.Assert((!pageBased || (route.settings is Page<object>)));
        System.Diagnostics.Debug.Assert((((((object.Equals(initialState, _RouteLifecycle__navigator.staging)) || (object.Equals(initialState, _RouteLifecycle__navigator.add))) || (object.Equals(initialState, _RouteLifecycle__navigator.push))) || (object.Equals(initialState, _RouteLifecycle__navigator.pushReplace))) || (object.Equals(initialState, _RouteLifecycle__navigator.replace))));
    }

    public virtual string? restorationId
    {
        get
        {
            if (this.pageBased)
            {
                var page = ((Page<object?>?)(object?)this.route.settings)!;
                return ((((Page<object>)page).restorationId is not null) ? $"p+{((Page<object>)page).restorationId}" : null);
            }
            if ((this.restorationInformation is not null))
            {
                return $"r+{this.restorationInformation!.restorationScopeId}";
            }
            return ((string)(object)null);
            return default!;
        }
    }
    public virtual bool canUpdateFrom(Page<object> page)
    {
        if (!this.willBePresent)
        {
            return false;
        }
        if (!this.pageBased)
        {
            return false;
        }
        var routePage = ((Page<object>?)(object?)this.route.settings)!;
        return page.canUpdate(routePage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleAdd(NavigatorState navigator, RouteBase? previousPresent)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this.currentState, _RouteLifecycle__navigator.add)));
        DartRuntimePrimitives.Assert(() => ((NavigatorState)navigator)._debugLocked);
        currentState = _RouteLifecycle__navigator.adding;
        ((NavigatorState)navigator)._observedRouteAdditions.Enqueue(new _NavigatorPushObservation__navigator(this.route, previousPresent));
    }

    public virtual void handlePush(NavigatorState navigator, bool isNewFirst, RouteBase? previous, RouteBase? previousPresent)
    {
        DartRuntimePrimitives.Assert(() => (((object.Equals(this.currentState, _RouteLifecycle__navigator.push)) || (object.Equals(this.currentState, _RouteLifecycle__navigator.pushReplace))) || (object.Equals(this.currentState, _RouteLifecycle__navigator.replace))));
        DartRuntimePrimitives.Assert(() => ((NavigatorState)navigator)._debugLocked);
        DartRuntimePrimitives.Assert(() => !this.route._installed, () => (object?)"The pushed route has already been used. When pushing a route, a new " + "Route object must be provided.");
        _RouteLifecycle__navigator previousState = this.currentState;
        this.route._navigator = navigator;
        this.route.install();
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this.route.overlayEntries));
        if (((object.Equals(this.currentState, _RouteLifecycle__navigator.push)) || (object.Equals(this.currentState, _RouteLifecycle__navigator.pushReplace))))
        {
            global::Doroti.Framework.Scheduler.TickerFuture routeFuture = this.route.didPush();
            currentState = _RouteLifecycle__navigator.pushing;
            routeFuture.whenCompleteOrCancel(((global::System.Action)(() =>
            {
                if ((object.Equals(this.currentState, _RouteLifecycle__navigator.pushing)))
                {
                    currentState = _RouteLifecycle__navigator.idle;
                    DartRuntimePrimitives.Assert(() => !((NavigatorState)navigator)._debugLocked);
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
            })));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this.currentState, _RouteLifecycle__navigator.replace)));
            this.route.didReplace(previous);
            currentState = _RouteLifecycle__navigator.idle;
        }
        if (isNewFirst)
        {
            this.route.didChangeNext(null);
        }
        if (((object.Equals(previousState, _RouteLifecycle__navigator.replace)) || (object.Equals(previousState, _RouteLifecycle__navigator.pushReplace))))
        {
            ((NavigatorState)navigator)._observedRouteAdditions.Enqueue(new _NavigatorReplaceObservation__navigator(this.route, previousPresent));
            if (((previousPresent is not null) && previousPresent._isPageBased))
            {
                var page = ((Page<object?>?)(object?)previousPresent.settings)!;
                navigator.widget.onDidRemovePage?.Invoke(page);
            }
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(previousState, _RouteLifecycle__navigator.push)));
            ((NavigatorState)navigator)._observedRouteAdditions.Enqueue(new _NavigatorPushObservation__navigator(this.route, previousPresent));
        }
    }

    public virtual void handleDidPopNext(RouteBase poppedRoute)
    {
        this.route.didPopNext(poppedRoute);
        lastAnnouncedPoppedNextRoute = new WeakReference<object>(poppedRoute);
        if ((this.lastFocusNode is not null))
        {
            DartRuntimePrimitives.Ignore(poppedRoute.disposalCompleted.then((global::System.Func<object, Future<object>>)(async (result) =>
            {
                switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
                {
                    case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        {
                            long? reFocusNode = this.lastFocusNode;
                            await new Future(NavigatorLibrary._kAndroidRefocusingDelayDuration);
                            await global::Doroti.Framework.Services.SystemChannels.accessibility.send(new global::Doroti.Framework.Semantics.FocusSemanticEvent().toMap(nodeId: reFocusNode));
                            break;
                        }
                    case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                        {
                            await global::Doroti.Framework.Services.SystemChannels.accessibility.send(new global::Doroti.Framework.Semantics.FocusSemanticEvent().toMap(nodeId: this.lastFocusNode));
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            })).catchError(((global::System.Action<object, global::System.Diagnostics.StackTrace>)((error, stackTrace) =>
            {
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stackTrace, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while restoring focus in the navigator")));
            }))));
        }
    }

    public virtual bool handlePop(NavigatorState navigator, RouteBase? previousPresent)
    {
        DartRuntimePrimitives.Assert(() => ((NavigatorState)navigator)._debugLocked);
        DartRuntimePrimitives.Assert(() => this.route._isInstalledIn(navigator));
        currentState = _RouteLifecycle__navigator.popping;
        if (this.route.popCompleted)
        {
            DartRuntimePrimitives.Assert(() => this.pageBased);
            DartRuntimePrimitives.Assert(() => (this.pendingResult is null));
            return true;
        }
        if (!this.route.didPopObject(this.pendingResult))
        {
            currentState = _RouteLifecycle__navigator.idle;
            return false;
        }
        this.route.onPopInvokedWithResultObject(true, this.pendingResult);
        if ((this.pageBased && this.imperativeRemoval))
        {
            var page = ((Page<object?>?)(object?)this.route.settings)!;
            navigator.widget.onDidRemovePage?.Invoke(page);
        }
        pendingResult = null;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleComplete()
    {
        this.route.didCompleteObject(this.pendingResult);
        pendingResult = null;
        DartRuntimePrimitives.Assert(() => this.route.popCompleted);
        currentState = _RouteLifecycle__navigator.remove;
    }

    public virtual void handleRemoval(NavigatorState navigator, RouteBase? previousPresent)
    {
        DartRuntimePrimitives.Assert(() => ((NavigatorState)navigator)._debugLocked);
        if (this.route._isInstalledIn(navigator))
        {
            currentState = _RouteLifecycle__navigator.removing;
        }
        else
        {
            currentState = _RouteLifecycle__navigator.dispose;
        }
        if (this._reportRemovalToObserver)
        {
            ((NavigatorState)navigator)._observedRouteDeletions.Enqueue(new _NavigatorRemoveObservation__navigator(this.route, previousPresent));
        }
    }

    public virtual void didAdd(NavigatorState navigator, bool isNewFirst)
    {
        DartRuntimePrimitives.Assert(() => !this.route._installed);
        this.route._navigator = navigator;
        this.route.install();
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this.route.overlayEntries));
        this.route.didAdd();
        currentState = _RouteLifecycle__navigator.idle;
        if (isNewFirst)
        {
            this.route.didChangeNext(null);
        }
    }

    public virtual void pop<T>(T? result, bool imperativeRemoval)
    {
        DartRuntimePrimitives.Assert(() => this.isPresent);
        pendingResult = result;
        currentState = _RouteLifecycle__navigator.pop;
        this.imperativeRemoval = imperativeRemoval;
    }

    public virtual void complete<T>(T result, bool isReplaced, bool imperativeRemoval)
    {
        if ((FoundationRuntimePorts.EnumIndex(this.currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.remove)))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => this.isPresent);
        _reportRemovalToObserver = !isReplaced;
        pendingResult = result;
        currentState = _RouteLifecycle__navigator.complete;
        this.imperativeRemoval = imperativeRemoval;
    }

    public virtual void finalize()
    {
        DartRuntimePrimitives.Assert(() => (FoundationRuntimePorts.EnumIndex(this.currentState) < FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.dispose)));
        currentState = _RouteLifecycle__navigator.dispose;
    }

    public virtual void forcedDispose()
    {
        DartRuntimePrimitives.Assert(() => (FoundationRuntimePorts.EnumIndex(this.currentState) < FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.disposed)));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        currentState = _RouteLifecycle__navigator.disposed;
        this.route.dispose();
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => (FoundationRuntimePorts.EnumIndex(this.currentState) < FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.disposing)));
        currentState = _RouteLifecycle__navigator.disposing;
        IEnumerable<OverlayEntry> mountedEntries = this.route.overlayEntries.where(((e) => ((OverlayEntry)e).mounted));
        if (!System.Linq.Enumerable.Any(mountedEntries))
        {
            forcedDispose();
            return;
        }
        long mountedLocal = mountedEntries.Count();
        DartRuntimePrimitives.Assert(() => (mountedLocal > 0L));
        NavigatorState navigator = this.route._navigator!;
        ((NavigatorState)navigator)._entryWaitingForSubTreeDisposal.Add(this);
        foreach (var entry in mountedEntries)
        {
            global::System.Action listener = default!;
            listener = (global::System.Action)(() =>
            {
                DartRuntimePrimitives.Assert(() => (mountedLocal > 0L));
                DartRuntimePrimitives.Assert(() => !((OverlayEntry)entry).mounted);
                mountedLocal--;
                entry.removeListener(listener);
                if ((mountedLocal == 0L))
                {
                    DartRuntimePrimitives.Assert(() => this.route.overlayEntries.All(((e) => !((OverlayEntry)e).mounted)));
                    DartAsyncRuntime.scheduleMicrotask((() =>
                    {
                        if (!((NavigatorState)navigator)._entryWaitingForSubTreeDisposal.Remove(this))
                        {
                            DartRuntimePrimitives.Assert(() => (!this.route._installed && !navigator.mounted));
                            return;
                        }
                        DartRuntimePrimitives.Assert(() => (object.Equals(this.currentState, _RouteLifecycle__navigator.disposing)));
                        forcedDispose();
                    }));
                    return;
                }
            });
            entry.addListener(listener);
        }
    }

    public virtual bool willBePresent
    {
        get
        {
            return ((FoundationRuntimePorts.EnumIndex(this.currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.idle)) && (FoundationRuntimePorts.EnumIndex(this.currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.add)));
            return default!;
        }
    }
    public virtual bool isPresent
    {
        get
        {
            return ((FoundationRuntimePorts.EnumIndex(this.currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.remove)) && (FoundationRuntimePorts.EnumIndex(this.currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.add)));
            return default!;
        }
    }
    public virtual bool isPresentForRestoration => DartRuntimePrimitives.ConvertValue<bool>((FoundationRuntimePorts.EnumIndex(this.currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.idle)));
    public virtual bool suitableForAnnouncement
    {
        get
        {
            return ((FoundationRuntimePorts.EnumIndex(this.currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.removing)) && (FoundationRuntimePorts.EnumIndex(this.currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.push)));
            return default!;
        }
    }
    public virtual bool suitableForTransitionAnimation
    {
        get
        {
            return ((FoundationRuntimePorts.EnumIndex(this.currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.remove)) && (FoundationRuntimePorts.EnumIndex(this.currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.push)));
            return default!;
        }
    }
    public virtual bool shouldAnnounceChangeToNext(dynamic nextRoute)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(nextRoute, this.lastAnnouncedNextRoute)));
        return !(((nextRoute is null) && (object.Equals(DartCoreExtensions.weakTarget(this.lastAnnouncedPoppedNextRoute), this.lastAnnouncedNextRoute))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isPresentPredicate(_RouteEntry__navigator entry) => ((_RouteEntry__navigator)entry).isPresent;
    public static bool suitableForTransitionAnimationPredicate(_RouteEntry__navigator entry) => ((_RouteEntry__navigator)entry).suitableForTransitionAnimation;
    public static bool willBePresentPredicate(_RouteEntry__navigator entry) => ((_RouteEntry__navigator)entry).willBePresent;
    public static global::System.Func<_RouteEntry__navigator, bool> isRoutePredicate(RouteBase route)
    {
        return ((global::System.Func<_RouteEntry__navigator, bool>)((entry) => (object.Equals(((_RouteEntry__navigator)entry).route, route))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isWaitingForEnteringDecision => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(this.currentState, _RouteLifecycle__navigator.staging)));
    public override bool isWaitingForExitingDecision => this._isWaitingForExitingDecision;
    public virtual void markNeedsExitingDecision() => _isWaitingForExitingDecision = true;
    public override void markForPush()
    {
        DartRuntimePrimitives.Assert(() => (this.isWaitingForEnteringDecision && !this.isWaitingForExitingDecision), () => (object?)"This route cannot be marked for push. Either a decision has already been " + "made or it does not require an explicit decision on how to transition in.");
        currentState = _RouteLifecycle__navigator.push;
    }

    public override void markForAdd()
    {
        DartRuntimePrimitives.Assert(() => (this.isWaitingForEnteringDecision && !this.isWaitingForExitingDecision), () => (object?)"This route cannot be marked for add. Either a decision has already been " + "made or it does not require an explicit decision on how to transition in.");
        currentState = _RouteLifecycle__navigator.add;
    }

    public override void markForPop(dynamic result = default!)
    {
        DartRuntimePrimitives.Assert(() => ((!this.isWaitingForEnteringDecision && this.isWaitingForExitingDecision) && this.isPresent), () => (object?)"This route cannot be marked for pop. Either a decision has already been " + "made or it does not require an explicit decision on how to transition out.");
        var attempt = 0L;
        while (this.route.willHandlePopInternally)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    attempt += 1L;
                    return (attempt < kDebugPopAttemptLimit);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }, () => (object?)$"Attempted to pop {this.route} {kDebugPopAttemptLimit} times, but still failed");
            bool popResult = this.route.didPopObject((object?)result);
            DartRuntimePrimitives.Assert(() => !popResult);
        }
        pop<object>((object?)result, imperativeRemoval: false);
        _isWaitingForExitingDecision = false;
    }

    public override void markForComplete(dynamic result = default!)
    {
        DartRuntimePrimitives.Assert(() => ((!this.isWaitingForEnteringDecision && this.isWaitingForExitingDecision) && this.isPresent), () => (object?)"This route cannot be marked for complete. Either a decision has already " + "been made or it does not require an explicit decision on how to transition " + "out.");
        complete<object>((object?)result, isReplaced: false, imperativeRemoval: false);
        _isWaitingForExitingDecision = false;
    }

    public virtual bool restorationEnabled
    {
        get => this.route.restorationScopeId.value is not null;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (!__value || (this.restorationId is not null)));
            this.route._updateRestorationId((__value ? this.restorationId : null));
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
        observer.didPush(this.primaryRoute, this.secondaryRoute);
    }

}

internal class _NavigatorPopObservation__navigator : _NavigatorObservation__navigator
{
    internal _NavigatorPopObservation__navigator(RouteBase primaryRoute, RouteBase? secondaryRoute) : base(primaryRoute, secondaryRoute)
    {
    }

    public override void notify(NavigatorObserver observer)
    {
        observer.didPop(this.primaryRoute, this.secondaryRoute);
    }

}

internal class _NavigatorRemoveObservation__navigator : _NavigatorObservation__navigator
{
    internal _NavigatorRemoveObservation__navigator(RouteBase primaryRoute, RouteBase? secondaryRoute) : base(primaryRoute, secondaryRoute)
    {
    }

    public override void notify(NavigatorObserver observer)
    {
        observer.didRemove(this.primaryRoute, this.secondaryRoute);
    }

}

internal class _NavigatorReplaceObservation__navigator : _NavigatorObservation__navigator
{
    internal _NavigatorReplaceObservation__navigator(RouteBase primaryRoute, RouteBase? secondaryRoute) : base(primaryRoute, secondaryRoute)
    {
    }

    public override void notify(NavigatorObserver observer)
    {
        observer.didReplace(newRoute: this.primaryRoute, oldRoute: this.secondaryRoute);
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
        return this._value.indexWhere(test, start);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void add(_RouteEntry__navigator element)
    {
        this._value.Add(element);
        notifyListeners();
    }

    public virtual void addAll(IEnumerable<_RouteEntry__navigator> elements)
    {
        this._value.AddRange(elements.Cast<_RouteEntry__navigator>());
        if (System.Linq.Enumerable.Any(elements))
        {
            notifyListeners();
        }
    }

    public virtual void clear()
    {
        bool valueWasEmpty = !System.Linq.Enumerable.Any(this._value);
        this._value.Clear();
        if (!valueWasEmpty)
        {
            notifyListeners();
        }
    }

    public virtual void insert(long index, _RouteEntry__navigator element)
    {
        this._value.Insert(checked((int)index), element);
        notifyListeners();
    }

    public virtual _RouteEntry__navigator removeAt(long index)
    {
        _RouteEntry__navigator entry = ((_RouteEntry__navigator)(object?)this._value.removeAt(index));
        notifyListeners();
        return entry;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _RouteEntry__navigator removeLast()
    {
        _RouteEntry__navigator entry = ((_RouteEntry__navigator)(object?)this._value.removeLast<_RouteEntry__navigator>());
        notifyListeners();
        return entry;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public _RouteEntry__navigator this[long index]
    {
        get
        {
            return this._value[(int)(index)];
            return default!;
        }
    }

    public virtual IEnumerator<_RouteEntry__navigator> GetEnumerator()
    {
        return this._value.GetEnumerator();
        return default!;
    }
    public override string ToString()
    {
        return ((string)(object?)this._value.ToString());
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
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    internal virtual bool _usingPagesAPI => !ReferenceEquals(((Navigator)(object)this.widget).pages, Navigator._defaultPages);
    internal virtual void _handleHistoryChanged()
    {
        switch (global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase)
        {
            case global::Doroti.Framework.Scheduler.SchedulerPhase.postFrameCallbacks:
                {
                    new NavigationNotification(canHandlePop: _getNavigatorCanHandlePop()).dispatch(this.context);
                    break;
                }
            case global::Doroti.Framework.Scheduler.SchedulerPhase.idle:
            case global::Doroti.Framework.Scheduler.SchedulerPhase.midFrameMicrotasks:
            case global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks:
            case global::Doroti.Framework.Scheduler.SchedulerPhase.transientCallbacks:
                {
                    global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) =>
                    {
                        if (!this.mounted)
                        {
                            return;
                        }
                        new NavigationNotification(canHandlePop: _getNavigatorCanHandlePop()).dispatch(this.context);
                    })), debugLabel: "Navigator.dispatchNotification");
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
        _RouteEntry__navigator? lastEntry = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
        return ((lastEntry is not null) && (object.Equals(((_RouteEntry__navigator)lastEntry).route.popDisposition, RoutePopDisposition.doNotPop)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckPageApiParameters()
    {
        if (!this._usingPagesAPI)
        {
            return true;
        }
        if (!System.Linq.Enumerable.Any(((Navigator)(object)this.widget).pages))
        {
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Framework.Foundation.FlutterError.Create("The Navigator.pages must not be empty to use the " + "Navigator.pages API"), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
        }
        else
        {
            if ((((((Navigator)(object)this.widget).onDidRemovePage is null)) == ((((Navigator)(object)this.widget).onPopPage is null))))
            {
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Framework.Foundation.FlutterError.Create("Either onDidRemovePage or onPopPage must be provided to use the " + "Navigator.pages API but not both."), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        base.initState();
        DartRuntimePrimitives.Assert(() => _debugCheckPageApiParameters());
        foreach (NavigatorObserver observer in ((Navigator)(object)this.widget).observers)
        {
            DartRuntimePrimitives.Assert(() => (((NavigatorObserver)observer).navigator is null));
            NavigatorObserver._navigators[observer] = this;
        }
        _effectiveObservers = ((Navigator)(object)this.widget).observers;
        var heroControllerScope = ((HeroControllerScope?)(object?)this.context.getElementForInheritedWidgetOfExactType<HeroControllerScope>()?.widget)!;
        _updateHeroController(heroControllerScope?.controller);
        if (((Navigator)(object)this.widget).reportsRouteUpdateToEngine)
        {
            DartRuntimePrimitives.Ignore(SystemNavigator.selectSingleEntryHistory());
        }
        global::Doroti.Framework.Services.ServicesBinding.instance.accessibilityFocus.addListener(this._recordLastFocus);
        this._history.addListener(this._handleHistoryChanged);
    }

    internal virtual void _recordLastFocus()
    {
        _RouteEntry__navigator? entry = this._history.where(_RouteEntry__navigator.isPresentPredicate).LastOrDefault();
        entry?.lastFocusNode = global::Doroti.Framework.Services.ServicesBinding.instance.accessibilityFocus.value;
    }

    internal virtual long _nextPagelessRestorationScopeId => DartRuntimePrimitives.ConvertValue<long>(this._rawNextPagelessRestorationScopeId.value++);
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(this._rawNextPagelessRestorationScopeId, "id");
        registerForRestoration(this._serializableHistory, "history");
        _forcedDisposeAllRouteEntries();
        DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._history));
        _overlayKey = new GlobalKey<OverlayState>();
        this._history.addAll(this._serializableHistory.restoreEntriesForPage(((_RouteEntry__navigator)(object)null), this));
        foreach (Page<object> page in ((Navigator)(object)this.widget).pages)
        {
            var entry = new _RouteEntry__navigator(page.createRoute(this.context), pageBased: true, initialState: _RouteLifecycle__navigator.add);
            DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry).route.settings, page)), () => (object?)"The settings getter of a page-based Route must return a Page object. " + "Please set the settings to the Page in the Page.createRoute method.");
            this._history.add(entry);
            this._history.addAll(this._serializableHistory.restoreEntriesForPage(entry, this));
        }
        if (!((_HistoryProperty__navigator)this._serializableHistory).hasData)
        {
            string? initialRouteLocal = ((Navigator)(object)this.widget).initialRoute;
            if (!System.Linq.Enumerable.Any(((Navigator)(object)this.widget).pages))
            {
                initialRouteLocal ??= Navigator.defaultRouteName;
            }
            if ((initialRouteLocal is not null))
            {
                this._history.addAll(this.widget.onGenerateInitialRoutes(this, (((Navigator)(object)this.widget).initialRoute ?? Navigator.defaultRouteName)).map<dynamic, _RouteEntry__navigator>(((route) =>
                {
                    RouteBase typedRoute = Navigator._requireRoute(route);
                    return new _RouteEntry__navigator(typedRoute, pageBased: false, initialState: _RouteLifecycle__navigator.add, restorationInformation: ((typedRoute.settings.ToString() is not null) ? _RestorationInformation__navigator.CreateNamed(name: typedRoute.settings.ToString()!, arguments: null, restorationScopeId: this._nextPagelessRestorationScopeId) : null));
                })).Cast<_RouteEntry__navigator>());
            }
        }
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this._history), () => (object?)"All routes returned by onGenerateInitialRoutes are not restorable. " + "Please make sure that all routes returned by onGenerateInitialRoutes " + "have their RouteSettings defined with names that are defined in the " + "app's routes table.");
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
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
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
        if ((this.bucket is not null))
        {
            this._serializableHistory.update(this._history);
        }
        else
        {
            this._serializableHistory.clear();
        }
    }

    public virtual string? restorationId => ((Navigator)(object)this.widget).restorationScopeId;
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        _currentParent = RestorationScope.maybeOf(this.context);
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
        _updateHeroController(HeroControllerScope.maybeOf(this.context));
        foreach (_RouteEntry__navigator entry in this._history)
        {
            if ((object.Equals(((_RouteEntry__navigator)entry).route.navigator, this)))
            {
                ((_RouteEntry__navigator)entry).route.changedExternalState();
            }
        }
    }

    internal virtual void _forcedDisposeAllRouteEntries()
    {
        this._entryWaitingForSubTreeDisposal.removeWhere(((entry) =>
        {
            entry.forcedDispose();
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        while (System.Linq.Enumerable.Any(this._history))
        {
            NavigatorState._disposeRouteEntry(this._history.removeLast(), graceful: false);
        }
    }

    internal static void _disposeRouteEntry(_RouteEntry__navigator entry, bool graceful)
    {
        foreach (OverlayEntry overlayEntry in ((_RouteEntry__navigator)entry).route.overlayEntries)
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
        if ((!object.Equals(this._heroControllerFromScope, newHeroController)))
        {
            if ((newHeroController is not null))
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        if ((newHeroController.navigator is not null))
                        {
                            NavigatorState previousOwner = newHeroController.navigator!;
                            global::Doroti.Framework.Services.ServicesBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) =>
                            {
                                if ((object.Equals(this._heroControllerFromScope, newHeroController)))
                                {
                                    var hasHeroControllerOwnerShip = (object.Equals(this._heroControllerFromScope!.navigator, this));
                                    if ((!hasHeroControllerOwnerShip || (object.Equals(((NavigatorState)previousOwner)._heroControllerFromScope, newHeroController))))
                                    {
                                        NavigatorState otherOwner = (hasHeroControllerOwnerShip ? previousOwner : this._heroControllerFromScope!.navigator!);
                                        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Framework.Foundation.FlutterError.Create("A HeroController can not be shared by multiple Navigators. " + "The Navigators that share the same HeroController are:\n" + $"- {this}\n" + $"- {otherOwner}\n" + "Please create a HeroControllerScope for each Navigator or " + "use a HeroControllerScope.none to prevent subtree from " + "receiving a HeroController."), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
                                    }
                                }
                            })), debugLabel: "Navigator.checkHeroControllerOwnership");
                        }
                        return true;
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                NavigatorObserver._navigators[newHeroController] = this;
            }
            if ((object.Equals(this._heroControllerFromScope?.navigator, this)))
            {
                NavigatorObserver._navigators[this._heroControllerFromScope!] = DartRuntimePrimitives.ConvertValue<NavigatorState>(null);
            }
            _heroControllerFromScope = newHeroController;
            _updateEffectiveObservers();
        }
    }

    internal virtual void _updateEffectiveObservers()
    {
        if ((this._heroControllerFromScope is not null))
        {
            _effectiveObservers = (((Navigator)(object)this.widget).observers.Concat(new List<NavigatorObserver> { this._heroControllerFromScope! }).ToList());
        }
        else
        {
            _effectiveObservers = ((Navigator)(object)this.widget).observers;
        }
    }

    public override void didUpdateWidget(Navigator oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        DartRuntimePrimitives.Assert(() => _debugCheckPageApiParameters());
        if ((!object.Equals(((Navigator)oldWidget).observers, ((Navigator)(object)this.widget).observers)))
        {
            foreach (NavigatorObserver observer in ((Navigator)oldWidget).observers)
            {
                NavigatorObserver._navigators[observer] = DartRuntimePrimitives.ConvertValue<NavigatorState>(null);
            }
            foreach (NavigatorObserver observerLocal in ((Navigator)(object)this.widget).observers)
            {
                DartRuntimePrimitives.Assert(() => (((NavigatorObserver)observerLocal).navigator is null));
                NavigatorObserver._navigators[observerLocal] = this;
            }
            _updateEffectiveObservers();
        }
        if (((!object.Equals(((Navigator)oldWidget).pages, ((Navigator)(object)this.widget).pages)) && !this.restorePending))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (!System.Linq.Enumerable.Any(((Navigator)(object)this.widget).pages))
                    {
                        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Framework.Foundation.FlutterError.Create("The Navigator.pages must not be empty to use the " + "Navigator.pages API"), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            _updatePages();
        }
        foreach (_RouteEntry__navigator entry in this._history)
        {
            if ((object.Equals(((_RouteEntry__navigator)entry).route.navigator, this)))
            {
                ((_RouteEntry__navigator)entry).route.changedExternalState();
            }
        }
    }

    internal virtual void _debugCheckDuplicatedPageKeys()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var keyReservation = new HashSet<global::Doroti.Framework.Foundation.Key>();
                foreach (Page<object> page in ((Navigator)(object)this.widget).pages)
                {
                    global::Doroti.Framework.Foundation.LocalKey? keyLocal = ((Page<object>)page).key;
                    if ((keyLocal is not null))
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
        foreach (NavigatorObserver observer in this._effectiveObservers)
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
        foreach (NavigatorObserver observer in this._effectiveObservers)
        {
            DartRuntimePrimitives.Assert(() => (((NavigatorObserver)observer).navigator is null));
            NavigatorObserver._navigators[observer] = this;
        }
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._effectiveObservers));
        _updateHeroController(((HeroController)(object)null));
        this.focusNode.dispose();
        _forcedDisposeAllRouteEntries();
        this._rawNextPagelessRestorationScopeId.dispose();
        this._serializableHistory.dispose();
        this.userGestureInProgressNotifier.dispose();
        global::Doroti.Framework.Services.ServicesBinding.instance.accessibilityFocus.removeListener(this._recordLastFocus);
        this._history.removeListener(this._handleHistoryChanged);
        this._history.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) =>
        {
            if (!((dynamic)property)._disposed)
            {
                property.removeListener(listener);
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
        DartRuntimePrimitives.Assert(() => this._debugLocked);
    }

    public virtual OverlayState? overlay => ((GlobalKey<OverlayState>)this._overlayKey).currentState;
    internal virtual IEnumerable<OverlayEntry> _allRouteOverlayEntries
    {
        get
        {
            return this._history
                .where(_RouteEntry__navigator.isPresentPredicate)
                .SelectMany(entry => entry.route.overlayEntries);
        }
    }
    internal virtual void _updatePages()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => !this._debugUpdatingPage);
                _debugCheckDuplicatedPageKeys();
                _debugUpdatingPage = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        var needsExplicitDecision = false;
        var newPagesBottom = 0L;
        var oldEntriesBottom = 0L;
        long newPagesTop = (checked((long)(((Navigator)(object)this.widget).pages.Count)) - 1L);
        long oldEntriesTop = (this._history.Count() - 1L);
        var newHistory = new List<_RouteEntry__navigator>();
        var pageRouteToPagelessRoutesLocal = new DartMap<_RouteEntry__navigator?, List<_RouteEntry__navigator>>();
        _RouteEntry__navigator? previousOldPageRouteEntry = default!;
        while ((oldEntriesBottom <= oldEntriesTop))
        {
            _RouteEntry__navigator oldEntry = this._history[oldEntriesBottom];
            DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)oldEntry).currentState, _RouteLifecycle__navigator.disposed)));
            if (!((_RouteEntry__navigator)oldEntry).pageBased)
            {
                List<_RouteEntry__navigator> pagelessRoutes = pageRouteToPagelessRoutesLocal.putIfAbsent(previousOldPageRouteEntry, (() => new List<_RouteEntry__navigator>())).ToList();
                pagelessRoutes.Add(oldEntry);
                oldEntriesBottom += 1L;
                continue;
            }
            if ((newPagesBottom > newPagesTop))
            {
                break;
            }
            Page<object> newPage = ((Navigator)(object)this.widget).pages[(int)(newPagesBottom)];
            if (!oldEntry.canUpdateFrom(newPage))
            {
                break;
            }
            previousOldPageRouteEntry = oldEntry;
            ((_RouteEntry__navigator)oldEntry).route._updateSettings(newPage);
            newHistory.Add(oldEntry);
            newPagesBottom += 1L;
            oldEntriesBottom += 1L;
        }
        var unattachedPagelessRoutes = new List<_RouteEntry__navigator>();
        while ((((oldEntriesBottom <= oldEntriesTop)) && ((newPagesBottom <= newPagesTop))))
        {
            _RouteEntry__navigator oldEntryLocal = this._history[oldEntriesTop];
            DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)oldEntryLocal).currentState, _RouteLifecycle__navigator.disposed)));
            if (!((_RouteEntry__navigator)oldEntryLocal).pageBased)
            {
                unattachedPagelessRoutes.Add(oldEntryLocal);
                oldEntriesTop -= 1L;
                continue;
            }
            Page<object> newPageLocal = ((Navigator)(object)this.widget).pages[(int)(newPagesTop)];
            if (!oldEntryLocal.canUpdateFrom(newPageLocal))
            {
                break;
            }
            if (System.Linq.Enumerable.Any(unattachedPagelessRoutes))
            {
                pageRouteToPagelessRoutesLocal.putIfAbsent(oldEntryLocal, (() => new List<_RouteEntry__navigator>(DartRuntimePrimitives.ConvertEnumerable<_RouteEntry__navigator>(unattachedPagelessRoutes))));
                unattachedPagelessRoutes.Clear();
            }
            oldEntriesTop -= 1L;
            newPagesTop -= 1L;
        }
        oldEntriesTop += checked((long)(unattachedPagelessRoutes.Count));
        var oldEntriesBottomToScan = oldEntriesBottom;
        var pageKeyToOldEntry = new DartMap<global::Doroti.Framework.Foundation.LocalKey, _RouteEntry__navigator>();
        var phantomEntries = new HashSet<_RouteEntry__navigator>();
        while ((oldEntriesBottomToScan <= oldEntriesTop))
        {
            _RouteEntry__navigator oldEntryAlternate = this._history[oldEntriesBottomToScan];
            oldEntriesBottomToScan += 1L;
            DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)oldEntryAlternate).currentState, _RouteLifecycle__navigator.disposed)));
            if (!((_RouteEntry__navigator)oldEntryAlternate).pageBased)
            {
                continue;
            }
            var page = ((Page<object>?)(object?)((_RouteEntry__navigator)oldEntryAlternate).route.settings)!;
            if ((((Page<object>)page).key is null))
            {
                continue;
            }
            if (!((_RouteEntry__navigator)oldEntryAlternate).willBePresent)
            {
                phantomEntries.Add(oldEntryAlternate);
                continue;
            }
            DartRuntimePrimitives.Assert(() => !pageKeyToOldEntry.ContainsKey(((Page<object>)page).key));
            pageKeyToOldEntry[((Page<object>)page).key!] = oldEntryAlternate;
        }
        while ((newPagesBottom <= newPagesTop))
        {
            Page<object> nextPage = ((Navigator)(object)this.widget).pages[(int)(newPagesBottom)];
            newPagesBottom += 1L;
            if ((((((Page<object>)nextPage).key is null) || !pageKeyToOldEntry.ContainsKey(((Page<object>)nextPage).key)) || !pageKeyToOldEntry.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((Page<object>)nextPage).key))!.canUpdateFrom(nextPage)))
            {
                var newEntry = new _RouteEntry__navigator(nextPage.createRoute(this.context), pageBased: true, initialState: _RouteLifecycle__navigator.staging);
                needsExplicitDecision = true;
                DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)newEntry).route.settings, nextPage)), () => (object?)"The settings getter of a page-based Route must return a Page object. " + "Please set the settings to the Page in the Page.createRoute method.");
                newHistory.Add(newEntry);
            }
            else
            {
                _RouteEntry__navigator matchingEntry = pageKeyToOldEntry.remove(((Page<object>)nextPage).key)!;
                DartRuntimePrimitives.Assert(() => matchingEntry.canUpdateFrom(nextPage));
                ((_RouteEntry__navigator)matchingEntry).route._updateSettings(nextPage);
                newHistory.Add(matchingEntry);
            }
        }
        var locationToExitingPageRouteLocal = new DartMap<RouteTransitionRecord?, RouteTransitionRecord>();
        while ((oldEntriesBottom <= oldEntriesTop))
        {
            _RouteEntry__navigator potentialEntryToRemove = this._history[oldEntriesBottom];
            oldEntriesBottom += 1L;
            if (!((_RouteEntry__navigator)potentialEntryToRemove).pageBased)
            {
                DartRuntimePrimitives.Assert(() => (previousOldPageRouteEntry is not null));
                List<_RouteEntry__navigator> pagelessRoutesLocal = pageRouteToPagelessRoutesLocal.putIfAbsent(previousOldPageRouteEntry, (() => new List<_RouteEntry__navigator>())).ToList();
                pagelessRoutesLocal.Add(potentialEntryToRemove);
                if ((previousOldPageRouteEntry!.isWaitingForExitingDecision && ((_RouteEntry__navigator)potentialEntryToRemove).willBePresent))
                {
                    potentialEntryToRemove.markNeedsExitingDecision();
                }
                continue;
            }
            var potentialPageToRemove = ((Page<object>?)(object?)((_RouteEntry__navigator)potentialEntryToRemove).route.settings)!;
            if ((((((Page<object>)potentialPageToRemove).key is null) || pageKeyToOldEntry.ContainsKey(((Page<object>)potentialPageToRemove).key)) || phantomEntries.Contains(potentialEntryToRemove)))
            {
                locationToExitingPageRouteLocal[DartRuntimePrimitives.RequireReference(previousOldPageRouteEntry)] = DartRuntimePrimitives.ConvertValue<RouteTransitionRecord>(potentialEntryToRemove);
                if (((_RouteEntry__navigator)potentialEntryToRemove).willBePresent)
                {
                    potentialEntryToRemove.markNeedsExitingDecision();
                }
            }
            previousOldPageRouteEntry = potentialEntryToRemove;
        }
        DartRuntimePrimitives.Assert(() => (oldEntriesBottom == (oldEntriesTop + 1L)));
        DartRuntimePrimitives.Assert(() => (newPagesBottom == (newPagesTop + 1L)));
        newPagesTop = (checked((long)(((Navigator)(object)this.widget).pages.Count)) - 1L);
        oldEntriesTop = (this._history.Count() - 1L);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((oldEntriesBottom <= oldEntriesTop))
                {
                    return (((newPagesBottom <= newPagesTop) && this._history[oldEntriesBottom].pageBased) && this._history[oldEntriesBottom].canUpdateFrom(((Navigator)(object)this.widget).pages[(int)(newPagesBottom)]));
                }
                else
                {
                    return (newPagesBottom > newPagesTop);
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        while ((((oldEntriesBottom <= oldEntriesTop)) && ((newPagesBottom <= newPagesTop))))
        {
            _RouteEntry__navigator oldEntryNested = this._history[oldEntriesBottom];
            DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)oldEntryNested).currentState, _RouteLifecycle__navigator.disposed)));
            if (!((_RouteEntry__navigator)oldEntryNested).pageBased)
            {
                DartRuntimePrimitives.Assert(() => (previousOldPageRouteEntry is not null));
                List<_RouteEntry__navigator> pagelessRoutesAlternate = pageRouteToPagelessRoutesLocal.putIfAbsent(previousOldPageRouteEntry, (() => new List<_RouteEntry__navigator>())).ToList();
                pagelessRoutesAlternate.Add(oldEntryNested);
                continue;
            }
            previousOldPageRouteEntry = oldEntryNested;
            Page<object> newPageAlternate = ((Navigator)(object)this.widget).pages[(int)(newPagesBottom)];
            DartRuntimePrimitives.Assert(() => oldEntryNested.canUpdateFrom(newPageAlternate));
            ((_RouteEntry__navigator)oldEntryNested).route._updateSettings(newPageAlternate);
            newHistory.Add(oldEntryNested);
            oldEntriesBottom += 1L;
            newPagesBottom += 1L;
        }
        needsExplicitDecision = (needsExplicitDecision || System.Linq.Enumerable.Any(locationToExitingPageRouteLocal));
        IEnumerable<_RouteEntry__navigator> results = ((IEnumerable<_RouteEntry__navigator>)(object?)newHistory);
        if (needsExplicitDecision)
        {
            results = ((Navigator)(object)this.widget).transitionDelegate._transition(newPageRouteHistory: newHistory.Cast<RouteTransitionRecord>().ToList(), locationToExitingPageRoute: locationToExitingPageRouteLocal, pageRouteToPagelessRoutes: pageRouteToPagelessRoutesLocal.cast<RouteTransitionRecord?, List<RouteTransitionRecord>>()).cast<_RouteEntry__navigator>();
        }
        this._history.clear();
        if (pageRouteToPagelessRoutesLocal.ContainsKey(((_RouteEntry__navigator)(object)null)))
        {
            this._history.addAll(pageRouteToPagelessRoutesLocal.GetValueOrDefault(null)!.Cast<_RouteEntry__navigator>());
        }
        foreach (var result in results)
        {
            this._history.add(result);
            if (pageRouteToPagelessRoutesLocal.ContainsKey(result))
            {
                this._history.addAll(pageRouteToPagelessRoutesLocal.GetValueOrDefault(result)!.Cast<_RouteEntry__navigator>());
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
        DartRuntimePrimitives.Assert(() => (this._debugLocked && !this._debugUpdatingPage));
        _flushingHistory = true;
        long index = (this._history.Count() - 1L);
        _RouteEntry__navigator? next = default!;
        _RouteEntry__navigator? entry = this._history[index];
        _RouteEntry__navigator? previousLocal = ((index > 0L) ? this._history[(index - 1L)] : null);
        var canRemoveOrAdd = false;
        RouteBase? poppedRoute = default;
        var seenTopActiveRoute = false;
        var toBeDisposed = new List<_RouteEntry__navigator>();
        while ((index >= 0L))
        {
            switch (entry!.currentState)
            {
                case _RouteLifecycle__navigator.add:
                    {
                        DartRuntimePrimitives.Assert(() => rearrangeOverlay);
                        entry.handleAdd(navigator: this, previousPresent: _getRouteBefore((index - 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate)?.route);
                        DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.adding)));
                        continue;
                    }
                case _RouteLifecycle__navigator.adding:
                    {
                        if ((canRemoveOrAdd || (next is null)))
                        {
                            entry.didAdd(navigator: this, isNewFirst: (next is null));
                            DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.idle)));
                            continue;
                        }
                        break;
                    }
                case _RouteLifecycle__navigator.push:
                case _RouteLifecycle__navigator.pushReplace:
                case _RouteLifecycle__navigator.replace:
                    {
                        DartRuntimePrimitives.Assert(() => rearrangeOverlay);
                        entry.handlePush(navigator: this, previous: previousLocal?.route, previousPresent: _getRouteBefore((index - 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate)?.route, isNewFirst: (next is null));
                        DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.push)));
                        DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.pushReplace)));
                        DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.replace)));
                        if ((object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.idle)))
                        {
                            continue;
                        }
                        break;
                    }
                case _RouteLifecycle__navigator.pushing:
                    {
                        if ((!seenTopActiveRoute && (poppedRoute is not null)))
                        {
                            entry.handleDidPopNext(poppedRoute);
                        }
                        seenTopActiveRoute = true;
                        break;
                    }
                case _RouteLifecycle__navigator.idle:
                    {
                        if ((!seenTopActiveRoute && (poppedRoute is not null)))
                        {
                            entry.handleDidPopNext(poppedRoute);
                        }
                        seenTopActiveRoute = true;
                        canRemoveOrAdd = true;
                        break;
                    }
                case _RouteLifecycle__navigator.pop:
                    {
                        if (!entry.handlePop(navigator: this, previousPresent: _getRouteBefore(index, (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.willBePresentPredicate)?.route))
                        {
                            DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.idle)));
                            continue;
                        }
                        if (!seenTopActiveRoute)
                        {
                            if ((poppedRoute is not null))
                            {
                                entry.handleDidPopNext(poppedRoute);
                            }
                            poppedRoute = ((_RouteEntry__navigator)entry).route;
                        }
                        this._observedRouteDeletions.Enqueue(new _NavigatorPopObservation__navigator(((_RouteEntry__navigator)entry).route, _getRouteBefore(index, (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.willBePresentPredicate)?.route));
                        if ((object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.dispose)))
                        {
                            continue;
                        }
                        DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.popping)));
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
                        DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.remove)));
                        continue;
                    }
                case _RouteLifecycle__navigator.remove:
                    {
                        if ((!seenTopActiveRoute && ((_RouteEntry__navigator)entry).route._installed))
                        {
                            if ((poppedRoute is not null))
                            {
                                entry.handleDidPopNext(poppedRoute);
                            }
                            poppedRoute = null;
                        }
                        entry.handleRemoval(navigator: this, previousPresent: _getRouteBefore(index, (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.willBePresentPredicate)?.route);
                        DartRuntimePrimitives.Assert(() => (FoundationRuntimePorts.EnumIndex(((_RouteEntry__navigator)entry).currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.removing)));
                        continue;
                    }
                case _RouteLifecycle__navigator.removing:
                    {
                        if ((!canRemoveOrAdd && (next is not null)))
                        {
                            break;
                        }
                        entry.currentState = _RouteLifecycle__navigator.dispose;
                        continue;
                    }
                case _RouteLifecycle__navigator.dispose:
                    {
                        toBeDisposed.Add(this._history.removeAt(index));
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
            previousLocal = ((index > 0L) ? this._history[(index - 1L)] : null);
        }
        _flushObserverNotifications();
        _flushRouteAnnouncement();
        _RouteEntry__navigator? lastEntry = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
        if (((lastEntry is not null) && (!object.Equals(this._lastTopmostRoute, lastEntry))))
        {
            foreach (NavigatorObserver observer in this._effectiveObservers)
            {
                observer.didChangeTop(((_RouteEntry__navigator)lastEntry).route, this._lastTopmostRoute?.route);
            }
        }
        _lastTopmostRoute = lastEntry;
        if (((Navigator)(object)this.widget).reportsRouteUpdateToEngine)
        {
            string? routeName = lastEntry?.route.settings.ToString();
            if (((routeName is not null) && (routeName != this._lastAnnouncedRouteName)))
            {
                DartRuntimePrimitives.Ignore(SystemNavigator.routeInformationUpdated(uri: DartUri.parse(routeName)));
                _lastAnnouncedRouteName = routeName;
            }
        }
        foreach (var entryLocal in toBeDisposed)
        {
            NavigatorState._disposeRouteEntry(entryLocal, graceful: true);
        }
        if (rearrangeOverlay)
        {
            this.overlay?.rearrange(this._allRouteOverlayEntries.Cast<OverlayEntry>());
        }
        if ((this.bucket is not null))
        {
            this._serializableHistory.update(this._history);
        }
        _flushingHistory = false;
    }

    internal virtual void _flushObserverNotifications()
    {
        if (!System.Linq.Enumerable.Any(this._effectiveObservers))
        {
            this._observedRouteDeletions.Clear();
            this._observedRouteAdditions.Clear();
            return;
        }
        while (System.Linq.Enumerable.Any(this._observedRouteAdditions))
        {
            _NavigatorObservation__navigator observation = this._observedRouteAdditions.removeLast<_NavigatorObservation__navigator>();
            this._effectiveObservers.forEach((__arg0) => ((global::System.Action<NavigatorObserver>)((_NavigatorObservation__navigator)observation).notify)(__arg0));
        }
        while (System.Linq.Enumerable.Any(this._observedRouteDeletions))
        {
            _NavigatorObservation__navigator observationLocal = this._observedRouteDeletions.Dequeue();
            this._effectiveObservers.forEach((__arg0) => ((global::System.Action<NavigatorObserver>)((_NavigatorObservation__navigator)observationLocal).notify)(__arg0));
        }
    }

    internal virtual void _flushRouteAnnouncement()
    {
        long index = (this._history.Count() - 1L);
        while ((index >= 0L))
        {
            _RouteEntry__navigator entry = this._history[index];
            if (!((_RouteEntry__navigator)entry).suitableForAnnouncement)
            {
                index -= 1L;
                continue;
            }
            _RouteEntry__navigator? next = ((_RouteEntry__navigator?)(object?)_getRouteAfter((index + 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.suitableForTransitionAnimationPredicate));
            if ((!object.Equals(next?.route, ((_RouteEntry__navigator)entry).lastAnnouncedNextRoute)))
            {
                if (entry.shouldAnnounceChangeToNext(next?.route))
                {
                    ((_RouteEntry__navigator)entry).route.didChangeNext(next?.route);
                }
                entry.lastAnnouncedNextRoute = next?.route;
            }
            _RouteEntry__navigator? previous = ((_RouteEntry__navigator?)(object?)_getRouteBefore((index - 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.suitableForTransitionAnimationPredicate));
            if ((!object.Equals(previous?.route, ((_RouteEntry__navigator)entry).lastAnnouncedPreviousRoute)))
            {
                ((_RouteEntry__navigator)entry).route.didChangePrevious(previous?.route);
                entry.lastAnnouncedPreviousRoute = previous?.route;
            }
            index -= 1L;
        }
    }

    internal virtual _RouteEntry__navigator? _getRouteBefore(long index, global::System.Func<_RouteEntry__navigator, bool> predicate)
    {
        index = _getIndexBefore(index, (global::System.Func<_RouteEntry__navigator, bool>)predicate);
        return ((index >= 0L) ? this._history[index] : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _getIndexBefore(long index, global::System.Func<_RouteEntry__navigator, bool> predicate)
    {
        while (((index >= 0L) && !predicate(this._history[index])))
        {
            index -= 1L;
        }
        return index;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _RouteEntry__navigator? _getRouteAfter(long index, global::System.Func<_RouteEntry__navigator, bool> predicate)
    {
        while (((index < this._history.Count()) && !predicate(this._history[index])))
        {
            index += 1L;
        }
        return ((index < this._history.Count()) ? this._history[index] : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Route<T?>? _routeNamed<T>(string name, object? arguments, bool allowNull = false)
    {
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        if ((allowNull && (((Navigator)(object)this.widget).onGenerateRoute is null)))
        {
            return default;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((Navigator)(object)this.widget).onGenerateRoute is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"Navigator.onGenerateRoute was null, but the route named \"{name}\" was referenced.\n" + "To use the Navigator API with named routes (pushNamed, pushReplacementNamed, or " + "pushNamedAndRemoveUntil), the Navigator must be provided with an " + "onGenerateRoute handler.\n" + "The Navigator was:\n" + $"  {this}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        var settings = new RouteSettings(name: name, arguments: arguments);
        var route = ((Route<T?>?)(object?)((Navigator)(object)this.widget).onGenerateRoute!(settings))!;
        if (((route is null) && !allowNull))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((((Navigator)(object)this.widget).onUnknownRoute is null))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"Navigator.onGenerateRoute returned null when requested to build route \"{name}\"."), new global::Doroti.Framework.Foundation.ErrorDescription("The onGenerateRoute callback must never return null, unless an onUnknownRoute " + "callback is provided as well."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<NavigatorState>("The Navigator was", this, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            route = ((Route<T?>?)(object?)((Navigator)(object)this.widget).onUnknownRoute!(settings))!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((route is null))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"Navigator.onUnknownRoute returned null when requested to build route \"{name}\"."), new global::Doroti.Framework.Foundation.ErrorDescription("The onUnknownRoute callback must never return null."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<NavigatorState>("The Navigator was", this, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        DartRuntimePrimitives.Assert(() => ((route is not null) || allowNull));
        return route;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T?> pushNamed<T>(string routeName, object? arguments = null)
    {
        return ((Future<T?>)(object?)push<T?>(_routeNamed<T>(routeName, arguments: arguments)!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePushNamed<T>(string routeName, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateNamed(name: routeName, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.push));
        _pushEntry(entry);
        return ((_RouteEntry__navigator)entry).restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T?> pushReplacementNamed<T, TO>(string routeName, TO? result = default, object? arguments = null)
    {
        return ((Future<T?>)(object?)pushReplacement<T?, TO>(_routeNamed<T>(routeName, arguments: arguments)!, result: result));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePushReplacementNamed<T, TO>(string routeName, TO? result = default, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateNamed(name: routeName, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.pushReplace));
        _pushReplacementEntry(entry, result);
        return ((_RouteEntry__navigator)entry).restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T?> popAndPushNamed<T, TO>(string routeName, TO? result = default, object? arguments = null)
    {
        pop<TO>(result);
        return ((Future<T?>)(object?)pushNamed<T>(routeName, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePopAndPushNamed<T, TO>(string routeName, TO? result = default, object? arguments = null)
    {
        pop<TO>(result);
        return ((string)(object?)restorablePushNamed<object>(routeName, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T?> pushNamedAndRemoveUntil<T>(string newRouteName, global::System.Func<dynamic, bool> predicate, object? arguments = null)
    {
        return ((Future<T?>)(object?)pushAndRemoveUntil<T?>(_routeNamed<T>(newRouteName, arguments: arguments)!, (global::System.Func<dynamic, bool>)predicate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePushNamedAndRemoveUntil<T>(string newRouteName, global::System.Func<dynamic, bool> predicate, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateNamed(name: newRouteName, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.push));
        _pushEntryAndRemoveUntil(entry, (global::System.Func<dynamic, bool>)predicate);
        return ((_RouteEntry__navigator)entry).restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T?> push<T>(Route<T> route)
    {
        _pushEntry(new _RouteEntry__navigator(route, pageBased: false, initialState: _RouteLifecycle__navigator.push));
        return ((Route<T>)route).popped;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugIsStaticCallback(Delegate callback)
    {
        var result = false;
        DartRuntimePrimitives.Assert(() =>
            {
                result = (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb || (Dart_uiLibrary.PluginUtilities.getCallbackHandle(callback) is not null));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return DartRuntimePrimitives.RequireValue(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePush<T>(global::System.Func<BuildContext, object, Route<T>> routeBuilder, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback((global::System.Func<BuildContext, object, Route<T>>)routeBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateAnonymous(routeBuilder: (global::System.Func<BuildContext, object, Route<T>>)routeBuilder, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.push));
        _pushEntry(entry);
        return ((_RouteEntry__navigator)entry).restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _pushEntry(_RouteEntry__navigator entry)
    {
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => !((_RouteEntry__navigator)entry).route._installed);
        DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.push)));
        this._history.add(entry);
        _flushHistoryUpdates();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _afterNavigation(((_RouteEntry__navigator)entry).route);
    }

    internal virtual void _afterNavigation(RouteBase? route)
    {
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            DartMap<string, object>? routeJsonable = default!;
            if ((route is not null))
            {
                routeJsonable = new DartMap<string, object>();
                string description = default!;
                if ((route is ITransitionRoute transitionRoute))
                {
                    description = transitionRoute.debugLabel;
                }
                else
                {
                    description = $"{route}";
                }
                routeJsonable["description"] = description;
                RouteSettings settingsLocal = route.settings;
                var settingsJsonable = new DartMap<string, object> { ["name"] = ((RouteSettings)settingsLocal).name };
                if ((((RouteSettings)settingsLocal).arguments is not null))
                {
                    settingsJsonable["arguments"] = global::Doroti.Runtime.Dart_convertLibrary.jsonEncode(((RouteSettings)settingsLocal).arguments, toEncodable: ((@object) => $"{@object}"));
                }
                routeJsonable["settings"] = settingsJsonable;
            }
            Dart_developerLibrary.postEvent("Flutter.Navigation", new DartMap<string, object> { ["route"] = routeJsonable });
        }
        _cancelActivePointers();
    }

    public virtual Future<T?> pushReplacement<T, TO>(Route<T> newRoute, TO? result = default)
    {
        DartRuntimePrimitives.Assert(() => !((Route<T>)newRoute)._installed);
        _pushReplacementEntry(new _RouteEntry__navigator(newRoute, pageBased: false, initialState: _RouteLifecycle__navigator.pushReplace), result);
        return ((Route<T>)newRoute).popped;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePushReplacement<T, TO>(global::System.Func<BuildContext, object, Route<T>> routeBuilder, TO? result = default, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback((global::System.Func<BuildContext, object, Route<T>>)routeBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateAnonymous(routeBuilder: (global::System.Func<BuildContext, object, Route<T>>)routeBuilder, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.pushReplace));
        _pushReplacementEntry(entry, result);
        return ((_RouteEntry__navigator)entry).restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _pushReplacementEntry<TO>(_RouteEntry__navigator entry, TO? result)
    {
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => !((_RouteEntry__navigator)entry).route._installed);
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this._history));
        DartRuntimePrimitives.Assert(() => this._history.any(__item => _RouteEntry__navigator.isPresentPredicate(__item)), () => (object?)"Navigator has no active routes to replace.");
        DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.pushReplace)));
        this._history.lastWhere(_RouteEntry__navigator.isPresentPredicate).complete(result, isReplaced: true, imperativeRemoval: true);
        this._history.add(entry);
        _flushHistoryUpdates();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _afterNavigation(((_RouteEntry__navigator)entry).route);
    }

    public virtual Future<T?> pushAndRemoveUntil<T>(Route<T> newRoute, global::System.Func<dynamic, bool> predicate)
    {
        DartRuntimePrimitives.Assert(() => !((Route<T>)newRoute)._installed);
        DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(((Route<T>)newRoute).overlayEntries));
        _pushEntryAndRemoveUntil(new _RouteEntry__navigator(newRoute, pageBased: false, initialState: _RouteLifecycle__navigator.push), (global::System.Func<dynamic, bool>)predicate);
        return ((Route<T>)newRoute).popped;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePushAndRemoveUntil<T>(global::System.Func<BuildContext, object, Route<T>> newRouteBuilder, global::System.Func<dynamic, bool> predicate, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback((global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateAnonymous(routeBuilder: (global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.push));
        _pushEntryAndRemoveUntil(entry, (global::System.Func<dynamic, bool>)predicate);
        return ((_RouteEntry__navigator)entry).restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _pushEntryAndRemoveUntil(_RouteEntry__navigator entry, global::System.Func<dynamic, bool> predicate)
    {
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => !((_RouteEntry__navigator)entry).route._installed);
        DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(((_RouteEntry__navigator)entry).route.overlayEntries));
        DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.push)));
        long index = (this._history.Count() - 1L);
        this._history.add(entry);
        while (((index >= 0L) && !predicate(this._history[index].route)))
        {
            if (this._history[index].isPresent)
            {
                this._history[index].complete(((Navigator)(object)null), isReplaced: false, imperativeRemoval: true);
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
        _afterNavigation(((_RouteEntry__navigator)entry).route);
    }

    public virtual void replace<T>(dynamic oldRoute, Route<T> newRoute)
    {
        RouteBase typedOldRoute = Navigator._requireRoute((object?)oldRoute);
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        DartRuntimePrimitives.Assert(() => typedOldRoute._isInstalledIn(this));
        _replaceEntry(new _RouteEntry__navigator(newRoute, pageBased: false, initialState: _RouteLifecycle__navigator.replace), typedOldRoute);
    }

    public virtual string restorableReplace<T>(dynamic oldRoute, global::System.Func<BuildContext, object, Route<T>> newRouteBuilder, object? arguments = null)
    {
        RouteBase typedOldRoute = Navigator._requireRoute((object?)oldRoute);
        DartRuntimePrimitives.Assert(() => typedOldRoute._isInstalledIn(this));
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback((global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateAnonymous(routeBuilder: (global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.replace));
        _replaceEntry(entry, typedOldRoute);
        return ((_RouteEntry__navigator)entry).restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _replaceEntry(_RouteEntry__navigator entry, RouteBase oldRoute)
    {
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        if ((object.Equals(oldRoute, ((_RouteEntry__navigator)entry).route)))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.replace)));
        DartRuntimePrimitives.Assert(() => !((_RouteEntry__navigator)entry).route._installed);
        long index = this._history.indexWhere(_RouteEntry__navigator.isRoutePredicate(oldRoute));
        DartRuntimePrimitives.Assert(() => (index >= 0L), () => (object?)"This Navigator does not contain the specified oldRoute.");
        DartRuntimePrimitives.Assert(() => this._history[index].isPresent, () => (object?)"The specified oldRoute has already been removed from the Navigator.");
        bool wasCurrent = oldRoute.isCurrent;
        this._history.insert((index + 1L), entry);
        this._history[index].complete(((Navigator)(object)null), isReplaced: true, imperativeRemoval: true);
        _flushHistoryUpdates();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (wasCurrent)
        {
            _afterNavigation(((_RouteEntry__navigator)entry).route);
        }
    }

    public virtual void replaceRouteBelow<T>(dynamic anchorRoute, Route<T> newRoute)
    {
        RouteBase typedAnchorRoute = Navigator._requireRoute((object?)anchorRoute);
        DartRuntimePrimitives.Assert(() => !((Route<T>)newRoute)._installed);
        DartRuntimePrimitives.Assert(() => typedAnchorRoute._isInstalledIn(this));
        _replaceEntryBelow(new _RouteEntry__navigator(newRoute, pageBased: false, initialState: _RouteLifecycle__navigator.replace), typedAnchorRoute);
    }

    public virtual string restorableReplaceRouteBelow<T>(dynamic anchorRoute, global::System.Func<BuildContext, object, Route<T>> newRouteBuilder, object? arguments = null)
    {
        RouteBase typedAnchorRoute = Navigator._requireRoute((object?)anchorRoute);
        DartRuntimePrimitives.Assert(() => typedAnchorRoute._isInstalledIn(this));
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback((global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateAnonymous(routeBuilder: (global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.replace));
        _replaceEntryBelow(entry, typedAnchorRoute);
        return ((_RouteEntry__navigator)entry).restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _replaceEntryBelow(_RouteEntry__navigator entry, RouteBase anchorRoute)
    {
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        long anchorIndex = this._history.indexWhere(_RouteEntry__navigator.isRoutePredicate(anchorRoute));
        DartRuntimePrimitives.Assert(() => (anchorIndex >= 0L), () => (object?)"This Navigator does not contain the specified anchorRoute.");
        DartRuntimePrimitives.Assert(() => this._history[anchorIndex].isPresent, () => (object?)"The specified anchorRoute has already been removed from the Navigator.");
        long index = (anchorIndex - 1L);
        while ((index >= 0L))
        {
            if (this._history[index].isPresent)
            {
                break;
            }
            index -= 1L;
        }
        DartRuntimePrimitives.Assert(() => (index >= 0L), () => (object?)"There are no routes below the specified anchorRoute.");
        this._history.insert((index + 1L), entry);
        this._history[index].complete(((Navigator)(object)null), isReplaced: true, imperativeRemoval: true);
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
        IEnumerator<_RouteEntry__navigator> iterator = this._history.where(_RouteEntry__navigator.isPresentPredicate).GetEnumerator();
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
        _RouteEntry__navigator? lastEntry = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
        if ((lastEntry is null))
        {
            return false;
        }
        DartRuntimePrimitives.Assert(() => ((_RouteEntry__navigator)lastEntry).route._isInstalledIn(this));
        DartRuntimePrimitives.Assert(() => ((_RouteEntry__navigator)lastEntry).route._debugCheckCanConsumeResult(result, methodName: "maybePop"));
        if ((object.Equals(await ((_RouteEntry__navigator)lastEntry).route.willPop(), RoutePopDisposition.doNotPop)))
        {
            return true;
        }
        if (!this.mounted)
        {
            return true;
        }
        _RouteEntry__navigator? newLastEntry = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
        if ((!object.Equals(lastEntry, newLastEntry)))
        {
            return true;
        }
        switch (((_RouteEntry__navigator)lastEntry).route.popDisposition)
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
                    ((_RouteEntry__navigator)lastEntry).route.onPopInvokedWithResultObject(false, result);
                    return true;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void pop<T>(T? result = default)
    {
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _RouteEntry__navigator? entry = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
                return entry?.route._debugCheckCanConsumeResult(result, methodName: "pop") ?? true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _RouteEntry__navigator entryLocal = this._history.lastWhere(_RouteEntry__navigator.isPresentPredicate);
        if ((((_RouteEntry__navigator)entryLocal).pageBased && (((Navigator)(object)this.widget).onPopPage is not null)))
        {
            if (((Navigator)(object)this.widget).onPopPage!(((_RouteEntry__navigator)entryLocal).route, result))
            {
                if ((FoundationRuntimePorts.EnumIndex(((_RouteEntry__navigator)entryLocal).currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.idle)))
                {
                    DartRuntimePrimitives.Assert(() => ((_RouteEntry__navigator)entryLocal).route.popCompleted);
                    entryLocal.currentState = _RouteLifecycle__navigator.pop;
                }
                ((_RouteEntry__navigator)entryLocal).route.onPopInvokedWithResultObject(true, result);
            }
        }
        else
        {
            entryLocal.pop<T>(result, imperativeRemoval: true);
            DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entryLocal).currentState, _RouteLifecycle__navigator.pop)));
        }
        if ((object.Equals(((_RouteEntry__navigator)entryLocal).currentState, _RouteLifecycle__navigator.pop)))
        {
            _flushHistoryUpdates(rearrangeOverlay: false);
        }
        DartRuntimePrimitives.Assert(() => ((object.Equals(((_RouteEntry__navigator)entryLocal).currentState, _RouteLifecycle__navigator.idle)) || ((_RouteEntry__navigator)entryLocal).route.popCompleted));
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _afterNavigation(((_RouteEntry__navigator)entryLocal).route);
    }

    public virtual void popUntil(global::System.Func<dynamic, bool> predicate)
    {
        _RouteEntry__navigator? candidate = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
        while ((candidate is not null))
        {
            if (predicate(((_RouteEntry__navigator)candidate).route))
            {
                return;
            }
            pop<object>();
            candidate = _lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate);
        }
    }

    public virtual void popUntilWithResult<T>(global::System.Func<dynamic, bool> predicate, T? result)
    {
        _RouteEntry__navigator? candidate = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
        while ((candidate is not null))
        {
            if (predicate(((_RouteEntry__navigator)candidate).route))
            {
                return;
            }
            _RouteEntry__navigator? next = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull(((global::System.Func<_RouteEntry__navigator, bool>)((e) => (_RouteEntry__navigator.isPresentPredicate(e) && (!object.Equals(e, candidate)))))));
            if ((((next is not null) && !((_RouteEntry__navigator)next).route.willHandlePopInternally) && predicate(((_RouteEntry__navigator)next).route)))
            {
                pop<T>(result);
            }
            else
            {
                pop<object>();
            }
            candidate = _lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate);
        }
    }

    public virtual void removeRoute<T>(Route<T> route, T? result = default)
    {
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => route._isInstalledIn(this));
        bool wasCurrent = ((Route<T>)route).isCurrent;
        _RouteEntry__navigator entry = this._history.firstWhere(_RouteEntry__navigator.isRoutePredicate(route));
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
            _afterNavigation(_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate)?.route);
        }
    }

    public virtual void removeRouteBelow<T>(Route<T> anchorRoute, T? result = default)
    {
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => anchorRoute._isInstalledIn(this));
        long anchorIndex = this._history.indexWhere(_RouteEntry__navigator.isRoutePredicate(anchorRoute));
        DartRuntimePrimitives.Assert(() => (anchorIndex >= 0L), () => (object?)"This Navigator does not contain the specified anchorRoute.");
        DartRuntimePrimitives.Assert(() => this._history[anchorIndex].isPresent, () => (object?)"The specified anchorRoute has already been removed from the Navigator.");
        long index = (anchorIndex - 1L);
        while ((index >= 0L))
        {
            if (this._history[index].isPresent)
            {
                break;
            }
            index -= 1L;
        }
        DartRuntimePrimitives.Assert(() => (index >= 0L), () => (object?)"There are no routes below the specified anchorRoute.");
        this._history[index].complete(result, isReplaced: false, imperativeRemoval: true);
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
                wasDebugLocked = this._debugLocked;
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => (this._history.where((entry) => _RouteEntry__navigator.isRoutePredicate(typedRoute)(DartRuntimePrimitives.ConvertValue<_RouteEntry__navigator>(entry))).Count() == 1L));
        long index = this._history.indexWhere(_RouteEntry__navigator.isRoutePredicate(typedRoute));
        _RouteEntry__navigator entryLocal = this._history[index];
        if ((((_RouteEntry__navigator)entryLocal).pageBased && (FoundationRuntimePorts.EnumIndex(((_RouteEntry__navigator)entryLocal).currentState) < FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.pop))))
        {
            this._observedRouteDeletions.Enqueue(new _NavigatorPopObservation__navigator(typedRoute, _getRouteBefore((index - 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.willBePresentPredicate)?.route));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entryLocal).currentState, _RouteLifecycle__navigator.popping)));
        }
        entryLocal.finalize();
        if (!this._flushingHistory)
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
        return ((Route<T>?)(object?)_firstRouteEntryWhereOrNull(((global::System.Func<_RouteEntry__navigator, bool>)((entry) => (((_RouteEntry__navigator)entry).restorationId == id))))?.route)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _userGesturesInProgress
    {
        get => this._userGesturesInProgressCount;
        set
        {
            var __value = value;
            _userGesturesInProgressCount = __value;
            this.userGestureInProgressNotifier.value = (this._userGesturesInProgress > 0L);
        }
    }
    public virtual bool userGestureInProgress => ((global::Doroti.Framework.Foundation.ValueNotifier<bool>)this.userGestureInProgressNotifier).value;
    public virtual void didStartUserGesture()
    {
        _userGesturesInProgress += 1L;
        if ((this._userGesturesInProgress == 1L))
        {
            long routeIndex = _getIndexBefore((this._history.Count() - 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.willBePresentPredicate);
            RouteBase routeLocal = this._history[routeIndex].route;
            RouteBase? previousRoute = default;
            if ((!routeLocal.willHandlePopInternally && (routeIndex > 0L)))
            {
                previousRoute = _getRouteBefore((routeIndex - 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.willBePresentPredicate)!.route;
            }
            foreach (NavigatorObserver observer in this._effectiveObservers)
            {
                observer.didStartUserGesture(routeLocal, previousRoute);
            }
        }
    }

    public virtual void didStopUserGesture()
    {
        DartRuntimePrimitives.Assert(() => (this._userGesturesInProgress > 0L));
        _userGesturesInProgress -= 1L;
        if ((this._userGesturesInProgress == 0L))
        {
            foreach (NavigatorObserver observer in this._effectiveObservers)
            {
                observer.didStopUserGesture();
            }
        }
    }

    internal virtual void _handlePointerDown(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        this._activePointers.Add(@event.pointer);
    }

    internal virtual void _handlePointerUpOrCancel(global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        this._activePointers.Remove(((global::Doroti.Framework.Gestures.PointerEvent)@event).pointer);
    }

    internal virtual void _cancelActivePointers()
    {
        if ((object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.idle)))
        {
            global::Doroti.Framework.Rendering.RenderAbsorbPointer? absorber = ((global::Doroti.Framework.Rendering.RenderAbsorbPointer?)(object?)((GlobalKey<OverlayState>)this._overlayKey).currentContext?.findAncestorRenderObjectOfType<global::Doroti.Framework.Rendering.RenderAbsorbPointer>());
            setState(((global::System.Action)(() =>
            {
                absorber?.absorbing = true;
            })));
        }
        this._activePointers.ToList().forEach((__arg0) => ((global::System.Action<long>)WidgetsBinding.instance.cancelPointer)(__arg0));
    }

    internal virtual _RouteEntry__navigator? _firstRouteEntryWhereOrNull(global::System.Func<_RouteEntry__navigator, bool> test)
    {
        foreach (_RouteEntry__navigator element in this._history)
        {
            if (test(element))
            {
                return element;
            }
        }
        return ((_RouteEntry__navigator)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _RouteEntry__navigator? _lastRouteEntryWhereOrNull(global::System.Func<_RouteEntry__navigator, bool> test)
    {
        _RouteEntry__navigator? result = default!;
        foreach (_RouteEntry__navigator element in this._history)
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
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this._history));
        return new Overlay(
            key: this._overlayKey,
            clipBehavior: ((Navigator)(object)this.widget).clipBehavior,
            initialEntries: this.overlay is null ? this._allRouteOverlayEntries.ToList() : new List<OverlayEntry>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
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
            property.addListener((global::System.Action)listener);
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
    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\"."), new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:") }));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: ((global::Doroti.Framework.Services.RestorationBucket)(object)null), restorePending: restorePending);
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
        => ((_RestorationInformation__navigator)(object?)new _NamedRestorationInformation__navigator(name, arguments, restorationScopeId));

    internal static _RestorationInformation__navigator CreateAnonymous(global::System.Func<BuildContext, object, dynamic> routeBuilder, object? arguments, long restorationScopeId)
        => ((_RestorationInformation__navigator)(object?)new _AnonymousRestorationInformation__navigator(routeBuilder, arguments, restorationScopeId));

    internal static _RestorationInformation__navigator CreateFromSerializableData(object data)
    {
        var casted = ((List<object?>?)(object?)data)!;
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(casted));
        _RouteRestorationType__navigator @type = System.Enum.GetValues<_RouteRestorationType__navigator>().ToList()[(int)(((long)casted[(int)(0L)]!))];
        switch (@type)
        {
            case _RouteRestorationType__navigator.named:
                {
                    return ((_RestorationInformation__navigator)(object?)_NamedRestorationInformation__navigator.CreateFromSerializableData(casted.Skip(checked((int)1L)).ToList()));
                }
            case _RouteRestorationType__navigator.anonymous:
                {
                    return ((_RestorationInformation__navigator)(object?)_AnonymousRestorationInformation__navigator.CreateFromSerializableData(casted.Skip(checked((int)1L)).ToList()));
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
        return this._serializableData!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> computeSerializableData()
    {
        return new List<object> { FoundationRuntimePorts.EnumIndex(this.type) };
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
        this.__field_restorationScopeId = restorationScopeId;
    }

    internal static _NamedRestorationInformation__navigator CreateFromSerializableData(List<object> data)
    {
        var __instance = new _NamedRestorationInformation__navigator(default!, default!, default!);
        __instance.__field_restorationScopeId = ((long)data[(int)(0L)]!);
        __instance.name = ((string?)(object?)data[(int)(1L)]!)!;
        __instance.arguments = data.elementAtOrNull(2L);
        return __instance;
    }

    public override List<object> computeSerializableData()
    {
        return ((Func<List<object>>)(() =>
{
    var __cascade = base.computeSerializableData();
    __cascade.AddRange(new List<object> { this.restorationScopeId, this.name }.Cast<object>());
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RouteBase createRoute(NavigatorState navigator)
    {
        RouteBase route = navigator._routeNamed<object>(this.name, arguments: this.arguments)!;
        return route;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AnonymousRestorationInformation__navigator : _RestorationInformation__navigator
{
    private long __field_restorationScopeId = default!;
    public override long restorationScopeId { get => __field_restorationScopeId; }
    public virtual global::System.Func<BuildContext, object, dynamic> routeBuilder { get; private set; } = default!;
    public virtual object? arguments { get; private set; }

    internal _AnonymousRestorationInformation__navigator(global::System.Func<BuildContext, object, dynamic> routeBuilder, object? arguments, long restorationScopeId) : base(_RouteRestorationType__navigator.anonymous)
    {
        this.routeBuilder = routeBuilder;
        this.arguments = arguments;
        this.__field_restorationScopeId = restorationScopeId;
    }

    internal static _AnonymousRestorationInformation__navigator CreateFromSerializableData(List<object> data)
    {
        var __instance = new _AnonymousRestorationInformation__navigator(default!, default!, default!);
        __instance.__field_restorationScopeId = ((long)data[(int)(0L)]!);
        __instance.routeBuilder = ((global::System.Func<BuildContext, object, Route<object>>?)(object?)Dart_uiLibrary.PluginUtilities.getCallbackFromHandle(new global::Doroti.Ui.CallbackHandle(((long)data[(int)(1L)]!)))!)!;
        __instance.arguments = data.elementAtOrNull(2L);
        return __instance;
    }

    public override bool isRestorable => !global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb;
    public override List<object> computeSerializableData()
    {
        DartRuntimePrimitives.Assert(() => this.isRestorable);
        global::Doroti.Ui.CallbackHandle? handle = ((global::Doroti.Ui.CallbackHandle?)(object?)Dart_uiLibrary.PluginUtilities.getCallbackHandle(this.routeBuilder));
        DartRuntimePrimitives.Assert(() => (handle is not null));
        return ((Func<List<object>>)(() =>
{
    var __cascade = base.computeSerializableData();
    __cascade.AddRange(new List<object> { this.restorationScopeId, handle!.toRawHandle() }.Cast<object>());
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RouteBase createRoute(NavigatorState navigator)
    {
        object? result = this.routeBuilder(navigator.context, this.arguments);
        return Navigator._requireRoute(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _HistoryProperty__navigator : RestorableProperty<DartMap<string?, List<object>>?>
{
    internal virtual DartMap<string?, List<object>>? _pageToPagelessRoutes { get; set; } = default;

    public virtual void update(_History__navigator history)
    {
        DartRuntimePrimitives.Assert(() => this.isRegistered);
        var wasUninitialized = (this._pageToPagelessRoutes is null);
        var needsSerialization = wasUninitialized;
        _pageToPagelessRoutes ??= new DartMap<string, List<object>>();
        _RouteEntry__navigator? currentPage = default!;
        var newRoutesForCurrentPage = new List<object>();
        List<object> oldRoutesForCurrentPage = (this._pageToPagelessRoutes!.GetValueOrDefault(null) ?? new List<object>()).ToList();
        var restorationEnabledLocal = true;
        var newMap = new DartMap<string?, List<object>>();
        HashSet<string?> removedPages = this._pageToPagelessRoutes!.Keys.toSet();
        foreach (var entry in history)
        {
            if (!((_RouteEntry__navigator)entry).isPresentForRestoration)
            {
                entry.restorationEnabled = false;
                continue;
            }
            DartRuntimePrimitives.Assert(() => ((_RouteEntry__navigator)entry).isPresentForRestoration);
            if (((_RouteEntry__navigator)entry).pageBased)
            {
                needsSerialization = (needsSerialization || (checked((long)(newRoutesForCurrentPage.Count)) != checked((long)(oldRoutesForCurrentPage.Count))));
                _finalizeEntry(newRoutesForCurrentPage, currentPage, newMap, removedPages);
                currentPage = entry;
                restorationEnabledLocal = (((_RouteEntry__navigator)entry).restorationId is not null);
                entry.restorationEnabled = restorationEnabledLocal;
                if (restorationEnabledLocal)
                {
                    DartRuntimePrimitives.Assert(() => (((_RouteEntry__navigator)entry).restorationId is not null));
                    newRoutesForCurrentPage = new List<object>();
                    oldRoutesForCurrentPage = (this._pageToPagelessRoutes!.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((_RouteEntry__navigator)entry).restorationId)) ?? new List<object>());
                }
                else
                {
                    newRoutesForCurrentPage = new List<object>();
                    oldRoutesForCurrentPage = new List<object>();
                }
                continue;
            }
            DartRuntimePrimitives.Assert(() => !((_RouteEntry__navigator)entry).pageBased);
            restorationEnabledLocal = (restorationEnabledLocal && ((((_RouteEntry__navigator)entry).restorationInformation?.isRestorable ?? false)));
            entry.restorationEnabled = restorationEnabledLocal;
            if (restorationEnabledLocal)
            {
                DartRuntimePrimitives.Assert(() => (((_RouteEntry__navigator)entry).restorationId is not null));
                DartRuntimePrimitives.Assert(() => ((currentPage is null) || (((_RouteEntry__navigator)currentPage).restorationId is not null)));
                DartRuntimePrimitives.Assert(() => (((_RouteEntry__navigator)entry).restorationInformation is not null));
                object serializedData = ((_RouteEntry__navigator)entry).restorationInformation!.getSerializableData();
                needsSerialization = ((needsSerialization || (checked((long)(oldRoutesForCurrentPage.Count)) <= checked((long)(newRoutesForCurrentPage.Count)))) || (!object.Equals(oldRoutesForCurrentPage[(int)(checked((long)(newRoutesForCurrentPage.Count)))], serializedData)));
                newRoutesForCurrentPage.Add(serializedData);
            }
        }
        needsSerialization = (needsSerialization || (checked((long)(newRoutesForCurrentPage.Count)) != checked((long)(oldRoutesForCurrentPage.Count))));
        _finalizeEntry(newRoutesForCurrentPage, currentPage, newMap, removedPages);
        needsSerialization = (needsSerialization || System.Linq.Enumerable.Any(removedPages));
        DartRuntimePrimitives.Assert(() => (wasUninitialized || (_debugMapsEqual(this._pageToPagelessRoutes!, newMap) != needsSerialization)));
        if (needsSerialization)
        {
            _pageToPagelessRoutes = newMap.cast<string?, List<object>>();
            notifyListeners();
        }
    }

    internal virtual void _finalizeEntry(List<object> routes, _RouteEntry__navigator? page, DartMap<string?, List<object>> pageToRoutes, HashSet<string?> pagesToRemove)
    {
        DartRuntimePrimitives.Assert(() => ((page is null) || ((_RouteEntry__navigator)page).pageBased));
        DartRuntimePrimitives.Assert(() => !pageToRoutes.ContainsKey(page?.restorationId));
        if (System.Linq.Enumerable.Any(routes))
        {
            DartRuntimePrimitives.Assert(() => ((page is null) || (((_RouteEntry__navigator)page).restorationId is not null)));
            string? restorationIdLocal = page?.restorationId;
            pageToRoutes[DartRuntimePrimitives.RequireReference(restorationIdLocal)] = routes;
            pagesToRemove.Remove(restorationIdLocal);
        }
    }

    internal virtual bool _debugMapsEqual(DartMap<string?, List<object>> a, DartMap<string?, List<object>> b)
    {
        if (!global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals(a.Keys.toSet(), b.Keys.toSet()))
        {
            return false;
        }
        foreach (string? key in a.Keys)
        {
            if (!global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(a.GetValueOrDefault(DartRuntimePrimitives.RequireReference(key)).Cast<DartMap<string?, List<object>>?>().ToList(), b.GetValueOrDefault(DartRuntimePrimitives.RequireReference(key)).Cast<DartMap<string?, List<object>>?>().ToList()))
            {
                return false;
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void clear()
    {
        DartRuntimePrimitives.Assert(() => this.isRegistered);
        if ((this._pageToPagelessRoutes is null))
        {
            return;
        }
        _pageToPagelessRoutes = null;
        notifyListeners();
    }

    public virtual bool hasData => DartRuntimePrimitives.ConvertValue<bool>((this._pageToPagelessRoutes is not null));
    public virtual List<_RouteEntry__navigator> restoreEntriesForPage(_RouteEntry__navigator? page, NavigatorState navigator)
    {
        DartRuntimePrimitives.Assert(() => this.isRegistered);
        DartRuntimePrimitives.Assert(() => ((page is null) || ((_RouteEntry__navigator)page).pageBased));
        var result = new List<_RouteEntry__navigator>();
        if (((this._pageToPagelessRoutes is null) || (((page is not null) && (((_RouteEntry__navigator)page).restorationId is null)))))
        {
            return result;
        }
        List<object>? serializedData = this._pageToPagelessRoutes!.GetValueOrDefault(DartRuntimePrimitives.RequireReference(page?.restorationId)).ToList();
        if ((serializedData is null))
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
        return ((DartMap<string?, List<object>>)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DartMap<string?, List<object>>? fromPrimitives(object? data)
    {
        var casted = DartRuntimePrimitives.ConvertMap<object, object>((System.Collections.IDictionary)data!);
        return casted.map<object, object, string?, List<object>>(((key, value) => new MapEntry<string?, List<object>>(((string?)(object?)key)!, new List<object>(DartRuntimePrimitives.ConvertEnumerable<object>(((List<object>?)(object?)value)!)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initWithValue(DartMap<string?, List<object>>? value)
    {
        _pageToPagelessRoutes = value;
    }

    public override object? toPrimitives()
    {
        return this._pageToPagelessRoutes;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool enabled => this.hasData;
}

public delegate NavigatorState NavigatorFinderCallback(BuildContext context);

public delegate string RoutePresentationCallback(NavigatorState navigator, object? arguments);

public delegate void RouteCompletionCallback<T>(T result);

public class RestorableRouteFuture<T> : RestorableProperty<string?>
{
    public virtual global::System.Func<BuildContext, NavigatorState> navigatorFinder { get; private set; } = default!;
    public virtual global::System.Func<NavigatorState, object, string> onPresent { get; private set; } = default!;
    public virtual global::System.Action<T>? onComplete { get; private set; }
    internal virtual Route<T>? _route { get; set; } = default;
    internal virtual bool _disposed { get; set; } = false;

    public RestorableRouteFuture(global::System.Func<BuildContext, NavigatorState> navigatorFinder = default!, global::System.Func<NavigatorState, object, string> onPresent = default!, global::System.Action<T>? onComplete = null)
    {
        global::System.Func<BuildContext, NavigatorState> __navigatorFinder = navigatorFinder ?? _defaultNavigatorFinder;
        this.navigatorFinder = __navigatorFinder;
        this.onPresent = onPresent;
        this.onComplete = onComplete;
    }

    public virtual void present(object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => !this.isPresent);
        DartRuntimePrimitives.Assert(() => this.isRegistered);
        string routeId = this.onPresent(this._navigator, arguments);
        _hookOntoRouteFuture(routeId);
        notifyListeners();
    }

    public virtual bool isPresent => DartRuntimePrimitives.ConvertValue<bool>((this.route is not null));
    public virtual Route<T>? route => this._route;
    public override string? createDefaultValue() => DartRuntimePrimitives.ConvertValue<string>(null);
    public override void initWithValue(string? value)
    {
        if ((value is not null))
        {
            _hookOntoRouteFuture(value);
        }
    }

    public override object? toPrimitives()
    {
        DartRuntimePrimitives.Assert(() => (this.route is not null));
        DartRuntimePrimitives.Assert(() => this.enabled);
        return this.route?.restorationScopeId.value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string? fromPrimitives(object? data)
    {
        DartRuntimePrimitives.Assert(() => (data is not null));
        return ((string?)(object?)data!)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        base.dispose();
        this._route?.restorationScopeId.removeListener(this.notifyListeners);
        _disposed = true;
    }

    public override bool enabled => DartRuntimePrimitives.ConvertValue<bool>((this.route?.restorationScopeId.value is not null));
    internal virtual NavigatorState _navigator
    {
        get
        {
            NavigatorState navigator = this.navigatorFinder(this.state.context);
            return navigator;
            return default!;
        }
    }
    internal virtual void _hookOntoRouteFuture(string id)
    {
        _route = this._navigator._getRouteById<T>(id);
        DartRuntimePrimitives.Assert(() => (this._route is not null));
        this.route!.restorationScopeId.addListener(this.notifyListeners);
        DartRuntimePrimitives.Ignore(this.route!.popped.then((global::System.Action<object>)((result) =>
        {
            if (this._disposed)
            {
                return;
            }
            this._route?.restorationScopeId.removeListener(this.notifyListeners);
            _route = null;
            notifyListeners();
            this.onComplete?.Invoke(((T?)(object?)result)!);
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
        return $"NavigationNotification canHandlePop: {this.canHandlePop}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
