using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.Win32;
using SkiaSharp;
using Windows.UI.Composition;
using Windows.UI.Composition.Desktop;

namespace Doroti.Validation.WindowsDwmRedirectionAlphaSpike;

internal sealed class SpikeApplication : IDisposable
{
    private const uint WsOverlappedWindow = 0x00CF0000;
    private const uint WsChild = 0x40000000;
    private const uint WsVisible = 0x10000000;
    private const uint CsOwnDc = 0x0020;
    private const int SwShow = 5;
    private const uint SwpNoZOrder = 0x0004;
    private const uint SwpNoActivate = 0x0010;
    private const uint PmRemove = 0x0001;
    private const uint WmDestroy = 0x0002;
    private const uint WmSize = 0x0005;
    private const uint WmActivate = 0x0006;
    private const uint WmSetFocus = 0x0007;
    private const uint WmPaint = 0x000F;
    private const uint WmClose = 0x0010;
    private const uint WmEraseBackground = 0x0014;
    private const uint WmSettingChange = 0x001A;
    private const uint WmThemeChanged = 0x031A;
    private const uint WmSetCursor = 0x0020;
    private const uint WmKeyDown = 0x0100;
    private const uint WmMouseMove = 0x0200;
    private const uint WmLeftButtonDown = 0x0201;
    private const uint WmNcHitTest = 0x0084;
    private const uint WmQuit = 0x0012;
    private const int DwmwaUseHostBackdropBrush = 17;
    private const int DwmwaSystemBackdropType = 38;
    private const int DwmwaRedirectionBitmapAlpha = 39;
    private const int DwmSystemBackdropTransientWindow = 3;
    private const int SmRemoteSession = 0x1000;
    private const uint SpiGetHighContrast = 0x0042;
    private const uint HcfHighContrastOn = 0x00000001;

    private static readonly string TopClass = $"Doroti.AcrylicSpike.Top.{Environment.ProcessId}";
    internal const string ChildClassPrefix = "Doroti.AcrylicSpike.Child.";
    private static readonly string ChildClass = $"{ChildClassPrefix}{Environment.ProcessId}";
    private static readonly Native.WndProc RootedWindowProcedure = StaticWindowProcedure;
    private static SpikeApplication? _current;

    private readonly SpikeOptions _options;
    private readonly Stopwatch _runtime = Stopwatch.StartNew();
    private readonly List<FrameTrace> _frames = [];
    private readonly List<string> _stateTransitions = [];
    private readonly CompositionDispatcher _compositionDispatcher;
    private readonly WindowsManagedAngleEglPresenter _presenter;
    private readonly DwmAttributeLedger _dwm = new();
    private nint _topWindow;
    private nint _childWindow;
    private Compositor? _compositor;
    private ContainerVisual? _root;
    private DesktopWindowTarget? _desktopTarget;
    private DesktopAcrylicController? _controller;
    private SystemBackdropConfiguration? _configuration;
    private BackdropUpdateQueue? _updates;
    private bool _inputActive = true;
    private bool _quit;
    private bool _disposed;
    private ulong _generation;
    private int _exactMismatchCount;
    private int _renderFailureCount;
    private int _controllerCreateCount;
    private int _targetCreateCount;
    private int _rootCreateCount;
    private int _setTargetCount;
    private bool _setTargetResult;
    private int _focusMessageCount;
    private int _pointerMessageCount;
    private int _keyboardMessageCount;
    private int _cursorMessageCount;
    private int _hitTestResult;

    internal SpikeApplication(SpikeOptions options)
    {
        if (_current is not null) throw new InvalidOperationException("Only one spike window may run per process.");
        _current = this;
        _options = options;
        if (!Native.SetProcessDpiAwarenessContext(new nint(-4)) && Marshal.GetLastWin32Error() != 5)
            throw new InvalidOperationException($"SetProcessDpiAwarenessContext failed: {Marshal.GetLastWin32Error()}.");
        Native.RegisterWindowClass(TopClass, RootedWindowProcedure,
            classStyle: _options.TopDirect ? CsOwnDc : 0);
        Native.RegisterWindowClass(ChildClass, RootedWindowProcedure, classStyle: CsOwnDc);
        _compositionDispatcher = new CompositionDispatcher();
        _presenter = new WindowsManagedAngleEglPresenter(enableDiagnostics: true);
        CreateWindows();
        ConfigureBackdrop();
        Native.ShowWindow(_topWindow, SwShow);
        Native.UpdateWindow(_topWindow);
        RenderExact("initial-show");
        WriteReadyFile();
    }

    internal SpikeReport Run()
    {
        if (_options.Automated)
        {
            RunAutomatedResizeMatrix();
            RunAutomatedInputProbe();
            if (_options.Arm == SpikeArm.Controller) RunControllerContract();
        }

        var deadline = _options.DurationMilliseconds <= 0
            ? DateTime.MaxValue
            : DateTime.UtcNow.AddMilliseconds(_options.DurationMilliseconds);
        while (!_quit && DateTime.UtcNow < deadline)
        {
            PumpMessages();
            if (_options.RuntimeChurn && _options.Arm == SpikeArm.Controller)
                RunRuntimeChurnTick();
            Thread.Sleep(1);
        }

        PumpMessages();
        return CreateReport();
    }

    private void CreateWindows()
    {
        _topWindow = Native.CreateWindow(
            0, TopClass,
            $"Doroti Acrylic {(_options.TopDirect ? "P0.5" : "A1")} - {_options.Arm}",
            WsOverlappedWindow | WsVisible,
            160, 120, 900, 620, 0);
        if (_options.TopDirect) return;
        _childWindow = Native.CreateWindow(
            0, ChildClass, string.Empty, WsChild | WsVisible,
            0, 0, 1, 1, _topWindow);
        ResizeChildToClient();
    }

