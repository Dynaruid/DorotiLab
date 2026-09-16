// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/progress_indicator.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Progress_indicatorLibrary
{
    internal static long _kIndeterminateLinearDuration = 1800L;
}

public static partial class Progress_indicatorLibrary
{
    internal static long _kIndeterminateCircularDuration = 1333L * 2222L;
}

public static partial class Progress_indicatorLibrary
{
    internal static double _kTrackGapRampDownThreshold = 0.01;
}

internal enum _ActivityIndicatorType__progress_indicator
{
    material,
    adaptive
}

public static partial class Progress_indicatorLibrary
{
    internal static string _kValueControllerAssertion = "A progress indicator cannot have both a value and a controller.\n" + "The \"value\" property is for a determinate indicator with a specific progress, " + "while the \"controller\" is for controlling the animation of an indeterminate indicator.\n" + "To resolve this, provide only one of the two properties.";
}

public abstract class ProgressIndicator : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual double? value { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual global::Doroti.Framework.Animation.Animation<Color?>? valueColor { get; private set; }
    public virtual string? semanticsLabel { get; private set; }
    public virtual string? semanticsValue { get; private set; }

    protected ProgressIndicator(global::Doroti.Framework.Foundation.Key? key = null, double? value = null, Color? backgroundColor = null, Color? color = null, global::Doroti.Framework.Animation.Animation<Color?>? valueColor = null, string? semanticsLabel = null, string? semanticsValue = null) : base(key: key)
    {
        this.value = value;
        this.backgroundColor = backgroundColor;
        this.color = color;
        this.valueColor = valueColor;
        this.semanticsLabel = semanticsLabel;
        this.semanticsValue = semanticsValue;
    }

