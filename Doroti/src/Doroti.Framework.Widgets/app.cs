// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/app.dart
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

public delegate Locale? LocaleListResolutionCallback(List<Locale>? locales, IEnumerable<Locale> supportedLocales);

public delegate Locale? LocaleResolutionCallback(Locale? locale, IEnumerable<Locale> supportedLocales);

public static partial class AppLibrary
{
    public static Locale basicLocaleListResolution(List<Locale>? preferredLocales, IEnumerable<Locale> supportedLocales)
    {
        if (((preferredLocales is null) || !System.Linq.Enumerable.Any(preferredLocales)))
        {
            return supportedLocales.First();
        }
        DartMap<string, global::Doroti.Ui.Locale> allSupportedLocales__6694 = new DartMap<string, global::Doroti.Ui.Locale>().cast<string, global::Doroti.Ui.Locale>();
        DartMap<string, global::Doroti.Ui.Locale> languageAndCountryLocales__6771 = new DartMap<string, global::Doroti.Ui.Locale>().cast<string, global::Doroti.Ui.Locale>();
        DartMap<string, global::Doroti.Ui.Locale> languageAndScriptLocales__6854 = new DartMap<string, global::Doroti.Ui.Locale>().cast<string, global::Doroti.Ui.Locale>();
        DartMap<string, global::Doroti.Ui.Locale> languageLocales__6936 = new DartMap<string, global::Doroti.Ui.Locale>().cast<string, global::Doroti.Ui.Locale>();
        DartMap<string?, global::Doroti.Ui.Locale> countryLocales__7010 = new DartMap<string?, global::Doroti.Ui.Locale>().cast<string?, global::Doroti.Ui.Locale>();
        foreach (var locale__7068 in supportedLocales)
        {
            allSupportedLocales__6694.putIfAbsent($"{locale__7068.languageCode}_{locale__7068.scriptCode}_{locale__7068.countryCode}", () => locale__7068);
            languageAndScriptLocales__6854.putIfAbsent($"{locale__7068.languageCode}_{locale__7068.scriptCode}", () => locale__7068);
            languageAndCountryLocales__6771.putIfAbsent($"{locale__7068.languageCode}_{locale__7068.countryCode}", () => locale__7068);
            languageLocales__6936.putIfAbsent(locale__7068.languageCode, () => locale__7068);
            countryLocales__7010.putIfAbsent(locale__7068.countryCode, () => locale__7068);
        }
        global::Doroti.Ui.Locale? matchesLanguageCode__7850 = default!;
        global::Doroti.Ui.Locale? matchesCountryCode__7881 = default!;
        for (var localeIndex__7952 = 0L; (localeIndex__7952 < checked((long)(preferredLocales.Count))); localeIndex__7952 += 1L)
        {
            global::Doroti.Ui.Locale userLocale__8045 = preferredLocales[(int)(localeIndex__7952)];
            if (allSupportedLocales__6694.ContainsKey($"{userLocale__8045.languageCode}_{userLocale__8045.scriptCode}_{userLocale__8045.countryCode}"))
            {
                return userLocale__8045;
            }
            if ((userLocale__8045.scriptCode is not null))
            {
                global::Doroti.Ui.Locale? match__8388 = DartCollectionRuntime.NullableMapValue<Locale>(languageAndScriptLocales__6854, $"{userLocale__8045.languageCode}_{userLocale__8045.scriptCode}");
                if ((match__8388 is not null))
                {
                    Locale match__8388__value8497 = DartRuntimePrimitives.RequireValue(match__8388);
                    return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(match__8388__value8497));
                }
            }
            if ((userLocale__8045.countryCode is not null))
            {
                global::Doroti.Ui.Locale? match__8652 = DartCollectionRuntime.NullableMapValue<Locale>(languageAndCountryLocales__6771, $"{userLocale__8045.languageCode}_{userLocale__8045.countryCode}");
                if ((match__8652 is not null))
                {
                    Locale match__8652__value8763 = DartRuntimePrimitives.RequireValue(match__8652);
                    return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(match__8652__value8763));
                }
            }
            if ((matchesLanguageCode__7850 is not null))
            {
                Locale matchesLanguageCode__7850__value9013 = DartRuntimePrimitives.RequireValue(matchesLanguageCode__7850);
                return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(matchesLanguageCode__7850__value9013));
            }
            global::Doroti.Ui.Locale? match__9139 = DartCollectionRuntime.NullableMapValue<Locale>(languageLocales__6936, userLocale__8045.languageCode);
            if ((match__9139 is not null))
            {
                Locale match__9139__value9197 = DartRuntimePrimitives.RequireValue(match__9139);
                matchesLanguageCode__7850 = DartRuntimePrimitives.RequireValue(match__9139__value9197);
                if (((localeIndex__7952 == 0L) && !((((localeIndex__7952 + 1L) < checked((long)(preferredLocales.Count))) && (preferredLocales[(int)((localeIndex__7952 + 1L))].languageCode == userLocale__8045.languageCode)))))
                {
                    return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(matchesLanguageCode__7850));
                }
            }
            if (((matchesCountryCode__7881 is null) && (userLocale__8045.countryCode is not null)))
            {
                match__9139 = DartCollectionRuntime.NullableMapValue<Locale>(countryLocales__7010, userLocale__8045.countryCode);
                if ((match__9139 is not null))
                {
                    Locale match__9139__value10144 = DartRuntimePrimitives.RequireValue(match__9139);
                    matchesCountryCode__7881 = DartRuntimePrimitives.RequireValue(match__9139__value10144);
                }
            }
        }
        global::Doroti.Ui.Locale resolvedLocale__10439 = ((matchesLanguageCode__7850 ?? matchesCountryCode__7881) ?? supportedLocales.First());
        return resolvedLocale__10439;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate string GenerateAppTitle(BuildContext context);

