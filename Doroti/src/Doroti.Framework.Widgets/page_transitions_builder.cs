// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/page_transitions_builder.dart
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

namespace Doroti.Framework.Widgets;

public abstract class PageTransitionsBuilder
{
    protected PageTransitionsBuilder()
    {
    }

    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, Widget?, Widget?>? delegatedTransition => DartRuntimePrimitives.ConvertValue<global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, Widget?, Widget?>>(null);
    public virtual Duration transitionDuration => Duration.Create(milliseconds: 300L);
    public virtual Duration reverseTransitionDuration => this.transitionDuration;
    public abstract Widget buildTransitions<T>(PageRoute<T> route, BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, Widget child);
}

internal class _FadeUpwardsPageTransition__page_transitions_builder : StatelessWidget
{
    internal static global::Doroti.Framework.Animation.Tween<Offset> _bottomUpTween = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.0, 0.25), end: Offset.zero);
    internal static global::Doroti.Framework.Animation.Animatable<double> _fastOutSlowInTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn));
    internal static global::Doroti.Framework.Animation.Animatable<double> _easeInTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.easeIn));
    internal virtual global::Doroti.Framework.Animation.Animation<Offset> _positionAnimation { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _opacityAnimation { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _FadeUpwardsPageTransition__page_transitions_builder(global::Doroti.Framework.Animation.Animation<double> routeAnimation, Widget child)
    {
        this.child = child;
        this._positionAnimation = routeAnimation.drive(_bottomUpTween.chain(_fastOutSlowInTween));
        this._opacityAnimation = routeAnimation.drive(_easeInTween);
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new SlideTransition(position: this._positionAnimation, child: new FadeTransition(opacity: this._opacityAnimation, child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FadeUpwardsPageTransitionsBuilder : PageTransitionsBuilder
{
    public FadeUpwardsPageTransitionsBuilder()
    {
    }

    public override Widget buildTransitions<T>(PageRoute<T> route, BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, Widget child)
    {
        return ((Widget)(object?)new _FadeUpwardsPageTransition__page_transitions_builder(routeAnimation: animation, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _OpenUpwardsPageTransition__page_transitions_builder : StatefulWidget
{
    internal static global::Doroti.Framework.Animation.Tween<Offset> _primaryTranslationTween = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.0, 0.05), end: Offset.zero);
    internal static global::Doroti.Framework.Animation.Tween<Offset> _secondaryTranslationTween = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: new global::Doroti.Ui.Offset(0.0, -0.025));
    internal static Color _scrimColor = new global::Doroti.Ui.Color(4278190080L);
    internal static global::Doroti.Framework.Animation.Tween<double> _scrimOpacityTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 0.25);
    internal static global::Doroti.Framework.Animation.Curve _transitionCurve = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.2, 0.0, 0.0, 1.0));
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _OpenUpwardsPageTransition__page_transitions_builder(global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, Widget child)
    {
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _OpenUpwardsPageTransitionState__page_transitions_builder());
}

internal class _OpenUpwardsPageTransitionState__page_transitions_builder : State<_OpenUpwardsPageTransition__page_transitions_builder>
{
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _primaryAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _secondaryTranslationCurvedAnimation { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _setAnimations();
    }

    public override void didUpdateWidget(_OpenUpwardsPageTransition__page_transitions_builder oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((!object.Equals(((_OpenUpwardsPageTransition__page_transitions_builder)oldWidget).animation, ((_OpenUpwardsPageTransition__page_transitions_builder)this.widget).animation)) || (!object.Equals(((_OpenUpwardsPageTransition__page_transitions_builder)oldWidget).secondaryAnimation, ((_OpenUpwardsPageTransition__page_transitions_builder)this.widget).secondaryAnimation))))
        {
            _disposeAnimations();
            _setAnimations();
        }
    }

    internal virtual void _setAnimations()
    {
        _primaryAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_OpenUpwardsPageTransition__page_transitions_builder)this.widget).animation, curve: _OpenUpwardsPageTransition__page_transitions_builder._transitionCurve, reverseCurve: _OpenUpwardsPageTransition__page_transitions_builder._transitionCurve.flipped);
        _secondaryTranslationCurvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_OpenUpwardsPageTransition__page_transitions_builder)this.widget).secondaryAnimation, curve: _OpenUpwardsPageTransition__page_transitions_builder._transitionCurve, reverseCurve: _OpenUpwardsPageTransition__page_transitions_builder._transitionCurve.flipped);
    }

    internal virtual void _disposeAnimations()
    {
        this._primaryAnimation.dispose();
        this._secondaryTranslationCurvedAnimation.dispose();
    }

    public override void dispose()
    {
        _disposeAnimations();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new LayoutBuilder(builder: ((global::System.Func<BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, Widget>)((context, constraints) =>
        {
            global::Doroti.Ui.Size size = ((global::Doroti.Ui.Size)(object?)((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest);
            global::Doroti.Framework.Animation.Animation<double> clipAnimation = ((global::Doroti.Framework.Animation.Animation<double>)(object?)new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: size.height).animate(this._primaryAnimation));
            global::Doroti.Framework.Animation.Animation<double> opacityAnimation = ((global::Doroti.Framework.Animation.Animation<double>)(object?)_OpenUpwardsPageTransition__page_transitions_builder._scrimOpacityTween.animate(this._primaryAnimation));
            global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset> primaryTranslationAnimation = ((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset>)(object?)_OpenUpwardsPageTransition__page_transitions_builder._primaryTranslationTween.animate(this._primaryAnimation));
            global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset> secondaryTranslationAnimation = ((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset>)(object?)_OpenUpwardsPageTransition__page_transitions_builder._secondaryTranslationTween.animate(this._secondaryTranslationCurvedAnimation));
            return ((Widget)(object?)new AnimatedBuilder(animation: global::Doroti.Framework.Foundation.Listenable.CreateMerge(new List<global::Doroti.Framework.Foundation.Listenable> { ((_OpenUpwardsPageTransition__page_transitions_builder)this.widget).animation, ((_OpenUpwardsPageTransition__page_transitions_builder)this.widget).secondaryAnimation }.Cast<global::Doroti.Framework.Foundation.Listenable?>()), builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) =>
            {
                return ((Widget)(object?)new ColoredBox(color: _OpenUpwardsPageTransition__page_transitions_builder._scrimColor.withOpacity(((global::Doroti.Framework.Animation.Animation<double>)opacityAnimation).value), child: new Align(alignment: global::Doroti.Framework.Painting.Alignment.bottomLeft, child: new ClipRect(child: new SizedBox(height: ((global::Doroti.Framework.Animation.Animation<double>)clipAnimation).value, child: new OverflowBox(alignment: global::Doroti.Framework.Painting.Alignment.bottomLeft, maxHeight: size.height, child: new FractionalTranslation(translation: ((global::Doroti.Framework.Animation.Animation<Offset>)secondaryTranslationAnimation).value, child: new FractionalTranslation(translation: ((global::Doroti.Framework.Animation.Animation<Offset>)primaryTranslationAnimation).value, child: ((_OpenUpwardsPageTransition__page_transitions_builder)this.widget).child))))))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class OpenUpwardsPageTransitionsBuilder : PageTransitionsBuilder
{
    public OpenUpwardsPageTransitionsBuilder()
    {
    }

    public override Widget buildTransitions<T>(PageRoute<T> route, BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, Widget child)
    {
        return ((Widget)(object?)new _OpenUpwardsPageTransition__page_transitions_builder(animation: animation, secondaryAnimation: secondaryAnimation, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

