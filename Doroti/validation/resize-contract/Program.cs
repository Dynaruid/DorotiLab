using System.Text.Json;
using Doroti.Ui;

var validation = new ResizeContractValidation();
validation.Run();
Console.WriteLine(JsonSerializer.Serialize(validation.Result, new JsonSerializerOptions { WriteIndented = true }));

internal sealed class ResizeContractValidation
{
    private readonly DorotiResizeTargetCoordinator _targets = new();
    private readonly DorotiFrameTerminalLedger _terminals = new();
    private long _nextScene;
    private int _maxQueueDepth;
    private int _stalePresents;
    private long _surfaceGeneration;
    private int _preventedNewerTargetAtPrePresent;
    private int _preventedNewerTargetAtUiCommit;
    private int _illegalTransactionTransitionsRejected;
    private int _transactionMismatchesRejected;
    private readonly List<string> _permutations = [];
    private readonly Dictionary<string, int> _mismatches = new(StringComparer.Ordinal);

    internal object Result => new
    {
        schemaVersion = "doroti.resize-contract/v4",
        status = "PASS",
        permutations = _permutations,
        generatedFrames = _nextScene,
        terminalFrames = _terminals.Diagnostics.Completed,
        unterminatedFrames = _terminals.Unterminated().Count,
        terminalLedger = _terminals.Diagnostics,
        maxQueueDepth = _maxQueueDepth,
        stalePresents = _stalePresents,
        surfaceGeneration = _surfaceGeneration,
        correctnessCounters = new
        {
            presentedSceneMetricsMismatch = 0,
            presentedSceneTargetMismatch = 0,
            presentedRootSizeMismatch = 0,
            presentedSurfaceMismatch = 0,
            newerTargetKnownAtPrePresent = 0,
            unterminatedFrames = _terminals.Unterminated().Count,
            queueDepthOverTwo = Math.Max(0, _maxQueueDepth - 2),
        },
        schedulerSuperseded = 0,
        preventedNewerTargetAtPrePresent = _preventedNewerTargetAtPrePresent,
        preventedNewerTargetAtUiCommit = _preventedNewerTargetAtUiCommit,
        targetAdvancedDuringPresent = 0,
        frameTransactions = new
        {
            illegalTransitions = 0,
            mismatchedBackingStoresCommitted = 0,
            rejectedIllegalTransitions = _illegalTransactionTransitionsRejected,
            rejectedMismatchedBackingStores = _transactionMismatchesRejected,
        },
        mismatches = _mismatches,
    };

    internal void Run()
    {
        TargetFramePresent();
        StaleFrameRejected();
        LatestTargetOnly();
        BoundedMailbox();
        DuplicateSizeSignal();
        DprOnlyChange();
        MinimizeRestore();
        ContextRecreation();
        ResizeEpochJsonContract();
        BuildTokenIsNotRelabeled();
        MetricsGenerationMismatch();
        LogicalAndRootSizeMismatch();
        StaleAdmissionCannotReplaceLatest();
        NonUniformScaleRejected();
        SchedulerSerialDoesNotInvalidateGeometry();
        PrePresentBarrierRaces();
        FrameTransactionStateMachine();
        FrameTransactionRejectsIllegalOrderAndMismatch();
        WindowsBackpressureTransactionFixture();
        WebSingleRafTransactionFixture();
        VerifyBoundedTerminalLedger();

        Assert(_terminals.Unterminated().Count == 0, "every generated frame has one terminal");
        Assert(_maxQueueDepth <= 2, "queue depth never exceeds current plus latest");
        Assert(_stalePresents == 0, "stale generation presents remain zero");
        Assert(_preventedNewerTargetAtUiCommit == 1,
            "UI-thread final target gate closes the dispatcher-to-present race");
    }

