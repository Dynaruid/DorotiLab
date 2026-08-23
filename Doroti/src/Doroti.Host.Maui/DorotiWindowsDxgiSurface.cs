#if WINDOWS
using System.Diagnostics.Tracing;
using System.Numerics;
using System.Runtime.InteropServices;
using Doroti.Ui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using SharpGen.Runtime;
using SkiaSharp;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DXGI;
using static Vortice.Direct3D12.D3D12;
using static Vortice.DXGI.DXGI;

namespace Doroti.Host.Maui;

public sealed class DorotiWindowsDxgiElement : View
{
    internal DorotiWindowsDxgiElement(DorotiWindowsDxgiSurface owner) => Owner = owner;
    internal DorotiWindowsDxgiSurface Owner { get; }
}

public sealed class DorotiWindowsDxgiHost : Microsoft.UI.Xaml.Controls.Grid
{
    internal DorotiWindowsDxgiHost()
    {
        Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        Presenter = new SwapChainPanel
        {
            Width = 0,
            Height = 0,
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Left,
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Top,
            IsHitTestVisible = false,
        };
        Children.Add(Presenter);
    }

    internal SwapChainPanel Presenter { get; }
}

public sealed class DorotiWindowsDxgiElementHandler
    : ViewHandler<DorotiWindowsDxgiElement, DorotiWindowsDxgiHost>
{
    public static readonly IPropertyMapper<DorotiWindowsDxgiElement, DorotiWindowsDxgiElementHandler> Mapper =
        new PropertyMapper<DorotiWindowsDxgiElement, DorotiWindowsDxgiElementHandler>();

    public DorotiWindowsDxgiElementHandler() : base(Mapper) { }
    protected override DorotiWindowsDxgiHost CreatePlatformView() => new();
    protected override void ConnectHandler(DorotiWindowsDxgiHost platformView)
    {
        base.ConnectHandler(platformView);
        VirtualView.Owner.Connect(platformView);
    }
    protected override void DisconnectHandler(DorotiWindowsDxgiHost platformView)
    {
        VirtualView.Owner.Disconnect(platformView);
        base.DisconnectHandler(platformView);
    }
}

/// <summary>
/// Doroti-owned Windows presenter. XAML owns layout/input only; a dedicated
/// raster thread owns D3D12, Skia, swap-chain buffers and Present.
/// </summary>
internal sealed class DorotiWindowsDxgiSurface : IMauiSkiaSurface
{
    private readonly object _gate = new();
    private readonly DorotiWindowsDxgiElement _view;
    private readonly AutoResetEvent _wake = new(false);
    private readonly AutoResetEvent _metricsWake = new(false);
    private readonly Thread _rasterThread;
    private readonly Thread _metricsThread;
    private readonly DorotiResizeTargetCoordinator _targets = new();
    private readonly DorotiResizeTrace _trace = new();
    private DorotiWindowsDxgiHost? _host;
    private SwapChainPanel? _panel;
    private WindowsClientResizeSource? _nativeResizeSource;
    private DorotiResizeEpoch? _latestTarget;
    private long _requestSerial;
    private long _surfaceGeneration;
    private long _presented;
    private long _superseded;
    private long _activations;
    private long _deactivations;
    private MauiPaintCompletion? _activeCompletion;
    private DorotiMouseCursorKind _currentCursor = DorotiMouseCursorKind.basic;
    private bool _loaded;
    private bool _disposed;

    internal DorotiWindowsDxgiSurface()
    {
        _view = new(this);
        _view.SizeChanged += HandleMauiSizeChanged;
        _view.Loaded += HandleMauiLoaded;
        _view.Unloaded += HandleMauiUnloaded;
        _rasterThread = new(RasterMain)
        {
            IsBackground = true,
            Name = "Doroti Windows DXGI raster",
            Priority = ThreadPriority.AboveNormal,
        };
        _rasterThread.SetApartmentState(ApartmentState.MTA);
        _metricsThread = new(MetricsMain)
        {
            IsBackground = true,
            Name = "Doroti Windows resize framework",
            Priority = ThreadPriority.AboveNormal,
        };
        _metricsThread.SetApartmentState(ApartmentState.MTA);
        _metricsThread.Start();
        _rasterThread.Start();
    }

    public View Element => _view;
    public IDispatcher Dispatcher => _view.Dispatcher;
    public double Width => _view.Width;
    public double Height => _view.Height;
    public DorotiResizeEpoch? ResizeTarget => _targets.Latest;
    public event Action<MauiSkiaPaintContext>? Paint;
    public event Action<MauiPaintCompletion, bool>? PresentCompleted;
    public event Action<MauiPaintCompletion?, Exception>? PaintFailed;
    public event Action<MauiSurfacePointerData>? Pointer;
    public event Action<KeyData>? Key;
    public event Action<bool>? FocusChanged;
    public event Action<DorotiResizeEpoch?>? SizeChanged;

    internal void Connect(DorotiWindowsDxgiHost host)
    {
        var panel = host.Presenter;
        lock (_gate)
        {
            if (_disposed) return;
            _host = host;
            _panel = panel;
        }
        host.Loaded += HandlePanelLoaded;
        host.Unloaded += HandlePanelUnloaded;
        host.SizeChanged += HandlePanelSizeChanged;
        panel.CompositionScaleChanged += HandleCompositionScaleChanged;
        host.PointerPressed += HandlePointerPressed;
        host.PointerMoved += HandlePointerMoved;
        host.PointerReleased += HandlePointerReleased;
        host.PointerWheelChanged += HandlePointerWheel;
        panel.GotFocus += HandleGotFocus;
        panel.LostFocus += HandleLostFocus;
        panel.KeyDown += HandleKeyDown;
        panel.KeyUp += HandleKeyUp;
        if (host.IsLoaded)
        {
            AttachNativeResizeSource();
            PublishTarget("DorotiWindowsDxgiHost.Connect");
        }
    }

    internal void Disconnect(DorotiWindowsDxgiHost host)
    {
        var panel = host.Presenter;
        host.Loaded -= HandlePanelLoaded;
        host.Unloaded -= HandlePanelUnloaded;
        host.SizeChanged -= HandlePanelSizeChanged;
        panel.CompositionScaleChanged -= HandleCompositionScaleChanged;
        host.PointerPressed -= HandlePointerPressed;
        host.PointerMoved -= HandlePointerMoved;
        host.PointerReleased -= HandlePointerReleased;
        host.PointerWheelChanged -= HandlePointerWheel;
        panel.GotFocus -= HandleGotFocus;
        panel.LostFocus -= HandleLostFocus;
        panel.KeyDown -= HandleKeyDown;
        panel.KeyUp -= HandleKeyUp;
        DetachNativeResizeSource();
        lock (_gate)
        {
            if (ReferenceEquals(_host, host)) _host = null;
            if (ReferenceEquals(_panel, panel)) _panel = null;
        }
        _wake.Set();
    }

