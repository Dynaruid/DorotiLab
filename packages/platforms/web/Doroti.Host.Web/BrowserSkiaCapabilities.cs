using System.Text.Json;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;
using UiImage = Doroti.Ui.Image;

namespace Doroti.Host.Web;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
internal sealed class BrowserSkiaCapabilities : IBrowserGraphicsCapabilities
{
    private readonly HostBridge _bridge;
    private readonly BrowserHostAdapter _host;
    private readonly SkiaSceneRenderer _renderer;
    public TextureRegistry Textures => _renderer.Textures;
    GraphicsFeatureSupport ISceneHostCapability.Features => _renderer.SceneFeatures with
    {
        Backend = _host.Snapshot.Gpu.Api, Generation = _host.Snapshot.Gpu.ContextGeneration,
        Wgsl = _host.Snapshot.Gpu.Api == "webgpu", EffectBudgetBytes = 64L * 1024 * 1024,
    };
    SemanticsFeatureSupport ISemanticsHostCapability.Features => new(true, false,
        [SemanticsRole.dialog, SemanticsRole.alertDialog, SemanticsRole.list, SemanticsRole.listItem,
         SemanticsRole.tab, SemanticsRole.tabBar, SemanticsRole.menu, SemanticsRole.menuItem],
        [SemanticsAction.tap, SemanticsAction.setText, SemanticsAction.setSelection, SemanticsAction.focus,
         SemanticsAction.increase, SemanticsAction.decrease, SemanticsAction.expand, SemanticsAction.collapse],
        "DOM accessibility geometry represents semantic bounds; native glyph text ranges are not exposed.");
    private readonly BrowserPlatformViewHost _platform;
    private readonly object _paintGate = new();
    private readonly Dictionary<long, SkiaPaintCompletion> _pendingPaints = [];

    internal BrowserSkiaCapabilities(
        ulong viewId,
        BrowserHostAdapter host,
        Color? backgroundColor,
        Color? darkBackgroundColor,
        string backendIdentity,
        SkiaFallbackFontCollection? fallbackFonts,
        BrowserPlatformViewHost platform
    )
    {
        _host = host;
        _platform = platform;
        _bridge = new(host);
        _renderer = new(
            viewId,
            _bridge,
            backgroundColor,
            darkBackgroundColor,
            backendIdentity,
            host.Snapshot.Gpu.Api == "webgpu"
                ? DorotiSkiaRuntimeEffects.WebGraphiteBackend
                : DorotiSkiaRuntimeEffects.WebGpuBackend,
            host.Snapshot.Gpu.Api == "webgpu"
                ? "doroti-owned-canvas-graphite-dawn"
                : "doroti-owned-canvas-webgl2-skia-gpu",
            fallbackFonts: fallbackFonts,
            pictureRasterCachePixels: DorotiWebWorkerSurface.PictureRasterCachePixels
        );
        _renderer.PlatformScenePainter = (canvas, commands, descriptor, width, height) =>
            _platform.Draw(_renderer, canvas, commands, descriptor, width, height);
        DorotiWebWorkerSurface.AttachTextureRenderer(viewId, _renderer);
        _renderer.TextureFeatureProvider = () => new(BrowserVideo: _host.Snapshot.Gpu.ContextGeneration > 0,
            Reason: "Browser video/canvas sources use browser GPU handles; native OS handles and Android producer Surface are unsupported.",
            Generation: _host.Snapshot.Gpu.ContextGeneration);
    }

    public event Action<SemanticsActionEvent>? Action
    {
        add => _renderer.Action += value;
        remove => _renderer.Action -= value;
    }

    public bool CoalesceGeometryDuringActiveMetrics => true;

    public BrowserFrameDiagnostics Diagnostics
    {
        get
        {
            var value = _renderer.Diagnostics;
            return new(
                value.Submitted,
                value.Presented,
                value.Replayed,
                value.Failed,
                value.ContextGeneration,
                value.SurfaceGeneration,
                value.LastInputSequence,
                value.PendingScene,
                value.Backend,
                value
            );
        }
    }

    public void AttachSurface(Action invalidate)
    {
        _bridge.Invalidate = invalidate;
        _renderer.AttachSurface(invalidate);
    }

