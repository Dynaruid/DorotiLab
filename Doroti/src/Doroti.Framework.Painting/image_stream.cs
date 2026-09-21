// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/image_stream.dart
using Doroti.Runtime;
using Doroti.Ui;

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
        return new ImageInfo(image: image.clone(), scale: scale, debugLabel: debugLabel);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool isCloneOf(ImageInfo other)
    {
        return other.image.isCloneOf(image)
            && (other.scale == scale)
            && (other.debugLabel == debugLabel);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual long sizeBytes => image.height * image.width * 4L;

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !image.debugDisposed);
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        image.dispose();
    }

    public override string ToString() =>
        $"{((debugLabel is not null) ? $"{debugLabel} " : "")}{image} @ {Foundation.DebugLibrary.debugFormatDouble(scale)}x";

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(image, scale, debugLabel);

    public override bool Equals(object? other)
    {
        var __other = other as ImageInfo;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ImageInfo)
            && Equals(__other.image, image)
            && (__other.scale == scale)
            && (__other.debugLabel == debugLabel);
    }
}

public class ImageStreamListener
{
    public virtual Action<ImageInfo, bool> onImage { get; private set; } = default!;
    public virtual Action<ImageChunkEvent>? onChunk { get; private set; }
    public virtual Action<object, System.Diagnostics.StackTrace?>? onError { get; private set; }
    public virtual bool reportErrors { get; private set; } = default!;

    public ImageStreamListener(
        Action<ImageInfo, bool> onImage,
        Action<ImageChunkEvent>? onChunk = null,
        Action<object, System.Diagnostics.StackTrace?>? onError = null,
        bool reportErrors = true
    )
    {
        this.onImage = onImage;
        this.onChunk = onChunk;
        this.onError = onError;
        this.reportErrors = reportErrors;
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(onImage, onChunk, onError, reportErrors);

    public override bool Equals(object? other)
    {
        var __other = other as ImageStreamListener;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ImageStreamListener)
            && Equals(__other.onImage, onImage)
            && Equals(__other.onChunk, onChunk)
            && Equals(__other.onError, onError)
            && (__other.reportErrors == reportErrors);
    }
}

public delegate void ImageListener(ImageInfo image, bool synchronousCall);

public delegate void ImageChunkListener(ImageChunkEvent @event);

public delegate void ImageErrorListener(
    object exception,
    System.Diagnostics.StackTrace? stackTrace
);

public class ImageChunkEvent : Diagnosticable
{
    public virtual long cumulativeBytesLoaded { get; private set; } = default!;
    public virtual long? expectedTotalBytes { get; private set; }

    public ImageChunkEvent(long cumulativeBytesLoaded, long? expectedTotalBytes)
    {
        this.cumulativeBytesLoaded = cumulativeBytesLoaded;
        this.expectedTotalBytes = expectedTotalBytes;
        System.Diagnostics.Debug.Assert(cumulativeBytesLoaded >= 0L);
        System.Diagnostics.Debug.Assert((expectedTotalBytes is null) || (expectedTotalBytes >= 0L));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("cumulativeBytesLoaded", cumulativeBytesLoaded));
        properties.add(new IntProperty("expectedTotalBytes", expectedTotalBytes));
    }
}

public class ImageStream : Diagnosticable
{
    internal virtual ImageStreamCompleter? _completer { get; set; } = default;
    internal virtual List<ImageStreamListener>? _listeners { get; set; } = default;

    public ImageStream() { }

    public virtual ImageStreamCompleter? completer => _completer;

    public virtual void setCompleter(ImageStreamCompleter value)
    {
        DartRuntimePrimitives.Assert(() => _completer is null);
        _completer = value;
        if (_listeners is not null)
        {
            List<ImageStreamListener> initialListeners = _listeners!;
            _listeners = null;
            _completer!._addingInitialListeners = true;
            initialListeners.forEach(_completer!.addListener);
            _completer!._addingInitialListeners = false;
        }
    }

    public virtual void addListener(ImageStreamListener listener)
    {
        if (_completer is not null)
        {
            _completer!.addListener(listener);
            return;
        }
        _listeners ??= new List<ImageStreamListener>();
        _listeners!.Add(listener);
    }