    public void InvalidateSurface()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _requestSerial++;
        }
        _wake.Set();
    }

    public void RequestFocus(bool focused)
    {
        void Apply()
        {
            var panel = _panel;
            if (panel is null) return;
            _ = focused
                ? panel.Focus(FocusState.Programmatic)
                : panel.Focus(FocusState.Unfocused);
        }
        if (_view.Dispatcher.IsDispatchRequired) _view.Dispatcher.Dispatch(Apply);
        else Apply();
    }

    public void SetCursor(DorotiMouseCursorKind cursor)
    {
        lock (_gate) _currentCursor = cursor;
        void Apply()
        {
            var panel = _panel;
            WindowsClientResizeSource? nativeSource;
            lock (_gate) nativeSource = _nativeResizeSource;
            nativeSource?.SetCursor(cursor);
            if (panel is null) return;
            var type = cursor switch
            {
                DorotiMouseCursorKind.click => Windows.UI.Core.CoreCursorType.Hand,
                DorotiMouseCursorKind.text => Windows.UI.Core.CoreCursorType.IBeam,
                DorotiMouseCursorKind.precise => Windows.UI.Core.CoreCursorType.Cross,
                DorotiMouseCursorKind.resizeLeftRight => Windows.UI.Core.CoreCursorType.SizeWestEast,
                DorotiMouseCursorKind.resizeUpDown => Windows.UI.Core.CoreCursorType.SizeNorthSouth,
                _ => Windows.UI.Core.CoreCursorType.Arrow,
            };
            var property = typeof(UIElement).GetProperty(
                "ProtectedCursor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            property?.SetValue(panel, Microsoft.UI.Input.InputCursor.CreateFromCoreCursor(
                new Windows.UI.Core.CoreCursor(type, 0)));
        }
        if (_view.Dispatcher.IsDispatchRequired) _view.Dispatcher.Dispatch(Apply);
        else Apply();
    }

    public MauiSurfaceSnapshot CaptureSnapshot(MauiSurfaceSnapshot current)
    {
        var target = _targets.Latest;
        return current with
        {
            PixelWidth = target?.PhysicalWidth ?? current.PixelWidth,
            PixelHeight = target?.PhysicalHeight ?? current.PixelHeight,
            DevicePixelRatio = target?.DevicePixelRatio ?? current.DevicePixelRatio,
            SurfaceGeneration = Interlocked.Read(ref _surfaceGeneration),
            NativeViewType = "Win32 child HWND hosted by DorotiWindowsDxgiHost",
            GraphicsBackend = "Win32/child-HWND/offscreen-copy/DXGI-D3D12-Skia",
            LogicalWidth = target?.LogicalWidth ?? current.LogicalWidth,
            LogicalHeight = target?.LogicalHeight ?? current.LogicalHeight,
            ResizeContinuityActivations = Interlocked.Read(ref _activations),
            ResizeContinuityDeactivations = Interlocked.Read(ref _deactivations),
            ResizeContinuityActive = _loaded,
            ResizeSynchronousPresents = Interlocked.Read(ref _presented),
            ResizeSynchronousMisses = Interlocked.Read(ref _superseded),
            DwmCompositionEnabled = true,
            EglSwapIntervalPolicy = "not-applicable-dxgi-owned",
            ExactSwapTimingAvailable = true,
            ResizeTrace = _trace.Snapshot(),
        };
    }

    private void HandleMauiLoaded(object? sender, EventArgs args) { _ = sender; _ = args; _loaded = true; AttachNativeResizeSource(); PublishTarget("Maui.Loaded"); }
    private void HandleMauiUnloaded(object? sender, EventArgs args) { _ = sender; _ = args; _loaded = false; _wake.Set(); }
    private void HandleMauiSizeChanged(object? sender, EventArgs args) { _ = sender; _ = args; PublishFallbackTarget("Maui.SizeChanged"); }
    private void HandlePanelLoaded(object sender, RoutedEventArgs args) { _ = sender; _ = args; _loaded = true; AttachNativeResizeSource(); PublishTarget("SwapChainPanel.Loaded"); }
    private void HandlePanelUnloaded(object sender, RoutedEventArgs args) { _ = sender; _ = args; _loaded = false; _wake.Set(); }
    private void HandlePanelSizeChanged(object sender, SizeChangedEventArgs args) { _ = sender; _ = args; PublishFallbackTarget("DorotiWindowsDxgiHost.SizeChanged"); }
    private void HandleCompositionScaleChanged(SwapChainPanel sender, object args) { _ = sender; _ = args; PublishTarget("SwapChainPanel.CompositionScaleChanged"); }

    private void AttachNativeResizeSource()
    {
        if (_disposed || _nativeResizeSource is not null) return;
        var source = WindowsClientResizeSource.TryCreate(
            _view,
            HandleNativeResize,
            HandleNativeResizeTimeout,
            HandleNativePointer);
        if (source is null) return;
        var attached = false;
        var cursor = DorotiMouseCursorKind.basic;
        lock (_gate)
        {
            if (_disposed || _nativeResizeSource is not null)
            {
                source.Dispose();
                return;
            }
            _nativeResizeSource = source;
            cursor = _currentCursor;
            attached = true;
        }
        if (attached)
        {
            source.SetCursor(cursor);
            source.Start();
        }
    }

    private long HandleNativeResize() =>
        PublishTarget("child-HWND.WM_SIZE")?.Generation ?? 0;

    private void HandleNativePointer(MauiSurfacePointerData pointer)
    {
        if (pointer.Change == PointerChange.down && _panel is { } panel)
            _ = panel.Focus(FocusState.Pointer);
        Pointer?.Invoke(pointer);
    }

    private void HandleNativeResizeTimeout(long generation)
    {
        DorotiResizeEpoch? target;
        lock (_gate) target = _latestTarget?.Generation == generation ? _latestTarget : null;
        if (target is null) return;
        Interlocked.Increment(ref _superseded);
        Record("ack", target, "Windows resize transaction timeout",
            terminal: "timedOut",
            detail: "matching exact present did not complete within 100ms; handler returned without presenting stale pixels");
    }

    private void DetachNativeResizeSource()
    {
        WindowsClientResizeSource? source;
        lock (_gate)
        {
            source = _nativeResizeSource;
            _nativeResizeSource = null;
        }
        source?.Dispose();
    }

    private void PublishFallbackTarget(string source)
    {
        lock (_gate)
        {
            if (_nativeResizeSource is not null) return;
        }
        PublishTarget(source);
    }

    private DorotiResizeEpoch? PublishTarget(string source)
    {
        var host = _host;
        var panel = _panel;
        if (_disposed || !_loaded || host is null || panel is null) return null;
        WindowsClientResizeSource? nativeSource;
        lock (_gate) nativeSource = _nativeResizeSource;
        var nativeScale = nativeSource?.GetDeviceScale();
        var scaleX = nativeScale ?? panel.CompositionScaleX;
        var scaleY = nativeScale ?? panel.CompositionScaleY;
        if (!double.IsFinite(scaleX) || scaleX <= 0 ||
            !double.IsFinite(scaleY) || scaleY <= 0) return null;
        var physicalWidth = 0;
        var physicalHeight = 0;
        var hasNativeSize = nativeSource is not null &&
                            nativeSource.TryGetClientSize(out physicalWidth, out physicalHeight);
        var logicalWidth = hasNativeSize ? physicalWidth / scaleX : Math.Max(0, host.ActualWidth);
        var logicalHeight = hasNativeSize ? physicalHeight / scaleY : Math.Max(0, host.ActualHeight);
        DorotiResizeEpoch target;
        lock (_gate)
        {
            target = _targets.Publish(logicalWidth, logicalHeight, scaleX, scaleY);
            if (_latestTarget?.Generation == target.Generation) return target;
            _latestTarget = target;
        }
        Interlocked.Increment(ref _activations);
        Record("target", target, source);
        // Win32 size delivery is the platform transaction. Do not run the
        // framework's layout/build callback inside WM_SIZING/WM_SIZE: Flutter
        // has a distinct framework UI runner for the same reason. A dedicated
        // latest-only worker publishes metrics and builds the matching scene.
        _metricsWake.Set();
        // AttachSurface can submit and signal the first scene before WinUI has
        // loaded the panel; that wake is intentionally ignored while unloaded.
        // Re-arm startup once a drawable target exists. Continuous resize does
        // not take this path after the first successful present.
        if (Interlocked.Read(ref _presented) == 0) _wake.Set();
        return target;
    }

    private void MetricsMain()
    {
        long dispatchedGeneration = 0;
        while (true)
        {
            _metricsWake.WaitOne();
            while (true)
            {
                DorotiResizeEpoch? target;
                lock (_gate)
                {
                    if (_disposed) return;
                    target = _latestTarget;
                }
                if (target is null || target.Generation == dispatchedGeneration) break;
                SizeChanged?.Invoke(target);
                dispatchedGeneration = target.Generation;
                lock (_gate)
                {
                    if (_disposed) return;
                    if (_latestTarget?.Generation == dispatchedGeneration) break;
                }
            }
        }
    }

    private void RasterMain()
    {
        using var presenter = new WindowsHwndD3D12Presenter();
        long processedSerial = -1;
        while (true)
        {
            _wake.WaitOne();
            DorotiResizeEpoch? target;
            WindowsClientResizeSource? nativeSource;
            long serial;
            lock (_gate)
            {
                if (_disposed) break;
                target = _latestTarget;
                nativeSource = _nativeResizeSource;
                serial = _requestSerial;
            }
            if (!_loaded || nativeSource is null || target is null || !target.HasDrawableSize)
                continue;
            if (serial == processedSerial) continue;

            EventSource.SetCurrentThreadActivityId(Guid.NewGuid(), out var previousActivityId);
            _activeCompletion = null;
            var nativeResizeCompleted = false;
            var nativeResizeFailed = false;
            try
            {
                var surfacePrepareStarted = DorotiFrameClock.Now;
                presenter.EnsureTarget(
                    nativeSource.RenderWindowHandle,
                    target.PhysicalWidth,
                    target.PhysicalHeight);
                if (presenter.SurfaceChanged) Interlocked.Increment(ref _surfaceGeneration);
                Record("surface-ready", target, "D3D12 exact offscreen backing",
                    DorotiFrameClock.Now - surfacePrepareStarted,
                    surfaceWidth: presenter.Width, surfaceHeight: presenter.Height,
                    detail: $"backingStoreResized={presenter.SurfaceChanged}; hwnd={nativeSource.RenderWindowHandle}; surface={presenter.Width}x{presenter.Height}; adapter={presenter.AdapterDescription}");
                var paint = new MauiSkiaPaintContext(
                    presenter.Surface,
                    presenter.Context,
                    presenter.Width,
                    presenter.Height,
                    target.DevicePixelRatio,
                    Interlocked.Read(ref _surfaceGeneration),
                    "Win32 child HWND",
                    "Win32/child-HWND/offscreen-copy/DXGI-D3D12-Skia");
                lock (_gate) paint.SkipRaster = _latestTarget?.Generation != target.Generation;
                if (paint.SkipRaster)
                {
                    // Still cross BeginPaint/EndPaint so MauiHostAdapter releases
                    // _invalidatePending. Skipping the callback entirely would
                    // stall all later CompositionTarget frame requests.
                    Paint?.Invoke(paint);
                    Interlocked.Increment(ref _superseded);
                    Record("ack", target, "pre-raster latest target gate", terminal: "superseded");
                    processedSerial = serial;
                    continue;
                }
                var rasterStarted = DorotiFrameClock.Now;
                Record("raster-start", target, "D3D12 raster thread",
                    surfaceWidth: presenter.Width, surfaceHeight: presenter.Height);
                Paint?.Invoke(paint);
                _activeCompletion = paint.Completion;
                var surfaceMatch = paint.Completion?.Descriptor.MatchTargetAndSurface(
                    target,
                    presenter.Width,
                    presenter.Height,
                    target.DeviceScaleX,
                    target.DeviceScaleY);
                if (paint.Completion is not { } completion || surfaceMatch is not { IsExact: true })
                {
                    Interlocked.Increment(ref _superseded);
                    Record("ack", target, "exact-frame gate", terminal: "superseded",
                        detail: paint.Completion is null
                            ? "no exact scene ready"
                            : $"{surfaceMatch?.MismatchCode}: {surfaceMatch?.Detail}");
                    if (paint.Completion is { } rejected) PresentCompleted?.Invoke(rejected, true);
                    processedSerial = serial;
                    continue;
                }
                Record("paint-end", target, "D3D12 raster thread", DorotiFrameClock.Now - rasterStarted,
                    surfaceWidth: presenter.Width, surfaceHeight: presenter.Height);
                long preFlushGeneration;
                lock (_gate)
                {
                    preFlushGeneration = _latestTarget?.Generation ?? 0;
                }
                if (preFlushGeneration != target.Generation)
                {
                    Interlocked.Increment(ref _superseded);
                    Record("ack", target, "pre-flush latest target gate",
                        terminal: "superseded",
                        detail: $"latestTargetGeneration={preFlushGeneration}; presentedGeneration={target.Generation}; schedulerSerial={Volatile.Read(ref _requestSerial)}; frameSerial={serial}");
                    PresentCompleted?.Invoke(completion, true);
                    processedSerial = serial;
                    continue;
                }
                presenter.Flush();
                Record("raster-end", target, "D3D12 raster thread", DorotiFrameClock.Now - rasterStarted,
                    surfaceWidth: presenter.Width, surfaceHeight: presenter.Height);
                long postFlushGeneration;
                lock (_gate)
                {
                    postFlushGeneration = _latestTarget?.Generation ?? 0;
                }
                if (postFlushGeneration != target.Generation)
                {
                    Interlocked.Increment(ref _superseded);
                    Record("ack", target, "latest target gate",
                        terminal: "superseded",
                        detail: $"latestTargetGeneration={postFlushGeneration}; presentedGeneration={target.Generation}; schedulerSerial={Volatile.Read(ref _requestSerial)}; frameSerial={serial}");
                    PresentCompleted?.Invoke(completion, true);
                    processedSerial = serial;
                    continue;
                }
                long prePresentGeneration;
                long prePresentSerial;
                lock (_gate)
                {
                    prePresentGeneration = _latestTarget?.Generation ?? 0;
                    prePresentSerial = _requestSerial;
                }
                if (prePresentGeneration != target.Generation)
                {
                    Interlocked.Increment(ref _superseded);
                    Record("ack", target, "pre-present latest target gate",
                        terminal: "superseded",
                        detail: $"prePresentTargetGeneration={prePresentGeneration}; presentedGeneration={target.Generation}; schedulerSerial={prePresentSerial}; frameSerial={serial}");
                    PresentCompleted?.Invoke(completion, true);
                    processedSerial = serial;
                    continue;
                }
                var presentStarted = DorotiFrameClock.Now;
                var committed = false;
                var nativeCommitGeneration = 0L;
                lock (_gate)
                {
                    nativeCommitGeneration = _latestTarget?.Generation ?? 0;
                    if (nativeCommitGeneration == target.Generation &&
                        ReferenceEquals(_nativeResizeSource, nativeSource) &&
                        !nativeSource.IsRetired(target.Generation))
                    {
                        Record("pre-swap", target, "Doroti-owned child HWND DXGI",
                            surfaceWidth: presenter.Width, surfaceHeight: presenter.Height,
                            detail: $"prePresentTargetGeneration={prePresentGeneration}; presentedGeneration={target.Generation}; nativeCommitTargetGeneration={target.Generation}; schedulerSerial={prePresentSerial}; frameSerial={serial}; newerTargetKnownAtPrePresent=0");
                        presenter.Present();
                        committed = true;
                    }
                }
                if (!committed)
                {
                    Interlocked.Increment(ref _superseded);
                    Record("ack", target, "native final target gate",
                        terminal: "superseded",
                        detail: $"prePresentTargetGeneration={prePresentGeneration}; nativeCommitTargetGeneration={nativeCommitGeneration}; presentedGeneration={target.Generation}; schedulerSerial={prePresentSerial}; frameSerial={serial}");
                    PresentCompleted?.Invoke(completion, true);
                    processedSerial = serial;
                    continue;
                }
                long postPresentGeneration;
                lock (_gate) postPresentGeneration = _latestTarget?.Generation ?? 0;
                Record("post-swap", target, "Doroti-owned child HWND DXGI", DorotiFrameClock.Now - presentStarted,
                    surfaceWidth: presenter.Width, surfaceHeight: presenter.Height,
                    detail: $"prePresentTargetGeneration={prePresentGeneration}; presentedGeneration={target.Generation}; postPresentObservedGeneration={postPresentGeneration}; swapChainResized={presenter.LastCommitResized}");
                if (postPresentGeneration != target.Generation)
                {
                    Record("target-advanced-during-present", target, "targetAdvancedDuringPresent",
                        detail: $"presentedGeneration={target.Generation}; postPresentObservedGeneration={postPresentGeneration}");
                }
                var releaseStarted = DorotiFrameClock.Now;
                presenter.ReleasePresentedBuffer();
                Record("buffer-release", target, "Doroti-owned D3D12/DXGI",
                    DorotiFrameClock.Now - releaseStarted);
                // W0's visible transaction is Present -> DwmFlush -> completion.
                // In particular, do not reveal a pre-grown child through a larger
                // parent client rect until DWM has consumed the matching buffer.
                if (presenter.LastCommitResized)
                {
                    var dwmFlushSucceeded = presenter.FlushDwmAfterResize();
                    Record(dwmFlushSucceeded ? "dwm-flush-end" : "dwm-flush-failed",
                        target,
                        "post-completion resize-only DwmFlush",
                        presenter.LastDwmFlushDuration,
                        surfaceWidth: presenter.Width,
                        surfaceHeight: presenter.Height);
                }
                Interlocked.Increment(ref _presented);
                Record("ack", target, "D3D12 raster thread", terminal: "presented");
                PresentCompleted?.Invoke(completion, false);
                nativeSource.CompletePresented(target.Generation);
                nativeResizeCompleted = true;
                Record("native-resize-complete", target, "post-DwmFlush platform unblock",
                    DorotiFrameClock.Now - presentStarted,
                    surfaceWidth: presenter.Width,
                    surfaceHeight: presenter.Height);
                processedSerial = serial;
            }
            catch (Exception exception)
            {
                nativeResizeFailed = true;
                PaintFailed?.Invoke(_activeCompletion, exception);
                presenter.Reset();
                processedSerial = serial;
            }
            finally
            {
                long latestGeneration;
                lock (_gate) latestGeneration = _latestTarget?.Generation ?? 0;
                // An exact-scene miss is a retry inside the same bounded WM_SIZE
                // transaction. Releasing it here lets the next WM_SIZE overtake
                // that retry during Present -> DwmFlush. Only a failed, retired,
                // or genuinely superseded transaction may unblock without a
                // matching present.
                if (!nativeResizeCompleted &&
                    (nativeResizeFailed || latestGeneration != target.Generation ||
                     nativeSource.IsRetired(target.Generation)))
                    nativeSource.Complete(target.Generation);
                _activeCompletion = null;
                Interlocked.Increment(ref _deactivations);
                EventSource.SetCurrentThreadActivityId(previousActivityId);
            }
            lock (_gate)
            {
                if (!_disposed && _requestSerial != processedSerial) _wake.Set();
            }
        }
    }

    private void HandlePointerPressed(object sender, PointerRoutedEventArgs args) =>
        HandlePointer(sender, args, PointerChange.down);

    private void HandlePointerMoved(object sender, PointerRoutedEventArgs args) =>
        HandlePointer(sender, args, PointerChange.move);

    private void HandlePointerReleased(object sender, PointerRoutedEventArgs args) =>
        HandlePointer(sender, args, PointerChange.up);

    private void HandlePointerWheel(object sender, PointerRoutedEventArgs args) =>
        HandlePointer(sender, args, PointerChange.move);

    private void HandlePointer(object sender, PointerRoutedEventArgs args, PointerChange change)
    {
        var host = (DorotiWindowsDxgiHost)sender;
        var panel = _panel;
        if (panel is null) return;
        if (change == PointerChange.down) _ = panel.Focus(FocusState.Pointer);
        var point = args.GetCurrentPoint(host);
        var scale = Math.Max(1, panel.CompositionScaleX);
        var buttons = (point.Properties.IsLeftButtonPressed ? 1 : 0) |
                      (point.Properties.IsRightButtonPressed ? 2 : 0) |
                      (point.Properties.IsMiddleButtonPressed ? 4 : 0);
        Pointer?.Invoke(new(DorotiFrameClock.Now, change, PointerDeviceKind.mouse,
            point.PointerId, point.Position.X * scale, point.Position.Y * scale, buttons,
            point.Properties.IsHorizontalMouseWheel ? point.Properties.MouseWheelDelta : 0,
            point.Properties.IsHorizontalMouseWheel ? 0 : -point.Properties.MouseWheelDelta,
            point.Properties.MouseWheelDelta == 0 ? PointerSignalKind.none : PointerSignalKind.scroll,
            point.Properties.Pressure));
        args.Handled = true;
    }

    private void HandleGotFocus(object sender, RoutedEventArgs args) { _ = sender; _ = args; FocusChanged?.Invoke(true); }
    private void HandleLostFocus(object sender, RoutedEventArgs args) { _ = sender; _ = args; FocusChanged?.Invoke(false); }
    private void HandleKeyDown(object sender, KeyRoutedEventArgs args) => RaiseKey(args, KeyEventType.down);
    private void HandleKeyUp(object sender, KeyRoutedEventArgs args) => RaiseKey(args, KeyEventType.up);
    private void RaiseKey(KeyRoutedEventArgs args, KeyEventType type)
    {
        var value = (long)args.Key;
        Key?.Invoke(new(1, DorotiFrameClock.Now, type, 0x100000000L | value, 0x100000000L | value, false));
        args.Handled = true;
    }

    private void InvokeOnUiThread(Action action)
    {
        if (!_view.Dispatcher.IsDispatchRequired)
        {
            action();
            return;
        }
        using var completed = new ManualResetEventSlim();
        Exception? failure = null;
        if (!_view.Dispatcher.Dispatch(() =>
        {
            try { action(); }
            catch (Exception exception) { failure = exception; }
            finally { completed.Set(); }
        }))
        {
            throw new InvalidOperationException("Windows UI dispatcher rejected swap-chain attachment.");
        }
        if (!completed.Wait(TimeSpan.FromSeconds(5)))
            throw new TimeoutException("Windows UI dispatcher did not attach the swap chain within five seconds.");
        if (failure is not null)
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
    }

    private void Record(string phase, DorotiResizeEpoch target, string source,
        TimeSpan? duration = null, int surfaceWidth = 0, int surfaceHeight = 0,
        string? terminal = null, string? detail = null)
    {
        _trace.Record(phase, target, source, duration,
            surfaceWidth: surfaceWidth, surfaceHeight: surfaceHeight,
            terminal: terminal, detail: detail);
        WindowsResizeEtw.Log.Marker(phase, target, surfaceWidth, surfaceHeight, source);
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
        }
        _view.SizeChanged -= HandleMauiSizeChanged;
        _view.Loaded -= HandleMauiLoaded;
        _view.Unloaded -= HandleMauiUnloaded;
        if (_host is { } host) Disconnect(host);
        _wake.Set();
        _metricsWake.Set();
        if (Thread.CurrentThread != _metricsThread && !_metricsThread.Join(TimeSpan.FromSeconds(5)))
            PaintFailed?.Invoke(null, new TimeoutException("Windows resize framework thread did not stop within five seconds."));
        if (Thread.CurrentThread != _rasterThread && !_rasterThread.Join(TimeSpan.FromSeconds(5)))
            PaintFailed?.Invoke(null, new TimeoutException("Windows D3D12 raster thread did not stop within five seconds."));
        _metricsWake.Dispose();
        _wake.Dispose();
    }
}

