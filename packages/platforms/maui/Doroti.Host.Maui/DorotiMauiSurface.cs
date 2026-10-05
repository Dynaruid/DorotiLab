using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Hosting;
using Doroti.Ui;
using Color = Doroti.Ui.Color;

namespace Doroti.Host.Maui;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = true
)]
[JsonSerializable(typeof(MauiHostDiagnostics))]
internal sealed partial class MauiEvidenceJsonContext : JsonSerializerContext;

public sealed class DorotiMauiSurface : Grid, IDisposable, IAsyncDisposable
{
#if WINDOWS || MACOS || MACCATALYST
    internal bool DesktopManaged { get; init; }
    internal event Action? DesktopFrameReady;
    internal event Action<Exception>? DesktopFrameFailed;
#endif
#if MACCATALYST
    internal DorotiUIKitGraphiteView? DesktopCatalystSurface => _renderSurface.Element.Handler?.PlatformView as DorotiUIKitGraphiteView;
    internal void RequestDesktopFrame() => _renderSurface.InvalidateSurface();
#endif
#if MACOS
    internal DorotiMacOSMetalSurface DesktopMetalSurface => (DorotiMacOSMetalSurface)_renderSurface;
#endif
    internal void PrepareFrameworkClose()
    {
        _closing = true;
        if (SharedFramework is null) _session?.ShutdownFramework();
        else if (FrameworkView is { } view) SharedFramework.BeginViewClose(view);
    }
    internal DorotiSharedHostSession? SharedFramework { get; init; }
#if WINDOWS || MACOS || MACCATALYST
    internal Doroti.Desktop.DesktopWindowContext? WindowContext { get; init; }
#endif
    internal DorotiApplicationBoundary? SharedApplication { get; init; }
    internal Func<DorotiApplicationBoundary?>? SharedApplicationFactory { get; init; }
    internal Action<DorotiApplicationBoundary>? ApplicationAttached { get; init; }
    internal bool OwnsApplicationActivation { get; init; } = true;
    internal DorotiView? FrameworkView { get; private set; }
    internal IPlatformMenuHostCapability? PlatformMenus { get; init; }
    internal IPlatformMenuBarHostCapability? PlatformMenuBar { get; init; }
    internal Action<DorotiMauiSurface>? SurfaceDisposed { get; init; }
#if WINDOWS
    internal DorotiWindowsDxgiSurface WindowsSurface => (DorotiWindowsDxgiSurface)_renderSurface;
    internal Task PrepareDesktopCloseAsync() =>
        ((DorotiWindowsDxgiSurface)_renderSurface).PrepareForCloseAsync();

    // Set only by the dedicated full-window runner. Embedded MAUI surfaces keep
    // their XAML clipping, layout and overlay behavior.
    internal bool OwnsWindowContent
    {
        get => ((DorotiWindowsDxgiSurface)_renderSurface).OwnsWindowContent;
        init => ((DorotiWindowsDxgiSurface)_renderSurface).OwnsWindowContent = value;
    }
#endif
    private static readonly TimeSpan EvidenceWriteInterval = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan EvidenceWriteQuiescence = TimeSpan.FromMilliseconds(250);
    private readonly ulong _viewId;
    private readonly DorotiApplicationDescriptor _application;
    private DorotiApplicationBoundary? _boundary;
    private DorotiHostSession? _session;
    private DorotiSharedHostSession? _ownedFramework;
    private MauiFrameworkHost? _host;
    private readonly IMauiSkiaSurface _renderSurface;
    private readonly MauiTextInputBridge _textInput;
    internal MauiTextInputBridge TextInput => _textInput;
    private readonly AbsoluteLayout _semanticsLayer;
    private bool _attached;
    private bool _disposed;
    private Task? _disposeTask;
    private readonly object _disposeGate = new();
    private bool _closing;
    private long _lastEvidenceWriteTimestamp;
    private long _lastEvidenceReplayed;
    private long _evidenceWriteGeneration;
    private int _evidenceWritePending;
    private Window? _window;
#if WINDOWS
    private Microsoft.UI.Xaml.Window? _closingWindow;
    private WindowsWindowBackdrop? _windowsBackdrop;
    private bool _closeStarted;
    private bool _closeReady;
#endif

