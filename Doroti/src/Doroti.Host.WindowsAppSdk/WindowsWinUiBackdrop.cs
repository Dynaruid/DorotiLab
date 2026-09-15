using System.Numerics;
using System.Runtime.InteropServices;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Content;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Samples live XAML visuals and the preceding Doroti raster layers on the
/// WinUI compositor. Each effect samples only its prefix, so it cannot feed back
/// into itself or include a foreground layer. Native HWNDs remain the input owners.</summary>
internal sealed class WindowsWinUiBackdrop : IDisposable
{
    internal static readonly PlatformEffectSupport Support = new(true, 4, 32,
        Reason: "WinUI live VisualSurface sampling supports four bounded isotropic backdrops, logical sigma <= 32 and physical sigma <= 128.");
    private readonly WindowsWinUiControls _controls;
    private readonly nint _parent;
    private Compositor? _compositor;
    private nint _graphics;
    private readonly Dictionary<int, RasterSource> _rasters = [];
    private readonly Dictionary<nint, NativeSource> _native = [];
    private readonly Dictionary<int, EffectLayer> _effects = [];
    private readonly List<IDisposable> _retired = [];
    private long _uploads;
    private long _reuses;
    private long _commits;
    // Negative control for the opt-in pixel test; never enabled in ordinary runs.
    private readonly bool _omitNativeProbe = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_PLATFORM_CAPTURE") == "1" &&
        Environment.GetEnvironmentVariable("DOROTI_WINUI_BACKDROP_OMIT_NATIVE_PROBE") == "1";
    internal WindowsWinUiBackdrop(WindowsWinUiControls controls, nint parent) { _controls = controls; _parent = parent; }
    internal nint WindowFor(int order) => _effects[order].Hwnd;
    internal object Evidence => new
    {
        strategy = "WinUI CompositionVisualSurface live prefix sampling", effects = _effects.Count,
        nativeSources = _native.Count, nativeReadbackBytes = 0, rasterUploads = _uploads, rasterReuses = _reuses,
        omittedNativeProbe = _omitNativeProbe,
        commits = _commits, input = "direct native HWND; effect bridges input-transparent",
        bounds = _effects.Select(pair => new { order = pair.Key, sigma = pair.Value.Sigma,
            area = Coordinates(pair.Value.Area), clip = Coordinates(pair.Value.Bounds) }).ToArray(),
    };
    private static double[] Coordinates(Rect bounds) => [bounds.left, bounds.top, bounds.right, bounds.bottom];

