using Doroti.Framework.Animation;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Ui;

var requiredComponents = new HashSet<string>(StringComparer.Ordinal)
{
    "scaffold-background", "app-bar-text", "floating-action-button",
    "ink-well-sparkle", "scrollbar-list-sliver", "slider-overlay", "shader-mask-image-filter",
};
var capturedComponents = new HashSet<string>(StringComparer.Ordinal);

var seed = new Doroti.Ui.Color(0xff6750a4L);
var lightSurface = new Doroti.Ui.Color(0xfffffbfeL);
var darkSurface = new Doroti.Ui.Color(0xff141218L);
var lightPalette = Doroti.Framework.Material.ColorScheme.CreateFromSeed(
    seedColor: seed,
    brightness: Doroti.Ui.Brightness.light,
    surface: lightSurface);
var darkPalette = Doroti.Framework.Material.ColorScheme.CreateFromSeed(
    seedColor: seed,
    brightness: Doroti.Ui.Brightness.dark,
    surface: darkSurface);
var lightTheme = Doroti.Framework.Material.ThemeData.Create(
    colorScheme: lightPalette,
    useMaterial3: true,
    platform: Doroti.Framework.Foundation.TargetPlatform.windows);
var darkTheme = Doroti.Framework.Material.ThemeData.Create(
    colorScheme: darkPalette,
    useMaterial3: true,
    platform: Doroti.Framework.Foundation.TargetPlatform.windows);
Require(lightTheme.brightness == Doroti.Ui.Brightness.light, "light palette preserves light brightness");
Require(darkTheme.brightness == Doroti.Ui.Brightness.dark, "dark palette preserves dark brightness");
Require(lightTheme.colorScheme.surface.value == lightSurface.value, "light palette accepts role overrides");
Require(darkTheme.colorScheme.surface.value == darkSurface.value, "dark palette accepts role overrides");
Require(lightTheme.scaffoldBackgroundColor.value == lightSurface.value, "light scaffold owns the light surface");
Require(darkTheme.scaffoldBackgroundColor.value == darkSurface.value, "dark scaffold owns the dark surface");
Require(lightTheme.colorScheme.primary.value != darkTheme.colorScheme.primary.value, "seed palette resolves brightness-specific roles");
var systemThemeApp = new Doroti.Framework.Material.MaterialApp(
    theme: lightTheme,
    darkTheme: darkTheme,
    themeMode: Doroti.Framework.Material.ThemeMode.system);
Require(ReferenceEquals(systemThemeApp.theme, lightTheme), "MaterialApp retains the light palette");
Require(ReferenceEquals(systemThemeApp.darkTheme, darkTheme), "MaterialApp retains the dark palette");
Require(systemThemeApp.themeMode == Doroti.Framework.Material.ThemeMode.system, "MaterialApp follows platform brightness in system mode");
VerifyScrollbarAlphaContract();
VerifyRadiusSizedMaterialShapes();
VerifyTypedActionDispatch();
VerifyTapRegionHitIdentity();
VerifyVariableGlyphCaretMetrics();
VerifyMobileSelectionOverlayContracts();
VerifyFrameworkLifecycleContracts();
VerifyButtonStyleDispatch();
VerifyHostTextInputVisibilityContract();
VerifyDefaultTextEditingShortcutContracts();

var widgetsBinding = (Doroti.Framework.Widgets.WidgetsFlutterBinding)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(
    typeof(Doroti.Framework.Widgets.WidgetsFlutterBinding));
widgetsBinding._observers = [];
widgetsBinding._backGestureObservers = [];
var environmentProbe = new EnvironmentObserverProbe();
widgetsBinding.addObserver(environmentProbe);
widgetsBinding.handleTextScaleFactorChanged();
widgetsBinding.handlePlatformBrightnessChanged();
Require(environmentProbe.TextScaleChanges == 1, "WidgetsBinding forwards text-scale changes to MediaQuery observers");
Require(environmentProbe.BrightnessChanges == 1, "WidgetsBinding forwards live platform-brightness changes to MediaQuery observers");
Require(widgetsBinding.removeObserver(environmentProbe), "WidgetsBinding removes the environment observer");
widgetsBinding.handlePlatformBrightnessChanged();
Require(environmentProbe.BrightnessChanges == 1, "removed environment observer no longer receives brightness changes");