    public DorotiMauiSurface(DorotiApplicationDescriptor application, ulong viewId = 1)
    {
        SafeAreaEdges = Microsoft.Maui.SafeAreaEdges.None;
        _application = application ?? throw new ArgumentNullException(nameof(application));
        _viewId = viewId;
        var startupColor = ResolveBackgroundColor(
            Application.Current?.RequestedTheme ?? AppTheme.Unspecified
        );
        BackgroundColor = new Microsoft.Maui.Graphics.Color(
            (float)startupColor.r,
            (float)startupColor.g,
            (float)startupColor.b,
            (float)startupColor.a
        );
        _semanticsLayer = new AbsoluteLayout
        {
            SafeAreaEdges = Microsoft.Maui.SafeAreaEdges.None,
            InputTransparent = true,
            CascadeInputTransparent = false,
        };
#if IOS || MACCATALYST
        _textInput = new(
            CreateHiddenInput<DorotiUIKitEntry>,
            CreateHiddenInput<DorotiUIKitEditor>,
            this,
            attachOnDemand: true
        );
#else
        _textInput = new(
            CreateHiddenInput<Entry>,
            CreateHiddenInput<Editor>,
            this,
            attachOnDemand: true
        );
#endif
#if MACOS
        _renderSurface = new DorotiMacOSMetalSurface(_viewId, _textInput)
        {
            Appearance = _application.ViewConfiguration.ResolveAppearance(),
        };
        // The renderer applies the configured base color once. A second MAUI
        // background would cover the native material behind its Metal surface.
        BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent;
#elif WINDOWS
        _renderSurface = new DorotiWindowsDxgiSurface(_textInput);
        // Skia already applies the configured base color. Keep the MAUI
        // container transparent so it does not cover the system material.
        BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent;
#else
        _renderSurface = new MauiSkglSurface(_textInput, _viewId);
#endif
        Children.Add(_renderSurface.Element);
        Children.Add(_semanticsLayer);
#if IOS && !MACCATALYST
        if (_renderSurface is MauiSkglSurface graphitePulse)
            graphitePulse.FrameworkFramePrepared += ScheduleEvidenceWrite;
#endif
        _renderSurface.Paint += PaintGpuSurface;
        _renderSurface.PresentCompleted += CompleteNativePaint;
        _renderSurface.PaintFailed += HandlePaintFailure;
        HandlerChanged += HandleHandlerChanged;
        Loaded += HandleLoaded;
        Unloaded += HandleUnloaded;
        if (Application.Current is { } currentApplication)
        {
            currentApplication.RequestedThemeChanged += HandleRequestedThemeChanged;
        }
    }

#if WINDOWS || MACCATALYST || IOS || ANDROID || MACOS
    // Timing probes must not allocate the full frame/semantics/GPU trace on
    // every display pulse and thereby delay the viewport they are measuring.
    internal MauiSurfaceSnapshot? GeometrySnapshot => _host?.CaptureGeometry(_viewId);
    public MauiHostDiagnostics? Diagnostics =>
        _host?.CaptureDiagnostics(
            _viewId,
            "src/App.cs",
#if WINDOWS
            "windows/App.xaml.cs"
#elif MACCATALYST
        "obj/Doroti.Generated/DorotiBootstrap.g.cs -> macos/AppDelegate.cs"
#elif IOS
            "obj/Doroti.Generated/DorotiBootstrap.g.cs -> ios/AppDelegate.cs"
#elif ANDROID
            RuntimeInformation.ProcessArchitecture == Architecture.X64
                ? "obj/android-x64/Doroti.Generated/DorotiBootstrap.g.cs -> android/MainApplication.cs"
                : "obj/android-arm64/Doroti.Generated/DorotiBootstrap.g.cs -> android/MainApplication.cs"
#elif MACOS
            "obj/Doroti.Generated/DorotiBootstrap.g.cs -> macos/AppKitDelegate.cs"
#endif
        );
#else
#error Doroti.Host.Maui requires an explicit bootstrap source.
#endif