    internal virtual double? _effectiveValue => (value is null) ? null : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), 0.0, 1.0);
    internal virtual global::Doroti.Ui.Color _getValueColor(global::Doroti.Framework.Widgets.BuildContext context, Color? defaultColor = null)
    {
        return (((valueColor?.value ?? color) ?? ProgressIndicatorTheme.of(context).color) ?? defaultColor) ?? Theme.of(context).colorScheme.primary;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.PercentProperty("value", value, showName: false, ifNull: "<indeterminate>"));
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildSemanticsWrapper(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        var isProgressBar = false;
        string? expandedSemanticsValue = semanticsValue;
        if (value is not null)
        {
            double value__value5992 = DartRuntimePrimitives.RequireValue(value);
            expandedSemanticsValue ??= $"{(DartRuntimePrimitives.RequireValue(_effectiveValue) * 100L).round()}";
            isProgressBar = true;
        }
        return new global::Doroti.Framework.Widgets.Semantics(label: semanticsLabel, role: isProgressBar ? SemanticsRole.progressBar : SemanticsRole.loadingSpinner, minValue: isProgressBar ? "0" : null, maxValue: isProgressBar ? "100" : null, value: expandedSemanticsValue, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _LinearProgressIndicatorPainter__progress_indicator : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual Color trackColor { get; private set; } = default!;
    public virtual Color valueColor { get; private set; } = default!;
    public virtual double? value { get; private set; }
    public virtual double animationValue { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderRadiusGeometry? indicatorBorderRadius { get; private set; }
    public virtual Color? stopIndicatorColor { get; private set; }
    public virtual double? stopIndicatorRadius { get; private set; }
    public virtual double? trackGap { get; private set; }
    public static global::Doroti.Framework.Animation.Curve line1Head = new global::Doroti.Framework.Animation.Interval(0.0, 750.0 / Progress_indicatorLibrary._kIndeterminateLinearDuration, curve: new global::Doroti.Framework.Animation.Cubic(0.2, 0.0, 0.8, 1.0));
    public static global::Doroti.Framework.Animation.Curve line1Tail = new global::Doroti.Framework.Animation.Interval(333.0 / Progress_indicatorLibrary._kIndeterminateLinearDuration, (333.0 + 750.0) / Progress_indicatorLibrary._kIndeterminateLinearDuration, curve: new global::Doroti.Framework.Animation.Cubic(0.4, 0.0, 1.0, 1.0));
    public static global::Doroti.Framework.Animation.Curve line2Head = new global::Doroti.Framework.Animation.Interval(1000.0 / Progress_indicatorLibrary._kIndeterminateLinearDuration, (1000.0 + 567.0) / Progress_indicatorLibrary._kIndeterminateLinearDuration, curve: new global::Doroti.Framework.Animation.Cubic(0.0, 0.0, 0.65, 1.0));
    public static global::Doroti.Framework.Animation.Curve line2Tail = new global::Doroti.Framework.Animation.Interval(1267.0 / Progress_indicatorLibrary._kIndeterminateLinearDuration, (1267.0 + 533.0) / Progress_indicatorLibrary._kIndeterminateLinearDuration, curve: new global::Doroti.Framework.Animation.Cubic(0.1, 0.0, 0.45, 1.0));

    internal _LinearProgressIndicatorPainter__progress_indicator(Color trackColor, Color valueColor, double? value = null, double animationValue = default!, TextDirection textDirection = default!, global::Doroti.Framework.Painting.BorderRadiusGeometry? indicatorBorderRadius = default!, Color? stopIndicatorColor = default!, double? stopIndicatorRadius = default!, double? trackGap = default!)
    {
        this.trackColor = trackColor;
        this.valueColor = valueColor;
        this.value = value;
        this.animationValue = animationValue;
        this.textDirection = textDirection;
        this.indicatorBorderRadius = indicatorBorderRadius;
        this.stopIndicatorColor = stopIndicatorColor;
        this.stopIndicatorRadius = stopIndicatorRadius;
        this.trackGap = trackGap;
    }

    public override void paint(Canvas canvas, Size size)
    {
        double effectiveTrackGap = trackGap ?? 0.0;
        void drawLinearIndicator(double startFraction, double endFraction, Color color)
        {
            if ((endFraction - startFraction) <= 0L)
            {
                return;
            }
            var isLtr = Equals(textDirection, TextDirection.ltr);
            double left = (isLtr ? startFraction : (1L - endFraction)) * size.width;
            double right = (isLtr ? endFraction : (1L - startFraction)) * size.width;
            var rect = Rect.fromLTRB(left, 0, right, size.height);
            var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = color;
    return __cascade;
}))();
            if (indicatorBorderRadius is not null)
            {
                global::Doroti.Ui.RRect rrect = indicatorBorderRadius!.resolve(textDirection).toRRect(rect);
                canvas.drawRRect(rrect, paintLocal);
            }
            else
            {
                canvas.drawRect(rect, paintLocal);
            }
        }
        void drawStopIndicator()
        {
            double maxRadius = size.height / 2L;
            double radius = Math.Min(DartRuntimePrimitives.RequireValue(stopIndicatorRadius), maxRadius);
            var indicatorPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = stopIndicatorColor!;
    return __cascade;
}))();
            global::Doroti.Ui.Offset position = textDirection switch { TextDirection.rtl => new global::Doroti.Ui.Offset(maxRadius, maxRadius), TextDirection.ltr => new global::Doroti.Ui.Offset(size.width - maxRadius, maxRadius), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            canvas.drawCircle(position, radius, indicatorPaint);
        }
        double getEffectiveTrackGapFraction(double currentValue, double trackGapFraction)
        {
            return trackGapFraction * Dart_uiLibrary.clampDouble(currentValue, 0, Progress_indicatorLibrary._kTrackGapRampDownThreshold) / Progress_indicatorLibrary._kTrackGapRampDownThreshold;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double trackGapFractionLocal = effectiveTrackGap / size.width;
        double? effectiveValue = (value is null) ? null : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), 0.0, 1.0);
        if (effectiveValue is not null)
        {
            double effectiveValue__10098__value10217 = DartRuntimePrimitives.RequireValue(effectiveValue);
            double trackStartFraction = (trackGapFractionLocal > 0L) ? (DartRuntimePrimitives.RequireValue(effectiveValue__10098__value10217) + getEffectiveTrackGapFraction(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(effectiveValue__10098__value10217)), trackGapFractionLocal)) : 0;
            if (trackStartFraction < 1L)
            {
                drawLinearIndicator(startFraction: trackStartFraction, endFraction: 1, color: trackColor);
            }
            if ((stopIndicatorRadius is not null) && (DartRuntimePrimitives.RequireValue(stopIndicatorRadius) > 0L))
            {
                double stopIndicatorRadius__value10651 = DartRuntimePrimitives.RequireValue(stopIndicatorRadius);
                drawStopIndicator();
            }
            if (DartRuntimePrimitives.RequireValue(effectiveValue__10098__value10217) > 0L)
            {
                drawLinearIndicator(startFraction: 0, endFraction: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(effectiveValue__10098__value10217)), color: valueColor);
            }
            return;
        }
        double firstLineHead = line1Head.transform(animationValue);
        double firstLineTail = line1Tail.transform(animationValue);
        double secondLineHead = line2Head.transform(animationValue);
        double secondLineTail = line2Tail.transform(animationValue);
        if (firstLineHead < (1L - trackGapFractionLocal))
        {
            double trackStartFractionLocal = (firstLineHead > 0L) ? (firstLineHead + getEffectiveTrackGapFraction(firstLineHead, trackGapFractionLocal)) : 0;
            drawLinearIndicator(startFraction: trackStartFractionLocal, endFraction: 1, color: trackColor);
        }
        if ((firstLineHead - firstLineTail) > 0L)
        {
            drawLinearIndicator(startFraction: firstLineTail, endFraction: firstLineHead, color: valueColor);
        }
        if (firstLineTail > trackGapFractionLocal)
        {
            double trackStartFractionAlternate = (secondLineHead > 0L) ? (secondLineHead + getEffectiveTrackGapFraction(secondLineHead, trackGapFractionLocal)) : 0;
            double trackEndFraction = (firstLineTail < 1L) ? (firstLineTail - getEffectiveTrackGapFraction(1L - firstLineTail, trackGapFractionLocal)) : 1;
            drawLinearIndicator(startFraction: trackStartFractionAlternate, endFraction: trackEndFraction, color: trackColor);
        }
        if ((secondLineHead - secondLineTail) > 0L)
        {
            drawLinearIndicator(startFraction: secondLineTail, endFraction: secondLineHead, color: valueColor);
        }
        if (secondLineTail > trackGapFractionLocal)
        {
            double trackEndFractionLocal = (secondLineTail < 1L) ? (secondLineTail - getEffectiveTrackGapFraction(1L - secondLineTail, trackGapFractionLocal)) : 1;
            drawLinearIndicator(startFraction: 0, endFraction: trackEndFractionLocal, color: trackColor);
        }
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_LinearProgressIndicatorPainter__progress_indicator)oldDelegate;
        return (!Equals(__oldPainter.trackColor, trackColor)) || (!Equals(__oldPainter.valueColor, valueColor)) || (__oldPainter.value != value) || (__oldPainter.animationValue != animationValue) || (!Equals(__oldPainter.textDirection, textDirection)) || (!Equals(__oldPainter.indicatorBorderRadius, indicatorBorderRadius)) || (!Equals(__oldPainter.stopIndicatorColor, stopIndicatorColor)) || (__oldPainter.stopIndicatorRadius != stopIndicatorRadius) || (__oldPainter.trackGap != trackGap);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class LinearProgressIndicator : ProgressIndicator
{
    public virtual double? minHeight { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderRadiusGeometry? borderRadius { get; private set; }
    public virtual Color? stopIndicatorColor { get; private set; }
    public virtual double? stopIndicatorRadius { get; private set; }
    public virtual double? trackGap { get; private set; }
    public virtual bool? year2023 { get; private set; }
    public virtual global::Doroti.Framework.Animation.AnimationController? controller { get; private set; }
    public static Duration defaultAnimationDuration = Duration.Create(milliseconds: Progress_indicatorLibrary._kIndeterminateLinearDuration);

    public LinearProgressIndicator(global::Doroti.Framework.Foundation.Key? key = null, double? value = null, Color? backgroundColor = null, Color? color = null, global::Doroti.Framework.Animation.Animation<Color?>? valueColor = null, double? minHeight = null, string? semanticsLabel = null, string? semanticsValue = null, global::Doroti.Framework.Painting.BorderRadiusGeometry? borderRadius = null, Color? stopIndicatorColor = null, double? stopIndicatorRadius = null, double? trackGap = null, bool? year2023 = null, global::Doroti.Framework.Animation.AnimationController? controller = null) : base(key: key, value: value, backgroundColor: backgroundColor, color: color, valueColor: valueColor, semanticsLabel: semanticsLabel, semanticsValue: semanticsValue)
    {
        this.minHeight = minHeight;
        this.borderRadius = borderRadius;
        this.stopIndicatorColor = stopIndicatorColor;
        this.stopIndicatorRadius = stopIndicatorRadius;
        this.trackGap = trackGap;
        this.year2023 = year2023;
        this.controller = controller;
        System.Diagnostics.Debug.Assert((minHeight is null) || (DartRuntimePrimitives.RequireValue(minHeight) > 0L));
        System.Diagnostics.Debug.Assert((value is null) || (controller is null));
    }

    public override Color? backgroundColor => base.backgroundColor;
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _LinearProgressIndicatorState__progress_indicator());
}

