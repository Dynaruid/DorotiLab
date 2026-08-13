namespace Doroti.DartToCSharp;

public sealed record FrameworkCoverageDocument(
    string SchemaVersion,
    string Status,
    CompilerIdentity Identity,
    FrameworkCoverageInput[] Inputs,
    int DeclarationCount,
    int MemberCount,
    int AstNodeCount,
    int UnclassifiedAstNodeCount,
    int SilentOmissionCount,
    int GeneratedCompileErrorCount);

public sealed record FrameworkCoverageInput(
    string Source,
    string Library,
    string AnalysisMode,
    string[] DependencyPath,
    string[] DeclarationElementIds,
    string[] MemberElementIds,
    FrameworkAstClassification[] Classifications);

public sealed record FrameworkAstClassification(string Category, string Kind, int Count);
