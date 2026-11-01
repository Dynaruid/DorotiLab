using System.Reflection;
using Doroti.Ui;
using UiPath = Doroti.Ui.Path;

internal static class FrozenScrollRecordingRegression
{
    internal static void Run()
    {
        var pathSnapshot = typeof(UiPath).GetMethod("SnapshotForPainting", BindingFlags.Instance | BindingFlags.NonPublic)!
            .CreateDelegate<Func<UiPath, UiPath>>();
        var paragraphSnapshot = typeof(Paragraph).GetMethod("SnapshotForPainting", BindingFlags.Instance | BindingFlags.NonPublic)!
            .CreateDelegate<Func<Paragraph, Paragraph>>();
        var sourcePath = new UiPath();
        sourcePath.moveTo(1.25, 2.5);
        sourcePath.lineTo(3.75, 4.5);
        var recordedPath = pathSnapshot(sourcePath);
        sourcePath.reset();
        sourcePath.fillType = PathFillType.evenOdd;
        Check(recordedPath.Commands.Count == 2 && recordedPath.fillType == PathFillType.nonZero,
            "recorded path survives producer mutation");
        Check(ReferenceEquals(recordedPath, pathSnapshot(recordedPath)), "recorded path is shared across frozen frames");
        ExpectImmutable(() => recordedPath.lineTo(9, 9));
        ExpectImmutable(() => recordedPath.fillType = PathFillType.evenOdd);
        Check(recordedPath.Commands is not List<PathCommand>, "recorded command list does not expose the mutable producer list");

        var sourceParagraph = new Paragraph("already loaded text", 160, 24);
        sourceParagraph.layout(new ParagraphConstraints(160));
        var recordedParagraph = paragraphSnapshot(sourceParagraph);
        var oldWidth = recordedParagraph.width;
        var oldHeight = recordedParagraph.height;
        sourceParagraph.layout(new ParagraphConstraints(35));
        sourceParagraph.dispose();
        Check(recordedParagraph.width == oldWidth && recordedParagraph.height == oldHeight && !recordedParagraph.debugDisposed,
            "recorded paragraph survives relayout and disposal of the public handle");
        ExpectImmutable(() => recordedParagraph.layout(new ParagraphConstraints(10)));
        paragraphSnapshot(recordedParagraph); pathSnapshot(recordedPath);
        var bytes = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 20; i++)
        {
            pathSnapshot(recordedPath);
            paragraphSnapshot(recordedParagraph);
        }
        Check(GC.GetAllocatedBytesForCurrentThread() == bytes, "re-freezing closed path/text recordings allocates no copies");

        var freeze = typeof(DorotiFrozenScene).GetMethod("Capture", BindingFlags.Static | BindingFlags.NonPublic)!
            .CreateDelegate<Func<Scene, DorotiFrozenScene>>();
        var commands = typeof(DorotiFrozenScene).GetProperty("Commands", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var captureTextures = typeof(DorotiFrozenScene).GetMethod("CaptureTextures", BindingFlags.Instance | BindingFlags.NonPublic)!;
        DorotiFrozenScene Capture(DorotiFrozenScene scene, Func<long, bool, IDisposable> capture, bool refresh = false) =>
            (DorotiFrozenScene)captureTextures.Invoke(scene, [capture, refresh])!;

        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder);
        canvas.drawPath(recordedPath, new Paint());
        canvas.drawParagraph(recordedParagraph, Offset.zero);
        using var picture = recorder.endRecording();
        Check(picture.Commands[0].Arguments is IList<double> { IsReadOnly: true },
            "the recorder closes numeric arguments before publishing its picture");
        var builder = new SceneBuilder();
        builder.addPicture(Offset.zero, picture);
        using var sourceScene = builder.build();
        IReadOnlyList<PathCommand> DrawingCommands(DorotiFrozenScene scene)
        {
            var list = (IReadOnlyList<SceneCommand>)commands.GetValue(scene)!;
            var payload = typeof(SceneCommand).GetProperty("HostPayload", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(list[0])!;
            return (IReadOnlyList<PathCommand>)payload.GetType().GetProperty("Commands")!.GetValue(payload)!;
        }
        using (var one = freeze(sourceScene))
        using (var two = freeze(sourceScene))
            Check(ReferenceEquals(DrawingCommands(one)[0], DrawingCommands(two)[0]),
                "repeated frames reuse closed drawings without changing their command data");
        double[] mutableArguments = [3.5, 7.25];
        using (var publicPicture = new Picture([new PathCommand("translate", mutableArguments)]))
        {
            var publicBuilder = new SceneBuilder();
            publicBuilder.addPicture(Offset.zero, publicPicture);
            using var publicScene = publicBuilder.build();
            using var closed = freeze(publicScene);
            mutableArguments[0] = 999;
            Check(DrawingCommands(closed)[0].Arguments[0] == 3.5,
                "externally supplied mutable arguments never alias the frozen frame");
        }
        var frozen = freeze(sourceScene);
        var pure = Capture(frozen, (_, _) => throw new InvalidOperationException("No texture exists in this scene"));
        Check(ReferenceEquals(commands.GetValue(frozen), commands.GetValue(pure)), "texture-free frames retain closed storage");
        frozen.Dispose();
        Check(pure.CommandCount == 1, "shared snapshot survives release of its producer");
        pure.Dispose();

        var textures = new SceneBuilder();
        var layer = textures.pushOffset(3, 5);
        textures.addTexture(42, width: 10, height: 10);
        textures.addTexture(42, width: 20, height: 20);
        textures.addTexture(43, width: 10, height: 10, freeze: true);
        textures.pop();
        var retained = new SceneBuilder();
        retained.addRetained(layer);
        using var textureSource = retained.build();
        using var textureFrozen = freeze(textureSource);
        var captured = 0;
        var released = 0;
        IDisposable CaptureFrame(long id, bool locked) { captured++; return new Lease(() => released++); }
        var first = Capture(textureFrozen, CaptureFrame);
        Check(captured == 2 && !ReferenceEquals(commands.GetValue(textureFrozen), commands.GetValue(first)),
            "nested texture branches copy storage and capture each texture/freeze pair once");
        var replay = Capture(first, CaptureFrame, refresh: true);
        Check(captured == 3, "replay refreshes live textures and retains the explicitly frozen frame");
        first.Dispose();
        Check(released == 0, "replay retains the previous frozen texture leases");
        replay.Dispose();
        Check(released == 3, "last consumer releases every captured texture exactly once");
        Console.WriteLine("PASS: immutable scroll recordings, producer mutation/disposal, texture copy-on-write and lease retirement");
    }

    private sealed class Lease(Action release) : IDisposable
    {
        private bool _disposed;
        public void Dispose() { if (!_disposed) { _disposed = true; release(); } }
    }
    private static void ExpectImmutable(Action change)
    {
        try { change(); }
        catch (InvalidOperationException) { return; }
        throw new InvalidOperationException("A closed recording was mutable");
    }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
