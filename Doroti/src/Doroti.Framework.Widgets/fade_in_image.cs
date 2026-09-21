// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/fade_in_image.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class FadeInImage : StatefulWidget
{
    public virtual IImageProvider placeholder { get; private set; } = default!;
    public virtual Func<
        BuildContext,
        object,
        System.Diagnostics.StackTrace?,
        Widget
    >? placeholderErrorBuilder { get; private set; }
    public virtual IImageProvider image { get; private set; } = default!;
    public virtual Func<
        BuildContext,
        object,
        System.Diagnostics.StackTrace?,
        Widget
    >? imageErrorBuilder { get; private set; }
    public virtual Duration fadeOutDuration { get; private set; } = default!;
    public virtual Curve fadeOutCurve { get; private set; } = default!;
    public virtual Duration fadeInDuration { get; private set; } = default!;
    public virtual Curve fadeInCurve { get; private set; } = default!;
    public virtual double? width { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual BlendMode? colorBlendMode { get; private set; }
    public virtual Color? placeholderColor { get; private set; }
    public virtual BlendMode? placeholderColorBlendMode { get; private set; }
    public virtual double? height { get; private set; }
    public virtual BoxFit? fit { get; private set; }
    public virtual BoxFit? placeholderFit { get; private set; }
    public virtual FilterQuality filterQuality { get; private set; } = default!;
    public virtual FilterQuality? placeholderFilterQuality { get; private set; }
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual ImageRepeat repeat { get; private set; } = default!;
    public virtual bool matchTextDirection { get; private set; } = default!;
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual string? imageSemanticLabel { get; private set; }

    public FadeInImage(
        Key? key = null,
        IImageProvider placeholder = default!,
        Func<
            BuildContext,
            object,
            System.Diagnostics.StackTrace?,
            Widget
        >? placeholderErrorBuilder = null,
        IImageProvider image = default!,
        Func<BuildContext, object, System.Diagnostics.StackTrace?, Widget>? imageErrorBuilder =
            null,
        bool excludeFromSemantics = false,
        string? imageSemanticLabel = null,
        Duration? fadeOutDuration = null,
        Curve fadeOutCurve = default!,
        Duration? fadeInDuration = null,
        Curve fadeInCurve = default!,
        Color? color = null,
        BlendMode? colorBlendMode = null,
        Color? placeholderColor = null,
        BlendMode? placeholderColorBlendMode = null,
        double? width = null,
        double? height = null,
        BoxFit? fit = null,
        BoxFit? placeholderFit = null,
        FilterQuality filterQuality = FilterQuality.medium,
        FilterQuality? placeholderFilterQuality = null,
        AlignmentGeometry alignment = default!,
        ImageRepeat repeat = ImageRepeat.noRepeat,
        bool matchTextDirection = false
    )
        : base(key: key)
    {
        Duration __fadeOutDuration = fadeOutDuration ?? Duration.Create(milliseconds: 300);
        Curve __fadeOutCurve = fadeOutCurve ?? Curves.easeOut;
        Duration __fadeInDuration = fadeInDuration ?? Duration.Create(milliseconds: 700);
        Curve __fadeInCurve = fadeInCurve ?? Curves.easeIn;
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.placeholder = placeholder;
        this.placeholderErrorBuilder = placeholderErrorBuilder;
        this.image = image;
        this.imageErrorBuilder = imageErrorBuilder;
        this.excludeFromSemantics = excludeFromSemantics;
        this.imageSemanticLabel = imageSemanticLabel;
        this.fadeOutDuration = __fadeOutDuration;
        this.fadeOutCurve = __fadeOutCurve;
        this.fadeInDuration = __fadeInDuration;
        this.fadeInCurve = __fadeInCurve;
        this.color = color;
        this.colorBlendMode = colorBlendMode;
        this.placeholderColor = placeholderColor;
        this.placeholderColorBlendMode = placeholderColorBlendMode;
        this.width = width;
        this.height = height;
        this.fit = fit;
        this.placeholderFit = placeholderFit;
        this.filterQuality = filterQuality;
        this.placeholderFilterQuality = placeholderFilterQuality;
        this.alignment = __alignment;
        this.repeat = repeat;
        this.matchTextDirection = matchTextDirection;
    }

    public static FadeInImage CreateMemoryNetwork(
        Key? key = null,
        Uint8List placeholder = default!,
        Func<
            BuildContext,
            object,
            System.Diagnostics.StackTrace?,
            Widget
        >? placeholderErrorBuilder = null,
        string image = default!,
        Func<BuildContext, object, System.Diagnostics.StackTrace?, Widget>? imageErrorBuilder =
            null,
        double placeholderScale = 1.0,
        double imageScale = 1.0,
        bool excludeFromSemantics = false,
        string? imageSemanticLabel = null,
        Duration? fadeOutDuration = null,
        Curve fadeOutCurve = default!,
        Duration? fadeInDuration = null,
        Curve fadeInCurve = default!,
        double? width = null,
        double? height = null,
        BoxFit? fit = null,
        Color? color = null,
        BlendMode? colorBlendMode = null,
        Color? placeholderColor = null,
        BlendMode? placeholderColorBlendMode = null,
        BoxFit? placeholderFit = null,
        FilterQuality filterQuality = FilterQuality.medium,
        FilterQuality? placeholderFilterQuality = null,
        AlignmentGeometry alignment = default!,
        ImageRepeat repeat = ImageRepeat.noRepeat,
        bool matchTextDirection = false,
        long? placeholderCacheWidth = null,
        long? placeholderCacheHeight = null,
        long? imageCacheWidth = null,
        long? imageCacheHeight = null
    )
    {
        var __instance = new FadeInImage(
            key,
            ResizeImage.resizeIfNeeded(
                placeholderCacheWidth,
                placeholderCacheHeight,
                new MemoryImage(placeholder, scale: ((placeholderScale)))
            ),
            placeholderErrorBuilder,
            ResizeImage.resizeIfNeeded(
                imageCacheWidth,
                imageCacheHeight,
                new NetworkImageIo(image, scale: imageScale)
            ),
            imageErrorBuilder,
            excludeFromSemantics,
            imageSemanticLabel,
            fadeOutDuration,
            fadeOutCurve,
            fadeInDuration,
            fadeInCurve,
            color,
            colorBlendMode,
            placeholderColor,
            placeholderColorBlendMode,
            width,
            height,
            fit,
            placeholderFit,
            filterQuality,
            placeholderFilterQuality,
            alignment,
            repeat,
            matchTextDirection
        );
        Duration __fadeOutDuration = fadeOutDuration ?? Duration.Create(milliseconds: 300);
        Curve __fadeOutCurve = fadeOutCurve ?? Curves.easeOut;
        Duration __fadeInDuration = fadeInDuration ?? Duration.Create(milliseconds: 700);
        Curve __fadeInCurve = fadeInCurve ?? Curves.easeIn;
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        __instance.placeholderErrorBuilder = placeholderErrorBuilder;
        __instance.imageErrorBuilder = imageErrorBuilder;
        __instance.excludeFromSemantics = excludeFromSemantics;
        __instance.imageSemanticLabel = imageSemanticLabel;
        __instance.fadeOutDuration = __fadeOutDuration;
        __instance.fadeOutCurve = __fadeOutCurve;
        __instance.fadeInDuration = __fadeInDuration;
        __instance.fadeInCurve = __fadeInCurve;
        __instance.width = width;
        __instance.height = height;
        __instance.fit = fit;
        __instance.color = color;
        __instance.colorBlendMode = colorBlendMode;
        __instance.placeholderColor = placeholderColor;
        __instance.placeholderColorBlendMode = placeholderColorBlendMode;
        __instance.placeholderFit = placeholderFit;
        __instance.filterQuality = filterQuality;
        __instance.placeholderFilterQuality = placeholderFilterQuality;
        __instance.alignment = __alignment;
        __instance.repeat = repeat;
        __instance.matchTextDirection = matchTextDirection;
        return __instance;
    }

    public static FadeInImage CreateAssetNetwork(
        Key? key = null,
        string placeholder = default!,
        Func<
            BuildContext,
            object,
            System.Diagnostics.StackTrace?,
            Widget
        >? placeholderErrorBuilder = null,
        string image = default!,
        Func<BuildContext, object, System.Diagnostics.StackTrace?, Widget>? imageErrorBuilder =
            null,
        AssetBundle? bundle = null,
        double? placeholderScale = null,
        double imageScale = 1.0,
        bool excludeFromSemantics = false,
        string? imageSemanticLabel = null,
        Duration? fadeOutDuration = null,
        Curve fadeOutCurve = default!,
        Duration? fadeInDuration = null,
        Curve fadeInCurve = default!,
        double? width = null,
        double? height = null,
        BoxFit? fit = null,
        Color? color = null,
        BlendMode? colorBlendMode = null,
        Color? placeholderColor = null,
        BlendMode? placeholderColorBlendMode = null,
        BoxFit? placeholderFit = null,
        FilterQuality filterQuality = FilterQuality.medium,
        FilterQuality? placeholderFilterQuality = null,
        AlignmentGeometry alignment = default!,
        ImageRepeat repeat = ImageRepeat.noRepeat,
        bool matchTextDirection = false,
        long? placeholderCacheWidth = null,
        long? placeholderCacheHeight = null,
        long? imageCacheWidth = null,
        long? imageCacheHeight = null
    )
    {
        var __instance = new FadeInImage(
            key,
            (placeholderScale is not null)
                ? ResizeImage.resizeIfNeeded(
                    placeholderCacheWidth,
                    placeholderCacheHeight,
                    new ExactAssetImage(
                        placeholder,
                        bundle: bundle,
                        scale: (
                            (
                                placeholderScale
                                ?? throw new global::System.NullReferenceException(
                                    "Dart null assertion failed."
                                )
                            )
                        )
                    )
                )
                : ResizeImage.resizeIfNeeded(
                    placeholderCacheWidth,
                    placeholderCacheHeight,
                    new AssetImage(placeholder, bundle: bundle)
                ),
            placeholderErrorBuilder,
            ResizeImage.resizeIfNeeded(
                imageCacheWidth,
                imageCacheHeight,
                new NetworkImageIo(image, scale: imageScale)
            ),
            imageErrorBuilder,
            excludeFromSemantics,
            imageSemanticLabel,
            fadeOutDuration,
            fadeOutCurve,
            fadeInDuration,
            fadeInCurve,
            color,
            colorBlendMode,
            placeholderColor,
            placeholderColorBlendMode,
            width,
            height,
            fit,
            placeholderFit,
            filterQuality,
            placeholderFilterQuality,
            alignment,
            repeat,
            matchTextDirection
        );
        Duration __fadeOutDuration = fadeOutDuration ?? Duration.Create(milliseconds: 300);
        Curve __fadeOutCurve = fadeOutCurve ?? Curves.easeOut;
        Duration __fadeInDuration = fadeInDuration ?? Duration.Create(milliseconds: 700);
        Curve __fadeInCurve = fadeInCurve ?? Curves.easeIn;
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        __instance.placeholderErrorBuilder = placeholderErrorBuilder;
        __instance.imageErrorBuilder = imageErrorBuilder;
        __instance.excludeFromSemantics = excludeFromSemantics;
        __instance.imageSemanticLabel = imageSemanticLabel;
        __instance.fadeOutDuration = __fadeOutDuration;
        __instance.fadeOutCurve = __fadeOutCurve;
        __instance.fadeInDuration = __fadeInDuration;
        __instance.fadeInCurve = __fadeInCurve;
        __instance.width = width;
        __instance.height = height;
        __instance.fit = fit;
        __instance.color = color;
        __instance.colorBlendMode = colorBlendMode;
        __instance.placeholderColor = placeholderColor;
        __instance.placeholderColorBlendMode = placeholderColorBlendMode;
        __instance.placeholderFit = placeholderFit;
        __instance.filterQuality = filterQuality;
        __instance.placeholderFilterQuality = placeholderFilterQuality;
        __instance.alignment = __alignment;
        __instance.repeat = repeat;
        __instance.matchTextDirection = matchTextDirection;
        return __instance;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _FadeInImageState__fade_in_image());
}

internal class _FadeInImageState__fade_in_image : State<FadeInImage>
{
    internal static Animation<double> _kOpaqueAnimation = new AlwaysStoppedAnimation<double>(1.0);
    public virtual bool targetLoaded { get; set; } = false;
    internal virtual ProxyAnimation _imageAnimation { get; private set; } =
        new ProxyAnimation(_kOpaqueAnimation);
    internal virtual ProxyAnimation _placeholderAnimation { get; private set; } =
        new ProxyAnimation(_kOpaqueAnimation);

    internal virtual Image _image(
        IImageProvider image,
        Func<BuildContext, object, System.Diagnostics.StackTrace?, Widget>? errorBuilder = null,
        Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null,
        BoxFit? fit = null,
        Color? color = null,
        BlendMode? colorBlendMode = null,
        FilterQuality filterQuality = default!,
        Animation<double> opacity = default!
    )
    {
        return new Image(
            image: image,
            errorBuilder: errorBuilder,
            frameBuilder: frameBuilder,
            opacity: opacity,
            width: widget.width,
            height: widget.height,
            fit: fit,
            color: color,
            colorBlendMode: colorBlendMode,
            filterQuality: filterQuality,
            alignment: widget.alignment,
            repeat: widget.repeat,
            matchTextDirection: widget.matchTextDirection,
            gaplessPlayback: true,
            excludeFromSemantics: true
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        Widget result = _image(
            image: widget.image,
            errorBuilder: widget.imageErrorBuilder,
            opacity: _imageAnimation,
            fit: widget.fit,
            color: widget.color,
            colorBlendMode: widget.colorBlendMode,
            filterQuality: widget.filterQuality,
            frameBuilder: (context, child, frame, wasSynchronouslyLoaded) =>
            {
                if (wasSynchronouslyLoaded || (frame is not null))
                {
                    targetLoaded = true;
                }
                return new _AnimatedFadeOutFadeIn__fade_in_image(
                    target: child,
                    targetProxyAnimation: _imageAnimation,
                    placeholder: _image(
                        image: widget.placeholder,
                        errorBuilder: widget.placeholderErrorBuilder,
                        opacity: _placeholderAnimation,
                        color: widget.placeholderColor,
                        colorBlendMode: widget.placeholderColorBlendMode,
                        fit: widget.placeholderFit ?? widget.fit,
                        filterQuality: widget.placeholderFilterQuality ?? widget.filterQuality
                    ),
                    placeholderProxyAnimation: _placeholderAnimation,
                    isTargetLoaded: targetLoaded,
                    wasSynchronouslyLoaded: wasSynchronouslyLoaded,
                    fadeInDuration: widget.fadeInDuration,
                    fadeOutDuration: widget.fadeOutDuration,
                    fadeInCurve: widget.fadeInCurve,
                    fadeOutCurve: widget.fadeOutCurve
                );
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        if (!widget.excludeFromSemantics)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(
                new Semantics(
                    container: widget.imageSemanticLabel is not null,
                    image: true,
                    label: widget.imageSemanticLabel ?? "",
                    child: result
                )
            );
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _AnimatedFadeOutFadeIn__fade_in_image : ImplicitlyAnimatedWidget
{
    public virtual Widget target { get; private set; } = default!;
    public virtual ProxyAnimation targetProxyAnimation { get; private set; } = default!;
    public virtual Widget placeholder { get; private set; } = default!;
    public virtual ProxyAnimation placeholderProxyAnimation { get; private set; } = default!;
    public virtual bool isTargetLoaded { get; private set; } = default!;
    public virtual Duration fadeInDuration { get; private set; } = default!;
    public virtual Duration fadeOutDuration { get; private set; } = default!;
    public virtual Curve fadeInCurve { get; private set; } = default!;
    public virtual Curve fadeOutCurve { get; private set; } = default!;
    public virtual bool wasSynchronouslyLoaded { get; private set; } = default!;

    internal _AnimatedFadeOutFadeIn__fade_in_image(
        Widget target,
        ProxyAnimation targetProxyAnimation,
        Widget placeholder,
        ProxyAnimation placeholderProxyAnimation,
        bool isTargetLoaded,
        Duration fadeOutDuration,
        Curve fadeOutCurve,
        Duration fadeInDuration,
        Curve fadeInCurve,
        bool wasSynchronouslyLoaded
    )
        : base(duration: fadeInDuration + fadeOutDuration)
    {
        this.target = target;
        this.targetProxyAnimation = targetProxyAnimation;
        this.placeholder = placeholder;
        this.placeholderProxyAnimation = placeholderProxyAnimation;
        this.isTargetLoaded = isTargetLoaded;
        this.fadeOutDuration = fadeOutDuration;
        this.fadeOutCurve = fadeOutCurve;
        this.fadeInDuration = fadeInDuration;
        this.fadeInCurve = fadeInCurve;
        this.wasSynchronouslyLoaded = wasSynchronouslyLoaded;
        System.Diagnostics.Debug.Assert(!wasSynchronouslyLoaded || isTargetLoaded);
    }

    public override _AnimatedFadeOutFadeInState__fade_in_image createState() =>
        new _AnimatedFadeOutFadeInState__fade_in_image();
}

internal class _AnimatedFadeOutFadeInState__fade_in_image
    : ImplicitlyAnimatedWidgetState<_AnimatedFadeOutFadeIn__fade_in_image>
{
    internal virtual Tween<double>? _targetOpacity { get; set; } = default;
    internal virtual Tween<double>? _placeholderOpacity { get; set; } = default;
    internal virtual Animation<double>? _targetOpacityAnimation { get; set; } = default;
    internal virtual Animation<double>? _placeholderOpacityAnimation { get; set; } = default;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _targetOpacity = (
            (Tween<double>?)visitor(
                _targetOpacity,
                widget.isTargetLoaded ? 1.0 : 0.0,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _placeholderOpacity = (
            (Tween<double>?)visitor(
                _placeholderOpacity,
                widget.isTargetLoaded ? 0.0 : 1.0,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
    }

    public override void didUpdateTweens()
    {
        if (widget.wasSynchronouslyLoaded)
        {
            return;
        }
        _placeholderOpacityAnimation = (
            (Func<Animation<double>>)(
                () =>
                {
                    var __cascade = animation.drive(
                        new TweenSequence<double>(
                            new List<TweenSequenceItem<double>>
                            {
                                new TweenSequenceItem<double>(
                                    tween: _placeholderOpacity!.chain(
                                        new CurveTween(curve: widget.fadeOutCurve)
                                    ),
                                    weight: widget.fadeOutDuration.inMilliseconds.toDouble()
                                ),
                                new TweenSequenceItem<double>(
                                    tween: new ConstantTween<double>(0),
                                    weight: widget.fadeInDuration.inMilliseconds.toDouble()
                                ),
                            }
                        )
                    );
                    __cascade.addStatusListener(
                        (status) =>
                        {
                            if (_placeholderOpacityAnimation!.isCompleted)
                            {
                                setState(() => { });
                            }
                        }
                    );
                    return __cascade;
                }
            )
        )();
        _targetOpacityAnimation = animation.drive(
            new TweenSequence<double>(
                new List<TweenSequenceItem<double>>
                {
                    new TweenSequenceItem<double>(
                        tween: new ConstantTween<double>(0),
                        weight: widget.fadeOutDuration.inMilliseconds.toDouble()
                    ),
                    new TweenSequenceItem<double>(
                        tween: _targetOpacity!.chain(new CurveTween(curve: widget.fadeInCurve)),
                        weight: widget.fadeInDuration.inMilliseconds.toDouble()
                    ),
                }
            )
        );
        widget.targetProxyAnimation.parent = _targetOpacityAnimation;
        widget.placeholderProxyAnimation.parent = _placeholderOpacityAnimation;
    }

    public override Widget build(BuildContext context)
    {
        if (widget.wasSynchronouslyLoaded || (_placeholderOpacityAnimation?.isCompleted ?? true))
        {
            return widget.target;
        }
        return new Stack(
            fit: StackFit.passthrough,
            alignment: AlignmentDirectional.center,
            textDirection: TextDirection.ltr,
            children: new List<Widget> { widget.target, widget.placeholder }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<Animation<double>>("targetOpacity", _targetOpacityAnimation)
        );
        properties.add(
            new DiagnosticsProperty<Animation<double>>(
                "placeholderOpacity",
                _placeholderOpacityAnimation
            )
        );
    }
}
