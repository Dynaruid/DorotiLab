// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/checkbox.dart
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

internal enum _CheckboxType__checkbox
{
    material,
    adaptive
}

public class Checkbox : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool? value { get; private set; }
    public virtual global::System.Action<bool?>? onChanged { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual Color? checkColor { get; private set; }
    public virtual bool tristate { get; private set; } = default!;
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? side { get; private set; }
    public virtual bool isError { get; private set; } = default!;
    public virtual string? semanticLabel { get; private set; }
    public const double width = 18.0;
    internal virtual _CheckboxType__checkbox _checkboxType { get; private set; } = default!;

    public Checkbox(global::Doroti.Framework.Foundation.Key? key = null, bool? value = default!, bool tristate = false, global::System.Action<bool?>? onChanged = default!, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Color? activeColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, Color? checkColor = null, Color? focusColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, MaterialTapTargetSize? materialTapTargetSize = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Framework.Painting.BorderSide? side = null, bool isError = false, string? semanticLabel = null) : base(key: key)
    {
        this.value = value;
        this.tristate = tristate;
        this.onChanged = onChanged;
        this.mouseCursor = mouseCursor;
        this.activeColor = activeColor;
        this.fillColor = fillColor;
        this.checkColor = checkColor;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.overlayColor = overlayColor;
        this.splashRadius = splashRadius;
        this.materialTapTargetSize = materialTapTargetSize;
        this.visualDensity = visualDensity;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.shape = shape;
        this.side = side;
        this.isError = isError;
        this.semanticLabel = semanticLabel;
        this._checkboxType = _CheckboxType__checkbox.material;
        System.Diagnostics.Debug.Assert((tristate || (value is not null)));
    }

    public static Checkbox CreateAdaptive(global::Doroti.Framework.Foundation.Key? key = null, bool? value = default!, bool tristate = false, global::System.Action<bool?>? onChanged = default!, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Color? activeColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, Color? checkColor = null, Color? focusColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, MaterialTapTargetSize? materialTapTargetSize = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Framework.Painting.BorderSide? side = null, bool isError = false, string? semanticLabel = null)
    {
        var __instance = new Checkbox(key: key, value: value, tristate: tristate, onChanged: onChanged, mouseCursor: mouseCursor, activeColor: activeColor, fillColor: fillColor, checkColor: checkColor, focusColor: focusColor, hoverColor: hoverColor, overlayColor: overlayColor, splashRadius: splashRadius, materialTapTargetSize: materialTapTargetSize, visualDensity: visualDensity, focusNode: focusNode, autofocus: autofocus, shape: shape, side: side, isError: isError, semanticLabel: semanticLabel);
        __instance.value = value;
        __instance.tristate = tristate;
        __instance.onChanged = onChanged;
        __instance.mouseCursor = mouseCursor;
        __instance.activeColor = activeColor;
        __instance.fillColor = fillColor;
        __instance.checkColor = checkColor;
        __instance.focusColor = focusColor;
        __instance.hoverColor = hoverColor;
        __instance.overlayColor = overlayColor;
        __instance.splashRadius = splashRadius;
        __instance.materialTapTargetSize = materialTapTargetSize;
        __instance.visualDensity = visualDensity;
        __instance.focusNode = focusNode;
        __instance.autofocus = autofocus;
        __instance.shape = shape;
        __instance.side = side;
        __instance.isError = isError;
        __instance.semanticLabel = semanticLabel;
        __instance._checkboxType = _CheckboxType__checkbox.adaptive;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CheckboxState__checkbox());
}

internal class _CheckboxState__checkbox : global::Doroti.Framework.Widgets.State<Checkbox>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<Checkbox>, global::Doroti.Framework.Widgets.ToggleableStateMixin<Checkbox>
{
    internal virtual _CheckboxPainter__checkbox _painter { get; private set; } = new _CheckboxPainter__checkbox();
    internal virtual bool? _previousValue { get; set; } = default;
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

