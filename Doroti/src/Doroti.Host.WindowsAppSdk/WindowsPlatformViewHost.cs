using System.ComponentModel;
using System.Runtime.InteropServices;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Live HWND interleaving. Raster slices use the active Graphite recorder,
/// then bounded GPU readback and premultiplied DirectComposition child surfaces. Native content
/// is never captured or replaced. Physical atomic display is not advertised.</summary>
internal sealed class WindowsPlatformViewHost : IDisposable
{
    private readonly Dictionary<PlatformViewHandle, nint> _controls = [];
    private readonly Dictionary<long, Frame> _ready = [];
    private readonly object _gate = new();
    private WindowsWebViewComposition? _webViews;
    private readonly List<CompositionRaster>[] _banks = [[], []];
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
    private bool _needsReplay;
    internal bool NeedsReplay => Volatile.Read(ref _needsReplay);
    private long _commits;
    private long _readbackBytes;
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

    internal WindowsPlatformViewDispatcher Bind(WindowsManagedProductHost host, WindowsManagedVulkanPresenter? presenter)
    {
        if (_parent != 0) throw new InvalidOperationException("PlatformView owner already bound.");
        _parent = host.TopLevelHwnd;
        _host = host;
        _presenter = presenter;
        _dispatcher = new();
        _webViews = new(_parent, () =>
        {
            if (_closed) return;
            // Native navigation/messages can finish after the framework becomes idle.
            // Force replay so the next receipt observes the completed native revision.
            Volatile.Write(ref _needsReplay, true); host.RequestInvalidate();
        }, host.ClearClient, presenter is not null);
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
                Interleaved = presenter is not null,
                SiblingRasterTopology = presenter is not null,
                Created = (handle, hwnd) => _controls.Add(handle, hwnd),
                Destroyed = handle => _controls.Remove(handle),
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
                StagePlacement = (hwnd, x, y, width, height, visible) => Position(new(hwnd, x, y, width, height, 0x0010u | (visible ? 0x0040u : 0x0080u))),
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
        if (!HasPlatformCommands(commands) && !Volatile.Read(ref _hasVisibleParts))
        { renderer.DrawPlatformRasterSegment(canvas, commands, width, height); return; }
        var token = new PlatformCompositionToken(descriptor.ViewId, descriptor.MetricsGeneration,
            Interlocked.Increment(ref _compositionFrame), descriptor.ResizeTargetGeneration, descriptor.DeviceScaleX, descriptor.DeviceScaleY);
        var composition = Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_COMPOSITION") == "overlay"
            ? PlatformViewComposition.NativeOverlay : PlatformViewComposition.InterleavedComposition;
        var plan = PlatformCompositionPlanner.Build(commands, token, _coordinator!, composition,
            effects: WindowsWebViewComposition.Effects);
        if (plan.Parts.OfType<PlatformBackdropSegment>().Any() &&
            plan.Parts.OfType<PlatformNativeSegment>().Any(p => !_webViews!.Contains(p.Placement.Handle)))
        { plan.Dispose(); throw new NotSupportedException("Windows native backdrop requires CompositionVisual attachments, not legacy HWNDs."); }
        SKSurface? atlas = null;
        try
        {
            var rasters = plan.Parts.OfType<PlatformRasterSegment>().ToArray();
            if (plan.Parts.OfType<PlatformShieldSegment>().Any(p => !p.Shield.Transform.IsAxisAligned))
                throw new NotSupportedException("Windows native input shields require axis-aligned rectangles.");
            if (!plan.HasNativeContent)
            {
                foreach (var raster in rasters) renderer.DrawPlatformRasterSegment(canvas, raster.Commands, width, height);
                _recording = new(plan, null, null, width, height, []) { Started = started };
                return;
            }
            var visualComposition = plan.Parts.OfType<PlatformNativeSegment>().All(p => _webViews!.Contains(p.Placement.Handle));
            var slices = new List<WindowsCompositionSlice>();
            var atlasHeight = 0;
            foreach (var raster in rasters)
            {
                if (visualComposition && raster.PaintOrder != 0 && !SkiaPlatformRasterContent.HasDrawing(raster.Commands)) continue;
                var bounds = new SKRectI(0, 0, width, height);
                if (visualComposition && raster.PaintOrder != 0)
                {
                    var coverage = SkiaPlatformRasterContent.Split(raster.Commands, width, height);
                    if (coverage.Count == 0) continue;
                    bounds = new(coverage.Min(s => s.Bounds.Left), coverage.Min(s => s.Bounds.Top),
                        coverage.Max(s => s.Bounds.Right), coverage.Max(s => s.Bounds.Bottom));
                }
                slices.Add(new(raster, bounds, atlasHeight)); atlasHeight = checked(atlasHeight + bounds.Height);
            }
            var atlasWidth = slices.Max(s => s.Bounds.Width);
            // Account for GPU atlas, CPU readback, and current/prepared compositor surfaces.
            var reservedBytes = (long)atlasWidth * atlasHeight * 16;
            if (atlasWidth > 16384 || atlasHeight > 16384 || reservedBytes > 256L * 1024 * 1024)
                throw new InvalidOperationException("Windows PlatformView active/staging resources exceed 256 MiB / 16384 pixels.");
            var info = new SKImageInfo(atlasWidth, atlasHeight, SKColorType.Bgra8888, SKAlphaType.Premul);
            atlas = SkiaGpuSurfaces.CreateCompatible(canvas, info, null);
            atlas.Canvas.Clear(SKColors.Transparent);
            foreach (var slice in slices)
            {
                atlas.Canvas.Save();
                atlas.Canvas.ClipRect(SKRect.Create(0, slice.AtlasY, slice.Bounds.Width, slice.Bounds.Height), SKClipOperation.Intersect, false);
                atlas.Canvas.Translate(-slice.Bounds.Left, slice.AtlasY - slice.Bounds.Top);
                if (slice.Segment.PaintOrder == 0) atlas.Canvas.DrawColor(renderer.PlatformBackgroundColor);
                renderer.DrawPlatformRasterSegment(atlas.Canvas, slice.Segment.Commands, width, height);
                atlas.Canvas.Restore();
            }
            var readback = _presenter!.RequestPlatformReadback(atlas, info);
            _recording = new(plan, atlas, readback, width, height, rasters) { Started = started, Slices = slices.ToArray() };
            atlas = null;
        }
        catch { atlas?.Dispose(); plan.Dispose(); throw; }
    }

