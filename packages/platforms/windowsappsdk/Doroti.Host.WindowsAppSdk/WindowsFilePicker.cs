using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>HWND-owned picker shared with the Windows MAUI host.</summary>
public sealed class WindowsFilePicker : IFilePickerHostCapability, IDisposable
{
    private readonly SharedWindows.WindowsFilePickerService _service;
    public WindowsFilePicker(nint owner) => _service = new(owner);
    public ValueTask<FilePickResult> PickFilesAsync(FilePickOptions options, CancellationToken cancellationToken = default) =>
        _service.PickFilesAsync(options, cancellationToken);
    public void Dispose() => _service.Dispose();
}
