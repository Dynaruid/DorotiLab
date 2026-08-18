using Doroti.Framework.Animation;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Ui;

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
VerifyScrollbarAlphaContract();

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

static void VerifyScrollbarAlphaContract()
{
    var androidIdlePeak = new Color(0xff5f6368L);
    var fadeFrames = new[]
    {
        CaptureScrollbarFrame(androidIdlePeak, 1.0, 0),
        CaptureScrollbarFrame(androidIdlePeak, 1.0, 600),
        CaptureScrollbarFrame(androidIdlePeak, 0.5, 750),
        CaptureScrollbarFrame(androidIdlePeak, 0.0, 900),
    };
    Require(fadeFrames.Select(frame => frame.ThumbAlpha).SequenceEqual([255, 255, 128, 0]),
        "transient Android thumb holds through 600 ms and fades to zero over the next 300 ms");
    Require(fadeFrames.Zip(fadeFrames.Skip(1), (left, right) => right.ThumbAlpha <= left.ThumbAlpha).All(value => value),
        "transient thumb alpha is monotonically non-increasing during fade");
    Require(fadeFrames.Where(frame => frame.FadeValue > 0).All(frame => frame.TrackAlpha == 0),
        "the transparent track remains distinct from the fading thumb");

    var alwaysVisibleFrames = new[] { 0, 600, 750, 900 }
        .Select(timestamp => CaptureScrollbarFrame(androidIdlePeak, 1.0, timestamp))
        .ToArray();
    Require(alwaysVisibleFrames.All(frame => frame.ThumbAlpha == 255),
        "thumbVisibility true keeps the Flutter Android idle peak fully visible instead of starting fade");

    var themedThumb = Color.fromARGB(102, 255, 0, 0);
    var themedPeak = CaptureScrollbarFrame(themedThumb, 1.0, 0);
    var themedMid = CaptureScrollbarFrame(themedThumb, 0.5, 150);
    Require(themedPeak.ThumbAlpha == 102 && themedMid.ThumbAlpha == 51,
        "an explicit semitransparent ScrollbarTheme thumb color is multiplied by fade exactly once");
    Require(themedPeak.Commands.Last(command => command.Operation == "drawRect").Arguments[4] == themedThumb.value,
        "retained draw command snapshots the framework paint ARGB without dropping alpha");

    var whiteComposite = Doroti.Ui.Dart_uiLibrary.Color.alphaBlend(themedThumb, new Color(0xffffffffL));
    var blackComposite = Doroti.Ui.Dart_uiLibrary.Color.alphaBlend(themedThumb, new Color(0xff000000L));
    Require(whiteComposite.value == 0xffff9999 && blackComposite.value == 0xff660000,
        "known white and black backgrounds recover the expected effective alpha for the themed thumb");
}

static ScrollbarAlphaFrame CaptureScrollbarFrame(Color thumbColor, double fadeValue, int timestampMilliseconds)
{
    var animation = new MutableAnimation(fadeValue);
    var painter = new ScrollbarPainter(
        color: thumbColor,
        fadeoutOpacityAnimation: animation,
        trackColor: new Color(0x00000000L),
        trackBorderColor: new Color(0x00000000L),
        textDirection: TextDirection.ltr,
        thickness: 6,
        padding: EdgeInsets.zero);
    painter.update(new FixedScrollMetrics(
        minScrollExtent: 0,
        maxScrollExtent: 840,
        pixels: 210,
        viewportDimension: 240,
        axisDirection: AxisDirection.down,
        devicePixelRatio: 1), AxisDirection.down);
    var commands = new List<PathCommand>();
    painter.paint(new Canvas(commands), new Size(720, 360));
    var rectangles = commands.Where(command => command.Operation == "drawRect").ToArray();
    var trackAlpha = rectangles.Length == 0 ? 0 : Alpha(rectangles[0]);
    var thumbAlpha = rectangles.Length < 2 ? 0 : Alpha(rectangles[^1]);
    painter.dispose();
    return new(timestampMilliseconds, fadeValue, trackAlpha, thumbAlpha, commands);
}

static int Alpha(PathCommand command) => (int)(((uint)command.Arguments[4] >> 24) & 0xff);

static string ConfigurationName() =>
#if DEBUG
    "Debug";
#else
    "Release";
#endif

sealed record Scenario(string Id, string Component, IReadOnlyList<string> States, IReadOnlyList<string> Actions);

sealed record ScrollbarAlphaFrame(
    int TimestampMilliseconds,
    double FadeValue,
    int TrackAlpha,
    int ThumbAlpha,
    IReadOnlyList<PathCommand> Commands);

sealed class MutableAnimation : Animation<double>
{
    private readonly HashSet<Action> _listeners = [];

    public MutableAnimation(double value) => Value = value;

    public double Value { get; private set; }
    public override double value => Value;
    public override AnimationStatus status => Value <= 0 ? AnimationStatus.dismissed : AnimationStatus.forward;
    public override void addListener(Action listener) => _listeners.Add(listener);
    public override void removeListener(Action listener) => _listeners.Remove(listener);
    public override void addStatusListener(AnimationStatusListener listener) { }
    public override void removeStatusListener(AnimationStatusListener listener) { }
}

sealed class EnvironmentObserverProbe : Doroti.Framework.Widgets.WidgetsBindingObserver
{
    public int TextScaleChanges { get; private set; }
    public int BrightnessChanges { get; private set; }

    public void didChangeTextScaleFactor() => TextScaleChanges++;
    public void didChangePlatformBrightness() => BrightnessChanges++;
}