    public void AttachFrameworkTrace(DorotiFrameTrace trace)
    {
        trace.MeasureRecordingTime =
            Environment.GetEnvironmentVariable("DOROTI_WEB_DIRECT_TRACE") == "1";
        _renderer.AttachFrameworkTrace(trace);
    }

    public void BindOwner(DorotiSceneOwner owner) => _renderer.BindOwner(owner);

    public void Submit(
        ulong viewId,
        DorotiSceneSubmission submission,
        DorotiUiInvocation invocation
    ) => _renderer.Submit(viewId, submission, invocation);

    public string Paint(
        SKSurface surface,
        int pixelWidth,
        int pixelHeight,
        DorotiResizeEpoch target,
        long requestId
    )
    {
        var started = BrowserHostAdapter.RasterDiagnosticsEnabled ? DorotiFrameClock.Now : default;
        _host.RecordRaster("managed-raster-start", pixelWidth, pixelHeight);
        try
        {
            var result = _renderer.Paint(surface, pixelWidth, pixelHeight, target, requestId);
            if (result.ShouldPresent && result.Completion is { } completion)
            {
                lock (_paintGate)
                {
                    _pendingPaints[requestId] = completion;
                }
            }
            return result.Disposition switch
            {
                SkiaPaintDisposition.exact => "exact-rendered",
                SkiaPaintDisposition.replay => "replay-rendered",
                SkiaPaintDisposition.superseded => "superseded",
                _ => "empty",
            };
        }
        finally
        {
            if (BrowserHostAdapter.RasterDiagnosticsEnabled)
            {
                _host.RecordRaster(
                    "managed-raster-end",
                    pixelWidth,
                    pixelHeight,
                    DorotiFrameClock.Now - started
                );
            }
        }
    }

    public void CompletePaint(long requestId, string terminal, string reason)
    {
        _platform.Complete(terminal is "submitted" or "presented");
        SkiaPaintCompletion completion;
        lock (_paintGate)
        {
            if (!_pendingPaints.Remove(requestId, out completion))
            {
                return;
            }
        }
        switch (terminal)
        {
            case "submitted":
                _renderer.CompletePaint(completion, DorotiFrameTerminal.submitted);
                break;
            case "presented":
                _renderer.CompletePaint(completion, DorotiFrameTerminal.presented);
                break;
            case "superseded":
                _renderer.SupersedePaint(completion, reason);
                break;
            case "dropped":
                _renderer.DropPaint(completion, reason);
                break;
            case "failed":
                _renderer.FailPaint(completion, reason);
                break;
            default:
                throw new InvalidDataException($"Unknown browser frame terminal '{terminal}'.");
        }
    }

    public void InvalidateGpuContext(long requestId, string reason)
    {
        _platform.Complete();
        SkiaPaintCompletion completion;
        lock (_paintGate)
        {
            if (_pendingPaints.Remove(requestId, out completion))
            {
                _renderer.FailPaint(completion, reason);
            }
        }
        _renderer.InvalidateGpuContextResources();
    }

    public void InvalidateWindowSurfaceResources() => _renderer.InvalidateWindowSurfaceResources();

    public Paragraph Layout(ParagraphRequest request, DorotiUiInvocation invocation) =>
        _renderer.Layout(request, invocation);

    public ValueTask<UiImage> DecodeSizedAsync(
        ReadOnlyMemory<byte> bytes,
        Func<long, long, TargetImageSize?> targetSize,
        bool allowUpscaling,
        DorotiUiInvocation invocation,
        CancellationToken cancellationToken = default
    ) =>
        _renderer.DecodeSizedAsync(
            bytes,
            targetSize,
            allowUpscaling,
            invocation,
            cancellationToken
        );

    public ValueTask<UiImage> RasterizeAsync(
        Picture picture,
        int width,
        int height,
        DorotiUiInvocation invocation,
        CancellationToken cancellationToken = default
    ) => _renderer.RasterizeAsync(picture, width, height, invocation, cancellationToken);

    public ValueTask<UiImage> DecodeAsync(
        ReadOnlyMemory<byte> bytes,
        DorotiUiInvocation invocation,
        CancellationToken cancellationToken = default
    ) => _renderer.DecodeAsync(bytes, invocation, cancellationToken);

