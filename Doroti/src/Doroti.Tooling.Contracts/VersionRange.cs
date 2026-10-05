using System.Globalization;
namespace Doroti.Tooling.Contracts;
/// <summary>Strict bounded SemVer ranges for provider/host handshake compatibility.</summary>
public static class ToolVersionRange
{
    private sealed record SemVersion(int Major, int Minor, int Patch, string[] Pre) : IComparable<SemVersion>
    {
        public int CompareTo(SemVersion? other)
        {
            if (other is null) return 1;
            var result = Major.CompareTo(other.Major); if (result != 0) return result;
            result = Minor.CompareTo(other.Minor); if (result != 0) return result;
            result = Patch.CompareTo(other.Patch); if (result != 0) return result;
            if (Pre.Length == 0 || other.Pre.Length == 0) return Pre.Length == other.Pre.Length ? 0 : Pre.Length == 0 ? 1 : -1;
            for (var i = 0; i < Math.Min(Pre.Length, other.Pre.Length); i++)
            {
                var firstNumeric = Pre[i].All(char.IsAsciiDigit);
                var secondNumeric = other.Pre[i].All(char.IsAsciiDigit);
                result = firstNumeric && secondNumeric ? Pre[i].Length != other.Pre[i].Length ? Pre[i].Length.CompareTo(other.Pre[i].Length) : string.CompareOrdinal(Pre[i], other.Pre[i]) : firstNumeric != secondNumeric ? firstNumeric ? -1 : 1 : string.CompareOrdinal(Pre[i], other.Pre[i]);
                if (result != 0) return result;
            }
            return Pre.Length.CompareTo(other.Pre.Length);
        }
    }
    private static SemVersion Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length > 256) throw new ToolContractException("invalid-version", text ?? "null");
        var metadata = text.Split('+');
        if (metadata.Length > 2 || metadata.Length == 2 && metadata[1].Split('.').Any(part => part.Length == 0 || part.Any(character => !char.IsAsciiLetterOrDigit(character) && character != '-')))
            throw new ToolContractException("invalid-version", text);
        var version = metadata[0].Split('-', 2);
        var core = version[0].Split('.');
        if (core.Length != 3 || core.Any(part => part.Length == 0 || part.Length > 1 && part[0] == '0' || !int.TryParse(part, NumberStyles.None, CultureInfo.InvariantCulture, out _))) throw new ToolContractException("invalid-version", text);
        var pre = version.Length == 1 ? [] : version[1].Split('.');
        if (pre.Any(part => part.Length == 0 || part.Any(character => !char.IsAsciiLetterOrDigit(character) && character != '-') || part.All(char.IsAsciiDigit) && part.Length > 1 && part[0] == '0')) throw new ToolContractException("invalid-version", text);
        return new(int.Parse(core[0], CultureInfo.InvariantCulture), int.Parse(core[1], CultureInfo.InvariantCulture), int.Parse(core[2], CultureInfo.InvariantCulture), pre);
    }
    public static bool Contains(string range, string version)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(range);
        var value = Parse(version);
        if (range.Length < 3 || range[0] is not ('[' or '(') || range[^1] is not (']' or ')')) throw new ToolContractException("invalid-core-range", range);
        var bounds = range[1..^1].Split(',').Select(part => part.Trim()).ToArray();
        if (bounds.Length == 1) return range[0] == '[' && range[^1] == ']' && value.CompareTo(Parse(bounds[0])) == 0;
        if (bounds.Length != 2 || bounds.All(string.IsNullOrEmpty)) throw new ToolContractException("invalid-core-range", range);
        if (bounds.All(bound => bound.Length != 0) && Parse(bounds[0]).CompareTo(Parse(bounds[1])) > 0) throw new ToolContractException("invalid-core-range", range);
        if (bounds[0].Length != 0) { var lower = value.CompareTo(Parse(bounds[0])); if (lower < 0 || lower == 0 && range[0] == '(') return false; }
        if (bounds[1].Length != 0) { var upper = value.CompareTo(Parse(bounds[1])); if (upper > 0 || upper == 0 && range[^1] == ')') return false; }
        return true;
    }
}
