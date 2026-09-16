// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/route.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class RouteLibrary
{
    internal static double _kBackGestureWidth = 20.0;
}

public static partial class RouteLibrary
{
    internal static double _kMinFlingVelocity = 1.0;
}

public static partial class RouteLibrary
{
    internal static Duration _kDroppedSwipePageAnimationDuration = Duration.Create(milliseconds: 350L);
}

public static partial class RouteLibrary
{
    internal static Color _kCupertinoPageTransitionBarrierColor = new Color(402653184L);
}

public static partial class RouteLibrary
{
    public static Color kCupertinoModalBarrierColor = new CupertinoDynamicColor(color: new Color(855638016L), darkColor: new Color(2046820352L));
}

public static partial class RouteLibrary
{
    internal static Duration _kModalPopupTransitionDuration = Duration.Create(milliseconds: 335L);
}

public static partial class RouteLibrary
{
    internal static Animatable<Offset> _kRightMiddleTween = new Tween<Offset>(begin: new Offset(1.0, 0.0), end: Offset.zero);
}

public static partial class RouteLibrary
{
    internal static Animatable<Offset> _kMiddleLeftTween = new Tween<Offset>(begin: Offset.zero, end: new Offset(-1.0 / 3.0, 0.0));
}

public static partial class RouteLibrary
{
    internal static Animatable<Offset> _kBottomUpTween = new Tween<Offset>(begin: new Offset(0.0, 1.0), end: Offset.zero);
}

public interface ICupertinoRouteTitle
{
    string? title { get; }
    ValueListenable<string?> previousTitle { get; }
}

public interface CupertinoRouteTransitionMixin<T> : ICupertinoRouteTitle
{
    ValueNotifier<string?>? _previousTitle { get; set; }
    public static Duration kTransitionDuration = Duration.Create(milliseconds: 500L);

    public Widget buildContent(BuildContext context);
    public new string? title { get; }
    public new ValueListenable<string?> previousTitle { get; }
    public void dispose();
    public void didChangePrevious(dynamic? previousRoute);
    public Duration transitionDuration { get; }
    public Color? barrierColor { get; }
    public string? barrierLabel { get; }
    public bool canTransitionTo(dynamic nextRoute);
    public bool canTransitionFrom(dynamic previousRoute);
    public Widget buildPage(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation);
    public static _CupertinoBackGestureController__route<TRouteResult> _startPopGesture<TRouteResult>(PageRoute<TRouteResult> route)
    {
        DartRuntimePrimitives.Assert(() => route.popGestureEnabled);
        return new _CupertinoBackGestureController__route<TRouteResult>(navigator: route.navigator!, getIsCurrent: () => route.isCurrent, getIsActive: () => route.isActive, controller: route.controller!);
    }
    public static Widget buildPageTransitions<TRouteResult>(PageRoute<TRouteResult> route, BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child)
    {
        bool linearTransitionLocal = route.popGestureInProgress;
        if (route.fullscreenDialog)
        {
            return new CupertinoFullscreenDialogTransition(primaryRouteAnimation: animation, secondaryRouteAnimation: secondaryAnimation, linearTransition: linearTransitionLocal, child: child);
        }
        else
        {
            return new CupertinoPageTransition(primaryRouteAnimation: animation, secondaryRouteAnimation: secondaryAnimation, linearTransition: linearTransitionLocal, child: new _CupertinoBackGestureDetector__route<TRouteResult>(enabledCallback: () => route.popGestureEnabled, onStartPopGesture: () => CupertinoRouteTransitionMixin<TRouteResult>._startPopGesture(route), child: child));
        }
    }
    public Widget buildTransitions(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child);
}

public class CupertinoPageRoute<T> : PageRoute<T>, CupertinoRouteTransitionMixin<T>
{
    public virtual Func<BuildContext, Widget> builder { get; private set; } = default!;
    public virtual string? title { get; private set; }
    private bool __field_maintainState = default!;
    public override bool maintainState { get => __field_maintainState; }
    public virtual ValueNotifier<string?>? _previousTitle { get; set; } = default;

    public CupertinoPageRoute(Func<BuildContext, Widget> builder, string? title = null, RouteSettings? settings = null, bool? requestFocus = null, bool maintainState = true, bool fullscreenDialog = false, bool allowSnapshotting = true, bool barrierDismissible = false) : base(settings: settings, requestFocus: requestFocus, fullscreenDialog: fullscreenDialog, allowSnapshotting: allowSnapshotting, barrierDismissible: barrierDismissible)
    {
        this.builder = builder;
        this.title = title;
        __field_maintainState = maintainState;
        DartRuntimePrimitives.Assert(() => opaque);
    }