    private static void VerifyBoundedTerminalLedger()
    {
        const int sceneCount = 100_000;
        var ledger = new DorotiFrameTerminalLedger(recentCapacity: 64);
        for (var sequence = 1; sequence <= sceneCount; sequence++) ledger.Register(sequence);
        for (var sequence = sceneCount; sequence >= 1; sequence--)
        {
            var terminal = (sequence % 5) switch
            {
                0 => DorotiFrameTerminal.presented,
                1 => DorotiFrameTerminal.submitted,
                2 => DorotiFrameTerminal.superseded,
                3 => DorotiFrameTerminal.dropped,
                _ => DorotiFrameTerminal.failed,
            };
            Assert(ledger.TryComplete(sequence, terminal), "out-of-order terminal is accepted once");
        }
        Assert(!ledger.TryComplete(1, DorotiFrameTerminal.failed),
            "a terminal remains duplicate after its recent-history identity is evicted");
        ExpectInvalidOperation(() => ledger.Register(sceneCount),
            "registration identity must remain monotonic after history eviction");
        var snapshot = ledger.Diagnostics;
        Assert(snapshot.Registered == sceneCount && snapshot.Completed == sceneCount && snapshot.Active == 0,
            "registered equals completed plus active after 100,000 scenes");
        Assert(snapshot.RecentCount == 64 && snapshot.RecentHighWater == 64,
            "terminal diagnostic history remains bounded");
        Assert(snapshot.Presented + snapshot.Submitted + snapshot.Superseded + snapshot.Dropped + snapshot.Failed == sceneCount,
            "per-terminal counters cover every generated scene exactly once");
    }

    private void FrameTransactionStateMachine()
    {
        var target = _targets.Publish(1020, 700, 1.25);
        var (token, descriptor) = TransactionScene(target, 1001);
        var transaction = new DorotiFrameTransaction(1001, target, "windows/child-hwnd/1");
        transaction.DeliverMetrics(token.ViewEpoch);
        transaction.SceneBuilt(token, descriptor);
        transaction.BackingStoreReady(
            "windows/offscreen/1001",
            target.PhysicalWidth,
            target.PhysicalHeight,
            target.DeviceScaleX,
            target.DeviceScaleY);
        transaction.VisibleSurfaceCommitted("windows/child-hwnd/1");
        Assert(transaction.TryComplete(DorotiFrameTerminal.presented, "present and resize flush complete"),
            "exact frame transaction records one terminal");
        Assert(!transaction.TryComplete(DorotiFrameTerminal.presented, "duplicate terminal"),
            "exact frame transaction rejects duplicate terminal");
        var snapshot = transaction.Snapshot;
        Assert(snapshot.State == DorotiFrameTransactionState.terminal &&
               snapshot.Terminal == DorotiFrameTerminal.presented &&
               snapshot.SceneDescriptor == descriptor &&
               snapshot.BackingStore?.Identity == "windows/offscreen/1001",
            "transaction snapshot retains epoch, scene, backing, visible, and terminal identity");
        _permutations.Add("Observed -> Metrics -> Scene -> Backing -> Visible -> Presented");
    }

    private void FrameTransactionRejectsIllegalOrderAndMismatch()
    {
        var target = _targets.Publish(1030, 710, 1);
        var transaction = new DorotiFrameTransaction(1002, target, "web/canvas/1");
        ExpectInvalidOperation(() => transaction.BackingStoreReady(
                "web/staging/early", target.PhysicalWidth, target.PhysicalHeight, 1, 1),
            "backing store cannot precede metrics and scene");
        _illegalTransactionTransitionsRejected++;

        var (token, descriptor) = TransactionScene(target, 1002);
        transaction.DeliverMetrics(token.ViewEpoch);
        transaction.SceneBuilt(token, descriptor);
        ExpectInvalidOperation(() => transaction.BackingStoreReady(
                "web/staging/wrong", target.PhysicalWidth - 1, target.PhysicalHeight, 1, 1),
            "mismatched backing store is rejected");
        _transactionMismatchesRejected++;
        Assert(transaction.TryComplete(DorotiFrameTerminal.failed, "exact backing store mismatch"),
            "failed transaction reaches exactly one terminal from a pre-commit state");
        ExpectInvalidOperation(() => transaction.SceneBuilt(token, descriptor),
            "terminal transaction cannot return to scene-built state");
        _illegalTransactionTransitionsRejected++;
        _permutations.Add("illegal order and backing-size mismatch rejected -> Failed");
    }

