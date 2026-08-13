// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/checkbox.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

internal enum _CheckboxType__checkbox
{
    material,
    adaptive
}

public class Checkbox : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual bool? value { get; private set; }
    public virtual global::System.Action<bool?>? onChanged { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual Color? checkColor { get; private set; }
    public virtual bool tristate { get; private set; } = default!;
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide? side { get; private set; }
    public virtual bool isError { get; private set; } = default!;
    public virtual string? semanticLabel { get; private set; }
    public const double width = 18.0;
    internal virtual _CheckboxType__checkbox _checkboxType { get; private set; } = default!;

    public Checkbox(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool? value = default!, bool tristate = false, global::System.Action<bool?>? onChanged = default!, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, Color? activeColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, Color? checkColor = null, Color? focusColor = null, Color? hoverColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, MaterialTapTargetSize? materialTapTargetSize = null, VisualDensity? visualDensity = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Generated.Framework.Painting.BorderSide? side = null, bool isError = false, string? semanticLabel = null) : base(key: key)
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

    public static Checkbox CreateAdaptive(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool? value = default!, bool tristate = false, global::System.Action<bool?>? onChanged = default!, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, Color? activeColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, Color? checkColor = null, Color? focusColor = null, Color? hoverColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, MaterialTapTargetSize? materialTapTargetSize = null, VisualDensity? visualDensity = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Generated.Framework.Painting.BorderSide? side = null, bool isError = false, string? semanticLabel = null)
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

internal class _CheckboxState__checkbox : global::Doroti.Generated.Framework.Widgets.State<Checkbox>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<Checkbox>, global::Doroti.Generated.Framework.Widgets.ToggleableStateMixin<Checkbox>
{
    internal virtual _CheckboxPainter__checkbox _painter { get; private set; } = new _CheckboxPainter__checkbox();
    internal virtual bool? _previousValue { get; set; } = default;
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController _positionController { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _position { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController _reactionController { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _reaction { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _reactionHoverFade { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController _reactionHoverFadeController { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _reactionFocusFade { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController _reactionFocusFadeController { get; set; } = default!;
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
        _positionController = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: global::Doroti.Generated.Framework.Widgets.ToggleableLibrary._kToggleDuration, value: ((this.value == false) ? 0.0 : 1.0), vsync: this);
        _position = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._positionController, curve: global::Doroti.Generated.Framework.Animation.Curves.easeIn, reverseCurve: global::Doroti.Generated.Framework.Animation.Curves.easeOut);
        _reactionController = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: this._reactionAnimationDuration, vsync: this);
        _reaction = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._reactionController, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn);
        _reactionHoverFadeController = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: global::Doroti.Generated.Framework.Widgets.ToggleableLibrary._kReactionFadeDuration, value: ((this._hovering || this._focused) ? 1.0 : 0.0), vsync: this);
        _reactionHoverFade = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._reactionHoverFadeController, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn);
        _reactionFocusFadeController = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: global::Doroti.Generated.Framework.Widgets.ToggleableLibrary._kReactionFadeDuration, value: ((this._hovering || this._focused) ? 1.0 : 0.0), vsync: this);
        _reactionFocusFade = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._reactionFocusFadeController, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn);
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
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?> _widgetFillColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color?>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, Color?>)((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return null;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return ((Checkbox)this.widget).activeColor;
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            return default!;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Painting.BorderSide? _resolveSide(global::Doroti.Generated.Framework.Painting.BorderSide? side, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)
    {
        if ((side is global::Doroti.Generated.Framework.Widgets.WidgetStateBorderSide))
        {
            global::Doroti.Generated.Framework.Widgets.WidgetStateBorderSide side__as16436 = (global::Doroti.Generated.Framework.Widgets.WidgetStateBorderSide)side;
            return ((global::Doroti.Generated.Framework.Painting.BorderSide?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Painting.BorderSide?>(((global::Doroti.Generated.Framework.Widgets.WidgetStateBorderSide)side__as16436), states));
        }
        if (!states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
        {
            return side;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        switch (((Checkbox)this.widget)._checkboxType)
        {
            case _CheckboxType__checkbox.material:
                {
                    break;
                }
            case _CheckboxType__checkbox.adaptive:
                {
                    ThemeData theme__16840 = Theme.of(context);
                    switch (theme__16840.platform)
                    {
                        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                            {
                                break;
                            }
                        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
                        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                            {
                                return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoCheckbox(value: this.value, tristate: this.tristate, onChanged: this.onChanged, mouseCursor: ((Checkbox)this.widget).mouseCursor, activeColor: ((Checkbox)this.widget).activeColor, checkColor: ((Checkbox)this.widget).checkColor, focusColor: ((Checkbox)this.widget).focusColor, focusNode: ((Checkbox)this.widget).focusNode, autofocus: ((Checkbox)this.widget).autofocus, side: ((Checkbox)this.widget).side, shape: ((Checkbox)this.widget).shape, semanticLabel: ((Checkbox)this.widget).semanticLabel));
                            }
                        default:
                            throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                    }
                    break;
                }
        }
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        CheckboxThemeData checkboxTheme__17775 = CheckboxTheme.of(context);
        dynamic defaults__17846 = (Theme.of(context).useMaterial3 ? new _CheckboxDefaultsM3__checkbox(context) : new _CheckboxDefaultsM2__checkbox(context));
        MaterialTapTargetSize effectiveMaterialTapTargetSize__17999 = DartRuntimePrimitives.RequireValue(((Checkbox)this.widget).materialTapTargetSize ?? checkboxTheme__17775.materialTapTargetSize ?? defaults__17846.materialTapTargetSize);
        VisualDensity effectiveVisualDensity__18184 = ((((Checkbox)this.widget).visualDensity ?? checkboxTheme__17775.visualDensity) ?? defaults__17846.visualDensity!);
        global::Doroti.Flutter.Ui.Size size__18306 = ((global::Doroti.Flutter.Ui.Size)(object?)(effectiveMaterialTapTargetSize__17999 switch { var __constant18361 when (object.Equals(__constant18361, MaterialTapTargetSize.padded)) => new global::Doroti.Flutter.Ui.Size(ConstantsLibrary.kMinInteractiveDimension, ConstantsLibrary.kMinInteractiveDimension), var __constant18488 when (object.Equals(__constant18488, MaterialTapTargetSize.shrinkWrap)) => new global::Doroti.Flutter.Ui.Size((ConstantsLibrary.kMinInteractiveDimension - 8.0), (ConstantsLibrary.kMinInteractiveDimension - 8.0)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        size__18306 += effectiveVisualDensity__18184.baseSizeAdjustment;
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor> effectiveMouseCursor__18731 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Services.MouseCursor>((states) => {
return (((((WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor?>(((Checkbox)this.widget).mouseCursor, states) ?? (global::Doroti.Generated.Framework.Services.MouseCursor)checkboxTheme__17775.mouseCursor?.resolve(states))) ?? (global::Doroti.Generated.Framework.Services.MouseCursor)global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable.resolve(states))));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> activeStates__19180 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.selected);
            return __cascade;        }))();
        HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> inactiveStates__19257 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Remove(global::Doroti.Generated.Framework.Widgets.WidgetState.selected);
            return __cascade;        }))();
        if (((Checkbox)this.widget).isError)
        {
            activeStates__19180.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.error);
            inactiveStates__19257.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.error);
        }
        global::Doroti.Flutter.Ui.Color? activeColor__19449 = ((global::Doroti.Flutter.Ui.Color?)(object?)((((((Checkbox)this.widget).fillColor?.resolve(activeStates__19180) ?? (Color)this._widgetFillColor.resolve(activeStates__19180))) ?? (Color)checkboxTheme__17775.fillColor?.resolve(activeStates__19180))));
        global::Doroti.Flutter.Ui.Color effectiveActiveColor__19636 = ((global::Doroti.Flutter.Ui.Color)(object?)(activeColor__19449 ?? ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)defaults__17846.fillColor).resolve(activeStates__19180)!));
        global::Doroti.Flutter.Ui.Color? inactiveColor__19735 = ((global::Doroti.Flutter.Ui.Color?)(object?)((((((Checkbox)this.widget).fillColor?.resolve(inactiveStates__19257) ?? (Color)this._widgetFillColor.resolve(inactiveStates__19257))) ?? (Color)checkboxTheme__17775.fillColor?.resolve(inactiveStates__19257))));
        global::Doroti.Flutter.Ui.Color effectiveInactiveColor__19930 = ((global::Doroti.Flutter.Ui.Color)(object?)(inactiveColor__19735 ?? ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)defaults__17846.fillColor).resolve(inactiveStates__19257)!));
        global::Doroti.Generated.Framework.Painting.BorderSide activeSide__20048 = (((_resolveSide(((Checkbox)this.widget).side, activeStates__19180) ?? (global::Doroti.Generated.Framework.Painting.BorderSide)_resolveSide(checkboxTheme__17775.side, activeStates__19180))) ?? _resolveSide(defaults__17846.side, activeStates__19180)!);
        global::Doroti.Generated.Framework.Painting.BorderSide inactiveSide__20243 = (((_resolveSide(((Checkbox)this.widget).side, inactiveStates__19257) ?? (global::Doroti.Generated.Framework.Painting.BorderSide)_resolveSide(checkboxTheme__17775.side, inactiveStates__19257))) ?? _resolveSide(defaults__17846.side, inactiveStates__19257)!);
        HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> focusedStates__20453 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.focused);
            return __cascade;        }))();
        if (((Checkbox)this.widget).isError)
        {
            focusedStates__20453.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.error);
        }
        global::Doroti.Flutter.Ui.Color effectiveFocusOverlayColor__20589 = ((global::Doroti.Flutter.Ui.Color)(object?)((((((Checkbox)this.widget).overlayColor?.resolve(focusedStates__20453) ?? ((Checkbox)this.widget).focusColor) ?? (Color)checkboxTheme__17775.overlayColor?.resolve(focusedStates__20453))) ?? ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)defaults__17846.overlayColor).resolve(focusedStates__20453)!));
        HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> hoveredStates__20848 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered);
            return __cascade;        }))();
        if (((Checkbox)this.widget).isError)
        {
            hoveredStates__20848.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.error);
        }
        global::Doroti.Flutter.Ui.Color effectiveHoverOverlayColor__20984 = ((global::Doroti.Flutter.Ui.Color)(object?)((((((Checkbox)this.widget).overlayColor?.resolve(hoveredStates__20848) ?? ((Checkbox)this.widget).hoverColor) ?? (Color)checkboxTheme__17775.overlayColor?.resolve(hoveredStates__20848))) ?? ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)defaults__17846.overlayColor).resolve(hoveredStates__20848)!));
        var activePressedStates__21226 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = activeStates__19180;
            __cascade.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed);
            return __cascade;        }))();
        global::Doroti.Flutter.Ui.Color effectiveActivePressedOverlayColor__21304 = ((global::Doroti.Flutter.Ui.Color)(object?)((((((Checkbox)this.widget).overlayColor?.resolve(activePressedStates__21226) ?? (Color)checkboxTheme__17775.overlayColor?.resolve(activePressedStates__21226))) ?? activeColor__19449?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)defaults__17846.overlayColor).resolve(activePressedStates__21226)!));
        var inactivePressedStates__21599 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = inactiveStates__19257;
            __cascade.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed);
            return __cascade;        }))();
        global::Doroti.Flutter.Ui.Color effectiveInactivePressedOverlayColor__21681 = ((global::Doroti.Flutter.Ui.Color)(object?)((((((Checkbox)this.widget).overlayColor?.resolve(inactivePressedStates__21599) ?? (Color)checkboxTheme__17775.overlayColor?.resolve(inactivePressedStates__21599))) ?? inactiveColor__19735?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)defaults__17846.overlayColor).resolve(inactivePressedStates__21599)!));
        if ((this.downPosition is not null))
        {
            effectiveHoverOverlayColor__20984 = (this.states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected) ? effectiveActivePressedOverlayColor__21304 : effectiveInactivePressedOverlayColor__21681);
            effectiveFocusOverlayColor__20589 = (this.states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected) ? effectiveActivePressedOverlayColor__21304 : effectiveInactivePressedOverlayColor__21681);
        }
        HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> checkStates__22382 = (((Checkbox)this.widget).isError ? (((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.error);
            return __cascade;        }))()) : this.states);
        global::Doroti.Flutter.Ui.Color effectiveCheckColor__22472 = ((global::Doroti.Flutter.Ui.Color)(object?)(((((Checkbox)this.widget).checkColor ?? (Color)checkboxTheme__17775.checkColor?.resolve(checkStates__22382))) ?? ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)defaults__17846.checkColor).resolve(checkStates__22382)!));
        double effectiveSplashRadius__22651 = ((((Checkbox)this.widget).splashRadius ?? checkboxTheme__17775.splashRadius) ?? DartRuntimePrimitives.RequireValue(defaults__17846.splashRadius));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(label: ((Checkbox)this.widget).semanticLabel, @checked: (((Checkbox)this.widget).value ?? false), mixed: (((Checkbox)this.widget).tristate ? (((Checkbox)this.widget).value is null) : null), child: buildToggleable(mouseCursor: effectiveMouseCursor__18731, focusNode: ((Checkbox)this.widget).focusNode, autofocus: ((Checkbox)this.widget).autofocus, size: size__18306, painter: ((Func<_CheckboxPainter__checkbox>)(() =>
{            var __cascade = this._painter;
            __cascade.position = this.position;
            __cascade.reaction = this.reaction;
            __cascade.reactionFocusFade = this.reactionFocusFade;
            __cascade.reactionHoverFade = this.reactionHoverFade;
            __cascade.inactiveReactionColor = effectiveInactivePressedOverlayColor__21681;
            __cascade.reactionColor = effectiveActivePressedOverlayColor__21304;
            __cascade.hoverColor = effectiveHoverOverlayColor__20984;
            __cascade.focusColor = effectiveFocusOverlayColor__20589;
            __cascade.splashRadius = effectiveSplashRadius__22651;
            __cascade.downPosition = this.downPosition;
            __cascade.isFocused = this.states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused);
            __cascade.isHovered = this.states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered);
            __cascade.activeColor = effectiveActiveColor__19636;
            __cascade.inactiveColor = effectiveInactiveColor__19930;
            __cascade.checkColor = effectiveCheckColor__22472;
            __cascade.value = this.value;
            __cascade.previousValue = this._previousValue;
            __cascade.shape = ((((Checkbox)this.widget).shape ?? checkboxTheme__17775.shape) ?? defaults__17846.shape!);
            __cascade.activeSide = activeSide__20048;
            __cascade.inactiveSide = inactiveSide__20243;
            return __cascade;        }))())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

    public virtual global::Doroti.Generated.Framework.Animation.AnimationController positionController => this._positionController;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation position => this._position;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController reactionController => this._reactionController;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation reaction => this._reaction;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation reactionHoverFade => this._reactionHoverFade;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation reactionFocusFade => this._reactionFocusFade;
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
    public virtual void _handleTapDown(global::Doroti.Generated.Framework.Gestures.TapDownDetails details)
    {
        if (this.isInteractive)
        {
            setState(((global::System.Action)(() => {
this._downPosition = ((global::Doroti.Generated.Framework.Gestures.TapDownDetails)details).localPosition;
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
        ((dynamic)this.context.findRenderObject()!).sendSemanticsEvent(new global::Doroti.Generated.Framework.Semantics.TapSemanticEvent());
    }

    public virtual void _handleTapEnd(global::Doroti.Generated.Framework.Gestures.TapUpDetails? __unused0 = null)
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
    public virtual Widget buildToggleable(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor>? mouseCursor = null, Size size = default!, global::Doroti.Generated.Framework.Widgets.ToggleablePainter painter = default!)
    {
        return ((Widget)(object?)buildToggleableWithChild(focusNode: focusNode, onFocusChange: (global::System.Action<bool>?)onFocusChange, autofocus: autofocus, mouseCursor: mouseCursor, child: new CustomPaint(size: size, painter: painter)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildToggleableWithChild(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor>? mouseCursor = null, Widget child = default!)
    {
        return ((Widget)(object?)new FocusableActionDetector(actions: this._actionMap, focusNode: focusNode, autofocus: autofocus, onFocusChange: (global::System.Action<bool>?)onFocusChange, enabled: this.isInteractive, onShowFocusHighlight: (global::System.Action<bool>)this._handleFocusHighlightChanged, onShowHoverHighlight: (global::System.Action<bool>)this._handleHoverChanged, mouseCursor: (mouseCursor?.resolve(this.states) ?? global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic), child: new GestureDetector(excludeFromSemantics: !this.isInteractive, onTapDown: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>)(this.isInteractive ? this._handleTapDown : null)), onTap: () => ((global::System.Action<Intent?>)(this.isInteractive ? this._handleTap : null))(default), onTapUp: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails?>)(this.isInteractive ? this._handleTapEnd : null)), onTapCancel: () => ((global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails?>)(this.isInteractive ? this._handleTapEnd : null))(default), child: new global::Doroti.Generated.Framework.Widgets.Semantics(enabled: this.isInteractive, child: child))));
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

internal class _CheckboxPainter__checkbox : global::Doroti.Generated.Framework.Widgets.ToggleablePainter
{
    internal virtual Color? _checkColor { get; set; } = default;
    internal virtual bool? _value { get; set; } = default;
    internal virtual bool? _previousValue { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.OutlinedBorder? _shape { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.BorderSide? _activeSide { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.BorderSide? _inactiveSide { get; set; } = default;

    public virtual global::Doroti.Flutter.Ui.Color checkColor
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
    public virtual global::Doroti.Generated.Framework.Painting.OutlinedBorder shape
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
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide activeSide
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
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide inactiveSide
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
    internal virtual global::Doroti.Flutter.Ui.Rect _outerRectAt(Offset origin, double t)
    {
        double inset__25755 = (1.0 - (((t - 0.5)).abs() * 2.0));
        double size__25809 = (CheckboxLibrary._kEdgeSize - (inset__25755 * CheckboxLibrary._kStrokeWidth));
        var rect__25862 = global::Doroti.Flutter.Ui.Rect.fromLTWH((origin.dx + inset__25755), (origin.dy + inset__25755), size__25809, size__25809);
        return rect__25862;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Flutter.Ui.Color _colorAt(double t)
    {
        return ((global::Doroti.Flutter.Ui.Color)(object?)((t >= 0.25) ? this.activeColor : Dart_uiLibrary.Color.lerp(this.inactiveColor, this.activeColor, (t * 4.0))!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Flutter.Ui.Paint _createStrokePaint()
    {
        return ((global::Doroti.Flutter.Ui.Paint)(object?)((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.color = this.checkColor;
            __cascade.style = PaintingStyle.stroke;
            __cascade.strokeWidth = CheckboxLibrary._kStrokeWidth;
            return __cascade;        }))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _drawBox(Canvas canvas, Rect outer, Paint paint, global::Doroti.Generated.Framework.Painting.BorderSide? side)
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
        var path__26957 = new global::Doroti.Flutter.Ui.Path();
        var start__26982 = new global::Doroti.Flutter.Ui.Offset((CheckboxLibrary._kEdgeSize * 0.15), (CheckboxLibrary._kEdgeSize * 0.45));
        var mid__27046 = new global::Doroti.Flutter.Ui.Offset((CheckboxLibrary._kEdgeSize * 0.4), (CheckboxLibrary._kEdgeSize * 0.7));
        var end__27106 = new global::Doroti.Flutter.Ui.Offset((CheckboxLibrary._kEdgeSize * 0.85), (CheckboxLibrary._kEdgeSize * 0.25));
        if ((t < 0.5))
        {
            double strokeT__27196 = (t * 2.0);
            global::Doroti.Flutter.Ui.Offset drawMid__27234 = ((global::Doroti.Flutter.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(start__26982, mid__27046, strokeT__27196)));
            path__26957.moveTo((origin.dx + start__26982.dx), (origin.dy + start__26982.dy));
            path__26957.lineTo((origin.dx + drawMid__27234.dx), (origin.dy + drawMid__27234.dy));
        }
        else
        {
            double strokeT__27441 = (((t - 0.5)) * 2.0);
            global::Doroti.Flutter.Ui.Offset drawEnd__27487 = ((global::Doroti.Flutter.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(mid__27046, end__27106, strokeT__27441)));
            path__26957.moveTo((origin.dx + start__26982.dx), (origin.dy + start__26982.dy));
            path__26957.lineTo((origin.dx + mid__27046.dx), (origin.dy + mid__27046.dy));
            path__26957.lineTo((origin.dx + drawEnd__27487.dx), (origin.dy + drawEnd__27487.dy));
        }
        canvas.drawPath(path__26957, paint);
    }

    internal virtual void _drawDash(Canvas canvas, Offset origin, double t, Paint paint)
    {
        DartRuntimePrimitives.Assert(() => ((t >= 0.0) && (t <= 1.0)));
        var start__27978 = new global::Doroti.Flutter.Ui.Offset((CheckboxLibrary._kEdgeSize * 0.2), (CheckboxLibrary._kEdgeSize * 0.5));
        var mid__28040 = new global::Doroti.Flutter.Ui.Offset((CheckboxLibrary._kEdgeSize * 0.5), (CheckboxLibrary._kEdgeSize * 0.5));
        var end__28100 = new global::Doroti.Flutter.Ui.Offset((CheckboxLibrary._kEdgeSize * 0.8), (CheckboxLibrary._kEdgeSize * 0.5));
        global::Doroti.Flutter.Ui.Offset drawStart__28167 = ((global::Doroti.Flutter.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(start__27978, mid__28040, (1.0 - t))));
        global::Doroti.Flutter.Ui.Offset drawEnd__28231 = ((global::Doroti.Flutter.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(mid__28040, end__28100, t)));
        canvas.drawLine((origin + drawStart__28167), (origin + drawEnd__28231), paint);
    }

    public virtual void paint(Canvas canvas, Size size)
    {
        paintRadialReaction(canvas: canvas, origin: size.center(Offset.zero));
        global::Doroti.Flutter.Ui.Paint strokePaint__28484 = ((global::Doroti.Flutter.Ui.Paint)(object?)_createStrokePaint());
        var origin__28530 = ((size / 2.0) - (new global::Doroti.Flutter.Ui.Size(CheckboxLibrary._kEdgeSize) / 2.0));
        double tNormalized__28616 = (((global::Doroti.Generated.Framework.Animation.Animation<double>)this.position).status switch { global::Doroti.Generated.Framework.Animation.AnimationStatus.forward => ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.position).value, global::Doroti.Generated.Framework.Animation.AnimationStatus.completed => ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.position).value, global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse => (1.0 - ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.position).value), global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed => (1.0 - ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.position).value), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if (((this.previousValue == false) || (this.value == false)))
        {
            double t__28976 = ((this.value == false) ? (1.0 - tNormalized__28616) : tNormalized__28616);
            global::Doroti.Flutter.Ui.Rect outer__29047 = ((global::Doroti.Flutter.Ui.Rect)(object?)_outerRectAt(origin__28530, t__28976));
            var paint__29092 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.color = _colorAt(t__28976);
            return __cascade;        }))();
            if ((t__28976 <= 0.5))
            {
                global::Doroti.Generated.Framework.Painting.BorderSide border__29178 = ((global::Doroti.Generated.Framework.Painting.BorderSide)(object?)BorderSide.lerp(this.inactiveSide, this.activeSide, t__28976));
                _drawBox(canvas, outer__29047, paint__29092, border__29178);
            }
            else
            {
                _drawBox(canvas, outer__29047, paint__29092, this.activeSide);
                double tShrink__29369 = (((t__28976 - 0.5)) * 2.0);
                if (((this.previousValue is null) || (this.value is null)))
                {
                    _drawDash(canvas, origin__28530, tShrink__29369, strokePaint__28484);
                }
                else
                {
                    _drawCheck(canvas, origin__28530, tShrink__29369, strokePaint__28484);
                }
            }
        }
        else
        {
            global::Doroti.Flutter.Ui.Rect outer__29681 = ((global::Doroti.Flutter.Ui.Rect)(object?)_outerRectAt(origin__28530, 1.0));
            var paint__29728 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.color = _colorAt(1.0);
            return __cascade;        }))();
            _drawBox(canvas, outer__29681, paint__29728, this.activeSide);
            if ((tNormalized__28616 <= 0.5))
            {
                double tShrink__29872 = (1.0 - (tNormalized__28616 * 2.0));
                if ((this.previousValue ?? false))
                {
                    _drawCheck(canvas, origin__28530, tShrink__29872, strokePaint__28484);
                }
                else
                {
                    _drawDash(canvas, origin__28530, tShrink__29872, strokePaint__28484);
                }
            }
            else
            {
                double tExpand__30127 = (((tNormalized__28616 - 0.5)) * 2.0);
                if ((this.value ?? false))
                {
                    _drawCheck(canvas, origin__28530, tExpand__30127, strokePaint__28484);
                }
                else
                {
                    _drawDash(canvas, origin__28530, tExpand__30127, strokePaint__28484);
                }
            }
        }
    }

}

internal class _CheckboxDefaultsM2__checkbox : CheckboxThemeData
{
    internal virtual ThemeData _theme { get; private set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;

    internal _CheckboxDefaultsM2__checkbox(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        this._theme = Theme.of(context);
        this._colors = Theme.of(context).colorScheme;
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateBorderSide? side
    {
        get
        {
            return global::Doroti.Generated.Framework.Widgets.WidgetStateBorderSide.CreateResolveWith(((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
    {
        return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 2.0, color: Colors.transparent);
    }
    return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 2.0, color: this._theme.disabledColor);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 2.0, color: Colors.transparent);
}
return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 2.0, color: this._theme.unselectedWidgetColor);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color> fillColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
    {
        return (this._theme.disabledColor);
    }
    return (Colors.transparent);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (this._colors.secondary);
}
return (Colors.transparent);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color> checkColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.all<global::Doroti.Flutter.Ui.Color>(new global::Doroti.Flutter.Ui.Color(4294967295L)));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?> overlayColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this.fillColor.resolve(states).withAlpha(ConstantsLibrary.kRadialReactionAlpha));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._theme.hoverColor);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
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
    public virtual global::Doroti.Generated.Framework.Painting.OutlinedBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(1.0))));
}

