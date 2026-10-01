using System.Reflection;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Testing;
using Doroti.Ui;
using SkiaSharp;

internal static class ShaderFramePipelineRegression
{
    public static void Run()
    {
        using var tester = new WidgetTester(new Size(80, 60));
        var host = (ISkiaSceneRendererHost)typeof(WidgetTester)
            .GetField("_host", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tester)!;
        using var renderer = new SkiaSceneRenderer(1, host, null, null, "pipeline-test", "cpu", "cpu", false);
        long frameworkFrame = 0;
        void Submit(IReadOnlyList<SceneCommand> commands)
        {
            using var scene = new Scene(1, commands);
            renderer.Submit(1, new(scene, new(host.ViewEpoch, ++frameworkFrame, 80, 60)),
                DorotiUiInvocation.Managed("ShaderFramePipelineRegression"));
        }
        var variable = new SceneCommand("backdropFilter", null)
        {
            HostPayload = new SceneBackdropFilterPayload(ImageFilterSnapshot.Capture(
                ImageFilter.variableBlur(new(0, 0), new(0, 40), resolutionScale: .25)), BlendMode.srcOver, null),
        };
        Submit([variable, new("pop", null)]);
        if (!renderer.CanRecordShaderSceneAhead) throw new Exception("A new shader scene was not available for pipelining.");
        // Nested native/shield content must stay pending for the serial path.
        var retained = new SceneCommand("retained", null) { HostPayload = new SceneRetainedPayload(
            [new("inputShield", null)], 1, 0) };
        Submit([variable, new("pop", null), retained]);
        if (renderer.CanRecordShaderSceneAhead) throw new Exception("Native composition was admitted ahead of GPU completion.");
        using var surface = SKSurface.Create(new SKImageInfo(80, 60));
        var rejected = renderer.PaintNewShaderScene(surface, 80, 60, host.ResizeTarget);
        if (rejected.Disposition != SkiaPaintDisposition.superseded || rejected.Completion is not null
            || !renderer.Diagnostics.PendingScene)
            throw new Exception("Fresh-only admission consumed a scene that needed native composition.");
        SceneCommand Picture(SKColor color, long identity)
        {
            var commands = new List<PathCommand>();
            var canvas = new Doroti.Ui.Canvas(commands);
            canvas.drawRect(new Rect(0, 0, 80, 60), new Doroti.Ui.Paint { color = new Color(color.Red == 255 ? 0xffff0000 : 0xff00ff00) });
            return new("picture", null) { HostPayload = new ScenePicturePayload(identity, new(0, 0), commands, null, false, true) };
        }
        Submit([Picture(SKColors.Red, 6001)]);
        var older = renderer.Paint(surface, 80, 60, host.ResizeTarget).Completion ?? throw new Exception("Older paint missing.");
        Submit([Picture(SKColors.Lime, 6002)]);
        var newer = renderer.Paint(surface, 80, 60, host.ResizeTarget).Completion ?? throw new Exception("Newer paint missing.");
        renderer.CompletePaint(newer);
        renderer.CompletePaint(older);
        if (renderer.CanRecordShaderSceneAhead) throw new Exception("A replay was reported as a fresh shader scene.");
        var noFresh = renderer.PaintNewShaderScene(surface, 80, 60, host.ResizeTarget);
        if (noFresh.Completion is not null || noFresh.Disposition != SkiaPaintDisposition.superseded)
            throw new Exception("Fresh-only recording allowed a retained replay.");
        var replay = renderer.Paint(surface, 80, 60, host.ResizeTarget);
        using var image = surface.Snapshot();
        using var bitmap = SKBitmap.FromImage(image);
        if (bitmap.GetPixel(10, 10) != SKColors.Lime || replay.Disposition != SkiaPaintDisposition.replay)
            throw new Exception("An older GPU completion replaced the newest replay source.");
        Console.WriteLine("PASS: fresh shader admission; native/replay deferral; out-of-order completion keeps the newest scene (CPU lifecycle contract).");
    }
}
