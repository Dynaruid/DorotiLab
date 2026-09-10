using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.Json;

// Host-side inspection: never loads or executes application assemblies, and
// therefore works independently of their runtime and platform dependencies.
var records = new List<object>();
var total = 0;
var expectZero = args.Contains("--expect-zero");
var files = args.Where(arg => arg != "--expect-zero").ToArray();
if (files.Length == 0 || files.Any(file => !File.Exists(file)))
{
    Console.Error.WriteLine("Usage: il-audit [--expect-zero] <existing managed assemblies...>. Missing inputs cannot pass.");
    return 2;
}
foreach (var file in files.Order(StringComparer.Ordinal))
{
    using var stream = File.OpenRead(file);
    using var pe = new PEReader(stream);
    if (!pe.HasMetadata)
    {
        Console.Error.WriteLine($"Expected managed PE metadata: {file}");
        return 2;
    }
    var metadata = pe.GetMetadataReader();
    var provider = new TypeNames();
    var sites = new List<object>();
    foreach (var handle in metadata.TypeDefinitions)
    {
        var type = metadata.GetTypeDefinition(handle);
        foreach (var fieldHandle in type.GetFields())
        {
            var field = metadata.GetFieldDefinition(fieldHandle);
            var signature = field.DecodeSignature(provider, (object?)null);
            if (!signature.StartsWith("System.Runtime.CompilerServices.CallSite`1<", StringComparison.Ordinal)) continue;
            sites.Add(new { owner = provider.GetTypeFromDefinition(metadata, handle, 0),
                field = metadata.GetString(field.Name), signature });
        }
    }
    total += sites.Count;
    records.Add(new { assembly = metadata.GetString(metadata.GetAssemblyDefinition().Name),
        path = Path.GetFullPath(file), sha256 = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(file))).ToLowerInvariant(),
        callSiteFieldCount = sites.Count, sites });
}
Console.WriteLine(JsonSerializer.Serialize(new { schemaVersion = "doroti.dlr-il-audit/v1", totalCallSiteFields = total, assemblies = records }, new JsonSerializerOptions { WriteIndented = true }));
return expectZero && total != 0 ? 1 : 0;

sealed class TypeNames : ISignatureTypeProvider<string, object?>
{
    public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
    {
        var type = reader.GetTypeDefinition(handle);
        var parent = type.GetDeclaringType();
        return (parent.IsNil ? reader.GetString(type.Namespace) : GetTypeFromDefinition(reader, parent, 0)) + "." + reader.GetString(type.Name);
    }
    public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
    {
        var type = reader.GetTypeReference(handle);
        return reader.GetString(type.Namespace) + "." + reader.GetString(type.Name);
    }
    public string GetTypeFromSpecification(MetadataReader reader, object? context, TypeSpecificationHandle handle, byte rawTypeKind) => reader.GetTypeSpecification(handle).DecodeSignature(this, context);
    public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments) => genericType + "<" + string.Join(",", typeArguments) + ">";
    public string GetArrayType(string elementType, ArrayShape shape) => elementType + "[" + new string(',', shape.Rank - 1) + "]";
    public string GetByReferenceType(string elementType) => elementType + "&";
    public string GetFunctionPointerType(MethodSignature<string> signature) => "fnptr";
    public string GetGenericMethodParameter(object? context, int index) => "!!" + index;
    public string GetGenericTypeParameter(object? context, int index) => "!" + index;
    public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) => unmodifiedType;
    public string GetPinnedType(string elementType) => elementType;
    public string GetPointerType(string elementType) => elementType + "*";
    public string GetPrimitiveType(PrimitiveTypeCode typeCode) => typeCode.ToString();
    public string GetSZArrayType(string elementType) => elementType + "[]";
}
