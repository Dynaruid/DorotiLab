using Doroti.Framework.Foundation;
using Doroti.Framework.Cupertino;
using PointerDownEvent = Doroti.Framework.Gestures.PointerDownEvent;
using PointerUpEvent = Doroti.Framework.Gestures.PointerUpEvent;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Runtime;
using Doroti.Ui;
using SkiaSharp;
using Path = System.IO.Path;

using var dispatcher = new PlatformDispatcher();
using var scope = dispatcher.EnterScope();
var portrait = args.Contains("--portrait", StringComparer.Ordinal);
var variableBlur = args.Contains("--variable-blur", StringComparer.Ordinal);
using var gpu = variableBlur ? new VulkanFixture() : null;
var host = new BuildHost(portrait ? new Size(400, 800) : new Size(720, 840));
using var renderer = new SkiaSceneRenderer(
    1,
    host,
    null,
    null,
    "cupertino-sample",
    variableBlur ? DorotiSkiaRuntimeEffects.WindowsVulkanBackend : "skia-raster",
    "cupertino-sample",
    enablePictureRasterCache: false
);
using (var stream = typeof(DorotiSampleApp2.App).Assembly.GetManifestResourceStream("CupertinoSample.icons.ttf")!)
{
    using var bytes = new MemoryStream();
    stream.CopyTo(bytes);
    renderer.RegisterFontAsync(bytes.ToArray(), "packages/cupertino_icons/CupertinoIcons").GetAwaiter().GetResult();
}
using var view = dispatcher.RegisterView(
    1,
    new DorotiViewCapabilities("cupertino-sample-build")
        .Register(DorotiCapabilityIds.ViewLifecycleMetrics, host)
        .Register(DorotiCapabilityIds.ViewFrameDispatch, host)
        .Register(DorotiCapabilityIds.PlatformMessaging, host)
        .Register(DorotiCapabilityIds.PlatformServices, host)
        .Register(DorotiCapabilityIds.GraphicsText, renderer)
        .Register(DorotiCapabilityIds.GraphicsScene, renderer)
        .Register(DorotiCapabilityIds.GraphicsFont, renderer)
        .Register(DorotiCapabilityIds.TextInput, host)
        .Register(DorotiCapabilityIds.InputEvents, host)
        .Register(DorotiCapabilityIds.PlatformEnvironment, host)
);
using var environment = view.EnterPlatformEnvironmentScope();
using var binding = new WidgetsFlutterBinding(dispatcher);
var errors = new List<FlutterErrorDetails>();
FlutterError.onError = errors.Add;
var root = (Widget)typeof(DorotiSampleApp2.App).Assembly.CreateInstance("DorotiSampleApp2.CupertinoSample", true)!;
binding.attachRootWidget(binding.wrapWithDefaultView(root));
Pump();
if (variableBlur)
{
    Tab(3);
    var listElement = Elements(binding.rootElement!).Single(element => element.widget is ListView);
    var controller = ((ListView)listElement.widget).controller!;
    var box = (RenderBox)listElement.findRenderObject()!;
    // Start within the overlay: IgnorePointer must allow the list to receive pan/zoom.
    var position = box.localToGlobal(new Offset(box.size.width / 2, 80));
    binding.handlePointerEvent(new Doroti.Framework.Gestures.PointerPanZoomStartEvent(
        viewId: 1, pointer: 99, device: 99, position: position));
    using var before = CaptureVariableBlur();
    var lastPixels = before.Pixels;
    var lastOffset = controller.offset;
    for (var update = 1; update <= 8; update++)
    {
        binding.handlePointerEvent(new Doroti.Framework.Gestures.PointerPanZoomUpdateEvent(
            viewId: 1, pointer: 99, device: 99, position: position,
            timeStamp: new Duration(microseconds: update * 16_000),
            pan: new Offset(0, -update * 32), panDelta: new Offset(0, -32)));
        host.Pump(16);
        // The first update can establish drag slop; subsequent updates must scroll
        // BEFORE PanZoomEnd, including through the GPU backdrop-filter scene path.
        if (update > 1)
            Check(controller.offset > lastOffset, $"pan/zoom update {update} scrolls before gesture end");
        lastOffset = controller.offset;
        if (update is 4 or 8)
        {
            using var during = CaptureVariableBlur();
            var changed = lastPixels.Zip(during.Pixels).Count(pair => pair.First != pair.Second);
            Check(changed > 1000, $"GPU pixels change during active pan/zoom ({changed})");
            lastPixels = during.Pixels;
            var directory = Path.Combine(AppContext.BaseDirectory, "snapshots");
            Directory.CreateDirectory(directory);
            using var data = during.Encode(SKEncodedImageFormat.Png, 100);
            File.WriteAllBytes(Path.Combine(directory,
                $"variable-blur-during-{update}-{(portrait ? "portrait" : "landscape")}.png"), data.ToArray());
        }
    }
    binding.handlePointerEvent(new Doroti.Framework.Gestures.PointerPanZoomEndEvent(
        viewId: 1, pointer: 99, device: 99, position: position,
        timeStamp: new Duration(microseconds: 144_000)));
    Pump();
    Console.WriteLine("PASS Variable Blur synthetic trackpad updates and Vulkan pixels before gesture end");
    return;
}
// Advance through more than a full indicator cycle, including negative Dart-modulo inputs.
for (var frame = 0; frame < 24; frame++) host.Pump();
Pump();
Press("Tap me");
Check(Find<Text>().Any(text => text.data == "Button taps: 1"), "counter updates");
Find<CupertinoSwitch>().Single().onChanged!(false);
Find<CupertinoSlider>().Single().onChanged!(0.75);
Pump();
Snapshot("components");
var switchElement = Elements(binding.rootElement!).Single(element => element.widget is CupertinoSwitch);
Check(Elements(switchElement).Select(element => element.widget).OfType<CustomPaint>().Any(paint => paint.painter is not null),
    "switch has a connected painter");
