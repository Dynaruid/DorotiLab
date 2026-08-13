using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using Doroti.Core;
using Doroti.Graphics;
using Doroti.Platform;
using DorotiSize = Doroti.Graphics.Size;

namespace Doroti.Host.Avalonia;

public sealed class AvaloniaWindowBackend : IWindowBackend
{
    private readonly AvaloniaHostDiagnostics _diagnostics;
    private ulong _nextWindowId;

    public AvaloniaWindowBackend(AvaloniaHostRenderingMode renderingMode)
    {
        if (Application.Current is null)
        {
            throw new InvalidOperationException("Avalonia must be initialized before creating the Doroti host backend.");
        }
        if (!Dispatcher.UIThread.CheckAccess())
        {
            throw new InvalidOperationException("The Avalonia host backend must be created on the UI thread.");
        }
        _diagnostics = new(renderingMode);
    }

    public IAvaloniaHostDiagnostics Diagnostics => _diagnostics;

    public IWindow CreateWindow(WindowConfiguration configuration, IWindowEventSink eventSink)
    {
        ArgumentNullException.ThrowIfNull(eventSink);
        if (!configuration.InitialSize.IsFinite || configuration.InitialSize.IsEmpty)
        {
            throw new ArgumentOutOfRangeException(nameof(configuration), "The initial window size must be finite and positive.");
        }
        RequireUiThread();
        return new AvaloniaWindow(new(++_nextWindowId), configuration, eventSink, _diagnostics);
    }

    internal static void RequireUiThread()
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            throw new InvalidOperationException("Avalonia window operations must run on the UI thread.");
        }
    }
}

internal sealed class AvaloniaWindow : IWindow, IWindowPlacementController, IAvaloniaWindowCapture
{
    private readonly IWindowEventSink _eventSink;
    private readonly AvaloniaHostDiagnostics _diagnostics;
    private readonly Window _window;
    private readonly AvaloniaDisplayListControl _scene = new();
    private readonly AvaloniaRawInputSource _rawInput;
    private readonly AvaloniaTextInputConnection _textInput;
    private readonly AvaloniaCursorController _cursor;
    private readonly AvaloniaClipboard _clipboard;
    private readonly AvaloniaAccessibilityBridge _accessibility;
    private readonly AvaloniaFrameDispatcher _frameDispatcher;
    private readonly AvaloniaFramePipeline _framePipeline;
    private WindowMetrics _metrics;
    private long _metricsGeneration = 1;
    private long _scaleGeneration = 1;
    private long _surfaceGeneration = 1;
    private bool _shown;
    private bool _closed;
    private bool _disposed;

    internal AvaloniaWindow(
        WindowId id,
        WindowConfiguration configuration,
        IWindowEventSink eventSink,
        AvaloniaHostDiagnostics diagnostics)
    {
        Id = id;
        _eventSink = eventSink;
        _diagnostics = diagnostics;
        _metrics = new(
            configuration.InitialSize,
            PixelExtentPolicy.ToPixelSize(configuration.InitialSize, 1),
            1,
            false,
            _metricsGeneration,
            _scaleGeneration,
            _surfaceGeneration);
        _window = new()
        {
            Title = configuration.Title,
            Width = configuration.InitialSize.Width,
            Height = configuration.InitialSize.Height,
            Content = _scene,
            Background = null,
        };
        _scene.Attach(_window, Id, () => _metrics, _diagnostics);
        _rawInput = new(_scene, _window, Id, () => _metrics, _diagnostics);
        _textInput = new(_scene, Id, () => _metrics, _diagnostics);
        _cursor = new(_scene, Id);
        _clipboard = new(_window);
        _accessibility = new(_scene, Id, () => _metrics, _diagnostics);
        _frameDispatcher = new(_diagnostics, Id, () => _metrics);
        _framePipeline = new(_scene);
        _window.Opened += OnOpened;
        _window.Activated += OnActivated;
        _window.Deactivated += OnDeactivated;
        _window.Resized += OnResized;
        _window.ScalingChanged += OnScalingChanged;
        _window.PropertyChanged += OnPropertyChanged;
        _window.Closing += OnClosing;
        _window.Closed += OnClosed;
        _diagnostics.Record("created", Id, _metrics, configuration.Title);
    }

    public WindowId Id { get; }

    public WindowMetrics Metrics => _metrics;

    public bool IsClosed => _closed;

    public IRawInputSource RawInput => _rawInput;

    public ITextInputConnection TextInput => _textInput;

    public ICursorController Cursor => _cursor;

    public IReadOnlyList<DisplayInfo> Displays
    {
        get
        {
            AvaloniaWindowBackend.RequireUiThread();
            return (_window.Screens?.All ?? []).Select(screen => new DisplayInfo(
                new(ScreenId(screen)),
                new(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Right, screen.WorkingArea.Bottom))).ToArray();
        }
    }

