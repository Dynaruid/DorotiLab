// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/_network_image_io.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Painting;

internal delegate Future<Codec> _SimpleDecoderCallback___network_image_io(ImmutableBuffer buffer);

public class NetworkImageIo : ImageProvider<NetworkImageIo>, NetworkImage
{
    public virtual string url { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;
    public virtual DartMap<string, string>? headers { get; private set; }
    public virtual WebHtmlElementStrategy webHtmlElementStrategy { get; private set; } = default!;
    internal static global::Doroti.Flutter.Runtime.HttpClient _sharedHttpClient = ((Func<global::Doroti.Flutter.Runtime.HttpClient>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Runtime.HttpClient();
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
        var chunkEvents__1719 = new StreamController<ImageChunkEvent>();
        return new MultiFrameImageStreamCompleter(codec: _loadAsync(((NetworkImageIo?)(object?)key)!, chunkEvents__1719, decode: (ImmutableBuffer __buffer) => decode(__buffer, false, null, null)), chunkEvents: chunkEvents__1719.stream, scale: ((NetworkImageIo)key).scale, debugLabel: ((NetworkImageIo)key).url, informationCollector: (() => new List<DiagnosticsNode> { new DiagnosticsProperty<dynamic>("Image provider", this), new DiagnosticsProperty<NetworkImageIo>("Image key", key) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ImageStreamCompleter loadImage(NetworkImageIo key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode)
    {
        var chunkEvents__2562 = new StreamController<ImageChunkEvent>();
        return new MultiFrameImageStreamCompleter(codec: _loadAsync(((NetworkImageIo?)(object?)key)!, chunkEvents__2562, decode: (ImmutableBuffer __buffer) => decode(__buffer, null)), chunkEvents: chunkEvents__2562.stream, scale: ((NetworkImageIo)key).scale, debugLabel: ((NetworkImageIo)key).url, informationCollector: (() => new List<DiagnosticsNode> { new DiagnosticsProperty<dynamic>("Image provider", this), new DiagnosticsProperty<NetworkImageIo>("Image key", key) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Flutter.Runtime.HttpClient _httpClient
    {
        get
        {
            global::Doroti.Flutter.Runtime.HttpClient? client__3477 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((global::Doroti.Generated.Framework.Painting.DebugLibrary.debugNetworkImageHttpClientProvider is not null))
                    {
                        client__3477 = global::Doroti.Generated.Framework.Painting.DebugLibrary.debugNetworkImageHttpClientProvider!();
                    }
                    return true;
                });
            return (client__3477 ?? _sharedHttpClient);
            return default!;
        }
    }
    internal async virtual Future<global::Doroti.Flutter.Ui.Codec> _loadAsync(NetworkImageIo key, StreamController<ImageChunkEvent> chunkEvents, Func<ImmutableBuffer, Future<Codec>> decode)
    {
        try
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(key, this)));
            DartUri resolved__3914 = DartUri.@base.resolve(((NetworkImageIo)key).url);
            global::Doroti.Flutter.Runtime.HttpClientRequest request__3983 = await _httpClient.getUrl(resolved__3914);
            this.headers?.forEach(((name, value) =>
            {
                request__3983.headers.add(name, value);
            }));
            global::Doroti.Flutter.Runtime.HttpClientResponse response__4166 = await request__3983.close();
            if ((response__4166.statusCode != HttpStatus.ok))
            {
                await response__4166.drain<List<long>>(new List<long>());
                throw new NetworkImageLoadException(statusCode: response__4166.statusCode, uri: resolved__3914);
            }
            Uint8List bytes__4665 = await global::Doroti.Generated.Framework.Foundation.Consolidate_responseLibrary.consolidateHttpClientResponseBytes(response__4166, onBytesReceived: ((cumulative, total) =>
            {
                chunkEvents.add(new ImageChunkEvent(cumulativeBytesLoaded: cumulative, expectedTotalBytes: total));
            }));
            if ((bytes__4665.lengthInBytes == 0L))
            {
                throw new Exception($"NetworkImage is an empty file: {resolved__3914}");
            }
            return await decode(await Dart_uiLibrary.ImmutableBuffer.fromUint8List(bytes__4665));
        }
        catch (Exception e__5143)
        {
            DartAsyncRuntime.scheduleMicrotask((() =>
            {
                PaintingBinding.instance.imageCache.evict(key);
            }));
            throw;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as NetworkImageIo;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is NetworkImageIo) && (((NetworkImageIo)((NetworkImageIo)__other)).url == this.url)) && (((NetworkImageIo)((NetworkImageIo)__other)).scale == this.scale)) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mapEquals(((NetworkImageIo)((NetworkImageIo)__other)).headers, this.headers));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.url, this.scale, new MapEquality<string, string>().hash(this.headers));
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "NetworkImage"))}(\"{this.url}\", scale: {this.scale.toStringAsFixed(1L)}, webHtmlElementStrategy: {webHtmlElementStrategy.ToString()}, headers: {this.headers})";
    ImageStreamCompleter NetworkImage.loadBuffer(NetworkImage key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode) =>
        loadBuffer((NetworkImageIo)key, decode);
    ImageStreamCompleter NetworkImage.loadImage(NetworkImage key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode) =>
        loadImage((NetworkImageIo)key, decode);
}

