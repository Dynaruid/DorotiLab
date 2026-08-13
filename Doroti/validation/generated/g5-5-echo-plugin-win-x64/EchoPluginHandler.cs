using System.Text;
using Doroti.Flutter.Hosting;

namespace Doroti.Plugin.G55Echo.WinX64;

public sealed class EchoPluginHandler : IFlutterNativePluginHandler
{
    public string PluginId => "g55.echo";

    public string AbiVersion => "doroti.plugin-abi/v1";

    public ValueTask<ReadOnlyMemory<byte>?> HandleAsync(
        string channel,
        string codec,
        ReadOnlyMemory<byte>? message,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (channel != "g55/echo" || codec != "StandardMethodCodec")
            throw new InvalidOperationException("G5-5 echo plugin descriptor drifted.");
        var prefix = Encoding.UTF8.GetBytes("win-x64:");
        var input = message?.ToArray() ?? [];
        var output = new byte[prefix.Length + input.Length];
        prefix.CopyTo(output, 0);
        input.CopyTo(output, prefix.Length);
        return ValueTask.FromResult<ReadOnlyMemory<byte>?>(output);
    }
}
