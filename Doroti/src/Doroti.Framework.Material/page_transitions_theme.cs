// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/page_transitions_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

internal class _ZoomPageTransition__page_transitions_theme : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public static List<global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>> fastOutExtraSlowInTweenSequenceItems = new List<global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: 0.4).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Cubic(0.05, 0.0, 0.133333, 0.06))), weight: 0.166666), new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.4, end: 1.0).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Cubic(0.208333, 0.82, 0.25, 1.0))), weight: (1.0 - 0.166666)) };
    internal static global::Doroti.Generated.Framework.Animation.TweenSequence<double> _scaleCurveSequence = new global::Doroti.Generated.Framework.Animation.TweenSequence<double>(fastOutExtraSlowInTweenSequenceItems);
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual bool allowSnapshotting { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual bool allowEnterRouteSnapshotting { get; private set; } = default!;

    internal _ZoomPageTransition__page_transitions_theme(global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, bool allowSnapshotting, bool allowEnterRouteSnapshotting, Color? backgroundColor = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.allowSnapshotting = allowSnapshotting;
        this.allowEnterRouteSnapshotting = allowEnterRouteSnapshotting;
        this.backgroundColor = backgroundColor;
        this.child = child;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color enterTransitionBackgroundColor__4001 = ((global::Doroti.Ui.Color)(object?)(this.backgroundColor ?? Theme.of(context).colorScheme.surface));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.DualTransitionBuilder(animation: this.animation, forwardBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ZoomEnterTransition__page_transitions_theme(animation: animation, allowSnapshotting: (this.allowSnapshotting && this.allowEnterRouteSnapshotting), backgroundColor: enterTransitionBackgroundColor__4001, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), reverseBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ZoomExitTransition__page_transitions_theme(animation: animation, allowSnapshotting: this.allowSnapshotting, reverse: true, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: ZoomPageTransitionsBuilder._snapshotAwareDelegatedTransition(context, this.animation, this.secondaryAnimation, this.child, this.allowSnapshotting, this.allowEnterRouteSnapshotting, enterTransitionBackgroundColor__4001)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ZoomEnterTransition__page_transitions_theme : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual bool allowSnapshotting { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual Color backgroundColor { get; private set; } = default!;

    internal _ZoomEnterTransition__page_transitions_theme(global::Doroti.Generated.Framework.Animation.Animation<double> animation, bool reverse = false, bool allowSnapshotting = default!, Color backgroundColor = default!, global::Doroti.Generated.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.reverse = reverse;
        this.allowSnapshotting = allowSnapshotting;
        this.backgroundColor = backgroundColor;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ZoomEnterTransitionState__page_transitions_theme());
}

internal class _ZoomEnterTransitionState__page_transitions_theme : global::Doroti.Generated.Framework.Widgets.State<_ZoomEnterTransition__page_transitions_theme>, _ZoomTransitionBase__page_transitions_theme<_ZoomEnterTransition__page_transitions_theme>
{
    public virtual _ZoomEnterTransitionPainter__page_transitions_theme @delegate { get; set; } = default!;
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _fadeInTransition = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval(0.125, 0.25)));
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _scaleDownTransition = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 1.1, end: 1.0).chain(_ZoomPageTransition__page_transitions_theme._scaleCurveSequence);
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _scaleUpTransition = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.85, end: 1.0).chain(_ZoomPageTransition__page_transitions_theme._scaleCurveSequence);
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double?> _scrimOpacityTween = new global::Doroti.Generated.Framework.Animation.Tween<double?>(begin: 0.0, end: 0.6).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval(0.2075, 0.4175)));
    public virtual global::Doroti.Generated.Framework.Widgets.SnapshotController controller { get; set; } = new global::Doroti.Generated.Framework.Widgets.SnapshotController();
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> fadeTransition { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> scaleTransition { get; set; } = default!;

    public virtual bool useSnapshot => DartRuntimePrimitives.ConvertValue<bool>((!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb && ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).allowSnapshotting));
    internal virtual void _updateAnimations()
    {
        fadeTransition = (((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).reverse ? global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation : _fadeInTransition.animate(((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).animation));
        scaleTransition = ((((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).reverse ? _scaleDownTransition : _scaleUpTransition)).animate(((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).animation);
        ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).animation.addListener(() => this.onAnimationValueChange());
        ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).animation.addStatusListener((AnimationStatusListener)this.onAnimationStatusChange);
    }

    public override void initState()
    {
        _updateAnimations();
        @delegate = new _ZoomEnterTransitionPainter__page_transitions_theme(reverse: ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).reverse, fade: this.fadeTransition, scale: this.scaleTransition, animation: ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).animation, backgroundColor: ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).backgroundColor);
        base.initState();
    }

    public override void didUpdateWidget(_ZoomEnterTransition__page_transitions_theme oldWidget)
    {
        if (((((_ZoomEnterTransition__page_transitions_theme)oldWidget).reverse != ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).reverse) || (!object.Equals(((_ZoomEnterTransition__page_transitions_theme)oldWidget).animation, ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).animation))))
        {
            ((_ZoomEnterTransition__page_transitions_theme)oldWidget).animation.removeListener(() => this.onAnimationValueChange());
            ((_ZoomEnterTransition__page_transitions_theme)oldWidget).animation.removeStatusListener((AnimationStatusListener)this.onAnimationStatusChange);
            _updateAnimations();
            this.@delegate.dispose();
            @delegate = new _ZoomEnterTransitionPainter__page_transitions_theme(reverse: ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).reverse, fade: this.fadeTransition, scale: this.scaleTransition, animation: ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).animation, backgroundColor: ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).backgroundColor);
        }
        base.didUpdateWidget(oldWidget);
    }

    public override void dispose()
    {
        ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).animation.removeListener(() => this.onAnimationValueChange());
        ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).animation.removeStatusListener((AnimationStatusListener)this.onAnimationStatusChange);
        this.@delegate.dispose();
        this.controller.dispose();
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SnapshotWidget(painter: this.@delegate, controller: this.controller, mode: global::Doroti.Generated.Framework.Widgets.SnapshotMode.permissive, autoresize: true, child: ((_ZoomEnterTransition__page_transitions_theme)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void onAnimationValueChange()
    {
        if ((((((global::Doroti.Generated.Framework.Animation.Animation<double>)this.scaleTransition).value == 1.0)) && (((((global::Doroti.Generated.Framework.Animation.Animation<double>)this.fadeTransition).value == 0.0) || (((global::Doroti.Generated.Framework.Animation.Animation<double>)this.fadeTransition).value == 1.0)))))
        {
            this.controller.allowSnapshotting = false;
        }
        else
        {
            this.controller.allowSnapshotting = this.useSnapshot;
        }
    }

    public virtual void onAnimationStatusChange(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
    {
        this.controller.allowSnapshotting = (global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isAnimating(status) && this.useSnapshot);
    }

}

public class _ZoomExitTransition__page_transitions_theme : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual bool allowSnapshotting { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }

    internal _ZoomExitTransition__page_transitions_theme(global::Doroti.Generated.Framework.Animation.Animation<double> animation, bool reverse = false, bool allowSnapshotting = default!, global::Doroti.Generated.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.reverse = reverse;
        this.allowSnapshotting = allowSnapshotting;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ZoomExitTransitionState__page_transitions_theme());
}

internal class _ZoomExitTransitionState__page_transitions_theme : global::Doroti.Generated.Framework.Widgets.State<_ZoomExitTransition__page_transitions_theme>, _ZoomTransitionBase__page_transitions_theme<_ZoomExitTransition__page_transitions_theme>
{
    public virtual _ZoomExitTransitionPainter__page_transitions_theme @delegate { get; set; } = default!;
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _fadeOutTransition = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval(0.0825, 0.2075)));
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _scaleUpTransition = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 1.0, end: 1.05).chain(_ZoomPageTransition__page_transitions_theme._scaleCurveSequence);
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _scaleDownTransition = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 1.0, end: 0.9).chain(_ZoomPageTransition__page_transitions_theme._scaleCurveSequence);
    public virtual global::Doroti.Generated.Framework.Widgets.SnapshotController controller { get; set; } = new global::Doroti.Generated.Framework.Widgets.SnapshotController();
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> fadeTransition { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> scaleTransition { get; set; } = default!;

    public virtual bool useSnapshot => DartRuntimePrimitives.ConvertValue<bool>((!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb && ((_ZoomExitTransition__page_transitions_theme)(object)this.widget).allowSnapshotting));
    internal virtual void _updateAnimations()
    {
        fadeTransition = (((_ZoomExitTransition__page_transitions_theme)(object)this.widget).reverse ? _fadeOutTransition.animate(((_ZoomExitTransition__page_transitions_theme)(object)this.widget).animation) : global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation);
        scaleTransition = ((((_ZoomExitTransition__page_transitions_theme)(object)this.widget).reverse ? _scaleDownTransition : _scaleUpTransition)).animate(((_ZoomExitTransition__page_transitions_theme)(object)this.widget).animation);
        ((_ZoomExitTransition__page_transitions_theme)(object)this.widget).animation.addListener(() => this.onAnimationValueChange());
        ((_ZoomExitTransition__page_transitions_theme)(object)this.widget).animation.addStatusListener((AnimationStatusListener)this.onAnimationStatusChange);
    }

    public override void initState()
    {
        _updateAnimations();
        @delegate = new _ZoomExitTransitionPainter__page_transitions_theme(reverse: ((_ZoomExitTransition__page_transitions_theme)(object)this.widget).reverse, fade: this.fadeTransition, scale: this.scaleTransition, animation: ((_ZoomExitTransition__page_transitions_theme)(object)this.widget).animation);
        base.initState();
    }

    public override void didUpdateWidget(_ZoomExitTransition__page_transitions_theme oldWidget)
    {
        if (((((_ZoomExitTransition__page_transitions_theme)oldWidget).reverse != ((_ZoomExitTransition__page_transitions_theme)(object)this.widget).reverse) || (!object.Equals(((_ZoomExitTransition__page_transitions_theme)oldWidget).animation, ((_ZoomExitTransition__page_transitions_theme)(object)this.widget).animation))))
        {
            ((_ZoomExitTransition__page_transitions_theme)oldWidget).animation.removeListener(() => this.onAnimationValueChange());
            ((_ZoomExitTransition__page_transitions_theme)oldWidget).animation.removeStatusListener((AnimationStatusListener)this.onAnimationStatusChange);
            _updateAnimations();
            this.@delegate.dispose();
            @delegate = new _ZoomExitTransitionPainter__page_transitions_theme(reverse: ((_ZoomExitTransition__page_transitions_theme)(object)this.widget).reverse, fade: this.fadeTransition, scale: this.scaleTransition, animation: ((_ZoomExitTransition__page_transitions_theme)(object)this.widget).animation);
        }
        base.didUpdateWidget(oldWidget);
    }

    public override void dispose()
    {
        ((_ZoomExitTransition__page_transitions_theme)(object)this.widget).animation.removeListener(() => this.onAnimationValueChange());
        ((_ZoomExitTransition__page_transitions_theme)(object)this.widget).animation.removeStatusListener((AnimationStatusListener)this.onAnimationStatusChange);
        this.@delegate.dispose();
        this.controller.dispose();
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SnapshotWidget(painter: this.@delegate, controller: this.controller, mode: global::Doroti.Generated.Framework.Widgets.SnapshotMode.permissive, autoresize: true, child: ((_ZoomExitTransition__page_transitions_theme)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void onAnimationValueChange()
    {
        if ((((((global::Doroti.Generated.Framework.Animation.Animation<double>)this.scaleTransition).value == 1.0)) && (((((global::Doroti.Generated.Framework.Animation.Animation<double>)this.fadeTransition).value == 0.0) || (((global::Doroti.Generated.Framework.Animation.Animation<double>)this.fadeTransition).value == 1.0)))))
        {
            this.controller.allowSnapshotting = false;
        }
        else
        {
            this.controller.allowSnapshotting = this.useSnapshot;
        }
    }

    public virtual void onAnimationStatusChange(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
    {
        this.controller.allowSnapshotting = (global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isAnimating(status) && this.useSnapshot);
    }

}

internal class _FadeForwardsPageTransition__page_transitions_theme : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    internal static global::Doroti.Generated.Framework.Animation.Animatable<Offset> _forwardTranslationTween = new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.25, 0.0), end: Offset.zero).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: FadeForwardsPageTransitionsBuilder._transitionCurve));
    internal static global::Doroti.Generated.Framework.Animation.Animatable<Offset> _backwardTranslationTween = new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: new global::Doroti.Ui.Offset(0.25, 0.0)).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: FadeForwardsPageTransitionsBuilder._transitionCurve));

    internal _FadeForwardsPageTransition__page_transitions_theme(global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, Color? backgroundColor = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.backgroundColor = backgroundColor;
        this.child = child;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.DualTransitionBuilder(animation: this.animation, forwardBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: FadeForwardsPageTransitionsBuilder._fadeInTransition.animate(animation), child: new global::Doroti.Generated.Framework.Widgets.SlideTransition(position: _forwardTranslationTween.animate(animation), child: child)));
