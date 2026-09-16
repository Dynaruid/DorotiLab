// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/tab_view.dart

using Doroti.Runtime;

namespace Doroti.Framework.Cupertino;

public class CupertinoTabView : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? builder { get; private set; }
    public virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.NavigatorState>? navigatorKey { get; private set; }
    public virtual string? defaultTitle { get; private set; }
    public virtual DartMap<string, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>>? routes { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.RouteSettings, dynamic?>? onGenerateRoute { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.RouteSettings, dynamic?>? onUnknownRoute { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.NavigatorObserver> navigatorObservers { get; private set; } = default!;
    public virtual string? restorationScopeId { get; private set; }

    public CupertinoTabView(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? builder = null, global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.NavigatorState>? navigatorKey = null, string? defaultTitle = null, DartMap<string, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>>? routes = null, global::System.Func<global::Doroti.Framework.Widgets.RouteSettings, dynamic?>? onGenerateRoute = null, global::System.Func<global::Doroti.Framework.Widgets.RouteSettings, dynamic?>? onUnknownRoute = null, List<global::Doroti.Framework.Widgets.NavigatorObserver> navigatorObservers = default!, string? restorationScopeId = null) : base(key: key)
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
        if ((!Equals(widget.navigatorKey, oldWidget.navigatorKey)) || (!Equals(widget.navigatorObservers, oldWidget.navigatorObservers)))
        {
            _updateObservers();
        }
    }

    public override void dispose()
    {
        _heroController.dispose();
        base.dispose();
    }

    internal virtual void _updateObservers()
    {
        _navigatorObservers = ((Func<List<global::Doroti.Framework.Widgets.NavigatorObserver>>)(() =>
{
    var __cascade = new List<global::Doroti.Framework.Widgets.NavigatorObserver>(DartRuntimePrimitives.ConvertEnumerable<global::Doroti.Framework.Widgets.NavigatorObserver>(widget.navigatorObservers));
    __cascade.Add(_heroController);
    return __cascade;
}))();
    }

    internal virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.NavigatorState> _navigatorKey
    {
        get
        {
            if (widget.navigatorKey is not null)
            {
                return widget.navigatorKey!;
            }
            _ownedNavigatorKey ??= GlobalKey<NavigatorState>.Create();
            return _ownedNavigatorKey!;
        }
    }
    internal virtual bool _isActive => TickerMode.of(context);
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget childLocal = new global::Doroti.Framework.Widgets.Navigator(key: _navigatorKey, onGenerateRoute: _onGenerateRoute, onUnknownRoute: _onUnknownRoute, observers: _navigatorObservers, restorationScopeId: widget.restorationScopeId);
        return new global::Doroti.Framework.Widgets.NavigatorPopHandler<object>(enabled: _isActive, onPop: () =>
        {
            if (!_isActive)
            {
                return;
            }
            DartRuntimePrimitives.Ignore(_navigatorKey.currentState!.maybePop<object>());
        }, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual dynamic? _onGenerateRoute(global::Doroti.Framework.Widgets.RouteSettings settings)
    {
        string? nameLocal = settings.name;
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? routeBuilder = default!;
        string? titleLocal = default!;
        if ((nameLocal == Navigator.defaultRouteName) && (widget.builder is not null))
        {
            routeBuilder = widget.builder;
            titleLocal = widget.defaultTitle;
        }
        else
        {
            routeBuilder = (widget.routes?.GetValueOrDefault(nameLocal));
        }
        if (routeBuilder is not null)
        {
            return new CupertinoPageRoute<object>(builder: routeBuilder, title: titleLocal, settings: settings);
        }
        return widget.onGenerateRoute?.Invoke(settings);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual dynamic _onUnknownRoute(global::Doroti.Framework.Widgets.RouteSettings settings)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (widget.onUnknownRoute is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create($"Could not find a generator for route {settings} in the {GetType()}.\n" + "Generators for routes are searched for in the following order:\n" + " 1. For the \"/\" route, the \"builder\" property, if non-null, is used.\n" + " 2. Otherwise, the \"routes\" table is used, if it has an entry for " + "the route.\n" + " 3. Otherwise, onGenerateRoute is called. It should return a " + "non-null value for any valid route not handled by \"builder\" and \"routes\".\n" + " 4. Finally if all else fails onUnknownRoute is called.\n" + "Unfortunately, onUnknownRoute was not set."));
                }
                return true;
            });
        dynamic result = widget.onUnknownRoute!(settings) ?? throw new InvalidOperationException("The onUnknownRoute callback must return a route.");
        DartRuntimePrimitives.Assert(() =>
            {
                if (result is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("The onUnknownRoute callback returned null.\n" + $"When the {GetType()} requested the route {settings} from its " + "onUnknownRoute callback, the callback returned null. Such callbacks " + "must never return null."));
                }
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