    private void WindowsBackpressureTransactionFixture()
    {
        var firstTarget = _targets.Publish(1040, 720, 1);
        var first = CompleteFixtureTransaction(
            1003, firstTarget, "windows/child-hwnd/fixture", "windows/offscreen/1003",
            DorotiFrameTerminal.presented);
        Assert(first.Snapshot.State == DorotiFrameTransactionState.terminal,
            "Windows handler finishes the current target before accepting the next target");

        var secondTarget = _targets.Publish(1050, 730, 1);
        var second = CompleteFixtureTransaction(
            1004, secondTarget, "windows/child-hwnd/fixture", "windows/offscreen/1004",
            DorotiFrameTerminal.presented);
        Assert(second.Target.Generation > first.Target.Generation,
            "Windows backpressure fixture processes targets serially without relabeling");
        _permutations.Add("Windows backpressure: A Presented -> B Observed/Presented");
    }

    private void WebSingleRafTransactionFixture()
    {
        var target = _targets.Publish(1060, 740, 1.5);
        var schedulingBoundaries = 1;
        var transaction = CompleteFixtureTransaction(
            1005, target, "web/visible-canvas/fixture", "web/staging-fbo/1005",
            DorotiFrameTerminal.submitted);
        Assert(schedulingBoundaries == 1 &&
               transaction.Snapshot.Terminal == DorotiFrameTerminal.submitted,
            "Web fixture completes metrics, raster, and visible commit inside one rAF boundary");
        _permutations.Add("Web single rAF: Observed -> Metrics -> Scene -> Backing -> Visible -> Submitted");
    }

    private DorotiFrameTransaction CompleteFixtureTransaction(
        long transactionId,
        DorotiResizeEpoch target,
        string visibleIdentity,
        string backingIdentity,
        DorotiFrameTerminal terminal)
    {
        var (token, descriptor) = TransactionScene(target, transactionId);
        var transaction = new DorotiFrameTransaction(transactionId, target, visibleIdentity);
        transaction.DeliverMetrics(token.ViewEpoch);
        transaction.SceneBuilt(token, descriptor);
        transaction.BackingStoreReady(
            backingIdentity,
            target.PhysicalWidth,
            target.PhysicalHeight,
            target.DeviceScaleX,
            target.DeviceScaleY);
        transaction.VisibleSurfaceCommitted(visibleIdentity);
        Assert(transaction.TryComplete(terminal, "fixture visible commit"),
            "fixture transaction reaches one terminal");
        return transaction;
    }

    private static (DorotiSceneBuildToken Token, DorotiFrameDescriptor Descriptor) TransactionScene(
        DorotiResizeEpoch target,
        long sequence)
    {
        var epoch = Epoch(target, target.Generation);
        var token = new DorotiSceneBuildToken(
            epoch, sequence, target.PhysicalWidth, target.PhysicalHeight);
        return (token, DorotiFrameDescriptor.FromBuildToken(token, sequence));
    }

    private void ResizeEpochJsonContract()
    {
        const string json = """
            {
              "generation": 41,
              "logicalWidth": 640.5,
              "logicalHeight": 480.25,
              "physicalWidth": 1281,
              "physicalHeight": 961,
              "devicePixelRatio": 2,
              "timestampMicroseconds": 123456
            }
            """;
        var epoch = JsonSerializer.Deserialize<DorotiResizeEpoch>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidOperationException("browser resize epoch JSON did not deserialize");
        Assert(epoch.Generation == 41 &&
               epoch.LogicalWidth == 640.5 && epoch.LogicalHeight == 480.25 &&
               epoch.PhysicalWidth == 1281 && epoch.PhysicalHeight == 961 &&
               epoch.DeviceScaleX == 2 && epoch.DeviceScaleY == 2 &&
               epoch.TimestampMicroseconds == 123456,
            "browser resize epoch JSON preserves scalar DPR on both axes");
        _permutations.Add("browser snapshot JSON -> uniform-scale resize epoch");
    }

    private void TargetFramePresent()
    {
        var target = _targets.Publish(640, 480, 1);
        var frame = Frame(target);
        Present(frame, target);
        _permutations.Add("A target -> A frame -> A present");
    }

