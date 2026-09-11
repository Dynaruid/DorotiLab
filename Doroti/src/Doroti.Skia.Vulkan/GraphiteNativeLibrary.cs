using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Diagnostics;
using SkiaSharp;

namespace Doroti.Skia.Vulkan;

/// <summary>Build/publish provenance for one official NuGet native asset.</summary>
public sealed record OfficialGraphiteAsset(string NativePath, string PackageId, string Version, string Rid, string Sha256, string ManagedSha256);

/// <summary>Select one native asset for the process before creating any Skia object.</summary>
public static unsafe class GraphiteNativeLibrary
{
    private static readonly object Gate = new();
    private static nint _module;
    private static OfficialGraphiteAsset? _official;
    private static bool _officialAndroid;
    public static bool IsOfficialSelected => _official is not null || _officialAndroid;
    [StructLayout(LayoutKind.Sequential)]
    private struct DlInfo { public nint FileName, Base, Symbol, Address; }
    [DllImport("libdl.so", EntryPoint = "dladdr")]
    private static extern int DlAddress(nint address, out DlInfo info);

    /// <summary>Version-pinned desktop package deployment; no custom staging directory.</summary>
    public static OfficialGraphiteAsset PackagedOfficialAsset()
    {
        var windows = OperatingSystem.IsWindows();
        if (!windows && !OperatingSystem.IsLinux()) throw new PlatformNotSupportedException();
        var managedPath = typeof(SKGraphiteContext).Assembly.Location;
        if (string.IsNullOrEmpty(managedPath)) throw new PlatformNotSupportedException("Desktop single-file/AOT needs linked-asset provenance.");
        using var managed = File.OpenRead(managedPath);
        var managedHash = Convert.ToHexString(SHA256.HashData(managed)).ToLowerInvariant();
        if (managedHash != "7c8cdb451146fcb12899899286e279e6aa615a5fc9b610f01137a741819f210f" &&
            (!windows || managedHash != "702657b10552a9aa75dd65ca63fac752e0bb795a9cab3fb169ff52cb2ecee7b9"))
            throw new InvalidDataException("Managed SkiaSharp differs from the pinned official desktop package entries.");
        return new(Path.Combine(AppContext.BaseDirectory, windows ? "libSkiaSharp.dll" : "libSkiaSharp.so"),
            windows ? "SkiaSharp.NativeAssets.Win32" : "SkiaSharp.NativeAssets.Linux", "4.154.0-preview.1.26454.9",
            windows ? "win-x64" : "linux-x64",
            windows ? "07ce51fd59e099b9561b0327223c27b21aa5605b5b8f4484dd297fdb8c8725a1" : "3a7778cce23720da6503e0ca845b423ad366cfe7e6a6b9ad6eea052fc0c91e92", managedHash);
    }

