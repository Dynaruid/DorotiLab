using System.Text;
using Doroti.Rendering;
using Doroti.Tooling;

namespace Doroti.SceneLab;

public static class SceneRunner
{
    public static RenderTreeTraceDocument WriteRenderTreeTrace(
        string outputPath,
        IReadOnlyList<RenderTraceEvent> events,
        RenderPipelineFrame frame)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        ArgumentNullException.ThrowIfNull(events);
        ArgumentNullException.ThrowIfNull(frame);
        var document = new RenderTreeTraceDocument(
            "doroti.render-tree-trace/v1",
            frame.Sequence,
            frame.Snapshot.Bounds,
            frame.Snapshot.DisplayListBytes,
            events.ToArray());
        ArtifactFiles.WriteJson(outputPath, document);
        return document;
    }

    public static SceneResult Render(string scenePath, string outputDirectory, string? actualColor = null)
    {
        var scene = ArtifactFiles.ReadJson<SolidScene>(scenePath);
        if (scene.SchemaVersion != "doroti.scene/v1")
        {
            throw new InvalidDataException($"Unsupported scene schema: {scene.SchemaVersion}");
        }

        var expected = new RgbaImage(scene.Width, scene.Height, Rgba.Parse(scene.Color));
        var actual = new RgbaImage(scene.Width, scene.Height, Rgba.Parse(actualColor ?? scene.Color));
        var diff = RgbaImage.Diff(expected, actual);
        Directory.CreateDirectory(outputDirectory);
        var expectedPath = Path.Combine(outputDirectory, "expected.png");
        var actualPath = Path.Combine(outputDirectory, "actual.png");
        var diffPath = Path.Combine(outputDirectory, "diff.png");
        expected.SavePng(expectedPath);
        actual.SavePng(actualPath);
        diff.Image.SavePng(diffPath);

        var result = new SceneResult(
            "doroti.scene-result/v1",
            scene.Name,
            scene.Width,
            scene.Height,
            diff.MismatchedPixels == 0,
            diff.MismatchedPixels,
            diff.MaxChannelDelta,
            new SceneArtifacts("expected.png", "actual.png", "diff.png"));
        ArtifactFiles.WriteJson(Path.Combine(outputDirectory, "result.json"), result);
        ArtifactFiles.WriteUtf8(Path.Combine(outputDirectory, "result.md"), ToMarkdown(result));
        return result;
    }

    private static string ToMarkdown(SceneResult result)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# SceneLab: {result.Scene}");
        builder.AppendLine();
        builder.AppendLine($"Status: **{(result.Matches ? "PASS" : "FAIL")}**");
        builder.AppendLine();
        builder.AppendLine($"- Size: {result.Width} x {result.Height}");
        builder.AppendLine($"- Mismatched pixels: {result.MismatchedPixels}");
        builder.AppendLine($"- Maximum channel delta: {result.MaxChannelDelta}");
        builder.AppendLine($"- Expected: `{result.Artifacts.Expected}`");
        builder.AppendLine($"- Actual: `{result.Artifacts.Actual}`");
        builder.AppendLine($"- Diff: `{result.Artifacts.Diff}`");
        return builder.ToString();
    }
}

public sealed record SolidScene(string SchemaVersion, string Name, int Width, int Height, string Color);
public sealed record SceneResult(string SchemaVersion, string Scene, int Width, int Height, bool Matches, int MismatchedPixels, int MaxChannelDelta, SceneArtifacts Artifacts);
public sealed record SceneArtifacts(string Expected, string Actual, string Diff);
public sealed record RenderTreeTraceDocument(
    string SchemaVersion,
    long FrameSequence,
    Doroti.Graphics.Rect Bounds,
    int DisplayListBytes,
    RenderTraceEvent[] Events);