    private void ConfigureBackdrop()
    {
        ApplyRedirectionAlpha();
        switch (_options.Arm)
        {
            case SpikeArm.Opaque:
                break;
            case SpikeArm.Dwm:
                _dwm.SetInt(_topWindow, DwmwaSystemBackdropType, DwmSystemBackdropTransientWindow,
                    "top.systemBackdrop=transientWindow");
                break;
            case SpikeArm.Controller:
                ConfigureController();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ApplyRedirectionAlpha()
    {
        if (_options.Alpha is AlphaTarget.Top or AlphaTarget.Both)
            _dwm.SetBool(_topWindow, DwmwaRedirectionBitmapAlpha, true, "top.redirectionBitmapAlpha");
        if (_options.Alpha is AlphaTarget.Child or AlphaTarget.Both)
            _dwm.SetBool(_childWindow, DwmwaRedirectionBitmapAlpha, true, "child.redirectionBitmapAlpha");
    }

    private void ConfigureController()
    {
        _dwm.SetBool(_topWindow, DwmwaUseHostBackdropBrush, true, "top.useHostBackdropBrush");
        _compositionDispatcher.Invoke(() =>
        {
            _compositor = new Compositor();
            _root = _compositor.CreateContainerVisual();
            _rootCreateCount++;
            _desktopTarget = DesktopCompositionInterop.CreateDesktopWindowTarget(_compositor, _topWindow);
            _targetCreateCount++;
            _desktopTarget.Root = _root;
            _controller = new DesktopAcrylicController();
            _controllerCreateCount++;
            _controller.StateChanged += (_, _) => _stateTransitions.Add(
                $"{Stopwatch.GetTimestamp()}:{_controller.State}");
            _configuration = new SystemBackdropConfiguration
            {
                IsInputActive = _inputActive,
                Theme = ResolveTheme(),
            };
            ApplySnapshot(_options.InitialSnapshot, "initial");
            _setTargetCount++;
            _setTargetResult = _controller.SetTarget(
                Win32Interop.GetWindowIdFromWindow(_topWindow), _desktopTarget);
        });
        _updates = new BackdropUpdateQueue(_compositionDispatcher.Enqueue, ApplySnapshot);
        if (!_setTargetResult)
            throw new InvalidOperationException("DesktopAcrylicController.SetTarget returned false.");
    }

    private AppliedBackdrop ApplySnapshot(AcrylicSnapshot snapshot, string origin)
    {
        var controller = _controller ?? throw new InvalidOperationException("Controller is unavailable.");
        var configuration = _configuration ?? throw new InvalidOperationException("Configuration is unavailable.");
        snapshot.Validate();
        controller.ResetProperties();
        controller.Kind = snapshot.Kind switch
        {
            AcrylicKind.SystemDefault => DesktopAcrylicKind.Default,
            AcrylicKind.Base => DesktopAcrylicKind.Base,
            AcrylicKind.Thin => DesktopAcrylicKind.Thin,
            _ => throw new ArgumentOutOfRangeException(nameof(snapshot)),
        };
        var appearance = ResolveTheme() == SystemBackdropTheme.Dark ? snapshot.Dark : snapshot.Light;
        if (appearance is not null)
        {
            controller.FallbackColor = Windows.UI.Color.FromArgb(255, 30, 32, 40);
            if (appearance.TintRgb is uint rgb)
            {
                controller.TintColor = Windows.UI.Color.FromArgb(
                    255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
            }
            if (appearance.TintOpacity is double tintOpacity)
                controller.TintOpacity = (float)tintOpacity;
            if (appearance.LuminosityOpacity is double luminosityOpacity)
                controller.LuminosityOpacity = (float)luminosityOpacity;
        }
        configuration.Theme = ResolveTheme();
        configuration.IsInputActive = _inputActive;
        controller.SetSystemBackdropConfiguration(configuration);
        return new AppliedBackdrop(
            snapshot,
            controller.Kind.ToString(),
            ColorToRgb(controller.TintColor),
            controller.TintOpacity,
            controller.LuminosityOpacity,
            configuration.Theme.ToString(),
            controller.State.ToString(),
            origin,
            Stopwatch.GetTimestamp());
    }

    private void RunAutomatedResizeMatrix()
    {
        var seedSizes = new (int Width, int Height)[]
        {
            (700, 500), (840, 600), (500, 300), (901, 541),
            (640, 720), (1024, 640), (777, 433), (900, 620),
        };
        var resizeCount = _options.TopDirect ? _options.ResizeCount : seedSizes.Length;
        for (var index = 0; index < resizeCount; index++)
        {
            var seed = seedSizes[index % seedSizes.Length];
            var phase = (index / seedSizes.Length) % 4;
            var delta = index % 37;
            var size = phase switch
            {
                0 => (Width: seed.Width + delta, Height: seed.Height + delta / 2),
                1 => (Width: seed.Width - delta, Height: seed.Height - delta / 2),
                2 => (Width: seed.Width + delta, Height: seed.Height - delta / 2),
                _ => (Width: seed.Width - delta, Height: seed.Height + delta / 2),
            };
            Native.SetWindowPos(_topWindow, 0, 0, 0, size.Width, size.Height,
                SwpNoZOrder | SwpNoActivate);
            PumpMessages();
        }
    }

    private void RunAutomatedInputProbe()
    {
        Native.SendMessage(_topWindow, WmSetFocus, 0, 0);
        Native.SendMessage(_topWindow, WmMouseMove, 0, 0);
        Native.SendMessage(_topWindow, WmLeftButtonDown, 0, 0);
        Native.SendMessage(_topWindow, WmKeyDown, 0x41, 0);
        Native.SendMessage(_topWindow, WmSetCursor, (nuint)_topWindow, 1);
        _hitTestResult = checked((int)Native.SendMessage(_topWindow, WmNcHitTest, 0, 0));
    }

    private DateTime _nextRuntimeChurn = DateTime.MinValue;
    private int _runtimeChurnIndex;

    private void RunRuntimeChurnTick()
    {
        if (DateTime.UtcNow < _nextRuntimeChurn || _updates is null) return;
        var profiles = new[]
        {
            AcrylicSnapshot.SystemDefault,
            AcrylicSnapshot.Base,
            AcrylicSnapshot.Thin,
            AcrylicSnapshot.Custom,
        };
        _ = _updates.Request(profiles[_runtimeChurnIndex++ % profiles.Length], "visible-churn");
        _nextRuntimeChurn = DateTime.UtcNow.AddMilliseconds(80);
    }

    private void RunControllerContract()
    {
        var queue = _updates ?? throw new InvalidOperationException("Update queue is unavailable.");
        var profiles = new[]
        {
            AcrylicSnapshot.SystemDefault,
            AcrylicSnapshot.Base,
            AcrylicSnapshot.Thin,
            AcrylicSnapshot.Custom,
            AcrylicSnapshot.SystemDefault,
        };
        foreach (var profile in profiles)
        {
            var task = queue.Request(profile, "sequential");
            PumpUntil(task);
        }
        RunBurst(queue, 100);
        RunBurst(queue, 500);
        foreach (var invalid in AcrylicSnapshot.InvalidFixtures)
        {
            try
            {
                queue.Request(invalid, "invalid");
                throw new InvalidOperationException("An invalid Acrylic snapshot was accepted.");
            }
            catch (ArgumentOutOfRangeException)
            {
                queue.InvalidRejectedCount++;
            }
        }
    }

    private void RunBurst(BackdropUpdateQueue queue, int count)
    {
        var tasks = new Task<UpdateTerminal>[count];
        for (var index = 0; index < count; index++)
        {
            var baseProfile = (index % 3) switch
            {
                0 => AcrylicSnapshot.SystemDefault,
                1 => AcrylicSnapshot.Base,
                _ => AcrylicSnapshot.Thin,
            };
            tasks[index] = queue.Request(baseProfile, $"burst-{count}");
        }
        PumpUntil(Task.WhenAll(tasks));
    }

    private void PumpUntil(Task task)
    {
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!task.IsCompleted && DateTime.UtcNow < deadline)
        {
            PumpMessages();
            Thread.Sleep(1);
        }
        if (!task.IsCompleted) throw new TimeoutException("DispatcherQueue Acrylic update timed out.");
        task.GetAwaiter().GetResult();
    }

    private void PumpMessages()
    {
        while (Native.PeekMessage(out var message, 0, 0, 0, PmRemove))
        {
            if (message.Id == WmQuit)
            {
                _quit = true;
                return;
            }
            Native.TranslateMessage(in message);
            Native.DispatchMessage(in message);
        }
    }

    private nint WindowProcedure(nint window, uint message, nuint wParam, nint lParam)
    {
        if (window == _topWindow || (_topWindow == 0 && Native.GetParent(window) == 0))
        {
            switch (message)
            {
                case WmSize:
                    if (_options.TopDirect) RenderExact("top-wm-size");
                    else if (_childWindow != 0) ResizeChildToClient();
                    return 0;
                case WmPaint when _options.TopDirect:
                    Native.ValidateRect(window, 0);
                    RenderExact("top-wm-paint");
                    return 0;
                case WmEraseBackground when _options.TopDirect:
                    return 1;
                case WmSetFocus:
                    _focusMessageCount++;
                    break;
                case WmMouseMove:
                case WmLeftButtonDown:
                    _pointerMessageCount++;
                    break;
                case WmKeyDown:
                    _keyboardMessageCount++;
                    break;
                case WmSetCursor:
                    _cursorMessageCount++;
                    break;
                case WmActivate:
                    _inputActive = (wParam & 0xffff) != 0;
                    if (_updates?.Requested is { } activeSnapshot)
                        _ = _updates.Request(activeSnapshot, "activation");
                    return 0;
                case WmThemeChanged:
                case WmSettingChange:
                    ReapplyForThemeChange();
                    break;
                case WmClose:
                    Native.DestroyWindow(window);
                    return 0;
                case WmDestroy:
                    Native.PostQuitMessage(0);
                    return 0;
            }
        }
        else if (window == _childWindow || Native.GetParent(window) == _topWindow)
        {
            switch (message)
            {
                case WmSize:
                    RenderExact("child-wm-size");
                    return 0;
                case WmPaint:
                    Native.ValidateRect(window, 0);
                    RenderExact("child-wm-paint");
                    return 0;
                case WmEraseBackground:
                    return 1;
            }
        }
        return Native.DefWindowProc(window, message, wParam, lParam);
    }

    private void ReapplyForThemeChange()
    {
        if (_controller is null || _updates is null) return;
        _ = _updates.Request(_updates.Requested ?? _options.InitialSnapshot, "theme");
    }

    private void ResizeChildToClient()
    {
        if (_topWindow == 0 || _childWindow == 0 || !Native.GetClientRect(_topWindow, out var rect)) return;
        var width = Math.Max(0, rect.Right - rect.Left);
        var height = Math.Max(0, rect.Bottom - rect.Top);
        Native.SetWindowPos(_childWindow, 0, 0, 0, width, height, SwpNoZOrder | SwpNoActivate);
    }

    private void RenderExact(string reason)
    {
        var renderWindow = _options.TopDirect ? _topWindow : _childWindow;
        if (renderWindow == 0 || !Native.GetClientRect(renderWindow, out var rect)) return;
        var width = rect.Right - rect.Left;
        var height = rect.Bottom - rect.Top;
        if (width <= 0 || height <= 0) return;
        var generation = ++_generation;
        var started = Stopwatch.GetTimestamp();
        try
        {
            _presenter.EnsureTarget(renderWindow, width, height);
            _presenter.RenderAndPresent(
                surface =>
                {
                    if (surface.Canvas.DeviceClipBounds.Width != width ||
                        surface.Canvas.DeviceClipBounds.Height != height)
                        _exactMismatchCount++;
                    DrawAlphaGrid(surface.Canvas, width, height, generation,
                        Native.GetDpiForWindow(renderWindow));
                    return true;
                },
                static value => value);
            _frames.Add(new FrameTrace(
                generation, width, height, reason, started, Stopwatch.GetTimestamp(),
                _presenter.LastPresentSucceeded ? "presented" : "failed"));
        }
        catch (Exception exception)
        {
            _renderFailureCount++;
            _frames.Add(new FrameTrace(
                generation, width, height, reason, started, Stopwatch.GetTimestamp(),
                $"failed:{exception.GetType().Name}:{exception.Message}"));
            throw;
        }
    }

    private static void DrawAlphaGrid(
        SKCanvas canvas, int width, int height, ulong generation, uint dpi)
    {
        canvas.Clear(SKColors.Transparent);
        using var paint = new SKPaint { IsAntialias = false, BlendMode = SKBlendMode.Src };
        var bandWidth = Math.Max(1, width / 5);
        var colors = new[]
        {
            new SKColor(255, 255, 255, 0),
            new SKColor(44, 180, 255, 64),
            new SKColor(255, 86, 160, 128),
            new SKColor(126, 242, 145, 204),
            new SKColor(26, 32, 44, 255),
        };
        for (var index = 0; index < colors.Length; index++)
        {
            paint.Color = colors[index];
            var left = index * bandWidth;
            var right = index == colors.Length - 1 ? width : Math.Min(width, left + bandWidth);
            canvas.DrawRect(left, 0, right - left, height, paint);
        }
        paint.Color = new SKColor(255, 210, 0, 255);
        canvas.DrawRect(0, 0, Math.Max(6, width / 50), Math.Max(24, height / 5), paint);
        paint.Color = new SKColor(0, 220, 255, 255);
        canvas.DrawRect(width - Math.Max(12, width / 25), height - Math.Max(8, height / 40),
            Math.Max(12, width / 25), Math.Max(8, height / 40), paint);
        paint.Color = new SKColor((byte)generation, (byte)(generation >> 8), 0, 255);
        canvas.DrawRect(width / 2f - 8, height / 2f - 8, 16, 16, paint);

        var scale = Math.Max(1f, dpi / 96f);
        var bitSize = Math.Max(4, (int)MathF.Round(7 * scale));
        var bitGap = Math.Max(1, (int)MathF.Round(scale));
        const int bitCount = 12;
        var stripWidth = bitCount * bitSize + (bitCount - 1) * bitGap;
        var startX = width - stripWidth - Math.Max(4, (int)MathF.Round(4 * scale));
        var startY = Math.Max(1, (int)MathF.Round(5 * scale));
        var binary = checked((int)(generation & 0xFFF));
        var gray = binary ^ (binary >> 1);
        for (var bit = 0; bit < bitCount; bit++)
        {
            paint.Color = (gray & (1 << bit)) != 0 ? SKColors.White : SKColors.Black;
            canvas.DrawRect(startX + bit * (bitSize + bitGap), startY, bitSize, bitSize, paint);
        }
    }

    private SpikeReport CreateReport()
    {
        Native.DwmIsCompositionEnabled(out var compositionEnabled);
        var highContrast = Native.GetHighContrast();
        var systemBackdropValue = 0;
        var systemBackdropGetHresult = _topWindow == 0
            ? unchecked((int)0x80070006)
            : Native.DwmGetWindowAttribute(
                _topWindow, DwmwaSystemBackdropType, ref systemBackdropValue, sizeof(int));
        var updates = _updates?.CreateReport();
        var pass = _renderFailureCount == 0 && _exactMismatchCount == 0 &&
                   _presenter.PresentCount > 0 &&
                   _presenter.PresentCount == _presenter.GpuSubmitCount &&
                   _presenter.PresentCount == _presenter.GpuCopyCount &&
                   _presenter.OperationalDebugErrorCount == 0 &&
                   _dwm.Failures.Count == 0 &&
                    (_options.Arm != SpikeArm.Controller ||
                     (_setTargetResult && _setTargetCount == 1 && _controllerCreateCount == 1 &&
                      _targetCreateCount == 1 && _rootCreateCount == 1 &&
                      updates is { DuplicateTerminalCount: 0, MissingTerminalCount: 0,
                                   MaxPendingDepth: <= 1 } &&
                      (!_options.Automated || updates.InvalidRejectedCount == 7)));
        return new SpikeReport(
            _options.TopDirect
                ? "doroti.windows-acrylic-top-hwnd-p05/v1"
                : "doroti.windows-dwm-redirection-alpha-a1/v1",
            pass ? "PASS" : "FAIL",
            _options.Arm.ToString().ToLowerInvariant(),
            _options.Alpha.ToString().ToLowerInvariant(),
            _options.TopDirect ? "top-hwnd-direct" : "top-child",
            Environment.OSVersion.VersionString,
            ReadRegistryString(Registry.LocalMachine,
                @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuildNumber"),
            Native.GetDpiForWindow(_topWindow),
            RuntimeInformation.ProcessArchitecture.ToString(),
            compositionEnabled,
            ReadTransparencyPolicy(),
            highContrast,
            Native.GetSystemMetrics(SmRemoteSession) != 0,
            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_DWM_FLUSH") ?? "notSet",
            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_EGL_SWAP_INTERVAL") ?? "notSet",
            _topWindow == 0 ? null : $"0x{_topWindow:X}",
            _childWindow == 0 ? null : $"0x{_childWindow:X}",
            ChildClass,
            _dwm.Calls,
            _dwm.Failures,
            systemBackdropGetHresult,
            systemBackdropValue,
            _presenter.BackendName,
            _presenter.AdapterDescription,
            _presenter.Width,
            _presenter.Height,
            _presenter.DeviceGeneration,
            _presenter.ResizeBuffersCount,
            _presenter.PresentCount,
            _presenter.GpuSubmitCount,
            _presenter.GpuCopyCount,
            _presenter.InitializationDebugErrorCount,
            _presenter.OperationalDebugErrorCount,
            _exactMismatchCount,
            _renderFailureCount,
            _frames.Count,
            Native.IsWindowVisible(_topWindow) ? 1 +
                (_childWindow != 0 && Native.IsWindowVisible(_childWindow) ? 1 : 0) : 0,
            _childWindow != 0 && Native.IsWindowVisible(_childWindow) ? 1 : 0,
            _childWindow == 0 ? 0 : 1,
            1,
            _focusMessageCount,
            _pointerMessageCount,
            _keyboardMessageCount,
            _cursorMessageCount,
            _hitTestResult,
            _controllerCreateCount,
            _targetCreateCount,
            _rootCreateCount,
            _setTargetCount,
            _setTargetResult,
            ReadControllerValue(static controller => controller.Kind.ToString()),
            ReadControllerValue(static controller => controller.State.ToString()),
            _configuration is null ? null : _compositionDispatcher.Invoke(() => _configuration.Theme.ToString()),
            _stateTransitions,
            updates,
            _frames,
            "notVerified",
            "notVerified",
            "Automated API, HWND ownership, fixed-size ANGLE, DWM HRESULT, option ordering, frame marker, input-message, and terminal evidence only. Windows Graphics Capture and physical border-drag quality are separate gates.");
    }

    private void WriteReadyFile()
    {
        if (_options.ReadyPath is null) return;
        Directory.CreateDirectory(Path.GetDirectoryName(_options.ReadyPath)!);
        File.WriteAllText(_options.ReadyPath, JsonSerializer.Serialize(new
        {
            schema = "doroti.windows-dwm-redirection-alpha-ready/v1",
            processId = Environment.ProcessId,
            topHwnd = $"0x{_topWindow:X}",
            childHwnd = _childWindow == 0 ? null : $"0x{_childWindow:X}",
            childClass = ChildClass,
            arm = _options.Arm.ToString().ToLowerInvariant(),
            alpha = _options.Alpha.ToString().ToLowerInvariant(),
        }, JsonOptions));
    }

    private SystemBackdropTheme ResolveTheme()
    {
        if (_options.Theme == RequestedTheme.Light) return SystemBackdropTheme.Light;
        if (_options.Theme == RequestedTheme.Dark) return SystemBackdropTheme.Dark;
        using var key = Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        return key?.GetValue("AppsUseLightTheme") is int value && value == 0
            ? SystemBackdropTheme.Dark
            : SystemBackdropTheme.Light;
    }

    private static string ReadTransparencyPolicy()
    {
        using var key = Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        return key?.GetValue("EnableTransparency") is int value
            ? value == 0 ? "disabled" : "enabled"
            : "unknown";
    }

    private static string ReadRegistryString(RegistryKey root, string path, string name)
    {
        using var key = root.OpenSubKey(path);
        return Convert.ToString(key?.GetValue(name)) ?? "unknown";
    }

    private static uint ColorToRgb(Windows.UI.Color color) =>
        ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;

    private string? ReadControllerValue(Func<DesktopAcrylicController, string> read) =>
        _controller is null ? null : _compositionDispatcher.Invoke(() => read(_controller));

    private static nint StaticWindowProcedure(nint window, uint message, nuint wParam, nint lParam) =>
        _current?.WindowProcedure(window, message, wParam, lParam) ??
        Native.DefWindowProc(window, message, wParam, lParam);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _updates?.Close();
        _compositionDispatcher.Invoke(() =>
        {
            _controller?.Dispose();
            _controller = null;
            _desktopTarget?.Dispose();
            _desktopTarget = null;
            _root?.Dispose();
            _root = null;
            _compositor?.Dispose();
            _compositor = null;
        });
        _presenter.Dispose();
        if (_childWindow != 0) Native.DestroyWindow(_childWindow);
        if (_topWindow != 0) Native.DestroyWindow(_topWindow);
        _childWindow = _topWindow = 0;
        _compositionDispatcher.Dispose();
        _current = null;
    }

    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        var options = SpikeOptions.Parse(args);
        SpikeReport? report = null;
        try
        {
            using (var app = new SpikeApplication(options)) report = app.Run();
            WriteReport(options.ReportPath, report);
            Console.WriteLine(
                $"A1 status={report.Status} arm={report.Arm} alpha={report.Alpha} " +
                $"present={report.PresentCount} setTarget={report.SetTargetResult} " +
                $"visible={report.VisibleStatus} physical={report.PhysicalStatus}");
            Console.WriteLine($"report={options.ReportPath}");
            return report.Status == "PASS" ? 0 : 2;
        }
        catch (Exception exception)
        {
            WriteReport(options.ReportPath, new
            {
                schema = "doroti.windows-dwm-redirection-alpha-a1/v1",
                status = "FAIL",
                arm = options.Arm.ToString().ToLowerInvariant(),
                alpha = options.Alpha.ToString().ToLowerInvariant(),
                exception = exception.ToString(),
                visibleStatus = "notVerified",
                physicalStatus = "notVerified",
            });
            Console.Error.WriteLine(exception);
            Console.Error.WriteLine($"report={options.ReportPath}");
            return 1;
        }
    }

    private static void WriteReport(string path, object report)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(report, SpikeApplication.JsonOptions));
    }
}

