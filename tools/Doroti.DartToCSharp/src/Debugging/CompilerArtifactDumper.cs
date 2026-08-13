using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal static class CompilerArtifactDumper
{
    public static void Write(
        CompilerDumpOptions options,
        CompilerIdentity identity,
        IEnumerable<CompilerDumpInput> inputs)
    {
        var directory = Path.GetFullPath(options.Directory);
        Directory.CreateDirectory(directory);
        var ordered = inputs.OrderBy(item => item.Source, StringComparer.Ordinal).ToArray();

        if (options.Stages.HasFlag(CompilerDumpStage.AnalyzerProtocol))
        {
            ArtifactFiles.WriteJson(Path.Combine(directory, "analyzer-protocol.json"), new
            {
                schemaVersion = "doroti.analyzer-protocol-dump/v1",
                identity,
                inputs = ordered.Select(item => new { source = item.Source, output = item.AnalyzerOutput }).ToArray(),
            });
        }
        if (options.Stages.HasFlag(CompilerDumpStage.DartIr))
        {
            ArtifactFiles.WriteJson(Path.Combine(directory, "dart-ir.json"), new
            {
                schemaVersion = "doroti.dart-ir/v1",
                identity,
                inputs = ordered.Select(item => new
                {
                    source = item.Source,
                    library = item.Library,
                    declarations = item.DartDeclarations.OrderBy(value => value.Offset).Select(DumpDeclaration).ToArray(),
                }).ToArray(),
            });
        }
        if (options.Stages.HasFlag(CompilerDumpStage.CoreIr))
        {
            ArtifactFiles.WriteJson(Path.Combine(directory, "core-ir.json"), new
            {
                schemaVersion = "doroti.core-ir/v1",
                identity,
                inputs = ordered.Select(item => new
                {
                    source = item.Source,
                    library = item.Library,
                    declarations = item.CoreDeclarations.OrderBy(value => value.Offset).Select(declaration => new
                    {
                        symbolId = declaration.Element.CanonicalId,
                        declarationKind = declaration.Kind,
                        memberSymbols = declaration.Members.Select(member => member.Element.CanonicalId)
                            .OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                        runtimeBindings = CoreDescendantsAndSelf(declaration.Ast)
                            .Select(node => CoreBinding(node))
                            .Where(value => value is not null)
                            .Distinct(StringComparer.Ordinal)
                            .OrderBy(value => value, StringComparer.Ordinal)
                            .ToArray(),
                    }).ToArray(),
                }).ToArray(),
            });
        }
        if (options.Stages.HasFlag(CompilerDumpStage.CSharpIr))
        {
            ArtifactFiles.WriteJson(Path.Combine(directory, "csharp-ir.json"), new
            {
                schemaVersion = "doroti.csharp-ir/v1",
                identity,
                inputs = ordered.Where(item => item.GeneratedFile is not null).Select(item => new
                {
                    source = item.Source,
                    generatedFile = item.GeneratedFile,
                    generatedSha256 = Sha256Text(item.GeneratedCode!),
                    declarations = item.Mappings.OrderBy(mapping => mapping.SourceOffset).Select(mapping => new
                    {
                        mapping.Symbol,
                        origin = new { mapping.Source, mapping.SourceOffset, mapping.SourceLength },
                        printedSpan = new { mapping.GeneratedLineStart, mapping.GeneratedLineEnd },
                    }).ToArray(),
                }).ToArray(),
            });
        }
    }

    private static object DumpDeclaration(DartResolvedDeclaration declaration) => new
    {
        declaration.Kind,
        declaration.Name,
        symbolId = declaration.Element.CanonicalId,
        origin = new { declaration.Ast.Origin.Source, declaration.Offset, declaration.Length },
        node = DumpNode(declaration.Ast),
        members = declaration.Members.OrderBy(member => member.Offset).Select(member => new
        {
            member.Kind,
            member.Name,
            symbolId = member.Element.CanonicalId,
            node = DumpNode(member.Ast),
        }).ToArray(),
    };

    private static object DumpNode(DartAstNode node) => new
    {
        node.Kind,
        node.Category,
        origin = new { node.Origin.Source, node.Offset, node.Length },
        staticType = node.StaticType,
        resolvedElement = node.ResolvedElement?.Value,
        properties = node.Properties.OrderBy(item => item.Key, StringComparer.Ordinal)
            .ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal),
        children = node.Children.Select(DumpNode).ToArray(),
    };

    private static string? CoreBinding(CoreAstNode node)
    {
        if (node.ResolvedElement is { } symbol && node.FlutterBinding is { } flutter)
        {
            return $"flutter:{symbol.Value}->{flutter}";
        }
        if (node.ResolvedElement is { } runtimeSymbol && node.RuntimeIntrinsic is { } intrinsic)
        {
            return $"runtime:{runtimeSymbol.Value}->{intrinsic}";
        }
        return null;
    }

    private static IEnumerable<CoreAstNode> CoreDescendantsAndSelf(CoreAstNode node)
    {
        yield return node;
        foreach (var child in node.Children)
        {
            foreach (var descendant in CoreDescendantsAndSelf(child))
            {
                yield return descendant;
            }
        }
    }

    private static IEnumerable<DartAstNode> DescendantsAndSelf(DartAstNode node)
    {
        yield return node;
        foreach (var child in node.Children)
        {
            foreach (var descendant in DescendantsAndSelf(child))
            {
                yield return descendant;
            }
        }
    }

    private static string Sha256Text(string value)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(value.Replace("\r\n", "\n", StringComparison.Ordinal));
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)).ToLowerInvariant();
    }
}
