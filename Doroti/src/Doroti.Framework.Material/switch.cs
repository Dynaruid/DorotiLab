// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/switch.dart

using Doroti.Runtime;
using Doroti.Ui;

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
    public virtual global::Doroti.Framework.Painting.IImageProvider? activeThumbImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError { get; private set; }
    public virtual global::Doroti.Framework.Painting.IImageProvider? inactiveThumbImage { get; private set; } = default!;
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

    public Switch(global::Doroti.Framework.Foundation.Key? key = null, bool value = default!, global::System.Action<bool>? onChanged = default!, Color? activeColor = null, Color? activeThumbColor = null, Color? activeTrackColor = null, Color? inactiveThumbColor = null, Color? inactiveTrackColor = null, global::Doroti.Framework.Painting.IImageProvider? activeThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError = null, global::Doroti.Framework.Painting.IImageProvider? inactiveThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Color? focusColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null) : base(key: key)
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
        _switchType = _SwitchType__switch.material;
        applyCupertinoTheme = false;
        System.Diagnostics.Debug.Assert((activeThumbImage is not null) || (onActiveThumbImageError is null));
        System.Diagnostics.Debug.Assert((inactiveThumbImage is not null) || (onInactiveThumbImageError is null));
    }

    public static Switch CreateAdaptive(global::Doroti.Framework.Foundation.Key? key = null, bool value = default!, global::System.Action<bool>? onChanged = default!, Color? activeColor = null, Color? activeThumbColor = null, Color? activeTrackColor = null, Color? inactiveThumbColor = null, Color? inactiveTrackColor = null, global::Doroti.Framework.Painting.IImageProvider? activeThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError = null, global::Doroti.Framework.Painting.IImageProvider? inactiveThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Color? focusColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, bool? applyCupertinoTheme = null)
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
        ThemeData theme = Theme.of(context);
        SwitchThemeData switchTheme = SwitchTheme.of(context);
        SwitchThemeData defaults = new _SwitchDefaultsM3__switch(context);
        if (Equals(_switchType, _SwitchType__switch.adaptive))
        {
            Adaptation<SwitchThemeData> switchAdaptation = theme.getAdaptation<SwitchThemeData>() ?? new _SwitchThemeAdaptation__switch();
            switchTheme = switchAdaptation.adapt(theme, switchTheme);
        }
        _SwitchConfig__switch switchConfig = new _SwitchConfigM3__switch(context);
        MaterialTapTargetSize effectiveMaterialTapTargetSize = (materialTapTargetSize ?? switchTheme.materialTapTargetSize) ?? theme.materialTapTargetSize;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry effectivePadding = (padding ?? switchTheme.padding) ?? defaults.padding!;
        return effectiveMaterialTapTargetSize switch { var __constant22389 when Equals(__constant22389, MaterialTapTargetSize.padded) => new global::Doroti.Ui.Size(switchConfig.switchWidth + effectivePadding.horizontal, switchConfig.switchHeight + effectivePadding.vertical), var __constant22569 when Equals(__constant22569, MaterialTapTargetSize.shrinkWrap) => new global::Doroti.Ui.Size(switchConfig.switchWidth + effectivePadding.horizontal, switchConfig.switchHeightCollapsed + effectivePadding.vertical), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color? effectiveActiveThumbColor = default!;
        global::Doroti.Ui.Color? effectiveActiveTrackColor = default!;
        switch (_switchType)
        {
            case _SwitchType__switch.material:
                {
                    effectiveActiveThumbColor = activeColor;
                    break;
                }
            case _SwitchType__switch.adaptive:
                {
                    switch (Theme.of(context).platform)
                    {
                        case TargetPlatform.android:
                        case TargetPlatform.fuchsia:
                        case TargetPlatform.linux:
                        case TargetPlatform.windows:
                            {
                                effectiveActiveThumbColor = activeColor;
                                break;
                            }
                        case TargetPlatform.iOS:
                        case TargetPlatform.macOS:
                            {
                                effectiveActiveTrackColor = activeColor;
                                break;
                            }
                    }
                    break;
                }
        }
        return new _MaterialSwitch__switch(value: value, onChanged: onChanged, size: _getSwitchSize(context), activeThumbColor: activeThumbColor ?? effectiveActiveThumbColor, activeTrackColor: activeTrackColor ?? effectiveActiveTrackColor, inactiveThumbColor: inactiveThumbColor, inactiveTrackColor: inactiveTrackColor, activeThumbImage: activeThumbImage, onActiveThumbImageError: onActiveThumbImageError, inactiveThumbImage: inactiveThumbImage, onInactiveThumbImageError: onInactiveThumbImageError, thumbColor: thumbColor, trackColor: trackColor, trackOutlineColor: trackOutlineColor, trackOutlineWidth: trackOutlineWidth, thumbIcon: thumbIcon, materialTapTargetSize: materialTapTargetSize, dragStartBehavior: dragStartBehavior, mouseCursor: mouseCursor, focusColor: focusColor, hoverColor: hoverColor, overlayColor: overlayColor, splashRadius: splashRadius, focusNode: focusNode, onFocusChange: onFocusChange, autofocus: autofocus, applyCupertinoTheme: applyCupertinoTheme, switchType: _switchType);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("value", value: value, ifTrue: "on", ifFalse: "off", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action<bool>>("onChanged", onChanged, ifNull: "disabled"));
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
    public virtual global::Doroti.Framework.Painting.IImageProvider? activeThumbImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError { get; private set; }
    public virtual global::Doroti.Framework.Painting.IImageProvider? inactiveThumbImage { get; private set; } = default!;
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

    internal _MaterialSwitch__switch(bool value, global::System.Action<bool>? onChanged, Size size, _SwitchType__switch switchType, Color? activeThumbColor = null, Color? activeTrackColor = null, Color? inactiveThumbColor = null, Color? inactiveTrackColor = null, global::Doroti.Framework.Painting.IImageProvider? activeThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError = null, global::Doroti.Framework.Painting.IImageProvider? inactiveThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Color? focusColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, bool? applyCupertinoTheme = null)
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
        System.Diagnostics.Debug.Assert((activeThumbImage is not null) || (onActiveThumbImageError is null));
        System.Diagnostics.Debug.Assert((inactiveThumbImage is not null) || (onInactiveThumbImageError is null));
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
                __late__actionMap = new DartMap<Type, dynamic> { [typeof(ActivateIntent)] = new CallbackAction<ActivateIntent>(onInvoke: (__arg0) => { ((global::System.Action<Intent?>)_handleTap)(__arg0); return default!; }) };
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
        if (oldWidget.value != widget.value)
        {
            if ((position.value == 0.0) || (position.value == 1.0))
            {
                switch (widget.switchType)
                {
                    case _SwitchType__switch.adaptive:
                        {
                            switch (Theme.of(context).platform)
                            {
                                case TargetPlatform.android:
                                case TargetPlatform.fuchsia:
                                case TargetPlatform.linux:
                                case TargetPlatform.windows:
                                    {
                                        updateCurve();
                                        break;
                                    }
                                case TargetPlatform.iOS:
                                case TargetPlatform.macOS:
                                    {
                                        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.CurvedAnimation>)(() =>
{
    var __cascade = position;
    __cascade.curve = Curves.linear;
    __cascade.reverseCurve = Curves.linear;
    return __cascade;
}))());
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
        _painter.dispose();
        _positionController.dispose();
        _position.dispose();
        _reactionController.dispose();
        _reaction.dispose();
        _reactionHoverFadeController.dispose();
        _reactionHoverFade.dispose();
        _reactionFocusFadeController.dispose();
        _reactionFocusFade.dispose();
        base.dispose();
    }

    public virtual global::System.Action<bool?>? onChanged => (widget.onChanged is not null) ? _handleChanged : null;
    public virtual bool tristate => false;
    public virtual bool? value => widget.value;
    public virtual Duration? reactionAnimationDuration => ConstantsLibrary.kRadialReactionDuration;
    public virtual void updateCurve()
    {
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.CurvedAnimation>)(() =>
{
    var __cascade = position;
    __cascade.curve = Curves.easeOutBack;
    __cascade.reverseCurve = Curves.easeOutBack.flipped;
    return __cascade;
}))());
        }
    }

    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> _widgetThumbColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color?>((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return widget.inactiveThumbColor;
                }
                if (states.Contains(WidgetState.selected))
                {
                    return widget.activeThumbColor;
                }
                return widget.inactiveThumbColor;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> _widgetTrackColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color?>((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return widget.activeTrackColor;
                }
                return widget.inactiveTrackColor;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    internal virtual double _trackInnerLength
    {
        get
        {
            switch (widget.switchType)
            {
                case _SwitchType__switch.adaptive:
                    {
                        switch (Theme.of(context).platform)
                        {
                            case TargetPlatform.android:
                            case TargetPlatform.fuchsia:
                            case TargetPlatform.linux:
                            case TargetPlatform.windows:
                                {
                                    _SwitchConfig__switch config = new _SwitchConfigM3__switch(context);
                                    double trackInnerStart = config.trackHeight / 2.0;
                                    double trackInnerEnd = config.trackWidth - trackInnerStart;
                                    double trackInnerLength = trackInnerEnd - trackInnerStart;
                                    return trackInnerLength;
                                }
                            case TargetPlatform.iOS:
                            case TargetPlatform.macOS:
                                {
                                    _SwitchConfig__switch configLocal = new _SwitchConfigCupertino__switch(context);
                                    double trackInnerStartLocal = configLocal.trackHeight / 2.0;
                                    double trackInnerEndLocal = configLocal.trackWidth - trackInnerStartLocal;
                                    double trackInnerLengthLocal = trackInnerEndLocal - trackInnerStartLocal;
                                    return trackInnerLengthLocal;
                                }
                            default:
                                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                        }
                    }
                case _SwitchType__switch.material:
                    {
                        _SwitchConfig__switch configAlternate = new _SwitchConfigM3__switch(context);
                        double trackInnerStartAlternate = configAlternate.trackHeight / 2.0;
                        double trackInnerEndAlternate = configAlternate.trackWidth - trackInnerStartAlternate;
                        double trackInnerLengthAlternate = trackInnerEndAlternate - trackInnerStartAlternate;
                        return trackInnerLengthAlternate;
                    }
            }
            return default!;
        }
    }
    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        if (isInteractive)
        {
            reactionController.forward();
        }
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (isInteractive)
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.CurvedAnimation>)(() =>
{
    var __cascade = position;
    __cascade.curve = Curves.linear;
    __cascade.reverseCurve = null;
    return __cascade;
}))());
            double delta = DartRuntimePrimitives.RequireValue(details.primaryDelta) / _trackInnerLength;
            positionController.value += Directionality.of(context) switch { TextDirection.rtl => -delta, TextDirection.ltr => delta, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        if ((position.value >= 0.5) != widget.value)
        {
            widget.onChanged?.Invoke(!widget.value);
            setState(() =>
            {
                _needsPositionAnimation = true;
            });
        }
        else
        {
            animateToValue();
        }
        reactionController.reverse();
    }

    internal virtual void _handleChanged(bool? value)
    {
        DartRuntimePrimitives.Assert(() => value is not null);
        DartRuntimePrimitives.Assert(() => widget.onChanged is not null);
        widget.onChanged?.Invoke(DartRuntimePrimitives.RequireValue(value));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        if (_needsPositionAnimation)
        {
            _needsPositionAnimation = false;
            animateToValue();
        }
        ThemeData theme = Theme.of(context);
        SwitchThemeData switchTheme = SwitchTheme.of(context);
        global::Doroti.Ui.Color cupertinoPrimaryColor = theme.cupertinoOverrideTheme?.primaryColor ?? theme.colorScheme.primary;
        _SwitchConfig__switch switchConfig = default!;
        SwitchThemeData defaults = default!;
        var applyCupertinoThemeLocal = false;
        double disabledOpacity = 1;
        switch (widget.switchType)
        {
            case _SwitchType__switch.material:
                {
                    switchConfig = new _SwitchConfigM3__switch(context);
                    defaults = new _SwitchDefaultsM3__switch(context);
                    break;
                }
            case _SwitchType__switch.adaptive:
                {
                    Adaptation<SwitchThemeData> switchAdaptation = theme.getAdaptation<SwitchThemeData>() ?? new _SwitchThemeAdaptation__switch();
                    switchTheme = switchAdaptation.adapt(theme, switchTheme);
                    switch (theme.platform)
                    {
                        case TargetPlatform.android:
                        case TargetPlatform.fuchsia:
                        case TargetPlatform.linux:
                        case TargetPlatform.windows:
                            {
                                switchConfig = new _SwitchConfigM3__switch(context);
                                defaults = new _SwitchDefaultsM3__switch(context);
                                break;
                            }
                        case TargetPlatform.iOS:
                        case TargetPlatform.macOS:
                            {
                                isCupertino = true;
                                applyCupertinoThemeLocal = (widget.applyCupertinoTheme ?? theme.cupertinoOverrideTheme?.applyThemeToAll) ?? false;
                                disabledOpacity = 0.5;
                                switchConfig = DartRuntimePrimitives.ConvertValue<_SwitchConfig__switch>(new _SwitchConfigCupertino__switch(context));
                                defaults = DartRuntimePrimitives.ConvertValue<SwitchThemeData>(new _SwitchDefaultsCupertino__switch(context));
                                reactionController.duration = Duration.Create(milliseconds: 200L);
                                break;
                            }
                    }
                    break;
                }
        }
        var defaultOverlayColor = defaults.overlayColor ?? throw new InvalidOperationException("The built-in switch theme must provide overlayColor.");
        var defaultThumbColor = defaults.thumbColor ?? throw new InvalidOperationException("The built-in switch theme must provide thumbColor.");
        var defaultTrackColor = defaults.trackColor ?? throw new InvalidOperationException("The built-in switch theme must provide trackColor.");
        positionController.duration = Duration.Create(milliseconds: switchConfig.toggleDuration);
        HashSet<global::Doroti.Framework.Widgets.WidgetState> activeStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = states;
    __cascade.Add(WidgetState.selected);
    return __cascade;
}))();
        HashSet<global::Doroti.Framework.Widgets.WidgetState> inactiveStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = states;
    __cascade.Remove(WidgetState.selected);
    return __cascade;
}))();
        global::Doroti.Ui.Color? activeThumbColor = (widget.thumbColor?.resolve(activeStates) ?? _widgetThumbColor.resolve(activeStates)) ?? (switchTheme.thumbColor?.resolve(activeStates));
        global::Doroti.Ui.Color effectiveActiveThumbColor = activeThumbColor ?? defaultThumbColor.resolve(activeStates)!;
        global::Doroti.Ui.Color? inactiveThumbColor = (widget.thumbColor?.resolve(inactiveStates) ?? _widgetThumbColor.resolve(inactiveStates)) ?? (switchTheme.thumbColor?.resolve(inactiveStates));
        global::Doroti.Ui.Color effectiveInactiveThumbColor = inactiveThumbColor ?? defaultThumbColor.resolve(inactiveStates)!;
        global::Doroti.Ui.Color effectiveActiveTrackColor = (((widget.trackColor?.resolve(activeStates) ?? _widgetTrackColor.resolve(activeStates)) ?? (applyCupertinoThemeLocal ? cupertinoPrimaryColor : switchTheme.trackColor?.resolve(activeStates))) ?? _widgetThumbColor.resolve(activeStates)?.withAlpha(128L)) ?? defaultTrackColor.resolve(activeStates)!;
        global::Doroti.Ui.Color? effectiveActiveTrackOutlineColor = (widget.trackOutlineColor?.resolve(activeStates) ?? (switchTheme.trackOutlineColor?.resolve(activeStates))) ?? defaults.trackOutlineColor?.resolve(activeStates);
        double? effectiveActiveTrackOutlineWidth = (widget.trackOutlineWidth?.resolve(activeStates) ?? switchTheme.trackOutlineWidth?.resolve(activeStates)) ?? defaults.trackOutlineWidth?.resolve(activeStates);
        global::Doroti.Ui.Color effectiveInactiveTrackColor = ((widget.trackColor?.resolve(inactiveStates) ?? _widgetTrackColor.resolve(inactiveStates)) ?? (switchTheme.trackColor?.resolve(inactiveStates))) ?? defaultTrackColor.resolve(inactiveStates)!;
        global::Doroti.Ui.Color? effectiveInactiveTrackOutlineColor = (widget.trackOutlineColor?.resolve(inactiveStates) ?? (switchTheme.trackOutlineColor?.resolve(inactiveStates))) ?? (defaults.trackOutlineColor?.resolve(inactiveStates));
        double? effectiveInactiveTrackOutlineWidth = (widget.trackOutlineWidth?.resolve(inactiveStates) ?? switchTheme.trackOutlineWidth?.resolve(inactiveStates)) ?? defaults.trackOutlineWidth?.resolve(inactiveStates);
        global::Doroti.Framework.Widgets.Icon? effectiveActiveIcon = widget.thumbIcon?.resolve(activeStates) ?? (switchTheme.thumbIcon?.resolve(activeStates));
        global::Doroti.Framework.Widgets.Icon? effectiveInactiveIcon = widget.thumbIcon?.resolve(inactiveStates) ?? (switchTheme.thumbIcon?.resolve(inactiveStates));
        global::Doroti.Ui.Color effectiveActiveIconColor = effectiveActiveIcon?.color ?? switchConfig.iconColor.resolve(activeStates);
        global::Doroti.Ui.Color effectiveInactiveIconColor = effectiveInactiveIcon?.color ?? switchConfig.iconColor.resolve(inactiveStates);
        HashSet<global::Doroti.Framework.Widgets.WidgetState> focusedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = states;
    __cascade.Add(WidgetState.focused);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveFocusOverlayColor = (((widget.overlayColor?.resolve(focusedStates) ?? widget.focusColor) ?? (switchTheme.overlayColor?.resolve(focusedStates))) ?? (applyCupertinoThemeLocal ? HSLColor.CreateFromColor(cupertinoPrimaryColor.withOpacity(0.8)).withLightness(0.69).withSaturation(0.835).toColor() : null)) ?? defaultOverlayColor.resolve(focusedStates)!;
        HashSet<global::Doroti.Framework.Widgets.WidgetState> hoveredStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = states;
    __cascade.Add(WidgetState.hovered);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveHoverOverlayColor = ((widget.overlayColor?.resolve(hoveredStates) ?? widget.hoverColor) ?? (switchTheme.overlayColor?.resolve(hoveredStates))) ?? defaultOverlayColor.resolve(hoveredStates)!;
        var activePressedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = activeStates;
    __cascade.Add(WidgetState.pressed);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveActivePressedThumbColor = ((widget.thumbColor?.resolve(activePressedStates) ?? _widgetThumbColor.resolve(activePressedStates)) ?? (switchTheme.thumbColor?.resolve(activePressedStates))) ?? defaultThumbColor.resolve(activePressedStates)!;
        global::Doroti.Ui.Color effectiveActivePressedOverlayColor = ((widget.overlayColor?.resolve(activePressedStates) ?? (switchTheme.overlayColor?.resolve(activePressedStates))) ?? activeThumbColor?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? defaultOverlayColor.resolve(activePressedStates)!;
        var inactivePressedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = inactiveStates;
    __cascade.Add(WidgetState.pressed);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveInactivePressedThumbColor = ((widget.thumbColor?.resolve(inactivePressedStates) ?? _widgetThumbColor.resolve(inactivePressedStates)) ?? (switchTheme.thumbColor?.resolve(inactivePressedStates))) ?? defaultThumbColor.resolve(inactivePressedStates)!;
        global::Doroti.Ui.Color effectiveInactivePressedOverlayColor = ((widget.overlayColor?.resolve(inactivePressedStates) ?? (switchTheme.overlayColor?.resolve(inactivePressedStates))) ?? inactiveThumbColor?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? defaultOverlayColor.resolve(inactivePressedStates)!;
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> effectiveMouseCursor = WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>((states) =>
        {
            return WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(widget.mouseCursor, states)
                ?? switchTheme.mouseCursor?.resolve(states)
                ?? WidgetStateMouseCursor.adaptiveClickable.resolve(states);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        double effectiveActiveThumbRadius = (effectiveActiveIcon is null) ? switchConfig.activeThumbRadius : switchConfig.thumbRadiusWithIcon;
        double effectiveInactiveThumbRadius = ((effectiveInactiveIcon is null) && (widget.inactiveThumbImage is null)) ? switchConfig.inactiveThumbRadius : switchConfig.thumbRadiusWithIcon;
        double effectiveSplashRadius = (widget.splashRadius ?? switchTheme.splashRadius) ?? DartRuntimePrimitives.RequireValue(defaults.splashRadius);
        return new global::Doroti.Framework.Widgets.Semantics(toggled: widget.value, child: new global::Doroti.Framework.Widgets.GestureDetector(excludeFromSemantics: true, onHorizontalDragStart: _handleDragStart, onHorizontalDragUpdate: _handleDragUpdate, onHorizontalDragEnd: _handleDragEnd, dragStartBehavior: widget.dragStartBehavior, child: new global::Doroti.Framework.Widgets.Opacity(opacity: (onChanged is null) ? disabledOpacity : 1, child: buildToggleable(mouseCursor: effectiveMouseCursor, focusNode: widget.focusNode, onFocusChange: widget.onFocusChange, autofocus: widget.autofocus, size: widget.size, painter: ((Func<_SwitchPainter__switch>)(() =>
{
    var __cascade = _painter;
    __cascade.position = position;
    __cascade.reaction = reaction;
    __cascade.reactionFocusFade = reactionFocusFade;
    __cascade.reactionHoverFade = reactionHoverFade;
    __cascade.inactiveReactionColor = effectiveInactivePressedOverlayColor;
    __cascade.reactionColor = effectiveActivePressedOverlayColor;
    __cascade.hoverColor = effectiveHoverOverlayColor;
    __cascade.focusColor = effectiveFocusOverlayColor;
    __cascade.splashRadius = effectiveSplashRadius;
    __cascade.downPosition = downPosition;
    __cascade.isFocused = states.Contains(WidgetState.focused);
    __cascade.isHovered = states.Contains(WidgetState.hovered);
    __cascade.activeColor = effectiveActiveThumbColor;
    __cascade.inactiveColor = effectiveInactiveThumbColor;
    __cascade.activePressedColor = effectiveActivePressedThumbColor;
    __cascade.inactivePressedColor = effectiveInactivePressedThumbColor;
    __cascade.activeThumbImage = widget.activeThumbImage;
    __cascade.onActiveThumbImageError = widget.onActiveThumbImageError;
    __cascade.inactiveThumbImage = widget.inactiveThumbImage;
    __cascade.onInactiveThumbImageError = widget.onInactiveThumbImageError;
    __cascade.activeTrackColor = effectiveActiveTrackColor;
    __cascade.activeTrackOutlineColor = effectiveActiveTrackOutlineColor;
    __cascade.activeTrackOutlineWidth = effectiveActiveTrackOutlineWidth;
    __cascade.inactiveTrackColor = effectiveInactiveTrackColor;
    __cascade.inactiveTrackOutlineColor = effectiveInactiveTrackOutlineColor;
    __cascade.inactiveTrackOutlineWidth = effectiveInactiveTrackOutlineWidth;
    __cascade.configuration = ImageLibrary.createLocalImageConfiguration(context);
    __cascade.isInteractive = isInteractive;
    __cascade.trackInnerLength = _trackInnerLength;
    __cascade.textDirection = Directionality.of(context);
    __cascade.surfaceColor = theme.colorScheme.surface;
    __cascade.inactiveThumbRadius = effectiveInactiveThumbRadius;
    __cascade.activeThumbRadius = effectiveActiveThumbRadius;
    __cascade.pressedThumbRadius = switchConfig.pressedThumbRadius;
    __cascade.thumbOffset = switchConfig.thumbOffset;
    __cascade.trackHeight = switchConfig.trackHeight;
    __cascade.trackWidth = switchConfig.trackWidth;
    __cascade.activeIconColor = effectiveActiveIconColor;
    __cascade.inactiveIconColor = effectiveInactiveIconColor;
    __cascade.activeIcon = effectiveActiveIcon;
    __cascade.inactiveIcon = effectiveInactiveIcon;
    __cascade.iconTheme = IconTheme.of(context);
    __cascade.thumbShadow = switchConfig.thumbShadow;
    __cascade.transitionalThumbSize = switchConfig.transitionalThumbSize;
    __cascade.positionController = positionController;
    __cascade.isCupertino = isCupertino;
    return __cascade;
}))()))));
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

    public virtual global::Doroti.Framework.Animation.AnimationController positionController => _positionController;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation position => _position;
    public virtual global::Doroti.Framework.Animation.AnimationController reactionController => _reactionController;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation reaction => _reaction;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation reactionHoverFade => _reactionHoverFade;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation reactionFocusFade => _reactionFocusFade;
    public virtual bool isInteractive => DartRuntimePrimitives.ConvertValue<bool>(onChanged is not null);
    public override void initState()
    {
        base.initState();
        _positionController = new global::Doroti.Framework.Animation.AnimationController(duration: ToggleableLibrary._kToggleDuration, value: (value == false) ? 0.0 : 1.0, vsync: this);
        _position = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _positionController, curve: Curves.easeIn, reverseCurve: Curves.easeOut);
        _reactionController = new global::Doroti.Framework.Animation.AnimationController(duration: _reactionAnimationDuration, vsync: this);
        _reaction = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _reactionController, curve: Curves.fastOutSlowIn);
        _reactionHoverFadeController = new global::Doroti.Framework.Animation.AnimationController(duration: ToggleableLibrary._kReactionFadeDuration, value: (_hovering || _focused) ? 1.0 : 0.0, vsync: this);
        _reactionHoverFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _reactionHoverFadeController, curve: Curves.fastOutSlowIn);
        _reactionFocusFadeController = new global::Doroti.Framework.Animation.AnimationController(duration: ToggleableLibrary._kReactionFadeDuration, value: (_hovering || _focused) ? 1.0 : 0.0, vsync: this);
        _reactionFocusFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _reactionFocusFadeController, curve: Curves.fastOutSlowIn);
    }

    public virtual void animateToValue()
    {
        if (tristate)
        {
            if (value is null)
            {
                _positionController.value = 0.0;
            }
            if (value ?? true)
            {
                _positionController.forward();
            }
            else
            {
                _positionController.reverse();
            }
        }
        else
        {
            if (value ?? false)
            {
                _positionController.forward();
            }
            else
            {
                _positionController.reverse();
            }
        }
    }

    public virtual Offset? downPosition => _downPosition;
    public virtual void _handleTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        if (isInteractive)
        {
            setState(() =>
            {
                _downPosition = details.localPosition;
            });
            _reactionController.forward();
        }
    }

    public virtual void _handleTap(Intent? __unused0 = null)
    {
        if (!isInteractive)
        {
            return;
        }
        switch (value)
        {
            case false:
                {
                    onChanged!(true);
                    break;
                }
            case true:
                {
                    onChanged!(tristate ? null : false);
                    break;
                }
            case null:
                {
                    onChanged!(false);
                    break;
                }
        }
        context.findRenderObject()!.sendSemanticsEvent(new global::Doroti.Framework.Semantics.TapSemanticEvent());
    }

    public virtual void _handleTapEnd(global::Doroti.Framework.Gestures.TapUpDetails? __unused0 = null)
    {
        if (_downPosition is not null)
        {
            setState(() =>
            {
                _downPosition = null;
            });
        }
        _reactionController.reverse();
    }

    public virtual void _handleFocusHighlightChanged(bool focused)
    {
        if (focused != _focused)
        {
            setState(() =>
            {
                _focused = focused;
            });
            if (focused)
            {
                _reactionFocusFadeController.forward();
            }
            else
            {
                _reactionFocusFadeController.reverse();
            }
        }
    }

    public virtual void _handleHoverChanged(bool hovering)
    {
        if (hovering != _hovering)
        {
            setState(() =>
            {
                _hovering = hovering;
            });
            if (hovering)
            {
                _reactionHoverFadeController.forward();
            }
            else
            {
                _reactionHoverFadeController.reverse();
            }
        }
    }

    public virtual HashSet<WidgetState> states => ((Func<HashSet<WidgetState>>)(() => { var __collection10795 = new HashSet<WidgetState>(); if (!isInteractive) { __collection10795.Add(WidgetState.disabled); } if (_hovering) { __collection10795.Add(WidgetState.hovered); } if (_focused) { __collection10795.Add(WidgetState.focused); } if (value ?? true) { __collection10795.Add(WidgetState.selected); } return __collection10795; }))();
    public virtual Widget buildToggleable(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor = null, Size size = default!, global::Doroti.Framework.Widgets.ToggleablePainter painter = default!)
    {
        return buildToggleableWithChild(focusNode: focusNode, onFocusChange: onFocusChange, autofocus: autofocus, mouseCursor: mouseCursor, child: new CustomPaint(size: size, painter: painter));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildToggleableWithChild(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor = null, Widget child = default!)
    {
        return new FocusableActionDetector(actions: _actionMap, focusNode: focusNode, autofocus: autofocus, onFocusChange: onFocusChange, enabled: isInteractive, onShowFocusHighlight: _handleFocusHighlightChanged, onShowHoverHighlight: _handleHoverChanged, mouseCursor: mouseCursor?.resolve(states) ?? SystemMouseCursors.basic, child: new GestureDetector(excludeFromSemantics: !isInteractive, onTapDown: isInteractive ? _handleTapDown : null, onTap: isInteractive ? () => _handleTap() : null, onTapUp: isInteractive ? _handleTapEnd : null, onTapCancel: isInteractive ? () => _handleTapEnd() : null, child: new global::Doroti.Framework.Widgets.Semantics(enabled: isInteractive, child: child)));
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
    internal virtual global::Doroti.Framework.Painting.IImageProvider? _activeThumbImage { get; set; } = default!;
    internal virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? _onActiveThumbImageError { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.IImageProvider? _inactiveThumbImage { get; set; } = default!;
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
    internal virtual global::Doroti.Framework.Painting.IImageProvider? _cachedThumbImage { get; set; } = default!;
    internal virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? _cachedThumbErrorListener { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.BoxPainter? _cachedThumbPainter { get; set; } = default;
    internal virtual bool _isPainting { get; set; } = false;
    internal virtual bool _stopPressAnimation { get; set; } = false;
    internal virtual double? _pressedInactiveThumbRadius { get; set; } = default;
    internal virtual double? _pressedActiveThumbRadius { get; set; } = default;
    internal virtual double? _pressedThumbExtension { get; set; } = default;

    public virtual global::Doroti.Framework.Animation.AnimationController positionController
    {
        get => _positionController!;
        set
        {
            var __value = value;
            if (Equals(__value, _positionController))
            {
                return;
            }
            _positionController = __value;
            _colorAnimation?.dispose();
            _colorAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: positionController, curve: Curves.easeOut, reverseCurve: Curves.easeIn);
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Widgets.Icon? activeIcon
    {
        get => _activeIcon;
        set
        {
            var __value = value;
            if (Equals(__value, _activeIcon))
            {
                return;
            }
            _activeIcon = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Widgets.Icon? inactiveIcon
    {
        get => _inactiveIcon;
        set
        {
            var __value = value;
            if (Equals(__value, _inactiveIcon))
            {
                return;
            }
            _inactiveIcon = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? iconTheme
    {
        get => _iconTheme;
        set
        {
            var __value = value;
            if (Equals(__value, _iconTheme))
            {
                return;
            }
            _iconTheme = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activeIconColor
    {
        get => _activeIconColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _activeIconColor))
            {
                return;
            }
            _activeIconColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactiveIconColor
    {
        get => _inactiveIconColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _inactiveIconColor))
            {
                return;
            }
            _inactiveIconColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activePressedColor
    {
        get => _activePressedColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _activePressedColor))
            {
                return;
            }
            _activePressedColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactivePressedColor
    {
        get => _inactivePressedColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _inactivePressedColor))
            {
                return;
            }
            _inactivePressedColor = __value;
            notifyListeners();
        }
    }
    public virtual double activeThumbRadius
    {
        get => DartRuntimePrimitives.RequireValue(_activeThumbRadius);
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _activeThumbRadius)
            {
                return;
            }
            _activeThumbRadius = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double inactiveThumbRadius
    {
        get => DartRuntimePrimitives.RequireValue(_inactiveThumbRadius);
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _inactiveThumbRadius)
            {
                return;
            }
            _inactiveThumbRadius = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double pressedThumbRadius
    {
        get => DartRuntimePrimitives.RequireValue(_pressedThumbRadius);
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _pressedThumbRadius)
            {
                return;
            }
            _pressedThumbRadius = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double? thumbOffset
    {
        get => _thumbOffset;
        set
        {
            var __value = value;
            if (__value == _thumbOffset)
            {
                return;
            }
            _thumbOffset = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Size transitionalThumbSize
    {
        get => DartRuntimePrimitives.RequireValue(_transitionalThumbSize);
        set
        {
            var __value = value;
            if (Equals(DartRuntimePrimitives.RequireValue(__value), _transitionalThumbSize))
            {
                return;
            }
            _transitionalThumbSize = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double trackHeight
    {
        get => DartRuntimePrimitives.RequireValue(_trackHeight);
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _trackHeight)
            {
                return;
            }
            _trackHeight = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double trackWidth
    {
        get => DartRuntimePrimitives.RequireValue(_trackWidth);
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _trackWidth)
            {
                return;
            }
            _trackWidth = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.IImageProvider? activeThumbImage
    {
        get => _activeThumbImage;
        set
        {
            var __value = value;
            if (Equals(__value, _activeThumbImage))
            {
                return;
            }
            _activeThumbImage = __value;
            notifyListeners();
        }
    }
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError
    {
        get => _onActiveThumbImageError;
        set
        {
            var __value = value;
            if (Equals(__value, _onActiveThumbImageError))
            {
                return;
            }
            _onActiveThumbImageError = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.IImageProvider? inactiveThumbImage
    {
        get => _inactiveThumbImage;
        set
        {
            var __value = value;
            if (Equals(__value, _inactiveThumbImage))
            {
                return;
            }
            _inactiveThumbImage = __value;
            notifyListeners();
        }
    }
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError
    {
        get => _onInactiveThumbImageError;
        set
        {
            var __value = value;
            if (Equals(__value, _onInactiveThumbImageError))
            {
                return;
            }
            _onInactiveThumbImageError = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activeTrackColor
    {
        get => _activeTrackColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _activeTrackColor))
            {
                return;
            }
            _activeTrackColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color? activeTrackOutlineColor
    {
        get => _activeTrackOutlineColor;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(__value, _activeTrackOutlineColor))
            {
                return;
            }
            _activeTrackOutlineColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color? inactiveTrackOutlineColor
    {
        get => _inactiveTrackOutlineColor;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(__value, _inactiveTrackOutlineColor))
            {
                return;
            }
            _inactiveTrackOutlineColor = __value;
            notifyListeners();
        }
    }
    public virtual double? activeTrackOutlineWidth
    {
        get => _activeTrackOutlineWidth;
        set
        {
            var __value = value;
            if (__value == _activeTrackOutlineWidth)
            {
                return;
            }
            _activeTrackOutlineWidth = __value;
            notifyListeners();
        }
    }
    public virtual double? inactiveTrackOutlineWidth
    {
        get => _inactiveTrackOutlineWidth;
        set
        {
            var __value = value;
            if (__value == _inactiveTrackOutlineWidth)
            {
                return;
            }
            _inactiveTrackOutlineWidth = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactiveTrackColor
    {
        get => _inactiveTrackColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _inactiveTrackColor))
            {
                return;
            }
            _inactiveTrackColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.ImageConfiguration configuration
    {
        get => _configuration!;
        set
        {
            var __value = value;
            if (Equals(__value, _configuration))
            {
                return;
            }
            _configuration = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => DartRuntimePrimitives.RequireValue(_textDirection);
        set
        {
            var __value = value;
            if (Equals(_textDirection, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color surfaceColor
    {
        get => _surfaceColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _surfaceColor))
            {
                return;
            }
            _surfaceColor = __value;
            notifyListeners();
        }
    }
    public virtual bool isInteractive
    {
        get => DartRuntimePrimitives.RequireValue(_isInteractive);
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _isInteractive)
            {
                return;
            }
            _isInteractive = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double trackInnerLength
    {
        get => DartRuntimePrimitives.RequireValue(_trackInnerLength);
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _trackInnerLength)
            {
                return;
            }
            _trackInnerLength = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual bool isCupertino
    {
        get => DartRuntimePrimitives.RequireValue(_isCupertino);
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _isCupertino)
            {
                return;
            }
            _isCupertino = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual List<global::Doroti.Framework.Painting.BoxShadow>? thumbShadow
    {
        get => _thumbShadow;
        set
        {
            var __value = value;
            if (Equals(__value, _thumbShadow))
            {
                return;
            }
            _thumbShadow = __value;
            notifyListeners();
        }
    }
    internal virtual global::Doroti.Framework.Painting.ShapeDecoration _createDefaultThumbDecoration(Color color, global::Doroti.Framework.Painting.IImageProvider? image, global::System.Action<object, global::System.Diagnostics.StackTrace?>? errorListener)
    {
        return new global::Doroti.Framework.Painting.ShapeDecoration(color: color, image: (image is null) ? null : new global::Doroti.Framework.Painting.DecorationImage(image: image, onError: errorListener), shape: new global::Doroti.Framework.Painting.StadiumBorder(), shadows: isCupertino ? null : thumbShadow);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleDecorationChanged()
    {
        if (!_isPainting)
        {
            notifyListeners();
        }
    }

    public override void paint(Canvas canvas, Size size)
    {
        double currentValue = position.value;
        double visualPosition = textDirection switch { TextDirection.rtl => 1.0 - currentValue, TextDirection.ltr => currentValue, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        if (Equals(reaction.status, AnimationStatus.reverse) && !_stopPressAnimation)
        {
            _stopPressAnimation = true;
        }
        else
        {
            _stopPressAnimation = false;
        }
        if (!_stopPressAnimation)
        {
            _pressedThumbExtension = isCupertino ? (reaction.value * 7L) : 0;
            if (reaction.isCompleted)
            {
                _pressedInactiveThumbRadius = Dart_uiLibrary.lerpDouble(inactiveThumbRadius, pressedThumbRadius, reaction.value);
                _pressedActiveThumbRadius = Dart_uiLibrary.lerpDouble(activeThumbRadius, pressedThumbRadius, reaction.value);
            }
            if (currentValue == 0L)
            {
                _pressedInactiveThumbRadius = Dart_uiLibrary.lerpDouble(inactiveThumbRadius, pressedThumbRadius, reaction.value);
                _pressedActiveThumbRadius = activeThumbRadius;
            }
            if (currentValue == 1L)
            {
                _pressedActiveThumbRadius = Dart_uiLibrary.lerpDouble(activeThumbRadius, pressedThumbRadius, reaction.value);
                _pressedInactiveThumbRadius = inactiveThumbRadius;
            }
        }
        var inactiveThumbSize = isCupertino ? new global::Doroti.Ui.Size((DartRuntimePrimitives.RequireValue(_pressedInactiveThumbRadius) * 2L) + DartRuntimePrimitives.RequireValue(_pressedThumbExtension), DartRuntimePrimitives.RequireValue(_pressedInactiveThumbRadius) * 2L) : Size.fromRadius(_pressedInactiveThumbRadius ?? (double)inactiveThumbRadius);
        var activeThumbSize = isCupertino ? new global::Doroti.Ui.Size((DartRuntimePrimitives.RequireValue(_pressedActiveThumbRadius) * 2L) + DartRuntimePrimitives.RequireValue(_pressedThumbExtension), DartRuntimePrimitives.RequireValue(_pressedActiveThumbRadius) * 2L) : Size.fromRadius(_pressedActiveThumbRadius ?? (double)activeThumbRadius);
        global::Doroti.Framework.Animation.Animation<Size> thumbSizeAnimation(bool isForward)
        {
            List<global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>> thumbSizeSequence = default!;
            if (isForward)
            {
                thumbSizeSequence = new List<global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>> { new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>(tween: new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Size>(begin: inactiveThumbSize, end: transitionalThumbSize).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Cubic(0.31, 0.0, 0.56, 1.0))), weight: 11), new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>(tween: new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Size>(begin: transitionalThumbSize, end: activeThumbSize).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Cubic(0.2, 0.0, 0.0, 1.0))), weight: 72), new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>(tween: new global::Doroti.Framework.Animation.ConstantTween<global::Doroti.Ui.Size>(activeThumbSize), weight: 17) };
            }
            else
            {
                thumbSizeSequence = new List<global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>> { new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>(tween: new global::Doroti.Framework.Animation.ConstantTween<global::Doroti.Ui.Size>(inactiveThumbSize), weight: 17), new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>(tween: new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Size>(begin: inactiveThumbSize, end: transitionalThumbSize).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Cubic(0.2, 0.0, 0.0, 1.0).flipped)), weight: 72), new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>(tween: new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Size>(begin: transitionalThumbSize, end: activeThumbSize).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Cubic(0.31, 0.0, 0.56, 1.0).flipped)), weight: 11) };
            }
            return new global::Doroti.Framework.Animation.TweenSequence<global::Doroti.Ui.Size>(thumbSizeSequence.Cast<global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Size>>().ToList()).animate(positionController);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Ui.Size? thumbSize = default!;
        if (isCupertino)
        {
            if (reaction.isCompleted)
            {
                thumbSize = new global::Doroti.Ui.Size((DartRuntimePrimitives.RequireValue(_pressedInactiveThumbRadius) * 2L) + DartRuntimePrimitives.RequireValue(_pressedThumbExtension), DartRuntimePrimitives.RequireValue(_pressedInactiveThumbRadius) * 2L);
            }
            else
            {
                if (position.isDismissed || Equals(position.status, AnimationStatus.forward))
                {
                    thumbSize = Dart_uiLibrary.Size.lerp(inactiveThumbSize, activeThumbSize, position.value);
                }
                else
                {
                    thumbSize = Dart_uiLibrary.Size.lerp(inactiveThumbSize, activeThumbSize, position.value);
                }
            }
        }
        else
        {
            if (reaction.isCompleted)
            {
                thumbSize = Size.fromRadius(pressedThumbRadius);
            }
            else
            {
                if (position.isDismissed || Equals(position.status, AnimationStatus.forward))
                {
                    thumbSize = thumbSizeAnimation(true).value;
                }
                else
                {
                    thumbSize = thumbSizeAnimation(false).value;
                }
            }
        }
        double inset = (thumbOffset is null) ? 0 : (1.0 - ((currentValue - DartRuntimePrimitives.RequireValue(thumbOffset)).abs() * 2.0));
        thumbSize = new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(thumbSize).width - inset, DartRuntimePrimitives.RequireValue(thumbSize).height - inset);
        double colorValue = _colorAnimation!.value;
        global::Doroti.Ui.Color trackColor = Dart_uiLibrary.Color.lerp(inactiveTrackColor, activeTrackColor, colorValue)!;
        global::Doroti.Ui.Color? trackOutlineColor = ((inactiveTrackOutlineColor is null) || (activeTrackOutlineColor is null)) ? null : Dart_uiLibrary.Color.lerp(inactiveTrackOutlineColor, activeTrackOutlineColor, colorValue);
        double? trackOutlineWidth = Dart_uiLibrary.lerpDouble(inactiveTrackOutlineWidth, activeTrackOutlineWidth, colorValue);
        global::Doroti.Ui.Color lerpedThumbColor = default!;
        if (!reaction.isDismissed)
        {
            lerpedThumbColor = Dart_uiLibrary.Color.lerp(inactivePressedColor, activePressedColor, colorValue)!;
        }
        else
        {
            if (Equals(positionController.status, AnimationStatus.forward))
            {
                lerpedThumbColor = Dart_uiLibrary.Color.lerp(inactivePressedColor, activeColor, colorValue)!;
            }
            else
            {
                if (Equals(positionController.status, AnimationStatus.reverse))
                {
                    lerpedThumbColor = Dart_uiLibrary.Color.lerp(inactiveColor, activePressedColor, colorValue)!;
                }
                else
                {
                    lerpedThumbColor = Dart_uiLibrary.Color.lerp(inactiveColor, activeColor, colorValue)!;
                }
            }
        }
        global::Doroti.Ui.Color thumbColor = Dart_uiLibrary.Color.alphaBlend(lerpedThumbColor, surfaceColor);
        global::Doroti.Framework.Widgets.Icon? thumbIcon = (currentValue < 0.5) ? inactiveIcon : activeIcon;
        global::Doroti.Framework.Painting.IImageProvider? thumbImage = (currentValue < 0.5) ? inactiveThumbImage : activeThumbImage;
        global::System.Action<object, global::System.Diagnostics.StackTrace?>? thumbErrorListener = (currentValue < 0.5) ? onInactiveThumbImageError : onActiveThumbImageError;
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = trackColor;
    return __cascade;
}))();
        global::Doroti.Ui.Offset trackPaintOffset = _computeTrackPaintOffset(size, trackWidth, trackHeight);
        global::Doroti.Ui.Offset thumbPaintOffset = _computeThumbPaintOffset(trackPaintOffset, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(thumbSize)), visualPosition);
        var radialReactionOrigin = new global::Doroti.Ui.Offset(thumbPaintOffset.dx + (DartRuntimePrimitives.RequireValue(thumbSize).height / 2L), size.height / 2L);
        _paintTrackWith(canvas, paintLocal, trackPaintOffset, trackOutlineColor, trackOutlineWidth);
        paintRadialReaction(canvas: canvas, origin: radialReactionOrigin);
        _paintThumbWith(thumbPaintOffset, canvas, colorValue, thumbColor, thumbImage, thumbErrorListener, thumbIcon, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(thumbSize)), inset);
    }

    internal virtual global::Doroti.Ui.Offset _computeTrackPaintOffset(Size canvasSize, double trackWidth, double trackHeight)
    {
        double horizontalOffset = (canvasSize.width - trackWidth) / 2.0;
        double verticalOffset = (canvasSize.height - trackHeight) / 2.0;
        return new global::Doroti.Ui.Offset(horizontalOffset, verticalOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _computeThumbPaintOffset(Offset trackPaintOffset, Size thumbSize, double visualPosition)
    {
        double trackRadius = trackHeight / 2L;
        double additionalThumbRadius = (thumbSize.height / 2L) - trackRadius;
        double horizontalProgress = visualPosition * (trackInnerLength - DartRuntimePrimitives.RequireValue(_pressedThumbExtension));
        double thumbHorizontalOffset = trackPaintOffset.dx + trackRadius + DartRuntimePrimitives.RequireValue(_pressedThumbExtension) / 2L - (thumbSize.width / 2L) + horizontalProgress;
        double thumbVerticalOffset = trackPaintOffset.dy - additionalThumbRadius;
        return new global::Doroti.Ui.Offset(thumbHorizontalOffset, thumbVerticalOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintTrackWith(Canvas canvas, Paint paint, Offset trackPaintOffset, Color? trackOutlineColor, double? trackOutlineWidth)
    {
        var trackRect = Rect.fromLTWH(trackPaintOffset.dx, trackPaintOffset.dy, trackWidth, trackHeight);
        double trackRadius = trackHeight / 2L;
        var trackRRect = RRect.fromRectAndRadius(trackRect, Radius.circular(trackRadius));
        canvas.drawRRect(trackRRect, paint);
        if (trackOutlineColor is not null)
        {
            var outlineTrackRect = Rect.fromLTWH(trackPaintOffset.dx + 1L, trackPaintOffset.dy + 1L, trackWidth - 2L, trackHeight - 2L);
            var outlineTrackRRect = RRect.fromRectAndRadius(outlineTrackRect, Radius.circular(trackRadius));
            var outlinePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = trackOutlineWidth ?? 2.0;
    __cascade.color = trackOutlineColor;
    return __cascade;
}))();
            canvas.drawRRect(outlineTrackRRect, outlinePaint);
        }
        if (isCupertino)
        {
            if (isFocused)
            {
                global::Doroti.Ui.RRect focusedOutline = trackRRect.inflate(1.75);
                var focusedPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.color = focusColor;
    __cascade.strokeWidth = SwitchLibrary._kCupertinoFocusTrackOutline;
    return __cascade;
}))();
                canvas.drawRRect(focusedOutline, focusedPaint);
            }
            canvas.clipRRect(trackRRect);
        }
    }

    internal virtual void _paintThumbWith(Offset thumbPaintOffset, Canvas canvas, double currentValue, Color thumbColor, global::Doroti.Framework.Painting.IImageProvider? thumbImage, global::System.Action<object, global::System.Diagnostics.StackTrace?>? thumbErrorListener, global::Doroti.Framework.Widgets.Icon? thumbIcon, Size thumbSize, double inset)
    {
        try
        {
            _isPainting = true;
            if ((_cachedThumbPainter is null) || (!Equals(thumbColor, _cachedThumbColor)) || (!Equals(thumbImage, _cachedThumbImage)) || (!Equals(thumbErrorListener, _cachedThumbErrorListener)))
            {
                _cachedThumbColor = thumbColor;
                _cachedThumbImage = thumbImage;
                _cachedThumbErrorListener = thumbErrorListener;
                _cachedThumbPainter?.dispose();
                _cachedThumbPainter = _createDefaultThumbDecoration(thumbColor, thumbImage, thumbErrorListener).createBoxPainter(() => _handleDecorationChanged());
            }
            global::Doroti.Framework.Painting.BoxPainter thumbPainter = _cachedThumbPainter!;
            if (isCupertino)
            {
                _paintCupertinoThumbShadowAndBorder(canvas, thumbPaintOffset, thumbSize);
            }
            thumbPainter.paint(canvas, thumbPaintOffset, configuration.copyWith(size: thumbSize));
            if ((thumbIcon is not null) && (thumbIcon.icon is not null))
            {
                global::Doroti.Ui.Color iconColor = Dart_uiLibrary.Color.lerp(inactiveIconColor, activeIconColor, currentValue)!;
                double iconSizeLocal = thumbIcon.size ?? _SwitchConfigM3__switch.iconSize;
                global::Doroti.Framework.Widgets.IconData iconData = thumbIcon.icon!;
                double? iconWeight = thumbIcon.weight ?? iconTheme?.weight;
                double? iconFill = thumbIcon.fill ?? iconTheme?.fill;
                double? iconGrade = thumbIcon.grade ?? iconTheme?.grade;
                double? iconOpticalSize = thumbIcon.opticalSize ?? iconTheme?.opticalSize;
                List<global::Doroti.Ui.Shadow>? iconShadows = (thumbIcon.shadows ?? iconTheme?.shadows)?.ToList();
                var textSpan = new global::Doroti.Framework.Painting.TextSpan(text: char.ConvertFromUtf32(checked((int)iconData.codePoint)), style: new global::Doroti.Framework.Painting.TextStyle(fontVariations: ((Func<List<global::Doroti.Ui.FontVariation>>)(() => { var __collection65120 = new List<global::Doroti.Ui.FontVariation>(); if (iconFill is not null) { __collection65120.Add(new global::Doroti.Ui.FontVariation("FILL", DartRuntimePrimitives.RequireValue(iconFill))); } if (iconWeight is not null) { __collection65120.Add(new global::Doroti.Ui.FontVariation("wght", DartRuntimePrimitives.RequireValue(iconWeight))); } if (iconGrade is not null) { __collection65120.Add(new global::Doroti.Ui.FontVariation("GRAD", DartRuntimePrimitives.RequireValue(iconGrade))); } if (iconOpticalSize is not null) { __collection65120.Add(new global::Doroti.Ui.FontVariation("opsz", DartRuntimePrimitives.RequireValue(iconOpticalSize))); } return __collection65120; }))(), color: iconColor, fontSize: iconSizeLocal, inherit: false, fontFamily: iconData.fontFamily, package: iconData.fontPackage, shadows: iconShadows));
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = _textPainter;
    __cascade.textDirection = textDirection;
    __cascade.text = textSpan;
    return __cascade;
}))());
                _textPainter.layout();
                double additionalHorizontalOffset = (thumbSize.width - iconSizeLocal) / 2L;
                double additionalVerticalOffset = (thumbSize.height - iconSizeLocal) / 2L;
                global::Doroti.Ui.Offset offset = thumbPaintOffset + new global::Doroti.Ui.Offset(additionalHorizontalOffset, additionalVerticalOffset);
                _textPainter.paint(canvas, offset);
            }
        }
        finally
        {
            _isPainting = false;
        }
    }

    internal virtual void _paintCupertinoThumbShadowAndBorder(Canvas canvas, Offset thumbPaintOffset, Size thumbSize)
    {
        var thumbBounds = RRect.fromLTRBR(thumbPaintOffset.dx, thumbPaintOffset.dy, thumbPaintOffset.dx + thumbSize.width, thumbPaintOffset.dy + thumbSize.height, Radius.circular(thumbSize.height / 2.0));
        if (thumbShadow is not null)
        {
            foreach (global::Doroti.Framework.Painting.BoxShadow shadow in thumbShadow!)
            {
                canvas.drawRRect(thumbBounds.shift(shadow.offset), shadow.toPaint());
            }
        }
        canvas.drawRRect(thumbBounds.inflate(0.5), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(167772160L);
    return __cascade;
}))());
    }

    public override void dispose()
    {
        _textPainter.dispose();
        _cachedThumbPainter?.dispose();
        _cachedThumbPainter = null;
        _cachedThumbColor = null;
        _cachedThumbImage = null;
        _cachedThumbErrorListener = null;
        _colorAnimation?.dispose();
        base.dispose();
    }

}

