// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/image_provider.dart
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

namespace Doroti.Generated.Framework.Painting;

internal delegate void _KeyAndErrorHandlerCallback__image_provider<T>(T key, Action<object, global::System.Diagnostics.StackTrace?> handleError);

internal delegate Future _AsyncKeyErrorHandler__image_provider<T>(T key, object exception, global::System.Diagnostics.StackTrace? stack);

public class ImageConfiguration
{
    public virtual AssetBundle? bundle { get; private set; }
    public virtual double? devicePixelRatio { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual Size? size { get; private set; }
    public virtual TargetPlatform? platform { get; private set; }
    public static ImageConfiguration empty = new ImageConfiguration();

    public ImageConfiguration(AssetBundle? bundle = null, double? devicePixelRatio = null, Locale? locale = null, TextDirection? textDirection = null, Size? size = null, TargetPlatform? platform = null)
    {
        this.bundle = bundle;
        this.devicePixelRatio = devicePixelRatio;
        this.locale = locale;
        this.textDirection = textDirection;
        this.size = size;
        this.platform = platform;
    }

    public virtual ImageConfiguration copyWith(AssetBundle? bundle = null, double? devicePixelRatio = null, Locale? locale = null, TextDirection? textDirection = null, Size? size = null, TargetPlatform? platform = null)
    {
        return new ImageConfiguration(bundle: (bundle ?? this.bundle), devicePixelRatio: (devicePixelRatio ?? this.devicePixelRatio), locale: (locale ?? this.locale), textDirection: (textDirection ?? this.textDirection), size: (size ?? this.size), platform: (platform ?? this.platform));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ImageConfiguration;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((__other is ImageConfiguration) && (object.Equals(((ImageConfiguration)((ImageConfiguration)__other)).bundle, this.bundle))) && (((ImageConfiguration)((ImageConfiguration)__other)).devicePixelRatio == this.devicePixelRatio)) && (object.Equals(((ImageConfiguration)((ImageConfiguration)__other)).locale, this.locale))) && (object.Equals(((ImageConfiguration)((ImageConfiguration)__other)).textDirection, this.textDirection))) && (object.Equals(((ImageConfiguration)((ImageConfiguration)__other)).size, this.size))) && (object.Equals(((ImageConfiguration)((ImageConfiguration)__other)).platform, this.platform)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.bundle, this.devicePixelRatio, this.locale, this.size, this.platform);
    public override string ToString()
    {
        var result__4099 = new StringBuffer();
        result__4099.write("ImageConfiguration(");
        var hasArguments__4173 = false;
        if ((this.bundle is not null))
        {
            result__4099.write($"bundle: {this.bundle}");
            hasArguments__4173 = true;
        }
        if ((this.devicePixelRatio is not null))
        {
            double devicePixelRatio__value4301 = DartRuntimePrimitives.RequireValue(devicePixelRatio);
            if (hasArguments__4173)
            {
                result__4099.write(", ");
            }
            result__4099.write($"devicePixelRatio: {DartRuntimePrimitives.RequireValue(this.devicePixelRatio).toStringAsFixed(1L)}");
            hasArguments__4173 = true;
        }
        if ((this.locale is not null))
        {
            Locale locale__value4513 = DartRuntimePrimitives.RequireValue(locale);
            if (hasArguments__4173)
            {
                result__4099.write(", ");
            }
            result__4099.write($"locale: {this.locale}");
            hasArguments__4173 = true;
        }
        if ((this.textDirection is not null))
        {
            TextDirection textDirection__value4673 = DartRuntimePrimitives.RequireValue(textDirection);
            if (hasArguments__4173)
            {
                result__4099.write(", ");
            }
            result__4099.write($"textDirection: {this.textDirection}");
            hasArguments__4173 = true;
        }
        if ((this.size is not null))
        {
            Size size__value4854 = DartRuntimePrimitives.RequireValue(size);
            if (hasArguments__4173)
            {
                result__4099.write(", ");
            }
            result__4099.write($"size: {this.size}");
            hasArguments__4173 = true;
        }
        if ((this.platform is not null))
        {
            if (hasArguments__4173)
            {
                result__4099.write(", ");
            }
            result__4099.write($"platform: {this.platform!.ToString()}");
            hasArguments__4173 = true;
        }
        result__4099.write(")");
        return result__4099.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate Future<Codec> DecoderBufferCallback(ImmutableBuffer buffer, bool allowUpscaling = default!, long? cacheHeight = null, long? cacheWidth = null);

internal delegate Future<Codec> _SimpleDecoderCallback__image_provider(ImmutableBuffer buffer);

public delegate Future<Codec> ImageDecoderCallback(ImmutableBuffer buffer, Func<long, long, TargetImageSize>? getTargetSize = null);

public abstract class ImageProvider<T>
{
    protected ImageProvider()
    {
    }

    public virtual ImageStream resolve(ImageConfiguration configuration)
    {
        ImageStream stream__13537 = createStream(configuration);
        _createErrorHandlerAndKey(configuration, ((Action<T, Action<object, global::System.Diagnostics.StackTrace?>>)((key, errorHandler) =>
        {
            resolveStreamForKey(configuration, stream__13537, key, (Action<object, global::System.Diagnostics.StackTrace?>)errorHandler);
        })), ((Func<T?, object, global::System.Diagnostics.StackTrace?, Future>)(async (key, exception, stack) =>
        {
            await Task.Yield();
            InformationCollector? collector__14061 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    collector__14061 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<dynamic>("Image provider", this), new DiagnosticsProperty<ImageConfiguration>("Image configuration", configuration), new DiagnosticsProperty<T>("Image key", key, defaultValue: null) });
                    return true;
                });
            if ((((ImageStream)stream__13537).completer is null))
            {
                stream__13537.setCompleter(new _ErrorImageCompleter__image_provider());
            }
            ((ImageStream)stream__13537).completer!.reportError(exception: exception, stack: stack, context: new ErrorDescription("while resolving an image"), silent: true, informationCollector: (InformationCollector?)collector__14061);
        })));
        return stream__13537;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ImageStream createStream(ImageConfiguration configuration)
    {
        return new ImageStream();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<ImageCacheStatus?> obtainCacheStatus(ImageConfiguration configuration, Action<object, global::System.Diagnostics.StackTrace?>? handleError = null)
    {
        var completer__15820 = new Completer<ImageCacheStatus?>();
        _createErrorHandlerAndKey(configuration, ((Action<T, Action<object, global::System.Diagnostics.StackTrace?>>)((key, innerHandleError) =>
        {
            completer__15820.complete(PaintingBinding.instance.imageCache.statusForKey(key));
        })), ((Func<T?, object, global::System.Diagnostics.StackTrace?, Future>)(async (key, exception, stack) =>
        {
            if ((handleError is not null))
            {
                handleError(exception, stack);
            }
            else
            {
                InformationCollector? collector__16246 = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        collector__16246 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<dynamic>("Image provider", this), new DiagnosticsProperty<ImageConfiguration>("Image configuration", configuration), new DiagnosticsProperty<T>("Image key", key, defaultValue: null) });
                        return true;
                    });
                FlutterError.reportError(new FlutterErrorDetails(context: new ErrorDescription("while checking the cache location of an image"), informationCollector: collector__16246, exception: exception, stack: stack));
                completer__15820.complete();
            }
        })));
        return completer__15820.future;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _createErrorHandlerAndKey(ImageConfiguration configuration, Action<T, Action<object, global::System.Diagnostics.StackTrace?>> successCallback, Func<T?, object, global::System.Diagnostics.StackTrace?, Future> errorCallback)
    {
        T? obtainedKey__17377 = default!;
        var didError__17398 = false;
        async Future handleError(object exception, global::System.Diagnostics.StackTrace? stack)
        {
            if (didError__17398)
            {
                return;
            }
            if (!didError__17398)
            {
                didError__17398 = true;
                await errorCallback(obtainedKey__17377, exception, stack);
            }
        }
        Future<T> key__17895 = default!;
        try
        {
            _ = key__17895 = obtainKey(configuration);
        }
        catch (Exception error__17961)
        {
            var stackTrace__17968 = new System.Diagnostics.StackTrace();
            _ = handleError(error__17961, stackTrace__17968);
            return;
        }
        _ = key__17895.then((Action<T>)((key) =>
        {
            obtainedKey__17377 = key;
            try
            {
                successCallback(key, (__exception, __stack) => { _ = handleError(__exception, __stack); });
            }
            catch (Exception error__18189)
            {
                var stackTrace__18196 = new System.Diagnostics.StackTrace();
                _ = handleError(error__18189, stackTrace__18196);
            }
        })).catchError(handleError);
    }

    public virtual void resolveStreamForKey(ImageConfiguration configuration, ImageStream stream, T key, Action<object, global::System.Diagnostics.StackTrace?> handleError)
    {
        if ((((ImageStream)stream).completer is not null))
        {
            ImageStreamCompleter? completer__19713 = PaintingBinding.instance.imageCache.putIfAbsent(key, ((Func<ImageStreamCompleter>)(() => ((ImageStream)stream).completer!)), onError: (Action<object, global::System.Diagnostics.StackTrace?>)handleError);
            DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(completer__19713, ((ImageStream)stream).completer));
            return;
        }
        ImageStreamCompleter? completer__19965 = PaintingBinding.instance.imageCache.putIfAbsent(key, ((Func<ImageStreamCompleter>)(() =>
        {
            ImageStreamCompleter result__20077 = loadImage(key, (Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>>)PaintingBinding.instance.instantiateImageCodecWithSize);
            if ((result__20077 is _AbstractImageStreamCompleter__image_provider))
            {
                _AbstractImageStreamCompleter__image_provider result__20077__as20577 = (_AbstractImageStreamCompleter__image_provider)result__20077;
                result__20077 = loadBuffer(
                    key,
                    (Func<ImmutableBuffer, bool, long?, long?, Future<Codec>>)
                        ((__buffer, __allowUpscaling, __cacheHeight, __cacheWidth) =>
                            PaintingBinding.instance.instantiateImageCodecFromBuffer(
                                __buffer,
                                __cacheWidth,
                                __cacheHeight,
                                __allowUpscaling)));
            }
            return result__20077;
            return default;
        })), onError: (Action<object, global::System.Diagnostics.StackTrace?>)handleError);
        if ((completer__19965 is not null))
        {
            stream.setCompleter(completer__19965);
        }
    }

    public async virtual Future<bool> evict(ImageCache? cache = null, ImageConfiguration configuration = default!)
    {
        cache ??= global::Doroti.Generated.Framework.Painting.BindingLibrary.imageCache;
        T key__22301 = await obtainKey(configuration);
        return cache.evict(key__22301);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Future<T> obtainKey(ImageConfiguration configuration);
    public virtual ImageStreamCompleter loadBuffer(T key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode)
    {
        return new _AbstractImageStreamCompleter__image_provider();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ImageStreamCompleter loadBuffer(T key, DecoderBufferCallback decode)
    {
        ArgumentNullException.ThrowIfNull(decode);
        return loadBuffer(
            key,
            (Func<ImmutableBuffer, bool, long?, long?, Future<Codec>>)
                ((buffer, allowUpscaling, cacheHeight, cacheWidth) =>
                    decode(buffer, allowUpscaling, cacheHeight, cacheWidth)));
    }

    public virtual ImageStreamCompleter loadImage(T key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode)
    {
        return new _AbstractImageStreamCompleter__image_provider();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ImageStreamCompleter loadImage(T key, ImageDecoderCallback decode)
    {
        ArgumentNullException.ThrowIfNull(decode);
        return loadImage(
            key,
            (Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>>)
                ((buffer, getTargetSize) => decode(buffer, getTargetSize)));
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ImageConfiguration"))}()";
}

internal class _AbstractImageStreamCompleter__image_provider : ImageStreamCompleter
{
}

public class AssetBundleImageKey
{
    public virtual AssetBundle bundle { get; private set; } = default!;
    public virtual string name { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;

    public AssetBundleImageKey(AssetBundle bundle, string name, double scale)
    {
        this.bundle = bundle;
        this.name = name;
        this.scale = scale;
    }

    public override bool Equals(object? other)
    {
        var __other = other as AssetBundleImageKey;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is AssetBundleImageKey) && (object.Equals(((AssetBundleImageKey)((AssetBundleImageKey)__other)).bundle, this.bundle))) && (((AssetBundleImageKey)((AssetBundleImageKey)__other)).name == this.name)) && (((AssetBundleImageKey)((AssetBundleImageKey)__other)).scale == this.scale));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.bundle, this.name, this.scale);
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "AssetBundleImageKey"))}(bundle: {this.bundle}, name: \"{this.name}\", scale: {this.scale})";
}

