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
using Windows.Graphics;
using Windows.Graphics.DirectX;
using Windows.UI.Composition;

namespace Doroti.Validation.WindowsAcrylicCompositionSpike;

internal static partial class Program
{
    private const uint WsOverlappedWindow = 0x00CF0000;
    private const uint WsVisible = 0x10000000;
    private const uint SwpNoZOrder = 0x0004;
    private const uint PmRemove = 0x0001;
    private const uint WmDestroy = 0x0002;
    private const uint WmSize = 0x0005;
    private const int SwShow = 5;
    private static readonly string WindowClass = $"DorotiAcrylicCompositionSpike-{Environment.ProcessId}";
    private static readonly WndProc WindowProcedure = HandleWindowMessage;
    private static RenderCoordinator? _coordinator;
    private static DesktopAttachedSiteBridge? _siteBridge;

    [STAThread]
    private static int Main(string[] args)
    {
        var options = Options.Parse(args);
        var roResult = RoInitialize(0);
        if (roResult < 0)
        {
            Console.Error.WriteLine($"RoInitialize failed: 0x{roResult:X8}");
            return 1;
        }

        nint window = 0;
        DispatcherQueueController? islandDispatcher = null;
        CompositionWorker? composition = null;
        SceneSession? scene = null;
        ContentIsland? island = null;
        DesktopAttachedSiteBridge? bridge = null;
        AppWindow? appWindow = null;
        RenderCoordinator? coordinator = null;
        try
        {
            SetProcessDpiAwarenessContext(new nint(-4));
            RegisterWindowClass();
            islandDispatcher = DispatcherQueueController.CreateOnCurrentThread();
            composition = new CompositionWorker();
            var root = composition.Invoke(() => composition.Compositor.CreateContainerVisual());

            window = CreateWindowExW(
                0, WindowClass, "Doroti Acrylic Composition B1/B2",
                WsOverlappedWindow | WsVisible,
                120, 120, 820, 560, 0, 0, GetModuleHandleW(null), 0);
            if (window == 0)
                throw new InvalidOperationException($"CreateWindowExW failed: {Marshal.GetLastWin32Error()}.");

            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(window);
            appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.AssociateWithDispatcherQueue(islandDispatcher.DispatcherQueue);
            island = ContentIsland.CreateForSystemVisual(islandDispatcher.DispatcherQueue, root);
            bridge = DesktopAttachedSiteBridge.CreateFromWindowId(islandDispatcher.DispatcherQueue, windowId);
            bridge.ProcessesPointerInput = false;
            bridge.Connect(island);
            _siteBridge = bridge;
            island.StateChanged += (_, eventArgs) =>
            {
                if (eventArgs.DidActualSizeChange || eventArgs.DidRasterizationScaleChange)
                    PublishAppliedSiteView();
            };

            scene = composition.Invoke(() => new SceneSession(composition, root, island));
            coordinator = new RenderCoordinator(composition, scene);
            _coordinator = coordinator;
            RecordCurrentTarget(window);

            ShowWindow(window, SwShow);
            UpdateWindow(window);
            PublishAppliedSiteView();
            WriteReady(options.ReadyPath, window, Environment.ProcessId);

            var clock = Stopwatch.StartNew();
            var resizeIndex = 0;
            var schedule = new[]
            {
                (TimeSpan.FromMilliseconds(700), 760, 510),
                (TimeSpan.FromMilliseconds(1400), 930, 610),
                (TimeSpan.FromMilliseconds(2100), 700, 470),
            };
            while (clock.Elapsed < options.Duration)
            {
                while (PeekMessageW(out var message, 0, 0, 0, PmRemove))
                {
                    TranslateMessage(in message);
                    DispatchMessageW(in message);
                }

                if (resizeIndex < schedule.Length && clock.Elapsed >= schedule[resizeIndex].Item1)
                {
                    var step = schedule[resizeIndex++];
                    SetWindowPos(window, 0, 0, 0, step.Item2, step.Item3, SwpNoZOrder);
                }
                Thread.Sleep(4);
            }

            coordinator.Stop();
            var session = composition.Invoke(scene.Snapshot);
            var coordination = coordinator.Snapshot();
            var b1Pass = session.BackdropTargetAdded && session.SuccessfulCommits == 3 &&
                         session.ControllerCreateCount == 1 && session.AddTargetCount == 1 &&
                         session.BackdropApplications.Count == 5 &&
                         session.CpuReadbackCount == 0 && session.SurfacePoolSize == 3 &&
                         coordination.MaxQueueDepth <= 2 && coordination.DuplicateTerminals == 0 &&
                         coordination.MissingTerminals == 0;
            var b2Pass = b1Pass && session.SafeRetirementProven && session.SafeReuseCount > 0;
            var report = new SpikeReport(
                "doroti.windows-acrylic-composition-b1/v1",
                b1Pass ? "PASS-capability" : "FAIL",
                b2Pass ? "PASS" : "FAIL",
                b2Pass ? "candidate" : "FAIL",
                "Microsoft.WindowsAppSDK/2.4.0",
                Environment.OSVersion.VersionString,
                window.ToInt64(),
                GetDpiForWindow(window),
                Environment.CurrentManagedThreadId,
                composition.ThreadId,
                island.IsConnected,
                bridge.ProcessesPointerInput,
                bridge.ProcessesKeyboardInput,
                session,
                coordination,
                new EvidenceBoundary(
                    "PASS",
                    "notVerified",
                    "notVerified",
                    "notVerified",
                    "Top HWND input-owner candidate only; pointer/key/focus/IME/UIA duplication, device loss, visible pixels, physical drag, and scan-out were not accepted."),
                b2Pass
                    ? "No blocking condition observed."
                    : "No documented compositor-retirement/acquire signal was proven before a retired CompositionDrawingSurface would need mutation. The pool is capped at three and reuse is refused.");
            WriteJson(options.ReportPath, report);
            Console.WriteLine(
                $"B1={report.B1Status} B2={report.B2Status} P1={report.P1Status} " +
                $"commits={session.SuccessfulCommits} pool={session.SurfacePoolSize} " +
                $"safeReuse={session.SafeReuseCount} noSafeSlot={session.NoSafeSlotFailures}");
            Console.WriteLine($"report={options.ReportPath}");
            return b2Pass ? 0 : 2;
        }
        catch (Exception exception)
        {
            WriteJson(options.ReportPath, new
            {
                schema = "doroti.windows-acrylic-composition-b1/v1",
                b1Status = "FAIL",
                b2Status = "FAIL",
                p1Status = "FAIL",
                exception = exception.ToString(),
                visible = "notVerified",
                physical = "notVerified",
            });
            Console.Error.WriteLine(exception);
            return 1;
        }
        finally
        {
            _coordinator = null;
            _siteBridge = null;
            coordinator?.Dispose();
            if (scene is not null && composition is not null)
                composition.Invoke(scene.Dispose);
            bridge?.Dispose();
            island?.Dispose();
            appWindow = null;
            composition?.Dispose();
            islandDispatcher?.ShutdownQueue();
            if (window != 0) DestroyWindow(window);
            RoUninitialize();
        }
    }

