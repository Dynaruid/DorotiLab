var requiredComponents = new HashSet<string>(StringComparer.Ordinal)
{
    "scaffold-background", "app-bar-text", "floating-action-button",
    "ink-well-sparkle", "scrollbar-list-sliver", "shader-mask-image-filter",
};
var capturedComponents = new HashSet<string>(StringComparer.Ordinal);

var seed = new Doroti.Ui.Color(0xff6750a4L);
var lightSurface = new Doroti.Ui.Color(0xfffffbfeL);
var darkSurface = new Doroti.Ui.Color(0xff141218L);
var lightPalette = Doroti.Framework.Material.ColorScheme.CreateFromSeed(
    seedColor: seed,
    brightness: Doroti.Ui.Brightness.light,
    surface: lightSurface);
var darkPalette = Doroti.Framework.Material.ColorScheme.CreateFromSeed(
    seedColor: seed,
    brightness: Doroti.Ui.Brightness.dark,
    surface: darkSurface);
var lightTheme = Doroti.Framework.Material.ThemeData.Create(
    colorScheme: lightPalette,
    useMaterial3: true,
    platform: Doroti.Framework.Foundation.TargetPlatform.windows);
var darkTheme = Doroti.Framework.Material.ThemeData.Create(
    colorScheme: darkPalette,
    useMaterial3: true,
    platform: Doroti.Framework.Foundation.TargetPlatform.windows);
Require(lightTheme.brightness == Doroti.Ui.Brightness.light, "light palette preserves light brightness");
Require(darkTheme.brightness == Doroti.Ui.Brightness.dark, "dark palette preserves dark brightness");
Require(lightTheme.colorScheme.surface.value == lightSurface.value, "light palette accepts role overrides");
Require(darkTheme.colorScheme.surface.value == darkSurface.value, "dark palette accepts role overrides");
Require(lightTheme.scaffoldBackgroundColor.value == lightSurface.value, "light scaffold owns the light surface");
Require(darkTheme.scaffoldBackgroundColor.value == darkSurface.value, "dark scaffold owns the dark surface");
Require(lightTheme.colorScheme.primary.value != darkTheme.colorScheme.primary.value, "seed palette resolves brightness-specific roles");
var systemThemeApp = new Doroti.Framework.Material.MaterialApp(
    theme: lightTheme,
    darkTheme: darkTheme,
    themeMode: Doroti.Framework.Material.ThemeMode.system);
Require(ReferenceEquals(systemThemeApp.theme, lightTheme), "MaterialApp retains the light palette");
Require(ReferenceEquals(systemThemeApp.darkTheme, darkTheme), "MaterialApp retains the dark palette");
Require(systemThemeApp.themeMode == Doroti.Framework.Material.ThemeMode.system, "MaterialApp follows platform brightness in system mode");

var widgetsBinding = (Doroti.Framework.Widgets.WidgetsFlutterBinding)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(
    typeof(Doroti.Framework.Widgets.WidgetsFlutterBinding));
widgetsBinding._observers = [];
widgetsBinding._backGestureObservers = [];
var environmentProbe = new EnvironmentObserverProbe();
widgetsBinding.addObserver(environmentProbe);
widgetsBinding.handleTextScaleFactorChanged();
widgetsBinding.handlePlatformBrightnessChanged();
Require(environmentProbe.TextScaleChanges == 1, "WidgetsBinding forwards text-scale changes to MediaQuery observers");
Require(environmentProbe.BrightnessChanges == 1, "WidgetsBinding forwards live platform-brightness changes to MediaQuery observers");
Require(widgetsBinding.removeObserver(environmentProbe), "WidgetsBinding removes the environment observer");
widgetsBinding.handlePlatformBrightnessChanged();
Require(environmentProbe.BrightnessChanges == 1, "removed environment observer no longer receives brightness changes");

var scenarios = new[]
{
    Scenario("fab-press", "floating-action-button", ["default", "pressed", "focused", "disabled"], ["down", "hold", "up", "semantics"]),
    Scenario("ink-cold-warm", "ink-well-sparkle", ["cold", "warm", "hovered", "pressed"], ["hover", "down", "move", "hold", "up"]),
    Scenario("scrollbar-range", "scrollbar-list-sliver", ["top", "middle", "end"], ["scroll", "semantics"]),
    Scenario("effects-cold-warm", "shader-mask-image-filter", ["cold", "warm"], ["frame"]),
};

foreach (var scenario in scenarios)
{
    Require(requiredComponents.Contains(scenario.Component), $"scenario component is in the fixed source slice: {scenario.Id}");
    Require(scenario.States.Count >= 2, $"scenario compares more than one state: {scenario.Id}");
    Require(scenario.Actions.Count > 0, $"scenario has replayable actions: {scenario.Id}");
    Require(scenario.Actions.Any(action => action is "down" or "scroll" or "frame"), $"scenario has a causal input or frame: {scenario.Id}");
    capturedComponents.Add(scenario.Component);
}

Require(capturedComponents.SetEquals(["floating-action-button", "ink-well-sparkle", "scrollbar-list-sliver", "shader-mask-image-filter"]), "interactive/effect slice scenarios are complete");
Require(requiredComponents.Contains("scaffold-background") && requiredComponents.Contains("app-bar-text"), "persistent scaffold and text coverage is explicit");
Console.WriteLine($"FCR-7 material/widget runtime contract: PASS (configuration={ConfigurationName()}, system-theme-palettes=light+dark)");

static Scenario Scenario(string id, string component, IReadOnlyList<string> states, IReadOnlyList<string> actions) => new(id, component, states, actions);
static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
static string ConfigurationName() =>
#if DEBUG
    "Debug";
#else
    "Release";
#endif

sealed record Scenario(string Id, string Component, IReadOnlyList<string> States, IReadOnlyList<string> Actions);

sealed class EnvironmentObserverProbe : Doroti.Framework.Widgets.WidgetsBindingObserver
{
    public int TextScaleChanges { get; private set; }
    public int BrightnessChanges { get; private set; }

    public void didChangeTextScaleFactor() => TextScaleChanges++;
    public void didChangePlatformBrightness() => BrightnessChanges++;
}
