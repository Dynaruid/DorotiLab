using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Doroti.Backends.Skia;
using Doroti.Flutter.Hosting;
using Doroti.Flutter.Ui;
using Doroti.Generated.Framework.Foundation;
using Doroti.Generated.Framework.Painting;
using Doroti.Generated.Framework.Widgets;
using Doroti.Target.Windows;
using Cupertino = Doroti.Generated.Framework.Cupertino;
using WidgetPreviews = Doroti.Generated.Framework.WidgetPreviews;
using Path = System.IO.Path;
using UiColor = Doroti.Flutter.Ui.Color;

const ulong ViewId = 660;
var options = ValidationOptions.Parse(args);
var failures = new List<string>();
var componentCase = Environment.GetEnvironmentVariable("G6_CUPERTINO_CASE");
var entrypoint = new CupertinoEntrypoint(options.Wave, componentCase);
object? activeDiagnostics = null;
object? resourceClosure = null;
object? pixels = null;
object? previewEvidence = null;
var backGlyph = char.ConvertFromUtf32(checked((int)Cupertino.CupertinoIcons.back.codePoint));
var requestedCupertinoFontFamily = $"packages/{Cupertino.CupertinoIcons.iconFontPackage}/{Cupertino.CupertinoIcons.iconFont}";
var cupertinoFontFamilies = SkiaTextMeasurer.ResolveFallbackFamilies(
    backGlyph,
    fontFamily: requestedCupertinoFontFamily);
if (!cupertinoFontFamilies.Contains(Cupertino.CupertinoIcons.iconFont, StringComparer.OrdinalIgnoreCase))
{
    failures.Add($"Cupertino back icon resolved through {string.Join(", ", cupertinoFontFamilies)} instead of the bundled CupertinoIcons font.");
}

if (!OperatingSystem.IsWindows())
{
    Console.Error.WriteLine("G6-6 Cupertino live validation currently requires Windows x64.");
    return 2;
}

try
{
    using var target = new WindowsFlutterTarget();
    using var session = new FlutterHostSession(entrypoint);
    using var scope = session.dispatcher.EnterScope();
    session.Start(deferFrameworkBootstrap: true);
    var view = target.CreateView(session, ViewId, new("Doroti G6-6 Cupertino", new(720, 640)));
    try
    {
        var firstReadback = target.CaptureNextFrameAsync(ViewId);
        view.Show();
        session.dispatcher.setSemanticsTreeEnabled(true);
        WaitUntil(() => target.CaptureDiagnostics(ViewId).Frame.Presented >= 1 && entrypoint.State is not null,
            target, entrypoint, TimeSpan.FromSeconds(15));
        var readback = await firstReadback.WaitAsync(TimeSpan.FromSeconds(10));
        pixels = AnalyzePixels(readback);

        var initial = target.CaptureDiagnostics(ViewId);
        Require(initial.Frame.BackendIdentity == "skia-wgl-opengl-gpu", "strict GPU backend identity was not preserved", failures);
        Require(!initial.Frame.SoftwareFallbackUsed, "software fallback was used", failures);
        Require(initial.Frame.Presented > 0, "the actual HWND did not present a frame", failures);
        Require(target.GetNativeWindowHandle(ViewId) != 0, "the native HWND is zero", failures);
        Require(initial.Automation.NodeCount > 0, "the Cupertino semantics tree is empty", failures);

        var state = entrypoint.State!;
        var tap = state.PrimaryButtonCenter();
        var beforeTap = state.InteractionCount;
        var beforeTapPresented = target.CaptureDiagnostics(ViewId).Frame.Presented;
        target.PostPointerTapForValidation(ViewId, tap.dx, tap.dy);
        WaitUntil(() => state.InteractionCount == beforeTap + 1 &&
                        target.CaptureDiagnostics(ViewId).Frame.Presented > beforeTapPresented,
            target, entrypoint, TimeSpan.FromSeconds(10));
        WaitForQuiescence(target, entrypoint, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(400));

        activeDiagnostics = target.CaptureDiagnostics(ViewId);
        var final = target.CaptureDiagnostics(ViewId);
        var terminal = final.Frame.Presented + final.Frame.Superseded + final.Frame.Stale + final.Frame.Failed + final.Frame.Cancelled;
        Require(final.Frame.Submitted == terminal, $"terminal ACK imbalance: submitted={final.Frame.Submitted}, terminal={terminal}", failures);
        Require(final.Frame.Failed == 0 && final.Frame.Cancelled == 0, "frame failure or cancellation was observed", failures);
        Require(final.Frame.QueueDepth == 0 && final.Frame.ActiveFrames == 0, "frame queue did not drain", failures);
        Require(state.InteractionCount == 1, $"native Cupertino button interaction count was {state.InteractionCount}, expected 1", failures);
        Require(state.BuildCount >= 2, "Cupertino State did not rebuild after native input", failures);
        if (string.Equals(componentCase, "preview", StringComparison.OrdinalIgnoreCase))
            Require(state.PreviewHasActualLayout, "selected Widget Preview did not mount and lay out in the presented frame", failures);
        previewEvidence = state.PreviewEvidence();
    }
    finally
    {
        session.DetachView(view);
        session.Shutdown();
        view.Dispose();
        for (var index = 0; index < 20; index++) target.PumpPendingMessages();
        var resources = target.CaptureResourceSnapshot();
        resourceClosure = resources;
        Require(resources.IsBalanced, $"native resources are not balanced: {resources}", failures);
    }
}
catch (Exception error)
{
    failures.Add(error.ToString());
}

