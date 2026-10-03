#if WINDOWS
using Doroti.Host.SharedWindows;
using Doroti.Ui;

namespace Doroti.Host.Maui;

/// <summary>Resolve the actual MAUI window lazily, on its UI thread.</summary>
internal sealed class MauiWindowsFilePicker(Func<nint> owner) : IFilePickerHostCapability, IDisposable
{
    private WindowsFilePickerService? _service;
    private nint _window;
    private bool _disposed;

    public async ValueTask<FilePickResult> PickFilesAsync(FilePickOptions options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            cancellationToken.ThrowIfCancellationRequested();
            var window = owner();
            if (window == 0)
                return new FilePickResult(FilePickStatus.failed, [], "The MAUI view is not attached to a live window.");
            if (_window != 0 && window != _window)
                throw new InvalidOperationException("A file picker owner cannot move between windows.");
            _service ??= new WindowsFilePickerService(_window = window);
            return await _service.PickFilesAsync(options, cancellationToken).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _service?.Dispose();
        _service = null;
    }
}
#endif
