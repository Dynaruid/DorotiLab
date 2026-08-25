using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Doroti.Validation.WinRtContentIslandSpike;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Windows.UI;
using Windows.UI.Composition;

internal static class D1COwnedEnvelopeProgram
{
    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    [STAThread]
    private static int Main(string[] args)
    {
        var options = OwnedEnvelopeOptions.Parse(args);
        var roResult = Native.RoInitialize(0);
        if (roResult < 0)
        {
            Marshal.ThrowExceptionForHR(roResult);
        }

        try
        {
            using var spike = new WinRtOwnedEnvelopeSpike(options);
            return spike.Run();
        }
        catch (Exception exception)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(options.ReportPath)!);
            File.WriteAllText(options.ReportPath, JsonSerializer.Serialize(new
            {
                schema = "doroti.winrt-owned-envelope-d1c/v1",
                contractStatus = "FAIL",
                physicalStatus = "notVerified",
                exception = exception.ToString(),
            }, JsonOptions));
            Console.Error.WriteLine(exception);
            return 2;
        }
        finally
        {
            Native.RoUninitialize();
        }
    }
}

internal sealed record OwnedEnvelopeOptions(bool Automated, int HoldMilliseconds, string ReportPath)
{
    internal static OwnedEnvelopeOptions Parse(string[] args)
    {
        var automated = false;
        var holdMilliseconds = 200;
        string? reportPath = null;
        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--automated":
                    automated = true;
                    break;
                case "--hold-ms" when index + 1 < args.Length:
                    holdMilliseconds = int.Parse(args[++index]);
                    break;
                case "--report" when index + 1 < args.Length:
                    reportPath = args[++index];
                    break;
            }
        }

        reportPath ??= Path.Combine(
            FindRepositoryRoot(), ".doroti", "evidence", "d1c-owned-envelope-report.json");
        return new OwnedEnvelopeOptions(
            automated,
            Math.Clamp(holdMilliseconds, 0, 10_000),
            Path.GetFullPath(reportPath));
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Doroti", "Doroti.slnx")))
            {
                return current.FullName;
            }
            current = current.Parent;
        }
        throw new DirectoryNotFoundException("Unable to locate DorotiLab root.");
    }
}

internal sealed class WinRtOwnedEnvelopeSpike : IDisposable
{
    private const uint WmClose = 0x0010;
    private const uint WmDestroy = 0x0002;
    private const uint WmEraseBackground = 0x0014;
    private const uint WmSetCursor = 0x0020;
    private const uint WmNcHitTest = 0x0084;
    private const uint WmKeyDown = 0x0100;
    private const uint WmMouseMove = 0x0200;
    private const uint WmLeftButtonDown = 0x0201;
    private const uint WmLeftButtonUp = 0x0202;
    private const uint WmCaptureChanged = 0x0215;
    private const uint WmOwnedFrontReady = 0x8002;
    private const int VirtualKeyEscape = 0x1B;
    private const int HtClient = 1;
    private const int HtTransparent = -1;
    private const int MinimumWidth = 360;
    private const int MinimumHeight = 280;
    private const int ResizeBand = 10;
    private const int TitleBand = 44;
    private const int ExpectedAutomatedTargets = 236;
    private static readonly Dictionary<nint, WinRtOwnedEnvelopeSpike> Instances = [];
    private static readonly OwnedEnvelopeNative.WindowProcedure WindowProcedure = WndProc;

    private readonly OwnedEnvelopeOptions _options;
    private readonly object _stateGate = new();
    private readonly AutoResetEvent _renderRequested = new(false);
    private readonly ManualResetEventSlim _frontAvailable = new(true);
    private readonly List<string> _failures = [];
    private readonly List<OwnedTransition> _transitions = [];
    private DispatcherQueueController? _dispatcher;
    private Windows.System.DispatcherQueueController? _systemDispatcher;
    private Compositor? _compositor;
    private ContainerVisual? _root;
    private readonly ContainerVisual?[] _fronts = new ContainerVisual?[2];
    private readonly SpriteVisual?[] _contentVisuals = new SpriteVisual?[2];
    private readonly SpriteVisual?[] _chromeVisuals = new SpriteVisual?[2];
    private readonly CompositionSurfaceBrush?[] _brushes = new CompositionSurfaceBrush?[2];
    private readonly D3DCompositionGrid?[] _grids = new D3DCompositionGrid?[2];
    private readonly PreparedFront?[] _prepared = new PreparedFront?[2];
    private ContentIsland? _island;
    private DesktopAttachedSiteBridge? _siteBridge;
    private AppWindow? _appWindow;
    private Thread? _rasterThread;
    private nint _hwnd;
    private OwnedRect _envelopeScreen;
    private OwnedRect _envelopeLocal;
    private OwnedRect _initialRect;
    private OwnedRect _targetRect;
    private OwnedRect _visibleRect;
    private OwnedRect _dragStartRect;
    private OwnedPoint _dragStartPointer;
    private OwnedDragMode _dragMode;
    private long _targetGeneration;
    private long _visibleGeneration;
    private long _pendingRegionGeneration;
    private int _visibleSlot;
    private int _targetPublishCount;
    private int _preparedFrontCount;
    private int _frontSwitchCount;
    private int _abandonedPreparedFrontCount;
    private int _duplicateVisibleFrontCount;
    private int _geometryMismatchCount;
    private int _regionOpenCount;
    private int _regionApplyCount;
    private int _regionValidationCount;
    private int _captureBeginCount;
    private int _captureEndCount;
    private int _captureLostCount;
    private int _outsideHitPassCount;
    private int _insideHitPassCount;
    private int _fixedEnvelopeMismatchCount;
    private int _platformChildHwndCount;
    private IReadOnlyList<string> _platformChildWindowClasses = Array.Empty<string>();
    private uint _windowStyle;
    private bool _regionOpen;
    private bool _stopRaster;
    private bool _snapshotTaken;
    private bool _disposed;
    private float _rasterizationScale = 1f;