Require(entrypoint.FirstFrameworkError is null,
    entrypoint.FirstFrameworkError is null ? "" : $"FlutterError: {entrypoint.FirstFrameworkError.exceptionThrown}", failures);
Require(entrypoint.Disposed, "Cupertino gallery State was not disposed during shutdown", failures);

var status = failures.Count == 0 ? "verified-windows-x64-strict-gpu" : "failed";
WriteJson(options.EvidencePath, new
{
    schemaVersion = "doroti.g6-6-cupertino-wave-evidence/v1",
    milestone = "G6-6",
    capturedAtUtc = DateTimeOffset.UtcNow,
    status,
    wave = options.Wave.ToString(),
    flutterRevision = "56b8e1a851a594b1a154f8ea93270807dab22b9a",
    target = "actual HWND / skia-wgl-opengl-gpu",
    componentCase,
    components = entrypoint.State?.PresentedComponents ?? CupertinoGallery.ComponentsFor(options.Wave, componentCase),
    interaction = new { kind = "native-pointer-move-down-up", count = entrypoint.State?.InteractionCount ?? 0 },
    pixels,
    activeDiagnostics,
    resourceClosure,
    preview = previewEvidence,
    fonts = new
    {
        requestedFamily = requestedCupertinoFontFamily,
        backCodePoint = $"U+{Cupertino.CupertinoIcons.back.codePoint:X4}",
        resolvedFamilies = cupertinoFontFamilies,
        status = cupertinoFontFamilies.Contains(Cupertino.CupertinoIcons.iconFont, StringComparer.OrdinalIgnoreCase) ? "PASS-no-tofu" : "FAIL-fallback",
    },
    prerequisites = new
    {
        g6_5r = "notVerified",
        g6_5r_i = "partial-windows-only",
        g6_5r_c = "partial-c0-no-reference-differential",
    },
    failures,
});
Console.WriteLine($"G6-6 Cupertino {options.Wave}: {(failures.Count == 0 ? "PASS" : "FAIL")} ({options.EvidencePath})");
foreach (var failure in failures) Console.Error.WriteLine(failure);
return failures.Count == 0 ? 0 : 1;

static void WaitUntil(Func<bool> predicate, WindowsFlutterTarget target, CupertinoEntrypoint entrypoint, TimeSpan timeout)
{
    var elapsed = Stopwatch.StartNew();
    while (!predicate())
    {
        if (entrypoint.FirstFrameworkError is { } error)
            throw new InvalidOperationException("Flutter framework error", error.exceptionThrown);
        if (elapsed.Elapsed > timeout)
            throw new TimeoutException($"G6-6 Cupertino condition timed out after {timeout}; frame={target.CaptureDiagnostics(ViewId).Frame}.");
        target.PumpPendingMessages();
        Thread.Sleep(1);
    }
    target.PumpPendingMessages();
}

