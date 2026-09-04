using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Material = Doroti.Framework.Material;
using ListView = Doroti.Framework.Widgets.ListView;
using Locale = Doroti.Ui.Locale;
using Rect = Doroti.Ui.Rect;
using Semantics = Doroti.Framework.Widgets.Semantics;
using Size = Doroti.Ui.Size;
using UiColor = Doroti.Ui.Color;

internal static class DemoTheme
{
    private static readonly UiColor Seed = new(0xff6750a4L);

    internal static Material.ThemeData Create(
        Brightness brightness, bool revealSystemBackdrop = false)
    {
        var isDark = brightness == Brightness.dark;
        var surfaceAlpha = revealSystemBackdrop ? 0x99L : 0xccL;
        var palette = Material.ColorScheme.CreateFromSeed(
            seedColor: Seed,
            brightness: brightness,
            surface: new UiColor((surfaceAlpha << 24) |
                (isDark ? 0x00141218L : 0x00fffbfeL)),
            surfaceContainer: new UiColor(isDark ? 0xff211f26L : 0xfff3edf7L),
            surfaceContainerHigh: new UiColor(isDark ? 0xff2b2930L : 0xffece6f0L),
            outline: new UiColor(isDark ? 0xff938f99L : 0xff79747eL));
        return Material.ThemeData.Create(
            useMaterial3: true,
            colorScheme: palette,
            scaffoldBackgroundColor: palette.surface);
    }
}

internal sealed class MaterialDemoEntrypoint(DemoEntryMode entryMode, bool requireExternalUia) : IDorotiViewEntrypoint
{
    private WidgetsFlutterBinding? _binding;
    private DorotiView? _view;

    internal Material.Scaffold? RootScaffold { get; private set; }
    internal MaterialGalleryState? GalleryState { get; private set; }
    internal Widget RootApp => _rootApp ??= CreateRootApp();
    internal DemoEntryMode EntryMode { get; } = entryMode;
    internal bool RequireExternalUia { get; } = requireExternalUia;
#if DOROTI_LEGACY_DESKTOP
    internal DesktopFrameworkPixelReadback? InitialReadback { get; set; }
    internal DesktopFrameworkPixelReadback? ChangedReadback { get; set; }
    internal DesktopFrameworkPixelReadback? BackdropOnReadback { get; set; }
    internal DesktopFrameworkPixelReadback? BackdropOffReadback { get; set; }
#endif
    internal string? InitialStateSignature { get; set; }
    internal string? ChangedStateSignature { get; set; }
    internal long CadencePresented { get; set; }
    internal TimeSpan CadenceDuration { get; set; }
    internal int NativePointerInteractionCount { get; set; }
    internal IReadOnlyList<string> NativePointerHitTestTargets { get; set; } = [];
    internal Offset? NativeEffectTogglePoint { get; set; }
    internal Rect? NativeEffectPanelBounds { get; set; }
    internal IReadOnlyList<string> NativeEffectHitTestTargets { get; set; } = [];

    private Material.MaterialApp? _rootApp;