public delegate Route<object> PageRouteFactory(RouteSettings settings, global::System.Func<BuildContext, Widget> builder);

public delegate List<dynamic> InitialRouteListFactory(string initialRoute);

public class WidgetsApp : StatefulWidget
{
    public virtual GlobalKey<NavigatorState>? navigatorKey { get; private set; }
    public virtual global::System.Func<RouteSettings, dynamic>? onGenerateRoute { get; private set; }
    public virtual global::System.Func<string, List<dynamic>>? onGenerateInitialRoutes { get; private set; }
    public virtual PageRouteFactory? pageRouteBuilder { get; private set; }
    public virtual dynamic routeInformationParser { get; private set; } = default!;
    public virtual dynamic routerDelegate { get; private set; } = default!;
    public virtual BackButtonDispatcher? backButtonDispatcher { get; private set; }
    public virtual RouteInformationProvider? routeInformationProvider { get; private set; }
    public virtual RouterConfig<object>? routerConfig { get; private set; }
    public virtual Widget? home { get; private set; }
    public virtual DartMap<string, global::System.Func<BuildContext, Widget>>? routes { get; private set; }
    public virtual global::System.Func<RouteSettings, dynamic>? onUnknownRoute { get; private set; }
    public virtual global::System.Func<NavigationNotification, bool>? onNavigationNotification { get; private set; }
    public virtual string? initialRoute { get; private set; }
    public virtual List<NavigatorObserver>? navigatorObservers { get; private set; }
    public virtual global::System.Func<BuildContext, Widget?, Widget>? builder { get; private set; }
    public virtual string? title { get; private set; }
    public virtual global::System.Func<BuildContext, string>? onGenerateTitle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual Color color { get; private set; } = default!;
    public virtual Locale? locale { get; private set; }
    public virtual IEnumerable<dynamic>? localizationsDelegates { get; private set; }
    public virtual global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback { get; private set; }
    public virtual global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback { get; private set; }
    public virtual IEnumerable<Locale> supportedLocales { get; private set; } = default!;
    public virtual bool showPerformanceOverlay { get; private set; } = default!;
    public virtual bool showSemanticsDebugger { get; private set; } = default!;
    public virtual bool debugShowWidgetInspector { get; private set; } = default!;
    public virtual ExitWidgetSelectionButtonBuilder? exitWidgetSelectionButtonBuilder { get; private set; }
    public virtual MoveExitWidgetSelectionButtonBuilder? moveExitWidgetSelectionButtonBuilder { get; private set; }
    public virtual TapBehaviorButtonBuilder? tapBehaviorButtonBuilder { get; private set; }
    public virtual bool debugShowCheckedModeBanner { get; private set; } = default!;
    public virtual DartMap<ShortcutActivator, Intent>? shortcuts { get; private set; }
    public virtual DartMap<Type, dynamic>? actions { get; private set; }
    public virtual string? restorationScopeId { get; private set; }
    public virtual bool useInheritedMediaQuery { get; private set; } = default!;
    public static bool showPerformanceOverlayOverride = false;
    public static bool debugAllowBannerOverride = true;
    internal static DartMap<ShortcutActivator, Intent> _defaultShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.enter)] = ((Intent)(object?)new ActivateIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadEnter)] = ((Intent)(object?)new ActivateIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.space)] = ((Intent)(object?)new ActivateIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.gameButtonA)] = ((Intent)(object?)new ActivateIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.select)] = ((Intent)(object?)new ActivateIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.escape)] = ((Intent)(object?)new DismissIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.tab)] = ((Intent)(object?)new NextFocusIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.tab, shift: true)] = ((Intent)(object?)new PreviousFocusIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((Intent)(object?)new DirectionalFocusIntent(TraversalDirection.left)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((Intent)(object?)new DirectionalFocusIntent(TraversalDirection.right)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((Intent)(object?)new DirectionalFocusIntent(TraversalDirection.down)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((Intent)(object?)new DirectionalFocusIntent(TraversalDirection.up)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp, control: true)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.up)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown, control: true)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.down)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowLeft, control: true)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.left)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowRight, control: true)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.right)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.pageUp)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.up, type: ScrollIncrementType.page)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.pageDown)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.down, type: ScrollIncrementType.page)) };
    internal static DartMap<ShortcutActivator, Intent> _defaultWebShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.space)] = ((Intent)(object?)new PrioritizedIntents(orderedIntents: new List<Intent> { new ActivateIntent(), new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.down, type: ScrollIncrementType.page) })), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.enter)] = ((Intent)(object?)new ButtonActivateIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadEnter)] = ((Intent)(object?)new ButtonActivateIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.escape)] = ((Intent)(object?)new DismissIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.tab)] = ((Intent)(object?)new NextFocusIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.tab, shift: true)] = ((Intent)(object?)new PreviousFocusIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.up)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.down)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.left)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.right)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.pageUp)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.up, type: ScrollIncrementType.page)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.pageDown)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.down, type: ScrollIncrementType.page)) };
    internal static DartMap<ShortcutActivator, Intent> _defaultAppleOsShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.enter)] = ((Intent)(object?)new ActivateIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.numpadEnter)] = ((Intent)(object?)new ActivateIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.space)] = ((Intent)(object?)new ActivateIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.escape)] = ((Intent)(object?)new DismissIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.tab)] = ((Intent)(object?)new NextFocusIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.tab, shift: true)] = ((Intent)(object?)new PreviousFocusIntent()), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((Intent)(object?)new DirectionalFocusIntent(TraversalDirection.left)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((Intent)(object?)new DirectionalFocusIntent(TraversalDirection.right)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((Intent)(object?)new DirectionalFocusIntent(TraversalDirection.down)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((Intent)(object?)new DirectionalFocusIntent(TraversalDirection.up)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp, meta: true)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.up)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown, meta: true)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.down)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowLeft, meta: true)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.left)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowRight, meta: true)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.right)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.pageUp)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.up, type: ScrollIncrementType.page)), [new SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.pageDown)] = ((Intent)(object?)new ScrollIntent(direction: global::Doroti.Generated.Framework.Painting.AxisDirection.down, type: ScrollIncrementType.page)) };
    public static DartMap<Type, dynamic> defaultActions = new DartMap<Type, dynamic> { [typeof(DoNothingIntent)] = new DoNothingAction(), [typeof(DoNothingAndStopPropagationIntent)] = new DoNothingAction(consumesKey: false), [typeof(RequestFocusIntent)] = new RequestFocusAction(), [typeof(NextFocusIntent)] = new NextFocusAction(), [typeof(PreviousFocusIntent)] = new PreviousFocusAction(), [typeof(DirectionalFocusIntent)] = new DirectionalFocusAction(), [typeof(ScrollIntent)] = new ScrollAction(), [typeof(PrioritizedIntents)] = new PrioritizedAction(), [typeof(VoidCallbackIntent)] = new VoidCallbackAction() };

    public WidgetsApp(global::Doroti.Generated.Framework.Foundation.Key? key = null, GlobalKey<NavigatorState>? navigatorKey = null, global::System.Func<RouteSettings, dynamic>? onGenerateRoute = null, global::System.Func<string, List<dynamic>>? onGenerateInitialRoutes = null, global::System.Func<RouteSettings, dynamic>? onUnknownRoute = null, global::System.Func<NavigationNotification, bool>? onNavigationNotification = null, List<NavigatorObserver> navigatorObservers = default!, string? initialRoute = null, PageRouteFactory? pageRouteBuilder = null, Widget? home = null, DartMap<string, global::System.Func<BuildContext, Widget>> routes = default!, global::System.Func<BuildContext, Widget?, Widget>? builder = null, string? title = null, global::System.Func<BuildContext, string>? onGenerateTitle = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, Color color = default!, Locale? locale = null, IEnumerable<dynamic>? localizationsDelegates = null, global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null, global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null, IEnumerable<Locale> supportedLocales = default!, bool showPerformanceOverlay = false, bool showSemanticsDebugger = false, bool debugShowWidgetInspector = false, bool debugShowCheckedModeBanner = true, ExitWidgetSelectionButtonBuilder? exitWidgetSelectionButtonBuilder = null, MoveExitWidgetSelectionButtonBuilder? moveExitWidgetSelectionButtonBuilder = null, TapBehaviorButtonBuilder? tapBehaviorButtonBuilder = null, DartMap<ShortcutActivator, Intent>? shortcuts = null, DartMap<Type, dynamic>? actions = null, string? restorationScopeId = null, bool useInheritedMediaQuery = false) : base(key: key)
    {
        List<NavigatorObserver> __navigatorObservers = navigatorObservers ?? new List<NavigatorObserver>();
        DartMap<string, global::System.Func<BuildContext, Widget>> __routes = routes ?? new DartMap<string, global::System.Func<BuildContext, Widget>>();
        IEnumerable<Locale> __supportedLocales = supportedLocales ?? new List<Locale> { new Locale("en", "US") };
        this.navigatorKey = navigatorKey;
        this.onGenerateRoute = onGenerateRoute;
        this.onGenerateInitialRoutes = onGenerateInitialRoutes;
        this.onUnknownRoute = onUnknownRoute;
        this.onNavigationNotification = onNavigationNotification;
        this.navigatorObservers = __navigatorObservers;
        this.initialRoute = initialRoute;
        this.pageRouteBuilder = pageRouteBuilder;
        this.home = home;
        this.routes = __routes;
        this.builder = builder;
        this.title = title;
        this.onGenerateTitle = onGenerateTitle;
        this.textStyle = textStyle;
        this.color = color;
        this.locale = locale;
        this.localizationsDelegates = localizationsDelegates;
        this.localeListResolutionCallback = localeListResolutionCallback;
        this.localeResolutionCallback = localeResolutionCallback;
        this.supportedLocales = __supportedLocales;
        this.showPerformanceOverlay = showPerformanceOverlay;
        this.showSemanticsDebugger = showSemanticsDebugger;
        this.debugShowWidgetInspector = debugShowWidgetInspector;
        this.debugShowCheckedModeBanner = debugShowCheckedModeBanner;
        this.exitWidgetSelectionButtonBuilder = exitWidgetSelectionButtonBuilder;
        this.moveExitWidgetSelectionButtonBuilder = moveExitWidgetSelectionButtonBuilder;
        this.tapBehaviorButtonBuilder = tapBehaviorButtonBuilder;
        this.shortcuts = shortcuts;
        this.actions = actions;
        this.restorationScopeId = restorationScopeId;
        this.useInheritedMediaQuery = useInheritedMediaQuery;
        this.routeInformationProvider = null;
        this.routeInformationParser = null;
        this.routerDelegate = null;
        this.backButtonDispatcher = null;
        this.routerConfig = null;
        System.Diagnostics.Debug.Assert(((home is null) || (onGenerateInitialRoutes is null)));
        System.Diagnostics.Debug.Assert(((home is null) || !__routes.ContainsKey(Navigator.defaultRouteName)));
        System.Diagnostics.Debug.Assert((((((builder is not null) || (home is not null)) || __routes.ContainsKey(Navigator.defaultRouteName)) || (onGenerateRoute is not null)) || (onUnknownRoute is not null)));
        System.Diagnostics.Debug.Assert(((((((home is not null) || System.Linq.Enumerable.Any(__routes)) || (onGenerateRoute is not null)) || (onUnknownRoute is not null))) || (((((builder is not null) && (navigatorKey is null)) && (initialRoute is null)) && !System.Linq.Enumerable.Any(__navigatorObservers)))));
        System.Diagnostics.Debug.Assert((((builder is not null) || (onGenerateRoute is not null)) || (pageRouteBuilder is not null)));
        System.Diagnostics.Debug.Assert(System.Linq.Enumerable.Any(__supportedLocales));
    }

    public static WidgetsApp CreateRouter(global::Doroti.Generated.Framework.Foundation.Key? key = null, RouteInformationProvider? routeInformationProvider = null, dynamic routeInformationParser = null, dynamic routerDelegate = null, RouterConfig<object>? routerConfig = null, BackButtonDispatcher? backButtonDispatcher = null, global::System.Func<BuildContext, Widget?, Widget>? builder = null, string? title = null, global::System.Func<BuildContext, string>? onGenerateTitle = null, global::System.Func<NavigationNotification, bool>? onNavigationNotification = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, Color color = default!, Locale? locale = null, IEnumerable<dynamic>? localizationsDelegates = null, global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null, global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null, IEnumerable<Locale> supportedLocales = default!, bool showPerformanceOverlay = false, bool showSemanticsDebugger = false, bool debugShowWidgetInspector = false, bool debugShowCheckedModeBanner = true, ExitWidgetSelectionButtonBuilder? exitWidgetSelectionButtonBuilder = null, MoveExitWidgetSelectionButtonBuilder? moveExitWidgetSelectionButtonBuilder = null, TapBehaviorButtonBuilder? tapBehaviorButtonBuilder = null, DartMap<ShortcutActivator, Intent>? shortcuts = null, DartMap<Type, dynamic>? actions = null, string? restorationScopeId = null, bool useInheritedMediaQuery = false)
    {
        var __instance = new WidgetsApp(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        IEnumerable<Locale> __supportedLocales = supportedLocales ?? new List<Locale> { new Locale("en", "US") };
        __instance.routeInformationProvider = routeInformationProvider;
        __instance.routeInformationParser = routeInformationParser;
        __instance.routerDelegate = routerDelegate;
        __instance.routerConfig = routerConfig;
        __instance.backButtonDispatcher = backButtonDispatcher;
        __instance.builder = builder;
        __instance.title = title;
        __instance.onGenerateTitle = onGenerateTitle;
        __instance.onNavigationNotification = onNavigationNotification;
        __instance.textStyle = textStyle;
        __instance.color = color;
        __instance.locale = locale;
        __instance.localizationsDelegates = localizationsDelegates;
        __instance.localeListResolutionCallback = localeListResolutionCallback;
        __instance.localeResolutionCallback = localeResolutionCallback;
        __instance.supportedLocales = __supportedLocales;
        __instance.showPerformanceOverlay = showPerformanceOverlay;
        __instance.showSemanticsDebugger = showSemanticsDebugger;
        __instance.debugShowWidgetInspector = debugShowWidgetInspector;
        __instance.debugShowCheckedModeBanner = debugShowCheckedModeBanner;
        __instance.exitWidgetSelectionButtonBuilder = exitWidgetSelectionButtonBuilder;
        __instance.moveExitWidgetSelectionButtonBuilder = moveExitWidgetSelectionButtonBuilder;
        __instance.tapBehaviorButtonBuilder = tapBehaviorButtonBuilder;
        __instance.shortcuts = shortcuts;
        __instance.actions = actions;
        __instance.restorationScopeId = restorationScopeId;
        __instance.useInheritedMediaQuery = useInheritedMediaQuery;
        __instance.navigatorObservers = null;
        __instance.navigatorKey = null;
        __instance.onGenerateRoute = null;
        __instance.pageRouteBuilder = null;
        __instance.home = null;
        __instance.onGenerateInitialRoutes = null;
        __instance.onUnknownRoute = null;
        __instance.routes = null;
        __instance.initialRoute = null;
        return __instance;
    }

    public static bool debugShowWidgetInspectorOverride
    {
        get{
            return WidgetsBinding.instance.debugShowWidgetInspectorOverrideNotifier.value;
            return default!;
        }
        set
        {
            var __value = value;
            WidgetsBinding.instance.debugShowWidgetInspectorOverrideNotifier.value = __value;
        }
    }
    public static DartMap<ShortcutActivator, Intent> defaultShortcuts
    {
        get
        {
            if (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb)
            {
                return _defaultWebShortcuts;
            }
            switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                    {
                        return _defaultShortcuts;
                    }
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                    {
                        return _defaultAppleOsShortcuts;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            return default!;
        }
    }
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _WidgetsAppState__app());
}

