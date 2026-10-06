using System.Collections.Concurrent;
using Doroti.CustomCarousel;
using Doroti.Framework;
using Doroti.Framework.Animation;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Hosting;
using Doroti.Material;
using Doroti.Runtime;
using Doroti.Ui;
using Carousel = Doroti.CustomCarousel.CustomCarousel;
using TextStyle = Doroti.Framework.Painting.TextStyle;
using Image = Doroti.Framework.Widgets.Image;

namespace DorotiCarouselApp;

public static class App
{
    public static Func<IDorotiViewEntrypoint> Definition => () => new DorotiWidgetEntrypoint(() => new CarouselGallery());
    public static DorotiViewConfiguration ViewConfiguration { get; } = new(
        "Doroti Custom Carousel", new Size(560, 850), Ui.Background, Ui.Ink,
        terminateAfterLastWindowClosed: true);
}

public sealed class CarouselGallery : StatefulWidget
{
    public override IState createState() => new CarouselGalleryState();
}

internal sealed class CarouselGalleryState : State<CarouselGallery>
{
    private int _demo = -1;
    private int _homeIndex;
    public override Widget build(BuildContext context) => new MaterialApp(
        title: "Doroti Custom Carousel", debugShowCheckedModeBanner: false,
        scrollBehavior: new ScrollBehavior().copyWith(scrollbars: false,
            dragDevices: [PointerDeviceKind.touch, PointerDeviceKind.mouse, PointerDeviceKind.stylus, PointerDeviceKind.trackpad]),
        theme: ThemeData.CreateLight().copyWith(scaffoldBackgroundColor: Ui.Background),
        home: new Scaffold(body: new SafeArea(child: new Center(child: new ConstrainedBox(
            constraints: new BoxConstraints(maxWidth: 780), child: new Column(children:
            [
                new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 28, vertical: 14), child: new Row(children:
                [
                    _demo >= 0
                        ? Ui.Button("Gallery", () => setState(() => _demo = -1), "back", icon: "←")
                        : Ui.Label("doroti", 22, FontWeight.w600, spacing: -0.8),
                    new SizedBox(width: 20),
                    new Expanded(child: _demo < 0 ? new Align(alignment: Alignment.centerRight,
                        child: Ui.Eyebrow("CAROUSEL GALLERY")) : Ui.Label(Ui.Titles[_demo], 16, FontWeight.w500)),
                ])),
                new Container(height: 1, color: Ui.Line),
                new Expanded(child: new KeyedSubtree(key: new ValueKey<int>(_demo), child:
                    _demo < 0 ? new HomeCarousel(_homeIndex, index => setState(() => { _homeIndex = index; _demo = index; })) : new CarouselDemo(_demo))),
            ]))))));
}