internal sealed class BackdropUpdateQueue(
    Func<Action, bool> enqueue,
    Func<AcrylicSnapshot, string, AppliedBackdrop> apply)
{
    private readonly object _gate = new();
    private readonly List<UpdateTerminal> _terminals = [];
    private PendingUpdate? _pending;
    private bool _scheduled;
    private bool _closed;
    private ulong _nextRevision;
    private int _acceptedCount;
    private int _appliedCount;
    private int _supersededCount;
    private int _closedCount;
    private int _maxPendingDepth;
    internal int InvalidRejectedCount { get; set; }
    internal AcrylicSnapshot? Requested { get; private set; }

    internal Task<UpdateTerminal> Request(AcrylicSnapshot snapshot, string origin)
    {
        snapshot.Validate();
        var completion = new TaskCompletionSource<UpdateTerminal>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        PendingUpdate? superseded = null;
        lock (_gate)
        {
            if (_closed)
            {
                completion.SetResult(CreateTerminal(
                    ++_nextRevision, "closed", snapshot, null, origin, "queue-closed"));
                _closedCount++;
                return completion.Task;
            }
            var request = new PendingUpdate(++_nextRevision, snapshot, origin, completion);
            _acceptedCount++;
            Requested = snapshot;
            superseded = _pending;
            _pending = request;
            _maxPendingDepth = Math.Max(_maxPendingDepth, 1);
            if (!_scheduled)
            {
                _scheduled = true;
                if (!enqueue(Drain))
                {
                    _scheduled = false;
                    _pending = null;
                    completion.SetResult(CreateTerminal(
                        request.Revision, "failed", snapshot, null, origin, "dispatcher-rejected"));
                }
            }
        }
        if (superseded is not null)
        {
            var terminal = CreateTerminal(
                superseded.Revision, "superseded", superseded.Snapshot, null,
                superseded.Origin, "replaced-before-apply");
            AddTerminal(terminal);
            superseded.Completion.SetResult(terminal);
            Interlocked.Increment(ref _supersededCount);
        }
        return completion.Task;
    }

    private void Drain()
    {
        PendingUpdate? current;
        lock (_gate)
        {
            current = _pending;
            _pending = null;
            _scheduled = false;
        }
        if (current is null) return;
        UpdateTerminal terminal;
        try
        {
            var effective = apply(current.Snapshot, current.Origin);
            terminal = CreateTerminal(
                current.Revision, "applied", current.Snapshot, effective,
                current.Origin, "properties-applied");
            Interlocked.Increment(ref _appliedCount);
        }
        catch (Exception exception)
        {
            terminal = CreateTerminal(
                current.Revision, "failed", current.Snapshot, null,
                current.Origin, $"{exception.GetType().Name}:{exception.Message}");
        }
        AddTerminal(terminal);
        current.Completion.SetResult(terminal);
        lock (_gate)
        {
            if (_pending is not null && !_scheduled)
            {
                _scheduled = true;
                if (!enqueue(Drain)) _scheduled = false;
            }
        }
    }

    internal void Close()
    {
        PendingUpdate? pending;
        lock (_gate)
        {
            if (_closed) return;
            _closed = true;
            pending = _pending;
            _pending = null;
        }
        if (pending is null) return;
        var terminal = CreateTerminal(
            pending.Revision, "closed", pending.Snapshot, null, pending.Origin, "queue-closed");
        AddTerminal(terminal);
        pending.Completion.SetResult(terminal);
        _closedCount++;
    }

    private UpdateTerminal CreateTerminal(
        ulong revision,
        string status,
        AcrylicSnapshot requested,
        AppliedBackdrop? effective,
        string origin,
        string reason) => new(
            revision, status, requested, effective, origin, reason, Stopwatch.GetTimestamp());

    private void AddTerminal(UpdateTerminal terminal)
    {
        lock (_gate) _terminals.Add(terminal);
    }

    internal BackdropQueueReport CreateReport()
    {
        UpdateTerminal[] terminals;
        int pending;
        lock (_gate)
        {
            terminals = [.. _terminals];
            pending = _pending is null ? 0 : 1;
        }
        var groups = terminals.GroupBy(terminal => terminal.Revision).ToArray();
        return new BackdropQueueReport(
            _acceptedCount,
            _appliedCount,
            _supersededCount,
            _closedCount,
            InvalidRejectedCount,
            _maxPendingDepth,
            pending,
            groups.Count(group => group.Count() > 1),
            Math.Max(0, _acceptedCount - groups.Count()),
            terminals.Length == 0 ? 0 : terminals.Max(terminal => terminal.Revision),
            terminals.LastOrDefault(),
            terminals);
    }

    private sealed record PendingUpdate(
        ulong Revision,
        AcrylicSnapshot Snapshot,
        string Origin,
        TaskCompletionSource<UpdateTerminal> Completion);
}

