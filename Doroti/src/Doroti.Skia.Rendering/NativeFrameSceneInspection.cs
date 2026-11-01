using Doroti.Ui;

namespace Doroti.Skia.Rendering;

// Admission is queried several times per frame. Traversal must not allocate a
// closure/enumerator for every retained layer or cache an outdated native view.
internal static class NativeFrameSceneInspection
{
    internal static bool IsShaderOnly(IReadOnlyList<SceneCommand> commands) => !HasNative(commands, 0);

    private static bool HasNative(IReadOnlyList<SceneCommand> commands, int depth)
    {
        if (depth > 256) return true;
        for (var i = 0; i < commands.Count; i++)
        {
            var command = commands[i];
            if (command.Operation is "platformView" or "inputShield"
                || command.RetainedCommands is { } retained && HasNative(retained, depth + 1))
                return true;
        }
        return false;
    }
}
