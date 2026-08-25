using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;

namespace Doroti.Validation.WindowsAppSdkFlutterBootstrap;

internal static partial class Program
{
    private const int RequiredCycles = 100;
    private const uint WsOverlappedWindow = 0x00CF0000;
    private const string EvidenceSchema =
        "doroti.windowsappsdk-flutter-bootstrap-evidence/v1";

    private static int Main(string[] args)
    {
        var options = ParseOptions(args);
        try
        {
            var platform = RunPlatformCycles(RequiredCycles);
            var raster = RunRasterCycles(RequiredCycles);
            var startup = CaptureStartupEvidence();
            var executablePath = Environment.ProcessPath ?? throw new InvalidOperationException(
                "The F1 validator executable path is unavailable.");
            var executableHash = ComputeFileHash(executablePath);
            if (!string.IsNullOrWhiteSpace(options.PublishedExecutableSha256) &&
                !string.Equals(executableHash, options.PublishedExecutableSha256,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The F1 validator executable hash does not match the publish-gate input.");
            }
            var evidence = new
            {
                schemaVersion = EvidenceSchema,
                runId = Guid.NewGuid().ToString("N"),
                adapter = "FlutterEmbedder",
                runtime = new
                {
                    packageContractVersion = FlutterWindowsAppSdkBootstrap.ExpectedWindowsAppSdkVersion,
                    deployment = "self-contained-unpackaged",
                    pathFallbackUsed = raster.NativeProvenance.PathFallbackUsed,
                    appBaseDirectory = AppContext.BaseDirectory,
                    windowsAppSdkAssemblyVersion = platform.WindowsAppSdkAssemblyVersion,
                    windowsAppSdkAssemblyPath = platform.WindowsAppSdkAssemblyPath,
                },
                platformThread = new
                {
                    apartment = "STA",
                    executionMode = "same-sta-thread",
                    dispatcherQueueAssociated = platform.DispatcherQueueAssociated,
                    rawWindowAssociated = platform.RawWindowAssociated,
                    shutdownCompleted = platform.ShutdownCompleted,
                    cycles = platform.Count,
                    distinctPlatformThreads = platform.DistinctNativeThreadCount,
                    failureCount = platform.FailureCount,
                },
                rasterThread = new
                {
                    apartment = "MTA",
                    backend = "ANGLE/EGL/GLES",
                    contextCreateDestroyCycles = raster.Count,
                    renderer = raster.Renderer,
                    softwareFallback = raster.SoftwareFallback,
                    teardownFailureCount = raster.TeardownFailureCount,
                    managedThreadId = raster.ManagedThreadId,
                    nativeThreadId = raster.NativeThreadId,
                },
                nativeArtifacts = new
                {
                    angle = new
                    {
                        packageId = raster.NativeProvenance.AnglePackageId,
                        version = raster.NativeProvenance.AnglePackageVersion,
                        architecture = raster.NativeProvenance.Architecture,
                        hash = raster.NativeProvenance.AngleSha256,
                        path = raster.NativeProvenance.AnglePath,
                    },
                    skia = new
                    {
                        packageId = raster.NativeProvenance.SkiaPackageId,
                        version = raster.NativeProvenance.SkiaPackageVersion,
                        hash = raster.NativeProvenance.SkiaSha256,
                        path = raster.NativeProvenance.SkiaPath,
                    },
                },
                startup = new
                {
                    mauiOrXamlAssemblyCount = startup.MauiOrXamlAssemblyNames.Length,
                    mauiOrXamlAssemblyNames = startup.MauiOrXamlAssemblyNames,
                    noMauiOrXamlWindowStartup = startup.MauiOrXamlAssemblyNames.Length == 0,
                },
                validation = new
                {
                    sourceFingerprint = options.SourceFingerprint,
                    executablePath = executablePath,
                    executableSha256 = executableHash,
                },
                scopeBoundary = "F1 bootstrap only; no child HWND, window-surface resize, first-frame show, input, or visible acceptance.",
            };
            var json = JsonSerializer.Serialize(evidence, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
            if (!string.IsNullOrWhiteSpace(options.EvidencePath))
            {
                var fullPath = Path.GetFullPath(options.EvidencePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ??
                    throw new InvalidOperationException("The evidence path has no parent directory."));
                File.WriteAllText(fullPath, json + Environment.NewLine);
            }
            Console.WriteLine(json);
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"windowsappsdk-flutter-bootstrap FAIL: {exception}");
            return 1;
        }
    }

    private static ValidationOptions ParseOptions(string[] args)
    {
        string? evidencePath = null;
        string? sourceFingerprint = null;
        string? publishedExecutableSha256 = null;
        for (var index = 0; index < args.Length; index += 2)
        {
            if (index + 1 >= args.Length)
                throw new ArgumentException("Every F1 validator option requires a value.");
            switch (args[index])
            {
                case "--evidence":
                    evidencePath = args[index + 1];
                    break;
                case "--source-fingerprint":
                    sourceFingerprint = args[index + 1];
                    break;
                case "--published-executable-sha256":
                    publishedExecutableSha256 = args[index + 1];
                    break;
                default:
                    throw new ArgumentException(
                        "Usage: Doroti.Validation.WindowsAppSdkFlutterBootstrap " +
                        "[--evidence <path>] [--source-fingerprint <sha256>] " +
                        "[--published-executable-sha256 <sha256>]");
            }
        }
        return new ValidationOptions(evidencePath, sourceFingerprint, publishedExecutableSha256);
    }

    private static PlatformCycleSummary RunPlatformCycles(int count) =>
        RunOnDedicatedThread(ApartmentState.STA, () =>
        {
            PlatformCycleResult? last = null;
            var completed = 0;
            for (var cycle = 0; cycle < count; cycle++)
            {
                last = RunPlatformCycle(cycle);
                completed++;
            }
            var result = last ?? throw new InvalidOperationException(
                "No Windows App SDK platform bootstrap cycle was run.");
            return new PlatformCycleSummary(
                completed,
                1,
                result.DispatcherQueueAssociated,
                result.RawWindowAssociated,
                result.ShutdownCompleted,
                0,
                result.WindowsAppSdkAssemblyVersion,
                result.WindowsAppSdkAssemblyPath);
        });

    private static PlatformCycleResult RunPlatformCycle(int cycle)
    {
        var bootstrap = new FlutterWindowsAppSdkBootstrap();
        nint hwnd = 0;
        try
        {
            bootstrap.InitializeOnCurrentThread();
            hwnd = CreateWindowExW(
                0,
                "STATIC",
                $"Doroti F1 bootstrap {cycle}",
                WsOverlappedWindow,
                0,
                0,
                1,
                1,
                0,
                0,
                0,
                0);
            if (hwnd == 0)
                throw new InvalidOperationException(
                    $"CreateWindowExW failed: {Marshal.GetLastWin32Error()}");

            _ = bootstrap.AssociateRawWindow(hwnd);
            var associated = bootstrap.Snapshot;
            if (!associated.DispatcherQueueCreated ||
                !associated.PlatformTaskRunnerCreated ||
                !associated.RawWindowAssociated ||
                !associated.AppWindowAssociated ||
                associated.RawWindowAssociationCount != 1)
            {
                throw new InvalidOperationException(
                    "The raw HWND, AppWindow, and DispatcherQueue did not form the required 1:1 association.");
            }

            if (!DestroyWindow(hwnd))
                throw new InvalidOperationException(
                    $"DestroyWindow failed: {Marshal.GetLastWin32Error()}");
            bootstrap.ReleaseRawWindowAssociation(hwnd);
            hwnd = 0;
            bootstrap.DisposeOnCurrentThread();
            if (bootstrap.Snapshot.Phase != FlutterWindowsAppSdkBootstrapPhase.Disposed)
                throw new InvalidOperationException("The platform bootstrap did not reach Disposed.");
            return new PlatformCycleResult(
                associated.PlatformNativeThreadId,
                associated.DispatcherQueueCreated && associated.PlatformTaskRunnerCreated &&
                associated.AppWindowAssociated,
                associated.RawWindowAssociated,
                bootstrap.Snapshot.Phase == FlutterWindowsAppSdkBootstrapPhase.Disposed,
                associated.WindowsAppSdkAssemblyVersion,
                associated.WindowsAppSdkAssemblyPath);
        }
        finally
        {
            if (hwnd != 0 && IsWindow(hwnd)) _ = DestroyWindow(hwnd);
            if (bootstrap.Snapshot.Phase != FlutterWindowsAppSdkBootstrapPhase.Disposed)
            {
                if (bootstrap.Snapshot.RawWindowAssociated && hwnd != 0)
                    bootstrap.ReleaseRawWindowAssociation(hwnd);
                bootstrap.DisposeOnCurrentThread();
            }
        }
    }

    private static RasterCycleSummary RunRasterCycles(int count) =>
        RunOnDedicatedThread(ApartmentState.MTA, () =>
        {
            FlutterWindowsAngleEglSmokeResult? last = null;
            for (var cycle = 0; cycle < count; cycle++)
            {
                using var context = FlutterWindowsAngleEglContext.CreateOffscreenOnCurrentThread();
                last = context.RunOffscreenSmoke();
            }
            var smoke = last ?? throw new InvalidOperationException("No ANGLE/EGL smoke was run.");
            var teardownFailureCount = FlutterWindowsAngleEglContext.TeardownFailureCount;
            if (teardownFailureCount != 0)
                throw new InvalidOperationException(
                    $"ANGLE/EGL teardown reported {teardownFailureCount} failure(s).");
            return new RasterCycleSummary(
                count,
                smoke.Renderer,
                smoke.NativeProvenance,
                smoke.RasterManagedThreadId,
                smoke.RasterNativeThreadId,
                smoke.SoftwareFallback,
                teardownFailureCount);
        });

    private static T RunOnDedicatedThread<T>(ApartmentState apartment, Func<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        T? result = default;
        Exception? failure = null;
        using var completed = new ManualResetEventSlim();
        var thread = new Thread(() =>
        {
            try
            {
                result = action();
            }
            catch (Exception exception)
            {
                failure = exception;
            }
            finally
            {
                completed.Set();
            }
        })
        {
            IsBackground = true,
            Name = $"Doroti F1 {apartment} validation",
        };
        thread.SetApartmentState(apartment);
        thread.Start();
        if (!completed.Wait(TimeSpan.FromSeconds(10)) || !thread.Join(TimeSpan.FromSeconds(10)))
            throw new TimeoutException($"F1 {apartment} validation thread did not terminate.");
        if (failure is not null)
            throw new InvalidOperationException($"F1 {apartment} validation failed.", failure);
        return result ?? throw new InvalidOperationException("F1 validation thread returned no result.");
    }

    private static StartupEvidence CaptureStartupEvidence()
    {
        var assemblyNames = AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetName().Name ?? assembly.FullName ?? "unknown")
            .Where(name => name.StartsWith("Microsoft.Maui", StringComparison.OrdinalIgnoreCase) ||
                           name.StartsWith("Microsoft.UI.Xaml", StringComparison.OrdinalIgnoreCase))
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return new StartupEvidence(assemblyNames);
    }