internal sealed class CompositionDispatcher : IDisposable
{
    private readonly Windows.System.DispatcherQueueController _controller;
    private int _threadId;
    private bool _disposed;

    internal CompositionDispatcher()
    {
        _controller = Windows.System.DispatcherQueueController.CreateOnDedicatedThread();
        Invoke(() => _threadId = Environment.CurrentManagedThreadId);
    }

    internal bool Enqueue(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return !_disposed && _controller.DispatcherQueue.TryEnqueue(() => action());
    }

    internal void Invoke(Action action)
    {
        Invoke(() =>
        {
            action();
            return true;
        });
    }

    internal T Invoke<T>(Func<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_threadId != 0 && Environment.CurrentManagedThreadId == _threadId) return action();
        using var completed = new ManualResetEventSlim();
        Exception? failure = null;
        T? result = default;
        if (!_controller.DispatcherQueue.TryEnqueue(() =>
            {
                try { result = action(); }
                catch (Exception exception) { failure = exception; }
                finally { completed.Set(); }
            }))
            throw new InvalidOperationException("The composition DispatcherQueue rejected a callback.");
        if (!completed.Wait(TimeSpan.FromSeconds(10)))
            throw new TimeoutException("The composition DispatcherQueue callback timed out.");
        if (failure is not null)
            throw new InvalidOperationException("The composition DispatcherQueue callback failed.", failure);
        return result!;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _controller.ShutdownQueueAsync().AsTask().GetAwaiter().GetResult();
    }
}

