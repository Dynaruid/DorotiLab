// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/radio.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public static partial class RadioLibrary
{
    internal static Size _size = new global::Doroti.Ui.Size(18.0, 18.0);
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
    internal static Color _kDisabledInnerColor = ((Color)(object?)new CupertinoDynamicColor(color: global::Doroti.Ui.Color.fromARGB(64L, 0L, 0L, 0L), darkColor: global::Doroti.Ui.Color.fromARGB(64L, 255L, 255L, 255L)));
}

public static partial class RadioLibrary
{
    internal static Color _kDisabledBorderColor = ((Color)(object?)new CupertinoDynamicColor(color: global::Doroti.Ui.Color.fromARGB(64L, 0L, 0L, 0L), darkColor: global::Doroti.Ui.Color.fromARGB(64L, 0L, 0L, 0L)));
}

public static partial class RadioLibrary
{
    internal static CupertinoDynamicColor _kDefaultBorderColor = new CupertinoDynamicColor(color: global::Doroti.Ui.Color.fromARGB(255L, 209L, 209L, 214L), darkColor: global::Doroti.Ui.Color.fromARGB(64L, 0L, 0L, 0L));
}

public static partial class RadioLibrary
{
    internal static CupertinoDynamicColor _kDefaultInnerColor = new CupertinoDynamicColor(color: CupertinoColors.white, darkColor: global::Doroti.Ui.Color.fromARGB(255L, 222L, 232L, 248L));
}

public static partial class RadioLibrary
{
    internal static CupertinoDynamicColor _kDefaultOuterColor = new CupertinoDynamicColor(color: CupertinoColors.activeBlue, darkColor: global::Doroti.Ui.Color.fromARGB(255L, 50L, 100L, 215L));
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

public class CupertinoRadio<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual T? groupValue { get; private set; }
    public virtual global::System.Action<T?>? onChanged { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual bool toggleable { get; private set; } = default!;
    public virtual bool useCheckmarkStyle { get; private set; } = default!;
    public virtual Color? activeColor { get; private set; }
    public virtual Color? inactiveColor { get; private set; }
    public virtual Color? fillColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.RadioGroupRegistry<T>? groupRegistry { get; private set; }
    public virtual bool? enabled { get; private set; }

    public CupertinoRadio(global::Doroti.Framework.Foundation.Key? key = null, T value = default!, T? groupValue = default, global::System.Action<T?>? onChanged = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, bool toggleable = false, Color? activeColor = null, Color? inactiveColor = null, Color? fillColor = null, Color? focusColor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, bool useCheckmarkStyle = false, bool? enabled = null, global::Doroti.Framework.Widgets.RadioGroupRegistry<T>? groupRegistry = null) : base(key: key)
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoRadioState__radio<T>());
}

