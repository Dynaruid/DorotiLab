// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/switch.dart
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

public enum _SwitchType__switch
{
    material,
    adaptive
}

public class Switch : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual bool value { get; private set; } = default!;
    public virtual global::System.Action<bool>? onChanged { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual Color? activeThumbColor { get; private set; }
    public virtual Color? activeTrackColor { get; private set; }
    public virtual Color? inactiveThumbColor { get; private set; }
    public virtual Color? inactiveTrackColor { get; private set; }
    public virtual dynamic activeThumbImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError { get; private set; }
    public virtual dynamic inactiveThumbImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    internal virtual _SwitchType__switch _switchType { get; private set; } = default!;
    public virtual bool? applyCupertinoTheme { get; private set; }
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }

    public Switch(global::Doroti.Framework.Foundation.Key? key = null, bool value = default!, global::System.Action<bool>? onChanged = default!, Color? activeColor = null, Color? activeThumbColor = null, Color? activeTrackColor = null, Color? inactiveThumbColor = null, Color? inactiveTrackColor = null, dynamic activeThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError = null, dynamic inactiveThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Color? focusColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null) : base(key: key)
    {
        this.value = value;
        this.onChanged = onChanged;
        this.activeColor = activeColor;
        this.activeThumbColor = activeThumbColor;
        this.activeTrackColor = activeTrackColor;
        this.inactiveThumbColor = inactiveThumbColor;
        this.inactiveTrackColor = inactiveTrackColor;
        this.activeThumbImage = activeThumbImage;
        this.onActiveThumbImageError = onActiveThumbImageError;
        this.inactiveThumbImage = inactiveThumbImage;
        this.onInactiveThumbImageError = onInactiveThumbImageError;
        this.thumbColor = thumbColor;
        this.trackColor = trackColor;
        this.trackOutlineColor = trackOutlineColor;
        this.trackOutlineWidth = trackOutlineWidth;
        this.thumbIcon = thumbIcon;
        this.materialTapTargetSize = materialTapTargetSize;
        this.dragStartBehavior = dragStartBehavior;
        this.mouseCursor = mouseCursor;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.overlayColor = overlayColor;
        this.splashRadius = splashRadius;
        this.focusNode = focusNode;
        this.onFocusChange = onFocusChange;
        this.autofocus = autofocus;
        this.padding = padding;
        this._switchType = _SwitchType__switch.material;
        this.applyCupertinoTheme = false;
        System.Diagnostics.Debug.Assert(((activeThumbImage is not null) || (onActiveThumbImageError is null)));
        System.Diagnostics.Debug.Assert(((inactiveThumbImage is not null) || (onInactiveThumbImageError is null)));
    }

    public static Switch CreateAdaptive(global::Doroti.Framework.Foundation.Key? key = null, bool value = default!, global::System.Action<bool>? onChanged = default!, Color? activeColor = null, Color? activeThumbColor = null, Color? activeTrackColor = null, Color? inactiveThumbColor = null, Color? inactiveTrackColor = null, dynamic activeThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError = null, dynamic inactiveThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Color? focusColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, bool? applyCupertinoTheme = null)
    {
        var __instance = new Switch(key: key, value: value, onChanged: onChanged, activeColor: activeColor, activeThumbColor: activeThumbColor, activeTrackColor: activeTrackColor, inactiveThumbColor: inactiveThumbColor, inactiveTrackColor: inactiveTrackColor, activeThumbImage: activeThumbImage, onActiveThumbImageError: onActiveThumbImageError, inactiveThumbImage: inactiveThumbImage, onInactiveThumbImageError: onInactiveThumbImageError, thumbColor: thumbColor, trackColor: trackColor, trackOutlineColor: trackOutlineColor, trackOutlineWidth: trackOutlineWidth, thumbIcon: thumbIcon, materialTapTargetSize: materialTapTargetSize, dragStartBehavior: dragStartBehavior, mouseCursor: mouseCursor, focusColor: focusColor, hoverColor: hoverColor, overlayColor: overlayColor, splashRadius: splashRadius, focusNode: focusNode, onFocusChange: onFocusChange, autofocus: autofocus, padding: padding);
        __instance.value = value;
        __instance.onChanged = onChanged;
        __instance.activeColor = activeColor;
        __instance.activeThumbColor = activeThumbColor;
        __instance.activeTrackColor = activeTrackColor;
        __instance.inactiveThumbColor = inactiveThumbColor;
        __instance.inactiveTrackColor = inactiveTrackColor;
        __instance.activeThumbImage = activeThumbImage;
        __instance.onActiveThumbImageError = onActiveThumbImageError;
        __instance.inactiveThumbImage = inactiveThumbImage;
        __instance.onInactiveThumbImageError = onInactiveThumbImageError;
        __instance.materialTapTargetSize = materialTapTargetSize;
        __instance.thumbColor = thumbColor;
        __instance.trackColor = trackColor;
        __instance.trackOutlineColor = trackOutlineColor;
        __instance.trackOutlineWidth = trackOutlineWidth;
        __instance.thumbIcon = thumbIcon;
        __instance.dragStartBehavior = dragStartBehavior;
        __instance.mouseCursor = mouseCursor;
        __instance.focusColor = focusColor;
        __instance.hoverColor = hoverColor;
        __instance.overlayColor = overlayColor;
        __instance.splashRadius = splashRadius;
        __instance.focusNode = focusNode;
        __instance.onFocusChange = onFocusChange;
        __instance.autofocus = autofocus;
        __instance.padding = padding;
        __instance.applyCupertinoTheme = applyCupertinoTheme;
        __instance._switchType = _SwitchType__switch.adaptive;
        return __instance;
    }

    internal virtual global::Doroti.Ui.Size _getSwitchSize(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__21465 = Theme.of(context);
        SwitchThemeData switchTheme__21512 = SwitchTheme.of(context);
        SwitchThemeData defaults__21577 = (theme__21465.useMaterial3 ? new _SwitchDefaultsM3__switch(context) : new _SwitchDefaultsM2__switch(context));
        if ((object.Equals(this._switchType, _SwitchType__switch.adaptive)))
        {
            Adaptation<SwitchThemeData> switchAdaptation__21769 = (theme__21465.getAdaptation<SwitchThemeData>() ?? new _SwitchThemeAdaptation__switch());
            switchTheme__21512 = switchAdaptation__21769.adapt(theme__21465, switchTheme__21512);
        }
        _SwitchConfig__switch switchConfig__21966 = (theme__21465.useMaterial3 ? new _SwitchConfigM3__switch(context) : new _SwitchConfigM2__switch());
        MaterialTapTargetSize effectiveMaterialTapTargetSize__22097 = ((this.materialTapTargetSize ?? switchTheme__21512.materialTapTargetSize) ?? theme__21465.materialTapTargetSize);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry effectivePadding__22258 = ((this.padding ?? switchTheme__21512.padding) ?? defaults__21577.padding!);
        return (effectiveMaterialTapTargetSize__22097 switch { var __constant22389 when (object.Equals(__constant22389, MaterialTapTargetSize.padded)) => new global::Doroti.Ui.Size((((_SwitchConfig__switch)switchConfig__21966).switchWidth + ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)effectivePadding__22258).horizontal), (((_SwitchConfig__switch)switchConfig__21966).switchHeight + ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)effectivePadding__22258).vertical)), var __constant22569 when (object.Equals(__constant22569, MaterialTapTargetSize.shrinkWrap)) => new global::Doroti.Ui.Size((((_SwitchConfig__switch)switchConfig__21966).switchWidth + ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)effectivePadding__22258).horizontal), (((_SwitchConfig__switch)switchConfig__21966).switchHeightCollapsed + ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)effectivePadding__22258).vertical)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color? effectiveActiveThumbColor__22830 = default!;
        global::Doroti.Ui.Color? effectiveActiveTrackColor__22868 = default!;
        switch (this._switchType)
        {
            case _SwitchType__switch.material:
                {
                    effectiveActiveThumbColor__22830 = this.activeColor;
                    break;
                }
            case _SwitchType__switch.adaptive:
                {
                    switch (Theme.of(context).platform)
                    {
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                        case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                            {
                                effectiveActiveThumbColor__22830 = this.activeColor;
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                            {
                                effectiveActiveTrackColor__22868 = this.activeColor;
                                break;
                            }
                    }
                    break;
                }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _MaterialSwitch__switch(value: this.value, onChanged: (global::System.Action<bool>?)this.onChanged, size: _getSwitchSize(context), activeThumbColor: (this.activeThumbColor ?? effectiveActiveThumbColor__22830), activeTrackColor: (this.activeTrackColor ?? effectiveActiveTrackColor__22868), inactiveThumbColor: this.inactiveThumbColor, inactiveTrackColor: this.inactiveTrackColor, activeThumbImage: this.activeThumbImage, onActiveThumbImageError: (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)this.onActiveThumbImageError, inactiveThumbImage: this.inactiveThumbImage, onInactiveThumbImageError: (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)this.onInactiveThumbImageError, thumbColor: this.thumbColor, trackColor: this.trackColor, trackOutlineColor: this.trackOutlineColor, trackOutlineWidth: this.trackOutlineWidth, thumbIcon: this.thumbIcon, materialTapTargetSize: this.materialTapTargetSize, dragStartBehavior: this.dragStartBehavior, mouseCursor: this.mouseCursor, focusColor: this.focusColor, hoverColor: this.hoverColor, overlayColor: this.overlayColor, splashRadius: this.splashRadius, focusNode: this.focusNode, onFocusChange: (global::System.Action<bool>?)this.onFocusChange, autofocus: this.autofocus, applyCupertinoTheme: this.applyCupertinoTheme, switchType: this._switchType));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("value", value: this.value, ifTrue: "on", ifFalse: "off", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action<bool>>("onChanged", (global::System.Action<bool>?)this.onChanged, ifNull: "disabled"));
    }

}

