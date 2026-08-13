namespace Doroti.DartToCSharp;

internal static class CallNormalizationPass
{
    public static PassResult<DartCall> Normalize(DartCall call) => new(
        call with
        {
            Arguments = call.Arguments
                .OrderBy(argument => argument.Name is null ? 0 : 1)
                .ThenBy(argument => argument.Name, StringComparer.Ordinal)
                .ToArray(),
            Origin = call.Origin.Through(nameof(CallNormalizationPass)),
        },
        []);
}
