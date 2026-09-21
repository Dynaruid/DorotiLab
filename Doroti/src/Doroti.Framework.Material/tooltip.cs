// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tooltip.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class Tooltip : StatefulWidget
{
    public virtual string? message { get; private set; }
    public virtual InlineSpan? richMessage { get; private set; }
    public virtual double? height { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual EdgeInsetsGeometry? margin { get; private set; }
    public virtual double? verticalOffset { get; private set; }
    public virtual bool? preferBelow { get; private set; }
    public virtual bool? excludeFromSemantics { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual Decoration? decoration { get; private set; }
    public virtual TextStyle? textStyle { get; private set; }
    public virtual TextAlign? textAlign { get; private set; }
    public virtual Duration? waitDuration { get; private set; }
    public virtual Duration? showDuration { get; private set; }
    public virtual Duration? exitDuration { get; private set; }
    public virtual bool enableTapToDismiss { get; private set; } = default!;
    public virtual TooltipTriggerMode? triggerMode { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual Action? onTriggered { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual bool? ignorePointer { get; private set; }
    public virtual Func<TooltipPositionContext, Offset>? positionDelegate { get; private set; }

    public Tooltip(
        Key? key = null,
        string? message = null,
        InlineSpan? richMessage = null,
        double? height = null,
        BoxConstraints? constraints = null,
        EdgeInsetsGeometry? padding = null,
        EdgeInsetsGeometry? margin = null,
        double? verticalOffset = null,
        bool? preferBelow = null,
        bool? excludeFromSemantics = null,
        Decoration? decoration = null,
        TextStyle? textStyle = null,
        TextAlign? textAlign = null,
        Duration? waitDuration = null,
        Duration? showDuration = null,
        Duration? exitDuration = null,
        bool enableTapToDismiss = true,
        TooltipTriggerMode? triggerMode = null,
        bool? enableFeedback = null,
        Action? onTriggered = null,
        MouseCursor? mouseCursor = null,
        bool? ignorePointer = null,
        Func<TooltipPositionContext, Offset>? positionDelegate = null,
        Widget? child = null
    )
        : base(key: key)
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
        System.Diagnostics.Debug.Assert((message is null) != (richMessage is null));
        System.Diagnostics.Debug.Assert((height is null) || (constraints is null));
    }

    public static bool dismissAllToolTips()
    {
        return RawTooltip.dismissAllToolTips();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new TooltipState());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new StringProperty(
                "message",
                message,
                showName: message is null,
                defaultValue: (message is null)
                    ? null
                    : DiagnosticsLibrary.kNoDefaultValue.ToString()
            )
        );
        properties.add(
            new StringProperty(
                "richMessage",
                richMessage?.toPlainText(),
                showName: richMessage is null,
                defaultValue: (richMessage is null)
                    ? null
                    : DiagnosticsLibrary.kNoDefaultValue.ToString()
            )
        );
        properties.add(new DoubleProperty("height", height, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<BoxConstraints>("constraints", constraints, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>("margin", margin, defaultValue: null)
        );
        properties.add(new DoubleProperty("vertical offset", verticalOffset, defaultValue: null));
        properties.add(
            new FlagProperty(
                "position",
                value: preferBelow,
                ifTrue: "below",
                ifFalse: "above",
                showName: true
            )
        );
        properties.add(
            new FlagProperty(
                "semantics",
                value: excludeFromSemantics,
                ifTrue: "excluded",
                showName: true
            )
        );
        properties.add(
            new DiagnosticsProperty<Duration>("wait duration", waitDuration, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Duration>("show duration", showDuration, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Duration>("exit duration", exitDuration, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TooltipTriggerMode>(
                "triggerMode",
                triggerMode,
                defaultValue: null
            )
        );
        properties.add(
            new FlagProperty(
                "enableFeedback",
                value: enableFeedback,
                ifTrue: "true",
                showName: true
            )
        );
        properties.add(
            new DiagnosticsProperty<TextAlign>("textAlign", textAlign, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Func<TooltipPositionContext, Offset>>(
                "positionDelegate",
                positionDelegate,
                defaultValue: null
            )
        );
    }
}

public class TooltipState : State<Tooltip>, SingleTickerProviderStateMixin<Tooltip>
{
    internal const double _defaultVerticalOffset = 24.0;
    internal const bool _defaultPreferBelow = true;
    internal static EdgeInsetsGeometry _defaultMargin = EdgeInsets.zero;
    internal static Duration _defaultShowDuration = Duration.Create(milliseconds: 1500L);
    internal static Duration _defaultExitDuration = Duration.Create(milliseconds: 100L);
    internal static Duration _defaultWaitDuration = Duration.zero;
    internal const bool _defaultExcludeFromSemantics = false;
    internal static TooltipTriggerMode _defaultTriggerMode = TooltipTriggerMode.longPress;
    internal const bool _defaultEnableFeedback = true;
    internal static TextAlign _defaultTextAlign = TextAlign.start;
    internal virtual GlobalKey<RawTooltipState> _tooltipKey { get; private set; } =
        GlobalKey<RawTooltipState>.Create();
    internal virtual bool _visible { get; set; } = default!;
    internal virtual TooltipThemeData _tooltipTheme { get; set; } = default!;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual string _tooltipMessage =>
        DartRuntimePrimitives.ConvertValue<string>(
            widget.message ?? widget.richMessage!.toPlainText()
        );

    public virtual bool ensureTooltipVisible()
    {
        return _tooltipKey.currentState?.ensureTooltipVisible() ?? false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _visible = TooltipVisibility.of(context);
        _tooltipTheme = TooltipTheme.of(context);
    }

    internal virtual double _getDefaultTooltipHeight()
    {
        return Theme.of(context).platform switch
        {
            TargetPlatform.macOS or TargetPlatform.linux => 24.0,
            TargetPlatform.windows => 24.0,
            TargetPlatform.android or TargetPlatform.fuchsia => 32.0,
            TargetPlatform.iOS => 32.0,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual EdgeInsets _getDefaultPadding()
    {
        return Theme.of(context).platform switch
        {
            TargetPlatform.macOS or TargetPlatform.linux => EdgeInsets.CreateSymmetric(
                horizontal: 8.0,
                vertical: 4.0
            ),
            TargetPlatform.windows => EdgeInsets.CreateSymmetric(horizontal: 8.0, vertical: 4.0),
            TargetPlatform.android or TargetPlatform.fuchsia => EdgeInsets.CreateSymmetric(
                horizontal: 16.0,
                vertical: 4.0
            ),
            TargetPlatform.iOS => EdgeInsets.CreateSymmetric(horizontal: 16.0, vertical: 4.0),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static double _getDefaultFontSize(TargetPlatform platform)
    {
        return platform switch
        {
            TargetPlatform.macOS or TargetPlatform.linux => 12.0,
            TargetPlatform.windows => 12.0,
            TargetPlatform.android or TargetPlatform.fuchsia => 14.0,
            TargetPlatform.iOS => 14.0,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Offset _getDefaultPositionDelegate(TooltipPositionContext context)
    {
        double effectiveVerticalOffset =
            (widget.verticalOffset ?? _tooltipTheme.verticalOffset) ?? _defaultVerticalOffset;
        bool effectivePreferBelow =
            (widget.preferBelow ?? _tooltipTheme.preferBelow) ?? _defaultPreferBelow;
        var resolvedContext = new TooltipPositionContext(
            target: context.target,
            targetSize: context.targetSize,
            tooltipSize: context.tooltipSize,
            overlaySize: context.overlaySize,
            verticalOffset: effectiveVerticalOffset,
            preferBelow: effectivePreferBelow
        );
        return widget.positionDelegate is null
            ? GeometryLibrary.positionDependentBox(
                size: context.overlaySize,
                childSize: context.tooltipSize,
                target: context.target,
                verticalOffset: effectiveVerticalOffset,
                preferBelow: effectivePreferBelow
            )
            : widget.positionDelegate.Invoke(resolvedContext);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        if (_tooltipMessage.Length == 0)
        {
            return widget.child ?? SizedBox.CreateShrink();
        }
        var (defaultTextStyle, defaultDecoration) = Theme.of(context) switch
        {
            ThemeData
            {
                brightness: Brightness.dark,
                textTheme: TextTheme textThemeLocal,
                platform: TargetPlatform platformLocal
            } __object18397 => ((TextStyle, BoxDecoration))
                (
                    textThemeLocal.bodyMedium!.copyWith(
                        color: Colors.black,
                        fontSize: _getDefaultFontSize(platformLocal)
                    ),
                    new BoxDecoration(
                        color: Colors.white.withOpacity(0.9),
                        borderRadius: BorderRadius.CreateAll(Radius.circular(4))
                    )
                ),
            ThemeData
            {
                brightness: Brightness.light,
                textTheme: TextTheme textThemeAlternate,
                platform: TargetPlatform platformAlternate
            } __object18858 => ((TextStyle, BoxDecoration))
                (
                    textThemeAlternate.bodyMedium!.copyWith(
                        color: Colors.white,
                        fontSize: _getDefaultFontSize(platformAlternate)
                    ),
                    new BoxDecoration(
                        color: Colors.grey[700L]!.withOpacity(0.9),
                        borderRadius: BorderRadius.CreateAll(Radius.circular(4))
                    )
                ),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        var defaultConstraints = new BoxConstraints(
            minHeight: (widget.height ?? _tooltipTheme.height) ?? (double)_getDefaultTooltipHeight()
        );
        Widget tooltipBox = new _TooltipBox__tooltip(
            constraints: (widget.constraints ?? _tooltipTheme.constraints) ?? defaultConstraints,
            textStyle: (widget.textStyle ?? _tooltipTheme.textStyle) ?? defaultTextStyle,
            textAlign: (widget.textAlign ?? _tooltipTheme.textAlign) ?? _defaultTextAlign,
            decoration: (widget.decoration ?? _tooltipTheme.decoration) ?? defaultDecoration,
            padding: (widget.padding ?? _tooltipTheme.padding) ?? _getDefaultPadding(),
            margin: (widget.margin ?? _tooltipTheme.margin) ?? _defaultMargin,
            richMessage: widget.richMessage ?? new TextSpan(text: widget.message)
        );
        Widget effectiveChild = new MouseRegion(
            cursor: widget.mouseCursor ?? MouseCursor.defer,
            child: widget.child ?? SizedBox.CreateShrink()
        );
        bool excludeFromSemanticsLocal =
            (widget.excludeFromSemantics ?? _tooltipTheme.excludeFromSemantics)
            ?? _defaultExcludeFromSemantics;
        if (_visible)
        {
            effectiveChild = DartRuntimePrimitives.ConvertValue<Widget>(
                new RawTooltip(
                    key: _tooltipKey,
                    semanticsTooltip: excludeFromSemanticsLocal ? null : _tooltipMessage,
                    tooltipBuilder: (context, animation) =>
                        new FadeTransition(opacity: animation, child: tooltipBox),
                    touchDelay: (widget.showDuration ?? _tooltipTheme.showDuration)
                        ?? _defaultShowDuration,
                    triggerMode: (widget.triggerMode ?? _tooltipTheme.triggerMode)
                        ?? _defaultTriggerMode,
                    enableFeedback: (widget.enableFeedback ?? _tooltipTheme.enableFeedback)
                        ?? _defaultEnableFeedback,
                    hoverDelay: (widget.waitDuration ?? _tooltipTheme.waitDuration)
                        ?? _defaultWaitDuration,
                    enableTapToDismiss: widget.enableTapToDismiss,
                    onTriggered: widget.onTriggered,
                    dismissDelay: (widget.exitDuration ?? _tooltipTheme.exitDuration)
                        ?? _defaultExitDuration,
                    positionDelegate: _getDefaultPositionDelegate,
                    ignorePointer: widget.ignorePointer ?? (widget.message is not null),
                    child: effectiveChild
                )
            );
        }
        return effectiveChild;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_ticker is null)
            {
                return true;
            }
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."
                        ),
                        new ErrorDescription(
                            "A SingleTickerProviderStateMixin can only be used as a TickerProvider once."
                        ),
                        new ErrorHint(
                            "If a State is used for multiple AnimationController objects, or if it is passed to other "
                                + "objects and those objects might use it more than one time in total, then instead of "
                                + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin."
                        ),
                    }
                )
            );
        });
        _ticker = new Scheduler.Ticker(
            onTick,
            debugLabel: Foundation.ConstantsLibrary.kDebugMode
                ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                : null
        );
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if ((_ticker is null) || !_ticker!.isActive)
            {
                return true;
            }
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary($"{this} was disposed with an active Ticker."),
                        new ErrorDescription(
                            $"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time "
                                + "dispose() was called on the mixin, that Ticker was still active. The Ticker must "
                                + "be disposed before calling super.dispose()."
                        ),
                        new ErrorHint(
                            "Tickers used by AnimationControllers "
                                + "should be disposed by calling dispose() on the AnimationController itself. "
                                + "Otherwise, the ticker will leak."
                        ),
                        _ticker!.describeForError("The offending ticker was"),
                    }
                )
            );
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
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch
        {
            (true, true) => "active but muted",
            (true, _) => "active",
            (false, true) => "inactive and muted",
            (false, _) => "inactive",
            (null, _) => DartRuntimePrimitives.ConvertValue<string>(null),
        };
        properties.add(
            new DiagnosticsProperty<Scheduler.Ticker>(
                "ticker",
                _ticker,
                description: tickerDescription,
                showSeparator: false,
                defaultValue: default
            )
        );
    }
}

internal class _TooltipBox__tooltip : StatelessWidget
{
    public virtual BoxConstraints constraints { get; private set; } = default!;
    public virtual TextStyle textStyle { get; private set; } = default!;
    public virtual TextAlign textAlign { get; private set; } = default!;
    public virtual Decoration? decoration { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual EdgeInsetsGeometry? margin { get; private set; }
    public virtual InlineSpan richMessage { get; private set; } = default!;

    internal _TooltipBox__tooltip(
        BoxConstraints constraints,
        TextStyle textStyle,
        TextAlign textAlign,
        Decoration? decoration,
        EdgeInsetsGeometry? padding,
        EdgeInsetsGeometry? margin,
        InlineSpan richMessage
    )
    {
        this.constraints = constraints;
        this.textStyle = textStyle;
        this.textAlign = textAlign;
        this.decoration = decoration;
        this.padding = padding;
        this.margin = margin;
        this.richMessage = richMessage;
    }

    public override Widget build(BuildContext context)
    {
        return new ConstrainedBox(
            constraints: constraints,
            child: new DefaultTextStyle(
                style: textStyle,
                textAlign: textAlign,
                child: new Container(
                    decoration: decoration,
                    padding: padding,
                    margin: margin,
                    child: new Center(
                        widthFactor: 1.0,
                        heightFactor: 1.0,
                        child: Text.CreateRich(richMessage, style: textStyle, textAlign: textAlign)
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
