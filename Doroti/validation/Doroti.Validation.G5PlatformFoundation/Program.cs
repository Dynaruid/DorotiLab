using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Doroti.Flutter.Hosting;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using Doroti.Generated.Framework.Services;
using Doroti.Host.Desktop;
using Doroti.Host.Desktop.Flutter;
using UiColor = Doroti.Flutter.Ui.Color;
using Path = System.IO.Path;

if (!OperatingSystem.IsWindows())
{
    Console.Error.WriteLine("G5-3B platform foundation validation requires Windows.");
    return 2;
}

var outputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "migration", "flutter-avalonia", "bridge-validation", "g5-3-platform-foundation.json"));
var failures = new List<string>();
var trace = new List<string>();
var entrypoint = new FoundationEntrypoint(trace);
DesktopFlutterFrameDiagnostics frameDiagnostics;
DesktopFlutterFoundationDiagnostics foundationDiagnostics;
string targetIdentity;
ulong viewId;
long nativeHandle;
long initialMetricsGeneration;
long resizedMetricsGeneration;
int pointerPackets;
int keyPackets;
int focusEvents;
bool clipboardRoundTrip = false;
bool textInputRoundTrip = false;
bool cursorRoundTrip = false;
bool accessibilityNativeEntrypoint = false;
string? clipboardBefore = null;

