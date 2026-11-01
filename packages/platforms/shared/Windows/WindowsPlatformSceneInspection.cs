using Doroti.Ui;

namespace Doroti.Host.SharedWindows;

// Both Windows hosts inspect the retained tree on every raster frame. Indexing
// avoids a closure and interface enumerator for every retained layer, and still
// reads the current tree when native views or input shields are added/removed.
internal static class WindowsPlatformSceneInspection
{
    internal static bool HasPlatformCommands(IReadOnlyList<SceneCommand> commands, int depth = 0)
    {
        if (depth > 256)
            throw new InvalidDataException("PlatformView retained depth limit exceeded.");

        for (var i = 0; i < commands.Count; i++)
        {
            var command = commands[i];
            if (command.Operation is "platformView" or "inputShield"
                || command.RetainedCommands is { } retained && HasPlatformCommands(retained, depth + 1))
                return true;
        }
        return false;
    }
}
