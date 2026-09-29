#if MACOS || IOS || MACCATALYST
using Foundation;
using Doroti.Ui;
using Microsoft.Win32.SafeHandles;

namespace Doroti.Host.Maui;

internal sealed class AppleReadFile : IPickedFile
{
    private readonly NSUrl _url;
    private readonly bool _scoped;
    private readonly SafeFileHandle _handle;
    private readonly CancellationTokenSource _lifetime = new();
    private int _disposed;
    public string Name { get; }
    public long Length { get; }
    internal event Action? Released;
    internal AppleReadFile(NSUrl url)
    {
        _url = new NSUrl(url.AbsoluteString!);
        _scoped = _url.StartAccessingSecurityScopedResource();
        SafeFileHandle? opened = null;
        try
        {
            var path = _url.Path ?? throw new IOException("The file URL has no path.");
            var attributes = NSFileManager.DefaultManager.GetAttributes(path)
                ?? throw new IOException("The selected file's metadata is unavailable.");
            if (attributes.Type != NSFileType.Regular)
                throw new NotSupportedException("Only regular files have random-access read grants.");
            Name = System.IO.Path.GetFileName(path);
            _handle = opened = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, FileOptions.Asynchronous);
            Length = RandomAccess.GetLength(_handle);
        }
        catch { opened?.Dispose(); if (_scoped) _url.StopAccessingSecurityScopedResource(); _url.Dispose(); throw; }
    }
    public async ValueTask<int> ReadAsync(long offset, Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
        var read = await RandomAccess.ReadAsync(_handle, buffer[..Math.Min(buffer.Length, 65536)], offset, linked.Token);
        linked.Token.ThrowIfCancellationRequested();
        return read;
    }
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        _lifetime.Cancel();
        _handle.Dispose();
        if (_scoped) _url.StopAccessingSecurityScopedResource();
        _url.Dispose();
        Released?.Invoke();
        Released = null;
    }
}

#endif
