using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using SharpGen.Runtime;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;
using Windows.UI.Composition;
using static Vortice.Direct3D11.D3D11;
using static Vortice.DXGI.DXGI;

namespace Doroti.Validation.WinRtContentIslandSpike;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        var options = SpikeOptions.Parse(args);
        var roResult = Native.RoInitialize(0);
        if (roResult < 0)
        {
            Console.Error.WriteLine($"RoInitialize failed: 0x{roResult:X8}");
            return 1;
        }

        try
        {
            using var spike = new WinRtIslandSpike(options);
            return spike.Run();
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            WriteFailure(options.ReportPath, exception);
            return 1;
        }
        finally
        {
            Native.RoUninitialize();
        }
    }

    private static void WriteFailure(string reportPath, Exception exception)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
        File.WriteAllText(reportPath, JsonSerializer.Serialize(new
        {
            schema = "doroti.winrt-content-island-spike/v1",
            status = "FAIL",
            exception = exception.ToString(),
        }, JsonOptions));
    }

    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}

internal sealed record SpikeOptions(bool Automated, int HoldMilliseconds, string ReportPath)
{
    internal static SpikeOptions Parse(string[] args)
    {
        var automated = args.Contains("--automated", StringComparer.OrdinalIgnoreCase);
        var hold = GetInt(args, "--hold-ms", automated ? 250 : 0);
        var report = GetString(args, "--report") ?? DefaultReportPath();
        if (hold < 0 || hold > 300_000) throw new ArgumentOutOfRangeException(nameof(hold));
        return new SpikeOptions(automated, hold, Path.GetFullPath(report));
    }

    private static int GetInt(string[] args, string name, int fallback)
    {
        var index = Array.FindIndex(args, value => value.Equals(name, StringComparison.OrdinalIgnoreCase));
        return index >= 0 && index + 1 < args.Length ? int.Parse(args[index + 1]) : fallback;
    }

    private static string? GetString(string[] args, string name)
    {
        var index = Array.FindIndex(args, value => value.Equals(name, StringComparison.OrdinalIgnoreCase));
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }

    private static string DefaultReportPath()
    {
        var current = new DirectoryInfo(Environment.CurrentDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Doroti", "Doroti.slnx")))
            {
                var id = $"w1-{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}"[..44];
                return Path.Combine(current.FullName, ".doroti", "evidence", id, "w1-report.json");
            }
            current = current.Parent;
        }
        throw new DirectoryNotFoundException("Unable to locate DorotiLab root.");
    }
}

internal sealed class WinRtIslandSpike : IDisposable
{
    private const uint WmAppRender = 0x8001;
    private const int InitialWidth = 700;
    private const int InitialHeight = 520;
    private static readonly Dictionary<nint, WinRtIslandSpike> Instances = [];
    private static readonly Native.WndProc WindowProcedure = WndProc;

    private readonly SpikeOptions _options;
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly List<MetricsRecord> _metrics = [];
    private readonly List<string> _failures = [];
    private DispatcherQueueController? _dispatcher;
    private Windows.System.DispatcherQueueController? _systemDispatcher;
    private Compositor? _compositor;
    private ContainerVisual? _root;
    private SpriteVisual? _contentVisual;
    private CompositionSurfaceBrush? _brush;
    private InsetClip? _clip;
    private ContentIsland? _island;
    private DesktopAttachedSiteBridge? _siteBridge;
    private InputPointerSource? _pointerSource;
    private InputKeyboardSource? _keyboardSource;
    private InputFocusController? _focusController;
    private D3DCompositionGrid? _grid;
    private AppWindow? _appWindow;
    private nint _winRtHwnd;
    private nint _bareHwnd;
    private long _targetGeneration;
    private long _presentedGeneration;
    private int _presentCount;
    private int _coalescedRenderCount;
    private int _canceledTerminalCount;
    private int _stalePresentCount;
    private int _metricsReversalCount;
    private int _surfaceRecreateCount;
    private int _islandReconnectCount;
    private int _closeDuringResizeCount;
    private int _rawPointerMessageCount;
    private int _rawKeyboardMessageCount;
    private bool _firstExactPresented;
    private int _firstShowBeforeExactCount;
    private bool _disposed;

