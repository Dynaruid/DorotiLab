using System.Text.Json;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal static class CompilerIdentityFactory
{
    public static CompilerIdentity Create(
        AnalyzerHome analyzerHome,
        string flutterBaselinePath,
        string workspaceId,
        CompilerProfile profile)
    {
        using var baseline = JsonDocument.Parse(File.ReadAllText(flutterBaselinePath));
        var flutterRevision = baseline.RootElement.TryGetProperty("upstreamRevision", out var goal3Revision)
            ? goal3Revision.GetString()
            : baseline.RootElement.TryGetProperty("flutterGitRevision", out var historicalRevision)
                ? historicalRevision.GetString()
                : null;
        if (string.IsNullOrWhiteSpace(flutterRevision))
        {
            throw new InvalidDataException("Flutter source lock revision is missing.");
        }
        var analyzerProject = analyzerHome.AnalyzerRoot;
        var dart = ProcessRunner.Run("dart", ["--version"], analyzerProject);
        dart.EnsureSuccess("Dart SDK identity");
        var versionText = string.Join(' ', dart.StandardOutput, dart.StandardError);
        var dartVersion = Regex.Match(versionText, @"Dart SDK version:\s*([^\s]+)", RegexOptions.CultureInvariant).Groups[1].Value;
        if (dartVersion.Length == 0)
        {
            throw new InvalidDataException($"Could not parse Dart SDK identity: {versionText}");
        }

        var analyzerLock = File.ReadAllText(Path.Combine(analyzerProject, "pubspec.lock"));
        var analyzerVersion = Regex.Match(
            analyzerLock,
            "(?ms)^  analyzer:\\s+.*?^    version:\\s+\"([^\"]+)\"",
            RegexOptions.CultureInvariant).Groups[1].Value;
        if (analyzerVersion != CompilerVersions.Analyzer)
        {
            throw new InvalidDataException($"Pinned analyzer is {analyzerVersion}; converter requires {CompilerVersions.Analyzer}.");
        }

        var buildProps = File.ReadAllText(Path.Combine(analyzerHome.DorotiRoot, "Directory.Build.props"));
        var versionPrefix = ReadXmlProperty(buildProps, "VersionPrefix");
        var versionSuffix = ReadXmlProperty(buildProps, "VersionSuffix");
        return new(
            CompilerVersions.Converter,
            dartVersion,
            CompilerVersions.Analyzer,
            flutterRevision,
            profile.EnableTypedSemanticCompiler ? "doroti.migration-ir/v3" : "doroti.migration-ir/v2",
            profile.IrVersion,
            profile.LoweringRuleSetVersion,
            profile.EmitterVersion,
            versionSuffix.Length == 0 ? versionPrefix : $"{versionPrefix}-{versionSuffix}",
            workspaceId);
    }

    private static string ReadXmlProperty(string xml, string name)
    {
        var value = Regex.Match(xml, $@"<{Regex.Escape(name)}>([^<]+)</{Regex.Escape(name)}>", RegexOptions.CultureInvariant).Groups[1].Value;
        if (value.Length == 0)
        {
            throw new InvalidDataException($"Directory.Build.props is missing {name}.");
        }

        return value;
    }
}
