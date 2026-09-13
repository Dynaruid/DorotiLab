using Doroti.Framework.Foundation;
using PointerDownEvent = Doroti.Framework.Gestures.PointerDownEvent;
using PointerUpEvent = Doroti.Framework.Gestures.PointerUpEvent;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using M = Doroti.Framework.Material;
using C = Doroti.Framework.Cupertino;

internal static partial class MountedPickerContracts
{
    internal static void VerifyIosTextMenu()
    {
        foreach (var brightness in new[] { Brightness.light, Brightness.dark })
        foreach (var fieldTop in new[] { 350.0, 12.0 })
        foreach (var longPress in new[] { false, true }) VerifyIosTextMenu(brightness, fieldTop, longPress);
    }

    private static IEnumerable<Element> Onstage(Element element)
    {
        yield return element;
        var children = new List<Element>();
        element.debugVisitOnstageChildren(children.Add);
        foreach (var child in children)
        foreach (var descendant in Onstage(child)) yield return descendant;
    }

    private static void VerifyIosTextMenu(Brightness brightness, double fieldTop, bool longPress)
    {
        using var platform = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
            [new Locale("en", "US")], brightness, false, false, HostOperatingSystem.iOS));
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var host = new Host { Configuration = new([new Locale("en", "US")], brightness, false, false, HostOperatingSystem.iOS) };
        const int viewportWidth = 390;
        host.Resize(viewportWidth);
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null,
            "ios-menu", "ios-menu", "ios-menu");
        var clipboard = new ClipboardFixtureHost { Text = "Paste fixture" };
        using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("ios-menu")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
            .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, host)
            .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, clipboard));
        var binding = new WidgetsFlutterBinding(dispatcher);
        var errors = new List<FlutterErrorDetails>();
        var previousError = FlutterError.onError;
        FlutterError.onError = errors.Add;
        try
        {
            long pointer = 0;
            var controller = new TextEditingController(text: "Hello Doroti");
            var focus = new FocusNode();
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(
                new M.MaterialApp(locale: new Locale("en", "US"), theme: M.ThemeData.Create(platform: TargetPlatform.iOS, brightness: brightness),
                    home: new M.Scaffold(body: new Align(alignment: Doroti.Framework.Painting.Alignment.topCenter, child: new Padding(padding: Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: fieldTop), child: new SizedBox(width: 300,
                        child: new M.TextField(controller: controller, focusNode: focus)))))))));
            Pump();
            var editable = (EditableTextState)Elements(binding.rootElement!).OfType<StatefulElement>()
                .Single(e => e.widget is EditableText).state;
            if (editable.widget.selectionControls is not C.CupertinoTextSelectionHandleControls)
                throw new Exception("iOS TextField must default to Cupertino selection handles.");
            var position = editable.renderEditable.localToGlobal(new Offset(20, 10));
            if (longPress) LongPress(position); else DoubleTap(position);
            Pump();
            var menu = Elements(binding.rootElement!).SingleOrDefault(e => e.widget is C.CupertinoTextSelectionToolbar)
                ?? throw new Exception($"Touch selection did not build iOS menu: focus={focus.hasFocus}, selection={controller.selection}");
            var box = (RenderBox)menu.findRenderObject()!;
            if (box.size.isEmpty) throw new Exception("iOS menu has no layout size");
            var items = Onstage(menu).Select(e => e.renderObject)
                .OfType<C._RenderCupertinoTextSelectionToolbarItems__text_selection_toolbar>().First();
            foreach (var button in Onstage(menu).Where(e => e.widget is C.CupertinoTextSelectionToolbarButton))
            {
                var label = Onstage(button).FirstOrDefault(e => e.widget is Text);
                if (label is null) continue;
                var text = (Text)label.widget;
                var labelBox = (RenderBox)label.findRenderObject()!;
                var buttonBox = (RenderBox)button.findRenderObject()!;
                if (!longPress && fieldTop == 350)
                {
                    var origin = labelBox.localToGlobal(Offset.zero);
                    var buttonOrigin = buttonBox.localToGlobal(Offset.zero);
                    Console.WriteLine($"{brightness} {text.data}: text=({origin.dx:F2},{origin.dy:F2},{labelBox.size.width:F2},{labelBox.size.height:F2}) button=({buttonOrigin.dx:F2},{buttonOrigin.dy:F2},{buttonBox.size.width:F2},{buttonBox.size.height:F2}) fallback={SKTypeface.Default.FamilyName}");
                }
                if (text.style?.fontSize != 15 ||
                    Math.Abs(buttonBox.size.width - labelBox.size.width - 32) > 0.1 ||
                    Math.Abs(buttonBox.size.height - labelBox.size.height - 36) > 0.1)
                    throw new Exception($"iOS button '{text.data}' lost mobile typography/padding: {buttonBox.size}, label={labelBox.size}, font={text.style?.fontSize}");
            }
            if (!items.hasNextPage || items.hasPreviousPage)
                throw new Exception("iPhone-width menu must paginate its six mobile-sized actions");
            using var surface = SKSurface.Create(new SKImageInfo(viewportWidth, Height));
            if (renderer.Paint(surface, viewportWidth, Height) is { } completion)
                renderer.CompletePaint(completion, DorotiFrameTerminal.submitted);
            using var snapshot = surface.Snapshot();
            using var pixels = SKBitmap.FromImage(snapshot);
            using var png = snapshot.Encode(SKEncodedImageFormat.Png, 100);
            Directory.CreateDirectory("Doroti/artifacts/ios-input");
            File.WriteAllBytes($"Doroti/artifacts/ios-input/text-menu-{brightness}-{fieldTop}-{longPress}.png", png.ToArray());
            var firstLabel = Onstage(menu).First(e => e.widget is Text);
            var firstLabelBox = (RenderBox)firstLabel.findRenderObject()!;
            var backgroundPoint = firstLabelBox.localToGlobal(new Offset(-8, firstLabelBox.size.height / 2));
            var background = pixels.GetPixel((int)backgroundPoint.dx, (int)backgroundPoint.dy);
            var expectedBackground = brightness == Brightness.light ? new SKColor(246, 246, 246) : new SKColor(34, 34, 34);
            if (background != expectedBackground)
                throw new Exception($"iOS menu background should be {expectedBackground}, got {background}");
            foreach (var label in Onstage(menu).Where(e => e.widget is Text))
            {
                var textBox = (RenderBox)label.findRenderObject()!;
                var origin = textBox.localToGlobal(Offset.zero);
                var ink = 0;
                for (var y = (int)origin.dy; y < (int)(origin.dy + textBox.size.height); y++)
                for (var x = (int)origin.dx; x < (int)(origin.dx + textBox.size.width); x++)
                {
                    if (x < 0 || y < 0 || x >= viewportWidth || y >= Height) throw new Exception("Menu text outside viewport");
                    var color = pixels.GetPixel(x, y);
                    if (brightness == Brightness.light ? color.Red < 100 && color.Green < 100 && color.Blue < 100
                        : color.Red > 200 && color.Green > 200 && color.Blue > 200) ink++;
                }
                if (ink < 10) throw new Exception($"{brightness}: menu label '{((Text)label.widget).data}' has no visible ink");
            }
            var firstPageLabels = Onstage(menu).Where(e => e.widget is Text).Select(e => ((Text)e.widget).data).ToArray();
            Tap(items.nextButton!.localToGlobal(items.nextButton.size.center(Offset.zero)));
            Pump();
            if (items.page != 1 || !items.hasPreviousPage ||
                !Onstage(menu).Any(e => e.widget is Text text && text.data == "Share..."))
                throw new Exception("Overflow arrow did not reveal the remaining iOS actions");
            if (renderer.Paint(surface, viewportWidth, Height) is { } overflowCompletion)
                renderer.CompletePaint(overflowCompletion, DorotiFrameTerminal.submitted);
            using (var overflowSnapshot = surface.Snapshot())
            using (var overflowPng = overflowSnapshot.Encode(SKEncodedImageFormat.Png, 100))
                File.WriteAllBytes($"Doroti/artifacts/ios-input/text-menu-{brightness}-{fieldTop}-{longPress}-overflow.png", overflowPng.ToArray());
            Tap(items.backButton!.localToGlobal(items.backButton.size.center(Offset.zero)));
            Pump();
            if (items.page != 0 || !firstPageLabels.SequenceEqual(Onstage(menu).Where(e => e.widget is Text).Select(e => ((Text)e.widget).data)))
                throw new Exception("Back arrow did not restore the first iOS menu page");
            PressMenuItem(ContextMenuButtonType.copy);
            if (clipboard.Text != "Hello" || controller.text != "Hello Doroti") throw new Exception("Menu Copy failed");
            DoubleTap(position);
            Pump();
            PressMenuItem(ContextMenuButtonType.cut);
            if (controller.text != " Doroti") throw new Exception("Menu Cut failed");
            view.DispatchPlatformEvent(() => controller.text = "");
            Pump();
            var collapsedHandle = editable.widget.selectionControls!.buildHandle(
                editable.context, TextSelectionHandleType.collapsed, editable.renderEditable.preferredLineHeight);
            if (collapsedHandle is not SizedBox { child: null })
                throw new Exception("An iOS collapsed selection must retain an invisible drag target, without a painted handle.");
            if (editable.renderEditable.size.height <= 0)
                throw new Exception("Clearing an iOS text field collapsed its editable line height.");
            DoubleTap(position);
            Pump();
            PressMenuItem(ContextMenuButtonType.paste);
            if (controller.text != "Hello") throw new Exception("Empty-field menu Paste failed");
            Console.WriteLine($"iOS {brightness} top={fieldTop} longPress={longPress}: mobile style, visible menu text, pagination, Copy/Cut/Paste and dismissal PASS");

            void PressMenuItem(ContextMenuButtonType type)
            {
                var label = type switch { ContextMenuButtonType.copy => "Copy", ContextMenuButtonType.cut => "Cut", _ => "Paste" };
                var button = Elements(binding.rootElement!).Single(e => e.widget is C.CupertinoTextSelectionToolbarButton &&
                    Elements(e).Any(child => child.widget is Text text && text.data == label));
                var buttonBox = (RenderBox)button.findRenderObject()!;
                var center = buttonBox.localToGlobal(buttonBox.size.center(Offset.zero));
                Tap(center);
                Pump();
                if (Elements(binding.rootElement!).Any(e => e.widget is C.CupertinoTextSelectionToolbar))
                    throw new Exception($"{type}: menu did not dismiss");
            }

            void Tap(Offset point)
            {
                var id = ++pointer;
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerDownEvent(viewId: 1,
                    pointer: id, kind: PointerDeviceKind.touch, position: point)));
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerUpEvent(viewId: 1,
                    pointer: id, kind: PointerDeviceKind.touch, position: point)));
            }
            void DoubleTap(Offset point) { Tap(point); Thread.Sleep(60); Tap(point); }
            void LongPress(Offset point)
            {
                var id = ++pointer;
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerDownEvent(viewId: 1,
                    pointer: id, kind: PointerDeviceKind.touch, position: point)));
                Pump(70);
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerUpEvent(viewId: 1,
                    pointer: id, kind: PointerDeviceKind.touch, position: point)));
            }
            void Pump(int frames = 30)
            {
                for (var i = 0; i < frames; i++) { host.Fire(); Thread.Sleep(10); }
                if (errors.Count > 0) throw new Exception(string.Join("\n", errors.Select(e => e.exceptionThrown)));
            }
        }
        finally
        {
            ContextMenuController.removeAny();
            FlutterError.onError = previousError;
        }
    }
}
