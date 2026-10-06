using Doroti.CustomCarousel;
using Doroti.Framework.Foundation;
using Doroti.Framework.Gestures;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Testing;
using Doroti.Ui;
using Carousel = Doroti.CustomCarousel.CustomCarousel;
using Path = System.IO.Path;

if (args.Contains("--presentation-input"))
{
    PresentationInputRegression.Run(args.LastOrDefault(a => !a.StartsWith("--")) ?? "temp/testing/carousel-presentation");
    return;
}

if (args.Contains("--continuous-input"))
{
    ContinuousInputRegression.Run(args.LastOrDefault(a => !a.StartsWith("--")) ?? "temp/testing/carousel-input");
    return;
}

static void Require(bool value, string message)
{
    if (!value) throw new InvalidOperationException(message);
}

var output = Path.GetFullPath(args.FirstOrDefault() ?? "temp/testing/carousel");
Directory.CreateDirectory(output);
ContinuousInputRegression.Run(Path.Combine(output, "input"));
PresentationInputRegression.Run(Path.Combine(output, "presentation"));
var changes = new List<int>();
var settled = new List<int?>();
var ratios = new Dictionary<int, double>();
var controller = new CustomCarouselScrollController(initialItem: -1);
Widget Build(bool loop = true, bool reverse = false, int count = 8, DepthOrder depth = DepthOrder.selectedInFront,
    CustomCarouselScrollController? ownedController = null) => new Directionality(textDirection: TextDirection.ltr,
        child: new Carousel(controller: ownedController ?? controller, loop: loop, reverse: reverse,
            scrollDirection: Axis.horizontal, itemCountBefore: count == 1 ? 0 : 1, itemCountAfter: count == 1 ? 0 : 1,
            alignment: Alignment.center, depthOrder: depth, physics: new CustomCarouselScrollPhysics(sticky: true),
            onSelectedItemChanged: changes.Add, onSettledItemChanged: settled.Add,
            effectsBuilder: (i, ratio, child) =>
            {
                ratios[i] = ratio;
                return Transform.CreateTranslate(offset: new Offset(ratio * 120, 0), child: child);
            },
            children: Enumerable.Range(0, count).Select(i => (Widget)new SizedBox(width: 60, height: 100,
                child: new ColoredBox(color: new Color(0xff336699 + i * 0x110000), child: new Text($"Item {i}")))).ToArray()));

using (var tester = new WidgetTester(new Size(400, 400)))
{
    tester.pumpWidget(Build());
    Require(controller.selectedItem == 7, "Negative initialItem must use Euclidean modulo.");
    Require(Math.Abs(ratios[7]) < 0.001 && ratios.ContainsKey(0) && ratios.ContainsKey(6), "Loop seam visible items/ratios.");
    foreach (var (depth, expected) in new[]
    {
        (DepthOrder.forward, new[] { 6, 7, 0 }),
        (DepthOrder.reverse, new[] { 0, 7, 6 }),
        (DepthOrder.selectedInFront, new[] { 0, 6, 7 }),
    })
    {
        tester.pumpWidget(Build(depth: depth));
        var stack = (Stack)tester.byType<Stack>().Single().widget;
        var actual = stack.children.Select(w => ((ValueKey<int>)w.key!).value).ToArray();
        Require(actual.SequenceEqual(expected), $"Paint depth order {depth}: {string.Join(',', actual)}.");
    }
    Task? moving = null;
    tester.View.DispatchPlatformEvent(() => moving = controller.nextItem(Duration.Create(milliseconds: 240)));
    tester.pump();
    tester.pump(TimeSpan.FromMilliseconds(120));
    Require(Math.Abs(controller.position.pixels - 3000) < 60, "Loop navigation must take the shortest path (one item).");
    tester.pumpUntilComplete(moving!);
    tester.pumpAndSettle();
    Require(controller.selectedItem == 0 && settled.Last() == 0 && settled.Contains(null), "Selected/settled callback lifecycle.");
    tester.tapAt(new Offset(280, 200));
    tester.pumpAndSettle();
    Require(controller.selectedItem == 1, "Tapping an unselected transformed child selects it through hit testing.");
    tester.setViewport(new Size(800, 400), 2);
    Require(controller.selectedItem == 1 && Math.Abs(controller.position.pixels - 800) < 0.01, "Resize preserves the selected item.");
    tester.drag(tester.byType<Carousel>().Single(), new Offset(-500, 0));
    tester.pumpAndSettle();
    Require(controller.selectedItem > 1 && Math.Abs(controller.position.pixels / controller.position.itemExtent - controller.selectedItem) < 0.01,
        "Pointer drag/fling snaps to an item.");
    tester.View.DispatchPlatformEvent(() => controller.jumpToItem(-17));
    tester.pump();
    Require(controller.selectedItem == 7, "Large negative jumps wrap correctly.");
    tester.View.DispatchPlatformEvent(() => WidgetsFlutterBinding.instance.handlePointerEvent(
        new PointerScrollEvent(viewId: checked((long)tester.View.viewId), position: new Offset(400, 200), scrollDelta: new Offset(800, 0))));
    tester.pumpAndSettle();
    Require(controller.selectedItem == 0, "Mouse wheel crosses the loop seam and snaps.");
    tester.View.DispatchPlatformEvent(() => controller.position.pointerScroll(800 * 100));
    tester.pumpAndSettle();
    Require(controller.selectedItem == 4, "Loop wheel input is independent of the finite scroll buffer.");
    tester.pumpWidget(Build(loop: false));
    tester.pumpAndSettle();
    tester.View.DispatchPlatformEvent(() => controller.jumpToItem(-10));
    tester.pump();
    Require(controller.selectedItem == 0, "Non-loop negative jumps clamp.");
    tester.View.DispatchPlatformEvent(() => controller.jumpToItem(99));
    tester.pump();
    Require(controller.selectedItem == 7, "Non-loop positive jumps clamp.");
    tester.pumpWidget(Build(loop: false, count: 4));
    tester.pumpAndSettle();
    Require(controller.selectedItem == 3, "Shrinking children clamps selection.");
    var replacement = new CustomCarouselScrollController(initialItem: 2);
    tester.pumpWidget(Build(ownedController: replacement));
    tester.pump();
    Require(!controller.hasClients && replacement.hasClients, "Controller replacement detaches the previous owner.");
    tester.pumpWidget(new SizedBox());
    Require(!replacement.hasClients, "Unmount detaches the caller-owned controller.");
    replacement.dispose();
    controller.dispose();
}
Console.WriteLine("PASS: loop seam, depth order, shortest navigation, callbacks, tap hit testing, drag/wheel snap, resize, bounds, child updates and ownership.");

