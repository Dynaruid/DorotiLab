// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/_network_image_web.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Painting;

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
        InformationCollector? collector = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                collector = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<dynamic>("Image provider", this), new DiagnosticsProperty<NetworkImageIo>("Image key", ((NetworkImageIo?)(object?)key)!) });
                return true;
            });
        return collector;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future<ImageStreamCompleter> _loadAsync(NetworkImageIo key, Func<ImmutableBuffer, Future<Codec>> decode)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(key, this)));
        async Future<ImageStreamCompleter> loadViaDecode()
        {
            global::Doroti.Ui.Codec codecLocal = await _fetchImageBytes((Func<ImmutableBuffer, Future<Codec>>)decode);
            return new MultiFrameImageStreamCompleter(codec: Future<global::Doroti.Ui.Codec>.value(codecLocal), scale: key.scale, debugLabel: key.url, informationCollector: _imageStreamInformationCollector(key));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        async Future<ImageStreamCompleter> loadViaImgElement()
        {
            HTMLImageElement imageElement = _network_image_webLibrary.imgElementFactory();
            imageElement.src = key.url;
            await imageElement.decode().toDart;
            return ((Func<OneFrameImageStreamCompleter>)(() =>
{            var __cascade = new OneFrameImageStreamCompleter(Future<ImageInfo>.value(new WebImageInfoIo(imageElement, debugLabel: key.url)), informationCollector: _imageStreamInformationCollector(key));
            __cascade.debugLabel = key.url;
            return __cascade;        }))();
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool containsNetworkImageHeaders = ((((long?)(key.headers?.Count)) is { } __count5178 ? __count5178 != 0 : (bool?)null) ?? false);
        if (containsNetworkImageHeaders)
        {
            return await loadViaDecode();
        }
        switch (this.webHtmlElementStrategy)
        {
            case global::Doroti.Framework.Painting.Image_providerLibrary.WebHtmlElementStrategy.never:
                {
                    return await loadViaDecode();
                }
            case global::Doroti.Framework.Painting.Image_providerLibrary.WebHtmlElementStrategy.prefer:
                {
                    return await loadViaImgElement();
                }
            case global::Doroti.Framework.Painting.Image_providerLibrary.WebHtmlElementStrategy.fallback:
                {
                    try
                    {
                        return await loadViaDecode();
                    }
                    catch (Exception e)
                    {
                        return await loadViaImgElement();
                    }
                    break;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future<global::Doroti.Ui.Codec> _fetchImageBytes(Func<ImmutableBuffer, Future<Codec>> decode)
    {
        DartUri resolved = DartUri.@base.resolve(this.url);
        bool containsNetworkImageHeaders = ((((long?)(this.headers?.Count)) is { } __count6471 ? __count6471 != 0 : (bool?)null) ?? false);
        var completer = new Completer<XMLHttpRequest>();
        XMLHttpRequest request = _network_image_webLibrary.httpRequestFactory();
        request.open("GET", this.url, true);
        request.responseType = "arraybuffer";
        if (containsNetworkImageHeaders)
        {
            this.headers!.forEach(((header, value) => {
request.setRequestHeader(header, value);
}));
        }
        request.addEventListener("load", ((e) => {
long statusLocal = request.status;
bool accepted = ((statusLocal >= 200L) && (statusLocal < 300L));
var fileUri = (statusLocal == 0L);
var notModified = (statusLocal == 304L);
bool unknownRedirect = ((statusLocal > 307L) && (statusLocal < 400L));
bool success = (((accepted || fileUri) || notModified) || unknownRedirect);
if (success)
{
    completer.complete(request);
}
else
{
    completer.completeError(new NetworkImageLoadException(statusCode: statusLocal, uri: resolved));
}
}).toJS);
        request.addEventListener("error", (((e) => completer.completeError(new NetworkImageLoadException(statusCode: request.status, uri: resolved)))).toJS);
        request.send();
        await completer.future;
        Uint8List bytes = (((JSArrayBuffer?)(object?)request.response!)!).toDart.asUint8List();
        if ((bytes.lengthInBytes == 0L))
        {
            throw new NetworkImageLoadException(statusCode: request.status, uri: resolved);
        }
        return await decode(await Dart_uiLibrary.ImmutableBuffer.fromUint8List(bytes));
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
        return (((((__other is NetworkImageIo) && (((NetworkImageIo)__other).url == this.url)) && (((NetworkImageIo)__other).scale == this.scale)) && (object.Equals(((NetworkImageIo)__other).webHtmlElementStrategy, this.webHtmlElementStrategy))) && global::Doroti.Framework.Foundation.CollectionsLibrary.mapEquals(((NetworkImageIo)__other).headers, this.headers));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.url, this.scale, DartRuntimePrimitives.RequireValue(this.webHtmlElementStrategy), new MapEquality<string, string>().hash(this.headers));
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "NetworkImage"))}(\"{this.url}\", scale: {this.scale.toStringAsFixed(1L)}, webHtmlElementStrategy: {webHtmlElementStrategy.ToString()}, headers: {this.headers})";
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