internal class _LinearProgressIndicatorState__progress_indicator : global::Doroti.Framework.Widgets.State<LinearProgressIndicator>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<LinearProgressIndicator>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _internalController { get; private set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _internalController = new global::Doroti.Framework.Animation.AnimationController(duration: LinearProgressIndicator.defaultAnimationDuration, vsync: this);
        _updateControllerAnimatingStatus();
    }

    public override void didUpdateWidget(LinearProgressIndicator oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _updateControllerAnimatingStatus();
    }

    public override void dispose()
    {
        _internalController.dispose();
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

    internal virtual global::Doroti.Framework.Animation.AnimationController _controller => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.AnimationController>(((widget.controller ?? context.getInheritedWidgetOfExactType<ProgressIndicatorTheme>()?.data.controller) ?? context.findAncestorWidgetOfExactType<Theme>()?.data.progressIndicatorTheme.controller) ?? _internalController);
    internal virtual void _updateControllerAnimatingStatus()
    {
        if ((widget._effectiveValue is null) && !_internalController.isAnimating)
        {
            _internalController.repeat();
        }
        else
        {
            if ((widget._effectiveValue is not null) && _internalController.isAnimating)
            {
                _internalController.stop();
            }
        }
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildIndicator(global::Doroti.Framework.Widgets.BuildContext context, double animationValue, TextDirection textDirection)
    {
        ProgressIndicatorThemeData indicatorTheme = ProgressIndicatorTheme.of(context);
        bool year2023Local = (widget.year2023 ?? indicatorTheme.year2023) ?? true;
        ProgressIndicatorThemeData defaults = year2023Local ? new _LinearProgressIndicatorDefaultsM3Year2023__progress_indicator(context) : new _LinearProgressIndicatorDefaultsM3__progress_indicator(context);
        global::Doroti.Ui.Color trackColorLocal = (widget.backgroundColor ?? indicatorTheme.linearTrackColor) ?? defaults.linearTrackColor!;
        double minHeightLocal = (widget.minHeight ?? indicatorTheme.linearMinHeight) ?? DartRuntimePrimitives.RequireValue(defaults.linearMinHeight);
        global::Doroti.Framework.Painting.BorderRadiusGeometry? borderRadiusLocal = (widget.borderRadius ?? indicatorTheme.borderRadius) ?? defaults.borderRadius;
        global::Doroti.Ui.Color? stopIndicatorColorLocal = !year2023Local ? ((widget.stopIndicatorColor ?? indicatorTheme.stopIndicatorColor) ?? defaults.stopIndicatorColor) : null;
        double? stopIndicatorRadiusLocal = !year2023Local ? ((widget.stopIndicatorRadius ?? indicatorTheme.stopIndicatorRadius) ?? defaults.stopIndicatorRadius) : null;
        double? trackGapLocal = !year2023Local ? ((widget.trackGap ?? indicatorTheme.trackGap) ?? defaults.trackGap) : null;
        global::Doroti.Framework.Widgets.Widget result = new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: double.PositiveInfinity, minHeight: minHeightLocal), child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _LinearProgressIndicatorPainter__progress_indicator(trackColor: trackColorLocal, valueColor: widget._getValueColor(context, defaultColor: defaults.color), value: widget._effectiveValue, animationValue: animationValue, textDirection: textDirection, indicatorBorderRadius: borderRadiusLocal, stopIndicatorColor: stopIndicatorColorLocal, stopIndicatorRadius: stopIndicatorRadiusLocal, trackGap: trackGapLocal)));
        if ((borderRadiusLocal is not null) && (widget._effectiveValue is null))
        {
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ClipRRect(borderRadius: borderRadiusLocal, child: result));
        }
        return widget._buildSemanticsWrapper(context: context, child: result);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.TextDirection textDirection = Directionality.of(context);
        if (widget._effectiveValue is not null)
        {
            return _buildIndicator(context, _controller.value, textDirection);
        }
        return new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: _controller.view, builder: (context, child) =>
        {
            return _buildIndicator(context, _controller.value, textDirection);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
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
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted",(true, _) => "active",(false, true) => "inactive and muted",(false, _) => "inactive",(null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

internal class _CircularProgressIndicatorPainter__progress_indicator : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual Color? trackColor { get; private set; }
    public virtual Color valueColor { get; private set; } = default!;
    public virtual double? value { get; private set; }
    public virtual double headValue { get; private set; } = default!;
    public virtual double tailValue { get; private set; } = default!;
    public virtual double offsetValue { get; private set; } = default!;
    public virtual double rotationValue { get; private set; } = default!;
    public virtual double strokeWidth { get; private set; } = default!;
    public virtual double strokeAlign { get; private set; } = default!;
    public virtual double arcStart { get; private set; } = default!;
    public virtual double arcSweep { get; private set; } = default!;
    public virtual StrokeCap? strokeCap { get; private set; }
    public virtual double? trackGap { get; private set; }
    public virtual bool year2023 { get; private set; } = default!;
    internal static double _twoPi = Dart_mathLibrary.pi * 2.0;
    internal const double _epsilon = 0.001;
    internal static double _sweep = _twoPi - _epsilon;
    internal static double _startAngle = -Dart_mathLibrary.pi / 2.0;

    internal _CircularProgressIndicatorPainter__progress_indicator(Color? trackColor = null, Color valueColor = default!, double? value = default!, double headValue = default!, double tailValue = default!, double offsetValue = default!, double rotationValue = default!, double strokeWidth = default!, double strokeAlign = default!, StrokeCap? strokeCap = null, double? trackGap = null, bool year2023 = true)
    {
        this.trackColor = trackColor;
        this.valueColor = valueColor;
        this.value = value;
        this.headValue = headValue;
        this.tailValue = tailValue;
        this.offsetValue = offsetValue;
        this.rotationValue = rotationValue;
        this.strokeWidth = strokeWidth;
        this.strokeAlign = strokeAlign;
        this.strokeCap = strokeCap;
        this.trackGap = trackGap;
        this.year2023 = year2023;
        arcStart = (value is not null) ? _startAngle : (_startAngle + (tailValue * 3L / 2L * Dart_mathLibrary.pi) + (rotationValue * Dart_mathLibrary.pi * 2.0) + (offsetValue * 0.5 * Dart_mathLibrary.pi));
        arcSweep = (value is not null) ? (Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), 0.0, 1.0) * _sweep) : Math.Max((headValue * 3L / 2L * Dart_mathLibrary.pi) - (tailValue * 3L / 2L * Dart_mathLibrary.pi), _epsilon);
    }

    public override void paint(Canvas canvas, Size size)
    {
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = valueColor;
    __cascade.strokeWidth = strokeWidth;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
        double strokeOffset = strokeWidth / 2L * -strokeAlign;
        var arcBaseOffset = new global::Doroti.Ui.Offset(strokeOffset, strokeOffset);
        var arcActualSize = new global::Doroti.Ui.Size(size.width - (strokeOffset * 2L), size.height - (strokeOffset * 2L));
        bool hasGap = (trackGap is not null) && (DartRuntimePrimitives.RequireValue(trackGap) > 0L);
        if (trackColor is not null)
        {
            var backgroundPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = trackColor!;
    __cascade.strokeWidth = strokeWidth;
    __cascade.strokeCap = strokeCap ?? StrokeCap.round;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
            if (hasGap && (value is not null) && (DartRuntimePrimitives.RequireValue(value) > _epsilon))
            {
                double value__value28734 = DartRuntimePrimitives.RequireValue(value);
                double arcRadius = arcActualSize.shortestSide / 2L;
                double strokeRadius = strokeWidth / arcRadius;
                double gapRadius = DartRuntimePrimitives.RequireValue(trackGap) / arcRadius;
                double startGap = strokeRadius + gapRadius;
                double endGap = (DartRuntimePrimitives.RequireValue(value) < _epsilon) ? startGap : (startGap * 2L);
                double startSweep = -Dart_mathLibrary.pi / 2.0 + startGap;
                double endSweep = Math.Max(0.0, _twoPi - (Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), 0.0, 1.0) * _twoPi) - endGap);
                canvas.save();
                canvas.scale(-1, 1);
                canvas.translate(-size.width, 0);
                canvas.drawArc(arcBaseOffset & arcActualSize, startSweep, endSweep, false, backgroundPaint);
                canvas.restore();
            }
            else
            {
                canvas.drawArc(arcBaseOffset & arcActualSize, 0, _sweep, false, backgroundPaint);
            }
        }
        if (year2023)
        {
            if ((value is null) && (strokeCap is null))
            {
                paintLocal.strokeCap = StrokeCap.square;
            }
            else
            {
                paintLocal.strokeCap = strokeCap ?? StrokeCap.butt;
            }
        }
        else
        {
            paintLocal.strokeCap = strokeCap ?? StrokeCap.round;
        }
        canvas.drawArc(arcBaseOffset & arcActualSize, arcStart, arcSweep, false, paintLocal);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_CircularProgressIndicatorPainter__progress_indicator)oldDelegate;
        return (!Equals(__oldPainter.trackColor, trackColor)) || (!Equals(__oldPainter.valueColor, valueColor)) || (__oldPainter.value != value) || (__oldPainter.headValue != headValue) || (__oldPainter.tailValue != tailValue) || (__oldPainter.offsetValue != offsetValue) || (__oldPainter.rotationValue != rotationValue) || (__oldPainter.strokeWidth != strokeWidth) || (__oldPainter.strokeAlign != strokeAlign) || (!Equals(__oldPainter.strokeCap, strokeCap)) || (__oldPainter.trackGap != trackGap) || (__oldPainter.year2023 != year2023);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CircularProgressIndicator : ProgressIndicator
{
    internal virtual _ActivityIndicatorType__progress_indicator _indicatorType { get; private set; } = default!;
    public virtual double? strokeWidth { get; private set; }
    public virtual double? strokeAlign { get; private set; }
    public virtual StrokeCap? strokeCap { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual double? trackGap { get; private set; }
    public virtual bool? year2023 { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Animation.AnimationController? controller { get; private set; }
    public static double strokeAlignInside = -1.0;
    public const double strokeAlignCenter = 0.0;
    public const double strokeAlignOutside = 1.0;
    public static Duration defaultAnimationDuration = Duration.Create(milliseconds: Progress_indicatorLibrary._kIndeterminateCircularDuration);

    public CircularProgressIndicator(global::Doroti.Framework.Foundation.Key? key = null, double? value = null, Color? backgroundColor = null, Color? color = null, global::Doroti.Framework.Animation.Animation<Color?>? valueColor = null, double? strokeWidth = null, double? strokeAlign = null, string? semanticsLabel = null, string? semanticsValue = null, StrokeCap? strokeCap = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, double? trackGap = null, bool? year2023 = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Animation.AnimationController? controller = null) : base(key: key, value: value, backgroundColor: backgroundColor, color: color, valueColor: valueColor, semanticsLabel: semanticsLabel, semanticsValue: semanticsValue)
    {
        this.strokeWidth = strokeWidth;
        this.strokeAlign = strokeAlign;
        this.strokeCap = strokeCap;
        this.constraints = constraints;
        this.trackGap = trackGap;
        this.year2023 = year2023;
        this.padding = padding;
        this.controller = controller;
        _indicatorType = _ActivityIndicatorType__progress_indicator.material;
        System.Diagnostics.Debug.Assert((value is null) || (controller is null));
    }

    public static CircularProgressIndicator CreateAdaptive(global::Doroti.Framework.Foundation.Key? key = null, double? value = null, Color? backgroundColor = null, global::Doroti.Framework.Animation.Animation<Color?>? valueColor = null, double? strokeWidth = null, string? semanticsLabel = null, string? semanticsValue = null, StrokeCap? strokeCap = null, double? strokeAlign = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, double? trackGap = null, bool? year2023 = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Animation.AnimationController? controller = null)
    {
        var __instance = new CircularProgressIndicator(key: key, value: value, backgroundColor: backgroundColor, valueColor: valueColor, strokeWidth: strokeWidth, strokeAlign: strokeAlign, semanticsLabel: semanticsLabel, semanticsValue: semanticsValue, strokeCap: strokeCap, constraints: constraints, trackGap: trackGap, year2023: year2023, padding: padding, controller: controller);
        __instance.strokeWidth = strokeWidth;
        __instance.strokeCap = strokeCap;
        __instance.strokeAlign = strokeAlign;
        __instance.constraints = constraints;
        __instance.trackGap = trackGap;
        __instance.year2023 = year2023;
        __instance.padding = padding;
        __instance.controller = controller;
        __instance._indicatorType = _ActivityIndicatorType__progress_indicator.adaptive;
        return __instance;
    }

    public override Color? backgroundColor => base.backgroundColor;
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CircularProgressIndicatorState__progress_indicator());
}

internal class _CircularProgressIndicatorState__progress_indicator : global::Doroti.Framework.Widgets.State<CircularProgressIndicator>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<CircularProgressIndicator>
{
    internal static long _pathCount = checked(Progress_indicatorLibrary._kIndeterminateCircularDuration / 1333L);
    internal static long _rotationCount = checked(Progress_indicatorLibrary._kIndeterminateCircularDuration / 2222L);
    internal static global::Doroti.Framework.Animation.Animatable<double> _strokeHeadTween = new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.0, 0.5, curve: Curves.fastOutSlowIn)).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.SawTooth(_pathCount)));
    internal static global::Doroti.Framework.Animation.Animatable<double> _strokeTailTween = new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.5, 1.0, curve: Curves.fastOutSlowIn)).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.SawTooth(_pathCount)));
    internal static global::Doroti.Framework.Animation.Animatable<double> _offsetTween = new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.SawTooth(_pathCount));
    internal static global::Doroti.Framework.Animation.Animatable<double> _rotationTween = new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.SawTooth(_rotationCount));
    internal virtual global::Doroti.Framework.Animation.AnimationController _internalController { get; private set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _internalController = new global::Doroti.Framework.Animation.AnimationController(duration: CircularProgressIndicator.defaultAnimationDuration, vsync: this);
        _updateControllerAnimatingStatus();
    }

    public override void didUpdateWidget(CircularProgressIndicator oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _updateControllerAnimatingStatus();
    }

    public override void dispose()
    {
        _internalController.dispose();
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

    internal virtual global::Doroti.Framework.Animation.AnimationController _controller => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.AnimationController>(((widget.controller ?? context.getInheritedWidgetOfExactType<ProgressIndicatorTheme>()?.data.controller) ?? context.findAncestorWidgetOfExactType<Theme>()?.data.progressIndicatorTheme.controller) ?? _internalController);
    internal virtual void _updateControllerAnimatingStatus()
    {
        if ((widget._effectiveValue is null) && !_internalController.isAnimating)
        {
            _internalController.repeat();
        }
        else
        {
            if ((widget._effectiveValue is not null) && _internalController.isAnimating)
            {
                _internalController.stop();
            }
        }
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildCupertinoIndicator(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color? tickColor = widget.backgroundColor;
        double? value = widget._effectiveValue;
        if (value is null)
        {
            return new CupertinoActivityIndicator(key: widget.key, color: tickColor);
        }
        return CupertinoActivityIndicator.CreatePartiallyRevealed(key: widget.key, color: tickColor, progress: DartRuntimePrimitives.RequireValue(value));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildMaterialIndicator(global::Doroti.Framework.Widgets.BuildContext context, double headValue, double tailValue, double offsetValue, double rotationValue)
    {
        ProgressIndicatorThemeData indicatorTheme = ProgressIndicatorTheme.of(context);
        bool year2023Local = (widget.year2023 ?? indicatorTheme.year2023) ?? true;
        ProgressIndicatorThemeData defaults = year2023Local ? new _CircularProgressIndicatorDefaultsM3Year2023__progress_indicator(context, indeterminate: widget._effectiveValue is null) : new _CircularProgressIndicatorDefaultsM3__progress_indicator(context, indeterminate: widget._effectiveValue is null);
        global::Doroti.Ui.Color? trackColorLocal = (widget.backgroundColor ?? indicatorTheme.circularTrackColor) ?? defaults.circularTrackColor;
        double strokeWidthLocal = (widget.strokeWidth ?? indicatorTheme.strokeWidth) ?? DartRuntimePrimitives.RequireValue(defaults.strokeWidth);
        double strokeAlignLocal = (widget.strokeAlign ?? indicatorTheme.strokeAlign) ?? DartRuntimePrimitives.RequireValue(defaults.strokeAlign);
        global::Doroti.Ui.StrokeCap? strokeCapLocal = widget.strokeCap ?? indicatorTheme.strokeCap;
        global::Doroti.Framework.Rendering.BoxConstraints constraintsLocal = (widget.constraints ?? indicatorTheme.constraints) ?? defaults.constraints!;
        double? trackGapLocal = year2023Local ? null : ((widget.trackGap ?? indicatorTheme.trackGap) ?? defaults.trackGap);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? effectivePadding = (widget.padding ?? indicatorTheme.circularTrackPadding) ?? defaults.circularTrackPadding;
        global::Doroti.Framework.Widgets.Widget result = new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: constraintsLocal, child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _CircularProgressIndicatorPainter__progress_indicator(trackColor: trackColorLocal, valueColor: widget._getValueColor(context, defaultColor: defaults.color), value: widget._effectiveValue, headValue: headValue, tailValue: tailValue, offsetValue: offsetValue, rotationValue: rotationValue, strokeWidth: strokeWidthLocal, strokeAlign: strokeAlignLocal, strokeCap: strokeCapLocal, trackGap: trackGapLocal, year2023: year2023Local)));
        if (effectivePadding is not null)
        {
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: effectivePadding, child: result));
        }
        return widget._buildSemanticsWrapper(context: context, child: result);
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildAnimation()
    {
        return new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: _controller, builder: (context, child) =>
        {
            return _buildMaterialIndicator(context, _strokeHeadTween.evaluate(_controller), _strokeTailTween.evaluate(_controller), _offsetTween.evaluate(_controller), _rotationTween.evaluate(_controller));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.Builder(builder: (context) =>
        {
            switch (widget._indicatorType)
            {
                case _ActivityIndicatorType__progress_indicator.material:
                    {
                        if (widget._effectiveValue is not null)
                        {
                            return _buildMaterialIndicator(context, 0.0, 0.0, 0, 0.0);
                        }
                        return _buildAnimation();
                    }
                case _ActivityIndicatorType__progress_indicator.adaptive:
                    {
                        ThemeData theme = Theme.of(context);
                        switch (theme.platform)
                        {
                            case TargetPlatform.iOS:
                            case TargetPlatform.macOS:
                                {
                                    return _buildCupertinoIndicator(context);
                                }
                            case TargetPlatform.android:
                            case TargetPlatform.fuchsia:
                            case TargetPlatform.linux:
                            case TargetPlatform.windows:
                                {
                                    if (widget._effectiveValue is not null)
                                    {
                                        return _buildMaterialIndicator(context, 0.0, 0.0, 0, 0.0);
                                    }
                                    return _buildAnimation();
                                }
                            default:
                                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                        }
                    }
            }
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
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
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted",(true, _) => "active",(false, true) => "inactive and muted",(false, _) => "inactive",(null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

internal class _RefreshProgressIndicatorPainter__progress_indicator : _CircularProgressIndicatorPainter__progress_indicator
{
    public virtual double arrowheadScale { get; private set; } = default!;

    internal _RefreshProgressIndicatorPainter__progress_indicator(Color valueColor, double? value, double headValue, double tailValue, double offsetValue, double rotationValue, double strokeWidth, double strokeAlign, double arrowheadScale, StrokeCap? strokeCap) : base(valueColor: valueColor, value: value, headValue: headValue, tailValue: tailValue, offsetValue: offsetValue, rotationValue: rotationValue, strokeWidth: strokeWidth, strokeAlign: strokeAlign, strokeCap: strokeCap)
    {
        this.arrowheadScale = arrowheadScale;
    }

    public virtual void paintArrowhead(Canvas canvas, Size size)
    {
        double arcEnd = arcStart + arcSweep;
        double ux = Dart_mathLibrary.cos(arcEnd);
        double uy = Dart_mathLibrary.sin(arcEnd);
        DartRuntimePrimitives.Assert(() => size.width == size.height);
        double radius = size.width / 2.0;
        double arrowheadPointX = radius + (ux * radius) + (-uy * strokeWidth * 2.0 * arrowheadScale);
        double arrowheadPointY = radius + (uy * radius) + (ux * strokeWidth * 2.0 * arrowheadScale);
        double arrowheadRadius = strokeWidth * 2.0 * arrowheadScale;
        double innerRadius = radius - arrowheadRadius;
        double outerRadius = radius + arrowheadRadius;
        var path = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(radius + (ux * innerRadius), radius + (uy * innerRadius));
    __cascade.lineTo(radius + (ux * outerRadius), radius + (uy * outerRadius));
    __cascade.lineTo(arrowheadPointX, arrowheadPointY);
    __cascade.close();
    return __cascade;
}))();
        var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = valueColor;
    __cascade.strokeWidth = strokeWidth;
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
        canvas.drawPath(path, paint);
    }

    public override void paint(Canvas canvas, Size size)
    {
        base.paint(canvas, size);
        if (arrowheadScale > 0.0)
        {
            paintArrowhead(canvas, size);
        }
    }

}

public class RefreshProgressIndicator : CircularProgressIndicator
{
    public virtual double elevation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry indicatorMargin { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry indicatorPadding { get; private set; } = default!;
    public const double defaultStrokeWidth = 2.5;

    public RefreshProgressIndicator(global::Doroti.Framework.Foundation.Key? key = null, double? value = null, Color? backgroundColor = null, Color? color = null, global::Doroti.Framework.Animation.Animation<Color?>? valueColor = null, double? strokeWidth = null, double? strokeAlign = null, string? semanticsLabel = null, string? semanticsValue = null, StrokeCap? strokeCap = null, double elevation = 2.0, global::Doroti.Framework.Painting.EdgeInsetsGeometry indicatorMargin = default!, global::Doroti.Framework.Painting.EdgeInsetsGeometry indicatorPadding = default!) : base(key: key, value: value, backgroundColor: backgroundColor, color: color, valueColor: valueColor, strokeWidth: strokeWidth ?? defaultStrokeWidth, strokeAlign: strokeAlign, semanticsLabel: semanticsLabel, semanticsValue: semanticsValue, strokeCap: strokeCap)
    {
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __indicatorMargin = indicatorMargin ?? EdgeInsets.CreateAll(4.0);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __indicatorPadding = indicatorPadding ?? EdgeInsets.CreateAll(12.0);
        this.elevation = elevation;
        this.indicatorMargin = __indicatorMargin;
        this.indicatorPadding = __indicatorPadding;
    }

    public override Color? backgroundColor => base.backgroundColor;
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RefreshProgressIndicatorState__progress_indicator());
}

internal class _RefreshProgressIndicatorState__progress_indicator : _CircularProgressIndicatorState__progress_indicator
{
    internal const double _indicatorSize = 41.0;
    internal const double _strokeHeadInterval = 0.33;
    private bool __late__convertTween_initialized;
    private global::Doroti.Framework.Animation.Animatable<double> __late__convertTween = default!;
    internal virtual global::Doroti.Framework.Animation.Animatable<double> _convertTween
    {
        get
        {
            if (!__late__convertTween_initialized)
            {
                __late__convertTween = new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.1, _strokeHeadInterval));
                __late__convertTween_initialized = true;
            }
            return __late__convertTween;
        }
    }
    private bool __late__additionalRotationTween_initialized;
    private global::Doroti.Framework.Animation.Animatable<double> __late__additionalRotationTween = default!;
    internal virtual global::Doroti.Framework.Animation.Animatable<double> _additionalRotationTween
    {
        get
        {
            if (!__late__additionalRotationTween_initialized)
            {
                __late__additionalRotationTween = new global::Doroti.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: -0.1, end: -0.2), weight: _strokeHeadInterval), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: -0.2, end: 1.35), weight: 1L - _strokeHeadInterval) });
                __late__additionalRotationTween_initialized = true;
            }
            return __late__additionalRotationTween;
        }
    }
    internal virtual double? _lastValue { get; set; } = default;

    public override RefreshProgressIndicator widget => ((RefreshProgressIndicator?)base.widget)!;
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        double? valueLocal = widget._effectiveValue;
        if (valueLocal is not null)
        {
            double value__54874__value54914 = DartRuntimePrimitives.RequireValue(valueLocal);
            _lastValue = DartRuntimePrimitives.RequireValue(value__54874__value54914);
            _controller.value = _convertTween.transform(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value__54874__value54914))) * (1333L / 2L / Progress_indicatorLibrary._kIndeterminateCircularDuration);
        }
        return _buildAnimation();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override global::Doroti.Framework.Widgets.Widget _buildAnimation()
    {
        return new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: _controller, builder: (context, child) =>
        {
            return _buildMaterialIndicator(context, 1.05 * _strokeHeadTween.transform(_controller.value), _strokeTailTween.transform(_controller.value), _offsetTween.transform(_controller.value), _rotationTween.transform(_controller.value));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override global::Doroti.Framework.Widgets.Widget _buildMaterialIndicator(global::Doroti.Framework.Widgets.BuildContext context, double headValue, double tailValue, double offsetValue, double rotationValue)
    {
        double? valueLocal = widget._effectiveValue;
        double arrowheadScaleLocal = (valueLocal is null) ? 0.0 : new global::Doroti.Framework.Animation.Interval(0.1, _strokeHeadInterval).transform(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(valueLocal)));
        double rotation = default!;
        if ((valueLocal is null) && (_lastValue is null))
        {
            rotation = 0.0;
        }
        else
        {
            rotation = Dart_mathLibrary.pi * _additionalRotationTween.transform(valueLocal ?? DartRuntimePrimitives.RequireValue(_lastValue));
        }
        global::Doroti.Ui.Color valueColorLocal = widget._getValueColor(context);
        double opacityLocal = valueColorLocal.opacity;
        valueColorLocal = valueColorLocal.withOpacity(1.0);
        ProgressIndicatorThemeData defaults = DartRuntimePrimitives.ConvertValue<ProgressIndicatorThemeData>(new _CircularProgressIndicatorDefaultsM3Year2023__progress_indicator(context, indeterminate: valueLocal is null));
        ProgressIndicatorThemeData indicatorTheme = ProgressIndicatorTheme.of(context);
        global::Doroti.Ui.Color backgroundColorLocal = (widget.backgroundColor ?? indicatorTheme.refreshBackgroundColor) ?? Theme.of(context).canvasColor;
        double strokeWidthLocal = (widget.strokeWidth ?? indicatorTheme.strokeWidth) ?? DartRuntimePrimitives.RequireValue(defaults.strokeWidth);
        double strokeAlignLocal = (widget.strokeAlign ?? indicatorTheme.strokeAlign) ?? DartRuntimePrimitives.RequireValue(defaults.strokeAlign);
        global::Doroti.Ui.StrokeCap? strokeCapLocal = widget.strokeCap ?? indicatorTheme.strokeCap;
        return widget._buildSemanticsWrapper(context: context, child: new global::Doroti.Framework.Widgets.Padding(padding: widget.indicatorMargin, child: SizedBox.CreateFromSize(size: new global::Doroti.Ui.Size(_indicatorSize), child: new Material(type: MaterialType.circle, color: backgroundColorLocal, elevation: widget.elevation, child: new global::Doroti.Framework.Widgets.Padding(padding: widget.indicatorPadding, child: new global::Doroti.Framework.Widgets.Opacity(opacity: opacityLocal, child: Transform.CreateRotate(angle: rotation, child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _RefreshProgressIndicatorPainter__progress_indicator(valueColor: valueColorLocal, value: null, headValue: headValue, tailValue: tailValue, offsetValue: offsetValue, rotationValue: rotationValue, strokeWidth: strokeWidthLocal, strokeAlign: strokeAlignLocal, arrowheadScale: arrowheadScaleLocal, strokeCap: strokeCapLocal)))))))));
    }

}

