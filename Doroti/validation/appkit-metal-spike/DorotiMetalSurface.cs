using System.Text.Json;
using AppKit;
using CoreGraphics;
using Doroti.Skia.Rendering;
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
    private readonly Scene _scene;
    private DorotiMetalView? _nativeView;
    private bool _submitted;
    private bool _disposed;
    private bool _automationStarted;
    private long _presented;
    private long _replayed;
    private long _failed;
    private int _evidenceWritePending;

    public DorotiMetalSurface()
    {
        _renderer = new SkiaSceneRenderer(
            ViewId,
            _host,
            new UiColor(0xfff7f2fa),
            new UiColor(0xff141218),
            "macOS/Maui/AppKit-Main/osx-arm64",
            "Metal-AppKit",
            "AppKit/MTKView/Metal-Skia");

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
        if (_submitted) return;
        _submitted = true;
        _renderer.Submit(ViewId, _scene, DartUiInvocation.Managed("appkit-metal-spike#initial-scene"));
    }

    internal SkiaPaintCompletion? Paint(
        SKSurface surface,
        int pixelWidth,
        int pixelHeight,
        long surfaceGeneration)
    {
        if (_disposed) return null;
        _host.SetSurfaceGeneration(surfaceGeneration);
        return _renderer.Paint(surface, pixelWidth, pixelHeight);
    }

    internal void CompletePaint(SkiaPaintCompletion completion, bool stale)
    {
        if (_disposed) return;
        if (stale)
        {
            Interlocked.Increment(ref _failed);
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
        _ = completion;
        _ = reason;
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
        _ = RunAutomationAsync();
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
        var renderer = _renderer.Diagnostics;
        var native = _nativeView?.CaptureDiagnostics();
        var evidence = new
        {
            schema = "doroti-appkit-metal-spike/v1",
            timestampUtc = DateTimeOffset.UtcNow,
            identity = "macOS | net10.0-macos | osx-arm64 | AppKit-Main",
            backend = "AppKit/MTKView/Metal-Skia",
            backendPackage = "Microsoft.Maui.Platforms.MacOS/0.1.0-preview.12.26368.2",
            backendSourceCommit = "229f764fd688754497fe5822213e7b13b4e9caa3",
            mauiVersion = "10.0.90",
            skiaSharpVersion = "4.152.0-rc.1.26426.14",
            native,
            frame = new
            {
                submitted = renderer.Submitted,
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

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _nativeView = null;
        _renderer.Dispose();
        _scene.Dispose();
        _picture.Dispose();
        _paragraph.Dispose();
    }
}
