using System.Text;
using System.Text.Json;
using Doroti.Flutter.Ui;
using Doroti.Generated.Framework.Foundation;
using Doroti.Generated.Framework.Rendering;
using Doroti.Generated.Framework.Widgets;
using Cupertino = Doroti.Generated.Framework.Cupertino;
using Material = Doroti.Generated.Framework.Material;
using Path = System.IO.Path;

var evidencePath = ReadOption(args, "--evidence") ??
    throw new ArgumentException("--evidence is required.");
var failures = new List<string>();
var traces = new List<object>();
FlutterErrorDetails? firstFrameworkError = null;
FlutterError.onError = details => firstFrameworkError ??= details;

using var dispatcher = new PlatformDispatcher();
using var dispatcherScope = dispatcher.EnterScope();
var fixtureHost = new FixtureHost();
using var view = dispatcher.RegisterView(720, new FlutterViewCapabilities("g7-product-managed")
    .Register<IViewHostCapability>(FlutterCapabilityIds.ViewLifecycleMetrics, fixtureHost)
    .Register<IFrameHostCapability>(FlutterCapabilityIds.ViewFrameDispatch, fixtureHost)
    .Register<IPlatformMessageHostCapability>(FlutterCapabilityIds.PlatformMessaging, fixtureHost));
using var binding = new WidgetsFlutterBinding(dispatcher);

foreach (var platform in new[] { TargetPlatform.windows, TargetPlatform.macOS })
{
    using var environment = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
        [], Brightness.light, false, false,
        platform == TargetPlatform.macOS ? HostOperatingSystem.macOS : HostOperatingSystem.windows));
    var probe = new AdaptiveProbe();
    var themedProbe = new Material.Theme(
        data: Material.ThemeData.Create(platform: platform),
        child: new Material.Material(child: probe));
    var owner = new BuildOwner();
    var root = new RootWidget(child: new Directionality(
        textDirection: TextDirection.ltr,
        child: new MediaQuery(
            data: new MediaQueryData(size: new Size(800, 600)),
            child: new Overlay(initialEntries: [new OverlayEntry(_ => themedProbe)]))));
    var element = root.attach(owner);
    owner.buildScope(element);

    var widgetTypes = Flatten(element)
        .Select(candidate => candidate.widget.GetType().Name)
        .ToHashSet(StringComparer.Ordinal);
    var cupertinoTypes = new[]
    {
        nameof(Cupertino.CupertinoCheckbox),
        nameof(Cupertino.CupertinoSlider),
        nameof(Cupertino.CupertinoActivityIndicator),
    };
    var selectedCupertino = cupertinoTypes.All(widgetTypes.Contains);
    var selectedMaterial = new[] { "Checkbox", "Switch", "Slider", "CircularProgressIndicator" }
        .All(widgetTypes.Contains) && cupertinoTypes.All(type => !widgetTypes.Contains(type)) &&
        !widgetTypes.Contains(nameof(Cupertino.CupertinoSwitch));
    if (platform == TargetPlatform.macOS &&
        (!selectedCupertino || !widgetTypes.Contains("Switch") || widgetTypes.Contains(nameof(Cupertino.CupertinoSwitch))))
        failures.Add($"macOS adaptive selection did not produce all Cupertino controls: {string.Join(',', widgetTypes.Order())}");
    if (platform == TargetPlatform.windows && !selectedMaterial)
        failures.Add($"Windows adaptive selection unexpectedly produced a Cupertino control: {string.Join(',', widgetTypes.Order())}");

    var state = probe.State ?? throw new InvalidDataException("Adaptive probe State was not mounted.");
    state.ExerciseManagedActions();
    owner.buildScope(element);
    if (state.CallbackCount != 3 || !state.Checked || !state.Switched || state.SliderValue != 0.75)
        failures.Add($"{platform} managed adaptive callbacks did not preserve state.");

    var semantics = Flatten(element)
        .Select(candidate => candidate.widget)
        .OfType<Semantics>()
        .Where(candidate => candidate.properties.label?.StartsWith("adaptive ", StringComparison.Ordinal) == true)
        .Select(candidate => new
        {
            candidate.properties.label,
            hasAction = candidate.properties.onTap is not null || candidate.properties.onIncrease is not null,
        })
        .OrderBy(candidate => candidate.label, StringComparer.Ordinal)
        .ToArray();
    if (semantics.Length != 4 || semantics.Count(item => item.hasAction) != 3)
        failures.Add($"{platform} adaptive semantics/action contract drifted.");

    traces.Add(new
    {
        platform = platform.ToString(),
        selection = platform == TargetPlatform.macOS
            ? new Dictionary<string, string>
            {
                ["checkbox"] = "cupertino",
                ["switch"] = "material-cupertino-colors",
                ["slider"] = "cupertino",
                ["progress"] = "cupertino",
            }
            : new Dictionary<string, string>
            {
                ["checkbox"] = "material",
                ["switch"] = "material",
                ["slider"] = "material",
                ["progress"] = "material",
            },
        controls = new[] { "checkbox", "switch", "slider", "progress" },
        callbackCount = state.CallbackCount,
        checkboxValue = state.Checked,
        switchValue = state.Switched,
        sliderValue = state.SliderValue,
        semantics,
        selectedWidgetTypes = widgetTypes.Where(type =>
            type.StartsWith("Cupertino", StringComparison.Ordinal) ||
            type is "Checkbox" or "Switch" or "Slider" or "CircularProgressIndicator")
            .Order(StringComparer.Ordinal)
            .ToArray(),
    });

    new RootWidget(child: null).attach(owner, element);
    owner.buildScope(element);
    owner.finalizeTree();
}

