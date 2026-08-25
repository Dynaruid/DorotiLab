using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// Immutable child-HWND message passed to optional per-view extensions.  This
/// hook is never raised for the standard top-level HWND, so non-client cursor,
/// caption, system menu, and Snap behavior remain owned by DefWindowProc.
/// </summary>
internal readonly record struct FlutterWindowsChildMessage(
    nint Hwnd,
    uint Message,
    nuint WParam,
    nint LParam,
    TimeSpan Timestamp);

/// <summary>
/// A typed child WndProc response. An unhandled result deliberately falls
/// through to DefWindowProc; independent child extensions can answer their
/// own messages without making this host or the input router their owner.
/// </summary>
internal readonly record struct FlutterWindowsChildMessageResult(bool Handled, nint Result)
{
    internal static FlutterWindowsChildMessageResult Unhandled { get; } = new(false, 0);

    internal static FlutterWindowsChildMessageResult HandledResult(nint result = default) =>
        new(true, result);
}

internal readonly record struct FlutterWindowsTopLevelMessage(
    nint Hwnd,
    uint Message,
    nuint WParam,
    nint LParam,
    TimeSpan Timestamp);

/// <summary>
/// F2-only raw Win32 ownership for the Flutter-style Windows host path.
/// It deliberately owns one standard top-level HWND and one child view HWND,
/// while F3+ add metrics, EGL window surfaces, input, and frame scheduling.
/// </summary>
internal sealed class FlutterWindowsHostWindow : IDisposable
{
    private const uint WmSize = 0x0005;
    private const uint WmGetMinMaxInfo = 0x0024;
    private const uint WmDpiChanged = 0x02E0;
    private const uint WmDisplayChange = 0x007E;
    private const uint WmPaint = 0x000F;
    private const uint WmEraseBackground = 0x0014;
    private const uint WmNcDestroy = 0x0082;
    private const uint WsOverlappedWindow = 0x00CF0000;
    private const uint WsClipChildren = 0x02000000;
    private const uint WsChild = 0x40000000;
    private const uint WsVisible = 0x10000000;
    private const uint WsClipSiblings = 0x04000000;
    private const uint WsExAppWindow = 0x00040000;
    private const uint WsExNoRedirectionBitmap = 0x00200000;
    private const int GwlStyle = -16;
    private const int SwHide = 0;
    private const int SwShowNoActivate = 4;
    private const int ClassAlreadyExists = 1410;
    private const string TopLevelClassName = "Doroti.Flutter.Windows.TopLevel";
    private const string ViewClassName = "Doroti.Flutter.Windows.View";

    private static readonly ConcurrentDictionary<nint, FlutterWindowsHostWindow> TopLevelWindows = new();
    private static readonly ConcurrentDictionary<nint, FlutterWindowsHostWindow> ViewWindows = new();
    private static readonly NativeMethods.WindowProcedure WindowProcedure = StaticWindowProcedure;
    private static readonly object ClassGate = new();
    private static ushort _topLevelClass;
    private static ushort _viewClass;

    private readonly FlutterWindowsAppSdkBootstrap _bootstrap;
    private readonly FlutterWindowsHostWindowOptions _options;
    private readonly FlutterWindowsHostWindowTeardown _teardown;
    private readonly object _stateGate = new();
    private readonly List<string> _teardownOrder = [];
    private readonly int _platformManagedThreadId;
    private readonly uint _platformNativeThreadId;
    private nint _topLevelHwnd;
    private nint _viewHwnd;
    private bool _firstFrameSwapped;
    private bool _shown;
    private bool _disposed;
    private long _childRectMismatchCount;
    private long _childLayoutCount;
    private long _topLevelEraseBeforeFirstSwap;
    private long _viewEraseBeforeFirstSwap;
    private long _firstFrameShowCount;
    private long _dpiSuggestedRectApplyCount;
    private int _clientWidth;
    private int _clientHeight;
    private int _childWidth;
    private int _childHeight;
    private int _minimumTrackWidth;
    private int _minimumTrackHeight;
    private int _maximumTrackWidth;
    private int _maximumTrackHeight;