    private void HandleHandlerChanged(object? sender, EventArgs args)
    {
        _ = sender;
        _ = args;
        if (Handler is null || _attached || _disposed || _closing)
        {
            return;
        }

        try
        {
            if (SharedFramework is { } framework)
            {
                framework.Start();
                _session = framework.Session;
            }
            else
            {
                _ownedFramework = new(_application, new MauiApplicationDispatcher());
                _ownedFramework.Start();
                _session = _ownedFramework.Session;
            }
            _host = new();
#if WINDOWS
            var windowsSurface = (DorotiWindowsDxgiSurface)_renderSurface;
            windowsSurface.PlatformViews = new WindowsMauiPlatformViewHost(windowsSurface, _textInput);
#endif
#if MACOS
            var appKitSurface = (DorotiMacOSMetalSurface)_renderSurface;
            appKitSurface.PlatformViews = new AppKitPlatformViewHost(appKitSurface, _textInput);
#endif
#if ANDROID
            var androidPlatformViews = _renderSurface.Element is DorotiGraphiteView androidGraphite
                ? androidGraphite.PlatformViews = new AndroidPlatformViewHost(
                    androidGraphite,
                    _textInput
                )
                : null;
#endif
#if IOS || MACCATALYST
            var uiKitPlatformViews = _renderSurface.Element is DorotiGraphiteView uiKitGraphite
                ? uiKitGraphite.PlatformViews = new UIKitPlatformViewHost(uiKitGraphite, _textInput)
                : null;
#endif
            var sharedApplication = SharedApplication ?? SharedApplicationFactory?.Invoke();
            _boundary =
#if WINDOWS
                sharedApplication is { } shared
                    ? shared.CreateWindowBoundary(windowsSurface.PlatformViews.CreateFactories(() => _boundary!.ApplicationResources))
                    :
#endif
#if MACOS
                sharedApplication is { } shared
                    ? shared.CreateWindowBoundary(appKitSurface.PlatformViews.CreateFactories(() => _boundary!.ApplicationResources))
                    :
#endif
#if IOS || MACCATALYST
                sharedApplication is { } shared
                    ? shared.CreateWindowBoundary(uiKitPlatformViews?.CreateFactories(() => _boundary!.ApplicationResources) ?? [])
                    :
#endif
                DorotiApplicationBoundary.Load(
                _application.ManifestAssembly,
                _application.ApplicationAssembly,
                _application.LaunchContext.RuntimeIdentifier,
                _application.NativePluginHandlers
#if WINDOWS
                , windowsSurface.PlatformViews.CreateFactories(() => _boundary!.ApplicationResources)
#endif
#if MACOS
                ,
                appKitSurface.PlatformViews.CreateFactories(() => _boundary!.ApplicationResources)
#endif
#if ANDROID
                ,
                androidPlatformViews?.CreateFactories(() => _boundary!.ApplicationResources)
#endif
#if IOS || MACCATALYST
                ,
                uiKitPlatformViews?.CreateFactories(() => _boundary!.ApplicationResources)
#endif
            );
            ApplicationAttached?.Invoke(_boundary);
            IMauiSemanticsBridge semantics =
#if ANDROID
            DorotiGraphiteView.Enabled
                ? new MauiAndroidSemanticsBridge(_renderSurface.Element)
                :
#endif
                new MauiSemanticsBridge(_semanticsLayer);
            FrameworkView =
            _host.CreateView(
                _session,
                _viewId,
                _renderSurface,
                _application.ViewConfiguration,
                semantics,
                _boundary,
                _textInput
                , ownsApplicationActivation: OwnsApplicationActivation, sharedFramework: SharedFramework ?? _ownedFramework,
                platformMenus: PlatformMenus, platformMenuBar: PlatformMenuBar
#if WINDOWS || MACOS || MACCATALYST
                , windowContext: WindowContext
#endif
            );
            using (var dispatcherScope = _session.dispatcher.EnterScope())
            {
                _session.dispatcher.setSemanticsTreeEnabled(true);
            }

            _attached = true;
        }
        catch (Exception exception)
        {
            WriteFailure(exception);
            throw;
        }
    }

