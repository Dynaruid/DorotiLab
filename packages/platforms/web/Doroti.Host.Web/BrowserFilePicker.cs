using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;
using Doroti.Ui;

namespace Doroti.Host.Web;

[SupportedOSPlatform("browser")]
internal static partial class BrowserInterop
{
    [JSImport("openFileOwner", Module)] internal static partial bool OpenFileOwner(int id);
    [JSImport("registerFileActivation", Module)] internal static partial void RegisterFileActivation(int id, string identifier, bool multiple, string accept);
    [JSImport("unregisterFileActivation", Module)] internal static partial void UnregisterFileActivation(int id, string identifier);
    [JSImport("pickBrowserFiles", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    internal static partial Task<string> PickBrowserFiles(int id, bool multiple, string accept);
    [JSImport("cancelBrowserPicker", Module)] internal static partial void CancelBrowserPicker(int id);
    [JSImport("readBrowserFileBase64", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    internal static partial Task<string> ReadBrowserFile(int id, string token, double offset, int count);
    [JSImport("releaseBrowserFile", Module)] internal static partial void ReleaseBrowserFile(int id, string token);
    [JSImport("closeFileOwner", Module)] internal static partial void CloseFileOwner(int id);
}

[SupportedOSPlatform("browser")]
internal sealed class BrowserFilePicker(int hostId) : IFilePickerHostCapability, IDisposable
{
    public bool Available { get; } = BrowserInterop.OpenFileOwner(hostId);
    private bool _disposed;
    public IDisposable RegisterActivation(string semanticsIdentifier, FilePickOptions options)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrEmpty(semanticsIdentifier);
        BrowserInterop.RegisterFileActivation(hostId, semanticsIdentifier, options.AllowMultiple,
            string.Join(",", FilePickFilters.Normalize(options.Extensions)));
        return new ActivationRegistration(hostId, semanticsIdentifier);
    }
    private sealed class ActivationRegistration(int id, string identifier) : IDisposable
    {
        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            BrowserInterop.UnregisterFileActivation(id, identifier);
        }
    }
    public async ValueTask<FilePickResult> PickFilesAsync(FilePickOptions options, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();
        var extensions = FilePickFilters.Normalize(options.Extensions);
        var task = BrowserInterop.PickBrowserFiles(hostId, options.AllowMultiple,
            string.Join(",", extensions));
        var dispatcher = SynchronizationContext.Current;
        using var cancel = cancellationToken.Register(() =>
        {
            if (dispatcher is not null) dispatcher.Post(_ => { if (!task.IsCompleted) BrowserInterop.CancelBrowserPicker(hostId); }, null);
        });
        using var json = JsonDocument.Parse(await task);
        var root = json.RootElement;
        var files = root.TryGetProperty("files", out var values)
            ? values.EnumerateArray().Select(value => (IPickedFile)new BrowserPickedFile(hostId,
                value.GetProperty("token").GetString()!, value.GetProperty("name").GetString()!, value.GetProperty("length").GetInt64())).ToArray() : [];
        if (cancellationToken.IsCancellationRequested || _disposed)
        {
            foreach (var file in files) file.Dispose();
            cancellationToken.ThrowIfCancellationRequested();
            throw new ObjectDisposedException(nameof(BrowserFilePicker));
        }
        return FilePickFilters.Enforce(new(Enum.Parse<FilePickStatus>(root.GetProperty("status").GetString()!), files,
            root.TryGetProperty("message", out var message) ? message.GetString() : null), extensions);
    }
    public void Dispose() { if (_disposed) return; _disposed = true; BrowserInterop.CloseFileOwner(hostId); }
}

[SupportedOSPlatform("browser")]
internal sealed class BrowserPickedFile(int hostId, string token, string name, long length) : IPickedFile
{
    private bool _disposed;
    public string Name => name;
    public long Length => length;
    public async ValueTask<int> ReadAsync(long offset, Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();
        if (offset < 0 || offset > 9007199254740991L) throw new ArgumentOutOfRangeException(nameof(offset));
        var bytes = Convert.FromBase64String(await BrowserInterop.ReadBrowserFile(hostId, token, offset, Math.Min(buffer.Length, 65536)));
        cancellationToken.ThrowIfCancellationRequested();
        ObjectDisposedException.ThrowIf(_disposed, this);
        bytes.CopyTo(buffer); return bytes.Length;
    }
    public void Dispose() { if (_disposed) return; _disposed = true; BrowserInterop.ReleaseBrowserFile(hostId, token); }
}