    private static void RecordCurrentTarget(nint window)
    {
        if (!GetClientRect(window, out var client)) return;
        RecordTarget(client.Right - client.Left, client.Bottom - client.Top);
    }

    private static void RecordTarget(int width, int height)
    {
        if (width <= 0 || height <= 0) return;
        _coordinator?.RecordTarget(width, height);
    }

    private static void PublishAppliedSiteView()
    {
        var bridge = _siteBridge;
        var coordinator = _coordinator;
        if (bridge is null || coordinator is null) return;
        var siteView = bridge.SiteView;
        var client = siteView.ClientSize;
        if (client.Width <= 0 || client.Height <= 0) return;
        var scale = siteView.RasterizationScale > 0 ? siteView.RasterizationScale : 1f;
        var actual = siteView.ActualSize;
        if (actual.X <= 0 || actual.Y <= 0)
            actual = new Vector2(client.Width / scale, client.Height / scale);
        coordinator.PublishApplied(client.Width, client.Height, actual.X, actual.Y, scale);
    }

    private static nint HandleWindowMessage(nint hwnd, uint message, nint wParam, nint lParam)
    {
        if (message == WmSize)
        {
            var packed = unchecked((ulong)lParam.ToInt64());
            RecordTarget((int)(packed & 0xFFFF), (int)((packed >> 16) & 0xFFFF));
        }
        else if (message == WmDestroy)
        {
            PostQuitMessage(0);
        }
        return DefWindowProcW(hwnd, message, wParam, lParam);
    }

