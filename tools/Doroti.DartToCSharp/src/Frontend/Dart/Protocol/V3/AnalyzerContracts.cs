namespace Doroti.DartToCSharp;

public sealed record AnalyzerOutput(
    string SchemaVersion,
    string LibraryUri,
    string[] Imports,
    string[] Directives,
    AnalyzerDeclaration[] Declarations,
    AnalyzerDiagnostic[] Diagnostics,
    string? AnalysisMode = null,
    AnalyzerLibraryGraph? LibraryGraph = null);

public sealed record AnalyzerDeclaration(
    string Kind,
    string Name,
    int Offset,
    int Length,
    string Source,
    AnalyzerElement? Element,
    AnalyzerMember[] Members,
    AnalyzerAstNode? Ast = null);

public sealed record AnalyzerMember(
    string Kind,
    string Name,
    int Offset,
    int Length,
    string Source,
    AnalyzerElement? Element,
    AnalyzerStatement[] Statements,
    AnalyzerAstNode? Ast = null,
    bool IsStatic = false,
    bool IsFinal = false,
    bool IsConst = false,
    bool IsLate = false,
    bool IsAbstract = false,
    bool IsGetter = false,
    bool IsSetter = false,
    bool IsOperator = false,
    bool IsFactory = false);

public sealed record AnalyzerStatement(string Kind, int Offset, int Length, string Source);

public sealed record AnalyzerElement(
    string Kind,
    string Name,
    string CanonicalId,
    bool IsDeprecated,
    string? Type,
    string? Supertype,
    string[]? Mixins,
    string[]? Interfaces,
    AnalyzerTypeParameter[]? TypeParameters,
    string? ReturnType,
    AnalyzerParameter[]? Parameters,
    bool IsAbstract = false,
    bool IsPrivate = false);

public sealed record AnalyzerTypeParameter(string Name, string? Bound);
public sealed record AnalyzerParameter(
    string Name,
    string Type,
    string Kind,
    string? DefaultValue,
    bool IsInitializingFormal = false,
    bool IsSuperFormal = false);
public sealed record AnalyzerDiagnostic(string Code, string Severity, int Offset, int Length, string Message);

public sealed record AnalyzerLibraryGraph(
    string Library,
    AnalyzerLibraryFragment[] Fragments,
    string[] Imports,
    AnalyzerLibraryImport[]? ImportDetails = null,
    string[]? AccessibleExtensions = null);

public sealed record AnalyzerLibraryFragment(
    string Uri,
    string[] Declarations,
    bool IsDefining = true,
    string? OwnerLibrary = null);

public sealed record AnalyzerLibraryImport(string Uri, string? Prefix, bool IsSynthetic = false);

public sealed record AnalyzerAstNode(
    string Kind,
    string AnalyzerKind,
    string Category,
    int Offset,
    int Length,
    string? StaticType,
    string? ElementId,
    Dictionary<string, string?> Properties,
    AnalyzerAstNode[] Children);
