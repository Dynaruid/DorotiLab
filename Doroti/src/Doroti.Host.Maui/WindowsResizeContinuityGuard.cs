#if WINDOWS
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Doroti.Ui;
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
    private nint _hwnd;
    private MauiPaintCompletion? _synchronousCompletion;
    private long _activations;
    private long _deactivations;
    private long _synchronousPresents;
    private long _synchronousMisses;
    private bool _synchronousPaint;
    private bool _insideResize;
    private bool _active;
    private bool _disposed;
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
        EglSwapIntervalPolicy = "unknown-default-public-SKGLView",
        ExactSwapTimingAvailable = false,
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
        string? detail = null) =>
        _trace.Record(phase, epoch, source, duration,
            surfaceWidth: surfaceWidth, surfaceHeight: surfaceHeight,
            terminal: terminal, detail: detail);

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

    /// <summary>
    /// Called from SKGLView.PaintSurface immediately before SKSwapChainPanel
    /// performs eglSwapBuffers. A synchronous resize owns this completion and
    /// publishes it only after InvalidateSurface has returned from that swap.
    /// </summary>
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
        var platformWindow = _view.Window?.Handler?.PlatformView;
        if (platformWindow is null) return;
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(platformWindow);
        if (hwnd == 0 || hwnd == _hwnd) return;
        Detach();
        if (!SetWindowSubclass(hwnd, _subclassProc, _subclassId, 0))
            throw new InvalidOperationException(
                $"Unable to install the Doroti resize-continuity HWND subclass (error {Marshal.GetLastWin32Error()}).");
        _hwnd = hwnd;
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

        // DefSubclassProc may synchronously cause nested layout/size messages.
        // Coalesce those into this outer transaction and paint only once after
        // WinUI and AngleSwapChainPanel have consumed the final native size.
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

            _synchronousCompletion = null;
            _synchronousPaint = true;
            try
            {
                // The Windows handler renders synchronously with HasRenderLoop
                // disabled. Returning means eglSwapBuffers completed.
                var swapBoundaryStarted = DorotiFrameClock.Now;
                Record("swap-boundary-start", epoch, "SKGLView.InvalidateSurface",
                    detail: "aggregate paint plus ANGLE swap; exact eglSwapBuffers timing unavailable");
                _view.InvalidateSurface();
                Record("swap-boundary-end", epoch, "SKGLView.InvalidateSurface",
                    DorotiFrameClock.Now - swapBoundaryStarted,
                    detail: "aggregate paint plus ANGLE swap; exact eglSwapBuffers timing unavailable");
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
            _synchronousCompletion = null;
            _currentEpoch = null;
            _active = false;
            Interlocked.Increment(ref _deactivations);
        }
    }

    private void Detach()
    {
        var hwnd = _hwnd;
        _hwnd = 0;
        if (hwnd != 0) _ = RemoveWindowSubclass(hwnd, _subclassProc, _subclassId);
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
