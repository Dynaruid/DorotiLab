using Doroti.Ui;

namespace Doroti.Host.Maui;

// AndroidTouchProcessor's SOURCE_MOUSE/buttonless stream, shared by Graphite
// and the SKTouch fallback. Normal mouse/touch streams remain ordinary pointers.
internal sealed class AndroidTrackpadGesture(Action<MauiSurfacePointerData> dispatch)
{
    private readonly Dictionary<ulong, (MauiTrackpadGesture Gesture, double X, double Y)> _pans = [];

    internal bool Handle(ulong device, PointerChange change, PointerDeviceKind kind,
        bool sourceMouse, int buttons, double x, double y, TimeSpan time)
    {
        if (change == PointerChange.down && kind == PointerDeviceKind.mouse && sourceMouse && buttons == 0)
        {
            if (_pans.Remove(device, out var old)) old.Gesture.Remove(time);
            var gesture = new MauiTrackpadGesture((1UL << 62) | device, dispatch);
            _pans[device] = (gesture, x, y);
            gesture.Begin(time, x, y);
            return true;
        }
        if (!_pans.TryGetValue(device, out var pan)) return false;
        if (change == PointerChange.move) pan.Gesture.Update(time, x - pan.X, y - pan.Y);
        else if (change is PointerChange.up or PointerChange.cancel or PointerChange.remove)
        {
            pan.Gesture.Remove(time);
            _pans.Remove(device);
        }
        return true;
    }

    internal void Cancel(TimeSpan time)
    {
        foreach (var pan in _pans.Values) pan.Gesture.Remove(time);
        _pans.Clear();
    }
}
