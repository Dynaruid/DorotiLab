using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Validation.WindowsAcrylicContentIslandCapability;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Vortice.Direct3D11;
using Windows.UI.Composition;

namespace Doroti.Validation.WindowsAcrylicCompositionSwapchainSpike;

internal static partial class Program
{
    private const uint WsOverlappedWindow = 0x00CF0000;
    private const uint WsVisible = 0x10000000;
    private const uint PmRemove = 0x0001;
    private const uint WmDestroy = 0x0002;
    private const uint WmSize = 0x0005;
    private static readonly string WindowClass =
        $"DorotiAcrylicCompositionSwapchainP1Cs-{Environment.ProcessId}";
    private static readonly WndProc WindowProcedure = HandleWindowMessage;
    private static VisibleRenderQueue? _visibleRenderer;

    [STAThread]
    private static int Main(string[] args)
    {
        var options = Options.Parse(args);
        var reportPath = options.ReportPath;
        var roResult = RoInitialize(0);
        if (roResult < 0) return WriteFailure(reportPath, $"RoInitialize failed: 0x{roResult:X8}");
        nint context = 0;
        try
        {
            using var angle = new AngleDevice();
            var snapshot = new ProbeSnapshot
            {
                AbiVersion = 1,
                StructSize = checked((uint)Marshal.SizeOf<ProbeSnapshot>()),
            };
            var probeResult = ProbeCreate(
                angle.D3D11DevicePointer, out context, out var surfaceHandle, ref snapshot);
            var compositionSurfaceConnected = false;
            string? compositionSurfaceFailure = null;
            if (probeResult >= 0 && snapshot.PresentationSupported != 0 &&
                snapshot.PresentationSurfaceHresult >= 0 && surfaceHandle != 0)
            {
                try
                {
                    using var worker = new CompositionWorker();
                    compositionSurfaceConnected = worker.Invoke(() =>
                    {
                        var surface = CreateCompositionSurfaceForHandle(
                            worker.Compositor, checked((nint)surfaceHandle));
                        using var surfaceLifetime = (IDisposable)surface;
                        using var brush = worker.Compositor.CreateSurfaceBrush(surface);
                        using var visual = worker.Compositor.CreateSpriteVisual();
                        visual.Brush = brush;
                        visual.Size = new System.Numerics.Vector2(320, 180);
                        return true;
                    });
                }
                catch (Exception exception)
                {
                    compositionSurfaceFailure = exception.ToString();
                }
            }
            var bufferProtocol = probeResult >= 0 && compositionSurfaceConnected
                ? RunBufferProtocol(angle, context, 500)
                : BufferProtocolResult.NotRun;
            var visible = options.Duration > TimeSpan.Zero && bufferProtocol.Status == "PASS"
                ? RunVisibleSession(angle, context, surfaceHandle, options)
                : VisibleResult.NotRun;
            var pass = probeResult >= 0 && snapshot.FactoryHresult >= 0 &&
                snapshot.PresentationSupported != 0 && snapshot.ManagerHresult >= 0 &&
                snapshot.SurfaceHandleHresult >= 0 && snapshot.PresentationSurfaceHresult >= 0 &&
                surfaceHandle != 0 && compositionSurfaceConnected && bufferProtocol.Status == "PASS" &&
                (options.Duration <= TimeSpan.Zero || visible.Status == "PASS-internal");
            if (context != 0)
            {
                ProbeDestroy(context);
                context = 0;
            }
            var report = new
            {
                schema = "doroti.windows-acrylic-composition-swapchain-p1cs/v1",
                status = pass
                    ? options.Duration > TimeSpan.Zero ? "PASS-internal" : "PASS-capability"
                    : "FAIL",
                operatingSystem = Environment.OSVersion.VersionString,
                windowsAppSdk = "Microsoft.WindowsAppSDK/2.4.0",
                angle = new
                {
                    angle.RequiredExtensionsPresent,
                    angle.HasD3DDeviceQuery,
                    angle.HasD3DTextureClientBuffer,
                    angle.HasShareHandleSurface,
                    angle.HasFenceSync,
                    d3D11DevicePointerNonZero = angle.D3D11DevicePointer != 0,
                },
                probeResult,
                snapshot,
                compositionSurfaceHandle = $"0x{surfaceHandle:X}",
                compositionSurfaceConnected,
                compositionSurfaceFailure,
                bufferProtocol,
                visible,
                deviceFlagRequirements = new
                {
                    singleThreaded = 0x1,
                    preventInternalThreadingOptimizations = 0x8,
                    note = "The official Composition Swapchain sample creates its D3D11 device with both flags; this probe records the actual ANGLE device flags without treating the sample choice as a capability requirement.",
                },
                evidence = new
                {
                    capability = pass ? "PASS" : "FAIL",
                    buffersAndPresentation = bufferProtocol.Status,
                    visibleCapture = options.Duration > TimeSpan.Zero ? "externalPending" : "notRun",
                    physical = "notVerified",
                },
                decision = pass
                    ? "Capability passed; continue to bounded three-slot buffer implementation."
                    : "P1-CS immediate FAIL at S0 capability; do not create buffers or product integration.",
            };
            WriteReport(reportPath, report);
            Console.WriteLine(
                $"P1-CS status={report.status} factory=0x{snapshot.FactoryHresult:X8} " +
                $"supported={snapshot.PresentationSupported} flags=0x{snapshot.DeviceCreationFlags:X8} " +
                $"compositionSurface={compositionSurfaceConnected}");
            Console.WriteLine($"report={reportPath}");
            return pass ? 0 : 2;
        }
        catch (Exception exception)
        {
            return WriteFailure(reportPath, exception.ToString());
        }
        finally
        {
            if (context != 0) ProbeDestroy(context);
            RoUninitialize();
        }
    }