    internal FlutterErrorDetails? FirstFrameworkError { get; private set; }

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        FlutterError.onError = details =>
        {
            FirstFrameworkError ??= details;
            Console.Error.WriteLine(details.exceptionThrown);
        };
        _binding = new WidgetsFlutterBinding(dispatcher);
    }

    public void AttachView(DorotiView view)
    {
        if (_binding is null)
        {
            throw new InvalidOperationException("The Material framework binding was not bootstrapped.");
        }
        if (_view is not null)
        {
            throw new InvalidOperationException("DorotiDemoApp owns exactly one Doroti view.");
        }

        _view = view;
        _binding.scheduleFrameCallback(_ =>
        {
            _binding.attachRootWidget(_binding.wrapWithDefaultView(RootApp));
        });
    }

    public void DetachView(DorotiView view)
    {
        if (ReferenceEquals(_view, view))
        {
            _view = null;
        }
    }

    internal void ExerciseAll() =>
        (GalleryState ?? throw new InvalidOperationException("The Material gallery State is not mounted.")).ExerciseAll();

    internal void RequestFrame()
    {
        if (GalleryState is { } galleryState)
        {
            galleryState.PulseFrame();
            return;
        }
        (_binding ?? throw new InvalidOperationException("The Material binding is not initialized.")).scheduleFrame();
    }

    internal IReadOnlyList<string> HitTestTargetsAt(double x, double y)
    {
        var binding = _binding ?? throw new InvalidOperationException("The Material binding is not initialized.");
        var result = new Doroti.Framework.Gestures.HitTestResult();
        binding.hitTestInView(
            result,
            new Offset(x, y),
            checked((long)(_view ?? throw new InvalidOperationException("The Doroti view is not attached.")).viewId));
        return result.path.Select(entry => entry.target.GetType().FullName ?? entry.target.GetType().Name).ToArray();
    }

    internal Rect BackdropPanelPhysicalBounds()
    {
        var view = _view ?? throw new InvalidOperationException("The Doroti view is not attached.");
        var logical = (GalleryState ?? throw new InvalidOperationException("The Material gallery State is not mounted."))
            .BackdropPanelBounds();
        var scale = view.devicePixelRatio;
        return new Rect(logical.left * scale, logical.top * scale, logical.right * scale, logical.bottom * scale);
    }

    public void Shutdown()
    {
        _binding?.Dispose();
        _binding = null;
        _view = null;
        FlutterError.onError = null;
    }

    private Material.MaterialApp CreateRootApp()
    {
        Widget Gallery() => new MaterialGallery(
                state => GalleryState = state,
                scaffold => RootScaffold = scaffold);

        return EntryMode == DemoEntryMode.Builder
            ? new Material.MaterialApp(
                title: "Doroti Material Demo",
                color: new UiColor(0xff6750a4L),
                themeFactory: () => DemoTheme.Create(
                    Brightness.light, App.ExperimentalAcrylicEnabled),
                darkThemeFactory: () => DemoTheme.Create(
                    Brightness.dark, App.ExperimentalAcrylicEnabled),
                themeMode: Material.ThemeMode.system,
                locale: new Locale("en", "US"),
                debugShowCheckedModeBanner: false,
                builder: (_, _) => new Overlay(initialEntries:
                [
                    new OverlayEntry(builder: _ => Gallery()),
                ]))
            : new Material.MaterialApp(
                title: "Doroti Material Demo",
                color: new UiColor(0xff6750a4L),
                themeFactory: () => DemoTheme.Create(
                    Brightness.light, App.ExperimentalAcrylicEnabled),
                darkThemeFactory: () => DemoTheme.Create(
                    Brightness.dark, App.ExperimentalAcrylicEnabled),
                themeMode: Material.ThemeMode.system,
                locale: new Locale("en", "US"),
                debugShowCheckedModeBanner: false,
                home: Gallery());
    }
}

internal sealed class MaterialGallery(
    System.Action<MaterialGalleryState> mounted,
    System.Action<Material.Scaffold> scaffoldBuilt) : StatefulWidget
{
    internal System.Action<MaterialGalleryState> Mounted { get; } = mounted;
    internal System.Action<Material.Scaffold> ScaffoldBuilt { get; } = scaffoldBuilt;
    public override IState createState() => new MaterialGalleryState();
}

internal sealed class MaterialGalleryState : State<MaterialGallery>
{
    private const string GalleryShaderSource = """
        uniform float2 uSize;
        uniform float uPhase;

        half4 main(float2 position) {
            float2 uv = position / max(uSize, float2(1.0));
            return half4(uv.x, 0.35 + 0.35 * sin(uPhase + uv.x * 6.2831853), uv.y, 1.0);
        }
        """;

    private const string GalleryFilterShaderSource = """
        uniform float2 uSize;
        uniform float uPhase;
        uniform shader uInput;

        half4 main(float2 position) {
            half4 inputColor = uInput.eval(position);
            float wave = 0.82 + 0.18 * sin(uPhase + position.x / max(uSize.x, 1.0) * 6.2831853);
            return half4(inputColor.r * wave, inputColor.g, inputColor.b / max(wave, 0.01), inputColor.a);
        }
        """;