public class _MaterialSwitch__switch : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool value { get; private set; } = default!;
    public virtual global::System.Action<bool>? onChanged { get; private set; }
    public virtual Color? activeThumbColor { get; private set; }
    public virtual Color? activeTrackColor { get; private set; }
    public virtual Color? inactiveThumbColor { get; private set; }
    public virtual Color? inactiveTrackColor { get; private set; }
    public virtual dynamic activeThumbImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError { get; private set; }
    public virtual dynamic inactiveThumbImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Size size { get; private set; } = default!;
    public virtual bool? applyCupertinoTheme { get; private set; }
    public virtual _SwitchType__switch switchType { get; private set; } = default!;

    internal _MaterialSwitch__switch(bool value, global::System.Action<bool>? onChanged, Size size, _SwitchType__switch switchType, Color? activeThumbColor = null, Color? activeTrackColor = null, Color? inactiveThumbColor = null, Color? inactiveTrackColor = null, dynamic activeThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError = null, dynamic inactiveThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Color? focusColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, bool? applyCupertinoTheme = null)
    {
        this.value = value;
        this.onChanged = onChanged;
        this.size = size;
        this.switchType = switchType;
        this.activeThumbColor = activeThumbColor;
        this.activeTrackColor = activeTrackColor;
        this.inactiveThumbColor = inactiveThumbColor;
        this.inactiveTrackColor = inactiveTrackColor;
        this.activeThumbImage = activeThumbImage;
        this.onActiveThumbImageError = onActiveThumbImageError;
        this.inactiveThumbImage = inactiveThumbImage;
        this.onInactiveThumbImageError = onInactiveThumbImageError;
        this.thumbColor = thumbColor;
        this.trackColor = trackColor;
        this.trackOutlineColor = trackOutlineColor;
        this.trackOutlineWidth = trackOutlineWidth;
        this.thumbIcon = thumbIcon;
        this.materialTapTargetSize = materialTapTargetSize;
        this.dragStartBehavior = dragStartBehavior;
        this.mouseCursor = mouseCursor;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.overlayColor = overlayColor;
        this.splashRadius = splashRadius;
        this.focusNode = focusNode;
        this.onFocusChange = onFocusChange;
        this.autofocus = autofocus;
        this.applyCupertinoTheme = applyCupertinoTheme;
        System.Diagnostics.Debug.Assert(((activeThumbImage is not null) || (onActiveThumbImageError is null)));
        System.Diagnostics.Debug.Assert(((inactiveThumbImage is not null) || (onInactiveThumbImageError is null)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MaterialSwitchState__switch());
}

internal class _MaterialSwitchState__switch : global::Doroti.Framework.Widgets.State<_MaterialSwitch__switch>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<_MaterialSwitch__switch>, global::Doroti.Framework.Widgets.ToggleableStateMixin<_MaterialSwitch__switch>
{
    internal virtual _SwitchPainter__switch _painter { get; private set; } = new _SwitchPainter__switch();
    internal virtual bool _needsPositionAnimation { get; set; } = false;
    public virtual bool isCupertino { get; set; } = false;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual global::Doroti.Framework.Animation.AnimationController _positionController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation _position { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController _reactionController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation _reaction { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation _reactionHoverFade { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController _reactionHoverFadeController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation _reactionFocusFade { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController _reactionFocusFadeController { get; set; } = default!;
    public virtual Duration _reactionAnimationDuration { get; set; } = Duration.Create(milliseconds: 100L);
    private bool __late__actionMap_initialized;
    private DartMap<Type, dynamic> __late__actionMap = default!;
    public virtual DartMap<Type, dynamic> _actionMap
    {
        get
        {
            if (!__late__actionMap_initialized)
            {
                __late__actionMap = new DartMap<Type, dynamic> { [typeof(ActivateIntent)] = new CallbackAction<ActivateIntent>(onInvoke: (__arg0) => { ((global::System.Action<Intent?>)this._handleTap)(__arg0); return default!; }) };
                __late__actionMap_initialized = true;
            }
            return __late__actionMap;
        }
    }
    public virtual Offset? _downPosition { get; set; } = default;
    public virtual bool _focused { get; set; } = false;
    public virtual bool _hovering { get; set; } = false;

    public override void didUpdateWidget(_MaterialSwitch__switch oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((_MaterialSwitch__switch)oldWidget).value != ((_MaterialSwitch__switch)this.widget).value))
        {
            if (((((global::Doroti.Framework.Animation.CurvedAnimation)this.position).value == 0.0) || (((global::Doroti.Framework.Animation.CurvedAnimation)this.position).value == 1.0)))
            {
                switch (((_MaterialSwitch__switch)this.widget).switchType)
                {
                    case _SwitchType__switch.adaptive:
                        {
                            switch (Theme.of(this.context).platform)
                            {
                                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                                    {
                                        updateCurve();
                                        break;
                                    }
                                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                                    {
                                        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.CurvedAnimation>)(() =>
{            var __cascade = this.position;
            __cascade.curve = global::Doroti.Framework.Animation.Curves.linear;
            __cascade.reverseCurve = global::Doroti.Framework.Animation.Curves.linear;
            return __cascade;        }))());
                                        break;
                                    }
                            }
                            break;
                        }
                    case _SwitchType__switch.material:
                        {
                            updateCurve();
                            break;
                        }
                }
            }
            animateToValue();
        }
    }

    public override void dispose()
    {
        this._painter.dispose();
        this._positionController.dispose();
        this._position.dispose();
        this._reactionController.dispose();
        this._reaction.dispose();
        this._reactionHoverFadeController.dispose();
        this._reactionHoverFade.dispose();
        this._reactionFocusFadeController.dispose();
        this._reactionFocusFade.dispose();
        base.dispose();
    }

    public virtual global::System.Action<bool?>? onChanged => ((global::System.Action<bool?>)((((_MaterialSwitch__switch)this.widget).onChanged is not null) ? this._handleChanged : null));
    public virtual bool tristate => false;
    public virtual bool? value => ((_MaterialSwitch__switch)this.widget).value;
    public virtual Duration? reactionAnimationDuration => ConstantsLibrary.kRadialReactionDuration;
    public virtual void updateCurve()
    {
        if (Theme.of(this.context).useMaterial3)
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.CurvedAnimation>)(() =>
{            var __cascade = this.position;
            __cascade.curve = global::Doroti.Framework.Animation.Curves.easeOutBack;
            __cascade.reverseCurve = global::Doroti.Framework.Animation.Curves.easeOutBack.flipped;
            return __cascade;        }))());
        }
        else
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.CurvedAnimation>)(() =>
{            var __cascade = this.position;
            __cascade.curve = global::Doroti.Framework.Animation.Curves.easeIn;
            __cascade.reverseCurve = global::Doroti.Framework.Animation.Curves.easeOut;
            return __cascade;        }))());
        }
    }

    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> _widgetThumbColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color?>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, Color?>)((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
{
    return ((_MaterialSwitch__switch)this.widget).inactiveThumbColor;
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
    return ((_MaterialSwitch__switch)this.widget).activeThumbColor;
}
return ((_MaterialSwitch__switch)this.widget).inactiveThumbColor;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> _widgetTrackColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color?>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, Color?>)((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
    return ((_MaterialSwitch__switch)this.widget).activeTrackColor;
}
return ((_MaterialSwitch__switch)this.widget).inactiveTrackColor;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            return default!;
        }
    }
    internal virtual double _trackInnerLength
    {
        get
        {
            switch (((_MaterialSwitch__switch)this.widget).switchType)
            {
                case _SwitchType__switch.adaptive:
                    {
                        switch (Theme.of(this.context).platform)
                        {
                            case global::Doroti.Framework.Foundation.TargetPlatform.android:
                            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                                {
                                    _SwitchConfig__switch config__29910 = (Theme.of(this.context).useMaterial3 ? new _SwitchConfigM3__switch(this.context) : new _SwitchConfigM2__switch());
                                    double trackInnerStart__30055 = (((_SwitchConfig__switch)config__29910).trackHeight / 2.0);
                                    double trackInnerEnd__30124 = (((_SwitchConfig__switch)config__29910).trackWidth - trackInnerStart__30055);
                                    double trackInnerLength__30202 = (trackInnerEnd__30124 - trackInnerStart__30055);
                                    return trackInnerLength__30202;
                                }
                            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                                {
                                    _SwitchConfig__switch config__30395 = ((_SwitchConfig__switch)(object?)new _SwitchConfigCupertino__switch(this.context));
                                    double trackInnerStart__30462 = (((_SwitchConfig__switch)config__30395).trackHeight / 2.0);
                                    double trackInnerEnd__30531 = (((_SwitchConfig__switch)config__30395).trackWidth - trackInnerStart__30462);
                                    double trackInnerLength__30609 = (trackInnerEnd__30531 - trackInnerStart__30462);
                                    return trackInnerLength__30609;
                                }
                            default:
                                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                        }
                        break;
                    }
                case _SwitchType__switch.material:
                    {
                        _SwitchConfig__switch config__30769 = (Theme.of(this.context).useMaterial3 ? new _SwitchConfigM3__switch(this.context) : new _SwitchConfigM2__switch());
                        double trackInnerStart__30902 = (((_SwitchConfig__switch)config__30769).trackHeight / 2.0);
                        double trackInnerEnd__30967 = (((_SwitchConfig__switch)config__30769).trackWidth - trackInnerStart__30902);
                        double trackInnerLength__31041 = (trackInnerEnd__30967 - trackInnerStart__30902);
                        return trackInnerLength__31041;
                    }
            }
            return default!;
        }
    }
    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        if (this.isInteractive)
        {
            this.reactionController.forward();
        }
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (this.isInteractive)
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.CurvedAnimation>)(() =>
{            var __cascade = this.position;
            __cascade.curve = global::Doroti.Framework.Animation.Curves.linear;
            __cascade.reverseCurve = null;
            return __cascade;        }))());
            double delta__31437 = (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).primaryDelta) / this._trackInnerLength);
            this.positionController.value += (Directionality.of(this.context) switch { TextDirection.rtl => -delta__31437, TextDirection.ltr => delta__31437, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        if (((((global::Doroti.Framework.Animation.CurvedAnimation)this.position).value >= 0.5) != ((_MaterialSwitch__switch)this.widget).value))
        {
            ((_MaterialSwitch__switch)this.widget).onChanged?.Invoke(!((_MaterialSwitch__switch)this.widget).value);
            setState(((global::System.Action)(() => {
_needsPositionAnimation = true;
})));
        }
        else
        {
            animateToValue();
        }
        this.reactionController.reverse();
    }

    internal virtual void _handleChanged(bool? value)
    {
        DartRuntimePrimitives.Assert(() => (value is not null));
        DartRuntimePrimitives.Assert(() => (((_MaterialSwitch__switch)this.widget).onChanged is not null));
        ((_MaterialSwitch__switch)this.widget).onChanged?.Invoke(DartRuntimePrimitives.RequireValue(value));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        if (this._needsPositionAnimation)
        {
            _needsPositionAnimation = false;
            animateToValue();
        }
        ThemeData theme__32525 = Theme.of(context);
        SwitchThemeData switchTheme__32572 = SwitchTheme.of(context);
        global::Doroti.Ui.Color cupertinoPrimaryColor__32627 = ((global::Doroti.Ui.Color)(object?)(theme__32525.cupertinoOverrideTheme?.primaryColor ?? theme__32525.colorScheme.primary));
        _SwitchConfig__switch switchConfig__32751 = default!;
        dynamic defaults__32785 = default!;
        var applyCupertinoTheme__32803 = false;
        double disabledOpacity__32843 = 1;
        switch (((_MaterialSwitch__switch)this.widget).switchType)
        {
            case _SwitchType__switch.material:
                {
                    switchConfig__32751 = (theme__32525.useMaterial3 ? new _SwitchConfigM3__switch(context) : new _SwitchConfigM2__switch());
                    defaults__32785 = (theme__32525.useMaterial3 ? new _SwitchDefaultsM3__switch(context) : new _SwitchDefaultsM2__switch(context));
                    break;
                }
            case _SwitchType__switch.adaptive:
                {
                    Adaptation<SwitchThemeData> switchAdaptation__33192 = (theme__32525.getAdaptation<SwitchThemeData>() ?? new _SwitchThemeAdaptation__switch());
                    switchTheme__32572 = switchAdaptation__33192.adapt(theme__32525, switchTheme__32572);
                    switch (theme__32525.platform)
                    {
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                        case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                            {
                                switchConfig__32751 = (theme__32525.useMaterial3 ? new _SwitchConfigM3__switch(context) : new _SwitchConfigM2__switch());
                                defaults__32785 = (theme__32525.useMaterial3 ? new _SwitchDefaultsM3__switch(context) : new _SwitchDefaultsM2__switch(context));
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                            {
                                isCupertino = true;
                                applyCupertinoTheme__32803 = ((((_MaterialSwitch__switch)this.widget).applyCupertinoTheme ?? theme__32525.cupertinoOverrideTheme?.applyThemeToAll) ?? false);
                                disabledOpacity__32843 = 0.5;
                                switchConfig__32751 = DartRuntimePrimitives.ConvertValue<_SwitchConfig__switch>(new _SwitchConfigCupertino__switch(context));
                                defaults__32785 = DartRuntimePrimitives.ConvertValue<SwitchThemeData>(new _SwitchDefaultsCupertino__switch(context));
                                this.reactionController.duration = Duration.Create(milliseconds: 200L);
                                break;
                            }
                    }
                    break;
                }
        }
        this.positionController.duration = Duration.Create(milliseconds: ((_SwitchConfig__switch)switchConfig__32751).toggleDuration);
        HashSet<global::Doroti.Framework.Widgets.WidgetState> activeStates__34504 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.selected);
            return __cascade;        }))();
        HashSet<global::Doroti.Framework.Widgets.WidgetState> inactiveStates__34581 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Remove(global::Doroti.Framework.Widgets.WidgetState.selected);
            return __cascade;        }))();
        global::Doroti.Ui.Color? activeThumbColor__34654 = ((global::Doroti.Ui.Color?)(object?)((((((_MaterialSwitch__switch)this.widget).thumbColor?.resolve(activeStates__34504) ?? (Color)this._widgetThumbColor.resolve(activeStates__34504))) ?? (Color)switchTheme__32572.thumbColor?.resolve(activeStates__34504))));
        global::Doroti.Ui.Color effectiveActiveThumbColor__34847 = ((global::Doroti.Ui.Color)(object?)(activeThumbColor__34654 ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults__32785.thumbColor).resolve(activeStates__34504)!));
        global::Doroti.Ui.Color? inactiveThumbColor__34965 = ((global::Doroti.Ui.Color?)(object?)((((((_MaterialSwitch__switch)this.widget).thumbColor?.resolve(inactiveStates__34581) ?? (Color)this._widgetThumbColor.resolve(inactiveStates__34581))) ?? (Color)switchTheme__32572.thumbColor?.resolve(inactiveStates__34581))));
        global::Doroti.Ui.Color effectiveInactiveThumbColor__35166 = ((global::Doroti.Ui.Color)(object?)(inactiveThumbColor__34965 ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults__32785.thumbColor).resolve(inactiveStates__34581)!));
        global::Doroti.Ui.Color effectiveActiveTrackColor__35289 = ((global::Doroti.Ui.Color)(object?)(((((((_MaterialSwitch__switch)this.widget).trackColor?.resolve(activeStates__34504) ?? (Color)this._widgetTrackColor.resolve(activeStates__34504))) ?? ((applyCupertinoTheme__32803 ? cupertinoPrimaryColor__32627 : switchTheme__32572.trackColor?.resolve(activeStates__34504)))) ?? this._widgetThumbColor.resolve(activeStates__34504)?.withAlpha(128L)) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults__32785.trackColor).resolve(activeStates__34504)!));
        global::Doroti.Ui.Color? effectiveActiveTrackOutlineColor__35687 = ((global::Doroti.Ui.Color?)(object?)((((((_MaterialSwitch__switch)this.widget).trackOutlineColor?.resolve(activeStates__34504) ?? (Color)switchTheme__32572.trackOutlineColor?.resolve(activeStates__34504))) ?? (Color)((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults__32785.trackOutlineColor).resolve(activeStates__34504))));
        double? effectiveActiveTrackOutlineWidth__35922 = ((((((_MaterialSwitch__switch)this.widget).trackOutlineWidth?.resolve(activeStates__34504) ?? switchTheme__32572.trackOutlineWidth?.resolve(activeStates__34504))) ?? (double)((global::Doroti.Framework.Widgets.WidgetStateProperty<double>)defaults__32785.trackOutlineWidth).resolve(activeStates__34504)));
        global::Doroti.Ui.Color effectiveInactiveTrackColor__36156 = ((global::Doroti.Ui.Color)(object?)(((((((_MaterialSwitch__switch)this.widget).trackColor?.resolve(inactiveStates__34581) ?? (Color)this._widgetTrackColor.resolve(inactiveStates__34581))) ?? (Color)switchTheme__32572.trackColor?.resolve(inactiveStates__34581))) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults__32785.trackColor).resolve(inactiveStates__34581)!));
        global::Doroti.Ui.Color? effectiveInactiveTrackOutlineColor__36424 = ((global::Doroti.Ui.Color?)(object?)((((((_MaterialSwitch__switch)this.widget).trackOutlineColor?.resolve(inactiveStates__34581) ?? (Color)switchTheme__32572.trackOutlineColor?.resolve(inactiveStates__34581))) ?? (Color)((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>)defaults__32785.trackOutlineColor).resolve(inactiveStates__34581))));
        double? effectiveInactiveTrackOutlineWidth__36667 = ((((((_MaterialSwitch__switch)this.widget).trackOutlineWidth?.resolve(inactiveStates__34581) ?? switchTheme__32572.trackOutlineWidth?.resolve(inactiveStates__34581))) ?? (double)((global::Doroti.Framework.Widgets.WidgetStateProperty<double>)defaults__32785.trackOutlineWidth).resolve(inactiveStates__34581)));
        global::Doroti.Framework.Widgets.Icon? effectiveActiveIcon__36909 = ((((_MaterialSwitch__switch)this.widget).thumbIcon?.resolve(activeStates__34504) ?? (global::Doroti.Framework.Widgets.Icon)switchTheme__32572.thumbIcon?.resolve(activeStates__34504)));
        global::Doroti.Framework.Widgets.Icon? effectiveInactiveIcon__37044 = ((((_MaterialSwitch__switch)this.widget).thumbIcon?.resolve(inactiveStates__34581) ?? (global::Doroti.Framework.Widgets.Icon)switchTheme__32572.thumbIcon?.resolve(inactiveStates__34581)));
        global::Doroti.Ui.Color effectiveActiveIconColor__37186 = ((global::Doroti.Ui.Color)(object?)((effectiveActiveIcon__36909?.color ?? (Color)((_SwitchConfig__switch)switchConfig__32751).iconColor.resolve(activeStates__34504))));
        global::Doroti.Ui.Color effectiveInactiveIconColor__37313 = ((global::Doroti.Ui.Color)(object?)((effectiveInactiveIcon__37044?.color ?? (Color)((_SwitchConfig__switch)switchConfig__32751).iconColor.resolve(inactiveStates__34581))));
        HashSet<global::Doroti.Framework.Widgets.WidgetState> focusedStates__37458 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.focused);
            return __cascade;        }))();
        global::Doroti.Ui.Color effectiveFocusOverlayColor__37524 = ((global::Doroti.Ui.Color)(object?)(((((((_MaterialSwitch__switch)this.widget).overlayColor?.resolve(focusedStates__37458) ?? ((_MaterialSwitch__switch)this.widget).focusColor) ?? (Color)switchTheme__32572.overlayColor?.resolve(focusedStates__37458))) ?? ((applyCupertinoTheme__32803 ? global::Doroti.Framework.Painting.HSLColor.CreateFromColor(cupertinoPrimaryColor__32627.withOpacity(0.8)).withLightness(0.69).withSaturation(0.835).toColor() : null))) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults__32785.overlayColor).resolve(focusedStates__37458)!));
        HashSet<global::Doroti.Framework.Widgets.WidgetState> hoveredStates__37992 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.hovered);
            return __cascade;        }))();
        global::Doroti.Ui.Color effectiveHoverOverlayColor__38058 = ((global::Doroti.Ui.Color)(object?)((((((_MaterialSwitch__switch)this.widget).overlayColor?.resolve(hoveredStates__37992) ?? ((_MaterialSwitch__switch)this.widget).hoverColor) ?? (Color)switchTheme__32572.overlayColor?.resolve(hoveredStates__37992))) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults__32785.overlayColor).resolve(hoveredStates__37992)!));
        var activePressedStates__38298 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = activeStates__34504;
            __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.pressed);
            return __cascade;        }))();
        global::Doroti.Ui.Color effectiveActivePressedThumbColor__38376 = ((global::Doroti.Ui.Color)(object?)(((((((_MaterialSwitch__switch)this.widget).thumbColor?.resolve(activePressedStates__38298) ?? (Color)this._widgetThumbColor.resolve(activePressedStates__38298))) ?? (Color)switchTheme__32572.thumbColor?.resolve(activePressedStates__38298))) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults__32785.thumbColor).resolve(activePressedStates__38298)!));
        global::Doroti.Ui.Color effectiveActivePressedOverlayColor__38668 = ((global::Doroti.Ui.Color)(object?)((((((_MaterialSwitch__switch)this.widget).overlayColor?.resolve(activePressedStates__38298) ?? (Color)switchTheme__32572.overlayColor?.resolve(activePressedStates__38298))) ?? activeThumbColor__34654?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults__32785.overlayColor).resolve(activePressedStates__38298)!));
        var inactivePressedStates__38966 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = inactiveStates__34581;
            __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.pressed);
            return __cascade;        }))();
        global::Doroti.Ui.Color effectiveInactivePressedThumbColor__39048 = ((global::Doroti.Ui.Color)(object?)(((((((_MaterialSwitch__switch)this.widget).thumbColor?.resolve(inactivePressedStates__38966) ?? (Color)this._widgetThumbColor.resolve(inactivePressedStates__38966))) ?? (Color)switchTheme__32572.thumbColor?.resolve(inactivePressedStates__38966))) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults__32785.thumbColor).resolve(inactivePressedStates__38966)!));
        global::Doroti.Ui.Color effectiveInactivePressedOverlayColor__39350 = ((global::Doroti.Ui.Color)(object?)((((((_MaterialSwitch__switch)this.widget).overlayColor?.resolve(inactivePressedStates__38966) ?? (Color)switchTheme__32572.overlayColor?.resolve(inactivePressedStates__38966))) ?? inactiveThumbColor__34965?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults__32785.overlayColor).resolve(inactivePressedStates__38966)!));
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> effectiveMouseCursor__39691 = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>((states) => {
return WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(((_MaterialSwitch__switch)this.widget).mouseCursor, states)
    ?? switchTheme__32572.mouseCursor?.resolve(states)
    ?? global::Doroti.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable.resolve(states);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        double effectiveActiveThumbRadius__40029 = ((effectiveActiveIcon__36909 is null) ? ((_SwitchConfig__switch)switchConfig__32751).activeThumbRadius : ((_SwitchConfig__switch)switchConfig__32751).thumbRadiusWithIcon);
        double effectiveInactiveThumbRadius__40188 = (((effectiveInactiveIcon__37044 is null) && (((_MaterialSwitch__switch)this.widget).inactiveThumbImage is null)) ? ((_SwitchConfig__switch)switchConfig__32751).inactiveThumbRadius : ((_SwitchConfig__switch)switchConfig__32751).thumbRadiusWithIcon);
        double effectiveSplashRadius__40398 = ((((_MaterialSwitch__switch)this.widget).splashRadius ?? switchTheme__32572.splashRadius) ?? DartRuntimePrimitives.RequireValue(defaults__32785.splashRadius));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(toggled: ((_MaterialSwitch__switch)this.widget).value, child: new global::Doroti.Framework.Widgets.GestureDetector(excludeFromSemantics: true, onHorizontalDragStart: (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this._handleDragStart, onHorizontalDragUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handleDragUpdate, onHorizontalDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._handleDragEnd, dragStartBehavior: ((_MaterialSwitch__switch)this.widget).dragStartBehavior, child: new global::Doroti.Framework.Widgets.Opacity(opacity: ((this.onChanged is null) ? disabledOpacity__32843 : 1), child: buildToggleable(mouseCursor: effectiveMouseCursor__39691, focusNode: ((_MaterialSwitch__switch)this.widget).focusNode, onFocusChange: (global::System.Action<bool>?)((_MaterialSwitch__switch)this.widget).onFocusChange, autofocus: ((_MaterialSwitch__switch)this.widget).autofocus, size: ((_MaterialSwitch__switch)this.widget).size, painter: ((Func<_SwitchPainter__switch>)(() =>
{            var __cascade = this._painter;
            __cascade.position = this.position;
            __cascade.reaction = this.reaction;
            __cascade.reactionFocusFade = this.reactionFocusFade;
            __cascade.reactionHoverFade = this.reactionHoverFade;
            __cascade.inactiveReactionColor = effectiveInactivePressedOverlayColor__39350;
            __cascade.reactionColor = effectiveActivePressedOverlayColor__38668;
            __cascade.hoverColor = effectiveHoverOverlayColor__38058;
            __cascade.focusColor = effectiveFocusOverlayColor__37524;
            __cascade.splashRadius = effectiveSplashRadius__40398;
            __cascade.downPosition = this.downPosition;
            __cascade.isFocused = this.states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused);
            __cascade.isHovered = this.states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered);
            __cascade.activeColor = effectiveActiveThumbColor__34847;
            __cascade.inactiveColor = effectiveInactiveThumbColor__35166;
            __cascade.activePressedColor = effectiveActivePressedThumbColor__38376;
            __cascade.inactivePressedColor = effectiveInactivePressedThumbColor__39048;
            __cascade.activeThumbImage = ((_MaterialSwitch__switch)this.widget).activeThumbImage;
            __cascade.onActiveThumbImageError = ((_MaterialSwitch__switch)this.widget).onActiveThumbImageError;
            __cascade.inactiveThumbImage = ((_MaterialSwitch__switch)this.widget).inactiveThumbImage;
            __cascade.onInactiveThumbImageError = ((_MaterialSwitch__switch)this.widget).onInactiveThumbImageError;
            __cascade.activeTrackColor = effectiveActiveTrackColor__35289;
            __cascade.activeTrackOutlineColor = effectiveActiveTrackOutlineColor__35687;
            __cascade.activeTrackOutlineWidth = effectiveActiveTrackOutlineWidth__35922;
            __cascade.inactiveTrackColor = effectiveInactiveTrackColor__36156;
            __cascade.inactiveTrackOutlineColor = effectiveInactiveTrackOutlineColor__36424;
            __cascade.inactiveTrackOutlineWidth = effectiveInactiveTrackOutlineWidth__36667;
            __cascade.configuration = global::Doroti.Framework.Widgets.ImageLibrary.createLocalImageConfiguration(context);
            __cascade.isInteractive = this.isInteractive;
            __cascade.trackInnerLength = this._trackInnerLength;
            __cascade.textDirection = Directionality.of(context);
            __cascade.surfaceColor = theme__32525.colorScheme.surface;
            __cascade.inactiveThumbRadius = effectiveInactiveThumbRadius__40188;
            __cascade.activeThumbRadius = effectiveActiveThumbRadius__40029;
            __cascade.pressedThumbRadius = ((_SwitchConfig__switch)switchConfig__32751).pressedThumbRadius;
            __cascade.thumbOffset = ((_SwitchConfig__switch)switchConfig__32751).thumbOffset;
            __cascade.trackHeight = ((_SwitchConfig__switch)switchConfig__32751).trackHeight;
            __cascade.trackWidth = ((_SwitchConfig__switch)switchConfig__32751).trackWidth;
            __cascade.activeIconColor = effectiveActiveIconColor__37186;
            __cascade.inactiveIconColor = effectiveInactiveIconColor__37313;
            __cascade.activeIcon = effectiveActiveIcon__36909;
            __cascade.inactiveIcon = effectiveInactiveIcon__37044;
            __cascade.iconTheme = IconTheme.of(context);
            __cascade.thumbShadow = ((_SwitchConfig__switch)switchConfig__32751).thumbShadow;
            __cascade.transitionalThumbSize = ((_SwitchConfig__switch)switchConfig__32751).transitionalThumbSize;
            __cascade.positionController = this.positionController;
            __cascade.isCupertino = this.isCupertino;
            return __cascade;        }))())))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        var result__17553 = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

    public virtual global::Doroti.Framework.Animation.AnimationController positionController => this._positionController;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation position => this._position;
    public virtual global::Doroti.Framework.Animation.AnimationController reactionController => this._reactionController;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation reaction => this._reaction;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation reactionHoverFade => this._reactionHoverFade;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation reactionFocusFade => this._reactionFocusFade;
    public virtual bool isInteractive => DartRuntimePrimitives.ConvertValue<bool>((this.onChanged is not null));
    public override void initState()
    {
        base.initState();
        this._positionController = new global::Doroti.Framework.Animation.AnimationController(duration: global::Doroti.Framework.Widgets.ToggleableLibrary._kToggleDuration, value: ((this.value == false) ? 0.0 : 1.0), vsync: this);
        this._position = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._positionController, curve: global::Doroti.Framework.Animation.Curves.easeIn, reverseCurve: global::Doroti.Framework.Animation.Curves.easeOut);
        this._reactionController = new global::Doroti.Framework.Animation.AnimationController(duration: this._reactionAnimationDuration, vsync: this);
        this._reaction = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._reactionController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        this._reactionHoverFadeController = new global::Doroti.Framework.Animation.AnimationController(duration: global::Doroti.Framework.Widgets.ToggleableLibrary._kReactionFadeDuration, value: ((this._hovering || this._focused) ? 1.0 : 0.0), vsync: this);
        this._reactionHoverFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._reactionHoverFadeController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        this._reactionFocusFadeController = new global::Doroti.Framework.Animation.AnimationController(duration: global::Doroti.Framework.Widgets.ToggleableLibrary._kReactionFadeDuration, value: ((this._hovering || this._focused) ? 1.0 : 0.0), vsync: this);
        this._reactionFocusFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._reactionFocusFadeController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
    }

    public virtual void animateToValue()
    {
        if (this.tristate)
        {
            if ((this.value is null))
            {
                this._positionController.value = 0.0;
            }
            if ((this.value ?? true))
            {
                this._positionController.forward();
            }
            else
            {
                this._positionController.reverse();
            }
        }
        else
        {
            if ((this.value ?? false))
            {
                this._positionController.forward();
            }
            else
            {
                this._positionController.reverse();
            }
        }
    }

    public virtual Offset? downPosition => this._downPosition;
    public virtual void _handleTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        if (this.isInteractive)
        {
            setState(((global::System.Action)(() => {
this._downPosition = ((global::Doroti.Framework.Gestures.TapDownDetails)details).localPosition;
})));
            this._reactionController.forward();
        }
    }

    public virtual void _handleTap(Intent? __unused0 = null)
    {
        if (!this.isInteractive)
        {
            return;
        }
        switch (this.value)
        {
            case false:
                {
                    this.onChanged!(true);
                    break;
                }
            case true:
                {
                    this.onChanged!((this.tristate ? null : false));
                    break;
                }
            case null:
                {
                    this.onChanged!(false);
                    break;
                }
        }
        ((dynamic)this.context.findRenderObject()!).sendSemanticsEvent(new global::Doroti.Framework.Semantics.TapSemanticEvent());
    }

    public virtual void _handleTapEnd(global::Doroti.Framework.Gestures.TapUpDetails? __unused0 = null)
    {
        if ((this._downPosition is not null))
        {
            setState(((global::System.Action)(() => {
this._downPosition = null;
})));
        }
        this._reactionController.reverse();
    }

    public virtual void _handleFocusHighlightChanged(bool focused)
    {
        if ((focused != this._focused))
        {
            setState(((global::System.Action)(() => {
this._focused = focused;
})));
            if (focused)
            {
                this._reactionFocusFadeController.forward();
            }
            else
            {
                this._reactionFocusFadeController.reverse();
            }
        }
    }

    public virtual void _handleHoverChanged(bool hovering)
    {
        if ((hovering != this._hovering))
        {
            setState(((global::System.Action)(() => {
this._hovering = hovering;
})));
            if (hovering)
            {
                this._reactionHoverFadeController.forward();
            }
            else
            {
                this._reactionHoverFadeController.reverse();
            }
        }
    }

    public virtual HashSet<WidgetState> states => ((Func<HashSet<WidgetState>>)(() => { var __collection10795 = new HashSet<WidgetState>(); if (!this.isInteractive) { __collection10795.Add(WidgetState.disabled); } if (this._hovering) { __collection10795.Add(WidgetState.hovered); } if (this._focused) { __collection10795.Add(WidgetState.focused); } if ((this.value ?? true)) { __collection10795.Add(WidgetState.selected); } return __collection10795; }))();
    public virtual Widget buildToggleable(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor = null, Size size = default!, global::Doroti.Framework.Widgets.ToggleablePainter painter = default!)
    {
        return ((Widget)(object?)buildToggleableWithChild(focusNode: focusNode, onFocusChange: (global::System.Action<bool>?)onFocusChange, autofocus: autofocus, mouseCursor: mouseCursor, child: new CustomPaint(size: size, painter: painter)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildToggleableWithChild(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor = null, Widget child = default!)
    {
        return ((Widget)(object?)new FocusableActionDetector(actions: this._actionMap, focusNode: focusNode, autofocus: autofocus, onFocusChange: (global::System.Action<bool>?)onFocusChange, enabled: this.isInteractive, onShowFocusHighlight: (global::System.Action<bool>)this._handleFocusHighlightChanged, onShowHoverHighlight: (global::System.Action<bool>)this._handleHoverChanged, mouseCursor: (mouseCursor?.resolve(this.states) ?? global::Doroti.Framework.Services.SystemMouseCursors.basic), child: new GestureDetector(excludeFromSemantics: !this.isInteractive, onTapDown: ((global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>)(this.isInteractive ? this._handleTapDown : null)), onTap: () => ((global::System.Action<Intent?>)(this.isInteractive ? this._handleTap : null))(default), onTapUp: ((global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails?>)(this.isInteractive ? this._handleTapEnd : null)), onTapCancel: () => ((global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails?>)(this.isInteractive ? this._handleTapEnd : null))(default), child: new global::Doroti.Framework.Widgets.Semantics(enabled: this.isInteractive, child: child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SwitchPainter__switch : global::Doroti.Framework.Widgets.ToggleablePainter
{
    internal virtual global::Doroti.Framework.Animation.AnimationController? _positionController { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _colorAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.Icon? _activeIcon { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.Icon? _inactiveIcon { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.IconThemeData? _iconTheme { get; set; } = default;
    internal virtual Color? _activeIconColor { get; set; } = default;
    internal virtual Color? _inactiveIconColor { get; set; } = default;
    internal virtual Color? _activePressedColor { get; set; } = default;
    internal virtual Color? _inactivePressedColor { get; set; } = default;
    internal virtual double? _activeThumbRadius { get; set; } = default;
    internal virtual double? _inactiveThumbRadius { get; set; } = default;
    internal virtual double? _pressedThumbRadius { get; set; } = default;
    internal virtual double? _thumbOffset { get; set; } = default;
    internal virtual Size? _transitionalThumbSize { get; set; } = default;
    internal virtual double? _trackHeight { get; set; } = default;
    internal virtual double? _trackWidth { get; set; } = default;
    internal virtual dynamic _activeThumbImage { get; set; } = default!;
    internal virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? _onActiveThumbImageError { get; set; } = default;
    internal virtual dynamic _inactiveThumbImage { get; set; } = default!;
    internal virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? _onInactiveThumbImageError { get; set; } = default;
    internal virtual Color? _activeTrackColor { get; set; } = default;
    internal virtual Color? _activeTrackOutlineColor { get; set; } = default;
    internal virtual Color? _inactiveTrackOutlineColor { get; set; } = default;
    internal virtual double? _activeTrackOutlineWidth { get; set; } = default;
    internal virtual double? _inactiveTrackOutlineWidth { get; set; } = default;
    internal virtual Color? _inactiveTrackColor { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.ImageConfiguration? _configuration { get; set; } = default;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual Color? _surfaceColor { get; set; } = default;
    internal virtual bool? _isInteractive { get; set; } = default;
    internal virtual double? _trackInnerLength { get; set; } = default;
    internal virtual bool? _isCupertino { get; set; } = default;
    internal virtual List<global::Doroti.Framework.Painting.BoxShadow>? _thumbShadow { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.TextPainter _textPainter { get; private set; } = new global::Doroti.Framework.Painting.TextPainter();
    internal virtual Color? _cachedThumbColor { get; set; } = default;
    internal virtual dynamic _cachedThumbImage { get; set; } = default!;
    internal virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? _cachedThumbErrorListener { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.BoxPainter? _cachedThumbPainter { get; set; } = default;
    internal virtual bool _isPainting { get; set; } = false;
    internal virtual bool _stopPressAnimation { get; set; } = false;
    internal virtual double? _pressedInactiveThumbRadius { get; set; } = default;
    internal virtual double? _pressedActiveThumbRadius { get; set; } = default;
    internal virtual double? _pressedThumbExtension { get; set; } = default;

    public virtual global::Doroti.Framework.Animation.AnimationController positionController
    {
        get => this._positionController!;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._positionController)))
            {
                return;
            }
            _positionController = __value;
            this._colorAnimation?.dispose();
            _colorAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this.positionController, curve: global::Doroti.Framework.Animation.Curves.easeOut, reverseCurve: global::Doroti.Framework.Animation.Curves.easeIn);
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Widgets.Icon? activeIcon
    {
        get => this._activeIcon;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._activeIcon)))
            {
                return;
            }
            _activeIcon = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Widgets.Icon? inactiveIcon
    {
        get => this._inactiveIcon;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._inactiveIcon)))
            {
                return;
            }
            _inactiveIcon = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? iconTheme
    {
        get => this._iconTheme;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._iconTheme)))
            {
                return;
            }
            _iconTheme = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activeIconColor
    {
        get => this._activeIconColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._activeIconColor)))
            {
                return;
            }
            _activeIconColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactiveIconColor
    {
        get => this._inactiveIconColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._inactiveIconColor)))
            {
                return;
            }
            _inactiveIconColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activePressedColor
    {
        get => this._activePressedColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._activePressedColor)))
            {
                return;
            }
            _activePressedColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactivePressedColor
    {
        get => this._inactivePressedColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._inactivePressedColor)))
            {
                return;
            }
            _inactivePressedColor = __value;
            notifyListeners();
        }
    }
    public virtual double activeThumbRadius
    {
        get => DartRuntimePrimitives.RequireValue(this._activeThumbRadius);
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._activeThumbRadius))
            {
                return;
            }
            _activeThumbRadius = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double inactiveThumbRadius
    {
        get => DartRuntimePrimitives.RequireValue(this._inactiveThumbRadius);
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._inactiveThumbRadius))
            {
                return;
            }
            _inactiveThumbRadius = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double pressedThumbRadius
    {
        get => DartRuntimePrimitives.RequireValue(this._pressedThumbRadius);
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._pressedThumbRadius))
            {
                return;
            }
            _pressedThumbRadius = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double? thumbOffset
    {
        get => this._thumbOffset;
        set
        {
            var __value = value;
            if ((__value == this._thumbOffset))
            {
                return;
            }
            _thumbOffset = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Size transitionalThumbSize
    {
        get => DartRuntimePrimitives.RequireValue(this._transitionalThumbSize);
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this._transitionalThumbSize)))
            {
                return;
            }
            _transitionalThumbSize = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double trackHeight
    {
        get => DartRuntimePrimitives.RequireValue(this._trackHeight);
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._trackHeight))
            {
                return;
            }
            _trackHeight = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double trackWidth
    {
        get => DartRuntimePrimitives.RequireValue(this._trackWidth);
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._trackWidth))
            {
                return;
            }
            _trackWidth = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual dynamic activeThumbImage
    {
        get => this._activeThumbImage;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._activeThumbImage)))
            {
                return;
            }
            _activeThumbImage = __value;
            notifyListeners();
        }
    }
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError
    {
        get => this._onActiveThumbImageError;
        set
        {
            var __value = value;
            if ((object.Equals((global::System.Action<object, global::System.Diagnostics.StackTrace?>?)__value, (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)this._onActiveThumbImageError)))
            {
                return;
            }
            _onActiveThumbImageError = (global::System.Action<object, global::System.Diagnostics.StackTrace?>)__value;
            notifyListeners();
        }
    }
    public virtual dynamic inactiveThumbImage
    {
        get => this._inactiveThumbImage;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._inactiveThumbImage)))
            {
                return;
            }
            _inactiveThumbImage = __value;
            notifyListeners();
        }
    }
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError
    {
        get => this._onInactiveThumbImageError;
        set
        {
            var __value = value;
            if ((object.Equals((global::System.Action<object, global::System.Diagnostics.StackTrace?>?)__value, (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)this._onInactiveThumbImageError)))
            {
                return;
            }
            _onInactiveThumbImageError = (global::System.Action<object, global::System.Diagnostics.StackTrace?>)__value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activeTrackColor
    {
        get => this._activeTrackColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._activeTrackColor)))
            {
                return;
            }
            _activeTrackColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color? activeTrackOutlineColor
    {
        get => this._activeTrackOutlineColor;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            if ((object.Equals(__value, this._activeTrackOutlineColor)))
            {
                return;
            }
            _activeTrackOutlineColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color? inactiveTrackOutlineColor
    {
        get => this._inactiveTrackOutlineColor;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            if ((object.Equals(__value, this._inactiveTrackOutlineColor)))
            {
                return;
            }
            _inactiveTrackOutlineColor = __value;
            notifyListeners();
        }
    }
    public virtual double? activeTrackOutlineWidth
    {
        get => this._activeTrackOutlineWidth;
        set
        {
            var __value = value;
            if ((__value == this._activeTrackOutlineWidth))
            {
                return;
            }
            _activeTrackOutlineWidth = __value;
            notifyListeners();
        }
    }
    public virtual double? inactiveTrackOutlineWidth
    {
        get => this._inactiveTrackOutlineWidth;
        set
        {
            var __value = value;
            if ((__value == this._inactiveTrackOutlineWidth))
            {
                return;
            }
            _inactiveTrackOutlineWidth = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactiveTrackColor
    {
        get => this._inactiveTrackColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._inactiveTrackColor)))
            {
                return;
            }
            _inactiveTrackColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.ImageConfiguration configuration
    {
        get => this._configuration!;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._configuration)))
            {
                return;
            }
            _configuration = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => DartRuntimePrimitives.RequireValue(this._textDirection);
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color surfaceColor
    {
        get => this._surfaceColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._surfaceColor)))
            {
                return;
            }
            _surfaceColor = __value;
            notifyListeners();
        }
    }
    public virtual bool isInteractive
    {
        get => DartRuntimePrimitives.RequireValue(this._isInteractive);
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._isInteractive))
            {
                return;
            }
            _isInteractive = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double trackInnerLength
    {
        get => DartRuntimePrimitives.RequireValue(this._trackInnerLength);
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._trackInnerLength))
            {
                return;
            }
            _trackInnerLength = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual bool isCupertino
    {
        get => DartRuntimePrimitives.RequireValue(this._isCupertino);
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._isCupertino))
            {
                return;
            }
            _isCupertino = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual List<global::Doroti.Framework.Painting.BoxShadow>? thumbShadow
    {
        get => this._thumbShadow;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._thumbShadow)))
            {
                return;
            }
            _thumbShadow = __value;
            notifyListeners();
        }
    }
    internal virtual global::Doroti.Framework.Painting.ShapeDecoration _createDefaultThumbDecoration(Color color, dynamic image, global::System.Action<object, global::System.Diagnostics.StackTrace?>? errorListener)
    {
        return new global::Doroti.Framework.Painting.ShapeDecoration(color: color, image: ((image is null) ? null : new global::Doroti.Framework.Painting.DecorationImage(image: image, onError: (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)errorListener)), shape: new global::Doroti.Framework.Painting.StadiumBorder(), shadows: (this.isCupertino ? null : this.thumbShadow));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleDecorationChanged()
    {
        if (!this._isPainting)
        {
            notifyListeners();
        }
    }

    public virtual void paint(Canvas canvas, Size size)
    {
        double currentValue__53365 = ((global::Doroti.Framework.Animation.Animation<double>)this.position).value;
        double visualPosition__53414 = (this.textDirection switch { TextDirection.rtl => (1.0 - currentValue__53365), TextDirection.ltr => currentValue__53365, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if (((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.reaction).status, global::Doroti.Framework.Animation.AnimationStatus.reverse)) && !this._stopPressAnimation))
        {
            _stopPressAnimation = true;
        }
        else
        {
            _stopPressAnimation = false;
        }
        if (!this._stopPressAnimation)
        {
            _pressedThumbExtension = (this.isCupertino ? (((global::Doroti.Framework.Animation.Animation<double>)this.reaction).value * 7L) : 0);
            if (((global::Doroti.Framework.Animation.Animation<double>)this.reaction).isCompleted)
            {
                _pressedInactiveThumbRadius = Dart_uiLibrary.lerpDouble(this.inactiveThumbRadius, this.pressedThumbRadius, ((global::Doroti.Framework.Animation.Animation<double>)this.reaction).value);
                _pressedActiveThumbRadius = Dart_uiLibrary.lerpDouble(this.activeThumbRadius, this.pressedThumbRadius, ((global::Doroti.Framework.Animation.Animation<double>)this.reaction).value);
            }
            if ((currentValue__53365 == 0L))
            {
                _pressedInactiveThumbRadius = Dart_uiLibrary.lerpDouble(this.inactiveThumbRadius, this.pressedThumbRadius, ((global::Doroti.Framework.Animation.Animation<double>)this.reaction).value);
                _pressedActiveThumbRadius = this.activeThumbRadius;
            }
            if ((currentValue__53365 == 1L))
            {
                _pressedActiveThumbRadius = Dart_uiLibrary.lerpDouble(this.activeThumbRadius, this.pressedThumbRadius, ((global::Doroti.Framework.Animation.Animation<double>)this.reaction).value);
                _pressedInactiveThumbRadius = this.inactiveThumbRadius;
            }
        }
        var inactiveThumbSize__54885 = (this.isCupertino ? new global::Doroti.Ui.Size(((DartRuntimePrimitives.RequireValue(this._pressedInactiveThumbRadius) * 2L) + DartRuntimePrimitives.RequireValue(this._pressedThumbExtension)), (DartRuntimePrimitives.RequireValue(this._pressedInactiveThumbRadius) * 2L)) : new global::Doroti.Ui.Size(((this._pressedInactiveThumbRadius ?? (double)this.inactiveThumbRadius))));
        var activeThumbSize__55152 = (this.isCupertino ? new global::Doroti.Ui.Size(((DartRuntimePrimitives.RequireValue(this._pressedActiveThumbRadius) * 2L) + DartRuntimePrimitives.RequireValue(this._pressedThumbExtension)), (DartRuntimePrimitives.RequireValue(this._pressedActiveThumbRadius) * 2L)) : new global::Doroti.Ui.Size(((this._pressedActiveThumbRadius ?? (double)this.activeThumbRadius))));
        global::Doroti.Framework.Animation.Animation<Size> thumbSizeAnimation(bool isForward)
        {
            List<global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>> thumbSizeSequence__55492 = default!;
            if (isForward)
            {
                thumbSizeSequence__55492 = new List<global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>> { new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>(tween: new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Size>(begin: inactiveThumbSize__54885, end: this.transitionalThumbSize).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Cubic(0.31, 0.0, 0.56, 1.0))), weight: 11), new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>(tween: new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Size>(begin: this.transitionalThumbSize, end: activeThumbSize__55152).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Cubic(0.2, 0.0, 0.0, 1.0))), weight: 72), new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>(tween: new global::Doroti.Framework.Animation.ConstantTween<global::Doroti.Ui.Size>(activeThumbSize__55152), weight: 17) };
            }
            else
            {
                thumbSizeSequence__55492 = new List<global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>> { new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>(tween: new global::Doroti.Framework.Animation.ConstantTween<global::Doroti.Ui.Size>(inactiveThumbSize__54885), weight: 17), new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>(tween: new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Size>(begin: inactiveThumbSize__54885, end: this.transitionalThumbSize).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Cubic(0.2, 0.0, 0.0, 1.0).flipped)), weight: 72), new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>(tween: new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Size>(begin: this.transitionalThumbSize, end: activeThumbSize__55152).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Cubic(0.31, 0.0, 0.56, 1.0).flipped)), weight: 11) };
            }
            return ((global::Doroti.Framework.Animation.Animation<Size>)(object?)new global::Doroti.Framework.Animation.TweenSequence<global::Doroti.Ui.Size>(thumbSizeSequence__55492.Cast<global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>>().ToList()).animate(this.positionController));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Ui.Size? thumbSize__57038 = default!;
        if (this.isCupertino)
        {
            if (((global::Doroti.Framework.Animation.Animation<double>)this.reaction).isCompleted)
            {
                thumbSize__57038 = new global::Doroti.Ui.Size(((DartRuntimePrimitives.RequireValue(this._pressedInactiveThumbRadius) * 2L) + DartRuntimePrimitives.RequireValue(this._pressedThumbExtension)), (DartRuntimePrimitives.RequireValue(this._pressedInactiveThumbRadius) * 2L));
            }
            else
            {
                if ((((global::Doroti.Framework.Animation.Animation<double>)this.position).isDismissed || (object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.position).status, global::Doroti.Framework.Animation.AnimationStatus.forward))))
                {
                    thumbSize__57038 = Dart_uiLibrary.Size.lerp(inactiveThumbSize__54885, activeThumbSize__55152, ((global::Doroti.Framework.Animation.Animation<double>)this.position).value);
                }
                else
                {
                    thumbSize__57038 = Dart_uiLibrary.Size.lerp(inactiveThumbSize__54885, activeThumbSize__55152, ((global::Doroti.Framework.Animation.Animation<double>)this.position).value);
                }
            }
        }
        else
        {
            if (((global::Doroti.Framework.Animation.Animation<double>)this.reaction).isCompleted)
            {
                thumbSize__57038 = new global::Doroti.Ui.Size(this.pressedThumbRadius);
            }
            else
            {
                if ((((global::Doroti.Framework.Animation.Animation<double>)this.position).isDismissed || (object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.position).status, global::Doroti.Framework.Animation.AnimationStatus.forward))))
                {
                    thumbSize__57038 = thumbSizeAnimation(true).value;
                }
                else
                {
                    thumbSize__57038 = thumbSizeAnimation(false).value;
                }
            }
        }
        double inset__58000 = ((this.thumbOffset is null) ? 0 : (1.0 - (((currentValue__53365 - DartRuntimePrimitives.RequireValue(this.thumbOffset))).abs() * 2.0)));
        thumbSize__57038 = new global::Doroti.Ui.Size((DartRuntimePrimitives.RequireValue(thumbSize__57038).width - inset__58000), (DartRuntimePrimitives.RequireValue(thumbSize__57038).height - inset__58000));
        double colorValue__58175 = this._colorAnimation!.value;
        global::Doroti.Ui.Color trackColor__58228 = ((global::Doroti.Ui.Color)(object?)Dart_uiLibrary.Color.lerp(this.inactiveTrackColor, this.activeTrackColor, colorValue__58175)!);
        global::Doroti.Ui.Color? trackOutlineColor__58321 = ((global::Doroti.Ui.Color?)(object?)(((this.inactiveTrackOutlineColor is null) || (this.activeTrackOutlineColor is null)) ? null : Dart_uiLibrary.Color.lerp(this.inactiveTrackOutlineColor, this.activeTrackOutlineColor, colorValue__58175)));
        double? trackOutlineWidth__58537 = Dart_uiLibrary.lerpDouble(this.inactiveTrackOutlineWidth, this.activeTrackOutlineWidth, colorValue__58175);
        global::Doroti.Ui.Color lerpedThumbColor__58668 = default!;
        if (!((global::Doroti.Framework.Animation.Animation<double>)this.reaction).isDismissed)
        {
            lerpedThumbColor__58668 = Dart_uiLibrary.Color.lerp(this.inactivePressedColor, this.activePressedColor, colorValue__58175)!;
        }
        else
        {
            if ((object.Equals(((global::Doroti.Framework.Animation.AnimationController)this.positionController).status, global::Doroti.Framework.Animation.AnimationStatus.forward)))
            {
                lerpedThumbColor__58668 = Dart_uiLibrary.Color.lerp(this.inactivePressedColor, this.activeColor, colorValue__58175)!;
            }
            else
            {
                if ((object.Equals(((global::Doroti.Framework.Animation.AnimationController)this.positionController).status, global::Doroti.Framework.Animation.AnimationStatus.reverse)))
                {
                    lerpedThumbColor__58668 = Dart_uiLibrary.Color.lerp(this.inactiveColor, this.activePressedColor, colorValue__58175)!;
                }
                else
                {
                    lerpedThumbColor__58668 = Dart_uiLibrary.Color.lerp(this.inactiveColor, this.activeColor, colorValue__58175)!;
                }
            }
        }
        global::Doroti.Ui.Color thumbColor__59420 = ((global::Doroti.Ui.Color)(object?)Dart_uiLibrary.Color.alphaBlend(lerpedThumbColor__58668, this.surfaceColor));
        global::Doroti.Framework.Widgets.Icon? thumbIcon__59500 = ((currentValue__53365 < 0.5) ? this.inactiveIcon : this.activeIcon);
        dynamic thumbImage__59586 = ((currentValue__53365 < 0.5) ? this.inactiveThumbImage : this.activeThumbImage);
        global::System.Action<object, global::System.Diagnostics.StackTrace?>? thumbErrorListener__59690 = ((global::System.Action<object, global::System.Diagnostics.StackTrace?>)((currentValue__53365 < 0.5) ? this.onInactiveThumbImageError : this.onActiveThumbImageError));
        var paint__59812 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = trackColor__58228;
            return __cascade;        }))();
        global::Doroti.Ui.Offset trackPaintOffset__59867 = ((global::Doroti.Ui.Offset)(object?)_computeTrackPaintOffset(size, this.trackWidth, this.trackHeight));
        global::Doroti.Ui.Offset thumbPaintOffset__59960 = ((global::Doroti.Ui.Offset)(object?)_computeThumbPaintOffset(trackPaintOffset__59867, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(thumbSize__57038)), visualPosition__53414));
        var radialReactionOrigin__60085 = new global::Doroti.Ui.Offset((thumbPaintOffset__59960.dx + (DartRuntimePrimitives.RequireValue(thumbSize__57038).height / 2L)), (size.height / 2L));
        _paintTrackWith(canvas, paint__59812, trackPaintOffset__59867, trackOutlineColor__58321, trackOutlineWidth__58537);
        paintRadialReaction(canvas: canvas, origin: radialReactionOrigin__60085);
        _paintThumbWith(thumbPaintOffset__59960, canvas, colorValue__58175, thumbColor__59420, thumbImage__59586, (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)thumbErrorListener__59690, thumbIcon__59500, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(thumbSize__57038)), inset__58000);
    }

    internal virtual global::Doroti.Ui.Offset _computeTrackPaintOffset(Size canvasSize, double trackWidth, double trackHeight)
    {
        double horizontalOffset__60726 = (((canvasSize.width - trackWidth)) / 2.0);
        double verticalOffset__60801 = (((canvasSize.height - trackHeight)) / 2.0);
        return new global::Doroti.Ui.Offset(horizontalOffset__60726, verticalOffset__60801);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _computeThumbPaintOffset(Offset trackPaintOffset, Size thumbSize, double visualPosition)
    {
        double trackRadius__61177 = (this.trackHeight / 2L);
        double additionalThumbRadius__61225 = ((thumbSize.height / 2L) - trackRadius__61177);
        double horizontalProgress__61303 = (visualPosition * ((this.trackInnerLength - DartRuntimePrimitives.RequireValue(this._pressedThumbExtension))));
        double thumbHorizontalOffset__61404 = ((((trackPaintOffset.dx + trackRadius__61177) + ((DartRuntimePrimitives.RequireValue(this._pressedThumbExtension) / 2L))) - (thumbSize.width / 2L)) + horizontalProgress__61303);
        double thumbVerticalOffset__61595 = (trackPaintOffset.dy - additionalThumbRadius__61225);
        return new global::Doroti.Ui.Offset(thumbHorizontalOffset__61404, thumbVerticalOffset__61595);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintTrackWith(Canvas canvas, Paint paint, Offset trackPaintOffset, Color? trackOutlineColor, double? trackOutlineWidth)
    {
        var trackRect__61896 = global::Doroti.Ui.Rect.fromLTWH(trackPaintOffset.dx, trackPaintOffset.dy, this.trackWidth, this.trackHeight);
        double trackRadius__62038 = (this.trackHeight / 2L);
        var trackRRect__62079 = global::Doroti.Ui.RRect.fromRectAndRadius(trackRect__61896, global::Doroti.Ui.Radius.circular(trackRadius__62038));
        canvas.drawRRect(trackRRect__62079, paint);
        if ((trackOutlineColor is not null))
        {
            var outlineTrackRect__62277 = global::Doroti.Ui.Rect.fromLTWH((trackPaintOffset.dx + 1L), (trackPaintOffset.dy + 1L), (this.trackWidth - 2L), (this.trackHeight - 2L));
            var outlineTrackRRect__62447 = global::Doroti.Ui.RRect.fromRectAndRadius(outlineTrackRect__62277, global::Doroti.Ui.Radius.circular(trackRadius__62038));
            var outlinePaint__62578 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.style = PaintingStyle.stroke;
            __cascade.strokeWidth = (trackOutlineWidth ?? 2.0);
            __cascade.color = trackOutlineColor;
            return __cascade;        }))();
            canvas.drawRRect(outlineTrackRRect__62447, outlinePaint__62578);
        }
        if (this.isCupertino)
        {
            if (this.isFocused)
            {
                global::Doroti.Ui.RRect focusedOutline__62857 = ((global::Doroti.Ui.RRect)(object?)trackRRect__62079.inflate(1.75));
                var focusedPaint__62914 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.style = PaintingStyle.stroke;
            __cascade.color = this.focusColor;
            __cascade.strokeWidth = SwitchLibrary._kCupertinoFocusTrackOutline;
            return __cascade;        }))();
                canvas.drawRRect(focusedOutline__62857, focusedPaint__62914);
            }
            canvas.clipRRect(trackRRect__62079);
        }
    }

    internal virtual void _paintThumbWith(Offset thumbPaintOffset, Canvas canvas, double currentValue, Color thumbColor, dynamic thumbImage, global::System.Action<object, global::System.Diagnostics.StackTrace?>? thumbErrorListener, global::Doroti.Framework.Widgets.Icon? thumbIcon, Size thumbSize, double inset)
    {
        try
        {
            _isPainting = true;
            if (((((this._cachedThumbPainter is null) || (!object.Equals(thumbColor, this._cachedThumbColor))) || (!object.Equals(thumbImage, this._cachedThumbImage))) || (!object.Equals((global::System.Action<object, global::System.Diagnostics.StackTrace?>?)thumbErrorListener, (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)this._cachedThumbErrorListener))))
            {
                _cachedThumbColor = thumbColor;
                _cachedThumbImage = thumbImage;
                _cachedThumbErrorListener = (global::System.Action<object, global::System.Diagnostics.StackTrace?>)thumbErrorListener;
                this._cachedThumbPainter?.dispose();
                _cachedThumbPainter = _createDefaultThumbDecoration(thumbColor, thumbImage, (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)thumbErrorListener).createBoxPainter((global::System.Action)(() => this._handleDecorationChanged()));
            }
            global::Doroti.Framework.Painting.BoxPainter thumbPainter__64059 = this._cachedThumbPainter!;
            if (this.isCupertino)
            {
                _paintCupertinoThumbShadowAndBorder(canvas, thumbPaintOffset, thumbSize);
            }
            thumbPainter__64059.paint(canvas, thumbPaintOffset, this.configuration.copyWith(size: thumbSize));
            if (((thumbIcon is not null) && (((global::Doroti.Framework.Widgets.Icon)thumbIcon).icon is not null)))
            {
                global::Doroti.Ui.Color iconColor__64384 = ((global::Doroti.Ui.Color)(object?)Dart_uiLibrary.Color.lerp(this.inactiveIconColor, this.activeIconColor, currentValue)!);
                double iconSize__64480 = (((global::Doroti.Framework.Widgets.Icon)thumbIcon).size ?? _SwitchConfigM3__switch.iconSize);
                global::Doroti.Framework.Widgets.IconData iconData__64558 = ((global::Doroti.Framework.Widgets.Icon)thumbIcon).icon!;
                double? iconWeight__64608 = (((global::Doroti.Framework.Widgets.Icon)thumbIcon).weight ?? this.iconTheme?.weight);
                double? iconFill__64682 = (((global::Doroti.Framework.Widgets.Icon)thumbIcon).fill ?? this.iconTheme?.fill);
                double? iconGrade__64750 = (((global::Doroti.Framework.Widgets.Icon)thumbIcon).grade ?? this.iconTheme?.grade);
                double? iconOpticalSize__64821 = (((global::Doroti.Framework.Widgets.Icon)thumbIcon).opticalSize ?? this.iconTheme?.opticalSize);
                List<global::Doroti.Ui.Shadow>? iconShadows__64916 = (((global::Doroti.Framework.Widgets.Icon)thumbIcon).shadows ?? this.iconTheme?.shadows).ToList();
                var textSpan__64986 = new global::Doroti.Framework.Painting.TextSpan(text: char.ConvertFromUtf32(checked((int)((global::Doroti.Framework.Widgets.IconData)iconData__64558).codePoint)), style: new global::Doroti.Framework.Painting.TextStyle(fontVariations: ((Func<List<global::Doroti.Ui.FontVariation>>)(() => { var __collection65120 = new List<global::Doroti.Ui.FontVariation>(); if ((iconFill__64682 is not null)) { __collection65120.Add(new global::Doroti.Ui.FontVariation("FILL", DartRuntimePrimitives.RequireValue(iconFill__64682))); } if ((iconWeight__64608 is not null)) { __collection65120.Add(new global::Doroti.Ui.FontVariation("wght", DartRuntimePrimitives.RequireValue(iconWeight__64608))); } if ((iconGrade__64750 is not null)) { __collection65120.Add(new global::Doroti.Ui.FontVariation("GRAD", DartRuntimePrimitives.RequireValue(iconGrade__64750))); } if ((iconOpticalSize__64821 is not null)) { __collection65120.Add(new global::Doroti.Ui.FontVariation("opsz", DartRuntimePrimitives.RequireValue(iconOpticalSize__64821))); } return __collection65120; }))(), color: iconColor__64384, fontSize: iconSize__64480, inherit: false, fontFamily: ((global::Doroti.Framework.Widgets.IconData)iconData__64558).fontFamily, package: ((global::Doroti.Framework.Widgets.IconData)iconData__64558).fontPackage, shadows: iconShadows__64916));
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{            var __cascade = this._textPainter;
            __cascade.textDirection = this.textDirection;
            __cascade.text = textSpan__64986;
            return __cascade;        }))());
                this._textPainter.layout();
                double additionalHorizontalOffset__65828 = (((thumbSize.width - iconSize__64480)) / 2L);
                double additionalVerticalOffset__65912 = (((thumbSize.height - iconSize__64480)) / 2L);
                global::Doroti.Ui.Offset offset__65995 = ((global::Doroti.Ui.Offset)(object?)(thumbPaintOffset + new global::Doroti.Ui.Offset(additionalHorizontalOffset__65828, additionalVerticalOffset__65912)));
                this._textPainter.paint(canvas, offset__65995);
            }
        }
        finally
        {
            _isPainting = false;
        }
    }

    internal virtual void _paintCupertinoThumbShadowAndBorder(Canvas canvas, Offset thumbPaintOffset, Size thumbSize)
    {
        var thumbBounds__66315 = global::Doroti.Ui.RRect.fromLTRBR(thumbPaintOffset.dx, thumbPaintOffset.dy, (thumbPaintOffset.dx + thumbSize.width), (thumbPaintOffset.dy + thumbSize.height), global::Doroti.Ui.Radius.circular((thumbSize.height / 2.0)));
        if ((this.thumbShadow is not null))
        {
            foreach (global::Doroti.Framework.Painting.BoxShadow shadow__66603 in this.thumbShadow!)
            {
                canvas.drawRRect(thumbBounds__66315.shift(shadow__66603.offset), shadow__66603.toPaint());
            }
        }
        canvas.drawRRect(thumbBounds__66315.inflate(0.5), ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = new global::Doroti.Ui.Color(167772160L);
            return __cascade;        }))());
    }

    public override void dispose()
    {
        this._textPainter.dispose();
        this._cachedThumbPainter?.dispose();
        _cachedThumbPainter = null;
        _cachedThumbColor = null;
        _cachedThumbImage = null;
        _cachedThumbErrorListener = null;
        this._colorAnimation?.dispose();
        base.dispose();
    }

}