    private static BufferProtocolResult RunBufferProtocol(
        AngleDevice angle, nint context, int presentCount)
    {
        var textures = new ID3D11Texture2D?[3];
        var ledger = new List<PresentRecord>(presentCount);
        var registered = new bool[3];
        var availableReuseCount = 0;
        var waitCount = 0;
        var unavailableReuseAttempts = 0;
        var maximumSlots = 0;
        try
        {
            var sizes = new (int Width, int Height)[]
            {
                (500, 300), (731, 419), (419, 731),
                (640, 360), (853, 481), (481, 853),
            };
            for (var generation = 1; generation <= presentCount; generation++)
            {
                var slot = -1;
                for (var index = 0; index < 3; index++)
                {
                    if (!registered[index])
                    {
                        slot = index;
                        break;
                    }
                    var availableResult = IsAvailable(context, checked((uint)index), out var available);
                    if (availableResult < 0) Marshal.ThrowExceptionForHR(availableResult);
                    if (available != 0)
                    {
                        slot = index;
                        availableReuseCount++;
                        break;
                    }
                }
                if (slot < 0)
                {
                    waitCount++;
                    var waitResult = WaitForAvailable(context, 1000, out var availableSlot);
                    if (waitResult < 0) Marshal.ThrowExceptionForHR(waitResult);
                    slot = checked((int)availableSlot);
                    availableReuseCount++;
                }
                uint beforeAvailable = registered[slot] ? 0u : 1u;
                var beforeResult = registered[slot]
                    ? IsAvailable(context, checked((uint)slot), out beforeAvailable)
                    : 1;
                if (beforeResult < 0) Marshal.ThrowExceptionForHR(beforeResult);
                if (registered[slot] && beforeAvailable == 0)
                {
                    unavailableReuseAttempts++;
                    throw new InvalidOperationException("An unavailable P1-CS slot was selected for reuse.");
                }

                textures[slot]?.Dispose();
                textures[slot] = null;
                var size = sizes[(generation - 1) % sizes.Length];
                var bufferSnapshot = new BufferSnapshot
                {
                    AbiVersion = 1,
                    StructSize = checked((uint)Marshal.SizeOf<BufferSnapshot>()),
                };
                var replaceResult = ReplaceBuffer(
                    context, checked((uint)slot), checked((uint)size.Width),
                    checked((uint)size.Height), out var texturePointer,
                    out var availableEvent, ref bufferSnapshot);
                if (replaceResult < 0) Marshal.ThrowExceptionForHR(replaceResult);
                if (texturePointer == 0 || availableEvent == 0 ||
                    bufferSnapshot.InitiallyAvailable == 0)
                    throw new InvalidOperationException("A newly registered P1-CS buffer was not available.");
                textures[slot] = new ID3D11Texture2D(texturePointer);
                registered[slot] = true;
                maximumSlots = Math.Max(maximumSlots, registered.Count(static value => value));

                var render = angle.ImportAndClear(
                    texturePointer, size.Width, size.Height, 0, 0,
                    drawAlphaGrid: true, generation);
                if (!render.DirectImportSucceeded || !render.MakeCurrentSucceeded ||
                    render.EglError != 0x3000 || render.GlError != 0)
                    throw new InvalidOperationException("ANGLE direct import failed for a P1-CS buffer.");
                var presentResult = Present(
                    context, checked((uint)slot), checked((uint)size.Width),
                    checked((uint)size.Height), checked((ulong)generation),
                    out var presentId, out var retiringFenceValue);
                if (presentResult < 0) Marshal.ThrowExceptionForHR(presentResult);
                ledger.Add(new PresentRecord(
                    generation, slot, size.Width, size.Height, presentId,
                    availableEvent, retiringFenceValue,
                    render.DirectImportSucceeded, render.GpuClearCount,
                    render.UnbindCount, render.TextureFormat, render.TextureBindFlags));
            }
            return new BufferProtocolResult(
                "PASS", presentCount, ledger.Count, maximumSlots, 1,
                availableReuseCount, waitCount, unavailableReuseAttempts,
                0, 0, 0, 0, 0,
                ledger.Count == 0 ? [] : ledger.Take(12).Concat(ledger.TakeLast(12)).ToArray(),
                "free -> rendering -> submitted -> available -> free; IsAvailable/GetAvailableEvent is the final reuse authority.");
        }
        catch (Exception exception)
        {
            return new BufferProtocolResult(
                "FAIL", presentCount, ledger.Count, maximumSlots, 1,
                availableReuseCount, waitCount, unavailableReuseAttempts,
                0, 0, 0, 0, 0,
                ledger.TakeLast(24).ToArray(), exception.ToString());
        }
        finally
        {
            foreach (var texture in textures) texture?.Dispose();
        }
    }