    internal WinRtIslandSpike(SpikeOptions options)
    {
        _options = options;
    }

    internal int Run()
    {
        Native.SetProcessDpiAwarenessContext(new nint(-4));
        RegisterWindowClass();
        _dispatcher = DispatcherQueueController.CreateOnCurrentThread();
        _systemDispatcher = Windows.System.DispatcherQueueController.CreateOnDedicatedThread();
        CreateWindows();
        InitializeContentIsland();
        RenderLatest();
        ShowAfterFirstExact();
        Console.WriteLine($"winrtHwnd=0x{_winRtHwnd:X} bareHwnd=0x{_bareHwnd:X}");
        Console.WriteLine("WinRT ContentIsland is on the left; the bare GDI standard window is on the right.");

        if (_options.Automated)
            RunAutomatedScenario();
        else
            RunInteractiveLoop();

        var report = BuildReport();
        Directory.CreateDirectory(Path.GetDirectoryName(_options.ReportPath)!);
        File.WriteAllText(_options.ReportPath, JsonSerializer.Serialize(report, Program.JsonOptions));
        Console.WriteLine(
            $"W1 contract={report.ContractStatus} visible={report.VisibleStatus} " +
            $"presents={_presentCount} targets={_targetGeneration} failures={report.Failures.Count}");
        Console.WriteLine($"report={_options.ReportPath}");
        return report.ContractStatus == "PASS" && report.VisibleStatus == "PASS" ? 0 : 2;
    }

    private void CreateWindows()
    {
        var work = Native.GetPrimaryWorkArea();
        var windowWidth = Math.Min(InitialWidth, Math.Max(480, (work.Width - 60) / 2));
        var windowHeight = Math.Min(InitialHeight, Math.Max(360, work.Height - 120));
        _winRtHwnd = Native.CreateWindow(
            "DorotiWinRtContentIslandSpike", "Doroti W1 - WinRT ContentIsland",
            work.Left + 20, work.Top + 50, windowWidth, windowHeight);
        Instances[_winRtHwnd] = this;
        _bareHwnd = Native.CreateWindow(
            "DorotiWinRtContentIslandSpike", "Doroti W1 - Bare GDI control",
            work.Left + 40 + windowWidth, work.Top + 50, windowWidth, windowHeight);
        Instances[_bareHwnd] = this;
    }

    private void InitializeContentIsland()
    {
        using var ready = new ManualResetEventSlim();
        Exception? failure = null;
        if (!_systemDispatcher!.DispatcherQueue.TryEnqueue(() =>
            {
                try { _compositor = new Compositor(); }
                catch (Exception exception) { failure = exception; }
                finally { ready.Set(); }
            }))
            throw new InvalidOperationException("System compositor dispatch was rejected.");
        if (!ready.Wait(TimeSpan.FromSeconds(5)))
            throw new TimeoutException("System compositor creation timed out.");
        if (failure is not null)
            throw new InvalidOperationException("System compositor creation failed.", failure);

        _root = _compositor!.CreateContainerVisual();
        _contentVisual = _compositor.CreateSpriteVisual();
        _brush = _compositor.CreateSurfaceBrush();
        _brush.Stretch = CompositionStretch.None;
        _brush.HorizontalAlignmentRatio = 0;
        _brush.VerticalAlignmentRatio = 0;
        _contentVisual.Brush = _brush;
        _clip = _compositor.CreateInsetClip();
        _root.Clip = _clip;
        _root.Children.InsertAtTop(_contentVisual);

        _island = ContentIsland.CreateForSystemVisual(_dispatcher!.DispatcherQueue, _root);
        _island.StateChanged += (_, args) =>
        {
            if (args.DidActualSizeChange || args.DidRasterizationScaleChange)
                QueueRender();
        };
        var windowId = Win32Interop.GetWindowIdFromWindow(_winRtHwnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);
        _appWindow.AssociateWithDispatcherQueue(_dispatcher.DispatcherQueue);
        _siteBridge = DesktopAttachedSiteBridge.CreateFromWindowId(_dispatcher.DispatcherQueue, windowId);
        _siteBridge.ProcessesPointerInput = false;
        _siteBridge.Connect(_island);
        _pointerSource = InputPointerSource.GetForIsland(_island);
        _keyboardSource = InputKeyboardSource.GetForIsland(_island);
        _focusController = InputFocusController.GetForIsland(_island);
        _grid = new D3DCompositionGrid(_compositor);
        _brush.Surface = _grid.Surface;
    }

