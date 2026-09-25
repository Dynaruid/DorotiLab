namespace Doroti.Host.WindowsAppSdk;

/// <summary>Uses the Windows mapping shared with the MAUI native input paths.</summary>
internal static class WindowsKeyMap
{
    internal static long Physical(long scanCode, long virtualKey) =>
        Hosting.WindowsKeyboardMap.Physical(scanCode, virtualKey);

    internal static long Logical(long scanCode, long virtualKey, string character) =>
        Hosting.WindowsKeyboardMap.Logical(scanCode, virtualKey, character);
}