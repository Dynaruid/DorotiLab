using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Plugins;

public sealed record NativeFeatureCapabilities(bool FilePicker, bool UrlLauncher);
public sealed record PickedFileInfo(string Token, string Name, long Length);
internal sealed record FeatureRequest(string Operation, FilePickOptions? Options = null,
    string? Value = null, long Offset = 0, int Count = 0);
internal sealed record FeatureReply(string Status, string? Message = null,
    PickedFileInfo[]? Files = null, byte[]? Bytes = null, NativeFeatureCapabilities? Capabilities = null);

[JsonSerializable(typeof(FeatureRequest))]
[JsonSerializable(typeof(FeatureReply))]
internal partial class FeatureJson : JsonSerializerContext;

/// <summary>Stateless application handler. File grants belong to the calling view's context.</summary>
public sealed class NativeFeaturesHandler : IDorotiViewPluginHandler
{
    public const string Channel = "doroti/native-features";
    public string PluginId => "doroti.native-features";
    public string AbiVersion => "1";

    public ValueTask<ReadOnlyMemory<byte>?> HandleAsync(string channel, string codec,
        ReadOnlyMemory<byte>? message, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("Native features require a view plugin context.");

    public async ValueTask<ReadOnlyMemory<byte>?> HandleAsync(DorotiPluginContext context, string channel,
        string codec, ReadOnlyMemory<byte>? message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (channel != Channel || codec != "json") throw new InvalidDataException("Native features require their registered channel and json codec.");
        var request = message is { } bytes
            ? JsonSerializer.Deserialize(bytes.Span, FeatureJson.Default.FeatureRequest) : null;
        if (request is null) throw new InvalidDataException("Missing native feature request.");
        FeatureReply reply;
        try { reply = await Execute(context, request, cancellationToken).ConfigureAwait(false); }
        catch (UnauthorizedAccessException error) { reply = new("denied", error.Message); }
        catch (NotSupportedException error) { reply = new("unsupported", error.Message); }
        catch (IOException error) { reply = new("failed", error.Message); }
        return JsonSerializer.SerializeToUtf8Bytes(reply, FeatureJson.Default.FeatureReply);
    }

    private static async ValueTask<FeatureReply> Execute(DorotiPluginContext context, FeatureRequest request, CancellationToken token)
    {
        var capabilities = context.Capabilities;
        bool Has(string id) => capabilities.RegisteredIds.Contains(id);
        T Host<T>(string id) where T : class => capabilities.Require<T>(0, id, DorotiUiInvocation.Managed(Channel));
        switch (request.Operation)
        {
            case "capabilities":
                return new("ok", Capabilities: new(Has(DorotiCapabilityIds.FilePicker), Has(DorotiCapabilityIds.UrlLauncher)));
            case "pick":
                if (!Has(DorotiCapabilityIds.FilePicker)) return new("unsupported");
                var result = await Host<IFilePickerHostCapability>(DorotiCapabilityIds.FilePicker)
                    .PickFilesAsync(request.Options ?? new(), token).ConfigureAwait(false);
                var retained = new List<string>();
                try
                {
                    token.ThrowIfCancellationRequested();
                    var files = result.Files.Select(file =>
                    {
                        var id = context.Retain(file, token);
                        retained.Add(id);
                        return new PickedFileInfo(id, file.Name, file.Length);
                    }).ToArray();
                    return new(result.Status.ToString(), result.Message, files);
                }
                catch
                {
                    foreach (var id in retained) context.Release(id);
                    foreach (var file in result.Files) file.Dispose();
                    throw;
                }
            case "read":
                if (request.Offset < 0 || request.Count is < 0 or > 65536) throw new ArgumentOutOfRangeException(nameof(request));
                var buffer = new byte[request.Count];
                var count = await context.Require<IPickedFile>(request.Value ?? "")
                    .ReadAsync(request.Offset, buffer, token).ConfigureAwait(false);
                return new("ok", Bytes: buffer.AsSpan(0, count).ToArray());
            case "release":
                context.Release(request.Value ?? "");
                return new("ok");
            case "launch":
                if (!Uri.TryCreate(request.Value, UriKind.Absolute, out var uri)) return new("invalidUrl");
                // External programs/files require a separate explicit capability.
                if (uri.Scheme is not ("http" or "https" or "mailto")) return new("unsupported", "Supported schemes: http, https, mailto.");
                if (!Has(DorotiCapabilityIds.UrlLauncher)) return new("unsupported");
                var launched = await Host<IUrlLauncherHostCapability>(DorotiCapabilityIds.UrlLauncher)
                    .LaunchUrlAsync(uri.AbsoluteUri, token).ConfigureAwait(false);
                return new(launched.Status.ToString(), launched.Message);
            default: throw new InvalidDataException($"Unknown native feature operation '{request.Operation}'.");
        }
    }
}

/// <summary>The same client is used by every host. Retain one instance per view.</summary>
public sealed class NativeFeatures(IPlatformMessageHostCapability messages)
{
    public static NativeFeatures ForView(DorotiView view)
    {
        ArgumentNullException.ThrowIfNull(view);
        var invocation = DorotiUiInvocation.Managed("NativeFeatures.ForView");
        if (!view.registeredCapabilityIds.Contains(DorotiCapabilityIds.PlatformPlugins)
            || !view.RequireCapability<IPlatformPluginHostCapability>(DorotiCapabilityIds.PlatformPlugins, invocation)
                .RegisteredChannels.Contains(NativeFeaturesHandler.Channel))
            return new(new UnsupportedMessages());
        return new(view.RequireCapability<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, invocation));
    }

