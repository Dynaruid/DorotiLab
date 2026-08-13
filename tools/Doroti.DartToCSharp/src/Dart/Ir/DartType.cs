namespace Doroti.DartToCSharp;

internal abstract record DartType(Nullability Nullability);

internal sealed record DartDynamicType() : DartType(Nullability.Nullable);
internal sealed record DartVoidType() : DartType(Nullability.NonNullable);
internal sealed record DartNeverType(Nullability ValueNullability) : DartType(ValueNullability);
internal sealed record DartNullType() : DartType(Nullability.Nullable);
internal sealed record DartTypeParameterType(SymbolId Symbol, Nullability ValueNullability) : DartType(ValueNullability);
internal sealed record DartInterfaceType(
    SymbolId Symbol,
    DartType[] TypeArguments,
    Nullability ValueNullability) : DartType(ValueNullability);
internal sealed record DartFunctionType(
    DartType ReturnType,
    DartFunctionParameter[] Parameters,
    Nullability ValueNullability) : DartType(ValueNullability);
internal sealed record DartRecordType(
    DartType[] Positional,
    IReadOnlyDictionary<string, DartType> Named,
    Nullability ValueNullability) : DartType(ValueNullability);

internal enum DartParameterKind
{
    RequiredPositional,
    OptionalPositional,
    RequiredNamed,
    OptionalNamed,
}

internal sealed record DartFunctionParameter(string? Name, DartType Type, DartParameterKind Kind);