    public virtual void removeListener(ImageStreamListener listener)
    {
        if (_completer is not null)
        {
            _completer!.removeListener(listener);
            return;
        }
        DartRuntimePrimitives.Assert(() => _listeners is not null);
        for (var i = 0L; i < checked(_listeners!.Count); i += 1L)
        {
            if (Equals(_listeners![(int)i], listener))
            {
                _listeners!.removeAt(i);
                break;
            }
        }
    }

    public virtual object key => (object?)_completer ?? this;

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new ObjectFlagProperty<ImageStreamCompleter>(
                "completer",
                _completer,
                ifPresent: ((Diagnosticable?)_completer)?.toStringShort(),
                ifNull: "unresolved"
            )
        );
        properties.add(
            new ObjectFlagProperty<List<ImageStreamListener>>(
                "listeners",
                _listeners,
                ifPresent: $"{(long?)(_listeners?.Count)} listener{(((_listeners?.Count) == 1L) ? "" : "s")}",
                ifNull: "no listeners",
                level: (_completer is not null) ? DiagnosticLevel.hidden : DiagnosticLevel.info
            )
        );
        _completer?.debugFillProperties(properties);
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
        DartRuntimePrimitives.Assert(() => _completer is not null);
        DartRuntimePrimitives.Assert(() => _completer!._keepAliveHandles > 0L);
        DartRuntimePrimitives.Assert(() => !_completer!._disposed);
        _completer!._keepAliveHandles -= 1L;
        _completer!._maybeDispose();
        _completer = null;
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
    }
}

public abstract class ImageStreamCompleter : Diagnosticable
{
    internal virtual List<ImageStreamListener> _listeners { get; private set; } =
        new List<ImageStreamListener>();
    internal virtual List<Action<object, System.Diagnostics.StackTrace?>> _ephemeralErrorListeners
    {
        get;
        private set;
    } = new List<Action<object, System.Diagnostics.StackTrace?>>();
    internal virtual ImageInfo? _currentImage { get; set; } = default;
    internal virtual FlutterErrorDetails? _currentError { get; set; } = default;
    public virtual string? debugLabel { get; set; } = default;
    internal virtual bool _addingInitialListeners { get; set; } = false;
    internal virtual bool _hadErrorListener { get; set; } = false;
    internal virtual long _keepAliveHandles { get; set; } = 0L;
    internal virtual bool _disposed { get; set; } = false;
    internal virtual List<Action> _onLastListenerRemovedCallbacks { get; private set; } =
        new List<Action>();

    public virtual bool hasListeners => checked((long)_listeners.Count) != 0;

    public virtual void addListener(ImageStreamListener listener)
    {
        _checkDisposed();
        if (!listener.reportErrors)
        {
            _hadErrorListener = true;
        }
        _listeners.Add(listener);
        if (_currentImage is not null)
        {
            try
            {
                listener.onImage(_currentImage!.clone(), !_addingInitialListeners);
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                reportError(
                    context: new ErrorDescription("by a synchronously-called image listener"),
                    exception: exceptionLocal,
                    stack: stackLocal
                );
            }
        }
        if ((_currentError is not null) && (listener.onError is not null))
        {
            try
            {
                listener.onError!(_currentError!.exception, _currentError!.stack);
            }
            catch (Exception newException)
            {
                var newStack = new System.Diagnostics.StackTrace();
                if (!Equals(newException, _currentError!.exception))
                {
                    FlutterError.reportError(
                        new FlutterErrorDetails(
                            exception: newException,
                            library: "image resource service",
                            context: new ErrorDescription(
                                "by a synchronously-called image error listener"
                            ),
                            stack: newStack
                        )
                    );
                }
            }
        }
    }

