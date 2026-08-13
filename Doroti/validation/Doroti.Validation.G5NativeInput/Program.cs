using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Doroti.Core;
using Doroti.Graphics;
using Doroti.Host.Desktop;
using Doroti.Platform;

if (!OperatingSystem.IsWindows())
{
    Console.Error.WriteLine("G5-1 native input validation requires Windows.");
    return 2;
}

var outputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "artifacts", "g5-1", "native-input.json"));
var failures = new List<string>();
var input = new InputSink();
var windowEvents = new WindowSink();
var frameTimestamps = new List<TimeSpan>();
double[] intervals = [];
NativeResourceSnapshot resourceSnapshot;
WindowMetrics metrics;

using (var backend = new DesktopWindowBackend())
using (var window = backend.CreateWindow(new("Doroti G5-1 native input validation", new(480, 320)), windowEvents))
{
    window.RawInput.Attach(input);
    window.Show();
    backend.PumpPendingMessages();
    metrics = window.Metrics;

    Require(window.TryGetFeature<IWindowInputTestController>(out var controller) && controller is not null,
        "Native window does not expose its target input controller.", failures);
    Require(window.TryGetFeature<IWindowCoordinateDiagnostics>(out var coordinates) && coordinates is not null,
        "Native window does not expose coordinate diagnostics.", failures);
    Require(window.TryGetFeature<IFrameDispatcher>(out var frames) && frames is not null,
        "Native window does not expose the source-ported frame dispatcher.", failures);
    Require(window.TryGetFeature<INativeResourceDiagnostics>(out var resources) && resources is not null,
        "Native window does not expose resource diagnostics.", failures);

    if (controller is not null)
    {
        controller.PostPointerTap(new(37.25, 42.5));
        controller.PostPointerDrag(new(52.5, 80.25), new(52.5, 24.75));
        controller.PostPointerWheel(new(61.5, 73.25), new(0, 1));
        controller.PostPointerCaptureLoss(new(91.5, 101.25));
        PumpUntil(backend, () => input.PointerEvents.Count >= 14, TimeSpan.FromSeconds(3));
    }

    var downEvents = input.PointerEvents.Where(item => item.Phase == PointerPhase.Down).ToArray();
    var upEvents = input.PointerEvents.Where(item => item.Phase == PointerPhase.Up).ToArray();
    var cancelEvents = input.PointerEvents.Where(item => item.Phase == PointerPhase.Cancelled).ToArray();
    var wheelEvents = input.PointerEvents.Where(item => item.ScrollDelta != Offset.Zero).ToArray();
    Require(downEvents.Length == 3, $"Expected three native pointer-down events, got {downEvents.Length}.", failures);
    Require(upEvents.Length == 2, $"Expected two native pointer-up events, got {upEvents.Length}.", failures);
    Require(cancelEvents.Length == 1, $"Capture loss must emit one cancel, got {cancelEvents.Length}.", failures);
    Require(wheelEvents.Length == 1, $"Wheel input must emit exactly once, got {wheelEvents.Length}.", failures);
    Require(input.PointerEvents.All(item => item.DeviceKind == PointerDeviceKind.Mouse),
        "Posted Win32 mouse packets changed device kind.", failures);
    Require(IsMonotonic(input.PointerEvents.Select(item => item.Timestamp)),
        "Native message timestamps are not monotonic.", failures);
    Require(ContainsPosition(input.PointerEvents, new(37.25, 42.5), metrics.ScaleFactor),
        "Tap logical coordinates exceeded the pixel-rounding tolerance.", failures);
    Require(ContainsPosition(input.PointerEvents, new(52.5, 24.75), metrics.ScaleFactor),
        "Drag logical coordinates exceeded the pixel-rounding tolerance.", failures);
    Require(wheelEvents.Length == 1 && Math.Abs(wheelEvents[0].ScrollDelta.Y + 100) < 1e-9,
        "Win32 wheel delta was not normalized to logical pixels.", failures);

    if (frames is not null)
    {
        void OnFrame(TimeSpan timestamp)
        {
            frameTimestamps.Add(timestamp);
            if (frameTimestamps.Count < 180)
            {
                frames.ScheduleFrame(OnFrame);
            }
        }
        frames.ScheduleFrame(OnFrame);
        PumpUntil(backend, () => frameTimestamps.Count >= 180, TimeSpan.FromSeconds(8));
    }
    Require(frameTimestamps.Count == 180, $"Expected 180 sustained native frames, got {frameTimestamps.Count}.", failures);
    intervals = frameTimestamps.Zip(frameTimestamps.Skip(1), (left, right) => (right - left).TotalMilliseconds).ToArray();
    var p95 = Percentile(intervals, 0.95);
    Require(intervals.Length == 179 && intervals.All(value => value > 0), "Native frame timestamps did not advance.", failures);
    Require(p95 <= 25, $"Native frame pacing p95 {p95:0.###}ms exceeded 25ms.", failures);

    metrics = window.Metrics;
    var coordinateSnapshot = coordinates?.Coordinates;
    Require(coordinateSnapshot is not null && coordinateSnapshot.Value.Generation == metrics.Generation,
        "Pointer coordinates and window metrics do not share a generation.", failures);

    window.RawInput.Detach(input);
    window.Close();
    backend.PumpPendingMessages();
    resourceSnapshot = resources?.Snapshot ?? default;
}