internal static class DesktopCompositionInterop
{
    private static readonly Guid CompositorDesktopInteropIid =
        new("29E691FA-4567-4DCA-B319-D0F207EB6807");

    internal static unsafe DesktopWindowTarget CreateDesktopWindowTarget(
        Compositor compositor,
        nint window)
    {
        using var interop = ((WinRT.IWinRTObject)compositor).NativeObject.As(
            CompositorDesktopInteropIid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var create = (delegate* unmanaged[Stdcall]<nint, nint, int, nint*, int>)vtable[3];
        nint result = 0;
        var hresult = create(thisPointer, window, 0, &result);
        if (hresult < 0) Marshal.ThrowExceptionForHR(hresult);
        try
        {
            return WinRT.MarshalInterface<DesktopWindowTarget>.FromAbi(result);
        }
        finally
        {
            Marshal.Release(result);
        }
    }
}

internal sealed class DwmAttributeLedger
{
    internal List<DwmCall> Calls { get; } = [];
    internal List<DwmCall> Failures { get; } = [];

    internal void SetBool(nint window, int attribute, bool value, string name) =>
        SetInt(window, attribute, value ? 1 : 0, name);

    internal void SetInt(nint window, int attribute, int value, string name)
    {
        var hresult = Native.DwmSetWindowAttribute(window, attribute, ref value, sizeof(int));
        var call = new DwmCall(name, attribute, value, hresult, Stopwatch.GetTimestamp());
        Calls.Add(call);
        if (hresult < 0) Failures.Add(call);
    }
}

internal sealed record AcrylicAppearance(uint? TintRgb, double? TintOpacity, double? LuminosityOpacity)
{
    internal void Validate()
    {
        ValidateOpacity(TintOpacity, nameof(TintOpacity));
        ValidateOpacity(LuminosityOpacity, nameof(LuminosityOpacity));
    }