    public virtual void addEphemeralErrorListener(
        Action<object, System.Diagnostics.StackTrace?> listener
    )
    {
        _checkDisposed();
        if (_currentError is not null)
        {
            try
            {
                listener(_currentError!.exception, _currentError!.stack);
            }
            catch (Exception newException)
            {
                var newStack = new System.Diagnostics.StackTrace();
                if (!Equals(newException, _currentError!.exception))
                {
                    FlutterError.reportError(
                        new FlutterErrorDetails(
                            exception: newException,
                            library: "image resource service",
                            context: new ErrorDescription(
                                "by a synchronously-called image error listener"
                            ),
                            stack: newStack
                        )
                    );
                }
            }
        }
        else
        {
            if (_currentImage is null)
            {
                _ephemeralErrorListeners.Add(listener);
            }
        }
    }

    public virtual ImageStreamCompleterHandle keepAlive()
    {
        _checkDisposed();
        return new ImageStreamCompleterHandle(this);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void removeListener(ImageStreamListener listener)
    {
        _checkDisposed();
        for (var i = 0L; i < checked(_listeners.Count); i += 1L)
        {
            if (Equals(_listeners[(int)i], listener))
            {
                _listeners.removeAt(i);
                break;
            }
        }
        if (checked((long)_listeners.Count) == 0)
        {
            List<Action> callbacks = _onLastListenerRemovedCallbacks.ToList();
            foreach (var callback in callbacks)
            {
                callback();
            }
            _onLastListenerRemovedCallbacks.Clear();
            _maybeDispose();
        }
    }

    public virtual void onDisposed() { }

    public virtual void maybeDispose()
    {
        _maybeDispose();
    }

    internal virtual void _maybeDispose()
    {
        if (_disposed || (checked((long)_listeners.Count) != 0) || (_keepAliveHandles != 0L))
        {
            return;
        }
        _ephemeralErrorListeners.Clear();
        _currentImage?.dispose();
        _currentImage = null;
        _disposed = true;
        onDisposed();
    }

    internal virtual void _checkDisposed()
    {
        if (_disposed)
        {
            throw new InvalidOperationException(
                "Stream has been disposed.\n"
                    + "An ImageStream is considered disposed once at least one listener has "
                    + "been added and subsequently all listeners have been removed and no "
                    + "handles are outstanding from the keepAlive method.\n"
                    + "To resolve this error, maintain at least one listener on the stream, "
                    + "or create an ImageStreamCompleterHandle from the keepAlive "
                    + "method, or create a new stream for the image."
            );
        }
    }

    public virtual void addOnLastListenerRemovedCallback(Action callback)
    {
        _checkDisposed();
        _onLastListenerRemovedCallbacks.Add(callback);
    }

    public virtual void removeOnLastListenerRemovedCallback(Action callback)
    {
        _checkDisposed();
        _onLastListenerRemovedCallbacks.Remove(callback);
    }

    public virtual void setImage(ImageInfo image)
    {
        _checkDisposed();
        _currentImage?.dispose();
        _currentImage = image;
        _ephemeralErrorListeners.Clear();
        if (checked((long)_listeners.Count) == 0)
        {
            return;
        }
        var localListeners = new List<ImageStreamListener>(_listeners);
        foreach (var listener in localListeners)
        {
            try
            {
                listener.onImage(image.clone(), false);
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                reportError(
                    context: new ErrorDescription("by an image listener"),
                    exception: exceptionLocal,
                    stack: stackLocal
                );
            }
        }
    }

    public virtual void reportError(
        DiagnosticsNode? context = null,
        object exception = default!,
        System.Diagnostics.StackTrace? stack = null,
        InformationCollector? informationCollector = null,
        bool silent = false
    )
    {
        _currentError = new FlutterErrorDetails(
            exception: exception,
            stack: stack,
            library: "image resource service",
            context: context,
            informationCollector: informationCollector,
            silent: silent
        );
        var localErrorListeners = _listeners
            .Where(listener => listener.onError is not null)
            .Select(listener => listener.onError!)
            .Concat(_ephemeralErrorListeners)
            .ToList();
        _ephemeralErrorListeners.Clear();
        var handled = false;
        foreach (var errorListener in localErrorListeners)
        {
            try
            {
                errorListener(exception, stack);
                handled = true;
            }
            catch (Exception newException)
            {
                var newStack = new System.Diagnostics.StackTrace();
                if (!Equals(newException, exception))
                {
                    FlutterError.reportError(
                        new FlutterErrorDetails(
                            context: new ErrorDescription(
                                "when reporting an error to an image listener"
                            ),
                            library: "image resource service",
                            exception: newException,
                            stack: newStack
                        )
                    );
                }
            }
        }
        if (!handled)
        {
            if (_hadErrorListener)
            {
                return;
            }
            FlutterError.reportError(_currentError!);
        }
    }

    public virtual void reportImageChunkEvent(ImageChunkEvent @event)
    {
        _checkDisposed();
        if (hasListeners)
        {
            List<Action<ImageChunkEvent>> localListeners = _listeners
                .map((listener) => listener.onChunk)
                .OfType<Action<ImageChunkEvent>>()
                .ToList();
            foreach (var listenerLocal in localListeners)
            {
                listenerLocal(@event);
            }
        }
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(
            new DiagnosticsProperty<ImageInfo>(
                "current",
                _currentImage,
                ifNull: "unresolved",
                showName: false
            )
        );
        description.add(
            new ObjectFlagProperty<List<ImageStreamListener>>(
                "listeners",
                _listeners,
                ifPresent: $"{checked((long)_listeners.Count)} listener{((checked(_listeners.Count) == 1L) ? "" : "s")}"
            )
        );
        description.add(
            new ObjectFlagProperty<List<Action<object, System.Diagnostics.StackTrace?>>>(
                "ephemeralErrorListeners",
                _ephemeralErrorListeners,
                ifPresent: $"{checked((long)_ephemeralErrorListeners.Count)} ephemeralErrorListener{((checked(_ephemeralErrorListeners.Count) == 1L) ? "" : "s")}"
            )
        );
        description.add(new FlagProperty("disposed", value: _disposed, ifTrue: "<disposed>"));
    }
}

public class OneFrameImageStreamCompleter : ImageStreamCompleter
{
    public OneFrameImageStreamCompleter(
        Future<ImageInfo> image,
        InformationCollector? informationCollector = null
    )
    {
        _ = image.then(
            setImage,
            (error, stack) =>
                reportError(
                    context: new ErrorDescription("resolving a single-frame image stream"),
                    exception: error,
                    stack: stack,
                    informationCollector: informationCollector,
                    silent: true
                )
        );
    }
}

public class MultiFrameImageStreamCompleter : ImageStreamCompleter
{
    internal virtual StreamSubscription<ImageChunkEvent>? _chunkSubscription { get; set; } =
        default;
    internal virtual Codec? _codec { get; set; } = default;
    internal virtual double _scale { get; private set; } = default!;
    internal virtual InformationCollector? _informationCollector { get; private set; }
    internal virtual FrameInfo? _nextFrame { get; set; } = default;
    internal virtual Duration _shownTimestamp { get; set; } = default!;
    internal virtual Duration? _frameDuration { get; set; } = default;
    internal virtual long _framesEmitted { get; set; } = 0L;
    internal virtual Timer? _timer { get; set; } = default;
    internal virtual bool _frameCallbackScheduled { get; set; } = false;

