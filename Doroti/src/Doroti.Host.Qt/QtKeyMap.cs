using System.Text;

namespace Doroti.Host.Qt;

/// <summary>Maps Qt key identities to Doroti/Flutter-compatible logical and USB HID planes.</summary>
internal static class QtKeyMap
{
    private const long HidPlane = 0x00070000;
    private const long QtFallbackPlane = 0x130000000;

    internal static long Physical(long nativeScanCode, long qtKey)
    {
        if (qtKey is >= 'A' and <= 'Z') return HidPlane + 0x04 + qtKey - 'A';
        if (qtKey is >= '1' and <= '9') return HidPlane + 0x1e + qtKey - '1';
        if (qtKey == '0') return HidPlane + 0x27;
        if (qtKey is >= 0x01000030 and <= 0x0100003b)
            return HidPlane + 0x3a + qtKey - 0x01000030;
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
        if (!string.IsNullOrEmpty(character) &&
            character.EnumerateRunes().Take(2).ToArray() is [var rune])
            return Rune.ToLowerInvariant(rune).Value;
        if (qtKey is >= 0x01000030 and <= 0x01000047)
            return 0x100000801 + qtKey - 0x01000030;
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
            0x01000020 => 0x200000100,
            0x01000021 => 0x200000102,
            0x01000023 => 0x200000104,
            0x01000022 => 0x200000106,
            _ => 0x140000000 | (qtKey & 0xffffffff),
        };
    }
}