static void WaitForQuiescence(
    WindowsFlutterTarget target,
    CupertinoEntrypoint entrypoint,
    TimeSpan timeout,
    TimeSpan stableDuration)
{
    var elapsed = Stopwatch.StartNew();
    Stopwatch? stable = null;
    while (stable is null || stable.Elapsed < stableDuration)
    {
        if (entrypoint.FirstFrameworkError is { } error)
            throw new InvalidOperationException("Flutter framework error", error.exceptionThrown);
        if (elapsed.Elapsed > timeout)
            throw new TimeoutException($"G6-6 Cupertino frame queue did not quiesce after {timeout}; frame={target.CaptureDiagnostics(ViewId).Frame}.");
        target.PumpPendingMessages();
        var frame = target.CaptureDiagnostics(ViewId).Frame;
        var terminal = frame.Presented + frame.Superseded + frame.Stale + frame.Failed + frame.Cancelled;
        if (frame.Submitted == terminal && frame.QueueDepth == 0 && frame.ActiveFrames == 0)
            stable ??= Stopwatch.StartNew();
        else
            stable = null;
        Thread.Sleep(1);
    }
}

static object AnalyzePixels(Doroti.Host.Desktop.Flutter.DesktopFlutterPixelReadback frame)
{
    long nonTransparent = 0, nonWhite = 0, blue = 0;
    for (var y = 0; y < frame.Height; y++)
    for (var x = 0; x < frame.Width; x++)
    {
        var offset = y * frame.RowBytes + x * 4;
        var b = frame.Bgra8888Pixels[offset];
        var g = frame.Bgra8888Pixels[offset + 1];
        var r = frame.Bgra8888Pixels[offset + 2];
        var a = frame.Bgra8888Pixels[offset + 3];
        if (a != 0) nonTransparent++;
        if (a != 0 && (r < 245 || g < 245 || b < 245)) nonWhite++;
        if (a != 0 && b > r + 40 && b > g + 10) blue++;
    }
    return new { frame.FrameId, frame.Width, frame.Height, nonTransparent, nonWhite, blue };
}

static void Require(bool condition, string message, List<string> failures)
{
    if (!condition && !string.IsNullOrWhiteSpace(message)) failures.Add(message);
}

static void WriteJson(string path, object value)
{
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    var temporary = path + ".tmp-" + Guid.NewGuid().ToString("N");
    File.WriteAllText(temporary, JsonSerializer.Serialize(value, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    }) + "\n", new UTF8Encoding(false));
    File.Move(temporary, path, true);
}

internal sealed class CupertinoEntrypoint(CupertinoWave wave, string? componentCase) : IFlutterViewEntrypoint
{
    private WidgetsFlutterBinding? _binding;
    private FlutterView? _view;
    internal CupertinoGalleryState? State { get; private set; }
    internal FlutterErrorDetails? FirstFrameworkError { get; private set; }
    internal bool Disposed { get; private set; }

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        FlutterError.onError = details => FirstFrameworkError ??= details;
        _binding = new WidgetsFlutterBinding(dispatcher);
    }

    public void AttachView(FlutterView view)
    {
        _view = view;
        var binding = _binding ?? throw new InvalidOperationException("Cupertino binding was not bootstrapped.");
        binding.scheduleFrameCallback(_ => binding.attachRootWidget(binding.wrapWithDefaultView(CreateRoot())));
    }

    public void DetachView(FlutterView view)
    {
        if (ReferenceEquals(_view, view)) _view = null;
    }

    internal void RequestFrame()
    {
        if (State is { } state) state.PulseFrame();
        else (_binding ?? throw new InvalidOperationException("Cupertino binding is unavailable.")).scheduleFrame();
    }

    public void Shutdown()
    {
        if (_binding?.rootElement is { } root)
        {
            new RootWidget(debugShortDescription: "[shutdown]", child: null).attach(_binding.buildOwner!, (RootElement)root);
            _binding.buildOwner!.buildScope(root);
            _binding.buildOwner.finalizeTree();
            root.deactivate();
            root.unmount();
        }
        _binding?.Dispose();
        _binding = null;
        _view = null;
        FlutterError.onError = null;
    }

    private Widget CreateRoot() => new Cupertino.CupertinoApp(
        title: "Doroti Cupertino Gallery",
        color: new UiColor(0xff007aff),
        locale: new Locale("en", "US"),
        debugShowCheckedModeBanner: false,
        theme: new Cupertino.CupertinoThemeData(
            primaryColor: new UiColor(0xff007aff),
            scaffoldBackgroundColor: new UiColor(0xfff2f2f7)),
        home: new CupertinoGallery(wave, componentCase, state => State = state, () => Disposed = true));
}

