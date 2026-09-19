#if ANDROID
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;
using NativeView = Android.Views.View;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

public sealed class AndroidPlatformViewDispatcher : IPlatformViewDispatcher
{
    public ValueTask InvokeAsync(Func<ValueTask> action)
    {
        if (Microsoft.Maui.ApplicationModel.MainThread.IsMainThread) return action();
        return new(Microsoft.Maui.ApplicationModel.MainThread.InvokeOnMainThreadAsync(async () => await action()));
    }
    internal static void VerifyThread()
    {
        if (!Microsoft.Maui.ApplicationModel.MainThread.IsMainThread)
            throw new InvalidOperationException("Android PlatformView requires the UI thread.");
    }
}

/// <summary>Live Android Views and Graphite raster readback in one window View hierarchy.
/// The SurfaceView remains below that window. No native snapshot or texture fallback.</summary>
internal sealed class AndroidPlatformViewHost(DorotiGraphiteView owner, MauiTextInputBridge textInput) : IDisposable
{
    private static readonly PlatformEffectSupport NativeEffects = new(true, 1, 32, Saturation: true,
        Reason: "Android RenderNode supports one bounded Gaussian backdrop with logical sigma at most 32.");
    private DorotiAndroidVulkanView? _surface;
    private FrameLayout? _container;
    private PlatformViewCoordinator? _coordinator;
    private PlatformCompositionSession? _session;
    private long _compositionFrame;
    private readonly Dictionary<PlatformViewHandle, Instance> _instances = [];
    private PlatformViewPlacement[] _visible = [];
    private readonly List<RasterView> _rasters = [];
    private readonly List<ShieldView> _shields = [];
    private AndroidPlatformBackdropView? _backdrop;
    private Pending? _pending;
    private bool _disposed;
    private double _scaleX = 1, _scaleY = 1;
    private long _rasterEpoch, _commits, _readbackFrames, _readbackBytes, _reusedSlices;
    private readonly bool _optimize = Environment.GetEnvironmentVariable("DOROTI_ANDROID_PLATFORM_RASTER_MODE") != "baseline";
    private readonly bool _routeRaster = Environment.GetEnvironmentVariable("DOROTI_ANDROID_PLATFORM_RASTER_MODE") is not ("baseline" or "full-regions");
    private readonly bool _profile = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Intent?.GetStringExtra("DOROTI_MAUI_EVIDENCE") == "1";
    private const int SliceStride = 128;
    internal bool RejectFrame { get; private set; }
    internal IEnumerable<IPlatformViewFactory> CreateFactories(Func<IApplicationResourceHostCapability> resources) =>
        [new Factory(this, false), new Factory(this, true), new Factory(this, true, webView: true, resources)];
    internal void Configure(PlatformViewCoordinator coordinator)
    { _coordinator = coordinator; owner.GpuResourcesReleasing += InvalidateRasterCache; }
    private void InvalidateRasterCache() => _rasterEpoch++;
    internal void Bind(DorotiAndroidVulkanView surface, FrameLayout container)
    {
        AndroidPlatformViewDispatcher.VerifyThread();
        if (_container is not null && _container != container)
            throw new InvalidOperationException("A live Android PlatformView owner cannot change containers.");
        _surface = surface; _container = container;
    }
    private FrameLayout Container
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_container is null && owner.Handler?.PlatformView is DorotiAndroidViewContainer container)
                Bind(container.Surface, container);
            return _container ?? throw new InvalidOperationException("Android PlatformView owner is not attached.");
        }
    }
    private static bool HasCommands(IReadOnlyList<SceneCommand> commands, int depth = 0)
    {
        if (depth > 256) throw new InvalidDataException("Retained platform scene depth exceeded.");
        return commands.Any(c => c.Operation is "platformView" or "inputShield" ||
            c.HostPayload is SceneRetainedPayload retained && HasCommands(retained.Commands, depth + 1));
    }
    internal void Draw(SkiaSceneRenderer renderer, SKCanvas canvas, IReadOnlyList<SceneCommand> commands,
        DorotiFrameDescriptor descriptor, int width, int height)
    {
        AndroidPlatformViewDispatcher.VerifyThread();
        if (_pending is not null) throw new InvalidOperationException("Unretired Android platform frame.");
        RejectFrame = false;
        if (_visible.Length == 0 && _rasters.Count == 0 && !HasCommands(commands))
        { renderer.DrawPlatformRasterSegment(canvas, commands, width, height); return; }
        var parent = Container;
        var token = new PlatformCompositionToken(descriptor.ViewId, descriptor.MetricsGeneration,
            ++_compositionFrame, descriptor.ResizeTargetGeneration, descriptor.DeviceScaleX, descriptor.DeviceScaleY);
        var mode = Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_COMPOSITION") == "overlay"
            ? PlatformViewComposition.NativeOverlay : PlatformViewComposition.InterleavedComposition;
        PlatformCompositionPlan plan;
        try { plan = PlatformCompositionPlanner.Build(commands, token, _coordinator!, mode,
            effects: OperatingSystem.IsAndroidVersionAtLeast(31) ? NativeEffects : null); }
        catch (DorotiCapabilityException) when (HasStaleNative(commands))
        {
            // Widget removal disables native input before the replacement scene arrives.
            // A replay of the previous scene is superseded, never a renderer/device fault.
            RejectFrame = true; return;
        }
        SKSurface? atlas = null;
        IPlatformViewPlacementBatch? batch = null;
        try
        {
            var placements = plan.Parts.OfType<PlatformNativeSegment>().Select(p => p.Placement).ToArray();
            foreach (var placement in placements) _instances[placement.Handle].Validate(placement);
            foreach (var part in plan.Parts.OfType<PlatformShieldSegment>()) ValidateTransform(part.Shield.Transform);
            var handles = placements.Select(p => p.Handle).Concat(_visible.Select(p => p.Handle)
                .Where(IsLive)).Distinct();
            if (!_coordinator!.TryBeginPlacementBatch(handles, out batch))
            { RejectFrame = true; plan.Dispose(); return; }
            var allRasters = plan.Parts.OfType<PlatformRasterSegment>().ToArray();
            var backdropPart = plan.Parts.OfType<PlatformBackdropSegment>().SingleOrDefault();
            var backdropSample = backdropPart is not null ? BackdropSample(backdropPart, token, width, height) : default;
            if (!plan.HasNativeContent || _optimize && mode == PlatformViewComposition.NativeOverlay)
            {
                foreach (var raster in allRasters) renderer.DrawPlatformRasterSegment(canvas, raster.Commands, width, height);
                _pending = new(plan, batch!, null, null, [], width, height); return;
            }
            var cached = new Dictionary<int, RasterView>();
            var changed = new List<RasterSlice>();
            for (var i = 0; i < allRasters.Length; i++)
            {
                var raster = allRasters[i];
                var overlayBounds = new SKRectI(0, 0, width, height);
                // The first slice is below every native view, so WSI can display it directly.
                if (_optimize && i == 0 && plan.Parts[0] == raster)
                {
                    renderer.DrawPlatformRasterSegment(canvas, raster.Commands, width, height);
                    if (backdropPart is null || backdropSample.IsEmpty) continue;
                }
                else if (_routeRaster)
                {
                    overlayBounds = RasterOverlayBounds(plan, raster.PaintOrder, token, width, height, backdropSample);
                    // Outside earlier native Views (and the blur sample), the normal
                    // SurfaceView already gives the correct paint order. Exclude the
                    // overlay rectangle to avoid applying translucent foreground twice.
                    canvas.Save();
                    if (!overlayBounds.IsEmpty) canvas.ClipRect(overlayBounds, SKClipOperation.Difference, false);
                    renderer.DrawPlatformRasterSegment(canvas, raster.Commands, width, height);
                    canvas.Restore();
                    if (overlayBounds.IsEmpty) continue;
                }
                if (_optimize && !SkiaPlatformRasterContent.HasDrawing(raster.Commands)) continue;
                var slices = _optimize && i == 0 && backdropPart is not null ?
                    new[] { new SkiaPlatformRasterContent.Slice(raster.Commands, backdropSample) } :
                    _optimize ? SkiaPlatformRasterContent.Split(raster.Commands, width, height) :
                    [new SkiaPlatformRasterContent.Slice(raster.Commands, new(0, 0, width, height))];
                if (slices.Count >= SliceStride || cached.Count + changed.Count + slices.Count > SliceStride)
                    throw new InvalidOperationException("Android platform raster slice count exceeds 128.");
                for (var index = 0; index < slices.Count; index++)
                {
                    var slice = slices[index];
                    if (_routeRaster && i != 0)
                    {
                        var clipped = Intersect(slice.Bounds, overlayBounds);
                        if (clipped.IsEmpty) continue;
                        slice = slice with { Bounds = clipped };
                    }
                    var key = checked(raster.PaintOrder * SliceStride + index);
                    var existing = _optimize ? _rasters.FirstOrDefault(r => r.PixelWidth == width && r.PixelHeight == height &&
                        r.Epoch == _rasterEpoch && r.Bounds == slice.Bounds && !cached.ContainsValue(r) &&
                        SkiaPlatformRasterContent.Equivalent(r.Commands, slice.Commands)) : null;
                    if (existing is not null) cached.Add(key, existing);
                    else changed.Add(new(new(key, slice.Commands), slice.Bounds));
                }
            }
            var rasters = changed.ToArray();
            if (rasters.Length == 0)
            { _pending = new(plan, batch!, null, null, [], width, height, cached); return; }
            var atlasWidth = rasters.Max(r => r.Bounds.Width);
            var atlasHeight = 0;
            foreach (var raster in rasters) { raster.AtlasY = atlasHeight; atlasHeight = checked(atlasHeight + raster.Bounds.Height); }
            // GPU atlas, managed readback, packing scratch and old/new bitmap banks.
            var reservedBytes = _rasters.Sum(r => r.Bytes) + cached.Values.Sum(r => r.Bytes) +
                rasters.Sum(r => (long)r.Bounds.Width * r.Bounds.Height * 12) + (long)atlasWidth * atlasHeight * 8;
            if (atlasWidth > 16384 || atlasHeight > 16384 || reservedBytes > 256L * 1024 * 1024)
                throw new InvalidOperationException($"Android platform raster banks exceed 256 MiB / 16384 pixels: atlas={atlasWidth}x{atlasHeight}, slices={rasters.Length}, bytes={reservedBytes}.");
            var info = new SKImageInfo(atlasWidth, atlasHeight, SKColorType.Rgba8888, SKAlphaType.Premul);
            atlas = SkiaGpuSurfaces.CreateCompatible(canvas, info, null);
            atlas.Canvas.Clear(SKColors.Transparent);
            for (var i = 0; i < rasters.Length; i++)
            {
                var raster = rasters[i];
                atlas.Canvas.Save();
                atlas.Canvas.ClipRect(SKRect.Create(0, raster.AtlasY, raster.Bounds.Width, raster.Bounds.Height), SKClipOperation.Intersect, false);
                atlas.Canvas.Translate(-raster.Bounds.Left, raster.AtlasY - raster.Bounds.Top);
                if (raster.Segment.PaintOrder == 0) atlas.Canvas.DrawColor(renderer.PlatformBackgroundColor);
                renderer.DrawPlatformRasterSegment(atlas.Canvas, raster.Segment.Commands, width, height);
                atlas.Canvas.Restore();
            }
            _pending = new(plan, batch!, atlas, _surface!.RequestPlatformReadback(atlas, info), rasters, width, height, cached);
        }
        catch (DorotiCapabilityException) when (HasStaleNative(commands)) { atlas?.Dispose(); batch?.Dispose(); plan.Dispose(); RejectFrame = true; }
        catch { atlas?.Dispose(); batch?.Dispose(); plan.Dispose(); throw; }
    }
    private bool IsLive(PlatformViewHandle handle)
    {
        try { return _coordinator!.Resolve(handle.InstanceId) == handle; }
        catch (DorotiCapabilityException) { return false; }
    }
    private static SKRectI Intersect(SKRectI a, SKRectI b)
    {
        var left = Math.Max(a.Left, b.Left); var top = Math.Max(a.Top, b.Top);
        var right = Math.Min(a.Right, b.Right); var bottom = Math.Min(a.Bottom, b.Bottom);
        // SKRectI.IsEmpty only recognizes its empty sentinel, not inverted bounds.
        return left >= right || top >= bottom ? default : new(left, top, right, bottom);
    }
    private static SKRectI RasterOverlayBounds(PlatformCompositionPlan plan, int order, PlatformCompositionToken token,
        int width, int height, SKRectI backdropSample)
    {
        var result = backdropSample;
        foreach (var native in plan.Parts.OfType<PlatformNativeSegment>().Where(n => n.PaintOrder < order && n.Placement.Visible))
        {
            var p = native.Placement;
            var rect = Map(p.Bounds, p.Transform, p.Clip);
            if (rect.isEmpty) continue;
            var pixels = Intersect(new((int)Math.Floor(rect.left * token.DeviceScaleX), (int)Math.Floor(rect.top * token.DeviceScaleY),
                (int)Math.Ceiling(rect.right * token.DeviceScaleX), (int)Math.Ceiling(rect.bottom * token.DeviceScaleY)), new(0, 0, width, height));
            if (pixels.IsEmpty) continue;
            result = result.IsEmpty ? pixels : new(Math.Min(result.Left, pixels.Left), Math.Min(result.Top, pixels.Top),
                Math.Max(result.Right, pixels.Right), Math.Max(result.Bottom, pixels.Bottom));
        }
        return result.IsEmpty ? default : result;
    }
    private static SKRectI BackdropSample(PlatformBackdropSegment backdrop, PlatformCompositionToken token, int width, int height)
    {
        var sx = token.DeviceScaleX; var sy = token.DeviceScaleY;
        var bounds = backdrop.Bounds;
        var rect = Intersect(new((int)Math.Floor((bounds.left - backdrop.SigmaX * 3) * sx),
            (int)Math.Floor((bounds.top - backdrop.SigmaY * 3) * sy),
            (int)Math.Ceiling((bounds.right + backdrop.SigmaX * 3) * sx),
            (int)Math.Ceiling((bounds.bottom + backdrop.SigmaY * 3) * sy)), new(0, 0, width, height));
        if (rect.Width > 4096 || rect.Height > 4096 || (long)rect.Width * rect.Height * 4 > 64L * 1024 * 1024)
            throw new InvalidOperationException("Android backdrop sample exceeds its 4096px / 64 MiB limit.");
        return rect;
    }
    private bool HasStaleNative(IReadOnlyList<SceneCommand> commands) => commands.Any(c =>
        c.HostPayload is ScenePlatformViewPayload native && !IsLive(native.Handle) ||
        c.HostPayload is SceneRetainedPayload retained && HasStaleNative(retained.Commands));
    internal bool Finish(bool accepted)
    {
        AndroidPlatformViewDispatcher.VerifyThread();
        if (!accepted || _pending is null || _disposed) return FinishCore(accepted);
        var plan = _pending.Plan;
        _session ??= new(plan.Token.OwnerViewId);
        return _session.CommitRetiredFrame(plan, () => FinishCore(accepted));
    }
    private bool FinishCore(bool accepted)
    {
        AndroidPlatformViewDispatcher.VerifyThread();
        var frame = _pending; _pending = null;
        if (frame is null) return accepted;
        var prepared = new List<RasterView>();
        var started = System.Diagnostics.Stopwatch.GetTimestamp();
        try
        {
            if (!accepted || _disposed) return false;
            var parent = Container;
            if (frame.Width != parent.Width || frame.Height != parent.Height) return false;
            var desired = frame.Cached is { } cached ? new Dictionary<int, RasterView>(cached) : [];
            if (frame.Readback is { } task)
            {
                if (!task.IsCompleted) throw new InvalidOperationException("Platform readback outlived its completion fence.");
                var pixels = task.GetAwaiter().GetResult();
                _readbackFrames++; _readbackBytes += pixels.Pixels.LongLength;
                for (var i = 0; i < frame.Rasters.Length; i++)
                {
                    var slice = frame.Rasters[i];
                    var bitmap = Bitmap.CreateBitmap(slice.Bounds.Width, slice.Bounds.Height, Bitmap.Config.Argb8888!)!;
                    try
                    {
                        var packed = new byte[checked(slice.Bounds.Width * slice.Bounds.Height * 4)];
                        for (var y = 0; y < slice.Bounds.Height; y++)
                            Buffer.BlockCopy(pixels.Pixels, (slice.AtlasY + y) * pixels.RowBytes, packed,
                                y * slice.Bounds.Width * 4, slice.Bounds.Width * 4);
                        using var buffer = Java.Nio.ByteBuffer.Wrap(packed)!;
                        bitmap.CopyPixelsFromBuffer(buffer);
                        var raster = new RasterView(parent.Context!, bitmap, slice.Segment.Commands, frame.Width, frame.Height, _rasterEpoch, slice.Bounds);
                        prepared.Add(raster);
                        desired.Add(slice.Segment.PaintOrder, raster);
                    }
                    catch { bitmap.Dispose(); throw; }
                }
            }
            var next = frame.Plan.Parts.OfType<PlatformNativeSegment>().Select(p => p.Placement).ToArray();
            _scaleX = frame.Plan.Token.DeviceScaleX; _scaleY = frame.Plan.Token.DeviceScaleY;
            foreach (var p in next) CompleteNow(frame.Batch.AttachAsync(p));
            var nextHandles = next.Select(p => p.Handle).ToHashSet();
            foreach (var old in _visible.Where(p => !nextHandles.Contains(p.Handle) && frame.Batch.Contains(p.Handle)))
                CompleteNow(frame.Batch.DetachAsync(old.Handle));
            // Keep Android View/RenderNode identity during animation. Replacing only
            // the immutable bitmap avoids subtree events and touch-target churn.
            if (_optimize)
            {
                foreach (var item in desired.ToArray())
                {
                    if (!prepared.Contains(item.Value)) continue;
                    var slot = _rasters.FirstOrDefault(r => r.PaintOrder == item.Key && !desired.ContainsValue(r));
                    if (slot is null) continue;
                    slot.Adopt(item.Value); prepared.Remove(item.Value); item.Value.Dispose(); desired[item.Key] = slot;
                }
            }
            foreach (var raster in _rasters.Where(r => !desired.ContainsValue(r))) { parent.RemoveView(raster); raster.Dispose(); }
            _rasters.Clear();
            var shieldParts = frame.Plan.Parts.OfType<PlatformShieldSegment>().ToArray();
            while (_shields.Count > shieldParts.Length)
            { var shield = _shields[^1]; _shields.RemoveAt(_shields.Count - 1); parent.RemoveView(shield); shield.Dispose(); }
            while (_shields.Count < shieldParts.Length)
            {
                var shield = new ShieldView(parent.Context!, _surface!) { ImportantForAccessibility = ImportantForAccessibility.No };
                _shields.Add(shield); parent.AddView(shield);
            }
            foreach (var raster in prepared)
            {
                parent.AddView(raster, new FrameLayout.LayoutParams(raster.Bounds.Width, raster.Bounds.Height)
                    { LeftMargin = raster.Bounds.Left, TopMargin = raster.Bounds.Top, Gravity = GravityFlags.Left | GravityFlags.Top });
                raster.Layout(raster.Bounds.Left, raster.Bounds.Top, raster.Bounds.Right, raster.Bounds.Bottom);
            }
            foreach (var (order, raster) in desired)
            {
                raster.PaintOrder = order; _rasters.Add(raster);
                Place(raster, Rect.fromLTWH(raster.Bounds.Left, raster.Bounds.Top, raster.Bounds.Width, raster.Bounds.Height), 1, 1);
            }
            prepared.Clear();
            var paintOrder = new List<NativeView>();
            var shieldIndex = 0;
            if (OperatingSystem.IsAndroidVersionAtLeast(31) && _backdrop is not null && !frame.Plan.Parts.OfType<PlatformBackdropSegment>().Any())
            { parent.RemoveView(_backdrop); _backdrop.Dispose(); _backdrop = null; }
            foreach (var part in frame.Plan.Parts.OrderBy(p => p.PaintOrder))
            {
                switch (part)
                {
                    case PlatformRasterSegment raster:
                        foreach (var slice in _rasters.Where(r => r.PaintOrder / SliceStride == raster.PaintOrder).OrderBy(r => r.PaintOrder))
                            paintOrder.Add(slice);
                        break;
                    case PlatformNativeSegment native:
                        paintOrder.Add(_instances[native.Placement.Handle].Clip); break;
                    case PlatformShieldSegment shield:
                        var bounds = Map(shield.Shield.Bounds, shield.Shield.Transform, shield.Shield.Clip);
                        var view = _shields[shieldIndex++];
                        Place(view, bounds, frame.Plan.Token.DeviceScaleX, frame.Plan.Token.DeviceScaleY);
                        paintOrder.Add(view); break;
                    case PlatformBackdropSegment backdrop when OperatingSystem.IsAndroidVersionAtLeast(31):
                        if (!parent.IsHardwareAccelerated) throw new PlatformNotSupportedException("Native backdrop requires Android hardware acceleration.");
                        if (_backdrop is null) { _backdrop = new(parent.Context!); parent.AddView(_backdrop); }
                        var viewport = Rect.fromLTWH(0, 0, frame.Width / _scaleX, frame.Height / _scaleY);
                        var visible = backdrop.Bounds.intersect(viewport);
                        Place(_backdrop, visible, _scaleX, _scaleY);
                        _backdrop.UpdateFrame(BackdropSample(backdrop, frame.Plan.Token, frame.Width, frame.Height),
                            paintOrder.Where(v => v is not ShieldView).ToArray(),
                            (float)(backdrop.SigmaX * _scaleX), (float)(backdrop.SigmaY * _scaleY), (float)(backdrop.Style?.Saturation ?? 1));
                        paintOrder.Add(_backdrop); break;
                }
            }
            if (!paintOrder.OrderBy(parent.IndexOfChild).SequenceEqual(paintOrder))
                foreach (var view in paintOrder) view.BringToFront();
            _visible = next;
            parent.Invalidate();
            _reusedSlices += frame.Cached?.Count ?? 0;
            if (_profile)
                Android.Util.Log.Info("DorotiPlatformFrame", $"mode={(_optimize ? "optimized" : "baseline")} frame={++_commits} readbackFrames={_readbackFrames} readbackBytes={_readbackBytes} reusedSlices={_reusedSlices} changed={frame.Rasters.Length} cached={frame.Cached?.Count ?? 0} uiMs={System.Diagnostics.Stopwatch.GetElapsedTime(started).TotalMilliseconds:F3}");
            return true;
        }
        finally
        {
            foreach (var raster in prepared) raster.Dispose();
            frame.Dispose();
        }
    }
    private static void CompleteNow(ValueTask task)
    {
        if (!task.IsCompleted) throw new InvalidOperationException("Reserved Android UI operation unexpectedly suspended.");
        task.GetAwaiter().GetResult();
    }
    private sealed record Pending(PlatformCompositionPlan Plan, IPlatformViewPlacementBatch Batch, SKSurface? Atlas,
        Task<SkiaGraphiteReadback>? Readback, RasterSlice[] Rasters, int Width, int Height,
        Dictionary<int, RasterView>? Cached = null) : IDisposable
    {
        public void Dispose() { Atlas?.Dispose(); Batch.Dispose(); Plan.Dispose(); }
    }
    private sealed record RasterSlice(PlatformRasterSegment Segment, SKRectI Bounds)
    {
        internal int AtlasY { get; set; }
    }
    private sealed class RasterView(Context context, Bitmap bitmap, IReadOnlyList<SceneCommand> commands,
        int width, int height, long epoch, SKRectI bounds) : NativeView(context)
    {
        private Bitmap? _bitmap = bitmap;
        internal IReadOnlyList<SceneCommand> Commands => commands;
        internal int PixelWidth => width;
        internal int PixelHeight => height;
        internal long Epoch => epoch;
        internal SKRectI Bounds => bounds;
        internal long Bytes => (long)bounds.Width * bounds.Height * 4;
        internal int PaintOrder { get; set; }
        internal void Adopt(RasterView next)
        {
            var old = _bitmap; _bitmap = next._bitmap; next._bitmap = null;
            commands = next.Commands; width = next.PixelWidth; height = next.PixelHeight; epoch = next.Epoch;
            bounds = next.Bounds;
            old?.Dispose(); Invalidate();
        }
        protected override void OnDraw(Android.Graphics.Canvas canvas)
        { if (_bitmap is { } value) canvas.DrawBitmap(value, 0, 0, null); }
        public override bool OnTouchEvent(MotionEvent? e) => false;
        protected override void Dispose(bool disposing) { if (disposing) { _bitmap?.Dispose(); _bitmap = null; } base.Dispose(disposing); }
    }
    private sealed class ShieldView(Context context, DorotiAndroidVulkanView surface) : NativeView(context)
    {
        public override bool OnTouchEvent(MotionEvent? e)
        {
            if (e is null) return false;
            using var copy = MotionEvent.Obtain(e)!;
            copy.OffsetLocation(Left, Top);
            return surface.OnTouchEvent(copy);
        }
    }
    private static void ValidateTransform(PlatformViewTransform transform)
    {
        if (!transform.IsAxisAligned || transform.M11 != 1 || transform.M22 != 1)
            throw new NotSupportedException("Android platform views support translation and rectangular clipping only.");
    }
    private static Rect Map(Rect bounds, PlatformViewTransform transform, Rect? clip)
    {
        var origin = transform.Map(bounds.topLeft);
        var result = Rect.fromLTWH(origin.dx, origin.dy, bounds.width, bounds.height);
        return clip is { } value ? result.intersect(value) : result;
    }
    private static void Place(NativeView view, Rect bounds, double sx, double sy)
    {
        var left = (int)Math.Ceiling(bounds.left * sx); var top = (int)Math.Ceiling(bounds.top * sy);
        var right = (int)Math.Floor(bounds.right * sx); var bottom = (int)Math.Floor(bounds.bottom * sy);
        if (view.LayoutParameters is FrameLayout.LayoutParams current &&
            current.Width == Math.Max(0, right - left) && current.Height == Math.Max(0, bottom - top) &&
            view.Width == current.Width && view.Height == current.Height)
        {
            // Position-only scroll updates must not request another tree measure/layout.
            if (current.LeftMargin != left) current.LeftMargin = left;
            if (current.TopMargin != top) current.TopMargin = top;
            if (view.Left != left) view.OffsetLeftAndRight(left - view.Left);
            if (view.Top != top) view.OffsetTopAndBottom(top - view.Top);
            return;
        }
        view.LayoutParameters = new FrameLayout.LayoutParams(Math.Max(0, right - left), Math.Max(0, bottom - top))
            { LeftMargin = left, TopMargin = top, Gravity = GravityFlags.Left | GravityFlags.Top };
        view.Layout(left, top, Math.Max(left, right), Math.Max(top, bottom));
    }
    private sealed class Factory(AndroidPlatformViewHost host, bool editor, bool webView = false,
        Func<IApplicationResourceHostCapability>? resources = null) : IPlatformViewFactory
    {
        public string ViewType => webView ? "doroti/webview" : editor ? "doroti/native-editor" : "doroti/native-button";
        public PlatformViewSupport QuerySupport(PlatformViewRequest request)
        {
            var supported = request.ViewType == ViewType &&
                request.Composition is PlatformViewComposition.NativeOverlay or PlatformViewComposition.InterleavedComposition &&
                (request.Effects & ~PlatformViewEffects.RectClip) == 0;
            return new("Android/View/Graphite-readback", Environment.OSVersion.VersionString, ViewType, supported,
                request.Composition, PlatformViewEffects.RectClip, Reason: supported ? null : "Only native button/editor, translation and rect clip are supported.",
                NativeBackdropBlur: supported && request.Composition == PlatformViewComposition.InterleavedComposition && OperatingSystem.IsAndroidVersionAtLeast(31),
                Capabilities: new(PlatformViewRepresentation.NativeHierarchy, PlatformViewTransport.BoundedReadback,
                    PlatformViewInputPolicy.DirectNative, OperatingSystem.IsAndroidVersionAtLeast(31) ? NativeEffects : PlatformEffectSupport.Unsupported),
                WebViewCommands: supported && webView);
        }
        public async ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
            Action<PlatformViewHandle> onFocused, CancellationToken cancellationToken)
        {
            AndroidPlatformViewDispatcher.VerifyThread(); cancellationToken.ThrowIfCancellationRequested();
            var text = parameters.IsEmpty ? (editor ? "Native editor" : "Native button") : System.Text.Encoding.UTF8.GetString(parameters.Span);
            var options = webView ? AndroidWebViewSession.Parse(text) : null;
            var content = options is null ? null : await AndroidWebViewSession.LoadContent(options, resources?.Invoke(), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            AndroidPlatformViewDispatcher.VerifyThread();
            var instance = new Instance(host, handle, editor, text, onFocused, options, content);
            host._instances.Add(handle, instance);
            return instance;
        }
    }
    private sealed class Instance : IPlatformViewInstance, IPlatformWebViewInstance
    {
        private readonly AndroidPlatformViewHost _host;
        private readonly PlatformViewHandle _handle;
        private readonly NativeView _control;
        private readonly Action<PlatformViewHandle> _focused;
        private readonly AndroidWebViewSession? _web;
        public event Action<WebViewEvent>? WebViewChanged;
        public Task<WebViewResult> ExecuteAsync(WebViewCommand command, CancellationToken cancellationToken) =>
            _web?.ExecuteAsync(command, cancellationToken) ?? throw new WebViewException(WebViewError.Unsupported, "Not a WebView.");
        internal FrameLayout Clip { get; }
        private bool _disabled, _disposed;
        private int _clicks;
        internal Instance(AndroidPlatformViewHost host, PlatformViewHandle handle, bool editor, string text, Action<PlatformViewHandle> focused,
            WebViewOptions? webOptions = null, Dictionary<string, byte[]>? content = null)
        {
            _host = host; _handle = handle; _focused = focused;
            var parent = host.Container;
            if (webOptions is not null)
            {
                _web = new AndroidWebViewSession(parent.Context!, handle, webOptions,
                    content ?? throw new InvalidOperationException("WebView content was not prepared."), value => WebViewChanged?.Invoke(value));
                _control = _web.View;
            }
            else
            {
                TextView control = editor ? new EditText(parent.Context!) : new Android.Widget.Button(parent.Context!);
                control.Text = text; _control = control;
            }
            Clip = new(parent.Context!) { Visibility = ViewStates.Invisible };
            Clip.SetClipChildren(true);
            _control.ContentDescription = $"doroti-platform-view-{handle.InstanceId}-{handle.InstanceGeneration}";
            _control.FocusableInTouchMode = editor;
            _control.FocusChange += FocusChanged;
            _control.KeyPress += KeyPressed;
            _control.Touch += NativeTouch;
            if (_control is TextView textControl) textControl.TextChanged += TextChanged;
            if (!editor && webOptions is null) _control.Click += Click;
            Clip.AddView(_control); parent.AddView(Clip);
            Android.Util.Log.Info("DorotiPlatformView", $"create handle={handle} native={_control.Handle}");
        }
        private void Click(object? sender, EventArgs e)
        {
            if (!_disabled && !_disposed && _control is TextView textControl) { textControl.Text = $"Native clicks: {++_clicks}";
                Android.Util.Log.Info("DorotiPlatformView", $"click handle={_handle} count={_clicks}"); }
        }
        private void KeyPressed(object? sender, NativeView.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.KeyCode == Keycode.Tab && e.Event is { } key)
                e.Handled = _host._surface?.DispatchKeyEvent(key) == true;
        }
        private void NativeTouch(object? sender, NativeView.TouchEventArgs e)
        {
            e.Handled = false;
            if (e.Event?.ActionMasked is MotionEventActions.Down or MotionEventActions.Up)
                Android.Util.Log.Info("DorotiPlatformView", $"touch handle={_handle} action={e.Event.ActionMasked}");
        }
        private void TextChanged(object? sender, Android.Text.TextChangedEventArgs e) => _host.InvalidateBackdrop();
        private void FocusChanged(object? sender, NativeView.FocusChangeEventArgs e)
        {
            Android.Util.Log.Info("DorotiPlatformView", $"focus handle={_handle} focused={e.HasFocus}");
            if (!e.HasFocus || _disabled || _disposed) return;
            try { _host.YieldTextFocus(); _focused(_handle); }
            catch (Exception error) { Android.Util.Log.Error("DorotiPlatformView", error.ToString()); }
        }
        internal void Validate(PlatformViewPlacement placement)
        {
            placement.Validate(); ValidateTransform(placement.Transform);
            if (_disposed || _disabled || placement.Handle != _handle || Clip.Parent != _host.Container)
                throw new InvalidOperationException("Stale or reparented Android PlatformView.");
        }
        public ValueTask ApplyAsync(PlatformViewPlacement placement)
        {
            AndroidPlatformViewDispatcher.VerifyThread(); Validate(placement);
            var bounds = Map(placement.Bounds, placement.Transform, null);
            var visible = placement.Clip is { } clip ? bounds.intersect(clip) : bounds;
            var sx = _host._scaleX; var sy = _host._scaleY;
            visible = visible.intersect(Rect.fromLTWH(0, 0, _host.Container.Width / sx, _host.Container.Height / sy));
            if (!placement.Visible || visible.isEmpty || visible.width * sx < 1 || visible.height * sy < 1)
                return DetachAsync();
            Place(Clip, visible, sx, sy);
            Place(_control, Rect.fromLTWH(bounds.left - Math.Ceiling(visible.left * sx) / sx,
                bounds.top - Math.Ceiling(visible.top * sy) / sy, bounds.width, bounds.height), sx, sy);
            Clip.Visibility = ViewStates.Visible;
            return ValueTask.CompletedTask;
        }
        private void ReleaseFocus()
        {
            if (!_control.HasFocus) return;
            _control.ClearFocus(); _host._surface?.RequestFocus();
            var ime = _control.Context?.GetSystemService(Context.InputMethodService) as Android.Views.InputMethods.InputMethodManager;
            ime?.HideSoftInputFromWindow(_control.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);
        }
        public ValueTask DetachAsync()
        { AndroidPlatformViewDispatcher.VerifyThread(); ReleaseFocus(); Clip.Visibility = ViewStates.Invisible; return ValueTask.CompletedTask; }
        public ValueTask SetFocusAsync(bool focused)
        {
            AndroidPlatformViewDispatcher.VerifyThread();
            if (focused && !_disabled && !_disposed && Clip.Visibility == ViewStates.Visible) _control.RequestFocus();
            else if (!focused) ReleaseFocus();
            return ValueTask.CompletedTask;
        }
        public ValueTask DisableInputAsync()
        { AndroidPlatformViewDispatcher.VerifyThread(); _disabled = true; _web?.CloseCommands(); _control.Enabled = false; return DetachAsync(); }
        public async ValueTask DisposeAsync()
        {
            AndroidPlatformViewDispatcher.VerifyThread();
            if (_disposed) return;
            ReleaseFocus(); _disposed = true;
            _control.FocusChange -= FocusChanged; _control.Click -= Click; _control.KeyPress -= KeyPressed; _control.Touch -= NativeTouch;
            if (_control is TextView textControl) textControl.TextChanged -= TextChanged;
            (Clip.Parent as ViewGroup)?.RemoveView(Clip);
            Clip.RemoveView(_control); WebViewChanged = null;
            try { if (_web is not null) await _web.DisposeAsync(); }
            finally { _control.Dispose(); Clip.Dispose(); _host._instances.Remove(_handle); }
            Android.Util.Log.Info("DorotiPlatformView", $"dispose handle={_handle}");
        }
    }
    private void YieldTextFocus() => textInput.YieldAndroidNativeFocus();
    private void InvalidateBackdrop()
    { if (OperatingSystem.IsAndroidVersionAtLeast(31)) _backdrop?.Invalidate(); }
    public void Dispose()
    {
        if (_disposed) return;
        Finish(false); _disposed = true;
        if (_session is { } session) CompleteNow(session.DisposeAsync());
        owner.PlatformViews = null;
        owner.GpuResourcesReleasing -= InvalidateRasterCache;
        _coordinator?.Dispose();
        if (OperatingSystem.IsAndroidVersionAtLeast(31) && _backdrop is { } backdrop)
        { _container?.RemoveView(backdrop); backdrop.Dispose(); _backdrop = null; }
        foreach (var raster in _rasters) { _container?.RemoveView(raster); raster.Dispose(); }
        foreach (var shield in _shields) { _container?.RemoveView(shield); shield.Dispose(); }
        _rasters.Clear(); _shields.Clear(); _visible = [];
    }
}
#endif
