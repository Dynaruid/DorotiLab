using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Ui;
using Doroti.Generated.Framework.Foundation;
using Doroti.Generated.Framework.Painting;
using Doroti.Generated.Framework.Widgets;
using Doroti.Target.Windows;
using UiColor = Doroti.Ui.Color;
using Path = System.IO.Path;

const ulong ViewId = 620;
var options = ValidationOptions.Parse(args);
var failures = new List<string>();
var events = new List<string>();
var entrypoint = new WidgetsLiveEntrypoint(events);
var started = Stopwatch.StartNew();
object? firstPixels = null;
object? resizedPixels = null;
object? activeDiagnostics = null;
object? resourceClosure = null;

if (!OperatingSystem.IsWindows())
{
    Console.Error.WriteLine("G6-2 Widgets live validation requires Windows x64.");
    return 2;
}

using (var target = new WindowsTarget())
using (var session = new DorotiHostSession(entrypoint))
using (session.dispatcher.EnterScope())
{
    session.Start(deferFrameworkBootstrap: true);
    var view = target.CreateView(session, ViewId, new("Doroti G6-2 Widgets Live", new(640, 420)));
    try
    {
        var firstReadback = target.CaptureNextFrameAsync(ViewId);
        view.Show();
        session.dispatcher.setSemanticsTreeEnabled(true);
        WaitUntil(() => target.CaptureDiagnostics(ViewId).Frame.Presented >= 1, target, entrypoint, TimeSpan.FromSeconds(15));
        firstPixels = AnalyzePixels(await firstReadback.WaitAsync(TimeSpan.FromSeconds(10)), 0xff172033, 0xff2f80ed);
        var first = target.CaptureDiagnostics(ViewId);
        Require(first.Frame.BackendIdentity == "skia-wgl-opengl-gpu", "strict GPU backend identity was not preserved", failures);
        Require(!first.Frame.SoftwareFallbackUsed, "software fallback was used", failures);
        Require(first.Frame.Presented > 0, "the HWND did not present a first frame", failures);
        Require(target.GetNativeWindowHandle(ViewId) != 0, "the native HWND is zero", failures);
        Require(first.Automation.NodeCount > 0, "the live semantics tree is empty", failures);

        entrypoint.Advance(1);
        WaitForNextPresented(target, entrypoint);
        entrypoint.Advance(2);
        WaitForNextPresented(target, entrypoint);
        entrypoint.Advance(3);
        WaitForNextPresented(target, entrypoint);
        entrypoint.Advance(4);
        WaitForNextPresented(target, entrypoint);

        Require(entrypoint.Probe.SameBStateAfterReorder, "keyed child B lost State identity during reorder", failures);
        Require(entrypoint.Probe.ADisposed, "removed keyed child A was not disposed", failures);
        Require(events.Contains("single:update:3", StringComparer.Ordinal), "single-child update was not observed", failures);
        Require(events.Contains("dependency:4", StringComparer.Ordinal), "inherited dependency update was not observed", failures);

        var beforeResize = target.CaptureDiagnostics(ViewId);
        view.Resize(new(760, 500));
        WaitUntil(() => target.CaptureDiagnostics(ViewId).Input.MetricsChanges > beforeResize.Input.MetricsChanges,
            target, entrypoint, TimeSpan.FromSeconds(10));
        var resizeReadback = target.CaptureNextFrameAsync(ViewId);
        for (var attempt = 0; attempt < 3 && target.CaptureDiagnostics(ViewId).Frame.Presented <= beforeResize.Frame.Presented; attempt++)
        {
            var submitted = target.CaptureDiagnostics(ViewId).Frame.Submitted;
            entrypoint.RequestFrame();
            WaitUntil(() =>
            {
                var frame = target.CaptureDiagnostics(ViewId).Frame;
                var terminal = frame.Presented + frame.Superseded + frame.Stale + frame.Failed + frame.Cancelled;
                return frame.Submitted > submitted && frame.Submitted == terminal;
            }, target, entrypoint, TimeSpan.FromSeconds(10));
        }
        WaitUntil(() => target.CaptureDiagnostics(ViewId).Frame.Presented > beforeResize.Frame.Presented,
            target, entrypoint, TimeSpan.FromSeconds(10));
        resizedPixels = AnalyzePixels(await resizeReadback.WaitAsync(TimeSpan.FromSeconds(10)), 0xff172033, 0xff2f80ed);

        var handle = target.GetNativeWindowHandle(ViewId);
        NativeMethods.ShowWindow(handle, 6);
        WaitUntil(() => view.metrics.lifecycleState == AppLifecycleState.hidden, target, entrypoint, TimeSpan.FromSeconds(5));
        var beforeRestore = target.CaptureDiagnostics(ViewId).Frame;
        NativeMethods.ShowWindow(handle, 9);
        NativeMethods.SetForegroundWindow(handle);
        entrypoint.RequestFrame();
        WaitUntil(() => view.metrics.lifecycleState != AppLifecycleState.hidden &&
                        target.CaptureDiagnostics(ViewId).Frame.Presented > beforeRestore.Presented,
            target, entrypoint, TimeSpan.FromSeconds(10));

        if (options.ReadyPath is not null)
        {
            WriteJson(options.ReadyPath, new
            {
                schemaVersion = "doroti.g6-2-widgets-live-ready/v1",
                processId = Environment.ProcessId,
                hwnd = handle.ToInt64(),
                windowTitle = "Doroti G6-2 Widgets Live",
                semanticsName = WidgetsLiveEntrypoint.SemanticsLabel,
            });
        }

        var cadence = TimeSpan.FromMilliseconds(options.Duration.TotalMilliseconds / Math.Max(1, options.Frames - 1));
        var frameRun = Stopwatch.StartNew();
        var baselinePresented = target.CaptureDiagnostics(ViewId).Frame.Presented;
        for (var index = 0; index < options.Frames; index++)
        {
            var due = TimeSpan.FromTicks(cadence.Ticks * index);
            while (frameRun.Elapsed < due)
            {
                target.PumpPendingMessages();
                Thread.Sleep(1);
            }
            entrypoint.RequestFrame();
            var expected = baselinePresented + index + 1;
            WaitUntil(() => target.CaptureDiagnostics(ViewId).Frame.Presented >= expected,
                target, entrypoint, TimeSpan.FromSeconds(5));
        }
        while (frameRun.Elapsed < options.Duration)
        {
            target.PumpPendingMessages();
            Thread.Sleep(1);
        }

        activeDiagnostics = target.CaptureDiagnostics(ViewId);
        var final = target.CaptureDiagnostics(ViewId);
        var terminal = final.Frame.Presented + final.Frame.Superseded + final.Frame.Stale + final.Frame.Failed + final.Frame.Cancelled;
        Require(final.Frame.Presented - baselinePresented >= options.Frames, $"only {final.Frame.Presented - baselinePresented}/{options.Frames} cadence frames presented", failures);
        Require(frameRun.Elapsed >= options.Duration, $"cadence ran for only {frameRun.Elapsed}", failures);
        Require(final.Frame.Submitted == terminal, $"terminal ACK imbalance: submitted={final.Frame.Submitted}, terminal={terminal}", failures);
        Require(final.Frame.Failed == 0 && final.Frame.Cancelled == 0, "frame errors or cancellations were observed", failures);
        Require(final.Frame.QueueDepth == 0 && final.Frame.ActiveFrames == 0, "frame queue did not drain", failures);
        Require(final.Input.MetricsChanges >= 2, "resize/minimize/restore metrics roundtrip was incomplete", failures);
        Require(final.Input.FocusChanges > 0, "native focus did not roundtrip into dart:ui", failures);
        Require(final.Automation.NodeCount > 0, "semantics disappeared after lifecycle recovery", failures);
        Require(!options.RequireExternalUia || entrypoint.Probe.SemanticsTapCount > 0, "external UIA Invoke did not reach Semantics.onTap", failures);
        Require(!options.RequireExternalUia || entrypoint.Probe.SemanticsFocusCount > 0, "external UIA focus did not reach Semantics.onFocus", failures);
    }
    finally
    {
        session.DetachView(view);
        session.Shutdown();
        view.Dispose();
        for (var i = 0; i < 20; i++) target.PumpPendingMessages();
        var resources = target.CaptureResourceSnapshot();
        resourceClosure = resources;
        Require(resources.IsBalanced, $"native resources are not balanced: {resources}", failures);
    }
}

