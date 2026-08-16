// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/image_stream.dart
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

public class ImageInfo
{
    public virtual Image image { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;
    public virtual string? debugLabel { get; private set; }
    public ImageInfo() { }


    public ImageInfo(Image image, double scale = 1.0, string? debugLabel = null)
    {
        this.image = image;
        this.scale = scale;
        this.debugLabel = debugLabel;
    }

    public virtual ImageInfo clone()
    {
        return new ImageInfo(image: this.image.clone(), scale: this.scale, debugLabel: this.debugLabel);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isCloneOf(ImageInfo other)
    {
        return ((((ImageInfo)other).image.isCloneOf(this.image) && (((ImageInfo)other).scale == this.scale)) && (((ImageInfo)other).debugLabel == this.debugLabel));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long sizeBytes => ((this.image.height * this.image.width) * 4L);
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => (((((long?)(global::Doroti.Ui.Image.debugGetOpenHandleStackTraces()?.Count)) ?? 1L)) > 0L));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this.image.dispose();
    }

    public override string ToString() => $"{((this.debugLabel is not null) ? $"{this.debugLabel} " : "")}{this.image} @ {(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(this.scale))}x";
    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.image, this.scale, this.debugLabel);
    public override bool Equals(object? other)
    {
        var __other = other as ImageInfo;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is ImageInfo) && (object.Equals(((ImageInfo)((ImageInfo)__other)).image, this.image))) && (((ImageInfo)((ImageInfo)__other)).scale == this.scale)) && (((ImageInfo)((ImageInfo)__other)).debugLabel == this.debugLabel));
    }

}

public class ImageStreamListener
{
    public virtual Action<ImageInfo, bool> onImage { get; private set; } = default!;
    public virtual Action<ImageChunkEvent>? onChunk { get; private set; }
    public virtual Action<object, global::System.Diagnostics.StackTrace?>? onError { get; private set; }
    public virtual bool reportErrors { get; private set; } = default!;

    public ImageStreamListener(Action<ImageInfo, bool> onImage, Action<ImageChunkEvent>? onChunk = null, Action<object, global::System.Diagnostics.StackTrace?>? onError = null, bool reportErrors = true)
    {
        this.onImage = onImage;
        this.onChunk = onChunk;
        this.onError = onError;
        this.reportErrors = reportErrors;
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.onImage, this.onChunk, this.onError, this.reportErrors);
    public override bool Equals(object? other)
    {
        var __other = other as ImageStreamListener;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is ImageStreamListener) && (object.Equals((Action<ImageInfo, bool>)((ImageStreamListener)((ImageStreamListener)__other)).onImage, (Action<ImageInfo, bool>)this.onImage))) && (object.Equals((Action<ImageChunkEvent>?)((ImageStreamListener)((ImageStreamListener)__other)).onChunk, (Action<ImageChunkEvent>?)this.onChunk))) && (object.Equals((Action<object, global::System.Diagnostics.StackTrace?>?)((ImageStreamListener)((ImageStreamListener)__other)).onError, (Action<object, global::System.Diagnostics.StackTrace?>?)this.onError))) && (((ImageStreamListener)((ImageStreamListener)__other)).reportErrors == this.reportErrors));
    }

}

public delegate void ImageListener(ImageInfo image, bool synchronousCall);

public delegate void ImageChunkListener(ImageChunkEvent @event);

public delegate void ImageErrorListener(object exception, global::System.Diagnostics.StackTrace? stackTrace);

public class ImageChunkEvent : Diagnosticable
{
    public virtual long cumulativeBytesLoaded { get; private set; } = default!;
    public virtual long? expectedTotalBytes { get; private set; }

    public ImageChunkEvent(long cumulativeBytesLoaded, long? expectedTotalBytes)
    {
        this.cumulativeBytesLoaded = cumulativeBytesLoaded;
        this.expectedTotalBytes = expectedTotalBytes;
        System.Diagnostics.Debug.Assert((cumulativeBytesLoaded >= 0L));
        System.Diagnostics.Debug.Assert(((expectedTotalBytes is null) || (expectedTotalBytes >= 0L)));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("cumulativeBytesLoaded", this.cumulativeBytesLoaded));
        properties.add(new IntProperty("expectedTotalBytes", this.expectedTotalBytes));
    }

}