internal sealed class WindowsClientResizeSource : IDisposable
{
    private const uint WmSize = 0x0005;
    private const uint WmCancelMode = 0x001F;
    private const uint WmSetCursor = 0x0020;
    private const uint WmEraseBackground = 0x0014;
    private const uint WmNcDestroy = 0x0082;
    private const uint WmNcHitTest = 0x0084;
    private const uint WmCaptureChanged = 0x0215;
    private const uint WmMouseMove = 0x0200;
    private const uint WmLeftButtonDown = 0x0201;
    private const uint WmLeftButtonUp = 0x0202;
    private const uint WmRightButtonDown = 0x0204;
    private const uint WmRightButtonUp = 0x0205;
    private const uint WmMiddleButtonDown = 0x0207;
    private const uint WmMiddleButtonUp = 0x0208;
    private const uint WmMouseWheel = 0x020A;
    private const uint WmMouseHorizontalWheel = 0x020E;
    private const uint WmMouseLeave = 0x02A3;
    private const int HtClient = 1;
    private const uint TrackMouseEventLeave = 0x00000002;
    private const int MouseWheelDelta = 120;
    private const int IdcArrow = 32512;
    private const int IdcIBeam = 32513;
    private const int IdcWait = 32514;
    private const int IdcCross = 32515;
    private const int IdcSizeNwSe = 32642;
    private const int IdcSizeNeSw = 32643;
    private const int IdcSizeWe = 32644;
    private const int IdcSizeNs = 32645;
    private const int IdcSizeAll = 32646;
    private const int IdcNo = 32648;
    private const int IdcHand = 32649;
    private const int IdcAppStarting = 32650;
    private const int IdcHelp = 32651;
    private const uint WsChild = 0x40000000;
    private const uint WsVisible = 0x10000000;
    private const uint WsClipSiblings = 0x04000000;
    private const uint WsExNoActivate = 0x08000000;
    private const uint SwpNoActivate = 0x0010;
    private const uint SwpShowWindow = 0x0040;
    private static long _nextSubclassId;
    private readonly Microsoft.UI.Xaml.Window _platformWindow;
    private readonly nint _parentWindowHandle;
    private readonly nint _renderWindowHandle;
    private readonly nuint _parentSubclassId;
    private readonly nuint _childSubclassId;
    private readonly SubclassProcedure _parentProcedure;
    private readonly SubclassProcedure _childProcedure;
    private readonly Func<long> _sizeChanged;
    private readonly Action<long> _timedOut;
    private readonly Action<MauiSurfacePointerData> _pointer;
    private readonly WindowsResizeCoordinator _coordinator = new();
    private readonly WindowsPlatformTaskRunner _taskRunner;
    private bool _parentAttached;
    private bool _childAttached;
    private bool _started;
    private bool _disposed;
    private bool _trackingMouseLeave;
    private bool _pointerAdded;
    private bool _releasingCapture;
    private int _hasPresented;
    private nint _clientCursor;
    private int _lastPointerX;
    private int _lastPointerY;
    private int _buttons;

