using System.Diagnostics;

namespace Doroti.Skia.Vulkan;

public readonly record struct VulkanWindowFrameTiming(
    double AcquireMs, double PaintMs, double SubmitMs, double CopyMs,
    double FenceMs, double PresentMs, double TotalMs);

public sealed unsafe partial class GraphiteVulkanWindow
{
    /// <summary>Opt-in CPU phase timing; none of these values is a scan-out receipt.</summary>
    public bool EnableFrameTiming { get; set; }
    public VulkanWindowFrameTiming LastFrameTiming { get; private set; }

    private long FrameTimestamp() => EnableFrameTiming ? Stopwatch.GetTimestamp() : 0;
    private static double Milliseconds(long start, long end) =>
        start == 0 || end == 0 ? 0 : Stopwatch.GetElapsedTime(start, end).TotalMilliseconds;
}
