using System.Globalization;
using Doroti.Ui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Locale = Doroti.Ui.Locale;
using Rect = Doroti.Ui.Rect;
using Size = Doroti.Ui.Size;

namespace Doroti.Host.Maui;

internal sealed class MauiHostAdapter :
    IViewHostCapability,
    IFrameHostCapability,
    IPlatformEnvironmentHostCapability,
    IInputHostCapability,
    IViewFocusRequestCapability,
    IPlatformServicesHostCapability,
    ITextInputHostCapability
{
    private readonly ulong _viewId;
    private readonly SKGLView _view;
    private readonly object _gate = new();
    private readonly Queue<Action<TimeSpan>> _frameCallbacks = [];
    private readonly IMauiSemanticsBridge _semantics;
    private Size _logicalSize;
    private double _density;
    private long _metricsGeneration = 1;
    private long _contextGeneration;
    private long _surfaceGeneration;
    private long _invalidationsRequested;
    private long _invalidationsCoalesced;
    private bool _invalidatePending;
    private bool _disposed;

    internal MauiHostAdapter(ulong viewId, SKGLView view, Size logicalSize, IMauiSemanticsBridge? semantics = null)
    {
        _viewId = viewId;
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _logicalSize = logicalSize ?? throw new ArgumentNullException(nameof(logicalSize));
        _density = Math.Max(1, DeviceDisplay.Current.MainDisplayInfo.Density);
        _semantics = semantics ?? new NullMauiSemanticsBridge();
        _view.HasRenderLoop = false;
        _view.EnableTouchEvents = true;
        _view.Touch += HandleTouch;
        _view.SizeChanged += HandleSizeChanged;
    }

    internal MauiSurfaceSnapshot Snapshot { get; private set; } = new(
        0, 0, 1, 1, 0, 0, "not-attached", "not-attached");

    internal long InvalidationsRequested => Interlocked.Read(ref _invalidationsRequested);
    internal long InvalidationsCoalesced => Interlocked.Read(ref _invalidationsCoalesced);
    public ViewMetrics Metrics => new(
        new Size(_logicalSize.width * _density, _logicalSize.height * _density), _density,
        ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed,
        _metricsGeneration, _surfaceGeneration);
    public PlatformConfiguration Configuration => new(
        [ToLocale(CultureInfo.CurrentUICulture)],
        Application.Current?.RequestedTheme == AppTheme.Dark ? Brightness.dark : Brightness.light,
        false, false,
#if WINDOWS
        HostOperatingSystem.windows
#else
        HostOperatingSystem.macOS
#endif
    );

    public event Action<ViewMetrics>? MetricsChanged;
    public event Action<AppLifecycleState>? LifecycleChanged;
    public event Action? CloseRequested;
    public event Action? Closed;
    public event Action<PlatformConfiguration>? ConfigurationChanged;
    public event Action<PointerDataPacket>? PointerData;
    public event Action<KeyData>? KeyData;
    public event Action<RawFocusData>? FocusData;
    public event Action<DorotiTextEditingState>? EditingStateChanged;
    public event Action<DorotiTextInputAction>? ActionPerformed;

    public void Show()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        RequestInvalidate();
        LifecycleChanged?.Invoke(AppLifecycleState.resumed);
    }

    public void Resize(Size logicalSize)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _logicalSize = logicalSize;
        _metricsGeneration++;
        MetricsChanged?.Invoke(Metrics);
        RequestInvalidate();
    }

    public void ScheduleFrame(Action<TimeSpan> callback)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(callback);
        lock (_gate) _frameCallbacks.Enqueue(callback);
        RequestInvalidate();
    }

    internal void BeginPaint(SKPaintGLSurfaceEventArgs args, object? context, string nativeViewType, string backend)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_gate) _invalidatePending = false;
        var previous = Snapshot;
        if (context is not null && !ReferenceEquals(context, _lastContext))
        {
            _lastContext = context;
            _contextGeneration++;
            _surfaceGeneration++;
        }
        var pixelSizeChanged = previous.PixelWidth != args.BackendRenderTarget.Width ||
                               previous.PixelHeight != args.BackendRenderTarget.Height;
        if (pixelSizeChanged)
            _surfaceGeneration++;
        var density = Math.Max(1, DeviceDisplay.Current.MainDisplayInfo.Density);
        var densityChanged = Math.Abs(_density - density) > double.Epsilon;
        _density = density;
        if (pixelSizeChanged || densityChanged)
            _metricsGeneration++;
        Snapshot = new(args.BackendRenderTarget.Width, args.BackendRenderTarget.Height, _density,
            _metricsGeneration, _contextGeneration, _surfaceGeneration, nativeViewType, backend);
        if (pixelSizeChanged || densityChanged)
            MetricsChanged?.Invoke(Metrics);
        Action<TimeSpan>[] callbacks;
        lock (_gate)
        {
            callbacks = _frameCallbacks.ToArray();
            _frameCallbacks.Clear();
        }
        var now = TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
        foreach (var callback in callbacks) callback(now);
    }

    private object? _lastContext;

    internal void UpdateSemantics(string serializedTree) => _semantics.Update(serializedTree);

    internal void RequestInvalidate()
    {
        Interlocked.Increment(ref _invalidationsRequested);
        lock (_gate)
        {
            if (_invalidatePending)
            {
                Interlocked.Increment(ref _invalidationsCoalesced);
                return;
            }
            _invalidatePending = true;
        }
        _view.Dispatcher.Dispatch(_view.InvalidateSurface);
    }

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction)
    {
        _ = direction;
        FocusData?.Invoke(new(_viewId, state == ViewFocusState.focused, TimeSpan.FromTicks(DateTime.UtcNow.Ticks)));
    }

    public async ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await Clipboard.Default.GetTextAsync();
    }

    public async ValueTask SetClipboardTextAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Clipboard.Default.SetTextAsync(text ?? string.Empty);
    }

    public void SetCursor(DorotiMouseCursorKind cursor) => _ = cursor;
    public void SetClient(DorotiTextEditingState initialState) => _ = initialState;
    public void UpdateState(DorotiTextEditingState state) => _ = state;
    public void SetCaretRect(Rect logicalRect) => _ = logicalRect;
    public void ClearClient() { }

    public void Close()
    {
        if (_disposed) return;
        CloseRequested?.Invoke();
        Dispose();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _view.Touch -= HandleTouch;
        _view.SizeChanged -= HandleSizeChanged;
        lock (_gate) _frameCallbacks.Clear();
        Closed?.Invoke();
        GC.KeepAlive(ConfigurationChanged);
        GC.KeepAlive(KeyData);
        GC.KeepAlive(EditingStateChanged);
        GC.KeepAlive(ActionPerformed);
    }

    private void HandleSizeChanged(object? sender, EventArgs args)
    {
        _ = sender;
        _ = args;
        if (_view.Width <= 0 || _view.Height <= 0) return;
        _logicalSize = new(_view.Width, _view.Height);
        _metricsGeneration++;
        MetricsChanged?.Invoke(Metrics);
        RequestInvalidate();
    }

    private void HandleTouch(object? sender, SKTouchEventArgs args)
    {
        _ = sender;
        var change = args.ActionType switch
        {
            SKTouchAction.Pressed => PointerChange.down,
            SKTouchAction.Released => PointerChange.up,
            SKTouchAction.Cancelled => PointerChange.cancel,
            SKTouchAction.Entered => PointerChange.add,
            SKTouchAction.Exited => PointerChange.remove,
            _ => PointerChange.move,
        };
        PointerData?.Invoke(new([new(_viewId, TimeSpan.FromTicks(DateTime.UtcNow.Ticks), change,
            PointerDeviceKind.touch, checked((ulong)Math.Max(0, args.Id)), args.Location.X, args.Location.Y,
            0, 0, args.InContact ? 1 : 0, pointerIdentifier: checked((ulong)Math.Max(0, args.Id))) ]));
        args.Handled = true;
    }

    private static Locale ToLocale(CultureInfo culture)
    {
        var pieces = culture.Name.Split('-', StringSplitOptions.RemoveEmptyEntries);
        return pieces.Length > 1 ? new(pieces[0], pieces[^1]) : new(culture.TwoLetterISOLanguageName);
    }
}