    public MultiFrameImageStreamCompleter(
        Future<Codec> codec,
        double scale,
        string? debugLabel = null,
        Stream<ImageChunkEvent>? chunkEvents = null,
        InformationCollector? informationCollector = null
    )
    {
        _informationCollector = informationCollector;
        _scale = scale;
        this.debugLabel = debugLabel;
        _ = ObserveCodec(codec);
        if (chunkEvents is not null)
        {
            _chunkSubscription = chunkEvents.listen(
                reportImageChunkEvent,
                (object error, System.Diagnostics.StackTrace? stack) =>
                    reportError(
                        context: new ErrorDescription("loading an image"),
                        exception: error,
                        stack: stack,
                        informationCollector: informationCollector,
                        silent: true
                    )
            );
        }
    }

    private async Task ObserveCodec(Future<Codec> codec)
    {
        try
        {
            var decoded = await codec;
            if (_disposed)
            {
                decoded.dispose();
            }
            else
            {
                _handleCodecReady(decoded);
            }
        }
        catch (Exception exception)
        {
            if (!_disposed)
            {
                reportError(
                    context: new ErrorDescription("resolving an image codec"),
                    exception: exception,
                    stack: new System.Diagnostics.StackTrace(exception),
                    informationCollector: _informationCollector,
                    silent: true
                );
            }
        }
    }

