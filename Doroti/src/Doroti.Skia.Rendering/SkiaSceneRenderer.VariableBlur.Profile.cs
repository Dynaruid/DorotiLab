using System.Diagnostics;

namespace Doroti.Skia.Rendering;

public sealed record VariableBlurStageProfile(
    string Stage,
    long Calls,
    double TotalMilliseconds,
    double MaximumMilliseconds
);

public sealed partial class SkiaSceneRenderer
{
    private readonly bool _profileVariableBlur =
        Environment.GetEnvironmentVariable("DOROTI_VARIABLE_BLUR_PROFILE") == "1";
    private readonly Dictionary<
        string,
        (long Calls, double Total, double Maximum)
    > _variableBlurStages = [];

    public IReadOnlyList<VariableBlurStageProfile> VariableBlurProfile
    {
        get
        {
            lock (_paintGate)
                return _variableBlurStages
                    .Select(p => new VariableBlurStageProfile(
                        p.Key,
                        p.Value.Calls,
                        p.Value.Total,
                        p.Value.Maximum
                    ))
                    .ToArray();
        }
    }

    private long StartVariableBlurStage() => _profileVariableBlur ? Stopwatch.GetTimestamp() : 0;

    private void EndVariableBlurStage(string stage, long started)
    {
        if (started == 0)
            return;
        var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
        var value = _variableBlurStages.GetValueOrDefault(stage);
        _variableBlurStages[stage] = (
            value.Calls + 1,
            value.Total + elapsed,
            Math.Max(value.Maximum, elapsed)
        );
    }
}
