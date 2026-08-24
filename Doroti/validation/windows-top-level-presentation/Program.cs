using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using SharpGen.Runtime;
using SkiaSharp;
using Vortice;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DirectComposition;
using Vortice.DXGI;
using static Vortice.Direct3D12.D3D12;
using static Vortice.DXGI.DXGI;

namespace Doroti.Validation.WindowsTopLevelPresentation;

internal sealed class Program : IDisposable
{
    private const uint WmCreate = 0x0001;
    private const uint WmDestroy = 0x0002;
    private const uint WmSize = 0x0005;
    private const uint WmSetCursor = 0x0020;
    private const uint WmWindowPosChanging = 0x0046;
    private const uint WmNcHitTest = 0x0084;
    private const uint WmPaint = 0x000F;
    private const uint WmEraseBackground = 0x0014;
    private const uint WmClose = 0x0010;
    private const uint WmSizing = 0x0214;
    private const uint WmCaptureChanged = 0x0215;
    private const uint WmMouseMove = 0x0200;
    private const uint WmLeftButtonDown = 0x0201;
    private const uint WmLeftButtonUp = 0x0202;
    private const uint WmDpiChanged = 0x02E0;
    private const uint WmQualificationControl = 0x8001;
    private const uint WmOwnedSmoke = 0x8002;
    private const nuint SizeMinimized = 1;
    private const uint WsOverlappedWindow = 0x00CF0000;
    private const uint WsPopup = 0x80000000;
    private const uint WsVisible = 0x10000000;
    private const uint WsExNoRedirectionBitmap = 0x00200000;
    private const nuint MouseKeyLeftButton = 0x0001;
    private const int HitTestTransparent = -1;
    private const int HitTestClient = 1;
    private const int CwUseDefault = unchecked((int)0x80000000);
    private const int SwShow = 5;
    private const uint SwpNoActivate = 0x0010;
    private static readonly nint PerMonitorAwareV2 = new(-4);
    private const uint WindowBackgroundColorRef = 0x00181214; // RGB(20, 18, 24)
    private static Program? _current;
    private readonly string _arm;
    private readonly string? _evidencePath;
    private readonly DirectHwndPresenter[] _presenters = new DirectHwndPresenter[1];
    private readonly D3D12RenderWorker[] _renderWorkers = new D3D12RenderWorker[1];
    private readonly WindowProcedure _windowProcedure;
    private readonly bool _qualification;
    private readonly int _qualificationRefreshHz;
    private readonly bool _ownedSmoke;
    private readonly ManualResetEventSlim _ownedSmokeInputDrained = new(false);
    private int _contentFrameId;
    private long _qualificationTickCount;
    private long _qualificationAnimationStarted;
    private long _qualificationAnimationElapsed;
    private nint _window;
    private nint _windowBackgroundBrush;
    private readonly nint[] _renderWindows = new nint[1];
    private int _renderCapacityWidth;
    private int _renderCapacityHeight;
    private PreparedResizeTarget? _preparedResize;
    private NativeRect _committedOuter;
    private bool _hasCommittedOuter;
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
    private long _fixedOriginPreparedPresentCount;
    private long _preparedOuterMismatchCount;
    private long _wmSizingPreparedCount;
    private long _windowPosPreparationFallbackCount;
    private long _clipOnlyShrinkCount;
    private long _asyncShrinkPublishCount;
    private long _asyncOriginMovingPublishCount;
    private long _compositionVisualEpoch;
    private NativeRect _ownedHostRect;
    private NativeRect _ownedWindowRect;
    private NativeRect _ownedDragStartRect;
    private NativePoint _ownedDragStartPointer;
    private OwnedDragMode _ownedDragMode;
    private long _ownedResizeInputCount;
    private long _ownedResizePublishCount;
    private long _ownedSmokeDrainTimeoutCount;
    private Exception? _failure;
    private bool _disposed;