    internal WinRtOwnedEnvelopeSpike(OwnedEnvelopeOptions options) => _options = options;

    internal int Run()
    {
        Native.SetProcessDpiAwarenessContext(new nint(-4));
        RegisterWindowClass();
        _dispatcher = DispatcherQueueController.CreateOnCurrentThread();
        _systemDispatcher = Windows.System.DispatcherQueueController.CreateOnDedicatedThread();
        CreateFixedEnvelopeWindow();
        InitializeContentIsland();
        InitializeFronts();
        ApplyOwnedInputRegion();
        StartRasterThread();
        OwnedEnvelopeNative.ShowWindow(_hwnd, 5);
        OwnedEnvelopeNative.UpdateWindow(_hwnd);
        Console.WriteLine(
            $"D1-C fixed envelope HWND=0x{_hwnd:X}; drag the colored frame edges or title strip. Alt+F4 closes.");

        if (_options.Automated)
        {
            RunAutomatedScenario();
        }
        else
        {
            RunInteractiveLoop();
        }

        if (!_snapshotTaken)
        {
            CaptureSnapshot();
        }
        var report = BuildReport();
        Directory.CreateDirectory(Path.GetDirectoryName(_options.ReportPath)!);
        File.WriteAllText(
            _options.ReportPath,
            JsonSerializer.Serialize(report, D1COwnedEnvelopeProgram.JsonOptions));
        Console.WriteLine(
            $"D1-C contract={report.ContractStatus} physical={report.PhysicalStatus} " +
            $"targets={report.Presentation.TargetGeneration} visible={report.Presentation.VisibleGeneration} " +
            $"switches={report.Presentation.FrontSwitchCount} failures={report.Failures.Count}");
        Console.WriteLine($"report={_options.ReportPath}");
        return report.ContractStatus is "PASS" or "notRun" ? 0 : 2;
    }

    private void CreateFixedEnvelopeWindow()
    {
        var work = Native.GetPrimaryWorkArea();
        _envelopeScreen = new OwnedRect(work.Left, work.Top, work.Right, work.Bottom);
        _envelopeLocal = new OwnedRect(0, 0, work.Width, work.Height);
        _hwnd = OwnedEnvelopeNative.CreateOwnedEnvelopeWindow(
            "DorotiWinRtOwnedEnvelopeSpike",
            "Doroti D1-C - WinRT owned envelope",
            _envelopeScreen,
            WindowProcedure);
        Instances.Add(_hwnd, this);
        var width = Math.Min(900, Math.Max(MinimumWidth, work.Width - 240));
        var height = Math.Min(650, Math.Max(MinimumHeight, work.Height - 200));
        var left = Math.Max(40, (work.Width - width) / 2);
        var top = Math.Max(40, (work.Height - height) / 2);
        _initialRect = new OwnedRect(left, top, left + width, top + height);
        _targetRect = _initialRect;
        _visibleRect = _initialRect;
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
        {
            throw new InvalidOperationException("System compositor dispatch was rejected.");
        }
        if (!ready.Wait(TimeSpan.FromSeconds(5)))
        {
            throw new TimeoutException("System compositor creation timed out.");
        }
        if (failure is not null)
        {
            throw new InvalidOperationException("System compositor creation failed.", failure);
        }

        _root = _compositor!.CreateContainerVisual();
        _island = ContentIsland.CreateForSystemVisual(_dispatcher!.DispatcherQueue, _root);
        var windowId = Win32Interop.GetWindowIdFromWindow(_hwnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);
        _appWindow.AssociateWithDispatcherQueue(_dispatcher.DispatcherQueue);
        _siteBridge = DesktopAttachedSiteBridge.CreateFromWindowId(_dispatcher.DispatcherQueue, windowId);
        _siteBridge.ProcessesKeyboardInput = true;
        _siteBridge.ProcessesPointerInput = false;
        _siteBridge.Connect(_island);
        var siteView = _siteBridge.SiteView;
        _rasterizationScale = siteView.RasterizationScale > 0 ? siteView.RasterizationScale : 1f;
        _root.Size = new Vector2(
            _envelopeLocal.Width / _rasterizationScale,
            _envelopeLocal.Height / _rasterizationScale);
    }

