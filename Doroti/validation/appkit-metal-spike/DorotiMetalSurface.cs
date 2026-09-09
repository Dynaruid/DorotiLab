using System.Text.Json;
using AppKit;
using CoreGraphics;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using Foundation;
using SkiaSharp;
using UiBlendMode = Doroti.Ui.BlendMode;
using UiColor = Doroti.Ui.Color;
using UiPaint = Doroti.Ui.Paint;
using UiRect = Doroti.Ui.Rect;

namespace Doroti.Validation.AppKitMetalSpike;

internal sealed class DorotiMetalSurface : View, IDisposable
{
    private const ulong ViewId = 1;
    private readonly object _gate = new();
    private readonly SpikeRendererHost _host = new();
    private readonly SkiaSceneRenderer _renderer;
    private readonly Picture _picture;
    private readonly Paragraph _paragraph;
    private readonly FragmentShader _shader;
    private readonly Scene _scene;
    private DorotiMetalView? _nativeView;
    private long _submittedGeneration;
    private long _frameworkFrame;
    private bool _disposed;
    private bool _automationStarted;
    private long _presented;
    private long _replayed;
    private long _failed;
    private int _evidenceWritePending;
    private readonly SemaphoreSlim _evidenceGate = new(1, 1);
    private string? _firstFailure;
    private bool _automationComplete;