internal static class Ui
{
    public static readonly Color Background = new(0xfff7f6f2);
    public static readonly Color Surface = new(0xffffffff);
    public static readonly Color Line = new(0xffe3e1db);
    public static readonly Color Ink = new(0xff252622);
    public static readonly Color Muted = new(0xff86877f);
    public static readonly Color Accent = Ink;
    public static readonly Color RecordBackground = new(0xff1f1b2e);
    public static readonly string[] Titles = ["Cover Slider", "Circular Menu", "Card Deck", "Digital Wallet", "Record Box"];
    public static readonly Color[] ArtworkColors = [new(0xffe8e4dc), new(0xffe8ece2), new(0xffeae7ed), new(0xffe5e7eb), RecordBackground];
    public static readonly string[] Descriptions = ["Fresh finds, in focus.", "Built around your taste.",
        "A new hand, every time.", "Every card, in one place.", "A record for every mood."];
    private static readonly ConcurrentDictionary<string, MemoryImage> Images = new();
    public static MemoryImage Image(string path) => Images.GetOrAdd(path, name =>
    {
        var assembly = typeof(App).Assembly;
        var resource = assembly.GetManifestResourceNames().SingleOrDefault(n =>
            n.Replace('\\', '/').Equals("CarouselAssets." + name, StringComparison.Ordinal));
        using var source = resource is null ? null : assembly.GetManifestResourceStream(resource);
        if (source is null) throw new InvalidOperationException($"Missing carousel image: {name}");
        using var buffer = new MemoryStream();
        source.CopyTo(buffer);
        return new MemoryImage(new Uint8List(buffer.ToArray()));
    });
    public static Widget Label(string text, double size = 16, FontWeight? weight = null, Color? color = null,
        double? spacing = null, long? maxLines = null) =>
        new Text(text, maxLines: maxLines, overflow: maxLines is null ? TextOverflow.clip : TextOverflow.ellipsis,
            style: new TextStyle(fontSize: size, height: 1.2, letterSpacing: spacing, fontWeight: weight, color: color ?? Ink));
    public static Widget Eyebrow(string text, Color? color = null) => Label(text, 10, FontWeight.w600, color ?? Muted, 1.8);
    public static Widget Button(string text, Action onTap, string? key = null, bool primary = false,
        string? icon = null) => new TextButton(
        key: key is null ? null : new ValueKey<string>(key), onPressed: onTap,
        style: TextButton.styleFrom(foregroundColor: primary ? Surface : Ink,
            backgroundColor: primary ? Accent : new Color(0x00000000), minimumSize: new Size(44, 44),
            padding: EdgeInsets.CreateSymmetric(horizontal: 14, vertical: 12),
            shape: new RoundedRectangleBorder(borderRadius: BorderRadius.CreateCircular(24))),
        child: new Row(mainAxisSize: MainAxisSize.min, mainAxisAlignment: MainAxisAlignment.center, children:
        [.. (icon is null ? Array.Empty<Widget>() : new Widget[] { Label(icon, 18, color: primary ? Surface : Ink), new SizedBox(width: 7) }),
            Label(text, 13, FontWeight.w500, primary ? Surface : Ink)]));
    public static Widget Dots(int selected, Action<int> select) =>
        new Row(mainAxisAlignment: MainAxisAlignment.center, children: Enumerable.Range(0, Titles.Length).Select(i =>
            (Widget)new Semantics(button: true, selected: selected == i, label: $"Show {Titles[i]}", excludeSemantics: true,
                child: new Tooltip(message: Titles[i], child: new TextButton(onPressed: () => select(i),
                    style: TextButton.styleFrom(minimumSize: new Size(36, 44), padding: EdgeInsets.zero,
                        foregroundColor: Ink, shape: new RoundedRectangleBorder(borderRadius: BorderRadius.CreateCircular(12))),
                    child: new Container(width: selected == i ? 22 : 6, height: 6,
                        decoration: new BoxDecoration(color: selected == i ? Ink : new Color(0xffcecec6), borderRadius: BorderRadius.CreateCircular(4))))))).ToList());
    public static Widget Heading(string title, string description, double size = 30) =>
        new Container(width: double.PositiveInfinity, padding: EdgeInsets.CreateSymmetric(horizontal: 28, vertical: 24), child: new Column(
            crossAxisAlignment: CrossAxisAlignment.start, children:
            [Label(title, size, FontWeight.w500, spacing: -1), new SizedBox(height: 8), Label(description, 13, color: Muted)]));
    public static Widget Photo(string path, double width, double height, double radius = 16, Widget? child = null, Color? background = null) =>
        new Container(key: new ValueKey<string>("photo:" + path), width: width, height: height,
            decoration: new BoxDecoration(color: background, image: new DecorationImage(image: Image(path), fit: BoxFit.cover),
                borderRadius: BorderRadius.CreateCircular(radius)), child: child);
    public static Widget Preview(int demo) => new Container(color: ArtworkColors[demo], child: new Center(
        child: new FittedBox(fit: BoxFit.contain, child: new SizedBox(width: 420, height: 320, child: new Stack(
            alignment: Alignment.center, children: demo switch
            {
                0 => [Move(PreviewTile("cover_slider/food-a-1.jpg", 150, 200), -120, 18, rotate: -0.1),
                    Move(PreviewTile("cover_slider/food-b-3.jpg", 150, 200), 120, 18, rotate: 0.1),
                    Move(PreviewTile("cover_slider/food-b-2.jpg", 164, 224), 0, -8)],
                1 => [new SizedBox(width: 240, height: 240, child: new Stack(fit: StackFit.expand, children:
                        new[] { "bun", "lettuce", "burger", "cheese", "sauce", "ketchup" }.Select(id =>
                            (Widget)new Image(image: Image($"circular_menu/{id}.png"), fit: BoxFit.contain)).ToList())),
                    Move(new Image(image: Image("circular_menu/icon-lettuce.png"), width: 64, height: 64), -130, 65),
                    Move(new Image(image: Image("circular_menu/icon-cheese.png"), width: 56, height: 56), 130, -45)],
                2 => [Move(PreviewTile("card_deck/card-4.jpg", 152, 228, 12), -48, 12, rotate: -0.2),
                    Move(PreviewTile("card_deck/card-8.jpg", 152, 228, 12), 48, 12, rotate: 0.2),
                    PreviewTile("card_deck/card-13.jpg", 160, 240, 12)],
                3 => [Move(PreviewTile("digital_wallet/wallet-3.jpg", 286, 174), 24, -42, rotate: 0.06),
                    Move(PreviewTile("digital_wallet/wallet-2.jpg", 286, 174), 12, -12, rotate: 0.03),
                    Move(PreviewTile("digital_wallet/wallet-1.jpg", 286, 174, child: new Padding(
                        padding: EdgeInsets.CreateAll(22), child: new Column(crossAxisAlignment: CrossAxisAlignment.start,
                            children: [Label("doroti / wallet", 15, FontWeight.w500, new Color(0xffffffff)), new Spacer(),
                                Label("$11,234.56", 28, FontWeight.w500, new Color(0xffffffff)), new SizedBox(height: 8),
                                Label("••••  2345", 11, color: new Color(0xccffffff), spacing: 2)]))), 0, 30)],
                _ => Enumerable.Range(-4, 9).Select(distance => RecordLayer(
                    RecordSleeve((distance + 16) % 15, 196), distance / 4.5, 196, 320)).ToList(),
            })))));
    private static Widget PreviewTile(string path, double width, double height, double radius = 18, Widget? child = null) =>
        new Container(decoration: new BoxDecoration(borderRadius: BorderRadius.CreateCircular(radius),
            boxShadow: [new BoxShadow(color: new Color(0x20000000), offset: new Offset(0, 12), blurRadius: 22)]),
            child: Photo(path, width, height, radius, child));
    public static Widget RecordSleeve(int index, double size) => Photo($"record_box/album-{index}.jpg", size, size, 12,
        child: new ClipRRect(borderRadius: BorderRadius.CreateCircular(12), child: new Container(
            decoration: new BoxDecoration(borderRadius: BorderRadius.CreateCircular(12),
                border: new Border(top: new BorderSide(color: new Color(0x62ffffff), width: 2))))));
    public static Widget RecordLayer(Widget child, double ratio, double size, double height)
    {
        var progress = Math.Clamp((ratio + 1) / 2, 0, 1);
        var scale = 0.75 + 0.25 * Curves.fastEaseInToSlowEaseOut.transform(progress);
        var y = (height - size) * (Curves.easeIn.transform(progress) - 0.5) + size * 0.5 * progress;
        // A centered, bounded camera tilt keeps every cover on the visible
        // side of the projection plane, including the loop's entering cards.
        var pitch = Math.Clamp((-0.15 + 0.65 * progress) * Math.PI, -0.15 * Math.PI, 82 * Math.PI / 180);
        var camera = Matrix4.identity();
        camera.setEntry(3, 2, 0.0015);
        camera.rotateX(pitch);
        return Transform.CreateTranslate(offset: new Offset(0, y), child: Transform.CreateScale(scale: scale,
            child: new Transform(transform: camera, alignment: Alignment.center,
                child: new ColorFiltered(colorFilter: ColorFilter.mode(
                    RecordBackground.withOpacity(0.5 * (1 - progress)), BlendMode.srcATop), child: child))));
    }
    public static Widget Move(Widget child, double x, double y, double scale = 1, double rotate = 0) =>
        Transform.CreateTranslate(offset: new Offset(x, y), child:
            Transform.CreateRotate(angle: rotate, child: Transform.CreateScale(scale: scale, child: child)));
    public static Widget Blur(Widget child, double sigma) =>
        new ImageFiltered(enabled: sigma > 0.5, imageFilter: new ImageFilter(sigmaX: sigma, sigmaY: sigma), child: child);
    public static Widget Flip(Widget child, double radians, bool vertical = false)
    {
        var matrix = Matrix4.identity();
        matrix.setEntry(3, 2, 0.0015);
        if (vertical) matrix.rotateX(radians); else matrix.rotateY(radians);
        return new Transform(transform: matrix, alignment: Alignment.center, child: child);
    }
}

