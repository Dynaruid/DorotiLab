using System.Text;
using Doroti.Flutter.Runtime;
using Doroti.Generated.Framework.Scheduler;
using Doroti.Generated.Framework.Services;

var codec = new StandardMethodCodec(new StandardMessageCodec());
var encoded = codec.encodeMethodCall(new("echo", "consumer"));
var decoded = codec.decodeMethodCall(encoded);
if (decoded.method != "echo" || decoded.arguments as string != "consumer")
{
    throw new InvalidOperationException("Method codec package round trip failed.");
}

var bundle = new ConsumerAssetBundle(new Dictionary<string, ReadOnlyMemory<byte>>
{
    ["consumer.txt"] = Encoding.UTF8.GetBytes("package"),
});
if (await bundle.loadString("consumer.txt") != "package")
{
    throw new InvalidOperationException("AssetBundle package behavior failed.");
}
if (Priority.animation.value != 100000L)
{
    throw new InvalidOperationException("Scheduler package API was not transitively available.");
}

Console.WriteLine("G4-3-SCHEDULER-SERVICES-PACKAGE-CONSUMER-PASS");

sealed class ConsumerAssetBundle(IReadOnlyDictionary<string, ReadOnlyMemory<byte>> assets) : CachingAssetBundle
{
    public override Future<ByteData> load(string key) => assets.TryGetValue(key, out var data)
        ? Future<ByteData>.value((ByteData)data)
        : Future<ByteData>.error(new FileNotFoundException($"Asset not found: {key}", key));
}