    private static void RegisterWindowClass()
    {
        var definition = new WndClassEx
        {
            Size = (uint)Marshal.SizeOf<WndClassEx>(),
            Instance = GetModuleHandleW(null),
            Cursor = LoadCursorW(0, new nint(32512)),
            ClassName = WindowClass,
            Procedure = Marshal.GetFunctionPointerForDelegate(WindowProcedure),
        };
        if (RegisterClassExW(in definition) == 0)
            throw new InvalidOperationException($"RegisterClassExW failed: {Marshal.GetLastWin32Error()}.");
    }

    private static void WriteReady(string path, nint hwnd, int processId) =>
        WriteJson(path, new { hwnd = hwnd.ToInt64(), processId, title = "Doroti Acrylic Composition B1/B2" });

    private static void WriteJson(string path, object value)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(value, JsonOptions));
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private sealed record Options(string ReportPath, string ReadyPath, TimeSpan Duration)
    {
        internal static Options Parse(string[] args)
        {
            string? report = null;
            string? ready = null;
            var duration = 5;
            for (var index = 0; index < args.Length; index++)
            {
                switch (args[index])
                {
                    case "--report": report = args[++index]; break;
                    case "--ready-file": ready = args[++index]; break;
                    case "--duration": duration = int.Parse(args[++index]); break;
                    default: throw new ArgumentException($"Unknown argument: {args[index]}");
                }
            }
            var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            report ??= Path.Combine(".doroti", "evidence", $"acrylic-composition-b1-{stamp}.json");
            ready ??= Path.Combine(".doroti", "evidence", $"acrylic-composition-b1-{stamp}.ready.json");
            return new Options(Path.GetFullPath(report), Path.GetFullPath(ready),
                TimeSpan.FromSeconds(Math.Clamp(duration, 3, 60)));
        }
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

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect { internal int Left, Top, Right, Bottom; }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point { internal int X, Y; }

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

    [LibraryImport("combase.dll")]
    private static partial int RoInitialize(uint initType);
    [LibraryImport("combase.dll")]
    private static partial void RoUninitialize();
    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandleW(string? moduleName);
    [DllImport("user32.dll", EntryPoint = "RegisterClassExW", SetLastError = true,
        CharSet = CharSet.Unicode)]
    private static extern ushort RegisterClassExW(in WndClassEx definition);
    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint CreateWindowExW(
        uint extendedStyle, string className, string title, uint style,
        int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DestroyWindow(nint window);
    [LibraryImport("user32.dll")]
    private static partial nint DefWindowProcW(nint hwnd, uint message, nint wParam, nint lParam);
    [LibraryImport("user32.dll")]
    private static partial nint LoadCursorW(nint instance, nint cursor);
    [LibraryImport("user32.dll")]
    private static partial int ShowWindow(nint window, int command);
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UpdateWindow(nint window);
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowPos(
        nint window, nint insertAfter, int x, int y, int width, int height, uint flags);
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
    private static partial bool PeekMessageW(out Message message, nint window, uint min, uint max, uint remove);
    [LibraryImport("user32.dll")]
    private static partial nint DispatchMessageW(in Message message);
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool TranslateMessage(in Message message);
    [LibraryImport("user32.dll")]
    private static partial void PostQuitMessage(int exitCode);
}

internal sealed class SceneSession : IDisposable
{
    private readonly CompositionWorker _worker;
    private readonly ContainerVisual _root;
    private readonly ContentIsland _island;
    private readonly AngleDevice _angle;
    private readonly CompositionGraphicsDevice _graphicsDevice;
    private readonly CompositionSurfaceBrush _brush;
    private readonly SpriteVisual _content;
    private readonly DesktopAcrylicController _backdrop;
    private readonly SystemBackdropConfiguration _configuration;
    private readonly List<SurfaceSlot> _slots = [];
    private readonly List<BackdropSnapshot> _backdropApplications = [];
    private bool _disposed;
    private int _frontSlot = -1;
    private int _noSafeSlotFailures;
    private int _successfulCommits;
    private int _beginDrawCount;
    private int _endDrawCount;
    private int _gpuClearCount;

    internal SceneSession(
        CompositionWorker worker,
        ContainerVisual root,
        ContentIsland island)
    {
        _worker = worker;
        _root = root;
        _island = island;
        _angle = new AngleDevice();
        _graphicsDevice = CompositionInterop.CreateGraphicsDevice(
            worker.Compositor, _angle.D3D11DevicePointer);
        _brush = worker.Compositor.CreateSurfaceBrush();
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
        ApplyBackdrop("system-default", DesktopAcrylicKind.Default, SystemBackdropTheme.Default,
            null, null, null);
        ApplyBackdrop("base-light", DesktopAcrylicKind.Base, SystemBackdropTheme.Light,
            Windows.UI.Color.FromArgb(255, 66, 112, 168), .30f, .72f);
        ApplyBackdrop("thin-dark", DesktopAcrylicKind.Thin, SystemBackdropTheme.Dark,
            Windows.UI.Color.FromArgb(255, 92, 54, 128), .52f, .46f);
        ApplyBackdrop("reset-default", DesktopAcrylicKind.Default, SystemBackdropTheme.Dark,
            null, null, null);
        ApplyBackdrop("base-final", DesktopAcrylicKind.Base, SystemBackdropTheme.Dark,
            Windows.UI.Color.FromArgb(255, 44, 84, 132), .32f, .68f);
    }

    internal bool BackdropTargetAdded { get; }

    internal RenderOutcome Render(
        int generation,
        int width,
        int height,
        float actualWidth,
        float actualHeight,
        float rasterizationScale)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_slots.Count == 3)
        {
            _noSafeSlotFailures++;
            return new RenderOutcome(false, -1, "failed-safe-retirement-unproven");
        }

        var slotId = _slots.Count;
        var surface = _graphicsDevice.CreateDrawingSurface(
            new Windows.Foundation.Size(width, height),
            DirectXPixelFormat.B8G8R8A8UIntNormalized,
            DirectXAlphaMode.Premultiplied);
        var slot = new SurfaceSlot(
            slotId, generation, width, height,
            actualWidth, actualHeight, rasterizationScale, surface);
        _slots.Add(slot);

        var draw = CompositionInterop.BeginDraw(surface);
        _beginDrawCount++;
        ID3D11Texture2D? texture = null;
        try
        {
            texture = new ID3D11Texture2D(draw.TexturePointer);
            draw.TexturePointer = 0;
            var result = _angle.ImportAndClear(
                texture.NativePointer, width, height, draw.Offset.X, draw.Offset.Y,
                drawAlphaGrid: true, generation);
            if (!result.DirectImportSucceeded || !result.MakeCurrentSucceeded ||
                result.GlError != 0 || result.EglError != 0x3000)
                return new RenderOutcome(false, slotId, "failed-angle-import");
            _gpuClearCount += result.GpuClearCount;
        }
        finally
        {
            texture?.Dispose();
            if (draw.TexturePointer != 0) Marshal.Release(draw.TexturePointer);
            CompositionInterop.EndDraw(draw);
            _endDrawCount++;
        }

        _brush.Surface = surface;
        _brush.Scale = new Vector2(1f / rasterizationScale, 1f / rasterizationScale);
        _content.Size = new Vector2(actualWidth, actualHeight);
        _root.Size = new Vector2(actualWidth, actualHeight);
        _frontSlot = slotId;
        _successfulCommits++;
        return new RenderOutcome(true, slotId, "committed-not-scanout");
    }

    internal SceneSnapshot Snapshot() => new(
        BackdropTargetAdded,
        _backdrop.State.ToString(),
        _slots.Count,
        _frontSlot,
        _successfulCommits,
        0,
        false,
        _noSafeSlotFailures,
        _beginDrawCount,
        _endDrawCount,
        _gpuClearCount,
        0,
        0,
        0,
        1,
        1,
        _backdropApplications.ToArray(),
        _slots.Select(slot => new SlotSnapshot(
            slot.Id, slot.Generation, slot.Width, slot.Height,
            slot.ActualWidth, slot.ActualHeight, slot.RasterizationScale,
            slot.Id == _frontSlot ? "visible-front" : "retained-retiring")).ToArray(),
        "CompositionDrawingSurface is persistent; BeginDraw update texture and POINT are transient and released before EndDraw.",
        "No retired slot is resized or redrawn because a documented compositor-retirement/acquire signal was not established.");

    private void ApplyBackdrop(
        string name,
        DesktopAcrylicKind kind,
        SystemBackdropTheme theme,
        Windows.UI.Color? tint,
        float? tintOpacity,
        float? luminosityOpacity)
    {
        _configuration.Theme = theme;
        _backdrop.ResetProperties();
        _backdrop.Kind = kind;
        if (tint is Windows.UI.Color tintColor) _backdrop.TintColor = tintColor;
        if (tintOpacity is float tintValue) _backdrop.TintOpacity = tintValue;
        if (luminosityOpacity is float luminosityValue)
            _backdrop.LuminosityOpacity = luminosityValue;
        _backdrop.SetSystemBackdropConfiguration(_configuration);
        _backdropApplications.Add(new BackdropSnapshot(
            name,
            _backdrop.Kind.ToString(),
            _configuration.Theme.ToString(),
            ((uint)_backdrop.TintColor.R << 16) |
            ((uint)_backdrop.TintColor.G << 8) |
            _backdrop.TintColor.B,
            _backdrop.TintOpacity,
            _backdrop.LuminosityOpacity));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _backdrop.RemoveSystemBackdropTarget(_island);
        _backdrop.Dispose();
        _content.Dispose();
        _brush.Dispose();
        foreach (var slot in _slots) slot.Surface.Dispose();
        _slots.Clear();
        _graphicsDevice.Dispose();
        _angle.Dispose();
    }

    private sealed record SurfaceSlot(
        int Id,
        int Generation,
        int Width,
        int Height,
        float ActualWidth,
        float ActualHeight,
        float RasterizationScale,
        CompositionDrawingSurface Surface);
}

