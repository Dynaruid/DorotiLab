using System.ComponentModel;
using System.Runtime.InteropServices;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Generic HWND attachment for an explicitly selected lower-DComp/CLIPCHILDREN container.</summary>
public sealed class WindowsHwndPlatformViewFactory : IPlatformViewFactory
{
    private readonly nint _parent;
    private readonly bool _lowerCompositionTarget;
    private readonly bool _editor;
    private readonly uint _uiThread;
    public WindowsHwndPlatformViewFactory(nint parent, bool editor, bool lowerCompositionTarget)
    {
        _parent = parent;
        _editor = editor;
        _lowerCompositionTarget = lowerCompositionTarget;
        _uiThread = Native.GetCurrentThreadId();
    }
    public string ViewType => _editor ? "doroti/native-editor" : "doroti/native-button";
    public PlatformViewSupport QuerySupport(PlatformViewRequest request)
    {
        var supported = _lowerCompositionTarget && request.Composition == PlatformViewComposition.NativeOverlay &&
            (request.Effects & ~PlatformViewEffects.RectClip) == 0;
        return new("Windows-HWND", Environment.OSVersion.VersionString, ViewType, supported,
            PlatformViewComposition.NativeOverlay, PlatformViewEffects.RectClip,
            Reason: supported ? null : "Generic HWND requires an explicit lower DComp target with WS_CLIPCHILDREN; interleaved visuals are not implemented for HWND controls.");
    }
    public ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
        Action<PlatformViewHandle> onFocused, CancellationToken cancellationToken)
    {
        VerifyThread();
        cancellationToken.ThrowIfCancellationRequested();
        if (!_lowerCompositionTarget || !Native.IsWindow(_parent) || (Native.GetWindowLongPtrW(_parent, -16).ToInt64() & 0x02000000) == 0)
            throw new InvalidOperationException("Native HWND parent must use WS_CLIPCHILDREN with a lower composition target.");
        var text = parameters.IsEmpty ? (_editor ? "Native editor" : "Native button") : System.Text.Encoding.UTF8.GetString(parameters.Span);
        var hwnd = Native.CreateWindowExW(_editor ? 0x200u : 0u, _editor ? "EDIT" : "BUTTON", text,
            0x40000000u | 0x00010000u | (_editor ? 0x00800080u : 0u), 0, 0, 0, 0, _parent, 0, 0, 0);
        if (hwnd == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
        try { return ValueTask.FromResult<IPlatformViewInstance>(new Instance(this, hwnd, handle, onFocused)); }
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
        public Instance(WindowsHwndPlatformViewFactory owner, nint hwnd, PlatformViewHandle handle, Action<PlatformViewHandle> onFocused)
        {
            _owner = owner; _hwnd = hwnd; _handle = handle; _onFocused = onFocused; _callback = WindowProc;
            if (!Native.SetWindowSubclass(hwnd, _callback, 1, 0)) throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        private nint WindowProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint subclassId, nuint data)
        {
            // Never let managed exceptions unwind through the unmanaged window procedure.
            if (message == 7 && _inputEnabled)
            {
                try { _onFocused(_handle); }
                catch (Exception exception) { System.Diagnostics.Trace.TraceError(exception.ToString()); }
            }
            if (message == 0x82) { Native.RemoveWindowSubclass(hwnd, _callback, 1); _hwnd = 0; }
            return Native.DefSubclassProc(hwnd, message, wparam, lparam);
        }
        public ValueTask ApplyAsync(PlatformViewPlacement placement)
        {
            _owner.VerifyThread(); placement.Validate();
            if (placement.Handle != _handle || !placement.Transform.IsAxisAligned || placement.Transform.M11 != 1 || placement.Transform.M22 != 1)
                throw new InvalidOperationException("HWND only supports translation and rectangular clipping.");
            ObjectDisposedException.ThrowIf(_hwnd == 0, this);
            var dpi = Native.GetDpiForWindow(_owner._parent) / 96d;
            var origin = placement.Transform.Map(placement.Bounds.topLeft);
            var bounds = Rect.fromLTWH(origin.dx, origin.dy, placement.Bounds.width, placement.Bounds.height);
            int x = Pixel(bounds.left), y = Pixel(bounds.top), right = Pixel(bounds.right), bottom = Pixel(bounds.bottom);
            if (!Native.SetWindowPos(_hwnd, 0, x, y, right - x, bottom - y, 0x0010 | 0x0004)) throw new Win32Exception(Marshal.GetLastWin32Error());
            var clip = placement.Clip is { } clipping ? bounds.intersect(clipping) : bounds;
            var region = Native.CreateRectRgn(Math.Max(0, Pixel(clip.left) - x), Math.Max(0, Pixel(clip.top) - y),
                Math.Max(0, Pixel(clip.right) - x), Math.Max(0, Pixel(clip.bottom) - y));
            if (region == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
            if (Native.SetWindowRgn(_hwnd, region, true) == 0) { Native.DeleteObject(region); throw new Win32Exception(Marshal.GetLastWin32Error()); }
            Native.ShowWindow(_hwnd, placement.Visible && !clip.isEmpty && !bounds.isEmpty ? 4 : 0);
            return ValueTask.CompletedTask;
            int Pixel(double value) => checked((int)Math.Round(value * dpi));
        }
        public ValueTask DetachAsync() { _owner.VerifyThread(); if (_hwnd != 0) Native.ShowWindow(_hwnd, 0); return ValueTask.CompletedTask; }
        public ValueTask SetFocusAsync(bool focused)
        {
            _owner.VerifyThread(); ObjectDisposedException.ThrowIf(_hwnd == 0, this);
            if (focused) Native.SetFocus(_hwnd);
            else if (Native.GetFocus() == _hwnd) Native.SetFocus(_owner._parent);
            return ValueTask.CompletedTask;
        }
        public ValueTask DisableInputAsync()
        {
            _owner.VerifyThread(); _inputEnabled = false;
            if (_hwnd != 0) { Native.EnableWindow(_hwnd, false); if (Native.GetFocus() == _hwnd) Native.SetFocus(_owner._parent); }
            return ValueTask.CompletedTask;
        }
        public ValueTask DisposeAsync()
        {
            _owner.VerifyThread(); _inputEnabled = false;
            if (_hwnd != 0)
            {
                if (!Native.DestroyWindow(_hwnd)) throw new Win32Exception(Marshal.GetLastWin32Error());
                _hwnd = 0;
            }
            GC.KeepAlive(_callback);
            return ValueTask.CompletedTask;
        }
    }
    private static class Native
    {
        internal delegate nint SubclassProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint subclassId, nuint data);
        [DllImport("kernel32.dll")] internal static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool IsWindow(nint hwnd);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")] internal static extern nint GetWindowLongPtrW(nint hwnd, int index);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)] internal static extern nint CreateWindowExW(uint exStyle, string className, string text, uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);
        [DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool DestroyWindow(nint hwnd);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool ShowWindow(nint hwnd, int command);
        [DllImport("user32.dll")] internal static extern uint GetDpiForWindow(nint hwnd);
        [DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool SetWindowPos(nint hwnd, nint after, int x, int y, int width, int height, uint flags);
        [DllImport("user32.dll")] internal static extern nint SetFocus(nint hwnd);
        [DllImport("user32.dll")] internal static extern nint GetFocus();
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool EnableWindow(nint hwnd, [MarshalAs(UnmanagedType.Bool)] bool enabled);
        [DllImport("user32.dll", SetLastError = true)] internal static extern int SetWindowRgn(nint hwnd, nint region, [MarshalAs(UnmanagedType.Bool)] bool redraw);
        [DllImport("gdi32.dll", SetLastError = true)] internal static extern nint CreateRectRgn(int left, int top, int right, int bottom);
        [DllImport("gdi32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool DeleteObject(nint value);
        [DllImport("comctl32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool SetWindowSubclass(nint hwnd, SubclassProc callback, nuint id, nuint data);
        [DllImport("comctl32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool RemoveWindowSubclass(nint hwnd, SubclassProc callback, nuint id);
        [DllImport("comctl32.dll")] internal static extern nint DefSubclassProc(nint hwnd, uint message, nuint wparam, nint lparam);
    }
}
