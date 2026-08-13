// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/implicit_animations.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class BoxConstraintsTween : global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Generated.Framework.Rendering.BoxConstraints>
{
    public BoxConstraintsTween(global::Doroti.Generated.Framework.Rendering.BoxConstraints? begin = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.BoxConstraints lerp(double t) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.BoxConstraints>(BoxConstraints.lerp(this.begin, this.end, t)!);
}

public class DecorationTween : global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Generated.Framework.Painting.Decoration>
{
    public DecorationTween(global::Doroti.Generated.Framework.Painting.Decoration? begin = null, global::Doroti.Generated.Framework.Painting.Decoration? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Generated.Framework.Painting.Decoration lerp(double t) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.Decoration>(Decoration.lerp(this.begin, this.end, t)!);
}

public class EdgeInsetsTween : global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Generated.Framework.Painting.EdgeInsets>
{
    public EdgeInsetsTween(global::Doroti.Generated.Framework.Painting.EdgeInsets? begin = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Generated.Framework.Painting.EdgeInsets lerp(double t) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsets>(EdgeInsets.lerp(this.begin, this.end, t)!);
}

public class EdgeInsetsGeometryTween : global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>
{
    public EdgeInsetsGeometryTween(global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? begin = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry lerp(double t) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(EdgeInsetsGeometry.lerp(this.begin, this.end, t)!);
}

public class BorderRadiusTween : global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Generated.Framework.Painting.BorderRadius?>
{
    public BorderRadiusTween(global::Doroti.Generated.Framework.Painting.BorderRadius? begin = null, global::Doroti.Generated.Framework.Painting.BorderRadius? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Generated.Framework.Painting.BorderRadius? lerp(double t) => BorderRadius.lerp(this.begin, this.end, t);
}

public class BorderTween : global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Generated.Framework.Painting.Border?>
{
    public BorderTween(global::Doroti.Generated.Framework.Painting.Border? begin = null, global::Doroti.Generated.Framework.Painting.Border? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Generated.Framework.Painting.Border? lerp(double t) => Border.lerp(this.begin, this.end, t);
}

public class Matrix4Tween : global::Doroti.Generated.Framework.Animation.Tween<Matrix4>
{
    public Matrix4Tween(Matrix4? begin = null, Matrix4? end = null) : base(begin: begin, end: end)
    {
    }

    public override Matrix4 lerp(double t)
    {
        DartRuntimePrimitives.Assert(() => (this.begin is not null));
        DartRuntimePrimitives.Assert(() => (this.end is not null));
        var beginTranslation__6685 = new Vector3();
        var endTranslation__6730 = new Vector3();
        var beginRotation__6773 = new Quaternion();
        var endRotation__6822 = new Quaternion();
        var beginScale__6869 = new Vector3();
        var endScale__6908 = new Vector3();
        this.begin!.decompose(beginTranslation__6685, beginRotation__6773, beginScale__6869);
        this.end!.decompose(endTranslation__6730, endRotation__6822, endScale__6908);
        Vector3 lerpTranslation__7079 = ((beginTranslation__6685 * ((1.0 - t))) + (endTranslation__6730 * t));
        Quaternion lerpRotation__7232 = ((beginRotation__6773.scaled((1.0 - t)) + endRotation__6822.scaled(t))).normalized();
        Vector3 lerpScale__7344 = ((beginScale__6869 * ((1.0 - t))) + (endScale__6908 * t));
        return Matrix4.compose(lerpTranslation__7079, lerpRotation__7232, lerpScale__7344);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TextStyleTween : global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Generated.Framework.Painting.TextStyle>
{
    public TextStyleTween(global::Doroti.Generated.Framework.Painting.TextStyle? begin = null, global::Doroti.Generated.Framework.Painting.TextStyle? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Generated.Framework.Painting.TextStyle lerp(double t) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.TextStyle>(TextStyle.lerp(this.begin, this.end, t)!);
}

public abstract class ImplicitlyAnimatedWidget : StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.Curve curve { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual global::System.Action? onEnd { get; private set; }

    protected ImplicitlyAnimatedWidget(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null) : base(key: key)
    {
        global::Doroti.Generated.Framework.Animation.Curve __curve = curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear;
        this.curve = __curve;
        this.duration = duration;
        this.onEnd = onEnd;
    }

    public abstract override IState createState();
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.IntProperty("duration", this.duration.inMilliseconds, unit: "ms"));
    }

}

public delegate global::Doroti.Generated.Framework.Animation.Tween<T> TweenConstructor<T>(T targetValue);

public delegate global::Doroti.Generated.Framework.Animation.Tween<T>? TweenVisitor<T>(global::Doroti.Generated.Framework.Animation.Tween<T>? tween, T targetValue, global::System.Func<T, global::Doroti.Generated.Framework.Animation.Tween<T>> constructor);

public abstract class ImplicitlyAnimatedWidgetState<T> : State<T>, SingleTickerProviderStateMixin<T> where T : ImplicitlyAnimatedWidget
{
    private bool __late_controller_initialized;
    private global::Doroti.Generated.Framework.Animation.AnimationController __late_controller = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController controller
    {
        get
        {
            if (!__late_controller_initialized)
            {
                __late_controller = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: ((ImplicitlyAnimatedWidget)(object)this.widget).duration, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? ((Diagnosticable)this.widget).toStringShort() : null), vsync: this);
                __late_controller_initialized = true;
            }
            return __late_controller;
        }
    }
    private bool __late__animation_initialized;
    private global::Doroti.Generated.Framework.Animation.CurvedAnimation __late__animation = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _animation
    {
        get
        {
            if (!__late__animation_initialized)
            {
                __late__animation = _createCurve();
                __late__animation_initialized = true;
            }
            return __late__animation;
        }
        set { __late__animation = value; __late__animation_initialized = true; }
    }
    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animation<double>>(this._animation);
    public override void initState()
    {
        base.initState();
        this.controller.addStatusListener(((AnimationStatusListener)((status) => {
if (global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isCompleted(status))
{
    ((ImplicitlyAnimatedWidget)(object)this.widget).onEnd?.Invoke();
}
})));
        _constructTweens();
        didUpdateTweens();
    }

    public override void didUpdateWidget(T oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((ImplicitlyAnimatedWidget)(object)this.widget).curve, ((ImplicitlyAnimatedWidget)(object)oldWidget).curve)))
        {
            this._animation.dispose();
            _animation = _createCurve();
        }
        this.controller.duration = ((ImplicitlyAnimatedWidget)(object)this.widget).duration;
        if (_constructTweens())
        {
            forEachTween(((global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?>)((tween, targetValue, constructor) => {
return ((Func<global::Doroti.Generated.Framework.Animation.IDartTween?>)(() =>
{            var __cascade = tween;
            __cascade.begin = tween.evaluate(this._animation);
            __cascade.end = targetValue;
            return __cascade;        }))();
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            this.controller.forward(from: 0.0);
            didUpdateTweens();
        }
    }

    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _createCurve()
    {
        return new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this.controller, curve: ((ImplicitlyAnimatedWidget)(object)this.widget).curve);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._animation.dispose();
        this.controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual bool _constructTweens()
    {
        var shouldStartAnimation__16124 = false;
        forEachTween(((global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?>)((tween, targetValue, constructor) => {
if ((targetValue is not null))
{
    tween ??= constructor(targetValue);
    if ((!object.Equals(targetValue, ((tween.end ?? tween.begin)))))
    {
        shouldStartAnimation__16124 = true;
    }
    else
    {
        tween.end ??= tween.begin;
    }
}
else
{
    tween = null;
}
return tween;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        return shouldStartAnimation__16124;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor);
    public virtual void didUpdateTweens()
    {
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
                throw new InvalidOperationException("Dart closure completed without a value.");
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

public abstract class AnimatedWidgetBaseState<T> : ImplicitlyAnimatedWidgetState<T> where T : ImplicitlyAnimatedWidget
{
    public override void initState()
    {
        base.initState();
        this.controller.addListener(() => this._handleAnimationChanged());
    }

    internal virtual void _handleAnimationChanged()
    {
        setState(((global::System.Action)(() => {
})));
    }

}

public class AnimatedContainer : ImplicitlyAnimatedWidget
{
    public virtual Widget? child { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Decoration? decoration { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Decoration? foregroundDecoration { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual Matrix4? transform { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? transformAlignment { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public AnimatedContainer(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, Color? color = null, global::Doroti.Generated.Framework.Painting.Decoration? decoration = null, global::Doroti.Generated.Framework.Painting.Decoration? foregroundDecoration = null, double? width = null, double? height = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? margin = null, Matrix4? transform = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? transformAlignment = null, Widget? child = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.alignment = alignment;
        this.padding = padding;
        this.foregroundDecoration = foregroundDecoration;
        this.margin = margin;
        this.transform = transform;
        this.transformAlignment = transformAlignment;
        this.child = child;
        this.clipBehavior = clipBehavior;
        this.decoration = (decoration ?? (((color is not null) ? new global::Doroti.Generated.Framework.Painting.BoxDecoration(color: color) : null)));
        this.constraints = ((((width is not null) || (height is not null))) ? (constraints?.tighten(width: width, height: height) ?? global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: width, height: height)) : constraints);
        System.Diagnostics.Debug.Assert(((margin is null) || ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)margin).isNonNegative));
        System.Diagnostics.Debug.Assert(((padding is null) || ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)padding).isNonNegative));
        System.Diagnostics.Debug.Assert(((decoration is null) || decoration.debugAssertIsValid()));
        System.Diagnostics.Debug.Assert(((constraints is null) || constraints.debugAssertIsValid()));
        System.Diagnostics.Debug.Assert(((color is null) || (decoration is null)));
    }

    public override AnimatedWidgetBaseState<AnimatedContainer> createState() => DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedContainer>>(new _AnimatedContainerState__implicit_animations());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, showName: false, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.Decoration>("bg", this.decoration, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.Decoration>("fg", this.foregroundDecoration, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.BoxConstraints>("constraints", this.constraints, defaultValue: null, showName: false));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("margin", this.margin, defaultValue: null));
        properties.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<Matrix4>.CreateHas("transform", this.transform));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("transformAlignment", this.transformAlignment, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Flutter.Ui.Clip>("clipBehavior", this.clipBehavior));
    }

}

internal class _AnimatedContainerState__implicit_animations : AnimatedWidgetBaseState<AnimatedContainer>
{
    internal virtual global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween? _alignment { get; set; } = default;
    internal virtual EdgeInsetsGeometryTween? _padding { get; set; } = default;
    internal virtual DecorationTween? _decoration { get; set; } = default;
    internal virtual DecorationTween? _foregroundDecoration { get; set; } = default;
    internal virtual BoxConstraintsTween? _constraints { get; set; } = default;
    internal virtual EdgeInsetsGeometryTween? _margin { get; set; } = default;
    internal virtual Matrix4Tween? _transform { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween? _transformAlignment { get; set; } = default;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _alignment = ((global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween?)(object?)visitor(this._alignment, ((AnimatedContainer)(object)this.widget).alignment, ((value) => new global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween(begin: ((global::Doroti.Generated.Framework.Painting.AlignmentGeometry?)(object?)value)!))))!;
        _padding = ((EdgeInsetsGeometryTween?)(object?)visitor(this._padding, ((AnimatedContainer)(object)this.widget).padding, ((value) => new EdgeInsetsGeometryTween(begin: ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?)(object?)value)!))))!;
        _decoration = ((DecorationTween?)(object?)visitor(this._decoration, ((AnimatedContainer)(object)this.widget).decoration, ((value) => new DecorationTween(begin: ((global::Doroti.Generated.Framework.Painting.Decoration?)(object?)value)!))))!;
        _foregroundDecoration = ((DecorationTween?)(object?)visitor(this._foregroundDecoration, ((AnimatedContainer)(object)this.widget).foregroundDecoration, ((value) => new DecorationTween(begin: ((global::Doroti.Generated.Framework.Painting.Decoration?)(object?)value)!))))!;
        _constraints = ((BoxConstraintsTween?)(object?)visitor(this._constraints, ((AnimatedContainer)(object)this.widget).constraints, ((value) => new BoxConstraintsTween(begin: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints?)(object?)value)!))))!;
        _margin = ((EdgeInsetsGeometryTween?)(object?)visitor(this._margin, ((AnimatedContainer)(object)this.widget).margin, ((value) => new EdgeInsetsGeometryTween(begin: ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?)(object?)value)!))))!;
        _transform = ((Matrix4Tween?)(object?)visitor(this._transform, ((AnimatedContainer)(object)this.widget).transform, ((value) => new Matrix4Tween(begin: ((Matrix4?)(object?)value)!))))!;
        _transformAlignment = ((global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween?)(object?)visitor(this._transformAlignment, ((AnimatedContainer)(object)this.widget).transformAlignment, ((value) => new global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween(begin: ((global::Doroti.Generated.Framework.Painting.AlignmentGeometry?)(object?)value)!))))!;
    }

    public override Widget build(BuildContext context)
    {
        global::Doroti.Generated.Framework.Animation.Animation<double> animation__31381 = this.animation;
        return ((Widget)(object?)new Container(alignment: this._alignment?.evaluate(animation__31381), padding: this._padding?.evaluate(animation__31381), decoration: this._decoration?.evaluate(animation__31381), foregroundDecoration: this._foregroundDecoration?.evaluate(animation__31381), constraints: this._constraints?.evaluate(animation__31381), margin: this._margin?.evaluate(animation__31381), transform: this._transform?.evaluate(animation__31381), transformAlignment: this._transformAlignment?.evaluate(animation__31381), clipBehavior: ((AnimatedContainer)(object)this.widget).clipBehavior, child: ((AnimatedContainer)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween>("alignment", this._alignment, showName: false, defaultValue: null));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<EdgeInsetsGeometryTween>("padding", this._padding, defaultValue: null));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<DecorationTween>("bg", this._decoration, defaultValue: null));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<DecorationTween>("fg", this._foregroundDecoration, defaultValue: null));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<BoxConstraintsTween>("constraints", this._constraints, showName: false, defaultValue: null));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<EdgeInsetsGeometryTween>("margin", this._margin, defaultValue: null));
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<Matrix4Tween>.CreateHas("transform", this._transform));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween>("transformAlignment", this._transformAlignment, defaultValue: null));
    }

}

