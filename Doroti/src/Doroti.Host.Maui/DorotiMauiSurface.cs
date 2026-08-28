using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.Maui;

public sealed class DorotiMauiSurface : Grid, IDisposable
{
    private static readonly JsonSerializerOptions EvidenceJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };
    private static readonly TimeSpan EvidenceWriteInterval = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan EvidenceWriteQuiescence = TimeSpan.FromMilliseconds(250);
    private readonly ulong _viewId;
    private readonly DorotiApplicationDescriptor _application;
    private DorotiApplicationBoundary? _boundary;
    private DorotiHostSession? _session;
    private MauiFrameworkHost? _host;
    private readonly IMauiSkiaSurface _renderSurface;
    private readonly Entry _singleLineInput;
    private readonly Editor _multilineInput;
    private readonly MauiTextInputBridge _textInput;
    private readonly AbsoluteLayout _semanticsLayer;
    private bool _attached;
    private bool _disposed;
    private long _lastEvidenceWriteTimestamp;
    private long _lastEvidenceReplayed;
    private long _evidenceWriteGeneration;
    private int _evidenceWritePending;
    private Window? _window;

    public DorotiMauiSurface(
        DorotiApplicationDescriptor application,
        ulong viewId = 1)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
        _viewId = viewId;
        var startupColor = ResolveBackgroundColor(Application.Current?.RequestedTheme ?? AppTheme.Unspecified);
        BackgroundColor = new Microsoft.Maui.Graphics.Color(
            (float)startupColor.r, (float)startupColor.g, (float)startupColor.b, (float)startupColor.a);
        _singleLineInput = CreateHiddenInput<Entry>();
        _multilineInput = CreateHiddenInput<Editor>();
        _semanticsLayer = new AbsoluteLayout { InputTransparent = true, CascadeInputTransparent = false };
        _textInput = new(_singleLineInput, _multilineInput, this, attachOnDemand: true);
#if MACOS
        _renderSurface = new DorotiMacOSMetalSurface(_viewId);
#elif WINDOWS
        _renderSurface = new DorotiWindowsDxgiSurface();
#else
        _renderSurface = new MauiSkglSurface(_textInput, _viewId);
