namespace Doroti.Host.Maui;

/// <summary>Platform-independent key translation used by native MAUI input adapters.</summary>
internal static class MauiKeyMap
{
    internal static long Logical(string key, long physical)
    {
        // UIKit/AppKit use private-use characters for arrows/function keys.
        // A modifier or navigation HID identity takes precedence over that text.
        var special = SpecialLogical(physical);
        if (special != 0) return special;
        if (key.Length == 1 && !char.IsControl(key[0])) return char.ToLowerInvariant(key[0]);
        if (key.Length is 2 or 3 && key[0] == 'F' && int.TryParse(key.AsSpan(1), out var function) && function is >= 1 and <= 24)
            return 0x100000801 + function - 1;
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

    private static long SpecialLogical(long physical) => physical switch
    {
        0x70028 => 0x10000000d, 0x70058 => 0x20000020d,
        0x70029 => 0x10000001b, 0x7002a => 0x100000008, 0x7002b => 0x100000009,
        0x70049 => 0x100000407, 0x7004a => 0x100000306, 0x7004b => 0x100000308,
        0x7004c => 0x10000007f, 0x7004d => 0x100000305, 0x7004e => 0x100000307,
        0x7004f => 0x100000303, 0x70050 => 0x100000302, 0x70051 => 0x100000301, 0x70052 => 0x100000304,
        >= 0x7003a and <= 0x70045 => 0x100000801 + physical - 0x7003a,
        >= 0x70068 and <= 0x70073 => 0x10000080d + physical - 0x70068,
        0x700e0 => 0x200000100, 0x700e4 => 0x200000101,
        0x700e1 => 0x200000102, 0x700e5 => 0x200000103,
        0x700e2 => 0x200000104, 0x700e6 => 0x200000105,
        0x700e3 => 0x200000106, 0x700e7 => 0x200000107,
        _ => 0,
    };

    // android.view.KeyEvent keycodes; kept free of Android runtime types so
    // the same conversion used by the event adapter is regression-tested.
    internal static long AndroidPhysical(int keyCode) => keyCode switch
    {
        >= 29 and <= 54 => 0x70004 + keyCode - 29,
        >= 8 and <= 16 => 0x7001e + keyCode - 8,
        7 => 0x70027, 66 => 0x70028, 111 or 4 => 0x70029,
        67 => 0x7002a, 61 => 0x7002b, 62 => 0x7002c, 112 => 0x7004c,
        22 => 0x7004f, 21 => 0x70050, 20 => 0x70051, 19 => 0x70052,
        122 => 0x7004a, 123 => 0x7004d, 92 => 0x7004b, 93 => 0x7004e,
        113 => 0x700e0, 114 => 0x700e4, 59 => 0x700e1, 60 => 0x700e5,
        57 => 0x700e2, 58 => 0x700e6, 117 => 0x700e3, 118 => 0x700e7,
        _ => 0x100000000 | (uint)keyCode,
    };

}
