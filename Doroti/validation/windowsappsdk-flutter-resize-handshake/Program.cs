using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;
using SkiaSharp;

namespace Doroti.Validation.WindowsAppSdkFlutterResizeHandshake;

internal static partial class Program
{
    private const int RequiredDirectionalCycles = 3;
    private const int WarmupDirectionalCycles = 1;
    private const int GwlStyle = -16;
    private const int GwlExStyle = -20;
    private const uint SwpNoZOrder = 0x0004;
    private const uint SwpNoActivate = 0x0010;
    private const int SwMinimize = 6;
    private const string EvidenceSchema =
        "doroti.windowsappsdk-flutter-resize-handshake-evidence/v1";
    private static readonly TimeSpan TestTimeout = TimeSpan.FromMinutes(20);
    private static readonly ResizeDirection[] Directions =
    [
        new("left", AnchorRight: true, AnchorBottom: false),
        new("right", AnchorRight: false, AnchorBottom: false),
        new("top", AnchorRight: false, AnchorBottom: true),
        new("bottom", AnchorRight: false, AnchorBottom: false),
        new("top-left", AnchorRight: true, AnchorBottom: true),
        new("top-right", AnchorRight: false, AnchorBottom: true),
        new("bottom-left", AnchorRight: true, AnchorBottom: false),
        new("bottom-right", AnchorRight: false, AnchorBottom: false),
    ];

