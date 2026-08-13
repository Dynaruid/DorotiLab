using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Shell.Core;

namespace Doroti.Host.Desktop;

/// <summary>Converts the internal vendor event shape into backend-neutral Doroti contracts.</summary>
internal static class DesktopAdapterBoundary
{
    internal static WindowMetrics Convert(ShellWindowMetrics input) => new(
        input.LogicalClientSize,
        input.PhysicalClientSize,
        input.ScaleFactor,
        input.State == ShellWindowState.Minimized,
        input.Generation,
        input.ScaleGeneration,
        input.SurfaceGeneration);

    internal static DisplayInfo Convert(ShellScreen input) => new(new(input.Id), input.WorkArea);
}
