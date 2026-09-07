using System.Reflection;
using Doroti.Framework.Animation;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using M = Doroti.Framework.Material;

internal static partial class MountedPickerContracts
{
    internal static void VerifySelectionControls(string output)
    {
        var queued = new List<System.Action>();
        using (Doroti.Runtime.DartAsyncRuntime.enterMicrotaskScheduler(queued.Add))
        {
            Doroti.Runtime.Future<int> typed = new SynchronousFuture<int>(4);
            Doroti.Runtime.Future untyped = typed;
            var typedResult = 0;
            var untypedResult = 0;
            typed.then(value => typedResult = value);
            untyped.then<int>(value => untypedResult = (int)value!);
            if (typedResult != 4 || untypedResult != 4 || queued.Count != 0)
                throw new InvalidOperationException("SynchronousFuture must complete inline through typed and untyped Future references.");
            var chained = 0;
            typed.then(value => value + 1).then(value => chained = value);
            if (chained != 5 || queued.Count != 0)
                throw new InvalidOperationException("SynchronousFuture chains must preserve inline completion.");
            var ordinaryValue = 0;
            var ordinary = Doroti.Runtime.Future<int>.value(7).then(value => ordinaryValue = value);
            if (ordinaryValue != 0 || queued.Count != 1)
                throw new InvalidOperationException("Ordinary Future must still queue its continuation.");
            queued[0]();
            if (ordinary.asTask().GetAwaiter().GetResult() != 7 || ordinaryValue != 7)
                throw new InvalidOperationException("Ordinary Future continuation did not complete through the scheduler.");
        }
        Console.WriteLine("Future dispatch: typed/untyped synchronous completion and chains; ordinary async completion PASS");
        Directory.CreateDirectory(output);
        using var platform = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
            [new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.windows));
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var host = new Host();
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null,
            "selection-controls", "selection-controls", "selection-controls");
        using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("selection-controls")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
            .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, host)
            .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host));
        renderer.RegisterFontAsync(File.ReadAllBytes("DorotiTestbedApp/assets/fonts/MaterialIcons-Regular.otf"), "MaterialIcons").GetAwaiter().GetResult();
        var binding = new WidgetsFlutterBinding(dispatcher);
        var errors = new List<FlutterErrorDetails>();
        var failures = new List<string>();
        var previousError = FlutterError.onError;
        FlutterError.onError = errors.Add;
        try
        {
            var ticks = new SelectionTicks();
            var rangeTicks = new SelectionRangeTicks();
            System.Action<System.Action>? rebuild = null;
            var direction = TextDirection.ltr;
            var dark = false;
            var enabled = true;
            var selected = true;
            long? divisions = 5;
            var sliderValue = .2;
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(
                new StatefulBuilder(builder: (_, change) =>
                {
                    rebuild = change;
                    return new M.MaterialApp(locale: new Locale("en", "US"), theme: M.ThemeData.Create(brightness: dark ? Brightness.dark : Brightness.light),
                        home: new Directionality(textDirection: direction, child: new M.Scaffold(body: new Center(
                            child: new SizedBox(width: 360, child: new M.SliderTheme(
                                data: new M.SliderThemeData(tickMarkShape: ticks, rangeTickMarkShape: rangeTicks),
                                child: new Column(mainAxisSize: MainAxisSize.min, children:
                                [
                                    new M.Slider(value: sliderValue, divisions: divisions, onChanged: enabled ? value => change(() => sliderValue = value) : null),
                                    new M.RangeSlider(values: new M.RangeValues(.2, .8), divisions: divisions, onChanged: enabled ? _ => { } : null),
                                    new M.Switch(value: selected, onChanged: enabled ? value => change(() => selected = value) : null,
                                        thumbIcon: WidgetStateProperty<Icon?>.resolveWith<Icon?>(states => new Icon(states.Contains(WidgetState.selected) ? M.Icons.check : M.Icons.close)))
                                ])))))));
                }))));
            foreach (var configuration in new[]
            {
                (false, TextDirection.ltr, true, true, (long?)5),
                (true, TextDirection.rtl, true, false, (long?)5),
                (false, TextDirection.rtl, false, true, (long?)5),
                (true, TextDirection.ltr, false, false, (long?)5),
                (false, TextDirection.ltr, true, true, (long?)null)
            })
            {
                ticks.Centers.Clear(); rangeTicks.Centers.Clear();
                view.DispatchPlatformEvent(() => rebuild!(() =>
                    (dark, direction, enabled, selected, divisions) = configuration));
                var name = $"{(dark ? "dark" : "light")}-{direction}-{enabled}-{selected}-{divisions}";
                Pump(name);
                CheckTicks(ticks.Centers, "slider " + name);
                CheckTicks(rangeTicks.Centers, "range slider " + name);
                foreach (var element in Elements(binding.rootElement!).Where(e => e.widget is M.Slider or M.RangeSlider))
                {
                    var render = Elements(element).Select(e => e.findRenderObject()).First(r =>
                        r?.GetType().Name is "_RenderSlider__slider" or "_RenderRangeSlider__range_slider")!;
                    foreach (var value in new[] { 0.0, .19, .41, .59, .81, 1.0 })
                    {
                        var actual = (double)render.GetType().GetMethod("_discretize", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(render, [value])!;
                        var expected = divisions is null ? value : Math.Round(value * 5, MidpointRounding.AwayFromZero) / 5.0;
                        Check(Math.Abs(actual - expected) < .001, $"{name} {render.GetType().Name} snap({value})={actual}, expected {expected}");
                    }
                    if (element.widget is M.Slider && divisions is not null)
                    {
                        var state = ((StatefulElement)element).state;
                        var actual = (double)state.GetType().GetMethod("_discretize", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(state, [.41])!;
                        Check(Math.Abs(actual - .4) < .001, $"{name} Slider state normalization={actual}, expected .4");
                    }
                }
                var switchState = Elements(binding.rootElement!).OfType<StatefulElement>()
                    .Single(e => e.widget.GetType().Name == "_MaterialSwitch__switch").state;
                var painter = switchState.GetType().GetProperty("_painter", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(switchState)!;
                var textPainter = (TextPainter)painter.GetType().GetProperty("_textPainter", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(painter)!;
                Check(Math.Abs(textPainter.width - 16) < .01 && Math.Abs(textPainter.height - 16) < .01,
                    $"{name} switch icon em box={textPainter.width}x{textPainter.height}, expected 16x16");
                Check(Math.Abs(textPainter.computeDistanceToActualBaseline(TextBaseline.alphabetic) - 16) < .01,
                    $"{name} switch icon baseline must match MaterialIcons ascent 16");
                var sliderElement = Elements(binding.rootElement!).Single(e => e.widget is M.Slider);
                var sliderBox = (RenderBox)Elements(sliderElement).Select(e => e.findRenderObject())
                    .First(r => r?.GetType().Name == "_RenderSlider__slider")!;
                var track = (Rect)sliderBox.GetType().GetProperty("_trackRect", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sliderBox)!;
                var oldSliderValue = sliderValue;
                var visualValue = direction == TextDirection.ltr ? .41 : .59;
                Tap(sliderBox.localToGlobal(new Offset(track.left + track.width * visualValue, track.center.dy)));
                Pump(name + "-slider-tap");
                var expectedSliderValue = enabled ? divisions is null ? .41 : .4 : oldSliderValue;
                Check(Math.Abs(sliderValue - expectedSliderValue) < .001, $"{name} pointer slider value={sliderValue}, expected {expectedSliderValue}");
                var switchElement = Elements(binding.rootElement!).Single(e => e.widget is M.Switch);
                var switchBox = (RenderBox)switchElement.findRenderObject()!;
                var oldSelected = selected;
                Tap(switchBox.localToGlobal(switchBox.size.center(Offset.zero)));
                Pump(name + "-switch-tap");
                Check(selected == (enabled ? !oldSelected : oldSelected), name + " switch pointer toggle/disabled guard");
                Console.WriteLine($"Selection controls inspected: {name}");
            }
            // Paragraph-level inheritance and run overrides must still work
            // when the default stops inventing a line-height multiplier.
            foreach (var (paragraphHeight, runHeight, expectedHeight) in new[]
            {
                ((double?)null, (double?)null, 16.0),
                ((double?)1.5, (double?)null, 24.0),
                ((double?)1.5, (double?)1.0, 16.0),
                ((double?)null, (double?)2.0, 32.0)
            })
            {
                var builder = new ParagraphBuilder(new ParagraphStyle(fontSize: 16, height: paragraphHeight));
                builder.pushStyle(new Doroti.Ui.TextStyle(fontFamily: "MaterialIcons", fontSize: 16, height: runHeight));
                builder.addText(char.ConvertFromUtf32((int)M.Icons.close.codePoint));
                using var paragraph = builder.build();
                paragraph.layout(new ParagraphConstraints(100));
                Check(Math.Abs(paragraph.height - expectedHeight) < .01,
                    $"paragraph height={paragraphHeight}, run={runHeight}: {paragraph.height}, expected {expectedHeight}");
            }
            Check(errors.Count == 0, string.Join("\n", errors.Select(e => e.exceptionThrown)));
            foreach (var failure in failures) Console.WriteLine("FAIL: " + failure);
            if (failures.Count > 0) throw new InvalidOperationException($"Selection controls: {failures.Count} failures");
            Console.WriteLine("Selection controls: evenly spaced ticks, continuous mode, state/render snapping, switch check/close em boxes and baselines, pointer input, enabled/disabled, light/dark, LTR/RTL, natural/explicit/inherited paragraph heights PASS");

            void Tap(Offset position)
            {
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new Doroti.Framework.Gestures.PointerDownEvent(
                    viewId: 1, pointer: 1, position: position, buttons: 1, timeStamp: Doroti.Runtime.Duration.Create(milliseconds: Environment.TickCount64))));
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new Doroti.Framework.Gestures.PointerUpEvent(
                    viewId: 1, pointer: 1, position: position, timeStamp: Doroti.Runtime.Duration.Create(milliseconds: Environment.TickCount64 + 20))));
            }

            void CheckTicks(List<Offset> centers, string label)
            {
                if (divisions is null) { Check(centers.Count == 0, label + " continuous track has ticks"); return; }
                var xs = centers.Select(p => p.dx).Distinct().Order().ToArray();
                Check(xs.Length == 6, $"{label} expected 6 distinct tick positions, got {xs.Length}");
                if (xs.Length == 6)
                    for (var i = 1; i < 6; i++) Check(Math.Abs(xs[i] - xs[0] - i * (xs[5] - xs[0]) / 5) < .001, label + " ticks not evenly spaced");
            }
            void Pump(string name)
            {
                for (var frame = 0; frame < 30; frame++) { host.Fire(); Thread.Sleep(10); }
                if (errors.Count > 0) throw new InvalidOperationException(string.Join("\n", errors.Select(e => e.exceptionThrown)));
                using var surface = SKSurface.Create(new SKImageInfo(Width, Height));
                if (renderer.Paint(surface, Width, Height) is { } completion)
                    renderer.CompletePaint(completion, DorotiFrameTerminal.submitted);
                using var snapshot = surface.Snapshot();
                using var png = snapshot.Encode(SKEncodedImageFormat.Png, 100);
                File.WriteAllBytes(System.IO.Path.Combine(output, name + ".png"), png.ToArray());
            }
            void Check(bool condition, string message) { if (!condition) failures.Add(message); }
        }
        finally { FlutterError.onError = previousError; }
    }

    private sealed class SelectionTicks : M.RoundSliderTickMarkShape
    {
        internal List<Offset> Centers { get; } = [];
        public override void paint(PaintingContext context, Offset center, RenderBox parentBox, M.SliderThemeData sliderTheme,
            Animation<double> enableAnimation, Offset thumbCenter, bool isEnabled, TextDirection textDirection)
        {
            Centers.Add(center);
            base.paint(context, center, parentBox, sliderTheme, enableAnimation, thumbCenter, isEnabled, textDirection);
        }
    }
    private sealed class SelectionRangeTicks : M.RoundRangeSliderTickMarkShape
    {
        internal List<Offset> Centers { get; } = [];
        public override void paint(PaintingContext context, Offset center, RenderBox parentBox, M.SliderThemeData sliderTheme,
            Animation<double> enableAnimation, Offset startThumbCenter, Offset endThumbCenter, bool isEnabled = false, TextDirection textDirection = default)
        {
            Centers.Add(center);
            base.paint(context, center, parentBox, sliderTheme, enableAnimation, startThumbCenter, endThumbCenter, isEnabled, textDirection);
        }
    }
}
