using System.Runtime.InteropServices;

namespace Doroti.Target.Android.Maui;

public static class AndroidMauiTarget
{
    public const string Rid = "android-x64";
    public const string TargetFramework = "net10.0-android";
    public const string NativeViewType = "SkiaSharp.Views.Maui.Handlers.SKGLViewHandler+MauiSKGLTextureView";
    public const string GraphicsBackend = "Android/MauiSKGLTextureView/OpenGL-ES-Skia";

    public static void EnsureSupported()
    {
        if (!OperatingSystem.IsAndroid() || RuntimeInformation.ProcessArchitecture != Architecture.X64)
            throw new PlatformNotSupportedException("Doroti.Target.Android.Maui.android-x64 requires an Android x64 process.");
    }
}