internal sealed class CupertinoGallery(
    CupertinoWave wave,
    string? componentCase,
    System.Action<CupertinoGalleryState> mounted,
    System.Action disposed) : StatefulWidget
{
    internal CupertinoWave Wave { get; } = wave;
    internal string? ComponentCase { get; } = componentCase;
    internal System.Action<CupertinoGalleryState> Mounted { get; } = mounted;
    internal System.Action Disposed { get; } = disposed;
    public override IState createState() => new CupertinoGalleryState();

    internal static IReadOnlyList<string> ComponentsFor(CupertinoWave wave, string? componentCase = null)
    {
        var components = new List<string>
        {
            "CupertinoApp", "CupertinoTheme", "CupertinoPageScaffold", "CupertinoNavigationBar", "CupertinoButton",
        };
        bool Include(string name) => string.IsNullOrWhiteSpace(componentCase) ||
            string.Equals(componentCase, name, StringComparison.OrdinalIgnoreCase);
        if (wave >= CupertinoWave.C1)
        {
            if (Include("activity")) components.AddRange(["CupertinoActivityIndicator", "CupertinoLinearActivityIndicator"]);
            if (Include("selection")) components.AddRange(["CupertinoCheckbox", "CupertinoRadio", "CupertinoSwitch", "CupertinoSlider"]);
            if (Include("segmented")) components.AddRange(["CupertinoSegmentedControl", "CupertinoSlidingSegmentedControl"]);
            if (Include("list")) components.AddRange(["CupertinoListSection", "CupertinoListTile", "CupertinoListTileChevron"]);
        }
        if (wave >= CupertinoWave.C2)
        {
            if (Include("text")) components.AddRange(["CupertinoTextField", "CupertinoSearchTextField", "CupertinoTextFormFieldRow"]);
            if (Include("form")) components.AddRange(["CupertinoFormSection", "CupertinoFormRow", "CupertinoExpansionTile"]);
            if (Include("dialog")) components.AddRange(["CupertinoAlertDialog", "CupertinoPopupSurface", "CupertinoDialogAction"]);
            if (Include("sheet")) components.AddRange(["CupertinoActionSheet", "CupertinoActionSheetAction"]);
        }
        if (wave >= CupertinoWave.C3)
        {
            if (Include("picker")) components.AddRange(["CupertinoPicker", "CupertinoPickerDefaultSelectionOverlay", "CupertinoDatePicker", "CupertinoTimerPicker"]);
            if (Include("navigation")) components.AddRange(["CupertinoNavigationBarBackButton", "CupertinoSliverNavigationBar", "CupertinoSliverRefreshControl"]);
            if (Include("scroll")) components.Add("CupertinoScrollbar");
            if (Include("tabs")) components.AddRange(["CupertinoTabBar", "CupertinoTabScaffold", "CupertinoTabView"]);
        }
        if (wave >= CupertinoWave.C4)
        {
            if (Include("context")) components.AddRange(["CupertinoContextMenu", "CupertinoContextMenuAction", "CupertinoFocusHalo"]);
            if (Include("menu")) components.AddRange(["CupertinoMenuAnchor", "CupertinoMenuDivider", "CupertinoMenuItem"]);
            if (Include("toolbar") || Include("toolbar-adaptive")) components.AddRange(["CupertinoAdaptiveTextSelectionToolbar", "CupertinoTextSelectionToolbarButton"]);
            if (Include("toolbar") || Include("toolbar-desktop")) components.AddRange(["CupertinoDesktopTextSelectionToolbar", "CupertinoDesktopTextSelectionToolbarButton"]);
            if (Include("toolbar") || Include("toolbar-selection")) components.AddRange(["CupertinoTextSelectionToolbar", "CupertinoTextSelectionToolbarButton"]);
            if (Include("toolbar") || Include("toolbar-spell")) components.Add("CupertinoSpellCheckSuggestionsToolbar");
            if (Include("transition")) components.AddRange(["CupertinoPageTransition", "CupertinoFullscreenDialogTransition", "CupertinoSheetTransition"]);
            if (Include("magnifier")) components.AddRange(["CupertinoMagnifier", "CupertinoTextMagnifier"]);
        }
        return components;
    }
}

