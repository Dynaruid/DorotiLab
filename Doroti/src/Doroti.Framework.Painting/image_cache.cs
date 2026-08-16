// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/image_cache.dart
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

public static partial class Image_cacheLibrary
{
    internal static long _kDefaultSize = 1000L;
}

public static partial class Image_cacheLibrary
{
    internal static long _kDefaultSizeBytes = (100L << (int)(20L));
}

public class ImageCache
{
    internal virtual DartMap<object, _PendingImage__image_cache> _pendingImages { get; private set; } = new DartMap<object, _PendingImage__image_cache>();
    internal virtual DartMap<object, _CachedImage__image_cache> _cache { get; private set; } = new DartMap<object, _CachedImage__image_cache>();
    internal virtual DartMap<object, _LiveImage__image_cache> _liveImages { get; private set; } = new DartMap<object, _LiveImage__image_cache>();
    internal virtual long _maximumSize { get; set; } = Image_cacheLibrary._kDefaultSize;
    internal virtual long _maximumSizeBytes { get; set; } = Image_cacheLibrary._kDefaultSizeBytes;
    internal virtual long _currentSizeBytes { get; set; } = 0L;

    public virtual long maximumSize
    {
        get => this._maximumSize;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0L));
            if ((__value == this.maximumSize))
            {
                return;
            }
            TimelineTask? debugTimelineTask__3943 = default!;
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask__3943 = ((Func<TimelineTask>)(() =>
{
    var __cascade = new TimelineTask();
    __cascade.start("ImageCache.setMaximumSize", arguments: new DartMap<string, object> { ["value"] = __value });
    return __cascade;
}))();
            }
            _maximumSize = __value;
            if ((this.maximumSize == 0L))
            {
                clear();
            }
            else
            {
                _checkCacheSize(debugTimelineTask__3943);
            }
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask__3943!.finish();
            }
        }
    }
    public virtual long currentSize => checked((long)(this._cache.Count));
    public virtual long maximumSizeBytes
    {
        get => this._maximumSizeBytes;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0L));
            if ((__value == this._maximumSizeBytes))
            {
                return;
            }
            TimelineTask? debugTimelineTask__5157 = default!;
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask__5157 = ((Func<TimelineTask>)(() =>
{
    var __cascade = new TimelineTask();
    __cascade.start("ImageCache.setMaximumSizeBytes", arguments: new DartMap<string, object> { ["value"] = __value });
    return __cascade;
}))();
            }
            _maximumSizeBytes = __value;
            if ((this._maximumSizeBytes == 0L))
            {
                clear();
            }
            else
            {
                _checkCacheSize(debugTimelineTask__5157);
            }
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask__5157!.finish();
            }
        }
    }
    public virtual long currentSizeBytes => this._currentSizeBytes;
    public virtual void clear()
    {
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            Timeline.instantSync("ImageCache.clear", arguments: new DartMap<string, object> { ["pendingImages"] = checked((long)(this._pendingImages.Count)), ["keepAliveImages"] = checked((long)(this._cache.Count)), ["liveImages"] = checked((long)(this._liveImages.Count)), ["currentSizeInBytes"] = this._currentSizeBytes });
        }
        foreach (_CachedImage__image_cache image__6709 in this._cache.Values)
        {
            image__6709.dispose();
        }
        this._cache.Clear();
        foreach (_PendingImage__image_cache pendingImage__6813 in this._pendingImages.Values)
        {
            pendingImage__6813.removeListener();
        }
        this._pendingImages.Clear();
        _currentSizeBytes = 0L;
    }

    public virtual bool evict(object key, bool includeLive = true)
    {
        if (includeLive)
        {
            _LiveImage__image_cache? image__9342 = this._liveImages.remove(key);
            image__9342?.dispose();
        }
        _PendingImage__image_cache? pendingImage__9430 = this._pendingImages.remove(key);
        if ((pendingImage__9430 is not null))
        {
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                Timeline.instantSync("ImageCache.evict", arguments: new DartMap<string, object> { ["type"] = "pending" });
            }
            pendingImage__9430.removeListener();
            return true;
        }
        _CachedImage__image_cache? image__9725 = this._cache.remove(key);
        if ((image__9725 is not null))
        {
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                Timeline.instantSync("ImageCache.evict", arguments: new DartMap<string, object> { ["type"] = "keepAlive", ["sizeInBytes"] = image__9725.sizeBytes });
            }
            _currentSizeBytes -= DartRuntimePrimitives.RequireValue(image__9725.sizeBytes);
            image__9725.dispose();
            return true;
        }
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            Timeline.instantSync("ImageCache.evict", arguments: new DartMap<string, object> { ["type"] = "miss" });
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _touch(object key, _CachedImage__image_cache image, TimelineTask? timelineTask)
    {
        if ((((image.sizeBytes is not null) && (DartRuntimePrimitives.RequireValue(image.sizeBytes) <= this.maximumSizeBytes)) && (this.maximumSize > 0L)))
        {
            _currentSizeBytes += DartRuntimePrimitives.RequireValue(image.sizeBytes);
            this._cache[key] = image;
            _checkCacheSize(timelineTask);
        }
        else
        {
            image.dispose();
        }
    }

    internal virtual void _trackLiveImage(object key, ImageStreamCompleter completer, long? sizeBytes)
    {
        this._liveImages.putIfAbsent(key, (() =>
        {
            return new _LiveImage__image_cache(completer, (() =>
            {
                this._liveImages.remove(key);
            }));
            return default;
        })).sizeBytes ??= sizeBytes;
    }

    public virtual ImageStreamCompleter? putIfAbsent(object key, Func<ImageStreamCompleter> loader, Action<object, global::System.Diagnostics.StackTrace?>? onError = null)
    {
        TimelineTask? debugTimelineTask__12217 = default!;
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            debugTimelineTask__12217 = ((Func<TimelineTask>)(() =>
{
    var __cascade = new TimelineTask();
    __cascade.start("ImageCache.putIfAbsent", arguments: new DartMap<string, object> { ["key"] = key.ToString() });
    return __cascade;
}))();
        }
        ImageStreamCompleter? result__12430 = this._pendingImages.GetValueOrDefault(key)?.completer;
        if ((result__12430 is not null))
        {
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask__12217!.finish(arguments: new DartMap<string, object> { ["result"] = "pending" });
            }
            return result__12430;
        }
        _CachedImage__image_cache? image__13007 = this._cache.remove(key);
        if ((image__13007 is not null))
        {
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask__12217!.finish(arguments: new DartMap<string, object> { ["result"] = "keepAlive" });
            }
            _trackLiveImage(key, image__13007.completer, image__13007.sizeBytes);
            this._cache[key] = image__13007;
            return image__13007.completer;
        }
        _LiveImage__image_cache? liveImage__13475 = this._liveImages.GetValueOrDefault(key);
        if ((liveImage__13475 is not null))
        {
            _touch(key, new _CachedImage__image_cache(liveImage__13475.completer, sizeBytes: liveImage__13475.sizeBytes), debugTimelineTask__12217);
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask__12217!.finish(arguments: new DartMap<string, object> { ["result"] = "keepAlive" });
            }
            return liveImage__13475.completer;
        }
        try
        {
            result__12430 = loader();
            _trackLiveImage(key, result__12430, null);
        }
        catch (Exception error__13926)
        {
            var stackTrace__13933 = new System.Diagnostics.StackTrace();
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugTimelineTask__12217!.finish(arguments: new DartMap<string, object> { ["result"] = "error", ["error"] = error__13926.ToString(), ["stackTrace"] = stackTrace__13933.ToString() });
            }
            if ((onError is not null))
            {
                onError(error__13926, stackTrace__13933);
                return null;
            }
            else
            {
                throw;
            }
        }
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            debugTimelineTask__12217!.start("listener");
        }
        var listenedOnce__14630 = false;
        bool trackPendingImage__14857 = ((this.maximumSize > 0L) && (this.maximumSizeBytes > 0L));
        _PendingImage__image_cache pendingImage__14941 = default!;
        void listener(ImageInfo? info, bool syncCall)
        {
            long? sizeBytes__15018 = default!;
            if ((info is not null))
            {
                sizeBytes__15018 = ((ImageInfo)info).sizeBytes;
                info.dispose();
            }
            var image__15135 = new _CachedImage__image_cache(result__12430!, sizeBytes: sizeBytes__15018);
            _trackLiveImage(key, result__12430, sizeBytes__15018);
            if (trackPendingImage__14857)
            {
                _touch(key, image__15135, debugTimelineTask__12217);
            }
            else
            {
                image__15135.dispose();
            }
            this._pendingImages.remove(key);
            if (!listenedOnce__14630)
            {
                pendingImage__14941.removeListener();
            }
            if ((!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && !listenedOnce__14630))
            {
                ((Func<TimelineTask>)(() =>
{
    var __cascade = debugTimelineTask__12217!;
    __cascade.finish(arguments: new DartMap<string, object> { ["syncCall"] = syncCall, ["sizeInBytes"] = sizeBytes__15018 });
    __cascade.finish(arguments: new DartMap<string, object> { ["currentSizeBytes"] = this.currentSizeBytes, ["currentSize"] = this.currentSize });
    return __cascade;
}))();
            }
            listenedOnce__14630 = true;
        }
        var streamListener__15956 = new ImageStreamListener((Action<ImageInfo, bool>)((image, synchronousCall) => listener(image, synchronousCall)));
        pendingImage__14941 = new _PendingImage__image_cache(result__12430, streamListener__15956);
        if (trackPendingImage__14857)
        {
            this._pendingImages[key] = pendingImage__14941;
        }
        result__12430.addListener(streamListener__15956);
        return result__12430;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ImageCacheStatus statusForKey(object key)
    {
        return new ImageCacheStatus(pending: this._pendingImages.ContainsKey(key), keepAlive: this._cache.ContainsKey(key), live: this._liveImages.ContainsKey(key));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool containsKey(object key)
    {
        return ((this._pendingImages.ContainsKey(key)) || (this._cache.ContainsKey(key)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long liveImageCount => checked((long)(this._liveImages.Count));
    public virtual long pendingImageCount => checked((long)(this._pendingImages.Count));
    public virtual void clearLiveImages()
    {
        foreach (_LiveImage__image_cache image__17785 in this._liveImages.Values)
        {
            image__17785.dispose();
        }
        this._liveImages.Clear();
    }

    internal virtual void _checkCacheSize(TimelineTask? timelineTask)
    {
        var finishArgs__18051 = new DartMap<string, object>();
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            timelineTask!.start("checkCacheSize");
            finishArgs__18051["evictedKeys"] = new List<string>();
            finishArgs__18051["currentSize"] = this.currentSize;
            finishArgs__18051["currentSizeBytes"] = this.currentSizeBytes;
        }
        while (((this._currentSizeBytes > this._maximumSizeBytes) || (checked((long)(this._cache.Count)) > this._maximumSize)))
        {
            object key__18414 = this._cache.Keys.First();
            _CachedImage__image_cache image__18464 = this._cache.GetValueOrDefault(key__18414)!;
            _currentSizeBytes -= DartRuntimePrimitives.RequireValue(image__18464.sizeBytes);
            image__18464.dispose();
            this._cache.remove(key__18414);
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                (((List<string>?)(object?)finishArgs__18051.GetValueOrDefault("evictedKeys"))!).Add(key__18414.ToString());
            }
        }
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            finishArgs__18051["endSize"] = this.currentSize;
            finishArgs__18051["endSizeBytes"] = this.currentSizeBytes;
            timelineTask!.finish(arguments: finishArgs__18051);
        }
        DartRuntimePrimitives.Assert(() => (this._currentSizeBytes >= 0L));
        DartRuntimePrimitives.Assert(() => (checked((long)(this._cache.Count)) <= this.maximumSize));
        DartRuntimePrimitives.Assert(() => (this._currentSizeBytes <= this.maximumSizeBytes));
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
        System.Diagnostics.Debug.Assert((!pending || !keepAlive));
    }

    public virtual bool tracked => ((this.pending || this.keepAlive) || this.live);
    public virtual bool untracked => ((!this.pending && !this.keepAlive) && !this.live);
    public override bool Equals(object? other)
    {
        var __other = other as ImageCacheStatus;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is ImageCacheStatus) && (((ImageCacheStatus)((ImageCacheStatus)__other)).pending == this.pending)) && (((ImageCacheStatus)((ImageCacheStatus)__other)).keepAlive == this.keepAlive)) && (((ImageCacheStatus)((ImageCacheStatus)__other)).live == this.live));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.pending, this.keepAlive, this.live);
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ImageCacheStatus"))}(pending: {this.pending}, live: {this.live}, keepAlive: {this.keepAlive})";
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
        this.handle = completer.keepAlive();
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => (this.handle is not null));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        SchedulerBinding.instance.addPostFrameCallback(((timeStamp) =>
        {
            DartRuntimePrimitives.Assert(() => (this.handle is not null));
            this.handle?.dispose();
            handle = null;
        }), debugLabel: "CachedImage.disposeHandle");
    }

}

internal class _CachedImage__image_cache : _CachedImageBase__image_cache
{
    internal _CachedImage__image_cache(ImageStreamCompleter completer, long? sizeBytes = null) : base(completer, sizeBytes: DartRuntimePrimitives.RequireValue(sizeBytes))
    {
    }

}

internal class _LiveImage__image_cache : _CachedImageBase__image_cache
{
    internal virtual Action _handleRemove { get; set; } = default!;

    internal _LiveImage__image_cache(ImageStreamCompleter completer, Action handleRemove) : base(completer)
    {
    }

    public override void dispose()
    {
        completer.removeOnLastListenerRemovedCallback((Action)this._handleRemove);
        base.dispose();
    }

    public override string ToString() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
}

internal class _PendingImage__image_cache
{
    public virtual ImageStreamCompleter completer { get; private set; } = default!;
    public virtual ImageStreamListener listener { get; private set; } = default!;

    internal _PendingImage__image_cache(ImageStreamCompleter completer, ImageStreamListener listener)
    {
        this.completer = completer;
        this.listener = listener;
    }

    public virtual void removeListener()
    {
        this.completer.removeListener(this.listener);
    }

}

