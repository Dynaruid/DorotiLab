using System.Runtime.InteropServices;
using System.Text;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using Microsoft.Web.WebView2.Core;
using SkiaSharp;
using C = Windows.UI.Composition;

namespace Doroti.Host.WindowsAppSdk;

internal sealed record WindowsCompositionSlice(PlatformRasterSegment Segment, SKRectI Bounds, int AtlasY);

/// <summary>Owner-local WebView2 visual attachment. All native pixels and inline effects share
/// one Windows.UI.Composition tree; legacy HWND controls cannot enter this tree.</summary>
internal sealed class WindowsWebViewComposition : IPlatformViewFactory, IDisposable
{
    internal static readonly PlatformEffectSupport Effects = new(true, 4, 32,
        Reason: "Windows Composition supports at most four bounded isotropic backdrops, sigma <= 32.");
    private readonly nint _parent;
    private readonly Action _invalidate;
    private readonly Action _yieldText;
    private readonly bool _transportAvailable;
    private readonly System.Collections.Concurrent.ConcurrentDictionary<PlatformViewHandle, Instance> _instances = [];
    private readonly Dictionary<int, (double Sigma, C.SpriteVisual Visual, C.CompositionEffectBrush Brush)> _effects = [];
    private C.Compositor? _compositor;
    private C.ContainerVisual? _root;
    private Windows.UI.Composition.Desktop.DesktopWindowTarget? _target;
    private Windows.System.DispatcherQueueController? _queue;
    private Task<CoreWebView2Environment>? _environment;
    private nint _graphics;
    private readonly List<IDisposable> _rasterResources = [];
    private PlatformCompositionPart[] _visible = [];
    private C.Visual[] _orderedVisuals = [];
    private PlatformViewHandle? _capture;
    private PlatformViewHandle? _hover;
    private PlatformCompositionToken _token;
    private readonly Native.SubclassProc _callback;
    private bool _disposed;
    private long _nativeMessages, _mouseEvents;
    internal object Evidence => new { strategy = "WebView2-CompositionController", apiFamily = "Windows.UI.Composition",
        liveInstances = _instances.Count, effects = _effects.Count, nativeMessages = _nativeMessages, mouseEvents = _mouseEvents,
        loadedViews = _instances.Values.Count(instance => instance.Loaded),
        navigation = _instances.Values.Select(instance => instance.NavigationStatus).ToArray(),
        nativeContentCaptured = false, observation = "BackendAccepted" };
    internal WindowsWebViewComposition(nint parent, Action invalidate, Action yieldText, bool transportAvailable)
    { _parent = parent; _invalidate = invalidate; _yieldText = yieldText; _transportAvailable = transportAvailable; _callback = WindowProc; }
    public string ViewType => "doroti/webview";
    internal bool Contains(PlatformViewHandle handle) => _instances.ContainsKey(handle);
    internal bool HasVisibleContent => _orderedVisuals.Length != 0;
    public PlatformViewSupport QuerySupport(PlatformViewRequest request) => new("Windows-WebView2-Composition",
        Environment.OSVersion.VersionString, ViewType,
        _transportAvailable && !_disposed && request.ViewType == ViewType && request.Composition == PlatformViewComposition.InterleavedComposition &&
        (request.Effects & ~PlatformViewEffects.RectClip) == 0,
        PlatformViewComposition.InterleavedComposition, PlatformViewEffects.RectClip,
        NativeBackdropBlur: _transportAvailable && !_disposed, Capabilities: new(PlatformViewRepresentation.CompositionVisual,
            PlatformViewTransport.CpuUpload, PlatformViewInputPolicy.DirectNative, Effects),
        Reason: "Requires installed WebView2 runtime and a Windows.UI.Composition tree; legacy HWND mixing is unsupported.");

    private void Initialize()
    {
        if (!_transportAvailable) throw new NotSupportedException("WebView composition requires the Graphite/Vulkan raster transport.");
        if (_compositor is not null) return;
        if (Windows.System.DispatcherQueue.GetForCurrentThread() is null)
        {
            var options = new Native.QueueOptions { Size = 12, ThreadType = 2, ApartmentType = 2 };
            Marshal.ThrowExceptionForHR(Native.CreateDispatcherQueueController(options, out var queue));
            try { _queue = WinRT.MarshalInterface<Windows.System.DispatcherQueueController>.FromAbi(queue); }
            finally { Marshal.Release(queue); }
        }
        _compositor = new();
        _root = _compositor.CreateContainerVisual();
        _target = SystemDesktopCompositionInterop.CreateDesktopWindowTarget(_compositor, _parent, true);
        _target.Root = _root;
        Marshal.ThrowExceptionForHR(Native.CreateGraphics(Abi(_compositor), out _graphics));
        if (!Native.SetWindowSubclass(_parent, _callback, 0x505657, 0))
            throw new System.ComponentModel.Win32Exception();
    }
    private static nint Abi(WinRT.IWinRTObject value) => value.NativeObject.ThisPtr;

