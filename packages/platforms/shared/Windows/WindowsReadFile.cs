using Doroti.Ui;
using Microsoft.Win32.SafeHandles;

namespace Doroti.Host.SharedWindows;

/// <summary>Shared read grant for picked and dropped files. Never executes or loads the whole file.</summary>
internal sealed class WindowsReadFile : IPickedFile
{
    private readonly SafeFileHandle _handle;
    public WindowsReadFile(string path)
    {
        _handle = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, FileOptions.Asynchronous);
        try { Length = RandomAccess.GetLength(_handle); Name = System.IO.Path.GetFileName(path); }
        catch { _handle.Dispose(); throw; }
    }
    public string Name { get; }
    public long Length { get; }
    public ValueTask<int> ReadAsync(long offset, Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        RandomAccess.ReadAsync(_handle, buffer, offset, cancellationToken);
    public void Dispose() => _handle.Dispose();
}
