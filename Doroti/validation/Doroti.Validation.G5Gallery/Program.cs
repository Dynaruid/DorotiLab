using System.Text.Json;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using Cupertino = Doroti.Generated.Framework.Cupertino;
using Foundation = Doroti.Generated.Framework.Foundation;
using Material = Doroti.Generated.Framework.Material;
using Painting = Doroti.Generated.Framework.Painting;
using Widgets = Doroti.Generated.Framework.Widgets;
using IOPath = System.IO.Path;

var failures = new List<string>();
var traces = new Dictionary<string, string[]>(StringComparer.Ordinal);

RunShell("material", CreateMaterialShell(), "G5-4 Material", 0xff6750a4L);
RunShell("cupertino", CreateCupertinoShell(), "G5-4 Cupertino", 0xff007affL);
RunBehavior();
RunInput();
RunSemantics();

var root = FindDorotiRoot(Environment.CurrentDirectory);
var evidencePath = args.Length == 0
    ? IOPath.Combine(root, "migration", "flutter-framework", "g5-4-gallery-differential.json")
    : IOPath.GetFullPath(args[0]);
var evidence = new
{
    schemaVersion = "doroti.g5-4-gallery-differential/v1",
    milestone = "G5-4",
    capturedAtUtc = DateTimeOffset.UtcNow,
    status = failures.Count == 0 ? "PASS" : "FAIL",
    evidenceClass = "syntheticContract",
    eligibleForLivePass = false,
    liveWidgetLifecycle = "notVerified",
    reference = "Flutter 56b8e1a8 public constructor, property, callback, input and semantics contracts",
    shell = "one shared SourcePortedGalleryShell factory with Material and Cupertino branches",
    dimensions = new[] { "behavior", "visual", "input", "semantics" },
    traces,
    failures
};
Directory.CreateDirectory(IOPath.GetDirectoryName(evidencePath)!);
File.WriteAllText(evidencePath, JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true }) + "\n");

if (failures.Count != 0)
{
    Console.Error.WriteLine(string.Join(Environment.NewLine, failures));
    return 1;
}

Console.WriteLine("G5-4-GALLERY-DIFFERENTIAL-PASS");
return 0;

void RunShell(string kind, Widgets.Widget shell, string title, long colorValue)
{
    switch (shell)
    {
        case Material.MaterialApp material:
            Require(material.title == title, $"{kind}: title drifted");
            Require(material.home is Widgets.Text, $"{kind}: home is not source-ported Text");
            Require(material.color?.value == colorValue, $"{kind}: visual color drifted");
            traces[$"visual:{kind}"] = [material.title!, material.color!.value.ToString("x8"), material.home!.GetType().Name];
            break;
        case Cupertino.CupertinoApp cupertino:
            Require(cupertino.title == title, $"{kind}: title drifted");
            Require(cupertino.home is Widgets.Text, $"{kind}: home is not source-ported Text");
            Require(cupertino.color?.value == colorValue, $"{kind}: visual color drifted");
            traces[$"visual:{kind}"] = [cupertino.title!, cupertino.color!.value.ToString("x8"), cupertino.home!.GetType().Name];
            break;
        default:
            failures.Add($"{kind}: unsupported shell {shell.GetType().FullName}");
            break;
    }
}

void RunBehavior()
{
    var materialPressed = 0;
    var cupertinoPressed = 0;
    var materialSwitchValue = false;
    var cupertinoSwitchValue = false;
    var label = new Widgets.Text("Action");
    var materialButton = new Material.ElevatedButton(onPressed: () => materialPressed++, clipBehavior: Clip.none, child: label);
    var cupertinoButton = new Cupertino.CupertinoButton(
        child: label,
        disabledColor: new Color(0xffd1d1d6L),
        alignment: Painting.Alignment.center,
        onPressed: () => cupertinoPressed++);
    var materialSwitch = new Material.Switch(value: false, onChanged: value => materialSwitchValue = value);
    var cupertinoSwitch = new Cupertino.CupertinoSwitch(value: false, onChanged: value => cupertinoSwitchValue = value);
    materialButton.onPressed!();
    cupertinoButton.onPressed!();
    materialSwitch.onChanged!(true);
    cupertinoSwitch.onChanged!(true);
    Require(materialPressed == 1 && cupertinoPressed == 1, "behavior: button callback differential failed");
    Require(materialSwitchValue && cupertinoSwitchValue, "behavior: switch callback differential failed");
    traces["behavior"] = [$"material:{materialPressed}/{materialSwitchValue}", $"cupertino:{cupertinoPressed}/{cupertinoSwitchValue}"];
}

void RunInput()
{
    var materialText = "";
    var cupertinoText = "";
    var material = new Material.TextField(
        groupId: "gallery",
        decoration: new Material.InputDecoration(),
        readOnly: false,
        obscureText: false,
        onChanged: value => materialText = value);
    var cupertino = new Cupertino.CupertinoTextField(
        groupId: "gallery",
        decoration: new Painting.BoxDecoration(),
        padding: Painting.EdgeInsets.zero,
        placeholderStyle: new Painting.TextStyle(),
        readOnly: false,
        obscureText: false,
        onChanged: value => cupertinoText = value);
    material.onChanged!("doroti");
    cupertino.onChanged!("doroti");
    Require(materialText == "doroti" && cupertinoText == "doroti", "input: text callback differential failed");
    Require(!material.readOnly && !cupertino.readOnly, "input: editable contract drifted");
    traces["input"] = [$"material:{materialText}/{material.readOnly}", $"cupertino:{cupertinoText}/{cupertino.readOnly}"];
}

void RunSemantics()
{
    var material = new Doroti.Generated.Framework.Semantics.SemanticsProperties(label: "Material action", button: true);
    var cupertino = new Doroti.Generated.Framework.Semantics.SemanticsProperties(label: "Cupertino action", button: true);
    Require(material.label == "Material action" && material.button == true, "semantics: Material contract drifted");
    Require(cupertino.label == "Cupertino action" && cupertino.button == true, "semantics: Cupertino contract drifted");
    traces["semantics"] = [$"{material.label}/{material.button}", $"{cupertino.label}/{cupertino.button}"];
}

Material.MaterialApp CreateMaterialShell() => new(
    home: new Widgets.Text("Material gallery"),
    routes: new DartMap<string, Func<Widgets.BuildContext, Widgets.Widget>>(),
    navigatorObservers: [],
    supportedLocales: [new Locale("en")],
    title: "G5-4 Material",
    color: new Color(0xff6750a4L),
    debugShowCheckedModeBanner: false);

Cupertino.CupertinoApp CreateCupertinoShell() => new(
    home: new Widgets.Text("Cupertino gallery"),
    routes: new DartMap<string, Func<Widgets.BuildContext, Widgets.Widget>>(),
    navigatorObservers: [],
    supportedLocales: [new Locale("en")],
    title: "G5-4 Cupertino",
    color: new Color(0xff007affL),
    debugShowCheckedModeBanner: false);

void Require(bool condition, string message)
{
    if (!condition) failures.Add(message);
}

static string FindDorotiRoot(string start)
{
    for (var directory = new DirectoryInfo(start); directory is not null; directory = directory.Parent)
    {
        if (File.Exists(IOPath.Combine(directory.FullName, "Doroti.Product.slnx"))) return directory.FullName;
        var nested = IOPath.Combine(directory.FullName, "Doroti", "Doroti.Product.slnx");
        if (File.Exists(nested)) return IOPath.GetDirectoryName(nested)!;
    }
    throw new DirectoryNotFoundException("Could not locate the Doroti root.");
}
