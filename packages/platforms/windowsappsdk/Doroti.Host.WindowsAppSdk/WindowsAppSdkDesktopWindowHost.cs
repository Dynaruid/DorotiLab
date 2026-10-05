using System.Runtime.InteropServices;
using Doroti.Desktop;
using Doroti.Hosting;
using Doroti.Ui;
using Appearance = Doroti.Desktop.WindowAppearanceOptions;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Desktop controller for the runner's existing native window. No second HWND/root.</summary>
internal sealed partial class WindowsAppSdkDesktopWindowHost : IWindowHost, IWindowHostFactory, IPlatformMenuHostCapability, IPlatformMenuBarHostCapability
{
    private readonly WindowsPlatformViewDispatcher _ui = new();
    private readonly TaskCompletionSource _attached = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _closed = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly SubclassProc _procedure;
    private WindowsManagedProductHost? _host;
    private nint _hwnd;
    private WindowOptions _options = new();
    private bool _acceptedClose;
    private bool _disposed;
    private bool _allocated;
    private NativeRect _restoredBounds;
    private nint _restoredStyle;
    private bool _fullScreen;
    internal DesktopWindowContext? WindowContext { get; private set; }
    internal nint NativeWindow => _hwnd;
    internal bool VisibilityRequested { get; private set; }
    internal bool WasVisible { get; private set; }
    internal nint NativeOwner { get; init; }
    private IDisposable? _ownerSubscription;
    private bool _ownerDisabled;
    private static readonly object ModalGate = new();
    private static readonly Dictionary<nint, int> ModalOwners = [];