    private sealed class UnsupportedMessages : IPlatformMessageHostCapability
    {
        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult<ReadOnlyMemory<byte>?>(JsonSerializer.SerializeToUtf8Bytes(new FeatureReply("unsupported"), FeatureJson.Default.FeatureReply));
        }
        public void SetMessageHandler(string channel, PlatformMessageHandler? handler) => throw new NotSupportedException();
    }

    private async ValueTask<FeatureReply> Send(FeatureRequest request, CancellationToken token = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(request, FeatureJson.Default.FeatureRequest);
        var reply = await messages.SendAsync(NativeFeaturesHandler.Channel, bytes, token).ConfigureAwait(false);
        return reply is { } value
            ? JsonSerializer.Deserialize(value.Span, FeatureJson.Default.FeatureReply) ?? throw new InvalidDataException("Empty feature reply.")
            : throw new InvalidDataException("Missing feature handler reply.");
    }

    public async ValueTask<NativeFeatureCapabilities> GetCapabilitiesAsync(CancellationToken token = default) =>
        (await Send(new("capabilities"), token).ConfigureAwait(false)).Capabilities ?? new(false, false);

    public async ValueTask<FileSelection> PickFilesAsync(FilePickOptions? options = null, CancellationToken token = default)
    {
        var reply = await Send(new("pick", options), token).ConfigureAwait(false);
        return new(Enum.Parse<FilePickStatus>(reply.Status),
            (reply.Files ?? []).Select(file => new SelectedFile(this, file)).ToArray(), reply.Message);
    }

    public async ValueTask<UrlLaunchResult> LaunchUrlAsync(string url, CancellationToken token = default)
    {
        var reply = await Send(new("launch", Value: url), token).ConfigureAwait(false);
        return new(reply.Status == "denied" ? UrlLaunchStatus.blocked : Enum.Parse<UrlLaunchStatus>(reply.Status), reply.Message);
    }

    public sealed class SelectedFile(NativeFeatures owner, PickedFileInfo info) : IAsyncDisposable
    {
        private int _disposed;
        public string Name => info.Name;
        public long Length => info.Length;
        /// <summary>Bounded random access; at most 64 KiB per request. No local path is exposed.</summary>
        public async ValueTask<int> ReadAsync(long offset, Memory<byte> buffer, CancellationToken token = default)
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
            var reply = await owner.Send(new("read", Value: info.Token, Offset: offset, Count: Math.Min(buffer.Length, 65536)), token).ConfigureAwait(false);
            if (reply.Status == "denied") throw new UnauthorizedAccessException(reply.Message);
            if (reply.Status != "ok") throw new IOException(reply.Message ?? reply.Status);
            (reply.Bytes ?? []).CopyTo(buffer);
            return reply.Bytes?.Length ?? 0;
        }
        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
                try { await owner.Send(new("release", Value: info.Token)).ConfigureAwait(false); }
                catch (ObjectDisposedException) { /* Owner already revoked every grant. */ }
                catch (OperationCanceledException) { /* Owner is revoking the grant; release has no caller token. */ }
        }
    }
}

public sealed record FileSelection(FilePickStatus Status, IReadOnlyList<NativeFeatures.SelectedFile> Files, string? Message = null) : IAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        List<Exception> errors = [];
        foreach (var file in Files)
            try { await file.DisposeAsync().ConfigureAwait(false); } catch (Exception error) { errors.Add(error); }
        if (errors.Count != 0) throw new AggregateException(errors);
    }
}
