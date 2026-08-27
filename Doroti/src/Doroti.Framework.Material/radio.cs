// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/radio.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
        this._radioType = _RadioType__radio.material;
        this.useCupertinoCheckmarkStyle = false;
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

    internal virtual global::Doroti.Framework.Widgets.FocusNode _focusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>((((Radio<T>)(object)this.widget).focusNode ?? (_internalFocusNode ??= new global::Doroti.Framework.Widgets.FocusNode())));
    internal virtual bool _enabled => DartRuntimePrimitives.ConvertValue<bool>((((Radio<T>)(object)this.widget).enabled ?? ((((((Radio<T>)(object)this.widget).onChanged is not null) || (((Radio<T>)(object)this.widget).groupRegistry is not null)) || (RadioGroup.maybeOf<T>(this.context) is not null)))));
    internal virtual global::Doroti.Framework.Widgets.RadioGroupRegistry<T> _effectiveRegistry
    {
        get
        {
            if ((((Radio<T>)(object)this.widget).groupRegistry is not null))
            {
                return ((Radio<T>)(object)this.widget).groupRegistry!;
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
        DartRuntimePrimitives.Assert(() => (((!((((Radio<T>)(object)this.widget).enabled ?? false)) || (((Radio<T>)(object)this.widget).onChanged is not null)) || (((Radio<T>)(object)this.widget).groupRegistry is not null)) || (RadioGroup.maybeOf<T>(context) is not null)), () => (object?)"Radio is enabled but has no Radio.onChange or registry above");
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        switch (((Radio<T>)(object)this.widget)._radioType)
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
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                        case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                            {
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                            {
                                return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoRadio<T>(value: ((Radio<T>)(object)this.widget).value, groupValue: ((Radio<T>)(object)this.widget).groupValue, onChanged: ((Radio<T>)(object)this.widget).onChanged, mouseCursor: ((Radio<T>)(object)this.widget).mouseCursor, toggleable: ((Radio<T>)(object)this.widget).toggleable, activeColor: ((Radio<T>)(object)this.widget).activeColor, focusColor: ((Radio<T>)(object)this.widget).focusColor, focusNode: this._focusNode, autofocus: ((Radio<T>)(object)this.widget).autofocus, useCheckmarkStyle: ((Radio<T>)(object)this.widget).useCupertinoCheckmarkStyle, groupRegistry: this._effectiveRegistry, enabled: this._enabled));
                            }
                        default:
                            throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                    }
                    break;
                }
        }
        RadioThemeData radioTheme = RadioTheme.of(context);
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> effectiveMouseCursor = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>((states) =>
        {
            return (((((WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(((Radio<T>)(object)this.widget).mouseCursor, states) ?? (global::Doroti.Framework.Services.MouseCursor)radioTheme.mouseCursor?.resolve(states))) ?? (global::Doroti.Framework.Services.MouseCursor)WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor>(global::Doroti.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable, states))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.RawRadio<T>(value: ((Radio<T>)(object)this.widget).value, mouseCursor: effectiveMouseCursor, toggleable: ((Radio<T>)(object)this.widget).toggleable, focusNode: this._focusNode, autofocus: ((Radio<T>)(object)this.widget).autofocus, groupRegistry: this._effectiveRegistry, enabled: this._enabled, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, dynamic, global::Doroti.Framework.Widgets.Widget>)((context, state) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new _RadioPaint__radio(toggleableState: state, activeColor: ((Radio<T>)(object)this.widget).activeColor, fillColor: ((Radio<T>)(object)this.widget).fillColor, hoverColor: ((Radio<T>)(object)this.widget).hoverColor, focusColor: ((Radio<T>)(object)this.widget).focusColor, overlayColor: ((Radio<T>)(object)this.widget).overlayColor, splashRadius: ((Radio<T>)(object)this.widget).splashRadius, visualDensity: ((Radio<T>)(object)this.widget).visualDensity, materialTapTargetSize: ((Radio<T>)(object)this.widget).materialTapTargetSize, backgroundColor: ((Radio<T>)(object)this.widget).backgroundColor, side: ((Radio<T>)(object)this.widget).side, innerRadius: ((Radio<T>)(object)this.widget).innerRadius));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
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

    internal _RadioPaint__radio(dynamic toggleableState, Color? activeColor, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor, Color? hoverColor, Color? focusColor, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor, double? splashRadius, VisualDensity? visualDensity, MaterialTapTargetSize? materialTapTargetSize, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor, global::Doroti.Framework.Painting.BorderSide? side, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? innerRadius)
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
        this._painter.dispose();
        base.dispose();
    }

    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> _widgetFillColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color?>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, Color?>)((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    return null;
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return ((_RadioPaint__radio)(object)this.widget).activeColor;
                }
                return null;
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Painting.BorderSide? _resolveSide(global::Doroti.Framework.Painting.BorderSide? side, HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
    {
        if ((side is global::Doroti.Framework.Widgets.WidgetStateProperty<object>))
        {
            global::Doroti.Framework.Widgets.WidgetStateProperty<object> side__as22874 = (global::Doroti.Framework.Widgets.WidgetStateProperty<object>)side;
            return ((global::Doroti.Framework.Painting.BorderSide?)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.BorderSide?>(WidgetStateProperty.resolveAs<object>(side__as22874, states)));
        }
        if (!states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
        {
            return side;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        RadioThemeData radioTheme = RadioTheme.of(context);
        dynamic defaults = (Theme.of(context).useMaterial3 ? new _RadioDefaultsM3__radio(context) : new _RadioDefaultsM2__radio(context));
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
        global::Doroti.Ui.Color? activeColorLocal = ((global::Doroti.Ui.Color?)(object?)((((((_RadioPaint__radio)(object)this.widget).fillColor?.resolve(activeStates) ?? (Color)this._widgetFillColor.resolve(activeStates))) ?? (Color)radioTheme.fillColor?.resolve(activeStates))));
        global::Doroti.Ui.Color effectiveActiveColor = ((global::Doroti.Ui.Color)(object?)(activeColorLocal ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.fillColor).resolve(activeStates)!));
        global::Doroti.Ui.Color? inactiveColorLocal = ((global::Doroti.Ui.Color?)(object?)((((((_RadioPaint__radio)(object)this.widget).fillColor?.resolve(inactiveStates) ?? (Color)this._widgetFillColor.resolve(inactiveStates))) ?? (Color)radioTheme.fillColor?.resolve(inactiveStates))));
        global::Doroti.Ui.Color effectiveInactiveColor = ((global::Doroti.Ui.Color)(object?)(inactiveColorLocal ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.fillColor).resolve(inactiveStates)!));
        global::Doroti.Ui.Color activeBackgroundColorLocal = ((global::Doroti.Ui.Color)(object?)(((((_RadioPaint__radio)(object)this.widget).backgroundColor?.resolve(activeStates) ?? (Color)radioTheme.backgroundColor?.resolve(activeStates))) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.backgroundColor).resolve(activeStates)!));
        global::Doroti.Ui.Color inactiveBackgroundColorLocal = ((global::Doroti.Ui.Color)(object?)(((((_RadioPaint__radio)(object)this.widget).backgroundColor?.resolve(inactiveStates) ?? (Color)radioTheme.backgroundColor?.resolve(inactiveStates))) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.backgroundColor).resolve(inactiveStates)!));
        HashSet<global::Doroti.Framework.Widgets.WidgetState> focusedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = ((HashSet<global::Doroti.Framework.Widgets.WidgetState>)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).states);
    __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.focused);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveFocusOverlayColor = ((global::Doroti.Ui.Color)(object?)((((((_RadioPaint__radio)(object)this.widget).overlayColor?.resolve(focusedStates) ?? ((_RadioPaint__radio)(object)this.widget).focusColor) ?? (Color)radioTheme.overlayColor?.resolve(focusedStates))) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.overlayColor).resolve(focusedStates)!));
        HashSet<global::Doroti.Framework.Widgets.WidgetState> hoveredStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = ((HashSet<global::Doroti.Framework.Widgets.WidgetState>)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).states);
    __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.hovered);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveHoverOverlayColor = ((global::Doroti.Ui.Color)(object?)((((((_RadioPaint__radio)(object)this.widget).overlayColor?.resolve(hoveredStates) ?? ((_RadioPaint__radio)(object)this.widget).hoverColor) ?? (Color)radioTheme.overlayColor?.resolve(hoveredStates))) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.overlayColor).resolve(hoveredStates)!));
        var activePressedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = activeStates;
    __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.pressed);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveActivePressedOverlayColor = ((global::Doroti.Ui.Color)(object?)((((((_RadioPaint__radio)(object)this.widget).overlayColor?.resolve(activePressedStates) ?? (Color)radioTheme.overlayColor?.resolve(activePressedStates))) ?? activeColorLocal?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.overlayColor).resolve(activePressedStates)!));
        var inactivePressedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = inactiveStates;
    __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.pressed);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveInactivePressedOverlayColor = ((global::Doroti.Ui.Color)(object?)((((((_RadioPaint__radio)(object)this.widget).overlayColor?.resolve(inactivePressedStates) ?? (Color)radioTheme.overlayColor?.resolve(inactivePressedStates))) ?? inactiveColorLocal?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.overlayColor).resolve(inactivePressedStates)!));
        if ((((Offset?)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).downPosition) is not null))
        {
            effectiveHoverOverlayColor = (((HashSet<global::Doroti.Framework.Widgets.WidgetState>)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).states).Contains(global::Doroti.Framework.Widgets.WidgetState.selected) ? effectiveActivePressedOverlayColor : effectiveInactivePressedOverlayColor);
            effectiveFocusOverlayColor = (((HashSet<global::Doroti.Framework.Widgets.WidgetState>)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).states).Contains(global::Doroti.Framework.Widgets.WidgetState.selected) ? effectiveActivePressedOverlayColor : effectiveInactivePressedOverlayColor);
        }
        MaterialTapTargetSize effectiveMaterialTapTargetSize = DartRuntimePrimitives.RequireValue(((_RadioPaint__radio)(object)this.widget).materialTapTargetSize ?? radioTheme.materialTapTargetSize ?? defaults.materialTapTargetSize);
        VisualDensity effectiveVisualDensity = ((((_RadioPaint__radio)(object)this.widget).visualDensity ?? radioTheme.visualDensity) ?? defaults.visualDensity!);
        global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)(object?)(effectiveMaterialTapTargetSize switch { var __constant26966 when (object.Equals(__constant26966, MaterialTapTargetSize.padded)) => new global::Doroti.Ui.Size(global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension, global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension), var __constant27093 when (object.Equals(__constant27093, MaterialTapTargetSize.shrinkWrap)) => new global::Doroti.Ui.Size((global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension - 8.0), (global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension - 8.0)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        sizeLocal += effectiveVisualDensity.baseSizeAdjustment;
        global::Doroti.Framework.Painting.BorderSide activeSideLocal = (((_resolveSide(((_RadioPaint__radio)(object)this.widget).side, activeStates) ?? (global::Doroti.Framework.Painting.BorderSide)_resolveSide(radioTheme.side, activeStates))) ?? new global::Doroti.Framework.Painting.BorderSide(color: effectiveActiveColor, width: 2.0, strokeAlign: global::Doroti.Framework.Painting.BorderSide.strokeAlignCenter));
        global::Doroti.Framework.Painting.BorderSide inactiveSideLocal = (((_resolveSide(((_RadioPaint__radio)(object)this.widget).side, inactiveStates) ?? (global::Doroti.Framework.Painting.BorderSide)_resolveSide(radioTheme.side, inactiveStates))) ?? new global::Doroti.Framework.Painting.BorderSide(color: effectiveInactiveColor, width: 2.0, strokeAlign: global::Doroti.Framework.Painting.BorderSide.strokeAlignCenter));
        double innerRadiusLocal = (((((_RadioPaint__radio)(object)this.widget).innerRadius?.resolve(activeStates) ?? radioTheme.innerRadius?.resolve(activeStates))) ?? RadioLibrary._kInnerRadius);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CustomPaint(size: sizeLocal, painter: ((Func<_RadioPainter__radio>)(() =>
{
    var __cascade = this._painter;
    __cascade.position = ((global::Doroti.Framework.Animation.CurvedAnimation)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).position);
    __cascade.reaction = ((global::Doroti.Framework.Animation.CurvedAnimation)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).reaction);
    __cascade.reactionFocusFade = ((global::Doroti.Framework.Animation.CurvedAnimation)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).reactionFocusFade);
    __cascade.reactionHoverFade = ((global::Doroti.Framework.Animation.CurvedAnimation)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).reactionHoverFade);
    __cascade.inactiveReactionColor = effectiveInactivePressedOverlayColor;
    __cascade.reactionColor = effectiveActivePressedOverlayColor;
    __cascade.hoverColor = effectiveHoverOverlayColor;
    __cascade.focusColor = effectiveFocusOverlayColor;
    __cascade.splashRadius = ((((_RadioPaint__radio)(object)this.widget).splashRadius ?? radioTheme.splashRadius) ?? ConstantsLibrary.kRadialReactionRadius);
    __cascade.downPosition = ((Offset?)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).downPosition);
    __cascade.isFocused = ((HashSet<global::Doroti.Framework.Widgets.WidgetState>)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).states).Contains(global::Doroti.Framework.Widgets.WidgetState.focused);
    __cascade.isHovered = ((HashSet<global::Doroti.Framework.Widgets.WidgetState>)((dynamic)((_RadioPaint__radio)(object)this.widget).toggleableState).states).Contains(global::Doroti.Framework.Widgets.WidgetState.hovered);
    __cascade.activeColor = effectiveActiveColor;
    __cascade.inactiveColor = effectiveInactiveColor;
    __cascade.activeBackgroundColor = activeBackgroundColorLocal;
    __cascade.inactiveBackgroundColor = inactiveBackgroundColorLocal;
    __cascade.activeSide = activeSideLocal;
    __cascade.inactiveSide = inactiveSideLocal;
    __cascade.innerRadius = innerRadiusLocal;
    return __cascade;
}))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        get => this._inactiveBackgroundColor!;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            if ((object.Equals(this._inactiveBackgroundColor, __value)))
            {
                return;
            }
            _inactiveBackgroundColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activeBackgroundColor
    {
        get => this._activeBackgroundColor!;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            if ((object.Equals(this._activeBackgroundColor, __value)))
            {
                return;
            }
            _activeBackgroundColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide inactiveSide
    {
        get => this._inactiveSide!;
        set
        {
            global::Doroti.Framework.Painting.BorderSide? __value = value;
            if ((object.Equals(this._inactiveSide, __value)))
            {
                return;
            }
            _inactiveSide = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide activeSide
    {
        get => this._activeSide!;
        set
        {
            global::Doroti.Framework.Painting.BorderSide? __value = value;
            if ((object.Equals(this._activeSide, __value)))
            {
                return;
            }
            _activeSide = __value;
            notifyListeners();
        }
    }
    public virtual double innerRadius
    {
        get => DartRuntimePrimitives.RequireValue(this._innerRadius);
        set
        {
            double? __value = value;
            if ((this._innerRadius == __value))
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
        global::Doroti.Ui.Rect rect = ((global::Doroti.Ui.Rect)(object?)(Offset.zero & size));
        global::Doroti.Ui.Offset centerLocal = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)rect).center));
        global::Doroti.Ui.Rect effectiveRect = ((global::Doroti.Ui.Rect)(object?)((centerLocal & new global::Doroti.Ui.Size((RadioLibrary._kOuterRadius * 2L)))).translate(-RadioLibrary._kOuterRadius, -RadioLibrary._kOuterRadius));
        var backgroundPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = Dart_uiLibrary.Color.lerp(this.inactiveBackgroundColor, this.activeBackgroundColor, ((global::Doroti.Framework.Animation.Animation<double>)this.position).value)!;
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
        canvas.drawCircle(centerLocal, RadioLibrary._kOuterRadius, backgroundPaint);
        global::Doroti.Framework.Painting.BorderSide sideLocal = ((global::Doroti.Framework.Painting.BorderSide)(object?)BorderSide.lerp(this.inactiveSide, this.activeSide, DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Animation.Animation<double>)this.position).value)));
        new global::Doroti.Framework.Painting.CircleBorder(side: sideLocal).paint(canvas, effectiveRect);
        if (!((global::Doroti.Framework.Animation.Animation<double>)this.position).isDismissed)
        {
            var innerCirclePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.fill;
    __cascade.color = Dart_uiLibrary.Color.lerp(this.inactiveColor, this.activeColor, ((global::Doroti.Framework.Animation.Animation<double>)this.position).value)!;
    return __cascade;
}))();
            canvas.drawCircle(centerLocal, (this.innerRadius * ((global::Doroti.Framework.Animation.Animation<double>)this.position).value), innerCirclePaint);
        }
    }

}

