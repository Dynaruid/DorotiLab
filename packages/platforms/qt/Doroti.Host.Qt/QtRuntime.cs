using System.Runtime.InteropServices;
using System.Text.Json;

namespace Doroti.Host.Qt;

/// <summary>Resolve the private runtime before the first Qt/shim load.</summary>
internal static partial class QtRuntime
{
    private static readonly object Gate = new();
    private static bool _configured;
    private static readonly List<nint> Handles = [];

    internal static void Configure()
    {
        lock (Gate)
        {
            if (_configured) return;
            var path = Path.Combine(AppContext.BaseDirectory, "doroti-qt-runtime.json");
            // Development builds may deliberately use distribution Qt.
            if (!File.Exists(path))
            {
                if (Directory.Exists(Path.Combine(AppContext.BaseDirectory, "qt"))
                    || File.Exists(Path.Combine(AppContext.BaseDirectory, "qt.conf")))
                    throw new FileNotFoundException("The private Qt runtime manifest is missing; repair the application installation.", path);
                return;
            }
            using var document = JsonDocument.Parse(File.ReadAllText(path));
            var manifest = document.RootElement;
            if (manifest.GetProperty("schemaVersion").GetString() != "doroti.qt-runtime/v1"
                || manifest.GetProperty("architecture").GetString() != "linux-x64"
                || manifest.GetProperty("sandbox").GetString() != "default-required")
                throw new InvalidDataException("Unsupported Qt runtime manifest.");
            var framework = manifest.GetProperty("framework");
            var frameworkPath = ResolveRoot(framework, "DOROTI_QT_FRAMEWORK_PATH");
            var parts = new List<(JsonElement Part, string Root)> { (framework, frameworkPath) };
            if (manifest.GetProperty("webView").GetBoolean())
            {
                var engine = manifest.GetProperty("webEngine");
                var enginePath = ResolveRoot(engine, "DOROTI_QT_WEBENGINE_PATH");
                if (engine.GetProperty("frameworkVersion").GetString() != framework.GetProperty("version").GetString())
                    throw new InvalidDataException("WebEngine requires a different Framework ABI.");
                if (manifest.GetProperty("deployment").GetString() == "download")
                {
                    using var installed = JsonDocument.Parse(File.ReadAllText(Path.Combine(enginePath, "engine.json")));
                    foreach (var key in new[] { "version", "frameworkVersion", "packageId", "packageVersion" })
                        if (installed.RootElement.GetProperty(key).GetString() != engine.GetProperty(key).GetString())
                            throw new InvalidDataException("Installed WebEngine package does not match " + key + ".");
                }
                parts.Add((engine, enginePath));
                Default("QTWEBENGINEPROCESS_PATH", Path.Combine(enginePath, "libexec/QtWebEngineProcess"));
                Default("QTWEBENGINE_RESOURCES_PATH", Path.Combine(enginePath, "resources"));
                Default("QTWEBENGINE_LOCALES_PATH", Path.Combine(enginePath, "translations/qtwebengine_locales"));
            }
            Default("QT_QPA_PLATFORM", !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY"))
                || Environment.GetEnvironmentVariable("XDG_SESSION_TYPE") == "wayland" ? "wayland" : "xcb");
            Default("QT_VULKAN_LIB", "libvulkan.so.1");
            Prepend("QT_PLUGIN_PATH", Path.Combine(frameworkPath, "plugins"));
            foreach (var (_, root) in parts)
            {
                Prepend("QML2_IMPORT_PATH", Path.Combine(root, "qml"));
                Prepend("QML_IMPORT_PATH", Path.Combine(root, "qml"));
                // New helper processes observe this at exec; the current process
                // preloads by absolute path because glibc caches loader paths.
                Prepend("LD_LIBRARY_PATH", Path.Combine(root, "lib"));
            }
            var libraries = new Dictionary<string, (JsonElement File, string Root)>();
            foreach (var (part, root) in parts)
                foreach (var file in part.GetProperty("files").EnumerateArray())
                {
                    var name = file.GetProperty("path").GetString()!;
                    var target = Within(root, name);
                    if (!File.Exists(target))
                        throw new FileNotFoundException("Required Qt runtime file is missing. Install the matching engine package or set an explicit replacement runtime path.", target);
                    if (name.StartsWith("lib/", StringComparison.Ordinal) && file.TryGetProperty("soname", out var soname))
                        libraries.TryAdd(soname.GetString()!, (file, root));
                }
            var visiting = new HashSet<string>();
            var loaded = new HashSet<string>();
            void Load(string name)
            {
                if (loaded.Contains(name) || !libraries.TryGetValue(name, out var entry)) return;
                if (!visiting.Add(name)) throw new InvalidDataException("Cyclic Qt library dependency: " + name);
                foreach (var needed in entry.File.GetProperty("needed").EnumerateArray())
                {
                    var dependency = needed.GetString()!;
                    if (dependency.StartsWith("libQt6", StringComparison.Ordinal) && !libraries.ContainsKey(dependency))
                        throw new InvalidDataException("Qt runtime dependency is missing: " + dependency);
                    Load(dependency);
                }
                var fullPath = Within(entry.Root, entry.File.GetProperty("path").GetString()!);
                var handle = Dlopen(fullPath, 2 | 0x100); // RTLD_NOW | RTLD_GLOBAL
                if (handle == 0)
                    throw new DllNotFoundException("Cannot load " + fullPath + ": " + Marshal.PtrToStringUTF8(Dlerror()));
                Handles.Add(handle); // Keep Qt and its plugins valid for the process lifetime.
                visiting.Remove(name);
                loaded.Add(name);
            }
            foreach (var name in libraries.Keys) Load(name);
            // Hashes are publication evidence, never an execution restriction on
            // compatible user-modified LGPL libraries.
            _configured = true;
        }
    }

    private static string ResolveRoot(JsonElement part, string environment)
    {
        var root = Environment.GetEnvironmentVariable(environment);
        if (string.IsNullOrEmpty(root))
        {
            var declared = part.GetProperty("path").GetString()!;
            root = Path.IsPathRooted(declared) ? declared : Within(AppContext.BaseDirectory, declared);
        }
        if (!Path.IsPathRooted(root) || !Directory.Exists(root))
            throw new DirectoryNotFoundException("Qt runtime prefix is missing: " + root + ". Set " + environment + " to a compatible runtime prefix.");
        return Path.GetFullPath(root);
    }

    private static string Within(string root, string relative)
    {
        var path = Path.GetFullPath(Path.Combine(root, relative));
        if (Path.IsPathRooted(relative) || !path.StartsWith(Path.TrimEndingDirectorySeparator(root) + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new InvalidDataException("Qt runtime path escapes its prefix: " + relative);
        return path;
    }

    private static void Default(string key, string value)
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key))) Environment.SetEnvironmentVariable(key, value);
    }

    private static void Prepend(string key, string value)
    {
        var previous = Environment.GetEnvironmentVariable(key);
        Environment.SetEnvironmentVariable(key, value + (string.IsNullOrEmpty(previous) ? "" : ":" + previous));
    }

    [LibraryImport("libdl.so.2", EntryPoint = "dlopen", StringMarshalling = StringMarshalling.Utf8)]
    private static partial nint Dlopen(string filename, int flags);
    [LibraryImport("libdl.so.2", EntryPoint = "dlerror")]
    private static partial nint Dlerror();
}