    public override void initState()
    {
        base.initState();
        _positionController = new global::Doroti.Framework.Animation.AnimationController(duration: global::Doroti.Framework.Widgets.ToggleableLibrary._kToggleDuration, value: ((this.value == false) ? 0.0 : 1.0), vsync: this);
        _position = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._positionController, curve: global::Doroti.Framework.Animation.Curves.easeIn, reverseCurve: global::Doroti.Framework.Animation.Curves.easeOut);
        _reactionController = new global::Doroti.Framework.Animation.AnimationController(duration: this._reactionAnimationDuration, vsync: this);
        _reaction = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._reactionController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        _reactionHoverFadeController = new global::Doroti.Framework.Animation.AnimationController(duration: global::Doroti.Framework.Widgets.ToggleableLibrary._kReactionFadeDuration, value: ((this._hovering || this._focused) ? 1.0 : 0.0), vsync: this);
        _reactionHoverFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._reactionHoverFadeController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        _reactionFocusFadeController = new global::Doroti.Framework.Animation.AnimationController(duration: global::Doroti.Framework.Widgets.ToggleableLibrary._kReactionFadeDuration, value: ((this._hovering || this._focused) ? 1.0 : 0.0), vsync: this);
        _reactionFocusFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._reactionFocusFadeController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        _previousValue = ((Checkbox)this.widget).value;
    }

    public override void didUpdateWidget(Checkbox oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((Checkbox)oldWidget).value != ((Checkbox)this.widget).value))
        {
            _previousValue = ((Checkbox)oldWidget).value;
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

    public virtual global::System.Action<bool?>? onChanged => ((Checkbox)this.widget).onChanged;
    public virtual bool tristate => ((Checkbox)this.widget).tristate;
    public virtual bool? value => ((Checkbox)this.widget).value;
    public virtual Duration? reactionAnimationDuration => ConstantsLibrary.kRadialReactionDuration;
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
                    return ((Checkbox)this.widget).activeColor;
                }
                return null;
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Painting.BorderSide? _resolveSide(global::Doroti.Framework.Painting.BorderSide? side, HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
    {
        if ((side is global::Doroti.Framework.Widgets.WidgetStateBorderSide))
        {
            global::Doroti.Framework.Widgets.WidgetStateBorderSide side__as16436 = (global::Doroti.Framework.Widgets.WidgetStateBorderSide)side;
            return ((global::Doroti.Framework.Painting.BorderSide?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.BorderSide?>(((global::Doroti.Framework.Widgets.WidgetStateBorderSide)side__as16436), states));
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
        switch (((Checkbox)this.widget)._checkboxType)
        {
            case _CheckboxType__checkbox.material:
                {
                    break;
                }
            case _CheckboxType__checkbox.adaptive:
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
                                return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoCheckbox(value: this.value, tristate: this.tristate, onChanged: this.onChanged, mouseCursor: ((Checkbox)this.widget).mouseCursor, activeColor: ((Checkbox)this.widget).activeColor, checkColor: ((Checkbox)this.widget).checkColor, focusColor: ((Checkbox)this.widget).focusColor, focusNode: ((Checkbox)this.widget).focusNode, autofocus: ((Checkbox)this.widget).autofocus, side: ((Checkbox)this.widget).side, shape: ((Checkbox)this.widget).shape, semanticLabel: ((Checkbox)this.widget).semanticLabel));
                            }
                        default:
                            throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                    }
                    break;
                }
        }
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        CheckboxThemeData checkboxTheme = CheckboxTheme.of(context);
        dynamic defaults = (Theme.of(context).useMaterial3 ? new _CheckboxDefaultsM3__checkbox(context) : new _CheckboxDefaultsM2__checkbox(context));
        MaterialTapTargetSize effectiveMaterialTapTargetSize = DartRuntimePrimitives.RequireValue(((Checkbox)this.widget).materialTapTargetSize ?? checkboxTheme.materialTapTargetSize ?? defaults.materialTapTargetSize);
        VisualDensity effectiveVisualDensity = ((((Checkbox)this.widget).visualDensity ?? checkboxTheme.visualDensity) ?? defaults.visualDensity!);
        global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)(object?)(effectiveMaterialTapTargetSize switch { var __constant18361 when (object.Equals(__constant18361, MaterialTapTargetSize.padded)) => new global::Doroti.Ui.Size(ConstantsLibrary.kMinInteractiveDimension, ConstantsLibrary.kMinInteractiveDimension), var __constant18488 when (object.Equals(__constant18488, MaterialTapTargetSize.shrinkWrap)) => new global::Doroti.Ui.Size((ConstantsLibrary.kMinInteractiveDimension - 8.0), (ConstantsLibrary.kMinInteractiveDimension - 8.0)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        sizeLocal += effectiveVisualDensity.baseSizeAdjustment;
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> effectiveMouseCursor = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>((states) =>
        {
            return (((((WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(((Checkbox)this.widget).mouseCursor, states) ?? (global::Doroti.Framework.Services.MouseCursor)checkboxTheme.mouseCursor?.resolve(states))) ?? (global::Doroti.Framework.Services.MouseCursor)global::Doroti.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable.resolve(states))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        HashSet<global::Doroti.Framework.Widgets.WidgetState> activeStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = this.states;
    __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.selected);
    return __cascade;
}))();
        HashSet<global::Doroti.Framework.Widgets.WidgetState> inactiveStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = this.states;
    __cascade.Remove(global::Doroti.Framework.Widgets.WidgetState.selected);
    return __cascade;
}))();
        if (((Checkbox)this.widget).isError)
        {
            activeStates.Add(global::Doroti.Framework.Widgets.WidgetState.error);
            inactiveStates.Add(global::Doroti.Framework.Widgets.WidgetState.error);
        }
        global::Doroti.Ui.Color? activeColorLocal = ((global::Doroti.Ui.Color?)(object?)((((((Checkbox)this.widget).fillColor?.resolve(activeStates) ?? (Color)this._widgetFillColor.resolve(activeStates))) ?? (Color)checkboxTheme.fillColor?.resolve(activeStates))));
        global::Doroti.Ui.Color effectiveActiveColor = ((global::Doroti.Ui.Color)(object?)(activeColorLocal ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.fillColor).resolve(activeStates)!));
        global::Doroti.Ui.Color? inactiveColorLocal = ((global::Doroti.Ui.Color?)(object?)((((((Checkbox)this.widget).fillColor?.resolve(inactiveStates) ?? (Color)this._widgetFillColor.resolve(inactiveStates))) ?? (Color)checkboxTheme.fillColor?.resolve(inactiveStates))));
        global::Doroti.Ui.Color effectiveInactiveColor = ((global::Doroti.Ui.Color)(object?)(inactiveColorLocal ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.fillColor).resolve(inactiveStates)!));
        global::Doroti.Framework.Painting.BorderSide activeSideLocal = (((_resolveSide(((Checkbox)this.widget).side, activeStates) ?? (global::Doroti.Framework.Painting.BorderSide)_resolveSide(checkboxTheme.side, activeStates))) ?? _resolveSide(defaults.side, activeStates)!);
        global::Doroti.Framework.Painting.BorderSide inactiveSideLocal = (((_resolveSide(((Checkbox)this.widget).side, inactiveStates) ?? (global::Doroti.Framework.Painting.BorderSide)_resolveSide(checkboxTheme.side, inactiveStates))) ?? _resolveSide(defaults.side, inactiveStates)!);
        HashSet<global::Doroti.Framework.Widgets.WidgetState> focusedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = this.states;
    __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.focused);
    return __cascade;
}))();
        if (((Checkbox)this.widget).isError)
        {
            focusedStates.Add(global::Doroti.Framework.Widgets.WidgetState.error);
        }
        global::Doroti.Ui.Color effectiveFocusOverlayColor = ((global::Doroti.Ui.Color)(object?)((((((Checkbox)this.widget).overlayColor?.resolve(focusedStates) ?? ((Checkbox)this.widget).focusColor) ?? (Color)checkboxTheme.overlayColor?.resolve(focusedStates))) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.overlayColor).resolve(focusedStates)!));
        HashSet<global::Doroti.Framework.Widgets.WidgetState> hoveredStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = this.states;
    __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.hovered);
    return __cascade;
}))();
        if (((Checkbox)this.widget).isError)
        {
            hoveredStates.Add(global::Doroti.Framework.Widgets.WidgetState.error);
        }
        global::Doroti.Ui.Color effectiveHoverOverlayColor = ((global::Doroti.Ui.Color)(object?)((((((Checkbox)this.widget).overlayColor?.resolve(hoveredStates) ?? ((Checkbox)this.widget).hoverColor) ?? (Color)checkboxTheme.overlayColor?.resolve(hoveredStates))) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.overlayColor).resolve(hoveredStates)!));
        var activePressedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = activeStates;
    __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.pressed);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveActivePressedOverlayColor = ((global::Doroti.Ui.Color)(object?)((((((Checkbox)this.widget).overlayColor?.resolve(activePressedStates) ?? (Color)checkboxTheme.overlayColor?.resolve(activePressedStates))) ?? activeColorLocal?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.overlayColor).resolve(activePressedStates)!));
        var inactivePressedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = inactiveStates;
    __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.pressed);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveInactivePressedOverlayColor = ((global::Doroti.Ui.Color)(object?)((((((Checkbox)this.widget).overlayColor?.resolve(inactivePressedStates) ?? (Color)checkboxTheme.overlayColor?.resolve(inactivePressedStates))) ?? inactiveColorLocal?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.overlayColor).resolve(inactivePressedStates)!));
        if ((this.downPosition is not null))
        {
            effectiveHoverOverlayColor = (this.states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) ? effectiveActivePressedOverlayColor : effectiveInactivePressedOverlayColor);
            effectiveFocusOverlayColor = (this.states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) ? effectiveActivePressedOverlayColor : effectiveInactivePressedOverlayColor);
        }
        HashSet<global::Doroti.Framework.Widgets.WidgetState> checkStates = (((Checkbox)this.widget).isError ? (((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = this.states;
    __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.error);
    return __cascade;
}))()) : this.states);
        global::Doroti.Ui.Color effectiveCheckColor = ((global::Doroti.Ui.Color)(object?)(((((Checkbox)this.widget).checkColor ?? (Color)checkboxTheme.checkColor?.resolve(checkStates))) ?? ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)defaults.checkColor).resolve(checkStates)!));
        double effectiveSplashRadius = ((((Checkbox)this.widget).splashRadius ?? checkboxTheme.splashRadius) ?? DartRuntimePrimitives.RequireValue(defaults.splashRadius));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(label: ((Checkbox)this.widget).semanticLabel, @checked: (((Checkbox)this.widget).value ?? false), mixed: (((Checkbox)this.widget).tristate ? (((Checkbox)this.widget).value is null) : null), child: buildToggleable(mouseCursor: effectiveMouseCursor, focusNode: ((Checkbox)this.widget).focusNode, autofocus: ((Checkbox)this.widget).autofocus, size: sizeLocal, painter: ((Func<_CheckboxPainter__checkbox>)(() =>
{
    var __cascade = this._painter;
    __cascade.position = this.position;
    __cascade.reaction = this.reaction;
    __cascade.reactionFocusFade = this.reactionFocusFade;
    __cascade.reactionHoverFade = this.reactionHoverFade;
    __cascade.inactiveReactionColor = effectiveInactivePressedOverlayColor;
    __cascade.reactionColor = effectiveActivePressedOverlayColor;
    __cascade.hoverColor = effectiveHoverOverlayColor;
    __cascade.focusColor = effectiveFocusOverlayColor;
    __cascade.splashRadius = effectiveSplashRadius;
    __cascade.downPosition = this.downPosition;
    __cascade.isFocused = this.states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused);
    __cascade.isHovered = this.states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered);
    __cascade.activeColor = effectiveActiveColor;
    __cascade.inactiveColor = effectiveInactiveColor;
    __cascade.checkColor = effectiveCheckColor;
    __cascade.value = this.value;
    __cascade.previousValue = this._previousValue;
    __cascade.shape = ((((Checkbox)this.widget).shape ?? checkboxTheme.shape) ?? defaults.shape!);
    __cascade.activeSide = activeSideLocal;
    __cascade.inactiveSide = inactiveSideLocal;
    return __cascade;
}))())));
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
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
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
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
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
            setState(((global::System.Action)(() =>
            {
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
            setState(((global::System.Action)(() =>
            {
                this._downPosition = null;
            })));
        }
        this._reactionController.reverse();
    }

    public virtual void _handleFocusHighlightChanged(bool focused)
    {
        if ((focused != this._focused))
        {
            setState(((global::System.Action)(() =>
            {
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
            setState(((global::System.Action)(() =>
            {
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

public static partial class CheckboxLibrary
{
    internal static double _kEdgeSize = Checkbox.width;
}

public static partial class CheckboxLibrary
{
    internal static double _kStrokeWidth = 2.0;
}

internal class _CheckboxPainter__checkbox : global::Doroti.Framework.Widgets.ToggleablePainter
{
    internal virtual Color? _checkColor { get; set; } = default;
    internal virtual bool? _value { get; set; } = default;
    internal virtual bool? _previousValue { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.OutlinedBorder? _shape { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.BorderSide? _activeSide { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.BorderSide? _inactiveSide { get; set; } = default;

    public virtual global::Doroti.Ui.Color checkColor
    {
        get => this._checkColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this._checkColor, __value)))
            {
                return;
            }
            _checkColor = __value;
            notifyListeners();
        }
    }
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
    public virtual bool? previousValue
    {
        get => this._previousValue;
        set
        {
            var __value = value;
            if ((this._previousValue == __value))
            {
                return;
            }
            _previousValue = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder shape
    {
        get => this._shape!;
        set
        {
            var __value = value;
            if ((object.Equals(this._shape, __value)))
            {
                return;
            }
            _shape = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide activeSide
    {
        get => this._activeSide!;
        set
        {
            var __value = value;
            if ((object.Equals(this._activeSide, __value)))
            {
                return;
            }
            _activeSide = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide inactiveSide
    {
        get => this._inactiveSide!;
        set
        {
            var __value = value;
            if ((object.Equals(this._inactiveSide, __value)))
            {
                return;
            }
            _inactiveSide = __value;
            notifyListeners();
        }
    }
    internal virtual global::Doroti.Ui.Rect _outerRectAt(Offset origin, double t)
    {
        double inset = (1.0 - (((t - 0.5)).abs() * 2.0));
        double size = (CheckboxLibrary._kEdgeSize - (inset * CheckboxLibrary._kStrokeWidth));
        var rect = global::Doroti.Ui.Rect.fromLTWH((origin.dx + inset), (origin.dy + inset), size, size);
        return rect;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _colorAt(double t)
    {
        return ((global::Doroti.Ui.Color)(object?)((t >= 0.25) ? this.activeColor : Dart_uiLibrary.Color.lerp(this.inactiveColor, this.activeColor, (t * 4.0))!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Paint _createStrokePaint()
    {
        return ((global::Doroti.Ui.Paint)(object?)((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.checkColor;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = CheckboxLibrary._kStrokeWidth;
    return __cascade;
}))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _drawBox(Canvas canvas, Rect outer, Paint paint, global::Doroti.Framework.Painting.BorderSide? side)
    {
        if (this.shape.preferPaintInterior)
        {
            this.shape.paintInterior(canvas, outer, paint);
        }
        else
        {
            canvas.drawPath(this.shape.getOuterPath(outer), paint);
        }
        if ((side is not null))
        {
            this.shape.copyWith(side: side).paint(canvas, outer);
        }
    }

    internal virtual void _drawCheck(Canvas canvas, Offset origin, double t, Paint paint)
    {
        DartRuntimePrimitives.Assert(() => ((t >= 0.0) && (t <= 1.0)));
        var path = new global::Doroti.Ui.Path();
        var start = new global::Doroti.Ui.Offset((CheckboxLibrary._kEdgeSize * 0.15), (CheckboxLibrary._kEdgeSize * 0.45));
        var mid = new global::Doroti.Ui.Offset((CheckboxLibrary._kEdgeSize * 0.4), (CheckboxLibrary._kEdgeSize * 0.7));
        var end = new global::Doroti.Ui.Offset((CheckboxLibrary._kEdgeSize * 0.85), (CheckboxLibrary._kEdgeSize * 0.25));
        if ((t < 0.5))
        {
            double strokeT = (t * 2.0);
            global::Doroti.Ui.Offset drawMid = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(start, mid, strokeT)));
            path.moveTo((origin.dx + start.dx), (origin.dy + start.dy));
            path.lineTo((origin.dx + drawMid.dx), (origin.dy + drawMid.dy));
        }
        else
        {
            double strokeTLocal = (((t - 0.5)) * 2.0);
            global::Doroti.Ui.Offset drawEnd = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(mid, end, strokeTLocal)));
            path.moveTo((origin.dx + start.dx), (origin.dy + start.dy));
            path.lineTo((origin.dx + mid.dx), (origin.dy + mid.dy));
            path.lineTo((origin.dx + drawEnd.dx), (origin.dy + drawEnd.dy));
        }
        canvas.drawPath(path, paint);
    }

    internal virtual void _drawDash(Canvas canvas, Offset origin, double t, Paint paint)
    {
        DartRuntimePrimitives.Assert(() => ((t >= 0.0) && (t <= 1.0)));
        var start = new global::Doroti.Ui.Offset((CheckboxLibrary._kEdgeSize * 0.2), (CheckboxLibrary._kEdgeSize * 0.5));
        var mid = new global::Doroti.Ui.Offset((CheckboxLibrary._kEdgeSize * 0.5), (CheckboxLibrary._kEdgeSize * 0.5));
        var end = new global::Doroti.Ui.Offset((CheckboxLibrary._kEdgeSize * 0.8), (CheckboxLibrary._kEdgeSize * 0.5));
        global::Doroti.Ui.Offset drawStart = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(start, mid, (1.0 - t))));
        global::Doroti.Ui.Offset drawEnd = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(mid, end, t)));
        canvas.drawLine((origin + drawStart), (origin + drawEnd), paint);
    }

    public override void paint(Canvas canvas, Size size)
    {
        paintRadialReaction(canvas: canvas, origin: size.center(Offset.zero));
        global::Doroti.Ui.Paint strokePaint = ((global::Doroti.Ui.Paint)(object?)_createStrokePaint());
        var originLocal = ((size / 2.0) - (new global::Doroti.Ui.Size(CheckboxLibrary._kEdgeSize) / 2.0));
        double tNormalized = (((global::Doroti.Framework.Animation.Animation<double>)this.position).status switch { global::Doroti.Framework.Animation.AnimationStatus.forward => ((global::Doroti.Framework.Animation.Animation<double>)this.position).value, global::Doroti.Framework.Animation.AnimationStatus.completed => ((global::Doroti.Framework.Animation.Animation<double>)this.position).value, global::Doroti.Framework.Animation.AnimationStatus.reverse => (1.0 - ((global::Doroti.Framework.Animation.Animation<double>)this.position).value), global::Doroti.Framework.Animation.AnimationStatus.dismissed => (1.0 - ((global::Doroti.Framework.Animation.Animation<double>)this.position).value), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if (((this.previousValue == false) || (this.value == false)))
        {
            double t = ((this.value == false) ? (1.0 - tNormalized) : tNormalized);
            global::Doroti.Ui.Rect outer = ((global::Doroti.Ui.Rect)(object?)_outerRectAt(originLocal, t));
            var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = _colorAt(t);
    return __cascade;
}))();
            if ((t <= 0.5))
            {
                global::Doroti.Framework.Painting.BorderSide border = ((global::Doroti.Framework.Painting.BorderSide)(object?)BorderSide.lerp(this.inactiveSide, this.activeSide, t));
                _drawBox(canvas, outer, paintLocal, border);
            }
            else
            {
                _drawBox(canvas, outer, paintLocal, this.activeSide);
                double tShrink = (((t - 0.5)) * 2.0);
                if (((this.previousValue is null) || (this.value is null)))
                {
                    _drawDash(canvas, originLocal, tShrink, strokePaint);
                }
                else
                {
                    _drawCheck(canvas, originLocal, tShrink, strokePaint);
                }
            }
        }
        else
        {
            global::Doroti.Ui.Rect outerLocal = ((global::Doroti.Ui.Rect)(object?)_outerRectAt(originLocal, 1.0));
            var paintAlternate = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = _colorAt(1.0);
    return __cascade;
}))();
            _drawBox(canvas, outerLocal, paintAlternate, this.activeSide);
            if ((tNormalized <= 0.5))
            {
                double tShrinkLocal = (1.0 - (tNormalized * 2.0));
                if ((this.previousValue ?? false))
                {
                    _drawCheck(canvas, originLocal, tShrinkLocal, strokePaint);
                }
                else
                {
                    _drawDash(canvas, originLocal, tShrinkLocal, strokePaint);
                }
            }
            else
            {
                double tExpand = (((tNormalized - 0.5)) * 2.0);
                if ((this.value ?? false))
                {
                    _drawCheck(canvas, originLocal, tExpand, strokePaint);
                }
                else
                {
                    _drawDash(canvas, originLocal, tExpand, strokePaint);
                }
            }
        }
    }

}

internal class _CheckboxDefaultsM2__checkbox : CheckboxThemeData
{
    internal virtual ThemeData _theme { get; private set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;

    internal _CheckboxDefaultsM2__checkbox(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this._theme = Theme.of(context);
        this._colors = Theme.of(context).colorScheme;
    }

    public virtual global::Doroti.Framework.Widgets.WidgetStateBorderSide? side
    {
        get
        {
            return global::Doroti.Framework.Widgets.WidgetStateBorderSide.CreateResolveWith(((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                    {
                        return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: Colors.transparent);
                    }
                    return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: this._theme.disabledColor);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: Colors.transparent);
                }
                return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: this._theme.unselectedWidgetColor);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> fillColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                    {
                        return (this._theme.disabledColor);
                    }
                    return (Colors.transparent);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return (this._colors.secondary);
                }
                return (Colors.transparent);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> checkColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.all<global::Doroti.Ui.Color>(new global::Doroti.Ui.Color(4294967295L)));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> overlayColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>)(object?)WidgetStateProperty.resolveWith((states) =>
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
    public virtual double splashRadius => ConstantsLibrary.kRadialReactionRadius;
    public virtual MaterialTapTargetSize materialTapTargetSize => this._theme.materialTapTargetSize;
    public virtual VisualDensity visualDensity => this._theme.visualDensity;
    public virtual global::Doroti.Framework.Painting.OutlinedBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(1.0))));
}

internal class _CheckboxDefaultsM3__checkbox : CheckboxThemeData
{
    internal virtual ThemeData _theme { get; private set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;

    internal _CheckboxDefaultsM3__checkbox(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this._theme = Theme.of(context);
        this._colors = Theme.of(context).colorScheme;
    }

    public virtual global::Doroti.Framework.Widgets.WidgetStateBorderSide? side
    {
        get
        {
            return global::Doroti.Framework.Widgets.WidgetStateBorderSide.CreateResolveWith(((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                    {
                        return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: Colors.transparent);
                    }
                    return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: this._colors.onSurface.withOpacity(0.38));
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(width: 0.0, color: Colors.transparent);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.error))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: this._colors.error);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: this._colors.onSurface);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: this._colors.onSurface);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: this._colors.onSurface);
                }
                return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: this._colors.onSurfaceVariant);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> fillColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                    {
                        return (this._colors.onSurface.withOpacity(0.38));
                    }
                    return (Colors.transparent);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.error))
                    {
                        return (this._colors.error);
                    }
                    return (this._colors.primary);
                }
                return (Colors.transparent);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> checkColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                    {
                        return (this._colors.surface);
                    }
                    return (Colors.transparent);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.error))
                    {
                        return (this._colors.onError);
                    }
                    return (this._colors.onPrimary);
                }
                return (Colors.transparent);
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
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.error))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                    {
                        return (this._colors.error.withOpacity(0.1));
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                    {
                        return (this._colors.error.withOpacity(0.08));
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                    {
                        return (this._colors.error.withOpacity(0.1));
                    }
                }
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
    public virtual double splashRadius => DartRuntimePrimitives.ConvertValue<double>((40.0 / 2L));
    public virtual MaterialTapTargetSize materialTapTargetSize => this._theme.materialTapTargetSize;
    public virtual VisualDensity visualDensity => VisualDensity.standard;
    public virtual global::Doroti.Framework.Painting.OutlinedBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(2.0))));
}
