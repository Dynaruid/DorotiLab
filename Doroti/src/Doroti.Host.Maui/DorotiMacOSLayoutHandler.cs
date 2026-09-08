#if MACOS
using Microsoft.Maui.Handlers;

namespace Doroti.Host.Maui;

/// <summary>
/// Connects MAUI's dynamic layout commands to the AppKit layout implementation.
/// The preview backend exposes these methods but does not map the commands.
/// </summary>
public sealed class DorotiMacOSLayoutHandler : Microsoft.Maui.Platforms.MacOS.Handlers.LayoutHandler
{
    public override void Invoke(string command, object? args)
    {
        if (command == nameof(ILayoutHandler.Clear))
        {
            Clear();
            return;
        }
        if (args is LayoutHandlerUpdate update)
        {
            switch (command)
            {
                case nameof(ILayoutHandler.Add): Add(update.View); return;
                case nameof(ILayoutHandler.Remove): Remove(update.View); return;
                case nameof(ILayoutHandler.Insert): Insert(update.Index, update.View); return;
                case nameof(ILayoutHandler.Update): Update(update.Index, update.View); return;
            }
        }
        base.Invoke(command, args);
    }
}
#endif