#endif
        Children.Add(_renderSurface.Element);
        Children.Add(_semanticsLayer);
        _renderSurface.Paint += PaintGpuSurface;
        _renderSurface.PresentCompleted += CompleteNativePaint;
        _renderSurface.PaintFailed += HandlePaintFailure;
        HandlerChanged += HandleHandlerChanged;
        Loaded += HandleLoaded;
        Unloaded += HandleUnloaded;
        if (Application.Current is { } currentApplication)
            currentApplication.RequestedThemeChanged += HandleRequestedThemeChanged;
    }

    public MauiHostDiagnostics? Diagnostics => _host?.CaptureDiagnostics(
        _viewId, "src/App.cs",
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
#else
#error Doroti.Host.Maui requires an explicit bootstrap source.
#endif
        );

    private void HandleHandlerChanged(object? sender, EventArgs args)
    {
        _ = sender;
        _ = args;
        if (Handler is null || _attached || _disposed) return;
        try
        {
            _session = new(_application.EntrypointFactory());
            _host = new();
            _session.Start(deferFrameworkBootstrap: true);
            _boundary = DorotiApplicationBoundary.Load(
                _application.ManifestAssembly,
                _application.ApplicationAssembly,
                _application.LaunchContext.RuntimeIdentifier,
                _application.NativePluginHandlers);
            _host.CreateView(_session, _viewId, _renderSurface, _application.ViewConfiguration,
                new MauiSemanticsBridge(_semanticsLayer), _boundary, _textInput);
            using (var dispatcherScope = _session.dispatcher.EnterScope())
                _session.dispatcher.setSemanticsTreeEnabled(true);
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
        if (!_attached || _host is null) return;
        try
        {
            try
            {
                _host.BeginPaint(_viewId, paint);
                if (!paint.SkipRaster)
                {
                    paint.Completion = _host.PaintSkiaSurface(
                        _viewId, paint.Surface, paint.PixelWidth, paint.PixelHeight,
                        out var shouldPresent);
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
        if (_disposed || _host is null) return;
        if (stale)
        {
            _host.FailPaint(_viewId, completion,
                "Metal completion belongs to a stale AppKit surface generation.");
            ScheduleEvidenceWrite();
            return;
        }
        _host.CompletePaint(_viewId, completion);
        ScheduleEvidenceWrite();
    }

    private void HandlePaintFailure(MauiPaintCompletion? completion, Exception exception)
    {
        if (completion is { } value && _host is not null)
            _host.FailPaint(_viewId, value, exception.Message);
        WriteFailure(exception);
        ScheduleEvidenceWrite();
    }

    private void ScheduleEvidenceWrite()
    {
        if (!EvidenceEnabled()) return;
        var generation = Interlocked.Increment(ref _evidenceWriteGeneration);
        if (Interlocked.CompareExchange(ref _evidenceWritePending, 1, 0) != 0) return;
        _ = Task.Run(() =>
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(EvidenceWriteQuiescence);
                    var latestGeneration = Interlocked.Read(ref _evidenceWriteGeneration);
                    if (latestGeneration == generation) break;
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
                    ScheduleEvidenceWrite();
            }
        });
    }

    private void WriteEvidence()
    {
        var diagnostics = Diagnostics;
        if (diagnostics is null) return;
        var evidencePath = Environment.GetEnvironmentVariable("DOROTI_MAUI_EVIDENCE");
        var shouldRequestReplay = diagnostics.Frame.Presented > 0 && diagnostics.Frame.Replayed == 0;
        var timestamp = Stopwatch.GetTimestamp();
        var lastWrite = Interlocked.Read(ref _lastEvidenceWriteTimestamp);
        var lastReplay = Interlocked.Read(ref _lastEvidenceReplayed);
        var firstEvidence = lastWrite == 0;
        var firstReplay = diagnostics.Frame.Replayed > 0 && lastReplay == 0;
        var intervalElapsed = !firstEvidence &&
                              Stopwatch.GetElapsedTime(lastWrite, timestamp) >= EvidenceWriteInterval;

        // Evidence collection is coalesced onto one background writer. JSON serialization and
        // file I/O must never occupy the native paint callback while an interaction is active.
        if (firstEvidence || firstReplay || intervalElapsed)
        {
            var json = JsonSerializer.Serialize(diagnostics, EvidenceJsonOptions);
            var path = evidencePath;
#if ANDROID
            path = System.IO.Path.Combine(Android.App.Application.Context.ExternalCacheDir?.AbsolutePath
                ?? throw new InvalidOperationException("Android external cache directory is unavailable."), "doroti-maui-evidence.json");
#endif
            TryWriteText(path, json);
            Interlocked.Exchange(ref _lastEvidenceWriteTimestamp, timestamp);
            Interlocked.Exchange(ref _lastEvidenceReplayed, diagnostics.Frame.Replayed);
#if MACOS
            if (firstReplay) TryExitAfterEvidence();
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
        if (string.Equals(Environment.GetEnvironmentVariable("DOROTI_EXIT_AFTER_EVIDENCE"), "1", StringComparison.Ordinal) &&
            !string.IsNullOrWhiteSpace(bridgePath) && File.Exists(bridgePath))
        {
            Environment.Exit(0);
        }
    }
#endif

    internal static void WriteFailure(Exception exception)
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_MAUI_EVIDENCE");
#if ANDROID
        Android.Util.Log.Error("DorotiMauiFailure", exception.ToString());
        path = System.IO.Path.Combine(Android.App.Application.Context.ExternalCacheDir?.AbsolutePath
            ?? throw new InvalidOperationException("Android external cache directory is unavailable."), "doroti-maui-evidence.exception.txt");
#endif
        TryWriteText(
#if ANDROID
            path,
#else
            string.IsNullOrWhiteSpace(path) ? null : path + ".exception.txt",
#endif
            exception.ToString());
    }

    private static bool EvidenceEnabled()
    {
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOROTI_MAUI_EVIDENCE"))) return true;
#if ANDROID
        return string.Equals(
            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Intent?.GetStringExtra("DOROTI_MAUI_EVIDENCE"),
            "1",
            StringComparison.Ordinal);
#else
        return false;
#endif
    }

    private static void TryWriteText(string? path, string contents)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        try
        {
            var directory = System.IO.Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(path, contents);
        }
        catch (Exception)
        {
            // Evidence must never fail the GPU paint or startup path.
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _renderSurface.Paint -= PaintGpuSurface;
        _renderSurface.PresentCompleted -= CompleteNativePaint;
        _renderSurface.PaintFailed -= HandlePaintFailure;
        HandlerChanged -= HandleHandlerChanged;
        Loaded -= HandleLoaded;
        Unloaded -= HandleUnloaded;
        if (Application.Current is { } currentApplication)
            currentApplication.RequestedThemeChanged -= HandleRequestedThemeChanged;
        DetachWindow();
        if (_host is null) _renderSurface.Dispose();
        _host?.Dispose();
        _session?.Dispose();
        _boundary?.Dispose();
        _host = null;
        _session = null;
        _boundary = null;
        _textInput.Dispose();
    }

    private static T CreateHiddenInput<T>() where T : InputView, new()
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
        if (Window is not { } window || ReferenceEquals(window, _window)) return;
        DetachWindow();
        _window = window;
        window.Activated += HandleActivated;
        window.Deactivated += HandleDeactivated;
        window.Resumed += HandleResumed;
        window.Stopped += HandleStopped;
        window.Destroying += HandleDestroying;
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
        var color = ResolveBackgroundColor(args.RequestedTheme);
        BackgroundColor = new Microsoft.Maui.Graphics.Color(
            (float)color.r, (float)color.g, (float)color.b, (float)color.a);
    }

    private Doroti.Ui.Color ResolveBackgroundColor(AppTheme theme) =>
        theme == AppTheme.Dark
            ? _application.ViewConfiguration.darkBackgroundColor ??
              _application.ViewConfiguration.backgroundColor ?? new Doroti.Ui.Color(0xff141218L)
            : _application.ViewConfiguration.backgroundColor ?? new Doroti.Ui.Color(0xfffffbfeL);

    private void HandleDestroying(object? sender, EventArgs args)
    {
        _host?.NotifyCloseRequested(_viewId);
        _host?.NotifyLifecycle(_viewId, AppLifecycleState.detached);
#if WINDOWS
        // Release Doroti's timers and render workers before the WinUI Closed
        // lifecycle handler ends the desktop application message loop.
        Dispose();
#endif
    }

    private void DetachWindow()
    {
        if (_window is null) return;
        _window.Activated -= HandleActivated;
        _window.Deactivated -= HandleDeactivated;
        _window.Resumed -= HandleResumed;
        _window.Stopped -= HandleStopped;
        _window.Destroying -= HandleDestroying;
        _window = null;
    }
}
