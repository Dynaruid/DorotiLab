using System.Threading.Channels;
using System.Diagnostics;

namespace Doroti.Framework.Rendering;

/// <summary>Opt-in numeric layout island. No RenderObject, callback, or native handle crosses threads.</summary>
public sealed class PreparedTreemapLayout : IDisposable, IAsyncDisposable
{
    public readonly record struct Box(double X, double Y, double Width, double Height);
    public sealed record Partition(IReadOnlyList<Box> Boxes, int ThreadId, long Start, long End);
    public sealed record Result(Partition Left, Partition Right, int OwnerThreadId, double ElapsedMs)
    {
        public double OverlapMs => Math.Max(0, Math.Min(Left.End, Right.End) - Math.Max(Left.Start, Right.Start)) * 1000d / Stopwatch.Frequency;
    }

    private sealed record Job(double[] Weights, Box Bounds, CancellationToken Cancellation, TaskCompletionSource<Partition> Completion);
    private readonly Channel<Job>[] _queues = [NewQueue(), NewQueue()];
    private readonly TaskCompletionSource[] _ready = [NewCompletion(), NewCompletion()];
    private readonly TaskCompletionSource[] _stopped = [NewCompletion(), NewCompletion()];
    private readonly CancellationTokenSource _shutdown = new();
    private int _busy, _disposed;

    public PreparedTreemapLayout()
    {
        for (var i = 0; i < 2; i++)
        {
            var lane = i;
            new Thread(() => Run(lane)) { IsBackground = true, Name = $"Doroti numeric layout {lane}" }.Start();
        }
    }

    private static TaskCompletionSource NewCompletion() => new(TaskCreationOptions.RunContinuationsAsynchronously);
    private static Channel<Job> NewQueue() => Channel.CreateBounded<Job>(new BoundedChannelOptions(1)
        { SingleReader = true, SingleWriter = true, AllowSynchronousContinuations = false });

    private void Run(int lane)
    {
        _ready[lane].TrySetResult();
        try
        {
            // Only these non-JS compute threads block. Owner submission uses TryWrite.
            while (_queues[lane].Reader.WaitToReadAsync().AsTask().GetAwaiter().GetResult())
            {
                while (_queues[lane].Reader.TryRead(out var job))
                {
                    try { job.Completion.TrySetResult(Compute(job.Weights, job.Bounds, job.Cancellation)); }
                    catch (OperationCanceledException) { job.Completion.TrySetCanceled(job.Cancellation); }
                    catch (Exception error) { job.Completion.TrySetException(error); }
                }
            }
        }
        finally { _stopped[lane].TrySetResult(); }
    }

