// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/animated_cross_fade.dart
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

public enum CrossFadeState
{
    showFirst,
    showSecond
}

public delegate Widget AnimatedCrossFadeBuilder(Widget topChild, global::Doroti.Framework.Foundation.Key topChildKey, Widget bottomChild, global::Doroti.Framework.Foundation.Key bottomChildKey);

public class AnimatedCrossFade : StatefulWidget
{
    public virtual Widget firstChild { get; private set; } = default!;
    public virtual Widget secondChild { get; private set; } = default!;
    public virtual CrossFadeState crossFadeState { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual Duration? reverseDuration { get; private set; }
    public virtual global::Doroti.Framework.Animation.Curve firstCurve { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve secondCurve { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve sizeCurve { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual global::System.Func<Widget, global::Doroti.Framework.Foundation.Key, Widget, global::Doroti.Framework.Foundation.Key, Widget> layoutBuilder { get; private set; } = default!;
    public virtual bool excludeBottomFocus { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::System.Action? onEnd { get; private set; }

    public AnimatedCrossFade(global::Doroti.Framework.Foundation.Key? key = null, Widget firstChild = default!, Widget secondChild = default!, global::Doroti.Framework.Animation.Curve firstCurve = default!, global::Doroti.Framework.Animation.Curve secondCurve = default!, global::Doroti.Framework.Animation.Curve sizeCurve = default!, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, CrossFadeState crossFadeState = default!, Duration duration = default!, Duration? reverseDuration = null, global::System.Func<Widget, global::Doroti.Framework.Foundation.Key, Widget, global::Doroti.Framework.Foundation.Key, Widget> layoutBuilder = default!, bool excludeBottomFocus = true, Clip clipBehavior = Clip.hardEdge, global::System.Action? onEnd = null) : base(key: key)
    {
        global::Doroti.Framework.Animation.Curve __firstCurve = firstCurve ?? global::Doroti.Framework.Animation.Curves.linear;
        global::Doroti.Framework.Animation.Curve __secondCurve = secondCurve ?? global::Doroti.Framework.Animation.Curves.linear;
        global::Doroti.Framework.Animation.Curve __sizeCurve = sizeCurve ?? global::Doroti.Framework.Animation.Curves.linear;
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Framework.Painting.Alignment.topCenter;
        global::System.Func<Widget, global::Doroti.Framework.Foundation.Key, Widget, global::Doroti.Framework.Foundation.Key, Widget> __layoutBuilder = layoutBuilder ?? defaultLayoutBuilder;
        this.firstChild = firstChild;
        this.secondChild = secondChild;
        this.firstCurve = __firstCurve;
        this.secondCurve = __secondCurve;
        this.sizeCurve = __sizeCurve;
        this.alignment = __alignment;
        this.crossFadeState = crossFadeState;
        this.duration = duration;
        this.reverseDuration = reverseDuration;
        this.layoutBuilder = __layoutBuilder;
        this.excludeBottomFocus = excludeBottomFocus;
        this.clipBehavior = clipBehavior;
        this.onEnd = onEnd;
    }

    public static Widget defaultLayoutBuilder(Widget topChild, global::Doroti.Framework.Foundation.Key topChildKey, Widget bottomChild, global::Doroti.Framework.Foundation.Key bottomChildKey)
    {
        return ((Widget)(object?)new Stack(clipBehavior: Clip.none, children: new List<Widget> { new Positioned(key: bottomChildKey, left: 0.0, top: 0.0, right: 0.0, child: bottomChild), new Positioned(key: topChildKey, child: topChild) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AnimatedCrossFadeState__animated_cross_fade());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<CrossFadeState>("crossFadeState", this.crossFadeState));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, defaultValue: global::Doroti.Framework.Painting.Alignment.topCenter));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("duration", this.duration.inMilliseconds, unit: "ms"));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("reverseDuration", this.reverseDuration?.inMilliseconds, unit: "ms", defaultValue: null));
    }

}

internal class _AnimatedCrossFadeState__animated_cross_fade : State<AnimatedCrossFade>, TickerProviderStateMixin<AnimatedCrossFade>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _firstAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _secondAnimation { get; set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(duration: ((AnimatedCrossFade)this.widget).duration, reverseDuration: ((AnimatedCrossFade)this.widget).reverseDuration, vsync: this);
        if ((object.Equals(((AnimatedCrossFade)this.widget).crossFadeState, CrossFadeState.showSecond)))
        {
            this._controller.value = 1.0;
        }
        _firstAnimation = _initAnimation(((AnimatedCrossFade)this.widget).firstCurve, true);
        _secondAnimation = _initAnimation(((AnimatedCrossFade)this.widget).secondCurve, false);
        this._controller.addStatusListener(((AnimationStatusListener)((status) => {
setState(((global::System.Action)(() => {
})));
if (((object.Equals(status, global::Doroti.Framework.Animation.AnimationStatus.completed)) || (object.Equals(status, global::Doroti.Framework.Animation.AnimationStatus.dismissed))))
{
    ((AnimatedCrossFade)this.widget).onEnd?.Invoke();
}
})));
    }

    internal virtual global::Doroti.Framework.Animation.Animation<double> _initAnimation(global::Doroti.Framework.Animation.Curve curve, bool inverted)
    {
        global::Doroti.Framework.Animation.Animation<double> result__11620 = ((global::Doroti.Framework.Animation.Animation<double>)(object?)this._controller.drive(new global::Doroti.Framework.Animation.CurveTween(curve: curve)));
        if (inverted)
        {
            result__11620 = result__11620.drive(new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0));
        }
        return result__11620;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(AnimatedCrossFade oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((AnimatedCrossFade)this.widget).duration, ((AnimatedCrossFade)oldWidget).duration)))
        {
            this._controller.duration = ((AnimatedCrossFade)this.widget).duration;
        }
        if ((!object.Equals(((AnimatedCrossFade)this.widget).reverseDuration, ((AnimatedCrossFade)oldWidget).reverseDuration)))
        {
            this._controller.reverseDuration = ((AnimatedCrossFade)this.widget).reverseDuration;
        }
        if ((!object.Equals(((AnimatedCrossFade)this.widget).firstCurve, ((AnimatedCrossFade)oldWidget).firstCurve)))
        {
            _firstAnimation = _initAnimation(((AnimatedCrossFade)this.widget).firstCurve, true);
        }
        if ((!object.Equals(((AnimatedCrossFade)this.widget).secondCurve, ((AnimatedCrossFade)oldWidget).secondCurve)))
        {
            _secondAnimation = _initAnimation(((AnimatedCrossFade)this.widget).secondCurve, false);
        }
        if ((!object.Equals(((AnimatedCrossFade)this.widget).crossFadeState, ((AnimatedCrossFade)oldWidget).crossFadeState)))
        {
            switch (((AnimatedCrossFade)this.widget).crossFadeState)
            {
                case CrossFadeState.showFirst:
                    {
                        this._controller.reverse();
                        break;
                    }
                case CrossFadeState.showSecond:
                    {
                        this._controller.forward();
                        break;
                    }
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        global::Doroti.Framework.Foundation.Key kFirstChildKey__12790 = ((global::Doroti.Framework.Foundation.Key)(object?)new global::Doroti.Framework.Foundation.ValueKey<CrossFadeState>(CrossFadeState.showFirst));
        global::Doroti.Framework.Foundation.Key kSecondChildKey__12873 = ((global::Doroti.Framework.Foundation.Key)(object?)new global::Doroti.Framework.Foundation.ValueKey<CrossFadeState>(CrossFadeState.showSecond));
        global::Doroti.Framework.Foundation.Key topKey__12958 = default!;
        Widget topChild__12977 = default!;
        global::Doroti.Framework.Animation.Animation<double> topAnimation__13015 = default!;
        global::Doroti.Framework.Foundation.Key bottomKey__13043 = default!;
        Widget bottomChild__13065 = default!;
        global::Doroti.Framework.Animation.Animation<double> bottomAnimation__13106 = default!;
        if (this._controller.isForwardOrCompleted)
        {
            topKey__12958 = kSecondChildKey__12873;
            topChild__12977 = ((AnimatedCrossFade)this.widget).secondChild;
            topAnimation__13015 = this._secondAnimation;
            bottomKey__13043 = kFirstChildKey__12790;
            bottomChild__13065 = ((AnimatedCrossFade)this.widget).firstChild;
            bottomAnimation__13106 = this._firstAnimation;
        }
        else
        {
            topKey__12958 = kFirstChildKey__12790;
            topChild__12977 = ((AnimatedCrossFade)this.widget).firstChild;
            topAnimation__13015 = this._firstAnimation;
            bottomKey__13043 = kSecondChildKey__12873;
            bottomChild__13065 = ((AnimatedCrossFade)this.widget).secondChild;
            bottomAnimation__13106 = this._secondAnimation;
        }
        bottomChild__13065 = DartRuntimePrimitives.ConvertValue<Widget>(new TickerMode(key: bottomKey__13043, enabled: ((global::Doroti.Framework.Animation.AnimationController)this._controller).isAnimating, child: new IgnorePointer(child: new ExcludeSemantics(child: new ExcludeFocus(excluding: ((AnimatedCrossFade)this.widget).excludeBottomFocus, child: new FadeTransition(opacity: bottomAnimation__13106, child: bottomChild__13065))))));
        topChild__12977 = DartRuntimePrimitives.ConvertValue<Widget>(new TickerMode(key: topKey__12958, enabled: true, child: new IgnorePointer(ignoring: false, child: new ExcludeSemantics(excluding: false, child: new ExcludeFocus(excluding: false, child: new FadeTransition(opacity: topAnimation__13015, child: topChild__12977))))));
        return ((Widget)(object?)new ClipRect(clipBehavior: ((AnimatedCrossFade)this.widget).clipBehavior, child: new AnimatedSize(alignment: ((AnimatedCrossFade)this.widget).alignment, duration: ((AnimatedCrossFade)this.widget).duration, reverseDuration: ((AnimatedCrossFade)this.widget).reverseDuration, curve: ((AnimatedCrossFade)this.widget).sizeCurve, clipBehavior: ((AnimatedCrossFade)this.widget).clipBehavior, child: this.widget.layoutBuilder(topChild__12977, topKey__12958, bottomChild__13065, bottomKey__13043))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
        description.add(new global::Doroti.Framework.Foundation.EnumProperty<CrossFadeState>("crossFadeState", ((AnimatedCrossFade)this.widget).crossFadeState));
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Animation.AnimationController>("controller", this._controller, showName: false));
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", ((AnimatedCrossFade)this.widget).alignment, defaultValue: global::Doroti.Framework.Painting.Alignment.topCenter));
        description.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.Clip>("clipBehavior", ((AnimatedCrossFade)this.widget).clipBehavior, defaultValue: Clip.hardEdge));
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<_WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

}

