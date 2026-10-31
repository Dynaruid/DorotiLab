using System.Reflection.PortableExecutable;
using System.Text.Json;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Doroti.IconTools;

try
{
    if (args is ["--inspect", var inspected])
    {
        using var assembly = AssemblyDefinition.ReadAssembly(inspected);
        using var image = new PEReader(File.OpenRead(inspected));
        var resource = assembly.MainModule.Resources.OfType<EmbeddedResource>().SingleOrDefault(r => r.Name.EndsWith(".font"));
        Console.WriteLine(JsonSerializer.Serialize(new
        {
            assembly = assembly.Name.Name,
            readyToRun = image.PEHeaders.CorHeader?.ManagedNativeHeaderDirectory.Size > 0,
            getters = assembly.MainModule.Types.SelectMany(t => t.Methods).Count(m => m.Name.StartsWith("get_")),
            fontBytes = resource?.GetResourceData().Length ?? 0,
            codePoints = resource is null ? [] : OpenTypeSubset.ReadCmap(resource.GetResourceData()).Keys.Order().ToArray(),
        }));
        return;
    }
    if (args is ["--extract-font", var extracted, var fontOutput])
    {
        using var assembly = AssemblyDefinition.ReadAssembly(extracted);
        File.WriteAllBytes(fontOutput, assembly.MainModule.Resources.OfType<EmbeddedResource>().Single(r => r.Name.EndsWith(".font")).GetResourceData());
        return;
    }
    if (args is not ["--input-list", var inputList, "--output-dir", var output, "--receipt", var receipt])
        throw new ArgumentException("Expected --input-list <file> --output-dir <directory> --receipt <file>.");

    var inputs = File.ReadAllLines(inputList).Where(File.Exists).Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    var packs = new Dictionary<string, AssemblyDefinition>(StringComparer.Ordinal);
    var consumers = new List<AssemblyDefinition>();
    using var resolver = new DefaultAssemblyResolver();
    resolver.AddSearchDirectory(Path.GetDirectoryName(typeof(object).Assembly.Location)!);
    foreach (var directory in inputs.Select(Path.GetDirectoryName).Distinct()) resolver.AddSearchDirectory(directory!);
    try
    {
        foreach (var input in inputs.Where(p => p.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
        {
            using var pe = new PEReader(File.OpenRead(input));
            if (!pe.HasMetadata) continue;
            var assembly = AssemblyDefinition.ReadAssembly(new MemoryStream(File.ReadAllBytes(input)), new ReaderParameters { AssemblyResolver = resolver });
            if (assembly.Name.Name is "Doroti.Icons.Material" or "Doroti.Icons.Cupertino")
            {
                if (pe.PEHeaders.CorHeader!.ManagedNativeHeaderDirectory.Size != 0)
                    throw new InvalidOperationException("Icon subsetting must run before ReadyToRun/AOT compilation.");
                packs.Add(assembly.Name.Name, assembly);
            }
            else consumers.Add(assembly);
        }
        Directory.CreateDirectory(output);
        var receipts = new List<object>();
        foreach (var (identity, pack) in packs)
        {
            var material = identity.EndsWith("Material");
            var iconType = material ? "Doroti.Material.Icons" : "Doroti.Cupertino.CupertinoIcons";
            var fontFamily = material ? "MaterialIcons" : "CupertinoIcons";
            bool BelongsToPack(TypeReference type) => type.FullName == iconType ||
                material && type.FullName == "Doroti.Material.PlatformAdaptiveIcons";
            bool NamesPack(string name) => name.Split(',')[0].Trim() == iconType ||
                material && name.Split(',')[0].Trim() == "Doroti.Material.PlatformAdaptiveIcons";
            var methods = pack.MainModule.Types.SelectMany(t => t.Methods).ToDictionary(m => m.FullName);
            var roots = new HashSet<string>();
            var fallback = new HashSet<string>();
            foreach (var attribute in consumers.SelectMany(a => a.CustomAttributes.Concat(
                AllTypes(a.MainModule.Types).SelectMany(t => t.CustomAttributes.Concat(t.Methods.SelectMany(m => m.CustomAttributes)))))
                .Where(a => a.AttributeType.FullName == "System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute"))
                if (attribute.ConstructorArguments.Any(a => a.Value is TypeReference type && BelongsToPack(type) ||
                    a.Value is string name && NamesPack(name)))
                    fallback.Add("icon preservation/reflection attribute");
            foreach (var method in consumers.SelectMany(a => AllTypes(a.MainModule.Types)).SelectMany(t => t.Methods).Where(m => m.HasBody))
            {
                var il = method.Body.Instructions;
                for (var i = 0; i < il.Count; i++)
                {
                    var instruction = il[i];
                    if (instruction.Operand is MethodReference called && BelongsToPack(called.DeclaringType))
                        roots.Add(called.FullName);
                    if (instruction.Operand is FieldReference field && BelongsToPack(field.DeclaringType))
                        fallback.Add("icon field access in " + method.FullName);
                    if (instruction.Operand is TypeReference token && BelongsToPack(token))
                        fallback.Add("icon type metadata/reflection in " + method.FullName);
                    if (instruction.OpCode == OpCodes.Ldstr && instruction.Operand is string text && NamesPack(text))
                        fallback.Add("icon type lookup by name in " + method.FullName);
                    if (instruction.OpCode == OpCodes.Newobj && instruction.Operand is MethodReference constructor &&
                        constructor.DeclaringType.FullName == "Doroti.Framework.Widgets.IconData")
                    {
                        var count = constructor.Parameters.Count;
                        var arguments = il.Take(i).Where(p => p.OpCode.Code is not (Code.Conv_I8 or Code.Nop)).TakeLast(count).ToArray();
                        // A constant unrelated/null font is safe. Unknown stack expressions preserve the full face.
                        var constant = arguments.Length == count && arguments.All(IsConstant);
                        if (!constant || arguments[1].OpCode == OpCodes.Ldstr &&
                            ((string)arguments[1].Operand).Contains(fontFamily, StringComparison.Ordinal))
                            fallback.Add("runtime IconData construction in " + method.FullName);
                    }
                }
            }
            var resource = pack.MainModule.Resources.OfType<EmbeddedResource>().SingleOrDefault(r => r.Name == identity + ".font");
            if (resource is null)
            {
                // Stage even an assembly whose resource was already removed by a previous step.
                var emptyDestination = Path.Combine(output, identity + ".dll");
                pack.Write(emptyDestination + ".icons.tmp");
                File.Move(emptyDestination + ".icons.tmp", emptyDestination, true);
                receipts.Add(new { assembly = identity, mode = "unused", inputBytes = 0, outputBytes = 0 });
                continue;
            }
            var original = resource.GetResourceData();
            var selected = new SortedSet<int>();
            var visited = new HashSet<string>();
            void Visit(string name)
            {
                if (!visited.Add(name) || !methods.TryGetValue(name, out var method) || !method.HasBody) return;
                var il = method.Body.Instructions;
                if (il.Any(i => i.Operand is FieldReference field && BelongsToPack(field.DeclaringType)))
                    fallback.Add("icon field access in " + method.FullName);
                if (method.Name.StartsWith("get_") && method.ReturnType.FullName == "Doroti.Framework.Widgets.IconData")
                {
                    var literal = il.Select(Integer).FirstOrDefault(v => v is not null);
                    if (literal is not null) selected.Add(checked((int)literal.Value));
                }
                foreach (var call in il.Select(i => i.Operand).OfType<MethodReference>())
                    if (methods.ContainsKey(call.FullName)) Visit(call.FullName);
            }
            foreach (var root in roots) Visit(root);
            if (fallback.Count > 0)
            {
                Console.WriteLine($"warning DOROTIICON001: Preserving full {fontFamily} font: {fallback.First()}. Use static icon properties for automatic subsetting.");
                receipts.Add(new { assembly = identity, mode = "full", reasons = fallback.ToArray(), inputBytes = original.Length, outputBytes = original.Length });
                // Publish must stage the full assembly too, even when no rewriting is needed.
                var fullDestination = Path.Combine(output, identity + ".dll");
                pack.Write(fullDestination + ".icons.tmp");
                File.Move(fullDestination + ".icons.tmp", fullDestination, true);
                continue;
            }
            byte[]? subset = selected.Count == 0 ? null : OpenTypeSubset.Create(original, selected);
            pack.MainModule.Resources.Remove(resource);
            if (subset is not null) pack.MainModule.Resources.Add(new EmbeddedResource(resource.Name, resource.Attributes, subset));
            var destination = Path.Combine(output, identity + ".dll");
            var temporary = destination + ".icons.tmp";
            pack.Write(temporary);
            File.Move(temporary, destination, true);
            receipts.Add(new { assembly = identity, mode = selected.Count == 0 ? "unused" : "subset",
                inputBytes = original.Length, outputBytes = subset?.Length ?? 0, codePoints = selected.ToArray() });
            Console.WriteLine($"Doroti icons: {fontFamily}, {selected.Count} codepoints, {original.Length:N0} -> {subset?.Length ?? 0:N0} bytes.");
        }
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(receipt))!);
        File.WriteAllText(receipt, JsonSerializer.Serialize(receipts, new JsonSerializerOptions { WriteIndented = true }));
    }
    finally
    {
        foreach (var assembly in packs.Values.Concat(consumers)) assembly.Dispose();
    }
}
catch (Exception error)
{
    Console.Error.WriteLine("error DOROTIICON002: " + error.Message);
    Environment.ExitCode = 1;
}