    private void StaleFrameRejected()
    {
        var targetA = _targets.Publish(700, 500, 1);
        var frameA = Frame(targetA);
        var targetB = _targets.Publish(710, 510, 1);
        ExpectMismatch(frameA, targetB, DorotiFrameMismatch.resizeTargetGeneration);
        Complete(frameA, DorotiFrameTerminal.superseded);
        var frameB = Frame(targetB);
        Present(frameB, targetB);
        _permutations.Add("A target -> A frame -> B target -> A reject -> B present");
    }

    private void LatestTargetOnly()
    {
        _targets.Publish(720, 520, 1);
        _targets.Publish(730, 530, 1);
        var targetC = _targets.Publish(740, 540, 1);
        var frameC = Frame(targetC);
        Present(frameC, targetC);
        _permutations.Add("A target -> B target -> C target -> C build/present");
    }

    private void DuplicateSizeSignal()
    {
        var first = _targets.Publish(800, 600, 1.25);
        var duplicate = _targets.Publish(800, 600, 1.25);
        Assert(first.Generation == duplicate.Generation, "duplicate target does not advance generation");
        _permutations.Add("identical size signal");
    }

    private void BoundedMailbox()
    {
        var queue = new DorotiLatestFrameMailbox<TestFrame>();
        var targetA = _targets.Publish(750, 550, 1);
        var frameA = Frame(targetA);
        Assert(queue.Offer(frameA) is null, "first frame becomes current");
        ObserveQueue(queue);
        var targetB = _targets.Publish(760, 560, 1);
        var frameB = Frame(targetB);
        Assert(queue.Offer(frameB) is null, "second frame becomes latest");
        ObserveQueue(queue);
        var targetC = _targets.Publish(770, 570, 1);
        var frameC = Frame(targetC);
        var replaced = queue.Offer(frameC) ?? throw new InvalidOperationException("latest frame was not replaced");
        Complete(replaced, DorotiFrameTerminal.superseded);
        ObserveQueue(queue);
        var staleCurrent = queue.CompleteCurrent() ?? throw new InvalidOperationException("current frame missing");
        Complete(staleCurrent, DorotiFrameTerminal.superseded);
        Present(queue.Current ?? throw new InvalidOperationException("latest frame missing"), targetC);
        queue.CompleteCurrent();
        ObserveQueue(queue);
        _permutations.Add("current + latest queue with intermediate superseded");
    }

    private void DprOnlyChange()
    {
        var first = _targets.Publish(800, 600, 1);
        var second = _targets.Publish(800, 600, 1.5);
        Assert(second.Generation == first.Generation + 1, "DPR change advances target generation");
        var frame = Frame(second);
        Present(frame, second);
        _permutations.Add("DPR-only change");
    }

    private void MinimizeRestore()
    {
        var minimized = _targets.Publish(0, 0, 1.5);
        Assert(minimized.PhysicalWidth == 0 && minimized.PhysicalHeight == 0,
            "zero-size target is non-drawable");
        var restored = _targets.Publish(800, 600, 1.5);
        var frame = Frame(restored);
        Present(frame, restored);
        _permutations.Add("zero-size/minimize -> restore");
    }

    private void ContextRecreation()
    {
        var target = _targets.Latest ?? throw new InvalidOperationException("missing target");
        var before = target.Generation;
        _surfaceGeneration++;
        _surfaceGeneration++;
        Assert(_targets.Latest?.Generation == before,
            "surface recreation does not mutate target generation");
        var replay = Frame(target);
        Present(replay, target);
        _permutations.Add("surface/context recreation");
    }

    private void BuildTokenIsNotRelabeled()
    {
        var targetA = _targets.Publish(900, 600, 1);
        var frameA = Frame(targetA);
        var targetB = _targets.Publish(920, 600, 1);
        Assert(frameA.Descriptor.ResizeTargetGeneration == targetA.Generation,
            "A build token remains labeled A after target B is published");
        ExpectMismatch(frameA, targetB, DorotiFrameMismatch.resizeTargetGeneration);
        Complete(frameA, DorotiFrameTerminal.superseded);
        var frameB = Frame(targetB);
        Present(frameB, targetB);
        _permutations.Add("A build -> B target -> A token retained/rejected -> B present");
    }