    internal static readonly string[] InteractiveLabels =
    [
        "G6 Material button", "G6 Material checkbox", "G6 Material radio",
        "G6 Material switch", "G6 Material slider", "G6 Material FAB",
    ];

    private int _buttonCount;
    private bool _checked;
    private long _radio;
    private bool _switched;
    private double _slider = 0.2;
    private int _fabCount;
    private string _textValue = string.Empty;
    private bool _blurEnabled = true;
    private readonly TextEditingController _textController = new();
    private readonly FocusNode _textFocusNode = new();
    private readonly ScrollController _outerScrollController = new();
    private readonly ScrollController _innerScrollController = new();
    private readonly FragmentShader _galleryShader =
        FragmentProgram.fromSource(GalleryShaderSource, "doroti-demo-gallery").fragmentShader();
    private readonly FragmentShader _galleryFilterShader =
        FragmentProgram.fromSource(GalleryFilterShaderSource, "doroti-demo-image-filter").fragmentShader();
    private readonly GlobalKey<IState> _blurToggleKey = new("g6-backdrop-blur-toggle");
    private readonly GlobalKey<IState> _backdropPanelKey = new("g6-backdrop-blur-panel");
    private readonly GlobalKey<IState> _outerScrollbarKey = new("fcr5-outer-scrollbar");
    private readonly GlobalKey<IState> _innerScrollbarKey = new("fcr5-inner-scrollbar");

    internal int InteractionCount { get; private set; }
    internal int EffectInteractionCount { get; private set; }
    internal bool BlurEnabled => _blurEnabled;
    internal int BuildCount { get; private set; }
    internal string StateSignature =>
        $"button={_buttonCount};checked={_checked};radio={_radio};switch={_switched};slider={_slider:F1};fab={_fabCount}";
    internal string EffectStateSignature => $"backdropBlur={_blurEnabled};effectInteractions={EffectInteractionCount}";

    public override void initState()
    {
        base.initState();
        widget.Mounted(this);
    }

    public override void dispose()
    {
        _textFocusNode.dispose();
        _textController.dispose();
        _innerScrollController.dispose();
        _outerScrollController.dispose();
        base.dispose();
    }

    internal void ExerciseAll() => setState(() =>
    {
        _buttonCount++;
        _checked = !_checked;
        _radio = _radio == 1 ? 0 : 1;
        _switched = !_switched;
        _slider = _slider < 0.7 ? 0.8 : 0.2;
        _fabCount++;
        InteractionCount += InteractiveLabels.Length;
    });

    internal void PulseFrame() => setState(() => { });

    internal Offset BlurToggleCenter()
    {
        var context = _blurToggleKey.currentContext ?? throw new InvalidOperationException("Backdrop blur toggle is not mounted.");
        var box = context.findRenderObject() as Doroti.Framework.Rendering.RenderBox
            ?? throw new InvalidOperationException("Backdrop blur toggle does not own a RenderBox.");
        return box.localToGlobal(box.size.center(Offset.zero));
    }

    internal Rect BackdropPanelBounds()
    {
        var context = _backdropPanelKey.currentContext ?? throw new InvalidOperationException("Backdrop panel is not mounted.");
        var box = context.findRenderObject() as Doroti.Framework.Rendering.RenderBox
            ?? throw new InvalidOperationException("Backdrop panel does not own a RenderBox.");
        var origin = box.localToGlobal(Offset.zero);
        return Rect.fromLTWH(origin.dx, origin.dy, box.size.width, box.size.height);
    }

    private void ToggleBlur() => setState(() =>
    {
        _blurEnabled = !_blurEnabled;
        EffectInteractionCount++;
    });

    private void Mutate(System.Action mutation) => setState(() =>
    {
        mutation();
        InteractionCount++;
    });

