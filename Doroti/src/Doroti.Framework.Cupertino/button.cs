// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/button.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public enum CupertinoButtonSize
{
    small,
    medium,
    large
}

internal enum _CupertinoButtonStyle__button
{
    plain,
    tinted,
    filled
}

public class CupertinoButton : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color disabledColor { get; private set; } = default!;
    public virtual Color? foregroundColor { get; private set; }
    public virtual Action? onPressed { get; private set; }
    public virtual Action? onLongPress { get; private set; }
    public virtual double? minSize { get; private set; }
    public virtual Size? minimumSize { get; private set; }
    public virtual double? pressedOpacity { get; private set; }
    public virtual BorderRadius? borderRadius { get; private set; }
    public virtual CupertinoButtonSize sizeStyle { get; private set; } = default!;
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Color? focusColor { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual Action<bool>? onFocusChange { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual MouseCursor? mouseCursor { get; private set; }
    internal virtual _CupertinoButtonStyle__button _style { get; private set; } = default!;

    public CupertinoButton(Key? key = null, Widget child = default!, CupertinoButtonSize sizeStyle = CupertinoButtonSize.large, EdgeInsetsGeometry? padding = null, Color? color = null, Color? foregroundColor = null, Color disabledColor = default!, double? minSize = null, Size? minimumSize = null, double? pressedOpacity = 0.4, BorderRadius? borderRadius = null, AlignmentGeometry alignment = default!, Color? focusColor = null, FocusNode? focusNode = null, Action<bool>? onFocusChange = null, bool autofocus = false, MouseCursor? mouseCursor = null, Action? onLongPress = null, Action? onPressed = default!) : base(key: key)
    {
        Color __disabledColor = disabledColor ?? CupertinoColors.quaternarySystemFill;
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.child = child;
        this.sizeStyle = sizeStyle;
        this.padding = padding;
        this.color = color;
        this.foregroundColor = foregroundColor;
        this.disabledColor = __disabledColor;
        this.minSize = minSize;
        this.minimumSize = minimumSize;
        this.pressedOpacity = pressedOpacity;
        this.borderRadius = borderRadius;
        this.alignment = __alignment;
        this.focusColor = focusColor;
        this.focusNode = focusNode;
        this.onFocusChange = onFocusChange;
        this.autofocus = autofocus;
        this.mouseCursor = mouseCursor;
        this.onLongPress = onLongPress;
        this.onPressed = onPressed;
        _style = _CupertinoButtonStyle__button.plain;
        System.Diagnostics.Debug.Assert((pressedOpacity is null) || (pressedOpacity >= 0.0) && (pressedOpacity <= 1.0));
        System.Diagnostics.Debug.Assert((minimumSize is null) || (minSize is null));
    }

    public static CupertinoButton CreateTinted(Key? key = null, Widget child = default!, CupertinoButtonSize sizeStyle = CupertinoButtonSize.large, EdgeInsetsGeometry? padding = null, Color? color = null, Color? foregroundColor = null, Color disabledColor = default!, double? minSize = null, Size? minimumSize = null, double? pressedOpacity = 0.4, BorderRadius? borderRadius = null, AlignmentGeometry alignment = default!, Color? focusColor = null, FocusNode? focusNode = null, Action<bool>? onFocusChange = null, bool autofocus = false, MouseCursor? mouseCursor = null, Action? onLongPress = null, Action? onPressed = default!)
    {
        var __instance = new CupertinoButton(key: key, child: child, sizeStyle: sizeStyle, padding: padding, color: color, foregroundColor: foregroundColor, disabledColor: disabledColor, minSize: minSize, minimumSize: minimumSize, pressedOpacity: pressedOpacity, borderRadius: borderRadius, alignment: alignment, focusColor: focusColor, focusNode: focusNode, onFocusChange: onFocusChange, autofocus: autofocus, mouseCursor: mouseCursor, onLongPress: onLongPress, onPressed: onPressed);
        Color __disabledColor = disabledColor ?? CupertinoColors.tertiarySystemFill;
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        __instance.child = child;
        __instance.sizeStyle = sizeStyle;
        __instance.padding = padding;
        __instance.color = color;
        __instance.foregroundColor = foregroundColor;
        __instance.disabledColor = __disabledColor;
        __instance.minSize = minSize;
        __instance.minimumSize = minimumSize;
        __instance.pressedOpacity = pressedOpacity;
        __instance.borderRadius = borderRadius;
        __instance.alignment = __alignment;
        __instance.focusColor = focusColor;
        __instance.focusNode = focusNode;
        __instance.onFocusChange = onFocusChange;
        __instance.autofocus = autofocus;
        __instance.mouseCursor = mouseCursor;
        __instance.onLongPress = onLongPress;
        __instance.onPressed = onPressed;
        __instance._style = _CupertinoButtonStyle__button.tinted;
        return __instance;
    }

    public static CupertinoButton CreateFilled(Key? key = null, Widget child = default!, CupertinoButtonSize sizeStyle = CupertinoButtonSize.large, EdgeInsetsGeometry? padding = null, Color? color = null, Color disabledColor = default!, Color? foregroundColor = null, double? minSize = null, Size? minimumSize = null, double? pressedOpacity = 0.4, BorderRadius? borderRadius = null, AlignmentGeometry alignment = default!, Color? focusColor = null, FocusNode? focusNode = null, Action<bool>? onFocusChange = null, bool autofocus = false, MouseCursor? mouseCursor = null, Action? onLongPress = null, Action? onPressed = default!)
    {
        var __instance = new CupertinoButton(key: key, child: child, sizeStyle: sizeStyle, padding: padding, color: color, foregroundColor: foregroundColor, disabledColor: disabledColor, minSize: minSize, minimumSize: minimumSize, pressedOpacity: pressedOpacity, borderRadius: borderRadius, alignment: alignment, focusColor: focusColor, focusNode: focusNode, onFocusChange: onFocusChange, autofocus: autofocus, mouseCursor: mouseCursor, onLongPress: onLongPress, onPressed: onPressed);
        Color __disabledColor = disabledColor ?? CupertinoColors.tertiarySystemFill;
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        __instance.child = child;
        __instance.sizeStyle = sizeStyle;
        __instance.padding = padding;
        __instance.color = color;
        __instance.disabledColor = __disabledColor;
        __instance.foregroundColor = foregroundColor;
        __instance.minSize = minSize;
        __instance.minimumSize = minimumSize;
        __instance.pressedOpacity = pressedOpacity;
        __instance.borderRadius = borderRadius;
        __instance.alignment = __alignment;
        __instance.focusColor = focusColor;
        __instance.focusNode = focusNode;
        __instance.onFocusChange = onFocusChange;
        __instance.autofocus = autofocus;
        __instance.mouseCursor = mouseCursor;
        __instance.onLongPress = onLongPress;
        __instance.onPressed = onPressed;
        __instance._style = _CupertinoButtonStyle__button.filled;
        return __instance;
    }

    public virtual bool enabled => DartRuntimePrimitives.ConvertValue<bool>((onPressed is not null) || (onLongPress is not null));
    public static double tapMoveSlop()
    {
        return PlatformLibrary.defaultTargetPlatform switch { TargetPlatform.iOS or TargetPlatform.android => ConstantsLibrary.kCupertinoButtonTapMoveSlop, TargetPlatform.fuchsia => ConstantsLibrary.kCupertinoButtonTapMoveSlop, TargetPlatform.macOS or TargetPlatform.linux => 0.0, TargetPlatform.windows => 0.0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoButtonState__button());
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagProperty("enabled", value: enabled, ifFalse: "disabled"));
    }

}

