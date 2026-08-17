using System.Text;

namespace Doroti.Host.Web;

/// <summary>Converts DOM KeyboardEvent values to Flutter-compatible key identifiers.</summary>
internal static class BrowserKeyMap
{
    private const long HidPlane = 0x00070000;

    private static readonly IReadOnlyDictionary<string, long> PhysicalKeys = new Dictionary<string, long>(StringComparer.Ordinal)
    {
        ["Enter"] = HidPlane + 0x28, ["Escape"] = HidPlane + 0x29,
        ["Backspace"] = HidPlane + 0x2a, ["Tab"] = HidPlane + 0x2b,
        ["Space"] = HidPlane + 0x2c, ["Minus"] = HidPlane + 0x2d,
        ["Equal"] = HidPlane + 0x2e, ["BracketLeft"] = HidPlane + 0x2f,
        ["BracketRight"] = HidPlane + 0x30, ["Backslash"] = HidPlane + 0x31,
        ["Semicolon"] = HidPlane + 0x33, ["Quote"] = HidPlane + 0x34,
        ["Backquote"] = HidPlane + 0x35, ["Comma"] = HidPlane + 0x36,
        ["Period"] = HidPlane + 0x37, ["Slash"] = HidPlane + 0x38,
        ["CapsLock"] = HidPlane + 0x39, ["PrintScreen"] = HidPlane + 0x46,
        ["ScrollLock"] = HidPlane + 0x47, ["Pause"] = HidPlane + 0x48,
        ["Insert"] = HidPlane + 0x49, ["Home"] = HidPlane + 0x4a,
        ["PageUp"] = HidPlane + 0x4b, ["Delete"] = HidPlane + 0x4c,
        ["End"] = HidPlane + 0x4d, ["PageDown"] = HidPlane + 0x4e,
        ["ArrowRight"] = HidPlane + 0x4f, ["ArrowLeft"] = HidPlane + 0x50,
        ["ArrowDown"] = HidPlane + 0x51, ["ArrowUp"] = HidPlane + 0x52,
        ["NumLock"] = HidPlane + 0x53, ["NumpadDivide"] = HidPlane + 0x54,
        ["NumpadMultiply"] = HidPlane + 0x55, ["NumpadSubtract"] = HidPlane + 0x56,
        ["NumpadAdd"] = HidPlane + 0x57, ["NumpadEnter"] = HidPlane + 0x58,
        ["NumpadDecimal"] = HidPlane + 0x63,
        ["ControlLeft"] = HidPlane + 0xe0, ["ShiftLeft"] = HidPlane + 0xe1,
        ["AltLeft"] = HidPlane + 0xe2, ["MetaLeft"] = HidPlane + 0xe3,
        ["ControlRight"] = HidPlane + 0xe4, ["ShiftRight"] = HidPlane + 0xe5,
        ["AltRight"] = HidPlane + 0xe6, ["MetaRight"] = HidPlane + 0xe7,
    };

    private static readonly IReadOnlyDictionary<string, long> LogicalKeys = new Dictionary<string, long>(StringComparer.Ordinal)
    {
        ["Backspace"] = 0x100000008, ["Tab"] = 0x100000009,
        ["Enter"] = 0x10000000d, ["Escape"] = 0x10000001b,
        ["Delete"] = 0x10000007f, ["ArrowDown"] = 0x100000301,
        ["ArrowLeft"] = 0x100000302, ["ArrowRight"] = 0x100000303,
        ["ArrowUp"] = 0x100000304, ["End"] = 0x100000305,
        ["Home"] = 0x100000306, ["PageDown"] = 0x100000307,
        ["PageUp"] = 0x100000308, ["ShiftLeft"] = 0x200000100,
        ["Insert"] = 0x100000407, ["CapsLock"] = 0x100000104,
        ["ShiftRight"] = 0x200000101, ["ControlLeft"] = 0x200000102,
        ["ControlRight"] = 0x200000103, ["AltLeft"] = 0x200000104,
        ["AltRight"] = 0x200000105, ["MetaLeft"] = 0x200000106,
        ["MetaRight"] = 0x200000107,
    };

    internal static long Physical(string code)
    {
        if (code.Length == 4 && code.StartsWith("Key", StringComparison.Ordinal) && code[3] is >= 'A' and <= 'Z')
            return HidPlane + 0x04 + code[3] - 'A';
        if (code.Length == 6 && code.StartsWith("Digit", StringComparison.Ordinal) && code[5] is >= '1' and <= '9')
            return HidPlane + 0x1e + code[5] - '1';
        if (code == "Digit0") return HidPlane + 0x27;
        if (code.Length is 2 or 3 && code[0] == 'F' && int.TryParse(code.AsSpan(1), out var function) && function is >= 1 and <= 12)
            return HidPlane + 0x3a + function - 1;
        if (code.StartsWith("Numpad", StringComparison.Ordinal) && code.Length == 7 && code[6] is >= '0' and <= '9')
            return code[6] == '0' ? HidPlane + 0x62 : HidPlane + 0x59 + code[6] - '1';
        return PhysicalKeys.TryGetValue(code, out var value) ? value : 0x100000000 | Stable32(code);
    }

    internal static long Logical(string code, string key)
    {
        if (LogicalKeys.TryGetValue(code, out var logical)) return logical;
        if (code.Length is 2 or 3 && code[0] == 'F' && int.TryParse(code.AsSpan(1), out var function) && function is >= 1 and <= 24)
            return 0x100000801 + function - 1;
        if (key == " ") return 0x20;
        if (key.EnumerateRunes().Take(2).ToArray() is [var rune])
            return Rune.ToLowerInvariant(rune).Value;
        var physical = Physical(code);
        return physical == 0 ? 0x200000000 | Stable32(key) : 0x100000000 | physical;
    }

    internal static string? Character(string key) =>
        key.EnumerateRunes().Take(2).Count() == 1 ? key : null;

    private static long Stable32(string value)
    {
        unchecked
        {
            uint hash = 2166136261;
            foreach (var character in value) hash = (hash ^ character) * 16777619;
            return hash;
        }
    }
}