    private static VisibleResult RunVisibleSession(
        AngleDevice angle, nint context, ulong surfaceHandle, Options options)
    {
        nint window = 0;
        DispatcherQueueController? islandDispatcher = null;
        CompositionWorker? composition = null;
        ContentIsland? island = null;
        DesktopAttachedSiteBridge? bridge = null;
        AppWindow? appWindow = null;
        VisibleScene? scene = null;
        VisibleRenderQueue? renderer = null;
        try
        {
            SetProcessDpiAwarenessContext(new nint(-4));
            islandDispatcher = DispatcherQueueController.CreateOnCurrentThread();
            composition = new CompositionWorker();
            var root = composition.Invoke(() => composition.Compositor.CreateContainerVisual());
            RegisterWindowClass();
            window = CreateWindowExW(
                0, WindowClass, "Doroti Acrylic Composition Swapchain P1-CS",
                WsOverlappedWindow | WsVisible,
                140, 120, 900, 620, 0, 0, GetModuleHandleW(null), 0);
            if (window == 0)
                throw new InvalidOperationException($"CreateWindowExW failed: {Marshal.GetLastWin32Error()}.");
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(window);
            appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.AssociateWithDispatcherQueue(islandDispatcher.DispatcherQueue);
            island = ContentIsland.CreateForSystemVisual(islandDispatcher.DispatcherQueue, root);
            bridge = DesktopAttachedSiteBridge.CreateFromWindowId(
                islandDispatcher.DispatcherQueue, windowId);
            bridge.ProcessesPointerInput = false;
            bridge.Connect(island);
            var siteViewChanged = 1;
            island.StateChanged += (_, eventArgs) =>
            {
                if (eventArgs.DidActualSizeChange || eventArgs.DidRasterizationScaleChange)
                    Interlocked.Exchange(ref siteViewChanged, 1);
            };
            var capturedIsland = island;
            scene = composition.Invoke(() => new VisibleScene(
                composition, root, capturedIsland, checked((nint)surfaceHandle)));
            renderer = new VisibleRenderQueue(angle, context, composition, scene);
            _visibleRenderer = renderer;
            UpdateWindow(window);

            var clock = Stopwatch.StartNew();
            var readyWritten = false;
            var lastWidth = 0;
            var lastHeight = 0;
            var lastScale = 0f;
            var nextChurn = TimeSpan.Zero;
            while (clock.Elapsed < options.Duration)
            {
                while (PeekMessageW(out var message, 0, 0, 0, PmRemove))
                {
                    TranslateMessage(in message);
                    DispatchMessageW(in message);
                }
                var siteView = bridge.SiteView;
                if (GetClientRect(window, out var client))
                {
                    var width = client.Right - client.Left;
                    var height = client.Bottom - client.Top;
                    var scale = siteView.RasterizationScale > 0
                        ? siteView.RasterizationScale
                        : Math.Max(1f, GetDpiForWindow(window) / 96f);
                    var actual = new Vector2(width / scale, height / scale);
                    if (width > 0 && height > 0 &&
                        (Interlocked.Exchange(ref siteViewChanged, 0) != 0 ||
                        width != lastWidth || height != lastHeight ||
                        Math.Abs(scale - lastScale) > .0001f))
                    {
                        renderer.Publish(width, height, actual.X, actual.Y, scale);
                        lastWidth = width;
                        lastHeight = height;
                        lastScale = scale;
                    }
                }
                if (clock.Elapsed >= nextChurn)
                {
                    composition.Invoke(scene.ApplyNextBackdrop);
                    nextChurn = clock.Elapsed.Add(TimeSpan.FromMilliseconds(80));
                }
                var queueSnapshot = renderer.Snapshot();
                if (!readyWritten && queueSnapshot.Presented > 0)
                {
                    WriteReport(options.ReadyPath, new
                    {
                        schema = "doroti.windows-acrylic-composition-swapchain-p1cs-ready/v1",
                        hwnd = window.ToInt64(),
                        processId = Environment.ProcessId,
                        title = "Doroti Acrylic Composition Swapchain P1-CS",
                    });
                    readyWritten = true;
                }
                Thread.Sleep(2);
            }
            renderer.Stop();
            var render = renderer.Snapshot();
            var visual = composition.Invoke(scene.Snapshot);
            var status = readyWritten && render.Presented > 0 &&
                render.MaximumQueueDepth <= 2 && render.MaximumSlots <= 3 &&
                render.UnavailableReuseAttempts == 0 && render.WrongSizePresents == 0 &&
                render.DuplicateTerminals == 0 && render.MissingTerminals == 0 &&
                visual.BackdropTargetAdded && visual.ControllerCreateCount == 1 &&
                visual.BackdropApplicationCount >= 5
                ? "PASS-internal"
                : "FAIL";
            return new VisibleResult(
                status, window.ToInt64(), GetDpiForWindow(window),
                island.IsConnected, bridge.ProcessesPointerInput,
                bridge.ProcessesKeyboardInput, render, visual,
                "WGC pixels and physical scan-out remain external gates.");
        }
        catch (Exception exception)
        {
            return new VisibleResult(
                "FAIL", window.ToInt64(), window == 0 ? 0 : GetDpiForWindow(window),
                island?.IsConnected ?? false,
                bridge?.ProcessesPointerInput ?? false,
                bridge?.ProcessesKeyboardInput ?? false,
                renderer?.Snapshot() ?? VisibleRenderSnapshot.Empty,
                scene is not null && composition is not null
                    ? composition.Invoke(scene.Snapshot)
                    : VisibleSceneSnapshot.Empty,
                exception.ToString());
        }
        finally
        {
            _visibleRenderer = null;
            renderer?.Dispose();
            if (scene is not null && composition is not null)
                composition.Invoke(scene.Dispose);
            bridge?.Dispose();
            island?.Dispose();
            appWindow = null;
            composition?.Dispose();
            islandDispatcher?.ShutdownQueue();
            if (window != 0) DestroyWindow(window);
        }
    }