using (var tester = new WidgetTester(new Size(400, 400)))
{
    using var scope = new ControllerScope(new CustomCarouselScrollController());
    controller = scope.Controller;
    tester.pumpWidget(Build(reverse: true));
    tester.drag(tester.byType<Carousel>().Single(), new Offset(-260, 0));
    tester.pumpAndSettle();
    Require(controller.selectedItem == 7, "Reverse input wraps to the previous item.");
    tester.pumpWidget(Build(count: 1));
    tester.pumpAndSettle();
    tester.View.DispatchPlatformEvent(() => controller.jumpToItem(999));
    tester.pump();
    Require(controller.selectedItem == 0, "Single-item loop remains valid.");
    tester.pumpWidget(new SizedBox());
}
Console.WriteLine("PASS: reversed pointer input and single-item carousel.");

using (var tester = new WidgetTester(new Size(560, 850)))
{
    tester.pumpWidget(new DorotiCarouselApp.CarouselGallery());
    tester.pumpAndSettle();
    // Image decoding completes asynchronously; pump the owner event loop before capture.
    for (var frame = 0; frame < 10; frame++) { Thread.Sleep(30); tester.pump(TimeSpan.FromMilliseconds(16)); }
    tester.WritePng(Path.Combine(output, "home.png"));
    for (var demo = 0; demo < 5; demo++)
    {
        tester.tap(tester.byKey(new ValueKey<string>("open-demo")).Single());
        tester.pumpAndSettle();
        Require(tester.text(new[] { "Cover Slider", "Circular Menu", "Card Deck", "Digital Wallet", "Record Box" }[demo]).Count > 0, "Demo navigation.");
        for (var frame = 0; frame < 10; frame++) { Thread.Sleep(30); tester.pump(TimeSpan.FromMilliseconds(16)); }
        tester.WritePng(Path.Combine(output, $"demo-{demo}.png"));
        if (demo > 0)
        {
            if (demo == 1)
            {
                tester.tap(tester.byKey(new ValueKey<string>("topping-lettuce")).Single());
                tester.pumpAndSettle();
                Require(tester.find(w => w is Text text && text.data?.StartsWith("Toppings added: 3") == true).Count > 0,
                    "Selected circular-menu item toggles a topping.");
            }
            if (demo == 2)
            {
                tester.tap(tester.byKey(new ValueKey<string>("previous")).Single());
                tester.pumpAndSettle();
            }
            tester.tap(tester.byKey(new ValueKey<string>("next")).Single());
            tester.pumpAndSettle();
        }
        else
        {
            tester.tap(tester.byKey(new ValueKey<string>("food-a-1")).Single());
            tester.pumpAndSettle();
            Require(tester.byKey(new ValueKey<string>("close-detail")).Count == 1, "Cover image opens detail.");
            tester.tap(tester.byKey(new ValueKey<string>("close-detail")).Single());
        }
        if (demo == 2)
        {
            tester.tap(tester.byKey(new ValueKey<string>("shuffle")).Single());
            tester.pumpAndSettle();
        }
        tester.setViewport(new Size(390, 740), 1);
        tester.pumpAndSettle();
        tester.WritePng(Path.Combine(output, $"demo-{demo}-mobile.png"));
        tester.setViewport(new Size(560, 850), 1);
        tester.tap(tester.byKey(new ValueKey<string>("back")).Single());
        tester.pumpAndSettle();
        if (demo < 4)
        {
            tester.tap(tester.byKey(new ValueKey<string>("home-next")).Single());
            tester.pumpAndSettle();
        }
    }
    tester.pumpWidget(new SizedBox());
}
Console.WriteLine($"PASS: all five sample demos, navigation, controls, image detail, shuffle and two viewport sizes. PNGs: {output}");

internal sealed class ControllerScope(CustomCarouselScrollController controller) : IDisposable
{
    public CustomCarouselScrollController Controller { get; } = controller;
    public void Dispose() => Controller.dispose();
}
