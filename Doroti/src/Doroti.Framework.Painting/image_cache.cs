// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/image_cache.dart
using Doroti.Runtime;

namespace Doroti.Framework.Painting;

public static partial class Image_cacheLibrary
{
    internal static long _kDefaultSize = 1000L;
}

public static partial class Image_cacheLibrary
{
    internal static long _kDefaultSizeBytes = 100L << (int)20L;
}

public class ImageCache
{
    internal virtual DartMap<object, _PendingImage__image_cache> _pendingImages
    {
        get;
        private set;
    } = new DartMap<object, _PendingImage__image_cache>();
    internal virtual DartMap<object, _CachedImage__image_cache> _cache { get; private set; } =
        new DartMap<object, _CachedImage__image_cache>();
    internal virtual DartMap<object, _LiveImage__image_cache> _liveImages { get; private set; } =
        new DartMap<object, _LiveImage__image_cache>();
    internal virtual long _maximumSize { get; set; } = Image_cacheLibrary._kDefaultSize;
    internal virtual long _maximumSizeBytes { get; set; } = Image_cacheLibrary._kDefaultSizeBytes;
    internal virtual long _currentSizeBytes { get; set; } = 0L;

    public virtual long maximumSize
    {
        get => _maximumSize;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value >= 0L);
            if (__value == maximumSize)
            {
                return;
            }
            TimelineTask? debugTimelineTask = default!;
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask = (
                    (Func<TimelineTask>)(
                        () =>
                        {
                            var __cascade = new TimelineTask();
                            __cascade.start(
                                "ImageCache.setMaximumSize",
                                arguments: new DartMap<string, object?> { ["value"] = __value }
                            );
                            return __cascade;
                        }
                    )
                )();
            }
            _maximumSize = __value;
            if (maximumSize == 0L)
            {
                clear();
            }
            else
            {
                _checkCacheSize(debugTimelineTask);
            }
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask!.finish();
            }
        }
    }
    public virtual long currentSize => checked(_cache.Count);
    public virtual long maximumSizeBytes
    {
        get => _maximumSizeBytes;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value >= 0L);
            if (__value == _maximumSizeBytes)
            {
                return;
            }
            TimelineTask? debugTimelineTask = default!;
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask = (
                    (Func<TimelineTask>)(
                        () =>
                        {
                            var __cascade = new TimelineTask();
                            __cascade.start(
                                "ImageCache.setMaximumSizeBytes",
                                arguments: new DartMap<string, object?> { ["value"] = __value }
                            );
                            return __cascade;
                        }
                    )
                )();
            }
            _maximumSizeBytes = __value;
            if (_maximumSizeBytes == 0L)
            {
                clear();
            }
            else
            {
                _checkCacheSize(debugTimelineTask);
            }
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask!.finish();
            }
        }
    }
    public virtual long currentSizeBytes => _currentSizeBytes;

    public virtual void clear()
    {
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            Timeline.instantSync(
                "ImageCache.clear",
                arguments: new DartMap<string, object?>
                {
                    ["pendingImages"] = checked((long)_pendingImages.Count),
                    ["keepAliveImages"] = checked((long)_cache.Count),
                    ["liveImages"] = checked((long)_liveImages.Count),
                    ["currentSizeInBytes"] = _currentSizeBytes,
                }
            );
        }
        foreach (_CachedImage__image_cache image in _cache.Values)
        {
            image.dispose();
        }
        _cache.Clear();
        foreach (_PendingImage__image_cache pendingImage in _pendingImages.Values)
        {
            pendingImage.removeListener();
        }
        _pendingImages.Clear();
        _currentSizeBytes = 0L;
    }

    public virtual bool evict(object key, bool includeLive = true)
    {
        if (includeLive)
        {
            _LiveImage__image_cache? image = _liveImages.remove(key);
            image?.dispose();
        }
        _PendingImage__image_cache? pendingImage = _pendingImages.remove(key);
        if (pendingImage is not null)
        {
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                Timeline.instantSync(
                    "ImageCache.evict",
                    arguments: new DartMap<string, object?> { ["type"] = "pending" }
                );
            }
            pendingImage.removeListener();
            return true;
        }
        _CachedImage__image_cache? imageLocal = _cache.remove(key);
        if (imageLocal is not null)
        {
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                Timeline.instantSync(
                    "ImageCache.evict",
                    arguments: new DartMap<string, object?>
                    {
                        ["type"] = "keepAlive",
                        ["sizeInBytes"] = imageLocal.sizeBytes,
                    }
                );
            }
            _currentSizeBytes -= (
                imageLocal.sizeBytes
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            imageLocal.dispose();
            return true;
        }
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            Timeline.instantSync(
                "ImageCache.evict",
                arguments: new DartMap<string, object?> { ["type"] = "miss" }
            );
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _touch(
        object key,
        _CachedImage__image_cache image,
        TimelineTask? timelineTask
    )
    {
        if (
            (image.sizeBytes is not null)
            && (
                (
                    image.sizeBytes
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) <= maximumSizeBytes
            )
            && (maximumSize > 0L)
        )
        {
            _currentSizeBytes += (
                image.sizeBytes
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            _cache[key] = image;
            _checkCacheSize(timelineTask);
        }
        else
        {
            image.dispose();
        }
    }

    internal virtual void _trackLiveImage(
        object key,
        ImageStreamCompleter completer,
        long? sizeBytes
    )
    {
        _liveImages
            .putIfAbsent(
                key,
                () =>
                {
                    return new _LiveImage__image_cache(
                        completer,
                        () =>
                        {
                            _liveImages.remove(key);
                        }
                    );
                }
            )
            .sizeBytes ??= sizeBytes;
    }

    public virtual ImageStreamCompleter? putIfAbsent(
        object key,
        Func<ImageStreamCompleter> loader,
        Action<object, System.Diagnostics.StackTrace?>? onError = null
    )
    {
        TimelineTask? debugTimelineTask = default!;
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            debugTimelineTask = (
                (Func<TimelineTask>)(
                    () =>
                    {
                        var __cascade = new TimelineTask();
                        __cascade.start(
                            "ImageCache.putIfAbsent",
                            arguments: new DartMap<string, object?> { ["key"] = key.ToString() }
                        );
                        return __cascade;
                    }
                )
            )();
        }
        ImageStreamCompleter? result = _pendingImages.GetValueOrDefault(key)?.completer;
        if (result is not null)
        {
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask!.finish(
                    arguments: new DartMap<string, object?> { ["result"] = "pending" }
                );
            }
            return result;
        }
        _CachedImage__image_cache? imageLocal = _cache.remove(key);
        if (imageLocal is not null)
        {
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask!.finish(
                    arguments: new DartMap<string, object?> { ["result"] = "keepAlive" }
                );
            }
            _trackLiveImage(key, imageLocal.completer, imageLocal.sizeBytes);
            _cache[key] = imageLocal;
            return imageLocal.completer;
        }
        _LiveImage__image_cache? liveImage = _liveImages.GetValueOrDefault(key);
        if (liveImage is not null)
        {
            _touch(
                key,
                new _CachedImage__image_cache(liveImage.completer, sizeBytes: liveImage.sizeBytes),
                debugTimelineTask
            );
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask!.finish(
                    arguments: new DartMap<string, object?> { ["result"] = "keepAlive" }
                );
            }
            return liveImage.completer;
        }
        try
        {
            result = loader();
            _trackLiveImage(key, result, null);
        }
        catch (Exception error)
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask!.finish(
                    arguments: new DartMap<string, object?>
                    {
                        ["result"] = "error",
                        ["error"] = error.ToString(),
                        ["stackTrace"] = stackTrace.ToString(),
                    }
                );
            }
            if (onError is not null)
            {
                onError(error, stackTrace);
                return null;
            }
            else
            {
                throw;
            }
        }
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            debugTimelineTask!.start("listener");
        }
        var listenedOnce = false;
        bool trackPendingImage = (maximumSize > 0L) && (maximumSizeBytes > 0L);
        _PendingImage__image_cache pendingImage = default!;
        void listener(ImageInfo? info, bool syncCall)
        {
            long? sizeBytesLocal = default!;
            if (info is not null)
            {
                sizeBytesLocal = info.sizeBytes;
                info.dispose();
            }
            var imageAlternate = new _CachedImage__image_cache(result!, sizeBytes: sizeBytesLocal);
            _trackLiveImage(key, result, sizeBytesLocal);
            if (trackPendingImage)
            {
                _touch(key, imageAlternate, debugTimelineTask);
            }
            else
            {
                imageAlternate.dispose();
            }
            _pendingImages.remove(key);
            if (!listenedOnce)
            {
                pendingImage.removeListener();
            }
            if (!Foundation.ConstantsLibrary.kReleaseMode && !listenedOnce)
            {
                (
                    (Func<TimelineTask>)(
                        () =>
                        {
                            var __cascade = debugTimelineTask!;
                            __cascade.finish(
                                arguments: new DartMap<string, object?>
                                {
                                    ["syncCall"] = syncCall,
                                    ["sizeInBytes"] = sizeBytesLocal,
                                }
                            );
                            __cascade.finish(
                                arguments: new DartMap<string, object?>
                                {
                                    ["currentSizeBytes"] = currentSizeBytes,
                                    ["currentSize"] = currentSize,
                                }
                            );
                            return __cascade;
                        }
                    )
                )();
            }
            listenedOnce = true;
        }
        var streamListener = new ImageStreamListener(
            (image, synchronousCall) => listener(image, synchronousCall)
        );
        pendingImage = new _PendingImage__image_cache(result, streamListener);
        if (trackPendingImage)
        {
            _pendingImages[key] = pendingImage;
        }
        result.addListener(streamListener);
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual ImageCacheStatus statusForKey(object key)
    {
        return new ImageCacheStatus(
            pending: _pendingImages.ContainsKey(key),
            keepAlive: _cache.ContainsKey(key),
            live: _liveImages.ContainsKey(key)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool containsKey(object key)
    {
        return _pendingImages.ContainsKey(key) || _cache.ContainsKey(key);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual long liveImageCount => checked(_liveImages.Count);
    public virtual long pendingImageCount => checked(_pendingImages.Count);

    public virtual void clearLiveImages()
    {
        foreach (_LiveImage__image_cache image in _liveImages.Values)
        {
            image.dispose();
        }
        _liveImages.Clear();
    }

    internal virtual void _checkCacheSize(TimelineTask? timelineTask)
    {
        var finishArgs = new DartMap<string, object?>();
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            timelineTask!.start("checkCacheSize");
            finishArgs["evictedKeys"] = new List<string?>();
            finishArgs["currentSize"] = currentSize;
            finishArgs["currentSizeBytes"] = currentSizeBytes;
        }
        while ((_currentSizeBytes > _maximumSizeBytes) || (checked(_cache.Count) > _maximumSize))
        {
            object key = _cache.Keys.First();
            _CachedImage__image_cache image = _cache.GetValueOrDefault(key)!;
            _currentSizeBytes -= (
                image.sizeBytes
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            image.dispose();
            _cache.remove(key);
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                ((List<string?>?)finishArgs.GetValueOrDefault("evictedKeys"))!.Add(key.ToString());
            }
        }
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            finishArgs["endSize"] = currentSize;
            finishArgs["endSizeBytes"] = currentSizeBytes;
            timelineTask!.finish(arguments: finishArgs);
        }
        DartRuntimePrimitives.Assert(() => _currentSizeBytes >= 0L);
        DartRuntimePrimitives.Assert(() => checked(_cache.Count) <= maximumSize);
        DartRuntimePrimitives.Assert(() => _currentSizeBytes <= maximumSizeBytes);
    }
}

