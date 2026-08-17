using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Ui;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Doroti.Host.Maui;

public sealed class DorotiMauiSurface : Grid, IDisposable
{
    private static readonly JsonSerializerOptions EvidenceJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };
    private static readonly TimeSpan EvidenceWriteInterval = TimeSpan.FromSeconds(1);
    private readonly ulong _viewId;
    private readonly DorotiApplicationDescriptor _application;
    private DorotiApplicationBoundary? _boundary;
    private DorotiHostSession? _session;
    private MauiFrameworkHost? _host;
    private readonly SKGLView _skiaView;
    private readonly Entry _singleLineInput;
    private readonly Editor _multilineInput;
    private readonly MauiTextInputBridge _textInput;
    private readonly AbsoluteLayout _semanticsLayer;
    private bool _attached;
    private bool _disposed;
    private long _lastEvidenceWriteTimestamp;
    private long _lastEvidenceReplayed;
    private Window? _window;

    public DorotiMauiSurface(
        DorotiApplicationDescriptor application,
        ulong viewId = 1)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
        _viewId = viewId;
        _skiaView = new SKGLView { HasRenderLoop = false, EnableTouchEvents = true };
        _singleLineInput = CreateHiddenInput<Entry>();
        _multilineInput = CreateHiddenInput<Editor>();
        _semanticsLayer = new AbsoluteLayout { InputTransparent = true, CascadeInputTransparent = false };
        _textInput = new(_singleLineInput, _multilineInput);
        Children.Add(_skiaView);
        Children.Add(_singleLineInput);
        Children.Add(_multilineInput);
        Children.Add(_semanticsLayer);
        _skiaView.PaintSurface += PaintGpuSurface;
        HandlerChanged += HandleHandlerChanged;
        Loaded += HandleLoaded;
        Unloaded += HandleUnloaded;
    }

    public MauiHostDiagnostics? Diagnostics => _host?.CaptureDiagnostics(
        _viewId, "src/App.cs",
#if WINDOWS
        "Platforms/Windows/App.xaml.cs"
#elif MACCATALYST
        "obj/maccatalyst/Doroti.Generated/DorotiBootstrap.g.cs -> Platforms/MacCatalyst/AppDelegate.cs"
#elif ANDROID
        RuntimeInformation.ProcessArchitecture == Architecture.X64
            ? "obj/android-x64/Doroti.Generated/DorotiBootstrap.g.cs -> Platforms/Android/MainApplication.cs"
            : "obj/android/Doroti.Generated/DorotiBootstrap.g.cs -> Platforms/Android/MainApplication.cs"
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
                _application.ApplicationAssembly,
                _application.LaunchContext.RuntimeIdentifier);
            _host.CreateView(_session, _viewId, _skiaView, _application.ViewConfiguration,
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

    private void PaintGpuSurface(object? sender, SKPaintGLSurfaceEventArgs args)
    {
        _ = sender;
        if (!_attached || _host is null) return;
        try
        {
            if (args.Surface is null || _skiaView.GRContext is null)
                throw new InvalidOperationException("Strict Doroti MAUI mode requires a GPU-backed SKSurface and GRContext.");
            var nativeType = _skiaView.Handler?.PlatformView?.GetType().FullName ?? "unknown";
            try
            {
                _host.BeginPaint(_viewId, args, _skiaView.GRContext, nativeType);
                _host.PaintSkiaSurface(_viewId, args.Surface, args.BackendRenderTarget.Width, args.BackendRenderTarget.Height);
                WriteEvidence();
            }
            finally
            {
                _host.EndPaint(_viewId);
            }
        }
        catch (Exception exception)
        {
            WriteFailure(exception);
            throw;
        }
    }

    private void WriteEvidence()
    {
        var diagnostics = Diagnostics;
        if (diagnostics is null) return;
        var quitFrames = GetAutoQuitFrames();
        var shouldRequestReplay = quitFrames > 0 && diagnostics.Frame.Presented >= quitFrames &&
                                  diagnostics.Frame.Replayed == 0;
        var shouldQuit = quitFrames > 0 && diagnostics.Frame.Presented >= quitFrames &&
                         diagnostics.Frame.Replayed > 0;
        var timestamp = Stopwatch.GetTimestamp();
        var firstEvidence = _lastEvidenceWriteTimestamp == 0;
        var firstReplay = diagnostics.Frame.Replayed > 0 && _lastEvidenceReplayed == 0;
        var intervalElapsed = !firstEvidence &&
                              Stopwatch.GetElapsedTime(_lastEvidenceWriteTimestamp, timestamp) >= EvidenceWriteInterval;

        // Evidence collection must not serialize, log, and synchronously rewrite a file on every
        // interactive frame. That work runs on the native paint path and can starve Android's
        // TextureView compositor while a drag is producing frames.
        if (firstEvidence || firstReplay || intervalElapsed || shouldQuit)
        {
            var json = JsonSerializer.Serialize(diagnostics, EvidenceJsonOptions);
            var path = Environment.GetEnvironmentVariable("DOROTI_MAUI_EVIDENCE");
#if ANDROID
            path = System.IO.Path.Combine(Android.App.Application.Context.CacheDir?.AbsolutePath
                ?? throw new InvalidOperationException("Android cache directory is unavailable."), "doroti-maui-evidence.json");
            Android.Util.Log.Info("DorotiMauiEvidence", json.ReplaceLineEndings(string.Empty));
#endif
            TryWriteText(path, json);
            _lastEvidenceWriteTimestamp = timestamp;
            _lastEvidenceReplayed = diagnostics.Frame.Replayed;
        }

        if (shouldQuit)
        {
            Dispatcher.Dispatch(QuitHost);
        }
        else if (shouldRequestReplay)
        {
            Dispatcher.Dispatch(_skiaView.InvalidateSurface);
        }
    }

    internal static void WriteFailure(Exception exception)
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_MAUI_EVIDENCE");
        if (string.IsNullOrWhiteSpace(path)) return;
        TryWriteText(path + ".exception.txt", exception.ToString());
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

    private static void QuitHost()
    {
        Application.Current?.Quit();
#if MACCATALYST
        Environment.Exit(0);
#endif
    }

    private static int GetAutoQuitFrames()
    {
        if (int.TryParse(Environment.GetEnvironmentVariable("DOROTI_MAUI_AUTO_QUIT_FRAMES"), out var value))
            return value;
#if ANDROID
        return Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Intent?
            .GetIntExtra("doroti_auto_quit_frames", 0) ?? 0;
#else
        return 0;
#endif
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _skiaView.PaintSurface -= PaintGpuSurface;
        HandlerChanged -= HandleHandlerChanged;
        Loaded -= HandleLoaded;
        Unloaded -= HandleUnloaded;
        DetachWindow();
        _host?.Dispose();
        _session?.Dispose();
        _boundary?.Dispose();
        _host = null;
        _session = null;
        _boundary = null;
        _textInput.Dispose();
    }

    private static T CreateHiddenInput<T>() where T : InputView, new() => new()
    {
        Opacity = 0.01,
        WidthRequest = 1,
        HeightRequest = 1,
        HorizontalOptions = LayoutOptions.Start,
        VerticalOptions = LayoutOptions.Start,
        ZIndex = 1,
    };

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

    private void HandleActivated(object? sender, EventArgs args) => _host?.NotifyLifecycle(_viewId, AppLifecycleState.resumed);
    private void HandleDeactivated(object? sender, EventArgs args) => _host?.NotifyLifecycle(_viewId, AppLifecycleState.inactive);
    private void HandleResumed(object? sender, EventArgs args) => _host?.NotifyLifecycle(_viewId, AppLifecycleState.resumed);
    private void HandleStopped(object? sender, EventArgs args) => _host?.NotifyLifecycle(_viewId, AppLifecycleState.paused);
    private void HandleDestroying(object? sender, EventArgs args)
    {
        _host?.NotifyCloseRequested(_viewId);
        _host?.NotifyLifecycle(_viewId, AppLifecycleState.detached);
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