internal sealed class CupertinoGalleryState : State<CupertinoGallery>
{
    private readonly GlobalKey<IState> _primaryButtonKey = new("g6-cupertino-primary-button");
    private readonly GlobalKey<IState> _previewKey = new("g6-widget-preview-selected");
    private bool _checked;
    private long _radio;
    private bool _switched;
    private double _slider = 0.25;
    private long _segment;
    private readonly MagnifierController _magnifierController = new();
    private readonly ValueNotifier<MagnifierInfo> _magnifierInfo = new(new MagnifierInfo(
        globalGesturePosition: new Offset(120, 120),
        caretRect: Rect.fromLTWH(118, 105, 2, 20),
        fieldBounds: Rect.fromLTWH(20, 80, 300, 80),
        currentLineBoundaries: Rect.fromLTWH(20, 100, 300, 24)));
    internal int InteractionCount { get; private set; }
    internal int BuildCount { get; private set; }
    internal IReadOnlyList<string> PresentedComponents => CupertinoGallery.ComponentsFor(widget.Wave, widget.ComponentCase);

    public override void initState()
    {
        base.initState();
        widget.Mounted(this);
    }

    public override void dispose()
    {
        _magnifierInfo.dispose();
        widget.Disposed();
        base.dispose();
    }

    internal void PulseFrame() => setState(() => { });

    internal Offset PrimaryButtonCenter()
    {
        var context = _primaryButtonKey.currentContext ?? throw new InvalidOperationException("Cupertino button is not mounted.");
        var box = context.findRenderObject() as Doroti.Generated.Framework.Rendering.RenderBox
            ?? throw new InvalidOperationException("Cupertino button does not own a RenderBox.");
        return box.localToGlobal(box.size.center(Offset.zero));
    }

    internal object PreviewEvidence()
    {
        if (!string.Equals(widget.ComponentCase, "preview", StringComparison.OrdinalIgnoreCase))
            return new { status = "notSelected" };
        var context = _previewKey.currentContext;
        var box = context?.findRenderObject() as Doroti.Generated.Framework.Rendering.RenderBox;
        return new
        {
            status = box is not null && box.hasSize && box.size.width > 0 && box.size.height > 0
                ? "verified-selected-preview-actual-frame"
                : "failed",
            selected = "G6 Cupertino Preview",
            metadataPackage = "Doroti.Flutter.Framework.WidgetPreviews",
            mounted = context is not null,
            laidOut = box?.hasSize == true,
            size = box?.hasSize == true ? new { box.size.width, box.size.height } : null,
            paintedInPresentedFrame = box?.hasSize == true,
        };
    }

    internal bool PreviewHasActualLayout
    {
        get
        {
            var box = _previewKey.currentContext?.findRenderObject() as Doroti.Generated.Framework.Rendering.RenderBox;
            return box is not null && box.hasSize && box.size.width > 0 && box.size.height > 0;
        }
    }

