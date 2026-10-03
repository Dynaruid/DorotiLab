#if IOS || MACCATALYST
using Doroti.Ui;
using Foundation;
using UIKit;
using UniformTypeIdentifiers;

namespace Doroti.Host.Maui;

/// <summary>A document picker and security-scoped grants owned by the requesting view.</summary>
internal sealed class UIKitFilePicker(Func<UIViewController?> owner) : IFilePickerHostCapability, IDisposable
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
        lock (_gate) ObjectDisposedException.ThrowIf(_disposed, this);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
        linked.Token.ThrowIfCancellationRequested();
        if (Interlocked.Exchange(ref _picking, 1) != 0)
            return new(FilePickStatus.failed, [], "A picker is already open for this view.");
        UIDocumentPickerViewController? picker = null;
        PickerDelegate? callbacks = null;
        var completion = new TaskCompletionSource<FilePickResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                linked.Token.ThrowIfCancellationRequested();
                var controller = owner() ?? throw new InvalidOperationException("The view has no native window.");
                if (controller.PresentedViewController is not null)
                    throw new InvalidOperationException("The requesting window already presents a controller.");
                var types = options.Extensions is { Length: > 0 } extensions && !extensions.Contains("*")
                    ? extensions.Select(extension => UTType.CreateFromExtension(extension.TrimStart('.'))
                        ?? UTTypes.Item).ToArray()
                    : new[] { UTTypes.Item };
                picker = new UIDocumentPickerViewController(types, asCopy: false) { AllowsMultipleSelection = options.AllowMultiple };
                callbacks = new PickerDelegate(urls =>
                {
                    List<IPickedFile> acquired = [];
                    try
                    {
                        linked.Token.ThrowIfCancellationRequested();
                        lock (_gate)
                        {
                            ObjectDisposedException.ThrowIf(_disposed, this);
                            foreach (var url in urls)
                            {
                                var file = new AppleReadFile(url);
                                acquired.Add(file);
                                if (!FilePickFilters.Matches(file.Name, options.Extensions ?? [])) throw new IOException("Selected file does not match the requested extensions.");
                                _files.Add(file);
                                file.Released += () => { lock (_gate) _files.Remove(file); };
                            }
                        }
                        if (completion.TrySetResult(new(FilePickStatus.selected, acquired.ToArray()))) acquired.Clear();
                    }
                    catch (OperationCanceledException) { completion.TrySetCanceled(linked.Token); }
                    catch (Exception error) { completion.TrySetResult(new(error is UnauthorizedAccessException ? FilePickStatus.denied : FilePickStatus.failed, [], error.Message)); }
                    finally { foreach (var file in acquired) file.Dispose(); }
                }, () => completion.TrySetResult(new(FilePickStatus.cancelled, [])));
                picker.Delegate = callbacks;
                controller.PresentViewController(picker, false, null);
            });
            using var registration = linked.Token.Register(() => completion.TrySetCanceled(linked.Token));
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
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (picker is not null)
                {
                    if (picker.PresentingViewController is not null)
                        await picker.DismissViewControllerAsync(false);
                    picker.WeakDelegate = null;
                    picker.Dispose();
                }
                callbacks?.Dispose();
            });
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

    private sealed class PickerDelegate(Action<NSUrl[]> selected, Action cancelled) : UIDocumentPickerDelegate
    {
        public override void DidPickDocument(UIDocumentPickerViewController controller, NSUrl[] urls) => selected(urls);
        public override void WasCancelled(UIDocumentPickerViewController controller) => cancelled();
    }
}
#endif
