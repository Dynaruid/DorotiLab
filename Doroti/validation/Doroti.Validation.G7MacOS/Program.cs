using System.Text.Json;
using Doroti.Backends.Skia;
using Doroti.Composition;
using Doroti.Engine;
using Doroti.Graphics;
using Doroti.Host.Desktop;
using Doroti.Platform;
using Doroti.Vendor.Avalonia.Native;

if (!OperatingSystem.IsMacOS()) throw new PlatformNotSupportedException("G7 macOS live probe requires macOS.");
var evidencePath = args.Length == 2 && args[0] == "--evidence" ? Path.GetFullPath(args[1]) : null;
var trace = new List<string>();
var windowSink = new WindowSink(trace);
using var backend = new DesktopWindowBackend(MacOsShellPlatformFactory.Create());
using var window = backend.CreateWindow(new("Doroti G7 macOS native probe", new(480, 320)), windowSink);
var raw = new RawSink(trace);
window.RawInput.Attach(raw);
var text = new TextClient(trace);
window.TextInput.SetClient(text, new("", new(0, 0), null));
window.Show();
Pump(backend, 10);

window.Resize(new(520, 360)); Pump(backend, 5);
window.SetMinimized(true); Pump(backend, 5);
window.SetMinimized(false); Pump(backend, 5);

var input = Require<IWindowInputTestController>(window);
input.PostPointerMove(new(30.25, 40.5));
input.PostPointerDrag(new(30.25, 40.5), new(82.75, 93.125));
input.PostPointerWheel(new(82.75, 93.125), new(0.5, -1.25));
input.PostPointerCaptureLoss(new(82.75, 93.125));
input.PostKeyboardActivation(49);
input.PostTextInput("Doroti 한글");
Pump(backend, 5);

window.Cursor.SetCursor(window.Id, CursorKind.Click);
window.Cursor.SetCursor(window.Id, CursorKind.Text);
window.Cursor.SetCursor(window.Id, CursorKind.Basic);

var clipboard = Require<IClipboard>(window);
var previousClipboard = clipboard.GetTextAsync().AsTask().GetAwaiter().GetResult();
var sentinel = $"doroti-g7-{Guid.NewGuid():N}";
var clipboardWrite = clipboard.SetTextAsync(sentinel).AsTask().GetAwaiter().GetResult();
var clipboardRead = clipboard.GetTextAsync().AsTask().GetAwaiter().GetResult();
var clipboardRestore = previousClipboard.Success
    ? clipboard.SetTextAsync(previousClipboard.Text ?? string.Empty).AsTask().GetAwaiter().GetResult()
    : previousClipboard;

var accessibility = Require<IAccessibilityBridge>(window);
var accessibilityDiagnostics = Require<IAccessibilityDiagnostics>(window);
var semanticsInvoked = false;
accessibility.Update(new(1, new(7, SemanticsRole.Button, "G7 macOS action", null,
    SemanticsState.Enabled, SemanticsAction.Tap, new(10, 10, 150, 60), [])), request =>
{
    semanticsInvoked = request.NodeId == 7 && request.Action == SemanticsAction.Tap;
    trace.Add("semantics-action");
    return semanticsInvoked;
});
if (!accessibilityDiagnostics.InvokeAction(7, SemanticsAction.Tap)) throw new InvalidDataException("NSAccessibility action bridge rejected the action.");

Exception? rasterFailure = null;
string renderer = "";
string version = "";
var hardware = false;
long nonEmptyPixels = 0;
var raster = new Thread(() =>
{
    try
    {
        using var surface = SkiaSurfaceFactory.CreateHardware(window).Surface;
        using var frame = surface.BeginFrame();
        frame.Clear(Color.FromArgb(255, 13, 21, 38));
        frame.Canvas.DrawRect(new(20, 20, 200, 120), new(Color.FromArgb(255, 80, 60, 200)));
        if (frame is not IPixelReadableSurfaceFrame readable) throw new InvalidDataException("GPU readback capability missing.");
        var width = checked((int)frame.PixelSize.Width);
        var height = checked((int)frame.PixelSize.Height);
        var pixels = new byte[checked(width * height * 4)];
        if (!readable.TryReadPixels(pixels, width * 4)) throw new InvalidDataException("GPU readback failed.");
        nonEmptyPixels = pixels.Chunk(4).LongCount(pixel => pixel[3] != 0);
        frame.Present();
        trace.Add("strict-gpu-present");

        var target = Require<IOpenGlWindowTarget>(window);
        using var context = target.CreateContext();
        renderer = context.Renderer; version = context.Version; hardware = context.IsHardwareAccelerated;
    }
    catch (Exception exception) { rasterFailure = exception; }
});
raster.Start(); raster.Join();
if (rasterFailure is not null) throw new InvalidOperationException("Raster probe failed.", rasterFailure);

