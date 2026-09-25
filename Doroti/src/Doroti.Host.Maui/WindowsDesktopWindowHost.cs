#if WINDOWS
using System.Runtime.InteropServices;
using Doroti.Desktop;
using Doroti.Hosting;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using DSize = Doroti.Ui.Size;
using DRect = Doroti.Ui.Rect;
using Appearance = Doroti.Desktop.WindowAppearanceOptions;
using NativeWindow = Microsoft.UI.Xaml.Window;
using Window = Microsoft.Maui.Controls.Window;

namespace Doroti.Host.Maui;

/// <summary>Dedicated full-window runner adapter. Embedded surfaces are never implicitly authorized.</summary>
internal sealed class WindowsDesktopWindowHost : IWindowHost
{
    private readonly Window _window;
    private readonly DorotiApplicationDescriptor _descriptor;
    private readonly TaskCompletionSource _attached = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    private readonly TaskCompletionSource _ready = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    private readonly SubclassProc _procedure;
    private DorotiMauiSurface? _surface;
    private NativeWindow? _native;
    private nint _handle;
    private WindowOptions _options = new();
    private WindowsWindowBackdrop? _backdrop;
    private bool _revealed;
    private bool _initialPresentationApplied;
    private bool _destroying;
    private bool _disposed;
    private bool _resizing;
    private TaskCompletionSource? _resizeEnded;
    private long _revision;

    private WindowsDesktopWindowHost(Window window, DorotiApplicationDescriptor descriptor)
    {
        _window = window;
        _descriptor = descriptor;
        _procedure = WindowProc;
        State = new(
            null,
            new(800, 600),
            1,
            false,
            false,
            WindowPresentationState.Normal,
            new(),
            new(new()),
            new(0, Doroti.Ui.ViewPadding.zero, 0, 0, 0)
        );
    }

    internal static Window CreateMainWindow(
        DorotiApplicationDescriptor descriptor,
        DesktopApplicationDefinition definition
    )
    {
        EvaluateOptions(definition.MainWindow.Options, null).ThrowIfUnsupported();
        var window = new Window { Title = definition.MainWindow.Options.Title };
        var host = new WindowsDesktopWindowHost(window, descriptor);
        var manager = new DorotiWindowManager(new Factory(host), definition.LifetimePolicy);
        manager.ExitRequested += () =>
            window.Dispatcher.Dispatch(() => Microsoft.UI.Xaml.Application.Current.Exit());
        manager.InitializationFailed += (_, error) => DorotiMauiSurface.WriteFailure(error);
        // Initialization assigns Page before yielding for the native handler. No UI-thread wait.
        _ = StartAsync();
        return window;
        async Task StartAsync()
        {
            try
            {
                await manager.CreateMainWindowAsync(definition.MainWindow);
            }
            catch (Exception error)
            {
                DorotiMauiSurface.WriteFailure(error);
                await host.DisposeAsync();
            }
        }
    }

    private sealed class Factory(WindowsDesktopWindowHost host) : IWindowHostFactory
    {
        public WindowManagerCapabilities Capabilities { get; } = new(false);

        public WindowEvaluation Evaluate(WindowCreateOptions options) =>
            EvaluateOptions(options.Options, null);

        public ValueTask<IWindowHost> CreateAsync(
            Doroti.Desktop.WindowId id,
            WindowCreateOptions options,
            CancellationToken cancellationToken
        ) => ValueTask.FromResult<IWindowHost>(host);
    }

    internal static void PrepareNativeWindow(NativeWindow window)
    {
        // DWM continues composing while cloaked; WinUI can load and render its
        // first frame without exposing the default geometry/caption to the user.
        // This is not a timer or an off-screen substitute for a completed frame.
        SetDwm(WinRT.Interop.WindowNative.GetWindowHandle(window), 13, 1);
        window.AppWindow.IsShownInSwitchers = false;
        WindowsNativeCaption.Enable(window);
    }

