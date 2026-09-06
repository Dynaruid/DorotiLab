using System.Reflection;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
using Material = Doroti.Framework.Material;

internal static class MaterialSampleContracts
{
    // Public widget construction and recorded pixels, independent of Testbed state.
    // Route mounting, focus and host readback must also be exercised by live tests.
    public static async Task<int> Verify(bool searchOnly = false)
    {
        var failures = 0;
        await Check("D04/default-enabled", () =>
        {
            Require(Create().enabled, "SearchAnchor.bar must be enabled by default");
            Require(!Material.SearchAnchor.CreateBar(enabled: false, suggestionsBuilder: Suggestions).enabled,
                "explicit disabled state must survive the factory");
            return Task.CompletedTask;
        });
        await Check("D04/default-bar", () =>
        {
            using var controller = new ControllerLease();
            var bar = (Material.SearchBar)Create().builder(null!, controller.Value);
            Require(bar.trailing is null, "omitted trailing remains null");
            Require(bar.scrollPadding.Equals(EdgeInsets.CreateAll(20)), "default scroll padding is 20");
            Require(bar.contextMenuBuilder is not null, "default editing context menu is available");
            return Task.CompletedTask;
        });
        await Check("D04/optional-callbacks", () =>
        {
            var anchor = Create();
            anchor.viewOnOpen?.Invoke();
            anchor.viewOnClose?.Invoke();
            Require(anchor.viewOnOpen is null && anchor.viewOnClose is null,
                "omitted route lifecycle callbacks retain their null contract");
            return Task.CompletedTask;
        });
        await Check("D04/explicit-options", () =>
        {
            var opened = 0;
            var closed = 0;
            var trailing = new Widget[] { SizedBox.CreateShrink() };
            var padding = EdgeInsets.CreateAll(7);
            Func<BuildContext, EditableTextState, Widget> menu = (_, _) => SizedBox.CreateShrink();
            var anchor = Material.SearchAnchor.CreateBar(barTrailing: trailing, viewTrailing: trailing,
                scrollPadding: padding, contextMenuBuilder: menu,
                onOpen: () => opened++, onClose: () => closed++, suggestionsBuilder: Suggestions);
            using var controller = new ControllerLease();
            var bar = (Material.SearchBar)anchor.builder(null!, controller.Value);
            anchor.viewOnOpen?.Invoke();
            anchor.viewOnClose?.Invoke();
            Require(opened == 1 && closed == 1, "explicit callbacks fire once");
            Require(bar.trailing!.SequenceEqual(trailing) && anchor.viewTrailing!.SequenceEqual(trailing),
                "explicit trailing widgets are retained");
            Require(ReferenceEquals(bar.scrollPadding, padding) && ReferenceEquals(bar.contextMenuBuilder, menu),
                "explicit editing options are retained");
            return Task.CompletedTask;
        });
        await Check("D05-D11/host-boundaries", SampleHostContracts.Verify);
        await Check("D09/default-theme-dispatch", () => { SampleDefaultsContracts.Verify(); return Task.CompletedTask; });
        await Check("D08/drawer-typed-key", () =>
        {
            var key = new GlobalKey<Material.DrawerControllerState>();
            var drawer = new Material.DrawerController(key: key, alignment: Material.DrawerAlignment.end, child: new Text("Independent drawer"));
            Require(ReferenceEquals(drawer.key, key), "typed drawer key must retain identity without invariant generic casts");
            return Task.CompletedTask;
        });
        if (!searchOnly)
        {
            await Check("D01-D02/picture-raw-rgba", async () =>
            {
                using var environment = new ImageFixtureEnvironment();
                var recorder = new PictureRecorder();
                var canvas = new Canvas(recorder);
                canvas.drawRect(new Rect(0, 0, 1, 1), new Paint { color = new Color(0xffff0000) });
                using var picture = recorder.endRecording();
                using var image = await picture.toImage(2, 1);
                Require(image.width == 2 && image.height == 1, "recorded image dimensions");
                var data = await image.toByteData(ImageByteFormat.rawRgba);
                Require(data is not null, "raw RGBA bytes must be returned");
                Require(data!.asMemory().Span.SequenceEqual(new byte[] { 255, 0, 0, 255, 0, 0, 0, 0 }),
                    "recorded red pixel and transparent pixel must survive rasterization");
                await Doroti.Validation.ImagePipelineValidation.VerifyPixels();
            });
            await Check("D03/max-colors", async () =>
            {
                // The quantizer is currently internal; reflection isolates it from the blocked
                // image-provider path without widening the product API for a validation fixture.
                var type = typeof(Material.ColorScheme).Assembly.GetType("Doroti.Framework.Material.QuantizerCelebi")!;
                var quantizer = Activator.CreateInstance(type, nonPublic: true)!;
                var future = (Future)type.GetMethod("quantize", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .Invoke(quantizer, [new long[] { 0xffff0000, 0xff00ff00, 0xff0000ff }, 1L, false])!;
                await future;
                var result = future.GetType().GetMethod("GetAwaiter")!.Invoke(future, null)!;
                var value = result.GetType().GetMethod("GetResult")!.Invoke(result, null)!;
                var colors = (DartMap<long, long>)value.GetType()
                    .GetProperty("colorToCount", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(value)!;
                Require(colors.Count <= 1, $"maxColors=1 returned {colors.Count} colors");
            });
            await Check("D03/score-gray-majority", () =>
            {
                // Pinned Dart oracle selects the chromatic minority, not the gray majority.
                Require(Score(new DartMap<long, long> { [0xff808080] = 100, [0xffff0000] = 10 })
                    .SequenceEqual(new long[] { 0xffff0000 }), "theme score must reject the gray majority");
                return Task.CompletedTask;
            });
            await Check("D03/score-empty-fallback", () =>
            {
                Require(Score(new DartMap<long, long>()).SequenceEqual(new long[] { 0xff4285f4 }),
                    "empty score must return the reference Google Blue fallback");
                return Task.CompletedTask;
            });
        }
        Console.WriteLine($"Material sample contracts: {(failures == 0 ? "PASS" : "FAIL")} ({failures} failures)");
        return failures == 0 ? 0 : 1;

        async Task Check(string id, Func<Task> action)
        {
            try { await action(); Console.WriteLine($"{id}: PASS"); }
            catch (Exception exception) { failures++; Console.WriteLine($"{id}: FAIL\n{exception}"); }
        }
    }

    private static Material.SearchAnchor Create() =>
        Material.SearchAnchor.CreateBar(barHintText: "Search", suggestionsBuilder: Suggestions);
    private static object Suggestions(BuildContext context, Material.SearchController controller) => Array.Empty<Widget>();
    private static IEnumerable<long> Score(DartMap<long, long> colors) =>
        (IEnumerable<long>)typeof(Material.ColorScheme).Assembly.GetType("Doroti.Framework.Material.Score")!
            .GetMethod("score", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [colors, 1L])!;
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
    private sealed class ControllerLease : IDisposable
    {
        internal Material.SearchController Value { get; } = new();
        public void Dispose() => Value.dispose();
    }
}