Require(resourceSnapshot.IsBalanced, "Native window resources were not balanced after close.", failures);

var touchFlags = GetSystemMetrics(94);
var maximumTouches = GetSystemMetrics(95);
var touchDigitizerPresent = (touchFlags & 0x80) != 0 && (touchFlags & 0x03) != 0;
var evidence = new
{
    schemaVersion = "doroti.g5-1-native-input/v1",
    capturedAtUtc = DateTimeOffset.UtcNow,
    success = failures.Count == 0,
    environment = new
    {
        operatingSystem = RuntimeInformation.OSDescription,
        architecture = RuntimeInformation.ProcessArchitecture.ToString(),
        backend = "Doroti.Vendor.Avalonia.Win32 source-port",
        source = "Win32 PostMessage target-controller through an actual HWND",
        physicalMousePresent = GetSystemMetrics(19) != 0,
        touchDigitizerPresent,
        maximumTouches,
    },
    nativeWindow = new
    {
        metrics.LogicalSize,
        metrics.PixelSize,
        metrics.ScaleFactor,
        pointerEventCount = input.PointerEvents.Count,
        pointerDownCount = input.PointerEvents.Count(item => item.Phase == PointerPhase.Down),
        pointerUpCount = input.PointerEvents.Count(item => item.Phase == PointerPhase.Up),
        pointerCancelCount = input.PointerEvents.Count(item => item.Phase == PointerPhase.Cancelled),
        wheelEventCount = input.PointerEvents.Count(item => item.ScrollDelta != Offset.Zero),
        timestampMonotonic = IsMonotonic(input.PointerEvents.Select(item => item.Timestamp)),
        coordinateToleranceLogicalPixels = 0.5 / metrics.ScaleFactor,
    },
    framePacing = new
    {
        frameCount = frameTimestamps.Count,
        durationMilliseconds = frameTimestamps.Count > 1 ? (frameTimestamps[^1] - frameTimestamps[0]).TotalMilliseconds : 0,
        intervalP50Milliseconds = Percentile(intervals, 0.50),
        intervalP95Milliseconds = Percentile(intervals, 0.95),
        intervalP99Milliseconds = Percentile(intervals, 0.99),
    },
    resources = resourceSnapshot,
    evidenceBoundary = new
    {
        mouse = "verified-native-window-synthetic-source",
        trackpad = "not-verified-physical-event",
        touch = maximumTouches > 0 ? "not-verified-physical-event" : "not-verified-device-unavailable",
        tickerLifecycle = "verified-managed-by-G4-SchedulerServices; native-frame-source-verified-here",
    },
    failures,
};
Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
var temporaryPath = outputPath + ".tmp-" + Guid.NewGuid().ToString("N");
File.WriteAllText(temporaryPath, JsonSerializer.Serialize(evidence, new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
}) + "\n", new UTF8Encoding(false));
File.Move(temporaryPath, outputPath, true);
Console.WriteLine($"G5-1 native input/window/frame validation: {(failures.Count == 0 ? "PASS" : "FAIL")}");
foreach (var failure in failures) Console.WriteLine($"  {failure}");
Console.WriteLine($"Evidence: {outputPath}");
return failures.Count == 0 ? 0 : 1;

static void PumpUntil(DesktopWindowBackend backend, Func<bool> completed, TimeSpan timeout)
{
    var stopwatch = Stopwatch.StartNew();
    while (!completed() && stopwatch.Elapsed < timeout)
    {
        backend.PumpPendingMessages();
        Thread.Sleep(1);
    }
    backend.PumpPendingMessages();
}

static bool ContainsPosition(IEnumerable<RawPointerEvent> events, Offset expected, double scale)
{
    var tolerance = 0.5 / scale + 1e-9;
    return events.Any(item => Math.Abs(item.Position.X - expected.X) <= tolerance && Math.Abs(item.Position.Y - expected.Y) <= tolerance);
}

static bool IsMonotonic(IEnumerable<TimeSpan> values)
{
    TimeSpan? previous = null;
    foreach (var value in values)
    {
        if (previous is not null && value < previous.Value) return false;
        previous = value;
    }
    return true;
}

static double Percentile(double[] values, double percentile)
{
    if (values.Length == 0) return 0;
    var ordered = values.Order().ToArray();
    return ordered[(int)Math.Ceiling(percentile * ordered.Length) - 1];
}

static void Require(bool condition, string failure, List<string> failures)
{
    if (!condition) failures.Add(failure);
}

[DllImport("user32.dll")]
static extern int GetSystemMetrics(int index);

sealed class InputSink : IRawInputSink
{
    public List<RawPointerEvent> PointerEvents { get; } = [];
    public void OnPointer(RawPointerEvent input) => PointerEvents.Add(input);
    public void OnKey(RawKeyEvent input) { }
}

sealed class WindowSink : IWindowEventSink
{
    public void OnMetricsChanged(WindowId window, WindowMetrics metrics) { }
    public void OnCloseRequested(WindowId window) { }
    public void OnClosed(WindowId window) { }
}