    private void InitializeFronts()
    {
        for (var slot = 0; slot < 2; slot++)
        {
            var front = _compositor!.CreateContainerVisual();
            var content = _compositor.CreateSpriteVisual();
            var brush = _compositor.CreateSurfaceBrush();
            brush.Stretch = CompositionStretch.None;
            brush.HorizontalAlignmentRatio = 0;
            brush.VerticalAlignmentRatio = 0;
            var grid = new D3DCompositionGrid(_compositor);
            brush.Surface = grid.Surface;
            content.Brush = brush;
            var chrome = _compositor.CreateSpriteVisual();
            chrome.Brush = _compositor.CreateColorBrush(Color.FromArgb(210, 30, 72, 118));
            front.Children.InsertAtTop(content);
            front.Children.InsertAtTop(chrome);
            _fronts[slot] = front;
            _contentVisuals[slot] = content;
            _chromeVisuals[slot] = chrome;
            _brushes[slot] = brush;
            _grids[slot] = grid;
        }

        _targetGeneration = 1;
        _grids[0]!.Render(_initialRect.Width, _initialRect.Height, _targetGeneration);
        ConfigureFront(0, _initialRect);
        _root!.Children.InsertAtTop(_fronts[0]);
        _visibleGeneration = _targetGeneration;
        _visibleSlot = 0;
        _preparedFrontCount = 1;
        _frontSwitchCount = 1;
        _transitions.Add(new OwnedTransition(
            _visibleGeneration, _visibleSlot, _visibleRect.Left, _visibleRect.Top,
            _visibleRect.Width, _visibleRect.Height));
    }

    private void ConfigureFront(int slot, OwnedRect rect)
    {
        var scale = _rasterizationScale;
        var logicalWidth = rect.Width / scale;
        var logicalHeight = rect.Height / scale;
        _fronts[slot]!.Offset = new Vector3(rect.Left / scale, rect.Top / scale, 0);
        _fronts[slot]!.Size = new Vector2(logicalWidth, logicalHeight);
        _contentVisuals[slot]!.Size = new Vector2(logicalWidth, logicalHeight);
        _brushes[slot]!.Scale = new Vector2(1f / scale, 1f / scale);
        _chromeVisuals[slot]!.Size = new Vector2(logicalWidth, Math.Min(logicalHeight, TitleBand / scale));
    }

    private void StartRasterThread()
    {
        _rasterThread = new Thread(RenderLoop)
        {
            IsBackground = true,
            Name = "Doroti D1-C owned-envelope raster",
        };
        _rasterThread.Start();
    }

    private void RenderLoop()
    {
        while (true)
        {
            _renderRequested.WaitOne();
            if (_stopRaster) return;
            while (true)
            {
                _frontAvailable.Wait();
                if (_stopRaster) return;
                long generation;
                OwnedRect rect;
                int slot;
                lock (_stateGate)
                {
                    generation = _targetGeneration;
                    rect = _targetRect;
                    slot = 1 - _visibleSlot;
                }

                _grids[slot]!.Render(rect.Width, rect.Height, generation);
                lock (_stateGate)
                {
                    if (generation != _targetGeneration)
                    {
                        _abandonedPreparedFrontCount++;
                        continue;
                    }
                    _prepared[slot] = new PreparedFront(generation, rect);
                    _preparedFrontCount++;
                    _frontAvailable.Reset();
                }
                OwnedEnvelopeNative.PostMessage(
                    _hwnd, WmOwnedFrontReady, checked((nuint)generation), slot);
                break;
            }
        }
    }

    private void PublishTarget(OwnedRect target)
    {
        target = Normalize(target, _dragMode);
        lock (_stateGate)
        {
            if (target == _targetRect) return;
            _targetRect = target;
            _targetGeneration++;
            _targetPublishCount++;
        }
        _renderRequested.Set();
    }

    private void CommitPreparedFront(long generation, int slot)
    {
        PreparedFront? prepared;
        lock (_stateGate)
        {
            prepared = slot is >= 0 and < 2 ? _prepared[slot] : null;
            _prepared[slot] = null;
            if (prepared is null || prepared.Generation != generation || generation != _targetGeneration)
            {
                _abandonedPreparedFrontCount++;
                _frontAvailable.Set();
                _renderRequested.Set();
                return;
            }
        }

        ConfigureFront(slot, prepared.Rect);
        _root!.Children.RemoveAll();
        _root.Children.InsertAtTop(_fronts[slot]);
        lock (_stateGate)
        {
            _visibleSlot = slot;
            _visibleGeneration = generation;
            _visibleRect = prepared.Rect;
            _frontSwitchCount++;
            if (_root.Children.Count != 1) _duplicateVisibleFrontCount++;
            if (_visibleRect.Width != prepared.Rect.Width || _visibleRect.Height != prepared.Rect.Height)
            {
                _geometryMismatchCount++;
            }
            if (_transitions.Count < 512)
            {
                _transitions.Add(new OwnedTransition(
                    generation, slot, prepared.Rect.Left, prepared.Rect.Top,
                    prepared.Rect.Width, prepared.Rect.Height));
            }
        }
        _frontAvailable.Set();
        if (_pendingRegionGeneration == generation)
        {
            ApplyOwnedInputRegion();
            _pendingRegionGeneration = 0;
        }
    }