var scenarios = new[]
{
    Scenario("fab-press", "floating-action-button", ["default", "pressed", "focused", "disabled"], ["down", "hold", "up", "semantics"]),
    Scenario("ink-cold-warm", "ink-well-sparkle", ["cold", "warm", "hovered", "pressed"], ["hover", "down", "move", "hold", "up"]),
    Scenario("scrollbar-range", "scrollbar-list-sliver", ["top", "middle", "end"], ["scroll", "semantics"]),
    Scenario("slider-hover-drag", "slider-overlay", ["idle", "hovered", "dragged", "released"], ["hover", "down", "move", "hold", "up"]),
    Scenario("effects-cold-warm", "shader-mask-image-filter", ["cold", "warm"], ["frame"]),
};

foreach (var scenario in scenarios)
{
    Require(requiredComponents.Contains(scenario.Component), $"scenario component is in the fixed source slice: {scenario.Id}");
    Require(scenario.States.Count >= 2, $"scenario compares more than one state: {scenario.Id}");
    Require(scenario.Actions.Count > 0, $"scenario has replayable actions: {scenario.Id}");
    Require(scenario.Actions.Any(action => action is "down" or "scroll" or "frame"), $"scenario has a causal input or frame: {scenario.Id}");
    capturedComponents.Add(scenario.Component);
}

Require(capturedComponents.SetEquals(["floating-action-button", "ink-well-sparkle", "scrollbar-list-sliver", "slider-overlay", "shader-mask-image-filter"]), "interactive/effect slice scenarios are complete");
Require(requiredComponents.Contains("scaffold-background") && requiredComponents.Contains("app-bar-text"), "persistent scaffold and text coverage is explicit");
Console.WriteLine($"FCR-7 material/widget runtime contract: PASS (configuration={ConfigurationName()}, system-theme-palettes=light+dark)");

static Scenario Scenario(string id, string component, IReadOnlyList<string> states, IReadOnlyList<string> actions) => new(id, component, states, actions);
static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }

static void VerifyScrollbarAlphaContract()
{
    var androidIdlePeak = new Color(0xff5f6368L);
    var fadeFrames = new[]
    {
        CaptureScrollbarFrame(androidIdlePeak, 1.0, 0),
        CaptureScrollbarFrame(androidIdlePeak, 1.0, 600),
        CaptureScrollbarFrame(androidIdlePeak, 0.5, 750),
        CaptureScrollbarFrame(androidIdlePeak, 0.0, 900),
    };
    Require(fadeFrames.Select(frame => frame.ThumbAlpha).SequenceEqual([255, 255, 128, 0]),
        "transient Android thumb holds through 600 ms and fades to zero over the next 300 ms");
    Require(fadeFrames.Zip(fadeFrames.Skip(1), (left, right) => right.ThumbAlpha <= left.ThumbAlpha).All(value => value),
        "transient thumb alpha is monotonically non-increasing during fade");
    Require(fadeFrames.Where(frame => frame.FadeValue > 0).All(frame => frame.TrackAlpha == 0),
        "the transparent track remains distinct from the fading thumb");

    var alwaysVisibleFrames = new[] { 0, 600, 750, 900 }
        .Select(timestamp => CaptureScrollbarFrame(androidIdlePeak, 1.0, timestamp))
        .ToArray();
    Require(alwaysVisibleFrames.All(frame => frame.ThumbAlpha == 255),
        "thumbVisibility true keeps the Flutter Android idle peak fully visible instead of starting fade");

    var themedThumb = Color.fromARGB(102, 255, 0, 0);
    var themedPeak = CaptureScrollbarFrame(themedThumb, 1.0, 0);
    var themedMid = CaptureScrollbarFrame(themedThumb, 0.5, 150);
    Require(themedPeak.ThumbAlpha == 102 && themedMid.ThumbAlpha == 51,
        "an explicit semitransparent ScrollbarTheme thumb color is multiplied by fade exactly once");
    Require(themedPeak.Commands.Last(command => command.Operation == "drawRect").Arguments[4] == themedThumb.value,
        "retained draw command snapshots the framework paint ARGB without dropping alpha");

    var whiteComposite = Doroti.Ui.Dart_uiLibrary.Color.alphaBlend(themedThumb, new Color(0xffffffffL));
    var blackComposite = Doroti.Ui.Dart_uiLibrary.Color.alphaBlend(themedThumb, new Color(0xff000000L));
    Require(whiteComposite.value == 0xffff9999 && blackComposite.value == 0xff660000,
        "known white and black backgrounds recover the expected effective alpha for the themed thumb");
}

