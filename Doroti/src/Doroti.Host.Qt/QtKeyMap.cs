using System.Text;

namespace Doroti.Host.Qt;

/// <summary>Maps Qt key identities to Doroti/Flutter-compatible logical and USB HID planes.</summary>
internal static class QtKeyMap
{
    private const long HidPlane = 0x00070000;
    private const long QtFallbackPlane = 0x130000000;

    internal static long Physical(long nativeScanCode, long qtKey)
    {
        // Qt xcb and Wayland report XKB keycodes, which are Linux evdev codes
        // plus the historical XKB offset of eight. Zero denotes a synthetic
        // event; preserve the Qt-key fallback only for that case.
        if (nativeScanCode >= 8 && nativeScanCode <= 0xffff)
        {
            var evdev = (int)nativeScanCode - 8;
            var usage = evdev switch
            {
                30 => 0x04, 48 => 0x05, 46 => 0x06, 32 => 0x07, 18 => 0x08,
                33 => 0x09, 34 => 0x0a, 35 => 0x0b, 23 => 0x0c, 36 => 0x0d,
                37 => 0x0e, 38 => 0x0f, 50 => 0x10, 49 => 0x11, 24 => 0x12,
                25 => 0x13, 16 => 0x14, 19 => 0x15, 31 => 0x16, 20 => 0x17,
                22 => 0x18, 47 => 0x19, 17 => 0x1a, 45 => 0x1b, 21 => 0x1c,
                44 => 0x1d,
                2 => 0x1e, 3 => 0x1f, 4 => 0x20, 5 => 0x21, 6 => 0x22,
                7 => 0x23, 8 => 0x24, 9 => 0x25, 10 => 0x26, 11 => 0x27,
                28 => 0x28, 1 => 0x29, 14 => 0x2a, 15 => 0x2b, 57 => 0x2c,
                12 => 0x2d, 13 => 0x2e, 26 => 0x2f, 27 => 0x30, 43 => 0x31,
                39 => 0x33, 40 => 0x34, 41 => 0x35, 51 => 0x36, 52 => 0x37,
                53 => 0x38, 58 => 0x39,
                >= 59 and <= 68 => 0x3a + evdev - 59,
                87 => 0x44, 88 => 0x45, 99 => 0x46, 70 => 0x47, 119 => 0x48,
                110 => 0x49, 102 => 0x4a, 104 => 0x4b, 111 => 0x4c,
                107 => 0x4d, 109 => 0x4e, 106 => 0x4f, 105 => 0x50,
                108 => 0x51, 103 => 0x52, 69 => 0x53, 98 => 0x54,
                55 => 0x55, 74 => 0x56, 78 => 0x57, 96 => 0x58,
                79 => 0x59, 80 => 0x5a, 81 => 0x5b, 75 => 0x5c,
                76 => 0x5d, 77 => 0x5e, 71 => 0x5f, 72 => 0x60,
                73 => 0x61, 82 => 0x62, 83 => 0x63, 86 => 0x64,
                127 => 0x65, 116 => 0x66, 117 => 0x67,
                29 => 0xe0, 42 => 0xe1, 56 => 0xe2, 125 => 0xe3,
                97 => 0xe4, 54 => 0xe5, 100 => 0xe6, 126 => 0xe7,
                _ => 0,
            };
            return usage == 0 ? QtFallbackPlane | (nativeScanCode & 0xffffffff)
                : HidPlane + usage;
        }

        if (qtKey is >= 'A' and <= 'Z')
        {
            return HidPlane + 0x04 + qtKey - 'A';
        }

        if (qtKey is >= '1' and <= '9')
        {
            return HidPlane + 0x1e + qtKey - '1';
        }

        if (qtKey == '0')
        {
            return HidPlane + 0x27;
        }

        if (qtKey is >= 0x01000030 and <= 0x0100003b)
        {
            return HidPlane + 0x3a + qtKey - 0x01000030;
        }

        return qtKey switch
        {
            0x01000004 or 0x01000005 => HidPlane + 0x28,
            0x01000000 => HidPlane + 0x29,
            0x01000003 => HidPlane + 0x2a,
            0x01000001 => HidPlane + 0x2b,
            0x20 => HidPlane + 0x2c,
            0x01000006 => HidPlane + 0x49,
            0x01000010 => HidPlane + 0x4a,
            0x01000016 => HidPlane + 0x4b,
            0x01000007 => HidPlane + 0x4c,
            0x01000011 => HidPlane + 0x4d,
            0x01000017 => HidPlane + 0x4e,
            0x01000014 => HidPlane + 0x4f,
            0x01000012 => HidPlane + 0x50,
            0x01000015 => HidPlane + 0x51,
            0x01000013 => HidPlane + 0x52,
            0x01000021 => HidPlane + 0xe0,
            0x01000020 => HidPlane + 0xe1,
            0x01000023 => HidPlane + 0xe2,
            0x01000022 => HidPlane + 0xe3,
            _ => QtFallbackPlane | (nativeScanCode & 0xffffffff),
        };
    }

    internal static long Logical(long qtKey, string character)
    {
        if (
            !string.IsNullOrEmpty(character)
            && character.EnumerateRunes().Take(2).ToArray() is [var rune]
            && !Rune.IsControl(rune)
        )
        {
            return Rune.ToLowerInvariant(rune).Value;
        }
        // QKeyEvent.text() can contain a control character for Ctrl+letter,
        // and can be empty on release. Neither changes the logical letter.
        if (qtKey is >= 'A' and <= 'Z')
        {
            return 'a' + qtKey - 'A';
        }

        if (qtKey is >= '0' and <= '9')
        {
            return qtKey;
        }

        if (qtKey is >= 0x01000030 and <= 0x01000047)
        {
            return 0x100000801 + qtKey - 0x01000030;
        }

        return qtKey switch
        {
            0x01000003 => 0x100000008,
            0x01000001 => 0x100000009,
            0x01000004 or 0x01000005 => 0x10000000d,
            0x01000000 => 0x10000001b,
            0x01000007 => 0x10000007f,
            0x01000015 => 0x100000301,
            0x01000012 => 0x100000302,
            0x01000014 => 0x100000303,
            0x01000013 => 0x100000304,
            0x01000011 => 0x100000305,
            0x01000010 => 0x100000306,
            0x01000017 => 0x100000307,
            0x01000016 => 0x100000308,
            0x01000020 => 0x200000102,
            0x01000021 => 0x200000100,
            0x01000023 => 0x200000104,
            0x01000022 => 0x200000106,
            _ => 0x140000000 | (qtKey & 0xffffffff),
        };
    }
}
