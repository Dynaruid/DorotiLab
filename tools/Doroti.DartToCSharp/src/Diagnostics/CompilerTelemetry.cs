using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed record CompilerTelemetryPhase(
    string Name,
    string? Input,
    long ElapsedMilliseconds,
    long AllocatedBytes,
    bool Success,
    string? Failure);

internal sealed record CompilerTelemetryDocument(
    string SchemaVersion,
    string Status,
    string Manifest,
    string? CompilerIdentity,
    DateTimeOffset StartedUtc,
    long ElapsedMilliseconds,
    long AccountedElapsedMilliseconds,
    double AccountedRatio,
    int InputCount,
    int AnalyzerWorkers,
    int LoweringParallelism,
    int DartProcessCount,
    int AnalysisContextCount,
    int CacheHits,
    int CacheMisses,
    long CacheReadBytes,
    long CacheWriteBytes,
    long InputBytes,
    long OutputBytes,
    long PeakWorkingSetBytes,
    long ManagedAllocatedBytes,
    int MaximumWorkerQueueDepth,
    string? LastCompletedPhase,
    bool PartialArtifactPublished,
    CompilerTelemetryPhase[] InvocationPhases,
    CompilerTelemetryPhase[] LibraryPhases,
    string? Failure);

/// <summary>
/// Thread-safe, invocation-owned performance telemetry. Top-level invocation phases are
/// deliberately non-overlapping; per-library phases may overlap and are reported separately.
/// </summary>
internal sealed class CompilerProfiler : IDisposable
{
    private readonly ConcurrentQueue<CompilerTelemetryPhase> _invocationPhases = new();
    private readonly ConcurrentQueue<CompilerTelemetryPhase> _libraryPhases = new();
    private readonly Stopwatch _elapsed = Stopwatch.StartNew();
    private readonly CancellationTokenSource _samplingCancellation = new();
    private readonly Task _samplingTask;
    private readonly DateTimeOffset _startedUtc = DateTimeOffset.UtcNow;
    private readonly string _manifest;
    private readonly string? _telemetryPath;
    private long _peakWorkingSetBytes;
    private long _cacheReadBytes;
    private long _cacheWriteBytes;
    private long _inputBytes;
    private long _outputBytes;
    private int _dartProcessCount;
    private int _analysisContextCount;
    private int _cacheHits;
    private int _cacheMisses;
    private int _workerQueueDepth;
    private int _maximumWorkerQueueDepth;
    private int _disposed;
    private string? _compilerIdentity;
    private string? _lastCompletedPhase;
    private string? _failure;
    private string _status = "running";
    private bool _partialArtifactPublished;

    public CompilerProfiler(string manifest, string? telemetryPath, int analyzerWorkers, int loweringParallelism)
    {
        _manifest = ArtifactFiles.NormalizePath(Path.GetFullPath(manifest));
        _telemetryPath = telemetryPath is null ? null : Path.GetFullPath(telemetryPath);
        AnalyzerWorkers = analyzerWorkers;
        LoweringParallelism = loweringParallelism;
        _peakWorkingSetBytes = Environment.WorkingSet;
        _samplingTask = Task.Run(SampleWorkingSetAsync);
    }

    public int AnalyzerWorkers { get; }
    public int LoweringParallelism { get; }
    public int InputCount { get; set; }

    public IDisposable MeasureInvocation(string name) => new PhaseScope(this, name, null, invocation: true);

    public IDisposable MeasureLibrary(string name, string input) => new PhaseScope(this, name, input, invocation: false);

    public void SetCompilerIdentity(string identity) => _compilerIdentity = identity;

    public void AddInputBytes(long value) => Interlocked.Add(ref _inputBytes, value);
    public void AddOutputBytes(long value) => Interlocked.Add(ref _outputBytes, value);
    public void RecordDartProcess() => Interlocked.Increment(ref _dartProcessCount);
    public void RecordAnalysisContext(int count = 1) => Interlocked.Add(ref _analysisContextCount, count);

    public void RecordCacheHit(long bytes)
    {
        Interlocked.Increment(ref _cacheHits);
        Interlocked.Add(ref _cacheReadBytes, bytes);
    }

    public void RecordCacheMiss() => Interlocked.Increment(ref _cacheMisses);
    public void RecordCacheWrite(long bytes) => Interlocked.Add(ref _cacheWriteBytes, bytes);

    public IDisposable EnterWorkerQueue()
    {
        var depth = Interlocked.Increment(ref _workerQueueDepth);
        while (true)
        {
            var maximum = Volatile.Read(ref _maximumWorkerQueueDepth);
            if (depth <= maximum || Interlocked.CompareExchange(ref _maximumWorkerQueueDepth, depth, maximum) == maximum)
            {
                break;
            }
        }
        return new CallbackScope(() => Interlocked.Decrement(ref _workerQueueDepth));
    }