Find<CupertinoButton>().Single(button => (button.child as Text)?.data == "Show dialog").onPressed!();
host.Pump(0);
host.Pump(80);
CheckDialogBackdrop("opening");
Snapshot("dialog-opening");
Pump();
Check(Find<CupertinoAlertDialog>().Any(), "dialog opens");
var messageBox = (RenderBox)Elements(binding.rootElement!).Single(element =>
    element.widget is Text text && text.data == "A Cupertino dialog in Doroti.").findRenderObject()!;
var actionBox = (RenderBox)Elements(binding.rootElement!).Single(element => element.widget is CupertinoDialogAction).findRenderObject()!;
Check(messageBox.localToGlobal(new Offset(0, messageBox.size.height)).dy <= actionBox.localToGlobal(Offset.zero).dy,
    "dialog message fits above its action");
Snapshot("dialog");
Find<CupertinoDialogAction>().Single().onPressed!();
host.Pump(0);
host.Pump(80);
CheckDialogBackdrop("closing");
Snapshot("dialog-closing");
Pump();
Tab(0);
Tab(1);
Tab(1);
var field = Find<CupertinoTextField>().Single();
field.controller!.text = "Doroti";
field.onChanged!("Doroti");
Pump();
Check(Find<Text>().Any(text => text.data == "Hello, Doroti!"), "profile greeting updates");
Snapshot("profile");
Tab(2);
Find<CupertinoListTile>().Single(tile => (tile.title as Text)?.data == "Dark").onTap!();
Pump();
Check(Find<CupertinoApp>().Single().theme!.brightness == Brightness.dark, "dark theme selected");
Snapshot("settings-dark");
Tab(0);
Check(!Find<CupertinoSwitch>().Single().value && Find<CupertinoSlider>().Single().value == 0.75,
    "controls survive tab and theme changes");
Snapshot("components-dark");
Tab(1);
Check(Find<CupertinoTextField>().Single().controller!.text == "Doroti", "profile survives tab changes");
Console.WriteLine("PASS Cupertino sample layout, raster, state, dialog, theme and indicator animation");

