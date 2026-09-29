#if MACOS
using AppKit;
using Foundation;
using Doroti.Ui;
using Microsoft.Win32.SafeHandles;
using UniformTypeIdentifiers;

namespace Doroti.Host.Maui;

/// <summary>Each panel belongs to its requesting window; grants end with the owning view.</summary>
internal sealed class AppKitFilePicker(Func<NSWindow?> window) : IFilePickerHostCapability, IDisposable
{
    private readonly CancellationTokenSource _lifetime = new();
    private readonly HashSet<AppKitReadFile> _files = [];
    private readonly object _gate = new();
    private bool _disposed;
    private int _picking;

    public async ValueTask<FilePickResult> PickFilesAsync(FilePickOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
        linked.Token.ThrowIfCancellationRequested();
        if (Interlocked.Exchange(ref _picking, 1) != 0) return new(FilePickStatus.failed, [], "A picker is already open for this view.");
        NSOpenPanel? panel = null;
        var completion = new TaskCompletionSource<FilePickResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        try
        {
            await AppKitUi.Invoke(() =>
            {
                linked.Token.ThrowIfCancellationRequested();
                var owner = window() ?? throw new InvalidOperationException("The view has no native window.");
                panel = NSOpenPanel.OpenPanel;
                panel.CanChooseFiles = true;
                panel.CanChooseDirectories = false;
                panel.AllowsMultipleSelection = options.AllowMultiple;
                if (options.Extensions is { Length: > 0 } extensions && !extensions.Contains("*"))
                    panel.AllowedContentTypes = extensions.Select(extension =>
                        UTType.CreateFromExtension(extension.TrimStart('.')) ?? throw new ArgumentException("Unknown file extension: " + extension)).ToArray();
                panel.BeginSheet(owner, response =>
                {
                    List<IPickedFile> acquired = [];
                    try
                    {
                        linked.Token.ThrowIfCancellationRequested();
                        if (response == 1)
                        {
                            lock (_gate)
                            {
                                ObjectDisposedException.ThrowIf(_disposed, this);
                                foreach (var url in panel.Urls)
                                {
                                    var file = new AppKitReadFile(url);
                                    acquired.Add(file);
                                    _files.Add(file);
                                    file.Released += () => { lock (_gate) _files.Remove(file); };
                                }
                            }
                        }
                        if (completion.TrySetResult(new(response == 1 ? FilePickStatus.selected : FilePickStatus.cancelled, acquired.ToArray())))
                            acquired.Clear();
                    }
                    catch (OperationCanceledException) { completion.TrySetCanceled(linked.Token); }
                    catch (Exception error) { completion.TrySetResult(new(error is UnauthorizedAccessException ? FilePickStatus.denied : FilePickStatus.failed, [], error.Message)); }
                    finally { foreach (var file in acquired) file.Dispose(); }
                });
            });
            using var registration = linked.Token.Register(() => NSApplication.SharedApplication.BeginInvokeOnMainThread(() =>
            {
                if (!completion.Task.IsCompleted) panel?.Cancel(NSApplication.SharedApplication);
            }));
            var result = await completion.Task;
            if (linked.IsCancellationRequested)
            {
                foreach (var file in result.Files) file.Dispose();
                linked.Token.ThrowIfCancellationRequested();
            }
            return result;
        }
        finally
        {
            if (panel is not null) await AppKitUi.Invoke(panel.Dispose);
            Volatile.Write(ref _picking, 0);
        }
    }
    public void Dispose()
    {
        AppKitReadFile[] files;
        lock (_gate) { if (_disposed) return; _disposed = true; files = _files.ToArray(); _files.Clear(); }
        _lifetime.Cancel();
        foreach (var file in files) file.Dispose();
    }
}

internal sealed class AppKitReadFile : IPickedFile
{
    private readonly NSUrl _url;
    private readonly bool _scoped;
    private readonly SafeFileHandle _handle;
    private readonly CancellationTokenSource _lifetime = new();
    private int _disposed;
    public string Name { get; }
    public long Length { get; }
    internal event Action? Released;
    internal AppKitReadFile(NSUrl url)
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

internal static class AppKitUi
{
    internal static Task Invoke(Action action)
    {
        if (NSThread.IsMain) { action(); return Task.CompletedTask; }
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        NSApplication.SharedApplication.BeginInvokeOnMainThread(() =>
        {
            try { action(); completion.SetResult(); }
            catch (Exception error) { completion.SetException(error); }
        });
        return completion.Task;
    }
}
#endif
