// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/error_dumper.dart and _error_dumper_io.dart
namespace Doroti.Generated.Framework.Foundation;

public static class ErrorToConsoleDumper
{
    public static void dump(string message) => PrintLibrary.debugPrint(message);

    public static void addWebDumpListener(Action<string> listener)
    {
        ArgumentNullException.ThrowIfNull(listener);
        // The pinned dart:io implementation intentionally has no web listener transport.
    }

    public static void clearWebDumpListeners()
    {
        // The pinned dart:io implementation intentionally has no web listener transport.
    }
}
