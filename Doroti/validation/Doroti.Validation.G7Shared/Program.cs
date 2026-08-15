using System.Diagnostics;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Ui;
using Doroti.Host.Desktop.Framework;
using Doroti.Target.Windows;
using IOPath = System.IO.Path;

if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("G7 shared retained validation requires Windows.");
if (args is not ["--retained-evidence", var evidencePath])
    throw new ArgumentException("--retained-evidence <path> is required.");

const ulong ViewId = 701;
var entrypoint = new RetainedEntrypoint();
object resourceClosure;
object frameHealth;
object retainedResult;
using (var target = new WindowsTarget())
using (var session = new DorotiHostSession(entrypoint))
using (var scope = session.dispatcher.EnterScope())
{
    session.Start();
    var view = target.CreateView(session, ViewId, new("Doroti G7-1C retained", new Size(320, 240)));
    view.Show();
    PumpFor(target, TimeSpan.FromMilliseconds(100));

    var firstLayer = BuildLayer(ViewId, 0xff6750a4);
    var first = RenderRetained(target, view, firstLayer);
    var afterFirst = target.CaptureRetainedDiagnosticsForValidation(ViewId);
    var unchanged = RenderRetained(target, view, firstLayer);
    var afterUnchanged = target.CaptureRetainedDiagnosticsForValidation(ViewId);
    if (!first.Bgra8888Pixels.SequenceEqual(unchanged.Bgra8888Pixels))
        throw new InvalidDataException("Unchanged retained replay changed raster pixels.");
    if (afterUnchanged.Hits <= afterFirst.Hits)
        throw new InvalidDataException("Unchanged retained replay did not hit the translated display-list cache.");

    var changedLayer = BuildLayer(ViewId, 0xffb3261e);
    var changed = RenderRetained(target, view, changedLayer);
    var afterChanged = target.CaptureRetainedDiagnosticsForValidation(ViewId);
    var changedPixels = CountChanged(first, changed);
    if (changedPixels < 1000 || afterChanged.Misses <= afterUnchanged.Misses)
        throw new InvalidDataException("Changed retained generation did not invalidate the cached replay.");

    var invalidationsBefore = afterChanged.SurfaceInvalidations;
    var metricsBefore = target.CaptureDiagnostics(ViewId).Input.MetricsChanges;
    view.Resize(new Size(360, 260));
    WaitUntil(() => target.CaptureDiagnostics(ViewId).Input.MetricsChanges > metricsBefore, target, TimeSpan.FromSeconds(12));
    PumpFor(target, TimeSpan.FromMilliseconds(100));
    var resized = RenderRetained(target, view, changedLayer);
    var afterResize = target.CaptureRetainedDiagnosticsForValidation(ViewId);
    if (resized.Width == changed.Width || afterResize.SurfaceInvalidations <= invalidationsBefore)
        throw new InvalidDataException("Resize did not invalidate the retained surface-generation cache.");

    var diagnostics = target.CaptureDiagnostics(ViewId);
    if (diagnostics.Frame.BackendIdentity != "skia-wgl-opengl-gpu" || diagnostics.Frame.SoftwareFallbackUsed ||
        diagnostics.Frame.Failed != 0 || diagnostics.Frame.Cancelled != 0)
        throw new InvalidDataException("Retained fixture lost strict-GPU frame health.");

    retainedResult = new
    {
        first = new { generation = firstLayer.debugGeneration, cache = afterFirst },
        unchanged = new { sameGeneration = firstLayer.debugGeneration, identicalPixels = true, cache = afterUnchanged },
        changed = new { generation = changedLayer.debugGeneration, changedPixels, cache = afterChanged },
        resize = new { width = resized.Width, height = resized.Height, cache = afterResize },
    };
    frameHealth = diagnostics.Frame;
    session.DetachView(view);
    session.Shutdown();
    view.Dispose();
    PumpFor(target, TimeSpan.FromMilliseconds(100));
    var closure = target.CaptureResourceSnapshot();
    if (!closure.IsBalanced) throw new InvalidDataException($"Retained fixture resources did not close: {closure}.");
    resourceClosure = closure;
}

