// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/image_provider.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

internal delegate void _KeyAndErrorHandlerCallback__image_provider<T>(
    T key,
    Action<object, System.Diagnostics.StackTrace?> handleError
);

internal delegate Future _AsyncKeyErrorHandler__image_provider<T>(
    T key,
    object exception,
    System.Diagnostics.StackTrace? stack
);

public class ImageConfiguration
{
    public virtual AssetBundle? bundle { get; private set; }
    public virtual double? devicePixelRatio { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual Size? size { get; private set; }
    public virtual TargetPlatform? platform { get; private set; }
    public static ImageConfiguration empty = new ImageConfiguration();

    public ImageConfiguration(
        AssetBundle? bundle = null,
        double? devicePixelRatio = null,
        Locale? locale = null,
        TextDirection? textDirection = null,
        Size? size = null,
        TargetPlatform? platform = null
    )
    {
        this.bundle = bundle;
        this.devicePixelRatio = devicePixelRatio;
        this.locale = locale;
        this.textDirection = textDirection;
        this.size = size;
        this.platform = platform;
    }

    public virtual ImageConfiguration copyWith(
        AssetBundle? bundle = null,
        double? devicePixelRatio = null,
        Locale? locale = null,
        TextDirection? textDirection = null,
        Size? size = null,
        TargetPlatform? platform = null
    )
    {
        return new ImageConfiguration(
            bundle: bundle ?? this.bundle,
            devicePixelRatio: devicePixelRatio ?? this.devicePixelRatio,
            locale: locale ?? this.locale,
            textDirection: textDirection ?? this.textDirection,
            size: size ?? this.size,
            platform: platform ?? this.platform
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ImageConfiguration;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ImageConfiguration)
            && Equals(__other.bundle, bundle)
            && (__other.devicePixelRatio == devicePixelRatio)
            && Equals(__other.locale, locale)
            && Equals(__other.textDirection, textDirection)
            && Equals(__other.size, size)
            && Equals(__other.platform, platform);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(bundle, devicePixelRatio, locale, size, platform);

    public override string ToString()
    {
        var result = new System.Text.StringBuilder();
        result.Append("ImageConfiguration(");
        var hasArguments = false;
        if (bundle is not null)
        {
            result.Append($"bundle: {bundle}");
            hasArguments = true;
        }
        if (devicePixelRatio is not null)
        {
            double devicePixelRatio__value4301 = (
                devicePixelRatio
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            if (hasArguments)
            {
                result.Append(", ");
            }
            result.Append(
                $"devicePixelRatio: {(devicePixelRatio ?? throw new global::System.NullReferenceException("A required value was null.")).toStringAsFixed(1L)}"
            );
            hasArguments = true;
        }
        if (locale is not null)
        {
            Locale locale__value4513 = (
                locale
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            if (hasArguments)
            {
                result.Append(", ");
            }
            result.Append($"locale: {locale}");
            hasArguments = true;
        }
        if (textDirection is not null)
        {
            TextDirection textDirection__value4673 = (
                textDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            if (hasArguments)
            {
                result.Append(", ");
            }
            result.Append($"textDirection: {textDirection}");
            hasArguments = true;
        }
        if (size is not null)
        {
            Size size__value4854 = (
                size
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            if (hasArguments)
            {
                result.Append(", ");
            }
            result.Append($"size: {size}");
            hasArguments = true;
        }
        if (platform is not null)
        {
            if (hasArguments)
            {
                result.Append(", ");
            }
            result.Append($"platform: {platform!.ToString()}");
            hasArguments = true;
        }
        result.Append(")");
        return result.ToString();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public delegate Future<Codec> DecoderBufferCallback(
    ImmutableBuffer buffer,
    bool allowUpscaling = default!,
    long? cacheHeight = null,
    long? cacheWidth = null
);

internal delegate Future<Codec> _SimpleDecoderCallback__image_provider(ImmutableBuffer buffer);

public delegate Future<Codec> ImageDecoderCallback(
    ImmutableBuffer buffer,
    Func<long, long, TargetImageSize>? getTargetSize = null
);

/// <summary>Key-independent image operations, including providers defined by consumer apps.</summary>
public interface IImageProvider
{
    ImageStream resolve(ImageConfiguration configuration);
    ImageStream createStream(ImageConfiguration configuration);
    Future<ImageCacheStatus?> obtainCacheStatus(
        ImageConfiguration configuration,
        Action<object, System.Diagnostics.StackTrace?>? handleError = null
    );
    Future<bool> evict(ImageCache? cache = null, ImageConfiguration configuration = default!);
    Future<object> obtainKeyObject(ImageConfiguration configuration);
    void resolveStreamForKeyObject(
        ImageConfiguration configuration,
        ImageStream stream,
        object key,
        Action<object, System.Diagnostics.StackTrace?> handleError
    );
    ImageStreamCompleter loadBufferObject(
        object key,
        Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode
    );
    ImageStreamCompleter loadImageObject(
        object key,
        Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode
    );
}

public abstract class ImageProvider<T> : IImageProvider
    where T : notnull
{
    protected ImageProvider() { }

    Future<object> IImageProvider.obtainKeyObject(ImageConfiguration configuration) =>
        obtainKey(configuration).then<object>(key => key!);

    void IImageProvider.resolveStreamForKeyObject(
        ImageConfiguration configuration,
        ImageStream stream,
        object key,
        Action<object, System.Diagnostics.StackTrace?> handleError
    ) => resolveStreamForKey(configuration, stream, (T)key, handleError);

    ImageStreamCompleter IImageProvider.loadBufferObject(
        object key,
        Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode
    ) => loadBuffer((T)key, decode);

    ImageStreamCompleter IImageProvider.loadImageObject(
        object key,
        Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode
    ) => loadImage((T)key, decode);

    public virtual ImageStream resolve(ImageConfiguration configuration)
    {
        ImageStream stream = createStream(configuration);
        _createErrorHandlerAndKey(
            configuration,
            (key, errorHandler) =>
            {
                resolveStreamForKey(configuration, stream, key, errorHandler);
            },
            async (key, exception, stack) =>
            {
                await Task.Yield();
                InformationCollector? collector = default!;
                DartRuntimePrimitives.Assert(() =>
                {
                    collector = () =>
                        new List<DiagnosticsNode>
                        {
                            new DiagnosticsProperty<dynamic>("Image provider", this),
                            new DiagnosticsProperty<ImageConfiguration>(
                                "Image configuration",
                                configuration
                            ),
                            new DiagnosticsProperty<T>("Image key", key, defaultValue: null),
                        };
                    return true;
                });
                if (stream.completer is null)
                {
                    stream.setCompleter(new _ErrorImageCompleter__image_provider());
                }
                stream.completer!.reportError(
                    exception: exception,
                    stack: stack,
                    context: new ErrorDescription("while resolving an image"),
                    silent: true,
                    informationCollector: (InformationCollector?)collector
                );
            }
        );
        return stream;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual ImageStream createStream(ImageConfiguration configuration)
    {
        return new ImageStream();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Future<ImageCacheStatus?> obtainCacheStatus(
        ImageConfiguration configuration,
        Action<object, System.Diagnostics.StackTrace?>? handleError = null
    )
    {
        var completer = new Completer<ImageCacheStatus?>();
        _createErrorHandlerAndKey(
            configuration,
            (key, innerHandleError) =>
            {
                completer.complete(PaintingBinding.instance.imageCache.statusForKey(key));
            },
            async (key, exception, stack) =>
            {
                if (handleError is not null)
                {
                    handleError(exception, stack);
                }
                else
                {
                    InformationCollector? collector = default!;
                    DartRuntimePrimitives.Assert(() =>
                    {
                        collector = () =>
                            new List<DiagnosticsNode>
                            {
                                new DiagnosticsProperty<dynamic>("Image provider", this),
                                new DiagnosticsProperty<ImageConfiguration>(
                                    "Image configuration",
                                    configuration
                                ),
                                new DiagnosticsProperty<T>("Image key", key, defaultValue: null),
                            };
                        return true;
                    });
                    FlutterError.reportError(
                        new FlutterErrorDetails(
                            context: new ErrorDescription(
                                "while checking the cache location of an image"
                            ),
                            informationCollector: collector,
                            exception: exception,
                            stack: stack
                        )
                    );
                    completer.complete();
                }
            }
        );
        return completer.future;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _createErrorHandlerAndKey(
        ImageConfiguration configuration,
        Action<T, Action<object, System.Diagnostics.StackTrace?>> successCallback,
        Func<T?, object, System.Diagnostics.StackTrace?, Future> errorCallback
    )
    {
        T? obtainedKey = default!;
        var didError = false;
        async Future handleError(object exception, System.Diagnostics.StackTrace? stack)
        {
            if (didError)
            {
                return;
            }
            if (!didError)
            {
                didError = true;
                await errorCallback(obtainedKey, exception, stack);
            }
        }
        Future<T> keyLocal = default!;
        try
        {
            _ = keyLocal = obtainKey(configuration);
        }
        catch (Exception error)
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            _ = handleError(error, stackTrace);
            return;
        }
        _ = keyLocal
            .then(
                (key) =>
                {
                    obtainedKey = key;
                    try
                    {
                        successCallback(
                            key,
                            (__exception, __stack) =>
                            {
                                _ = handleError(__exception, __stack);
                            }
                        );
                    }
                    catch (Exception errorLocal)
                    {
                        var stackTraceLocal = new System.Diagnostics.StackTrace();
                        _ = handleError(errorLocal, stackTraceLocal);
                    }
                }
            )
            .catchError(handleError);
    }

    public virtual void resolveStreamForKey(
        ImageConfiguration configuration,
        ImageStream stream,
        T key,
        Action<object, System.Diagnostics.StackTrace?> handleError
    )
    {
        if (stream.completer is not null)
        {
            ImageStreamCompleter? completerLocal = PaintingBinding.instance.imageCache.putIfAbsent(
                key,
                () => stream.completer!,
                onError: handleError
            );
            DartRuntimePrimitives.Assert(() =>
                DartRuntimePrimitives.Identical(completerLocal, stream.completer)
            );
            return;
        }
        ImageStreamCompleter? completerAlternate = PaintingBinding.instance.imageCache.putIfAbsent(
            key,
            () =>
            {
                ImageStreamCompleter result = loadImage(
                    key,
                    (Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>>)
                        PaintingBinding.instance.instantiateImageCodecWithSize
                );
                if (result is _AbstractImageStreamCompleter__image_provider)
                {
                    _AbstractImageStreamCompleter__image_provider result__20077__as20577 =
                        (_AbstractImageStreamCompleter__image_provider)result;
                    result = loadBuffer(
                        key,
                        (Func<ImmutableBuffer, bool, long?, long?, Future<Codec>>)(
                            (__buffer, __allowUpscaling, __cacheHeight, __cacheWidth) =>
                                PaintingBinding.instance.instantiateImageCodecFromBuffer(
                                    __buffer,
                                    __cacheWidth,
                                    __cacheHeight,
                                    __allowUpscaling
                                )
                        )
                    );
                }
                return result;
            },
            onError: handleError
        );
        if (completerAlternate is not null)
        {
            stream.setCompleter(completerAlternate);
        }
    }

    public virtual async Future<bool> evict(
        ImageCache? cache = null,
        ImageConfiguration configuration = default!
    )
    {
        cache ??= BindingLibrary.imageCache;
        T key = await obtainKey(configuration);
        return cache.evict(key);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public abstract Future<T> obtainKey(ImageConfiguration configuration);

    public virtual ImageStreamCompleter loadBuffer(
        T key,
        Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode
    )
    {
        return new _AbstractImageStreamCompleter__image_provider();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual ImageStreamCompleter loadBuffer(T key, DecoderBufferCallback decode)
    {
        ArgumentNullException.ThrowIfNull(decode);
        return loadBuffer(
            key,
            (Func<ImmutableBuffer, bool, long?, long?, Future<Codec>>)(
                (buffer, allowUpscaling, cacheHeight, cacheWidth) =>
                    decode(buffer, allowUpscaling, cacheHeight, cacheWidth)
            )
        );
    }

    public virtual ImageStreamCompleter loadImage(
        T key,
        Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode
    )
    {
        return new _AbstractImageStreamCompleter__image_provider();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual ImageStreamCompleter loadImage(T key, ImageDecoderCallback decode)
    {
        ArgumentNullException.ThrowIfNull(decode);
        return loadImage(
            key,
            (Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>>)(
                (buffer, getTargetSize) => decode(buffer, getTargetSize)
            )
        );
    }

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "ImageConfiguration")}()";
}

internal class _AbstractImageStreamCompleter__image_provider : ImageStreamCompleter { }

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
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is AssetBundleImageKey)
            && Equals(__other.bundle, bundle)
            && (__other.name == name)
            && (__other.scale == scale);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(bundle, name, scale);

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "AssetBundleImageKey")}(bundle: {bundle}, name: \"{name}\", scale: {scale})";
}

public abstract class AssetBundleImageProvider : ImageProvider<AssetBundleImageKey>
{
    protected AssetBundleImageProvider() { }

    public override ImageStreamCompleter loadImage(
        AssetBundleImageKey key,
        Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode
    )
    {
        InformationCollector? collector = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            collector = () =>
                new List<DiagnosticsNode>
                {
                    new DiagnosticsProperty<dynamic>("Image provider", this),
                    new DiagnosticsProperty<AssetBundleImageKey>("Image key", key),
                };
            return true;
        });
        return new MultiFrameImageStreamCompleter(
            codec: _loadAsync(key, decode: (ImmutableBuffer __buffer) => decode(__buffer, null)),
            scale: key.scale,
            debugLabel: key.name,
            informationCollector: collector
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ImageStreamCompleter loadBuffer(
        AssetBundleImageKey key,
        Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode
    )
    {
        InformationCollector? collector = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            collector = () =>
                new List<DiagnosticsNode>
                {
                    new DiagnosticsProperty<dynamic>("Image provider", this),
                    new DiagnosticsProperty<AssetBundleImageKey>("Image key", key),
                };
            return true;
        });
        return new MultiFrameImageStreamCompleter(
            codec: _loadAsync(
                key,
                decode: (ImmutableBuffer __buffer) => decode(__buffer, false, null, null)
            ),
            scale: key.scale,
            debugLabel: key.name,
            informationCollector: collector
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual async Future<Codec> _loadAsync(
        AssetBundleImageKey key,
        Func<ImmutableBuffer, Future<Codec>> decode
    )
    {
        ImmutableBuffer buffer = default!;
        try
        {
            buffer = await key.bundle.loadBuffer(key.name);
        }
        catch (FlutterError)
        {
            PaintingBinding.instance.imageCache.evict(key);
            throw;
        }
        return await decode(buffer);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class ResizeImageKey
{
    internal virtual object _providerCacheKey { get; private set; } = default!;
    internal virtual ResizeImagePolicy _policy { get; private set; } = default!;
    internal virtual long? _width { get; private set; }
    internal virtual long? _height { get; private set; }
    internal virtual bool _allowUpscaling { get; private set; } = default!;

    public ResizeImageKey(
        object _providerCacheKey,
        ResizeImagePolicy _policy,
        long? _width,
        long? _height,
        bool _allowUpscaling
    )
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
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ResizeImageKey)
            && Equals(__other._providerCacheKey, _providerCacheKey)
            && Equals(__other._policy, _policy)
            && (__other._width == _width)
            && (__other._height == _height)
            && (__other._allowUpscaling == _allowUpscaling);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            _providerCacheKey,
            _policy,
            _width,
            _height,
            _allowUpscaling
        );
}

public enum ResizeImagePolicy
{
    exact,
    fit,
}

public class ResizeImage : ImageProvider<ResizeImageKey>
{
    public virtual IImageProvider imageProvider { get; private set; } = default!;
    public virtual long? width { get; private set; }
    public virtual long? height { get; private set; }
    public virtual ResizeImagePolicy policy { get; private set; } = default!;
    public virtual bool allowUpscaling { get; private set; } = default!;

    public ResizeImage(
        IImageProvider imageProvider,
        long? width = null,
        long? height = null,
        ResizeImagePolicy policy = ResizeImagePolicy.exact,
        bool allowUpscaling = false
    )
    {
        this.imageProvider = imageProvider;
        this.width = width;
        this.height = height;
        this.policy = policy;
        this.allowUpscaling = allowUpscaling;
        System.Diagnostics.Debug.Assert((width is not null) || (height is not null));
    }

    public static IImageProvider resizeIfNeeded(
        long? cacheWidth,
        long? cacheHeight,
        IImageProvider provider
    )
    {
        if ((cacheWidth is not null) || (cacheHeight is not null))
        {
            return new ResizeImage(provider, width: cacheWidth, height: cacheHeight);
        }
        return provider;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ImageStreamCompleter loadBuffer(
        ResizeImageKey key,
        Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode
    )
    {
        Future<Codec> decodeResize(
            ImmutableBuffer buffer,
            long? cacheWidth = null,
            long? cacheHeight = null,
            bool? allowUpscaling = null
        )
        {
            DartRuntimePrimitives.Assert(() =>
                (cacheWidth is null) && (cacheHeight is null) && (allowUpscaling is not true)
            );
            return decode(buffer, this.allowUpscaling, height, width);
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        ImageStreamCompleter completer = imageProvider.loadBufferObject(
            key._providerCacheKey,
            (__buffer, __allowUpscaling, __cacheHeight, __cacheWidth) =>
                decodeResize(__buffer, __cacheWidth, __cacheHeight, __allowUpscaling)
        );
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            completer.debugLabel = $"{completer.debugLabel} - Resized({key._width}×{key._height})";
        }
        _configureErrorListener(completer, key);
        return completer;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ImageStreamCompleter loadImage(
        ResizeImageKey key,
        Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode
    )
    {
        Future<Codec> decodeResize(
            ImmutableBuffer buffer,
            Func<long, long, TargetImageSize>? getTargetSize = null
        )
        {
            DartRuntimePrimitives.Assert(() => getTargetSize is null);
            return decode(
                buffer,
                (intrinsicWidth, intrinsicHeight) =>
                {
                    switch (policy)
                    {
                        case ResizeImagePolicy.exact:
                        {
                            long? targetWidth = width;
                            long? targetHeight = height;
                            if (!allowUpscaling)
                            {
                                if (
                                    (targetWidth is not null)
                                    && (
                                        (
                                            targetWidth
                                            ?? throw new global::System.NullReferenceException(
                                                "A required value was null."
                                            )
                                        ) > intrinsicWidth
                                    )
                                )
                                {
                                    long targetWidth__43553__value43674 = (
                                        targetWidth
                                        ?? throw new global::System.NullReferenceException(
                                            "A required value was null."
                                        )
                                    );
                                    targetWidth = intrinsicWidth;
                                }
                                if (
                                    (targetHeight is not null)
                                    && (
                                        (
                                            targetHeight
                                            ?? throw new global::System.NullReferenceException(
                                                "A required value was null."
                                            )
                                        ) > intrinsicHeight
                                    )
                                )
                                {
                                    long targetHeight__43593__value43815 = (
                                        targetHeight
                                        ?? throw new global::System.NullReferenceException(
                                            "A required value was null."
                                        )
                                    );
                                    targetHeight = intrinsicHeight;
                                }
                            }
                            return new TargetImageSize(width: targetWidth, height: targetHeight);
                        }
                        case ResizeImagePolicy.fit:
                        {
                            double aspectRatio = (double)intrinsicWidth / intrinsicHeight;
                            long maxWidth = width ?? intrinsicWidth;
                            long maxHeight = height ?? intrinsicHeight;
                            var targetWidthLocal = intrinsicWidth;
                            var targetHeightLocal = intrinsicHeight;
                            if (targetWidthLocal > maxWidth)
                            {
                                targetWidthLocal = maxWidth;
                                targetHeightLocal = checked((long)(targetWidthLocal / aspectRatio));
                            }
                            if (targetHeightLocal > maxHeight)
                            {
                                targetHeightLocal = maxHeight;
                                targetWidthLocal = (targetHeightLocal * aspectRatio).floor();
                            }
                            if (allowUpscaling)
                            {
                                if (width is null)
                                {
                                    DartRuntimePrimitives.Assert(() => height is not null);
                                    targetHeightLocal = (
                                        height
                                        ?? throw new global::System.NullReferenceException(
                                            "A required value was null."
                                        )
                                    );
                                    targetWidthLocal = (targetHeightLocal * aspectRatio).floor();
                                }
                                else
                                {
                                    if (height is null)
                                    {
                                        targetWidthLocal = (
                                            width
                                            ?? throw new global::System.NullReferenceException(
                                                "A required value was null."
                                            )
                                        );
                                        targetHeightLocal = checked(
                                            (long)(targetWidthLocal / aspectRatio)
                                        );
                                    }
                                    else
                                    {
                                        long derivedMaxWidth = (maxHeight * aspectRatio).floor();
                                        long derivedMaxHeight = checked(
                                            (long)(maxWidth / aspectRatio)
                                        );
                                        targetWidthLocal = Math.Min(maxWidth, derivedMaxWidth);
                                        targetHeightLocal = Math.Min(maxHeight, derivedMaxHeight);
                                    }
                                }
                            }
                            return new TargetImageSize(
                                width: targetWidthLocal,
                                height: targetHeightLocal
                            );
                        }
                    }
                    throw new ArgumentOutOfRangeException(
                        nameof(policy),
                        policy,
                        "Unknown image resize policy."
                    );
                }
            );
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        ImageStreamCompleter completer = imageProvider.loadImageObject(
            key._providerCacheKey,
            decodeResize
        );
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            completer.debugLabel = $"{completer.debugLabel} - Resized({key._width}×{key._height})";
        }
        _configureErrorListener(completer, key);
        return completer;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _configureErrorListener(
        ImageStreamCompleter completer,
        ResizeImageKey key
    )
    {
        completer.addEphemeralErrorListener(
            (exception, stackTrace) =>
            {
                DartAsyncRuntime.scheduleMicrotask(() =>
                {
                    PaintingBinding.instance.imageCache.evict(key);
                });
            }
        );
    }

    public override Future<ResizeImageKey> obtainKey(ImageConfiguration configuration) =>
        WrapKey(imageProvider.obtainKeyObject(configuration));

    private Future<ResizeImageKey> WrapKey<T>(Future<T> pending)
    {
        // Preserve completed keys synchronously and forward asynchronous errors.
        // Avoid the result/completer race introduced by an Action continuation.
        var task = pending.asTask();
        if (task.IsCompletedSuccessfully)
        {
            return new SynchronousFuture<ResizeImageKey>(MakeKey(task.Result!));
        }

        return Future<ResizeImageKey>.fromTask(CompleteKey(task));
    }

    private ResizeImageKey MakeKey(object key) => new(key, policy, width, height, allowUpscaling);

    private async Task<ResizeImageKey> CompleteKey<T>(Task<T> pending) => MakeKey((await pending)!);

    public override bool Equals(object? other)
    {
        var __other = other as ResizeImage;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ResizeImage)
            && Equals(imageProvider, __other.imageProvider)
            && (width == __other.width)
            && (height == __other.height)
            && Equals(policy, __other.policy)
            && (allowUpscaling == __other.allowUpscaling);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(imageProvider, width, height, policy, allowUpscaling);
}

public enum WebHtmlElementStrategy
{
    never,
    fallback,
    prefer,
}

public interface NetworkImage : IImageProvider
{
    public static NetworkImage Create(
        string url,
        double scale = default!,
        DartMap<string, string>? headers = null,
        WebHtmlElementStrategy webHtmlElementStrategy = default!
    ) => new NetworkImageIo(url, scale, headers, webHtmlElementStrategy);

    public string url { get; }
    public double scale { get; }
    public DartMap<string, string>? headers { get; }
    public WebHtmlElementStrategy webHtmlElementStrategy { get; }
    public ImageStreamCompleter loadBuffer(
        NetworkImage key,
        Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode
    );
    public ImageStreamCompleter loadImage(
        NetworkImage key,
        Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode
    );
}

public class FileImage : ImageProvider<FileImage>
{
    public virtual FileInfo file { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;

    public FileImage(FileInfo file, double scale = 1.0)
    {
        this.file = file;
        this.scale = scale;
    }

    public override Future<FileImage> obtainKey(ImageConfiguration configuration)
    {
        return new SynchronousFuture<FileImage>(this);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ImageStreamCompleter loadBuffer(
        FileImage key,
        Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode
    )
    {
        return new MultiFrameImageStreamCompleter(
            codec: _loadAsync(
                key,
                decode: (ImmutableBuffer __buffer) => decode(__buffer, false, null, null)
            ),
            scale: key.scale,
            debugLabel: key.file.FullName,
            informationCollector: () =>
                new List<DiagnosticsNode> { new ErrorDescription($"Path: {file.FullName}") }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ImageStreamCompleter loadImage(
        FileImage key,
        Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode
    )
    {
        return new MultiFrameImageStreamCompleter(
            codec: _loadAsync(key, decode: (ImmutableBuffer __buffer) => decode(__buffer, null)),
            scale: key.scale,
            debugLabel: key.file.FullName,
            informationCollector: () =>
                new List<DiagnosticsNode> { new ErrorDescription($"Path: {file.FullName}") }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual async Future<Codec> _loadAsync(
        FileImage key,
        Func<ImmutableBuffer, Future<Codec>> decode
    )
    {
        DartRuntimePrimitives.Assert(() => Equals(key, this));
        long lengthInBytes = file.Length;
        if (lengthInBytes == 0L)
        {
            PaintingBinding.instance.imageCache.evict(key);
            throw new InvalidOperationException(
                $"File: '{file.FullName}' is empty and cannot be loaded as an image."
            );
        }
        return await decode(await DorotiUiLibrary.ImmutableBuffer.fromFilePath(file.FullName));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as FileImage;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is FileImage)
            && (__other.file.FullName == file.FullName)
            && (__other.scale == scale);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(file.FullName, scale);

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "FileImage")}(\"{file.FullName}\", scale: {scale.toStringAsFixed(1L)})";
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ImageStreamCompleter loadBuffer(
        MemoryImage key,
        Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode
    )
    {
        DartRuntimePrimitives.Assert(() => Equals(key, this));
        return new MultiFrameImageStreamCompleter(
            codec: _loadAsync(
                key,
                decode: (ImmutableBuffer __buffer) => decode(__buffer, false, null, null)
            ),
            scale: key.scale,
            debugLabel: $"MemoryImage({DiagnosticsLibrary.describeIdentity(key.bytes)})"
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ImageStreamCompleter loadImage(
        MemoryImage key,
        Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode
    )
    {
        return new MultiFrameImageStreamCompleter(
            codec: _loadAsync(key, decode: (ImmutableBuffer __buffer) => decode(__buffer, null)),
            scale: key.scale,
            debugLabel: $"MemoryImage({DiagnosticsLibrary.describeIdentity(key.bytes)})"
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual async Future<Codec> _loadAsync(
        MemoryImage key,
        Func<ImmutableBuffer, Future<Codec>> decode
    )
    {
        DartRuntimePrimitives.Assert(() => Equals(key, this));
        return await decode(await DorotiUiLibrary.ImmutableBuffer.fromUint8List(bytes));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as MemoryImage;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is MemoryImage) && Equals(__other.bytes, bytes) && (__other.scale == scale);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(bytes.GetHashCode(), scale);

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "MemoryImage")}({DiagnosticsLibrary.describeIdentity(bytes)}, scale: {scale.toStringAsFixed(1L)})";
}

public class ExactAssetImage : AssetBundleImageProvider
{
    public virtual string assetName { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;
    public virtual AssetBundle? bundle { get; private set; }
    public virtual string? package { get; private set; }

    public ExactAssetImage(
        string assetName,
        double scale = 1.0,
        AssetBundle? bundle = null,
        string? package = null
    )
    {
        this.assetName = assetName;
        this.scale = scale;
        this.bundle = bundle;
        this.package = package;
    }

    public virtual string keyName =>
        (package is null) ? assetName : $"packages/{package}/{assetName}";

    public override Future<AssetBundleImageKey> obtainKey(ImageConfiguration configuration)
    {
        return new SynchronousFuture<AssetBundleImageKey>(
            new AssetBundleImageKey(
                bundle: (bundle ?? configuration.bundle) ?? Asset_bundleLibrary.rootBundle,
                name: keyName,
                scale: scale
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ExactAssetImage;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ExactAssetImage)
            && (__other.keyName == keyName)
            && (__other.scale == scale)
            && Equals(__other.bundle, bundle);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(keyName, scale, bundle);

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "ExactAssetImage")}(name: \"{keyName}\", scale: {scale.toStringAsFixed(1L)}, bundle: {bundle})";
}

internal class _ErrorImageCompleter__image_provider : ImageStreamCompleter { }

public class NetworkImageLoadException : Exception
{
    public virtual long statusCode { get; private set; } = default!;
    internal virtual string _message { get; private set; } = default!;
    public virtual DartUri uri { get; private set; } = default!;

    public NetworkImageLoadException(long statusCode, DartUri uri)
    {
        this.statusCode = statusCode;
        this.uri = uri;
        _message = $"HTTP request failed, statusCode: {statusCode}, {uri}";
    }

    public override string ToString() => _message;
}