    private FlutterWindowsHostWindow(
        FlutterWindowsAppSdkBootstrap bootstrap,
        FlutterWindowsHostWindowOptions options,
        FlutterWindowsHostWindowTeardown teardown,
        nint topLevelHwnd,
        nint viewHwnd)
    {
        _bootstrap = bootstrap;
        _options = options;
        _teardown = teardown;
        _topLevelHwnd = topLevelHwnd;
        _viewHwnd = viewHwnd;
        _platformManagedThreadId = Environment.CurrentManagedThreadId;
        _platformNativeThreadId = NativeMethods.GetCurrentThreadId();
    }

    internal nint TopLevelHwnd => _topLevelHwnd;

    internal nint ViewHwnd => _viewHwnd;

    internal event Action? CloseRequested;

    /// <summary>
    /// F3 listens only after the child has reported its actual client rect;
    /// it never infers render size from the top-level outer geometry.
    /// </summary>
    internal event Action? ChildClientRectChanged;

    /// <summary>
    /// DPI/display notifications require F3 to re-read the child HWND's DPI,
    /// monitor identity, and physical client rect as one new observation.
    /// </summary>
    internal event Action? ChildDpiOrDisplayChanged;

    /// <summary>
    /// A child invalidation requests a retained-scene redraw. DefWindowProc
    /// still validates the native paint region; this event only mirrors
    /// FlutterWindow::OnPaint -> ForceRedraw.
    /// </summary>
    internal event Action? ChildRepaintRequested;

    /// <summary>
    /// Optional child-only WndProc extension point. It is invoked after this
    /// host has performed its F2 child bookkeeping and before the child falls
    /// through to DefWindowProc. Subscribers run in registration order; the
    /// first handled result wins. The top-level HWND never raises this event.
    /// </summary>
    internal event Func<FlutterWindowsChildMessage, FlutterWindowsChildMessageResult>? ChildMessageReceived;

    /// <summary>
    /// F8 lifecycle extension point. Standard non-client messages remain
    /// unhandled and continue to DefWindowProc.
    /// </summary>
    internal event Func<FlutterWindowsTopLevelMessage, FlutterWindowsChildMessageResult>? TopLevelMessageReceived;