    private void BeginOwnedDrag()
    {
        var mode = OwnedHitTest();
        if (mode is OwnedDragMode.None or OwnedDragMode.Client) return;
        if (!OwnedEnvelopeNative.GetCursorPos(out _dragStartPointer)) return;
        _dragStartPointer = new OwnedPoint(
            _dragStartPointer.X - _envelopeScreen.Left,
            _dragStartPointer.Y - _envelopeScreen.Top);
        _dragStartRect = _visibleRect;
        _dragMode = mode;
        OpenOwnedInputRegion();
        OwnedEnvelopeNative.SetCapture(_hwnd);
        _captureBeginCount++;
    }

    private void UpdateOwnedDrag()
    {
        if (!OwnedEnvelopeNative.GetCursorPos(out var screenPointer)) return;
        var pointer = new OwnedPoint(
            screenPointer.X - _envelopeScreen.Left,
            screenPointer.Y - _envelopeScreen.Top);
        var deltaX = pointer.X - _dragStartPointer.X;
        var deltaY = pointer.Y - _dragStartPointer.Y;
        var target = _dragStartRect;
        if ((_dragMode & OwnedDragMode.Move) != 0)
        {
            target = target.Offset(deltaX, deltaY);
        }
        else
        {
            if ((_dragMode & OwnedDragMode.Left) != 0) target = target with { Left = target.Left + deltaX };
            if ((_dragMode & OwnedDragMode.Right) != 0) target = target with { Right = target.Right + deltaX };
            if ((_dragMode & OwnedDragMode.Top) != 0) target = target with { Top = target.Top + deltaY };
            if ((_dragMode & OwnedDragMode.Bottom) != 0) target = target with { Bottom = target.Bottom + deltaY };
        }
        PublishTarget(target);
    }

    private void EndOwnedDrag(bool releaseCapture)
    {
        if (_dragMode == OwnedDragMode.None) return;
        _dragMode = OwnedDragMode.None;
        if (releaseCapture && OwnedEnvelopeNative.GetCapture() == _hwnd)
        {
            OwnedEnvelopeNative.ReleaseCapture();
        }
        _captureEndCount++;
        long targetGeneration;
        long visibleGeneration;
        lock (_stateGate)
        {
            targetGeneration = _targetGeneration;
            visibleGeneration = _visibleGeneration;
        }
        if (targetGeneration == visibleGeneration)
        {
            ApplyOwnedInputRegion();
        }
        else
        {
            _pendingRegionGeneration = targetGeneration;
        }
    }

    private OwnedDragMode OwnedHitTest()
    {
        if (!OwnedEnvelopeNative.GetCursorPos(out var point)) return OwnedDragMode.None;
        var x = point.X - _envelopeScreen.Left;
        var y = point.Y - _envelopeScreen.Top;
        var rect = _visibleRect;
        if (!rect.Contains(x, y)) return OwnedDragMode.None;
        var mode = OwnedDragMode.None;
        if (x < rect.Left + ResizeBand) mode |= OwnedDragMode.Left;
        else if (x >= rect.Right - ResizeBand) mode |= OwnedDragMode.Right;
        if (y < rect.Top + ResizeBand) mode |= OwnedDragMode.Top;
        else if (y >= rect.Bottom - ResizeBand) mode |= OwnedDragMode.Bottom;
        if (mode != OwnedDragMode.None) return mode;
        return y < rect.Top + TitleBand ? OwnedDragMode.Move : OwnedDragMode.Client;
    }

    private void SetOwnedCursor()
    {
        var cursorId = OwnedHitTest() switch
        {
            OwnedDragMode.Left or OwnedDragMode.Right => 32644,
            OwnedDragMode.Top or OwnedDragMode.Bottom => 32645,
            OwnedDragMode.Left | OwnedDragMode.Top or
            OwnedDragMode.Right | OwnedDragMode.Bottom => 32642,
            OwnedDragMode.Right | OwnedDragMode.Top or
            OwnedDragMode.Left | OwnedDragMode.Bottom => 32643,
            OwnedDragMode.Move => 32646,
            _ => 32512,
        };
        OwnedEnvelopeNative.SetCursor(OwnedEnvelopeNative.LoadCursor(0, new nint(cursorId)));
    }

    private OwnedRect Normalize(OwnedRect rect, OwnedDragMode mode)
    {
        if ((mode & OwnedDragMode.Move) != 0)
        {
            var left = Math.Clamp(rect.Left, 0, Math.Max(0, _envelopeLocal.Width - rect.Width));
            var top = Math.Clamp(rect.Top, 0, Math.Max(0, _envelopeLocal.Height - rect.Height));
            return new OwnedRect(left, top, left + rect.Width, top + rect.Height);
        }
        var leftEdge = Math.Clamp(rect.Left, 0, rect.Right - MinimumWidth);
        var rightEdge = Math.Clamp(rect.Right, leftEdge + MinimumWidth, _envelopeLocal.Width);
        var topEdge = Math.Clamp(rect.Top, 0, rect.Bottom - MinimumHeight);
        var bottomEdge = Math.Clamp(rect.Bottom, topEdge + MinimumHeight, _envelopeLocal.Height);
        return new OwnedRect(leftEdge, topEdge, rightEdge, bottomEdge);
    }