    public async ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
        Action<PlatformViewHandle> onFocused, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Initialize();
        var folder = Environment.GetEnvironmentVariable("DOROTI_WEBVIEW_USER_DATA") ??
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Doroti", "WebView2");
        _environment ??= CoreWebView2Environment.CreateWithOptionsAsync(null, folder, null).AsTask();
        var environment = await _environment;
        cancellationToken.ThrowIfCancellationRequested();
        var controller = await environment.CreateCoreWebView2CompositionControllerAsync(CoreWebView2ControllerWindowReference.CreateFromWindowHandle((ulong)_parent));
        Instance? instance = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            instance = new(this, handle, environment, controller, _compositor!.CreateContainerVisual(), onFocused);
            if (!_instances.TryAdd(handle, instance)) throw new InvalidOperationException("Duplicate WebView identity.");
            controller.RootVisualTarget = instance.Visual;
            controller.IsVisible = false;
            instance.Core.Settings.IsWebMessageEnabled = true;
            instance.Core.Settings.IsScriptEnabled = true;
            // WebView2 installs its own parent hooks while creating the controller.
            // Install input routing last so the application owns explicit hit testing.
            Native.RemoveWindowSubclass(_parent, _callback, 0x505657);
            if (!Native.SetWindowSubclass(_parent, _callback, 0x505657, 0)) throw new System.ComponentModel.Win32Exception();
            instance.InitialHtml = parameters.IsEmpty ? "<!doctype html><input value='Native WebView2'>" : Encoding.UTF8.GetString(parameters.Span);
            return instance;
        }
        catch
        {
            if (instance is not null) await instance.DisposeAsync(); else controller.Close();
            throw;
        }
    }

    internal unsafe void Commit(PlatformCompositionPlan plan, SkiaGraphiteReadback? pixels, WindowsCompositionSlice[] slices,
        IPlatformViewPlacementBatch batch)
    {
        Initialize();
        var next = plan.Parts.ToArray();
        if (next.OfType<PlatformNativeSegment>().Any(p => !Contains(p.Placement.Handle)))
            throw new NotSupportedException("HWND controls and WebView2 composition visuals require separate owner scenes.");
        var prepared = new List<(C.SpriteVisual Visual, C.CompositionSurfaceBrush Brush, C.CompositionDrawingSurface Surface)>();
        var visuals = new Dictionary<int, C.SpriteVisual>();
        var nextEffects = new Dictionary<int, (double Sigma, C.SpriteVisual Visual, C.CompositionEffectBrush Brush)>();
        var allocatedEffects = new List<(C.SpriteVisual Visual, C.CompositionEffectBrush Brush)>();
        var mutationStarted = false;
        try
        {
            foreach (var slice in slices)
            {
                if (pixels is null) break;
                nint surface;
                fixed (byte* data = pixels.Pixels)
                    Marshal.ThrowExceptionForHR(Native.CreateSurface(_graphics, (nint)(data + slice.AtlasY * pixels.RowBytes),
                        (uint)slice.Bounds.Width, (uint)slice.Bounds.Height, (uint)pixels.RowBytes, out surface));
                C.CompositionDrawingSurface drawing;
                try { drawing = WinRT.MarshalInterface<C.CompositionDrawingSurface>.FromAbi(surface); }
                finally { Marshal.Release(surface); }
                var brush = _compositor!.CreateSurfaceBrush(drawing);
                var visual = _compositor.CreateSpriteVisual();
                visual.Brush = brush; visual.Size = new(slice.Bounds.Width, slice.Bounds.Height);
                visual.Offset = new(slice.Bounds.Left, slice.Bounds.Top, 0);
                prepared.Add((visual, brush, drawing)); visuals.Add(slice.Segment.PaintOrder, visual);
            }
            // Allocate every effect before touching the currently displayed tree.
            foreach (var effect in next.OfType<PlatformBackdropSegment>())
            {
                if (effect.SigmaX != effect.SigmaY || plan.Token.DeviceScaleX != plan.Token.DeviceScaleY)
                    throw new NotSupportedException("Windows Composition requires isotropic backdrop sigma and device scale.");
                var sigma = effect.SigmaX * plan.Token.DeviceScaleX;
                if (_effects.TryGetValue(effect.PaintOrder, out var cached) && cached.Sigma == sigma)
                { nextEffects.Add(effect.PaintOrder, cached); continue; }
                Marshal.ThrowExceptionForHR(Native.CreateEffect(Abi(_compositor!), (float)sigma, out var value));
                C.CompositionEffectBrush brush;
                try { brush = WinRT.MarshalInterface<C.CompositionEffectBrush>.FromAbi(value); }
                finally { Marshal.Release(value); }
                var visual = _compositor!.CreateSpriteVisual(); visual.Brush = brush;
                allocatedEffects.Add((visual, brush));
                nextEffects.Add(effect.PaintOrder, (sigma, visual, brush));
            }
            mutationStarted = true;
            foreach (var native in next.OfType<PlatformNativeSegment>()) Complete(batch.AttachAsync(native.Placement));
            var nextHandles = next.OfType<PlatformNativeSegment>().Select(p => p.Placement.Handle).ToHashSet();
            foreach (var old in _visible.OfType<PlatformNativeSegment>())
                if (!nextHandles.Contains(old.Placement.Handle) && batch.Contains(old.Placement.Handle))
                    Complete(batch.DetachAsync(old.Placement.Handle));
            var ordered = new List<C.Visual>();
            _root!.Children.RemoveAll();
            foreach (var part in next)
            {
                switch (part)
                {
                    case PlatformRasterSegment raster when visuals.TryGetValue(raster.PaintOrder, out var rasterVisual):
                        _root.Children.InsertAtTop(rasterVisual); ordered.Add(rasterVisual); break;
                    case PlatformNativeSegment native:
                        var nativeVisual = _instances[native.Placement.Handle].Visual;
                        _root.Children.InsertAtTop(nativeVisual); ordered.Add(nativeVisual); break;
                    case PlatformBackdropSegment effect:
                        var visual = nextEffects[effect.PaintOrder].Visual;
                        visual.Offset = new((float)(effect.Bounds.left * plan.Token.DeviceScaleX), (float)(effect.Bounds.top * plan.Token.DeviceScaleY), 0);
                        visual.Size = new((float)(effect.Bounds.width * plan.Token.DeviceScaleX), (float)(effect.Bounds.height * plan.Token.DeviceScaleY));
                        _root.Children.InsertAtTop(visual); ordered.Add(visual); break;
                }
            }
            foreach (var resource in _rasterResources) resource.Dispose();
            _rasterResources.Clear();
            foreach (var item in prepared) { _rasterResources.Add(item.Visual); _rasterResources.Add(item.Brush); _rasterResources.Add(item.Surface); }
            prepared.Clear();
            foreach (var old in _effects.Values)
                if (!nextEffects.Values.Any(value => ReferenceEquals(value.Visual, old.Visual)))
                { old.Visual.Dispose(); old.Brush.Dispose(); }
            _effects.Clear(); foreach (var item in nextEffects) _effects.Add(item.Key, item.Value);
            allocatedEffects.Clear();
            _visible = next; _token = plan.Token; _orderedVisuals = ordered.ToArray();
        }
        catch
        {
            if (mutationStarted)
            {
                var previousHandles = _visible.OfType<PlatformNativeSegment>().Select(p => p.Placement.Handle).ToHashSet();
                foreach (var native in next.OfType<PlatformNativeSegment>())
                    if (!previousHandles.Contains(native.Placement.Handle)) Complete(batch.DetachAsync(native.Placement.Handle));
                foreach (var native in _visible.OfType<PlatformNativeSegment>())
                    if (batch.Contains(native.Placement.Handle)) Complete(batch.AttachAsync(native.Placement));
                foreach (var effect in _visible.OfType<PlatformBackdropSegment>())
                {
                    var visual = _effects[effect.PaintOrder].Visual;
                    visual.Offset = new((float)(effect.Bounds.left * _token.DeviceScaleX), (float)(effect.Bounds.top * _token.DeviceScaleY), 0);
                    visual.Size = new((float)(effect.Bounds.width * _token.DeviceScaleX), (float)(effect.Bounds.height * _token.DeviceScaleY));
                }
                _root!.Children.RemoveAll(); foreach (var visual in _orderedVisuals) _root.Children.InsertAtTop(visual);
            }
            throw;
        }
        finally
        {
            foreach (var item in prepared) { item.Visual.Dispose(); item.Brush.Dispose(); item.Surface.Dispose(); }
            foreach (var item in allocatedEffects) { item.Visual.Dispose(); item.Brush.Dispose(); }
        }
    }

    private static void Complete(ValueTask task)
    {
        if (!task.IsCompleted) throw new InvalidOperationException("Reserved native operation must complete on the UI thread.");
        task.GetAwaiter().GetResult();
    }
    internal void Clear(IPlatformViewPlacementBatch batch)
    {
        foreach (var native in _visible.OfType<PlatformNativeSegment>())
            if (batch.Contains(native.Placement.Handle)) Complete(batch.DetachAsync(native.Placement.Handle));
        if (_capture is not null) Native.ReleaseCapture();
        _capture = null; _hover = null;
        _root?.Children.RemoveAll();
        foreach (var resource in _rasterResources) resource.Dispose();
        _rasterResources.Clear();
        foreach (var effect in _effects.Values) { effect.Visual.Dispose(); effect.Brush.Dispose(); }
        _effects.Clear(); _visible = []; _orderedVisuals = [];
    }
    private nint WindowProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint id, nuint data)
    {
        if (message == 0x2a3 || message == 0x215)
        {
            if (_hover is { } previous && _instances.TryGetValue(previous, out var hovered))
                hovered.Controller.SendMouseInput(CoreWebView2MouseEventKind.Leave, 0, 0, default);
            _hover = null;
        }
        if (message == 0x20 && _hover is { } cursorOwner && _instances.TryGetValue(cursorOwner, out var cursorView))
        {
            var cursor = cursorView.Controller.Cursor.Type switch
            {
                Windows.UI.Core.CoreCursorType.IBeam => 32513,
                Windows.UI.Core.CoreCursorType.Hand => 32649,
                Windows.UI.Core.CoreCursorType.Cross => 32515,
                Windows.UI.Core.CoreCursorType.Wait => 32514,
                Windows.UI.Core.CoreCursorType.SizeAll => 32646,
                Windows.UI.Core.CoreCursorType.SizeNorthSouth => 32645,
                Windows.UI.Core.CoreCursorType.SizeWestEast => 32644,
                _ => 32512,
            };
            Native.SetCursor(Native.LoadCursorW(0, cursor)); return 1;
        }
        if (message is >= 0x200 and <= 0x20e && HasVisibleContent)
        {
            var point = new Native.Point { X = (short)(lparam.ToInt64() & 0xffff), Y = (short)(lparam.ToInt64() >> 16) };
            if (message is 0x20a or 0x20e) Native.ScreenToClient(hwnd, ref point);
            var logical = new Offset(point.X / _token.DeviceScaleX, point.Y / _token.DeviceScaleY);
            var native = _visible.OfType<PlatformNativeSegment>().LastOrDefault(p => Bounds(p.Placement).contains(logical) &&
                !_visible.OfType<PlatformShieldSegment>().Any(s => s.PaintOrder > p.PaintOrder && ShieldBounds(s.Shield).contains(logical)));
            var handle = _capture ?? native?.Placement.Handle;
            if (message == 0x200 && handle != _hover)
            {
                if (_hover is { } previous && _instances.TryGetValue(previous, out var hovered))
                    hovered.Controller.SendMouseInput(CoreWebView2MouseEventKind.Leave, 0, 0, default);
                _hover = handle;
                var tracking = new Native.TrackMouse { Size = (uint)Marshal.SizeOf<Native.TrackMouse>(), Flags = 2, Window = hwnd };
                Native.TrackMouseEvent(ref tracking);
            }
            if (handle is { } target && _instances.TryGetValue(target, out var instance) && instance.Placement is { } placement)
            {
                if (message is 0x201 or 0x204 or 0x207)
                {
                    _capture = target; Native.SetCapture(hwnd);
                    _yieldText();
                    instance.FocusNative();
                }
                var origin = placement.Transform.Map(placement.Bounds.topLeft);
                var local = new Windows.Foundation.Point(point.X - (int)Math.Round(origin.dx * _token.DeviceScaleX),
                    point.Y - (int)Math.Round(origin.dy * _token.DeviceScaleY));
                instance.Controller.SendMouseInput((CoreWebView2MouseEventKind)message,
                    (CoreWebView2MouseEventVirtualKeys)(wparam & 0xffff),
                    message is 0x20a or 0x20e ? (uint)(short)(wparam >> 16) : 0, local);
                _mouseEvents++;
                if (message is 0x202 or 0x205 or 0x208)
                {
                    _capture = null; Native.ReleaseCapture();
                }
                return 0;
            }
        }
        if (message == 0x215) _capture = null;
        return Native.DefSubclassProc(hwnd, message, wparam, lparam);
    }
    private static Rect Bounds(PlatformViewPlacement placement)
    {
        var a = placement.Transform.Map(placement.Bounds.topLeft);
        var bounds = Rect.fromLTWH(a.dx, a.dy, placement.Bounds.width, placement.Bounds.height);
        return placement.Clip is { } clip ? bounds.intersect(clip) : bounds;
    }
    private static Rect ShieldBounds(PlatformInputShield shield) => Bounds(new(default, shield.Bounds, shield.Transform, shield.Clip, shield.PaintOrder));
    private sealed class Instance : IPlatformViewInstance
    {
        private readonly WindowsWebViewComposition _owner;
        private readonly PlatformViewHandle _handle;
        private readonly Action<PlatformViewHandle> _focused;
        internal readonly CoreWebView2CompositionController Controller;
        // WinRT event callbacks require environment and core wrappers to outlive the attachment.
        private readonly CoreWebView2Environment _environment;
        internal readonly CoreWebView2 Core;
        internal readonly C.ContainerVisual Visual;
        internal PlatformViewPlacement? Placement;
        private bool _disposed;
        private bool _hasFocus;
        internal bool Loaded { get; private set; }
        internal string NavigationStatus { get; private set; } = "Pending";
        internal string? InitialHtml;
        private double _scale;
        internal Instance(WindowsWebViewComposition owner, PlatformViewHandle handle,
            CoreWebView2Environment environment, CoreWebView2CompositionController controller, C.ContainerVisual visual, Action<PlatformViewHandle> focused)
        {
            _owner = owner; _handle = handle; _environment = environment; Controller = controller; Core = controller.CoreWebView2; Visual = visual; _focused = focused;
            controller.GotFocus += Focused; controller.LostFocus += LostFocus; Core.NavigationCompleted += Navigated;
            Core.WebMessageReceived += MessageReceived;
        }
        private void Focused(object? sender, object args)
        { _hasFocus = true; _focused(_handle); }
        private void LostFocus(object? sender, object args) { _hasFocus = false; }
        internal void FocusNative()
        { if (!_hasFocus) { _owner._yieldText(); Controller.MoveFocus(CoreWebView2MoveFocusReason.Programmatic); } }
        private void Navigated(object? sender, CoreWebView2NavigationCompletedEventArgs args)
        { Loaded = args.IsSuccess; NavigationStatus = args.IsSuccess ? "Loaded" : args.WebErrorStatus.ToString(); _owner._invalidate(); }
        private void MessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs args)
        { _owner._nativeMessages++; _owner._invalidate(); }
        public ValueTask ApplyAsync(PlatformViewPlacement placement)
        {
            var scale = Native.GetDpiForWindow(_owner._parent) / 96d;
            if (Placement == placement && scale == _scale) return ValueTask.CompletedTask;
            _scale = scale;
            var origin = placement.Transform.Map(placement.Bounds.topLeft);
            Controller.Bounds = new(0, 0, (int)Math.Round(placement.Bounds.width * scale), (int)Math.Round(placement.Bounds.height * scale));
            Visual.Offset = new((float)(origin.dx * scale), (float)(origin.dy * scale), 0);
            var bounds = Bounds(placement);
            var previousClip = Visual.Clip;
            Visual.Clip = _owner._compositor!.CreateInsetClip((float)((bounds.left - origin.dx) * scale),
                (float)((bounds.top - origin.dy) * scale), (float)((origin.dx + placement.Bounds.width - bounds.right) * scale),
                (float)((origin.dy + placement.Bounds.height - bounds.bottom) * scale));
            previousClip?.Dispose();
            Visual.Size = new((float)Controller.Bounds.Width, (float)Controller.Bounds.Height);
            Controller.IsVisible = placement.Visible && !bounds.isEmpty; Placement = placement;
            if (Controller.IsVisible && InitialHtml is { } html)
            {
                InitialHtml = null;
                Core.NavigateToString(html);
            }
            return ValueTask.CompletedTask;
        }
        public ValueTask DetachAsync()
        {
            // Closing the parent HWND can close WebView2 before coordinator retirement drains.
            try { if (!_disposed) Controller.IsVisible = false; }
            catch (COMException error) when (error.HResult == unchecked((int)0x8007139f)) { }
            Placement = null; return ValueTask.CompletedTask;
        }
        public ValueTask DisableInputAsync() => DetachAsync();
        public ValueTask SetFocusAsync(bool focused)
        {
            if (focused && Placement is { Visible: true }) FocusNative();
            else if (!focused && _hasFocus) Native.SetFocus(_owner._parent);
            return ValueTask.CompletedTask;
        }
        public ValueTask DisposeAsync()
        {
            if (_disposed) return ValueTask.CompletedTask;
            _disposed = true; _owner._instances.TryRemove(_handle, out _);
            Controller.GotFocus -= Focused; Controller.LostFocus -= LostFocus; Core.NavigationCompleted -= Navigated;
            Core.WebMessageReceived -= MessageReceived;
            try { Controller.RootVisualTarget = null; Controller.Close(); }
            catch (COMException error) when (error.HResult == unchecked((int)0x8007139f)) { }
            finally { Visual.Clip?.Dispose(); Visual.Dispose(); GC.KeepAlive(Core); GC.KeepAlive(_environment); }
            return ValueTask.CompletedTask;
        }
    }
    public void Dispose()
    {
        if (_disposed) return; _disposed = true;
        Native.RemoveWindowSubclass(_parent, _callback, 0x505657);
        foreach (var instance in _instances.Values.ToArray()) Complete(instance.DisposeAsync());
        if (_target is not null) _target.Root = null;
        foreach (var resource in _rasterResources) resource.Dispose();
        foreach (var effect in _effects.Values) { effect.Visual.Dispose(); effect.Brush.Dispose(); }
        _root?.Dispose(); _target?.Dispose();
        if (_graphics != 0) Native.DestroyGraphics(_graphics);
        _compositor?.Dispose();
        if (_queue is not null) _ = _queue.ShutdownQueueAsync();
    }
    private static class Native
    {
        [StructLayout(LayoutKind.Sequential)] internal struct QueueOptions { internal uint Size, ThreadType, ApartmentType; }
        [StructLayout(LayoutKind.Sequential)] internal struct Point { internal int X, Y; }
        [StructLayout(LayoutKind.Sequential)] internal struct TrackMouse { internal uint Size, Flags; internal nint Window; internal uint Hover; }
        internal delegate nint SubclassProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint id, nuint data);
        [DllImport("CoreMessaging.dll")] internal static extern int CreateDispatcherQueueController(QueueOptions options, out nint controller);
        [DllImport("comctl32.dll")] internal static extern bool SetWindowSubclass(nint hwnd, SubclassProc callback, nuint id, nuint data);
        [DllImport("comctl32.dll")] internal static extern bool RemoveWindowSubclass(nint hwnd, SubclassProc callback, nuint id);
        [DllImport("comctl32.dll")] internal static extern nint DefSubclassProc(nint hwnd, uint message, nuint wparam, nint lparam);
        [DllImport("user32.dll")] internal static extern bool ScreenToClient(nint hwnd, ref Point point);
        [DllImport("user32.dll")] internal static extern uint GetDpiForWindow(nint hwnd);
        [DllImport("user32.dll")] internal static extern nint SetCapture(nint hwnd);
        [DllImport("user32.dll")] internal static extern bool ReleaseCapture();
        [DllImport("user32.dll")] internal static extern nint SetFocus(nint hwnd);
        [DllImport("user32.dll")] internal static extern nint LoadCursorW(nint module, nint id);
        [DllImport("user32.dll")] internal static extern nint SetCursor(nint cursor);
        [DllImport("user32.dll")] internal static extern bool TrackMouseEvent(ref TrackMouse tracking);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_platform_effect_v1")]
        internal static extern int CreateEffect(nint compositor, float sigma, out nint brush);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_platform_graphics_create_v1")]
        internal static extern int CreateGraphics(nint compositor, out nint context);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_platform_surface_v1")]
        internal static extern int CreateSurface(nint context, nint pixels, uint width, uint height, uint stride, out nint surface);
        [DllImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_platform_graphics_destroy_v1")]
        internal static extern void DestroyGraphics(nint context);
    }
}