internal class _CupertinoButtonState__button : State<CupertinoButton>, SingleTickerProviderStateMixin<CupertinoButton>
{
    public static Duration kFadeOutDuration = Duration.Create(milliseconds: 120L);
    public static Duration kFadeInDuration = Duration.Create(milliseconds: 180L);
    internal virtual Tween<double> _opacityTween { get; private set; } = new Tween<double>(begin: 1.0);
    internal virtual AnimationController _animationController { get; set; } = default!;
    internal virtual Animation<double> _opacityAnimation { get; set; } = default!;
    public virtual bool isFocused { get; set; } = default!;
    internal static WidgetStateProperty<MouseCursor> _defaultCursor = WidgetStateProperty.resolveWith((states) =>
    {
        return (!states.Contains(WidgetState.disabled) && Foundation.ConstantsLibrary.kIsWeb) ? SystemMouseCursors.click : MouseCursor.defer;
        throw new InvalidOperationException("Dart closure completed without a value.");
    });
    internal virtual bool _buttonHeldDown { get; set; } = false;
    internal virtual bool _tapInProgress { get; set; } = false;
    private bool __late__actionMap_initialized;
    private DartMap<Type, dynamic> __late__actionMap = default!;
    internal virtual DartMap<Type, dynamic> _actionMap
    {
        get
        {
            if (!__late__actionMap_initialized)
            {
                __late__actionMap = new DartMap<Type, dynamic> { [typeof(ActivateIntent)] = new CallbackAction<ActivateIntent>(onInvoke: _handleTap) };
                __late__actionMap_initialized = true;
            }
            return __late__actionMap;
        }
    }
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        isFocused = false;
        _animationController = new AnimationController(duration: Duration.Create(milliseconds: 200L), value: 0.0, vsync: this);
        _opacityAnimation = _animationController.drive(new CurveTween(curve: Curves.decelerate)).drive(_opacityTween);
        _setTween();
    }

    public override void didUpdateWidget(CupertinoButton old)
    {
        base.didUpdateWidget(old);
        _setTween();
    }

    internal virtual void _setTween()
    {
        _opacityTween.end = widget.pressedOpacity ?? 1.0;
    }

    public override void dispose()
    {
        _animationController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _handleTapDown(Gestures.TapDownDetails @event)
    {
        setState(() =>
        {
            _tapInProgress = true;
        });
        if (!_buttonHeldDown)
        {
            _buttonHeldDown = true;
            _animate();
        }
    }

    internal virtual void _handleTapUp(Gestures.TapUpDetails @event)
    {
        setState(() =>
        {
            _tapInProgress = false;
        });
        if (_buttonHeldDown)
        {
            _buttonHeldDown = false;
            _animate();
        }
        var renderObject = ((RenderBox?)context.findRenderObject()!)!;
        Offset localPosition = DartRuntimePrimitives.ConvertValue<Offset>(renderObject.globalToLocal(@event.globalPosition));
        if (renderObject.paintBounds.inflate(CupertinoButton.tapMoveSlop()).contains(localPosition))
        {
            _handleTap();
        }
    }

    internal virtual void _handleTapCancel()
    {
        setState(() =>
        {
            _tapInProgress = false;
        });
        if (_buttonHeldDown)
        {
            _buttonHeldDown = false;
            _animate();
        }
    }

    internal virtual void _handleTapMove(Gestures.TapMoveDetails @event)
    {
        var renderObject = ((RenderBox?)context.findRenderObject()!)!;
        Offset localPosition = DartRuntimePrimitives.ConvertValue<Offset>(renderObject.globalToLocal(@event.globalPosition));
        bool buttonShouldHeldDown = DartRuntimePrimitives.ConvertValue<bool>(renderObject.paintBounds.inflate(CupertinoButton.tapMoveSlop()).contains(localPosition));
        if (_tapInProgress && (buttonShouldHeldDown != _buttonHeldDown))
        {
            _buttonHeldDown = buttonShouldHeldDown;
            _animate();
        }
    }

    internal virtual void _handleTap(Intent? __unused0 = null)
    {
        if (widget.onPressed is not null)
        {
            widget.onPressed!();
            context.findRenderObject()!.sendSemanticsEvent(new Semantics.TapSemanticEvent());
        }
    }

    internal virtual void _animate()
    {
        if (_animationController.isAnimating)
        {
            return;
        }
        bool wasHeldDown = _buttonHeldDown;
        Scheduler.TickerFuture ticker = _buttonHeldDown ? _animationController.animateTo(1.0, duration: kFadeOutDuration, curve: Curves.easeInOutCubicEmphasized) : _animationController.animateTo(0.0, duration: kFadeInDuration, curve: Curves.easeOutCubic);
        DartRuntimePrimitives.Ignore(ticker.then((value) =>
        {
            if (mounted && (wasHeldDown != _buttonHeldDown))
            {
                _animate();
            }
            return null!;
        }));
    }

    internal virtual void _onShowFocusHighlight(bool showHighlight)
    {
        setState(() =>
        {
            isFocused = showHighlight;
        });
    }

    public override Widget build(BuildContext context)
    {
        bool enabledLocal = widget.enabled;
        Size? minimumSizeLocal = (widget.minimumSize is null) ? ((widget.minSize is null) ? null : new Size(DartRuntimePrimitives.RequireValue(widget.minSize), DartRuntimePrimitives.RequireValue(widget.minSize))) : DartRuntimePrimitives.RequireValue(widget.minimumSize);
        CupertinoThemeData themeData = CupertinoTheme.of(context);
        Color primaryColorLocal = themeData.primaryColor;
        Color? backgroundColor = ((widget.color is null) ? ((!Equals(widget._style, _CupertinoButtonStyle__button.plain)) ? primaryColorLocal : null) : CupertinoDynamicColor.maybeResolve(widget.color, context))?.withOpacity(Equals(widget._style, _CupertinoButtonStyle__button.tinted) ? (Equals(CupertinoTheme.brightnessOf(context), Brightness.light) ? ConstantsLibrary.kCupertinoButtonTintedOpacityLight : ConstantsLibrary.kCupertinoButtonTintedOpacityDark) : (widget.color?.opacity ?? 1.0));
        Color effectiveForegroundColor = widget.foregroundColor ?? ((widget._style, enabledLocal) switch { (_CupertinoButtonStyle__button.filled, _) => themeData.primaryContrastingColor, (_, true) => primaryColorLocal, (_, false) => CupertinoDynamicColor.resolve(CupertinoColors.tertiaryLabel, context) });
        Color effectiveFocusOutlineColor = widget.focusColor ?? HSLColor.CreateFromColor((backgroundColor ?? CupertinoColors.activeBlue).withOpacity(ConstantsLibrary.kCupertinoFocusColorOpacity)).withLightness(ConstantsLibrary.kCupertinoFocusColorBrightness).withSaturation(ConstantsLibrary.kCupertinoFocusColorSaturation).toColor();
        TextStyle textStyle = (Equals(widget.sizeStyle, CupertinoButtonSize.small) ? themeData.textTheme.actionSmallTextStyle : themeData.textTheme.actionTextStyle).copyWith(color: effectiveForegroundColor);
        IconThemeData iconTheme = IconTheme.of(context).copyWith(color: effectiveForegroundColor, size: (textStyle.fontSize is not null) ? (DartRuntimePrimitives.RequireValue(textStyle.fontSize) * 1.2) : ConstantsLibrary.kCupertinoButtonDefaultIconSize);
        Gestures.DeviceGestureSettings? gestureSettingsLocal = MediaQuery.maybeGestureSettingsOf(context);
        var states = ((Func<HashSet<WidgetState>>)(() => { var __collection17491 = new HashSet<WidgetState>(); if (!enabledLocal) { __collection17491.Add(WidgetState.disabled); } if (_tapInProgress) { __collection17491.Add(WidgetState.pressed); } if (isFocused) { __collection17491.Add(WidgetState.focused); } return __collection17491; }))();
        MouseCursor effectiveMouseCursor = WidgetStateProperty.resolveAs(widget.mouseCursor, states) ?? _defaultCursor.resolve(states);
        var shapeDecoration = new ShapeDecoration(shape: new RoundedSuperellipseBorder(side: (enabledLocal && isFocused) ? new BorderSide(color: effectiveFocusOutlineColor, width: 3.5, strokeAlign: BorderSide.strokeAlignOutside) : BorderSide.none, borderRadius: widget.borderRadius ?? ConstantsLibrary.kCupertinoButtonSizeBorderRadius.GetValueOrDefault(widget.sizeStyle)), color: ((backgroundColor is not null) && !enabledLocal) ? CupertinoDynamicColor.resolve(widget.disabledColor, context) : backgroundColor);
        return new MouseRegion(cursor: effectiveMouseCursor, child: new FocusableActionDetector(actions: _actionMap, focusNode: widget.focusNode, autofocus: widget.autofocus, onFocusChange: widget.onFocusChange, onShowFocusHighlight: _onShowFocusHighlight, enabled: enabledLocal, child: new RawGestureDetector(behavior: HitTestBehavior.opaque, gestures: new DartMap<Type, dynamic>
        {
            [typeof(Gestures.TapGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<Gestures.TapGestureRecognizer>(() => new Gestures.TapGestureRecognizer(postAcceptSlopTolerance: null), (instance) =>
            {
                instance.onTapDown = enabledLocal ? _handleTapDown : null;
                instance.onTapUp = enabledLocal ? _handleTapUp : null;
                instance.onTapCancel = enabledLocal ? _handleTapCancel : null;
                instance.onTapMove = enabledLocal ? _handleTapMove : null;
                instance.gestureSettings = gestureSettingsLocal;
            })
        }, child: new Widgets.Semantics(button: true, child: new ConstrainedBox(constraints: new BoxConstraints(minWidth: (minimumSizeLocal?.width ?? DartCollectionRuntime.NullableMapValue<double>(ConstantsLibrary.kCupertinoButtonMinSize, widget.sizeStyle)) ?? ConstantsLibrary.kMinInteractiveDimensionCupertino, minHeight: (minimumSizeLocal?.height ?? DartCollectionRuntime.NullableMapValue<double>(ConstantsLibrary.kCupertinoButtonMinSize, widget.sizeStyle)) ?? ConstantsLibrary.kMinInteractiveDimensionCupertino), child: new FadeTransition(opacity: _opacityAnimation, child: new DecoratedBox(decoration: shapeDecoration, child: new Padding(padding: widget.padding ?? ConstantsLibrary.kCupertinoButtonPadding.GetValueOrDefault(widget.sizeStyle)!, child: new Align(alignment: widget.alignment, widthFactor: 1.0, heightFactor: 1.0, child: new DefaultTextStyle(style: textStyle, child: new IconTheme(data: iconTheme, child: widget.child)))))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        _ticker = new Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values = _tickerModeNotifier!.value;
        if (_ticker is not null)
        {
            _ticker!.muted = !values.enabled;
            _ticker!.forceFrames = values.forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new DiagnosticsProperty<Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}