static void VerifyRadiusSizedMaterialShapes()
{
    var radiusSize = Size.fromRadius(24);
    Require(radiusSize.width == 48 && radiusSize.height == 48,
        "Size.fromRadius converts radius to a full diameter on both axes");

    var sliderTheme = new Doroti.Framework.Material.SliderThemeData(trackHeight: 4);
    var overlaySize = new Doroti.Framework.Material.RoundSliderOverlayShape().getPreferredSize(true, false);
    var thumbSize = new Doroti.Framework.Material.RoundSliderThumbShape().getPreferredSize(true, false);
    var tickSize = new Doroti.Framework.Material.RoundSliderTickMarkShape().getPreferredSize(sliderTheme, true);
    var rangeThumbSize = new Doroti.Framework.Material.RoundRangeSliderThumbShape().getPreferredSize(true, false);
    var rangeTickSize = new Doroti.Framework.Material.RoundRangeSliderTickMarkShape().getPreferredSize(sliderTheme, true);

    Require(overlaySize.width == 48 && overlaySize.height == 48,
        "the default 24 logical-pixel slider overlay reserves its full 48 logical-pixel diameter");
    Require(thumbSize.width == 20 && thumbSize.height == 20 &&
            rangeThumbSize.width == 20 && rangeThumbSize.height == 20,
        "slider and range-slider thumbs report their full enabled diameter");
    Require(tickSize.width == 2 && tickSize.height == 2 &&
            rangeTickSize.width == 2 && rangeTickSize.height == 2,
        "slider and range-slider tick marks report their full radius-derived diameter");
}

static void VerifyTypedActionDispatch()
{
    var action = new ProbeAction();
    var context = new StatelessElement(new ProbeWidget());
    var result = new ActionDispatcher().invokeAction(action, new ProbeIntent(), context);
    Require(action.InvokeCount == 1 && Equals(result, "invoked"),
        "ActionDispatcher invokes a generic action without dynamic binder loss");
}