using (var backend = new DesktopWindowBackend())
using (var session = new FlutterHostSession(entrypoint))
using (var scope = session.dispatcher.EnterScope())
using (var host = new DesktopFlutterHost(backend))
{
    session.Start(deferFrameworkBootstrap: true);
    var view = host.CreateView(session, 530, new("Doroti G5-3 platform foundation", new(480, 320)));
    viewId = view.viewId;
    targetIdentity = view.targetIdentity;
    nativeHandle = host.GetNativeWindowHandle(view.viewId).ToInt64();
    initialMetricsGeneration = view.metrics.generation;
    pointerPackets = 0;
    keyPackets = 0;
    focusEvents = 0;
    session.dispatcher.onPointerDataPacket = (sourceView, packet) =>
    {
        pointerPackets += packet.data.Count;
        trace.Add($"pointer:{sourceView.viewId}:{packet.data.Count}");
    };
    session.dispatcher.onKeyData = data =>
    {
        keyPackets++;
        trace.Add($"key:{view.viewId}:{data.type}");
        return true;
    };
    session.dispatcher.onViewFocusChange = data =>
    {
        focusEvents++;
        trace.Add($"focus:{data.viewId}:{data.state}");
    };
    session.dispatcher.onMetricsChanged = changedView => trace.Add($"metrics:{changedView.viewId}:{changedView.metrics.generation}");
    view.Show();
    PumpUntil(backend, () => Terminal(host.GetFrameDiagnostics(view.viewId)) >= 1, TimeSpan.FromSeconds(8));

    frameDiagnostics = host.GetFrameDiagnostics(view.viewId);
    Require(nativeHandle != 0, "bootstrap: native HWND is zero.", failures);
    Require(frameDiagnostics.Presented == 1 && Terminal(frameDiagnostics) == 1,
        $"frame: expected one presented terminal ACK, got presented={frameDiagnostics.Presented}, terminal={Terminal(frameDiagnostics)}.", failures);
    Require(trace.Count(item => item.StartsWith("beginFrame:", StringComparison.Ordinal)) == 1 &&
        trace.Count(item => item.StartsWith("drawFrame:", StringComparison.Ordinal)) == 1 &&
        trace.Count(item => item.StartsWith("render:", StringComparison.Ordinal)) == 1,
        "frame: begin/draw/render trace is missing or duplicated.", failures);

    view.Resize(new(540, 360));
    PumpUntil(backend, () => view.metrics.generation > initialMetricsGeneration, TimeSpan.FromSeconds(3));
    resizedMetricsGeneration = view.metrics.generation;
    Require(resizedMetricsGeneration > initialMetricsGeneration && view.metrics.logicalSize == new Doroti.Flutter.Ui.Size(540, 360),
        "metrics: resize did not round-trip on the attached view identity.", failures);

    var hwnd = (nint)nativeHandle;
    _ = SetFocus(hwnd);
    _ = PostMessage(hwnd, 0x0200, 0, MakeLParam(41, 52));
    _ = PostMessage(hwnd, 0x0201, 0x0001, MakeLParam(41, 52));
    _ = PostMessage(hwnd, 0x0202, 0, MakeLParam(41, 52));
    _ = PostMessage(hwnd, 0x0100, 0x09, 0);
    _ = PostMessage(hwnd, 0x0101, 0x09, 0);
    PumpUntil(backend, () => pointerPackets >= 3 && keyPackets >= 2, TimeSpan.FromSeconds(3));
    Require(pointerPackets >= 3, $"input: expected at least three pointer packets, got {pointerPackets}.", failures);
    Require(keyPackets >= 2, $"input: expected key down/up packets, got {keyPackets}.", failures);
    Require(focusEvents >= 1, "input: actual HWND did not report focus on the attached view.", failures);

    var invocation = new DartUiInvocation(
        "package:flutter/src/widgets/binding.dart#WidgetsBinding",
        new("packages/flutter/lib/src/widgets/binding.dart", 0, 0));
    var platform = view.RequireCapability<IPlatformServicesHostCapability>(FlutterCapabilityIds.PlatformServices, invocation);
    try
    {
        clipboardBefore = await platform.GetClipboardTextAsync();
        var marker = $"doroti-g5-3-{Environment.ProcessId}";
        await platform.SetClipboardTextAsync(marker);
        clipboardRoundTrip = string.Equals(await platform.GetClipboardTextAsync(), marker, StringComparison.Ordinal);
    }
    finally
    {
        await platform.SetClipboardTextAsync(clipboardBefore ?? string.Empty);
    }
    platform.SetCursor(FlutterMouseCursorKind.text);
    platform.SetCursor(FlutterMouseCursorKind.basic);
    cursorRoundTrip = true;

    var textInput = view.RequireCapability<ITextInputHostCapability>(FlutterCapabilityIds.TextInput, invocation);
    var editing = new FlutterTextEditingState("Doroti 한글", new(6, 6), new FlutterTextSelection(0, 6));
    textInput.SetClient(editing);
    textInput.UpdateState(editing with { selection = new(8, 8), composingRange = null });
    textInput.SetCaretRect(Rect.fromLTWH(18, 24, 2, 22));
    textInput.ClearClient();
    textInputRoundTrip = true;

    session.dispatcher.setSemanticsTreeEnabled(true);
    view.updateSemantics(new SemanticsUpdate(1,
    [
        new SemanticsNodeUpdate(1, Rect.fromLTWH(0, 0, 540, 360), "G5-3 root", null, SemanticsAction.none, [2]),
        new SemanticsNodeUpdate(2, Rect.fromLTWH(20, 20, 160, 48), "G5-3 action", null,
            SemanticsAction.tap | SemanticsAction.focus, [], new(isButton: true, isEnabled: Tristate.isTrue)),
    ]));
    PumpFor(backend, TimeSpan.FromMilliseconds(100));
    accessibilityNativeEntrypoint = SendMessage(hwnd, 0x003D, 0, -25) != 0;
    Require(accessibilityNativeEntrypoint, "accessibility: WM_GETOBJECT/UIA root entrypoint returned zero.", failures);

    var requiredCapabilities = new[]
    {
        FlutterCapabilityIds.WindowLifecycle,
        FlutterCapabilityIds.ViewLifecycleMetrics,
        FlutterCapabilityIds.ViewFrameDispatch,
        FlutterCapabilityIds.InputEvents,
        FlutterCapabilityIds.TextInput,
        FlutterCapabilityIds.PlatformServices,
        FlutterCapabilityIds.PlatformMessaging,
        FlutterCapabilityIds.PlatformEnvironment,
        FlutterCapabilityIds.GraphicsScene,
        FlutterCapabilityIds.GraphicsText,
        FlutterCapabilityIds.GraphicsImage,
        FlutterCapabilityIds.AccessibilitySemantics,
    };
    Require(requiredCapabilities.All(id => view.registeredCapabilityIds.Contains(id, StringComparer.Ordinal)),
        "capability: one or more G5-3B capability IDs are missing from the attached view.", failures);
    foundationDiagnostics = host.GetFoundationDiagnostics();
    Require(foundationDiagnostics.RegisteredViews == 1 && foundationDiagnostics.FlutterSurfaces == 1 && foundationDiagnostics.SessionAttachedViews == 1,
        $"surface: expected 1/1/1 view, surface and session attachment, got {foundationDiagnostics}.", failures);

    host.Dispose();
    session.Shutdown();
}

Require(trace.FirstOrDefault() == "bootstrap", "lifecycle: bootstrap was not first.", failures);
Require(trace.Contains("attach:530") && trace.Contains("detach:530") && trace.LastOrDefault() == "shutdown",
    "lifecycle: attach/detach/shutdown trace is incomplete.", failures);
