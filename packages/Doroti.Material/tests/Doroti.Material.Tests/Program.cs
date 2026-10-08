using Doroti.Material;
using Doroti.Framework.Widgets;
using Doroti.Testing;
using Doroti.Ui;

static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
if (args.Contains("--editable-context-menu"))
{
    EditableContextMenuRegression.Run((controller, focus, readOnly, obscure) =>
        new MaterialApp(home: new Scaffold(body: new TextField(
            controller: controller, focusNode: focus, readOnly: readOnly, obscureText: obscure))));
    return;
}
static void Reject<T>(Action action) where T : Exception
{
    try { action(); } catch (T) { return; }
    throw new InvalidOperationException($"Expected {typeof(T).Name}.");
}
Require(!FrameworkShaderManifest.Assets.Any(asset => asset.Id == "material.ink-sparkle"), "Material was registered implicitly.");
MaterialShaderAssets.Register(); MaterialShaderAssets.Register();
Parallel.For(0, 4, _ => MaterialShaderAssets.Register());
var shader = FrameworkShaderManifest.Get("material.ink-sparkle");
Require(shader.OwningAssembly == "Doroti.Material", "Shader has the wrong owner.");
Reject<InvalidOperationException>(() => FrameworkShaderManifest.Register(typeof(MaterialApp).Assembly,
    [shader with { AdaptedSourceSha256 = new string('0', 64) }]));
Reject<InvalidOperationException>(() => FrameworkShaderManifest.Register(typeof(Widget).Assembly, [shader]));
var mutableUniforms = shader.Uniforms.ToArray();
FrameworkShaderManifest.Register(typeof(MaterialApp).Assembly, [shader with { Id = "test.frozen-material", Uniforms = mutableUniforms }]);
mutableUniforms[0] = new("corrupt", "float");
Require(FrameworkShaderManifest.Get("test.frozen-material").Uniforms[0] == shader.Uniforms[0], "Shader ABI backing array was not frozen.");
Require(MaterialImageColorRuntime.Quantize([0xffff0000, 0xffff0000], 1).Single().Value == 2, "Quantizer population changed.");
Require(MaterialImageColorRuntime.ArgbFromRgba([255, 0, 0, 255]).Single() == 0xffff0000, "RGBA-to-ARGB precision changed.");
Reject<ArgumentOutOfRangeException>(() => MaterialImageColorRuntime.Quantize([0xff000000], 0));
Reject<ArgumentOutOfRangeException>(() => MaterialColorSchemeRuntime.GetArgb(0xff6750a4, false, "tonalSpot", double.NaN, "primary"));
var scheme = ColorScheme.CreateFromSeed(new Color(0xff6750a4));
Require(scheme.primary.value == unchecked((uint)MaterialColorSchemeRuntime.GetArgb(0xff6750a4, false, "tonalSpot", 0, "primary")), "Seed role bridge changed.");
using var tester = new WidgetTester();
using var controller = new TextEditingController();
tester.pumpWidget(new MaterialApp(home: new Scaffold(body: new TextField(controller: controller))));
tester.tap(tester.byType<TextField>().Single());
Require(tester.HasTextClient, "Material focus did not attach the typed IME client.");
tester.enterText(new("한", new(1, 1), new(0, 1)));
tester.enterText(new("한글", new(0, 2), null));
Require(controller.text == "한글" && controller.value.selection.extentOffset == 2, "Material composing/selection lost state.");
var program = FrameworkShaderLoader.LoadProgram("material.ink-sparkle").asTask().GetAwaiter().GetResult();
Require(program is not null, "Packaged Material shader failed hash/ABI validation.");
FrameworkShaderManifest.Register(typeof(MaterialApp).Assembly, [shader with { Id = "test.bad-packaged-hash", AdaptedSourceSha256 = new string('0', 64) }]);
Reject<InvalidDataException>(() => FrameworkShaderLoader.LoadProgram("test.bad-packaged-hash").asTask().GetAwaiter().GetResult());
FrameworkShaderManifest.Register(typeof(MaterialApp).Assembly, [shader with { Id = "test.missing-resource", EmbeddedResourceName = "missing.shader" }]);
Reject<InvalidDataException>(() => FrameworkShaderLoader.LoadProgram("test.missing-resource").asTask().GetAwaiter().GetResult());
FrameworkShaderManifest.Register(typeof(MaterialApp).Assembly, [shader with { Id = "test.bad-abi", Uniforms = [new("missingUniform", "float")] }]);
Reject<InvalidDataException>(() => FrameworkShaderLoader.LoadProgram("test.bad-abi").asTask().GetAwaiter().GetResult());
Reject<InvalidOperationException>(() => FrameworkShaderManifest.Register(typeof(MaterialApp).Assembly,
    [shader with { Id = "test.atomic" }, shader with { Uniforms = [] }]));
