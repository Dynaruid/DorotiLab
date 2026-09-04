namespace Doroti.Host.WindowsAppSdk;

internal readonly record struct MovingFrameKey(
    ulong ResizeEpoch, ulong MetricsGeneration, long InputSequence, uint SizingEdge,
    int Left, int Top, int Right, int Bottom, int Width, int Height, double Scale);

internal readonly record struct PreparedMovingFrame(
    MovingFrameKey Key, int SlotIndex, long ViewportRevision);

// Accessed only under the presenter's viewport gate. A prepared buffer has not
// been bound to IPresentationSurface and must not enter ordinary acquisition.
internal sealed class PreparedMovingFrameLedger
{
    internal PreparedMovingFrame? Current { get; private set; }
    internal ulong Prepared { get; private set; }
    internal ulong Cancelled { get; private set; }
    internal ulong Committed { get; private set; }
    internal bool IsReserved(int slot) => Current?.SlotIndex == slot;

    internal void Reserve(PreparedMovingFrame frame)
    {
        if (frame.SlotIndex is < 0 or >= 3 || frame.Key.ResizeEpoch == 0 ||
            frame.Key.MetricsGeneration == 0 || frame.Key.Width <= 0 ||
            frame.Key.Height <= 0 || !double.IsFinite(frame.Key.Scale) || frame.Key.Scale <= 0)
            throw new ArgumentException("Invalid prepared moving frame.", nameof(frame));
        Cancel();
        Current = frame;
        Prepared++;
    }

    internal bool Matches(MovingFrameKey key, long revision) =>
        Current is { } frame && frame.Key == key && frame.ViewportRevision == revision;

    internal bool Cancel()
    {
        if (Current is null) return false;
        Current = null;
        Cancelled++;
        return true;
    }

    internal bool Complete(MovingFrameKey key, long revision)
    {
        if (!Matches(key, revision)) return false;
        Current = null;
        Committed++;
        return true;
    }
}