internal class _CupertinoRadioState__radio<T> : global::Doroti.Framework.Widgets.State<CupertinoRadio<T>>
{
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _internalFocusNode { get; set; } = default;
    internal virtual _RadioRegistry__radio<T>? _internalRadioRegistry { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.FocusNode _effectiveFocusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>((((CupertinoRadio<T>)(object)this.widget).focusNode ?? (_internalFocusNode ??= new global::Doroti.Framework.Widgets.FocusNode())));
    internal virtual bool _enabled => DartRuntimePrimitives.ConvertValue<bool>((((CupertinoRadio<T>)(object)this.widget).enabled ?? ((((((CupertinoRadio<T>)(object)this.widget).onChanged is not null) || (((CupertinoRadio<T>)(object)this.widget).groupRegistry is not null)) || (RadioGroup.maybeOf<T>(this.context) is not null)))));
    internal virtual global::Doroti.Framework.Widgets.RadioGroupRegistry<T> _effectiveRegistry
    {
        get
        {
            if ((((CupertinoRadio<T>)(object)this.widget).groupRegistry is not null))
            {
                return ((CupertinoRadio<T>)(object)this.widget).groupRegistry!;
            }
            global::Doroti.Framework.Widgets.RadioGroupRegistry<T>? inheritedRegistry = ((global::Doroti.Framework.Widgets.RadioGroupRegistry<T>?)(object?)RadioGroup.maybeOf<T>(this.context));
            if ((inheritedRegistry is not null))
            {
                return inheritedRegistry;
            }
            return _internalRadioRegistry ??= new _RadioRegistry__radio<T>(this);
            return default!;
        }
    }
    public override void dispose()
    {
        this._internalFocusNode?.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (((!((((CupertinoRadio<T>)(object)this.widget).enabled ?? false)) || (((CupertinoRadio<T>)(object)this.widget).onChanged is not null)) || (((CupertinoRadio<T>)(object)this.widget).groupRegistry is not null)) || (RadioGroup.maybeOf<T>(context) is not null)), () => (object?)"Radio is enabled but has no CupertinoRadio.onChange, " + "CupertinoRadio.groupRegistry, or RadioGroup above");
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> effectiveMouseCursor = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Services.MouseCursor>)((states) =>
        {
            return ((global::Doroti.Framework.Services.MouseCursor)(object?)(WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(((CupertinoRadio<T>)(object)this.widget).mouseCursor, states) ?? (((!states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled) && global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb) ? global::Doroti.Framework.Services.SystemMouseCursors.click : global::Doroti.Framework.Services.SystemMouseCursors.basic))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.RawRadio<T>(value: ((CupertinoRadio<T>)(object)this.widget).value, groupRegistry: this._effectiveRegistry, mouseCursor: effectiveMouseCursor, toggleable: ((CupertinoRadio<T>)(object)this.widget).toggleable, focusNode: this._effectiveFocusNode, autofocus: ((CupertinoRadio<T>)(object)this.widget).autofocus, enabled: this._enabled, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, dynamic, global::Doroti.Framework.Widgets.Widget>)((context, state) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new _RadioPaint__radio(activeColor: ((CupertinoRadio<T>)(object)this.widget).activeColor, inactiveColor: ((CupertinoRadio<T>)(object)this.widget).inactiveColor, fillColor: ((CupertinoRadio<T>)(object)this.widget).fillColor, focusColor: ((CupertinoRadio<T>)(object)this.widget).focusColor, useCheckmarkStyle: ((CupertinoRadio<T>)(object)this.widget).useCheckmarkStyle, isActive: this._enabled, toggleableState: state, focused: ((global::Doroti.Framework.Widgets.FocusNode)this._effectiveFocusNode).hasFocus));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RadioRegistry__radio<T> : global::Doroti.Framework.Widgets.RadioGroupRegistry<T>
{
    public virtual _CupertinoRadioState__radio<T> state { get; private set; } = default!;

    internal _RadioRegistry__radio(_CupertinoRadioState__radio<T> state)
    {
        this.state = state;
    }

    public virtual T? groupValue => this.state.widget.groupValue;
    public virtual global::System.Action<T?> onChanged => DartRuntimePrimitives.ConvertValue<global::System.Action<T?>>(this.state.widget.onChanged!);
    public virtual void registerClient(global::Doroti.Framework.Widgets.RadioClient<T> radio)
    {
    }

    public virtual void unregisterClient(global::Doroti.Framework.Widgets.RadioClient<T> radio)
    {
    }

}

internal class _RadioPaint__radio : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual dynamic toggleableState { get; private set; } = default!;
    public virtual Color? activeColor { get; private set; }
    public virtual Color? inactiveColor { get; private set; }
    public virtual Color? fillColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual bool useCheckmarkStyle { get; private set; } = default!;
    public virtual bool isActive { get; private set; } = default!;
    public virtual bool focused { get; private set; } = default!;

    internal _RadioPaint__radio(bool focused, dynamic toggleableState, Color? activeColor, Color? inactiveColor, Color? fillColor, Color? focusColor, bool useCheckmarkStyle, bool isActive)
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RadioPaintState__radio());
}

internal class _RadioPaintState__radio : global::Doroti.Framework.Widgets.State<_RadioPaint__radio>
{
    internal virtual _RadioPainter__radio _painter { get; private set; } = new _RadioPainter__radio();

    public override void dispose()
    {
        this._painter.dispose();
        base.dispose();
    }

    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _defaultOuterColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color>((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    return CupertinoDynamicColor.resolve(RadioLibrary._kDisabledOuterColor, this.context);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return widget.activeColor ?? CupertinoDynamicColor.resolve(RadioLibrary._kDefaultOuterColor, this.context);
                }
                return widget.inactiveColor ?? CupertinoColors.white;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _defaultInnerColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color>((states) =>
            {
                if ((states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled) && states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected)))
                {
                    return widget.fillColor ?? CupertinoDynamicColor.resolve(RadioLibrary._kDisabledInnerColor, this.context);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return widget.fillColor ?? CupertinoDynamicColor.resolve(RadioLibrary._kDefaultInnerColor, this.context);
                }
                return CupertinoColors.white;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _defaultBorderColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color>((states) =>
            {
                if ((((states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) || states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))) && !states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled)))
                {
                    return CupertinoColors.transparent;
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    return CupertinoDynamicColor.resolve(CheckboxLibrary._kDisabledBorderColor, this.context);
                }
                return CupertinoDynamicColor.resolve(CheckboxLibrary._kDefaultBorderColor, this.context);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            return default!;
        }
    }
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        HashSet<global::Doroti.Framework.Widgets.WidgetState> activeStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = ((HashSet<global::Doroti.Framework.Widgets.WidgetState>)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).states);
    __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.selected);
    return __cascade;
}))();
        HashSet<global::Doroti.Framework.Widgets.WidgetState> inactiveStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = ((HashSet<global::Doroti.Framework.Widgets.WidgetState>)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).states);
    __cascade.Remove(global::Doroti.Framework.Widgets.WidgetState.selected);
    return __cascade;
}))();
        HashSet<global::Doroti.Framework.Widgets.WidgetState> currentStates = ((HashSet<global::Doroti.Framework.Widgets.WidgetState>)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).states);
        global::Doroti.Ui.Color effectiveActiveColor = ((global::Doroti.Ui.Color)(object?)this._defaultOuterColor.resolve(activeStates));
        global::Doroti.Ui.Color effectiveInactiveColor = ((global::Doroti.Ui.Color)(object?)this._defaultOuterColor.resolve(inactiveStates));
        global::Doroti.Ui.Color effectiveFocusOverlayColor = ((global::Doroti.Ui.Color)(object?)((((_RadioPaint__radio)(object)this.widget).focusColor ?? (Color)global::Doroti.Framework.Painting.HSLColor.CreateFromColor(effectiveActiveColor.withOpacity(ConstantsLibrary.kCupertinoFocusColorOpacity)).withLightness(ConstantsLibrary.kCupertinoFocusColorBrightness).withSaturation(ConstantsLibrary.kCupertinoFocusColorSaturation).toColor())));
        global::Doroti.Ui.Color effectiveFillColor = ((global::Doroti.Ui.Color)(object?)this._defaultInnerColor.resolve(currentStates));
        global::Doroti.Ui.Color effectiveBorderColor = ((global::Doroti.Ui.Color)(object?)this._defaultBorderColor.resolve(currentStates));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CustomPaint(size: RadioLibrary._size, painter: ((Func<_RadioPainter__radio>)(() =>
{
    var __cascade = this._painter;
    __cascade.position = ((global::Doroti.Framework.Animation.CurvedAnimation)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).position);
    __cascade.reaction = ((global::Doroti.Framework.Animation.CurvedAnimation)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).reaction);
    __cascade.focusColor = effectiveFocusOverlayColor;
    __cascade.downPosition = ((Offset?)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).downPosition);
    __cascade.isFocused = ((_RadioPaint__radio)(object)this.widget).focused;
    __cascade.activeColor = effectiveActiveColor;
    __cascade.inactiveColor = effectiveInactiveColor;
    __cascade.fillColor = effectiveFillColor;
    __cascade.value = ((bool?)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).value);
    __cascade.checkmarkStyle = ((_RadioPaint__radio)(object)this.widget).useCheckmarkStyle;
    __cascade.isActive = ((_RadioPaint__radio)(object)this.widget).isActive;
    __cascade.borderColor = effectiveBorderColor;
    __cascade.brightness = CupertinoTheme.of(context).brightness;
    return __cascade;
}))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RadioPainter__radio : global::Doroti.Framework.Widgets.ToggleablePainter
{
    internal virtual bool? _value { get; set; } = default;
    internal virtual Color? _fillColor { get; set; } = default;
    internal virtual bool _checkmarkStyle { get; set; } = false;
    internal virtual Brightness? _brightness { get; set; } = default;
    internal virtual Color? _borderColor { get; set; } = default;

    public virtual bool? value
    {
        get => this._value;
        set
        {
            var __value = value;
            if ((this._value == __value))
            {
                return;
            }
            _value = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color fillColor
    {
        get => this._fillColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._fillColor)))
            {
                return;
            }
            _fillColor = __value;
            notifyListeners();
        }
    }
    public virtual bool checkmarkStyle
    {
        get => this._checkmarkStyle;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._checkmarkStyle))
            {
                return;
            }
            _checkmarkStyle = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(__value));
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Brightness? brightness
    {
        get => this._brightness;
        set
        {
            var __value = value;
            if ((object.Equals(this._brightness, __value)))
            {
                return;
            }
            _brightness = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color borderColor
    {
        get => this._borderColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this._borderColor, __value)))
            {
                return;
            }
            _borderColor = __value;
            notifyListeners();
        }
    }
    internal virtual void _drawPressedOverlay(Canvas canvas, Offset center, double radius)
    {
        var pressedPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = ((object.Equals(this.brightness, Brightness.light)) ? CupertinoColors.black.withOpacity(CheckboxLibrary._kPressedOverlayOpacity) : CupertinoColors.white.withOpacity(CheckboxLibrary._kPressedOverlayOpacity));
    return __cascade;
}))();
        canvas.drawCircle(center, radius, pressedPaint);
    }

    internal virtual void _drawFillGradient(Canvas canvas, Offset center, double radius, Color topColor, Color bottomColor)
    {
        var fillGradient = new global::Doroti.Framework.Painting.LinearGradient(begin: global::Doroti.Framework.Painting.Alignment.topCenter, end: global::Doroti.Framework.Painting.Alignment.bottomCenter, colors: new List<global::Doroti.Ui.Color> { topColor, bottomColor });
        var circleRect = global::Doroti.Ui.Rect.fromCircle(center: center, radius: radius);
        var gradientPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.shader = fillGradient.createShader(circleRect);
    return __cascade;
}))();
        canvas.drawPath(((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addOval(circleRect);
    return __cascade;
}))(), gradientPaint);
    }

    internal virtual void _drawOuterBorder(Canvas canvas, Offset center)
    {
        var borderPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.color = this.borderColor;
    __cascade.strokeWidth = RadioLibrary._kBorderOutlineStrokeWidth;
    return __cascade;
}))();
        canvas.drawCircle(center, RadioLibrary._kOuterRadius, borderPaint);
    }

    public override void paint(Canvas canvas, Size size)
    {
        global::Doroti.Ui.Offset centerLocal = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)((Offset.zero & size))).center));
        if (this.checkmarkStyle)
        {
            if ((this.value ?? false))
            {
                var path = new global::Doroti.Ui.Path();
                var checkPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.activeColor;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = RadioLibrary._kCheckmarkStrokeWidth;
    __cascade.strokeCap = StrokeCap.round;
    return __cascade;
}))();
                double widthLocal = RadioLibrary._size.width;
                var origin = new global::Doroti.Ui.Offset((centerLocal.dx - ((widthLocal / 2L))), (centerLocal.dy - ((widthLocal / 2L))));
                var start = new global::Doroti.Ui.Offset((widthLocal * 0.25), (widthLocal * 0.52));
                var mid = new global::Doroti.Ui.Offset((widthLocal * 0.46), (widthLocal * 0.75));
                var end = new global::Doroti.Ui.Offset((widthLocal * 0.85), (widthLocal * 0.29));
                path.moveTo((origin.dx + start.dx), (origin.dy + start.dy));
                path.lineTo((origin.dx + mid.dx), (origin.dy + mid.dy));
                canvas.drawPath(path, checkPaint);
                path.moveTo((origin.dx + mid.dx), (origin.dy + mid.dy));
                path.lineTo((origin.dx + end.dx), (origin.dy + end.dy));
                canvas.drawPath(path, checkPaint);
            }
        }
        else
        {
            if ((this.value ?? false))
            {
                var outerPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.activeColor;
    return __cascade;
}))();
                if (((object.Equals(this.brightness, Brightness.dark)) && !this.isActive))
                {
                    _drawFillGradient(canvas, centerLocal, RadioLibrary._kOuterRadius, outerPaint.color.withOpacity((this.isActive ? CheckboxLibrary._kDarkGradientOpacities[(int)(0L)] : CheckboxLibrary._kDisabledDarkGradientOpacities[(int)(0L)])), outerPaint.color.withOpacity((this.isActive ? CheckboxLibrary._kDarkGradientOpacities[(int)(1L)] : CheckboxLibrary._kDisabledDarkGradientOpacities[(int)(1L)])));
                }
                else
                {
                    canvas.drawCircle(centerLocal, RadioLibrary._kOuterRadius, outerPaint);
                }
                if ((this.downPosition is not null))
                {
                    _drawPressedOverlay(canvas, centerLocal, RadioLibrary._kOuterRadius);
                }
                var innerPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.fillColor;
    return __cascade;
}))();
                canvas.drawCircle(centerLocal, RadioLibrary._kInnerRadius, innerPaint);
                if (!this.isActive)
                {
                    _drawOuterBorder(canvas, centerLocal);
                }
            }
            else
            {
                var paintLocal = new global::Doroti.Ui.Paint();
                paintLocal.color = (this.isActive ? this.inactiveColor : RadioLibrary._kDisabledOuterColor);
                if ((object.Equals(this.brightness, Brightness.dark)))
                {
                    _drawFillGradient(canvas, centerLocal, RadioLibrary._kOuterRadius, paintLocal.color.withOpacity((this.isActive ? CheckboxLibrary._kDarkGradientOpacities[(int)(0L)] : CheckboxLibrary._kDisabledDarkGradientOpacities[(int)(0L)])), paintLocal.color.withOpacity((this.isActive ? CheckboxLibrary._kDarkGradientOpacities[(int)(1L)] : CheckboxLibrary._kDisabledDarkGradientOpacities[(int)(1L)])));
                }
                else
                {
                    canvas.drawCircle(centerLocal, RadioLibrary._kOuterRadius, paintLocal);
                }
                if ((this.downPosition is not null))
                {
                    _drawPressedOverlay(canvas, centerLocal, RadioLibrary._kOuterRadius);
                }
                _drawOuterBorder(canvas, centerLocal);
            }
        }
        if (this.isFocused)
        {
            var focusPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.color = this.focusColor;
    __cascade.strokeWidth = RadioLibrary._kFocusOutlineStrokeWidth;
    return __cascade;
}))();
            canvas.drawCircle(centerLocal, (RadioLibrary._kOuterRadius + (RadioLibrary._kFocusOutlineStrokeWidth / 2L)), focusPaint);
        }
    }

}
