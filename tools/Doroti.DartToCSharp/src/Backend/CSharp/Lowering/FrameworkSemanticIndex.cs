using System.Collections.ObjectModel;

namespace Doroti.DartToCSharp;

/// <summary>Immutable, compilation-wide symbol index shared by all lowering workers.</summary>
internal sealed class FrameworkSemanticIndex
{
    private readonly IReadOnlyDictionary<string, CoreResolvedDeclaration> _declarationsByCanonicalId;
    private readonly IReadOnlyDictionary<string, CoreResolvedMember> _membersByCanonicalId;
    private readonly IReadOnlyDictionary<string, CoreResolvedDeclaration> _memberOwnersByCanonicalId;
    private readonly IReadOnlyDictionary<(string Library, string Name), CoreResolvedDeclaration> _declarationsByLibraryAndName;
    private readonly IReadOnlyDictionary<string, CoreResolvedDeclaration[]> _declarationsByEmittedName;
    private readonly IReadOnlyDictionary<string, CoreResolvedDeclaration[]> _typeUsersBySimpleName;
    private readonly IReadOnlyDictionary<string, CoreResolvedDeclaration[]> _descendantsBySimpleName;
    private readonly HashSet<string> _enumGetterIds;

    public FrameworkSemanticIndex(IEnumerable<CoreResolvedDeclaration> declarations)
    {
        AllDeclarations = declarations
            .OrderBy(item => item.Element.CanonicalId, StringComparer.Ordinal)
            .ToArray();
        DeclarationsBySimpleName = new ReadOnlyDictionary<string, CoreResolvedDeclaration[]>(AllDeclarations
            .GroupBy(item => item.Name, StringComparer.Ordinal)
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(item => item.Element.CanonicalId, StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal));
        _declarationsByCanonicalId = new ReadOnlyDictionary<string, CoreResolvedDeclaration>(AllDeclarations
            .GroupBy(item => item.Element.CanonicalId, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal));
        _declarationsByLibraryAndName = new ReadOnlyDictionary<(string Library, string Name), CoreResolvedDeclaration>(AllDeclarations
            .GroupBy(item => (Library(item.Element.CanonicalId), item.Name))
            .ToDictionary(group => group.Key, group => group.First()));
        _declarationsByEmittedName = new ReadOnlyDictionary<string, CoreResolvedDeclaration[]>(AllDeclarations
            .GroupBy(item => EmittedName(Library(item.Element.CanonicalId), item.Name), StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(item => item.Element.CanonicalId, StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal));

        var members = AllDeclarations
            .SelectMany(declaration => declaration.Members.Select(member => (Declaration: declaration, Member: member)))
            .Where(item => !string.IsNullOrEmpty(item.Member.Element.CanonicalId))
            .ToArray();
        _membersByCanonicalId = new ReadOnlyDictionary<string, CoreResolvedMember>(members
            .GroupBy(item => item.Member.Element.CanonicalId, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First().Member, StringComparer.Ordinal));
        _memberOwnersByCanonicalId = new ReadOnlyDictionary<string, CoreResolvedDeclaration>(members
            .GroupBy(item => item.Member.Element.CanonicalId, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First().Declaration, StringComparer.Ordinal));
        _enumGetterIds = AllDeclarations
            .Where(item => item.Ast.Kind == CoreNodeKind.EnumDeclaration)
            .SelectMany(item => item.Members)
            .Where(item => item.IsGetter)
            .Select(item => item.Element.CanonicalId)
            .ToHashSet(StringComparer.Ordinal);

        var typeUsers = new Dictionary<string, List<CoreResolvedDeclaration>>(StringComparer.Ordinal);
        foreach (var declaration in AllDeclarations)
        {
            foreach (var type in DirectReferencedTypes(declaration)
                .SelectMany(TypeIdentifiers)
                .Where(item => item.Length > 0)
                .Distinct(StringComparer.Ordinal))
            {
                if (!typeUsers.TryGetValue(type, out var users)) typeUsers[type] = users = [];
                users.Add(declaration);
            }
        }
        _typeUsersBySimpleName = new ReadOnlyDictionary<string, CoreResolvedDeclaration[]>(typeUsers
            .ToDictionary(item => item.Key, item => item.Value.Distinct().OrderBy(value => value.Element.CanonicalId, StringComparer.Ordinal).ToArray(), StringComparer.Ordinal));
        var descendants = new Dictionary<string, CoreResolvedDeclaration[]>(StringComparer.Ordinal);
        foreach (var declaration in AllDeclarations)
        {
            var found = new Dictionary<string, CoreResolvedDeclaration>(StringComparer.Ordinal);
            var pending = new Queue<CoreResolvedDeclaration>(TypeUsers(declaration.Name));
            while (pending.Count > 0)
            {
                var candidate = pending.Dequeue();
                if (!found.TryAdd(candidate.Element.CanonicalId, candidate)) continue;
                foreach (var child in TypeUsers(candidate.Name)) pending.Enqueue(child);
            }
            descendants[declaration.Name] = found.Values.OrderBy(item => item.Element.CanonicalId, StringComparer.Ordinal).ToArray();
        }
        _descendantsBySimpleName = new ReadOnlyDictionary<string, CoreResolvedDeclaration[]>(descendants);
        MixinDeclarations = AllDeclarations.Where(item => item.Ast.Kind == CoreNodeKind.MixinDeclaration).ToArray();
    }

    public CoreResolvedDeclaration[] AllDeclarations { get; }
    public IReadOnlyDictionary<string, CoreResolvedDeclaration[]> DeclarationsBySimpleName { get; }
    public CoreResolvedDeclaration[] MixinDeclarations { get; }

    public CoreResolvedDeclaration? FindDeclarationByCanonicalId(string? canonicalId) =>
        canonicalId is not null && _declarationsByCanonicalId.TryGetValue(canonicalId, out var declaration) ? declaration : null;

    public CoreResolvedDeclaration? FindDeclaration(string? simpleName) =>
        simpleName is not null && DeclarationsBySimpleName.TryGetValue(SimpleTypeName(simpleName), out var matches) && matches.Length == 1
            ? matches[0]
            : null;

    public CoreResolvedDeclaration[] FindDeclarations(string? simpleName) =>
        simpleName is not null && DeclarationsBySimpleName.TryGetValue(SimpleTypeName(simpleName), out var matches) ? matches : [];

    public CoreResolvedMember? FindMember(string? canonicalId) =>
        canonicalId is not null && _membersByCanonicalId.TryGetValue(canonicalId, out var member) ? member : null;

    public CoreResolvedDeclaration? FindMemberOwner(CoreResolvedMember member) =>
        _memberOwnersByCanonicalId.TryGetValue(member.Element.CanonicalId, out var declaration) ? declaration : null;

    public CoreResolvedDeclaration? FindDeclaration(string library, string name) =>
        _declarationsByLibraryAndName.TryGetValue((library, name), out var declaration) ? declaration : null;

    public bool HasDeclaration(string library, string name) => _declarationsByLibraryAndName.ContainsKey((library, name));
    public CoreResolvedDeclaration? FindEmittedDeclaration(string emittedName) =>
        _declarationsByEmittedName.TryGetValue(emittedName, out var matches) ? matches[0] : null;
    public bool IsEnumGetter(string? memberId) => memberId is not null && _enumGetterIds.Contains(memberId);

    public CoreResolvedDeclaration[] TypeUsers(string typeName) =>
        _typeUsersBySimpleName.TryGetValue(SimpleTypeName(typeName), out var declarations) ? declarations : [];

    public CoreResolvedDeclaration[] Descendants(string typeName) =>
        _descendantsBySimpleName.TryGetValue(SimpleTypeName(typeName), out var declarations) ? declarations : [];

    internal static string SimpleTypeName(string value)
    {
        var result = value.Trim().TrimEnd('?');
        var generic = result.IndexOf('<');
        if (generic >= 0) result = result[..generic];
        var hash = result.LastIndexOf('#');
        if (hash >= 0) result = result[(hash + 1)..];
        var dot = result.LastIndexOf('.');
        if (dot >= 0) result = result[(dot + 1)..];
        return result;
    }

    private static IEnumerable<string> TypeIdentifiers(string value)
    {
        for (var index = 0; index < value.Length;)
        {
            if (!(char.IsLetter(value[index]) || value[index] == '_'))
            {
                index++;
                continue;
            }
            var start = index++;
            while (index < value.Length && (char.IsLetterOrDigit(value[index]) || value[index] == '_')) index++;
            yield return value[start..index];
        }
    }

    private static IEnumerable<string> DirectReferencedTypes(CoreResolvedDeclaration declaration) =>
        new[] { declaration.Element.Supertype }
            .Concat(declaration.Element.Mixins ?? [])
            .Concat(declaration.Element.Interfaces ?? [])
            .Concat(declaration.Members.SelectMany(member =>
                new[] { member.Element.Type, member.Element.ReturnType }
                    .Concat((member.Element.Parameters ?? []).Select(parameter => parameter.Type))))
            .Where(item => !string.IsNullOrWhiteSpace(item))!
            .Cast<string>();

    private static string Library(string canonicalId)
    {
        var marker = canonicalId.IndexOf('#');
        return marker < 0 ? string.Empty : canonicalId[..marker];
    }

    private static string EmittedName(string library, string name)
    {
        var safe = SafeIdentifier(name);
        if (safe.StartsWith('_'))
        {
            var file = library[(library.LastIndexOf('/') + 1)..];
            var stem = Path.GetFileNameWithoutExtension(file);
            var suffix = "__" + SafeIdentifier(stem).TrimStart('@');
            return safe.EndsWith(suffix, StringComparison.Ordinal) ? safe : safe + suffix;
        }
        var fileName = library[(library.LastIndexOf('/') + 1)..];
        return fileName.StartsWith('_') &&
            !library.EndsWith("/_background_isolate_binary_messenger_io.dart", StringComparison.Ordinal)
            ? safe + "Io"
            : safe;
    }

    private static string SafeIdentifier(string value)
    {
        var builder = new System.Text.StringBuilder(value.Length + 1);
        if (value.Length == 0 || !(char.IsLetter(value[0]) || value[0] == '_')) builder.Append('_');
        foreach (var character in value) builder.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
        var identifier = builder.ToString();
        return Microsoft.CodeAnalysis.CSharp.SyntaxFacts.GetKeywordKind(identifier) == Microsoft.CodeAnalysis.CSharp.SyntaxKind.None
            ? identifier
            : "@" + identifier;
    }
}
