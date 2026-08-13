using Doroti.Tooling;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Doroti.DartToCSharp;

public sealed record AdoptionReportDocument(
    string SchemaVersion,
    string WorkspaceId,
    string Mode,
    PortSource Source,
    CompilerIdentity CompilerIdentity,
    string Library,
    string Symbol,
    string CandidatePath,
    string CandidateSha256,
    string GeneratedBaseSha256,
    PortManualInput[] ManualInputs,
    string[] RequiredFixtures,
    bool ProductSourceModified);

public sealed record AdoptionBundle(string Path, AdoptionReportDocument Report);

/// <summary>Creates a review-only runtime adoption candidate and never writes product source.</summary>
public sealed class PortAdoption
{
    public AdoptionBundle Create(
        string manifestPath,
        string workspaceRoot,
        string symbol,
        string outputDirectory,
        string? cacheDirectory = null,
        string? library = null)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw Error("DORPORT011", "Adoption requires a symbol.");
        }
        var workspace = new PortCompiler().Compile(manifestPath, workspaceRoot, cacheDirectory);
        if (workspace.State.Mode != PortSchemas.RuntimeAdoption)
        {
            throw Error("DORPORT011", $"Adoption requires port mode '{PortSchemas.RuntimeAdoption}'.");
        }
        var ir = ArtifactFiles.ReadJson<MigrationIr>(Path.Combine(workspace.GeneratedBasePath, "migration-ir.json"));
        var matches = ir.Inputs
            .Where(input => library is null || input.Library == library)
            .SelectMany(input => input.Declarations
                .Where(declaration => declaration.Name == symbol)
                .Select(declaration => (Input: input, Declaration: declaration)))
            .ToArray();
        if (matches.Length != 1)
        {
            throw Error("DORPORT011", $"Adoption symbol must resolve exactly once: {symbol}; found {matches.Length}.");
        }
        var match = matches[0];
        var output = workspace.Report.Outputs.Single(item => item.Input == match.Input.Path);
        var generatedPath = Path.Combine(workspace.GeneratedBasePath, output.Output);
        var candidateName = $"{SafeName(symbol)}.adoption.cs";
        AdoptionReportDocument? report = null;
        ReviewBundlePublisher.Publish(outputDirectory, staging =>
        {
            var candidateRelative = $"candidate/{candidateName}";
            var candidatePath = Path.Combine(staging, candidateRelative);
            var candidate = ExtractCandidate(
                File.ReadAllText(generatedPath),
                symbol,
                match.Input.Library,
                workspace.State.Source,
                output.Sha256);
            ArtifactFiles.WriteUtf8(candidatePath, candidate);
            CopyManualSnapshot(workspace.Path, staging, workspace.State.ManualInputs);
            report = new(
                PortSchemas.Adoption,
                workspace.State.WorkspaceId,
                workspace.State.Mode,
                workspace.State.Source,
                workspace.State.CompilerIdentity,
                match.Input.Library,
                symbol,
                candidateRelative,
                ArtifactFiles.Sha256(candidatePath),
                output.Sha256,
                workspace.State.ManualInputs,
                workspace.State.RequiredFixtures,
                false);
            ArtifactFiles.WriteJson(Path.Combine(staging, "adoption-report.json"), report);
            ArtifactFiles.WriteJson(
                Path.Combine(staging, "provenance.json"),
                new PortProvenanceDocument(
                    PortSchemas.Provenance,
                    workspace.State.WorkspaceId,
                    workspace.State.Source,
                    workspace.State.CompilerIdentity,
                    [new(
                        candidateRelative,
                        PortSchemas.AdoptedProduct,
                        report.CandidateSha256,
                        match.Input.Path,
                        match.Input.Library,
                        symbol,
                        null,
                        output.Sha256)]));
            ArtifactFiles.WriteUtf8(
                Path.Combine(staging, "review.md"),
                $"# Adoption review: {symbol}\n\n" +
                $"- Upstream: `{match.Input.Library}` at `{workspace.State.Source.Revision}`\n" +
                $"- License: `{workspace.State.Source.License}`\n" +
                $"- Generated base: `{output.Sha256}`\n" +
                $"- Candidate: `{candidateRelative}`\n" +
                $"- Required fixtures: {string.Join(", ", workspace.State.RequiredFixtures.Select(item => $"`{item}`"))}\n\n" +
                "This bundle is review-only. No product source was modified. Promotion requires API, behavior and provenance review.\n");
        });
        return new(Path.GetFullPath(outputDirectory), report!);
    }

    private static string ExtractCandidate(
        string generated,
        string symbol,
        string library,
        PortSource source,
        string generatedBaseSha256)
    {
        var root = CSharpSyntaxTree.ParseText(generated).GetCompilationUnitRoot();
        var targets = root.DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>()
            .Where(node => node.Identifier.ValueText == symbol &&
                !node.Ancestors().OfType<BaseTypeDeclarationSyntax>().Any())
            .ToArray();
        if (targets.Length != 1)
        {
            throw Error("DORPORT011", $"Generated adoption candidate must be a unique type declaration: {symbol}.");
        }
        var target = targets[0].WithoutLeadingTrivia().WithoutTrailingTrivia();
        MemberDeclarationSyntax member = target;
        if (targets[0].Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault() is { } containingNamespace)
        {
            member = containingNamespace.WithMembers(SyntaxFactory.SingletonList(member))
                .WithoutLeadingTrivia()
                .WithoutTrailingTrivia();
        }
        var candidate = root.WithMembers(SyntaxFactory.SingletonList(member)).NormalizeWhitespace().ToFullString();
        var header =
            "// Doroti runtime adoption review candidate.\n" +
            $"// Upstream-Library: {library}\n" +
            $"// Upstream-Revision: {source.Revision}\n" +
            $"// Upstream-License: {source.License}\n" +
            $"// Adoption-Base-SHA256: {generatedBaseSha256}\n" +
            "// Product ownership begins only after explicit review and promotion.\n";
        return header + candidate + "\n";
    }

    private static void CopyManualSnapshot(string workspacePath, string staging, IEnumerable<PortManualInput> inputs)
    {
        foreach (var input in inputs)
        {
            var source = Path.Combine(workspacePath, input.SnapshotPath);
            var target = Path.Combine(staging, input.SnapshotPath);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(source, target, overwrite: false);
        }
    }

    private static string SafeName(string value) => string.Concat(value.Select(character =>
        char.IsLetterOrDigit(character) || character == '_' ? character : '_'));

    private static PortContractException Error(string code, string message) => new(code, message);
}