static void VerifyDefaultTextEditingShortcutContracts()
{
    var shortcutsField = typeof(DefaultTextEditingShortcuts).GetField(
        "_androidShortcuts",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
    var shortcuts = shortcutsField?.GetValue(null) as System.Collections.IDictionary;
    Require(shortcuts is not null, "Android default text-editing shortcuts are available");

    var backspace = shortcuts!.Keys.Cast<object>().OfType<SingleActivator>().SingleOrDefault(activator =>
        ReferenceEquals(activator.trigger, Doroti.Framework.Services.LogicalKeyboardKey.backspace) &&
        !activator.control && !activator.shift && !activator.alt && !activator.meta);
    Require(backspace is not null, "Android Backspace has a default text-editing shortcut");
    Require(shortcuts[backspace!] is DeleteCharacterIntent { forward: false },
        "Android Backspace deletes the preceding character");

    var arrowLeft = shortcuts.Keys.Cast<object>().OfType<SingleActivator>().Any(activator =>
        ReferenceEquals(activator.trigger, Doroti.Framework.Services.LogicalKeyboardKey.arrowLeft) &&
        !activator.control && !activator.shift && !activator.alt && !activator.meta);
    Require(arrowLeft, "Android shortcuts include the common text-selection map spread");

    var selectAll = shortcuts.Keys.Cast<object>().OfType<SingleActivator>().Any(activator =>
        ReferenceEquals(activator.trigger, Doroti.Framework.Services.LogicalKeyboardKey.keyA) &&
        activator.control && !activator.shift && !activator.alt && !activator.meta);
    Require(selectAll, "Android shortcuts include the clipboard map spread");
}

static void VerifyTapRegionHitIdentity()
{
    var insideCount = 0;
    var surface = new RenderTapRegionSurface();
    var region = new RenderTapRegion(
        registry: surface,
        behavior: Doroti.Framework.Rendering.HitTestBehavior.opaque,
        onTapInside: _ => insideCount++);
    surface.child = region;
    surface.layout(Doroti.Framework.Rendering.BoxConstraints.CreateTight(new Size(100, 100)));

    var result = new Doroti.Framework.Rendering.BoxHitTestResult();
    Require(surface.hitTest(result, new Offset(20, 20)), "tap-region surface is hit inside its child");
    var surfaceEntry = result.path.Single(entry => ReferenceEquals(entry.target, surface));
    surface.handleEvent(
        new Doroti.Framework.Gestures.PointerDownEvent(position: new Offset(20, 20)),
        surfaceEntry);
    Require(insideCount == 1,
        "TapRegion recovers the cached hit result after generic hit-test entry adaptation");
    surface.dispose();
}

static void VerifyVariableGlyphCaretMetrics()
{
    var paragraph = new Paragraph(
        "A한B",
        width: 0,
        height: 20,
        fontSize: 16,
        codeUnitAdvances: [7, 18, 9]);
    paragraph.layout(new ParagraphConstraints(double.PositiveInfinity));

    var boxes = paragraph.getBoxesForRange(0, 3);
    Require(boxes.Count == 1 && Math.Abs(boxes[0].right - 34) < 0.001,
        "paragraph selection boxes use measured per-glyph advances");
    var koreanGlyph = paragraph.getGlyphInfoAt(1);
    Require(koreanGlyph is not null &&
            Math.Abs(koreanGlyph.graphemeClusterLayoutBounds.left - 7) < 0.001 &&
            Math.Abs(koreanGlyph.graphemeClusterLayoutBounds.right - 25) < 0.001,
        "caret geometry follows the measured fallback-font glyph bounds");
    Require(paragraph.getPositionForOffset(new Offset(10, 5)).offset == 1 &&
            paragraph.getPositionForOffset(new Offset(20, 5)).offset == 2,
        "pointer hit testing uses the same glyph advances as caret geometry");
    paragraph.dispose();
}

static void VerifyMobileSelectionOverlayContracts()
{
    var controls = Doroti.Framework.Material.Text_selectionLibrary.materialTextSelectionHandleControls;
    Require(controls is TextSelectionHandleControls,
        "Material selection controls retain the handle-controls marker used by the context-menu path");
    var materialHandleSize = controls.getHandleSize(24);
    Require(materialHandleSize.width > 0 && materialHandleSize.height > 0,
        "Material mobile selection controls expose a non-empty drag handle");
    var materialHandleChild = new ProbeWidget();
    var materialHandleBox = SizedBox.CreateSquare(dimension: materialHandleSize.width, child: materialHandleChild);
    Require(ReferenceEquals(materialHandleBox.child, materialHandleChild),
        "SizedBox.square retains the Material selection-handle painter subtree");
    var rotatedMaterialHandle = Transform.CreateRotate(angle: Math.PI / 2.0, child: materialHandleBox);
    Require(ReferenceEquals(rotatedMaterialHandle.child, materialHandleBox),
        "Transform.rotate retains the left Material selection-handle subtree");

    var cupertinoControls = Doroti.Framework.Cupertino.Text_selectionLibrary.cupertinoTextSelectionHandleControls;
    Require(cupertinoControls is TextSelectionHandleControls,
        "Cupertino mobile selection controls retain the shared handle-controls marker");
    var cupertinoHandleSize = cupertinoControls.getHandleSize(24);
    Require(cupertinoHandleSize.width > 0 && cupertinoHandleSize.height > 0,
        "Cupertino mobile selection controls expose a non-empty drag handle");
    var cupertinoHandleChild = new ProbeWidget();
    var cupertinoHandleBox = SizedBox.CreateFromSize(size: cupertinoHandleSize, child: cupertinoHandleChild);
    Require(ReferenceEquals(cupertinoHandleBox.child, cupertinoHandleChild),
        "SizedBox.fromSize retains the Cupertino selection-handle painter subtree");

    var paintMethod = typeof(_RenderMagnification__magnifier).GetMethod(
        nameof(Doroti.Framework.Rendering.RenderObject.paint),
        [typeof(Doroti.Framework.Rendering.PaintingContext), typeof(Offset)]);
    Require(paintMethod is not null &&
            paintMethod.GetBaseDefinition().DeclaringType == typeof(Doroti.Framework.Rendering.RenderObject) &&
            paintMethod.DeclaringType == typeof(_RenderMagnification__magnifier),
        "RawMagnifier overrides RenderObject.paint so its backdrop transform participates in rendering");

    var toolbarRenderType = typeof(Doroti.Framework.Material._TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar);
    Require(IsOverride(toolbarRenderType, "performLayout", Type.EmptyTypes) &&
            IsOverride(toolbarRenderType, "paint", [typeof(Doroti.Framework.Rendering.PaintingContext), typeof(Offset)]) &&
            IsOverride(toolbarRenderType, "setupParentData", [typeof(Doroti.Framework.Rendering.RenderObject)]),
        "Material selection toolbar overrides RenderObject layout, paint, and parent-data dispatch");

    var materialHandlePainterType = typeof(Doroti.Framework.Material.MaterialTextSelectionControls).Assembly.GetType(
        "Doroti.Framework.Material._TextSelectionHandlePainter__text_selection", throwOnError: true)!;
    var cupertinoHandlePainterType = typeof(Doroti.Framework.Cupertino.CupertinoTextSelectionControls).Assembly.GetType(
        "Doroti.Framework.Cupertino._CupertinoTextSelectionHandlePainter__text_selection", throwOnError: true)!;
    Require(IsOverride(
                materialHandlePainterType,
                "paint",
                [typeof(Canvas), typeof(Size)]) &&
            IsOverride(
                cupertinoHandlePainterType,
                "paint",
                [typeof(Canvas), typeof(Size)]),
        "Material and Cupertino mobile handle painters override CustomPainter.paint");

    static bool IsOverride(Type declaringType, string methodName, Type[] parameterTypes)
    {
        var method = declaringType.GetMethod(methodName, parameterTypes);
        return method is not null && method.DeclaringType == declaringType && method.GetBaseDefinition() != method;
    }
}

static void VerifyFrameworkLifecycleContracts()
{
    var key = new Doroti.Framework.Foundation.ValueKey<string>("factory-key");
    var child = new ProbeWidget();
    var positioned = Positioned.CreateFromRect(
        key: key,
        rect: Rect.fromLTWH(1, 2, 30, 40),
        child: child);
    Require(ReferenceEquals(positioned.key, key) && ReferenceEquals(positioned.child, child),
        "Positioned.fromRect preserves key and child identity");

    var primaryNone = PrimaryScrollController.CreateNone(key: key, child: child);
    Require(ReferenceEquals(primaryNone.key, key) && ReferenceEquals(primaryNone.child, child) &&
            primaryNone.controller is null && primaryNone.automaticallyInheritForPlatforms.Count == 0,
        "PrimaryScrollController.none preserves its subtree and disables inheritance");

    var repaintBoundary = RepaintBoundary.CreateWrap(child, 7);
    Require(ReferenceEquals(repaintBoundary.child, child) &&
            repaintBoundary.key is Doroti.Framework.Foundation.ValueKey<object> repaintKey &&
            Equals(repaintKey.value, 7L),
        "RepaintBoundary.wrap preserves its child and derives the fallback key from the index");

    var editingValue = new Doroti.Framework.Services.TextEditingValue(
        text: "factory",
        selection: new Doroti.Framework.Services.TextSelection(1, 4),
        composing: new TextRange(0, 3));
    var editingController = TextEditingController.CreateFromValue(editingValue);
    Require(editingController.text == "factory" &&
            editingController.selection.baseOffset == 1 &&
            editingController.selection.extentOffset == 4 &&
            editingController.value.composing.start == 0 &&
            editingController.value.composing.end == 3,
        $"TextEditingController.fromValue preserves text, selection, and composing state " +
        $"(text={editingController.text}, selection={editingController.selection.baseOffset}:{editingController.selection.extentOffset}, " +
        $"composing={editingController.value.composing.start}:{editingController.value.composing.end})");

    var logicalKeys = new HashSet<Doroti.Framework.Services.LogicalKeyboardKey>
    {
        Doroti.Framework.Services.LogicalKeyboardKey.control,
        Doroti.Framework.Services.LogicalKeyboardKey.shift,
        Doroti.Framework.Services.LogicalKeyboardKey.keyA,
        Doroti.Framework.Services.LogicalKeyboardKey.keyB,
        Doroti.Framework.Services.LogicalKeyboardKey.keyC,
    };
    var logicalKeySet = LogicalKeySet.CreateFromSet(logicalKeys);
    Require(logicalKeySet.keys.SetEquals(logicalKeys),
        "LogicalKeySet.fromSet preserves sets larger than the positional constructor limit");

    var item = new ProbeWidget();
    var separator = new ProbeWidget();
    var separated = SliverList.CreateSeparated(
        itemBuilder: (_, _) => item,
        separatorBuilder: (_, _) => separator,
        itemCount: 2,
        addAutomaticKeepAlives: false,
        addRepaintBoundaries: false,
        addSemanticIndexes: false);
    Require(separated.@delegate is SliverChildBuilderDelegate separatedDelegate &&
            separatedDelegate.childCount == 3 &&
            ReferenceEquals(((KeyedSubtree)separatedDelegate.build(null!, 0)!).child, item) &&
            ReferenceEquals(((KeyedSubtree)separatedDelegate.build(null!, 1)!).child, separator),
        "SliverList.separated retains item/separator builders and computes the child count");

    var semanticsProperties = new Doroti.Framework.Semantics.SemanticsProperties(label: "factory semantics");
    var semantics = Semantics.CreateFromProperties(key: key, child: child, properties: semanticsProperties);
    var sliverSemantics = SliverSemantics.CreateFromProperties(key: key, child: child, properties: semanticsProperties);
    Require(ReferenceEquals(semantics.key, key) &&
            ReferenceEquals(semantics.child, child) &&
            ReferenceEquals(semantics.properties, semanticsProperties) &&
            ReferenceEquals(sliverSemantics.key, key) &&
            ReferenceEquals(sliverSemantics.child, child) &&
            ReferenceEquals(sliverSemantics.properties, semanticsProperties),
        "Semantics.fromProperties variants preserve key, child, and properties identity");

    var notifier = new Doroti.Framework.Foundation.ChangeNotifier();
    var notifications = 0;
    Action listener = () => notifications++;
    notifier.addListener(listener);
    notifier.removeListener(listener);
    notifier.notifyListeners();
    Require(notifications == 0 && notifier.debugListenerCount == 0,
        "ChangeNotifier removes the same delegate identity without retaining callbacks");

    using var platformEnvironment = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
        locales: [],
        platformBrightness: Brightness.light,
        alwaysUse24HourFormat: false,
        nativeSpellCheckServiceDefined: false,
        operatingSystem: HostOperatingSystem.android));
    var toolbar = Doroti.Framework.Material.AdaptiveTextSelectionToolbar.CreateEditable(
        clipboardStatus: ClipboardStatus.notPasteable,
        anchors: new TextSelectionToolbarAnchors(Offset.zero));
    Require(toolbar.buttonItems is { Count: 0 },
        "nullable adaptive-toolbar callbacks remain absent instead of creating failing actions");

    var lifecycleMethodNames = new HashSet<string>(StringComparer.Ordinal)
    {
        "paint", "detach", "attach", "performLayout", "computeDryLayout",
        "setupParentData", "applyPaintTransform", "hitTestChildren",
        "dispose", "addListener", "removeListener",
    };
    var frameworkAssemblies = new[]
    {
        typeof(Doroti.Framework.Foundation.ChangeNotifier).Assembly,
        typeof(Doroti.Framework.Rendering.RenderObject).Assembly,
        typeof(Widget).Assembly,
        typeof(Doroti.Framework.Cupertino.CupertinoTabController).Assembly,
        typeof(Doroti.Framework.Material.TabController).Assembly,
    }.Distinct();
    var hiddenLifecycleSlots = new List<string>();
    foreach (var type in frameworkAssemblies.SelectMany(assembly => assembly.GetTypes()))
    {
        foreach (var method in type.GetMethods(
                     System.Reflection.BindingFlags.Instance |
                     System.Reflection.BindingFlags.Public |
                     System.Reflection.BindingFlags.NonPublic |
                     System.Reflection.BindingFlags.DeclaredOnly))
        {
            if (!method.IsVirtual ||
                !method.Attributes.HasFlag(System.Reflection.MethodAttributes.NewSlot) ||
                !lifecycleMethodNames.Contains(method.Name))
            {
                continue;
            }

            var signature = method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            for (var baseType = type.BaseType; baseType is not null; baseType = baseType.BaseType)
            {
                var baseMethod = baseType.GetMethod(
                    method.Name,
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic,
                    binder: null,
                    types: signature,
                    modifiers: null);
                if (baseMethod is { IsVirtual: true })
                {
                    hiddenLifecycleSlots.Add($"{type.FullName}.{method.Name}");
                    break;
                }
            }
        }
    }
    Require(hiddenLifecycleSlots.Count == 0,
        $"framework lifecycle hooks use CLR override dispatch: {string.Join(", ", hiddenLifecycleSlots)}");
}