public class ImageStream : Diagnosticable
{
    internal virtual ImageStreamCompleter? _completer { get; set; } = default;
    internal virtual List<ImageStreamListener>? _listeners { get; set; } = default;

    public ImageStream()
    {
    }

    public virtual ImageStreamCompleter? completer => this._completer;
    public virtual void setCompleter(ImageStreamCompleter value)
    {
        DartRuntimePrimitives.Assert(() => (this._completer is null));
        _completer = value;
        if ((this._listeners is not null))
        {
            List<ImageStreamListener> initialListeners__14296 = this._listeners!;
            _listeners = null;
            this._completer!._addingInitialListeners = true;
            initialListeners__14296.forEach(this._completer!.addListener);
            this._completer!._addingInitialListeners = false;
        }
    }

    public virtual void addListener(ImageStreamListener listener)
    {
        if ((this._completer is not null))
        {
            this._completer!.addListener(listener);
            return;
        }
        _listeners ??= new List<ImageStreamListener>();
        this._listeners!.Add(listener);
    }

    public virtual void removeListener(ImageStreamListener listener)
    {
        if ((this._completer is not null))
        {
            this._completer!.removeListener(listener);
            return;
        }
        DartRuntimePrimitives.Assert(() => (this._listeners is not null));
        for (var i__16247 = 0L; (i__16247 < checked((long)(this._listeners!.Count))); i__16247 += 1L)
        {
            if ((object.Equals(this._listeners![(int)(i__16247)], listener)))
            {
                this._listeners!.removeAt(i__16247);
                break;
            }
        }
    }

    public virtual object key => (((object?)this._completer ?? (object?)this));
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new ObjectFlagProperty<ImageStreamCompleter>("completer", this._completer, ifPresent: ((Diagnosticable)this._completer).toStringShort(), ifNull: "unresolved"));
        properties.add(new ObjectFlagProperty<List<ImageStreamListener>>("listeners", this._listeners, ifPresent: $"{((long?)(this._listeners?.Count))} listener{((((long?)(this._listeners?.Count)) == 1L) ? "" : "s")}", ifNull: "no listeners", level: ((this._completer is not null) ? DiagnosticLevel.hidden : DiagnosticLevel.info)));
        this._completer?.debugFillProperties(properties);
    }

}

public class ImageStreamCompleterHandle
{
    internal virtual ImageStreamCompleter? _completer { get; set; } = default;

    public ImageStreamCompleterHandle(ImageStreamCompleter _completer)
    {
        this._completer = _completer;
        this._completer._keepAliveHandles += 1L;
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => (this._completer is not null));
        DartRuntimePrimitives.Assert(() => (this._completer!._keepAliveHandles > 0L));
        DartRuntimePrimitives.Assert(() => !this._completer!._disposed);
        this._completer!._keepAliveHandles -= 1L;
        this._completer!._maybeDispose();
        _completer = null;
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
    }

}

public abstract class ImageStreamCompleter : Diagnosticable
{
    internal virtual List<ImageStreamListener> _listeners { get; private set; } = new List<ImageStreamListener>();
    internal virtual List<Action<object, global::System.Diagnostics.StackTrace?>> _ephemeralErrorListeners { get; private set; } = new List<Action<object, global::System.Diagnostics.StackTrace?>>();
    internal virtual ImageInfo? _currentImage { get; set; } = default;
    internal virtual FlutterErrorDetails? _currentError { get; set; } = default;
    public virtual string? debugLabel { get; set; } = default;
    internal virtual bool _addingInitialListeners { get; set; } = false;
    internal virtual bool _hadErrorListener { get; set; } = false;
    internal virtual long _keepAliveHandles { get; set; } = 0L;
    internal virtual bool _disposed { get; set; } = false;
    internal virtual List<Action> _onLastListenerRemovedCallbacks { get; private set; } = new List<Action>();

