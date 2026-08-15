using System.Text;
using Doroti.Hosting;

namespace Doroti.Plugin.G6GeneratedDemoEcho.WinX64;

public sealed class EchoPluginHandler : IDorotiNativePluginHandler
{
    public string PluginId => "g6.generated-demo.echo";

    public string AbiVersion => "doroti.plugin-abi/v1";

    public ValueTask<ReadOnlyMemory<byte>?> HandleAsync(
        string channel,
        string codec,
        ReadOnlyMemory<byte>? message,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (channel != "g6/generated-demo/echo" || codec != "StandardMethodCodec")
            throw new InvalidOperationException("G6-7 generated DemoApp echo plugin descriptor drifted.");
        var prefix = Encoding.UTF8.GetBytes("win-x64:");
        var input = message?.ToArray() ?? [];
        var output = new byte[prefix.Length + input.Length];
        prefix.CopyTo(output, 0);
        input.CopyTo(output, prefix.Length);
        return ValueTask.FromResult<ReadOnlyMemory<byte>?>(output);
    }
}
