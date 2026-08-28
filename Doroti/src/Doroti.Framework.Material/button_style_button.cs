// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button_style_button.dart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Doroti.Framework.Material;

public enum IconAlignment
{
    start,
    end
}

public abstract class ButtonStyleButton : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual bool? isSemanticButton { get; private set; }
    public virtual IconAlignment? iconAlignment { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    protected ButtonStyleButton(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = default!, global::System.Action<bool>? onHover = default!, global::System.Action<bool>? onFocusChange = default!, ButtonStyle? style = default!, global::Doroti.Framework.Widgets.FocusNode? focusNode = default!, bool autofocus = default!, Clip? clipBehavior = default!, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, bool? isSemanticButton = true, IconAlignment? iconAlignment = null, string? tooltip = null, global::Doroti.Framework.Widgets.Widget? child = default!) : base(key: key)
    {
        this.onPressed = onPressed;
        this.onLongPress = onLongPress;
        this.onHover = onHover;
        this.onFocusChange = onFocusChange;
        this.style = style;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.clipBehavior = clipBehavior;
        this.statesController = statesController;
        this.isSemanticButton = isSemanticButton;
        this.iconAlignment = iconAlignment;
        this.tooltip = tooltip;
        this.child = child;
    }

    public virtual ButtonStyle defaultStyleOf(global::Doroti.Framework.Widgets.BuildContext context) => default!;
    public virtual ButtonStyle? themeStyleOf(global::Doroti.Framework.Widgets.BuildContext context) => default;
    public virtual bool enabled => DartRuntimePrimitives.ConvertValue<bool>(((this.onPressed is not null) || (this.onLongPress is not null)));
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ButtonStyleState__button_style_button());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enabled", value: this.enabled, ifFalse: "disabled"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonStyle>("style", this.style, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.FocusNode>("focusNode", this.focusNode, defaultValue: null));
    }

    public static global::Doroti.Framework.Widgets.WidgetStateProperty<T>? allOrNull<T>(T? value) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<T>>(((value is null) ? null : new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<T>(value)));
    public static global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? defaultColor(Color? enabled, Color? disabled)
    {
        if ((((enabled ?? disabled)) is null))
        {
            return null;
        }
        return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Framework.Widgets.WidgetState.disabled.asConstraint()] = disabled, [global::Doroti.Framework.Widgets.WidgetStateMembers.any] = enabled }.cast<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Ui.Color?>()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Painting.EdgeInsetsGeometry scaledPadding(global::Doroti.Framework.Painting.EdgeInsetsGeometry geometry1x, global::Doroti.Framework.Painting.EdgeInsetsGeometry geometry2x, global::Doroti.Framework.Painting.EdgeInsetsGeometry geometry3x, double fontSizeMultiplier)
    {
        return (fontSizeMultiplier switch { <= 1L => geometry1x, < 2L => EdgeInsetsGeometry.lerp(geometry1x, geometry2x, (fontSizeMultiplier - 1L))!, < 3L => EdgeInsetsGeometry.lerp(geometry2x, geometry3x, (fontSizeMultiplier - 2L))!, _ => geometry3x });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ButtonStyleState__button_style_button : global::Doroti.Framework.Widgets.State<ButtonStyleButton>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<ButtonStyleButton>
{
    public virtual global::Doroti.Framework.Animation.AnimationController? controller { get; set; } = default;
    public virtual double? elevation { get; set; } = default;
    public virtual Color? backgroundColor { get; set; } = default;
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? internalStatesController { get; set; } = default;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public virtual void handleStatesControllerChange()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public virtual global::Doroti.Framework.Widgets.WidgetStatesController statesController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStatesController>((((ButtonStyleButton)this.widget).statesController ?? this.internalStatesController!));
    public virtual void initStatesController()
    {
        if ((((ButtonStyleButton)this.widget).statesController is null))
        {
            internalStatesController = new global::Doroti.Framework.Widgets.WidgetStatesController();
        }
        this.statesController.update(global::Doroti.Framework.Widgets.WidgetState.disabled, !((ButtonStyleButton)this.widget).enabled);
        this.statesController.addListener(this.handleStatesControllerChange);
    }

    public override void initState()
    {
        base.initState();
        initStatesController();
    }

    public override void didUpdateWidget(ButtonStyleButton oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((ButtonStyleButton)this.widget).statesController, ((ButtonStyleButton)oldWidget).statesController)))
        {
            ((ButtonStyleButton)oldWidget).statesController?.removeListener(this.handleStatesControllerChange);
            if ((((ButtonStyleButton)this.widget).statesController is not null))
            {
                this.internalStatesController?.dispose();
                internalStatesController = null;
            }
            initStatesController();
        }
        if ((((ButtonStyleButton)this.widget).enabled != ((ButtonStyleButton)oldWidget).enabled))
        {
            this.statesController.update(global::Doroti.Framework.Widgets.WidgetState.disabled, !((ButtonStyleButton)this.widget).enabled);
            if (!((ButtonStyleButton)this.widget).enabled)
            {
                this.statesController.update(global::Doroti.Framework.Widgets.WidgetState.pressed, false);
            }
        }
    }

    public override void dispose()
    {
        this.statesController.removeListener(this.handleStatesControllerChange);
        this.internalStatesController?.dispose();
        this.controller?.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = ((ThemeData)(object?)Theme.of(context));
        global::Doroti.Framework.Widgets.IconThemeData iconThemeLocal = ((global::Doroti.Framework.Widgets.IconThemeData)(object?)IconTheme.of(context));
        ButtonStyle? widgetStyle = ((ButtonStyleButton)this.widget).style;
        ButtonStyle? themeStyle = ((ButtonStyle?)(object?)this.widget.themeStyleOf(context));
        ButtonStyle defaultStyle = ((ButtonStyle)(object?)this.widget.defaultStyleOf(context));
        P? effectiveValue<P>(global::System.Func<ButtonStyle?, P?> getProperty)
        {
            P? widgetValue = getProperty(widgetStyle);
            P? themeValue = getProperty(themeStyle);
            P? defaultValue = getProperty(defaultStyle);
            return ((widgetValue ?? themeValue) ?? defaultValue);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(global::System.Func<ButtonStyle?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty)
        {
            return effectiveValue(((style) =>
            {
                return getProperty(style) is { } property ? property.resolve(this.statesController.value) : default;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        Color? effectiveIconColor()
        {
            // Icon color is optional for text-only buttons. Avoid eagerly
            // resolving nullable style properties; IconTheme below supplies
            // the foreground fallback when an icon is actually present.
            return null;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double? resolvedElevation = resolve<double?>(((style) => style?.elevation)) ?? 0.0;
        global::Doroti.Framework.Painting.TextStyle? resolvedTextStyle = resolve<global::Doroti.Framework.Painting.TextStyle?>(((style) => style?.textStyle));
        global::Doroti.Ui.Color? resolvedBackgroundColor = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((style) => style?.backgroundColor)));
        global::Doroti.Ui.Color? resolvedForegroundColor = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((style) => style?.foregroundColor)));
        global::Doroti.Ui.Color? resolvedShadowColor = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((style) => style?.shadowColor)));
        global::Doroti.Ui.Color? resolvedSurfaceTintColor = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((style) => style?.surfaceTintColor)));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? resolvedPadding = resolve<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(((style) => style?.padding)) ?? global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16, vertical: 8);
        global::Doroti.Ui.Size? resolvedMinimumSize = ((global::Doroti.Ui.Size?)(object?)resolve<global::Doroti.Ui.Size?>(((style) => style?.minimumSize))) ?? new global::Doroti.Ui.Size(64, 40);
        global::Doroti.Ui.Size? resolvedFixedSize = ((global::Doroti.Ui.Size?)(object?)resolve<global::Doroti.Ui.Size?>(((style) => style?.fixedSize)));
        global::Doroti.Ui.Size? resolvedMaximumSize = ((global::Doroti.Ui.Size?)(object?)resolve<global::Doroti.Ui.Size?>(((style) => style?.maximumSize))) ?? new global::Doroti.Ui.Size(double.PositiveInfinity, double.PositiveInfinity);
        global::Doroti.Ui.Color? resolvedIconColor = ((global::Doroti.Ui.Color?)(object?)effectiveIconColor());
        double? resolvedIconSize = resolve<double?>(((style) => style?.iconSize));
        global::Doroti.Framework.Painting.BorderSide? resolvedSide = resolve<global::Doroti.Framework.Painting.BorderSide?>(((style) => style?.side));
        global::Doroti.Framework.Painting.OutlinedBorder? resolvedShape = resolve<global::Doroti.Framework.Painting.OutlinedBorder?>(((style) => style?.shape)) ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder();
        global::Doroti.Framework.Widgets.WidgetStateMouseCursor mouseCursorLocal = ((global::Doroti.Framework.Widgets.WidgetStateMouseCursor)(object?)new _MouseCursor__button_style_button(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Services.MouseCursor?>)((states) => effectiveValue(((style) => style?.mouseCursor?.resolve(states)))))));
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> overlayColorLocal = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Ui.Color?>)((states) => effectiveValue(((style) => style?.overlayColor?.resolve(states)))))));
        VisualDensity? resolvedVisualDensity = effectiveValue(((style) => style?.visualDensity)) ?? theme.visualDensity;
        MaterialTapTargetSize? resolvedTapTargetSize = effectiveValue(((style) => style?.tapTargetSize)) ?? MaterialTapTargetSize.padded;
        Duration? resolvedAnimationDuration = effectiveValue(((style) => style?.animationDuration)) ?? Duration.Create(milliseconds: 200);
        bool resolvedEnableFeedback = (effectiveValue(((style) => style?.enableFeedback)) ?? true);
        global::Doroti.Framework.Painting.AlignmentGeometry? resolvedAlignment = effectiveValue(((style) => style?.alignment)) ?? global::Doroti.Framework.Painting.Alignment.center;
        global::Doroti.Ui.Offset densityAdjustment = ((global::Doroti.Ui.Offset)(object?)resolvedVisualDensity!.baseSizeAdjustment);
        InteractiveInkFeatureFactory? resolvedSplashFactory = effectiveValue(((style) => style?.splashFactory));
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? resolvedBackgroundBuilder = effectiveValue(((style) => style?.backgroundBuilder));
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? resolvedForegroundBuilder = effectiveValue(((style) => style?.foregroundBuilder));
        global::Doroti.Ui.Clip effectiveClipBehavior = (((ButtonStyleButton)this.widget).clipBehavior ?? ((((((resolvedBackgroundBuilder ?? (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)resolvedForegroundBuilder))) is not null) ? Clip.antiAlias : Clip.none)));
        global::Doroti.Framework.Rendering.BoxConstraints effectiveConstraintsLocal = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)resolvedVisualDensity.effectiveConstraints(new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: DartRuntimePrimitives.RequireValue(resolvedMinimumSize).width, minHeight: DartRuntimePrimitives.RequireValue(resolvedMinimumSize).height, maxWidth: DartRuntimePrimitives.RequireValue(resolvedMaximumSize).width, maxHeight: DartRuntimePrimitives.RequireValue(resolvedMaximumSize).height)));
        if ((resolvedFixedSize is not null))
        {
            Size resolvedFixedSize__16402__value18999 = DartRuntimePrimitives.RequireValue(resolvedFixedSize);
            global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)(object?)effectiveConstraintsLocal.constrain(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(resolvedFixedSize__16402__value18999))));
            if (double.IsFinite(sizeLocal.width))
            {
                effectiveConstraintsLocal = effectiveConstraintsLocal.copyWith(minWidth: sizeLocal.width, maxWidth: sizeLocal.width);
            }
            if (double.IsFinite(sizeLocal.height))
            {
                effectiveConstraintsLocal = effectiveConstraintsLocal.copyWith(minHeight: sizeLocal.height, maxHeight: sizeLocal.height);
            }
        }
        double dyLocal = densityAdjustment.dy;
        double dxLocal = Math.Max(0, densityAdjustment.dx);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)resolvedPadding!.add(new global::Doroti.Framework.Painting.EdgeInsets(dxLocal, dyLocal, dxLocal, dyLocal)).clamp(global::Doroti.Framework.Painting.EdgeInsets.zero, global::Doroti.Framework.Painting.EdgeInsetsGeometry.infinity));
        if (((((((((DartRuntimePrimitives.RequireValue(resolvedAnimationDuration) > Duration.zero) && (this.elevation is not null)) && (this.backgroundColor is not null)) && (this.elevation != resolvedElevation)) && (this.backgroundColor!.value != resolvedBackgroundColor!.value)) && (this.backgroundColor!.opacity == 1L)) && (resolvedBackgroundColor.opacity < 1L)) && (resolvedElevation == 0L)))
        {
            if ((!object.Equals(this.controller?.duration, DartRuntimePrimitives.RequireValue(resolvedAnimationDuration))))
            {
                this.controller?.dispose();
                controller = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: DartRuntimePrimitives.RequireValue(resolvedAnimationDuration), vsync: this);
    __cascade.addStatusListener(((AnimationStatusListener)((status) =>
    {
        if ((object.Equals(status, global::Doroti.Framework.Animation.AnimationStatus.completed)))
        {
            setState(((global::System.Action)(() =>
            {
            })));
        }
    })));
    return __cascade;
}))();
            }
            resolvedBackgroundColor = this.backgroundColor;
            this.controller!.value = 0;
            this.controller!.forward();
        }
        elevation = resolvedElevation;
        backgroundColor = resolvedBackgroundColor;
        global::Doroti.Framework.Widgets.Widget result = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal, child: new global::Doroti.Framework.Widgets.Align(alignment: resolvedAlignment!, widthFactor: 1.0, heightFactor: 1.0, child: ((resolvedForegroundBuilder is not null) ? resolvedForegroundBuilder(context, this.statesController.value, ((ButtonStyleButton)this.widget).child) : ((ButtonStyleButton)this.widget).child))));
        if ((resolvedBackgroundBuilder is not null))
        {
            result = resolvedBackgroundBuilder(context, this.statesController.value, result);
        }
        result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new AnimatedTheme(duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(resolvedAnimationDuration)), data: theme.copyWith(iconTheme: iconThemeLocal.merge(new global::Doroti.Framework.Widgets.IconThemeData(color: resolvedIconColor, size: resolvedIconSize))), child: new InkWell(onTap: ((ButtonStyleButton)this.widget).onPressed, onLongPress: ((ButtonStyleButton)this.widget).onLongPress, onHover: ((ButtonStyleButton)this.widget).onHover, mouseCursor: mouseCursorLocal, enableFeedback: resolvedEnableFeedback, focusNode: ((ButtonStyleButton)this.widget).focusNode, canRequestFocus: ((ButtonStyleButton)this.widget).enabled, onFocusChange: ((ButtonStyleButton)this.widget).onFocusChange, autofocus: ((ButtonStyleButton)this.widget).autofocus, splashFactory: resolvedSplashFactory, overlayColor: overlayColorLocal, highlightColor: Colors.transparent, customBorder: resolvedShape!.copyWith(side: resolvedSide), statesController: this.statesController, child: result)));
        if ((((ButtonStyleButton)this.widget).tooltip is not null))
        {
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Tooltip(message: ((ButtonStyleButton)this.widget).tooltip, child: result));
        }
        global::Doroti.Ui.Size minSizeLocal = default!;
        switch (DartRuntimePrimitives.RequireValue(resolvedTapTargetSize))
        {
            case MaterialTapTargetSize.padded:
                {
                    minSizeLocal = new global::Doroti.Ui.Size((global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dx), (global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dy));
                    DartRuntimePrimitives.Assert(() => (minSizeLocal.width >= 0.0));
                    DartRuntimePrimitives.Assert(() => (minSizeLocal.height >= 0.0));
                    break;
                }
            case MaterialTapTargetSize.shrinkWrap:
                {
                    minSizeLocal = Size.zero;
                    break;
                }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, button: ((ButtonStyleButton)this.widget).isSemanticButton, enabled: ((ButtonStyleButton)this.widget).enabled, child: new _InputPadding__button_style_button(minSize: minSizeLocal, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: effectiveConstraintsLocal, child: new Material(elevation: DartRuntimePrimitives.RequireValue(resolvedElevation), textStyle: resolvedTextStyle?.copyWith(color: resolvedForegroundColor), shape: resolvedShape.copyWith(side: resolvedSide), color: resolvedBackgroundColor, shadowColor: resolvedShadowColor, surfaceTintColor: resolvedSurfaceTintColor, type: ((resolvedBackgroundColor is null) ? MaterialType.transparency : MaterialType.button), animationDuration: DartRuntimePrimitives.RequireValue(resolvedAnimationDuration), clipBehavior: effectiveClipBehavior, borderOnForeground: false, child: result)))));
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
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _MouseCursor__button_style_button : global::Doroti.Framework.Widgets.WidgetStateMouseCursor
{
    public virtual global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Services.MouseCursor?> resolveCallback { get; private set; } = default!;

    internal _MouseCursor__button_style_button(global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Services.MouseCursor?> resolveCallback)
    {
        this.resolveCallback = resolveCallback;
    }

    public override global::Doroti.Framework.Services.MouseCursor resolve(HashSet<global::Doroti.Framework.Widgets.WidgetState> states) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.MouseCursor>(this.resolveCallback(states)!);
    public override string debugDescription => "ButtonStyleButton_MouseCursor";
}

