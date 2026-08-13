// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/_network_image_web.dart
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

public delegate XMLHttpRequest HttpRequestFactoryIo();

public delegate HTMLImageElement HtmlElementFactoryIo();

internal delegate Future<Codec> _SimpleDecoderCallback___network_image_web(ImmutableBuffer buffer);

public static partial class _network_image_webLibrary
{
    internal static XMLHttpRequest _httpClient()
    {
        return new XMLHttpRequest();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class _network_image_webLibrary
{
    public static Func<XMLHttpRequest> httpRequestFactory = _network_image_webLibrary._httpClient;
}

public static partial class _network_image_webLibrary
{
    public static void debugRestoreHttpRequestFactory()
    {
        _network_image_webLibrary.httpRequestFactory = _network_image_webLibrary._httpClient;
    }
}

public static partial class _network_image_webLibrary
{
    internal static HTMLImageElement _imgElementFactory()
    {
        return ((HTMLImageElement?)(object?)WebLibrary.document.createElement("img"))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class _network_image_webLibrary
{
    public static Func<HTMLImageElement> imgElementFactory = _network_image_webLibrary._imgElementFactory;
}

public static partial class _network_image_webLibrary
{
    public static void debugRestoreImgElementFactory()
    {
        _network_image_webLibrary.imgElementFactory = _network_image_webLibrary._imgElementFactory;
    }
}

public class NetworkImageIo : ImageProvider<NetworkImageIo>, NetworkImage
{
    public virtual string url { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;
    public virtual DartMap<string, string>? headers { get; private set; }
    public virtual WebHtmlElementStrategy webHtmlElementStrategy { get; private set; } = default!;
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
        return new _ForwardingImageStreamCompleter___network_image_web(_loadAsync(((NetworkImageIo?)(object?)key)!, (Func<ImmutableBuffer, bool, long?, long?, Future<Codec>>)decode), informationCollector: _imageStreamInformationCollector(key), debugLabel: key.url);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ImageStreamCompleter loadImage(NetworkImageIo key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode)
    {
        return new _ForwardingImageStreamCompleter___network_image_web(_loadAsync(((NetworkImageIo?)(object?)key)!, (Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>>)decode), informationCollector: _imageStreamInformationCollector(key), debugLabel: key.url);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual InformationCollector? _imageStreamInformationCollector(NetworkImageIo key)
    {
        InformationCollector? collector__3447 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                collector__3447 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<dynamic>("Image provider", this), new DiagnosticsProperty<NetworkImageIo>("Image key", ((NetworkImageIo?)(object?)key)!) });
                return true;
            });
        return collector__3447;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future<ImageStreamCompleter> _loadAsync(NetworkImageIo key, Func<ImmutableBuffer, Future<Codec>> decode)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(key, this)));
        async Future<ImageStreamCompleter> loadViaDecode()
        {
            global::Doroti.Flutter.Ui.Codec codec__4201 = await _fetchImageBytes((Func<ImmutableBuffer, Future<Codec>>)decode);
            return new MultiFrameImageStreamCompleter(codec: Future<global::Doroti.Flutter.Ui.Codec>.value(codec__4201), scale: key.scale, debugLabel: key.url, informationCollector: _imageStreamInformationCollector(key));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        async Future<ImageStreamCompleter> loadViaImgElement()
        {
            HTMLImageElement imageElement__4668 = _network_image_webLibrary.imgElementFactory();
            imageElement__4668.src = key.url;
            await imageElement__4668.decode().toDart;
            return ((Func<OneFrameImageStreamCompleter>)(() =>
{            var __cascade = new OneFrameImageStreamCompleter(Future<ImageInfo>.value(new WebImageInfoIo(imageElement__4668, debugLabel: key.url)), informationCollector: _imageStreamInformationCollector(key));
            __cascade.debugLabel = key.url;
            return __cascade;        }))();
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool containsNetworkImageHeaders__5148 = ((((long?)(key.headers?.Count)) is { } __count5178 ? __count5178 != 0 : (bool?)null) ?? false);
        if (containsNetworkImageHeaders__5148)
        {
            return await loadViaDecode();
        }
        switch (this.webHtmlElementStrategy)
        {
            case global::Doroti.Generated.Framework.Painting.Image_providerLibrary.WebHtmlElementStrategy.never:
                {
                    return await loadViaDecode();
                }
            case global::Doroti.Generated.Framework.Painting.Image_providerLibrary.WebHtmlElementStrategy.prefer:
                {
                    return await loadViaImgElement();
                }
            case global::Doroti.Generated.Framework.Painting.Image_providerLibrary.WebHtmlElementStrategy.fallback:
                {
                    try
                    {
                        return await loadViaDecode();
                    }
                    catch (Exception e__6238)
                    {
                        return await loadViaImgElement();
                    }
                    break;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future<global::Doroti.Flutter.Ui.Codec> _fetchImageBytes(Func<ImmutableBuffer, Future<Codec>> decode)
    {
        DartUri resolved__6391 = DartUri.@base.resolve(this.url);
        bool containsNetworkImageHeaders__6441 = ((((long?)(this.headers?.Count)) is { } __count6471 ? __count6471 != 0 : (bool?)null) ?? false);
        var completer__6512 = new Completer<XMLHttpRequest>();
        XMLHttpRequest request__6586 = _network_image_webLibrary.httpRequestFactory();
        request__6586.open("GET", this.url, true);
        request__6586.responseType = "arraybuffer";
        if (containsNetworkImageHeaders__6441)
        {
            this.headers!.forEach(((header, value) => {
request__6586.setRequestHeader(header, value);
}));
        }
        request__6586.addEventListener("load", ((e) => {
long status__6941 = request__6586.status;
bool accepted__6985 = ((status__6941 >= 200L) && (status__6941 < 300L));
var fileUri__7041 = (status__6941 == 0L);
var notModified__7112 = (status__6941 == 304L);
bool unknownRedirect__7160 = ((status__6941 > 307L) && (status__6941 < 400L));
bool success__7227 = (((accepted__6985 || fileUri__7041) || notModified__7112) || unknownRedirect__7160);
if (success__7227)
{
    completer__6512.complete(request__6586);
}
else
{
    completer__6512.completeError(new NetworkImageLoadException(statusCode: status__6941, uri: resolved__6391));
}
}).toJS);
        request__6586.addEventListener("error", (((e) => completer__6512.completeError(new NetworkImageLoadException(statusCode: request__6586.status, uri: resolved__6391)))).toJS);
        request__6586.send();
        await completer__6512.future;
        Uint8List bytes__7820 = (((JSArrayBuffer?)(object?)request__6586.response!)!).toDart.asUint8List();
        if ((bytes__7820.lengthInBytes == 0L))
        {
            throw new NetworkImageLoadException(statusCode: request__6586.status, uri: resolved__6391);
        }
        return await decode(await Dart_uiLibrary.ImmutableBuffer.fromUint8List(bytes__7820));
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
        return (((((__other is NetworkImageIo) && (((NetworkImageIo)__other).url == this.url)) && (((NetworkImageIo)__other).scale == this.scale)) && (object.Equals(((NetworkImageIo)__other).webHtmlElementStrategy, this.webHtmlElementStrategy))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mapEquals(((NetworkImageIo)__other).headers, this.headers));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.url, this.scale, DartRuntimePrimitives.RequireValue(this.webHtmlElementStrategy), new MapEquality<string, string>().hash(this.headers));
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "NetworkImage"))}(\"{this.url}\", scale: {this.scale.toStringAsFixed(1L)}, webHtmlElementStrategy: {webHtmlElementStrategy.ToString()}, headers: {this.headers})";
    ImageStreamCompleter NetworkImage.loadBuffer(NetworkImage key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode) =>
        loadBuffer((NetworkImageIo)key, decode);
    ImageStreamCompleter NetworkImage.loadImage(NetworkImage key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode) =>
        loadImage((NetworkImageIo)key, decode);
}

internal class _ForwardingImageStreamCompleter___network_image_web : ImageStreamCompleter
{
    public virtual Future<ImageStreamCompleter> task { get; private set; } = default!;
    public virtual bool resolved { get; set; } = false;
    public virtual ImageStreamCompleter completer { get; private set; } = default!;
    internal virtual bool _disposed { get; set; } = false;
    private bool __late_listener_initialized;
    private ImageStreamListener __late_listener = default!;
    public virtual ImageStreamListener listener
    {
        get
        {
            if (!__late_listener_initialized)
            {
                __late_listener = new ImageStreamListener(((image, synchronousCall) => {
setImage(image);
}), onChunk: ((@event) => {
reportImageChunkEvent(@event);
}), onError: ((exception, stackTrace) => {
reportError(exception: exception, stack: stackTrace);
}));
                __late_listener_initialized = true;
            }
            return __late_listener;
        }
    }

    internal _ForwardingImageStreamCompleter___network_image_web(Future<ImageStreamCompleter> task, InformationCollector? informationCollector = null, string? debugLabel = null)
    {
        this.task = task;
    }

    public override void onDisposed()
    {
        if (this.resolved)
        {
            this.completer.removeListener(this.listener);
        }
        _disposed = true;
        base.onDisposed();
    }

}

