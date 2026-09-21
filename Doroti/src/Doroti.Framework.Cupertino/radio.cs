// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/radio.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class RadioLibrary
{
    internal static Size _size = new Size(18.0, 18.0);
}

public static partial class RadioLibrary
{
    internal static double _kOuterRadius = 7.0;
}

public static partial class RadioLibrary
{
    internal static double _kInnerRadius = 2.975;
}

public static partial class RadioLibrary
{
    internal static Color _kDisabledOuterColor = CupertinoColors.white.withOpacity(0.5);
}

public static partial class RadioLibrary
{
    internal static Color _kDisabledInnerColor = new CupertinoDynamicColor(
        color: Color.fromARGB(64L, 0L, 0L, 0L),
        darkColor: Color.fromARGB(64L, 255L, 255L, 255L)
    );
}

public static partial class RadioLibrary
{
    internal static Color _kDisabledBorderColor = new CupertinoDynamicColor(
        color: Color.fromARGB(64L, 0L, 0L, 0L),
        darkColor: Color.fromARGB(64L, 0L, 0L, 0L)
    );
}

public static partial class RadioLibrary
{
    internal static CupertinoDynamicColor _kDefaultBorderColor = new CupertinoDynamicColor(
        color: Color.fromARGB(255L, 209L, 209L, 214L),
        darkColor: Color.fromARGB(64L, 0L, 0L, 0L)
    );
}

public static partial class RadioLibrary
{
    internal static CupertinoDynamicColor _kDefaultInnerColor = new CupertinoDynamicColor(
        color: CupertinoColors.white,
        darkColor: Color.fromARGB(255L, 222L, 232L, 248L)
    );
}

public static partial class RadioLibrary
{
    internal static CupertinoDynamicColor _kDefaultOuterColor = new CupertinoDynamicColor(
        color: CupertinoColors.activeBlue,
        darkColor: Color.fromARGB(255L, 50L, 100L, 215L)
    );
}

public static partial class RadioLibrary
{
    internal static double _kPressedOverlayOpacity = 0.15;
}

public static partial class RadioLibrary
{
    internal static double _kCheckmarkStrokeWidth = 2.0;
}

public static partial class RadioLibrary
{
    internal static double _kFocusOutlineStrokeWidth = 3.0;
}

public static partial class RadioLibrary
{
    internal static double _kBorderOutlineStrokeWidth = 0.3;
}

public static partial class RadioLibrary
{
    internal static List<double> _kDarkGradientOpacities = new List<double> { 0.14, 0.29 };
}

public static partial class RadioLibrary
{
    internal static List<double> _kDisabledDarkGradientOpacities = new List<double> { 0.08, 0.14 };
}

public class CupertinoRadio<T> : StatefulWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual T? groupValue { get; private set; }
    public virtual Action<T?>? onChanged { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual bool toggleable { get; private set; } = default!;
    public virtual bool useCheckmarkStyle { get; private set; } = default!;
    public virtual Color? activeColor { get; private set; }
    public virtual Color? inactiveColor { get; private set; }
    public virtual Color? fillColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual RadioGroupRegistry<T>? groupRegistry { get; private set; }
    public virtual bool? enabled { get; private set; }

    public CupertinoRadio(
        Key? key = null,
        T value = default!,
        T? groupValue = default,
        Action<T?>? onChanged = null,
        MouseCursor? mouseCursor = null,
        bool toggleable = false,
        Color? activeColor = null,
        Color? inactiveColor = null,
        Color? fillColor = null,
        Color? focusColor = null,
        FocusNode? focusNode = null,
        bool autofocus = false,
        bool useCheckmarkStyle = false,
        bool? enabled = null,
        RadioGroupRegistry<T>? groupRegistry = null
    )
        : base(key: key)
    {
        this.value = value;
        this.groupValue = groupValue;
        this.onChanged = onChanged;
        this.mouseCursor = mouseCursor;
        this.toggleable = toggleable;
        this.activeColor = activeColor;
        this.inactiveColor = inactiveColor;
        this.fillColor = fillColor;
        this.focusColor = focusColor;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.useCheckmarkStyle = useCheckmarkStyle;
        this.enabled = enabled;
        this.groupRegistry = groupRegistry;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoRadioState__radio<T>());
}

