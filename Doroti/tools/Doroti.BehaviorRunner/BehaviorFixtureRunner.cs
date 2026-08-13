using Doroti.Graphics;
using Doroti.Rendering;
using Doroti.Tooling;

namespace Doroti.BehaviorRunner;

public static class BehaviorFixtureRunner
{
    public static BehaviorResultDocument Run(string fixturePath, string outputPath)
    {
        var fixture = ArtifactFiles.ReadJson<BehaviorFixtureDocument>(fixturePath);
        if (fixture.SchemaVersion != "doroti.behavior-fixture/v1")
        {
            throw new InvalidDataException($"Unsupported behavior fixture schema {fixture.SchemaVersion}.");
        }
        var results = fixture.Cases
            .OrderBy(item => item.Id, StringComparer.Ordinal)
            .Select(RunCase)
            .ToArray();
        var document = new BehaviorResultDocument(
            "doroti.behavior-result/v1",
            "doroti",
            fixture.FlutterGitRevision,
            results);
        ArtifactFiles.WriteJson(outputPath, document);
        return document;
    }

    public static BehaviorDelta Compare(string referencePath, string actualPath, string outputPath)
    {
        var reference = ArtifactFiles.ReadJson<BehaviorResultDocument>(referencePath);
        var actual = ArtifactFiles.ReadJson<BehaviorResultDocument>(actualPath);
        if (reference.SchemaVersion != "doroti.behavior-result/v1" || actual.SchemaVersion != reference.SchemaVersion)
        {
            throw new InvalidDataException("Behavior result schemas do not match v1.");
        }
        if (reference.FlutterGitRevision != actual.FlutterGitRevision)
        {
            throw new InvalidDataException("Behavior results use different Flutter revisions.");
        }
        var expected = reference.Results.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var observed = actual.Results.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var ids = expected.Keys.Union(observed.Keys, StringComparer.Ordinal).OrderBy(item => item, StringComparer.Ordinal);
        var differences = new List<BehaviorDifference>();
        foreach (var id in ids)
        {
            expected.TryGetValue(id, out var left);
            observed.TryGetValue(id, out var right);
            if (left is null || right is null)
            {
                differences.Add(new(id, left, right, "missing-result"));
            }
            else if (left != right)
            {
                differences.Add(new(id, left, right, "value-mismatch"));
            }
        }
        var delta = new BehaviorDelta("doroti.behavior-delta/v1", differences.Count == 0, differences.ToArray());
        ArtifactFiles.WriteJson(outputPath, delta);
        return delta;
    }

    private static BehaviorCaseResult RunCase(BehaviorCase fixture)
    {
        var constraints = new BoxConstraints(
            fixture.Constraints.MinWidth,
            fixture.Constraints.MaxWidth,
            fixture.Constraints.MinHeight,
            fixture.Constraints.MaxHeight);
        return fixture.Operation switch
        {
            "constrain" => Result(fixture.Id, constraints.Constrain(new(fixture.Width, fixture.Height))),
            "loosen-constrain" => Result(fixture.Id, constraints.Loosen().Constrain(new(fixture.Width, fixture.Height))),
            "deflate-constrain" => Result(
                fixture.Id,
                constraints.Deflate(new(fixture.InsetLeft, fixture.InsetTop, fixture.InsetRight, fixture.InsetBottom))
                    .Constrain(new(fixture.Width, fixture.Height))),
            _ => throw new InvalidDataException($"Unknown behavior operation {fixture.Operation} in {fixture.Id}."),
        };
    }

    private static BehaviorCaseResult Result(string id, Size size) => new(id, size.Width, size.Height);
}

public sealed record BehaviorFixtureDocument(
    string SchemaVersion,
    string FlutterGitRevision,
    BehaviorCase[] Cases);

public sealed record BehaviorCase(
    string Id,
    string Operation,
    BehaviorConstraints Constraints,
    double Width,
    double Height,
    double InsetLeft = 0,
    double InsetTop = 0,
    double InsetRight = 0,
    double InsetBottom = 0);

public sealed record BehaviorConstraints(double MinWidth, double MaxWidth, double MinHeight, double MaxHeight);

public sealed record BehaviorResultDocument(
    string SchemaVersion,
    string Runner,
    string FlutterGitRevision,
    BehaviorCaseResult[] Results);

public sealed record BehaviorCaseResult(string Id, double Width, double Height);

public sealed record BehaviorDifference(
    string Id,
    BehaviorCaseResult? Reference,
    BehaviorCaseResult? Actual,
    string Cause);

public sealed record BehaviorDelta(string SchemaVersion, bool Matches, BehaviorDifference[] Differences);
