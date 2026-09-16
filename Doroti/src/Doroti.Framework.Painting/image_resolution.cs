// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/image_resolution.dart
using Doroti.Runtime;

namespace Doroti.Framework.Painting;

public static partial class Image_resolutionLibrary
{
    internal static double _kLowDprLimit = 2.0;
}

public class AssetImage : AssetBundleImageProvider
{
    public virtual string assetName { get; private set; } = default!;
    public virtual AssetBundle? bundle { get; private set; }
    public virtual string? package { get; private set; }
    internal const double _naturalResolution = 1.0;

    public AssetImage(string assetName, AssetBundle? bundle = null, string? package = null)
    {
        this.assetName = assetName;
        this.bundle = bundle;
        this.package = package;
    }

    public virtual string keyName => ((this.package is null) ? this.assetName : $"packages/{this.package}/{this.assetName}");
    public override Future<AssetBundleImageKey> obtainKey(ImageConfiguration configuration)
    {
        AssetBundle chosenBundle = ((this.bundle ?? ((ImageConfiguration)configuration).bundle) ?? Asset_bundleLibrary.rootBundle);
        Completer<AssetBundleImageKey>? completer = default!;
        Future<AssetBundleImageKey>? result = default!;
        _ = AssetManifest.loadFromAssetBundle(chosenBundle).then((Action<AssetManifest>)((manifest) =>
        {
            IEnumerable<AssetMetadata>? candidateVariants = manifest.getAssetVariants(this.keyName);
            AssetMetadata chosenVariant = _chooseVariant(this.keyName, configuration, candidateVariants);
            var keyLocal = new AssetBundleImageKey(bundle: chosenBundle, name: chosenVariant.key, scale: (chosenVariant.targetDevicePixelRatio ?? _naturalResolution));
            if ((completer is not null))
            {
                completer.complete(keyLocal);
            }
            else
            {
                result = new SynchronousFuture<AssetBundleImageKey>(keyLocal);
            }
        })).onError(((error, stack) =>
        {
            DartRuntimePrimitives.Assert(() => (completer is not null));
            DartRuntimePrimitives.Assert(() => (result is null));
            completer!.completeError(error, stack);
        }));
        if ((result is not null))
        {
            return result!;
        }
        completer = new Completer<AssetBundleImageKey>();
        return completer.future;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual AssetMetadata _chooseVariant(string mainAssetKey, ImageConfiguration config, IEnumerable<AssetMetadata>? candidateVariants)
    {
        if ((((candidateVariants is null) || (candidateVariants.Count() == 0)) || (((ImageConfiguration)config).devicePixelRatio is null)))
        {
            return new AssetMetadata(key: mainAssetKey, targetDevicePixelRatio: null, main: true);
        }
        var candidatesByDevicePixelRatio = new SortedDictionary<double, AssetMetadata>();
        foreach (AssetMetadata candidate in candidateVariants)
        {
            candidatesByDevicePixelRatio[(candidate.targetDevicePixelRatio ?? _naturalResolution)] = candidate;
        }
        return _findBestVariant(candidatesByDevicePixelRatio, DartRuntimePrimitives.RequireValue(((ImageConfiguration)config).devicePixelRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual AssetMetadata _findBestVariant(SortedDictionary<double, AssetMetadata> candidatesByDpr, double value)
    {
        if (candidatesByDpr.ContainsKey(value))
        {
            return candidatesByDpr.GetValueOrDefault(value)!;
        }
        double? lower = candidatesByDpr.lastKeyBefore(value);
        double? upper = candidatesByDpr.firstKeyAfter(value);
        if ((lower is null))
        {
            return candidatesByDpr.GetValueOrDefault(DartRuntimePrimitives.RequireValue(upper))!;
        }
        if ((upper is null))
        {
            return candidatesByDpr.GetValueOrDefault(DartRuntimePrimitives.RequireValue(lower))!;
        }
        if (((value < Image_resolutionLibrary._kLowDprLimit) || (value > (((DartRuntimePrimitives.RequireValue(lower) + DartRuntimePrimitives.RequireValue(upper))) / 2L))))
        {
            return candidatesByDpr.GetValueOrDefault(DartRuntimePrimitives.RequireValue(upper))!;
        }
        else
        {
            return candidatesByDpr.GetValueOrDefault(DartRuntimePrimitives.RequireValue(lower))!;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as AssetImage;
        if (__other is null) return false;
        if ((!Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is AssetImage) && (((AssetImage)((AssetImage)__other)).keyName == this.keyName)) && (Equals(((AssetImage)((AssetImage)__other)).bundle, this.bundle)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.keyName, this.bundle);
    public override string ToString() => $"{(objectRuntimeTypeFunctions.objectRuntimeType(this, "AssetImage"))}(bundle: {this.bundle}, name: \"{this.keyName}\")";
}

