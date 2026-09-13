using Doroti.Framework.Foundation;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using PointerDownEvent = Doroti.Framework.Gestures.PointerDownEvent;
using PointerMoveEvent = Doroti.Framework.Gestures.PointerMoveEvent;
using PointerUpEvent = Doroti.Framework.Gestures.PointerUpEvent;
using M = Doroti.Framework.Material;

internal static partial class MountedPickerContracts
{
    internal static void VerifyTouchCursorDrag()
    {
        foreach (var target in new[] { TargetPlatform.iOS, TargetPlatform.android })
            foreach (var sampleController in new string?[] { null, "_filled", "_outlined" })
                VerifyTouchCursorDrag(target, sampleController);
    }

    private static void VerifyTouchCursorDrag(TargetPlatform target, string? sampleController)
    {
        var sample = sampleController is not null;
        var os = target == TargetPlatform.iOS ? HostOperatingSystem.iOS : HostOperatingSystem.android;
        using var platform = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
            [new Locale("en", "US")], Brightness.light, false, false, os, supportsShowingSystemContextMenu: !sample && target == TargetPlatform.iOS));
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var host = new Host { Configuration = new([new Locale("en", "US")], Brightness.light, false, false, os, supportsShowingSystemContextMenu: !sample && target == TargetPlatform.iOS) };
        const int viewportWidth = 390;
        host.Resize(viewportWidth);
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null,
            "touch-cursor-drag", "touch-cursor-drag", "touch-cursor-drag");
        var clipboard = new ClipboardFixtureHost();
        using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("touch-cursor-drag")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
            .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
            .Register<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, renderer)
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, host)
            .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, clipboard));
        var platformMethods = new List<string>();
        host.MessageSent = (channel, data) =>
        {
            if (channel == "flutter/platform" && data is { } bytes)
            {
                using var message = System.Text.Json.JsonDocument.Parse(bytes);
                platformMethods.Add(message.RootElement.GetProperty("method").GetString()!);
            }
        };
        var binding = new WidgetsFlutterBinding(dispatcher);
        var errors = new List<FlutterErrorDetails>();
        var previousError = FlutterError.onError;
        FlutterError.onError = errors.Add;
        try
        {
            long pointer = 0;
            var controller = new TextEditingController(text: "Hello Doroti");
            var focus = new FocusNode();
            var scaffoldKey = new GlobalKey<M.ScaffoldState>();
            Widget body = sample ? new MaterialSample.ComponentsScreen(false, scaffoldKey)
                : new Align(alignment: Doroti.Framework.Painting.Alignment.topCenter,
                    child: new Padding(padding: Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: 120),
                        child: new SizedBox(width: 300, child: new M.TextField(controller: controller, focusNode: focus, maxLines: 3))));
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(
                new M.MaterialApp(locale: new Locale("en", "US"), theme: M.ThemeData.Create(platform: target, brightness: Brightness.light),
                    home: new M.Scaffold(key: scaffoldKey, body: body)))));
            Pump();
            EditableTextState editable;
            if (sample)
            {
                var components = Elements(binding.rootElement!).OfType<StatefulElement>()
                    .Single(e => e.widget is MaterialSample.ComponentsScreen).state;
                var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
                view.DispatchPlatformEvent(() => components.GetType().GetMethod("MaterializeSection", flags)!.Invoke(components, [27]));
                Pump();
                controller = (TextEditingController)components.GetType().GetField(sampleController!, flags)!.GetValue(components)!;
                view.DispatchPlatformEvent(() => controller.text = "Hello Doroti");
                Pump();
                editable = Elements(binding.rootElement!).OfType<StatefulElement>()
                    .Where(e => e.widget is EditableText field && ReferenceEquals(field.controller, controller))
                    .Select(e => (EditableTextState)e.state).OrderBy(e => e.renderEditable.localToGlobal(Offset.zero).dy).First();
                focus = editable.widget.focusNode;
            }
            else editable = (EditableTextState)Elements(binding.rootElement!).OfType<StatefulElement>()
                .Single(e => e.widget is EditableText).state;
            var scenario = $"{target} {sampleController ?? "standalone"}";
            var focusLosses = 0;
            focus.addListener(() => { if (!focus.hasFocus) focusLosses++; });

            var render = editable.renderEditable;
            Offset Caret(long offset) => render.localToGlobal(render.getLocalRectForCaret(new TextPosition(offset: offset)).center);
            var tap = Caret(5);
            view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerDownEvent(viewId: 1, pointer: ++pointer, position: tap)));
            view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerUpEvent(viewId: 1, pointer: pointer, position: tap)));
            Pump(40);
            if (!focus.hasFocus || !controller.selection.isCollapsed)
                throw new Exception($"{target}: initial tap did not focus/collapse selection: {controller.selection}");
            var initial = controller.selection.extentOffset;
            var from = Caret(initial);
            var to = Caret(10);
            view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerDownEvent(viewId: 1, pointer: ++pointer, position: from)));
            var last = from;
            for (var step = 1; step <= 10; step++)
            {
                var next = from + (to - from) * (step / 10.0);
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerMoveEvent(viewId: 1, pointer: pointer, position: next, delta: next - last)));
                last = next;
                Pump(1);
            }
            view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerUpEvent(viewId: 1, pointer: pointer, position: to)));
            Pump();
            if (!controller.selection.isCollapsed || controller.selection.extentOffset != 10)
                throw new Exception($"{target}: dragging the cursor from {initial} to 10 produced {controller.selection}");
            CheckFocus();
            Console.WriteLine($"{scenario}: touch cursor drag PASS");
            // Start outside the current caret's handle target so the field's
            // long-press recognizer owns the gesture.
            from = Caret(1);
            to = Caret(8);
            view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerDownEvent(viewId: 1, pointer: ++pointer, position: from)));
            Pump(20); // Past tap-down dispatch, before long-press acceptance.
            CheckFocus();
            Pump(50);
            CheckFocus();
            if (target == TargetPlatform.iOS && !render.floatingCursorOn)
                throw new Exception("A focused iOS long press must start the floating cursor.");
            last = from;
            for (var step = 1; step <= 10; step++)
            {
                var next = from + (to - from) * (step / 10.0);
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerMoveEvent(viewId: 1, pointer: pointer, position: next, delta: next - last)));
                last = next;
                Pump(1);
            }
            // Check while the pointer is still held: releasing must not be
            // required to update selection after crossing the normal touch slop.
            CheckLongPressSelection();
            view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerUpEvent(viewId: 1, pointer: pointer, position: to)));
            Pump();
            CheckLongPressSelection();
            if (render.floatingCursorOn)
                throw new Exception($"{target}: floating cursor remained active after releasing the drag");
            Console.WriteLine($"{scenario}: long-press drag, continuous focus and release PASS");
            if (!sample && target == TargetPlatform.iOS)
            {
                if (!platformMethods.Contains("ContextMenu.showSystemContextMenu"))
                    throw new Exception("Supported iOS TextField did not request the native context menu.");
                // Exercise the host capability boundary, as used by the iOS
                // keyboard rather than a pointer gesture on the field.
                view.DispatchPlatformEvent(() => editable.hideToolbar());
                view.DispatchPlatformEvent(() => { controller.text = "Hello Doroti\nSecond line"; controller.selection = Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: 1); });
                Pump();
                var origin = Caret(1);
                var destination = Caret(18);
                host.FloatCursor(new(DorotiFloatingCursorPhase.start, Offset.zero));
                host.FloatCursor(new(DorotiFloatingCursorPhase.update, destination - origin));
                Pump(1);
                if (!render.floatingCursorOn) throw new Exception("Host floating cursor event was not delivered to EditableText.");
                host.FloatCursor(new(DorotiFloatingCursorPhase.end, Offset.zero));
                Pump();
                if (render.floatingCursorOn || controller.selection.extentOffset != 18)
                    throw new Exception($"Host floating cursor did not commit selection: {controller.selection}");
                CheckFocus();
                if (!platformMethods.Contains("ContextMenu.hideSystemContextMenu"))
                    throw new Exception("Native context menu did not receive the hide request.");
                Console.WriteLine("iOS system context menu: default selection, serialization and dismissal PASS");
                Console.WriteLine("iOS host floating cursor capability: start/update/end and selection commit PASS");
            }
            if (sample)
            {
                view.DispatchPlatformEvent(() => editable.hideToolbar());
                Pump();
                var section = Elements(binding.rootElement!).OfType<StatefulElement>()
                    .Single(e => e.widget is MaterialSample.ComponentSection { Label: "Text fields" });
                var cardFocus = (FocusNode)section.state.GetType().GetField("_focus",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(section.state)!;
                var card = (RenderBox)cardFocus.context!.findRenderObject()!;
                var background = card.localToGlobal(new Offset(card.size.width / 2, 5));
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerDownEvent(viewId: 1, pointer: ++pointer, position: background)));
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new PointerUpEvent(viewId: 1, pointer: pointer, position: background)));
                Pump();
                if (!cardFocus.hasPrimaryFocus || focus.hasFocus)
                    throw new Exception($"{scenario}: tapping the card background did not transfer focus");
                Console.WriteLine($"{scenario}: card background tap PASS");
            }

            void CheckFocus()
            {
                if (!focus.hasFocus || focusLosses != 0)
                    throw new Exception($"{scenario}: cursor gesture lost field focus ({focusLosses} times)");
            }

            void CheckLongPressSelection()
            {
                CheckFocus();
                if (target == TargetPlatform.iOS
                    ? !controller.selection.isCollapsed || controller.selection.extentOffset != 8
                    : controller.selection.baseOffset != 0 || controller.selection.extentOffset != 12)
                    throw new Exception($"{scenario}: long-press drag produced {controller.selection}");
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