internal class _SwitchThemeAdaptation__switch : Adaptation<SwitchThemeData>
{
    internal _SwitchThemeAdaptation__switch()
    {
    }

    public virtual SwitchThemeData adapt(ThemeData theme, SwitchThemeData defaultValue)
    {
        switch (theme.platform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return defaultValue;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    return new SwitchThemeData();
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal interface _SwitchConfig__switch
{
    public double trackHeight { get; }
    public double trackWidth { get; }
    public double switchWidth { get; }
    public double switchHeight { get; }
    public double switchHeightCollapsed { get; }
    public double activeThumbRadius { get; }
    public double inactiveThumbRadius { get; }
    public double pressedThumbRadius { get; }
    public double thumbRadiusWithIcon { get; }
    public List<global::Doroti.Framework.Painting.BoxShadow>? thumbShadow { get; }
    public global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> iconColor { get; }
    public double? thumbOffset { get; }
    public global::Doroti.Ui.Size transitionalThumbSize { get; }
    public long toggleDuration { get; }
    public global::Doroti.Ui.Size switchMinSize { get; }
}

internal class _SwitchDefaultsCupertino__switch : SwitchThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _SwitchDefaultsCupertino__switch(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?> mouseCursor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
{
    return (global::Doroti.Framework.Services.SystemMouseCursors.basic);
}
return ((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? global::Doroti.Framework.Services.SystemMouseCursors.click : global::Doroti.Framework.Services.SystemMouseCursors.basic));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> thumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.white));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> trackColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
    return (CupertinoDynamicColor.resolve(CupertinoColors.systemGreen, this.context));
}
return (CupertinoDynamicColor.resolve(CupertinoColors.secondarySystemFill, this.context));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> trackOutlineColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> overlayColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
{
    return (global::Doroti.Framework.Painting.HSLColor.CreateFromColor(CupertinoDynamicColor.resolve(CupertinoColors.systemGreen, this.context).withOpacity(0.8)).withLightness(0.69).withSaturation(0.835).toColor());
}
return (Colors.transparent);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual double splashRadius => 0.0;
}

