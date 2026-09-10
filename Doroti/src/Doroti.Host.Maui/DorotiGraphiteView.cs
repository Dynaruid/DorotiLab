#if ANDROID || IOS || MACCATALYST
using SkiaSharp.Views.Maui.Controls;

namespace Doroti.Host.Maui;

// SKGLView remains the MAUI input/property contract only. Its Graphite handlers
// never construct a SkiaSharp GL/Ganesh platform view or publish a fake GRContext.
public sealed class DorotiGraphiteView : SKGLView
{
    internal static bool Enabled => Environment.GetEnvironmentVariable(
#if ANDROID
        "DOROTI_ANDROID_GRAPHITE"
#else
        "DOROTI_IOS_GRAPHITE"
#endif
        ) != "0";
    internal event Action<MauiSkiaPaintContext>? GraphitePaint;
    internal event Action<MauiPaintCompletion, bool>? GraphitePresentCompleted;
    internal event Action<MauiPaintCompletion?, Exception>? GraphiteFailed;
    internal event Action? GpuResourcesReleasing;
    internal void PaintGraphite(MauiSkiaPaintContext paint) => GraphitePaint?.Invoke(paint);
    internal void CompleteGraphite(MauiPaintCompletion completion, bool stale = false) => GraphitePresentCompleted?.Invoke(completion, stale);
    internal void FailGraphite(MauiPaintCompletion? completion, Exception exception) => GraphiteFailed?.Invoke(completion, exception);
    internal void ReleaseGraphiteResources() => GpuResourcesReleasing?.Invoke();
}
#endif
