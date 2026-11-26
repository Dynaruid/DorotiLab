using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using SkiaSharp;

namespace Doroti.Skia.Vulkan;

/// <summary>Build/publish provenance for one official NuGet native asset.</summary>
public sealed record GraphiteNativeAsset(
    string NativePath,
    string PackageId,
    string Version,
    string Rid,
    string Sha256,
    string ManagedSha256
);

/// <summary>Select one native asset for the process before creating any Skia object.</summary>
public static unsafe class GraphiteNativeLibrary
{
    private static readonly object Gate = new();
    private static nint _module;
    private static GraphiteNativeAsset? _selectedAsset;
    private static bool _androidAssetSelected;
    public static bool IsAssetSelected => _selectedAsset is not null || _androidAssetSelected;

    [StructLayout(LayoutKind.Sequential)]
    private struct DlInfo
    {
        public nint FileName,
            Base,
            Symbol,
            Address;
    }

    [DllImport("libdl.so", EntryPoint = "dladdr")]
    private static extern int DlAddress(nint address, out DlInfo info);

    [DllImport("libdl.so.2", EntryPoint = "dladdr")]
    private static extern int DlAddressLinux(nint address, out DlInfo info);

    /// <summary>Version-pinned desktop package deployment; no custom staging directory.</summary>
    public static GraphiteNativeAsset GetPackagedAsset()
    {
        var windows = OperatingSystem.IsWindows();
        if (!windows && !OperatingSystem.IsLinux())
        {
            throw new PlatformNotSupportedException();
        }

        var managedHash = GetManagedHash();
        var package4156 = managedHash == "b25527130f8f27fd0a3c9c7732bf2432a4732c148be774125c66e1d8a5e88ae3"
            || (windows && managedHash == "58f32363c74ce37dc2cfbc16ee49278453189e37948fb7746e268969e1ea8fee");
        if (
            !package4156 && managedHash != "71274ef112eefdf5ca8a1517086787bc786c7103c30115abbb9dc424a09db43e"
            && (
                !windows
                || managedHash != "c3719ba116d106b97d65665cc83cf9cfa1cbb8e58fbda0580d26a69340c8a82e"
            )
        )
        {
            throw new InvalidDataException(
                "Managed SkiaSharp differs from the pinned official desktop package entries."
            );
        }

        return new(
            Path.Combine(
                AppContext.BaseDirectory,
                windows ? "libSkiaSharp.dll" : "libSkiaSharp.so"
            ),
            windows ? "SkiaSharp.NativeAssets.Win32" : "SkiaSharp.NativeAssets.Linux",
            package4156 ? "4.156.0" : "4.154.0",
            windows ? "win-x64" : "linux-x64",
            (windows, package4156) switch
            {
                (true, true) => "591421fa5322466196321a5fd216716c74ca02d7af64c0a62cb45edb0147c0d7",
                (true, false) => "82f9b2ee2b2dc91f738ae4baa54cd21fdd1181883c0fa207ab7066c708f07108",
                (false, true) => "d32f2d9fe8d7b63f03909726462ebc96442c173ddbc3e6d118d8d2cc80775736",
                _ => "a96876fc38d29ad5364ceee6f1ad57d87d94844a6cd38306a36f3f91edf9beab",
            },
            managedHash
        );
    }

    private static string GetManagedHash()
    {
        if (OperatingSystem.IsWindows() && !RuntimeFeature.IsDynamicCodeSupported)
        {
            using var provenance = Assembly.GetEntryAssembly()?.GetManifestResourceStream("Doroti.Skia.NativeAot.SkiaSharp.sha256")
                ?? throw new PlatformNotSupportedException("Windows Native AOT requires embedded SkiaSharp build provenance.");
            using var reader = new StreamReader(provenance);
            return reader.ReadToEnd().Trim().ToLowerInvariant();
        }

        return GetFileBackedManagedHash();
    }

