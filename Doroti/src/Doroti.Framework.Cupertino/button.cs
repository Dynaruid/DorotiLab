// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/button.dart
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

public class CupertinoButton : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color disabledColor { get; private set; } = default!;
    public virtual Color? foregroundColor { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual double? minSize { get; private set; }
    public virtual Size? minimumSize { get; private set; }
    public virtual double? pressedOpacity { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual CupertinoButtonSize sizeStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Color? focusColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    internal virtual _CupertinoButtonStyle__button _style { get; private set; } = default!;

    public CupertinoButton(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget child = default!, CupertinoButtonSize sizeStyle = CupertinoButtonSize.large, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Color? color = null, Color? foregroundColor = null, Color disabledColor = default!, double? minSize = null, Size? minimumSize = null, double? pressedOpacity = 0.4, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, Color? focusColor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::System.Action? onLongPress = null, global::System.Action? onPressed = default!) : base(key: key)
    {
        Color __disabledColor = disabledColor ?? CupertinoColors.quaternarySystemFill;
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Framework.Painting.Alignment.center;
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
        this._style = _CupertinoButtonStyle__button.plain;
        System.Diagnostics.Debug.Assert(((pressedOpacity is null) || (((pressedOpacity >= 0.0) && (pressedOpacity <= 1.0)))));
        System.Diagnostics.Debug.Assert(((minimumSize is null) || (minSize is null)));
    }

    public static CupertinoButton CreateTinted(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget child = default!, CupertinoButtonSize sizeStyle = CupertinoButtonSize.large, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Color? color = null, Color? foregroundColor = null, Color disabledColor = default!, double? minSize = null, Size? minimumSize = null, double? pressedOpacity = 0.4, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, Color? focusColor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::System.Action? onLongPress = null, global::System.Action? onPressed = default!)
    {
        var __instance = new CupertinoButton(key: key, child: child, sizeStyle: sizeStyle, padding: padding, color: color, foregroundColor: foregroundColor, disabledColor: disabledColor, minSize: minSize, minimumSize: minimumSize, pressedOpacity: pressedOpacity, borderRadius: borderRadius, alignment: alignment, focusColor: focusColor, focusNode: focusNode, onFocusChange: onFocusChange, autofocus: autofocus, mouseCursor: mouseCursor, onLongPress: onLongPress, onPressed: onPressed);
        Color __disabledColor = disabledColor ?? CupertinoColors.tertiarySystemFill;
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Framework.Painting.Alignment.center;
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

    public static CupertinoButton CreateFilled(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget child = default!, CupertinoButtonSize sizeStyle = CupertinoButtonSize.large, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Color? color = null, Color disabledColor = default!, Color? foregroundColor = null, double? minSize = null, Size? minimumSize = null, double? pressedOpacity = 0.4, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, Color? focusColor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::System.Action? onLongPress = null, global::System.Action? onPressed = default!)
    {
        var __instance = new CupertinoButton(key: key, child: child, sizeStyle: sizeStyle, padding: padding, color: color, foregroundColor: foregroundColor, disabledColor: disabledColor, minSize: minSize, minimumSize: minimumSize, pressedOpacity: pressedOpacity, borderRadius: borderRadius, alignment: alignment, focusColor: focusColor, focusNode: focusNode, onFocusChange: onFocusChange, autofocus: autofocus, mouseCursor: mouseCursor, onLongPress: onLongPress, onPressed: onPressed);
        Color __disabledColor = disabledColor ?? CupertinoColors.tertiarySystemFill;
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Framework.Painting.Alignment.center;
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

    public virtual bool enabled => DartRuntimePrimitives.ConvertValue<bool>(((this.onPressed is not null) || (this.onLongPress is not null)));
    public static double tapMoveSlop()
    {
        return (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.iOS or global::Doroti.Framework.Foundation.TargetPlatform.android => ConstantsLibrary.kCupertinoButtonTapMoveSlop, global::Doroti.Framework.Foundation.TargetPlatform.fuchsia => ConstantsLibrary.kCupertinoButtonTapMoveSlop, global::Doroti.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Framework.Foundation.TargetPlatform.linux => 0.0, global::Doroti.Framework.Foundation.TargetPlatform.windows => 0.0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoButtonState__button());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enabled", value: this.enabled, ifFalse: "disabled"));
    }

}

internal class _CupertinoButtonState__button : global::Doroti.Framework.Widgets.State<CupertinoButton>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<CupertinoButton>
{
    public static Duration kFadeOutDuration = Duration.Create(milliseconds: 120L);
    public static Duration kFadeInDuration = Duration.Create(milliseconds: 180L);
    internal virtual global::Doroti.Framework.Animation.Tween<double> _opacityTween { get; private set; } = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0);
    internal virtual global::Doroti.Framework.Animation.AnimationController _animationController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _opacityAnimation { get; set; } = default!;
    public virtual bool isFocused { get; set; } = default!;
    internal static global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> _defaultCursor = WidgetStateProperty.resolveWith<global::Doroti.Framework.Services.MouseCursor>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Services.MouseCursor>)((states) =>
    {
        return ((global::Doroti.Framework.Services.MouseCursor)(object?)((!states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled) && global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb) ? global::Doroti.Framework.Services.SystemMouseCursors.click : global::Doroti.Framework.Services.MouseCursor.defer));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
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
                __late__actionMap = new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.ActivateIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.ActivateIntent>(onInvoke: (global::System.Action<global::Doroti.Framework.Widgets.Intent?>)this._handleTap) };
                __late__actionMap_initialized = true;
            }
            return __late__actionMap;
        }
    }
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        isFocused = false;
        _animationController = new global::Doroti.Framework.Animation.AnimationController(duration: Duration.Create(milliseconds: 200L), value: 0.0, vsync: this);
        _opacityAnimation = this._animationController.drive(new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.decelerate)).drive(this._opacityTween);
        _setTween();
    }

    public override void didUpdateWidget(CupertinoButton old)
    {
        base.didUpdateWidget(old);
        _setTween();
    }

    internal virtual void _setTween()
    {
        this._opacityTween.end = (((CupertinoButton)this.widget).pressedOpacity ?? 1.0);
    }

    public override void dispose()
    {
        this._animationController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(this._updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _handleTapDown(global::Doroti.Framework.Gestures.TapDownDetails @event)
    {
        setState(((global::System.Action)(() =>
        {
            _tapInProgress = true;
        })));
        if (!this._buttonHeldDown)
        {
            _buttonHeldDown = true;
            _animate();
        }
    }

    internal virtual void _handleTapUp(global::Doroti.Framework.Gestures.TapUpDetails @event)
    {
        setState(((global::System.Action)(() =>
        {
            _tapInProgress = false;
        })));
        if (this._buttonHeldDown)
        {
            _buttonHeldDown = false;
            _animate();
        }
        var renderObject = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
        global::Doroti.Ui.Offset localPosition = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(((Offset)((dynamic)renderObject).globalToLocal(((global::Doroti.Framework.Gestures.TapUpDetails)@event).globalPosition))));
        if (((global::Doroti.Framework.Rendering.RenderBox)renderObject).paintBounds.inflate(CupertinoButton.tapMoveSlop()).contains(localPosition))
        {
            _handleTap();
        }
    }

    internal virtual void _handleTapCancel()
    {
        setState(((global::System.Action)(() =>
        {
            _tapInProgress = false;
        })));
        if (this._buttonHeldDown)
        {
            _buttonHeldDown = false;
            _animate();
        }
    }

    internal virtual void _handleTapMove(global::Doroti.Framework.Gestures.TapMoveDetails @event)
    {
        var renderObject = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
        global::Doroti.Ui.Offset localPosition = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(((Offset)((dynamic)renderObject).globalToLocal(((global::Doroti.Framework.Gestures.TapMoveDetails)@event).globalPosition))));
        bool buttonShouldHeldDown = DartRuntimePrimitives.ConvertValue<bool>(((global::Doroti.Framework.Rendering.RenderBox)renderObject).paintBounds.inflate(CupertinoButton.tapMoveSlop()).contains(localPosition));
        if ((this._tapInProgress && (buttonShouldHeldDown != this._buttonHeldDown)))
        {
            _buttonHeldDown = buttonShouldHeldDown;
            _animate();
        }
    }

    internal virtual void _handleTap(global::Doroti.Framework.Widgets.Intent? __unused0 = null)
    {
        if ((((CupertinoButton)this.widget).onPressed is not null))
        {
            ((CupertinoButton)this.widget).onPressed!();
            ((dynamic)this.context.findRenderObject()!).sendSemanticsEvent(new global::Doroti.Framework.Semantics.TapSemanticEvent());
        }
    }

    internal virtual void _animate()
    {
        if (((global::Doroti.Framework.Animation.AnimationController)this._animationController).isAnimating)
        {
            return;
        }
        bool wasHeldDown = this._buttonHeldDown;
        global::Doroti.Framework.Scheduler.TickerFuture ticker = (this._buttonHeldDown ? this._animationController.animateTo(1.0, duration: kFadeOutDuration, curve: global::Doroti.Framework.Animation.Curves.easeInOutCubicEmphasized) : this._animationController.animateTo(0.0, duration: kFadeInDuration, curve: global::Doroti.Framework.Animation.Curves.easeOutCubic));
        DartRuntimePrimitives.Ignore(ticker.then(((global::System.Func<object?, object>)((value) =>
        {
            if ((this.mounted && (wasHeldDown != this._buttonHeldDown)))
            {
                _animate();
            }
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
    }

    internal virtual void _onShowFocusHighlight(bool showHighlight)
    {
        setState(((global::System.Action)(() =>
        {
            isFocused = showHighlight;
        })));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        bool enabledLocal = ((CupertinoButton)this.widget).enabled;
        global::Doroti.Ui.Size? minimumSizeLocal = ((global::Doroti.Ui.Size?)(object?)((((CupertinoButton)this.widget).minimumSize is null) ? ((((CupertinoButton)this.widget).minSize is null) ? null : new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(((CupertinoButton)this.widget).minSize), DartRuntimePrimitives.RequireValue(((CupertinoButton)this.widget).minSize))) : DartRuntimePrimitives.RequireValue(((CupertinoButton)this.widget).minimumSize)));
        CupertinoThemeData themeData = CupertinoTheme.of(context);
        global::Doroti.Ui.Color primaryColorLocal = ((global::Doroti.Ui.Color)(object?)themeData.primaryColor);
        global::Doroti.Ui.Color? backgroundColor = ((global::Doroti.Ui.Color?)(object?)(((((CupertinoButton)this.widget).color is null) ? ((!object.Equals(((CupertinoButton)this.widget)._style, _CupertinoButtonStyle__button.plain)) ? primaryColorLocal : null) : CupertinoDynamicColor.maybeResolve(((CupertinoButton)this.widget).color, context)))?.withOpacity(((object.Equals(((CupertinoButton)this.widget)._style, _CupertinoButtonStyle__button.tinted)) ? ((object.Equals(CupertinoTheme.brightnessOf(context), Brightness.light)) ? ConstantsLibrary.kCupertinoButtonTintedOpacityLight : ConstantsLibrary.kCupertinoButtonTintedOpacityDark) : (((CupertinoButton)this.widget).color?.opacity ?? 1.0))));
        global::Doroti.Ui.Color effectiveForegroundColor = ((global::Doroti.Ui.Color)(object?)(((CupertinoButton)this.widget).foregroundColor ?? ((((CupertinoButton)this.widget)._style, enabledLocal) switch { (_CupertinoButtonStyle__button.filled, _) => themeData.primaryContrastingColor, (_, true) => primaryColorLocal, (_, false) => CupertinoDynamicColor.resolve(CupertinoColors.tertiaryLabel, context) })));
        global::Doroti.Ui.Color effectiveFocusOutlineColor = ((global::Doroti.Ui.Color)(object?)((((CupertinoButton)this.widget).focusColor ?? (Color)global::Doroti.Framework.Painting.HSLColor.CreateFromColor(((backgroundColor ?? CupertinoColors.activeBlue)).withOpacity(ConstantsLibrary.kCupertinoFocusColorOpacity)).withLightness(ConstantsLibrary.kCupertinoFocusColorBrightness).withSaturation(ConstantsLibrary.kCupertinoFocusColorSaturation).toColor())));
        global::Doroti.Framework.Painting.TextStyle textStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)(((object.Equals(((CupertinoButton)this.widget).sizeStyle, CupertinoButtonSize.small)) ? themeData.textTheme.actionSmallTextStyle : themeData.textTheme.actionTextStyle)).copyWith(color: effectiveForegroundColor));
        global::Doroti.Framework.Widgets.IconThemeData iconTheme = ((global::Doroti.Framework.Widgets.IconThemeData)(object?)IconTheme.of(context).copyWith(color: effectiveForegroundColor, size: ((((global::Doroti.Framework.Painting.TextStyle)textStyle).fontSize is not null) ? (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Painting.TextStyle)textStyle).fontSize) * 1.2) : ConstantsLibrary.kCupertinoButtonDefaultIconSize)));
        global::Doroti.Framework.Gestures.DeviceGestureSettings? gestureSettingsLocal = ((global::Doroti.Framework.Gestures.DeviceGestureSettings?)(object?)MediaQuery.maybeGestureSettingsOf(context));
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection17491 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (!enabledLocal) { __collection17491.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } if (this._tapInProgress) { __collection17491.Add(global::Doroti.Framework.Widgets.WidgetState.pressed); } if (this.isFocused) { __collection17491.Add(global::Doroti.Framework.Widgets.WidgetState.focused); } return __collection17491; }))();
        global::Doroti.Framework.Services.MouseCursor effectiveMouseCursor = ((WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(((CupertinoButton)this.widget).mouseCursor, states) ?? (global::Doroti.Framework.Services.MouseCursor)_defaultCursor.resolve(states)));
        var shapeDecoration = new global::Doroti.Framework.Painting.ShapeDecoration(shape: new global::Doroti.Framework.Painting.RoundedSuperellipseBorder(side: ((enabledLocal && this.isFocused) ? new global::Doroti.Framework.Painting.BorderSide(color: effectiveFocusOutlineColor, width: 3.5, strokeAlign: global::Doroti.Framework.Painting.BorderSide.strokeAlignOutside) : global::Doroti.Framework.Painting.BorderSide.none), borderRadius: (((CupertinoButton)this.widget).borderRadius ?? ConstantsLibrary.kCupertinoButtonSizeBorderRadius.GetValueOrDefault(((CupertinoButton)this.widget).sizeStyle))), color: (((backgroundColor is not null) && !enabledLocal) ? CupertinoDynamicColor.resolve(((CupertinoButton)this.widget).disabledColor, context) : backgroundColor));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.MouseRegion(cursor: effectiveMouseCursor, child: new global::Doroti.Framework.Widgets.FocusableActionDetector(actions: this._actionMap, focusNode: ((CupertinoButton)this.widget).focusNode, autofocus: ((CupertinoButton)this.widget).autofocus, onFocusChange: (global::System.Action<bool>?)((CupertinoButton)this.widget).onFocusChange, onShowFocusHighlight: (global::System.Action<bool>)this._onShowFocusHighlight, enabled: enabledLocal, child: new global::Doroti.Framework.Widgets.RawGestureDetector(behavior: global::Doroti.Framework.Rendering.HitTestBehavior.opaque, gestures: new DartMap<Type, dynamic>
        {
            [typeof(global::Doroti.Framework.Gestures.TapGestureRecognizer)] = new global::Doroti.Framework.Widgets.GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.TapGestureRecognizer(postAcceptSlopTolerance: null))), ((global::System.Action<global::Doroti.Framework.Gestures.TapGestureRecognizer>)((instance) =>
            {
                instance.onTapDown = (global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>)((global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>)(enabledLocal ? this._handleTapDown : null));
                instance.onTapUp = (global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails>)((global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails>)(enabledLocal ? this._handleTapUp : null));
                instance.onTapCancel = (global::System.Action)((global::System.Action)(enabledLocal ? this._handleTapCancel : null));
                instance.onTapMove = (global::System.Action<global::Doroti.Framework.Gestures.TapMoveDetails>)((global::System.Action<global::Doroti.Framework.Gestures.TapMoveDetails>)(enabledLocal ? this._handleTapMove : null));
                instance.gestureSettings = gestureSettingsLocal;
            })))
        }, child: new global::Doroti.Framework.Widgets.Semantics(button: true, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: ((minimumSizeLocal?.width ?? DartCollectionRuntime.NullableMapValue<double>(ConstantsLibrary.kCupertinoButtonMinSize, ((CupertinoButton)this.widget).sizeStyle)) ?? ConstantsLibrary.kMinInteractiveDimensionCupertino), minHeight: ((minimumSizeLocal?.height ?? DartCollectionRuntime.NullableMapValue<double>(ConstantsLibrary.kCupertinoButtonMinSize, ((CupertinoButton)this.widget).sizeStyle)) ?? ConstantsLibrary.kMinInteractiveDimensionCupertino)), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: this._opacityAnimation, child: new global::Doroti.Framework.Widgets.DecoratedBox(decoration: shapeDecoration, child: new global::Doroti.Framework.Widgets.Padding(padding: (((CupertinoButton)this.widget).padding ?? ConstantsLibrary.kCupertinoButtonPadding.GetValueOrDefault(((CupertinoButton)this.widget).sizeStyle)!), child: new global::Doroti.Framework.Widgets.Align(alignment: ((CupertinoButton)this.widget).alignment, widthFactor: 1.0, heightFactor: 1.0, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyle, child: new global::Doroti.Framework.Widgets.IconTheme(data: iconTheme, child: ((CupertinoButton)this.widget).child))))))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
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
        TickerModeData values = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTicker);
        newNotifier.addListener(this._updateTicker);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}
