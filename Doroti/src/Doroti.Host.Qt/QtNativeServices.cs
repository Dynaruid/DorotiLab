using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Ui;
using Microsoft.Win32.SafeHandles;

namespace Doroti.Host.Qt;

/// <summary>View-owned asynchronous Qt dialog and synchronous OS drop negotiation.</summary>
internal sealed class QtNativeServices : IFilePickerHostCapability, IOsDragSourceHostCapability, IDisposable
{
    private readonly nint _window;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly HashSet<ulong> _requests = [];
    private GCHandle _dropContext;
    private bool _disposed;
    private int _picking;
    private readonly HashSet<ulong> _drags = [];
    private static long _nextRequest;
    internal OsDropReceiver Drop { get; }

    internal unsafe QtNativeServices(nint window, Action<Action> dispatch)
    {
        _window = window;
        Drop = new(new(true, false, OsDropAction.Copy,
            [OsDropFormats.Files, OsDropFormats.Text, OsDropFormats.UriList]), dispatch);
        _dropContext = GCHandle.Alloc(this);
        try { Check(BindDrop(window, &OnDrop, GCHandle.ToIntPtr(_dropContext))); }
        catch { _dropContext.Free(); Drop.Dispose(); _lifetime.Dispose(); throw; }
    }

    public OsDropAction Actions => OsDropAction.Copy | OsDropAction.Move | OsDropAction.Link;
    public async ValueTask<OsDragResult> StartDragAsync(OsDragSourceData data, OsDropAction actions = OsDropAction.Copy,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();
        if (actions == OsDropAction.None || (actions & ~Actions) != 0) throw new ArgumentOutOfRangeException(nameof(actions));
        if (data.Text is null && (data.Uris is null || data.Uris.Count == 0)) throw new ArgumentException("Drag data is empty.", nameof(data));
        if (data.ImagePng.Length > 1024 * 1024 || data.Text?.Length > 1024 * 1024 || data.Uris?.Count > 1024)
            throw new ArgumentException("Drag data exceeds its bounded payload limit.", nameof(data));
        var uris = data.Uris?.Select(uri => uri.IsAbsoluteUri ? uri.AbsoluteUri : throw new ArgumentException("Drag URIs must be absolute.")).ToArray() ?? [];
        var bytes = JsonSerializer.SerializeToUtf8Bytes(new { text = data.Text, uris, actions = (int)actions,
            image = Convert.ToBase64String(data.ImagePng.Span), hotX = data.ImageHotspot?.dx ?? 0, hotY = data.ImageHotspot?.dy ?? 0 });
        if (bytes.Length > 8 * 1024 * 1024) throw new ArgumentException("Drag payload exceeds 8 MiB.", nameof(data));
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
        var request = new DragRequest(this, (ulong)Interlocked.Increment(ref _nextRequest));
        await QtApplicationDispatcher.Post(() => StartDrag(request, bytes), linked.Token).ConfigureAwait(false);
        using var registration = linked.Token.Register(() => ObserveCancelDrag(request.Id));
        var result = await request.Completion.Task.ConfigureAwait(false);
        linked.Token.ThrowIfCancellationRequested();
        return result;
    }
    private unsafe void StartDrag(DragRequest request, byte[] bytes)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var handle = GCHandle.Alloc(request);
        _drags.Add(request.Id);
        try { fixed (byte* data = bytes) Check(DragNative(_window, request.Id, new(data, (ulong)bytes.Length), &OnDragged, GCHandle.ToIntPtr(handle))); }
        catch { _drags.Remove(request.Id); handle.Free(); throw; }
    }
    private sealed record DragRequest(QtNativeServices Owner, ulong Id)
    {
        internal TaskCompletionSource<OsDragResult> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnDragged(nint context, int action, QtNativeV2.Utf8 unused)
    {
        var handle = GCHandle.FromIntPtr(context);
        var request = (DragRequest)handle.Target!;
        handle.Free();
        request.Owner._drags.Remove(request.Id);
        request.Completion.TrySetResult(new((OsDropAction)action, action == 0));
    }
    private static async void ObserveCancelDrag(ulong id)
    {
        try { await QtApplicationDispatcher.Post(() => CancelDrag(id)).ConfigureAwait(false); }
        catch (ObjectDisposedException) { }
        catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
    }

    public async ValueTask<FilePickResult> PickFilesAsync(FilePickOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        cancellationToken.ThrowIfCancellationRequested();
        ObjectDisposedException.ThrowIf(_disposed, this);
        var normalized = FilePickFilters.Normalize(options.Extensions);
        var extensions = normalized.Length == 0 ? ["*"] : normalized;
        if (Interlocked.Exchange(ref _picking, 1) != 0)
            return new(FilePickStatus.failed, [], "A file picker is already open for this window.");
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
        var request = new PickRequest(this, (ulong)Interlocked.Increment(ref _nextRequest), linked.Token, normalized);
        try
        {
            await QtApplicationDispatcher.Post(() => Start(request, options.AllowMultiple, extensions), linked.Token).ConfigureAwait(false);
            using var registration = linked.Token.Register(() => ObserveCancel(request.Id));
            var result = await request.Completion.Task.ConfigureAwait(false);
            if (linked.IsCancellationRequested)
            {
                foreach (var file in result.Files) file.Dispose();
                linked.Token.ThrowIfCancellationRequested();
            }
            return result;
        }
        finally { Interlocked.Exchange(ref _picking, 0); }
    }
    private static async void ObserveCancel(ulong id)
    {
        try { await QtApplicationDispatcher.Post(() => CancelPicker(id)).ConfigureAwait(false); }
        catch (ObjectDisposedException) { /* Native owner teardown completes the request. */ }
        catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
    }
    private unsafe void Start(PickRequest request, bool multiple, string[] extensions)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var bytes = JsonSerializer.SerializeToUtf8Bytes(new { multiple, extensions });
        var handle = GCHandle.Alloc(request);
        _requests.Add(request.Id);
        try
        {
            fixed (byte* data = bytes) Check(Pick(_window, request.Id, new(data, (ulong)bytes.Length), &OnPicked, GCHandle.ToIntPtr(handle)));
        }
        catch { _requests.Remove(request.Id); handle.Free(); throw; }
    }
    private sealed record PickRequest(QtNativeServices Owner, ulong Id, CancellationToken Token, string[] Extensions)
    {
        internal TaskCompletionSource<FilePickResult> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnPicked(nint context, int status, QtNativeV2.Utf8 json)
    {
        var handle = GCHandle.FromIntPtr(context);
        var request = (PickRequest)handle.Target!;
        handle.Free();
        request.Owner._requests.Remove(request.Id);
        List<IPickedFile> files = [];
        try
        {
            request.Token.ThrowIfCancellationRequested();
            if (status == 0)
            {
                using var document = Parse(json);
                foreach (var path in document.RootElement.EnumerateArray()) files.Add(new QtReadFile(path.GetString()!));
            }
            var result = FilePickFilters.Enforce(new(status == 0 ? FilePickStatus.selected : FilePickStatus.cancelled, files), request.Extensions);
            if (!request.Completion.TrySetResult(result)) foreach (var file in result.Files) file.Dispose();
        }
        catch (Exception error)
        {
            foreach (var file in files) file.Dispose();
            if (error is OperationCanceledException) request.Completion.TrySetCanceled(request.Token);
            else if (error is IOException or UnauthorizedAccessException)
                request.Completion.TrySetResult(new(error is UnauthorizedAccessException ? FilePickStatus.denied : FilePickStatus.failed, [], error.Message));
            else request.Completion.TrySetException(error);
        }
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int OnDrop(nint context, QtNativeV2.Utf8 json)
    {
        try
        {
            var owner = (QtNativeServices)GCHandle.FromIntPtr(context).Target!;
            if (owner._disposed) return 0;
            using var document = Parse(json);
            var item = document.RootElement;
            var phase = (OsDropPhase)item.GetProperty("phase").GetInt32();
            if (phase == OsDropPhase.Leave)
            { owner.Drop.Hover(phase, new(Offset.zero, [], OsDropAction.None)); return 0; }
            var offer = new OsDropOffer(new(item.GetProperty("x").GetDouble(), item.GetProperty("y").GetDouble()),
                item.GetProperty("formats").EnumerateArray().Select(value => value.GetString()!).ToArray(),
                item.GetProperty("copy").GetBoolean() ? OsDropAction.Copy : OsDropAction.None);
            if (phase != OsDropPhase.Drop) return (int)owner.Drop.Hover(phase, offer);
            return (int)owner.Drop.Drop(offer, formats =>
            {
                List<IPickedFile> files = [];
                try
                {
                    if (formats.Contains(OsDropFormats.Files))
                        foreach (var path in item.GetProperty("paths").EnumerateArray()) files.Add(new QtReadFile(path.GetString()!));
                    var uris = formats.Contains(OsDropFormats.UriList)
                        ? item.GetProperty("uris").EnumerateArray().Select(value => new Uri(value.GetString()!, UriKind.Absolute)).ToArray() : [];
                    return new(files, formats.Contains(OsDropFormats.Text) ? item.GetProperty("text").GetString() : null, uris);
                }
                catch { foreach (var file in files) file.Dispose(); throw; }
            });
        }
        catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); return 0; }
    }
    private static unsafe JsonDocument Parse(QtNativeV2.Utf8 json)
    {
        if (json.Length > 4 * 1024 * 1024 || json.Data == null) throw new InvalidDataException("Qt service payload exceeds its limit.");
        return JsonDocument.Parse(new ReadOnlySpan<byte>(json.Data, (int)json.Length).ToArray());
    }
    public unsafe void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        // Called on the GUI thread before QWindow destruction ends.
        _lifetime.Cancel();
        foreach (var id in _requests.ToArray()) CancelPicker(id);
        foreach (var id in _drags.ToArray()) CancelDrag(id);
        BindDrop(_window, null, 0);
        if (_dropContext.IsAllocated) _dropContext.Free();
        Drop.Dispose();
        _lifetime.Dispose();
    }
    private static void Check(int status)
    {
        if (status != 0) throw new InvalidOperationException($"Qt services ABI failed ({status}). Rebuild the app-owned native shim.");
    }
    [DllImport("doroti_qt_host", EntryPoint = "doroti_qt_drag_v1", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int DragNative(nint window, ulong id, QtNativeV2.Utf8 data, delegate* unmanaged[Cdecl]<nint, int, QtNativeV2.Utf8, void> callback, nint context);
    [DllImport("doroti_qt_host", EntryPoint = "doroti_qt_cancel_drag_v1", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CancelDrag(ulong id);
    [DllImport("doroti_qt_host", EntryPoint = "doroti_qt_pick_files_v1", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int Pick(nint window, ulong id, QtNativeV2.Utf8 options, delegate* unmanaged[Cdecl]<nint, int, QtNativeV2.Utf8, void> callback, nint context);
    [DllImport("doroti_qt_host", EntryPoint = "doroti_qt_cancel_picker_v1", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CancelPicker(ulong id);
    [DllImport("doroti_qt_host", EntryPoint = "doroti_qt_drop_bind_v1", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int BindDrop(nint window, delegate* unmanaged[Cdecl]<nint, QtNativeV2.Utf8, int> callback, nint context);
}

internal sealed class QtReadFile : IPickedFile
{
    private readonly SafeFileHandle _handle;
    internal QtReadFile(string path)
    {
        if (!System.IO.Path.IsPathFullyQualified(path) || Directory.Exists(path)) throw new InvalidDataException("A local regular file is required.");
        var fd = OpenReadFile(path);
        if (fd is -1 or -13) throw new UnauthorizedAccessException("File read permission denied.");
        if (fd < 0) throw new IOException($"Cannot open regular file (errno {-fd}).");
        _handle = new SafeFileHandle((nint)fd, ownsHandle: true);
        try { Length = RandomAccess.GetLength(_handle); Name = System.IO.Path.GetFileName(path); }
        catch { _handle.Dispose(); throw; }
    }
    [DllImport("doroti_qt_host", EntryPoint = "doroti_qt_open_read_file_v1", CallingConvention = CallingConvention.Cdecl)]
    private static extern int OpenReadFile([MarshalAs(UnmanagedType.LPUTF8Str)] string path);
    public string Name { get; }
    public long Length { get; }
    public ValueTask<int> ReadAsync(long offset, Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        RandomAccess.ReadAsync(_handle, buffer, offset, cancellationToken);
    public void Dispose() => _handle.Dispose();
}