public static partial class SwitchLibrary
{
    internal static double _kCupertinoFocusTrackOutline = 3.5;
}

internal class _SwitchConfigCupertino__switch : _SwitchConfig__switch
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;

    internal _SwitchConfigCupertino__switch(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
        this._colors = Theme.of(context).colorScheme;
    }

    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color> iconColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
return (this._colors.onPrimaryContainer);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual double activeThumbRadius => 14.0;
    public virtual double inactiveThumbRadius => 14.0;
    public virtual double pressedThumbRadius => 14.0;
    public virtual double switchHeight => DartRuntimePrimitives.ConvertValue<double>((this.switchMinSize.height + 8.0));
    public virtual double switchHeightCollapsed => this.switchMinSize.height;
    public virtual double switchWidth => 60.0;
    public virtual double thumbRadiusWithIcon => 14.0;
    public virtual List<global::Doroti.Framework.Painting.BoxShadow>? thumbShadow => new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(637534208L), offset: new global::Doroti.Ui.Offset(0, 3), blurRadius: 8.0), new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(251658240L), offset: new global::Doroti.Ui.Offset(0, 3), blurRadius: 1.0) };
    public virtual double trackHeight => 31.0;
    public virtual double trackWidth => 51.0;
    public virtual Size transitionalThumbSize => new global::Doroti.Ui.Size(28.0, 28.0);
    public virtual long toggleDuration => 140L;
    public virtual double? thumbOffset => DartRuntimePrimitives.ConvertValue<double>(null);
    public virtual Size switchMinSize => new global::Doroti.Ui.Size((global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension - 8.0));
}

