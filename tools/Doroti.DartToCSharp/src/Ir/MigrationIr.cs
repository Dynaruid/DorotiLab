namespace Doroti.DartToCSharp;

public sealed record MigrationIr(
    string SchemaVersion,
    CompilerIdentity Identity,
    string IrVersion,
    string GenerationMode,
    string CompatibilityProfile,
    PackageGraph PackageGraph,
    MigrationIrInput[] Inputs,
    CompatibilityRule[] CompatibilityRules,
    ConverterOutput[] Outputs);

public sealed record MigrationIrInput(
    string Path,
    string Library,
    string[] Imports,
    string[] Directives,
    string[] SelectedSymbols,
    MigrationIrDeclaration[] Declarations,
    MigrationIrLibraryGraph? LibraryGraph = null);

public sealed record MigrationIrDeclaration(
    string Kind,
    string Name,
    int Offset,
    int Length,
    AnalyzerElement? Element,
    MigrationIrMember[] Members,
    MigrationIrNode? Ast = null);

public sealed record MigrationIrMember(
    string Kind,
    string Name,
    int Offset,
    int Length,
    AnalyzerElement? Element,
    MigrationIrStatement[] Statements,
    MigrationIrNode? Ast = null,
    bool IsStatic = false,
    bool IsFinal = false,
    bool IsConst = false,
    bool IsLate = false,
    bool IsAbstract = false,
    bool IsGetter = false,
    bool IsSetter = false,
    bool IsOperator = false,
    bool IsFactory = false);

public sealed record MigrationIrStatement(string Kind, int Offset, int Length, string Source);
public sealed record MigrationIrLibraryGraph(
    string Library,
    MigrationIrLibraryFragment[] Fragments,
    string[] Imports,
    MigrationIrLibraryImport[]? ImportDetails = null,
    string[]? AccessibleExtensions = null);
public sealed record MigrationIrLibraryFragment(string Uri, string[] Declarations, bool IsDefining = true, string? OwnerLibrary = null);
public sealed record MigrationIrLibraryImport(string Uri, string? Prefix, bool IsSynthetic = false);
public sealed record MigrationIrNode(
    string Kind,
    string AnalyzerKind,
    string Category,
    int Offset,
    int Length,
    string? StaticType,
    string? ElementId,
    Dictionary<string, string?> Properties,
    MigrationIrNode[] Children);
public sealed record CompatibilityRule(string Id, string SourceType, string TargetType, string SemanticScope, string BehaviorFixture);

public sealed record CompilerIdentity(
    string ConverterVersion,
    string DartSdkVersion,
    string DartAnalyzerVersion,
    string FlutterGitRevision,
    string IrSchemaVersion,
    string IrVersion,
    string LoweringRuleSetVersion,
    string EmitterVersion,
    string RuntimeBindingVersion,
    string WorkspaceId);

public sealed record PackageGraph(string SchemaVersion, string RootPackage, PackageGraphNode[] Packages);
public sealed record PackageGraphNode(string Name, string Version, string Source, string[] Dependencies);