void Press(string text)
{
    Find<CupertinoButton>().Single(button => (button.child as Text)?.data == text).onPressed!();
    Pump();
}
void Tab(long index)
{
    var tabElement = Elements(binding.rootElement!).Single(element => element.widget is CupertinoTabBar);
    var tabBar = (CupertinoTabBar)tabElement.widget;
    var box = (RenderBox)tabElement.findRenderObject()!;
    var position = box.localToGlobal(new Offset(box.size.width * (index + 0.5) / tabBar.items.Count, box.size.height / 2));
    var kind = portrait ? PointerDeviceKind.touch : PointerDeviceKind.mouse;
    binding.handlePointerEvent(new PointerDownEvent(viewId: 1, pointer: 1, kind: kind, position: position));
    binding.handlePointerEvent(new PointerUpEvent(viewId: 1, pointer: 1, kind: kind, position: position));
    Pump();
    Check(Find<CupertinoTabBar>().Single().currentIndex == index, $"pointer click selects tab {index}");
    var activePages = Elements(binding.rootElement!).Where(element =>
        element.widget is Offstage offstage && !offstage.offstage).SelectMany(Elements);
    var expected = new[] { "Components", "Profile", "Settings", "Variable Blur" }[index];
    Check(activePages.Any(element => element.widget is CupertinoNavigationBar bar && (bar.middle as Text)?.data == expected),
        $"{expected} page is onstage after pointer click");
}
void Pump()
{
    for (var i = 0; i < 3; i++)
    {
        binding.scheduleForcedFrame();
        host.Pump();
    }
    if (errors.Count > 0)
        throw new InvalidOperationException(
            string.Join("\n", errors.Select(e => e.exceptionThrown))
        );
    Check(!Find<ErrorWidget>().Any(), "no ErrorWidget");
}
IEnumerable<T> Find<T>()
    where T : Widget => Elements(binding.rootElement!).Select(e => e.widget).OfType<T>();
IEnumerable<Element> Elements(Element element)
{
    yield return element;
    var children = new List<Element>();
    element.visitChildren(children.Add);
    foreach (var child in children)
    foreach (var descendant in Elements(child))
        yield return descendant;
}
void Snapshot(string name)
{
    var size = host.Metrics.physicalSize;
    using var surface = gpu is null
        ? SKSurface.Create(new SKImageInfo((int)size.width, (int)size.height))
        : gpu.CreateSurface(new SKImageInfo((int)size.width, (int)size.height));
    Check(
        renderer.Paint(surface, (int)size.width, (int)size.height) is not null,
        "Skia paints Cupertino sample"
    );
    var directory = Path.Combine(AppContext.BaseDirectory, "snapshots");
    Directory.CreateDirectory(directory);
    using var image = surface.Snapshot();
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
    File.WriteAllBytes(
        Path.Combine(directory, name + (portrait ? "-portrait" : "-landscape") + ".png"),
        data.ToArray()
    );
}
SKBitmap CaptureVariableBlur()
{
    var size = host.Metrics.physicalSize;
    using var surface = gpu!.CreateSurface(new SKImageInfo((int)size.width, (int)size.height));
    Check(renderer.Paint(surface, (int)size.width, (int)size.height) is not null,
        "Vulkan paints Variable Blur scene");
    using var image = surface.Snapshot();
    return SKBitmap.FromImage(image);
}
void CheckDialogBackdrop(string phase)
{
    var dialog = Elements(binding.rootElement!).Single(element => element.widget is CupertinoAlertDialog);
    var backdrop = (RenderBackdropFilter)Elements(dialog).Single(element => element.widget is BackdropFilter).findRenderObject()!;
    var progress = ModalRoute<object>.of<object>(dialog)!.animation!.value;
    Check(progress > 0 && progress < 1, $"{phase}: checking an intermediate fade frame ({progress:F3})");
    using var filtered = Capture();
    backdrop.enabled = false;
    binding.rootPipelineOwner.flushPaint();
    using var unfiltered = Capture();
    var changed = filtered.Pixels.Zip(unfiltered.Pixels).Count(pair =>
        Math.Abs(pair.First.Red - pair.Second.Red) + Math.Abs(pair.First.Green - pair.Second.Green)
        + Math.Abs(pair.First.Blue - pair.Second.Blue) > 6);
    Check(changed > 100, $"{phase}: backdrop affects {changed} pixels before fade completes");
    backdrop.enabled = true;
    binding.rootPipelineOwner.flushPaint();

    SKBitmap Capture()
    {
        var size = host.Metrics.physicalSize;
        using var surface = SKSurface.Create(new SKImageInfo((int)size.width, (int)size.height));
        using var scene = binding.renderViews.Single().layer!.buildScene(new SceneBuilder(1));
        renderer.DrawPlatformRasterSegment(surface.Canvas, scene.Commands, (int)size.width, (int)size.height);
        using var image = surface.Snapshot();
        return SKBitmap.FromImage(image);
    }
}
static void Check(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
    Console.WriteLine("PASS " + message);
}