    private static WindowEvaluation EvaluateOptions(WindowOptions options, WindowOptions? current)
    {
        options.Validate();
        var appearance = options.Appearance;
        if (
            appearance.TitleBar.Style != WindowTitleBarStyle.Normal
            || appearance.TitleBar.Frame != WindowFrame.Standard
            || appearance.TitleBar.Buttons != WindowCaptionButtonMode.Native
        )
            return new(
                WindowSupport.Unsupported,
                "Hidden/custom chrome awaits native input and accessibility qualification."
            );
        if (appearance.ThemeSource == WindowThemeSource.App)
            return new(
                WindowSupport.Unsupported,
                "App theme bridge is not yet connected on this host; select System or Explicit."
            );
        if (appearance.Backdrop.Mode == Desktop.WindowBackdropMode.LiquidGlass)
            return new(WindowSupport.Unsupported, "Liquid Glass is an AppKit material.");
        if (
            appearance.Backdrop.Mode != Desktop.WindowBackdropMode.Acrylic
            && (
                appearance.Backdrop.TintColor is not null
                || appearance.Backdrop.TintOpacity is not null
                || appearance.Backdrop.LuminosityOpacity is not null
                || appearance.Backdrop.AcrylicKind != Desktop.WindowAcrylicKind.Default
            )
        )
            return new(WindowSupport.Unsupported, "Acrylic tint/kind requires Acrylic mode.");
        if (
            appearance.TitleBar.Background == WindowTitleBarBackground.Backdrop
            && appearance.Backdrop.Mode != Desktop.WindowBackdropMode.Acrylic
        )
            return new(WindowSupport.Unsupported, "Backdrop caption requires Acrylic.");
        if (
            appearance.TitleBar.Background != WindowTitleBarBackground.System
            && !OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22621)
        )
            return new(
                WindowSupport.Unsupported,
                "Caption material/color requires Windows 11 build 22621."
            );
        if (
            current is not null
            && (
                current.Appearance.Backdrop.Mode != appearance.Backdrop.Mode
                || current.Appearance.BackgroundColor != appearance.BackgroundColor
                || current.Appearance.DarkBackgroundColor != appearance.DarkBackgroundColor
            )
        )
            return new(
                WindowSupport.RequiresRecreation,
                "Changing renderer base transparency/material topology requires explicit recreation."
            );
        return WindowEvaluation.Supported;
    }

    public WindowCapabilities Capabilities { get; } = new(EvaluateOptions);
    public WindowState State { get; private set; }
    public Task ReadyToShow => _ready.Task;
    public event Action<WindowState>? StateChanged;
    public event Action? CloseRequested;
    public event Action? Closed;

    public async Task InitializeAsync(
        WindowOptions options,
        DesktopWindowContext context,
        IDorotiViewEntrypoint content,
        CancellationToken cancellationToken
    )
    {
        _options = options;
        _window.HandlerChanged += HandleHandlerChanged;
        var descriptor = _descriptor with { EntrypointFactory = () => content };
        _surface = new(descriptor) { OwnsWindowContent = true, DesktopManaged = true };
        _surface.DesktopFrameReady += OnFrameReady;
        _surface.DesktopFrameFailed += OnFrameFailed;
        _surface.Loaded += HandleContentLoaded;
        _window.Page = new ContentPage
        {
            BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent,
            SafeAreaEdges = Microsoft.Maui.SafeAreaEdges.None,
            Content = _surface,
        };
        HandleHandlerChanged(null, EventArgs.Empty);
        await _attached.Task.WaitAsync(cancellationToken);
    }

    private void HandleHandlerChanged(object? sender, EventArgs args)
    {
        if (_native is not null || _window.Handler?.PlatformView is not NativeWindow native)
            return;
        try
        {
            _native = native;
            _handle = WinRT.Interop.WindowNative.GetWindowHandle(native);
            PrepareNativeWindow(native);
            if (!SetWindowSubclass(_handle, _procedure, 0xD070, 0))
                throw new System.ComponentModel.Win32Exception();
            native.AppWindow.Closing += HandleClosing;
            native.AppWindow.Changed += HandleChanged;
            native.Activated += HandleActivated;
            native.Closed += HandleClosed;
            native.Title = _options.Title;
            native.AppWindow.Title = _options.Title;
            var presenter = Overlapped;
            presenter.IsAlwaysOnTop = _options.AlwaysOnTop;
            presenter.IsResizable = _options.Resizable;
            ApplyAppearance(_options.Appearance);
            if (_options.Position is { } position)
                native.AppWindow.Move(new(checked((int)position.dx), checked((int)position.dy)));
            ResizeClient(_options.Size);
            if (_options.Centered)
                Center();
            switch (_options.PresentationState)
            {
                case WindowPresentationState.Maximized:
                    presenter.Maximize();
                    break;
                case WindowPresentationState.FullScreen:
                    native.AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                    break;
            }
            Publish();
        }
        catch (Exception error)
        {
            _attached.TrySetException(error);
            _ready.TrySetException(error);
        }
    }

    private void HandleContentLoaded(object? sender, EventArgs args)
    {
        if (_native is null || _attached.Task.IsCompleted)
            return;
        try
        {
            // MAUI's OnWindowCreated/title mapping runs after HandlerChanged.
            // Reassert the one snapshot after its root exists, before first show.
            _native.Title = _options.Title;
            _native.AppWindow.Title = _options.Title;
            WindowsNativeCaption.KeepMauiCaptionCollapsed(_native);
            ApplyAppearance(_options.Appearance);
            Publish();
            _attached.TrySetResult();
        }
        catch (Exception error)
        {
            _attached.TrySetException(error);
            _ready.TrySetException(error);
        }
    }

    private OverlappedPresenter Overlapped =>
        _native?.AppWindow.Presenter as OverlappedPresenter
        ?? throw new NotSupportedException(
            "This command requires an overlapped window; restore from full screen first."
        );
    private double Scale => Math.Max(96, GetDpiForWindow(_handle)) / 96.0;

    private void ResizeClient(DSize size)
    {
        _options = _options with { Size = size };
        _native!.AppWindow.ResizeClient(
            new(
                checked((int)Math.Round(size.width * Scale)),
                checked((int)Math.Round(size.height * Scale))
            )
        );
    }

    private void Center()
    {
        var app = _native!.AppWindow;
        var area = DisplayArea.GetFromWindowId(app.Id, DisplayAreaFallback.Nearest).WorkArea;
        app.Move(
            new(
                area.X + (area.Width - app.Size.Width) / 2,
                area.Y + (area.Height - app.Size.Height) / 2
            )
        );
    }

    public Task<WindowState> ExecuteAsync(
        WindowCommand command,
        CancellationToken cancellationToken
    ) =>
        OnUiAsync(
            () =>
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                var app = _native!.AppWindow;
                WindowsNativeCaption.KeepMauiCaptionCollapsed(_native);
                switch (command.Kind)
                {
                    case WindowCommandKind.Show:
                        if (!_revealed)
                        {
                            SetDwm(_handle, 13, 0);
                            _revealed = true;
                        }
                        app.IsShownInSwitchers = !_options.SkipTaskbar;
                        app.Show(false);
                        if (
                            !_initialPresentationApplied
                            && _options.PresentationState == WindowPresentationState.Minimized
                        )
                            Overlapped.Minimize();
                        _initialPresentationApplied = true;
                        break;
                    case WindowCommandKind.Hide:
                        app.Hide();
                        break;
                    case WindowCommandKind.Focus:
                        _native.Activate();
                        break;
                    case WindowCommandKind.Title:
                        _options = _options with { Title = (string)command.Value! };
                        app.Title = _options.Title;
                        _native.Title = _options.Title;
                        break;
                    case WindowCommandKind.Size:
                        ResizeClient((DSize)command.Value!);
                        break;
                    case WindowCommandKind.Bounds:
                        var b = (DRect)command.Value!;
                        app.MoveAndResize(
                            new(
                                checked((int)b.left),
                                checked((int)b.top),
                                checked((int)b.width),
                                checked((int)b.height)
                            )
                        );
                        break;
                    case WindowCommandKind.Center:
                        Center();
                        break;
                    case WindowCommandKind.MinimumSize:
                        _options = _options with { MinimumSize = (DSize?)command.Value };
                        break;
                    case WindowCommandKind.MaximumSize:
                        _options = _options with { MaximumSize = (DSize?)command.Value };
                        break;
                    case WindowCommandKind.AlwaysOnTop:
                        Overlapped.IsAlwaysOnTop = (bool)command.Value!;
                        _options = _options with { AlwaysOnTop = (bool)command.Value! };
                        break;
                    case WindowCommandKind.Resizable:
                        Overlapped.IsResizable = (bool)command.Value!;
                        _options = _options with { Resizable = (bool)command.Value! };
                        break;
                    case WindowCommandKind.SkipTaskbar:
                        _options = _options with { SkipTaskbar = (bool)command.Value! };
                        app.IsShownInSwitchers = _revealed && !_options.SkipTaskbar;
                        break;
                    case WindowCommandKind.Minimize:
                        Overlapped.Minimize();
                        break;
                    case WindowCommandKind.Maximize:
                        Overlapped.Maximize();
                        break;
                    case WindowCommandKind.Restore:
                        if (app.Presenter.Kind == AppWindowPresenterKind.FullScreen)
                            app.SetPresenter(AppWindowPresenterKind.Overlapped);
                        Overlapped.Restore();
                        break;
                    case WindowCommandKind.FullScreen:
                        app.SetPresenter(
                            (bool)command.Value!
                                ? AppWindowPresenterKind.FullScreen
                                : AppWindowPresenterKind.Overlapped
                        );
                        break;
                    case WindowCommandKind.Drag:
                    case WindowCommandKind.Resize:
                        // OS move loops require live pointer authorization, not arbitrary queued invocation.
                        if ((GetKeyState(1) & 0x8000) == 0)
                            throw new InvalidOperationException(
                                "System drag/resize requires the pressed primary pointer."
                            );
                        var hit =
                            command.Kind == WindowCommandKind.Drag ? 2
                            : command.Value is WindowResizeEdge edge
                                ? edge switch
                                {
                                    WindowResizeEdge.Left => 10,
                                    WindowResizeEdge.Right => 11,
                                    WindowResizeEdge.Top => 12,
                                    WindowResizeEdge.TopLeft => 13,
                                    WindowResizeEdge.TopRight => 14,
                                    WindowResizeEdge.Bottom => 15,
                                    WindowResizeEdge.BottomLeft => 16,
                                    WindowResizeEdge.BottomRight => 17,
                                    _ => throw new ArgumentOutOfRangeException(nameof(command)),
                                }
                            : throw new ArgumentException("Resize edge missing.");
                        ReleaseCapture();
                        SendMessage(_handle, 0x00A1, hit, 0);
                        break;
                    default:
                        throw new NotSupportedException(command.Kind.ToString());
                }
                Publish();
                return State;
            },
            cancellationToken
        );

    public async Task<WindowState> ApplyAppearanceAsync(
        Appearance appearance,
        CancellationToken cancellationToken
    )
    {
        var pendingResize = await OnUiAsync(
            () =>
                _resizing
                    ? (
                        _resizeEnded ??= new(TaskCreationOptions.RunContinuationsAsynchronously)
                    ).Task
                    : Task.CompletedTask,
            cancellationToken
        );
        await pendingResize.WaitAsync(cancellationToken);
        return await OnUiAsync(
            () =>
            {
                ApplyAppearance(appearance);
                Publish();
                return State;
            },
            cancellationToken
        );
    }

    private void ApplyAppearance(Appearance appearance)
    {
        var old = _backdrop;
        old?.Dispose();
        _backdrop = new(_native!, appearance);
        _options = _options with { Appearance = appearance };
    }

    private void Publish()
    {
        if (_native is null || _disposed)
            return;
        GetWindowRect(_handle, out var outer);
        GetClientRect(_handle, out var client);
        var clientOrigin = new Point(0, 0);
        ClientToScreen(_handle, ref clientOrigin);
        var app = _native.AppWindow;
        var appearance = _options.Appearance;
        var fallback =
            appearance.Backdrop.Mode == Desktop.WindowBackdropMode.Acrylic
            && (
                !Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported()
                || new Windows.UI.ViewManagement.AccessibilitySettings().HighContrast
                || !new Windows.UI.ViewManagement.UISettings().AdvancedEffectsEnabled
            );
        var effectiveMode =
            !Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported()
            && appearance.Backdrop.Fallback == Desktop.WindowBackdropFallback.Transparent
                ? Desktop.WindowBackdropMode.Transparent
                : Desktop.WindowBackdropMode.Solid;
        var effective = fallback
            ? appearance with
            {
                Backdrop = appearance.Backdrop with { Mode = effectiveMode },
            }
            : appearance;
        var presentation =
            app.Presenter.Kind == AppWindowPresenterKind.FullScreen
                ? WindowPresentationState.FullScreen
            : app.Presenter is OverlappedPresenter p
                ? p.State switch
                {
                    OverlappedPresenterState.Maximized => WindowPresentationState.Maximized,
                    OverlappedPresenterState.Minimized => WindowPresentationState.Minimized,
                    _ => WindowPresentationState.Normal,
                }
            : WindowPresentationState.Normal;
        State = new(
            DRect.fromLTRB(outer.Left, outer.Top, outer.Right, outer.Bottom),
            new(client.Right / Scale, client.Bottom / Scale),
            Scale,
            _revealed && IsWindowVisible(_handle),
            GetForegroundWindow() == _handle && _revealed,
            presentation,
            appearance,
            new(
                effective,
                fallback,
                appearance.TitleBar.Background == WindowTitleBarBackground.Backdrop
                    ? "Native DWM caption uses the OS Acrylic variant; tint is independent of client material."
                    : null
            ),
            new(
                Math.Max(0, (clientOrigin.Y - outer.Top) / Scale),
                Doroti.Ui.ViewPadding.zero,
                0,
                0,
                ++_revision
            ),
            _revision
        );
        StateChanged?.Invoke(State);
    }

    private void OnFrameReady() => _ready.TrySetResult();

    private void OnFrameFailed(Exception error) => _ready.TrySetException(error);

    private void HandleChanged(AppWindow sender, AppWindowChangedEventArgs args) => Publish();

    private void HandleActivated(object sender, WindowActivatedEventArgs args) => Publish();

    private void HandleClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (_destroying)
            return;
        args.Cancel = true;
        CloseRequested?.Invoke();
    }

    private void HandleClosed(object sender, WindowEventArgs args)
    {
        if (_destroying)
            return;
        _destroying = true;
        Closed?.Invoke();
    }

    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        var retirement = await OnUiAsync(
            () => _surface?.PrepareDesktopCloseAsync() ?? Task.CompletedTask,
            cancellationToken
        );
        await retirement.WaitAsync(cancellationToken);
        await OnUiAsync(
            () =>
            {
                _destroying = true;
                Cleanup(); // Release COM/material/subclass owners while the HWND is still alive.
                _native?.Close();
                return true;
            },
            cancellationToken
        );
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;
        if (!_destroying)
            await CloseAsync(CancellationToken.None);
        else
            await OnUiAsync(
                () =>
                {
                    Cleanup();
                    return true;
                },
                CancellationToken.None
            );
    }

    private void Cleanup()
    {
        if (_disposed)
            return;
        _disposed = true;
        _window.HandlerChanged -= HandleHandlerChanged;
        _backdrop?.Dispose();
        if (_surface is { } surface)
        {
            surface.DesktopFrameReady -= OnFrameReady;
            surface.DesktopFrameFailed -= OnFrameFailed;
            surface.Loaded -= HandleContentLoaded;
            surface.Dispose();
        }
        if (_native is { } native)
        {
            native.AppWindow.Closing -= HandleClosing;
            native.AppWindow.Changed -= HandleChanged;
            native.Activated -= HandleActivated;
            native.Closed -= HandleClosed;
            RemoveWindowSubclass(_handle, _procedure, 0xD070);
            WindowsNativeCaption.Release(native);
        }
        _ready.TrySetException(new ObjectDisposedException(nameof(WindowsDesktopWindowHost)));
        _attached.TrySetException(new ObjectDisposedException(nameof(WindowsDesktopWindowHost)));
        _resizeEnded?.TrySetCanceled();
        _backdrop = null;
    }

    private Task<T> OnUiAsync<T>(Func<T> action, CancellationToken cancellationToken)
    {
        if (!_window.Dispatcher.IsDispatchRequired)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(action());
        }
        var completion = new TaskCompletionSource<T>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        if (
            !_window.Dispatcher.Dispatch(() =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    completion.TrySetResult(action());
                }
                catch (OperationCanceledException)
                {
                    completion.TrySetCanceled(cancellationToken);
                }
                catch (Exception error)
                {
                    completion.TrySetException(error);
                }
            })
        )
            completion.TrySetException(new ObjectDisposedException("Window dispatcher"));
        return completion.Task;
    }

    private nint WindowProc(
        nint hwnd,
        uint message,
        nuint wParam,
        nint lParam,
        nuint id,
        nuint data
    )
    {
        if (message == 0x0231)
            _resizing = true; // WM_ENTERSIZEMOVE
        if (message == 0x0232)
        {
            _resizing = false;
            _resizeEnded?.TrySetResult();
            _resizeEnded = null;
        }
        var result = DefSubclassProc(hwnd, message, wParam, lParam);
        if (message == 0x0024) // WM_GETMINMAXINFO: client DIP limits converted on this window's monitor.
        {
            var info = Marshal.PtrToStructure<MinMaxInfo>(lParam);
            GetWindowRect(hwnd, out var outer);
            GetClientRect(hwnd, out var client);
            var dx = outer.Right - outer.Left - client.Right;
            var dy = outer.Bottom - outer.Top - client.Bottom;
            if (_options.MinimumSize is { } min)
                info.MinTrack = new(
                    (int)Math.Ceiling(min.width * Scale) + dx,
                    (int)Math.Ceiling(min.height * Scale) + dy
                );
            if (_options.MaximumSize is { } max)
                info.MaxTrack = new(
                    (int)Math.Floor(max.width * Scale) + dx,
                    (int)Math.Floor(max.height * Scale) + dy
                );
            else
                info.MaxTrack = new(
                    Math.Max(info.MaxTrack.X, (int)Math.Ceiling(_options.Size.width * Scale) + dx),
                    Math.Max(info.MaxTrack.Y, (int)Math.Ceiling(_options.Size.height * Scale) + dy)
                );
            Marshal.StructureToPtr(info, lParam, false);
        }
        if (message is 0x02E0 or 0x001A)
            Publish(); // DPI and system policy changes.
        return result;
    }

    private static void SetDwm(nint hwnd, uint attribute, uint value) =>
        Marshal.ThrowExceptionForHR(DwmSetWindowAttribute(hwnd, attribute, in value, 4));

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left,
            Top,
            Right,
            Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private record struct Point(int X, int Y);

    [StructLayout(LayoutKind.Sequential)]
    private struct MinMaxInfo
    {
        public Point Reserved,
            MaxSize,
            MaxPosition,
            MinTrack,
            MaxTrack;
    }

    private delegate nint SubclassProc(
        nint hwnd,
        uint message,
        nuint wParam,
        nint lParam,
        nuint id,
        nuint data
    );

    [DllImport("comctl32.dll")]
    private static extern bool SetWindowSubclass(
        nint hwnd,
        SubclassProc proc,
        nuint id,
        nuint data
    );

    [DllImport("comctl32.dll")]
    private static extern bool RemoveWindowSubclass(nint hwnd, SubclassProc proc, nuint id);

    [DllImport("comctl32.dll")]
    private static extern nint DefSubclassProc(nint hwnd, uint message, nuint wParam, nint lParam);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(
        nint hwnd,
        uint attribute,
        in uint value,
        uint size
    );

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(nint hwnd);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(nint hwnd, out NativeRect rect);

    [DllImport("user32.dll")]
    private static extern bool GetClientRect(nint hwnd, out NativeRect rect);

    [DllImport("user32.dll")]
    private static extern bool ClientToScreen(nint hwnd, ref Point point);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(nint hwnd);

    [DllImport("user32.dll")]
    private static extern nint GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern short GetKeyState(int key);

    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    private static extern nint SendMessage(nint hwnd, uint message, nint wParam, nint lParam);
}
#endif