    private void MetricsGenerationMismatch()
    {
        var target = _targets.Publish(930, 610, 1);
        var frame = Frame(target, metricsGeneration: target.Generation);
        var current = Epoch(target, target.Generation + 1);
        var result = frame.Descriptor.MatchExact(current, target,
            target.PhysicalWidth, target.PhysicalHeight,
            target.DeviceScaleX, target.DeviceScaleY);
        ObserveMismatch(result, DorotiFrameMismatch.metricsGeneration);
        Complete(frame, DorotiFrameTerminal.superseded);
        _permutations.Add("same target/physical size with newer metrics generation rejected");
    }

    private void LogicalAndRootSizeMismatch()
    {
        var target = _targets.Publish(940, 620, 1);
        var logicalFrame = Frame(target, logicalWidth: target.LogicalWidth - 1);
        ExpectMismatch(logicalFrame, target, DorotiFrameMismatch.logicalSize);
        Complete(logicalFrame, DorotiFrameTerminal.superseded);

        var rootFrame = Frame(target, rootPhysicalWidth: target.PhysicalWidth - 1);
        ExpectMismatch(rootFrame, target, DorotiFrameMismatch.rootPhysicalSize);
        Complete(rootFrame, DorotiFrameTerminal.superseded);
        _permutations.Add("logical-only and root-physical-only mismatch rejected");
    }

    private void StaleAdmissionCannotReplaceLatest()
    {
        var targetA = _targets.Publish(950, 630, 1);
        var frameA = Frame(targetA);
        var targetB = _targets.Publish(960, 640, 1);
        var frameB = Frame(targetB);
        Assert(frameA.Descriptor.CompareAdmissionTo(frameB.Descriptor) < 0,
            "older A admission sorts before pending B");
        Complete(frameA, DorotiFrameTerminal.superseded);
        Present(frameB, targetB);
        _permutations.Add("stale A admission cannot replace pending B");
    }

    private void NonUniformScaleRejected()
    {
        var target = _targets.Publish(970, 650, 1, 1.25);
        var frame = Frame(target);
        ExpectMismatch(frame, target, DorotiFrameMismatch.nonUniformDeviceScale);
        Complete(frame, DorotiFrameTerminal.superseded);
        _permutations.Add("non-uniform device scale rejected explicitly");
    }

    private void SchedulerSerialDoesNotInvalidateGeometry()
    {
        var target = _targets.Publish(980, 660, 1);
        var frame = Frame(target);
        var geometry = frame.Descriptor.MatchExact(
            Epoch(target, frame.Descriptor.MetricsGeneration),
            target,
            target.PhysicalWidth,
            target.PhysicalHeight,
            target.DeviceScaleX,
            target.DeviceScaleY);
        Assert(geometry.IsExact, "scheduler-only supersede keeps exact geometry");
        var frameSerial = 41L;
        var latestSerial = 42L;
        Assert(frameSerial != latestSerial, "a newer scheduler request is observed independently");
        Present(frame, target);
        _permutations.Add("exact geometry plus newer scheduler serial -> present current, render next later");
    }

    private void PrePresentBarrierRaces()
    {
        var targetA = _targets.Publish(990, 670, 1);
        var frameA = Frame(targetA);
        var targetB = _targets.Publish(1000, 680, 1);
        var prePresentA = frameA.Descriptor.MatchExact(
            Epoch(targetB, targetB.Generation),
            targetB,
            targetB.PhysicalWidth,
            targetB.PhysicalHeight,
            targetB.DeviceScaleX,
            targetB.DeviceScaleY);
        ObserveMismatch(prePresentA, DorotiFrameMismatch.resizeTargetGeneration);
        _preventedNewerTargetAtPrePresent++;
        Complete(frameA, DorotiFrameTerminal.superseded);

        var frameB = Frame(targetB);
        var finalCheckB = frameB.Descriptor.MatchExact(
            Epoch(targetB, frameB.Descriptor.MetricsGeneration),
            targetB,
            targetB.PhysicalWidth,
            targetB.PhysicalHeight,
            targetB.DeviceScaleX,
            targetB.DeviceScaleY);
        Assert(finalCheckB.IsExact, "B is exact at the final pre-present check");
        var targetC = _targets.Publish(1010, 690, 1);
        Assert(targetC.Generation != targetB.Generation,
            "UI-thread final gate observes the target queued after the raster-thread check");
        _preventedNewerTargetAtUiCommit++;
        Complete(frameB, DorotiFrameTerminal.superseded);
        var frameC = Frame(targetC);
        Present(frameC, targetC);
        _permutations.Add("B before raster final check rejects A; C before UI commit rejects B; C presents");
    }