    private static void ValidateOpacity(double? value, string name)
    {
        if (value is null) return;
        if (!double.IsFinite(value.Value) || value.Value < 0 || value.Value > 1)
            throw new ArgumentOutOfRangeException(name, value, "Opacity must be finite and in [0, 1].");
    }
}

internal sealed record AcrylicSnapshot(
    AcrylicKind Kind,
    AcrylicAppearance? Light,
    AcrylicAppearance? Dark)
{
    internal static AcrylicSnapshot SystemDefault { get; } =
        new(AcrylicKind.SystemDefault, null, null);
    internal static AcrylicSnapshot Base { get; } =
        new(AcrylicKind.Base, null, null);
    internal static AcrylicSnapshot Thin { get; } =
        new(AcrylicKind.Thin, null, null);
    internal static AcrylicSnapshot Custom { get; } = new(
        AcrylicKind.Thin,
        new AcrylicAppearance(0x4A90E2, 0.35, 0.72),
        new AcrylicAppearance(0x311B52, 0.58, 0.45));
    internal static IReadOnlyList<AcrylicSnapshot> InvalidFixtures { get; } =
    [
        new((AcrylicKind)99, null, null),
        new(AcrylicKind.Base, new AcrylicAppearance(null, double.NaN, null), null),
        new(AcrylicKind.Base, new AcrylicAppearance(null, double.PositiveInfinity, null), null),
        new(AcrylicKind.Base, new AcrylicAppearance(null, -0.01, null), null),
        new(AcrylicKind.Base, new AcrylicAppearance(null, 1.01, null), null),
        new(AcrylicKind.Base, new AcrylicAppearance(null, null, -0.01), null),
        new(AcrylicKind.Base, new AcrylicAppearance(null, null, 1.01), null),
    ];

    internal void Validate()
    {
        if (Kind is < AcrylicKind.SystemDefault or > AcrylicKind.Thin)
            throw new ArgumentOutOfRangeException(nameof(Kind), Kind, "Unknown Acrylic kind.");
        Light?.Validate();
        Dark?.Validate();
    }
}

internal sealed record SpikeOptions(
    SpikeArm Arm,
    AlphaTarget Alpha,
    RequestedTheme Theme,
    AcrylicSnapshot InitialSnapshot,
    bool TopDirect,
    bool RuntimeChurn,
    int ResizeCount,
    bool Automated,
    int DurationMilliseconds,
    string ReportPath,
    string? ReadyPath)
{
    internal static SpikeOptions Parse(string[] args)
    {
        string? Value(string name)
        {
            var index = Array.IndexOf(args, name);
            return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
        }
        var arm = (Value("--arm") ?? "opaque").ToLowerInvariant() switch
        {
            "opaque" => SpikeArm.Opaque,
            "dwm" => SpikeArm.Dwm,
            "controller" => SpikeArm.Controller,
            var value => throw new ArgumentException($"Unknown --arm value: {value}"),
        };
        var alpha = (Value("--alpha") ?? (arm == SpikeArm.Opaque ? "off" : "both"))
            .ToLowerInvariant() switch
        {
            "off" => AlphaTarget.Off,
            "top" => AlphaTarget.Top,
            "child" => AlphaTarget.Child,
            "both" => AlphaTarget.Both,
            var value => throw new ArgumentException($"Unknown --alpha value: {value}"),
        };
        var theme = (Value("--theme") ?? "system").ToLowerInvariant() switch
        {
            "system" => RequestedTheme.System,
            "light" => RequestedTheme.Light,
            "dark" => RequestedTheme.Dark,
            var value => throw new ArgumentException($"Unknown --theme value: {value}"),
        };
        var kind = (Value("--kind") ?? "default").ToLowerInvariant() switch
        {
            "default" => AcrylicKind.SystemDefault,
            "base" => AcrylicKind.Base,
            "thin" => AcrylicKind.Thin,
            var value => throw new ArgumentException($"Unknown --kind value: {value}"),
        };
        var custom = args.Contains("--custom", StringComparer.Ordinal);
        var snapshot = custom
            ? AcrylicSnapshot.Custom with { Kind = kind }
            : new AcrylicSnapshot(kind, null, null);
        var duration = int.TryParse(Value("--duration-ms"), out var parsedDuration)
            ? parsedDuration
            : args.Contains("--automated", StringComparer.Ordinal) ? 250 : 30_000;
        if (duration < 0 || duration > 300_000)
            throw new ArgumentOutOfRangeException("--duration-ms");
        var resizeCount = int.TryParse(Value("--resize-count"), out var parsedResizeCount)
            ? parsedResizeCount
            : 500;
        if (resizeCount < 1 || resizeCount > 10_000)
            throw new ArgumentOutOfRangeException("--resize-count");
        var report = Path.GetFullPath(Value("--report") ?? Path.Combine(
            ".doroti", "evidence", $"acrylic-a1-{arm.ToString().ToLowerInvariant()}-{DateTime.Now:yyyyMMdd-HHmmss}.json"));
        var readyValue = Value("--ready");
        return new SpikeOptions(
            arm,
            alpha,
            theme,
            snapshot,
            args.Contains("--top-direct", StringComparer.Ordinal),
            args.Contains("--runtime-churn", StringComparer.Ordinal),
            resizeCount,
            args.Contains("--automated", StringComparer.Ordinal),
            duration,
            report,
            readyValue is null ? null : Path.GetFullPath(readyValue));
    }
}

internal enum SpikeArm { Opaque, Dwm, Controller }
internal enum AlphaTarget { Off, Top, Child, Both }
internal enum RequestedTheme { System, Light, Dark }
internal enum AcrylicKind { SystemDefault, Base, Thin }

internal sealed record DwmCall(string Name, int Attribute, int Value, int HResult, long TimestampQpc);
internal sealed record FrameTrace(
    ulong Generation, int Width, int Height, string Reason, long BeginQpc, long EndQpc, string Terminal);
internal sealed record AppliedBackdrop(
    AcrylicSnapshot Requested, string Kind, uint TintRgb, float TintOpacity,
    float LuminosityOpacity, string Theme, string State, string Origin, long AppliedQpc);
internal sealed record UpdateTerminal(
    ulong Revision, string Status, AcrylicSnapshot Requested, AppliedBackdrop? Effective,
    string Origin, string Reason, long TerminalQpc);
internal sealed record BackdropQueueReport(
    int AcceptedCount,
    int AppliedCount,
    int SupersededCount,
    int ClosedCount,
    int InvalidRejectedCount,
    int MaxPendingDepth,
    int PendingCount,
    int DuplicateTerminalCount,
    int MissingTerminalCount,
    ulong LastTerminalRevision,
    UpdateTerminal? LastTerminal,
    IReadOnlyList<UpdateTerminal> Terminals);
internal sealed record SpikeReport(
    string Schema,
    string Status,
    string Arm,
    string Alpha,
    string Topology,
    string OperatingSystem,
    string WindowsBuild,
    uint Dpi,
    string ProcessArchitecture,
    bool DwmCompositionEnabled,
    string TransparencyPolicy,
    bool HighContrast,
    bool RemoteSession,
    string DwmFlushEnvironment,
    string SwapIntervalEnvironment,
    string? TopHwnd,
    string? ChildHwnd,
    string ChildClass,
    IReadOnlyList<DwmCall> DwmCalls,
    IReadOnlyList<DwmCall> DwmFailures,
    int SystemBackdropGetHresult,
    int SystemBackdropValue,
    string PresenterBackend,
    string AdapterDescription,
    int FinalWidth,
    int FinalHeight,
    ulong DeviceGeneration,
    ulong SurfaceRecreateCount,
    ulong PresentCount,
    ulong GpuSubmitCount,
    ulong GpuCopyCount,
    ulong InitializationGpuErrorCount,
    ulong OperationalGpuErrorCount,
    int ExactMismatchCount,
    int RenderFailureCount,
    int FrameCount,
    int VisibleHwndCount,
    int VisibleChildRenderHwndCount,
    int CreatedChildRenderHwndCount,
    int MaximumRenderQueueDepth,
    int FocusMessageCount,
    int PointerMessageCount,
    int KeyboardMessageCount,
    int CursorMessageCount,
    int HitTestResult,
    int ControllerCreateCount,
    int TargetCreateCount,
    int RootCreateCount,
    int SetTargetCount,
    bool SetTargetResult,
    string? EffectiveKind,
    string? ControllerState,
    string? ResolvedTheme,
    IReadOnlyList<string> ControllerStateTransitions,
    BackdropQueueReport? Updates,
    IReadOnlyList<FrameTrace> Frames,
    string VisibleStatus,
    string PhysicalStatus,
    string ScopeBoundary);

internal static partial class Native
{
    private const uint SpiGetHighContrast = 0x0042;
    private const uint HcfHighContrastOn = 0x00000001;

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate nint WndProc(nint window, uint message, nuint wParam, nint lParam);

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