    private WindowsClientResizeSource(
        Microsoft.UI.Xaml.Window platformWindow,
        nint parentWindowHandle,
        Func<long> sizeChanged,
        Action<long> timedOut,
        Action<MauiSurfacePointerData> pointer)
    {
        _platformWindow = platformWindow;
        _parentWindowHandle = parentWindowHandle;
        _sizeChanged = sizeChanged;
        _timedOut = timedOut;
        _pointer = pointer;
        _taskRunner = new(_coordinator);
        _clientCursor = LoadCursor(0, (nint)IdcArrow);
        _parentSubclassId = checked((nuint)Interlocked.Increment(ref _nextSubclassId));
        _childSubclassId = checked((nuint)Interlocked.Increment(ref _nextSubclassId));
        _parentProcedure = HandleParentWindowMessage;
        _childProcedure = HandleChildWindowMessage;
        _renderWindowHandle = CreateWindowEx(
            WsExNoActivate,
            "STATIC",
            string.Empty,
            WsChild | WsVisible | WsClipSiblings,
            0,
            0,
            1,
            1,
            _parentWindowHandle,
            0,
            GetModuleHandle(null),
            0);
        if (_renderWindowHandle == 0) return;
        _childAttached = SetWindowSubclass(
            _renderWindowHandle, _childProcedure, _childSubclassId, 0);
        _parentAttached = _childAttached && SetWindowSubclass(
            _parentWindowHandle, _parentProcedure, _parentSubclassId, 0);
    }

    internal nint RenderWindowHandle => _renderWindowHandle;

    internal void SetCursor(DorotiMouseCursorKind cursor) =>
        _clientCursor = ResolveCursor(cursor);

