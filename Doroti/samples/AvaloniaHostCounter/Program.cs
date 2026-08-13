using System.Security.Cryptography;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Doroti.Composition;
using Doroti.Core;
using Doroti.Graphics;
using Doroti.Host.Avalonia;
using Doroti.Platform;
using Doroti.Rendering;
using Size = Doroti.Graphics.Size;

var options = SampleOptions.Parse(args);
SampleApp.Options = options;

var appBuilder = AppBuilder.Configure<SampleApp>()
    .UsePlatformDetect()
    .LogToTrace();
if (OperatingSystem.IsWindows())
{
    appBuilder = appBuilder.With(new Win32PlatformOptions
    {
        RenderingMode = options.RenderingMode == AvaloniaHostRenderingMode.Hardware
            ? [Win32RenderingMode.AngleEgl]
            : [Win32RenderingMode.Software],
        CompositionMode = [Win32CompositionMode.RedirectionSurface],
    });
}

return appBuilder.StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);

internal sealed class SampleApp : Application
{
    internal static SampleOptions Options { get; set; } = null!;

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            throw new InvalidOperationException("The H1 sample requires a classic desktop lifetime.");
        }
        Dispatcher.UIThread.Post(async () =>
        {
            var exitCode = 1;
            try
            {
                exitCode = await RunTargetAsync(Options);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
            }
            finally
            {
                desktop.Shutdown(exitCode);
            }
        }, DispatcherPriority.Loaded);
    }

    private static async Task<int> RunTargetAsync(SampleOptions options)
    {
        Directory.CreateDirectory(options.ArtifactDirectory);
        var tracePath = Path.Combine(options.ArtifactDirectory, "avalonia-host-trace.json");
        var frameTracePath = Path.Combine(options.ArtifactDirectory, "frame-trace.json");
        var screenshotPath = Path.Combine(options.ArtifactDirectory, "display-list-scene.png");
        var reportPath = Path.Combine(options.ArtifactDirectory, "runtime-report.json");
        var backend = new AvaloniaWindowBackend(options.RenderingMode);
        var fixture = H2FrameFixture.Load();
        var disposedSnapshots = new List<AvaloniaFramePipelineSnapshot>();
        AvaloniaPixelReadback? readback = null;
        string? screenshotSha256 = null;
        string? readbackSha256 = null;
        DisplayList? evidenceDisplayList = null;
        var dpiChanged = false;
        var ackStatuses = new List<FrameAckStatus>();

        for (var cycle = 0; cycle < 5; cycle++)
        {
            var sink = new RecordingWindowSink();
            var window = backend.CreateWindow(new($"Doroti H2 Avalonia Host {cycle + 1}", new(640, 420)), sink);
            RequireFeature(window, out IAvaloniaFramePipeline pipeline);
            RequireFeature(window, out IAvaloniaFrameTestController controller);
            RequireFeature(window, out IAvaloniaDisplayListPresenter presenter);
            RequireFeature(window, out IFrameDispatcher frameDispatcher);
            window.Show();
            await WaitForAsync(() => sink.Metrics.Count > 0, "initial metrics");

            if (cycle == 0)
            {
                var initialMetrics = window.Metrics;
                window.Resize(new(760, 500));
                await WaitForAsync(
                    () => backend.Diagnostics.Snapshot.Events.Any(item => item.Window == window.Id && item.Kind == "resized" && item.Metrics.LogicalSize != initialMetrics.LogicalSize),
                    "resize trace");
                window.SetMinimized(true);
                await WaitForAsync(() => backend.Diagnostics.Snapshot.Events.Any(item => item.Window == window.Id && item.Kind == "minimized"), "minimize trace");
                window.SetMinimized(false);
                await WaitForAsync(() => backend.Diagnostics.Snapshot.Events.Any(item => item.Window == window.Id && item.Kind == "restored" && !item.Metrics.IsMinimized), "restore trace");
                dpiChanged = await TryExerciseDpiChangeAsync(window, backend.Diagnostics);
            }

            var image = pipeline.RegisterImage(fixture.ImageWidth, fixture.ImageHeight, fixture.ImagePixels);
            var displayList = CreateScene(window.Metrics.PixelSize, image);
            evidenceDisplayList ??= displayList;
            var warmAck = await PresentRetryStaleAsync(pipeline, displayList);
            Require(warmAck.Status == FrameAckStatus.Presented, "Warm frame did not present.");
            ackStatuses.Add(warmAck.Status);

            if (cycle == 0)
            {
                var tickCount = 0;
                var tick = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                var scheduler = new FrameSchedulerPort(new SampleClock(), frameDispatcher);
                scheduler.BeginFrame += _ =>
                {
                    tickCount++;
                    tick.TrySetResult();
                };
                scheduler.ScheduleFrame();
                scheduler.ScheduleFrame();
                scheduler.ScheduleFrame();
                await tick.Task.WaitAsync(TimeSpan.FromSeconds(5));
                Require(tickCount == 1, "Repeated invalidation produced more than one frame-clock tick.");

                var capture = pipeline.CaptureNextFrameAsync();
                var capturedAck = await pipeline.PresentAsync(displayList);
                Require(capturedAck.Status == FrameAckStatus.Presented, "Captured frame did not present.");
                ackStatuses.Add(capturedAck.Status);
                readback = await capture.WaitAsync(TimeSpan.FromSeconds(5));

                controller.PauseNextPresent();
                var first = pipeline.PresentAsync(displayList);
                Require(controller.WaitForPausedPresent(TimeSpan.FromSeconds(5)), "The first bounded-mailbox frame did not reach present.");
                var second = pipeline.PresentAsync(displayList);
                var third = pipeline.PresentAsync(displayList);
                controller.ResumePresent();
                var coalesced = await Task.WhenAll(first, second, third);
                Require(coalesced[0].Status == FrameAckStatus.Presented && coalesced[1].Status == FrameAckStatus.Superseded && coalesced[2].Status == FrameAckStatus.Presented, "Bounded mailbox did not keep one in-flight and the latest pending frame.");
                ackStatuses.AddRange(coalesced.Select(item => item.Status));

                controller.StaleNextPresent();
                var stale = await pipeline.PresentAsync(displayList);
                Require(stale.Status == FrameAckStatus.Stale && stale.FaultKind == FrameFaultKind.Stale, "Forced stale frame did not receive a stale ACK.");
                ackStatuses.Add(stale.Status);

                controller.FailNextPresent();
                var failed = await pipeline.PresentAsync(displayList);
                Require(failed.Status == FrameAckStatus.Failed && failed.FaultKind == FrameFaultKind.RecoverableSurfaceLoss, "Present failure did not receive a recoverable failed ACK.");
                ackStatuses.Add(failed.Status);
                var recovered = await pipeline.PresentAsync(displayList);
                Require(recovered.Status == FrameAckStatus.Presented, "The frame after surface recovery did not present.");
                ackStatuses.Add(recovered.Status);
            }

            Require(pipeline.Snapshot.ActiveResourceLeases == 0, "A terminal ACK retained an image resource lease.");
            Require(pipeline.RemoveResource(image), "The image fixture could not be removed after its final ACK.");
            await WaitForAsync(() => pipeline.Snapshot.UploadResources.FramesImported > 0, "Avalonia frame import");
            if (cycle == 0)
            {
                readback ??= presenter.Capture();
                _ = presenter.Capture(screenshotPath);
                ValidateScenePixels(readback);
                screenshotSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(screenshotPath))).ToLowerInvariant();
                readbackSha256 = Convert.ToHexString(SHA256.HashData(readback.Bgra8888Pixels)).ToLowerInvariant();
            }
            window.Close();
            await WaitForAsync(() => window.IsClosed, "close trace");
            window.Dispose();
            if (cycle == 0)
            {
                pipeline.WriteFrameTrace(frameTracePath);
            }
            var disposed = pipeline.Snapshot;
            Require(disposed.SurfaceCreatedThreadId == disposed.RasterThreadId && disposed.SurfaceDisposedThreadId == disposed.RasterThreadId, "Doroti surface resources were not created and released on the raster thread.");
            Require(disposed.UploadResources.IsBalanced && disposed.UploadResources.ImportThreadId == disposed.UiThreadId, "Avalonia frame upload resources were not released on the UI thread.");
            Require(disposed.GpuResources.IsBalanced && disposed.ActiveResourceLeases == 0, "A disposed cycle retained GPU/frame resources or image leases.");
            disposedSnapshots.Add(disposed);
        }

        backend.Diagnostics.Write(tracePath);

        var trace = backend.Diagnostics.Snapshot;
        RequireTrace(trace, dpiChanged, options.VerifyTarget);
        Require(readback is not null && evidenceDisplayList is not null && screenshotSha256 is not null && readbackSha256 is not null, "H2 evidence capture is incomplete.");
        var verifiedReadback = readback!;
        var verifiedDisplayList = evidenceDisplayList!;
        var report = new
        {
            schemaVersion = "doroti.h2-avalonia-runtime-report/v1",
            status = "pass",
            renderingMode = options.RenderingMode.ToString().ToLowerInvariant(),
            strictAvaloniaRenderingMode = OperatingSystem.IsWindows()
                ? options.RenderingMode == AvaloniaHostRenderingMode.Hardware ? "angle-egl" : "software"
                : "platform-default",
            operatingSystem = Environment.OSVersion.VersionString,
            framework = Environment.Version.ToString(),
            displayList = new
            {
                commands = verifiedDisplayList.Commands.Count,
                bytes = verifiedDisplayList.ByteSize,
                bounds = verifiedDisplayList.Bounds,
                includesImageResource = true,
            },
            readback = new
            {
                width = verifiedReadback.PixelSize.Width,
                height = verifiedReadback.PixelSize.Height,
                verifiedReadback.RowBytes,
                sha256 = readbackSha256,
                screenshotSha256,
                screenshot = Path.GetFileName(screenshotPath),
            },
            frameContract = new
            {
                fixture = "migration/host/h2-frame-fixture.json",
                fixtureId = fixture.Id,
                singleFrameClock = "pass",
                queueHighWatermark = disposedSnapshots.Max(item => item.QueueHighWatermark),
                supersededFrames = disposedSnapshots.Sum(item => item.SupersededFrames),
                ackStatuses = ackStatuses.Select(item => item.ToString().ToLowerInvariant()).ToArray(),
                frameTrace = Path.GetFileName(frameTracePath),
                gpuContextOwnership = "official-avalonia-render-thread;doroti-contexts=0",
                surfaceThreadOwnership = disposedSnapshots.All(item => item.SurfaceCreatedThreadId == item.RasterThreadId && item.SurfaceDisposedThreadId == item.RasterThreadId) ? "pass" : "fail",
                uploadThreadOwnership = disposedSnapshots.All(item => item.UploadResources.ImportThreadId == item.UiThreadId) ? "pass" : "fail",
            },
            resourceBalance = new
            {
                cycles = disposedSnapshots.Count,
                activeResourceLeases = disposedSnapshots.Sum(item => item.ActiveResourceLeases),
                activeGpuContexts = disposedSnapshots.Sum(item => item.GpuResources.ActiveContexts),
                activeGpuFrames = disposedSnapshots.Sum(item => item.GpuResources.ActiveFrames),
                activeAvaloniaBitmaps = disposedSnapshots.Sum(item => item.UploadResources.ActiveBitmaps),
                bitmapsCreated = disposedSnapshots.Sum(item => item.UploadResources.BitmapsCreated),
                bitmapsReleased = disposedSnapshots.Sum(item => item.UploadResources.BitmapsReleased),
            },
            lifecycle = new
            {
                eventCount = trace.Events.Length,
                kinds = trace.Events.Select(item => item.Kind).Distinct(StringComparer.Ordinal).ToArray(),
                scaleFactors = trace.Events.Select(item => item.Metrics.ScaleFactor).Distinct().Order().ToArray(),
                dpiChanged,
            },
        };
        File.WriteAllText(reportPath, JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        }).Replace("\r\n", "\n", StringComparison.Ordinal) + "\n");
        Console.WriteLine($"H2 Avalonia {options.RenderingMode}: PASS");
        Console.WriteLine($"Trace: {tracePath}");
        Console.WriteLine($"Screenshot: {screenshotPath}");
        Console.WriteLine($"Report: {reportPath}");
        return 0;
    }

    private static DisplayList CreateScene(Size pixelSize, ResourceId image)
    {
        var width = Math.Max(1, pixelSize.Width);
        var height = Math.Max(1, pixelSize.Height);
        var builder = new DisplayListBuilder(new(0, 0, width, height));
        builder.DrawColor(new(0xFF142033));
        builder.DrawRect(new(width * 0.08, height * 0.14, width * 0.45, height * 0.58), new(new(0xFF41D19A)));
        builder.DrawRect(new(width * 0.52, height * 0.22, width * 0.91, height * 0.76), new(new(0xFF6F7BF7)));
        builder.DrawPath(new([
            new(width * 0.41, height * 0.68),
            new(width * 0.50, height * 0.84),
            new(width * 0.32, height * 0.84),
        ]), new(new(0xFFFFC857)));
        builder.DrawImage(image, new(0, 0, 2, 2), new(width * 0.72, height * 0.06, width * 0.88, height * 0.20));
        return builder.Build();
    }

    private static async Task<bool> TryExerciseDpiChangeAsync(IWindow window, IAvaloniaHostDiagnostics diagnostics)
    {
        if (!window.TryGetFeature<IWindowPlacementController>(out var placement) || placement is null || placement.Displays.Count < 2)
        {
            return false;
        }
        var initialScale = window.Metrics.ScaleFactor;
        foreach (var display in placement.Displays)
        {
            placement.MoveToDisplay(display.Id);
            var changed = await WaitForAsync(
                () => diagnostics.Snapshot.Events.Any(item => item.Kind == "dpi-changed" && Math.Abs(item.Metrics.ScaleFactor - initialScale) > 0.001),
                "DPI change trace",
                throwOnTimeout: false);
            if (changed)
            {
                return true;
            }
        }
        return false;
    }

    private static void ValidateScenePixels(AvaloniaPixelReadback readback)
    {
        var width = (int)readback.PixelSize.Width;
        var height = (int)readback.PixelSize.Height;
        AssertPixel(readback, (int)(width * 0.02), (int)(height * 0.02), 0x33, 0x20, 0x14, "background");
        AssertPixel(readback, (int)(width * 0.20), (int)(height * 0.30), 0x9A, 0xD1, 0x41, "green rectangle");
        AssertPixel(readback, (int)(width * 0.70), (int)(height * 0.40), 0xF7, 0x7B, 0x6F, "blue rectangle");
    }

    private static void AssertPixel(AvaloniaPixelReadback readback, int x, int y, byte blue, byte green, byte red, string label)
    {
        x = Math.Clamp(x, 0, (int)readback.PixelSize.Width - 1);
        y = Math.Clamp(y, 0, (int)readback.PixelSize.Height - 1);
        var offset = checked((y * readback.RowBytes) + (x * 4));
        var actual = readback.Bgra8888Pixels.AsSpan(offset, 4);
        if (Math.Abs(actual[0] - blue) > 2 || Math.Abs(actual[1] - green) > 2 || Math.Abs(actual[2] - red) > 2 || actual[3] < 253)
        {
            throw new InvalidDataException($"{label} pixel mismatch at ({x}, {y}): BGRA={actual[0]},{actual[1]},{actual[2]},{actual[3]}.");
        }
    }

    private static void RequireTrace(AvaloniaHostTraceDocument trace, bool dpiChanged, bool verifyTarget)
    {
        trace.Validate();
        foreach (var required in new[] { "created", "shown", "opened", "resized", "minimized", "restored", "close-requested", "closed" })
        {
            if (!trace.Events.Any(item => item.Kind == required))
            {
                throw new InvalidDataException($"Avalonia host trace is missing {required}.");
            }
        }
        if (verifyTarget && !dpiChanged)
        {
            throw new InvalidDataException("Target verification requires an observed Avalonia DPI change; no distinct-scale display transition was recorded.");
        }
    }

    private static async Task<bool> WaitForAsync(Func<bool> predicate, string description, bool throwOnTimeout = true)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(8);
        while (DateTime.UtcNow < deadline)
        {
            if (predicate())
            {
                return true;
            }
            await Task.Delay(50);
        }
        if (throwOnTimeout)
        {
            throw new TimeoutException($"Timed out waiting for {description}.");
        }
        return false;
    }

    private static async Task<FrameAckResult> PresentRetryStaleAsync(IAvaloniaFramePipeline pipeline, DisplayList displayList)
    {
        var result = await pipeline.PresentAsync(displayList);
        return result.Status == FrameAckStatus.Stale ? await pipeline.PresentAsync(displayList) : result;
    }

    private static void RequireFeature<TFeature>(IWindow window, out TFeature feature)
        where TFeature : class
    {
        if (!window.TryGetFeature<TFeature>(out var candidate) || candidate is null)
        {
            throw new InvalidOperationException($"Avalonia window did not expose {typeof(TFeature).Name}.");
        }
        feature = candidate;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidDataException(message);
        }
    }
}