internal class _WidgetsAppState__app : State<WidgetsApp>, WidgetsBindingObserver
{
    internal virtual AppLifecycleState? _appLifecycleState { get; set; } = default;
    internal virtual PlatformRouteInformationProvider? _defaultRouteInformationProvider { get; set; } = default;
    internal virtual RootBackButtonDispatcher? _defaultBackButtonDispatcher { get; set; } = default;
    internal virtual GlobalKey<NavigatorState>? _navigator { get; set; } = default;
    private bool __late__localizationsResolver_initialized;
    private LocalizationsResolver __late__localizationsResolver = default!;
    internal virtual LocalizationsResolver _localizationsResolver
    {
        get
        {
            if (!__late__localizationsResolver_initialized)
            {
                __late__localizationsResolver = new LocalizationsResolver(locale: ((WidgetsApp)this.widget).locale, localeListResolutionCallback: (global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>?)((WidgetsApp)this.widget).localeListResolutionCallback, localeResolutionCallback: (global::System.Func<Locale?, IEnumerable<Locale>, Locale?>?)((WidgetsApp)this.widget).localeResolutionCallback, localizationsDelegates: ((WidgetsApp)this.widget).localizationsDelegates.Cast<dynamic>(), supportedLocales: ((WidgetsApp)this.widget).supportedLocales.Cast<Locale>());
                __late__localizationsResolver_initialized = true;
            }
            return __late__localizationsResolver;
        }
    }

