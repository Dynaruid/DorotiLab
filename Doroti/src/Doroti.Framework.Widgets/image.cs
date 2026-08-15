// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/image.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public static partial class ImageLibrary
{
    public static global::Doroti.Generated.Framework.Painting.ImageConfiguration createLocalImageConfiguration(BuildContext context, Size? size = null)
    {
        return new global::Doroti.Generated.Framework.Painting.ImageConfiguration(bundle: DefaultAssetBundle.of(context), devicePixelRatio: (MediaQuery.maybeDevicePixelRatioOf(context) ?? 1.0), locale: Localizations.maybeLocaleOf(context), textDirection: Directionality.maybeOf(context), size: size, platform: global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class ImageLibrary
{
    public static Future precacheImage(dynamic provider, BuildContext context, Size? size = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onError = null)
    {
        global::Doroti.Generated.Framework.Painting.ImageConfiguration config__4859 = ImageLibrary.createLocalImageConfiguration(context, size: size);
        var completer__4928 = new Completer<object?>();
        global::Doroti.Generated.Framework.Painting.ImageStream stream__4979 = ((global::Doroti.Generated.Framework.Painting.ImageStream)(object?)((global::Doroti.Generated.Framework.Painting.ImageStream)((dynamic)provider).resolve(config__4859)));
        global::Doroti.Generated.Framework.Painting.ImageStreamListener? listener__5037 = default!;
        listener__5037 = new global::Doroti.Generated.Framework.Painting.ImageStreamListener(((global::System.Action<global::Doroti.Generated.Framework.Painting.ImageInfo?, bool>)((image, sync) => {
if (!completer__4928.isCompleted)
{
    completer__4928.complete();
}
global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) => {
image?.dispose();
stream__4979.removeListener(listener__5037!);
})), debugLabel: "precacheImage.removeListener");
})), onError: ((global::System.Action<object, global::System.Diagnostics.StackTrace?>)((exception, stackTrace) => {
if (!completer__4928.isCompleted)
{
    completer__4928.complete();
}
stream__4979.removeListener(listener__5037!);
if ((onError is not null))
{
    onError(exception, stackTrace);
}
else
{
    FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("image failed to precache"), library: "image resource service", exception: exception, stack: stackTrace, silent: true));
}
})));
        stream__4979.addListener(listener__5037);
        return completer__4928.future;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate Widget ImageFrameBuilder(BuildContext context, Widget child, long? frame, bool wasSynchronouslyLoaded);

public delegate Widget ImageLoadingBuilder(BuildContext context, Widget child, global::Doroti.Generated.Framework.Painting.ImageChunkEvent? loadingProgress);

public delegate Widget ImageErrorWidgetBuilder(BuildContext context, object error, global::System.Diagnostics.StackTrace? stackTrace);

