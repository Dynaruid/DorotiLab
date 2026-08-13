using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Graphics;

namespace Doroti.Engine;

public sealed record RuntimeVerification(string Status, string Evidence, string? Diagnostic = null);

public sealed record RuntimeReportInput(
    string Backend,
    string BackendDiagnostic,
    double DevicePixelRatio,
    Size LogicalSize,
    int WarmupFrames,
    TimeSpan FirstPresent,
    int BaselineHandles,
    int BaselineThreads,
    IReadOnlyDictionary<string, RuntimeVerification> Verifications);

public static class RuntimeReportWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    public static void Write(string path, RuntimeReportInput input, IReadOnlyList<FrameTiming> timings, RasterCompositor compositor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(timings);
        ArgumentNullException.ThrowIfNull(compositor);
        var process = Process.GetCurrentProcess();
        process.Refresh();
        var measured = timings.Skip(Math.Min(input.WarmupFrames, timings.Count)).ToArray();
        var report = new
        {
            schemaVersion = "doroti.runtime-report/v1",
            status = input.Verifications.Values.Any(item => item.Status == "fail") ? "fail" :
                input.Verifications.Values.Any(item => item.Status == "not-verified") ? "partial" : "pass",
            environment = new
            {
                operatingSystem = Environment.OSVersion.VersionString,
                framework = Environment.Version.ToString(),
                processorCount = Environment.ProcessorCount,
                processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "unknown",
                backend = input.Backend,
                input.BackendDiagnostic,
                input.DevicePixelRatio,
                input.LogicalSize,
                pixelSize = PixelExtentPolicy.ToPixelSize(input.LogicalSize, input.DevicePixelRatio),
            },
            samples = new
            {
                warmupFrames = input.WarmupFrames,
                measuredFrames = measured.Length,
                firstPresentMicroseconds = input.FirstPresent.TotalMicroseconds,
                commit = Percentiles(measured.Select(item => item.Commit.TotalMicroseconds)),
                preroll = Percentiles(measured.Select(item => item.Preroll.TotalMicroseconds)),
                raster = Percentiles(measured.Select(item => item.Raster.TotalMicroseconds)),
                present = Percentiles(measured.Select(item => item.Present.TotalMicroseconds)),
                allocationBytes = Percentiles(measured.Select(item => (double)item.AllocatedBytes)),
                displayListBytes = Percentiles(measured.Select(item => (double)item.DisplayListBytes)),
                mailboxHighWatermark = compositor.QueueHighWatermark,
                supersededFrames = compositor.SupersededFrameCount,
            },
            resources = new
            {
                handleDelta = process.HandleCount - input.BaselineHandles,
                threadDelta = process.Threads.Count - input.BaselineThreads,
                gpuResourceDelta = "not-available-from-current-backend-port",
            },
            verifications = input.Verifications.OrderBy(item => item.Key, StringComparer.Ordinal)
                .ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal),
        };
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        File.WriteAllText(path, JsonSerializer.Serialize(report, JsonOptions).Replace("\r\n", "\n", StringComparison.Ordinal) + "\n");
    }

    private static object Percentiles(IEnumerable<double> source)
    {
        var values = source.Order().ToArray();
        return new
        {
            p50 = Percentile(values, 0.50),
            p95 = Percentile(values, 0.95),
            p99 = Percentile(values, 0.99),
        };
    }

    private static double? Percentile(IReadOnlyList<double> values, double percentile)
    {
        if (values.Count == 0)
        {
            return null;
        }
        var rank = percentile * (values.Count - 1);
        var lower = (int)Math.Floor(rank);
        var upper = (int)Math.Ceiling(rank);
        if (lower == upper)
        {
            return values[lower];
        }
        return values[lower] + ((values[upper] - values[lower]) * (rank - lower));
    }
}