window.Close(); Pump(backend, 10);
var resources = backend.CaptureResourceSnapshot();
var failures = new List<string>();
if (nonEmptyPixels == 0) failures.Add("strict-GPU first frame was empty");
if (!hardware) failures.Add("OpenGL renderer was not hardware accelerated");
if (!raw.Events.Contains("pointer-wheel") || !raw.Events.Contains("key-down")) failures.Add("native input trace incomplete");
if (!text.States.Any(state => state.Text.Contains("Doroti 한글", StringComparison.Ordinal))) failures.Add("native text trace incomplete");
if (!clipboardWrite.Success || !clipboardRead.Success || clipboardRead.Text != sentinel || !clipboardRestore.Success) failures.Add("NSPasteboard round trip failed");
if (!semanticsInvoked) failures.Add("NSAccessibility action trace incomplete");
if (!windowSink.Closed || windowSink.MetricsChanges < 2 || raw.FocusChanges < 1) failures.Add("AppKit lifecycle trace incomplete");
if (!resources.IsBalanced) failures.Add("native resources did not return to baseline");

var evidence = new
{
    schemaVersion = "doroti.g7-macos-live-probe/v1", milestone = "G7-3M", capturedAtUtc = DateTimeOffset.UtcNow,
    status = failures.Count == 0 ? "pass" : "failed",
    target = new { rid = "osx-arm64", nativeHandle = "NSWindow", scale = window.Metrics.ScaleFactor, metricsGeneration = window.Metrics.Generation },
    gpu = new { backend = "skia-nsopengl-opengl-gpu", renderer, version, hardware, softwareFallbackUsed = false, nonEmptyPixels, terminalPresent = true },
    lifecycle = new { windowSink.MetricsChanges, raw.FocusChanges, windowSink.Closed },
    input = new { events = raw.Events, raw.LastScrollDelta, textStates = text.States },
    clipboard = new { write = clipboardWrite.Success, read = clipboardRead.Success, restored = clipboardRestore.Success },
    accessibility = new { nodeId = 7, action = "tap", invoked = semanticsInvoked },
    resources, trace,
    notVerified = new[] { "Korean IME physical candidate-window placement", "VoiceOver physical navigation", "precise trackpad physical gesture" },
    failures,
};
var json = JsonSerializer.Serialize(evidence, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true }) + "\n";
if (evidencePath is not null) { Directory.CreateDirectory(Path.GetDirectoryName(evidencePath)!); File.WriteAllText(evidencePath, json); }
Console.WriteLine($"G7_MAC_LIVE={JsonSerializer.Serialize(evidence)}");
return failures.Count == 0 ? 0 : 1;

static T Require<T>(IWindow window) where T : class =>
    window.TryGetFeature<T>(out var value) && value is not null ? value : throw new NotSupportedException(typeof(T).FullName);
static void Pump(DesktopWindowBackend backend, int count) { for (var i = 0; i < count; i++) backend.PumpPendingMessages(); }

sealed class WindowSink(List<string> trace) : IWindowEventSink
{
    public int MetricsChanges { get; private set; }
    public bool Closed { get; private set; }
    public void OnMetricsChanged(WindowId window, WindowMetrics metrics) { MetricsChanges++; trace.Add("metrics"); }
    public void OnCloseRequested(WindowId window) => trace.Add("close-requested");
    public void OnClosed(WindowId window) { Closed = true; trace.Add("closed"); }
}

sealed class RawSink(List<string> trace) : IRawInputSink
{
    public List<string> Events { get; } = [];
    public Offset LastScrollDelta { get; private set; }
    public int FocusChanges { get; private set; }
    public void OnPointer(RawPointerEvent input)
    {
        var name = input.ScrollDelta != Offset.Zero ? "pointer-wheel" : $"pointer-{input.Phase.ToString().ToLowerInvariant()}";
        Events.Add(name); trace.Add(name);
        if (input.ScrollDelta != Offset.Zero) LastScrollDelta = input.ScrollDelta;
    }
    public void OnKey(RawKeyEvent input) { var name = $"key-{input.Phase.ToString().ToLowerInvariant()}"; Events.Add(name); trace.Add(name); }
    public void OnFocus(RawFocusEvent input) { FocusChanges++; trace.Add(input.IsFocused ? "focus" : "blur"); }
}

sealed class TextClient(List<string> trace) : ITextInputClient
{
    public List<TextEditingState> States { get; } = [];
    public void UpdateEditingState(TextEditingState state) { States.Add(state); trace.Add("text-state"); }
    public void PerformAction(TextInputAction action) => trace.Add($"text-action-{action}");
}
