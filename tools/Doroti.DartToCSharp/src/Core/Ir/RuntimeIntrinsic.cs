namespace Doroti.DartToCSharp;

internal enum RuntimeIntrinsic
{
    RequireValue,
    DynamicIndexGet,
    DynamicIndexSet,
    ObjectHash,
    RuntimeType,
    EnumIndex,
    EnumName,
    ScheduleMicrotask,
    AwaitFutureOr,
    Assert,
    Environment,
    Equality,
    Identical,
    Hash,
    CollectionLength,
    CollectionIsEmpty,
    CollectionIsNotEmpty,
    StringLength,
    StringCodeUnitAt,
}

internal sealed record RuntimeIntrinsicBinding(
    RuntimeIntrinsic Intrinsic,
    SymbolId Member,
    SymbolId? ReceiverType = null,
    int? TypeArgumentCount = null);

internal static class RuntimeIntrinsicRegistry
{
    private static readonly RuntimeIntrinsicBinding[] Bindings =
    [
        new(RuntimeIntrinsic.ObjectHash, SymbolId.Parse("dart:core#Object.hash")),
        new(RuntimeIntrinsic.Identical, SymbolId.Parse("dart:core#identical")),
        new(RuntimeIntrinsic.RuntimeType, SymbolId.Parse("dart:core#Object.runtimeType")),
        new(RuntimeIntrinsic.EnumIndex, SymbolId.Parse("dart:core#Enum.index")),
        new(RuntimeIntrinsic.EnumName, SymbolId.Parse("dart:core#Enum.name")),
        new(RuntimeIntrinsic.ScheduleMicrotask, SymbolId.Parse("dart:async#scheduleMicrotask")),
        new(RuntimeIntrinsic.StringLength, SymbolId.Parse("dart:core#String.length"), SymbolId.Parse("dart:core#String")),
        new(RuntimeIntrinsic.StringCodeUnitAt, SymbolId.Parse("dart:core#String.codeUnitAt"), SymbolId.Parse("dart:core#String")),
        new(RuntimeIntrinsic.CollectionLength, SymbolId.Parse("dart:core#Iterable.length"), SymbolId.Parse("dart:core#Iterable"), 1),
        new(RuntimeIntrinsic.CollectionIsEmpty, SymbolId.Parse("dart:core#Iterable.isEmpty"), SymbolId.Parse("dart:core#Iterable"), 1),
        new(RuntimeIntrinsic.CollectionIsNotEmpty, SymbolId.Parse("dart:core#Iterable.isNotEmpty"), SymbolId.Parse("dart:core#Iterable"), 1),
    ];

    public static RuntimeIntrinsicBinding? Resolve(SymbolId member, DartType? receiverType)
    {
        return Bindings.FirstOrDefault(binding =>
            binding.Member == member &&
            ReceiverMatches(binding, receiverType));
    }

    private static bool ReceiverMatches(RuntimeIntrinsicBinding binding, DartType? receiverType)
    {
        if (binding.ReceiverType is null) return true;
        return receiverType is DartInterfaceType interfaceType &&
            interfaceType.Symbol == binding.ReceiverType.Value &&
            (binding.TypeArgumentCount is null || interfaceType.TypeArguments.Length == binding.TypeArgumentCount.Value);
    }
}
