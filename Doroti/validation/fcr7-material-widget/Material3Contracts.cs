using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using Material = Doroti.Framework.Material;

internal static class Material3Contracts
{
    public static void Verify()
    {
        using var environment = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
            [new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.windows));
        var light = Material.ThemeData.CreateLight();
        var dark = Material.ThemeData.CreateDark();
        var seed = new Color(0xff6750a4L);
        foreach (var theme in new[]
                 {
                     light, dark, Material.ThemeData.CreateFallback(),
                     Material.ThemeData.Create(colorSchemeSeed: seed, platform: TargetPlatform.windows),
                     Material.ThemeData.CreateFrom(Material.ColorScheme.CreateFromSeed(seedColor: seed)),
                     light.copyWith(), Material.ThemeData.lerp(light, dark, 0.5),
                 })
        {
            Require(theme.typography.englishLike.displayLarge!.fontSize == 57,
                "theme factories, copies and interpolation retain Material 3 typography");
            var context = new ThemeContext(theme);
            var resolved = Material.Theme.of(context);
            Require(resolved.textTheme.displayLarge!.fontSize == 57,
                "localized theme applies Material 3 display geometry");
            var button = new Material.ElevatedButton(onPressed: () => { }, child: SizedBox.CreateShrink());
            var style = button.defaultStyleOf(context);
            var states = new HashSet<WidgetState>();
            Require(style.minimumSize!.resolve(states)!.height == 40,
                "elevated button uses the Material 3 minimum height");
            Require(style.shape!.resolve(states) is StadiumBorder,
                "elevated button uses the Material 3 rounded shape");
            Require(style.backgroundColor!.resolve(states)!.value == resolved.colorScheme.surfaceContainerLow.value,
                "elevated button reads the active Material 3 surface role");
        }

        Require(light.colorScheme.surface.value == 0xfffef7ffL && dark.colorScheme.surface.value == 0xff141218L,
            "default light and dark factories retain the Material 3 baseline surfaces");
        Require(light.Equals(light.copyWith()) && light.GetHashCode() == light.copyWith().GetHashCode(),
            "theme equality and hashing remain consistent after removing the version field");
        Require(Material.Typography.Create().englishLike.displayLarge!.fontSize == 57,
            "standalone typography defaults to Material 3");
        Console.WriteLine("Material 3 theme and button contracts: PASS");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed class ThemeContext : StatelessElement
    {
        private readonly InheritedTheme _theme;
        private readonly MediaQuery _mediaQuery = new(data: new MediaQueryData(), child: SizedBox.CreateShrink());

        public ThemeContext(Material.ThemeData data) : base(new Builder(builder: _ => SizedBox.CreateShrink()))
        {
            _theme = (InheritedTheme)new Material.Theme(data: data, child: SizedBox.CreateShrink()).build(this);
        }

        public override T? dependOnInheritedWidgetOfExactType<T>(object? aspect = null) where T : default =>
            _theme is T theme ? theme : _mediaQuery is T query ? query : default;

        public override InheritedElement? getElementForInheritedWidgetOfExactType<T>() =>
            typeof(T) == typeof(MediaQuery) ? _mediaQuery.createElement() : null;

        public override InheritedWidget dependOnInheritedElement(InheritedElement ancestor, object? aspect = null) =>
            (InheritedWidget)ancestor.widget;
    }
}
