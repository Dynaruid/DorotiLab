namespace Doroti.DartToCSharp;

internal abstract record CoreType(bool IsNullable);
internal sealed record CoreDynamicType() : CoreType(true);
internal sealed record CoreVoidType() : CoreType(false);
internal sealed record CoreNeverType() : CoreType(false);
internal sealed record CoreBooleanType(bool Nullable = false) : CoreType(Nullable);
internal sealed record CoreIntegerType(bool Nullable = false) : CoreType(Nullable);
internal sealed record CoreFloatingType(bool Nullable = false) : CoreType(Nullable);
internal sealed record CoreStringType(bool Nullable = false) : CoreType(Nullable);
internal sealed record CoreNominalType(SymbolId Symbol, CoreType[] TypeArguments, bool Nullable = false) : CoreType(Nullable);
internal sealed record CoreFunctionType(CoreType ReturnType, CoreType[] ParameterTypes, bool Nullable = false) : CoreType(Nullable);