    private void RenderLatest()
    {
        if (_disposed || _grid is null || _siteBridge is null || _contentVisual is null || _brush is null || _root is null)
            return;
        var generation = _targetGeneration == 0 ? ++_targetGeneration : _targetGeneration;
        var siteView = _siteBridge.SiteView;
        var client = siteView.ClientSize;
        var width = Math.Max(1, client.Width);
        var height = Math.Max(1, client.Height);
        var scale = siteView.RasterizationScale > 0 ? siteView.RasterizationScale : 1f;
        var actual = siteView.ActualSize;
        if (actual.X <= 0 || actual.Y <= 0)
            actual = new Vector2(width / scale, height / scale);

        if (_presentedGeneration == generation)
        {
            _coalescedRenderCount++;
            return;
        }
        if (_presentedGeneration > generation)
        {
            _metricsReversalCount++;
            return;
        }

        _grid.Render(width, height, generation);
        if (generation != _targetGeneration)
        {
            _stalePresentCount++;
            return;
        }
        _contentVisual.Size = actual;
        _root.Size = actual;
        _brush.Scale = new Vector2(1f / scale, 1f / scale);
        _presentedGeneration = generation;
        _presentCount++;
        _firstExactPresented = true;
        _metrics.Add(new MetricsRecord(
            generation, width, height, actual.X, actual.Y, scale, _clock.Elapsed.TotalMilliseconds));
    }

    private void ShowAfterFirstExact()
    {
        if (!_firstExactPresented) _firstShowBeforeExactCount++;
        _appWindow!.Show();
        Native.ShowWindow(_bareHwnd, 5);
        Native.UpdateWindow(_bareHwnd);
    }

    private void RunAutomatedScenario()
    {
        Console.Error.WriteLine("w1-stage=automated-resize-start");
        var sizes = new (int Width, int Height)[]
        {
            (700, 520), (1100, 520), (620, 520), (980, 760),
            (540, 380), (900, 640), (680, 440), (1040, 700),
        };
        for (var cycle = 0; cycle < 2; cycle++)
        {
            foreach (var size in sizes)
            {
                Console.Error.WriteLine($"w1-stage=resize size={size.Width}x{size.Height}");
                Native.SetWindowPos(_winRtHwnd, 0, 0, 0, size.Width, size.Height, 0x0006);
                PumpFor(TimeSpan.FromMilliseconds(35));
            }
        }

        Console.Error.WriteLine("w1-stage=surface-recreate");
        RecreateSurface();
        PumpFor(TimeSpan.FromMilliseconds(100));
        Console.Error.WriteLine("w1-stage=island-reconnect");
        ReconnectIsland();
        PumpFor(TimeSpan.FromMilliseconds(100));
        if (_options.HoldMilliseconds > 0)
            PumpFor(TimeSpan.FromMilliseconds(_options.HoldMilliseconds));
        Console.Error.WriteLine("w1-stage=close");
        Native.SetWindowPos(_winRtHwnd, 0, 0, 0, 760, 560, 0x0006);
        if (_presentedGeneration < _targetGeneration) _canceledTerminalCount++;
        _closeDuringResizeCount++;
        Native.DestroyWindow(_winRtHwnd);
        PumpFor(TimeSpan.FromMilliseconds(50));
    }

