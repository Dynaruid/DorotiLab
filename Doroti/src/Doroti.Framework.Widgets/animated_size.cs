// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/animated_size.dart
using Doroti.Runtime;
using Doroti.Ui;

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
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? Alignment.center;
        global::Doroti.Framework.Animation.Curve __curve = curve ?? Curves.linear;
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
        return new _AnimatedSize__animated_size(alignment: widget.alignment, curve: widget.curve, duration: widget.duration, reverseDuration: widget.reverseDuration, vsync: this, clipBehavior: widget.clipBehavior, onEnd: widget.onEnd, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _ticker = new global::Doroti.Framework.Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
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
        TickerModeData values = _tickerModeNotifier!.value;
        if (_ticker is not null)
        {
            _ticker!.muted = !values.enabled;
            _ticker!.forceFrames = values.forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
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
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? Alignment.center;
        global::Doroti.Framework.Animation.Curve __curve = curve ?? Curves.linear;
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
        return new global::Doroti.Framework.Rendering.RenderAnimatedSize(alignment: alignment, duration: duration, reverseDuration: reverseDuration, curve: curve, vsync: vsync, textDirection: Directionality.maybeOf(context), clipBehavior: clipBehavior, onEnd: onEnd);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderAnimatedSize)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderAnimatedSize>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alignment = alignment;
    __cascade.duration = duration;
    __cascade.reverseDuration = reverseDuration;
    __cascade.curve = curve;
    __cascade.vsync = vsync;
    __cascade.textDirection = Directionality.maybeOf(context);
    __cascade.clipBehavior = clipBehavior;
    __cascade.onEnd = onEnd;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", alignment, defaultValue: Alignment.topCenter));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("duration", duration.inMilliseconds, unit: "ms"));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("reverseDuration", reverseDuration?.inMilliseconds, unit: "ms", defaultValue: null));
    }

}