    private Widget ActionSemantics(string label, Widget child, System.Action action, string value) => new Semantics(
        container: true,
        excludeSemantics: true,
        identifier: label.Replace(' ', '-').ToLowerInvariant(),
        label: label,
        value: value,
        focusable: true,
        button: label is "G6 Material button" or "G6 Material FAB",
        @checked: label == "G6 Material checkbox" ? _checked : null,
        selected: label == "G6 Material radio" ? _radio == 1 : null,
        inMutuallyExclusiveGroup: label == "G6 Material radio" ? true : null,
        toggled: label == "G6 Material switch" ? _switched : null,
        slider: label == "G6 Material slider" ? true : null,
        increasedValue: label == "G6 Material slider"
            ? Math.Min(1, _slider + 0.1).ToString("F1", System.Globalization.CultureInfo.InvariantCulture)
            : null,
        decreasedValue: label == "G6 Material slider"
            ? Math.Max(0, _slider - 0.1).ToString("F1", System.Globalization.CultureInfo.InvariantCulture)
            : null,
        minValue: label == "G6 Material slider" ? "0" : null,
        maxValue: label == "G6 Material slider" ? "1" : null,
        onIncrease: label == "G6 Material slider"
            ? () => Mutate(() => _slider = Math.Min(1, _slider + 0.1))
            : null,
        onDecrease: label == "G6 Material slider"
            ? () => Mutate(() => _slider = Math.Max(0, _slider - 0.1))
            : null,
        onTap: () => Mutate(action),
        child: child);

