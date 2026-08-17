using System.Text.Json;
using Doroti.Hosting;
using Doroti.Ui;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Doroti.Host.Maui;

public sealed class DorotiMauiSurface : Grid, IDisposable
{
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
        _viewId, "src/App.cs", OperatingSystem.IsWindows()
            ? "Platforms/Windows/App.xaml.cs"
            : "obj/maccatalyst/Doroti.Generated/DorotiBootstrap.g.cs -> Platforms/MacCatalyst/AppDelegate.cs");

    private void HandleHandlerChanged(object? sender, EventArgs args)
    {
        _ = sender;
        _ = args;
        if (Handler is null || _attached || _disposed) return;
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

    private void PaintGpuSurface(object? sender, SKPaintGLSurfaceEventArgs args)
    {
        _ = sender;
        if (!_attached || _host is null) return;
        if (args.Surface is null || _skiaView.GRContext is null)
            throw new InvalidOperationException("Strict Doroti MAUI mode requires a GPU-backed SKSurface and GRContext.");
        var nativeType = _skiaView.Handler?.PlatformView?.GetType().FullName ?? "unknown";
        _host.BeginPaint(_viewId, args, _skiaView.GRContext, nativeType);
        _host.PaintSkiaSurface(_viewId, args.Surface, args.BackendRenderTarget.Width, args.BackendRenderTarget.Height);
        WriteEvidence();
    }

    private void WriteEvidence()
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_MAUI_EVIDENCE");
        var diagnostics = Diagnostics;
        if (string.IsNullOrWhiteSpace(path) || diagnostics is null) return;
        File.WriteAllText(path, JsonSerializer.Serialize(diagnostics, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        }));
        var quitFrames = int.TryParse(Environment.GetEnvironmentVariable("DOROTI_MAUI_AUTO_QUIT_FRAMES"), out var value)
            ? value : 0;
        if (quitFrames > 0 && diagnostics.Frame.Presented >= quitFrames)
        {
            if (diagnostics.Frame.Replayed > 0)
                Dispatcher.Dispatch(() => Application.Current?.Quit());
            else
                Dispatcher.Dispatch(_skiaView.InvalidateSurface);
        }
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