internal class _CircularProgressIndicatorDefaultsM3Year2023__progress_indicator : ProgressIndicatorThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    public virtual bool indeterminate { get; private set; } = default!;

    internal _CircularProgressIndicatorDefaultsM3Year2023__progress_indicator(global::Doroti.Framework.Widgets.BuildContext context, bool indeterminate)
    {
        this.context = context;
        this.indeterminate = indeterminate;
    }

    public override global::Doroti.Ui.Color color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary);
    public override double? strokeWidth => 4.0;
    public override double? strokeAlign => CircularProgressIndicator.strokeAlignCenter;
    public override global::Doroti.Framework.Rendering.BoxConstraints constraints => new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: 36.0, minHeight: 36.0);
}

internal class _LinearProgressIndicatorDefaultsM3Year2023__progress_indicator : ProgressIndicatorThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _LinearProgressIndicatorDefaultsM3Year2023__progress_indicator(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary);
    public override global::Doroti.Ui.Color linearTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.secondaryContainer);
    public override double? linearMinHeight => 4.0;
}

internal class _CircularProgressIndicatorDefaultsM3__progress_indicator : ProgressIndicatorThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    public virtual bool indeterminate { get; private set; } = default!;

    internal _CircularProgressIndicatorDefaultsM3__progress_indicator(global::Doroti.Framework.Widgets.BuildContext context, bool indeterminate)
    {
        this.context = context;
        this.indeterminate = indeterminate;
    }

    public override global::Doroti.Ui.Color color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary);
    public override global::Doroti.Ui.Color? circularTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(indeterminate ? null : _colors.secondaryContainer);
    public override double? strokeWidth => 4.0;
    public override double? strokeAlign => CircularProgressIndicator.strokeAlignInside;
    public override global::Doroti.Framework.Rendering.BoxConstraints constraints => new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: 40.0, minHeight: 40.0);
    public override double? trackGap => 4.0;
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? circularTrackPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsets.CreateAll(4.0));
}

internal class _LinearProgressIndicatorDefaultsM3__progress_indicator : ProgressIndicatorThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _LinearProgressIndicatorDefaultsM3__progress_indicator(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary);
    public override global::Doroti.Ui.Color linearTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.secondaryContainer);
    public override double? linearMinHeight => 4.0;
    public override global::Doroti.Framework.Painting.BorderRadius borderRadius => BorderRadius.CreateAll(Radius.circular(4.0 / 2L));
    public override global::Doroti.Ui.Color stopIndicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary);
    public override double? stopIndicatorRadius => DartRuntimePrimitives.ConvertValue<double>(4.0 / 2L);
    public override double? trackGap => 4.0;
}
