using System.Reflection;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;
using Duration = Doroti.Runtime.Duration;

internal static class SampleDefaultsContracts
{
    internal static void Verify()
    {
        using var environment = PlatformEnvironmentContext.Enter(new PlatformConfiguration([new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.windows));
        var theme = M.ThemeData.CreateLight();
        var context = new Material3Contracts.ThemeContext(theme);
        T Defaults<T>(string name) => (T)Activator.CreateInstance(typeof(M.ThemeData).Assembly.GetType("Doroti.Framework.Material." + name)!,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, [context], null)!;
        var bar = Defaults<M.NavigationBarThemeData>("_NavigationBarDefaultsM3__navigation_bar");
        var drawer = Defaults<M.NavigationDrawerThemeData>("_NavigationDrawerDefaultsM3__navigation_drawer");
        var rail = Defaults<M.NavigationRailThemeData>("_NavigationRailDefaultsM3__navigation_rail");
        var search = Defaults<M.SearchBarThemeData>("_SearchBarDefaultsM3__search_anchor");
        var appBar = Defaults<M.AppBarThemeData>("_AppBarDefaultsM3__app_bar");
        var selected = new HashSet<WidgetState> { WidgetState.selected };
        var decoration = new M.InputDecorationTheme(filled: true);
        var dropdown = new M.DropdownMenu<string>(inputDecorationTheme: decoration, dropdownMenuEntries: []);
        Require(dropdown.inputDecorationTheme!.filled, "DropdownMenu accepts the public InputDecorationTheme wrapper");
        var decorationData = new M.InputDecorationThemeData(filled: true);
        Require(ReferenceEquals(new M.DropdownMenu<string>(inputDecorationTheme: decorationData, dropdownMenuEntries: []).inputDecorationTheme, decorationData),
            "DropdownMenu preserves an InputDecorationThemeData instance");
        Require(bar.iconTheme!.resolve(selected)!.color == theme.colorScheme.onSecondaryContainer, "navigation bar selected icon uses the inherited default");
        Require(drawer.iconTheme!.resolve(selected)!.color == theme.colorScheme.onSecondaryContainer, "navigation drawer selected icon uses the inherited default");
        Require(rail.selectedIconTheme!.color == theme.colorScheme.onSecondaryContainer, "navigation rail selected icon uses the inherited default");
        Require(bar.backgroundColor == theme.colorScheme.surfaceContainer && bar.labelTextStyle!.resolve(selected) is not null, "navigation bar defaults dispatch through base theme type");
        Require(search.padding!.resolve(selected) is not null && search.elevation!.resolve(selected) == 6, "search defaults provide padding and elevation through base theme type");
        Require(appBar.actionsPadding is not null, "default app bar actions padding is non-null");
        foreach (var button in new M.ButtonStyleButton[] { new M.FilledButton(child: new Text("Filled")), M.FilledButton.CreateTonal(child: new Text("Tonal")), new M.TextButton(child: new Text("Text")) })
        {
            var style = button.defaultStyleOf(context);
            Require(style.minimumSize!.resolve(selected) is not null && style.iconSize!.resolve(selected) == 18, "button size and icon defaults are available");
            Require(style.foregroundColor!.resolve(selected) is not null && style.elevation!.resolve(selected) is not null, "button colors and elevation survive base style dispatch");
        }
        var bottomAppBar = Defaults<M.BottomAppBarThemeData>("_BottomAppBarDefaultsM3__bottom_app_bar");
        Require(bottomAppBar.color == theme.colorScheme.surfaceContainer && bottomAppBar.shadowColor is not null, "bottom app bar cannot paint a null physical shape color");
        var icon = new Icon(M.Icons.add);
        var label = new Text("Create");
        foreach (var button in new M.ButtonStyleButton[] { M.ElevatedButton.CreateIcon(icon: icon, label: label), M.FilledButton.CreateIcon(icon: icon, label: label), M.FilledButton.CreateTonalIcon(icon: icon, label: label), M.OutlinedButton.CreateIcon(icon: icon, label: label), M.TextButton.CreateIcon(icon: icon, label: label) })
            Require(button.child is StatelessWidget && !ReferenceEquals(button.child, label), "icon factory preserves both icon and label child");
        Require(ReferenceEquals(M.FilledButton.CreateIcon(label: label).child, label), "null icon keeps label");
        var stringRoute = new M.MaterialPageRoute<string>(_ => SizedBox.CreateShrink());
        var scopeType = typeof(ModalRoute<>).Assembly.GetType("Doroti.Framework.Widgets._ModalScopeStatus__routes")!;
        var scope = (InheritedWidget)Activator.CreateInstance(scopeType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, [true, true, true, stringRoute, true, SizedBox.CreateShrink()], null)!;
        var routeContext = new RouteScopeContext(scope);
        Require(ModalRoute<object>.isCurrentOf(routeContext) == true && ModalRoute<object>.canPopOf(routeContext) == true,
            "route status queries support a string result inside an object navigator");
        Require(ReferenceEquals(ModalRoute<object>.settingsOf(routeContext), stringRoute.settings), "untyped route settings preserve identity");
        Console.WriteLine("D18/heterogeneous-route-status: PASS");
        var searchRouteType = typeof(M.SearchAnchor).Assembly.GetType("Doroti.Framework.Material._SearchViewRoute__search_anchor")!;
        var didPop = searchRouteType.GetMethod("didPop", [searchRouteType])!;
        Require(didPop.GetBaseDefinition().DeclaringType != searchRouteType, "search close overrides route pop so anchor visibility is restored");
        Console.WriteLine("D18/search-route-pop-dispatch: PASS");
        Require(Doroti.Runtime.DartRuntimePrimitives.CreateDateTime(2026, 13, 0) == new DateTime(2026, 12, 31), "Dart dates normalize month overflow and day zero");
        Require(Doroti.Runtime.DartRuntimePrimitives.CreateDateTime(2026, 0, 1) == new DateTime(2025, 12, 1), "Dart dates normalize negative month offsets");
        Require(M.DateUtils.addMonthsToMonthDate(new DateTime(2026, 9, 6), 5) == new DateTime(2027, 2, 1), "calendar paging crosses year boundaries");
        Console.WriteLine("D19/calendar-date-normalization: PASS");
        Require(new RestorableBoolN(null).createDefaultValue() is null && new RestorableDoubleN(null).createDefaultValue() is null
            && new RestorableIntN(null).createDefaultValue() is null, "nullable restoration properties retain absent defaults");
        Console.WriteLine("D19/nullable-restoration: PASS");
        var aspectModel = new PrivateAspectModel();
        Require(ReferenceEquals(InheritedModel<object>.inheritFrom<PrivateAspectModel>(new RouteScopeContext(aspectModel), "value"), aspectModel),
            "private inherited models in consumer assemblies support typed aspect discovery");
        Console.WriteLine("D19/private-inherited-model: PASS");
        var animation = new Doroti.Framework.Animation.AnimationController(duration: Duration.zero, vsync: new TestTickerProvider(), animationBehavior: Doroti.Framework.Animation.AnimationBehavior.preserve);
        using var completed = new ManualResetEventSlim();
        animation.forward().whenComplete(() => { completed.Set(); return null!; });
        Require(completed.Wait(TimeSpan.FromSeconds(2)) && animation.value == 1, "zero duration animations return a completed future");
        animation.dispose();
        Console.WriteLine("D20/zero-duration-completion: PASS");
        Console.WriteLine("D09/Material3-default-dispatch: PASS");
    }
    private sealed class RouteScopeContext(InheritedWidget scope) : StatelessElement(new Builder(builder: _ => SizedBox.CreateShrink()))
    {
        public override InheritedElement? getElementForInheritedWidgetOfExactType<T>() => scope is T ? scope.createElement() : null;
        public override InheritedWidget dependOnInheritedElement(InheritedElement ancestor, object? aspect = null) => (InheritedWidget)ancestor.widget;
    }
    private sealed class PrivateAspectModel() : InheritedModel<string>(child: SizedBox.CreateShrink())
    {
        public override bool updateShouldNotify(InheritedWidget oldWidget) => false;
        public override bool updateShouldNotifyDependent(InheritedModel<string> oldWidget, HashSet<string> dependencies) => false;
        public override bool isSupportedAspect(object aspect) => Equals(aspect, "value");
    }
    private sealed class TestTickerProvider : Doroti.Framework.Scheduler.TickerProvider
    {
        public Doroti.Framework.Scheduler.Ticker createTicker(System.Action<Duration> onTick) => new(onTick);
    }
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
