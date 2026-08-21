#if WINDOWS
using System.Diagnostics.Tracing;
using System.Numerics;
using Doroti.Ui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
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

public sealed class DorotiWindowsDxgiElementHandler
    : ViewHandler<DorotiWindowsDxgiElement, SwapChainPanel>
{
    public static readonly IPropertyMapper<DorotiWindowsDxgiElement, DorotiWindowsDxgiElementHandler> Mapper =
        new PropertyMapper<DorotiWindowsDxgiElement, DorotiWindowsDxgiElementHandler>();

    public DorotiWindowsDxgiElementHandler() : base(Mapper) { }
    protected override SwapChainPanel CreatePlatformView() => new();
    protected override void ConnectHandler(SwapChainPanel platformView)
    {
        base.ConnectHandler(platformView);
        VirtualView.Owner.Connect(platformView);
    }
    protected override void DisconnectHandler(SwapChainPanel platformView)
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
    private readonly Thread _rasterThread;
    private readonly DorotiResizeTargetCoordinator _targets = new();
    private readonly DorotiResizeTrace _trace = new();
    private SwapChainPanel? _panel;
    private DorotiResizeEpoch? _latestTarget;
    private long _requestSerial;
    private long _surfaceGeneration;
    private long _presented;
    private long _superseded;
    private long _activations;
    private long _deactivations;
    private MauiPaintCompletion? _activeCompletion;
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
        };
        _rasterThread.SetApartmentState(ApartmentState.MTA);
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
    public event Action? SizeChanged;

    internal void Connect(SwapChainPanel panel)
    {
        lock (_gate)
        {
            if (_disposed) return;
            _panel = panel;
        }
        panel.Loaded += HandlePanelLoaded;
        panel.Unloaded += HandlePanelUnloaded;
        panel.SizeChanged += HandlePanelSizeChanged;
        panel.CompositionScaleChanged += HandleCompositionScaleChanged;
        panel.PointerPressed += HandlePointerPressed;
        panel.PointerMoved += HandlePointerMoved;
        panel.PointerReleased += HandlePointerReleased;
        panel.PointerWheelChanged += HandlePointerWheel;
        panel.GotFocus += HandleGotFocus;
        panel.LostFocus += HandleLostFocus;
        panel.KeyDown += HandleKeyDown;
        panel.KeyUp += HandleKeyUp;
        if (panel.IsLoaded) PublishTarget("SwapChainPanel.Connect");
    }

    internal void Disconnect(SwapChainPanel panel)
    {
        panel.Loaded -= HandlePanelLoaded;
        panel.Unloaded -= HandlePanelUnloaded;
        panel.SizeChanged -= HandlePanelSizeChanged;
        panel.CompositionScaleChanged -= HandleCompositionScaleChanged;
        panel.PointerPressed -= HandlePointerPressed;
        panel.PointerMoved -= HandlePointerMoved;
        panel.PointerReleased -= HandlePointerReleased;
        panel.PointerWheelChanged -= HandlePointerWheel;
        panel.GotFocus -= HandleGotFocus;
        panel.LostFocus -= HandleLostFocus;
        panel.KeyDown -= HandleKeyDown;
        panel.KeyUp -= HandleKeyUp;
        lock (_gate)
        {
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
        var panel = _panel;
        if (panel is null) return;
        _ = focused
            ? panel.Focus(FocusState.Programmatic)
            : panel.Focus(FocusState.Unfocused);
    }

    public void SetCursor(DorotiMouseCursorKind cursor)
    {
        var panel = _panel;
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

    public MauiSurfaceSnapshot CaptureSnapshot(MauiSurfaceSnapshot current)
    {
        var target = _targets.Latest;
        return current with
        {
            PixelWidth = target?.PhysicalWidth ?? current.PixelWidth,
            PixelHeight = target?.PhysicalHeight ?? current.PixelHeight,
            DevicePixelRatio = target?.DevicePixelRatio ?? current.DevicePixelRatio,
            SurfaceGeneration = Interlocked.Read(ref _surfaceGeneration),
            NativeViewType = typeof(SwapChainPanel).FullName!,
            GraphicsBackend = "WinUI3/Doroti-owned-SwapChainPanel/DXGI-D3D12-Skia",
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

    private void HandleMauiLoaded(object? sender, EventArgs args) { _ = sender; _ = args; _loaded = true; PublishTarget("Maui.Loaded"); }
    private void HandleMauiUnloaded(object? sender, EventArgs args) { _ = sender; _ = args; _loaded = false; _wake.Set(); }
    private void HandleMauiSizeChanged(object? sender, EventArgs args) { _ = sender; _ = args; PublishTarget("Maui.SizeChanged"); }
    private void HandlePanelLoaded(object sender, RoutedEventArgs args) { _ = sender; _ = args; _loaded = true; PublishTarget("SwapChainPanel.Loaded"); }
    private void HandlePanelUnloaded(object sender, RoutedEventArgs args) { _ = sender; _ = args; _loaded = false; _wake.Set(); }
    private void HandlePanelSizeChanged(object sender, SizeChangedEventArgs args) { _ = sender; _ = args; PublishTarget("SwapChainPanel.SizeChanged"); }
    private void HandleCompositionScaleChanged(SwapChainPanel sender, object args) { _ = sender; _ = args; PublishTarget("SwapChainPanel.CompositionScaleChanged"); }

    private void PublishTarget(string source)
    {
        var panel = _panel;
        if (_disposed || !_loaded || panel is null) return;
        var target = _targets.Publish(
            Math.Max(0, panel.ActualWidth),
            Math.Max(0, panel.ActualHeight),
            Math.Max(1, panel.CompositionScaleX));
        lock (_gate)
        {
            if (_latestTarget?.Generation == target.Generation) return;
            _latestTarget = target;
            _requestSerial++;
        }
        Interlocked.Increment(ref _activations);
        Record("target", target, source);
        SizeChanged?.Invoke();
        _wake.Set();
    }

    private void RasterMain()
    {
        using var presenter = new WindowsD3D12Presenter();
        long processedSerial = -1;
        while (true)
        {
            _wake.WaitOne();
            DorotiResizeEpoch? target;
            SwapChainPanel? panel;
            long serial;
            lock (_gate)
            {
                if (_disposed) break;
                target = _latestTarget;
                panel = _panel;
                serial = _requestSerial;
            }
            if (!_loaded || panel is null || target is null || !target.HasDrawableSize || serial == processedSerial)
                continue;

            EventSource.SetCurrentThreadActivityId(Guid.NewGuid(), out var previousActivityId);
            _activeCompletion = null;
            try
            {
                presenter.EnsureTarget(
                    panel,
                    target.PhysicalWidth,
                    target.PhysicalHeight,
                    target.DevicePixelRatio,
                    InvokeOnUiThread);
                if (presenter.SurfaceChanged) Interlocked.Increment(ref _surfaceGeneration);
                var rasterStarted = DorotiFrameClock.Now;
                Record("raster-start", target, "D3D12 raster thread",
                    surfaceWidth: presenter.Width, surfaceHeight: presenter.Height);
                var paint = new MauiSkiaPaintContext(
                    presenter.Surface,
                    presenter.Context,
                    presenter.Width,
                    presenter.Height,
                    target.DevicePixelRatio,
                    Interlocked.Read(ref _surfaceGeneration),
                    typeof(SwapChainPanel).FullName!,
                    "WinUI3/Doroti-owned-SwapChainPanel/DXGI-D3D12-Skia");
                Paint?.Invoke(paint);
                _activeCompletion = paint.Completion;
                if (paint.Completion is not { } completion || !completion.Descriptor.IsExactFor(target))
                {
                    Interlocked.Increment(ref _superseded);
                    Record("ack", target, "exact-frame gate", terminal: "superseded",
                        detail: paint.Completion is null ? "no exact scene ready" : "descriptor mismatch");
                    if (paint.Completion is { } rejected) PresentCompleted?.Invoke(rejected, true);
                    processedSerial = serial;
                    continue;
                }
                presenter.Flush();
                Record("raster-end", target, "D3D12 raster thread", DorotiFrameClock.Now - rasterStarted,
                    surfaceWidth: presenter.Width, surfaceHeight: presenter.Height);
                lock (_gate)
                {
                    if (_latestTarget?.Generation != target.Generation || _requestSerial != serial)
                    {
                        Interlocked.Increment(ref _superseded);
                        Record("ack", target, "latest target gate", terminal: "superseded");
                        PresentCompleted?.Invoke(completion, true);
                        processedSerial = serial;
                        continue;
                    }
                }
                Record("pre-swap", target, "Doroti-owned DXGI", surfaceWidth: presenter.Width, surfaceHeight: presenter.Height);
                var presentStarted = DorotiFrameClock.Now;
                presenter.Present();
                Record("post-swap", target, "Doroti-owned DXGI", DorotiFrameClock.Now - presentStarted,
                    surfaceWidth: presenter.Width, surfaceHeight: presenter.Height);
                Interlocked.Increment(ref _presented);
                Record("ack", target, "D3D12 raster thread", terminal: "presented");
                PresentCompleted?.Invoke(completion, false);
                processedSerial = serial;
            }
            catch (Exception exception)
            {
                PaintFailed?.Invoke(_activeCompletion, exception);
                presenter.Reset();
                processedSerial = serial;
            }
            finally
            {
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
        var panel = (SwapChainPanel)sender;
        var point = args.GetCurrentPoint(panel);
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
        if (_panel is { } panel) Disconnect(panel);
        _wake.Set();
        if (Thread.CurrentThread != _rasterThread && !_rasterThread.Join(TimeSpan.FromSeconds(5)))
            PaintFailed?.Invoke(null, new TimeoutException("Windows D3D12 raster thread did not stop within five seconds."));
        _wake.Dispose();
    }
}

internal sealed class WindowsD3D12Presenter : IDisposable
{
    private static readonly Guid SwapChainPanelNativeInterface =
        new("63aad0b8-7c24-40ff-85a8-640d944cc325");
    private IDXGIAdapter1? _adapter;
    private ID3D12Device2? _device;
    private ID3D12CommandQueue? _queue;
    private IDXGIFactory2? _factory;
    private IDXGISwapChain3? _swapChain;
    private GRVorticeD3DBackendContext? _backend;
    private GRContext? _context;
    private ID3D12Resource? _buffer;
    private GRVorticeD3DTextureResourceInfo? _resourceInfo;
    private GRBackendRenderTarget? _renderTarget;
    private SKSurface? _surface;
    private SwapChainPanel? _panel;
    private double _compositionScale;

    internal int Width { get; private set; }
    internal int Height { get; private set; }
    internal bool SurfaceChanged { get; private set; }
    internal GRContext Context => _context ?? throw new InvalidOperationException("D3D12 Skia context is unavailable.");
    internal SKSurface Surface => _surface ?? throw new InvalidOperationException("D3D12 Skia surface is unavailable.");

    internal void EnsureTarget(
        SwapChainPanel panel,
        int width,
        int height,
        double compositionScale,
        Action<Action> invokeOnUiThread)
    {
        SurfaceChanged = false;
        EnsureDevice();
        if (_swapChain is null || !ReferenceEquals(panel, _panel))
        {
            ReleaseSwapChain();
            var description = new SwapChainDescription1(
                (uint)width, (uint)height, Format.R8G8B8A8_UNorm,
                false, Usage.RenderTargetOutput, 2,
                Scaling.Stretch, SwapEffect.FlipSequential, AlphaMode.Ignore, SwapChainFlags.None);
            using var created = _factory!.CreateSwapChainForComposition(_queue!, description, null);
            _swapChain = created.QueryInterface<IDXGISwapChain3>();
            invokeOnUiThread(() =>
            {
                using var panelReference = ((WinRT.IWinRTObject)panel).NativeObject.As(SwapChainPanelNativeInterface);
                using var nativePanel = new ISwapChainPanelNative(panelReference.GetRef());
                nativePanel.SetSwapChain(_swapChain).CheckError();
            });
            _panel = panel;
            Width = width;
            Height = height;
            SurfaceChanged = true;
        }
        else if (Width != width || Height != height)
        {
            ReleaseBuffer();
            Context.Submit(true);
            _swapChain.ResizeBuffers(2, (uint)width, (uint)height, Format.R8G8B8A8_UNorm, SwapChainFlags.None).CheckError();
            Width = width;
            Height = height;
            SurfaceChanged = true;
        }
        ApplyCompositionScale(compositionScale);
        CreateCurrentBufferSurface();
    }

    private void ApplyCompositionScale(double compositionScale)
    {
        var safeScale = Math.Max(1.0, compositionScale);
        if (Math.Abs(_compositionScale - safeScale) < 0.0001) return;

        // SwapChainPanel applies its composition scale after the composition
        // swap chain is rendered. The back buffer is already allocated in
        // physical pixels, so cancel that scale to map it back to XAML DIPs.
        using var swapChain2 = _swapChain!.QueryInterface<IDXGISwapChain2>();
        swapChain2.MatrixTransform = Matrix3x2.CreateScale((float)(1.0 / safeScale));
        _compositionScale = safeScale;
    }

    internal void Flush()
    {
        Surface.Canvas.Flush();
        Context.Flush(Surface);
        Context.Submit(true);
    }

    internal void Present()
    {
        _swapChain!.Present(0, PresentFlags.None).CheckError();
        ReleaseBuffer();
    }

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
        _buffer = _swapChain!.GetBuffer<ID3D12Resource>(_swapChain.CurrentBackBufferIndex);
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

    private void ReleaseSwapChain()
    {
        ReleaseBuffer();
        _swapChain?.Dispose();
        _swapChain = null;
        _panel = null;
        _compositionScale = 0;
        Width = Height = 0;
    }

    internal void Reset()
    {
        ReleaseSwapChain();
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