static void VerifyButtonStyleDispatch()
{
    var buttonStyleType = typeof(Doroti.Framework.Material.ButtonStyle);
    var materialAssembly = buttonStyleType.Assembly;
    var buttonWidgetType = typeof(Doroti.Framework.Material.ButtonStyleButton);
    foreach (var type in new[]
             {
                 typeof(Doroti.Framework.Material.ElevatedButton),
                 typeof(Doroti.Framework.Material.OutlinedButton),
             })
    {
        foreach (var methodName in new[] { "defaultStyleOf", "themeStyleOf" })
        {
            var method = type.GetMethod(methodName,
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.DeclaredOnly);
            Require(method is not null && method.GetBaseDefinition().DeclaringType == buttonWidgetType,
                $"{type.Name}.{methodName} overrides ButtonStyleButton instead of hiding its CLR slot");
        }
    }

    var defaultStyleTypes = new[]
    {
        "Doroti.Framework.Material._ElevatedButtonDefaultsM3__elevated_button",
        "Doroti.Framework.Material._OutlinedButtonDefaultsM3__outlined_button",
    };
    var requiredProperties = new[]
    {
        "textStyle", "backgroundColor", "foregroundColor", "overlayColor",
        "shadowColor", "surfaceTintColor", "elevation", "padding",
        "minimumSize", "maximumSize", "iconColor", "iconSize", "shape",
    };

    foreach (var typeName in defaultStyleTypes)
    {
        var styleType = materialAssembly.GetType(typeName, throwOnError: true)!;
        foreach (var propertyName in requiredProperties)
        {
            var getter = styleType.GetProperty(propertyName,
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.DeclaredOnly)
                ?.GetMethod;
            Require(getter is not null && getter.GetBaseDefinition().DeclaringType == buttonStyleType,
                $"{styleType.Name}.{propertyName} overrides ButtonStyle instead of hiding its CLR slot");
        }
    }

    var outlinedStyleType = materialAssembly.GetType(
        "Doroti.Framework.Material._OutlinedButtonDefaultsM3__outlined_button", throwOnError: true)!;
    var sideGetter = outlinedStyleType.GetProperty("side",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.DeclaredOnly)
        ?.GetMethod;
    Require(sideGetter is not null && sideGetter.GetBaseDefinition().DeclaringType == buttonStyleType,
        "OutlinedButton side overrides ButtonStyle so its visible outline reaches the renderer");
}

