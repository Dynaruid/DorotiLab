namespace Doroti.Host.Maui;

/// <summary>Platform-independent key translation used by native MAUI input adapters.</summary>
internal static class MauiKeyMap
{
    internal static long Logical(string key, long physical)
    {
        // UIKit/AppKit use private-use characters for arrows/function keys.
        // A modifier or navigation HID identity takes precedence over that text.
        var special = SpecialLogical(physical);
        if (special != 0)
        {
            return special;
        }

        if (key.EnumerateRunes().Take(2).ToArray() is [var rune] && !System.Text.Rune.IsControl(rune))
        {
            return System.Text.Rune.ToLowerInvariant(rune).Value;
        }

        if (
            key.Length is 2 or 3
            && key[0] == 'F'
            && int.TryParse(key.AsSpan(1), out var function)
            && function is >= 1 and <= 24
        )
        {
            return 0x100000801 + function - 1;
        }

        return key switch
        {
            "Backspace" => 0x100000008,
            "Tab" => 0x100000009,
            "Enter" => 0x10000000d,
            "Escape" => 0x10000001b,
            "Delete" => 0x10000007f,
            "ArrowDown" => 0x100000301,
            "ArrowLeft" => 0x100000302,
            "ArrowRight" => 0x100000303,
            "ArrowUp" => 0x100000304,
            "End" => 0x100000305,
            "Home" => 0x100000306,
            "PageDown" => 0x100000307,
            "PageUp" => 0x100000308,
            "ShiftLeft" => 0x200000102,
            "ShiftRight" => 0x200000103,
            "ControlLeft" => 0x200000100,
            "ControlRight" => 0x200000101,
            "AltLeft" => 0x200000104,
            "AltRight" => 0x200000105,
            "MetaLeft" => 0x200000106,
            "MetaRight" => 0x200000107,
            _ => physical switch
            {
                >= 0x70004 and <= 0x7001d => 'a' + physical - 0x70004,
                >= 0x7001e and <= 0x70026 => '1' + physical - 0x7001e,
                0x70027 => '0',
                _ => physical == 0 ? 0 : 0x100000000 | physical,
            },
        };
    }

    private static long SpecialLogical(long physical) =>
        physical switch
        {
            0x70028 => 0x10000000d,
            0x70058 => 0x20000020d,
            >= 0x70059 and <= 0x70061 => 0x200000231 + physical - 0x70059,
            0x70062 => 0x200000230,
            0x70054 => 0x20000022f,
            0x70055 => 0x20000022a,
            0x70056 => 0x20000022d,
            0x70057 => 0x20000022b,
            0x70063 => 0x20000022e,
            0x70029 => 0x10000001b,
            0x7002a => 0x100000008,
            0x7002b => 0x100000009,
            0x70049 => 0x100000407,
            0x7004a => 0x100000306,
            0x7004b => 0x100000308,
            0x7004c => 0x10000007f,
            0x7004d => 0x100000305,
            0x7004e => 0x100000307,
            0x7004f => 0x100000303,
            0x70050 => 0x100000302,
            0x70051 => 0x100000301,
            0x70052 => 0x100000304,
            >= 0x7003a and <= 0x70045 => 0x100000801 + physical - 0x7003a,
            >= 0x70068 and <= 0x70073 => 0x10000080d + physical - 0x70068,
            0x700e0 => 0x200000100,
            0x700e4 => 0x200000101,
            0x700e1 => 0x200000102,
            0x700e5 => 0x200000103,
            0x700e2 => 0x200000104,
            0x700e6 => 0x200000105,
            0x700e3 => 0x200000106,
            0x700e7 => 0x200000107,
            _ => 0,
        };

    // android.view.KeyEvent keycodes; kept free of Android runtime types so
    // the same conversion used by the event adapter is regression-tested.
    internal static long AndroidPhysical(int keyCode) =>
        keyCode switch
        {
            >= 29 and <= 54 => 0x70004 + keyCode - 29,
            >= 8 and <= 16 => 0x7001e + keyCode - 8,
            7 => 0x70027,
            66 => 0x70028,
            111 or 4 => 0x70029,
            67 => 0x7002a,
            61 => 0x7002b,
            62 => 0x7002c,
            112 => 0x7004c,
            22 => 0x7004f,
            21 => 0x70050,
            20 => 0x70051,
            19 => 0x70052,
            122 => 0x7004a,
            123 => 0x7004d,
            92 => 0x7004b,
            93 => 0x7004e,
            113 => 0x700e0,
            114 => 0x700e4,
            59 => 0x700e1,
            60 => 0x700e5,
            57 => 0x700e2,
            58 => 0x700e6,
            117 => 0x700e3,
            118 => 0x700e7,
            >= 131 and <= 142 => 0x7003a + keyCode - 131,
            >= 145 and <= 153 => 0x70059 + keyCode - 145,
            144 => 0x70062,
            154 => 0x70054,
            155 => 0x70055,
            156 => 0x70056,
            157 => 0x70057,
            158 => 0x70063,
            160 => 0x70058,
            69 => 0x7002d, 70 => 0x7002e, 71 => 0x7002f, 72 => 0x70030,
            73 => 0x70031, 74 => 0x70033, 75 => 0x70034, 68 => 0x70035,
            55 => 0x70036, 56 => 0x70037, 76 => 0x70038,
            _ => 0x100000000 | (uint)keyCode,
        };

    // Android keyboard scan codes normally use Linux evdev positions. Keep
    // unknown nonzero codes distinct; keycode fallback is for synthetic events.
    internal static long AndroidPhysical(int keyCode, int scanCode)
    {
        if (scanCode == 0) return AndroidPhysical(keyCode);
            var usage = scanCode switch
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
                >= 59 and <= 68 => 0x3a + scanCode - 59,
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
            return usage == 0 ? 0x110000000 | (uint)scanCode
                : 0x70000 + usage;
    }

    // getUnicodeChar can return a dead-key flag or a full Unicode scalar.
    // Invalid/dead-key values are not committed text; the native IME owns composition.
    internal static string? AndroidCharacter(int unicode) =>
        System.Text.Rune.TryCreate(unicode, out var rune) && !System.Text.Rune.IsControl(rune)
            && unicode != 0 ? rune.ToString() : null;
}
