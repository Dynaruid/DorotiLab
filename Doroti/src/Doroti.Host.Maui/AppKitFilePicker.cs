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
    private readonly HashSet<AppleReadFile> _files = [];
    private readonly object _gate = new();
    private bool _disposed;
    private int _picking;

    public async ValueTask<FilePickResult> PickFilesAsync(FilePickOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        options = options.Normalize();
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
                {
                    var types = extensions.Select(extension => UTType.CreateFromExtension(extension.TrimStart('.'))).ToArray();
                    if (types.All(type => type is not null)) panel.AllowedContentTypes = types.OfType<UTType>().ToArray();
                }
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
                                    var file = new AppleReadFile(url);
                                    acquired.Add(file);
                                    if (!FilePickFilters.Matches(file.Name, options.Extensions ?? [])) throw new IOException("Selected file does not match the requested extensions.");
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
        AppleReadFile[] files;
        lock (_gate) { if (_disposed) return; _disposed = true; files = _files.ToArray(); _files.Clear(); }
        _lifetime.Cancel();
        foreach (var file in files) file.Dispose();
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