    public void SetEnabled(bool enabled, DorotiUiInvocation invocation) =>
        _renderer.SetEnabled(enabled, invocation);

    public void Update(SemanticsUpdate update, DorotiUiInvocation invocation)
    {
        var started = DorotiFrameClock.Now;
        _host.RecordRaster("managed-semantics-start", 0, 0);
        try
        {
            _renderer.Update(update, invocation);
        }
        finally
        {
            _host.RecordRaster("managed-semantics-end", 0, 0, DorotiFrameClock.Now - started);
        }
    }

    public ValueTask RegisterFontAsync(
        ReadOnlyMemory<byte> bytes,
        string? family,
        CancellationToken cancellationToken = default
    ) => _renderer.RegisterFontAsync(bytes, family, cancellationToken);

    public void Dispose()
    {
        lock (_paintGate)
        {
            foreach (var pending in _pendingPaints.Values)
            {
                _renderer.SupersedePaint(pending, "browser graphics capability disposed");
            }

            _pendingPaints.Clear();
        }
        DorotiWebWorkerSurface.DetachTextureRenderer(_renderer);
        _renderer.Dispose();
        _platform.Dispose();
        _bridge.Dispose();
    }

    private sealed class HostBridge : ISkiaSceneRendererHost, IDisposable
    {
        private readonly BrowserHostAdapter _host;
        private readonly BrowserSemanticsTransport _semantics = new();

        internal HostBridge(BrowserHostAdapter host)
        {
            _host = host;
            _host.SemanticsAction += HandleSemanticsAction;
            _host.SemanticsSnapshotRequested += SendSemanticsSnapshot;
        }

        internal Action? Invalidate { get; set; }
        public long InputSequence => _host.InputSequence;
        public long SurfaceGeneration => _host.Snapshot.SurfaceGeneration;
        public DorotiViewEpoch ViewEpoch => _host.ViewEpoch;
        public DorotiResizeEpoch ResizeTarget => _host.Snapshot.ResizeEpoch;
        public PlatformConfiguration Configuration => _host.Configuration;
        public event Action<int, SemanticsAction, object?>? SemanticsAction;
        public event Action<long, TimeSpan>? InputReceived
        {
            add => _host.InputReceived += value;
            remove => _host.InputReceived -= value;
        }
        public event Action<PlatformConfiguration>? ConfigurationChanged
        {
            add => _host.ConfigurationChanged += value;
            remove => _host.ConfigurationChanged -= value;
        }

        public void UpdateSemantics(SemanticsUpdate update)
        {
            using var profile = FrameworkWorkCounters.Enabled
                ? FrameworkWorkProfile.Begin(GetType(), 2) : default;
            if (_semantics.Update(update) is { } json) _host.UpdateSemantics(json);
        }

        private void SendSemanticsSnapshot() => _host.UpdateSemantics(_semantics.CaptureSnapshot());

        public void ClearSemantics() => _host.UpdateSemantics(_semantics.Clear());

        public void RequestInvalidate() => Invalidate?.Invoke();

        public void Dispose()
        {
            _host.SemanticsAction -= HandleSemanticsAction;
            _host.SemanticsSnapshotRequested -= SendSemanticsSnapshot;
            Invalidate = null;
            _semantics.Reset();
        }

        private void HandleSemanticsAction(long nodeId, long action, string argumentsJson)
        {
            if (nodeId is < int.MinValue or > int.MaxValue)
            {
                return;
            }

            SemanticsAction?.Invoke(
                checked((int)nodeId),
                (SemanticsAction)action,
                ParseArguments(argumentsJson)
            );
        }

        private static object? ParseArguments(string json)
        {
            if (string.IsNullOrWhiteSpace(json) || json == "null")
            {
                return null;
            }

            using var document = JsonDocument.Parse(json);
            return ConvertElement(document.RootElement);
        }

        private static object? ConvertElement(JsonElement element) =>
            element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number when element.TryGetInt64(out var integer) => integer,
                JsonValueKind.Number => element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Array => element.EnumerateArray().Select(ConvertElement).ToArray(),
                JsonValueKind.Object => element
                    .EnumerateObject()
                    .ToDictionary(
                        property => property.Name,
                        property => ConvertElement(property.Value),
                        StringComparer.Ordinal
                    ),
                _ => null,
            };
    }
}