Reject<KeyNotFoundException>(() => FrameworkShaderManifest.Get("test.atomic"));
Require(DefaultMaterialLocalizations.@delegate.isSupported(new Locale("en")) && !DefaultMaterialLocalizations.@delegate.isSupported(new Locale("ko")), "Default Material locale support changed.");
Require(Icons.add.fontFamily == "MaterialIcons" && Icons.add.fontPackage is null, "Material icon identity changed.");
tester.pumpWidget(new SizedBox());
Require(!tester.HasTextClient, "Material teardown retained the IME client.");
string? locale = null;
tester.pumpWidget(new MaterialApp(locale: new Locale("ko"), home: new Builder(builder: context =>
{ locale = Localizations.localeOf(context).languageCode; return new Text("Locale fallback"); })));
Require(locale == "en", "Unsupported Material locale did not resolve to the declared English fallback.");
Console.WriteLine("PASS: Material package-owned color extraction, seed roles, shader owner/hash/ABI/freeze/conflicts, typed text focus/composition/selection and teardown (CPU/synthetic).");
var menu = new MenuController();
var actions = 0;
tester.pumpWidget(new MaterialApp(home: new Scaffold(body: new Center(child: new MenuAnchor(controller: menu,
    menuChildren: [new MenuItemButton(onPressed: () => actions++, child: new Text("Choose item"))], child: new Text("Menu anchor"))))));
menu.open(); tester.pumpAndSettle(step: TimeSpan.FromMilliseconds(50), timeout: TimeSpan.FromMilliseconds(1500));
Require(menu.isOpen && tester.text("Choose item").Count == 1, "Material/Raw menu did not mount its overlay.");
tester.tap(tester.text("Choose item").Single()); tester.pumpAndSettle(step: TimeSpan.FromMilliseconds(50), timeout: TimeSpan.FromMilliseconds(1500));
Require(actions == 1 && !menu.isOpen, "Material/Raw menu action did not close exactly once.");
tester.pumpWidget(new SizedBox());
long radioSelection = -1;
tester.pumpWidget(new MaterialApp(home: new Scaffold(body: new Radio<long>(value: 2, groupValue: 1, onChanged: value => radioSelection = value))));
tester.tap(tester.byType<Radio<long>>().Single()); tester.pumpAndSettle();
Require(radioSelection == 2, "Material/Raw radio selection failed.");
tester.pumpWidget(new SizedBox());
Console.WriteLine("PASS: actual Material/Raw menu action/overlay close and radio pointer selection/unmount (synthetic).");
var tooltipKey = GlobalKey<RawTooltipState>.Create();
tester.pumpWidget(new MaterialApp(home: new Scaffold(body: new Center(child: new RawTooltip(key: tooltipKey,
    presentation: WindowPresentation.Auto, nativeSize: new Size(160, 40), enableFeedback: false,
    animationStyle: Doroti.Framework.Animation.AnimationStyle.noAnimation,
    tooltipBuilder: (_, _) => new Text("Raw tooltip content"), child: new Text("Tooltip anchor"))))));
Require(tooltipKey.currentState!.ensureTooltipVisible(), "Manual tooltip did not start.");
tester.pumpAndSettle();
Require(tester.text("Raw tooltip content").Count == 1, "Unsupported native host did not use the tooltip Overlay.");
Require(RawTooltip.dismissAllToolTips(), "Tooltip dismissal lost its live owner.");
tester.pumpAndSettle();
Require(tester.text("Raw tooltip content").Count == 0, "Tooltip dismissal retained Overlay content.");
tooltipKey.currentState!.ensureTooltipVisible();
tester.pumpWidget(new SizedBox());
tester.pump(TimeSpan.FromSeconds(2));
Require(!RawTooltip.dismissAllToolTips() && tester.text("Raw tooltip content").Count == 0, "Unmounted tooltip installed late content or retained a listener.");
using var expansion = new ExpansibleController();
tester.pumpWidget(new MaterialApp(home: new Scaffold(body: new ExpansionTile(controller: expansion, maintainState: true,
    title: new Text("Expand"), children: [new TextField(controller: controller)]))));
expansion.expand(); tester.pumpAndSettle();
tester.tap(tester.byType<TextField>().Single());
tester.enterText(new("preserved", new(9, 9), null));
expansion.collapse(); tester.pump(TimeSpan.FromMilliseconds(300)); tester.pump();
expansion.expand(); tester.pump(TimeSpan.FromMilliseconds(300)); tester.pump();
Require(controller.text == "preserved" && expansion.isExpanded, "Expansion lost its maintained editing state.");
tester.pumpWidget(new SizedBox());
expansion.toggle(); tester.pump();
Require(!tester.HasTextClient, "Expansion unmount retained a text connection or controller listener.");
Console.WriteLine("PASS: Raw tooltip Auto/Overlay dismissal and pending-show unmount; Material expansion state and external controller disposal (synthetic).");
