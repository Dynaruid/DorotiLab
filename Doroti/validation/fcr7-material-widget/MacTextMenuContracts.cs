using Doroti.Framework.Foundation;
using PointerDownEvent = Doroti.Framework.Gestures.PointerDownEvent;
using PointerUpEvent = Doroti.Framework.Gestures.PointerUpEvent;
using PointerHoverEvent = Doroti.Framework.Gestures.PointerHoverEvent;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using M = Doroti.Framework.Material;
using C = Doroti.Framework.Cupertino;

internal static partial class MountedPickerContracts
{
    internal static void VerifyMacTextMenu()
    {
        foreach (var brightness in new[] { Brightness.light, Brightness.dark }) VerifyMacTextMenu(brightness);
    }

    private static void VerifyMacTextMenu(Brightness brightness)
    {
        using var platform = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
            [new Locale("en", "US")], brightness, false, false, HostOperatingSystem.macOS));
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var host = new Host();
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null,
            "mac-menu", "mac-menu", "mac-menu");
        var clipboard = new ClipboardFixtureHost { Text = "Paste fixture" };
        using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("mac-menu")
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
            var controller = new TextEditingController(text: "Hello Doroti");
            var focus = new FocusNode();
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(
                new M.MaterialApp(locale: new Locale("en", "US"), theme: M.ThemeData.Create(platform: TargetPlatform.macOS, brightness: brightness),
                    home: new M.Scaffold(body: new Center(child: new SizedBox(width: 300,
                        child: new M.TextField(controller: controller, focusNode: focus))))))));
            Pump();
            var editable = (EditableTextState)Elements(binding.rootElement!).OfType<StatefulElement>()
                .Single(e => e.widget is EditableText).state;
            var position = editable.renderEditable.localToGlobal(new Offset(20, 10));
            Click(position, 2);
            Pump();
            var menu = Elements(binding.rootElement!).SingleOrDefault(e => e.widget is C.CupertinoDesktopTextSelectionToolbar)
                ?? throw new Exception($"Right-click did not build macOS menu: focus={focus.hasFocus}, selection={controller.selection}");
            var box = (RenderBox)menu.findRenderObject()!;
            if (box.size.isEmpty) throw new Exception("macOS menu has no layout size");
            using var surface = SKSurface.Create(new SKImageInfo(Width, Height));
            if (renderer.Paint(surface, Width, Height) is { } completion)
                renderer.CompletePaint(completion, DorotiFrameTerminal.submitted);
            using var snapshot = surface.Snapshot();
            using var pixels = SKBitmap.FromImage(snapshot);
            foreach (var label in Elements(menu).Where(e => e.widget is Text))
            {
                var textBox = (RenderBox)label.findRenderObject()!;
                var origin = textBox.localToGlobal(Offset.zero);
                var ink = 0;
                for (var y = (int)origin.dy; y < (int)(origin.dy + textBox.size.height); y++)
                for (var x = (int)origin.dx; x < (int)(origin.dx + textBox.size.width); x++)
                {
                    if (x < 0 || y < 0 || x >= Width || y >= Height) throw new Exception("Menu text outside viewport");
                    var color = pixels.GetPixel(x, y);
                    if (brightness == Brightness.light ? color.Red < 100 && color.Green < 100 && color.Blue < 100
                        : color.Red > 200 && color.Green > 200 && color.Blue > 200) ink++;
                }
                if (ink < 10) throw new Exception($"{brightness}: menu label '{((Text)label.widget).data}' has no visible ink");
            }
            using var png = snapshot.Encode(SKEncodedImageFormat.Png, 100);
            Directory.CreateDirectory("Doroti/artifacts/mac-input");
            File.WriteAllBytes($"Doroti/artifacts/mac-input/text-menu-{brightness}.png", png.ToArray());
            PressMenuItem(ContextMenuButtonType.copy);
            if (clipboard.Text != "Hello" || controller.text != "Hello Doroti") throw new Exception("Menu Copy failed");
            Click(position, 2);
            Pump();
            PressMenuItem(ContextMenuButtonType.cut);
            if (controller.text != " Doroti") throw new Exception("Menu Cut failed");
            view.DispatchPlatformEvent(() => controller.text = "");
            Pump();
            Click(position, 2);
            Pump();
            PressMenuItem(ContextMenuButtonType.paste);
            if (controller.text != "Hello") throw new Exception("Empty-field menu Paste failed");
            Console.WriteLine($"macOS {brightness}: menu text, rounded hover, pressed opacity, Copy/Cut/Paste and dismissal PASS");

            void PressMenuItem(ContextMenuButtonType type)
            {
                var label = type switch { ContextMenuButtonType.copy => "Copy", ContextMenuButtonType.cut => "Cut", _ => "Paste" };
                var button = Elements(binding.rootElement!).Single(e => e.widget is C.CupertinoDesktopTextSelectionToolbarButton &&
                    Elements(e).Any(child => child.widget is Text text && text.data == label));
                var buttonBox = (RenderBox)button.findRenderObject()!;
                var center = buttonBox.localToGlobal(buttonBox.size.center(Offset.zero));
                Hover(new Offset(10, 10));
                Hover(center);
                Pump();
                using var hover = Capture(type == ContextMenuButtonType.copy ? "hover" : null);
                var origin = buttonBox.localToGlobal(Offset.zero);
                var sampleX = (int)(origin.dx + buttonBox.size.width - 15);
                var sampleY = (int)center.dy;
                var selected = hover.GetPixel(sampleX, sampleY);
                var theme = C.CupertinoTheme.of(button);
                var primary = theme.primaryColor!;
                if (Math.Abs(selected.Red - primary.red) > 1 || Math.Abs(selected.Green - primary.green) > 1 ||
                    Math.Abs(selected.Blue - primary.blue) > 1)
                    throw new Exception($"{brightness}: hovered row does not use the theme highlight: {selected}");
                var corner = hover.GetPixel((int)origin.dx, (int)origin.dy);
                if (corner == selected)
                    throw new Exception($"{brightness}: hover highlight has a square corner");
                var textElement = Elements(button).Single(e => e.widget is Text);
                var textBox = (RenderBox)textElement.findRenderObject()!;
                var textOrigin = textBox.localToGlobal(Offset.zero);
                var foreground = theme.primaryContrastingColor!;
                var ink = 0;
                for (var y = (int)textOrigin.dy; y < textOrigin.dy + textBox.size.height; y++)
                for (var x = (int)textOrigin.dx; x < textOrigin.dx + textBox.size.width; x++)
                {
                    var pixel = hover.GetPixel(x, y);
                    if (Math.Abs(pixel.Red - foreground.red) + Math.Abs(pixel.Green - foreground.green) +
                        Math.Abs(pixel.Blue - foreground.blue) < 60) ink++;
                }
                if (ink < 10) throw new Exception($"{brightness}: hovered label lost contrasting text");
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerDownEvent(viewId: 1,
                    pointer: 1, kind: PointerDeviceKind.mouse, position: center, buttons: 1)));
                Pump();
                using var pressed = Capture(type == ContextMenuButtonType.copy ? "pressed" : null);
                if (pressed.GetPixel(sampleX, sampleY) == selected)
                    throw new Exception($"{brightness}: pressed row did not fade");
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerUpEvent(viewId: 1,
                    pointer: 1, kind: PointerDeviceKind.mouse, position: center)));
                Pump();
                if (Elements(binding.rootElement!).Any(e => e.widget is C.CupertinoDesktopTextSelectionToolbar))
                    throw new Exception($"{type}: menu did not dismiss");
            }

            void Hover(Offset point) => view.DispatchPlatformEvent(() => binding.handlePointerEvent(
                new PointerHoverEvent(viewId: 1, kind: PointerDeviceKind.mouse, position: point)));

            SKBitmap Capture(string? state)
            {
                if (renderer.Paint(surface, Width, Height) is { } frame)
                    renderer.CompletePaint(frame, DorotiFrameTerminal.submitted);
                using var current = surface.Snapshot();
                if (state is not null)
                {
                    using var data = current.Encode(SKEncodedImageFormat.Png, 100);
                    File.WriteAllBytes($"Doroti/artifacts/mac-input/text-menu-{brightness}-{state}.png", data.ToArray());
                }
                return SKBitmap.FromImage(current);
            }

            void Click(Offset point, int buttons)
            {
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerDownEvent(viewId: 1,
                    pointer: 1, kind: PointerDeviceKind.mouse, position: point, buttons: buttons)));
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerUpEvent(viewId: 1,
                    pointer: 1, kind: PointerDeviceKind.mouse, position: point)));
            }
            void Pump()
            {
                for (var i = 0; i < 30; i++) { host.Fire(); Thread.Sleep(10); }
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