// Deterministic host services; layout and raster use the production framework and Skia.
// Native OS input, window presentation, and keyboard UI are outside this regression.
sealed class BuildHost(Size size)
    : IViewHostCapability,
        IFrameHostCapability,
        IPlatformMessageHostCapability,
        IPlatformEnvironmentHostCapability,
        IPlatformServicesHostCapability,
        ISkiaSceneRendererHost,
        ITextInputHostCapability,
        IInputHostCapability,
        IViewFocusRequestCapability
{
    public ViewMetrics Metrics { get; } =
        new(
            size,
            1,
            ViewPadding.zero,
            ViewPadding.zero,
            ViewPadding.zero,
            AppLifecycleState.resumed,
            1,
            1
        );
    public DorotiViewEpoch ViewEpoch =>
        new(1, 1, 1, size.width, size.height, (int)size.width, (int)size.height, 1, 1, 1);
    public PlatformConfiguration Configuration { get; } =
        new([new Locale("en", "US")], Brightness.light, true, false, HostOperatingSystem.windows);
    public event Action<ViewMetrics>? MetricsChanged
    {
        add { }
        remove { }
    }
    public event Action<AppLifecycleState>? LifecycleChanged
    {
        add { }
        remove { }
    }
    public event Action? CloseRequested
    {
        add { }
        remove { }
    }
    public event Action? Closed
    {
        add { }
        remove { }
    }
    public event Action<PlatformConfiguration>? ConfigurationChanged
    {
        add { }
        remove { }
    }

    public void Show() { }

    public void Resize(Size size) { }

    public void Close() { }

    public void Dispose() { }

    private Action<TimeSpan>? _frame;
    private long _milliseconds;

    public void ScheduleFrame(Action<TimeSpan> callback) => _frame = callback;

    public void Pump(int milliseconds = 100)
    {
        var callback = _frame;
        _frame = null;
        callback?.Invoke(TimeSpan.FromMilliseconds(_milliseconds += milliseconds));
    }

    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(
        string channel,
        ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken = default
    ) => ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);

    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }

    public ValueTask<string?> GetClipboardTextAsync(
        CancellationToken cancellationToken = default
    ) => ValueTask.FromResult<string?>(null);

    public ValueTask SetClipboardTextAsync(
        string text,
        CancellationToken cancellationToken = default
    ) => ValueTask.CompletedTask;

    public void SetCursor(DorotiMouseCursorKind cursor) { }

    public long InputSequence => 0;
    public long SurfaceGeneration => 1;
    public DorotiResizeEpoch ResizeTarget =>
        new(1, size.width, size.height, (int)size.width, (int)size.height, 1, 1);
    public event Action<int, SemanticsAction, object?>? SemanticsAction
    {
        add { }
        remove { }
    }
    public event Action<long, TimeSpan>? InputReceived
    {
        add { }
        remove { }
    }

    public void UpdateSemantics(SemanticsUpdate update) { }

    public void ClearSemantics() { }

    public void RequestInvalidate() { }

    public event Action<DorotiTextEditingState>? EditingStateChanged
    {
        add { }
        remove { }
    }
    public event Action<DorotiTextInputAction>? ActionPerformed
    {
        add { }
        remove { }
    }

    public void SetClient(
        DorotiTextInputConfiguration configuration,
        DorotiTextEditingState initialState
    ) { }

    public void UpdateState(DorotiTextEditingState state) { }

    public void SetCaretRect(Rect logicalRect) { }

    public void ClearClient() { }

    public event Action<PointerDataPacket>? PointerData
    {
        add { }
        remove { }
    }
    public event Action<KeyData>? KeyData
    {
        add { }
        remove { }
    }
    public event Action<RawFocusData>? FocusData
    {
        add { }
        remove { }
    }

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction) { }
}