    private void RecreateSurface()
    {
        _brush!.Surface = null;
        _grid!.Dispose();
        _grid = new D3DCompositionGrid(_compositor!);
        _brush.Surface = _grid.Surface;
        _surfaceRecreateCount++;
        _targetGeneration++;
        RenderLatest();
    }

    private void ReconnectIsland()
    {
        _siteBridge!.Dispose();
        _island!.Dispose();
        _brush!.Dispose();
        _contentVisual!.Dispose();
        _root!.Dispose();
        _root = _compositor!.CreateContainerVisual();
        _contentVisual = _compositor.CreateSpriteVisual();
        _brush = _compositor.CreateSurfaceBrush();
        _brush.Stretch = CompositionStretch.None;
        _brush.HorizontalAlignmentRatio = 0;
        _brush.VerticalAlignmentRatio = 0;
        _brush.Surface = _grid!.Surface;
        _contentVisual.Brush = _brush;
        _clip = _compositor.CreateInsetClip();
        _root.Clip = _clip;
        _root.Children.InsertAtTop(_contentVisual);
        _island = ContentIsland.CreateForSystemVisual(_dispatcher!.DispatcherQueue, _root);
        _island.StateChanged += (_, args) =>
        {
            if (args.DidActualSizeChange || args.DidRasterizationScaleChange)
                QueueRender();
        };
        var windowId = Win32Interop.GetWindowIdFromWindow(_winRtHwnd);
        _siteBridge = DesktopAttachedSiteBridge.CreateFromWindowId(_dispatcher.DispatcherQueue, windowId);
        _siteBridge.ProcessesPointerInput = false;
        _siteBridge.Connect(_island);
        _pointerSource = InputPointerSource.GetForIsland(_island);
        _keyboardSource = InputKeyboardSource.GetForIsland(_island);
        _focusController = InputFocusController.GetForIsland(_island);
        _islandReconnectCount++;
        _targetGeneration++;
        RenderLatest();
    }

    private void RunInteractiveLoop()
    {
        while (Native.GetMessage(out var message, 0, 0, 0) > 0)
        {
            Native.TranslateMessage(in message);
            Native.DispatchMessage(in message);
        }
    }

    private static void PumpFor(TimeSpan duration)
    {
        var until = Stopwatch.GetTimestamp() + (long)(duration.TotalSeconds * Stopwatch.Frequency);
        while (Stopwatch.GetTimestamp() < until)
        {
            while (Native.PeekMessage(out var message, 0, 0, 0, 1))
            {
                Native.TranslateMessage(in message);
                Native.DispatchMessage(in message);
            }
            Thread.Sleep(1);
        }
    }

    private void QueueRender()
    {
        if (_winRtHwnd != 0) Native.PostMessage(_winRtHwnd, WmAppRender, 0, 0);
    }

    private W1Report BuildReport()
    {
        var childCount = Native.CountChildWindows(_winRtHwnd);
        var pointerOwner = _siteBridge?.ProcessesPointerInput == true ? "ContentIsland" : "raw-hwnd";
        var keyboardOwner = _siteBridge?.ProcessesKeyboardInput == true ? "ContentIsland" : "raw-hwnd";
        if (pointerOwner != "ContentIsland")
            _failures.Add("DesktopAttachedSiteBridge pointer ownership is raw-hwnd; W1 requires ContentIsland primary.");
        if (childCount != 0) _failures.Add($"Expected zero child HWNDs, found {childCount}.");
        if (_stalePresentCount != 0) _failures.Add($"Stale presents: {_stalePresentCount}.");
        if (_firstShowBeforeExactCount != 0) _failures.Add("Window was shown before the first exact frame.");
        var missingTerminalCount = checked((int)(_targetGeneration - _presentCount - _canceledTerminalCount));
        if (missingTerminalCount != 0) _failures.Add($"Missing terminals: {missingTerminalCount}.");
        return new W1Report(
            "doroti.winrt-content-island-spike/v1",
            _failures.Count == 0 ? "PASS" : "FAIL",
            "notVerified",
            _options.Automated ? "automated" : "interactive",
            new W1Topology(childCount, 1, 1, 1, _surfaceRecreateCount, _islandReconnectCount,
                _closeDuringResizeCount),
            new W1Input(pointerOwner, keyboardOwner, _pointerSource is not null, _keyboardSource is not null,
                _focusController is not null, _rawPointerMessageCount, _rawKeyboardMessageCount),
            new W1Counters(_targetGeneration, _presentedGeneration, _presentCount, 0,
                _coalescedRenderCount, _canceledTerminalCount, missingTerminalCount, _stalePresentCount,
                _metricsReversalCount, _firstShowBeforeExactCount),
            _metrics,
            _failures,
            new[]
            {
                "Left fast/medium/slow expand/shrink/reverse: notVerified",
                "Top fast/medium/slow expand/shrink/reverse: notVerified",
                "Right fast/medium/slow expand/shrink/reverse: notVerified",
                "Bottom fast/medium/slow expand/shrink/reverse: notVerified",
                "external 240fps provenance: notVerified",
            });
    }