internal sealed class HomeCarousel(int initialItem, Action<int> open) : StatefulWidget
{
    public int InitialItem { get; } = initialItem;
    public Action<int> Open { get; } = open;
    public override IState createState() => new HomeCarouselState();
}

internal sealed class HomeCarouselState : State<HomeCarousel>
{
    private CustomCarouselScrollController _controller = null!;
    private int _selected;
    private int? _active = 0;
    public override void initState()
    {
        base.initState();
        _controller = new CustomCarouselScrollController(widget.InitialItem);
        _selected = widget.InitialItem;
        _active = widget.InitialItem;
    }
    public override void dispose() { _controller.dispose(); base.dispose(); }
    public override Widget build(BuildContext context) => new LayoutBuilder(builder: (layoutContext, viewport) => new Column(
        crossAxisAlignment: CrossAxisAlignment.stretch, children:
        [
            new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 28, vertical: 28), child: new Column(
                crossAxisAlignment: CrossAxisAlignment.start, children:
                [Ui.Label("Explore motion.", viewport.maxWidth < 360 ? 34 : viewport.maxWidth < 440 ? 40 : 48,
                    FontWeight.w500, spacing: -1.8), new SizedBox(height: 10),
                    Ui.Label("Five ways to move through a collection.", 13, color: Ui.Muted)])),
            new Expanded(child: new LayoutBuilder(builder: (_, box) =>
            {
                var width = Math.Min(440, Math.Max(180, box.maxWidth - 56));
                var height = Math.Min(width * 0.86, box.maxHeight * 0.8);
                return new ClipRect(child: new Carousel(controller: _controller, loop: true,
                    alignment: Alignment.center, itemCountBefore: 2, itemCountAfter: 2, scrollSpeed: 1.5,
                    depthOrder: DepthOrder.selectedInFront,
                    onSelectedItemChanged: i => setState(() => _selected = i),
                    onSettledItemChanged: i => setState(() => _active = i),
                    effectsBuilder: (_, ratio, child) => Ui.Move(
                        new Opacity(opacity: Math.Clamp(1 - Math.Abs(ratio) * 0.9, 0, 1), child: Ui.Flip(child, ratio * 0.25)),
                        0, ratio * height * 2.7, Math.Max(0.8, 1 - Math.Abs(ratio) * 0.15)),
                    children: Enumerable.Range(0, 5).Select(i => (Widget)new MouseRegion(
                        cursor: Doroti.Framework.Services.SystemMouseCursors.click, child: new GestureDetector(
                            onTap: () => { if (_active == i) widget.Open(i); },
                            child: new SizedBox(width: width, height: height, child: new ClipRRect(
                                borderRadius: BorderRadius.CreateCircular(20), child: Ui.Preview(i)))))).ToArray()));
            })),
            new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 28, vertical: 20), child: new Row(children:
            [new Expanded(child: new Column(crossAxisAlignment: CrossAxisAlignment.start, mainAxisSize: MainAxisSize.min,
                children: [Ui.Label(Ui.Titles[_selected], viewport.maxWidth < 360 ? 21 : 25, FontWeight.w500, spacing: -0.6),
                    new SizedBox(height: 6), Ui.Label(Ui.Descriptions[_selected], 12, color: Ui.Muted, maxLines: 1)])),
                new SizedBox(width: 12),
                Ui.Button("Open", () => { if (_active is { } index) widget.Open(index); }, "open-demo", primary: true, icon: "→")
            ])),
            new Container(height: 1, margin: EdgeInsets.CreateSymmetric(horizontal: 28), color: Ui.Line),
            new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 14, vertical: 12), child: new Row(children:
            [new Tooltip(message: "Previous demo", child: Ui.Button("↑", () => { _ = _controller.previousItem(); })),
                new Expanded(child: Ui.Dots(_selected, i => { _ = _controller.animateToItem(i); })),
                new Tooltip(message: "Next demo", child: Ui.Button("↓", () => { _ = _controller.nextItem(); }, "home-next"))]))
        ]));
}