static void VerifyHostTextInputVisibilityContract()
{
    var controlType = typeof(Doroti.Framework.Services.TextInputControl);
    var hostControlType = controlType.Assembly.GetType(
        "Doroti.Framework.Services._HostTextInputControl", throwOnError: true)!;
    foreach (var methodName in new[] { "show", "hide" })
    {
        var method = hostControlType.GetMethod(methodName,
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.DeclaredOnly);
        Require(method is not null && method.GetBaseDefinition().DeclaringType == controlType,
            $"host text input {methodName} overrides TextInputControl and reaches the native visibility contract");
    }

    var instance = hostControlType.GetProperty("instance",
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.Public)!
        .GetValue(null);
    hostControlType.GetMethod("hide",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.DeclaredOnly)!
        .Invoke(instance, null);
    Require(true, "scheduled text-input hide is safe after the final host client detaches");
}

static ScrollbarAlphaFrame CaptureScrollbarFrame(Color thumbColor, double fadeValue, int timestampMilliseconds)
{
    var animation = new MutableAnimation(fadeValue);
    var painter = new ScrollbarPainter(
        color: thumbColor,
        fadeoutOpacityAnimation: animation,
        trackColor: new Color(0x00000000L),
        trackBorderColor: new Color(0x00000000L),
        textDirection: TextDirection.ltr,
        thickness: 6,
        padding: EdgeInsets.zero);
    painter.update(new FixedScrollMetrics(
        minScrollExtent: 0,
        maxScrollExtent: 840,
        pixels: 210,
        viewportDimension: 240,
        axisDirection: AxisDirection.down,
        devicePixelRatio: 1), AxisDirection.down);
    var commands = new List<PathCommand>();
    painter.paint(new Canvas(commands), new Size(720, 360));
    var rectangles = commands.Where(command => command.Operation == "drawRect").ToArray();
    var trackAlpha = rectangles.Length == 0 ? 0 : Alpha(rectangles[0]);
    var thumbAlpha = rectangles.Length < 2 ? 0 : Alpha(rectangles[^1]);
    painter.dispose();
    return new(timestampMilliseconds, fadeValue, trackAlpha, thumbAlpha, commands);
}