    private static nint WndProc(nint hwnd, uint message, nuint wParam, nint lParam)
    {
        if (!Instances.TryGetValue(hwnd, out var spike))
            return Native.DefWindowProc(hwnd, message, wParam, lParam);
        if (message is >= 0x0200 and <= 0x020E) spike._rawPointerMessageCount++;
        if (message is >= 0x0100 and <= 0x0109) spike._rawKeyboardMessageCount++;
        switch (message)
        {
            case 0x0005 when hwnd == spike._winRtHwnd:
                spike._targetGeneration++;
                spike.QueueRender();
                return 0;
            case WmAppRender when hwnd == spike._winRtHwnd:
                spike.RenderLatest();
                return 0;
            case 0x000F when hwnd == spike._bareHwnd:
                Native.PaintBareGrid(hwnd);
                return 0;
            case 0x0010:
                if (hwnd == spike._winRtHwnd && spike._bareHwnd != 0)
                    Native.DestroyWindow(spike._bareHwnd);
                Native.DestroyWindow(hwnd);
                return 0;
            case 0x0002 when hwnd == spike._winRtHwnd:
                Native.PostQuitMessage(0);
                return 0;
        }
        return Native.DefWindowProc(hwnd, message, wParam, lParam);
    }

    private static void RegisterWindowClass()
    {
        Native.RegisterWindowClass("DorotiWinRtContentIslandSpike", WindowProcedure);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_winRtHwnd != 0 && Native.IsWindow(_winRtHwnd)) Native.DestroyWindow(_winRtHwnd);
        if (_bareHwnd != 0 && Native.IsWindow(_bareHwnd)) Native.DestroyWindow(_bareHwnd);
        Instances.Remove(_winRtHwnd);
        Instances.Remove(_bareHwnd);
        _grid?.Dispose();
        _brush?.Dispose();
        _contentVisual?.Dispose();
        _root?.Dispose();
        _siteBridge?.Dispose();
        _island?.Dispose();
        _compositor?.Dispose();
        _systemDispatcher?.ShutdownQueueAsync();
        _dispatcher?.ShutdownQueue();
    }
}

internal sealed class D3DCompositionGrid : IDisposable
{
    private static readonly Guid CompositorInteropIid = new("25297D5C-3AD4-4C9C-B5CF-E36A38512330");
    private readonly ID3D11Device _device;
    private readonly ID3D11DeviceContext _context;
    private readonly IDXGIFactory2 _factory;
    private IDXGISwapChain1 _swapChain;
    private int _width;
    private int _height;

    internal D3DCompositionGrid(Compositor compositor)
    {
        D3D11CreateDevice(
            null,
            DriverType.Hardware,
            DeviceCreationFlags.BgraSupport,
            [FeatureLevel.Level_11_1, FeatureLevel.Level_11_0],
            out _device,
            out _,
            out _context).CheckError();
        _factory = CreateDXGIFactory2<IDXGIFactory2>(false);
        _swapChain = CreateSwapChain(1, 1);
        Surface = CreateCompositionSurface(compositor, _swapChain);
    }

