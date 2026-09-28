using System.Runtime.InteropServices;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>HWND-owned read picker with a cancellable, dedicated STA dialog thread.</summary>
public sealed class WindowsFilePicker : IFilePickerHostCapability, IDisposable
{
    private readonly nint _owner;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly object _gate = new();
    private Task<FilePickResult>? _pending;
    private bool _closed;

    /// <summary>Create on the thread owning the HWND; dispose before that HWND is destroyed.</summary>
    public WindowsFilePicker(nint owner)
    {
        if (owner == 0 || GetWindowThreadProcessId(owner, out var process) != GetCurrentThreadId() || process != Environment.ProcessId)
            throw new ArgumentException("A live HWND on the calling UI thread is required.", nameof(owner));
        _owner = owner;
    }

    public ValueTask<FilePickResult> PickFilesAsync(FilePickOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        cancellationToken.ThrowIfCancellationRequested();
        var extensions = options.Extensions is { Length: > 0 } values ? values.ToArray() : ["*"];
        foreach (var extension in extensions)
            if (extension is null || (extension != "*" && (extension.Length < 2 || extension[0] != '.' || extension[1..].Any(c => !char.IsLetterOrDigit(c)))))
                throw new ArgumentException("File filters must be '*' or extensions such as '.txt'.", nameof(options));
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_closed, this);
            if (_pending is { IsCompleted: false }) return ValueTask.FromResult(new FilePickResult(FilePickStatus.failed, [], "A file picker is already open for this window."));
            var completion = new TaskCompletionSource<FilePickResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
            var thread = new Thread(() => RunDialog(options.AllowMultiple, extensions, linked, completion))
                { IsBackground = true, Name = "Doroti file picker STA" };
            thread.SetApartmentState(ApartmentState.STA);
            _pending = completion.Task;
            try { thread.Start(); }
            catch { linked.Dispose(); _pending = null; throw; }
            return new(completion.Task);
        }
    }

    private void RunDialog(bool multiple, string[] extensions, CancellationTokenSource linked, TaskCompletionSource<FilePickResult> completion)
    {
        var grants = new List<IPickedFile>();
        FilePickResult? result = null;
        Exception? failure = null;
        var initialized = false;
        try
        {
            linked.Token.ThrowIfCancellationRequested();
            Marshal.ThrowExceptionForHR(OleInitialize(0));
            initialized = true;
            using var dispatcher = new WindowsPlatformViewDispatcher();
            try
            {
                using var picker = new WindowsOpenFileDialog();
                // Close runs on the dialog apartment through the modal message loop.
                using var cancellation = linked.Token.Register(() => dispatcher.Post(_ => picker.Cancel(), null));
                linked.Token.ThrowIfCancellationRequested();
                var paths = picker.Show(_owner, multiple, extensions);
                linked.Token.ThrowIfCancellationRequested();
                foreach (var path in paths) grants.Add(new WindowsReadFile(path));
                result = new(grants.Count == 0 ? FilePickStatus.cancelled : FilePickStatus.selected, grants.ToArray());
            }
            finally { dispatcher.DrainShutdown(Task.CompletedTask); }
            linked.Token.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException error) { failure = error; }
        catch (UnauthorizedAccessException error) { result = new(FilePickStatus.denied, [], error.Message); }
        catch (Exception error) when (error is IOException or COMException)
        { result = new(FilePickStatus.failed, [], error.Message); }
        catch (Exception error) { failure = error; }
        finally
        {
            if (failure is not null || result?.Status != FilePickStatus.selected)
                foreach (var file in grants) file.Dispose();
            if (initialized) OleUninitialize();
            linked.Dispose();
        }
        if (failure is OperationCanceledException) completion.TrySetCanceled();
        else if (failure is not null) completion.TrySetException(failure);
        else completion.TrySetResult(result!);
    }

    public void Dispose()
    {
        Task? pending;
        lock (_gate) { if (_closed) return; _closed = true; pending = _pending; }
        _lifetime.Cancel();
        if (pending is not null)
        {
            try { pending.WaitAsync(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult(); }
            catch (OperationCanceledException) { }
        }
        _lifetime.Dispose();
    }

    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(nint window, out int process);
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
    [DllImport("ole32.dll")] private static extern int OleInitialize(nint reserved);
    [DllImport("ole32.dll")] private static extern void OleUninitialize();
}