    private sealed class VisibleScene : IDisposable
    {
        private readonly ContentIsland _island;
        private readonly ICompositionSurface _surface;
        private readonly CompositionSurfaceBrush _brush;
        private readonly SpriteVisual _content;
        private readonly ContainerVisual _root;
        private readonly DesktopAcrylicController _backdrop;
        private readonly SystemBackdropConfiguration _configuration;
        private int _backdropIndex;
        private int _backdropApplications;
        private int _geometryApplications;
        private bool _disposed;

        internal VisibleScene(
            CompositionWorker worker,
            ContainerVisual root,
            ContentIsland island,
            nint surfaceHandle)
        {
            _island = island;
            _root = root;
            _surface = CreateCompositionSurfaceForHandle(worker.Compositor, surfaceHandle);
            _brush = worker.Compositor.CreateSurfaceBrush(_surface);
            _brush.Stretch = CompositionStretch.None;
            _brush.HorizontalAlignmentRatio = 0;
            _brush.VerticalAlignmentRatio = 0;
            _content = worker.Compositor.CreateSpriteVisual();
            _content.Brush = _brush;
            _root.Children.InsertAtTop(_content);
            _configuration = new SystemBackdropConfiguration
            {
                IsInputActive = true,
                Theme = SystemBackdropTheme.Dark,
            };
            _backdrop = new DesktopAcrylicController();
            BackdropTargetAdded = _backdrop.AddSystemBackdropTarget(island);
            for (var index = 0; index < 5; index++) ApplyNextBackdrop();
        }

        internal bool BackdropTargetAdded { get; }

        internal void ApplyNextBackdrop()
        {
            _backdrop.ResetProperties();
            switch (_backdropIndex++ % 4)
            {
                case 0:
                    _configuration.Theme = SystemBackdropTheme.Dark;
                    _backdrop.Kind = DesktopAcrylicKind.Default;
                    break;
                case 1:
                    _configuration.Theme = SystemBackdropTheme.Light;
                    _backdrop.Kind = DesktopAcrylicKind.Base;
                    _backdrop.TintColor = Windows.UI.Color.FromArgb(255, 52, 104, 164);
                    _backdrop.TintOpacity = .30f;
                    _backdrop.LuminosityOpacity = .70f;
                    break;
                case 2:
                    _configuration.Theme = SystemBackdropTheme.Dark;
                    _backdrop.Kind = DesktopAcrylicKind.Thin;
                    _backdrop.TintColor = Windows.UI.Color.FromArgb(255, 82, 48, 124);
                    _backdrop.TintOpacity = .52f;
                    _backdrop.LuminosityOpacity = .46f;
                    break;
                default:
                    _configuration.Theme = SystemBackdropTheme.Dark;
                    _backdrop.Kind = DesktopAcrylicKind.Base;
                    _backdrop.TintColor = Windows.UI.Color.FromArgb(255, 44, 84, 132);
                    _backdrop.TintOpacity = .32f;
                    _backdrop.LuminosityOpacity = .68f;
                    break;
            }
            _backdrop.SetSystemBackdropConfiguration(_configuration);
            _backdropApplications++;
        }