    public DorotiMetalSurface()
    {
        _renderer = new SkiaSceneRenderer(
            ViewId,
            _host,
            new UiColor(0xfff7f2fa),
            new UiColor(0xff141218),
            "macOS/Maui/AppKit-Main/osx-arm64",
            DorotiMetalView.UseGraphite ? DorotiSkiaRuntimeEffects.NativeGraphiteMetalBackend : DorotiSkiaRuntimeEffects.AppKitMetalBackend,
            DorotiMetalView.UseGraphite ? "AppKit/MTKView/Graphite-Metal" : "AppKit/MTKView/Ganesh-Metal");

        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder);
        canvas.drawColor(new UiColor(0xfff7f2fa), UiBlendMode.src);
        canvas.drawRRect(
            RRect.fromRectAndRadius(UiRect.fromLTWH(72, 64, 496, 292), Radius.circular(28)),
            new UiPaint { color = new UiColor(0xff6750a4) });
        canvas.drawCircle(
            new Offset(320, 210),
            78,
            new UiPaint { color = new UiColor(0xffffd8e4) });
        canvas.drawLine(
            new Offset(244, 210),
            new Offset(396, 210),
            new UiPaint
            {
                color = new UiColor(0xff21005d),
                strokeWidth = 12,
                strokeCap = StrokeCap.round,
            });
        _shader = FragmentProgram.fromSource(
            "half4 main(float2 p) { return half4(0.4, 0.2, 0.8, 1); }", "appkit-graphite-runtime-effect").fragmentShader();
        canvas.drawRect(UiRect.fromLTWH(72, 382, 96, 32), new UiPaint { shader = _shader });
        _paragraph = new Paragraph(
            "Doroti AppKit / Metal",
            260,
            32,
            24,
            color: new UiColor(0xff21005d));
        canvas.drawParagraph(_paragraph, new Offset(190, 382));
        _picture = recorder.endRecording();
        var sceneBuilder = new SceneBuilder(ViewId);
        sceneBuilder.addPicture(Offset.zero, _picture);
        _scene = sceneBuilder.build();
    }

    internal void ConnectNativeView(DorotiMetalView nativeView)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _nativeView = nativeView;
        _host.AttachInvalidate(nativeView.RequestFrame);
        _renderer.AttachSurface(nativeView.RequestFrame);
        nativeView.RequestFrame();
    }

    internal SkiaPaintCompletion? Paint(
        SKSurface surface,
        int pixelWidth,
        int pixelHeight,
        long surfaceGeneration,
        double scale)
    {
        if (_disposed) return null;
        _host.SetViewport(pixelWidth, pixelHeight, scale, surfaceGeneration);
        if (_submittedGeneration != surfaceGeneration)
        {
            _submittedGeneration = surfaceGeneration;
            var token = new DorotiSceneBuildToken(_host.ViewEpoch, ++_frameworkFrame, pixelWidth, pixelHeight);
            _renderer.Submit(ViewId, new DorotiSceneSubmission(_scene, token),
                DartUiInvocation.Managed("appkit-metal-spike#viewport-scene"));
        }
        return _renderer.Paint(surface, pixelWidth, pixelHeight);
    }

    internal void CompletePaint(SkiaPaintCompletion completion, bool stale)
    {
        if (_disposed) return;
        if (stale)
        {
            _renderer.SupersedePaint(completion, "stale Metal completion rejected");
        }
        else
        {
            _renderer.CompletePaint(completion);
            if (completion.IsNewFrame) Interlocked.Increment(ref _presented);
            else Interlocked.Increment(ref _replayed);
        }
        QueueEvidenceWrite();
        StartAutomationIfRequested();
    }

    internal void FailPaint(SkiaPaintCompletion? completion, string reason)
    {
        _firstFailure ??= reason;
        if (completion is { } value) _renderer.FailPaint(value, reason);
        if (_disposed) return;
        Interlocked.Increment(ref _failed);
        QueueEvidenceWrite();
        StartAutomationIfRequested();
    }

    private void StartAutomationIfRequested()
    {
        if (!string.Equals(
                Environment.GetEnvironmentVariable("DOROTI_APPKIT_SPIKE_AUTOMATE"),
                "1",
                StringComparison.Ordinal) ||
            Interlocked.Read(ref _presented) == 0)
            return;
        lock (_gate)
        {
            if (_automationStarted) return;
            _automationStarted = true;
        }
        _ = RunAutomationGuardedAsync();
    }

    private async Task RunAutomationGuardedAsync()
    {
        try { await RunAutomationAsync().ConfigureAwait(false); }
        catch (Exception exception)
        {
            _firstFailure ??= exception.ToString();
            Interlocked.Increment(ref _failed);
            await WriteEvidenceAsync().ConfigureAwait(false);
            Environment.ExitCode = 1;
            await OnMainThreadAsync(() => NSApplication.SharedApplication.Terminate(NSApplication.SharedApplication)).ConfigureAwait(false);
        }
    }

    private async Task RunAutomationAsync()
    {
        for (var index = 0; index < 20; index++)
        {
            await Task.Delay(45).ConfigureAwait(false);
            var step = index;
            await OnMainThreadAsync(() =>
            {
                var window = _nativeView?.Window;
                if (window is null) return;
                window.SetContentSize(new CGSize(640 + (step % 5) * 24, 420 + (step % 4) * 18));
                _nativeView?.RequestFrame();
            }).ConfigureAwait(false);
        }

        await OnMainThreadAsync(() => _nativeView?.Window?.Miniaturize(NSApplication.SharedApplication)).ConfigureAwait(false);
        await Task.Delay(150).ConfigureAwait(false);
        await OnMainThreadAsync(() => _nativeView?.Window?.Deminiaturize(NSApplication.SharedApplication)).ConfigureAwait(false);
        await Task.Delay(150).ConfigureAwait(false);
        await OnMainThreadAsync(() => NSApplication.SharedApplication.Hide(NSApplication.SharedApplication)).ConfigureAwait(false);
        await Task.Delay(150).ConfigureAwait(false);
        await OnMainThreadAsync(() => NSApplication.SharedApplication.Unhide(NSApplication.SharedApplication)).ConfigureAwait(false);
        _nativeView?.RequestFrame();
        await Task.Delay(500).ConfigureAwait(false);
        Task? shutdown = null;
        await OnMainThreadAsync(() =>
        {
            _nativeView?.Draw();
            shutdown = _nativeView?.ShutdownAsync();
        }).ConfigureAwait(false);
        if (shutdown is not null) await shutdown.WaitAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        _automationComplete = true;
        await WriteEvidenceAsync().ConfigureAwait(false);
        await OnMainThreadAsync(() => _nativeView?.Window?.Close()).ConfigureAwait(false);
        await Task.Delay(100).ConfigureAwait(false);
        await OnMainThreadAsync(() => NSApplication.SharedApplication.Terminate(NSApplication.SharedApplication)).ConfigureAwait(false);
    }

    private static Task OnMainThreadAsync(Action action)
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        NSApplication.SharedApplication.BeginInvokeOnMainThread(() =>
        {
            try
            {
                action();
                completion.SetResult();
            }
            catch (Exception exception)
            {
                completion.SetException(exception);
            }
        });
        return completion.Task;
    }

    private void QueueEvidenceWrite()
    {
        if (Interlocked.CompareExchange(ref _evidenceWritePending, 1, 0) != 0) return;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(50).ConfigureAwait(false);
                await WriteEvidenceAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"[DorotiMetalSurface] evidence write failed: {exception}");
            }
            finally
            {
                Interlocked.Exchange(ref _evidenceWritePending, 0);
            }
        });
    }

    private async Task WriteEvidenceAsync()
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_APPKIT_SPIKE_EVIDENCE");
        if (string.IsNullOrWhiteSpace(path)) return;
        await _evidenceGate.WaitAsync().ConfigureAwait(false);
        try
        {
            var renderer = _renderer.Diagnostics;
            object? native = null;
            await OnMainThreadAsync(() => native = _nativeView?.CaptureDiagnostics()).ConfigureAwait(false);
            var evidence = new
            {
                schema = "doroti-appkit-metal-spike/v2",
                status = _firstFailure is not null ? "FAIL" : _automationComplete ? "PASS" : "RUNNING",
                firstFailure = _firstFailure,
                physicalScanOut = "notVerified",
                performance = "notVerified",
                timestampUtc = DateTimeOffset.UtcNow,
                identity = "macOS | net10.0-macos | osx-arm64 | AppKit-Main",
                backend = DorotiMetalView.UseGraphite ? "AppKit/MTKView/Graphite-Metal" : "AppKit/MTKView/Ganesh-Metal",
                skiaIdentity = GraphiteMetalContract.Identity(),
                native,
                frame = new
                {
                    sceneAccepted = renderer.SceneAccepted,
                    platformSubmitted = renderer.Submitted,
                    presented = Interlocked.Read(ref _presented),
                    replayed = Interlocked.Read(ref _replayed),
                    superseded = renderer.Superseded,
                    failed = Interlocked.Read(ref _failed),
                    dropped = renderer.Dropped,
                },
                softwareFallbackFrames = 0,
                cpuReadbacks = 0,
                fullFrameCopies = 0,
            };
            var json = JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true });
            await Task.Run(() =>
            {
                var directory = System.IO.Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
                File.WriteAllText(path, json);
            }).ConfigureAwait(false);
        }
        finally { _evidenceGate.Release(); }
    }

    internal void ReleaseRendererResources() => _renderer.InvalidateGpuContextResources();

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _nativeView = null;
        _renderer.Dispose();
        _scene.Dispose();
        _picture.Dispose();
        _paragraph.Dispose();
        _shader.dispose();
    }
}
