using System.Diagnostics;

namespace Doroti.Host.WindowsAppSdk;

internal enum WindowsResizeTerminal
{
    Presented,
    Superseded,
    Failed,
}

internal sealed record WindowsResizeTarget(
    ulong ViewId,
    long Generation,
    int WidthPx,
    int HeightPx,
    double Scale,
    ulong CausalFrameId,
    long AcceptedTimestamp);

internal sealed record WindowsResizeReceipt(
    WindowsResizeTarget Target,
    WindowsResizeTerminal Terminal,
    bool PlatformWaitTimedOut,
    string Detail,
    long TerminalTimestamp);

internal sealed record WindowsResizeWaitResult(
    WindowsResizeReceipt? Receipt,
    bool TimedOut,
    TimeSpan Elapsed);

internal sealed record WindowsResizeCoordinatorSnapshot(
    int QueueDepth,
    int MaximumQueueDepth,
    long AcceptedCount,
    long PresentedCount,
    long SupersededCount,
    long FailedCount,
    long DuplicateTerminalCount,
    long ExactAdmissionMismatchCount,
    long StalePresentPreventedCount,
    long PlatformWaitTimeoutCount,
    int UnterminatedCount,
    IReadOnlyList<WindowsResizeReceipt> Receipts);

internal sealed class WindowsManagedResizeCoordinator : IDisposable
{
    private sealed class Entry(WindowsResizeTarget target)
    {
        internal WindowsResizeTarget Target { get; } = target;
        internal ManualResetEventSlim Completion { get; } = new(false);
        internal WindowsResizeReceipt? Receipt { get; set; }
        internal bool PlatformWaitTimedOut { get; set; }
    }

    private readonly object _gate = new();
    private readonly TimeSpan _maximumWait;
    private readonly Dictionary<long, Entry> _entries = [];
    private readonly List<WindowsResizeReceipt> _receipts = [];
    private Entry? _current;
    private Entry? _latest;
    private long _generation;
    private int _maximumQueueDepth;
    private long _duplicateTerminalCount;
    private long _exactAdmissionMismatchCount;
    private long _stalePresentPreventedCount;
    private long _platformWaitTimeoutCount;
    private bool _closed;

    internal WindowsManagedResizeCoordinator(TimeSpan? maximumWait = null)
    {
        _maximumWait = maximumWait ?? TimeSpan.FromMilliseconds(100);
        if (_maximumWait <= TimeSpan.Zero || _maximumWait > TimeSpan.FromMilliseconds(100))
            throw new ArgumentOutOfRangeException(nameof(maximumWait));
    }

    internal WindowsResizeTarget Publish(
        ulong viewId,
        int widthPx,
        int heightPx,
        double scale,
        ulong causalFrameId,
        long? externalGeneration = null)
    {
        if (viewId == 0) throw new ArgumentOutOfRangeException(nameof(viewId));
        if (widthPx < 0 || heightPx < 0) throw new ArgumentOutOfRangeException(nameof(widthPx));
        if (!double.IsFinite(scale) || scale <= 0) throw new ArgumentOutOfRangeException(nameof(scale));
        lock (_gate)
        {
            if (_closed) throw new InvalidOperationException("The resize coordinator is closed.");
            var generation = externalGeneration ?? checked(_generation + 1);
            if (generation <= _generation)
                throw new InvalidOperationException(
                    $"Resize generation {generation} is not newer than {_generation}.");
            _generation = generation;
            var target = new WindowsResizeTarget(
                viewId, generation, widthPx, heightPx, scale,
                causalFrameId, Stopwatch.GetTimestamp());
            var entry = new Entry(target);
            _entries.Add(target.Generation, entry);
            if (_current is null)
            {
                _current = entry;
            }
            else
            {
                if (_latest is not null)
                    CompleteCore(_latest, WindowsResizeTerminal.Superseded, "replaced pending target");
                _latest = entry;
            }
            _maximumQueueDepth = Math.Max(_maximumQueueDepth, QueueDepthCore());
            if (widthPx == 0 || heightPx == 0)
                CompleteCore(entry, WindowsResizeTerminal.Failed, "non-drawable lifecycle target");
            return target;
        }
    }