    internal unsafe void Prepare(PlatformCompositionPlan plan, SkiaGraphiteReadback pixels,
        WindowsCompositionSlice[] slices, int width, int height, Func<PlatformViewHandle, nint> getWindow)
    {
        var effects = plan.Parts.OfType<PlatformBackdropSegment>().ToArray();
        if (effects.Length == 0) { Clear(); return; }
        foreach (var effect in effects)
        {
            Support.Validate(effect.SigmaX, effect.SigmaY, effects.Length);
            if (effect.SigmaX != effect.SigmaY || plan.Token.DeviceScaleX != plan.Token.DeviceScaleY ||
                effect.SigmaX * plan.Token.DeviceScaleX > 128)
                throw new NotSupportedException(Support.Reason);
        }
        var nativeParts = plan.Parts.OfType<PlatformNativeSegment>().ToArray();
        if (_compositor is null)
        {
            var first = nativeParts.FirstOrDefault() ?? throw new InvalidOperationException("WinUI backdrop requires a live native source.");
            _compositor = _controls.GetVisual(getWindow(first.Placement.Handle)).Compositor;
            Marshal.ThrowExceptionForHR(Native.CreateGraphics(Abi(_compositor), out _graphics));
        }
        var scale = plan.Token.DeviceScaleX;
        var viewport = Rect.fromLTWH(0, 0, width, height);
        var areas = effects.ToDictionary(effect => effect.PaintOrder, effect =>
            Scale(effect.Bounds, scale).inflate(Math.Ceiling(3 * effect.SigmaX * scale)).intersect(viewport));
        // Source and output surfaces plus current/staging raster uploads are bounded.
        var bytes = areas.Values.Sum(area => area.width * area.height * 16) +
            slices.Sum(slice => (double)slice.Bounds.Width * slice.Bounds.Height * 16);
        if (bytes > 256L * 1024 * 1024) throw new InvalidOperationException("WinUI backdrop staging exceeds 256 MiB.");

        foreach (var slice in slices)
        {
            if (_rasters.TryGetValue(slice.Segment.PaintOrder, out var previous) && previous.Slice.Bounds == slice.Bounds &&
                SkiaPlatformRasterContent.Equivalent(previous.Slice.Segment.Commands, slice.Segment.Commands))
            { _reuses++; continue; }
            nint surface;
            fixed (byte* data = pixels.Pixels)
                Marshal.ThrowExceptionForHR(Native.CreateSurface(_graphics, (nint)(data + slice.AtlasY * pixels.RowBytes),
                    (uint)slice.Bounds.Width, (uint)slice.Bounds.Height, (uint)pixels.RowBytes, out surface));
            CompositionDrawingSurface drawing;
            try { drawing = WinRT.MarshalInterface<CompositionDrawingSurface>.FromAbi(surface); }
            finally { Marshal.Release(surface); }
            var source = new RasterSource(slice, drawing, _compositor.CreateSurfaceBrush(drawing));
            if (_rasters.Remove(slice.Segment.PaintOrder, out var old)) _retired.Add(old);
            _rasters.Add(slice.Segment.PaintOrder, source);
            _uploads++;
        }
        var liveWindows = nativeParts.Select(part => getWindow(part.Placement.Handle)).ToHashSet();
        foreach (var hwnd in _native.Keys.Except(liveWindows).ToArray()) { _retired.Add(_native[hwnd]); _native.Remove(hwnd); }
        foreach (var part in nativeParts)
        {
            var hwnd = getWindow(part.Placement.Handle);
            if (!_native.TryGetValue(hwnd, out var source))
            {
                var visual = _controls.GetVisual(hwnd);
                if (Abi(visual.Compositor) != Abi(_compositor)) throw new InvalidOperationException("WinUI sources must share one compositor.");
                var surface = _compositor.CreateVisualSurface();
                surface.SourceVisual = visual;
                var brush = _compositor.CreateSurfaceBrush(surface);
                brush.Stretch = CompositionStretch.Fill;
                source = new(surface, brush);
                _native.Add(hwnd, source);
            }
            source.Surface.SourceSize = new((float)part.Placement.Bounds.width, (float)part.Placement.Bounds.height);
        }
        var orders = effects.Select(effect => effect.PaintOrder).ToHashSet();
        foreach (var order in _effects.Keys.Except(orders).ToArray()) { _effects[order].Dispose(); _effects.Remove(order); }
        foreach (var effect in effects)
        {
            if (!_effects.TryGetValue(effect.PaintOrder, out var layer))
            { layer = new(_compositor, _parent); _effects.Add(effect.PaintOrder, layer); }
            layer.ResetPrefix();
            foreach (var part in plan.Parts.TakeWhile(part => part.PaintOrder < effect.PaintOrder))
            {
                if (part is PlatformRasterSegment raster && _rasters.TryGetValue(raster.PaintOrder, out var source))
                {
                    var rect = source.Slice.Bounds;
                    layer.Add(source.Brush, Rect.fromLTWH(rect.Left, rect.Top, rect.Width, rect.Height));
                }
                else if (part is PlatformNativeSegment native && native.Placement.Visible && !_omitNativeProbe)
                {
                    var placement = native.Placement;
                    var origin = placement.Transform.Map(placement.Bounds.topLeft);
                    var bounds = Scale(Rect.fromLTWH(origin.dx, origin.dy, placement.Bounds.width, placement.Bounds.height), scale);
                    layer.Add(_native[getWindow(placement.Handle)].Brush, bounds,
                        placement.Clip is { } clip ? Scale(clip, scale).intersect(bounds) : bounds);
                }
                else if (part is PlatformBackdropSegment preceding)
                {
                    var lower = _effects[preceding.PaintOrder];
                    layer.Add(lower.Brush!, lower.Area, lower.Bounds);
                }
            }
            layer.Update(areas[effect.PaintOrder], Scale(effect.Bounds, scale).intersect(viewport), (float)(effect.SigmaX * scale), width, height);
        }
        foreach (var order in _rasters.Keys.Except(slices.Select(slice => slice.Segment.PaintOrder)).ToArray())
        { _rasters[order].Dispose(); _rasters.Remove(order); }
        foreach (var retired in _retired) retired.Dispose();
        _retired.Clear();
        _commits++;
    }