    private static string ComputeFileHash(string path) =>
        Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();

    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint CreateWindowExW(
        uint extendedStyle,
        string className,
        string windowName,
        uint style,
        int x,
        int y,
        int width,
        int height,
        nint parent,
        nint menu,
        nint instance,
        nint parameter);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DestroyWindow(nint hwnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsWindow(nint hwnd);

    private sealed record ValidationOptions(
        string? EvidencePath,
        string? SourceFingerprint,
        string? PublishedExecutableSha256);

    private sealed record StartupEvidence(string[] MauiOrXamlAssemblyNames);

    private sealed record PlatformCycleSummary(
        int Count,
        int DistinctNativeThreadCount,
        bool DispatcherQueueAssociated,
        bool RawWindowAssociated,
        bool ShutdownCompleted,
        int FailureCount,
        string WindowsAppSdkAssemblyVersion,
        string WindowsAppSdkAssemblyPath);

    private sealed record PlatformCycleResult(
        uint NativeThreadId,
        bool DispatcherQueueAssociated,
        bool RawWindowAssociated,
        bool ShutdownCompleted,
        string WindowsAppSdkAssemblyVersion,
        string WindowsAppSdkAssemblyPath);

    private sealed record RasterCycleSummary(
        int Count,
        string Renderer,
        FlutterWindowsAngleNativeProvenance NativeProvenance,
        int ManagedThreadId,
        uint NativeThreadId,
        bool SoftwareFallback,
        int TeardownFailureCount);
}