    public override Widget build(BuildContext context)
    {
        BuildCount++;
        var palette = Material.Theme.of(context).colorScheme;
        var interactionOverlay = palette.primary.withOpacity(0.13);
        _galleryFilterShader.setFloat(2, _slider * 6.2831853);
        var button = ActionSemantics(InteractiveLabels[0], new Material.ElevatedButton(
            onPressed: () => Mutate(() => _buttonCount++),
            child: new Text("Press button")), () => _buttonCount++, _buttonCount.ToString());
        var checkbox = ActionSemantics(InteractiveLabels[1], new Material.Checkbox(
            value: _checked,
            semanticLabel: "Gallery checkbox",
            activeColor: palette.primary,
            fillColor: new WidgetStatePropertyAll<UiColor?>(palette.primary),
            overlayColor: new WidgetStatePropertyAll<UiColor?>(interactionOverlay),
            checkColor: palette.onPrimary,
            focusColor: interactionOverlay,
            hoverColor: interactionOverlay,
            splashRadius: 20,
            side: new BorderSide(color: palette.outline, width: 2),
            shape: new RoundedRectangleBorder(),
            materialTapTargetSize: Material.MaterialTapTargetSize.padded,
            visualDensity: Material.VisualDensity.standard,
            onChanged: value => Mutate(() => _checked = value == true)), () => _checked = !_checked, _checked.ToString());
        var radio = ActionSemantics(InteractiveLabels[2], new Material.Radio<long>(
            value: 1,
            groupValue: _radio,
            activeColor: palette.primary,
            fillColor: new WidgetStatePropertyAll<UiColor?>(palette.primary),
            backgroundColor: new WidgetStatePropertyAll<UiColor?>(new UiColor(0x00000000L)),
            overlayColor: new WidgetStatePropertyAll<UiColor?>(interactionOverlay),
            focusColor: interactionOverlay,
            hoverColor: interactionOverlay,
            splashRadius: 20,
            side: new BorderSide(color: palette.outline, width: 2),
            materialTapTargetSize: Material.MaterialTapTargetSize.padded,
            visualDensity: Material.VisualDensity.standard,
            onChanged: value => Mutate(() => _radio = value)), () => _radio = _radio == 1 ? 0 : 1, _radio.ToString());
        var toggle = ActionSemantics(InteractiveLabels[3], new Material.Switch(
            value: _switched,
            activeThumbColor: palette.primary,
            activeTrackColor: palette.primaryContainer,
            inactiveThumbColor: palette.onSurfaceVariant,
            inactiveTrackColor: palette.surfaceContainerHighest,
            thumbColor: new WidgetStatePropertyAll<UiColor?>(_switched ? palette.primary : palette.onSurfaceVariant),
            trackColor: new WidgetStatePropertyAll<UiColor?>(_switched ? palette.primaryContainer : palette.surfaceContainerHighest),
            trackOutlineColor: new WidgetStatePropertyAll<UiColor?>(palette.outline),
            trackOutlineWidth: new WidgetStatePropertyAll<double?>(1),
            overlayColor: new WidgetStatePropertyAll<UiColor?>(interactionOverlay),
            focusColor: interactionOverlay,
            hoverColor: interactionOverlay,
            splashRadius: 20,
            onChanged: value => Mutate(() => _switched = value)), () => _switched = !_switched, _switched.ToString());
        var slider = ActionSemantics(InteractiveLabels[4], new Material.Slider(
            value: _slider,
            min: 0,
            max: 1,
            divisions: 10,
            activeColor: palette.primary,
            inactiveColor: palette.surfaceContainerHighest,
            thumbColor: palette.primary,
            overlayColor: new WidgetStatePropertyAll<UiColor?>(interactionOverlay),
            showValueIndicator: Material.ShowValueIndicator.never,
            onChanged: value => Mutate(() => _slider = value)), () => _slider = _slider < 0.7 ? 0.8 : 0.2,
            _slider.ToString("F1", System.Globalization.CultureInfo.InvariantCulture));
        var textFieldStatus = _textValue.Length == 0
            ? "Enter text to test keyboard and IME input"
            : $"Entered: {_textValue}";
        var textField = new Material.TextField(
            controller: _textController,
            focusNode: _textFocusNode,
            decoration: new Material.InputDecoration(
                labelText: "Text field",
                hintText: "Type in English or 한국어",
                border: new Material.OutlineInputBorder()),
            onChanged: value => setState(() => _textValue = value));
        var textFieldArea = new Column(
            mainAxisSize: Doroti.Framework.Rendering.MainAxisSize.min,
            crossAxisAlignment: Doroti.Framework.Rendering.CrossAxisAlignment.stretch,
            spacing: 4,
            children:
            [
                textField,
                new Padding(
                    padding: EdgeInsets.CreateOnly(left: 16),
                    child: new Semantics(
                        identifier: "text-field-status",
                        label: textFieldStatus,
                        child: new ExcludeSemantics(child: new Text(
                            textFieldStatus,
                            style: new Doroti.Framework.Painting.TextStyle(
                                color: palette.onSurfaceVariant,
                                fontSize: 12))))),
            ]);

        var lazyList = ListView.CreateBuilder(
            controller: _innerScrollController,
            primary: false,
            itemCount: 24,
            itemExtent: 30,
            itemBuilder: (_, index) => new Container(
                color: index % 2 == 0 ? palette.primaryContainer : palette.tertiaryContainer,
                child: new Text($"Lazy item {index + 1}",
                    style: new Doroti.Framework.Painting.TextStyle(
                        color: index % 2 == 0 ? palette.onPrimaryContainer : palette.onTertiaryContainer))));
        var nestedInnerScroll = new Material.Scrollbar(
            key: _innerScrollbarKey,
            controller: _innerScrollController,
            child: lazyList);
        var blurToggle = new Semantics(
            key: _blurToggleKey,
            container: true,
            excludeSemantics: true,
            identifier: "g6-backdrop-blur-toggle",
            label: "G6 backdrop blur",
            value: _blurEnabled ? "on" : "off",
            toggled: _blurEnabled,
            child: new Material.ElevatedButton(
                onPressed: ToggleBlur,
                child: new Row(spacing: 6, children:
                [
                    new IgnorePointer(child: new Material.Checkbox(value: _blurEnabled, onChanged: _ => { })),
                    new Text(_blurEnabled ? "Blur ON" : "Blur OFF"),
                ])));
        var effectPanel = new SizedBox(height: 180, child: new Stack(
            children:
            [
                new Positioned(left: 0, top: 0, right: 0, height: 170, child: nestedInnerScroll),
                new Positioned(left: 64, top: 30, width: 560, height: 100, child: new SizedBox(
                    key: _backdropPanelKey,
                    width: 560,
                    height: 100,
                    child: new Stack(children:
                    [
                        new Positioned(left: 0, top: 0, right: 0, bottom: 0, child: new IgnorePointer(
                            child: new BackdropFilter(
                                filterConfig: Doroti.Framework.Rendering.ImageFilterConfig.CreateBlur(
                                    sigmaX: 12, sigmaY: 6, tileMode: TileMode.clamp, bounded: true),
                                enabled: _blurEnabled,
                                child: new Container(color: new UiColor(0x01ffffffL))))),
                        new Positioned(left: 0, top: 0, right: 0, bottom: 0, child: new IgnorePointer(
                            child: new Container(
                                color: palette.surface.withOpacity(0.33),
                                padding: EdgeInsets.CreateAll(12),
                                child: new Column(
                                    crossAxisAlignment: Doroti.Framework.Rendering.CrossAxisAlignment.start,
                                    children:
                                    [
                                        new Text("FROSTED GLASS · BACKDROP BLUR"),
                                        new Text("ListView rows continue behind this overlay"),
                                    ])))),
                    ]))),
            ]));

        var scaffold = new Material.Scaffold(
            backgroundColor: palette.surface,
            appBar: new Material.AppBar(
                title: new Text("Doroti Material Gallery"),
                backgroundColor: palette.primaryContainer,
                foregroundColor: palette.onPrimaryContainer,
                iconTheme: new IconThemeData(color: palette.onPrimaryContainer, size: 24),
                actionsIconTheme: new IconThemeData(color: palette.onPrimaryContainer, size: 24)),
            body: new Stack(children:
            [
                new Positioned(left: 0, top: 0, right: 0, bottom: 0,
                    child: new Material.Scrollbar(
                    key: _outerScrollbarKey,
                    controller: _outerScrollController,
                    child: new SingleChildScrollView(
                        controller: _outerScrollController,
                        primary: false,
                        child: new Container(
                        padding: EdgeInsets.CreateAll(16),
                        child: new Column(
                        crossAxisAlignment: Doroti.Framework.Rendering.CrossAxisAlignment.start,
                        spacing: 10,
                        children:
                        [
                            new Stack(children:
                            [
                                new Text("Custom SkSL · shared GPU runtime effect · all targets",
                                    style: new Doroti.Framework.Painting.TextStyle(color: palette.primary)),
                                new ExcludeSemantics(child: new ShaderMask(
                                    blendMode: Doroti.Ui.BlendMode.srcIn,
                                    shaderCallback: bounds =>
                                    {
                                        _galleryShader.setFloat(0, bounds.width);
                                        _galleryShader.setFloat(1, bounds.height);
                                        _galleryShader.setFloat(2, _slider * 6.2831853);
                                        return _galleryShader;
                                    },
                                    child: new Text("Custom SkSL · shared GPU runtime effect · all targets",
                                        style: new Doroti.Framework.Painting.TextStyle(color: palette.primary)))),
                            ]),
                            new ImageFiltered(
                                imageFilter: new ImageFilter(_galleryFilterShader, FilterQuality.low),
                                child: new Container(
                                    width: 340,
                                    height: 48,
                                    color: palette.primary,
                                    alignment: Alignment.center,
                                    child: new Text("ImageFilter.shader · implicit child texture · GPU 2-pass",
                                        style: new Doroti.Framework.Painting.TextStyle(color: palette.onPrimary)))),
                            new Text("Reviewed Material · promoted product · strict Skia GPU"),
                            new Material.Card(
                                color: palette.surfaceContainer,
                                child: new Material.ListTile(
                                    title: new Text("Material components"),
                                    subtitle: new Text("Card + ListTile + local state"))),
                            new Row(spacing: 12, children:
                            [
                                new Expanded(child: textFieldArea),
                                new Material.OutlinedButton(
                                    onPressed: () => _textFocusNode.unfocus(),
                                    child: new Text("Clear focus")),
                            ]),
                            new Row(spacing: 12, children: [button, new Text($"Pressed {_buttonCount}")]),
                            new Row(spacing: 12, children: [checkbox, new Text("Checkbox"), radio, new Text("Radio")]),
                            new Row(spacing: 12, children: [toggle, new Text("Switch")]),
                            slider,
                            new SizedBox(height: 64, child: new Stack(
                                alignment: Alignment.center,
                                children:
                                [
                                    new Container(width: 260, height: 56,
                                        color: _switched ? palette.primaryContainer : palette.errorContainer),
                                    new Text($"Stack state · {StateSignature}",
                                        style: new Doroti.Framework.Painting.TextStyle(
                                            color: _switched ? palette.onPrimaryContainer : palette.onErrorContainer)),
                                ])),
                            new Row(spacing: 8, children: [blurToggle, new Text("Backdrop blur (native effect gate)")]),
                            new Text("Nested scroll regression · outer/inner controllers · transient Android scrollbar"),
                            new Text("Lazy ListView.builder + clipped backdrop panel"),
                            effectPanel,
                        ]))))),
                new Positioned(left: 0, top: 0, right: 0, bottom: 0,
                    child: new IgnorePointer(child: new Stack(children:
                    [
                        // The magenta markers deliberately avoid the presenter's
                        // safe-background samples (x=8 and bottom-9). Their
                        // asymmetric shapes distinguish child-local origin from
                        // the moving screen origin and verify the right/bottom
                        // edge phase independently from the periodic grid.
                        new Positioned(left: 2, top: 3, width: 22, height: 3,
                            child: new Container(color: new UiColor(0xffff1744L))),
                        new Positioned(left: 2, top: 3, width: 3, height: 15,
                            child: new Container(color: new UiColor(0xffff1744L))),
                        new Positioned(right: 1, top: 7, width: 3, height: 19,
                            child: new Container(color: new UiColor(0xffff1744L))),
                        new Positioned(right: 1, bottom: 1, width: 27, height: 3,
                            child: new Container(color: new UiColor(0xffff1744L))),
                        new Positioned(right: 1, bottom: 1, width: 3, height: 13,
                            child: new Container(color: new UiColor(0xffff1744L))),
                    ]))),
            ]),
            floatingActionButton: ActionSemantics(InteractiveLabels[5], new Material.FloatingActionButton(
                tooltip: "Material action",
                backgroundColor: palette.primaryContainer,
                foregroundColor: palette.onPrimaryContainer,
                onPressed: () => Mutate(() => _fabCount++),
                child: new Text("+")), () => _fabCount++, _fabCount.ToString()));
        widget.ScaffoldBuilt(scaffold);
        return new Stack(children:
        [
            // R0 resize oracle: keep the 32-logical-pixel grid behind the
            // translucent Scaffold so it remains visible without covering the
            // gallery, app bar, controls, or floating action button.
            new Positioned(left: 0, top: 0, right: 0, bottom: 0,
                child: new IgnorePointer(child: new GridPaper(
                    color: new UiColor(0xff00e5ffL),
                    interval: 32,
                    divisions: 1,
                    subdivisions: 1))),
            new Positioned(left: 0, top: 0, right: 0, bottom: 0, child: scaffold),
        ]);
    }
}