    // Called by the raster owner after the presenter has completed its same-queue
    // copy fence and Graphite readback callback, before the native terminal is sent.
    internal void FinishRaster(long causalFrameId, bool accepted)
    {
        var frame = _recording;
        _recording = null;
        if (frame is null) return;
        try
        {
            if (accepted && frame.Readback is { } task)
            {
                if (!task.IsCompleted) throw new InvalidOperationException("PlatformView readback has not retired with the GPU frame.");
                frame.Pixels = task.GetAwaiter().GetResult();
                Interlocked.Add(ref _readbackBytes, frame.Pixels.Pixels.LongLength);
            }
            frame.Atlas?.Dispose();
            frame.Atlas = null;
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
            if (!_session.CommitRetiredFrame(frame.Plan, () => CommitFrame(frame, started))) return false;
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
            if (next.OfType<PlatformNativeSegment>().Any(p => _webViews!.Contains(p.Placement.Handle)))
            {
                _webViews!.Commit(frame.Plan, frame.Pixels, frame.Slices, batch);
                foreach (var layer in _banks[_visibleBank]) layer.Hide();
                _visible = next;
                Volatile.Write(ref _hasVisibleParts, next.Length != 0);
                Volatile.Write(ref _needsReplay, false);
                _commits++;
                return true;
            }
            if (_webViews!.HasVisibleContent) _webViews.Clear(batch);
            // Repainting a spinner or hover must not replace the HWND underneath
            // the pointer. Keep the visible siblings until their layer topology
            // changes; alternating hidden banks on every frame flickers Acrylic
            // and repeatedly invalidates native painting and mouse targeting.
            var reuseVisible = HasSameWindowTopology(next);
            var placementChanged = !reuseVisible || frame.Plan.Token.DeviceScaleX != _visibleScaleX ||
                frame.Plan.Token.DeviceScaleY != _visibleScaleY || !next.OfType<PlatformNativeSegment>()
                .SequenceEqual(_visible.OfType<PlatformNativeSegment>());
            var bank = reuseVisible ? _visibleBank : 1 - _visibleBank;
            var pool = _banks[bank];
            try
            {
                while (pool.Count < frame.Rasters.Length) pool.Add(new(this));
                for (var index = 0; index < frame.Rasters.Length; index++)
                    pool[index].Prepare(frame.Pixels!, index * frame.Height, frame.Width, frame.Height);
                if (placementChanged)
                {
                    _positions = [];
                    Apply(next, pool, frame.Rasters.Length, batch);
                    if (!reuseVisible)
                        foreach (var old in _banks[_visibleBank]) Position(new(old.Hwnd, 0, 0, 0, 0, 0x0097));
                    CommitPositions();
                }
                _visible = next;
                _visibleBank = bank;
                _visibleRasterCount = frame.Rasters.Length;
                _visibleScaleX = frame.Plan.Token.DeviceScaleX;
                _visibleScaleY = frame.Plan.Token.DeviceScaleY;
                if (placementChanged)
                    foreach (var hwnd in _controls.Values) Native.RedrawWindow(hwnd, 0, 0, 0x0001 | 0x0080 | 0x0100 | 0x0400);
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

    private bool HasSameWindowTopology(PlatformCompositionPart[] next)
    {
        var previousWindows = _visible.Where(p => p is PlatformRasterSegment or PlatformNativeSegment).ToArray();
        var nextWindows = next.Where(p => p is PlatformRasterSegment or PlatformNativeSegment).ToArray();
        return nextWindows.Length != 0 && previousWindows.Length == nextWindows.Length &&
            previousWindows.Zip(nextWindows).All(pair => (pair.First, pair.Second) switch
            {
                (PlatformRasterSegment, PlatformRasterSegment) => true,
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
            if (part is PlatformRasterSegment && raster < rasterCount)
                Position(new(layers[raster++].Hwnd, 0, 0, 0, 0, 0x0053));
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

    private bool IsOverNative(nint packedScreenPoint)
    {
        var point = new Native.Point { X = (short)(packedScreenPoint.ToInt64() & 0xffff), Y = (short)((packedScreenPoint.ToInt64() >> 16) & 0xffff) };
        if (!Native.ScreenToClient(_parent, ref point)) return false;
        var scale = Native.GetDpiForWindow(_parent) / 96d;
        var logical = new Offset(point.X / scale, point.Y / scale);
        return _visible.OfType<PlatformNativeSegment>().Any(native =>
        {
            var placement = native.Placement;
            var origin = placement.Transform.Map(placement.Bounds.topLeft);
            var bounds = Rect.fromLTWH(origin.dx, origin.dy, placement.Bounds.width, placement.Bounds.height);
            if (placement.Clip is { } clip) bounds = bounds.intersect(clip);
            return placement.Visible && bounds.contains(logical) && !Intercepts(native.PaintOrder, packedScreenPoint);
        });
    }

    internal void BeginClose()
    {
        _closed = true;
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
        _visible = [];
        if (_coordinator is { } coordinator) _dispatcher.DrainShutdown(coordinator.DisposalCompletion);
        _webViews?.Dispose();
        _dispatcher.Dispose(); _dispatcher = null;
    }
    private void WriteEvidence(Frame frame)
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_EVIDENCE");
        if (string.IsNullOrWhiteSpace(path)) return;
        double[] rasterTimings;
        lock (_gate) rasterTimings = _rasterMilliseconds.ToArray();
        var payload = new
        {
            backend = _webViews?.HasVisibleContent == true ? "Graphite/Vulkan atlas -> Windows.UI.Composition/WebView2/backdrop" :
                "Graphite/Vulkan GPU atlas -> premultiplied DirectComposition HWND", commits = _commits,
            readbackBytes = Interlocked.Read(ref _readbackBytes), dpi = Native.GetDpiForWindow(_parent),
            frame = frame.Plan.Token, native = _visible.OfType<PlatformNativeSegment>().Select(p => new {
                handle = p.Placement.Handle, hwnd = _controls.GetValueOrDefault(p.Placement.Handle).ToInt64(),
                bounds = Coordinates(p.Placement.Bounds), transform = p.Placement.Transform, order = p.PaintOrder }),
            shields = _visible.OfType<PlatformShieldSegment>().Select(p => new { bounds = Coordinates(ShieldBounds(p.Shield)), order = p.PaintOrder }),
            sessionCommit = _session?.LastCommit, sessionPendingRetirements = _session?.PendingRetirements,
            rasterCount = frame.Rasters.Length, physicalAtomicDisplay = "notVerified",
            rasterSlices = frame.Slices.Select(slice => new { order = slice.Segment.PaintOrder,
                bounds = new[] { slice.Bounds.Left, slice.Bounds.Top, slice.Bounds.Right, slice.Bounds.Bottom }, slice.AtlasY }),
            alphaRegionRectangles = _banks[_visibleBank].Take(frame.Rasters.Length).Select(layer => layer.RegionRectangles),
            probe = Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_PROBE_STATE"),
            effectProbe = Environment.GetEnvironmentVariable("DOROTI_PLATFORM_EFFECT_PROBE_STATE"), webView = _webViews?.Evidence,
            timings = new { sampleLimit = 128, rasterAndReadbackP50Ms = Percentile(rasterTimings, .5),
                rasterAndReadbackP95Ms = Percentile(rasterTimings, .95), uiCommitP95Ms = Percentile(_uiMilliseconds.ToArray(), .95) },
        };
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        System.IO.File.WriteAllText(path + ".tmp", json);
        System.IO.File.Move(path + ".tmp", path, true);
    }
    private static double[] Coordinates(Rect rect) => [rect.left, rect.top, rect.right, rect.bottom];
    private static void AddTiming(Queue<double> values, double value)
    { if (values.Count == 128) values.Dequeue(); values.Enqueue(value); }
    private static double Percentile(double[] values, double percentile)
    { Array.Sort(values); return values.Length == 0 ? 0 : values[Math.Max(0, (int)Math.Ceiling(values.Length * percentile) - 1)]; }
    private sealed class Frame(PlatformCompositionPlan plan, SKSurface? atlas,
        Task<SkiaGraphiteReadback>? readback, int width, int height, PlatformRasterSegment[] rasters) : IDisposable
    {
        internal PlatformCompositionPlan Plan = plan;
        internal SKSurface? Atlas = atlas;
        internal long Started;
        internal Task<SkiaGraphiteReadback>? Readback = readback;
        internal SkiaGraphiteReadback? Pixels;
        internal int Width = width, Height = height;
        internal PlatformRasterSegment[] Rasters = rasters;
        internal WindowsCompositionSlice[] Slices = [];
        public void Dispose() { Atlas?.Dispose(); Plan.Dispose(); }
    }
    private static void Check(bool success) { if (!success) throw new Win32Exception(Marshal.GetLastWin32Error()); }

    private sealed class CompositionRaster : IDisposable
    {
        private nint _hwnd, _surface;
        private int _width, _height;
        private int[]? _regionData;
        private readonly Native.SubclassProc _callback;
        private readonly WindowsPlatformViewHost _owner;
        internal nint Hwnd => _hwnd;
        internal int RegionRectangles { get; private set; }
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
                _surface = owner._presenter!.CreatePlatformRasterSurface(_hwnd);
            }
            catch { Native.DestroyWindow(_hwnd); _hwnd = 0; throw; }
        }
        private nint WindowProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint id, nuint data)
        {
            if (message == 0x0084) return _owner.IsOverNative(lparam) ? -1 : 1;
            if (message == 0x0021) return Native.SendMessageW(_owner._parent, message, (nuint)_owner._parent, lparam);
            // HTTRANSPARENT falls through sibling windows, not reliably back
            // into the parent's clipped client. Forward framework areas to the
            // input owner; let unshielded native regions hit their own HWND.
            if (message is >= 0x200 and <= 0x20e)
            {
                var point = new Native.Point { X = (short)(lparam.ToInt64() & 0xffff), Y = (short)((lparam.ToInt64() >> 16) & 0xffff) };
                if (message is not (0x20a or 0x20e))
                {
                    Native.ClientToScreen(hwnd, ref point);
                    Native.ScreenToClient(_owner._parent, ref point);
                }
                var packed = (nint)((uint)(ushort)point.X | ((uint)(ushort)point.Y << 16));
                var forwarded = message switch { 0x203 => 0x201u, 0x206 => 0x204u, 0x209 => 0x207u, _ => message };
                return Native.SendMessageW(_owner._parent, forwarded, wparam, packed);
            }
            if (message == 0x20) return Native.SendMessageW(_owner._parent, message, (nuint)_owner._parent, lparam);
            return Native.DefSubclassProc(hwnd, message, wparam, lparam);
        }
        internal unsafe void Prepare(SkiaGraphiteReadback image, int sourceY, int width, int height)
        {
            if (sourceY < 0 || width <= 0 || height <= 0 || image.RowBytes < checked(width * 4) ||
                ((long)sourceY + height - 1) * image.RowBytes + (long)width * 4 > image.Pixels.LongLength)
                throw new ArgumentOutOfRangeException(nameof(sourceY));
            fixed (byte* bytes = image.Pixels)
            {
                var pixels = bytes + checked(sourceY * image.RowBytes);
                Marshal.ThrowExceptionForHR(Native.UpdateCompositionRaster(_surface, (nint)pixels,
                    checked((uint)width), checked((uint)height), checked((uint)image.RowBytes)));
                // Native sibling clipping still needs the slice's painted region.
                ApplyAlphaRegion(pixels, width, height, image.RowBytes);
            }
            if (_width != width || _height != height)
            {
                Check(Native.SetWindowPos(_hwnd, 0, 0, 0, width, height, 0x0010 | 0x0004));
                _width = width; _height = height;
            }
        }
        private unsafe void ApplyAlphaRegion(byte* pixels, int width, int height, int rowBytes)
        {
            var rectangles = new List<(int Left, int Top, int Right, int Bottom)>();
            var previous = new Dictionary<(int Left, int Right), int>();
            for (var y = 0; y < height; y++)
            {
                var current = new Dictionary<(int Left, int Right), int>();
                for (var x = 0; x < width;)
                {
                    while (x < width && pixels[y * rowBytes + x * 4 + 3] == 0) x++;
                    var left = x;
                    while (x < width && pixels[y * rowBytes + x * 4 + 3] != 0) x++;
                    if (left == x) continue;
                    if (previous.TryGetValue((left, x), out var index))
                    {
                        var old = rectangles[index]; rectangles[index] = (old.Left, old.Top, old.Right, y + 1);
                    }
                    else
                    {
                        index = rectangles.Count;
                        if (index == 16384) throw new InvalidOperationException("PlatformView raster alpha region exceeds 16384 rectangles.");
                        rectangles.Add((left, y, x, y + 1));
                    }
                    current.Add((left, x), index);
                }
                previous = current;
            }
            // ExtCreateRegion requires rectangles sorted top-to-bottom, left-to-right.
            // Vertical merging may change bottom edges; preserving top/left order is sufficient.
            rectangles.Sort((a, b) => a.Top != b.Top ? a.Top.CompareTo(b.Top) : a.Left.CompareTo(b.Left));
            var data = new int[8 + rectangles.Count * 4];
            data[0] = 32; data[1] = 1; data[2] = rectangles.Count; data[3] = rectangles.Count * 16;
            data[6] = width; data[7] = height;
            for (var index = 0; index < rectangles.Count; index++)
            {
                var rect = rectangles[index]; var offset = 8 + index * 4;
                data[offset] = rect.Left; data[offset + 1] = rect.Top; data[offset + 2] = rect.Right; data[offset + 3] = rect.Bottom;
            }
            if (_regionData is not null && data.AsSpan().SequenceEqual(_regionData)) return;
            nint region;
            fixed (int* bytes = data) region = Native.ExtCreateRegion(0, checked((uint)(data.Length * 4)), (nint)bytes);
            if (region == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
            if (Native.SetWindowRgn(_hwnd, region, false) == 0) { Native.DeleteObject(region); throw new Win32Exception(); }
            _regionData = data;
            RegionRectangles = rectangles.Count;
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
        internal delegate nint SubclassProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint id, nuint data);
        [DllImport("comctl32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool SetWindowSubclass(nint hwnd, SubclassProc callback, nuint id, nuint data);
        [DllImport("comctl32.dll")] internal static extern bool RemoveWindowSubclass(nint hwnd, SubclassProc callback, nuint id);
        [DllImport("comctl32.dll")] internal static extern nint DefSubclassProc(nint hwnd, uint message, nuint wparam, nint lparam);
        [StructLayout(LayoutKind.Sequential)] internal struct Point { public int X, Y; }
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_composition_raster_update_v1")]
        internal static extern int UpdateCompositionRaster(nint surface, nint pixels, uint width, uint height, uint rowBytes);
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
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool ShowWindow(nint hwnd, int command);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool DestroyWindow(nint hwnd);
        [DllImport("user32.dll")] internal static extern bool RedrawWindow(nint hwnd, nint rect, nint region, uint flags);
        [DllImport("user32.dll", SetLastError = true)] internal static extern int SetWindowRgn(nint hwnd, nint region, [MarshalAs(UnmanagedType.Bool)] bool redraw);
        [DllImport("user32.dll", SetLastError = true)] internal static extern nint BeginDeferWindowPos(int count);
        [DllImport("user32.dll", SetLastError = true)] internal static extern nint DeferWindowPos(nint batch, nint hwnd, nint after, int x, int y, int width, int height, uint flags);
        [DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool EndDeferWindowPos(nint batch);
        [DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool SetWindowPos(nint hwnd, nint after, int x, int y, int w, int h, uint flags);
        [DllImport("gdi32.dll")] internal static extern bool DeleteObject(nint value);
        [DllImport("gdi32.dll", SetLastError = true)] internal static extern nint ExtCreateRegion(nint transform, uint bytes, nint data);
    }
}
