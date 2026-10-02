namespace Doroti.Host.Maui;

internal sealed class IosFrameLifetime
{
    internal bool TerminalCommitted { get; private set; }
    internal bool Retired { get; private set; }
    internal void MarkTerminalCommitted()
    {
        if (TerminalCommitted || Retired) throw new InvalidOperationException("Frame terminal already committed.");
        TerminalCommitted = true;
    }
    internal bool CanRetire(bool completed, bool deviceLost) => !Retired && (completed || deviceLost);
    internal void MarkRetired(bool completed, bool deviceLost)
    {
        if (!CanRetire(completed, deviceLost)) throw new InvalidOperationException("GPU completion is not established.");
        Retired = true;
    }
}
