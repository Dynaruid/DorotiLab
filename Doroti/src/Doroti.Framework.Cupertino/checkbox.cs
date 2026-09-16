// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/checkbox.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class CheckboxLibrary
{
    internal static Color _kDisabledCheckColor = new CupertinoDynamicColor(color: Color.fromARGB(64L, 0L, 0L, 0L), darkColor: Color.fromARGB(64L, 255L, 255L, 255L));
}

public static partial class CheckboxLibrary
{
    internal static Color _kDisabledBorderColor = new CupertinoDynamicColor(color: Color.fromARGB(13L, 0L, 0L, 0L), darkColor: Color.fromARGB(13L, 0L, 0L, 0L));
}

public static partial class CheckboxLibrary
{
    internal static CupertinoDynamicColor _kDefaultBorderColor = new CupertinoDynamicColor(color: Color.fromARGB(255L, 209L, 209L, 214L), darkColor: Color.fromARGB(50L, 128L, 128L, 128L));
}

public static partial class CheckboxLibrary
{
    internal static CupertinoDynamicColor _kDefaultFillColor = new CupertinoDynamicColor(color: CupertinoColors.activeBlue, darkColor: Color.fromARGB(255L, 50L, 100L, 215L));
}

public static partial class CheckboxLibrary
{
    internal static Color _kDefaultCheckColor = new CupertinoDynamicColor(color: CupertinoColors.white, darkColor: Color.fromARGB(255L, 222L, 232L, 248L));
}

public static partial class CheckboxLibrary
{
    internal static double _kPressedOverlayOpacity = 0.15;
}

public static partial class CheckboxLibrary
{
    internal static List<double> _kDarkGradientOpacities = new List<double> { 0.14, 0.29 };
}

public static partial class CheckboxLibrary
{
    internal static List<double> _kDisabledDarkGradientOpacities = new List<double> { 0.08, 0.14 };
}