    private void OpenOwnedInputRegion()
    {
        if (_regionOpen) return;
        if (OwnedEnvelopeNative.SetWindowRgn(_hwnd, 0, true) == 0)
        {
            throw new InvalidOperationException(
                $"SetWindowRgn(open) failed: {Marshal.GetLastWin32Error()}.");
        }
        _regionOpen = true;
        _regionOpenCount++;
    }

    private void ApplyOwnedInputRegion()
    {
        var rect = _visibleRect;
        var region = OwnedEnvelopeNative.CreateRectRgn(rect.Left, rect.Top, rect.Right, rect.Bottom);
        if (region == 0)
        {
            throw new InvalidOperationException(
                $"CreateRectRgn failed: {Marshal.GetLastWin32Error()}.");
        }
        if (OwnedEnvelopeNative.SetWindowRgn(_hwnd, region, true) == 0)
        {
            OwnedEnvelopeNative.DeleteObject(region);
            throw new InvalidOperationException(
                $"SetWindowRgn(constrain) failed: {Marshal.GetLastWin32Error()}.");
        }
        _regionOpen = false;
        _regionApplyCount++;
    }

    private void ValidateOwnedInputRegion()
    {
        var probe = OwnedEnvelopeNative.CreateRectRgn(0, 0, 0, 0);
        if (probe == 0) throw new InvalidOperationException("CreateRectRgn probe failed.");
        try
        {
            if (OwnedEnvelopeNative.GetWindowRgn(_hwnd, probe) == 0)
            {
                throw new InvalidOperationException("GetWindowRgn returned no owned input region.");
            }
            var insideX = _visibleRect.Left + Math.Max(1, _visibleRect.Width / 2);
            var insideY = _visibleRect.Top + Math.Max(1, _visibleRect.Height / 2);
            if (OwnedEnvelopeNative.PtInRegion(probe, insideX, insideY)) _insideHitPassCount++;
            if (!OwnedEnvelopeNative.PtInRegion(probe, 2, 2)) _outsideHitPassCount++;
            _regionValidationCount++;
        }
        finally
        {
            OwnedEnvelopeNative.DeleteObject(probe);
        }
    }

    private void RunAutomatedScenario()
    {
        OpenOwnedInputRegion();
        OwnedEnvelopeNative.SetCapture(_hwnd);
        _captureBeginCount++;
        if (OwnedEnvelopeNative.GetCapture() != _hwnd)
        {
            _failures.Add("Automated owned-envelope smoke failed to acquire capture.");
        }
        var origin = _initialRect;
        var modes = new[]
        {
            OwnedDragMode.Left,
            OwnedDragMode.Top,
            OwnedDragMode.Right,
            OwnedDragMode.Bottom,
            OwnedDragMode.Left | OwnedDragMode.Top,
            OwnedDragMode.Right | OwnedDragMode.Bottom,
        };
        foreach (var mode in modes)
        {
            _dragMode = mode;
            for (var step = 0; step < 40; step++)
            {
                var direction = step < 20 ? step + 1 : 40 - step;
                var amountX = direction * 9;
                var amountY = direction * 6;
                var target = origin;
                if ((mode & OwnedDragMode.Left) != 0) target = target with { Left = origin.Left - amountX };
                if ((mode & OwnedDragMode.Right) != 0) target = target with { Right = origin.Right + amountX };
                if ((mode & OwnedDragMode.Top) != 0) target = target with { Top = origin.Top - amountY };
                if ((mode & OwnedDragMode.Bottom) != 0) target = target with { Bottom = origin.Bottom + amountY };
                PublishTarget(target);
                PumpFor(TimeSpan.FromMilliseconds(2));
            }
        }
        _dragMode = OwnedDragMode.None;
        PublishTarget(origin);
        WaitForLatestVisible(TimeSpan.FromSeconds(10));
        if (OwnedEnvelopeNative.GetCapture() == _hwnd)
        {
            OwnedEnvelopeNative.ReleaseCapture();
            _captureEndCount++;
        }
        ApplyOwnedInputRegion();
        ValidateOwnedInputRegion();
        if (_options.HoldMilliseconds > 0)
        {
            PumpFor(TimeSpan.FromMilliseconds(_options.HoldMilliseconds));
        }
        CaptureSnapshot();
        OwnedEnvelopeNative.DestroyWindow(_hwnd);
        PumpFor(TimeSpan.FromMilliseconds(20));
    }

    private void WaitForLatestVisible(TimeSpan timeout)
    {
        var until = Stopwatch.GetTimestamp() + (long)(timeout.TotalSeconds * Stopwatch.Frequency);
        while (Stopwatch.GetTimestamp() < until)
        {
            long target;
            long visible;
            lock (_stateGate)
            {
                target = _targetGeneration;
                visible = _visibleGeneration;
            }
            if (target == visible) return;
            PumpFor(TimeSpan.FromMilliseconds(5));
        }
        _failures.Add("Latest owned-envelope target did not become visible within 10 seconds.");
    }

    private static void PumpFor(TimeSpan duration)
    {
        var until = Stopwatch.GetTimestamp() + (long)(duration.TotalSeconds * Stopwatch.Frequency);
        while (Stopwatch.GetTimestamp() < until)
        {
            while (Native.PeekMessage(out var message, 0, 0, 0, 1))
            {
                Native.DispatchTranslatedMessage(in message);
            }
            Thread.Sleep(1);
        }
    }

