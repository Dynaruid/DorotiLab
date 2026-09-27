using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Doroti.DartToCSharp;

internal sealed partial class FrameworkCSharpLowerer
{
    private void EmitModulo(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreAstNode left,
        CoreAstNode right,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics,
        string? assignmentTarget = null
    )
    {
        string Emit(CoreAstNode expression, bool assignment = false)
        {
            var output = new CsSyntaxBuilder();
            var previous = _session.EmittingAssignmentLeft;
            _session.EmittingAssignmentLeft = assignment;
            try
            {
                LowerExpression(
                    output,
                    expression,
                    declaration,
                    package,
                    library,
                    inputPath,
                    diagnostics
                );
            }
            finally
            {
                _session.EmittingAssignmentLeft = previous;
            }
            return string.Concat(output.Build().Tokens.Select(token => token.Text));
        }

        var leftType = (left.StaticType ?? node.StaticType ?? "dynamic").TrimEnd('?');
        var numeric = leftType is "int" or "double" or "num" or "dynamic";
        var suffix = node.Origin.Offset.ToString(System.Globalization.CultureInfo.InvariantCulture);
        static bool HasAwait(CoreAstNode expression) =>
            expression.Kind != CoreNodeKind.FunctionExpression
            && (
                expression.Kind == CoreNodeKind.AwaitExpression || expression.Children.Any(HasAwait)
            );
        var hasAwait = HasAwait(left) || HasAwait(right);
        string Wrap(string body, string resultType) =>
            hasAwait
                ? $"await ((global::System.Func<global::System.Threading.Tasks.Task<{resultType}>>)(async () => {{ {body} }}))()"
                : $"((global::System.Func<{resultType}>)(() => {{ {body} }}))()";
        string Modulo(string value, string divisor)
        {
            if (leftType == "dynamic")
            {
                // Dynamic values may hold either a Dart number or a user-defined operator.
                var a = "__moduloDynamicLeft" + suffix;
                var b = "__moduloDynamicRight" + suffix;
                return Wrap(
                    $"object {a} = {value}; object {b} = {divisor}; return {a} is long or int or double ? global::Doroti.Runtime.DartNumeric.Modulo({a}, {b}) : ((dynamic){a}).op_Modulus((dynamic){b});",
                    "dynamic"
                );
            }
            return numeric
                ? $"global::Doroti.Runtime.DartNumeric.Modulo({value}, {divisor})"
                : $"({value}).op_Modulus({divisor})";
        }
        if (node.Text(CoreProperty.@operator) == "%")
        {
            builder.Append(Modulo(Emit(left), Emit(right)));
            return;
        }

        // Save the receiver and index before reading the value and evaluating the RHS.
        // A textual `target = Modulo(target, rhs)` would repeat getters/calls/indexes.
        var target = SyntaxFactory.ParseExpression(
            assignmentTarget ?? Emit(left, assignment: true)
        );
        var receiver = "__moduloReceiver" + suffix;
        var prelude = "";
        var returnType = MapType(node.StaticType ?? left.StaticType ?? "dynamic");
        switch (target)
        {
            case ConditionalAccessExpressionSyntax conditional:
                prelude +=
                    $"var {receiver} = {conditional.Expression}; if ({receiver} is null) return default; ";
                if (returnType is "long" or "double" or "int")
                    returnType += "?";
                target = SyntaxFactory.ParseExpression(receiver + conditional.WhenNotNull);
                if (target is ElementAccessExpressionSyntax nullableElement)
                {
                    var index = "__moduloIndex" + suffix;
                    prelude +=
                        $"var {index} = {nullableElement.ArgumentList.Arguments[0].Expression}; ";
                    target = SyntaxFactory.ParseExpression($"{receiver}[{index}]");
                }
                break;
            case ElementAccessExpressionSyntax element:
                prelude += $"var {receiver} = {element.Expression}; ";
                var indices = new List<string>();
                for (var i = 0; i < element.ArgumentList.Arguments.Count; i++)
                {
                    var index = $"__moduloIndex{suffix}_{i}";
                    prelude += $"var {index} = {element.ArgumentList.Arguments[i].Expression}; ";
                    indices.Add(index);
                }
                target = SyntaxFactory.ParseExpression($"{receiver}[{string.Join(", ", indices)}]");
                break;
            case MemberAccessExpressionSyntax member
                when member.Expression is not (ThisExpressionSyntax or BaseExpressionSyntax)
                    && FindGlobalMember(left.ElementId)?.IsStatic != true
                    && FindGlobalDeclaration(member.Expression.ToString()) is null:
                prelude += $"var {receiver} = {member.Expression}; ";
                target = member.WithExpression(SyntaxFactory.IdentifierName(receiver));
                break;
        }
        var valueName = "__moduloValue" + suffix;
        var resultName = "__moduloResult" + suffix;
        builder.Append(
            Wrap(
                $"{prelude}var {valueName} = {target}; var {resultName} = {Modulo(valueName, Emit(right))}; {target} = {resultName}; return {resultName};",
                returnType
            )
        );
    }
}