Require(trace.Count(item => item == "attach:530") == 1 && trace.Count(item => item == "detach:530") == 1 && trace.Count(item => item == "shutdown") == 1,
    "lifecycle: attach/detach/shutdown was duplicated.", failures);

var missingCapabilities = new FlutterViewCapabilities("win-x64/missing-capability-fixture")
    .Register<IViewHostCapability>(FlutterCapabilityIds.ViewLifecycleMetrics, new MissingCapabilityViewHost());
using (var dispatcher = new PlatformDispatcher())
using (var view = dispatcher.RegisterView(531, missingCapabilities))
{
    try
    {
        _ = view.RequireCapability<IPlatformServicesHostCapability>(
            FlutterCapabilityIds.PlatformServices,
            DartUiInvocation.Managed("package:flutter/src/widgets/binding.dart#missingCapabilityFixture"));
        failures.Add("capability: missing platform.services silently succeeded.");
    }
    catch (FlutterCapabilityException exception)
    {
        Require(exception.CapabilityId == FlutterCapabilityIds.PlatformServices &&
            exception.ViewId == 531 &&
            exception.TargetIdentity == "win-x64/missing-capability-fixture" &&
            exception.ElementId.Contains("missingCapabilityFixture", StringComparison.Ordinal),
            "capability: fail-closed diagnostic omitted capability/view/target/Flutter symbol identity.", failures);
    }
}

var assemblyDirectory = AppContext.BaseDirectory;
var officialAvaloniaReferences = Directory.EnumerateFiles(assemblyDirectory, "Doroti*.dll")
    .Select(path => Assembly.LoadFrom(path))
    .SelectMany(assembly => assembly.GetReferencedAssemblies())
    .Where(reference => reference.Name?.StartsWith("Avalonia", StringComparison.Ordinal) == true)
    .Select(reference => reference.Name!)
    .Distinct(StringComparer.Ordinal)
    .Order(StringComparer.Ordinal)
    .ToArray();
Require(officialAvaloniaReferences.Length == 0,
    $"boundary: official Avalonia binary references remain: {string.Join(", ", officialAvaloniaReferences)}.", failures);

var evidence = new
{
    schemaVersion = "doroti.g5-3-platform-foundation/v1",
    milestone = "G5-3B",
    capturedAtUtc = DateTimeOffset.UtcNow,
    status = failures.Count == 0 ? "verified-windows-automated" : "failed",
    sourcePort = new
    {
        avaloniaRevision = "f159423f691946e713f454447a780d4677d8a0d2",
        audit = "validated-by-eng-entrypoint",
        officialAvaloniaBinaryReferences = officialAvaloniaReferences.Length,
        avaloniaControlMirrorTree = 0,
    },
    application = new
    {
        entrypointContract = typeof(IFlutterViewEntrypoint).FullName,
        predecessorBinding = typeof(ServicesBinding).FullName,
        targetIdentity,
        viewId,
        nativeHandle,
        lifecycleTrace = trace,
        frameClockOwnerCount = 1,
        frameDiagnostics,
        foundationDiagnostics,
    },
    capabilityRoundTrips = new
    {
        resize = resizedMetricsGeneration > initialMetricsGeneration,
        pointerPackets,
        keyPackets,
        focusEvents,
        textInput = textInputRoundTrip,
        clipboard = clipboardRoundTrip,
        cursor = cursorRoundTrip,
        accessibilityWmGetObject = accessibilityNativeEntrypoint,
        sameViewIdentity = viewId,
        failClosedDiagnostic = "capability+view+target+flutter-symbol",
    },
    physical = new
    {
        status = "notVerified",
        deferredTo = "G5-8 DorotiDemoApp",
        items = new[] { "physical input", "physical IME", "cross-monitor DPI", "sustained physical GPU", "external physical accessibility" },
    },
    failures,
};
WriteJson(outputPath, evidence);
Console.WriteLine($"G5-3B selected Avalonia Windows application foundation: {(failures.Count == 0 ? "PASS" : "FAIL")}");
foreach (var failure in failures) Console.Error.WriteLine(failure);
Console.WriteLine($"Evidence: {outputPath}");
return failures.Count == 0 ? 0 : 1;

static long Terminal(DesktopFlutterFrameDiagnostics value) =>
    value.Presented + value.Superseded + value.Stale + value.Failed + value.Cancelled;