throw new InvalidOperationException("Dart closure completed without a value.");
})), reverseBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.IgnorePointer(ignoring: (object.Equals(((global::Doroti.Generated.Framework.Animation.Animation<double>)animation).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward)), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: FadeForwardsPageTransitionsBuilder._fadeOutTransition.animate(animation), child: new global::Doroti.Generated.Framework.Widgets.SlideTransition(position: _backwardTranslationTween.animate(animation), child: child))));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: FadeForwardsPageTransitionsBuilder._delegatedTransition(context, this.secondaryAnimation, this.backgroundColor, this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FadeForwardsPageTransitionsBuilder : global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder
{
    public virtual Color? backgroundColor { get; private set; }
    public const long kTransitionMilliseconds = 450L;
    internal static global::Doroti.Generated.Framework.Animation.Curve _transitionCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)global::Doroti.Generated.Framework.Animation.Curves.easeInOutCubicEmphasized);
    internal static global::Doroti.Generated.Framework.Animation.Animatable<Offset> _secondaryBackwardTranslationTween = new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: new global::Doroti.Ui.Offset(-0.25, 0.0)).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: _transitionCurve));
    internal static global::Doroti.Generated.Framework.Animation.Animatable<Offset> _secondaryForwardTranslationTween = new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(-0.25, 0.0), end: Offset.zero).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: _transitionCurve));
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _fadeInTransition = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval(0.0, 0.75)));
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _fadeOutTransition = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval(0.0, 0.25)));

    public FadeForwardsPageTransitionsBuilder(Color? backgroundColor = null)
    {
        this.backgroundColor = backgroundColor;
    }

    public override Duration transitionDuration => Duration.Create(milliseconds: kTransitionMilliseconds);
    public override global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?>? delegatedTransition => ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?>?)((context, animation, secondaryAnimation, allowSnapshotting, child) => FadeForwardsPageTransitionsBuilder._delegatedTransition(context, secondaryAnimation, this.backgroundColor, child)));
    internal static global::Doroti.Generated.Framework.Widgets.Widget _delegatedTransition(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, Color? backgroundColor, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        global::Doroti.Generated.Framework.Widgets.Widget builder__17626 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.DualTransitionBuilder(animation: new global::Doroti.Generated.Framework.Animation.ReverseAnimation(secondaryAnimation), forwardBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: _fadeInTransition.animate(animation), child: new global::Doroti.Generated.Framework.Widgets.SlideTransition(position: _secondaryForwardTranslationTween.animate(animation), child: child)));