internal sealed class SampleClock : IClock
{
    private readonly long _origin = System.Diagnostics.Stopwatch.GetTimestamp();

    public TimeSpan Now => System.Diagnostics.Stopwatch.GetElapsedTime(_origin);
}

internal sealed record H2FrameFixture(string Id, int ImageWidth, int ImageHeight, byte[] ImagePixels)
{
    internal static H2FrameFixture Load()
    {
        var root = FindRoot();
        var path = Path.Combine(root, "migration", "host", "h2-frame-fixture.json");
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var fixture = document.RootElement;
        if (fixture.GetProperty("schemaVersion").GetString() != "doroti.avalonia-frame-fixture/v1")
        {
            throw new InvalidDataException("Unsupported H2 frame fixture schema.");
        }
        var image = fixture.GetProperty("image");
        var pixels = Convert.FromBase64String(image.GetProperty("pixelsBase64").GetString()!);
        var width = image.GetProperty("width").GetInt32();
        var height = image.GetProperty("height").GetInt32();
        if (pixels.Length != checked(width * height * 4))
        {
            throw new InvalidDataException("H2 image fixture byte length is invalid.");
        }
        return new(fixture.GetProperty("id").GetString()!, width, height, pixels);
    }

    private static string FindRoot()
    {
        var current = new DirectoryInfo(Environment.CurrentDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Doroti.slnx")))
            {
                return current.FullName;
            }
            var nested = Path.Combine(current.FullName, "Doroti");
            if (File.Exists(Path.Combine(nested, "Doroti.slnx")))
            {
                return nested;
            }
            current = current.Parent;
        }
        throw new DirectoryNotFoundException("Could not locate Doroti.slnx for the H2 frame fixture.");
    }
}