    private static Rect Scale(Rect value, double scale) =>
        new(value.left * scale, value.top * scale, value.right * scale, value.bottom * scale);
    private static nint Abi(WinRT.IWinRTObject value) => value.NativeObject.ThisPtr;
    internal void Clear()
    {
        // Close consumers before the surfaces and XAML visuals they reference.
        foreach (var effect in _effects.Values.Reverse()) effect.Dispose();
        _effects.Clear();
        foreach (var source in _native.Values) source.Dispose();
        _native.Clear();
        foreach (var raster in _rasters.Values) raster.Dispose();
        _rasters.Clear();
        foreach (var retired in _retired) retired.Dispose();
        _retired.Clear();
    }
    public void Dispose()
    {
        Clear();
        if (_graphics != 0) Native.DestroyGraphics(_graphics);
        _graphics = 0;
        _compositor = null; // Owned by XAML, never dispose the shared compositor.
    }

    private sealed record RasterSource(WindowsCompositionSlice Slice, CompositionDrawingSurface Surface,
        CompositionSurfaceBrush Brush) : IDisposable
    { public void Dispose() { Brush.Dispose(); Surface.Dispose(); } }
    private sealed record NativeSource(CompositionVisualSurface Surface, CompositionSurfaceBrush Brush) : IDisposable
    { public void Dispose() { Surface.SourceVisual = null; Brush.Dispose(); Surface.Dispose(); } }