if (firstFrameworkError is not null)
    failures.Add($"FlutterError: {firstFrameworkError.exceptionThrown}");
FlutterError.onError = null;

WriteJson(evidencePath, new
{
    schemaVersion = "doroti.g7-product-managed/v1",
    milestone = "G7-2",
    capturedAtUtc = DateTimeOffset.UtcNow,
    status = failures.Count == 0 ? "pass" : "failed",
    host = OperatingSystem.IsMacOS() ? "macos" : OperatingSystem.IsWindows() ? "windows" : "other",
    trace = traces,
    capabilityReuse = new
    {
        pointerKeySemantics = "managed contract only; native capability evidence is composed by validate-g7-product.ps1",
        directCallbackNativePasses = 0,
    },
    frameDispatchCount = fixtureHost.FrameDispatchCount,
    failures,
});

Console.WriteLine($"G7-2 managed Cupertino/adaptive product trace: {(failures.Count == 0 ? "PASS" : "FAIL")}");
foreach (var failure in failures) Console.Error.WriteLine(failure);
return failures.Count == 0 ? 0 : 1;

static IEnumerable<Element> Flatten(Element root)
{
    yield return root;
    var children = new List<Element>();
    root.visitChildren(children.Add);
    foreach (var child in children)
        foreach (var descendant in Flatten(child))
            yield return descendant;
}

static string? ReadOption(string[] values, string name)
{
    var index = Array.IndexOf(values, name);
    return index >= 0 && index + 1 < values.Length ? values[index + 1] : null;
}

static void WriteJson(string path, object value)
{
    var fullPath = Path.GetFullPath(path);
    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
    File.WriteAllText(fullPath, JsonSerializer.Serialize(value, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    }) + "\n", new UTF8Encoding(false));
}

sealed class AdaptiveProbe : StatefulWidget
{
    internal AdaptiveProbeState? State { get; private set; }
    public override IState createState() => new AdaptiveProbeState(state => State = state);
}

sealed class AdaptiveProbeState(System.Action<AdaptiveProbeState> mounted) : State<AdaptiveProbe>
{
    internal bool Checked { get; private set; }
    internal bool Switched { get; private set; }
    internal double SliderValue { get; private set; } = 0.25;
    internal int CallbackCount { get; private set; }

    public override void initState()
    {
        base.initState();
        mounted(this);
    }

    internal void ExerciseManagedActions()
    {
        setState(() =>
        {
            Checked = true;
            Switched = true;
            SliderValue = 0.75;
            CallbackCount += 3;
        });
    }

    public override Widget build(BuildContext context) => new Column(children:
    [
        new Semantics(
            label: "adaptive checkbox",
            @checked: Checked,
            onTap: () => setState(() => { Checked = !Checked; CallbackCount++; }),
            child: Material.Checkbox.CreateAdaptive(value: Checked, onChanged: value =>
                setState(() => { Checked = value ?? false; CallbackCount++; }))),
        new Semantics(
            label: "adaptive switch",
            toggled: Switched,
            onTap: () => setState(() => { Switched = !Switched; CallbackCount++; }),
            child: Material.Switch.CreateAdaptive(
                value: Switched,
                onChanged: value => setState(() => { Switched = value; CallbackCount++; }),
                thumbColor: new WidgetStatePropertyAll<Color?>(new Color(0xff6750a4)),
                trackColor: new WidgetStatePropertyAll<Color?>(new Color(0xffd0bcff)),
                trackOutlineColor: new WidgetStatePropertyAll<Color?>(new Color(0xff79747e)),
                trackOutlineWidth: new WidgetStatePropertyAll<double?>(1))),
        new Semantics(
            label: "adaptive slider",
            slider: true,
            onIncrease: () => setState(() => { SliderValue = Math.Min(1, SliderValue + 0.1); CallbackCount++; }),
            child: Material.Slider.CreateAdaptive(value: SliderValue, onChanged: value =>
                setState(() => { SliderValue = value; CallbackCount++; }))),
        new Semantics(
            label: "adaptive progress",
            value: "50%",
            child: Material.CircularProgressIndicator.CreateAdaptive(
                value: 0.5,
                valueColor: new Doroti.Generated.Framework.Animation.AlwaysStoppedAnimation<Color?>(new Color(0xff6750a4)),
                strokeWidth: 4,
                strokeAlign: 0,
                constraints: new BoxConstraints(minWidth: 36, minHeight: 36))),
    ]);
}

sealed class FixtureHost : IViewHostCapability, IFrameHostCapability, IPlatformMessageHostCapability
{
    public int FrameDispatchCount { get; private set; }
    public ViewMetrics Metrics { get; } = new(new Size(800, 600), 1, ViewPadding.zero, ViewPadding.zero,
        ViewPadding.zero, AppLifecycleState.resumed, 0, 0);
    public event System.Action<ViewMetrics>? MetricsChanged { add { } remove { } }
    public event System.Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
    public event System.Action? CloseRequested { add { } remove { } }
    public event System.Action? Closed { add { } remove { } }
    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken = default) => ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);
    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
    public void ScheduleFrame(System.Action<TimeSpan> callback)
    {
        FrameDispatchCount++;
        callback(TimeSpan.FromMilliseconds(FrameDispatchCount * 16));
    }
    public void Show() { }
    public void Resize(Size logicalSize) { }
    public void Close() { }
    public void Dispose() { }
}