internal class _CheckboxDefaultsM3__checkbox : CheckboxThemeData
{
    internal virtual ThemeData _theme { get; private set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;

    internal _CheckboxDefaultsM3__checkbox(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        this._theme = Theme.of(context);
        this._colors = Theme.of(context).colorScheme;
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateBorderSide? side
    {
        get
        {
            return global::Doroti.Generated.Framework.Widgets.WidgetStateBorderSide.CreateResolveWith(((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
    {
        return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 2.0, color: Colors.transparent);
    }
    return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 2.0, color: this._colors.onSurface.withOpacity(0.38));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 0.0, color: Colors.transparent);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.error))
{
    return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 2.0, color: this._colors.error);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 2.0, color: this._colors.onSurface);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 2.0, color: this._colors.onSurface);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 2.0, color: this._colors.onSurface);
}
return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 2.0, color: this._colors.onSurfaceVariant);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color> fillColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
    {
        return (this._colors.onSurface.withOpacity(0.38));
    }
    return (Colors.transparent);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.error))
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
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color> checkColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
    {
        return (this._colors.surface);
    }
    return (Colors.transparent);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.error))
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
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color> overlayColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.error))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (this._colors.error.withOpacity(0.1));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (this._colors.error.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (this._colors.error.withOpacity(0.1));
    }
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (this._colors.onSurface.withOpacity(0.1));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (this._colors.primary.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (this._colors.primary.withOpacity(0.1));
    }
    return (Colors.transparent);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.primary.withOpacity(0.1));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onSurface.withOpacity(0.08));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
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
    public virtual global::Doroti.Generated.Framework.Painting.OutlinedBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(2.0))));
}
