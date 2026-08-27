using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;
using BlendMode = Doroti.Ui.BlendMode;
using Rect = Doroti.Ui.Rect;
using UiColor = Doroti.Ui.Color;
using UiImage = Doroti.Ui.Image;
using UiPath = Doroti.Ui.Path;

namespace Doroti.Skia.Rendering;

public sealed class SkiaSceneRenderer :
    ISceneHostCapability,
    IParagraphHostCapability,
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
    private readonly ulong _viewId;
    private readonly ISkiaSceneRendererHost _host;
    private readonly UiColor? _lightBackgroundColor;
    private readonly UiColor? _darkBackgroundColor;
    private readonly string _targetIdentity;
    private readonly string _runtimeEffectBackend;
    private readonly string _diagnosticsBackend;
    private SKColor _backgroundColor;
    private readonly object _gate = new();
    private readonly object _paintGate = new();
    private readonly Dictionary<TextRenderKey, TextRenderResources> _textRenderResources = [];
    private readonly Dictionary<int, SemanticsNodeUpdate> _semantics = [];
    private readonly Dictionary<object, PictureRasterCacheEntry> _pictureRasterCache =
        new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<object, int> _pictureRasterWarmups =
        new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<ImageFilterSnapshot, SKImageFilter> _imageFilterResources = [];
    private readonly DorotiFrameTerminalLedger _terminalLedger = new();
    private readonly Dictionary<long, SceneFrame> _rasterizedFrames = [];
    private SceneFrame? _pendingFrame;
    private SceneFrame? _presentedFrame;
    private Action? _invalidate;
    private long _submitted;
    private long _presented;
    private long _replayed;
    private long _failed;
    private long _superseded;
    private long _dropped;
    private long _nextSceneSequence;
    private long _lastSubmittedInputSequence;
    private long _lastPresentedInputSequence;
    private long _contextGeneration;
    private long _shaderImageFiltersRendered;
    private long _pictureRasterFrame;
    private long _pictureRasterPixels;
    private long _pictureRasterCacheHits;
    private long _pictureRasterCacheMisses;
    private long _pictureRasterCacheEntries;
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
        string diagnosticsBackend)
    {
        _viewId = viewId;
        _host = host;
        _lightBackgroundColor = backgroundColor;
        _darkBackgroundColor = darkBackgroundColor;
        _targetIdentity = targetIdentity;
        _runtimeEffectBackend = runtimeEffectBackend;
        _diagnosticsBackend = diagnosticsBackend;
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
                    _frameTrace.Snapshot());
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
        lock (_gate)
        {
            _invalidate = invalidate;
            _contextGeneration++;
            contextGeneration = _contextGeneration;
            hasFrame = _pendingFrame is not null || _presentedFrame is not null;
        }
        DorotiSkiaRuntimeEffects.InvalidateContext(
            RuntimeEffectBackend, contextGeneration);
        DorotiSkiaImageFilterRenderer.InvalidateContext(
            RuntimeEffectBackend, contextGeneration);
        lock (_paintGate)
        {
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
                RuntimeEffectBackend, _contextGeneration);
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
        lock (_gate)
        {
            _contextGeneration++;
            contextGeneration = _contextGeneration;
        }
        DorotiSkiaRuntimeEffects.InvalidateContext(
            RuntimeEffectBackend, contextGeneration);
        DorotiSkiaImageFilterRenderer.InvalidateContext(
            RuntimeEffectBackend, contextGeneration);
        lock (_paintGate)
        {
            foreach (var filter in _imageFilterResources.Values) filter.Dispose();
            _imageFilterResources.Clear();
            ClearPictureRasterCache();
        }
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
            _submitted++;
            _lastSubmittedInputSequence = inputSequence;
            if (submission.BuildToken is not { } buildToken)
            {
                _terminalLedger.TryComplete(sceneSequence, DorotiFrameTerminal.dropped);
                submission.FrameTransaction?.TryComplete(
                    DorotiFrameTerminal.dropped,
                    "scene submitted outside a framework frame");
                _dropped++;
                _frameTrace.Record(DorotiFramePhase.dropped, _viewId, timestamp,
                    inputSequence, sceneSequence, _host.SurfaceGeneration,
                    $"{DorotiFrameMismatch.missingBuildToken}: scene submitted outside a framework frame");
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
                sceneSequence, inputSequence, timestamp, descriptor, scene.Commands.ToArray(),
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
                inputSequence, sceneSequence, _host.SurfaceGeneration, invocation.ElementId);
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
        lock (_paintGate) return PaintCore(surface, pixelWidth, pixelHeight, desiredTarget, causalFrameId);
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
            DorotiSkiaImageFilterRenderer.BeginFrame(RuntimeEffectBackend, _contextGeneration);
            _pictureRasterFrame++;
            _frameTrace.Record(DorotiFramePhase.raster, _viewId, rasterStart,
                frame.InputSequence, frame.SceneSequence, _host.SurfaceGeneration,
                isNewFrame ? null : "retained scene replay", rasterStart - frame.SubmittedAt);
            DrawScene(canvas, frame.Commands, pixelWidth, pixelHeight);
            var rasterEnd = DorotiFrameClock.Now;
            _frameTrace.Record(DorotiFramePhase.rasterEnd, _viewId, rasterEnd,
                frame.InputSequence, frame.SceneSequence, _host.SurfaceGeneration,
                "Doroti draw complete", rasterEnd - rasterStart);
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
                receipt = CreateFrameReceipt(completion, terminal);
            }
            else
            {
                _replayed++;
                _frameTrace.Record(DorotiFramePhase.replay, _viewId, DorotiFrameClock.Now,
                    completion.InputSequence, completion.SceneSequence, completion.SurfaceGeneration,
                    "fresh native back buffer submitted");
                receipt = CreateFrameReceipt(completion, terminal);
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
            if (!completion.IsNewFrame ||
                !_rasterizedFrames.Remove(completion.SceneSequence, out var frame)) return;
            if (MarkTerminal(frame, DorotiFrameTerminal.failed, reason, completion.SurfaceGeneration))
                receipt = CreateFrameReceipt(completion, DorotiFrameTerminal.failed);
        }
        PublishFrameReceipt(receipt);
    }

    public void SupersedePaint(SkiaPaintCompletion completion, string reason)
    {
        if (_disposed) return;
        SkiaFrameReceipt? receipt = null;
        lock (_gate)
        {
            if (!completion.IsNewFrame ||
                !_rasterizedFrames.Remove(completion.SceneSequence, out var frame)) return;
            if (MarkTerminal(frame, DorotiFrameTerminal.superseded, reason, completion.SurfaceGeneration))
                receipt = CreateFrameReceipt(completion, DorotiFrameTerminal.superseded);
        }
        PublishFrameReceipt(receipt);
    }

    public Paragraph Layout(ParagraphRequest request, DartUiInvocation invocation)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_paintGate)
        {
            var resources = GetTextRenderResources(
                request.FontFamily,
                (float)request.FontSize,
                ToColor(request.Color ?? new UiColor(0xFF000000)));
            var advances = resources.MeasureCodeUnitAdvances(request.Text);
            var naturalWidth = advances.Sum();
            var width = double.IsFinite(request.Width) ? Math.Min(request.Width, naturalWidth) : naturalWidth;
            return new Paragraph(
                request.Text,
                width,
                request.Height ?? request.FontSize * 1.2,
                request.FontSize,
                request.MaxLines,
                request.FontFamily,
                request.Color,
                advances);
        }
    }

    public ValueTask<UiImage> DecodeAsync(
        ReadOnlyMemory<byte> bytes,
        DartUiInvocation invocation,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();
        var data = SKData.CreateCopy(bytes.Span);
        var image = SKImage.FromEncodedData(data);
        data.Dispose();
        if (image is null) throw new InvalidDataException("SkiaSharp could not decode the image resource.");
        var handle = new SkiaImageHandle(image);
        return ValueTask.FromResult(new UiImage(_viewId, image.Width, image.Height, handle.Release) { HostHandle = handle });
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
        var nodes = _semantics.Values
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

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _host.SemanticsAction -= HandleSemanticsAction;
        _host.InputReceived -= HandleInput;
        _host.ConfigurationChanged -= HandleConfigurationChanged;
        DorotiSkiaImageFilterRenderer.ReleaseContext(RuntimeEffectBackend, _contextGeneration);
        lock (_paintGate)
        {
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
        DorotiFrameTerminal terminal) => new(
        completion.CausalFrameId,
        completion.InputSequence,
        completion.SceneSequence,
        completion.SurfaceGeneration,
        completion.Descriptor,
        terminal,
        DorotiFrameClock.Now);

    private void PublishFrameReceipt(SkiaFrameReceipt? receipt)
    {
        if (receipt is not { } value) return;
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
            case DorotiFrameTerminal.submitted:
                _presented++;
                _lastPresentedInputSequence = frame.InputSequence;
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
            $"{terminal}: {reason}");
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
                        canvas.Save(); restoreCounts.Push(1); canvas.ClipPath(ToPath(clip.RRect), SKClipOperation.Intersect, true); break;
                    case "clipRSuperellipse" when command.HostPayload is SceneClipRSuperellipsePayload clip:
                        canvas.Save(); restoreCounts.Push(1); canvas.ClipRect(ToRect(clip.RSuperellipse.outerRect), SKClipOperation.Intersect, true); break;
                    case "clipPath" when command.HostPayload is SceneClipPathPayload clip:
                        canvas.Save(); restoreCounts.Push(1); canvas.ClipPath(ToPath(clip.Path), SKClipOperation.Intersect, true); break;
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
                                out var cacheHit);
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
                case "clipRRect" when command.HostPayload is CanvasClipRRectPayload clip: canvas.ClipPath(ToPath(clip.RRect), SKClipOperation.Intersect, true); break;
                case "clipRSuperellipse" when command.HostPayload is CanvasClipRSuperellipsePayload clip: canvas.ClipRect(ToRect(clip.RSuperellipse.outerRect), SKClipOperation.Intersect, clip.DoAntiAlias); break;
                case "clipPath" when command.HostPayload is CanvasClipPathPayload clip: canvas.ClipPath(ToPath(clip.Path), SKClipOperation.Intersect, true); break;
                case "drawRect" when command.HostPayload is CanvasRectPayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawRect(ToRect(draw.Rect), paint); break;
                case "drawRRect" when command.HostPayload is CanvasRRectPayload draw: DrawRRect(canvas, draw); break;
                case "drawDRRect" when command.HostPayload is CanvasDRRectPayload draw: DrawDRRect(canvas, draw); break;
                case "drawRSuperellipse" when command.HostPayload is CanvasRSuperellipsePayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawRect(ToRect(draw.RSuperellipse.outerRect), paint); break;
                case "drawPath" when command.HostPayload is CanvasPathPayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawPath(ToPath(draw.Path), paint); break;
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
                    var textResources = GetTextRenderResources(
                        draw.Paragraph.fontFamily,
                        (float)draw.Paragraph.fontSize,
                        ToColor(draw.Paragraph.color));
                    textResources.DrawText(
                        canvas,
                        draw.Paragraph.text,
                        (float)draw.Offset.dx,
                        (float)(draw.Offset.dy + draw.Paragraph.alphabeticBaseline));
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
        var cacheKey = (object)commands;
        if (payload.WillChangeHint || payload.CanvasBounds is not { } canvasBounds ||
            !canvasBounds.IsFinite || canvasBounds.isEmpty ||
            (!payload.IsComplexHint && commands.Count < PictureRasterComplexityThreshold) ||
            canvas.Context is not { } context)
        {
            DrawPicture(canvas, commands);
            return;
        }

        var mappedBounds = canvas.TotalMatrix.MapRect(ToRect(canvasBounds));
        if (!IsFinite(mappedBounds) || mappedBounds.Width <= 0 || mappedBounds.Height <= 0)
        {
            DrawPicture(canvas, commands);
            return;
        }

        if (mappedBounds.Width > MaxCacheablePicturePixels ||
            mappedBounds.Height > MaxCacheablePicturePixels)
        {
            DrawPicture(canvas, commands);
            return;
        }

        var width = checked((int)Math.Ceiling(mappedBounds.Width));
        var height = checked((int)Math.Ceiling(mappedBounds.Height));
        var pixels = (long)width * height;
        if (pixels <= 0 || pixels > MaxCacheablePicturePixels)
        {
            DrawPicture(canvas, commands);
            return;
        }

        var signature = PictureRasterTransform.From(canvas.TotalMatrix);
        if (_pictureRasterCache.TryGetValue(cacheKey, out var cached))
        {
            if (cached.Width == width && cached.Height == height && cached.Transform == signature)
            {
                cached.LastUsedFrame = _pictureRasterFrame;
                DrawRasterImage(canvas, cached.Image, mappedBounds.Left, mappedBounds.Top);
                Interlocked.Increment(ref _pictureRasterCacheHits);
                return;
            }
            RemovePictureRaster(cacheKey, cached);
        }

        var warmups = _pictureRasterWarmups.GetValueOrDefault(cacheKey) + 1;
        _pictureRasterWarmups[cacheKey] = warmups;
        if (warmups < PictureRasterWarmupFrames)
        {
            DrawPicture(canvas, commands);
            return;
        }

        var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(context, true, info)
            ?? throw new InvalidOperationException(
                $"Doroti picture raster cache could not allocate a {width}x{height} GPU surface.");
        var rasterCanvas = surface.Canvas;
        rasterCanvas.Clear(SKColors.Transparent);
        rasterCanvas.Save();
        rasterCanvas.Translate(-mappedBounds.Left, -mappedBounds.Top);
        var matrix = canvas.TotalMatrix;
        rasterCanvas.Concat(in matrix);
        DrawPicture(rasterCanvas, commands);
        rasterCanvas.Restore();
        rasterCanvas.Flush();
        var image = surface.Snapshot()
            ?? throw new InvalidOperationException("Doroti picture raster cache could not snapshot its GPU surface.");
        cached = new(image, width, height, signature, _pictureRasterFrame);
        _pictureRasterCache.Add(cacheKey, cached);
        Interlocked.Increment(ref _pictureRasterCacheEntries);
        _pictureRasterPixels += cached.Pixels;
        _pictureRasterWarmups.Remove(cacheKey);
        TrimPictureRasterCache();
        DrawRasterImage(canvas, image, mappedBounds.Left, mappedBounds.Top);
        Interlocked.Increment(ref _pictureRasterCacheMisses);
    }

    private static void DrawRasterImage(SKCanvas canvas, SKImage image, float left, float top)
    {
        canvas.Save();
        canvas.ResetMatrix();
        canvas.DrawImage(image, left, top, SKSamplingOptions.Default);
        canvas.Restore();
    }

    private void TrimPictureRasterCache()
    {
        while (_pictureRasterCache.Count > MaxPictureRasterCacheEntries ||
               _pictureRasterPixels > MaxPictureRasterPixels)
        {
            var oldest = _pictureRasterCache.MinBy(pair => pair.Value.LastUsedFrame);
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
        foreach (var cached in _pictureRasterCache.Values) cached.Image.Dispose();
        _pictureRasterCache.Clear();
        _pictureRasterWarmups.Clear();
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
        float Persp2)
    {
        internal static PictureRasterTransform From(SKMatrix matrix) => new(
            matrix.ScaleX, matrix.SkewX, matrix.SkewY, matrix.ScaleY,
            matrix.Persp0, matrix.Persp1, matrix.Persp2);
    }

    private sealed class PictureRasterCacheEntry(
        SKImage image,
        int width,
        int height,
        PictureRasterTransform transform,
        long lastUsedFrame)
    {
        internal SKImage Image { get; } = image;
        internal int Width { get; } = width;
        internal int Height { get; } = height;
        internal PictureRasterTransform Transform { get; } = transform;
        internal long LastUsedFrame { get; set; } = lastUsedFrame;
        internal long Pixels => (long)Width * Height;
    }

    private TextRenderResources GetTextRenderResources(string? fontFamily, float fontSize, SKColor color)
    {
        var key = new TextRenderKey(fontFamily ?? string.Empty, fontSize, color);
        if (_textRenderResources.TryGetValue(key, out var resources)) return resources;
        resources = new TextRenderResources(fontFamily, fontSize, color);
        _textRenderResources.Add(key, resources);
        return resources;
    }

    private readonly record struct TextRenderKey(string FontFamily, float FontSize, SKColor Color);

    private sealed class TextRenderResources : IDisposable
    {
        private readonly TextFontResource _primary;
        private readonly Dictionary<int, TextFontResource> _fallbackByCodePoint = [];
        private readonly Dictionary<string, TextFontResource> _fallbackByFamily =
            new(StringComparer.OrdinalIgnoreCase);

        internal TextRenderResources(string? fontFamily, float fontSize, SKColor color)
        {
            _primary = new TextFontResource(SKTypeface.FromFamilyName(fontFamily), fontSize);
            Paint = new SKPaint { Color = color, IsAntialias = true };
        }

        internal SKPaint Paint { get; }

        internal void DrawText(SKCanvas canvas, string text, float x, float baseline)
        {
            if (text.Length == 0) return;

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

        private sealed class TextFontResource(SKTypeface typeface, float fontSize) : IDisposable
        {
            internal SKTypeface Typeface { get; } = typeface;
            internal SKFont Font { get; } = new(typeface, fontSize);
            internal string FamilyName => Typeface.FamilyName;

            public void Dispose()
            {
                Font.Dispose();
                Typeface.Dispose();
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
            fragment, CreateImageShader, RuntimeEffectBackend, _contextGeneration),
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

    private static void DrawShadow(SKCanvas canvas, CanvasShadowPayload shadow)
    {
        using var path = ToPath(shadow.Path);
        var elevation = Math.Max(0, shadow.Elevation);
        DrawPass(elevation * .2, .18, .24, Math.Max(.75, elevation * .45));
        DrawPass(elevation * .55, .24, .32, Math.Max(1, elevation * .8));

        void DrawPass(double offsetY, double transparentOpacity, double opaqueOpacity, double sigma)
        {
            var opacity = shadow.TransparentOccluder ? transparentOpacity : opaqueOpacity;
            using var paint = new SKPaint
            {
                Color = ToColor(shadow.Color).WithAlpha((byte)Math.Clamp(Math.Round(shadow.Color.alpha * opacity), 0, 255)),
                ImageFilter = SKImageFilter.CreateBlur((float)sigma, (float)sigma),
                IsAntialias = true,
            };
            canvas.Save();
            canvas.Translate(0, (float)offsetY);
            canvas.DrawPath(path, paint);
            canvas.Restore();
        }
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
                case "addRRect": builder.AddRoundRect(new((float)a[0], (float)a[1], (float)a[2], (float)a[3]), (float)a[4], (float)a[5], SKPathDirection.Clockwise); break;
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
        public void Release() { if (Interlocked.Decrement(ref _shared.References) == 0) _shared.Image.Dispose(); }
        private sealed class SharedImage(SKImage image) { internal readonly SKImage Image = image; internal int References = 1; }
    }
}
