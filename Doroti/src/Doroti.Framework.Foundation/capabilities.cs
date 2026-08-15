// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/capabilities.dart and _capabilities_io.dart
namespace Doroti.Generated.Framework.Foundation;

public static class CapabilitiesLibrary
{
    public static bool isCanvasKit => throw new NotSupportedException("isCanvasKit is not implemented for dart:io.");

    public static bool isSkwasm => throw new NotSupportedException("isSkwasm is not implemented for dart:io.");

    public static bool isSkiaWeb => isCanvasKit || isSkwasm;
}

internal static class _capabilities_ioLibrary
{
    internal static bool isCanvasKit => CapabilitiesLibrary.isCanvasKit;

    internal static bool isSkwasm => CapabilitiesLibrary.isSkwasm;
}