internal class _SwitchConfigM2__switch : _SwitchConfig__switch
{

    internal _SwitchConfigM2__switch()
    {
    }

    public virtual double activeThumbRadius => 10.0;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color> iconColor => WidgetStateProperty.all<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual double inactiveThumbRadius => 10.0;
    public virtual double pressedThumbRadius => 10.0;
    public virtual double switchHeight => DartRuntimePrimitives.ConvertValue<double>((this.switchMinSize.height + 8.0));
    public virtual double switchHeightCollapsed => this.switchMinSize.height;
    public virtual double switchWidth => DartRuntimePrimitives.ConvertValue<double>(((this.trackWidth - (2L * ((this.trackHeight / 2.0)))) + this.switchMinSize.width));
    public virtual double thumbRadiusWithIcon => 10.0;
    public virtual List<global::Doroti.Framework.Painting.BoxShadow>? thumbShadow => ShadowsLibrary.kElevationToShadow.GetValueOrDefault(1L);
    public virtual double trackHeight => 14.0;
    public virtual double trackWidth => 33.0;
    public virtual double? thumbOffset => 0.5;
    public virtual Size transitionalThumbSize => new global::Doroti.Ui.Size(20, 20);
    public virtual long toggleDuration => 200L;
    public virtual Size switchMinSize => new global::Doroti.Ui.Size((global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension - 8.0));
}