    internal WindowsResizeTarget? Current
    {
        get { lock (_gate) return _current?.Target; }
    }

    internal bool IsLatest(long generation)
    {
        lock (_gate) return LatestGenerationCore() == generation;
    }

    internal bool IsComplete(long generation)
    {
        lock (_gate)
            return _entries.TryGetValue(generation, out var entry) && entry.Receipt is not null;
    }

    internal bool ValidateExact(long generation, int widthPx, int heightPx)
    {
        lock (_gate)
        {
            if (!_entries.TryGetValue(generation, out var entry) ||
                entry.Target.WidthPx != widthPx || entry.Target.HeightPx != heightPx)
            {
                _exactAdmissionMismatchCount++;
                return false;
            }
            return true;
        }
    }

    internal bool TryComplete(
        long generation,
        WindowsResizeTerminal terminal,
        string detail,
        bool enforceLatest = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(detail);
        lock (_gate)
        {
            if (!_entries.TryGetValue(generation, out var entry))
                throw new InvalidOperationException($"Resize generation {generation} was not accepted.");
            if (entry.Receipt is not null)
            {
                _duplicateTerminalCount++;
                return false;
            }
            if (enforceLatest && terminal == WindowsResizeTerminal.Presented && LatestGenerationCore() != generation)
            {
                terminal = WindowsResizeTerminal.Superseded;
                detail = $"stale present prevented: {detail}";
                _stalePresentPreventedCount++;
            }
            CompleteCore(entry, terminal, detail);
            return true;
        }
    }

    internal WindowsResizeWaitResult WaitForCompletion(long generation)
    {
        Entry entry;
        lock (_gate)
        {
            if (!_entries.TryGetValue(generation, out entry!))
                throw new InvalidOperationException($"Resize generation {generation} was not accepted.");
        }
        var started = Stopwatch.GetTimestamp();
        var signaled = entry.Completion.Wait(_maximumWait);
        var elapsed = Stopwatch.GetElapsedTime(started);
        lock (_gate)
        {
            if (!signaled && entry.Receipt is null)
            {
                entry.PlatformWaitTimedOut = true;
                _platformWaitTimeoutCount++;
                return new(null, true, elapsed);
            }
            return new(entry.Receipt, false, elapsed);
        }
    }

    internal void Close()
    {
        lock (_gate)
        {
            if (_closed) return;
            _closed = true;
            foreach (var entry in _entries.Values.Where(value => value.Receipt is null).ToArray())
                CompleteCore(entry, WindowsResizeTerminal.Failed, "shutdown");
        }
    }

    internal WindowsResizeCoordinatorSnapshot Snapshot()
    {
        lock (_gate)
        {
            return new(
                QueueDepthCore(),
                _maximumQueueDepth,
                _entries.Count,
                _receipts.Count(value => value.Terminal == WindowsResizeTerminal.Presented),
                _receipts.Count(value => value.Terminal == WindowsResizeTerminal.Superseded),
                _receipts.Count(value => value.Terminal == WindowsResizeTerminal.Failed),
                _duplicateTerminalCount,
                _exactAdmissionMismatchCount,
                _stalePresentPreventedCount,
                _platformWaitTimeoutCount,
                _entries.Values.Count(value => value.Receipt is null),
                _receipts.ToArray());
        }
    }

    private void CompleteCore(Entry entry, WindowsResizeTerminal terminal, string detail)
    {
        if (entry.Receipt is not null) return;
        var receipt = new WindowsResizeReceipt(
            entry.Target, terminal, entry.PlatformWaitTimedOut, detail,
            Stopwatch.GetTimestamp());
        entry.Receipt = receipt;
        _receipts.Add(receipt);
        entry.Completion.Set();
        if (ReferenceEquals(_current, entry))
        {
            _current = _latest;
            _latest = null;
        }
        else if (ReferenceEquals(_latest, entry))
        {
            _latest = null;
        }
    }

    private long LatestGenerationCore() =>
        _latest?.Target.Generation ?? _current?.Target.Generation ?? 0;

    private int QueueDepthCore() =>
        (_current is null ? 0 : 1) + (_latest is null ? 0 : 1);

    public void Dispose()
    {
        Close();
        foreach (var entry in _entries.Values) entry.Completion.Dispose();
    }
}
