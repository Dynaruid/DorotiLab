using WindowId = Doroti.Ui.WindowId;
#if MACCATALYST
using System.Runtime.Versioning;
using CoreGraphics;
using Doroti.Desktop;
using Doroti.Hosting;
using Foundation;
using UIKit;
using DSize = Doroti.Ui.Size;
using Appearance = Doroti.Desktop.WindowAppearanceOptions;
using Window = Microsoft.Maui.Controls.Window;

namespace Doroti.Host.Maui;

/// <summary>One MAUI UIWindowScene controlled through supported UIKit APIs.</summary>
[SupportedOSPlatform("maccatalyst16.0")]
internal sealed class MacCatalystDesktopWindowHost : IWindowHost
{
    private static readonly HashSet<Window> PendingScenes = [];
    internal static bool HasPendingScene => PendingScenes.Count != 0;
    private readonly Window _window;
    private bool _additional;
    private Factory? _factory;
    private readonly DorotiApplicationDescriptor _descriptor;
    private readonly TaskCompletionSource _attached = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    private readonly TaskCompletionSource _ready = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    private readonly TaskCompletionSource _disconnected = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    private readonly List<Action> _detach = [];
    private WindowOptions _options = new();
    private DorotiMauiSurface? _surface;
    private UIWindow? _native;
    private UIWindowScene? _scene;
    private TaskCompletionSource? _geometry;
    private DSize? _geometrySize;
    private bool _loaded,
        _disposed,
        _destroying,
        _initializing;
    private long _revision;
    private CGSize _defaultMinimum,
        _defaultMaximum;

    private MacCatalystDesktopWindowHost(Window window, DorotiApplicationDescriptor descriptor)
    {
        _window = window;
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
        MacCatalystDesktopWindowPolicy
            .Evaluate(definition.MainWindow.Options, null)
            .ThrowIfUnsupported();
        if (definition.LifetimePolicy != WindowLifetimePolicy.Explicit)
            throw new NotSupportedException(
                "Catalyst requires Explicit lifetime: UIKit owns native application termination."
            );
        if (!UIApplication.SharedApplication.SupportsMultipleScenes)
            throw new NotSupportedException(
                "Catalyst scene close requires UIApplicationSupportsMultipleScenes=true; each Desktop window owns a separate scene."
            );
        if (!DorotiGraphiteView.Enabled)
            throw new NotSupportedException(
                "The Catalyst Desktop adapter requires Graphite for observable GPU completion and retirement."
            );
        if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Mac)
            throw new NotSupportedException(
                "The Catalyst Desktop adapter requires UIDeviceFamily=6 (Mac idiom), without iPad scaling."
            );
        var window = new Window
        {
            Title = definition.MainWindow.Options.Title,
            Width = definition.MainWindow.Options.Size.width,
            Height = definition.MainWindow.Options.Size.height,
        };
        var host = new MacCatalystDesktopWindowHost(window, descriptor);
        var factory = new Factory(host);
        host._factory = factory;
        var manager = new DorotiWindowManager(factory, definition.LifetimePolicy);
        manager.ExitRequested += () => _ = ReleaseApplicationAsync();
        manager.InitializationFailed += (_, error) => DorotiMauiSurface.WriteFailure(error);
        _ = StartAsync();
        return window;
        async Task ReleaseApplicationAsync()
        {
            try { await manager.WaitForInitializationAsync().ConfigureAwait(false); }
            catch (Exception error) { DorotiMauiSurface.WriteFailure(error); }
            await new MauiApplicationDispatcher().InvokeAsync(factory.Dispose);
        }
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