    public virtual bool hasListeners => (checked((long)(this._listeners.Count)) != 0);
    public virtual void addListener(ImageStreamListener listener)
    {
        _checkDisposed();
        if (!((ImageStreamListener)listener).reportErrors)
        {
            _hadErrorListener = true;
        }
        this._listeners.Add(listener);
        if ((this._currentImage is not null))
        {
            try
            {
                listener.onImage(this._currentImage!.clone(), !this._addingInitialListeners);
            }
            catch (Exception exception__22664)
            {
                var stack__22675 = new System.Diagnostics.StackTrace();
                reportError(context: new ErrorDescription("by a synchronously-called image listener"), exception: exception__22664, stack: stack__22675);
            }
        }
        if (((this._currentError is not null) && (((ImageStreamListener)listener).onError is not null)))
        {
            try
            {
                ((ImageStreamListener)listener).onError!(this._currentError!.exception, this._currentError!.stack);
            }
            catch (Exception newException__23030)
            {
                var newStack__23044 = new System.Diagnostics.StackTrace();
                if ((!object.Equals(newException__23030, this._currentError!.exception)))
                {
                    FlutterError.reportError(new FlutterErrorDetails(exception: newException__23030, library: "image resource service", context: new ErrorDescription("by a synchronously-called image error listener"), stack: newStack__23044));
                }
            }
        }
    }

    public virtual void addEphemeralErrorListener(Action<object, global::System.Diagnostics.StackTrace?> listener)
    {
        _checkDisposed();
        if ((this._currentError is not null))
        {
            try
            {
                listener(this._currentError!.exception, this._currentError!.stack);
            }
            catch (Exception newException__25108)
            {
                var newStack__25122 = new System.Diagnostics.StackTrace();
                if ((!object.Equals(newException__25108, this._currentError!.exception)))
                {
                    FlutterError.reportError(new FlutterErrorDetails(exception: newException__25108, library: "image resource service", context: new ErrorDescription("by a synchronously-called image error listener"), stack: newStack__25122));
                }
            }
        }
        else
        {
            if ((this._currentImage is null))
            {
                this._ephemeralErrorListeners.Add(listener);
            }
        }
    }