public class ImageCacheStatus
{
    public virtual bool pending { get; private set; } = default!;
    public virtual bool keepAlive { get; private set; } = default!;
    public virtual bool live { get; private set; } = default!;

    public ImageCacheStatus(bool pending = false, bool keepAlive = false, bool live = false)
    {
        this.pending = pending;
        this.keepAlive = keepAlive;
        this.live = live;
        System.Diagnostics.Debug.Assert(!pending || !keepAlive);
    }

    public virtual bool tracked => pending || keepAlive || live;
    public virtual bool untracked => !pending && !keepAlive && !live;

    public override bool Equals(object? other)
    {
        var __other = other as ImageCacheStatus;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ImageCacheStatus)
            && (__other.pending == pending)
            && (__other.keepAlive == keepAlive)
            && (__other.live == live);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(pending, keepAlive, live);

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "ImageCacheStatus")}(pending: {pending}, live: {live}, keepAlive: {keepAlive})";
}

internal abstract class _CachedImageBase__image_cache
{
    public virtual ImageStreamCompleter completer { get; private set; } = default!;
    public virtual long? sizeBytes { get; set; } = default;
    public virtual ImageStreamCompleterHandle? handle { get; set; } = default;

    internal _CachedImageBase__image_cache(ImageStreamCompleter completer, long? sizeBytes = null)
    {
        this.completer = completer;
        this.sizeBytes = sizeBytes;
        handle = completer.keepAlive();
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => handle is not null);
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        SchedulerBinding.instance.addPostFrameCallback(
            (timeStamp) =>
            {
                DartRuntimePrimitives.Assert(() => handle is not null);
                handle?.dispose();
                handle = null;
            },
            debugLabel: "CachedImage.disposeHandle"
        );
    }
}

internal class _CachedImage__image_cache : _CachedImageBase__image_cache
{
    internal _CachedImage__image_cache(ImageStreamCompleter completer, long? sizeBytes = null)
        : base(
            completer,
            sizeBytes: (
                sizeBytes
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        ) { }
}

internal class _LiveImage__image_cache : _CachedImageBase__image_cache
{
    internal virtual Action _handleRemove { get; set; } = default!;

    internal _LiveImage__image_cache(ImageStreamCompleter completer, Action handleRemove)
        : base(completer) { }

    public override void dispose()
    {
        completer.removeOnLastListenerRemovedCallback(_handleRemove);
        base.dispose();
    }

    public override string ToString() => DiagnosticsLibrary.describeIdentity(this);
}

internal class _PendingImage__image_cache
{
    public virtual ImageStreamCompleter completer { get; private set; } = default!;
    public virtual ImageStreamListener listener { get; private set; } = default!;

    internal _PendingImage__image_cache(
        ImageStreamCompleter completer,
        ImageStreamListener listener
    )
    {
        this.completer = completer;
        this.listener = listener;
    }

    public virtual void removeListener()
    {
        completer.removeListener(listener);
    }
}