    /// <summary>Verify the final APK's native entry before normal Android/AOT binding.</summary>
    public static void ConfigureOfficialAndroid(string apkPath, string nativeLibraryDirectory)
    {
        if (!OperatingSystem.IsAndroid()) throw new PlatformNotSupportedException();
        lock (Gate)
        {
            if (_module != 0)
            {
                if (!_officialAndroid) throw new InvalidOperationException("Another Skia asset was already selected.");
                return;
            }
            var (abi, hash) = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.Arm64 => ("arm64-v8a", "63af1ec283b86965542bca1400ae446e6a179a7be5b6187e69aa5fa0bd49e180"),
                Architecture.X64 => ("x86_64", "9bb141c1ee6b40781044a4e13779c6f19a0b45554dc6030b5f1cf49b24f7e7b8"),
                _ => throw new PlatformNotSupportedException("Official Graphite supports Android arm64/x64.")
            };
            // Pins belong to SkiaSharp.NativeAssets.Android 4.154.0-preview.1.26454.9.
            // The final APK check also catches old host AARs overriding that package.
            using var apk = System.IO.Compression.ZipFile.OpenRead(apkPath);
            var entries = apk.Entries.Where(e => e.FullName == $"lib/{abi}/libSkiaSharp.so").ToArray();
            if (entries.Length != 1) throw new InvalidDataException("APK must contain exactly one official Skia library for this ABI.");
            using (var entry = entries[0].Open())
                if (!Convert.ToHexString(SHA256.HashData(entry)).Equals(hash, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("APK native Skia differs from the pinned official package.");
            var extracted = Path.Combine(nativeLibraryDirectory, "libSkiaSharp.so");
            if (File.Exists(extracted))
            {
                using var file = File.OpenRead(extracted);
                if (!Convert.ToHexString(SHA256.HashData(file)).Equals(hash, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Extracted Android library differs from APK provenance.");
            }
            var module = NativeLibrary.Load("libSkiaSharp.so", typeof(GraphiteNativeLibrary).Assembly, null);
            if (NativeLibrary.TryGetExport(module, "doroti_graphite_interop_version", out _))
                throw new InvalidDataException("Custom ABI library was loaded instead of the official APK asset.");
            var available = (delegate* unmanaged[Cdecl]<int, byte>)NativeLibrary.GetExport(module, "sk_graphite_backend_is_available");
            if (DlAddress((nint)available, out var location) == 0) throw new InvalidDataException("Cannot identify the loaded Android Skia module.");
            var loadedPath = Marshal.PtrToStringUTF8(location.FileName)!;
            if (loadedPath != extracted && loadedPath != apkPath + "!/lib/" + abi + "/libSkiaSharp.so")
                throw new InvalidDataException("Android loaded Skia outside the verified APK/native directory: " + loadedPath);
            if (available((int)SKGraphiteBackend.Vulkan) == 0) throw new PlatformNotSupportedException("Official Android asset lacks Graphite Vulkan.");
            NativeLibrary.SetDllImportResolver(typeof(SKGraphiteContext).Assembly,
                (library, _, _) => library is "libSkiaSharp" or "libSkiaSharp.so" ? module : 0);
            _module = module; _officialAndroid = true;
            Console.WriteLine($"DorotiGraphite official package=SkiaSharp.NativeAssets.Android version=4.154.0-preview.1.26454.9 abi={abi} sha256={hash} loaded={loadedPath}");
        }
    }

    public static OfficialGraphiteAsset ReadOfficialManifest(string manifestPath)
    {
        if (!Path.IsPathFullyQualified(manifestPath)) throw new InvalidDataException("Official native manifest requires an absolute path.");
        using var json = JsonDocument.Parse(File.ReadAllText(manifestPath));
        var root = json.RootElement;
        return new(root.GetProperty("nativePath").GetString()!, root.GetProperty("packageId").GetString()!,
            root.GetProperty("version").GetString()!, root.GetProperty("rid").GetString()!,
            root.GetProperty("sha256").GetString()!, root.GetProperty("managedSha256").GetString()!);
    }

    /// <summary>
    /// Explicit official-asset path for qualified desktop hosts. The supplied hash
    /// must come from the resolved, integrity-checked NuGet package at build/publish.
    /// This verifies the deployed file; it does not replace archive signature checks.
    /// Android AOT/static-link provenance requires its platform-specific package path.
    /// </summary>
    public static void ConfigureOfficial(OfficialGraphiteAsset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);
        if (!Path.IsPathFullyQualified(asset.NativePath)) throw new InvalidDataException("Official native asset requires an absolute path.");
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
            throw new PlatformNotSupportedException("This official native file loader is qualified for desktop dynamic libraries only.");
        var rid = OperatingSystem.IsWindows() ? "win-x64" : "linux-x64";
        var package = OperatingSystem.IsWindows() ? "SkiaSharp.NativeAssets.Win32" : "SkiaSharp.NativeAssets.Linux";
        if (RuntimeInformation.ProcessArchitecture != Architecture.X64 || asset.Rid != rid || !asset.PackageId.Equals(package, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Official Graphite package/RID does not match the process.");
        var version = typeof(SKGraphiteContext).Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
            .OfType<System.Reflection.AssemblyInformationalVersionAttribute>().Single().InformationalVersion.Split('+')[0];
        // Upstream assembly informational metadata omits the NuGet preview/build
        // suffix. Verify its numeric version and the exact managed file hash from
        // the same resolved package pair instead of pretending metadata is the full pin.
        if (asset.Version.Split('-')[0] != version) throw new InvalidDataException("Official Graphite managed/native base versions differ.");
        if (asset.Sha256.Length != 64 || !asset.Sha256.All(Uri.IsHexDigit)) throw new InvalidDataException("Official asset requires a SHA-256 provenance value.");
        if (asset.ManagedSha256.Length != 64 || !asset.ManagedSha256.All(Uri.IsHexDigit)) throw new InvalidDataException("Official asset requires managed package SHA-256 provenance.");
        var managedPath = typeof(SKGraphiteContext).Assembly.Location;
        if (string.IsNullOrEmpty(managedPath)) throw new PlatformNotSupportedException("Static/single-file managed provenance needs a separately qualified publish manifest.");
        using (var managedFile = File.OpenRead(managedPath))
            if (!Convert.ToHexString(SHA256.HashData(managedFile)).Equals(asset.ManagedSha256, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Loaded SkiaSharp managed assembly differs from NuGet provenance.");
        var path = Path.GetFullPath(asset.NativePath);
        var normalized = asset with { NativePath = path, PackageId = package, Sha256 = asset.Sha256.ToLowerInvariant(), ManagedSha256 = asset.ManagedSha256.ToLowerInvariant() };
        lock (Gate)
        {
            if (_module != 0)
            {
                if (_official != normalized) throw new InvalidOperationException("A different Skia asset was already selected for this process.");
                return;
            }
            using (var file = File.OpenRead(path))
                if (!Convert.ToHexString(SHA256.HashData(file)).Equals(asset.Sha256, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Deployed official native asset differs from NuGet provenance.");
            foreach (ProcessModule loaded in Process.GetCurrentProcess().Modules)
                if (loaded.ModuleName is "libSkiaSharp.dll" or "libSkiaSharp.so" or "libDorotiGraphite.dll" or "libDorotiGraphite.so")
                    if (!Path.GetFullPath(loaded.FileName).Equals(path, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                        throw new InvalidOperationException("Another Skia module was loaded before official asset selection.");
            var module = NativeLibrary.Load(path);
            if (NativeLibrary.TryGetExport(module, "doroti_graphite_interop_version", out _))
                throw new InvalidDataException("A custom ABI asset cannot be selected as an official asset.");
            var available = (delegate* unmanaged[Cdecl]<int, byte>)NativeLibrary.GetExport(module, "sk_graphite_backend_is_available");
            if (available((int)SKGraphiteBackend.Vulkan) == 0) throw new PlatformNotSupportedException("Official native asset has no Graphite Vulkan backend.");
            NativeLibrary.SetDllImportResolver(typeof(SKGraphiteContext).Assembly,
                (library, _, _) => library is "libSkiaSharp" or "libSkiaSharp.dll" or "libSkiaSharp.so" ? module : 0);
            _module = module;
            _official = normalized; // Module and resolver remain live through process exit.
        }
    }

    /// <summary>Select the pinned official desktop package before any Skia call.</summary>
    public static void Configure() => ConfigureOfficial(PackagedOfficialAsset());
}
