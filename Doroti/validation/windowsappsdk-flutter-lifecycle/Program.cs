using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;

namespace Doroti.Validation.WindowsAppSdkFlutterLifecycle;

internal static partial class Program
{
    private const uint WmSize = 0x0005;
    private const uint WmDisplayChange = 0x007e;
    private const uint WmDpiChanged = 0x02e0;
    private const uint WmPowerBroadcast = 0x0218;
    private const uint WmWtsSessionChange = 0x02b1;
    private const nuint SizeMinimized = 1;
    private const nuint PbtApmSuspend = 4;
    private const nuint PbtApmResumeAutomatic = 18;
    private const nuint WtsSessionLock = 7;
    private const nuint WtsSessionUnlock = 8;
    private const int Cycles = 10;

    private static int Main(string[] args)
    {
        try
        {
            var options = Options.Parse(args);
            var result = RunSta(Run);
            var executablePath = Environment.ProcessPath ?? throw new InvalidOperationException("Executable path unavailable.");
            var evidence = new
            {
                schemaVersion = "doroti.windowsappsdk-flutter-lifecycle-evidence/v1",
                status = "PASS",
                runId = Guid.NewGuid().ToString("N"),
                sourceFingerprint = options.SourceFingerprint ?? "unbound-local-run",
                executablePath,
                executableSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(executablePath))).ToLowerInvariant(),
                runtime = new
                {
                    packageContractVersion = FlutterWindowsAppSdkBootstrap.ExpectedWindowsAppSdkVersion,
                    deployment = "self-contained-unpackaged",
                },
                window = new
                {
                    topLevelHwnd = result.Host.TopLevelHwnd.ToInt64(),
                    childHwnd = result.Host.ViewHwnd.ToInt64(),
                    childRectMismatchCount = result.Host.ChildRectMismatchCount,
                    dpiSuggestedRectApplyCount = result.Host.DpiSuggestedRectApplyCount,
                    standardStyleRestored = (result.Host.TopLevelStyle & 0x00cf0000u) == 0x00cf0000u,
                },
                lifecycle = new
                {
                    result.Lifecycle.Fullscreen,
                    result.Lifecycle.Suspended,
                    result.Lifecycle.Shutdown,
                    restoreMonitor = result.Lifecycle.RestoreMonitor.ToInt64(),
                    result.Lifecycle.RestoreDpi,
                    result.Lifecycle.DpiChangedCount,
                    result.Lifecycle.DisplayChangedCount,
                    result.Lifecycle.MinimizeCount,
                    result.Lifecycle.RestoreCount,
                    result.Lifecycle.SuspendCount,
                    result.Lifecycle.ResumeCount,
                    result.Lifecycle.SessionDisconnectCount,
                    result.Lifecycle.SessionReconnectCount,
                    result.Lifecycle.FullscreenEnterCount,
                    result.Lifecycle.FullscreenExitCount,
                    result.Lifecycle.WorkAreaClampCount,
                    result.Lifecycle.GraphicsRecoveryRequestCount,
                    result.Lifecycle.PendingTerminalizationCount,
                },
                delegates = new
                {
                    latestFrameRequestCount = result.LatestFrameRequestCount,
                    graphicsRecoveryRequestCount = result.GraphicsRecoveryRequestCount,
                    pendingTerminalizationCount = result.PendingTerminalizationCount,
                },
                validation = new
                {
                    cycles = Cycles,
                    actualSuggestedRectApplied = result.Host.DpiSuggestedRectApplyCount >= Cycles,
                    fullscreenRoundTrips = result.Lifecycle.FullscreenEnterCount == Cycles &&
                        result.Lifecycle.FullscreenExitCount == Cycles && !result.Lifecycle.Fullscreen,
                    powerAndSessionRoundTrips = result.Lifecycle.SuspendCount == Cycles * 2 &&
                        result.Lifecycle.ResumeCount == Cycles * 2,
                    displayRecoveryRequests = result.Lifecycle.DisplayChangedCount == Cycles,
                    offscreenWindowClamped = result.Lifecycle.WorkAreaClampCount >= 1,
                    shutdownTerminalizedOnce = result.Lifecycle.PendingTerminalizationCount == 1,
                    noStaleChildExtent = result.Host.ChildRectMismatchCount == 0,
                },
                scopeBoundary = "F8 live lifecycle ownership and deterministic transition evidence; physical sleep, RDP infrastructure, monitor disconnect, and FG visible recovery remain separate acceptance.",
            };
            Assert(result.Host.DpiSuggestedRectApplyCount >= Cycles,
                $"WM_DPICHANGED suggested rect apply count was {result.Host.DpiSuggestedRectApplyCount}, expected at least {Cycles}.");
            Assert(result.Lifecycle.FullscreenEnterCount == Cycles && result.Lifecycle.FullscreenExitCount == Cycles,
                "Fullscreen round-trip count drifted.");
            Assert(result.Lifecycle.SuspendCount == Cycles * 2 && result.Lifecycle.ResumeCount == Cycles * 2,
                "Power/session suspend-resume count drifted.");
            Assert(result.Lifecycle.DisplayChangedCount == Cycles && result.Lifecycle.WorkAreaClampCount >= 1,
                "Display change did not clamp/recover the top-level window.");
            Assert(result.Lifecycle.PendingTerminalizationCount == 1 && result.PendingTerminalizationCount == 1,
                "Shutdown did not terminalize pending work exactly once.");
            Assert(result.Host.ChildRectMismatchCount == 0, "Lifecycle transition left child/client extent mismatched.");
            var json = JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true });
            if (!string.IsNullOrWhiteSpace(options.EvidencePath))
            {
                var path = Path.GetFullPath(options.EvidencePath);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllText(path, json + Environment.NewLine);
            }
            Console.WriteLine(json);
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"windowsappsdk-flutter-lifecycle FAIL: {exception}");
            return 1;
        }
    }

    private static Result Run()
    {
        var bootstrap = new FlutterWindowsAppSdkBootstrap();
        FlutterWindowsHostWindow? host = null;
        FlutterWindowsViewMetricsCoordinator? metrics = null;
        FlutterWindowsLifecycleManager? lifecycle = null;
        var latest = 0;
        var recovery = 0;
        var terminal = 0;
        try
        {
            host = FlutterWindowsHostWindow.CreateOnCurrentThread(
                bootstrap,
                new FlutterWindowsHostWindowOptions("Doroti F8 lifecycle validation", 640, 360, 320, 240, 1200, 900, 120, 90));
            metrics = FlutterWindowsViewMetricsCoordinator.AttachToHostWindow(
                host, 80_001, new FlutterWindowsPhysicalConstraints(320, 240, 1200, 900));
            lifecycle = new FlutterWindowsLifecycleManager(
                host,
                () => latest++,
                () => recovery++,
                () => terminal++);

            for (var cycle = 0; cycle < Cycles; cycle++)
            {
                var suggested = new NativeRect(120 + cycle, 90 + cycle, 800 + cycle, 500 + cycle);
                var memory = Marshal.AllocHGlobal(Marshal.SizeOf<NativeRect>());
                try
                {
                    Marshal.StructureToPtr(suggested, memory, false);
                    _ = SendMessageW(host.TopLevelHwnd, WmDpiChanged, MakeDpiWParam(96 + cycle), memory);
                }
                finally { Marshal.FreeHGlobal(memory); }

                lifecycle.SetFullscreen(true);
                lifecycle.SetFullscreen(false);
                _ = SendMessageW(host.TopLevelHwnd, WmSize, SizeMinimized, 0);
                _ = SendMessageW(host.TopLevelHwnd, WmSize, 0, 0);
                _ = SendMessageW(host.TopLevelHwnd, WmPowerBroadcast, PbtApmSuspend, 0);
                _ = SendMessageW(host.TopLevelHwnd, WmPowerBroadcast, PbtApmResumeAutomatic, 0);
                _ = SendMessageW(host.TopLevelHwnd, WmWtsSessionChange, WtsSessionLock, 0);
                _ = SendMessageW(host.TopLevelHwnd, WmWtsSessionChange, WtsSessionUnlock, 0);
                if (cycle == 0)
                    Assert(SetWindowPos(host.TopLevelHwnd, 0, -32000, -32000, 680, 410, 0x0010),
                        "Could not move F8 window offscreen for clamp validation.");
                _ = SendMessageW(host.TopLevelHwnd, WmDisplayChange, 32, 0);
            }

            lifecycle.BeginShutdown();
            lifecycle.BeginShutdown();
            var hostSnapshot = host.Snapshot;
            var lifecycleSnapshot = lifecycle.Snapshot;
            lifecycle.Dispose();
            lifecycle = null;
            metrics.Dispose();
            metrics = null;
            host.Dispose();
            host = null;
            return new(hostSnapshot, lifecycleSnapshot, latest, recovery, terminal);
        }
        finally
        {
            lifecycle?.Dispose();
            metrics?.Dispose();
            if (host is not null) host.Dispose();
            else if (bootstrap.Snapshot.Phase != FlutterWindowsAppSdkBootstrapPhase.Disposed)
                bootstrap.DisposeOnCurrentThread();
        }
    }

    private static T RunSta<T>(Func<T> action)
    {
        T? result = default;
        Exception? failure = null;
        using var completed = new ManualResetEventSlim();
        var thread = new Thread(() =>
        {
            try { result = action(); }
            catch (Exception exception) { failure = exception; }
            finally { completed.Set(); }
        }) { IsBackground = true, Name = "Doroti F8 platform STA validation" };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        if (!completed.Wait(TimeSpan.FromMinutes(20)) || !thread.Join(TimeSpan.FromMinutes(20)))
            throw new TimeoutException("F8 validation exceeded 20 minutes.");
        if (failure is not null) throw new InvalidOperationException("F8 STA validation failed.", failure);
        return result!;
    }

    private static nuint MakeDpiWParam(int dpi) => unchecked((nuint)((uint)dpi | ((uint)dpi << 16)));
    private static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }

    private sealed record Options(string? EvidencePath, string? SourceFingerprint)
    {
        internal static Options Parse(string[] args)
        {
            string? evidence = null;
            string? fingerprint = null;
            for (var index = 0; index < args.Length; index += 2)
            {
                if (index + 1 >= args.Length) throw new ArgumentException("F8 option requires a value.");
                if (args[index] == "--evidence") evidence = args[index + 1];
                else if (args[index] == "--source-fingerprint") fingerprint = args[index + 1];
                else throw new ArgumentException($"Unknown F8 option: {args[index]}");
            }
            return new(evidence, fingerprint);
        }
    }

    private sealed record Result(
        FlutterWindowsHostWindowSnapshot Host,
        FlutterWindowsLifecycleSnapshot Lifecycle,
        int LatestFrameRequestCount,
        int GraphicsRecoveryRequestCount,
        int PendingTerminalizationCount);

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct NativeRect(int Left, int Top, int Right, int Bottom);

    [LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
    private static partial nint SendMessageW(nint hwnd, uint message, nuint wParam, nint lParam);
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowPos(nint hwnd, nint insertAfter, int x, int y, int width, int height, uint flags);
}