    private void RunInteractiveLoop()
    {
        while (Native.GetMessage(out var message, 0, 0, 0) > 0)
        {
            Native.DispatchTranslatedMessage(in message);
        }
    }

    private void CaptureSnapshot()
    {
        if (_snapshotTaken || _hwnd == 0) return;
        if (_dragMode != OwnedDragMode.None) EndOwnedDrag(releaseCapture: true);
        WaitForLatestVisible(TimeSpan.FromSeconds(5));
        if (_regionOpen) ApplyOwnedInputRegion();
        ValidateOwnedInputRegion();
        _platformChildWindowClasses = OwnedEnvelopeNative.GetChildWindowClassNames(_hwnd);
        _platformChildHwndCount = _platformChildWindowClasses.Count;
        _windowStyle = unchecked((uint)OwnedEnvelopeNative.GetWindowLongPtr(_hwnd, -16).ToInt64());
        if (!OwnedEnvelopeNative.GetWindowRect(_hwnd, out var actualEnvelope) ||
            actualEnvelope != _envelopeScreen)
        {
            _fixedEnvelopeMismatchCount++;
        }
        _snapshotTaken = true;
    }

    private OwnedEnvelopeReport BuildReport()
    {
        if (_platformChildHwndCount != 1 ||
            _platformChildWindowClasses.Any(value => value != "InputSiteWindowClass"))
        {
            _failures.Add(
                $"Expected one platform InputSiteWindowClass child, found " +
                $"{_platformChildHwndCount}: {string.Join(", ", _platformChildWindowClasses)}.");
        }
        if (_siteBridge?.ProcessesPointerInput != false)
            _failures.Add("DesktopAttachedSiteBridge pointer processing must remain disabled.");
        if (_siteBridge?.ProcessesKeyboardInput != true)
            _failures.Add("DesktopAttachedSiteBridge keyboard runtime floor must remain enabled.");
        if (_root?.Children.Count != 1)
            _failures.Add($"Visible root child count was {_root?.Children.Count ?? -1}, expected one.");
        if (_visibleGeneration != _targetGeneration)
            _failures.Add($"Visible generation {_visibleGeneration} did not reach target {_targetGeneration}.");
        if (_duplicateVisibleFrontCount != 0)
            _failures.Add($"Duplicate visible fronts: {_duplicateVisibleFrontCount}.");
        if (_geometryMismatchCount != 0)
            _failures.Add($"Front geometry mismatches: {_geometryMismatchCount}.");
        if (_fixedEnvelopeMismatchCount != 0)
            _failures.Add($"Fixed envelope mismatches: {_fixedEnvelopeMismatchCount}.");
        if (_regionOpen) _failures.Add("Owned input region remained open at report time.");
        if (_regionValidationCount == 0 || _insideHitPassCount == 0 || _outsideHitPassCount == 0)
            _failures.Add("Owned input-region inside/outside validation did not pass.");
        if (_options.Automated && _targetGeneration != ExpectedAutomatedTargets)
            _failures.Add(
                $"Automated target generation was {_targetGeneration}, expected {ExpectedAutomatedTargets}.");

        var automatedStatus = _options.Automated
            ? (_failures.Count == 0 ? "PASS" : "FAIL")
            : "notRun";
        return new OwnedEnvelopeReport(
            "doroti.winrt-owned-envelope-d1c/v1",
            automatedStatus,
            "notVerified",
            _options.Automated ? "automated" : "interactive",
            new OwnedEnvelopeTopology(
                0, _platformChildHwndCount, _platformChildWindowClasses,
                1, 1, 2, 1,
                _envelopeScreen, _initialRect, _visibleRect,
                "fixed-work-area-ws-popup", $"0x{_windowStyle:X8}"),
            new OwnedEnvelopePresentation(
                _targetGeneration, _visibleGeneration, _targetPublishCount,
                _preparedFrontCount, _frontSwitchCount, _abandonedPreparedFrontCount,
                _duplicateVisibleFrontCount, _geometryMismatchCount,
                _fixedEnvelopeMismatchCount, _visibleSlot),
            new OwnedEnvelopeInputRegion(
                _regionOpenCount, _regionApplyCount, _regionValidationCount,
                _insideHitPassCount, _outsideHitPassCount, _regionOpen,
                _captureBeginCount, _captureEndCount, _captureLostCount),
            new OwnedEnvelopeShellRisk(
                "notVerified", "notVerified", "notVerified", "notVerified",
                "WS_POPUP fixed envelope differs from the user-visible owned rect"),
            _transitions,
            _failures.Distinct(StringComparer.Ordinal).ToArray(),
            new[]
            {
                "Left/Top/Right/Bottom/corners fast-medium-slow visible behavior: notVerified",
                "outside-window click-through and post-drag click recovery: notVerified",
                "Snap layouts/system menu/taskbar preview/maximize/restore: notVerified",
                "UIA bounds and physical Korean IME: notVerified",
            });
    }

