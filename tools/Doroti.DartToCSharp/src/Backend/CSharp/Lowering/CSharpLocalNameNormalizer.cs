using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Doroti.DartToCSharp;

internal static class CSharpLocalNameNormalizer
{
    private static readonly Regex AnalyzerOffsetSuffix = new(
        @"^(?<base>[A-Za-z_][A-Za-z0-9_]*)__\d+$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly string[] CollisionSuffixes =
    [
        "Local",
        "Alternate",
        "Nested",
        "Current",
        "Next",
        "Candidate",
    ];

    public static string Normalize(string source)
    {
        var root = CSharpSyntaxTree.ParseText(
                source,
                CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest))
            .GetRoot();
        var matchingTokens = root.DescendantTokens()
            .Where(token => token.IsKind(SyntaxKind.IdentifierToken) &&
                AnalyzerOffsetSuffix.IsMatch(token.ValueText))
            .ToArray();
        if (matchingTokens.Length == 0)
        {
            return source;
        }

        // Keep names unique within an executable body so Dart shadowing never
        // turns into an illegal or ambiguous C# declaration.
        var replacements = new Dictionary<SyntaxToken, string>();
        foreach (var group in matchingTokens.GroupBy(token => FindExecutableBoundary(token.Parent)))
        {
            var boundary = group.Key;
            var reservedNames = (boundary?.DescendantTokens() ?? root.DescendantTokens())
                .Where(token => token.IsKind(SyntaxKind.IdentifierToken) &&
                    !AnalyzerOffsetSuffix.IsMatch(token.ValueText))
                .Select(token => token.ValueText)
                .ToHashSet(StringComparer.Ordinal);
            var allocatedNames = new HashSet<string>(reservedNames, StringComparer.Ordinal);
            var names = group
                .GroupBy(token => token.ValueText, StringComparer.Ordinal)
                .OrderBy(nameGroup => nameGroup.Min(token => token.SpanStart));

            foreach (var nameGroup in names)
            {
                var match = AnalyzerOffsetSuffix.Match(nameGroup.Key);
                var baseName = match.Groups["base"].Value;
                var emittedName = AllocateName(baseName, allocatedNames);
                foreach (var token in nameGroup)
                {
                    replacements[token] = emittedName;
                }
            }
        }

        var rewritten = root.ReplaceTokens(
            replacements.Keys,
            (original, _) => CreateIdentifier(original, replacements[original]));
        return rewritten.ToFullString();
    }

    private static SyntaxNode? FindExecutableBoundary(SyntaxNode? node) =>
        node?.AncestorsAndSelf().FirstOrDefault(candidate => candidate is
            BaseMethodDeclarationSyntax or
            AccessorDeclarationSyntax or
            PropertyDeclarationSyntax or
            IndexerDeclarationSyntax or
            FieldDeclarationSyntax or
            EventFieldDeclarationSyntax or
            GlobalStatementSyntax);

    private static string AllocateName(string baseName, HashSet<string> allocatedNames)
    {
        if (allocatedNames.Add(baseName))
        {
            return baseName;
        }
        foreach (var suffix in CollisionSuffixes)
        {
            var candidate = baseName + suffix;
            if (allocatedNames.Add(candidate))
            {
                return candidate;
            }
        }
        for (var index = 0; ; index++)
        {
            var candidate = baseName + AlphabeticSuffix(index);
            if (allocatedNames.Add(candidate))
            {
                return candidate;
            }
        }
    }

    private static string AlphabeticSuffix(int index)
    {
        var result = string.Empty;
        do
        {
            result = (char)('A' + index % 26) + result;
            index = (index / 26) - 1;
        }
        while (index >= 0);
        return result;
    }

    private static SyntaxToken CreateIdentifier(SyntaxToken original, string valueText)
    {
        var needsEscape = SyntaxFacts.GetKeywordKind(valueText) != SyntaxKind.None ||
            SyntaxFacts.GetContextualKeywordKind(valueText) != SyntaxKind.None;
        var text = needsEscape ? "@" + valueText : valueText;
        return SyntaxFactory.Identifier(
            original.LeadingTrivia,
            SyntaxKind.IdentifierToken,
            text,
            valueText,
            original.TrailingTrivia);
    }
}
