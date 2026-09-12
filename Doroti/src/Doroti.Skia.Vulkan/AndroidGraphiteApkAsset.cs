using System.IO.Compression;
using System.Security.Cryptography;

namespace Doroti.Skia.Vulkan;

/// <summary>Checks the installed base/split APK set before loading an Android native asset.</summary>
internal static class AndroidGraphiteApkAsset
{
    internal static string Verify(string baseApkPath, IEnumerable<string>? splitApkPaths, string abi, string expectedHash)
    {
        var paths = new[] { baseApkPath }.Concat(splitApkPaths ?? []).Select(Path.GetFullPath).Distinct(StringComparer.Ordinal);
        var entryName = $"lib/{abi}/libSkiaSharp.so";
        string? containingApk = null;
        foreach (var path in paths)
        {
            using var apk = ZipFile.OpenRead(path);
            foreach (var entry in apk.Entries.Where(entry => entry.FullName == entryName))
            {
                if (containingApk is not null)
                    throw new InvalidDataException("Installed APKs contain more than one Skia library for this ABI.");
                using var stream = entry.Open();
                if (!Convert.ToHexString(SHA256.HashData(stream)).Equals(expectedHash, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("APK native Skia differs from the pinned official package: " + path);
                containingApk = path;
            }
        }
        return containingApk ?? throw new InvalidDataException("Installed APKs must contain exactly one official Skia library for this ABI.");
    }

    internal static bool IsVerifiedLoadPath(string loadedPath, string containingApk, string abi, string? verifiedExtractedPath) =>
        loadedPath == verifiedExtractedPath || loadedPath == containingApk + "!/lib/" + abi + "/libSkiaSharp.so";
}
