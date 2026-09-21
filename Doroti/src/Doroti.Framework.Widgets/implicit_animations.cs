// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/implicit_animations.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class BoxConstraintsTween : Tween<BoxConstraints>
{
    public BoxConstraintsTween(BoxConstraints? begin = null, BoxConstraints? end = null)
        : base(begin: begin, end: end) { }

    public override BoxConstraints lerp(double t) =>
        DartRuntimePrimitives.ConvertValue<BoxConstraints>(BoxConstraints.lerp(begin, end, t)!);
}

public class DecorationTween : Tween<Decoration>
{
    public DecorationTween(Decoration? begin = null, Decoration? end = null)
        : base(begin: begin, end: end) { }

    public override Decoration lerp(double t) =>
        DartRuntimePrimitives.ConvertValue<Decoration>(Decoration.lerp(begin, end, t)!);
}

public class EdgeInsetsTween : Tween<EdgeInsets>
{
    public EdgeInsetsTween(EdgeInsets? begin = null, EdgeInsets? end = null)
        : base(begin: begin, end: end) { }

    public override EdgeInsets lerp(double t) =>
        DartRuntimePrimitives.ConvertValue<EdgeInsets>(EdgeInsets.lerp(begin, end, t)!);
}

public class EdgeInsetsGeometryTween : Tween<EdgeInsetsGeometry>
{
    public EdgeInsetsGeometryTween(EdgeInsetsGeometry? begin = null, EdgeInsetsGeometry? end = null)
        : base(begin: begin, end: end) { }

    public override EdgeInsetsGeometry lerp(double t) =>
        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(
            EdgeInsetsGeometry.lerp(begin, end, t)!
        );
}

public class BorderRadiusTween : Tween<BorderRadius?>
{
    public BorderRadiusTween(BorderRadius? begin = null, BorderRadius? end = null)
        : base(begin: begin, end: end) { }

    public override BorderRadius? lerp(double t) => BorderRadius.lerp(begin, end, t);
}

public class BorderTween : Tween<Border?>
{
    public BorderTween(Border? begin = null, Border? end = null)
        : base(begin: begin, end: end) { }

    public override Border? lerp(double t) => Border.lerp(begin, end, t);
}

public class Matrix4Tween : Tween<Matrix4>
{
    public Matrix4Tween(Matrix4? begin = null, Matrix4? end = null)
        : base(begin: begin, end: end) { }

