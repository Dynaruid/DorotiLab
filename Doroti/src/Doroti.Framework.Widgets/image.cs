// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/image.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class ImageLibrary
{
    public static global::Doroti.Framework.Painting.ImageConfiguration createLocalImageConfiguration(BuildContext context, Size? size = null)
    {
        return new global::Doroti.Framework.Painting.ImageConfiguration(bundle: DefaultAssetBundle.of(context), devicePixelRatio: MediaQuery.maybeDevicePixelRatioOf(context) ?? 1.0, locale: Localizations.maybeLocaleOf(context), textDirection: Directionality.maybeOf(context), size: size, platform: PlatformLibrary.defaultTargetPlatform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class ImageLibrary
{
    public static Future precacheImage(global::Doroti.Framework.Painting.IImageProvider provider, BuildContext context, Size? size = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onError = null)
    {
        global::Doroti.Framework.Painting.ImageConfiguration config = createLocalImageConfiguration(context, size: size);
        var completer = new Completer<object?>();
        global::Doroti.Framework.Painting.ImageStream stream = provider.resolve(config);
        global::Doroti.Framework.Painting.ImageStreamListener? listener = default!;
        listener = new global::Doroti.Framework.Painting.ImageStreamListener((image, sync) =>
        {
            if (!completer.isCompleted)
            {
                completer.complete();
            }
            Scheduler.SchedulerBinding.instance.addPostFrameCallback((timeStamp) =>
            {
                image?.dispose();
                stream.removeListener(listener!);
            }, debugLabel: "precacheImage.removeListener");
        }, onError: (exception, stackTrace) =>
        {
            if (!completer.isCompleted)
            {
                completer.complete();
            }
            stream.removeListener(listener!);
            if (onError is not null)
            {
                onError(exception, stackTrace);
            }
            else
            {
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(context: new global::Doroti.Framework.Foundation.ErrorDescription("image failed to precache"), library: "image resource service", exception: exception, stack: stackTrace, silent: true));
            }
        });
        stream.addListener(listener);
        return completer.future;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate Widget ImageFrameBuilder(BuildContext context, Widget child, long? frame, bool wasSynchronouslyLoaded);

public delegate Widget ImageLoadingBuilder(BuildContext context, Widget child, global::Doroti.Framework.Painting.ImageChunkEvent? loadingProgress);

public delegate Widget ImageErrorWidgetBuilder(BuildContext context, object error, global::System.Diagnostics.StackTrace? stackTrace);

public class Image : StatefulWidget
{
    public virtual global::Doroti.Framework.Painting.IImageProvider image { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder { get; private set; }
    public virtual global::System.Func<BuildContext, Widget, global::Doroti.Framework.Painting.ImageChunkEvent?, Widget>? loadingBuilder { get; private set; }
    public virtual global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual global::Doroti.Framework.Animation.Animation<double>? opacity { get; private set; }
    public virtual FilterQuality filterQuality { get; private set; } = default!;
    public virtual BlendMode? colorBlendMode { get; private set; }
    public virtual global::Doroti.Framework.Painting.BoxFit? fit { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.ImageRepeat repeat { get; private set; } = default!;
    public virtual Rect? centerSlice { get; private set; }
    public virtual bool matchTextDirection { get; private set; } = default!;
    public virtual bool gaplessPlayback { get; private set; } = default!;
    public virtual string? semanticLabel { get; private set; }
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual bool isAntiAlias { get; private set; } = default!;

    public Image(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.IImageProvider image = default!, global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null, global::System.Func<BuildContext, Widget, global::Doroti.Framework.Painting.ImageChunkEvent?, Widget>? loadingBuilder = null, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder = null, string? semanticLabel = null, bool excludeFromSemantics = false, double? width = null, double? height = null, Color? color = null, global::Doroti.Framework.Animation.Animation<double>? opacity = null, BlendMode? colorBlendMode = null, global::Doroti.Framework.Painting.BoxFit? fit = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Painting.ImageRepeat repeat = ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, bool gaplessPlayback = false, bool isAntiAlias = false, FilterQuality filterQuality = FilterQuality.medium) : base(key: key)
    {
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.image = image;
        this.frameBuilder = frameBuilder;
        this.loadingBuilder = loadingBuilder;
        this.errorBuilder = errorBuilder;
        this.semanticLabel = semanticLabel;
        this.excludeFromSemantics = excludeFromSemantics;
        this.width = width;
        this.height = height;
        this.color = color;
        this.opacity = opacity;
        this.colorBlendMode = colorBlendMode;
        this.fit = fit;
        this.alignment = __alignment;
        this.repeat = repeat;
        this.centerSlice = centerSlice;
        this.matchTextDirection = matchTextDirection;
        this.gaplessPlayback = gaplessPlayback;
        this.isAntiAlias = isAntiAlias;
        this.filterQuality = filterQuality;
    }

    public static Image CreateNetwork(string src, global::Doroti.Framework.Foundation.Key? key = null, double scale = 1.0, global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null, global::System.Func<BuildContext, Widget, global::Doroti.Framework.Painting.ImageChunkEvent?, Widget>? loadingBuilder = null, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder = null, string? semanticLabel = null, bool excludeFromSemantics = false, double? width = null, double? height = null, Color? color = null, global::Doroti.Framework.Animation.Animation<double>? opacity = null, BlendMode? colorBlendMode = null, global::Doroti.Framework.Painting.BoxFit? fit = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Painting.ImageRepeat repeat = ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, bool gaplessPlayback = false, FilterQuality filterQuality = FilterQuality.medium, bool isAntiAlias = false, DartMap<string, string>? headers = null, long? cacheWidth = null, long? cacheHeight = null, global::Doroti.Framework.Painting.WebHtmlElementStrategy webHtmlElementStrategy = WebHtmlElementStrategy.never)
    {
        var __instance = new Image(key, default!, frameBuilder, loadingBuilder, errorBuilder, semanticLabel, excludeFromSemantics, width, height, color, opacity, colorBlendMode, fit, alignment, repeat, centerSlice, matchTextDirection, gaplessPlayback, isAntiAlias, filterQuality);
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? Alignment.center;
        __instance.frameBuilder = frameBuilder;
        __instance.loadingBuilder = loadingBuilder;
        __instance.errorBuilder = errorBuilder;
        __instance.semanticLabel = semanticLabel;
        __instance.excludeFromSemantics = excludeFromSemantics;
        __instance.width = width;
        __instance.height = height;
        __instance.color = color;
        __instance.opacity = opacity;
        __instance.colorBlendMode = colorBlendMode;
        __instance.fit = fit;
        __instance.alignment = __alignment;
        __instance.repeat = repeat;
        __instance.centerSlice = centerSlice;
        __instance.matchTextDirection = matchTextDirection;
        __instance.gaplessPlayback = gaplessPlayback;
        __instance.filterQuality = filterQuality;
        __instance.isAntiAlias = isAntiAlias;
        __instance.image = ResizeImage.resizeIfNeeded(cacheWidth, cacheHeight, new global::Doroti.Framework.Painting.NetworkImageIo(src, scale: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(scale)), headers: headers, webHtmlElementStrategy: webHtmlElementStrategy));
        return __instance;
    }

    public static Image CreateFile(global::Doroti.Runtime.DartFile file, global::Doroti.Framework.Foundation.Key? key = null, double scale = 1.0, global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder = null, string? semanticLabel = null, bool excludeFromSemantics = false, double? width = null, double? height = null, Color? color = null, global::Doroti.Framework.Animation.Animation<double>? opacity = null, BlendMode? colorBlendMode = null, global::Doroti.Framework.Painting.BoxFit? fit = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Painting.ImageRepeat repeat = ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, bool gaplessPlayback = false, bool isAntiAlias = false, FilterQuality filterQuality = FilterQuality.medium, long? cacheWidth = null, long? cacheHeight = null)
    {
        var __instance = new Image(key, default!, frameBuilder, default!, errorBuilder, semanticLabel, excludeFromSemantics, width, height, color, opacity, colorBlendMode, fit, alignment, repeat, centerSlice, matchTextDirection, gaplessPlayback, isAntiAlias, filterQuality);
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? Alignment.center;
        __instance.frameBuilder = frameBuilder;
        __instance.errorBuilder = errorBuilder;
        __instance.semanticLabel = semanticLabel;
        __instance.excludeFromSemantics = excludeFromSemantics;
        __instance.width = width;
        __instance.height = height;
        __instance.color = color;
        __instance.opacity = opacity;
        __instance.colorBlendMode = colorBlendMode;
        __instance.fit = fit;
        __instance.alignment = __alignment;
        __instance.repeat = repeat;
        __instance.centerSlice = centerSlice;
        __instance.matchTextDirection = matchTextDirection;
        __instance.gaplessPlayback = gaplessPlayback;
        __instance.isAntiAlias = isAntiAlias;
        __instance.filterQuality = filterQuality;
        __instance.image = ResizeImage.resizeIfNeeded(cacheWidth, cacheHeight, new global::Doroti.Framework.Painting.FileImage(file, scale: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(scale))));
        __instance.loadingBuilder = null;
        return __instance;
    }

    public static Image CreateAsset(string name, global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Services.AssetBundle? bundle = null, global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder = null, string? semanticLabel = null, bool excludeFromSemantics = false, double? scale = null, double? width = null, double? height = null, Color? color = null, global::Doroti.Framework.Animation.Animation<double>? opacity = null, BlendMode? colorBlendMode = null, global::Doroti.Framework.Painting.BoxFit? fit = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Painting.ImageRepeat repeat = ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, bool gaplessPlayback = false, bool isAntiAlias = false, string? package = null, FilterQuality filterQuality = FilterQuality.medium, long? cacheWidth = null, long? cacheHeight = null)
    {
        var __instance = new Image(key, default!, frameBuilder, default!, errorBuilder, semanticLabel, excludeFromSemantics, width, height, color, opacity, colorBlendMode, fit, alignment, repeat, centerSlice, matchTextDirection, gaplessPlayback, isAntiAlias, filterQuality);
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? Alignment.center;
        __instance.frameBuilder = frameBuilder;
        __instance.errorBuilder = errorBuilder;
        __instance.semanticLabel = semanticLabel;
        __instance.excludeFromSemantics = excludeFromSemantics;
        __instance.width = width;
        __instance.height = height;
        __instance.color = color;
        __instance.opacity = opacity;
        __instance.colorBlendMode = colorBlendMode;
        __instance.fit = fit;
        __instance.alignment = __alignment;
        __instance.repeat = repeat;
        __instance.centerSlice = centerSlice;
        __instance.matchTextDirection = matchTextDirection;
        __instance.gaplessPlayback = gaplessPlayback;
        __instance.isAntiAlias = isAntiAlias;
        __instance.filterQuality = filterQuality;
        __instance.image = ResizeImage.resizeIfNeeded(cacheWidth, cacheHeight, (scale is not null) ? new global::Doroti.Framework.Painting.ExactAssetImage(name, bundle: bundle, scale: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(scale)), package: package) : new global::Doroti.Framework.Painting.AssetImage(name, bundle: bundle, package: package));
        __instance.loadingBuilder = null;
        return __instance;
    }

    public static Image CreateMemory(Uint8List bytes, global::Doroti.Framework.Foundation.Key? key = null, double scale = 1.0, global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder = null, string? semanticLabel = null, bool excludeFromSemantics = false, double? width = null, double? height = null, Color? color = null, global::Doroti.Framework.Animation.Animation<double>? opacity = null, BlendMode? colorBlendMode = null, global::Doroti.Framework.Painting.BoxFit? fit = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Painting.ImageRepeat repeat = ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, bool gaplessPlayback = false, bool isAntiAlias = false, FilterQuality filterQuality = FilterQuality.medium, long? cacheWidth = null, long? cacheHeight = null)
    {
        var __instance = new Image(key, default!, frameBuilder, default!, errorBuilder, semanticLabel, excludeFromSemantics, width, height, color, opacity, colorBlendMode, fit, alignment, repeat, centerSlice, matchTextDirection, gaplessPlayback, isAntiAlias, filterQuality);
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? Alignment.center;
        __instance.frameBuilder = frameBuilder;
        __instance.errorBuilder = errorBuilder;
        __instance.semanticLabel = semanticLabel;
        __instance.excludeFromSemantics = excludeFromSemantics;
        __instance.width = width;
        __instance.height = height;
        __instance.color = color;
        __instance.opacity = opacity;
        __instance.colorBlendMode = colorBlendMode;
        __instance.fit = fit;
        __instance.alignment = __alignment;
        __instance.repeat = repeat;
        __instance.centerSlice = centerSlice;
        __instance.matchTextDirection = matchTextDirection;
        __instance.gaplessPlayback = gaplessPlayback;
        __instance.isAntiAlias = isAntiAlias;
        __instance.filterQuality = filterQuality;
        __instance.image = ResizeImage.resizeIfNeeded(cacheWidth, cacheHeight, new global::Doroti.Framework.Painting.MemoryImage(bytes, scale: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(scale))));
        __instance.loadingBuilder = null;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ImageState__image());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<object>("image", image));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Delegate>("frameBuilder", frameBuilder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Delegate>("loadingBuilder", loadingBuilder));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("width", width, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("height", height, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Animation.Animation<double>?>("opacity", opacity, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.BlendMode>("colorBlendMode", colorBlendMode, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.BoxFit>("fit", fit, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", alignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.ImageRepeat>("repeat", repeat, defaultValue: ImageRepeat.noRepeat));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Rect>("centerSlice", centerSlice, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("matchTextDirection", value: matchTextDirection, ifTrue: "match text direction"));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("semanticLabel", semanticLabel, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("this.excludeFromSemantics", excludeFromSemantics));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.FilterQuality>("filterQuality", filterQuality));
    }

}

internal class _ImageState__image : State<Image>, WidgetsBindingObserver
{
    internal virtual global::Doroti.Framework.Painting.ImageStream? _imageStream { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.ImageInfo? _imageInfo { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.ImageChunkEvent? _loadingProgress { get; set; } = default;
    internal virtual bool _isListeningToStream { get; set; } = false;
    internal virtual bool _invertColors { get; set; } = default!;
    internal virtual long? _frameNumber { get; set; } = default;
    internal virtual bool _wasSynchronouslyLoaded { get; set; } = false;
    internal virtual DisposableBuildContext<State<Image>> _scrollAwareContext { get; set; } = default!;
    internal virtual object? _lastException { get; set; } = default;
    internal virtual global::System.Diagnostics.StackTrace? _lastStack { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.ImageStreamCompleterHandle? _completerHandle { get; set; } = default;
    internal virtual bool _isPaused { get; set; } = false;
    internal virtual global::Doroti.Framework.Painting.ImageStreamListener? _imageStreamListener { get; set; } = default;

    public override void initState()
    {
        base.initState();
        WidgetsBinding.instance.addObserver(this);
        _scrollAwareContext = new DisposableBuildContext<State<Image>>(this);
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => _imageStream is not null);
        WidgetsBinding.instance.removeObserver(this);
        _stopListeningToStream();
        _completerHandle?.dispose();
        _scrollAwareContext.dispose();
        _replaceImage(info: null);
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        _updateInvertColors();
        _resolveImage();
        _isPaused = !TickerMode.of(context) || (MediaQuery.maybeDisableAnimationsOf(context) ?? false);
        if (_isPaused && (_frameNumber is not null))
        {
            _stopListeningToStream(keepStreamAlive: true);
        }
        else
        {
            _listenToStream();
        }
        base.didChangeDependencies();
    }

    public override void didUpdateWidget(Image oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (_isListeningToStream && (widget.loadingBuilder is null != oldWidget.loadingBuilder is null))
        {
            global::Doroti.Framework.Painting.ImageStreamListener oldListener = _getListener();
            _imageStream!.addListener(_getListener(recreateListener: true));
            _imageStream!.removeListener(oldListener);
        }
        if (!Equals(widget.image, oldWidget.image))
        {
            _resolveImage();
            _listenToStream();
        }
    }

    public override void didChangeAccessibilityFeatures()
    {
        base.didChangeAccessibilityFeatures();
        setState(() =>
        {
            _updateInvertColors();
        });
    }

    public override void reassemble()
    {
        _resolveImage();
        base.reassemble();
    }

    internal virtual void _updateInvertColors()
    {
        _invertColors = MediaQuery.maybeInvertColorsOf(context) ?? Framework.Semantics.SemanticsBinding.instance.accessibilityFeatures.invertColors;
    }

    private ScrollAwareImageProvider CreateScrollAwareProvider(global::Doroti.Framework.Painting.IImageProvider imageProvider) =>
        new(context: _scrollAwareContext, imageProvider: imageProvider);

    internal virtual void _resolveImage()
    {
        global::Doroti.Framework.Painting.IImageProvider provider = CreateScrollAwareProvider(widget.image);
        global::Doroti.Framework.Painting.ImageStream newStream = provider.resolve(ImageLibrary.createLocalImageConfiguration(context, size: ((widget.width is not null) && (widget.height is not null)) ? new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(widget.width), DartRuntimePrimitives.RequireValue(widget.height)) : null));
        _updateSourceStream(newStream);
    }

    internal virtual global::Doroti.Framework.Painting.ImageStreamListener _getListener(bool recreateListener = false)
    {
        if ((_imageStreamListener is null) || recreateListener)
        {
            _lastException = null;
            _lastStack = null;
            _imageStreamListener = new global::Doroti.Framework.Painting.ImageStreamListener(_handleImageFrame, onChunk: (widget.loadingBuilder is null) ? null : _handleImageChunk, onError: ((widget.errorBuilder is not null) || Foundation.ConstantsLibrary.kDebugMode) ? ((error, stackTrace) =>
            {
                setState(() =>
                {
                    _lastException = error;
                    _lastStack = stackTrace;
                });
                DartRuntimePrimitives.Assert(() =>
                    {
                        if (widget.errorBuilder is null)
                        {
                            throw DartRuntimePrimitives.AsException(error);
                        }
                        return true;
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
            }) : null, reportErrors: widget.errorBuilder is null);
        }
        return _imageStreamListener!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleImageFrame(global::Doroti.Framework.Painting.ImageInfo imageInfo, bool synchronousCall)
    {
        setState(() =>
        {
            _replaceImage(info: imageInfo);
            _loadingProgress = null;
            _lastException = null;
            _lastStack = null;
            _frameNumber = (_frameNumber is null) ? 0L : (DartRuntimePrimitives.RequireValue(_frameNumber) + 1L);
            _wasSynchronouslyLoaded = _wasSynchronouslyLoaded | synchronousCall;
        });
        if (_isPaused)
        {
            _stopListeningToStream(keepStreamAlive: true);
        }
    }

    internal virtual void _handleImageChunk(global::Doroti.Framework.Painting.ImageChunkEvent @event)
    {
        DartRuntimePrimitives.Assert(() => widget.loadingBuilder is not null);
        setState(() =>
        {
            _loadingProgress = @event;
            _lastException = null;
            _lastStack = null;
        });
    }

    internal virtual void _replaceImage(global::Doroti.Framework.Painting.ImageInfo? info)
    {
        global::Doroti.Framework.Painting.ImageInfo? oldImageInfo = _imageInfo;
        if (oldImageInfo is not null)
        {
            Scheduler.SchedulerBinding.instance.addPostFrameCallback((duration) => { oldImageInfo.dispose(); }, debugLabel: "Image.disposeOldInfo");
        }
        _imageInfo = info;
    }

    internal virtual void _updateSourceStream(global::Doroti.Framework.Painting.ImageStream newStream)
    {
        if (Equals(_imageStream?.key, newStream.key))
        {
            return;
        }
        if (_isListeningToStream)
        {
            _imageStream!.removeListener(_getListener());
        }
        if (!widget.gaplessPlayback)
        {
            setState(() =>
            {
                _replaceImage(info: null);
            });
        }
        setState(() =>
        {
            _loadingProgress = null;
            _frameNumber = null;
            _wasSynchronouslyLoaded = false;
        });
        _imageStream = newStream;
        if (_isListeningToStream)
        {
            _imageStream!.addListener(_getListener());
        }
    }

    internal virtual void _listenToStream()
    {
        if (_isListeningToStream)
        {
            return;
        }
        _isListeningToStream = true;
        _imageStream!.addListener(_getListener());
        _completerHandle?.dispose();
        _completerHandle = null;
    }

    internal virtual void _stopListeningToStream(bool keepStreamAlive = false)
    {
        if (!_isListeningToStream)
        {
            return;
        }
        if (keepStreamAlive && (_completerHandle is null) && (_imageStream?.completer is not null))
        {
            _completerHandle = _imageStream!.completer!.keepAlive();
        }
        if ((_imageStream!.completer is not null) && (widget.errorBuilder is not null))
        {
            _imageStream!.completer!.addEphemeralErrorListener((exception, stackTrace) =>
            {
            });
        }
        _imageStream!.removeListener(_getListener());
        _isListeningToStream = false;
    }

    internal virtual Widget _debugBuildErrorWidget(BuildContext context, object error)
    {
        return new Stack(alignment: Alignment.center, children: new List<Widget> { Positioned.CreateFill(child: new Placeholder(color: new global::Doroti.Ui.Color(3482124831L))), new Padding(padding: EdgeInsets.CreateAll(4.0), child: new FittedBox(child: new Text($"{error}", textAlign: TextAlign.center, textDirection: TextDirection.ltr, style: new global::Doroti.Framework.Painting.TextStyle(shadows: new List<global::Doroti.Ui.Shadow> { new global::Doroti.Ui.Shadow(blurRadius: 1.0) })))) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        if (_lastException is not null)
        {
            if (widget.errorBuilder is not null)
            {
                return widget.errorBuilder!(context, _lastException!, _lastStack);
            }
            if (Foundation.ConstantsLibrary.kDebugMode)
            {
                return _debugBuildErrorWidget(context, _lastException!);
            }
        }
        Widget result = default!;
        if (_imageInfo is WebImageInfoIo webImage)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new RawWebImageIo(image: webImage, debugImageLabel: _imageInfo?.debugLabel, width: widget.width, height: widget.height, fit: widget.fit, alignment: widget.alignment, matchTextDirection: widget.matchTextDirection));
        }
        else
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new RawImage(image: _imageInfo?.image, debugImageLabel: _imageInfo?.debugLabel, width: widget.width, height: widget.height, scale: _imageInfo?.scale ?? 1.0, color: widget.color, opacity: widget.opacity, colorBlendMode: widget.colorBlendMode, fit: widget.fit, alignment: widget.alignment, repeat: widget.repeat, centerSlice: widget.centerSlice, matchTextDirection: widget.matchTextDirection, invertColors: _invertColors, isAntiAlias: widget.isAntiAlias, filterQuality: widget.filterQuality));
        }
        if (!widget.excludeFromSemantics)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(container: widget.semanticLabel is not null, image: true, label: widget.semanticLabel ?? "", child: result));
        }
        if (widget.frameBuilder is not null)
        {
            result = widget.frameBuilder!(context, result, _frameNumber, _wasSynchronouslyLoaded);
        }
        if (widget.loadingBuilder is not null)
        {
            result = widget.loadingBuilder!(context, result, _loadingProgress);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ImageStream>("stream", _imageStream));
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ImageInfo>("pixels", _imageInfo));
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ImageChunkEvent>("loadingProgress", _loadingProgress));
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<long>("frameNumber", _frameNumber));
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("wasSynchronouslyLoaded", _wasSynchronouslyLoaded));
    }

}