    internal ICompositionSurface Surface { get; }

    internal void Render(int width, int height, long generation)
    {
        width = Math.Max(1, width);
        height = Math.Max(1, height);
        if (_width != width || _height != height)
        {
            _context.ClearState();
            _swapChain.ResizeBuffers(2, (uint)width, (uint)height, Format.B8G8R8A8_UNorm, SwapChainFlags.None)
                .CheckError();
            _width = width;
            _height = height;
        }

        using var buffer = _swapChain.GetBuffer<ID3D11Texture2D>(0);
        var pixels = new uint[checked(width * height)];
        Array.Fill(pixels, 0xFF170E09u);
        for (var y = 0; y < height; y++)
        {
            var row = y * width;
            for (var x = 0; x < width; x++)
            {
                if (x % 32 < 2 || y % 32 < 2) pixels[row + x] = 0xFFD99E1Fu;
                if (x >= width - 9) pixels[row + x] = 0xFF8C1FFFu;
                if (y >= height - 9) pixels[row + x] = 0xFF14D1FFu;
                if (x < 22 && y < 22) pixels[row + x] = 0xFF9EFF1Au;
            }
        }
        var markerWidth = Math.Min(60, width);
        var markerHeight = Math.Min(12, height);
        var offset = (int)(generation % Math.Max(1, width - markerWidth + 1));
        for (var y = Math.Max(0, height / 2 - 6); y < Math.Min(height, height / 2 - 6 + markerHeight); y++)
            Array.Fill(pixels, 0xFFFF59B8u, y * width + offset, markerWidth);
        _context.UpdateSubresource(pixels, buffer, 0, (uint)(width * sizeof(uint)), 0, null);
        _context.Flush();
        _swapChain.Present(0, PresentFlags.None).CheckError();
    }

    private IDXGISwapChain1 CreateSwapChain(int width, int height)
    {
        var description = new SwapChainDescription1(
            (uint)width,
            (uint)height,
            Format.B8G8R8A8_UNorm,
            false,
            Usage.RenderTargetOutput,
            2,
            Scaling.Stretch,
            SwapEffect.FlipSequential,
            AlphaMode.Premultiplied,
            SwapChainFlags.None);
        return _factory.CreateSwapChainForComposition(_device, description, null);
    }

    private static unsafe ICompositionSurface CreateCompositionSurface(Compositor compositor, IDXGISwapChain1 swapChain)
    {
        using var interop = ((WinRT.IWinRTObject)compositor).NativeObject.As(CompositorInteropIid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var create = (delegate* unmanaged[Stdcall]<nint, nint, nint*, int>)vtable[4];
        nint result = 0;
        var hresult = create(thisPointer, swapChain.NativePointer, &result);
        if (hresult < 0) Marshal.ThrowExceptionForHR(hresult);
        try { return WinRT.MarshalInterface<ICompositionSurface>.FromAbi(result); }
        finally { Marshal.Release(result); }
    }

    public void Dispose()
    {
        if (Surface is IDisposable disposable) disposable.Dispose();
        _swapChain.Dispose();
        _factory.Dispose();
        _context.Dispose();
        _device.Dispose();
    }
}

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct NativeRect(int Left, int Top, int Right, int Bottom);

internal sealed record MetricsRecord(
    long Generation,
    int ClientWidth,
    int ClientHeight,
    float ActualWidth,
    float ActualHeight,
    float RasterizationScale,
    double TimestampMilliseconds);

internal sealed record W1Topology(
    int ChildHwndCount,
    int ConnectedIslandCount,
    int SiteBridgeCount,
    int VisibleSurfaceCount,
    int SurfaceRecreateCount,
    int IslandReconnectCount,
    int CloseDuringResizeCount);

internal sealed record W1Input(
    string PointerOwner,
    string KeyboardOwner,
    bool PointerSourceAvailable,
    bool KeyboardSourceAvailable,
    bool FocusControllerAvailable,
    int RawPointerMessageCount,
    int RawKeyboardMessageCount);

internal sealed record W1Counters(
    long TargetGeneration,
    long PresentedGeneration,
    int PresentCount,
    int DuplicateTerminalCount,
    int CoalescedRenderCount,
    int CanceledTerminalCount,
    int MissingTerminalCount,
    int StalePresentCount,
    int MetricsReversalCount,
    int FirstShowBeforeExactCount);

internal sealed record W1Report(
    string Schema,
    string ContractStatus,
    string VisibleStatus,
    string Mode,
    W1Topology Topology,
    W1Input Input,
    W1Counters Counters,
    IReadOnlyList<MetricsRecord> Metrics,
    IReadOnlyList<string> Failures,
    IReadOnlyList<string> PhysicalMatrix);

internal static partial class Native
{
    private const uint WsOverlappedWindow = 0x00CF0000;
    private const uint CsHRedraw = 0x0002;
    private const uint CsVRedraw = 0x0001;
    private static readonly Dictionary<string, ushort> Classes = [];
    private static readonly EnumChildProc EnumChildProcedure = (_, _) =>
    {
        _childCount++;
        return true;
    };
    private static int _childCount;

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate nint WndProc(nint hwnd, uint message, nuint wParam, nint lParam);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private delegate bool EnumChildProc(nint hwnd, nint parameter);

