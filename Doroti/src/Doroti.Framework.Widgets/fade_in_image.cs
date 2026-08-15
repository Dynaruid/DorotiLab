// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/fade_in_image.dart
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

public class FadeInImage : StatefulWidget
{
    public virtual dynamic placeholder { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? placeholderErrorBuilder { get; private set; }
    public virtual dynamic image { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? imageErrorBuilder { get; private set; }
    public virtual Duration fadeOutDuration { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Curve fadeOutCurve { get; private set; } = default!;
    public virtual Duration fadeInDuration { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Curve fadeInCurve { get; private set; } = default!;
    public virtual double? width { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual BlendMode? colorBlendMode { get; private set; }
    public virtual Color? placeholderColor { get; private set; }
    public virtual BlendMode? placeholderColorBlendMode { get; private set; }
    public virtual double? height { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BoxFit? fit { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BoxFit? placeholderFit { get; private set; }
    public virtual FilterQuality filterQuality { get; private set; } = default!;
    public virtual FilterQuality? placeholderFilterQuality { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.ImageRepeat repeat { get; private set; } = default!;
    public virtual bool matchTextDirection { get; private set; } = default!;
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual string? imageSemanticLabel { get; private set; }

    public FadeInImage(global::Doroti.Generated.Framework.Foundation.Key? key = null, dynamic placeholder = default!, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? placeholderErrorBuilder = null, dynamic image = default!, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? imageErrorBuilder = null, bool excludeFromSemantics = false, string? imageSemanticLabel = null, Duration? fadeOutDuration = null, global::Doroti.Generated.Framework.Animation.Curve fadeOutCurve = default!, Duration? fadeInDuration = null, global::Doroti.Generated.Framework.Animation.Curve fadeInCurve = default!, Color? color = null, BlendMode? colorBlendMode = null, Color? placeholderColor = null, BlendMode? placeholderColorBlendMode = null, double? width = null, double? height = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, global::Doroti.Generated.Framework.Painting.BoxFit? placeholderFit = null, FilterQuality filterQuality = FilterQuality.medium, FilterQuality? placeholderFilterQuality = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.ImageRepeat repeat = global::Doroti.Generated.Framework.Painting.ImageRepeat.noRepeat, bool matchTextDirection = false) : base(key: key)
    {
        Duration __fadeOutDuration = fadeOutDuration ?? Duration.Create(milliseconds: 300);
        global::Doroti.Generated.Framework.Animation.Curve __fadeOutCurve = fadeOutCurve ?? global::Doroti.Generated.Framework.Animation.Curves.easeOut;
        Duration __fadeInDuration = fadeInDuration ?? Duration.Create(milliseconds: 700);
        global::Doroti.Generated.Framework.Animation.Curve __fadeInCurve = fadeInCurve ?? global::Doroti.Generated.Framework.Animation.Curves.easeIn;
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
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

    public static FadeInImage CreateMemoryNetwork(global::Doroti.Generated.Framework.Foundation.Key? key = null, Uint8List placeholder = default!, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? placeholderErrorBuilder = null, string image = default!, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? imageErrorBuilder = null, double placeholderScale = 1.0, double imageScale = 1.0, bool excludeFromSemantics = false, string? imageSemanticLabel = null, Duration? fadeOutDuration = null, global::Doroti.Generated.Framework.Animation.Curve fadeOutCurve = default!, Duration? fadeInDuration = null, global::Doroti.Generated.Framework.Animation.Curve fadeInCurve = default!, double? width = null, double? height = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, Color? color = null, BlendMode? colorBlendMode = null, Color? placeholderColor = null, BlendMode? placeholderColorBlendMode = null, global::Doroti.Generated.Framework.Painting.BoxFit? placeholderFit = null, FilterQuality filterQuality = FilterQuality.medium, FilterQuality? placeholderFilterQuality = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.ImageRepeat repeat = global::Doroti.Generated.Framework.Painting.ImageRepeat.noRepeat, bool matchTextDirection = false, long? placeholderCacheWidth = null, long? placeholderCacheHeight = null, long? imageCacheWidth = null, long? imageCacheHeight = null)
    {
        var __instance = new FadeInImage(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        Duration __fadeOutDuration = fadeOutDuration ?? Duration.Create(milliseconds: 300);
        global::Doroti.Generated.Framework.Animation.Curve __fadeOutCurve = fadeOutCurve ?? global::Doroti.Generated.Framework.Animation.Curves.easeOut;
        Duration __fadeInDuration = fadeInDuration ?? Duration.Create(milliseconds: 700);
        global::Doroti.Generated.Framework.Animation.Curve __fadeInCurve = fadeInCurve ?? global::Doroti.Generated.Framework.Animation.Curves.easeIn;
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
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
        __instance.placeholder = ResizeImage.resizeIfNeeded(placeholderCacheWidth, placeholderCacheHeight, new global::Doroti.Generated.Framework.Painting.MemoryImage(((Uint8List)(object)placeholder), scale: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(placeholderScale))));
        __instance.image = ResizeImage.resizeIfNeeded(imageCacheWidth, imageCacheHeight, new global::Doroti.Generated.Framework.Painting.NetworkImageIo(((string)(object)image), scale: imageScale));
        return __instance;
    }

    public static FadeInImage CreateAssetNetwork(global::Doroti.Generated.Framework.Foundation.Key? key = null, string placeholder = default!, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? placeholderErrorBuilder = null, string image = default!, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? imageErrorBuilder = null, global::Doroti.Generated.Framework.Services.AssetBundle? bundle = null, double? placeholderScale = null, double imageScale = 1.0, bool excludeFromSemantics = false, string? imageSemanticLabel = null, Duration? fadeOutDuration = null, global::Doroti.Generated.Framework.Animation.Curve fadeOutCurve = default!, Duration? fadeInDuration = null, global::Doroti.Generated.Framework.Animation.Curve fadeInCurve = default!, double? width = null, double? height = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, Color? color = null, BlendMode? colorBlendMode = null, Color? placeholderColor = null, BlendMode? placeholderColorBlendMode = null, global::Doroti.Generated.Framework.Painting.BoxFit? placeholderFit = null, FilterQuality filterQuality = FilterQuality.medium, FilterQuality? placeholderFilterQuality = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.ImageRepeat repeat = global::Doroti.Generated.Framework.Painting.ImageRepeat.noRepeat, bool matchTextDirection = false, long? placeholderCacheWidth = null, long? placeholderCacheHeight = null, long? imageCacheWidth = null, long? imageCacheHeight = null)
    {
        var __instance = new FadeInImage(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        Duration __fadeOutDuration = fadeOutDuration ?? Duration.Create(milliseconds: 300);
        global::Doroti.Generated.Framework.Animation.Curve __fadeOutCurve = fadeOutCurve ?? global::Doroti.Generated.Framework.Animation.Curves.easeOut;
        Duration __fadeInDuration = fadeInDuration ?? Duration.Create(milliseconds: 700);
        global::Doroti.Generated.Framework.Animation.Curve __fadeInCurve = fadeInCurve ?? global::Doroti.Generated.Framework.Animation.Curves.easeIn;
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
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
        __instance.placeholder = ((placeholderScale is not null) ? ResizeImage.resizeIfNeeded(placeholderCacheWidth, placeholderCacheHeight, new global::Doroti.Generated.Framework.Painting.ExactAssetImage(((string)(object)placeholder), bundle: bundle, scale: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(placeholderScale)))) : ResizeImage.resizeIfNeeded(placeholderCacheWidth, placeholderCacheHeight, new global::Doroti.Generated.Framework.Painting.AssetImage(((string)(object)placeholder), bundle: bundle)));
        __instance.image = ResizeImage.resizeIfNeeded(imageCacheWidth, imageCacheHeight, new global::Doroti.Generated.Framework.Painting.NetworkImageIo(((string)(object)image), scale: imageScale));
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _FadeInImageState__fade_in_image());
}

internal class _FadeInImageState__fade_in_image : State<FadeInImage>
{
    internal static global::Doroti.Generated.Framework.Animation.Animation<double> _kOpaqueAnimation = ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)new global::Doroti.Generated.Framework.Animation.AlwaysStoppedAnimation<double>(1.0));
    public virtual bool targetLoaded { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation _imageAnimation { get; private set; } = new global::Doroti.Generated.Framework.Animation.ProxyAnimation(_kOpaqueAnimation);
    internal virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation _placeholderAnimation { get; private set; } = new global::Doroti.Generated.Framework.Animation.ProxyAnimation(_kOpaqueAnimation);

    internal virtual Image _image(dynamic image, global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>? errorBuilder = null, global::System.Func<BuildContext, Widget, long?, bool, Widget>? frameBuilder = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, Color? color = null, BlendMode? colorBlendMode = null, FilterQuality filterQuality = default!, global::Doroti.Generated.Framework.Animation.Animation<double> opacity = default!)
    {
        return new Image(image: image, errorBuilder: (global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>?)errorBuilder, frameBuilder: (global::System.Func<BuildContext, Widget, long?, bool, Widget>?)frameBuilder, opacity: opacity, width: ((FadeInImage)this.widget).width, height: ((FadeInImage)this.widget).height, fit: fit, color: color, colorBlendMode: colorBlendMode, filterQuality: filterQuality, alignment: ((FadeInImage)this.widget).alignment, repeat: ((FadeInImage)this.widget).repeat, matchTextDirection: ((FadeInImage)this.widget).matchTextDirection, gaplessPlayback: true, excludeFromSemantics: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        Widget result__17561 = ((Widget)(object?)_image(image: ((FadeInImage)this.widget).image, errorBuilder: (global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>?)((FadeInImage)this.widget).imageErrorBuilder, opacity: this._imageAnimation, fit: ((FadeInImage)this.widget).fit, color: ((FadeInImage)this.widget).color, colorBlendMode: ((FadeInImage)this.widget).colorBlendMode, filterQuality: ((FadeInImage)this.widget).filterQuality, frameBuilder: ((global::System.Func<BuildContext, Widget, long?, bool, Widget>?)((context, child, frame, wasSynchronouslyLoaded) => {
if ((wasSynchronouslyLoaded || (frame is not null)))
{
    targetLoaded = true;
}
return ((Widget)(object?)new _AnimatedFadeOutFadeIn__fade_in_image(target: child, targetProxyAnimation: this._imageAnimation, placeholder: _image(image: ((FadeInImage)this.widget).placeholder, errorBuilder: (global::System.Func<BuildContext, object, global::System.Diagnostics.StackTrace?, Widget>?)((FadeInImage)this.widget).placeholderErrorBuilder, opacity: this._placeholderAnimation, color: ((FadeInImage)this.widget).placeholderColor, colorBlendMode: ((FadeInImage)this.widget).placeholderColorBlendMode, fit: (((FadeInImage)this.widget).placeholderFit ?? ((FadeInImage)this.widget).fit), filterQuality: (((FadeInImage)this.widget).placeholderFilterQuality ?? ((FadeInImage)this.widget).filterQuality)), placeholderProxyAnimation: this._placeholderAnimation, isTargetLoaded: this.targetLoaded, wasSynchronouslyLoaded: wasSynchronouslyLoaded, fadeInDuration: ((FadeInImage)this.widget).fadeInDuration, fadeOutDuration: ((FadeInImage)this.widget).fadeOutDuration, fadeInCurve: ((FadeInImage)this.widget).fadeInCurve, fadeOutCurve: ((FadeInImage)this.widget).fadeOutCurve));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        if (!((FadeInImage)this.widget).excludeFromSemantics)
        {
            result__17561 = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(container: (((FadeInImage)this.widget).imageSemanticLabel is not null), image: true, label: (((FadeInImage)this.widget).imageSemanticLabel ?? ""), child: result__17561));
        }
        return result__17561;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AnimatedFadeOutFadeIn__fade_in_image : ImplicitlyAnimatedWidget
{
    public virtual Widget target { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation targetProxyAnimation { get; private set; } = default!;
    public virtual Widget placeholder { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation placeholderProxyAnimation { get; private set; } = default!;
    public virtual bool isTargetLoaded { get; private set; } = default!;
    public virtual Duration fadeInDuration { get; private set; } = default!;
    public virtual Duration fadeOutDuration { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Curve fadeInCurve { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Curve fadeOutCurve { get; private set; } = default!;
    public virtual bool wasSynchronouslyLoaded { get; private set; } = default!;

    internal _AnimatedFadeOutFadeIn__fade_in_image(Widget target, global::Doroti.Generated.Framework.Animation.ProxyAnimation targetProxyAnimation, Widget placeholder, global::Doroti.Generated.Framework.Animation.ProxyAnimation placeholderProxyAnimation, bool isTargetLoaded, Duration fadeOutDuration, global::Doroti.Generated.Framework.Animation.Curve fadeOutCurve, Duration fadeInDuration, global::Doroti.Generated.Framework.Animation.Curve fadeInCurve, bool wasSynchronouslyLoaded) : base(duration: (fadeInDuration + fadeOutDuration))
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
        System.Diagnostics.Debug.Assert((!wasSynchronouslyLoaded || isTargetLoaded));
    }

    public override _AnimatedFadeOutFadeInState__fade_in_image createState() => new _AnimatedFadeOutFadeInState__fade_in_image();
}

internal class _AnimatedFadeOutFadeInState__fade_in_image : ImplicitlyAnimatedWidgetState<_AnimatedFadeOutFadeIn__fade_in_image>
{
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _targetOpacity { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _placeholderOpacity { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double>? _targetOpacityAnimation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double>? _placeholderOpacityAnimation { get; set; } = default;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _targetOpacity = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._targetOpacity, (((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).isTargetLoaded ? 1.0 : 0.0), ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _placeholderOpacity = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._placeholderOpacity, (((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).isTargetLoaded ? 0.0 : 1.0), ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
    }

    public override void didUpdateTweens()
    {
        if (((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).wasSynchronouslyLoaded)
        {
            return;
        }
        _placeholderOpacityAnimation = ((Func<global::Doroti.Generated.Framework.Animation.Animation<double>>)(() =>
{            var __cascade = this.animation.drive(new global::Doroti.Generated.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>(tween: this._placeholderOpacity!.chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: ((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).fadeOutCurve)), weight: ((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).fadeOutDuration.inMilliseconds.toDouble()), new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Generated.Framework.Animation.ConstantTween<double>(0), weight: ((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).fadeInDuration.inMilliseconds.toDouble()) }));
            __cascade.addStatusListener(((AnimationStatusListener)((status) => {
if (this._placeholderOpacityAnimation!.isCompleted)
{
    setState(((global::System.Action)(() => {
})));
}
})));
            return __cascade;        }))();
        _targetOpacityAnimation = this.animation.drive(new global::Doroti.Generated.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Generated.Framework.Animation.ConstantTween<double>(0), weight: ((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).fadeOutDuration.inMilliseconds.toDouble()), new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>(tween: this._targetOpacity!.chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: ((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).fadeInCurve)), weight: ((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).fadeInDuration.inMilliseconds.toDouble()) }));
        ((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).targetProxyAnimation.parent = this._targetOpacityAnimation;
        ((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).placeholderProxyAnimation.parent = this._placeholderOpacityAnimation;
    }

    public override Widget build(BuildContext context)
    {
        if ((((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).wasSynchronouslyLoaded || ((this._placeholderOpacityAnimation?.isCompleted ?? true))))
        {
            return ((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).target;
        }
        return ((Widget)(object?)new Stack(fit: global::Doroti.Generated.Framework.Rendering.StackFit.passthrough, alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.center, textDirection: TextDirection.ltr, children: new List<Widget> { ((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).target, ((_AnimatedFadeOutFadeIn__fade_in_image)this.widget).placeholder }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Animation.Animation<double>>("targetOpacity", this._targetOpacityAnimation));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Animation.Animation<double>>("placeholderOpacity", this._placeholderOpacityAnimation));
    }

}

