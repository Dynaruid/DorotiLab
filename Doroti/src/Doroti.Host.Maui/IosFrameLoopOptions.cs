namespace Doroti.Host.Maui;

internal readonly record struct IosFrameLoopOptions(bool Pipeline, bool Asynchronous)
{
    internal static IosFrameLoopOptions Resolve(string? serial, string? pipeline, string? presentation)
    {
        // Explicit serial, pipeline-off or transaction settings retain the
        // comparison/recovery path. An unconfigured iOS host uses policy C.
        var enabled = serial != "1" && pipeline != "0" && presentation != "transaction";
        return new(enabled, enabled || presentation == "async");
    }
}
