// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/browser_context_menu.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public class BrowserContextMenu
{
    internal static BrowserContextMenu _instance = new BrowserContextMenu();
    internal virtual bool _enabled { get; set; } = true;
    internal virtual MethodChannel _channel { get; private set; } = SystemChannels.contextMenu;

    public BrowserContextMenu() { }

    public static bool enabled => _instance._enabled;

    public static Future disableContextMenu()
    {
        DartRuntimePrimitives.Assert(() => ConstantsLibrary.kIsWeb);
        return _instance
            ._channel.invokeMethod<object?>("disableContextMenu")
            .then(
                (_) =>
                {
                    _instance._enabled = false;
                }
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future enableContextMenu()
    {
        DartRuntimePrimitives.Assert(() => ConstantsLibrary.kIsWeb);
        return _instance
            ._channel.invokeMethod<object?>("enableContextMenu")
            .then(
                (_) =>
                {
                    _instance._enabled = true;
                }
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