    private sealed class EffectLayer : IDisposable
    {
        private readonly Compositor _compositor;
        private readonly ContainerVisual _prefix;
        private readonly CompositionVisualSurface _sample;
        private readonly CompositionSurfaceBrush _input;
        private readonly SpriteVisual _output;
        private readonly ContainerVisual _root;
        private readonly ContentIsland _island;
        private readonly DesktopChildSiteBridge _bridge;
        private sealed class PrefixNode(SpriteVisual visual)
        {
            internal readonly SpriteVisual Visual = visual;
            internal CompositionBrush? Brush;
            internal Rect? Bounds, ClipBounds;
            internal InsetClip? Clip;
            internal void Dispose() { Visual.Dispose(); Clip?.Dispose(); }
        }
        private readonly List<PrefixNode> _children = [];
        private int _nextChild;
        private Vector2 _viewport;
        private InsetClip? _clip;
        private bool _disposed;
        internal CompositionEffectBrush? Brush { get; private set; }
        internal float Sigma { get; private set; }
        internal Rect Area { get; private set; } = Rect.zero;
        internal Rect Bounds { get; private set; } = Rect.zero;
        internal nint Hwnd => Win32Interop.GetWindowFromWindowId(_bridge.WindowId);
        internal EffectLayer(Compositor compositor, nint parent)
        {
            _compositor = compositor;
            _prefix = compositor.CreateContainerVisual();
            _sample = compositor.CreateVisualSurface(); _sample.SourceVisual = _prefix;
            _input = compositor.CreateSurfaceBrush(_sample);
            _output = compositor.CreateSpriteVisual();
            _root = compositor.CreateContainerVisual(); _root.Children.InsertAtTop(_output);
            // Exclude both the visual subtree and its HWND from input targeting.
            // Disable alone leaves a full-window bridge in the Windows 11 input
            // hit-test path, swallowing mouse/wheel input to raster siblings.
            _root.IsHitTestVisible = false;
            _island = ContentIsland.Create(_root);
            _island.IsHitTestVisibleWhenTransparent = false;
            _island.IsIslandEnabled = false;
            _bridge = DesktopChildSiteBridge.Create(compositor, Win32Interop.GetWindowIdFromWindow(parent));
            _bridge.OverrideScale = 1;
            _bridge.Disable(); // Effects never own native input or focus.
            _bridge.Connect(_island);
            var style = Native.GetWindowLongPtrW(Hwnd, -20);
            if (Native.SetWindowLongPtrW(Hwnd, -20, style | 0x20) == 0 && Marshal.GetLastWin32Error() != 0)
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }
        internal void ResetPrefix()
        {
            // Keep the sampled visual tree mounted across animation frames.
            _nextChild = 0;
        }
        internal void Add(CompositionBrush brush, Rect bounds, Rect? clipping = null)
        {
            if (bounds.isEmpty || clipping is { isEmpty: true }) return;
            if (_nextChild == _children.Count)
            {
                var created = _compositor.CreateSpriteVisual();
                _prefix.Children.InsertAtTop(created);
                _children.Add(new(created));
            }
            var node = _children[_nextChild++];
            if (!ReferenceEquals(node.Brush, brush)) { node.Visual.Brush = brush; node.Brush = brush; }
            if (node.Bounds != bounds)
            {
                node.Visual.Size = new((float)bounds.width, (float)bounds.height);
                node.Visual.Offset = new((float)bounds.left, (float)bounds.top, 0);
            }
            if (node.Bounds != bounds || node.ClipBounds != clipping)
            {
                var inset = clipping is { } clip ? MakeClip(bounds, clip) : null;
                node.Visual.Clip = inset; node.Clip?.Dispose(); node.Clip = inset;
            }
            node.Bounds = bounds; node.ClipBounds = clipping;
        }
        private InsetClip MakeClip(Rect bounds, Rect clip) => _compositor.CreateInsetClip(
            (float)Math.Max(0, clip.left - bounds.left), (float)Math.Max(0, clip.top - bounds.top),
            (float)Math.Max(0, bounds.right - clip.right), (float)Math.Max(0, bounds.bottom - clip.bottom));
        internal void Update(Rect area, Rect bounds, float sigma, int width, int height)
        {
            while (_children.Count > _nextChild)
            {
                var child = _children[^1]; _children.RemoveAt(_children.Count - 1);
                _prefix.Children.Remove(child.Visual); child.Dispose();
            }
            if (Brush is null || Sigma != sigma)
            {
                Marshal.ThrowExceptionForHR(Native.CreateEffect(Abi(_compositor), sigma, out var pointer));
                CompositionEffectBrush brush;
                try { brush = WinRT.MarshalInterface<CompositionEffectBrush>.FromAbi(pointer); }
                finally { Marshal.Release(pointer); }
                brush.SetSourceParameter("backdrop", _input);
                _output.Brush = brush;
                Brush?.Dispose(); Brush = brush; Sigma = sigma;
            }
            if (_viewport != new Vector2(width, height))
                _root.Size = _prefix.Size = _viewport = new(width, height);
            if (Area == area && Bounds == bounds) return;
            Area = area; Bounds = bounds;
            _sample.SourceOffset = new((float)area.left, (float)area.top);
            _sample.SourceSize = new((float)Math.Max(1, area.width), (float)Math.Max(1, area.height));
            _output.Offset = new((float)area.left, (float)area.top, 0);
            _output.Size = new((float)area.width, (float)area.height);
            var clip = MakeClip(area, bounds);
            _output.Clip = clip; _clip?.Dispose(); _clip = clip;
        }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _bridge.Hide();
            // ContentIsland.Close closes its root. Detach the children and
            // release sampled visuals before closing that root's owner.
            _root.Children.RemoveAll();
            _output.Brush = null; _output.Dispose(); _clip?.Dispose();
            Brush?.Dispose(); _input.Dispose(); _sample.SourceVisual = null; _sample.Dispose();
            _prefix.Children.RemoveAll();
            foreach (var child in _children) child.Dispose();
            _children.Clear(); _prefix.Dispose();
            _bridge.Dispose(); _island.Dispose();
        }
    }

    private static class Native
    {
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
        internal static extern nint GetWindowLongPtrW(nint hwnd, int index);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
        internal static extern nint SetWindowLongPtrW(nint hwnd, int index, nint value);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_winui_graphics_create_v1")]
        internal static extern int CreateGraphics(nint compositor, out nint context);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_winui_surface_v1")]
        internal static extern int CreateSurface(nint context, nint pixels, uint width, uint height, uint stride, out nint surface);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_winui_effect_v1")]
        internal static extern int CreateEffect(nint compositor, float sigma, out nint brush);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_winui_graphics_destroy_v1")]
        internal static extern void DestroyGraphics(nint context);
    }
}