public class AnimatedPadding : ImplicitlyAnimatedWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public AnimatedPadding(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding = default!, Widget? child = null, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.padding = padding;
        this.child = child;
        System.Diagnostics.Debug.Assert(((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)padding).isNonNegative);
    }

    public override AnimatedWidgetBaseState<AnimatedPadding> createState() => DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedPadding>>(new _AnimatedPaddingState__implicit_animations());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding));
    }

}

internal class _AnimatedPaddingState__implicit_animations : AnimatedWidgetBaseState<AnimatedPadding>
{
    internal virtual EdgeInsetsGeometryTween? _padding { get; set; } = default;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _padding = ((EdgeInsetsGeometryTween?)(object?)visitor(this._padding, ((AnimatedPadding)(object)this.widget).padding, ((value) => new EdgeInsetsGeometryTween(begin: ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?)(object?)value)!))))!;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Padding(padding: this._padding!.evaluate(this.animation).clamp(global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry.infinity), child: ((AnimatedPadding)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<EdgeInsetsGeometryTween>("padding", this._padding, defaultValue: null));
    }

}

public class AnimatedAlign : ImplicitlyAnimatedWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Widget? child { get; private set; }
    public virtual double? heightFactor { get; private set; }
    public virtual double? widthFactor { get; private set; }

    public AnimatedAlign(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, Widget? child = null, double? heightFactor = null, double? widthFactor = null, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.alignment = alignment;
        this.child = child;
        this.heightFactor = heightFactor;
        this.widthFactor = widthFactor;
        System.Diagnostics.Debug.Assert(((widthFactor is null) || (widthFactor >= 0.0)));
        System.Diagnostics.Debug.Assert(((heightFactor is null) || (heightFactor >= 0.0)));
    }

    public override AnimatedWidgetBaseState<AnimatedAlign> createState() => DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedAlign>>(new _AnimatedAlignState__implicit_animations());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
    }

}

