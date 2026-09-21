using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Doroti.DartToCSharp;

/// <summary>
/// Lowers the converter's temporary null-assertion form to a self-contained C#
/// switch expression. The property pattern both rejects null and extracts the
/// underlying value of nullable value types while evaluating the operand once.
/// </summary>
internal static class CSharpNullAssertionLowering
{
    private const string Placeholder = "__dorotiNullAssert";
    private const string NullMessage = "Dart null assertion failed.";

    public static string Normalize(string source)
    {
        var root = CSharpSyntaxTree
            .ParseText(
                source,
                CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest)
            )
            .GetRoot();
        var candidates = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(invocation =>
                invocation.Expression is IdentifierNameSyntax { Identifier.Text: Placeholder }
            )
            .ToArray();
        if (candidates.Length == 0)
        {
            return source;
        }

        var rewritten = root.ReplaceNodes(
            candidates,
            (original, visited) =>
            {
                if (visited.ArgumentList.Arguments.Count != 1)
                {
                    throw new InvalidOperationException(
                        "The internal null assertion form must have exactly one operand."
                    );
                }
                var operand = visited.ArgumentList.Arguments[0].Expression.WithoutTrivia();
                return SyntaxFactory
                    .ParseExpression(
                        $"({operand}) switch {{ {{ }} __requiredValue when global::Doroti.Runtime.DartRuntimePrimitives.NonExhaustiveSwitchGuard => __requiredValue, _ => throw new global::System.NullReferenceException(\"{NullMessage}\") }}"
                    )
                    .WithTriviaFrom(original);
            }
        );
        var result = rewritten.ToFullString();
        if (result.Contains(Placeholder, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "An internal null assertion placeholder escaped C# lowering."
            );
        }
        return result;
    }
}
