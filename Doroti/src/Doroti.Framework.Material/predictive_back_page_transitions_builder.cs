// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/predictive_back_page_transitions_builder.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class PredictiveBackPageTransitionsBuilder : global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder
{
    public virtual Color? fallbackColor { get; private set; }

    public PredictiveBackPageTransitionsBuilder(Color? fallbackColor = null)
    {
        this.fallbackColor = fallbackColor;
    }

    public override Duration transitionDuration => Duration.Create(milliseconds: FadeForwardsPageTransitionsBuilder.kTransitionMilliseconds);
    public override global::Doroti.Generated.Framework.Widgets.Widget buildTransitions<T>(global::Doroti.Generated.Framework.Widgets.PageRoute<T> route, global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _PredictiveBackGestureDetector__predictive_back_page_transitions_builder(route: route, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, _PredictiveBackPhase__predictive_back_page_transitions_builder, global::Doroti.Generated.Framework.Services.PredictiveBackEvent?, global::Doroti.Generated.Framework.Services.PredictiveBackEvent?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, phase, startBackEvent, currentBackEvent) => {
if (route.popGestureInProgress)
{
    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder(isDelegatedTransition: true, animation: animation, phase: phase, secondaryAnimation: secondaryAnimation, startBackEvent: startBackEvent, currentBackEvent: currentBackEvent, child: child));
}
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new FadeForwardsPageTransitionsBuilder(backgroundColor: this.fallbackColor).buildTransitions(route, context, animation, secondaryAnimation, child));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PredictiveBackFullscreenPageTransitionsBuilder : global::Doroti.Generated.Framework.Widgets.PageTransitionsBuilder
{
    public virtual Color? fallbackColor { get; private set; }

    public PredictiveBackFullscreenPageTransitionsBuilder(Color? fallbackColor = null)
    {
        this.fallbackColor = fallbackColor;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildTransitions<T>(global::Doroti.Generated.Framework.Widgets.PageRoute<T> route, global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _PredictiveBackGestureDetector__predictive_back_page_transitions_builder(route: route, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, _PredictiveBackPhase__predictive_back_page_transitions_builder, global::Doroti.Generated.Framework.Services.PredictiveBackEvent?, global::Doroti.Generated.Framework.Services.PredictiveBackEvent?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, phase, startBackEvent, currentBackEvent) => {
if (route.popGestureInProgress)
{
    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder(animation: animation, secondaryAnimation: secondaryAnimation, getIsCurrent: ((global::System.Func<bool>)(() => route.isCurrent)), phase: phase, child: child));
}
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new ZoomPageTransitionsBuilder(backgroundColor: this.fallbackColor).buildTransitions(route, context, animation, secondaryAnimation, child));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate global::Doroti.Generated.Framework.Widgets.Widget _PredictiveBackGestureDetectorWidgetBuilder__predictive_back_page_transitions_builder(global::Doroti.Generated.Framework.Widgets.BuildContext context, _PredictiveBackPhase__predictive_back_page_transitions_builder phase, global::Doroti.Generated.Framework.Services.PredictiveBackEvent? startBackEvent, global::Doroti.Generated.Framework.Services.PredictiveBackEvent? currentBackEvent);

public enum _PredictiveBackPhase__predictive_back_page_transitions_builder
{
    idle,
    start,
    update,
    commit,
    cancel
}

internal class _PredictiveBackGestureDetector__predictive_back_page_transitions_builder : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, _PredictiveBackPhase__predictive_back_page_transitions_builder, global::Doroti.Generated.Framework.Services.PredictiveBackEvent?, global::Doroti.Generated.Framework.Services.PredictiveBackEvent?, global::Doroti.Generated.Framework.Widgets.Widget> builder { get; private set; } = default!;
    public virtual dynamic route { get; private set; } = default!;

    internal _PredictiveBackGestureDetector__predictive_back_page_transitions_builder(dynamic route, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, _PredictiveBackPhase__predictive_back_page_transitions_builder, global::Doroti.Generated.Framework.Services.PredictiveBackEvent?, global::Doroti.Generated.Framework.Services.PredictiveBackEvent?, global::Doroti.Generated.Framework.Widgets.Widget> builder)
    {
        this.route = route;
        this.builder = builder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PredictiveBackGestureDetectorState__predictive_back_page_transitions_builder());
}

internal class _PredictiveBackGestureDetectorState__predictive_back_page_transitions_builder : global::Doroti.Generated.Framework.Widgets.State<_PredictiveBackGestureDetector__predictive_back_page_transitions_builder>, global::Doroti.Generated.Framework.Widgets.WidgetsBindingObserver
{
    internal virtual _PredictiveBackPhase__predictive_back_page_transitions_builder _phase { get; set; } = _PredictiveBackPhase__predictive_back_page_transitions_builder.idle;
    internal virtual global::Doroti.Generated.Framework.Services.PredictiveBackEvent? _startBackEvent { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Services.PredictiveBackEvent? _currentBackEvent { get; set; } = default;

    internal virtual bool _isEnabled
    {
        get
        {
            return (((bool)((dynamic)((_PredictiveBackGestureDetector__predictive_back_page_transitions_builder)this.widget).route).isCurrent) && ((bool)((dynamic)((_PredictiveBackGestureDetector__predictive_back_page_transitions_builder)this.widget).route).popGestureEnabled));
            return default!;
        }
    }
    public virtual _PredictiveBackPhase__predictive_back_page_transitions_builder phase
    {
        get => this._phase;
        set
        {
            var phase = value;
            if (((!object.Equals(this._phase, phase)) && this.mounted))
            {
                setState(((global::System.Action)(() => { _ = _phase = phase; })));
            }
        }
    }
    public virtual global::Doroti.Generated.Framework.Services.PredictiveBackEvent? startBackEvent
    {
        get => this._startBackEvent;
        set
        {
            var startBackEvent = value;
            if (((!object.Equals(this._startBackEvent, startBackEvent)) && this.mounted))
            {
                setState(((global::System.Action)(() => { _ = _startBackEvent = startBackEvent; })));
            }
        }
    }
    public virtual global::Doroti.Generated.Framework.Services.PredictiveBackEvent? currentBackEvent
    {
        get => this._currentBackEvent;
        set
        {
            var currentBackEvent = value;
            if (((!object.Equals(this._currentBackEvent, currentBackEvent)) && this.mounted))
            {
                setState(((global::System.Action)(() => { _ = _currentBackEvent = currentBackEvent; })));
            }
        }
    }
    public virtual bool handleStartBackGesture(global::Doroti.Generated.Framework.Services.PredictiveBackEvent backEvent)
    {
        phase = _PredictiveBackPhase__predictive_back_page_transitions_builder.start;
        bool gestureInProgress__9997 = (!((global::Doroti.Generated.Framework.Services.PredictiveBackEvent)backEvent).isButtonEvent && this._isEnabled);
        if (!gestureInProgress__9997)
        {
            return false;
        }
        ((dynamic)((_PredictiveBackGestureDetector__predictive_back_page_transitions_builder)this.widget).route).handleStartBackGesture(progress: (1L - ((global::Doroti.Generated.Framework.Services.PredictiveBackEvent)backEvent).progress));
        startBackEvent = currentBackEvent = backEvent;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleUpdateBackGestureProgress(global::Doroti.Generated.Framework.Services.PredictiveBackEvent backEvent)
    {
        phase = _PredictiveBackPhase__predictive_back_page_transitions_builder.update;
        ((dynamic)((_PredictiveBackGestureDetector__predictive_back_page_transitions_builder)this.widget).route).handleUpdateBackGestureProgress(progress: (1L - ((global::Doroti.Generated.Framework.Services.PredictiveBackEvent)backEvent).progress));
        currentBackEvent = backEvent;
    }

    public virtual void handleCancelBackGesture()
    {
        phase = _PredictiveBackPhase__predictive_back_page_transitions_builder.cancel;
        ((dynamic)((_PredictiveBackGestureDetector__predictive_back_page_transitions_builder)this.widget).route).handleCancelBackGesture();
        startBackEvent = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Services.PredictiveBackEvent>(currentBackEvent = null);
    }

    public virtual void handleCommitBackGesture()
    {
        phase = _PredictiveBackPhase__predictive_back_page_transitions_builder.commit;
        ((dynamic)((_PredictiveBackGestureDetector__predictive_back_page_transitions_builder)this.widget).route).handleCommitBackGesture();
        startBackEvent = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Services.PredictiveBackEvent>(currentBackEvent = null);
    }

    public override void initState()
    {
        base.initState();
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.addObserver(this);
    }

    public override void dispose()
    {
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.removeObserver(this);
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        _PredictiveBackPhase__predictive_back_page_transitions_builder effectivePhase__11210 = (((bool)((dynamic)((_PredictiveBackGestureDetector__predictive_back_page_transitions_builder)this.widget).route).popGestureInProgress) ? this.phase : _PredictiveBackPhase__predictive_back_page_transitions_builder.idle);
        return this.widget.builder(context, effectivePhase__11210, this.startBackEvent, this.currentBackEvent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual bool isDelegatedTransition { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual _PredictiveBackPhase__predictive_back_page_transitions_builder phase { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.PredictiveBackEvent? startBackEvent { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.PredictiveBackEvent? currentBackEvent { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder(bool isDelegatedTransition, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, _PredictiveBackPhase__predictive_back_page_transitions_builder phase, global::Doroti.Generated.Framework.Services.PredictiveBackEvent? startBackEvent, global::Doroti.Generated.Framework.Services.PredictiveBackEvent? currentBackEvent, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        this.isDelegatedTransition = isDelegatedTransition;
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.phase = phase;
        this.startBackEvent = startBackEvent;
        this.currentBackEvent = currentBackEvent;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PredictiveBackSharedElementPageTransitionState__predictive_back_page_transitions_builder());
}

internal class _PredictiveBackSharedElementPageTransitionState__predictive_back_page_transitions_builder : global::Doroti.Generated.Framework.Widgets.State<_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder>, global::Doroti.Generated.Framework.Widgets.SingleTickerProviderStateMixin<_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder>
{
    internal const double _kMinScale = 0.9;
    internal const double _kDivisionFactor = 20.0;
    internal const double _kMargin = 8.0;
    internal const double _kYPositionFactor = 0.1;
    internal const long _kCommitMilliseconds = 400L;
    internal static global::Doroti.Generated.Framework.Animation.Curve _kCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)global::Doroti.Generated.Framework.Animation.Curves.easeInOutCubicEmphasized);
    internal static global::Doroti.Generated.Framework.Animation.Interval _kCommitInterval = new global::Doroti.Generated.Framework.Animation.Interval(0.0, (_kCommitMilliseconds / FadeForwardsPageTransitionsBuilder.kTransitionMilliseconds), curve: _kCurve);
    internal const double _kDeviceBorderRadius = 32.0;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double> _borderRadiusTween { get; private set; } = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: _kDeviceBorderRadius);
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double> _opacityTween { get; private set; } = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0);
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double> _scaleTween { get; private set; } = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 1.0, end: _kMinScale);
    internal virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation _commitAnimation { get; private set; } = new global::Doroti.Generated.Framework.Animation.ProxyAnimation();
    internal virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation _bounceAnimation { get; private set; } = new global::Doroti.Generated.Framework.Animation.ProxyAnimation();
    internal virtual double _lastBounceAnimationValue { get; set; } = 0.0;
    internal virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation _animation { get; private set; } = new global::Doroti.Generated.Framework.Animation.ProxyAnimation();
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation? _curvedAnimation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation? _curvedAnimationReversed { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<Offset> _positionAnimation { get; set; } = default!;
    internal virtual Offset _lastDrag { get; set; } = Offset.zero;
    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual double _getYShiftPosition(double screenHeight)
    {
        double startTouchY__15631 = (((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).startBackEvent?.touchOffset?.dy ?? 0);
        double currentTouchY__15707 = (((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).currentBackEvent?.touchOffset?.dy ?? 0);
        double yShiftMax__15788 = (((screenHeight / _kDivisionFactor)) - _kMargin);
        double rawYShift__15864 = (currentTouchY__15707 - startTouchY__15631);
        double easedYShift__15922 = ((global::Doroti.Generated.Framework.Animation.Curves.easeOut.transform(Dart_uiLibrary.clampDouble((rawYShift__15864.abs() / screenHeight), 0.0, 1.0)) * Math.Sign(rawYShift__15864)) * yShiftMax__15788);
        return Dart_uiLibrary.clampDouble(easedYShift__15922, -yShiftMax__15788, yShiftMax__15788);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateAnimations(Size screenSize)
    {
        this._animation.parent = (((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).phase switch { _PredictiveBackPhase__predictive_back_page_transitions_builder.commit => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animation<double>>(this._curvedAnimationReversed), _ => ((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).animation });
        this._bounceAnimation.parent = (((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).phase switch { _PredictiveBackPhase__predictive_back_page_transitions_builder.commit => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: this._lastBounceAnimationValue).animate(this._curvedAnimation!), _ => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animation<double>>(new global::Doroti.Generated.Framework.Animation.ReverseAnimation(((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).animation)) });
        this._commitAnimation.parent = (((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).phase switch { _PredictiveBackPhase__predictive_back_page_transitions_builder.commit => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animation<double>>(this._animation), _ => global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation });
        double xShift__16820 = (((screenSize.width / _kDivisionFactor)) - _kMargin);
        _positionAnimation = this._animation.drive((((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).phase switch { _PredictiveBackPhase__predictive_back_page_transitions_builder.commit => new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: this._lastDrag, end: new global::Doroti.Ui.Offset((screenSize.height * _kYPositionFactor), 0.0)), _ => new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: (((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).currentBackEvent?.swipeEdge switch { global::Doroti.Generated.Framework.Services.SwipeEdge.left => new global::Doroti.Ui.Offset(xShift__16820, _getYShiftPosition(screenSize.height)), global::Doroti.Generated.Framework.Services.SwipeEdge.right => new global::Doroti.Ui.Offset(-xShift__16820, _getYShiftPosition(screenSize.height)), null => new global::Doroti.Ui.Offset(xShift__16820, _getYShiftPosition(screenSize.height)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), end: Offset.zero) }));
    }

    internal virtual void _updateCurvedAnimations()
    {
        this._curvedAnimation?.dispose();
        this._curvedAnimationReversed?.dispose();
        _curvedAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).animation, curve: _kCommitInterval);
        _curvedAnimationReversed = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: new global::Doroti.Generated.Framework.Animation.ReverseAnimation(((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).animation), curve: _kCommitInterval);
    }

    public override void initState()
    {
        base.initState();
    }

    public override void didUpdateWidget(_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).animation, ((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)oldWidget).animation)))
        {
            _updateCurvedAnimations();
        }
        if (((!object.Equals(((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).phase, ((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)oldWidget).phase)) && (object.Equals(((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).phase, _PredictiveBackPhase__predictive_back_page_transitions_builder.commit))))
        {
            _updateAnimations(MediaQuery.sizeOf(this.context));
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _updateCurvedAnimations();
        _updateAnimations(MediaQuery.sizeOf(this.context));
    }

    public override void dispose()
    {
        this._curvedAnimation!.dispose();
        this._curvedAnimationReversed!.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: ((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).animation, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, child) => {
_lastBounceAnimationValue = ((global::Doroti.Generated.Framework.Animation.ProxyAnimation)this._bounceAnimation).value;
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.Transform.CreateScale(scale: this._scaleTween.evaluate(this._bounceAnimation), child: global::Doroti.Generated.Framework.Widgets.Transform.CreateTranslate(offset: (((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).phase switch { _PredictiveBackPhase__predictive_back_page_transitions_builder.commit => ((global::Doroti.Generated.Framework.Animation.Animation<Offset>)this._positionAnimation).value, _ => _lastDrag = new global::Doroti.Ui.Offset(((global::Doroti.Generated.Framework.Animation.Animation<Offset>)this._positionAnimation).value.dx, _getYShiftPosition(MediaQuery.heightOf(context))) }), child: new global::Doroti.Generated.Framework.Widgets.Opacity(opacity: this._opacityTween.evaluate(this._commitAnimation), child: new global::Doroti.Generated.Framework.Widgets.ClipRRect(borderRadius: (MediaQuery.displayCornerRadiiOf(context) ?? global::Doroti.Generated.Framework.Painting.BorderRadius.CreateCircular(this._borderRadiusTween.evaluate(this._bounceAnimation))), child: child)))));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: ((_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Generated.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
    }

}

internal class _PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual _PredictiveBackPhase__predictive_back_page_transitions_builder phase { get; private set; } = default!;
    public virtual global::System.Func<bool> getIsCurrent { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder(global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::System.Func<bool> getIsCurrent, _PredictiveBackPhase__predictive_back_page_transitions_builder phase, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.getIsCurrent = getIsCurrent;
        this.phase = phase;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PredictiveBackFullscreenPageTransitionState__predictive_back_page_transitions_builder());
}

internal class _PredictiveBackFullscreenPageTransitionState__predictive_back_page_transitions_builder : global::Doroti.Generated.Framework.Widgets.State<_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder>
{
    internal const double _kScaleStart = 1.0;
    internal const double _kScaleCommit = 0.95;
    internal const double _kOpacityFullyOpened = 1.0;
    internal const double _kOpacityStartTransition = 0.95;
    internal const double _kCommitAt = 0.65;
    internal static double _kWeightPreCommit => _kCommitAt;
    internal static double _kWeightPostCommit = (1L - _kWeightPreCommit);
    internal const double _kScreenWidthDivisionFactor = 20.0;
    internal const double _kXShiftAdjustment = 8.0;
    internal static Duration _kCommitDuration = Duration.Create(milliseconds: 100L);
    internal virtual global::Doroti.Generated.Framework.Animation.Animatable<double> _primaryOpacityTween { get; private set; } = ((global::Doroti.Generated.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: _kOpacityStartTransition, end: _kOpacityFullyOpened));
    internal virtual global::Doroti.Generated.Framework.Animation.Animatable<double> _primaryScaleTween { get; private set; } = ((global::Doroti.Generated.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Generated.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: _kScaleStart, end: _kScaleStart), weight: _kWeightPreCommit), new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: _kScaleCommit, end: _kScaleStart), weight: _kWeightPostCommit) }));
    internal virtual global::Doroti.Generated.Framework.Animation.ConstantTween<double> _secondaryScaleTweenCurrent { get; private set; } = new global::Doroti.Generated.Framework.Animation.ConstantTween<double>(_kScaleStart);
    internal virtual global::Doroti.Generated.Framework.Animation.TweenSequence<double> _secondaryTweenScale { get; private set; } = new global::Doroti.Generated.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: _kScaleCommit, end: _kScaleStart), weight: _kWeightPreCommit), new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: _kScaleStart, end: _kScaleStart), weight: _kWeightPostCommit) });
    internal virtual global::Doroti.Generated.Framework.Animation.ConstantTween<double> _secondaryOpacityTweenCurrent { get; private set; } = new global::Doroti.Generated.Framework.Animation.ConstantTween<double>(_kOpacityFullyOpened);
    internal virtual global::Doroti.Generated.Framework.Animation.TweenSequence<double> _secondaryOpacityTween { get; private set; } = new global::Doroti.Generated.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: _kOpacityFullyOpened, end: _kOpacityStartTransition), weight: _kWeightPreCommit), new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: _kOpacityFullyOpened, end: _kOpacityFullyOpened), weight: _kWeightPostCommit) });
    internal virtual global::Doroti.Generated.Framework.Animation.Animatable<Offset> _primaryPositionTween { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animatable<Offset> _secondaryPositionTween { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animatable<Offset> _secondaryCurrentPositionTween { get; set; } = default!;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        double screenWidth__23637 = MediaQuery.widthOf(this.context);
        double xShift__23697 = (((screenWidth__23637 / _kScreenWidthDivisionFactor)) - _kXShiftAdjustment);
        _primaryPositionTween = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animatable<Offset>>(new global::Doroti.Generated.Framework.Animation.TweenSequence<global::Doroti.Ui.Offset>(new List<global::Doroti.Generated.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Offset>> { new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Offset>(tween: new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: Offset.zero), weight: _kWeightPreCommit), new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Offset>(tween: new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(xShift__23697, 0.0), end: Offset.zero), weight: _kWeightPostCommit) }.Cast<global::Doroti.Generated.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Offset>>().ToList()));
        _secondaryCurrentPositionTween = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animatable<Offset>>(new global::Doroti.Generated.Framework.Animation.ConstantTween<global::Doroti.Ui.Offset>(Offset.zero));
        _secondaryPositionTween = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animatable<Offset>>(new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(xShift__23697, 0.0), end: Offset.zero));
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _secondaryAnimatedBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        bool isCurrent__24417 = this.widget.getIsCurrent();
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.Transform.CreateTranslate(offset: (isCurrent__24417 ? this._secondaryCurrentPositionTween.evaluate(((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).secondaryAnimation) : this._secondaryPositionTween.evaluate(((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).secondaryAnimation)), child: global::Doroti.Generated.Framework.Widgets.Transform.CreateScale(scale: (isCurrent__24417 ? this._secondaryScaleTweenCurrent.evaluate(((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).secondaryAnimation) : this._secondaryTweenScale.evaluate(((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).secondaryAnimation)), child: new global::Doroti.Generated.Framework.Widgets.Opacity(opacity: (isCurrent__24417 ? this._secondaryOpacityTweenCurrent.evaluate(((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).secondaryAnimation) : this._secondaryOpacityTween.evaluate(((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).secondaryAnimation)), child: child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _primaryAnimatedBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.Transform.CreateTranslate(offset: this._primaryPositionTween.evaluate(((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).animation), child: global::Doroti.Generated.Framework.Widgets.Transform.CreateScale(scale: this._primaryScaleTween.evaluate(((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).animation), child: new global::Doroti.Generated.Framework.Widgets.Opacity(opacity: this._primaryOpacityTween.evaluate(((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).animation), child: new global::Doroti.Generated.Framework.Widgets.AnimatedOpacity(opacity: (((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).phase switch { _PredictiveBackPhase__predictive_back_page_transitions_builder.commit => 0.0, _ => ((((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).animation.value < _kCommitAt) ? 0.0 : 1.0) }), duration: _kCommitDuration, child: child)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: ((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).secondaryAnimation, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)this._secondaryAnimatedBuilder, child: new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: ((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).animation, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)this._primaryAnimatedBuilder, child: new global::Doroti.Generated.Framework.Widgets.ClipRRect(borderRadius: (MediaQuery.displayCornerRadiiOf(context) ?? global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(_PredictiveBackSharedElementPageTransitionState__predictive_back_page_transitions_builder._kDeviceBorderRadius))), child: ((_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder)this.widget).child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