throw new InvalidOperationException("Dart closure completed without a value.");
})), reverseBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: _fadeOutTransition.animate(animation), child: new global::Doroti.Generated.Framework.Widgets.SlideTransition(position: _secondaryBackwardTranslationTween.animate(animation), child: child)));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: child));
        bool isOpaque__18455 = (global::Doroti.Generated.Framework.Widgets.ModalRoute<object>.opaqueOf(context) ?? true);
        if (!isOpaque__18455)
        {
            return builder__17626;
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: (((global::Doroti.Generated.Framework.Animation.Animation<double>)secondaryAnimation).isAnimating ? (backgroundColor ?? ColorScheme.of(context).surface) : Colors.transparent), child: builder__17626));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildTransitions<T>(global::Doroti.Generated.Framework.Widgets.PageRoute<T> route, global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _FadeForwardsPageTransition__page_transitions_theme(animation: animation, secondaryAnimation: secondaryAnimation, backgroundColor: this.backgroundColor, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ZoomPageTransitionsBuilder : global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder
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

    public override global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?>? delegatedTransition => ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?>?)((context, animation, secondaryAnimation, allowSnapshotting, child) => ZoomPageTransitionsBuilder._snapshotAwareDelegatedTransition(context, animation, secondaryAnimation, child, (allowSnapshotting && this.allowSnapshotting), this.allowEnterRouteSnapshotting, this.backgroundColor)));
    internal static global::Doroti.Generated.Framework.Widgets.Widget _snapshotAwareDelegatedTransition(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget? child, bool allowSnapshotting, bool allowEnterRouteSnapshotting, Color? backgroundColor)
    {
        global::Doroti.Ui.Color enterTransitionBackgroundColor__23169 = ((global::Doroti.Ui.Color)(object?)(backgroundColor ?? Theme.of(context).colorScheme.surface));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.DualTransitionBuilder(animation: new global::Doroti.Generated.Framework.Animation.ReverseAnimation(secondaryAnimation), forwardBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ZoomEnterTransition__page_transitions_theme(animation: animation, allowSnapshotting: (allowSnapshotting && allowEnterRouteSnapshotting), reverse: true, backgroundColor: enterTransitionBackgroundColor__23169, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), reverseBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ZoomExitTransition__page_transitions_theme(animation: animation, allowSnapshotting: allowSnapshotting, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildTransitions<T>(global::Doroti.Generated.Framework.Widgets.PageRoute<T> route, global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        if (_kProfileForceDisableSnapshotting)
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ZoomPageTransitionNoCache__page_transitions_theme(animation: animation, secondaryAnimation: secondaryAnimation, child: child));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ZoomPageTransition__page_transitions_theme(animation: animation, secondaryAnimation: secondaryAnimation, allowSnapshotting: (this.allowSnapshotting && ((global::Doroti.Generated.Framework.Widgets.PageRoute<T>)route).allowSnapshotting), allowEnterRouteSnapshotting: this.allowEnterRouteSnapshotting, backgroundColor: this.backgroundColor, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PageTransitionsTheme : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    internal static DartMap<global::Doroti.Generated.Framework.Foundation.TargetPlatform, global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder> _defaultBuilders = new DartMap<global::Doroti.Generated.Framework.Foundation.TargetPlatform, global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder> { [global::Doroti.Generated.Framework.Foundation.TargetPlatform.android] = ((global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder)(object?)new PredictiveBackPageTransitionsBuilder()), [global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS] = ((global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder)(object?)new CupertinoPageTransitionsBuilder()), [global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS] = ((global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder)(object?)new CupertinoPageTransitionsBuilder()), [global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows] = ((global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder)(object?)new ZoomPageTransitionsBuilder()), [global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux] = ((global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder)(object?)new ZoomPageTransitionsBuilder()) };
    internal virtual DartMap<global::Doroti.Generated.Framework.Foundation.TargetPlatform, global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder> _builders { get; private set; } = default!;

    public PageTransitionsTheme(DartMap<global::Doroti.Generated.Framework.Foundation.TargetPlatform, global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder> builders = default!)
    {
        DartMap<global::Doroti.Generated.Framework.Foundation.TargetPlatform, global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder> __builders = builders ?? _defaultBuilders;
        this._builders = __builders;
    }

    public virtual DartMap<global::Doroti.Generated.Framework.Foundation.TargetPlatform, global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder> builders => this._builders;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget buildTransitions<T>(global::Doroti.Generated.Framework.Widgets.PageRoute<T> route, global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _PageTransitionsThemeTransitions__page_transitions_theme<T>(builders: this.builders, route: route, animation: animation, secondaryAnimation: secondaryAnimation, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?>? delegatedTransition(global::Doroti.Generated.Framework.Foundation.TargetPlatform platform)
    {
        global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder matchingBuilder__28223 = (this.builders.GetValueOrDefault(platform) ?? new ZoomPageTransitionsBuilder());
        return ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?>)((global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder)matchingBuilder__28223).delegatedTransition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder?> _all(DartMap<global::Doroti.Generated.Framework.Foundation.TargetPlatform, global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder> builders)
    {
        return System.Enum.GetValues<global::Doroti.Generated.Framework.Foundation.TargetPlatform>().ToList().map<global::Doroti.Generated.Framework.Foundation.TargetPlatform, global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder?>(((platform) => builders.GetValueOrDefault(platform))).ToList();
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        if (((__other is PageTransitionsTheme) && DartRuntimePrimitives.Identical(this.builders, ((PageTransitionsTheme)((PageTransitionsTheme)__other)).builders)))
        {
            PageTransitionsTheme other__as28851 = (PageTransitionsTheme)__other;
            return true;
        }
        return ((__other is PageTransitionsTheme) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder?>(_all(((PageTransitionsTheme)((PageTransitionsTheme)__other)).builders), _all(this.builders)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHashAll(_all(this.builders)));
    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<DartMap<global::Doroti.Generated.Framework.Foundation.TargetPlatform, global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder>>("builders", this.builders, defaultValue: PageTransitionsTheme._defaultBuilders));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PageTransitionsThemeTransitions__page_transitions_theme<T> : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual DartMap<global::Doroti.Generated.Framework.Foundation.TargetPlatform, global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder> builders { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.PageRoute<T> route { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _PageTransitionsThemeTransitions__page_transitions_theme(DartMap<global::Doroti.Generated.Framework.Foundation.TargetPlatform, global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder> builders, global::Doroti.Generated.Framework.Widgets.PageRoute<T> route, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        this.builders = builders;
        this.route = route;
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PageTransitionsThemeTransitionsState__page_transitions_theme<T>());
}

internal class _PageTransitionsThemeTransitionsState__page_transitions_theme<T> : global::Doroti.Generated.Framework.Widgets.State<_PageTransitionsThemeTransitions__page_transitions_theme<T>>
{
    internal virtual global::Doroti.Generated.Framework.Foundation.TargetPlatform? _transitionPlatform { get; set; } = default;

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Foundation.TargetPlatform platform__30280 = Theme.of(context).platform;
        if (((_PageTransitionsThemeTransitions__page_transitions_theme<T>)(object)this.widget).route.popGestureInProgress)
        {
            _transitionPlatform ??= platform__30280;
            platform__30280 = DartRuntimePrimitives.RequireValue(this._transitionPlatform);
        }
        else
        {
            _transitionPlatform = null;
        }
        global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder matchingBuilder__30689 = (((_PageTransitionsThemeTransitions__page_transitions_theme<T>)(object)this.widget).builders.GetValueOrDefault(platform__30280) ?? (platform__30280 switch { global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder>(new CupertinoPageTransitionsBuilder()), global::Doroti.Generated.Framework.Foundation.TargetPlatform.android or global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows or global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder>(new ZoomPageTransitionsBuilder()), global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder>(new ZoomPageTransitionsBuilder()), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)matchingBuilder__30689.buildTransitions<T>(((_PageTransitionsThemeTransitions__page_transitions_theme<T>)(object)this.widget).route, context, ((_PageTransitionsThemeTransitions__page_transitions_theme<T>)(object)this.widget).animation, ((_PageTransitionsThemeTransitions__page_transitions_theme<T>)(object)this.widget).secondaryAnimation, ((_PageTransitionsThemeTransitions__page_transitions_theme<T>)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Page_transitions_themeLibrary
{
    internal static void _drawImageScaledAndCentered(global::Doroti.Generated.Framework.Rendering.PaintingContext context, global::Doroti.Ui.Image image, double scale, double opacity, double pixelRatio)
    {
        if (((scale <= 0.0) || (opacity <= 0.0)))
        {
            return;
        }
        var paint__31543 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.filterQuality = FilterQuality.medium;
            __cascade.color = global::Doroti.Ui.Color.fromRGBO(0L, 0L, 0L, opacity);
            return __cascade;        }))();
        double logicalWidth__31668 = (image.width / pixelRatio);
        double logicalHeight__31724 = (image.height / pixelRatio);
        double scaledLogicalWidth__31782 = (logicalWidth__31668 * scale);
        double scaledLogicalHeight__31840 = (logicalHeight__31724 * scale);
        double left__31900 = (((logicalWidth__31668 - scaledLogicalWidth__31782)) / 2L);
        double top__31963 = (((logicalHeight__31724 - scaledLogicalHeight__31840)) / 2L);
        var dst__32020 = global::Doroti.Ui.Rect.fromLTWH(left__31900, top__31963, scaledLogicalWidth__31782, scaledLogicalHeight__31840);
        ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawImageRect(image, global::Doroti.Ui.Rect.fromLTWH(0, 0, image.width.toDouble(), image.height.toDouble()), dst__32020, paint__31543);
    }
}

public static partial class Page_transitions_themeLibrary
{
    internal static void _updateScaledTransform(Matrix4 transform, double scale, Size size)
    {
        transform.setIdentity();
        if ((scale == 1.0))
        {
            return;
        }
        transform.scaleByDouble(scale, scale, scale, 1);
        double dx__32443 = (((((size.width * scale)) - size.width)) / 2L);
        double dy__32504 = (((((size.height * scale)) - size.height)) / 2L);
        transform.translateByDouble(-dx__32443, -dy__32504, 0, 1);
    }
}

internal interface _ZoomTransitionBase__page_transitions_theme<S> where S : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    global::Doroti.Generated.Framework.Widgets.SnapshotController controller { get; }
    global::Doroti.Generated.Framework.Animation.Animation<double> fadeTransition { get; set; }
    global::Doroti.Generated.Framework.Animation.Animation<double> scaleTransition { get; set; }

    public bool useSnapshot { get; }
    public void onAnimationValueChange();
    public void onAnimationStatusChange(global::Doroti.Generated.Framework.Animation.AnimationStatus status);
    public void dispose();
}

public class _ZoomEnterTransitionPainter__page_transitions_theme : global::Doroti.Generated.Framework.Widgets.SnapshotPainter
{
    public virtual bool reverse { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> scale { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> fade { get; private set; } = default!;
    public virtual Color backgroundColor { get; private set; } = default!;
    internal virtual Matrix4 _transform { get; private set; } = Matrix4.zero();
    internal virtual global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.OpacityLayer> _opacityHandle { get; private set; } = new global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.OpacityLayer>();
    internal virtual global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.TransformLayer> _transformHandler { get; private set; } = new global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.TransformLayer>();

    internal _ZoomEnterTransitionPainter__page_transitions_theme(bool reverse, global::Doroti.Generated.Framework.Animation.Animation<double> scale, global::Doroti.Generated.Framework.Animation.Animation<double> fade, global::Doroti.Generated.Framework.Animation.Animation<double> animation, Color backgroundColor)
    {
        this.reverse = reverse;
        this.scale = scale;
        this.fade = fade;
        this.animation = animation;
        this.backgroundColor = backgroundColor;
        this.animation.addListener(() => this.notifyListeners());
        this.animation.addStatusListener((AnimationStatusListener)this._onStatusChange);
        this.scale.addListener(() => this.notifyListeners());
        this.fade.addListener(() => this.notifyListeners());
    }

    internal virtual void _onStatusChange(global::Doroti.Generated.Framework.Animation.AnimationStatus __unused0)
    {
        notifyListeners();
    }

    internal virtual void _drawScrim(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset, Size size)
    {
        var scrimOpacity__34468 = 0.0;
        if ((!this.reverse && !((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).isCompleted))
        {
            scrimOpacity__34468 = DartRuntimePrimitives.RequireValue(_ZoomEnterTransitionState__page_transitions_theme._scrimOpacityTween.evaluate(this.animation));
        }
        DartRuntimePrimitives.Assert(() => (!this.reverse || (scrimOpacity__34468 == 0.0)));
        if ((scrimOpacity__34468 > 0.0))
        {
            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRect((offset & size), ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.backgroundColor.withOpacity(scrimOpacity__34468);
            return __cascade;        }))());
        }
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset> painter)
    {
        if (!((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).isAnimating)
        {
            painter(context, offset);
            return;
        }
        _drawScrim(context, offset, size);
        Page_transitions_themeLibrary._updateScaledTransform(this._transform, ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.scale).value, size);
        this._transformHandler.layer = context.pushTransform(true, offset, this._transform, ((global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset>)((context, offset) => {
this._opacityHandle.layer = context.pushOpacity(offset, ((((global::Doroti.Generated.Framework.Animation.Animation<double>)this.fade).value * 255L)).round(), (global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset>)painter, oldLayer: ((global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.OpacityLayer>)this._opacityHandle).layer);
})), oldLayer: ((global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.TransformLayer>)this._transformHandler).layer);
    }

    public override void paintSnapshot(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::Doroti.Ui.Image image, Size sourceSize, double pixelRatio)
    {
        _drawScrim(context, offset, size);
        Page_transitions_themeLibrary._drawImageScaledAndCentered(context, image, ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.scale).value, ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.fade).value, pixelRatio);
    }

    public virtual void dispose()
    {
        this.animation.removeListener(() => this.notifyListeners());
        this.animation.removeStatusListener((AnimationStatusListener)this._onStatusChange);
        this.scale.removeListener(() => this.notifyListeners());
        this.fade.removeListener(() => this.notifyListeners());
        this._opacityHandle.layer = null;
        this._transformHandler.layer = null;
        base.dispose();
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Widgets.SnapshotPainter oldPainter)
    {
        var __oldDelegate = (_ZoomEnterTransitionPainter__page_transitions_theme)(object)oldPainter;
        return ((((((_ZoomEnterTransitionPainter__page_transitions_theme)__oldDelegate).reverse != this.reverse) || (((_ZoomEnterTransitionPainter__page_transitions_theme)__oldDelegate).animation.value != ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).value)) || (((_ZoomEnterTransitionPainter__page_transitions_theme)__oldDelegate).scale.value != ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.scale).value)) || (((_ZoomEnterTransitionPainter__page_transitions_theme)__oldDelegate).fade.value != ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.fade).value));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ZoomExitTransitionPainter__page_transitions_theme : global::Doroti.Generated.Framework.Widgets.SnapshotPainter
{
    public virtual bool reverse { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> scale { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> fade { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    internal virtual Matrix4 _transform { get; private set; } = Matrix4.zero();
    internal virtual global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.OpacityLayer> _opacityHandle { get; private set; } = new global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.OpacityLayer>();
    internal virtual global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.TransformLayer> _transformHandler { get; private set; } = new global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.TransformLayer>();

    internal _ZoomExitTransitionPainter__page_transitions_theme(bool reverse, global::Doroti.Generated.Framework.Animation.Animation<double> scale, global::Doroti.Generated.Framework.Animation.Animation<double> fade, global::Doroti.Generated.Framework.Animation.Animation<double> animation)
    {
        this.reverse = reverse;
        this.scale = scale;
        this.fade = fade;
        this.animation = animation;
        this.scale.addListener(() => this.notifyListeners());
        this.fade.addListener(() => this.notifyListeners());
        this.animation.addStatusListener((AnimationStatusListener)this._onStatusChange);
    }

    internal virtual void _onStatusChange(global::Doroti.Generated.Framework.Animation.AnimationStatus __unused0)
    {
        notifyListeners();
    }

    public override void paintSnapshot(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::Doroti.Ui.Image image, Size sourceSize, double pixelRatio)
    {
        Page_transitions_themeLibrary._drawImageScaledAndCentered(context, image, ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.scale).value, ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.fade).value, pixelRatio);
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset> painter)
    {
        if (!((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).isAnimating)
        {
            painter(context, offset);
            return;
        }
        Page_transitions_themeLibrary._updateScaledTransform(this._transform, ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.scale).value, size);
        this._transformHandler.layer = context.pushTransform(true, offset, this._transform, ((global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset>)((context, offset) => {
this._opacityHandle.layer = context.pushOpacity(offset, ((((global::Doroti.Generated.Framework.Animation.Animation<double>)this.fade).value * 255L)).round(), (global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset>)painter, oldLayer: ((global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.OpacityLayer>)this._opacityHandle).layer);
})), oldLayer: ((global::Doroti.Generated.Framework.Rendering.LayerHandle<global::Doroti.Generated.Framework.Rendering.TransformLayer>)this._transformHandler).layer);
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Widgets.SnapshotPainter oldPainter)
    {
        var __oldDelegate = (_ZoomExitTransitionPainter__page_transitions_theme)(object)oldPainter;
        return (((((_ZoomExitTransitionPainter__page_transitions_theme)__oldDelegate).reverse != this.reverse) || (((_ZoomExitTransitionPainter__page_transitions_theme)__oldDelegate).fade.value != ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.fade).value)) || (((_ZoomExitTransitionPainter__page_transitions_theme)__oldDelegate).scale.value != ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.scale).value));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        this._opacityHandle.layer = null;
        this._transformHandler.layer = null;
        this.scale.removeListener(() => this.notifyListeners());
        this.fade.removeListener(() => this.notifyListeners());
        this.animation.removeStatusListener((AnimationStatusListener)this._onStatusChange);
        base.dispose();
    }

}

internal class _ZoomPageTransitionNoCache__page_transitions_theme : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }

    internal _ZoomPageTransitionNoCache__page_transitions_theme(global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.child = child;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.DualTransitionBuilder(animation: this.animation, forwardBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ZoomEnterTransitionNoCache__page_transitions_theme(animation: animation, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), reverseBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ZoomExitTransitionNoCache__page_transitions_theme(animation: animation, reverse: true, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: new global::Doroti.Generated.Framework.Widgets.DualTransitionBuilder(animation: new global::Doroti.Generated.Framework.Animation.ReverseAnimation(this.secondaryAnimation), forwardBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ZoomEnterTransitionNoCache__page_transitions_theme(animation: animation, reverse: true, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), reverseBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ZoomExitTransitionNoCache__page_transitions_theme(animation: animation, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ZoomEnterTransitionNoCache__page_transitions_theme : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual bool reverse { get; private set; } = default!;

    internal _ZoomEnterTransitionNoCache__page_transitions_theme(global::Doroti.Generated.Framework.Animation.Animation<double> animation, bool reverse = false, global::Doroti.Generated.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.reverse = reverse;
        this.child = child;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        double opacity__41956 = 0;
        if ((!this.reverse && !((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).isCompleted))
        {
            opacity__41956 = DartRuntimePrimitives.RequireValue(_ZoomEnterTransitionState__page_transitions_theme._scrimOpacityTween.evaluate(this.animation));
        }
        global::Doroti.Generated.Framework.Animation.Animation<double> fadeTransition__42705 = (this.reverse ? global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation : _ZoomEnterTransitionState__page_transitions_theme._fadeInTransition.animate(this.animation));
        global::Doroti.Generated.Framework.Animation.Animation<double> scaleTransition__42868 = ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)((this.reverse ? _ZoomEnterTransitionState__page_transitions_theme._scaleDownTransition : _ZoomEnterTransitionState__page_transitions_theme._scaleUpTransition)).animate(this.animation));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: this.animation, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: Colors.black.withOpacity(opacity__41956), child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeTransition__42705, child: new global::Doroti.Generated.Framework.Widgets.ScaleTransition(scale: scaleTransition__42868, filterQuality: FilterQuality.medium, child: this.child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ZoomExitTransitionNoCache__page_transitions_theme : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }

    internal _ZoomExitTransitionNoCache__page_transitions_theme(global::Doroti.Generated.Framework.Animation.Animation<double> animation, bool reverse = false, global::Doroti.Generated.Framework.Widgets.Widget? child = null)
    {
        this.animation = animation;
        this.reverse = reverse;
        this.child = child;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Animation.Animation<double> fadeTransition__43821 = (this.reverse ? _ZoomExitTransitionState__page_transitions_theme._fadeOutTransition.animate(this.animation) : global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation);
        global::Doroti.Generated.Framework.Animation.Animation<double> scaleTransition__43983 = ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)((this.reverse ? _ZoomExitTransitionState__page_transitions_theme._scaleDownTransition : _ZoomExitTransitionState__page_transitions_theme._scaleUpTransition)).animate(this.animation));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeTransition__43821, child: new global::Doroti.Generated.Framework.Widgets.ScaleTransition(scale: scaleTransition__43983, filterQuality: FilterQuality.medium, child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