public abstract class AssetBundleImageProvider : ImageProvider<AssetBundleImageKey>
{
    protected AssetBundleImageProvider()
    {
    }

    public override ImageStreamCompleter loadImage(AssetBundleImageKey key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode)
    {
        InformationCollector? collector__26836 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                collector__26836 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<dynamic>("Image provider", this), new DiagnosticsProperty<AssetBundleImageKey>("Image key", key) });
                return true;
            });
        return new MultiFrameImageStreamCompleter(codec: _loadAsync(key, decode: (ImmutableBuffer __buffer) => decode(__buffer, null)), scale: ((AssetBundleImageKey)key).scale, debugLabel: ((AssetBundleImageKey)key).name, informationCollector: collector__26836);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ImageStreamCompleter loadBuffer(AssetBundleImageKey key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode)
    {
        InformationCollector? collector__27491 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                collector__27491 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<dynamic>("Image provider", this), new DiagnosticsProperty<AssetBundleImageKey>("Image key", key) });
                return true;
            });
        return new MultiFrameImageStreamCompleter(codec: _loadAsync(key, decode: (ImmutableBuffer __buffer) => decode(__buffer, false, null, null)), scale: ((AssetBundleImageKey)key).scale, debugLabel: ((AssetBundleImageKey)key).name, informationCollector: collector__27491);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future<global::Doroti.Ui.Codec> _loadAsync(AssetBundleImageKey key, Func<ImmutableBuffer, Future<Codec>> decode)
    {
        global::Doroti.Ui.ImmutableBuffer buffer__28251 = default!;
        try
        {
            buffer__28251 = await ((AssetBundleImageKey)key).bundle.loadBuffer(((AssetBundleImageKey)key).name);
        }
        catch (FlutterError)
        {
            PaintingBinding.instance.imageCache.evict(key);
            throw;
        }
        return await decode(buffer__28251);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ResizeImageKey
{
    internal virtual object _providerCacheKey { get; private set; } = default!;
    internal virtual ResizeImagePolicy _policy { get; private set; } = default!;
    internal virtual long? _width { get; private set; }
    internal virtual long? _height { get; private set; }
    internal virtual bool _allowUpscaling { get; private set; } = default!;

    public ResizeImageKey(object _providerCacheKey, ResizeImagePolicy _policy, long? _width, long? _height, bool _allowUpscaling)
    {
        this._providerCacheKey = _providerCacheKey;
        this._policy = _policy;
        this._width = _width;
        this._height = _height;
        this._allowUpscaling = _allowUpscaling;
    }

    public override bool Equals(object? other)
    {
        var __other = other as ResizeImageKey;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((__other is ResizeImageKey) && (object.Equals(((ResizeImageKey)((ResizeImageKey)__other))._providerCacheKey, this._providerCacheKey))) && (object.Equals(((ResizeImageKey)((ResizeImageKey)__other))._policy, this._policy))) && (((ResizeImageKey)((ResizeImageKey)__other))._width == this._width)) && (((ResizeImageKey)((ResizeImageKey)__other))._height == this._height)) && (((ResizeImageKey)((ResizeImageKey)__other))._allowUpscaling == this._allowUpscaling));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this._providerCacheKey, this._policy, this._width, this._height, this._allowUpscaling);
}

