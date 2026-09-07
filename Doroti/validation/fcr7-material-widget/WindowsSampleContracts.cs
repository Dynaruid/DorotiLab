using System.Reflection;
using Doroti.Framework.Animation;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
using Doroti.Skia.Rendering;
using SkiaSharp;
using M = Doroti.Framework.Material;
using Path = System.IO.Path;

internal static class WindowsSampleContracts
{
    internal static string OutputDirectory = System.IO.Path.Combine(".doroti/evidence", "windows-sample-" + Guid.NewGuid().ToString("N"));
    private const BindingFlags Members = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    internal static async Task Verify()
    {
        using var platform = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
            [new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.windows));
        Material3Contracts.Verify();
        VerifyRail();
        VerifyClock();
        VerifyCalendar();
        await VerifyNativeText();
        VerifyNativeShadows();
        MountedPickerContracts.Verify();
        Console.WriteLine("Windows sample regressions: PASS");
    }

    private static void VerifyNativeShadows()
    {
        using var environment = new ImageFixtureEnvironment();
        var draw = typeof(SkiaSceneRenderer).GetMethod("DrawShadow", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var convert = typeof(SkiaSceneRenderer).GetMethod("ToPath", BindingFlags.Static | BindingFlags.NonPublic, [typeof(Doroti.Ui.Path)])!;
        var path = new Doroti.Ui.Path();
        path.addRRect(RRect.fromRectAndCorners(Rect.fromLTWH(40, 30, 80, 60), topLeft: Radius.circular(12), topRight: Radius.circular(12)));
        using var nativePath = (SKPath)convert.Invoke(null, [path])!;
        Require(nativePath.Contains(41, 89) && !nativePath.Contains(41, 31), "asymmetric card corners survive native path conversion");
        foreach (var elevation in new double[] { 0, 1, 3, 6, 8, 12 })
        {
            using var bitmap = new SKBitmap(160, 140);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.Transparent);
            draw.Invoke(environment.Renderer, [canvas, new CanvasShadowPayload(path, new Color(0xff000000), elevation, false)]);
            var pixels = bitmap.Pixels;
            Require(elevation == 0 ? pixels.All(pixel => pixel.Alpha == 0) : pixels.Any(pixel => pixel.Alpha > 0),
                $"elevation {elevation} shadow visibility");
            Require(bitmap.GetPixel(80, 60).Alpha == 0, "opaque occluder interior is excluded from shadow");
            using var image = SKImage.FromBitmap(bitmap);
            using var png = image.Encode(SKEncodedImageFormat.Png, 100);
            File.WriteAllBytes(System.IO.Path.Combine(OutputDirectory, $"shadow-{elevation}.png"), png.ToArray());
        }
        Console.WriteLine("native shadows: zero elevation, all six levels, opaque occlusion and asymmetric corners PASS; tessellation pixel parity not asserted");
    }

    private static void VerifyClock()
    {
        foreach (var mode in new[] { M._HourMinuteMode__time_picker.hour, M._HourMinuteMode__time_picker.minute })
        foreach (var ring in new[] { M._HourDialType__time_picker.twelveHour, M._HourDialType__time_picker.twentyFourHourDoubleRing })
        for (var value = 0; value < (mode == M._HourMinuteMode__time_picker.hour ? 24 : 60); value++)
        {
            var time = new M.TimeOfDay(mode == M._HourMinuteMode__time_picker.hour ? value : 15,
                mode == M._HourMinuteMode__time_picker.minute ? value : 23);
            var widget = (M._Dial__time_picker)Activator.CreateInstance(typeof(M._Dial__time_picker), Members, null,
                [time, mode, ring, null, null], null)!;
            var state = widget.createElement().state;
            var theta = (double)state.GetType().GetMethod("_getThetaForTime", Members)!.Invoke(state, [time])!;
            var expected = Math.PI / 2 - 2 * Math.PI * (mode == M._HourMinuteMode__time_picker.hour ? value % 12 / 12.0 : value / 60.0);
            Require(Math.Abs(Math.Cos(theta) - Math.Cos(expected)) < 1e-8 && Math.Abs(Math.Sin(theta) - Math.Sin(expected)) < 1e-8,
                $"clock {mode} {value} points at its label");
            foreach (var angle in new[] { theta, theta - 2 * Math.PI, theta + 2 * Math.PI })
            {
                var result = (M.TimeOfDay)state.GetType().GetMethod("_getTimeForTheta", Members)!.Invoke(state,
                    [angle, false, time.hour >= 12 && ring == M._HourDialType__time_picker.twentyFourHourDoubleRing ? 0.0 : 1.0])!;
                Require(result.hour == time.hour && result.minute == time.minute, $"clock {mode} {value} round trip: {result.hour}:{result.minute}");
            }
        }
        Console.WriteLine("clock: all 24 hours and 60 minutes, both rings and signed angles PASS");
    }

    private static void VerifyCalendar()
    {
        var localizations = new M.DefaultMaterialLocalizations();
        for (var year = 2024; year <= 2028; year++)
        for (var month = 1; month <= 12; month++)
            Require(M.DateUtils.firstDayOffset(year, month, localizations) == (int)new DateTime(year, month, 1).DayOfWeek,
                "calendar weekdays match actual dates");
        var before = DateTime.Now;
        var now = new M.GregorianCalendarDelegate().now();
        Require(now >= before && now <= DateTime.Now, "calendar uses current local date");
        var currentTime = M.TimeOfDay.CreateNow();
        Require(currentTime.hour == DateTime.Now.Hour && currentTime.minute == DateTime.Now.Minute, "clock starts at current local time");
        var route = new M.DialogRoute<DateTime?>(new FixtureContext(), _ => SizedBox.CreateShrink());
        route.didComplete(null);
        Require(route.popped.GetAwaiter().GetResult() is null, "nullable date route preserves cancellation");
        Console.WriteLine("calendar: weekdays, current date/time, nullable cancellation PASS");
    }

    private static void VerifyRail()
    {
        var selected = new List<long>();
        var rail = new M.NavigationRail(selectedIndex: 0, onDestinationSelected: selected.Add,
            destinations: Enumerable.Range(0, 4).Select(index => new M.NavigationRailDestination(
                icon: new Icon(M.Icons.add), label: new Text($"Destination {index}"))).ToList());
        var state = rail.createElement().state;
        Set(state, "_destinationAnimations", Enumerable.Range(0, 4).Select(_ => (Animation<double>)new AlwaysStoppedAnimation<double>(0)).ToList());
        using var animation = new AnimationLease();
        Set(state, "_extendedAnimation", animation.Value);
        var tree = state.build(new FixtureContext());
        var destinations = Descendants(tree).Where(widget => widget.GetType().Name == "_RailDestination__navigation_rail").ToArray();
        Require(destinations.Length == 4, "rail builds four independent targets");
        foreach (var destination in destinations) ((Action)destination.GetType().GetProperty("onTap", Members)!.GetValue(destination)!)();
        Require(selected.SequenceEqual(new long[] { 0, 1, 2, 3 }), "rail callbacks retain their own destination: " + string.Join(',', selected));
        Console.WriteLine("rail: each built callback selects its own index PASS");
    }

    private static async Task VerifyNativeText()
    {
        using var environment = new ImageFixtureEnvironment();
        foreach (var weight in new[] { "medium", "bold", "regular" })
            await environment.Renderer.RegisterFontAsync(File.ReadAllBytes($"DorotiTestbedApp/assets/fonts/Roboto-{weight}.ttf"), "Roboto");
        const string label = "Create project";
        using var face = SKTypeface.FromFile("DorotiTestbedApp/assets/fonts/Roboto-medium.ttf");
        using var font = new SKFont(face, 14);
        var labelStyle = new TextStyle(fontFamily: "Roboto", fontSize: 14, fontWeight: FontWeight.w500, letterSpacing: 0.1);
        using var measured = environment.Renderer.Layout(new ParagraphRequest(label, double.PositiveInfinity, null, 14,
            TextRuns: [new ParagraphTextRun(label, labelStyle)]), DartUiInvocation.Managed("button-font-metrics"));
        var expectedWidth = font.MeasureText(label) + label.Length * 0.1;
        Require(Math.Abs(measured.maxIntrinsicWidth - expectedWidth) < 0.01,
            $"button label must use medium font and letter spacing: {measured.maxIntrinsicWidth} vs {expectedWidth}");
        var fallbackStyle = new TextStyle(fontFamily: "Missing sample font", fontFamilyFallback: ["Roboto"], fontSize: 14, fontWeight: FontWeight.w500, letterSpacing: 0.1);
        using var fallback = environment.Renderer.Layout(new ParagraphRequest(label, double.PositiveInfinity, null, 14,
            TextRuns: [new ParagraphTextRun(label, fallbackStyle)]), DartUiInvocation.Managed("explicit-fallback-font-metrics"));
        Require(Math.Abs(fallback.maxIntrinsicWidth - measured.maxIntrinsicWidth) < 0.01,
            "An unavailable primary family must use the loaded Roboto fallback, including its weight");
        var extentMethod = typeof(SkiaSceneRenderer).GetMethod("PictureRasterExtent", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
        var bounds = SKRect.Create(0, 0, 590, 732);
        var translatedHeights = new HashSet<double>();
        var extents = Enumerable.Range(0, 1000).Select(index =>
        {
            var matrix = SKMatrix.CreateScale(1.25f, 1.25f);
            matrix.TransY = index * 0.37f;
            translatedHeights.Add(Math.Ceiling(matrix.MapRect(bounds).Height));
            return (SKSize)extentMethod.Invoke(null, [matrix, bounds])!;
        }).ToArray();
        Require(extents.All(extent => extent == extents[0]), "fractional scrolling must not resize a retained raster cache");
        Console.WriteLine($"raster cache geometry: translated rounding={string.Join(',', translatedHeights)}, stable height={Math.Ceiling(extents[0].Height)}");
        var fontPath = Path.GetFullPath("DorotiTestbedApp/assets/fonts/MaterialIcons-Regular.otf");
        await environment.Renderer.RegisterFontAsync(File.ReadAllBytes(fontPath), "MaterialIcons");
        var builder = new ParagraphBuilder(new ParagraphStyle(fontSize: 24));
        builder.pushStyle(new TextStyle(fontFamily: "MaterialIcons", fontSize: 24, height: 1, leadingDistribution: TextLeadingDistribution.even));
        builder.addText(char.ConvertFromUtf32((int)M.Icons.add.codePoint));
        using var icon = builder.build();
        icon.layout(new ParagraphConstraints(24));
        Require(Math.Abs(icon.height - 24) < 0.01 && Math.Abs(icon.alphabeticBaseline - 24) < 0.01,
            $"icon em box and actual font baseline: {icon.height}/{icon.alphabeticBaseline}");
        using var centered = environment.Renderer.Layout(new ParagraphRequest("12", 100, null, 24, TextAlign: TextAlign.center), DartUiInvocation.Managed("native-text-test"));
        var box = centered.getBoxesForRange(0, 2).Single();
        Require(Math.Abs((box.left + box.right) / 2 - 50) < 0.01, "picker number paint and hit geometry centered");
        using var multiline = environment.Renderer.Layout(new ParagraphRequest("A\nB", 100, null, 24, TextAlign: TextAlign.right), DartUiInvocation.Managed("native-text-test"));
        Require(multiline.numberOfLines == 2 && multiline.getBoxesForRange(2, 3).Single().top > 0, "line break geometry");
        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder);
        canvas.drawColor(new Color(0xffffffff), BlendMode.src);
        canvas.drawParagraph(icon, new Offset(0, 0));
        canvas.drawParagraph(centered, new Offset(30, 0));
        canvas.drawParagraph(multiline, new Offset(30, 40));
        using var picture = recorder.endRecording();
        using var image = await picture.toImage(150, 110);
        var bytes = (await image.toByteData())!.asMemory().ToArray();
        var iconPixels = Enumerable.Range(0, 24 * 24).Where(i => bytes[((i / 24) * 150 + i % 24) * 4] < 128).ToArray();
        Require(iconPixels.Length > 0, "icon paints pixels");
        Require(Math.Abs((iconPixels.Min(i => i / 24) + iconPixels.Max(i => i / 24) + 1) / 2.0 - 12) <= 1,
            "icon pixels centered vertically");
        Require(Enumerable.Range(70, 30).Any(y => Enumerable.Range(100, 30).Any(x => bytes[(y * 150 + x) * 4] < 128)),
            "second line is actually painted at its right-aligned geometry");
        var output = OutputDirectory;
        Directory.CreateDirectory(output);
        File.WriteAllBytes(Path.Combine(output, "native-text.png"), (await image.toByteData(ImageByteFormat.png))!.asMemory().ToArray());
        var encoded = (await image.toByteData(ImageByteFormat.png))!.asMemory();
        using var decoded = await environment.Renderer.DecodeAsync(encoded, DartUiInvocation.Managed("native-decode-test"));
        var nativeImage = (SkiaSharp.SKImage)decoded.HostHandle!.GetType().GetProperty("Image", Members)!.GetValue(decoded.HostHandle)!;
        Require(!nativeImage.IsLazyGenerated, "decoded images have pixels ready before their first scroll paint");
        Console.WriteLine("native text: icon baseline/pixels, centered picker number, painted multiline alignment PASS");
    }

    private static void Set(object target, string name, object value) => target.GetType().GetProperty(name, Members)!.SetValue(target, value);
    private static IEnumerable<Widget> Descendants(Widget root)
    {
        yield return root;
        foreach (var property in root.GetType().GetProperties(Members).Where(p => p.Name is "child" or "children"))
        {
            var children = property.GetValue(root) switch { Widget child => new[] { child }, IEnumerable<Widget> many => many, _ => [] };
            foreach (var child in children) foreach (var nested in Descendants(child)) yield return nested;
        }
    }
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private sealed class AnimationLease : IDisposable
    {
        internal CurvedAnimation Value { get; } = new(new AlwaysStoppedAnimation<double>(0), Curves.linear);
        public void Dispose() => Value.dispose();
    }
    private sealed class FixtureContext : StatelessElement
    {
        private readonly List<InheritedWidget> _inherited = [];
        internal FixtureContext() : base(new Builder(builder: _ => SizedBox.CreateShrink()))
        {
            _inherited.Add(new MediaQuery(data: new MediaQueryData(size: new Size(1280, 900)), child: SizedBox.CreateShrink()));
            _inherited.Add(new Directionality(textDirection: TextDirection.ltr, child: SizedBox.CreateShrink()));
            _inherited.Add((InheritedWidget)new M.Theme(data: M.ThemeData.CreateLight(), child: SizedBox.CreateShrink()).build(this));
            var state = new Localizations(locale: new Locale("en", "US"), delegates: []).createState();
            var resources = new DartMap<Type, object> { [typeof(M.MaterialLocalizations)] = new M.DefaultMaterialLocalizations() };
            Set(state, "_typeToResources", resources);
            Set(state, "_locale", new Locale("en", "US"));
            var scope = typeof(Localizations).Assembly.GetType("Doroti.Framework.Widgets._LocalizationsScope__localizations")!;
            _inherited.Add((InheritedWidget)Activator.CreateInstance(scope, Members, null, [null, new Locale("en", "US"), state, resources, SizedBox.CreateShrink()], null)!);
        }
        public override T? dependOnInheritedWidgetOfExactType<T>(object? aspect = null) where T : default => _inherited.OfType<T>().FirstOrDefault();
        public override InheritedElement? getElementForInheritedWidgetOfExactType<T>() => _inherited.FirstOrDefault(widget => widget is T)?.createElement();
        public override InheritedWidget dependOnInheritedElement(InheritedElement ancestor, object? aspect = null) => (InheritedWidget)ancestor.widget;
    }
}
