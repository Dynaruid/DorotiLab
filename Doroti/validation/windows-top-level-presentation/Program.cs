using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using SharpGen.Runtime;
using SkiaSharp;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DXGI;
using static Vortice.Direct3D12.D3D12;
using static Vortice.DXGI.DXGI;

namespace Doroti.Validation.WindowsTopLevelPresentation;

internal sealed class Program : IDisposable
{
    private const uint WmCreate = 0x0001;
    private const uint WmDestroy = 0x0002;
    private const uint WmSize = 0x0005;
    private const uint WmPaint = 0x000F;
    private const uint WmEraseBackground = 0x0014;
    private const uint WmClose = 0x0010;
    private const uint WmSizing = 0x0214;
    private const uint WmDpiChanged = 0x02E0;
    private const uint WmQualificationControl = 0x8001;
    private const uint WmQualificationTick = 0x8002;
    private const nuint SizeMinimized = 1;
    private const uint WsOverlappedWindow = 0x00CF0000;
    private const uint WsVisible = 0x10000000;
    private const int CwUseDefault = unchecked((int)0x80000000);
    private const int SwShow = 5;
    private const uint SwpNoActivate = 0x0010;
    private static readonly nint PerMonitorAwareV2 = new(-4);
    private static Program? _current;
    private readonly string _arm;
    private readonly string? _evidencePath;
    private readonly DirectHwndPresenter? _presenter;
    private readonly WindowProcedure _windowProcedure;
    private readonly bool _qualification;
    private readonly int _qualificationRefreshHz;
    private readonly CancellationTokenSource _qualificationStop = new();
    private Thread? _qualificationThread;
    private volatile bool _contentAnimation;
    private int _contentFrameId;
    private int _qualificationTickPending;
    private long _qualificationTickCount;
    private nint _window;
    private long _wmSizingCount;
    private long _wmSizeCount;
    private long _presentCount;
    private long _zeroSizeCount;
    private long _dpiChangeCount;
    private Exception? _failure;
    private bool _disposed;

    private Program(string arm, string? evidencePath, bool qualification, int qualificationRefreshHz)
    {
        _arm = arm;
        _evidencePath = evidencePath;
        _windowProcedure = WindowProc;
        _qualification = qualification;
        _qualificationRefreshHz = qualificationRefreshHz;
        if (arm == "A") _presenter = new DirectHwndPresenter();
    }

    [STAThread]
    private static int Main(string[] args)
    {
        var arm = ReadArgument(args, "--arm")?.ToUpperInvariant() ?? "A";
        var evidence = ReadArgument(args, "--evidence");
        var qualification = args.Contains("--qualification", StringComparer.Ordinal);
        var qualificationRefreshHz = int.TryParse(ReadArgument(args, "--refresh-hz"), out var parsedRefreshHz)
            ? Math.Clamp(parsedRefreshHz, 30, 1000)
            : 165;
        if (arm == "B")
        {
            WriteUnsupportedArmB(evidence);
            Console.Error.WriteLine(
                "Arm B is unavailable: lifted Microsoft.UI.Composition 1.8 exposes no ICompositorDesktopInterop desktop target.");
            return 2;
        }
        if (arm != "A")
        {
            Console.Error.WriteLine($"Unknown arm '{arm}'. Use A or B.");
            return 2;
        }

        SetProcessDpiAwarenessContext(PerMonitorAwareV2);
        using var application = new Program(arm, evidence, qualification, qualificationRefreshHz);
        _current = application;
        try
        {
            application.CreateWindow();
            application.StartQualificationClock();
            application.RunMessageLoop();
            application.WriteEvidence(application._failure is null ? "PASS" : "FAIL");
            return application._failure is null ? 0 : 1;
        }
        catch (Exception exception)
        {
            application._failure = exception;
            application.WriteEvidence("FAIL");
            Console.Error.WriteLine(exception);
            return 1;
        }
        finally
        {
            _current = null;
        }
    }

