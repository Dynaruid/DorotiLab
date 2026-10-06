namespace Doroti.Ui;

public enum FilePickStatus { selected, cancelled, denied, unsupported, failed }

public sealed record FilePickOptions(bool AllowMultiple = false, string[]? Extensions = null)
{
    public FilePickOptions Normalize() => this with { Extensions = FilePickFilters.Normalize(Extensions) };
}

/// <summary>Empty means all files. Filters are copied and canonicalized before asynchronous admission.</summary>
public static class FilePickFilters
{
    public static string[] Normalize(IEnumerable<string>? extensions)
    {
        if (extensions is null) return [];
        var copied = extensions.ToArray();
        if (copied.Contains("*", StringComparer.Ordinal)) return [];
        var result = new HashSet<string>(StringComparer.Ordinal);
        foreach (var extension in copied)
        {
            if (string.IsNullOrEmpty(extension)) throw new ArgumentException("A file extension cannot be empty.", nameof(extensions));
            var value = extension[0] == '.' ? extension[1..] : extension;
            if (value.Length == 0 || value.Any(c => !char.IsAsciiLetterOrDigit(c)))
                throw new ArgumentException("Use '*' or a simple extension such as '.txt' or 'txt'.", nameof(extensions));
            result.Add("." + value.ToLowerInvariant());
        }
        return result.Order(StringComparer.Ordinal).ToArray();
    }

    public static bool Matches(string fileName, IReadOnlyList<string> normalized) => normalized.Count == 0
        || normalized.Any(extension => fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase));

    public static FilePickResult Enforce(FilePickResult result, IReadOnlyList<string> normalized)
    {
        if (result.Status == FilePickStatus.selected && result.Files.All(file => Matches(file.Name, normalized))) return result;
        if (result.Files.Count == 0) return result;
        List<Exception> errors = [];
        foreach (var file in result.Files) try { file.Dispose(); } catch (Exception error) { errors.Add(error); }
        return new(result.Status == FilePickStatus.selected ? FilePickStatus.failed : result.Status, [],
            (result.Status == FilePickStatus.selected ? "Selected file does not match the requested extensions." : result.Message)
            + (errors.Count == 0 ? "" : $" {errors.Count} grant cleanup failure(s)."));
    }
}

/// <summary>A read grant owned by the caller. Dispose revokes access, including open reads.</summary>
public interface IPickedFile : IDisposable
{
    string Name { get; }
    long Length { get; }
    ValueTask<int> ReadAsync(long offset, Memory<byte> buffer, CancellationToken cancellationToken = default);
}

public sealed record FilePickResult(FilePickStatus Status, IReadOnlyList<IPickedFile> Files, string? Message = null);

public interface IFilePickerHostCapability
{
    /// <summary>
    /// Associates a button's Semantics identifier with its picker options so a browser
    /// can open the native picker during the trusted tap, before a Worker round trip.
    /// Call PickFilesAsync with the same options from the button callback. Dispose
    /// the registration when the control leaves the view. Native hosts need no binding.
    /// </summary>
    IDisposable? RegisterActivation(string semanticsIdentifier, FilePickOptions options) => null;

    ValueTask<FilePickResult> PickFilesAsync(FilePickOptions options, CancellationToken cancellationToken = default);
}