    internal virtual void _handleCodecReady(Codec codec)
    {
        _codec = codec;
        DartRuntimePrimitives.Assert(() => _codec is not null);
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
        DartRuntimePrimitives.Assert(() => _nextFrame is not null);
        if (_isFirstFrame() || _hasFrameDurationPassed(timestamp))
        {
            var frame = _nextFrame!;
            _nextFrame = null;
            _shownTimestamp = timestamp;
            _frameDuration = frame.duration;
            using (frame.image)
            {
                _emitFrame(
                    new ImageInfo(image: frame.image.clone(), scale: _scale, debugLabel: debugLabel)
                );
            }

            if (_codec is null)
            {
                return;
            }
            long completedCycles = checked(_framesEmitted / _codec!.frameCount);
            if ((_codec!.repetitionCount == -1L) || (completedCycles <= _codec!.repetitionCount))
            {
                _ = _decodeNextFrameAndSchedule();
                return;
            }
            _codec!.dispose();
            _codec = null;
            return;
        }
        Duration delay =
            (
                _frameDuration
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) - (timestamp - _shownTimestamp);
        _timer = new Timer(
            delay * Scheduler.BindingLibrary.timeDilation,
            () =>
            {
                _scheduleAppFrame();
            }
        );
    }

    internal virtual bool _isFirstFrame()
    {
        return _frameDuration is null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _hasFrameDurationPassed(Duration timestamp)
    {
        return (timestamp - _shownTimestamp)
            >= (
                _frameDuration
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual async Future _decodeNextFrameAndSchedule()
    {
        _nextFrame?.image.dispose();
        _nextFrame = null;
        try
        {
            _nextFrame = await _codec!.getNextFrame();
        }
        catch (Exception exceptionLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            reportError(
                context: new ErrorDescription("resolving an image frame"),
                exception: exceptionLocal,
                stack: stackLocal,
                informationCollector: _informationCollector,
                silent: true
            );
            return;
        }
        if (_codec is null)
        {
            _nextFrame?.image.dispose();
            _nextFrame = null;
            return;
        }
        if (_codec!.frameCount == 1L)
        {
            if (!hasListeners)
            {
                return;
            }
            var frame = _nextFrame!;
            _nextFrame = null;
            using (frame.image)
            {
                _emitFrame(
                    new ImageInfo(image: frame.image.clone(), scale: _scale, debugLabel: debugLabel)
                );
            }

            _codec?.dispose();
            _codec = null;
            return;
        }
        _scheduleAppFrame();
    }

    internal virtual void _scheduleAppFrame()
    {
        if (_frameCallbackScheduled)
        {
            return;
        }
        _frameCallbackScheduled = true;
        SchedulerBinding.instance.scheduleFrameCallback(_handleAppFrame);
    }

    internal virtual void _emitFrame(ImageInfo imageInfo)
    {
        setImage(imageInfo);
        _framesEmitted += 1L;
    }

    public override void addListener(ImageStreamListener listener)
    {
        var startDecoding =
            !hasListeners && _codec is not null && (_currentImage is null || _codec.frameCount > 1);
        base.addListener(listener);
        if (startDecoding && !_disposed && _codec is not null)
        {
            _ = _decodeNextFrameAndSchedule();
        }
    }

    public override void removeListener(ImageStreamListener listener)
    {
        base.removeListener(listener);
        if (!hasListeners)
        {
            _timer?.cancel();
            _timer = null;
        }
    }

    internal override void _maybeDispose()
    {
        base._maybeDispose();
        if (_disposed)
        {
            _chunkSubscription?.onData(null);
            _ = _chunkSubscription?.cancel();
            _chunkSubscription = null;
            _codec?.dispose();
            _codec = null;
            _nextFrame?.image.dispose();
            _nextFrame = null;
        }
    }
}