internal sealed class RenderCoordinator : IDisposable
{
    private readonly CompositionWorker _composition;
    private readonly SceneSession _scene;
    private readonly object _gate = new();
    private readonly AutoResetEvent _changed = new(false);
    private readonly Thread _thread;
    private readonly List<RenderTerminal> _terminals = [];
    private ResizeRequest? _latest;
    private bool _running = true;
    private bool _current;
    private int _generation;
    private int _accepted;
    private int _maxQueueDepth;
    private int _targetWidth;
    private int _targetHeight;
    private int _targetCount;
    private int _appliedMismatchCount;
    private int _lastAdmittedGeneration;
    private readonly HashSet<int> _acceptedGenerations = [];

    internal RenderCoordinator(CompositionWorker composition, SceneSession scene)
    {
        _composition = composition;
        _scene = scene;
        _thread = new Thread(Run) { IsBackground = true, Name = "Doroti acrylic B1 coordinator" };
        _thread.Start();
    }

    internal void RecordTarget(int width, int height)
    {
        lock (_gate)
        {
            _targetWidth = width;
            _targetHeight = height;
            _targetCount++;
            _generation++;
        }
    }

    internal void PublishApplied(
        int width,
        int height,
        float actualWidth,
        float actualHeight,
        float rasterizationScale)
    {
        ResizeRequest request;
        lock (_gate)
        {
            if (width != _targetWidth || height != _targetHeight)
            {
                _appliedMismatchCount++;
                return;
            }
            if (_generation == _lastAdmittedGeneration) return;
            _lastAdmittedGeneration = _generation;
            request = new ResizeRequest(
                _generation, width, height, actualWidth, actualHeight,
                rasterizationScale, Stopwatch.GetTimestamp());
            _accepted++;
            _acceptedGenerations.Add(request.Generation);
            if (_latest is not null)
                _terminals.Add(new RenderTerminal(
                    _latest.Generation, "superseded", -1, _latest.Width, _latest.Height));
            _latest = request;
            _maxQueueDepth = Math.Max(_maxQueueDepth, (_current ? 1 : 0) + 1);
        }
        _changed.Set();
    }

