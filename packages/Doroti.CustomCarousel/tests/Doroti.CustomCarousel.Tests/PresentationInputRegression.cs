using System.Text.Json;
using Doroti.CustomCarousel;
using Doroti.Framework.Foundation;
using Doroti.Framework.Gestures;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Testing;
using Doroti.Ui;
using SkiaSharp;
using Carousel = Doroti.CustomCarousel.CustomCarousel;
using Path = System.IO.Path;
using Image = Doroti.Framework.Widgets.Image;

internal static class PresentationInputRegression
{
    internal static void Run(string output)
    {
        Directory.CreateDirectory(output);
        var results = new List<object>();
        var failures = new List<string>();
        void Case(string name, Func<object> test)
        {
            object? detail = null;
            string? error = null;
            try { detail = test(); }
            catch (Exception exception) { error = exception.Message; failures.Add(name + ": " + error); }
            results.Add(new { name, status = error is null ? "PASS" : "FAIL", detail, error });
            Console.WriteLine($"{(error is null ? "PASS" : "FAIL")}: {name}{(error is null ? "" : ": " + error)}");
        }
        Case("coarse-wheel-first-frame-and-retarget", () =>
        {
            using var tester = new WidgetTester(new Size(400, 300));
            var controller = new CustomCarouselScrollController();
            tester.pumpWidget(Wrap(new Carousel(controller: controller, loop: true, scrollDirection: Axis.horizontal,
                alignment: Alignment.center, itemCountBefore: 1, itemCountAfter: 1,
                effectsBuilder: (_, ratio, child) => Transform.CreateTranslate(offset: new Offset(ratio * 150, 0), child: child),
                children: Enumerable.Range(0, 8).Select(i => (Widget)new SizedBox(width: 80, height: 100,
                    child: new ColoredBox(color: new Color(0xff336699 + i * 0x110000)))).ToArray())));
            var positions = new List<double>();
            for (var packet = 0; packet < 12; packet++)
            {
                tester.View.DispatchPlatformEvent(() => WidgetsFlutterBinding.instance.handlePointerEvent(new PointerScrollEvent(
                    viewId: checked((long)tester.View.viewId), position: new Offset(160, 150), scrollDelta: new Offset(120, 0))));
                tester.pump(TimeSpan.FromMilliseconds(16));
                positions.Add(controller.position.pixels);
            }
            tester.pumpWidget(new SizedBox());
            controller.dispose();
            Require(positions[0] >= 30, $"First input frame made no useful progress: {positions[0]:F2}px.");
            Require(positions.Zip(positions.Skip(1), (a, b) => b > a).All(v => v),
                "Retargeting a wheel packet stalled an active frame.");
            var lag = positions.Select((position, i) => (i + 1) * 120 - position).Max();
            Require(lag <= 60, $"The wheel response trailed the current input by {lag:F2}px.");
            return new { positions, maximumLag = lag };
        });
        Case("selection-retains-visible-card-state", () =>
        {
            using var tester = new WidgetTester(new Size(400, 300));
            var controller = new CustomCarouselScrollController();
            var mounts = new Dictionary<int, int>();
            var disposals = new Dictionary<int, int>();
            using var bitmap = new SKBitmap(8, 8);
            bitmap.Erase(SKColors.OrangeRed);
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            var photo = new MemoryImage(new Uint8List(data.ToArray()));
            tester.pumpWidget(Wrap(new Carousel(controller: controller, loop: true, scrollDirection: Axis.horizontal,
                alignment: Alignment.center, itemCountBefore: 1, itemCountAfter: 1, depthOrder: DepthOrder.selectedInFront,
                effectsBuilder: (_, ratio, child) => Transform.CreateTranslate(offset: new Offset(ratio * 150, 0), child: child),
                children: Enumerable.Range(0, 8).Select(i => (Widget)new CardProbe(i, mounts, disposals, photo)).ToArray())));
            bool HasImage(int i)
            {
                var point = tester.center(tester.byKey(new ValueKey<string>($"image-{i}")).Single());
                return tester.pixel((int)Math.Round(point.dx), (int)Math.Round(point.dy)) == SKColors.OrangeRed;
            }
            for (var frame = 0; frame < 30 && (!HasImage(0) || !HasImage(1)); frame++)
            {
                Thread.Sleep(2);
                tester.pump(TimeSpan.FromMilliseconds(16));
            }
            Require(HasImage(0) && HasImage(1), "Test image did not load before movement.");
            foreach (var offset in new[] { 180.0, 220.0, 180.0, 220.0 })
            {
                tester.View.DispatchPlatformEvent(() => controller.jumpTo(offset));
                tester.pump();
                Require(HasImage(0) && HasImage(1), "A visible card lost its image for a movement frame.");
            }
            var count0 = mounts[0];
            var count1 = mounts[1];
            var disposed0 = disposals.GetValueOrDefault(0);
            var disposed1 = disposals.GetValueOrDefault(1);
            tester.pumpWidget(new SizedBox());
            controller.dispose();
            Require(count0 == 1 && count1 == 1 && disposed0 == 0 && disposed1 == 0,
                $"Visible cards were recreated while selection crossed: mounts {count0}/{count1}, disposals {disposed0}/{disposed1}.");
            return new { count0, count1, disposed0, disposed1 };
        });
        Case("home-blur-retains-image-boundary", () =>
        {
            using var tester = new WidgetTester(new Size(560, 850));
            tester.pumpWidget(new DorotiCarouselApp.CarouselGallery());
            tester.pumpAndSettle();
            var original = CardBoundary(tester, "Cover Slider");
            var controller = ((Carousel)tester.byType<Carousel>().Single().widget).controller!;
            for (var step = 0; step < 3; step++)
            {
                tester.View.DispatchPlatformEvent(() => controller.position.pointerScroll(controller.position.itemExtent * 0.08));
                tester.pump(TimeSpan.FromMilliseconds(16));
                Require(ReferenceEquals(original, CardBoundary(tester, "Cover Slider")),
                    "Changing the home card's blur inserted/removed its image subtree.");
            }
            tester.pumpWidget(new SizedBox());
            return new { retainedFrames = 3 };
        });
        Case("deck-flip-retains-image-boundary", () =>
        {
            using var tester = new WidgetTester(new Size(560, 850));
            tester.pumpWidget(new DorotiCarouselApp.CarouselGallery());
            tester.pumpAndSettle();
            var home = ((Carousel)tester.byType<Carousel>().Single().widget).controller!;
            tester.View.DispatchPlatformEvent(() => home.jumpToItem(2));
            tester.pumpAndSettle();
            tester.tap(tester.byKey(new ValueKey<string>("open-demo")).Single());
            tester.pumpAndSettle();
            var photoKey = new ValueKey<string>("photo:card_deck/card-13.jpg");
            var original = NearestBoundary(tester.byKey(photoKey).Single());
            var controller = ((Carousel)tester.byType<Carousel>().Single().widget).controller!;
            foreach (var item in new[] { 12.9, 13.0, 12.9, 13.0 })
            {
                tester.View.DispatchPlatformEvent(() => controller.jumpTo(controller.position.itemExtent * item));
                tester.pump();
                Require(ReferenceEquals(original, NearestBoundary(tester.byKey(photoKey).Single())),
                    "Changing the deck card's flip inserted/removed its image subtree.");
            }
            tester.pumpWidget(new SizedBox());
            return new { retainedFrames = 4 };
        });
        File.WriteAllText(Path.Combine(output, "presentation-input.json"), JsonSerializer.Serialize(results,
            new JsonSerializerOptions { WriteIndented = true }));
        if (failures.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, failures));
    }

    private static Widget Wrap(Widget child) => new Directionality(textDirection: TextDirection.ltr, child: child);
    private static Element CardBoundary(WidgetTester tester, string title) => NearestBoundary(tester.text(title).Single());
    private static Element NearestBoundary(Element child)
    {
        Element? result = null;
        child.visitAncestorElements(element =>
        {
            if (element.widget is not RepaintBoundary) return true;
            result = element;
            return false;
        });
        return result ?? throw new InvalidOperationException("No card image boundary.");
    }
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
    private sealed class CardProbe(int index, Dictionary<int, int> mounts, Dictionary<int, int> disposals, MemoryImage image) : StatefulWidget
    {
        internal int Index { get; } = index;
        internal Dictionary<int, int> Mounts { get; } = mounts;
        internal Dictionary<int, int> Disposals { get; } = disposals;
        internal MemoryImage Image { get; } = image;
        public override IState createState() => new CardProbeState();
    }
    private sealed class CardProbeState : State<CardProbe>
    {
        public override void initState() { base.initState(); widget.Mounts[widget.Index] = widget.Mounts.GetValueOrDefault(widget.Index) + 1; }
        public override void dispose() { widget.Disposals[widget.Index] = widget.Disposals.GetValueOrDefault(widget.Index) + 1; base.dispose(); }
        public override Widget build(BuildContext context) => new Image(key: new ValueKey<string>($"image-{widget.Index}"),
            image: widget.Image, width: 80, height: 100, fit: BoxFit.fill);
    }
}