    [StructLayout(LayoutKind.Sequential)]
    internal struct Message
    {
        internal nint Window;
        internal uint Id;
        internal nuint WParam;
        internal nint LParam;
        internal uint Time;
        internal int PointX;
        internal int PointY;
        internal uint Private;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct HighContrast
    {
        internal uint Size;
        internal uint Flags;
        internal nint DefaultScheme;
    }

    internal static void RegisterWindowClass(string className, WndProc procedure, uint classStyle)
    {
        var windowClass = new WindowClass
        {
            Size = checked((uint)Marshal.SizeOf<WindowClass>()),
            Style = classStyle,
            WindowProcedure = Marshal.GetFunctionPointerForDelegate(procedure),
            Instance = GetModuleHandle(null),
            Cursor = LoadCursor(0, new nint(32512)),
            ClassName = className,
        };
        if (RegisterClassEx(in windowClass) == 0)
            throw new InvalidOperationException($"RegisterClassExW failed: {Marshal.GetLastWin32Error()}.");
    }

    internal static nint CreateWindow(
        uint extendedStyle, string className, string title, uint style,
        int x, int y, int width, int height, nint parent)
    {
        var window = CreateWindowEx(
            extendedStyle, className, title, style, x, y, width, height,
            parent, 0, GetModuleHandle(null), 0);
        if (window == 0)
            throw new InvalidOperationException($"CreateWindowExW failed: {Marshal.GetLastWin32Error()}.");
        return window;
    }

