// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/fade_in_image.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class FadeInImage : StatefulWidget
{
    public virtual global::Doroti.Framework.Painting.IImageProvider placeholder { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? placeholderErrorBuilder { get; private set; }
    public virtual global::Doroti.Framework.Painting.IImageProvider image { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? imageErrorBuilder { get; private set; }
    public virtual Duration fadeOutDuration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve fadeOutCurve { get; private set; } = default!;
    public virtual Duration fadeInDuration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve fadeInCurve { get; private set; } = default!;
    public virtual double? width { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual BlendMode? colorBlendMode { get; private set; }
    public virtual Color? placeholderColor { get; private set; }
    public virtual BlendMode? placeholderColorBlendMode { get; private set; }
    public virtual double? height { get; private set; }
    public virtual global::Doroti.Framework.Painting.BoxFit? fit { get; private set; }
    public virtual global::Doroti.Framework.Painting.BoxFit? placeholderFit { get; private set; }
    public virtual FilterQuality filterQuality { get; private set; } = default!;
    public virtual FilterQuality? placeholderFilterQuality { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.ImageRepeat repeat { get; private set; } = default!;
    public virtual bool matchTextDirection { get; private set; } = default!;
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual string? imageSemanticLabel { get; private set; }

    public FadeInImage(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.IImageProvider placeholder = default!, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? placeholderErrorBuilder = null, global::Doroti.Framework.Painting.IImageProvider image = default!, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? imageErrorBuilder = null, bool excludeFromSemantics = false, string? imageSemanticLabel = null, Duration? fadeOutDuration = null, global::Doroti.Framework.Animation.Curve fadeOutCurve = default!, Duration? fadeInDuration = null, global::Doroti.Framework.Animation.Curve fadeInCurve = default!, Color? color = null, BlendMode? colorBlendMode = null, Color? placeholderColor = null, BlendMode? placeholderColorBlendMode = null, double? width = null, double? height = null, global::Doroti.Framework.Painting.BoxFit? fit = null, global::Doroti.Framework.Painting.BoxFit? placeholderFit = null, FilterQuality filterQuality = FilterQuality.medium, FilterQuality? placeholderFilterQuality = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Painting.ImageRepeat repeat = ImageRepeat.noRepeat, bool matchTextDirection = false) : base(key: key)
    {
        Duration __fadeOutDuration = fadeOutDuration ?? Duration.Create(milliseconds: 300);
        global::Doroti.Framework.Animation.Curve __fadeOutCurve = fadeOutCurve ?? Curves.easeOut;
        Duration __fadeInDuration = fadeInDuration ?? Duration.Create(milliseconds: 700);
        global::Doroti.Framework.Animation.Curve __fadeInCurve = fadeInCurve ?? Curves.easeIn;
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? Alignment.center;
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

    public static FadeInImage CreateMemoryNetwork(global::Doroti.Framework.Foundation.Key? key = null, Uint8List placeholder = default!, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? placeholderErrorBuilder = null, string image = default!, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? imageErrorBuilder = null, double placeholderScale = 1.0, double imageScale = 1.0, bool excludeFromSemantics = false, string? imageSemanticLabel = null, Duration? fadeOutDuration = null, global::Doroti.Framework.Animation.Curve fadeOutCurve = default!, Duration? fadeInDuration = null, global::Doroti.Framework.Animation.Curve fadeInCurve = default!, double? width = null, double? height = null, global::Doroti.Framework.Painting.BoxFit? fit = null, Color? color = null, BlendMode? colorBlendMode = null, Color? placeholderColor = null, BlendMode? placeholderColorBlendMode = null, global::Doroti.Framework.Painting.BoxFit? placeholderFit = null, FilterQuality filterQuality = FilterQuality.medium, FilterQuality? placeholderFilterQuality = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Painting.ImageRepeat repeat = ImageRepeat.noRepeat, bool matchTextDirection = false, long? placeholderCacheWidth = null, long? placeholderCacheHeight = null, long? imageCacheWidth = null, long? imageCacheHeight = null)
    {
        var __instance = new FadeInImage(key, ResizeImage.resizeIfNeeded(placeholderCacheWidth, placeholderCacheHeight, new global::Doroti.Framework.Painting.MemoryImage(placeholder, scale: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(placeholderScale)))), placeholderErrorBuilder, ResizeImage.resizeIfNeeded(imageCacheWidth, imageCacheHeight, new global::Doroti.Framework.Painting.NetworkImageIo(image, scale: imageScale)), imageErrorBuilder, excludeFromSemantics, imageSemanticLabel, fadeOutDuration, fadeOutCurve, fadeInDuration, fadeInCurve, color, colorBlendMode, placeholderColor, placeholderColorBlendMode, width, height, fit, placeholderFit, filterQuality, placeholderFilterQuality, alignment, repeat, matchTextDirection);
        Duration __fadeOutDuration = fadeOutDuration ?? Duration.Create(milliseconds: 300);
        global::Doroti.Framework.Animation.Curve __fadeOutCurve = fadeOutCurve ?? Curves.easeOut;
        Duration __fadeInDuration = fadeInDuration ?? Duration.Create(milliseconds: 700);
        global::Doroti.Framework.Animation.Curve __fadeInCurve = fadeInCurve ?? Curves.easeIn;
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? Alignment.center;
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

    public static FadeInImage CreateAssetNetwork(global::Doroti.Framework.Foundation.Key? key = null, string placeholder = default!, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? placeholderErrorBuilder = null, string image = default!, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? imageErrorBuilder = null, global::Doroti.Framework.Services.AssetBundle? bundle = null, double? placeholderScale = null, double imageScale = 1.0, bool excludeFromSemantics = false, string? imageSemanticLabel = null, Duration? fadeOutDuration = null, global::Doroti.Framework.Animation.Curve fadeOutCurve = default!, Duration? fadeInDuration = null, global::Doroti.Framework.Animation.Curve fadeInCurve = default!, double? width = null, double? height = null, global::Doroti.Framework.Painting.BoxFit? fit = null, Color? color = null, BlendMode? colorBlendMode = null, Color? placeholderColor = null, BlendMode? placeholderColorBlendMode = null, global::Doroti.Framework.Painting.BoxFit? placeholderFit = null, FilterQuality filterQuality = FilterQuality.medium, FilterQuality? placeholderFilterQuality = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Painting.ImageRepeat repeat = ImageRepeat.noRepeat, bool matchTextDirection = false, long? placeholderCacheWidth = null, long? placeholderCacheHeight = null, long? imageCacheWidth = null, long? imageCacheHeight = null)
    {
        var __instance = new FadeInImage(key, (placeholderScale is not null) ? ResizeImage.resizeIfNeeded(placeholderCacheWidth, placeholderCacheHeight, new global::Doroti.Framework.Painting.ExactAssetImage(placeholder, bundle: bundle, scale: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(placeholderScale)))) : ResizeImage.resizeIfNeeded(placeholderCacheWidth, placeholderCacheHeight, new global::Doroti.Framework.Painting.AssetImage(placeholder, bundle: bundle)), placeholderErrorBuilder, ResizeImage.resizeIfNeeded(imageCacheWidth, imageCacheHeight, new global::Doroti.Framework.Painting.NetworkImageIo(image, scale: imageScale)), imageErrorBuilder, excludeFromSemantics, imageSemanticLabel, fadeOutDuration, fadeOutCurve, fadeInDuration, fadeInCurve, color, colorBlendMode, placeholderColor, placeholderColorBlendMode, width, height, fit, placeholderFit, filterQuality, placeholderFilterQuality, alignment, repeat, matchTextDirection);
        Duration __fadeOutDuration = fadeOutDuration ?? Duration.Create(milliseconds: 300);
        global::Doroti.Framework.Animation.Curve __fadeOutCurve = fadeOutCurve ?? Curves.easeOut;
        Duration __fadeInDuration = fadeInDuration ?? Duration.Create(milliseconds: 700);
        global::Doroti.Framework.Animation.Curve __fadeInCurve = fadeInCurve ?? Curves.easeIn;
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? Alignment.center;
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _FadeInImageState__fade_in_image());
}

internal class _FadeInImageState__fade_in_image : State<FadeInImage>
{
    internal static global::Doroti.Framework.Animation.Animation<double> _kOpaqueAnimation = new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1.0);
    public virtual bool targetLoaded { get; set; } = false;
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation _imageAnimation { get; private set; } = new global::Doroti.Framework.Animation.ProxyAnimation(_kOpaqueAnimation);
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation _placeholderAnimation { get; private set; } = new global::Doroti.Framework.Animation.ProxyAnimation(_kOpaqueAnimation);

    internal virtual Image _image(global::Doroti.Framework.Painting.IImageProvider image, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder = null, global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null, global::Doroti.Framework.Painting.BoxFit? fit = null, Color? color = null, BlendMode? colorBlendMode = null, FilterQuality filterQuality = default!, global::Doroti.Framework.Animation.Animation<double> opacity = default!)
    {
        return new Image(image: image, errorBuilder: errorBuilder, frameBuilder: frameBuilder, opacity: opacity, width: widget.width, height: widget.height, fit: fit, color: color, colorBlendMode: colorBlendMode, filterQuality: filterQuality, alignment: widget.alignment, repeat: widget.repeat, matchTextDirection: widget.matchTextDirection, gaplessPlayback: true, excludeFromSemantics: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        Widget result = _image(image: widget.image, errorBuilder: widget.imageErrorBuilder, opacity: _imageAnimation, fit: widget.fit, color: widget.color, colorBlendMode: widget.colorBlendMode, filterQuality: widget.filterQuality, frameBuilder: (context, child, frame, wasSynchronouslyLoaded) =>
        {
            if (wasSynchronouslyLoaded || (frame is not null))
            {
                targetLoaded = true;
            }
            return new _AnimatedFadeOutFadeIn__fade_in_image(target: child, targetProxyAnimation: _imageAnimation, placeholder: _image(image: widget.placeholder, errorBuilder: widget.placeholderErrorBuilder, opacity: _placeholderAnimation, color: widget.placeholderColor, colorBlendMode: widget.placeholderColorBlendMode, fit: widget.placeholderFit ?? widget.fit, filterQuality: widget.placeholderFilterQuality ?? widget.filterQuality), placeholderProxyAnimation: _placeholderAnimation, isTargetLoaded: targetLoaded, wasSynchronouslyLoaded: wasSynchronouslyLoaded, fadeInDuration: widget.fadeInDuration, fadeOutDuration: widget.fadeOutDuration, fadeInCurve: widget.fadeInCurve, fadeOutCurve: widget.fadeOutCurve);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        if (!widget.excludeFromSemantics)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(container: widget.imageSemanticLabel is not null, image: true, label: widget.imageSemanticLabel ?? "", child: result));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AnimatedFadeOutFadeIn__fade_in_image : ImplicitlyAnimatedWidget
{
    public virtual Widget target { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.ProxyAnimation targetProxyAnimation { get; private set; } = default!;
    public virtual Widget placeholder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.ProxyAnimation placeholderProxyAnimation { get; private set; } = default!;
    public virtual bool isTargetLoaded { get; private set; } = default!;
    public virtual Duration fadeInDuration { get; private set; } = default!;
    public virtual Duration fadeOutDuration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve fadeInCurve { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve fadeOutCurve { get; private set; } = default!;
    public virtual bool wasSynchronouslyLoaded { get; private set; } = default!;

    internal _AnimatedFadeOutFadeIn__fade_in_image(Widget target, global::Doroti.Framework.Animation.ProxyAnimation targetProxyAnimation, Widget placeholder, global::Doroti.Framework.Animation.ProxyAnimation placeholderProxyAnimation, bool isTargetLoaded, Duration fadeOutDuration, global::Doroti.Framework.Animation.Curve fadeOutCurve, Duration fadeInDuration, global::Doroti.Framework.Animation.Curve fadeInCurve, bool wasSynchronouslyLoaded) : base(duration: fadeInDuration + fadeOutDuration)
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

    public override _AnimatedFadeOutFadeInState__fade_in_image createState() => new _AnimatedFadeOutFadeInState__fade_in_image();
}

internal class _AnimatedFadeOutFadeInState__fade_in_image : ImplicitlyAnimatedWidgetState<_AnimatedFadeOutFadeIn__fade_in_image>
{
    internal virtual global::Doroti.Framework.Animation.Tween<double>? _targetOpacity { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Tween<double>? _placeholderOpacity { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Animation<double>? _targetOpacityAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Animation<double>? _placeholderOpacityAnimation { get; set; } = default;

    public override void forEachTween(global::System.Func<global::Doroti.Framework.Animation.IDartTween?, object?, global::System.Func<object, global::Doroti.Framework.Animation.IDartTween>, global::Doroti.Framework.Animation.IDartTween?> visitor)
    {
        _targetOpacity = ((global::Doroti.Framework.Animation.Tween<double>?)visitor(_targetOpacity, widget.isTargetLoaded ? 1.0 : 0.0, (value) => new global::Doroti.Framework.Animation.Tween<double>(begin: (double)value)))!;
        _placeholderOpacity = ((global::Doroti.Framework.Animation.Tween<double>?)visitor(_placeholderOpacity, widget.isTargetLoaded ? 0.0 : 1.0, (value) => new global::Doroti.Framework.Animation.Tween<double>(begin: (double)value)))!;
    }

    public override void didUpdateTweens()
    {
        if (widget.wasSynchronouslyLoaded)
        {
            return;
        }
        _placeholderOpacityAnimation = ((Func<global::Doroti.Framework.Animation.Animation<double>>)(() =>
{
    var __cascade = animation.drive(new global::Doroti.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: _placeholderOpacity!.chain(new global::Doroti.Framework.Animation.CurveTween(curve: widget.fadeOutCurve)), weight: widget.fadeOutDuration.inMilliseconds.toDouble()), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.ConstantTween<double>(0), weight: widget.fadeInDuration.inMilliseconds.toDouble()) }));
    __cascade.addStatusListener((status) =>
    {
        if (_placeholderOpacityAnimation!.isCompleted)
        {
            setState(() =>
            {
            });
        }
    });
    return __cascade;
}))();
        _targetOpacityAnimation = animation.drive(new global::Doroti.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.ConstantTween<double>(0), weight: widget.fadeOutDuration.inMilliseconds.toDouble()), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: _targetOpacity!.chain(new global::Doroti.Framework.Animation.CurveTween(curve: widget.fadeInCurve)), weight: widget.fadeInDuration.inMilliseconds.toDouble()) }));
        widget.targetProxyAnimation.parent = _targetOpacityAnimation;
        widget.placeholderProxyAnimation.parent = _placeholderOpacityAnimation;
    }

    public override Widget build(BuildContext context)
    {
        if (widget.wasSynchronouslyLoaded || (_placeholderOpacityAnimation?.isCompleted ?? true))
        {
            return widget.target;
        }
        return new Stack(fit: StackFit.passthrough, alignment: AlignmentDirectional.center, textDirection: TextDirection.ltr, children: new List<Widget> { widget.target, widget.placeholder });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Animation.Animation<double>>("targetOpacity", _targetOpacityAnimation));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Animation.Animation<double>>("placeholderOpacity", _placeholderOpacityAnimation));
    }

}

