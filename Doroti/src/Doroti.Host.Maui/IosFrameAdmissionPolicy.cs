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
        // Preparation precedes both the scene query and the slot check. A full
        // queue still allows the framework to replace its one pending scene.
        if (pipeline && !resizing && prepareFramework) prepare();
        var pending = pendingCount();
        var scene = query();
        var fresh = pipeline && !resizing && !native && !pendingNative
            && scene == SkiaShaderSceneAdmission.eligible;
        var limit = fresh ? 2 : 1;
        var reason = resizing ? "resize" : native || pendingNative ? "native-active"
            : scene.ToString();
        return new(pending < limit, fresh,
            !asynchronous || resizing || native || pendingNative
                || scene != SkiaShaderSceneAdmission.eligible,
            pending >= limit ? "slots-full/" + reason : reason);
    }
}
