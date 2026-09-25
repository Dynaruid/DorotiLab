#if MACOS
using AppKit;
using CoreGraphics;
using Doroti.Desktop;
using Doroti.Hosting;
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platforms.MacOS.Platform;
using DSize = Doroti.Ui.Size;
using Appearance = Doroti.Desktop.WindowAppearanceOptions;
using BackdropMode = Doroti.Desktop.WindowBackdropMode;
using Window = Microsoft.Maui.Controls.Window;

namespace Doroti.Host.Maui;

/// <summary>Owns one native AppKit window; embedded MAUI surfaces never enter this path.</summary>
internal sealed class AppKitDesktopWindowHost : IWindowHost
{
    internal sealed class DesktopWindow : Window
    {
        internal required AppKitDesktopWindowHost Host { get; init; }
    }

    private readonly DorotiApplicationDescriptor _descriptor;
    private readonly TaskCompletionSource _attached = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    private readonly TaskCompletionSource _ready = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    private readonly List<(NSNotificationCenter Center, NSObject Token)> _observers = [];
    private Window _window = null!;
    private NSWindow? _native;
    private WindowDelegate? _delegate;
    private ContentRoot? _root;
    private CaptionFill? _caption;
    private DorotiMauiSurface? _surface;
    private WindowOptions _options = new();
    private bool _destroying,
        _disposed,
        _shown,
        _applyingAppearance,
        _active;
    private bool _nativeClosed;
    private TaskCompletionSource? _presentation;
    private TaskCompletionSource? _miniaturization;
    private long _revision;
    private Appearance _effective = new();
    private string? _fallback;
    private bool _policyFallback;

    private AppKitDesktopWindowHost(DorotiApplicationDescriptor descriptor)
    {
        _descriptor = descriptor;
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
        AppKitDesktopWindowPolicy
            .Evaluate(definition.MainWindow.Options, null)
            .ThrowIfUnsupported();
        var host = new AppKitDesktopWindowHost(descriptor);
        var window = new DesktopWindow { Host = host, Title = definition.MainWindow.Options.Title };
        host._window = window;
        var manager = new DorotiWindowManager(new Factory(host), definition.LifetimePolicy);
        if (NSApplication.SharedApplication.Delegate is DorotiMacOSMauiApplication app)
            app.AttachDesktopManager(manager);
        manager.InitializationFailed += (_, error) => DorotiMauiSurface.WriteFailure(error);
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

    private sealed class Factory(AppKitDesktopWindowHost host) : IWindowHostFactory
    {
        public WindowManagerCapabilities Capabilities { get; } = new(false);

        public WindowEvaluation Evaluate(WindowCreateOptions options) =>
            AppKitDesktopWindowPolicy.Evaluate(options.Options, null);

        public ValueTask<IWindowHost> CreateAsync(
            Doroti.Desktop.WindowId id,
            WindowCreateOptions options,
            CancellationToken cancellationToken
        ) => ValueTask.FromResult<IWindowHost>(host);
    }

    public WindowCapabilities Capabilities { get; } = new(AppKitDesktopWindowPolicy.Evaluate);
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
        _surface = new(_descriptor with { EntrypointFactory = () => content })
        {
            DesktopManaged = true,
        };
        _surface.DesktopFrameReady += FrameReady;
        _surface.DesktopFrameFailed += FrameFailed;
        _window.Page = new ContentPage
        {
            BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent,
            SafeAreaEdges = Microsoft.Maui.SafeAreaEdges.None,
            Content = _surface,
        };
        await _attached.Task.WaitAsync(cancellationToken);
    }

    internal NSWindow CreateNative()
    {
        var style =
            NSWindowStyle.Titled
            | NSWindowStyle.Closable
            | NSWindowStyle.Miniaturizable
            | NSWindowStyle.FullSizeContentView;
        if (_options.Resizable)
            style |= NSWindowStyle.Resizable;
        _native = new DesktopNativeWindow(
            new CGRect(0, 0, _options.Size.width, _options.Size.height),
            style,
            NSBackingStore.Buffered,
            false
        );
        _native.Title = _options.Title;
        _native.CollectionBehavior |= NSWindowCollectionBehavior.FullScreenPrimary;
        _delegate = new(this);
        _native.Delegate = _delegate;
        _root = new(this) { WantsLayer = true };
        _caption = new CaptionFill { WantsLayer = true, Hidden = true };
        _root.AddSubview(_caption);
        _native.ContentView = _root;
        ResizeClient(_options.Size);
        SetLimits();
        _native.Level = _options.AlwaysOnTop ? NSWindowLevel.Floating : NSWindowLevel.Normal;
        _native.Center();
        return _native;
    }