    public virtual ImageStreamCompleterHandle keepAlive()
    {
        _checkDisposed();
        return new ImageStreamCompleterHandle(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void removeListener(ImageStreamListener listener)
    {
        _checkDisposed();
        for (var i__26755 = 0L; (i__26755 < checked((long)(this._listeners.Count))); i__26755 += 1L)
        {
            if ((object.Equals(this._listeners[(int)(i__26755)], listener)))
            {
                this._listeners.removeAt(i__26755);
                break;
            }
        }
        if ((checked((long)(this._listeners.Count)) == 0))
        {
            List<Action> callbacks__26956 = this._onLastListenerRemovedCallbacks.ToList();
            foreach (var callback__27027 in callbacks__26956)
            {
                callback__27027();
            }
            this._onLastListenerRemovedCallbacks.Clear();
            _maybeDispose();
        }
    }

    public virtual void onDisposed()
    {
    }

    public virtual void maybeDispose()
    {
        _maybeDispose();
    }

    internal virtual void _maybeDispose()
    {
        if (((this._disposed || (checked((long)(this._listeners.Count)) != 0)) || (this._keepAliveHandles != 0L)))
        {
            return;
        }
        this._ephemeralErrorListeners.Clear();
        this._currentImage?.dispose();
        _currentImage = null;
        _disposed = true;
        onDisposed();
    }

    internal virtual void _checkDisposed()
    {
        if (this._disposed)
        {
            throw new InvalidOperationException("Stream has been disposed.\n" + "An ImageStream is considered disposed once at least one listener has " + "been added and subsequently all listeners have been removed and no " + "handles are outstanding from the keepAlive method.\n" + "To resolve this error, maintain at least one listener on the stream, " + "or create an ImageStreamCompleterHandle from the keepAlive " + "method, or create a new stream for the image.");
        }
    }

    public virtual void addOnLastListenerRemovedCallback(Action callback)
    {
        _checkDisposed();
        this._onLastListenerRemovedCallbacks.Add(callback);
    }

    public virtual void removeOnLastListenerRemovedCallback(Action callback)
    {
        _checkDisposed();
        this._onLastListenerRemovedCallbacks.Remove(callback);
    }

    public virtual void setImage(ImageInfo image)
    {
        _checkDisposed();
        this._currentImage?.dispose();
        _currentImage = image;
        this._ephemeralErrorListeners.Clear();
        if ((checked((long)(this._listeners.Count)) == 0))
        {
            return;
        }
        var localListeners__29575 = new List<ImageStreamListener>(this._listeners);
        foreach (var listener__29649 in localListeners__29575)
        {
            try
            {
                listener__29649.onImage(image.clone(), false);
            }
            catch (Exception exception__29754)
            {
                var stack__29765 = new System.Diagnostics.StackTrace();
                reportError(context: new ErrorDescription("by an image listener"), exception: exception__29754, stack: stack__29765);
            }
        }
    }

    public virtual void reportError(DiagnosticsNode? context = null, object exception = default!, global::System.Diagnostics.StackTrace? stack = null, InformationCollector? informationCollector = null, bool silent = false)
    {
        _currentError = new FlutterErrorDetails(exception: exception, stack: stack, library: "image resource service", context: context, informationCollector: informationCollector, silent: silent);
        var localErrorListeners__32351 = new List<Action<object, global::System.Diagnostics.StackTrace?>>();
        this._ephemeralErrorListeners.Clear();
        var handled__32637 = false;
        foreach (var errorListener__32669 in localErrorListeners__32351)
        {
            try
            {
                errorListener__32669(exception, stack);
                handled__32637 = true;
            }
            catch (Exception newException__32801)
            {
                var newStack__32815 = new System.Diagnostics.StackTrace();
                if ((!object.Equals(newException__32801, exception)))
                {
                    FlutterError.reportError(new FlutterErrorDetails(context: new ErrorDescription("when reporting an error to an image listener"), library: "image resource service", exception: newException__32801, stack: newStack__32815));
                }
            }
        }
        if (!handled__32637)
        {
            if (this._hadErrorListener)
            {
                return;
            }
            FlutterError.reportError(this._currentError!);
        }
    }

    public virtual void reportImageChunkEvent(ImageChunkEvent @event)
    {
        _checkDisposed();
        if (this.hasListeners)
        {
            List<Action<ImageChunkEvent>> localListeners__33907 = this._listeners.map<ImageStreamListener, Action<ImageChunkEvent>?>(((listener) => ((ImageStreamListener)listener).onChunk)).OfType<Action<ImageChunkEvent>>().ToList();
            foreach (var listener__34104 in localListeners__33907)
            {
                listener__34104(@event);
            }
        }
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new DiagnosticsProperty<ImageInfo>("current", this._currentImage, ifNull: "unresolved", showName: false));
        description.add(new ObjectFlagProperty<List<ImageStreamListener>>("listeners", this._listeners, ifPresent: $"{checked((long)(this._listeners.Count))} listener{((checked((long)(this._listeners.Count)) == 1L) ? "" : "s")}"));
        description.add(new ObjectFlagProperty<List<Action<object, global::System.Diagnostics.StackTrace?>>>("ephemeralErrorListeners", this._ephemeralErrorListeners, ifPresent: $"{checked((long)(this._ephemeralErrorListeners.Count))} ephemeralErrorListener{((checked((long)(this._ephemeralErrorListeners.Count)) == 1L) ? "" : "s")}"));
        description.add(new FlagProperty("disposed", value: this._disposed, ifTrue: "<disposed>"));
    }

}

public class OneFrameImageStreamCompleter : ImageStreamCompleter
{
    public OneFrameImageStreamCompleter(Future<ImageInfo> image, InformationCollector? informationCollector = null)
    {
        _ = image.then(
            (Action<ImageInfo>)setImage,
            (Action<Exception, global::System.Diagnostics.StackTrace>)((error, stack) =>
                reportError(
                    context: new ErrorDescription("resolving a single-frame image stream"),
                    exception: error,
                    stack: stack,
                    informationCollector: informationCollector,
                    silent: true)));
    }

}

public class MultiFrameImageStreamCompleter : ImageStreamCompleter
{
    internal virtual StreamSubscription<ImageChunkEvent>? _chunkSubscription { get; set; } = default;
    internal virtual Codec? _codec { get; set; } = default;
    internal virtual double _scale { get; private set; } = default!;
    internal virtual InformationCollector? _informationCollector { get; private set; }
    internal virtual FrameInfo? _nextFrame { get; set; } = default;
    internal virtual Duration _shownTimestamp { get; set; } = default!;
    internal virtual Duration? _frameDuration { get; set; } = default;
    internal virtual long _framesEmitted { get; set; } = 0L;
    internal virtual Timer? _timer { get; set; } = default;
    internal virtual bool _frameCallbackScheduled { get; set; } = false;