    private static int Main(string[] args)
    {
        var options = ParseOptions(args);
        try
        {
            _ = RunOnDedicatedPlatformStaThread(() =>
            {
                _ = RunF5Exercise(WarmupDirectionalCycles);
                return 0;
            });
            var gdiBefore = GetGuiResources(GuiResourceType.Gdi);
            var userBefore = GetGuiResources(GuiResourceType.User);
            var result = RunOnDedicatedPlatformStaThread(() => RunF5Exercise(RequiredDirectionalCycles));
            var gdiAfter = GetGuiResources(GuiResourceType.Gdi);
            var userAfter = GetGuiResources(GuiResourceType.User);
            var guiResourcesBounded = gdiAfter <= gdiBefore + 2 && userAfter <= userBefore + 2;
            if (!guiResourcesBounded)
            {
                throw new InvalidOperationException(
                    $"F5 GUI resources grew after warmup: GDI {gdiBefore}->{gdiAfter}, USER {userBefore}->{userAfter}.");
            }

            var executablePath = Environment.ProcessPath ?? throw new InvalidOperationException(
                "The F5 validator executable path is unavailable.");
            var executableHash = ComputeFileHash(executablePath);
            if (!string.IsNullOrWhiteSpace(options.PublishedExecutableSha256) &&
                !string.Equals(executableHash, options.PublishedExecutableSha256,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The F5 validator executable hash does not match the publish-gate input.");
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
                platformThread = new
                {
                    apartment = "STA",
                    executionMode = "same-sta-thread",
                    engineTaskRunnerPollOnly = result.Handshake.EngineTaskRunnerPollOnly,
                    maxPollMilliseconds = result.Handshake.MaximumPollMilliseconds,
                    arbitraryNestedWin32MessageDispatchCount =
                        result.Handshake.ArbitraryNestedWin32MessageDispatchCount,
                    engineTaskRunnerPollCount = result.Handshake.EngineTaskRunnerPollCount,
                },
                rasterThread = new
                {
                    apartment = "MTA",
                    executionMode = "dedicated-raster-thread",
                    exactGenerationExtentMismatchPresentCount =
                        result.Handshake.ExactGenerationExtentMismatchPresentCount,
                    dwmFlushAfterUnblockCount = result.Handshake.DwmFlushAfterPlatformUnblockCount,
                    dwmFlushBeforeDoneCount = result.Handshake.DwmFlushBeforeDoneCount,
                    donePublishedBeforeDwmFlush = result.AllDwmFlushesFollowDone,
                    dwmFlushFailureCount = result.Handshake.DwmFlushFailureCount,
                    rasterManagedThreadId = active.RasterManagedThreadId,
                    rasterNativeThreadId = active.RasterNativeThreadId,
                },
                transactions = new
                {
                    normalDragCount = result.Directions.Sum(direction => direction.TransactionCount),
                    normalDragTimeoutCount = result.Directions.Sum(direction => direction.TimeoutCount),
                    terminalMissingCount = result.Handshake.TerminalMissingCount,
                    terminalDuplicateCount = result.Handshake.TerminalDuplicateCount,
                    allTerminalsExactlyOnce = result.Handshake.AllTerminalsExactlyOnce,
                    metricsDeliveredCount = result.Handshake.MetricsDeliveredCount,
                    frameGeneratedCount = result.Handshake.FrameGeneratedCount,
                    surfaceReadyCount = result.Handshake.SurfaceReadyCount,
                    presentedCount = result.Handshake.PresentedCount,
                    doneCount = result.Handshake.DoneCount,
                },
                faultTimeout = new
                {
                    injectionCount = result.FaultTimeout.InjectionCount,
                    timeoutTerminalCount = result.FaultTimeout.TimeoutTerminalCount,
                    noUiDeadlock = result.FaultTimeout.NoUiDeadlock,
                    noInfiniteWait = result.FaultTimeout.NoInfiniteWait,
                    terminalExactlyOnce = result.FaultTimeout.TerminalExactlyOnce,
                    childRectReobserved = result.FaultTimeout.ChildRectReobserved,
                    latestRedrawRequested = result.FaultTimeout.LatestRedrawRequested,
                },
                directions = result.Directions.Select(direction => new
                {
                    name = direction.Name,
                    transactionCount = direction.TransactionCount,
                    protocolIdentical = direction.ProtocolIdentical,
                    stateOrderExact = direction.StateOrderExact,
                    exactPresentCount = direction.ExactPresentCount,
                    timeoutCount = direction.TimeoutCount,
                    terminalMissingCount = direction.TerminalMissingCount,
                    terminalDuplicateCount = direction.TerminalDuplicateCount,
                }).ToArray(),
                terminalCases = result.TerminalCases.Select(terminal => new
                {
                    name = terminal.Name,
                    terminalCount = terminal.TerminalCount,
                    terminalMissingCount = terminal.TerminalMissingCount,
                    terminalDuplicateCount = terminal.TerminalDuplicateCount,
                    exactlyOnce = terminal.ExactlyOnce,
                }).ToArray(),
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
                    sourceFingerprint = options.SourceFingerprint ?? "unbound-local-run",
                    executablePath,
                    executableSha256 = executableHash,
                    requiredDirectionalCycles = RequiredDirectionalCycles,
                },
                scopeBoundary = "F5 proves the bounded Flutter-style metrics-to-frame-to-F4-swap handshake, exact terminal ledger, timeout recovery, and post-unblock DwmFlush ordering. It does not claim scheduler cadence, product-runner selection, input/lifecycle completion, compositor acceptance, or visible white-frame absence.",
            };
            var json = JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true });
            if (!string.IsNullOrWhiteSpace(options.EvidencePath))
            {
                var evidencePath = Path.GetFullPath(options.EvidencePath);
                Directory.CreateDirectory(Path.GetDirectoryName(evidencePath) ??
                    throw new InvalidOperationException("The F5 evidence path has no parent directory."));
                File.WriteAllText(evidencePath, json + Environment.NewLine);
            }
            Console.WriteLine(json);
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"windowsappsdk-flutter-resize-handshake FAIL: {exception}");
            return 1;
        }
    }

    private static F5ExerciseResult RunF5Exercise(int directionalCycles)
    {
        var bootstrap = new FlutterWindowsAppSdkBootstrap();
        FlutterWindowsHostWindow? host = null;
        FlutterWindowsViewMetricsCoordinator? metricsCoordinator = null;
        FlutterWindowsResizeHandshake? handshake = null;
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
                    "Doroti F5 resize handshake validation",
                    InitialClientWidth: 640,
                    InitialClientHeight: 360,
                    MinimumClientWidth: 320,
                    MinimumClientHeight: 240,
                    MaximumClientWidth: 1200,
                    MaximumClientHeight: 900,
                    InitialX: -32000,
                    InitialY: -32000),
                new FlutterWindowsHostWindowTeardown
                {
                    DisposeViewSurface = () =>
                    {
                        disposedSurface = capturedRasterThread.Invoke(() =>
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
                            "F5 did not retain a surface snapshot during view-surface teardown.");
                        surfaceReleasedByHost = true;
                    },
                });
            metricsCoordinator = FlutterWindowsViewMetricsCoordinator.AttachToHostWindow(
                host,
                viewId: 50_001,
                new FlutterWindowsPhysicalConstraints(320, 240, 1200, 900));
            var initialMetrics = metricsCoordinator.Current;
            sharedContext = RunOnDedicatedRasterThread(
                capturedRasterThread,
                FlutterWindowsAngleEglSharedContext.CreateOnCurrentRasterThread);
            surface = RunOnDedicatedRasterThread(capturedRasterThread, () =>
                FlutterWindowsAngleEglWindowSurface.CreateOnCurrentRasterThread(
                    sharedContext,
                    host.ViewHwnd,
                    initialMetrics));

            var engineTaskRunner = new QueueEngineTaskRunner();
            var asyncRaster = new AsyncF4Raster(capturedRasterThread);
            var latestRedrawRequests = 0;
            var corruptFrameForNextRequest = false;
            long frameworkFrameNumber = 0;
            long sceneSequence = 0;
            handshake = new FlutterWindowsResizeHandshake(
                metricsCoordinator,
                engineTaskRunner,
                asyncRaster,
                request =>
                {
                    var frame = FlutterWindowsResizeFrame.CreateExact(
                        request.Metrics,
                        checked(++frameworkFrameNumber),
                        checked(++sceneSequence),
                        (request.ResizeGeneration & 1) == 0 ? SKColors.MediumPurple : SKColors.CadetBlue);
                    if (!corruptFrameForNextRequest) return frame;
                    corruptFrameForNextRequest = false;
                    return frame with { PhysicalWidth = frame.PhysicalWidth + 1 };
                },
                _ => Interlocked.Increment(ref latestRedrawRequests));
            var capturedHandshake = handshake;
            var presenter = RunOnDedicatedRasterThread(capturedRasterThread, () =>
                new FlutterWindowsResizeRasterPresenter(surface, capturedHandshake));
            asyncRaster.SetPresenter(presenter);

            var directionResults = new List<DirectionResult>(Directions.Length);
            var shownAfterExactSwap = false;
            var targetIndex = 0;
            foreach (var direction in Directions)
            {
                var accumulator = new DirectionAccumulator(direction.Name);
                for (var cycle = 0; cycle < directionalCycles; cycle++)
                {
                    var target = NextTarget(targetIndex++);
                    ResizeTopLevelFromDirection(host.TopLevelHwnd, direction, target.Width, target.Height);
                    var metrics = metricsCoordinator.Current;
                    var poll = handshake.BeginResizeAndPoll(metrics);
                    capturedRasterThread.Drain();
                    var transaction = FindTransaction(handshake.Snapshot, metrics.ResizeGeneration);
                    AssertExactDone(transaction, direction.Name);
                    if (!shownAfterExactSwap)
                    {
                        var beforeShow = host.Snapshot;
                        if (beforeShow.FirstFrameSwapped || beforeShow.Shown)
                            throw new InvalidOperationException("F5 platform HWND showed before its first exact F4 swap.");
                        host.NotifyFirstFrameSwapped();
                        var afterShow = host.Snapshot;
                        if (!afterShow.FirstFrameSwapped || !afterShow.Shown ||
                            afterShow.FirstFrameShowCount != 1)
                        {
                            throw new InvalidOperationException(
                                "F5 did not preserve the F2 first-exact-swap show gate.");
                        }
                        shownAfterExactSwap = true;
                    }
                    accumulator.Record(transaction, poll);
                }
                directionResults.Add(accumulator.ToResult(directionalCycles));
            }

            asyncRaster.InjectTimeoutForNextRender();
            var timeoutTarget = NextTarget(targetIndex++);
            ResizeTopLevelFromDirection(host.TopLevelHwnd, Directions[1], timeoutTarget.Width, timeoutTarget.Height);
            var timeoutMetrics = metricsCoordinator.Current;
            var timeoutPoll = handshake.BeginResizeAndPoll(timeoutMetrics);
            if (timeoutPoll.Terminal != FlutterWindowsResizeTerminal.TimedOut)
                throw new InvalidOperationException("F5 deterministic raster fault did not close as TimedOut.");
            var timeoutTransaction = FindTransaction(handshake.Snapshot, timeoutMetrics.ResizeGeneration);
            if (!timeoutTransaction.ChildRectReobservedAfterTimeout ||
                !timeoutTransaction.LatestRedrawRequestedAfterTimeout || latestRedrawRequests == 0)
            {
                throw new InvalidOperationException(
                    "F5 timeout did not re-observe the actual child rect and request a latest redraw.");
            }
            var faultTimeout = new FaultTimeoutResult(
                InjectionCount: 1,
                TimeoutTerminalCount: timeoutTransaction.Terminal == FlutterWindowsResizeTerminal.TimedOut ? 1 : 0,
                NoUiDeadlock: timeoutPoll.Elapsed <= TimeSpan.FromMilliseconds(150),
                NoInfiniteWait: timeoutPoll.Elapsed <= TimeSpan.FromMilliseconds(150),
                TerminalExactlyOnce: timeoutTransaction.Terminal is not null,
                timeoutTransaction.ChildRectReobservedAfterTimeout,
                timeoutTransaction.LatestRedrawRequestedAfterTimeout);

            var activeSurface = capturedRasterThread.Invoke(() => surface.Snapshot);
            var snapshot = handshake.Snapshot;
            if (!shownAfterExactSwap || activeSurface.ExactExtentMismatchCount != 0 ||
                activeSurface.ZeroSizedSurfaceCreateCount != 0 ||
                activeSurface.CreateOutsideRasterThreadCount != 0 ||
                activeSurface.RecreateOutsideRasterThreadCount != 0 ||
                activeSurface.SwapOutsideRasterThreadCount != 0 ||
                activeSurface.ThreadAffinityViolationCount != 0 ||
                snapshot.ExactGenerationExtentMismatchPresentCount != 0 ||
                snapshot.DwmFlushAfterPlatformUnblockCount < directionResults.Sum(result => result.ExactPresentCount) ||
                snapshot.DwmFlushBeforeDoneCount != 0 || snapshot.DwmFlushFailureCount != 0 ||
                snapshot.TerminalMissingCount != 0 || snapshot.TerminalDuplicateCount != 0)
            {
                throw new InvalidOperationException("F5 normal resize ledger/raster counters violate the handshake contract.");
            }

            handshake.Dispose();
            handshake = null;
            metricsCoordinator.Dispose();
            metricsCoordinator = null;
            host.Dispose();
            host = null;
            if (!surfaceReleasedByHost || disposedSurface is null ||
                disposedSurface.EglSurfaceLeakCount != 0 || disposedSurface.SkiaResourceLeakCount != 0)
            {
                throw new InvalidOperationException(
                    "F5 view-surface teardown did not release all per-window EGL and Skia resources.");
            }
            rasterThread.Dispose();
            rasterThread = null;

            var terminalCases = new[]
            {
                RunTerminalCase("TimedOut", TerminalExercise.TimedOut),
                RunTerminalCase("Failed", TerminalExercise.Failed),
                RunTerminalCase("Superseded", TerminalExercise.Superseded),
                RunTerminalCase("Suspended", TerminalExercise.Suspended),
            };
            if (terminalCases.Any(result => !result.ExactlyOnce || result.TerminalCount != 1 ||
                                            result.TerminalMissingCount != 0 || result.TerminalDuplicateCount != 0))
            {
                throw new InvalidOperationException("F5 terminal-case matrix did not close each transaction exactly once.");
            }

            return new F5ExerciseResult(
                activeSurface,
                disposedSurface,
                snapshot,
                directionResults.ToArray(),
                faultTimeout,
                terminalCases,
                AllDwmFlushesFollowDone: snapshot.Transactions
                    .Where(transaction => transaction.Terminal == FlutterWindowsResizeTerminal.Done)
                    .All(transaction => transaction.DwmFlushAfterPlatformUnblock &&
                                        transaction.DonePublishedBeforeDwmFlush),
                CaptureMauiOrXamlAssemblyNames());
        }
        finally
        {
            handshake?.Dispose();
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

    private static TerminalCaseResult RunTerminalCase(string name, TerminalExercise exercise)
    {
        return RunOnDedicatedPlatformStaThread(() =>
        {
            var bootstrap = new FlutterWindowsAppSdkBootstrap();
            FlutterWindowsHostWindow? host = null;
            FlutterWindowsViewMetricsCoordinator? coordinator = null;
            FlutterWindowsResizeHandshake? handshake = null;
            try
            {
                host = FlutterWindowsHostWindow.CreateOnCurrentThread(
                    bootstrap,
                    new FlutterWindowsHostWindowOptions(
                        $"Doroti F5 {name} terminal validation",
                        InitialClientWidth: 640,
                        InitialClientHeight: 360,
                        MinimumClientWidth: 320,
                        MinimumClientHeight: 240,
                        MaximumClientWidth: 1200,
                        MaximumClientHeight: 900,
                        InitialX: -32000,
                        InitialY: -32000));
                coordinator = FlutterWindowsViewMetricsCoordinator.AttachToHostWindow(
                    host,
                    viewId: checked((ulong)(60_000 + (int)exercise)),
                    new FlutterWindowsPhysicalConstraints(320, 240, 1200, 900));
                var engine = new QueueEngineTaskRunner();
                long frameNumber = 0;
                long sceneSequence = 0;
                handshake = new FlutterWindowsResizeHandshake(
                    coordinator,
                    engine,
                    exercise == TerminalExercise.TimedOut
                        ? new NeverCompletingRaster()
                        : new UnexpectedRaster(),
                    request =>
                    {
                        var frame = FlutterWindowsResizeFrame.CreateExact(
                            request.Metrics,
                            checked(++frameNumber),
                            checked(++sceneSequence),
                            SKColors.OrangeRed);
                        return exercise == TerminalExercise.Failed
                            ? frame with { PhysicalHeight = frame.PhysicalHeight + 1 }
                            : frame;
                    },
                    options: FlutterWindowsResizeHandshakeOptions.Default,
                    subscribeToMetricsPublished: false);

                WindowsViewMetrics metrics;
                if (exercise == TerminalExercise.Suspended)
                {
                    _ = ShowWindow(host.TopLevelHwnd, SwMinimize);
                    PumpPendingMessages();
                    metrics = coordinator.ObserveChildMetrics();
                    if (metrics.State != WindowsViewMetricsState.Suspended)
                        throw new InvalidOperationException("F5 suspended terminal did not observe a zero-sized child client rect.");
                }
                else
                {
                    ResizeTopLevelFromDirection(host.TopLevelHwnd, Directions[0], 657, 377);
                    metrics = coordinator.ObserveChildMetrics();
                }

                var request = handshake.BeginResize(metrics);
                if (exercise == TerminalExercise.Superseded)
                {
                    if (!handshake.ReportSuperseded(request,
                            "Deterministic newer-generation replacement test."))
                    {
                        throw new InvalidOperationException("F5 supersede test could not close its pending transaction.");
                    }
                }
                var poll = handshake.PollResize(request);
                var expected = exercise switch
                {
                    TerminalExercise.TimedOut => FlutterWindowsResizeTerminal.TimedOut,
                    TerminalExercise.Failed => FlutterWindowsResizeTerminal.Failed,
                    TerminalExercise.Superseded => FlutterWindowsResizeTerminal.Superseded,
                    TerminalExercise.Suspended => FlutterWindowsResizeTerminal.Suspended,
                    _ => throw new ArgumentOutOfRangeException(nameof(exercise)),
                };
                if (poll.Terminal != expected)
                {
                    throw new InvalidOperationException(
                        $"F5 {name} terminal expected {expected}, got {poll.Terminal}.");
                }
                var snapshot = handshake.Snapshot;
                var transaction = FindTransaction(snapshot, request.ResizeGeneration);
                var result = new TerminalCaseResult(
                    name,
                    TerminalCount: transaction.Terminal == expected ? 1 : 0,
                    TerminalMissingCount: transaction.Terminal is null ? 1 : 0,
                    TerminalDuplicateCount: checked((int)snapshot.TerminalDuplicateCount),
                    ExactlyOnce: transaction.Terminal == expected && snapshot.TerminalDuplicateCount == 0);
                handshake.Dispose();
                handshake = null;
                coordinator.Dispose();
                coordinator = null;
                host.Dispose();
                host = null;
                return result;
            }
            finally
            {
                handshake?.Dispose();
                coordinator?.Dispose();
                if (host is not null) host.Dispose();
                else if (bootstrap.Snapshot.Phase != FlutterWindowsAppSdkBootstrapPhase.Disposed)
                    bootstrap.DisposeOnCurrentThread();
            }
        });
    }

    private static void AssertExactDone(FlutterWindowsResizeTransactionSnapshot transaction, string direction)
    {
        var expected = new[]
        {
            FlutterWindowsResizeState.ResizeStarted,
            FlutterWindowsResizeState.FrameGenerated,
            FlutterWindowsResizeState.SurfaceReady,
            FlutterWindowsResizeState.Presented,
            FlutterWindowsResizeState.Done,
        };
        if (transaction.Terminal != FlutterWindowsResizeTerminal.Done ||
            !transaction.StateHistory.SequenceEqual(expected))
        {
            throw new InvalidOperationException(
                $"F5 {direction} did not preserve the exact resize state order.");
        }
    }

    private static FlutterWindowsResizeTransactionSnapshot FindTransaction(
        FlutterWindowsResizeHandshakeSnapshot snapshot,
        long generation) => snapshot.Transactions.Single(transaction => transaction.ResizeGeneration == generation);

    private static void DisposeRasterResources(
        DedicatedRasterThread rasterThread,
        FlutterWindowsAngleEglWindowSurface? surface,
        FlutterWindowsAngleEglSharedContext? sharedContext)
    {
        _ = rasterThread.Invoke(() =>
        {
            try { surface?.Dispose(); }
            finally { sharedContext?.Dispose(); }
            return 0;
        });
    }

    private static T RunOnDedicatedRasterThread<T>(
        DedicatedRasterThread rasterThread,
        Func<T> action) => rasterThread.Invoke(action);

    private static (int Width, int Height) NextTarget(int index) =>
        (641 + ((index * 17) % 200), 361 + ((index * 13) % 160));

    /// <summary>
    /// Every directional row resizes the standard F2 top-level HWND.  The
    /// child remains owned by F2, F3 re-reads that child client rect, and the
    /// same F5 protocol receives no edge-specific surface adjustment.
    /// </summary>
    private static void ResizeTopLevelFromDirection(
        nint topLevelHwnd,
        ResizeDirection direction,
        int clientWidth,
        int clientHeight)
    {
        var style = unchecked((uint)GetWindowLongPtrW(topLevelHwnd, GwlStyle).ToInt64());
        var extendedStyle = unchecked((uint)GetWindowLongPtrW(topLevelHwnd, GwlExStyle).ToInt64());
        var dpi = GetDpiForWindow(topLevelHwnd);
        if (dpi == 0) dpi = 96;
        var outer = new NativeRect { Right = clientWidth, Bottom = clientHeight };
        if (!AdjustWindowRectExForDpi(ref outer, style, hasMenu: false, extendedStyle, dpi))
        {
            throw new InvalidOperationException(
                $"AdjustWindowRectExForDpi F5 resize failed: {Marshal.GetLastWin32Error()}");
        }
        if (!GetWindowRect(topLevelHwnd, out var previous))
            throw new InvalidOperationException($"GetWindowRect F5 resize failed: {Marshal.GetLastWin32Error()}");
        var x = direction.AnchorRight ? previous.Right - outer.Width : previous.Left;
        var y = direction.AnchorBottom ? previous.Bottom - outer.Height : previous.Top;
        if (!SetWindowPos(
                topLevelHwnd,
                0,
                x,
                y,
                outer.Width,
                outer.Height,
                SwpNoZOrder | SwpNoActivate))
        {
            throw new InvalidOperationException($"SetWindowPos F5 top-level resize failed: {Marshal.GetLastWin32Error()}");
        }
        PumpPendingMessages();
    }

    private static void PumpPendingMessages()
    {
        while (PeekMessageW(out var message, 0, 0, 0, 1))
        {
            if (message.Message == 0x0012)
                throw new InvalidOperationException("F5 validation unexpectedly received WM_QUIT.");
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
            Name = "Doroti F5 platform STA validation",
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        if (!complete.Wait(TestTimeout) || !thread.Join(TestTimeout))
            throw new TimeoutException("F5 platform/raster validation did not terminate within 20 minutes.");
        if (failure is not null)
            throw new InvalidOperationException("F5 platform/raster validation failed.", failure);
        return result ?? throw new InvalidOperationException("F5 validation returned no result.");
    }

    private static F5Options ParseOptions(string[] args)
    {
        string? evidencePath = null;
        string? sourceFingerprint = null;
        string? publishedExecutableSha256 = null;
        for (var index = 0; index < args.Length; index += 2)
        {
            if (index + 1 >= args.Length)
                throw new ArgumentException("Every F5 validator option requires a value.");
            switch (args[index])
            {
                case "--evidence": evidencePath = args[index + 1]; break;
                case "--source-fingerprint": sourceFingerprint = args[index + 1]; break;
                case "--published-executable-sha256": publishedExecutableSha256 = args[index + 1]; break;
                default:
                    throw new ArgumentException(
                        "Usage: Doroti.Validation.WindowsAppSdkFlutterResizeHandshake " +
                        "[--evidence <path>] [--source-fingerprint <sha256>] " +
                        "[--published-executable-sha256 <sha256>]");
            }
        }
        return new F5Options(evidencePath, sourceFingerprint, publishedExecutableSha256);
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

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetWindowRect(nint hwnd, out NativeRect rect);

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

    private enum TerminalExercise
    {
        TimedOut,
        Failed,
        Superseded,
        Suspended,
    }

    private sealed record ResizeDirection(string Name, bool AnchorRight, bool AnchorBottom);

    private sealed record F5Options(
        string? EvidencePath,
        string? SourceFingerprint,
        string? PublishedExecutableSha256);

    private sealed record DirectionResult(
        string Name,
        int TransactionCount,
        bool ProtocolIdentical,
        bool StateOrderExact,
        int ExactPresentCount,
        int TimeoutCount,
        int TerminalMissingCount,
        int TerminalDuplicateCount);

    private sealed class DirectionAccumulator
    {
        private bool _protocolIdentical = true;
        private bool _stateOrderExact = true;
        private int _exactPresentCount;
        private int _timeoutCount;
        private int _terminalMissingCount;

        internal DirectionAccumulator(string name) => Name = name;

        internal string Name { get; }

        internal int TransactionCount { get; private set; }

        internal void Record(FlutterWindowsResizeTransactionSnapshot transaction, FlutterWindowsResizePollResult poll)
        {
            TransactionCount++;
            var expected = new[]
            {
                FlutterWindowsResizeState.ResizeStarted,
                FlutterWindowsResizeState.FrameGenerated,
                FlutterWindowsResizeState.SurfaceReady,
                FlutterWindowsResizeState.Presented,
                FlutterWindowsResizeState.Done,
            };
            _protocolIdentical &= transaction.StateHistory.SequenceEqual(expected);
            _stateOrderExact &= transaction.StateHistory.SequenceEqual(expected);
            if (transaction.Terminal == FlutterWindowsResizeTerminal.Done &&
                poll.Terminal == FlutterWindowsResizeTerminal.Done)
            {
                _exactPresentCount++;
            }
            if (transaction.Terminal == FlutterWindowsResizeTerminal.TimedOut) _timeoutCount++;
            if (transaction.Terminal is null) _terminalMissingCount++;
        }

        internal DirectionResult ToResult(int expectedCycles)
        {
            if (TransactionCount != expectedCycles)
                throw new InvalidOperationException($"F5 {Name} ran {TransactionCount}, expected {expectedCycles} cycles.");
            return new(
                Name,
                TransactionCount,
                _protocolIdentical,
                _stateOrderExact,
                _exactPresentCount,
                _timeoutCount,
                _terminalMissingCount,
                TerminalDuplicateCount: 0);
        }
    }

    private sealed record FaultTimeoutResult(
        int InjectionCount,
        int TimeoutTerminalCount,
        bool NoUiDeadlock,
        bool NoInfiniteWait,
        bool TerminalExactlyOnce,
        bool ChildRectReobserved,
        bool LatestRedrawRequested);

    private sealed record TerminalCaseResult(
        string Name,
        int TerminalCount,
        int TerminalMissingCount,
        int TerminalDuplicateCount,
        bool ExactlyOnce);

    private sealed record F5ExerciseResult(
        FlutterWindowsAngleEglWindowSurfaceSnapshot ActiveSurface,
        FlutterWindowsAngleEglWindowSurfaceSnapshot DisposedSurface,
        FlutterWindowsResizeHandshakeSnapshot Handshake,
        DirectionResult[] Directions,
        FaultTimeoutResult FaultTimeout,
        TerminalCaseResult[] TerminalCases,
        bool AllDwmFlushesFollowDone,
        string[] MauiOrXamlAssemblyNames);

    private sealed class QueueEngineTaskRunner : IFlutterWindowsEngineTaskRunner
    {
        private readonly ConcurrentQueue<Action> _tasks = new();

        public void PostEngineTask(Action task)
        {
            ArgumentNullException.ThrowIfNull(task);
            _tasks.Enqueue(task);
        }

        public bool TryRunOneTask()
        {
            if (!_tasks.TryDequeue(out var task)) return false;
            task();
            return true;
        }
    }

    /// <summary>
    /// Queueing is deliberately asynchronous: the platform task returns to
    /// its bounded engine-only poll while the MTA owns F4 and waits for the
    /// post-Done platform-unblocked signal before DwmFlush.
    /// </summary>
    private sealed class AsyncF4Raster : IFlutterWindowsResizeRaster
    {
        private readonly DedicatedRasterThread _rasterThread;
        private FlutterWindowsResizeRasterPresenter? _presenter;
        private int _timeoutInjectionPending;

        internal AsyncF4Raster(DedicatedRasterThread rasterThread) =>
            _rasterThread = rasterThread ?? throw new ArgumentNullException(nameof(rasterThread));

        internal void SetPresenter(FlutterWindowsResizeRasterPresenter presenter) =>
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));

        internal void InjectTimeoutForNextRender() => Interlocked.Exchange(ref _timeoutInjectionPending, 1);

        public ValueTask<FlutterWindowsResizeRasterPresentationResult> RenderExactAsync(
            FlutterWindowsResizeRequest request,
            FlutterWindowsResizeFrame frame,
            CancellationToken cancellationToken = default)
        {
            if (Interlocked.Exchange(ref _timeoutInjectionPending, 0) != 0)
            {
                var never = new TaskCompletionSource<FlutterWindowsResizeRasterPresentationResult>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
                return new ValueTask<FlutterWindowsResizeRasterPresentationResult>(never.Task);
            }
            var presenter = _presenter ?? throw new InvalidOperationException(
                "F5 async raster was used before its MTA F4 presenter was ready.");
            var task = _rasterThread.PostAsync(() =>
                presenter.RenderExactAsync(request, frame, cancellationToken).GetAwaiter().GetResult());
            return new ValueTask<FlutterWindowsResizeRasterPresentationResult>(task);
        }
    }

    private sealed class UnexpectedRaster : IFlutterWindowsResizeRaster
    {
        public ValueTask<FlutterWindowsResizeRasterPresentationResult> RenderExactAsync(
            FlutterWindowsResizeRequest request,
            FlutterWindowsResizeFrame frame,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("This terminal-case raster should never receive an exact present request.");
    }

    private sealed class NeverCompletingRaster : IFlutterWindowsResizeRaster
    {
        public ValueTask<FlutterWindowsResizeRasterPresentationResult> RenderExactAsync(
            FlutterWindowsResizeRequest request,
            FlutterWindowsResizeFrame frame,
            CancellationToken cancellationToken = default)
        {
            var never = new TaskCompletionSource<FlutterWindowsResizeRasterPresentationResult>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            return new ValueTask<FlutterWindowsResizeRasterPresentationResult>(never.Task);
        }
    }

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
                Name = "Doroti F5 dedicated MTA raster validation",
            };
            _thread.SetApartmentState(ApartmentState.MTA);
            _thread.Start();
            if (!_started.Wait(TestTimeout))
                throw new TimeoutException("F5 dedicated raster thread did not start within 20 minutes.");
            if (_threadFailure is not null)
                throw new InvalidOperationException("F5 dedicated raster thread failed during startup.", _threadFailure);
        }

        internal T Invoke<T>(Func<T> action)
        {
            var task = PostAsync(action);
            if (!task.Wait(TestTimeout))
                throw new TimeoutException("F5 raster operation did not complete within 20 minutes.");
            return task.GetAwaiter().GetResult();
        }

        internal Task<T> PostAsync<T>(Func<T> action)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            ArgumentNullException.ThrowIfNull(action);
            if (_threadFailure is not null)
                throw new InvalidOperationException("F5 dedicated raster thread is unavailable.", _threadFailure);
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
            return completion.Task;
        }

        internal void Drain() => _ = Invoke(static () => 0);

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _work.CompleteAdding();
            if (!_thread.Join(TestTimeout))
                throw new TimeoutException("F5 dedicated raster thread did not stop within 20 minutes.");
            _work.Dispose();
            _started.Dispose();
            if (_threadFailure is not null)
                throw new InvalidOperationException("F5 dedicated raster thread terminated with an error.", _threadFailure);
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