    private void PaintGpuSurface(MauiSkiaPaintContext paint)
    {
        if (!_attached || _host is null || _closing)
        {
            return;
        }

        try
        {
            try
            {
                _host.BeginPaint(_viewId, paint);
                if (!paint.SkipRaster)
                {
                    if (paint.ScenePrepared is { } observe && _host.PreparedScene(_viewId) is { } scene) observe(scene);
                    paint.ShaderOnly = _host.CanPresentWithoutNativeComposition(_viewId);
                    _host.SetPaintCpuStageMeasured(_viewId, paint.CpuStageMeasured);
                    paint.Completion = _host.PaintSkiaSurface(
                        _viewId,
                        paint.Surface,
                        paint.PixelWidth,
                        paint.PixelHeight,
                        out var shouldPresent,
                        paint.RequireNewShaderScene
                    );
                    paint.SkipPresent = !shouldPresent;
                }
            }
            finally
            {
                _host.EndPaint(_viewId);
                ScheduleEvidenceWrite();
            }
        }
        catch (Exception exception)
        {
            WriteFailure(exception);
            throw;
        }
    }

    private void CompleteNativePaint(MauiPaintCompletion completion, bool stale)
    {
        if (_disposed || _host is null)
        {
            return;
        }

        if (stale)
        {
            _host.SupersedePaint(
                _viewId,
                completion,
                "Native output was superseded by a newer surface or viewport generation."
            );
            ScheduleEvidenceWrite();
            return;
        }
        _host.CompletePaint(_viewId, completion);
#if WINDOWS || MACOS || MACCATALYST
        if (!_closing) DesktopFrameReady?.Invoke();
#endif
        ScheduleEvidenceWrite();
    }

    private void HandlePaintFailure(MauiPaintCompletion? completion, Exception exception)
    {
        if (completion is { } value && _host is not null)
        {
            _host.FailPaint(_viewId, value, exception.Message);
        }

        WriteFailure(exception);
#if WINDOWS || MACOS || MACCATALYST
        DesktopFrameFailed?.Invoke(exception);
#endif
        ScheduleEvidenceWrite();
    }

    private void ScheduleEvidenceWrite()
    {
        if (!EvidenceEnabled())
        {
            return;
        }

        var generation = Interlocked.Increment(ref _evidenceWriteGeneration);
        if (Interlocked.CompareExchange(ref _evidenceWritePending, 1, 0) != 0)
        {
            return;
        }

        _ = Task.Run(() =>
        {
            try
            {
                var started = Stopwatch.GetTimestamp();
                while (true)
                {
                    Thread.Sleep(EvidenceWriteQuiescence);
                    var latestGeneration = Interlocked.Read(ref _evidenceWriteGeneration);
                    if (
                        latestGeneration == generation
                        || Stopwatch.GetElapsedTime(started) >= EvidenceWriteInterval
                    )
                    {
                        break;
                    }

                    generation = latestGeneration;
                }
                WriteEvidence();
            }
            catch (Exception exception)
            {
                WriteFailure(exception);
            }
            finally
            {
                Interlocked.Exchange(ref _evidenceWritePending, 0);
                if (Interlocked.Read(ref _evidenceWriteGeneration) != generation)
                {
                    ScheduleEvidenceWrite();
                }
            }
        });
    }

    private void WriteEvidence()
    {
        var diagnostics = Diagnostics;
        if (diagnostics is null)
        {
            return;
        }

        var evidencePath = ResolveEvidencePath();
        var shouldRequestReplay =
            diagnostics.Frame.Presented > 0 && diagnostics.Frame.Replayed == 0;
        var timestamp = Stopwatch.GetTimestamp();
        var lastWrite = Interlocked.Read(ref _lastEvidenceWriteTimestamp);
        var lastReplay = Interlocked.Read(ref _lastEvidenceReplayed);
        var firstEvidence = lastWrite == 0;
        var firstReplay = diagnostics.Frame.Replayed > 0 && lastReplay == 0;
        var intervalElapsed =
            !firstEvidence
            && Stopwatch.GetElapsedTime(lastWrite, timestamp) >= EvidenceWriteInterval;

        // Evidence collection is coalesced onto one background writer. JSON serialization and
        // file I/O must never occupy the native paint callback while an interaction is active.
        if (firstEvidence || firstReplay || intervalElapsed)
        {
            var json = JsonSerializer.Serialize(
                diagnostics,
                MauiEvidenceJsonContext.Default.MauiHostDiagnostics
            );
            var path = evidencePath;
#if ANDROID
            path = System.IO.Path.Combine(
                Android.App.Application.Context.ExternalCacheDir?.AbsolutePath
                    ?? throw new InvalidOperationException(
                        "Android external cache directory is unavailable."
                    ),
                "doroti-maui-evidence.json"
            );
#endif
            TryWriteText(path, json);
            Interlocked.Exchange(ref _lastEvidenceWriteTimestamp, timestamp);
            Interlocked.Exchange(ref _lastEvidenceReplayed, diagnostics.Frame.Replayed);
#if MACOS
            if (firstReplay)
            {
                TryExitAfterEvidence();
            }
#endif
        }

        if (shouldRequestReplay)
        {
            _renderSurface.Dispatcher.Dispatch(_renderSurface.InvalidateSurface);
        }
    }

#if MACOS
    private static void TryExitAfterEvidence()
    {
        var bridgePath = Environment.GetEnvironmentVariable("DOROTI_NATIVE_BRIDGE_EVIDENCE");
        if (
            string.Equals(
                Environment.GetEnvironmentVariable("DOROTI_EXIT_AFTER_EVIDENCE"),
                "1",
                StringComparison.Ordinal
            )
            && !string.IsNullOrWhiteSpace(bridgePath)
            && File.Exists(bridgePath)
        )
        {
            Environment.Exit(0);
        }
    }
#endif

