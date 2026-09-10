using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using Aspect = Doroti.Framework.Widgets._MediaQueryAspect__media_query;

var checks = 0;
void Require(bool condition, string label)
{
    if (!condition) throw new Exception(label);
    checks++;
}
using var dispatcher = new PlatformDispatcher();
using var scope = dispatcher.EnterScope();
using var host = new FixtureHost();
using var view = dispatcher.RegisterView(1, host.Capabilities());
var notifications = 0;
dispatcher.onMetricsChanged += _ => notifications++;
foreach (var dpr in new[] { .75, 1, 1.25, 2, 3 })
{
    host.Publish(host.Metrics with { physicalSize = new Size(360 * dpr, 800 * dpr), devicePixelRatio = dpr,
        viewPadding = new(6 * dpr, 12 * dpr, 18 * dpr, 24 * dpr) });
    foreach (var keyboard in new[] { 0d, 10, 300, 0 })
    {
        host.Publish(host.Metrics with { viewInsets = new(0, 0, 0, keyboard * dpr) });
        var data = MediaQueryData.CreateFromView(view);
        Require(data.size == new Size(360, 800) && data.devicePixelRatio == dpr, "physical to logical once");
        Require(data.viewPadding.bottom == 24 && data.viewInsets.bottom == keyboard &&
            data.padding.bottom == Math.Max(0, 24 - keyboard), "keyboard derives padding without modifying viewPadding");
        Require(data.padding.left == 6 && data.padding.top == 12 && data.padding.right == 18, "asymmetric insets");
    }
}
var before = notifications;
host.Publish(host.Metrics);
Require(notifications == before, "same values do not notify even with a new metrics generation");
var stale = host.Metrics with { generation = host.Metrics.generation - 1, viewInsets = new(0, 0, 0, 999) };
host.Deliver(stale);
Require(view.viewInsets.bottom == 0 && notifications == before, "stale native event rejected");
Require(view.metrics.surfaceGeneration == 7 && host.ViewEpoch.ResizeTargetGeneration == 1, "insets do not create surfaces or resize targets");
var sourceFeatures = new List<DisplayFeature>();
var frozen = host.Metrics with { displayFeatures = sourceFeatures };
sourceFeatures.Add(new DisplayFeature(new Rect(10, 0, 11, 800), DisplayFeatureType.hinge, DisplayFeatureState.unknown));
Require(frozen.displayFeatures.Count == 0, "snapshot owns an immutable feature copy");
host.Publish(host.Metrics with { displayFeatures = sourceFeatures, displayCornerRadii = new(30, 60, 90, 120), gestureSettings = new(45) });
var featureData = MediaQueryData.CreateFromView(view);
Require(featureData.displayFeatures[0].bounds.left == 10 && featureData.gestureSettings.touchSlop == 15 &&
    featureData.displayCornerRadii!.topLeft.x == 10, "logical features, physical corner and slop units");
foreach (var invalid in new[] { double.NaN, double.PositiveInfinity, -1, 0 })
{
    var rejected = false;
    try { (host.Metrics with { devicePixelRatio = invalid }).Validate(); } catch (ArgumentOutOfRangeException) { rejected = true; }
    Require(rejected, "invalid DPR rejected");
}
Require(ViewOcclusion.EdgeInsets(new Rect(0, 0, 360, 800), new Rect(50, 500, 300, 750)) == ViewPadding.zero, "floating keyboard stays internal");
Require(ViewOcclusion.EdgeInsets(new Rect(0, 0, 360, 500), new Rect(0, 500, 360, 800)) == ViewPadding.zero, "already resized content not inset twice");
Require(ViewOcclusion.EdgeInsets(new Rect(0, 0, 360, 800), new Rect(0, 500, 360, 800)).bottom == 300, "docked keyboard intersects view");
Require(ViewOcclusion.SafeEdges(new Rect(0, 24, 360, 800), new Rect(0, 0, 360, 800), new(0, 24, 0, 0)).top == 0, "parent-consumed system edge excluded");

