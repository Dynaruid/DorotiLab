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

public abstract class Route<T> : _RoutePlaceholder__navigator
{
    internal virtual bool? _requestFocus { get; private set; }
    internal virtual NavigatorState? _navigator { get; set; } = default;
    internal virtual RouteSettings _settings { get; set; } = default!;
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<string?> _restorationScopeId { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<string?>(((string)(object)null));
    internal virtual Completer<T?> _popCompleter { get; private set; } = new Completer<T?>();
    internal virtual Completer<T?> _disposeCompleter { get; private set; } = new Completer<T?>();

    protected Route(RouteSettings? settings = null, bool? requestFocus = null)
    {
        this._settings = (settings ?? new RouteSettings());
        this._requestFocus = requestFocus;
    }

    public virtual bool requestFocus => DartRuntimePrimitives.ConvertValue<bool>(((this._requestFocus ?? this.navigator?.widget.requestFocus) ?? false));
    public virtual NavigatorState? navigator => this._navigator;
    internal virtual bool _installed => DartRuntimePrimitives.ConvertValue<bool>((this._navigator is not null));
    internal virtual bool _isInstalledIn(NavigatorState state) => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(this._navigator, state)));
    public virtual RouteSettings settings => this._settings;
    internal virtual bool _isPageBased => (this.settings is Page<object?>);
    public virtual global::Doroti.Framework.Foundation.ValueListenable<string?> restorationScopeId => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Foundation.ValueListenable<string?>>(this._restorationScopeId);
    internal virtual void _updateSettings(RouteSettings newSettings)
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

    internal virtual void _updateRestorationId(string? restorationId)
    {
        this._restorationScopeId.value = restorationId;
    }

    public virtual List<OverlayEntry> overlayEntries => new List<OverlayEntry>();
    public virtual void install()
    {
    }

    public virtual global::Doroti.Framework.Scheduler.TickerFuture didPush()
    {
        return ((Func<global::Doroti.Framework.Scheduler.TickerFuture>)(() =>
{            var __cascade = global::Doroti.Framework.Scheduler.TickerFuture.CreateComplete();
            __cascade.then(((global::System.Func<object?, object>)((_) => {
if (this.requestFocus)
{
    this.navigator!.focusNode.enclosingScope?.requestFocus();
}
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didAdd()
    {
        if (this.requestFocus)
        {
            DartRuntimePrimitives.Ignore(global::Doroti.Framework.Scheduler.TickerFuture.CreateComplete().then(((global::System.Func<object?, object>)((_) => {
this.navigator?.focusNode.enclosingScope?.requestFocus();
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        }
    }

    public virtual void didReplace(dynamic oldRoute)
    {
    }

    public async virtual Future<RoutePopDisposition> willPop()
    {
        return (this.isFirst ? RoutePopDisposition.bubble : RoutePopDisposition.pop);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RoutePopDisposition popDisposition
    {
        get
        {
            if (this._isPageBased)
            {
                var page__15547 = ((Page<object?>?)(object?)this.settings)!;
                if (!((Page<object>)page__15547).canPop)
                {
                    return RoutePopDisposition.doNotPop;
                }
            }
            return (this.isFirst ? RoutePopDisposition.bubble : RoutePopDisposition.pop);
            return default!;
        }
    }
    public virtual void onPopInvoked(bool didPop)
    {
    }

    public virtual void onPopInvokedWithResult(bool didPop, T? result)
    {
        if (this._isPageBased)
        {
            var page__16630 = ((Page<T>?)(object?)this.settings)!;
            page__16630.onPopInvoked(didPop, result);
        }
    }

    public virtual bool willHandlePopInternally => false;
    public virtual T? currentResult => DartRuntimePrimitives.ConvertValue<T>(null);
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

    public virtual void didPopNext(dynamic nextRoute)
    {
    }

    public virtual void didChangeNext(dynamic nextRoute)
    {
    }

    public virtual void didChangePrevious(dynamic previousRoute)
    {
    }

    public virtual void changedInternalState()
    {
    }

    public virtual void changedExternalState()
    {
    }

    public virtual void dispose()
    {
        _navigator = null;
        this._restorationScopeId.dispose();
        this._disposeCompleter.complete();
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
    }

    public virtual bool isCurrent
    {
        get
        {
            if (!this._installed)
            {
                return false;
            }
            _RouteEntry__navigator? currentRouteEntry__23776 = ((_RouteEntry__navigator?)(object?)this._navigator!._lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
            if ((currentRouteEntry__23776 is null))
            {
                return false;
            }
            return (object.Equals(((_RouteEntry__navigator)currentRouteEntry__23776).route, this));
            return default!;
        }
    }
    public virtual bool isFirst
    {
        get
        {
            if (!this._installed)
            {
                return false;
            }
            _RouteEntry__navigator? currentRouteEntry__24303 = ((_RouteEntry__navigator?)(object?)this._navigator!._firstRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
            if ((currentRouteEntry__24303 is null))
            {
                return false;
            }
            return (object.Equals(((_RouteEntry__navigator)currentRouteEntry__24303).route, this));
            return default!;
        }
    }
    public virtual bool hasActiveRouteBelow
    {
        get
        {
            if (!this._installed)
            {
                return false;
            }
            foreach (_RouteEntry__navigator entry__24715 in this._navigator!._history)
            {
                if ((object.Equals(((_RouteEntry__navigator)entry__24715).route, this)))
                {
                    return false;
                }
                if (_RouteEntry__navigator.isPresentPredicate(entry__24715))
                {
                    return true;
                }
            }
            return false;
            return default!;
        }
    }
    public virtual bool isActive
    {
        get
        {
            return (this._navigator?._firstRouteEntryWhereOrNull(_RouteEntry__navigator.isRoutePredicate(this))?.isPresent ?? false);
            return default!;
        }
    }
    internal virtual bool _debugCheckCanConsumeResult(object? result, string methodName)
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
        var __instance = new HeroControllerScope(default!, default!, default!);
        __instance.controller = null;
        return __instance;
    }

    public static HeroController? maybeOf(BuildContext context)
    {
        HeroControllerScope? host__34917 = ((HeroControllerScope?)(object?)context.dependOnInheritedWidgetOfExactType<HeroControllerScope>());
        return host__34917?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static HeroController of(BuildContext context)
    {
        HeroController? controller__35615 = ((HeroController?)(object?)HeroControllerScope.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((controller__35615 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("HeroControllerScope.of() was called with a context that does not contain a " + "HeroControllerScope widget.\n" + "No HeroControllerScope widget ancestor could be found starting from the " + "context that was passed to HeroControllerScope.of(). This can happen " + "because you are using a widget that looks for a HeroControllerScope " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return controller__35615!;
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
    public abstract dynamic route { get; }
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
        IEnumerable<RouteTransitionRecord> results__41862 = ((IEnumerable<RouteTransitionRecord>)(object?)resolve(newPageRouteHistory: newPageRouteHistory, locationToExitingPageRoute: locationToExitingPageRoute, pageRouteToPagelessRoutes: pageRouteToPagelessRoutes));
        DartRuntimePrimitives.Assert(() =>
            {
                List<RouteTransitionRecord> resultsToVerify__43187 = results__41862.ToList().ToList();
                HashSet<RouteTransitionRecord> exitingPageRoutes__43277 = locationToExitingPageRoute.Values.toSet();
                foreach (var exitingPageRoute__43432 in exitingPageRoutes__43277)
                {
                    DartRuntimePrimitives.Assert(() => !((RouteTransitionRecord)exitingPageRoute__43432).isWaitingForExitingDecision);
                    if (pageRouteToPagelessRoutes.ContainsKey(exitingPageRoute__43432))
                    {
                        foreach (RouteTransitionRecord pagelessRoute__43650 in pageRouteToPagelessRoutes.GetValueOrDefault(exitingPageRoute__43432)!)
                        {
                            DartRuntimePrimitives.Assert(() => !((RouteTransitionRecord)pagelessRoute__43650).isWaitingForExitingDecision);
                        }
                    }
                }
                var indexOfNextRouteInNewHistory__43960 = 0L;
                foreach (_RouteEntry__navigator routeEntry__44024 in resultsToVerify__43187.cast<_RouteEntry__navigator>())
                {
                    DartRuntimePrimitives.Assert(() => (!((_RouteEntry__navigator)routeEntry__44024).isWaitingForEnteringDecision && !((_RouteEntry__navigator)routeEntry__44024).isWaitingForExitingDecision));
                    if (((indexOfNextRouteInNewHistory__43960 >= checked((long)(newPageRouteHistory.Count))) || (!object.Equals(routeEntry__44024, newPageRouteHistory[(int)(indexOfNextRouteInNewHistory__43960)]))))
                    {
                        DartRuntimePrimitives.Assert(() => exitingPageRoutes__43277.Contains(routeEntry__44024));
                        exitingPageRoutes__43277.Remove(routeEntry__44024);
                    }
                    else
                    {
                        indexOfNextRouteInNewHistory__43960 += 1L;
                    }
                }
                DartRuntimePrimitives.Assert(() => ((indexOfNextRouteInNewHistory__43960 == checked((long)(newPageRouteHistory.Count))) && !System.Linq.Enumerable.Any(exitingPageRoutes__43277)), () => (object?)$"The merged result from the {this.GetType()}.resolve does not include all " + "required routes. Do you remember to merge all exiting routes?");
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return results__41862;
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
        var results__50187 = new List<RouteTransitionRecord>();
        void handleExitingRoute(RouteTransitionRecord? location, bool isLast)
        {
            RouteTransitionRecord? exitingPageRoute__50559 = locationToExitingPageRoute.GetValueOrDefault(DartRuntimePrimitives.RequireReference(location));
            if ((exitingPageRoute__50559 is null))
            {
                return;
            }
            if (((RouteTransitionRecord)exitingPageRoute__50559).isWaitingForExitingDecision)
            {
                bool hasPagelessRoute__50755 = pageRouteToPagelessRoutes.ContainsKey(exitingPageRoute__50559);
                bool isLastExitingPageRoute__50850 = (isLast && !locationToExitingPageRoute.ContainsKey(exitingPageRoute__50559));
                if ((isLastExitingPageRoute__50850 && !hasPagelessRoute__50755))
                {
                    exitingPageRoute__50559.markForPop(((dynamic)((RouteTransitionRecord)exitingPageRoute__50559).route).currentResult);
                }
                else
                {
                    exitingPageRoute__50559.markForComplete(((dynamic)((RouteTransitionRecord)exitingPageRoute__50559).route).currentResult);
                }
                if (hasPagelessRoute__50755)
                {
                    List<RouteTransitionRecord> pagelessRoutes__51277 = pageRouteToPagelessRoutes.GetValueOrDefault(exitingPageRoute__50559)!.ToList();
                    foreach (var pagelessRoute__51375 in pagelessRoutes__51277)
                    {
                        if (((RouteTransitionRecord)pagelessRoute__51375).isWaitingForExitingDecision)
                        {
                            if ((isLastExitingPageRoute__50850 && (object.Equals(pagelessRoute__51375, pagelessRoutes__51277.Last()))))
                            {
                                pagelessRoute__51375.markForPop(((dynamic)((RouteTransitionRecord)pagelessRoute__51375).route).currentResult);
                            }
                            else
                            {
                                pagelessRoute__51375.markForComplete(((dynamic)((RouteTransitionRecord)pagelessRoute__51375).route).currentResult);
                            }
                        }
                    }
                }
            }
            results__50187.Add(exitingPageRoute__50559);
            handleExitingRoute(exitingPageRoute__50559, isLast);
        }
        handleExitingRoute(((RouteTransitionRecord)(object)null), !System.Linq.Enumerable.Any(newPageRouteHistory));
        foreach (var pageRoute__52342 in newPageRouteHistory)
        {
            var isLastIteration__52390 = (object.Equals(newPageRouteHistory.Last(), pageRoute__52342));
            if (((RouteTransitionRecord)pageRoute__52342).isWaitingForEnteringDecision)
            {
                if ((!locationToExitingPageRoute.ContainsKey(pageRoute__52342) && isLastIteration__52390))
                {
                    pageRoute__52342.markForPush();
                }
                else
                {
                    pageRoute__52342.markForAdd();
                }
            }
            results__50187.Add(pageRoute__52342);
            handleExitingRoute(pageRoute__52342, isLastIteration__52390);
        }
        return ((IEnumerable<RouteTransitionRecord>)(object?)results__50187);
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
        Navigator.of(context).replace<T>(oldRoute: oldRoute, newRoute: newRoute);
        return;
    }

    public static string restorableReplace<T>(BuildContext context, dynamic oldRoute, global::System.Func<BuildContext, object, Route<T>> newRouteBuilder, object? arguments = null)
    {
        return ((string)(object?)Navigator.of(context).restorableReplace<T>(oldRoute: oldRoute, newRouteBuilder: (global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void replaceRouteBelow<T>(BuildContext context, dynamic anchorRoute, Route<T> newRoute)
    {
        Navigator.of(context).replaceRouteBelow<T>(anchorRoute: anchorRoute, newRoute: newRoute);
        return;
    }

    public static string restorableReplaceRouteBelow<T>(BuildContext context, dynamic anchorRoute, global::System.Func<BuildContext, object, Route<T>> newRouteBuilder, object? arguments = null)
    {
        return ((string)(object?)Navigator.of(context).restorableReplaceRouteBelow<T>(anchorRoute: anchorRoute, newRouteBuilder: (global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder, arguments: arguments));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool canPop(BuildContext context)
    {
        NavigatorState? navigator__110654 = ((NavigatorState?)(object?)Navigator.maybeOf(context));
        return ((navigator__110654 is not null) && navigator__110654.canPop());
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
        NavigatorState? navigator__119891 = default!;
        if (context is StatefulElement { state: NavigatorState state__119961 } __object119923)
        {
            navigator__119891 = state__119961;
        }
        navigator__119891 = (rootNavigator ? (context.findRootAncestorStateOfType<NavigatorState>() ?? navigator__119891) : ((navigator__119891 ?? (NavigatorState)context.findAncestorStateOfType<NavigatorState>())));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((navigator__119891 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Navigator operation requested with a context that does not include a Navigator.\n" + "The context used to push or pop routes from the Navigator must be that of a " + "widget that is a descendant of a Navigator widget."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return navigator__119891!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static NavigatorState? maybeOf(BuildContext context, bool rootNavigator = false)
    {
        NavigatorState? navigator__121426 = default!;
        if (context is StatefulElement { state: NavigatorState state__121496 } __object121458)
        {
            navigator__121426 = state__121496;
        }
        return (rootNavigator ? (context.findRootAncestorStateOfType<NavigatorState>() ?? navigator__121426) : ((navigator__121426 ?? (NavigatorState)context.findAncestorStateOfType<NavigatorState>())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static List<object> defaultGenerateInitialRoutes(NavigatorState navigator, string initialRouteName)
    {
        var result__122593 = new List<object>();
        if ((initialRouteName.startsWith("/") && (initialRouteName.Length > 1L)))
        {
            initialRouteName = initialRouteName.substring(1L);
            DartRuntimePrimitives.Assert(() => (Navigator.defaultRouteName == "/"));
            List<string>? debugRouteNames__122844 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    debugRouteNames__122844 = new List<string> { Navigator.defaultRouteName };
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            result__122593.Add(navigator._routeNamed<object>(Navigator.defaultRouteName, arguments: null, allowNull: true));
            List<string> routeParts__123171 = initialRouteName.split("/").ToList();
            if ((initialRouteName.Length != 0))
            {
                var routeName__123266 = "";
                foreach (var part__123301 in routeParts__123171)
                {
                    routeName__123266 += $"/{part__123301}";
                    DartRuntimePrimitives.Assert(() =>
                        {
                            debugRouteNames__122844!.Add(routeName__123266);
                            return true;
                            throw new InvalidOperationException("Dart closure completed without a value.");
                        });
                    result__122593.Add(navigator._routeNamed<object>(routeName__123266, arguments: null, allowNull: true));
                }
            }
            if ((result__122593.Last() is null))
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: "Could not navigate to initial route.\n" + $"The requested route name was: \"/{initialRouteName}\"\n" + "There was no corresponding route in the app, and therefore the initial route specified will be " + $"ignored and \"{Navigator.defaultRouteName}\" will be used instead."));
                        return true;
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                foreach (var route__124147 in result__122593)
                {
                    ((dynamic)route__124147)?.dispose();
                }
                result__122593.Clear();
            }
        }
        else
        {
            if ((initialRouteName != Navigator.defaultRouteName))
            {
                result__122593.Add(navigator._routeNamed<object>(initialRouteName, arguments: null, allowNull: true));
            }
        }
        result__122593.removeWhere(((route) => (route is null)));
        if (!System.Linq.Enumerable.Any(result__122593))
        {
            result__122593.Add(navigator._routeNamed<object>(Navigator.defaultRouteName, arguments: null));
        }
        return ((List<object>)(object?)result__122593.cast<object>());
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
    private dynamic __field_route = default!;
    public override dynamic route { get => __field_route; }
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

    internal _RouteEntry__navigator(dynamic route, _RouteLifecycle__navigator initialState, bool pageBased, _RestorationInformation__navigator? restorationInformation = null)
    {
        this.__field_route = route;
        this.pageBased = pageBased;
        this.restorationInformation = restorationInformation;
        this.currentState = initialState;
        System.Diagnostics.Debug.Assert((!pageBased || (((RouteSettings)((dynamic)route).settings) is Page<object>)));
        System.Diagnostics.Debug.Assert((((((object.Equals(initialState, _RouteLifecycle__navigator.staging)) || (object.Equals(initialState, _RouteLifecycle__navigator.add))) || (object.Equals(initialState, _RouteLifecycle__navigator.push))) || (object.Equals(initialState, _RouteLifecycle__navigator.pushReplace))) || (object.Equals(initialState, _RouteLifecycle__navigator.replace))));
    }

    public virtual string? restorationId
    {
        get
        {
            if (this.pageBased)
            {
                var page__131174 = ((Page<object?>?)(object?)((RouteSettings)((dynamic)this.route).settings))!;
                return ((((Page<object>)page__131174).restorationId is not null) ? $"p+{((Page<object>)page__131174).restorationId}" : null);
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
        var routePage__131583 = ((Page<object>?)(object?)((RouteSettings)((dynamic)this.route).settings))!;
        return page.canUpdate(routePage__131583);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleAdd(NavigatorState navigator, dynamic previousPresent)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this.currentState, _RouteLifecycle__navigator.add)));
        DartRuntimePrimitives.Assert(() => ((NavigatorState)navigator)._debugLocked);
        currentState = _RouteLifecycle__navigator.adding;
        ((NavigatorState)navigator)._observedRouteAdditions.Enqueue(new _NavigatorPushObservation__navigator(this.route, previousPresent));
    }

    public virtual void handlePush(NavigatorState navigator, bool isNewFirst, dynamic previous, dynamic previousPresent)
    {
        DartRuntimePrimitives.Assert(() => (((object.Equals(this.currentState, _RouteLifecycle__navigator.push)) || (object.Equals(this.currentState, _RouteLifecycle__navigator.pushReplace))) || (object.Equals(this.currentState, _RouteLifecycle__navigator.replace))));
        DartRuntimePrimitives.Assert(() => ((NavigatorState)navigator)._debugLocked);
        DartRuntimePrimitives.Assert(() => !((bool)((dynamic)this.route)._installed), () => (object?)"The pushed route has already been used. When pushing a route, a new " + "Route object must be provided.");
        _RouteLifecycle__navigator previousState__132573 = this.currentState;
        ((dynamic)this.route)._navigator = navigator;
        ((dynamic)this.route).install();
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(((List<OverlayEntry>)((dynamic)this.route).overlayEntries)));
        if (((object.Equals(this.currentState, _RouteLifecycle__navigator.push)) || (object.Equals(this.currentState, _RouteLifecycle__navigator.pushReplace))))
        {
            global::Doroti.Framework.Scheduler.TickerFuture routeFuture__132823 = ((global::Doroti.Framework.Scheduler.TickerFuture)(object?)((global::Doroti.Framework.Scheduler.TickerFuture)((dynamic)this.route).didPush()));
            currentState = _RouteLifecycle__navigator.pushing;
            routeFuture__132823.whenCompleteOrCancel(((global::System.Action)(() => {
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
            ((dynamic)this.route).didReplace(previous);
            currentState = _RouteLifecycle__navigator.idle;
        }
        if (isNewFirst)
        {
            ((dynamic)this.route).didChangeNext(null);
        }
        if (((object.Equals(previousState__132573, _RouteLifecycle__navigator.replace)) || (object.Equals(previousState__132573, _RouteLifecycle__navigator.pushReplace))))
        {
            ((NavigatorState)navigator)._observedRouteAdditions.Enqueue(new _NavigatorReplaceObservation__navigator(this.route, previousPresent));
            if (((previousPresent is not null) && ((bool)((dynamic)previousPresent)._isPageBased)))
            {
                var page__133861 = ((Page<object?>?)(object?)((RouteSettings)((dynamic)previousPresent).settings))!;
                navigator.widget.onDidRemovePage?.Invoke(page__133861);
            }
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(previousState__132573, _RouteLifecycle__navigator.push)));
            ((NavigatorState)navigator)._observedRouteAdditions.Enqueue(new _NavigatorPushObservation__navigator(this.route, previousPresent));
        }
    }

    public virtual void handleDidPopNext(dynamic poppedRoute)
    {
        ((dynamic)this.route).didPopNext(poppedRoute);
        lastAnnouncedPoppedNextRoute = new WeakReference<object>(poppedRoute);
        if ((this.lastFocusNode is not null))
        {
            DartRuntimePrimitives.Ignore(((Completer<object>)((dynamic)poppedRoute)._disposeCompleter).future.then((global::System.Func<object, Future<object>>)(async (result) => {
switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
{
    case global::Doroti.Framework.Foundation.TargetPlatform.android:
        {
            long? reFocusNode__134891 = this.lastFocusNode;
            await new Future(NavigatorLibrary._kAndroidRefocusingDelayDuration);
            await global::Doroti.Framework.Services.SystemChannels.accessibility.send(new global::Doroti.Framework.Semantics.FocusSemanticEvent().toMap(nodeId: reFocusNode__134891));
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
})).catchError(((global::System.Action<object, global::System.Diagnostics.StackTrace>)((error, stackTrace) => {
FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stackTrace, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while restoring focus in the navigator")));
}))));
        }
    }

    public virtual bool handlePop(NavigatorState navigator, dynamic previousPresent)
    {
        DartRuntimePrimitives.Assert(() => ((NavigatorState)navigator)._debugLocked);
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)this.route)._isInstalledIn(navigator)));
        currentState = _RouteLifecycle__navigator.popping;
        if ((bool)((dynamic)this.route)._popCompleter.isCompleted)
        {
            DartRuntimePrimitives.Assert(() => this.pageBased);
            DartRuntimePrimitives.Assert(() => (this.pendingResult is null));
            return true;
        }
        if (!((bool)((dynamic)this.route).didPop((dynamic?)this.pendingResult)))
        {
            currentState = _RouteLifecycle__navigator.idle;
            return false;
        }
        ((dynamic)this.route).onPopInvokedWithResult(true, (dynamic?)this.pendingResult);
        if ((this.pageBased && this.imperativeRemoval))
        {
            var page__136817 = ((Page<object?>?)(object?)((RouteSettings)((dynamic)this.route).settings))!;
            navigator.widget.onDidRemovePage?.Invoke(page__136817);
        }
        pendingResult = null;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleComplete()
    {
        ((dynamic)this.route).didComplete((dynamic?)this.pendingResult);
        pendingResult = null;
        DartRuntimePrimitives.Assert(() => ((Completer<object>)((dynamic)this.route)._popCompleter).isCompleted);
        currentState = _RouteLifecycle__navigator.remove;
    }

    public virtual void handleRemoval(NavigatorState navigator, dynamic previousPresent)
    {
        DartRuntimePrimitives.Assert(() => ((NavigatorState)navigator)._debugLocked);
        if (((bool)((dynamic)this.route)._isInstalledIn(navigator)))
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
        DartRuntimePrimitives.Assert(() => !((bool)((dynamic)this.route)._installed));
        ((dynamic)this.route)._navigator = navigator;
        ((dynamic)this.route).install();
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(((List<OverlayEntry>)((dynamic)this.route).overlayEntries)));
        ((dynamic)this.route).didAdd();
        currentState = _RouteLifecycle__navigator.idle;
        if (isNewFirst)
        {
            ((dynamic)this.route).didChangeNext(null);
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
        ((dynamic)this.route).dispose();
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => (FoundationRuntimePorts.EnumIndex(this.currentState) < FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.disposing)));
        currentState = _RouteLifecycle__navigator.disposing;
        IEnumerable<OverlayEntry> mountedEntries__140253 = ((List<OverlayEntry>)((dynamic)this.route).overlayEntries).where(((e) => ((OverlayEntry)e).mounted));
        if (!System.Linq.Enumerable.Any(mountedEntries__140253))
        {
            forcedDispose();
            return;
        }
        long mounted__140429 = mountedEntries__140253.Count();
        DartRuntimePrimitives.Assert(() => (mounted__140429 > 0L));
        NavigatorState navigator__140512 = ((NavigatorState?)((dynamic)this.route)._navigator)!;
        ((NavigatorState)navigator__140512)._entryWaitingForSubTreeDisposal.Add(this);
        foreach (var entry__140615 in mountedEntries__140253)
        {
            global::System.Action listener__140666 = default!;
            listener__140666 = (global::System.Action)(() => {
DartRuntimePrimitives.Assert(() => (mounted__140429 > 0L));
DartRuntimePrimitives.Assert(() => !((OverlayEntry)entry__140615).mounted);
mounted__140429--;
entry__140615.removeListener(() => listener__140666());
if ((mounted__140429 == 0L))
{
    DartRuntimePrimitives.Assert(() => ((List<OverlayEntry>)((dynamic)this.route).overlayEntries).All(((e) => !((OverlayEntry)e).mounted)));
    DartAsyncRuntime.scheduleMicrotask((() => {
if (!((NavigatorState)navigator__140512)._entryWaitingForSubTreeDisposal.Remove(this))
{
    DartRuntimePrimitives.Assert(() => (!((bool)((dynamic)this.route)._installed) && !navigator__140512.mounted));
    return;
}
DartRuntimePrimitives.Assert(() => (object.Equals(this.currentState, _RouteLifecycle__navigator.disposing)));
forcedDispose();
}));
    return;
}
});
            entry__140615.addListener(() => listener__140666());
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
    public static global::System.Func<_RouteEntry__navigator, bool> isRoutePredicate(dynamic route)
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
        var attempt__144676 = 0L;
        while (((bool)((dynamic)this.route).willHandlePopInternally))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    attempt__144676 += 1L;
                    return (attempt__144676 < kDebugPopAttemptLimit);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }, () => (object?)$"Attempted to pop {this.route} {kDebugPopAttemptLimit} times, but still failed");
            bool popResult__144924 = ((bool)((dynamic)this.route).didPop(result));
            DartRuntimePrimitives.Assert(() => !popResult__144924);
        }
        pop<object>(result, imperativeRemoval: false);
        _isWaitingForExitingDecision = false;
    }

    public override void markForComplete(dynamic result = default!)
    {
        DartRuntimePrimitives.Assert(() => ((!this.isWaitingForEnteringDecision && this.isWaitingForExitingDecision) && this.isPresent), () => (object?)"This route cannot be marked for complete. Either a decision has already " + "been made or it does not require an explicit decision on how to transition " + "out.");
        complete<object>(result, isReplaced: false, imperativeRemoval: false);
        _isWaitingForExitingDecision = false;
    }

    public virtual bool restorationEnabled
    {
        get => (((global::Doroti.Framework.Foundation.ValueListenable<string?>)((dynamic)this.route).restorationScopeId).value is not null);
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (!__value || (this.restorationId is not null)));
            ((dynamic)this.route)._updateRestorationId((__value ? this.restorationId : null));
        }
    }
}

internal abstract class _NavigatorObservation__navigator
{
    public virtual dynamic primaryRoute { get; private set; } = default!;
    public virtual dynamic secondaryRoute { get; private set; } = default!;

    internal _NavigatorObservation__navigator(dynamic primaryRoute, dynamic secondaryRoute)
    {
        this.primaryRoute = primaryRoute;
        this.secondaryRoute = secondaryRoute;
    }

    public abstract void notify(NavigatorObserver observer);
}

internal class _NavigatorPushObservation__navigator : _NavigatorObservation__navigator
{
    internal _NavigatorPushObservation__navigator(dynamic primaryRoute, dynamic secondaryRoute) : base((object?)primaryRoute, (object?)secondaryRoute)
    {
    }

    public override void notify(NavigatorObserver observer)
    {
        observer.didPush(this.primaryRoute, this.secondaryRoute);
    }

}

internal class _NavigatorPopObservation__navigator : _NavigatorObservation__navigator
{
    internal _NavigatorPopObservation__navigator(dynamic primaryRoute, dynamic secondaryRoute) : base((object?)primaryRoute, (object?)secondaryRoute)
    {
    }

    public override void notify(NavigatorObserver observer)
    {
        observer.didPop(this.primaryRoute, this.secondaryRoute);
    }

}

internal class _NavigatorRemoveObservation__navigator : _NavigatorObservation__navigator
{
    internal _NavigatorRemoveObservation__navigator(dynamic primaryRoute, dynamic secondaryRoute) : base((object?)primaryRoute, (object?)secondaryRoute)
    {
    }

    public override void notify(NavigatorObserver observer)
    {
        observer.didRemove(this.primaryRoute, this.secondaryRoute);
    }

}

internal class _NavigatorReplaceObservation__navigator : _NavigatorObservation__navigator
{
    internal _NavigatorReplaceObservation__navigator(dynamic primaryRoute, dynamic secondaryRoute) : base((object?)primaryRoute, (object?)secondaryRoute)
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
        bool valueWasEmpty__147926 = !System.Linq.Enumerable.Any(this._value);
        this._value.Clear();
        if (!valueWasEmpty__147926)
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
        _RouteEntry__navigator entry__148209 = ((_RouteEntry__navigator)(object?)this._value.removeAt(index));
        notifyListeners();
        return entry__148209;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _RouteEntry__navigator removeLast()
    {
        _RouteEntry__navigator entry__148338 = ((_RouteEntry__navigator)(object?)this._value.removeLast<_RouteEntry__navigator>());
        notifyListeners();
        return entry__148338;
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
                    global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) => {
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
        _RouteEntry__navigator? lastEntry__150924 = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
        return ((lastEntry__150924 is not null) && (object.Equals(((RoutePopDisposition)((dynamic)((_RouteEntry__navigator)lastEntry__150924).route).popDisposition), RoutePopDisposition.doNotPop)));
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
        foreach (NavigatorObserver observer__152109 in ((Navigator)(object)this.widget).observers)
        {
            DartRuntimePrimitives.Assert(() => (((NavigatorObserver)observer__152109).navigator is null));
            NavigatorObserver._navigators[observer__152109] = this;
        }
        _effectiveObservers = ((Navigator)(object)this.widget).observers;
        var heroControllerScope__152428 = ((HeroControllerScope?)(object?)this.context.getElementForInheritedWidgetOfExactType<HeroControllerScope>()?.widget)!;
        _updateHeroController(heroControllerScope__152428?.controller);
        if (((Navigator)(object)this.widget).reportsRouteUpdateToEngine)
        {
            DartRuntimePrimitives.Ignore(SystemNavigator.selectSingleEntryHistory());
        }
        global::Doroti.Framework.Services.ServicesBinding.instance.accessibilityFocus.addListener(() => this._recordLastFocus());
        this._history.addListener(() => this._handleHistoryChanged());
    }

    internal virtual void _recordLastFocus()
    {
        _RouteEntry__navigator? entry__152971 = this._history.where(_RouteEntry__navigator.isPresentPredicate).LastOrDefault();
        entry__152971?.lastFocusNode = global::Doroti.Framework.Services.ServicesBinding.instance.accessibilityFocus.value;
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
        foreach (Page<object> page__153933 in ((Navigator)(object)this.widget).pages)
        {
            var entry__153969 = new _RouteEntry__navigator(page__153933.createRoute(this.context), pageBased: true, initialState: _RouteLifecycle__navigator.add);
            DartRuntimePrimitives.Assert(() => (object.Equals(((RouteSettings)((dynamic)((_RouteEntry__navigator)entry__153969).route).settings), page__153933)), () => (object?)"The settings getter of a page-based Route must return a Page object. " + "Please set the settings to the Page in the Page.createRoute method.");
            this._history.add(entry__153969);
            this._history.addAll(this._serializableHistory.restoreEntriesForPage(entry__153969, this));
        }
        if (!((_HistoryProperty__navigator)this._serializableHistory).hasData)
        {
            string? initialRoute__154569 = ((Navigator)(object)this.widget).initialRoute;
            if (!System.Linq.Enumerable.Any(((Navigator)(object)this.widget).pages))
            {
                initialRoute__154569 ??= Navigator.defaultRouteName;
            }
            if ((initialRoute__154569 is not null))
            {
                this._history.addAll(this.widget.onGenerateInitialRoutes(this, (((Navigator)(object)this.widget).initialRoute ?? Navigator.defaultRouteName)).map<dynamic, _RouteEntry__navigator>(((route) => new _RouteEntry__navigator(route, pageBased: false, initialState: _RouteLifecycle__navigator.add, restorationInformation: ((((RouteSettings)((dynamic)route).settings).ToString() is not null) ? _RestorationInformation__navigator.CreateNamed(name: ((RouteSettings)((dynamic)route).settings).ToString()!, arguments: null, restorationScopeId: this._nextPagelessRestorationScopeId) : null)))).Cast<_RouteEntry__navigator>());
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
        global::Doroti.Framework.Services.RestorationBucket? oldBucket__41020 = this._bucket;
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
        _updateHeroController(HeroControllerScope.maybeOf(this.context));
        foreach (_RouteEntry__navigator entry__156496 in this._history)
        {
            if ((object.Equals(((NavigatorState?)((dynamic)((_RouteEntry__navigator)entry__156496).route).navigator), this)))
            {
                ((dynamic)((_RouteEntry__navigator)entry__156496).route).changedExternalState();
            }
        }
    }

    internal virtual void _forcedDisposeAllRouteEntries()
    {
        this._entryWaitingForSubTreeDisposal.removeWhere(((entry) => {
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
        foreach (OverlayEntry overlayEntry__157065 in ((List<OverlayEntry>)((dynamic)((_RouteEntry__navigator)entry).route).overlayEntries))
        {
            if (overlayEntry__157065._overlay is not null)
            {
                overlayEntry__157065.remove();
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
                            NavigatorState previousOwner__157803 = newHeroController.navigator!;
                            global::Doroti.Framework.Services.ServicesBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) => {
if ((object.Equals(this._heroControllerFromScope, newHeroController)))
{
    var hasHeroControllerOwnerShip__158100 = (object.Equals(this._heroControllerFromScope!.navigator, this));
    if ((!hasHeroControllerOwnerShip__158100 || (object.Equals(((NavigatorState)previousOwner__157803)._heroControllerFromScope, newHeroController))))
    {
        NavigatorState otherOwner__158347 = (hasHeroControllerOwnerShip__158100 ? previousOwner__157803 : this._heroControllerFromScope!.navigator!);
        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Framework.Foundation.FlutterError.Create("A HeroController can not be shared by multiple Navigators. " + "The Navigators that share the same HeroController are:\n" + $"- {this}\n" + $"- {otherOwner__158347}\n" + "Please create a HeroControllerScope for each Navigator or " + "use a HeroControllerScope.none to prevent subtree from " + "receiving a HeroController."), library: "widget library", stack: new global::System.Diagnostics.StackTrace(true)));
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
            foreach (NavigatorObserver observer__160307 in ((Navigator)oldWidget).observers)
            {
                NavigatorObserver._navigators[observer__160307] = DartRuntimePrimitives.ConvertValue<NavigatorState>(null);
            }
            foreach (NavigatorObserver observer__160441 in ((Navigator)(object)this.widget).observers)
            {
                DartRuntimePrimitives.Assert(() => (((NavigatorObserver)observer__160441).navigator is null));
                NavigatorObserver._navigators[observer__160441] = this;
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
        foreach (_RouteEntry__navigator entry__161179 in this._history)
        {
            if ((object.Equals(((NavigatorState?)((dynamic)((_RouteEntry__navigator)entry__161179).route).navigator), this)))
            {
                ((dynamic)((_RouteEntry__navigator)entry__161179).route).changedExternalState();
            }
        }
    }

    internal virtual void _debugCheckDuplicatedPageKeys()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var keyReservation__161375 = new HashSet<global::Doroti.Framework.Foundation.Key>();
                foreach (Page<object> page__161432 in ((Navigator)(object)this.widget).pages)
                {
                    global::Doroti.Framework.Foundation.LocalKey? key__161480 = ((Page<object>)page__161432).key;
                    if ((key__161480 is not null))
                    {
                        DartRuntimePrimitives.Assert(() => !keyReservation__161375.Contains(key__161480));
                        keyReservation__161375.Add(key__161480);
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override void deactivate()
    {
        foreach (NavigatorObserver observer__161739 in this._effectiveObservers)
        {
            NavigatorObserver._navigators[observer__161739] = DartRuntimePrimitives.ConvertValue<NavigatorState>(null);
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
        foreach (NavigatorObserver observer__162045 in this._effectiveObservers)
        {
            DartRuntimePrimitives.Assert(() => (((NavigatorObserver)observer__162045).navigator is null));
            NavigatorObserver._navigators[observer__162045] = this;
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
        global::Doroti.Framework.Services.ServicesBinding.instance.accessibilityFocus.removeListener(() => this._recordLastFocus());
        this._history.removeListener(() => this._handleHistoryChanged());
        this._history.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) => {
if (!((dynamic)property)._disposed)
{
    property.removeListener((global::System.Action)(() => listener()));
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
                .SelectMany(entry => ((List<OverlayEntry>)((dynamic)entry.route).overlayEntries));
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
        var needsExplicitDecision__165578 = false;
        var newPagesBottom__165617 = 0L;
        var oldEntriesBottom__165645 = 0L;
        long newPagesTop__165675 = (checked((long)(((Navigator)(object)this.widget).pages.Count)) - 1L);
        long oldEntriesTop__165722 = (this._history.Count() - 1L);
        var newHistory__165770 = new List<_RouteEntry__navigator>();
        var pageRouteToPagelessRoutes__165810 = new DartMap<_RouteEntry__navigator?, List<_RouteEntry__navigator>>();
        _RouteEntry__navigator? previousOldPageRouteEntry__165932 = default!;
        while ((oldEntriesBottom__165645 <= oldEntriesTop__165722))
        {
            _RouteEntry__navigator oldEntry__166031 = this._history[oldEntriesBottom__165645];
            DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)oldEntry__166031).currentState, _RouteLifecycle__navigator.disposed)));
            if (!((_RouteEntry__navigator)oldEntry__166031).pageBased)
            {
                List<_RouteEntry__navigator> pagelessRoutes__166304 = pageRouteToPagelessRoutes__165810.putIfAbsent(previousOldPageRouteEntry__165932, (() => new List<_RouteEntry__navigator>())).ToList();
                pagelessRoutes__166304.Add(oldEntry__166031);
                oldEntriesBottom__165645 += 1L;
                continue;
            }
            if ((newPagesBottom__165617 > newPagesTop__165675))
            {
                break;
            }
            Page<object> newPage__166627 = ((Navigator)(object)this.widget).pages[(int)(newPagesBottom__165617)];
            if (!oldEntry__166031.canUpdateFrom(newPage__166627))
            {
                break;
            }
            previousOldPageRouteEntry__165932 = oldEntry__166031;
            ((dynamic)((_RouteEntry__navigator)oldEntry__166031).route)._updateSettings(newPage__166627);
            newHistory__165770.Add(oldEntry__166031);
            newPagesBottom__165617 += 1L;
            oldEntriesBottom__165645 += 1L;
        }
        var unattachedPagelessRoutes__166932 = new List<_RouteEntry__navigator>();
        while ((((oldEntriesBottom__165645 <= oldEntriesTop__165722)) && ((newPagesBottom__165617 <= newPagesTop__165675))))
        {
            _RouteEntry__navigator oldEntry__167183 = this._history[oldEntriesTop__165722];
            DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)oldEntry__167183).currentState, _RouteLifecycle__navigator.disposed)));
            if (!((_RouteEntry__navigator)oldEntry__167183).pageBased)
            {
                unattachedPagelessRoutes__166932.Add(oldEntry__167183);
                oldEntriesTop__165722 -= 1L;
                continue;
            }
            Page<object> newPage__167445 = ((Navigator)(object)this.widget).pages[(int)(newPagesTop__165675)];
            if (!oldEntry__167183.canUpdateFrom(newPage__167445))
            {
                break;
            }
            if (System.Linq.Enumerable.Any(unattachedPagelessRoutes__166932))
            {
                pageRouteToPagelessRoutes__165810.putIfAbsent(oldEntry__167183, (() => new List<_RouteEntry__navigator>(DartRuntimePrimitives.ConvertEnumerable<_RouteEntry__navigator>(unattachedPagelessRoutes__166932))));
                unattachedPagelessRoutes__166932.Clear();
            }
            oldEntriesTop__165722 -= 1L;
            newPagesTop__165675 -= 1L;
        }
        oldEntriesTop__165722 += checked((long)(unattachedPagelessRoutes__166932.Count));
        var oldEntriesBottomToScan__168179 = oldEntriesBottom__165645;
        var pageKeyToOldEntry__168232 = new DartMap<global::Doroti.Framework.Foundation.LocalKey, _RouteEntry__navigator>();
        var phantomEntries__168390 = new HashSet<_RouteEntry__navigator>();
        while ((oldEntriesBottomToScan__168179 <= oldEntriesTop__165722))
        {
            _RouteEntry__navigator oldEntry__168502 = this._history[oldEntriesBottomToScan__168179];
            oldEntriesBottomToScan__168179 += 1L;
            DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)oldEntry__168502).currentState, _RouteLifecycle__navigator.disposed)));
            if (!((_RouteEntry__navigator)oldEntry__168502).pageBased)
            {
                continue;
            }
            var page__168813 = ((Page<object>?)(object?)((RouteSettings)((dynamic)((_RouteEntry__navigator)oldEntry__168502).route).settings))!;
            if ((((Page<object>)page__168813).key is null))
            {
                continue;
            }
            if (!((_RouteEntry__navigator)oldEntry__168502).willBePresent)
            {
                phantomEntries__168390.Add(oldEntry__168502);
                continue;
            }
            DartRuntimePrimitives.Assert(() => !pageKeyToOldEntry__168232.ContainsKey(((Page<object>)page__168813).key));
            pageKeyToOldEntry__168232[((Page<object>)page__168813).key!] = oldEntry__168502;
        }
        while ((newPagesBottom__165617 <= newPagesTop__165675))
        {
            Page<object> nextPage__169239 = ((Navigator)(object)this.widget).pages[(int)(newPagesBottom__165617)];
            newPagesBottom__165617 += 1L;
            if ((((((Page<object>)nextPage__169239).key is null) || !pageKeyToOldEntry__168232.ContainsKey(((Page<object>)nextPage__169239).key)) || !pageKeyToOldEntry__168232.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((Page<object>)nextPage__169239).key))!.canUpdateFrom(nextPage__169239)))
            {
                var newEntry__169671 = new _RouteEntry__navigator(nextPage__169239.createRoute(this.context), pageBased: true, initialState: _RouteLifecycle__navigator.staging);
                needsExplicitDecision__165578 = true;
                DartRuntimePrimitives.Assert(() => (object.Equals(((RouteSettings)((dynamic)((_RouteEntry__navigator)newEntry__169671).route).settings), nextPage__169239)), () => (object?)"The settings getter of a page-based Route must return a Page object. " + "Please set the settings to the Page in the Page.createRoute method.");
                newHistory__165770.Add(newEntry__169671);
            }
            else
            {
                _RouteEntry__navigator matchingEntry__170248 = pageKeyToOldEntry__168232.remove(((Page<object>)nextPage__169239).key)!;
                DartRuntimePrimitives.Assert(() => matchingEntry__170248.canUpdateFrom(nextPage__169239));
                ((dynamic)((_RouteEntry__navigator)matchingEntry__170248).route)._updateSettings(nextPage__169239);
                newHistory__165770.Add(matchingEntry__170248);
            }
        }
        var locationToExitingPageRoute__170561 = new DartMap<RouteTransitionRecord?, RouteTransitionRecord>();
        while ((oldEntriesBottom__165645 <= oldEntriesTop__165722))
        {
            _RouteEntry__navigator potentialEntryToRemove__170713 = this._history[oldEntriesBottom__165645];
            oldEntriesBottom__165645 += 1L;
            if (!((_RouteEntry__navigator)potentialEntryToRemove__170713).pageBased)
            {
                DartRuntimePrimitives.Assert(() => (previousOldPageRouteEntry__165932 is not null));
                List<_RouteEntry__navigator> pagelessRoutes__170926 = pageRouteToPagelessRoutes__165810.putIfAbsent(previousOldPageRouteEntry__165932, (() => new List<_RouteEntry__navigator>())).ToList();
                pagelessRoutes__170926.Add(potentialEntryToRemove__170713);
                if ((previousOldPageRouteEntry__165932!.isWaitingForExitingDecision && ((_RouteEntry__navigator)potentialEntryToRemove__170713).willBePresent))
                {
                    potentialEntryToRemove__170713.markNeedsExitingDecision();
                }
                continue;
            }
            var potentialPageToRemove__171347 = ((Page<object>?)(object?)((RouteSettings)((dynamic)((_RouteEntry__navigator)potentialEntryToRemove__170713).route).settings))!;
            if ((((((Page<object>)potentialPageToRemove__171347).key is null) || pageKeyToOldEntry__168232.ContainsKey(((Page<object>)potentialPageToRemove__171347).key)) || phantomEntries__168390.Contains(potentialEntryToRemove__170713)))
            {
                locationToExitingPageRoute__170561[DartRuntimePrimitives.RequireReference(previousOldPageRouteEntry__165932)] = DartRuntimePrimitives.ConvertValue<RouteTransitionRecord>(potentialEntryToRemove__170713);
                if (((_RouteEntry__navigator)potentialEntryToRemove__170713).willBePresent)
                {
                    potentialEntryToRemove__170713.markNeedsExitingDecision();
                }
            }
            previousOldPageRouteEntry__165932 = potentialEntryToRemove__170713;
        }
        DartRuntimePrimitives.Assert(() => (oldEntriesBottom__165645 == (oldEntriesTop__165722 + 1L)));
        DartRuntimePrimitives.Assert(() => (newPagesBottom__165617 == (newPagesTop__165675 + 1L)));
        newPagesTop__165675 = (checked((long)(((Navigator)(object)this.widget).pages.Count)) - 1L);
        oldEntriesTop__165722 = (this._history.Count() - 1L);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((oldEntriesBottom__165645 <= oldEntriesTop__165722))
                {
                    return (((newPagesBottom__165617 <= newPagesTop__165675) && this._history[oldEntriesBottom__165645].pageBased) && this._history[oldEntriesBottom__165645].canUpdateFrom(((Navigator)(object)this.widget).pages[(int)(newPagesBottom__165617)]));
                }
                else
                {
                    return (newPagesBottom__165617 > newPagesTop__165675);
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        while ((((oldEntriesBottom__165645 <= oldEntriesTop__165722)) && ((newPagesBottom__165617 <= newPagesTop__165675))))
        {
            _RouteEntry__navigator oldEntry__172952 = this._history[oldEntriesBottom__165645];
            DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)oldEntry__172952).currentState, _RouteLifecycle__navigator.disposed)));
            if (!((_RouteEntry__navigator)oldEntry__172952).pageBased)
            {
                DartRuntimePrimitives.Assert(() => (previousOldPageRouteEntry__165932 is not null));
                List<_RouteEntry__navigator> pagelessRoutes__173172 = pageRouteToPagelessRoutes__165810.putIfAbsent(previousOldPageRouteEntry__165932, (() => new List<_RouteEntry__navigator>())).ToList();
                pagelessRoutes__173172.Add(oldEntry__172952);
                continue;
            }
            previousOldPageRouteEntry__165932 = oldEntry__172952;
            Page<object> newPage__173443 = ((Navigator)(object)this.widget).pages[(int)(newPagesBottom__165617)];
            DartRuntimePrimitives.Assert(() => oldEntry__172952.canUpdateFrom(newPage__173443));
            ((dynamic)((_RouteEntry__navigator)oldEntry__172952).route)._updateSettings(newPage__173443);
            newHistory__165770.Add(oldEntry__172952);
            oldEntriesBottom__165645 += 1L;
            newPagesBottom__165617 += 1L;
        }
        needsExplicitDecision__165578 = (needsExplicitDecision__165578 || System.Linq.Enumerable.Any(locationToExitingPageRoute__170561));
        IEnumerable<_RouteEntry__navigator> results__173868 = ((IEnumerable<_RouteEntry__navigator>)(object?)newHistory__165770);
        if (needsExplicitDecision__165578)
        {
            results__173868 = ((Navigator)(object)this.widget).transitionDelegate._transition(newPageRouteHistory: newHistory__165770.Cast<RouteTransitionRecord>().ToList(), locationToExitingPageRoute: locationToExitingPageRoute__170561, pageRouteToPagelessRoutes: pageRouteToPagelessRoutes__165810.cast<RouteTransitionRecord?, List<RouteTransitionRecord>>()).cast<_RouteEntry__navigator>();
        }
        this._history.clear();
        if (pageRouteToPagelessRoutes__165810.ContainsKey(((_RouteEntry__navigator)(object)null)))
        {
            this._history.addAll(pageRouteToPagelessRoutes__165810.GetValueOrDefault(null)!.Cast<_RouteEntry__navigator>());
        }
        foreach (var result__174430 in results__173868)
        {
            this._history.add(result__174430);
            if (pageRouteToPagelessRoutes__165810.ContainsKey(result__174430))
            {
                this._history.addAll(pageRouteToPagelessRoutes__165810.GetValueOrDefault(result__174430)!.Cast<_RouteEntry__navigator>());
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
        long index__175362 = (this._history.Count() - 1L);
        _RouteEntry__navigator? next__175408 = default!;
        _RouteEntry__navigator? entry__175431 = this._history[index__175362];
        _RouteEntry__navigator? previous__175473 = ((index__175362 > 0L) ? this._history[(index__175362 - 1L)] : null);
        var canRemoveOrAdd__175532 = false;
        dynamic poppedRoute__175680 = default!;
        var seenTopActiveRoute__175770 = false;
        var toBeDisposed__175867 = new List<_RouteEntry__navigator>();
        while ((index__175362 >= 0L))
        {
            switch (entry__175431!.currentState)
            {
                case _RouteLifecycle__navigator.add:
                    {
                        DartRuntimePrimitives.Assert(() => rearrangeOverlay);
                        entry__175431.handleAdd(navigator: this, previousPresent: _getRouteBefore((index__175362 - 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate)?.route);
                        DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry__175431).currentState, _RouteLifecycle__navigator.adding)));
                        continue;
                    }
                case _RouteLifecycle__navigator.adding:
                    {
                        if ((canRemoveOrAdd__175532 || (next__175408 is null)))
                        {
                            entry__175431.didAdd(navigator: this, isNewFirst: (next__175408 is null));
                            DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry__175431).currentState, _RouteLifecycle__navigator.idle)));
                            continue;
                        }
                        break;
                    }
                case _RouteLifecycle__navigator.push:
                case _RouteLifecycle__navigator.pushReplace:
                case _RouteLifecycle__navigator.replace:
                    {
                        DartRuntimePrimitives.Assert(() => rearrangeOverlay);
                        entry__175431.handlePush(navigator: this, previous: previous__175473?.route, previousPresent: _getRouteBefore((index__175362 - 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate)?.route, isNewFirst: (next__175408 is null));
                        DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)entry__175431).currentState, _RouteLifecycle__navigator.push)));
                        DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)entry__175431).currentState, _RouteLifecycle__navigator.pushReplace)));
                        DartRuntimePrimitives.Assert(() => (!object.Equals(((_RouteEntry__navigator)entry__175431).currentState, _RouteLifecycle__navigator.replace)));
                        if ((object.Equals(((_RouteEntry__navigator)entry__175431).currentState, _RouteLifecycle__navigator.idle)))
                        {
                            continue;
                        }
                        break;
                    }
                case _RouteLifecycle__navigator.pushing:
                    {
                        if ((!seenTopActiveRoute__175770 && (poppedRoute__175680 is not null)))
                        {
                            entry__175431.handleDidPopNext(poppedRoute__175680);
                        }
                        seenTopActiveRoute__175770 = true;
                        break;
                    }
                case _RouteLifecycle__navigator.idle:
                    {
                        if ((!seenTopActiveRoute__175770 && (poppedRoute__175680 is not null)))
                        {
                            entry__175431.handleDidPopNext(poppedRoute__175680);
                        }
                        seenTopActiveRoute__175770 = true;
                        canRemoveOrAdd__175532 = true;
                        break;
                    }
                case _RouteLifecycle__navigator.pop:
                    {
                        if (!entry__175431.handlePop(navigator: this, previousPresent: _getRouteBefore(index__175362, (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.willBePresentPredicate)?.route))
                        {
                            DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry__175431).currentState, _RouteLifecycle__navigator.idle)));
                            continue;
                        }
                        if (!seenTopActiveRoute__175770)
                        {
                            if ((poppedRoute__175680 is not null))
                            {
                                entry__175431.handleDidPopNext(poppedRoute__175680);
                            }
                            poppedRoute__175680 = ((_RouteEntry__navigator)entry__175431).route;
                        }
                        this._observedRouteDeletions.Enqueue(new _NavigatorPopObservation__navigator(((_RouteEntry__navigator)entry__175431).route, _getRouteBefore(index__175362, (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.willBePresentPredicate)?.route));
                        if ((object.Equals(((_RouteEntry__navigator)entry__175431).currentState, _RouteLifecycle__navigator.dispose)))
                        {
                            continue;
                        }
                        DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry__175431).currentState, _RouteLifecycle__navigator.popping)));
                        canRemoveOrAdd__175532 = true;
                        break;
                    }
                case _RouteLifecycle__navigator.popping:
                    {
                        break;
                    }
                case _RouteLifecycle__navigator.complete:
                    {
                        entry__175431.handleComplete();
                        DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry__175431).currentState, _RouteLifecycle__navigator.remove)));
                        continue;
                    }
                case _RouteLifecycle__navigator.remove:
                    {
                        if ((!seenTopActiveRoute__175770 && ((bool)((dynamic)((_RouteEntry__navigator)entry__175431).route)._installed)))
                        {
                            if ((poppedRoute__175680 is not null))
                            {
                                entry__175431.handleDidPopNext(poppedRoute__175680);
                            }
                            poppedRoute__175680 = null;
                        }
                        entry__175431.handleRemoval(navigator: this, previousPresent: _getRouteBefore(index__175362, (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.willBePresentPredicate)?.route);
                        DartRuntimePrimitives.Assert(() => (FoundationRuntimePorts.EnumIndex(((_RouteEntry__navigator)entry__175431).currentState) >= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.removing)));
                        continue;
                    }
                case _RouteLifecycle__navigator.removing:
                    {
                        if ((!canRemoveOrAdd__175532 && (next__175408 is not null)))
                        {
                            break;
                        }
                        entry__175431.currentState = _RouteLifecycle__navigator.dispose;
                        continue;
                    }
                case _RouteLifecycle__navigator.dispose:
                    {
                        toBeDisposed__175867.Add(this._history.removeAt(index__175362));
                        entry__175431 = next__175408;
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
            index__175362 -= 1L;
            next__175408 = entry__175431;
            entry__175431 = previous__175473;
            previous__175473 = ((index__175362 > 0L) ? this._history[(index__175362 - 1L)] : null);
        }
        _flushObserverNotifications();
        _flushRouteAnnouncement();
        _RouteEntry__navigator? lastEntry__180790 = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
        if (((lastEntry__180790 is not null) && (!object.Equals(this._lastTopmostRoute, lastEntry__180790))))
        {
            foreach (NavigatorObserver observer__180960 in this._effectiveObservers)
            {
                observer__180960.didChangeTop(((_RouteEntry__navigator)lastEntry__180790).route, this._lastTopmostRoute?.route);
            }
        }
        _lastTopmostRoute = lastEntry__180790;
        if (((Navigator)(object)this.widget).reportsRouteUpdateToEngine)
        {
            string? routeName__181219 = ((RouteSettings)((dynamic)lastEntry__180790?.route).settings).ToString();
            if (((routeName__181219 is not null) && (routeName__181219 != this._lastAnnouncedRouteName)))
            {
                DartRuntimePrimitives.Ignore(SystemNavigator.routeInformationUpdated(uri: DartUri.parse(routeName__181219)));
                _lastAnnouncedRouteName = routeName__181219;
            }
        }
        foreach (var entry__181576 in toBeDisposed__175867)
        {
            NavigatorState._disposeRouteEntry(entry__181576, graceful: true);
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
            _NavigatorObservation__navigator observation__182112 = this._observedRouteAdditions.removeLast<_NavigatorObservation__navigator>();
            this._effectiveObservers.forEach((__arg0) => ((global::System.Action<NavigatorObserver>)((_NavigatorObservation__navigator)observation__182112).notify)(__arg0));
        }
        while (System.Linq.Enumerable.Any(this._observedRouteDeletions))
        {
            _NavigatorObservation__navigator observation__182309 = this._observedRouteDeletions.Dequeue();
            this._effectiveObservers.forEach((__arg0) => ((global::System.Action<NavigatorObserver>)((_NavigatorObservation__navigator)observation__182309).notify)(__arg0));
        }
    }

    internal virtual void _flushRouteAnnouncement()
    {
        long index__182471 = (this._history.Count() - 1L);
        while ((index__182471 >= 0L))
        {
            _RouteEntry__navigator entry__182549 = this._history[index__182471];
            if (!((_RouteEntry__navigator)entry__182549).suitableForAnnouncement)
            {
                index__182471 -= 1L;
                continue;
            }
            _RouteEntry__navigator? next__182689 = ((_RouteEntry__navigator?)(object?)_getRouteAfter((index__182471 + 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.suitableForTransitionAnimationPredicate));
            if ((!object.Equals(next__182689?.route, ((_RouteEntry__navigator)entry__182549).lastAnnouncedNextRoute)))
            {
                if (entry__182549.shouldAnnounceChangeToNext(next__182689?.route))
                {
                    ((dynamic)((_RouteEntry__navigator)entry__182549).route).didChangeNext(next__182689?.route);
                }
                entry__182549.lastAnnouncedNextRoute = DartRuntimePrimitives.ConvertValue<_RoutePlaceholder__navigator>(next__182689?.route);
            }
            _RouteEntry__navigator? previous__183065 = ((_RouteEntry__navigator?)(object?)_getRouteBefore((index__182471 - 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.suitableForTransitionAnimationPredicate));
            if ((!object.Equals(previous__183065?.route, ((_RouteEntry__navigator)entry__182549).lastAnnouncedPreviousRoute)))
            {
                ((dynamic)((_RouteEntry__navigator)entry__182549).route).didChangePrevious(previous__183065?.route);
                entry__182549.lastAnnouncedPreviousRoute = DartRuntimePrimitives.ConvertValue<_RoutePlaceholder__navigator>(previous__183065?.route);
            }
            index__182471 -= 1L;
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
        var settings__184688 = new RouteSettings(name: name, arguments: arguments);
        var route__184756 = ((Route<T?>?)(object?)((Navigator)(object)this.widget).onGenerateRoute!(settings__184688))!;
        if (((route__184756 is null) && !allowNull))
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
            route__184756 = ((Route<T?>?)(object?)((Navigator)(object)this.widget).onUnknownRoute!(settings__184688))!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((route__184756 is null))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"Navigator.onUnknownRoute returned null when requested to build route \"{name}\"."), new global::Doroti.Framework.Foundation.ErrorDescription("The onUnknownRoute callback must never return null."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<NavigatorState>("The Navigator was", this, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        }
        DartRuntimePrimitives.Assert(() => ((route__184756 is not null) || allowNull));
        return route__184756;
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
        _RouteEntry__navigator entry__187793 = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateNamed(name: routeName, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.push));
        _pushEntry(entry__187793);
        return ((_RouteEntry__navigator)entry__187793).restorationId!;
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
        _RouteEntry__navigator entry__190164 = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateNamed(name: routeName, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.pushReplace));
        _pushReplacementEntry(entry__190164, result);
        return ((_RouteEntry__navigator)entry__190164).restorationId!;
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
        _RouteEntry__navigator entry__194251 = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateNamed(name: newRouteName, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.push));
        _pushEntryAndRemoveUntil(entry__194251, (global::System.Func<dynamic, bool>)predicate);
        return ((_RouteEntry__navigator)entry__194251).restorationId!;
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
        var result__195427 = false;
        DartRuntimePrimitives.Assert(() =>
            {
                result__195427 = (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb || (Dart_uiLibrary.PluginUtilities.getCallbackHandle(callback) is not null));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return DartRuntimePrimitives.RequireValue(result__195427);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string restorablePush<T>(global::System.Func<BuildContext, object, Route<T>> routeBuilder, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback((global::System.Func<BuildContext, object, Route<T>>)routeBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry__196632 = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateAnonymous(routeBuilder: (global::System.Func<BuildContext, object, Route<T>>)routeBuilder, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.push));
        _pushEntry(entry__196632);
        return ((_RouteEntry__navigator)entry__196632).restorationId!;
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
        DartRuntimePrimitives.Assert(() => !((bool)((dynamic)((_RouteEntry__navigator)entry).route)._installed));
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

    internal virtual void _afterNavigation(dynamic route)
    {
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            DartMap<string, object>? routeJsonable__197642 = default!;
            if ((route is not null))
            {
                routeJsonable__197642 = new DartMap<string, object>();
                string description__197751 = default!;
                if ((route is TransitionRoute<dynamic>))
                {
                    dynamic route__as197776 = (dynamic)route;
                    dynamic transitionRoute__197854 = route__as197776;
                    description__197751 = ((string)((dynamic)transitionRoute__197854).debugLabel);
                }
                else
                {
                    description__197751 = $"{route}";
                }
                routeJsonable__197642["description"] = description__197751;
                RouteSettings settings__198073 = ((RouteSettings)((dynamic)route).settings);
                var settingsJsonable__198114 = new DartMap<string, object> { ["name"] = ((RouteSettings)settings__198073).name };
                if ((((RouteSettings)settings__198073).arguments is not null))
                {
                    settingsJsonable__198114["arguments"] = global::Doroti.Runtime.Dart_convertLibrary.jsonEncode(((RouteSettings)settings__198073).arguments, toEncodable: ((@object) => $"{@object}"));
                }
                routeJsonable__197642["settings"] = settingsJsonable__198114;
            }
            Dart_developerLibrary.postEvent("Flutter.Navigation", new DartMap<string, object> { ["route"] = routeJsonable__197642 });
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
        _RouteEntry__navigator entry__200851 = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateAnonymous(routeBuilder: (global::System.Func<BuildContext, object, Route<T>>)routeBuilder, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.pushReplace));
        _pushReplacementEntry(entry__200851, result);
        return ((_RouteEntry__navigator)entry__200851).restorationId!;
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
        DartRuntimePrimitives.Assert(() => !((bool)((dynamic)((_RouteEntry__navigator)entry).route)._installed));
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
        _RouteEntry__navigator entry__204142 = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateAnonymous(routeBuilder: (global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.push));
        _pushEntryAndRemoveUntil(entry__204142, (global::System.Func<dynamic, bool>)predicate);
        return ((_RouteEntry__navigator)entry__204142).restorationId!;
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
        DartRuntimePrimitives.Assert(() => !((bool)((dynamic)((_RouteEntry__navigator)entry).route)._installed));
        DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(((List<OverlayEntry>)((dynamic)((_RouteEntry__navigator)entry).route).overlayEntries)));
        DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry).currentState, _RouteLifecycle__navigator.push)));
        long index__204784 = (this._history.Count() - 1L);
        this._history.add(entry);
        while (((index__204784 >= 0L) && !predicate(this._history[index__204784].route)))
        {
            if (this._history[index__204784].isPresent)
            {
                this._history[index__204784].complete(((Navigator)(object)null), isReplaced: false, imperativeRemoval: true);
            }
            index__204784 -= 1L;
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
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)oldRoute)._isInstalledIn(this)));
        _replaceEntry(new _RouteEntry__navigator(newRoute, pageBased: false, initialState: _RouteLifecycle__navigator.replace), oldRoute);
    }

    public virtual string restorableReplace<T>(dynamic oldRoute, global::System.Func<BuildContext, object, Route<T>> newRouteBuilder, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)oldRoute)._isInstalledIn(this)));
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback((global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry__206788 = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateAnonymous(routeBuilder: (global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.replace));
        _replaceEntry(entry__206788, oldRoute);
        return ((_RouteEntry__navigator)entry__206788).restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _replaceEntry(_RouteEntry__navigator entry, dynamic oldRoute)
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
        DartRuntimePrimitives.Assert(() => !((bool)((dynamic)((_RouteEntry__navigator)entry).route)._installed));
        long index__207425 = this._history.indexWhere(_RouteEntry__navigator.isRoutePredicate(oldRoute));
        DartRuntimePrimitives.Assert(() => (index__207425 >= 0L), () => (object?)"This Navigator does not contain the specified oldRoute.");
        DartRuntimePrimitives.Assert(() => this._history[index__207425].isPresent, () => (object?)"The specified oldRoute has already been removed from the Navigator.");
        bool wasCurrent__207721 = ((bool)((dynamic)oldRoute).isCurrent);
        this._history.insert((index__207425 + 1L), entry);
        this._history[index__207425].complete(((Navigator)(object)null), isReplaced: true, imperativeRemoval: true);
        _flushHistoryUpdates();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (wasCurrent__207721)
        {
            _afterNavigation(((_RouteEntry__navigator)entry).route);
        }
    }

    public virtual void replaceRouteBelow<T>(dynamic anchorRoute, Route<T> newRoute)
    {
        DartRuntimePrimitives.Assert(() => !((Route<T>)newRoute)._installed);
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)anchorRoute)._isInstalledIn(this)));
        _replaceEntryBelow(new _RouteEntry__navigator(newRoute, pageBased: false, initialState: _RouteLifecycle__navigator.replace), anchorRoute);
    }

    public virtual string restorableReplaceRouteBelow<T>(dynamic anchorRoute, global::System.Func<BuildContext, object, Route<T>> newRouteBuilder, object? arguments = null)
    {
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)anchorRoute)._isInstalledIn(this)));
        DartRuntimePrimitives.Assert(() => _debugIsStaticCallback((global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder), () => (object?)"The provided routeBuilder must be a static function.");
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(arguments), () => (object?)"The arguments object must be serializable via the StandardMessageCodec.");
        _RouteEntry__navigator entry__209822 = ((_RouteEntry__navigator)(object?)_RestorationInformation__navigator.CreateAnonymous(routeBuilder: (global::System.Func<BuildContext, object, Route<T>>)newRouteBuilder, arguments: arguments, restorationScopeId: this._nextPagelessRestorationScopeId).toRouteEntry(this, initialState: _RouteLifecycle__navigator.replace));
        _replaceEntryBelow(entry__209822, anchorRoute);
        return ((_RouteEntry__navigator)entry__209822).restorationId!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _replaceEntryBelow(_RouteEntry__navigator entry, dynamic anchorRoute)
    {
        DartRuntimePrimitives.Assert(() => !this._debugLocked);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        long anchorIndex__210324 = this._history.indexWhere(_RouteEntry__navigator.isRoutePredicate(anchorRoute));
        DartRuntimePrimitives.Assert(() => (anchorIndex__210324 >= 0L), () => (object?)"This Navigator does not contain the specified anchorRoute.");
        DartRuntimePrimitives.Assert(() => this._history[anchorIndex__210324].isPresent, () => (object?)"The specified anchorRoute has already been removed from the Navigator.");
        long index__210640 = (anchorIndex__210324 - 1L);
        while ((index__210640 >= 0L))
        {
            if (this._history[index__210640].isPresent)
            {
                break;
            }
            index__210640 -= 1L;
        }
        DartRuntimePrimitives.Assert(() => (index__210640 >= 0L), () => (object?)"There are no routes below the specified anchorRoute.");
        this._history.insert((index__210640 + 1L), entry);
        this._history[index__210640].complete(((Navigator)(object)null), isReplaced: true, imperativeRemoval: true);
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
        IEnumerator<_RouteEntry__navigator> iterator__211354 = this._history.where(_RouteEntry__navigator.isPresentPredicate).GetEnumerator();
        if (!iterator__211354.MoveNext())
        {
            return false;
        }
        if (((bool)((dynamic)iterator__211354.Current.route).willHandlePopInternally))
        {
            return true;
        }
        if (!iterator__211354.MoveNext())
        {
            return false;
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<bool> maybePop<T>(T? result = default)
    {
        _RouteEntry__navigator? lastEntry__212544 = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
        if ((lastEntry__212544 is null))
        {
            return false;
        }
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)((_RouteEntry__navigator)lastEntry__212544).route)._isInstalledIn(this)));
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)((_RouteEntry__navigator)lastEntry__212544).route)._debugCheckCanConsumeResult(result, methodName: "maybePop")));
        if ((object.Equals(await ((Future<RoutePopDisposition>)((dynamic)((_RouteEntry__navigator)lastEntry__212544).route).willPop()), RoutePopDisposition.doNotPop)))
        {
            return true;
        }
        if (!this.mounted)
        {
            return true;
        }
        _RouteEntry__navigator? newLastEntry__213175 = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
        if ((!object.Equals(lastEntry__212544, newLastEntry__213175)))
        {
            return true;
        }
        switch (((RoutePopDisposition)((dynamic)((_RouteEntry__navigator)lastEntry__212544).route).popDisposition))
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
                    ((dynamic)((_RouteEntry__navigator)lastEntry__212544).route).onPopInvokedWithResult(false, result);
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
                _RouteEntry__navigator? entry__214362 = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
                return (((bool?)((dynamic)entry__214362?.route)._debugCheckCanConsumeResult(result, methodName: "pop")) ?? true);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _RouteEntry__navigator entry__214624 = this._history.lastWhere(_RouteEntry__navigator.isPresentPredicate);
        if ((((_RouteEntry__navigator)entry__214624).pageBased && (((Navigator)(object)this.widget).onPopPage is not null)))
        {
            if (((Navigator)(object)this.widget).onPopPage!(((_RouteEntry__navigator)entry__214624).route, result))
            {
                if ((FoundationRuntimePorts.EnumIndex(((_RouteEntry__navigator)entry__214624).currentState) <= FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.idle)))
                {
                    DartRuntimePrimitives.Assert(() => ((Completer<object>)((dynamic)((_RouteEntry__navigator)entry__214624).route)._popCompleter).isCompleted);
                    entry__214624.currentState = _RouteLifecycle__navigator.pop;
                }
                ((dynamic)((_RouteEntry__navigator)entry__214624).route).onPopInvokedWithResult(true, result);
            }
        }
        else
        {
            entry__214624.pop<T>(result, imperativeRemoval: true);
            DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry__214624).currentState, _RouteLifecycle__navigator.pop)));
        }
        if ((object.Equals(((_RouteEntry__navigator)entry__214624).currentState, _RouteLifecycle__navigator.pop)))
        {
            _flushHistoryUpdates(rearrangeOverlay: false);
        }
        DartRuntimePrimitives.Assert(() => ((object.Equals(((_RouteEntry__navigator)entry__214624).currentState, _RouteLifecycle__navigator.idle)) || ((Completer<object>)((dynamic)((_RouteEntry__navigator)entry__214624).route)._popCompleter).isCompleted));
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _afterNavigation(((_RouteEntry__navigator)entry__214624).route);
    }

    public virtual void popUntil(global::System.Func<dynamic, bool> predicate)
    {
        _RouteEntry__navigator? candidate__215967 = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
        while ((candidate__215967 is not null))
        {
            if (predicate(((_RouteEntry__navigator)candidate__215967).route))
            {
                return;
            }
            pop<object>();
            candidate__215967 = _lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate);
        }
    }

    public virtual void popUntilWithResult<T>(global::System.Func<dynamic, bool> predicate, T? result)
    {
        _RouteEntry__navigator? candidate__216532 = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate));
        while ((candidate__216532 is not null))
        {
            if (predicate(((_RouteEntry__navigator)candidate__216532).route))
            {
                return;
            }
            _RouteEntry__navigator? next__216782 = ((_RouteEntry__navigator?)(object?)_lastRouteEntryWhereOrNull(((global::System.Func<_RouteEntry__navigator, bool>)((e) => (_RouteEntry__navigator.isPresentPredicate(e) && (!object.Equals(e, candidate__216532)))))));
            if ((((next__216782 is not null) && !((bool)((dynamic)((_RouteEntry__navigator)next__216782).route).willHandlePopInternally)) && predicate(((_RouteEntry__navigator)next__216782).route)))
            {
                pop<T>(result);
            }
            else
            {
                pop<object>();
            }
            candidate__216532 = _lastRouteEntryWhereOrNull((global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.isPresentPredicate);
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
        bool wasCurrent__217528 = ((Route<T>)route).isCurrent;
        _RouteEntry__navigator entry__217580 = this._history.firstWhere(_RouteEntry__navigator.isRoutePredicate(route));
        entry__217580.complete(result, isReplaced: false, imperativeRemoval: true);
        _flushHistoryUpdates(rearrangeOverlay: false);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (wasCurrent__217528)
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
        long anchorIndex__218438 = this._history.indexWhere(_RouteEntry__navigator.isRoutePredicate(anchorRoute));
        DartRuntimePrimitives.Assert(() => (anchorIndex__218438 >= 0L), () => (object?)"This Navigator does not contain the specified anchorRoute.");
        DartRuntimePrimitives.Assert(() => this._history[anchorIndex__218438].isPresent, () => (object?)"The specified anchorRoute has already been removed from the Navigator.");
        long index__218754 = (anchorIndex__218438 - 1L);
        while ((index__218754 >= 0L))
        {
            if (this._history[index__218754].isPresent)
            {
                break;
            }
            index__218754 -= 1L;
        }
        DartRuntimePrimitives.Assert(() => (index__218754 >= 0L), () => (object?)"There are no routes below the specified anchorRoute.");
        this._history[index__218754].complete(result, isReplaced: false, imperativeRemoval: true);
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
        bool? wasDebugLocked__220042 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                wasDebugLocked__220042 = this._debugLocked;
                _debugLocked = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => (this._history.where((entry) => _RouteEntry__navigator.isRoutePredicate(route)(DartRuntimePrimitives.ConvertValue<_RouteEntry__navigator>(entry))).Count() == 1L));
        long index__220258 = this._history.indexWhere(_RouteEntry__navigator.isRoutePredicate(route));
        _RouteEntry__navigator entry__220346 = this._history[index__220258];
        if ((((_RouteEntry__navigator)entry__220346).pageBased && (FoundationRuntimePorts.EnumIndex(((_RouteEntry__navigator)entry__220346).currentState) < FoundationRuntimePorts.EnumIndex(_RouteLifecycle__navigator.pop))))
        {
            this._observedRouteDeletions.Enqueue(new _NavigatorPopObservation__navigator(route, _getRouteBefore((index__220258 - 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.willBePresentPredicate)?.route));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(((_RouteEntry__navigator)entry__220346).currentState, _RouteLifecycle__navigator.popping)));
        }
        entry__220346.finalize();
        if (!this._flushingHistory)
        {
            _flushHistoryUpdates(rearrangeOverlay: false);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugLocked = DartRuntimePrimitives.RequireValue(wasDebugLocked__220042);
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
            long routeIndex__222420 = _getIndexBefore((this._history.Count() - 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.willBePresentPredicate);
            dynamic route__222559 = this._history[routeIndex__222420].route;
            dynamic previousRoute__222617 = default!;
            if ((!((bool)((dynamic)route__222559).willHandlePopInternally) && (routeIndex__222420 > 0L)))
            {
                previousRoute__222617 = _getRouteBefore((routeIndex__222420 - 1L), (global::System.Func<_RouteEntry__navigator, bool>)_RouteEntry__navigator.willBePresentPredicate)!.route;
            }
            foreach (NavigatorObserver observer__222837 in this._effectiveObservers)
            {
                observer__222837.didStartUserGesture(route__222559, previousRoute__222617);
            }
        }
    }

    public virtual void didStopUserGesture()
    {
        DartRuntimePrimitives.Assert(() => (this._userGesturesInProgress > 0L));
        _userGesturesInProgress -= 1L;
        if ((this._userGesturesInProgress == 0L))
        {
            foreach (NavigatorObserver observer__223315 in this._effectiveObservers)
            {
                observer__223315.didStopUserGesture();
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
            global::Doroti.Framework.Rendering.RenderAbsorbPointer? absorber__224092 = ((global::Doroti.Framework.Rendering.RenderAbsorbPointer?)(object?)((GlobalKey<OverlayState>)this._overlayKey).currentContext?.findAncestorRenderObjectOfType<global::Doroti.Framework.Rendering.RenderAbsorbPointer>());
            setState(((global::System.Action)(() => {
absorber__224092?.absorbing = true;
})));
        }
        this._activePointers.ToList().forEach((__arg0) => ((global::System.Action<long>)WidgetsBinding.instance.cancelPointer)(__arg0));
    }

    internal virtual _RouteEntry__navigator? _firstRouteEntryWhereOrNull(global::System.Func<_RouteEntry__navigator, bool> test)
    {
        foreach (_RouteEntry__navigator element__224645 in this._history)
        {
            if (test(element__224645))
            {
                return element__224645;
            }
        }
        return ((_RouteEntry__navigator)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _RouteEntry__navigator? _lastRouteEntryWhereOrNull(global::System.Func<_RouteEntry__navigator, bool> test)
    {
        _RouteEntry__navigator? result__224919 = default!;
        foreach (_RouteEntry__navigator element__224954 in this._history)
        {
            if (test(element__224954))
            {
                result__224919 = element__224954;
            }
        }
        return result__224919;
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
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<_WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
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
        global::Doroti.Framework.Services.RestorationBucket? oldBucket__39230 = this._bucket;
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
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent__40517 = ((global::Doroti.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent__40517, this._currentParent)) && ((potentialNewParent__40517?.isReplacing ?? false)));
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
            bool didReplace__42801 = _setNewBucketIfNecessary(newBucket: ((global::Doroti.Framework.Services.RestorationBucket)(object)null), restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace__42801;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucket__43086 = ((global::Doroti.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
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

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket__43946 = this._bucket;
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
        var casted__227761 = ((List<object?>?)(object?)data)!;
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(casted__227761));
        _RouteRestorationType__navigator type__227856 = System.Enum.GetValues<_RouteRestorationType__navigator>().ToList()[(int)(((long)casted__227761[(int)(0L)]!))];
        switch (type__227856)
        {
            case _RouteRestorationType__navigator.named:
                {
                    return ((_RestorationInformation__navigator)(object?)_NamedRestorationInformation__navigator.CreateFromSerializableData(casted__227761.Skip(checked((int)1L)).ToList()));
                }
            case _RouteRestorationType__navigator.anonymous:
                {
                    return ((_RestorationInformation__navigator)(object?)_AnonymousRestorationInformation__navigator.CreateFromSerializableData(casted__227761.Skip(checked((int)1L)).ToList()));
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

    public abstract dynamic createRoute(NavigatorState navigator);
    public virtual _RouteEntry__navigator toRouteEntry(NavigatorState navigator, _RouteLifecycle__navigator initialState = _RouteLifecycle__navigator.add)
    {
        dynamic route__228769 = createRoute(navigator);
        return new _RouteEntry__navigator(route__228769, pageBased: false, initialState: initialState, restorationInformation: this);
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
{            var __cascade = base.computeSerializableData();
            __cascade.AddRange(new List<object> { this.restorationScopeId, this.name }.Cast<object>());
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override dynamic createRoute(NavigatorState navigator)
    {
        dynamic route__229796 = navigator._routeNamed<object>(this.name, arguments: this.arguments)!;
        return route__229796;
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
        global::Doroti.Ui.CallbackHandle? handle__230857 = ((global::Doroti.Ui.CallbackHandle?)(object?)Dart_uiLibrary.PluginUtilities.getCallbackHandle(this.routeBuilder));
        DartRuntimePrimitives.Assert(() => (handle__230857 is not null));
        return ((Func<List<object>>)(() =>
{            var __cascade = base.computeSerializableData();
            __cascade.AddRange(new List<object> { this.restorationScopeId, handle__230857!.toRawHandle() }.Cast<object>());
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override dynamic createRoute(NavigatorState navigator)
    {
        dynamic result__231295 = this.routeBuilder(navigator.context, this.arguments);
        return result__231295;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _HistoryProperty__navigator : RestorableProperty<DartMap<string?, List<object>>?>
{
    internal virtual DartMap<string?, List<object>>? _pageToPagelessRoutes { get; set; } = default;

    public virtual void update(_History__navigator history)
    {
        DartRuntimePrimitives.Assert(() => this.isRegistered);
        var wasUninitialized__231663 = (this._pageToPagelessRoutes is null);
        var needsSerialization__231721 = wasUninitialized__231663;
        _pageToPagelessRoutes ??= new DartMap<string, List<object>>();
        _RouteEntry__navigator? currentPage__231833 = default!;
        var newRoutesForCurrentPage__231854 = new List<object>();
        List<object> oldRoutesForCurrentPage__231909 = (this._pageToPagelessRoutes!.GetValueOrDefault(null) ?? new List<object>()).ToList();
        var restorationEnabled__231993 = true;
        var newMap__232031 = new DartMap<string?, List<object>>();
        HashSet<string?> removedPages__232090 = this._pageToPagelessRoutes!.Keys.toSet();
        foreach (var entry__232158 in history)
        {
            if (!((_RouteEntry__navigator)entry__232158).isPresentForRestoration)
            {
                entry__232158.restorationEnabled = false;
                continue;
            }
            DartRuntimePrimitives.Assert(() => ((_RouteEntry__navigator)entry__232158).isPresentForRestoration);
            if (((_RouteEntry__navigator)entry__232158).pageBased)
            {
                needsSerialization__231721 = (needsSerialization__231721 || (checked((long)(newRoutesForCurrentPage__231854.Count)) != checked((long)(oldRoutesForCurrentPage__231909.Count))));
                _finalizeEntry(newRoutesForCurrentPage__231854, currentPage__231833, newMap__232031, removedPages__232090);
                currentPage__231833 = entry__232158;
                restorationEnabled__231993 = (((_RouteEntry__navigator)entry__232158).restorationId is not null);
                entry__232158.restorationEnabled = restorationEnabled__231993;
                if (restorationEnabled__231993)
                {
                    DartRuntimePrimitives.Assert(() => (((_RouteEntry__navigator)entry__232158).restorationId is not null));
                    newRoutesForCurrentPage__231854 = new List<object>();
                    oldRoutesForCurrentPage__231909 = (this._pageToPagelessRoutes!.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((_RouteEntry__navigator)entry__232158).restorationId)) ?? new List<object>());
                }
                else
                {
                    newRoutesForCurrentPage__231854 = new List<object>();
                    oldRoutesForCurrentPage__231909 = new List<object>();
                }
                continue;
            }
            DartRuntimePrimitives.Assert(() => !((_RouteEntry__navigator)entry__232158).pageBased);
            restorationEnabled__231993 = (restorationEnabled__231993 && ((((_RouteEntry__navigator)entry__232158).restorationInformation?.isRestorable ?? false)));
            entry__232158.restorationEnabled = restorationEnabled__231993;
            if (restorationEnabled__231993)
            {
                DartRuntimePrimitives.Assert(() => (((_RouteEntry__navigator)entry__232158).restorationId is not null));
                DartRuntimePrimitives.Assert(() => ((currentPage__231833 is null) || (((_RouteEntry__navigator)currentPage__231833).restorationId is not null)));
                DartRuntimePrimitives.Assert(() => (((_RouteEntry__navigator)entry__232158).restorationInformation is not null));
                object serializedData__233537 = ((_RouteEntry__navigator)entry__232158).restorationInformation!.getSerializableData();
                needsSerialization__231721 = ((needsSerialization__231721 || (checked((long)(oldRoutesForCurrentPage__231909.Count)) <= checked((long)(newRoutesForCurrentPage__231854.Count)))) || (!object.Equals(oldRoutesForCurrentPage__231909[(int)(checked((long)(newRoutesForCurrentPage__231854.Count)))], serializedData__233537)));
                newRoutesForCurrentPage__231854.Add(serializedData__233537);
            }
        }
        needsSerialization__231721 = (needsSerialization__231721 || (checked((long)(newRoutesForCurrentPage__231854.Count)) != checked((long)(oldRoutesForCurrentPage__231909.Count))));
        _finalizeEntry(newRoutesForCurrentPage__231854, currentPage__231833, newMap__232031, removedPages__232090);
        needsSerialization__231721 = (needsSerialization__231721 || System.Linq.Enumerable.Any(removedPages__232090));
        DartRuntimePrimitives.Assert(() => (wasUninitialized__231663 || (_debugMapsEqual(this._pageToPagelessRoutes!, newMap__232031) != needsSerialization__231721)));
        if (needsSerialization__231721)
        {
            _pageToPagelessRoutes = newMap__232031.cast<string?, List<object>>();
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
            string? restorationId__234765 = page?.restorationId;
            pageToRoutes[DartRuntimePrimitives.RequireReference(restorationId__234765)] = routes;
            pagesToRemove.Remove(restorationId__234765);
        }
    }

    internal virtual bool _debugMapsEqual(DartMap<string?, List<object>> a, DartMap<string?, List<object>> b)
    {
        if (!global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals(a.Keys.toSet(), b.Keys.toSet()))
        {
            return false;
        }
        foreach (string? key__235088 in a.Keys)
        {
            if (!global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(a.GetValueOrDefault(DartRuntimePrimitives.RequireReference(key__235088)).Cast<DartMap<string?, List<object>>?>().ToList(), b.GetValueOrDefault(DartRuntimePrimitives.RequireReference(key__235088)).Cast<DartMap<string?, List<object>>?>().ToList()))
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
        var result__235612 = new List<_RouteEntry__navigator>();
        if (((this._pageToPagelessRoutes is null) || (((page is not null) && (((_RouteEntry__navigator)page).restorationId is null)))))
        {
            return result__235612;
        }
        List<object>? serializedData__235778 = this._pageToPagelessRoutes!.GetValueOrDefault(DartRuntimePrimitives.RequireReference(page?.restorationId)).ToList();
        if ((serializedData__235778 is null))
        {
            return result__235612;
        }
        foreach (object data__235923 in serializedData__235778)
        {
            result__235612.Add(_RestorationInformation__navigator.CreateFromSerializableData(data__235923).toRouteEntry(navigator));
        }
        return result__235612;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DartMap<string?, List<object>>? createDefaultValue()
    {
        return ((DartMap<string?, List<object>>)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DartMap<string?, List<object>>? fromPrimitives(object? data)
    {
        var casted__236279 = DartRuntimePrimitives.ConvertMap<object, object>((System.Collections.IDictionary)data!);
        return casted__236279.map<object, object, string?, List<object>>(((key, value) => new MapEntry<string?, List<object>>(((string?)(object?)key)!, new List<object>(DartRuntimePrimitives.ConvertEnumerable<object>(((List<object>?)(object?)value)!)))));
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
        string routeId__241157 = this.onPresent(this._navigator, arguments);
        _hookOntoRouteFuture(routeId__241157);
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
        this._route?.restorationScopeId.removeListener(() => this.notifyListeners());
        _disposed = true;
    }

    public override bool enabled => DartRuntimePrimitives.ConvertValue<bool>((this.route?.restorationScopeId.value is not null));
    internal virtual NavigatorState _navigator
    {
        get
        {
            NavigatorState navigator__242350 = this.navigatorFinder(this.state.context);
            return navigator__242350;
            return default!;
        }
    }
    internal virtual void _hookOntoRouteFuture(string id)
    {
        _route = this._navigator._getRouteById<T>(id);
        DartRuntimePrimitives.Assert(() => (this._route is not null));
        this.route!.restorationScopeId.addListener(() => this.notifyListeners());
        DartRuntimePrimitives.Ignore(this.route!.popped.then((global::System.Action<object>)((result) => {
if (this._disposed)
{
    return;
}
this._route?.restorationScopeId.removeListener(() => this.notifyListeners());
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
