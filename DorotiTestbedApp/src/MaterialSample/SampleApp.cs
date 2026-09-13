// Copyright 2021 The Flutter team. All rights reserved.
// Adapted from reference/flutter_sample_app; BSD license in LICENSE.flutter.
using Doroti.Framework.Animation;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
using M = Doroti.Framework.Material;
using Image = Doroti.Framework.Widgets.Image;
using TextStyle = Doroti.Framework.Painting.TextStyle;

namespace MaterialSample;

internal static class SampleConstants
{
    internal static readonly string[] Destinations = ["Components", "Color", "Typography", "Elevation"];
    internal static readonly IconData[] DestinationIcons = [M.Icons.widgets_outlined, M.Icons.format_paint_outlined, M.Icons.text_snippet_outlined, M.Icons.invert_colors_on_outlined];
    internal static readonly IconData[] SelectedDestinationIcons = [M.Icons.widgets, M.Icons.format_paint, M.Icons.text_snippet, M.Icons.opacity];
    internal static readonly (string Label, Color Color)[] Seeds =
    [
        ("M3 Baseline", new(0xff6750a4)), ("Indigo", new(0xff3f51b5)), ("Blue", new(0xff2196f3)),
        ("Teal", new(0xff009688)), ("Green", new(0xff4caf50)), ("Yellow", new(0xffffeb3b)),
        ("Orange", new(0xffff9800)), ("Deep Orange", new(0xffff5722)), ("Pink", new(0xffe91e63)),
    ];
    internal static readonly string[] Images = ["Leaves", "Peonies", "Bubbles", "Seaweed", "Sea Grapes", "Petals"];
    internal static string ImageUrl(int index) => $"https://flutter.github.io/assets-for-api-docs/assets/material/content_based_color_scheme_{index + 1}.png";
    internal static List<Widget> BarDestinations() => Destinations.Select((label, i) => (Widget)new M.NavigationDestination(icon: new Icon(DestinationIcons[i]), selectedIcon: new Icon(SelectedDestinationIcons[i]), tooltip: "", label: label)).ToList();
}

internal sealed class SampleApp(bool acrylicAvailable) : StatefulWidget
{
    internal bool AcrylicAvailable => acrylicAvailable;
    public override IState createState() => new SampleAppState();
}

internal sealed class SampleAppState : State<SampleApp>
{
    private M.ThemeMode _mode = M.ThemeMode.system;
    private int _seed, _image, _revision;
    private bool _fromImage, _loading;
    private bool _acrylic = OperatingSystem.IsMacOS();
    private string? _error;
    private M.ColorScheme? _imageScheme;
    private M.ThemeData? _light;
    private M.ThemeData? _dark;

    private void UpdateThemes()
    {
        _light = null;
        _dark = null;
    }
    private M.ThemeData WindowTheme(M.ThemeData theme) => _acrylic
        ? theme.copyWith(scaffoldBackgroundColor: theme.colorScheme.surface.withAlpha(153))
        : theme;
    private M.ThemeData LightTheme() => _light ??= WindowTheme(_fromImage
        ? M.ThemeData.Create(fontFamilyFallback: ["Roboto"], colorScheme: _imageScheme)
        : M.ThemeData.Create(fontFamilyFallback: ["Roboto"], colorSchemeSeed: SampleConstants.Seeds[_seed].Color));
    private M.ThemeData DarkTheme() => _dark ??= WindowTheme(M.ThemeData.Create(fontFamilyFallback: ["Roboto"],
        colorSchemeSeed: _fromImage ? _imageScheme!.primary : SampleConstants.Seeds[_seed].Color, brightness: Brightness.dark));
    private void ToggleAcrylic()
    {
        if (!widget.AcrylicAvailable) return;
        setState(() => { _acrylic = !_acrylic; UpdateThemes(); });
    }
    private void SelectSeed(int value) => setState(() =>
    {
        _revision++; _seed = value; _fromImage = false; _loading = false; _error = null; UpdateThemes();
    });
    private async void SelectImage(int value)
    {
        var revision = ++_revision;
        setState(() => { _image = value; _loading = true; _error = null; });
        try
        {
            var scheme = await M.ColorScheme.fromImageProvider(provider: new NetworkImageIo(SampleConstants.ImageUrl(value)));
            if (!mounted || revision != _revision) return;
            setState(() => { _imageScheme = scheme; _fromImage = true; _loading = false; UpdateThemes(); });
        }
        catch (Exception exception)
        {
            if (mounted && revision == _revision) setState(() => { _loading = false; _error = $"Could not load {SampleConstants.Images[value]}: {exception.Message}"; });
        }
    }
    public override void dispose() { _revision++; base.dispose(); }
    public override Widget build(BuildContext context) => new M.MaterialApp(
        title: "Doroti Material 3", debugShowCheckedModeBanner: false,
        locale: new Doroti.Ui.Locale("en", "US"), themeFactory: LightTheme, darkThemeFactory: DarkTheme, themeMode: _mode,
        home: new SampleHome(_seed, _image, _fromImage, _loading, _error,
            () => setState(() => _mode = (_mode == M.ThemeMode.dark || (_mode == M.ThemeMode.system && View.of(context).platformDispatcher.platformBrightness == Brightness.dark)) ? M.ThemeMode.light : M.ThemeMode.dark),
            SelectSeed, SelectImage, _acrylic, widget.AcrylicAvailable ? ToggleAcrylic : null));
}