    internal static void RegisterWindowClass(string name, WndProc procedure)
    {
        if (Classes.ContainsKey(name)) return;
        var windowClass = new WindowClass
        {
            Size = (uint)Marshal.SizeOf<WindowClass>(),
            Style = CsHRedraw | CsVRedraw,
            WindowProcedure = Marshal.GetFunctionPointerForDelegate(procedure),
            Instance = GetModuleHandle(null),
            Cursor = LoadCursor(0, new nint(32512)),
            Background = new nint(6),
            ClassName = name,
        };
        var atom = RegisterClassEx(in windowClass);
        if (atom == 0) throw new InvalidOperationException($"RegisterClassExW failed: {Marshal.GetLastWin32Error()}");
        Classes.Add(name, atom);
    }

    internal static nint CreateWindow(string className, string title, int x, int y, int width, int height)
    {
        var hwnd = CreateWindowEx(0, className, title, WsOverlappedWindow, x, y, width, height, 0, 0,
            GetModuleHandle(null), 0);
        if (hwnd == 0) throw new InvalidOperationException($"CreateWindowExW failed: {Marshal.GetLastWin32Error()}");
        return hwnd;
    }

    internal static WorkArea GetPrimaryWorkArea()
    {
        var area = new NativeRect();
        if (!SystemParametersInfo(0x0030, 0, ref area, 0))
            throw new InvalidOperationException($"SystemParametersInfoW failed: {Marshal.GetLastWin32Error()}");
        return new WorkArea(area.Left, area.Top, area.Right, area.Bottom);
    }

    internal static int CountChildWindows(nint hwnd)
    {
        _childCount = 0;
        EnumChildWindows(hwnd, EnumChildProcedure, 0);
        return _childCount;
    }

    internal static void PaintBareGrid(nint hwnd)
    {
        var hdc = BeginPaint(hwnd, out var paint);
        try
        {
            GetClientRect(hwnd, out var rect);
            Fill(hdc, rect, 0x00170E09);
            for (var x = 0; x < rect.Right; x += 32)
                Fill(hdc, new NativeRect(x, 0, Math.Min(x + 2, rect.Right), rect.Bottom), 0x00D99E1F);
            for (var y = 0; y < rect.Bottom; y += 32)
                Fill(hdc, new NativeRect(0, y, rect.Right, Math.Min(y + 2, rect.Bottom)), 0x00D99E1F);
            Fill(hdc, new NativeRect(Math.Max(0, rect.Right - 9), 0, rect.Right, rect.Bottom), 0x008C1FFF);
            Fill(hdc, new NativeRect(0, Math.Max(0, rect.Bottom - 9), rect.Right, rect.Bottom), 0x0014D1FF);
            Fill(hdc, new NativeRect(0, 0, Math.Min(22, rect.Right), Math.Min(22, rect.Bottom)), 0x009EFF1A);
        }
        finally
        {
            EndPaint(hwnd, in paint);
        }
    }

