#if WINDOWS
using System.Diagnostics.Tracing;
using Doroti.Ui;

namespace Doroti.Host.Maui;

/// <summary>
/// Metadata-only ETW markers used to correlate Doroti resize generations with
/// the DXGI Present events captured by the Windows GPU profile. The provider
/// is disabled unless an ETW session explicitly enables it.
/// </summary>
[EventSource(
    Name = "Doroti-Windows-Resize",
    Guid = "5a846f8d-54a1-4a4c-9e56-5b5a84e3b3c1")]
internal sealed class WindowsResizeEtw : EventSource
{
    internal static readonly WindowsResizeEtw Log = new();
    internal const string ProviderGuid = "{5a846f8d-54a1-4a4c-9e56-5b5a84e3b3c1}";

    private WindowsResizeEtw()
    {
    }

    [NonEvent]
    internal void Marker(
        string phase,
        DorotiResizeEpoch epoch,
        int surfaceWidth,
        int surfaceHeight,
        string source)
    {
        if (!IsEnabled()) return;
        ResizeMarker(
            phase,
            epoch.Generation,
            epoch.PhysicalWidth,
            epoch.PhysicalHeight,
            surfaceWidth,
            surfaceHeight,
            Environment.CurrentManagedThreadId,
            source);
    }

    [Event(1, Level = EventLevel.Informational, Opcode = EventOpcode.Info)]
    private void ResizeMarker(
        string phase,
        long generation,
        int physicalWidth,
        int physicalHeight,
        int surfaceWidth,
        int surfaceHeight,
        int managedThreadId,
        string source) =>
        WriteEvent(1, new object?[]
        {
            phase,
            generation,
            physicalWidth,
            physicalHeight,
            surfaceWidth,
            surfaceHeight,
            managedThreadId,
            source,
        });
}
#endif
