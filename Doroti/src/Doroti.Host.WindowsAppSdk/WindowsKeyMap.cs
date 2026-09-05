using System.Text;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Maps Win32 virtual/scan keys to Flutter-compatible logical and USB HID identities.</summary>
internal static class WindowsKeyMap
{
    private const long HidPlane = 0x00070000;
    private const long WindowsFallbackPlane = 0x1600000000;

    internal static long Physical(long scanCode, long virtualKey)
    {
        if (virtualKey is >= 0x41 and <= 0x5a) return HidPlane + 0x04 + virtualKey - 0x41;
        if (virtualKey is >= 0x31 and <= 0x39) return HidPlane + 0x1e + virtualKey - 0x31;
        if (virtualKey == 0x30) return HidPlane + 0x27;
        if (virtualKey is >= 0x70 and <= 0x7b) return HidPlane + 0x3a + virtualKey - 0x70;
        if (virtualKey is >= 0x60 and <= 0x69)
            return virtualKey == 0x60 ? HidPlane + 0x62 : HidPlane + 0x59 + virtualKey - 0x61;

        var extended = (scanCode & 0x100) != 0;
        return virtualKey switch
        {
            0x08 => HidPlane + 0x2a,
            0x09 => HidPlane + 0x2b,
            0x0d => HidPlane + (extended ? 0x58 : 0x28),
            0x10 => HidPlane + ((scanCode & 0xff) == 0x36 ? 0xe5 : 0xe1),
            0x11 => HidPlane + (extended ? 0xe4 : 0xe0),
            0x12 => HidPlane + (extended ? 0xe6 : 0xe2),
            0x13 => HidPlane + 0x48,
            0x14 => HidPlane + 0x39,
            0x1b => HidPlane + 0x29,
            0x20 => HidPlane + 0x2c,
            0x21 => HidPlane + 0x4b,
            0x22 => HidPlane + 0x4e,
            0x23 => HidPlane + 0x4d,
            0x24 => HidPlane + 0x4a,
            0x25 => HidPlane + 0x50,
            0x26 => HidPlane + 0x52,
            0x27 => HidPlane + 0x4f,
            0x28 => HidPlane + 0x51,
            0x2c => HidPlane + 0x46,
            0x2d => HidPlane + 0x49,
            0x2e => HidPlane + 0x4c,
            0x5b => HidPlane + 0xe3,
            0x5c => HidPlane + 0xe7,
            0x6a => HidPlane + 0x55,
            0x6b => HidPlane + 0x57,
            0x6d => HidPlane + 0x56,
            0x6e => HidPlane + 0x63,
            0x6f => HidPlane + 0x54,
            0x90 => HidPlane + 0x53,
            0x91 => HidPlane + 0x47,
            0xba => HidPlane + 0x33,
            0xbb => HidPlane + 0x2e,
            0xbc => HidPlane + 0x36,
            0xbd => HidPlane + 0x2d,
            0xbe => HidPlane + 0x37,
            0xbf => HidPlane + 0x38,
            0xc0 => HidPlane + 0x35,
            0xdb => HidPlane + 0x2f,
            0xdc => HidPlane + 0x31,
            0xdd => HidPlane + 0x30,
            0xde => HidPlane + 0x34,
            _ => WindowsFallbackPlane | (scanCode & 0xffffffff),
        };
    }

    internal static long Logical(long scanCode, long virtualKey, string character)
    {
        var runes = character.EnumerateRunes().Take(2).ToArray();
        if (runes is [var rune] && !Rune.IsControl(rune)) return Rune.ToLowerInvariant(rune).Value;
        if (virtualKey is >= 0x41 and <= 0x5a) return 'a' + virtualKey - 0x41;
        if (virtualKey is >= 0x30 and <= 0x39) return virtualKey;
        if (virtualKey is >= 0x70 and <= 0x87) return 0x100000801 + virtualKey - 0x70;
        if (virtualKey is >= 0x60 and <= 0x69) return 8589935152L + virtualKey - 0x60;

        var extended = (scanCode & 0x100) != 0;
        return virtualKey switch
        {
            0x08 => 0x100000008,
            0x09 => 0x100000009,
            0x0d => extended ? 8589935117L : 0x10000000d,
            // Flutter's logical modifier order is Control, Shift, Alt, Meta;
            // Win32's VK_SHIFT/VK_CONTROL numeric order is the reverse.
            0x10 => (scanCode & 0xff) == 0x36 ? 0x200000103 : 0x200000102,
            0x11 => extended ? 0x200000101 : 0x200000100,
            0x12 => extended ? 0x200000105 : 0x200000104,
            0x14 => 0x100000104,
            0x1b => 0x10000001b,
            0x20 => 0x20,
            0x21 => 0x100000308,
            0x22 => 0x100000307,
            0x23 => 0x100000305,
            0x24 => 0x100000306,
            0x25 => 0x100000302,
            0x26 => 0x100000304,
            0x27 => 0x100000303,
            0x28 => 0x100000301,
            0x2d => 0x100000407,
            0x2e => 0x10000007f,
            0x5b => 0x200000106,
            0x5c => 0x200000107,
            0x6a => 8589935146L,
            0x6b => 8589935147L,
            0x6d => 8589935149L,
            0x6e => 8589935150L,
            0x6f => 8589935151L,
            _ => WindowsFallbackPlane | (virtualKey & 0xffffffff),
        };
    }
}