internal class _SwitchDefaultsM2__switch : SwitchThemeData
{
    internal virtual ThemeData _theme { get; private set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;

    internal _SwitchDefaultsM2__switch(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this._theme = Theme.of(context);
        this._colors = Theme.of(context).colorScheme;
    }

    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> thumbColor
    {
        get
        {
            var isDark__72847 = (object.Equals(this._theme.brightness, Brightness.dark));
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
{
    return ((isDark__72847 ? Colors.grey.shade800 : Colors.grey.shade400));
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
    return (this._colors.secondary);
}
return ((isDark__72847 ? Colors.grey.shade400 : Colors.grey.shade50));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> trackColor
    {
        get
        {
            var isDark__73334 = (object.Equals(this._theme.brightness, Brightness.dark));
            var black32__73391 = new global::Doroti.Ui.Color(1375731712L);
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
{
    return ((isDark__73334 ? Colors.white10 : Colors.black12));
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
    global::Doroti.Ui.Color activeColor__73705 = ((global::Doroti.Ui.Color)(object?)this._colors.secondary);
    return (activeColor__73705.withAlpha(128L));
}
return ((isDark__73334 ? Colors.white30 : black32__73391));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? trackOutlineColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent));
    public virtual MaterialTapTargetSize materialTapTargetSize => this._theme.materialTapTargetSize;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> mouseCursor => WidgetStateProperty.resolveWith((states) => global::Doroti.Framework.Widgets.WidgetStateMouseCursor.clickable.resolve(states));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> overlayColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
{
    return (this.thumbColor.resolve(states).withAlpha(ConstantsLibrary.kRadialReactionAlpha));
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
{
    return (this._theme.hoverColor);
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
{
    return (this._theme.focusColor);
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual double splashRadius => ConstantsLibrary.kRadialReactionRadius;
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.zero);
}

internal class _SwitchDefaultsM3__switch : SwitchThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _SwitchDefaultsM3__switch(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> thumbColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
{
    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
    {
        return (this._colors.surface.withOpacity(1.0));
    }
    return (this._colors.onSurface.withOpacity(0.38));
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
    {
        return (this._colors.primaryContainer);
    }
    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
    {
        return (this._colors.primaryContainer);
    }
    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
    {
        return (this._colors.primaryContainer);
    }
    return (this._colors.onPrimary);
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.onSurfaceVariant);
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onSurfaceVariant);
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onSurfaceVariant);
}
return (this._colors.outline);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> trackColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
{
    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
    {
        return (this._colors.onSurface.withOpacity(0.12));
    }
    return (this._colors.surfaceContainerHighest.withOpacity(0.12));
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
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
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.surfaceContainerHighest);
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.surfaceContainerHighest);
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.surfaceContainerHighest);
}
return (this._colors.surfaceContainerHighest);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> trackOutlineColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
    return (Colors.transparent);
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.12));
}
return (this._colors.outline);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> overlayColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
    {
        return (this._colors.primary.withOpacity(0.1));
    }
    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
    {
        return (this._colors.primary.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
    {
        return (this._colors.primary.withOpacity(0.1));
    }
    return null;
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.onSurface.withOpacity(0.1));
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onSurface.withOpacity(0.08));
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onSurface.withOpacity(0.1));
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> mouseCursor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>)(object?)WidgetStateProperty.resolveWith((states) => global::Doroti.Framework.Widgets.WidgetStateMouseCursor.clickable.resolve(states)));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double> trackOutlineWidth => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double>(2.0));
    public virtual double splashRadius => DartRuntimePrimitives.ConvertValue<double>((40.0 / 2L));
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4));
}

