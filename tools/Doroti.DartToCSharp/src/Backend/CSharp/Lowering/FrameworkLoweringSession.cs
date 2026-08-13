namespace Doroti.DartToCSharp;

/// <summary>Mutable state owned by one library lowering worker.</summary>
internal sealed class FrameworkLoweringSession
{
    public string? ActiveSourceLibrary { get; set; }
    public CoreResolvedDeclaration? ActiveDeclaration { get; set; }
    public CoreResolvedDeclaration? ActiveDonorDeclaration { get; set; }
    public bool EmittingAssignmentLeft { get; set; }
    public string? ExplicitThisExpression { get; set; }
    public CoreResolvedDeclaration? ExplicitEnumDeclaration { get; set; }
    public string? ActivePatternExtensionTypeName { get; set; }
    public bool SuppressSyntheticPatternDesignation { get; set; }
    public List<CsSyntaxDocument>? PatternGuards { get; set; }
    public IReadOnlyDictionary<string, string> TypeParameterSubstitutions { get; set; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
    public IReadOnlyDictionary<string, string> ActiveMemberContractSubstitutions { get; set; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
    public IReadOnlySet<string> ActiveMethodTypeParameters { get; set; } =
        new HashSet<string>(StringComparer.Ordinal);
    public string? ActiveFunctionReturnType { get; set; }
    public string? ContextualLambdaReturnType { get; set; }
}