internal class _CupertinoRadioState__radio<T> : State<CupertinoRadio<T>>
{
    internal virtual FocusNode? _internalFocusNode { get; set; } = default;
    internal virtual _RadioRegistry__radio<T>? _internalRadioRegistry { get; set; } = default;

    internal virtual FocusNode _effectiveFocusNode =>
        DartRuntimePrimitives.ConvertValue<FocusNode>(
            widget.focusNode ?? (_internalFocusNode ??= new FocusNode())
        );
    internal virtual bool _enabled =>
        DartRuntimePrimitives.ConvertValue<bool>(
            widget.enabled
                ?? (
                    (widget.onChanged is not null)
                    || (widget.groupRegistry is not null)
                    || (RadioGroup.maybeOf<T>(context) is not null)
                )
        );
    internal virtual RadioGroupRegistry<T> _effectiveRegistry
    {
        get
        {
            if (widget.groupRegistry is not null)
            {
                return widget.groupRegistry!;
            }
            RadioGroupRegistry<T>? inheritedRegistry = RadioGroup.maybeOf<T>(context);
            if (inheritedRegistry is not null)
            {
                return inheritedRegistry;
            }
            return _internalRadioRegistry ??= new _RadioRegistry__radio<T>(this);
        }
    }

    public override void dispose()
    {
        _internalFocusNode?.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(
            () =>
                !(widget.enabled ?? false)
                || (widget.onChanged is not null)
                || (widget.groupRegistry is not null)
                || (RadioGroup.maybeOf<T>(context) is not null),
            () =>
                (object?)"Radio is enabled but has no CupertinoRadio.onChange, "
                + "CupertinoRadio.groupRegistry, or RadioGroup above"
        );
        WidgetStateProperty<MouseCursor> effectiveMouseCursor = WidgetStateProperty.resolveWith(
            (states) =>
            {
                return WidgetStateProperty.resolveAs(widget.mouseCursor, states)
                    ?? (
                        (
                            !states.Contains(WidgetState.disabled)
                            && Foundation.ConstantsLibrary.kIsWeb
                        )
                            ? SystemMouseCursors.click
                            : SystemMouseCursors.basic
                    );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return new RawRadio<T>(
            value: widget.value,
            groupRegistry: _effectiveRegistry,
            mouseCursor: effectiveMouseCursor,
            toggleable: widget.toggleable,
            focusNode: _effectiveFocusNode,
            autofocus: widget.autofocus,
            enabled: _enabled,
            builder: (context, state) =>
            {
                return new _RadioPaint__radio(
                    activeColor: widget.activeColor,
                    inactiveColor: widget.inactiveColor,
                    fillColor: widget.fillColor,
                    focusColor: widget.focusColor,
                    useCheckmarkStyle: widget.useCheckmarkStyle,
                    isActive: _enabled,
                    toggleableState: state,
                    focused: _effectiveFocusNode.hasFocus
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _RadioRegistry__radio<T> : RadioGroupRegistry<T>
{
    public virtual _CupertinoRadioState__radio<T> state { get; private set; } = default!;

    internal _RadioRegistry__radio(_CupertinoRadioState__radio<T> state)
    {
        this.state = state;
    }

    public virtual T? groupValue => state.widget.groupValue;
    public virtual Action<T?> onChanged =>
        DartRuntimePrimitives.ConvertValue<Action<T?>>(state.widget.onChanged!);

    public virtual void registerClient(RadioClient<T> radio) { }

    public virtual void unregisterClient(RadioClient<T> radio) { }
}

internal class _RadioPaint__radio : StatefulWidget
{
    public virtual IToggleableState toggleableState { get; private set; } = default!;
    public virtual Color? activeColor { get; private set; }
    public virtual Color? inactiveColor { get; private set; }
    public virtual Color? fillColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual bool useCheckmarkStyle { get; private set; } = default!;
    public virtual bool isActive { get; private set; } = default!;
    public virtual bool focused { get; private set; } = default!;

    internal _RadioPaint__radio(
        bool focused,
        IToggleableState toggleableState,
        Color? activeColor,
        Color? inactiveColor,
        Color? fillColor,
        Color? focusColor,
        bool useCheckmarkStyle,
        bool isActive
    )
    {
        this.focused = focused;
        this.toggleableState = toggleableState;
        this.activeColor = activeColor;
        this.inactiveColor = inactiveColor;
        this.fillColor = fillColor;
        this.focusColor = focusColor;
        this.useCheckmarkStyle = useCheckmarkStyle;
        this.isActive = isActive;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _RadioPaintState__radio());
}

internal class _RadioPaintState__radio : State<_RadioPaint__radio>
{
    internal virtual _RadioPainter__radio _painter { get; private set; } =
        new _RadioPainter__radio();

    public override void dispose()
    {
        _painter.dispose();
        base.dispose();
    }

    internal virtual WidgetStateProperty<Color> _defaultOuterColor
    {
        get
        {
            return WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (states.Contains(WidgetState.disabled))
                    {
                        return CupertinoDynamicColor.resolve(
                            RadioLibrary._kDisabledOuterColor,
                            context
                        );
                    }
                    if (states.Contains(WidgetState.selected))
                    {
                        return widget.activeColor
                            ?? CupertinoDynamicColor.resolve(
                                RadioLibrary._kDefaultOuterColor,
                                context
                            );
                    }
                    return widget.inactiveColor ?? CupertinoColors.white;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
        }
    }
    internal virtual WidgetStateProperty<Color> _defaultInnerColor
    {
        get
        {
            return WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (
                        states.Contains(WidgetState.disabled)
                        && states.Contains(WidgetState.selected)
                    )
                    {
                        return widget.fillColor
                            ?? CupertinoDynamicColor.resolve(
                                RadioLibrary._kDisabledInnerColor,
                                context
                            );
                    }
                    if (states.Contains(WidgetState.selected))
                    {
                        return widget.fillColor
                            ?? CupertinoDynamicColor.resolve(
                                RadioLibrary._kDefaultInnerColor,
                                context
                            );
                    }
                    return CupertinoColors.white;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
        }
    }
    internal virtual WidgetStateProperty<Color> _defaultBorderColor
    {
        get
        {
            return WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (
                        (
                            states.Contains(WidgetState.selected)
                            || states.Contains(WidgetState.focused)
                        ) && !states.Contains(WidgetState.disabled)
                    )
                    {
                        return CupertinoColors.transparent;
                    }
                    if (states.Contains(WidgetState.disabled))
                    {
                        return CupertinoDynamicColor.resolve(
                            CheckboxLibrary._kDisabledBorderColor,
                            context
                        );
                    }
                    return CupertinoDynamicColor.resolve(
                        CheckboxLibrary._kDefaultBorderColor,
                        context
                    );
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
        }
    }

    public override Widget build(BuildContext context)
    {
        HashSet<WidgetState> activeStates = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __cascade = widget.toggleableState.states;
                    __cascade.Add(WidgetState.selected);
                    return __cascade;
                }
            )
        )();
        HashSet<WidgetState> inactiveStates = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __cascade = widget.toggleableState.states;
                    __cascade.Remove(WidgetState.selected);
                    return __cascade;
                }
            )
        )();
        HashSet<WidgetState> currentStates = widget.toggleableState.states;
        Color effectiveActiveColor = _defaultOuterColor.resolve(activeStates);
        Color effectiveInactiveColor = _defaultOuterColor.resolve(inactiveStates);
        Color effectiveFocusOverlayColor =
            widget.focusColor
            ?? HSLColor
                .CreateFromColor(
                    effectiveActiveColor.withOpacity(ConstantsLibrary.kCupertinoFocusColorOpacity)
                )
                .withLightness(ConstantsLibrary.kCupertinoFocusColorBrightness)
                .withSaturation(ConstantsLibrary.kCupertinoFocusColorSaturation)
                .toColor();
        Color effectiveFillColor = _defaultInnerColor.resolve(currentStates);
        Color effectiveBorderColor = _defaultBorderColor.resolve(currentStates);
        return new CustomPaint(
            size: RadioLibrary._size,
            painter: (
                (Func<_RadioPainter__radio>)(
                    () =>
                    {
                        var __cascade = _painter;
                        __cascade.position = widget.toggleableState.position;
                        __cascade.reaction = widget.toggleableState.reaction;
                        __cascade.focusColor = effectiveFocusOverlayColor;
                        __cascade.downPosition = widget.toggleableState.downPosition;
                        __cascade.isFocused = widget.focused;
                        __cascade.activeColor = effectiveActiveColor;
                        __cascade.inactiveColor = effectiveInactiveColor;
                        __cascade.fillColor = effectiveFillColor;
                        __cascade.value = widget.toggleableState.value;
                        __cascade.checkmarkStyle = widget.useCheckmarkStyle;
                        __cascade.isActive = widget.isActive;
                        __cascade.borderColor = effectiveBorderColor;
                        __cascade.brightness = CupertinoTheme.of(context).brightness;
                        return __cascade;
                    }
                )
            )()
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _RadioPainter__radio : ToggleablePainter
{
    internal virtual bool? _value { get; set; } = default;
    internal virtual Color? _fillColor { get; set; } = default;
    internal virtual bool _checkmarkStyle { get; set; } = false;
    internal virtual Brightness? _brightness { get; set; } = default;
    internal virtual Color? _borderColor { get; set; } = default;

    public virtual bool? value
    {
        get => _value;
        set
        {
            var __value = value;
            if (_value == __value)
            {
                return;
            }
            _value = __value;
            notifyListeners();
        }
    }
    public virtual Color fillColor
    {
        get => _fillColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _fillColor))
            {
                return;
            }
            _fillColor = __value;
            notifyListeners();
        }
    }
    public virtual bool checkmarkStyle
    {
        get => _checkmarkStyle;
        set
        {
            var __value = value;
            if ((__value) == _checkmarkStyle)
            {
                return;
            }
            _checkmarkStyle = ((__value));
            notifyListeners();
        }
    }
    public virtual Brightness? brightness
    {
        get => _brightness;
        set
        {
            var __value = value;
            if (Equals(_brightness, __value))
            {
                return;
            }
            _brightness = __value;
            notifyListeners();
        }
    }
    public virtual Color borderColor
    {
        get => _borderColor!;
        set
        {
            var __value = value;
            if (Equals(_borderColor, __value))
            {
                return;
            }
            _borderColor = __value;
            notifyListeners();
        }
    }

    internal virtual void _drawPressedOverlay(Canvas canvas, Offset center, double radius)
    {
        var pressedPaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = Equals(brightness, Brightness.light)
                        ? CupertinoColors.black.withOpacity(CheckboxLibrary._kPressedOverlayOpacity)
                        : CupertinoColors.white.withOpacity(
                            CheckboxLibrary._kPressedOverlayOpacity
                        );
                    return __cascade;
                }
            )
        )();
        canvas.drawCircle(center, radius, pressedPaint);
    }

    internal virtual void _drawFillGradient(
        Canvas canvas,
        Offset center,
        double radius,
        Color topColor,
        Color bottomColor
    )
    {
        var fillGradient = new LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: new List<Color> { topColor, bottomColor }
        );
        var circleRect = Rect.fromCircle(center: center, radius: radius);
        var gradientPaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.shader = fillGradient.createShader(circleRect);
                    return __cascade;
                }
            )
        )();
        canvas.drawPath(
            (
                (Func<Path>)(
                    () =>
                    {
                        var __cascade = new Path();
                        __cascade.addOval(circleRect);
                        return __cascade;
                    }
                )
            )(),
            gradientPaint
        );
    }

    internal virtual void _drawOuterBorder(Canvas canvas, Offset center)
    {
        var borderPaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.style = PaintingStyle.stroke;
                    __cascade.color = borderColor;
                    __cascade.strokeWidth = RadioLibrary._kBorderOutlineStrokeWidth;
                    return __cascade;
                }
            )
        )();
        canvas.drawCircle(center, RadioLibrary._kOuterRadius, borderPaint);
    }

    public override void paint(Canvas canvas, Size size)
    {
        Offset centerLocal = (Offset.zero & size).center;
        if (checkmarkStyle)
        {
            if (value ?? false)
            {
                var path = new Path();
                var checkPaint = (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.color = activeColor;
                            __cascade.style = PaintingStyle.stroke;
                            __cascade.strokeWidth = RadioLibrary._kCheckmarkStrokeWidth;
                            __cascade.strokeCap = StrokeCap.round;
                            return __cascade;
                        }
                    )
                )();
                double widthLocal = RadioLibrary._size.width;
                var origin = new Offset(
                    centerLocal.dx - (widthLocal / 2L),
                    centerLocal.dy - (widthLocal / 2L)
                );
                var start = new Offset(widthLocal * 0.25, widthLocal * 0.52);
                var mid = new Offset(widthLocal * 0.46, widthLocal * 0.75);
                var end = new Offset(widthLocal * 0.85, widthLocal * 0.29);
                path.moveTo(origin.dx + start.dx, origin.dy + start.dy);
                path.lineTo(origin.dx + mid.dx, origin.dy + mid.dy);
                canvas.drawPath(path, checkPaint);
                path.moveTo(origin.dx + mid.dx, origin.dy + mid.dy);
                path.lineTo(origin.dx + end.dx, origin.dy + end.dy);
                canvas.drawPath(path, checkPaint);
            }
        }
        else
        {
            if (value ?? false)
            {
                var outerPaint = (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.color = activeColor;
                            return __cascade;
                        }
                    )
                )();
                if (Equals(brightness, Brightness.dark) && !isActive)
                {
                    _drawFillGradient(
                        canvas,
                        centerLocal,
                        RadioLibrary._kOuterRadius,
                        outerPaint.color.withOpacity(
                            isActive
                                ? CheckboxLibrary._kDarkGradientOpacities[(int)0L]
                                : CheckboxLibrary._kDisabledDarkGradientOpacities[(int)0L]
                        ),
                        outerPaint.color.withOpacity(
                            isActive
                                ? CheckboxLibrary._kDarkGradientOpacities[(int)1L]
                                : CheckboxLibrary._kDisabledDarkGradientOpacities[(int)1L]
                        )
                    );
                }
                else
                {
                    canvas.drawCircle(centerLocal, RadioLibrary._kOuterRadius, outerPaint);
                }
                if (downPosition is not null)
                {
                    _drawPressedOverlay(canvas, centerLocal, RadioLibrary._kOuterRadius);
                }
                var innerPaint = (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.color = fillColor;
                            return __cascade;
                        }
                    )
                )();
                canvas.drawCircle(centerLocal, RadioLibrary._kInnerRadius, innerPaint);
                if (!isActive)
                {
                    _drawOuterBorder(canvas, centerLocal);
                }
            }
            else
            {
                var paintLocal = new Paint();
                paintLocal.color = isActive ? inactiveColor : RadioLibrary._kDisabledOuterColor;
                if (Equals(brightness, Brightness.dark))
                {
                    _drawFillGradient(
                        canvas,
                        centerLocal,
                        RadioLibrary._kOuterRadius,
                        paintLocal.color.withOpacity(
                            isActive
                                ? CheckboxLibrary._kDarkGradientOpacities[(int)0L]
                                : CheckboxLibrary._kDisabledDarkGradientOpacities[(int)0L]
                        ),
                        paintLocal.color.withOpacity(
                            isActive
                                ? CheckboxLibrary._kDarkGradientOpacities[(int)1L]
                                : CheckboxLibrary._kDisabledDarkGradientOpacities[(int)1L]
                        )
                    );
                }
                else
                {
                    canvas.drawCircle(centerLocal, RadioLibrary._kOuterRadius, paintLocal);
                }
                if (downPosition is not null)
                {
                    _drawPressedOverlay(canvas, centerLocal, RadioLibrary._kOuterRadius);
                }
                _drawOuterBorder(canvas, centerLocal);
            }
        }
        if (isFocused)
        {
            var focusPaint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.style = PaintingStyle.stroke;
                        __cascade.color = focusColor;
                        __cascade.strokeWidth = RadioLibrary._kFocusOutlineStrokeWidth;
                        return __cascade;
                    }
                )
            )();
            canvas.drawCircle(
                centerLocal,
                RadioLibrary._kOuterRadius + (RadioLibrary._kFocusOutlineStrokeWidth / 2L),
                focusPaint
            );
        }
    }
}
