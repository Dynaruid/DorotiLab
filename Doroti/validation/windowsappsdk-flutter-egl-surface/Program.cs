using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;
using SkiaSharp;

namespace Doroti.Validation.WindowsAppSdkFlutterEglSurface;

internal static partial class Program
{
    private const int RequiredResizeCycles = 1000;
    private const int WarmupResizeCycles = 2;
    private const int GwlStyle = -16;
    private const int GwlExStyle = -20;
    private const uint SwpNoZOrder = 0x0004;
    private const uint SwpNoActivate = 0x0010;
    private const string EvidenceSchema =
        "doroti.windowsappsdk-flutter-egl-surface-evidence/v1";
    private static readonly TimeSpan TestTimeout = TimeSpan.FromMinutes(20);

    private static int Main(string[] args)
    {
        var options = ParseOptions(args);
        try
        {
            _ = RunOnDedicatedPlatformStaThread(() =>
            {
                _ = RunOneCycle(-1, WarmupResizeCycles, injectContextLoss: false);
                return 0;
            });
            var gdiBefore = GetGuiResources(GuiResourceType.Gdi);
            var userBefore = GetGuiResources(GuiResourceType.User);
            var result = RunOnDedicatedPlatformStaThread(() =>
                RunOneCycle(0, RequiredResizeCycles, injectContextLoss: true));
            var gdiAfter = GetGuiResources(GuiResourceType.Gdi);
            var userAfter = GetGuiResources(GuiResourceType.User);
            var guiResourcesBounded = gdiAfter <= gdiBefore + 2 && userAfter <= userBefore + 2;
            if (!guiResourcesBounded)
            {
                throw new InvalidOperationException(
                    $"F4 GUI resources grew after warmup: GDI {gdiBefore}->{gdiAfter}, USER {userBefore}->{userAfter}.");
            }

            var executablePath = Environment.ProcessPath ?? throw new InvalidOperationException(
                "The F4 validator executable path is unavailable.");
            var executableHash = ComputeFileHash(executablePath);
            if (!string.IsNullOrWhiteSpace(options.PublishedExecutableSha256) &&
                !string.Equals(executableHash, options.PublishedExecutableSha256,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The F4 validator executable hash does not match the publish-gate input.");
            }

            var active = result.ActiveSurface;
            var disposed = result.DisposedSurface;
            var evidence = new
            {
                schemaVersion = EvidenceSchema,
                runId = Guid.NewGuid().ToString("N"),
                adapter = "FlutterEmbedder",
                runtime = new
                {
                    packageContractVersion = FlutterWindowsAppSdkBootstrap.ExpectedWindowsAppSdkVersion,
                    deployment = "self-contained-unpackaged",
                },
                rasterThread = new
                {
                    apartment = "MTA",
                    executionMode = "dedicated-raster-thread",
                    createOutsideRasterThreadCount = active.CreateOutsideRasterThreadCount,
                    recreateOutsideRasterThreadCount = active.RecreateOutsideRasterThreadCount,
                    swapOutsideRasterThreadCount = active.SwapOutsideRasterThreadCount,
                    threadAffinityViolationCount = active.ThreadAffinityViolationCount,
                    managedThreadId = active.RasterManagedThreadId,
                    nativeThreadId = active.RasterNativeThreadId,
                },
                surface = new
                {
                    childHwndWindowSurface = true,
                    resizeCycleCount = RequiredResizeCycles,
                    createCount = disposed.CreateCount,
                    destroyCount = disposed.DestroyCount,
                    recreateCount = disposed.RecreateCount,
                    swapAttemptCount = disposed.SwapAttemptCount,
                    successfulSwapCount = disposed.SuccessfulSwapCount,
                    terminalFailureCount = 0,
                    exactExtentMismatchCount = disposed.ExactExtentMismatchCount,
                    zeroSizedSurfaceCreateCount = disposed.ZeroSizedSurfaceCreateCount,
                    physicalWidthPx = result.LastPhysicalWidth,
                    physicalHeightPx = result.LastPhysicalHeight,
                    firstActualSwapBeforePlatformShow = result.FirstActualSwapBeforePlatformShow,
                    firstFrameShowCount = result.FirstFrameShowCount,
                },
                context = new
                {
                    lossInjectionCount = active.ContextLossInjectionCount,
                    lossDetectedCount = active.ContextLossDetectedCount,
                    recoveryCount = active.RecoveryCount,
                    validFrameAfterRecovery = active.ValidFrameAfterRecovery,
                    sharedContextResourceLifetimeSeparated = active.SharedContextResourceLifetimeSeparated,
                    sharedContextGeneration = active.SharedContextGeneration,
                },
                resources = new
                {
                    eglSurfaceLeakCount = disposed.EglSurfaceLeakCount,
                    skiaResourceLeakCount = disposed.SkiaResourceLeakCount,
                    gdiBefore,
                    gdiAfter,
                    userBefore,
                    userAfter,
                    boundedAfterWarmup = guiResourcesBounded,
                },
                renderer = new
                {
                    name = active.Renderer,
                    classification = active.SoftwareFallback ? "software" : "hardware",
                    softwareFallback = active.SoftwareFallback,
                    visibleAcceptanceClaim = false,
                    hardwareVisiblePassClaim = false,
                },
                startup = new
                {
                    mauiOrXamlAssemblyCount = result.MauiOrXamlAssemblyNames.Length,
                    mauiOrXamlAssemblyNames = result.MauiOrXamlAssemblyNames,
                    noMauiOrXamlWindowStartup = result.MauiOrXamlAssemblyNames.Length == 0,
                },
                validation = new
                {
                    sourceFingerprint = options.SourceFingerprint,
                    executablePath,
                    executableSha256 = executableHash,
                    requiredResizeCycles = RequiredResizeCycles,
                },
                scopeBoundary = "F4 proves real ANGLE/EGL child-HWND window-surface creation, exact F3 physical extents, raster-thread Skia submit plus EGL swap, and deterministic context-loss recovery. It does not claim visible white-frame absence, compositor acceptance, or a hardware-visible pass.",
            };
            var json = JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true });
            if (!string.IsNullOrWhiteSpace(options.EvidencePath))
            {
                var evidencePath = Path.GetFullPath(options.EvidencePath);
                Directory.CreateDirectory(Path.GetDirectoryName(evidencePath) ??
                    throw new InvalidOperationException("The F4 evidence path has no parent directory."));
                File.WriteAllText(evidencePath, json + Environment.NewLine);
            }
            Console.WriteLine(json);
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"windowsappsdk-flutter-egl-surface FAIL: {exception}");
            return 1;
        }
    }

    private static F4CycleResult RunOneCycle(int cycle, int resizeCycles, bool injectContextLoss)
    {
        var bootstrap = new FlutterWindowsAppSdkBootstrap();
        FlutterWindowsHostWindow? host = null;
        FlutterWindowsViewMetricsCoordinator? metricsCoordinator = null;
        DedicatedRasterThread? rasterThread = null;
        FlutterWindowsAngleEglSharedContext? sharedContext = null;
        FlutterWindowsAngleEglWindowSurface? surface = null;
        FlutterWindowsAngleEglWindowSurfaceSnapshot? disposedSurface = null;
        var surfaceReleasedByHost = false;
        try
        {
            rasterThread = new DedicatedRasterThread();
            var capturedRasterThread = rasterThread;
            host = FlutterWindowsHostWindow.CreateOnCurrentThread(
                bootstrap,
                new FlutterWindowsHostWindowOptions(
                    $"Doroti F4 validation {cycle}",
                    InitialClientWidth: 640,
                    InitialClientHeight: 360,
                    MinimumClientWidth: 320,
                    MinimumClientHeight: 240,
                    MaximumClientWidth: 960,
                    MaximumClientHeight: 720,
                    InitialX: -32000,
                    InitialY: -32000),
                new FlutterWindowsHostWindowTeardown
                {
                    DisposeViewSurface = () =>
                    {
                        disposedSurface = RunOnDedicatedRasterThread(capturedRasterThread, () =>
                        {
                            try
                            {
                                surface?.Dispose();
                                return surface?.Snapshot;
                            }
                            finally
                            {
                                sharedContext?.Dispose();
                            }
                        }) ?? throw new InvalidOperationException(
                            "F4 did not retain a surface snapshot during view-surface teardown.");
                        surfaceReleasedByHost = true;
                    },
                });
            metricsCoordinator = FlutterWindowsViewMetricsCoordinator.AttachToHostWindow(
                host,
                viewId: checked((ulong)(10_000 + cycle + 2)),
                new FlutterWindowsPhysicalConstraints(320, 240, 960, 720));
            var initialMetrics = metricsCoordinator.Current;

            sharedContext = RunOnDedicatedRasterThread(capturedRasterThread, () =>
                FlutterWindowsAngleEglSharedContext.CreateOnCurrentRasterThread());
            surface = RunOnDedicatedRasterThread(capturedRasterThread, () =>
                FlutterWindowsAngleEglWindowSurface.CreateOnCurrentRasterThread(
                    sharedContext,
                    host.ViewHwnd,
                    initialMetrics));
            var firstPresent = RunOnDedicatedRasterThread(capturedRasterThread, () =>
                surface.RenderAndSwap(initialMetrics, SKColors.CornflowerBlue));
            var beforePlatformShow = host.Snapshot;
            if (!firstPresent.SuccessfulSwap || beforePlatformShow.FirstFrameSwapped || beforePlatformShow.Shown)
            {
                throw new InvalidOperationException(
                    "F4 attempted to show the platform HWND before a successful real child-HWND EGL swap.");
            }
            host.NotifyFirstFrameSwapped();
            var afterPlatformShow = host.Snapshot;
            if (!afterPlatformShow.FirstFrameSwapped || !afterPlatformShow.Shown ||
                afterPlatformShow.FirstFrameShowCount != 1)
            {
                throw new InvalidOperationException(
                    "F4 platform-thread first-frame notification did not show exactly one top-level HWND.");
            }

            WindowsViewMetrics lastMetrics = initialMetrics;
            for (var resize = 0; resize < resizeCycles; resize++)
            {
                var target = ResizeTarget(resize);
                SetTopLevelClientSize(host.TopLevelHwnd, target.Width, target.Height);
                var metrics = metricsCoordinator.ObserveChildMetrics();
                if (metrics.PhysicalWidth != target.Width || metrics.PhysicalHeight != target.Height)
                {
                    throw new InvalidOperationException(
                        "F4 platform resize did not publish the exact F3 child-client target.");
                }
                var update = RunOnDedicatedRasterThread(capturedRasterThread, () =>
                    surface.UpdateForMetrics(metrics));
                if (!update.Recreated || update.PhysicalWidth != metrics.PhysicalWidth ||
                    update.PhysicalHeight != metrics.PhysicalHeight)
                {
                    throw new InvalidOperationException(
                        "F4 did not recreate its EGL child surface for a changed exact metrics target.");
                }
                if (injectContextLoss && resize == resizeCycles / 2)
                {
                    RunOnDedicatedRasterThread(capturedRasterThread, () =>
                    {
                        surface.InjectContextLossForValidation();
                        return 0;
                    });
                }
                var present = RunOnDedicatedRasterThread(capturedRasterThread, () =>
                    surface.RenderAndSwap(metrics, (resize & 1) == 0 ? SKColors.MediumPurple : SKColors.CadetBlue));
                if (!present.SuccessfulSwap || present.PhysicalWidth != metrics.PhysicalWidth ||
                    present.PhysicalHeight != metrics.PhysicalHeight)
                {
                    throw new InvalidOperationException(
                        "F4 did not finish the exact child-HWND surface with a successful EGL swap.");
                }
                if (injectContextLoss && resize == resizeCycles / 2 && !present.RecoveredFromContextLoss)
                {
                    throw new InvalidOperationException(
                        "F4 deterministic context-loss injection did not recover before the next real swap.");
                }
                lastMetrics = metrics;
            }

            var activeSurface = RunOnDedicatedRasterThread(capturedRasterThread, () => surface.Snapshot);
            if (activeSurface.RecreateCount < resizeCycles ||
                activeSurface.SwapAttemptCount < resizeCycles + 1 ||
                activeSurface.SuccessfulSwapCount < resizeCycles + 1 ||
                activeSurface.ExactExtentMismatchCount != 0 ||
                activeSurface.ZeroSizedSurfaceCreateCount != 0 ||
                (injectContextLoss && (!activeSurface.ValidFrameAfterRecovery ||
                    activeSurface.ContextLossInjectionCount != 1 ||
                    activeSurface.ContextLossDetectedCount < 1 || activeSurface.RecoveryCount < 1)))
            {
                throw new InvalidOperationException("F4 surface lifecycle counters violate the live exercise contract.");
            }

            metricsCoordinator.Dispose();
            metricsCoordinator = null;
            host.Dispose();
            host = null;
            if (!surfaceReleasedByHost || disposedSurface is null ||
                disposedSurface.EglSurfaceLeakCount != 0 || disposedSurface.SkiaResourceLeakCount != 0)
            {
                throw new InvalidOperationException(
                    "F4 view-surface teardown did not release all per-window EGL and Skia resources.");
            }
            rasterThread.Dispose();
            rasterThread = null;
            return new F4CycleResult(
                activeSurface,
                disposedSurface,
                lastMetrics.PhysicalWidth,
                lastMetrics.PhysicalHeight,
                FirstActualSwapBeforePlatformShow: true,
                afterPlatformShow.FirstFrameShowCount,
                CaptureMauiOrXamlAssemblyNames());
        }
        finally
        {
            metricsCoordinator?.Dispose();
            if (host is not null)
            {
                host.Dispose();
            }
            else if (!surfaceReleasedByHost && rasterThread is not null)
            {
                DisposeRasterResources(rasterThread, surface, sharedContext);
            }
            if (host is null && bootstrap.Snapshot.Phase != FlutterWindowsAppSdkBootstrapPhase.Disposed)
            {
                bootstrap.DisposeOnCurrentThread();
            }
            rasterThread?.Dispose();
        }
    }

    private static T RunOnDedicatedRasterThread<T>(DedicatedRasterThread rasterThread, Func<T> action) =>
        rasterThread.Invoke(action);

    private static void DisposeRasterResources(
        DedicatedRasterThread rasterThread,
        FlutterWindowsAngleEglWindowSurface? surface,
        FlutterWindowsAngleEglSharedContext? sharedContext)
    {
        _ = RunOnDedicatedRasterThread(rasterThread, () =>
        {
            try { surface?.Dispose(); }
            finally { sharedContext?.Dispose(); }
            return 0;
        });
    }

    private static (int Width, int Height) ResizeTarget(int resize) =>
        (resize & 1) == 0 ? (641, 361) : (643, 363);

    /// <summary>
    /// F4 drives the standard top-level HWND only.  F2 lays out its one child;
    /// F3 then reads that child-client rect and F4 receives only that snapshot.
    /// </summary>
    private static void SetTopLevelClientSize(nint topLevelHwnd, int clientWidth, int clientHeight)
    {
        var style = unchecked((uint)GetWindowLongPtrW(topLevelHwnd, GwlStyle).ToInt64());
        var extendedStyle = unchecked((uint)GetWindowLongPtrW(topLevelHwnd, GwlExStyle).ToInt64());
        var dpi = GetDpiForWindow(topLevelHwnd);
        if (dpi == 0) dpi = 96;
        var outer = new NativeRect { Right = clientWidth, Bottom = clientHeight };
        if (!AdjustWindowRectExForDpi(ref outer, style, hasMenu: false, extendedStyle, dpi))
        {
            throw new InvalidOperationException(
                $"AdjustWindowRectExForDpi F4 resize failed: {Marshal.GetLastWin32Error()}");
        }
        if (!SetWindowPos(
                topLevelHwnd,
                0,
                -32000,
                -32000,
                outer.Width,
                outer.Height,
                SwpNoZOrder | SwpNoActivate))
        {
            throw new InvalidOperationException(
                $"SetWindowPos F4 top-level resize failed: {Marshal.GetLastWin32Error()}");
        }
        PumpPendingMessages();
    }

    private static void PumpPendingMessages()
    {
        while (PeekMessageW(out var message, 0, 0, 0, 1))
        {
            if (message.Message == 0x0012)
                throw new InvalidOperationException("F4 validation unexpectedly received WM_QUIT.");
            _ = TranslateMessage(in message);
            _ = DispatchMessageW(in message);
        }
    }

    private static T RunOnDedicatedPlatformStaThread<T>(Func<T> action)
    {
        T? result = default;
        Exception? failure = null;
        using var complete = new ManualResetEventSlim();
        var thread = new Thread(() =>
        {
            try { result = action(); }
            catch (Exception exception) { failure = exception; }
            finally { complete.Set(); }
        })
        {
            IsBackground = true,
            Name = "Doroti F4 platform STA validation",
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        if (!complete.Wait(TestTimeout) || !thread.Join(TestTimeout))
            throw new TimeoutException("F4 platform/raster validation did not terminate within 20 minutes.");
        if (failure is not null)
            throw new InvalidOperationException("F4 platform/raster validation failed.", failure);
        return result ?? throw new InvalidOperationException("F4 validation returned no result.");
    }

    private static string[] CaptureMauiOrXamlAssemblyNames() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetName().Name ?? assembly.FullName ?? "unknown")
            .Where(name => name.StartsWith(string.Concat("Microsoft.", "Maui"), StringComparison.OrdinalIgnoreCase) ||
                           name.StartsWith(string.Concat("Microsoft.UI.", "Xaml"), StringComparison.OrdinalIgnoreCase))
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

    private static int GetGuiResources(GuiResourceType type) =>
        GetGuiResourcesNative(GetCurrentProcess(), (uint)type);

    private static F4Options ParseOptions(string[] args)
    {
        string? evidencePath = null;
        string? sourceFingerprint = null;
        string? publishedExecutableSha256 = null;
        for (var index = 0; index < args.Length; index += 2)
        {
            if (index + 1 >= args.Length)
                throw new ArgumentException("Every F4 validator option requires a value.");
            switch (args[index])
            {
                case "--evidence": evidencePath = args[index + 1]; break;
                case "--source-fingerprint": sourceFingerprint = args[index + 1]; break;
                case "--published-executable-sha256": publishedExecutableSha256 = args[index + 1]; break;
                default:
                    throw new ArgumentException(
                        "Usage: Doroti.Validation.WindowsAppSdkFlutterEglSurface " +
                        "[--evidence <path>] [--source-fingerprint <sha256>] " +
                        "[--published-executable-sha256 <sha256>]");
            }
        }
        return new F4Options(evidencePath, sourceFingerprint, publishedExecutableSha256);
    }

    private static string ComputeFileHash(string path) =>
        Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    private static partial nint GetWindowLongPtrW(nint hwnd, int index);

    [LibraryImport("user32.dll")]
    private static partial uint GetDpiForWindow(nint hwnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AdjustWindowRectExForDpi(
        ref NativeRect rect,
        uint style,
        [MarshalAs(UnmanagedType.Bool)] bool hasMenu,
        uint extendedStyle,
        uint dpi);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowPos(
        nint hwnd,
        nint insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PeekMessageW(
        out NativeMessage message,
        nint hwnd,
        uint minimum,
        uint maximum,
        uint removeMessage);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool TranslateMessage(in NativeMessage message);

    [LibraryImport("user32.dll")]
    private static partial nint DispatchMessageW(in NativeMessage message);

    [LibraryImport("kernel32.dll")]
    private static partial nint GetCurrentProcess();

    [LibraryImport("user32.dll", EntryPoint = "GetGuiResources")]
    private static partial int GetGuiResourcesNative(nint process, uint flags);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        internal int X;
        internal int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
        internal int Width => Right - Left;
        internal int Height => Bottom - Top;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeMessage
    {
        internal nint Hwnd;
        internal uint Message;
        internal nuint WParam;
        internal nint LParam;
        internal uint Time;
        internal NativePoint Point;
        internal uint Private;
    }

    private enum GuiResourceType : uint
    {
        Gdi = 0,
        User = 1,
    }

    private sealed record F4Options(
        string? EvidencePath,
        string? SourceFingerprint,
        string? PublishedExecutableSha256);

    private sealed record F4CycleResult(
        FlutterWindowsAngleEglWindowSurfaceSnapshot ActiveSurface,
        FlutterWindowsAngleEglWindowSurfaceSnapshot DisposedSurface,
        int LastPhysicalWidth,
        int LastPhysicalHeight,
        bool FirstActualSwapBeforePlatformShow,
        long FirstFrameShowCount,
        string[] MauiOrXamlAssemblyNames);

    /// <summary>
    /// A single MTA thread owns every F4 context/surface call.  The STA
    /// platform thread can synchronously hand it immutable F3 metrics without
    /// ever touching EGL itself.
    /// </summary>
    private sealed class DedicatedRasterThread : IDisposable
    {
        private readonly BlockingCollection<Action> _work = new();
        private readonly ManualResetEventSlim _started = new();
        private readonly Thread _thread;
        private Exception? _threadFailure;
        private bool _disposed;

        internal DedicatedRasterThread()
        {
            _thread = new Thread(Run)
            {
                IsBackground = true,
                Name = "Doroti F4 dedicated MTA raster validation",
            };
            _thread.SetApartmentState(ApartmentState.MTA);
            _thread.Start();
            if (!_started.Wait(TestTimeout))
                throw new TimeoutException("F4 dedicated raster thread did not start within 20 minutes.");
            if (_threadFailure is not null)
                throw new InvalidOperationException("F4 dedicated raster thread failed during startup.", _threadFailure);
        }

        internal T Invoke<T>(Func<T> action)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            ArgumentNullException.ThrowIfNull(action);
            if (_threadFailure is not null)
                throw new InvalidOperationException("F4 dedicated raster thread is unavailable.", _threadFailure);
            var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            try
            {
                _work.Add(() =>
                {
                    try { completion.SetResult(action()); }
                    catch (Exception exception) { completion.SetException(exception); }
                });
            }
            catch (InvalidOperationException exception)
            {
                throw new ObjectDisposedException(nameof(DedicatedRasterThread), exception.Message);
            }
            if (!completion.Task.Wait(TestTimeout))
                throw new TimeoutException("F4 raster operation did not complete within 20 minutes.");
            return completion.Task.GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _work.CompleteAdding();
            if (!_thread.Join(TestTimeout))
                throw new TimeoutException("F4 dedicated raster thread did not stop within 20 minutes.");
            _work.Dispose();
            _started.Dispose();
            if (_threadFailure is not null)
                throw new InvalidOperationException("F4 dedicated raster thread terminated with an error.", _threadFailure);
        }

        private void Run()
        {
            try
            {
                _started.Set();
                foreach (var work in _work.GetConsumingEnumerable()) work();
            }
            catch (Exception exception)
            {
                _threadFailure = exception;
                _started.Set();
            }
        }
    }
}