    internal WindowsAppSdkDesktopWindowHost()
    {
        _procedure = WindowProc;
        State = new(null, new Size(800, 600), 1, false, false, WindowPresentationState.Normal,
            new(), new(new()), new(0, ViewPadding.zero, 0, 0, 0));
    }
    public WindowCapabilities Capabilities { get; } = new(EvaluateOptions);
    WindowManagerCapabilities IWindowHostFactory.Capabilities { get; } = new(false);
    public WindowEvaluation Evaluate(WindowCreateOptions options) => EvaluateOptions(options.Options, null);
    private static WindowEvaluation EvaluateOptions(WindowOptions options, WindowOptions? current)
    {
        options.Validate();
        if (!Enum.IsDefined(options.Kind)) return new(WindowSupport.Unsupported, "Unknown window kind.");
        if (options.Kind != WindowKind.Regular && options.OwnerWindowId is null) return new(WindowSupport.Unsupported, "Owned window kinds require an explicit live owner.");
        if (options.Kind is WindowKind.Popup or WindowKind.Tooltip && options.Anchor is null) return new(WindowSupport.Unsupported, "Popup and tooltip windows require a logical owner anchor.");
        if (options.Modal && options.Kind != WindowKind.Dialog) return new(WindowSupport.Unsupported, "Only Dialog windows can be modal.");
        if (options.StartupVisibility == WindowStartupVisibility.PlatformDefault)
            return new(WindowSupport.Unsupported, "This runner owns first visibility. Select Manual or WhenReady.");
        if (options.Appearance.TitleBar.Style != WindowTitleBarStyle.Normal ||
            options.Appearance.TitleBar.Frame != WindowFrame.Standard ||
            options.Appearance.TitleBar.Buttons != WindowCaptionButtonMode.Native)
            return new(WindowSupport.Unsupported, "Only standard native chrome is qualified.");
        if (options.Appearance.ThemeSource == WindowThemeSource.App ||
            options.Appearance.Backdrop.Mode == Desktop.WindowBackdropMode.LiquidGlass)
            return new(WindowSupport.Unsupported, "App theme bridge and Liquid Glass are not available on this host.");
        if (options.Appearance.Backdrop.Mode == Desktop.WindowBackdropMode.Transparent)
            return new(WindowSupport.Unsupported, "The opaque native presenter does not implement transparent window backgrounds.");
        if (options.Appearance.TitleBar.Background == WindowTitleBarBackground.Backdrop &&
            options.Appearance.Backdrop.Mode != Desktop.WindowBackdropMode.Acrylic)
            return new(WindowSupport.Unsupported, "Backdrop caption requires Acrylic.");
        if (options.Appearance.TitleBar.Background != WindowTitleBarBackground.System &&
            !OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22621))
            return new(WindowSupport.Unsupported, "Custom caption backgrounds require Windows 11 build 22621.");
        if (current is not null && (options.Appearance.Backdrop != current.Appearance.Backdrop ||
            options.Appearance.BackgroundColor != current.Appearance.BackgroundColor ||
            options.Appearance.DarkBackgroundColor != current.Appearance.DarkBackgroundColor))
            return new(WindowSupport.RequiresRecreation, "Changing the raster background or backdrop requires recreation.");
        return WindowEvaluation.Supported;
    }
    public ValueTask<IWindowHost> CreateAsync(WindowId id, WindowCreateOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_allocated) throw new NotSupportedException("Additional native windows are not implemented.");
        _allocated = true;
        return ValueTask.FromResult<IWindowHost>(this);
    }
    public WindowState State { get; private set; }
    public Task ReadyToShow => _ready.Task;
    public event Action<WindowState>? StateChanged;
    public event Action? CloseRequested;
    public event Action? Closed;
    public Task InitializeAsync(WindowOptions options, DesktopWindowContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EvaluateOptions(options, null).ThrowIfUnsupported();
        _options = options;
        WindowContext = context;
        State = State with { ClientSize = options.Size, RequestedAppearance = options.Appearance,
            EffectiveAppearance = new(options.Appearance) };
        return Task.CompletedTask;
    }
    internal void Attach(WindowsManagedProductHost host)
    {
        _ui.VerifyThread();
        _host = host;
        _hwnd = host.TopLevelHwnd;
        if (!SetWindowSubclass(_hwnd, _procedure, 0xD073, 0)) throw new InvalidOperationException("Cannot attach Desktop native window policy.");
        SetWindowTextW(_hwnd, _options.Title);
        SetResizable(_options.Resizable);
        SetTopmost(_options.AlwaysOnTop);
        SetTaskbar(_options.SkipTaskbar);
        Resize(_options.Size);
        if (_options.Position is { } point) SetWindowPos(_hwnd, 0, (int)point.dx, (int)point.dy, 0, 0, 0x15);
        if (_options.Centered) Center();
        ApplyKind();
        ApplyCaption(_options.Appearance);
        Publish();
        _attached.TrySetResult();
    }
    internal void Presented() => _ready.TrySetResult();
    internal Task NativeAttachment => _attached.Task;

    public async Task<WindowState> ExecuteAsync(WindowCommand command, CancellationToken cancellationToken)
    {
        await _attached.Task.WaitAsync(cancellationToken);
        await _ui.InvokeAsync(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ObjectDisposedException.ThrowIf(_disposed || _acceptedClose, this);
            switch (command.Kind)
            {
                case WindowCommandKind.Show:
                    VisibilityRequested = true;
                    if (_options.Modal && NativeOwner != 0 && !_ownerDisabled) { lock (ModalGate) { ModalOwners[NativeOwner] = ModalOwners.GetValueOrDefault(NativeOwner) + 1; EnableWindow(NativeOwner, false); _ownerDisabled = true; } }
                    _host!.Show();
                    if (_options.PresentationState == WindowPresentationState.FullScreen) FullScreen(true);
                    ShowWindow(_hwnd, _options.PresentationState == WindowPresentationState.Maximized ? 3 :
                        _options.PresentationState == WindowPresentationState.Minimized ? 2 : _options.Kind == WindowKind.Tooltip || !_options.Activate ? 4 : 5);
                    break;
                case WindowCommandKind.Hide: ShowWindow(_hwnd, 0); RestoreOwner(); break;
                case WindowCommandKind.Focus: SetForegroundWindow(_hwnd); break;
                case WindowCommandKind.Title:
                    _options = _options with { Title = (string)command.Value! };
                    SetWindowTextW(_hwnd, _options.Title); break;
                case WindowCommandKind.Size:
                    _options = _options with { Size = (Size)command.Value! };
                    Resize(_options.Size); break;
                case WindowCommandKind.Bounds:
                    var bounds = (Rect)command.Value!;
                    SetWindowPos(_hwnd, 0, (int)bounds.left, (int)bounds.top, (int)bounds.width, (int)bounds.height, 0x14);
                    break;
                case WindowCommandKind.Center: Center(); break;
                case WindowCommandKind.MinimumSize: _options = _options with { MinimumSize = (Size?)command.Value }; break;
                case WindowCommandKind.MaximumSize: _options = _options with { MaximumSize = (Size?)command.Value }; break;
                case WindowCommandKind.Resizable:
                    _options = _options with { Resizable = (bool)command.Value! };
                    SetResizable(_options.Resizable); break;
                case WindowCommandKind.AlwaysOnTop:
                    _options = _options with { AlwaysOnTop = (bool)command.Value! };
                    SetTopmost(_options.AlwaysOnTop); break;
                case WindowCommandKind.SkipTaskbar:
                    _options = _options with { SkipTaskbar = (bool)command.Value! };
                    SetTaskbar(_options.SkipTaskbar); break;
                case WindowCommandKind.Minimize: ShowWindow(_hwnd, 6); break;
                case WindowCommandKind.Maximize: ShowWindow(_hwnd, 3); break;
                case WindowCommandKind.Restore: FullScreen(false); ShowWindow(_hwnd, 9); break;
                case WindowCommandKind.FullScreen: FullScreen((bool)command.Value!); break;
                case WindowCommandKind.Drag: ReleaseCapture(); PostMessageW(_hwnd, 0x00A1, 2, 0); break;
                case WindowCommandKind.Resize:
                    ReleaseCapture();
                    var hit = (WindowResizeEdge)command.Value! switch { WindowResizeEdge.Left => 10, WindowResizeEdge.Right => 11,
                        WindowResizeEdge.Top => 12, WindowResizeEdge.TopLeft => 13, WindowResizeEdge.TopRight => 14,
                        WindowResizeEdge.Bottom => 15, WindowResizeEdge.BottomLeft => 16, _ => 17 };
                    PostMessageW(_hwnd, 0x00A1, (nuint)hit, 0); break;
                default: throw new NotSupportedException(command.Kind.ToString());
            }
            Publish();
            return ValueTask.CompletedTask;
        });
        return State;
    }
    public async Task<WindowState> ApplyAppearanceAsync(Appearance appearance, CancellationToken cancellationToken)
    {
        EvaluateOptions(_options with { Appearance = appearance }, _options).ThrowIfUnsupported();
        await _attached.Task.WaitAsync(cancellationToken);
        await _ui.InvokeAsync(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ApplyCaption(appearance);
            _options = _options with { Appearance = appearance };
            Publish();
            return ValueTask.CompletedTask;
        });
        return State;
    }
    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        await _attached.Task.WaitAsync(cancellationToken);
        await _ui.InvokeAsync(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            _acceptedClose = true;
            if (_menuActive) EndMenu();
            RemoveMenuBar();
            _host!.Close(); // Existing native WM_CLOSE retires the render worker and platform resources.
            return ValueTask.CompletedTask;
        });
        await _closed.Task;
    }
    internal void CompleteShutdown(Exception? error)
    {
        _ui.VerifyThread();
        if (_disposed) return;
        _disposed = true;
        _ownerSubscription?.Dispose(); _ownerSubscription = null;
        RestoreOwner();
        _attached.TrySetException(error ?? new ObjectDisposedException(nameof(WindowsAppSdkDesktopWindowHost)));
        _ready.TrySetException(error ?? new ObjectDisposedException(nameof(WindowsAppSdkDesktopWindowHost)));
        _ui.DrainShutdown(Task.CompletedTask);
        _ui.Dispose();
        State = State with { Closed = true, Visible = false, Focused = false, Revision = State.Revision + 1 };
        if (error is null) _closed.TrySetResult(); else _closed.TrySetException(error);
        Closed?.Invoke();
    }
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        if (_hwnd != 0) await CloseAsync(CancellationToken.None);
        else CompleteShutdown(null);
    }
    private void RestoreOwner()
    {
        if (!_ownerDisabled) return;
        lock (ModalGate)
        {
            _ownerDisabled = false;
            var remaining = ModalOwners.GetValueOrDefault(NativeOwner) - 1;
            if (remaining > 0) { ModalOwners[NativeOwner] = remaining; return; }
            ModalOwners.Remove(NativeOwner);
            if (IsWindow(NativeOwner)) { EnableWindow(NativeOwner, true); SetForegroundWindow(NativeOwner); }
        }
    }
    private void ApplyKind()
    {
        if (NativeOwner != 0) SetWindowLongPtrW(_hwnd, -8, NativeOwner);
        if (_options.Kind is WindowKind.Popup or WindowKind.Tooltip)
        {
            var style = GetWindowLongPtrW(_hwnd, -16).ToInt64();
            SetWindowLongPtrW(_hwnd, -16, (nint)((style & ~0x00CF0000L) | 0x80000000L));
        }
        if (_options.Kind != WindowKind.Regular)
        {
            var style = GetWindowLongPtrW(_hwnd, -20).ToInt64();
            style = (style | 0x80) & ~0x40000L;
            if (_options.Kind == WindowKind.Tooltip || !_options.Activate) style |= 0x08000000;
            SetWindowLongPtrW(_hwnd, -20, (nint)style);
        }
        if (_options.Kind == WindowKind.Tooltip) EnableWindow(_hwnd, false);
        SetWindowPos(_hwnd, 0, 0, 0, 0, 0, 0x37);
        PositionAnchor();
        if (_options.Anchor is not null && WindowContext is { } context && _options.OwnerWindowId is { } owner && context.Windows.TryGetWindow(owner, out var controller))
            _ownerSubscription = controller!.Subscribe(state => { _ = RepositionAsync(); });
    }
    private async Task RepositionAsync()
    {
        try { await _ui.InvokeAsync(() => { if (!_disposed) PositionAnchor(); return ValueTask.CompletedTask; }); }
        catch (ObjectDisposedException) { }
    }
    private void PositionAnchor()
    {
        if (_options.Anchor is not { } anchor || NativeOwner == 0 || !IsWindow(NativeOwner)) return;
        var scale = GetDpiForWindow(NativeOwner) / 96.0;
        var point = new MutablePoint { X = (int)Math.Round(anchor.LogicalBounds.left * scale), Y = (int)Math.Round(anchor.LogicalBounds.bottom * scale) };
        if (!ClientToScreen(NativeOwner, ref point)) throw new InvalidOperationException("Cannot resolve the owner anchor.");
        var monitor = new MonitorInfo { Size = Marshal.SizeOf<MonitorInfo>() };
        if (!GetMonitorInfoW(MonitorFromWindow(NativeOwner, 2), ref monitor) || !GetWindowRect(_hwnd, out var bounds)) throw new InvalidOperationException("Cannot resolve the owner's monitor work area.");
        var width = bounds.Right - bounds.Left; var height = bounds.Bottom - bounds.Top;
        point.X = Math.Clamp(point.X, monitor.Work.Left, Math.Max(monitor.Work.Left, monitor.Work.Right - width));
        point.Y = Math.Clamp(point.Y, monitor.Work.Top, Math.Max(monitor.Work.Top, monitor.Work.Bottom - height));
        SetWindowPos(_hwnd, 0, point.X, point.Y, 0, 0, 0x15);
    }
    [StructLayout(LayoutKind.Sequential)] private struct MutablePoint { public int X, Y; }
    [DllImport("user32.dll")] private static extern bool EnableWindow(nint window, bool enable);
    [DllImport("user32.dll")] private static extern bool IsWindow(nint window);
    [DllImport("user32.dll")] private static extern bool ClientToScreen(nint window, ref MutablePoint point);
    private void Resize(Size size)
    {
        var scale = GetDpiForWindow(_hwnd) / 96d;
        var rect = new NativeRect { Right = (int)Math.Round(size.width * scale), Bottom = (int)Math.Round(size.height * scale) };
        AdjustWindowRectExForDpi(ref rect, (uint)GetWindowLongPtrW(_hwnd, -16), false, (uint)GetWindowLongPtrW(_hwnd, -20), GetDpiForWindow(_hwnd));
        SetWindowPos(_hwnd, 0, 0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top, 0x16);
    }
    private void Center()
    {
        var monitor = new MonitorInfo { Size = Marshal.SizeOf<MonitorInfo>() };
        GetMonitorInfoW(MonitorFromWindow(_hwnd, 2), ref monitor);
        GetWindowRect(_hwnd, out var bounds);
        SetWindowPos(_hwnd, 0, monitor.Work.Left + (monitor.Work.Right - monitor.Work.Left - bounds.Right + bounds.Left) / 2,
            monitor.Work.Top + (monitor.Work.Bottom - monitor.Work.Top - bounds.Bottom + bounds.Top) / 2, 0, 0, 0x15);
    }
    private void SetResizable(bool value)
    {
        var style = GetWindowLongPtrW(_hwnd, -16).ToInt64();
        SetWindowLongPtrW(_hwnd, -16, (nint)(value ? style | 0x50000 : style & ~0x50000));
        SetWindowPos(_hwnd, 0, 0, 0, 0, 0, 0x37);
    }
    private void SetTopmost(bool value) => SetWindowPos(_hwnd, value ? -1 : -2, 0, 0, 0, 0, 0x13);
    private void SetTaskbar(bool skip)
    {
        var style = GetWindowLongPtrW(_hwnd, -20).ToInt64();
        SetWindowLongPtrW(_hwnd, -20, (nint)(skip ? (style | 0x80) & ~0x40000 : (style & ~0x80) | 0x40000));
        SetWindowPos(_hwnd, 0, 0, 0, 0, 0, 0x37);
    }
    private void FullScreen(bool enabled)
    {
        if (_fullScreen == enabled) return;
        _fullScreen = enabled;
        if (enabled)
        {
            GetWindowRect(_hwnd, out _restoredBounds);
            _restoredStyle = GetWindowLongPtrW(_hwnd, -16);
            var info = new MonitorInfo { Size = Marshal.SizeOf<MonitorInfo>() };
            GetMonitorInfoW(MonitorFromWindow(_hwnd, 2), ref info);
            SetWindowLongPtrW(_hwnd, -16, (nint)(_restoredStyle.ToInt64() & ~0xCF0000));
            SetWindowPos(_hwnd, 0, info.Monitor.Left, info.Monitor.Top, info.Monitor.Right - info.Monitor.Left, info.Monitor.Bottom - info.Monitor.Top, 0x34);
        }
        else
        {
            SetWindowLongPtrW(_hwnd, -16, _restoredStyle);
            SetWindowPos(_hwnd, 0, _restoredBounds.Left, _restoredBounds.Top, _restoredBounds.Right - _restoredBounds.Left, _restoredBounds.Bottom - _restoredBounds.Top, 0x34);
        }
    }
    private void ApplyCaption(Appearance appearance)
    {
        var dark = appearance.ThemeSource == WindowThemeSource.System
            ? (_host?.Configuration.platformBrightness == Brightness.dark ? 1 : 0)
            : (appearance.Theme == WindowTheme.Dark ? 1 : 0);
        Marshal.ThrowExceptionForHR(DwmSetWindowAttribute(_hwnd, 20, ref dark, 4));
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22621))
        {
            var color = appearance.TitleBar.BackgroundColor ?? appearance.BackgroundColor;
            var caption = appearance.TitleBar.Background == WindowTitleBarBackground.Solid
                ? (int)((color.value & 255) << 16 | (color.value & 0xff00) | ((color.value >> 16) & 255)) : -1;
            Marshal.ThrowExceptionForHR(DwmSetWindowAttribute(_hwnd, 35, ref caption, 4));
        }
    }
    private void Publish()
    {
        if (_hwnd == 0 || _disposed) return;
        GetWindowRect(_hwnd, out var outer);
        GetClientRect(_hwnd, out var client);
        var scale = GetDpiForWindow(_hwnd) / 96d;
        State = new(Rect.fromLTWH(outer.Left, outer.Top, outer.Right - outer.Left, outer.Bottom - outer.Top),
            new Size(client.Right / scale, client.Bottom / scale), scale, IsWindowVisible(_hwnd), GetForegroundWindow() == _hwnd,
            _fullScreen ? WindowPresentationState.FullScreen : IsIconic(_hwnd) ? WindowPresentationState.Minimized :
                IsZoomed(_hwnd) ? WindowPresentationState.Maximized : WindowPresentationState.Normal,
            _options.Appearance, new(_options.Appearance), new((outer.Bottom - outer.Top - client.Bottom) / scale, ViewPadding.zero, 0, 0, State.Revision + 1), State.Revision + 1);
        WasVisible |= State.Visible;
        StateChanged?.Invoke(State);
    }
    private nint WindowProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint id, nuint data)
    {
        try
        {
            // The renderer routes WM_CANCELMODE through its pointer capture handler.
            // Explicitly end the OS menu loop on its owning thread before that routing.
            if (message == 0x001F && _menuActive) EndMenu();
            if (HandleMenuBarMessage(message, wparam, lparam)) return 0;
            if (message == 0x0010 && !_acceptedClose) { CloseRequested?.Invoke(); return 0; }
            if (_options.Kind == WindowKind.Tooltip && message == 0x0021) return 4;
            if (_options.Kind == WindowKind.Popup && State.Visible && !_acceptedClose &&
                (message == 0x0100 && wparam == 0x1B || message == 0x0006 && (wparam & 0xffff) == 0))
            { CloseRequested?.Invoke(); return 0; }
            var result = DefSubclassProc(hwnd, message, wparam, lparam);
            if (message == 0x0024)
            {
                var info = Marshal.PtrToStructure<MinMaxInfo>(lparam);
                GetWindowRect(hwnd, out var outer); GetClientRect(hwnd, out var client);
                var dx = outer.Right - outer.Left - client.Right; var dy = outer.Bottom - outer.Top - client.Bottom;
                var scale = GetDpiForWindow(hwnd) / 96d;
                if (_options.MinimumSize is { } min) info.MinTrack = new((int)Math.Ceiling(min.width * scale) + dx, (int)Math.Ceiling(min.height * scale) + dy);
                if (_options.MaximumSize is { } max) info.MaxTrack = new((int)Math.Floor(max.width * scale) + dx, (int)Math.Floor(max.height * scale) + dy);
                Marshal.StructureToPtr(info, lparam, false);
            }
            if (message is 0x0005 or 0x0006 or 0x0047 or 0x02E0) Publish();
            if (message == 0x0082) { RemoveMenuBar(); RemoveWindowSubclass(hwnd, _procedure, 0xD073); _hwnd = 0; }
            return result;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 0; }
    }
    [StructLayout(LayoutKind.Sequential)] private struct NativeRect { public int Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)] private record struct Point(int X, int Y);
    [StructLayout(LayoutKind.Sequential)] private struct MinMaxInfo { public Point Reserved, MaxSize, MaxPosition, MinTrack, MaxTrack; }
    [StructLayout(LayoutKind.Sequential)] private struct MonitorInfo { public int Size; public NativeRect Monitor, Work; public uint Flags; }
    private delegate nint SubclassProc(nint h, uint m, nuint w, nint l, nuint id, nuint data);
    [DllImport("comctl32.dll")] private static extern bool SetWindowSubclass(nint h, SubclassProc callback, nuint id, nuint data);
    [DllImport("comctl32.dll")] private static extern bool RemoveWindowSubclass(nint h, SubclassProc callback, nuint id);
    [DllImport("comctl32.dll")] private static extern nint DefSubclassProc(nint h, uint m, nuint w, nint l);
    [DllImport("user32.dll")] private static extern bool SetWindowPos(nint h, nint after, int x, int y, int w, int ht, uint flags);
    [DllImport("user32.dll")] private static extern bool ShowWindow(nint h, int command);
    [DllImport("user32.dll")] private static extern bool SetForegroundWindow(nint h);
    [DllImport("user32.dll")] private static extern nint GetForegroundWindow();
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern bool SetWindowTextW(nint h, string title);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(nint h, out NativeRect r);
    [DllImport("user32.dll")] private static extern bool GetClientRect(nint h, out NativeRect r);
    [DllImport("user32.dll")] private static extern uint GetDpiForWindow(nint h);
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(nint h);
    [DllImport("user32.dll")] private static extern bool IsIconic(nint h);
    [DllImport("user32.dll")] private static extern bool IsZoomed(nint h);
    [DllImport("user32.dll")] private static extern nint GetWindowLongPtrW(nint h, int index);
    [DllImport("user32.dll")] private static extern nint SetWindowLongPtrW(nint h, int index, nint value);
    [DllImport("user32.dll")] private static extern bool AdjustWindowRectExForDpi(ref NativeRect rect, uint style, bool menu, uint exstyle, uint dpi);
    [DllImport("user32.dll")] private static extern nint MonitorFromWindow(nint h, uint flags);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern bool GetMonitorInfoW(nint monitor, ref MonitorInfo info);
    [DllImport("user32.dll")] private static extern bool ReleaseCapture();
    [DllImport("user32.dll")] private static extern bool PostMessageW(nint h, uint message, nuint w, nint l);
    [DllImport("dwmapi.dll")] private static extern int DwmSetWindowAttribute(nint h, int attribute, ref int value, int size);
}