internal sealed record SampleOptions(AvaloniaHostRenderingMode RenderingMode, string ArtifactDirectory, bool VerifyTarget)
{
    internal static SampleOptions Parse(string[] args)
    {
        var renderingMode = AvaloniaHostRenderingMode.Hardware;
        var artifactDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "artifacts", "h1-avalonia", "hardware"));
        var verifyTarget = false;
        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--renderer":
                    var value = args[++index];
                    renderingMode = value switch
                    {
                        "hardware" => AvaloniaHostRenderingMode.Hardware,
                        "software" => AvaloniaHostRenderingMode.Software,
                        _ => throw new ArgumentException($"Unsupported renderer {value}."),
                    };
                    break;
                case "--artifact-dir":
                    artifactDirectory = Path.GetFullPath(args[++index]);
                    break;
                case "--verify-target":
                    verifyTarget = true;
                    break;
            }
        }
        return new(renderingMode, artifactDirectory, verifyTarget);
    }
}

internal sealed class RecordingWindowSink : IWindowEventSink
{
    internal List<WindowMetrics> Metrics { get; } = [];

    public void OnMetricsChanged(WindowId window, WindowMetrics metrics) => Metrics.Add(metrics);

    public void OnCloseRequested(WindowId window)
    {
    }

    public void OnClosed(WindowId window)
    {
    }
}
