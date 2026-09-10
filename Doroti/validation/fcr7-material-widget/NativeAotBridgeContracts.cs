using Doroti.Framework.Painting;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;

internal static class NativeAotBridgeContracts
{
    public static void Verify()
    {
        FocusGeometryContract.Verify();
        var owner = new ScrollableState { _bucket = new RestorationBucket("external", null) };
        var property = new ExternalProperty();
        owner.registerForRestoration(property, "value");
        Require(property.value == 7L && ReferenceEquals(property.state, owner), "default and IState owner");
        Require(owner.bucket!.read<long>("value") == 7L, "default serialized into bucket");
        property.value = 42;
        Require(owner.bucket.read<long>("value") == 42L, "change listener updates bucket");
        property.Enabled = false;
        property.notifyListeners();
        Require(!owner.bucket.contains("value"), "disabled property removed from bucket");
        property.Enabled = true;
        property.notifyListeners();
        property.dispose();
        Require(owner._properties.Count == 0 && property._owner is null, "dispose detaches property and listener");
        var restored = new ExternalProperty();
        owner.registerForRestoration(restored, "value");
        Require(restored.value == 42L, "serialized value survives property replacement");
        owner.unregisterFromRestoration(restored);
        Require(!owner.bucket.contains("value") && owner._properties.Count == 0, "explicit unregister removes serialized value");
        restored.dispose();
        owner._bucket = null;
        var withoutBucket = new ExternalProperty();
        owner.registerForRestoration(withoutBucket, "unpersisted");
        Require(withoutBucket.value == 7, "absent bucket still initializes default");
        withoutBucket.dispose();

        var provider = new MemoryImage(new Uint8List(new byte[] { 1, 2, 3 }));
        var memory = FadeInImage.CreateMemoryNetwork(placeholder: provider.bytes, image: "https://example.invalid/image", placeholderCacheWidth: 11, imageCacheHeight: 23);
        Require(memory.placeholder is ResizeImage { width: 11, imageProvider: MemoryImage }, "memory factory wraps placeholder before construction");
        Require(memory.image is ResizeImage { height: 23, imageProvider: NetworkImageIo }, "network factory wraps image before construction");
        var asset = FadeInImage.CreateAssetNetwork(placeholder: "placeholder.png", image: "https://example.invalid/image", placeholderScale: 2);
        Require(asset.placeholder is ExactAssetImage { scale: 2 }, "asset factory retains explicit scale");
        Require(ReferenceEquals(new DecorationImage(provider).image, provider), "decoration retains provider identity");
        Console.WriteLine("NativeAOT bridges: actual ScrollableState registration/bucket/listener/dispose and image factories PASS");
    }
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private sealed class ExternalProperty : RestorableValue<long>
    {
        public bool Enabled = true;
        public override bool enabled => Enabled;
        public override long createDefaultValue() => 7;
        public override long fromPrimitives(object? value) => (long)value!;
        public override object toPrimitives() => value;
        public override void didUpdateValue(long oldValue) => notifyListeners();
    }
}
