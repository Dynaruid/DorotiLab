namespace Doroti.DartToCSharp;

[Flags]
public enum CompilerDumpStage
{
    None = 0,
    AnalyzerProtocol = 1,
    DartIr = 2,
    CoreIr = 4,
    CSharpIr = 8,
    All = AnalyzerProtocol | DartIr | CoreIr | CSharpIr,
}

public sealed record CompilerDumpOptions(string Directory, CompilerDumpStage Stages = CompilerDumpStage.All);

internal sealed record CompilerDumpInput(
    string Source,
    string Library,
    AnalyzerOutput AnalyzerOutput,
    DartResolvedDeclaration[] DartDeclarations,
    CoreResolvedDeclaration[] CoreDeclarations,
    string? GeneratedFile,
    string? GeneratedCode,
    SourceMapEntry[] Mappings);