    public override Matrix4 lerp(double t)
    {
        DartRuntimePrimitives.Assert(() => begin is not null);
        DartRuntimePrimitives.Assert(() => end is not null);
        var beginTranslation = new Vector3();
        var endTranslation = new Vector3();
        var beginRotation = new Quaternion();
        var endRotation = new Quaternion();
        var beginScale = new Vector3();
        var endScale = new Vector3();
        begin!.decompose(beginTranslation, beginRotation, beginScale);
        end!.decompose(endTranslation, endRotation, endScale);
        Vector3 lerpTranslation = (beginTranslation * (1.0 - t)) + (endTranslation * t);
        Quaternion lerpRotation = (
            beginRotation.scaled(1.0 - t) + endRotation.scaled(t)
        ).normalized();
        Vector3 lerpScale = (beginScale * (1.0 - t)) + (endScale * t);
        return Matrix4.compose(lerpTranslation, lerpRotation, lerpScale);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class TextStyleTween : Tween<TextStyle>
{
    public TextStyleTween(TextStyle? begin = null, TextStyle? end = null)
        : base(begin: begin, end: end) { }

    public override TextStyle lerp(double t) =>
        DartRuntimePrimitives.ConvertValue<TextStyle>(TextStyle.lerp(begin, end, t)!);
}

public abstract class ImplicitlyAnimatedWidget : StatefulWidget
{
    public virtual Curve curve { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual Action? onEnd { get; private set; }

    protected ImplicitlyAnimatedWidget(
        Key? key = null,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
        : base(key: key)
    {
        Curve __curve = curve ?? Curves.linear;
        this.curve = __curve;
        this.duration = duration;
        this.onEnd = onEnd;
    }

    public abstract override IState createState();

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("duration", duration.inMilliseconds, unit: "ms"));
    }
}

public delegate Tween<T> TweenConstructor<T>(T targetValue);

public delegate Tween<T>? TweenVisitor<T>(
    Tween<T>? tween,
    T targetValue,
    Func<T, Tween<T>> constructor
);

public abstract class ImplicitlyAnimatedWidgetState<T> : State<T>, SingleTickerProviderStateMixin<T>
    where T : ImplicitlyAnimatedWidget
{
    private bool __late_controller_initialized;
    private AnimationController __late_controller = default!;
    public virtual AnimationController controller
    {
        get
        {
            if (!__late_controller_initialized)
            {
                __late_controller = new AnimationController(
                    duration: widget.duration,
                    debugLabel: Foundation.ConstantsLibrary.kDebugMode
                        ? ((Diagnosticable)widget).toStringShort()
                        : null,
                    vsync: this
                );
                __late_controller_initialized = true;
            }
            return __late_controller;
        }
    }
    private bool __late__animation_initialized;
    private CurvedAnimation __late__animation = default!;
    internal virtual CurvedAnimation _animation
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
        set
        {
            __late__animation = value;
            __late__animation_initialized = true;
        }
    }
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public virtual Animation<double> animation =>
        DartRuntimePrimitives.ConvertValue<Animation<double>>(_animation);

    public override void initState()
    {
        base.initState();
        controller.addStatusListener(
            (status) =>
            {
                if (AnimationStatusMembers.isCompleted(status))
                {
                    widget.onEnd?.Invoke();
                }
            }
        );
        _constructTweens();
        didUpdateTweens();
    }

    public override void didUpdateWidget(T oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.curve, oldWidget.curve))
        {
            _animation.dispose();
            _animation = _createCurve();
        }
        controller.duration = widget.duration;
        using var profile = FrameworkWorkCounters.Enabled
            ? FrameworkWorkProfile.Begin(GetType(), 1)
            : default;
        FrameworkWorkCounters.Add(FrameworkWork.ImplicitTweenCheck);
        if (_constructTweens())
        {
            forEachTween(
                (tween, targetValue, constructor) =>
                {
                    return (
                        (Func<IDartTween?>)(
                            () =>
                            {
                                var __cascade = tween;
                                if (__cascade is null)
                                {
                                    return null;
                                }

                                __cascade.begin = __cascade.evaluate(_animation);
                                __cascade.end = targetValue;
                                return __cascade;
                            }
                        )
                    )();
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
            FrameworkWorkCounters.Add(FrameworkWork.ImplicitAnimationRestart);
            if (FrameworkWorkCounters.Enabled)
            {
                FrameworkWorkProfile.Count(GetType(), 6);
            }

            controller.forward(from: 0.0);
            didUpdateTweens();
        }
    }

    internal virtual CurvedAnimation _createCurve()
    {
        return new CurvedAnimation(parent: controller, curve: widget.curve);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        _animation.dispose();
        controller.dispose();
        DartRuntimePrimitives.Assert(() =>
        {
            if ((_ticker is null) || !_ticker!.isActive)
            {
                return true;
            }
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary($"{this} was disposed with an active Ticker."),
                        new ErrorDescription(
                            $"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time "
                                + "dispose() was called on the mixin, that Ticker was still active. The Ticker must "
                                + "be disposed before calling super.dispose()."
                        ),
                        new ErrorHint(
                            "Tickers used by AnimationControllers "
                                + "should be disposed by calling dispose() on the AnimationController itself. "
                                + "Otherwise, the ticker will leak."
                        ),
                        _ticker!.describeForError("The offending ticker was"),
                    }
                )
            );
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual bool _constructTweens()
    {
        var shouldStartAnimation = false;
        forEachTween(
            (tween, targetValue, constructor) =>
            {
                if (targetValue is not null)
                {
                    tween ??= constructor(targetValue);
                    if (!Equals(targetValue, tween.end ?? tween.begin))
                    {
                        shouldStartAnimation = true;
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
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return shouldStartAnimation;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public abstract void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    );

    public virtual void didUpdateTweens() { }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_ticker is null)
            {
                return true;
            }
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."
                        ),
                        new ErrorDescription(
                            "A SingleTickerProviderStateMixin can only be used as a TickerProvider once."
                        ),
                        new ErrorHint(
                            "If a State is used for multiple AnimationController objects, or if it is passed to other "
                                + "objects and those objects might use it more than one time in total, then instead of "
                                + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin."
                        ),
                    }
                )
            );
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        _ticker = new Scheduler.Ticker(
            onTick,
            debugLabel: Foundation.ConstantsLibrary.kDebugMode
                ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                : null
        );
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch
        {
            (true, true) => "active but muted",
            (true, _) => "active",
            (false, true) => "inactive and muted",
            (false, _) => "inactive",
            (null, _) => DartRuntimePrimitives.ConvertValue<string>(null),
        };
        properties.add(
            new DiagnosticsProperty<Scheduler.Ticker>(
                "ticker",
                _ticker,
                description: tickerDescription,
                showSeparator: false,
                defaultValue: default
            )
        );
    }
}

public abstract class AnimatedWidgetBaseState<T> : ImplicitlyAnimatedWidgetState<T>
    where T : ImplicitlyAnimatedWidget
{
    public override void initState()
    {
        base.initState();
        controller.addListener(_handleAnimationChanged);
    }

    internal virtual void _handleAnimationChanged()
    {
        setState(() => { });
    }
}