Require(entrypoint.FirstFrameworkError is null,
    entrypoint.FirstFrameworkError is null ? "" : $"FlutterError: {entrypoint.FirstFrameworkError.exceptionThrown}", failures);
Require(events.Contains("root:dispose", StringComparer.Ordinal), "root State was not disposed during shutdown", failures);

var evidence = new
{
    schemaVersion = "doroti.g6-2-widgets-live-evidence/v1",
    milestone = "G6-2",
    capturedAtUtc = DateTimeOffset.UtcNow,
    status = failures.Count == 0 ? "verified-windows-x64-strict-gpu" : "failed",
    flutterRevision = "56b8e1a851a594b1a154f8ea93270807dab22b9a",
    target = "actual HWND / skia-wgl-opengl-gpu",
    durationMs = options.Duration.TotalMilliseconds,
    requestedCadenceFrames = options.Frames,
    totalElapsedMs = started.Elapsed.TotalMilliseconds,
    firstPixels,
    resizedPixels,
    treeTrace = events,
    semantics = new { entrypoint.Probe.SemanticsTapCount, entrypoint.Probe.SemanticsFocusCount },
    activeDiagnostics,
    resourceClosure,
    gates = new
    {
        baseWidgets = "Directionality -> ColoredBox -> Container -> Flex -> Text",
        cumulative = new[] { "SingleChildScrollView(restorationId)", "Focus", "Semantics", "ticker/animation via RawScrollbar" },
        reference = "g6-widgets-reference.json exact lifecycle trace plus pixel bounds/color tolerance",
    },
    failures,
};
WriteJson(options.EvidencePath, evidence);
Console.WriteLine($"G6-2 Widgets live: {(failures.Count == 0 ? "PASS" : "FAIL")} ({options.EvidencePath})");
foreach (var failure in failures) Console.Error.WriteLine(failure);
return failures.Count == 0 ? 0 : 1;

