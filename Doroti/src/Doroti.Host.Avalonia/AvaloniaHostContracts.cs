using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Composition;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Rendering;

namespace Doroti.Host.Avalonia;

public enum AvaloniaHostRenderingMode
{
    Hardware,
    Software,
}

public sealed record AvaloniaPixelReadback(Size PixelSize, int RowBytes, byte[] Bgra8888Pixels);

public sealed record AvaloniaPixelBounds(int X, int Y, int Width, int Height);

public sealed record AvaloniaWindowCapture(
    AvaloniaPixelBounds WindowBounds,
    AvaloniaPixelBounds ClientBounds,
    double ScaleFactor,
    AvaloniaPixelReadback Screenshot);

public sealed record AvaloniaHostTraceEvent(
    long Sequence,
    string Kind,
    long TimestampTicks,
    int ThreadId,
    WindowId Window,
    WindowMetrics Metrics,
    string? Detail);

public sealed record AvaloniaHostTraceDocument(
    string SchemaVersion,
    AvaloniaHostRenderingMode RenderingMode,
    AvaloniaHostTraceEvent[] Events)
{
    public void Validate()
    {
        if (SchemaVersion != AvaloniaHostDiagnostics.TraceSchemaVersion)
        {
            throw new InvalidDataException($"Unsupported Avalonia host trace schema {SchemaVersion}.");
        }
        if (!Events.Select(item => item.Sequence).SequenceEqual(Enumerable.Range(1, Events.Length).Select(value => (long)value)))
        {
            throw new InvalidDataException("Avalonia host trace sequence is not contiguous.");
        }
        foreach (var window in Events.GroupBy(item => item.Window))
        {
            var kinds = window.Select(item => item.Kind).ToArray();
            RequireOrdered(kinds, window.Key, "created", "shown", "opened");
            if (kinds.Contains("closed", StringComparer.Ordinal))
            {
                RequireOrdered(kinds, window.Key, "close-requested", "closed");
            }
        }
    }

    private static void RequireOrdered(IReadOnlyList<string> kinds, WindowId window, params string[] expected)
    {
        var previous = -1;
        foreach (var kind in expected)
        {
            var index = -1;
            for (var candidate = previous + 1; candidate < kinds.Count; candidate++)
            {
                if (string.Equals(kinds[candidate], kind, StringComparison.Ordinal))
                {
                    index = candidate;
                    break;
                }
            }
            if (index < 0)
            {
                throw new InvalidDataException($"Window {window.Value} is missing ordered lifecycle event {kind}.");
            }
            previous = index;
        }
    }
}

public interface IAvaloniaDisplayListPresenter
{
    void Present(DisplayList displayList, IReadOnlyDictionary<ResourceId, IResourceSnapshot> resources);

    AvaloniaPixelReadback Capture(string? screenshotPath = null);
}

/// <summary>Captures the visible native window for target-only layout and rendering verification.</summary>
public interface IAvaloniaWindowCapture
{
    AvaloniaWindowCapture CaptureWindow(string screenshotPath);
}

public interface IAvaloniaHostDiagnostics
{
    AvaloniaHostRenderingMode RenderingMode { get; }

    AvaloniaHostTraceDocument Snapshot { get; }

    void Write(string path);
}

/// <summary>Target-only raw-platform replay used by runtime-v2 input diagnostics.</summary>
public interface IAvaloniaInputDiagnosticController
{
    void EmitPointer(
        string correlationId,
        ulong deviceId,
        PointerDeviceKind deviceKind,
        PointerPhase phase,
        Offset logicalPosition,
        uint buttons,
        Offset platformScrollDelta = default);
}

public static class AvaloniaHostDisplayListSupport
{
    public static void Validate(DisplayList displayList, IReadOnlyDictionary<ResourceId, IResourceSnapshot> resources)
    {
        ArgumentNullException.ThrowIfNull(displayList);
        ArgumentNullException.ThrowIfNull(resources);
        foreach (var command in displayList.Commands)
        {
            switch (command)
            {
                case SaveCommand or RestoreCommand or TransformCommand or ClipRectCommand or ClipPathCommand or
                    DrawColorCommand or DrawRectCommand or DrawPathCommand or DrawTextCommand:
                    break;
                case DrawImageCommand image:
                    if (!resources.TryGetValue(image.Resource, out var snapshot) || snapshot is not ImageResourceSnapshot)
                    {
                        throw new InvalidOperationException($"Image resource {image.Resource.Value} is missing or is not an immutable image snapshot.");
                    }
                    break;
                default:
                    throw new NotSupportedException($"The Avalonia host does not support {command.GetType().Name}.");
            }
        }
    }
}

internal sealed class AvaloniaHostDiagnostics(AvaloniaHostRenderingMode renderingMode) : IAvaloniaHostDiagnostics
{
    internal const string TraceSchemaVersion = "doroti.avalonia-host-trace/v1";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };
    private readonly object _gate = new();
    private readonly List<AvaloniaHostTraceEvent> _events = [];
    private long _sequence;

    public AvaloniaHostRenderingMode RenderingMode { get; } = renderingMode;

    public AvaloniaHostTraceDocument Snapshot
    {
        get
        {
            lock (_gate)
            {
                return new(TraceSchemaVersion, RenderingMode, _events.ToArray());
            }
        }
    }

    internal void Record(string kind, WindowId window, WindowMetrics metrics, string? detail = null)
    {
        lock (_gate)
        {
            _events.Add(new(
                ++_sequence,
                kind,
                System.Diagnostics.Stopwatch.GetTimestamp(),
                Environment.CurrentManagedThreadId,
                window,
                metrics,
                detail));
        }
    }

    public void Write(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var document = Snapshot;
        document.Validate();
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        File.WriteAllText(path, JsonSerializer.Serialize(document, JsonOptions).Replace("\r\n", "\n", StringComparison.Ordinal) + "\n");
    }
}
