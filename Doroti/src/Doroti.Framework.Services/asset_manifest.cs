// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/asset_manifest.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public static partial class Asset_manifestLibrary
{
    internal static string _kAssetManifestFilename = "AssetManifest.bin";
}

public static partial class Asset_manifestLibrary
{
    internal static string _kAssetManifestWebFilename = "AssetManifest.bin.json";
}

public interface AssetManifest
{
    public static Future<AssetManifest> loadFromAssetBundle(AssetBundle bundle)
    {
        if (ConstantsLibrary.kIsWeb)
        {
            return bundle.loadStructuredData<AssetManifest>(Asset_manifestLibrary._kAssetManifestWebFilename, (jsonData) =>
            {
                var message = new ByteData(Dart_convertLibrary.base64.decode(((string?)Dart_convertLibrary.json.decode(jsonData))!));
                return _AssetManifestBin.CreateFromStandardMessageCodecMessage(message);
            });
        }
        return bundle.loadStructuredBinaryData<AssetManifest>(Asset_manifestLibrary._kAssetManifestFilename, (arg0) => _AssetManifestBin.CreateFromStandardMessageCodecMessage(arg0));
    }
    public List<string> listAssets();
    public List<AssetMetadata>? getAssetVariants(string key);
}

internal class _AssetManifestBin : AssetManifest
{
    internal virtual DartMap<object?, object?> _data { get; private set; } = default!;
    internal virtual DartMap<string, List<AssetMetadata>> _typeCastedData { get; private set; } = new DartMap<string, List<AssetMetadata>>();

    internal _AssetManifestBin(DartMap<object?, object?> standardMessageData)
    {
        _data = standardMessageData;
    }

    internal static _AssetManifestBin CreateFromStandardMessageCodecMessage(ByteData message)
    {
        var data = new StandardMessageCodec().decodeMessage(message);
        if (data is not System.Collections.IDictionary entries)
            throw new FormatException("The asset manifest must decode to a map.");
        return new _AssetManifestBin(DartRuntimePrimitives.ConvertMap<object?, object?>(entries));
    }

    public virtual List<AssetMetadata>? getAssetVariants(string key)
    {
        if (!_typeCastedData.ContainsKey(key))
        {
            object? variantData = _data.GetValueOrDefault(key);
            if (variantData is null)
            {
                return null;
            }
            _typeCastedData[key] = ((IEnumerable<object?>?)(_data.GetValueOrDefault(key) ?? new List<object?>()))!.cast<DartMap<object?, object?>>().map((data) =>
            {
                var asset = ((string?)data.GetValueOrDefault("asset")!)!;
                object? dpr = data.GetValueOrDefault("dpr");
                return new AssetMetadata(key: ((string?)data.GetValueOrDefault("asset")!)!, targetDevicePixelRatio: (double?)dpr, main: key == asset);
            }).ToList();
            _data.remove(key);
        }
        return _typeCastedData.GetValueOrDefault(key)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<string> listAssets()
    {
        return new List<string>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AssetMetadata
{
    public virtual double? targetDevicePixelRatio { get; private set; }
    public virtual string key { get; private set; } = default!;
    public virtual bool main { get; private set; } = default!;

    public AssetMetadata(string key, double? targetDevicePixelRatio, bool main)
    {
        this.key = key;
        this.targetDevicePixelRatio = targetDevicePixelRatio;
        this.main = main;
    }

}
