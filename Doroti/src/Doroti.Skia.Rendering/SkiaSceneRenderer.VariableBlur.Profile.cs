using System.Diagnostics;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed record VariableBlurStageProfile(
    string Stage,
    long Calls,
    double TotalMilliseconds,
    double MaximumMilliseconds
)
{
    // Percentiles describe the last SampleCount CPU recording calls, not GPU time.
    public int SampleCount { get; init; }
    public double P50Milliseconds { get; init; }
    public double P95Milliseconds { get; init; }
    public double P99Milliseconds { get; init; }
}

public sealed record VariableBlurWorkProfile(
    string Stage,
    int InputWidth,
    int InputHeight,
    int OutputWidth,
    int OutputHeight,
    float Left,
    float Top,
    float Right,
    float Bottom,
    string Detail,
    long EstimatedRgbaBytes
);

public sealed record VariableBlurDiagnostics(
    string Backend,
    long ContextGeneration,
    long RecordingFrame,
    IReadOnlyList<VariableBlurStageProfile> CpuStages,
    IReadOnlyList<VariableBlurWorkProfile> LastFrameWork,
    int OmittedWorkItems
)
{
    public IReadOnlyList<VariableBlurCaptureProfile> CaptureDecisions { get; init; } = [];
}

public sealed record VariableBlurCaptureProfile(
    string Reason,
    long Calls,
    long CapturedPixels,
    long FullDomainPixels
);

public sealed partial class SkiaSceneRenderer
{
    private readonly bool _profileVariableBlur =
        Environment.GetEnvironmentVariable("DOROTI_VARIABLE_BLUR_PROFILE") == "1";
    private readonly Dictionary<
        string,
        (long Calls, double Total, double Maximum)
    > _variableBlurStages = [];
    private readonly Dictionary<string, Queue<double>> _variableBlurSamples = [];
    private readonly List<VariableBlurWorkProfile> _variableBlurWork = [];
    private int _variableBlurOmittedWork;
    private readonly Dictionary<
        string,
        (long Calls, long Pixels, long FullPixels)
    > _variableBlurCaptures = [];

    public VariableBlurDiagnostics? CaptureVariableBlurDiagnostics()
    {
        if (!_profileVariableBlur)
            return null;
        lock (_paintGate)
            return new(
                RuntimeEffectBackend,
                _contextGeneration,
                _rasterFrame,
                VariableBlurProfile,
                _variableBlurWork.ToArray(),
                _variableBlurOmittedWork
            )
            {
                CaptureDecisions = _variableBlurCaptures
                    .Select(p => new VariableBlurCaptureProfile(
                        p.Key,
                        p.Value.Calls,
                        p.Value.Pixels,
                        p.Value.FullPixels
                    ))
                    .ToArray(),
            };
    }

    public void ResetVariableBlurProfile()
    {
        lock (_paintGate)
        {
            _variableBlurStages.Clear();
            _variableBlurSamples.Clear();
            _variableBlurCaptures.Clear();
            BeginVariableBlurProfileFrame();
        }
    }

    private void BeginVariableBlurProfileFrame()
    {
        _variableBlurWork.Clear();
        _variableBlurOmittedWork = 0;
    }

    private void RecordVariableBlurWork(
        string stage,
        int inputWidth,
        int inputHeight,
        int outputWidth,
        int outputHeight,
        SKRect bounds,
        string detail = ""
    )
    {
        if (!_profileVariableBlur)
            return;
        if (stage == "backdrop-capture")
        {
            var previous = _variableBlurCaptures.GetValueOrDefault(detail);
            _variableBlurCaptures[detail] = (
                previous.Calls + 1,
                previous.Pixels + (long)outputWidth * outputHeight,
                previous.FullPixels + (long)inputWidth * inputHeight
            );
        }
        if (_variableBlurWork.Count == 128)
        {
            _variableBlurOmittedWork++;
            return;
        }
        _variableBlurWork.Add(
            new(
                stage,
                inputWidth,
                inputHeight,
                outputWidth,
                outputHeight,
                bounds.Left,
                bounds.Top,
                bounds.Right,
                bounds.Bottom,
                detail,
                4L * outputWidth * outputHeight
            )
        );
    }

    public IReadOnlyList<VariableBlurStageProfile> VariableBlurProfile
    {
        get
        {
            lock (_paintGate)
                return _variableBlurStages
                    .Select(p =>
                    {
                        var samples = _variableBlurSamples[p.Key].Order().ToArray();
                        double Percentile(double q) =>
                            samples[(int)Math.Ceiling(q * samples.Length) - 1];
                        return new VariableBlurStageProfile(
                            p.Key,
                            p.Value.Calls,
                            p.Value.Total,
                            p.Value.Maximum
                        )
                        {
                            SampleCount = samples.Length,
                            P50Milliseconds = Percentile(.5),
                            P95Milliseconds = Percentile(.95),
                            P99Milliseconds = Percentile(.99),
                        };
                    })
                    .ToArray();
        }
    }

    private long StartVariableBlurStage() => _profileVariableBlur ? Stopwatch.GetTimestamp() : 0;

    private void EndVariableBlurStage(string stage, long started)
    {
        if (started == 0)
            return;
        var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
        if (!_variableBlurSamples.TryGetValue(stage, out var samples))
            _variableBlurSamples.Add(stage, samples = new Queue<double>());
        if (samples.Count == 4096)
            samples.Dequeue();
        samples.Enqueue(elapsed);
        var value = _variableBlurStages.GetValueOrDefault(stage);
        _variableBlurStages[stage] = (
            value.Calls + 1,
            value.Total + elapsed,
            Math.Max(value.Maximum, elapsed)
        );
    }
}
