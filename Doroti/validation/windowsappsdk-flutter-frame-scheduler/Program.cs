using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using UiColor = Doroti.Ui.Color;

namespace Doroti.Validation.WindowsAppSdkFlutterFrameScheduler;

internal static partial class Program
{
    private const int RequiredCadenceSamples = 12;
    private const int GwlStyle = -16;
    private const int GwlExStyle = -20;
    private const uint SwpNoZOrder = 0x0004;
    private const uint SwpNoActivate = 0x0010;
    private const string EvidenceSchema =
        "doroti.windowsappsdk-flutter-frame-scheduler-evidence/v1";
    private static readonly TimeSpan TestTimeout = TimeSpan.FromMinutes(20);

    private static int Main(string[] args)
    {
        var options = ParseOptions(args);
        try
        {
            // Warm the actual Windows App SDK, ANGLE, Skia, DWM, and MTA-raster
            // graph before measuring process GUI resources. The following two
            // view runs are independent per-view schedulers, not a claim that a
            // synthetic cadence matrix measures visible scan-out.
            _ = RunOnDedicatedPlatformStaThread(() =>
                RunRuntimeView("warmup", 60_001, runLifecycleExercise: false));
            var gdiBefore = GetGuiResources(GuiResourceType.Gdi);
            var userBefore = GetGuiResources(GuiResourceType.User);

            var primary = RunOnDedicatedPlatformStaThread(() =>
                RunRuntimeView("primary", 60_101, runLifecycleExercise: true));
            var secondary = RunOnDedicatedPlatformStaThread(() =>
                RunRuntimeView("secondary", 60_102, runLifecycleExercise: true));
            var cadence = RunCadenceMatrix();

            var gdiAfter = GetGuiResources(GuiResourceType.Gdi);
            var userAfter = GetGuiResources(GuiResourceType.User);
            var guiResourcesBounded = gdiAfter <= gdiBefore + 4 && userAfter <= userBefore + 4;
            Assert(guiResourcesBounded,
                $"F6 GUI resources grew after warmup: GDI {gdiBefore}->{gdiAfter}, USER {userBefore}->{userAfter}.");

            AssertRuntimeView(primary);
            AssertRuntimeView(secondary);
            Assert(primary.Scheduler.ViewId != secondary.Scheduler.ViewId,
                "F6 primary and secondary schedulers unexpectedly share a view id.");
            Assert(primary.Scheduler.CrossViewLeakCount == 0 && secondary.Scheduler.CrossViewLeakCount == 0,
                "F6 per-view scheduler state leaked across views.");

            var sourceFingerprint = options.SourceFingerprint;
            var executablePath = Environment.ProcessPath ?? throw new InvalidOperationException(
                "The F6 validator executable path is unavailable.");
            var executableHash = ComputeFileHash(executablePath);
            if (!string.IsNullOrWhiteSpace(options.PublishedExecutableSha256) &&
                !string.Equals(executableHash, options.PublishedExecutableSha256,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The F6 validator executable hash does not match the publish-gate input.");
            }

            var runtimeViews = new[] { primary, secondary };
            var primaryTrace = RequirePresentedTrace(primary);
            var secondaryTrace = RequirePresentedTrace(secondary);
            var runtimeSampleCount = runtimeViews.Sum(view =>
                view.DwmTiming.NativeTimingSampleCount + view.DwmTiming.FallbackSampleCount);
            var runtimeRefreshHz = checked((long)Math.Max(1, Math.Round(
                runtimeViews.Max(view => view.DwmTiming.LastRefreshRateHz),
                MidpointRounding.AwayFromZero)));
            var runtimeNativeTimingSampleCount = runtimeViews.Sum(view =>
                view.DwmTiming.NativeTimingSampleCount);
            var runtimeFallbackSampleCount = runtimeViews.Sum(view =>
                view.DwmTiming.FallbackSampleCount);
            Assert(runtimeNativeTimingSampleCount >= 1,
                "F6 requires at least one native DwmGetCompositionTimingInfo sample; fallback cadence alone is diagnostic only.");
            Assert(runtimeViews.All(view => view.DwmTiming.UsesDesktopCompositionTiming &&
                view.DwmTiming.DwmTimingCallHwnd == nint.Zero),
                "F6 DWM timing must use the documented desktop NULL HWND target, not a child or top-level view HWND.");
            Assert(runtimeViews.All(view => view.DwmTiming.LastHResult >= 0),
                "F6 DWM timing did not finish with a successful HRESULT for every runtime view.");
            var activeSurface = primary.ActiveSurface;
            var mauiOrXamlAssemblyNames = CaptureMauiOrXamlAssemblyNames();

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
                    managedThreadId = primary.PlatformManagedThreadId,
                },
                rasterThread = new
                {
                    apartment = "MTA",
                    executionMode = "dedicated-raster-thread",
                    managedThreadId = primary.RasterRunner.ManagedThreadId,
                    maxQueueDepth = Math.Max(primary.RasterRunner.MaxObservedQueueDepth,
                        secondary.RasterRunner.MaxObservedQueueDepth),
                },
                vsync = new
                {
                    source = "DwmGetCompositionTimingInfo",
                    evidenceKind = "scheduler-timing-not-scanout",
                    swapInterval = 1,
                    runtimeSampleCount,
                    runtimeRefreshHz,
                    runtimeNativeTimingSampleCount,
                    runtimeFallbackSampleCount,
                    usesDesktopCompositionTiming = runtimeViews.All(view =>
                        view.DwmTiming.UsesDesktopCompositionTiming),
                    dwmTimingCallHwnds = runtimeViews.Select(view =>
                        view.DwmTiming.DwmTimingCallHwnd == nint.Zero
                            ? "NULL"
                            : $"0x{view.DwmTiming.DwmTimingCallHwnd.ToInt64():X}").ToArray(),
                    boundChildHwnds = runtimeViews.Select(view =>
                        $"0x{view.DwmTiming.ChildHwnd.ToInt64():X}").ToArray(),
                    runtimeLastHResults = runtimeViews.Select(view =>
                        view.DwmTiming.LastHResult).ToArray(),
                },
                scheduler = new
                {
                    viewCount = runtimeViews.Length,
                    animationStarvationCount = runtimeViews.Sum(view => view.Scheduler.AnimationStarvationCount),
                    resizeStarvationCount = runtimeViews.Sum(view => view.Scheduler.ResizeStarvationCount),
                    frameQueueOverflowCount = runtimeViews.Sum(view => view.Scheduler.FrameQueueOverflowCount),
                    maxObservedQueueDepth = runtimeViews.Max(view => view.Scheduler.MaxObservedQueueDepth),
                    staleOrWrongSizePresentCount = runtimeViews.Sum(view => view.Scheduler.StaleOrWrongSizePresentCount),
                    pendingResizeOrdinaryRejectedCount = runtimeViews.Sum(view => view.Scheduler.PendingResizeOrdinaryRejectedCount),
                    causalGapCount = runtimeViews.Sum(view => view.Scheduler.CausalGapCount +
                        view.ScheduledRaster.CausalReceiptMismatchCount),
                    callbackCount = runtimeViews.Sum(view => view.Scheduler.CallbackCount),
                    rasterCount = runtimeViews.Sum(view => view.ScheduledRaster.RasterCount),
                    swapCount = runtimeViews.Sum(view => view.ScheduledRaster.SwapCount),
                    presentedCount = runtimeViews.Sum(view => view.ScheduledRaster.PresentedReceiptCount),
                    ordinaryResumeCount = runtimeViews.Sum(view => view.Scheduler.OrdinaryResumeCount),
                    hiddenStopCount = runtimeViews.Sum(view => view.Scheduler.HiddenStopCount),
                    minimizedStopCount = runtimeViews.Sum(view => view.Scheduler.MinimizedStopCount),
                    suspendedStopCount = runtimeViews.Sum(view => view.Scheduler.SuspendedStopCount),
                    restoredLatestMetricsCount = runtimeViews.Sum(view => view.Scheduler.RestoredLatestMetricsCount),
                    multiViewStateIsolated = true,
                    pendingResizePriorityApplied = runtimeViews.All(view =>
                        view.Scheduler.PendingResizeOrdinaryRejectedCount > 0),
                    ordinaryResumeWithinBound = runtimeViews.All(view =>
                        view.Scheduler.OrdinaryResumeCount > 0),
                },
                lifecycle = new
                {
                    hiddenSchedulingStopped = runtimeViews.All(view => view.HiddenSchedulingStopped),
                    minimizedSchedulingStopped = runtimeViews.All(view => view.MinimizedSchedulingStopped),
                    suspendedSchedulingStopped = runtimeViews.All(view => view.SuspendedSchedulingStopped),
                    resumedWithLatestMetrics = runtimeViews.All(view => view.ResumedWithLatestMetrics),
                },
                cadenceMatrix = cadence.Select(row => new
                {
                    refreshHz = row.RefreshHz,
                    deterministicContract = true,
                    scheduledFrameCount = row.ScheduledFrameCount,
                    presentedFrameCount = row.PresentedFrameCount,
                    targetCadenceMet = row.TargetCadenceMet,
                    animationStarvationCount = row.AnimationStarvationCount,
                    resizeStarvationCount = row.ResizeStarvationCount,
                    maxQueueDepth = row.MaxQueueDepth,
                    staleOrWrongSizePresentCount = row.StaleOrWrongSizePresentCount,
                    ordinaryResumedAfterResize = row.OrdinaryResumedAfterResize,
                }).ToArray(),
                views = new[]
                {
                    ToViewEvidence("primary", primary),
                    ToViewEvidence("secondary", secondary),
                },
                causalChains = new[]
                {
                    ToCausalEvidence("primary", primaryTrace),
                    ToCausalEvidence("secondary", secondaryTrace),
                },
                resources = new
                {
                    eglSurfaceLeakCount = runtimeViews.Sum(view => view.DisposedSurface.EglSurfaceLeakCount),
                    skiaResourceLeakCount = runtimeViews.Sum(view => view.DisposedSurface.SkiaResourceLeakCount),
                    gdiBefore,
                    gdiAfter,
                    userBefore,
                    userAfter,
                    boundedAfterWarmup = guiResourcesBounded,
                },
                renderer = new
                {
                    name = activeSurface.Renderer,
                    classification = activeSurface.SoftwareFallback ? "software" : "hardware",
                    softwareFallback = activeSurface.SoftwareFallback,
                    visibleAcceptanceClaim = false,
                    hardwareVisiblePassClaim = false,
                },
                startup = new
                {
                    mauiOrXamlAssemblyCount = mauiOrXamlAssemblyNames.Length,
                    mauiOrXamlAssemblyNames,
                    noMauiOrXamlWindowStartup = mauiOrXamlAssemblyNames.Length == 0,
                },
                validation = new
                {
                    sourceFingerprint,
                    executablePath,
                    executableSha256 = executableHash,
                    requiredCadenceSamples = RequiredCadenceSamples,
                },
                scopeBoundary = "F6 proves DWM-derived scheduler timing observations, deterministic scheduler cadence rules, bounded per-view queue admission, real F4 raster/swap causal traces, and state-stop/latest-metrics resume. It does not prove physical scan-out cadence, visible blank or white-frame absence, compositor continuity, input, lifecycle integration, or a hardware-visible pass.",
            };

            var json = JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true });
            if (!string.IsNullOrWhiteSpace(options.EvidencePath))
            {
                var evidencePath = System.IO.Path.GetFullPath(options.EvidencePath);
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(evidencePath) ??
                    throw new InvalidOperationException("The F6 evidence path has no parent directory."));
                File.WriteAllText(evidencePath, json + Environment.NewLine);
            }
            Console.WriteLine(json);
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"windowsappsdk-flutter-frame-scheduler FAIL: {exception}");
            return 1;
        }
    }

    private static RuntimeViewResult RunRuntimeView(
        string name,
        ulong viewId,
        bool runLifecycleExercise)
    {
        var bootstrap = new FlutterWindowsAppSdkBootstrap();
        FlutterWindowsHostWindow? host = null;
        FlutterWindowsViewMetricsCoordinator? metricsCoordinator = null;
        FlutterWindowsFrameScheduler? scheduler = null;
        FlutterWindowsDedicatedRasterTaskRunner? rasterRunner = null;
        FlutterWindowsAngleEglSharedContext? sharedContext = null;
        FlutterWindowsAngleEglWindowSurface? surface = null;
        SkiaSceneRenderer? renderer = null;
        FlutterWindowsScheduledRaster? scheduledRaster = null;
        FlutterWindowsAngleEglWindowSurfaceSnapshot? disposedSurface = null;
        var surfaceReleasedByHost = false;
        var completed = false;
        try
        {
            rasterRunner = new FlutterWindowsDedicatedRasterTaskRunner(
                $"Doroti F6 {name} dedicated MTA raster validation");
            var capturedRasterRunner = rasterRunner;
            host = FlutterWindowsHostWindow.CreateOnCurrentThread(
                bootstrap,
                new FlutterWindowsHostWindowOptions(
                    $"Doroti F6 {name} scheduler validation",
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
                        disposedSurface = AwaitWithinTimeout(capturedRasterRunner.RunAsync(() =>
                        {
                            scheduledRaster?.Dispose();
                            renderer?.Dispose();
                            try
                            {
                                surface?.Dispose();
                                return surface?.Snapshot;
                            }
                            finally
                            {
                                sharedContext?.Dispose();
                            }
                        }), "F6 view-surface teardown") ?? throw new InvalidOperationException(
                            "F6 did not retain a surface snapshot during view-surface teardown.");
                        surfaceReleasedByHost = true;
                    },
                });
            metricsCoordinator = FlutterWindowsViewMetricsCoordinator.AttachToHostWindow(
                host,
                viewId,
                new FlutterWindowsPhysicalConstraints(320, 240, 960, 720));
            var dwmVsync = new FlutterWindowsDwmVsyncSource(host.ViewHwnd);
            scheduler = new FlutterWindowsFrameScheduler(metricsCoordinator, dwmVsync);
            var capturedScheduler = scheduler;
            var rendererHost = new FixtureRendererHost(() => capturedScheduler.Snapshot.CurrentMetrics);
            renderer = new SkiaSceneRenderer(
                viewId,
                rendererHost,
                new UiColor(0xff1e293b),
                new UiColor(0xff0f172a),
                $"f6/{name}/raw-child-hwnd",
                "ANGLE-EGL",
                "windowsappsdk-flutter-frame-scheduler");

            var initialMetrics = metricsCoordinator.Current;
            sharedContext = AwaitWithinTimeout(capturedRasterRunner.RunAsync(
                FlutterWindowsAngleEglSharedContext.CreateOnCurrentRasterThread),
                "F6 shared EGL context creation");
            surface = AwaitWithinTimeout(capturedRasterRunner.RunAsync(() =>
                FlutterWindowsAngleEglWindowSurface.CreateOnCurrentRasterThread(
                    sharedContext,
                    host.ViewHwnd,
                    initialMetrics)), "F6 child EGL surface creation");
            scheduledRaster = new FlutterWindowsScheduledRaster(
                capturedScheduler,
                surface,
                renderer,
                capturedRasterRunner);
            var frameCallback = scheduledRaster.CreateFrameCallback();

            long frameworkFrameNumber = 0;
            long latestMetricsRequestCount = 0;
            var resumedWithLatestMetrics = true;
            capturedScheduler.LatestMetricsFrameRequested += metrics =>
            {
                Interlocked.Increment(ref latestMetricsRequestCount);
                SubmitExactScene(renderer, metrics, ref frameworkFrameNumber, $"f6/{name}/restore");
                var request = capturedScheduler.ScheduleOrdinary(metrics.ToViewEpoch(), frameCallback);
                if (!request.Accepted || !ReferenceEquals(request.Ticket.Metrics, metrics))
                {
                    resumedWithLatestMetrics = false;
                    throw new InvalidOperationException(
                        "F6 restored scheduler did not request the current immutable metrics publication.");
                }
            };

            DispatchOrdinary(capturedScheduler, capturedRasterRunner, renderer, initialMetrics,
                ref frameworkFrameNumber, $"f6/{name}/first", frameCallback);
            var beforeShow = host.Snapshot;
            Assert(!beforeShow.FirstFrameSwapped && !beforeShow.Shown,
                "F6 platform HWND showed before the first real scheduled F4 swap.");
            host.NotifyFirstFrameSwapped();
            var afterShow = host.Snapshot;
            Assert(afterShow.FirstFrameSwapped && afterShow.Shown && afterShow.FirstFrameShowCount == 1,
                "F6 did not preserve the first-exact-swap show gate.");

            // Exercise a pending ordinary callback being displaced by the exact
            // resize before any raster work is admitted. The queue remains one
            // entry and the ordinary callback is re-requested after resize.
            var priorityMetrics = metricsCoordinator.Current;
            SubmitExactScene(renderer, priorityMetrics, ref frameworkFrameNumber, $"f6/{name}/priority-ordinary");
            var ordinaryBeforeResize = capturedScheduler.ScheduleOrdinary(
                priorityMetrics.ToViewEpoch(), frameCallback);
            Assert(ordinaryBeforeResize.Accepted, "F6 could not enqueue the pending ordinary callback.");
            SubmitExactScene(renderer, priorityMetrics, ref frameworkFrameNumber, $"f6/{name}/priority-resize");
            var resizePriority = capturedScheduler.ScheduleResize(priorityMetrics, frameCallback);
            Assert(resizePriority.Accepted && resizePriority.ReplacedLatest,
                "F6 resize did not replace the pending ordinary frame.");
            DispatchExisting(capturedScheduler, capturedRasterRunner, $"f6/{name}/priority-resize");
            DispatchOrdinary(capturedScheduler, capturedRasterRunner, renderer, priorityMetrics,
                ref frameworkFrameNumber, $"f6/{name}/ordinary-after-priority-resize", frameCallback);

            foreach (var target in new[] { (641, 361), (643, 363), (645, 365) })
            {
                SetTopLevelClientSize(host.TopLevelHwnd, target.Item1, target.Item2);
                var metrics = metricsCoordinator.ObserveChildMetrics();
                Assert(metrics.PhysicalWidth == target.Item1 && metrics.PhysicalHeight == target.Item2,
                    "F6 top-level resize did not publish the exact F3 child-client dimensions.");
                DispatchResize(capturedScheduler, capturedRasterRunner, renderer, metrics,
                    ref frameworkFrameNumber, $"f6/{name}/resize-{target.Item1}", frameCallback);
                DispatchOrdinary(capturedScheduler, capturedRasterRunner, renderer, metrics,
                    ref frameworkFrameNumber, $"f6/{name}/ordinary-after-resize-{target.Item1}", frameCallback);
            }

            var hiddenStopped = false;
            var minimizedStopped = false;
            var suspendedStopped = false;
            if (runLifecycleExercise)
            {
                hiddenStopped = ExerciseStoppedState(
                    capturedScheduler, capturedRasterRunner, renderer, metricsCoordinator.Current,
                    ref frameworkFrameNumber, frameCallback, $"f6/{name}/hidden", capturedScheduler.SetHidden);
                minimizedStopped = ExerciseStoppedState(
                    capturedScheduler, capturedRasterRunner, renderer, metricsCoordinator.Current,
                    ref frameworkFrameNumber, frameCallback, $"f6/{name}/minimized", capturedScheduler.SetMinimized);
                suspendedStopped = ExerciseStoppedState(
                    capturedScheduler, capturedRasterRunner, renderer, metricsCoordinator.Current,
                    ref frameworkFrameNumber, frameCallback, $"f6/{name}/suspended", capturedScheduler.SetSuspended);
            }

            DispatchOrdinary(capturedScheduler, capturedRasterRunner, renderer, metricsCoordinator.Current,
                ref frameworkFrameNumber, $"f6/{name}/causal-final", frameCallback);

            var activeSurface = AwaitWithinTimeout(capturedRasterRunner.RunAsync(() => surface.Snapshot),
                "F6 active surface snapshot");
            var schedulerSnapshot = capturedScheduler.Snapshot;
            var scheduledRasterSnapshot = scheduledRaster.Snapshot;
            var runnerSnapshot = capturedRasterRunner.Snapshot;
            var vsyncSnapshot = dwmVsync.Snapshot;
            Assert(activeSurface.ExactExtentMismatchCount == 0 && activeSurface.ZeroSizedSurfaceCreateCount == 0 &&
                activeSurface.ThreadAffinityViolationCount == 0 && activeSurface.SuccessfulSwapCount > 0,
                "F6 actual F4 surface did not preserve exact MTA-only swap ownership.");
            Assert(schedulerSnapshot.AnimationStarvationCount == 0 &&
                schedulerSnapshot.ResizeStarvationCount == 0 &&
                schedulerSnapshot.FrameQueueOverflowCount == 0 &&
                schedulerSnapshot.StaleOrWrongSizePresentCount == 0 &&
                schedulerSnapshot.CausalGapCount == 0 &&
                schedulerSnapshot.PendingResizeOrdinaryRejectedCount > 0 &&
                schedulerSnapshot.MaxObservedQueueDepth <= 1 &&
                schedulerSnapshot.PresentedFrameCount > 0 &&
                scheduledRasterSnapshot.CausalReceiptMismatchCount == 0 &&
                scheduledRasterSnapshot.FailureCount == 0,
                "F6 runtime scheduler counters violate bounded exact presentation.");
            Assert(!runLifecycleExercise ||
                (hiddenStopped && minimizedStopped && suspendedStopped && latestMetricsRequestCount >= 3 &&
                 schedulerSnapshot.OrdinaryResumeCount >= 3 && schedulerSnapshot.HiddenStopCount >= 1 &&
                 schedulerSnapshot.MinimizedStopCount >= 1 && schedulerSnapshot.SuspendedStopCount >= 1 &&
                 schedulerSnapshot.RestoredLatestMetricsCount >= 3 && resumedWithLatestMetrics),
                "F6 lifecycle stop/restore did not resume through the latest immutable metrics.");

            capturedScheduler.Dispose();
            scheduler = null;
            metricsCoordinator.Dispose();
            metricsCoordinator = null;
            host.Dispose();
            host = null;
            Assert(surfaceReleasedByHost && disposedSurface is not null &&
                disposedSurface.EglSurfaceLeakCount == 0 && disposedSurface.SkiaResourceLeakCount == 0,
                "F6 view teardown did not release all child EGL and Skia resources.");
            capturedRasterRunner.Dispose();
            rasterRunner = null;

            var finalDisposedSurface = disposedSurface ?? throw new InvalidOperationException(
                "F6 did not retain its disposed surface snapshot.");
            completed = true;
            return new RuntimeViewResult(
                name,
                Environment.CurrentManagedThreadId,
                activeSurface,
                finalDisposedSurface,
                schedulerSnapshot,
                scheduledRasterSnapshot,
                runnerSnapshot,
                vsyncSnapshot,
                hiddenStopped,
                minimizedStopped,
                suspendedStopped,
                resumedWithLatestMetrics && latestMetricsRequestCount > 0,
                afterShow.FirstFrameShowCount);
        }
        finally
        {
            if (!completed)
            {
                try { scheduler?.Dispose(); } catch { }
                try { metricsCoordinator?.Dispose(); } catch { }
                if (host is not null)
                {
                    try { host.Dispose(); } catch { }
                }
                else if (rasterRunner is not null)
                {
                    try
                    {
                        _ = AwaitWithinTimeout(rasterRunner.RunAsync(() =>
                        {
                            scheduledRaster?.Dispose();
                            renderer?.Dispose();
                            try { surface?.Dispose(); }
                            finally { sharedContext?.Dispose(); }
                            return true;
                        }), "F6 failed setup teardown");
                    }
                    catch { }
                }
                try { rasterRunner?.Dispose(); } catch { }
                try { bootstrap.DisposeOnCurrentThread(); } catch { }
            }
        }
    }

    private static IReadOnlyList<CadenceRow> RunCadenceMatrix()
    {
        AssertPendingLatestMetricsReplacement();
        AssertActiveResizeRetainsFrameworkContinuation();
        var rows = new List<CadenceRow>();
        foreach (var refreshHz in new[] { 60, 120, 144, 165 })
        {
            var viewId = checked((ulong)(70_000 + refreshHz));
            var initial = CreateDeterministicMetrics(viewId, 1, 640, 360, refreshHz);
            var vsyncSource = new FlutterWindowsDeterministicVsyncSource(refreshHz);
            using var scheduler = new FlutterWindowsFrameScheduler(initial, vsyncSource);
            var samples = new List<TimeSpan>();
            var callback = CreateSyntheticRasterCallback(scheduler, samples);

            for (var index = 0; index < RequiredCadenceSamples; index++)
            {
                var scheduled = scheduler.ScheduleOrdinary(initial.ToViewEpoch(), callback);
                Assert(scheduled.Accepted, $"F6 {refreshHz}Hz deterministic ordinary frame was not accepted.");
                DispatchSynthetic(scheduler, $"F6 {refreshHz}Hz deterministic ordinary frame");
            }

            var resized = initial with
            {
                ResizeGeneration = 2,
                PhysicalWidth = 641,
                PhysicalHeight = 361,
                TimestampMicroseconds = initial.TimestampMicroseconds + 1,
            };
            scheduler.PublishMetrics(resized);
            var displacedOrdinary = scheduler.ScheduleOrdinary(resized.ToViewEpoch(), callback);
            Assert(displacedOrdinary.Accepted,
                $"F6 {refreshHz}Hz did not queue the ordinary frame that resize must displace.");
            var resize = scheduler.ScheduleResize(resized, callback);
            Assert(resize.Accepted && resize.ReplacedLatest,
                $"F6 {refreshHz}Hz deterministic resize did not take the bounded pending slot.");
            DispatchSynthetic(scheduler, $"F6 {refreshHz}Hz deterministic resize");
            var ordinaryAfterResize = scheduler.ScheduleOrdinary(resized.ToViewEpoch(), callback);
            Assert(ordinaryAfterResize.Accepted,
                $"F6 {refreshHz}Hz ordinary animation did not resume after exact resize.");
            DispatchSynthetic(scheduler, $"F6 {refreshHz}Hz deterministic ordinary resume");

            var snapshot = scheduler.Snapshot;
            var expectedInterval = TimeSpan.FromSeconds(1.0 / refreshHz);
            var observedIntervals = samples
                .Take(RequiredCadenceSamples)
                .Zip(samples.Skip(1).Take(RequiredCadenceSamples - 1), (first, second) => second - first)
                .ToArray();
            var cadenceTolerance = TimeSpan.FromTicks(Math.Max(2, expectedInterval.Ticks / 100));
            var targetCadenceMet = observedIntervals.Length == RequiredCadenceSamples - 1 &&
                observedIntervals.All(interval => (interval - expectedInterval).Duration() <= cadenceTolerance);
            Assert(targetCadenceMet, $"F6 deterministic {refreshHz}Hz cadence drifted from the exact source interval.");
            Assert(snapshot.AnimationStarvationCount == 0 && snapshot.ResizeStarvationCount == 0 &&
                snapshot.FrameQueueOverflowCount == 0 && snapshot.StaleOrWrongSizePresentCount == 0 &&
                snapshot.CausalGapCount == 0 && snapshot.PendingResizeOrdinaryRejectedCount > 0 &&
                snapshot.MaxObservedQueueDepth <= 1,
                $"F6 deterministic {refreshHz}Hz scheduler counters violate the bounded contract.");
            rows.Add(new CadenceRow(
                refreshHz,
                snapshot.CallbackCount,
                snapshot.PresentedFrameCount,
                targetCadenceMet,
                snapshot.AnimationStarvationCount,
                snapshot.ResizeStarvationCount,
                snapshot.MaxObservedQueueDepth,
                snapshot.StaleOrWrongSizePresentCount,
                OrdinaryResumedAfterResize: snapshot.OrdinaryCallbackCount > RequiredCadenceSamples));
        }
        return rows;
    }

    private static void AssertPendingLatestMetricsReplacement()
    {
        const ulong viewId = 79_999;
        var initial = CreateDeterministicMetrics(viewId, 1, 640, 360, 60);
        using var scheduler = new FlutterWindowsFrameScheduler(
            initial,
            new FlutterWindowsDeterministicVsyncSource(60));
        FlutterWindowsFrameTicket? callbackTicket = null;
        var pending = scheduler.ScheduleOrdinary(
            initial.ToViewEpoch(),
            (ticket, _) => callbackTicket = ticket,
            canReplaceBeforeDispatch: true);
        Assert(pending.Accepted, "F6 could not queue a replaceable pre-dispatch frame.");

        var latest = initial with
        {
            ResizeGeneration = 2,
            PhysicalWidth = 812,
            PhysicalHeight = 476,
            TimestampMicroseconds = initial.TimestampMicroseconds + 1,
        };
        scheduler.PublishMetrics(latest);
        var run = scheduler.TryRunOneFrame();
        Assert(run.Dispatched && callbackTicket is not null,
            "F6 latest-metrics replacement did not dispatch the pending callback.");
        var admittedTicket = callbackTicket ?? throw new InvalidOperationException(
            "F6 latest-metrics replacement callback ticket was null after dispatch.");
        Assert(admittedTicket.Kind == FlutterWindowsFrameKind.Resize &&
            ReferenceEquals(admittedTicket.Metrics, latest) &&
            admittedTicket.ExpectedEpoch == latest.ToViewEpoch(),
            "F6 changed native metrics without replacing the not-yet-started callback ticket exactly.");
        Assert(scheduler.Snapshot.MaxObservedQueueDepth <= 1 &&
            scheduler.Snapshot.DroppedStaleCallbackCount == 0,
            "F6 latest-metrics replacement overflowed the bounded queue or dispatched stale work.");
    }

    private static void AssertActiveResizeRetainsFrameworkContinuation()
    {
        const ulong viewId = 79_998;
        var initial = CreateDeterministicMetrics(viewId, 1, 640, 360, 60);
        using var scheduler = new FlutterWindowsFrameScheduler(
            initial,
            new FlutterWindowsDeterministicVsyncSource(60));
        var active = scheduler.ScheduleResize(initial, (_, _) => { });
        Assert(active.Accepted && scheduler.TryRunOneFrame().Dispatched,
            "F6 could not start the resize used by the continuation regression.");

        FlutterWindowsFrameTicket? callbackTicket = null;
        var continuation = scheduler.ScheduleOrdinary(
            initial.ToViewEpoch(),
            (ticket, _) => callbackTicket = ticket,
            canReplaceBeforeDispatch: true);
        Assert(continuation.Accepted && scheduler.Snapshot.HasPendingOrdinary,
            "F6 dropped the framework continuation requested while resize raster was active.");

        var latest = initial with
        {
            ResizeGeneration = 2,
            PhysicalWidth = 824,
            PhysicalHeight = 488,
            TimestampMicroseconds = initial.TimestampMicroseconds + 1,
        };
        scheduler.PublishMetrics(latest);
        var run = scheduler.TryRunOneFrame();
        var admittedTicket = callbackTicket ?? throw new InvalidOperationException(
            "F6 active-resize continuation callback was not dispatched.");
        Assert(run.Dispatched && admittedTicket.Kind == FlutterWindowsFrameKind.Resize &&
            ReferenceEquals(admittedTicket.Metrics, latest) &&
            admittedTicket.ExpectedEpoch == latest.ToViewEpoch(),
            "F6 did not promote the retained continuation to the latest exact resize before dispatch.");
        Assert(scheduler.Snapshot.MaxObservedQueueDepth <= 1,
            "F6 active-resize continuation exceeded the single pending slot.");
    }

    private static FlutterWindowsScheduledFrameCallback CreateSyntheticRasterCallback(
        FlutterWindowsFrameScheduler scheduler,
        ICollection<TimeSpan> samples) => (ticket, vsync) =>
    {
        samples.Add(vsync.Timestamp);
        Assert(scheduler.TryAdmitRaster(ticket, out var admissionFailure),
            $"F6 deterministic raster admission failed: {admissionFailure}.");
        var present = new FlutterWindowsAngleEglPresentResult(
            ticket.Metrics.PhysicalWidth,
            ticket.Metrics.PhysicalHeight,
            ticket.Metrics.ResizeGeneration,
            RecoveredFromContextLoss: false,
            SuccessfulSwap: true);
        Assert(scheduler.ReportSwap(ticket, present, DorotiFrameClock.Now),
            "F6 deterministic scheduler rejected an exact synthetic swap.");
        var descriptor = ticket.Metrics.CreateFrameDescriptor(
            ticket.CausalFrameId,
            ticket.CausalFrameId);
        var receipt = new SkiaFrameReceipt(
            ticket.CausalFrameId,
            InputSequence: 0,
            SceneSequence: ticket.CausalFrameId,
            SurfaceGeneration: ticket.Metrics.ResizeGeneration,
            descriptor,
            DorotiFrameTerminal.presented,
            DorotiFrameClock.Now);
        Assert(scheduler.ReportSkiaReceipt(receipt),
            "F6 deterministic scheduler rejected its exact causal Skia receipt.");
    };

    private static void DispatchSynthetic(FlutterWindowsFrameScheduler scheduler, string description)
    {
        var run = scheduler.TryRunOneFrame();
        Assert(run.Dispatched && run.Ticket is not null && run.Vsync is not null,
            $"{description} was not dispatched at one scheduler boundary.");
    }

    private static bool ExerciseStoppedState(
        FlutterWindowsFrameScheduler scheduler,
        FlutterWindowsDedicatedRasterTaskRunner rasterRunner,
        SkiaSceneRenderer renderer,
        WindowsViewMetrics metrics,
        ref long frameworkFrameNumber,
        FlutterWindowsScheduledFrameCallback callback,
        string description,
        Action<bool> setState)
    {
        var postedBefore = rasterRunner.Snapshot.PostedCount;
        setState(true);
        SubmitExactScene(renderer, metrics, ref frameworkFrameNumber, description + "/stopped");
        _ = scheduler.ScheduleOrdinary(metrics.ToViewEpoch(), callback);
        var stopped = scheduler.TryRunOneFrame();
        Assert(stopped.Disposition == FlutterWindowsFrameRunDisposition.SchedulingStopped,
            $"{description} ran a scheduler callback while stopped.");
        Assert(rasterRunner.Snapshot.PostedCount == postedBefore,
            $"{description} posted raster work while scheduler was stopped.");
        setState(false);
        DispatchExisting(scheduler, rasterRunner, description + "/restored");
        return true;
    }

    private static void DispatchOrdinary(
        FlutterWindowsFrameScheduler scheduler,
        FlutterWindowsDedicatedRasterTaskRunner rasterRunner,
        SkiaSceneRenderer renderer,
        WindowsViewMetrics metrics,
        ref long frameworkFrameNumber,
        string reason,
        FlutterWindowsScheduledFrameCallback callback)
    {
        SubmitExactScene(renderer, metrics, ref frameworkFrameNumber, reason);
        var scheduled = scheduler.ScheduleOrdinary(metrics.ToViewEpoch(), callback);
        Assert(scheduled.Accepted, $"{reason} ordinary frame was not accepted.");
        DispatchExisting(scheduler, rasterRunner, reason);
    }

    private static void DispatchResize(
        FlutterWindowsFrameScheduler scheduler,
        FlutterWindowsDedicatedRasterTaskRunner rasterRunner,
        SkiaSceneRenderer renderer,
        WindowsViewMetrics metrics,
        ref long frameworkFrameNumber,
        string reason,
        FlutterWindowsScheduledFrameCallback callback)
    {
        SubmitExactScene(renderer, metrics, ref frameworkFrameNumber, reason);
        var scheduled = scheduler.ScheduleResize(metrics, callback);
        Assert(scheduled.Accepted, $"{reason} resize frame was not accepted.");
        DispatchExisting(scheduler, rasterRunner, reason);
    }

    private static void DispatchExisting(
        FlutterWindowsFrameScheduler scheduler,
        FlutterWindowsDedicatedRasterTaskRunner rasterRunner,
        string reason)
    {
        var before = scheduler.Snapshot.PresentedFrameCount;
        var run = scheduler.TryRunOneFrame();
        Assert(run.Dispatched && run.Ticket is not null && run.Vsync is not null,
            $"{reason} did not dispatch exactly one scheduler callback.");
        AwaitWithinTimeout(rasterRunner.DrainAsync(), reason + " MTA raster drain");
        var after = scheduler.Snapshot;
        Assert(after.PresentedFrameCount == before + 1,
            $"{reason} did not complete one exact causal raster/swap/present receipt.");
    }

    private static void SubmitExactScene(
        SkiaSceneRenderer renderer,
        WindowsViewMetrics metrics,
        ref long frameworkFrameNumber,
        string reason)
    {
        var frameNumber = checked(++frameworkFrameNumber);
        var scene = new Scene(metrics.ViewId, Array.Empty<SceneCommand>());
        var token = new DorotiSceneBuildToken(
            metrics.ToViewEpoch(),
            frameNumber,
            metrics.PhysicalWidth,
            metrics.PhysicalHeight);
        renderer.Submit(
            metrics.ViewId,
            new DorotiSceneSubmission(scene, token),
            DartUiInvocation.Managed(reason));
    }

    private static WindowsViewMetrics CreateDeterministicMetrics(
        ulong viewId,
        long generation,
        int width,
        int height,
        int refreshHz) => new(
            viewId,
            generation,
            width,
            height,
            DevicePixelRatio: 1,
            DisplayId: $"deterministic-{refreshHz}hz",
            MinimumPhysicalWidth: 1,
            MinimumPhysicalHeight: 1,
            MaximumPhysicalWidth: 4096,
            MaximumPhysicalHeight: 4096,
            State: WindowsViewMetricsState.Active,
            TimestampMicroseconds: generation);

    private static void AssertRuntimeView(RuntimeViewResult view)
    {
        var trace = RequirePresentedTrace(view);
        Assert(trace.ExactMetrics && trace.Presented && trace.SwapTimestamp is not null &&
            trace.PresentedTimestamp is not null &&
            trace.CallbackTimestamp <= trace.RasterTimestamp &&
            trace.RasterTimestamp <= trace.SwapTimestamp.Value &&
            trace.SwapTimestamp.Value <= trace.PresentedTimestamp.Value,
            $"F6 {view.Name} causal trace is incomplete or out of order.");
        Assert(view.FirstFrameShowCount == 1 && view.RasterRunner.ApartmentState == ApartmentState.MTA &&
            view.RasterRunner.ManagedThreadId != 0 &&
            view.Scheduler.CallbackCount >= view.ScheduledRaster.RasterCount &&
            view.ScheduledRaster.RasterCount >= view.ScheduledRaster.SwapCount &&
            view.ScheduledRaster.SwapCount >= view.ScheduledRaster.PresentedReceiptCount &&
            view.ScheduledRaster.PresentedReceiptCount > 0,
            $"F6 {view.Name} did not retain a valid platform-to-MTA causal count chain.");
    }

    private static FlutterWindowsScheduledRasterCausalTrace RequirePresentedTrace(RuntimeViewResult view) =>
        view.ScheduledRaster.LastCausalTrace is { Presented: true, ExactMetrics: true,
            SwapTimestamp: not null, PresentedTimestamp: not null } trace
            ? trace
            : throw new InvalidOperationException($"F6 {view.Name} did not retain a final exact presented causal trace.");

    private static object ToViewEvidence(string name, RuntimeViewResult view) => new
    {
        name,
        viewId = view.Scheduler.ViewId,
        isolatedState = view.Scheduler.CrossViewLeakCount == 0,
        callbackCount = view.Scheduler.CallbackCount,
        rasterCount = view.ScheduledRaster.RasterCount,
        swapCount = view.ScheduledRaster.SwapCount,
        crossViewLeakCount = view.Scheduler.CrossViewLeakCount,
    };

    private static object ToCausalEvidence(
        string name,
        FlutterWindowsScheduledRasterCausalTrace trace) => new
    {
        name,
        causalId = trace.CausalFrameId,
        viewId = trace.ViewId,
        callbackTimestampTicks = trace.CallbackTimestamp.Ticks,
        rasterTimestampTicks = trace.RasterTimestamp.Ticks,
        swapTimestampTicks = trace.SwapTimestamp?.Ticks ?? 0,
        presentedTimestampTicks = trace.PresentedTimestamp?.Ticks ?? 0,
        causalOrderValid = trace.SwapTimestamp is { } swap && trace.PresentedTimestamp is { } presented &&
            trace.CallbackTimestamp <= trace.RasterTimestamp && trace.RasterTimestamp <= swap && swap <= presented,
        exactMetrics = trace.ExactMetrics && trace.Presented,
    };

    private static T AwaitWithinTimeout<T>(ValueTask<T> operation, string description)
    {
        var task = operation.AsTask();
        if (!task.Wait(TestTimeout))
            throw new TimeoutException($"{description} did not complete within 20 minutes.");
        return task.GetAwaiter().GetResult();
    }

    private static void AwaitWithinTimeout(ValueTask operation, string description)
    {
        var task = operation.AsTask();
        if (!task.Wait(TestTimeout))
            throw new TimeoutException($"{description} did not complete within 20 minutes.");
        task.GetAwaiter().GetResult();
    }

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
                $"AdjustWindowRectExForDpi F6 resize failed: {Marshal.GetLastWin32Error()}");
        }
        if (!SetWindowPos(topLevelHwnd, 0, -32000, -32000, outer.Width, outer.Height,
                SwpNoZOrder | SwpNoActivate))
        {
            throw new InvalidOperationException(
                $"SetWindowPos F6 top-level resize failed: {Marshal.GetLastWin32Error()}");
        }
        PumpPendingMessages();
    }

    private static void PumpPendingMessages()
    {
        while (PeekMessageW(out var message, 0, 0, 0, 1))
        {
            if (message.Message == 0x0012)
                throw new InvalidOperationException("F6 validation unexpectedly received WM_QUIT.");
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
            Name = "Doroti F6 platform STA validation",
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        if (!complete.Wait(TestTimeout) || !thread.Join(TestTimeout))
            throw new TimeoutException("F6 platform/raster validation did not terminate within 20 minutes.");
        if (failure is not null)
            throw new InvalidOperationException("F6 platform/raster validation failed.", failure);
        return result ?? throw new InvalidOperationException("F6 validation returned no result.");
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

    private static F6Options ParseOptions(string[] args)
    {
        string? evidencePath = null;
        string? sourceFingerprint = null;
        string? publishedExecutableSha256 = null;
        for (var index = 0; index < args.Length; index += 2)
        {
            if (index + 1 >= args.Length)
                throw new ArgumentException("Every F6 validator option requires a value.");
            switch (args[index])
            {
                case "--evidence": evidencePath = args[index + 1]; break;
                case "--source-fingerprint": sourceFingerprint = args[index + 1]; break;
                case "--published-executable-sha256": publishedExecutableSha256 = args[index + 1]; break;
                default:
                    throw new ArgumentException(
                        "Usage: Doroti.Validation.WindowsAppSdkFlutterFrameScheduler " +
                        "[--evidence <path>] [--source-fingerprint <sha256>] " +
                        "[--published-executable-sha256 <sha256>]");
            }
        }
        return new F6Options(evidencePath, sourceFingerprint, publishedExecutableSha256);
    }

    private static string ComputeFileHash(string path) =>
        Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

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

    private sealed record F6Options(
        string? EvidencePath,
        string? SourceFingerprint,
        string? PublishedExecutableSha256);

    private sealed record CadenceRow(
        int RefreshHz,
        long ScheduledFrameCount,
        long PresentedFrameCount,
        bool TargetCadenceMet,
        long AnimationStarvationCount,
        long ResizeStarvationCount,
        int MaxQueueDepth,
        long StaleOrWrongSizePresentCount,
        bool OrdinaryResumedAfterResize);

    private sealed record RuntimeViewResult(
        string Name,
        int PlatformManagedThreadId,
        FlutterWindowsAngleEglWindowSurfaceSnapshot ActiveSurface,
        FlutterWindowsAngleEglWindowSurfaceSnapshot DisposedSurface,
        FlutterWindowsFrameSchedulerSnapshot Scheduler,
        FlutterWindowsScheduledRasterSnapshot ScheduledRaster,
        FlutterWindowsDedicatedRasterTaskRunnerSnapshot RasterRunner,
        FlutterWindowsDwmVsyncSourceSnapshot DwmTiming,
        bool HiddenSchedulingStopped,
        bool MinimizedSchedulingStopped,
        bool SuspendedSchedulingStopped,
        bool ResumedWithLatestMetrics,
        long FirstFrameShowCount);

    private sealed class FixtureRendererHost(Func<WindowsViewMetrics> currentMetrics) : ISkiaSceneRendererHost
    {
        public long InputSequence => 0;
        public long SurfaceGeneration => currentMetrics().ResizeGeneration;
        public DorotiViewEpoch ViewEpoch => currentMetrics().ToViewEpoch();
        public DorotiResizeEpoch ResizeTarget => currentMetrics().ToResizeEpoch();
        public PlatformConfiguration Configuration { get; } = new(
            [new Locale("en", "US")],
            Brightness.light,
            alwaysUse24HourFormat: false,
            nativeSpellCheckServiceDefined: false,
            HostOperatingSystem.windows);

        public event Action<int, SemanticsAction, object?>? SemanticsAction
        {
            add { }
            remove { }
        }

        public event Action<long, TimeSpan>? InputReceived
        {
            add { }
            remove { }
        }

        public event Action<PlatformConfiguration>? ConfigurationChanged
        {
            add { }
            remove { }
        }

        public void UpdateSemantics(SemanticsUpdate update) => _ = update;
        public void ClearSemantics() { }
        public void RequestInvalidate() { }
    }
}