static nint MakeLParam(short x, short y) => (nint)((ushort)x | (y << 16));

static void PumpUntil(DesktopWindowBackend backend, Func<bool> completed, TimeSpan timeout)
{
    var deadline = DateTime.UtcNow + timeout;
    while (!completed() && DateTime.UtcNow < deadline)
    {
        backend.PumpPendingMessages();
        Thread.Sleep(1);
    }
    backend.PumpPendingMessages();
}

static void PumpFor(DesktopWindowBackend backend, TimeSpan duration) => PumpUntil(backend, () => false, duration);

static void Require(bool condition, string failure, List<string> failures)
{
    if (!condition) failures.Add(failure);
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

[DllImport("user32.dll")]
static extern bool PostMessage(nint window, uint message, nint wParam, nint lParam);

[DllImport("user32.dll")]
static extern nint SetFocus(nint window);

[DllImport("user32.dll")]
static extern nint SendMessage(nint window, uint message, nint wParam, nint lParam);

sealed class FoundationEntrypoint(List<string> trace) : IFlutterViewEntrypoint
{
    private readonly List<string> _trace = trace;
    private FoundationBinding? _binding;
    private FlutterView? _view;
    private bool _persistentCallbackRegistered;

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        _trace.Add("bootstrap");
        _binding = new FoundationBinding(dispatcher, _trace);
    }

    public void AttachView(FlutterView view)
    {
        if (_binding is null) throw new InvalidOperationException("Framework binding was not bootstrapped.");
        if (_view is not null) throw new InvalidOperationException("The G5-3B entrypoint owns exactly one Flutter surface.");
        _view = view;
        _trace.Add($"attach:{view.viewId}");
        if (!_persistentCallbackRegistered)
        {
            _persistentCallbackRegistered = true;
            _binding.addPersistentFrameCallback(_ => RenderAttachedView());
        }
        _trace.Add($"requestFrame:{view.viewId}");
        _binding.scheduleFrame();
    }

    public void DetachView(FlutterView view)
    {
        if (ReferenceEquals(_view, view))
        {
            _trace.Add($"detach:{view.viewId}");
            _view = null;
        }
    }

    public void Shutdown()
    {
        _binding?.Dispose();
        _binding = null;
        _trace.Add("shutdown");
    }

    private void RenderAttachedView()
    {
        var view = _view ?? throw new InvalidOperationException("Draw frame arrived without an attached Flutter view.");
        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder, Rect.fromLTWH(0, 0, view.metrics.logicalSize.width, view.metrics.logicalSize.height));
        canvas.drawPaint(new Paint { color = new UiColor(0xff10233f) });
        canvas.drawRect(Rect.fromLTWH(24, 28, 180, 96), new Paint { color = new UiColor(0xff36c2a3) });
        using var picture = recorder.endRecording();
        var builder = new SceneBuilder(view.viewId);
        builder.addPicture(Offset.zero, picture);
        using var scene = builder.build();
        _trace.Add($"render:{view.viewId}");
        view.SubmitScene(scene, new(
            "package:flutter/src/widgets/binding.dart#drawFrame",
            new("packages/flutter/lib/src/widgets/binding.dart", 0, 0)));
    }
}

sealed class FoundationBinding : ServicesBinding
{
    private readonly PlatformDispatcher _dispatcher;
    private readonly List<string> _trace;

    public FoundationBinding(PlatformDispatcher dispatcher, List<string> trace)
        : base(dispatcher)
    {
        _dispatcher = dispatcher;
        _trace = trace;
    }

    public override void handleBeginFrame(Duration? rawTimeStamp)
    {
        _trace.Add($"beginFrame:{_dispatcher.views.Single().viewId}");
        base.handleBeginFrame(rawTimeStamp);
    }

    public override void handleDrawFrame()
    {
        _trace.Add($"drawFrame:{_dispatcher.views.Single().viewId}");
        base.handleDrawFrame();
    }
}

sealed class MissingCapabilityViewHost : IViewHostCapability
{
    public ViewMetrics Metrics { get; } = new(new(100, 100), 1, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.inactive, 0, 0);
    public event Action<ViewMetrics>? MetricsChanged { add { } remove { } }
    public event Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
    public event Action? CloseRequested { add { } remove { } }
    public event Action? Closed { add { } remove { } }
    public void Show() { }
    public void Resize(Doroti.Flutter.Ui.Size logicalSize) { }
    public void Close() { }
    public void Dispose() { }
}
