// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/animated_size.dart
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

public class AnimatedSize : StatefulWidget
{
    public virtual Widget? child { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve curve { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual Duration? reverseDuration { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::System.Action? onEnd { get; private set; }

    public AnimatedSize(global::Doroti.Framework.Foundation.Key? key = null, Widget? child = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Animation.Curve curve = default!, Duration duration = default!, Duration? reverseDuration = null, Clip clipBehavior = Clip.hardEdge, global::System.Action? onEnd = null) : base(key: key)
    {
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Framework.Painting.Alignment.center;
        global::Doroti.Framework.Animation.Curve __curve = curve ?? global::Doroti.Framework.Animation.Curves.linear;
        this.child = child;
        this.alignment = __alignment;
        this.curve = __curve;
        this.duration = duration;
        this.reverseDuration = reverseDuration;
        this.clipBehavior = clipBehavior;
        this.onEnd = onEnd;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AnimatedSizeState__animated_size());
}

internal class _AnimatedSizeState__animated_size : State<AnimatedSize>, SingleTickerProviderStateMixin<AnimatedSize>
{
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _AnimatedSize__animated_size(alignment: ((AnimatedSize)this.widget).alignment, curve: ((AnimatedSize)this.widget).curve, duration: ((AnimatedSize)this.widget).duration, reverseDuration: ((AnimatedSize)this.widget).reverseDuration, vsync: this, clipBehavior: ((AnimatedSize)this.widget).clipBehavior, onEnd: () => ((AnimatedSize)this.widget).onEnd(), child: ((AnimatedSize)this.widget).child));
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

    public override void dispose()
    {
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
        this._tickerModeNotifier = null;
        base.dispose();
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

internal class _AnimatedSize__animated_size : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve curve { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual Duration? reverseDuration { get; private set; }
    public virtual global::Doroti.Framework.Scheduler.TickerProvider vsync { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::System.Action? onEnd { get; private set; }

    internal _AnimatedSize__animated_size(Widget? child = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Animation.Curve curve = default!, Duration duration = default!, Duration? reverseDuration = null, global::Doroti.Framework.Scheduler.TickerProvider vsync = default!, Clip clipBehavior = Clip.hardEdge, global::System.Action? onEnd = null) : base(child: child)
    {
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Framework.Painting.Alignment.center;
        global::Doroti.Framework.Animation.Curve __curve = curve ?? global::Doroti.Framework.Animation.Curves.linear;
        this.alignment = __alignment;
        this.curve = __curve;
        this.duration = duration;
        this.reverseDuration = reverseDuration;
        this.vsync = vsync;
        this.clipBehavior = clipBehavior;
        this.onEnd = onEnd;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderAnimatedSize(alignment: this.alignment, duration: this.duration, reverseDuration: this.reverseDuration, curve: this.curve, vsync: this.vsync, textDirection: Directionality.maybeOf(context), clipBehavior: this.clipBehavior, onEnd: () => this.onEnd()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderAnimatedSize)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderAnimatedSize>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alignment = this.alignment;
    __cascade.duration = this.duration;
    __cascade.reverseDuration = this.reverseDuration;
    __cascade.curve = this.curve;
    __cascade.vsync = this.vsync;
    __cascade.textDirection = Directionality.maybeOf(context);
    __cascade.clipBehavior = this.clipBehavior;
    __cascade.onEnd = this.onEnd;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, defaultValue: global::Doroti.Framework.Painting.Alignment.topCenter));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("duration", this.duration.inMilliseconds, unit: "ms"));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("reverseDuration", this.reverseDuration?.inMilliseconds, unit: "ms", defaultValue: null));
    }

}

