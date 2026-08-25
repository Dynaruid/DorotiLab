using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
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
        Console.Error.WriteLine("w1r-stage=process-entry");
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
            schema = "doroti.winrt-content-island-w1r/v3",
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
    private WinRtTopLevelNativeIngress? _nativeIngress;
    private readonly WinRtNativeMessageTrace _nativeMessageTrace = new();
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
    private int _enableMouseInPointerCallCount;
    private bool _mouseInPointerEnabled;
    private int _platformChildHwndCount;
    private IReadOnlyList<string> _platformChildWindowClasses = Array.Empty<string>();
    private bool _liveTopologyCaptured;
    private bool _firstExactPresented;
    private int _firstShowBeforeExactCount;
    private bool _disposed;

    internal WinRtIslandSpike(SpikeOptions options)
    {
        _options = options;
    }

    internal int Run()
    {
        Console.Error.WriteLine("w1r-stage=run-entry");
        Native.SetProcessDpiAwarenessContext(new nint(-4));
        RegisterWindowClass();
        Console.Error.WriteLine("w1r-stage=dispatchers");
        _dispatcher = DispatcherQueueController.CreateOnCurrentThread();
        _systemDispatcher = Windows.System.DispatcherQueueController.CreateOnDedicatedThread();
        CreateWindows();
        Console.Error.WriteLine("w1r-stage=windows-created");
        InitializeContentIsland();
        Console.Error.WriteLine("w1r-stage=island-created");
        Console.Error.WriteLine("w1r-stage=enable-mouse-in-pointer");
        _enableMouseInPointerCallCount++;
        _mouseInPointerEnabled = Native.EnableMouseInPointer(true);
        if (!_mouseInPointerEnabled)
            throw new InvalidOperationException(
                $"EnableMouseInPointer(TRUE) failed: {Marshal.GetLastWin32Error()}");
        _nativeIngress = new WinRtTopLevelNativeIngress(_winRtHwnd, _mouseInPointerEnabled);
        RenderLatest();
        ShowAfterFirstExact();
        Console.WriteLine($"winrtHwnd=0x{_winRtHwnd:X} bareHwnd=0x{_bareHwnd:X}");
        Console.WriteLine("W1R WinRT ContentIsland is on the left; the bare GDI standard window is on the right.");

        if (_options.Automated)
            RunAutomatedScenario();
        else
            RunInteractiveLoop();

        var report = BuildReport();
        Directory.CreateDirectory(Path.GetDirectoryName(_options.ReportPath)!);
        File.WriteAllText(_options.ReportPath, JsonSerializer.Serialize(report, Program.JsonOptions));
        Console.WriteLine(
            $"W1R contract={report.ContractStatus} visible={report.VisibleStatus} " +
            $"presents={_presentCount} targets={_targetGeneration} failures={report.Failures.Count}");
        Console.WriteLine($"report={_options.ReportPath}");
        return report.ContractStatus == "PASS" ? 0 : 2;
    }

    private void CreateWindows()
    {
        var work = Native.GetPrimaryWorkArea();
        var windowWidth = Math.Min(InitialWidth, Math.Max(480, (work.Width - 60) / 2));
        var windowHeight = Math.Min(InitialHeight, Math.Max(360, work.Height - 120));
        _winRtHwnd = Native.CreateWindow(
            "DorotiWinRtContentIslandSpike", "Doroti W1R - WinRT ContentIsland",
            work.Left + 20, work.Top + 50, windowWidth, windowHeight);
        Instances[_winRtHwnd] = this;
        _bareHwnd = Native.CreateWindow(
            "DorotiWinRtContentIslandSpike", "Doroti W1R - Bare GDI control",
            work.Left + 40 + windowWidth, work.Top + 50, windowWidth, windowHeight);
        Instances[_bareHwnd] = this;
    }

    private void InitializeContentIsland()
    {
        Console.Error.WriteLine("w1r-stage=compositor-create");
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

        Console.Error.WriteLine("w1r-stage=island-create");
        _island = ContentIsland.CreateForSystemVisual(_dispatcher!.DispatcherQueue, _root);
        _island.StateChanged += (_, args) =>
        {
            if (args.DidActualSizeChange || args.DidRasterizationScaleChange)
                QueueRender();
        };
        var windowId = Win32Interop.GetWindowIdFromWindow(_winRtHwnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);
        _appWindow.AssociateWithDispatcherQueue(_dispatcher.DispatcherQueue);
        Console.Error.WriteLine("w1r-stage=bridge-create");
        _siteBridge = DesktopAttachedSiteBridge.CreateFromWindowId(_dispatcher.DispatcherQueue, windowId);
        // Windows App SDK 2.4 rejects disabling bridge keyboard processing after connect and
        // fail-fasts when it is disabled before connect. Keep the runtime floor explicit while
        // registering no root island keyboard source; WndProc remains the sole Doroti packet producer.
        _siteBridge.ProcessesKeyboardInput = true;
        _siteBridge.ProcessesPointerInput = false;
        Console.Error.WriteLine("w1r-stage=bridge-keyboard-runtime-floor-pointer-false");
        _siteBridge.Connect(_island);
        Console.Error.WriteLine("w1r-stage=bridge-connected");
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

        Console.Error.WriteLine("w1r-stage=native-ingress-fixture");
        _nativeIngress!.RunAutomatedFixture();

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
        CaptureLiveTopology();
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
        _siteBridge.ProcessesKeyboardInput = true;
        _siteBridge.ProcessesPointerInput = false;
        _siteBridge.Connect(_island);
        _islandReconnectCount++;
        _targetGeneration++;
        RenderLatest();
    }

    private void RunInteractiveLoop()
    {
        while (Native.GetMessage(out var message, 0, 0, 0) > 0)
            Native.DispatchTranslatedMessage(in message);
    }

    private static void PumpFor(TimeSpan duration)
    {
        var until = Stopwatch.GetTimestamp() + (long)(duration.TotalSeconds * Stopwatch.Frequency);
        while (Stopwatch.GetTimestamp() < until)
        {
            while (Native.PeekMessage(out var message, 0, 0, 0, 1))
                Native.DispatchTranslatedMessage(in message);
            Thread.Sleep(1);
        }
    }

    private void QueueRender()
    {
        if (_winRtHwnd != 0) Native.PostMessage(_winRtHwnd, WmAppRender, 0, 0);
    }

    private W1Report BuildReport()
    {
        CaptureLiveTopology();
        if (_siteBridge?.ProcessesPointerInput != false)
            _failures.Add("DesktopAttachedSiteBridge must keep pointer processing disabled.");
        if (_siteBridge?.ProcessesKeyboardInput != true)
            _failures.Add("DesktopAttachedSiteBridge must keep the Windows App SDK 2.4 keyboard runtime floor enabled.");
        if (_platformChildHwndCount != 1 ||
            _platformChildWindowClasses.Any(value => value != "InputSiteWindowClass"))
        {
            _failures.Add(
                $"Expected one platform InputSiteWindowClass child, found " +
                $"{_platformChildHwndCount}: {string.Join(", ", _platformChildWindowClasses)}.");
        }
        if (_stalePresentCount != 0) _failures.Add($"Stale presents: {_stalePresentCount}.");
        if (_firstShowBeforeExactCount != 0) _failures.Add("Window was shown before the first exact frame.");
        var missingTerminalCount = checked((int)(_targetGeneration - _presentCount - _canceledTerminalCount));
        if (missingTerminalCount != 0) _failures.Add($"Missing terminals: {missingTerminalCount}.");
        var ingress = _nativeIngress?.BuildReport(
            _enableMouseInPointerCallCount,
            _siteBridge?.ProcessesPointerInput ?? true,
            _siteBridge?.ProcessesKeyboardInput ?? false,
            _nativeMessageTrace.KeyboardMessageCount,
            _nativeMessageTrace.ImeMessageCount) ??
            throw new InvalidOperationException("Native ingress was not initialized.");
        _failures.AddRange(ingress.Failures);
        return new W1Report(
            "doroti.winrt-content-island-w1r/v3",
            _failures.Count == 0 ? "PASS" : "FAIL",
            "notVerified",
            _options.Automated ? "automated" : "interactive",
            new W1Topology(0, _platformChildHwndCount, _platformChildWindowClasses,
                1, 1, 1, _surfaceRecreateCount, _islandReconnectCount,
                _closeDuringResizeCount),
            ingress,
            new W1Counters(_targetGeneration, _presentedGeneration, _presentCount, 0,
                _coalescedRenderCount, _canceledTerminalCount, missingTerminalCount, _stalePresentCount,
                _metricsReversalCount, _firstShowBeforeExactCount),
            _metrics,
            _failures.Distinct(StringComparer.Ordinal).ToArray(),
            new[]
            {
                "Left fast/medium/slow expand/shrink/reverse: notVerified",
                "Top fast/medium/slow expand/shrink/reverse: notVerified",
                "Right fast/medium/slow expand/shrink/reverse: notVerified",
                "Bottom fast/medium/slow expand/shrink/reverse: notVerified",
                "external 240fps provenance: notVerified",
            });
    }

    private void CaptureLiveTopology()
    {
        if (_liveTopologyCaptured || _winRtHwnd == 0 || !Native.IsWindow(_winRtHwnd)) return;
        _platformChildWindowClasses = Native.GetChildWindowClassNames(_winRtHwnd);
        _platformChildHwndCount = _platformChildWindowClasses.Count;
        _liveTopologyCaptured = true;
    }

    private static nint WndProc(nint hwnd, uint message, nuint wParam, nint lParam)
    {
        if (!Instances.TryGetValue(hwnd, out var spike))
            return Native.DefWindowProc(hwnd, message, wParam, lParam);
        if (hwnd == spike._winRtHwnd && spike._nativeIngress is not null)
            spike._nativeMessageTrace.Observe(message);
        if (hwnd == spike._winRtHwnd && spike._nativeIngress is not null &&
            spike._nativeIngress.TryHandle(message, wParam, lParam, out var ingressResult))
            return ingressResult;
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
                if (hwnd == spike._winRtHwnd) spike.CaptureLiveTopology();
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

internal sealed class WinRtTopLevelNativeIngress
{
    private const uint WmSetFocus = 0x0007;
    private const uint WmKillFocus = 0x0008;
    private const uint WmSetCursor = 0x0020;
    private const uint WmGetObject = 0x003D;
    private const uint WmKeyDown = 0x0100;
    private const uint WmKeyUp = 0x0101;
    private const uint WmChar = 0x0102;
    private const uint WmDeadChar = 0x0103;
    private const uint WmSysKeyDown = 0x0104;
    private const uint WmSysKeyUp = 0x0105;
    private const uint WmSysChar = 0x0106;
    private const uint WmSysDeadChar = 0x0107;
    private const uint WmImeStartComposition = 0x010D;
    private const uint WmImeEndComposition = 0x010E;
    private const uint WmImeComposition = 0x010F;
    private const uint WmMouseFirst = 0x0200;
    private const uint WmMouseLast = 0x020E;
    private const uint WmPointerUpdate = 0x0245;
    private const uint WmPointerDown = 0x0246;
    private const uint WmPointerUp = 0x0247;
    private const uint WmPointerEnter = 0x0249;
    private const uint WmPointerLeave = 0x024A;
    private const uint WmPointerCaptureChanged = 0x024C;
    private const uint WmPointerWheel = 0x024E;
    private const uint WmPointerHWheel = 0x024F;
    private const int HtClient = 1;
    private const int HtLeft = 10;
    private const int UiaRootObjectId = -25;
    private const string Owner = "WinRtTopLevelNativeIngress";

    private readonly nint _hwnd;
    private readonly bool _mouseInPointerEnabled;
    private readonly WinRtAutomationRootProvider _automationRoot;
    private readonly HashSet<uint> _activePointers = [];
    private readonly HashSet<uint> _pressedPointers = [];
    private readonly HashSet<uint> _pressedKeys = [];
    private readonly List<string> _failures = [];
    private int _rawPointerMessageCount;
    private int _rawMouseCompatibilityMessageCount;
    private int _rawKeyboardMessageCount;
    private int _keyboardPacketCount;
    private int _duplicateKeyboardPacketCount = 0;
    private int _keyboardDefWindowProcDelegationCount = 0;
    private int _imeMessageCount;
    private int _imeDefWindowProcDelegationCount;
    private int _imeStartCount;
    private int _imeCompositionCount;
    private int _imeEndCount;
    private int _pointerPacketCount;
    private int _mouseCompatibilityPacketCount = 0;
    private int _duplicatePacketCount;
    private int _pointerHistorySampleCount;
    private int _pointerTerminalMissingCount;
    private int _keyTerminalMissingCount;
    private int _captureLostCancelCount;
    private int _focusTransitionCount;
    private int _clientCursorCount;
    private int _nonClientCursorDelegationCount;
    private int _uiaRootRequestCount;
    private int _uiaNonRootDelegationCount;
    private int _duplicateProviderRootCount = 0;
    private int _callbackWaitCount = 0;
    private bool _fixturePassed;
    private char? _pendingHighSurrogate;
    private int _deadCharacterCount;
    private int _surrogatePairCount;

    internal WinRtTopLevelNativeIngress(nint hwnd, bool mouseInPointerEnabled)
    {
        _hwnd = hwnd;
        _mouseInPointerEnabled = mouseInPointerEnabled;
        _automationRoot = new WinRtAutomationRootProvider(hwnd);
    }

    internal bool TryHandle(uint message, nuint wParam, nint lParam, out nint result)
    {
        var started = Stopwatch.GetTimestamp();
        result = 0;
        try
        {
            if (message is >= WmPointerUpdate and <= WmPointerHWheel && message != 0x0248 &&
                message != 0x024B && message != 0x024D)
            {
                HandlePointer(message, (uint)(wParam & 0xFFFF));
                return true;
            }

            if (message is >= WmMouseFirst and <= WmMouseLast)
            {
                _rawMouseCompatibilityMessageCount++;
                // EnableMouseInPointer makes WM_POINTER the sole Doroti packet path.
                // Compatibility mouse messages remain available to DefWindowProc only.
                return false;
            }

            if (message is WmKeyDown or WmKeyUp or WmChar or WmDeadChar or
                WmSysKeyDown or WmSysKeyUp or WmSysChar or WmSysDeadChar)
            {
                HandleKeyboard(message, (uint)wParam);
                return true;
            }

            if (message is WmImeStartComposition or WmImeComposition or WmImeEndComposition)
            {
                HandleIme(message);
                // The future WinRtTextInputAdapter will consume committed composition. Until
                // that adapter exists, preserve system IME behavior through DefWindowProc.
                _imeDefWindowProcDelegationCount++;
                return false;
            }

            if (message is WmSetFocus or WmKillFocus)
            {
                _focusTransitionCount++;
                return false;
            }

            if (message == WmSetCursor)
            {
                var hitTest = unchecked((short)((long)lParam & 0xFFFF));
                if (hitTest == HtClient)
                {
                    _clientCursorCount++;
                    Native.SetCursor(Native.LoadArrowCursor());
                    result = 1;
                    return true;
                }
                _nonClientCursorDelegationCount++;
                return false;
            }

            if (message == WmGetObject)
            {
                if ((int)lParam != UiaRootObjectId)
                {
                    _uiaNonRootDelegationCount++;
                    return false;
                }
                _uiaRootRequestCount++;
                result = Native.UiaReturnRawElementProvider(_hwnd, wParam, lParam, _automationRoot);
                return true;
            }

            return false;
        }
        finally
        {
            // This callback is deliberately enqueue/state-only. Any future synchronous wait
            // must increment the explicit counter and fail the W1R contract.
            _ = Stopwatch.GetTimestamp() - started;
        }
    }

    internal void RunAutomatedFixture()
    {
        AcceptPointer(41, PointerPhase.Add, 1);
        AcceptPointer(41, PointerPhase.Hover, 3);
        AcceptPointer(41, PointerPhase.Down, 1);
        Native.SetCapture(_hwnd);
        if (Native.GetCapture() != _hwnd)
            _failures.Add("Pointer fixture failed to acquire top-level HWND capture.");
        AcceptPointer(41, PointerPhase.Move, 4);
        AcceptPointer(41, PointerPhase.Cancel, 1);
        _captureLostCancelCount++;
        Native.ReleaseCapture();
        AcceptPointer(41, PointerPhase.Remove, 1);

        AcceptPointer(42, PointerPhase.Add, 1);
        AcceptPointer(42, PointerPhase.Down, 1);
        AcceptPointer(42, PointerPhase.Up, 1);
        AcceptPointer(42, PointerPhase.Remove, 1);

        Native.SendMessage(_hwnd, WmSetFocus, 0, 0);
        Native.SendMessage(_hwnd, WmKillFocus, 0, 0);
        Native.SendMessage(_hwnd, WmSetCursor, (nuint)_hwnd, HtClient);
        Native.SendMessage(_hwnd, WmSetCursor, (nuint)_hwnd, HtLeft);
        Native.SendMessage(_hwnd, WmKeyDown, 0x41, 1);
        Native.SendMessage(_hwnd, WmChar, 'a', 1);
        Native.SendMessage(_hwnd, WmKeyUp, 0x41, 1);
        Native.SendMessage(_hwnd, WmDeadChar, '^', 1);
        Native.SendMessage(_hwnd, WmChar, 0xD83D, 1);
        Native.SendMessage(_hwnd, WmChar, 0xDE00, 1);
        Native.SendMessage(_hwnd, WmSysKeyDown, 0x12, 1);
        Native.SendMessage(_hwnd, WmSysChar, 'x', 1);
        Native.SendMessage(_hwnd, WmSysKeyUp, 0x12, 1);
        Native.SendMessage(_hwnd, WmImeStartComposition, 0, 0);
        Native.SendMessage(_hwnd, WmImeComposition, 0, 0);
        Native.SendMessage(_hwnd, WmImeEndComposition, 0, 0);
        Native.SendMessage(_hwnd, WmGetObject, 0, unchecked((nint)UiaRootObjectId));
        Native.SendMessage(_hwnd, WmGetObject, 0, 1);

        _fixturePassed = _activePointers.Count == 0 && _pressedPointers.Count == 0 &&
            _pressedKeys.Count == 0 && _pointerHistorySampleCount >= 14 &&
            _deadCharacterCount == 1 && _surrogatePairCount == 1 &&
            _rawKeyboardMessageCount == 9 && _keyboardPacketCount == 9 &&
            _duplicateKeyboardPacketCount == 0 && _keyboardDefWindowProcDelegationCount == 0 &&
            _imeMessageCount == 3 && _imeStartCount == 1 && _imeCompositionCount == 1 &&
            _imeEndCount == 1 && _imeDefWindowProcDelegationCount == 3 &&
            _focusTransitionCount >= 2 && _clientCursorCount >= 1 &&
            _nonClientCursorDelegationCount >= 1 && _uiaRootRequestCount == 1 &&
            _uiaNonRootDelegationCount >= 1;
        if (!_fixturePassed)
            _failures.Add("Native-ingress automated lifecycle fixture did not reach all expected terminals.");
    }

    internal W1Input BuildReport(
        int enableMouseInPointerCallCount,
        bool bridgeProcessesPointerInput,
        bool bridgeProcessesKeyboardInput,
        int nativeTraceKeyboardMessageCount,
        int nativeTraceImeMessageCount)
    {
        _pointerTerminalMissingCount += _activePointers.Count + _pressedPointers.Count;
        _keyTerminalMissingCount += _pressedKeys.Count;
        if (!_mouseInPointerEnabled) _failures.Add("EnableMouseInPointer(TRUE) was not active.");
        if (enableMouseInPointerCallCount != 1)
            _failures.Add($"EnableMouseInPointer call count was {enableMouseInPointerCallCount}, expected 1.");
        if (_mouseCompatibilityPacketCount != 0)
            _failures.Add($"Compatibility mouse packets emitted: {_mouseCompatibilityPacketCount}.");
        if (_duplicatePacketCount != 0) _failures.Add($"Duplicate packets: {_duplicatePacketCount}.");
        if (_pointerTerminalMissingCount != 0)
            _failures.Add($"Missing pointer terminals: {_pointerTerminalMissingCount}.");
        if (_keyTerminalMissingCount != 0)
            _failures.Add($"Missing key terminals: {_keyTerminalMissingCount}.");
        if (_rawKeyboardMessageCount != _keyboardPacketCount)
            _failures.Add(
                $"Keyboard raw/packet mismatch: {_rawKeyboardMessageCount}/{_keyboardPacketCount}.");
        if (_duplicateKeyboardPacketCount != 0)
            _failures.Add($"Duplicate keyboard packets: {_duplicateKeyboardPacketCount}.");
        if (_keyboardDefWindowProcDelegationCount != 0)
            _failures.Add($"Doroti keyboard messages delegated to DefWindowProc: {_keyboardDefWindowProcDelegationCount}.");
        if (nativeTraceKeyboardMessageCount != _rawKeyboardMessageCount)
            _failures.Add(
                $"Native trace/keyboard ingress mismatch: {nativeTraceKeyboardMessageCount}/{_rawKeyboardMessageCount}.");
        if (nativeTraceImeMessageCount != _imeMessageCount)
            _failures.Add($"Native trace/IME ingress mismatch: {nativeTraceImeMessageCount}/{_imeMessageCount}.");
        if (_imeMessageCount != _imeDefWindowProcDelegationCount)
            _failures.Add(
                $"IME lifecycle delegation mismatch: {_imeMessageCount}/{_imeDefWindowProcDelegationCount}.");
        if (_duplicateProviderRootCount != 0)
            _failures.Add($"Duplicate UIA provider roots: {_duplicateProviderRootCount}.");
        if (_callbackWaitCount != 0)
            _failures.Add($"Native-ingress synchronous waits: {_callbackWaitCount}.");
        if (!_fixturePassed) _failures.Add("Native-ingress fixture was not completed.");
        if (bridgeProcessesPointerInput)
            _failures.Add("DesktopAttachedSiteBridge pointer processing is still enabled.");
        if (!bridgeProcessesKeyboardInput)
            _failures.Add("DesktopAttachedSiteBridge keyboard runtime floor is not enabled.");

        return new W1Input(
            Owner, Owner, Owner, Owner, Owner,
            "bridge-processing-enabled-without-root-input-source-registration",
            "top-level-wndproc-only-despite-bridge-runtime-floor",
            "WinRtTextInputAdapter-reserved",
            "notVerified",
            bridgeProcessesPointerInput, bridgeProcessesKeyboardInput, 0,
            _mouseInPointerEnabled, enableMouseInPointerCallCount,
            _rawPointerMessageCount, _rawMouseCompatibilityMessageCount, _rawKeyboardMessageCount,
            _keyboardPacketCount, _duplicateKeyboardPacketCount,
            _keyboardDefWindowProcDelegationCount, nativeTraceKeyboardMessageCount,
            _imeMessageCount, _imeDefWindowProcDelegationCount, nativeTraceImeMessageCount,
            _imeStartCount, _imeCompositionCount, _imeEndCount,
            _pointerPacketCount, _mouseCompatibilityPacketCount, _duplicatePacketCount,
            _pointerHistorySampleCount, _pointerTerminalMissingCount, _keyTerminalMissingCount,
            _captureLostCancelCount, _focusTransitionCount, _clientCursorCount,
            _nonClientCursorDelegationCount, _uiaRootRequestCount, _uiaNonRootDelegationCount,
            _duplicateProviderRootCount, _deadCharacterCount, _surrogatePairCount,
            _activePointers.Count, _pressedKeys.Count,
            _callbackWaitCount, _fixturePassed, _failures.Distinct(StringComparer.Ordinal).ToArray());
    }

    private void HandlePointer(uint message, uint pointerId)
    {
        _rawPointerMessageCount++;
        var historyCount = Native.GetPointerHistorySampleCount(pointerId);
        switch (message)
        {
            case WmPointerEnter:
                AcceptPointer(pointerId, PointerPhase.Add, historyCount);
                break;
            case WmPointerDown:
                if (!_activePointers.Contains(pointerId)) AcceptPointer(pointerId, PointerPhase.Add, 1);
                AcceptPointer(pointerId, PointerPhase.Down, historyCount);
                Native.SetCapture(_hwnd);
                break;
            case WmPointerUpdate:
                AcceptPointer(pointerId,
                    _pressedPointers.Contains(pointerId) ? PointerPhase.Move : PointerPhase.Hover,
                    historyCount);
                break;
            case WmPointerUp:
                AcceptPointer(pointerId, PointerPhase.Up, historyCount);
                Native.ReleaseCapture();
                break;
            case WmPointerCaptureChanged:
                if (_pressedPointers.Contains(pointerId))
                {
                    AcceptPointer(pointerId, PointerPhase.Cancel, historyCount);
                    _captureLostCancelCount++;
                }
                break;
            case WmPointerLeave:
                if (_pressedPointers.Contains(pointerId))
                {
                    AcceptPointer(pointerId, PointerPhase.Cancel, historyCount);
                    _captureLostCancelCount++;
                }
                AcceptPointer(pointerId, PointerPhase.Remove, historyCount);
                break;
            case WmPointerWheel:
            case WmPointerHWheel:
                AcceptPointer(pointerId,
                    _pressedPointers.Contains(pointerId) ? PointerPhase.Move : PointerPhase.Hover,
                    historyCount);
                break;
        }
    }

    private void AcceptPointer(uint pointerId, PointerPhase phase, int historySamples)
    {
        _pointerPacketCount++;
        _pointerHistorySampleCount += Math.Max(1, historySamples);
        switch (phase)
        {
            case PointerPhase.Add:
                if (!_activePointers.Add(pointerId)) _duplicatePacketCount++;
                break;
            case PointerPhase.Hover:
            case PointerPhase.Move:
                if (!_activePointers.Contains(pointerId)) _pointerTerminalMissingCount++;
                break;
            case PointerPhase.Down:
                if (!_activePointers.Contains(pointerId)) _pointerTerminalMissingCount++;
                if (!_pressedPointers.Add(pointerId)) _duplicatePacketCount++;
                break;
            case PointerPhase.Up:
            case PointerPhase.Cancel:
                if (!_pressedPointers.Remove(pointerId)) _pointerTerminalMissingCount++;
                break;
            case PointerPhase.Remove:
                if (_pressedPointers.Remove(pointerId)) _pointerTerminalMissingCount++;
                if (!_activePointers.Remove(pointerId)) _pointerTerminalMissingCount++;
                break;
        }
    }

    private void HandleKeyboard(uint message, uint value)
    {
        _rawKeyboardMessageCount++;
        _keyboardPacketCount++;
        switch (message)
        {
            case WmKeyDown:
            case WmSysKeyDown:
                _pressedKeys.Add(value);
                break;
            case WmKeyUp:
            case WmSysKeyUp:
                if (!_pressedKeys.Remove(value)) _keyTerminalMissingCount++;
                break;
            case WmDeadChar:
            case WmSysDeadChar:
                _deadCharacterCount++;
                break;
            case WmChar:
            case WmSysChar:
                var character = (char)value;
                if (char.IsHighSurrogate(character)) _pendingHighSurrogate = character;
                else if (char.IsLowSurrogate(character) && _pendingHighSurrogate is not null)
                {
                    _surrogatePairCount++;
                    _pendingHighSurrogate = null;
                }
                break;
        }
    }

    private void HandleIme(uint message)
    {
        _imeMessageCount++;
        switch (message)
        {
            case WmImeStartComposition:
                _imeStartCount++;
                break;
            case WmImeComposition:
                _imeCompositionCount++;
                break;
            case WmImeEndComposition:
                _imeEndCount++;
                break;
        }
    }

    private enum PointerPhase
    {
        Add,
        Hover,
        Down,
        Move,
        Up,
        Cancel,
        Remove,
    }
}

internal sealed class WinRtNativeMessageTrace
{
    internal int KeyboardMessageCount { get; private set; }
    internal int ImeMessageCount { get; private set; }

    internal void Observe(uint message)
    {
        if (message is >= 0x0100 and <= 0x0107)
            KeyboardMessageCount++;
        else if (message is >= 0x010D and <= 0x010F)
            ImeMessageCount++;
    }
}

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
internal sealed class WinRtAutomationRootProvider : IRawElementProviderSimple
{
    private readonly nint _hwnd;

    internal WinRtAutomationRootProvider(nint hwnd) => _hwnd = hwnd;

    public ProviderOptions ProviderOptions => ProviderOptions.ServerSideProvider | ProviderOptions.UseComThreading;

    public object? GetPatternProvider(int patternId) => null;

    public object? GetPropertyValue(int propertyId) => propertyId switch
    {
        30003 => 50032,
        30005 => "Doroti W1R ContentIsland",
        30009 => true,
        30020 => unchecked((int)_hwnd),
        _ => null,
    };

    public IRawElementProviderSimple? HostRawElementProvider => null;
}

[ComVisible(true)]
[Guid("D6DD68D1-86FD-4332-8666-9ABEDEA2D24C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IRawElementProviderSimple
{
    ProviderOptions ProviderOptions { get; }
    [return: MarshalAs(UnmanagedType.IUnknown)] object? GetPatternProvider(int patternId);
    [return: MarshalAs(UnmanagedType.Struct)] object? GetPropertyValue(int propertyId);
    IRawElementProviderSimple? HostRawElementProvider { get; }
}

[Flags]
internal enum ProviderOptions
{
    ClientSideProvider = 1,
    ServerSideProvider = 2,
    NonClientAreaProvider = 4,
    OverrideProvider = 8,
    ProviderOwnsSetFocus = 16,
    UseComThreading = 32,
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
    int ApplicationCreatedChildHwndCount,
    int PlatformInternalChildHwndCount,
    IReadOnlyList<string> PlatformInternalChildWindowClasses,
    int ConnectedIslandCount,
    int SiteBridgeCount,
    int VisibleSurfaceCount,
    int SurfaceRecreateCount,
    int IslandReconnectCount,
    int CloseDuringResizeCount);

internal sealed record W1Input(
    string PointerOwner,
    string KeyboardOwner,
    string FocusOwner,
    string CursorOwner,
    string AutomationRootOwner,
    string KeyboardBridgeMode,
    string KeyboardPacketProducer,
    string TextServiceOwner,
    string PhysicalKoreanImeStatus,
    bool BridgeProcessesPointerInput,
    bool BridgeProcessesKeyboardInput,
    int RootIslandInputSourceRegistrationCount,
    bool MouseInPointerEnabled,
    int EnableMouseInPointerCallCount,
    int RawPointerMessageCount,
    int RawMouseCompatibilityMessageCount,
    int RawKeyboardMessageCount,
    int KeyboardPacketCount,
    int DuplicateKeyboardPacketCount,
    int KeyboardDefWindowProcDelegationCount,
    int NativeTraceKeyboardMessageCount,
    int ImeMessageCount,
    int ImeDefWindowProcDelegationCount,
    int NativeTraceImeMessageCount,
    int ImeStartCount,
    int ImeCompositionCount,
    int ImeEndCount,
    int PointerPacketCount,
    int MouseCompatibilityPacketCount,
    int DuplicatePacketCount,
    int PointerHistorySampleCount,
    int PointerTerminalMissingCount,
    int KeyTerminalMissingCount,
    int CaptureLostCancelCount,
    int FocusTransitionCount,
    int ClientCursorCount,
    int NonClientCursorDelegationCount,
    int UiaRootRequestCount,
    int UiaNonRootDelegationCount,
    int DuplicateProviderRootCount,
    int DeadCharacterCount,
    int SurrogatePairCount,
    int ActivePointerCount,
    int PressedKeyCount,
    int CallbackWaitCount,
    bool FixturePassed,
    IReadOnlyList<string> Failures);

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

    internal static void DispatchTranslatedMessage(in Message message)
    {
        TranslateMessage(in message);
        DispatchMessage(in message);
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

    internal static IReadOnlyList<string> GetChildWindowClassNames(nint parent)
    {
        var result = new List<string>();
        EnumChildProc callback = (child, _) =>
        {
            var buffer = new StringBuilder(256);
            var length = GetClassName(child, buffer, buffer.Capacity);
            result.Add(length > 0 ? buffer.ToString() : $"<unknown:0x{child:X}>");
            return true;
        };
        EnumChildWindows(parent, callback, 0);
        GC.KeepAlive(callback);
        return result;
    }

    internal static nint LoadArrowCursor() => LoadCursor(0, new nint(32512));

    internal static int GetPointerHistorySampleCount(uint pointerId)
    {
        if (!GetPointerInfo(pointerId, out var current)) return 1;
        var count = Math.Clamp(current.HistoryCount, 1u, 64u);
        var entries = new PointerInfo[count];
        return GetPointerInfoHistory(pointerId, ref count, entries) ? checked((int)count) : 1;
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

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        internal int X;
        internal int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PointerInfo
    {
        internal uint PointerType;
        internal uint PointerId;
        internal uint FrameId;
        internal uint PointerFlags;
        internal nint SourceDevice;
        internal nint TargetHwnd;
        internal NativePoint PixelLocation;
        internal NativePoint HimetricLocation;
        internal NativePoint PixelLocationRaw;
        internal NativePoint HimetricLocationRaw;
        internal uint Time;
        internal uint HistoryCount;
        internal int InputData;
        internal uint KeyStates;
        internal ulong PerformanceCount;
        internal uint ButtonChangeType;
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

    [LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
    internal static partial nint SendMessage(nint hwnd, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool EnableMouseInPointer([MarshalAs(UnmanagedType.Bool)] bool enable);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetPointerInfo(uint pointerId, out PointerInfo pointerInfo);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetPointerInfoHistory(
        uint pointerId,
        ref uint entriesCount,
        [Out] PointerInfo[] pointerInfo);

    [LibraryImport("user32.dll")]
    internal static partial nint SetCapture(nint hwnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ReleaseCapture();

    [LibraryImport("user32.dll")]
    internal static partial nint GetCapture();

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

    [DllImport("user32.dll", EntryPoint = "GetClassNameW", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(nint hwnd, StringBuilder className, int maximumCount);

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

    [LibraryImport("user32.dll")]
    internal static partial nint SetCursor(nint cursor);

    [DllImport("UIAutomationCore.dll")]
    internal static extern nint UiaReturnRawElementProvider(
        nint hwnd,
        nuint wParam,
        nint lParam,
        [MarshalAs(UnmanagedType.Interface)] IRawElementProviderSimple provider);

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
