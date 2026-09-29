namespace Doroti.Hosting;

/// <summary>Atomic per-application checkpoint; no source tree or NuGet cache dependency.</summary>
public sealed class FileRestorationStore
{
    private readonly string _path;
    public FileRestorationStore(string directory, string restorationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(restorationId);
        var name = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(restorationId)));
        _path = Path.Combine(Path.GetFullPath(directory), name + ".json");
    }
    public string? Read()
    {
        try
        {
            if (!File.Exists(_path)) return null;
            if (new FileInfo(_path).Length > ApplicationNavigationHost.MaximumCheckpointBytes * 2) return null;
            return File.ReadAllText(_path);
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException) { return null; }
    }
    public void Write(string checkpoint)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var temporary = _path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(checkpoint);
                stream.Write(bytes);
                stream.Flush(flushToDisk: true);
            }
            File.Move(temporary, _path, overwrite: true);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }
}