internal class _RadioDefaultsM2__radio : RadioThemeData
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
                __late__theme = Theme.of(this.context);
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
                __late__colors = this._theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _RadioDefaultsM2__radio(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> fillColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    return (this._theme.disabledColor);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return (this._colors.secondary);
                }
                return (this._theme.unselectedWidgetColor);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> overlayColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                {
                    return (this.fillColor.resolve(states).withAlpha(ConstantsLibrary.kRadialReactionAlpha));
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                {
                    return (this._theme.hoverColor);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                {
                    return (this._theme.focusColor);
                }
                return (Colors.transparent);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public virtual MaterialTapTargetSize materialTapTargetSize => this._theme.materialTapTargetSize;
    public virtual VisualDensity visualDensity => this._theme.visualDensity;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(WidgetStateProperty.all<global::Doroti.Ui.Color>(Colors.transparent));
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
                __late__theme = Theme.of(this.context);
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
                __late__colors = this._theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _RadioDefaultsM3__radio(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> fillColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                    {
                        return (this._colors.onSurface.withOpacity(0.38));
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                    {
                        return (this._colors.primary);
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                    {
                        return (this._colors.primary);
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                    {
                        return (this._colors.primary);
                    }
                    return (this._colors.primary);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    return (this._colors.onSurface.withOpacity(0.38));
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                {
                    return (this._colors.onSurface);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                {
                    return (this._colors.onSurface);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                {
                    return (this._colors.onSurface);
                }
                return (this._colors.onSurfaceVariant);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> overlayColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                    {
                        return (this._colors.onSurface.withOpacity(0.1));
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                    {
                        return (this._colors.primary.withOpacity(0.08));
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                    {
                        return (this._colors.primary.withOpacity(0.1));
                    }
                    return (Colors.transparent);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                {
                    return (this._colors.primary.withOpacity(0.1));
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                {
                    return (this._colors.onSurface.withOpacity(0.08));
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                {
                    return (this._colors.onSurface.withOpacity(0.1));
                }
                return (Colors.transparent);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public virtual MaterialTapTargetSize materialTapTargetSize => this._theme.materialTapTargetSize;
    public virtual VisualDensity visualDensity => this._theme.visualDensity;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(WidgetStateProperty.all<global::Doroti.Ui.Color>(Colors.transparent));
}
