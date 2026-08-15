#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/browser_context_menu.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public class BrowserContextMenu
{
    internal static BrowserContextMenu _instance = new BrowserContextMenu();
    internal virtual bool _enabled { get; set; } = true;
    internal virtual MethodChannel _channel { get; private set; } = SystemChannels.contextMenu;

    public BrowserContextMenu()
    {
    }

    public static bool enabled => _instance._enabled;
    public static Future disableContextMenu()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb);
        return _instance._channel.invokeMethod<object?>("disableContextMenu").then(((_) =>
        {
            _instance._enabled = false;
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future enableContextMenu()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb);
        return _instance._channel.invokeMethod<object?>("enableContextMenu").then(((_) =>
        {
            _instance._enabled = true;
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