var textNotifications = 0; var brightnessNotifications = 0; var localeNotifications = 0;
dispatcher.onTextScaleFactorChanged += () => textNotifications++;
dispatcher.onPlatformBrightnessChanged += () => brightnessNotifications++;
dispatcher.onLocaleChanged += () => localeNotifications++;
host.Configure(host.Configuration with { alwaysUse24HourFormat = true });
Require(textNotifications == 0 && brightnessNotifications == 0, "24h update does not fake text or brightness callbacks");
host.Configure(host.Configuration with { textScaleFactor = 1.5 });
var originalScaler = MediaQueryData.CreateFromView(view).textScaler;
host.Configure(host.Configuration with { textScaleFactor = 2, fontSizeScaler = size => size < 20 ? size * 2 : size * 1.5 });
var nonlinear = MediaQueryData.CreateFromView(view).textScaler;
Require(originalScaler.scale(24) == 36 && nonlinear.scale(10) == 20 && nonlinear.scale(24) == 36, "immutable native nonlinear scaler snapshot");
Require(nonlinear.clamp(maxScaleFactor: 1.2).scale(10) == 12 && TextScaler.noScaling.scale(10) == 10, "scaler clamp and noScaling");
Require(new SystemTextScaler(host.Configuration with { textScaleFactor = 1, fontSizeScaler = null }, dispatcher).Equals(TextScaler.noScaling),
    "pinned SystemTextScaler equality reaches the noScaling case");
using var secondHost = new FixtureHost();
using var second = dispatcher.RegisterView(2, secondHost.Capabilities());
Require(dispatcher.implicitView is null && dispatcher.textScaleFactor == 2, "second view preserves dispatcher settings");
Require(MediaQueryData.CreateFromView(second).textScaler.scale(10) == 10, "fromView uses its own environment");
var parent = new MediaQueryData(textScaleFactor: 1.25, platformBrightness: Brightness.dark,
    alwaysUse24HourFormat: false, lineHeightScaleFactorOverride: 1.2, letterSpacingOverride: 3,
    wordSpacingOverride: 4, paragraphSpacingOverride: 5);
var overridden = MediaQueryData.CreateFromView(view, parent);
Require(overridden.textScaler.scale(10) == 12.5 && overridden.platformBrightness == Brightness.dark &&
    !overridden.alwaysUse24HourFormat && overridden.wordSpacingOverride == 4, "parent platform overrides retained");
var removed = overridden.removePadding(removeTop: true).removeViewInsets(removeBottom: true).removeViewPadding(removeLeft: true);
Require(removed.lineHeightScaleFactorOverride == 1.2 && removed.letterSpacingOverride == 3 &&
    removed.paragraphSpacingOverride == 5 && removed.displayCornerRadii == overridden.displayCornerRadii, "copy and removal preserve typography and corners");

