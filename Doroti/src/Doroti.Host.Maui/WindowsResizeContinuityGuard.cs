#if WINDOWS
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using Doroti.Ui;
using Microsoft.UI.Xaml;
using SkiaSharp.Views.Maui.Controls;

namespace Doroti.Host.Maui;

/// <summary>
/// Turns each Win32 interactive-resize notification into a synchronous
/// metrics -> framework frame -> ANGLE present transaction. WinUI/SkiaSharp
/// otherwise resize the swap chain before the framework has produced a scene
/// for the new metrics, which makes retained content visibly trail the window.
/// </summary>
internal sealed class WindowsResizeContinuityGuard : IDisposable
{
    private const uint WmSize = 0x0005;
    private const uint WmDpiChanged = 0x02e0;
    private const uint WmExitSizeMove = 0x0232;
    private const nuint SizeMinimized = 1;
    private static long _nextSubclassId;

    private readonly SKGLView _view;
    private readonly Action<MauiSynchronousResize> _prepareFrame;
    private readonly Action<MauiPaintCompletion> _presentCompleted;
    private readonly SubclassProc _subclassProc;
    private readonly nuint _subclassId;
    private readonly DorotiResizeTrace _trace = new();
    private DorotiWindowsSwapChainPanel? _swapChainPanel;
    private nint _hwnd;
    private nint _eglDisplay;
    private nint _eglDrawSurface;
    private long _eglSurfaceGeneration;
    private long _eglPolicySurfaceGeneration = -1;
    private string _eglSwapIntervalPolicy = "unknown-default-public-SKGLView";
    private DorotiWindowsPreSwap? _preSwap;
    private MauiPaintCompletion? _synchronousCompletion;
    private long _activations;
    private long _deactivations;
    private long _synchronousPresents;
    private long _synchronousMisses;
    private bool _synchronousPaint;
    private bool _insideResize;
    private bool _active;
    private bool _disposed;
    private bool _exactSwapTimingAvailable;
    private long _resizeGeneration;
    private DorotiResizeEpoch? _currentEpoch;
    private (double Width, double Height, double Density) _lastTarget;
    private bool _dwmCompositionEnabled;