    internal void AttachContent(IMauiContext context)
    {
        try
        {
            var page = (IView)_window.Page!;
            var view = page.ToMacOSPlatform(((MacOSMauiContext)context).MakeWindowScope(_native!));
            _root!.SetContent(page, view);
            ApplyAppearance(_options.Appearance);
            _root.Relayout();
            _surface!.DesktopMetalSurface.NativeView?.LayoutSubtreeIfNeeded();
            _surface.DesktopMetalSurface.NativeView?.RequestFrame();
            Observe(
                NSWorkspace.SharedWorkspace.NotificationCenter,
                new NSString("NSWorkspaceAccessibilityDisplayOptionsDidChangeNotification"),
                () =>
                {
                    ApplyAppearance(_options.Appearance);
                    Publish();
                }
            );
            Observe(
                NSNotificationCenter.DefaultCenter,
                NSApplication.DidHideNotification,
                () =>
                {
                    ((IWindow)_window).Stopped();
                    Publish();
                }
            );
            Observe(
                NSNotificationCenter.DefaultCenter,
                NSApplication.DidUnhideNotification,
                () =>
                {
                    ((IWindow)_window).Resumed();
                    Publish();
                }
            );
            Publish();
            _attached.TrySetResult();
        }
        catch (Exception error)
        {
            _attached.TrySetException(error);
            _ready.TrySetException(error);
        }
    }

    private void Observe(NSNotificationCenter center, NSString name, Action action) =>
        _observers.Add(
            (
                center,
                center.AddObserver(
                    name,
                    _ =>
                    {
                        if (!_disposed)
                            action();
                    }
                )
            )
        );

    private double CaptionHeight =>
        _native is { } w
            ? Math.Max(0, w.ContentView!.Bounds.Height - w.ContentLayoutRect.Height)
            : 0;

    private void ResizeClient(DSize size)
    {
        var w = _native!;
        // The renderer occupies the unobscured content rectangle, below native controls.
        w.SetContentSize(new CGSize(size.width, size.height + CaptionHeight));
        _options = _options with { Size = size };
        _root?.Relayout();
    }

    private void SetLimits()
    {
        var w = _native!;
        w.ContentMinSize = _options.MinimumSize is { } min
            ? new CGSize(min.width, min.height + CaptionHeight)
            : new CGSize(1, 1 + CaptionHeight);
        w.ContentMaxSize = _options.MaximumSize is { } max
            ? new CGSize(max.width, max.height + CaptionHeight)
            : new CGSize(double.MaxValue, double.MaxValue);
    }

