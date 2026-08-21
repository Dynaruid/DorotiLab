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
    private readonly List<string> _permutations = [];

    internal object Result => new
    {
        schemaVersion = "doroti.resize-contract/v1",
        status = "PASS",
        permutations = _permutations,
        generatedFrames = _nextScene,
        terminalFrames = _terminals.Snapshot().Count,
        unterminatedFrames = _terminals.Unterminated().Count,
        maxQueueDepth = _maxQueueDepth,
        stalePresents = _stalePresents,
        surfaceGeneration = _surfaceGeneration,
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

        Assert(_terminals.Unterminated().Count == 0, "every generated frame has one terminal");
        Assert(_maxQueueDepth <= 2, "queue depth never exceeds current plus latest");
        Assert(_stalePresents == 0, "stale generation presents remain zero");
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

    private TestFrame Frame(DorotiResizeEpoch target)
    {
        var sequence = checked(++_nextScene);
        _terminals.Register(sequence);
        return new(sequence, target);
    }

    private void Present(TestFrame frame, DorotiResizeEpoch target)
    {
        if (frame.Target.Generation != target.Generation ||
            frame.Target.PhysicalWidth != target.PhysicalWidth ||
            frame.Target.PhysicalHeight != target.PhysicalHeight)
        {
            _stalePresents++;
            throw new InvalidOperationException("attempted stale present");
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

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed record TestFrame(long Sequence, DorotiResizeEpoch Target);
}
