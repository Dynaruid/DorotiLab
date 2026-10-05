namespace Doroti.Host.Maui;

/// <summary>Coalesces native redraws without turning a GPU retry into a framework pulse.</summary>
internal sealed class MauiFrameWakeQueue
{
    private int _pending;

    internal void Request(bool prepareFramework) =>
        Interlocked.Or(ref _pending, prepareFramework ? 2 : 1);

    // Unsolicited native draws (layout/display link) are framework pulses.
    // A framework request always wins when coalesced with a raster retry.
    internal bool Take() => Interlocked.Exchange(ref _pending, 0) != 1;
}
