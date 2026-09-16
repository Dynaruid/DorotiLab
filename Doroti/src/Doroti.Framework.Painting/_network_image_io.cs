// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/_network_image_io.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

internal delegate Future<Codec> _SimpleDecoderCallback___network_image_io(ImmutableBuffer buffer);

public class NetworkImageIo : ImageProvider<NetworkImageIo>, NetworkImage
{
    public virtual string url { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;
    public virtual DartMap<string, string>? headers { get; private set; }
    public virtual WebHtmlElementStrategy webHtmlElementStrategy { get; private set; } = default!;
    internal static global::Doroti.Runtime.HttpClient _sharedHttpClient = ((Func<global::Doroti.Runtime.HttpClient>)(() =>
{
    var __cascade = new global::Doroti.Runtime.HttpClient();
    __cascade.autoUncompress = false;
    return __cascade;
}))();
    public NetworkImageIo() { }


    public NetworkImageIo(string url, double scale = 1.0, DartMap<string, string>? headers = null, WebHtmlElementStrategy? webHtmlElementStrategy = null)
    {
        WebHtmlElementStrategy __webHtmlElementStrategy = webHtmlElementStrategy ?? WebHtmlElementStrategy.never;
        this.url = url;
        this.scale = scale;
        this.headers = headers;
        this.webHtmlElementStrategy = __webHtmlElementStrategy;
    }

    public override Future<NetworkImageIo> obtainKey(ImageConfiguration configuration)
    {
        return new SynchronousFuture<NetworkImageIo>(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ImageStreamCompleter loadBuffer(NetworkImageIo key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode)
    {
        var chunkEventsLocal = new StreamController<ImageChunkEvent>();
        return new MultiFrameImageStreamCompleter(codec: _loadAsync(((NetworkImageIo?)(object?)key)!, chunkEventsLocal, decode: (ImmutableBuffer __buffer) => decode(__buffer, false, null, null)), chunkEvents: chunkEventsLocal.stream, scale: key.scale, debugLabel: key.url, informationCollector: () => new List<DiagnosticsNode> { new DiagnosticsProperty<dynamic>("Image provider", this), new DiagnosticsProperty<NetworkImageIo>("Image key", key) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ImageStreamCompleter loadImage(NetworkImageIo key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode)
    {
        var chunkEventsLocal = new StreamController<ImageChunkEvent>();
        return new MultiFrameImageStreamCompleter(codec: _loadAsync(((NetworkImageIo?)(object?)key)!, chunkEventsLocal, decode: (ImmutableBuffer __buffer) => decode(__buffer, null)), chunkEvents: chunkEventsLocal.stream, scale: key.scale, debugLabel: key.url, informationCollector: () => new List<DiagnosticsNode> { new DiagnosticsProperty<dynamic>("Image provider", this), new DiagnosticsProperty<NetworkImageIo>("Image key", key) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Runtime.HttpClient _httpClient
    {
        get
        {
            global::Doroti.Runtime.HttpClient? client = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (DebugLibrary.debugNetworkImageHttpClientProvider is not null)
                    {
                        client = DebugLibrary.debugNetworkImageHttpClientProvider!();
                    }
                    return true;
                });
            return client ?? _sharedHttpClient;
        }
    }
    internal async virtual Future<global::Doroti.Ui.Codec> _loadAsync(NetworkImageIo key, StreamController<ImageChunkEvent> chunkEvents, Func<ImmutableBuffer, Future<Codec>> decode)
    {
        try
        {
            DartRuntimePrimitives.Assert(() => Equals(key, this));
            DartUri resolved = DartUri.@base.resolve(key.url);
            global::Doroti.Runtime.HttpClientRequest request = await _httpClient.getUrl(resolved);
            headers?.forEach((name, value) =>
            {
                request.headers.add(name, value);
            });
            global::Doroti.Runtime.HttpClientResponse response = await request.close();
            if (response.statusCode != HttpStatus.ok)
            {
                await response.drain<List<long>>(new List<long>());
                throw new NetworkImageLoadException(statusCode: response.statusCode, uri: resolved);
            }
            Uint8List bytes = await Consolidate_responseLibrary.consolidateHttpClientResponseBytes(response, onBytesReceived: (cumulative, total) =>
            {
                chunkEvents.add(new ImageChunkEvent(cumulativeBytesLoaded: cumulative, expectedTotalBytes: total));
            });
            if (bytes.lengthInBytes == 0L)
            {
                throw new Exception($"NetworkImage is an empty file: {resolved}");
            }
            return await decode(await Dart_uiLibrary.ImmutableBuffer.fromUint8List(bytes));
        }
        catch (Exception)
        {
            DartAsyncRuntime.scheduleMicrotask(() =>
            {
                PaintingBinding.instance.imageCache.evict(key);
            });
            throw;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as NetworkImageIo;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is NetworkImageIo) && (__other.url == url) && (__other.scale == scale) && CollectionsLibrary.mapEquals(__other.headers, headers);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(url, scale, new MapEquality<string, string>().hash(headers));
    public override string ToString() => $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "NetworkImage")}(\"{url}\", scale: {scale.toStringAsFixed(1L)}, webHtmlElementStrategy: {webHtmlElementStrategy.ToString()}, headers: {headers})";
    ImageStreamCompleter NetworkImage.loadBuffer(NetworkImage key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode) =>
        loadBuffer((NetworkImageIo)key, decode);
    ImageStreamCompleter NetworkImage.loadImage(NetworkImage key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode) =>
        loadImage((NetworkImageIo)key, decode);
}

