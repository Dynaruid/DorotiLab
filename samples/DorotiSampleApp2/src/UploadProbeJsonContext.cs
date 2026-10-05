using System.Text.Json.Serialization;

namespace DorotiSampleApp2;

internal sealed record UploadPreviewProbe(
    [property: JsonPropertyName("Name")] string Name,
    [property: JsonPropertyName("Length")] long Length,
    [property: JsonPropertyName("Text")] string? Text);

internal sealed record UploadProbeResult(string Status, string? PickerStatus,
    UploadPreviewProbe[] Previews, bool ReadGrantsDisposed, string Message);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(UploadProbeResult))]
internal sealed partial class UploadProbeJsonContext : JsonSerializerContext;