public enum ResizeImagePolicy
{
    exact,
    fit
}

public class ResizeImage : ImageProvider<ResizeImageKey>
{
    public virtual dynamic imageProvider { get; private set; } = default!;
    public virtual long? width { get; private set; }
    public virtual long? height { get; private set; }
    public virtual ResizeImagePolicy policy { get; private set; } = default!;
    public virtual bool allowUpscaling { get; private set; } = default!;

    public ResizeImage(dynamic imageProvider, long? width = null, long? height = null, ResizeImagePolicy policy = ResizeImagePolicy.exact, bool allowUpscaling = false)
    {
        this.imageProvider = imageProvider;
        this.width = width;
        this.height = height;
        this.policy = policy;
        this.allowUpscaling = allowUpscaling;
        System.Diagnostics.Debug.Assert(((width is not null) || (height is not null)));
    }

    public static dynamic resizeIfNeeded(long? cacheWidth, long? cacheHeight, dynamic provider)
    {
        if (((cacheWidth is not null) || (cacheHeight is not null)))
        {
            return new ResizeImage(provider, width: cacheWidth, height: cacheHeight);
        }
        return provider;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ImageStreamCompleter loadBuffer(ResizeImageKey key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode)
    {
        Future<Codec> decodeResize(ImmutableBuffer buffer, long? cacheWidth = null, long? cacheHeight = null, bool? allowUpscaling = null)
        {
            DartRuntimePrimitives.Assert(() => (((cacheWidth is null) && (cacheHeight is null)) && (allowUpscaling is null)));
            return decode(buffer, this.allowUpscaling, this.height, this.width);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        ImageStreamCompleter completer__42681 = this.imageProvider.loadBuffer(((ResizeImageKey)key)._providerCacheKey, ((Func<ImmutableBuffer, bool, long?, long?, Future<Codec>>)((__buffer, __allowUpscaling, __cacheWidth, __cacheHeight) => decodeResize(__buffer, __cacheWidth, __cacheHeight, __allowUpscaling))));
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            completer__42681.debugLabel = $"{((ImageStreamCompleter)completer__42681).debugLabel} - Resized({((ResizeImageKey)key)._width}×{((ResizeImageKey)key)._height})";
        }
        _configureErrorListener(completer__42681, key);
        return completer__42681;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ImageStreamCompleter loadImage(ResizeImageKey key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode)
    {
        Future<Codec> decodeResize(ImmutableBuffer buffer, Func<long, long, TargetImageSize>? getTargetSize = null)
        {
            DartRuntimePrimitives.Assert(() => (getTargetSize is null));
            return decode(buffer, ((intrinsicWidth, intrinsicHeight) =>
            {
                switch (this.policy)
                {
                    case ResizeImagePolicy.exact:
                        {
                            long? targetWidth__43553 = this.width;
                            long? targetHeight__43593 = this.height;
                            if (!this.allowUpscaling)
                            {
                                if (((targetWidth__43553 is not null) && (DartRuntimePrimitives.RequireValue(targetWidth__43553) > intrinsicWidth)))
                                {
                                    long targetWidth__43553__value43674 = DartRuntimePrimitives.RequireValue(targetWidth__43553);
                                    targetWidth__43553 = intrinsicWidth;
                                }
                                if (((targetHeight__43593 is not null) && (DartRuntimePrimitives.RequireValue(targetHeight__43593) > intrinsicHeight)))
                                {
                                    long targetHeight__43593__value43815 = DartRuntimePrimitives.RequireValue(targetHeight__43593);
                                    targetHeight__43593 = intrinsicHeight;
                                }
                            }
                            return new global::Doroti.Ui.TargetImageSize(width: targetWidth__43553, height: targetHeight__43593);
                        }
                    case ResizeImagePolicy.fit:
                        {
                            double aspectRatio__44108 = (intrinsicWidth / intrinsicHeight);
                            long maxWidth__44180 = (this.width ?? intrinsicWidth);
                            long maxHeight__44240 = (this.height ?? intrinsicHeight);
                            var targetWidth__44297 = intrinsicWidth;
                            var targetHeight__44345 = intrinsicHeight;
                            if ((targetWidth__44297 > maxWidth__44180))
                            {
                                targetWidth__44297 = maxWidth__44180;
                                targetHeight__44345 = (checked((long)(targetWidth__44297 / aspectRatio__44108)));
                            }
                            if ((targetHeight__44345 > maxHeight__44240))
                            {
                                targetHeight__44345 = maxHeight__44240;
                                targetWidth__44297 = ((targetHeight__44345 * aspectRatio__44108)).floor();
                            }
                            if (this.allowUpscaling)
                            {
                                if ((this.width is null))
                                {
                                    DartRuntimePrimitives.Assert(() => (this.height is not null));
                                    targetHeight__44345 = DartRuntimePrimitives.RequireValue(this.height);
                                    targetWidth__44297 = ((targetHeight__44345 * aspectRatio__44108)).floor();
                                }
                                else
                                {
                                    if ((this.height is null))
                                    {
                                        targetWidth__44297 = DartRuntimePrimitives.RequireValue(this.width);
                                        targetHeight__44345 = (checked((long)(targetWidth__44297 / aspectRatio__44108)));
                                    }
                                    else
                                    {
                                        long derivedMaxWidth__45137 = ((maxHeight__44240 * aspectRatio__44108)).floor();
                                        long derivedMaxHeight__45218 = (checked((long)(maxWidth__44180 / aspectRatio__44108)));
                                        targetWidth__44297 = Math.Min(maxWidth__44180, derivedMaxWidth__45137);
                                        targetHeight__44345 = Math.Min(maxHeight__44240, derivedMaxHeight__45218);
                                    }
                                }
                            }
                            return new global::Doroti.Ui.TargetImageSize(width: targetWidth__44297, height: targetHeight__44345);
                        }
                }
                return default;
            }));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        ImageStreamCompleter completer__45591 = this.imageProvider.loadImage(((ResizeImageKey)key)._providerCacheKey, (Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>>)decodeResize);
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            completer__45591.debugLabel = $"{((ImageStreamCompleter)completer__45591).debugLabel} - Resized({((ResizeImageKey)key)._width}×{((ResizeImageKey)key)._height})";
        }
        _configureErrorListener(completer__45591, key);
        return completer__45591;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _configureErrorListener(ImageStreamCompleter completer, ResizeImageKey key)
    {
        completer.addEphemeralErrorListener(((Action<object, global::System.Diagnostics.StackTrace?>)((exception, stackTrace) =>
        {
            DartAsyncRuntime.scheduleMicrotask((() =>
            {
                PaintingBinding.instance.imageCache.evict(key);
            }));
        })));
    }

    public override Future<ResizeImageKey> obtainKey(ImageConfiguration configuration)
    {
        Completer<ResizeImageKey>? completer__46571 = default!;
        SynchronousFuture<ResizeImageKey>? result__46780 = default!;
        _ = this.imageProvider.obtainKey(configuration).then((Action<object>)((key) =>
        {
            if ((completer__46571 is null))
            {
                result__46780 = new SynchronousFuture<ResizeImageKey>(new ResizeImageKey(key, this.policy, this.width, this.height, this.allowUpscaling));
            }
            else
            {
                completer__46571.complete(new ResizeImageKey(key, this.policy, this.width, this.height, this.allowUpscaling));
            }
        }));
        if ((result__46780 is not null))
        {
            return result__46780!;
        }
        completer__46571 = new Completer<ResizeImageKey>();
        return completer__46571.future;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ResizeImage;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((__other is ResizeImage) && (object.Equals(this.imageProvider, ((ResizeImage)((ResizeImage)__other)).imageProvider))) && (this.width == ((ResizeImage)((ResizeImage)__other)).width)) && (this.height == ((ResizeImage)((ResizeImage)__other)).height)) && (object.Equals(this.policy, ((ResizeImage)((ResizeImage)__other)).policy))) && (this.allowUpscaling == ((ResizeImage)((ResizeImage)__other)).allowUpscaling));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.imageProvider, this.width, this.height, this.policy, this.allowUpscaling);
}

