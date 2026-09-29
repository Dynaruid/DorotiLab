// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/browser_context_menu.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public class BrowserContextMenu
{
    private static readonly Doroti.Ui.DispatcherLocal<BrowserContextMenu> _contextLocal_instance = new(() => new BrowserContextMenu());
    internal static BrowserContextMenu _instance { get => _contextLocal_instance.Value; set => _contextLocal_instance.Value = value; }
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