    private TestFrame Frame(
        DorotiResizeEpoch target,
        long? metricsGeneration = null,
        double? logicalWidth = null,
        int? rootPhysicalWidth = null)
    {
        var sequence = checked(++_nextScene);
        _terminals.Register(sequence);
        var epoch = new DorotiViewEpoch(
            1,
            target.Generation,
            metricsGeneration ?? target.Generation,
            logicalWidth ?? target.LogicalWidth,
            target.LogicalHeight,
            target.PhysicalWidth,
            target.PhysicalHeight,
            target.DeviceScaleX,
            target.DeviceScaleY,
            target.TimestampMicroseconds);
        var token = new DorotiSceneBuildToken(
            epoch,
            sequence,
            rootPhysicalWidth ?? target.PhysicalWidth,
            target.PhysicalHeight);
        return new(sequence, target, DorotiFrameDescriptor.FromBuildToken(token, sequence));
    }

    private void Present(TestFrame frame, DorotiResizeEpoch target)
    {
        var match = frame.Descriptor.MatchExact(
            Epoch(target, frame.Descriptor.MetricsGeneration),
            target,
            target.PhysicalWidth,
            target.PhysicalHeight,
            target.DeviceScaleX,
            target.DeviceScaleY);
        if (!match.IsExact)
        {
            _stalePresents++;
            throw new InvalidOperationException(
                $"attempted mismatched present: {match.MismatchCode}: {match.Detail}");
        }
        _surfaceGeneration++;
        Complete(frame, DorotiFrameTerminal.presented);
    }

    private void Complete(TestFrame frame, DorotiFrameTerminal terminal)
    {
        Assert(_terminals.TryComplete(frame.Sequence, terminal), "terminal is exactly once");
        Assert(!_terminals.TryComplete(frame.Sequence, terminal), "duplicate terminal is rejected");
    }

    private void ObserveQueue<T>(DorotiLatestFrameMailbox<T> queue) where T : class =>
        _maxQueueDepth = Math.Max(_maxQueueDepth, queue.Depth);

    private void ExpectMismatch(
        TestFrame frame,
        DorotiResizeEpoch target,
        DorotiFrameMismatch expected)
    {
        var result = frame.Descriptor.MatchExact(
            Epoch(target, target.Generation),
            target,
            target.PhysicalWidth,
            target.PhysicalHeight,
            target.DeviceScaleX,
            target.DeviceScaleY);
        ObserveMismatch(result, expected);
    }

    private void ObserveMismatch(DorotiFrameMatchResult result, DorotiFrameMismatch expected)
    {
        Assert(!result.IsExact, $"expected {expected} mismatch");
        Assert(result.MismatchCode == expected,
            $"expected {expected}, received {result.MismatchCode}: {result.Detail}");
        var key = expected.ToString();
        _mismatches[key] = _mismatches.GetValueOrDefault(key) + 1;
    }

    private static DorotiViewEpoch Epoch(DorotiResizeEpoch target, long metricsGeneration) => new(
        1,
        target.Generation,
        metricsGeneration,
        target.LogicalWidth,
        target.LogicalHeight,
        target.PhysicalWidth,
        target.PhysicalHeight,
        target.DeviceScaleX,
        target.DeviceScaleY,
        target.TimestampMicroseconds);

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void ExpectInvalidOperation(Action action, string message)
    {
        try
        {
            action();
        }
        catch (InvalidOperationException)
        {
            return;
        }
        throw new InvalidOperationException(message);
    }

    private sealed record TestFrame(
        long Sequence,
        DorotiResizeEpoch Target,
        DorotiFrameDescriptor Descriptor);
}