public enum WebHtmlElementStrategy
{
    never,
    fallback,
    prefer
}

public interface NetworkImage
{
    public static NetworkImage Create(string url, double scale = default!, DartMap<string, string>? headers = null, WebHtmlElementStrategy webHtmlElementStrategy = default!)
        => new NetworkImageIo(url, scale, headers, webHtmlElementStrategy);

    public string url { get; }
    public double scale { get; }
    public DartMap<string, string>? headers { get; }
    public WebHtmlElementStrategy webHtmlElementStrategy { get; }
    public ImageStreamCompleter loadBuffer(NetworkImage key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode);
    public ImageStreamCompleter loadImage(NetworkImage key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode);
}

public class FileImage : ImageProvider<FileImage>
{
    public virtual global::Doroti.Runtime.DartFile file { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;

    public FileImage(global::Doroti.Runtime.DartFile file, double scale = 1.0)
    {
        this.file = file;
        this.scale = scale;
    }

    public override Future<FileImage> obtainKey(ImageConfiguration configuration)
    {
        return new SynchronousFuture<FileImage>(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ImageStreamCompleter loadBuffer(FileImage key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode)
    {
        return new MultiFrameImageStreamCompleter(codec: _loadAsync(key, decode: (ImmutableBuffer __buffer) => decode(__buffer, false, null, null)), scale: ((FileImage)key).scale, debugLabel: ((FileImage)key).file.path, informationCollector: (() => new List<DiagnosticsNode> { new ErrorDescription($"Path: {this.file.path}") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ImageStreamCompleter loadImage(FileImage key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode)
    {
        return new MultiFrameImageStreamCompleter(codec: _loadAsync(key, decode: (ImmutableBuffer __buffer) => decode(__buffer, null)), scale: ((FileImage)key).scale, debugLabel: ((FileImage)key).file.path, informationCollector: (() => new List<DiagnosticsNode> { new ErrorDescription($"Path: {this.file.path}") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future<global::Doroti.Ui.Codec> _loadAsync(FileImage key, Func<ImmutableBuffer, Future<Codec>> decode)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(key, this)));
        long lengthInBytes__53776 = await this.file.length();
        if ((lengthInBytes__53776 == 0L))
        {
            PaintingBinding.instance.imageCache.evict(key);
            throw new InvalidOperationException($"{this.file} is empty and cannot be loaded as an image.");
        }
        return await (((object.Equals(DartRuntimePrimitives.RuntimeType(this.file), typeof(global::Doroti.Runtime.DartFile)))) ? decode(await Dart_uiLibrary.ImmutableBuffer.fromFilePath(this.file.path)) : decode(await Dart_uiLibrary.ImmutableBuffer.fromUint8List(await this.file.readAsBytes())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as FileImage;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is FileImage) && (((FileImage)((FileImage)__other)).file.path == this.file.path)) && (((FileImage)((FileImage)__other)).scale == this.scale));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.file.path, this.scale);
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FileImage"))}(\"{this.file.path}\", scale: {this.scale.toStringAsFixed(1L)})";
}

public class MemoryImage : ImageProvider<MemoryImage>
{
    public virtual Uint8List bytes { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;

    public MemoryImage(Uint8List bytes, double scale = 1.0)
    {
        this.bytes = bytes;
        this.scale = scale;
    }

    public override Future<MemoryImage> obtainKey(ImageConfiguration configuration)
    {
        return new SynchronousFuture<MemoryImage>(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ImageStreamCompleter loadBuffer(MemoryImage key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(key, this)));
        return new MultiFrameImageStreamCompleter(codec: _loadAsync(key, decode: (ImmutableBuffer __buffer) => decode(__buffer, false, null, null)), scale: ((MemoryImage)key).scale, debugLabel: $"MemoryImage({(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(((MemoryImage)key).bytes))})");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ImageStreamCompleter loadImage(MemoryImage key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode)
    {
        return new MultiFrameImageStreamCompleter(codec: _loadAsync(key, decode: (ImmutableBuffer __buffer) => decode(__buffer, null)), scale: ((MemoryImage)key).scale, debugLabel: $"MemoryImage({(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(((MemoryImage)key).bytes))})");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future<global::Doroti.Ui.Codec> _loadAsync(MemoryImage key, Func<ImmutableBuffer, Future<Codec>> decode)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(key, this)));
        return await decode(await Dart_uiLibrary.ImmutableBuffer.fromUint8List(this.bytes));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as MemoryImage;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is MemoryImage) && (object.Equals(((MemoryImage)((MemoryImage)__other)).bytes, this.bytes))) && (((MemoryImage)((MemoryImage)__other)).scale == this.scale));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.bytes.GetHashCode(), this.scale);
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MemoryImage"))}({(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this.bytes))}, scale: {this.scale.toStringAsFixed(1L)})";
}

public class ExactAssetImage : AssetBundleImageProvider
{
    public virtual string assetName { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;
    public virtual AssetBundle? bundle { get; private set; }
    public virtual string? package { get; private set; }

    public ExactAssetImage(string assetName, double scale = 1.0, AssetBundle? bundle = null, string? package = null)
    {
        this.assetName = assetName;
        this.scale = scale;
        this.bundle = bundle;
        this.package = package;
    }

    public virtual string keyName => ((this.package is null) ? this.assetName : $"packages/{this.package}/{this.assetName}");
    public override Future<AssetBundleImageKey> obtainKey(ImageConfiguration configuration)
    {
        return new SynchronousFuture<AssetBundleImageKey>(new AssetBundleImageKey(bundle: ((this.bundle ?? ((ImageConfiguration)configuration).bundle) ?? global::Doroti.Generated.Framework.Services.Asset_bundleLibrary.rootBundle), name: this.keyName, scale: this.scale));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ExactAssetImage;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is ExactAssetImage) && (((ExactAssetImage)((ExactAssetImage)__other)).keyName == this.keyName)) && (((ExactAssetImage)((ExactAssetImage)__other)).scale == this.scale)) && (object.Equals(((ExactAssetImage)((ExactAssetImage)__other)).bundle, this.bundle)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.keyName, this.scale, this.bundle);
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ExactAssetImage"))}(name: \"{this.keyName}\", scale: {this.scale.toStringAsFixed(1L)}, bundle: {this.bundle})";
}

internal class _ErrorImageCompleter__image_provider : ImageStreamCompleter
{
}

public class NetworkImageLoadException : Exception
{
    public virtual long statusCode { get; private set; } = default!;
    internal virtual string _message { get; private set; } = default!;
    public virtual DartUri uri { get; private set; } = default!;

    public NetworkImageLoadException(long statusCode, DartUri uri)
    {
        this.statusCode = statusCode;
        this.uri = uri;
        this._message = $"HTTP request failed, statusCode: {statusCode}, {uri}";
    }

    public override string ToString() => this._message;
}