internal sealed class CarouselDemo(int demo) : StatefulWidget
{
    public int Demo { get; } = demo;
    public override IState createState() => new CarouselDemoState();
}

internal sealed class CarouselDemoState : State<CarouselDemo>
{
    private CustomCarouselScrollController _controller = null!;
    private int _selected;
    private int[] _deck = Enumerable.Range(0, 14).ToArray();
    private bool _recordImagesPrimed;
    private string? _food;
    private readonly HashSet<string> _added = ["lettuce", "cheese", "sauce", "ketchup"];
    private static readonly string[] Toppings = ["lettuce", "cheese", "bacon", "tomatoes", "onions", "pickles", "egg", "sauce", "ketchup", "mustard"];
    private static readonly string[] Accounts = ["ProviderBank", "WidgetOne", "Dart Financial", "Dash+", "OtherBank"];
    private static readonly string[] Balances = ["$11,234.56", "$4,567.89", "$2,345.67", "10,678 pts", "$8,912.34"];

    public override void initState()
    {
        base.initState();
        _selected = widget.Demo == 2 ? 13 : 0;
        _controller = new CustomCarouselScrollController(initialItem: _selected);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (widget.Demo != 4 || _recordImagesPrimed) return;
        _recordImagesPrimed = true;
        // Warm this fixed collection when the view opens so new stack entries
        // can reuse decoded covers during fast scrolling.
        for (var index = 0; index < 15; index++)
            _ = ImageLibrary.precacheImage(Ui.Image($"record_box/album-{index}.jpg"), context);
    }

