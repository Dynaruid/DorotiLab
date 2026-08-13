namespace Doroti.Platform;

public readonly record struct ClipboardResult(bool Success, string? Text = null, string? Diagnostic = null)
{
    public static ClipboardResult FromText(string? text) => new(true, text);

    public static ClipboardResult Failure(string diagnostic) => new(false, null, diagnostic);
}

public interface IClipboard
{
    ValueTask<ClipboardResult> GetTextAsync(CancellationToken cancellationToken = default);

    ValueTask<ClipboardResult> SetTextAsync(string text, CancellationToken cancellationToken = default);
}

public sealed class UnsupportedClipboard(string diagnostic) : IClipboard
{
    public ValueTask<ClipboardResult> GetTextAsync(CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(ClipboardResult.Failure(diagnostic));

    public ValueTask<ClipboardResult> SetTextAsync(string text, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(ClipboardResult.Failure(diagnostic));
}
