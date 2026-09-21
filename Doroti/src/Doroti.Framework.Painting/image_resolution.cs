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

    public virtual string keyName =>
        (package is null) ? assetName : $"packages/{package}/{assetName}";

    public override Future<AssetBundleImageKey> obtainKey(ImageConfiguration configuration)
    {
        AssetBundle chosenBundle =
            (bundle ?? configuration.bundle) ?? Asset_bundleLibrary.rootBundle;
        Completer<AssetBundleImageKey>? completer = default!;
        Future<AssetBundleImageKey>? result = default!;
        _ = AssetManifest
            .loadFromAssetBundle(chosenBundle)
            .then(
                (manifest) =>
                {
                    IEnumerable<AssetMetadata>? candidateVariants = manifest.getAssetVariants(
                        keyName
                    );
                    AssetMetadata chosenVariant = _chooseVariant(
                        keyName,
                        configuration,
                        candidateVariants
                    );
                    var keyLocal = new AssetBundleImageKey(
                        bundle: chosenBundle,
                        name: chosenVariant.key,
                        scale: chosenVariant.targetDevicePixelRatio ?? _naturalResolution
                    );
                    if (completer is not null)
                    {
                        completer.complete(keyLocal);
                    }
                    else
                    {
                        result = new SynchronousFuture<AssetBundleImageKey>(keyLocal);
                    }
                }
            )
            .onError(
                (error, stack) =>
                {
                    DartRuntimePrimitives.Assert(() => completer is not null);
                    DartRuntimePrimitives.Assert(() => result is null);
                    completer!.completeError(error, stack);
                }
            );
        if (result is not null)
        {
            return result!;
        }
        completer = new Completer<AssetBundleImageKey>();
        return completer.future;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual AssetMetadata _chooseVariant(
        string mainAssetKey,
        ImageConfiguration config,
        IEnumerable<AssetMetadata>? candidateVariants
    )
    {
        if (
            (candidateVariants is null)
            || (candidateVariants.Count() == 0)
            || (config.devicePixelRatio is null)
        )
        {
            return new AssetMetadata(key: mainAssetKey, targetDevicePixelRatio: null, main: true);
        }
        var candidatesByDevicePixelRatio = new SortedDictionary<double, AssetMetadata>();
        foreach (AssetMetadata candidate in candidateVariants)
        {
            candidatesByDevicePixelRatio[candidate.targetDevicePixelRatio ?? _naturalResolution] =
                candidate;
        }
        return _findBestVariant(
            candidatesByDevicePixelRatio,
            (
                config.devicePixelRatio
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual AssetMetadata _findBestVariant(
        SortedDictionary<double, AssetMetadata> candidatesByDpr,
        double value
    )
    {
        if (candidatesByDpr.ContainsKey(value))
        {
            return candidatesByDpr.GetValueOrDefault(value)!;
        }
        double? lower = candidatesByDpr.lastKeyBefore(value);
        double? upper = candidatesByDpr.firstKeyAfter(value);
        if (lower is null)
        {
            return candidatesByDpr.GetValueOrDefault(
                (
                    upper
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            )!;
        }
        if (upper is null)
        {
            return candidatesByDpr.GetValueOrDefault(
                (
                    lower
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            )!;
        }
        if (
            (value < Image_resolutionLibrary._kLowDprLimit)
            || (
                value
                > (
                    (
                        (
                            lower
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        )
                        + (
                            upper
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        )
                    ) / 2L
                )
            )
        )
        {
            return candidatesByDpr.GetValueOrDefault(
                (
                    upper
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            )!;
        }
        else
        {
            return candidatesByDpr.GetValueOrDefault(
                (
                    lower
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            )!;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as AssetImage;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is AssetImage)
            && (__other.keyName == keyName)
            && Equals(__other.bundle, bundle);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(keyName, bundle);

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "AssetImage")}(bundle: {bundle}, name: \"{keyName}\")";
}
