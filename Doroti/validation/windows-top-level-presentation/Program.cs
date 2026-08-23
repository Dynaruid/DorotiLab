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
    private const uint WmWindowPosChanging = 0x0046;
    private const uint WmPaint = 0x000F;
    private const uint WmEraseBackground = 0x0014;
    private const uint WmClose = 0x0010;
    private const uint WmSizing = 0x0214;
    private const uint WmDpiChanged = 0x02E0;
    private const uint WmQualificationControl = 0x8001;
    private const nuint SizeMinimized = 1;
    private const uint WsOverlappedWindow = 0x00CF0000;
    private const uint WsVisible = 0x10000000;
    private const uint WsChild = 0x40000000;
    private const uint WsClipSiblings = 0x04000000;
    private const uint WsClipChildren = 0x02000000;
    private const int CwUseDefault = unchecked((int)0x80000000);
    private const int SwShow = 5;
    private const uint SwpNoActivate = 0x0010;
    private static readonly nint PerMonitorAwareV2 = new(-4);
    private static Program? _current;
    private readonly string _arm;
    private readonly string? _evidencePath;
    private readonly DirectHwndPresenter[] _presenters = new DirectHwndPresenter[1];
    private readonly D3D12RenderWorker[] _renderWorkers = new D3D12RenderWorker[1];
    private readonly WindowProcedure _windowProcedure;
    private readonly WindowProcedure _renderWindowProcedure;
    private readonly bool _qualification;
    private readonly int _qualificationRefreshHz;
    private int _contentFrameId;
    private long _qualificationTickCount;
    private long _qualificationAnimationStarted;
    private long _qualificationAnimationElapsed;
    private nint _window;
    private readonly nint[] _renderWindows = new nint[1];
    private int _preparedContentWidth;
    private int _preparedContentHeight;
    private int _qualificationAnimationEnabled;
    private long _wmSizingCount;
    private long _wmSizeCount;
    private long _presentCount;
    private long _zeroSizeCount;
    private long _dpiChangeCount;
    private long _resizeHandshakeCount;
    private long _resizeHandshakeTimeoutCount;
    private long _resizeHandshakeMaximumTicks;
    private long _precommitSizingCount;
    private long _precommitGeometryHoldCount;
    private Exception? _failure;
    private bool _disposed;

    private Program(string arm, string? evidencePath, bool qualification, int qualificationRefreshHz)
    {
        _arm = arm;
        _evidencePath = evidencePath;
        _windowProcedure = WindowProc;
        _renderWindowProcedure = RenderWindowProc;
        _qualification = qualification;
        _qualificationRefreshHz = qualificationRefreshHz;
        if (arm == "A")
        {
            for (var index = 0; index < _presenters.Length; index++)
            {
                _presenters[index] = new DirectHwndPresenter(
                    drawRightEdgeOracle: qualification);
                var renderIndex = index;
                _renderWorkers[index] = new D3D12RenderWorker(
                    _presenters[index],
                    (qualificationAnimation, frameId) =>
                        OnPresented(renderIndex, qualificationAnimation, frameId),
                    OnRenderFailure);
            }
        }
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
            WsOverlappedWindow | WsVisible | WsClipChildren,
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
        if (!GetClientRect(_window, out var client))
            throw new InvalidOperationException($"GetClientRect failed: {Marshal.GetLastWin32Error()}.");
        var renderClassName = $"DorotiD3D12RenderChild-{Environment.ProcessId}";
        var renderWindowClass = new WindowClassEx
        {
            Size = checked((uint)Marshal.SizeOf<WindowClassEx>()),
            Procedure = Marshal.GetFunctionPointerForDelegate(_renderWindowProcedure),
            Instance = windowClass.Instance,
            Cursor = windowClass.Cursor,
            ClassName = renderClassName,
        };
        if (RegisterClassEx(ref renderWindowClass) == 0)
            throw new InvalidOperationException($"Render child RegisterClassEx failed: {Marshal.GetLastWin32Error()}.");
        for (var index = 0; index < _renderWindows.Length; index++)
        {
            _renderWindows[index] = CreateWindowEx(
                0,
                renderClassName,
                $"Doroti D3D12 render child {index}",
                WsChild | WsVisible | WsClipSiblings,
                0,
                0,
                Math.Max(1, client.Right - client.Left),
                Math.Max(1, client.Bottom - client.Top),
                _window,
                0,
                renderWindowClass.Instance,
                0);
            if (_renderWindows[index] == 0)
                throw new InvalidOperationException(
                    $"Render child {index} CreateWindowEx failed: {Marshal.GetLastWin32Error()}.");
        }
        ShowWindow(_window, SwShow);
        UpdateWindow(_window);
        for (var index = 0; index < _renderWindows.Length; index++)
        {
            if (!CommitRenderWindow(index, TimeSpan.FromSeconds(5), recordResizeHandshake: false))
                throw new TimeoutException($"Initial D3D12 render-child {index} present exceeded five seconds.");
        }
        if (!SetWindowPos(
            _renderWindows[0], 0, 0, 0,
            Math.Max(1, client.Right - client.Left), Math.Max(1, client.Bottom - client.Top),
            SwpNoActivate | 0x0040))
            throw new InvalidOperationException($"Initial render-child z-order failed: {Marshal.GetLastWin32Error()}.");
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
                case WmWindowPosChanging:
                    var positionResult = DefWindowProc(window, message, wParam, lParam);
                    PrecommitExpandedCoverForWindowPosition(lParam);
                    return positionResult;
                case WmQualificationControl:
                    if (!_qualification) return 0;
                    if (wParam == 1)
                    {
                        Interlocked.Exchange(ref _qualificationAnimationStarted, Stopwatch.GetTimestamp());
                        Volatile.Write(ref _qualificationAnimationEnabled, 1);
                        foreach (var worker in _renderWorkers) worker?.SetAnimation(false);
                        _renderWorkers[0].SetAnimation(true);
                    }
                    else if (wParam == 0)
                    {
                        Volatile.Write(ref _qualificationAnimationEnabled, 0);
                        foreach (var worker in _renderWorkers) worker?.SetAnimation(false);
                        var started = Interlocked.Exchange(ref _qualificationAnimationStarted, 0);
                        if (started != 0)
                            Interlocked.Add(ref _qualificationAnimationElapsed, Stopwatch.GetTimestamp() - started);
                        WriteEvidence("RUNNING");
                    }
                    else if (wParam == 2)
                    {
                        return _renderWorkers[0].PresentSingleFrame();
                    }
                    return Volatile.Read(ref _contentFrameId);
                case WmSize:
                    _wmSizeCount++;
                    if (wParam == SizeMinimized)
                    {
                        _zeroSizeCount++;
                        return 0;
                    }
                    CommitExactRenderChild();
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
                    foreach (var worker in _renderWorkers) worker?.SetAnimation(false);
                    DestroyWindow(window);
                    return 0;
                case WmDestroy:
                    foreach (var worker in _renderWorkers) worker?.SetAnimation(false);
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

    private nint RenderWindowProc(nint window, uint message, nuint wParam, nint lParam)
    {
        try
        {
            switch (message)
            {
                case WmCreate:
                    return 0;
                case WmSize:
                    // Staging is rendered explicitly after SetWindowPos returns.
                    // Never destructively resize the currently visible front.
                    return 0;
                case WmEraseBackground:
                    return 1;
                case WmPaint:
                    ValidateRect(window, 0);
                    return 0;
                case WmDestroy:
                    return 0;
            }
        }
        catch (Exception exception)
        {
            _failure ??= exception;
            if (_window != 0) PostMessage(_window, WmClose, 0, 0);
            return 0;
        }
        return DefWindowProc(window, message, wParam, lParam);
    }

    private void PrecommitExpandedCoverForWindowPosition(nint windowPositionPointer)
    {
        if (_window == 0 || _renderWindows[0] == 0 ||
            windowPositionPointer == 0 ||
            !GetWindowRect(_window, out var currentOuter) ||
            !GetClientRect(_window, out var currentClient) ||
            !GetClientRect(_renderWindows[0], out var renderClient)) return;
        var position = Marshal.PtrToStructure<NativeWindowPosition>(windowPositionPointer);
        if ((position.Flags & 0x0001) != 0) return; // SWP_NOSIZE
        var nonClientWidth = Math.Max(0,
            (currentOuter.Right - currentOuter.Left) - (currentClient.Right - currentClient.Left));
        var nonClientHeight = Math.Max(0,
            (currentOuter.Bottom - currentOuter.Top) - (currentClient.Bottom - currentClient.Top));
        var requestedWidth = Math.Max(1, position.Width - nonClientWidth);
        var requestedHeight = Math.Max(1, position.Height - nonClientHeight);
        var currentWidth = Math.Max(1, renderClient.Right - renderClient.Left);
        var currentHeight = Math.Max(1, renderClient.Bottom - renderClient.Top);
        var visibleWidth = Math.Max(1, currentClient.Right - currentClient.Left);
        var visibleHeight = Math.Max(1, currentClient.Bottom - currentClient.Top);
        var coverWidth = Math.Max(currentWidth, requestedWidth);
        var coverHeight = Math.Max(currentHeight, requestedHeight);
        if (coverWidth != currentWidth || coverHeight != currentHeight)
        {
            if (!CommitRenderWindow(
                0, coverWidth, coverHeight,
                TimeSpan.FromMilliseconds(100),
                recordResizeHandshake: true,
                flushBeforeAck: true,
                contentWidth: visibleWidth,
                contentHeight: visibleHeight))
            {
                HoldCurrentWindowPosition(ref position, currentOuter, windowPositionPointer);
                return;
            }
            if (!SetWindowPos(
                _renderWindows[0], 0, 0, 0, coverWidth, coverHeight,
                SwpNoActivate | 0x0040))
                throw new InvalidOperationException(
                    $"SetWindowPos(expanded render cover) failed: {Marshal.GetLastWin32Error()}.");
            Interlocked.Increment(ref _precommitSizingCount);
        }

        if (CommitRenderWindow(
            0, coverWidth, coverHeight,
            TimeSpan.FromMilliseconds(100),
            recordResizeHandshake: true,
            contentWidth: requestedWidth,
            contentHeight: requestedHeight,
            present: false))
        {
            _preparedContentWidth = requestedWidth;
            _preparedContentHeight = requestedHeight;
            return;
        }

        HoldCurrentWindowPosition(ref position, currentOuter, windowPositionPointer);
    }

    private void HoldCurrentWindowPosition(
        ref NativeWindowPosition position,
        NativeRect currentOuter,
        nint windowPositionPointer)
    {
        // Do not expose geometry until the matching raster has been prepared.
        // A later pointer update retries it.
        position.X = currentOuter.Left;
        position.Y = currentOuter.Top;
        position.Width = currentOuter.Right - currentOuter.Left;
        position.Height = currentOuter.Bottom - currentOuter.Top;
        Marshal.StructureToPtr(position, windowPositionPointer, false);
        Interlocked.Increment(ref _precommitGeometryHoldCount);
    }

    private void CommitExactRenderChild()
    {
        if (_window == 0 || !GetClientRect(_window, out var client)) return;
        var width = Math.Max(0, client.Right - client.Left);
        var height = Math.Max(0, client.Bottom - client.Top);
        if (width == 0 || height == 0)
        {
            _zeroSizeCount++;
            return;
        }
        var prepared = _preparedContentWidth == width && _preparedContentHeight == height;
        if (prepared)
        {
            var started = Stopwatch.GetTimestamp();
            var completed = _renderWorkers[0].PresentPreparedAndWait(
                TimeSpan.FromMilliseconds(100), flushBeforeAck: false);
            var elapsed = Stopwatch.GetTimestamp() - started;
            Interlocked.Increment(ref _resizeHandshakeCount);
            UpdateMaximum(ref _resizeHandshakeMaximumTicks, elapsed);
            if (!completed)
            {
                Interlocked.Increment(ref _resizeHandshakeTimeoutCount);
                return;
            }
        }
        else if (!CommitRenderWindow(
            0, width, height,
            TimeSpan.FromMilliseconds(100),
            recordResizeHandshake: true,
            flushBeforeAck: false)) return;
        if (!SetWindowPos(
            _renderWindows[0], 0, 0, 0, width, height,
            SwpNoActivate | 0x0040))
            throw new InvalidOperationException(
                $"SetWindowPos(exact render child) failed: {Marshal.GetLastWin32Error()}.");
        _preparedContentWidth = 0;
        _preparedContentHeight = 0;
    }

    private bool CommitRenderWindow(
        int renderIndex,
        TimeSpan timeout,
        bool recordResizeHandshake,
        bool flushBeforeAck = false)
    {
        var renderWindow = _renderWindows[renderIndex];
        if (renderWindow == 0 || !GetClientRect(renderWindow, out var client)) return true;
        var width = Math.Max(0, client.Right - client.Left);
        var height = Math.Max(0, client.Bottom - client.Top);
        return CommitRenderWindow(
            renderIndex, width, height, timeout, recordResizeHandshake, flushBeforeAck);
    }

    private bool CommitRenderWindow(
        int renderIndex,
        int width,
        int height,
        TimeSpan timeout,
        bool recordResizeHandshake,
        bool flushBeforeAck = false,
        int? contentWidth = null,
        int? contentHeight = null,
        bool present = true)
    {
        var renderWindow = _renderWindows[renderIndex];
        if (renderWindow == 0) return true;
        if (width == 0 || height == 0)
        {
            _zeroSizeCount++;
            return true;
        }
        var scaleOwner = _window != 0 ? _window : renderWindow;
        var scale = Math.Max(1, GetDpiForWindow(scaleOwner)) / 96.0;
        var started = Stopwatch.GetTimestamp();
        var completed = _renderWorkers[renderIndex].CommitTargetAndWait(
            renderWindow, width, height,
            contentWidth ?? width, contentHeight ?? height,
            scale, timeout, flushBeforeAck, present,
            Volatile.Read(ref _contentFrameId));
        var elapsed = Stopwatch.GetTimestamp() - started;
        if (recordResizeHandshake)
        {
            Interlocked.Increment(ref _resizeHandshakeCount);
            UpdateMaximum(ref _resizeHandshakeMaximumTicks, elapsed);
            if (!completed) Interlocked.Increment(ref _resizeHandshakeTimeoutCount);
        }
        return completed;
    }

    private static void UpdateMaximum(ref long location, long value)
    {
        var observed = Volatile.Read(ref location);
        while (value > observed)
        {
            var previous = Interlocked.CompareExchange(ref location, value, observed);
            if (previous == observed) return;
            observed = previous;
        }
    }

    private void OnPresented(int renderIndex, bool qualificationAnimation, int frameId)
    {
        if (renderIndex != 0) return;
        Volatile.Write(ref _contentFrameId, frameId);
        Interlocked.Increment(ref _presentCount);
        if (qualificationAnimation) Interlocked.Increment(ref _qualificationTickCount);
    }

    private void OnRenderFailure(Exception exception)
    {
        _failure ??= exception;
        if (_window != 0) PostMessage(_window, WmClose, 0, 0);
    }

    private void WriteEvidence(string status)
    {
        if (string.IsNullOrWhiteSpace(_evidencePath)) return;
        var fullPath = Path.GetFullPath(_evidencePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        var animationElapsed = Volatile.Read(ref _qualificationAnimationElapsed);
        var animationStarted = Volatile.Read(ref _qualificationAnimationStarted);
        if (animationStarted != 0) animationElapsed += Stopwatch.GetTimestamp() - animationStarted;
        var animationSeconds = animationElapsed / (double)Stopwatch.Frequency;
        var qualificationFrames = Volatile.Read(ref _qualificationTickCount);
        var report = new
        {
            schemaVersion = "doroti.windows-top-level-presentation/v1",
            status,
            arm = _arm,
            visibleOwner = _arm == "A" ? "top-level chrome HWND + single D3D12 render child with 1:1 monotonic-capacity front and prepared exact-content raster" : "unavailable",
            rawRenderChildHwndCount = 1,
            swapChainPanelAttachmentCount = 0,
            cpuReadbackCount = 0,
            gdiCopyCount = 0,
            wmSizingCount = _wmSizingCount,
            wmSizeCount = _wmSizeCount,
            presentCount = Volatile.Read(ref _presentCount),
            zeroSizeCount = _zeroSizeCount,
            dpiChangeCount = _dpiChangeCount,
            resizeHandshakeCount = Volatile.Read(ref _resizeHandshakeCount),
            resizeHandshakeTimeoutCount = Volatile.Read(ref _resizeHandshakeTimeoutCount),
            resizeHandshakeMaximumMilliseconds = Volatile.Read(ref _resizeHandshakeMaximumTicks) * 1000.0 / Stopwatch.Frequency,
            resizeHandshakeTimeoutMilliseconds = 100,
            precommitSizingCount = Volatile.Read(ref _precommitSizingCount),
            precommitGeometryHoldCount = Volatile.Read(ref _precommitGeometryHoldCount),
            qualification = _qualification,
            qualificationRefreshHz = _qualificationRefreshHz,
            qualificationTickCount = qualificationFrames,
            qualificationAnimationSeconds = animationSeconds,
            qualificationPresentedFramesPerSecond = animationSeconds > 0 ? qualificationFrames / animationSeconds : 0,
            qualificationRenderBackend = "D3D12 dedicated render worker + DXGI Present(1) visible cadence; Present(0) resize commits",
            finalContentFrameId = Volatile.Read(ref _contentFrameId),
            adapters = _presenters.Select(presenter => presenter?.AdapterDescription).ToArray(),
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
        foreach (var worker in _renderWorkers) worker?.Dispose();
        foreach (var presenter in _presenters) presenter?.Dispose();
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
    private struct NativeWindowPosition
    {
        internal nint Window;
        internal nint InsertAfter;
        internal int X;
        internal int Y;
        internal int Width;
        internal int Height;
        internal uint Flags;
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
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(nint window, out NativeRect rect);

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

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool MoveWindow(
        nint window, int x, int y, int width, int height,
        [MarshalAs(UnmanagedType.Bool)] bool repaint);

    [DllImport("user32.dll", EntryPoint = "PostMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PostMessage(nint window, uint message, nuint wParam, nint lParam);
}

internal sealed class D3D12RenderWorker : IDisposable
{
    private readonly DirectHwndPresenter _presenter;
    private readonly Action<bool, int> _presented;
    private readonly Action<Exception> _failed;
    private readonly object _gate = new();
    private readonly AutoResetEvent _wake = new(false);
    private readonly Thread _thread;
    private readonly Queue<CommitRequest> _commits = new();
    private readonly Queue<PreparedPresentRequest> _preparedPresents = new();
    private readonly Queue<SingleFrameRequest> _singleFrames = new();
    private nint _window;
    private int _width;
    private int _height;
    private int _contentWidth;
    private int _contentHeight;
    private double _scale = 1;
    private long _targetVersion;
    private long _renderedTargetVersion;
    private bool _animate;
    private bool _stopping;
    private int _frameId;

    internal D3D12RenderWorker(
        DirectHwndPresenter presenter,
        Action<bool, int> presented,
        Action<Exception> failed)
    {
        _presenter = presenter;
        _presented = presented;
        _failed = failed;
        _thread = new Thread(Run)
        {
            IsBackground = true,
            Name = "Doroti D3D12 qualification renderer",
        };
        _thread.Start();
    }

    internal void UpdateTarget(nint window, int width, int height, double scale)
    {
        lock (_gate)
        {
            _window = window;
            _width = width;
            _height = height;
            _scale = scale;
            _targetVersion++;
        }
        _wake.Set();
    }

    internal bool CommitTargetAndWait(
        nint window,
        int width,
        int height,
        int contentWidth,
        int contentHeight,
        double scale,
        TimeSpan timeout,
        bool flushBeforeAck,
        bool present,
        int frameId)
    {
        CommitRequest request;
        lock (_gate)
        {
            if (_stopping) throw new ObjectDisposedException(nameof(D3D12RenderWorker));
            _window = window;
            _width = width;
            _height = height;
            _contentWidth = contentWidth;
            _contentHeight = contentHeight;
            _scale = scale;
            var version = ++_targetVersion;
            request = new CommitRequest(
                window, width, height, contentWidth, contentHeight,
                scale, version, flushBeforeAck, present, frameId);
            _commits.Enqueue(request);
        }
        _wake.Set();
        if (!request.Completion.Task.Wait(timeout)) return false;
        return request.Completion.Task.GetAwaiter().GetResult();
    }

    internal bool PresentPreparedAndWait(TimeSpan timeout, bool flushBeforeAck)
    {
        PreparedPresentRequest request;
        lock (_gate)
        {
            if (_stopping) throw new ObjectDisposedException(nameof(D3D12RenderWorker));
            request = new PreparedPresentRequest(flushBeforeAck);
            _preparedPresents.Enqueue(request);
        }
        _wake.Set();
        if (!request.Completion.Task.Wait(timeout)) return false;
        return request.Completion.Task.GetAwaiter().GetResult();
    }

    internal void SetAnimation(bool enabled)
    {
        lock (_gate) _animate = enabled;
        _wake.Set();
    }

    internal int PresentSingleFrame()
    {
        var request = new SingleFrameRequest();
        lock (_gate)
        {
            if (_stopping) throw new ObjectDisposedException(nameof(D3D12RenderWorker));
            _singleFrames.Enqueue(request);
        }
        _wake.Set();
        if (!request.Completed.Wait(TimeSpan.FromSeconds(5)))
            throw new TimeoutException("D3D12 qualification single-frame present timed out.");
        try
        {
            if (request.Failure is not null)
                throw new InvalidOperationException("D3D12 qualification single-frame present failed.", request.Failure);
            return request.FrameId;
        }
        finally
        {
            request.Dispose();
        }
    }

    private void Run()
    {
        try
        {
            while (true)
            {
                nint window;
                int width;
                int height;
                int contentWidth;
                int contentHeight;
                double scale;
                long targetVersion;
                bool animate;
                bool targetChanged;
                CommitRequest? commit;
                PreparedPresentRequest? preparedPresent;
                SingleFrameRequest? singleFrame;
                lock (_gate)
                {
                    if (_stopping) return;
                    window = _window;
                    width = _width;
                    height = _height;
                    contentWidth = _contentWidth;
                    contentHeight = _contentHeight;
                    scale = _scale;
                    targetVersion = _targetVersion;
                    animate = _animate;
                    targetChanged = targetVersion != _renderedTargetVersion;
                    commit = _commits.Count == 0 ? null : _commits.Dequeue();
                    preparedPresent = _preparedPresents.Count == 0
                        ? null
                        : _preparedPresents.Dequeue();
                    singleFrame = _singleFrames.Count == 0 ? null : _singleFrames.Dequeue();
                }

                if (preparedPresent is not null)
                {
                    try
                    {
                        _presenter.PresentPrepared(synchronizeToRefresh: false);
                        _presented(false, _frameId);
                        if (preparedPresent.FlushBeforeAck) _presenter.FlushDwm();
                        preparedPresent.Completion.TrySetResult(true);
                        if (!preparedPresent.FlushBeforeAck) _presenter.FlushDwm();
                    }
                    catch (Exception exception)
                    {
                        preparedPresent.Completion.TrySetException(exception);
                        throw;
                    }
                    continue;
                }

                if (commit is not null)
                {
                    window = commit.Window;
                    width = commit.Width;
                    height = commit.Height;
                    contentWidth = commit.ContentWidth;
                    contentHeight = commit.ContentHeight;
                    scale = commit.Scale;
                    targetVersion = commit.Version;
                    _frameId = commit.FrameId;
                    targetChanged = true;
                }

                if (window == 0 || width <= 0 || height <= 0 ||
                    (!animate && !targetChanged && commit is null && singleFrame is null))
                {
                    _wake.WaitOne();
                    continue;
                }

                var animatedFrame = animate && commit is null && singleFrame is null;
                if (animatedFrame || singleFrame is not null) _frameId = unchecked(_frameId + 1);
                try
                {
                    // Resize commits are ordered explicitly by the platform
                    // thread and must not wait for scan-out. Only continuous
                    // visible animation is refresh synchronized.
                    _presenter.RenderFrame(
                        window, width, height, contentWidth, contentHeight,
                        scale, _frameId,
                        present: commit?.Present ?? true,
                        synchronizeToRefresh: commit is null);
                    lock (_gate)
                    {
                        if (targetVersion > _renderedTargetVersion) _renderedTargetVersion = targetVersion;
                    }
                    if (commit?.Present ?? true) _presented(animatedFrame, _frameId);
                    if (commit is not null)
                    {
                        // A WM_SIZING expansion must finish composition before
                        // parent geometry exposes the newly covered pixels.
                        // Ordinary committed WM_SIZE follows Flutter's order:
                        // release the platform wait, then flush on this worker.
                        if (commit.Present && commit.FlushBeforeAck) _presenter.FlushDwm();
                        commit.Completion.TrySetResult(true);
                        if (commit.Present && !commit.FlushBeforeAck) _presenter.FlushDwm();
                    }
                    if (singleFrame is not null)
                    {
                        singleFrame.FrameId = _frameId;
                        singleFrame.Completed.Set();
                    }
                }
                catch (Exception exception)
                {
                    commit?.Completion.TrySetException(exception);
                    if (singleFrame is not null)
                    {
                        singleFrame.Failure = exception;
                        singleFrame.Completed.Set();
                    }
                    throw;
                }
            }
        }
        catch (Exception exception)
        {
            lock (_gate)
            {
                while (_commits.Count != 0)
                    _commits.Dequeue().Completion.TrySetException(exception);
                while (_preparedPresents.Count != 0)
                    _preparedPresents.Dequeue().Completion.TrySetException(exception);
                while (_singleFrames.Count != 0)
                {
                    var request = _singleFrames.Dequeue();
                    request.Failure = exception;
                    request.Completed.Set();
                }
            }
            _failed(exception);
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_stopping) return;
            _stopping = true;
        }
        _wake.Set();
        if (_thread.IsAlive && !_thread.Join(TimeSpan.FromSeconds(5)))
            throw new TimeoutException("D3D12 render worker did not stop within five seconds.");
        _wake.Dispose();
    }

    private sealed class SingleFrameRequest : IDisposable
    {
        internal ManualResetEventSlim Completed { get; } = new(false);
        internal int FrameId { get; set; }
        internal Exception? Failure { get; set; }
        public void Dispose() => Completed.Dispose();
    }

    private sealed record CommitRequest(
        nint Window,
        int Width,
        int Height,
        int ContentWidth,
        int ContentHeight,
        double Scale,
        long Version,
        bool FlushBeforeAck,
        bool Present,
        int FrameId)
    {
        internal TaskCompletionSource<bool> Completion { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private sealed record PreparedPresentRequest(bool FlushBeforeAck)
    {
        internal TaskCompletionSource<bool> Completion { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}

internal sealed class DirectHwndPresenter : IDisposable
{
    private readonly bool _drawRightEdgeOracle;
    private IDXGIFactory2? _factory;
    private IDXGIAdapter1? _adapter;
    private ID3D12Device2? _device;
    private ID3D12CommandQueue? _queue;
    private IDXGISwapChain3? _swapChain;
    private ID3D12CommandAllocator? _allocator;
    private ID3D12GraphicsCommandList? _commands;
    private ID3D12Fence? _fence;
    private EventWaitHandle? _fenceEvent;
    private GRVorticeD3DBackendContext? _backend;
    private GRContext? _context;
    private DirectBackingStore? _backing;
    private ulong _nextFence;
    private ulong _submittedFence;
    private int _width;
    private int _height;
    private nint _frameLatencyWaitableObject;

    internal string AdapterDescription { get; private set; } = "uninitialized";

    internal DirectHwndPresenter(bool drawRightEdgeOracle)
    {
        _drawRightEdgeOracle = drawRightEdgeOracle;
    }

    internal void RenderFrame(
        nint window,
        int width,
        int height,
        int contentWidth,
        int contentHeight,
        double scale,
        int frameId,
        bool present,
        bool synchronizeToRefresh)
    {
        EnsureDevice(window);
        EnsureSwapChain(window, width, height);
        // A size change replaces the exact backing resource. Confirm the prior
        // copy before DirectBackingStore disposes that resource; ResizeBuffers
        // used to provide this wait implicitly in the old per-size path.
        WaitForGpu();
        _backing ??= new DirectBackingStore(_device!, _context!);
        _backing.EnsureSize(_width, _height);
        DrawOracle(
            _backing.Surface.Canvas,
            Math.Clamp(contentWidth, 1, _width),
            Math.Clamp(contentHeight, 1, _height),
            scale,
            frameId,
            _drawRightEdgeOracle);
        _backing.Surface.Canvas.Flush();
        _context!.Flush(_backing.Surface);
        _context.Submit(false);
        if (present) PresentPrepared(synchronizeToRefresh);
    }

    internal void PresentPrepared(bool synchronizeToRefresh)
    {
        if (_backing is null || _swapChain is null)
            throw new InvalidOperationException("No prepared D3D12 frame is available.");
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
        // Resize preparation must not wait for scan-out. Visible animation has
        // exactly one refresh clock: the Present(1) interval itself.
        _swapChain!.Present(synchronizeToRefresh ? 1u : 0u, PresentFlags.None).CheckError();
    }

    internal void FlushDwm()
    {
        var result = DwmFlush();
        if (result < 0) Marshal.ThrowExceptionForHR(result);
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
        _fenceEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
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
                SwapEffect.FlipSequential, AlphaMode.Ignore, SwapChainFlags.FrameLatencyWaitableObject);
            using var created = _factory!.CreateSwapChainForHwnd(_queue!, window, description);
            _swapChain = created.QueryInterface<IDXGISwapChain3>();
            using var swapChain2 = _swapChain.QueryInterface<IDXGISwapChain2>();
            swapChain2.MaximumFrameLatency = 1;
            _frameLatencyWaitableObject = swapChain2.FrameLatencyWaitableObject;
            if (_frameLatencyWaitableObject == 0)
                throw new InvalidOperationException("DXGI did not expose a frame-latency waitable object.");
            _width = width;
            _height = height;
            return;
        }
        var capacityWidth = Math.Max(_width, width);
        var capacityHeight = Math.Max(_height, height);
        if (_width == capacityWidth && _height == capacityHeight) return;
        WaitForGpu();
        _backing?.Dispose();
        _backing = null;
        _swapChain.ResizeBuffers(
            2, checked((uint)capacityWidth), checked((uint)capacityHeight),
            Format.R8G8B8A8_UNorm, SwapChainFlags.FrameLatencyWaitableObject).CheckError();
        _width = capacityWidth;
        _height = capacityHeight;
    }

    private static void DrawOracle(
        SKCanvas canvas,
        int width,
        int height,
        double scale,
        int frameId,
        bool drawRightEdgeOracle)
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

        if (drawRightEdgeOracle)
        {
            paint.IsAntialias = false;
            paint.Color = purple;
            var edgeWidth = Math.Max(2, (int)Math.Ceiling(scale));
            canvas.DrawRect(
                width - edgeWidth, appBarHeight,
                edgeWidth, height - appBarHeight, paint);
        }
    }

    private void WaitForGpu()
    {
        if (_submittedFence == 0 || _fence!.CompletedValue >= _submittedFence) return;
        _fence.SetEventOnCompletion(_submittedFence, _fenceEvent!).CheckError();
        if (!_fenceEvent!.WaitOne(TimeSpan.FromSeconds(5)))
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
        _frameLatencyWaitableObject = 0;
        _fenceEvent?.Dispose();
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

    [DllImport("dwmapi.dll")]
    private static extern int DwmFlush();
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
