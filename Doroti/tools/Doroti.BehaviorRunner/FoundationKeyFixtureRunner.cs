using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;
using Doroti.Tooling;

namespace Doroti.BehaviorRunner;

public static class FoundationKeyFixtureRunner
{
    public static FoundationKeyResultDocument Run(
        string fixturePath,
        string outputPath,
        string assemblyPath,
        string implementationNamespace,
        string runner)
    {
        var fixture = ArtifactFiles.ReadJson<FoundationKeyFixtureDocument>(fixturePath);
        if (fixture.SchemaVersion != "doroti.foundation-key-fixture/v1")
        {
            throw new InvalidDataException($"Unsupported foundation key fixture schema {fixture.SchemaVersion}.");
        }

        var context = new AssemblyLoadContext($"doroti-key-fixture-{Guid.NewGuid():N}", isCollectible: false);
        context.Resolving += (_, name) => AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(item => string.Equals(item.GetName().Name, name.Name, StringComparison.Ordinal));
        using var assemblyStream = new MemoryStream(File.ReadAllBytes(Path.GetFullPath(assemblyPath)));
        var assembly = context.LoadFromStream(assemblyStream);
        var valueKey = assembly.GetType($"{implementationNamespace}.ValueKey`1", throwOnError: true)!;
        var uniqueKey = assembly.GetType($"{implementationNamespace}.UniqueKey", throwOnError: true)!;
        var results = fixture.Cases
            .OrderBy(item => item.Id, StringComparer.Ordinal)
            .Select(item => new FoundationKeyCaseResult(item.Id, RunCase(item.Operation, valueKey, uniqueKey)))
            .ToArray();
        var document = new FoundationKeyResultDocument(
            "doroti.foundation-key-result/v1",
            runner,
            fixture.FlutterGitRevision,
            results);
        ArtifactFiles.WriteJson(outputPath, document);
        return document;
    }

    private static bool RunCase(string operation, Type openValueKey, Type uniqueKey)
    {
        var intKey = openValueKey.MakeGenericType(typeof(int));
        var first = Activator.CreateInstance(intKey, 7)!;
        var second = Activator.CreateInstance(intKey, 7)!;
        return operation switch
        {
            "value-equal" => first.Equals(second),
            "value-different" => first.Equals(Activator.CreateInstance(intKey, 8)),
            "generic-type-isolation" => first.Equals(Activator.CreateInstance(openValueKey.MakeGenericType(typeof(string)), "7")),
            "subclass-isolation" => first.Equals(Activator.CreateInstance(CreatePrivateSubclass(intKey), 7)),
            "equal-hash" => first.GetHashCode() == second.GetHashCode(),
            "unique-same-instance" => SameUniqueInstance(uniqueKey),
            "unique-distinct" => Activator.CreateInstance(uniqueKey)!.Equals(Activator.CreateInstance(uniqueKey)),
            _ => throw new InvalidDataException($"Unknown foundation key operation {operation}."),
        };
    }

    private static bool SameUniqueInstance(Type uniqueKey)
    {
        var key = Activator.CreateInstance(uniqueKey)!;
        return key.Equals(key);
    }

    private static Type CreatePrivateSubclass(Type baseType)
    {
        var assembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName($"Doroti.FoundationKeyFixture.{Guid.NewGuid():N}"),
            AssemblyBuilderAccess.Run);
        var type = assembly.DefineDynamicModule("main").DefineType(
            "PrivateValueKey",
            TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.NotPublic,
            baseType);
        var baseConstructor = baseType.GetConstructor([typeof(int)])
            ?? throw new MissingMethodException(baseType.FullName, ".ctor(int)");
        var constructor = type.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            [typeof(int)]);
        var il = constructor.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Call, baseConstructor);
        il.Emit(OpCodes.Ret);
        return type.CreateType()!;
    }
}

public sealed record FoundationKeyFixtureDocument(
    string SchemaVersion,
    string FlutterGitRevision,
    FoundationKeyCase[] Cases);

public sealed record FoundationKeyCase(string Id, string Operation);

public sealed record FoundationKeyResultDocument(
    string SchemaVersion,
    string Runner,
    string FlutterGitRevision,
    FoundationKeyCaseResult[] Results);

public sealed record FoundationKeyCaseResult(string Id, bool Value);