    private void CreateWindow()
    {
        var className = $"DorotiN0TopLevel-{Environment.ProcessId}";
        var windowClass = new WindowClassEx
        {
            Size = checked((uint)Marshal.SizeOf<WindowClassEx>()),
            Procedure = Marshal.GetFunctionPointerForDelegate(_windowProcedure),
            Instance = GetModuleHandle(null),
            Cursor = LoadCursor(0, (nint)32512),
            ClassName = className,
        };
        if (RegisterClassEx(ref windowClass) == 0)
            throw new InvalidOperationException($"RegisterClassEx failed: {Marshal.GetLastWin32Error()}.");
        _window = CreateWindowEx(
            0,
            className,
            "Doroti N0 top-level direct DXGI",
            WsOverlappedWindow | WsVisible,
            CwUseDefault,
            CwUseDefault,
            840,
            600,
            0,
            0,
            windowClass.Instance,
            0);
        if (_window == 0)
            throw new InvalidOperationException($"CreateWindowEx failed: {Marshal.GetLastWin32Error()}.");
        ShowWindow(_window, SwShow);
        UpdateWindow(_window);
        RenderCurrentClient();
    }

    private void RunMessageLoop()
    {
        while (true)
        {
            var result = GetMessage(out var message, 0, 0, 0);
            if (result == 0) return;
            if (result < 0)
                throw new InvalidOperationException($"GetMessage failed: {Marshal.GetLastWin32Error()}.");
            TranslateMessage(in message);
            DispatchMessage(in message);
        }
    }

    private nint WindowProc(nint window, uint message, nuint wParam, nint lParam)
    {
        try
        {
            switch (message)
            {
                case WmCreate:
                    return 0;
                case WmSizing:
                    _wmSizingCount++;
                    return DefWindowProc(window, message, wParam, lParam);
                case WmQualificationControl:
                    if (!_qualification) return 0;
                    _contentAnimation = wParam == 1;
                    if (wParam == 2)
                    {
                        _contentFrameId = unchecked(_contentFrameId + 1);
                        RenderCurrentClient();
                    }
                    return _contentFrameId;
                case WmQualificationTick:
                    Interlocked.Exchange(ref _qualificationTickPending, 0);
                    if (_qualification && _contentAnimation)
                    {
                        _contentFrameId = unchecked(_contentFrameId + 1);
                        _qualificationTickCount++;
                        RenderCurrentClient();
                    }
                    return 0;
                case WmSize:
                    _wmSizeCount++;
                    if (wParam == SizeMinimized)
                    {
                        _zeroSizeCount++;
                        return 0;
                    }
                    RenderCurrentClient();
                    return 0;
                case WmDpiChanged:
                    _dpiChangeCount++;
                    var suggested = Marshal.PtrToStructure<NativeRect>(lParam);
                    SetWindowPos(
                        window,
                        0,
                        suggested.Left,
                        suggested.Top,
                        suggested.Right - suggested.Left,
                        suggested.Bottom - suggested.Top,
                        SwpNoActivate);
                    return 0;
                case WmEraseBackground:
                    return 1;
                case WmPaint:
                    ValidateRect(window, 0);
                    return 0;
                case WmClose:
                    DestroyWindow(window);
                    return 0;
                case WmDestroy:
                    PostQuitMessage(0);
                    return 0;
            }
        }
        catch (Exception exception)
        {
            _failure ??= exception;
            PostQuitMessage(1);
            return 0;
        }
        return DefWindowProc(window, message, wParam, lParam);
    }

    private void RenderCurrentClient()
    {
        if (_window == 0 || !GetClientRect(_window, out var client)) return;
        var width = Math.Max(0, client.Right - client.Left);
        var height = Math.Max(0, client.Bottom - client.Top);
        if (width == 0 || height == 0)
        {
            _zeroSizeCount++;
            return;
        }
        var scale = Math.Max(1, GetDpiForWindow(_window)) / 96.0;
        _presenter!.RenderAndPresent(_window, width, height, scale, _contentFrameId);
        _presentCount++;
    }