    private void Run()
    {
        while (true)
        {
            _changed.WaitOne();
            ResizeRequest? request;
            lock (_gate)
            {
                if (!_running && _latest is null) return;
                request = _latest;
                _latest = null;
                _current = request is not null;
            }
            if (request is null) continue;

            RenderOutcome outcome;
            try
            {
                outcome = _composition.Invoke(() =>
                    _scene.Render(
                        request.Generation, request.Width, request.Height,
                        request.ActualWidth, request.ActualHeight, request.RasterizationScale));
            }
            catch (Exception exception)
            {
                outcome = new RenderOutcome(false, -1, $"failed:{exception.GetType().Name}");
            }
            lock (_gate)
            {
                _terminals.Add(new RenderTerminal(
                    request.Generation, outcome.Terminal, outcome.SlotId,
                    request.Width, request.Height));
                _current = false;
                if (_latest is not null) _changed.Set();
            }
        }
    }

    internal void Stop()
    {
        lock (_gate) _running = false;
        _changed.Set();
        if (!_thread.Join(TimeSpan.FromSeconds(15)))
            throw new TimeoutException("Render coordinator did not stop.");
    }

    internal CoordinationSnapshot Snapshot()
    {
        lock (_gate)
        {
            var duplicate = _terminals.GroupBy(item => item.Generation).Count(group => group.Count() != 1);
            var terminalGenerations = _terminals.Select(item => item.Generation).ToHashSet();
            var missing = _acceptedGenerations.Count(generation => !terminalGenerations.Contains(generation));
            return new CoordinationSnapshot(
                _accepted,
                _terminals.Count(item => item.Terminal == "committed-not-scanout"),
                _terminals.Count(item => item.Terminal == "superseded"),
                _terminals.Count(item => item.Terminal.StartsWith("failed", StringComparison.Ordinal)),
                _maxQueueDepth,
                duplicate,
                missing,
                _targetCount,
                _appliedMismatchCount,
                _targetWidth,
                _targetHeight,
                _terminals.OrderBy(item => item.Generation).ToArray());
        }
    }

