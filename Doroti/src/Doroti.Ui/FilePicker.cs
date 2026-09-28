namespace Doroti.Ui;

public enum FilePickStatus { selected, cancelled, denied, unsupported, failed }

public sealed record FilePickOptions(bool AllowMultiple = false, string[]? Extensions = null);

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
    ValueTask<FilePickResult> PickFilesAsync(FilePickOptions options, CancellationToken cancellationToken = default);
}