    public MultiFrameImageStreamCompleter(Future<Codec> codec, double scale, string? debugLabel = null, Stream<ImageChunkEvent>? chunkEvents = null, InformationCollector? informationCollector = null)
    {
        this._informationCollector = informationCollector;
        this._scale = scale;
    }

    internal virtual void _handleCodecReady(Codec codec)
    {
        _codec = codec;
        DartRuntimePrimitives.Assert(() => (this._codec is not null));
        if (hasListeners)
        {
            _ = _decodeNextFrameAndSchedule();
        }
    }

    internal virtual void _handleAppFrame(Duration timestamp)
    {
        _frameCallbackScheduled = false;
        if (!hasListeners)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (this._nextFrame is not null));
        if ((_isFirstFrame() || _hasFrameDurationPassed(timestamp)))
        {
            _emitFrame(new ImageInfo(image: this._nextFrame!.image.clone(), scale: this._scale, debugLabel: debugLabel));
            _shownTimestamp = timestamp;
            _frameDuration = this._nextFrame!.duration;
            this._nextFrame!.image.dispose();
            _nextFrame = null;
            if ((this._codec is null))
            {
                return;
            }
            long completedCycles__41595 = (checked((long)(this._framesEmitted / this._codec!.frameCount)));
            if (((this._codec!.repetitionCount == -1L) || (completedCycles__41595 <= this._codec!.repetitionCount)))
            {
                _ = _decodeNextFrameAndSchedule();
                return;
            }
            this._codec!.dispose();
            _codec = null;
            return;
        }
        Duration delay__41889 = (DartRuntimePrimitives.RequireValue(this._frameDuration) - ((timestamp - this._shownTimestamp)));
        _timer = new Timer((delay__41889 * global::Doroti.Framework.Scheduler.BindingLibrary.timeDilation), (() =>
        {
            _scheduleAppFrame();
        }));
    }

    internal virtual bool _isFirstFrame()
    {
        return (this._frameDuration is null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasFrameDurationPassed(Duration timestamp)
    {
        return ((timestamp - this._shownTimestamp) >= DartRuntimePrimitives.RequireValue(this._frameDuration));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future _decodeNextFrameAndSchedule()
    {
        this._nextFrame?.image.dispose();
        _nextFrame = null;
        try
        {
            _nextFrame = await this._codec!.getNextFrame();
        }
        catch (Exception exception__42499)
        {
            var stack__42510 = new System.Diagnostics.StackTrace();
            reportError(context: new ErrorDescription("resolving an image frame"), exception: exception__42499, stack: stack__42510, informationCollector: (InformationCollector?)this._informationCollector, silent: true);
            return;
        }
        if ((this._codec is null))
        {
            return;
        }
        if ((this._codec!.frameCount == 1L))
        {
            if (!hasListeners)
            {
                return;
            }
            _emitFrame(new ImageInfo(image: this._nextFrame!.image.clone(), scale: this._scale, debugLabel: debugLabel));
            this._nextFrame!.image.dispose();
            _nextFrame = null;
            this._codec?.dispose();
            _codec = null;
            return;
        }
        _scheduleAppFrame();
    }

    internal virtual void _scheduleAppFrame()
    {
        if (this._frameCallbackScheduled)
        {
            return;
        }
        _frameCallbackScheduled = true;
        SchedulerBinding.instance.scheduleFrameCallback(this._handleAppFrame);
    }

    internal virtual void _emitFrame(ImageInfo imageInfo)
    {
        setImage(imageInfo);
        _framesEmitted += 1L;
    }

    public override void addListener(ImageStreamListener listener)
    {
        if (((!hasListeners && (this._codec is not null)) && (((_currentImage is null) || (this._codec!.frameCount > 1L)))))
        {
            _ = _decodeNextFrameAndSchedule();
        }
        base.addListener(listener);
    }

    public override void removeListener(ImageStreamListener listener)
    {
        base.removeListener(listener);
        if (!hasListeners)
        {
            this._timer?.cancel();
            _timer = null;
        }
    }

    internal override void _maybeDispose()
    {
        base._maybeDispose();
        if (_disposed)
        {
            this._chunkSubscription?.onData(null);
            _ = this._chunkSubscription?.cancel();
            _chunkSubscription = null;
            this._codec?.dispose();
            _codec = null;
        }
    }

}