internal sealed class SampleHome(int seed, int image, bool fromImage, bool loading, string? error,
    Action brightness, System.Action<int> selectSeed, System.Action<int> selectImage,
    bool acrylic = false, Action? toggleAcrylic = null) : StatefulWidget
{
    internal int Seed => seed;
    internal int Image => image;
    internal bool FromImage => fromImage;
    internal bool Loading => loading;
    internal string? Error => error;
    internal Action Brightness => brightness;
    internal bool Acrylic => acrylic;
    internal Action? ToggleAcrylic => toggleAcrylic;
    internal System.Action<int> SelectSeed => selectSeed;
    internal System.Action<int> SelectImage => selectImage;
    public override IState createState() => new SampleHomeState();
}

internal sealed class SampleHomeState : State<SampleHome>, Doroti.Framework.Scheduler.TickerProvider
{
    private Doroti.Framework.Scheduler.Ticker? _ticker;
    private ValueListenable<TickerModeData>? _tickerMode;
    public Doroti.Framework.Scheduler.Ticker createTicker(System.Action<Duration> onTick)
    {
        if (_ticker is not null) throw new InvalidOperationException("Home owns one navigation ticker.");
        _ticker = new Doroti.Framework.Scheduler.Ticker(onTick);
        UpdateTickerMode();
        return _ticker;
    }
    private void UpdateTicker() { if (_ticker is not null && _tickerMode is not null) { _ticker.muted = !_tickerMode.value.enabled; _ticker.forceFrames = _tickerMode.value.forceFrames; } }
    private void UpdateTickerMode()
    {
        var notifier = TickerMode.getValuesNotifier(context);
        if (!ReferenceEquals(notifier, _tickerMode)) { _tickerMode?.removeListener(UpdateTicker); _tickerMode = notifier; notifier.addListener(UpdateTicker); }
        UpdateTicker();
    }
    public override void activate() { base.activate(); UpdateTickerMode(); }
    private readonly GlobalKey<M.ScaffoldState> _scaffold = new();
    private readonly GlobalKey<ComponentsState> _components = new();
    private AnimationController _controller = null!;
    private CurvedAnimation _rail = null!, _barCurve = null!, _railSize = null!, _railOffset = null!, _barSize = null!, _barOffset = null!;
    private ReverseAnimation _bar = null!;
    private bool _initialized, _wide, _extended;
    private int _destination;
    public override void initState()
    {
        base.initState();
        _controller = new AnimationController(duration: new Duration(1_000_000L), vsync: this);
        _rail = new CurvedAnimation(parent: _controller, curve: new Interval(0.5, 1));
        _barCurve = new CurvedAnimation(parent: _controller, curve: new Interval(0, 0.5));
        _bar = new ReverseAnimation(_barCurve);
        _railSize = SizeAnimation(_rail); _railOffset = OffsetAnimation(_rail);
        _barSize = SizeAnimation(_bar); _barOffset = OffsetAnimation(_bar);
    }
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        var width = MediaQuery.widthOf(context);
        _wide = width > 1000; _extended = width > 1500;
        if (!_initialized) { _initialized = true; _controller.value = _wide ? 1 : 0; }
        else if (_wide && _controller.status is not (AnimationStatus.forward or AnimationStatus.completed)) _controller.forward();
        else if (!_wide && _controller.status is not (AnimationStatus.reverse or AnimationStatus.dismissed)) _controller.reverse();
    }
    private static CurvedAnimation SizeAnimation(Animation<double> parent) => new(parent, new Interval(0.2, 0.8, curve: Curves.easeInOutCubicEmphasized),
        reverseCurve: new Interval(0, 0.2, curve: Curves.easeInOutCubicEmphasized.flipped));
    private static CurvedAnimation OffsetAnimation(Animation<double> parent) => new(parent, new Interval(0.4, 1, curve: Curves.easeInOutCubicEmphasized),
        reverseCurve: new Interval(0, 0.2, curve: Curves.easeInOutCubicEmphasized.flipped));
    public override void dispose()
    {
        _railSize.dispose(); _railOffset.dispose(); _barSize.dispose(); _barOffset.dispose(); _barCurve.dispose(); _rail.dispose();
        _controller.dispose(); _tickerMode?.removeListener(UpdateTicker); base.dispose();
    }
    private void Navigate(long value) => setState(() => _destination = checked((int)value));
    private bool AcceptAppBarScroll(ScrollNotification notification) =>
        Scroll_notificationLibrary.defaultScrollNotificationPredicate(notification) &&
        (_destination != 0 || _components.currentState?.OwnsScrollNotification(notification) == true);
    private Widget BrightnessAction() => new M.IconButton(tooltip: "Toggle brightness", onPressed: widget.Brightness, icon: new Icon(M.Theme.of(context).brightness == Brightness.light ? M.Icons.dark_mode_outlined : M.Icons.light_mode_outlined));
    private string AcrylicTooltip => widget.ToggleAcrylic is null ? "Window effects are unavailable on this platform"
        : $"Turn {(widget.Acrylic ? "off" : "on")} {App.WindowEffectLabel}";
    private Widget AcrylicAction() => new M.IconButton(tooltip: AcrylicTooltip,
        onPressed: widget.ToggleAcrylic, isSelected: widget.Acrylic,
        icon: new Icon(M.Icons.blur_off), selectedIcon: new Icon(M.Icons.blur_on));
    private Widget SeedAction() => new M.PopupMenuButton<int>(tooltip: "Select a seed color", icon: new Icon(M.Icons.palette_outlined),
        shape: new RoundedRectangleBorder(borderRadius: BorderRadius.CreateCircular(10)), onSelected: widget.SelectSeed,
        itemBuilder: _ => SampleConstants.Seeds.Select((seed, i) => (M.PopupMenuEntry<int>)new M.PopupMenuItem<int>(value: i,
            enabled: widget.FromImage || widget.Seed != i, child: new Wrap(children: [
                new Padding(padding: EdgeInsets.CreateOnly(left: 10), child: new Icon(!widget.FromImage && widget.Seed == i ? M.Icons.color_lens : M.Icons.color_lens_outlined, color: seed.Color)),
                new Padding(padding: EdgeInsets.CreateOnly(left: 20), child: new Text(seed.Label))]))).ToList());
    private Widget ImageAction() => new M.PopupMenuButton<int>(tooltip: "Select a color extraction image", icon: new Icon(M.Icons.image_outlined),
        shape: new RoundedRectangleBorder(borderRadius: BorderRadius.CreateCircular(10)), onSelected: widget.SelectImage,
        itemBuilder: _ => SampleConstants.Images.Select((label, i) => (M.PopupMenuEntry<int>)new M.PopupMenuItem<int>(value: i,
            enabled: !widget.FromImage || widget.Image != i, child: new Wrap(crossAxisAlignment: WrapCrossAlignment.center, children: [
                new Padding(padding: EdgeInsets.CreateOnly(left: 10), child: new ConstrainedBox(constraints: new BoxConstraints(maxWidth: 48),
                    child: new Padding(padding: EdgeInsets.CreateAll(4), child: new ClipRRect(borderRadius: BorderRadius.CreateCircular(8), child: ThemeImage(i))))),
                new Padding(padding: EdgeInsets.CreateOnly(left: 20), child: new Text(label))]))).ToList());
    private static Widget ThemeImage(int i) => Image.CreateNetwork(SampleConstants.ImageUrl(i), errorBuilder: (_, _, _) => new Icon(M.Icons.broken_image));
    private Widget ImageTile(int i) => new Semantics(selected: widget.FromImage && widget.Image == i, child: new M.Tooltip(message: SampleConstants.Images[i],
        child: new M.InkWell(borderRadius: BorderRadius.CreateCircular(4), onTap: widget.FromImage && widget.Image == i ? null : () => widget.SelectImage(i),
            child: new Padding(padding: EdgeInsets.CreateAll(8), child: new M.Material(borderRadius: BorderRadius.CreateCircular(4),
                elevation: widget.FromImage && widget.Image == i ? 0 : 3, child: new Padding(padding: EdgeInsets.CreateAll(4),
                    child: new ClipRRect(borderRadius: BorderRadius.CreateCircular(4), child: ThemeImage(i))))))));
    private Widget Settings()
    {
        Widget body = new Container(width: 250, padding: EdgeInsets.CreateSymmetric(horizontal: 30), child: new Column(
            mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.stretch, children:
            [
                new Row(children: [new Text("Brightness"), new Expanded(child: SizedBox.CreateShrink()),
                    new M.Switch(value: M.Theme.of(context).brightness == Brightness.light, onChanged: _ => widget.Brightness())]),
                new M.Tooltip(message: AcrylicTooltip, child: new Row(children:
                    [new Text(App.WindowEffectLabel), new Expanded(child: SizedBox.CreateShrink()),
                        new M.Switch(value: widget.Acrylic, onChanged: widget.ToggleAcrylic is null ? null : _ => widget.ToggleAcrylic())])),
                new M.Divider(),
                new ConstrainedBox(constraints: new BoxConstraints(maxHeight: 200), child: GridView.CreateCount(crossAxisCount: 3, primary: false, children:
                    SampleConstants.Seeds.Select((seed, i) => (Widget)new M.IconButton(tooltip: seed.Label, color: seed.Color,
                        icon: new Icon(M.Icons.radio_button_unchecked), selectedIcon: new Icon(M.Icons.circle),
                        isSelected: !widget.FromImage && widget.Seed == i, onPressed: () => widget.SelectSeed(i))).ToList())),
                new M.Divider(),
                new ConstrainedBox(constraints: new BoxConstraints(maxHeight: 150), child: new Padding(padding: EdgeInsets.CreateSymmetric(vertical: 8),
                    child: GridView.CreateCount(crossAxisCount: 3, primary: false, children: Enumerable.Range(0, SampleConstants.Images.Length).Select(ImageTile).ToList()))),
            ]));
        return new Align(alignment: Alignment.bottomCenter,
            child: MediaQuery.heightOf(context) > 740 ? body : new SingleChildScrollView(child: body));
    }
    private Widget? _home;
    private SampleHome? _homeWidget;
    private M.ThemeData? _homeTheme;
    private bool _homeWide, _homeExtended, _homeShort;
    private TextDirection _homeDirection;
    private int _homeDestination;
    public override Widget build(BuildContext context)
    {
        var theme = M.Theme.of(context);
        var shortWindow = MediaQuery.heightOf(context) <= 740;
        var direction = Directionality.of(context);
        // Pixel-size changes still flow through render constraints. Rebuild the
        // navigation configuration only when its actual inputs change; its
        // AnimatedBuilder independently listens to breakpoint transitions.
        if (_home is null || !ReferenceEquals(_homeWidget, widget) || !Equals(_homeTheme, theme) ||
            _homeWide != _wide || _homeExtended != _extended || _homeShort != shortWindow || _homeDestination != _destination || _homeDirection != direction)
        {
            _homeWidget = widget; _homeTheme = theme;
            _homeWide = _wide; _homeExtended = _extended; _homeShort = shortWindow; _homeDestination = _destination;
            _homeDirection = direction;
            _home = BuildAnimatedHome();
        }
        return _home;
    }
    private Widget BuildAnimatedHome()
    {
        Widget body = _destination switch
        {
            0 => new ComponentsScreen(twoColumns: _wide, scaffold: _scaffold, key: _components),
            1 => new ColorScreen(), 2 => new TypographyScreen(), _ => new ElevationScreen(),
        };
        return new M.Scaffold(key: _scaffold,
            appBar: new M.AppBar(title: new Text("Doroti Material 3"), notificationPredicate: AcceptAppBarScroll,
                backgroundColor: widget.Acrylic ? M.Colors.transparent : null,
                surfaceTintColor: widget.Acrylic ? M.Colors.transparent : null,
                scrolledUnderElevation: widget.Acrylic ? 0 : null,
                actions: !_wide ? [BrightnessAction(), AcrylicAction(), SeedAction(), ImageAction()] : [new Container()]),
            endDrawer: new GalleryDrawer(),
            body: new Column(children:
            [
                ifLoading(),
                new Expanded(child: new Row(crossAxisAlignment: CrossAxisAlignment.stretch, children:
                [
                    Rail(), new Expanded(child: body),
                ])),
            ]),
            bottomNavigationBar: new AnimatedBuilder(animation: _controller,
                child: new M.NavigationBar(selectedIndex: _destination, destinations: SampleConstants.BarDestinations(), onDestinationSelected: Navigate,
                    backgroundColor: widget.Acrylic ? M.Colors.transparent : null,
                    surfaceTintColor: widget.Acrylic ? M.Colors.transparent : null),
                builder: (_, child) => new ExcludeSemantics(excluding: _barSize.value <= 0,
                    child: new ClipRect(child: new Align(alignment: Alignment.topLeft, heightFactor: _barSize.value,
                        child: new FractionalTranslation(translation: new Offset(0, 1 - _barOffset.value), child: child))))));
    }
    private Widget Rail()
    {
        var direction = Directionality.of(context) == TextDirection.ltr ? 1 : -1;
        return new AnimatedBuilder(animation: _controller,
            child: new M.NavigationRail(extended: _extended, selectedIndex: _destination,
                backgroundColor: widget.Acrylic ? M.Colors.transparent : null,
                onDestinationSelected: Navigate, destinations: SampleConstants.Destinations.Select((label, i) =>
                    new M.NavigationRailDestination(icon: new Icon(SampleConstants.DestinationIcons[i]), selectedIcon: new Icon(SampleConstants.SelectedDestinationIcons[i]), label: new Text(label))).ToList(),
                trailing: new Expanded(child: new Padding(padding: EdgeInsets.CreateOnly(bottom: 20), child: _extended ? Settings() :
                    new Column(mainAxisAlignment: MainAxisAlignment.end, children: [new Flexible(child: BrightnessAction()), new Flexible(child: AcrylicAction()), new Flexible(child: SeedAction()), new Flexible(child: ImageAction())])))),
            builder: (_, child) => new ExcludeSemantics(excluding: _railSize.value <= 0,
                child: new ClipRect(child: new Align(alignment: Alignment.topLeft, widthFactor: _railSize.value,
                    child: new FractionalTranslation(translation: new Offset((_railOffset.value - 1) * direction, 0), child: child)))));
    }
    private Widget ifLoading() => widget.Loading ? new Column(mainAxisSize: MainAxisSize.min, children:
        [new M.LinearProgressIndicator(), new Text($"Loading {SampleConstants.Images[widget.Image]}…")]) : widget.Error is { } error
        ? new M.ListTile(title: new Text(error), trailing: new M.TextButton(onPressed: () => widget.SelectImage(widget.Image), child: new Text("Retry image"))) : SizedBox.CreateShrink();
}