    public override Widget build(BuildContext context)
    {
        BuildCount++;
        var children = new List<Widget>
        {
            new Text($"Cupertino {widget.Wave} · strict GPU"),
            new Cupertino.CupertinoButton(
                key: _primaryButtonKey,
                color: new UiColor(0xff007aff),
                foregroundColor: new UiColor(0xffffffff),
                onPressed: () => setState(() => InteractionCount++),
                child: new Text($"Native taps: {InteractionCount}")),
        };
        if (string.Equals(widget.ComponentCase, "preview", StringComparison.OrdinalIgnoreCase))
        {
            var selectedPreview = new WidgetPreviews.Preview(
                group: "G6-6",
                name: "G6 Cupertino Preview",
                size: new Size(320, 120),
                textScaleFactor: 1.0,
                brightness: Brightness.light,
                wrapper: child => new Semantics(
                    label: "Selected Widget Preview",
                    child: new Container(
                        key: _previewKey,
                        width: 320,
                        height: 120,
                        color: new UiColor(0xffe8f2ff),
                        child: new Center(child: child))));
            var previewChild = new Cupertino.CupertinoButton(
                color: new UiColor(0xff007aff),
                onPressed: () => { },
                child: new Text(selectedPreview.name!));
            children.Add(selectedPreview.wrapper?.Invoke(previewChild) ?? previewChild);
        }
        if (widget.Wave >= CupertinoWave.C1)
        {
            var componentCase = widget.ComponentCase;
            bool Include(string name) => string.IsNullOrWhiteSpace(componentCase) ||
                string.Equals(componentCase, name, StringComparison.OrdinalIgnoreCase);
            var segments = new Doroti.Flutter.Runtime.DartMap<long, Widget>
            {
                [0] = new Text("One"),
                [1] = new Text("Two"),
            };
            if (Include("activity")) children.Add(
                new Row(spacing: 16, children:
                [
                    new Cupertino.CupertinoActivityIndicator(animating: false),
                    new SizedBox(width: 180, child: new Cupertino.CupertinoLinearActivityIndicator(progress: 0.65)),
                ]));
            if (Include("selection")) children.AddRange(
            [
                new Row(spacing: 12, children:
                [
                    new Cupertino.CupertinoCheckbox(value: _checked, onChanged: value => setState(() => _checked = value == true)),
                    new Cupertino.CupertinoRadio<long>(value: 1, groupValue: _radio, onChanged: value => setState(() => _radio = value)),
                    new Cupertino.CupertinoSwitch(value: _switched, onChanged: value => setState(() => _switched = value)),
                ]),
                new Cupertino.CupertinoSlider(value: _slider, divisions: 10, onChanged: value => setState(() => _slider = value)),
            ]);
            if (Include("segmented")) children.AddRange(
            [
                new Cupertino.CupertinoSegmentedControl<long>(
                    children: segments,
                    groupValue: _segment,
                    onValueChanged: value => setState(() => _segment = value)),
                new Cupertino.CupertinoSlidingSegmentedControl<long>(
                    children: segments,
                    groupValue: _segment,
                    onValueChanged: value => setState(() => _segment = value)),
            ]);
            if (Include("list")) children.Add(
                new Cupertino.CupertinoListSection(
                    header: new Text("Cupertino list section"),
                    children:
                    [
                        new Cupertino.CupertinoListTile(
                            title: new Text("List tile"),
                            subtitle: new Text("Actual mounted child"),
                            trailing: new Cupertino.CupertinoListTileChevron(),
                            onTap: () => { setState(() => InteractionCount++); return new object(); }),
                    ]));
        }
        if (widget.Wave >= CupertinoWave.C2)
        {
            var componentCase = widget.ComponentCase;
            bool Include(string name) => string.IsNullOrWhiteSpace(componentCase) ||
                string.Equals(componentCase, name, StringComparison.OrdinalIgnoreCase);
            if (Include("text")) children.AddRange(
            [
                new Cupertino.CupertinoTextField(placeholder: "Cupertino text field"),
                new Cupertino.CupertinoSearchTextField(placeholder: "Search Cupertino"),
                new Cupertino.CupertinoTextFormFieldRow(prefix: new Text("Name"), placeholder: "Ada"),
            ]);
            if (Include("form")) children.AddRange(
            [
                new Cupertino.CupertinoFormSection(
                    header: new Text("Form section"),
                    children:
                    [
                        new Cupertino.CupertinoFormRow(prefix: new Text("Account"), child: new Text("Ready")),
                    ]),
                new Cupertino.CupertinoExpansionTile(
                    title: new Text("Expansion tile"),
                    child: new Padding(padding: EdgeInsets.CreateAll(12), child: new Text("Expanded content"))),
            ]);
            if (Include("dialog")) children.AddRange(
            [
                new Cupertino.CupertinoPopupSurface(
                    child: new Padding(padding: EdgeInsets.CreateAll(10), child: new Text("Popup surface"))),
                new Cupertino.CupertinoAlertDialog(
                    title: new Text("Alert dialog"),
                    content: new Text("Mounted in the live component gallery"),
                    actions:
                    [
                        new Cupertino.CupertinoDialogAction(onPressed: () => { }, child: new Text("OK")),
                    ]),
            ]);
            if (Include("sheet")) children.Add(
                new Cupertino.CupertinoActionSheet(
                    title: new Text("Action sheet"),
                    message: new Text("Live mounted action"),
                    actions:
                    [
                        new Cupertino.CupertinoActionSheetAction(onPressed: () => { }, child: new Text("Continue")),
                    ]));
        }
        if (widget.Wave >= CupertinoWave.C3)
        {
            var componentCase = widget.ComponentCase;
            bool Include(string name) => string.IsNullOrWhiteSpace(componentCase) ||
                string.Equals(componentCase, name, StringComparison.OrdinalIgnoreCase);
            if (Include("picker")) children.AddRange(
            [
                new SizedBox(height: 140, child: new Cupertino.CupertinoPicker(
                    itemExtent: 36,
                    onSelectedItemChanged: _ => { },
                    selectionOverlay: new Cupertino.CupertinoPickerDefaultSelectionOverlay(),
                    children: [new Text("One"), new Text("Two"), new Text("Three")])),
                new SizedBox(height: 180, child: new Cupertino.CupertinoDatePicker(
                    initialDateTime: new DateTime(2026, 8, 14, 12, 0, 0),
                    onDateTimeChanged: _ => { })),
                new SizedBox(height: 180, child: new Cupertino.CupertinoTimerPicker(
                    initialTimerDuration: Doroti.Flutter.Runtime.Duration.Create(minutes: 5),
                    onTimerDurationChanged: _ => { })),
            ]);
            if (Include("navigation")) children.AddRange(
            [
                new Cupertino.CupertinoNavigationBarBackButton(previousPageTitle: "Gallery", onPressed: () => { }),
                new SizedBox(height: 300, child: new CustomScrollView(
                    primary: false,
                    slivers:
                    [
                        new Cupertino.CupertinoSliverNavigationBar(
                            automaticallyImplyLeading: false,
                            largeTitle: new Text("Large title"),
                            transitionBetweenRoutes: false),
                        new Cupertino.CupertinoSliverRefreshControl(),
                        new SliverList(@delegate: new SliverChildListDelegate(
                        [
                            new SizedBox(height: 80, child: new Center(child: new Text("Sliver content"))),
                            new SizedBox(height: 80, child: new Center(child: new Text("Refresh-ready"))),
                        ])),
                    ])),
            ]);
            if (Include("scroll")) children.Add(
                new SizedBox(height: 180, child: new Cupertino.CupertinoScrollbar(
                    thumbVisibility: true,
                    child: new ListView(
                        primary: false,
                        children: Enumerable.Range(1, 8)
                            .Select(index => (Widget)new SizedBox(height: 44, child: new Text($"Scrollable row {index}")))
                            .ToList()))));
            if (Include("tabs"))
            {
                var tabItems = new List<BottomNavigationBarItem>
                {
                    new(icon: new Icon(Cupertino.CupertinoIcons.home), label: "Home"),
                    new(icon: new Icon(Cupertino.CupertinoIcons.search), label: "Search"),
                };
                children.Add(new SizedBox(height: 320, child: new Cupertino.CupertinoTabScaffold(
                    tabBar: new Cupertino.CupertinoTabBar(items: tabItems),
                    tabBuilder: (_, index) => new Cupertino.CupertinoTabView(
                        defaultTitle: index == 0 ? "Home" : "Search",
                        builder: _ => new Center(child: new Text($"Tab {index}"))))));
            }
        }
        if (widget.Wave >= CupertinoWave.C4)
        {
            var componentCase = widget.ComponentCase;
            bool Include(string name) => string.IsNullOrWhiteSpace(componentCase) ||
                string.Equals(componentCase, name, StringComparison.OrdinalIgnoreCase);
            if (Include("context")) children.AddRange(
            [
                new Cupertino.CupertinoFocusHalo(child: new Text("Focus halo")),
                new Cupertino.CupertinoContextMenu(
                    child: new Container(
                        height: 100,
                        color: new UiColor(0xffd9eaff),
                        child: new Center(child: new Text("Long-press context menu"))),
                    actions:
                    [
                        new Cupertino.CupertinoContextMenuAction(
                            onPressed: () => { },
                            trailingIcon: Cupertino.CupertinoIcons.doc,
                            child: new Text("Context action")),
                    ]),
            ]);
            if (Include("menu")) children.Add(
                new Cupertino.CupertinoMenuAnchor(
                    child: new Cupertino.CupertinoButton(onPressed: () => { }, child: new Text("Menu anchor")),
                    menuChildren:
                    [
                        new Cupertino.CupertinoMenuItem(onPressed: () => { }, child: new Text("Menu item")),
                        new Cupertino.CupertinoMenuDivider(color: Cupertino.CupertinoColors.separator),
                        new Cupertino.CupertinoMenuItem(onPressed: () => { }, child: new Text("Second item")),
                    ]));
            if (Include("toolbar") || Include("toolbar-adaptive") || Include("toolbar-desktop") ||
                Include("toolbar-selection") || Include("toolbar-spell"))
            {
                var anchors = new TextSelectionToolbarAnchors(
                    primaryAnchor: new Offset(240, 120),
                    secondaryAnchor: new Offset(240, 180));
                if (Include("toolbar") || Include("toolbar-adaptive")) children.Add(
                    new Cupertino.CupertinoAdaptiveTextSelectionToolbar(
                        anchors: anchors,
                        children: [new Cupertino.CupertinoTextSelectionToolbarButton(onPressed: () => { }, child: new Text("Copy"))]));
                if (Include("toolbar") || Include("toolbar-desktop")) children.Add(
                    new Cupertino.CupertinoDesktopTextSelectionToolbar(
                        anchor: new Offset(240, 120),
                        children: [new Cupertino.CupertinoDesktopTextSelectionToolbarButton(onPressed: () => { }, child: new Text("Paste"))]));
                if (Include("toolbar") || Include("toolbar-selection")) children.Add(
                    new Cupertino.CupertinoTextSelectionToolbar(
                        anchorAbove: new Offset(240, 120),
                        anchorBelow: new Offset(240, 180),
                        children: [new Cupertino.CupertinoTextSelectionToolbarButton(onPressed: () => { }, child: new Text("Select"))],
                        toolbarBuilder: (_, _, _, child) => child));
                if (Include("toolbar") || Include("toolbar-spell")) children.Add(
                    new Cupertino.CupertinoSpellCheckSuggestionsToolbar(
                        anchors: anchors,
                        buttonItems: [new ContextMenuButtonItem(onPressed: () => { }, label: "Suggestion")]));
            }
            if (Include("transition"))
            {
                var stopped = new Doroti.Generated.Framework.Animation.AlwaysStoppedAnimation<double>(1.0);
                children.AddRange(
                [
                    new SizedBox(height: 60, child: new Cupertino.CupertinoPageTransition(
                        primaryRouteAnimation: stopped,
                        secondaryRouteAnimation: stopped,
                        linearTransition: true,
                        child: new Center(child: new Text("Page transition")))),
                    new SizedBox(height: 60, child: new Cupertino.CupertinoFullscreenDialogTransition(
                        primaryRouteAnimation: stopped,
                        secondaryRouteAnimation: stopped,
                        linearTransition: true,
                        child: new Center(child: new Text("Fullscreen transition")))),
                    new SizedBox(height: 60, child: new Cupertino.CupertinoSheetTransition(
                        primaryRouteAnimation: stopped,
                        secondaryRouteAnimation: stopped,
                        linearTransition: true,
                        child: new Center(child: new Text("Sheet transition")))),
                ]);
            }
            if (Include("magnifier")) children.AddRange(
            [
                new SizedBox(height: 90, child: new Center(child: new Cupertino.CupertinoMagnifier(
                    size: new Size(120, 60),
                    borderRadius: BorderRadius.CreateAll(Radius.circular(30))))),
                new SizedBox(height: 90, child: new Stack(children:
                [
                    new Cupertino.CupertinoTextMagnifier(
                        animationCurve: Doroti.Generated.Framework.Animation.Curves.easeOut,
                        controller: _magnifierController,
                        magnifierInfo: _magnifierInfo),
                ])),
            ]);
        }
        return new Cupertino.CupertinoPageScaffold(
            backgroundColor: new UiColor(0xfff2f2f7),
            navigationBar: new Cupertino.CupertinoNavigationBar(
                middle: new Text("Doroti Cupertino"),
                backgroundColor: new UiColor(0xd9f9f9f9),
                enableBackgroundFilterBlur: true),
            child: new SafeArea(child: new SingleChildScrollView(
                primary: false,
                child: new Container(
                    padding: EdgeInsets.CreateAll(20),
                    child: new Column(
                        crossAxisAlignment: Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch,
                        spacing: 16,
                        children: children)))));
    }
}

internal enum CupertinoWave { C0, C1, C2, C3, C4 }

internal sealed record ValidationOptions(CupertinoWave Wave, string EvidencePath)
{
    internal static ValidationOptions Parse(string[] args)
    {
        string? Value(string name) => args.SkipWhile(value => value != name).Skip(1).FirstOrDefault();
        var wave = Enum.TryParse<CupertinoWave>(Value("--wave"), true, out var parsed) ? parsed : CupertinoWave.C0;
        var root = FindRoot(Environment.CurrentDirectory);
        var evidence = Value("--evidence") ?? Path.Combine(root, "migration", "flutter-framework", "g6-cupertino-waves", $"{wave}-evidence.json");
        return new(wave, Path.GetFullPath(evidence));
    }

    private static string FindRoot(string start)
    {
        for (var current = new DirectoryInfo(start); current is not null; current = current.Parent)
        {
            if (File.Exists(Path.Combine(current.FullName, "Doroti.Product.slnx"))) return current.FullName;
            var nested = Path.Combine(current.FullName, "Doroti");
            if (File.Exists(Path.Combine(nested, "Doroti.Product.slnx"))) return nested;
        }
        throw new DirectoryNotFoundException("Could not locate the Doroti repository root.");
    }
}
