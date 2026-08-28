// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/repeating_animation_builder.dart
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

public enum RepeatMode
{
    restart,
    reverse
}

public class RepeatingAnimationBuilder<T> : StatefulWidget
{
    public virtual global::Doroti.Framework.Animation.Animatable<T> animatable { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve curve { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, T, Widget?, Widget> builder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }
    public virtual RepeatMode repeatMode { get; private set; } = default!;
    public virtual bool paused { get; private set; } = default!;

    public RepeatingAnimationBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animatable<T> animatable = default!, Duration duration = default!, global::Doroti.Framework.Animation.Curve curve = default!, RepeatMode repeatMode = RepeatMode.restart, bool paused = false, global::System.Func<BuildContext, T, Widget?, Widget> builder = default!, Widget? child = null) : base(key: key)
    {
        global::Doroti.Framework.Animation.Curve __curve = curve ?? global::Doroti.Framework.Animation.Curves.linear;
        this.animatable = animatable;
        this.duration = duration;
        this.curve = __curve;
        this.repeatMode = repeatMode;
        this.paused = paused;
        this.builder = builder;
        this.child = child;
    }

    public override IState createState()
    {
        return ((IState)(object?)new _RepeatingAnimationBuilderState__repeating_animation_builder<T>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RepeatingAnimationBuilderState__repeating_animation_builder<T> : State<RepeatingAnimationBuilder<T>>, SingleTickerProviderStateMixin<RepeatingAnimationBuilder<T>>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _curvedAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(duration: ((RepeatingAnimationBuilder<T>)(object)this.widget).duration, vsync: this);
        _curvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._controller, curve: ((RepeatingAnimationBuilder<T>)(object)this.widget).curve);
        if (!((RepeatingAnimationBuilder<T>)(object)this.widget).paused)
        {
            this._controller.repeat(reverse: (object.Equals(((RepeatingAnimationBuilder<T>)(object)this.widget).repeatMode, RepeatMode.reverse)));
        }
    }

    public override void didUpdateWidget(RepeatingAnimationBuilder<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((RepeatingAnimationBuilder<T>)(object)this.widget).duration, ((RepeatingAnimationBuilder<T>)oldWidget).duration)))
        {
            this._controller.duration = ((RepeatingAnimationBuilder<T>)(object)this.widget).duration;
        }
        if ((!object.Equals(((RepeatingAnimationBuilder<T>)(object)this.widget).curve, ((RepeatingAnimationBuilder<T>)oldWidget).curve)))
        {
            this._curvedAnimation.curve = ((RepeatingAnimationBuilder<T>)(object)this.widget).curve;
        }
        if (((RepeatingAnimationBuilder<T>)(object)this.widget).paused)
        {
            if ((!((RepeatingAnimationBuilder<T>)oldWidget).paused || ((global::Doroti.Framework.Animation.AnimationController)this._controller).isAnimating))
            {
                this._controller.stop(canceled: false);
            }
            return;
        }
        bool shouldRestart = (((((RepeatingAnimationBuilder<T>)oldWidget).paused || (!object.Equals(((RepeatingAnimationBuilder<T>)(object)this.widget).repeatMode, ((RepeatingAnimationBuilder<T>)oldWidget).repeatMode))) || (!object.Equals(((RepeatingAnimationBuilder<T>)(object)this.widget).duration, ((RepeatingAnimationBuilder<T>)oldWidget).duration))) || !((global::Doroti.Framework.Animation.AnimationController)this._controller).isAnimating);
        if (shouldRestart)
        {
            this._controller.repeat(reverse: (object.Equals(((RepeatingAnimationBuilder<T>)(object)this.widget).repeatMode, RepeatMode.reverse)));
        }
    }

    public override void dispose()
    {
        this._curvedAnimation.dispose();
        this._controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(this._updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new AnimatedBuilder(animation: this._curvedAnimation, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) =>
        {
            T valueLocal = ((T)(object?)((RepeatingAnimationBuilder<T>)(object)this.widget).animatable.transform(((global::Doroti.Framework.Animation.CurvedAnimation)this._curvedAnimation).value));
            return this.widget.builder(context, valueLocal, child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: ((RepeatingAnimationBuilder<T>)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._ticker = new global::Doroti.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
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
        TickerModeData values = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTicker);
        newNotifier.addListener(this._updateTicker);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

