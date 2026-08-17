using System.Runtime.InteropServices;

namespace Doroti.Target.Android.Maui;

public static class AndroidMauiTarget
{
    public const string Rid = "android-arm64";
    public const string TargetFramework = "net10.0-android";
    public const string NativeViewType = "SkiaSharp.Views.Maui.Handlers.SKGLViewHandler+MauiSKGLTextureView";
    public const string GraphicsBackend = "Android/MauiSKGLTextureView/OpenGL-ES-Skia";

    public static void EnsureSupported()
    {
        if (!OperatingSystem.IsAndroid() || RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
            throw new PlatformNotSupportedException("Doroti.Target.Android.Maui.android-arm64 requires an Android arm64 process.");
    }
}
