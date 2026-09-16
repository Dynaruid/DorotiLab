// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/radio.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal enum _RadioType__radio
{
    material,
    adaptive
}

public static partial class RadioLibrary
{
    internal static double _kOuterRadius = 8.0;
}

public static partial class RadioLibrary
{
    internal static double _kInnerRadius = 4.5;
}

public class Radio<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual T? groupValue { get; private set; }
    public virtual global::System.Action<T?>? onChanged { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual bool toggleable { get; private set; } = default!;
    public virtual Color? activeColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool useCupertinoCheckmarkStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.RadioGroupRegistry<T>? groupRegistry { get; private set; }
    internal virtual _RadioType__radio _radioType { get; private set; } = default!;
    public virtual bool? enabled { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? side { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? innerRadius { get; private set; }

    public Radio(global::Doroti.Framework.Foundation.Key? key = null, T value = default!, T? groupValue = default, global::System.Action<T?>? onChanged = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, bool toggleable = false, Color? activeColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, Color? focusColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, MaterialTapTargetSize? materialTapTargetSize = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, bool? enabled = null, global::Doroti.Framework.Widgets.RadioGroupRegistry<T>? groupRegistry = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? innerRadius = null) : base(key: key)
    {
        this.value = value;
        this.groupValue = groupValue;
        this.onChanged = onChanged;
        this.mouseCursor = mouseCursor;
        this.toggleable = toggleable;
        this.activeColor = activeColor;
        this.fillColor = fillColor;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.overlayColor = overlayColor;
        this.splashRadius = splashRadius;
        this.materialTapTargetSize = materialTapTargetSize;
        this.visualDensity = visualDensity;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.enabled = enabled;
        this.groupRegistry = groupRegistry;
        this.backgroundColor = backgroundColor;
        this.side = side;
        this.innerRadius = innerRadius;
        _radioType = _RadioType__radio.material;
        useCupertinoCheckmarkStyle = false;
    }

    public static Radio<T> CreateAdaptive(global::Doroti.Framework.Foundation.Key? key = null, T value = default!, T? groupValue = default, global::System.Action<T?>? onChanged = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, bool toggleable = false, Color? activeColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, Color? focusColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, MaterialTapTargetSize? materialTapTargetSize = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, bool useCupertinoCheckmarkStyle = false, bool? enabled = null, global::Doroti.Framework.Widgets.RadioGroupRegistry<T>? groupRegistry = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? innerRadius = null)
    {
        var __instance = new Radio<T>(key: key, value: value, groupValue: groupValue, onChanged: onChanged, mouseCursor: mouseCursor, toggleable: toggleable, activeColor: activeColor, fillColor: fillColor, focusColor: focusColor, hoverColor: hoverColor, overlayColor: overlayColor, splashRadius: splashRadius, materialTapTargetSize: materialTapTargetSize, visualDensity: visualDensity, focusNode: focusNode, autofocus: autofocus, enabled: enabled, groupRegistry: groupRegistry, backgroundColor: backgroundColor, side: side, innerRadius: innerRadius);
        __instance.value = value;
        __instance.groupValue = groupValue;
        __instance.onChanged = onChanged;
        __instance.mouseCursor = mouseCursor;
        __instance.toggleable = toggleable;
        __instance.activeColor = activeColor;
        __instance.fillColor = fillColor;
        __instance.focusColor = focusColor;
        __instance.hoverColor = hoverColor;
        __instance.overlayColor = overlayColor;
        __instance.splashRadius = splashRadius;
        __instance.materialTapTargetSize = materialTapTargetSize;
        __instance.visualDensity = visualDensity;
        __instance.focusNode = focusNode;
        __instance.autofocus = autofocus;
        __instance.useCupertinoCheckmarkStyle = useCupertinoCheckmarkStyle;
        __instance.enabled = enabled;
        __instance.groupRegistry = groupRegistry;
        __instance.backgroundColor = backgroundColor;
        __instance.side = side;
        __instance.innerRadius = innerRadius;
        __instance._radioType = _RadioType__radio.adaptive;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RadioState__radio<T>());
}

internal class _RadioState__radio<T> : global::Doroti.Framework.Widgets.State<Radio<T>>
{
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _internalFocusNode { get; set; } = default;
    internal virtual _RadioRegistry__radio<T>? _internalRadioRegistry { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.FocusNode _focusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>(widget.focusNode ?? (_internalFocusNode ??= new global::Doroti.Framework.Widgets.FocusNode()));
    internal virtual bool _enabled => DartRuntimePrimitives.ConvertValue<bool>(widget.enabled ?? (widget.onChanged is not null) || (widget.groupRegistry is not null) || (RadioGroup.maybeOf<T>(context) is not null));
    internal virtual global::Doroti.Framework.Widgets.RadioGroupRegistry<T> _effectiveRegistry
    {
        get
        {
            if (widget.groupRegistry is not null)
            {
                return widget.groupRegistry!;
            }
            global::Doroti.Framework.Widgets.RadioGroupRegistry<T>? inheritedRegistry = RadioGroup.maybeOf<T>(context);
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => !(widget.enabled ?? false) || (widget.onChanged is not null) || (widget.groupRegistry is not null) || (RadioGroup.maybeOf<T>(context) is not null), () => (object?)"Radio is enabled but has no Radio.onChange or registry above");
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        switch (widget._radioType)
        {
            case _RadioType__radio.material:
                {
                    break;
                }
            case _RadioType__radio.adaptive:
                {
                    ThemeData theme = Theme.of(context);
                    switch (theme.platform)
                    {
                        case TargetPlatform.android:
                        case TargetPlatform.fuchsia:
                        case TargetPlatform.linux:
                        case TargetPlatform.windows:
                            {
                                break;
                            }
                        case TargetPlatform.iOS:
                        case TargetPlatform.macOS:
                            {
                                return new CupertinoRadio<T>(value: widget.value, groupValue: widget.groupValue, onChanged: widget.onChanged, mouseCursor: widget.mouseCursor, toggleable: widget.toggleable, activeColor: widget.activeColor, focusColor: widget.focusColor, focusNode: _focusNode, autofocus: widget.autofocus, useCheckmarkStyle: widget.useCupertinoCheckmarkStyle, groupRegistry: _effectiveRegistry, enabled: _enabled);
                            }
                        default:
                            throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                    }
                    break;
                }
        }
        RadioThemeData radioTheme = RadioTheme.of(context);
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> effectiveMouseCursor = WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>((states) =>
        {
            return (WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(widget.mouseCursor, states) ?? (radioTheme.mouseCursor?.resolve(states))) ?? WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor>(WidgetStateMouseCursor.adaptiveClickable, states);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return new global::Doroti.Framework.Widgets.RawRadio<T>(value: widget.value, mouseCursor: effectiveMouseCursor, toggleable: widget.toggleable, focusNode: _focusNode, autofocus: widget.autofocus, groupRegistry: _effectiveRegistry, enabled: _enabled, builder: (context, state) =>
        {
            return new _RadioPaint__radio(toggleableState: state, activeColor: widget.activeColor, fillColor: widget.fillColor, hoverColor: widget.hoverColor, focusColor: widget.focusColor, overlayColor: widget.overlayColor, splashRadius: widget.splashRadius, visualDensity: widget.visualDensity, materialTapTargetSize: widget.materialTapTargetSize, backgroundColor: widget.backgroundColor, side: widget.side, innerRadius: widget.innerRadius);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RadioRegistry__radio<T> : global::Doroti.Framework.Widgets.RadioGroupRegistry<T>
{
    public virtual _RadioState__radio<T> state { get; private set; } = default!;

    internal _RadioRegistry__radio(_RadioState__radio<T> state)
    {
        this.state = state;
    }

    public virtual T? groupValue => state.widget.groupValue;
    public virtual global::System.Action<T?> onChanged => DartRuntimePrimitives.ConvertValue<global::System.Action<T?>>(state.widget.onChanged!);
    public virtual void registerClient(global::Doroti.Framework.Widgets.RadioClient<T> radio)
    {
    }

    public virtual void unregisterClient(global::Doroti.Framework.Widgets.RadioClient<T> radio)
    {
    }

}

internal class _RadioPaint__radio : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.IToggleableState toggleableState { get; private set; } = default!;
    public virtual Color? activeColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? side { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? innerRadius { get; private set; }

    internal _RadioPaint__radio(global::Doroti.Framework.Widgets.IToggleableState toggleableState, Color? activeColor, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor, Color? hoverColor, Color? focusColor, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor, double? splashRadius, VisualDensity? visualDensity, MaterialTapTargetSize? materialTapTargetSize, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor, global::Doroti.Framework.Painting.BorderSide? side, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? innerRadius)
    {
        this.toggleableState = toggleableState;
        this.activeColor = activeColor;
        this.fillColor = fillColor;
        this.hoverColor = hoverColor;
        this.focusColor = focusColor;
        this.overlayColor = overlayColor;
        this.splashRadius = splashRadius;
        this.visualDensity = visualDensity;
        this.materialTapTargetSize = materialTapTargetSize;
        this.backgroundColor = backgroundColor;
        this.side = side;
        this.innerRadius = innerRadius;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RadioPaintState__radio());
}

internal class _RadioPaintState__radio : global::Doroti.Framework.Widgets.State<_RadioPaint__radio>
{
    internal virtual _RadioPainter__radio _painter { get; private set; } = new _RadioPainter__radio();

    public override void dispose()
    {
        _painter.dispose();
        base.dispose();
    }

    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> _widgetFillColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color?>((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return null;
                }
                if (states.Contains(WidgetState.selected))
                {
                    return widget.activeColor;
                }
                return null;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    internal virtual global::Doroti.Framework.Painting.BorderSide? _resolveSide(global::Doroti.Framework.Painting.BorderSide? side, HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
    {
        if (side is global::Doroti.Framework.Widgets.WidgetStateProperty<object>)
        {
            global::Doroti.Framework.Widgets.WidgetStateProperty<object> side__as22874 = (global::Doroti.Framework.Widgets.WidgetStateProperty<object>)side;
            return DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.BorderSide?>(WidgetStateProperty.resolveAs<object>(side__as22874, states));
        }
        if (!states.Contains(WidgetState.selected))
        {
            return side;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        RadioThemeData radioTheme = RadioTheme.of(context);
        RadioThemeData defaults = new _RadioDefaultsM3__radio(context);
        var defaultBackgroundColor = defaults.backgroundColor ?? throw new InvalidOperationException("The built-in radio theme must provide backgroundColor.");
        var defaultFillColor = defaults.fillColor ?? throw new InvalidOperationException("The built-in radio theme must provide fillColor.");
        var defaultOverlayColor = defaults.overlayColor ?? throw new InvalidOperationException("The built-in radio theme must provide overlayColor.");
        HashSet<global::Doroti.Framework.Widgets.WidgetState> activeStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = widget.toggleableState.states;
    __cascade.Add(WidgetState.selected);
    return __cascade;
}))();
        HashSet<global::Doroti.Framework.Widgets.WidgetState> inactiveStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = widget.toggleableState.states;
    __cascade.Remove(WidgetState.selected);
    return __cascade;
}))();
        global::Doroti.Ui.Color? activeColorLocal = (widget.fillColor?.resolve(activeStates) ?? _widgetFillColor.resolve(activeStates)) ?? (radioTheme.fillColor?.resolve(activeStates));
        global::Doroti.Ui.Color effectiveActiveColor = activeColorLocal ?? defaultFillColor.resolve(activeStates)!;
        global::Doroti.Ui.Color? inactiveColorLocal = (widget.fillColor?.resolve(inactiveStates) ?? _widgetFillColor.resolve(inactiveStates)) ?? (radioTheme.fillColor?.resolve(inactiveStates));
        global::Doroti.Ui.Color effectiveInactiveColor = inactiveColorLocal ?? defaultFillColor.resolve(inactiveStates)!;
        global::Doroti.Ui.Color activeBackgroundColorLocal = (widget.backgroundColor?.resolve(activeStates) ?? (radioTheme.backgroundColor?.resolve(activeStates))) ?? defaultBackgroundColor.resolve(activeStates)!;
        global::Doroti.Ui.Color inactiveBackgroundColorLocal = (widget.backgroundColor?.resolve(inactiveStates) ?? (radioTheme.backgroundColor?.resolve(inactiveStates))) ?? defaultBackgroundColor.resolve(inactiveStates)!;
        HashSet<global::Doroti.Framework.Widgets.WidgetState> focusedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = widget.toggleableState.states;
    __cascade.Add(WidgetState.focused);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveFocusOverlayColor = ((widget.overlayColor?.resolve(focusedStates) ?? widget.focusColor) ?? (radioTheme.overlayColor?.resolve(focusedStates))) ?? defaultOverlayColor.resolve(focusedStates)!;
        HashSet<global::Doroti.Framework.Widgets.WidgetState> hoveredStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = widget.toggleableState.states;
    __cascade.Add(WidgetState.hovered);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveHoverOverlayColor = ((widget.overlayColor?.resolve(hoveredStates) ?? widget.hoverColor) ?? (radioTheme.overlayColor?.resolve(hoveredStates))) ?? defaultOverlayColor.resolve(hoveredStates)!;
        var activePressedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = activeStates;
    __cascade.Add(WidgetState.pressed);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveActivePressedOverlayColor = ((widget.overlayColor?.resolve(activePressedStates) ?? (radioTheme.overlayColor?.resolve(activePressedStates))) ?? activeColorLocal?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? defaultOverlayColor.resolve(activePressedStates)!;
        var inactivePressedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = inactiveStates;
    __cascade.Add(WidgetState.pressed);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveInactivePressedOverlayColor = ((widget.overlayColor?.resolve(inactivePressedStates) ?? (radioTheme.overlayColor?.resolve(inactivePressedStates))) ?? inactiveColorLocal?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? defaultOverlayColor.resolve(inactivePressedStates)!;
        if (widget.toggleableState.downPosition is not null)
        {
            effectiveHoverOverlayColor = widget.toggleableState.states.Contains(WidgetState.selected) ? effectiveActivePressedOverlayColor : effectiveInactivePressedOverlayColor;
            effectiveFocusOverlayColor = widget.toggleableState.states.Contains(WidgetState.selected) ? effectiveActivePressedOverlayColor : effectiveInactivePressedOverlayColor;
        }
        MaterialTapTargetSize effectiveMaterialTapTargetSize = DartRuntimePrimitives.RequireValue(widget.materialTapTargetSize ?? radioTheme.materialTapTargetSize ?? defaults.materialTapTargetSize);
        VisualDensity effectiveVisualDensity = (widget.visualDensity ?? radioTheme.visualDensity) ?? defaults.visualDensity!;
        global::Doroti.Ui.Size sizeLocal = effectiveMaterialTapTargetSize switch { var __constant26966 when Equals(__constant26966, MaterialTapTargetSize.padded) => new global::Doroti.Ui.Size(Widgets.ConstantsLibrary.kMinInteractiveDimension, Widgets.ConstantsLibrary.kMinInteractiveDimension), var __constant27093 when Equals(__constant27093, MaterialTapTargetSize.shrinkWrap) => new global::Doroti.Ui.Size(Widgets.ConstantsLibrary.kMinInteractiveDimension - 8.0, Widgets.ConstantsLibrary.kMinInteractiveDimension - 8.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        sizeLocal += effectiveVisualDensity.baseSizeAdjustment;
        global::Doroti.Framework.Painting.BorderSide activeSideLocal = (_resolveSide(widget.side, activeStates) ?? _resolveSide(radioTheme.side, activeStates)) ?? new global::Doroti.Framework.Painting.BorderSide(color: effectiveActiveColor, width: 2.0, strokeAlign: BorderSide.strokeAlignCenter);
        global::Doroti.Framework.Painting.BorderSide inactiveSideLocal = (_resolveSide(widget.side, inactiveStates) ?? _resolveSide(radioTheme.side, inactiveStates)) ?? new global::Doroti.Framework.Painting.BorderSide(color: effectiveInactiveColor, width: 2.0, strokeAlign: BorderSide.strokeAlignCenter);
        double innerRadiusLocal = (widget.innerRadius?.resolve(activeStates) ?? radioTheme.innerRadius?.resolve(activeStates)) ?? RadioLibrary._kInnerRadius;
        return new global::Doroti.Framework.Widgets.CustomPaint(size: sizeLocal, painter: ((Func<_RadioPainter__radio>)(() =>
{
    var __cascade = _painter;
    __cascade.position = widget.toggleableState.position;
    __cascade.reaction = widget.toggleableState.reaction;
    __cascade.reactionFocusFade = widget.toggleableState.reactionFocusFade;
    __cascade.reactionHoverFade = widget.toggleableState.reactionHoverFade;
    __cascade.inactiveReactionColor = effectiveInactivePressedOverlayColor;
    __cascade.reactionColor = effectiveActivePressedOverlayColor;
    __cascade.hoverColor = effectiveHoverOverlayColor;
    __cascade.focusColor = effectiveFocusOverlayColor;
    __cascade.splashRadius = (widget.splashRadius ?? radioTheme.splashRadius) ?? ConstantsLibrary.kRadialReactionRadius;
    __cascade.downPosition = widget.toggleableState.downPosition;
    __cascade.isFocused = widget.toggleableState.states.Contains(WidgetState.focused);
    __cascade.isHovered = widget.toggleableState.states.Contains(WidgetState.hovered);
    __cascade.activeColor = effectiveActiveColor;
    __cascade.inactiveColor = effectiveInactiveColor;
    __cascade.activeBackgroundColor = activeBackgroundColorLocal;
    __cascade.inactiveBackgroundColor = inactiveBackgroundColorLocal;
    __cascade.activeSide = activeSideLocal;
    __cascade.inactiveSide = inactiveSideLocal;
    __cascade.innerRadius = innerRadiusLocal;
    return __cascade;
}))());
    }

}

internal class _RadioPainter__radio : global::Doroti.Framework.Widgets.ToggleablePainter
{
    internal virtual Color? _inactiveBackgroundColor { get; set; } = default;
    internal virtual Color? _activeBackgroundColor { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.BorderSide? _inactiveSide { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.BorderSide? _activeSide { get; set; } = default;
    internal virtual double? _innerRadius { get; set; } = default;

    public virtual global::Doroti.Ui.Color inactiveBackgroundColor
    {
        get => _inactiveBackgroundColor!;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(_inactiveBackgroundColor, __value))
            {
                return;
            }
            _inactiveBackgroundColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activeBackgroundColor
    {
        get => _activeBackgroundColor!;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(_activeBackgroundColor, __value))
            {
                return;
            }
            _activeBackgroundColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide inactiveSide
    {
        get => _inactiveSide!;
        set
        {
            global::Doroti.Framework.Painting.BorderSide? __value = value;
            if (Equals(_inactiveSide, __value))
            {
                return;
            }
            _inactiveSide = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide activeSide
    {
        get => _activeSide!;
        set
        {
            global::Doroti.Framework.Painting.BorderSide? __value = value;
            if (Equals(_activeSide, __value))
            {
                return;
            }
            _activeSide = __value;
            notifyListeners();
        }
    }
    public virtual double innerRadius
    {
        get => DartRuntimePrimitives.RequireValue(_innerRadius);
        set
        {
            double? __value = value;
            if (_innerRadius == __value)
            {
                return;
            }
            _innerRadius = __value;
            notifyListeners();
        }
    }
    public override void paint(Canvas canvas, Size size)
    {
        paintRadialReaction(canvas: canvas, origin: size.center(Offset.zero));
        global::Doroti.Ui.Rect rect = Offset.zero & size;
        global::Doroti.Ui.Offset centerLocal = rect.center;
        global::Doroti.Ui.Rect effectiveRect = (centerLocal & new global::Doroti.Ui.Size(RadioLibrary._kOuterRadius * 2L)).translate(-RadioLibrary._kOuterRadius, -RadioLibrary._kOuterRadius);
        var backgroundPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = Dart_uiLibrary.Color.lerp(inactiveBackgroundColor, activeBackgroundColor, position.value)!;
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
        canvas.drawCircle(centerLocal, RadioLibrary._kOuterRadius, backgroundPaint);
        global::Doroti.Framework.Painting.BorderSide sideLocal = BorderSide.lerp(inactiveSide, activeSide, DartRuntimePrimitives.RequireValue(position.value));
        new global::Doroti.Framework.Painting.CircleBorder(side: sideLocal).paint(canvas, effectiveRect);
        if (!position.isDismissed)
        {
            var innerCirclePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.fill;
    __cascade.color = Dart_uiLibrary.Color.lerp(inactiveColor, activeColor, position.value)!;
    return __cascade;
}))();
            canvas.drawCircle(centerLocal, innerRadius * position.value, innerCirclePaint);
        }
    }

}

internal class _RadioDefaultsM3__radio : RadioThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = _theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _RadioDefaultsM3__radio(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> fillColor
    {
        get
        {
            return WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.disabled))
                    {
                        return _colors.onSurface.withOpacity(0.38);
                    }
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.primary;
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.primary;
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.primary;
                    }
                    return _colors.primary;
                }
                if (states.Contains(WidgetState.disabled))
                {
                    return _colors.onSurface.withOpacity(0.38);
                }
                if (states.Contains(WidgetState.pressed))
                {
                    return _colors.onSurface;
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return _colors.onSurface;
                }
                if (states.Contains(WidgetState.focused))
                {
                    return _colors.onSurface;
                }
                return _colors.onSurfaceVariant;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> overlayColor
    {
        get
        {
            return WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onSurface.withOpacity(0.1);
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.primary.withOpacity(0.08);
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.primary.withOpacity(0.1);
                    }
                    return Colors.transparent;
                }
                if (states.Contains(WidgetState.pressed))
                {
                    return _colors.primary.withOpacity(0.1);
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return _colors.onSurface.withOpacity(0.08);
                }
                if (states.Contains(WidgetState.focused))
                {
                    return _colors.onSurface.withOpacity(0.1);
                }
                return Colors.transparent;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override MaterialTapTargetSize? materialTapTargetSize => _theme.materialTapTargetSize;
    public override VisualDensity visualDensity => _theme.visualDensity;
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(WidgetStateProperty.all<global::Doroti.Ui.Color>(Colors.transparent));
}