    private static nint WndProc(nint hwnd, uint message, nuint wParam, nint lParam)
    {
        if (!Instances.TryGetValue(hwnd, out var spike))
        {
            return OwnedEnvelopeNative.DefWindowProc(hwnd, message, wParam, lParam);
        }
        try
        {
            switch (message)
            {
                case WmEraseBackground:
                    return 1;
                case WmNcHitTest:
                    return spike.OwnedHitTest() == OwnedDragMode.None ? HtTransparent : HtClient;
                case WmSetCursor:
                    spike.SetOwnedCursor();
                    return 1;
                case WmLeftButtonDown:
                    spike.BeginOwnedDrag();
                    return 0;
                case WmMouseMove when spike._dragMode != OwnedDragMode.None:
                    spike.UpdateOwnedDrag();
                    return 0;
                case WmLeftButtonUp when spike._dragMode != OwnedDragMode.None:
                    spike.EndOwnedDrag(releaseCapture: true);
                    return 0;
                case WmCaptureChanged when spike._dragMode != OwnedDragMode.None:
                    spike._captureLostCount++;
                    spike.EndOwnedDrag(releaseCapture: false);
                    return 0;
                case WmOwnedFrontReady:
                    spike.CommitPreparedFront(checked((long)wParam), checked((int)lParam));
                    return 0;
                case WmKeyDown when (int)wParam == VirtualKeyEscape:
                case WmClose:
                    spike.CaptureSnapshot();
                    OwnedEnvelopeNative.DestroyWindow(hwnd);
                    return 0;
                case WmDestroy:
                    OwnedEnvelopeNative.PostQuitMessage(0);
                    return 0;
            }
        }
        catch (Exception exception)
        {
            spike._failures.Add(exception.ToString());
            OwnedEnvelopeNative.PostQuitMessage(2);
            return 0;
        }
        return OwnedEnvelopeNative.DefWindowProc(hwnd, message, wParam, lParam);
    }

    private static void RegisterWindowClass() =>
        OwnedEnvelopeNative.RegisterWindowClass("DorotiWinRtOwnedEnvelopeSpike", WindowProcedure);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _stopRaster = true;
        _frontAvailable.Set();
        _renderRequested.Set();
        _rasterThread?.Join(TimeSpan.FromSeconds(2));
        if (_hwnd != 0 && OwnedEnvelopeNative.IsWindow(_hwnd))
            OwnedEnvelopeNative.DestroyWindow(_hwnd);
        Instances.Remove(_hwnd);
        foreach (var grid in _grids) grid?.Dispose();
        foreach (var brush in _brushes) brush?.Dispose();
        foreach (var visual in _chromeVisuals) visual?.Dispose();
        foreach (var visual in _contentVisuals) visual?.Dispose();
        foreach (var front in _fronts) front?.Dispose();
        _root?.Dispose();
        _siteBridge?.Dispose();
        _island?.Dispose();
        _compositor?.Dispose();
        _systemDispatcher?.ShutdownQueueAsync();
        _dispatcher?.ShutdownQueue();
        _frontAvailable.Dispose();
        _renderRequested.Dispose();
    }
}

[Flags]
internal enum OwnedDragMode
{
    None = 0,
    Client = 1,
    Move = 2,
    Left = 4,
    Top = 8,
    Right = 16,
    Bottom = 32,
}

internal readonly record struct OwnedRect(int Left, int Top, int Right, int Bottom)
{
    internal int Width => Right - Left;
    internal int Height => Bottom - Top;
    internal bool Contains(int x, int y) => x >= Left && x < Right && y >= Top && y < Bottom;
    internal OwnedRect Offset(int x, int y) => new(Left + x, Top + y, Right + x, Bottom + y);
}

[StructLayout(LayoutKind.Sequential)]
internal struct OwnedPoint
{
    internal int X;
    internal int Y;

    internal OwnedPoint(int x, int y)
    {
        X = x;
        Y = y;
    }
}

internal sealed record PreparedFront(long Generation, OwnedRect Rect);
internal sealed record OwnedTransition(long Generation, int Slot, int Left, int Top, int Width, int Height);
internal sealed record OwnedEnvelopeTopology(
    int ApplicationCreatedChildHwndCount,
    int PlatformInternalChildHwndCount,
    IReadOnlyList<string> PlatformInternalChildWindowClasses,
    int ContentIslandCount,
    int SiteBridgeCount,
    int PreparedSurfaceCount,
    int SimultaneouslyVisibleSurfaceCount,
    OwnedRect EnvelopeScreen,
    OwnedRect InitialOwnedRect,
    OwnedRect VisibleOwnedRect,
    string EnvelopeMode,
    string WindowStyle);
internal sealed record OwnedEnvelopePresentation(
    long TargetGeneration,
    long VisibleGeneration,
    int TargetPublishCount,
    int PreparedFrontCount,
    int FrontSwitchCount,
    int AbandonedPreparedFrontCount,
    int DuplicateVisibleFrontCount,
    int GeometryMismatchCount,
    int FixedEnvelopeMismatchCount,
    int VisibleSlot);
internal sealed record OwnedEnvelopeInputRegion(
    int FullRegionOpenCount,
    int ConstrainedRegionApplyCount,
    int ValidationCount,
    int InsideHitPassCount,
    int OutsideHitPassCount,
    bool RegionOpen,
    int CaptureBeginCount,
    int CaptureEndCount,
    int CaptureLostCount);
