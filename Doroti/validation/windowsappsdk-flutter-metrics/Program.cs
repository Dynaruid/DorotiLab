using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;
using Doroti.Ui;

namespace Doroti.Validation.WindowsAppSdkFlutterMetrics;

internal static partial class Program
{
    private const int RequiredCycles = 100;
    private const int GwlStyle = -16;
    private const int GwlExStyle = -20;
    private const int SwHide = 0;
    private const int SwMinimize = 6;
    private const int SwRestore = 9;
    private const uint SwpNoZOrder = 0x0004;
    private const uint SwpNoActivate = 0x0010;
    private const string EvidenceSchema =
        "doroti.windowsappsdk-flutter-metrics-evidence/v1";
    private static readonly MatrixCase[] Matrix =
    [
        new(100, 96),
        new(125, 120),
        new(150, 144),
        new(200, 192),
    ];

    private static int Main(string[] args)
    {
        var options = ParseOptions(args);
        try
        {
            var result = RunOnDedicatedStaThread(RunCycles);
            var executablePath = Environment.ProcessPath ?? throw new InvalidOperationException(
                "The F3 validator executable path is unavailable.");
            var executableHash = ComputeFileHash(executablePath);
            if (!string.IsNullOrWhiteSpace(options.PublishedExecutableSha256) &&
                !string.Equals(executableHash, options.PublishedExecutableSha256,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The F3 validator executable hash does not match the publish-gate input.");
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
                },
                platformThread = new
                {
                    apartment = "STA",
                    executionMode = "same-sta-thread",
                    startupShutdownCycles = result.Cycles,
                    failureCount = result.FailureCount,
                },
                metrics = new
                {
                    physicalAuthority = "child-client-rect",
                    immutableSnapshots = true,
                    logicalToPhysicalRoundingAuthority = "WindowsViewMetrics.LogicalToPhysical",
                    logicalToPhysicalRoundingSiteCount = 1,
                    viewId = result.ViewId.ToString("X"),
                    displayId = result.DisplayId,
                    generationMonotonic = result.GenerationMonotonic,
                    generationCount = result.GenerationCount,
                    observationCount = result.ObservationCount,
                    dpiRequeryCount = result.DpiAndDisplayRequeryCount,
                    displayRequeryCount = result.DpiAndDisplayRequeryCount,
                    exactClientMetricsFrameCount = result.ExactClientMetricsFrameCount,
                    frameAdmissionCount = result.FrameAdmissionCount,
                    metricsFrameGenerationMismatchCount = result.MetricsFrameGenerationMismatchAdmissionCount,
                    metricsFrameExtentMismatchCount = result.MetricsFrameExtentMismatchAdmissionCount,
                    staleMetricsAdmissionCount = result.StaleMetricsAdmissionCount,
                    staleFrameAdmissionCount = result.StaleFrameAdmissionCount,
                    repeatedIdenticalSizeSurfaceRecreateCount = result.RepeatedIdenticalSizeSurfaceRecreateCount,
                    zeroSizedSurfaceRecreateCount = result.ZeroSizedSurfaceRecreateCount,
                    suspensionHandled = result.SuspensionCount > 0 && result.RestoreCount > 0,
                    suspensionCount = result.SuspensionCount,
                    restoreCount = result.RestoreCount,
                    childDpiAndDisplayRequeryCount = result.DpiAndDisplayRequeryCount,
                    nativeChildDpiAndDisplayProbeCount = result.NativeChildDpiAndDisplayProbeCount,
                    repeatedIdenticalObservationCount = result.RepeatedIdenticalObservationCount,
                    rejectedStaleMetricsCount = result.RejectedStaleMetricsCount,
                    rejectedStaleFrameCount = result.RejectedStaleFrameCount,
                    rejectedFrameGenerationMismatchCount = result.RejectedFrameGenerationMismatchCount,
                    rejectedFrameExtentMismatchCount = result.RejectedFrameExtentMismatchCount,
                    minimumWidthPx = result.MinimumWidthPx,
                    minimumHeightPx = result.MinimumHeightPx,
                    maximumWidthPx = result.MaximumWidthPx,
                    maximumHeightPx = result.MaximumHeightPx,
                },
                dpiMatrix = result.DpiMatrix.Select(value => new
                {
                    scalePercent = value.ScalePercent,
                    dpi = value.Dpi,
                    devicePixelRatio = value.Dpi / 96.0,
                    cycles = value.Cycles,
                    clientWidthPx = value.ClientWidthPx,
                    clientHeightPx = value.ClientHeightPx,
                    metricsWidthPx = value.MetricsWidthPx,
                    metricsHeightPx = value.MetricsHeightPx,
                    frameWidthPx = value.FrameWidthPx,
                    frameHeightPx = value.FrameHeightPx,
                    metricsGeneration = value.MetricsGeneration,
                    frameGeneration = value.FrameGeneration,
                    actualClientEqualsMetrics = value.ActualClientEqualsMetrics,
                    metricsEqualsFrame = value.MetricsEqualsFrame,
                    generationPreserved = value.GenerationPreserved,
                    repeatedIdenticalSizeSurfaceRecreateCount = value.RepeatedIdenticalSizeSurfaceRecreateCount,
                    staleAdmissionCount = value.StaleAdmissionCount,
                    mismatchAdmissionCount = value.MismatchAdmissionCount,
                }),
                resources = new
                {
                    gdiBefore = result.GdiBefore,
                    gdiAfter = result.GdiAfter,
                    userBefore = result.UserBefore,
                    userAfter = result.UserAfter,
                    boundedAfterWarmup = result.GuiResourcesBoundedAfterWarmup,
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
                    matrixCycles = RequiredCycles,
                    matrixPassCount = RequiredCycles * Matrix.Length,
                    matrixFailureCount = 0,
                },
                scopeBoundary = "F3 proves child-client physical metrics, DPI/display re-observation, immutable generations, frame admission, suspension, and deterministic DPI inputs. F4 owns EGL window-surface recreation and F5+ own resize/present completion.",
            };
            var json = JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true });
            if (!string.IsNullOrWhiteSpace(options.EvidencePath))
            {
                var evidencePath = System.IO.Path.GetFullPath(options.EvidencePath);
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(evidencePath) ??
                    throw new InvalidOperationException("The evidence path has no parent directory."));
                File.WriteAllText(evidencePath, json + Environment.NewLine);
            }
            Console.WriteLine(json);
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"windowsappsdk-flutter-metrics FAIL: {exception}");
            return 1;
        }
    }

    private static F3CycleSummary RunCycles()
    {
        _ = RunOneCycle(-1); // Warm class/runtime state before GUI resource sampling.
        var gdiBefore = GetGuiResources(GuiResourceType.Gdi);
        var userBefore = GetGuiResources(GuiResourceType.User);
        var matrix = Matrix.ToDictionary(
            value => value.ScalePercent,
            value => new MatrixAccumulator(value.ScalePercent, value.Dpi));
        F3CycleResult? last = null;
        long generationCount = 0;
        long observationCount = 0;
        long dpiAndDisplayRequeryCount = 0;
        long nativeChildDpiAndDisplayProbeCount = 0;
        long repeatedIdenticalObservationCount = 0;
        long exactClientMetricsFrameCount = 0;
        long frameAdmissionCount = 0;
        long suspensionCount = 0;
        long restoreCount = 0;
        long rejectedStaleMetricsCount = 0;
        long rejectedStaleFrameCount = 0;
        long rejectedFrameGenerationMismatchCount = 0;
        long rejectedFrameExtentMismatchCount = 0;
        var generationMonotonic = true;
        for (var cycle = 0; cycle < RequiredCycles; cycle++)
        {
            last = RunOneCycle(cycle);
            foreach (var sample in last.DpiMatrix)
                matrix[sample.ScalePercent].Record(sample);
            generationCount += last.GenerationCount;
            observationCount += last.ObservationCount;
            dpiAndDisplayRequeryCount += last.DpiAndDisplayRequeryCount;
            nativeChildDpiAndDisplayProbeCount += last.NativeChildDpiAndDisplayProbeCount;
            repeatedIdenticalObservationCount += last.RepeatedIdenticalObservationCount;
            exactClientMetricsFrameCount += last.ExactClientMetricsFrameCount;
            frameAdmissionCount += last.FrameAdmissionCount;
            suspensionCount += last.SuspensionCount;
            restoreCount += last.RestoreCount;
            rejectedStaleMetricsCount += last.RejectedStaleMetricsCount;
            rejectedStaleFrameCount += last.RejectedStaleFrameCount;
            rejectedFrameGenerationMismatchCount += last.RejectedFrameGenerationMismatchCount;
            rejectedFrameExtentMismatchCount += last.RejectedFrameExtentMismatchCount;
            generationMonotonic &= last.GenerationMonotonic;
        }
        var gdiAfter = GetGuiResources(GuiResourceType.Gdi);
        var userAfter = GetGuiResources(GuiResourceType.User);
        var bounded = gdiAfter <= gdiBefore + 2 && userAfter <= userBefore + 2;
        if (!bounded)
        {
            throw new InvalidOperationException(
                $"F3 GUI resources grew after warmup: GDI {gdiBefore}->{gdiAfter}, USER {userBefore}->{userAfter}.");
        }
        var result = last ?? throw new InvalidOperationException("No F3 cycle was run.");
        if (!generationMonotonic || suspensionCount == 0 || restoreCount == 0 ||
            rejectedStaleMetricsCount == 0 || rejectedStaleFrameCount == 0 ||
            rejectedFrameExtentMismatchCount == 0)
        {
            throw new InvalidOperationException("F3 did not exercise generation, suspension, and stale/mismatched admission rejection.");
        }
        return new F3CycleSummary(
            RequiredCycles,
            0,
            result.ViewId,
            result.DisplayId,
            generationMonotonic,
            generationCount,
            observationCount,
            dpiAndDisplayRequeryCount,
            nativeChildDpiAndDisplayProbeCount,
            repeatedIdenticalObservationCount,
            exactClientMetricsFrameCount,
            frameAdmissionCount,
            suspensionCount,
            restoreCount,
            rejectedStaleMetricsCount,
            rejectedStaleFrameCount,
            rejectedFrameGenerationMismatchCount,
            rejectedFrameExtentMismatchCount,
            matrix.Values.OrderBy(value => value.ScalePercent).Select(value => value.ToSample()).ToArray(),
            gdiBefore,
            gdiAfter,
            userBefore,
            userAfter,
            bounded,
            result.MauiOrXamlAssemblyNames,
            result.MinimumWidthPx,
            result.MinimumHeightPx,
            result.MaximumWidthPx,
            result.MaximumHeightPx);
    }

    private static F3CycleResult RunOneCycle(int cycle)
    {
        const int logicalWidth = 641;
        const int logicalHeight = 359;
        const ulong baseViewId = 0xF300_0000_0000_0000;
        var constraints = new FlutterWindowsPhysicalConstraints(120, 90, 2400, 1600);
        var displaySource = new MatrixDisplayObservationSource(96, "matrix-display-100");
        var bootstrap = new FlutterWindowsAppSdkBootstrap();
        FlutterWindowsHostWindow? host = null;
        FlutterWindowsViewMetricsCoordinator? coordinator = null;
        try
        {
            bootstrap.InitializeOnCurrentThread();
            host = FlutterWindowsHostWindow.CreateOnCurrentThread(
                bootstrap,
                new FlutterWindowsHostWindowOptions(
                    $"Doroti F3 validation {cycle}",
                    InitialClientWidth: logicalWidth,
                    InitialClientHeight: logicalHeight,
                    MinimumClientWidth: constraints.MinimumPhysicalWidth,
                    MinimumClientHeight: constraints.MinimumPhysicalHeight,
                    MaximumClientWidth: constraints.MaximumPhysicalWidth,
                    MaximumClientHeight: constraints.MaximumPhysicalHeight,
                    InitialX: -32000,
                    InitialY: -32000));

            using (var nativeProbe = new FlutterWindowsViewMetricsCoordinator(
                       host.ViewHwnd, baseViewId + 0x10000UL + (ulong)(cycle + 1), constraints))
            {
                var nativeMetrics = nativeProbe.ObserveChildMetrics();
                if (nativeMetrics.PhysicalWidth <= 0 || nativeMetrics.PhysicalHeight <= 0 ||
                    string.IsNullOrWhiteSpace(nativeMetrics.DisplayId) || nativeMetrics.DevicePixelRatio <= 0)
                {
                    throw new InvalidOperationException("F3 native child DPI/display observation was incomplete.");
                }
            }

            coordinator = FlutterWindowsViewMetricsCoordinator.AttachToHostWindow(
                host,
                baseViewId + (ulong)(cycle + 1),
                constraints,
                displaySource);
            var samples = new List<MatrixSample>(Matrix.Length);
            WindowsViewMetrics? firstMetrics = null;
            DorotiFrameDescriptor? firstFrame = null;
            long previousGeneration = coordinator.Current.ResizeGeneration;
            var generationMonotonic = true;

            foreach (var matrixCase in Matrix)
            {
                var dpr = matrixCase.Dpi / 96.0;
                var expectedWidth = WindowsViewMetrics.LogicalToPhysical(logicalWidth, dpr);
                var expectedHeight = WindowsViewMetrics.LogicalToPhysical(logicalHeight, dpr);
                displaySource.Set(matrixCase.Dpi, $"matrix-display-{matrixCase.ScalePercent}");
                SetTopLevelClientSize(host.TopLevelHwnd, expectedWidth, expectedHeight);
                var metrics = coordinator.ObserveChildMetrics();
                var actual = GetChildClientRect(host.ViewHwnd);
                var frameworkMetrics = metrics.ToViewMetrics(surfaceGeneration: 0);
                var viewEpoch = metrics.ToViewEpoch();
                var resizeEpoch = metrics.ToResizeEpoch();
                var descriptor = metrics.CreateFrameDescriptor(
                    frameworkFrameNumber: (cycle + 1) * 10L + matrixCase.ScalePercent,
                    sceneSequence: (cycle + 1) * 1000L + matrixCase.ScalePercent);
                if (!coordinator.TryAdmitFrame(descriptor, out var exactMatch) || !exactMatch.IsExact)
                    throw new InvalidOperationException("F3 rejected an exact child-client frame.");
                var repeated = coordinator.ObserveChildMetrics();
                var clientEqualsMetrics = actual.Width == metrics.PhysicalWidth &&
                    actual.Height == metrics.PhysicalHeight &&
                    frameworkMetrics.physicalSize.width == metrics.PhysicalWidth &&
                    frameworkMetrics.physicalSize.height == metrics.PhysicalHeight;
                var metricsEqualsFrame = descriptor.PhysicalWidth == metrics.PhysicalWidth &&
                    descriptor.PhysicalHeight == metrics.PhysicalHeight &&
                    descriptor.RootPhysicalWidth == metrics.PhysicalWidth &&
                    descriptor.RootPhysicalHeight == metrics.PhysicalHeight;
                var generationPreserved = descriptor.ResizeTargetGeneration == metrics.ResizeGeneration &&
                    descriptor.MetricsGeneration == metrics.ResizeGeneration &&
                    viewEpoch.ResizeTargetGeneration == metrics.ResizeGeneration &&
                    viewEpoch.MetricsGeneration == metrics.ResizeGeneration &&
                    resizeEpoch.Generation == metrics.ResizeGeneration &&
                    repeated.ResizeGeneration == metrics.ResizeGeneration;
                if (!clientEqualsMetrics || !metricsEqualsFrame || !generationPreserved ||
                    metrics.State != WindowsViewMetricsState.Active ||
                    metrics.DisplayId != $"matrix-display-{matrixCase.ScalePercent}" ||
                    Math.Abs(metrics.DevicePixelRatio - dpr) > double.Epsilon)
                {
                    throw new InvalidOperationException("F3 matrix observation drifted from child-client physical authority.");
                }
                generationMonotonic &= metrics.ResizeGeneration >= previousGeneration;
                previousGeneration = metrics.ResizeGeneration;
                firstMetrics ??= metrics;
                firstFrame ??= descriptor;
                samples.Add(new MatrixSample(
                    matrixCase.ScalePercent,
                    matrixCase.Dpi,
                    Cycles: 1,
                    actual.Width,
                    actual.Height,
                    metrics.PhysicalWidth,
                    metrics.PhysicalHeight,
                    descriptor.PhysicalWidth,
                    descriptor.PhysicalHeight,
                    metrics.ResizeGeneration,
                    descriptor.ResizeTargetGeneration,
                    clientEqualsMetrics,
                    metricsEqualsFrame,
                    generationPreserved,
                    RepeatedIdenticalSizeSurfaceRecreateCount: 0,
                    StaleAdmissionCount: 0,
                    MismatchAdmissionCount: 0));
            }

            var finalMatrixCase = Matrix[^1];
            MinimizeTopLevel(host.TopLevelHwnd, host.ViewHwnd);
            var suspended = coordinator.ObserveChildMetrics();
            if (suspended.State != WindowsViewMetricsState.Suspended || suspended.HasDrawableSize ||
                coordinator.TryAdmitFrame(firstFrame!, out _))
            {
                throw new InvalidOperationException("F3 zero-sized child HWND did not suspend without frame admission.");
            }
            if (coordinator.TryAdmitMetrics(firstMetrics!))
                throw new InvalidOperationException("F3 admitted stale metrics after zero-sized suspension.");

            var finalDpr = finalMatrixCase.Dpi / 96.0;
            displaySource.Set(finalMatrixCase.Dpi, $"matrix-display-{finalMatrixCase.ScalePercent}");
            _ = ShowWindow(host.TopLevelHwnd, SwRestore);
            SetTopLevelClientSize(
                host.TopLevelHwnd,
                WindowsViewMetrics.LogicalToPhysical(logicalWidth, finalDpr),
                WindowsViewMetrics.LogicalToPhysical(logicalHeight, finalDpr));
            _ = ShowWindow(host.TopLevelHwnd, SwHide);
            var restored = coordinator.ObserveChildMetrics();
            if (restored.State != WindowsViewMetricsState.Active ||
                restored.ResizeGeneration <= suspended.ResizeGeneration ||
                coordinator.TryAdmitMetrics(firstMetrics!) ||
                coordinator.TryAdmitFrame(firstFrame!, out _))
            {
                throw new InvalidOperationException("F3 restore did not reject stale metrics/frame generations.");
            }
            var exactRestoredFrame = restored.CreateFrameDescriptor(
                frameworkFrameNumber: (cycle + 1) * 10_000L + 1,
                sceneSequence: (cycle + 1) * 10_000L + 2);
            var wrongExtentFrame = exactRestoredFrame with
            {
                PhysicalWidth = exactRestoredFrame.PhysicalWidth + 1,
                RootPhysicalWidth = exactRestoredFrame.RootPhysicalWidth + 1,
            };
            if (coordinator.TryAdmitFrame(wrongExtentFrame, out _))
                throw new InvalidOperationException("F3 admitted a physical extent mismatch.");
            if (!coordinator.TryAdmitFrame(exactRestoredFrame, out var restoredMatch) || !restoredMatch.IsExact)
                throw new InvalidOperationException("F3 rejected the restored exact child-client frame.");

            var snapshot = coordinator.Snapshot;
            if (snapshot.RepeatedIdenticalSizeSurfaceRecreateCount != 0 ||
                snapshot.ZeroSizedSurfaceRecreateCount != 0 ||
                snapshot.MetricsFrameGenerationMismatchAdmissionCount != 0 ||
                snapshot.MetricsFrameExtentMismatchAdmissionCount != 0 ||
                snapshot.StaleMetricsAdmissionCount != 0 || snapshot.StaleFrameAdmissionCount != 0 ||
                snapshot.DpiAndDisplayRequeryCount == 0 || snapshot.SuspensionCount == 0 ||
                snapshot.RestoreCount == 0 || snapshot.RejectedStaleMetricsCount == 0 ||
                snapshot.RejectedStaleFrameCount == 0 || snapshot.RejectedFrameExtentMismatchCount == 0)
            {
                throw new InvalidOperationException("F3 metrics/frame admission counters violate their contract.");
            }
            coordinator.Dispose();
            coordinator = null;
            host.Dispose();
            host = null;
            return new F3CycleResult(
                baseViewId + (ulong)(cycle + 1),
                snapshot.Current.DisplayId,
                generationMonotonic,
                snapshot.Current.ResizeGeneration,
                snapshot.ObservationCount,
                snapshot.DpiAndDisplayRequeryCount,
                NativeChildDpiAndDisplayProbeCount: 1,
                snapshot.RepeatedIdenticalObservationCount,
                ExactClientMetricsFrameCount: Matrix.Length,
                snapshot.ExactFrameAdmissionCount,
                snapshot.SuspensionCount,
                snapshot.RestoreCount,
                snapshot.RejectedStaleMetricsCount,
                snapshot.RejectedStaleFrameCount,
                snapshot.RejectedFrameGenerationMismatchCount,
                snapshot.RejectedFrameExtentMismatchCount,
                samples.ToArray(),
                CaptureMauiOrXamlAssemblyNames(),
                snapshot.Current.MinimumPhysicalWidth,
                snapshot.Current.MinimumPhysicalHeight,
                snapshot.Current.MaximumPhysicalWidth,
                snapshot.Current.MaximumPhysicalHeight);
        }
        finally
        {
            coordinator?.Dispose();
            if (host is not null) host.Dispose();
            else if (bootstrap.Snapshot.Phase != FlutterWindowsAppSdkBootstrapPhase.Disposed)
                bootstrap.DisposeOnCurrentThread();
        }
    }

    /// <summary>
    /// Drives the normal F2 top-level WM_SIZE path.  The matrix never resizes
    /// the child directly: after client-to-outer conversion, F2 reads the top
    /// client rect and lays out the one child view before F3 observes it.
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
                $"AdjustWindowRectExForDpi F3 client resize failed: {Marshal.GetLastWin32Error()}");
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
            throw new InvalidOperationException($"SetWindowPos F3 top-level resize failed: {Marshal.GetLastWin32Error()}");
        }
        PumpPendingMessages();
    }

    private static void MinimizeTopLevel(nint topLevelHwnd, nint childHwnd)
    {
        _ = ShowWindow(topLevelHwnd, SwMinimize);
        PumpPendingMessages();
        var childRect = GetChildClientRect(childHwnd);
        if (childRect.Width != 0 || childRect.Height != 0)
        {
            throw new InvalidOperationException(
                $"F3 minimized top-level did not produce a zero-sized child client rect: {childRect.Width}x{childRect.Height}.");
        }
    }

    private static NativeRect GetChildClientRect(nint childHwnd)
    {
        if (!GetClientRect(childHwnd, out var rect))
            throw new InvalidOperationException($"GetClientRect F3 child metrics failed: {Marshal.GetLastWin32Error()}");
        return rect;
    }

    private static void PumpPendingMessages()
    {
        while (PeekMessageW(out var message, 0, 0, 0, 1))
        {
            if (message.Message == 0x0012)
                throw new InvalidOperationException("F3 validation unexpectedly received WM_QUIT.");
            _ = TranslateMessage(in message);
            _ = DispatchMessageW(in message);
        }
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

    private static T RunOnDedicatedStaThread<T>(Func<T> action)
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
            Name = "Doroti F3 same-STA validation",
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        var timeout = TimeSpan.FromMinutes(20);
        if (!complete.Wait(timeout) || !thread.Join(timeout))
            throw new TimeoutException("F3 same-STA validation did not terminate within 20 minutes.");
        if (failure is not null)
            throw new InvalidOperationException("F3 same-STA validation failed.", failure);
        return result ?? throw new InvalidOperationException("F3 validation returned no result.");
    }

    private static F3Options ParseOptions(string[] args)
    {
        string? evidencePath = null;
        string? sourceFingerprint = null;
        string? publishedExecutableSha256 = null;
        for (var index = 0; index < args.Length; index += 2)
        {
            if (index + 1 >= args.Length)
                throw new ArgumentException("Every F3 validator option requires a value.");
            switch (args[index])
            {
                case "--evidence": evidencePath = args[index + 1]; break;
                case "--source-fingerprint": sourceFingerprint = args[index + 1]; break;
                case "--published-executable-sha256": publishedExecutableSha256 = args[index + 1]; break;
                default:
                    throw new ArgumentException(
                        "Usage: Doroti.Validation.WindowsAppSdkFlutterMetrics " +
                        "[--evidence <path>] [--source-fingerprint <sha256>] " +
                        "[--published-executable-sha256 <sha256>]");
            }
        }
        return new F3Options(evidencePath, sourceFingerprint, publishedExecutableSha256);
    }

    private static string ComputeFileHash(string path) =>
        Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();

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

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    private static partial nint GetWindowLongPtrW(nint hwnd, int index);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(nint hwnd, int command);

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
    private static partial bool GetClientRect(nint hwnd, out NativeRect rect);

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

    private sealed class MatrixDisplayObservationSource : IFlutterWindowsDisplayObservationSource
    {
        private int _dpi;
        private string _displayId;

        internal MatrixDisplayObservationSource(int dpi, string displayId)
        {
            _dpi = dpi;
            _displayId = displayId;
        }

        internal void Set(int dpi, string displayId)
        {
            _dpi = dpi;
            _displayId = displayId;
        }

        public FlutterWindowsDisplayObservation Observe(nint childHwnd) => new(_dpi, _displayId);
    }

    private sealed class MatrixAccumulator
    {
        private MatrixSample? _last;

        internal MatrixAccumulator(int scalePercent, int dpi)
        {
            ScalePercent = scalePercent;
            Dpi = dpi;
        }

        internal int ScalePercent { get; }
        internal int Dpi { get; }
        internal int Cycles { get; private set; }
        internal bool ActualClientEqualsMetrics { get; private set; } = true;
        internal bool MetricsEqualsFrame { get; private set; } = true;
        internal bool GenerationPreserved { get; private set; } = true;

        internal void Record(MatrixSample sample)
        {
            if (sample.ScalePercent != ScalePercent || sample.Dpi != Dpi)
                throw new InvalidOperationException("F3 matrix rows were recorded under the wrong DPI case.");
            Cycles++;
            ActualClientEqualsMetrics &= sample.ActualClientEqualsMetrics;
            MetricsEqualsFrame &= sample.MetricsEqualsFrame;
            GenerationPreserved &= sample.GenerationPreserved;
            _last = sample;
        }

        internal MatrixSample ToSample()
        {
            var last = _last ?? throw new InvalidOperationException("F3 matrix case received no samples.");
            if (Cycles != RequiredCycles)
                throw new InvalidOperationException($"F3 DPI {ScalePercent}% ran {Cycles}, expected {RequiredCycles} cycles.");
            return last with
            {
                Cycles = Cycles,
                ActualClientEqualsMetrics = ActualClientEqualsMetrics,
                MetricsEqualsFrame = MetricsEqualsFrame,
                GenerationPreserved = GenerationPreserved,
            };
        }
    }

    private sealed record F3Options(
        string? EvidencePath,
        string? SourceFingerprint,
        string? PublishedExecutableSha256);

    private sealed record MatrixCase(int ScalePercent, int Dpi);

    private sealed record MatrixSample(
        int ScalePercent,
        int Dpi,
        int Cycles,
        int ClientWidthPx,
        int ClientHeightPx,
        int MetricsWidthPx,
        int MetricsHeightPx,
        int FrameWidthPx,
        int FrameHeightPx,
        long MetricsGeneration,
        long FrameGeneration,
        bool ActualClientEqualsMetrics,
        bool MetricsEqualsFrame,
        bool GenerationPreserved,
        long RepeatedIdenticalSizeSurfaceRecreateCount,
        long StaleAdmissionCount,
        long MismatchAdmissionCount);

    private sealed record F3CycleResult(
        ulong ViewId,
        string DisplayId,
        bool GenerationMonotonic,
        long GenerationCount,
        long ObservationCount,
        long DpiAndDisplayRequeryCount,
        long NativeChildDpiAndDisplayProbeCount,
        long RepeatedIdenticalObservationCount,
        long ExactClientMetricsFrameCount,
        long FrameAdmissionCount,
        long SuspensionCount,
        long RestoreCount,
        long RejectedStaleMetricsCount,
        long RejectedStaleFrameCount,
        long RejectedFrameGenerationMismatchCount,
        long RejectedFrameExtentMismatchCount,
        MatrixSample[] DpiMatrix,
        string[] MauiOrXamlAssemblyNames,
        int MinimumWidthPx,
        int MinimumHeightPx,
        int MaximumWidthPx,
        int MaximumHeightPx);

    private sealed record F3CycleSummary(
        int Cycles,
        int FailureCount,
        ulong ViewId,
        string DisplayId,
        bool GenerationMonotonic,
        long GenerationCount,
        long ObservationCount,
        long DpiAndDisplayRequeryCount,
        long NativeChildDpiAndDisplayProbeCount,
        long RepeatedIdenticalObservationCount,
        long ExactClientMetricsFrameCount,
        long FrameAdmissionCount,
        long SuspensionCount,
        long RestoreCount,
        long RejectedStaleMetricsCount,
        long RejectedStaleFrameCount,
        long RejectedFrameGenerationMismatchCount,
        long RejectedFrameExtentMismatchCount,
        MatrixSample[] DpiMatrix,
        int GdiBefore,
        int GdiAfter,
        int UserBefore,
        int UserAfter,
        bool GuiResourcesBoundedAfterWarmup,
        string[] MauiOrXamlAssemblyNames,
        int MinimumWidthPx,
        int MinimumHeightPx,
        int MaximumWidthPx,
        int MaximumHeightPx)
    {
        internal long MetricsFrameGenerationMismatchAdmissionCount => 0;
        internal long MetricsFrameExtentMismatchAdmissionCount => 0;
        internal long StaleMetricsAdmissionCount => 0;
        internal long StaleFrameAdmissionCount => 0;
        internal long RepeatedIdenticalSizeSurfaceRecreateCount => 0;
        internal long ZeroSizedSurfaceRecreateCount => 0;
    }
}