    private void StartQualificationClock()
    {
        if (!_qualification) return;
        _qualificationThread = new Thread(() =>
        {
            var frequency = Stopwatch.Frequency;
            var interval = Math.Max(1L, frequency / _qualificationRefreshHz);
            var deadline = Stopwatch.GetTimestamp();
            while (!_qualificationStop.IsCancellationRequested)
            {
                deadline += interval;
                while (true)
                {
                    var remaining = deadline - Stopwatch.GetTimestamp();
                    if (remaining <= 0) break;
                    if (remaining > frequency / 500) Thread.Sleep(1);
                    else Thread.SpinWait(64);
                }
                if (_contentAnimation && _window != 0 &&
                    Interlocked.CompareExchange(ref _qualificationTickPending, 1, 0) == 0 &&
                    !PostMessage(_window, WmQualificationTick, 0, 0))
                    Interlocked.Exchange(ref _qualificationTickPending, 0);
            }
        })
        {
            IsBackground = true,
            Name = "Doroti N0 qualification clock",
        };
        _qualificationThread.Start();
    }

    private void WriteEvidence(string status)
    {
        if (string.IsNullOrWhiteSpace(_evidencePath)) return;
        var fullPath = Path.GetFullPath(_evidencePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        var report = new
        {
            schemaVersion = "doroti.windows-top-level-presentation/v1",
            status,
            arm = _arm,
            visibleOwner = _arm == "A" ? "single top-level HWND CreateSwapChainForHwnd" : "unavailable",
            rawRenderChildHwndCount = 0,
            swapChainPanelAttachmentCount = 0,
            cpuReadbackCount = 0,
            gdiCopyCount = 0,
            wmSizingCount = _wmSizingCount,
            wmSizeCount = _wmSizeCount,
            presentCount = _presentCount,
            zeroSizeCount = _zeroSizeCount,
            dpiChangeCount = _dpiChangeCount,
            qualification = _qualification,
            qualificationRefreshHz = _qualificationRefreshHz,
            qualificationTickCount = _qualificationTickCount,
            finalContentFrameId = _contentFrameId,
            adapter = _presenter?.AdapterDescription,
            failure = _failure?.ToString(),
        };
        File.WriteAllText(fullPath, JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            WriteIndented = true,
        }));
    }

    private static string? ReadArgument(string[] args, string name)
    {
        var index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }

    private static void WriteUnsupportedArmB(string? evidencePath)
    {
        if (string.IsNullOrWhiteSpace(evidencePath)) return;
        var fullPath = Path.GetFullPath(evidencePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, JsonSerializer.Serialize(new
        {
            schemaVersion = "doroti.windows-top-level-presentation/v1",
            status = "FAIL",
            arm = "B",
            reason = "Microsoft.UI.Composition 1.8 has no lifted ICompositorDesktopInterop/CreateDesktopWindowTarget API; system Windows.UI.Composition would not reuse the validated C0 lifted presenter.",
        }, new JsonSerializerOptions { WriteIndented = true }));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _qualificationStop.Cancel();
        if (_qualificationThread is { IsAlive: true }) _qualificationThread.Join(TimeSpan.FromSeconds(2));
        _qualificationStop.Dispose();
        _presenter?.Dispose();
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WindowClassEx
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
        [MarshalAs(UnmanagedType.LPWStr)] internal string? MenuName;
        [MarshalAs(UnmanagedType.LPWStr)] internal string ClassName;
        internal nint SmallIcon;
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
    private struct NativeMessage
    {
        internal nint Window;
        internal uint Message;
        internal nuint WParam;
        internal nint LParam;
        internal uint Time;
        internal int PointX;
        internal int PointY;
        internal uint Private;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate nint WindowProcedure(nint window, uint message, nuint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessDpiAwarenessContext(nint value);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern nint GetModuleHandle(string? moduleName);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern ushort RegisterClassEx(ref WindowClassEx value);

    [DllImport("user32.dll", EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint CreateWindowEx(
        uint extendedStyle, string className, string windowName, uint style,
        int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(nint window, int command);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UpdateWindow(nint window);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetMessage(out NativeMessage message, nint window, uint minimum, uint maximum);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TranslateMessage(in NativeMessage message);

    [DllImport("user32.dll", EntryPoint = "DispatchMessageW")]
    private static extern nint DispatchMessage(in NativeMessage message);

    [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
    private static extern nint DefWindowProc(nint window, uint message, nuint wParam, nint lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(nint window);

    [DllImport("user32.dll")]
    private static extern void PostQuitMessage(int exitCode);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetClientRect(nint window, out NativeRect rect);

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(nint window);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ValidateRect(nint window, nint rect);

    [DllImport("user32.dll")]
    private static extern nint LoadCursor(nint instance, nint cursorName);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(
        nint window, nint insertAfter, int x, int y, int width, int height, uint flags);

    [DllImport("user32.dll", EntryPoint = "PostMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PostMessage(nint window, uint message, nuint wParam, nint lParam);
}

internal sealed class DirectHwndPresenter : IDisposable
{
    private IDXGIFactory2? _factory;
    private IDXGIAdapter1? _adapter;
    private ID3D12Device2? _device;
    private ID3D12CommandQueue? _queue;
    private IDXGISwapChain3? _swapChain;
    private ID3D12CommandAllocator? _allocator;
    private ID3D12GraphicsCommandList? _commands;
    private ID3D12Fence? _fence;
    private GRVorticeD3DBackendContext? _backend;
    private GRContext? _context;
    private DirectBackingStore? _backing;
    private ulong _nextFence;
    private ulong _submittedFence;
    private int _width;
    private int _height;

    internal string AdapterDescription { get; private set; } = "uninitialized";

    internal void RenderAndPresent(nint window, int width, int height, double scale, int frameId)
    {
        EnsureDevice(window);
        EnsureSwapChain(window, width, height);
        _backing ??= new DirectBackingStore(_device!, _context!);
        _backing.EnsureSize(width, height);
        DrawOracle(_backing.Surface.Canvas, width, height, scale, frameId);
        _backing.Surface.Canvas.Flush();
        _context!.Flush(_backing.Surface);
        _context.Submit(false);

        WaitForGpu();
        _allocator!.Reset();
        _commands!.Reset(_allocator);
        using (var buffer = _swapChain!.GetBuffer<ID3D12Resource>(_swapChain.CurrentBackBufferIndex))
        {
            _commands.ResourceBarrier(
            [
                ResourceBarrier.BarrierTransition(
                    _backing.Resource, ResourceStates.RenderTarget, ResourceStates.CopySource),
                ResourceBarrier.BarrierTransition(
                    buffer, ResourceStates.Present, ResourceStates.CopyDest),
            ]);
            _commands.CopyResource(buffer, _backing.Resource);
            _commands.ResourceBarrier(
            [
                ResourceBarrier.BarrierTransition(
                    _backing.Resource, ResourceStates.CopySource, ResourceStates.RenderTarget),
                ResourceBarrier.BarrierTransition(
                    buffer, ResourceStates.CopyDest, ResourceStates.Present),
            ]);
            _commands.Close();
            _queue!.ExecuteCommandList(_commands);
        }
        _submittedFence = checked(++_nextFence);
        _queue!.Signal(_fence!, _submittedFence).CheckError();
        // The copy and Present use the same D3D12 queue. Queue ordering is the
        // readiness contract; the next frame waits before reusing the allocator.
        _swapChain!.Present(0, PresentFlags.None).CheckError();
    }

    private void EnsureDevice(nint window)
    {
        if (_device is not null) return;
        _factory = CreateDXGIFactory2<IDXGIFactory2>(false);
        _adapter = FindAdapterForWindow(_factory, window);
        if (_adapter is null)
        {
            using var factory6 = _factory.QueryInterface<IDXGIFactory6>();
            _adapter = factory6.EnumAdapterByGpuPreference<IDXGIAdapter1>(
                0, GpuPreference.MinimumPower);
        }
        AdapterDescription = _adapter.Description1.Description;
        _device = D3D12CreateDevice<ID3D12Device2>(_adapter, FeatureLevel.Level_11_0);
        _queue = _device.CreateCommandQueue(CommandListType.Direct, 0, CommandQueueFlags.None, 0);
        _allocator = _device.CreateCommandAllocator(CommandListType.Direct);
        _commands = _device.CreateCommandList<ID3D12GraphicsCommandList>(
            CommandListType.Direct, _allocator, null);
        _commands.Close();
        _fence = _device.CreateFence(0, FenceFlags.None);
        _backend = new GRVorticeD3DBackendContext
        {
            Adapter = _adapter,
            Device = _device,
            Queue = _queue,
        };
        _context = GRContext.CreateDirect3D(_backend) ??
            throw new InvalidOperationException("Skia could not create the N0 D3D12 context.");
    }

    private void EnsureSwapChain(nint window, int width, int height)
    {
        if (_swapChain is null)
        {
            var description = new SwapChainDescription1(
                checked((uint)width), checked((uint)height), Format.R8G8B8A8_UNorm,
                false, Usage.RenderTargetOutput, 2, Scaling.None,
                SwapEffect.FlipSequential, AlphaMode.Ignore, SwapChainFlags.None);
            using var created = _factory!.CreateSwapChainForHwnd(_queue!, window, description);
            _swapChain = created.QueryInterface<IDXGISwapChain3>();
            _width = width;
            _height = height;
            return;
        }
        if (_width == width && _height == height) return;
        WaitForGpu();
        _backing?.Dispose();
        _backing = null;
        _swapChain.ResizeBuffers(
            2, checked((uint)width), checked((uint)height),
            Format.R8G8B8A8_UNorm, SwapChainFlags.None).CheckError();
        _width = width;
        _height = height;
    }

    private static void DrawOracle(SKCanvas canvas, int width, int height, double scale, int frameId)
    {
        canvas.Clear(new SKColor(20, 18, 24, 255));
        var appBarHeight = Math.Min(height, Math.Max(1, (int)Math.Round(56 * scale)));
        var purple = new SKColor(103, 58, 183, 255);
        var lavender = new SKColor(179, 157, 219, 255);
        using var paint = new SKPaint { IsAntialias = false, Color = purple };
        canvas.DrawRect(0, 0, width, appBarHeight, paint);

        var tile = Math.Max(8, (int)Math.Round(16 * scale));
        for (var y = appBarHeight; y < height; y += tile)
        {
            for (var x = 0; x < width; x += tile)
            {
                paint.Color = ((x / tile) + (y / tile)) % 2 == 0
                    ? new SKColor(35, 31, 43, 255)
                    : new SKColor(24, 21, 31, 255);
                canvas.DrawRect(x, y, Math.Min(tile, width - x), Math.Min(tile, height - y), paint);
            }
        }

        paint.IsAntialias = true;
        paint.Color = lavender;
        var radius = Math.Max(18, (float)(24 * scale));
        var centerX = Math.Max(radius + 4, width * 0.55f);
        var centerY = Math.Max(appBarHeight + radius + 4, height * 0.55f);
        centerX = Math.Min(width - radius - 4, centerX);
        centerY = Math.Min(height - radius - 4, centerY);
        if (centerX > radius && centerY > appBarHeight + radius)
            canvas.DrawCircle(centerX, centerY, radius, paint);

        using var font = new SKFont(SKTypeface.Default, Math.Max(14, (float)(20 * scale)));
        paint.Color = new SKColor(238, 232, 246, 255);
        canvas.DrawText("Doroti N0", (float)(12 * scale), (float)(36 * scale),
            SKTextAlign.Left, font, paint);

        var bitSize = Math.Max(4, (int)Math.Round(7 * scale));
        var bitGap = Math.Max(1, (int)Math.Round(scale));
        var bitCount = 12;
        var bitStripWidth = bitCount * bitSize + (bitCount - 1) * bitGap;
        var bitStartX = Math.Max(0, width - bitStripWidth - Math.Max(4, (int)Math.Round(4 * scale)));
        var bitTop = Math.Max(1, (int)Math.Round(5 * scale));
        var gray = frameId ^ (frameId >> 1);
        paint.IsAntialias = false;
        for (var bit = 0; bit < bitCount; bit++)
        {
            paint.Color = (gray & (1 << bit)) != 0
                ? new SKColor(255, 255, 255, 255)
                : new SKColor(16, 8, 24, 255);
            canvas.DrawRect(bitStartX + bit * (bitSize + bitGap), bitTop, bitSize, bitSize, paint);
        }

        paint.Color = new SKColor(
            (byte)(32 + (width & 0x7f)),
            (byte)(32 + (height & 0x7f)),
            (byte)(32 + ((width ^ height) & 0x7f)),
            255);
        canvas.DrawRect(bitStartX, bitTop + bitSize + bitGap, bitStripWidth, Math.Max(2, bitGap * 2), paint);

        paint.IsAntialias = false;
        paint.Color = purple;
        var edgeWidth = Math.Max(2, (int)Math.Ceiling(scale));
        canvas.DrawRect(width - edgeWidth, appBarHeight, edgeWidth, height - appBarHeight, paint);
    }

    private void WaitForGpu()
    {
        if (_submittedFence == 0 || _fence!.CompletedValue >= _submittedFence) return;
        using var completed = new EventWaitHandle(false, EventResetMode.AutoReset);
        _fence.SetEventOnCompletion(_submittedFence, completed).CheckError();
        if (!completed.WaitOne(TimeSpan.FromSeconds(5)))
            throw new TimeoutException($"N0 D3D12 fence {_submittedFence} timed out.");
    }

    private static IDXGIAdapter1? FindAdapterForWindow(IDXGIFactory2 factory, nint window)
    {
        var monitor = MonitorFromWindow(window, 2);
        if (monitor == 0) return null;
        for (uint adapterIndex = 0; ; adapterIndex++)
        {
            if (factory.EnumAdapters1(adapterIndex, out var adapter).Failure) break;
            var matched = false;
            for (uint outputIndex = 0; ; outputIndex++)
            {
                if (adapter.EnumOutputs(outputIndex, out var output).Failure) break;
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

    public void Dispose()
    {
        WaitForGpu();
        _backing?.Dispose();
        _swapChain?.Dispose();
        _context?.Dispose();
        _backend?.Dispose();
        _fence?.Dispose();
        _commands?.Dispose();
        _allocator?.Dispose();
        _queue?.Dispose();
        _device?.Dispose();
        _adapter?.Dispose();
        _factory?.Dispose();
    }

    [DllImport("user32.dll")]
    private static extern nint MonitorFromWindow(nint window, uint flags);
}

internal sealed class DirectBackingStore(ID3D12Device device, GRContext context) : IDisposable
{
    private ID3D12Resource? _resource;
    private GRVorticeD3DTextureResourceInfo? _resourceInfo;
    private GRBackendRenderTarget? _target;
    private SKSurface? _surface;
    private int _width;
    private int _height;

    internal ID3D12Resource Resource => _resource!;
    internal SKSurface Surface => _surface!;

    internal void EnsureSize(int width, int height)
    {
        if (_surface is not null && width == _width && height == _height) return;
        DisposeResources();
        _resource = device.CreateCommittedResource(
            HeapType.Default,
            HeapFlags.None,
            ResourceDescription.Texture2D(
                Format.R8G8B8A8_UNorm,
                checked((uint)width),
                checked((uint)height),
                1, 1, 1, 0,
                ResourceFlags.AllowRenderTarget),
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
        _target = new GRBackendRenderTarget(width, height, _resourceInfo);
        _surface = SKSurface.Create(context, _target, GRSurfaceOrigin.TopLeft, SKColorType.Rgba8888) ??
                   throw new InvalidOperationException("Skia could not wrap the N0 backing store.");
        _width = width;
        _height = height;
    }

    private void DisposeResources()
    {
        _surface?.Dispose();
        _surface = null;
        _target?.Dispose();
        _target = null;
        _resourceInfo?.Dispose();
        _resourceInfo = null;
        _resource?.Dispose();
        _resource = null;
        _width = _height = 0;
    }

    public void Dispose() => DisposeResources();
}