    private static void Fill(nint hdc, NativeRect rect, uint color)
    {
        var brush = CreateSolidBrush(color);
        try { FillRect(hdc, in rect, brush); }
        finally { DeleteObject(brush); }
    }

    internal sealed record WorkArea(int Left, int Top, int Right, int Bottom)
    {
        internal int Width => Right - Left;
        internal int Height => Bottom - Top;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WindowClass
    {
        internal uint Size;
        internal uint Style;
        internal nint WindowProcedure;
        internal int ClassExtra;
        internal int WindowExtra;
        internal nint Instance;
        internal nint Icon;
        internal nint Cursor;
        internal nint Background;
        internal string? MenuName;
        internal string ClassName;
        internal nint IconSmall;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Message
    {
        internal nint Hwnd;
        internal uint Value;
        internal nuint WParam;
        internal nint LParam;
        internal uint Time;
        internal int PointX;
        internal int PointY;
        internal uint Private;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PaintStruct
    {
        internal nint DeviceContext;
        [MarshalAs(UnmanagedType.Bool)] internal bool Erase;
        internal NativeRect PaintRect;
        [MarshalAs(UnmanagedType.Bool)] internal bool Restore;
        [MarshalAs(UnmanagedType.Bool)] internal bool IncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] internal byte[] Reserved;
    }

    [DllImport("user32.dll", EntryPoint = "RegisterClassExW", SetLastError = true)]
    private static extern ushort RegisterClassEx(in WindowClass windowClass);

    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint CreateWindowEx(uint extendedStyle, string className, string title, uint style,
        int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);

    [LibraryImport("user32.dll", EntryPoint = "DefWindowProcW")]
    internal static partial nint DefWindowProc(nint hwnd, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll", EntryPoint = "GetMessageW")]
    internal static partial int GetMessage(out Message message, nint hwnd, uint min, uint max);

    [LibraryImport("user32.dll", EntryPoint = "PeekMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PeekMessage(out Message message, nint hwnd, uint min, uint max, uint remove);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool TranslateMessage(in Message message);

    [LibraryImport("user32.dll", EntryPoint = "DispatchMessageW")]
    internal static partial nint DispatchMessage(in Message message);

    [LibraryImport("user32.dll", EntryPoint = "PostMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PostMessage(nint hwnd, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    internal static partial void PostQuitMessage(int exitCode);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShowWindow(nint hwnd, int command);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UpdateWindow(nint hwnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowPos(nint hwnd, nint insertAfter, int x, int y, int width, int height, uint flags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyWindow(nint hwnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool IsWindow(nint hwnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool EnumChildWindows(nint hwnd, EnumChildProc callback, nint parameter);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetClientRect(nint hwnd, out NativeRect rect);

    [LibraryImport("user32.dll", EntryPoint = "SystemParametersInfoW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SystemParametersInfo(uint action, uint parameter, ref NativeRect value, uint flags);

    [LibraryImport("user32.dll")]
    internal static partial nint SetProcessDpiAwarenessContext(nint value);

    [LibraryImport("user32.dll", EntryPoint = "LoadCursorW")]
    private static partial nint LoadCursor(nint instance, nint cursorName);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandle(string? moduleName);

    [DllImport("user32.dll")]
    private static extern nint BeginPaint(nint hwnd, out PaintStruct paint);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EndPaint(nint hwnd, in PaintStruct paint);

    [LibraryImport("user32.dll")]
    private static partial int FillRect(nint hdc, in NativeRect rect, nint brush);

    [LibraryImport("gdi32.dll")]
    private static partial nint CreateSolidBrush(uint color);

    [LibraryImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DeleteObject(nint value);

    [LibraryImport("combase.dll")]
    internal static partial int RoInitialize(uint initType);

    [LibraryImport("combase.dll")]
    internal static partial void RoUninitialize();
}