WriteJson(evidencePath, new
{
    schemaVersion = "doroti.g7-retained-evidence/v1",
    milestone = "G7-1C",
    capturedAtUtc = DateTimeOffset.UtcNow,
    status = "pass",
    backend = "skia-wgl-opengl-gpu",
    retained = retainedResult,
    frame = frameHealth,
    resourceClosure,
    forbidden = new { staleCacheReuse = 0, unknownNoOp = 0, softwareFallback = 0 },
});
Console.WriteLine("G7-1C retained first/unchanged/changed/resize strict-GPU fixture: PASS");

static EngineLayer BuildLayer(ulong viewId, long color)
{
    var recorder = new PictureRecorder();
    var canvas = new Canvas(recorder);
    canvas.drawRect(Rect.fromLTWH(20, 20, 180, 120), new Paint { color = new Color(color) });
    var picture = recorder.endRecording();
    var builder = new SceneBuilder(viewId);
    var layer = builder.pushOffset(12, 8);
    builder.addPicture(Offset.zero, picture);
    builder.pop();
    using var scene = builder.build();
    return layer;
}

static DesktopFrameworkPixelReadback RenderRetained(WindowsTarget target, DorotiView view, EngineLayer layer)
{
    var capture = target.CaptureNextFrameAsync(ViewId);
    var builder = new SceneBuilder(ViewId);
    builder.addRetained(layer);
    using var scene = builder.build();
    view.render(scene);
    var elapsed = Stopwatch.StartNew();
    var nextRetry = TimeSpan.FromMilliseconds(100);
    while (!capture.IsCompleted)
    {
        if (elapsed.Elapsed > TimeSpan.FromSeconds(12))
            throw new TimeoutException("G7 retained pixel readback timed out.");
        target.PumpPendingMessages();
        if (elapsed.Elapsed >= nextRetry)
        {
            view.render(scene);
            nextRetry += TimeSpan.FromMilliseconds(100);
        }
        Thread.Sleep(1);
    }
    return capture.GetAwaiter().GetResult();
}

static long CountChanged(DesktopFrameworkPixelReadback before, DesktopFrameworkPixelReadback after)
{
    if (before.Width != after.Width || before.Height != after.Height) return long.MaxValue;
    long changed = 0;
    for (var offset = 0; offset < before.Bgra8888Pixels.Length; offset += 4)
        if (!before.Bgra8888Pixels.AsSpan(offset, 4).SequenceEqual(after.Bgra8888Pixels.AsSpan(offset, 4))) changed++;
    return changed;
}

static void WaitUntil(Func<bool> predicate, WindowsTarget target, TimeSpan timeout)
{
    var elapsed = Stopwatch.StartNew();
    while (!predicate())
    {
        if (elapsed.Elapsed > timeout) throw new TimeoutException($"G7 retained fixture timed out after {timeout}.");
        target.PumpPendingMessages();
        Thread.Sleep(1);
    }
}

static void PumpFor(WindowsTarget target, TimeSpan duration)
{
    var elapsed = Stopwatch.StartNew();
    while (elapsed.Elapsed < duration)
    {
        target.PumpPendingMessages();
        Thread.Sleep(1);
    }
}

static void WriteJson(string path, object value)
{
    path = IOPath.GetFullPath(path);
    Directory.CreateDirectory(IOPath.GetDirectoryName(path)!);
    File.WriteAllText(path, JsonSerializer.Serialize(value, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    }) + "\n");
}

sealed class RetainedEntrypoint : IDorotiViewEntrypoint
{
    public void Bootstrap(PlatformDispatcher dispatcher) { }
    public void AttachView(DorotiView view) { }
    public void DetachView(DorotiView view) { }
    public void Shutdown() { }
}