    [UnconditionalSuppressMessage("SingleFile", "IL3000", Justification = "File-backed deployments verify the real managed DLL. Windows AOT uses the embedded build hash.")]
    private static string GetFileBackedManagedHash()
    {
        var path = typeof(SKGraphiteContext).Assembly.Location;
        if (string.IsNullOrEmpty(path))
            throw new PlatformNotSupportedException("Desktop single-file/AOT needs linked-asset provenance.");

        using var managed = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(managed)).ToLowerInvariant();
    }

    /// <summary>Verify a standalone APK's native entry before normal Android/AOT binding.</summary>
    public static void ConfigureAndroid(string apkPath, string nativeLibraryDirectory) =>
        ConfigureAndroid(apkPath, nativeLibraryDirectory, null);

    /// <summary>Verify the installed base and split APKs before normal Android/AOT binding.</summary>
    public static void ConfigureAndroid(
        string apkPath,
        string nativeLibraryDirectory,
        IEnumerable<string>? splitApkPaths
    )
    {
        if (!OperatingSystem.IsAndroid())
        {
            throw new PlatformNotSupportedException();
        }

        lock (Gate)
        {
            if (_module != 0)
            {
                if (!_androidAssetSelected)
                {
                    throw new InvalidOperationException("Another Skia asset was already selected.");
                }

                return;
            }
            // SHA-256 pins from the signed SkiaSharp.NativeAssets.Android 4.156.0 package.
            var (abi, hash) = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.Arm64 => (
                    "arm64-v8a",
                    "73373bb034ea0cf5e49f9bcf776ea9e10d94483131c479a6266f15ca4e1cb087"
                ),
                Architecture.X64 => (
                    "x86_64",
                    "662da4edf92040279737c338ee87dd090d546465e77e7be2dba567c074b57b04"
                ),
                _ => throw new PlatformNotSupportedException(
                    "Official Graphite supports Android arm64/x64."
                ),
            };
            // ABI splits may own the native entry instead of base.apk. Check the
            // complete installed set, retaining duplicate and package-hash guards.
            var containingApk = AndroidGraphiteApkAsset.Verify(apkPath, splitApkPaths, abi, hash);
            var extracted = Path.Combine(nativeLibraryDirectory, "libSkiaSharp.so");
            string? verifiedExtractedPath = null;
            if (File.Exists(extracted))
            {
                using var file = File.OpenRead(extracted);
                if (
                    !Convert
                        .ToHexString(SHA256.HashData(file))
                        .Equals(hash, StringComparison.OrdinalIgnoreCase)
                )
                {
                    throw new InvalidDataException(
                        "Extracted Android library differs from APK provenance."
                    );
                }

                verifiedExtractedPath = extracted;
            }
            var module = NativeLibrary.Load(
                "libSkiaSharp.so",
                typeof(GraphiteNativeLibrary).Assembly,
                null
            );
            if (NativeLibrary.TryGetExport(module, "doroti_graphite_interop_version", out _))
            {
                throw new InvalidDataException(
                    "Custom ABI library was loaded instead of the official APK asset."
                );
            }

            var available = (delegate* unmanaged[Cdecl]<int, byte>)
                NativeLibrary.GetExport(module, "sk_graphite_backend_is_available");
            if (DlAddress((nint)available, out var location) == 0)
            {
                throw new InvalidDataException("Cannot identify the loaded Android Skia module.");
            }

            var loadedPath = Marshal.PtrToStringUTF8(location.FileName)!;
            if (
                !AndroidGraphiteApkAsset.IsVerifiedLoadPath(
                    loadedPath,
                    containingApk,
                    abi,
                    verifiedExtractedPath
                )
            )
            {
                throw new InvalidDataException(
                    "Android loaded Skia outside the verified APK/native directory: " + loadedPath
                );
            }

            if (available((int)SKGraphiteBackend.Vulkan) == 0)
            {
                throw new PlatformNotSupportedException(
                    "Official Android asset lacks Graphite Vulkan."
                );
            }

            NativeLibrary.SetDllImportResolver(
                typeof(SKGraphiteContext).Assembly,
                (library, _, _) => library is "libSkiaSharp" or "libSkiaSharp.so" ? module : 0
            );
            _module = module;
            _androidAssetSelected = true;
            Console.WriteLine(
                $"DorotiGraphite official package=SkiaSharp.NativeAssets.Android version=4.156.0 abi={abi} sha256={hash} apk={containingApk} loaded={loadedPath}"
            );
        }
    }

    public static GraphiteNativeAsset ReadAssetManifest(string manifestPath)
    {
        if (!Path.IsPathFullyQualified(manifestPath))
        {
            throw new InvalidDataException("Official native manifest requires an absolute path.");
        }

        using var json = JsonDocument.Parse(File.ReadAllText(manifestPath));
        var root = json.RootElement;
        return new(
            root.GetProperty("nativePath").GetString()!,
            root.GetProperty("packageId").GetString()!,
            root.GetProperty("version").GetString()!,
            root.GetProperty("rid").GetString()!,
            root.GetProperty("sha256").GetString()!,
            root.GetProperty("managedSha256").GetString()!
        );
    }

    /// <summary>
    /// Explicit official-asset path for qualified desktop hosts. The supplied hash
    /// must come from the resolved, integrity-checked NuGet package at build/publish.
    /// This verifies the deployed file; it does not replace archive signature checks.
    /// Android AOT/static-link provenance requires its platform-specific package path.
    /// </summary>
    public static void Configure(GraphiteNativeAsset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);
        if (!Path.IsPathFullyQualified(asset.NativePath))
        {
            throw new InvalidDataException("Official native asset requires an absolute path.");
        }

        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
        {
            throw new PlatformNotSupportedException(
                "This official native file loader is qualified for desktop dynamic libraries only."
            );
        }

        var rid = OperatingSystem.IsWindows() ? "win-x64" : "linux-x64";
        var package = OperatingSystem.IsWindows()
            ? "SkiaSharp.NativeAssets.Win32"
            : "SkiaSharp.NativeAssets.Linux";
        if (
            RuntimeInformation.ProcessArchitecture != Architecture.X64
            || asset.Rid != rid
            || !asset.PackageId.Equals(package, StringComparison.OrdinalIgnoreCase)
        )
        {
            throw new InvalidDataException(
                "Official Graphite package/RID does not match the process."
            );
        }

        var version = typeof(SKGraphiteContext)
            .Assembly.GetCustomAttributes(
                typeof(System.Reflection.AssemblyInformationalVersionAttribute),
                false
            )
            .OfType<System.Reflection.AssemblyInformationalVersionAttribute>()
            .Single()
            .InformationalVersion.Split('+')[0];
        // Upstream assembly informational metadata omits the NuGet preview/build
        // suffix. Verify its numeric version and the exact managed file hash from
        // the same resolved package pair instead of pretending metadata is the full pin.
        if (asset.Version.Split('-')[0] != version)
        {
            throw new InvalidDataException(
                "Official Graphite managed/native base versions differ."
            );
        }

        if (asset.Sha256.Length != 64 || !asset.Sha256.All(Uri.IsHexDigit))
        {
            throw new InvalidDataException("Official asset requires a SHA-256 provenance value.");
        }

        if (asset.ManagedSha256.Length != 64 || !asset.ManagedSha256.All(Uri.IsHexDigit))
        {
            throw new InvalidDataException(
                "Official asset requires managed package SHA-256 provenance."
            );
        }

        if (!GetManagedHash().Equals(asset.ManagedSha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("Loaded SkiaSharp managed assembly differs from NuGet provenance.");
        }

        var path = Path.GetFullPath(asset.NativePath);
        var normalized = asset with
        {
            NativePath = path,
            PackageId = package,
            Sha256 = asset.Sha256.ToLowerInvariant(),
            ManagedSha256 = asset.ManagedSha256.ToLowerInvariant(),
        };
        lock (Gate)
        {
            if (_module != 0)
            {
                if (_selectedAsset != normalized)
                {
                    throw new InvalidOperationException(
                        "A different Skia asset was already selected for this process."
                    );
                }

                return;
            }
            using (var file = File.OpenRead(path))
            {
                if (
                    !Convert
                        .ToHexString(SHA256.HashData(file))
                        .Equals(asset.Sha256, StringComparison.OrdinalIgnoreCase)
                )
                {
                    throw new InvalidDataException(
                        "Deployed official native asset differs from NuGet provenance."
                    );
                }
            }

            foreach (ProcessModule loaded in Process.GetCurrentProcess().Modules)
            {
                if (
                    loaded.ModuleName
                    is "libSkiaSharp.dll"
                        or "libSkiaSharp.so"
                        or "libDorotiGraphite.dll"
                        or "libDorotiGraphite.so"
                )
                {
                    if (
                        !Path.GetFullPath(loaded.FileName)
                            .Equals(
                                path,
                                OperatingSystem.IsWindows()
                                    ? StringComparison.OrdinalIgnoreCase
                                    : StringComparison.Ordinal
                            )
                    )
                    {
                        throw new InvalidOperationException(
                            "Another Skia module was loaded before official asset selection."
                        );
                    }
                }
            }

            var module = NativeLibrary.Load(path);
            if (NativeLibrary.TryGetExport(module, "doroti_graphite_interop_version", out _))
            {
                throw new InvalidDataException(
                    "A custom ABI asset cannot be selected as an official asset."
                );
            }

            var available = (delegate* unmanaged[Cdecl]<int, byte>)
                NativeLibrary.GetExport(module, "sk_graphite_backend_is_available");
            if (available((int)SKGraphiteBackend.Vulkan) == 0)
            {
                throw new PlatformNotSupportedException(
                    "Official native asset has no Graphite Vulkan backend."
                );
            }

            var loadedPath = path;
            if (OperatingSystem.IsLinux())
            {
                if (DlAddressLinux((nint)available, out var location) == 0)
                {
                    throw new InvalidDataException("Cannot identify the loaded Linux Skia module.");
                }

                loadedPath = Marshal.PtrToStringUTF8(location.FileName)!;
                if (!Path.GetFullPath(loadedPath).Equals(path, StringComparison.Ordinal))
                {
                    throw new InvalidDataException(
                        "Linux loaded Skia outside the verified package path: " + loadedPath
                    );
                }
            }
            NativeLibrary.SetDllImportResolver(
                typeof(SKGraphiteContext).Assembly,
                (library, _, _) =>
                    library is "libSkiaSharp" or "libSkiaSharp.dll" or "libSkiaSharp.so"
                        ? module
                        : 0
            );
            _module = module;
            _selectedAsset = normalized; // Module and resolver remain live through process exit.
            if (OperatingSystem.IsLinux())
            {
                Console.WriteLine(
                    $"DorotiGraphite official package={package} version={asset.Version} rid={rid} sha256={normalized.Sha256} loaded={loadedPath}"
                );
            }
        }
    }

    /// <summary>Select the pinned official desktop package before any Skia call.</summary>
    public static void Configure() => Configure(GetPackagedAsset());
}
