using System.ComponentModel;
using System.Runtime.InteropServices;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Live WinUI islands interleaved with DirectComposition raster windows.
/// Raster slices use the active Graphite recorder and immutable shared GPU textures.
/// WebView2 has its own visual composition path; physical atomic display is not advertised.</summary>
internal sealed class WindowsPlatformViewHost : IDisposable
{
    private readonly Dictionary<PlatformViewHandle, nint> _controls = [];
    private readonly Dictionary<long, Frame> _ready = [];
    private readonly object _gate = new();
    private WindowsCompositionSlice[] _rasterCache = [];
    private SkiaPlatformRasterContent.CacheScope _rasterCacheScope;
    private string _rasterCacheTopology = "";
    private long _rasterCacheRevision;
    private WindowsWebViewComposition? _webViews;
    private readonly WindowsWinUiControls _winUiControls = new();
    private WindowsWinUiBackdrop? _winUiBackdrop;
    private Task<int>? _captureProbe;
    private readonly List<CompositionRaster>[] _banks = [[], []];
    private CompositionRaster? _sceneOutput;
    private readonly Dictionary<string, IPlatformViewFactory> _factories = [];
    private WindowsPlatformViewDispatcher? _dispatcher;
    private WindowsManagedProductHost? _host;
    private WindowsManagedVulkanPresenter? _presenter;
    private PlatformViewCoordinator? _coordinator;
    private PlatformCompositionSession? _session;
    private long _compositionFrame;
    private Frame? _recording;
    private PlatformCompositionPart[] _visible = [];
    private int _visibleBank;
    private int _visibleRasterCount;
    private double _visibleScaleX, _visibleScaleY;
    private nint _parent;
    private bool _closed;
    private bool _hasVisibleParts;
    private bool _winUiSceneVisible;
    private int _winUiWidth, _winUiHeight;
    private long _winUiPlacementBatches;
    private bool _needsReplay;
    private long _nativeRevision, _committedNativeRevision;
    internal bool NeedsReplay => Volatile.Read(ref _needsReplay) ||
        Interlocked.Read(ref _nativeRevision) != Interlocked.Read(ref _committedNativeRevision);
    internal Action? RequestFrameworkFrame { get; set; }
    private long _commits;
    private const long _readbackBytes = 0;
    private const long _uploadedBytes = 0;
    private long _reusedRasters;
    private int _liveHwndSources;
    private readonly HashSet<nint> _nativeSourcesToPaint = [];
    private int _nativePointerDowns;
    private int _scriptPointerDowns;
    private readonly Queue<double> _rasterMilliseconds = new();
    private readonly Queue<double> _uiMilliseconds = new();
    private List<WindowPosition>? _positions;
    private sealed record WindowPosition(nint Hwnd, int X, int Y, int Width, int Height, uint Flags);

    internal IEnumerable<IPlatformViewFactory> CreateFactories() =>
        [new DeferredFactory(this, "doroti/native-button"), new DeferredFactory(this, "doroti/native-editor"), new DeferredFactory(this, "doroti/webview")];

