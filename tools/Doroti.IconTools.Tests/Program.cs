using System.Diagnostics;
using System.Text.Json;
using Mono.Cecil;
using Mono.Cecil.Cil;

// Run from the repository root: dotnet run --project tools/Doroti.IconTools.Tests -c Release
var root = Directory.GetCurrentDirectory();
var configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
var tool = Path.Combine(root, "tools", "Doroti.IconTools", "bin", configuration, "net10.0", "Doroti.IconTools.dll");
var artifacts = Path.Combine(root, "Doroti", "artifacts", "icon-subsetting-regression", Guid.NewGuid().ToString("N"));
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(20));
foreach (var material in new[] { true, false })
{
    var identity = material ? "Doroti.Icons.Material" : "Doroti.Icons.Cupertino";
    var typeName = material ? "Doroti.Material.Icons" : "Doroti.Cupertino.CupertinoIcons";
    var source = Path.Combine(root, "packages", identity, "bin", configuration, "net10.0", identity + ".dll");
    var expected = material
        ? new Dictionary<string, int> { ["event"] = 57918, ["explicit"] = 57927, ["lock"] = 58286, ["public"] = 58608 }
        : new Dictionary<string, int> { ["lock"] = 0xf4c8 };
    foreach (var scenario in new[] { "getters", "field", "alias-field" })
    {
        var directory = Path.Combine(artifacts, identity, scenario);
        Directory.CreateDirectory(directory);
        using var pack = AssemblyDefinition.ReadAssembly(source);
        var icons = pack.MainModule.GetType(typeName);
        Require(!icons.Fields.Any(f => f.IsPublic && f.IsStatic && f.FieldType.FullName == "Doroti.Framework.Widgets.IconData"),
            identity + " still exposes icon fields that cannot be subset as getters.");
        using var consumer = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Consumer", new Version(1, 0)), "Consumer", ModuleKind.Dll);
        var consumerType = new TypeDefinition("Test", "Consumer", TypeAttributes.Public, consumer.MainModule.TypeSystem.Object);
        consumer.MainModule.Types.Add(consumerType);
        var use = new MethodDefinition("UseIcons", MethodAttributes.Public | MethodAttributes.Static, consumer.MainModule.TypeSystem.Void);
        consumerType.Methods.Add(use);
        var il = use.Body.GetILProcessor();
        if (scenario == "getters")
        {
            foreach (var name in expected.Keys)
            {
                var getter = icons.Methods.Single(m => m.Name == "get_" + name);
                il.Emit(OpCodes.Call, consumer.MainModule.ImportReference(getter));
                il.Emit(OpCodes.Pop);
            }
        }
        else
        {
            // Model an older package's field, including one reached through an adaptive/alias getter.
            var iconData = icons.Methods.Single(m => m.Name == "get_lock").ReturnType;
            var field = new FieldDefinition("LegacyIcon", FieldAttributes.Public | FieldAttributes.Static, iconData);
            icons.Fields.Add(field);
            if (scenario == "field")
                il.Emit(OpCodes.Ldsfld, consumer.MainModule.ImportReference(field));
            else
            {
                var alias = new MethodDefinition("get_LegacyIcon", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.SpecialName, iconData);
                icons.Methods.Add(alias);
                var aliasIl = alias.Body.GetILProcessor();
                aliasIl.Emit(OpCodes.Ldsfld, field);
                aliasIl.Emit(OpCodes.Ret);
                il.Emit(OpCodes.Call, consumer.MainModule.ImportReference(alias));
            }
            il.Emit(OpCodes.Pop);
        }
        il.Emit(OpCodes.Ret);
        var packPath = Path.Combine(directory, identity + ".dll");
        var consumerPath = Path.Combine(directory, "Consumer.dll");
        pack.Write(packPath);
        consumer.Write(consumerPath);
        var inputs = Path.Combine(directory, "inputs.txt");
        File.WriteAllLines(inputs, [packPath, consumerPath]);
        var output = Path.Combine(directory, "output");
        var receipt = Path.Combine(output, "receipt.json");
        var log = await Run("--input-list", inputs, "--output-dir", output, "--receipt", receipt);
        using var report = JsonDocument.Parse(File.ReadAllText(receipt));
        var entry = report.RootElement.EnumerateArray().Single();
        using var staged = AssemblyDefinition.ReadAssembly(Path.Combine(output, identity + ".dll"));
        var font = staged.MainModule.Resources.OfType<EmbeddedResource>().Single(r => r.Name == identity + ".font").GetResourceData();
        if (scenario == "getters")
        {
            Require(entry.GetProperty("mode").GetString() == "subset", identity + " did not subset static properties.");
            using var inspection = JsonDocument.Parse(await Run("--inspect", Path.Combine(output, identity + ".dll")));
            var retained = inspection.RootElement.GetProperty("codePoints").EnumerateArray().Select(c => c.GetInt32()).ToHashSet();
            Require(retained.SetEquals(expected.Values), identity + " dropped or changed a reserved-name icon's cmap entry.");
            Require(font.Length < pack.MainModule.Resources.OfType<EmbeddedResource>().Single(r => r.Name == identity + ".font").GetResourceData().Length,
                identity + " kept the full font instead of a subset.");
        }
        else
        {
            Require(entry.GetProperty("mode").GetString() == "full" && log.Contains("DOROTIICON001"), identity + " silently discarded a field icon.");
            Require(font.SequenceEqual(pack.MainModule.Resources.OfType<EmbeddedResource>().Single(r => r.Name == identity + ".font").GetResourceData()),
                identity + " modified the font despite an unresolved field reference.");
        }
        Console.WriteLine($"PASS {identity}: {scenario}");
    }
}
Console.WriteLine("All 6 icon subsetting regressions passed.");

async Task<string> Run(params string[] arguments)
{
    var start = new ProcessStartInfo("dotnet") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
    start.ArgumentList.Add(tool);
    foreach (var argument in arguments) start.ArgumentList.Add(argument);
    using var process = Process.Start(start) ?? throw new InvalidOperationException("Could not start icon tools.");
    var stdout = process.StandardOutput.ReadToEndAsync(timeout.Token);
    var stderr = process.StandardError.ReadToEndAsync(timeout.Token);
    try { await process.WaitForExitAsync(timeout.Token); }
    catch (OperationCanceledException) { process.Kill(entireProcessTree: true); throw; }
    var output = await stdout;
    var errors = await stderr;
    Require(process.ExitCode == 0, output + errors);
    return output;
}

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