    /// <summary>Await outside synchronous layout. At most one batch (two immutable jobs) is accepted.</summary>
    public async Task<Result> PrepareAsync(IReadOnlyList<double> left, IReadOnlyList<double> right,
        Box bounds, bool parallel, CancellationToken cancellation = default)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        if (Interlocked.CompareExchange(ref _busy, 1, 0) != 0)
            throw new InvalidOperationException("Only one layout batch may be in flight; coalesce changes on the owner.");
        try
        {
            var started = Stopwatch.GetTimestamp();
            var owner = Environment.CurrentManagedThreadId;
            if (!double.IsFinite(bounds.X) || !double.IsFinite(bounds.Y) || !double.IsFinite(bounds.Width) ||
                !double.IsFinite(bounds.Height) || !double.IsFinite(bounds.X + bounds.Width) ||
                !double.IsFinite(bounds.Y + bounds.Height) || bounds.Width <= 0 || bounds.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(bounds));
            var a = CopyWeights(left);
            var b = CopyWeights(right);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellation, _shutdown.Token);
            var token = linked.Token;
            token.ThrowIfCancellationRequested();
            var width = bounds.Width / 2;
            var first = new Box(bounds.X, bounds.Y, width, bounds.Height);
            var second = new Box(bounds.X + width, bounds.Y, bounds.Width - width, bounds.Height);
            Partition l, r;
            if (parallel)
            {
                // Yield to the owner event loop while WASM pthreads initialize. Never Wait/Join here.
                await Task.WhenAll(_ready.Select(ready => ready.Task)).WaitAsync(token);
                var jobs = new[] { new Job(a, first, token, new(TaskCreationOptions.RunContinuationsAsynchronously)),
                    new Job(b, second, token, new(TaskCreationOptions.RunContinuationsAsynchronously)) };
                // Dispose can race enqueue. Observe every submitted completion before releasing the batch.
                var submitted = new List<Task<Partition>>(2);
                try
                {
                    for (var i = 0; i < 2; i++)
                    {
                        if (!_queues[i].Writer.TryWrite(jobs[i])) throw new InvalidOperationException("Layout queue is full or closed.");
                        submitted.Add(jobs[i].Completion.Task);
                    }
                }
                catch
                {
                    linked.Cancel();
                    try { await Task.WhenAll(submitted); } catch (OperationCanceledException) { }
                    throw;
                }
                var partitions = await Task.WhenAll(submitted);
                l = partitions[0]; r = partitions[1];
            }
            else { l = Compute(a, first, token); r = Compute(b, second, token); }
            token.ThrowIfCancellationRequested();
            return new Result(l, r, owner, Stopwatch.GetElapsedTime(started).TotalMilliseconds);
        }
        finally { Volatile.Write(ref _busy, 0); }
    }

    private static double[] CopyWeights(IReadOnlyList<double> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source.Count is < 1 or > 65536) throw new ArgumentOutOfRangeException(nameof(source));
        var result = new double[source.Count];
        double total = 0;
        for (var i = 0; i < result.Length; i++)
        {
            var value = source[i];
            if (!double.IsFinite(value) || value <= 0) throw new ArgumentOutOfRangeException(nameof(source));
            var previous = total;
            total += value;
            if (!double.IsFinite(total) || total <= previous) throw new ArgumentOutOfRangeException(nameof(source));
            result[i] = value;
        }
        return result;
    }

    private static Partition Compute(double[] weights, Box bounds, CancellationToken cancellation)
    {
        var start = Stopwatch.GetTimestamp();
        var prefix = new double[weights.Length + 1];
        for (var i = 0; i < weights.Length; i++) prefix[i + 1] = prefix[i] + weights[i];
        var boxes = new Box[weights.Length];
        // Balanced index splits bound depth to 16; weight sums determine exact area ratios.
        void Layout(int begin, int end, Box box)
        {
            cancellation.ThrowIfCancellationRequested();
            if (end - begin == 1) { boxes[begin] = box; return; }
            var middle = begin + (end - begin) / 2;
            var fraction = (prefix[middle] - prefix[begin]) / (prefix[end] - prefix[begin]);
            if (box.Width >= box.Height)
            {
                var split = box.Width * fraction;
                Layout(begin, middle, new(box.X, box.Y, split, box.Height));
                Layout(middle, end, new(box.X + split, box.Y, box.Width - split, box.Height));
            }
            else
            {
                var split = box.Height * fraction;
                Layout(begin, middle, new(box.X, box.Y, box.Width, split));
                Layout(middle, end, new(box.X, box.Y + split, box.Width, box.Height - split));
            }
        }
        Layout(0, weights.Length, bounds);
        return new Partition(Array.AsReadOnly(boxes), Environment.CurrentManagedThreadId, start, Stopwatch.GetTimestamp());
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        _shutdown.Cancel();
        foreach (var queue in _queues) queue.Writer.TryComplete();
    }

    public async ValueTask DisposeAsync()
    {
        Dispose();
        await Task.WhenAll(_stopped.Select(stopped => stopped.Task));
        // No owner-thread join, no shutdown callback from a compute thread.
    }
}
