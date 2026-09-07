using Doroti.Graphics.DisplayList;
using Doroti.Host.Web;
using Doroti.Ui;
using UiPath = Doroti.Ui.Path;

internal static class RetainedMappingContracts
{
    internal static void Verify()
    {
        var path = new UiPath(); path.addRect(Rect.fromLTWH(1, 2, 30, 40));
        var textPaint = new Paint { color = new Color(0xff123456) };
        var paragraph = new Paragraph("retained text", 150, 20,
            textRuns: [new ParagraphTextRun("retained text", new TextStyle(foreground: textPaint))]);
        paragraph.layout(new ParagraphConstraints(150));
        var recorder = new PictureRecorder(); var canvas = new Canvas(recorder);
        double[] matrix = [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 7, 9, 0, 1];
        canvas.transform(matrix);
        canvas.clipPath(path, doAntiAlias: false);
        canvas.clipRRect(RRect.fromRectAndRadius(Rect.fromLTWH(0, 0, 100, 100), new Radius(5, 5)), doAntiAlias: false);
        canvas.drawPath(path, new Paint());
        canvas.drawShadow(path, new Color(0xff000000), 3, false);
        canvas.drawParagraph(paragraph, Offset.zero);
        using var picture = recorder.endRecording();
        var builder = new SceneBuilder(1); builder.addPicture(Offset.zero, picture);
        using var scene = builder.build();
        var resources = new Fonts(); var retained = new BrowserRetainedMappingCache();
        var encoder = new DisplayListEncodingCache(); var blocks = new BrowserPictureBlockCache(true);
        var metadata = new DisplayListSceneMetadata(1, 1, 1, 1, 1, 1, 800, 600, 800, 600, 1);
        DisplayListDocument Map() => BrowserDisplayListMapper.Create(scene, metadata, 0, resources, blocks, retained);
        var initial = Map(); var bytes = DisplayListEncoder.Encode(initial, encoder);
        Check(!initial.Commands.OfType<DisplayClipPathCommand>().Single().IsAntiAlias &&
            !initial.Commands.OfType<DisplayClipRoundedRectCommand>().Single().IsAntiAlias,
            "recorded path and rounded-rectangle clips preserve the caller's antialiasing choice");
        var warm = Map();
        Check(blocks.Hits == 1 && blocks.MappedCommands == 0 &&
            ReferenceEquals(initial.Commands.OfType<DisplayDrawParagraphCommand>().Single(),
                warm.Commands.OfType<DisplayDrawParagraphCommand>().Single()),
            "a retained picture reuses its complete mapped text/path command body");
        Check(ReferenceEquals(initial.Commands.OfType<DisplayDrawPathCommand>().Single().Path,
            warm.Commands.OfType<DisplayDrawPathCommand>().Single().Path), "retained geometry is actually reused");
        Check(ReferenceEquals(initial.Commands.OfType<DisplayDrawParagraphCommand>().Single().Paragraph,
            warm.Commands.OfType<DisplayDrawParagraphCommand>().Single().Paragraph), "retained text recipe is actually reused");
        Check(DisplayListEncoder.Encode(warm, encoder).SequenceEqual(bytes) && encoder.FrameHits > 0, "warm encoding preserves bytes");

        matrix[12] = 999;
        textPaint.color = new Color(0xffabcdef);
        path.reset(); path.addOval(Rect.fromLTWH(50, 60, 70, 80));
        paragraph.layout(new ParagraphConstraints(25)); paragraph.dispose();
        Check(DisplayListEncoder.Encode(Map(), encoder).SequenceEqual(bytes),
            "mutating geometry, transform and paragraph after recording cannot alter a retained Picture");
        var newRecorder = new PictureRecorder(); var newCanvas = new Canvas(newRecorder);
        newCanvas.drawPath(path, new Paint());
        using var newPicture = newRecorder.endRecording(); var newBuilder = new SceneBuilder(1);
        newBuilder.addPicture(Offset.zero, newPicture); using var newScene = newBuilder.build();
        var repainted = BrowserDisplayListMapper.Create(newScene, metadata, 0, resources, retained: retained);
        Check(repainted.Commands.OfType<DisplayDrawPathCommand>().Single().Path.Verbs.Contains(DisplayPathVerb.AddOval),
            "a repaint records the changed path and cannot hit the old mapping");

        resources.Font = new(DisplayResourceKind.Font, 2, 1);
        var fontChanged = Map();
        Check(fontChanged.Commands.OfType<DisplayDrawParagraphCommand>().Single().Paragraph.Font == resources.Font,
            "font registration invalidates the retained paragraph recipe");
        Check(DisplayListEncoder.Encode(fontChanged, encoder).SequenceEqual(DisplayListEncoder.Encode(fontChanged)),
            "changed font table encodes canonically");
        resources.Live = false;
        try { Map(); throw new Exception("cached paragraph bypassed live resource validation"); }
        catch (KeyNotFoundException) { }
        retained.Clear(); encoder.Clear();
        Console.WriteLine("Retained mapping: PASS (snapshot, repaint, text geometry, fonts, resource lifetime, canonical bytes)");
    }
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    private sealed class Fonts : IBrowserDisplayListResources
    {
        internal DisplayResourceReference Font = new(DisplayResourceKind.Font, 1, 1);
        internal bool Live = true;
        public DisplayResourceReference DefaultFont => Font;
        public DisplayResourceReference ResolveFont(string? family) => Font;
        public IReadOnlyList<DisplayResourceReference> RegisteredFonts => [Font];
        public DisplayResourceDescriptor Describe(DisplayResourceReference reference) => Live
            ? new(reference, new(0, 0)) : throw new KeyNotFoundException();
        public DisplayResourceReference ResolveImage(Image image) => throw new NotSupportedException();
        public DisplayResourceReference ResolveRuntimeEffect(FragmentShaderState effect) => throw new NotSupportedException();
    }
}
