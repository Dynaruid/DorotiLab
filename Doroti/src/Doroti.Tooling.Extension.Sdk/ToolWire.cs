using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Tooling.Contracts;

namespace Doroti.Tooling.Extension.Sdk;

// JSON types live at the process/file boundary. Decimal string identifiers never pass through JS doubles.
public sealed record WireError(string Code, string Message);
public sealed record WireArguments(ToolContext? Context = null, ConfigurationRequest? Configuration = null, TemplateRequest? Template = null, OperationRequest? Operation = null, string? RequestId = null);
public sealed record WireResult(ToolIdentity? Identity = null, ToolConfiguration? Configuration = null, ConfigurationResult? Configured = null, DiagnosticResult? Diagnostics = null, DeviceResult? Devices = null, TemplateResult? Templates = null, TemplateGeneration? Generated = null, ExecutionPlan? Plan = null);
public sealed record WireRequest(string Jsonrpc, string? Id, string Method, WireArguments Params);
public sealed record WireResponse(string Jsonrpc, string Id, WireResult? Result, WireError? Error);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, UseStringEnumConverter = true, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow)]
[JsonSerializable(typeof(WireRequest))]
[JsonSerializable(typeof(WireResponse))]
[JsonSerializable(typeof(ProviderDescriptor))]
[JsonSerializable(typeof(WorkspaceDescriptor))]
[JsonSerializable(typeof(ExecutionPlan))]
public partial class ToolJsonContext : JsonSerializerContext;

public static class ToolFrames
{
    public const int MaximumPayload = 4 * 1024 * 1024;
    public static async ValueTask<byte[]?> ReadAsync(Stream stream, CancellationToken token)
    {
        var header = new List<byte>();
        var single = new byte[1];
        while (true)
        {
            if (await stream.ReadAsync(single, token) == 0)
            {
                if (header.Count == 0) return null;
                throw new EndOfStreamException("Truncated tool frame header.");
            }
            header.Add(single[0]);
            if (header.Count > 8192) throw new InvalidDataException("Tool frame header exceeds the limit.");
            if (header.Count >= 4 && header[^4] == 13 && header[^3] == 10 && header[^2] == 13 && header[^1] == 10) break;
        }
        var lines = System.Text.Encoding.ASCII.GetString(header.ToArray()).Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length != 1 || !lines[0].StartsWith("Content-Length: ", StringComparison.Ordinal) || !int.TryParse(lines[0][16..], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var length) || length <= 0 || length > MaximumPayload)
            throw new InvalidDataException("Invalid Content-Length tool frame.");
        var payload = new byte[length];
        await stream.ReadExactlyAsync(payload, token);
        return payload;
    }
    public static async ValueTask WriteAsync(Stream stream, ReadOnlyMemory<byte> payload, SemaphoreSlim writer, CancellationToken token)
    {
        if (payload.Length is <= 0 or > MaximumPayload) throw new InvalidDataException("Tool frame payload exceeds the limit.");
        await writer.WaitAsync(token);
        try
        {
            await stream.WriteAsync(System.Text.Encoding.ASCII.GetBytes($"Content-Length: {payload.Length}\r\n\r\n"), token);
            await stream.WriteAsync(payload, token);
            await stream.FlushAsync(token);
        }
        finally { writer.Release(); }
    }
    public static void ValidateJson(ReadOnlySpan<byte> payload)
    {
        using var doc = JsonDocument.Parse(payload.ToArray());
        void Visit(JsonElement node)
        {
            if (node.ValueKind == JsonValueKind.Object)
            {
                var names = new HashSet<string>(StringComparer.Ordinal);
                foreach (var property in node.EnumerateObject())
                {
                    if (!names.Add(property.Name)) throw new InvalidDataException("Duplicate JSON property.");
                    Visit(property.Value);
                }
            }
            else if (node.ValueKind == JsonValueKind.Array) foreach (var value in node.EnumerateArray()) Visit(value);
        }
        Visit(doc.RootElement);
    }
}


/// <summary>Source generated metadata plus strict external enum and decimal Int64 converters.</summary>
public static class ToolWireJson
{
    private sealed class Int64StringConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            var text = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
            if (text is null || !long.TryParse(text, System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out var value) || text != value.ToString(System.Globalization.CultureInfo.InvariantCulture))
                throw new JsonException("Int64 wire values must use decimal strings.");
            return value;
        }
        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }
    public static ToolJsonContext Context { get; } = Create();
    private static ToolJsonContext Create()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            RespectRequiredConstructorParameters = true,
            Converters = { new Int64StringConverter(), new JsonStringEnumConverter<ToolMode>(allowIntegerValues: false), new JsonStringEnumConverter<ToolService>(allowIntegerValues: false), new JsonStringEnumConverter<OptionKind>(allowIntegerValues: false), new JsonStringEnumConverter<DiagnosticStatus>(allowIntegerValues: false) },
        };
        return new(options);
    }
}