internal class _AnimatedAlignState__implicit_animations : AnimatedWidgetBaseState<AnimatedAlign>
{
    internal virtual global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween? _alignment { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _heightFactorTween { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _widthFactorTween { get; set; } = default;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _alignment = ((global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween?)(object?)visitor(this._alignment, ((AnimatedAlign)(object)this.widget).alignment, ((value) => new global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween(begin: ((global::Doroti.Generated.Framework.Painting.AlignmentGeometry?)(object?)value)!))))!;
        if ((((AnimatedAlign)(object)this.widget).heightFactor is not null))
        {
            _heightFactorTween = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._heightFactorTween, ((AnimatedAlign)(object)this.widget).heightFactor, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        }
        if ((((AnimatedAlign)(object)this.widget).widthFactor is not null))
        {
            _widthFactorTween = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._widthFactorTween, ((AnimatedAlign)(object)this.widget).widthFactor, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        }
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Align(alignment: this._alignment!.evaluate(this.animation)!, heightFactor: this._heightFactorTween?.evaluate(this.animation), widthFactor: this._widthFactorTween?.evaluate(this.animation), child: ((AnimatedAlign)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween>("alignment", this._alignment, defaultValue: null));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>("widthFactor", this._widthFactorTween, defaultValue: null));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>("heightFactor", this._heightFactorTween, defaultValue: null));
    }

}