internal class _InputPadding__button_style_button : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual Size minSize { get; private set; } = default!;

    internal _InputPadding__button_style_button(global::Doroti.Framework.Widgets.Widget? child = null, Size minSize = default!) : base(child: child)
    {
        this.minSize = minSize;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderInputPadding__button_style_button(this.minSize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderInputPadding__button_style_button)(object)renderObject;
        __renderObject.minSize = this.minSize;
    }

}

public class _RenderInputPadding__button_style_button : global::Doroti.Framework.Rendering.RenderShiftedBox
{
    internal virtual Size _minSize { get; set; } = default!;

    internal _RenderInputPadding__button_style_button(Size _minSize, global::Doroti.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this._minSize = _minSize;
    }

    public virtual global::Doroti.Ui.Size minSize
    {
        get => this._minSize;
        set
        {
            var __value = value;
            if ((object.Equals(this._minSize, __value)))
            {
                return;
            }
            _minSize = __value;
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        if ((this.child is not null))
        {
            return Math.Max(this.child!.getMinIntrinsicWidth(height), this.minSize.width);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if ((this.child is not null))
        {
            return Math.Max(this.child!.getMinIntrinsicHeight(width), this.minSize.height);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if ((this.child is not null))
        {
            return Math.Max(this.child!.getMaxIntrinsicWidth(height), this.minSize.width);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if ((this.child is not null))
        {
            return Math.Max(this.child!.getMaxIntrinsicHeight(width), this.minSize.height);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeSize(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild)
    {
        if ((this.child is not null))
        {
            global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)layoutChild(this.child!, constraints));
            double widthLocal = Math.Max(childSize.width, this.minSize.width);
            double heightLocal = Math.Max(childSize.height, this.minSize.height);
            return ((global::Doroti.Ui.Size)(object?)constraints.constrain(new global::Doroti.Ui.Size(widthLocal, heightLocal)));
        }
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return null;
        }
        double? result = childLocal.getDryBaseline(constraints, baseline);
        if ((result is null))
        {
            return null;
        }
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)childLocal.getDryLayout(constraints));
        return (DartRuntimePrimitives.RequireValue(result) + global::Doroti.Framework.Painting.Alignment.center.alongOffset((getDryLayout(constraints) - childSize)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: this.constraints, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.layoutChild);
        if ((this.child is not null))
        {
            var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
            childParentData.offset = global::Doroti.Framework.Painting.Alignment.center.alongOffset((this.size - this.child!.size));
        }
    }

    public override bool hitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if (base.hitTest(result, position: position))
        {
            return true;
        }
        global::Doroti.Ui.Offset centerLocal = ((global::Doroti.Ui.Offset)(object?)this.child!.size.center(Offset.zero));
        return result.addWithRawTransform(transform: MatrixUtils.forceToPoint(centerLocal), position: centerLocal, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, position) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(position, centerLocal)));
            return this.child!.hitTest(result, position: centerLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