internal enum DemoEntryMode { Builder, Home }

internal static class App
{
    internal static Func<IDorotiViewEntrypoint> Definition =>
        () => new MaterialDemoEntrypoint(DemoEntryMode.Home, requireExternalUia: false);

    internal static bool ExperimentalAcrylicEnabled => string.Equals(
        Environment.GetEnvironmentVariable("DOROTI_DEMO_EXPERIMENTAL_ACRYLIC"),
        "1", StringComparison.Ordinal);

    internal static DorotiViewConfiguration ViewConfiguration { get; } =
        new("Doroti Material Demo", new Size(720, 640),
            // The Scaffold already paints the 80%-opaque Material surface.
            // Keep the renderer base transparent in Acrylic mode so that tint
            // is applied once and the system-blurred backdrop is not hidden by
            // two stacked translucent fills.
            ExperimentalAcrylicEnabled
                ? new UiColor(0x00000000L) : new UiColor(0xccfffbfeL),
            ExperimentalAcrylicEnabled
                ? new UiColor(0x00000000L) : new UiColor(0xcc141218L),
            ExperimentalAcrylicEnabled
                ? new WindowBackdropOptions(
                    WindowBackdropMode.experimentalAcrylic,
                    WindowBackdropFallback.transparent,
                    WindowAcrylicKind.@default,
                    WindowBackdropTheme.system)
                : new WindowBackdropOptions(
                    WindowBackdropMode.acrylic,
                    WindowBackdropFallback.transparent),
            terminateAfterLastWindowClosed: true);
}