static void WaitForNextPresented(WindowsTarget target, WidgetsLiveEntrypoint entrypoint)
{
    var before = target.CaptureDiagnostics(ViewId).Frame.Presented;
    entrypoint.RequestFrame();
    WaitUntil(() => target.CaptureDiagnostics(ViewId).Frame.Presented > before, target, entrypoint, TimeSpan.FromSeconds(5));
}

static void WaitUntil(Func<bool> predicate, WindowsTarget target, WidgetsLiveEntrypoint entrypoint, TimeSpan timeout)
{
    var deadline = Stopwatch.StartNew();
    while (!predicate())
    {
        if (entrypoint.FirstFrameworkError is { } error) throw new InvalidOperationException("Flutter framework error", error.exceptionThrown);
        if (deadline.Elapsed > timeout)
        {
            var diagnostic = target.CaptureDiagnostics(ViewId);
            throw new TimeoutException($"G6-2 live condition timed out after {timeout}; frame={diagnostic.Frame}, metrics={diagnostic.Input.MetricsChanges}, size={diagnostic.Coordinates.LogicalClientSize}.");
        }
        target.PumpPendingMessages();
        Thread.Sleep(1);
    }
    target.PumpPendingMessages();
}

static object AnalyzePixels(Doroti.Host.Desktop.Framework.DesktopFrameworkPixelReadback frame, uint background, uint accent)
{
    var nonTransparent = 0L;
    var backgroundCount = 0L;
    var accentCount = 0L;
    var minX = frame.Width;
    var minY = frame.Height;
    var maxX = -1;
    var maxY = -1;
    for (var y = 0; y < frame.Height; y++)
    for (var x = 0; x < frame.Width; x++)
    {
        var offset = y * frame.RowBytes + x * 4;
        var value = (uint)(frame.Bgra8888Pixels[offset + 3] << 24 |
                           frame.Bgra8888Pixels[offset + 2] << 16 |
                           frame.Bgra8888Pixels[offset + 1] << 8 |
                           frame.Bgra8888Pixels[offset]);
        if ((value >> 24) != 0) nonTransparent++;
        if (value == background) backgroundCount++;
        if (value != accent) continue;
        accentCount++;
        minX = Math.Min(minX, x); minY = Math.Min(minY, y);
        maxX = Math.Max(maxX, x); maxY = Math.Max(maxY, y);
    }
    return new { frame.FrameId, frame.Width, frame.Height, nonTransparent, backgroundCount, accentCount,
        accentBounds = accentCount == 0 ? null : new { x = minX, y = minY, width = maxX - minX + 1, height = maxY - minY + 1 } };
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

sealed record ValidationOptions(string EvidencePath, string? ReadyPath, TimeSpan Duration, int Frames, bool RequireExternalUia)
{
    public static ValidationOptions Parse(string[] args)
    {
        string? Value(string name) => args.SkipWhile(arg => arg != name).Skip(1).FirstOrDefault();
        var root = FindRoot(Environment.CurrentDirectory);
        return new(
            Path.GetFullPath(Value("--evidence") ?? Path.Combine(root, "migration", "flutter-framework", "g6-widgets-live-evidence.json")),
            Value("--ready") is { } ready ? Path.GetFullPath(ready) : null,
            TimeSpan.FromMilliseconds(double.TryParse(Value("--duration-ms"), out var duration) ? duration : 30_000),
            int.TryParse(Value("--frames"), out var frames) ? frames : 300,
            args.Contains("--require-external-uia", StringComparer.Ordinal));
    }