    private Program(
        string arm,
        string? evidencePath,
        bool qualification,
        int qualificationRefreshHz,
        bool ownedSmoke)
    {
        _arm = arm;
        _evidencePath = evidencePath;
        _windowProcedure = WindowProc;
        _qualification = qualification;
        _qualificationRefreshHz = qualificationRefreshHz;
        _ownedSmoke = ownedSmoke;
        if (arm is "A" or "S" or "C" or "N")
        {
            for (var index = 0; index < _presenters.Length; index++)
            {
                _presenters[index] = new DirectHwndPresenter(
                    arm,
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
        var ownedSmoke = args.Contains("--owned-smoke", StringComparer.Ordinal);
        var qualificationRefreshHz = int.TryParse(ReadArgument(args, "--refresh-hz"), out var parsedRefreshHz)
            ? Math.Clamp(parsedRefreshHz, 30, 1000)
            : 165;
        if (arm == "B")
        {
            WriteUnsupportedArmB(evidence);
            Console.Error.WriteLine(
                "Legacy Arm B is retired; use Arm C for the native DirectComposition comparison. The Windows App SDK 2.4 product migration remains a separate gate.");
            return 2;
        }
        if (arm is not ("A" or "S" or "C" or "N"))
        {
            Console.Error.WriteLine($"Unknown arm '{arm}'. Use A, S, C, or N (legacy B remains unavailable).");
            return 2;
        }
        if (arm == "N" && qualification)
        {
            Console.Error.WriteLine("Arm N owns a custom composition resize surface and is not compatible with the standard-HWND qualification driver.");
            return 2;
        }
        if (ownedSmoke && arm != "N")
        {
            Console.Error.WriteLine("--owned-smoke is available only for Arm N.");
            return 2;
        }

        SetProcessDpiAwarenessContext(PerMonitorAwareV2);
        using var application = new Program(
            arm,
            evidence,
            qualification,
            qualificationRefreshHz,
            ownedSmoke);
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
        if (_arm != "N")
        {
            _windowBackgroundBrush = CreateSolidBrush(WindowBackgroundColorRef);
            if (_windowBackgroundBrush == 0)
                throw new InvalidOperationException($"CreateSolidBrush failed: {Marshal.GetLastWin32Error()}.");
        }
        var windowClass = new WindowClassEx
        {
            Size = checked((uint)Marshal.SizeOf<WindowClassEx>()),
            Procedure = Marshal.GetFunctionPointerForDelegate(_windowProcedure),
            Instance = GetModuleHandle(null),
            Cursor = LoadCursor(0, (nint)32512),
            Background = _windowBackgroundBrush,
            ClassName = className,
        };
        if (RegisterClassEx(ref windowClass) == 0)
            throw new InvalidOperationException($"RegisterClassEx failed: {Marshal.GetLastWin32Error()}.");
        _window = CreateWindowEx(
            _arm == "N" ? WsExNoRedirectionBitmap : 0,
            className,
            $"Doroti resize Arm {_arm}",
            _arm == "N" ? WsPopup : WsOverlappedWindow,
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
        if (!GetWindowRect(_window, out _committedOuter))
            throw new InvalidOperationException($"GetWindowRect failed: {Marshal.GetLastWin32Error()}.");
        _hasCommittedOuter = true;
        if (!GetClientRect(_window, out var client))
            throw new InvalidOperationException($"GetClientRect failed: {Marshal.GetLastWin32Error()}.");
        var initialClientWidth = Math.Max(1, client.Right - client.Left);
        var initialClientHeight = Math.Max(1, client.Bottom - client.Top);
        // One top-level HWND owns both standard chrome geometry and the DXGI
        // presentation surface. Its monitor-sized capacity stays out of the
        // ordinary interactive-resize hot path and is clipped by that same
        // HWND's client rect, eliminating cross-HWND composition ordering.
        var monitor = MonitorFromWindow(_window, 2); // MONITOR_DEFAULTTONEAREST
        var monitorInfo = new NativeMonitorInfo
        {
            Size = checked((uint)Marshal.SizeOf<NativeMonitorInfo>()),
        };
        var hasMonitorInfo = monitor != 0 && GetMonitorInfo(monitor, ref monitorInfo);
        if (_arm == "N")
        {
            var work = hasMonitorInfo ? monitorInfo.Work : _committedOuter;
            var workWidth = Math.Max(1, work.Right - work.Left);
            var workHeight = Math.Max(1, work.Bottom - work.Top);
            var ownedWidth = Math.Min(840, Math.Max(320, workWidth - 80));
            var ownedHeight = Math.Min(600, Math.Max(240, workHeight - 80));
            _ownedHostRect = work;
            _ownedWindowRect = new NativeRect
            {
                Left = work.Left + Math.Max(0, (workWidth - ownedWidth) / 2),
                Top = work.Top + Math.Max(0, (workHeight - ownedHeight) / 2),
                Right = work.Left + Math.Max(0, (workWidth - ownedWidth) / 2) + ownedWidth,
                Bottom = work.Top + Math.Max(0, (workHeight - ownedHeight) / 2) + ownedHeight,
            };
            if (!SetWindowPos(
                _window,
                0,
                work.Left,
                work.Top,
                workWidth,
                workHeight,
                SwpNoActivate))
                throw new InvalidOperationException($"SetWindowPos for the Arm N composition host failed: {Marshal.GetLastWin32Error()}.");
            if (!GetClientRect(_window, out client))
                throw new InvalidOperationException($"GetClientRect for the Arm N composition host failed: {Marshal.GetLastWin32Error()}.");
            initialClientWidth = Math.Max(1, client.Right - client.Left);
            initialClientHeight = Math.Max(1, client.Bottom - client.Top);
            _committedOuter = _ownedWindowRect;
            var visualEpoch = Interlocked.Increment(ref _compositionVisualEpoch);
            _presenters[0].StageOwnedVisual(
                visualEpoch,
                _ownedWindowRect.Left - _ownedHostRect.Left,
                _ownedWindowRect.Top - _ownedHostRect.Top,
                _ownedWindowRect.Right - _ownedWindowRect.Left,
                _ownedWindowRect.Bottom - _ownedWindowRect.Top);
        }
        _renderCapacityWidth = Math.Max(
            initialClientWidth,
            hasMonitorInfo ? monitorInfo.Work.Right - monitorInfo.Work.Left : initialClientWidth);
        _renderCapacityHeight = Math.Max(
            initialClientHeight,
            hasMonitorInfo ? monitorInfo.Work.Bottom - monitorInfo.Work.Top : initialClientHeight);
        for (var index = 0; index < _renderWindows.Length; index++)
            _renderWindows[index] = _window;
        for (var index = 0; index < _renderWindows.Length; index++)
        {
            if (!CommitRenderWindow(
                index,
                _renderCapacityWidth,
                _renderCapacityHeight,
                TimeSpan.FromSeconds(5),
                recordResizeHandshake: false,
                contentWidth: _arm == "N"
                    ? _ownedWindowRect.Right - _ownedWindowRect.Left
                    : initialClientWidth,
                contentHeight: _arm == "N"
                    ? _ownedWindowRect.Bottom - _ownedWindowRect.Top
                    : initialClientHeight))
                throw new TimeoutException($"Initial top-level D3D12 present {index} exceeded five seconds.");
        }
        ShowWindow(_window, SwShow);
        UpdateWindow(_window);
        if (_ownedSmoke) StartOwnedSmoke();
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
                    if (_arm == "N") return 1;
                    _wmSizingCount++;
                    PrepublishSizingTarget(lParam);
                    // WM_SIZING hands us the authoritative screen-space drag
                    // rectangle. We consume it directly and must return TRUE;
                    // returning DefWindowProc's result reports it unhandled
                    // and can defer origin-moving edge admission.
                    return 1;
                case WmWindowPosChanging:
                    if (_arm == "N") return DefWindowProc(window, message, wParam, lParam);
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
                case WmOwnedSmoke:
                    if (_arm == "N")
                    {
                        if (wParam == 120)
                            _ownedSmokeInputDrained.Set();
                        else
                            ApplyOwnedSmokeStep(checked((int)wParam));
                        return 0;
                    }
                    break;
                case WmSize:
                    _wmSizeCount++;
                    if (wParam == SizeMinimized)
                    {
                        _zeroSizeCount++;
                        return 0;
                    }
                    if (_arm == "N") return 0;
                    CommitExactRenderChild();
                    return 0;
                case WmDpiChanged:
                    _dpiChangeCount++;
                    if (_arm == "N") return 0;
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
                    if (_arm == "N") return 1;
                    if (_windowBackgroundBrush != 0 &&
                        GetClientRect(window, out var eraseRect) &&
                        FillRect((nint)wParam, ref eraseRect, _windowBackgroundBrush) != 0) return 1;
                    return 0;
                case WmPaint:
                    ValidateRect(window, 0);
                    return 0;
                case WmNcHitTest:
                    if (_arm == "N") return OwnedHitTest() == OwnedDragMode.None
                        ? HitTestTransparent
                        : HitTestClient;
                    break;
                case WmSetCursor:
                    if (_arm == "N")
                    {
                        SetOwnedCursor(OwnedHitTest());
                        return 1;
                    }
                    break;
                case WmLeftButtonDown:
                    if (_arm == "N")
                    {
                        BeginOwnedDrag();
                        return 0;
                    }
                    break;
                case WmMouseMove:
                    if (_arm == "N" && _ownedDragMode != OwnedDragMode.None)
                    {
                        if ((wParam & MouseKeyLeftButton) != 0) UpdateOwnedDrag();
                        else EndOwnedDrag();
                        return 0;
                    }
                    break;
                case WmLeftButtonUp:
                    if (_arm == "N")
                    {
                        EndOwnedDrag();
                        return 0;
                    }
                    break;
                case WmCaptureChanged:
                    if (_arm == "N")
                    {
                        _ownedDragMode = OwnedDragMode.None;
                        return 0;
                    }
                    break;
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

    private OwnedDragMode OwnedHitTest()
    {
        if (!GetCursorPos(out var pointer)) return OwnedDragMode.None;
        var rect = _ownedWindowRect;
        if (pointer.X < rect.Left || pointer.X >= rect.Right ||
            pointer.Y < rect.Top || pointer.Y >= rect.Bottom) return OwnedDragMode.None;
        var dpi = _window == 0 ? 96u : Math.Max(96u, GetDpiForWindow(_window));
        var resizeBand = Math.Max(6, checked((int)Math.Round(8 * dpi / 96.0)));
        var mode = OwnedDragMode.None;
        if (pointer.X < rect.Left + resizeBand) mode |= OwnedDragMode.Left;
        else if (pointer.X >= rect.Right - resizeBand) mode |= OwnedDragMode.Right;
        if (pointer.Y < rect.Top + resizeBand) mode |= OwnedDragMode.Top;
        else if (pointer.Y >= rect.Bottom - resizeBand) mode |= OwnedDragMode.Bottom;
        if (mode != OwnedDragMode.None) return mode;
        var titleBand = Math.Max(resizeBand + 1, checked((int)Math.Round(56 * dpi / 96.0)));
        return pointer.Y < rect.Top + titleBand ? OwnedDragMode.Move : OwnedDragMode.Client;
    }

    private void SetOwnedCursor(OwnedDragMode mode)
    {
        var cursorId = mode switch
        {
            OwnedDragMode.Left or OwnedDragMode.Right => 32644, // IDC_SIZEWE
            OwnedDragMode.Top or OwnedDragMode.Bottom => 32645, // IDC_SIZENS
            OwnedDragMode.Left | OwnedDragMode.Top or
            OwnedDragMode.Right | OwnedDragMode.Bottom => 32642, // IDC_SIZENWSE
            OwnedDragMode.Right | OwnedDragMode.Top or
            OwnedDragMode.Left | OwnedDragMode.Bottom => 32643, // IDC_SIZENESW
            OwnedDragMode.Move => 32646, // IDC_SIZEALL
            _ => 32512, // IDC_ARROW
        };
        SetCursor(LoadCursor(0, (nint)cursorId));
    }

    private void BeginOwnedDrag()
    {
        var mode = OwnedHitTest();
        if (mode is OwnedDragMode.None or OwnedDragMode.Client) return;
        if (!GetCursorPos(out _ownedDragStartPointer)) return;
        _ownedDragStartRect = _ownedWindowRect;
        _ownedDragMode = mode;
        SetCapture(_window);
    }

    private void UpdateOwnedDrag()
    {
        if (!GetCursorPos(out var pointer)) return;
        var deltaX = pointer.X - _ownedDragStartPointer.X;
        var deltaY = pointer.Y - _ownedDragStartPointer.Y;
        var target = _ownedDragStartRect;
        if ((_ownedDragMode & OwnedDragMode.Move) != 0)
        {
            target.Left += deltaX;
            target.Right += deltaX;
            target.Top += deltaY;
            target.Bottom += deltaY;
        }
        else
        {
            if ((_ownedDragMode & OwnedDragMode.Left) != 0) target.Left += deltaX;
            if ((_ownedDragMode & OwnedDragMode.Right) != 0) target.Right += deltaX;
            if ((_ownedDragMode & OwnedDragMode.Top) != 0) target.Top += deltaY;
            if ((_ownedDragMode & OwnedDragMode.Bottom) != 0) target.Bottom += deltaY;
        }
        NormalizeOwnedRect(ref target);
        PublishOwnedRect(target);
    }

    private void PublishOwnedRect(NativeRect target)
    {
        if (RectsEqual(target, _ownedWindowRect)) return;
        _ownedWindowRect = target;
        Interlocked.Increment(ref _ownedResizeInputCount);
        var visualEpoch = Interlocked.Increment(ref _compositionVisualEpoch);
        var width = target.Right - target.Left;
        var height = target.Bottom - target.Top;
        _presenters[0].StageOwnedVisual(
            visualEpoch,
            target.Left - _ownedHostRect.Left,
            target.Top - _ownedHostRect.Top,
            width,
            height);
        QueueLatestRenderWindow(width, height, visualEpoch, force: true);
        Interlocked.Increment(ref _ownedResizePublishCount);
    }

    private void StartOwnedSmoke()
    {
        var smokeThread = new Thread(() =>
        {
            const int stepCount = 120;
            for (var step = 0; step < stepCount; step++)
            {
                if (_window == 0) return;
                PostMessage(_window, WmOwnedSmoke, checked((nuint)step), 0);
                Thread.Sleep(TimeSpan.FromMilliseconds(8));
            }
            if (_window != 0) PostMessage(_window, WmOwnedSmoke, stepCount, 0);
            if (!_ownedSmokeInputDrained.Wait(TimeSpan.FromSeconds(5)))
            {
                Interlocked.Increment(ref _ownedSmokeDrainTimeoutCount);
                _failure ??= new TimeoutException(
                    "Arm N smoke UI message queue did not drain within five seconds.");
            }
            var expectedEpoch = Volatile.Read(ref _compositionVisualEpoch);
            var drainDeadline = Stopwatch.GetTimestamp() + 5 * Stopwatch.Frequency;
            while (_window != 0 &&
                   _presenters[0].OwnedCommittedEpoch != expectedEpoch &&
                   Stopwatch.GetTimestamp() < drainDeadline)
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            if (_window != 0 && _presenters[0].OwnedCommittedEpoch != expectedEpoch)
            {
                Interlocked.Increment(ref _ownedSmokeDrainTimeoutCount);
                _failure ??= new TimeoutException(
                    $"Arm N smoke did not commit latest visual epoch {expectedEpoch} within five seconds.");
            }
            if (_window != 0) PostMessage(_window, WmClose, 0, 0);
        })
        {
            IsBackground = true,
            Name = "Doroti Arm N owned resize smoke",
        };
        smokeThread.Start();
    }

    private void ApplyOwnedSmokeStep(int step)
    {
        const int phaseLength = 20;
        var initial = _committedOuter;
        var phase = Math.Clamp(step / phaseLength, 0, 5);
        var phaseStep = step % phaseLength;
        var direction = phase % 2 == 0 ? phaseStep : phaseLength - phaseStep;
        var amountX = checked((int)Math.Round(120 * direction / (double)phaseLength));
        var amountY = checked((int)Math.Round(80 * direction / (double)phaseLength));
        var target = initial;
        switch (phase / 2)
        {
            case 0:
                target.Left -= amountX;
                break;
            case 1:
                target.Top -= amountY;
                break;
            default:
                target.Left -= amountX;
                target.Top -= amountY;
                break;
        }
        target.Left = Math.Max(_ownedHostRect.Left, target.Left);
        target.Top = Math.Max(_ownedHostRect.Top, target.Top);
        PublishOwnedRect(target);
    }

    private void NormalizeOwnedRect(ref NativeRect rect)
    {
        const int minimumWidth = 320;
        const int minimumHeight = 240;
        var host = _ownedHostRect;
        if ((_ownedDragMode & OwnedDragMode.Move) != 0)
        {
            var width = rect.Right - rect.Left;
            var height = rect.Bottom - rect.Top;
            rect.Left = Math.Clamp(rect.Left, host.Left, Math.Max(host.Left, host.Right - width));
            rect.Top = Math.Clamp(rect.Top, host.Top, Math.Max(host.Top, host.Bottom - height));
            rect.Right = rect.Left + width;
            rect.Bottom = rect.Top + height;
            return;
        }
        if ((_ownedDragMode & OwnedDragMode.Left) != 0)
            rect.Left = Math.Clamp(rect.Left, host.Left, rect.Right - minimumWidth);
        if ((_ownedDragMode & OwnedDragMode.Right) != 0)
            rect.Right = Math.Clamp(rect.Right, rect.Left + minimumWidth, host.Right);
        if ((_ownedDragMode & OwnedDragMode.Top) != 0)
            rect.Top = Math.Clamp(rect.Top, host.Top, rect.Bottom - minimumHeight);
        if ((_ownedDragMode & OwnedDragMode.Bottom) != 0)
            rect.Bottom = Math.Clamp(rect.Bottom, rect.Top + minimumHeight, host.Bottom);
    }

    private void EndOwnedDrag()
    {
        if (_ownedDragMode == OwnedDragMode.None) return;
        _ownedDragMode = OwnedDragMode.None;
        if (GetCapture() == _window) ReleaseCapture();
    }

    private static bool RectsEqual(NativeRect left, NativeRect right) =>
        left.Left == right.Left && left.Top == right.Top &&
        left.Right == right.Right && left.Bottom == right.Bottom;

    private void PrecommitExpandedCoverForWindowPosition(nint windowPositionPointer)
    {
        if (_window == 0 || _renderWindows[0] == 0 ||
            windowPositionPointer == 0 ||
            !GetWindowRect(_window, out var currentOuter) ||
            !GetClientRect(_window, out var currentClient)) return;
        var position = Marshal.PtrToStructure<NativeWindowPosition>(windowPositionPointer);
        if ((position.Flags & 0x0001) != 0) return; // SWP_NOSIZE
        var targetOuterLeft = (position.Flags & 0x0002) != 0 ? currentOuter.Left : position.X; // SWP_NOMOVE
        var targetOuterTop = (position.Flags & 0x0002) != 0 ? currentOuter.Top : position.Y;
        var targetOuter = new NativeRect
        {
            Left = targetOuterLeft,
            Top = targetOuterTop,
            Right = targetOuterLeft + position.Width,
            Bottom = targetOuterTop + position.Height,
        };
        if (targetOuter.Right - targetOuter.Left == currentOuter.Right - currentOuter.Left &&
            targetOuter.Bottom - targetOuter.Top == currentOuter.Bottom - currentOuter.Top)
        {
            // SetWindowPos can omit SWP_NOSIZE even when it is only moving a
            // window. A pure move has no matching WM_SIZE, so creating an Arm
            // C visual epoch here would leave geometry admission closed and
            // incorrectly treat translation as an edge resize.
            return;
        }
        var target = CreatePreparedResizeTarget(currentOuter, currentClient, targetOuter);
        if (_preparedResize is { } prepared &&
            prepared.Matches(targetOuter, target.ContentWidth, target.ContentHeight)) return;
        Interlocked.Increment(ref _windowPosPreparationFallbackCount);
        if (TryPrepareResizeTarget(currentOuter, currentClient, targetOuter)) return;
        HoldCurrentWindowPosition(ref position, currentOuter, windowPositionPointer);
    }

    private void PrepublishSizingTarget(nint sizingRectPointer)
    {
        if (_window == 0 || _renderWindows[0] == 0 ||
            sizingRectPointer == 0 ||
            !GetWindowRect(_window, out var currentOuter) ||
            !GetClientRect(_window, out var currentClient)) return;
        var targetOuter = Marshal.PtrToStructure<NativeRect>(sizingRectPointer);
        if (TryPrepareResizeTarget(currentOuter, currentClient, targetOuter))
        {
            Interlocked.Increment(ref _wmSizingPreparedCount);
            return;
        }
        Marshal.StructureToPtr(currentOuter, sizingRectPointer, false);
        Interlocked.Increment(ref _precommitGeometryHoldCount);
    }

    private bool TryPrepareResizeTarget(
        NativeRect currentOuter,
        NativeRect currentClient,
        NativeRect targetOuter)
    {
        var target = CreatePreparedResizeTarget(currentOuter, currentClient, targetOuter);
        if (_preparedResize is { } prepared &&
            prepared.Matches(targetOuter, target.ContentWidth, target.ContentHeight)) return true;
        _preparedResize = null;
        var currentWidth = _renderCapacityWidth;
        var currentHeight = _renderCapacityHeight;
        var visibleWidth = Math.Max(1, currentClient.Right - currentClient.Left);
        var visibleHeight = Math.Max(1, currentClient.Bottom - currentClient.Top);
        var requiresSynchronousPrepresent = _arm == "A" && target.RequiresSynchronousPrepresent;
        if (!requiresSynchronousPrepresent)
        {
            // Arm A keeps the prior asynchronous origin-moving/shrink policy.
            // Arm S lets DXGI stretch the last exact source rect until the next
            // exact frame. Arm C first commits an edge-aware DirectComposition
            // translate/clip, then admits geometry without a DwmFlush wait.
            if (_arm == "C")
            {
                var visualEpoch = Interlocked.Increment(ref _compositionVisualEpoch);
                target = target with { VisualEpoch = visualEpoch };
                _presenters[0].ApplyProvisionalVisual(
                    visualEpoch,
                    targetOuter.Right == currentOuter.Right
                        ? currentOuter.Left - targetOuter.Left
                        : 0,
                    targetOuter.Bottom == currentOuter.Bottom
                        ? currentOuter.Top - targetOuter.Top
                        : 0,
                    visibleWidth,
                    visibleHeight,
                    target.ContentWidth,
                    target.ContentHeight);
            }
            _preparedResize = target;
            if (target.RequiresExpandedCoverage)
            {
                QueueLatestRenderWindow(
                    target.ContentWidth,
                    target.ContentHeight,
                    target.VisualEpoch);
                Interlocked.Increment(ref _asyncOriginMovingPublishCount);
            }
            else
            {
                Interlocked.Increment(ref _clipOnlyShrinkCount);
            }
            return true;
        }
        var coverWidth = Math.Max(currentWidth, target.ContentWidth);
        var coverHeight = Math.Max(currentHeight, target.ContentHeight);
        if (coverWidth != currentWidth || coverHeight != currentHeight)
        {
            if (!CommitRenderWindow(
                0, coverWidth, coverHeight,
                TimeSpan.FromMilliseconds(100),
                recordResizeHandshake: true,
                flushBeforeAck: true,
                contentWidth: visibleWidth,
                contentHeight: visibleHeight)) return false;
            _renderCapacityWidth = coverWidth;
            _renderCapacityHeight = coverHeight;
            Interlocked.Increment(ref _precommitSizingCount);
        }

        if (!CommitRenderWindow(
            0, coverWidth, coverHeight,
            TimeSpan.FromMilliseconds(100),
            recordResizeHandshake: true,
            flushBeforeAck: true,
            contentWidth: target.ContentWidth,
            contentHeight: target.ContentHeight,
            present: true)) return false;
        // The exact frame is already composed while the old geometry is still
        // admitted. Only after this ordering point may WINDOWPOS consume the
        // matching outer rect, so DWM cannot expose geometry-first pixels.
        _preparedResize = target;
        Interlocked.Increment(ref _fixedOriginPreparedPresentCount);
        return true;
    }

    private static PreparedResizeTarget CreatePreparedResizeTarget(
        NativeRect currentOuter,
        NativeRect currentClient,
        NativeRect targetOuter)
    {
        var nonClientWidth = Math.Max(0,
            (currentOuter.Right - currentOuter.Left) - (currentClient.Right - currentClient.Left));
        var nonClientHeight = Math.Max(0,
            (currentOuter.Bottom - currentOuter.Top) - (currentClient.Bottom - currentClient.Top));
        var contentWidth = Math.Max(1, targetOuter.Right - targetOuter.Left - nonClientWidth);
        var contentHeight = Math.Max(1, targetOuter.Bottom - targetOuter.Top - nonClientHeight);
        return new PreparedResizeTarget(
            contentWidth,
            contentHeight,
            targetOuter.Left,
            targetOuter.Top,
            targetOuter.Right - targetOuter.Left,
            targetOuter.Bottom - targetOuter.Top,
            targetOuter.Left != currentOuter.Left || targetOuter.Top != currentOuter.Top,
            contentWidth > Math.Max(1, currentClient.Right - currentClient.Left) ||
            contentHeight > Math.Max(1, currentClient.Bottom - currentClient.Top));
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
        if (_window == 0 ||
            !GetClientRect(_window, out var client) ||
            !GetWindowRect(_window, out var outer)) return;
        var width = Math.Max(0, client.Right - client.Left);
        var height = Math.Max(0, client.Bottom - client.Top);
        if (width == 0 || height == 0)
        {
            _zeroSizeCount++;
            return;
        }
        var preparedTarget = _preparedResize;
        var prepared = preparedTarget is { } target && target.Matches(outer, width, height);
        var committedOuterWidth = _committedOuter.Right - _committedOuter.Left;
        var committedOuterHeight = _committedOuter.Bottom - _committedOuter.Top;
        var clipsCommittedFrame = _hasCommittedOuter &&
            outer.Right - outer.Left <= committedOuterWidth &&
            outer.Bottom - outer.Top <= committedOuterHeight;
        var originMoved = prepared
            ? preparedTarget!.Value.OriginMoves
            : _hasCommittedOuter &&
              (outer.Left != _committedOuter.Left || outer.Top != _committedOuter.Top);
        if (preparedTarget is not null && !prepared)
            Interlocked.Increment(ref _preparedOuterMismatchCount);
        if (prepared)
        {
            var preparedValue = preparedTarget!.Value;
            if (_arm != "A" || !preparedValue.RequiresSynchronousPrepresent)
            {
                if (_arm == "C")
                {
                    // The worker may have finished the exact raster that was
                    // requested from WM_SIZING, but it must not make that
                    // frame visible (and reset the provisional anchor) until
                    // this matching WM_SIZE proves HWND geometry admission.
                    _presenters[0].AdmitCompositionGeometry(preparedValue.VisualEpoch);
                }
                QueueLatestRenderWindow(
                    width,
                    height,
                    preparedValue.VisualEpoch,
                    force: _arm == "C");
                if (!preparedValue.RequiresExpandedCoverage)
                    Interlocked.Increment(ref _asyncShrinkPublishCount);
            }
            // Fixed-origin expansion was presented before geometry admission.
            // Origin-moving resize is never held behind GPU/compositor work.
        }
        else if (clipsCommittedFrame)
        {
            // A one-pixel non-client rounding difference must not fall back to
            // a synchronous origin-moving present during a shrink.
            QueueLatestRenderWindow(width, height, CurrentCompositionVisualEpoch());
            Interlocked.Increment(ref _asyncShrinkPublishCount);
        }
        else if (originMoved)
        {
            // Preserve pointer/edge coupling even if a suggested outer rect
            // differs from WM_SIZE by non-client rounding.
            QueueLatestRenderWindow(width, height, CurrentCompositionVisualEpoch());
            Interlocked.Increment(ref _asyncOriginMovingPublishCount);
        }
        else if (!CommitRenderWindow(
            0,
            Math.Max(_renderCapacityWidth, width),
            Math.Max(_renderCapacityHeight, height),
            TimeSpan.FromMilliseconds(100),
            recordResizeHandshake: true,
            flushBeforeAck: originMoved,
            contentWidth: width,
            contentHeight: height)) return;
        EnsureRenderCapacity(width, height);
        _preparedResize = null;
        _committedOuter = outer;
        _hasCommittedOuter = true;
    }

    private long CurrentCompositionVisualEpoch() =>
        _arm is "C" or "N" ? Volatile.Read(ref _compositionVisualEpoch) : 0;

    private void QueueLatestRenderWindow(
        int contentWidth,
        int contentHeight,
        long visualEpoch = 0,
        bool force = false)
    {
        var renderWindow = _renderWindows[0];
        if (renderWindow == 0) return;
        var scaleOwner = _window != 0 ? _window : renderWindow;
        var scale = Math.Max(1, GetDpiForWindow(scaleOwner)) / 96.0;
        _renderWorkers[0].UpdateTarget(
            renderWindow,
            Math.Max(_renderCapacityWidth, contentWidth),
            Math.Max(_renderCapacityHeight, contentHeight),
            contentWidth,
            contentHeight,
            scale,
            visualEpoch,
            force);
    }

    private void EnsureRenderCapacity(int requiredWidth, int requiredHeight)
    {
        _renderCapacityWidth = Math.Max(_renderCapacityWidth, requiredWidth);
        _renderCapacityHeight = Math.Max(_renderCapacityHeight, requiredHeight);
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
            Volatile.Read(ref _contentFrameId),
            CurrentCompositionVisualEpoch());
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
            visibleOwner = _arm switch
            {
                "A" => "single standard-chrome top-level HWND + Scaling.None baseline",
                "S" => "single standard-chrome top-level HWND + transient DXGI Scaling.Stretch source rect",
                "C" => "standard-chrome top-level HWND + DirectComposition edge-aware visual transaction",
                "N" => "monitor envelope HWND + app-owned chrome/content DirectComposition transaction",
                _ => "unavailable",
            },
            rawRenderChildHwndCount = 0,
            renderCapacityWidth = _renderCapacityWidth,
            renderCapacityHeight = _renderCapacityHeight,
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
            fixedOriginPreparedPresentCount = Volatile.Read(ref _fixedOriginPreparedPresentCount),
            preparedOuterMismatchCount = Volatile.Read(ref _preparedOuterMismatchCount),
            wmSizingPreparedCount = Volatile.Read(ref _wmSizingPreparedCount),
            windowPosPreparationFallbackCount = Volatile.Read(ref _windowPosPreparationFallbackCount),
            clipOnlyShrinkCount = Volatile.Read(ref _clipOnlyShrinkCount),
            asyncShrinkPublishCount = Volatile.Read(ref _asyncShrinkPublishCount),
            asyncOriginMovingPublishCount = Volatile.Read(ref _asyncOriginMovingPublishCount),
            supersededResizeFrameCount = _renderWorkers.Sum(
                worker => worker?.SupersededResizeFrameCount ?? 0),
            stretchSourceSizeCommitCount = _presenters.Sum(
                presenter => presenter?.StretchSourceSizeCommitCount ?? 0),
            compositionProvisionalCommitCount = _presenters.Sum(
                presenter => presenter?.CompositionProvisionalCommitCount ?? 0),
            compositionExactCommitCount = _presenters.Sum(
                presenter => presenter?.CompositionExactCommitCount ?? 0),
            compositionStaleExactRejectCount = _presenters.Sum(
                presenter => presenter?.CompositionStaleExactRejectCount ?? 0),
            compositionPreAdmissionExactRejectCount = _presenters.Sum(
                presenter => presenter?.CompositionPreAdmissionExactRejectCount ?? 0),
            compositionGeometryAdmissionCount = _presenters.Sum(
                presenter => presenter?.CompositionGeometryAdmissionCount ?? 0),
            compositionGeometryAdmissionRejectCount = _presenters.Sum(
                presenter => presenter?.CompositionGeometryAdmissionRejectCount ?? 0),
            ownedResizeInputCount = Volatile.Read(ref _ownedResizeInputCount),
            ownedResizePublishCount = Volatile.Read(ref _ownedResizePublishCount),
            ownedCompositionCommitCount = _presenters.Sum(
                presenter => presenter?.OwnedCompositionCommitCount ?? 0),
            ownedCommittedEpoch = _presenters.Max(
                presenter => presenter?.OwnedCommittedEpoch ?? 0),
            ownedLatestEpoch = Volatile.Read(ref _compositionVisualEpoch),
            ownedSmokeDrainTimeoutCount = Volatile.Read(ref _ownedSmokeDrainTimeoutCount),
            ownedSmoke = _ownedSmoke,
            ownedHostRect = new
            {
                left = _ownedHostRect.Left,
                top = _ownedHostRect.Top,
                right = _ownedHostRect.Right,
                bottom = _ownedHostRect.Bottom,
            },
            ownedFinalRect = new
            {
                left = _ownedWindowRect.Left,
                top = _ownedWindowRect.Top,
                right = _ownedWindowRect.Right,
                bottom = _ownedWindowRect.Bottom,
            },
            qualification = _qualification,
            qualificationRefreshHz = _qualificationRefreshHz,
            qualificationTickCount = qualificationFrames,
            qualificationAnimationSeconds = animationSeconds,
            qualificationPresentedFramesPerSecond = animationSeconds > 0 ? qualificationFrames / animationSeconds : 0,
            qualificationRenderBackend = _arm is "C" or "N"
                ? "D3D12 dedicated render worker + composition frame-latency waitable cadence; Present(0) resize commits"
                : "D3D12 dedicated render worker + DXGI Present(1) visible cadence; Present(0) resize commits",
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
            reason = "Legacy lifted Microsoft.UI.Composition Arm B is retired. Arm C owns the native DirectComposition comparison while the exact Windows App SDK 2.4 product migration remains a separate gate.",
        }, new JsonSerializerOptions { WriteIndented = true }));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var worker in _renderWorkers) worker?.Dispose();
        foreach (var presenter in _presenters) presenter?.Dispose();
        _ownedSmokeInputDrained.Dispose();
        if (_windowBackgroundBrush != 0)
        {
            DeleteObject(_windowBackgroundBrush);
            _windowBackgroundBrush = 0;
        }
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
    private struct NativePoint
    {
        internal int X;
        internal int Y;
    }

    [Flags]
    private enum OwnedDragMode
    {
        None = 0,
        Client = 1,
        Move = 2,
        Left = 4,
        Top = 8,
        Right = 16,
        Bottom = 32,
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
    private struct NativeMonitorInfo
    {
        internal uint Size;
        internal NativeRect Monitor;
        internal NativeRect Work;
        internal uint Flags;
    }

    private readonly record struct PreparedResizeTarget(
        int ContentWidth,
        int ContentHeight,
        int OuterLeft,
        int OuterTop,
        int OuterWidth,
        int OuterHeight,
        bool OriginMoves,
        bool RequiresExpandedCoverage,
        long VisualEpoch = 0)
    {
        internal bool RequiresSynchronousPrepresent =>
            RequiresExpandedCoverage && !OriginMoves;

        internal bool Matches(NativeRect outer, int contentWidth, int contentHeight) =>
            ContentWidth == contentWidth &&
            ContentHeight == contentHeight &&
            OuterLeft == outer.Left &&
            OuterTop == outer.Top &&
            OuterWidth == outer.Right - outer.Left &&
            OuterHeight == outer.Bottom - outer.Top;
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
    private static extern nint MonitorFromWindow(nint window, uint flags);

    [DllImport("user32.dll", EntryPoint = "GetMonitorInfoW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(nint monitor, ref NativeMonitorInfo info);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ValidateRect(nint window, nint rect);

    [DllImport("user32.dll")]
    private static extern nint LoadCursor(nint instance, nint cursorName);

    [DllImport("user32.dll")]
    private static extern nint SetCursor(nint cursor);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out NativePoint point);

    [DllImport("user32.dll")]
    private static extern nint SetCapture(nint window);

    [DllImport("user32.dll")]
    private static extern nint GetCapture();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ReleaseCapture();

    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern nint CreateSolidBrush(uint colorRef);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteObject(nint value);

    [DllImport("user32.dll")]
    private static extern int FillRect(nint deviceContext, ref NativeRect rect, nint brush);

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
    private long _visualEpoch;
    private long _targetVersion;
    private long _renderedTargetVersion;
    private bool _animate;
    private bool _stopping;
    private int _frameId;
    private long _supersededResizeFrameCount;

    internal long SupersededResizeFrameCount =>
        Volatile.Read(ref _supersededResizeFrameCount);

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

    internal void UpdateTarget(
        nint window,
        int width,
        int height,
        int contentWidth,
        int contentHeight,
        double scale,
        long visualEpoch,
        bool force = false)
    {
        lock (_gate)
        {
            if (!force &&
                _window == window &&
                _width == width &&
                _height == height &&
                _contentWidth == contentWidth &&
                _contentHeight == contentHeight &&
                _scale.Equals(scale) &&
                _visualEpoch == visualEpoch) return;
            _window = window;
            _width = width;
            _height = height;
            _contentWidth = contentWidth;
            _contentHeight = contentHeight;
            _scale = scale;
            _visualEpoch = visualEpoch;
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
        int frameId,
        long visualEpoch = 0)
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
            _visualEpoch = visualEpoch;
            var version = ++_targetVersion;
            request = new CommitRequest(
                window, width, height, contentWidth, contentHeight,
                scale, version, flushBeforeAck, present, frameId, visualEpoch);
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
                long visualEpoch;
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
                    visualEpoch = _visualEpoch;
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
                    visualEpoch = commit.VisualEpoch;
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
                var coalescedResizeFrame = targetChanged &&
                    commit is null && !animate && singleFrame is null;
                if (animatedFrame || singleFrame is not null) _frameId = unchecked(_frameId + 1);
                try
                {
                    // Resize commits are ordered explicitly by the platform
                    // thread and must not wait for scan-out. Only continuous
                    // visible animation is refresh synchronized.
                    var renderedAndPresented = _presenter.RenderFrame(
                        window, width, height, contentWidth, contentHeight,
                        scale, _frameId, visualEpoch,
                        present: commit?.Present ?? !coalescedResizeFrame,
                        synchronizeToRefresh: animatedFrame || singleFrame is not null);
                    var superseded = false;
                    lock (_gate)
                    {
                        superseded = coalescedResizeFrame && targetVersion != _targetVersion;
                        if (targetVersion > _renderedTargetVersion) _renderedTargetVersion = targetVersion;
                    }
                    var presented = renderedAndPresented;
                    if (coalescedResizeFrame)
                    {
                        if (superseded)
                        {
                            Interlocked.Increment(ref _supersededResizeFrameCount);
                        }
                        else
                        {
                            // Publish only the newest origin-moving target and
                            // do not add a refresh wait after the HWND moved.
                            presented = _presenter.PresentPrepared(
                                synchronizeToRefresh: false,
                                expectedVisualEpoch: visualEpoch);
                        }
                    }
                    if (presented) _presented(animatedFrame, _frameId);
                    else if (animatedFrame)
                    {
                        // A provisional Arm C epoch can briefly wait for the
                        // matching WM_SIZE. Do not busy-spin or count those
                        // deferred frames as visible cadence.
                        _wake.WaitOne(TimeSpan.FromMilliseconds(10));
                    }
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
        int FrameId,
        long VisualEpoch)
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
    private readonly string _arm;
    private readonly bool _drawRightEdgeOracle;
    private readonly object _presentGate = new();
    private readonly object _compositionGate = new();
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
    private DirectCompositionVisualBridge? _composition;
    private ulong _nextFence;
    private ulong _submittedFence;
    private int _width;
    private int _height;
    private int _preparedContentWidth;
    private int _preparedContentHeight;
    private long _preparedVisualEpoch;
    private uint _sourceWidth;
    private uint _sourceHeight;
    private nint _frameLatencyWaitableObject;
    private long _stretchSourceSizeCommitCount;
    private long _compositionStaleExactRejectCount;
    private long _compositionPreAdmissionExactRejectCount;
    private long _ownedPendingEpoch;
    private int _ownedPendingOffsetX;
    private int _ownedPendingOffsetY;
    private int _ownedPendingWidth;
    private int _ownedPendingHeight;

    internal string AdapterDescription { get; private set; } = "uninitialized";

    internal long StretchSourceSizeCommitCount =>
        Volatile.Read(ref _stretchSourceSizeCommitCount);

    internal long CompositionStaleExactRejectCount =>
        Volatile.Read(ref _compositionStaleExactRejectCount);

    internal long CompositionPreAdmissionExactRejectCount =>
        Volatile.Read(ref _compositionPreAdmissionExactRejectCount);

    internal long CompositionGeometryAdmissionCount =>
        _composition?.GeometryAdmissionCount ?? 0;

    internal long CompositionGeometryAdmissionRejectCount =>
        _composition?.GeometryAdmissionRejectCount ?? 0;

    internal long CompositionProvisionalCommitCount =>
        _composition?.ProvisionalCommitCount ?? 0;

    internal long CompositionExactCommitCount =>
        _composition?.ExactCommitCount ?? 0;

    internal long OwnedCompositionCommitCount =>
        _composition?.OwnedCommitCount ?? 0;

    internal long OwnedCommittedEpoch =>
        _composition?.OwnedCommittedEpoch ?? 0;

    internal DirectHwndPresenter(string arm, bool drawRightEdgeOracle)
    {
        _arm = arm;
        _drawRightEdgeOracle = drawRightEdgeOracle;
    }

    internal void ApplyProvisionalVisual(
        long visualEpoch,
        int offsetX,
        int offsetY,
        int previousContentWidth,
        int previousContentHeight,
        int targetContentWidth,
        int targetContentHeight)
    {
        if (_arm != "C") return;
        lock (_compositionGate)
        {
            (_composition ?? throw new InvalidOperationException(
                "Arm C DirectComposition visual was not initialized."))
                .ApplyProvisional(
                    visualEpoch,
                    offsetX,
                    offsetY,
                    previousContentWidth,
                    previousContentHeight,
                    targetContentWidth,
                    targetContentHeight);
        }
    }

    internal void AdmitCompositionGeometry(long visualEpoch)
    {
        if (_arm != "C") return;
        lock (_compositionGate)
        {
            (_composition ?? throw new InvalidOperationException(
                "Arm C DirectComposition visual was not initialized."))
                .AdmitGeometry(visualEpoch);
        }
    }

    internal void StageOwnedVisual(
        long visualEpoch,
        int offsetX,
        int offsetY,
        int contentWidth,
        int contentHeight)
    {
        if (_arm != "N") return;
        lock (_compositionGate)
        {
            _ownedPendingEpoch = visualEpoch;
            _ownedPendingOffsetX = offsetX;
            _ownedPendingOffsetY = offsetY;
            _ownedPendingWidth = contentWidth;
            _ownedPendingHeight = contentHeight;
            _composition?.StageOwned(
                visualEpoch,
                offsetX,
                offsetY,
                contentWidth,
                contentHeight);
        }
    }

    internal bool RenderFrame(
        nint window,
        int width,
        int height,
        int contentWidth,
        int contentHeight,
        double scale,
        int frameId,
        long visualEpoch,
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
            _width,
            _height,
            Math.Clamp(contentWidth, 1, _width),
            Math.Clamp(contentHeight, 1, _height),
            scale,
            frameId,
            _drawRightEdgeOracle,
            drawOwnedFrame: _arm == "N");
        _backing.Surface.Canvas.Flush();
        _context!.Flush(_backing.Surface);
        _context.Submit(false);
        _preparedContentWidth = Math.Clamp(contentWidth, 1, _width);
        _preparedContentHeight = Math.Clamp(contentHeight, 1, _height);
        _preparedVisualEpoch = visualEpoch;
        return present && PresentPrepared(synchronizeToRefresh);
    }

    internal bool PresentPrepared(
        bool synchronizeToRefresh,
        long? expectedVisualEpoch = null)
    {
        lock (_presentGate)
        {
            if (_backing is null || _swapChain is null)
                throw new InvalidOperationException("No prepared D3D12 frame is available.");
            var visualEpoch = _arm is "C" or "N"
                ? expectedVisualEpoch ?? _composition!.LatestEpoch
                : expectedVisualEpoch ?? _preparedVisualEpoch;
            if (_arm is "C" or "N")
            {
                lock (_compositionGate)
                {
                    if (!CanPublishCompositionExact(visualEpoch)) return false;
                }
            }
            if (_arm is "C" or "N" && synchronizeToRefresh)
                WaitForCompositionFrameLatencySlot();
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
            if (_arm == "S")
            {
                var sourceWidth = checked((uint)_preparedContentWidth);
                var sourceHeight = checked((uint)_preparedContentHeight);
                if (_sourceWidth != sourceWidth || _sourceHeight != sourceHeight)
                {
                    using var swapChain2 = _swapChain.QueryInterface<IDXGISwapChain2>();
                    swapChain2.SetSourceSize(sourceWidth, sourceHeight);
                    _sourceWidth = sourceWidth;
                    _sourceHeight = sourceHeight;
                    Interlocked.Increment(ref _stretchSourceSizeCommitCount);
                }
            }
            // The copy and Present use the same D3D12 queue. Queue ordering is the
            // readiness contract; the next frame waits before reusing the allocator.
            if (_arm is "C" or "N")
            {
                // Do not hold the composition gate while waiting for/reusing
                // GPU resources. WM_SIZING must remain free to update the
                // provisional anchor. Recheck the epoch only at the visible
                // Present+exact-visual transaction boundary.
                lock (_compositionGate)
                {
                    if (!CanPublishCompositionExact(visualEpoch)) return false;
                    _swapChain.Present(
                        synchronizeToRefresh ? 1u : 0u,
                        PresentFlags.None).CheckError();
                    if (_arm == "C")
                    {
                        _composition!.CommitExact(
                            visualEpoch,
                            _preparedContentWidth,
                            _preparedContentHeight);
                    }
                    else
                    {
                        _composition!.CommitOwned(visualEpoch);
                    }
                }
            }
            else
            {
                _swapChain.Present(
                    synchronizeToRefresh ? 1u : 0u,
                    PresentFlags.None).CheckError();
            }
            return true;
        }
    }

    private bool CanPublishCompositionExact(long visualEpoch)
    {
        if (!_composition!.IsLatest(visualEpoch))
        {
            Interlocked.Increment(ref _compositionStaleExactRejectCount);
            return false;
        }
        if (_arm == "N") return true;
        if (!_composition.IsGeometryAdmitted(visualEpoch))
        {
            // WM_SIZING is allowed to prepare the exact backing early, but
            // only matching WM_SIZE geometry admission may publish it.
            Interlocked.Increment(ref _compositionPreAdmissionExactRejectCount);
            return false;
        }
        return true;
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
                false, Usage.RenderTargetOutput, 2,
                _arm == "A" ? Scaling.None : Scaling.Stretch,
                SwapEffect.FlipSequential,
                _arm is "C" or "N" ? AlphaMode.Premultiplied : AlphaMode.Ignore,
                SwapChainFlags.FrameLatencyWaitableObject);
            using var created = _arm is "C" or "N"
                ? _factory!.CreateSwapChainForComposition(_queue!, description, null)
                : _factory!.CreateSwapChainForHwnd(_queue!, window, description);
            // DXGI_SCALING_NONE can expose swap-chain background outside the
            // current back-buffer content while the HWND target changes. Own
            // that fallback explicitly instead of accepting DXGI's white
            // default, and match the Win32 class/WM_ERASEBKGND brush.
            created.BackgroundColor = new Vortice.Mathematics.Color4(
                20.0f / 255.0f,
                18.0f / 255.0f,
                24.0f / 255.0f,
                1.0f);
            _swapChain = created.QueryInterface<IDXGISwapChain3>();
            if (_arm is "C" or "N")
            {
                _composition = new DirectCompositionVisualBridge(
                    window,
                    _swapChain,
                    width,
                    height);
                if (_arm == "N" && _ownedPendingEpoch != 0)
                    _composition.StageOwned(
                        _ownedPendingEpoch,
                        _ownedPendingOffsetX,
                        _ownedPendingOffsetY,
                        _ownedPendingWidth,
                        _ownedPendingHeight);
            }
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
        int surfaceWidth,
        int surfaceHeight,
        int contentWidth,
        int contentHeight,
        double scale,
        int frameId,
        bool drawRightEdgeOracle,
        bool drawOwnedFrame)
    {
        canvas.Clear(new SKColor(20, 18, 24, 255));
        var appBarHeight = Math.Min(contentHeight, Math.Max(1, (int)Math.Round(56 * scale)));
        var purple = new SKColor(103, 58, 183, 255);
        var lavender = new SKColor(179, 157, 219, 255);
        using var paint = new SKPaint { IsAntialias = false, Color = purple };
        // Keep the entire retained front valid, not only the last logical
        // viewport. During a left/top expansion DXGI_SCALING_NONE reanchors
        // the surface before the exact scene can arrive; valid overscan keeps
        // that transient region from becoming a solid fallback band.
        canvas.DrawRect(0, 0, surfaceWidth, appBarHeight, paint);

        var tile = Math.Max(8, (int)Math.Round(16 * scale));
        for (var y = appBarHeight; y < surfaceHeight; y += tile)
        {
            for (var x = 0; x < surfaceWidth; x += tile)
            {
                paint.Color = ((x / tile) + (y / tile)) % 2 == 0
                    ? new SKColor(35, 31, 43, 255)
                    : new SKColor(24, 21, 31, 255);
                canvas.DrawRect(
                    x, y,
                    Math.Min(tile, surfaceWidth - x),
                    Math.Min(tile, surfaceHeight - y), paint);
            }
        }

        paint.IsAntialias = true;
        paint.Color = lavender;
        var radius = Math.Max(18, (float)(24 * scale));
        var centerX = Math.Max(radius + 4, contentWidth * 0.55f);
        var centerY = Math.Max(appBarHeight + radius + 4, contentHeight * 0.55f);
        centerX = Math.Min(contentWidth - radius - 4, centerX);
        centerY = Math.Min(contentHeight - radius - 4, centerY);
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
        var bitStartX = Math.Max(0, contentWidth - bitStripWidth - Math.Max(4, (int)Math.Round(4 * scale)));
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
            (byte)(32 + (contentWidth & 0x7f)),
            (byte)(32 + (contentHeight & 0x7f)),
            (byte)(32 + ((contentWidth ^ contentHeight) & 0x7f)),
            255);
        canvas.DrawRect(bitStartX, bitTop + bitSize + bitGap, bitStripWidth, Math.Max(2, bitGap * 2), paint);

        if (drawRightEdgeOracle)
        {
            paint.IsAntialias = false;
            paint.Color = purple;
            var edgeWidth = Math.Max(2, (int)Math.Ceiling(scale));
            canvas.DrawRect(
                contentWidth - edgeWidth, appBarHeight,
                edgeWidth, contentHeight - appBarHeight, paint);
        }

        if (drawOwnedFrame)
        {
            paint.IsAntialias = false;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = Math.Max(2, (float)Math.Round(3 * scale));
            paint.Color = new SKColor(179, 157, 219, 255);
            var inset = paint.StrokeWidth / 2;
            canvas.DrawRect(
                inset,
                inset,
                Math.Max(1, contentWidth - paint.StrokeWidth),
                Math.Max(1, contentHeight - paint.StrokeWidth),
                paint);
            paint.Style = SKPaintStyle.Fill;
        }
    }

    private void WaitForGpu()
    {
        if (_submittedFence == 0 || _fence!.CompletedValue >= _submittedFence) return;
        _fence.SetEventOnCompletion(_submittedFence, _fenceEvent!).CheckError();
        if (!_fenceEvent!.WaitOne(TimeSpan.FromSeconds(5)))
            throw new TimeoutException($"N0 D3D12 fence {_submittedFence} timed out.");
    }

    private void WaitForCompositionFrameLatencySlot()
    {
        if (_frameLatencyWaitableObject == 0)
            throw new InvalidOperationException("Composition swap chain has no frame-latency handle.");
        const uint waitObject0 = 0;
        var result = WaitForSingleObject(_frameLatencyWaitableObject, 5000);
        if (result != waitObject0)
            throw new TimeoutException(
                $"Composition frame-latency wait failed or timed out (0x{result:X8}).");
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
        _composition?.Dispose();
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

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(nint handle, uint milliseconds);
}

internal sealed class DirectCompositionVisualBridge : IDisposable
{
    private IDCompositionDevice? _device;
    private IDCompositionTarget? _target;
    private IDCompositionVisual? _visual;
    private long _latestEpoch;
    private long _provisionalCommitCount;
    private long _exactCommitCount;
    private long _geometryAdmissionCount;
    private long _geometryAdmissionRejectCount;
    private long _geometryAdmittedEpoch;
    private long _ownedCommitCount;
    private long _ownedCommittedEpoch;
    private int _ownedOffsetX;
    private int _ownedOffsetY;
    private int _ownedContentWidth;
    private int _ownedContentHeight;
    private bool _provisionalActive;
    private bool _disposed;

    internal long ProvisionalCommitCount =>
        Volatile.Read(ref _provisionalCommitCount);

    internal long ExactCommitCount =>
        Volatile.Read(ref _exactCommitCount);

    internal long OwnedCommitCount =>
        Volatile.Read(ref _ownedCommitCount);

    internal long OwnedCommittedEpoch =>
        Volatile.Read(ref _ownedCommittedEpoch);

    internal long GeometryAdmissionCount =>
        Volatile.Read(ref _geometryAdmissionCount);

    internal long GeometryAdmissionRejectCount =>
        Volatile.Read(ref _geometryAdmissionRejectCount);

    internal long LatestEpoch => Volatile.Read(ref _latestEpoch);

    internal DirectCompositionVisualBridge(
        nint window,
        IDXGISwapChain3 swapChain,
        int initialWidth,
        int initialHeight)
    {
        try
        {
            _device = DComp.DCompositionCreateDevice<IDCompositionDevice>(null!);
            _device.CreateTargetForHwnd(window, true, out _target).CheckError();
            _device.CreateVisual(out _visual).CheckError();
            _visual.SetContent(swapChain).CheckError();
            SetVisualState(0, 0, 0, initialWidth, initialHeight);
            _target.SetRoot(_visual).CheckError();
            _device.Commit().CheckError();
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    internal bool IsLatest(long epoch) => epoch == Volatile.Read(ref _latestEpoch);

    internal bool IsGeometryAdmitted(long epoch) =>
        epoch == Volatile.Read(ref _latestEpoch) &&
        epoch == Volatile.Read(ref _geometryAdmittedEpoch);

    internal void AdmitGeometry(long epoch)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (epoch != Volatile.Read(ref _latestEpoch))
        {
            Interlocked.Increment(ref _geometryAdmissionRejectCount);
            return;
        }
        Volatile.Write(ref _geometryAdmittedEpoch, epoch);
        Interlocked.Increment(ref _geometryAdmissionCount);
    }

    internal void ApplyProvisional(
        long epoch,
        int offsetX,
        int offsetY,
        int previousContentWidth,
        int previousContentHeight,
        int targetContentWidth,
        int targetContentHeight)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Volatile.Write(ref _latestEpoch, epoch);

        // Clip is expressed in bitmap coordinates and then follows the visual
        // offset. Intersect the retained source with the new client bounds so
        // a left/top shrink crops while a left/top expansion right-anchors it.
        var clipLeft = Math.Clamp(-offsetX, 0, previousContentWidth);
        var clipTop = Math.Clamp(-offsetY, 0, previousContentHeight);
        var clipRight = Math.Clamp(targetContentWidth - offsetX, clipLeft, previousContentWidth);
        var clipBottom = Math.Clamp(targetContentHeight - offsetY, clipTop, previousContentHeight);
        SetVisualState(offsetX, offsetY, clipLeft, clipTop, clipRight, clipBottom);
        _device!.Commit().CheckError();
        _provisionalActive = true;
        Interlocked.Increment(ref _provisionalCommitCount);
    }

    internal void StageOwned(
        long epoch,
        int offsetX,
        int offsetY,
        int contentWidth,
        int contentHeight)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _ownedOffsetX = offsetX;
        _ownedOffsetY = offsetY;
        _ownedContentWidth = contentWidth;
        _ownedContentHeight = contentHeight;
        Volatile.Write(ref _latestEpoch, epoch);
    }

    internal void CommitOwned(long epoch)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (epoch != Volatile.Read(ref _latestEpoch)) return;
        SetVisualState(
            _ownedOffsetX,
            _ownedOffsetY,
            0,
            0,
            _ownedContentWidth,
            _ownedContentHeight);
        _device!.Commit().CheckError();
        Volatile.Write(ref _ownedCommittedEpoch, epoch);
        Interlocked.Increment(ref _ownedCommitCount);
    }

    internal void CommitExact(long epoch, int contentWidth, int contentHeight)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (epoch != Volatile.Read(ref _latestEpoch)) return;
        if (!_provisionalActive) return;
        SetVisualState(0, 0, 0, contentWidth, contentHeight);
        _device!.Commit().CheckError();
        _provisionalActive = false;
        Interlocked.Increment(ref _exactCommitCount);
    }

    private void SetVisualState(
        float offsetX,
        float offsetY,
        float clipLeft,
        float clipTop,
        float clipRight,
        float clipBottom)
    {
        _visual!.SetOffsetX(offsetX).CheckError();
        _visual.SetOffsetY(offsetY).CheckError();
        _visual.SetClip(new RawRectF(clipLeft, clipTop, clipRight, clipBottom)).CheckError();
    }

    private void SetVisualState(
        float offsetX,
        float offsetY,
        float clipLeft,
        float clipRight,
        float clipBottom) =>
        SetVisualState(offsetX, offsetY, clipLeft, 0, clipRight, clipBottom);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_target is not null) _target.SetRoot(null).CheckError();
        if (_device is not null) _device.Commit().CheckError();
        _visual?.Dispose();
        _visual = null;
        _target?.Dispose();
        _target = null;
        _device?.Dispose();
        _device = null;
    }
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
