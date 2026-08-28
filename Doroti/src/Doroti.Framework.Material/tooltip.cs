// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tooltip.dart
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
        System.Diagnostics.Debug.Assert((((message is null)) != ((richMessage is null))));
        System.Diagnostics.Debug.Assert(((height is null) || (constraints is null)));
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
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("message", this.message, showName: (this.message is null), defaultValue: ((this.message is null) ? null : global::Doroti.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue.ToString())));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("richMessage", this.richMessage?.toPlainText(), showName: (this.richMessage is null), defaultValue: ((this.richMessage is null) ? null : global::Doroti.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue.ToString())));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("constraints", this.constraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("margin", this.margin, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("vertical offset", this.verticalOffset, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("position", value: this.preferBelow, ifTrue: "below", ifFalse: "above", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("semantics", value: this.excludeFromSemantics, ifTrue: "excluded", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("wait duration", this.waitDuration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("show duration", this.showDuration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("exit duration", this.exitDuration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.TooltipTriggerMode>("triggerMode", this.triggerMode, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enableFeedback", value: this.enableFeedback, ifTrue: "true", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextAlign>("textAlign", this.textAlign, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::System.Func<global::Doroti.Framework.Widgets.TooltipPositionContext, Offset>>("positionDelegate", this.positionDelegate, defaultValue: null));
    }

}

public class TooltipState : global::Doroti.Framework.Widgets.State<Tooltip>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<Tooltip>
{
    internal const double _defaultVerticalOffset = 24.0;
    internal const bool _defaultPreferBelow = true;
    internal static global::Doroti.Framework.Painting.EdgeInsetsGeometry _defaultMargin = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Framework.Painting.EdgeInsets.zero);
    internal static Duration _defaultShowDuration = Duration.Create(milliseconds: 1500L);
    internal static Duration _defaultExitDuration = Duration.Create(milliseconds: 100L);
    internal static Duration _defaultWaitDuration = Duration.zero;
    internal const bool _defaultExcludeFromSemantics = false;
    internal static global::Doroti.Framework.Widgets.TooltipTriggerMode _defaultTriggerMode = global::Doroti.Framework.Widgets.TooltipTriggerMode.longPress;
    internal const bool _defaultEnableFeedback = true;
    internal static TextAlign _defaultTextAlign = global::Doroti.Ui.TextAlign.start;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.RawTooltipState> _tooltipKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.RawTooltipState>.Create();
    internal virtual bool _visible { get; set; } = default!;
    internal virtual TooltipThemeData _tooltipTheme { get; set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual string _tooltipMessage => DartRuntimePrimitives.ConvertValue<string>(((((Tooltip)this.widget).message ?? (string)((Tooltip)this.widget).richMessage!.toPlainText())));
    public virtual bool ensureTooltipVisible()
    {
        return (((global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.RawTooltipState>)this._tooltipKey).currentState?.ensureTooltipVisible() ?? false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _visible = TooltipVisibility.of(this.context);
        _tooltipTheme = TooltipTheme.of(this.context);
    }

    internal virtual double _getDefaultTooltipHeight()
    {
        return (Theme.of(this.context).platform switch { global::Doroti.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Framework.Foundation.TargetPlatform.linux => 24.0, global::Doroti.Framework.Foundation.TargetPlatform.windows => 24.0, global::Doroti.Framework.Foundation.TargetPlatform.android or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia => 32.0, global::Doroti.Framework.Foundation.TargetPlatform.iOS => 32.0, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsets _getDefaultPadding()
    {
        return (Theme.of(this.context).platform switch { global::Doroti.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Framework.Foundation.TargetPlatform.linux => global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0, vertical: 4.0), global::Doroti.Framework.Foundation.TargetPlatform.windows => global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0, vertical: 4.0), global::Doroti.Framework.Foundation.TargetPlatform.android or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia => global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0, vertical: 4.0), global::Doroti.Framework.Foundation.TargetPlatform.iOS => global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0, vertical: 4.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _getDefaultFontSize(global::Doroti.Framework.Foundation.TargetPlatform platform)
    {
        return (platform switch { global::Doroti.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Framework.Foundation.TargetPlatform.linux => 12.0, global::Doroti.Framework.Foundation.TargetPlatform.windows => 12.0, global::Doroti.Framework.Foundation.TargetPlatform.android or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia => 14.0, global::Doroti.Framework.Foundation.TargetPlatform.iOS => 14.0, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _getDefaultPositionDelegate(global::Doroti.Framework.Widgets.TooltipPositionContext context)
    {
        double effectiveVerticalOffset = ((((Tooltip)this.widget).verticalOffset ?? this._tooltipTheme.verticalOffset) ?? _defaultVerticalOffset);
        bool effectivePreferBelow = ((((Tooltip)this.widget).preferBelow ?? this._tooltipTheme.preferBelow) ?? _defaultPreferBelow);
        var resolvedContext = new global::Doroti.Framework.Widgets.TooltipPositionContext(target: ((global::Doroti.Framework.Widgets.TooltipPositionContext)context).target, targetSize: ((global::Doroti.Framework.Widgets.TooltipPositionContext)context).targetSize, tooltipSize: ((global::Doroti.Framework.Widgets.TooltipPositionContext)context).tooltipSize, overlaySize: ((global::Doroti.Framework.Widgets.TooltipPositionContext)context).overlaySize, verticalOffset: effectiveVerticalOffset, preferBelow: effectivePreferBelow);
        return ((((Tooltip)this.widget).positionDelegate is null ? global::Doroti.Framework.Painting.GeometryLibrary.positionDependentBox(size: ((global::Doroti.Framework.Widgets.TooltipPositionContext)context).overlaySize, childSize: ((global::Doroti.Framework.Widgets.TooltipPositionContext)context).tooltipSize, target: ((global::Doroti.Framework.Widgets.TooltipPositionContext)context).target, verticalOffset: effectiveVerticalOffset, preferBelow: effectivePreferBelow) : ((Tooltip)this.widget).positionDelegate.Invoke(resolvedContext)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((this._tooltipMessage.Length == 0))
        {
            return (((Tooltip)this.widget).child ?? global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        var (defaultTextStyle, defaultDecoration) = (Theme.of(context) switch { ThemeData { brightness: Brightness.dark, textTheme: TextTheme textThemeLocal, platform: global::Doroti.Framework.Foundation.TargetPlatform platformLocal } __object18397 => (((global::Doroti.Framework.Painting.TextStyle, global::Doroti.Framework.Painting.BoxDecoration))((textThemeLocal.bodyMedium!.copyWith(color: Colors.black, fontSize: TooltipState._getDefaultFontSize(platformLocal)), new global::Doroti.Framework.Painting.BoxDecoration(color: Colors.white.withOpacity(0.9), borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4)))))), ThemeData { brightness: Brightness.light, textTheme: TextTheme textThemeAlternate, platform: global::Doroti.Framework.Foundation.TargetPlatform platformAlternate } __object18858 => (((global::Doroti.Framework.Painting.TextStyle, global::Doroti.Framework.Painting.BoxDecoration))((textThemeAlternate.bodyMedium!.copyWith(color: Colors.white, fontSize: TooltipState._getDefaultFontSize(platformAlternate)), new global::Doroti.Framework.Painting.BoxDecoration(color: Colors.grey[700L]!.withOpacity(0.9), borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4)))))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var defaultConstraints = new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: (((((Tooltip)this.widget).height ?? this._tooltipTheme.height) ?? (double)_getDefaultTooltipHeight())));
        global::Doroti.Framework.Widgets.Widget tooltipBox = ((global::Doroti.Framework.Widgets.Widget)(object?)new _TooltipBox__tooltip(constraints: ((((Tooltip)this.widget).constraints ?? this._tooltipTheme.constraints) ?? defaultConstraints), textStyle: ((((Tooltip)this.widget).textStyle ?? this._tooltipTheme.textStyle) ?? defaultTextStyle), textAlign: ((((Tooltip)this.widget).textAlign ?? this._tooltipTheme.textAlign) ?? _defaultTextAlign), decoration: ((((Tooltip)this.widget).decoration ?? this._tooltipTheme.decoration) ?? defaultDecoration), padding: (((((Tooltip)this.widget).padding ?? this._tooltipTheme.padding) ?? (global::Doroti.Framework.Painting.EdgeInsetsGeometry)_getDefaultPadding())), margin: ((((Tooltip)this.widget).margin ?? this._tooltipTheme.margin) ?? _defaultMargin), richMessage: (((Tooltip)this.widget).richMessage ?? new global::Doroti.Framework.Painting.TextSpan(text: ((Tooltip)this.widget).message))));
        global::Doroti.Framework.Widgets.Widget effectiveChild = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.MouseRegion(cursor: (((Tooltip)this.widget).mouseCursor ?? global::Doroti.Framework.Services.MouseCursor.defer), child: (((Tooltip)this.widget).child ?? global::Doroti.Framework.Widgets.SizedBox.CreateShrink())));
        bool excludeFromSemanticsLocal = ((((Tooltip)this.widget).excludeFromSemantics ?? this._tooltipTheme.excludeFromSemantics) ?? _defaultExcludeFromSemantics);
        if (this._visible)
        {
            effectiveChild = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.RawTooltip(key: this._tooltipKey, semanticsTooltip: (excludeFromSemanticsLocal ? null : this._tooltipMessage), tooltipBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)((context, animation) => new global::Doroti.Framework.Widgets.FadeTransition(opacity: animation, child: tooltipBox))), touchDelay: ((((Tooltip)this.widget).showDuration ?? this._tooltipTheme.showDuration) ?? _defaultShowDuration), triggerMode: ((((Tooltip)this.widget).triggerMode ?? this._tooltipTheme.triggerMode) ?? _defaultTriggerMode), enableFeedback: ((((Tooltip)this.widget).enableFeedback ?? this._tooltipTheme.enableFeedback) ?? _defaultEnableFeedback), hoverDelay: ((((Tooltip)this.widget).waitDuration ?? this._tooltipTheme.waitDuration) ?? _defaultWaitDuration), enableTapToDismiss: ((Tooltip)this.widget).enableTapToDismiss, onTriggered: () => ((Tooltip)this.widget).onTriggered(), dismissDelay: ((((Tooltip)this.widget).exitDuration ?? this._tooltipTheme.exitDuration) ?? _defaultExitDuration), positionDelegate: (global::System.Func<global::Doroti.Framework.Widgets.TooltipPositionContext, Offset>)this._getDefaultPositionDelegate, ignorePointer: (((Tooltip)this.widget).ignorePointer ?? (((Tooltip)this.widget).message is not null)), child: effectiveChild));
        }
        return effectiveChild;
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

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(this._updateTicker);
        this._tickerModeNotifier = null;
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
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: this.constraints, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: this.textStyle, textAlign: this.textAlign, child: new global::Doroti.Framework.Widgets.Container(decoration: this.decoration, padding: this.padding, margin: this.margin, child: new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: global::Doroti.Framework.Widgets.Text.CreateRich(this.richMessage, style: this.textStyle, textAlign: this.textAlign))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
