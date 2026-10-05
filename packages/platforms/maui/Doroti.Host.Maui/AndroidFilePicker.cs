#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Doroti.Ui;
using Microsoft.Maui.ApplicationModel;
using System.Collections.Concurrent;

namespace Doroti.Host.Maui;

/// <summary>Storage Access Framework grants are view scoped; no broad storage permission is requested.</summary>
internal sealed class AndroidFilePicker : IFilePickerHostCapability, IDisposable
{
    private readonly CancellationTokenSource _lifetime = new();
    private int _busy;
    private bool _disposed;
    public async ValueTask<FilePickResult> PickFilesAsync(FilePickOptions options, CancellationToken cancellationToken = default)
    {
        options = options.Normalize();
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
        linked.Token.ThrowIfCancellationRequested();
        if (Interlocked.Exchange(ref _busy, 1) != 0) return new(FilePickStatus.failed, [], "A picker is already open.");
        try { return await AndroidFilePickerActivity.Pick(options, linked.Token); }
        finally { Volatile.Write(ref _busy, 0); }
    }
    public void Dispose() { if (_disposed) return; _disposed = true; _lifetime.Cancel(); }
}

[Activity(Exported = false, Theme = "@android:style/Theme.Translucent.NoTitleBar",
    ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
public sealed class AndroidFilePickerActivity : Activity
{
    private const int RequestCode = 4701;
    private sealed class Request(FilePickOptions options)
    {
        public FilePickOptions Options { get; } = options;
        public TaskCompletionSource<FilePickResult> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public AndroidFilePickerActivity? Activity;
    }
    private static readonly ConcurrentDictionary<string, Request> Requests = new();
    private string? _id;
    internal static async Task<FilePickResult> Pick(FilePickOptions options, CancellationToken token)
    {
        var id = Guid.NewGuid().ToString("N");
        var request = new Request(options);
        Requests[id] = request;
        using var cancellation = token.Register(() => MainThread.BeginInvokeOnMainThread(() =>
        {
            request.Activity?.FinishActivity(RequestCode);
            request.Activity?.Finish();
            request.Completion.TrySetCanceled(token);
        }));
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                token.ThrowIfCancellationRequested();
                var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity ?? throw new InvalidOperationException("No foreground Android activity.");
                using var intent = new Intent(activity, typeof(AndroidFilePickerActivity));
                intent.PutExtra("doroti.file.request", id);
                activity.StartActivity(intent);
            });
            return await request.Completion.Task;
        }
        finally { Requests.TryRemove(id, out _); }
    }
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        _id = Intent?.GetStringExtra("doroti.file.request");
        if (_id is null || !Requests.TryGetValue(_id, out var request) || request.Completion.Task.IsCompleted) { Finish(); return; }
        request.Activity = this;
        if (savedInstanceState is not null) return;
        try
        {
            using var intent = new Intent(Intent.ActionOpenDocument);
            intent.AddCategory(Intent.CategoryOpenable);
            intent.SetType("*/*");
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            intent.PutExtra(Intent.ExtraAllowMultiple, request.Options.AllowMultiple);
            var extensions = request.Options.Extensions ?? [];
            var mime = extensions.Select(e => Android.Webkit.MimeTypeMap.Singleton?.GetMimeTypeFromExtension(e.TrimStart('.').ToLowerInvariant())).ToArray();
            // Unknown extensions must not silently exclude valid files; enforce names after selection.
            if (mime.Length > 0 && mime.All(m => m is not null)) intent.PutExtra(Intent.ExtraMimeTypes, mime!);
            StartActivityForResult(intent, RequestCode);
        }
        catch (Exception error) { request.Completion.TrySetResult(new(FilePickStatus.failed, [], error.Message)); Finish(); }
    }
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode != RequestCode) return;
        var files = new List<IPickedFile>();
        try
        {
            if (_id is null || !Requests.TryGetValue(_id, out var request)) return;
            FilePickResult result;
            if (resultCode != Result.Ok) result = new(FilePickStatus.cancelled, []);
            else
            {
                var uris = new List<Android.Net.Uri>();
                if (data?.ClipData is { } clip)
                    for (var i = 0; i < clip.ItemCount; i++) { if (clip.GetItemAt(i)?.Uri is { } uri) uris.Add(uri); }
                else if (data?.Data is { } uri) uris.Add(uri);
                foreach (var uri in uris.DistinctBy(u => u.ToString()))
                {
                    var file = new AndroidPickedFile(uri);
                    files.Add(file);
                    if (!FilePickFilters.Matches(file.Name, request.Options.Extensions ?? []))
                        throw new NotSupportedException("The selected file does not match the requested extensions.");
                    if (!request.Options.AllowMultiple) break;
                }
                result = new(files.Count > 0 ? FilePickStatus.selected : FilePickStatus.cancelled, files.ToArray());
            }
            if (request.Completion.TrySetResult(result)) files.Clear();
        }
        catch (Exception error)
        {
            if (_id is not null && Requests.TryGetValue(_id, out var request))
                request.Completion.TrySetResult(new(error is Java.Lang.SecurityException ? FilePickStatus.denied : FilePickStatus.failed, [], error.Message));
        }
        finally { foreach (var file in files) file.Dispose(); Finish(); }
    }
    protected override void OnDestroy()
    {
        if (_id is not null && Requests.TryGetValue(_id, out var request) && ReferenceEquals(request.Activity, this))
        {
            request.Activity = null;
            if (!IsChangingConfigurations) request.Completion.TrySetResult(new(FilePickStatus.cancelled, []));
        }
        base.OnDestroy();
    }
}

internal sealed class AndroidPickedFile : IPickedFile
{
    private readonly Android.Net.Uri _uri;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly SemaphoreSlim _gate = new(1);
    private bool _disposed;
    public string Name { get; }
    public long Length { get; }
    public AndroidPickedFile(Android.Net.Uri uri)
    {
        _uri = Android.Net.Uri.Parse(uri.ToString())!;
        var resolver = Android.App.Application.Context.ContentResolver!;
        using var cursor = resolver.Query(_uri, [IOpenableColumns.DisplayName!, IOpenableColumns.Size!], null, null, null);
        if (cursor is null || !cursor.MoveToFirst()) throw new IOException("Document metadata is unavailable.");
        Name = cursor.GetString(0) ?? "document";
        if (cursor.IsNull(1)) throw new NotSupportedException("Documents of unknown length are not supported by this random-access grant.");
        Length = cursor.GetLong(1);
        if (Length < 0) throw new IOException("Invalid document length.");
    }
    public async ValueTask<int> ReadAsync(long offset, Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
        await _gate.WaitAsync(linked.Token);
        try
        {
            await using var stream = Android.App.Application.Context.ContentResolver!.OpenInputStream(_uri) ?? throw new IOException("Cannot open document.");
            // Providers may expose pipes. Reopen and skip with bounded memory when seeking is unavailable.
            if (stream.CanSeek) stream.Seek(offset, SeekOrigin.Begin);
            else
            {
                var skip = new byte[65536];
                while (offset > 0)
                {
                    var read = await stream.ReadAsync(skip.AsMemory(0, (int)Math.Min(offset, skip.Length)), linked.Token);
                    if (read == 0) return 0;
                    offset -= read;
                }
            }
            var count = await stream.ReadAsync(buffer[..Math.Min(buffer.Length, 65536)], linked.Token);
            linked.Token.ThrowIfCancellationRequested();
            return count;
        }
        finally { _gate.Release(); }
    }
    public void Dispose() { if (_disposed) return; _disposed = true; _lifetime.Cancel(); }
}
#endif

