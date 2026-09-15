using System.ComponentModel;
using System.Runtime.InteropServices;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Generic HWND attachment. Standalone hosts use a lower DComp target;
/// the product may opt into live redirected HWND and raster sibling composition.</summary>
public sealed class WindowsHwndPlatformViewFactory : IPlatformViewFactory
{
    private readonly nint _parent;
    private readonly bool _lowerCompositionTarget;
    private readonly bool _editor;
    private readonly uint _uiThread;
    internal bool Interleaved { get; init; }
    internal bool SiblingRasterTopology { get; init; }
    internal bool KeepCompositionSourceAlive { get; init; }
    private bool SupportsNativePlacement => _lowerCompositionTarget || SiblingRasterTopology;
    internal Action<PlatformViewHandle, nint>? Created { get; init; }
    internal Action<PlatformViewHandle>? Destroyed { get; init; }
    internal Func<int, nint, bool>? InterceptsPoint { get; init; }
    internal Action? YieldFrameworkTextInput { get; init; }
    internal Action<PlatformViewHandle, nint, nint>? PointerDown { get; init; }
    internal Action<nint, int, int, int, int, bool>? StagePlacement { get; init; }
    internal Action<nint>? SourceSurfaceChanged { get; init; }
    public WindowsHwndPlatformViewFactory(nint parent, bool editor, bool lowerCompositionTarget)
    {
        _parent = parent;
        _editor = editor;
        _lowerCompositionTarget = lowerCompositionTarget;
        _uiThread = Native.GetCurrentThreadId();
        if (!Native.IsWindow(parent) || Native.GetWindowThreadProcessId(parent, out _) != _uiThread)
            throw new ArgumentException("The native parent must belong to the current UI thread.", nameof(parent));
    }
    public string ViewType => _editor ? "doroti/native-editor" : "doroti/native-button";
    public PlatformViewSupport QuerySupport(PlatformViewRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var supported = request.ViewType == ViewType && SupportsNativePlacement &&
            (request.Composition == PlatformViewComposition.NativeOverlay || Interleaved && request.Composition == PlatformViewComposition.InterleavedComposition) &&
            (request.Effects & ~PlatformViewEffects.RectClip) == 0;
        var backdrop = supported && SiblingRasterTopology && request.Composition == PlatformViewComposition.InterleavedComposition;
        return new("Windows-HWND", Environment.OSVersion.VersionString, ViewType, supported,
            supported ? request.Composition : PlatformViewComposition.NativeOverlay, PlatformViewEffects.RectClip,
            NativeBackdropBlur: backdrop,
            Capabilities: new(PlatformViewRepresentation.NativeHierarchy, PlatformViewTransport.BoundedReadback,
                PlatformViewInputPolicy.DirectNative, backdrop ? WindowsWebViewComposition.Effects : PlatformEffectSupport.Unsupported),
            Reason: supported ? null : SiblingRasterTopology
                ? "The Windows sibling HWND path supports matching view types, B/C composition, translation and rectangular clipping only."
                : "Generic HWND requires an explicit lower DComp target with WS_CLIPCHILDREN; standalone interleaving is not enabled.");
    }
    public ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
        Action<PlatformViewHandle> onFocused, CancellationToken cancellationToken)
    {
        VerifyThread();
        cancellationToken.ThrowIfCancellationRequested();
        if (!SupportsNativePlacement || !Native.IsWindow(_parent) ||
            Native.GetWindowThreadProcessId(_parent, out _) != _uiThread ||
            (Native.GetWindowLongPtrW(_parent, -16).ToInt64() & 0x02000000) == 0)
            throw new InvalidOperationException("Native HWND parent must use WS_CLIPCHILDREN with a lower composition target.");
        var text = parameters.IsEmpty ? (_editor ? "Native editor" : "Native button") : System.Text.Encoding.UTF8.GetString(parameters.Span);
        var hwnd = Native.CreateWindowExW((_editor ? 0x200u : 0u) | (SiblingRasterTopology ? 0x00080000u : 0u), _editor ? "EDIT" : "BUTTON", text,
            0x40000000u | 0x00010000u | (_editor ? 0x00800080u : 0u), 0, 0, 0, 0, _parent, 0, 0, 0);
        if (hwnd == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
        try
        {
            // Give live GDI controls their own redirected surface as well. Without
            // this, USER32 paints them into the parent bitmap below all layered slices.
            if (SiblingRasterTopology && !Native.SetLayeredWindowAttributes(hwnd, 0, 255, 2))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            var instance = new Instance(this, hwnd, handle, onFocused);
            Created?.Invoke(handle, hwnd);
            return ValueTask.FromResult<IPlatformViewInstance>(instance);
        }
        catch { Native.DestroyWindow(hwnd); throw; }
    }
    private void VerifyThread()
    {
        if (Native.GetCurrentThreadId() != _uiThread) throw new InvalidOperationException("HWND attachment operation must run on its owner UI thread.");
    }

    private sealed class Instance : IPlatformViewInstance
    {
        private readonly WindowsHwndPlatformViewFactory _owner;
        private readonly PlatformViewHandle _handle;
        private readonly Action<PlatformViewHandle> _onFocused;
        private readonly Native.SubclassProc _callback;
        private nint _hwnd;
        private bool _inputEnabled = true;
        private bool _visible;
        private int _paintOrder;
        private (int Left, int Top, int Right, int Bottom)? _clipRegion;
        private bool _sourceShown;
        private int _sourceWidth, _sourceHeight;
        public Instance(WindowsHwndPlatformViewFactory owner, nint hwnd, PlatformViewHandle handle, Action<PlatformViewHandle> onFocused)
        {
            _owner = owner; _hwnd = hwnd; _handle = handle; _onFocused = onFocused; _callback = WindowProc;
            if (!Native.SetWindowSubclass(hwnd, _callback, 1, 0)) throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        private nint WindowProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint subclassId, nuint data)
        {
            // Never let managed exceptions unwind through the unmanaged window procedure.
            if (message is 0x100 or 0x101 or 0x104 or 0x105 &&
                (wparam is 9 or 16 or 17 or 18 or 160 or 161 or 162 or 163 or 164 or 165) && _owner.SiblingRasterTopology)
            {
                Native.SendMessageW(_owner._parent, message, wparam, lparam);
                if (wparam == 9) return 0;
            }
            if (message == 0x102 && wparam == 9 && _owner.SiblingRasterTopology) return 0;
            // Keep the OS target stable and route shielded input explicitly once.
            // HTTRANSPARENT alone is insufficient during activation/capture changes.
            if (message is >= 0x200 and <= 0x20e && _owner.InterceptsPoint is { } intercepts)
            {
                try
                {
                    var point = new Native.Point { X = (short)(lparam.ToInt64() & 0xffff), Y = (short)((lparam.ToInt64() >> 16) & 0xffff) };
                    var wheel = message is 0x20a or 0x20e;
                    if (!wheel) Native.ClientToScreen(hwnd, ref point);
                    if (intercepts(_paintOrder, Pack(point)))
                    {
                        if (!wheel) Native.ScreenToClient(_owner._parent, ref point);
                        var forwarded = message switch { 0x203 => 0x201u, 0x206 => 0x204u, 0x209 => 0x207u, _ => message };
                        Native.SendMessageW(_owner._parent, forwarded, wparam, Pack(point));
                        return 0;
                    }
                }
                catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); return 0; }
            }
            if (message is 0x201 or 0x203 && _inputEnabled)
            {
                try
                {
                    var point = new Native.Point { X = (short)(lparam.ToInt64() & 0xffff), Y = (short)((lparam.ToInt64() >> 16) & 0xffff) };
                    Native.ClientToScreen(hwnd, ref point);
                    _owner.PointerDown?.Invoke(_handle, Pack(point), Native.GetMessageExtraInfo());
                }
                catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
            }
            if (message == 0x84)
            {
                try { if (_owner.InterceptsPoint?.Invoke(_paintOrder, lparam) == true) return 1; }
                catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); return 1; }
            }
            if (message == 7 && _inputEnabled && _visible)
            {
                try { _owner.YieldFrameworkTextInput?.Invoke(); _onFocused(_handle); }
                catch (Exception exception) { System.Diagnostics.Trace.TraceError(exception.ToString()); }
            }
            if (message == 0x82)
            {
                Native.RemoveWindowSubclass(hwnd, _callback, 1); _hwnd = 0;
                try { _owner.Destroyed?.Invoke(_handle); }
                catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
            }
            return Native.DefSubclassProc(hwnd, message, wparam, lparam);
        }
        private static nint Pack(Native.Point point) => (nint)((uint)(ushort)point.X | ((uint)(ushort)point.Y << 16));
        public ValueTask ApplyAsync(PlatformViewPlacement placement)
        {
            _owner.VerifyThread(); placement.Validate();
            if (placement.Handle != _handle || !placement.Transform.IsAxisAligned || placement.Transform.M11 != 1 || placement.Transform.M22 != 1)
                throw new InvalidOperationException("HWND only supports translation and rectangular clipping.");
            ObjectDisposedException.ThrowIf(_hwnd == 0, this);
            if (Native.GetParent(_hwnd) != _owner._parent)
                throw new InvalidOperationException("A platform HWND cannot be reparented outside its owner.");
            var parentDpi = Native.GetDpiForWindow(_owner._parent);
            if (parentDpi == 0) throw new InvalidOperationException("Native HWND parent has no valid DPI.");
            var dpi = parentDpi / 96d;
            var origin = placement.Transform.Map(placement.Bounds.topLeft);
            var bounds = Rect.fromLTWH(origin.dx, origin.dy, placement.Bounds.width, placement.Bounds.height);
            int x = Pixel(bounds.left), y = Pixel(bounds.top), right = Pixel(bounds.right), bottom = Pixel(bounds.bottom);
            if (_owner.StagePlacement is null && !Native.SetWindowPos(_hwnd, 0, x, y, right - x, bottom - y, 0x0010 | 0x0004))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            var clip = placement.Clip is { } clipping ? bounds.intersect(clipping) : bounds;
            var clipLeft = clip.isEmpty ? 0 : Math.Max(0, Pixel(clip.left) - x);
            var clipTop = clip.isEmpty ? 0 : Math.Max(0, Pixel(clip.top) - y);
            var clipRight = clip.isEmpty ? 0 : Math.Max(0, Pixel(clip.right) - x);
            var clipBottom = clip.isEmpty ? 0 : Math.Max(0, Pixel(clip.bottom) - y);
            _visible = placement.Visible && !clip.isEmpty && !bounds.isEmpty && right > x && bottom > y &&
                clipRight > clipLeft && clipBottom > clipTop;
            var sourceWidth = Math.Max(1, right - x);
            var sourceHeight = Math.Max(1, bottom - y);
            // The visible composition visual owns clipping. Emptying the HWND
            // region discards pixels needed when that visual reenters the viewport.
            if (_owner.KeepCompositionSourceAlive)
            {
                clipLeft = clipTop = 0;
                clipRight = sourceWidth; clipBottom = sourceHeight;
            }
            var nextClip = (clipLeft, clipTop, clipRight, clipBottom);
            if (_clipRegion != nextClip)
            {
                var region = Native.CreateRectRgn(clipLeft, clipTop, clipRight, clipBottom);
                if (region == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
                if (Native.SetWindowRgn(_hwnd, region, false) == 0) { Native.DeleteObject(region); throw new Win32Exception(Marshal.GetLastWin32Error()); }
                _clipRegion = nextClip;
            }
            if (!_visible) YieldFocus();
            if (_owner.StagePlacement is { } stage && _owner.KeepCompositionSourceAlive)
            {
                // Keep a cloaked, nonzero native source on-screen when culled.
                // Logical visibility still gates focus and committed hit testing.
                if (_visible || !_sourceShown || _sourceWidth != sourceWidth || _sourceHeight != sourceHeight)
                    stage(_hwnd, _visible ? x : 0, _visible ? y : 0, sourceWidth, sourceHeight, true);
                if (!_sourceShown || _sourceWidth != sourceWidth || _sourceHeight != sourceHeight)
                    _owner.SourceSurfaceChanged?.Invoke(_hwnd);
                _sourceShown = true;
                _sourceWidth = sourceWidth; _sourceHeight = sourceHeight;
            }
            else if (_owner.StagePlacement is { } placementStage) placementStage(_hwnd, x, y, right - x, bottom - y, _visible);
            else Native.ShowWindow(_hwnd, _visible ? 4 : 0);
            _paintOrder = placement.PaintOrder;
            return ValueTask.CompletedTask;
            int Pixel(double value) => checked((int)Math.Round(value * dpi));
        }
        private void YieldFocus()
        {
            if (_hwnd != 0 && Native.GetFocus() == _hwnd) Native.SetFocus(_owner._parent);
            if (_hwnd != 0 && Native.GetCapture() == _hwnd) Native.ReleaseCapture();
        }
        public ValueTask DetachAsync()
        {
            _owner.VerifyThread(); _visible = false;
            if (_hwnd != 0)
            {
                YieldFocus();
                if (!_owner.KeepCompositionSourceAlive) Native.ShowWindow(_hwnd, 0);
            }
            return ValueTask.CompletedTask;
        }
        public ValueTask SetFocusAsync(bool focused)
        {
            _owner.VerifyThread(); ObjectDisposedException.ThrowIf(_hwnd == 0, this);
            if (focused)
            {
                if (!_inputEnabled || !_visible || !Native.IsWindowVisible(_hwnd))
                    throw new InvalidOperationException("A hidden, detached, or retiring platform HWND cannot receive focus.");
                Native.SetFocus(_hwnd);
                if (Native.GetFocus() != _hwnd) throw new InvalidOperationException("Native HWND focus request failed.");
            }
            else YieldFocus();
            return ValueTask.CompletedTask;
        }
        public ValueTask DisableInputAsync()
        {
            _owner.VerifyThread(); _inputEnabled = false;
            if (_hwnd != 0) { YieldFocus(); Native.EnableWindow(_hwnd, false); }
            return ValueTask.CompletedTask;
        }
        public ValueTask DisposeAsync()
        {
            _owner.VerifyThread(); _inputEnabled = false;
            if (_hwnd != 0)
            {
                YieldFocus();
                if (!Native.DestroyWindow(_hwnd)) throw new Win32Exception(Marshal.GetLastWin32Error());
                _hwnd = 0;
            }
            GC.KeepAlive(_callback);
            return ValueTask.CompletedTask;
        }
    }
    private static class Native
    {
        [StructLayout(LayoutKind.Sequential)] internal struct Point { public int X, Y; }
        internal delegate nint SubclassProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint subclassId, nuint data);
        [DllImport("kernel32.dll")] internal static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool IsWindow(nint hwnd);
        [DllImport("user32.dll")] internal static extern uint GetWindowThreadProcessId(nint hwnd, out uint processId);
        [DllImport("user32.dll")] internal static extern nint GetParent(nint hwnd);
        [DllImport("user32.dll")] internal static extern nint GetMessageExtraInfo();
        [DllImport("user32.dll")] internal static extern bool ClientToScreen(nint hwnd, ref Point point);
        [DllImport("user32.dll")] internal static extern bool ScreenToClient(nint hwnd, ref Point point);
        [DllImport("user32.dll")] internal static extern nint SendMessageW(nint hwnd, uint message, nuint wparam, nint lparam);
        [DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool SetLayeredWindowAttributes(nint hwnd, uint color, byte alpha, uint flags);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool IsWindowVisible(nint hwnd);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")] internal static extern nint GetWindowLongPtrW(nint hwnd, int index);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)] internal static extern nint CreateWindowExW(uint exStyle, string className, string text, uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);
        [DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool DestroyWindow(nint hwnd);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool ShowWindow(nint hwnd, int command);
        [DllImport("user32.dll")] internal static extern uint GetDpiForWindow(nint hwnd);
        [DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool SetWindowPos(nint hwnd, nint after, int x, int y, int width, int height, uint flags);
        [DllImport("user32.dll")] internal static extern nint SetFocus(nint hwnd);
        [DllImport("user32.dll")] internal static extern nint GetFocus();
        [DllImport("user32.dll")] internal static extern nint GetCapture();
        [DllImport("user32.dll")] internal static extern bool ReleaseCapture();
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool EnableWindow(nint hwnd, [MarshalAs(UnmanagedType.Bool)] bool enabled);
        [DllImport("user32.dll", SetLastError = true)] internal static extern int SetWindowRgn(nint hwnd, nint region, [MarshalAs(UnmanagedType.Bool)] bool redraw);
        [DllImport("gdi32.dll", SetLastError = true)] internal static extern nint CreateRectRgn(int left, int top, int right, int bottom);
        [DllImport("gdi32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool DeleteObject(nint value);
        [DllImport("comctl32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool SetWindowSubclass(nint hwnd, SubclassProc callback, nuint id, nuint data);
        [DllImport("comctl32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool RemoveWindowSubclass(nint hwnd, SubclassProc callback, nuint id);
        [DllImport("comctl32.dll")] internal static extern nint DefSubclassProc(nint hwnd, uint message, nuint wparam, nint lparam);
    }
}
