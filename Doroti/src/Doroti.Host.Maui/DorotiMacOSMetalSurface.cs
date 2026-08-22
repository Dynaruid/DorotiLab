#if MACOS
using Doroti.Ui;
using Microsoft.Maui.Dispatching;

namespace Doroti.Host.Maui;

/// <summary>MAUI virtual view for the AppKit-owned MTKView render surface.</summary>
public sealed class DorotiMacOSMetalSurface : View, IMauiSkiaSurface
{
    private readonly ulong _viewId;
    private DorotiMacOSMetalView? _nativeView;
    private bool _disposed;

    internal DorotiMacOSMetalSurface(ulong viewId) => _viewId = viewId;

    View IMauiSkiaSurface.Element => this;
    IDispatcher IMauiSkiaSurface.Dispatcher => Dispatcher;
    event Action<MauiSkiaPaintContext>? IMauiSkiaSurface.Paint
    {
        add => Paint += value;
        remove => Paint -= value;
    }
    event Action<MauiPaintCompletion, bool>? IMauiSkiaSurface.PresentCompleted
    {
        add => PresentCompleted += value;
        remove => PresentCompleted -= value;
    }
    event Action<MauiPaintCompletion?, Exception>? IMauiSkiaSurface.PaintFailed
    {
        add => PaintFailed += value;
        remove => PaintFailed -= value;
    }
    event Action<MauiSurfacePointerData>? IMauiSkiaSurface.Pointer
    {
        add => Pointer += value;
        remove => Pointer -= value;
    }
    event Action<KeyData>? IMauiSkiaSurface.Key
    {
        add => Key += value;
        remove => Key -= value;
    }
    event Action<bool>? IMauiSkiaSurface.FocusChanged
    {
        add => FocusChanged += value;
        remove => FocusChanged -= value;
    }
    event Action<DorotiResizeEpoch?>? IMauiSkiaSurface.SizeChanged
    {
        add => SurfaceSizeChanged += value;
        remove => SurfaceSizeChanged -= value;
    }

    private event Action<MauiSkiaPaintContext>? Paint;
    private event Action<MauiPaintCompletion, bool>? PresentCompleted;
    private event Action<MauiPaintCompletion?, Exception>? PaintFailed;
    private event Action<MauiSurfacePointerData>? Pointer;
    private event Action<KeyData>? Key;
    private event Action<bool>? FocusChanged;
    private event Action<DorotiResizeEpoch?>? SurfaceSizeChanged;

    internal ulong ViewId => _viewId;

    internal void Connect(DorotiMacOSMetalView nativeView)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _nativeView = nativeView;
        nativeView.Connect(this);
    }

    internal void Disconnect(DorotiMacOSMetalView nativeView)
    {
        if (!ReferenceEquals(_nativeView, nativeView)) return;
        _nativeView = null;
        nativeView.Disconnect();
    }

    internal MauiPaintCompletion? RaisePaint(MauiSkiaPaintContext context)
    {
        if (_disposed) return null;
        Paint?.Invoke(context);
        return context.Completion;
    }

    internal void RaisePresent(MauiPaintCompletion completion, bool stale)
    {
        if (!_disposed) PresentCompleted?.Invoke(completion, stale);
    }

    internal void RaiseFailure(Exception exception, MauiPaintCompletion? completion = null)
    {
        if (!_disposed) PaintFailed?.Invoke(completion, exception);
    }

    internal void RaisePointer(MauiSurfacePointerData data)
    {
        if (!_disposed) Pointer?.Invoke(data);
    }

    internal void RaiseKey(KeyData data)
    {
        if (!_disposed) Key?.Invoke(data);
    }

    internal void RaiseFocus(bool focused)
    {
        if (!_disposed) FocusChanged?.Invoke(focused);
    }

    internal void RaiseSizeChanged()
    {
        if (!_disposed) SurfaceSizeChanged?.Invoke(null);
    }

    void IMauiSkiaSurface.InvalidateSurface() => _nativeView?.RequestFrame();
    void IMauiSkiaSurface.RequestFocus(bool focused) => _nativeView?.RequestFocus(focused);
    void IMauiSkiaSurface.SetCursor(DorotiMouseCursorKind cursor) => _nativeView?.SetCursor(cursor);
    MauiSurfaceSnapshot IMauiSkiaSurface.CaptureSnapshot(MauiSurfaceSnapshot current) =>
        _nativeView?.CaptureSnapshot(current) ?? current;

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        var native = _nativeView;
        _nativeView = null;
        native?.Disconnect();
    }
}
#endif