public class AnimatedContainer : ImplicitlyAnimatedWidget
{
    public virtual Widget? child { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual Decoration? decoration { get; private set; }
    public virtual Decoration? foregroundDecoration { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual EdgeInsetsGeometry? margin { get; private set; }
    public virtual Matrix4? transform { get; private set; }
    public virtual AlignmentGeometry? transformAlignment { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public AnimatedContainer(
        Key? key = null,
        AlignmentGeometry? alignment = null,
        EdgeInsetsGeometry? padding = null,
        Color? color = null,
        Decoration? decoration = null,
        Decoration? foregroundDecoration = null,
        double? width = null,
        double? height = null,
        BoxConstraints? constraints = null,
        EdgeInsetsGeometry? margin = null,
        Matrix4? transform = null,
        AlignmentGeometry? transformAlignment = null,
        Widget? child = null,
        Clip clipBehavior = Clip.none,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.alignment = alignment;
        this.padding = padding;
        this.foregroundDecoration = foregroundDecoration;
        this.margin = margin;
        this.transform = transform;
        this.transformAlignment = transformAlignment;
        this.child = child;
        this.clipBehavior = clipBehavior;
        this.decoration =
            decoration ?? ((color is not null) ? new BoxDecoration(color: color) : null);
        this.constraints =
            ((width is not null) || (height is not null))
                ? (
                    constraints?.tighten(width: width, height: height)
                    ?? BoxConstraints.CreateTightFor(width: width, height: height)
                )
                : constraints;
        System.Diagnostics.Debug.Assert((margin is null) || margin.isNonNegative);
        System.Diagnostics.Debug.Assert((padding is null) || padding.isNonNegative);
        System.Diagnostics.Debug.Assert((decoration is null) || decoration.debugAssertIsValid());
        System.Diagnostics.Debug.Assert((constraints is null) || constraints.debugAssertIsValid());
        System.Diagnostics.Debug.Assert((color is null) || (decoration is null));
    }

    public override AnimatedWidgetBaseState<AnimatedContainer> createState() =>
        DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedContainer>>(
            new _AnimatedContainerState__implicit_animations()
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<AlignmentGeometry>(
                "alignment",
                alignment,
                showName: false,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding, defaultValue: null)
        );
        properties.add(new DiagnosticsProperty<Decoration>("bg", decoration, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<Decoration>("fg", foregroundDecoration, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<BoxConstraints>(
                "constraints",
                constraints,
                defaultValue: null,
                showName: false
            )
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>("margin", margin, defaultValue: null)
        );
        properties.add(ObjectFlagProperty<Matrix4>.CreateHas("transform", transform));
        properties.add(
            new DiagnosticsProperty<AlignmentGeometry>(
                "transformAlignment",
                transformAlignment,
                defaultValue: null
            )
        );
        properties.add(new DiagnosticsProperty<Clip>("clipBehavior", clipBehavior));
    }
}

internal class _AnimatedContainerState__implicit_animations
    : AnimatedWidgetBaseState<AnimatedContainer>
{
    internal virtual AlignmentGeometryTween? _alignment { get; set; } = default;
    internal virtual EdgeInsetsGeometryTween? _padding { get; set; } = default;
    internal virtual DecorationTween? _decoration { get; set; } = default;
    internal virtual DecorationTween? _foregroundDecoration { get; set; } = default;
    internal virtual BoxConstraintsTween? _constraints { get; set; } = default;
    internal virtual EdgeInsetsGeometryTween? _margin { get; set; } = default;
    internal virtual Matrix4Tween? _transform { get; set; } = default;
    internal virtual AlignmentGeometryTween? _transformAlignment { get; set; } = default;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _alignment = (
            (AlignmentGeometryTween?)visitor(
                _alignment,
                widget.alignment,
                (value) => new AlignmentGeometryTween(begin: ((AlignmentGeometry?)value)!)
            )
        )!;
        _padding = (
            (EdgeInsetsGeometryTween?)visitor(
                _padding,
                widget.padding,
                (value) => new EdgeInsetsGeometryTween(begin: ((EdgeInsetsGeometry?)value)!)
            )
        )!;
        _decoration = (
            (DecorationTween?)visitor(
                _decoration,
                widget.decoration,
                (value) => new DecorationTween(begin: ((Decoration?)value)!)
            )
        )!;
        _foregroundDecoration = (
            (DecorationTween?)visitor(
                _foregroundDecoration,
                widget.foregroundDecoration,
                (value) => new DecorationTween(begin: ((Decoration?)value)!)
            )
        )!;
        _constraints = (
            (BoxConstraintsTween?)visitor(
                _constraints,
                widget.constraints,
                (value) => new BoxConstraintsTween(begin: ((BoxConstraints?)value)!)
            )
        )!;
        _margin = (
            (EdgeInsetsGeometryTween?)visitor(
                _margin,
                widget.margin,
                (value) => new EdgeInsetsGeometryTween(begin: ((EdgeInsetsGeometry?)value)!)
            )
        )!;
        _transform = (
            (Matrix4Tween?)visitor(
                _transform,
                widget.transform,
                (value) => new Matrix4Tween(begin: ((Matrix4?)value)!)
            )
        )!;
        _transformAlignment = (
            (AlignmentGeometryTween?)visitor(
                _transformAlignment,
                widget.transformAlignment,
                (value) => new AlignmentGeometryTween(begin: ((AlignmentGeometry?)value)!)
            )
        )!;
    }

    public override Widget build(BuildContext context)
    {
        Animation<double> animationLocal = animation;
        return new Container(
            alignment: _alignment?.evaluate(animationLocal),
            padding: _padding?.evaluate(animationLocal),
            decoration: _decoration?.evaluate(animationLocal),
            foregroundDecoration: _foregroundDecoration?.evaluate(animationLocal),
            constraints: _constraints?.evaluate(animationLocal),
            margin: _margin?.evaluate(animationLocal),
            transform: _transform?.evaluate(animationLocal),
            transformAlignment: _transformAlignment?.evaluate(animationLocal),
            clipBehavior: widget.clipBehavior,
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(
            new DiagnosticsProperty<AlignmentGeometryTween>(
                "alignment",
                _alignment,
                showName: false,
                defaultValue: null
            )
        );
        description.add(
            new DiagnosticsProperty<EdgeInsetsGeometryTween>(
                "padding",
                _padding,
                defaultValue: null
            )
        );
        description.add(
            new DiagnosticsProperty<DecorationTween>("bg", _decoration, defaultValue: null)
        );
        description.add(
            new DiagnosticsProperty<DecorationTween>(
                "fg",
                _foregroundDecoration,
                defaultValue: null
            )
        );
        description.add(
            new DiagnosticsProperty<BoxConstraintsTween>(
                "constraints",
                _constraints,
                showName: false,
                defaultValue: null
            )
        );
        description.add(
            new DiagnosticsProperty<EdgeInsetsGeometryTween>("margin", _margin, defaultValue: null)
        );
        description.add(ObjectFlagProperty<Matrix4Tween>.CreateHas("transform", _transform));
        description.add(
            new DiagnosticsProperty<AlignmentGeometryTween>(
                "transformAlignment",
                _transformAlignment,
                defaultValue: null
            )
        );
    }
}

public class AnimatedPadding : ImplicitlyAnimatedWidget
{
    public virtual EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public AnimatedPadding(
        Key? key = null,
        EdgeInsetsGeometry padding = default!,
        Widget? child = null,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.padding = padding;
        this.child = child;
        System.Diagnostics.Debug.Assert(padding.isNonNegative);
    }

    public override AnimatedWidgetBaseState<AnimatedPadding> createState() =>
        DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedPadding>>(
            new _AnimatedPaddingState__implicit_animations()
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding));
    }
}

internal class _AnimatedPaddingState__implicit_animations : AnimatedWidgetBaseState<AnimatedPadding>
{
    internal virtual EdgeInsetsGeometryTween? _padding { get; set; } = default;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _padding = (
            (EdgeInsetsGeometryTween?)visitor(
                _padding,
                widget.padding,
                (value) => new EdgeInsetsGeometryTween(begin: ((EdgeInsetsGeometry?)value)!)
            )
        )!;
    }

    public override Widget build(BuildContext context)
    {
        return new Padding(
            padding: _padding!
                .evaluate(animation)
                .clamp(EdgeInsets.zero, EdgeInsetsGeometry.infinity),
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(
            new DiagnosticsProperty<EdgeInsetsGeometryTween>(
                "padding",
                _padding,
                defaultValue: null
            )
        );
    }
}

public class AnimatedAlign : ImplicitlyAnimatedWidget
{
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Widget? child { get; private set; }
    public virtual double? heightFactor { get; private set; }
    public virtual double? widthFactor { get; private set; }

    public AnimatedAlign(
        Key? key = null,
        AlignmentGeometry alignment = default!,
        Widget? child = null,
        double? heightFactor = null,
        double? widthFactor = null,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.alignment = alignment;
        this.child = child;
        this.heightFactor = heightFactor;
        this.widthFactor = widthFactor;
        System.Diagnostics.Debug.Assert((widthFactor is null) || (widthFactor >= 0.0));
        System.Diagnostics.Debug.Assert((heightFactor is null) || (heightFactor >= 0.0));
    }

    public override AnimatedWidgetBaseState<AnimatedAlign> createState() =>
        DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedAlign>>(
            new _AnimatedAlignState__implicit_animations()
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
    }
}

internal class _AnimatedAlignState__implicit_animations : AnimatedWidgetBaseState<AnimatedAlign>
{
    internal virtual AlignmentGeometryTween? _alignment { get; set; } = default;
    internal virtual Tween<double>? _heightFactorTween { get; set; } = default;
    internal virtual Tween<double>? _widthFactorTween { get; set; } = default;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _alignment = (
            (AlignmentGeometryTween?)visitor(
                _alignment,
                widget.alignment,
                (value) => new AlignmentGeometryTween(begin: ((AlignmentGeometry?)value)!)
            )
        )!;
        if (widget.heightFactor is not null)
        {
            _heightFactorTween = (
                (Tween<double>?)visitor(
                    _heightFactorTween,
                    widget.heightFactor,
                    (value) => new Tween<double>(begin: (double)value)
                )
            )!;
        }
        if (widget.widthFactor is not null)
        {
            _widthFactorTween = (
                (Tween<double>?)visitor(
                    _widthFactorTween,
                    widget.widthFactor,
                    (value) => new Tween<double>(begin: (double)value)
                )
            )!;
        }
    }

    public override Widget build(BuildContext context)
    {
        return new Align(
            alignment: _alignment!.evaluate(animation)!,
            heightFactor: _heightFactorTween?.evaluate(animation),
            widthFactor: _widthFactorTween?.evaluate(animation),
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(
            new DiagnosticsProperty<AlignmentGeometryTween>(
                "alignment",
                _alignment,
                defaultValue: null
            )
        );
        description.add(
            new DiagnosticsProperty<Tween<double>>(
                "widthFactor",
                _widthFactorTween,
                defaultValue: null
            )
        );
        description.add(
            new DiagnosticsProperty<Tween<double>>(
                "heightFactor",
                _heightFactorTween,
                defaultValue: null
            )
        );
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

    public AnimatedPositioned(
        Key? key = null,
        Widget child = default!,
        double? left = null,
        double? top = null,
        double? right = null,
        double? bottom = null,
        double? width = null,
        double? height = null,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.child = child;
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
        this.width = width;
        this.height = height;
        System.Diagnostics.Debug.Assert((left is null) || (right is null) || (width is null));
        System.Diagnostics.Debug.Assert((top is null) || (bottom is null) || (height is null));
    }

    public static AnimatedPositioned CreateFromRect(
        Key? key = null,
        Widget child = default!,
        Rect rect = default!,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
    {
        var __instance = new AnimatedPositioned(
            key,
            child,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            curve,
            duration,
            onEnd
        );
        Curve __curve = curve ?? Curves.linear;
        __instance.child = child;
        __instance.left = rect.left;
        __instance.top = rect.top;
        __instance.width = rect.width;
        __instance.height = rect.height;
        __instance.right = null;
        __instance.bottom = null;
        return __instance;
    }

    public override AnimatedWidgetBaseState<AnimatedPositioned> createState() =>
        DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedPositioned>>(
            new _AnimatedPositionedState__implicit_animations()
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("left", left, defaultValue: null));
        properties.add(new DoubleProperty("top", top, defaultValue: null));
        properties.add(new DoubleProperty("right", right, defaultValue: null));
        properties.add(new DoubleProperty("bottom", bottom, defaultValue: null));
        properties.add(new DoubleProperty("width", width, defaultValue: null));
        properties.add(new DoubleProperty("height", height, defaultValue: null));
    }
}

internal class _AnimatedPositionedState__implicit_animations
    : AnimatedWidgetBaseState<AnimatedPositioned>
{
    internal virtual Tween<double>? _left { get; set; } = default;
    internal virtual Tween<double>? _top { get; set; } = default;
    internal virtual Tween<double>? _right { get; set; } = default;
    internal virtual Tween<double>? _bottom { get; set; } = default;
    internal virtual Tween<double>? _width { get; set; } = default;
    internal virtual Tween<double>? _height { get; set; } = default;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _left = (
            (Tween<double>?)visitor(
                _left,
                widget.left,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _top = (
            (Tween<double>?)visitor(
                _top,
                widget.top,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _right = (
            (Tween<double>?)visitor(
                _right,
                widget.right,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _bottom = (
            (Tween<double>?)visitor(
                _bottom,
                widget.bottom,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _width = (
            (Tween<double>?)visitor(
                _width,
                widget.width,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _height = (
            (Tween<double>?)visitor(
                _height,
                widget.height,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
    }

    public override Widget build(BuildContext context)
    {
        return new Positioned(
            left: _left?.evaluate(animation),
            top: _top?.evaluate(animation),
            right: _right?.evaluate(animation),
            bottom: _bottom?.evaluate(animation),
            width: _width?.evaluate(animation),
            height: _height?.evaluate(animation),
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(ObjectFlagProperty<Tween<double>>.CreateHas("left", _left));
        description.add(ObjectFlagProperty<Tween<double>>.CreateHas("top", _top));
        description.add(ObjectFlagProperty<Tween<double>>.CreateHas("right", _right));
        description.add(ObjectFlagProperty<Tween<double>>.CreateHas("bottom", _bottom));
        description.add(ObjectFlagProperty<Tween<double>>.CreateHas("width", _width));
        description.add(ObjectFlagProperty<Tween<double>>.CreateHas("height", _height));
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

    public AnimatedPositionedDirectional(
        Key? key = null,
        Widget child = default!,
        double? start = null,
        double? top = null,
        double? end = null,
        double? bottom = null,
        double? width = null,
        double? height = null,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.child = child;
        this.start = start;
        this.top = top;
        this.end = end;
        this.bottom = bottom;
        this.width = width;
        this.height = height;
        System.Diagnostics.Debug.Assert((start is null) || (end is null) || (width is null));
        System.Diagnostics.Debug.Assert((top is null) || (bottom is null) || (height is null));
    }

    public override AnimatedWidgetBaseState<AnimatedPositionedDirectional> createState() =>
        DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedPositionedDirectional>>(
            new _AnimatedPositionedDirectionalState__implicit_animations()
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("start", start, defaultValue: null));
        properties.add(new DoubleProperty("top", top, defaultValue: null));
        properties.add(new DoubleProperty("end", end, defaultValue: null));
        properties.add(new DoubleProperty("bottom", bottom, defaultValue: null));
        properties.add(new DoubleProperty("width", width, defaultValue: null));
        properties.add(new DoubleProperty("height", height, defaultValue: null));
    }
}

internal class _AnimatedPositionedDirectionalState__implicit_animations
    : AnimatedWidgetBaseState<AnimatedPositionedDirectional>
{
    internal virtual Tween<double>? _start { get; set; } = default;
    internal virtual Tween<double>? _top { get; set; } = default;
    internal virtual Tween<double>? _end { get; set; } = default;
    internal virtual Tween<double>? _bottom { get; set; } = default;
    internal virtual Tween<double>? _width { get; set; } = default;
    internal virtual Tween<double>? _height { get; set; } = default;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _start = (
            (Tween<double>?)visitor(
                _start,
                widget.start,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _top = (
            (Tween<double>?)visitor(
                _top,
                widget.top,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _end = (
            (Tween<double>?)visitor(
                _end,
                widget.end,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _bottom = (
            (Tween<double>?)visitor(
                _bottom,
                widget.bottom,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _width = (
            (Tween<double>?)visitor(
                _width,
                widget.width,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _height = (
            (Tween<double>?)visitor(
                _height,
                widget.height,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasDirectionality(context));
        return Positioned.CreateDirectional(
            textDirection: Directionality.of(context),
            start: _start?.evaluate(animation),
            top: _top?.evaluate(animation),
            end: _end?.evaluate(animation),
            bottom: _bottom?.evaluate(animation),
            width: _width?.evaluate(animation),
            height: _height?.evaluate(animation),
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(ObjectFlagProperty<Tween<double>>.CreateHas("start", _start));
        description.add(ObjectFlagProperty<Tween<double>>.CreateHas("top", _top));
        description.add(ObjectFlagProperty<Tween<double>>.CreateHas("end", _end));
        description.add(ObjectFlagProperty<Tween<double>>.CreateHas("bottom", _bottom));
        description.add(ObjectFlagProperty<Tween<double>>.CreateHas("width", _width));
        description.add(ObjectFlagProperty<Tween<double>>.CreateHas("height", _height));
    }
}

public class AnimatedScale : ImplicitlyAnimatedWidget
{
    public virtual Widget? child { get; private set; }
    public virtual double scale { get; private set; } = default!;
    public virtual Alignment alignment { get; private set; } = default!;
    public virtual FilterQuality? filterQuality { get; private set; }

    public AnimatedScale(
        Key? key = null,
        Widget? child = null,
        double scale = default!,
        Alignment alignment = default!,
        FilterQuality? filterQuality = null,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
    {
        Alignment __alignment = alignment ?? Alignment.center;
        this.child = child;
        this.scale = scale;
        this.alignment = __alignment;
        this.filterQuality = filterQuality;
    }

    public override IState createState() => new _AnimatedScaleState__implicit_animations();

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("scale", scale));
        properties.add(
            new DiagnosticsProperty<Alignment>(
                "alignment",
                alignment,
                defaultValue: Alignment.center
            )
        );
        properties.add(
            new EnumProperty<FilterQuality>("filterQuality", filterQuality, defaultValue: null)
        );
    }
}

internal class _AnimatedScaleState__implicit_animations
    : ImplicitlyAnimatedWidgetState<AnimatedScale>
{
    internal virtual Tween<double>? _scale { get; set; } = default;
    internal virtual Animation<double> _scaleAnimation { get; set; } = default!;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _scale = (
            (Tween<double>?)visitor(
                _scale,
                widget.scale,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
    }

    public override void didUpdateTweens()
    {
        _scaleAnimation = animation.drive(_scale!);
    }

    public override Widget build(BuildContext context)
    {
        return new ScaleTransition(
            scale: _scaleAnimation,
            alignment: widget.alignment,
            filterQuality: widget.filterQuality,
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class AnimatedRotation : ImplicitlyAnimatedWidget
{
    public virtual Widget? child { get; private set; }
    public virtual double turns { get; private set; } = default!;
    public virtual Alignment alignment { get; private set; } = default!;
    public virtual FilterQuality? filterQuality { get; private set; }

    public AnimatedRotation(
        Key? key = null,
        Widget? child = null,
        double turns = default!,
        Alignment alignment = default!,
        FilterQuality? filterQuality = null,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
    {
        Alignment __alignment = alignment ?? Alignment.center;
        this.child = child;
        this.turns = turns;
        this.alignment = __alignment;
        this.filterQuality = filterQuality;
    }

    public override IState createState() => new _AnimatedRotationState__implicit_animations();

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("turns", turns));
        properties.add(
            new DiagnosticsProperty<Alignment>(
                "alignment",
                alignment,
                defaultValue: Alignment.center
            )
        );
        properties.add(
            new EnumProperty<FilterQuality>("filterQuality", filterQuality, defaultValue: null)
        );
    }
}

internal class _AnimatedRotationState__implicit_animations
    : ImplicitlyAnimatedWidgetState<AnimatedRotation>
{
    internal virtual Tween<double>? _turns { get; set; } = default;
    internal virtual Animation<double> _turnsAnimation { get; set; } = default!;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _turns = (
            (Tween<double>?)visitor(
                _turns,
                widget.turns,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
    }

    public override void didUpdateTweens()
    {
        _turnsAnimation = animation.drive(_turns!);
    }

    public override Widget build(BuildContext context)
    {
        return new RotationTransition(
            turns: _turnsAnimation,
            alignment: widget.alignment,
            filterQuality: widget.filterQuality,
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class AnimatedSlide : ImplicitlyAnimatedWidget
{
    public virtual Widget? child { get; private set; }
    public virtual Offset offset { get; private set; } = default!;

    public AnimatedSlide(
        Key? key = null,
        Widget? child = null,
        Offset offset = default!,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.child = child;
        this.offset = offset;
    }

    public override IState createState() => new _AnimatedSlideState__implicit_animations();

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("offset", offset));
    }
}

internal class _AnimatedSlideState__implicit_animations
    : ImplicitlyAnimatedWidgetState<AnimatedSlide>
{
    internal virtual Tween<Offset>? _offset { get; set; } = default;
    internal virtual Animation<Offset> _offsetAnimation { get; set; } = default!;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _offset = (
            (Tween<Offset>?)visitor(
                _offset,
                widget.offset,
                (value) =>
                    new Tween<Offset>(begin: DartRuntimePrimitives.ConvertValue<Offset>(value))
            )
        )!;
    }

    public override void didUpdateTweens()
    {
        _offsetAnimation = animation.drive(_offset!);
    }

    public override Widget build(BuildContext context)
    {
        return new SlideTransition(position: _offsetAnimation, child: widget.child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class AnimatedOpacity : ImplicitlyAnimatedWidget
{
    public virtual Widget? child { get; private set; }
    public virtual double opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    public AnimatedOpacity(
        Key? key = null,
        Widget? child = null,
        double opacity = default!,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null,
        bool alwaysIncludeSemantics = false
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.child = child;
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
        System.Diagnostics.Debug.Assert((opacity >= 0.0) && (opacity <= 1.0));
    }

    public override IState createState() => new _AnimatedOpacityState__implicit_animations();

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("opacity", opacity));
    }
}

internal class _AnimatedOpacityState__implicit_animations
    : ImplicitlyAnimatedWidgetState<AnimatedOpacity>
{
    internal virtual Tween<double>? _opacity { get; set; } = default;
    internal virtual Animation<double> _opacityAnimation { get; set; } = default!;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _opacity = (
            (Tween<double>?)visitor(
                _opacity,
                widget.opacity,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
    }

    public override void didUpdateTweens()
    {
        _opacityAnimation = animation.drive(_opacity!);
    }

    public override Widget build(BuildContext context)
    {
        return new FadeTransition(
            opacity: _opacityAnimation,
            alwaysIncludeSemantics: widget.alwaysIncludeSemantics,
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class SliverAnimatedOpacity : ImplicitlyAnimatedWidget
{
    public virtual Widget? sliver { get; private set; }
    public virtual double opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    public SliverAnimatedOpacity(
        Key? key = null,
        Widget? sliver = null,
        double opacity = default!,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null,
        bool alwaysIncludeSemantics = false
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.sliver = sliver;
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
        System.Diagnostics.Debug.Assert((opacity >= 0.0) && (opacity <= 1.0));
    }

    public override IState createState() => new _SliverAnimatedOpacityState__implicit_animations();

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("opacity", opacity));
    }
}

internal class _SliverAnimatedOpacityState__implicit_animations
    : ImplicitlyAnimatedWidgetState<SliverAnimatedOpacity>
{
    internal virtual Tween<double>? _opacity { get; set; } = default;
    internal virtual Animation<double> _opacityAnimation { get; set; } = default!;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _opacity = (
            (Tween<double>?)visitor(
                _opacity,
                widget.opacity,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
    }

    public override void didUpdateTweens()
    {
        _opacityAnimation = animation.drive(_opacity!);
    }

    public override Widget build(BuildContext context)
    {
        return new SliverFadeTransition(
            opacity: _opacityAnimation,
            sliver: widget.sliver,
            alwaysIncludeSemantics: widget.alwaysIncludeSemantics
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class AnimatedDefaultTextStyle : ImplicitlyAnimatedWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual TextStyle style { get; private set; } = default!;
    public virtual TextAlign? textAlign { get; private set; }
    public virtual bool softWrap { get; private set; } = default!;
    public virtual TextOverflow overflow { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual TextWidthBasis textWidthBasis { get; private set; } = default!;
    public virtual TextHeightBehavior? textHeightBehavior { get; private set; }

    public AnimatedDefaultTextStyle(
        Key? key = null,
        Widget child = default!,
        TextStyle style = default!,
        TextAlign? textAlign = null,
        bool softWrap = true,
        TextOverflow overflow = TextOverflow.clip,
        long? maxLines = null,
        TextWidthBasis textWidthBasis = TextWidthBasis.parent,
        TextHeightBehavior? textHeightBehavior = null,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
    {
        this.child = child;
        this.style = style;
        this.textAlign = textAlign;
        this.softWrap = softWrap;
        this.overflow = overflow;
        this.maxLines = maxLines;
        this.textWidthBasis = textWidthBasis;
        this.textHeightBehavior = textHeightBehavior;
        System.Diagnostics.Debug.Assert(
            (maxLines is null)
                || (
                    (
                        maxLines
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
        );
    }

    public override AnimatedWidgetBaseState<AnimatedDefaultTextStyle> createState() =>
        DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedDefaultTextStyle>>(
            new _AnimatedDefaultTextStyleState__implicit_animations()
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        style.debugFillProperties(properties);
        properties.add(new EnumProperty<TextAlign>("textAlign", textAlign, defaultValue: null));
        properties.add(
            new FlagProperty(
                "softWrap",
                value: softWrap,
                ifTrue: "wrapping at box width",
                ifFalse: "no wrapping except at line break characters",
                showName: true
            )
        );
        properties.add(new EnumProperty<TextOverflow>("overflow", overflow, defaultValue: null));
        properties.add(new IntProperty("maxLines", maxLines, defaultValue: null));
        properties.add(
            new EnumProperty<TextWidthBasis>(
                "textWidthBasis",
                textWidthBasis,
                defaultValue: TextWidthBasis.parent
            )
        );
        properties.add(
            new DiagnosticsProperty<TextHeightBehavior>(
                "textHeightBehavior",
                textHeightBehavior,
                defaultValue: null
            )
        );
    }
}

internal class _AnimatedDefaultTextStyleState__implicit_animations
    : AnimatedWidgetBaseState<AnimatedDefaultTextStyle>
{
    internal virtual TextStyleTween? _style { get; set; } = default;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _style = (
            (TextStyleTween?)visitor(
                _style,
                widget.style,
                (value) => new TextStyleTween(begin: ((TextStyle?)value)!)
            )
        )!;
    }

    public override Widget build(BuildContext context)
    {
        return new DefaultTextStyle(
            style: _style!.evaluate(animation),
            textAlign: widget.textAlign,
            softWrap: widget.softWrap,
            overflow: widget.overflow,
            maxLines: widget.maxLines,
            textWidthBasis: widget.textWidthBasis,
            textHeightBehavior: widget.textHeightBehavior,
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class AnimatedPhysicalModel : ImplicitlyAnimatedWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual BoxShape shape { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual BorderRadius? borderRadius { get; private set; }
    public virtual double elevation { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual bool animateColor { get; private set; } = default!;
    public virtual Color shadowColor { get; private set; } = default!;
    public virtual bool animateShadowColor { get; private set; } = default!;

    public AnimatedPhysicalModel(
        Key? key = null,
        Widget child = default!,
        BoxShape shape = BoxShape.rectangle,
        Clip clipBehavior = Clip.none,
        BorderRadius? borderRadius = null,
        double elevation = 0.0,
        Color color = default!,
        bool animateColor = true,
        Color shadowColor = default!,
        bool animateShadowColor = true,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
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
        System.Diagnostics.Debug.Assert(elevation >= 0.0);
    }

    public override AnimatedWidgetBaseState<AnimatedPhysicalModel> createState() =>
        DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedPhysicalModel>>(
            new _AnimatedPhysicalModelState__implicit_animations()
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<BoxShape>("shape", shape));
        properties.add(new DiagnosticsProperty<BorderRadius>("borderRadius", borderRadius));
        properties.add(new DoubleProperty("elevation", elevation));
        properties.add(new ColorProperty("color", color));
        properties.add(new DiagnosticsProperty<bool>("animateColor", animateColor));
        properties.add(new ColorProperty("shadowColor", shadowColor));
        properties.add(new DiagnosticsProperty<bool>("animateShadowColor", animateShadowColor));
    }
}

internal class _AnimatedPhysicalModelState__implicit_animations
    : AnimatedWidgetBaseState<AnimatedPhysicalModel>
{
    internal virtual BorderRadiusTween? _borderRadius { get; set; } = default;
    internal virtual Tween<double>? _elevation { get; set; } = default;
    internal virtual ColorTween? _color { get; set; } = default;
    internal virtual ColorTween? _shadowColor { get; set; } = default;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _borderRadius = (
            (BorderRadiusTween?)visitor(
                _borderRadius,
                widget.borderRadius ?? BorderRadius.zero,
                (value) => new BorderRadiusTween(begin: ((BorderRadius?)value)!)
            )
        )!;
        _elevation = (
            (Tween<double>?)visitor(
                _elevation,
                widget.elevation,
                (value) => new Tween<double>(begin: (double)value)
            )
        )!;
        _color = (
            (ColorTween?)visitor(
                _color,
                widget.color,
                (value) => new ColorTween(begin: ((Color?)value)!)
            )
        )!;
        _shadowColor = (
            (ColorTween?)visitor(
                _shadowColor,
                widget.shadowColor,
                (value) => new ColorTween(begin: ((Color?)value)!)
            )
        )!;
    }

    public override Widget build(BuildContext context)
    {
        return new PhysicalModel(
            shape: widget.shape,
            clipBehavior: widget.clipBehavior,
            borderRadius: _borderRadius!.evaluate(animation),
            elevation: _elevation!.evaluate(animation),
            color: widget.animateColor ? _color!.evaluate(animation)! : widget.color,
            shadowColor: widget.animateShadowColor
                ? _shadowColor!.evaluate(animation)!
                : widget.shadowColor,
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class AnimatedFractionallySizedBox : ImplicitlyAnimatedWidget
{
    public virtual Widget? child { get; private set; }
    public virtual double? heightFactor { get; private set; }
    public virtual double? widthFactor { get; private set; }
    public virtual AlignmentGeometry alignment { get; private set; } = default!;

    public AnimatedFractionallySizedBox(
        Key? key = null,
        AlignmentGeometry alignment = default!,
        Widget? child = null,
        double? heightFactor = null,
        double? widthFactor = null,
        Curve curve = default!,
        Duration duration = default!,
        Action? onEnd = null
    )
        : base(key: key, curve: curve ?? Curves.linear, duration: duration, onEnd: onEnd)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.alignment = __alignment;
        this.child = child;
        this.heightFactor = heightFactor;
        this.widthFactor = widthFactor;
        System.Diagnostics.Debug.Assert((widthFactor is null) || (widthFactor >= 0.0));
        System.Diagnostics.Debug.Assert((heightFactor is null) || (heightFactor >= 0.0));
    }

    public override AnimatedWidgetBaseState<AnimatedFractionallySizedBox> createState() =>
        DartRuntimePrimitives.ConvertValue<AnimatedWidgetBaseState<AnimatedFractionallySizedBox>>(
            new _AnimatedFractionallySizedBoxState__implicit_animations()
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment));
        properties.add(new DiagnosticsProperty<double>("widthFactor", widthFactor));
        properties.add(new DiagnosticsProperty<double>("heightFactor", heightFactor));
    }
}

internal class _AnimatedFractionallySizedBoxState__implicit_animations
    : AnimatedWidgetBaseState<AnimatedFractionallySizedBox>
{
    internal virtual AlignmentGeometryTween? _alignment { get; set; } = default;
    internal virtual Tween<double>? _heightFactorTween { get; set; } = default;
    internal virtual Tween<double>? _widthFactorTween { get; set; } = default;

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        _alignment = (
            (AlignmentGeometryTween?)visitor(
                _alignment,
                widget.alignment,
                (value) => new AlignmentGeometryTween(begin: ((AlignmentGeometry?)value)!)
            )
        )!;
        if (widget.heightFactor is not null)
        {
            _heightFactorTween = (
                (Tween<double>?)visitor(
                    _heightFactorTween,
                    widget.heightFactor,
                    (value) => new Tween<double>(begin: (double)value)
                )
            )!;
        }
        if (widget.widthFactor is not null)
        {
            _widthFactorTween = (
                (Tween<double>?)visitor(
                    _widthFactorTween,
                    widget.widthFactor,
                    (value) => new Tween<double>(begin: (double)value)
                )
            )!;
        }
    }

    public override Widget build(BuildContext context)
    {
        return new FractionallySizedBox(
            alignment: _alignment!.evaluate(animation)!,
            heightFactor: _heightFactorTween?.evaluate(animation),
            widthFactor: _widthFactorTween?.evaluate(animation),
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(
            new DiagnosticsProperty<AlignmentGeometryTween>(
                "alignment",
                _alignment,
                defaultValue: null
            )
        );
        description.add(
            new DiagnosticsProperty<Tween<double>>(
                "widthFactor",
                _widthFactorTween,
                defaultValue: null
            )
        );
        description.add(
            new DiagnosticsProperty<Tween<double>>(
                "heightFactor",
                _heightFactorTween,
                defaultValue: null
            )
        );
    }
}
