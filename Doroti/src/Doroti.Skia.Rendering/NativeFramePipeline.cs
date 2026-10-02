namespace Doroti.Skia.Rendering;

/// <summary>Immutable metrics published before framework preparation and GPU admission.</summary>
public sealed record SkiaFramePreparation(object? ContextIdentity, int PixelWidth, int PixelHeight,
    double Density, long SurfaceGeneration, string NativeViewType, string GraphicsBackend,
    TimeSpan Timestamp, Action<string, double>? CpuStageMeasured = null);

/// <summary>C is the only native frame policy. Reject obsolete selectors before creating GPU resources.</summary>
public static class NativeFrameConfiguration
{
    public const string Mode = "C";

    private static readonly string[] RemovedSelectors =
    [
        "DOROTI_VARIABLE_BLUR_SERIAL_FRAMES", "DOROTI_VARIABLE_BLUR_PIPELINE",
        "DOROTI_NATIVE_PRESENTATION", "DOROTI_IOS_SHADER_PRESENTATION",
    ];

    /// <summary>Also validates Android Intent extras; no setting can select a different frame policy.</summary>
    public static void ValidateSettings(Func<string, string?> read)
    {
        ArgumentNullException.ThrowIfNull(read);
        var mode = read("DOROTI_NATIVE_FRAME_MODE");
        if (mode is not (null or "" or Mode))
            throw new ArgumentException("Native frame modes A/B were removed. Unset DOROTI_NATIVE_FRAME_MODE or set it to C.");
        foreach (var selector in RemovedSelectors)
        {
            if (!string.IsNullOrEmpty(read(selector)))
                throw new ArgumentException($"{selector} was removed. Delete this setting; native hosts always use frame policy C.");
        }
    }

    public static void ValidateEnvironment() => ValidateSettings(Environment.GetEnvironmentVariable);
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

    public static NativeFrameAdmission Decide(SkiaShaderSceneAdmission scene, int pending, bool native, bool pendingNative,
        bool resizing, bool supportsTwoFrames = true)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(pending);
        var fresh = supportsTwoFrames && !resizing && !native && !pendingNative
            && scene == SkiaShaderSceneAdmission.eligible;
        var limit = fresh ? ShaderFrameLimit : 1;
        var reason = resizing ? "resize" : native || pendingNative ? "native-active"
            : !supportsTwoFrames ? "serial-backend" : scene.ToString();
        return new(pending < limit, fresh, limit, !fresh,
            pending >= limit ? "slots-full/" + reason : reason);
    }

    public static NativeFrameAdmission PrepareAndDecide(Action prepare, Func<SkiaShaderSceneAdmission> query, Func<int> pendingCount,
        bool native, bool pendingNative, bool resizing, bool prepareFramework = true,
        bool supportsTwoFrames = true)
    {
        if (!resizing && prepareFramework) prepare();
        // Completion and native insertion can occur during preparation. Query the
        // actual retained scene and slot count afterwards, never a cached admission.
        var pending = pendingCount();
        return Decide(query(), pending, native, pendingNative,
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
