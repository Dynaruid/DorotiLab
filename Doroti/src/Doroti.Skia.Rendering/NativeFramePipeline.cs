namespace Doroti.Skia.Rendering;

/// <summary>Immutable metrics published before framework preparation and GPU admission.</summary>
public sealed record SkiaFramePreparation(object? ContextIdentity, int PixelWidth, int PixelHeight,
    double Density, long SurfaceGeneration, string NativeViewType, string GraphicsBackend,
    TimeSpan Timestamp, Action<string, double>? CpuStageMeasured = null);

public readonly record struct NativeFrameLoopOptions(bool Pipeline, bool Asynchronous)
{
    public string Mode => Pipeline ? "C" : Asynchronous ? "B" : "A";

    public static NativeFrameLoopOptions Resolve(string? serial, string? pipeline, string? presentation)
    {
        var enabled = serial != "1" && pipeline != "0" && presentation != "transaction";
        return new(enabled, enabled || presentation == "async");
    }

    /// <summary>Explicit common mode overrides legacy selectors. The delegate also supports Android intent extras.</summary>
    public static NativeFrameLoopOptions FromSettings(Func<string, string?> read, string? presentationAlias = null)
    {
        var mode = read("DOROTI_NATIVE_FRAME_MODE");
        return mode switch
        {
            "A" => new(false, false),
            "B" => new(false, true),
            "C" => new(true, true),
            null or "" => Resolve(read("DOROTI_VARIABLE_BLUR_SERIAL_FRAMES"),
                read("DOROTI_VARIABLE_BLUR_PIPELINE"), read("DOROTI_NATIVE_PRESENTATION")
                    ?? (presentationAlias is null ? null : read(presentationAlias))),
            _ => throw new ArgumentException("DOROTI_NATIVE_FRAME_MODE must be A, B or C."),
        };
    }

    public static NativeFrameLoopOptions FromEnvironment(string? presentationAlias = null) =>
        FromSettings(Environment.GetEnvironmentVariable, presentationAlias);
}

public readonly record struct NativeFrameAdmission(bool Admitted, bool FreshOnly,
    int GpuLimit, bool SynchronizePresentation, string Reason);

public sealed record NativeFramePipelineSnapshot(string Mode, long PreparedPulses,
    int PendingGpuFrames, int MaximumGpuFrames, long CompletedGpuFrames,
    string Retirement, string? Fallback = null, long PreparedWhileGpuFull = 0, long RejectedAdmissions = 0);

/// <summary>Logical GPU frames, independent of front images, swapchain or shared texture bank counts.</summary>
public static class NativeFrameAdmissionPolicy
{
    public const int ShaderFrameLimit = 2;

    public static NativeFrameAdmission Decide(bool pipeline, bool asynchronous,
        SkiaShaderSceneAdmission scene, int pending, bool native, bool pendingNative,
        bool resizing, bool supportsTwoFrames = true)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(pending);
        var fresh = pipeline && supportsTwoFrames && !resizing && !native && !pendingNative
            && scene == SkiaShaderSceneAdmission.eligible;
        var limit = fresh ? ShaderFrameLimit : 1;
        var reason = resizing ? "resize" : native || pendingNative ? "native-active"
            : !supportsTwoFrames ? "serial-backend" : scene.ToString();
        return new(pending < limit, fresh, limit,
            !asynchronous || resizing || native || pendingNative
                || scene != SkiaShaderSceneAdmission.eligible || !supportsTwoFrames,
            pending >= limit ? "slots-full/" + reason : reason);
    }

    public static NativeFrameAdmission PrepareAndDecide(bool pipeline, bool asynchronous,
        Action prepare, Func<SkiaShaderSceneAdmission> query, Func<int> pendingCount,
        bool native, bool pendingNative, bool resizing, bool prepareFramework = true,
        bool supportsTwoFrames = true)
    {
        if (pipeline && !resizing && prepareFramework) prepare();
        // Completion and native insertion can occur during preparation. Query the
        // actual retained scene and slot count afterwards, never a cached admission.
        var pending = pendingCount();
        return Decide(pipeline, asynchronous, query(), pending, native, pendingNative,
            resizing, supportsTwoFrames);
    }
}

/// <summary>Submission/terminal receipt is distinct from the final GPU consumer's completion.</summary>
public class NativeFrameLifetime
{
    public bool TerminalCommitted { get; private set; }
    public bool Retired { get; private set; }
    public void MarkTerminalCommitted()
    {
        if (TerminalCommitted || Retired) throw new InvalidOperationException("Frame terminal already committed.");
        TerminalCommitted = true;
    }
    public bool CanRetire(bool completed, bool deviceLost) => !Retired && (completed || deviceLost);
    public void MarkRetired(bool completed, bool deviceLost)
    {
        if (!CanRetire(completed, deviceLost)) throw new InvalidOperationException("GPU completion is not established.");
        Retired = true;
    }
}