    internal static bool GetHighContrast()
    {
        var value = new HighContrast { Size = checked((uint)Marshal.SizeOf<HighContrast>()) };
        return SystemParametersInfo(SpiGetHighContrast, value.Size, ref value, 0) &&
               (value.Flags & HcfHighContrastOn) != 0;
    }

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetProcessDpiAwarenessContext(nint context);

    [DllImport("user32.dll", EntryPoint = "RegisterClassExW", SetLastError = true)]
    private static extern ushort RegisterClassEx(in WindowClass windowClass);

    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint CreateWindowEx(
        uint extendedStyle, string className, string title, uint style,
        int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW",
        StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandle(string? name);

    [LibraryImport("user32.dll", EntryPoint = "LoadCursorW")]
    private static partial nint LoadCursor(nint instance, nint name);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShowWindow(nint window, int command);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UpdateWindow(nint window);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyWindow(nint window);

    [LibraryImport("user32.dll", EntryPoint = "DefWindowProcW")]
    internal static partial nint DefWindowProc(nint window, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
    internal static partial nint SendMessage(nint window, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool IsWindowVisible(nint window);

    [LibraryImport("user32.dll")]
    internal static partial uint GetDpiForWindow(nint window);

    [LibraryImport("user32.dll")]
    internal static partial nint GetParent(nint window);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetClientRect(nint window, out Rect rect);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowPos(
        nint window, nint insertAfter, int x, int y, int width, int height, uint flags);

    [LibraryImport("user32.dll", EntryPoint = "PeekMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PeekMessage(
        out Message message, nint window, uint minimum, uint maximum, uint remove);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool TranslateMessage(in Message message);

    [LibraryImport("user32.dll", EntryPoint = "DispatchMessageW")]
    internal static partial nint DispatchMessage(in Message message);

    [LibraryImport("user32.dll")]
    internal static partial void PostQuitMessage(int exitCode);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ValidateRect(nint window, nint rect);

    [LibraryImport("user32.dll")]
    internal static partial int GetSystemMetrics(int index);

    [LibraryImport("user32.dll", EntryPoint = "SystemParametersInfoW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SystemParametersInfo(
        uint action, uint parameter, ref HighContrast value, uint flags);

    [LibraryImport("dwmapi.dll")]
    internal static partial int DwmSetWindowAttribute(
        nint window, int attribute, ref int value, int valueSize);

    [LibraryImport("dwmapi.dll")]
    internal static partial int DwmGetWindowAttribute(
        nint window, int attribute, ref int value, int valueSize);

    [LibraryImport("dwmapi.dll")]
    internal static partial int DwmIsCompositionEnabled(
        [MarshalAs(UnmanagedType.Bool)] out bool enabled);
}