    private sealed class DeferredFactory(WindowsPlatformViewHost owner, string type) : IPlatformViewFactory
    {
        public string ViewType => type;
        private IPlatformViewFactory Resolve() => owner._factories.GetValueOrDefault(type)
            ?? throw new InvalidOperationException("Windows PlatformView parent/presenter is not bound yet.");
        public PlatformViewSupport QuerySupport(PlatformViewRequest request) => Resolve().QuerySupport(request);
        public ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
            Action<PlatformViewHandle> focused, CancellationToken cancellationToken) =>
            Resolve().CreateAsync(handle, parameters,
                value => owner._host!.DispatchPlatformViewEvent(() => focused(value)), cancellationToken);
    }

    internal WindowsPlatformViewDispatcher Bind(WindowsManagedProductHost host, WindowsManagedVulkanPresenter? presenter, IApplicationResourceHostCapability resources)
    {
        if (_parent != 0) throw new InvalidOperationException("PlatformView owner already bound.");
        _parent = host.TopLevelHwnd;
        _host = host;
        _winUiBackdrop = new(_winUiControls, _parent, () => _presenter!.RasterAdapter);
        _presenter = presenter;
        _dispatcher = new();
        _webViews = new(_parent, () =>
        {
            if (_closed) return;
            // Native navigation/messages can finish after the framework becomes idle.
            // Force replay so the next receipt observes the completed native revision.
            Interlocked.Increment(ref _nativeRevision);
            // A native event can follow input which did not dirty a widget.
            // Refresh the framework descriptor/input sequence before replay;
            // otherwise the stale-input presentation guard rejects it forever.
            if (RequestFrameworkFrame is { } request) request();
            else host.RequestInvalidate();
        }, host.ClearClient, presenter is not null, resources, () => _presenter!.RasterAdapter);
        _factories.Add(_webViews.ViewType, _webViews);
        if (presenter is not null)
        {
            Native.SetWindowLongPtrW(_parent, -16, Native.GetWindowLongPtrW(_parent, -16) | 0x02000000);
            // The backdrop may already own the top-level lower DComp target.
            // Put the Vulkan target on the existing background child; all native
            // and raster-slice HWNDs are its siblings, so neither target covers them.
            presenter.PlatformRasterWindow = host.ChildHwnd;
            Check(Native.SetWindowPos(host.ChildHwnd, 1, 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0010 | 0x0040));
        }
        foreach (var editor in new[] { false, true })
        {
            var factory = new WindowsHwndPlatformViewFactory(_parent, editor, false)
            {
                WinUiControls = _winUiControls,
                Interleaved = presenter is not null,
                SiblingRasterTopology = presenter is not null,
                KeepCompositionSourceAlive = false,
                Created = (handle, hwnd) =>
                {
                    // WinUI owns a live composition island, not a layered GDI bitmap.
                    _controls.Add(handle, hwnd);
                    Volatile.Write(ref _liveHwndSources, _controls.Count);
                },
                Destroyed = handle =>
                {
                    if (_controls.Remove(handle, out var hwnd)) _nativeSourcesToPaint.Remove(hwnd);
                    Volatile.Write(ref _liveHwndSources, _controls.Count);
                },
                InterceptsPoint = Intercepts,
                YieldFrameworkTextInput = host.ClearClient,
                PointerDown = (handle, screenPoint, extra) =>
                {
                    _nativePointerDowns++;
                    if (extra == 0x444f5250) _scriptPointerDowns++;
                    var path = Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_EVIDENCE");
                    if (!string.IsNullOrWhiteSpace(path))
                        File.WriteAllText(path + ".input.json", System.Text.Json.JsonSerializer.Serialize(new {
                            nativePointerDowns = _nativePointerDowns, scriptPointerDowns = _scriptPointerDowns,
                            screenX = (short)(screenPoint.ToInt64() & 0xffff), screenY = (short)((screenPoint.ToInt64() >> 16) & 0xffff),
                            extra = extra.ToInt64(), handle }));
                },
                StagePlacement = (hwnd, x, y, width, height, visible) => Position(new(hwnd, x, y, width, height, 0x0014u | (visible ? 0x0040u : 0x0080u))),
                SourceSurfaceChanged = hwnd => _nativeSourcesToPaint.Add(hwnd),
            };
            _factories.Add(factory.ViewType, factory);
        }
        return _dispatcher;
    }
    internal void Configure(PlatformViewCoordinator coordinator) => _coordinator = coordinator;

    private static bool HasPlatformCommands(IReadOnlyList<SceneCommand> commands, int depth = 0)
    {
        if (depth > 256) throw new InvalidDataException("PlatformView retained depth limit exceeded.");
        return commands.Any(c => c.Operation is "platformView" or "inputShield" ||
            c.HostPayload is SceneRetainedPayload retained && HasPlatformCommands(retained.Commands, depth + 1));
    }

    internal void Draw(SkiaSceneRenderer renderer, SKCanvas canvas, IReadOnlyList<SceneCommand> commands,
        DorotiFrameDescriptor descriptor, int width, int height)
    {
        if (_recording is not null) throw new InvalidOperationException("Unretired PlatformView raster frame.");
        var started = System.Diagnostics.Stopwatch.GetTimestamp();
        var nativeRevision = Interlocked.Read(ref _nativeRevision);
        if (!HasPlatformCommands(commands) && !Volatile.Read(ref _hasVisibleParts))
        { renderer.DrawPlatformRasterSegment(canvas, commands, width, height); return; }
        var token = new PlatformCompositionToken(descriptor.ViewId, descriptor.MetricsGeneration,
            Interlocked.Increment(ref _compositionFrame), descriptor.ResizeTargetGeneration, descriptor.DeviceScaleX, descriptor.DeviceScaleY);
        var composition = Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_COMPOSITION") == "overlay"
            ? PlatformViewComposition.NativeOverlay : PlatformViewComposition.InterleavedComposition;
        var plan = PlatformCompositionPlanner.Build(commands, token, _coordinator!, composition,
            effects: WindowsWebViewComposition.Effects);
        var nativeParts = plan.Parts.OfType<PlatformNativeSegment>().ToArray();
        if (nativeParts.Any(p => _webViews!.Contains(p.Placement.Handle)) &&
            nativeParts.Any(p => !_webViews!.Contains(p.Placement.Handle)))
        { plan.Dispose(); throw new NotSupportedException("Windows cannot mix WebView composition visuals and sibling HWNDs in one frame."); }
        if (nativeParts.Any(p => !_webViews!.Contains(p.Placement.Handle)) &&
            plan.Parts.OfType<PlatformBackdropSegment>().Any(p => p.Style is { Saturation: not 1 }))
        { plan.Dispose(); throw new NotSupportedException("Saturation adjustment requires the WebView2 Windows Composition tree; the WinUI island effect does not support it."); }
        if (plan.Parts.OfType<PlatformBackdropSegment>().Any(p =>
                Math.Abs(p.SigmaX - p.SigmaY) > .001 || Math.Abs(token.DeviceScaleX - token.DeviceScaleY) > .001))
        { plan.Dispose(); throw new NotSupportedException("Windows Composition requires isotropic backdrop sigma and device scale."); }
        if (plan.Parts.OfType<PlatformBackdropSegment>().Any(p => p.SigmaX * token.DeviceScaleX > 128))
        { plan.Dispose(); throw new NotSupportedException("Windows Composition backdrop physical sigma must not exceed 128."); }
        WindowsSharedRaster? shared = null;
        try
        {
            var rasters = plan.Parts.OfType<PlatformRasterSegment>().ToArray();
            if (plan.Parts.OfType<PlatformShieldSegment>().Any(p => !p.Shield.Transform.IsAxisAligned))
                throw new NotSupportedException("Windows native input shields require axis-aligned rectangles.");
            if (!plan.HasNativeContent && !(Volatile.Read(ref _hasVisibleParts) && Volatile.Read(ref _liveHwndSources) != 0))
            {
                _presenter!.PreservePrimaryRaster = false;
                foreach (var raster in rasters) renderer.DrawPlatformRasterSegment(canvas, raster.Commands, width, height);
                _recording = new(plan, width, height, []) { Started = started, NativeRevision = nativeRevision };
                return;
            }
            var visualComposition = nativeParts.Length != 0 && nativeParts.All(p => _webViews!.Contains(p.Placement.Handle));
            var cacheScope = SkiaPlatformRasterContent.CacheScope.From(token, width, height, renderer.PlatformBackgroundColor);
            var topology = (visualComposition ? "visual:" : "hwnd:") + string.Join(",", plan.Parts.Select(p => $"{p.GetType().Name}:{p.PaintOrder}"));
            WindowsCompositionSlice[] previous;
            long cacheRevision;
            lock (_gate)
            {
                previous = _rasterCacheScope == cacheScope && _rasterCacheTopology == topology ? _rasterCache : [];
                cacheRevision = _rasterCacheRevision;
            }
            // Keep the previous primary image until the UI owner admits the first
            // complete HWND scene. Never publish the cleared transport backing.
            _presenter!.PreservePrimaryRaster = !visualComposition;
            var slices = new List<WindowsCompositionSlice>();
            var atlasHeight = 0;
            foreach (var raster in rasters)
            {
                if (visualComposition && raster.PaintOrder != 0 && !SkiaPlatformRasterContent.HasDrawing(raster.Commands)) continue;
                var bounds = new SKRectI(0, 0, width, height);
                if (raster.PaintOrder != 0)
                {
                    var coverage = SkiaPlatformRasterContent.Split(raster.Commands, width, height);
                    if (coverage.Count == 0 && visualComposition) continue;
                    bounds = coverage.Count == 0 ? new(0, 0, 1, 1) : new(coverage.Min(s => s.Bounds.Left), coverage.Min(s => s.Bounds.Top),
                        coverage.Max(s => s.Bounds.Right), coverage.Max(s => s.Bounds.Bottom));
                }
                var prior = previous.FirstOrDefault(p => p.Segment.PaintOrder == raster.PaintOrder);
                var reused = prior is not null && SkiaPlatformRasterContent.CanReuse(cacheScope,
                    new(prior.Segment.Commands, prior.Bounds), cacheScope, new(raster.Commands, bounds), allowTranslation: visualComposition);
                slices.Add(new(raster, bounds, reused ? -1 : atlasHeight, reused));
                if (!reused) atlasHeight = checked(atlasHeight + bounds.Height);
            }
            var changes = slices.Where(s => !s.Reused).ToArray();
            var expectedRevision = slices.Any(s => s.Reused) ? cacheRevision : -1;
            if (changes.Length == 0)
            {
                _recording = new(plan, width, height, rasters) { Started = started, NativeRevision = nativeRevision,
                    Slices = slices.ToArray(), CacheScope = cacheScope, RasterTopology = topology, ExpectedCacheRevision = expectedRevision };
                return;
            }
            var atlasWidth = changes.Max(s => s.Bounds.Width);
            // Account for private/shared GPU atlas and current/prepared compositor surfaces.
            var reservedBytes = (long)atlasWidth * atlasHeight * 8 + slices.Sum(s => (long)s.Bounds.Width * s.Bounds.Height * 8);
            if (atlasWidth > 16384 || atlasHeight > 16384 || reservedBytes > 256L * 1024 * 1024)
                throw new InvalidOperationException("Windows PlatformView active/staging resources exceed 256 MiB / 16384 pixels.");
            shared = _presenter!.CreateSharedRaster(atlasWidth, atlasHeight);
            foreach (var slice in changes)
            {
                shared.Canvas.Save();
                shared.Canvas.ClipRect(SKRect.Create(0, slice.AtlasY, slice.Bounds.Width, slice.Bounds.Height), SKClipOperation.Intersect, false);
                shared.Canvas.Translate(-slice.Bounds.Left, slice.AtlasY - slice.Bounds.Top);
                if (slice.Segment.PaintOrder == 0) shared.Canvas.DrawColor(renderer.PlatformBackgroundColor);
                renderer.DrawPlatformRasterSegment(shared.Canvas, slice.Segment.Commands, width, height);
                shared.Canvas.Restore();
            }
            _recording = new(plan, width, height, rasters) { Started = started, NativeRevision = nativeRevision,
                Slices = slices.ToArray(), CacheScope = cacheScope, RasterTopology = topology, ExpectedCacheRevision = expectedRevision,
                Shared = shared };
            shared = null;
        }
        catch { shared?.Dispose(); plan.Dispose(); throw; }
    }

    // Called by the raster owner after the presenter has completed its same-queue
    // copy fence, before the native terminal is sent.
    internal void FinishRaster(long causalFrameId, bool accepted)
    {
        var frame = _recording;
        _recording = null;
        if (frame is null) return;
        try
        {
            if (accepted)
            {
                lock (_gate)
                {
                    AddTiming(_rasterMilliseconds, System.Diagnostics.Stopwatch.GetElapsedTime(frame.Started).TotalMilliseconds);
                    _ready.Add(causalFrameId, frame);
                }
                frame = null;
            }
        }
        finally { frame?.Dispose(); }
    }

    internal bool Terminal(long causalFrameId, bool presented, long generation)
    {
        _dispatcher!.VerifyThread();
        Frame? frame;
        lock (_gate) _ready.Remove(causalFrameId, out frame);
        if (frame is null) return true;
        using (frame)
        {
            var started = System.Diagnostics.Stopwatch.GetTimestamp();
            if (!presented || _closed || frame.Plan.Token.SurfaceGeneration != generation ||
                !_host!.IsLatestResizeGeneration(checked((ulong)generation))) return false;
            _session ??= new(frame.Plan.Token.OwnerViewId);
            lock (_gate)
                if (frame.ExpectedCacheRevision >= 0 && frame.ExpectedCacheRevision != _rasterCacheRevision)
                { Volatile.Write(ref _needsReplay, true); _host.RequestInvalidate(); return false; }
            try
            {
                if (!_session.CommitRetiredFrame(frame.Plan, () => CommitFrame(frame, started))) return false;
            }
            catch
            {
                // A partially failed native commit cannot authorize a later reuse.
                lock (_gate) { _rasterCache = []; _rasterCacheRevision++; }
                throw;
            }
            lock (_gate)
            {
                _rasterCache = frame.Slices; _rasterCacheScope = frame.CacheScope;
                _rasterCacheTopology = frame.RasterTopology; _rasterCacheRevision++;
            }
            Interlocked.Exchange(ref _committedNativeRevision, frame.NativeRevision);
            if (NeedsReplay) _host.RequestInvalidate();
            try { WriteEvidence(frame); }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException)
            { System.Diagnostics.Trace.TraceWarning($"PlatformView evidence could not be written: {error.Message}"); }
            return true;
        }
    }

    private bool CommitFrame(Frame frame, long started)
    {
            var next = frame.Plan.Parts.ToArray();
            try
            {
                if (next.OfType<PlatformNativeSegment>().Any(p => _coordinator!.GetState(p.Placement.Handle) is
                    not (PlatformViewState.Ready or PlatformViewState.Attached or PlatformViewState.Hidden or PlatformViewState.Detached))) return false;
            }
            catch (DorotiCapabilityException) { return false; }
            var handles = next.OfType<PlatformNativeSegment>().Select(p => p.Placement.Handle)
                .Concat(_visible.OfType<PlatformNativeSegment>().Select(p => p.Placement.Handle))
                .Where(handle => _controls.ContainsKey(handle) || _webViews!.Contains(handle)).Distinct().ToArray();
            IPlatformViewPlacementBatch? reservation;
            try
            {
                if (!_coordinator!.TryBeginPlacementBatch(handles, out reservation))
                {
                    Volatile.Write(ref _needsReplay, true);
                    _host!.RequestInvalidate();
                    return false;
                }
            }
            catch (DorotiCapabilityException)
            {
                Volatile.Write(ref _needsReplay, true);
                _host!.RequestInvalidate();
                return false;
            }
            using var batch = reservation!;
            if (!frame.Plan.HasNativeContent && frame.Slices.Length == 0)
            {
                // The whole scene is already on the primary GPU output. A
                // leftover backdrop-only HWND has no raster source and can
                // obscure that output/input after the last WebView is removed.
                _webViews!.Clear(batch);
                _winUiBackdrop?.Clear();
                _presenter!.WaitForPrimaryCopyCompletion();
                foreach (var oldBank in _banks) foreach (var layer in oldBank) layer.Hide();
                _positions = [];
                Position(new(_host!.ChildHwnd, 0, 0, 0, 0, 0x0057));
                if (_sceneOutput is not null) Position(new(_sceneOutput.Hwnd, 0, 0, 0, 0, 0x0097));
                CommitPositions();
                _visible = next; _visibleRasterCount = 0; _winUiSceneVisible = false;
                Volatile.Write(ref _hasVisibleParts, false);
                Volatile.Write(ref _needsReplay, false);
                _commits++;
                AddTiming(_uiMilliseconds, System.Diagnostics.Stopwatch.GetElapsedTime(started).TotalMilliseconds);
                return true;
            }
            if (next.OfType<PlatformNativeSegment>().Any(p => _webViews!.Contains(p.Placement.Handle)))
            {
                _winUiBackdrop?.Clear();
                foreach (var old in _visible.OfType<PlatformNativeSegment>())
                    if (_controls.ContainsKey(old.Placement.Handle) && batch.Contains(old.Placement.Handle))
                        RunNow(_ => batch.DetachAsync(old.Placement.Handle));
                _webViews!.Commit(frame.Plan, frame.Shared, frame.Slices, batch);
                _sceneOutput?.Hide();
                foreach (var layer in _banks[_visibleBank]) layer.Hide();
                _winUiSceneVisible = false;
                _visible = next;
                Volatile.Write(ref _hasVisibleParts, next.Length != 0);
                Volatile.Write(ref _needsReplay, false);
                _commits++;
                AddTiming(_uiMilliseconds, System.Diagnostics.Stopwatch.GetElapsedTime(started).TotalMilliseconds);
                return true;
            }
            if (_winUiSceneVisible || next.OfType<PlatformNativeSegment>().Any(p => _controls.ContainsKey(p.Placement.Handle)))
            {
                CommitWinUiFrame(frame, next, batch, started);
                return true;
            }
            if (_webViews!.HasVisibleContent) _webViews.Clear(batch);
            // Keep source banks while their topology is stable. The one visible
            // scene output stays mounted across bank changes, preserving input
            // targeting while native and raster geometry commit in the same tree.
            var reuseVisible = HasSameWindowTopology(next);
            var placementChanged = !reuseVisible || frame.Plan.Token.DeviceScaleX != _visibleScaleX ||
                frame.Plan.Token.DeviceScaleY != _visibleScaleY || !next.OfType<PlatformNativeSegment>()
                .SequenceEqual(_visible.OfType<PlatformNativeSegment>());
            var bank = reuseVisible ? _visibleBank : 1 - _visibleBank;
            var pool = _banks[bank];
            var layerCount = frame.Rasters.Length + next.OfType<PlatformBackdropSegment>().Count();
            try
            {
                while (pool.Count < layerCount) pool.Add(new(this));
                var sources = new List<Native.CompositionSource>();
                int layerIndex = 0, rasterIndex = 0;
                foreach (var part in next)
                {
                    if (part is PlatformRasterSegment && frame.Slices.Length != 0)
                    {
                        var layer = pool[layerIndex++];
                        layer.Prepare(frame.Shared, frame.Slices[rasterIndex++], frame.Width, frame.Height);
                        sources.Add(new() { Raster = layer.Surface });
                    }
                    else if (part is PlatformBackdropSegment effect)
                    {
                        var layer = pool[layerIndex++];
                        layer.PrepareBackdrop(sources.ToArray(), effect, frame);
                        sources.Add(new() { Raster = layer.Surface });
                    }
                    else if (part is PlatformNativeSegment native && native.Placement.Visible)
                    {
                        var placement = native.Placement;
                        var origin = placement.Transform.Map(placement.Bounds.topLeft);
                        var bounds = Rect.fromLTWH(origin.dx, origin.dy, placement.Bounds.width, placement.Bounds.height);
                        var clip = placement.Clip is { } clipping ? bounds.intersect(clipping) : bounds;
                        if (clip.isEmpty) continue;
                        var scale = frame.Plan.Token.DeviceScaleX;
                        sources.Add(new() { Hwnd = (ulong)_controls[placement.Handle], Generation = (ulong)placement.Handle.InstanceGeneration,
                            X = Pixel(origin.dx), Y = Pixel(origin.dy),
                            Left = Pixel(clip.left) - Pixel(origin.dx), Top = Pixel(clip.top) - Pixel(origin.dy),
                            Right = Pixel(clip.right) - Pixel(origin.dx), Bottom = Pixel(clip.bottom) - Pixel(origin.dy) });
                        float Pixel(double value) => (float)Math.Round(value * scale);
                    }
                }
                if (layerCount != 0)
                {
                    _sceneOutput ??= new(this);
                    _sceneOutput.PrepareScene(sources.ToArray(), frame.Width, frame.Height);
                }
                if (placementChanged)
                {
                    _positions = [];
                    Apply(next, pool, layerCount, batch);
                    if (!reuseVisible)
                        foreach (var old in _banks[_visibleBank]) Position(new(old.Hwnd, 0, 0, 0, 0, 0x0097));
                    CommitPositions();
                }
                // Populate newly shown/resized native backing before the first
                // visible scene commit. Pure movement never invalidates it.
                foreach (var hwnd in _nativeSourcesToPaint)
                    Check(Native.RedrawWindow(hwnd, 0, 0, 0x0585));
                _nativeSourcesToPaint.Clear();
                if (layerCount != 0)
                {
                    Marshal.ThrowExceptionForHR(Native.CommitComposition(_sceneOutput!.Surface));
                    if (!Native.IsWindowVisible(_sceneOutput.Hwnd))
                    {
                        // Only a presenter handoff waits. Steady scrolling never
                        // waits for DWM and retains the same visible output HWND.
                        Marshal.ThrowExceptionForHR(Native.WaitForCompositionCommit(_sceneOutput.Surface));
                        _positions = [];
                        Position(new(_sceneOutput.Hwnd, 0, 0, 0, 0, 0x0053));
                        Position(new(_host!.ChildHwnd, 0, 0, 0, 0, 0x0097));
                        CommitPositions();
                    }
                }
                else if (_sceneOutput is not null && !Native.IsWindowVisible(_host!.ChildHwnd))
                {
                    _presenter!.WaitForPrimaryCopyCompletion();
                    _positions = [];
                    Position(new(_host!.ChildHwnd, 0, 0, 0, 0, 0x0057));
                    Position(new(_sceneOutput.Hwnd, 0, 0, 0, 0, 0x0097));
                    CommitPositions();
                }
                _visible = next;
                _visibleBank = bank;
                _visibleRasterCount = layerCount;
                _visibleScaleX = frame.Plan.Token.DeviceScaleX;
                _visibleScaleY = frame.Plan.Token.DeviceScaleY;
                Volatile.Write(ref _hasVisibleParts, frame.Rasters.Length != 0);
                Volatile.Write(ref _needsReplay, false);
                _commits++;
                AddTiming(_uiMilliseconds, System.Diagnostics.Stopwatch.GetElapsedTime(started).TotalMilliseconds);
            }
            catch
            {
                _positions = null;
                foreach (var layer in pool) layer.Hide();
                _positions = [];
                Apply(_visible, _banks[_visibleBank], _visibleRasterCount, batch);
                CommitPositions();
                throw;
            }
            return true;
    }

    private void CommitWinUiFrame(Frame frame, PlatformCompositionPart[] next, IPlatformViewPlacementBatch batch, long started)
    {
        if (_webViews!.HasVisibleContent) _webViews.Clear(batch);
        if (frame.Slices.Length == 0)
        {
            _winUiBackdrop?.Clear();
            foreach (var old in _visible.OfType<PlatformNativeSegment>())
                if (batch.Contains(old.Placement.Handle)) RunNow(_ => batch.DetachAsync(old.Placement.Handle));
            _presenter!.WaitForPrimaryCopyCompletion();
            foreach (var layers in _banks) foreach (var layer in layers) layer.Hide();
            _sceneOutput?.Hide();
            Check(Native.SetWindowPos(_host!.ChildHwnd, 0, 0, 0, 0, 0, 0x0053));
            _visible = [];
            _winUiSceneVisible = false;
            Volatile.Write(ref _hasVisibleParts, false);
            Volatile.Write(ref _needsReplay, false);
            _commits++;
            return;
        }
        var sameTopology = _winUiSceneVisible && HasSameWindowTopology(next);
        var viewportChanged = _winUiWidth != frame.Width || _winUiHeight != frame.Height;
        var placementChanged = !sameTopology || viewportChanged ||
            _visibleScaleX != frame.Plan.Token.DeviceScaleX || _visibleScaleY != frame.Plan.Token.DeviceScaleY ||
            !next.OfType<PlatformNativeSegment>().SequenceEqual(_visible.OfType<PlatformNativeSegment>());
        var bank = sameTopology ? _visibleBank : 1 - _visibleBank;
        var pool = _banks[bank];
        while (pool.Count < frame.Slices.Length) pool.Add(new(this));
        for (var i = 0; i < frame.Slices.Length; i++)
            pool[i].Prepare(frame.Shared, frame.Slices[i], frame.Width, frame.Height);
        _winUiBackdrop!.Prepare(frame.Plan, frame.Shared, frame.Slices, frame.Width, frame.Height, handle => _controls[handle]);
        foreach (var native in next.OfType<PlatformNativeSegment>())
        {
            var placement = native.Placement;
            var origin = placement.Transform.Map(placement.Bounds.topLeft);
            var bounds = Rect.fromLTWH(origin.dx, origin.dy, placement.Bounds.width, placement.Bounds.height);
            if (placement.Clip is { } clip) bounds = bounds.intersect(clip);
            _winUiControls.SetInputShields(_controls[placement.Handle], next.OfType<PlatformShieldSegment>()
                .Where(shield => shield.PaintOrder > native.PaintOrder)
                .Select(shield => ShieldBounds(shield.Shield).intersect(bounds)).Where(rect => !rect.isEmpty)
                .Select(rect => new Rect(rect.left - origin.dx, rect.top - origin.dy, rect.right - origin.dx, rect.bottom - origin.dy)).ToArray());
        }
        if (frame.Slices.Length != 0) Marshal.ThrowExceptionForHR(Native.CommitComposition(pool[0].Surface));
        // Re-raising every HWND publishes transient sibling orders through DWM,
        // even when batched with DeferWindowPos. Animation-only frames must not
        // change window visibility, size or z-order.
        if (!placementChanged)
        {
            _visible = next;
            Volatile.Write(ref _needsReplay, false);
            _commits++;
            AddTiming(_uiMilliseconds, System.Diagnostics.Stopwatch.GetElapsedTime(started).TotalMilliseconds);
            return;
        }
        _positions = [];
        try
        {
            var handles = next.OfType<PlatformNativeSegment>().Select(p => p.Placement.Handle).ToHashSet();
            foreach (var old in _visible.OfType<PlatformNativeSegment>())
                if (!handles.Contains(old.Placement.Handle) && batch.Contains(old.Placement.Handle))
                    RunNow(_ => batch.DetachAsync(old.Placement.Handle));
            // Back-to-front HWND order preserves the live XAML render/input tree.
            // CreateSurfaceFromHwnd only accepts layered GDI windows, not WinUI.
            var raster = 0;
            foreach (var part in next)
            {
                if (part is PlatformRasterSegment)
                {
                    var layer = pool[raster++];
                    if (!sameTopology || viewportChanged)
                        Position(new(layer.Hwnd, 0, 0, frame.Width, frame.Height, sameTopology ? 0x0054u : 0x0050u));
                }
                else if (part is PlatformNativeSegment native)
                {
                    RunNow(_ => batch.AttachAsync(native.Placement));
                    if (!sameTopology) Position(new(_controls[native.Placement.Handle], 0, 0, 0, 0, 0x0013));
                }
                else if (part is PlatformBackdropSegment effect && (!sameTopology || viewportChanged))
                    Position(new(_winUiBackdrop.WindowFor(effect.PaintOrder), 0, 0, frame.Width, frame.Height, sameTopology ? 0x0054u : 0x0050u));
            }
            for (var i = raster; i < pool.Count; i++) Position(new(pool[i].Hwnd, 0, 0, 0, 0, 0x0097));
            if (bank != _visibleBank)
                foreach (var old in _banks[_visibleBank]) Position(new(old.Hwnd, 0, 0, 0, 0, 0x0097));
            if (!_winUiSceneVisible)
            {
                if (_sceneOutput is not null) Position(new(_sceneOutput.Hwnd, 0, 0, 0, 0, 0x0097));
                Position(new(_host!.ChildHwnd, 0, 0, 0, 0, 0x0097));
            }
            CommitPositions();
            _winUiPlacementBatches++;
            _nativeSourcesToPaint.Clear();
            _visible = next;
            _visibleBank = bank;
            _visibleRasterCount = raster;
            _winUiSceneVisible = true;
            _winUiWidth = frame.Width; _winUiHeight = frame.Height;
            _visibleScaleX = frame.Plan.Token.DeviceScaleX; _visibleScaleY = frame.Plan.Token.DeviceScaleY;
            Volatile.Write(ref _hasVisibleParts, next.Length != 0);
            Volatile.Write(ref _needsReplay, false);
            _commits++;
            AddTiming(_uiMilliseconds, System.Diagnostics.Stopwatch.GetElapsedTime(started).TotalMilliseconds);
        }
        finally { _positions = null; }
    }

    private bool HasSameWindowTopology(PlatformCompositionPart[] next)
    {
        var previousWindows = _visible.Where(p => p is PlatformRasterSegment or PlatformNativeSegment or PlatformBackdropSegment).ToArray();
        var nextWindows = next.Where(p => p is PlatformRasterSegment or PlatformNativeSegment or PlatformBackdropSegment).ToArray();
        return nextWindows.Length != 0 && previousWindows.Length == nextWindows.Length &&
            previousWindows.Zip(nextWindows).All(pair => (pair.First, pair.Second) switch
            {
                (PlatformRasterSegment, PlatformRasterSegment) => true,
                (PlatformBackdropSegment, PlatformBackdropSegment) => true,
                (PlatformNativeSegment before, PlatformNativeSegment after) => before.Placement.Handle == after.Placement.Handle,
                _ => false,
            });
    }

    private void Apply(PlatformCompositionPart[] parts, List<CompositionRaster> layers, int rasterCount, IPlatformViewPlacementBatch batch)
    {
        var handles = parts.OfType<PlatformNativeSegment>().Select(p => p.Placement.Handle).ToHashSet();
        foreach (var old in _visible.OfType<PlatformNativeSegment>())
            if (!handles.Contains(old.Placement.Handle) && batch.Contains(old.Placement.Handle))
            {
                try { RunNow(_ => batch.DetachAsync(old.Placement.Handle)); }
                catch (DorotiCapabilityException) { /* Removal already disabled this retiring instance. */ }
            }
        var raster = 0;
        foreach (var part in parts)
        {
            if (part is PlatformRasterSegment or PlatformBackdropSegment && raster < rasterCount)
                Position(new(layers[raster++].Hwnd, 0, 0, 0, 0, 0x0097));
            else if (part is PlatformNativeSegment native)
            {
                RunNow(_ => batch.AttachAsync(native.Placement));
            }
        }
        for (var index = raster; index < layers.Count; index++) Position(new(layers[index].Hwnd, 0, 0, 0, 0, 0x0097));
    }
    private void Position(WindowPosition position)
    {
        if (_positions is { } positions) positions.Add(position);
        else Check(Native.SetWindowPos(position.Hwnd, 0, position.X, position.Y, position.Width, position.Height, position.Flags));
    }
    private void CommitPositions()
    {
        var positions = _positions!;
        _positions = null;
        if (positions.Count == 0) return;
        var batch = Native.BeginDeferWindowPos(positions.Count);
        if (batch == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
        foreach (var position in positions)
        {
            batch = Native.DeferWindowPos(batch, position.Hwnd, 0, position.X, position.Y, position.Width, position.Height, position.Flags);
            if (batch == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        Check(Native.EndDeferWindowPos(batch));
    }
    private static void RunNow(Func<CancellationToken, ValueTask> action)
    {
        using var cancellation = new CancellationTokenSource();
        var task = action(cancellation.Token);
        if (!task.IsCompleted)
        {
            cancellation.Cancel();
            _ = Observe(task);
            throw new InvalidOperationException("Native placement is retiring; composition was not committed.");
        }
        task.GetAwaiter().GetResult();
    }
    private static async Task Observe(ValueTask task)
    { try { await task; } catch (OperationCanceledException) { } catch (Exception e) { System.Diagnostics.Trace.TraceError(e.ToString()); } }

    private bool Intercepts(int nativeOrder, nint packedScreenPoint)
    {
        var point = new Native.Point { X = (short)(packedScreenPoint.ToInt64() & 0xffff), Y = (short)((packedScreenPoint.ToInt64() >> 16) & 0xffff) };
        if (!Native.ScreenToClient(_parent, ref point)) return true;
        var scale = Native.GetDpiForWindow(_parent) / 96d;
        var logical = new Offset(point.X / scale, point.Y / scale);
        return _visible.OfType<PlatformShieldSegment>().Any(p => p.PaintOrder > nativeOrder && ShieldBounds(p.Shield).contains(logical));
    }
    private static Rect ShieldBounds(PlatformInputShield shield)
    {
        var a = shield.Transform.Map(shield.Bounds.topLeft);
        var b = shield.Transform.Map(shield.Bounds.bottomRight);
        var bounds = new Rect(a.dx, a.dy, b.dx, b.dy);
        return shield.Clip is { } clip ? bounds.intersect(clip) : bounds;
    }

    private nint NativeAt(Native.Point point)
    {
        var scale = Native.GetDpiForWindow(_parent) / 96d;
        var logical = new Offset(point.X / scale, point.Y / scale);
        foreach (var native in _visible.OfType<PlatformNativeSegment>().Reverse())
        {
            var placement = native.Placement;
            var origin = placement.Transform.Map(placement.Bounds.topLeft);
            var bounds = Rect.fromLTWH(origin.dx, origin.dy, placement.Bounds.width, placement.Bounds.height);
            if (placement.Clip is { } clip) bounds = bounds.intersect(clip);
            if (placement.Visible && bounds.contains(logical) &&
                !_visible.OfType<PlatformShieldSegment>().Any(shield => shield.PaintOrder > native.PaintOrder && ShieldBounds(shield.Shield).contains(logical)))
            {
                var hwnd = _controls.GetValueOrDefault(placement.Handle);
                return hwnd != 0 && Native.IsWindowEnabled(hwnd) ? hwnd : 0;
            }
        }
        return 0;
    }

    internal void BeginClose()
    {
        _closed = true;
    }
    internal void ReleaseWinUiIslands()
    {
        _dispatcher?.VerifyThread();
        try { _winUiBackdrop?.Dispose(); }
        finally { _winUiControls.CloseIslands(); }
    }
    public void Dispose()
    {
        if (_dispatcher is null) return;
        _dispatcher.VerifyThread();
        BeginClose();
        // Render worker has joined. Its pending frame has no GPU leases remaining.
        // In-flight scene planning must retain live instances until that join;
        // disposing them at WM_CLOSE races Resolve/Retain on the raster thread.
        if (_session is { } session)
        {
            var close = session.DisposeAsync();
            if (!close.IsCompleted) throw new InvalidOperationException("Windows composition retirement remained after raster shutdown.");
            close.GetAwaiter().GetResult();
        }
        _coordinator?.Dispose();
        _recording?.Dispose(); _recording = null;
        lock (_gate) { foreach (var frame in _ready.Values) frame.Dispose(); _ready.Clear(); }
        foreach (var bank in _banks) foreach (var layer in bank) layer.Dispose();
        _sceneOutput?.Dispose(); _sceneOutput = null;
        _visible = [];
        if (_coordinator is { } coordinator) _dispatcher.DrainShutdown(coordinator.DisposalCompletion);
        _winUiBackdrop?.Dispose();
        _winUiControls.Dispose();
        _webViews?.Dispose();
        _dispatcher.Dispose(); _dispatcher = null;
    }
    private void WriteEvidence(Frame frame)
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_EVIDENCE");
        if (string.IsNullOrWhiteSpace(path)) return;
        if (_captureProbe is null && _winUiSceneVisible && _commits >= 20 &&
            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_PLATFORM_CAPTURE") == "1")
        {
            var parent = _parent;
            var count = int.TryParse(Environment.GetEnvironmentVariable("DOROTI_WINDOWS_PLATFORM_CAPTURE_FRAMES"), out var requested)
                ? Math.Clamp(requested, 1, 60) : 1;
            _captureProbe = Task.Run(() => Native.CaptureProbe(parent, path + ".bmp", (uint)count));
        }
        double[] rasterTimings;
        lock (_gate) rasterTimings = _rasterMilliseconds.ToArray();
        var payload = new
        {
            backend = _winUiSceneVisible ? "Graphite/Vulkan raster slices + live WinUI 3 XAML Islands" :
                _webViews?.HasVisibleContent == true ? "Graphite/Vulkan atlas -> Windows.UI.Composition/WebView2/backdrop" :
                "Graphite/Vulkan GPU atlas -> single DirectComposition scene with live HWND surfaces", commits = _commits,
            readbackBytes = _readbackBytes, dpi = Native.GetDpiForWindow(_parent),
            uploadedBytes = _uploadedBytes + (_webViews?.RasterUploadBytes ?? 0), reusedRasters = _reusedRasters,
            liveHwndSources = Volatile.Read(ref _liveHwndSources),
            winUi = _winUiControls.Snapshot(),
            winUiBackdrop = _winUiBackdrop?.Evidence,
            winUiPlacementBatches = _winUiPlacementBatches,
            captureProbeStatus = _captureProbe?.IsCompletedSuccessfully == true ? _captureProbe.Result : (int?)null,
            frame = frame.Plan.Token, native = _visible.OfType<PlatformNativeSegment>().Select(p => new {
                handle = p.Placement.Handle, hwnd = _controls.GetValueOrDefault(p.Placement.Handle).ToInt64(),
                bounds = Coordinates(p.Placement.Bounds), transform = p.Placement.Transform, order = p.PaintOrder }),
            shields = _visible.OfType<PlatformShieldSegment>().Select(p => new { bounds = Coordinates(ShieldBounds(p.Shield)), order = p.PaintOrder }),
            sessionCommit = _session?.LastCommit, sessionPendingRetirements = _session?.PendingRetirements,
            rasterCount = frame.Rasters.Length, physicalAtomicDisplay = "notVerified",
            hwndBackdrops = _visible.OfType<PlatformBackdropSegment>().Select(effect => new {
                bounds = Coordinates(effect.Bounds), effect.SigmaX, effect.SigmaY,
                source = "DirectComposition live layered HWND and preceding raster surfaces", nativeContentCaptured = false }),
            rasterSlices = frame.Slices.Select(slice => new { order = slice.Segment.PaintOrder,
                bounds = new[] { slice.Bounds.Left, slice.Bounds.Top, slice.Bounds.Right, slice.Bounds.Bottom }, slice.AtlasY }),
            compositionOutputCount = _sceneOutput is null || frame.Rasters.Length == 0 ? 0 : 1,
            probe = Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_PROBE_STATE"),
            effectProbe = Environment.GetEnvironmentVariable("DOROTI_PLATFORM_EFFECT_PROBE_STATE"), webView = _webViews?.Evidence,
            timings = new { sampleLimit = 128, rasterSamples = rasterTimings.Length, uiSamples = _uiMilliseconds.Count,
                rasterAndReadbackP50Ms = Percentile(rasterTimings, .5), rasterAndReadbackP95Ms = Percentile(rasterTimings, .95),
                rasterAndReadbackP99Ms = Percentile(rasterTimings, .99),
                uiCommitP50Ms = Percentile(_uiMilliseconds.ToArray(), .5), uiCommitP95Ms = Percentile(_uiMilliseconds.ToArray(), .95),
                uiCommitP99Ms = Percentile(_uiMilliseconds.ToArray(), .99) },
        };
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        File.WriteAllText(path + ".tmp", json);
        File.Move(path + ".tmp", path, true);
    }
    private static double[] Coordinates(Rect rect) => [rect.left, rect.top, rect.right, rect.bottom];
    private static void AddTiming(Queue<double> values, double value)
    { if (values.Count == 128) values.Dequeue(); values.Enqueue(value); }
    private static double Percentile(double[] values, double percentile)
    { Array.Sort(values); return values.Length == 0 ? 0 : values[Math.Max(0, (int)Math.Ceiling(values.Length * percentile) - 1)]; }
    private sealed class Frame(PlatformCompositionPlan plan, int width, int height, PlatformRasterSegment[] rasters) : IDisposable
    {
        internal PlatformCompositionPlan Plan = plan;
        internal long Started;
        internal long NativeRevision;
        internal WindowsSharedRaster? Shared;
        internal int Width = width, Height = height;
        internal PlatformRasterSegment[] Rasters = rasters;
        internal WindowsCompositionSlice[] Slices = [];
        internal SkiaPlatformRasterContent.CacheScope CacheScope;
        internal string RasterTopology = "";
        internal long ExpectedCacheRevision = -1;
        public void Dispose() { Shared?.Dispose(); Plan.Dispose(); }
    }
    private static void Check(bool success) { if (!success) throw new Win32Exception(Marshal.GetLastWin32Error()); }

    private sealed class CompositionRaster : IDisposable
    {
        private nint _hwnd, _surface;
        private int _width, _height;
        private WindowsCompositionSlice? _rasterContent;
        private readonly Native.SubclassProc _callback;
        private readonly WindowsPlatformViewHost _owner;
        internal nint Hwnd => _hwnd;
        internal nint Surface => _surface;
        internal CompositionRaster(WindowsPlatformViewHost owner)
        {
            _owner = owner;
            _callback = WindowProc;
            _hwnd = Native.CreateWindowExW(0x00200000 | 0x00000020, "STATIC", "Doroti GPU raster slice",
                0x40000000, 0, 0, 0, 0, owner._parent, 0, 0, 0);
            if (_hwnd == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
            try
            {
                Check(Native.SetWindowSubclass(_hwnd, _callback, 1, 0));
                var shared = owner._banks.SelectMany(bank => bank).FirstOrDefault();
                if (shared is null) _surface = owner._presenter!.CreatePlatformRasterSurface(_hwnd);
                else Marshal.ThrowExceptionForHR(Native.CreateSharedCompositionRaster(shared.Surface, (ulong)_hwnd, out _surface));
            }
            catch { Native.DestroyWindow(_hwnd); _hwnd = 0; throw; }
        }
        private nint WindowProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint id, nuint data)
        {
            if (message == 0x0084)
            {
                if (_owner._winUiSceneVisible)
                {
                    var point = new Native.Point { X = (short)(lparam.ToInt64() & 0xffff), Y = (short)((lparam.ToInt64() >> 16) & 0xffff) };
                    Native.ScreenToClient(_owner._parent, ref point);
                    if (_owner.NativeAt(point) != 0) return -1; // HTTRANSPARENT: native WinUI input.
                }
                return 1;
            }
            if (message == 0x0021) return Native.SendMessageW(_owner._parent, message, (nuint)_owner._parent, lparam);
            // Native HWNDs are visually cloaked. Route unshielded native input
            // through their real window procedures; framework input keeps its owner.
            if (message is >= 0x200 and <= 0x20e)
            {
                var point = new Native.Point { X = (short)(lparam.ToInt64() & 0xffff), Y = (short)((lparam.ToInt64() >> 16) & 0xffff) };
                if (message is not (0x20a or 0x20e))
                {
                    Native.ClientToScreen(hwnd, ref point);
                    Native.ScreenToClient(_owner._parent, ref point);
                }
                var packed = (nint)((ushort)point.X | ((uint)(ushort)point.Y << 16));
                var forwarded = message switch { 0x203 => 0x201u, 0x206 => 0x204u, 0x209 => 0x207u, _ => message };
                if (message <= 0x209 && _owner.NativeAt(point) is var native && native != 0)
                {
                    Native.ClientToScreen(_owner._parent, ref point);
                    Native.ScreenToClient(native, ref point);
                    var nativePoint = (nint)((ushort)point.X | ((uint)(ushort)point.Y << 16));
                    return Native.SendMessageW(native, message, wparam, nativePoint);
                }
                return Native.SendMessageW(_owner._parent, forwarded, wparam, packed);
            }
            if (message == 0x20) return Native.SendMessageW(_owner._parent, message, (nuint)_owner._parent, lparam);
            return Native.DefSubclassProc(hwnd, message, wparam, lparam);
        }
        internal void Prepare(WindowsSharedRaster? image, WindowsCompositionSlice slice, int frameWidth, int frameHeight)
        {
            if (slice.Reused && _width == frameWidth && _height == frameHeight && _rasterContent is { } previous &&
                previous.Bounds == slice.Bounds && SkiaPlatformRasterContent.Equivalent(previous.Segment.Commands, slice.Segment.Commands))
            {
                _owner._reusedRasters++;
                return;
            }
            if (slice.Reused || image is null) throw new InvalidOperationException("Missing shared Windows raster surface.");
            Marshal.ThrowExceptionForHR(Native.UpdateCompositionRasterShared(_surface, image.Handle,
                (uint)slice.AtlasY, (uint)slice.Bounds.Width, (uint)slice.Bounds.Height, slice.Bounds.Left, slice.Bounds.Top));
            // Raster source windows stay hidden; only the shared scene output is sized.
            _width = frameWidth; _height = frameHeight;
            _rasterContent = slice;
        }
        internal void PrepareScene(Native.CompositionSource[] sources, int width, int height)
        {
            Marshal.ThrowExceptionForHR(Native.UpdateCompositionScene(_surface, sources, (uint)sources.Length, width, height));
            if (_width == width && _height == height) return;
            Check(Native.SetWindowPos(_hwnd, 0, 0, 0, width, height, 0x0010 | 0x0004));
            _width = width; _height = height;
        }
        internal void PrepareBackdrop(Native.CompositionSource[] sources, PlatformBackdropSegment effect, Frame frame)
        {
            _rasterContent = null;
            var scale = frame.Plan.Token.DeviceScaleX;
            var bounds = effect.Bounds.intersect(Rect.fromLTWH(0, 0, frame.Width / scale, frame.Height / scale));
            if (bounds.isEmpty) bounds = Rect.fromLTWH(0, 0, 0, 0);
            Marshal.ThrowExceptionForHR(Native.UpdateCompositionBackdrop(_surface, sources, (uint)sources.Length,
                (float)(bounds.left * scale), (float)(bounds.top * scale),
                (float)(bounds.right * scale), (float)(bounds.bottom * scale), (float)(effect.SigmaX * scale)));
        }
        internal void Hide() { if (Native.IsWindow(_hwnd)) Native.ShowWindow(_hwnd, 0); }
        public void Dispose()
        {
            if (_surface != 0) Native.DestroyCompositionRaster(_surface);
            _surface = 0;
            if (Native.IsWindow(_hwnd))
            {
                Native.RemoveWindowSubclass(_hwnd, _callback, 1);
                Native.DestroyWindow(_hwnd);
            }
            _hwnd = 0;
            GC.KeepAlive(_callback);
        }
    }
    private static class Native
    {
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_composition_raster_shared_v1", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int UpdateCompositionRasterShared(nint raster, nint handle, uint sourceY, uint width, uint height, int x, int y);

        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_platform_capture_probe_v1", CharSet = CharSet.Unicode)]
        internal static extern int CaptureProbe(nint hwnd, string path, uint frameCount);
        [DllImport("dwmapi.dll")]
        internal static extern int DwmSetWindowAttribute(nint hwnd, uint attribute, ref int value, int size);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_composition_scene_update_v1")]
        internal static extern int UpdateCompositionScene(nint surface, [In] CompositionSource[] sources, uint count, float width, float height);
        [StructLayout(LayoutKind.Sequential)]
        internal struct CompositionSource
        {
            internal nint Raster;
            internal ulong Hwnd;
            internal ulong Generation;
            internal float X, Y, Left, Top, Right, Bottom;
        }
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_composition_raster_create_shared_v1")]
        internal static extern int CreateSharedCompositionRaster(nint shared, ulong hwnd, out nint raster);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_composition_backdrop_update_v1")]
        internal static extern int UpdateCompositionBackdrop(nint raster, [In] CompositionSource[] sources, uint count,
            float left, float top, float right, float bottom, float sigma);
        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern nint CreateRectRgn(int left, int top, int right, int bottom);
        internal delegate nint SubclassProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint id, nuint data);
        [DllImport("comctl32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool SetWindowSubclass(nint hwnd, SubclassProc callback, nuint id, nuint data);
        [DllImport("comctl32.dll")] internal static extern bool RemoveWindowSubclass(nint hwnd, SubclassProc callback, nuint id);
        [DllImport("comctl32.dll")] internal static extern nint DefSubclassProc(nint hwnd, uint message, nuint wparam, nint lparam);
        [StructLayout(LayoutKind.Sequential)] internal struct Point { public int X, Y; }
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_composition_raster_update_region_v1")]
        internal static extern int UpdateCompositionRasterRegion(nint surface, nint pixels, uint width, uint height, uint rowBytes, int x, int y);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_composition_commit_v1")]
        internal static extern int CommitComposition(nint surface);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_composition_wait_commit_v1")]
        internal static extern int WaitForCompositionCommit(nint surface);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_composition_raster_destroy_v1")]
        internal static extern void DestroyCompositionRaster(nint surface);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)] internal static extern nint CreateWindowExW(uint ex, string cls, string text, uint style, int x, int y, int w, int h, nint parent, nint menu, nint instance, nint parameter);
        [DllImport("user32.dll")] internal static extern nint GetWindowLongPtrW(nint hwnd, int index);
        [DllImport("user32.dll")] internal static extern nint SetWindowLongPtrW(nint hwnd, int index, nint value);
        [DllImport("user32.dll")] internal static extern uint GetDpiForWindow(nint hwnd);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool ScreenToClient(nint hwnd, ref Point point);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool ClientToScreen(nint hwnd, ref Point point);
        [DllImport("user32.dll")] internal static extern nint SendMessageW(nint hwnd, uint message, nuint wparam, nint lparam);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool IsWindow(nint hwnd);
        [DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool RedrawWindow(nint hwnd, nint rect, nint region, uint flags);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool IsWindowVisible(nint hwnd);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool IsWindowEnabled(nint hwnd);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool ShowWindow(nint hwnd, int command);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool DestroyWindow(nint hwnd);
        [DllImport("user32.dll", SetLastError = true)] internal static extern nint BeginDeferWindowPos(int count);
        [DllImport("user32.dll", SetLastError = true)] internal static extern nint DeferWindowPos(nint batch, nint hwnd, nint after, int x, int y, int width, int height, uint flags);
        [DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool EndDeferWindowPos(nint batch);
        [DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool SetWindowPos(nint hwnd, nint after, int x, int y, int w, int h, uint flags);
    }
}
