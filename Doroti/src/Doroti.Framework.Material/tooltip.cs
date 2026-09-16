// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tooltip.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class Tooltip : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual string? message { get; private set; }
    public virtual global::Doroti.Framework.Painting.InlineSpan? richMessage { get; private set; }
    public virtual double? height { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual double? verticalOffset { get; private set; }
    public virtual bool? preferBelow { get; private set; }
    public virtual bool? excludeFromSemantics { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Framework.Painting.Decoration? decoration { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual TextAlign? textAlign { get; private set; }
    public virtual Duration? waitDuration { get; private set; }
    public virtual Duration? showDuration { get; private set; }
    public virtual Duration? exitDuration { get; private set; }
    public virtual bool enableTapToDismiss { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.TooltipTriggerMode? triggerMode { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual global::System.Action? onTriggered { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual bool? ignorePointer { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.TooltipPositionContext, Offset>? positionDelegate { get; private set; }

    public Tooltip(global::Doroti.Framework.Foundation.Key? key = null, string? message = null, global::Doroti.Framework.Painting.InlineSpan? richMessage = null, double? height = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, double? verticalOffset = null, bool? preferBelow = null, bool? excludeFromSemantics = null, global::Doroti.Framework.Painting.Decoration? decoration = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, TextAlign? textAlign = null, Duration? waitDuration = null, Duration? showDuration = null, Duration? exitDuration = null, bool enableTapToDismiss = true, global::Doroti.Framework.Widgets.TooltipTriggerMode? triggerMode = null, bool? enableFeedback = null, global::System.Action? onTriggered = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, bool? ignorePointer = null, global::System.Func<global::Doroti.Framework.Widgets.TooltipPositionContext, Offset>? positionDelegate = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key)
    {
        this.message = message;
        this.richMessage = richMessage;
        this.height = height;
        this.constraints = constraints;
        this.padding = padding;
        this.margin = margin;
        this.verticalOffset = verticalOffset;
        this.preferBelow = preferBelow;
        this.excludeFromSemantics = excludeFromSemantics;
        this.decoration = decoration;
        this.textStyle = textStyle;
        this.textAlign = textAlign;
        this.waitDuration = waitDuration;
        this.showDuration = showDuration;
        this.exitDuration = exitDuration;
        this.enableTapToDismiss = enableTapToDismiss;
        this.triggerMode = triggerMode;
        this.enableFeedback = enableFeedback;
        this.onTriggered = onTriggered;
        this.mouseCursor = mouseCursor;
        this.ignorePointer = ignorePointer;
        this.positionDelegate = positionDelegate;
        this.child = child;
        System.Diagnostics.Debug.Assert(message is null != richMessage is null);
        System.Diagnostics.Debug.Assert((height is null) || (constraints is null));
    }

    public static bool dismissAllToolTips()
    {
        return RawTooltip.dismissAllToolTips();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new TooltipState());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("message", message, showName: message is null, defaultValue: (message is null) ? null : DiagnosticsLibrary.kNoDefaultValue.ToString()));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("richMessage", richMessage?.toPlainText(), showName: richMessage is null, defaultValue: (richMessage is null) ? null : DiagnosticsLibrary.kNoDefaultValue.ToString()));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("height", height, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("constraints", constraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("margin", margin, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("vertical offset", verticalOffset, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("position", value: preferBelow, ifTrue: "below", ifFalse: "above", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("semantics", value: excludeFromSemantics, ifTrue: "excluded", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("wait duration", waitDuration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("show duration", showDuration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("exit duration", exitDuration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.TooltipTriggerMode>("triggerMode", triggerMode, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enableFeedback", value: enableFeedback, ifTrue: "true", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextAlign>("textAlign", textAlign, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::System.Func<global::Doroti.Framework.Widgets.TooltipPositionContext, Offset>>("positionDelegate", positionDelegate, defaultValue: null));
    }

}

public class TooltipState : global::Doroti.Framework.Widgets.State<Tooltip>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<Tooltip>
{
    internal const double _defaultVerticalOffset = 24.0;
    internal const bool _defaultPreferBelow = true;
    internal static global::Doroti.Framework.Painting.EdgeInsetsGeometry _defaultMargin = EdgeInsets.zero;
    internal static Duration _defaultShowDuration = Duration.Create(milliseconds: 1500L);
    internal static Duration _defaultExitDuration = Duration.Create(milliseconds: 100L);
    internal static Duration _defaultWaitDuration = Duration.zero;
    internal const bool _defaultExcludeFromSemantics = false;
    internal static global::Doroti.Framework.Widgets.TooltipTriggerMode _defaultTriggerMode = TooltipTriggerMode.longPress;
    internal const bool _defaultEnableFeedback = true;
    internal static TextAlign _defaultTextAlign = TextAlign.start;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.RawTooltipState> _tooltipKey { get; private set; } = GlobalKey<RawTooltipState>.Create();
    internal virtual bool _visible { get; set; } = default!;
    internal virtual TooltipThemeData _tooltipTheme { get; set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual string _tooltipMessage => DartRuntimePrimitives.ConvertValue<string>(widget.message ?? widget.richMessage!.toPlainText());
    public virtual bool ensureTooltipVisible()
    {
        return _tooltipKey.currentState?.ensureTooltipVisible() ?? false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _visible = TooltipVisibility.of(context);
        _tooltipTheme = TooltipTheme.of(context);
    }

    internal virtual double _getDefaultTooltipHeight()
    {
        return Theme.of(context).platform switch { TargetPlatform.macOS or TargetPlatform.linux => 24.0, TargetPlatform.windows => 24.0, TargetPlatform.android or TargetPlatform.fuchsia => 32.0, TargetPlatform.iOS => 32.0, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsets _getDefaultPadding()
    {
        return Theme.of(context).platform switch { TargetPlatform.macOS or TargetPlatform.linux => EdgeInsets.CreateSymmetric(horizontal: 8.0, vertical: 4.0), TargetPlatform.windows => EdgeInsets.CreateSymmetric(horizontal: 8.0, vertical: 4.0), TargetPlatform.android or TargetPlatform.fuchsia => EdgeInsets.CreateSymmetric(horizontal: 16.0, vertical: 4.0), TargetPlatform.iOS => EdgeInsets.CreateSymmetric(horizontal: 16.0, vertical: 4.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _getDefaultFontSize(global::Doroti.Framework.Foundation.TargetPlatform platform)
    {
        return platform switch { TargetPlatform.macOS or TargetPlatform.linux => 12.0, TargetPlatform.windows => 12.0, TargetPlatform.android or TargetPlatform.fuchsia => 14.0, TargetPlatform.iOS => 14.0, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _getDefaultPositionDelegate(global::Doroti.Framework.Widgets.TooltipPositionContext context)
    {
        double effectiveVerticalOffset = (widget.verticalOffset ?? _tooltipTheme.verticalOffset) ?? _defaultVerticalOffset;
        bool effectivePreferBelow = (widget.preferBelow ?? _tooltipTheme.preferBelow) ?? _defaultPreferBelow;
        var resolvedContext = new global::Doroti.Framework.Widgets.TooltipPositionContext(target: context.target, targetSize: context.targetSize, tooltipSize: context.tooltipSize, overlaySize: context.overlaySize, verticalOffset: effectiveVerticalOffset, preferBelow: effectivePreferBelow);
        return widget.positionDelegate is null ? GeometryLibrary.positionDependentBox(size: context.overlaySize, childSize: context.tooltipSize, target: context.target, verticalOffset: effectiveVerticalOffset, preferBelow: effectivePreferBelow) : widget.positionDelegate.Invoke(resolvedContext);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (_tooltipMessage.Length == 0)
        {
            return widget.child ?? SizedBox.CreateShrink();
        }
        var (defaultTextStyle, defaultDecoration) = Theme.of(context) switch { ThemeData { brightness: Brightness.dark, textTheme: TextTheme textThemeLocal, platform: global::Doroti.Framework.Foundation.TargetPlatform platformLocal } __object18397 => ((global::Doroti.Framework.Painting.TextStyle, global::Doroti.Framework.Painting.BoxDecoration))(textThemeLocal.bodyMedium!.copyWith(color: Colors.black, fontSize: _getDefaultFontSize(platformLocal)), new global::Doroti.Framework.Painting.BoxDecoration(color: Colors.white.withOpacity(0.9), borderRadius: BorderRadius.CreateAll(Radius.circular(4)))), ThemeData { brightness: Brightness.light, textTheme: TextTheme textThemeAlternate, platform: global::Doroti.Framework.Foundation.TargetPlatform platformAlternate } __object18858 => ((global::Doroti.Framework.Painting.TextStyle, global::Doroti.Framework.Painting.BoxDecoration))(textThemeAlternate.bodyMedium!.copyWith(color: Colors.white, fontSize: _getDefaultFontSize(platformAlternate)), new global::Doroti.Framework.Painting.BoxDecoration(color: Colors.grey[700L]!.withOpacity(0.9), borderRadius: BorderRadius.CreateAll(Radius.circular(4)))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        var defaultConstraints = new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: (widget.height ?? _tooltipTheme.height) ?? (double)_getDefaultTooltipHeight());
        global::Doroti.Framework.Widgets.Widget tooltipBox = new _TooltipBox__tooltip(constraints: (widget.constraints ?? _tooltipTheme.constraints) ?? defaultConstraints, textStyle: (widget.textStyle ?? _tooltipTheme.textStyle) ?? defaultTextStyle, textAlign: (widget.textAlign ?? _tooltipTheme.textAlign) ?? _defaultTextAlign, decoration: (widget.decoration ?? _tooltipTheme.decoration) ?? defaultDecoration, padding: (widget.padding ?? _tooltipTheme.padding) ?? _getDefaultPadding(), margin: (widget.margin ?? _tooltipTheme.margin) ?? _defaultMargin, richMessage: widget.richMessage ?? new global::Doroti.Framework.Painting.TextSpan(text: widget.message));
        global::Doroti.Framework.Widgets.Widget effectiveChild = new global::Doroti.Framework.Widgets.MouseRegion(cursor: widget.mouseCursor ?? MouseCursor.defer, child: widget.child ?? SizedBox.CreateShrink());
        bool excludeFromSemanticsLocal = (widget.excludeFromSemantics ?? _tooltipTheme.excludeFromSemantics) ?? _defaultExcludeFromSemantics;
        if (_visible)
        {
            effectiveChild = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.RawTooltip(key: _tooltipKey, semanticsTooltip: excludeFromSemanticsLocal ? null : _tooltipMessage, tooltipBuilder: (context, animation) => new global::Doroti.Framework.Widgets.FadeTransition(opacity: animation, child: tooltipBox), touchDelay: (widget.showDuration ?? _tooltipTheme.showDuration) ?? _defaultShowDuration, triggerMode: (widget.triggerMode ?? _tooltipTheme.triggerMode) ?? _defaultTriggerMode, enableFeedback: (widget.enableFeedback ?? _tooltipTheme.enableFeedback) ?? _defaultEnableFeedback, hoverDelay: (widget.waitDuration ?? _tooltipTheme.waitDuration) ?? _defaultWaitDuration, enableTapToDismiss: widget.enableTapToDismiss, onTriggered: widget.onTriggered, dismissDelay: (widget.exitDuration ?? _tooltipTheme.exitDuration) ?? _defaultExitDuration, positionDelegate: _getDefaultPositionDelegate, ignorePointer: widget.ignorePointer ?? (widget.message is not null), child: effectiveChild));
        }
        return effectiveChild;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        _ticker = new global::Doroti.Framework.Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
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
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

internal class _TooltipBox__tooltip : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle textStyle { get; private set; } = default!;
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Decoration? decoration { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual global::Doroti.Framework.Painting.InlineSpan richMessage { get; private set; } = default!;

    internal _TooltipBox__tooltip(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::Doroti.Framework.Painting.TextStyle textStyle, TextAlign textAlign, global::Doroti.Framework.Painting.Decoration? decoration, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin, global::Doroti.Framework.Painting.InlineSpan richMessage)
    {
        this.constraints = constraints;
        this.textStyle = textStyle;
        this.textAlign = textAlign;
        this.decoration = decoration;
        this.padding = padding;
        this.margin = margin;
        this.richMessage = richMessage;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: constraints, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyle, textAlign: textAlign, child: new global::Doroti.Framework.Widgets.Container(decoration: decoration, padding: padding, margin: margin, child: new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: Text.CreateRich(richMessage, style: textStyle, textAlign: textAlign)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