public class AnimatedPositioned : ImplicitlyAnimatedWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual double? left { get; private set; }
    public virtual double? top { get; private set; }
    public virtual double? right { get; private set; }
    public virtual double? bottom { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }

    public AnimatedPositioned(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, double? left = null, double? top = null, double? right = null, double? bottom = null, double? width = null, double? height = null, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.child = child;
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
        this.width = width;
        this.height = height;
        System.Diagnostics.Debug.Assert((((left is null) || (right is null)) || (width is null)));
        System.Diagnostics.Debug.Assert((((top is null) || (bottom is null)) || (height is null)));
    }

    public static AnimatedPositioned CreateFromRect(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, Rect rect = default!, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null)
    {
        var __instance = new AnimatedPositioned(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        global::Doroti.Generated.Framework.Animation.Curve __curve = curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear;
        __instance.child = child;
        __instance.left = rect.left;
        __instance.top = rect.top;
        __instance.width = rect.width;
        __instance.height = rect.height;
        __instance.right = null;
        __instance.bottom = null;
        return __instance;
    }

    public override AnimatedWidgetBaseState<AnimatedPositioned> createState() => DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedPositioned>>(new _AnimatedPositionedState__implicit_animations());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("left", this.left, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("top", this.top, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("right", this.right, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("bottom", this.bottom, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("width", this.width, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: null));
    }

}

internal class _AnimatedPositionedState__implicit_animations : AnimatedWidgetBaseState<AnimatedPositioned>
{
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _left { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _top { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _right { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _bottom { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _width { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _height { get; set; } = default;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _left = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._left, ((AnimatedPositioned)(object)this.widget).left, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _top = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._top, ((AnimatedPositioned)(object)this.widget).top, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _right = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._right, ((AnimatedPositioned)(object)this.widget).right, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _bottom = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._bottom, ((AnimatedPositioned)(object)this.widget).bottom, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _width = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._width, ((AnimatedPositioned)(object)this.widget).width, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _height = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._height, ((AnimatedPositioned)(object)this.widget).height, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Positioned(left: this._left?.evaluate(this.animation), top: this._top?.evaluate(this.animation), right: this._right?.evaluate(this.animation), bottom: this._bottom?.evaluate(this.animation), width: this._width?.evaluate(this.animation), height: this._height?.evaluate(this.animation), child: ((AnimatedPositioned)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>.CreateHas("left", this._left));
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>.CreateHas("top", this._top));
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>.CreateHas("right", this._right));
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>.CreateHas("bottom", this._bottom));
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>.CreateHas("width", this._width));
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>.CreateHas("height", this._height));
    }

}

public class AnimatedPositionedDirectional : ImplicitlyAnimatedWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual double? start { get; private set; }
    public virtual double? top { get; private set; }
    public virtual double? end { get; private set; }
    public virtual double? bottom { get; private set; }
    public virtual double? width { get; private set; }
    public virtual double? height { get; private set; }

    public AnimatedPositionedDirectional(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, double? start = null, double? top = null, double? end = null, double? bottom = null, double? width = null, double? height = null, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.child = child;
        this.start = start;
        this.top = top;
        this.end = end;
        this.bottom = bottom;
        this.width = width;
        this.height = height;
        System.Diagnostics.Debug.Assert((((start is null) || (end is null)) || (width is null)));
        System.Diagnostics.Debug.Assert((((top is null) || (bottom is null)) || (height is null)));
    }

    public override AnimatedWidgetBaseState<AnimatedPositionedDirectional> createState() => DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedPositionedDirectional>>(new _AnimatedPositionedDirectionalState__implicit_animations());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("start", this.start, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("top", this.top, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("end", this.end, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("bottom", this.bottom, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("width", this.width, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: null));
    }

}

internal class _AnimatedPositionedDirectionalState__implicit_animations : AnimatedWidgetBaseState<AnimatedPositionedDirectional>
{
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _start { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _top { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _end { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _bottom { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _width { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _height { get; set; } = default;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _start = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._start, ((AnimatedPositionedDirectional)(object)this.widget).start, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _top = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._top, ((AnimatedPositionedDirectional)(object)this.widget).top, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _end = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._end, ((AnimatedPositionedDirectional)(object)this.widget).end, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _bottom = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._bottom, ((AnimatedPositionedDirectional)(object)this.widget).bottom, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _width = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._width, ((AnimatedPositionedDirectional)(object)this.widget).width, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _height = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._height, ((AnimatedPositionedDirectional)(object)this.widget).height, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        return ((Widget)(object?)Positioned.CreateDirectional(textDirection: Directionality.of(context), start: this._start?.evaluate(this.animation), top: this._top?.evaluate(this.animation), end: this._end?.evaluate(this.animation), bottom: this._bottom?.evaluate(this.animation), width: this._width?.evaluate(this.animation), height: this._height?.evaluate(this.animation), child: ((AnimatedPositionedDirectional)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>.CreateHas("start", this._start));
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>.CreateHas("top", this._top));
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>.CreateHas("end", this._end));
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>.CreateHas("bottom", this._bottom));
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>.CreateHas("width", this._width));
        description.add(global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>.CreateHas("height", this._height));
    }

}

public class AnimatedScale : ImplicitlyAnimatedWidget
{
    public virtual Widget? child { get; private set; }
    public virtual double scale { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Alignment alignment { get; private set; } = default!;
    public virtual FilterQuality? filterQuality { get; private set; }

    public AnimatedScale(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, double scale = default!, global::Doroti.Generated.Framework.Painting.Alignment alignment = default!, FilterQuality? filterQuality = null, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        global::Doroti.Generated.Framework.Painting.Alignment __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.child = child;
        this.scale = scale;
        this.alignment = __alignment;
        this.filterQuality = filterQuality;
    }

    public override IState createState() => new _AnimatedScaleState__implicit_animations();
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("scale", this.scale));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.Alignment>("alignment", this.alignment, defaultValue: global::Doroti.Generated.Framework.Painting.Alignment.center));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Flutter.Ui.FilterQuality>("filterQuality", this.filterQuality, defaultValue: null));
    }

}

internal class _AnimatedScaleState__implicit_animations : ImplicitlyAnimatedWidgetState<AnimatedScale>
{
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _scale { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _scaleAnimation { get; set; } = default!;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _scale = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._scale, ((AnimatedScale)(object)this.widget).scale, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
    }

    public override void didUpdateTweens()
    {
        _scaleAnimation = this.animation.drive(this._scale!);
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new ScaleTransition(scale: this._scaleAnimation, alignment: ((AnimatedScale)(object)this.widget).alignment, filterQuality: ((AnimatedScale)(object)this.widget).filterQuality, child: ((AnimatedScale)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AnimatedRotation : ImplicitlyAnimatedWidget
{
    public virtual Widget? child { get; private set; }
    public virtual double turns { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Alignment alignment { get; private set; } = default!;
    public virtual FilterQuality? filterQuality { get; private set; }

    public AnimatedRotation(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, double turns = default!, global::Doroti.Generated.Framework.Painting.Alignment alignment = default!, FilterQuality? filterQuality = null, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        global::Doroti.Generated.Framework.Painting.Alignment __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.child = child;
        this.turns = turns;
        this.alignment = __alignment;
        this.filterQuality = filterQuality;
    }

    public override IState createState() => new _AnimatedRotationState__implicit_animations();
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("turns", this.turns));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.Alignment>("alignment", this.alignment, defaultValue: global::Doroti.Generated.Framework.Painting.Alignment.center));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Flutter.Ui.FilterQuality>("filterQuality", this.filterQuality, defaultValue: null));
    }

}

internal class _AnimatedRotationState__implicit_animations : ImplicitlyAnimatedWidgetState<AnimatedRotation>
{
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _turns { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _turnsAnimation { get; set; } = default!;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _turns = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._turns, ((AnimatedRotation)(object)this.widget).turns, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
    }

    public override void didUpdateTweens()
    {
        _turnsAnimation = this.animation.drive(this._turns!);
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new RotationTransition(turns: this._turnsAnimation, alignment: ((AnimatedRotation)(object)this.widget).alignment, filterQuality: ((AnimatedRotation)(object)this.widget).filterQuality, child: ((AnimatedRotation)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AnimatedSlide : ImplicitlyAnimatedWidget
{
    public virtual Widget? child { get; private set; }
    public virtual Offset offset { get; private set; } = default!;

    public AnimatedSlide(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, Offset offset = default!, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.child = child;
        this.offset = offset;
    }

    public override IState createState() => new _AnimatedSlideState__implicit_animations();
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("offset", this.offset));
    }

}

internal class _AnimatedSlideState__implicit_animations : ImplicitlyAnimatedWidgetState<AnimatedSlide>
{
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<Offset>? _offset { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<Offset> _offsetAnimation { get; set; } = default!;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _offset = ((global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Flutter.Ui.Offset>?)(object?)visitor(this._offset, ((AnimatedSlide)(object)this.widget).offset, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Flutter.Ui.Offset>(begin: DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Offset>(value)))))!;
    }

    public override void didUpdateTweens()
    {
        _offsetAnimation = this.animation.drive(this._offset!);
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new SlideTransition(position: this._offsetAnimation, child: ((AnimatedSlide)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AnimatedOpacity : ImplicitlyAnimatedWidget
{
    public virtual Widget? child { get; private set; }
    public virtual double opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    public AnimatedOpacity(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, double opacity = default!, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null, bool alwaysIncludeSemantics = false) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.child = child;
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
        System.Diagnostics.Debug.Assert(((opacity >= 0.0) && (opacity <= 1.0)));
    }

    public override IState createState() => new _AnimatedOpacityState__implicit_animations();
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("opacity", this.opacity));
    }

}

internal class _AnimatedOpacityState__implicit_animations : ImplicitlyAnimatedWidgetState<AnimatedOpacity>
{
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _opacity { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _opacityAnimation { get; set; } = default!;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _opacity = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._opacity, ((AnimatedOpacity)(object)this.widget).opacity, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
    }

    public override void didUpdateTweens()
    {
        _opacityAnimation = this.animation.drive(this._opacity!);
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new FadeTransition(opacity: this._opacityAnimation, alwaysIncludeSemantics: ((AnimatedOpacity)(object)this.widget).alwaysIncludeSemantics, child: ((AnimatedOpacity)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverAnimatedOpacity : ImplicitlyAnimatedWidget
{
    public virtual Widget? sliver { get; private set; }
    public virtual double opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    public SliverAnimatedOpacity(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? sliver = null, double opacity = default!, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null, bool alwaysIncludeSemantics = false) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.sliver = sliver;
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
        System.Diagnostics.Debug.Assert(((opacity >= 0.0) && (opacity <= 1.0)));
    }

    public override IState createState() => new _SliverAnimatedOpacityState__implicit_animations();
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("opacity", this.opacity));
    }

}

internal class _SliverAnimatedOpacityState__implicit_animations : ImplicitlyAnimatedWidgetState<SliverAnimatedOpacity>
{
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _opacity { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _opacityAnimation { get; set; } = default!;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _opacity = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._opacity, ((SliverAnimatedOpacity)(object)this.widget).opacity, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
    }

    public override void didUpdateTweens()
    {
        _opacityAnimation = this.animation.drive(this._opacity!);
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new SliverFadeTransition(opacity: this._opacityAnimation, sliver: ((SliverAnimatedOpacity)(object)this.widget).sliver, alwaysIncludeSemantics: ((SliverAnimatedOpacity)(object)this.widget).alwaysIncludeSemantics));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AnimatedDefaultTextStyle : ImplicitlyAnimatedWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle style { get; private set; } = default!;
    public virtual TextAlign? textAlign { get; private set; }
    public virtual bool softWrap { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextOverflow overflow { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextWidthBasis textWidthBasis { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }

    public AnimatedDefaultTextStyle(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, global::Doroti.Generated.Framework.Painting.TextStyle style = default!, TextAlign? textAlign = null, bool softWrap = true, global::Doroti.Generated.Framework.Painting.TextOverflow overflow = global::Doroti.Generated.Framework.Painting.TextOverflow.clip, long? maxLines = null, global::Doroti.Generated.Framework.Painting.TextWidthBasis textWidthBasis = global::Doroti.Generated.Framework.Painting.TextWidthBasis.parent, TextHeightBehavior? textHeightBehavior = null, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.child = child;
        this.style = style;
        this.textAlign = textAlign;
        this.softWrap = softWrap;
        this.overflow = overflow;
        this.maxLines = maxLines;
        this.textWidthBasis = textWidthBasis;
        this.textHeightBehavior = textHeightBehavior;
        System.Diagnostics.Debug.Assert(((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
    }

    public override AnimatedWidgetBaseState<AnimatedDefaultTextStyle> createState() => DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedDefaultTextStyle>>(new _AnimatedDefaultTextStyleState__implicit_animations());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        this.style.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Flutter.Ui.TextAlign>("textAlign", this.textAlign, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("softWrap", value: this.softWrap, ifTrue: "wrapping at box width", ifFalse: "no wrapping except at line break characters", showName: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.TextOverflow>("overflow", this.overflow, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.IntProperty("maxLines", this.maxLines, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.TextWidthBasis>("textWidthBasis", this.textWidthBasis, defaultValue: global::Doroti.Generated.Framework.Painting.TextWidthBasis.parent));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Flutter.Ui.TextHeightBehavior>("textHeightBehavior", this.textHeightBehavior, defaultValue: null));
    }

}

internal class _AnimatedDefaultTextStyleState__implicit_animations : AnimatedWidgetBaseState<AnimatedDefaultTextStyle>
{
    internal virtual TextStyleTween? _style { get; set; } = default;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _style = ((TextStyleTween?)(object?)visitor(this._style, ((AnimatedDefaultTextStyle)(object)this.widget).style, ((value) => new TextStyleTween(begin: ((global::Doroti.Generated.Framework.Painting.TextStyle?)(object?)value)!))))!;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new DefaultTextStyle(style: this._style!.evaluate(this.animation), textAlign: ((AnimatedDefaultTextStyle)(object)this.widget).textAlign, softWrap: ((AnimatedDefaultTextStyle)(object)this.widget).softWrap, overflow: ((AnimatedDefaultTextStyle)(object)this.widget).overflow, maxLines: ((AnimatedDefaultTextStyle)(object)this.widget).maxLines, textWidthBasis: ((AnimatedDefaultTextStyle)(object)this.widget).textWidthBasis, textHeightBehavior: ((AnimatedDefaultTextStyle)(object)this.widget).textHeightBehavior, child: ((AnimatedDefaultTextStyle)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AnimatedPhysicalModel : ImplicitlyAnimatedWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BoxShape shape { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual double elevation { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual bool animateColor { get; private set; } = default!;
    public virtual Color shadowColor { get; private set; } = default!;
    public virtual bool animateShadowColor { get; private set; } = default!;

    public AnimatedPhysicalModel(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, global::Doroti.Generated.Framework.Painting.BoxShape shape = global::Doroti.Generated.Framework.Painting.BoxShape.rectangle, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, double elevation = 0.0, Color color = default!, bool animateColor = true, Color shadowColor = default!, bool animateShadowColor = true, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.child = child;
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.borderRadius = borderRadius;
        this.elevation = elevation;
        this.color = color;
        this.animateColor = animateColor;
        this.shadowColor = shadowColor;
        this.animateShadowColor = animateShadowColor;
        System.Diagnostics.Debug.Assert((elevation >= 0.0));
    }

    public override AnimatedWidgetBaseState<AnimatedPhysicalModel> createState() => DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedPhysicalModel>>(new _AnimatedPhysicalModelState__implicit_animations());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.BoxShape>("shape", this.shape));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.BorderRadius>("borderRadius", this.borderRadius));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("animateColor", this.animateColor));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("animateShadowColor", this.animateShadowColor));
    }

}

internal class _AnimatedPhysicalModelState__implicit_animations : AnimatedWidgetBaseState<AnimatedPhysicalModel>
{
    internal virtual BorderRadiusTween? _borderRadius { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _elevation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.ColorTween? _color { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.ColorTween? _shadowColor { get; set; } = default;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _borderRadius = ((BorderRadiusTween?)(object?)visitor(this._borderRadius, (((AnimatedPhysicalModel)(object)this.widget).borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.zero), ((value) => new BorderRadiusTween(begin: ((global::Doroti.Generated.Framework.Painting.BorderRadius?)(object?)value)!))))!;
        _elevation = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._elevation, ((AnimatedPhysicalModel)(object)this.widget).elevation, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        _color = ((global::Doroti.Generated.Framework.Animation.ColorTween?)(object?)visitor(this._color, ((AnimatedPhysicalModel)(object)this.widget).color, ((value) => new global::Doroti.Generated.Framework.Animation.ColorTween(begin: ((global::Doroti.Flutter.Ui.Color?)(object?)value)!))))!;
        _shadowColor = ((global::Doroti.Generated.Framework.Animation.ColorTween?)(object?)visitor(this._shadowColor, ((AnimatedPhysicalModel)(object)this.widget).shadowColor, ((value) => new global::Doroti.Generated.Framework.Animation.ColorTween(begin: ((global::Doroti.Flutter.Ui.Color?)(object?)value)!))))!;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new PhysicalModel(shape: ((AnimatedPhysicalModel)(object)this.widget).shape, clipBehavior: ((AnimatedPhysicalModel)(object)this.widget).clipBehavior, borderRadius: this._borderRadius!.evaluate(this.animation), elevation: this._elevation!.evaluate(this.animation), color: (((AnimatedPhysicalModel)(object)this.widget).animateColor ? this._color!.evaluate(this.animation)! : ((AnimatedPhysicalModel)(object)this.widget).color), shadowColor: (((AnimatedPhysicalModel)(object)this.widget).animateShadowColor ? this._shadowColor!.evaluate(this.animation)! : ((AnimatedPhysicalModel)(object)this.widget).shadowColor), child: ((AnimatedPhysicalModel)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AnimatedFractionallySizedBox : ImplicitlyAnimatedWidget
{
    public virtual Widget? child { get; private set; }
    public virtual double? heightFactor { get; private set; }
    public virtual double? widthFactor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;

    public AnimatedFractionallySizedBox(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, Widget? child = null, double? heightFactor = null, double? widthFactor = null, global::Doroti.Generated.Framework.Animation.Curve curve = default!, Duration duration = default!, global::System.Action? onEnd = null) : base(key: key, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, duration: duration, onEnd: onEnd)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.alignment = __alignment;
        this.child = child;
        this.heightFactor = heightFactor;
        this.widthFactor = widthFactor;
        System.Diagnostics.Debug.Assert(((widthFactor is null) || (widthFactor >= 0.0)));
        System.Diagnostics.Debug.Assert(((heightFactor is null) || (heightFactor >= 0.0)));
    }

    public override AnimatedWidgetBaseState<AnimatedFractionallySizedBox> createState() => DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedFractionallySizedBox>>(new _AnimatedFractionallySizedBoxState__implicit_animations());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<double>("widthFactor", this.widthFactor));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<double>("heightFactor", this.heightFactor));
    }

}

internal class _AnimatedFractionallySizedBoxState__implicit_animations : AnimatedWidgetBaseState<AnimatedFractionallySizedBox>
{
    internal virtual global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween? _alignment { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _heightFactorTween { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double>? _widthFactorTween { get; set; } = default;

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        _alignment = ((global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween?)(object?)visitor(this._alignment, ((AnimatedFractionallySizedBox)(object)this.widget).alignment, ((value) => new global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween(begin: ((global::Doroti.Generated.Framework.Painting.AlignmentGeometry?)(object?)value)!))))!;
        if ((((AnimatedFractionallySizedBox)(object)this.widget).heightFactor is not null))
        {
            _heightFactorTween = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._heightFactorTween, ((AnimatedFractionallySizedBox)(object)this.widget).heightFactor, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        }
        if ((((AnimatedFractionallySizedBox)(object)this.widget).widthFactor is not null))
        {
            _widthFactorTween = ((global::Doroti.Generated.Framework.Animation.Tween<double>?)(object?)visitor(this._widthFactorTween, ((AnimatedFractionallySizedBox)(object)this.widget).widthFactor, ((value) => new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((double)value)))))!;
        }
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new FractionallySizedBox(alignment: this._alignment!.evaluate(this.animation)!, heightFactor: this._heightFactorTween?.evaluate(this.animation), widthFactor: this._widthFactorTween?.evaluate(this.animation), child: ((AnimatedFractionallySizedBox)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.AlignmentGeometryTween>("alignment", this._alignment, defaultValue: null));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>("widthFactor", this._widthFactorTween, defaultValue: null));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Animation.Tween<double>>("heightFactor", this._heightFactorTween, defaultValue: null));
    }

}
