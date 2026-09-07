namespace Doroti.DartToCSharp;

internal sealed partial class FrameworkCSharpLowerer
{
    private void ValidateOverrideContract(CoreResolvedDeclaration declaration, CoreResolvedMember member,
        CoreResolvedMember? contract, bool isInterface, string package, string library, string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        if (member.IsStatic || isInterface || contract is not null ||
            !HasOverrideAnnotation(member.Ast) || member.Name is "toString" or "hashCode" or "==" ||
            IsKnownExternalOverride(declaration, member) ||
            FindDeclaringDeclaration(member)?.Element.CanonicalId != declaration.Element.CanonicalId) return;
        diagnostics.Add(new ConverterDiagnostic("DOTCONV902", "error", package, library, inputPath,
            member.Offset, member.Length, member.Name,
            $"Cannot resolve the inherited contract for {declaration.Name}.{member.Name}; emitting a new virtual slot would lose Dart override dispatch.",
            "unresolved-override-contract", "diagnostic-only",
            "Include the declaring base library in the semantic selection (graph-only is sufficient for referenced bases).",
            member.Element.CanonicalId));
    }
}
