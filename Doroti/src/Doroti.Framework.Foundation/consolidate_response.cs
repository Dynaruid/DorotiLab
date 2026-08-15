// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/consolidate_response.dart
namespace Doroti.Generated.Framework.Foundation;

public delegate void BytesReceivedCallback(int cumulative, int? total);

public static class ConsolidateResponseLibrary
{
    public static async Task<byte[]> consolidateHttpClientResponseBytes(
        IAsyncEnumerable<ReadOnlyMemory<byte>> response,
        int? contentLength = null,
        BytesReceivedCallback? onBytesReceived = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        var output = new _OutputBuffer(contentLength is > 0 ? contentLength.Value : 0);
        await foreach (var chunk in response.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            output.add(chunk.Span);
            onBytesReceived?.Invoke(output.length, contentLength);
        }
        if (contentLength is >= 0 && output.length != contentLength.Value)
        {
            throw new InvalidDataException($"Content size below specified contentLength. Expected {contentLength}, but received {output.length} bytes.");
        }
        return output.takeBytes();
    }
}

internal sealed class _OutputBuffer
{
    private readonly MemoryStream _stream;
    internal _OutputBuffer(int capacity) => _stream = new MemoryStream(capacity);
    internal int length => checked((int)_stream.Length);
    internal void add(ReadOnlySpan<byte> bytes) => _stream.Write(bytes);
    internal byte[] takeBytes() => _stream.ToArray();
}