    private static string FindRoot(string start)
    {
        for (var current = new DirectoryInfo(start); current is not null; current = current.Parent)
        {
            if (File.Exists(Path.Combine(current.FullName, "Doroti.Product.slnx"))) return current.FullName;
            var nested = Path.Combine(current.FullName, "Doroti");
            if (File.Exists(Path.Combine(nested, "Doroti.Product.slnx"))) return nested;
        }
        throw new DirectoryNotFoundException("Could not locate the Doroti root.");
    }
}

sealed class WidgetsLiveProbe(List<string> events)
{
    public Dictionary<string, TraceTileState> States { get; } = new(StringComparer.Ordinal);
    public TraceTileState? OriginalB { get; set; }
    public bool SameBStateAfterReorder { get; set; }
    public bool ADisposed { get; set; }
    public int SemanticsTapCount { get; set; }
    public int SemanticsFocusCount { get; set; }
    public void Event(string value) => events.Add(value);
}

sealed class WidgetsLiveEntrypoint(List<string> events) : IDorotiViewEntrypoint
{
    public const string SemanticsLabel = "Doroti G6-2 live action";
    private WidgetsFlutterBinding? _binding;
    private LiveRootState? _state;
    public WidgetsLiveProbe Probe { get; } = new(events);
    public FlutterErrorDetails? FirstFrameworkError { get; private set; }

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        FlutterError.onError = details => FirstFrameworkError ??= details;
        _binding = new WidgetsFlutterBinding(dispatcher);
    }

    public void AttachView(DorotiView view)
    {
        var binding = _binding ?? throw new InvalidOperationException("Widgets binding is not initialized.");
        binding.scheduleFrameCallback(_ => binding.attachRootWidget(binding.wrapWithDefaultView(new LiveRoot(Probe, state => _state = state))));
    }

    public void DetachView(DorotiView view) { }
    public void Advance(int phase) => (_state ?? throw new InvalidOperationException("Root State is not mounted.")).Advance(phase);
    public void RequestFrame() => (_binding ?? throw new InvalidOperationException("Widgets binding is not initialized.")).scheduleFrame();

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
        FlutterError.onError = null;
    }
}

sealed class LiveRoot(WidgetsLiveProbe probe, System.Action<LiveRootState> mounted) : StatefulWidget
{
    public WidgetsLiveProbe Probe { get; } = probe;
    public System.Action<LiveRootState> Mounted { get; } = mounted;
    public override IState createState() => new LiveRootState();
}

sealed class LiveRootState : State<LiveRoot>
{
    private int _phase;
    public override void initState() { base.initState(); widget.Mounted(this); widget.Probe.Event("root:init"); }
    public void Advance(int phase) => setState(() => { _phase = phase; widget.Probe.Event($"root:setState:{phase}"); });