    public void Show()
    {
        AvaloniaWindowBackend.RequireUiThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_shown)
        {
            return;
        }
        _shown = true;
        _diagnostics.Record("shown", Id, _metrics);
        _window.Show();
    }

    public void Resize(DorotiSize logicalSize)
    {
        AvaloniaWindowBackend.RequireUiThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!logicalSize.IsFinite || logicalSize.IsEmpty)
        {
            throw new ArgumentOutOfRangeException(nameof(logicalSize), "The logical window size must be finite and positive.");
        }
        _window.Width = logicalSize.Width;
        _window.Height = logicalSize.Height;
    }

    public void SetMinimized(bool minimized)
    {
        AvaloniaWindowBackend.RequireUiThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        _window.WindowState = minimized ? WindowState.Minimized : WindowState.Normal;
    }

    public void MoveToDisplay(DisplayId display)
    {
        AvaloniaWindowBackend.RequireUiThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        var screen = (_window.Screens?.All ?? []).SingleOrDefault(candidate => ScreenId(candidate) == display.Value)
            ?? throw new ArgumentOutOfRangeException(nameof(display), $"Display {display.Value} is not connected.");
        var area = screen.WorkingArea;
        _window.Position = new(
            area.X + Math.Max(0, (area.Width - (int)Math.Round(_window.Bounds.Width * screen.Scaling)) / 2),
            area.Y + Math.Max(0, (area.Height - (int)Math.Round(_window.Bounds.Height * screen.Scaling)) / 2));
        _diagnostics.Record("display-move-requested", Id, _metrics, $"display={display.Value};scale={screen.Scaling:0.###}");
    }

    public AvaloniaWindowCapture CaptureWindow(string screenshotPath)
    {
        AvaloniaWindowBackend.RequireUiThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(screenshotPath);
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Native window screenshots are currently available only on Windows target verification.");
        }
        return WindowsWindowCapture.Capture(_window, _metrics.ScaleFactor, screenshotPath);
    }

    private static ulong ScreenId(Screen screen) => screen.TryGetPlatformHandle() is { } handle
        ? unchecked((ulong)handle.Handle.ToInt64())
        : unchecked((ulong)screen.GetHashCode());

    public void Close()
    {
        AvaloniaWindowBackend.RequireUiThread();
        if (!_closed)
        {
            _window.Close();
        }
    }

    public bool TryGetFeature<TFeature>(out TFeature? feature)
        where TFeature : class
    {
        feature = this as TFeature ??
            _scene as TFeature ??
            _rawInput as TFeature ??
            _framePipeline as TFeature ??
            _frameDispatcher as TFeature ??
            _clipboard as TFeature ??
            _textInput as TFeature ??
            _accessibility as TFeature ??
            _diagnostics as TFeature;
        return feature is not null;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        AvaloniaWindowBackend.RequireUiThread();
        _disposed = true;
        _framePipeline.Dispose();
        _rawInput.Dispose();
        _cursor.Dispose();
        Close();
        _scene.DisposeFrameResources();
        _textInput.Dispose();
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        UpdateMetrics("opened");
    }

    private void OnActivated(object? sender, EventArgs e) => _diagnostics.Record("activated", Id, _metrics);

    private void OnDeactivated(object? sender, EventArgs e) => _diagnostics.Record("deactivated", Id, _metrics);

    private void OnResized(object? sender, WindowResizedEventArgs e) => UpdateMetrics("resized", e.Reason.ToString());

    private void OnScalingChanged(object? sender, EventArgs e) => UpdateMetrics("dpi-changed", $"scale={_window.RenderScaling:0.###}");

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Window.WindowStateProperty)
        {
            var minimized = _window.WindowState == WindowState.Minimized;
            UpdateMetrics(minimized ? "minimized" : "restored", _window.WindowState.ToString());
        }
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        _diagnostics.Record("close-requested", Id, _metrics, e.CloseReason.ToString());
        _eventSink.OnCloseRequested(Id);
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (_closed)
        {
            return;
        }
        _closed = true;
        _diagnostics.Record("closed", Id, _metrics);
        _eventSink.OnClosed(Id);
    }

    private void UpdateMetrics(string kind, string? detail = null)
    {
        var logical = _window.ClientSize;
        if (logical.Width <= 0 || logical.Height <= 0)
        {
            logical = new(_metrics.LogicalSize.Width, _metrics.LogicalSize.Height);
        }
        var logicalSize = new DorotiSize(logical.Width, logical.Height);
        var scale = _window.RenderScaling;
        var pixelSize = PixelExtentPolicy.ToPixelSize(logicalSize, scale);
        if (_metrics.ScaleFactor != scale)
        {
            _scaleGeneration++;
        }
        if (_metrics.LogicalSize != logicalSize || _metrics.PixelSize != pixelSize || _metrics.ScaleFactor != scale)
        {
            _surfaceGeneration++;
        }
        _metrics = new(
            logicalSize,
            pixelSize,
            scale,
            _window.WindowState == WindowState.Minimized,
            ++_metricsGeneration,
            _scaleGeneration,
            _surfaceGeneration);
        _diagnostics.Record(kind, Id, _metrics, detail);
        _eventSink.OnMetricsChanged(Id, _metrics);
    }
}