    internal WindowsResizeContinuityGuard(
        SKGLView view,
        Action<MauiSynchronousResize> prepareFrame,
        Action<MauiPaintCompletion> presentCompleted)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _prepareFrame = prepareFrame ?? throw new ArgumentNullException(nameof(prepareFrame));
        _presentCompleted = presentCompleted ?? throw new ArgumentNullException(nameof(presentCompleted));
        _subclassProc = WindowSubclass;
        _subclassId = checked((nuint)Interlocked.Increment(ref _nextSubclassId));
        _view.HandlerChanged += HandleViewChanged;
        _view.Loaded += HandleViewChanged;
        _view.Unloaded += HandleViewUnloaded;
        TryAttach();
    }

    internal MauiSurfaceSnapshot CaptureSnapshot(MauiSurfaceSnapshot current) => current with
    {
        ResizeContinuityActivations = Interlocked.Read(ref _activations),
        ResizeContinuityDeactivations = Interlocked.Read(ref _deactivations),
        ResizeContinuityActive = Volatile.Read(ref _active),
        ResizeSynchronousPresents = Interlocked.Read(ref _synchronousPresents),
        ResizeSynchronousMisses = Interlocked.Read(ref _synchronousMisses),
        DwmCompositionEnabled = _dwmCompositionEnabled,
        EglSwapIntervalPolicy = _eglSwapIntervalPolicy,
        ExactSwapTimingAvailable = _exactSwapTimingAvailable,
        ResizeTrace = _trace.Snapshot(),
    };

    internal void Record(
        string phase,
        DorotiResizeEpoch epoch,
        string source,
        TimeSpan? duration = null,
        int surfaceWidth = 0,
        int surfaceHeight = 0,
        string? terminal = null,
        string? detail = null)
    {
        _trace.Record(phase, epoch, source, duration,
            surfaceWidth: surfaceWidth, surfaceHeight: surfaceHeight,
            terminal: terminal, detail: detail);
        WindowsResizeEtw.Log.Marker(phase, epoch, surfaceWidth, surfaceHeight, source);
    }

    internal void ObserveCurrentEgl(int surfaceWidth, int surfaceHeight)
    {
        var display = WindowsEglInterop.eglGetCurrentDisplay();
        var drawSurface = WindowsEglInterop.eglGetCurrentSurface(WindowsEglInterop.EglDraw);
        if (display == 0 || drawSurface == 0) return;

        if (display != _eglDisplay || drawSurface != _eglDrawSurface)
        {
            _eglDisplay = display;
            _eglDrawSurface = drawSurface;
            _eglSurfaceGeneration++;
        }

        var requested = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_EGL_SWAP_INTERVAL");
        var requestedInterval = requested is "0" or "1" ? int.Parse(requested) : (int?)null;
        var callState = "not-called";
        var eglError = WindowsEglInterop.EglSuccess;
        if (requestedInterval is { } interval && _eglPolicySurfaceGeneration != _eglSurfaceGeneration)
        {
            var succeeded = WindowsEglInterop.eglSwapInterval(display, interval) != 0;
            eglError = WindowsEglInterop.eglGetError();
            callState = succeeded && eglError == WindowsEglInterop.EglSuccess ? "true" : "false";
            _eglPolicySurfaceGeneration = _eglSurfaceGeneration;
            _eglSwapIntervalPolicy = succeeded && eglError == WindowsEglInterop.EglSuccess
                ? $"requested-{interval}-retained-generation-{_eglSurfaceGeneration}"
                : $"requested-{interval}-failed-0x{eglError:x4}-generation-{_eglSurfaceGeneration}";
        }
        else if (requestedInterval is null)
        {
            _eglSwapIntervalPolicy = $"default-retained-generation-{_eglSurfaceGeneration}";
        }

        _dwmCompositionEnabled = DwmIsCompositionEnabled(out var enabled) == 0 && enabled;
        if (_currentEpoch is { } epoch)
        {
            Record("egl-state", epoch, "WindowsEglInterop",
                surfaceWidth: surfaceWidth, surfaceHeight: surfaceHeight,
                detail: $"display=0x{display:x}; drawSurface=0x{drawSurface:x}; " +
                        $"surfaceGeneration={_eglSurfaceGeneration}; composition={_dwmCompositionEnabled}; " +
                        $"requestedInterval={requestedInterval?.ToString() ?? "default"}; " +
                        $"callSuccess={callState}; eglError=0x{eglError:x4}");
        }
    }

    internal void RecordRasterStart(int width, int height)
    {
        if (_currentEpoch is { } epoch)
            Record("raster-start", epoch, "SKGLView.PaintSurface",
                surfaceWidth: width, surfaceHeight: height);
    }

    internal void RecordRasterEnd(int width, int height, TimeSpan duration)
    {
        if (_currentEpoch is { } epoch)
            Record("raster-end", epoch, "SKGLView.PaintSurface", duration,
                surfaceWidth: width, surfaceHeight: height);
    }

    internal bool CaptureSynchronousCompletion(MauiPaintCompletion completion)
    {
        if (!_synchronousPaint) return false;
        _synchronousCompletion = completion;
        return true;
    }

    private void HandleViewChanged(object? sender, EventArgs args)
    {
        _ = sender;
        _ = args;
        TryAttach();
    }

    private void HandleViewUnloaded(object? sender, EventArgs args)
    {
        _ = sender;
        _ = args;
        Detach();
    }

    private void TryAttach()
    {
        if (_disposed) return;
        if (_view.Handler?.PlatformView is DorotiWindowsSwapChainPanel panel &&
            !ReferenceEquals(panel, _swapChainPanel))
        {
            DetachPanel();
            _swapChainPanel = panel;
            _swapChainPanel.BeforeFinalSwap += HandleBeforeFinalSwap;
            _swapChainPanel.ContextDestroying += HandleContextDestroying;
            _exactSwapTimingAvailable = true;
        }

        var platformWindow = _view.Window?.Handler?.PlatformView;
        if (platformWindow is null) return;
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(platformWindow);
        if (hwnd == 0 || hwnd == _hwnd) return;
        DetachWindow();
        if (!SetWindowSubclass(hwnd, _subclassProc, _subclassId, 0))
            throw new InvalidOperationException(
                $"Unable to install the Doroti resize-continuity HWND subclass (error {Marshal.GetLastWin32Error()}).");
        _hwnd = hwnd;
    }

    private void HandleBeforeFinalSwap(DorotiWindowsPreSwap boundary)
    {
        if (!_synchronousPaint || _currentEpoch is not { } epoch) return;
        _preSwap = boundary;
        Record("pre-swap", epoch, "DorotiWindowsSwapChainPanel",
            surfaceWidth: boundary.SurfaceWidth, surfaceHeight: boundary.SurfaceHeight,
            detail: "base.OnRenderFrame returned; final eglSwapBuffers has not started");
    }

    private void HandleContextDestroying()
    {
        _eglDisplay = 0;
        _eglDrawSurface = 0;
        _eglPolicySurfaceGeneration = -1;
    }

    private nint WindowSubclass(
        nint hwnd,
        uint message,
        nuint wParam,
        nint lParam,
        nuint subclassId,
        nuint referenceData)
    {
        _ = subclassId;
        _ = referenceData;

        var resizeMessage = message is WmSize or WmDpiChanged or WmExitSizeMove;
        if (!resizeMessage || _disposed || _insideResize ||
            (message == WmSize && wParam == SizeMinimized))
            return DefSubclassProc(hwnd, message, wParam, lParam);

        _insideResize = true;
        try
        {
            var result = DefSubclassProc(hwnd, message, wParam, lParam);
            SynchronizeResize(message switch
            {
                WmSize => "WM_SIZE",
                WmDpiChanged => "WM_DPICHANGED",
                _ => "WM_EXITSIZEMOVE",
            });
            return result;
        }
        finally
        {
            _insideResize = false;
        }
    }

    private void SynchronizeResize(string source)
    {
        if (_disposed || !_view.IsLoaded ||
            _view.Handler?.PlatformView is not FrameworkElement native ||
            native.ActualWidth <= 0 || native.ActualHeight <= 0) return;

        var activityId = Guid.NewGuid();
        EventSource.SetCurrentThreadActivityId(activityId, out var previousActivityId);
        _active = true;
        Interlocked.Increment(ref _activations);
        try
        {
            native.UpdateLayout();
            var density = Math.Max(1, native.XamlRoot?.RasterizationScale ?? 1);
            var target = (native.ActualWidth, native.ActualHeight, density);
            if (target != _lastTarget)
            {
                _lastTarget = target;
                _resizeGeneration++;
            }
            var epoch = new DorotiResizeEpoch(
                _resizeGeneration,
                native.ActualWidth,
                native.ActualHeight,
                Math.Max(1, checked((int)Math.Round(native.ActualWidth * density))),
                Math.Max(1, checked((int)Math.Round(native.ActualHeight * density))),
                density,
                DorotiFrameClock.Now.Ticks / 10);
            _currentEpoch = epoch;
            Record("target", epoch, source);
            var prepareStarted = DorotiFrameClock.Now;
            _prepareFrame(new(native.ActualWidth, native.ActualHeight, density, epoch));
            Record("framework-ready", epoch, "synchronous-resize", DorotiFrameClock.Now - prepareStarted);

            _preSwap = null;
            _synchronousCompletion = null;
            _synchronousPaint = true;
            try
            {
                var swapBoundaryStarted = DorotiFrameClock.Now;
                Record("swap-boundary-start", epoch, "SKGLView.InvalidateSurface",
                    detail: "aggregate private leading resize swap, paint, and final ANGLE swap boundary");
                _view.InvalidateSurface();
                if (_preSwap is { } preSwap)
                {
                    Record("post-swap", epoch, "DorotiWindowsSwapChainPanel",
                        DorotiFrameClock.Now - preSwap.Timestamp,
                        preSwap.SurfaceWidth, preSwap.SurfaceHeight,
                        detail: "synchronous InvalidateSurface returned after final eglSwapBuffers");
                }
                Record("swap-boundary-end", epoch, "SKGLView.InvalidateSurface",
                    DorotiFrameClock.Now - swapBoundaryStarted,
                    detail: "aggregate boundary; pre-swap/post-swap isolates only the final ANGLE swap");
            }
            finally
            {
                _synchronousPaint = false;
            }

            _dwmCompositionEnabled = DwmIsCompositionEnabled(out var compositionEnabled) == 0 && compositionEnabled;
            var dwmStarted = DorotiFrameClock.Now;
            Record("dwm-flush-start", epoch, "DwmFlush",
                detail: $"composition={_dwmCompositionEnabled}");
            var dwmResult = DwmFlush();
            Record("dwm-flush-end", epoch, "DwmFlush", DorotiFrameClock.Now - dwmStarted,
                detail: $"hresult={dwmResult}; composition={_dwmCompositionEnabled}");
            if (_synchronousCompletion is { } completion)
            {
                Interlocked.Increment(ref _synchronousPresents);
                Record("ack", epoch, "SKGLView return plus DwmFlush",
                    terminal: "presented",
                    detail: $"scene={completion.SceneSequence}; surface={completion.SurfaceGeneration}");
                _presentCompleted(completion);
            }
            else
            {
                Interlocked.Increment(ref _synchronousMisses);
                Record("ack", epoch, "SKGLView return plus DwmFlush",
                    terminal: "dropped", detail: "PaintSurface produced no scene completion");
            }
        }
        finally
        {
            _preSwap = null;
            _synchronousCompletion = null;
            _currentEpoch = null;
            _active = false;
            Interlocked.Increment(ref _deactivations);
            EventSource.SetCurrentThreadActivityId(previousActivityId);
        }
    }

    private void DetachPanel()
    {
        if (_swapChainPanel is null) return;
        _swapChainPanel.BeforeFinalSwap -= HandleBeforeFinalSwap;
        _swapChainPanel.ContextDestroying -= HandleContextDestroying;
        _swapChainPanel = null;
        _exactSwapTimingAvailable = false;
    }

    private void DetachWindow()
    {
        var hwnd = _hwnd;
        _hwnd = 0;
        if (hwnd != 0) _ = RemoveWindowSubclass(hwnd, _subclassProc, _subclassId);
    }

    private void Detach()
    {
        DetachWindow();
        DetachPanel();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _view.HandlerChanged -= HandleViewChanged;
        _view.Loaded -= HandleViewChanged;
        _view.Unloaded -= HandleViewUnloaded;
        Detach();
        GC.KeepAlive(_subclassProc);
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate nint SubclassProc(
        nint hwnd,
        uint message,
        nuint wParam,
        nint lParam,
        nuint subclassId,
        nuint referenceData);

    [DllImport("comctl32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowSubclass(
        nint hwnd,
        SubclassProc subclassProc,
        nuint subclassId,
        nuint referenceData);

    [DllImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RemoveWindowSubclass(
        nint hwnd,
        SubclassProc subclassProc,
        nuint subclassId);

    [DllImport("comctl32.dll")]
    private static extern nint DefSubclassProc(nint hwnd, uint message, nuint wParam, nint lParam);

    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmFlush();

    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmIsCompositionEnabled([MarshalAs(UnmanagedType.Bool)] out bool enabled);
}
#endif