    internal static FlutterWindowsHostWindow CreateOnCurrentThread(
        FlutterWindowsAppSdkBootstrap bootstrap,
        FlutterWindowsHostWindowOptions options,
        FlutterWindowsHostWindowTeardown? teardown = null)
    {
        ArgumentNullException.ThrowIfNull(bootstrap);
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();
        EnsureWindowClasses();
        bootstrap.InitializeOnCurrentThread();

        nint topLevel = 0;
        nint view = 0;
        var associated = false;
        try
        {
            var dpi = NativeMethods.GetDpiForSystem();
            if (dpi == 0) dpi = 96;
            var initialWindowRect = ClientToWindowRect(
                options.InitialClientWidth,
                options.InitialClientHeight,
                WsOverlappedWindow | WsClipChildren,
                WsExAppWindow,
                dpi);
            topLevel = NativeMethods.CreateWindowExW(
                WsExAppWindow,
                TopLevelClassName,
                options.Title,
                WsOverlappedWindow | WsClipChildren,
                options.InitialX,
                options.InitialY,
                initialWindowRect.Width,
                initialWindowRect.Height,
                0,
                0,
                NativeMethods.GetModuleHandleW(null),
                0);
            if (topLevel == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "CreateWindowExW for the Flutter top-level HWND failed.");
            }

            _ = bootstrap.AssociateRawWindow(topLevel);
            associated = true;
            if (!NativeMethods.GetClientRect(topLevel, out var clientRect))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "GetClientRect for the Flutter top-level HWND failed.");
            }

            view = NativeMethods.CreateWindowExW(
                options.UseNoRedirectionBitmap ? WsExNoRedirectionBitmap : 0,
                ViewClassName,
                string.Empty,
                WsChild | WsVisible | WsClipSiblings,
                0,
                0,
                clientRect.Width,
                clientRect.Height,
                topLevel,
                0,
                NativeMethods.GetModuleHandleW(null),
                0);
            if (view == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "CreateWindowExW for the Flutter child view HWND failed.");
            }

            var host = new FlutterWindowsHostWindow(
                bootstrap,
                options,
                teardown ?? FlutterWindowsHostWindowTeardown.Empty,
                topLevel,
                view);
            TopLevelWindows[topLevel] = host;
            ViewWindows[view] = host;
            host.UpdateChildToClientRect();
            return host;
        }
        catch
        {
            if (view != 0 && NativeMethods.IsWindow(view)) _ = NativeMethods.DestroyWindow(view);
            if (topLevel != 0 && NativeMethods.IsWindow(topLevel)) _ = NativeMethods.DestroyWindow(topLevel);
            if (associated) bootstrap.ReleaseRawWindowAssociation(topLevel);
            throw;
        }
    }

    /// <summary>
    /// F2 accepts this only from the later raster/present owner after its first
    /// exact successful swap. Until then the ordinary top-level HWND is hidden.
    /// </summary>
    internal void NotifyFirstFrameSwapped()
    {
        EnsurePlatformThread();
        ThrowIfDisposed();
        if (_firstFrameSwapped) return;
        _firstFrameSwapped = true;
        _ = NativeMethods.ShowWindow(_topLevelHwnd, SwShowNoActivate);
        _ = NativeMethods.UpdateWindow(_topLevelHwnd);
        _shown = NativeMethods.IsWindowVisible(_topLevelHwnd);
        Interlocked.Increment(ref _firstFrameShowCount);
        if (!_shown)
            throw new InvalidOperationException(
                "The Flutter top-level HWND was not visible after the first-frame show callback.");
    }

    internal void HideForValidation()
    {
        EnsurePlatformThread();
        ThrowIfDisposed();
        _ = NativeMethods.ShowWindow(_topLevelHwnd, SwHide);
        _shown = false;
    }

    internal FlutterWindowsHostWindowSnapshot Snapshot
    {
        get
        {
            lock (_stateGate)
            {
                var bootstrap = _bootstrap.Snapshot;
                var topLevelStyle = _topLevelHwnd == 0
                    ? 0u
                    : unchecked((uint)NativeMethods.GetWindowLongPtrW(_topLevelHwnd, GwlStyle).ToInt64());
                var viewStyle = _viewHwnd == 0
                    ? 0u
                    : unchecked((uint)NativeMethods.GetWindowLongPtrW(_viewHwnd, GwlStyle).ToInt64());
                return new FlutterWindowsHostWindowSnapshot(
                    _topLevelHwnd,
                    _viewHwnd,
                    topLevelStyle,
                    viewStyle,
                    _clientWidth,
                    _clientHeight,
                    _childWidth,
                    _childHeight,
                    Interlocked.Read(ref _childRectMismatchCount),
                    Interlocked.Read(ref _childLayoutCount),
                    Interlocked.Read(ref _topLevelEraseBeforeFirstSwap),
                    Interlocked.Read(ref _viewEraseBeforeFirstSwap),
                    Interlocked.Read(ref _firstFrameShowCount),
                    Interlocked.Read(ref _dpiSuggestedRectApplyCount),
                    _minimumTrackWidth,
                    _minimumTrackHeight,
                    _maximumTrackWidth,
                    _maximumTrackHeight,
                    _firstFrameSwapped,
                    _shown,
                    _disposed,
                    bootstrap.DispatcherQueueCreated,
                    bootstrap.RawWindowAssociated,
                    bootstrap.AppWindowAssociated,
                    bootstrap.RawWindowAssociationCount,
                    [.. _teardownOrder]);
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        EnsurePlatformThread();
        _disposed = true;
        ChildRepaintRequested = null;
        ChildMessageReceived = null;
        TopLevelMessageReceived = null;
        var failures = new List<Exception>();

        RunTeardownStep("auxiliary-islands", _teardown.DisposeAuxiliaryIslands, failures);
        RunTeardownStep("view-surface", _teardown.DisposeViewSurface, failures);

        var view = _viewHwnd;
        _viewHwnd = 0;
        ViewWindows.TryRemove(view, out _);
        RunTeardownStep("child", () => DestroyOwnedWindow(view, "Flutter child view"), failures);

        RunTeardownStep("engine", _teardown.DisposeChildEngine, failures);

        var topLevel = _topLevelHwnd;
        _topLevelHwnd = 0;
        TopLevelWindows.TryRemove(topLevel, out _);
        RunTeardownStep("appwindow-top-level", () =>
        {
            DestroyOwnedWindow(topLevel, "Flutter top-level");
            _bootstrap.ReleaseRawWindowAssociation(topLevel);
        }, failures);
        RunTeardownStep("dispatcher-queue", _bootstrap.DisposeOnCurrentThread, failures);

        if (failures.Count != 0)
            throw new AggregateException("Flutter Windows F2 teardown failed.", failures);
    }

    private FlutterWindowsChildMessageResult HandleTopLevelMessage(uint message, nuint wParam, nint lParam)
    {
        var handledByHost = false;
        var hostResult = nint.Zero;
        switch (message)
        {
            case WmSize:
                if (!_disposed && _viewHwnd != 0) UpdateChildToClientRect();
                break;
            case WmGetMinMaxInfo:
                if (!_disposed && lParam != 0) ApplyMinMaxInfo(lParam);
                break;
            case WmDpiChanged:
                if (!_disposed && lParam != 0) ApplyDpiSuggestedRect(lParam);
                if (!_disposed) ChildDpiOrDisplayChanged?.Invoke();
                break;
            case WmDisplayChange:
                if (!_disposed) ChildDpiOrDisplayChanged?.Invoke();
                break;
            case WmEraseBackground:
                if (!_firstFrameSwapped) Interlocked.Increment(ref _topLevelEraseBeforeFirstSwap);
                handledByHost = true;
                hostResult = 1;
                break;
            case 0x0010: // WM_CLOSE: later stages coordinate engine/raster shutdown.
                CloseRequested?.Invoke();
                handledByHost = true;
                break;
        }
        var routed = DispatchTopLevelMessage(new(
            _topLevelHwnd,
            message,
            wParam,
            lParam,
            Doroti.Ui.DorotiFrameClock.Now));
        if (routed.Handled) return routed;
        return handledByHost
            ? FlutterWindowsChildMessageResult.HandledResult(hostResult)
            : FlutterWindowsChildMessageResult.Unhandled;
    }

    private FlutterWindowsChildMessageResult DispatchTopLevelMessage(FlutterWindowsTopLevelMessage message)
    {
        var handlers = TopLevelMessageReceived;
        if (handlers is null) return FlutterWindowsChildMessageResult.Unhandled;
        foreach (var candidate in handlers.GetInvocationList())
        {
            var result = ((Func<FlutterWindowsTopLevelMessage, FlutterWindowsChildMessageResult>)candidate)(message);
            if (result.Handled) return result;
        }
        return FlutterWindowsChildMessageResult.Unhandled;
    }

    private void ApplyDpiSuggestedRect(nint lParam)
    {
        var rect = Marshal.PtrToStructure<NativeMethods.NativeRect>(lParam);
        if (rect.Width <= 0 || rect.Height <= 0) return;
        if (!NativeMethods.SetWindowPos(
                _topLevelHwnd,
                0,
                rect.Left,
                rect.Top,
                rect.Width,
                rect.Height,
                0x0004u | 0x0010u))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                "SetWindowPos for the WM_DPICHANGED suggested top-level rect failed.");
        }
        Interlocked.Increment(ref _dpiSuggestedRectApplyCount);
    }

    private FlutterWindowsChildMessageResult HandleViewMessage(
        nint hwnd,
        uint message,
        nuint wParam,
        nint lParam)
    {
        if (message == WmSize && !_disposed) CaptureChildRect();
        if (message == WmPaint && !_disposed) ChildRepaintRequested?.Invoke();
        var handledByHost = false;
        var hostResult = nint.Zero;
        if (message == WmEraseBackground)
        {
            if (!_firstFrameSwapped) Interlocked.Increment(ref _viewEraseBeforeFirstSwap);
            handledByHost = true;
            hostResult = 1;
        }
        var routed = DispatchChildMessage(new(
            hwnd,
            message,
            wParam,
            lParam,
            Doroti.Ui.DorotiFrameClock.Now));
        if (routed.Handled) return routed;
        return handledByHost
            ? FlutterWindowsChildMessageResult.HandledResult(hostResult)
            : FlutterWindowsChildMessageResult.Unhandled;
    }

    private FlutterWindowsChildMessageResult DispatchChildMessage(FlutterWindowsChildMessage message)
    {
        var handlers = ChildMessageReceived;
        if (handlers is null) return FlutterWindowsChildMessageResult.Unhandled;
        foreach (var candidate in handlers.GetInvocationList())
        {
            var handler = (Func<FlutterWindowsChildMessage, FlutterWindowsChildMessageResult>)candidate;
            var result = handler(message);
            if (result.Handled) return result;
        }
        return FlutterWindowsChildMessageResult.Unhandled;
    }

    private void UpdateChildToClientRect()
    {
        if (_topLevelHwnd == 0 || _viewHwnd == 0) return;
        if (!NativeMethods.GetClientRect(_topLevelHwnd, out var clientRect))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                "GetClientRect during Flutter child layout failed.");
        }
        lock (_stateGate)
        {
            _clientWidth = clientRect.Width;
            _clientHeight = clientRect.Height;
        }
        if (!NativeMethods.MoveWindow(
                _viewHwnd, 0, 0, clientRect.Width, clientRect.Height, repaint: false))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                "MoveWindow during Flutter child layout failed.");
        }
        CaptureChildRect();
        Interlocked.Increment(ref _childLayoutCount);
    }

    private void CaptureChildRect()
    {
        if (_viewHwnd == 0 || !NativeMethods.GetClientRect(_viewHwnd, out var childRect)) return;
        int clientWidth;
        int clientHeight;
        lock (_stateGate)
        {
            _childWidth = childRect.Width;
            _childHeight = childRect.Height;
            clientWidth = _clientWidth;
            clientHeight = _clientHeight;
        }
        if (childRect.Width != clientWidth || childRect.Height != clientHeight)
            Interlocked.Increment(ref _childRectMismatchCount);
        ChildClientRectChanged?.Invoke();
    }

    private void ApplyMinMaxInfo(nint lParam)
    {
        var info = Marshal.PtrToStructure<NativeMethods.MinMaxInfo>(lParam);
        var dpi = NativeMethods.GetDpiForWindow(_topLevelHwnd);
        if (dpi == 0) dpi = 96;
        var minimum = ClientToWindowRect(
            _options.MinimumClientWidth,
            _options.MinimumClientHeight,
            WsOverlappedWindow | WsClipChildren,
            WsExAppWindow,
            dpi);
        var maximum = ClientToWindowRect(
            _options.MaximumClientWidth,
            _options.MaximumClientHeight,
            WsOverlappedWindow | WsClipChildren,
            WsExAppWindow,
            dpi);
        info.MinimumTrackSize = new NativeMethods.NativePoint
        {
            X = minimum.Width,
            Y = minimum.Height,
        };
        info.MaximumTrackSize = new NativeMethods.NativePoint
        {
            X = maximum.Width,
            Y = maximum.Height,
        };
        Marshal.StructureToPtr(info, lParam, fDeleteOld: false);
        lock (_stateGate)
        {
            _minimumTrackWidth = minimum.Width;
            _minimumTrackHeight = minimum.Height;
            _maximumTrackWidth = maximum.Width;
            _maximumTrackHeight = maximum.Height;
        }
    }

    private static NativeMethods.NativeRect ClientToWindowRect(
        int clientWidth,
        int clientHeight,
        uint style,
        uint extendedStyle,
        uint dpi)
    {
        var rect = new NativeMethods.NativeRect
        {
            Left = 0,
            Top = 0,
            Right = clientWidth,
            Bottom = clientHeight,
        };
        if (!NativeMethods.AdjustWindowRectExForDpi(ref rect, style, hasMenu: false, extendedStyle, dpi))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                "AdjustWindowRectExForDpi for the Flutter standard top-level HWND failed.");
        }
        return rect;
    }

    private static void EnsureWindowClasses()
    {
        lock (ClassGate)
        {
            if (_topLevelClass != 0 && _viewClass != 0) return;
            var instance = NativeMethods.GetModuleHandleW(null);
            _topLevelClass = RegisterWindowClass(TopLevelClassName, instance);
            _viewClass = RegisterWindowClass(ViewClassName, instance);
        }
    }

    private static ushort RegisterWindowClass(string className, nint instance)
    {
        var windowClass = new NativeMethods.WindowClassEx
        {
            Size = checked((uint)Marshal.SizeOf<NativeMethods.WindowClassEx>()),
            WindowProcedure = Marshal.GetFunctionPointerForDelegate(WindowProcedure),
            Instance = instance,
            Cursor = NativeMethods.LoadCursorW(0, 32512),
            ClassName = className,
        };
        var result = NativeMethods.RegisterClassExW(in windowClass);
        if (result != 0) return result;
        var error = Marshal.GetLastWin32Error();
        if (error == ClassAlreadyExists) return 1;
        throw new Win32Exception(error, $"RegisterClassExW failed for {className}.");
    }

    private static nint StaticWindowProcedure(nint hwnd, uint message, nuint wParam, nint lParam)
    {
        if (TopLevelWindows.TryGetValue(hwnd, out var topLevel))
        {
            var result = topLevel.HandleTopLevelMessage(message, wParam, lParam);
            if (message == WmNcDestroy) TopLevelWindows.TryRemove(hwnd, out _);
            if (result.Handled) return result.Result;
        }
        else if (ViewWindows.TryGetValue(hwnd, out var view))
        {
            var result = view.HandleViewMessage(hwnd, message, wParam, lParam);
            if (message == WmNcDestroy) ViewWindows.TryRemove(hwnd, out _);
            if (result.Handled) return result.Result;
        }
        if (message == WmEraseBackground) return 1;
        return NativeMethods.DefWindowProcW(hwnd, message, wParam, lParam);
    }

    private static void DestroyOwnedWindow(nint hwnd, string description)
    {
        if (hwnd == 0 || !NativeMethods.IsWindow(hwnd)) return;
        if (!NativeMethods.DestroyWindow(hwnd))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                $"DestroyWindow failed for {description} HWND.");
        }
    }

    private void RunTeardownStep(string name, Action action, ICollection<Exception> failures)
    {
        lock (_stateGate) _teardownOrder.Add(name);
        try
        {
            action();
        }
        catch (Exception exception)
        {
            failures.Add(new InvalidOperationException($"F2 teardown step '{name}' failed.", exception));
        }
    }

    private void EnsurePlatformThread()
    {
        if (Environment.CurrentManagedThreadId != _platformManagedThreadId ||
            NativeMethods.GetCurrentThreadId() != _platformNativeThreadId)
        {
            throw new InvalidOperationException(
                "The Flutter F2 HWND tree must be owned and torn down by its platform thread.");
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private static partial class NativeMethods
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate nint WindowProcedure(nint hwnd, uint message, nuint wParam, nint lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WindowClassEx
        {
            internal uint Size;
            internal uint Style;
            internal nint WindowProcedure;
            internal int ClassExtra;
            internal int WindowExtra;
            internal nint Instance;
            internal nint Icon;
            internal nint Cursor;
            internal nint Background;
            internal string? MenuName;
            internal string ClassName;
            internal nint SmallIcon;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NativePoint
        {
            internal int X;
            internal int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeRect
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
            internal int Width => Right - Left;
            internal int Height => Bottom - Top;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MinMaxInfo
        {
            internal NativePoint Reserved;
            internal NativePoint MaximumSize;
            internal NativePoint MaximumPosition;
            internal NativePoint MinimumTrackSize;
            internal NativePoint MaximumTrackSize;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern ushort RegisterClassExW(in WindowClassEx windowClass);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern nint CreateWindowExW(
            uint extendedStyle,
            string className,
            string windowName,
            uint style,
            int x,
            int y,
            int width,
            int height,
            nint parent,
            nint menu,
            nint instance,
            nint parameter);

        [DllImport("user32.dll")]
        internal static extern nint DefWindowProcW(nint hwnd, uint message, nuint wParam, nint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(nint hwnd, out NativeRect rect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MoveWindow(
            nint hwnd,
            int x,
            int y,
            int width,
            int height,
            [MarshalAs(UnmanagedType.Bool)] bool repaint);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(
            nint hwnd,
            nint insertAfter,
            int x,
            int y,
            int width,
            int height,
            uint flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowWindow(nint hwnd, int command);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UpdateWindow(nint hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyWindow(nint hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(nint hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowVisible(nint hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AdjustWindowRectExForDpi(
            ref NativeRect rect,
            uint style,
            [MarshalAs(UnmanagedType.Bool)] bool hasMenu,
            uint extendedStyle,
            uint dpi);

        [DllImport("user32.dll")]
        internal static extern uint GetDpiForWindow(nint hwnd);

        [DllImport("user32.dll")]
        internal static extern uint GetDpiForSystem();

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
        internal static extern nint GetWindowLongPtrW(nint hwnd, int index);

        [DllImport("user32.dll")]
        internal static extern nint LoadCursorW(nint instance, int cursorName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern nint GetModuleHandleW(string? moduleName);

        [DllImport("kernel32.dll")]
        internal static extern uint GetCurrentThreadId();
    }
}

internal sealed class FlutterWindowsHostWindowTeardown
{
    internal static readonly FlutterWindowsHostWindowTeardown Empty = new();

    internal Action DisposeAuxiliaryIslands { get; init; } = static () => { };

    internal Action DisposeViewSurface { get; init; } = static () => { };

    internal Action DisposeChildEngine { get; init; } = static () => { };
}

/// <summary>
/// F2 physical-pixel window constraints. F3 promotes these to the immutable
/// per-view metrics snapshot and display/DPI contract.
/// </summary>
internal sealed record FlutterWindowsHostWindowOptions(
    string Title,
    int InitialClientWidth,
    int InitialClientHeight,
    int MinimumClientWidth,
    int MinimumClientHeight,
    int MaximumClientWidth,
    int MaximumClientHeight,
    int InitialX = 96,
    int InitialY = 96,
    bool UseNoRedirectionBitmap = false)
{
    internal void Validate()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Title);
        if (InitialClientWidth <= 0 || InitialClientHeight <= 0 ||
            MinimumClientWidth <= 0 || MinimumClientHeight <= 0 ||
            MaximumClientWidth < MinimumClientWidth ||
            MaximumClientHeight < MinimumClientHeight ||
            InitialClientWidth < MinimumClientWidth || InitialClientHeight < MinimumClientHeight ||
            InitialClientWidth > MaximumClientWidth || InitialClientHeight > MaximumClientHeight)
        {
            throw new ArgumentOutOfRangeException(nameof(InitialClientWidth),
                "Flutter F2 client constraints must be finite positive physical pixels.");
        }
    }
}

internal sealed record FlutterWindowsHostWindowSnapshot(
    nint TopLevelHwnd,
    nint ViewHwnd,
    uint TopLevelStyle,
    uint ViewStyle,
    int ClientWidth,
    int ClientHeight,
    int ChildWidth,
    int ChildHeight,
    long ChildRectMismatchCount,
    long ChildLayoutCount,
    long TopLevelEraseBeforeFirstSwap,
    long ViewEraseBeforeFirstSwap,
    long FirstFrameShowCount,
    long DpiSuggestedRectApplyCount,
    int MinimumTrackWidth,
    int MinimumTrackHeight,
    int MaximumTrackWidth,
    int MaximumTrackHeight,
    bool FirstFrameSwapped,
    bool Shown,
    bool Disposed,
    bool DispatcherQueueCreated,
    bool RawWindowAssociated,
    bool AppWindowAssociated,
    int RawWindowAssociationCount,
    IReadOnlyList<string> TeardownOrder);