    public void Dispose()
    {
        if (_running) Stop();
        _changed.Dispose();
    }

    private sealed record ResizeRequest(
        int Generation,
        int Width,
        int Height,
        float ActualWidth,
        float ActualHeight,
        float RasterizationScale,
        long RequestedQpc);
}

internal sealed record RenderOutcome(bool Succeeded, int SlotId, string Terminal);
internal sealed record RenderTerminal(int Generation, string Terminal, int SlotId, int Width, int Height);
internal sealed record SlotSnapshot(
    int Id,
    int Generation,
    int Width,
    int Height,
    float ActualWidth,
    float ActualHeight,
    float RasterizationScale,
    string State);
internal sealed record BackdropSnapshot(
    string Name,
    string Kind,
    string Theme,
    uint TintRgb,
    float TintOpacity,
    float LuminosityOpacity);
internal sealed record SceneSnapshot(
    bool BackdropTargetAdded,
    string BackdropState,
    int SurfacePoolSize,
    int FrontSlot,
    int SuccessfulCommits,
    int SafeReuseCount,
    bool SafeRetirementProven,
    int NoSafeSlotFailures,
    int BeginDrawCount,
    int EndDrawCount,
    int GpuClearCount,
    int CpuReadbackCount,
    int StagingMapCount,
    int BitmapUploadCount,
    int ControllerCreateCount,
    int AddTargetCount,
    IReadOnlyList<BackdropSnapshot> BackdropApplications,
    IReadOnlyList<SlotSnapshot> Slots,
    string TransientOwnership,
    string RetirementBoundary);
internal sealed record CoordinationSnapshot(
    int Accepted,
    int Committed,
    int Superseded,
    int Failed,
    int MaxQueueDepth,
    int DuplicateTerminals,
    int MissingTerminals,
    int TargetCount,
    int AppliedMismatchCount,
    int LastTargetWidth,
    int LastTargetHeight,
    IReadOnlyList<RenderTerminal> Terminals);
internal sealed record EvidenceBoundary(
    string AutomatedCapability,
    string AutomatedVisible,
    string DeviceLoss,
    string Physical,
    string Scope);
internal sealed record SpikeReport(
    string Schema,
    string B1Status,
    string B2Status,
    string P1Status,
    string WindowsAppSdk,
    string OperatingSystem,
    long TopHwnd,
    uint Dpi,
    int IslandThreadId,
    int CompositionThreadId,
    bool IslandConnected,
    bool BridgeProcessesPointerInput,
    bool BridgeProcessesKeyboardInput,
    SceneSnapshot Scene,
    CoordinationSnapshot Coordination,
    EvidenceBoundary Evidence,
    string Decision);