    private sealed class Factory(MacCatalystDesktopWindowHost host) : IWindowHostFactory, IDisposable
    {
        private bool _allocated;
        internal DorotiSharedHostSession Framework { get; } = new(host._descriptor, new MauiApplicationDispatcher());
        internal DorotiApplicationBoundary? Application { get; private set; }
        internal void Attach(DorotiApplicationBoundary boundary) => Application ??= boundary.Retain();
        public void Dispose() { Framework.Dispose(); Application?.Dispose(); Application = null; }
        public WindowManagerCapabilities Capabilities { get; } = new(true, null);
        public WindowOptions MapRequest(Doroti.Ui.WindowRequest request) => MacCatalystDesktopWindowPolicy.MapRequest(request);
        public WindowEvaluation Evaluate(WindowCreateOptions options) =>
            MacCatalystDesktopWindowPolicy.Evaluate(options.Options, null);
        public async ValueTask<IWindowHost> CreateAsync(
            Doroti.Ui.WindowId id, WindowCreateOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!_allocated) { _allocated = true; return host; }
            if (Application is null) throw new InvalidOperationException("Initialize the main scene before creating another window.");
            return await OnUiAsync(() => new MacCatalystDesktopWindowHost(new Window
            {
                Title = options.Options.Title,
                Width = options.Options.Size.width,
                Height = options.Options.Size.height,
            }, host._descriptor) { _additional = true, _factory = this }, cancellationToken);
        }
    }

    public WindowCapabilities Capabilities { get; } =
        new(MacCatalystDesktopWindowPolicy.Evaluate, canCancelNativeClose: false);
    public WindowState State { get; private set; }
    public Task ReadyToShow => _ready.Task;
    public event Action<WindowState>? StateChanged;

    // UIKit exposes disconnect, not a cancellable pre-close delegate. The capability
    // reports this explicitly; CloseAsync still uses the controller's decision.
    public event Action? CloseRequested
    {
        add { }
        remove { }
    }
    public event Action? Closed;

    public async Task InitializeAsync(
        WindowOptions options,
        DesktopWindowContext context,
        CancellationToken cancellationToken
    )
    {
        await OnUiAsync(() =>
        {
            _options = options;
            _window.HandlerChanged += HandlerChanged;
            var configuration = DesktopApplication.ToViewConfiguration(options, context.Windows.LifetimePolicy)
                with { Navigation = _descriptor.ViewConfiguration.Navigation };
            if (_additional && configuration.Navigation is { } navigation)
                configuration = configuration with { Navigation = navigation with { ProtocolScheme = null, RestorationId = null } };
            _surface = new(_descriptor with { ViewConfiguration = configuration }, _factory!.Framework.AllocateViewId())
            {
                DesktopManaged = true,
                SharedFramework = _factory!.Framework, WindowContext = context,
                SharedApplication = _additional ? _factory!.Application : null,
                ApplicationAttached = boundary => _factory!.Attach(boundary),
                OwnsApplicationActivation = !_additional,
            };
            _surface.DesktopFrameReady += FrameReady;
            _surface.DesktopFrameFailed += FrameFailed;
            _surface.Loaded += Loaded;
            _surface.SizeChanged += SurfaceSizeChanged;
            _window.Page = new ContentPage
            {
                Title = options.Title,
                SafeAreaEdges = Microsoft.Maui.SafeAreaEdges.None,
                Content = _surface,
            };
            if (_additional)
            {
                PendingScenes.Add(_window);
                try { Microsoft.Maui.Controls.Application.Current!.OpenWindow(_window); }
                catch { PendingScenes.Remove(_window); throw; }
            }
            HandlerChanged(null, EventArgs.Empty);
            return true;
        }, cancellationToken);
        await _attached.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
    }

    private void HandlerChanged(object? sender, EventArgs args)
    {
        if (_native is not null || _window.Handler?.PlatformView is not UIWindow native)
            return;
        try
        {
            PendingScenes.Remove(_window);
            _native = native;
            _scene =
                native.WindowScene
                ?? throw new NotSupportedException(
                    "Catalyst Desktop requires UIApplicationSceneManifest."
                );
            var restrictions =
                _scene.SizeRestrictions
                ?? throw new NotSupportedException("Scene size restrictions are unavailable.");
            _defaultMinimum = restrictions.MinimumSize;
            _defaultMaximum = restrictions.MaximumSize;
            var observer = _scene.AddObserver(
                "effectiveGeometry",
                NSKeyValueObservingOptions.New,
                _ =>
                {
                    GeometryChanged();
                }
            );
            _detach.Add(observer.Dispose);
            Observe(
                UIScene.DidDisconnectNotification,
                _scene,
                () =>
                {
                    _disconnected.TrySetResult();
                    if (!_destroying)
                    {
                        _destroying = true;
                        Closed?.Invoke();
                    }
                }
            );
            Observe(
                UIScene.DidActivateNotification,
                _scene,
                () =>
                {
                    Publish();
                    if (_loaded)
                        _ = InitializeGeometryAsync();
                }
            );
            Observe(UIScene.WillDeactivateNotification, _scene, Publish);
            Observe(UIScene.DidEnterBackgroundNotification, _scene, Publish);
            Observe(UIScene.WillEnterForegroundNotification, _scene, Publish);
            Observe(UIWindow.DidBecomeKeyNotification, native, Publish);
            Observe(UIWindow.DidResignKeyNotification, native, Publish);
            _scene.Title = _options.Title;
            ApplyAppearance(_options.Appearance);
            SetLimits(lockSize: false);
            if (_loaded)
                _ = InitializeGeometryAsync();
        }
        catch (Exception error)
        {
            _attached.TrySetException(error);
            _ready.TrySetException(error);
        }
    }

    private void Observe(NSString name, NSObject obj, Action action)
    {
        var center = NSNotificationCenter.DefaultCenter;
        var token = center.AddObserver(name, _ => action(), obj);
        _detach.Add(() =>
        {
            center.RemoveObserver(token);
            token.Dispose();
        });
    }

    private void Loaded(object? sender, EventArgs args)
    {
        _loaded = true;
        if (_native is not null)
            _ = InitializeGeometryAsync();
    }

    private async Task InitializeGeometryAsync()
    {
        if (_initializing || _scene?.ActivationState != UISceneActivationState.ForegroundActive)
            return;
        _initializing = true;
        try
        {
            _scene!.Title = _options.Title;
            ApplyAppearance(_options.Appearance);
            await ResizeAsync(_options.Size, CancellationToken.None);
            SetLimits();
            Publish();
            _attached.TrySetResult();
            _surface!.RequestDesktopFrame();
        }
        catch (Exception error)
        {
            _attached.TrySetException(error);
            _ready.TrySetException(error);
        }
    }

    private DSize ClientSize =>
        _surface is { Width: > 0, Height: > 0 } surface
            ? new(surface.Width, surface.Height)
            : new(
                Math.Max(1, (double)(_native?.Bounds.Width ?? 1)),
                Math.Max(1, (double)(_native?.Bounds.Height ?? 1))
            );

    private async Task ResizeAsync(DSize size, CancellationToken cancellationToken)
    {
        var pending = await OnUiAsync(
            () =>
            {
                if (_scene!.FullScreen)
                    throw new NotSupportedException(
                        "Leave native full screen before requesting a client size."
                    );
                _native!.LayoutIfNeeded();
                var frame = _scene!.EffectiveGeometry.SystemFrame;
                if (HasClientSize(size))
                    return Task.CompletedTask;
                if (_geometry is not null)
                    throw new InvalidOperationException(
                        "A scene geometry request is already pending."
                    );
                if (!_options.Resizable)
                    SetLimits(lockSize: false);
                var completion = _geometry = new(
                    TaskCreationOptions.RunContinuationsAsynchronously
                );
                _geometrySize = size;
                // A Mac-idiom scene's system frame describes its UIKit content
                // rectangle; AppKit's outer titlebar is not part of this size.
                // Do not subtract asynchronously updated MAUI/native bounds.
                var desired = new CGRect(frame.X, frame.Y, size.width, size.height);
                using var preferences = new UIWindowSceneGeometryPreferencesMac(desired);
                _scene.RequestGeometryUpdate(
                    preferences,
                    error => completion.TrySetException(new NSErrorException(error))
                );
                return completion.Task;
            },
            cancellationToken
        );
        try
        {
            // UIKit has no success callback. KVO confirms a resolved native geometry;
            // timeout is a failure, never an inferred successful resize.
            await pending.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);
        }
        finally
        {
            await OnUiAsync(
                () =>
                {
                    _geometry = null;
                    _geometrySize = null;
                    if (!_disposed && _scene is not null && !_options.Resizable)
                        SetLimits();
                    return true;
                },
                CancellationToken.None
            );
        }
        await OnUiAsync(
            () =>
            {
                _native!.LayoutIfNeeded();
                _native.RootViewController?.View?.LayoutIfNeeded();
                Publish();
                return true;
            },
            cancellationToken
        );
    }

    private void SetLimits(bool lockSize = true)
    {
        var limits = _scene!.SizeRestrictions!;
        if (!_options.Resizable && lockSize)
        {
            var size = ClientSize;
            limits.MinimumSize = limits.MaximumSize = new CGSize(size.width, size.height);
        }
        else
        {
            limits.MinimumSize = _options.MinimumSize is { } min
                ? new CGSize(min.width, min.height)
                : _defaultMinimum;
            limits.MaximumSize = _options.MaximumSize is { } max
                ? new CGSize(max.width, max.height)
                : _defaultMaximum;
        }
    }

    public async Task<WindowState> ExecuteAsync(
        WindowCommand command,
        CancellationToken cancellationToken
    )
    {
        if (command.Kind == WindowCommandKind.Size)
        {
            await ResizeAsync((DSize)command.Value!, cancellationToken);
            _options = _options with { Size = (DSize)command.Value! };
            return State;
        }
        return await OnUiAsync(
            () =>
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                switch (command.Kind)
                {
                    case WindowCommandKind.Show:
                    case WindowCommandKind.Focus:
                        _native!.MakeKeyAndVisible();
                        break;
                    case WindowCommandKind.Title:
                        _options = _options with { Title = (string)command.Value! };
                        _scene!.Title = _options.Title;
                        _window.Title = _options.Title;
                        break;
                    case WindowCommandKind.MinimumSize:
                        _options = _options with { MinimumSize = (DSize?)command.Value };
                        SetLimits();
                        break;
                    case WindowCommandKind.MaximumSize:
                        _options = _options with { MaximumSize = (DSize?)command.Value };
                        SetLimits();
                        break;
                    case WindowCommandKind.Resizable:
                        _options = _options with { Resizable = (bool)command.Value! };
                        SetLimits();
                        break;
                    case WindowCommandKind.SkipTaskbar:
                    case WindowCommandKind.AlwaysOnTop:
                        if ((bool)command.Value!)
                            throw new NotSupportedException(
                                "UIKit does not expose that desktop window policy."
                            );
                        break;
                    default:
                        throw new NotSupportedException(
                            $"Catalyst does not expose {command.Kind} through this adapter. Use native window controls."
                        );
                }
                Publish();
                return State;
            },
            cancellationToken
        );
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
        _native!.OverrideUserInterfaceStyle =
            appearance.ThemeSource == WindowThemeSource.System ? UIUserInterfaceStyle.Unspecified
            : appearance.Theme == WindowTheme.Dark ? UIUserInterfaceStyle.Dark
            : UIUserInterfaceStyle.Light;
        var color =
            _native.TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark
                ? appearance.DarkBackgroundColor ?? appearance.BackgroundColor
                : appearance.BackgroundColor;
        _native.BackgroundColor = UIColor.FromRGBA(
            (nfloat)color.r,
            (nfloat)color.g,
            (nfloat)color.b,
            1
        );
        _options = _options with { Appearance = appearance };
    }

    private void SurfaceSizeChanged(object? sender, EventArgs args) => GeometryChanged();

    private bool HasClientSize(DSize size) =>
        Math.Abs(ClientSize.width - size.width) < .5
        && Math.Abs(ClientSize.height - size.height) < .5
        && _native is { } native
        && Math.Abs(native.Bounds.Width - size.width) < .5
        && Math.Abs(native.Bounds.Height - size.height) < .5;

    private void GeometryChanged()
    {
        Publish();
        if (_geometrySize is { } requested && HasClientSize(requested))
            _geometry?.TrySetResult();
    }

    private void FrameReady()
    {
        GeometryChanged();
        if (_attached.Task.IsCompletedSuccessfully)
        {
            Publish();
            _ready.TrySetResult();
        }
    }

    private void FrameFailed(Exception error) => _ready.TrySetException(error);

    private void Publish()
    {
        if (_native is null || _scene is null || _disposed)
            return;
        var effective = _options.Appearance with
        {
            Theme =
                _native.TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark
                    ? WindowTheme.Dark
                    : WindowTheme.Light,
        };
        State = new(
            null,
            ClientSize,
            _native.Screen.Scale,
            !_native.Hidden
                && _scene.ActivationState
                    is UISceneActivationState.ForegroundActive
                        or UISceneActivationState.ForegroundInactive,
            _native.IsKeyWindow
                && _scene.ActivationState == UISceneActivationState.ForegroundActive,
            _scene.FullScreen ? WindowPresentationState.FullScreen : WindowPresentationState.Normal,
            _options.Appearance,
            new(effective),
            new(0, Doroti.Ui.ViewPadding.zero, 0, 0, ++_revision),
            _revision
        );
        StateChanged?.Invoke(State);
    }

    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        if (_disposed)
            return;
        var retirement = await OnUiAsync(
            () =>
            {
                _surface?.PrepareFrameworkClose();
                return _surface?.DesktopCatalystSurface?.RetireAsync() ?? Task.CompletedTask;
            },
            cancellationToken
        );
        if (_surface?.FrameworkView is { } view) await view.DrainInvocationsAsync(cancellationToken).AsTask().WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
        await retirement.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
        await OnUiAsync(
            () =>
            {
                if (!_destroying && _scene is { } scene)
                {
                    _destroying = true;
                    UIApplication.SharedApplication.RequestSceneSessionDestruction(
                        scene.Session,
                        null,
                        error => _disconnected.TrySetException(new NSErrorException(error))
                    );
                }
                else if (_scene is null)
                    _disconnected.TrySetResult();
                return true;
            },
            cancellationToken
        );
        await _disconnected.Task.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);
        await OnUiAsync(
            () =>
            {
                Cleanup();
                return true;
            },
            CancellationToken.None
        );
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;
        await CloseAsync(CancellationToken.None);
    }

    private void Cleanup()
    {
        if (_disposed)
            return;
        _disposed = true;
        PendingScenes.Remove(_window);
        _window.HandlerChanged -= HandlerChanged;
        if (_surface is { } surface)
        {
            surface.Loaded -= Loaded;
            surface.SizeChanged -= SurfaceSizeChanged;
            surface.DesktopFrameReady -= FrameReady;
            surface.DesktopFrameFailed -= FrameFailed;
            surface.Dispose();
        }
        foreach (var action in _detach)
            action();
        _detach.Clear();
        var error = new ObjectDisposedException(nameof(MacCatalystDesktopWindowHost));
        _attached.TrySetException(error);
        _ready.TrySetException(error);
        _geometry?.TrySetException(error);
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
        UIApplication.SharedApplication.BeginInvokeOnMainThread(() =>
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
}
#endif
