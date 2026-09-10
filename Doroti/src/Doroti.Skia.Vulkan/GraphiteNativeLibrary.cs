using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Diagnostics;
using SkiaSharp;

namespace Doroti.Skia.Vulkan;

/// <summary>Select the packaged, separately named ABI 3 library before any Skia call.</summary>
public static unsafe class GraphiteNativeLibrary
{
    private static readonly object Gate = new();
    private static nint _module;

    public static void Configure()
    {
        lock (Gate)
        {
            if (_module != 0) return;
            // Android AOT can bind P/Invokes directly, bypassing a managed resolver.
            // Its APK therefore contains the qualified asset under libSkiaSharp.so.
            var name = OperatingSystem.IsAndroid() ? "libSkiaSharp.so" : OperatingSystem.IsWindows() ? "libDorotiGraphite.dll" : "libDorotiGraphite.so";
            if (!OperatingSystem.IsAndroid())
            {
                var path = Path.Combine(AppContext.BaseDirectory, "graphite", name);
                using var provenance = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "graphite", "build-provenance.json")));
                var root = provenance.RootElement;
                var rid = OperatingSystem.IsWindows() ? "win-x64" : "linux-x64";
                var key = OperatingSystem.IsWindows() ? "libSkiaSharp.dll" : name;
                if (root.GetProperty("bridgeAbi").GetInt32() != 3 || root.GetProperty("rid").GetString() != rid)
                    throw new InvalidDataException("Packaged Graphite native ABI/RID differs from this host.");
                using var file = File.OpenRead(path);
                if (!Convert.ToHexString(SHA256.HashData(file)).Equals(root.GetProperty("files").GetProperty(key).GetString(), StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Packaged Graphite native hash differs from its build provenance.");
                foreach (ProcessModule loaded in Process.GetCurrentProcess().Modules)
                    if (loaded.ModuleName is "libSkiaSharp.dll" or "libSkiaSharp.so")
                        throw new InvalidOperationException("Stock Skia was loaded before Graphite selection; initialize the host before application Skia calls.");
            }
            var module = OperatingSystem.IsAndroid()
                ? NativeLibrary.Load(name, typeof(GraphiteNativeLibrary).Assembly, null)
                : NativeLibrary.Load(Path.Combine(AppContext.BaseDirectory, "graphite", name));
            var version = (delegate* unmanaged[Cdecl]<uint>)NativeLibrary.GetExport(module, "doroti_graphite_interop_version");
            if (version() != 3) throw new NotSupportedException("The packaged Graphite library must implement ABI 3.");
            NativeLibrary.SetDllImportResolver(typeof(SKGraphiteContext).Assembly,
                (library, _, _) => library is "libSkiaSharp" or "libSkiaSharp.dll" or "libSkiaSharp.so" ? module : 0);
            _module = module; // Retained until process exit, including outstanding native callbacks.
        }
    }

    public static nint Resolve(string name)
    {
        Configure();
        return NativeLibrary.GetExport(_module, name);
    }
}