    public override Func<BuildContext, Animation<double>, Animation<double>, bool, Widget?, Widget?>? delegatedTransition => CupertinoPageTransition.delegatedTransition;
    public virtual Widget buildContent(BuildContext context) => builder(context);
    public override string debugLabel => $"{base.debugLabel}({settings.name})";
    public virtual ValueListenable<string?> previousTitle
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _previousTitle is not null, () => (object?)"Cannot read the previousTitle for a route that has not yet been installed");
            return _previousTitle!;
        }
    }
    public override void dispose()
    {
        _previousTitle?.dispose();
        base.dispose();
    }

    public override void didChangePrevious(dynamic? previousRoute)
    {
        string? previousTitleString = (previousRoute is ICupertinoRouteTitle) ? ((ICupertinoRouteTitle)(object)previousRoute).title : null;
        if (_previousTitle is null)
        {
            _previousTitle = new ValueNotifier<string?>(previousTitleString);
        }
        else
        {
            _previousTitle!.value = previousTitleString;
        }
        base.didChangePrevious((object?)previousRoute);
    }

    public override Duration transitionDuration => CupertinoRouteTransitionMixin<object>.kTransitionDuration;
    public override Color? barrierColor => fullscreenDialog ? null : RouteLibrary._kCupertinoPageTransitionBarrierColor;
    public override string? barrierLabel => DartRuntimePrimitives.ConvertValue<string>(null);
    public override bool canTransitionTo(dynamic nextRoute)
    {
        bool nextRouteIsNotFullscreen = nextRoute is not IPageRoute || !((IPageRoute)(object)nextRoute).fullscreenDialog;
        bool nextRouteHasDelegatedTransition = (nextRoute is IModalRoute) && (((IModalRoute)(object)nextRoute).delegatedTransition is not null);
        return nextRouteIsNotFullscreen && (nextRoute is ICupertinoRouteTitle || nextRouteHasDelegatedTransition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionFrom(dynamic previousRoute)
    {
        return (previousRoute is IPageRoute) && !fullscreenDialog;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildPage(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation)
    {
        Widget childLocal = buildContent(context);
        return new Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildTransitions(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child)
    {
        return CupertinoRouteTransitionMixin<object>.buildPageTransitions(this, context, animation, secondaryAnimation, child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PageBasedCupertinoPageRoute__route<T> : PageRoute<T>, CupertinoRouteTransitionMixin<T>
{
    public virtual ValueNotifier<string?>? _previousTitle { get; set; } = default;

    internal _PageBasedCupertinoPageRoute__route(CupertinoPage<T> page, bool allowSnapshotting = true) : base(allowSnapshotting: allowSnapshotting, settings: page)
    {
        DartRuntimePrimitives.Assert(() => opaque);
    }

    public override Func<BuildContext, Animation<double>, Animation<double>, bool, Widget?, Widget?>? delegatedTransition => fullscreenDialog ? null : CupertinoPageTransition.delegatedTransition;
    internal virtual CupertinoPage<T> _page => ((CupertinoPage<T>?)settings)!;
    public virtual Widget buildContent(BuildContext context) => _page.child;
    public virtual string? title => _page.title;
    public override bool maintainState => _page.maintainState;
    public override bool fullscreenDialog => _page.fullscreenDialog;
    public override string debugLabel => $"{base.debugLabel}({_page.name})";
    public virtual ValueListenable<string?> previousTitle
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _previousTitle is not null, () => (object?)"Cannot read the previousTitle for a route that has not yet been installed");
            return _previousTitle!;
        }
    }
    public override void dispose()
    {
        _previousTitle?.dispose();
        base.dispose();
    }

    public override void didChangePrevious(dynamic? previousRoute)
    {
        string? previousTitleString = (previousRoute is ICupertinoRouteTitle) ? ((ICupertinoRouteTitle)(object)previousRoute).title : null;
        if (_previousTitle is null)
        {
            _previousTitle = new ValueNotifier<string?>(previousTitleString);
        }
        else
        {
            _previousTitle!.value = previousTitleString;
        }
        base.didChangePrevious((object?)previousRoute);
    }

    public override Duration transitionDuration => CupertinoRouteTransitionMixin<object>.kTransitionDuration;
    public override Color? barrierColor => fullscreenDialog ? null : RouteLibrary._kCupertinoPageTransitionBarrierColor;
    public override string? barrierLabel => DartRuntimePrimitives.ConvertValue<string>(null);
    public override bool canTransitionTo(dynamic nextRoute)
    {
        bool nextRouteIsNotFullscreen = nextRoute is not IPageRoute || !((IPageRoute)(object)nextRoute).fullscreenDialog;
        bool nextRouteHasDelegatedTransition = (nextRoute is IModalRoute) && (((IModalRoute)(object)nextRoute).delegatedTransition is not null);
        return nextRouteIsNotFullscreen && (nextRoute is ICupertinoRouteTitle || nextRouteHasDelegatedTransition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionFrom(dynamic previousRoute)
    {
        return (previousRoute is IPageRoute) && !fullscreenDialog;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildPage(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation)
    {
        Widget childLocal = buildContent(context);
        return new Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildTransitions(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child)
    {
        return CupertinoRouteTransitionMixin<object>.buildPageTransitions(this, context, animation, secondaryAnimation, child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoPage<T> : Page<T>
{
    public virtual Widget child { get; private set; } = default!;
    public virtual string? title { get; private set; }
    public virtual bool maintainState { get; private set; } = default!;
    public virtual bool fullscreenDialog { get; private set; } = default!;
    public virtual bool allowSnapshotting { get; private set; } = default!;

    public CupertinoPage(Widget child, bool maintainState = true, string? title = null, bool fullscreenDialog = false, bool allowSnapshotting = true, bool canPop = true, Action<bool, T?> onPopInvoked = default!, LocalKey? key = null, string? name = null, object? arguments = null, string? restorationId = null) : base(canPop: canPop, onPopInvoked: onPopInvoked ?? ((didPop, result) => Page<object?>._defaultPopInvokedHandler(didPop, result)), key: key, name: name, arguments: arguments, restorationId: restorationId)
    {
        this.child = child;
        this.maintainState = maintainState;
        this.title = title;
        this.fullscreenDialog = fullscreenDialog;
        this.allowSnapshotting = allowSnapshotting;
    }

    public override Route<T> createRoute(BuildContext context)
    {
        return new _PageBasedCupertinoPageRoute__route<T>(page: this, allowSnapshotting: allowSnapshotting);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoPageTransition : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Animation<double> primaryRouteAnimation { get; private set; } = default!;
    public virtual Animation<double> secondaryRouteAnimation { get; private set; } = default!;
    public virtual bool linearTransition { get; private set; } = default!;

    public CupertinoPageTransition(Key? key = null, Animation<double> primaryRouteAnimation = default!, Animation<double> secondaryRouteAnimation = default!, Widget child = default!, bool linearTransition = default!) : base(key: key)
    {
        this.primaryRouteAnimation = primaryRouteAnimation;
        this.secondaryRouteAnimation = secondaryRouteAnimation;
        this.child = child;
        this.linearTransition = linearTransition;
    }

    public static Widget? delegatedTransition(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, bool allowSnapshotting, Widget? child)
    {
        var animationLocal = new CurvedAnimation(parent: secondaryAnimation, curve: Curves.linearToEaseOut, reverseCurve: Curves.easeInToLinear);
        Animation<Offset> delegatedPositionAnimation = animationLocal.drive(RouteLibrary._kMiddleLeftTween);
        animationLocal.dispose();
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        TextDirection textDirectionLocal = Directionality.of(context);
        return (Widget?)new SlideTransition(position: delegatedPositionAnimation, textDirection: textDirectionLocal, transformHitTests: false, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoPageTransitionState__route());
}

internal class _CupertinoPageTransitionState__route : State<CupertinoPageTransition>
{
    internal virtual Animation<Offset> _primaryPositionAnimation { get; set; } = default!;
    internal virtual Animation<Offset> _secondaryPositionAnimation { get; set; } = default!;
    internal virtual Animation<Decoration> _primaryShadowAnimation { get; set; } = default!;
    internal virtual CurvedAnimation? _primaryPositionCurve { get; set; } = default;
    internal virtual CurvedAnimation? _secondaryPositionCurve { get; set; } = default;
    internal virtual CurvedAnimation? _primaryShadowCurve { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _setupAnimation();
    }

    public override void didUpdateWidget(CupertinoPageTransition oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!Equals(oldWidget.primaryRouteAnimation, widget.primaryRouteAnimation)) || (!Equals(oldWidget.secondaryRouteAnimation, widget.secondaryRouteAnimation)) || (oldWidget.linearTransition != widget.linearTransition))
        {
            _disposeCurve();
            _setupAnimation();
        }
    }

    public override void dispose()
    {
        _disposeCurve();
        base.dispose();
    }

    internal virtual void _disposeCurve()
    {
        _primaryPositionCurve?.dispose();
        _secondaryPositionCurve?.dispose();
        _primaryShadowCurve?.dispose();
        _primaryPositionCurve = null;
        _secondaryPositionCurve = null;
        _primaryShadowCurve = null;
    }

    internal virtual void _setupAnimation()
    {
        if (!widget.linearTransition)
        {
            _primaryPositionCurve = new CurvedAnimation(parent: widget.primaryRouteAnimation, curve: Curves.fastEaseInToSlowEaseOut, reverseCurve: Curves.fastEaseInToSlowEaseOut.flipped);
            _secondaryPositionCurve = new CurvedAnimation(parent: widget.secondaryRouteAnimation, curve: Curves.linearToEaseOut, reverseCurve: Curves.easeInToLinear);
            _primaryShadowCurve = new CurvedAnimation(parent: widget.primaryRouteAnimation, curve: Curves.linearToEaseOut);
        }
        _primaryPositionAnimation = (_primaryPositionCurve ?? widget.primaryRouteAnimation).drive(RouteLibrary._kRightMiddleTween);
        _secondaryPositionAnimation = (_secondaryPositionCurve ?? widget.secondaryRouteAnimation).drive(RouteLibrary._kMiddleLeftTween);
        _primaryShadowAnimation = (_primaryShadowCurve ?? widget.primaryRouteAnimation).drive(_CupertinoEdgeShadowDecoration__route.kTween);
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        TextDirection textDirectionLocal = Directionality.of(context);
        return new SlideTransition(position: _secondaryPositionAnimation, textDirection: textDirectionLocal, transformHitTests: false, child: new SlideTransition(position: _primaryPositionAnimation, textDirection: textDirectionLocal, child: new DecoratedBoxTransition(decoration: _primaryShadowAnimation, child: widget.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoFullscreenDialogTransition : StatefulWidget
{
    public virtual Animation<double> primaryRouteAnimation { get; private set; } = default!;
    public virtual Animation<double> secondaryRouteAnimation { get; private set; } = default!;
    public virtual bool linearTransition { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public CupertinoFullscreenDialogTransition(Key? key = null, Animation<double> primaryRouteAnimation = default!, Animation<double> secondaryRouteAnimation = default!, Widget child = default!, bool linearTransition = default!) : base(key: key)
    {
        this.primaryRouteAnimation = primaryRouteAnimation;
        this.secondaryRouteAnimation = secondaryRouteAnimation;
        this.child = child;
        this.linearTransition = linearTransition;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoFullscreenDialogTransitionState__route());
}

internal class _CupertinoFullscreenDialogTransitionState__route : State<CupertinoFullscreenDialogTransition>
{
    internal virtual Animation<Offset> _primaryPositionAnimation { get; set; } = default!;
    internal virtual Animation<Offset> _secondaryPositionAnimation { get; set; } = default!;
    internal virtual CurvedAnimation? _primaryPositionCurve { get; set; } = default;
    internal virtual CurvedAnimation? _secondaryPositionCurve { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _setupAnimation();
    }

    public override void didUpdateWidget(CupertinoFullscreenDialogTransition oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!Equals(oldWidget.primaryRouteAnimation, widget.primaryRouteAnimation)) || (!Equals(oldWidget.secondaryRouteAnimation, widget.secondaryRouteAnimation)) || (oldWidget.linearTransition != widget.linearTransition))
        {
            _disposeCurve();
            _setupAnimation();
        }
    }

    public override void dispose()
    {
        _disposeCurve();
        base.dispose();
    }

    internal virtual void _disposeCurve()
    {
        _primaryPositionCurve?.dispose();
        _secondaryPositionCurve?.dispose();
        _primaryPositionCurve = null;
        _secondaryPositionCurve = null;
    }

    internal virtual void _setupAnimation()
    {
        _primaryPositionAnimation = (_primaryPositionCurve = new CurvedAnimation(parent: widget.primaryRouteAnimation, curve: Curves.linearToEaseOut, reverseCurve: Curves.linearToEaseOut.flipped)).drive(RouteLibrary._kBottomUpTween);
        _secondaryPositionAnimation = (widget.linearTransition ? widget.secondaryRouteAnimation : _secondaryPositionCurve = new CurvedAnimation(parent: widget.secondaryRouteAnimation, curve: Curves.linearToEaseOut, reverseCurve: Curves.easeInToLinear)).drive(RouteLibrary._kMiddleLeftTween);
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        TextDirection textDirectionLocal = Directionality.of(context);
        return new SlideTransition(position: _secondaryPositionAnimation, textDirection: textDirectionLocal, transformHitTests: false, child: new SlideTransition(position: _primaryPositionAnimation, child: widget.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoBackGestureDetector__route<T> : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Func<bool> enabledCallback { get; private set; } = default!;
    public virtual Func<_CupertinoBackGestureController__route<T>> onStartPopGesture { get; private set; } = default!;

    internal _CupertinoBackGestureDetector__route(Key? key = null, Func<bool> enabledCallback = default!, Func<_CupertinoBackGestureController__route<T>> onStartPopGesture = default!, Widget child = default!) : base(key: key)
    {
        this.enabledCallback = enabledCallback;
        this.onStartPopGesture = onStartPopGesture;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoBackGestureDetectorState__route<T>());
}

internal class _CupertinoBackGestureDetectorState__route<T> : State<_CupertinoBackGestureDetector__route<T>>
{
    internal virtual _CupertinoBackGestureController__route<T>? _backGestureController { get; set; } = default;
    internal virtual Gestures.HorizontalDragGestureRecognizer _recognizer { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _recognizer = ((Func<Gestures.HorizontalDragGestureRecognizer>)(() =>
{
    var __cascade = new Gestures.HorizontalDragGestureRecognizer(debugOwner: this);
    __cascade.onStart = _handleDragStart;
    __cascade.onUpdate = _handleDragUpdate;
    __cascade.onEnd = _handleDragEnd;
    __cascade.onCancel = _handleDragCancel;
    return __cascade;
}))();
    }

    public override void dispose()
    {
        _recognizer.dispose();
        if (_backGestureController is not null)
        {
            WidgetsBinding.instance.addPostFrameCallback((_) =>
            {
                if (_backGestureController?.navigator.mounted ?? false)
                {
                    _backGestureController?.navigator.didStopUserGesture();
                }
                _backGestureController = null;
            });
        }
        base.dispose();
    }

    internal virtual void _handleDragStart(Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        DartRuntimePrimitives.Assert(() => _backGestureController is null);
        _backGestureController = widget.onStartPopGesture();
    }

    internal virtual void _handleDragUpdate(Gestures.DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        DartRuntimePrimitives.Assert(() => _backGestureController is not null);
        _backGestureController!.dragUpdate(_convertToLogical(DartRuntimePrimitives.RequireValue(details.primaryDelta) / DartRuntimePrimitives.RequireValue(context.size).width));
    }

    internal virtual void _handleDragEnd(Gestures.DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        DartRuntimePrimitives.Assert(() => _backGestureController is not null);
        _backGestureController!.dragEnd(_convertToLogical(details.velocity.pixelsPerSecond.dx / DartRuntimePrimitives.RequireValue(context.size).width));
        _backGestureController = null;
    }

    internal virtual void _handleDragCancel()
    {
        DartRuntimePrimitives.Assert(() => mounted);
        _backGestureController?.dragEnd(0.0);
        _backGestureController = null;
    }

    internal virtual void _handlePointerDown(Gestures.PointerDownEvent @event)
    {
        if (widget.enabledCallback())
        {
            _recognizer.addPointer(@event);
        }
    }

    internal virtual double _convertToLogical(double value)
    {
        return Directionality.of(context) switch { TextDirection.rtl => -value, TextDirection.ltr => value, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        double dragAreaWidth = Directionality.of(context) switch { TextDirection.rtl => MediaQuery.paddingOf(context).right, TextDirection.ltr => MediaQuery.paddingOf(context).left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return new Stack(fit: StackFit.passthrough, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(widget.child), DartRuntimePrimitives.ConvertValue<Widget>(new PositionedDirectional(start: 0.0, width: Math.Max(dragAreaWidth, RouteLibrary._kBackGestureWidth), top: 0.0, bottom: 0.0, child: new Listener(onPointerDown: _handlePointerDown, behavior: HitTestBehavior.translucent))) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _CupertinoBackGestureController__route<T>
{
    public virtual AnimationController controller { get; private set; } = default!;
    public virtual NavigatorState navigator { get; private set; } = default!;
    public virtual Func<bool> getIsActive { get; private set; } = default!;
    public virtual Func<bool> getIsCurrent { get; private set; } = default!;

    internal _CupertinoBackGestureController__route(NavigatorState navigator, AnimationController controller, Func<bool> getIsActive, Func<bool> getIsCurrent)
    {
        this.navigator = navigator;
        this.controller = controller;
        this.getIsActive = getIsActive;
        this.getIsCurrent = getIsCurrent;
        this.navigator.didStartUserGesture();
    }

    public virtual void dragUpdate(double delta)
    {
        controller.value -= delta;
    }

    public virtual void dragEnd(double velocity)
    {
        Curve animationCurve = Curves.fastEaseInToSlowEaseOut;
        bool isCurrent = getIsCurrent();
        bool animateForward = default!;
        if (!isCurrent)
        {
            animateForward = getIsActive();
        }
        else
        {
            if (velocity.abs() >= RouteLibrary._kMinFlingVelocity)
            {
                animateForward = velocity <= 0L;
            }
            else
            {
                animateForward = controller.value > 0.5;
            }
        }
        if (animateForward)
        {
            controller.animateTo(1.0, duration: RouteLibrary._kDroppedSwipePageAnimationDuration, curve: animationCurve);
        }
        else
        {
            if (isCurrent)
            {
                navigator.pop<object>();
            }
            if (controller.isAnimating)
            {
                controller.animateBack(0.0, duration: RouteLibrary._kDroppedSwipePageAnimationDuration, curve: animationCurve);
            }
        }
        if (controller.isAnimating)
        {
            AnimationStatusListener animationStatusCallback = default!;
            animationStatusCallback = (status) =>
            {
                navigator.didStopUserGesture();
                controller.removeStatusListener(animationStatusCallback);
            };
            controller.addStatusListener(animationStatusCallback);
        }
        else
        {
            navigator.didStopUserGesture();
        }
    }

}

public class _CupertinoEdgeShadowDecoration__route : Decoration
{
    public static DecorationTween kTween = new DecorationTween(begin: new _CupertinoEdgeShadowDecoration__route(), end: new _CupertinoEdgeShadowDecoration__route(new List<Color> { new Color(67108864L), CupertinoColors.transparent }));
    internal virtual List<Color>? _colors { get; private set; }

    internal _CupertinoEdgeShadowDecoration__route(List<Color>? _colors = null)
    {
        this._colors = _colors;
    }

    public static _CupertinoEdgeShadowDecoration__route? lerp(_CupertinoEdgeShadowDecoration__route? a, _CupertinoEdgeShadowDecoration__route? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return (b!._colors is null) ? b : new _CupertinoEdgeShadowDecoration__route(b._colors!.map((color) => Dart_uiLibrary.Color.lerp(null, color, t)!).ToList());
        }
        if (b is null)
        {
            return (a._colors is null) ? a : new _CupertinoEdgeShadowDecoration__route(a._colors.map((color) => Dart_uiLibrary.Color.lerp(null, color, 1.0 - t)!).ToList());
        }
        DartRuntimePrimitives.Assert(() => (b._colors is not null) || (a._colors is not null));
        DartRuntimePrimitives.Assert(() => (b._colors is null) || (a._colors is null) || (checked(a._colors.Count) == checked((long)b._colors.Count)));
        var colors = new List<Color>();
        int count = b._colors?.Count ?? a._colors?.Count ?? 0;
        for (var i = 0; i < count; i++)
            colors.Add(Dart_uiLibrary.Color.lerp(a._colors?[i], b._colors?[i], t)!);
        return new _CupertinoEdgeShadowDecoration__route(colors);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _CupertinoEdgeShadowDecoration__route lerpFrom(Decoration? a, double t)
    {
        if (a is _CupertinoEdgeShadowDecoration__route)
        {
            _CupertinoEdgeShadowDecoration__route a__as34106 = (_CupertinoEdgeShadowDecoration__route)a;
            return lerp(a__as34106, this, t)!;
        }
        return lerp(null, this, t)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _CupertinoEdgeShadowDecoration__route lerpTo(Decoration? b, double t)
    {
        if (b is _CupertinoEdgeShadowDecoration__route)
        {
            _CupertinoEdgeShadowDecoration__route b__as34370 = (_CupertinoEdgeShadowDecoration__route)b;
            return lerp(this, b__as34370, t)!;
        }
        return lerp(this, null, t)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _CupertinoEdgeShadowPainter__route createBoxPainter(Action onChanged = default!)
    {
        return new _CupertinoEdgeShadowPainter__route(this, () => onChanged());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _CupertinoEdgeShadowDecoration__route;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _CupertinoEdgeShadowDecoration__route) && Equals(__other._colors, _colors);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(_colors?.GetHashCode() ?? 0);
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IterableProperty<Color>("colors", _colors));
    }

}

public class _CupertinoEdgeShadowPainter__route : BoxPainter
{
    internal virtual _CupertinoEdgeShadowDecoration__route _decoration { get; private set; } = default!;

    internal _CupertinoEdgeShadowPainter__route(_CupertinoEdgeShadowDecoration__route _decoration, Action? onChanged) : base(onChanged)
    {
        this._decoration = _decoration;
        System.Diagnostics.Debug.Assert((_decoration._colors is null) || (checked(_decoration._colors.Count) > 1L));
    }

    public override void paint(Canvas canvas, Offset offset, ImageConfiguration configuration)
    {
        List<Color>? colors = _decoration._colors?.ToList();
        if (colors is null)
        {
            return;
        }
        double shadowWidth = 0.05 * DartRuntimePrimitives.RequireValue(configuration.size).width;
        double shadowHeight = DartRuntimePrimitives.RequireValue(configuration.size).height;
        double bandWidth = shadowWidth / (checked(colors.Count) - 1L);
        TextDirection? textDirectionLocal = configuration.textDirection;
        DartRuntimePrimitives.Assert(() => textDirectionLocal is not null);
        var (shadowDirection, start) = DartRuntimePrimitives.RequireValue(textDirectionLocal) switch { TextDirection.rtl => (1, offset.dx + DartRuntimePrimitives.RequireValue(configuration.size).width), TextDirection.ltr => ((double, double))(-1, offset.dx), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        var bandColorIndex = 0L;
        for (var dxLocal = 0L; dxLocal < shadowWidth; dxLocal += 1L)
        {
            if (checked((long)(dxLocal / bandWidth)) != bandColorIndex)
            {
                bandColorIndex += 1L;
            }
            var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.color = Dart_uiLibrary.Color.lerp(colors[(int)bandColorIndex], colors[(int)(bandColorIndex + 1L)], dxLocal % bandWidth / bandWidth)!;
    return __cascade;
}))();
            double x = start + (shadowDirection * dxLocal);
            canvas.drawRect(Rect.fromLTWH(x - 1.0, offset.dy, 1.0, shadowHeight), paintLocal);
        }
    }

}

public static partial class RouteLibrary
{
    internal static double _kStandardStiffness = 522.35;
}

public static partial class RouteLibrary
{
    internal static double _kStandardDamping = 45.7099552;
}

public static partial class RouteLibrary
{
    internal static Physics.SpringDescription _kStandardSpring = new Physics.SpringDescription(mass: 1, stiffness: _kStandardStiffness, damping: _kStandardDamping);
}

public static partial class RouteLibrary
{
    internal static Physics.Tolerance _kStandardTolerance = new Physics.Tolerance(velocity: 0.03);
}

public class CupertinoModalPopupRoute<T> : PopupRoute<T>
{
    public virtual Func<BuildContext, Widget> builder { get; private set; } = default!;
    internal virtual bool _barrierDismissible { get; private set; } = default!;
    internal virtual bool _semanticsDismissible { get; private set; } = default!;
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel { get => __field_barrierLabel; }
    private Color? __field_barrierColor = default!;
    public override Color? barrierColor { get => __field_barrierColor; }
    public virtual Offset? anchorPoint { get; private set; }
    internal static Tween<Offset> _offsetTween = new Tween<Offset>(begin: new Offset(0.0, 1.0), end: Offset.zero);

    public CupertinoModalPopupRoute(Func<BuildContext, Widget> builder, string barrierLabel = "Dismiss", Color? barrierColor = default!, bool barrierDismissible = true, bool semanticsDismissible = false, ImageFilter? filter = null, RouteSettings? settings = null, bool? requestFocus = null, Offset? anchorPoint = null) : base(filter: filter, settings: settings, requestFocus: requestFocus)
    {
        Color? __barrierColor = barrierColor ?? RouteLibrary.kCupertinoModalBarrierColor;
        this.builder = builder;
        __field_barrierLabel = barrierLabel;
        __field_barrierColor = __barrierColor;
        this.anchorPoint = anchorPoint;
        _barrierDismissible = barrierDismissible;
        _semanticsDismissible = semanticsDismissible;
    }

    public override bool barrierDismissible => _barrierDismissible;
    public override bool semanticsDismissible => _semanticsDismissible;
    public override Duration transitionDuration => RouteLibrary._kModalPopupTransitionDuration;
    public override Physics.Simulation? createSimulation(bool forward)
    {
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {GetType()} after disposing it.");
        var end = forward ? 1.0 : 0.0;
        return (Physics.Simulation?)new Physics.SpringSimulation(RouteLibrary._kStandardSpring, controller!.value, end, 0, tolerance: RouteLibrary._kStandardTolerance, snapToEnd: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildPage(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation)
    {
        return new CupertinoUserInterfaceLevel(data: CupertinoUserInterfaceLevelData.elevated, child: new DisplayFeatureSubScreen(anchorPoint: anchorPoint, child: new Builder(builder: builder)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildTransitions(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child)
    {
        return new Align(alignment: Alignment.bottomCenter, child: new FractionalTranslation(translation: _offsetTween.evaluate(animation), child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class RouteLibrary
{
    public static Future<T?> showCupertinoModalPopup<T>(BuildContext context, Func<BuildContext, Widget> builder, ImageFilter? filter = null, Color barrierColor = default!, bool barrierDismissible = true, bool useRootNavigator = true, bool semanticsDismissible = false, RouteSettings? routeSettings = null, Offset? anchorPoint = null, bool? requestFocus = null)
    {
        return Navigator.of(context, rootNavigator: useRootNavigator).push(new CupertinoModalPopupRoute<T>(builder: builder, filter: filter, barrierColor: CupertinoDynamicColor.resolve(barrierColor, context), barrierDismissible: barrierDismissible, semanticsDismissible: semanticsDismissible, settings: routeSettings, anchorPoint: anchorPoint, requestFocus: requestFocus));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class RouteLibrary
{
    internal static Widget _buildCupertinoDialogTransitions(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child)
    {
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class RouteLibrary
{
    public static Future<T?> showCupertinoDialog<T>(BuildContext context, Func<BuildContext, Widget> builder, string? barrierLabel = null, Color? barrierColor = null, bool useRootNavigator = true, bool barrierDismissible = false, RouteSettings? routeSettings = null, Offset? anchorPoint = null, bool? requestFocus = null)
    {
        return Navigator.of(context, rootNavigator: useRootNavigator).push(new CupertinoDialogRoute<T>(builder: builder, context: context, barrierDismissible: barrierDismissible, barrierLabel: barrierLabel, barrierColor: barrierColor, settings: routeSettings, anchorPoint: anchorPoint, requestFocus: requestFocus));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoDialogRoute<T> : RawDialogRoute<T>
{
    public virtual Func<BuildContext, Animation<double>, Animation<double>, Widget, Widget>? transitionBuilder { get; set; } = default;
    internal virtual CurvedAnimation? _fadeAnimation { get; set; } = default;
    internal static Tween<double> _dialogScaleTween = new Tween<double>(begin: 1.3, end: 1.0);

    public CupertinoDialogRoute(Func<BuildContext, Widget> builder, BuildContext context, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, Duration? transitionDuration = null, Func<BuildContext, Animation<double>, Animation<double>, Widget, Widget>? transitionBuilder = null, RouteSettings? settings = null, bool? requestFocus = null, Offset? anchorPoint = null) : base(barrierDismissible: barrierDismissible, transitionDuration: transitionDuration ?? Duration.Create(milliseconds: 250), settings: settings, requestFocus: requestFocus, anchorPoint: anchorPoint, pageBuilder: (context, animation, secondaryAnimation) =>
    {
        return builder(context);
        throw new InvalidOperationException("Dart closure completed without a value.");
    }, transitionBuilder: transitionBuilder ?? RouteLibrary._buildCupertinoDialogTransitions, barrierLabel: barrierLabel ?? CupertinoLocalizations.of(context).modalBarrierDismissLabel, barrierColor: barrierColor ?? CupertinoDynamicColor.resolve(RouteLibrary.kCupertinoModalBarrierColor, context))
    {
        this.transitionBuilder = transitionBuilder;
    }

    public override Physics.Simulation? createSimulation(bool forward)
    {
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {GetType()} after disposing it.");
        var end = forward ? 1.0 : 0.0;
        return (Physics.Simulation?)new Physics.SpringSimulation(RouteLibrary._kStandardSpring, controller!.value, end, 0, tolerance: RouteLibrary._kStandardTolerance, snapToEnd: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildTransitions(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child)
    {
        if (transitionBuilder is not null)
        {
            return base.buildTransitions(context, animation, secondaryAnimation, child);
        }
        if (Equals(animation.status, AnimationStatus.reverse))
        {
            return new FadeTransition(opacity: animation, child: child);
        }
        return new FadeTransition(opacity: animation, child: new ScaleTransition(scale: animation.drive(_dialogScaleTween), child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _fadeAnimation?.dispose();
        base.dispose();
    }

}

public class CupertinoPageTransitionsBuilder : PageTransitionsBuilder
{
    public CupertinoPageTransitionsBuilder()
    {
    }

    public override Duration transitionDuration => CupertinoRouteTransitionMixin<object>.kTransitionDuration;
    public override Func<BuildContext, Animation<double>, Animation<double>, bool, Widget?, Widget?>? delegatedTransition => CupertinoPageTransition.delegatedTransition;
    public override Widget buildTransitions<T>(PageRoute<T> route, BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation, Widget child)
    {
        return CupertinoRouteTransitionMixin<object>.buildPageTransitions(route, context, animation, secondaryAnimation, child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