    internal static WindowsClientResizeSource? TryCreate(
        DorotiWindowsDxgiElement view,
        Func<long> sizeChanged,
        Action<long> timedOut,
        Action<MauiSurfacePointerData> pointer)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(sizeChanged);
        ArgumentNullException.ThrowIfNull(timedOut);
        ArgumentNullException.ThrowIfNull(pointer);
        if (view.Window?.Handler?.PlatformView is not Microsoft.UI.Xaml.Window platformWindow)
            return null;
        // MAUI/WinUI renders its default title bar inside the top-level HWND's
        // client coordinates. A raw child HWND at (0, 0) therefore sits above
        // the title text and caption buttons even though WS_CAPTION remains on
        // the parent. Keep the system title bar enabled and reserve its current
        // client-coordinate height when positioning the render child.
        platformWindow.ExtendsContentIntoTitleBar = false;
        var titleBarForeground = platformWindow.Content is FrameworkElement
            { ActualTheme: ElementTheme.Light }
            ? Microsoft.UI.Colors.Black
            : Microsoft.UI.Colors.White;
        platformWindow.AppWindow.TitleBar.ForegroundColor = titleBarForeground;
        platformWindow.AppWindow.TitleBar.ButtonForegroundColor = titleBarForeground;
        platformWindow.AppWindow.TitleBar.ButtonHoverForegroundColor = titleBarForeground;
        platformWindow.AppWindow.TitleBar.ButtonPressedForegroundColor = titleBarForeground;
        var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(platformWindow);
        if (windowHandle == 0) return null;
        var source = new WindowsClientResizeSource(
            platformWindow, windowHandle, sizeChanged, timedOut, pointer);
        if (source._parentAttached && source._childAttached) return source;
        source.Dispose();
        return null;
    }

    internal void Start()
    {
        if (_started) return;
        _started = true;
        ResizeChildToParent();
    }

    internal bool TryGetClientSize(out int width, out int height)
    {
        width = 0;
        height = 0;
        if (!_childAttached || !GetClientRect(_renderWindowHandle, out var rect)) return false;
        width = Math.Max(0, rect.Right - rect.Left);
        height = Math.Max(0, rect.Bottom - rect.Top);
        return true;
    }

    internal double GetDeviceScale()
    {
        var dpi = GetDpiForWindow(_renderWindowHandle);
        return dpi == 0 ? 1 : dpi / 96.0;
    }

    internal void Complete(long generation)
    {
        if (Volatile.Read(ref _hasPresented) == 0)
            _coordinator.DiscardCompletion(generation);
        else
            _coordinator.Complete(generation);
    }

    internal void CompletePresented(long generation)
    {
        if (Interlocked.Exchange(ref _hasPresented, 1) == 0)
            _coordinator.DiscardCompletion(generation);
        else
            _coordinator.Complete(generation);
    }

    internal bool IsRetired(long generation) => _coordinator.IsRetired(generation);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _started = false;
        if (_parentAttached)
        {
            RemoveWindowSubclass(_parentWindowHandle, _parentProcedure, _parentSubclassId);
            _parentAttached = false;
        }
        if (_childAttached)
        {
            RemoveWindowSubclass(_renderWindowHandle, _childProcedure, _childSubclassId);
            _childAttached = false;
        }
        if (_renderWindowHandle != 0) DestroyWindow(_renderWindowHandle);
        _taskRunner.Dispose();
        _coordinator.Dispose();
    }

    private nint HandleParentWindowMessage(
        nint windowHandle,
        uint message,
        nuint wParam,
        nint lParam,
        nuint subclassId,
        nuint referenceData)
    {
        if (message == WmSetCursor && GetHitTest(lParam) == HtClient)
        {
            // Keep client cursor ownership separate from the system-owned
            // non-client resize cursor. This also restores the Doroti cursor
            // immediately after the pointer crosses a sizing border.
            SetNativeCursor(_clientCursor);
            return 1;
        }
        if (message == WmSize && _started)
            ResizeChildToParent();
        if (message == WmNcDestroy) _parentAttached = false;
        // The parent remains a WinUI top-level window. Its real WM_SIZE must
        // reach WinUI during the sizing loop so the system title bar, content
        // island, pointer capture, and WM_EXITSIZEMOVE lifecycle stay intact.
        return DefSubclassProc(windowHandle, message, wParam, lParam);
    }

    private static int GetHitTest(nint lParam) => unchecked((short)(long)lParam);

    private static nint ResolveCursor(DorotiMouseCursorKind cursor)
    {
        var resource = cursor switch
        {
            DorotiMouseCursorKind.click or DorotiMouseCursorKind.grab or
                DorotiMouseCursorKind.grabbing => IdcHand,
            DorotiMouseCursorKind.forbidden or DorotiMouseCursorKind.noDrop => IdcNo,
            DorotiMouseCursorKind.wait => IdcWait,
            DorotiMouseCursorKind.progress => IdcAppStarting,
            DorotiMouseCursorKind.help => IdcHelp,
            DorotiMouseCursorKind.text or DorotiMouseCursorKind.verticalText => IdcIBeam,
            DorotiMouseCursorKind.cell or DorotiMouseCursorKind.precise => IdcCross,
            DorotiMouseCursorKind.move or DorotiMouseCursorKind.allScroll => IdcSizeAll,
            DorotiMouseCursorKind.resizeLeftRight or DorotiMouseCursorKind.resizeLeft or
                DorotiMouseCursorKind.resizeRight or DorotiMouseCursorKind.resizeColumn => IdcSizeWe,
            DorotiMouseCursorKind.resizeUpDown or DorotiMouseCursorKind.resizeUp or
                DorotiMouseCursorKind.resizeDown or DorotiMouseCursorKind.resizeRow => IdcSizeNs,
            DorotiMouseCursorKind.resizeUpLeftDownRight or DorotiMouseCursorKind.resizeUpLeft or
                DorotiMouseCursorKind.resizeDownRight => IdcSizeNwSe,
            DorotiMouseCursorKind.resizeUpRightDownLeft or DorotiMouseCursorKind.resizeUpRight or
                DorotiMouseCursorKind.resizeDownLeft => IdcSizeNeSw,
            DorotiMouseCursorKind.none => 0,
            _ => IdcArrow,
        };
        return resource == 0 ? 0 : LoadCursor(0, (nint)resource);
    }

    private nint HandleChildWindowMessage(
        nint windowHandle,
        uint message,
        nuint wParam,
        nint lParam,
        nuint subclassId,
        nuint referenceData)
    {
        if (message is WmMouseMove or WmLeftButtonDown or WmLeftButtonUp or
            WmRightButtonDown or WmRightButtonUp or WmMiddleButtonDown or
            WmMiddleButtonUp or WmMouseWheel or WmMouseHorizontalWheel)
        {
            HandleMouseMessage(windowHandle, message, wParam, lParam);
            return 0;
        }
        if (message == WmMouseLeave)
        {
            _trackingMouseLeave = false;
            // Mouse capture must keep the active pointer alive while a drag is
            // outside the child. The release/cancel path closes the sequence.
            if (_pointerAdded && _buttons == 0)
            {
                DispatchPointer(PointerChange.remove, _lastPointerX, _lastPointerY,
                    0, 0, 0, PointerSignalKind.none);
                _pointerAdded = false;
            }
            return 0;
        }
        if (message == WmCancelMode ||
            (message == WmCaptureChanged && !_releasingCapture && _buttons != 0))
        {
            if (_pointerAdded && _buttons != 0)
                DispatchPointer(PointerChange.cancel, _lastPointerX, _lastPointerY,
                    0, 0, 0, PointerSignalKind.none);
            _buttons = 0;
            _pointerAdded = false;
        }
        if (message == WmSize && _started)
        {
            var generation = _sizeChanged();
            // Cold startup can spend longer than the 100ms interactive budget
            // compiling/rasterizing its first framework scene. No older visible
            // frame exists at that point, so blocking and retiring the only
            // target leaves a normal CLI launch permanently blank. Let startup
            // finish asynchronously; after the first successful present every
            // later WM_SIZE uses the bounded matching transaction wait.
            if (generation > 0 && Volatile.Read(ref _hasPresented) != 0 &&
                TryGetClientSize(out var width, out var height) &&
                width > 0 && height > 0 &&
                !_taskRunner.PollOnce(generation, TimeSpan.FromMilliseconds(100)))
                _timedOut(generation);
            return 0;
        }
        if (message == WmEraseBackground) return 1;
        if (message == WmNcHitTest) return HtClient;
        if (message == WmNcDestroy) _childAttached = false;
        return DefSubclassProc(windowHandle, message, wParam, lParam);
    }

    private void HandleMouseMessage(nint windowHandle, uint message, nuint wParam, nint lParam)
    {
        var point = message is WmMouseWheel or WmMouseHorizontalWheel
            ? ScreenPointToClient(lParam)
            : ClientPoint(lParam);
        _lastPointerX = point.X;
        _lastPointerY = point.Y;
        var previousButtons = _buttons;
        _buttons = ButtonsFromWParam(wParam);
        if (message == WmLeftButtonDown) _buttons |= 1;
        if (message == WmRightButtonDown) _buttons |= 2;
        if (message == WmMiddleButtonDown) _buttons |= 4;

        if (!_pointerAdded)
        {
            DispatchPointer(PointerChange.add, point.X, point.Y,
                0, 0, 0, PointerSignalKind.none);
            _pointerAdded = true;
        }

        if (!_trackingMouseLeave && message == WmMouseMove && _buttons == 0)
        {
            var tracking = new NativeTrackMouseEvent
            {
                Size = checked((uint)Marshal.SizeOf<NativeTrackMouseEvent>()),
                Flags = TrackMouseEventLeave,
                WindowHandle = windowHandle,
            };
            _trackingMouseLeave = TrackMouseEvent(ref tracking);
        }

        var change = message switch
        {
            WmLeftButtonDown or WmRightButtonDown or WmMiddleButtonDown
                when previousButtons == 0 => PointerChange.down,
            WmLeftButtonUp or WmRightButtonUp or WmMiddleButtonUp
                when _buttons == 0 => PointerChange.up,
            WmMouseMove when _buttons == 0 => PointerChange.hover,
            _ => PointerChange.move,
        };
        var verticalScroll = message == WmMouseWheel
            ? -GetWheelDelta(wParam) / (double)MouseWheelDelta * MouseWheelDelta
            : 0;
        var horizontalScroll = message == WmMouseHorizontalWheel
            ? GetWheelDelta(wParam) / (double)MouseWheelDelta * MouseWheelDelta
            : 0;
        DispatchPointer(change, point.X, point.Y, _buttons,
            horizontalScroll, verticalScroll,
            message is WmMouseWheel or WmMouseHorizontalWheel
                ? PointerSignalKind.scroll
                : PointerSignalKind.none);

        if (change == PointerChange.down)
        {
            SetCapture(windowHandle);
        }
        else if (change == PointerChange.up && _buttons == 0 && GetCapture() == windowHandle)
        {
            _releasingCapture = true;
            try { ReleaseCapture(); }
            finally { _releasingCapture = false; }
            if (_pointerAdded && !IsInsideClient(windowHandle, point))
            {
                DispatchPointer(PointerChange.remove, point.X, point.Y,
                    0, 0, 0, PointerSignalKind.none);
                _pointerAdded = false;
            }
        }
    }

    private static bool IsInsideClient(nint windowHandle, NativePoint point) =>
        GetClientRect(windowHandle, out var rect) &&
        point.X >= rect.Left && point.X < rect.Right &&
        point.Y >= rect.Top && point.Y < rect.Bottom;

    private void DispatchPointer(
        PointerChange change,
        int x,
        int y,
        int buttons,
        double scrollDeltaX,
        double scrollDeltaY,
        PointerSignalKind signalKind) =>
        _pointer(new(
            DorotiFrameClock.Now,
            change,
            PointerDeviceKind.mouse,
            1,
            x,
            y,
            buttons,
            scrollDeltaX,
            scrollDeltaY,
            signalKind,
            buttons == 0 ? 0 : 1));

    private static NativePoint ClientPoint(nint lParam) => new()
    {
        X = unchecked((short)(long)lParam),
        Y = unchecked((short)((long)lParam >> 16)),
    };

    private NativePoint ScreenPointToClient(nint lParam)
    {
        var point = ClientPoint(lParam);
        ScreenToClient(_renderWindowHandle, ref point);
        return point;
    }

    private static int ButtonsFromWParam(nuint wParam)
    {
        var keys = unchecked((ushort)wParam);
        return ((keys & 0x0001) != 0 ? 1 : 0) |
               ((keys & 0x0002) != 0 ? 2 : 0) |
               ((keys & 0x0010) != 0 ? 4 : 0);
    }

    private static int GetWheelDelta(nuint wParam) =>
        unchecked((short)((ulong)wParam >> 16));

    private void ResizeChildToParent()
    {
        if (!_childAttached || !GetClientRect(_parentWindowHandle, out var rect) ||
            !TryGetClientSize(out var childWidth, out var childHeight)) return;
        var width = Math.Max(0, rect.Right - rect.Left);
        var top = GetTitleBarInset();
        var height = Math.Max(0, rect.Bottom - rect.Top - top);
        if (width == childWidth && height == childHeight &&
            GetChildTop() == top) return;
        SetWindowPos(
            _renderWindowHandle,
            0,
            0,
            top,
            width,
            height,
            SwpNoActivate | SwpShowWindow);
    }

    private int GetTitleBarInset()
    {
        try
        {
            return Math.Max(0, _platformWindow.AppWindow.TitleBar.Height);
        }
        catch (InvalidOperationException)
        {
            return 0;
        }
    }

    private int GetChildTop()
    {
        if (!GetWindowRect(_renderWindowHandle, out var child)) return -1;
        var origin = new NativePoint { X = child.Left, Y = child.Top };
        return ScreenToClient(_parentWindowHandle, ref origin)
            ? origin.Y
            : -1;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        internal int X;
        internal int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeTrackMouseEvent
    {
        internal uint Size;
        internal uint Flags;
        internal nint WindowHandle;
        internal uint HoverTime;
    }

    private delegate nint SubclassProcedure(
        nint windowHandle,
        uint message,
        nuint wParam,
        nint lParam,
        nuint subclassId,
        nuint referenceData);

    [DllImport("comctl32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowSubclass(
        nint windowHandle,
        SubclassProcedure procedure,
        nuint subclassId,
        nuint referenceData);

    [DllImport("comctl32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RemoveWindowSubclass(
        nint windowHandle,
        SubclassProcedure procedure,
        nuint subclassId);

    [DllImport("comctl32.dll", ExactSpelling = true)]
    private static extern nint DefSubclassProc(
        nint windowHandle,
        uint message,
        nuint wParam,
        nint lParam);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetClientRect(nint windowHandle, out NativeRect rect);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(nint windowHandle, out NativeRect rect);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ScreenToClient(nint windowHandle, ref NativePoint point);

    [DllImport("user32.dll", EntryPoint = "CreateWindowExW", ExactSpelling = true,
        CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint CreateWindowEx(
        uint extendedStyle,
        string className,
        string windowName,
        uint style,
        int x,
        int y,
        int width,
        int height,
        nint parentWindow,
        nint menu,
        nint instance,
        nint parameter);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(nint windowHandle);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(
        nint windowHandle,
        nint insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern uint GetDpiForWindow(nint windowHandle);

    [DllImport("user32.dll", EntryPoint = "LoadCursorW", ExactSpelling = true)]
    private static extern nint LoadCursor(nint instance, nint cursorName);

    [DllImport("user32.dll", EntryPoint = "SetCursor", ExactSpelling = true)]
    private static extern nint SetNativeCursor(nint cursor);

    [DllImport("user32.dll", EntryPoint = "TrackMouseEvent", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TrackMouseEvent(ref NativeTrackMouseEvent tracking);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint SetCapture(nint windowHandle);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint GetCapture();

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ReleaseCapture();

    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", ExactSpelling = true,
        CharSet = CharSet.Unicode)]
    private static extern nint GetModuleHandle(string? moduleName);
}

/// <summary>
/// Serial completion primitive for native resize targets. A completion may
/// arrive before the native handler starts waiting, so entries are created by
/// either side and consumed exactly once by the waiter.
/// </summary>
internal sealed class WindowsResizeCoordinator : IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<long, ManualResetEventSlim> _transactions = [];
    private long _retiredGeneration;
    private bool _disposed;

    internal bool Wait(long generation, TimeSpan timeout)
    {
        ManualResetEventSlim completion;
        lock (_gate)
        {
            if (_disposed || generation <= _retiredGeneration) return false;
            if (!_transactions.TryGetValue(generation, out completion!))
            {
                completion = new(false);
                _transactions.Add(generation, completion);
            }
        }
        var signaled = completion.Wait(timeout);
        lock (_gate)
        {
            if (_transactions.Remove(generation, out var removed)) removed.Dispose();
            if (!signaled)
                _retiredGeneration = Math.Max(_retiredGeneration, generation);
        }
        return signaled;
    }

    internal void Complete(long generation)
    {
        lock (_gate)
        {
            if (_disposed || generation <= _retiredGeneration || generation <= 0) return;
            if (!_transactions.TryGetValue(generation, out var completion))
            {
                completion = new(false);
                _transactions.Add(generation, completion);
            }
            completion.Set();
        }
    }

    internal void DiscardCompletion(long generation)
    {
        lock (_gate)
        {
            if (_transactions.Remove(generation, out var completion)) completion.Dispose();
        }
    }

    internal bool IsRetired(long generation)
    {
        lock (_gate) return _disposed || generation <= _retiredGeneration;
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            foreach (var completion in _transactions.Values)
            {
                completion.Set();
                completion.Dispose();
            }
            _transactions.Clear();
        }
    }
}

/// <summary>
/// Bounded task runner used by the child-window handler. PollOnce observes only
/// the matching transaction event; it never dispatches arbitrary WinUI or
/// nested WM_SIZE messages.
/// </summary>
internal sealed class WindowsPlatformTaskRunner(WindowsResizeCoordinator coordinator) : IDisposable
{
    private WindowsResizeCoordinator? _coordinator = coordinator;

    internal bool PollOnce(long generation, TimeSpan timeout) =>
        (_coordinator ?? throw new ObjectDisposedException(nameof(WindowsPlatformTaskRunner)))
        .Wait(generation, timeout);

    public void Dispose() => _coordinator = null;
}

#if DOROTI_WINDOWS_ANGLE_SPIKE
/// <summary>
/// Inactive W0 diagnostic spike retained with the failed fallback evidence.
/// The product presenter remains WindowsHwndD3D12Presenter; this code is not
/// compiled unless a dedicated spike project defines the opt-in symbol and
/// supplies the matching desktop ANGLE runtime.
/// </summary>
internal sealed class WindowsHwndAngleEglPresenter : IDisposable
{
    private const string AngleLibrary = "av_libglesv2.dll";
    private const int EglFalse = 0;
    private const int EglNone = 0x3038;
    private const int EglRedSize = 0x3024;
    private const int EglGreenSize = 0x3023;
    private const int EglBlueSize = 0x3022;
    private const int EglAlphaSize = 0x3021;
    private const int EglDepthSize = 0x3025;
    private const int EglStencilSize = 0x3026;
    private const int EglContextClientVersion = 0x3098;
    private const int EglOpenGlesApi = 0x30A0;
    private const int EglWidth = 0x3057;
    private const int EglHeight = 0x3056;
    private const int EglFixedSizeAngle = 0x3201;
    private const uint GlRenderer = 0x1F01;
    private const uint GlSamples = 0x80A9;
    private const uint GlStencilBits = 0x0D57;
    private const uint GlRgba8 = 0x8058;
    private nint _display;
    private nint _config;
    private nint _eglContext;
    private nint _eglSurface;
    private nint _windowHandle;
    private GRGlInterface? _glInterface;
    private GRContext? _context;
    private GRBackendRenderTarget? _renderTarget;
    private SKSurface? _windowSurface;
    private SKSurface? _backingSurface;
    private bool _surfaceNeedsFirstSwap;
    private string _adapterDescription = "uninitialized";

    internal static string RequestedSwapIntervalPolicy =>
        Environment.GetEnvironmentVariable("DOROTI_WINDOWS_EGL_SWAP_INTERVAL") switch
        {
            "0" => "ANGLE explicit eglSwapInterval(0)",
            "1" => "ANGLE explicit eglSwapInterval(1)",
            _ => "ANGLE default swap interval",
        };

    internal int Width { get; private set; }
    internal int Height { get; private set; }
    internal bool SurfaceChanged { get; private set; }
    internal bool LastCommitResized { get; private set; }
    internal TimeSpan LastDwmFlushDuration { get; private set; }
    internal string AdapterDescription => _adapterDescription;
    internal GRContext Context => _context ??
        throw new InvalidOperationException("ANGLE Skia context is unavailable.");
    internal SKSurface Surface => _backingSurface ??
        throw new InvalidOperationException("ANGLE exact offscreen backing surface is unavailable.");

    internal void EnsureTarget(nint windowHandle, int width, int height)
    {
        if (windowHandle == 0) throw new ArgumentOutOfRangeException(nameof(windowHandle));
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        SurfaceChanged = false;
        if (_display == 0) InitializeEgl();
        if (_windowHandle != 0 && _windowHandle != windowHandle)
        {
            Reset();
            InitializeEgl();
        }
        _windowHandle = windowHandle;
        if (_eglSurface != 0 && Width == width && Height == height)
        {
            MakeCurrent();
            return;
        }

        ReleaseWindowSurface();
        var attributes = new[]
        {
            EglFixedSizeAngle, 1,
            EglWidth, width,
            EglHeight, height,
            EglNone,
        };
        _eglSurface = EglCreateWindowSurface(_display, _config, windowHandle, attributes);
        if (_eglSurface == 0) ThrowEgl("eglCreateWindowSurface(EGL_FIXED_SIZE_ANGLE)");
        Width = width;
        Height = height;
        MakeCurrent();
        ApplyRequestedSwapInterval();

        _glInterface ??= GRGlInterface.CreateGles(EglGetProcAddress)
            ?? throw new InvalidOperationException("Skia could not resolve the desktop ANGLE GLES interface.");
        _context ??= GRContext.CreateGl(_glInterface)
            ?? throw new InvalidOperationException("Skia could not create an ANGLE GLES context.");
        _context.ResetContext(GRGlBackendState.All);
        GlGetIntegerv(GlSamples, out var sampleCount);
        GlGetIntegerv(GlStencilBits, out var stencilBits);
        _renderTarget = new GRBackendRenderTarget(
            width,
            height,
            Math.Max(0, sampleCount),
            Math.Max(0, stencilBits),
            new GRGlFramebufferInfo(0, GlRgba8));
        _windowSurface = SKSurface.Create(
            _context,
            _renderTarget,
            GRSurfaceOrigin.BottomLeft,
            SKColorType.Rgba8888)
            ?? throw new InvalidOperationException("Skia could not wrap the ANGLE default framebuffer.");
        _backingSurface = SKSurface.Create(
            _context,
            true,
            new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul),
            0,
            GRSurfaceOrigin.TopLeft)
            ?? throw new InvalidOperationException("Skia could not create the exact ANGLE offscreen backing surface.");
        var renderer = GlGetString(GlRenderer);
        _adapterDescription = renderer == 0
            ? "ANGLE renderer unavailable"
            : Marshal.PtrToStringAnsi(renderer) ?? "ANGLE renderer unavailable";
        SurfaceChanged = true;
        _surfaceNeedsFirstSwap = true;
    }

    internal void Flush()
    {
        MakeCurrent();
        Surface.Canvas.Flush();
        Context.Flush(Surface);
        Context.Submit(false);
    }

    internal void Present()
    {
        MakeCurrent();
        LastCommitResized = _surfaceNeedsFirstSwap;
        LastDwmFlushDuration = TimeSpan.Zero;
        var windowSurface = _windowSurface ??
            throw new InvalidOperationException("ANGLE fixed-size window surface is unavailable.");
        using (var image = Surface.Snapshot())
        using (var paint = new SKPaint { BlendMode = SKBlendMode.Src })
        {
            windowSurface.Canvas.DrawImage(image, 0, 0, SKSamplingOptions.Default, paint);
            windowSurface.Canvas.Flush();
            Context.Flush(windowSurface);
            Context.Submit(false);
        }
        if (EglSwapBuffers(_display, _eglSurface) == EglFalse)
            ThrowEgl("eglSwapBuffers");
        _surfaceNeedsFirstSwap = false;
    }

    internal bool FlushDwmAfterResize()
    {
        if (!LastCommitResized) return true;
        var started = DorotiFrameClock.Now;
        var result = DwmFlush();
        LastDwmFlushDuration = DorotiFrameClock.Now - started;
        return result >= 0;
    }

    internal void ReleasePresentedBuffer() { }

    private void InitializeEgl()
    {
        _display = EglGetDisplay(0);
        if (_display == 0) ThrowEgl("eglGetDisplay");
        if (EglInitialize(_display, out _, out _) == EglFalse)
            ThrowEgl("eglInitialize");
        if (EglBindApi(EglOpenGlesApi) == EglFalse)
            ThrowEgl("eglBindAPI(EGL_OPENGL_ES_API)");
        var configAttributes = new[]
        {
            EglRedSize, 8,
            EglGreenSize, 8,
            EglBlueSize, 8,
            EglAlphaSize, 8,
            EglDepthSize, 8,
            EglStencilSize, 8,
            EglNone,
        };
        if (EglChooseConfig(_display, configAttributes, out _config, 1, out var configCount) == EglFalse ||
            configCount <= 0 || _config == 0)
            ThrowEgl("eglChooseConfig");
        var contextAttributes = new[]
        {
            EglContextClientVersion, 2,
            EglNone,
        };
        _eglContext = EglCreateContext(_display, _config, 0, contextAttributes);
        if (_eglContext == 0) ThrowEgl("eglCreateContext");
    }

    private void ApplyRequestedSwapInterval()
    {
        var requested = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_EGL_SWAP_INTERVAL");
        if (requested is not ("0" or "1")) return;
        if (EglSwapInterval(_display, requested == "0" ? 0 : 1) == EglFalse)
            ThrowEgl($"eglSwapInterval({requested})");
    }

    private void MakeCurrent()
    {
        if (_display == 0 || _eglSurface == 0 || _eglContext == 0)
            throw new InvalidOperationException("ANGLE EGL target is incomplete.");
        if (EglMakeCurrent(_display, _eglSurface, _eglSurface, _eglContext) == EglFalse)
            ThrowEgl("eglMakeCurrent");
    }

    private void ReleaseWindowSurface()
    {
        _backingSurface?.Dispose();
        _backingSurface = null;
        _windowSurface?.Dispose();
        _windowSurface = null;
        _renderTarget?.Dispose();
        _renderTarget = null;
        if (_display != 0 && _eglSurface != 0)
        {
            EglMakeCurrent(_display, 0, 0, 0);
            EglDestroySurface(_display, _eglSurface);
        }
        _eglSurface = 0;
        Width = 0;
        Height = 0;
        _surfaceNeedsFirstSwap = false;
    }

    internal void Reset()
    {
        if (_display != 0 && _eglSurface != 0 && _eglContext != 0)
            EglMakeCurrent(_display, _eglSurface, _eglSurface, _eglContext);
        _backingSurface?.Dispose();
        _backingSurface = null;
        _windowSurface?.Dispose();
        _windowSurface = null;
        _renderTarget?.Dispose();
        _renderTarget = null;
        _context?.AbandonContext(false);
        _context?.Dispose();
        _context = null;
        _glInterface?.Dispose();
        _glInterface = null;
        if (_display != 0) EglMakeCurrent(_display, 0, 0, 0);
        if (_display != 0 && _eglSurface != 0) EglDestroySurface(_display, _eglSurface);
        if (_display != 0 && _eglContext != 0) EglDestroyContext(_display, _eglContext);
        if (_display != 0) EglTerminate(_display);
        _display = 0;
        _config = 0;
        _eglContext = 0;
        _eglSurface = 0;
        _windowHandle = 0;
        Width = 0;
        Height = 0;
        SurfaceChanged = false;
        LastCommitResized = false;
        LastDwmFlushDuration = TimeSpan.Zero;
        _surfaceNeedsFirstSwap = false;
        _adapterDescription = "uninitialized";
    }

    private static void ThrowEgl(string operation)
    {
        var error = EglGetError();
        throw new InvalidOperationException($"{operation} failed with EGL error 0x{error:x4}.");
    }

    public void Dispose() => Reset();

    [DllImport(AngleLibrary, EntryPoint = "EGL_GetDisplay", ExactSpelling = true)]
    private static extern nint EglGetDisplay(nint nativeDisplay);

    [DllImport(AngleLibrary, EntryPoint = "EGL_Initialize", ExactSpelling = true)]
    private static extern int EglInitialize(nint display, out int major, out int minor);

    [DllImport(AngleLibrary, EntryPoint = "EGL_BindAPI", ExactSpelling = true)]
    private static extern int EglBindApi(int api);

    [DllImport(AngleLibrary, EntryPoint = "EGL_ChooseConfig", ExactSpelling = true)]
    private static extern int EglChooseConfig(
        nint display,
        int[] attributes,
        out nint config,
        int configSize,
        out int configCount);

    [DllImport(AngleLibrary, EntryPoint = "EGL_CreateContext", ExactSpelling = true)]
    private static extern nint EglCreateContext(
        nint display,
        nint config,
        nint sharedContext,
        int[] attributes);

    [DllImport(AngleLibrary, EntryPoint = "EGL_CreateWindowSurface", ExactSpelling = true)]
    private static extern nint EglCreateWindowSurface(
        nint display,
        nint config,
        nint nativeWindow,
        int[] attributes);

    [DllImport(AngleLibrary, EntryPoint = "EGL_MakeCurrent", ExactSpelling = true)]
    private static extern int EglMakeCurrent(
        nint display,
        nint drawSurface,
        nint readSurface,
        nint context);

    [DllImport(AngleLibrary, EntryPoint = "EGL_SwapInterval", ExactSpelling = true)]
    private static extern int EglSwapInterval(nint display, int interval);

    [DllImport(AngleLibrary, EntryPoint = "EGL_SwapBuffers", ExactSpelling = true)]
    private static extern int EglSwapBuffers(nint display, nint surface);

    [DllImport(AngleLibrary, EntryPoint = "EGL_DestroySurface", ExactSpelling = true)]
    private static extern int EglDestroySurface(nint display, nint surface);

    [DllImport(AngleLibrary, EntryPoint = "EGL_DestroyContext", ExactSpelling = true)]
    private static extern int EglDestroyContext(nint display, nint context);

    [DllImport(AngleLibrary, EntryPoint = "EGL_Terminate", ExactSpelling = true)]
    private static extern int EglTerminate(nint display);

    [DllImport(AngleLibrary, EntryPoint = "EGL_GetError", ExactSpelling = true)]
    private static extern int EglGetError();

    [DllImport(AngleLibrary, EntryPoint = "EGL_GetProcAddress", ExactSpelling = true,
        CharSet = CharSet.Ansi)]
    private static extern nint EglGetProcAddress(string name);

    [DllImport(AngleLibrary, EntryPoint = "glGetIntegerv", ExactSpelling = true)]
    private static extern void GlGetIntegerv(uint name, out int value);

    [DllImport(AngleLibrary, EntryPoint = "glGetString", ExactSpelling = true)]
    private static extern nint GlGetString(uint name);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmFlush();
}
#endif

internal sealed class WindowsHwndD3D12Presenter : IDisposable
{
    private const uint WaitObject0 = 0;
    private const uint WaitTimeout = 0x00000102;
    private IDXGIAdapter1? _adapter;
    private ID3D12Device2? _device;
    private ID3D12CommandQueue? _queue;
    private IDXGIFactory2? _factory;
    private IDXGISwapChain3? _swapChain;
    private ID3D12CommandAllocator? _copyAllocator;
    private ID3D12GraphicsCommandList? _copyCommandList;
    private ID3D12Fence? _copyFence;
    private ulong _nextFenceValue;
    private ulong _lastSubmittedFence;
    private ulong _lastConfirmedFence;
    private nint _frameLatencyWaitableObject;
    private bool _hasPresented;
    private GRVorticeD3DBackendContext? _backend;
    private GRContext? _context;
    private WindowsD3D12BackingStore? _backingStore;
    private nint _windowHandle;
    private int _swapChainWidth;
    private int _swapChainHeight;
    private string _adapterDescription = "uninitialized";

    internal int Width { get; private set; }
    internal int Height { get; private set; }
    internal bool SurfaceChanged { get; private set; }
    internal bool LastCommitResized { get; private set; }
    internal TimeSpan LastDwmFlushDuration { get; private set; }
    internal string AdapterDescription => _adapterDescription;
    internal GRContext Context => _context ??
        throw new InvalidOperationException("D3D12 Skia context is unavailable.");
    internal SKSurface Surface => _backingStore?.Surface ??
        throw new InvalidOperationException("D3D12 offscreen Skia backing store is unavailable.");

    internal void EnsureTarget(nint windowHandle, int width, int height)
    {
        if (windowHandle == 0) throw new ArgumentOutOfRangeException(nameof(windowHandle));
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        SurfaceChanged = false;
        EnsureDevice(windowHandle);
        if (_windowHandle != windowHandle)
        {
            ReleaseSwapChain(waitForGpu: true, releaseBackingStore: true);
            _windowHandle = windowHandle;
        }
        if (_swapChain is null)
        {
            var description = new SwapChainDescription1(
                (uint)width,
                (uint)height,
                Format.R8G8B8A8_UNorm,
                false,
                Usage.RenderTargetOutput,
                2,
                Scaling.None,
                SwapEffect.FlipSequential,
                AlphaMode.Ignore,
                SwapChainFlags.FrameLatencyWaitableObject);
            using var created = _factory!.CreateSwapChainForHwnd(
                _queue!, windowHandle, description, null, null);
            _swapChain = created.QueryInterface<IDXGISwapChain3>();
            using var swapChain2 = _swapChain.QueryInterface<IDXGISwapChain2>();
            swapChain2.MaximumFrameLatency = 1;
            _frameLatencyWaitableObject = swapChain2.FrameLatencyWaitableObject;
            if (_frameLatencyWaitableObject == 0)
                throw new InvalidOperationException("DXGI did not expose a frame-latency waitable object.");
            _swapChainWidth = width;
            _swapChainHeight = height;
            SurfaceChanged = true;
        }
        WaitForPreviousCopy();
        _backingStore ??= new(_device!, Context);
        SurfaceChanged |= _backingStore.EnsureSize(width, height);
        Width = width;
        Height = height;
    }

    internal void Flush()
    {
        Surface.Canvas.Flush();
        Context.Flush(Surface);
        Context.Submit(false);
    }

    internal void Present()
    {
        var swapChain = _swapChain ??
            throw new InvalidOperationException("HWND swap chain is unavailable.");
        var backingStore = _backingStore ??
            throw new InvalidOperationException("D3D12 backing store is unavailable.");
        // Consume a frame-latency token only for work that will actually call
        // Present. An exact-scene miss may return after EnsureTarget; waiting
        // there consumes the available token without enqueueing the present
        // that would signal the next one.
        if (_hasPresented) WaitForFrameLatency();
        var resized = _swapChainWidth != Width || _swapChainHeight != Height;
        LastCommitResized = resized;
        LastDwmFlushDuration = TimeSpan.Zero;
        if (resized)
        {
            // No back-buffer wrapper survives a commit. The fence covers the
            // previous GPU copy before ResizeBuffers invalidates old buffers.
            WaitForPreviousCopy();
            swapChain.ResizeBuffers(
                2,
                (uint)Width,
                (uint)Height,
                Format.R8G8B8A8_UNorm,
                SwapChainFlags.FrameLatencyWaitableObject).CheckError();
            _swapChainWidth = Width;
            _swapChainHeight = Height;
        }

        WaitForPreviousCopy();
        _copyAllocator!.Reset();
        _copyCommandList!.Reset(_copyAllocator);
        using (var buffer = swapChain.GetBuffer<ID3D12Resource>(swapChain.CurrentBackBufferIndex))
        {
            _copyCommandList.ResourceBarrier(
            [
                ResourceBarrier.BarrierTransition(
                    backingStore.Resource,
                    ResourceStates.RenderTarget,
                    ResourceStates.CopySource),
                ResourceBarrier.BarrierTransition(
                    buffer,
                    ResourceStates.Present,
                    ResourceStates.CopyDest),
            ]);
            _copyCommandList.CopyResource(buffer, backingStore.Resource);
            _copyCommandList.ResourceBarrier(
            [
                ResourceBarrier.BarrierTransition(
                    backingStore.Resource,
                    ResourceStates.CopySource,
                    ResourceStates.RenderTarget),
                ResourceBarrier.BarrierTransition(
                    buffer,
                    ResourceStates.CopyDest,
                    ResourceStates.Present),
            ]);
            _copyCommandList.Close();
            _queue!.ExecuteCommandList(_copyCommandList);
        }
        _lastSubmittedFence = checked(++_nextFenceValue);
        _queue!.Signal(_copyFence!, _lastSubmittedFence).CheckError();
        // The allocator, transient back-buffer wrapper, and exact backing store
        // are all reused or released by the next resize transaction. Confirm
        // this copy's own monotonically increasing fence value here; deferring
        // the wait until a later transaction lets an old auto-reset event pulse
        // satisfy a newer wait while the queue is still at fence value zero.
        WaitForPreviousCopy();
        swapChain.Present(0, PresentFlags.None).CheckError();
        _hasPresented = true;
    }

    internal bool FlushDwmAfterResize()
    {
        if (!LastCommitResized) return true;
        var started = DorotiFrameClock.Now;
        var result = DwmFlush();
        LastDwmFlushDuration = DorotiFrameClock.Now - started;
        return result >= 0;
    }

    internal void ReleasePresentedBuffer() { }

    private void EnsureDevice(nint windowHandle)
    {
        if (_context is not null) return;
        _factory = CreateDXGIFactory2<IDXGIFactory2>(false);
        _adapter = FindAdapterForWindowMonitor(_factory, windowHandle);
        if (_adapter is null)
        {
            using var factory6 = _factory.QueryInterface<IDXGIFactory6>();
            _adapter = factory6.EnumAdapterByGpuPreference<IDXGIAdapter1>(0, GpuPreference.MinimumPower);
        }
        _adapterDescription = _adapter.Description1.Description;
        _device = D3D12CreateDevice<ID3D12Device2>(_adapter, FeatureLevel.Level_11_0);
        _queue = _device.CreateCommandQueue(CommandListType.Direct, 0, CommandQueueFlags.None, 0);
        _copyAllocator = _device.CreateCommandAllocator(CommandListType.Direct);
        _copyCommandList = _device.CreateCommandList<ID3D12GraphicsCommandList>(
            CommandListType.Direct, _copyAllocator, null);
        _copyCommandList.Close();
        _copyFence = _device.CreateFence(0, FenceFlags.None);
        _backend = new GRVorticeD3DBackendContext
        {
            Adapter = _adapter,
            Device = _device,
            Queue = _queue,
        };
        _context = GRContext.CreateDirect3D(_backend)
            ?? throw new InvalidOperationException("Skia could not create the Doroti D3D12 context.");
    }

    private static IDXGIAdapter1? FindAdapterForWindowMonitor(
        IDXGIFactory2 factory,
        nint windowHandle)
    {
        var monitor = MonitorFromWindow(windowHandle, 2);
        if (monitor == 0) return null;
        for (uint adapterIndex = 0; ; adapterIndex++)
        {
            var adapterResult = factory.EnumAdapters1(adapterIndex, out var adapter);
            if (adapterResult.Failure) break;
            var matched = false;
            for (uint outputIndex = 0; ; outputIndex++)
            {
                var outputResult = adapter.EnumOutputs(outputIndex, out var output);
                if (outputResult.Failure) break;
                using (output)
                {
                    if (output.Description.Monitor == monitor)
                    {
                        matched = true;
                        break;
                    }
                }
            }
            if (matched) return adapter;
            adapter.Dispose();
        }
        return null;
    }

    private void WaitForFrameLatency()
    {
        var result = WaitForSingleObject(_frameLatencyWaitableObject, 100);
        if (result is not WaitObject0)
        {
            var detail = result == WaitTimeout ? "timed out" : $"failed with 0x{result:x8}";
            throw new TimeoutException($"DXGI frame-latency wait {detail}.");
        }
    }

    private void WaitForPreviousCopy()
    {
        var target = _lastSubmittedFence;
        if (target == 0 || _lastConfirmedFence >= target) return;
        var completedBeforeWait = _copyFence!.CompletedValue;
        if (completedBeforeWait >= target)
        {
            _lastConfirmedFence = target;
            return;
        }
        using var completion = new EventWaitHandle(false, EventResetMode.AutoReset);
        _copyFence.SetEventOnCompletion(target, completion).CheckError();
        if (!completion.WaitOne(TimeSpan.FromMilliseconds(100)))
        {
            var completedAfterWait = _copyFence.CompletedValue;
            var removedReason = _device?.DeviceRemovedReason;
            throw new TimeoutException(
                $"D3D12 copy fence did not complete within 100ms " +
                $"(submitted={target}, confirmed={_lastConfirmedFence}, " +
                $"completedBefore={completedBeforeWait}, " +
                $"completedAfter={completedAfterWait}, deviceRemovedReason={removedReason}).");
        }
        _lastConfirmedFence = target;
    }

    private void ReleaseSwapChain(bool waitForGpu, bool releaseBackingStore)
    {
        if (waitForGpu && _copyFence is not null) WaitForPreviousCopy();
        if (releaseBackingStore)
        {
            _backingStore?.Dispose();
            _backingStore = null;
        }
        _swapChain?.Dispose();
        _swapChain = null;
        _frameLatencyWaitableObject = 0;
        _hasPresented = false;
        _windowHandle = 0;
        Width = Height = 0;
        _swapChainWidth = _swapChainHeight = 0;
    }

    internal void Reset()
    {
        ReleaseSwapChain(waitForGpu: false, releaseBackingStore: true);
        _context?.AbandonContext(false);
        _context?.Dispose();
        _context = null;
        _backend?.Dispose();
        _backend = null;
        _copyCommandList?.Dispose();
        _copyCommandList = null;
        _copyAllocator?.Dispose();
        _copyAllocator = null;
        _copyFence?.Dispose();
        _copyFence = null;
        _lastSubmittedFence = 0;
        _lastConfirmedFence = 0;
        _queue?.Dispose();
        _queue = null;
        _device?.Dispose();
        _device = null;
        _adapter?.Dispose();
        _adapter = null;
        _adapterDescription = "uninitialized";
        _factory?.Dispose();
        _factory = null;
    }

    public void Dispose()
    {
        Reset();
    }

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern uint WaitForSingleObject(nint handle, uint milliseconds);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint MonitorFromWindow(nint windowHandle, uint flags);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmFlush();
}

internal sealed class WindowsD3D12BackingStore : IDisposable
{
    private readonly ID3D12Device _device;
    private readonly GRContext _context;
    private ID3D12Resource? _resource;
    private GRVorticeD3DTextureResourceInfo? _resourceInfo;
    private GRBackendRenderTarget? _renderTarget;
    private SKSurface? _surface;

    internal WindowsD3D12BackingStore(ID3D12Device device, GRContext context)
    {
        _device = device;
        _context = context;
    }

    internal int Width { get; private set; }
    internal int Height { get; private set; }
    internal ID3D12Resource Resource => _resource ??
        throw new InvalidOperationException("D3D12 backing-store resource is unavailable.");
    internal SKSurface Surface => _surface ??
        throw new InvalidOperationException("D3D12 backing-store Skia surface is unavailable.");

    internal bool EnsureSize(int width, int height)
    {
        if (_surface is not null && Width == width && Height == height) return false;
        ReleaseResources();
        var description = ResourceDescription.Texture2D(
            Format.R8G8B8A8_UNorm,
            checked((uint)width),
            checked((uint)height),
            1,
            1,
            1,
            0,
            ResourceFlags.AllowRenderTarget);
        _resource = _device.CreateCommittedResource(
            HeapType.Default,
            HeapFlags.None,
            description,
            ResourceStates.RenderTarget,
            null);
        _resourceInfo = new GRVorticeD3DTextureResourceInfo
        {
            Resource = _resource,
            ResourceState = ResourceStates.RenderTarget,
            Format = Format.R8G8B8A8_UNorm,
            SampleCount = 1,
            LevelCount = 1,
        };
        _renderTarget = new GRBackendRenderTarget(width, height, _resourceInfo);
        _surface = SKSurface.Create(
            _context,
            _renderTarget,
            GRSurfaceOrigin.TopLeft,
            SKColorType.Rgba8888) ??
            throw new InvalidOperationException("Skia could not wrap the D3D12 offscreen backing store.");
        Width = width;
        Height = height;
        return true;
    }

    private void ReleaseResources()
    {
        _surface?.Dispose();
        _surface = null;
        _renderTarget?.Dispose();
        _renderTarget = null;
        _resourceInfo?.Dispose();
        _resourceInfo = null;
        _resource?.Dispose();
        _resource = null;
        Width = Height = 0;
    }

    public void Dispose() => ReleaseResources();
}

internal sealed class WindowsD3D12Presenter : IDisposable
{
    private static readonly Guid SwapChainPanelNativeInterface =
        new("63aad0b8-7c24-40ff-85a8-640d944cc325");
    private IDXGIAdapter1? _adapter;
    private ID3D12Device2? _device;
    private ID3D12CommandQueue? _queue;
    private IDXGIFactory2? _factory;
    private IDXGISwapChain3? _renderSwapChain;
    private IDXGISwapChain3? _presentedSwapChain;
    private GRVorticeD3DBackendContext? _backend;
    private GRContext? _context;
    private ID3D12Resource? _buffer;
    private GRVorticeD3DTextureResourceInfo? _resourceInfo;
    private GRBackendRenderTarget? _renderTarget;
    private SKSurface? _surface;
    private SwapChainPanel? _panel;
    private double _compositionScaleX;
    private double _compositionScaleY;
    private int _renderWidth;
    private int _renderHeight;
    private int _presentedWidth;
    private int _presentedHeight;

    internal int Width { get; private set; }
    internal int Height { get; private set; }
    internal bool SurfaceChanged { get; private set; }
    internal double CompositionScaleX => _compositionScaleX;
    internal double CompositionScaleY => _compositionScaleY;
    internal GRContext Context => _context ?? throw new InvalidOperationException("D3D12 Skia context is unavailable.");
    internal SKSurface Surface => _surface ?? throw new InvalidOperationException("D3D12 Skia surface is unavailable.");

    internal void EnsureTarget(
        SwapChainPanel panel,
        int width,
        int height,
        double compositionScaleX,
        double compositionScaleY)
    {
        SurfaceChanged = false;
        EnsureDevice();
        if (!ReferenceEquals(panel, _panel))
        {
            ReleaseSwapChains(waitForGpu: true);
            _panel = panel;
        }
        if (_renderSwapChain is null)
        {
            var description = new SwapChainDescription1(
                (uint)width, (uint)height, Format.R8G8B8A8_UNorm,
                false, Usage.RenderTargetOutput, 2,
                Scaling.Stretch, SwapEffect.FlipSequential, AlphaMode.Ignore, SwapChainFlags.None);
            using var created = _factory!.CreateSwapChainForComposition(_queue!, description, null);
            _renderSwapChain = created.QueryInterface<IDXGISwapChain3>();
            _renderWidth = width;
            _renderHeight = height;
            SurfaceChanged = true;
        }
        else if (_renderWidth != width || _renderHeight != height)
        {
            CompleteGpuWorkAndReleaseBuffer();
            _renderSwapChain.ResizeBuffers(2,
                (uint)width,
                (uint)height,
                Format.R8G8B8A8_UNorm,
                SwapChainFlags.None).CheckError();
            _renderWidth = width;
            _renderHeight = height;
            SurfaceChanged = true;
        }
        Width = width;
        Height = height;
        ApplyCompositionScale(compositionScaleX, compositionScaleY);
        CreateCurrentBufferSurface();
    }

    private static void AttachSwapChainOnUiThread(SwapChainPanel panel, IDXGISwapChain3 swapChain)
    {
        using var panelReference = ((WinRT.IWinRTObject)panel).NativeObject.As(SwapChainPanelNativeInterface);
        using var nativePanel = new ISwapChainPanelNative(panelReference.GetRef());
        nativePanel.SetSwapChain(swapChain).CheckError();
    }

    private void ApplyCompositionScale(double compositionScaleX, double compositionScaleY)
    {
        if (!double.IsFinite(compositionScaleX) || compositionScaleX <= 0)
            throw new ArgumentOutOfRangeException(nameof(compositionScaleX));
        if (!double.IsFinite(compositionScaleY) || compositionScaleY <= 0)
            throw new ArgumentOutOfRangeException(nameof(compositionScaleY));
        var safeScaleX = compositionScaleX;
        var safeScaleY = compositionScaleY;
        // SwapChainPanel applies its composition scale after the composition
        // swap chain is rendered. The back buffer is already allocated in
        // physical pixels, so cancel that scale to map it back to XAML DIPs.
        // Apply it to every staging chain because the two chains alternate.
        using var swapChain2 = _renderSwapChain!.QueryInterface<IDXGISwapChain2>();
        swapChain2.MatrixTransform = Matrix3x2.CreateScale(
            (float)(1.0 / safeScaleX),
            (float)(1.0 / safeScaleY));
        _compositionScaleX = safeScaleX;
        _compositionScaleY = safeScaleY;
    }

    internal void Flush()
    {
        Surface.Canvas.Flush();
        Context.Flush(Surface);
        Context.Submit(true);
    }

    internal bool TryCommitViewportAndPresent(
        double logicalWidth,
        double logicalHeight,
        long expectedTargetGeneration,
        Func<long> latestTargetGeneration,
        out long observedTargetGeneration,
        Action onCommitStarting,
        Action<Action> invokeOnUiThread)
    {
        if (!double.IsFinite(logicalWidth) || logicalWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(logicalWidth));
        if (!double.IsFinite(logicalHeight) || logicalHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(logicalHeight));
        ArgumentNullException.ThrowIfNull(latestTargetGeneration);
        ArgumentNullException.ThrowIfNull(onCommitStarting);
        var panel = _panel ?? throw new InvalidOperationException("Swap-chain panel is unavailable.");
        var committed = false;
        var observed = 0L;
        invokeOnUiThread(() =>
        {
            // Size publication and this callback are both serialized by the
            // Windows UI thread. Recheck here, after any queued SizeChanged
            // work and immediately before the panel/Present transaction, so a
            // stale raster cannot slip through the dispatcher queue gap.
            observed = latestTargetGeneration();
            if (observed != expectedTargetGeneration) return;
            onCommitStarting();
            // The panel keeps presenting the previous exact swap chain while
            // raster prepares this detached staging chain. Present the exact
            // staging buffer first, then atomically replace the panel binding;
            // no capacity/source scaling or destructive resize touches the
            // currently visible chain.
            var next = _renderSwapChain ??
                throw new InvalidOperationException("Staging swap chain is unavailable.");
            next.Present(0, PresentFlags.None).CheckError();
            panel.Width = logicalWidth;
            panel.Height = logicalHeight;
            AttachSwapChainOnUiThread(panel, next);
            var previous = _presentedSwapChain;
            var previousWidth = _presentedWidth;
            var previousHeight = _presentedHeight;
            _presentedSwapChain = next;
            _presentedWidth = Width;
            _presentedHeight = Height;
            _renderSwapChain = previous;
            _renderWidth = previousWidth;
            _renderHeight = previousHeight;
            committed = true;
        });
        observedTargetGeneration = observed;
        return committed;
    }

    internal void ReleasePresentedBuffer() => ReleaseBuffer();

    private void EnsureDevice()
    {
        if (_context is not null) return;
        _factory = CreateDXGIFactory2<IDXGIFactory2>(false);
        using var factory6 = _factory.QueryInterface<IDXGIFactory6>();
        _adapter = factory6.EnumAdapterByGpuPreference<IDXGIAdapter1>(0, GpuPreference.HighPerformance);
        _device = D3D12CreateDevice<ID3D12Device2>(_adapter, FeatureLevel.Level_11_0);
        _queue = _device.CreateCommandQueue(CommandListType.Direct, 0, CommandQueueFlags.None, 0);
        _backend = new GRVorticeD3DBackendContext
        {
            Adapter = _adapter,
            Device = _device,
            Queue = _queue,
        };
        _context = GRContext.CreateDirect3D(_backend)
            ?? throw new InvalidOperationException("Skia could not create the Doroti D3D12 context.");
    }

    private void CreateCurrentBufferSurface()
    {
        ReleaseBuffer();
        _buffer = _renderSwapChain!.GetBuffer<ID3D12Resource>(_renderSwapChain.CurrentBackBufferIndex);
        _resourceInfo = new GRVorticeD3DTextureResourceInfo
        {
            Resource = _buffer,
            ResourceState = ResourceStates.Present,
            Format = Format.R8G8B8A8_UNorm,
            SampleCount = 1,
            LevelCount = 1,
        };
        _renderTarget = new GRBackendRenderTarget(Width, Height, _resourceInfo);
        _surface = SKSurface.Create(Context, _renderTarget, GRSurfaceOrigin.TopLeft, SKColorType.Rgba8888);
        if (_surface is null)
        {
            throw new InvalidOperationException(
                $"Skia could not wrap the current DXGI back buffer: " +
                $"renderTargetValid={_renderTarget.IsValid}, " +
                $"renderTargetBackend={_renderTarget.Backend}, " +
                $"maxSamples={Context.GetMaxSurfaceSampleCount(SKColorType.Rgba8888)}, " +
                $"resourceFormat={_resourceInfo.Format}, resourceState={_resourceInfo.ResourceState}.");
        }
    }

    private void ReleaseBuffer()
    {
        _surface?.Dispose();
        _surface = null;
        _renderTarget?.Dispose();
        _renderTarget = null;
        _resourceInfo?.Dispose();
        _resourceInfo = null;
        _buffer?.Dispose();
        _buffer = null;
    }

    private void CompleteGpuWorkAndReleaseBuffer()
    {
        if (_surface is not null)
        {
            _surface.Canvas.Flush();
            Context.Flush(_surface);
        }
        Context.Submit(true);
        ReleaseBuffer();
    }

    private void ReleaseSwapChains(bool waitForGpu)
    {
        if (waitForGpu && _surface is not null && _context is not null)
            CompleteGpuWorkAndReleaseBuffer();
        else
            ReleaseBuffer();
        _renderSwapChain?.Dispose();
        _renderSwapChain = null;
        _presentedSwapChain?.Dispose();
        _presentedSwapChain = null;
        _panel = null;
        _compositionScaleX = 0;
        _compositionScaleY = 0;
        Width = Height = 0;
        _renderWidth = _renderHeight = 0;
        _presentedWidth = _presentedHeight = 0;
    }

    internal void Reset()
    {
        ReleaseSwapChains(waitForGpu: false);
        _context?.AbandonContext(false);
        _context?.Dispose();
        _context = null;
        _backend?.Dispose();
        _backend = null;
        _queue?.Dispose();
        _queue = null;
        _device?.Dispose();
        _device = null;
        _adapter?.Dispose();
        _adapter = null;
        _factory?.Dispose();
        _factory = null;
    }

    public void Dispose() => Reset();
}
#endif