    internal static void WriteFailure(Exception exception)
    {
        var path = ResolveEvidencePath();
#if ANDROID
        Android.Util.Log.Error("DorotiMauiFailure", exception.ToString());
        path = System.IO.Path.Combine(
            Android.App.Application.Context.ExternalCacheDir?.AbsolutePath
                ?? throw new InvalidOperationException(
                    "Android external cache directory is unavailable."
                ),
            "doroti-maui-evidence.exception.txt"
        );
#endif
        TryWriteText(
#if ANDROID
            path,
#else
            string.IsNullOrWhiteSpace(path) ? null : path + ".exception.txt",
#endif
            exception.ToString());
    }

    private static string? ResolveEvidencePath()
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_MAUI_EVIDENCE");
#if IOS || MACCATALYST
        if (path is { Length: > 0 } && path != "1" && !System.IO.Path.IsPathRooted(path))
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), path);
#endif
        return path == "1"
            ? System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "doroti-maui-evidence.json"
            )
            : path;
    }

    private static bool EvidenceEnabled()
    {
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOROTI_MAUI_EVIDENCE")))
        {
            return true;
        }
#if ANDROID
        return string.Equals(
            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Intent?.GetStringExtra(
                "DOROTI_MAUI_EVIDENCE"
            ),
            "1",
            StringComparison.Ordinal
        );
#else
        return false;
#endif
    }

    private static void TryWriteText(string? path, string contents)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        string? temporary = null;
        try
        {
            var directory = System.IO.Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

#if IOS && !MACCATALYST
            // Keep a container read on one complete version while the next
            // diagnostic snapshot is written. Never truncate the visible file.
            temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            File.WriteAllText(temporary, contents);
            File.Move(temporary, path, overwrite: true);
            temporary = null;
#else
            File.WriteAllText(path, contents);
#endif
        }
        catch (Exception)
        {
            // Evidence must never fail the GPU paint or startup path.
        }
        finally
        {
            if (temporary is not null)
            {
                try { File.Delete(temporary); }
                catch (Exception) { }
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
#if IOS && !MACCATALYST
        if (_renderSurface is MauiSkglSurface graphitePulse)
            graphitePulse.FrameworkFramePrepared -= ScheduleEvidenceWrite;
#endif
        _renderSurface.Paint -= PaintGpuSurface;
        _renderSurface.PresentCompleted -= CompleteNativePaint;
        _renderSurface.PaintFailed -= HandlePaintFailure;
        HandlerChanged -= HandleHandlerChanged;
        Loaded -= HandleLoaded;
        Unloaded -= HandleUnloaded;
        if (Application.Current is { } currentApplication)
        {
            currentApplication.RequestedThemeChanged -= HandleRequestedThemeChanged;
        }

        DetachWindow();
        // Unmount widgets before unregistering the view and retiring its capabilities.
        PrepareFrameworkClose();
        if (_host is null)
        {
            _renderSurface.Dispose();
        }

        _host?.Dispose();
        if (SharedFramework is null) _ownedFramework?.Dispose();
        _boundary?.Dispose();
        _host = null;
        _session = null;
        _ownedFramework = null;
        _boundary = null;
        _textInput.Dispose();
        SurfaceDisposed?.Invoke(this);
    }

    /// <summary>Detaches the framework branch before releasing its view services and native GPU consumers.</summary>
    public ValueTask DisposeAsync()
    {
        lock (_disposeGate)
        {
            if (_disposeTask is not null && !_disposeTask.IsFaulted) return new(_disposeTask);
            var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _disposeTask = completion.Task;
            _ = CompleteDisposeAsync(completion);
            return new(_disposeTask);
        }
    }

    private async Task CompleteDisposeAsync(TaskCompletionSource completion)
    {
        try { await DisposeCoreAsync(); completion.TrySetResult(); }
        catch (Exception error) { completion.TrySetException(error); }
    }

    private async Task DisposeCoreAsync()
    {
        if (_disposed) return;
        await new MauiApplicationDispatcher().InvokeAsync(() =>
        {
            _closing = true;
            PrepareFrameworkClose();
        });
        if (FrameworkView is { } view) await view.DrainInvocationsAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(30));
#if IOS || MACCATALYST
        var retirement = await new MauiApplicationDispatcher().InvokeAsync(() =>
            _renderSurface.Element.Handler?.PlatformView is DorotiUIKitGraphiteView native ? native.RetireAsync() : Task.CompletedTask);
        await retirement.WaitAsync(TimeSpan.FromSeconds(30));
#elif MACOS
        var retirement = await new MauiApplicationDispatcher().InvokeAsync(() =>
            DesktopMetalSurface.NativeView?.RetireAsync() ?? Task.CompletedTask);
        await retirement.WaitAsync(TimeSpan.FromSeconds(30));
#elif WINDOWS
        await PrepareDesktopCloseAsync();
#elif ANDROID
        var retirement = await new MauiApplicationDispatcher().InvokeAsync(() =>
            _renderSurface.Element.Handler?.PlatformView is DorotiAndroidViewContainer native
                ? native.Surface.RetireAsync() : Task.CompletedTask);
        await retirement.WaitAsync(TimeSpan.FromSeconds(30));
#endif
        await new MauiApplicationDispatcher().InvokeAsync(Dispose);
    }

    private static T CreateHiddenInput<T>()
        where T : InputView, new()
    {
        var input = new T
        {
            // Keep the native IME proxy in the visual tree without allowing even a
            // faint native pixel to leak into the Skia-owned scene. Flutter's host
            // text input is likewise fully transparent rather than nearly transparent.
            Opacity = 0,
            WidthRequest = 1,
            HeightRequest = 1,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            ZIndex = 1,
        };
        AutomationProperties.SetExcludedWithChildren(input, true);
        AutomationProperties.SetIsInAccessibleTree(input, false);
        return input;
    }

    private void HandleLoaded(object? sender, EventArgs args)
    {
#if IOS
        // This switch is process-wide, so only the standalone Doroti application owns it.
        if (Application.Current is DorotiMauiApplication)
        {
            Microsoft.Maui.Platform.KeyboardAutoManagerScroll.Disconnect();
        }
#endif
        if (Window is not { } window || ReferenceEquals(window, _window))
        {
            return;
        }

        DetachWindow();
        _window = window;
        window.Activated += HandleActivated;
        window.Deactivated += HandleDeactivated;
        window.Resumed += HandleResumed;
        window.Stopped += HandleStopped;
        window.Destroying += HandleDestroying;
#if WINDOWS
        if (
            !DesktopManaged
            && window.Handler?.PlatformView is Microsoft.UI.Xaml.Window backdropWindow
        )
        {
            _windowsBackdrop = new(
                backdropWindow,
                _application.ViewConfiguration.ResolveAppearance().ResolveBackdrop(isMacOS: false),
                OwnsWindowContent && WindowsNativeCaption.IsEnabled(backdropWindow)
            );
        }
        if (
            !DesktopManaged
            && window.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow
            && WindowsCompositionSurfaceFeature.GraphiteEnabled
        )
        {
            _closingWindow = nativeWindow;
            nativeWindow.AppWindow.Closing += HandleNativeClosing;
        }
#endif
        _host?.NotifyLifecycle(_viewId, AppLifecycleState.resumed);
    }

    private void HandleUnloaded(object? sender, EventArgs args) =>
        _host?.NotifyLifecycle(_viewId, AppLifecycleState.detached);

    private void HandleActivated(object? sender, EventArgs args)
    {
#if WINDOWS
        _textInput.Resume();
#endif
        _host?.NotifyLifecycle(_viewId, AppLifecycleState.resumed);
    }

    private void HandleDeactivated(object? sender, EventArgs args)
    {
#if WINDOWS
        _textInput.Suspend();
#endif
        _host?.NotifyLifecycle(_viewId, AppLifecycleState.inactive);
    }

    private void HandleResumed(object? sender, EventArgs args)
    {
#if WINDOWS
        _textInput.Resume();
#endif
        _host?.NotifyLifecycle(_viewId, AppLifecycleState.resumed);
    }

    private void HandleStopped(object? sender, EventArgs args)
    {
#if WINDOWS
        _textInput.Suspend();
#endif
        _host?.NotifyLifecycle(_viewId, AppLifecycleState.paused);
    }

    private void HandleRequestedThemeChanged(object? sender, AppThemeChangedEventArgs args)
    {
#if MACOS || WINDOWS
        BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent;
#else
        var color = ResolveBackgroundColor(args.RequestedTheme);
        BackgroundColor = new Microsoft.Maui.Graphics.Color(
            (float)color.r,
            (float)color.g,
            (float)color.b,
            (float)color.a
        );
#endif
    }

    private Color ResolveBackgroundColor(AppTheme theme) =>
        theme == AppTheme.Dark
            ? _application.ViewConfiguration.darkBackgroundColor
                ?? _application.ViewConfiguration.backgroundColor
                ?? new Color(0xff141218L)
            : _application.ViewConfiguration.backgroundColor ?? new Color(0xfffffbfeL);

