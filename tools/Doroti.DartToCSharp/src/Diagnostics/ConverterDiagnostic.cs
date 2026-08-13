namespace Doroti.DartToCSharp;

public sealed record ConverterDiagnostic(
    string Code,
    string Severity,
    string Package,
    string Library,
    string Source,
    int Offset,
    int Length,
    string? Symbol,
    string Message,
    string Cause,
    string SupportState,
    string ManualAction,
    string? CanonicalElementId = null,
    string[]? DependencyPath = null);