    public async Task<WindowState> ExecuteAsync(
        WindowCommand command,
        CancellationToken cancellationToken
    )
    {
        var transition = await OnUiAsync(
            () =>
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                var w = _native!;
                Task pending = Task.CompletedTask;
                switch (command.Kind)
                {
                    case WindowCommandKind.Show:
                        _surface!.DesktopMetalSurface.NativeView?.PresentPreparedFrame();
                        w.OrderFront(null);
                        _surface.DesktopMetalSurface.NativeView?.RequestFrame();
                        if (!_shown)
                        {
                            _shown = true;
                            if (_options.PresentationState == WindowPresentationState.Minimized)
                                pending = Miniaturize(true);
                            if (
                                _options.PresentationState == WindowPresentationState.Maximized
                                && !w.IsZoomed
                            )
                                w.Zoom(null);
                            if (_options.PresentationState == WindowPresentationState.FullScreen)
                                pending = FullScreen(true);
                        }
                        break;
                    case WindowCommandKind.Hide:
                        w.OrderOut(null);
                        break;
                    case WindowCommandKind.Focus:
                        if (!w.IsVisible)
                            throw new InvalidOperationException(
                                "Show the window before requesting focus."
                            );
                        NSApplication.SharedApplication.Activate();
                        w.MakeKeyWindow();
                        break;
                    case WindowCommandKind.Size:
                        ResizeClient((DSize)command.Value!);
                        break;
                    case WindowCommandKind.Bounds:
                        throw new NotSupportedException(
                            "Global physical-pixel bounds are not mapped across AppKit screens."
                        );
                    case WindowCommandKind.Center:
                        w.Center();
                        break;
                    case WindowCommandKind.MinimumSize:
                        _options = _options with { MinimumSize = (DSize?)command.Value };
                        SetLimits();
                        break;
                    case WindowCommandKind.MaximumSize:
                        _options = _options with { MaximumSize = (DSize?)command.Value };
                        SetLimits();
                        break;
                    case WindowCommandKind.Title:
                        _options = _options with { Title = (string)command.Value! };
                        w.Title = _options.Title;
                        break;
                    case WindowCommandKind.AlwaysOnTop:
                        _options = _options with { AlwaysOnTop = (bool)command.Value! };
                        w.Level = _options.AlwaysOnTop
                            ? NSWindowLevel.Floating
                            : NSWindowLevel.Normal;
                        break;
                    case WindowCommandKind.SkipTaskbar:
                        if ((bool)command.Value!)
                            throw new NotSupportedException("Dock policy is application-scoped.");
                        break;
                    case WindowCommandKind.Resizable:
                        _options = _options with { Resizable = (bool)command.Value! };
                        w.StyleMask = _options.Resizable
                            ? w.StyleMask | NSWindowStyle.Resizable
                            : w.StyleMask & ~NSWindowStyle.Resizable;
                        break;
                    case WindowCommandKind.Minimize:
                        pending = Miniaturize(true);
                        break;
                    case WindowCommandKind.Maximize:
                        RequireWindowed();
                        if (!w.IsZoomed)
                            w.Zoom(null);
                        break;
                    case WindowCommandKind.Restore:
                        if (w.StyleMask.HasFlag(NSWindowStyle.FullScreenWindow))
                            pending = FullScreen(false);
                        else if (w.IsMiniaturized)
                            pending = Miniaturize(false);
                        else if (w.IsZoomed)
                            w.Zoom(null);
                        break;
                    case WindowCommandKind.FullScreen:
                        pending = FullScreen((bool)command.Value!);
                        break;
                    case WindowCommandKind.Drag:
                        if (
                            NSApplication.SharedApplication.CurrentEvent
                                is not { Type: NSEventType.LeftMouseDown } mouse
                            || mouse.Window != w
                        )
                            throw new InvalidOperationException(
                                "Window drag requires this window's current mouse-down event."
                            );
                        w.PerformWindowDrag(mouse);
                        break;
                    case WindowCommandKind.Resize:
                        throw new NotSupportedException("Use the native AppKit resize border.");
                    default:
                        throw new NotSupportedException(command.Kind.ToString());
                }
                Publish();
                return pending;
            },
            cancellationToken
        );
        await transition.WaitAsync(cancellationToken);
        return await OnUiAsync(
            () =>
            {
                Publish();
                return State;
            },
            cancellationToken
        );
    }

    private Task Miniaturize(bool value)
    {
        if (_native!.IsMiniaturized == value)
            return Task.CompletedTask;
        if (_miniaturization is not null)
            throw new InvalidOperationException("A minimize transition is in progress.");
        var pending = _miniaturization = new(TaskCreationOptions.RunContinuationsAsynchronously);
        if (value)
            _native.Miniaturize(null);
        else
            _native.Deminiaturize(null);
        return pending.Task;
    }

    private void MiniaturizationFinished()
    {
        Publish();
        _miniaturization?.TrySetResult();
        _miniaturization = null;
    }

    private void RequireWindowed()
    {
        if (_native!.StyleMask.HasFlag(NSWindowStyle.FullScreenWindow))
            throw new NotSupportedException("Exit native full screen first.");
    }

    private Task FullScreen(bool value)
    {
        if (
            _native!.StyleMask.HasFlag(NSWindowStyle.FullScreenWindow) == value
            && _presentation is null
        )
            return Task.CompletedTask;
        if (_presentation is not null)
            throw new InvalidOperationException("A native full-screen transition is in progress.");
        _presentation = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var pending = _presentation;
        _native.ToggleFullScreen(null);
        return pending.Task;
    }

    private void PresentationFinished(Exception? error = null)
    {
        var pending = _presentation;
        _presentation = null;
        _root?.Relayout();
        Publish();
        if (error is null)
            pending?.TrySetResult();
        else
            pending?.TrySetException(error);
    }

    public Task<WindowState> ApplyAppearanceAsync(
        Appearance appearance,
        CancellationToken cancellationToken
    ) =>
        OnUiAsync(
            () =>
            {
                ApplyAppearance(appearance);
                Publish();
                return State;
            },
            cancellationToken
        );

    private void ApplyAppearance(Appearance appearance)
    {
        if (_applyingAppearance || _disposed)
            return;
        _applyingAppearance = true;
        try
        {
            ApplyAppearanceCore(appearance);
        }
        finally
        {
            _applyingAppearance = false;
        }
    }

    private void ApplyAppearanceCore(Appearance appearance)
    {
        var effective = AppKitDesktopWindowPolicy.Resolve(
            appearance,
            OperatingSystem.IsMacOSVersionAtLeast(26),
            NSWorkspace.SharedWorkspace.AccessibilityDisplayShouldReduceTransparency
        );
        _effective = effective.Appearance;
        _fallback = effective.Detail;
        _policyFallback = effective.SystemPolicyFallback;
        var mode = _effective.Backdrop.Mode;
        _native!.Appearance =
            appearance.ThemeSource == WindowThemeSource.System
                ? null
                : NSAppearance.GetAppearance(
                    appearance.Theme == WindowTheme.Dark
                        ? NSAppearance.NameDarkAqua
                        : NSAppearance.NameAqua
                );
        _effective = _effective with
        {
            Theme =
                _native.EffectiveAppearance.FindBestMatch([
                    NSAppearance.NameAqua,
                    NSAppearance.NameDarkAqua,
                ]) == NSAppearance.NameDarkAqua
                    ? WindowTheme.Dark
                    : WindowTheme.Light,
        };
        var legacy = DesktopApplication.ToLegacy(_effective);
        _surface!.DesktopMetalSurface.NativeView!.SetWindowAppearance(
            new Doroti.Ui.WindowAppearanceOptions(
                legacy,
                appearance.TitleBar.Background != WindowTitleBarBackground.System
                    ? Doroti.Ui.WindowTitlebarStyle.unified
                    : Doroti.Ui.WindowTitlebarStyle.solid
            )
        );
        _native.TitleVisibility = NSWindowTitleVisibility.Visible;
        _native.TitlebarAppearsTransparent =
            appearance.TitleBar.Background == WindowTitleBarBackground.Solid
            || (
                appearance.TitleBar.Background == WindowTitleBarBackground.Backdrop
                && mode is BackdropMode.Acrylic or BackdropMode.LiquidGlass
            );
        _caption!.Hidden = appearance.TitleBar.Background != WindowTitleBarBackground.Solid;
        var captionColor = appearance.TitleBar.BackgroundColor;
        _native.EffectiveAppearance.PerformAsCurrentDrawingAppearance(() =>
            _caption.Layer!.BackgroundColor = captionColor is null
                ? NSColor.WindowBackground.CGColor
                : NSColor
                    .FromRgba(
                        (nfloat)captionColor.r,
                        (nfloat)captionColor.g,
                        (nfloat)captionColor.b,
                        1
                    )
                    .CGColor
        );
        _native.TitlebarSeparatorStyle = _native.TitlebarAppearsTransparent
            ? NSTitlebarSeparatorStyle.None
            : NSTitlebarSeparatorStyle.Automatic;
        if (mode == BackdropMode.Solid)
        {
            var color =
                _effective.Theme == WindowTheme.Dark
                    ? appearance.DarkBackgroundColor ?? appearance.BackgroundColor
                    : appearance.BackgroundColor;
            _native.BackgroundColor =
                color.a == 0
                    ? NSColor.WindowBackground
                    : NSColor.FromRgba((nfloat)color.r, (nfloat)color.g, (nfloat)color.b, 1);
            _native.IsOpaque = true;
        }
        _options = _options with { Appearance = appearance };
        _root?.Relayout();
    }

    private void FrameReady()
    {
        if (_attached.Task.IsCompletedSuccessfully)
        {
            Publish();
            _ready.TrySetResult();
        }
    }

    private void FrameFailed(Exception error) => _ready.TrySetException(error);

    private void Publish()
    {
        if (_native is not { } w || _disposed)
            return;
        var layout = w.ContentLayoutRect;
        var left = 0.0;
        foreach (
            var kind in new[]
            {
                NSWindowButton.CloseButton,
                NSWindowButton.MiniaturizeButton,
                NSWindowButton.ZoomButton,
            }
        )
            if (w.StandardWindowButton(kind) is { } button)
                left = Math.Max(left, button.ConvertRectToView(button.Bounds, w.ContentView).Right);
        State = new(
            null,
            new(layout.Width, layout.Height),
            w.BackingScaleFactor,
            w.IsVisible && !w.IsMiniaturized && !NSApplication.SharedApplication.Hidden,
            w.IsKeyWindow,
            w.StyleMask.HasFlag(NSWindowStyle.FullScreenWindow) ? WindowPresentationState.FullScreen
                : w.IsMiniaturized ? WindowPresentationState.Minimized
                : w.IsZoomed ? WindowPresentationState.Maximized
                : WindowPresentationState.Normal,
            _options.Appearance,
            new(_effective, _policyFallback, _fallback),
            new(CaptionHeight, Doroti.Ui.ViewPadding.zero, left, 0, ++_revision),
            _revision
        );
        StateChanged?.Invoke(State);
    }

    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        var retirement = await OnUiAsync(
            () => _surface?.DesktopMetalSurface.NativeView?.RetireAsync() ?? Task.CompletedTask,
            cancellationToken
        );
        await retirement.WaitAsync(cancellationToken);
        await OnUiAsync(
            () =>
            {
                _destroying = true;
                Cleanup();
                if (!_nativeClosed)
                    _native?.Close();
                return true;
            },
            cancellationToken
        );
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
            await CloseAsync(CancellationToken.None);
    }

    private void Cleanup()
    {
        if (_disposed)
            return;
        _disposed = true;
        foreach (var (center, token) in _observers)
        {
            center.RemoveObserver(token);
            token.Dispose();
        }
        _observers.Clear();
        if (_surface is { } surface)
        {
            surface.DesktopFrameReady -= FrameReady;
            surface.DesktopFrameFailed -= FrameFailed;
            surface.Dispose();
        }
        _ready.TrySetException(new ObjectDisposedException(nameof(AppKitDesktopWindowHost)));
        _attached.TrySetException(new ObjectDisposedException(nameof(AppKitDesktopWindowHost)));
        _presentation?.TrySetException(
            new ObjectDisposedException(nameof(AppKitDesktopWindowHost))
        );
        _miniaturization?.TrySetException(
            new ObjectDisposedException(nameof(AppKitDesktopWindowHost))
        );
    }

    private static Task<T> OnUiAsync<T>(Func<T> action, CancellationToken cancellationToken)
    {
        if (NSThread.IsMain)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(action());
        }
        var completion = new TaskCompletionSource<T>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        NSApplication.SharedApplication.BeginInvokeOnMainThread(() =>
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
        });
        return completion.Task;
    }

    private sealed class WindowDelegate(AppKitDesktopWindowHost host) : NSWindowDelegate
    {
        public override bool WindowShouldClose(NSObject sender)
        {
            if (host._destroying)
                return true;
            host.CloseRequested?.Invoke();
            return false;
        }

        public override void WillClose(NSNotification notification)
        {
            if (host._nativeClosed)
                return;
            host._nativeClosed = true;
            var unexpected = !host._destroying;
            host._destroying = true;
            ((IWindow)host._window).Destroying();
            if (unexpected)
                host.Closed?.Invoke();
        }

        public override void DidBecomeKey(NSNotification notification)
        {
            if (!host._destroying && !host._active)
            {
                host._active = true;
                ((IWindow)host._window).Activated();
            }
            host.Publish();
        }

        public override void DidResignKey(NSNotification notification)
        {
            if (!host._destroying && host._active)
            {
                host._active = false;
                ((IWindow)host._window).Deactivated();
            }
            host.Publish();
        }

        public override void DidResize(NSNotification notification)
        {
            host._root?.Relayout();
            host.Publish();
        }

        public override void DidMove(NSNotification notification) => host.Publish();

        public override void DidChangeScreen(NSNotification notification) => host.Publish();

        public override void DidChangeBackingProperties(NSNotification notification) =>
            host.Publish();

        public override void DidMiniaturize(NSNotification notification) =>
            host.MiniaturizationFinished();

        public override void DidDeminiaturize(NSNotification notification) =>
            host.MiniaturizationFinished();

        public override void DidEnterFullScreen(NSNotification notification) =>
            host.PresentationFinished();

        public override void DidExitFullScreen(NSNotification notification) =>
            host.PresentationFinished();

        public override void DidFailToEnterFullScreen(NSWindow window) =>
            host.PresentationFinished(new InvalidOperationException("AppKit refused full screen."));

        public override void DidFailToExitFullScreen(NSWindow window) =>
            host.PresentationFinished(
                new InvalidOperationException("AppKit refused leaving full screen.")
            );
    }

    private sealed class DesktopNativeWindow(
        CGRect frame,
        NSWindowStyle style,
        NSBackingStore backing,
        bool defer
    ) : NSWindow(frame, style, backing, defer)
    {
        // Preserve explicit client sizes larger than the work area, including at first show.
        public override CGRect ConstrainFrameRect(CGRect frameRect, NSScreen? screen) => frameRect;
    }

    private sealed class CaptionFill : NSView
    {
        public override NSView? HitTest(CGPoint point) => null;
    }

    private sealed class ContentRoot(AppKitDesktopWindowHost host) : NSView
    {
        private IView? _page;
        private NSView? _view;
        private bool _layout;
        public override bool IsFlipped => true;

        public override void ViewDidChangeEffectiveAppearance()
        {
            base.ViewDidChangeEffectiveAppearance();
            if (host._attached.Task.IsCompletedSuccessfully && !host._disposed)
            {
                (Microsoft.Maui.Controls.Application.Current as IApplication)?.ThemeChanged();
                host.ApplyAppearance(host._options.Appearance);
                host.Publish();
            }
        }

        internal void SetContent(IView page, NSView view)
        {
            _page = page;
            _view = view;
            AddSubview(view);
            Relayout();
        }

        public override void Layout()
        {
            base.Layout();
            Relayout();
        }

        public override void SetFrameSize(CGSize size)
        {
            base.SetFrameSize(size);
            Relayout();
        }

        internal void Relayout()
        {
            if (host._disposed || _layout || _page is null || _view is null || host._native is null)
                return;
            _layout = true;
            try
            {
                var r = host._native.ContentLayoutRect;
                var top = Bounds.Height - r.Y - r.Height;
                if (host._caption is { } caption)
                    caption.Frame = new CGRect(0, 0, Bounds.Width, top);
                _view.Frame = new CGRect(r.X, top, r.Width, r.Height);
                _page.Measure(r.Width, r.Height);
                _page.Arrange(new Microsoft.Maui.Graphics.Rect(r.X, top, r.Width, r.Height));
            }
            finally
            {
                _layout = false;
            }
        }
    }
}

/// <summary>The preview upstream handler shows during allocation; this handler defers all ordering.</summary>
internal sealed class AppKitDesktopWindowHandler : ElementHandler<IWindow, NSWindow>
{
    private static readonly IPropertyMapper<IWindow, AppKitDesktopWindowHandler> DesktopMapper =
        new PropertyMapper<IWindow, AppKitDesktopWindowHandler>(ElementMapper)
        {
            [nameof(IWindow.Content)] = (handler, _) =>
                handler.Host.AttachContent(handler.MauiContext!),
            [nameof(IWindow.Title)] = (handler, window) =>
                handler.PlatformView.Title = window.Title ?? string.Empty,
        };

    public AppKitDesktopWindowHandler()
        : base(DesktopMapper) { }

    private AppKitDesktopWindowHost Host =>
        ((AppKitDesktopWindowHost.DesktopWindow)VirtualView).Host;

    protected override NSWindow CreatePlatformElement() => Host.CreateNative();
}
#endif