// Mounted host -> dispatcher -> binding -> fromView -> SafeArea chain, no manually injected MediaQueryData.
second.Dispose();
var binding = new WidgetsFlutterBinding(dispatcher);
var resolver = new LocalizationsResolver([new Locale("en", "US"), new Locale("ko", "KR")]);
Require(resolver.locale == new Locale("en", "US"), "locale resolver initializes from the host before first app build");
host.Configure(host.Configuration with { locales = [new Locale("ko", "KR")] });
Require(resolver.locale == new Locale("ko", "KR"), "locale resolver observes native locale changes");
resolver.didChangeLocales(null);
Require(resolver.locale == new Locale("en", "US"), "null locale list resolves supported fallback");
resolver.dispose();
host.Configure(host.Configuration with { locales = [new Locale("en", "US")] });
var owner = binding.buildOwner!;
var errors = new List<FlutterErrorDetails>();
FlutterError.onError = errors.Add;
var container = new RenderPositionedBox(alignment: Alignment.topLeft, textDirection: TextDirection.ltr);
var pipeline = new PipelineOwner();
pipeline.rootNode = container;
container.layout(BoxConstraints.CreateTight(new Size(360, 800)));
RenderObjectToWidgetElement<RenderBox>? root = null;
void Mount(Widget child)
{
    view.DispatchPlatformEvent(() => {
        root = new RenderObjectToWidgetAdapter<RenderBox>(container: container, child: child).attachToRenderTree(owner, root);
        Pump();
    });
}
void Pump()
{
    owner.buildScope(root!);
    container.markNeedsLayout();
    pipeline.flushLayout();
    owner.finalizeTree();
    if (errors.Count != 0) throw new Exception(string.Join("\n", errors.Select(e => e.exceptionThrown)));
}
host.Configure(new([new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.windows));
host.Publish(host.Metrics with { displayFeatures = [], displayCornerRadii = null, gestureSettings = new(),
    viewPadding = new(18, 36, 54, 72), viewInsets = default });
for (var flags = 0; flags < 16; flags++)
foreach (var minimum in new[] { 0d, 30 })
foreach (var rtl in new[] { false, true })
{
    var left = (flags & 1) != 0; var top = (flags & 2) != 0; var right = (flags & 4) != 0; var bottom = (flags & 8) != 0;
    MediaQueryData? consumed = null;
    BoxConstraints? bounds = null;
    var child = new Builder(builder: context => {
        consumed = MediaQuery.of(context);
        return new BoundsProbe(constraints => bounds = constraints);
    });
    Mount(MediaQuery.fromView(view: view, child: new Directionality(textDirection: rtl ? TextDirection.rtl : TextDirection.ltr,
        child: new SafeArea(left: left, top: top, right: right, bottom: bottom, minimum: EdgeInsets.CreateAll(minimum), child: child))));
    Require(consumed!.padding.left == (left ? 0 : 6) && consumed.padding.bottom == (bottom ? 0 : 24), "mounted SafeArea consumes enabled edges");
    Require(bounds!.maxWidth == 360 - Math.Max(left ? 6 : 0, minimum) - Math.Max(right ? 18 : 0, minimum) &&
        bounds.maxHeight == 800 - Math.Max(top ? 12 : 0, minimum) - Math.Max(bottom ? 24 : 0, minimum), "mounted SafeArea flag/minimum/RTL layout");
}
foreach (var maintain in new[] { false, true })
{
    BoxConstraints? bounds = null;
    MediaQueryData? consumed = null;
    Mount(MediaQuery.fromView(view: view, child: new Directionality(textDirection: TextDirection.ltr,
        child: new SafeArea(maintainBottomViewPadding: maintain, child: new SafeArea(child: new Builder(builder: context => {
            consumed = MediaQuery.of(context);
            return new BoundsProbe(constraints => bounds = constraints);
        }))))));
    foreach (var keyboard in new[] { 30d, 900, 0 })
    {
        view.DispatchPlatformEvent(() => host.Publish(host.Metrics with { viewInsets = new(0, 0, 0, keyboard) }));
        Pump();
        Require(consumed!.padding.left == 0 && consumed.padding.top == 0 && consumed.padding.right == 0 && consumed.padding.bottom == 0, $"nested SafeArea consumes padding only once: {consumed.padding}");
        Require(bounds!.maxHeight == 800 - 12 - (maintain ? 24 : Math.Max(0, 24 - keyboard / 3)), "mounted partial/full keyboard and restoration");
    }
}
foreach (var reverse in new[] { false, true })
{
    MediaQueryData? sliverData = null;
    Mount(MediaQuery.fromView(view: view, child: new Directionality(textDirection: TextDirection.rtl,
        child: new CustomScrollView(reverse: reverse, slivers: [new SliverSafeArea(sliver: new SliverToBoxAdapter(
            child: new Builder(builder: context => { sliverData = MediaQuery.of(context); return new SizedBox(height: 120); })))]))));
    Require(sliverData is not null && sliverData.padding.top == 0 && sliverData.padding.bottom == 0,
        "mounted SliverSafeArea consumes padding with reverse scroll and RTL");
}
var removal = new MediaQueryData(padding: EdgeInsets.CreateOnly(bottom: 14),
    viewPadding: EdgeInsets.CreateOnly(bottom: 24), viewInsets: EdgeInsets.CreateOnly(bottom: 10));
Require(removal.removePadding(removeBottom: true).viewPadding.bottom == 10, "removePadding adjusts viewPadding by consumed padding");
Require(removal.removeViewInsets(removeBottom: true).viewPadding.bottom == 14, "removeViewInsets adjusts viewPadding by occlusion");
Require(removal.removeViewPadding(removeBottom: true).padding.bottom == 0 &&
    removal.removeViewPadding(removeBottom: true).viewInsets.bottom == 10, "removeViewPadding retains keyboard");
Require(ReferenceEquals(removal.removePadding(), removal) && ReferenceEquals(removal.removeViewInsets(), removal), "remove no-op identity");
Require(removal.Equals(removal.copyWith(displayFeatures: [])), "Flutter pin list content equality preserved");
var subscreen = featureData.removeDisplayFeatures(new Rect(0, 0, 10, 800));
Require(subscreen.displayFeatures.Count == 0 && subscreen.size == featureData.size, "subscreen filters separating display features");
var widthBuilds = 0; var insetBuilds = 0;
var widthChild = new Builder(builder: context => { widthBuilds++; _ = MediaQuery.widthOf(context); return new SizedBox(); });
var insetChild = new Builder(builder: context => { insetBuilds++; _ = MediaQuery.viewInsetsOf(context); return new SizedBox(); });
Mount(MediaQuery.fromView(view: view, child: new Column(children: [widthChild, insetChild])));
var previousWidthBuilds = widthBuilds; var previousInsetBuilds = insetBuilds;
view.DispatchPlatformEvent(() => host.Publish(host.Metrics with { viewInsets = new(0, 0, 0, 900) })); Pump();
Require(widthBuilds == previousWidthBuilds && insetBuilds == previousInsetBuilds + 1, "mounted aspect rebuild selectivity");
view.DispatchPlatformEvent(() => host.Publish(host.Metrics)); Pump();
Require(insetBuilds == previousInsetBuilds + 1, "identical snapshot does not rebuild mounted dependents");
var timeFormat = false;
Mount(MediaQuery.fromView(view: view, child: new Builder(builder: context => { timeFormat = MediaQuery.alwaysUse24HourFormatOf(context); return new SizedBox(); })));
view.DispatchPlatformEvent(() => host.Configure(host.Configuration with { alwaysUse24HourFormat = true })); Pump();
Require(timeFormat, "non-text platform settings reach mounted fromView");
Mount(new SizedBox());
var observerCount = ((WidgetsBinding)binding)._observers.Count;
view.DispatchPlatformEvent(() => host.Configure(host.Configuration with { alwaysUse24HourFormat = false })); Pump();
Require(((WidgetsBinding)binding)._observers.Count == observerCount, "fromView observer disposal remains stable");
// Every public MediaQuery aspect is mounted independently. Expected rebuild sets
// come from the Flutter field contract, not from inspecting the C# switch.
var aspects = Enum.GetValues<Aspect>();
var counts = aspects.ToDictionary(a => a, _ => 0);
var entireBuilds = 0;
var stableFeatures = new List<DisplayFeature>();
var constructor = typeof(MediaQueryData).GetConstructors().Single();
MediaQueryData Data(string? field = null, object? value = null) => (MediaQueryData)constructor.Invoke(
    constructor.GetParameters().Select(p => p.Name == field ? value : p.Name == "size" ? new Size(100, 200) :
        p.Name == "displayFeatures" ? stableFeatures : Type.Missing).ToArray());
var readers = aspects.Select(aspect => {
    var name = aspect.ToString();
    var method = typeof(MediaQuery).GetMethod(name + "Of") ?? typeof(MediaQuery).GetMethod(name) ??
        typeof(MediaQuery).GetMethod("maybe" + char.ToUpperInvariant(name[0]) + name[1..] + "Of")!;
    return (Widget)new Builder(builder: context => { counts[aspect]++; method.Invoke(null, [context]); return new SizedBox(); });
}).ToList();
readers.Add(new Builder(builder: context => { entireBuilds++; _ = MediaQuery.of(context); return new SizedBox(); }));
var stableChildren = new Column(children: readers);
var changes = new List<(string field, object value, Aspect[] expected)> {
    ("size", new Size(200, 100), [Aspect.size, Aspect.width, Aspect.height, Aspect.orientation]),
    ("devicePixelRatio", 2d, [Aspect.devicePixelRatio]),
    ("textScaleFactor", 1.5, [Aspect.textScaleFactor, Aspect.textScaler]),
    ("textScaler", TextScaler.CreateLinear(2), [Aspect.textScaleFactor, Aspect.textScaler]),
    ("platformBrightness", Brightness.dark, [Aspect.platformBrightness]),
    ("padding", EdgeInsets.CreateAll(3), [Aspect.padding]),
    ("viewPadding", EdgeInsets.CreateAll(4), [Aspect.viewPadding]),
    ("viewInsets", EdgeInsets.CreateAll(5), [Aspect.viewInsets]),
    ("systemGestureInsets", EdgeInsets.CreateAll(6), [Aspect.systemGestureInsets]),
    ("navigationMode", NavigationMode.directional, [Aspect.navigationMode]),
    ("gestureSettings", new Doroti.Framework.Gestures.DeviceGestureSettings(9), [Aspect.gestureSettings]),
    ("displayFeatures", new List<DisplayFeature> { new(new Rect(50, 0, 51, 200), DisplayFeatureType.hinge, DisplayFeatureState.postureFlat) }, [Aspect.displayFeatures]),
    ("displayCornerRadii", new BorderRadius(topLeft: Radius.circular(8)), [Aspect.displayCornerRadii]),
};
foreach (var name in new[] { "alwaysUse24HourFormat", "accessibleNavigation", "invertColors", "highContrast", "onOffSwitchLabels",
    "disableAnimations", "reduceMotion", "boldText", "supportsAnnounce", "supportsShowingSystemContextMenu" })
    changes.Add((name, true, [Enum.Parse<Aspect>(name)]));
foreach (var name in new[] { "lineHeightScaleFactorOverride", "letterSpacingOverride", "wordSpacingOverride", "paragraphSpacingOverride" })
    changes.Add((name, 1.2, [Enum.Parse<Aspect>(name)]));
foreach (var change in changes)
{
    Mount(new MediaQuery(data: Data(), child: stableChildren));
    var previousCounts = new Dictionary<Aspect, int>(counts);
    var previousEntire = entireBuilds;
    Mount(new MediaQuery(data: Data(change.field, change.value), child: stableChildren));
    foreach (var aspect in aspects)
        Require(counts[aspect] - previousCounts[aspect] == (change.expected.Contains(aspect) ? 1 : 0),
            $"mounted aspect {aspect} after {change.field}");
    Require(entireBuilds == previousEntire + 1, "MediaQuery.of tracks every field");
}
Mount(new SizedBox());
Require(errors.Count == 0, string.Join("\n", errors.Select(e => e.exceptionThrown)));
FlutterError.onError = null;
Console.WriteLine($"MediaQuery / SafeArea PASS: {checks} checks (DPR, snapshots, multi-view, settings, mounted layout/aspects/lifecycle)");

sealed class FixtureHost : IViewHostCapability, IFrameHostCapability, IPlatformEnvironmentHostCapability, IPlatformMessageHostCapability
{
    public ViewMetrics Metrics { get; private set; } = new(new Size(1080, 2400), 3, default, default, default, AppLifecycleState.resumed, 1, 7);
    public PlatformConfiguration Configuration { get; private set; } = new([new Locale("en", "US")], Brightness.light, false, false);
    public DorotiViewEpoch ViewEpoch => new(1, 1, Metrics.generation, 360, 800, 1080, 2400, 3, 3, 0);
    public event System.Action<ViewMetrics>? MetricsChanged;
    public event System.Action<PlatformConfiguration>? ConfigurationChanged;
    public event System.Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
    public event Action? CloseRequested { add { } remove { } }
    public event Action? Closed { add { } remove { } }
    public DorotiViewCapabilities Capabilities() => new DorotiViewCapabilities("media-query-fixture")
        .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, this)
        .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, this)
        .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, this)
        .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, this);
    public void Publish(ViewMetrics metrics) { Metrics = metrics with { generation = Metrics.generation + 1 }; MetricsChanged?.Invoke(Metrics); }
    public void Deliver(ViewMetrics metrics) => MetricsChanged?.Invoke(metrics);
    public void Configure(PlatformConfiguration configuration) { Configuration = configuration; ConfigurationChanged?.Invoke(configuration); }
    public void Show() { } public void Resize(Size size) { } public void Close() { } public void Dispose() { }
    public void ScheduleFrame(System.Action<TimeSpan> callback) { }
    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default) => ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);
    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
}

sealed class BoundsProbe(System.Action<BoxConstraints> report) : LeafRenderObjectWidget
{
    public override RenderObject createRenderObject(BuildContext context) => new BoundsRenderBox(report);
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    { ((BoundsRenderBox)renderObject).Report = report; renderObject.markNeedsLayout(); }
}
sealed class BoundsRenderBox(System.Action<BoxConstraints> report) : RenderBox
{
    internal System.Action<BoxConstraints> Report = report;
    public override void performLayout()
    {
        var value = (BoxConstraints)constraints;
        Report(value);
        size = value.constrain(new Size(360, 800));
    }
}