    internal virtual string _initialRouteName => ((WidgetsBinding.instance.platformDispatcher.defaultRouteName != Navigator.defaultRouteName) ? WidgetsBinding.instance.platformDispatcher.defaultRouteName : (((WidgetsApp)this.widget).initialRoute ?? WidgetsBinding.instance.platformDispatcher.defaultRouteName));
    internal virtual bool _defaultOnNavigationNotification(NavigationNotification notification)
    {
        switch (this._appLifecycleState)
        {
            case null:
            case var __constant63443 when (object.Equals(__constant63443, AppLifecycleState.detached)):
                {
                    return true;
                }
            case var __constant63566 when (object.Equals(__constant63566, AppLifecycleState.inactive)):
            case var __constant63605 when (object.Equals(__constant63605, AppLifecycleState.resumed)):
            case var __constant63643 when (object.Equals(__constant63643, AppLifecycleState.hidden)):
            case var __constant63680 when (object.Equals(__constant63680, AppLifecycleState.paused)):
                {
                    DartRuntimePrimitives.Ignore(SystemNavigator.setFrameworkHandlesBack(((NavigationNotification)notification).canHandlePop));
                    return true;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didChangeAppLifecycleState(AppLifecycleState state)
    {
        _appLifecycleState = state;
        base.didChangeAppLifecycleState(state);
    }

    public override void initState()
    {
        base.initState();
        _updateRouting();
        WidgetsBinding.instance.addObserver(this);
        _appLifecycleState = WidgetsBinding.instance.lifecycleState;
    }

    public override void didUpdateWidget(WidgetsApp oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _updateRouting(oldWidget: oldWidget);
        _updateLocalizations(oldWidget: oldWidget);
    }

    public override void dispose()
    {
        WidgetsBinding.instance.removeObserver(this);
        this._defaultRouteInformationProvider?.dispose();
        this._localizationsResolver.dispose();
        base.dispose();
    }

    internal virtual void _clearRouterResource()
    {
        this._defaultRouteInformationProvider?.dispose();
        _defaultRouteInformationProvider = null;
        _defaultBackButtonDispatcher = null;
    }

    internal virtual void _clearNavigatorResource()
    {
        _navigator = null;
    }

    internal virtual void _updateRouting(WidgetsApp? oldWidget = null)
    {
        if (this._usesRouterWithDelegates)
        {
            DartRuntimePrimitives.Assert(() => (!this._usesNavigator && !this._usesRouterWithConfig));
            _clearNavigatorResource();
            if (((((WidgetsApp)this.widget).routeInformationProvider is null) && (((WidgetsApp)this.widget).routeInformationParser is not null)))
            {
                _defaultRouteInformationProvider ??= new PlatformRouteInformationProvider(initialRouteInformation: new RouteInformation(uri: DartUri.parse(this._initialRouteName)));
            }
            else
            {
                this._defaultRouteInformationProvider?.dispose();
                _defaultRouteInformationProvider = null;
            }
            if ((((WidgetsApp)this.widget).backButtonDispatcher is null))
            {
                _defaultBackButtonDispatcher ??= new RootBackButtonDispatcher();
            }
        }
        else
        {
            if (this._usesNavigator)
            {
                DartRuntimePrimitives.Assert(() => (!this._usesRouterWithDelegates && !this._usesRouterWithConfig));
                _clearRouterResource();
                if (((this._navigator is null) || (!object.Equals(((WidgetsApp)this.widget).navigatorKey, oldWidget!.navigatorKey))))
                {
                    _navigator = (((WidgetsApp)this.widget).navigatorKey ?? new GlobalObjectKey<NavigatorState>(this));
                }
                DartRuntimePrimitives.Assert(() => (this._navigator is not null));
            }
            else
            {
                DartRuntimePrimitives.Assert(() => ((((WidgetsApp)this.widget).builder is not null) || this._usesRouterWithConfig));
                DartRuntimePrimitives.Assert(() => (!this._usesRouterWithDelegates && !this._usesNavigator));
                _clearRouterResource();
                _clearNavigatorResource();
            }
        }
        DartRuntimePrimitives.Assert(() => (this._usesNavigator == ((this._navigator is not null))));
    }

    internal virtual bool _usesRouterWithDelegates => DartRuntimePrimitives.ConvertValue<bool>((((WidgetsApp)this.widget).routerDelegate is not null));
    internal virtual bool _usesRouterWithConfig => DartRuntimePrimitives.ConvertValue<bool>((((WidgetsApp)this.widget).routerConfig is not null));
    internal virtual bool _usesNavigator => DartRuntimePrimitives.ConvertValue<bool>(((((((WidgetsApp)this.widget).home is not null) || (((((WidgetsApp)this.widget).routes is { } __items66337 ? System.Linq.Enumerable.Any(__items66337) : (bool?)null) ?? false))) || (((WidgetsApp)this.widget).onGenerateRoute is not null)) || (((WidgetsApp)this.widget).onUnknownRoute is not null)));
    internal virtual RouteInformationProvider? _effectiveRouteInformationProvider => DartRuntimePrimitives.ConvertValue<RouteInformationProvider>((((WidgetsApp)this.widget).routeInformationProvider ?? this._defaultRouteInformationProvider));
    internal virtual BackButtonDispatcher _effectiveBackButtonDispatcher => DartRuntimePrimitives.ConvertValue<BackButtonDispatcher>((((WidgetsApp)this.widget).backButtonDispatcher ?? this._defaultBackButtonDispatcher!));
    internal virtual dynamic _onGenerateRoute(RouteSettings settings)
    {
        string? name__67007 = ((RouteSettings)settings).name;
        global::System.Func<BuildContext, Widget>? pageContentBuilder__67054 = ((global::System.Func<BuildContext, Widget>)(((name__67007 == Navigator.defaultRouteName) && (((WidgetsApp)this.widget).home is not null)) ? ((context) => ((WidgetsApp)this.widget).home!) : ((WidgetsApp)this.widget).routes!.GetValueOrDefault(DartRuntimePrimitives.RequireReference(name__67007))));
        if ((pageContentBuilder__67054 is not null))
        {
            DartRuntimePrimitives.Assert(() => (((WidgetsApp)this.widget).pageRouteBuilder is not null), () => (object?)"The default onGenerateRoute handler for WidgetsApp must have a " + "pageRouteBuilder set if the home or routes properties are set.");
            dynamic route__67500 = ((WidgetsApp)this.widget).pageRouteBuilder!(settings, pageContentBuilder__67054);
            return route__67500;
        }
        if ((((WidgetsApp)this.widget).onGenerateRoute is not null))
        {
            return ((WidgetsApp)this.widget).onGenerateRoute!(settings);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual dynamic _onUnknownRoute(RouteSettings settings)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((WidgetsApp)this.widget).onUnknownRoute is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"Could not find a generator for route {settings} in the {this.GetType()}.\n" + "Make sure your root app widget has provided a way to generate \n" + "this route.\n" + "Generators for routes are searched for in the following order:\n" + " 1. For the \"/\" route, the \"home\" property, if non-null, is used.\n" + " 2. Otherwise, the \"routes\" table is used, if it has an entry for " + "the route.\n" + " 3. Otherwise, onGenerateRoute is called. It should return a " + "non-null value for any valid route not handled by \"home\" and \"routes\".\n" + " 4. Finally if all else fails onUnknownRoute is called.\n" + "Unfortunately, onUnknownRoute was not set."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        dynamic result__68668 = ((WidgetsApp)this.widget).onUnknownRoute!(settings);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__68668 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("The onUnknownRoute callback returned null.\n" + $"When the {this.GetType()} requested the route {settings} from its " + "onUnknownRoute callback, the callback returned null. Such callbacks " + "must never return null."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__68668!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<bool> didPopRoute()
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        if (this._usesRouterWithDelegates)
        {
            return false;
        }
        NavigatorState? navigator__69408 = this._navigator?.currentState;
        if ((navigator__69408 is null))
        {
            return false;
        }
        return await navigator__69408.maybePop<object>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<bool> didPushRouteInformation(RouteInformation routeInformation)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        if (this._usesRouterWithDelegates)
        {
            return false;
        }
        NavigatorState? navigator__69831 = this._navigator?.currentState;
        if ((navigator__69831 is null))
        {
            return false;
        }
        DartUri uri__69938 = ((RouteInformation)routeInformation).uri;
        DartRuntimePrimitives.Ignore(navigator__69831.pushNamed<object>(Dart_coreLibrary.decodeComponent(new DartUri(path: ((uri__69938.path.Length == 0) ? "/" : uri__69938.path), queryParameters: (!System.Linq.Enumerable.Any(uri__69938.queryParametersAll) ? null : uri__69938.queryParametersAll), fragment: ((uri__69938.fragment.Length == 0) ? null : uri__69938.fragment)).ToString())));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _shouldUpdateLocalizations(WidgetsApp oldWidget)
    {
        return (((((!object.Equals(((WidgetsApp)this.widget).locale, ((WidgetsApp)oldWidget).locale)) || (!object.Equals((global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>?)((WidgetsApp)this.widget).localeListResolutionCallback, (global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>?)((WidgetsApp)oldWidget).localeListResolutionCallback))) || (!object.Equals((global::System.Func<Locale?, IEnumerable<Locale>, Locale?>?)((WidgetsApp)this.widget).localeResolutionCallback, (global::System.Func<Locale?, IEnumerable<Locale>, Locale?>?)((WidgetsApp)oldWidget).localeResolutionCallback))) || (!object.Equals(((WidgetsApp)this.widget).supportedLocales, ((WidgetsApp)oldWidget).supportedLocales))) || (!object.Equals(((WidgetsApp)this.widget).localizationsDelegates, ((WidgetsApp)oldWidget).localizationsDelegates)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateLocalizations(WidgetsApp oldWidget)
    {
        if (_shouldUpdateLocalizations(oldWidget))
        {
            this._localizationsResolver.update(locale: ((WidgetsApp)this.widget).locale, localeListResolutionCallback: (global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>?)((WidgetsApp)this.widget).localeListResolutionCallback, localeResolutionCallback: (global::System.Func<Locale?, IEnumerable<Locale>, Locale?>?)((WidgetsApp)this.widget).localeResolutionCallback, localizationsDelegates: ((WidgetsApp)this.widget).localizationsDelegates.Cast<dynamic>(), supportedLocales: ((WidgetsApp)this.widget).supportedLocales.Cast<Locale>());
        }
    }

    public override Widget build(BuildContext context)
    {
        Widget? routing__71625 = default!;
        if (this._usesRouterWithDelegates)
        {
            routing__71625 = DartRuntimePrimitives.ConvertValue<Widget>(new Router<object>(restorationScopeId: "router", routeInformationProvider: this._effectiveRouteInformationProvider, routeInformationParser: ((WidgetsApp)this.widget).routeInformationParser, routerDelegate: ((WidgetsApp)this.widget).routerDelegate!, backButtonDispatcher: this._effectiveBackButtonDispatcher));
        }
        else
        {
            if (this._usesNavigator)
            {
                DartRuntimePrimitives.Assert(() => (this._navigator is not null));
                routing__71625 = DartRuntimePrimitives.ConvertValue<Widget>(new FocusScope(debugLabel: "Navigator Scope", autofocus: true, child: new Navigator(clipBehavior: Clip.none, restorationScopeId: "nav", key: this._navigator, initialRoute: this._initialRouteName, onGenerateRoute: (global::System.Func<RouteSettings, dynamic>)this._onGenerateRoute, onGenerateInitialRoutes: ((global::System.Func<NavigatorState, string, List<dynamic>>)((((WidgetsApp)this.widget).onGenerateInitialRoutes is null) ? Navigator.defaultGenerateInitialRoutes : ((navigator, initialRouteName) => {
return ((List<object>)(object?)((WidgetsApp)this.widget).onGenerateInitialRoutes!(initialRouteName));
throw new InvalidOperationException("Dart closure completed without a value.");
}))), onUnknownRoute: (global::System.Func<RouteSettings, dynamic>)this._onUnknownRoute, observers: ((WidgetsApp)this.widget).navigatorObservers!, routeTraversalEdgeBehavior: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb ? TraversalEdgeBehavior.leaveDorotiView : TraversalEdgeBehavior.parentScope), reportsRouteUpdateToEngine: true)));
            }
            else
            {
                if (this._usesRouterWithConfig)
                {
                    routing__71625 = DartRuntimePrimitives.ConvertValue<Widget>(Router<object>.CreateWithConfig(restorationScopeId: "router", config: ((WidgetsApp)this.widget).routerConfig!));
                }
            }
        }
        Widget result__73151 = default!;
        if ((((WidgetsApp)this.widget).builder is not null))
        {
            result__73151 = DartRuntimePrimitives.ConvertValue<Widget>(new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) => {
return ((WidgetsApp)this.widget).builder!(context, routing__71625);
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (routing__71625 is not null));
            result__73151 = routing__71625!;
        }
        if ((((WidgetsApp)this.widget).textStyle is not null))
        {
            result__73151 = DartRuntimePrimitives.ConvertValue<Widget>(new DefaultTextStyle(style: ((WidgetsApp)this.widget).textStyle!, child: result__73151));
        }
        if ((((WidgetsApp)this.widget).showPerformanceOverlay || WidgetsApp.showPerformanceOverlayOverride))
        {
            result__73151 = DartRuntimePrimitives.ConvertValue<Widget>(new Stack(children: new List<Widget> { result__73151, new Positioned(top: 0.0, left: 0.0, right: 0.0, child: PerformanceOverlay.CreateAllEnabled()) }));
        }
        if (((WidgetsApp)this.widget).showSemanticsDebugger)
        {
            result__73151 = DartRuntimePrimitives.ConvertValue<Widget>(new SemanticsDebugger(child: result__73151));
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (!WidgetsBinding.instance.debugExcludeRootWidgetInspector)
                {
                    result__73151 = DartRuntimePrimitives.ConvertValue<Widget>(new ValueListenableBuilder<bool>(valueListenable: WidgetsBinding.instance.debugShowWidgetInspectorOverrideNotifier, builder: ((global::System.Func<BuildContext, bool, Widget?, Widget>)((context, debugShowWidgetInspectorOverride, child) => {
if ((((WidgetsApp)this.widget).debugShowWidgetInspector || debugShowWidgetInspectorOverride))
{
    return ((Widget)(object?)new WidgetInspector(exitWidgetSelectionButtonBuilder: (ExitWidgetSelectionButtonBuilder?)((WidgetsApp)this.widget).exitWidgetSelectionButtonBuilder, moveExitWidgetSelectionButtonBuilder: (MoveExitWidgetSelectionButtonBuilder?)((WidgetsApp)this.widget).moveExitWidgetSelectionButtonBuilder, tapBehaviorButtonBuilder: (TapBehaviorButtonBuilder?)((WidgetsApp)this.widget).tapBehaviorButtonBuilder, child: child!));
}
return child!;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: result__73151));
                }
                if ((((WidgetsApp)this.widget).debugShowCheckedModeBanner && WidgetsApp.debugAllowBannerOverride))
                {
                    result__73151 = DartRuntimePrimitives.ConvertValue<Widget>(new CheckedModeBanner(child: result__73151));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        result__73151 = DartRuntimePrimitives.ConvertValue<Widget>(new Focus(canRequestFocus: false, onKeyEvent: ((global::System.Func<FocusNode, global::Doroti.Generated.Framework.Services.KeyEvent, KeyEventResult>?)((node, @event) => {
if (((((@event is not global::Doroti.Generated.Framework.Services.KeyDownEvent) && (@event is not global::Doroti.Generated.Framework.Services.KeyRepeatEvent))) || (!object.Equals(((global::Doroti.Generated.Framework.Services.KeyEvent)@event).logicalKey, global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.escape))))
{
    return KeyEventResult.ignored;
}
return (RawTooltip.dismissAllToolTips() ? KeyEventResult.handled : KeyEventResult.ignored);
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: result__73151));
        Widget? title__75556 = default!;
        if ((((WidgetsApp)this.widget).onGenerateTitle is not null))
        {
            title__75556 = DartRuntimePrimitives.ConvertValue<Widget>(new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) => {
string title__75882 = ((WidgetsApp)this.widget).onGenerateTitle!(context);
return ((Widget)(object?)new Title(title: title__75882, color: ((WidgetsApp)this.widget).color.withOpacity(1.0), child: result__73151));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        }
        else
        {
            if (((((WidgetsApp)this.widget).title is null) && global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb))
            {
                title__75556 = null;
            }
            else
            {
                title__75556 = DartRuntimePrimitives.ConvertValue<Widget>(new Title(title: (((WidgetsApp)this.widget).title ?? ""), color: ((WidgetsApp)this.widget).color.withOpacity(1.0), child: result__73151));
            }
        }
        return ((Widget)(object?)new RootRestorationScope(restorationId: ((WidgetsApp)this.widget).restorationScopeId, child: new SharedAppData(child: new NotificationListener<NavigationNotification>(onNotification: ((((WidgetsApp)this.widget).onNavigationNotification ?? (global::System.Func<NavigationNotification, bool>)this._defaultOnNavigationNotification)), child: new Shortcuts(debugLabel: "<Default WidgetsApp Shortcuts>", shortcuts: ((((WidgetsApp)this.widget).shortcuts ?? (DartMap<ShortcutActivator, Intent>)WidgetsApp.defaultShortcuts)), child: new DefaultTextEditingShortcuts(child: new Actions(actions: (((WidgetsApp)this.widget).actions ?? new DartMap<Type, dynamic> { [typeof(ScrollIntent)] = Action<ScrollIntent>.CreateOverridable(context: context, defaultAction: new ScrollAction()) }), child: new FocusTraversalGroup(policy: new ReadingOrderTraversalPolicy(), child: new TapRegionSurface(child: new ShortcutRegistrar(child: new ListenableBuilder(listenable: this._localizationsResolver, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, _) => {
return ((Widget)(object?)new Localizations(isApplicationLevel: true, locale: ((LocalizationsResolver)this._localizationsResolver).locale, delegates: ((LocalizationsResolver)this._localizationsResolver).localizationsDelegates.ToList(), child: (title__75556 ?? result__73151)));
throw new InvalidOperationException("Dart closure completed without a value.");
})))))))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