internal class _SwitchThemeAdaptation__switch : Adaptation<SwitchThemeData>
{
    internal _SwitchThemeAdaptation__switch()
    {
    }

    public override SwitchThemeData adapt(ThemeData theme, SwitchThemeData defaultValue)
    {
        switch (theme.platform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    return defaultValue;
                }
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
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

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?> mouseCursor
    {
        get
        {
            return WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return SystemMouseCursors.basic;
                }
                return Foundation.ConstantsLibrary.kIsWeb ? SystemMouseCursors.click : SystemMouseCursors.basic;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> thumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.white));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> trackColor
    {
        get
        {
            return WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return CupertinoDynamicColor.resolve(CupertinoColors.systemGreen, context);
                }
                return CupertinoDynamicColor.resolve(CupertinoColors.secondarySystemFill, context);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> trackOutlineColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> overlayColor
    {
        get
        {
            return (global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.focused))
                {
                    return HSLColor.CreateFromColor(CupertinoDynamicColor.resolve(CupertinoColors.systemGreen, context).withOpacity(0.8)).withLightness(0.69).withSaturation(0.835).toColor();
                }
                return Colors.transparent;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override double? splashRadius => 0.0;
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
        _colors = Theme.of(context).colorScheme;
    }

    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color> iconColor
    {
        get
        {
            return WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return _colors.onSurface.withOpacity(0.38);
                }
                return _colors.onPrimaryContainer;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public virtual double activeThumbRadius => 14.0;
    public virtual double inactiveThumbRadius => 14.0;
    public virtual double pressedThumbRadius => 14.0;
    public virtual double switchHeight => DartRuntimePrimitives.ConvertValue<double>(switchMinSize.height + 8.0);
    public virtual double switchHeightCollapsed => switchMinSize.height;
    public virtual double switchWidth => 60.0;
    public virtual double thumbRadiusWithIcon => 14.0;
    public virtual List<global::Doroti.Framework.Painting.BoxShadow>? thumbShadow => new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(637534208L), offset: new global::Doroti.Ui.Offset(0, 3), blurRadius: 8.0), new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(251658240L), offset: new global::Doroti.Ui.Offset(0, 3), blurRadius: 1.0) };
    public virtual double trackHeight => 31.0;
    public virtual double trackWidth => 51.0;
    public virtual Size transitionalThumbSize => new global::Doroti.Ui.Size(28.0, 28.0);
    public virtual long toggleDuration => 140L;
    public virtual double? thumbOffset => DartRuntimePrimitives.ConvertValue<double>(null);
    public virtual Size switchMinSize => new global::Doroti.Ui.Size(Widgets.ConstantsLibrary.kMinInteractiveDimension - 8.0);
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
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _SwitchDefaultsM3__switch(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> thumbColor
    {
        get
        {
            return WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    if (states.Contains(WidgetState.selected))
                    {
                        return _colors.surface.withOpacity(1.0);
                    }
                    return _colors.onSurface.withOpacity(0.38);
                }
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.primaryContainer;
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.primaryContainer;
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.primaryContainer;
                    }
                    return _colors.onPrimary;
                }
                if (states.Contains(WidgetState.pressed))
                {
                    return _colors.onSurfaceVariant;
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return _colors.onSurfaceVariant;
                }
                if (states.Contains(WidgetState.focused))
                {
                    return _colors.onSurfaceVariant;
                }
                return _colors.outline;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> trackColor
    {
        get
        {
            return WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    if (states.Contains(WidgetState.selected))
                    {
                        return _colors.onSurface.withOpacity(0.12);
                    }
                    return _colors.surfaceContainerHighest.withOpacity(0.12);
                }
                if (states.Contains(WidgetState.selected))
                {
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
                if (states.Contains(WidgetState.pressed))
                {
                    return _colors.surfaceContainerHighest;
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return _colors.surfaceContainerHighest;
                }
                if (states.Contains(WidgetState.focused))
                {
                    return _colors.surfaceContainerHighest;
                }
                return _colors.surfaceContainerHighest;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> trackOutlineColor
    {
        get
        {
            return (global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return Colors.transparent;
                }
                if (states.Contains(WidgetState.disabled))
                {
                    return _colors.onSurface.withOpacity(0.12);
                }
                return _colors.outline;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> overlayColor
    {
        get
        {
            return WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.primary.withOpacity(0.1);
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.primary.withOpacity(0.08);
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.primary.withOpacity(0.1);
                    }
                    return null;
                }
                if (states.Contains(WidgetState.pressed))
                {
                    return _colors.onSurface.withOpacity(0.1);
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return _colors.onSurface.withOpacity(0.08);
                }
                if (states.Contains(WidgetState.focused))
                {
                    return _colors.onSurface.withOpacity(0.1);
                }
                return null;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> mouseCursor
    {
        get
        {
            return WidgetStateProperty.resolveWith((states) => WidgetStateMouseCursor.clickable.resolve(states));
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?> trackOutlineWidth => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(2.0));
    public override double? splashRadius => DartRuntimePrimitives.ConvertValue<double>(40.0 / 2L);
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsets.CreateSymmetric(horizontal: 4));
}

internal class _SwitchConfigM3__switch : _SwitchConfig__switch
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;
    public const double iconSize = 16.0;

    internal _SwitchConfigM3__switch(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
        _colors = Theme.of(context).colorScheme;
    }

    public virtual double activeThumbRadius => DartRuntimePrimitives.ConvertValue<double>(24.0 / 2L);
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color> iconColor
    {
        get
        {
            return WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    if (states.Contains(WidgetState.selected))
                    {
                        return _colors.onSurface.withOpacity(0.38);
                    }
                    return _colors.surfaceContainerHighest.withOpacity(0.38);
                }
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onPrimaryContainer;
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onPrimaryContainer;
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onPrimaryContainer;
                    }
                    return _colors.onPrimaryContainer;
                }
                if (states.Contains(WidgetState.pressed))
                {
                    return _colors.surfaceContainerHighest;
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return _colors.surfaceContainerHighest;
                }
                if (states.Contains(WidgetState.focused))
                {
                    return _colors.surfaceContainerHighest;
                }
                return _colors.surfaceContainerHighest;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public virtual double inactiveThumbRadius => DartRuntimePrimitives.ConvertValue<double>(16.0 / 2L);
    public virtual double pressedThumbRadius => DartRuntimePrimitives.ConvertValue<double>(28.0 / 2L);
    public virtual double switchHeight => DartRuntimePrimitives.ConvertValue<double>(switchMinSize.height + 8.0);
    public virtual double switchHeightCollapsed => switchMinSize.height;
    public virtual double switchWidth => 52.0;
    public virtual double thumbRadiusWithIcon => DartRuntimePrimitives.ConvertValue<double>(24.0 / 2L);
    public virtual List<global::Doroti.Framework.Painting.BoxShadow>? thumbShadow => ShadowsLibrary.kElevationToShadow.GetValueOrDefault(0L);
    public virtual double trackHeight => 32.0;
    public virtual double trackWidth => 52.0;
    public virtual Size transitionalThumbSize => new global::Doroti.Ui.Size(34, 22);
    public virtual long toggleDuration => 300L;
    public virtual double? thumbOffset => DartRuntimePrimitives.ConvertValue<double>(null);
    public virtual Size switchMinSize => new global::Doroti.Ui.Size(Widgets.ConstantsLibrary.kMinInteractiveDimension, Widgets.ConstantsLibrary.kMinInteractiveDimension - 8.0);
}