    public override Widget build(BuildContext context)
    {
        widget.Probe.Event($"root:build:{_phase}");
        var ids = _phase switch
        {
            0 or 1 => new[] { "A", "B" },
            2 => new[] { "B", "A" },
            _ => new[] { "B" },
        };
        var children = ids.Select(id => (Widget)new TraceTile(id, _phase, widget.Probe, new ValueKey<string>(id))).ToList();
        children.Insert(0, new DependencyReader(widget.Probe));
        children.Add(new Container(width: 160, height: 60, color: new UiColor(0xff2f80ed)));
        children.Add(new Text("Directionality · ColoredBox · Container · Flex · Text", locale: new Locale("en", "US")));
        Widget content = new TraceSingle(_phase, new TraceInherited(_phase, new Flex(
            direction: Axis.vertical,
            crossAxisAlignment: Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start,
            spacing: 12,
            children: children)), widget.Probe);
        if (_phase >= 4)
        {
            content = new SingleChildScrollView(primary: false, restorationId: "g6-live-scroll", child: content);
        }
        content = new Container(padding: EdgeInsets.CreateAll(24), child: content);
        content = new Semantics(
            identifier: "g6-live-action",
            label: WidgetsLiveEntrypoint.SemanticsLabel,
            button: true,
            focusable: true,
            onTap: () => widget.Probe.SemanticsTapCount++,
            onFocus: () => widget.Probe.SemanticsFocusCount++,
            child: new Focus(autofocus: false, onFocusChange: focused => widget.Probe.Event($"focus:{focused.ToString().ToLowerInvariant()}"), child: content));
        return new Directionality(textDirection: TextDirection.ltr, child: new DefaultTextStyle(
            style: new Doroti.Generated.Framework.Painting.TextStyle(color: new UiColor(0xfff7f7fb), fontSize: 20),
            child: new ColoredBox(color: new UiColor(0xff172033), child: content)));
    }

    public override void dispose() { widget.Probe.Event("root:dispose"); base.dispose(); }
}

sealed class TraceTile(string id, int phase, WidgetsLiveProbe probe, Key key) : StatefulWidget(key)
{
    public string Id { get; } = id;
    public int Phase { get; } = phase;
    public WidgetsLiveProbe Probe { get; } = probe;
    public override IState createState() => new TraceTileState();
}

sealed class TraceTileState : State<TraceTile>
{
    public override void initState()
    {
        base.initState();
        widget.Probe.States[widget.Id] = this;
        if (widget.Id == "B" && widget.Probe.OriginalB is null) widget.Probe.OriginalB = this;
        widget.Probe.Event($"tile:{widget.Id}:init");
    }
    public override Widget build(BuildContext context) => new Text($"{widget.Id}: phase {widget.Phase}", locale: new Locale("en", "US"));
    public override void didUpdateWidget(TraceTile oldWidget)
    {
        widget.Probe.Event($"tile:{widget.Id}:update:{widget.Phase}");
        if (widget.Id == "B" && widget.Phase >= 2) widget.Probe.SameBStateAfterReorder = ReferenceEquals(this, widget.Probe.OriginalB);
        base.didUpdateWidget(oldWidget);
    }
    public override void deactivate() { widget.Probe.Event($"tile:{widget.Id}:deactivate"); base.deactivate(); }
    public override void dispose()
    {
        widget.Probe.Event($"tile:{widget.Id}:dispose");
        if (widget.Id == "A") widget.Probe.ADisposed = true;
        base.dispose();
    }
}

sealed class TraceSingle(int phase, Widget child, WidgetsLiveProbe probe) : StatefulWidget
{
    public int Phase { get; } = phase;
    public Widget Child { get; } = child;
    public WidgetsLiveProbe Probe { get; } = probe;
    public override IState createState() => new TraceSingleState();
}
sealed class TraceSingleState : State<TraceSingle>
{
    public override Widget build(BuildContext context) => widget.Child;
    public override void didUpdateWidget(TraceSingle oldWidget) { widget.Probe.Event($"single:update:{widget.Phase}"); base.didUpdateWidget(oldWidget); }
}
sealed class TraceInherited(int value, Widget child) : InheritedWidget(child)
{
    public int Value { get; } = value;
    public override bool updateShouldNotify(InheritedWidget oldWidget) => oldWidget is TraceInherited old && old.Value != Value;
}
sealed class DependencyReader(WidgetsLiveProbe probe) : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        probe.Event($"dependency:{context.dependOnInheritedWidgetOfExactType<TraceInherited>()?.Value}");
        return new Text("Inherited dependency", locale: new Locale("en", "US"));
    }
}

static class NativeMethods
{
    [DllImport("user32.dll")] internal static extern bool ShowWindow(nint hwnd, int command);
    [DllImport("user32.dll")] internal static extern bool SetForegroundWindow(nint hwnd);
}
