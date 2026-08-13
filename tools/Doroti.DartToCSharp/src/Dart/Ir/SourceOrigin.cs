namespace Doroti.DartToCSharp;

internal sealed record SourceOrigin
{
    public SourceOrigin(
        string source,
        int offset,
        int length,
        SymbolId? symbolId,
        SourceOrigin? original = null,
        string[]? passTrace = null)
    {
        if (offset < 0 || length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "Source spans cannot be negative.");
        }
        Source = source;
        Offset = offset;
        Length = length;
        SymbolId = symbolId;
        Original = original;
        PassTrace = passTrace;
    }

    public string Source { get; init; }
    public int Offset { get; init; }
    public int Length { get; init; }
    public SymbolId? SymbolId { get; init; }
    public SourceOrigin? Original { get; init; }
    public string[]? PassTrace { get; init; }

    public SourceOrigin Through(string pass) => this with
    {
        Original = Original ?? this,
        PassTrace = [.. PassTrace ?? [], pass],
    };
}
