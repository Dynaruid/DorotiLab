// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/image_resolution.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
        AssetBundle chosenBundle__11235 = ((this.bundle ?? ((ImageConfiguration)configuration).bundle) ?? global::Doroti.Framework.Services.Asset_bundleLibrary.rootBundle);
        Completer<AssetBundleImageKey>? completer__11332 = default!;
        Future<AssetBundleImageKey>? result__11376 = default!;
        _ = AssetManifest.loadFromAssetBundle(chosenBundle__11235).then((Action<AssetManifest>)((manifest) =>
        {
            IEnumerable<AssetMetadata>? candidateVariants__11519 = manifest.getAssetVariants(this.keyName);
            AssetMetadata chosenVariant__11605 = _chooseVariant(this.keyName, configuration, candidateVariants__11519);
            var key__11745 = new AssetBundleImageKey(bundle: chosenBundle__11235, name: chosenVariant__11605.key, scale: (chosenVariant__11605.targetDevicePixelRatio ?? _naturalResolution));
            if ((completer__11332 is not null))
            {
                completer__11332.complete(key__11745);
            }
            else
            {
                result__11376 = new SynchronousFuture<AssetBundleImageKey>(key__11745);
            }
        })).onError(((error, stack) =>
        {
            DartRuntimePrimitives.Assert(() => (completer__11332 is not null));
            DartRuntimePrimitives.Assert(() => (result__11376 is null));
            completer__11332!.completeError(error, stack);
        }));
        if ((result__11376 is not null))
        {
            return result__11376!;
        }
        completer__11332 = new Completer<AssetBundleImageKey>();
        return completer__11332.future;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual AssetMetadata _chooseVariant(string mainAssetKey, ImageConfiguration config, IEnumerable<AssetMetadata>? candidateVariants)
    {
        if ((((candidateVariants is null) || (candidateVariants.Count() == 0)) || (((ImageConfiguration)config).devicePixelRatio is null)))
        {
            return new AssetMetadata(key: mainAssetKey, targetDevicePixelRatio: null, main: true);
        }
        var candidatesByDevicePixelRatio__13645 = new SortedDictionary<double, AssetMetadata>();
        foreach (AssetMetadata candidate__13744 in candidateVariants)
        {
            candidatesByDevicePixelRatio__13645[(candidate__13744.targetDevicePixelRatio ?? _naturalResolution)] = candidate__13744;
        }
        return _findBestVariant(candidatesByDevicePixelRatio__13645, DartRuntimePrimitives.RequireValue(((ImageConfiguration)config).devicePixelRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual AssetMetadata _findBestVariant(SortedDictionary<double, AssetMetadata> candidatesByDpr, double value)
    {
        if (candidatesByDpr.ContainsKey(value))
        {
            return candidatesByDpr.GetValueOrDefault(value)!;
        }
        double? lower__14996 = candidatesByDpr.lastKeyBefore(value);
        double? upper__15060 = candidatesByDpr.firstKeyAfter(value);
        if ((lower__14996 is null))
        {
            return candidatesByDpr.GetValueOrDefault(DartRuntimePrimitives.RequireValue(upper__15060))!;
        }
        if ((upper__15060 is null))
        {
            return candidatesByDpr.GetValueOrDefault(DartRuntimePrimitives.RequireValue(lower__14996))!;
        }
        if (((value < Image_resolutionLibrary._kLowDprLimit) || (value > (((DartRuntimePrimitives.RequireValue(lower__14996) + DartRuntimePrimitives.RequireValue(upper__15060))) / 2L))))
        {
            return candidatesByDpr.GetValueOrDefault(DartRuntimePrimitives.RequireValue(upper__15060))!;
        }
        else
        {
            return candidatesByDpr.GetValueOrDefault(DartRuntimePrimitives.RequireValue(lower__14996))!;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as AssetImage;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is AssetImage) && (((AssetImage)((AssetImage)__other)).keyName == this.keyName)) && (object.Equals(((AssetImage)((AssetImage)__other)).bundle, this.bundle)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.keyName, this.bundle);
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "AssetImage"))}(bundle: {this.bundle}, name: \"{this.keyName}\")";
}

