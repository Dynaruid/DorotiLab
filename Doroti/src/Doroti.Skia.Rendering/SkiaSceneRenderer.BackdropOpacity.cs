using Doroti.Ui;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    // Like Flutter's layer-state opacity inheritance, a single backdrop layer
    // can carry its ancestor's alpha on the final composite. An intermediate
    // transparent opacity saveLayer would hide the page from the filter.
    // Sibling paint and other isolating effects must keep their own saveLayer.
    internal static bool CanInheritBackdropOpacity(IReadOnlyList<SceneCommand> commands, int start, int end)
    {
        if (start >= end)
            return false;
        var command = commands[start];
        if (command.HostPayload is SceneRetainedPayload retained)
            return start + 1 == end
                && CanInheritBackdropOpacity(retained.Commands, 0, retained.Commands.Count);
        if (!IsSceneScopeStart(command.Operation) || FindMatchingPop(commands, start, end) != end - 1)
            return false;
        if (command.HostPayload is SceneBackdropFilterPayload backdrop)
            return backdrop.BlendMode == BlendMode.srcOver
                && !RequiresGpuFilterLayers(commands, start, end);
        return (command.Operation is "offset" or "transform" or "clipRect" or "clipRRect"
            or "clipRSuperellipse" or "clipPath" or "opacity")
            && CanInheritBackdropOpacity(commands, start + 1, end - 1);
    }
}
