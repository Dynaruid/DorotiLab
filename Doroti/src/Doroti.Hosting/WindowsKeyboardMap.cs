using System.Text;

namespace Doroti.Hosting;

/// <summary>Maps Win32 virtual/scan keys to Flutter-compatible logical and USB HID identities.</summary>
internal static class WindowsKeyboardMap
{
    private const long HidPlane = 0x00070000;
    private const long WindowsFallbackPlane = 0x1600000000;

    internal static long Physical(long scanCode, long virtualKey)
    {
        // The native packet retains lParam bits 16..24: Scan 1 plus the
        // extended-key bit. Physical identity must survive layout/NumLock changes.
        if (scanCode != 0)
        {
            var usage = scanCode switch
            {
                0x1e => 0x04, 0x30 => 0x05, 0x2e => 0x06, 0x20 => 0x07,
                0x12 => 0x08, 0x21 => 0x09, 0x22 => 0x0a, 0x23 => 0x0b,
                0x17 => 0x0c, 0x24 => 0x0d, 0x25 => 0x0e, 0x26 => 0x0f,
                0x32 => 0x10, 0x31 => 0x11, 0x18 => 0x12, 0x19 => 0x13,
                0x10 => 0x14, 0x13 => 0x15, 0x1f => 0x16, 0x14 => 0x17,
                0x16 => 0x18, 0x2f => 0x19, 0x11 => 0x1a, 0x2d => 0x1b,
                0x15 => 0x1c, 0x2c => 0x1d,
                >= 0x02 and <= 0x0a => 0x1e + (int)scanCode - 0x02,
                0x0b => 0x27, 0x1c => 0x28, 0x01 => 0x29, 0x0e => 0x2a,
                0x0f => 0x2b, 0x39 => 0x2c, 0x0c => 0x2d, 0x0d => 0x2e,
                0x1a => 0x2f, 0x1b => 0x30, 0x2b => 0x31, 0x27 => 0x33,
                0x28 => 0x34, 0x29 => 0x35, 0x33 => 0x36, 0x34 => 0x37,
                0x35 => 0x38, 0x3a => 0x39,
                >= 0x3b and <= 0x44 => 0x3a + (int)scanCode - 0x3b,
                0x57 => 0x44, 0x58 => 0x45, 0x137 => 0x46, 0x46 => 0x47,
                // Pause and NumLock share the base scan code in Win32 messages.
                0x45 => virtualKey == 0x90 ? 0x53 : 0x48,
                0x145 => 0x53,
                0x152 => 0x49, 0x147 => 0x4a, 0x149 => 0x4b, 0x153 => 0x4c,
                0x14f => 0x4d, 0x151 => 0x4e, 0x14d => 0x4f, 0x14b => 0x50,
                0x150 => 0x51, 0x148 => 0x52, 0x135 => 0x54, 0x37 => 0x55,
                0x4a => 0x56, 0x4e => 0x57, 0x11c => 0x58,
                0x4f => 0x59, 0x50 => 0x5a, 0x51 => 0x5b, 0x4b => 0x5c,
                0x4c => 0x5d, 0x4d => 0x5e, 0x47 => 0x5f, 0x48 => 0x60,
                0x49 => 0x61, 0x52 => 0x62, 0x53 => 0x63, 0x56 => 0x64,
                0x15d => 0x65,
                >= 0x64 and <= 0x6e => 0x68 + (int)scanCode - 0x64,
                0x76 => 0x73,
                0x1d => 0xe0, 0x2a => 0xe1, 0x38 => 0xe2, 0x15b => 0xe3,
                0x11d => 0xe4, 0x36 => 0xe5, 0x138 => 0xe6, 0x15c => 0xe7,
                _ => 0,
            };
            return usage == 0 ? WindowsFallbackPlane | (scanCode & 0xffffffff)
                : HidPlane + usage;
        }

        // Synthetic input without a scan code has only its virtual identity.
        if (virtualKey is >= 0x41 and <= 0x5a)
        {
            return HidPlane + 0x04 + virtualKey - 0x41;
        }

        if (virtualKey is >= 0x31 and <= 0x39)
        {
            return HidPlane + 0x1e + virtualKey - 0x31;
        }

        if (virtualKey == 0x30)
        {
            return HidPlane + 0x27;
        }

        if (virtualKey is >= 0x70 and <= 0x87)
        {
            return virtualKey <= 0x7b ? HidPlane + 0x3a + virtualKey - 0x70
                : HidPlane + 0x68 + virtualKey - 0x7c;
        }

        if (virtualKey is >= 0x60 and <= 0x69)
        {
            return virtualKey == 0x60 ? HidPlane + 0x62 : HidPlane + 0x59 + virtualKey - 0x61;
        }

        var extended = (scanCode & 0x100) != 0;
        return virtualKey switch
        {
            0x08 => HidPlane + 0x2a,
            0x09 => HidPlane + 0x2b,
            0x0d => HidPlane + (extended ? 0x58 : 0x28),
            0x10 => HidPlane + ((scanCode & 0xff) == 0x36 ? 0xe5 : 0xe1),
            0x11 => HidPlane + (extended ? 0xe4 : 0xe0),
            0x12 => HidPlane + (extended ? 0xe6 : 0xe2),
            0xa0 => HidPlane + 0xe1,
            0xa1 => HidPlane + 0xe5,
            0xa2 => HidPlane + 0xe0,
            0xa3 => HidPlane + 0xe4,
            0xa4 => HidPlane + 0xe2,
            0xa5 => HidPlane + 0xe6,
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
            _ => WindowsFallbackPlane | 0x100000000 | (virtualKey & 0xffffffff),
        };
    }

    internal static long Logical(long scanCode, long virtualKey, string character)
    {
        // Printable numpad characters still have dedicated logical identities.
        if (virtualKey is >= 0x60 and <= 0x69)
        {
            return 8589935152L + virtualKey - 0x60;
        }
        var numpad = virtualKey switch
        {
            0x6a => 8589935146L,
            0x6b => 8589935147L,
            0x6d => 8589935149L,
            0x6e => 8589935150L,
            0x6f => 8589935151L,
            _ => 0L,
        };
        if (numpad != 0) return numpad;

        var runes = character.EnumerateRunes().Take(2).ToArray();
        if (runes is [var rune] && !Rune.IsControl(rune))
        {
            return Rune.ToLowerInvariant(rune).Value;
        }

        if (virtualKey is >= 0x41 and <= 0x5a)
        {
            return 'a' + virtualKey - 0x41;
        }

        if (virtualKey is >= 0x30 and <= 0x39)
        {
            return virtualKey;
        }

        if (virtualKey is >= 0x70 and <= 0x87)
        {
            return 0x100000801 + virtualKey - 0x70;
        }

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
            0xa0 => 0x200000102,
            0xa1 => 0x200000103,
            0xa2 => 0x200000100,
            0xa3 => 0x200000101,
            0xa4 => 0x200000104,
            0xa5 => 0x200000105,
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
            _ => WindowsFallbackPlane | (virtualKey & 0xffffffff),
        };
    }
}
