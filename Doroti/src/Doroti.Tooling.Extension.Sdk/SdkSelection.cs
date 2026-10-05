using System.Text.Json;
using System.Text.RegularExpressions;
namespace Doroti.Tooling.Extension.Sdk;

internal static class SdkSelection
{
    internal static bool Accepts(string selected, string project, string framework, out string reason)
    {
        var match = Regex.Match(selected.Trim(), @"^(\d+\.\d+\.\d+)(-[^\s]+)?$");
        if (!match.Success || !Version.TryParse(match.Groups[1].Value, out var actual))
        { reason = "The selected executable returned an invalid SDK version."; return false; }
        var target = Regex.Match(framework, @"^net(\d+)");
        if (!target.Success || actual.Major < int.Parse(target.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture))
        { reason = "The selected SDK cannot build the target framework."; return false; }
        for (var directory = new DirectoryInfo(Path.GetDirectoryName(project)!); directory is not null; directory = directory.Parent)
        {
            var path = Path.Combine(directory.FullName, "global.json");
            if (!File.Exists(path)) continue;
            using var json = JsonDocument.Parse(File.ReadAllBytes(path));
            if (!json.RootElement.TryGetProperty("sdk", out var sdk) || !sdk.TryGetProperty("version", out var version)) break;
            var requestedText = version.GetString()!;
            if (!Version.TryParse(requestedText.Split('-')[0], out var requested))
            { reason = "Invalid SDK version in " + path; return false; }
            var policy = sdk.TryGetProperty("rollForward", out var forward) ? forward.GetString() : "patch";
            var prereleaseAllowed = !sdk.TryGetProperty("allowPrerelease", out var allow) || allow.GetBoolean();
            var compatible = actual >= requested && (policy switch
            {
                "disable" => selected.Trim() == requestedText,
                "patch" or "latestPatch" => actual.Major == requested.Major && actual.Minor == requested.Minor && actual.Build / 100 == requested.Build / 100,
                "feature" or "latestFeature" => actual.Major == requested.Major && actual.Minor == requested.Minor,
                "minor" or "latestMinor" => actual.Major == requested.Major,
                "major" or "latestMajor" => true,
                _ => false,
            }) && (prereleaseAllowed || !match.Groups[2].Success);
            reason = $"Selected SDK {selected.Trim()}, {path}: version={requestedText}, rollForward={policy}, allowPrerelease={prereleaseAllowed}.";
            return compatible;
        }
        reason = "Selected executable SDK " + selected.Trim() + " supports " + framework + ".";
        return true;
    }
}