#if WINDOWS
    private async void HandleNativeClosing(
        Microsoft.UI.Windowing.AppWindow sender,
        Microsoft.UI.Windowing.AppWindowClosingEventArgs args
    )
    {
        if (_closeReady)
        {
            return;
        }

        args.Cancel = true;
        if (_closeStarted)
        {
            return;
        }

        _closeStarted = true;
        var retirement = ((DorotiWindowsDxgiSurface)_renderSurface).PrepareForCloseAsync();
        sender.Hide();
        try
        {
            try
            {
                await retirement.WaitAsync(TimeSpan.FromSeconds(5));
            }
            catch (TimeoutException exception)
            {
                WriteFailure(exception); // Faulted hold; retain this generation until actual completion.
                await retirement;
            }
            _closeReady = true;
            _closingWindow?.Close();
        }
        catch (Exception exception)
        {
            // A failed completion cannot authorize native disposal or a replacement renderer.
            WriteFailure(exception);
        }
    }
#endif

    private void HandleDestroying(object? sender, EventArgs args)
    {
#if ANDROID
        // An OS Activity recreation does not close the logical application view.
        // Its native handler/surface retires separately and can reconnect.
        var activity = _window?.Handler?.PlatformView as Android.App.Activity
            ?? Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activity is { IsFinishing: false })
        {
            _host?.NotifyLifecycle(_viewId, AppLifecycleState.paused);
            return;
        }