        internal void UpdateGeometry(float actualWidth, float actualHeight, float scale)
        {
            _brush.Scale = new Vector2(1f / scale, 1f / scale);
            _content.Size = new Vector2(actualWidth, actualHeight);
            _root.Size = new Vector2(actualWidth, actualHeight);
            _geometryApplications++;
        }

        internal VisibleSceneSnapshot Snapshot() => new(
            BackdropTargetAdded,
            _backdrop.State.ToString(),
            1,
            _backdropApplications,
            _geometryApplications,
            _content.Size.X,
            _content.Size.Y,
            _brush.Scale.X,
            _brush.Scale.Y);

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _backdrop.RemoveSystemBackdropTarget(_island);
            _backdrop.Dispose();
            _content.Dispose();
            _brush.Dispose();
            if (_surface is IDisposable disposable) disposable.Dispose();
        }
    }

    private sealed class VisibleRenderQueue : IDisposable
    {
        private readonly AngleDevice _angle;
        private readonly nint _context;
        private readonly CompositionWorker _composition;
        private readonly VisibleScene _scene;
        private readonly ID3D11Texture2D?[] _textures = new ID3D11Texture2D?[3];
        private readonly object _gate = new();
        private readonly AutoResetEvent _changed = new(false);
        private readonly Thread _thread;
        private readonly List<VisibleTerminal> _terminals = [];
        private readonly HashSet<int> _acceptedGenerations = [];
        private VisibleRequest? _latest;
        private bool _running = true;
        private bool _current;
        private int _generation;
        private int _accepted;
        private int _presented;
        private int _superseded;
        private int _failed;
        private int _maximumQueueDepth;
        private int _availableReuseCount;
        private int _waitCount;
        private int _unavailableReuseAttempts;
        private int _maximumSlots = 3;

        internal VisibleRenderQueue(
            AngleDevice angle,
            nint context,
            CompositionWorker composition,
            VisibleScene scene)
        {
            _angle = angle;
            _context = context;
            _composition = composition;
            _scene = scene;
            _thread = new Thread(Run)
            {
                IsBackground = true,
                Name = "Doroti P1-CS available-event render worker",
            };
            _thread.Start();
        }

        internal void Publish(
            int width, int height, float actualWidth, float actualHeight, float scale)
        {
            VisibleRequest? superseded = null;
            lock (_gate)
            {
                var request = new VisibleRequest(
                    ++_generation, width, height, actualWidth, actualHeight, scale,
                    Stopwatch.GetTimestamp());
                _accepted++;
                _acceptedGenerations.Add(request.Generation);
                superseded = _latest;
                _latest = request;
                _maximumQueueDepth = Math.Max(_maximumQueueDepth, (_current ? 1 : 0) + 1);
            }
            if (superseded is not null)
            {
                lock (_gate)
                {
                    _terminals.Add(new VisibleTerminal(
                        superseded.Generation, "superseded", -1,
                        superseded.Width, superseded.Height, 0,
                        superseded.RequestedQpc, Stopwatch.GetTimestamp()));
                    _superseded++;
                }
            }
            _changed.Set();
        }

        private void Run()
        {
            while (true)
            {
                _changed.WaitOne();
                VisibleRequest? request;
                lock (_gate)
                {
                    if (!_running && _latest is null) return;
                    request = _latest;
                    _latest = null;
                    _current = request is not null;
                }
                if (request is null) continue;
                Render(request);
                lock (_gate)
                {
                    _current = false;
                    if (_latest is not null) _changed.Set();
                }
            }
        }

        private void Render(VisibleRequest request)
        {
            try
            {
                var slot = -1;
                for (var index = 0; index < 3; index++)
                {
                    var result = IsAvailable(_context, checked((uint)index), out var available);
                    if (result < 0) Marshal.ThrowExceptionForHR(result);
                    if (available != 0)
                    {
                        slot = index;
                        _availableReuseCount++;
                        break;
                    }
                }
                if (slot < 0)
                {
                    _waitCount++;
                    var wait = WaitForAvailable(_context, 1000, out var availableSlot);
                    if (wait < 0) Marshal.ThrowExceptionForHR(wait);
                    slot = checked((int)availableSlot);
                }
                var recheck = IsAvailable(_context, checked((uint)slot), out var reusable);
                if (recheck < 0) Marshal.ThrowExceptionForHR(recheck);
                if (reusable == 0)
                {
                    _unavailableReuseAttempts++;
                    throw new InvalidOperationException("Available-event slot was unsignaled before reuse.");
                }
                _textures[slot]?.Dispose();
                var bufferSnapshot = new BufferSnapshot
                {
                    AbiVersion = 1,
                    StructSize = checked((uint)Marshal.SizeOf<BufferSnapshot>()),
                };
                var replace = ReplaceBuffer(
                    _context, checked((uint)slot), checked((uint)request.Width),
                    checked((uint)request.Height), out var texture,
                    out _, ref bufferSnapshot);
                if (replace < 0) Marshal.ThrowExceptionForHR(replace);
                _textures[slot] = new ID3D11Texture2D(texture);
                var render = _angle.ImportAndClear(
                    texture, request.Width, request.Height, 0, 0,
                    drawAlphaGrid: true, request.Generation, request.Scale);
                if (!render.DirectImportSucceeded || render.EglError != 0x3000 || render.GlError != 0)
                    throw new InvalidOperationException("ANGLE failed to render the visible P1-CS buffer.");
                lock (_gate)
                {
                    if (_latest is { Generation: > 0 } latest && latest.Generation > request.Generation)
                    {
                        _terminals.Add(new VisibleTerminal(
                            request.Generation, "superseded-before-present", slot,
                            request.Width, request.Height, 0,
                            request.RequestedQpc, Stopwatch.GetTimestamp()));
                        _superseded++;
                        return;
                    }
                }
                var present = Present(
                    _context, checked((uint)slot), checked((uint)request.Width),
                    checked((uint)request.Height), checked((ulong)request.Generation),
                    out var presentId, out _);
                if (present < 0) Marshal.ThrowExceptionForHR(present);
                _composition.Invoke(() =>
                    _scene.UpdateGeometry(
                        request.ActualWidth, request.ActualHeight, request.Scale));
                lock (_gate)
                {
                    _presented++;
                    _terminals.Add(new VisibleTerminal(
                        request.Generation, "presented", slot,
                        request.Width, request.Height, presentId,
                        request.RequestedQpc, Stopwatch.GetTimestamp()));
                }
            }
            catch (Exception exception)
            {
                lock (_gate)
                {
                    _failed++;
                    _terminals.Add(new VisibleTerminal(
                        request.Generation, $"failed:{exception.GetType().Name}:{exception.Message}",
                        -1, request.Width, request.Height, 0,
                        request.RequestedQpc, Stopwatch.GetTimestamp()));
                }
            }
        }

        internal void Stop()
        {
            lock (_gate) _running = false;
            _changed.Set();
            if (!_thread.Join(TimeSpan.FromSeconds(15)))
                throw new TimeoutException("P1-CS visible render worker did not stop.");
        }

        internal VisibleRenderSnapshot Snapshot()
        {
            lock (_gate)
            {
                var groups = _terminals.GroupBy(item => item.Generation).ToArray();
                var terminalGenerations = groups.Select(group => group.Key).ToHashSet();
                return new VisibleRenderSnapshot(
                    _accepted, _presented, _superseded, _failed,
                    _maximumQueueDepth, _maximumSlots,
                    _availableReuseCount, _waitCount, _unavailableReuseAttempts,
                    0,
                    groups.Count(group => group.Count() != 1),
                    _acceptedGenerations.Count(generation => !terminalGenerations.Contains(generation)),
                    _terminals.ToArray());
            }
        }

        public void Dispose()
        {
            if (_running) Stop();
            foreach (var texture in _textures) texture?.Dispose();
            _changed.Dispose();
        }
    }

    private sealed record VisibleRequest(
        int Generation, int Width, int Height,
        float ActualWidth, float ActualHeight, float Scale, long RequestedQpc);

    private sealed record VisibleTerminal(
        int Generation, string Status, int Slot, int Width, int Height,
        ulong PresentId, long RequestedQpc, long TerminalQpc);

    private sealed record VisibleRenderSnapshot(
        int Accepted,
        int Presented,
        int Superseded,
        int Failed,
        int MaximumQueueDepth,
        int MaximumSlots,
        int AvailableReuseCount,
        int WaitCount,
        int UnavailableReuseAttempts,
        int WrongSizePresents,
        int DuplicateTerminals,
        int MissingTerminals,
        IReadOnlyList<VisibleTerminal> TerminalSample)
    {
        internal static VisibleRenderSnapshot Empty { get; } = new(
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, []);
    }

    private sealed record VisibleSceneSnapshot(
        bool BackdropTargetAdded,
        string ControllerState,
        int ControllerCreateCount,
        int BackdropApplicationCount,
        int GeometryApplicationCount,
        float ActualWidth,
        float ActualHeight,
        float BrushScaleX,
        float BrushScaleY)
    {
        internal static VisibleSceneSnapshot Empty { get; } = new(
            false, "unavailable", 0, 0, 0, 0, 0, 0, 0);
    }

    private sealed record VisibleResult(
        string Status,
        long TopHwnd,
        uint Dpi,
        bool IslandConnected,
        bool BridgeProcessesPointerInput,
        bool BridgeProcessesKeyboardInput,
        VisibleRenderSnapshot Render,
        VisibleSceneSnapshot Scene,
        string Boundary)
    {
        internal static VisibleResult NotRun { get; } = new(
            "notRun", 0, 0, false, false, false,
            VisibleRenderSnapshot.Empty, VisibleSceneSnapshot.Empty,
            "Visible mode was not requested.");
    }

    private static nint HandleWindowMessage(
        nint hwnd, uint message, nint wParam, nint lParam)
    {
        if (message == WmSize)
        {
            var packed = unchecked((ulong)lParam.ToInt64());
            var width = checked((int)(packed & 0xFFFF));
            var height = checked((int)((packed >> 16) & 0xFFFF));
            if (width > 0 && height > 0)
            {
                var scale = Math.Max(1f, GetDpiForWindow(hwnd) / 96f);
                _visibleRenderer?.Publish(
                    width, height, width / scale, height / scale, scale);
            }
        }
        else if (message == WmDestroy) PostQuitMessage(0);
        return DefWindowProcW(hwnd, message, wParam, lParam);
    }

    private static void RegisterWindowClass()
    {
        var definition = new WndClassEx
        {
            Size = checked((uint)Marshal.SizeOf<WndClassEx>()),
            Instance = GetModuleHandleW(null),
            Cursor = LoadCursorW(0, new nint(32512)),
            ClassName = WindowClass,
            Procedure = Marshal.GetFunctionPointerForDelegate(WindowProcedure),
        };
        if (RegisterClassExW(in definition) == 0)
            throw new InvalidOperationException(
                $"RegisterClassExW failed: {Marshal.GetLastWin32Error()}.");
    }

    private static unsafe ICompositionSurface CreateCompositionSurfaceForHandle(
        Compositor compositor, nint surfaceHandle)
    {
        var iid = new Guid("25297D5C-3AD4-4C9C-B5CF-E36A38512330");
        using var interop = ((WinRT.IWinRTObject)compositor).NativeObject.As(iid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var create = (delegate* unmanaged[Stdcall]<nint, nint, nint*, int>)vtable[3];
        nint result = 0;
        var hresult = create(thisPointer, surfaceHandle, &result);
        if (hresult < 0) Marshal.ThrowExceptionForHR(hresult);
        try { return WinRT.MarshalInterface<ICompositionSurface>.FromAbi(result); }
        finally { Marshal.Release(result); }
    }

    private sealed record Options(string ReportPath, string ReadyPath, TimeSpan Duration)
    {
        internal static Options Parse(string[] args)
        {
            string? report = null;
            string? ready = null;
            var durationSeconds = 0;
            for (var index = 0; index < args.Length; index++)
            {
                switch (args[index])
                {
                    case "--report": report = args[++index]; break;
                    case "--ready-file": ready = args[++index]; break;
                    case "--duration": durationSeconds = int.Parse(args[++index]); break;
                    default: throw new ArgumentException($"Unknown argument: {args[index]}");
                }
            }
            var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            report ??= Path.Combine(".doroti", "evidence", $"p1cs-{stamp}.json");
            ready ??= Path.Combine(".doroti", "evidence", $"p1cs-{stamp}.ready.json");
            return new Options(
                Path.GetFullPath(report),
                Path.GetFullPath(ready),
                durationSeconds <= 0
                    ? TimeSpan.Zero
                    : TimeSpan.FromSeconds(Math.Clamp(durationSeconds, 3, 60)));
        }
    }

    private static int WriteFailure(string path, string failure)
    {
        WriteReport(path, new
        {
            schema = "doroti.windows-acrylic-composition-swapchain-p1cs/v1",
            status = "FAIL",
            failure,
            evidence = new { capability = "FAIL", physical = "notVerified" },
        });
        Console.Error.WriteLine(failure);
        Console.Error.WriteLine($"report={path}");
        return 1;
    }

    private static void WriteReport(string path, object value)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
        }));
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ProbeSnapshot
    {
        public uint AbiVersion;
        public uint StructSize;
        public int FactoryHresult;
        public int ManagerHresult;
        public int SurfaceHandleHresult;
        public int PresentationSurfaceHresult;
        public int RetiringFenceHresult;
        public uint DeviceCreationFlags;
        public uint PresentationSupported;
        public uint IndependentFlipSupported;
        public int AdapterLuidLow;
        public int AdapterLuidHigh;
        public uint AdapterVendorId;
        public uint AdapterDeviceId;
        public ulong RetiringFenceCompletedValue;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BufferSnapshot
    {
        public uint AbiVersion;
        public uint StructSize;
        public int TextureHresult;
        public int AddBufferHresult;
        public int AvailableEventHresult;
        public uint Width;
        public uint Height;
        public uint Format;
        public uint BindFlags;
        public uint MiscFlags;
        public uint InitiallyAvailable;
    }

    internal sealed record PresentRecord(
        int Generation,
        int Slot,
        int Width,
        int Height,
        ulong PresentId,
        ulong AvailableEvent,
        ulong RetiringFenceValue,
        bool DirectImport,
        int GpuClearCount,
        int UnbindCount,
        string TextureFormat,
        string TextureBindFlags);

    internal sealed record BufferProtocolResult(
        string Status,
        int Requested,
        int Presented,
        int MaximumSlots,
        int MaximumQueueDepth,
        int AvailableReuseCount,
        int WaitCount,
        int UnavailableReuseAttempts,
        int WrongSizePresents,
        int StalePresents,
        int DuplicateTerminals,
        int MissingTerminals,
        int CpuCopyCount,
        IReadOnlyList<PresentRecord> LedgerSample,
        string Boundary)
    {
        internal static BufferProtocolResult NotRun { get; } = new(
            "notRun", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, [],
            "Capability did not authorize buffer work.");
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        internal int X;
        internal int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Message
    {
        internal nint Hwnd;
        internal uint Value;
        internal nuint WParam;
        internal nint LParam;
        internal uint Time;
        internal Point Location;
        internal uint Private;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate nint WndProc(nint hwnd, uint message, nint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WndClassEx
    {
        internal uint Size;
        internal uint Style;
        internal nint Procedure;
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

    [LibraryImport("doroti_p1cs_native", EntryPoint = "doroti_p1cs_probe_create")]
    private static partial int ProbeCreate(
        nint d3d11Device, out nint context, out ulong compositionSurfaceHandle,
        ref ProbeSnapshot snapshot);

    [LibraryImport("doroti_p1cs_native", EntryPoint = "doroti_p1cs_probe_destroy")]
    private static partial void ProbeDestroy(nint context);

    [LibraryImport("doroti_p1cs_native", EntryPoint = "doroti_p1cs_replace_buffer")]
    private static partial int ReplaceBuffer(
        nint context, uint slotIndex, uint width, uint height,
        out nint texture, out ulong availableEvent, ref BufferSnapshot snapshot);

    [LibraryImport("doroti_p1cs_native", EntryPoint = "doroti_p1cs_is_available")]
    private static partial int IsAvailable(nint context, uint slotIndex, out uint available);

    [LibraryImport("doroti_p1cs_native", EntryPoint = "doroti_p1cs_present")]
    private static partial int Present(
        nint context, uint slotIndex, uint width, uint height, ulong tag,
        out ulong presentId, out ulong retiringFenceValue);

    [LibraryImport("doroti_p1cs_native", EntryPoint = "doroti_p1cs_wait_for_available")]
    private static partial int WaitForAvailable(
        nint context, uint timeoutMilliseconds, out uint slotIndex);

    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint CreateWindowExW(
        uint extendedStyle, string className, string title, uint style,
        int x, int y, int width, int height,
        nint parent, nint menu, nint instance, nint parameter);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandleW(string? moduleName);

    [DllImport("user32.dll", EntryPoint = "RegisterClassExW", SetLastError = true,
        CharSet = CharSet.Unicode)]
    private static extern ushort RegisterClassExW(in WndClassEx definition);

    [LibraryImport("user32.dll")]
    private static partial nint DefWindowProcW(
        nint window, uint message, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    private static partial nint LoadCursorW(nint instance, nint cursorName);

    [LibraryImport("user32.dll")]
    private static partial void PostQuitMessage(int exitCode);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DestroyWindow(nint window);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UpdateWindow(nint window);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetClientRect(nint window, out Rect rect);

    [LibraryImport("user32.dll")]
    private static partial uint GetDpiForWindow(nint window);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetProcessDpiAwarenessContext(nint context);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PeekMessageW(
        out Message message, nint window, uint minimum, uint maximum, uint remove);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool TranslateMessage(in Message message);

    [LibraryImport("user32.dll")]
    private static partial nint DispatchMessageW(in Message message);

    [LibraryImport("combase.dll")]
    private static partial int RoInitialize(uint initType);

    [LibraryImport("combase.dll")]
    private static partial void RoUninitialize();
}
