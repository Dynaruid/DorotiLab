using System.Text.Json;
using Doroti.Hosting;
using Doroti.Ui;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Doroti.Host.Maui;

public sealed class DorotiMauiSurface : SKGLView, IDisposable
{
    private readonly ulong _viewId;
    private readonly DorotiApplicationDescriptor _application;
    private DorotiApplicationBoundary? _boundary;
    private DorotiHostSession? _session;
    private MauiFrameworkHost? _host;
    private bool _attached;
    private bool _disposed;

    public DorotiMauiSurface(
        DorotiApplicationDescriptor application,
        ulong viewId = 1)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
        _viewId = viewId;
        HasRenderLoop = false;
        EnableTouchEvents = true;
        PaintSurface += PaintGpuSurface;
        HandlerChanged += HandleHandlerChanged;
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
        _host.CreateView(_session, _viewId, this, _application.ViewConfiguration, application: _boundary);
        _attached = true;
    }

    private void PaintGpuSurface(object? sender, SKPaintGLSurfaceEventArgs args)
    {
        _ = sender;
        if (!_attached || _host is null) return;
        if (args.Surface is null || GRContext is null)
            throw new InvalidOperationException("Strict Doroti MAUI mode requires a GPU-backed SKSurface and GRContext.");
        var nativeType = Handler?.PlatformView?.GetType().FullName ?? "unknown";
        _host.BeginPaint(_viewId, args, GRContext, nativeType);
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
                Dispatcher.Dispatch(InvalidateSurface);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        PaintSurface -= PaintGpuSurface;
        HandlerChanged -= HandleHandlerChanged;
        _host?.Dispose();
        _session?.Dispose();
        _boundary?.Dispose();
        _host = null;
        _session = null;
        _boundary = null;
    }
}
