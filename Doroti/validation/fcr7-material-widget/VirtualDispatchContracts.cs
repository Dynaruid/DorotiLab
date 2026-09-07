using System.Reflection;
using Doroti.Framework.Animation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;
using C = Doroti.Framework.Cupertino;

internal static class VirtualDispatchContracts
{
    internal static void Verify()
    {
        using var environment = PlatformEnvironmentContext.Enter(new PlatformConfiguration([new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.windows));
        var theme = M.ThemeData.CreateLight();
        var context = new Material3Contracts.ThemeContext(theme);
        T Create<T>(Assembly assembly, string name, params object?[] arguments) =>
            (T)Activator.CreateInstance(assembly.GetType(name)!, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, arguments, null)!;

        var yes = new StateConstraint(WidgetState.selected);
        var no = new StateConstraint(WidgetState.disabled);
        var and = Create<WidgetStatesConstraint>(typeof(Widget).Assembly, "Doroti.Framework.Widgets._WidgetStateAnd__widget_state", yes, no);
        var or = Create<WidgetStatesConstraint>(typeof(Widget).Assembly, "Doroti.Framework.Widgets._WidgetStateOr__widget_state", yes, no);
        Require(!and.isSatisfiedBy([WidgetState.selected]) && and.isSatisfiedBy([WidgetState.selected, WidgetState.disabled]), "AND evaluates through inherited interface slot");
        Require(or.isSatisfiedBy([WidgetState.selected]) && !or.isSatisfiedBy([]), "OR evaluates through inherited interface slot");
        Require(and.op_OnesComplement().isSatisfiedBy([WidgetState.selected]), "nested state composition retains dispatch");

        Tween<Alignment> tween = new AlignmentTween(new Alignment(-1, -1), new Alignment(1, 1));
        var middle = tween.transform(0.5);
        Require(middle.x == 0 && middle.y == 0, "Tween.transform invokes geometric interpolation");
        Tween<FractionalOffset?> fractional = new FractionalOffsetTween(null, new FractionalOffset(1, 1));
        Require(fractional.transform(0.5) is { dx: 0.75, dy: 0.75 }, "nullable geometric tween interpolates from the Flutter null origin (center)");

        var slider = Create<M.SliderThemeData>(typeof(M.ThemeData).Assembly, "Doroti.Framework.Material._RangeSliderDefaultsM3__range_slider", context);
        Require(slider.activeTrackColor == theme.colorScheme.primary && slider.inactiveTrackColor == theme.colorScheme.secondaryContainer, "range slider colors survive base theme dispatch");
        Require(slider.rangeThumbShape is M.HandleRangeSliderThumbShape && slider.thumbSize!.resolve([]) is not null, "range slider supplies shapes and state sizes");

        ScrollMetrics page = new PageMetrics(0, 1000, 100, 200, AxisDirection.right, 0.8, 1);
        Require(page.copyWith(pixels: 300) is PageMetrics { pixels: 300, viewportFraction: 0.8 }, "page snapshot preserves metric subtype and page fraction");
        ScrollMetrics wheel = new FixedExtentMetrics(0, 1000, 100, 200, AxisDirection.down, 2, 1);
        Require(wheel.copyWith(pixels: 300, itemIndex: 6) is FixedExtentMetrics { pixels: 300, itemIndex: 6 }, "wheel snapshot preserves item index");
        ScrollMetrics carousel = Create<ScrollMetrics>(typeof(M.ThemeData).Assembly, "Doroti.Framework.Material._CarouselMetrics__carousel", 0d, 1000d, 100d, 200d, AxisDirection.right, 50d, null, true, 1d);
        Require(carousel.copyWith(pixels: 250) is M._CarouselMetrics__carousel { pixels: 250, itemExtent: 50, consumeMaxWeight: true }, "carousel snapshot uses canonical base signature and retains item configuration");

        var dynamicColor = C.CupertinoDynamicColor.CreateWithBrightness(color: new Color(0xff112233), darkColor: new Color(0xffabcdef));
        Color color = dynamicColor;
        Require(color.resolveFrom(context) is C.CupertinoDynamicColor resolved && !ReferenceEquals(resolved, dynamicColor), "Color generic entry point resolves a Cupertino dynamic color");

        RenderSliverFixedExtentBoxAdaptor varied = new RenderSliverVariedExtentList(null!, (_, _) => 32);
        Require(varied.itemExtentBuilder!(0, default!) == 32, "varied sliver exposes its extent callback through the layout base");
        varied.itemExtentBuilder = (_, _) => 48;
        Require(varied.itemExtentBuilder!(0, default!) == 48, "varied sliver callback replacement uses the same property slot");

        ShapeBorder inputBorder = new M.UnderlineInputBorder(borderSide: new BorderSide(color: new Color(0xff112233), width: 2));
        var recorder = new PictureRecorder();
        inputBorder.paint(new Canvas(recorder), Rect.fromLTWH(0, 0, 100, 40));
        using var borderPicture = recorder.endRecording();
        Require(borderPicture.Commands.Count > 0, "ShapeBorder.paint draws the input border through its extended paint signature");

        object diagnostic = theme;
        Require(diagnostic.ToString() == theme.ToString(Doroti.Framework.Foundation.DiagnosticLevel.info), "Object.ToString forwards to Flutter diagnostic formatting");
        object renderDiagnostic = new RenderLimitedBox();
        Require(renderDiagnostic.ToString() == ((RenderObject)renderDiagnostic).toStringShort(), "render diagnostics use the short description through Object");

        foreach (var assembly in new[] { typeof(M.ThemeData).Assembly, typeof(C.CupertinoDynamicColor).Assembly })
        foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(TextSelectionHandleControls).IsAssignableFrom(t)))
        {
            var control = (TextSelectionControls)Activator.CreateInstance(type, nonPublic: true)!;
            // A handle-only control deliberately does not consult a selection delegate.
            control.handleCut(null!);
            control.handleCopy(null!);
        }
        Console.WriteLine("Virtual dispatch behavior: PASS (state composition, tweens, range slider, page/wheel/carousel metrics, dynamic color, four handle-only controls, varied sliver, diagnostics, input border)");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("Virtual dispatch: " + message);
    }

    private sealed class StateConstraint(WidgetState state) : WidgetStatesConstraint
    {
        public bool isSatisfiedBy(HashSet<WidgetState> states) => states.Contains(state);
        public WidgetStatesConstraint op_BitwiseAnd(WidgetStatesConstraint other) => throw new NotSupportedException();
        public WidgetStatesConstraint op_BitwiseOr(WidgetStatesConstraint other) => throw new NotSupportedException();
        public WidgetStatesConstraint op_OnesComplement() => throw new NotSupportedException();
    }
}