internal sealed record OwnedEnvelopeShellRisk(
    string SnapLayouts,
    string TaskbarPreview,
    string MaximizeRestore,
    string UiaBounds,
    string Boundary);
internal sealed record OwnedEnvelopeReport(
    string Schema,
    string ContractStatus,
    string PhysicalStatus,
    string Mode,
    OwnedEnvelopeTopology Topology,
    OwnedEnvelopePresentation Presentation,
    OwnedEnvelopeInputRegion InputRegion,
    OwnedEnvelopeShellRisk ShellRisk,
    IReadOnlyList<OwnedTransition> Transitions,
    IReadOnlyList<string> Failures,
    IReadOnlyList<string> PhysicalMatrix);

internal static partial class OwnedEnvelopeNative
{
    private const uint ClassOwnDc = 0x0020;
    private const uint WindowStylePopup = 0x80000000;
    private const uint WindowStyleClipChildren = 0x02000000;
    private const uint ExtendedStyleAppWindow = 0x00040000;
    private const uint ExtendedStyleNoRedirectionBitmap = 0x00200000;
    private static readonly Dictionary<string, ushort> Classes = [];

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate nint WindowProcedure(nint hwnd, uint message, nuint wParam, nint lParam);

    internal static void RegisterWindowClass(string name, WindowProcedure procedure)
    {
        if (Classes.ContainsKey(name)) return;
        var value = new WindowClass
        {
            Size = (uint)Marshal.SizeOf<WindowClass>(),
            Style = ClassOwnDc,
            WindowProcedure = Marshal.GetFunctionPointerForDelegate(procedure),
            Instance = GetModuleHandle(null),
            Cursor = LoadCursor(0, new nint(32512)),
            ClassName = name,
        };
        var atom = RegisterClassEx(in value);
        if (atom == 0) throw new InvalidOperationException($"RegisterClassEx failed: {Marshal.GetLastWin32Error()}.");
        Classes.Add(name, atom);
    }

    internal static nint CreateOwnedEnvelopeWindow(
        string className,
        string title,
        OwnedRect bounds,
        WindowProcedure procedure)
    {
        RegisterWindowClass(className, procedure);
        var hwnd = CreateWindowEx(
            ExtendedStyleAppWindow | ExtendedStyleNoRedirectionBitmap,
            className,
            title,
            WindowStylePopup | WindowStyleClipChildren,
            bounds.Left,
            bounds.Top,
            bounds.Width,
            bounds.Height,
            0, 0, GetModuleHandle(null), 0);
        if (hwnd == 0) throw new InvalidOperationException($"CreateWindowEx failed: {Marshal.GetLastWin32Error()}.");
        return hwnd;
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
        internal nint SmallIcon;
    }

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandle(string? moduleName);

    [DllImport("user32.dll", EntryPoint = "RegisterClassExW", SetLastError = true)]
    private static extern ushort RegisterClassEx(in WindowClass value);

    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint CreateWindowEx(
        uint extendedStyle, string className, string windowName, uint style,
        int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShowWindow(nint hwnd, int command);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UpdateWindow(nint hwnd);

    [LibraryImport("user32.dll", EntryPoint = "DefWindowProcW")]
    internal static partial nint DefWindowProc(nint hwnd, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyWindow(nint hwnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool IsWindow(nint hwnd);

    [LibraryImport("user32.dll")]
    internal static partial void PostQuitMessage(int exitCode);

    [LibraryImport("user32.dll", EntryPoint = "PostMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PostMessage(nint hwnd, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetWindowRect(nint hwnd, out OwnedRect rect);

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    internal static partial nint GetWindowLongPtr(nint hwnd, int index);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetCursorPos(out OwnedPoint point);

    [LibraryImport("user32.dll")]
    internal static partial nint SetCapture(nint hwnd);

    [LibraryImport("user32.dll")]
    internal static partial nint GetCapture();

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ReleaseCapture();

    [LibraryImport("user32.dll", EntryPoint = "LoadCursorW")]
    internal static partial nint LoadCursor(nint instance, nint cursorName);

    [LibraryImport("user32.dll")]
    internal static partial nint SetCursor(nint cursor);

    [LibraryImport("gdi32.dll", SetLastError = true)]
    internal static partial nint CreateRectRgn(int left, int top, int right, int bottom);

    [LibraryImport("user32.dll", SetLastError = true)]
    internal static partial int GetWindowRgn(nint hwnd, nint region);

    [LibraryImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PtInRegion(nint region, int x, int y);

    [LibraryImport("user32.dll", SetLastError = true)]
    internal static partial int SetWindowRgn(
        nint hwnd,
        nint region,
        [MarshalAs(UnmanagedType.Bool)] bool redraw);

    [LibraryImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DeleteObject(nint value);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate bool EnumChildProcedure(nint hwnd, nint parameter);

    internal static IReadOnlyList<string> GetChildWindowClassNames(nint parent)
    {
        var result = new List<string>();
        EnumChildProcedure callback = (child, _) =>
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

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumChildWindows(
        nint parent,
        EnumChildProcedure callback,
        nint parameter);

    [DllImport("user32.dll", EntryPoint = "GetClassNameW", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(nint hwnd, StringBuilder className, int maximumCount);
}
