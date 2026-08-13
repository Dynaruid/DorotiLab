#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/asset_manifest.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

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
        if (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb)
        {
            return bundle.loadStructuredData<AssetManifest>(Asset_manifestLibrary._kAssetManifestWebFilename, ((jsonData) =>
            {
                var message = new ByteData(Dart_convertLibrary.base64.decode(((string?)Dart_convertLibrary.json.decode(jsonData))!));
                return _AssetManifestBin.CreateFromStandardMessageCodecMessage(message);
            }));
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
        this._data = standardMessageData;
    }

    internal static _AssetManifestBin CreateFromStandardMessageCodecMessage(ByteData message)
    {
        object data = new StandardMessageCodec().decodeMessage(message);
        return new _AssetManifestBin(DartRuntimePrimitives.ConvertMap<object?, object?>((System.Collections.IDictionary)data));
    }

    public virtual List<AssetMetadata>? getAssetVariants(string key)
    {
        if (!_typeCastedData.ContainsKey(key))
        {
            object? variantData__4334 = _data.GetValueOrDefault(key);
            if ((variantData__4334 is null))
            {
                return null;
            }
            _typeCastedData[key] = (((IEnumerable<object?>?)((_data.GetValueOrDefault(key) ?? new List<object?>())))!).cast<DartMap<object?, object?>>().map(((data) =>
            {
                var asset = ((string?)data.GetValueOrDefault("asset")!)!;
                object? dpr = data.GetValueOrDefault("dpr");
                return new AssetMetadata(key: ((string?)data.GetValueOrDefault("asset")!)!, targetDevicePixelRatio: ((double?)dpr), main: (key == asset));
            })).ToList();
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