public class Image : StatefulWidget
{
    public virtual dynamic image { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder { get; private set; }
    public virtual global::System.Func<BuildContext, Widget, global::Doroti.Generated.Framework.Painting.ImageChunkEvent?, Widget>? loadingBuilder { get; private set; }
    public virtual global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double>? opacity { get; private set; }
    public virtual FilterQuality filterQuality { get; private set; } = default!;
    public virtual BlendMode? colorBlendMode { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BoxFit? fit { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.ImageRepeat repeat { get; private set; } = default!;
    public virtual Rect? centerSlice { get; private set; }
    public virtual bool matchTextDirection { get; private set; } = default!;
    public virtual bool gaplessPlayback { get; private set; } = default!;
    public virtual string? semanticLabel { get; private set; }
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual bool isAntiAlias { get; private set; } = default!;

    public Image(global::Doroti.Generated.Framework.Foundation.Key? key = null, dynamic image = default!, global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null, global::System.Func<BuildContext, Widget, global::Doroti.Generated.Framework.Painting.ImageChunkEvent?, Widget>? loadingBuilder = null, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder = null, string? semanticLabel = null, bool excludeFromSemantics = false, double? width = null, double? height = null, Color? color = null, global::Doroti.Generated.Framework.Animation.Animation<double>? opacity = null, BlendMode? colorBlendMode = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.ImageRepeat repeat = global::Doroti.Generated.Framework.Painting.ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, bool gaplessPlayback = false, bool isAntiAlias = false, FilterQuality filterQuality = FilterQuality.medium) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
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

    public static Image CreateNetwork(string src, global::Doroti.Generated.Framework.Foundation.Key? key = null, double scale = 1.0, global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null, global::System.Func<BuildContext, Widget, global::Doroti.Generated.Framework.Painting.ImageChunkEvent?, Widget>? loadingBuilder = null, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder = null, string? semanticLabel = null, bool excludeFromSemantics = false, double? width = null, double? height = null, Color? color = null, global::Doroti.Generated.Framework.Animation.Animation<double>? opacity = null, BlendMode? colorBlendMode = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.ImageRepeat repeat = global::Doroti.Generated.Framework.Painting.ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, bool gaplessPlayback = false, FilterQuality filterQuality = FilterQuality.medium, bool isAntiAlias = false, DartMap<string, string>? headers = null, long? cacheWidth = null, long? cacheHeight = null, global::Doroti.Generated.Framework.Painting.WebHtmlElementStrategy webHtmlElementStrategy = global::Doroti.Generated.Framework.Painting.WebHtmlElementStrategy.never)
    {
        var __instance = new Image(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
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
        __instance.image = ResizeImage.resizeIfNeeded(cacheWidth, cacheHeight, new global::Doroti.Generated.Framework.Painting.NetworkImageIo(src, scale: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(scale)), headers: headers, webHtmlElementStrategy: webHtmlElementStrategy));
        return __instance;
    }

    public static Image CreateFile(global::Doroti.Runtime.DartFile file, global::Doroti.Generated.Framework.Foundation.Key? key = null, double scale = 1.0, global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder = null, string? semanticLabel = null, bool excludeFromSemantics = false, double? width = null, double? height = null, Color? color = null, global::Doroti.Generated.Framework.Animation.Animation<double>? opacity = null, BlendMode? colorBlendMode = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.ImageRepeat repeat = global::Doroti.Generated.Framework.Painting.ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, bool gaplessPlayback = false, bool isAntiAlias = false, FilterQuality filterQuality = FilterQuality.medium, long? cacheWidth = null, long? cacheHeight = null)
    {
        var __instance = new Image(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
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
        __instance.image = ResizeImage.resizeIfNeeded(cacheWidth, cacheHeight, new global::Doroti.Generated.Framework.Painting.FileImage(file, scale: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(scale))));
        __instance.loadingBuilder = null;
        return __instance;
    }

    public static Image CreateAsset(string name, global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Services.AssetBundle? bundle = null, global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder = null, string? semanticLabel = null, bool excludeFromSemantics = false, double? scale = null, double? width = null, double? height = null, Color? color = null, global::Doroti.Generated.Framework.Animation.Animation<double>? opacity = null, BlendMode? colorBlendMode = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.ImageRepeat repeat = global::Doroti.Generated.Framework.Painting.ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, bool gaplessPlayback = false, bool isAntiAlias = false, string? package = null, FilterQuality filterQuality = FilterQuality.medium, long? cacheWidth = null, long? cacheHeight = null)
    {
        var __instance = new Image(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
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
        __instance.image = ResizeImage.resizeIfNeeded(cacheWidth, cacheHeight, ((scale is not null) ? new global::Doroti.Generated.Framework.Painting.ExactAssetImage(name, bundle: bundle, scale: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(scale)), package: package) : new global::Doroti.Generated.Framework.Painting.AssetImage(name, bundle: bundle, package: package)));
        __instance.loadingBuilder = null;
        return __instance;
    }

    public static Image CreateMemory(Uint8List bytes, global::Doroti.Generated.Framework.Foundation.Key? key = null, double scale = 1.0, global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder = null, string? semanticLabel = null, bool excludeFromSemantics = false, double? width = null, double? height = null, Color? color = null, global::Doroti.Generated.Framework.Animation.Animation<double>? opacity = null, BlendMode? colorBlendMode = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.ImageRepeat repeat = global::Doroti.Generated.Framework.Painting.ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, bool gaplessPlayback = false, bool isAntiAlias = false, FilterQuality filterQuality = FilterQuality.medium, long? cacheWidth = null, long? cacheHeight = null)
    {
        var __instance = new Image(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
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
        __instance.image = ResizeImage.resizeIfNeeded(cacheWidth, cacheHeight, new global::Doroti.Generated.Framework.Painting.MemoryImage(bytes, scale: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(scale))));
        __instance.loadingBuilder = null;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ImageState__image());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object>("image", this.image));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<Delegate>("frameBuilder", this.frameBuilder));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<Delegate>("loadingBuilder", this.loadingBuilder));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("width", this.width, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Animation.Animation<double>?>("opacity", this.opacity, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.BlendMode>("colorBlendMode", this.colorBlendMode, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.BoxFit>("fit", this.fit, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.ImageRepeat>("repeat", this.repeat, defaultValue: global::Doroti.Generated.Framework.Painting.ImageRepeat.noRepeat));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Rect>("centerSlice", this.centerSlice, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("matchTextDirection", value: this.matchTextDirection, ifTrue: "match text direction"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.StringProperty("semanticLabel", this.semanticLabel, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("this.excludeFromSemantics", this.excludeFromSemantics));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.FilterQuality>("filterQuality", this.filterQuality));
    }

}

internal class _ImageState__image : State<Image>, WidgetsBindingObserver
{
    internal virtual global::Doroti.Generated.Framework.Painting.ImageStream? _imageStream { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.ImageInfo? _imageInfo { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.ImageChunkEvent? _loadingProgress { get; set; } = default;
    internal virtual bool _isListeningToStream { get; set; } = false;
    internal virtual bool _invertColors { get; set; } = default!;
    internal virtual long? _frameNumber { get; set; } = default;
    internal virtual bool _wasSynchronouslyLoaded { get; set; } = false;
    internal virtual DisposableBuildContext<State<Image>> _scrollAwareContext { get; set; } = default!;
    internal virtual object? _lastException { get; set; } = default;
    internal virtual global::System.Diagnostics.StackTrace? _lastStack { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.ImageStreamCompleterHandle? _completerHandle { get; set; } = default;
    internal virtual bool _isPaused { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Painting.ImageStreamListener? _imageStreamListener { get; set; } = default;

    public override void initState()
    {
        base.initState();
        WidgetsBinding.instance.addObserver(this);
        _scrollAwareContext = new DisposableBuildContext<State<Image>>(this);
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => (this._imageStream is not null));
        WidgetsBinding.instance.removeObserver(this);
        _stopListeningToStream();
        this._completerHandle?.dispose();
        this._scrollAwareContext.dispose();
        _replaceImage(info: ((global::Doroti.Generated.Framework.Painting.ImageInfo)(object)null));
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        _updateInvertColors();
        _resolveImage();
        _isPaused = (!TickerMode.of(this.context) || ((MediaQuery.maybeDisableAnimationsOf(this.context) ?? false)));
        if ((this._isPaused && (this._frameNumber is not null)))
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
        if ((this._isListeningToStream && (((((Image)this.widget).loadingBuilder is null)) != ((((Image)oldWidget).loadingBuilder is null)))))
        {
            global::Doroti.Generated.Framework.Painting.ImageStreamListener oldListener__47573 = ((global::Doroti.Generated.Framework.Painting.ImageStreamListener)(object?)_getListener());
            this._imageStream!.addListener(_getListener(recreateListener: true));
            this._imageStream!.removeListener(oldListener__47573);
        }
        if ((!object.Equals(((Image)this.widget).image, ((Image)oldWidget).image)))
        {
            _resolveImage();
            _listenToStream();
        }
    }

    public virtual void didChangeAccessibilityFeatures()
    {
        base.didChangeAccessibilityFeatures();
        setState(((global::System.Action)(() => {
_updateInvertColors();
})));
    }

    public override void reassemble()
    {
        _resolveImage();
        base.reassemble();
    }

    internal virtual void _updateInvertColors()
    {
        _invertColors = (MediaQuery.maybeInvertColorsOf(this.context) ?? global::Doroti.Generated.Framework.Semantics.SemanticsBinding.instance.accessibilityFeatures.invertColors);
    }

    internal virtual void _resolveImage()
    {
        var provider__48324 = new ScrollAwareImageProvider<object>(context: this._scrollAwareContext, imageProvider: ((Image)this.widget).image);
        global::Doroti.Generated.Framework.Painting.ImageStream newStream__48469 = ((global::Doroti.Generated.Framework.Painting.ImageStream)(object?)provider__48324.resolve(ImageLibrary.createLocalImageConfiguration(this.context, size: (((((Image)this.widget).width is not null) && (((Image)this.widget).height is not null)) ? new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(((Image)this.widget).width), DartRuntimePrimitives.RequireValue(((Image)this.widget).height)) : null))));
        _updateSourceStream(newStream__48469);
    }

    internal virtual global::Doroti.Generated.Framework.Painting.ImageStreamListener _getListener(bool recreateListener = false)
    {
        if (((this._imageStreamListener is null) || recreateListener))
        {
            _lastException = null;
            _lastStack = null;
            _imageStreamListener = new global::Doroti.Generated.Framework.Painting.ImageStreamListener((global::System.Action<global::Doroti.Generated.Framework.Painting.ImageInfo, bool>)this._handleImageFrame, onChunk: ((global::System.Action<global::Doroti.Generated.Framework.Painting.ImageChunkEvent>)((((Image)this.widget).loadingBuilder is null) ? null : this._handleImageChunk)), onError: ((global::System.Action<object, global::System.Diagnostics.StackTrace?>)(((((Image)this.widget).errorBuilder is not null) || global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode) ? ((error, stackTrace) => {
setState(((global::System.Action)(() => {
_lastException = error;
_lastStack = stackTrace;
})));
DartRuntimePrimitives.Assert(() =>
    {
        if ((((Image)this.widget).errorBuilder is null))
        {
            throw DartRuntimePrimitives.AsException(error);
        }
        return true;
        throw new InvalidOperationException("Dart closure completed without a value.");
    });
}) : null)), reportErrors: (((Image)this.widget).errorBuilder is null));
        }
        return this._imageStreamListener!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleImageFrame(global::Doroti.Generated.Framework.Painting.ImageInfo imageInfo, bool synchronousCall)
    {
        setState(((global::System.Action)(() => {
_replaceImage(info: imageInfo);
_loadingProgress = null;
_lastException = null;
_lastStack = null;
_frameNumber = ((this._frameNumber is null) ? 0L : (DartRuntimePrimitives.RequireValue(this._frameNumber) + 1L));
_wasSynchronouslyLoaded = (this._wasSynchronouslyLoaded | synchronousCall);
})));
        if (this._isPaused)
        {
            _stopListeningToStream(keepStreamAlive: true);
        }
    }

    internal virtual void _handleImageChunk(global::Doroti.Generated.Framework.Painting.ImageChunkEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (((Image)this.widget).loadingBuilder is not null));
        setState(((global::System.Action)(() => {
_loadingProgress = @event;
_lastException = null;
_lastStack = null;
})));
    }

    internal virtual void _replaceImage(global::Doroti.Generated.Framework.Painting.ImageInfo? info)
    {
        global::Doroti.Generated.Framework.Painting.ImageInfo? oldImageInfo__50641 = this._imageInfo;
        if ((oldImageInfo__50641 is not null))
        {
            global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) => { oldImageInfo__50641.dispose(); })), debugLabel: "Image.disposeOldInfo");
        }
        _imageInfo = info;
    }

    internal virtual void _updateSourceStream(global::Doroti.Generated.Framework.Painting.ImageStream newStream)
    {
        if ((object.Equals(this._imageStream?.key, ((global::Doroti.Generated.Framework.Painting.ImageStream)newStream).key)))
        {
            return;
        }
        if (this._isListeningToStream)
        {
            this._imageStream!.removeListener(_getListener());
        }
        if (!((Image)this.widget).gaplessPlayback)
        {
            setState(((global::System.Action)(() => {
_replaceImage(info: ((global::Doroti.Generated.Framework.Painting.ImageInfo)(object)null));
})));
        }
        setState(((global::System.Action)(() => {
_loadingProgress = null;
_frameNumber = null;
_wasSynchronouslyLoaded = false;
})));
        _imageStream = newStream;
        if (this._isListeningToStream)
        {
            this._imageStream!.addListener(_getListener());
        }
    }

    internal virtual void _listenToStream()
    {
        if (this._isListeningToStream)
        {
            return;
        }
        _isListeningToStream = true;
        this._imageStream!.addListener(_getListener());
        this._completerHandle?.dispose();
        _completerHandle = null;
    }

    internal virtual void _stopListeningToStream(bool keepStreamAlive = false)
    {
        if (!this._isListeningToStream)
        {
            return;
        }
        if (((keepStreamAlive && (this._completerHandle is null)) && (this._imageStream?.completer is not null)))
        {
            _completerHandle = this._imageStream!.completer!.keepAlive();
        }
        if (((this._imageStream!.completer is not null) && (((Image)this.widget).errorBuilder is not null)))
        {
            this._imageStream!.completer!.addEphemeralErrorListener(((global::System.Action<object, global::System.Diagnostics.StackTrace?>)((exception, stackTrace) => {
})));
        }
        this._imageStream!.removeListener(_getListener());
        _isListeningToStream = false;
    }

    internal virtual Widget _debugBuildErrorWidget(BuildContext context, object error)
    {
        return ((Widget)(object?)new Stack(alignment: global::Doroti.Generated.Framework.Painting.Alignment.center, children: new List<Widget> { Positioned.CreateFill(child: new Placeholder(color: new global::Doroti.Ui.Color(3482124831L))), new Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(4.0), child: new FittedBox(child: new Text($"{error}", textAlign: global::Doroti.Ui.TextAlign.center, textDirection: TextDirection.ltr, style: new global::Doroti.Generated.Framework.Painting.TextStyle(shadows: new List<global::Doroti.Ui.Shadow> { new global::Doroti.Ui.Shadow(blurRadius: 1.0) })))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        if ((this._lastException is not null))
        {
            if ((((Image)this.widget).errorBuilder is not null))
            {
                return ((Image)this.widget).errorBuilder!(context, this._lastException!, this._lastStack);
            }
            if (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode)
            {
                return ((Widget)(object?)_debugBuildErrorWidget(context, this._lastException!));
            }
        }
        Widget result__54531 = default!;
        if (this._imageInfo is WebImageInfoIo webImage__54582)
        {
            result__54531 = DartRuntimePrimitives.ConvertValue<Widget>(new RawWebImageIo(image: webImage__54582, debugImageLabel: this._imageInfo?.debugLabel, width: ((Image)this.widget).width, height: ((Image)this.widget).height, fit: ((Image)this.widget).fit, alignment: ((Image)this.widget).alignment, matchTextDirection: ((Image)this.widget).matchTextDirection));
        }
        else
        {
            result__54531 = DartRuntimePrimitives.ConvertValue<Widget>(new RawImage(image: this._imageInfo?.image, debugImageLabel: this._imageInfo?.debugLabel, width: ((Image)this.widget).width, height: ((Image)this.widget).height, scale: (this._imageInfo?.scale ?? 1.0), color: ((Image)this.widget).color, opacity: ((Image)this.widget).opacity, colorBlendMode: ((Image)this.widget).colorBlendMode, fit: ((Image)this.widget).fit, alignment: ((Image)this.widget).alignment, repeat: ((Image)this.widget).repeat, centerSlice: ((Image)this.widget).centerSlice, matchTextDirection: ((Image)this.widget).matchTextDirection, invertColors: this._invertColors, isAntiAlias: ((Image)this.widget).isAntiAlias, filterQuality: ((Image)this.widget).filterQuality));
        }
        if (!((Image)this.widget).excludeFromSemantics)
        {
            result__54531 = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(container: (((Image)this.widget).semanticLabel is not null), image: true, label: (((Image)this.widget).semanticLabel ?? ""), child: result__54531));
        }
        if ((((Image)this.widget).frameBuilder is not null))
        {
            result__54531 = ((Image)this.widget).frameBuilder!(context, result__54531, this._frameNumber, this._wasSynchronouslyLoaded);
        }
        if ((((Image)this.widget).loadingBuilder is not null))
        {
            result__54531 = ((Image)this.widget).loadingBuilder!(context, result__54531, this._loadingProgress);
        }
        return result__54531;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ImageStream>("stream", this._imageStream));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ImageInfo>("pixels", this._imageInfo));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ImageChunkEvent>("loadingProgress", this._loadingProgress));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<long>("frameNumber", this._frameNumber));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("wasSynchronouslyLoaded", this._wasSynchronouslyLoaded));
    }

}

