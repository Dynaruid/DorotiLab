using Doroti.Ui;

namespace Doroti.Host.Maui;

// Native recognizers report cumulative physical pan/scale/rotation. Keep the
// origin fixed, derive deltas once, and give each gesture a fresh pointer ID.
internal sealed class MauiTrackpadGesture(ulong device, Action<MauiSurfacePointerData> dispatch)
{
    private static long _nextPointer = 1L << 40;
    private bool _added;
    private MauiSurfacePointerData _last;
    internal bool Active { get; private set; }

    internal void Begin(TimeSpan time, double x, double y)
    {
        if (Active)
        {
            return;
        }

        _last = new(
            time,
            PointerChange.panZoomStart,
            PointerDeviceKind.trackpad,
            checked((ulong)Interlocked.Increment(ref _nextPointer)),
            x,
            y,
            0,
            0,
            0,
            PointerSignalKind.none,
            0,
            Device: device
        );
        if (!_added)
        {
            dispatch(_last with { Change = PointerChange.add });
            _added = true;
        }
        Active = true;
        dispatch(_last);
    }

    internal void Update(
        TimeSpan time,
        double panX,
        double panY,
        double scale = 1,
        double rotation = 0
    )
    {
        if (
            !Active
            || !double.IsFinite(panX)
            || !double.IsFinite(panY)
            || !double.IsFinite(scale)
            || scale <= 0
            || !double.IsFinite(rotation)
        )
        {
            return;
        }

        _last = _last with
        {
            Timestamp = time,
            Change = PointerChange.panZoomUpdate,
            PanDeltaX = panX - _last.PanX,
            PanDeltaY = panY - _last.PanY,
            PanX = panX,
            PanY = panY,
            Scale = scale,
            Rotation = rotation,
        };
        dispatch(_last);
    }

    internal void End(TimeSpan time)
    {
        if (!Active)
        {
            return;
        }

        Active = false;
        _last = _last with
        {
            Timestamp = time,
            Change = PointerChange.panZoomEnd,
            PanDeltaX = 0,
            PanDeltaY = 0,
        };
        dispatch(_last);
    }

    internal void CancelInertia(TimeSpan time)
    {
        if (_added)
        {
            dispatch(
                _last with
                {
                    Timestamp = time,
                    Change = PointerChange.hover,
                    SignalKind = PointerSignalKind.scrollInertiaCancel,
                    PanDeltaX = 0,
                    PanDeltaY = 0,
                }
            );
        }
    }

    internal void Remove(TimeSpan time)
    {
        End(time);
        if (_added)
        {
            dispatch(_last with { Timestamp = time, Change = PointerChange.remove });
        }

        _added = false;
    }
}