internal class _SwitchConfigM3__switch : _SwitchConfig__switch
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;
    public const double iconSize = 16.0;

    internal _SwitchConfigM3__switch(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
        this._colors = Theme.of(context).colorScheme;
    }

    public virtual double activeThumbRadius => DartRuntimePrimitives.ConvertValue<double>((24.0 / 2L));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color> iconColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
{
    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
    {
        return (this._colors.onSurface.withOpacity(0.38));
    }
    return (this._colors.surfaceContainerHighest.withOpacity(0.38));
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
    {
        return (this._colors.onPrimaryContainer);
    }
    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
    {
        return (this._colors.onPrimaryContainer);
    }
    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
    {
        return (this._colors.onPrimaryContainer);
    }
    return (this._colors.onPrimaryContainer);
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.surfaceContainerHighest);
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.surfaceContainerHighest);
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.surfaceContainerHighest);
}
return (this._colors.surfaceContainerHighest);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual double inactiveThumbRadius => DartRuntimePrimitives.ConvertValue<double>((16.0 / 2L));
    public virtual double pressedThumbRadius => DartRuntimePrimitives.ConvertValue<double>((28.0 / 2L));
    public virtual double switchHeight => DartRuntimePrimitives.ConvertValue<double>((this.switchMinSize.height + 8.0));
    public virtual double switchHeightCollapsed => this.switchMinSize.height;
    public virtual double switchWidth => 52.0;
    public virtual double thumbRadiusWithIcon => DartRuntimePrimitives.ConvertValue<double>((24.0 / 2L));
    public virtual List<global::Doroti.Framework.Painting.BoxShadow>? thumbShadow => ShadowsLibrary.kElevationToShadow.GetValueOrDefault(0L);
    public virtual double trackHeight => 32.0;
    public virtual double trackWidth => 52.0;
    public virtual Size transitionalThumbSize => new global::Doroti.Ui.Size(34, 22);
    public virtual long toggleDuration => 300L;
    public virtual double? thumbOffset => DartRuntimePrimitives.ConvertValue<double>(null);
    public virtual Size switchMinSize => new global::Doroti.Ui.Size(global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension, (global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension - 8.0));
}
