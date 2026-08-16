// <doroti-reviewed-framework-source />
using Doroti.Runtime;

namespace Doroti.Framework.Foundation;

/// <summary>Source-library spelling used by deterministic cross-library qualification.</summary>
public static class Consolidate_responseLibrary
{
    public static Future<Uint8List> consolidateHttpClientResponseBytes(HttpClientResponse response) =>
        Future<Uint8List>.fromTask(ReadAsync(response));

    public static Future<Uint8List> consolidateHttpClientResponseBytes(
        HttpClientResponse response,
        Action<long, long?>? onBytesReceived = null) =>
        Future<Uint8List>.fromTask(ReadAsync(response, onBytesReceived));

    private static async Task<Uint8List> ReadAsync(HttpClientResponse response)
    {
        var bytes = await ConsolidateResponseLibrary.consolidateHttpClientResponseBytes(response);
        return new Uint8List(bytes);
    }

    private static async Task<Uint8List> ReadAsync(HttpClientResponse response, Action<long, long?>? onBytesReceived)
    {
        var bytes = await ConsolidateResponseLibrary.consolidateHttpClientResponseBytes(response);
        onBytesReceived?.Invoke(bytes.LongLength, bytes.LongLength);
        return new Uint8List(bytes);
    }
}
