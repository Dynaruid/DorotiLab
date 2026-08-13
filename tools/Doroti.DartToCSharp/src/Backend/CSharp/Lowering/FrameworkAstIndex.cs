namespace Doroti.DartToCSharp;

/// <summary>One full traversal per declaration; repeated root queries reuse immutable arrays.</summary>
internal sealed class FrameworkAstIndex
{
    private readonly Dictionary<CoreAstNode, CoreAstNode[]> _declarationNodes =
        new(ReferenceEqualityComparer.Instance);

    public FrameworkAstIndex(IEnumerable<CoreResolvedDeclaration> declarations)
    {
        foreach (var declaration in declarations)
        {
            _declarationNodes[declaration.Ast] = Traverse(declaration.Ast).ToArray();
        }
    }

    public IEnumerable<CoreAstNode> DescendantsAndSelf(CoreAstNode node) =>
        _declarationNodes.TryGetValue(node, out var nodes) ? nodes : Traverse(node);

    private static IEnumerable<CoreAstNode> Traverse(CoreAstNode node)
    {
        var stack = new Stack<CoreAstNode>();
        stack.Push(node);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            yield return current;
            for (var index = current.Children.Length - 1; index >= 0; index--)
            {
                stack.Push(current.Children[index]);
            }
        }
    }
}
