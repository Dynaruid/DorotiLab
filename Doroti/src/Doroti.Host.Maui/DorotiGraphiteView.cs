#if IOS && !MACCATALYST
using SKGLView = Doroti.Host.Maui.DorotiSkiaView;
#endif
#if ANDROID || IOS || MACCATALYST
using SkiaSharp.Views.Maui.Controls;

namespace Doroti.Host.Maui;

// SKGLView remains the MAUI input/property contract only. Its Graphite handlers
// never construct a SkiaSharp GL/Ganesh platform view or publish a fake GRContext.
public sealed class DorotiGraphiteView : SKGLView
{
#if ANDROID
    internal AndroidPlatformViewHost? PlatformViews { get; set; }
#endif
#if IOS && !MACCATALYST
    internal UIKitPlatformViewHost? PlatformViews { get; set; }
#endif
    internal static bool Enabled =>
        Environment.GetEnvironmentVariable(
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

    // SKTouchDeviceType cannot represent invertedStylus or unknown. Preserve
    // native device identity across the Graphite surface boundary.
    internal event Action<MauiSurfacePointerData>? NativePointer;

    internal void DispatchNativePointer(MauiSurfacePointerData data) => NativePointer?.Invoke(data);

    internal void PaintGraphite(MauiSkiaPaintContext paint) => GraphitePaint?.Invoke(paint);

    internal void CompleteGraphite(MauiPaintCompletion completion, bool stale = false) =>
        GraphitePresentCompleted?.Invoke(completion, stale);

    internal void FailGraphite(MauiPaintCompletion? completion, Exception exception) =>
        GraphiteFailed?.Invoke(completion, exception);

    internal void ReleaseGraphiteResources() => GpuResourcesReleasing?.Invoke();
}
#endif
