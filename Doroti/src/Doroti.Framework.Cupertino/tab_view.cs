// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/tab_view.dart
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

public class CupertinoTabView : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? builder { get; private set; }
    public virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.NavigatorState>? navigatorKey { get; private set; }
    public virtual string? defaultTitle { get; private set; }
    public virtual DartMap<string, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>>? routes { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.RouteSettings, dynamic>? onGenerateRoute { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.RouteSettings, dynamic>? onUnknownRoute { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.NavigatorObserver> navigatorObservers { get; private set; } = default!;
    public virtual string? restorationScopeId { get; private set; }

    public CupertinoTabView(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? builder = null, global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.NavigatorState>? navigatorKey = null, string? defaultTitle = null, DartMap<string, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>>? routes = null, global::System.Func<global::Doroti.Framework.Widgets.RouteSettings, dynamic>? onGenerateRoute = null, global::System.Func<global::Doroti.Framework.Widgets.RouteSettings, dynamic>? onUnknownRoute = null, List<global::Doroti.Framework.Widgets.NavigatorObserver> navigatorObservers = default!, string? restorationScopeId = null) : base(key: key)
    {
        List<global::Doroti.Framework.Widgets.NavigatorObserver> __navigatorObservers = navigatorObservers ?? new List<global::Doroti.Framework.Widgets.NavigatorObserver>();
        this.builder = builder;
        this.navigatorKey = navigatorKey;
        this.defaultTitle = defaultTitle;
        this.routes = routes;
        this.onGenerateRoute = onGenerateRoute;
        this.onUnknownRoute = onUnknownRoute;
        this.navigatorObservers = __navigatorObservers;
        this.restorationScopeId = restorationScopeId;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTabViewState__tab_view());
}

internal class _CupertinoTabViewState__tab_view : global::Doroti.Framework.Widgets.State<CupertinoTabView>
{
    internal virtual global::Doroti.Framework.Widgets.HeroController _heroController { get; set; } = default!;
    internal virtual List<global::Doroti.Framework.Widgets.NavigatorObserver> _navigatorObservers { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.NavigatorState>? _ownedNavigatorKey { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _heroController = CupertinoApp.createCupertinoHeroController();
        _updateObservers();
    }

    public override void didUpdateWidget(CupertinoTabView oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((!object.Equals(((CupertinoTabView)this.widget).navigatorKey, ((CupertinoTabView)oldWidget).navigatorKey)) || (!object.Equals(((CupertinoTabView)this.widget).navigatorObservers, ((CupertinoTabView)oldWidget).navigatorObservers))))
        {
            _updateObservers();
        }
    }

    public override void dispose()
    {
        this._heroController.dispose();
        base.dispose();
    }

    internal virtual void _updateObservers()
    {
        _navigatorObservers = ((Func<List<global::Doroti.Framework.Widgets.NavigatorObserver>>)(() =>
{
    var __cascade = new List<global::Doroti.Framework.Widgets.NavigatorObserver>(DartRuntimePrimitives.ConvertEnumerable<global::Doroti.Framework.Widgets.NavigatorObserver>(((CupertinoTabView)this.widget).navigatorObservers));
    __cascade.Add(this._heroController);
    return __cascade;
}))();
    }

    internal virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.NavigatorState> _navigatorKey
    {
        get
        {
            if ((((CupertinoTabView)this.widget).navigatorKey is not null))
            {
                return ((CupertinoTabView)this.widget).navigatorKey!;
            }
            _ownedNavigatorKey ??= global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.NavigatorState>.Create();
            return this._ownedNavigatorKey!;
            return default!;
        }
    }
    internal virtual bool _isActive => TickerMode.of(this.context);
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget child__6797 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Navigator(key: this._navigatorKey, onGenerateRoute: (global::System.Func<global::Doroti.Framework.Widgets.RouteSettings, dynamic>)this._onGenerateRoute, onUnknownRoute: (global::System.Func<global::Doroti.Framework.Widgets.RouteSettings, dynamic>)this._onUnknownRoute, observers: this._navigatorObservers, restorationScopeId: ((CupertinoTabView)this.widget).restorationScopeId));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NavigatorPopHandler<object>(enabled: this._isActive, onPop: ((global::System.Action)(() =>
        {
            if (!this._isActive)
            {
                return;
            }
            DartRuntimePrimitives.Ignore(((global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.NavigatorState>)this._navigatorKey).currentState!.maybePop<object>());
        })), child: child__6797));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual dynamic _onGenerateRoute(global::Doroti.Framework.Widgets.RouteSettings settings)
    {
        string? name__7391 = ((global::Doroti.Framework.Widgets.RouteSettings)settings).name;
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? routeBuilder__7438 = default!;
        string? title__7464 = default!;
        if (((name__7391 == global::Doroti.Framework.Widgets.Navigator.defaultRouteName) && (((CupertinoTabView)this.widget).builder is not null)))
        {
            routeBuilder__7438 = (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((CupertinoTabView)this.widget).builder;
            title__7464 = ((CupertinoTabView)this.widget).defaultTitle;
        }
        else
        {
            routeBuilder__7438 = (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((CupertinoTabView)this.widget).routes.GetValueOrDefault(name__7391);
        }
        if ((routeBuilder__7438 is not null))
        {
            return new CupertinoPageRoute<object>(builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)routeBuilder__7438, title: title__7464, settings: settings);
        }
        return ((CupertinoTabView)this.widget).onGenerateRoute?.Invoke(settings);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual dynamic _onUnknownRoute(global::Doroti.Framework.Widgets.RouteSettings settings)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((CupertinoTabView)this.widget).onUnknownRoute is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"Could not find a generator for route {settings} in the {this.GetType()}.\n" + "Generators for routes are searched for in the following order:\n" + " 1. For the \"/\" route, the \"builder\" property, if non-null, is used.\n" + " 2. Otherwise, the \"routes\" table is used, if it has an entry for " + "the route.\n" + " 3. Otherwise, onGenerateRoute is called. It should return a " + "non-null value for any valid route not handled by \"builder\" and \"routes\".\n" + " 4. Finally if all else fails onUnknownRoute is called.\n" + "Unfortunately, onUnknownRoute was not set."));
                }
                return true;
            });
        dynamic result__8725 = ((CupertinoTabView)this.widget).onUnknownRoute!(settings);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__8725 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("The onUnknownRoute callback returned null.\n" + $"When the {this.GetType()} requested the route {settings} from its " + "onUnknownRoute callback, the callback returned null. Such callbacks " + "must never return null."));
                }
                return true;
            });
        return result__8725;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