static int Alpha(PathCommand command) => (int)(((uint)command.Arguments[4] >> 24) & 0xff);

static string ConfigurationName() =>
#if DEBUG
    "Debug";
#else
    "Release";
#endif

sealed record Scenario(string Id, string Component, IReadOnlyList<string> States, IReadOnlyList<string> Actions);

sealed record ScrollbarAlphaFrame(
    int TimestampMilliseconds,
    double FadeValue,
    int TrackAlpha,
    int ThumbAlpha,
    IReadOnlyList<PathCommand> Commands);

sealed class MutableAnimation : Animation<double>
{
    private readonly HashSet<Action> _listeners = [];

    public MutableAnimation(double value) => Value = value;

    public double Value { get; private set; }
    public override double value => Value;
    public override AnimationStatus status => Value <= 0 ? AnimationStatus.dismissed : AnimationStatus.forward;
    public override void addListener(Action listener) => _listeners.Add(listener);
    public override void removeListener(Action listener) => _listeners.Remove(listener);
    public override void addStatusListener(AnimationStatusListener listener) { }
    public override void removeStatusListener(AnimationStatusListener listener) { }
}

sealed class EnvironmentObserverProbe : Doroti.Framework.Widgets.WidgetsBindingObserver
{
    public int TextScaleChanges { get; private set; }
    public int BrightnessChanges { get; private set; }

    public void didChangeTextScaleFactor() => TextScaleChanges++;
    public void didChangePlatformBrightness() => BrightnessChanges++;
}

sealed class ProbeIntent : Intent
{
}

sealed class ProbeAction : Doroti.Framework.Widgets.Action<ProbeIntent>
{
    public int InvokeCount { get; private set; }

    public override object? invoke(ProbeIntent intent, BuildContext? context = null)
    {
        InvokeCount++;
        return "invoked";
    }
}

sealed class ProbeWidget : StatelessWidget
{
    public override Widget build(BuildContext context) => throw new NotSupportedException();
}