    public override void dispose() { _controller.dispose(); base.dispose(); }
    public override Widget build(BuildContext context)
    {
        if (widget.Demo == 0) return CoverSlider();
        return new LayoutBuilder(builder: (layoutContext, box) =>
        {
            var width = Math.Min(390, Math.Max(180, box.maxWidth - 72));
            return new Column(children:
            [
                Ui.Heading(widget.Demo switch
                    { 1 => "Make it yours.", 2 => "A fresh hand.", 3 => "All your cards.", _ => "On repeat." },
                    widget.Demo switch { 1 => "Swipe to choose. Tap to add a topping.",
                        2 => "Swipe through the deck, or shuffle.", 3 => "Swipe to switch accounts.",
                        _ => "Scroll to find your next favorite." }, 34),
                new Expanded(child: new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 28),
                    child: new ClipRRect(borderRadius: BorderRadius.CreateCircular(20), child: new Container(
                        color: Ui.ArtworkColors[widget.Demo], child: new LayoutBuilder(builder: (_, stage) =>
                        {
                            var height = stage.maxHeight;
                            return widget.Demo switch
                            {
                                1 => CircularMenu(width, height),
                                2 => CardDeck(width, height),
                                3 => DigitalWallet(width, height),
                                _ => RecordBox(stage.maxWidth, height),
                            };
                        }))))),
                new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 24, vertical: 16), child: new Column(children:
                [
                    Ui.Label(widget.Demo == 1 ? $"Toppings added: {_added.Count} · {Toppings[_selected]}" :
                        widget.Demo == 3 ? $"{Accounts[_selected]} · {Balances[_selected]}" :
                        $"{(widget.Demo == 2 ? "Card" : "Record")} {_selected + 1:00} / {(widget.Demo == 2 ? 14 : 15)}", 13,
                        color: Ui.Muted),
                    new SizedBox(height: 14),
                    new Row(mainAxisAlignment: MainAxisAlignment.center, children:
                    [
                        new Expanded(child: Ui.Button(box.maxWidth < 360 ? "Prev" : "Previous", () => { _ = _controller.previousItem(); }, "previous",
                            icon: widget.Demo == 4 ? "↑" : "←")),
                        new SizedBox(width: 8),
                        .. (widget.Demo == 2 ? new Widget[] { Ui.Button("Shuffle", Shuffle, "shuffle", icon: box.maxWidth < 360 ? null : "↻"), new SizedBox(width: 8) } : []),
                        new Expanded(child: Ui.Button("Next", () => { _ = _controller.nextItem(); }, "next",
                            icon: widget.Demo == 4 ? "↓" : "→")),
                    ]),
                ])),
            ]);
        });
    }

    private Carousel Make(IReadOnlyList<Widget> children, EffectsBuilder effects, Axis axis = Axis.horizontal,
        int before = 2, int after = 2, bool loop = true, double speed = 1,
        DepthOrder depth = DepthOrder.selectedInFront, bool reverse = false, bool tap = true,
        bool sticky = false, Alignment? alignment = null) => new(effects, children,
            controller: _controller, scrollDirection: axis, itemCountBefore: before, itemCountAfter: after,
            alignment: alignment ?? Alignment.center, loop: loop, reverse: reverse, tapToSelect: tap,
            scrollSpeed: speed, depthOrder: depth, physics: new CustomCarouselScrollPhysics(sticky: sticky),
            onSelectedItemChanged: i => setState(() => _selected = i));

    private Widget CoverSlider()
    {
        if (_food is not null) return new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children:
        [Ui.Heading("A closer look.", "Something worth savoring."),
            new Expanded(child: new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 24), child: new Center(child: new AspectRatio(aspectRatio: 1,
                child: new ClipRRect(borderRadius: BorderRadius.CreateCircular(24), child:
                    new Image(image: Ui.Image(_food), fit: BoxFit.cover)))))),
            new Padding(padding: EdgeInsets.CreateAll(24), child: Ui.Button("Back to menu", () => setState(() => _food = null),
                "close-detail", primary: true, icon: "←"))]);
        Widget Row(string category, int count, double height) => new SizedBox(height: height,
            child: new ClipRect(child: new Carousel(scrollDirection: Axis.horizontal, loop: true,
                itemCountBefore: 2, itemCountAfter: 2, alignment: Alignment.centerLeft, tapToSelect: false,
                effectsBuilder: (_, ratio, child) => Ui.Move(child, ratio * 176 * 2.5, 0),
                children: Enumerable.Range(1, count).Select(i =>
                {
                    var path = $"cover_slider/food-{category}-{i}.jpg";
                    return (Widget)new GestureDetector(key: new ValueKey<string>($"food-{category}-{i}"), onTap: () => setState(() => _food = path),
                        child: new MouseRegion(cursor: Doroti.Framework.Services.SystemMouseCursors.click,
                            child: Ui.Photo(path, 160, height, 18)));
                }).ToArray())));
        Widget Section(string number, string title, string category, int count, double height) => new Column(
            crossAxisAlignment: CrossAxisAlignment.start, children:
            [new Row(children: [Ui.Eyebrow(number), new SizedBox(width: 10),
                new Expanded(child: Ui.Label(title, 19, FontWeight.w600)), Ui.Label($"{count:00} plates", 11, color: Ui.Muted)]),
                new SizedBox(height: 12), Row(category, count, height)]);
        return new SingleChildScrollView(child: new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children:
        [Ui.Heading("On the menu.", "Discover a dish. Tap for a closer look.", 34),
            new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 24), child: new Column(
                crossAxisAlignment: CrossAxisAlignment.start, children:
                [Section("01", "For the table", "a", 6, 120), new SizedBox(height: 26),
                    Section("02", "The main event", "b", 10, 144), new SizedBox(height: 26),
                    Section("03", "A sweet finish", "c", 6, 180), new SizedBox(height: 24)]))]));
    }

    private Widget CircularMenu(double width, double height)
    {
        var iconSize = Math.Min(width * 0.45, height * 0.26);
        var items = Toppings.Select(id => (Widget)new GestureDetector(key: new ValueKey<string>($"topping-{id}"), onTap: () => setState(() =>
        { if (!_added.Add(id)) _added.Remove(id); }), child: new SizedBox(width: iconSize, height: iconSize,
            child: new Image(image: Ui.Image($"circular_menu/icon-{id}.png"), fit: BoxFit.contain)))).ToArray();
        var carousel = Make(items, (_, ratio, child) =>
        {
            var angle = (-ratio + 0.5) * Math.PI;
            var scale = (Math.Sin(angle) + 2) / 3;
            return Ui.Move(Ui.Blur(new Opacity(opacity: Math.Clamp(scale, 0, 1), child: child), 16 * (1 - scale)),
                width * 0.6 * Math.Cos(angle), Math.Min(height * 0.33, width * 0.48) * Math.Sin(angle), scale);
        }, before: 3, after: 3);
        return new Stack(fit: StackFit.expand, children:
        [new ColoredBox(color: Ui.ArtworkColors[1]), carousel,
         new IgnorePointer(child: new Center(child: new SizedBox(width: width * 0.76, height: width * 0.76,
            child: new Stack(fit: StackFit.expand, children:
                new[] { "bun", "lettuce", "burger", "cheese", "bacon", "tomatoes", "onions", "pickles", "egg", "sauce", "ketchup", "mustard" }
                    .Where(id => id is "bun" or "burger" || _added.Contains(id))
                    .Select(id => (Widget)new Image(image: Ui.Image($"circular_menu/{id}.png"), fit: BoxFit.contain)).ToList()))))]);
    }

    private Widget CardDeck(double width, double height)
    {
        var cardHeight = Math.Min(height * 0.8, width * 1.5);
        var cardWidth = cardHeight * 2 / 3;
        return new Stack(fit: StackFit.expand, children:
        [new Container(decoration: new BoxDecoration(image: new DecorationImage(image: Ui.Image("card_deck/background.jpg"), fit: BoxFit.cover))),
         new ColoredBox(color: new Color(0xbbeae7ed)),
         Make(_deck.Select(i => Ui.Photo($"card_deck/card-{i}.jpg", cardWidth, cardHeight, 24)).ToArray(),
            (_, ratio, child) => Ui.Move(
                Ui.Flip(new Opacity(opacity: Math.Clamp(1 - Math.Max(0, ratio), 0, 1), child: child),
                    -Math.Max(0, ratio) * 0.3 * Math.PI),
                ratio <= 0 ? ratio * 14 : ratio * cardWidth * 3,
                ratio <= 0 ? ratio * 10 : 0, rotate: ratio <= 0 ? -ratio * 0.12 : 0),
            before: 3, after: 0, loop: false, speed: 0.5, depth: DepthOrder.forward, tap: false, sticky: true)]);
    }

    private void Shuffle()
    {
        setState(() => Random.Shared.Shuffle(_deck.AsSpan(1)));
        _ = _controller.animateToItem(13, Duration.Create(milliseconds: 700));
    }

    private Widget DigitalWallet(double width, double height)
    {
        var cardHeight = Math.Min((height - 24) * 0.64, width * 0.8);
        return new Padding(padding: EdgeInsets.CreateOnly(bottom: 24), child: Make(
            Enumerable.Range(0, 5).Select(i => Ui.Photo($"digital_wallet/wallet-{i + 1}.jpg", width, cardHeight, 20,
            new Container(padding: EdgeInsets.CreateAll(24), decoration: new BoxDecoration(borderRadius: BorderRadius.CreateCircular(20),
                gradient: new LinearGradient(begin: Alignment.topLeft, end: Alignment.bottomRight,
                    colors: [new Color(0x33000000), new Color(0x88000000)])),
                child: new Column(crossAxisAlignment: CrossAxisAlignment.start, children:
                [new Row(children: [new Expanded(child: Ui.Label(Accounts[i], 20, FontWeight.w500, new Color(0xffffffff))),
                    Ui.Label($"{i + 1:00}", 12, color: new Color(0xffffffff))]),
                    new Spacer(), Ui.Eyebrow(i == 3 ? "REWARD POINTS" : "AVAILABLE BALANCE", new Color(0xffd5d6e0)),
                    new SizedBox(height: 8), Ui.Label(Balances[i], 30, FontWeight.w500, new Color(0xffffffff), spacing: -0.5),
                    new SizedBox(height: 14), Ui.Label($"••••  {2345 + i * 1111}", 13, color: new Color(0xffffffff), spacing: 2)])))).ToArray(),
            (_, ratio, child) => Ui.Move(Ui.Blur(new Opacity(opacity: Math.Clamp(1 - Math.Max(0, ratio), 0, 1), child: child),
                Math.Max(0, ratio) * 8), ratio * width * 0.7, -ratio * cardHeight * 0.45),
            before: 0, after: 3, reverse: true, speed: 0.5, depth: DepthOrder.reverse, alignment: Alignment.bottomCenter));
    }

    private Widget RecordBox(double width, double height)
    {
        var size = Math.Min(width * 0.70, height * 0.70);
        return Make(Enumerable.Range(0, 15).Select(i => Ui.RecordSleeve(i, size)).ToArray(),
            (_, ratio, child) => Ui.RecordLayer(child, ratio, size, height),
            axis: Axis.vertical, before: 4, after: 4, speed: 5, depth: DepthOrder.forward, tap: true);
    }
}