    public void Complete(bool partialArtifactPublished = false)
    {
        _partialArtifactPublished = partialArtifactPublished;
        _status = "success";
    }

    public void Fail(Exception exception, bool partialArtifactPublished = false)
    {
        _partialArtifactPublished = partialArtifactPublished;
        _status = exception is OperationCanceledException ? "cancelled" : "failed";
        _failure = $"{exception.GetType().Name}: {exception.Message}";
    }

    public CompilerTelemetryDocument Snapshot()
    {
        UpdatePeakWorkingSet();
        var invocationPhases = _invocationPhases.ToArray();
        var accounted = invocationPhases.Sum(item => item.ElapsedMilliseconds);
        var elapsed = Math.Max(1, _elapsed.ElapsedMilliseconds);
        return new(
            "doroti.dart-to-csharp-telemetry/v1",
            _status,
            _manifest,
            _compilerIdentity,
            _startedUtc,
            elapsed,
            accounted,
            Math.Round((double)accounted / elapsed, 4),
            InputCount,
            AnalyzerWorkers,
            LoweringParallelism,
            Volatile.Read(ref _dartProcessCount),
            Volatile.Read(ref _analysisContextCount),
            Volatile.Read(ref _cacheHits),
            Volatile.Read(ref _cacheMisses),
            Volatile.Read(ref _cacheReadBytes),
            Volatile.Read(ref _cacheWriteBytes),
            Volatile.Read(ref _inputBytes),
            Volatile.Read(ref _outputBytes),
            Volatile.Read(ref _peakWorkingSetBytes),
            GC.GetTotalAllocatedBytes(precise: false),
            Volatile.Read(ref _maximumWorkerQueueDepth),
            _lastCompletedPhase,
            _partialArtifactPublished,
            invocationPhases,
            _libraryPhases.ToArray(),
            _failure);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        _elapsed.Stop();
        _samplingCancellation.Cancel();
        try
        {
            _samplingTask.GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
        }
        _samplingCancellation.Dispose();
        if (_telemetryPath is not null)
        {
            ArtifactFiles.WriteJson(_telemetryPath, Snapshot());
        }
    }

    private async Task SampleWorkingSetAsync()
    {
        while (!_samplingCancellation.IsCancellationRequested)
        {
            UpdatePeakWorkingSet();
            await Task.Delay(25, _samplingCancellation.Token).ConfigureAwait(false);
        }
    }

    private void UpdatePeakWorkingSet()
    {
        var workingSet = Environment.WorkingSet;
        while (true)
        {
            var peak = Volatile.Read(ref _peakWorkingSetBytes);
            if (workingSet <= peak || Interlocked.CompareExchange(ref _peakWorkingSetBytes, workingSet, peak) == peak)
            {
                return;
            }
        }
    }

    private void FinishPhase(string name, string? input, bool invocation, long elapsedMilliseconds, long allocatedBytes, Exception? failure)
    {
        var phase = new CompilerTelemetryPhase(
            name,
            input,
            elapsedMilliseconds,
            allocatedBytes,
            failure is null,
            failure is null ? null : $"{failure.GetType().Name}: {failure.Message}");
        if (invocation)
        {
            _invocationPhases.Enqueue(phase);
            if (failure is null) _lastCompletedPhase = name;
        }
        else
        {
            _libraryPhases.Enqueue(phase);
        }
    }

    private sealed class PhaseScope : IDisposable
    {
        private readonly CompilerProfiler _owner;
        private readonly string _name;
        private readonly string? _input;
        private readonly bool _invocation;
        private readonly Stopwatch _elapsed = Stopwatch.StartNew();
        private readonly long _allocated = GC.GetAllocatedBytesForCurrentThread();
        private Exception? _failure;
        private int _disposed;

        public PhaseScope(CompilerProfiler owner, string name, string? input, bool invocation)
        {
            _owner = owner;
            _name = name;
            _input = input;
            _invocation = invocation;
        }

        public void Fail(Exception exception) => _failure = exception;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
            _elapsed.Stop();
            _owner.FinishPhase(
                _name,
                _input,
                _invocation,
                _elapsed.ElapsedMilliseconds,
                Math.Max(0, GC.GetAllocatedBytesForCurrentThread() - _allocated),
                _failure);
        }
    }

    private sealed class CallbackScope(Action callback) : IDisposable
    {
        private int _disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0) callback();
        }
    }
}