#endif
        _host?.NotifyCloseRequested(_viewId);
        _host?.NotifyLifecycle(_viewId, AppLifecycleState.detached);
#if WINDOWS
        // Release Doroti's timers and render workers before the WinUI Closed
        // lifecycle handler ends the desktop application message loop.
        Dispose();
#elif (IOS && !MACCATALYST) || ANDROID
        _ = DisposeDestroyedViewAsync();
#endif
    }

#if (IOS && !MACCATALYST) || ANDROID
    private async Task DisposeDestroyedViewAsync()
    {
        try { await DisposeAsync(); }
        catch (Exception failure) { WriteFailure(failure); }
    }
#endif

    private void DetachWindow()
    {
        if (_window is null)
        {
            return;
        }

        _window.Activated -= HandleActivated;
        _window.Deactivated -= HandleDeactivated;
        _window.Resumed -= HandleResumed;
        _window.Stopped -= HandleStopped;
        _window.Destroying -= HandleDestroying;
#if WINDOWS
        _windowsBackdrop?.Dispose();
        _windowsBackdrop = null;
        if (_closingWindow is { } nativeWindow)
        {
            nativeWindow.AppWindow.Closing -= HandleNativeClosing;
        }

        _closingWindow = null;
#endif
        _window = null;
    }
}
