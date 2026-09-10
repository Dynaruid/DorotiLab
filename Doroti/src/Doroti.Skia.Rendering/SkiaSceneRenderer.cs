using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;
using BlendMode = Doroti.Ui.BlendMode;
using Rect = Doroti.Ui.Rect;
using UiColor = Doroti.Ui.Color;
using UiImage = Doroti.Ui.Image;
using UiPath = Doroti.Ui.Path;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer :
    ISceneHostCapability,
    IParagraphHostCapability,
    IFontHostCapability,
    IImageHostCapability,
    ISemanticsHostCapability,
    IDisposable
{
    private const int PictureRasterWarmupFrames = 2;
    private const int PictureRasterComplexityThreshold = 8;
    private const int MaxImageFilterResources = 64;
    private const int MaxPictureRasterCacheEntries = 24;
    private const long MaxPictureRasterPixels = 16L * 1024 * 1024;
    private const long MaxCacheablePicturePixels = 4L * 1024 * 1024;
    private const int MaxPictureRasterWarmups = 128;
    private const long PictureWarmupLifetimeFrames = 120;
    private const long PromotionBudgetMicroseconds = 2000;
    private readonly ulong _viewId;
    private readonly ISkiaSceneRendererHost _host;
    private readonly UiColor? _lightBackgroundColor;
    private readonly UiColor? _darkBackgroundColor;
    private readonly string _targetIdentity;
    private readonly string _runtimeEffectBackend;
    private readonly string _diagnosticsBackend;
    private readonly bool _enablePictureRasterCache;
    private readonly SkiaFallbackFontCollection _fallbackFonts;
    private readonly bool _ownsFallbackFonts;
    private SKColor _backgroundColor;
    private double _shadowDeviceScale = 1; // Scoped by _paintGate; offscreen pictures use logical pixels.
    private readonly object _gate = new();
    private readonly object _paintGate = new();
    private readonly Dictionary<TextRenderKey, TextRenderResources> _textRenderResources = [];
    private readonly Dictionary<int, SemanticsNodeUpdate> _semantics = [];
    private readonly Dictionary<object, PictureRasterCacheEntry> _pictureRasterCache =
        new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<object, PictureRasterWarmup> _pictureRasterWarmups =
        new(ReferenceEqualityComparer.Instance);
    private readonly LinkedList<object> _pictureRasterWarmupOrder = new();
    private readonly Dictionary<ImageFilterSnapshot, SKImageFilter> _imageFilterResources = [];
    private readonly DorotiFrameTerminalLedger _terminalLedger = new();
    private readonly Dictionary<long, SceneFrame> _rasterizedFrames = [];
    private SceneFrame? _pendingFrame;
    private SceneFrame? _presentedFrame;
    private Action? _invalidate;
    private long _submitted;
    private long _presented;
    private long _sceneAccepted;
    private long _causalPaintAttempts;
    private long _replayed;
    private long _failed;
    private long _superseded;
    private long _dropped;
    private long _nextSceneSequence;
    private long _lastSubmittedInputSequence;
    private long _lastPresentedInputSequence;
    private long _contextGeneration;
    private readonly object _runtimeEffectContextOwner = new();
    private long _shaderImageFiltersRendered;
    private long _pictureRasterUseSequence;
    private long _pictureRasterPixels;
    private long _pictureRasterCacheHits;
    private long _pictureRasterCacheMisses;
    private long _pictureRasterCacheEntries;
    private long _promotionMicroseconds;
    private long _promotionMaximumMicroseconds;
    private long _paragraphCount;
    private long _paragraphMicroseconds;
    private long _rasterFrame;
    private int _framePromotions;
    private long _framePromotionPixels;
    private long _framePromotionMicroseconds;
    private bool _semanticsEnabled;
    private bool _disposed;
    private DorotiFrameTrace _frameTrace = new();

    private string RuntimeEffectBackend => $"{_runtimeEffectBackend}/{_viewId}";

    public SkiaSceneRenderer(
        ulong viewId,
        ISkiaSceneRendererHost host,
        UiColor? backgroundColor,
        UiColor? darkBackgroundColor,
        string targetIdentity,
        string runtimeEffectBackend,
        string diagnosticsBackend,
        bool enablePictureRasterCache = true,
        SkiaFallbackFontCollection? fallbackFonts = null)
    {
        _viewId = viewId;
        _host = host;
        _lightBackgroundColor = backgroundColor;
        _darkBackgroundColor = darkBackgroundColor;
        _targetIdentity = targetIdentity;
        _runtimeEffectBackend = runtimeEffectBackend;
        _diagnosticsBackend = diagnosticsBackend;
        _enablePictureRasterCache = enablePictureRasterCache;
        _fallbackFonts = fallbackFonts ?? new SkiaFallbackFontCollection();
        _ownsFallbackFonts = fallbackFonts is null;
        _backgroundColor = ResolveBackgroundColor(_host.Configuration.platformBrightness);
        _host.SemanticsAction += HandleSemanticsAction;
        _host.InputReceived += HandleInput;
        _host.ConfigurationChanged += HandleConfigurationChanged;
    }

    private Action<SemanticsActionEvent>? _action;
    public event Action<SemanticsActionEvent>? Action { add => _action += value; remove => _action -= value; }

    /// <summary>
    /// Observes terminal native paint submissions.  It is intentionally a
    /// receipt rather than a scheduling callback: the scene descriptor keeps
    /// the immutable identity captured when the framework built it.
    /// </summary>
    public event Action<SkiaFrameReceipt>? FrameReceipt;

    public SkiaFrameDiagnostics Diagnostics
    {
        get
        {
            var imageFilterSurfaces = DorotiSkiaImageFilterRenderer.Diagnostics;
            lock (_gate)
                return new(_submitted, _presented, _replayed, _failed, _contextGeneration,
                    _host.SurfaceGeneration, _pendingFrame is not null,
                    Volatile.Read(ref _shaderImageFiltersRendered),
                    _diagnosticsBackend, _superseded, _dropped,
                    _host.InputSequence, _lastSubmittedInputSequence, _lastPresentedInputSequence,
                    imageFilterSurfaces.Created, imageFilterSurfaces.Reused, imageFilterSurfaces.Active,
                    imageFilterSurfaces.CacheHits, imageFilterSurfaces.CacheMisses,
                    Volatile.Read(ref _pictureRasterCacheHits),
                    Volatile.Read(ref _pictureRasterCacheMisses),
                    Volatile.Read(ref _pictureRasterCacheEntries),
                    _frameTrace.Snapshot(), _sceneAccepted, _causalPaintAttempts,
                    _terminalLedger.Diagnostics,
                    new(_pictureRasterCacheMisses, _promotionMicroseconds, _promotionMaximumMicroseconds,
                        _paragraphCount, _paragraphMicroseconds, _pictureRasterWarmups.Count,
                        _pictureRasterPixels, _textRenderResources.Count,
                        _pictureCommandHits, _pictureCommandRecordings, _pictureCommandCache.Count, _pictureCommandCount, _pictureCommandBytes));
        }
    }

    public void AttachFrameworkTrace(DorotiFrameTrace frameTrace)
    {
        ArgumentNullException.ThrowIfNull(frameTrace);
        lock (_gate) _frameTrace = frameTrace;
    }

    public void AttachSurface(Action invalidate)
    {
        ArgumentNullException.ThrowIfNull(invalidate);
        ObjectDisposedException.ThrowIf(_disposed, this);
        bool hasFrame;
        long contextGeneration;
        lock (_paintGate)
        {
            lock (_gate)
            {
                _invalidate = invalidate;
                _contextGeneration++;
                contextGeneration = _contextGeneration;
                hasFrame = _pendingFrame is not null || _presentedFrame is not null;
            }
            DorotiSkiaRuntimeEffects.InvalidateContext(
                RuntimeEffectBackend, contextGeneration, _runtimeEffectContextOwner);
            DorotiSkiaImageFilterRenderer.InvalidateContext(
                RuntimeEffectBackend, contextGeneration, _runtimeEffectContextOwner);
            ClearPictureRasterCache();
        }
        if (hasFrame) invalidate();
    }

    /// <summary>
    /// Releases GPU images and temporary render targets that were recorded
    /// against the current native window surface before that surface is
    /// destroyed and recreated. The Skia/GL context itself remains current.
    /// </summary>
    public void InvalidateWindowSurfaceResources()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_paintGate)
        {
            DorotiSkiaImageFilterRenderer.InvalidateSurface(
                RuntimeEffectBackend, _contextGeneration, _runtimeEffectContextOwner);
            ClearPictureRasterCache();
        }
    }

    /// <summary>
    /// Advances the renderer context identity and releases every cached GPU
    /// resource before the host destroys and recreates its native GL context.
    /// </summary>
    public void InvalidateGpuContextResources()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        long contextGeneration;
        lock (_paintGate)
        {
            lock (_gate)
            {
                _contextGeneration++;
                contextGeneration = _contextGeneration;
            }
            DorotiSkiaRuntimeEffects.InvalidateContext(
                RuntimeEffectBackend, contextGeneration, _runtimeEffectContextOwner);
            DorotiSkiaImageFilterRenderer.InvalidateContext(
                RuntimeEffectBackend, contextGeneration, _runtimeEffectContextOwner);
            foreach (var filter in _imageFilterResources.Values) filter.Dispose();
            _imageFilterResources.Clear();
            ClearPictureRasterCache();
        }
    }

    /// <summary>
    /// Completes rasterized scenes that cannot cross presentation because the
    /// native GPU context was lost. This must run before context invalidation.
    /// </summary>
    public void FailOutstandingGpuPaints(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        if (_disposed) return;
        List<SkiaFrameReceipt> receipts = [];
        lock (_gate)
        {
            foreach (var pair in _rasterizedFrames.ToArray())
            {
                var frame = pair.Value;
                if (MarkTerminal(frame, DorotiFrameTerminal.failed, reason, _host.SurfaceGeneration))
                {
                    var completion = new SkiaPaintCompletion(
                        frame.InputSequence, frame.SceneSequence, _host.SurfaceGeneration,
                        IsNewFrame: true, frame.Descriptor);
                    receipts.Add(CreateFrameReceipt(
                        completion, DorotiFrameTerminal.failed,
                        SkiaPaintDisposition.exact, reason));
                }
            }
            _rasterizedFrames.Clear();
        }
        foreach (var receipt in receipts) PublishFrameReceipt(receipt);
    }

    public void Submit(ulong viewId, DorotiSceneSubmission submission, DartUiInvocation invocation)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(submission);
        var scene = submission.Scene;
        ArgumentNullException.ThrowIfNull(scene);
        if (viewId != _viewId || scene.viewId != _viewId)
            throw new DorotiCapabilityException(DorotiCapabilityIds.GraphicsScene, viewId, invocation,
                "scene/view ownership mismatch", _targetIdentity);
        Action? invalidate;
        var timestamp = DorotiFrameClock.Now;
        lock (_gate)
        {
            var sceneSequence = ++_nextSceneSequence;
            _terminalLedger.Register(sceneSequence);
            var inputSequence = _host.InputSequence;
            _sceneAccepted++;
            if (submission.BuildToken is not { } buildToken)
            {
                _terminalLedger.TryComplete(sceneSequence, DorotiFrameTerminal.dropped);
                submission.FrameTransaction?.TryComplete(
                    DorotiFrameTerminal.dropped,
                    "scene submitted outside a framework frame");
                _dropped++;
                _frameTrace.Record(DorotiFramePhase.dropped, _viewId, timestamp,
                    inputSequence, sceneSequence, _host.SurfaceGeneration,
                    $"{DorotiFrameMismatch.missingBuildToken}: scene submitted outside a framework frame",
                    resizeTargetGeneration: _host.ViewEpoch.ResizeTargetGeneration,
                    metricsGeneration: _host.ViewEpoch.MetricsGeneration,
                    contextGeneration: _contextGeneration);
                return;
            }
            // The producer hands raster an immutable command array. It never
            // mutates or disposes a scene currently being consumed by Paint.
            var descriptor = DorotiFrameDescriptor.FromBuildToken(buildToken, sceneSequence);
            try
            {
                submission.FrameTransaction?.SceneBuilt(buildToken, descriptor);
            }
            catch (Exception exception)
            {
                _terminalLedger.TryComplete(sceneSequence, DorotiFrameTerminal.failed);
                submission.FrameTransaction?.TryComplete(
                    DorotiFrameTerminal.failed,
                    $"scene transaction admission failed: {exception.Message}");
                throw;
            }
            var incoming = new SceneFrame(
                sceneSequence, inputSequence, timestamp, descriptor, scene.Commands,
                submission.FrameTransaction);
            if (_pendingFrame is { } pending)
            {
                if (descriptor.CompareAdmissionTo(pending.Descriptor) < 0)
                {
                    MarkTerminal(incoming, DorotiFrameTerminal.superseded,
                        "older viewport epoch cannot replace pending scene");
                    return;
                }
                MarkTerminal(pending, DorotiFrameTerminal.superseded,
                    "latest immutable scene replaced before raster");
            }
            _pendingFrame = incoming;
            invalidate = _invalidate;
            _frameTrace.Record(DorotiFramePhase.sceneSubmitted, _viewId, timestamp,
                inputSequence, sceneSequence, _host.SurfaceGeneration, invocation.ElementId,
                resizeTargetGeneration: descriptor.ResizeTargetGeneration,
                metricsGeneration: descriptor.MetricsGeneration,
                frameworkFrameNumber: descriptor.FrameworkFrameNumber,
                contextGeneration: _contextGeneration);
        }
        invalidate?.Invoke();
    }

    public SkiaPaintCompletion? Paint(SKSurface surface, int pixelWidth, int pixelHeight)
        => Paint(surface, pixelWidth, pixelHeight, _host.ResizeTarget, causalFrameId: 0).Completion;

    public SkiaPaintResult Paint(
        SKSurface surface,
        int pixelWidth,
        int pixelHeight,
        DorotiResizeEpoch desiredTarget)
        => Paint(surface, pixelWidth, pixelHeight, desiredTarget, causalFrameId: 0);

    /// <summary>
    /// Rasters an exact immutable target under a host-generated causal frame
    /// identifier.  A non-positive identifier is reserved for pre-F6 callers
    /// that only use the legacy paint overload.
    /// </summary>
    public SkiaPaintResult Paint(
        SKSurface surface,
        int pixelWidth,
        int pixelHeight,
        DorotiResizeEpoch desiredTarget,
        long causalFrameId)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(desiredTarget);
        if (causalFrameId < 0) throw new ArgumentOutOfRangeException(nameof(causalFrameId));
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_paintGate)
        {
            var previousScale = _shadowDeviceScale;
            _shadowDeviceScale = desiredTarget.DeviceScaleY;
            try { return PaintCore(surface, pixelWidth, pixelHeight, desiredTarget, causalFrameId); }
            finally { _shadowDeviceScale = previousScale; }
        }
    }

    private SkiaPaintResult PaintCore(
        SKSurface surface,
        int pixelWidth,
        int pixelHeight,
        DorotiResizeEpoch desiredTarget,
        long causalFrameId)
    {
        SceneFrame? frame;
        bool isNewFrame;
        lock (_gate)
        {
            frame = _pendingFrame;
            isNewFrame = frame is not null;
            if (isNewFrame) _pendingFrame = null;
            else frame = _presentedFrame;
        }

        var currentEpoch = _host.ViewEpoch;
        var match = frame?.Descriptor.MatchExact(
            currentEpoch,
            desiredTarget,
            pixelWidth,
            pixelHeight,
            desiredTarget.DeviceScaleX,
            desiredTarget.DeviceScaleY);
        if (frame is not null && match is { IsExact: false })
        {
            if (isNewFrame)
            {
                lock (_gate)
                    MarkTerminal(frame, DorotiFrameTerminal.superseded,
                        $"{match.MismatchCode}: {match.Detail}");
            }
            return new(SkiaPaintDisposition.superseded, null, frame.Descriptor, match);
        }

        var canvas = surface.Canvas;
        // Clear every fresh back buffer to the app-owned background color.
        // Its alpha is intentionally preserved: premultiplied composition
        // targets can later reveal a Windows backdrop through this scene.
        // A retained-capacity Windows Vulkan surface can be larger than the
        // current visible client. Clearing the full surface keeps newly
        // revealed parent-clipped pixels on the app background instead of an
        // undefined/black swapchain image.
        canvas.Clear(_backgroundColor);
        if (frame is null)
        {
#if !WINDOWS
            canvas.Flush();
#endif
            return new(SkiaPaintDisposition.empty, null, null, DorotiFrameMatchResult.Exact);
        }
        try
        {
            // SKSwapChainPanel and TextureView can rotate to a fresh back buffer.
            // Replay the last successful framework scene when no replacement is pending.
            // RenderView's root transform has already converted logical coordinates
            // into physical pixels. Applying host DPR here would scale twice.
            var rasterStart = DorotiFrameClock.Now;
            BeginPictureRasterFrame();
            DorotiSkiaImageFilterRenderer.BeginFrame(RuntimeEffectBackend, _contextGeneration, _runtimeEffectContextOwner);
            _frameTrace.Record(DorotiFramePhase.raster, _viewId, rasterStart,
                frame.InputSequence, frame.SceneSequence, _host.SurfaceGeneration,
                isNewFrame ? null : "retained scene replay", rasterStart - frame.SubmittedAt,
                resizeTargetGeneration: frame.Descriptor.ResizeTargetGeneration,
                metricsGeneration: frame.Descriptor.MetricsGeneration,
                frameworkFrameNumber: frame.Descriptor.FrameworkFrameNumber,
                causalFrameId: causalFrameId,
                contextGeneration: _contextGeneration);
            canvas.Save();
            canvas.ClipRect(SKRect.Create(pixelWidth, pixelHeight), SKClipOperation.Intersect, false);
            try
            {
                DrawScene(canvas, frame.Commands, pixelWidth, pixelHeight);
            }
            finally
            {
                canvas.Restore();
            }
            var rasterEnd = DorotiFrameClock.Now;
            _frameTrace.Record(DorotiFramePhase.rasterEnd, _viewId, rasterEnd,
                frame.InputSequence, frame.SceneSequence, _host.SurfaceGeneration,
                "Doroti draw complete", rasterEnd - rasterStart,
                resizeTargetGeneration: frame.Descriptor.ResizeTargetGeneration,
                metricsGeneration: frame.Descriptor.MetricsGeneration,
                frameworkFrameNumber: frame.Descriptor.FrameworkFrameNumber,
                causalFrameId: causalFrameId,
                contextGeneration: _contextGeneration);
#if !WINDOWS
            canvas.Flush();
#endif
            var surfaceGeneration = _host.SurfaceGeneration;
            if (isNewFrame)
            {
                frame.FrameTransaction?.BackingStoreReady(
                    $"{_targetIdentity}/skia-surface/{surfaceGeneration}",
                    pixelWidth,
                    pixelHeight,
                    desiredTarget.DeviceScaleX,
                    desiredTarget.DeviceScaleY);
            }
            lock (_gate)
            {
                if (isNewFrame)
                {
                    _rasterizedFrames[frame.SceneSequence] = frame;
                }
            }
            var completion = new SkiaPaintCompletion(
                frame.InputSequence, frame.SceneSequence, surfaceGeneration, isNewFrame,
                frame.Descriptor, causalFrameId);
            return new(
                isNewFrame ? SkiaPaintDisposition.exact : SkiaPaintDisposition.replay,
                completion,
                frame.Descriptor,
                match);
        }
        catch
        {
            lock (_gate)
            {
                if (isNewFrame)
                    MarkTerminal(frame, DorotiFrameTerminal.failed, "raster failure");
            }
            throw;
        }
    }

    public void CompletePaint(
        SkiaPaintCompletion completion,
        DorotiFrameTerminal terminal = DorotiFrameTerminal.presented)
    {
        if (terminal is not DorotiFrameTerminal.presented and not DorotiFrameTerminal.submitted)
            throw new ArgumentOutOfRangeException(nameof(terminal));
        if (_disposed) return;
        SkiaFrameReceipt? receipt = null;
        lock (_gate)
        {
            if (completion.IsNewFrame)
            {
                if (!_rasterizedFrames.Remove(completion.SceneSequence, out var frame)) return;
                try
                {
                    frame.FrameTransaction?.VisibleSurfaceCommitted(
                        frame.FrameTransaction.VisibleTargetIdentity);
                }
                catch
                {
                    MarkTerminal(frame, DorotiFrameTerminal.failed,
                        "visible surface transaction commit failed", completion.SurfaceGeneration);
                    throw;
                }
                if (!MarkTerminal(frame, terminal, "native frame submitted",
                    completion.SurfaceGeneration)) return;
                _presentedFrame = frame;
                receipt = CreateFrameReceipt(completion, terminal,
                    SkiaPaintDisposition.exact, "new scene crossed the host submission boundary");
            }
            else
            {
                _replayed++;
                _frameTrace.Record(DorotiFramePhase.replay, _viewId, DorotiFrameClock.Now,
                    completion.InputSequence, completion.SceneSequence, completion.SurfaceGeneration,
                    "fresh native back buffer submitted",
                    resizeTargetGeneration: completion.Descriptor.ResizeTargetGeneration,
                    metricsGeneration: completion.Descriptor.MetricsGeneration,
                    frameworkFrameNumber: completion.Descriptor.FrameworkFrameNumber,
                    causalFrameId: completion.CausalFrameId,
                    contextGeneration: _contextGeneration);
                receipt = CreateFrameReceipt(completion, terminal,
                    SkiaPaintDisposition.replay, "retained scene replay crossed the host submission boundary");
            }
        }
        PublishFrameReceipt(receipt);
    }

    public void FailPaint(SkiaPaintCompletion completion, string reason)
    {
        if (_disposed) return;
        SkiaFrameReceipt? receipt = null;
        lock (_gate)
        {
            if (!completion.IsNewFrame)
                receipt = CreateFrameReceipt(completion, DorotiFrameTerminal.failed,
                    SkiaPaintDisposition.replay, reason);
            else if (_rasterizedFrames.Remove(completion.SceneSequence, out var frame) &&
                MarkTerminal(frame, DorotiFrameTerminal.failed, reason, completion.SurfaceGeneration))
                receipt = CreateFrameReceipt(completion, DorotiFrameTerminal.failed,
                    SkiaPaintDisposition.exact, reason);
        }
        PublishFrameReceipt(receipt);
    }

    public void SupersedePaint(SkiaPaintCompletion completion, string reason)
    {
        if (_disposed) return;
        SkiaFrameReceipt? receipt = null;
        lock (_gate)
        {
            if (!completion.IsNewFrame)
                receipt = CreateFrameReceipt(completion, DorotiFrameTerminal.superseded,
                    SkiaPaintDisposition.replay, reason);
            else if (_rasterizedFrames.Remove(completion.SceneSequence, out var frame) &&
                MarkTerminal(frame, DorotiFrameTerminal.superseded, reason, completion.SurfaceGeneration))
                receipt = CreateFrameReceipt(completion, DorotiFrameTerminal.superseded,
                    SkiaPaintDisposition.superseded, reason);
        }
        PublishFrameReceipt(receipt);
    }

    public void DropPaint(SkiaPaintCompletion completion, string reason)
    {
        if (_disposed) return;
        SkiaFrameReceipt? receipt = null;
        lock (_gate)
        {
            if (!completion.IsNewFrame)
                receipt = CreateFrameReceipt(completion, DorotiFrameTerminal.dropped,
                    SkiaPaintDisposition.replay, reason);
            else if (_rasterizedFrames.Remove(completion.SceneSequence, out var frame) &&
                MarkTerminal(frame, DorotiFrameTerminal.dropped, reason, completion.SurfaceGeneration))
                receipt = CreateFrameReceipt(completion, DorotiFrameTerminal.dropped,
                    SkiaPaintDisposition.exact, reason);
        }
        PublishFrameReceipt(receipt);
    }

    public Paragraph Layout(ParagraphRequest request, DartUiInvocation invocation)
    {
        var started = DorotiFrameClock.Now;
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_paintGate)
        {
            var textRuns = NormalizeTextRuns(request);
            var advances = MeasureTextRuns(request, textRuns);
            var naturalWidth = advances.Sum();
            var width = double.IsFinite(request.Width) ? Math.Min(request.Width, naturalWidth) : naturalWidth;
            var ascent = 0.0;
            var descent = 0.0;
            var metricRuns = textRuns.Count == 0
                ? new[] { new ParagraphTextRun(request.Text, new TextStyle(fontFamily: request.FontFamily, fontSize: request.FontSize)) }
                : textRuns;
            foreach (var run in metricRuns)
            {
                var style = run.Style;
                var resources = GetTextRenderResources(style.fontFamily ?? request.FontFamily,
                    (float)(style.fontSize ?? request.FontSize), SKColors.Black, style);
                var metrics = resources.Metrics(run.Text);
                var naturalHeight = metrics.Ascent + metrics.Descent;
                var lineHeight = style.height is { } multiplier ? (style.fontSize ?? request.FontSize) * multiplier
                    : request.Height ?? naturalHeight;
                var extra = lineHeight - naturalHeight;
                var above = style.leadingDistribution == TextLeadingDistribution.even
                    ? extra / 2 : extra * metrics.Ascent / Math.Max(1, naturalHeight);
                ascent = Math.Max(ascent, metrics.Ascent + above);
                descent = Math.Max(descent, metrics.Descent + extra - above);
            }
            var paragraph = new Paragraph(
                request.Text,
                width,
                ascent + descent,
                request.FontSize,
                request.MaxLines,
                request.FontFamily,
                request.Color,
                advances,
                textRuns)
            {
                NativeAlphabeticBaseline = ascent,
                LayoutTextAlign = request.TextAlign ?? TextAlign.start,
                LayoutTextDirection = request.TextDirection ?? TextDirection.ltr,
            };
            paragraph.layout(new ParagraphConstraints(request.Width));
            _paragraphCount++;
            _paragraphMicroseconds += (DorotiFrameClock.Now - started).Ticks / 10;
            return paragraph;
        }
    }

    public ValueTask<UiImage> DecodeAsync(ReadOnlyMemory<byte> bytes, DartUiInvocation invocation,
        CancellationToken cancellationToken = default) =>
        DecodeSizedAsync(bytes, static (_, _) => null, false, invocation, cancellationToken);

    public async ValueTask<UiImage> DecodeSizedAsync(ReadOnlyMemory<byte> bytes, Func<long, long, TargetImageSize?> targetSize,
        bool allowUpscaling, DartUiInvocation invocation, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();
        using var data = SKData.CreateCopy(bytes.Span);
        using var codec = SKCodec.Create(data) ?? throw new InvalidDataException("SkiaSharp could not open the image resource.");
        var original = codec.Info;
        var target = ImageDecodeSizing.Resolve(original.Width, original.Height, targetSize(original.Width, original.Height), allowUpscaling);
        // Run the target-size callback on the calling context. Only pixel work
        // moves off-thread on hosts with threads; Web still benefits from scaled decode.
        return await Task.Run(() =>
        {
            var scale = Math.Min(1, Math.Max((double)target.Width / original.Width, (double)target.Height / original.Height));
            // GetScaledDimensions' float P/Invoke aborts the trimmed .NET 10
            // WASM runtime. Ask GetPixels to validate the requested scale there;
            // unsupported codec scales use intrinsic pixels and then resample.
            var scaled = OperatingSystem.IsBrowser()
                ? new SKSizeI(Math.Max(1, (int)Math.Round(original.Width * scale)), Math.Max(1, (int)Math.Round(original.Height * scale)))
                : codec.GetScaledDimensions((float)scale);
            var info = new SKImageInfo(scaled.Width, scaled.Height, SKColorType.Rgba8888, SKAlphaType.Premul, original.ColorSpace);
            using var bitmap = DecodePixels(codec, info, original);
            cancellationToken.ThrowIfCancellationRequested();
            SKBitmap? resized = null;
            try
            {
                var pixels = bitmap;
                if (bitmap.Width != target.Width || bitmap.Height != target.Height)
                    pixels = resized = bitmap.Resize(new SKSizeI(target.Width, target.Height), new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None))
                        ?? throw new InvalidDataException("SkiaSharp could not resize the decoded image.");
                pixels.SetImmutable();
                var handle = new SkiaImageHandle(SKImage.FromBitmap(pixels));
                return new UiImage(_viewId, target.Width, target.Height, handle.Release) { HostHandle = handle };
            }
            finally { resized?.Dispose(); }
        }, cancellationToken).ConfigureAwait(false);
    }

    private static SKBitmap DecodePixels(SKCodec codec, SKImageInfo requested, SKImageInfo original)
    {
        var bitmap = new SKBitmap(requested);
        try
        {
            var result = codec.GetPixels(requested, bitmap.GetPixels());
            if (result == SKCodecResult.InvalidScale && (requested.Width != original.Width || requested.Height != original.Height))
            {
                bitmap.Dispose();
                var full = new SKImageInfo(original.Width, original.Height, requested.ColorType, requested.AlphaType, requested.ColorSpace);
                bitmap = new SKBitmap(full);
                result = codec.GetPixels(full, bitmap.GetPixels());
            }
            if (result is not SKCodecResult.Success and not SKCodecResult.IncompleteInput)
                throw new InvalidDataException($"SkiaSharp could not decode the image resource: {result}.");
            return bitmap;
        }
        catch { bitmap.Dispose(); throw; }
    }

    public ValueTask<UiImage> RasterizeAsync(Picture picture, int width, int height,
        DartUiInvocation invocation, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ObjectDisposedException.ThrowIf(picture.debugDisposed, picture);
        cancellationToken.ThrowIfCancellationRequested();
        if (width <= 0 || height <= 0 || (long)width * height > int.MaxValue / 4)
            throw new ArgumentOutOfRangeException(nameof(width));
        lock (_paintGate)
        {
            // Offscreen pictures have their own storage and never borrow the visible swapchain.
            ObjectDisposedException.ThrowIf(_disposed, this);
            using var colorSpace = SKColorSpace.CreateSrgb();
            using var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul, colorSpace))
                ?? throw new InvalidOperationException("Skia could not allocate picture storage.");
            surface.Canvas.Clear(SKColors.Transparent);
            var previousScale = _shadowDeviceScale;
            _shadowDeviceScale = 1;
            try { DrawPicture(surface.Canvas, picture.Commands); }
            finally { _shadowDeviceScale = previousScale; }
            surface.Canvas.Flush();
            var handle = new SkiaImageHandle(surface.Snapshot());
            return ValueTask.FromResult(new UiImage(_viewId, width, height, handle.Release) { HostHandle = handle });
        }
    }

    public void SetEnabled(bool enabled, DartUiInvocation invocation)
    {
        _semanticsEnabled = enabled;
        if (!enabled)
        {
            _semantics.Clear();
            _host.ClearSemantics();
        }
    }

    public void Update(SemanticsUpdate update, DartUiInvocation invocation)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!_semanticsEnabled) return;
        foreach (var node in update.nodes) _semantics[node.id] = node;
        PruneUnreachableSemantics(_semantics);
        var nodes = SemanticsGeometryProjection.ToViewCoordinates(_semantics.Values, update.viewDevicePixelRatio)
            .OrderBy(node => node.indexInParent ?? int.MaxValue)
            .ThenBy(node => node.id)
            .ToArray();
        _host.UpdateSemantics(new SemanticsUpdate(update.generation, nodes, update.urgency));
    }

    private static void PruneUnreachableSemantics(Dictionary<int, SemanticsNodeUpdate> nodes)
    {
        const int rootNodeId = 0;
        if (!nodes.ContainsKey(rootNodeId)) return;
        var reachable = new HashSet<int>();
        var pending = new Stack<int>();
        pending.Push(rootNodeId);
        while (pending.TryPop(out var nodeId))
        {
            if (!reachable.Add(nodeId) || !nodes.TryGetValue(nodeId, out var node)) continue;
            foreach (var childId in node.children) pending.Push(childId);
        }
        foreach (var staleId in nodes.Keys.Where(id => !reachable.Contains(id)).ToArray())
            nodes.Remove(staleId);
    }

    public ValueTask RegisterFontAsync(ReadOnlyMemory<byte> bytes, string? family, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_paintGate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _fallbackFonts.Register(bytes, family);
            foreach (var resources in _textRenderResources.Values) resources.Dispose();
            _textRenderResources.Clear();
            ClearPictureRasterCache();
        }
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _host.SemanticsAction -= HandleSemanticsAction;
        _host.InputReceived -= HandleInput;
        _host.ConfigurationChanged -= HandleConfigurationChanged;
        lock (_paintGate)
        {
            DorotiSkiaImageFilterRenderer.ReleaseContext(RuntimeEffectBackend, _contextGeneration, _runtimeEffectContextOwner);
            DorotiSkiaRuntimeEffects.ReleaseContext(RuntimeEffectBackend, _contextGeneration, _runtimeEffectContextOwner);
            lock (_gate)
            {
                if (_pendingFrame is { } pending)
                    MarkTerminal(pending, DorotiFrameTerminal.dropped, "renderer disposed");
                foreach (var frame in _rasterizedFrames.Values.ToArray())
                    MarkTerminal(frame, DorotiFrameTerminal.dropped, "renderer disposed before present");
                _rasterizedFrames.Clear();
                _pendingFrame = null;
                _presentedFrame = null;
                _invalidate = null;
            }
            foreach (var resources in _textRenderResources.Values) resources.Dispose();
            _textRenderResources.Clear();
            foreach (var filter in _imageFilterResources.Values) filter.Dispose();
            _imageFilterResources.Clear();
            ClearPictureRasterCache();
        }
        _semantics.Clear();
        if (_ownsFallbackFonts) _fallbackFonts.Dispose();
    }

    private void HandleSemanticsAction(int nodeId, SemanticsAction action, object? arguments)
    {
        if (!_disposed) _action?.Invoke(new(_viewId, nodeId, action, arguments));
    }

    private void HandleInput(long sequence, TimeSpan timestamp) =>
        _frameTrace.Record(DorotiFramePhase.input, _viewId, timestamp, sequence,
            surfaceGeneration: _host.SurfaceGeneration);

    private void HandleConfigurationChanged(PlatformConfiguration configuration)
    {
        lock (_paintGate) _backgroundColor = ResolveBackgroundColor(configuration.platformBrightness);
        _host.RequestInvalidate();
    }

    private SKColor ResolveBackgroundColor(Brightness brightness)
    {
        var color = brightness == Brightness.dark
            ? _darkBackgroundColor ?? _lightBackgroundColor ?? new UiColor(0xff141218L)
            : _lightBackgroundColor ?? new UiColor(0xfffffbfeL);
        return new SKColor(
            checked((byte)color.red), checked((byte)color.green),
            checked((byte)color.blue), checked((byte)color.alpha));
    }

    private sealed record SceneFrame(
        long SceneSequence,
        long InputSequence,
        TimeSpan SubmittedAt,
        DorotiFrameDescriptor Descriptor,
        IReadOnlyList<SceneCommand> Commands,
        DorotiFrameTransaction? FrameTransaction);

    private static SkiaFrameReceipt CreateFrameReceipt(
        SkiaPaintCompletion completion,
        DorotiFrameTerminal terminal,
        SkiaPaintDisposition disposition,
        string reason) => new(
        completion.CausalFrameId,
        completion.InputSequence,
        completion.SceneSequence,
        completion.SurfaceGeneration,
        completion.Descriptor,
        terminal,
        DorotiFrameClock.Now,
        completion.IsNewFrame,
        disposition,
        reason);

    private void PublishFrameReceipt(SkiaFrameReceipt? receipt)
    {
        if (receipt is not { } value) return;
        Interlocked.Increment(ref _causalPaintAttempts);
        FrameReceipt?.Invoke(value);
    }

    private bool MarkTerminal(
        SceneFrame frame,
        DorotiFrameTerminal terminal,
        string reason,
        long? surfaceGeneration = null)
    {
        if (!_terminalLedger.TryComplete(frame.SceneSequence, terminal)) return false;
        frame.FrameTransaction?.TryComplete(terminal, reason);
        var phase = terminal switch
        {
            DorotiFrameTerminal.presented or DorotiFrameTerminal.submitted => DorotiFramePhase.present,
            DorotiFrameTerminal.superseded => DorotiFramePhase.superseded,
            DorotiFrameTerminal.dropped => DorotiFramePhase.dropped,
            _ => DorotiFramePhase.failed,
        };
        switch (terminal)
        {
            case DorotiFrameTerminal.presented:
                _presented++;
                _lastPresentedInputSequence = frame.InputSequence;
                break;
            case DorotiFrameTerminal.submitted:
                _submitted++;
                _lastSubmittedInputSequence = frame.InputSequence;
                break;
            case DorotiFrameTerminal.superseded:
                _superseded++;
                break;
            case DorotiFrameTerminal.dropped:
                _dropped++;
                break;
            case DorotiFrameTerminal.failed:
                _failed++;
                break;
        }
        _frameTrace.Record(phase, _viewId, DorotiFrameClock.Now,
            frame.InputSequence, frame.SceneSequence,
            surfaceGeneration ?? _host.SurfaceGeneration,
            reason,
            resizeTargetGeneration: frame.Descriptor.ResizeTargetGeneration,
            metricsGeneration: frame.Descriptor.MetricsGeneration,
            frameworkFrameNumber: frame.Descriptor.FrameworkFrameNumber,
            contextGeneration: _contextGeneration);
        return true;
    }

    private void DrawScene(
        SKCanvas canvas,
        IReadOnlyList<SceneCommand> commands,
        int pixelWidth,
        int pixelHeight) =>
        DrawScene(canvas, commands, 0, commands.Count, pixelWidth, pixelHeight);

    private void DrawScene(
        SKCanvas canvas,
        IReadOnlyList<SceneCommand> commands,
        int start,
        int end,
        int pixelWidth,
        int pixelHeight)
    {
        var restoreCounts = new Stack<int>();
        DrawCommands(commands, start, end);
        if (restoreCounts.Count != 0)
            throw new InvalidDataException($"Doroti Skia scene has {restoreCounts.Count} unclosed scopes.");

        void DrawCommands(IReadOnlyList<SceneCommand> source, int sourceStart = 0, int sourceEnd = -1)
        {
            if (sourceEnd < 0) sourceEnd = source.Count;
            for (var commandIndex = sourceStart; commandIndex < sourceEnd; commandIndex++)
            {
                var command = source[commandIndex];
                switch (command.Operation)
                {
                    case "picture" when command.HostPayload is ScenePicturePayload picture:
                        canvas.Save();
                        canvas.Translate((float)picture.Offset.dx, (float)picture.Offset.dy);
                        DrawPictureLayer(canvas, picture);
                        canvas.Restore();
                        break;
                    case "offset" when command.HostPayload is SceneOffsetPayload offset:
                        canvas.Save();
                        canvas.Translate((float)offset.Dx, (float)offset.Dy);
                        restoreCounts.Push(1);
                        break;
                    case "clipRect" when command.HostPayload is SceneClipRectPayload clip:
                        canvas.Save();
                        restoreCounts.Push(1);
                        canvas.ClipRect(ToRect(clip.Rect), SKClipOperation.Intersect, true);
                        break;
                    case "clipRRect" when command.HostPayload is SceneClipRRectPayload clip:
                        canvas.Save(); restoreCounts.Push(1);
                        using (var path = ToPath(clip.RRect)) canvas.ClipPath(path, SKClipOperation.Intersect, true);
                        break;
                    case "clipRSuperellipse" when command.HostPayload is SceneClipRSuperellipsePayload clip:
                        canvas.Save(); restoreCounts.Push(1);
                        using (var path = SkiaRSuperellipsePath.Create(clip.RSuperellipse)) canvas.ClipPath(path, SKClipOperation.Intersect, true);
                        break;
                    case "clipPath" when command.HostPayload is SceneClipPathPayload clip:
                        canvas.Save(); restoreCounts.Push(1);
                        using (var path = ToPath(clip.Path)) canvas.ClipPath(path, SKClipOperation.Intersect, true);
                        break;
                    case "transform" when command.HostPayload is SceneTransformPayload transform:
                        canvas.Save();
                        restoreCounts.Push(1);
                        Concat(canvas, transform.Matrix4);
                        break;
                    case "opacity" when command.HostPayload is SceneOpacityPayload opacity:
                        using (var paint = new SKPaint { Color = SKColors.White.WithAlpha((byte)Math.Clamp(Math.Round(opacity.Opacity * 255), 0, 255)) })
                            canvas.SaveLayer(paint);
                        restoreCounts.Push(1); canvas.Translate((float)opacity.Offset.dx, (float)opacity.Offset.dy); break;
                    case "colorFilter" when command.HostPayload is SceneColorFilterPayload:
                        canvas.SaveLayer(); restoreCounts.Push(1); break;
                    case "shaderMask" when command.HostPayload is SceneShaderMaskPayload mask:
                        {
                            // A shader mask is a source drawn into the alpha of its already-rendered
                            // child. Supplying the shader paint to SaveLayer instead applies srcIn
                            // against the scene behind the child and can fill the entire mask bounds.
                            var matchingPop = FindMatchingPop(source, commandIndex, sourceEnd);
                            var bounds = ToRect(mask.MaskRect);
                            canvas.SaveLayer(bounds, null);
                            DrawCommands(source, commandIndex + 1, matchingPop);
                            using (var shader = ToShader(mask.Shader))
                            using (var paint = new SKPaint
                            {
                                Shader = shader,
                                BlendMode = ToBlend(mask.BlendMode),
                                IsAntialias = true,
                            })
                                canvas.DrawRect(bounds, paint);
                            canvas.Restore();
                            commandIndex = matchingPop;
                            break;
                        }
                    case "imageFilter" when command.HostPayload is SceneImageFilterPayload image &&
                                                  image.Filter.Shader is FragmentShaderSnapshot fragment:
                        {
                            var matchingPop = FindMatchingPop(source, commandIndex, sourceEnd);
                            var offset = new SKPoint((float)image.Offset.dx, (float)image.Offset.dy);
                            var bounds = image.Bounds is { } explicitBounds
                                ? ToRect(explicitBounds)
                                : new SKRect(
                                    canvas.LocalClipBounds.Left - offset.X,
                                    canvas.LocalClipBounds.Top - offset.Y,
                                    canvas.LocalClipBounds.Right - offset.X,
                                    canvas.LocalClipBounds.Bottom - offset.Y);
                            var rendered = DorotiSkiaImageFilterRenderer.Draw(
                                canvas,
                                pixelWidth,
                                pixelHeight,
                                fragment,
                                bounds,
                                offset,
                                ToSamplingOptions(image.Filter.FilterQuality),
                                CreateImageShader,
                                (inputCanvas, inputWidth, inputHeight) =>
                                    DrawScene(inputCanvas, source, commandIndex + 1, matchingPop, inputWidth, inputHeight),
                                RuntimeEffectBackend,
                                _contextGeneration,
                                image.CacheKey,
                                image.CacheGeneration,
                                out var cacheHit, _runtimeEffectContextOwner);
                            if (rendered && !cacheHit)
                                Interlocked.Increment(ref _shaderImageFiltersRendered);
                            commandIndex = matchingPop;
                            break;
                        }
                    case "imageFilter" when command.HostPayload is SceneImageFilterPayload image:
                        canvas.Save();
                        var imageRestoreCount = 1;
                        if (image.Filter.Matrix4 is not null && image.Filter.Outer is null &&
                            image.Filter.Inner is null && image.Filter.ColorFilter is null && image.Filter.Shader is null)
                        {
                            // Doroti retains vector scene commands, so replay a pure matrix image filter as
                            // an equivalent scene transform. An unbounded GPU SaveLayer + matrix filter can
                            // produce an empty texture while Android is animating stretch overscroll.
                            Concat(canvas, image.Filter.Matrix4);
                        }
                        else
                        {
                            using var paint = FilterPaint(image.Filter);
                            canvas.SaveLayer(paint);
                            imageRestoreCount++;
                        }
                        canvas.Translate((float)image.Offset.dx, (float)image.Offset.dy);
                        restoreCounts.Push(imageRestoreCount);
                        break;
                    case "backdropFilter" when command.HostPayload is SceneBackdropFilterPayload backdrop:
                        using (var paint = new SKPaint { BlendMode = ToBlend(backdrop.BlendMode) })
                        {
                            var restoreCount = 1;
                            if (backdrop.Filter.Bounds is { } clipBounds)
                            {
                                canvas.Save();
                                canvas.ClipRect(ToRect(clipBounds), SKClipOperation.Intersect, true);
                                restoreCount++;
                            }
                            var layer = new SKCanvasSaveLayerRec
                            {
                                Backdrop = GetImageFilter(backdrop.Filter),
                                Bounds = backdrop.Filter.Bounds is { } bounds ? ToRect(bounds) : null,
                                Paint = paint,
                            };
                            canvas.SaveLayer(layer);
                            restoreCounts.Push(restoreCount);
                        }
                        break;
                    case "retained" when command.HostPayload is SceneRetainedPayload retained:
                        DrawCommands(retained.Commands); break;
                    case "pop" when restoreCounts.Count > 0:
                        for (var count = restoreCounts.Pop(); count > 0; count--) canvas.Restore();
                        break;
                    default:
                        throw new NotSupportedException($"Doroti scene operation '{command.Operation}' has no Skia GPU mapping.");
                }
            }
        }
    }

    private static int FindMatchingPop(IReadOnlyList<SceneCommand> source, int scopeStart, int end)
    {
        var depth = 1;
        for (var index = scopeStart + 1; index < end; index++)
        {
            if (IsSceneScopeStart(source[index].Operation))
            {
                depth++;
            }
            else if (source[index].Operation == "pop" && --depth == 0)
            {
                return index;
            }
        }
        throw new InvalidDataException(
            $"Doroti scene image-filter scope at command {scopeStart} has no matching pop.");
    }

    private static bool IsSceneScopeStart(string operation) => operation is
        "offset" or "clipRect" or "clipRRect" or "clipRSuperellipse" or "clipPath" or
        "transform" or "opacity" or "colorFilter" or "shaderMask" or "imageFilter" or
        "backdropFilter";

    private void DrawPicture(SKCanvas canvas, IReadOnlyList<PathCommand> commands)
    {
        foreach (var command in commands)
        {
            switch (command.Operation)
            {
                case "save": canvas.Save(); break;
                case "saveLayer" when command.HostPayload is CanvasSaveLayerPayload layer:
                    using (var paint = ToPaint(layer.Paint))
                    {
                        if (layer.Bounds is { } bounds) canvas.SaveLayer(ToRect(bounds), paint);
                        else canvas.SaveLayer(paint);
                    }
                    break;
                case "restore": canvas.Restore(); break;
                case "translate": canvas.Translate((float)command.Arguments[0], (float)command.Arguments[1]); break;
                case "scale": canvas.Scale((float)command.Arguments[0], (float)command.Arguments[1]); break;
                case "rotate": canvas.RotateRadians((float)command.Arguments[0]); break;
                case "transform": Concat(canvas, command.Arguments); break;
                case "clipRect": canvas.ClipRect(new((float)command.Arguments[0], (float)command.Arguments[1], (float)command.Arguments[2], (float)command.Arguments[3]), SKClipOperation.Intersect, true); break;
                case "clipRRect" when command.HostPayload is CanvasClipRRectPayload clip: using (var path = ToPath(clip.RRect)) canvas.ClipPath(path, SKClipOperation.Intersect, clip.DoAntiAlias); break;
                case "clipRSuperellipse" when command.HostPayload is CanvasClipRSuperellipsePayload clip: using (var path = SkiaRSuperellipsePath.Create(clip.RSuperellipse)) canvas.ClipPath(path, SKClipOperation.Intersect, clip.DoAntiAlias); break;
                case "clipPath" when command.HostPayload is CanvasClipPathPayload clip: using (var path = ToPath(clip.Path)) canvas.ClipPath(path, SKClipOperation.Intersect, clip.DoAntiAlias); break;
                case "drawRect" when command.HostPayload is CanvasRectPayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawRect(ToRect(draw.Rect), paint); break;
                case "drawRRect" when command.HostPayload is CanvasRRectPayload draw: DrawRRect(canvas, draw); break;
                case "drawDRRect" when command.HostPayload is CanvasDRRectPayload draw: DrawDRRect(canvas, draw); break;
                case "drawRSuperellipse" when command.HostPayload is CanvasRSuperellipsePayload draw: using (var path = SkiaRSuperellipsePath.Create(draw.RSuperellipse)) using (var paint = ToPaint(draw.Paint)) canvas.DrawPath(path, paint); break;
                case "drawPath" when command.HostPayload is CanvasPathPayload draw: using (var path = ToPath(draw.Path)) using (var paint = ToPaint(draw.Paint)) canvas.DrawPath(path, paint); break;
                case "drawPaint" when command.HostPayload is PaintSnapshot draw: using (var paint = ToPaint(draw)) canvas.DrawPaint(paint); break;
                case "drawCircle" when command.HostPayload is CanvasCirclePayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawCircle((float)draw.Center.dx, (float)draw.Center.dy, (float)draw.Radius, paint); break;
                case "drawOval" when command.HostPayload is CanvasOvalPayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawOval(ToRect(draw.Rect), paint); break;
                case "drawLine" when command.HostPayload is CanvasLinePayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawLine((float)draw.Start.dx, (float)draw.Start.dy, (float)draw.End.dx, (float)draw.End.dy, paint); break;
                case "drawPoints" or "drawRawPoints" when command.HostPayload is CanvasPointsPayload draw:
                    using (var paint = ToPaint(draw.Paint))
                        canvas.DrawPoints(ToPointMode(draw.PointMode), draw.Points.Select(ToPoint).ToArray(), paint);
                    break;
                case "drawArc" when command.HostPayload is CanvasArcPayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawArc(ToRect(draw.Rect), (float)(draw.StartAngle * 180 / Math.PI), (float)(draw.SweepAngle * 180 / Math.PI), draw.UseCenter, paint); break;
                case "drawColor" when command.HostPayload is CanvasColorPayload draw: canvas.DrawColor(ToColor(draw.Color), ToBlend(draw.BlendMode)); break;
                case "drawParagraph" when command.HostPayload is CanvasParagraphPayload draw:
                    DrawParagraphText(canvas, draw.Paragraph, draw.Offset);
                    break;
                case "drawImageRect" or "drawImage" when command.HostPayload is CanvasImagePayload draw && draw.Image.HostHandle is SkiaImageHandle handle:
                    using (var paint = ToPaint(draw.Paint))
                        canvas.DrawImage(handle.Image, ToRect(draw.Source), ToRect(draw.Destination),
                            ToSamplingOptions(draw.Paint.FilterQuality), paint);
                    break;
                case "drawShadow" when command.HostPayload is CanvasShadowPayload draw:
                    DrawShadow(canvas, draw);
                    break;
                default: throw new NotSupportedException($"Doroti canvas operation '{command.Operation}' has no Skia GPU mapping.");
            }
        }
    }

    private void DrawPictureLayer(SKCanvas canvas, ScenePicturePayload payload)
    {
        var commands = payload.Commands;
        if (!_enablePictureRasterCache)
        {
            DrawRetainedPicture(canvas, payload);
            return;
        }

        var cacheKey = (object)commands;
        if (payload.WillChangeHint || payload.CanvasBounds is not { } canvasBounds ||
            !canvasBounds.IsFinite || canvasBounds.isEmpty ||
            (!payload.IsComplexHint && commands.Count < PictureRasterComplexityThreshold &&
                !HasDownscaledImage(commands)) ||
            !SkiaGpuSurfaces.IsGpu(canvas) || !PictureCanCompositeOverBackground(commands) || HasBlurredPaint(commands))
        {
            DrawRetainedPicture(canvas, payload);
            return;
        }

        var transform = canvas.TotalMatrix;
        // Perspective changes the sampling geometry; it is not translation-only
        // reuse and must go through the ordinary draw path.
        if (transform.Persp0 != 0 || transform.Persp1 != 0 || transform.Persp2 != 1)
        {
            DrawRetainedPicture(canvas, payload);
            return;
        }
        var mappedBounds = transform.MapRect(ToRect(canvasBounds));
        var rasterExtent = PictureRasterExtent(transform, ToRect(canvasBounds));
        if (!IsFinite(mappedBounds) || mappedBounds.Width <= 0 || mappedBounds.Height <= 0)
        {
            DrawRetainedPicture(canvas, payload);
            return;
        }

        if (mappedBounds.Width > MaxCacheablePicturePixels ||
            mappedBounds.Height > MaxCacheablePicturePixels)
        {
            DrawRetainedPicture(canvas, payload);
            return;
        }

        // Keep the original device-pixel phase when switching from direct draw
        // to an image. Removing the fractional origin changes glyph hinting;
        // drawing that image back at a fractional position changes sampling too.
        var rasterLeft = MathF.Floor(mappedBounds.Left);
        var rasterTop = MathF.Floor(mappedBounds.Top);
        var phaseX = mappedBounds.Left - rasterLeft;
        var phaseY = mappedBounds.Top - rasterTop;
        // Use the untranslated extent to avoid cancellation in float endpoints.
        var width = checked((int)Math.Ceiling(rasterExtent.Width + phaseX));
        var height = checked((int)Math.Ceiling(rasterExtent.Height + phaseY));
        var pixels = (long)width * height;
        if (pixels <= 0 || pixels > MaxCacheablePicturePixels)
        {
            DrawRetainedPicture(canvas, payload);
            return;
        }

        // Font rasterization also depends on the destination surface policy.
        // In particular, device-independent fonts must not switch to hinted
        // glyphs merely because a picture was promoted to an offscreen image.
        using var surfaceProperties = canvas.Surface?.SurfaceProperties;
        var signature = PictureRasterTransform.From(transform, phaseX, phaseY,
            surfaceProperties?.Flags ?? SKSurfacePropsFlags.None,
            surfaceProperties?.PixelGeometry ?? SKPixelGeometry.Unknown);
        if (_pictureRasterCache.TryGetValue(cacheKey, out var cached))
        {
            if (cached.Recording?.IsDiscarded != true && cached.Width == width && cached.Height == height && cached.Transform == signature)
            {
                cached.LastUsedSequence = ++_pictureRasterUseSequence;
                DrawRasterImage(canvas, cached.Image, rasterLeft, rasterTop);
                Interlocked.Increment(ref _pictureRasterCacheHits);
                return;
            }
            RemovePictureRaster(cacheKey, cached);
        }

        if (!_pictureRasterWarmups.TryGetValue(cacheKey, out var warmup))
        {
            if (_pictureRasterWarmups.Count >= MaxPictureRasterWarmups)
                RemovePictureWarmup(_pictureRasterWarmupOrder.First!.Value);
            warmup = new PictureRasterWarmup(_pictureRasterWarmupOrder.AddLast(cacheKey));
            _pictureRasterWarmups.Add(cacheKey, warmup);
        }
        else
        {
            _pictureRasterWarmupOrder.Remove(warmup.Node);
            _pictureRasterWarmupOrder.AddLast(warmup.Node);
        }
        // A moving subpixel phase cannot reuse this raster. Wait until it is
        // stable instead of promoting a new image on every animation tick.
        warmup.Uses = warmup.Transform == signature
            ? Math.Min(PictureRasterWarmupFrames, warmup.Uses + 1) : 1;
        warmup.Transform = signature;
        warmup.LastFrame = _rasterFrame;
        // Never spend several synchronous surface/replay/flush/snapshot costs
        // in one frame. A single promotion is non-preemptible; record its real
        // duration and render every deferred picture through normal replay.
        if (warmup.Uses < PictureRasterWarmupFrames || _framePromotions >= 2 ||
            _framePromotionPixels + pixels > MaxCacheablePicturePixels ||
            _framePromotionMicroseconds >= PromotionBudgetMicroseconds)
        {
            DrawRetainedPicture(canvas, payload);
            return;
        }

        var promotionStarted = DorotiFrameClock.Now;
        var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SkiaGpuSurfaces.CreateCompatible(canvas, info, surfaceProperties)
            ?? throw new InvalidOperationException(
                $"Doroti picture raster cache could not allocate a {width}x{height} GPU surface.");
        var rasterCanvas = surface.Canvas;
        rasterCanvas.Clear(SKColors.Transparent);
        rasterCanvas.Save();
        rasterCanvas.Translate(-rasterLeft, -rasterTop);
        var matrix = canvas.TotalMatrix;
        rasterCanvas.Concat(in matrix);
        DrawRetainedPicture(rasterCanvas, payload);
        rasterCanvas.Restore();
        rasterCanvas.Flush();
        var image = surface.Snapshot()
            ?? throw new InvalidOperationException("Doroti picture raster cache could not snapshot its GPU surface.");
        cached = new(image, width, height, signature, ++_pictureRasterUseSequence, SkiaGpuSurfaces.RecordingFor(canvas));
        _pictureRasterCache.Add(cacheKey, cached);
        Interlocked.Increment(ref _pictureRasterCacheEntries);
        _pictureRasterPixels += cached.Pixels;
        RemovePictureWarmup(cacheKey);
        DrawRasterImage(canvas, image, rasterLeft, rasterTop);
        TrimPictureRasterCache();
        Interlocked.Increment(ref _pictureRasterCacheMisses);
        var promotionMicroseconds = (DorotiFrameClock.Now - promotionStarted).Ticks / 10;
        _promotionMicroseconds += promotionMicroseconds;
        _promotionMaximumMicroseconds = Math.Max(_promotionMaximumMicroseconds, promotionMicroseconds);
        _framePromotions++;
        _framePromotionPixels += pixels;
        _framePromotionMicroseconds += promotionMicroseconds;
    }

    private void BeginPictureRasterFrame()
    {
        _rasterFrame++;
        _framePromotions = 0;
        _framePromotionPixels = 0;
        _framePromotionMicroseconds = 0;
        while (_pictureRasterWarmupOrder.First is { } first &&
            _rasterFrame - _pictureRasterWarmups[first.Value].LastFrame >= PictureWarmupLifetimeFrames)
            RemovePictureWarmup(first.Value);
    }

    private void RemovePictureWarmup(object key)
    {
        if (_pictureRasterWarmups.Remove(key, out var warmup)) _pictureRasterWarmupOrder.Remove(warmup.Node);
    }

    private sealed class PictureRasterWarmup(LinkedListNode<object> node)
    {
        internal LinkedListNode<object> Node { get; } = node;
        internal int Uses;
        internal PictureRasterTransform? Transform;
        internal long LastFrame;
    }

    private static void DrawRasterImage(SKCanvas canvas, SKImage image, float left, float top)
    {
        canvas.Save();
        canvas.ResetMatrix();
        canvas.DrawImage(image, left, top, SKSamplingOptions.Default);
        canvas.Restore();
    }

    private static bool HasDownscaledImage(IReadOnlyList<PathCommand> commands) =>
        commands.Any(command => command.HostPayload is CanvasImagePayload image &&
            (image.Source.width > image.Destination.width * 2 || image.Source.height > image.Destination.height * 2));

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<object, PictureCompositePolicy> PictureCompositePolicies = new();

    private static bool PictureCanCompositeOverBackground(IReadOnlyList<PathCommand> commands) =>
        PictureCompositePolicies.GetValue(commands, static key => new(((IReadOnlyList<PathCommand>)key).All(static command =>
            command.HostPayload switch
            {
                CanvasColorPayload color => color.BlendMode == BlendMode.srcOver,
                PaintSnapshot paint => paint.BlendMode == BlendMode.srcOver,
                CanvasSaveLayerPayload layer => layer.Paint.BlendMode == BlendMode.srcOver,
                CanvasPathPayload draw => draw.Paint.BlendMode == BlendMode.srcOver,
                CanvasRectPayload draw => draw.Paint.BlendMode == BlendMode.srcOver,
                CanvasRRectPayload draw => draw.Paint.BlendMode == BlendMode.srcOver,
                CanvasRSuperellipsePayload draw => draw.Paint.BlendMode == BlendMode.srcOver,
                CanvasDRRectPayload draw => draw.Paint.BlendMode == BlendMode.srcOver,
                CanvasImagePayload draw => draw.Paint.BlendMode == BlendMode.srcOver,
                CanvasImageNinePayload draw => draw.Paint.BlendMode == BlendMode.srcOver,
                CanvasCirclePayload draw => draw.Paint.BlendMode == BlendMode.srcOver,
                CanvasLinePayload draw => draw.Paint.BlendMode == BlendMode.srcOver,
                CanvasPointsPayload draw => draw.Paint.BlendMode == BlendMode.srcOver,
                CanvasOvalPayload draw => draw.Paint.BlendMode == BlendMode.srcOver,
                CanvasArcPayload draw => draw.Paint.BlendMode == BlendMode.srcOver,
                _ => true,
            }))).CanComposite;

    // A picture cache is drawn with srcOver onto the existing destination. A
    // clear/src/destination-dependent operation cannot be flattened against
    // transparent pixels first. Replay those pictures against the real target.
    private sealed record PictureCompositePolicy(bool CanComposite);

    private static SKSize PictureRasterExtent(SKMatrix matrix, SKRect bounds)
    {
        matrix.TransX = 0;
        matrix.TransY = 0;
        var mapped = matrix.MapRect(bounds);
        return new SKSize(mapped.Width, mapped.Height);
    }

    private void TrimPictureRasterCache()
    {
        while (_pictureRasterCache.Count > MaxPictureRasterCacheEntries ||
               _pictureRasterPixels > MaxPictureRasterPixels)
        {
            // A frame can draw more pictures than the cache holds. Frame numbers
            // tie in that case, and Dictionary reuses removed slots: MinBy could
            // select the newly inserted image and dispose it before its draw.
            // Order every access, including accesses within the same frame.
            var oldest = _pictureRasterCache.MinBy(pair => pair.Value.LastUsedSequence);
            if (oldest.Key is null) break;
            RemovePictureRaster(oldest.Key, oldest.Value);
        }
    }

    private void RemovePictureRaster(object cacheKey, PictureRasterCacheEntry cached)
    {
        _pictureRasterCache.Remove(cacheKey);
        Interlocked.Decrement(ref _pictureRasterCacheEntries);
        _pictureRasterPixels -= cached.Pixels;
        cached.Image.Dispose();
    }

    private void ClearPictureRasterCache()
    {
        ClearPictureCommandCache();
        foreach (var cached in _pictureRasterCache.Values) cached.Image.Dispose();
        _pictureRasterCache.Clear();
        _pictureRasterWarmups.Clear();
        _pictureRasterWarmupOrder.Clear();
        _pictureRasterPixels = 0;
        Interlocked.Exchange(ref _pictureRasterCacheEntries, 0);
    }

    private readonly record struct PictureRasterTransform(
        float ScaleX,
        float SkewX,
        float SkewY,
        float ScaleY,
        float Persp0,
        float Persp1,
        float Persp2,
        float PhaseX,
        float PhaseY,
        SKSurfacePropsFlags SurfaceFlags,
        SKPixelGeometry PixelGeometry)
    {
        internal static PictureRasterTransform From(SKMatrix matrix, float phaseX, float phaseY,
            SKSurfacePropsFlags surfaceFlags, SKPixelGeometry pixelGeometry) => new(
            matrix.ScaleX, matrix.SkewX, matrix.SkewY, matrix.ScaleY,
            matrix.Persp0, matrix.Persp1, matrix.Persp2, phaseX, phaseY, surfaceFlags, pixelGeometry);
    }

    private sealed class PictureRasterCacheEntry(
        SKImage image,
        int width,
        int height,
        PictureRasterTransform transform,
        long lastUsedSequence, SkiaGpuSurfaces.Recording? recording)
    {
        internal SkiaGpuSurfaces.Recording? Recording { get; } = recording;
        internal SKImage Image { get; } = image;
        internal int Width { get; } = width;
        internal int Height { get; } = height;
        internal PictureRasterTransform Transform { get; } = transform;
        internal long LastUsedSequence { get; set; } = lastUsedSequence;
        internal long Pixels => (long)Width * Height;
    }

    private TextRenderResources GetTextRenderResources(string? fontFamily, float fontSize, SKColor color, TextStyle? style = null)
    {
        var key = new TextRenderKey(fontFamily ?? string.Empty, fontSize, color,
            style?.fontWeight?.value ?? 400, style?.fontStyle == FontStyle.italic,
            (float)(style?.letterSpacing ?? 0), (float)(style?.wordSpacing ?? 0),
            System.Text.Json.JsonSerializer.Serialize(style?.fontFamilyFallback ?? []));
        if (_textRenderResources.TryGetValue(key, out var resources)) return resources;
        resources = new TextRenderResources(fontFamily, fontSize, color, _fallbackFonts, key, style?.fontFamilyFallback);
        _textRenderResources.Add(key, resources);
        return resources;
    }

    private static IReadOnlyList<ParagraphTextRun> NormalizeTextRuns(ParagraphRequest request)
    {
        var runs = request.TextRuns ?? [];
        if (runs.Count != 0 &&
            !string.Equals(string.Concat(runs.Select(run => run.Text)), request.Text, StringComparison.Ordinal))
            throw new InvalidDataException("Paragraph text runs must concatenate to the paragraph text.");
        return runs;
    }

    private double[] MeasureTextRuns(ParagraphRequest request, IReadOnlyList<ParagraphTextRun> runs)
    {
        if (runs.Count == 0)
        {
            return GetTextRenderResources(
                    request.FontFamily,
                    (float)request.FontSize,
                    ToColor(request.Color ?? new UiColor(0xFF000000)))
                .MeasureCodeUnitAdvances(request.Text);
        }

        var advances = new double[request.Text.Length];
        var offset = 0;
        foreach (var run in runs)
        {
            var style = run.Style;
            var resources = GetTextRenderResources(
                style.fontFamily ?? request.FontFamily,
                (float)(style.fontSize ?? request.FontSize),
                ToColor(style.foreground?.color ?? style.color ?? request.Color ?? new UiColor(0xFF000000)), style);
            var runAdvances = resources.MeasureCodeUnitAdvances(run.Text);
            Array.Copy(runAdvances, 0, advances, offset, runAdvances.Length);
            offset += runAdvances.Length;
        }
        return advances;
    }

    private void DrawParagraphText(SKCanvas canvas, Paragraph paragraph, Offset offset)
    {
        foreach (var line in paragraph.PaintLines)
        {
            var x = (float)(offset.dx + line.Left);
            var baseline = (float)(offset.dy + line.Baseline);
            if (paragraph.TextRuns.Count == 0)
            {
                GetTextRenderResources(paragraph.fontFamily, (float)paragraph.fontSize, ToColor(paragraph.color))
                    .DrawText(canvas, paragraph.text[line.Start..line.End], x, baseline);
                continue;
            }
            var runStart = 0;
            foreach (var run in paragraph.TextRuns)
            {
                var start = Math.Max(line.Start, runStart);
                var end = Math.Min(line.End, runStart + run.Text.Length);
                if (end > start)
                {
                    var style = run.Style;
                    GetTextRenderResources(style.fontFamily ?? paragraph.fontFamily,
                        (float)(style.fontSize ?? paragraph.fontSize),
                        ToColor(style.foreground?.color ?? style.color ?? paragraph.color), style)
                        .DrawText(canvas, run.Text[(start - runStart)..(end - runStart)], x, baseline);
                    x += (float)paragraph.TextAdvance(start, end);
                }
                runStart += run.Text.Length;
                if (runStart >= line.End) break;
            }
        }
    }

    private readonly record struct TextRenderKey(string FontFamily, float FontSize, SKColor Color,
        int Weight, bool Italic, float LetterSpacing, float WordSpacing, string FallbackFamilies);

    private sealed class TextRenderResources : IDisposable
    {
        private readonly TextFontResource _primary;
        private readonly SkiaFallbackFontCollection? _registeredFallbacks;
        private readonly float _letterSpacing, _wordSpacing;
        private readonly Dictionary<int, TextFontResource> _fallbackByCodePoint = [];
        private readonly Dictionary<string, TextFontResource> _fallbackByFamily =
            new(StringComparer.OrdinalIgnoreCase);

        internal TextRenderResources(
            string? fontFamily,
            float fontSize,
            SKColor color,
            SkiaFallbackFontCollection? registeredFallbacks, TextRenderKey key, IReadOnlyList<string>? fallbackFamilies)
        {
            _registeredFallbacks = registeredFallbacks;
            _letterSpacing = key.LetterSpacing;
            _wordSpacing = key.WordSpacing;
            using var fontStyle = new SKFontStyle(key.Weight, 5, key.Italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright);
            // FromFamilyName silently returns the platform default for an unknown
            // name. Resolve the explicit fallback list before accepting that face.
            var families = new[] { fontFamily }.Concat(fallbackFamilies ?? []).Where(family => !string.IsNullOrWhiteSpace(family));
            SKTypeface? primary = null;
            var ownsPrimary = false;
            foreach (var family in families)
            {
                primary = registeredFallbacks?.MatchFamily(family, fontStyle);
                if (primary is not null) break;
                primary = SKFontManager.Default.MatchFamily(family, fontStyle);
                if (primary is not null) { ownsPrimary = true; break; }
            }
            if (primary is null) { primary = SKTypeface.FromFamilyName(null, fontStyle); ownsPrimary = true; }
            _primary = new TextFontResource(primary, fontSize, ownsTypeface: ownsPrimary);
            Paint = new SKPaint { Color = color, IsAntialias = true };
        }

        internal SKPaint Paint { get; }

        internal (double Ascent, double Descent) Metrics(string text)
        {
            var ascent = -(double)_primary.Font.Metrics.Ascent;
            var descent = (double)_primary.Font.Metrics.Descent;
            for (var index = 0; index < text.Length;)
            {
                var font = ResolveFont(CodePointAt(text, index, out var length)).Font;
                ascent = Math.Max(ascent, -font.Metrics.Ascent);
                descent = Math.Max(descent, font.Metrics.Descent);
                index += length;
            }
            return (ascent, descent);
        }

        internal void DrawText(SKCanvas canvas, string text, float x, float baseline)
        {
            if (text.Length == 0) return;
            if (_letterSpacing != 0 || _wordSpacing != 0)
            {
                for (var index = 0; index < text.Length;)
                {
                    var font = ResolveFont(CodePointAt(text, index, out var length)).Font;
                    var glyph = text.Substring(index, length);
                    canvas.DrawText(glyph, x + _letterSpacing / 2, baseline, SKTextAlign.Left, font, Paint);
                    x += font.MeasureText(glyph, Paint) + _letterSpacing + (glyph == " " ? _wordSpacing : 0);
                    index += length;
                }
                return;
            }

            var runStart = 0;
            var runFont = ResolveFont(CodePointAt(text, 0, out var firstLength));
            for (var index = firstLength; index < text.Length;)
            {
                var codePoint = CodePointAt(text, index, out var codePointLength);
                var font = ResolveFont(codePoint);
                if (!ReferenceEquals(font, runFont))
                {
                    var run = text[runStart..index];
                    canvas.DrawText(run, x, baseline, SKTextAlign.Left, runFont.Font, Paint);
                    x += runFont.Font.MeasureText(run, Paint);
                    runStart = index;
                    runFont = font;
                }
                index += codePointLength;
            }

            canvas.DrawText(text[runStart..], x, baseline, SKTextAlign.Left, runFont.Font, Paint);
        }

        internal double[] MeasureCodeUnitAdvances(string text)
        {
            var advances = new double[text.Length];
            if (text.Length == 0) return advances;

            var runStart = 0;
            var runFont = ResolveFont(CodePointAt(text, 0, out var firstLength));
            for (var index = firstLength; index <= text.Length;)
            {
                TextFontResource? nextFont = null;
                var nextLength = 0;
                if (index < text.Length)
                {
                    nextFont = ResolveFont(CodePointAt(text, index, out nextLength));
                }

                if (index == text.Length || !ReferenceEquals(nextFont, runFont))
                {
                    var run = text.AsSpan(runStart, index - runStart);
                    var widths = runFont.Font.GetGlyphWidths(run, Paint);
                    var glyphIndex = 0;
                    for (var cursor = runStart; cursor < index;)
                    {
                        CodePointAt(text, cursor, out var codePointLength);
                        advances[cursor] = glyphIndex < widths.Length
                            ? Math.Max(0, widths[glyphIndex++])
                            : Math.Max(0, runFont.Font.MeasureText(text.AsSpan(cursor, codePointLength), Paint));
                        advances[cursor] += _letterSpacing + (text[cursor] == ' ' ? _wordSpacing : 0);
                        cursor += codePointLength;
                    }
                    runStart = index;
                    if (nextFont is not null) runFont = nextFont;
                }

                if (index == text.Length) break;
                index += nextLength;
            }
            return advances;
        }

        private static int CodePointAt(string text, int index, out int length)
        {
            var first = text[index];
            if (char.IsHighSurrogate(first) && index + 1 < text.Length && char.IsLowSurrogate(text[index + 1]))
            {
                length = 2;
                return char.ConvertToUtf32(first, text[index + 1]);
            }
            length = 1;
            return first;
        }

        private TextFontResource ResolveFont(int codePoint)
        {
            if (_primary.Font.ContainsGlyph(codePoint)) return _primary;
            if (_fallbackByCodePoint.TryGetValue(codePoint, out var cached)) return cached;

            var registeredTypeface = _registeredFallbacks?.MatchCharacter(codePoint);
            if (registeredTypeface is not null)
            {
                if (!_fallbackByFamily.TryGetValue(registeredTypeface.FamilyName, out var registered))
                {
                    registered = new TextFontResource(
                        registeredTypeface, _primary.Font.Size, ownsTypeface: false);
                    _fallbackByFamily.Add(registered.FamilyName, registered);
                }
                _fallbackByCodePoint.Add(codePoint, registered);
                return registered;
            }

            var matchedTypeface = SKFontManager.Default.MatchCharacter(_primary.FamilyName, codePoint);
            if (matchedTypeface is null)
            {
                _fallbackByCodePoint.Add(codePoint, _primary);
                return _primary;
            }

            if (!_fallbackByFamily.TryGetValue(matchedTypeface.FamilyName, out var fallback))
            {
                fallback = new TextFontResource(matchedTypeface, _primary.Font.Size);
                _fallbackByFamily.Add(fallback.FamilyName, fallback);
            }
            else
            {
                matchedTypeface.Dispose();
            }
            _fallbackByCodePoint.Add(codePoint, fallback);
            return fallback;
        }

        public void Dispose()
        {
            Paint.Dispose();
            foreach (var fallback in _fallbackByFamily.Values) fallback.Dispose();
            _primary.Dispose();
        }

        private sealed class TextFontResource(
            SKTypeface typeface,
            float fontSize,
            bool ownsTypeface = true) : IDisposable
        {
            internal SKTypeface Typeface { get; } = typeface;
            internal SKFont Font { get; } = new(typeface, fontSize);
            internal string FamilyName => Typeface.FamilyName;

            public void Dispose()
            {
                Font.Dispose();
                if (ownsTypeface) Typeface.Dispose();
            }
        }
    }

    private SKPaint ToPaint(PaintSnapshot value)
    {
        var paint = new SKPaint
        {
            Color = ToColor(value.Color),
            Style = value.Style == PaintingStyle.stroke ? SKPaintStyle.Stroke : SKPaintStyle.Fill,
            StrokeWidth = (float)value.StrokeWidth,
            IsAntialias = value.IsAntiAlias,
            BlendMode = ToBlend(value.BlendMode),
            StrokeCap = value.StrokeCap switch { StrokeCap.round => SKStrokeCap.Round, StrokeCap.square => SKStrokeCap.Square, _ => SKStrokeCap.Butt },
            StrokeJoin = value.StrokeJoin switch { StrokeJoin.round => SKStrokeJoin.Round, StrokeJoin.bevel => SKStrokeJoin.Bevel, _ => SKStrokeJoin.Miter },
        };
        if (value.Shader is not null) paint.Shader = ToShader(value.Shader);
        if (value.MaskFilter is { sigma: > 0 } blur)
        {
            using var filter = SKMaskFilter.CreateBlur(blur.style switch
            {
                BlurStyle.solid => SKBlurStyle.Solid,
                BlurStyle.outer => SKBlurStyle.Outer,
                BlurStyle.inner => SKBlurStyle.Inner,
                _ => SKBlurStyle.Normal,
            }, (float)blur.sigma);
            paint.MaskFilter = filter;
        }
        return paint;
    }

    private SKPaint FilterPaint(ImageFilterSnapshot filter) => new() { ImageFilter = GetImageFilter(filter) };

    private SKImageFilter GetImageFilter(ImageFilterSnapshot filter)
    {
        if (_imageFilterResources.TryGetValue(filter, out var resource)) return resource;
        if (_imageFilterResources.Count >= MaxImageFilterResources)
        {
            var oldest = _imageFilterResources.First();
            _imageFilterResources.Remove(oldest.Key);
            oldest.Value.Dispose();
        }
        resource = CreateImageFilter(filter);
        _imageFilterResources.Add(filter, resource);
        return resource;
    }

    private SKImageFilter CreateImageFilter(ImageFilterSnapshot filter)
    {
        if (filter.Shader is not null)
            throw new InvalidOperationException(
                "Shader image filters must be rendered through Doroti's GPU offscreen input path.");
        if (filter.Outer is not null && filter.Inner is not null)
        {
            using var outer = CreateImageFilter(filter.Outer);
            using var inner = CreateImageFilter(filter.Inner);
            return SKImageFilter.CreateCompose(outer, inner);
        }
        if (filter.ColorFilter is not null)
        {
            using var color = ToColorFilter(filter.ColorFilter);
            if (filter.Inner is null) return SKImageFilter.CreateColorFilter(color);
            using var inner = CreateImageFilter(filter.Inner);
            return SKImageFilter.CreateColorFilter(color, inner);
        }
        if (filter.Matrix4 is not null)
            return SKImageFilter.CreateMatrix(ToMatrix(filter.Matrix4), ToSamplingOptions(filter.FilterQuality), null);
        return SKImageFilter.CreateBlur(
            (float)filter.SigmaX,
            (float)filter.SigmaY,
            filter.TileMode switch
            {
                TileMode.repeated => SKShaderTileMode.Repeat,
                TileMode.mirror => SKShaderTileMode.Mirror,
                TileMode.decal => SKShaderTileMode.Decal,
                _ => SKShaderTileMode.Clamp,
            });
    }

    private SKShader ToShader(ShaderSnapshot value) => value switch
    {
        GradientShaderSnapshot gradient => ToGradientShader(gradient),
        ImageShaderSnapshot image => ToImageShader(image),
        FragmentShaderSnapshot fragment => DorotiSkiaRuntimeEffects.CreateShader(
            fragment, CreateImageShader, RuntimeEffectBackend, _contextGeneration, _runtimeEffectContextOwner),
        UnsupportedShaderSnapshot unsupported => throw new NotSupportedException(
            $"The Doroti Skia backend rejects shader family '{unsupported.Family}'."),
        _ => throw new NotSupportedException($"The Doroti Skia backend rejects shader snapshot '{value.GetType().Name}'."),
    };

    private static SKShader ToGradientShader(GradientShaderSnapshot value)
    {
        var colors = value.Colors.Select(ToColor).ToArray();
        var stops = value.Stops.Select(stop => (float)stop).ToArray();
        var tile = ToTileMode(value.TileMode);
        var matrix = value.Matrix4 is null ? SKMatrix.Identity : ToMatrix(value.Matrix4);
        if (value.Begin is { } begin && value.End is { } end)
            return SKShader.CreateLinearGradient(new((float)begin.dx, (float)begin.dy),
                new((float)end.dx, (float)end.dy), colors, stops, tile, matrix);
        if (value.Center is { } center && value.Radius > 0)
            return SKShader.CreateRadialGradient(new((float)center.dx, (float)center.dy),
                (float)value.Radius, colors, stops, tile, matrix);
        if (value.Center is { } sweepCenter)
            return SKShader.CreateSweepGradient(new((float)sweepCenter.dx, (float)sweepCenter.dy),
                colors, stops, tile, (float)(value.StartAngle * 180 / Math.PI),
                (float)(value.EndAngle * 180 / Math.PI), matrix);
        throw new InvalidDataException("Doroti gradient shader has no supported geometry.");
    }

    private static SKShader ToImageShader(ImageShaderSnapshot value)
    {
        if (value.Image.HostHandle is not SkiaImageHandle handle)
            throw new InvalidDataException("Doroti image shader has no native image handle.");
        return handle.Image.ToShader(ToTileMode(value.TileModeX), ToTileMode(value.TileModeY),
            ToSamplingOptions(value.FilterQuality ?? FilterQuality.none), ToMatrix(value.Matrix4));
    }

    private static SKShader CreateImageShader(Doroti.Ui.Image image)
    {
        if (image.HostHandle is not SkiaImageHandle handle)
            throw new InvalidDataException("Doroti fragment shader sampler has no native image handle.");
        return handle.Image.ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp, SKSamplingOptions.Default);
    }

    private static SKColorFilter ToColorFilter(ColorFilterSnapshot value) => value.Kind switch
    {
        ColorFilterKind.mode => SKColorFilter.CreateBlendMode(
            value.Color is null ? throw new InvalidDataException("Mode color filter has no color.") : ToColor(value.Color),
            ToBlend(value.BlendMode)),
        ColorFilterKind.matrix => SKColorFilter.CreateColorMatrix(
            value.Matrix?.Select(item => (float)item).ToArray()
            ?? throw new InvalidDataException("Matrix color filter has no matrix.")),
        ColorFilterKind.linearToSrgbGamma => SKColorFilter.CreateLinearToSrgbGamma(),
        ColorFilterKind.srgbToLinearGamma => SKColorFilter.CreateSrgbToLinearGamma(),
        _ => throw new NotSupportedException($"Unsupported Doroti color filter '{value.Kind}'."),
    };

    private static SKShaderTileMode ToTileMode(TileMode value) => value switch
    {
        TileMode.repeated => SKShaderTileMode.Repeat,
        TileMode.mirror => SKShaderTileMode.Mirror,
        TileMode.decal => SKShaderTileMode.Decal,
        _ => SKShaderTileMode.Clamp,
    };

    private static SKPointMode ToPointMode(PointMode value) => value switch
    {
        PointMode.lines => SKPointMode.Lines,
        PointMode.polygon => SKPointMode.Polygon,
        _ => SKPointMode.Points,
    };

    private static SKPoint ToPoint(Offset value) => new((float)value.dx, (float)value.dy);

    private static SKSamplingOptions ToSamplingOptions(FilterQuality value) => value switch
    {
        FilterQuality.low => new SKSamplingOptions(SKFilterMode.Linear),
        FilterQuality.medium => new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear),
        FilterQuality.high => new SKSamplingOptions(SKCubicResampler.Mitchell),
        _ => new SKSamplingOptions(SKFilterMode.Nearest),
    };

    private void DrawShadow(SKCanvas canvas, CanvasShadowPayload shadow)
    {
        var elevation = Math.Max(0, shadow.Elevation) * _shadowDeviceScale;
        if (elevation <= 0 || shadow.Color.alpha == 0) return;
        using var path = ToPath(shadow.Path);
        path.Transform(canvas.TotalMatrix);
        var ambientRadius = Math.Min(elevation * 0.5, 150);
        var ambientBlur = 0.5 * ambientRadius * (1 + elevation / 128);
        var ambientStroke = 0.5 * (ambientRadius - ambientBlur);
        var ambient = new SKColor(0, 0, 0, (byte)Math.Round(shadow.Color.alpha * 0.039, MidpointRounding.AwayFromZero));
        var spot = TonalSpotColor(ToColor(shadow.Color));
        var saved = canvas.Save();
        try
        {
            canvas.ResetMatrix();
            if (!shadow.TransparentOccluder) canvas.ClipPath(path, SKClipOperation.Difference, true);
            DrawPass(0, ambient, ambientBlur, ambientStroke);
            DrawPass(elevation, spot, elevation * (800.0 / 600), 0);
        }
        finally { canvas.RestoreToCount(saved); }

        void DrawPass(double offsetY, SKColor color, double radius, double stroke)
        {
            if (color.Alpha == 0) return;
            using var mask = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, (float)(radius * 0.57735 + 0.5), false);
            using var paint = new SKPaint { Color = color, MaskFilter = mask, IsAntialias = true,
                Style = stroke > 0 ? SKPaintStyle.StrokeAndFill : SKPaintStyle.Fill, StrokeWidth = (float)Math.Max(0, stroke) };
            var save = canvas.Save();
            canvas.Translate(0, (float)offsetY);
            canvas.DrawPath(path, paint);
            canvas.RestoreToCount(save);
        }
    }

    // Skia's tonal color and blur fallback equations, used because SkiaSharp
    // does not expose SkShadowUtils. CanvasKit uses Skia's tessellated path.
    // https://github.com/google/skia/blob/main/src/utils/SkShadowUtils.cpp
    private static SKColor TonalSpotColor(SKColor color)
    {
        var alpha = Math.Round(color.Alpha * 0.25, MidpointRounding.AwayFromZero) / 255;
        if (alpha == 0) return SKColors.Transparent;
        var luminance = (Math.Max(color.Red, Math.Max(color.Green, color.Blue)) + Math.Min(color.Red, Math.Min(color.Green, color.Blue))) / 510.0;
        var adjusted = (2.6 + (-2.66667 + 1.06667 * alpha) * alpha) * alpha;
        var colorAlpha = Math.Clamp(adjusted * (3.544762 + (-4.891428 + 2.3466 * luminance) * luminance) * luminance, 0, 1);
        var greyAlpha = Math.Clamp(alpha * (1 - 0.4 * luminance), 0, 1);
        var colorScale = colorAlpha * (1 - greyAlpha);
        var tonalAlpha = colorScale + greyAlpha;
        var scale = colorScale / tonalAlpha;
        return new SKColor((byte)(scale * color.Red), (byte)(scale * color.Green), (byte)(scale * color.Blue), (byte)(tonalAlpha * 255.999));
    }

    private static SKPath ToPath(UiPath path)
    {
        using var builder = new SKPathBuilder
        {
            FillType = path.fillType == PathFillType.evenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding,
        };
        foreach (var command in path.Commands)
        {
            var a = command.Arguments;
            switch (command.Operation)
            {
                case "moveTo": builder.MoveTo((float)a[0], (float)a[1]); break;
                case "lineTo": builder.LineTo((float)a[0], (float)a[1]); break;
                case "quadraticBezierTo": builder.QuadTo((float)a[0], (float)a[1], (float)a[2], (float)a[3]); break;
                case "cubicTo": builder.CubicTo((float)a[0], (float)a[1], (float)a[2], (float)a[3], (float)a[4], (float)a[5]); break;
                case "addRect": builder.AddRect(new((float)a[0], (float)a[1], (float)a[2], (float)a[3]), SKPathDirection.Clockwise); break;
                case "addOval": builder.AddOval(new((float)a[0], (float)a[1], (float)a[2], (float)a[3]), SKPathDirection.Clockwise); break;
                case "addArc": builder.AddArc(
                    new((float)a[0], (float)a[1], (float)a[2], (float)a[3]),
                    (float)(a[4] * 180 / Math.PI),
                    (float)(a[5] * 180 / Math.PI)); break;
                case "arcTo": builder.ArcTo(
                    new((float)a[0], (float)a[1], (float)a[2], (float)a[3]),
                    (float)(a[4] * 180 / Math.PI),
                    (float)(a[5] * 180 / Math.PI), a[6] != 0); break;
                case "addRSuperellipse":
                    using (var superellipse = SkiaRSuperellipsePath.Create(new RSuperellipse(
                        Rect.fromLTRB(a[0], a[1], a[2], a[3]),
                        Radius.elliptical(a[4], a[5]),
                        Radius.elliptical(a[6], a[7]),
                        Radius.elliptical(a[8], a[9]),
                        Radius.elliptical(a[10], a[11])))) builder.AddPath(superellipse);
                    break;
                case "addRRect":
                    using (var rounded = new SKRoundRect())
                    {
                        var radii = a.Count >= 12
                            ? new[] { new SKPoint((float)a[4], (float)a[5]), new SKPoint((float)a[6], (float)a[7]), new SKPoint((float)a[8], (float)a[9]), new SKPoint((float)a[10], (float)a[11]) }
                            : Enumerable.Repeat(new SKPoint((float)a[4], (float)a[5]), 4).ToArray();
                        rounded.SetRectRadii(new((float)a[0], (float)a[1], (float)a[2], (float)a[3]), radii);
                        builder.AddRoundRect(rounded, SKPathDirection.Clockwise);
                    }
                    break;
                case "close": builder.Close(); break;
            }
        }
        return builder.Detach();
    }

    private static SKPath ToPath(RRect value)
    {
        using var builder = new SKPathBuilder();
        using var roundRect = new SKRoundRect();
        roundRect.SetRectRadii(ToRect(value.outerRect),
        [
            new((float)value.tlRadius.x, (float)value.tlRadius.y),
            new((float)value.trRadius.x, (float)value.trRadius.y),
            new((float)value.brRadius.x, (float)value.brRadius.y),
            new((float)value.blRadius.x, (float)value.blRadius.y),
        ]);
        builder.AddRoundRect(roundRect, SKPathDirection.Clockwise);
        return builder.Detach();
    }

    private void DrawRRect(SKCanvas canvas, CanvasRRectPayload draw)
    {
        using var paint = ToPaint(draw.Paint);
        var rrect = draw.RRect;
        if (rrect.tlRadius == Radius.zero && rrect.trRadius == Radius.zero &&
            rrect.brRadius == Radius.zero && rrect.blRadius == Radius.zero)
        {
            canvas.DrawRect(ToRect(rrect.outerRect), paint);
            return;
        }
        using var path = ToPath(rrect);
        canvas.DrawPath(path, paint);
    }

    private void DrawDRRect(SKCanvas canvas, CanvasDRRectPayload draw)
    {
        using var paint = ToPaint(draw.Paint);
        using var builder = new SKPathBuilder { FillType = SKPathFillType.EvenOdd };
        builder.AddRoundRect(ToRect(draw.Outer.outerRect), (float)draw.Outer.tlRadiusX, (float)draw.Outer.tlRadiusY,
            SKPathDirection.Clockwise);
        builder.AddRoundRect(ToRect(draw.Inner.outerRect), (float)draw.Inner.tlRadiusX, (float)draw.Inner.tlRadiusY,
            SKPathDirection.Clockwise);
        using var path = builder.Detach();
        canvas.DrawPath(path, paint);
    }

    private static SKRect ToRect(Rect value) => new((float)value.left, (float)value.top, (float)value.right, (float)value.bottom);
    private static bool IsFinite(SKRect value) =>
        float.IsFinite(value.Left) && float.IsFinite(value.Top) &&
        float.IsFinite(value.Right) && float.IsFinite(value.Bottom);
    private static SKColor ToColor(UiColor value) => new((byte)value.red, (byte)value.green, (byte)value.blue, (byte)value.alpha);
    private static SKBlendMode ToBlend(BlendMode value) => Enum.TryParse<SKBlendMode>(value.ToString(), true, out var result) ? result : SKBlendMode.SrcOver;

    private static void Concat(SKCanvas canvas, IReadOnlyList<double> matrix)
    {
        canvas.Concat(ToMatrix(matrix));
    }

    private static SKMatrix ToMatrix(IReadOnlyList<double> matrix)
    {
        if (matrix.Count < 16) throw new InvalidDataException("A Doroti transform must contain 16 values.");
        return new SKMatrix
        {
            ScaleX = (float)matrix[0],
            SkewX = (float)matrix[4],
            TransX = (float)matrix[12],
            SkewY = (float)matrix[1],
            ScaleY = (float)matrix[5],
            TransY = (float)matrix[13],
            Persp0 = (float)matrix[3],
            Persp1 = (float)matrix[7],
            Persp2 = (float)matrix[15],
        };
    }

    private sealed class SkiaImageHandle : IDorotiImageHandle
    {
        private readonly SharedImage _shared;
        internal SkiaImageHandle(SKImage image) => _shared = new(image);
        private SkiaImageHandle(SharedImage shared) { _shared = shared; Interlocked.Increment(ref shared.References); }
        internal SKImage Image => _shared.Image;
        public IDorotiImageHandle Clone() => new SkiaImageHandle(_shared);
        public ValueTask<Doroti.Runtime.ByteData> ReadBytesAsync(ImageByteFormat format)
        {
            if (format == ImageByteFormat.png)
            {
                using var encoded = Image.Encode(SKEncodedImageFormat.Png, 100)
                    ?? throw new InvalidOperationException("Skia PNG encoding failed.");
                return ValueTask.FromResult(new Doroti.Runtime.ByteData(new Doroti.Runtime.Uint8List(encoded.ToArray())));
            }
            var alpha = format == ImageByteFormat.rawStraightRgba ? SKAlphaType.Unpremul : SKAlphaType.Premul;
            // rawUnmodified is canonicalized to this host's tightly packed RGBA8/premultiplied storage.
            using var colorSpace = SKColorSpace.CreateSrgb();
            using var bitmap = new SKBitmap(new SKImageInfo(Image.Width, Image.Height, SKColorType.Rgba8888, alpha, colorSpace));
            if (!Image.ReadPixels(bitmap.Info, bitmap.GetPixels(), bitmap.RowBytes, 0, 0))
                throw new InvalidOperationException("Skia image pixel readback failed.");
            var bytes = new byte[checked(Image.Width * Image.Height * 4)];
            for (var row = 0; row < Image.Height; row++)
                System.Runtime.InteropServices.Marshal.Copy(bitmap.GetPixels() + row * bitmap.RowBytes,
                    bytes, row * Image.Width * 4, Image.Width * 4);
            return ValueTask.FromResult(new Doroti.Runtime.ByteData(new Doroti.Runtime.Uint8List(bytes)));
        }
        public void Release() { if (Interlocked.Decrement(ref _shared.References) == 0) _shared.Image.Dispose(); }
        private sealed class SharedImage(SKImage image) { internal readonly SKImage Image = image; internal int References = 1; }
    }
}