static IEnumerable<TypeDefinition> AllTypes(IEnumerable<TypeDefinition> types) =>
    types.SelectMany(t => new[] { t }.Concat(AllTypes(t.NestedTypes)));

static bool IsConstant(Instruction instruction) => instruction.OpCode.Code is
    Code.Ldnull or Code.Ldstr or Code.Ldc_I8 or Code.Ldc_I4 or Code.Ldc_I4_S or
    Code.Ldc_I4_0 or Code.Ldc_I4_1 or Code.Ldc_I4_2 or Code.Ldc_I4_3 or
    Code.Ldc_I4_4 or Code.Ldc_I4_5 or Code.Ldc_I4_6 or Code.Ldc_I4_7 or Code.Ldc_I4_8 or Code.Ldc_I4_M1;

static long? Integer(Instruction instruction) => instruction.OpCode.Code switch
{
    Code.Ldc_I8 => (long)instruction.Operand,
    Code.Ldc_I4 => (int)instruction.Operand,
    Code.Ldc_I4_S => (sbyte)instruction.Operand,
    Code.Ldc_I4_M1 => -1,
    Code.Ldc_I4_0 => 0, Code.Ldc_I4_1 => 1, Code.Ldc_I4_2 => 2, Code.Ldc_I4_3 => 3,
    Code.Ldc_I4_4 => 4, Code.Ldc_I4_5 => 5, Code.Ldc_I4_6 => 6, Code.Ldc_I4_7 => 7, Code.Ldc_I4_8 => 8,
    _ => null,
};
