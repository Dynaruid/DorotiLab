using System.Collections.Concurrent;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using Doroti.Ui;

namespace Doroti.Host.Web;

internal interface ICanvasKitSceneCallback
{
    void CompleteScene(long sceneSequence, string terminal, string reason, string receiptJson);
}

internal interface ICanvasKitResourceCallback
{
    void CompleteResource(
        long resourceId,
        long generation,
        string terminal,
        string reason,
        string receiptJson);
}

/// <summary>
/// Managed boundary hosted exclusively by the UI Worker.  The JavaScript side
/// copies each managed memory view before transferring its ArrayBuffer to the
/// dedicated Raster Worker.
/// </summary>
[SupportedOSPlatform("browser")]
internal static partial class BrowserCanvasKitInterop
{
    private const string Module = "doroti.web";
    private static readonly ConcurrentDictionary<long, ICanvasKitSceneCallback> Scenes = new();
    private static readonly ConcurrentDictionary<ulong, DorotiFrameTrace> Traces = new();
    internal static void RegisterTrace(ulong viewId, DorotiFrameTrace trace) => Traces[viewId] = trace;
    internal static void ForgetTrace(ulong viewId) => Traces.TryRemove(viewId, out _);

    [JSExport]
    internal static string CaptureFrameTrace()
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteNumber("clockMicroseconds", DorotiFrameClock.Now.Ticks / 10);
            writer.WriteStartArray("entries");
            foreach (var trace in Traces.Values)
                foreach (var entry in trace.Snapshot())
                {
                    writer.WriteStartObject();
                    writer.WriteNumber("sequence", entry.Sequence);
                      writer.WriteNumber("timestampMicroseconds", entry.TimestampMicroseconds);
                      writer.WriteNumber("recordedAtMicroseconds", entry.RecordedAtMicroseconds);
                    writer.WriteString("phase", entry.Phase.ToString());
                    writer.WriteNumber("viewId", entry.ViewId);
                    writer.WriteNumber("frame", entry.FrameworkFrameNumber);
                    writer.WriteNumber("resizeGeneration", entry.ResizeTargetGeneration);
                    writer.WriteNumber("scene", entry.SceneSequence);
                    writer.WriteEndObject();
                }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
        return Encoding.UTF8.GetString(stream.ToArray());
    }
    private static readonly ConcurrentDictionary<(long Id, long Generation), ICanvasKitResourceCallback> Resources = new();

    [JSImport("initializeCanvasKitManagedCallbacks", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    internal static partial Task<string> InitializeManagedCallbacksAsync();

    [JSImport("submitCanvasKitDisplayList", Module)]
    [return: JSMarshalAs<JSType.Number>]
    private static partial long SubmitDisplayList(
        [JSMarshalAs<JSType.MemoryView>] Span<byte> bytes);

    [JSImport("registerCanvasKitResource", Module)]
    private static partial void RegisterResourceCore(
        [JSMarshalAs<JSType.Number>] long resourceId,
        [JSMarshalAs<JSType.Number>] long generation,
        string kind,
        string descriptorJson,
        [JSMarshalAs<JSType.MemoryView>] Span<byte> bytes);

    [JSImport("releaseCanvasKitResource", Module)]
    private static partial void ReleaseResourceCore(
        [JSMarshalAs<JSType.Number>] long resourceId,
        [JSMarshalAs<JSType.Number>] long generation);

    [JSImport("layoutCanvasKitParagraph", Module)]
    internal static partial string LayoutParagraph(string requestJson);

    internal static long Submit(
        long sceneSequence,
        byte[] wireBytes,
        ICanvasKitSceneCallback callback)
    {
        if (sceneSequence <= 0) throw new ArgumentOutOfRangeException(nameof(sceneSequence));
        ArgumentNullException.ThrowIfNull(wireBytes);
        ArgumentNullException.ThrowIfNull(callback);
        if (!Scenes.TryAdd(sceneSequence, callback))
            throw new InvalidOperationException($"CanvasKit scene {sceneSequence} is already admitted.");
        try
        {
            var admittedSequence = SubmitDisplayList(wireBytes.AsSpan());
            if (admittedSequence != sceneSequence)
                throw new InvalidDataException(
                    $"CanvasKit admitted scene {admittedSequence}, expected {sceneSequence}.");
            return admittedSequence;
        }
        catch
        {
            Scenes.TryRemove(sceneSequence, out _);
            throw;
        }
    }

    internal static void RegisterResource(
        long resourceId,
        long generation,
        string kind,
        string descriptorJson,
        byte[] bytes,
        ICanvasKitResourceCallback callback)
    {
        if (resourceId <= 0) throw new ArgumentOutOfRangeException(nameof(resourceId));
        if (generation <= 0) throw new ArgumentOutOfRangeException(nameof(generation));
        ArgumentException.ThrowIfNullOrWhiteSpace(kind);
        ArgumentNullException.ThrowIfNull(descriptorJson);
        ArgumentNullException.ThrowIfNull(bytes);
        ArgumentNullException.ThrowIfNull(callback);
        var key = (resourceId, generation);
        if (!Resources.TryAdd(key, callback))
            throw new InvalidOperationException(
                $"CanvasKit resource {resourceId}/{generation} is already registering.");
        try
        {
            RegisterResourceCore(resourceId, generation, kind, descriptorJson, bytes.AsSpan());
        }
        catch
        {
            Resources.TryRemove(key, out _);
            throw;
        }
    }

    internal static void ReleaseResource(long resourceId, long generation)
    {
        Resources.TryRemove((resourceId, generation), out _);
        ReleaseResourceCore(resourceId, generation);
    }

    internal static void ForgetScene(long sceneSequence) => Scenes.TryRemove(sceneSequence, out _);

    [JSExport]
    internal static void CompleteScene(
        [JSMarshalAs<JSType.Number>] long sceneSequence,
        string terminal,
        string reason,
        string receiptJson)
    {
        if (Scenes.TryRemove(sceneSequence, out var callback))
            callback.CompleteScene(sceneSequence, terminal, reason, receiptJson);
    }

    [JSExport]
    internal static void CompleteResource(
        [JSMarshalAs<JSType.Number>] long resourceId,
        [JSMarshalAs<JSType.Number>] long generation,
        string terminal,
        string reason,
        string receiptJson)
    {
        if (Resources.TryRemove((resourceId, generation), out var callback))
            callback.CompleteResource(resourceId, generation, terminal, reason, receiptJson);
    }
}
