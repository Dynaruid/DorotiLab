using System.IO.Compression;
using System.Security.Cryptography;
using Doroti.Skia.Vulkan;

var directory = Path.Combine(Path.GetTempPath(), "doroti-apk-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(directory);
try
{
    const string arm64 = "lib/arm64-v8a/libSkiaSharp.so";
    const string x64 = "lib/x86_64/libSkiaSharp.so";
    byte[] official = [1, 2, 3, 4]; // Fixture identity, never a product package pin.
    var hash = Convert.ToHexString(SHA256.HashData(official));
    var empty = Apk("base.apk", []);
    var standalone = Apk("standalone.apk", [(arm64, official), (x64, new byte[] { 9 })]);
    var split = Apk("split_config.arm64_v8a.apk", [(arm64, official)]);
    var resources = Apk("split_resources.apk", [("res/raw/example", official)]);
    var otherAbi = Apk("split_config.x86_64.apk", [(x64, official)]);
    var corrupt = Apk("corrupt.apk", [(arm64, new byte[] { 9 })]);
    var duplicate = Apk("duplicate.apk", [(arm64, official), (arm64, official)]);
    var passed = 0;

    Check("standalone ignores other ABI", Verify(standalone, null) == standalone);
    Check("native ABI split among resource splits", Verify(empty, [resources, split, otherAbi]) == split);
    Check("base entry with resource-only splits", Verify(standalone, [resources]) == standalone);
    Check("repeated installed path is one archive", Verify(empty, [split, split]) == split);
    Reject("missing current ABI", () => Verify(empty, [otherAbi]));
    Reject("duplicate across base and split", () => Verify(standalone, [split]));
    Reject("duplicate within archive", () => Verify(duplicate, null));
    Reject("incorrect package hash", () => Verify(empty, [corrupt]));
    Check("direct split load", AndroidGraphiteApkAsset.IsVerifiedLoadPath(split + "!/" + arm64, split, "arm64-v8a", null));
    Check("wrong APK load rejected", !AndroidGraphiteApkAsset.IsVerifiedLoadPath(empty + "!/" + arm64, split, "arm64-v8a", null));
    Check("wrong ABI load rejected", !AndroidGraphiteApkAsset.IsVerifiedLoadPath(split + "!/" + x64, split, "arm64-v8a", null));
    var extracted = Path.Combine(directory, "libSkiaSharp.so");
    Check("verified extracted load", AndroidGraphiteApkAsset.IsVerifiedLoadPath(extracted, split, "arm64-v8a", extracted));
    Check("unverified extracted load rejected", !AndroidGraphiteApkAsset.IsVerifiedLoadPath(extracted, split, "arm64-v8a", null));
    Console.WriteLine($"PASS: {passed} Android APK provenance cases (synthetic archives; no device/GPU claim).");

    string Verify(string path, string[]? splits) => AndroidGraphiteApkAsset.Verify(path, splits, "arm64-v8a", hash);
    void Check(string name, bool condition)
    {
        if (!condition) throw new InvalidOperationException(name);
        passed++;
    }
    void Reject(string name, Action action)
    {
        try { action(); }
        catch (InvalidDataException) { passed++; return; }
        throw new InvalidOperationException(name + " was accepted");
    }
    string Apk(string name, (string Name, byte[] Content)[] entries)
    {
        var path = Path.Combine(directory, name);
        using var archive = ZipFile.Open(path, ZipArchiveMode.Create);
        foreach (var entry in entries)
        {
            using var stream = archive.CreateEntry(entry.Name).Open();
            stream.Write(entry.Content);
        }
        return path;
    }
}
finally
{
    Directory.Delete(directory, recursive: true);
}
