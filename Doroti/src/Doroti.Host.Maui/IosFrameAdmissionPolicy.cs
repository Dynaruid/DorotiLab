using Doroti.Skia.Rendering;

namespace Doroti.Host.Maui;

// Kept independent of UIKit so the same host decision runs in CPU regressions.
internal readonly record struct IosFrameAdmission(bool Admitted, bool FreshOnly, bool Transaction, string Reason);

internal static class IosFrameAdmissionPolicy
{
    internal static IosFrameAdmission PrepareAndDecide(
        bool pipeline, bool asynchronous, Action prepare, Func<SkiaShaderSceneAdmission> query,
        Func<int> pendingCount, bool native, bool pendingNative, bool resizing, bool prepareFramework = true)
    {
        var value = NativeFrameAdmissionPolicy.PrepareAndDecide(pipeline, asynchronous,
            prepare, query, pendingCount, native, pendingNative, resizing, prepareFramework);
        return new(value.Admitted, value.FreshOnly, value.SynchronizePresentation, value.Reason);
    }
}
