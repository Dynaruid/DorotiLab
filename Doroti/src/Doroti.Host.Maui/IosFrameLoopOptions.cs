namespace Doroti.Host.Maui;

internal readonly record struct IosFrameLoopOptions(bool Pipeline, bool Asynchronous)
{
    internal static IosFrameLoopOptions Resolve(string? serial, string? pipeline, string? presentation)
    {
        var value = Doroti.Skia.Rendering.NativeFrameLoopOptions.Resolve(serial, pipeline, presentation);
        return new(value.Pipeline, value.Asynchronous);
    }
}
