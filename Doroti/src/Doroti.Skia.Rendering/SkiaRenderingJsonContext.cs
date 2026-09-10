using System.Text.Json.Serialization;

namespace Doroti.Skia.Rendering;

[JsonSerializable(typeof(IReadOnlyList<string>))]
internal sealed partial class SkiaRenderingJsonContext : JsonSerializerContext;
