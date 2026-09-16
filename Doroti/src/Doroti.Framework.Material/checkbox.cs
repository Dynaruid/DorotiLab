// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/checkbox.dart

using Doroti.Runtime;
using Doroti.Ui;

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
        _checkboxType = _CheckboxType__checkbox.material;
        System.Diagnostics.Debug.Assert(tristate || (value is not null));
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
                __late__actionMap = new DartMap<Type, dynamic> { [typeof(ActivateIntent)] = new CallbackAction<ActivateIntent>(onInvoke: (__arg0) => { ((global::System.Action<Intent?>)_handleTap)(__arg0); return default!; }) };
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
        _positionController = new global::Doroti.Framework.Animation.AnimationController(duration: ToggleableLibrary._kToggleDuration, value: (value == false) ? 0.0 : 1.0, vsync: this);
        _position = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _positionController, curve: Curves.easeIn, reverseCurve: Curves.easeOut);
        _reactionController = new global::Doroti.Framework.Animation.AnimationController(duration: _reactionAnimationDuration, vsync: this);
        _reaction = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _reactionController, curve: Curves.fastOutSlowIn);
        _reactionHoverFadeController = new global::Doroti.Framework.Animation.AnimationController(duration: ToggleableLibrary._kReactionFadeDuration, value: (_hovering || _focused) ? 1.0 : 0.0, vsync: this);
        _reactionHoverFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _reactionHoverFadeController, curve: Curves.fastOutSlowIn);
        _reactionFocusFadeController = new global::Doroti.Framework.Animation.AnimationController(duration: ToggleableLibrary._kReactionFadeDuration, value: (_hovering || _focused) ? 1.0 : 0.0, vsync: this);
        _reactionFocusFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _reactionFocusFadeController, curve: Curves.fastOutSlowIn);
        _previousValue = widget.value;
    }

    public override void didUpdateWidget(Checkbox oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.value != widget.value)
        {
            _previousValue = oldWidget.value;
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

    public virtual global::System.Action<bool?>? onChanged => widget.onChanged;
    public virtual bool tristate => widget.tristate;
    public virtual bool? value => widget.value;
    public virtual Duration? reactionAnimationDuration => ConstantsLibrary.kRadialReactionDuration;
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
        if (side is global::Doroti.Framework.Widgets.WidgetStateBorderSide)
        {
            global::Doroti.Framework.Widgets.WidgetStateBorderSide side__as16436 = (global::Doroti.Framework.Widgets.WidgetStateBorderSide)side;
            return WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.BorderSide?>(side__as16436, states);
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
        switch (widget._checkboxType)
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
                                return new CupertinoCheckbox(value: value, tristate: tristate, onChanged: onChanged, mouseCursor: widget.mouseCursor, activeColor: widget.activeColor, checkColor: widget.checkColor, focusColor: widget.focusColor, focusNode: widget.focusNode, autofocus: widget.autofocus, side: widget.side, shape: widget.shape, semanticLabel: widget.semanticLabel);
                            }
                        default:
                            throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                    }
                    break;
                }
        }
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        CheckboxThemeData checkboxTheme = CheckboxTheme.of(context);
        CheckboxThemeData defaults = new _CheckboxDefaultsM3__checkbox(context);
        var defaultCheckColor = defaults.checkColor ?? throw new InvalidOperationException("The built-in checkbox theme must provide checkColor.");
        var defaultFillColor = defaults.fillColor ?? throw new InvalidOperationException("The built-in checkbox theme must provide fillColor.");
        var defaultOverlayColor = defaults.overlayColor ?? throw new InvalidOperationException("The built-in checkbox theme must provide overlayColor.");
        MaterialTapTargetSize effectiveMaterialTapTargetSize = DartRuntimePrimitives.RequireValue(widget.materialTapTargetSize ?? checkboxTheme.materialTapTargetSize ?? defaults.materialTapTargetSize);
        VisualDensity effectiveVisualDensity = (widget.visualDensity ?? checkboxTheme.visualDensity) ?? defaults.visualDensity!;
        global::Doroti.Ui.Size sizeLocal = effectiveMaterialTapTargetSize switch { var __constant18361 when Equals(__constant18361, MaterialTapTargetSize.padded) => new global::Doroti.Ui.Size(ConstantsLibrary.kMinInteractiveDimension, ConstantsLibrary.kMinInteractiveDimension), var __constant18488 when Equals(__constant18488, MaterialTapTargetSize.shrinkWrap) => new global::Doroti.Ui.Size(ConstantsLibrary.kMinInteractiveDimension - 8.0, ConstantsLibrary.kMinInteractiveDimension - 8.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        sizeLocal += effectiveVisualDensity.baseSizeAdjustment;
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> effectiveMouseCursor = WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>((states) =>
        {
            return (WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(widget.mouseCursor, states) ?? (checkboxTheme.mouseCursor?.resolve(states))) ?? WidgetStateMouseCursor.adaptiveClickable.resolve(states);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
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
        if (widget.isError)
        {
            activeStates.Add(WidgetState.error);
            inactiveStates.Add(WidgetState.error);
        }
        global::Doroti.Ui.Color? activeColorLocal = (widget.fillColor?.resolve(activeStates) ?? _widgetFillColor.resolve(activeStates)) ?? (checkboxTheme.fillColor?.resolve(activeStates));
        global::Doroti.Ui.Color effectiveActiveColor = activeColorLocal ?? defaultFillColor.resolve(activeStates)!;
        global::Doroti.Ui.Color? inactiveColorLocal = (widget.fillColor?.resolve(inactiveStates) ?? _widgetFillColor.resolve(inactiveStates)) ?? (checkboxTheme.fillColor?.resolve(inactiveStates));
        global::Doroti.Ui.Color effectiveInactiveColor = inactiveColorLocal ?? defaultFillColor.resolve(inactiveStates)!;
        global::Doroti.Framework.Painting.BorderSide activeSideLocal = (_resolveSide(widget.side, activeStates) ?? _resolveSide(checkboxTheme.side, activeStates)) ?? _resolveSide(defaults.side, activeStates)!;
        global::Doroti.Framework.Painting.BorderSide inactiveSideLocal = (_resolveSide(widget.side, inactiveStates) ?? _resolveSide(checkboxTheme.side, inactiveStates)) ?? _resolveSide(defaults.side, inactiveStates)!;
        HashSet<global::Doroti.Framework.Widgets.WidgetState> focusedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = states;
    __cascade.Add(WidgetState.focused);
    return __cascade;
}))();
        if (widget.isError)
        {
            focusedStates.Add(WidgetState.error);
        }
        global::Doroti.Ui.Color effectiveFocusOverlayColor = ((widget.overlayColor?.resolve(focusedStates) ?? widget.focusColor) ?? (checkboxTheme.overlayColor?.resolve(focusedStates))) ?? defaultOverlayColor.resolve(focusedStates)!;
        HashSet<global::Doroti.Framework.Widgets.WidgetState> hoveredStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = states;
    __cascade.Add(WidgetState.hovered);
    return __cascade;
}))();
        if (widget.isError)
        {
            hoveredStates.Add(WidgetState.error);
        }
        global::Doroti.Ui.Color effectiveHoverOverlayColor = ((widget.overlayColor?.resolve(hoveredStates) ?? widget.hoverColor) ?? (checkboxTheme.overlayColor?.resolve(hoveredStates))) ?? defaultOverlayColor.resolve(hoveredStates)!;
        var activePressedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = activeStates;
    __cascade.Add(WidgetState.pressed);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveActivePressedOverlayColor = ((widget.overlayColor?.resolve(activePressedStates) ?? (checkboxTheme.overlayColor?.resolve(activePressedStates))) ?? activeColorLocal?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? defaultOverlayColor.resolve(activePressedStates)!;
        var inactivePressedStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = inactiveStates;
    __cascade.Add(WidgetState.pressed);
    return __cascade;
}))();
        global::Doroti.Ui.Color effectiveInactivePressedOverlayColor = ((widget.overlayColor?.resolve(inactivePressedStates) ?? (checkboxTheme.overlayColor?.resolve(inactivePressedStates))) ?? inactiveColorLocal?.withAlpha(ConstantsLibrary.kRadialReactionAlpha)) ?? defaultOverlayColor.resolve(inactivePressedStates)!;
        if (downPosition is not null)
        {
            effectiveHoverOverlayColor = states.Contains(WidgetState.selected) ? effectiveActivePressedOverlayColor : effectiveInactivePressedOverlayColor;
            effectiveFocusOverlayColor = states.Contains(WidgetState.selected) ? effectiveActivePressedOverlayColor : effectiveInactivePressedOverlayColor;
        }
        HashSet<global::Doroti.Framework.Widgets.WidgetState> checkStates = widget.isError ? ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{
    var __cascade = states;
    __cascade.Add(WidgetState.error);
    return __cascade;
}))() : states;
        global::Doroti.Ui.Color effectiveCheckColor = (widget.checkColor ?? (checkboxTheme.checkColor?.resolve(checkStates))) ?? defaultCheckColor.resolve(checkStates)!;
        double effectiveSplashRadius = (widget.splashRadius ?? checkboxTheme.splashRadius) ?? DartRuntimePrimitives.RequireValue(defaults.splashRadius);
        return new global::Doroti.Framework.Widgets.Semantics(label: widget.semanticLabel, @checked: widget.value ?? false, mixed: widget.tristate ? (widget.value is null) : null, child: buildToggleable(mouseCursor: effectiveMouseCursor, focusNode: widget.focusNode, autofocus: widget.autofocus, size: sizeLocal, painter: ((Func<_CheckboxPainter__checkbox>)(() =>
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
    __cascade.activeColor = effectiveActiveColor;
    __cascade.inactiveColor = effectiveInactiveColor;
    __cascade.checkColor = effectiveCheckColor;
    __cascade.value = value;
    __cascade.previousValue = _previousValue;
    __cascade.shape = (widget.shape ?? checkboxTheme.shape) ?? defaults.shape!;
    __cascade.activeSide = activeSideLocal;
    __cascade.inactiveSide = inactiveSideLocal;
    return __cascade;
}))()));
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
        return new FocusableActionDetector(actions: _actionMap, focusNode: focusNode, autofocus: autofocus, onFocusChange: onFocusChange, enabled: isInteractive, onShowFocusHighlight: _handleFocusHighlightChanged, onShowHoverHighlight: _handleHoverChanged, mouseCursor: mouseCursor?.resolve(states) ?? SystemMouseCursors.basic, child: new GestureDetector(excludeFromSemantics: !isInteractive, onTapDown: isInteractive ? _handleTapDown : null, onTap: isInteractive ? () => _handleTap(null) : null, onTapUp: isInteractive ? _handleTapEnd : null, onTapCancel: isInteractive ? () => _handleTapEnd(null) : null, child: new global::Doroti.Framework.Widgets.Semantics(enabled: isInteractive, child: child)));
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
        get => _checkColor!;
        set
        {
            var __value = value;
            if (Equals(_checkColor, __value))
            {
                return;
            }
            _checkColor = __value;
            notifyListeners();
        }
    }
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
    public virtual bool? previousValue
    {
        get => _previousValue;
        set
        {
            var __value = value;
            if (_previousValue == __value)
            {
                return;
            }
            _previousValue = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder shape
    {
        get => _shape!;
        set
        {
            var __value = value;
            if (Equals(_shape, __value))
            {
                return;
            }
            _shape = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide activeSide
    {
        get => _activeSide!;
        set
        {
            var __value = value;
            if (Equals(_activeSide, __value))
            {
                return;
            }
            _activeSide = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide inactiveSide
    {
        get => _inactiveSide!;
        set
        {
            var __value = value;
            if (Equals(_inactiveSide, __value))
            {
                return;
            }
            _inactiveSide = __value;
            notifyListeners();
        }
    }
    internal virtual global::Doroti.Ui.Rect _outerRectAt(Offset origin, double t)
    {
        double inset = 1.0 - ((t - 0.5).abs() * 2.0);
        double size = CheckboxLibrary._kEdgeSize - (inset * CheckboxLibrary._kStrokeWidth);
        var rect = Rect.fromLTWH(origin.dx + inset, origin.dy + inset, size, size);
        return rect;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _colorAt(double t)
    {
        return (t >= 0.25) ? activeColor : Dart_uiLibrary.Color.lerp(inactiveColor, activeColor, t * 4.0)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Paint _createStrokePaint()
    {
        return ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = checkColor;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = CheckboxLibrary._kStrokeWidth;
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _drawBox(Canvas canvas, Rect outer, Paint paint, global::Doroti.Framework.Painting.BorderSide? side)
    {
        if (shape.preferPaintInterior)
        {
            shape.paintInterior(canvas, outer, paint);
        }
        else
        {
            canvas.drawPath(shape.getOuterPath(outer), paint);
        }
        if (side is not null)
        {
            shape.copyWith(side: side).paint(canvas, outer);
        }
    }

    internal virtual void _drawCheck(Canvas canvas, Offset origin, double t, Paint paint)
    {
        DartRuntimePrimitives.Assert(() => (t >= 0.0) && (t <= 1.0));
        var path = new global::Doroti.Ui.Path();
        var start = new global::Doroti.Ui.Offset(CheckboxLibrary._kEdgeSize * 0.15, CheckboxLibrary._kEdgeSize * 0.45);
        var mid = new global::Doroti.Ui.Offset(CheckboxLibrary._kEdgeSize * 0.4, CheckboxLibrary._kEdgeSize * 0.7);
        var end = new global::Doroti.Ui.Offset(CheckboxLibrary._kEdgeSize * 0.85, CheckboxLibrary._kEdgeSize * 0.25);
        if (t < 0.5)
        {
            double strokeT = t * 2.0;
            global::Doroti.Ui.Offset drawMid = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(start, mid, strokeT));
            path.moveTo(origin.dx + start.dx, origin.dy + start.dy);
            path.lineTo(origin.dx + drawMid.dx, origin.dy + drawMid.dy);
        }
        else
        {
            double strokeTLocal = (t - 0.5) * 2.0;
            global::Doroti.Ui.Offset drawEnd = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(mid, end, strokeTLocal));
            path.moveTo(origin.dx + start.dx, origin.dy + start.dy);
            path.lineTo(origin.dx + mid.dx, origin.dy + mid.dy);
            path.lineTo(origin.dx + drawEnd.dx, origin.dy + drawEnd.dy);
        }
        canvas.drawPath(path, paint);
    }

    internal virtual void _drawDash(Canvas canvas, Offset origin, double t, Paint paint)
    {
        DartRuntimePrimitives.Assert(() => (t >= 0.0) && (t <= 1.0));
        var start = new global::Doroti.Ui.Offset(CheckboxLibrary._kEdgeSize * 0.2, CheckboxLibrary._kEdgeSize * 0.5);
        var mid = new global::Doroti.Ui.Offset(CheckboxLibrary._kEdgeSize * 0.5, CheckboxLibrary._kEdgeSize * 0.5);
        var end = new global::Doroti.Ui.Offset(CheckboxLibrary._kEdgeSize * 0.8, CheckboxLibrary._kEdgeSize * 0.5);
        global::Doroti.Ui.Offset drawStart = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(start, mid, 1.0 - t));
        global::Doroti.Ui.Offset drawEnd = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(mid, end, t));
        canvas.drawLine(origin + drawStart, origin + drawEnd, paint);
    }

    public override void paint(Canvas canvas, Size size)
    {
        paintRadialReaction(canvas: canvas, origin: size.center(Offset.zero));
        global::Doroti.Ui.Paint strokePaint = _createStrokePaint();
        var originLocal = (size / 2.0) - (new global::Doroti.Ui.Size(CheckboxLibrary._kEdgeSize) / 2.0);
        double tNormalized = position.status switch { AnimationStatus.forward => position.value, AnimationStatus.completed => position.value, AnimationStatus.reverse => 1.0 - position.value, AnimationStatus.dismissed => 1.0 - position.value, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        if ((previousValue == false) || (value == false))
        {
            double t = (value == false) ? (1.0 - tNormalized) : tNormalized;
            global::Doroti.Ui.Rect outer = _outerRectAt(originLocal, t);
            var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = _colorAt(t);
    return __cascade;
}))();
            if (t <= 0.5)
            {
                global::Doroti.Framework.Painting.BorderSide border = BorderSide.lerp(inactiveSide, activeSide, t);
                _drawBox(canvas, outer, paintLocal, border);
            }
            else
            {
                _drawBox(canvas, outer, paintLocal, activeSide);
                double tShrink = (t - 0.5) * 2.0;
                if ((previousValue is null) || (value is null))
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
            global::Doroti.Ui.Rect outerLocal = _outerRectAt(originLocal, 1.0);
            var paintAlternate = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = _colorAt(1.0);
    return __cascade;
}))();
            _drawBox(canvas, outerLocal, paintAlternate, activeSide);
            if (tNormalized <= 0.5)
            {
                double tShrinkLocal = 1.0 - (tNormalized * 2.0);
                if (previousValue ?? false)
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
                double tExpand = (tNormalized - 0.5) * 2.0;
                if (value ?? false)
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

internal class _CheckboxDefaultsM3__checkbox : CheckboxThemeData
{
    internal virtual ThemeData _theme { get; private set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;

    internal _CheckboxDefaultsM3__checkbox(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _theme = Theme.of(context);
        _colors = Theme.of(context).colorScheme;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateBorderSide? side
    {
        get
        {
            return WidgetStateBorderSide.CreateResolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    if (states.Contains(WidgetState.selected))
                    {
                        return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: Colors.transparent);
                    }
                    return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: _colors.onSurface.withOpacity(0.38));
                }
                if (states.Contains(WidgetState.selected))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(width: 0.0, color: Colors.transparent);
                }
                if (states.Contains(WidgetState.error))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: _colors.error);
                }
                if (states.Contains(WidgetState.pressed))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: _colors.onSurface);
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: _colors.onSurface);
                }
                if (states.Contains(WidgetState.focused))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: _colors.onSurface);
                }
                return new global::Doroti.Framework.Painting.BorderSide(width: 2.0, color: _colors.onSurfaceVariant);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> fillColor
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
                    return Colors.transparent;
                }
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.error))
                    {
                        return _colors.error;
                    }
                    return _colors.primary;
                }
                return Colors.transparent;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> checkColor
    {
        get
        {
            return WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    if (states.Contains(WidgetState.selected))
                    {
                        return _colors.surface;
                    }
                    return Colors.transparent;
                }
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.error))
                    {
                        return _colors.onError;
                    }
                    return _colors.onPrimary;
                }
                return Colors.transparent;
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
                if (states.Contains(WidgetState.error))
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.error.withOpacity(0.1);
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.error.withOpacity(0.08);
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.error.withOpacity(0.1);
                    }
                }
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
    public override double? splashRadius => DartRuntimePrimitives.ConvertValue<double>(40.0 / 2L);
    public override MaterialTapTargetSize? materialTapTargetSize => _theme.materialTapTargetSize;
    public override VisualDensity visualDensity => VisualDensity.standard;
    public override global::Doroti.Framework.Painting.OutlinedBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(2.0))));
}