public class CupertinoCheckbox : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool? value { get; private set; }
    public virtual global::System.Action<bool?>? onChanged { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual Color? inactiveColor { get; private set; }
    public virtual Color? checkColor { get; private set; }
    public virtual bool tristate { get; private set; } = default!;
    public virtual Color? focusColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderSide? side { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual Size? tapTargetSize { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public const double width = 14.0;

    public CupertinoCheckbox(global::Doroti.Framework.Foundation.Key? key = null, bool? value = default!, bool tristate = false, global::System.Action<bool?>? onChanged = default!, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Color? activeColor = null, Color? inactiveColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, Color? checkColor = null, Color? focusColor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, Size? tapTargetSize = null, string? semanticLabel = null) : base(key: key)
    {
        this.value = value;
        this.tristate = tristate;
        this.onChanged = onChanged;
        this.mouseCursor = mouseCursor;
        this.activeColor = activeColor;
        this.inactiveColor = inactiveColor;
        this.fillColor = fillColor;
        this.checkColor = checkColor;
        this.focusColor = focusColor;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.side = side;
        this.shape = shape;
        this.tapTargetSize = tapTargetSize;
        this.semanticLabel = semanticLabel;
        System.Diagnostics.Debug.Assert(tristate || (value is not null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoCheckboxState__checkbox());
}

internal class _CupertinoCheckboxState__checkbox : global::Doroti.Framework.Widgets.State<CupertinoCheckbox>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<CupertinoCheckbox>, global::Doroti.Framework.Widgets.ToggleableStateMixin<CupertinoCheckbox>
{
    internal virtual _CheckboxPainter__checkbox _painter { get; private set; } = new _CheckboxPainter__checkbox();
    internal virtual bool? _previousValue { get; set; } = default;
    public virtual bool focused { get; set; } = false;
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

    public override void didUpdateWidget(CupertinoCheckbox oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.value != widget.value)
        {
            _previousValue = oldWidget.value;
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
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _defaultFillColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color>((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return CupertinoColors.white.withOpacity(0.5);
                }
                if (states.Contains(WidgetState.selected))
                {
                    return widget.activeColor ?? CupertinoDynamicColor.resolve(CheckboxLibrary._kDefaultFillColor, context);
                }
                return CupertinoColors.white;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _defaultCheckColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color>((states) =>
            {
                if (states.Contains(WidgetState.disabled) && states.Contains(WidgetState.selected))
                {
                    return widget.checkColor ?? CupertinoDynamicColor.resolve(CheckboxLibrary._kDisabledCheckColor, context);
                }
                if (states.Contains(WidgetState.selected))
                {
                    return widget.checkColor ?? CupertinoDynamicColor.resolve(CheckboxLibrary._kDefaultCheckColor, context);
                }
                return CupertinoColors.white;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide> _defaultSide
    {
        get
        {
            return WidgetStateProperty.resolveWith<global::Doroti.Framework.Painting.BorderSide>((states) =>
            {
                if ((states.Contains(WidgetState.selected) || states.Contains(WidgetState.focused)) && !states.Contains(WidgetState.disabled))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(width: 0.0, color: CupertinoColors.transparent);
                }
                if (states.Contains(WidgetState.disabled))
                {
                    return new global::Doroti.Framework.Painting.BorderSide(color: CupertinoDynamicColor.resolve(CheckboxLibrary._kDisabledBorderColor, context));
                }
                return new global::Doroti.Framework.Painting.BorderSide(color: CupertinoDynamicColor.resolve(CheckboxLibrary._kDefaultBorderColor, context));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    internal virtual global::Doroti.Framework.Painting.BorderSide? _resolveSide(global::Doroti.Framework.Painting.BorderSide? side, HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
    {
        if (side is global::Doroti.Framework.Widgets.WidgetStateBorderSide)
        {
            global::Doroti.Framework.Widgets.WidgetStateBorderSide side__as14535 = (global::Doroti.Framework.Widgets.WidgetStateBorderSide)side;
            return WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.BorderSide?>(side__as14535, states);
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
        HashSet<global::Doroti.Framework.Widgets.WidgetState> currentStates = states;
        global::Doroti.Ui.Color effectiveActiveColor = widget.fillColor?.resolve(activeStates) ?? _defaultFillColor.resolve(activeStates);
        global::Doroti.Ui.Color effectiveInactiveColor = widget.fillColor?.resolve(inactiveStates) ?? _defaultFillColor.resolve(inactiveStates);
        global::Doroti.Framework.Painting.BorderSide effectiveBorderSide = _resolveSide(widget.side, currentStates) ?? _defaultSide.resolve(currentStates);
        global::Doroti.Ui.Color effectiveFocusOverlayColor = widget.focusColor ?? HSLColor.CreateFromColor(effectiveActiveColor.withOpacity(ConstantsLibrary.kCupertinoFocusColorOpacity)).withLightness(ConstantsLibrary.kCupertinoFocusColorBrightness).withSaturation(ConstantsLibrary.kCupertinoFocusColorSaturation).toColor();
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> effectiveMouseCursor = WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>((states) =>
        {
            return WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(widget.mouseCursor, states) ?? ((Foundation.ConstantsLibrary.kIsWeb && !states.Contains(WidgetState.disabled)) ? SystemMouseCursors.click : SystemMouseCursors.basic);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        global::Doroti.Ui.Size effectiveSize = widget.tapTargetSize ?? (PlatformLibrary.defaultTargetPlatform switch { TargetPlatform.iOS or TargetPlatform.android => new global::Doroti.Ui.Size(ConstantsLibrary.kMinInteractiveDimensionCupertino), TargetPlatform.fuchsia => new global::Doroti.Ui.Size(ConstantsLibrary.kMinInteractiveDimensionCupertino), TargetPlatform.macOS or TargetPlatform.linux => new global::Doroti.Ui.Size(CupertinoCheckbox.width), TargetPlatform.windows => new global::Doroti.Ui.Size(CupertinoCheckbox.width), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return new global::Doroti.Framework.Widgets.Semantics(label: widget.semanticLabel, @checked: widget.value ?? false, mixed: widget.tristate ? (widget.value is null) : null, child: buildToggleable(mouseCursor: effectiveMouseCursor, focusNode: widget.focusNode, autofocus: widget.autofocus, size: effectiveSize, painter: ((Func<_CheckboxPainter__checkbox>)(() =>
{
    var __cascade = _painter;
    __cascade.position = position;
    __cascade.reaction = reaction;
    __cascade.focusColor = effectiveFocusOverlayColor;
    __cascade.downPosition = downPosition;
    __cascade.isFocused = currentStates.Contains(WidgetState.focused);
    __cascade.isHovered = currentStates.Contains(WidgetState.hovered);
    __cascade.activeColor = effectiveActiveColor;
    __cascade.inactiveColor = effectiveInactiveColor;
    __cascade.checkColor = _defaultCheckColor.resolve(currentStates);
    __cascade.value = value;
    __cascade.previousValue = _previousValue;
    __cascade.isActive = widget.onChanged is not null;
    __cascade.shape = widget.shape ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(4.0)));
    __cascade.side = effectiveBorderSide;
    __cascade.brightness = CupertinoTheme.of(context).brightness;
    return __cascade;
}))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
    public virtual Duration? reactionAnimationDuration => _reactionAnimationDuration;
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
    public virtual Widget buildToggleable(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor = null, Size size = default!, object? painter = default!)
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

internal class _CheckboxPainter__checkbox : global::Doroti.Framework.Widgets.ToggleablePainter
{
    internal virtual Color? _checkColor { get; set; } = default;
    internal virtual bool? _value { get; set; } = default;
    internal virtual bool? _previousValue { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.OutlinedBorder? _shape { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.BorderSide? _side { get; set; } = default;
    internal virtual Brightness? _brightness { get; set; } = default;

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
    public virtual global::Doroti.Framework.Painting.BorderSide side
    {
        get => _side!;
        set
        {
            var __value = value;
            if (Equals(_side, __value))
            {
                return;
            }
            _side = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Brightness? brightness
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
    internal virtual global::Doroti.Ui.Rect _outerRectAt(Offset origin)
    {
        double size = CupertinoCheckbox.width;
        var rect = Rect.fromLTWH(origin.dx, origin.dy, size, size);
        return rect;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _colorAt(bool value)
    {
        return (DartRuntimePrimitives.RequireValue(value) && isActive) ? activeColor : inactiveColor;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Paint _createStrokePaint()
    {
        return ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = checkColor;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 2.0;
    __cascade.strokeCap = StrokeCap.round;
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _drawFillGradient(Canvas canvas, Rect outer, Color topColor, Color bottomColor)
    {
        var fillGradient = new global::Doroti.Framework.Painting.LinearGradient(begin: Alignment.topCenter, end: Alignment.bottomCenter, colors: new List<global::Doroti.Ui.Color> { topColor, bottomColor });
        var gradientPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.shader = fillGradient.createShader(outer);
    return __cascade;
}))();
        if (shape.preferPaintInterior)
        {
            shape.paintInterior(canvas, outer, gradientPaint);
        }
        else
        {
            canvas.drawPath(shape.getOuterPath(outer), gradientPaint);
        }
    }

    internal virtual void _drawBox(Canvas canvas, Rect outer, Paint paint, global::Doroti.Framework.Painting.BorderSide? side, bool value)
    {
        if (Equals(brightness, Brightness.dark) && !(isActive && DartRuntimePrimitives.RequireValue(value)))
        {
            _drawFillGradient(canvas, outer, paint.color.withOpacity(isActive ? CheckboxLibrary._kDarkGradientOpacities[(int)0L] : CheckboxLibrary._kDisabledDarkGradientOpacities[(int)0L]), paint.color.withOpacity(isActive ? CheckboxLibrary._kDarkGradientOpacities[(int)1L] : CheckboxLibrary._kDisabledDarkGradientOpacities[(int)1L]));
        }
        else
        {
            if (shape.preferPaintInterior)
            {
                shape.paintInterior(canvas, outer, paint);
            }
            else
            {
                canvas.drawPath(shape.getOuterPath(outer), paint);
            }
        }
        if (side is not null)
        {
            shape.copyWith(side: side).paint(canvas, outer);
        }
    }

    internal virtual void _drawCheck(Canvas canvas, Offset origin, Paint paint)
    {
        var path = new global::Doroti.Ui.Path();
        var start = new global::Doroti.Ui.Offset(CupertinoCheckbox.width * 0.22, CupertinoCheckbox.width * 0.54);
        var mid = new global::Doroti.Ui.Offset(CupertinoCheckbox.width * 0.4, CupertinoCheckbox.width * 0.75);
        var end = new global::Doroti.Ui.Offset(CupertinoCheckbox.width * 0.78, CupertinoCheckbox.width * 0.25);
        path.moveTo(origin.dx + start.dx, origin.dy + start.dy);
        path.lineTo(origin.dx + mid.dx, origin.dy + mid.dy);
        path.moveTo(origin.dx + mid.dx, origin.dy + mid.dy);
        path.lineTo(origin.dx + end.dx, origin.dy + end.dy);
        canvas.drawPath(path, paint);
    }

    internal virtual void _drawDash(Canvas canvas, Offset origin, Paint paint)
    {
        var start = new global::Doroti.Ui.Offset(CupertinoCheckbox.width * 0.25, CupertinoCheckbox.width * 0.5);
        var end = new global::Doroti.Ui.Offset(CupertinoCheckbox.width * 0.75, CupertinoCheckbox.width * 0.5);
        canvas.drawLine(origin + start, origin + end, paint);
    }

    public override void paint(Canvas canvas, Size size)
    {
        global::Doroti.Ui.Paint strokePaint = _createStrokePaint();
        var origin = (size / 2.0) - (new global::Doroti.Ui.Size(CupertinoCheckbox.width) / 2.0);
        global::Doroti.Ui.Rect outer = _outerRectAt(origin);
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = _colorAt(value ?? true);
    return __cascade;
}))();
        switch (value)
        {
            case false:
                {
                    _drawBox(canvas, outer, paintLocal, side, value ?? true);
                    break;
                }
            case true:
                {
                    _drawBox(canvas, outer, paintLocal, side, value ?? true);
                    _drawCheck(canvas, origin, strokePaint);
                    break;
                }
            case null:
                {
                    _drawBox(canvas, outer, paintLocal, side, value ?? true);
                    _drawDash(canvas, origin, strokePaint);
                    break;
                }
        }
        if (downPosition is not null)
        {
            var pressedPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = Equals(brightness, Brightness.light) ? CupertinoColors.black.withOpacity(CheckboxLibrary._kPressedOverlayOpacity) : CupertinoColors.white.withOpacity(CheckboxLibrary._kPressedOverlayOpacity);
    return __cascade;
}))();
            if (shape.preferPaintInterior)
            {
                shape.paintInterior(canvas, outer, pressedPaint);
            }
            else
            {
                canvas.drawPath(shape.getOuterPath(outer), pressedPaint);
            }
        }
        if (isFocused)
        {
            global::Doroti.Ui.Rect focusOuter = outer.inflate(1);
            var borderPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = focusColor;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 3.5;
    return __cascade;
}))();
            _drawBox(canvas, focusOuter, borderPaint, side, value ?? true);
        }
    }

}
