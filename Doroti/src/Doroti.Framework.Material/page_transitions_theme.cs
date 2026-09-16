// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/page_transitions_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal class _ZoomPageTransition__page_transitions_theme : global::Doroti.Framework.Widgets.StatelessWidget
{
    public static List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> fastOutExtraSlowInTweenSequenceItems = new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 0.4).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Cubic(0.05, 0.0, 0.133333, 0.06))), weight: 0.166666), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: 0.4, end: 1.0).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Cubic(0.208333, 0.82, 0.25, 1.0))), weight: 1.0 - 0.166666) };
    internal static global::Doroti.Framework.Animation.TweenSequence<double> _scaleCurveSequence = new global::Doroti.Framework.Animation.TweenSequence<double>(fastOutExtraSlowInTweenSequenceItems);
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual bool allowSnapshotting { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual bool allowEnterRouteSnapshotting { get; private set; } = default!;

    internal _ZoomPageTransition__page_transitions_theme(global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, bool allowSnapshotting, bool allowEnterRouteSnapshotting, Color? backgroundColor = null, global::Doroti.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.allowSnapshotting = allowSnapshotting;
        this.allowEnterRouteSnapshotting = allowEnterRouteSnapshotting;
        this.backgroundColor = backgroundColor;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color enterTransitionBackgroundColor = backgroundColor ?? Theme.of(context).colorScheme.surface;
        return new global::Doroti.Framework.Widgets.DualTransitionBuilder(animation: animation, forwardBuilder: (context, animation, child) =>
        {
            return new _ZoomEnterTransition__page_transitions_theme(animation: animation, allowSnapshotting: allowSnapshotting && allowEnterRouteSnapshotting, backgroundColor: enterTransitionBackgroundColor, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, reverseBuilder: (context, animation, child) =>
        {
            return new _ZoomExitTransition__page_transitions_theme(animation: animation, allowSnapshotting: allowSnapshotting, reverse: true, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: ZoomPageTransitionsBuilder._snapshotAwareDelegatedTransition(context, animation, secondaryAnimation, child, allowSnapshotting, allowEnterRouteSnapshotting, enterTransitionBackgroundColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ZoomEnterTransition__page_transitions_theme : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual bool allowSnapshotting { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual Color backgroundColor { get; private set; } = default!;

    internal _ZoomEnterTransition__page_transitions_theme(global::Doroti.Framework.Animation.Animation<double> animation, bool reverse = false, bool allowSnapshotting = default!, Color backgroundColor = default!, global::Doroti.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.reverse = reverse;
        this.allowSnapshotting = allowSnapshotting;
        this.backgroundColor = backgroundColor;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ZoomEnterTransitionState__page_transitions_theme());
}

internal class _ZoomEnterTransitionState__page_transitions_theme : global::Doroti.Framework.Widgets.State<_ZoomEnterTransition__page_transitions_theme>, _ZoomTransitionBase__page_transitions_theme<_ZoomEnterTransition__page_transitions_theme>
{
    public virtual _ZoomEnterTransitionPainter__page_transitions_theme @delegate { get; set; } = default!;
    internal static global::Doroti.Framework.Animation.Animatable<double> _fadeInTransition = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.125, 0.25)));
    internal static global::Doroti.Framework.Animation.Animatable<double> _scaleDownTransition = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.1, end: 1.0).chain(_ZoomPageTransition__page_transitions_theme._scaleCurveSequence);
    internal static global::Doroti.Framework.Animation.Animatable<double> _scaleUpTransition = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.85, end: 1.0).chain(_ZoomPageTransition__page_transitions_theme._scaleCurveSequence);
    internal static global::Doroti.Framework.Animation.Animatable<double?> _scrimOpacityTween = new global::Doroti.Framework.Animation.Tween<double?>(begin: 0.0, end: 0.6).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.2075, 0.4175)));
    public virtual global::Doroti.Framework.Widgets.SnapshotController controller { get; set; } = new global::Doroti.Framework.Widgets.SnapshotController();
    public virtual global::Doroti.Framework.Animation.Animation<double> fadeTransition { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> scaleTransition { get; set; } = default!;

    public virtual bool useSnapshot => DartRuntimePrimitives.ConvertValue<bool>(!Foundation.ConstantsLibrary.kIsWeb && widget.allowSnapshotting);
    internal virtual void _updateAnimations()
    {
        fadeTransition = widget.reverse ? AnimationsLibrary.kAlwaysCompleteAnimation : _fadeInTransition.animate(widget.animation);
        scaleTransition = (widget.reverse ? _scaleDownTransition : _scaleUpTransition).animate(widget.animation);
        widget.animation.addListener(onAnimationValueChange);
        widget.animation.addStatusListener(onAnimationStatusChange);
    }

    public override void initState()
    {
        _updateAnimations();
        @delegate = new _ZoomEnterTransitionPainter__page_transitions_theme(reverse: widget.reverse, fade: fadeTransition, scale: scaleTransition, animation: widget.animation, backgroundColor: widget.backgroundColor);
        base.initState();
    }

    public override void didUpdateWidget(_ZoomEnterTransition__page_transitions_theme oldWidget)
    {
        if ((oldWidget.reverse != widget.reverse) || (!Equals(oldWidget.animation, widget.animation)))
        {
            oldWidget.animation.removeListener(onAnimationValueChange);
            oldWidget.animation.removeStatusListener(onAnimationStatusChange);
            _updateAnimations();
            @delegate.dispose();
            @delegate = new _ZoomEnterTransitionPainter__page_transitions_theme(reverse: widget.reverse, fade: fadeTransition, scale: scaleTransition, animation: widget.animation, backgroundColor: widget.backgroundColor);
        }
        base.didUpdateWidget(oldWidget);
    }

    public override void dispose()
    {
        widget.animation.removeListener(onAnimationValueChange);
        widget.animation.removeStatusListener(onAnimationStatusChange);
        @delegate.dispose();
        controller.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.SnapshotWidget(painter: @delegate, controller: controller, mode: SnapshotMode.permissive, autoresize: true, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void onAnimationValueChange()
    {
        if (scaleTransition.value == 1.0 && ((fadeTransition.value == 0.0) || (fadeTransition.value == 1.0)))
        {
            controller.allowSnapshotting = false;
        }
        else
        {
            controller.allowSnapshotting = useSnapshot;
        }
    }

    public virtual void onAnimationStatusChange(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        controller.allowSnapshotting = AnimationStatusMembers.isAnimating(status) && useSnapshot;
    }

}

public class _ZoomExitTransition__page_transitions_theme : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual bool allowSnapshotting { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    internal _ZoomExitTransition__page_transitions_theme(global::Doroti.Framework.Animation.Animation<double> animation, bool reverse = false, bool allowSnapshotting = default!, global::Doroti.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.reverse = reverse;
        this.allowSnapshotting = allowSnapshotting;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ZoomExitTransitionState__page_transitions_theme());
}

internal class _ZoomExitTransitionState__page_transitions_theme : global::Doroti.Framework.Widgets.State<_ZoomExitTransition__page_transitions_theme>, _ZoomTransitionBase__page_transitions_theme<_ZoomExitTransition__page_transitions_theme>
{
    public virtual _ZoomExitTransitionPainter__page_transitions_theme @delegate { get; set; } = default!;
    internal static global::Doroti.Framework.Animation.Animatable<double> _fadeOutTransition = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.0825, 0.2075)));
    internal static global::Doroti.Framework.Animation.Animatable<double> _scaleUpTransition = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 1.05).chain(_ZoomPageTransition__page_transitions_theme._scaleCurveSequence);
    internal static global::Doroti.Framework.Animation.Animatable<double> _scaleDownTransition = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.9).chain(_ZoomPageTransition__page_transitions_theme._scaleCurveSequence);
    public virtual global::Doroti.Framework.Widgets.SnapshotController controller { get; set; } = new global::Doroti.Framework.Widgets.SnapshotController();
    public virtual global::Doroti.Framework.Animation.Animation<double> fadeTransition { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> scaleTransition { get; set; } = default!;

    public virtual bool useSnapshot => DartRuntimePrimitives.ConvertValue<bool>(!Foundation.ConstantsLibrary.kIsWeb && widget.allowSnapshotting);
    internal virtual void _updateAnimations()
    {
        fadeTransition = widget.reverse ? _fadeOutTransition.animate(widget.animation) : AnimationsLibrary.kAlwaysCompleteAnimation;
        scaleTransition = (widget.reverse ? _scaleDownTransition : _scaleUpTransition).animate(widget.animation);
        widget.animation.addListener(onAnimationValueChange);
        widget.animation.addStatusListener(onAnimationStatusChange);
    }

    public override void initState()
    {
        _updateAnimations();
        @delegate = new _ZoomExitTransitionPainter__page_transitions_theme(reverse: widget.reverse, fade: fadeTransition, scale: scaleTransition, animation: widget.animation);
        base.initState();
    }

    public override void didUpdateWidget(_ZoomExitTransition__page_transitions_theme oldWidget)
    {
        if ((oldWidget.reverse != widget.reverse) || (!Equals(oldWidget.animation, widget.animation)))
        {
            oldWidget.animation.removeListener(onAnimationValueChange);
            oldWidget.animation.removeStatusListener(onAnimationStatusChange);
            _updateAnimations();
            @delegate.dispose();
            @delegate = new _ZoomExitTransitionPainter__page_transitions_theme(reverse: widget.reverse, fade: fadeTransition, scale: scaleTransition, animation: widget.animation);
        }
        base.didUpdateWidget(oldWidget);
    }

    public override void dispose()
    {
        widget.animation.removeListener(onAnimationValueChange);
        widget.animation.removeStatusListener(onAnimationStatusChange);
        @delegate.dispose();
        controller.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.SnapshotWidget(painter: @delegate, controller: controller, mode: SnapshotMode.permissive, autoresize: true, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void onAnimationValueChange()
    {
        if (scaleTransition.value == 1.0 && ((fadeTransition.value == 0.0) || (fadeTransition.value == 1.0)))
        {
            controller.allowSnapshotting = false;
        }
        else
        {
            controller.allowSnapshotting = useSnapshot;
        }
    }

    public virtual void onAnimationStatusChange(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        controller.allowSnapshotting = AnimationStatusMembers.isAnimating(status) && useSnapshot;
    }

}

internal class _FadeForwardsPageTransition__page_transitions_theme : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _forwardTranslationTween = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.25, 0.0), end: Offset.zero).chain(new global::Doroti.Framework.Animation.CurveTween(curve: FadeForwardsPageTransitionsBuilder._transitionCurve));
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _backwardTranslationTween = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: new global::Doroti.Ui.Offset(0.25, 0.0)).chain(new global::Doroti.Framework.Animation.CurveTween(curve: FadeForwardsPageTransitionsBuilder._transitionCurve));

    internal _FadeForwardsPageTransition__page_transitions_theme(global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, Color? backgroundColor = null, global::Doroti.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.backgroundColor = backgroundColor;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.DualTransitionBuilder(animation: animation, forwardBuilder: (context, animation, child) =>
        {
            return new global::Doroti.Framework.Widgets.FadeTransition(opacity: FadeForwardsPageTransitionsBuilder._fadeInTransition.animate(animation), child: new global::Doroti.Framework.Widgets.SlideTransition(position: _forwardTranslationTween.animate(animation), child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, reverseBuilder: (context, animation, child) =>
        {
            return new global::Doroti.Framework.Widgets.IgnorePointer(ignoring: Equals(animation.status, AnimationStatus.forward), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: FadeForwardsPageTransitionsBuilder._fadeOutTransition.animate(animation), child: new global::Doroti.Framework.Widgets.SlideTransition(position: _backwardTranslationTween.animate(animation), child: child)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: FadeForwardsPageTransitionsBuilder._delegatedTransition(context, secondaryAnimation, backgroundColor, child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FadeForwardsPageTransitionsBuilder : global::Doroti.Framework.Widgets.PageTransitionsBuilder
{
    public virtual Color? backgroundColor { get; private set; }
    public const long kTransitionMilliseconds = 450L;
    internal static global::Doroti.Framework.Animation.Curve _transitionCurve = Curves.easeInOutCubicEmphasized;
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _secondaryBackwardTranslationTween = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: new global::Doroti.Ui.Offset(-0.25, 0.0)).chain(new global::Doroti.Framework.Animation.CurveTween(curve: _transitionCurve));
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _secondaryForwardTranslationTween = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(-0.25, 0.0), end: Offset.zero).chain(new global::Doroti.Framework.Animation.CurveTween(curve: _transitionCurve));
    internal static global::Doroti.Framework.Animation.Animatable<double> _fadeInTransition = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.0, 0.75)));
    internal static global::Doroti.Framework.Animation.Animatable<double> _fadeOutTransition = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.0, 0.25)));

    public FadeForwardsPageTransitionsBuilder(Color? backgroundColor = null)
    {
        this.backgroundColor = backgroundColor;
    }

    public override Duration transitionDuration => Duration.Create(milliseconds: kTransitionMilliseconds);
    public override global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>? delegatedTransition => (context, animation, secondaryAnimation, allowSnapshotting, child) => _delegatedTransition(context, secondaryAnimation, backgroundColor, child);
    internal static global::Doroti.Framework.Widgets.Widget _delegatedTransition(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, Color? backgroundColor, global::Doroti.Framework.Widgets.Widget? child)
    {
        global::Doroti.Framework.Widgets.Widget builder = new global::Doroti.Framework.Widgets.DualTransitionBuilder(animation: new global::Doroti.Framework.Animation.ReverseAnimation(secondaryAnimation), forwardBuilder: (context, animation, child) =>
        {
            return new global::Doroti.Framework.Widgets.FadeTransition(opacity: _fadeInTransition.animate(animation), child: new global::Doroti.Framework.Widgets.SlideTransition(position: _secondaryForwardTranslationTween.animate(animation), child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, reverseBuilder: (context, animation, child) =>
        {
            return new global::Doroti.Framework.Widgets.FadeTransition(opacity: _fadeOutTransition.animate(animation), child: new global::Doroti.Framework.Widgets.SlideTransition(position: _secondaryBackwardTranslationTween.animate(animation), child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: child);
        bool isOpaque = ModalRoute<object>.opaqueOf(context) ?? true;
        if (!isOpaque)
        {
            return builder;
        }
        return new global::Doroti.Framework.Widgets.ColoredBox(color: secondaryAnimation.isAnimating ? (backgroundColor ?? ColorScheme.of(context).surface) : Colors.transparent, child: builder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildTransitions<T>(global::Doroti.Framework.Widgets.PageRoute<T> route, global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return new _FadeForwardsPageTransition__page_transitions_theme(animation: animation, secondaryAnimation: secondaryAnimation, backgroundColor: backgroundColor, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ZoomPageTransitionsBuilder : global::Doroti.Framework.Widgets.PageTransitionsBuilder
{
    public virtual bool allowSnapshotting { get; private set; } = default!;
    public virtual bool allowEnterRouteSnapshotting { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    internal static bool _kProfileForceDisableSnapshotting = false;

    public ZoomPageTransitionsBuilder(bool allowSnapshotting = true, bool allowEnterRouteSnapshotting = true, Color? backgroundColor = null)
    {
        this.allowSnapshotting = allowSnapshotting;
        this.allowEnterRouteSnapshotting = allowEnterRouteSnapshotting;
        this.backgroundColor = backgroundColor;
    }

    public override global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>? delegatedTransition => (context, animation, secondaryAnimation, allowSnapshotting, child) => _snapshotAwareDelegatedTransition(context, animation, secondaryAnimation, child, allowSnapshotting && this.allowSnapshotting, allowEnterRouteSnapshotting, backgroundColor);
    internal static global::Doroti.Framework.Widgets.Widget _snapshotAwareDelegatedTransition(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget? child, bool allowSnapshotting, bool allowEnterRouteSnapshotting, Color? backgroundColor)
    {
        global::Doroti.Ui.Color enterTransitionBackgroundColor = backgroundColor ?? Theme.of(context).colorScheme.surface;
        return new global::Doroti.Framework.Widgets.DualTransitionBuilder(animation: new global::Doroti.Framework.Animation.ReverseAnimation(secondaryAnimation), forwardBuilder: (context, animation, child) =>
        {
            return new _ZoomEnterTransition__page_transitions_theme(animation: animation, allowSnapshotting: allowSnapshotting && allowEnterRouteSnapshotting, reverse: true, backgroundColor: enterTransitionBackgroundColor, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, reverseBuilder: (context, animation, child) =>
        {
            return new _ZoomExitTransition__page_transitions_theme(animation: animation, allowSnapshotting: allowSnapshotting, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildTransitions<T>(global::Doroti.Framework.Widgets.PageRoute<T> route, global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        if (_kProfileForceDisableSnapshotting)
        {
            return new _ZoomPageTransitionNoCache__page_transitions_theme(animation: animation, secondaryAnimation: secondaryAnimation, child: child);
        }
        return new _ZoomPageTransition__page_transitions_theme(animation: animation, secondaryAnimation: secondaryAnimation, allowSnapshotting: allowSnapshotting && route.allowSnapshotting, allowEnterRouteSnapshotting: allowEnterRouteSnapshotting, backgroundColor: backgroundColor, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PageTransitionsTheme : global::Doroti.Framework.Foundation.Diagnosticable
{
    internal static DartMap<global::Doroti.Framework.Foundation.TargetPlatform, global::Doroti.Framework.Widgets.PageTransitionsBuilder> _defaultBuilders = new DartMap<global::Doroti.Framework.Foundation.TargetPlatform, global::Doroti.Framework.Widgets.PageTransitionsBuilder> { [TargetPlatform.android] = new PredictiveBackPageTransitionsBuilder(), [TargetPlatform.iOS] = new CupertinoPageTransitionsBuilder(), [TargetPlatform.macOS] = new CupertinoPageTransitionsBuilder(), [TargetPlatform.windows] = new ZoomPageTransitionsBuilder(), [TargetPlatform.linux] = new ZoomPageTransitionsBuilder() };
    internal virtual DartMap<global::Doroti.Framework.Foundation.TargetPlatform, global::Doroti.Framework.Widgets.PageTransitionsBuilder> _builders { get; private set; } = default!;

    public PageTransitionsTheme(DartMap<global::Doroti.Framework.Foundation.TargetPlatform, global::Doroti.Framework.Widgets.PageTransitionsBuilder> builders = default!)
    {
        DartMap<global::Doroti.Framework.Foundation.TargetPlatform, global::Doroti.Framework.Widgets.PageTransitionsBuilder> __builders = builders ?? _defaultBuilders;
        _builders = __builders;
    }

    public virtual DartMap<global::Doroti.Framework.Foundation.TargetPlatform, global::Doroti.Framework.Widgets.PageTransitionsBuilder> builders => _builders;
    public virtual global::Doroti.Framework.Widgets.Widget buildTransitions<T>(global::Doroti.Framework.Widgets.PageRoute<T> route, global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return new _PageTransitionsThemeTransitions__page_transitions_theme<T>(builders: builders, route: route, animation: animation, secondaryAnimation: secondaryAnimation, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>? delegatedTransition(global::Doroti.Framework.Foundation.TargetPlatform platform)
    {
        global::Doroti.Framework.Widgets.PageTransitionsBuilder matchingBuilder = builders.GetValueOrDefault(platform) ?? new ZoomPageTransitionsBuilder();
        return matchingBuilder.delegatedTransition;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<global::Doroti.Framework.Widgets.PageTransitionsBuilder?> _all(DartMap<global::Doroti.Framework.Foundation.TargetPlatform, global::Doroti.Framework.Widgets.PageTransitionsBuilder> builders)
    {
        return Enum.GetValues<global::Doroti.Framework.Foundation.TargetPlatform>().ToList().map<global::Doroti.Framework.Foundation.TargetPlatform, global::Doroti.Framework.Widgets.PageTransitionsBuilder?>((platform) => builders.GetValueOrDefault(platform)).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as PageTransitionsTheme;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        if ((__other is PageTransitionsTheme) && DartRuntimePrimitives.Identical(builders, __other.builders))
        {
            PageTransitionsTheme other__as28851 = __other;
            return true;
        }
        return (__other is PageTransitionsTheme) && CollectionsLibrary.listEquals<global::Doroti.Framework.Widgets.PageTransitionsBuilder?>(_all(__other.builders), _all(builders));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHashAll(_all(builders)));
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DartMap<global::Doroti.Framework.Foundation.TargetPlatform, global::Doroti.Framework.Widgets.PageTransitionsBuilder>>("builders", builders, defaultValue: _defaultBuilders));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PageTransitionsThemeTransitions__page_transitions_theme<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DartMap<global::Doroti.Framework.Foundation.TargetPlatform, global::Doroti.Framework.Widgets.PageTransitionsBuilder> builders { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.PageRoute<T> route { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _PageTransitionsThemeTransitions__page_transitions_theme(DartMap<global::Doroti.Framework.Foundation.TargetPlatform, global::Doroti.Framework.Widgets.PageTransitionsBuilder> builders, global::Doroti.Framework.Widgets.PageRoute<T> route, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        this.builders = builders;
        this.route = route;
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PageTransitionsThemeTransitionsState__page_transitions_theme<T>());
}

internal class _PageTransitionsThemeTransitionsState__page_transitions_theme<T> : global::Doroti.Framework.Widgets.State<_PageTransitionsThemeTransitions__page_transitions_theme<T>>
{
    internal virtual global::Doroti.Framework.Foundation.TargetPlatform? _transitionPlatform { get; set; } = default;

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Foundation.TargetPlatform platformLocal = Theme.of(context).platform;
        if (widget.route.popGestureInProgress)
        {
            _transitionPlatform ??= platformLocal;
            platformLocal = DartRuntimePrimitives.RequireValue(_transitionPlatform);
        }
        else
        {
            _transitionPlatform = null;
        }
        global::Doroti.Framework.Widgets.PageTransitionsBuilder matchingBuilder = widget.builders.GetValueOrDefault(platformLocal) ?? (platformLocal switch { TargetPlatform.iOS => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.PageTransitionsBuilder>(new CupertinoPageTransitionsBuilder()), TargetPlatform.android or TargetPlatform.fuchsia or TargetPlatform.windows or TargetPlatform.macOS => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.PageTransitionsBuilder>(new ZoomPageTransitionsBuilder()), TargetPlatform.linux => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.PageTransitionsBuilder>(new ZoomPageTransitionsBuilder()), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return matchingBuilder.buildTransitions<T>(widget.route, context, widget.animation, widget.secondaryAnimation, widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Page_transitions_themeLibrary
{
    internal static void _drawImageScaledAndCentered(global::Doroti.Framework.Rendering.PaintingContext context, global::Doroti.Ui.Image image, double scale, double opacity, double pixelRatio)
    {
        if ((scale <= 0.0) || (opacity <= 0.0))
        {
            return;
        }
        var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.filterQuality = FilterQuality.medium;
    __cascade.color = Color.fromRGBO(0L, 0L, 0L, opacity);
    return __cascade;
}))();
        double logicalWidth = image.width / pixelRatio;
        double logicalHeight = image.height / pixelRatio;
        double scaledLogicalWidth = logicalWidth * scale;
        double scaledLogicalHeight = logicalHeight * scale;
        double left = (logicalWidth - scaledLogicalWidth) / 2L;
        double top = (logicalHeight - scaledLogicalHeight) / 2L;
        var dst = Rect.fromLTWH(left, top, scaledLogicalWidth, scaledLogicalHeight);
        context.canvas.drawImageRect(image, Rect.fromLTWH(0, 0, image.width.toDouble(), image.height.toDouble()), dst, paint);
    }
}

public static partial class Page_transitions_themeLibrary
{
    internal static void _updateScaledTransform(Matrix4 transform, double scale, Size size)
    {
        transform.setIdentity();
        if (scale == 1.0)
        {
            return;
        }
        transform.scaleByDouble(scale, scale, scale, 1);
        double dx = (size.width * scale - size.width) / 2L;
        double dy = (size.height * scale - size.height) / 2L;
        transform.translateByDouble(-dx, -dy, 0, 1);
    }
}

internal interface _ZoomTransitionBase__page_transitions_theme<S> where S : global::Doroti.Framework.Widgets.StatefulWidget
{
    global::Doroti.Framework.Widgets.SnapshotController controller { get; }
    global::Doroti.Framework.Animation.Animation<double> fadeTransition { get; set; }
    global::Doroti.Framework.Animation.Animation<double> scaleTransition { get; set; }

    public bool useSnapshot { get; }
    public void onAnimationValueChange();
    public void onAnimationStatusChange(global::Doroti.Framework.Animation.AnimationStatus status);
    public void dispose();
}

public class _ZoomEnterTransitionPainter__page_transitions_theme : global::Doroti.Framework.Widgets.SnapshotPainter
{
    public virtual bool reverse { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> scale { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> fade { get; private set; } = default!;
    public virtual Color backgroundColor { get; private set; } = default!;
    internal virtual Matrix4 _transform { get; private set; } = Matrix4.zero();
    internal virtual global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer> _opacityHandle { get; private set; } = new global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer>();
    internal virtual global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.TransformLayer> _transformHandler { get; private set; } = new global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.TransformLayer>();

    internal _ZoomEnterTransitionPainter__page_transitions_theme(bool reverse, global::Doroti.Framework.Animation.Animation<double> scale, global::Doroti.Framework.Animation.Animation<double> fade, global::Doroti.Framework.Animation.Animation<double> animation, Color backgroundColor)
    {
        this.reverse = reverse;
        this.scale = scale;
        this.fade = fade;
        this.animation = animation;
        this.backgroundColor = backgroundColor;
        this.animation.addListener(notifyListeners);
        this.animation.addStatusListener(_onStatusChange);
        this.scale.addListener(notifyListeners);
        this.fade.addListener(notifyListeners);
    }

    internal virtual void _onStatusChange(global::Doroti.Framework.Animation.AnimationStatus __unused0)
    {
        notifyListeners();
    }

    internal virtual void _drawScrim(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, Size size)
    {
        var scrimOpacity = 0.0;
        if (!reverse && !animation.isCompleted)
        {
            scrimOpacity = DartRuntimePrimitives.RequireValue(_ZoomEnterTransitionState__page_transitions_theme._scrimOpacityTween.evaluate(animation));
        }
        DartRuntimePrimitives.Assert(() => !reverse || (scrimOpacity == 0.0));
        if (scrimOpacity > 0.0)
        {
            context.canvas.drawRect(offset & size, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = backgroundColor.withOpacity(scrimOpacity);
    return __cascade;
}))());
        }
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset> painter)
    {
        if (!animation.isAnimating)
        {
            painter(context, offset);
            return;
        }
        _drawScrim(context, offset, size);
        Page_transitions_themeLibrary._updateScaledTransform(_transform, scale.value, size);
        _transformHandler.layer = context.pushTransform(true, offset, _transform, (context, offset) =>
        {
            _opacityHandle.layer = context.pushOpacity(offset, (fade.value * 255L).round(), painter, oldLayer: _opacityHandle.layer);
        }, oldLayer: _transformHandler.layer);
    }

    public override void paintSnapshot(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::Doroti.Ui.Image image, Size sourceSize, double pixelRatio)
    {
        _drawScrim(context, offset, size);
        Page_transitions_themeLibrary._drawImageScaledAndCentered(context, image, scale.value, fade.value, pixelRatio);
    }

    public override void dispose()
    {
        animation.removeListener(notifyListeners);
        animation.removeStatusListener(_onStatusChange);
        scale.removeListener(notifyListeners);
        fade.removeListener(notifyListeners);
        _opacityHandle.layer = null;
        _transformHandler.layer = null;
        base.dispose();
    }

    public override bool shouldRepaint(global::Doroti.Framework.Widgets.SnapshotPainter oldPainter)
    {
        var __oldDelegate = (_ZoomEnterTransitionPainter__page_transitions_theme)oldPainter;
        return (__oldDelegate.reverse != reverse) || (__oldDelegate.animation.value != animation.value) || (__oldDelegate.scale.value != scale.value) || (__oldDelegate.fade.value != fade.value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ZoomExitTransitionPainter__page_transitions_theme : global::Doroti.Framework.Widgets.SnapshotPainter
{
    public virtual bool reverse { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> scale { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> fade { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    internal virtual Matrix4 _transform { get; private set; } = Matrix4.zero();
    internal virtual global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer> _opacityHandle { get; private set; } = new global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer>();
    internal virtual global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.TransformLayer> _transformHandler { get; private set; } = new global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.TransformLayer>();

    internal _ZoomExitTransitionPainter__page_transitions_theme(bool reverse, global::Doroti.Framework.Animation.Animation<double> scale, global::Doroti.Framework.Animation.Animation<double> fade, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        this.reverse = reverse;
        this.scale = scale;
        this.fade = fade;
        this.animation = animation;
        this.scale.addListener(notifyListeners);
        this.fade.addListener(notifyListeners);
        this.animation.addStatusListener(_onStatusChange);
    }

    internal virtual void _onStatusChange(global::Doroti.Framework.Animation.AnimationStatus __unused0)
    {
        notifyListeners();
    }

    public override void paintSnapshot(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::Doroti.Ui.Image image, Size sourceSize, double pixelRatio)
    {
        Page_transitions_themeLibrary._drawImageScaledAndCentered(context, image, scale.value, fade.value, pixelRatio);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset> painter)
    {
        if (!animation.isAnimating)
        {
            painter(context, offset);
            return;
        }
        Page_transitions_themeLibrary._updateScaledTransform(_transform, scale.value, size);
        _transformHandler.layer = context.pushTransform(true, offset, _transform, (context, offset) =>
        {
            _opacityHandle.layer = context.pushOpacity(offset, (fade.value * 255L).round(), painter, oldLayer: _opacityHandle.layer);
        }, oldLayer: _transformHandler.layer);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Widgets.SnapshotPainter oldPainter)
    {
        var __oldDelegate = (_ZoomExitTransitionPainter__page_transitions_theme)oldPainter;
        return (__oldDelegate.reverse != reverse) || (__oldDelegate.fade.value != fade.value) || (__oldDelegate.scale.value != scale.value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _opacityHandle.layer = null;
        _transformHandler.layer = null;
        scale.removeListener(notifyListeners);
        fade.removeListener(notifyListeners);
        animation.removeStatusListener(_onStatusChange);
        base.dispose();
    }

}

internal class _ZoomPageTransitionNoCache__page_transitions_theme : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    internal _ZoomPageTransitionNoCache__page_transitions_theme(global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.DualTransitionBuilder(animation: animation, forwardBuilder: (context, animation, child) =>
        {
            return new _ZoomEnterTransitionNoCache__page_transitions_theme(animation: animation, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, reverseBuilder: (context, animation, child) =>
        {
            return new _ZoomExitTransitionNoCache__page_transitions_theme(animation: animation, reverse: true, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: new global::Doroti.Framework.Widgets.DualTransitionBuilder(animation: new global::Doroti.Framework.Animation.ReverseAnimation(secondaryAnimation), forwardBuilder: (context, animation, child) =>
        {
            return new _ZoomEnterTransitionNoCache__page_transitions_theme(animation: animation, reverse: true, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, reverseBuilder: (context, animation, child) =>
        {
            return new _ZoomExitTransitionNoCache__page_transitions_theme(animation: animation, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ZoomEnterTransitionNoCache__page_transitions_theme : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual bool reverse { get; private set; } = default!;

    internal _ZoomEnterTransitionNoCache__page_transitions_theme(global::Doroti.Framework.Animation.Animation<double> animation, bool reverse = false, global::Doroti.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.reverse = reverse;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        double opacityLocal = 0;
        if (!reverse && !animation.isCompleted)
        {
            opacityLocal = DartRuntimePrimitives.RequireValue(_ZoomEnterTransitionState__page_transitions_theme._scrimOpacityTween.evaluate(animation));
        }
        global::Doroti.Framework.Animation.Animation<double> fadeTransition = reverse ? AnimationsLibrary.kAlwaysCompleteAnimation : _ZoomEnterTransitionState__page_transitions_theme._fadeInTransition.animate(animation);
        global::Doroti.Framework.Animation.Animation<double> scaleTransition = (reverse ? _ZoomEnterTransitionState__page_transitions_theme._scaleDownTransition : _ZoomEnterTransitionState__page_transitions_theme._scaleUpTransition).animate(animation);
        return new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: animation, builder: (context, child) =>
        {
            return new global::Doroti.Framework.Widgets.ColoredBox(color: Colors.black.withOpacity(opacityLocal), child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeTransition, child: new global::Doroti.Framework.Widgets.ScaleTransition(scale: scaleTransition, filterQuality: FilterQuality.medium, child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ZoomExitTransitionNoCache__page_transitions_theme : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    internal _ZoomExitTransitionNoCache__page_transitions_theme(global::Doroti.Framework.Animation.Animation<double> animation, bool reverse = false, global::Doroti.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.reverse = reverse;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Animation.Animation<double> fadeTransition = reverse ? _ZoomExitTransitionState__page_transitions_theme._fadeOutTransition.animate(animation) : AnimationsLibrary.kAlwaysCompleteAnimation;
        global::Doroti.Framework.Animation.Animation<double> scaleTransition = (reverse ? _ZoomExitTransitionState__page_transitions_theme._scaleDownTransition : _ZoomExitTransitionState__page_transitions_theme._scaleUpTransition).animate(animation);
        return new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeTransition, child: new global::Doroti.Framework.Widgets.ScaleTransition(scale: scaleTransition, filterQuality: FilterQuality.medium, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
