// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/page_transitions_builder.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public abstract class PageTransitionsBuilder
{
    protected PageTransitionsBuilder()
    {
    }

    public virtual Func<BuildContext, Animation<double>, Animation<double>, bool, Widget?, Widget?>? delegatedTransition => DartRuntimePrimitives.ConvertValue<Func<BuildContext, Animation<double>, Animation<double>, bool, Widget?, Widget?>>(null);
    public virtual Duration transitionDuration => Duration.Create(milliseconds: 300L);
    public virtual Duration reverseTransitionDuration => transitionDuration;
    public abstract Widget buildTransitions<T>(PageRoute<T> route, BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child);
}

internal class _FadeUpwardsPageTransition__page_transitions_builder : StatelessWidget
{
    internal static Tween<Offset> _bottomUpTween = new Tween<Offset>(begin: new Offset(0.0, 0.25), end: Offset.zero);
    internal static Animatable<double> _fastOutSlowInTween = new CurveTween(curve: Curves.fastOutSlowIn);
    internal static Animatable<double> _easeInTween = new CurveTween(curve: Curves.easeIn);
    internal virtual Animation<Offset> _positionAnimation { get; private set; } = default!;
    internal virtual Animation<double> _opacityAnimation { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _FadeUpwardsPageTransition__page_transitions_builder(Animation<double> routeAnimation, Widget child)
    {
        this.child = child;
        _positionAnimation = routeAnimation.drive(_bottomUpTween.chain(_fastOutSlowInTween));
        _opacityAnimation = routeAnimation.drive(_easeInTween);
    }

    public override Widget build(BuildContext context)
    {
        return new SlideTransition(position: _positionAnimation, child: new FadeTransition(opacity: _opacityAnimation, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FadeUpwardsPageTransitionsBuilder : PageTransitionsBuilder
{
    public FadeUpwardsPageTransitionsBuilder()
    {
    }

    public override Widget buildTransitions<T>(PageRoute<T>? route, BuildContext? context, Animation<double> animation, Animation<double>? secondaryAnimation, Widget child)
    {
        return new _FadeUpwardsPageTransition__page_transitions_builder(routeAnimation: animation, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _OpenUpwardsPageTransition__page_transitions_builder : StatefulWidget
{
    internal static Tween<Offset> _primaryTranslationTween = new Tween<Offset>(begin: new Offset(0.0, 0.05), end: Offset.zero);
    internal static Tween<Offset> _secondaryTranslationTween = new Tween<Offset>(begin: Offset.zero, end: new Offset(0.0, -0.025));
    internal static Color _scrimColor = new Color(4278190080L);
    internal static Tween<double> _scrimOpacityTween = new Tween<double>(begin: 0.0, end: 0.25);
    internal static Curve _transitionCurve = new Cubic(0.2, 0.0, 0.0, 1.0);
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _OpenUpwardsPageTransition__page_transitions_builder(Animation<double> animation, Animation<double> secondaryAnimation, Widget child)
    {
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _OpenUpwardsPageTransitionState__page_transitions_builder());
}

internal class _OpenUpwardsPageTransitionState__page_transitions_builder : State<_OpenUpwardsPageTransition__page_transitions_builder>
{
    internal virtual CurvedAnimation _primaryAnimation { get; set; } = default!;
    internal virtual CurvedAnimation _secondaryTranslationCurvedAnimation { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _setAnimations();
    }

    public override void didUpdateWidget(_OpenUpwardsPageTransition__page_transitions_builder oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!Equals(oldWidget.animation, widget.animation)) || (!Equals(oldWidget.secondaryAnimation, widget.secondaryAnimation)))
        {
            _disposeAnimations();
            _setAnimations();
        }
    }

    internal virtual void _setAnimations()
    {
        _primaryAnimation = new CurvedAnimation(parent: widget.animation, curve: _OpenUpwardsPageTransition__page_transitions_builder._transitionCurve, reverseCurve: _OpenUpwardsPageTransition__page_transitions_builder._transitionCurve.flipped);
        _secondaryTranslationCurvedAnimation = new CurvedAnimation(parent: widget.secondaryAnimation, curve: _OpenUpwardsPageTransition__page_transitions_builder._transitionCurve, reverseCurve: _OpenUpwardsPageTransition__page_transitions_builder._transitionCurve.flipped);
    }

    internal virtual void _disposeAnimations()
    {
        _primaryAnimation.dispose();
        _secondaryTranslationCurvedAnimation.dispose();
    }

    public override void dispose()
    {
        _disposeAnimations();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new LayoutBuilder(builder: (context, constraints) =>
        {
            Size size = constraints.biggest;
            Animation<double> clipAnimation = new Tween<double>(begin: 0.0, end: size.height).animate(_primaryAnimation);
            Animation<double> opacityAnimation = _OpenUpwardsPageTransition__page_transitions_builder._scrimOpacityTween.animate(_primaryAnimation);
            Animation<Offset> primaryTranslationAnimation = _OpenUpwardsPageTransition__page_transitions_builder._primaryTranslationTween.animate(_primaryAnimation);
            Animation<Offset> secondaryTranslationAnimation = _OpenUpwardsPageTransition__page_transitions_builder._secondaryTranslationTween.animate(_secondaryTranslationCurvedAnimation);
            return new AnimatedBuilder(animation: Listenable.CreateMerge(new List<Listenable> { widget.animation, widget.secondaryAnimation }.Cast<Listenable?>()), builder: (context, child) =>
            {
                return new ColoredBox(color: _OpenUpwardsPageTransition__page_transitions_builder._scrimColor.withOpacity(opacityAnimation.value), child: new Align(alignment: Alignment.bottomLeft, child: new ClipRect(child: new SizedBox(height: clipAnimation.value, child: new OverflowBox(alignment: Alignment.bottomLeft, maxHeight: size.height, child: new FractionalTranslation(translation: secondaryTranslationAnimation.value, child: new FractionalTranslation(translation: primaryTranslationAnimation.value, child: widget.child)))))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class OpenUpwardsPageTransitionsBuilder : PageTransitionsBuilder
{
    public OpenUpwardsPageTransitionsBuilder()
    {
    }

    public override Widget buildTransitions<T>(PageRoute<T> route, BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child)
    {
        return new _OpenUpwardsPageTransition__page_transitions_builder(animation: animation, secondaryAnimation: secondaryAnimation, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

