// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/progress_indicator.dart
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

public static partial class Progress_indicatorLibrary
{
    internal static long _kIndeterminateLinearDuration = 1800L;
}

public static partial class Progress_indicatorLibrary
{
    internal static long _kIndeterminateCircularDuration = (1333L * 2222L);
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

    internal virtual double? _effectiveValue => ((this.value is null) ? null : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(this.value), 0.0, 1.0));
    internal virtual global::Doroti.Ui.Color _getValueColor(global::Doroti.Framework.Widgets.BuildContext context, Color? defaultColor = null)
    {
        return ((global::Doroti.Ui.Color)(object?)((((this.valueColor?.value ?? this.color) ?? ProgressIndicatorTheme.of(context).color) ?? defaultColor) ?? Theme.of(context).colorScheme.primary));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.PercentProperty("value", this.value, showName: false, ifNull: "<indeterminate>"));
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildSemanticsWrapper(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        var isProgressBar__5908 = false;
        string? expandedSemanticsValue__5943 = this.semanticsValue;
        if ((this.value is not null))
        {
            double value__value5992 = DartRuntimePrimitives.RequireValue(value);
            expandedSemanticsValue__5943 ??= $"{((DartRuntimePrimitives.RequireValue(this._effectiveValue) * 100L)).round()}";
            isProgressBar__5908 = true;
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(label: this.semanticsLabel, role: (isProgressBar__5908 ? SemanticsRole.progressBar : SemanticsRole.loadingSpinner), minValue: (isProgressBar__5908 ? "0" : null), maxValue: (isProgressBar__5908 ? "100" : null), value: expandedSemanticsValue__5943, child: child));
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
    public static global::Doroti.Framework.Animation.Curve line1Head = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval(0.0, (750.0 / Progress_indicatorLibrary._kIndeterminateLinearDuration), curve: new global::Doroti.Framework.Animation.Cubic(0.2, 0.0, 0.8, 1.0)));
    public static global::Doroti.Framework.Animation.Curve line1Tail = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval((333.0 / Progress_indicatorLibrary._kIndeterminateLinearDuration), (((333.0 + 750.0)) / Progress_indicatorLibrary._kIndeterminateLinearDuration), curve: new global::Doroti.Framework.Animation.Cubic(0.4, 0.0, 1.0, 1.0)));
    public static global::Doroti.Framework.Animation.Curve line2Head = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval((1000.0 / Progress_indicatorLibrary._kIndeterminateLinearDuration), (((1000.0 + 567.0)) / Progress_indicatorLibrary._kIndeterminateLinearDuration), curve: new global::Doroti.Framework.Animation.Cubic(0.0, 0.0, 0.65, 1.0)));
    public static global::Doroti.Framework.Animation.Curve line2Tail = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval((1267.0 / Progress_indicatorLibrary._kIndeterminateLinearDuration), (((1267.0 + 533.0)) / Progress_indicatorLibrary._kIndeterminateLinearDuration), curve: new global::Doroti.Framework.Animation.Cubic(0.1, 0.0, 0.45, 1.0)));

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
        double effectiveTrackGap__8029 = (this.trackGap ?? 0.0);
        void drawLinearIndicator(double startFraction, double endFraction, Color color)
        {
            if (((endFraction - startFraction) <= 0L))
            {
                return;
            }
            var isLtr__8290 = (object.Equals(this.textDirection, TextDirection.ltr));
            double left__8353 = (((isLtr__8290 ? startFraction : (1L - endFraction))) * size.width);
            double right__8435 = (((isLtr__8290 ? endFraction : (1L - startFraction))) * size.width);
            var rect__8512 = global::Doroti.Ui.Rect.fromLTRB(left__8353, 0, right__8435, size.height);
            var paint__8575 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = color;
            return __cascade;        }))();
            if ((this.indicatorBorderRadius is not null))
            {
                global::Doroti.Ui.RRect rrect__8671 = ((global::Doroti.Ui.RRect)(object?)this.indicatorBorderRadius!.resolve(this.textDirection).toRRect(rect__8512));
                canvas.drawRRect(rrect__8671, paint__8575);
            }
            else
            {
                canvas.drawRect(rect__8512, paint__8575);
            }
        }
        void drawStopIndicator()
        {
            double maxRadius__8964 = (size.height / 2L);
            double radius__9012 = Math.Min(DartRuntimePrimitives.RequireValue(this.stopIndicatorRadius), maxRadius__8964);
            var indicatorPaint__9076 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.stopIndicatorColor!;
            return __cascade;        }))();
            global::Doroti.Ui.Offset position__9150 = ((global::Doroti.Ui.Offset)(object?)(this.textDirection switch { TextDirection.rtl => new global::Doroti.Ui.Offset(maxRadius__8964, maxRadius__8964), TextDirection.ltr => new global::Doroti.Ui.Offset((size.width - maxRadius__8964), maxRadius__8964), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            canvas.drawCircle(position__9150, radius__9012, indicatorPaint__9076);
        }
        double getEffectiveTrackGapFraction(double currentValue, double trackGapFraction)
        {
            return ((trackGapFraction * Dart_uiLibrary.clampDouble(currentValue, 0, Progress_indicatorLibrary._kTrackGapRampDownThreshold)) / Progress_indicatorLibrary._kTrackGapRampDownThreshold);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double trackGapFraction__10029 = (effectiveTrackGap__8029 / size.width);
        double? effectiveValue__10098 = ((this.value is null) ? null : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(this.value), 0.0, 1.0));
        if ((effectiveValue__10098 is not null))
        {
            double effectiveValue__10098__value10217 = DartRuntimePrimitives.RequireValue(effectiveValue__10098);
            double trackStartFraction__10262 = ((trackGapFraction__10029 > 0L) ? (DartRuntimePrimitives.RequireValue(effectiveValue__10098__value10217) + getEffectiveTrackGapFraction(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(effectiveValue__10098__value10217)), trackGapFraction__10029)) : 0);
            if ((trackStartFraction__10262 < 1L))
            {
                drawLinearIndicator(startFraction: trackStartFraction__10262, endFraction: 1, color: this.trackColor);
            }
            if (((this.stopIndicatorRadius is not null) && (DartRuntimePrimitives.RequireValue(this.stopIndicatorRadius) > 0L)))
            {
                double stopIndicatorRadius__value10651 = DartRuntimePrimitives.RequireValue(stopIndicatorRadius);
                drawStopIndicator();
            }
            if ((DartRuntimePrimitives.RequireValue(effectiveValue__10098__value10217) > 0L))
            {
                drawLinearIndicator(startFraction: 0, endFraction: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(effectiveValue__10098__value10217)), color: this.valueColor);
            }
            return;
        }
        double firstLineHead__11106 = line1Head.transform(this.animationValue);
        double firstLineTail__11176 = line1Tail.transform(this.animationValue);
        double secondLineHead__11246 = line2Head.transform(this.animationValue);
        double secondLineTail__11317 = line2Tail.transform(this.animationValue);
        if ((firstLineHead__11106 < (1L - trackGapFraction__10029)))
        {
            double trackStartFraction__11563 = ((firstLineHead__11106 > 0L) ? (firstLineHead__11106 + getEffectiveTrackGapFraction(firstLineHead__11106, trackGapFraction__10029)) : 0);
            drawLinearIndicator(startFraction: trackStartFraction__11563, endFraction: 1, color: this.trackColor);
        }
        if (((firstLineHead__11106 - firstLineTail__11176) > 0L))
        {
            drawLinearIndicator(startFraction: firstLineTail__11176, endFraction: firstLineHead__11106, color: this.valueColor);
        }
        if ((firstLineTail__11176 > trackGapFraction__10029))
        {
            double trackStartFraction__12261 = ((secondLineHead__11246 > 0L) ? (secondLineHead__11246 + getEffectiveTrackGapFraction(secondLineHead__11246, trackGapFraction__10029)) : 0);
            double trackEndFraction__12427 = ((firstLineTail__11176 < 1L) ? (firstLineTail__11176 - getEffectiveTrackGapFraction((1L - firstLineTail__11176), trackGapFraction__10029)) : 1);
            drawLinearIndicator(startFraction: trackStartFraction__12261, endFraction: trackEndFraction__12427, color: this.trackColor);
        }
        if (((secondLineHead__11246 - secondLineTail__11317) > 0L))
        {
            drawLinearIndicator(startFraction: secondLineTail__11317, endFraction: secondLineHead__11246, color: this.valueColor);
        }
        if ((secondLineTail__11317 > trackGapFraction__10029))
        {
            double trackEndFraction__13128 = ((secondLineTail__11317 < 1L) ? (secondLineTail__11317 - getEffectiveTrackGapFraction((1L - secondLineTail__11317), trackGapFraction__10029)) : 1);
            drawLinearIndicator(startFraction: 0, endFraction: trackEndFraction__13128, color: this.trackColor);
        }
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_LinearProgressIndicatorPainter__progress_indicator)(object)oldDelegate;
        return (((((((((!object.Equals(((_LinearProgressIndicatorPainter__progress_indicator)__oldPainter).trackColor, this.trackColor)) || (!object.Equals(((_LinearProgressIndicatorPainter__progress_indicator)__oldPainter).valueColor, this.valueColor))) || (((_LinearProgressIndicatorPainter__progress_indicator)__oldPainter).value != this.value)) || (((_LinearProgressIndicatorPainter__progress_indicator)__oldPainter).animationValue != this.animationValue)) || (!object.Equals(((_LinearProgressIndicatorPainter__progress_indicator)__oldPainter).textDirection, this.textDirection))) || (!object.Equals(((_LinearProgressIndicatorPainter__progress_indicator)__oldPainter).indicatorBorderRadius, this.indicatorBorderRadius))) || (!object.Equals(((_LinearProgressIndicatorPainter__progress_indicator)__oldPainter).stopIndicatorColor, this.stopIndicatorColor))) || (((_LinearProgressIndicatorPainter__progress_indicator)__oldPainter).stopIndicatorRadius != this.stopIndicatorRadius)) || (((_LinearProgressIndicatorPainter__progress_indicator)__oldPainter).trackGap != this.trackGap));
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
        System.Diagnostics.Debug.Assert(((minHeight is null) || (DartRuntimePrimitives.RequireValue(minHeight) > 0L)));
        System.Diagnostics.Debug.Assert(((value is null) || (controller is null)));
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
        this._internalController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual global::Doroti.Framework.Animation.AnimationController _controller => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.AnimationController>((((((LinearProgressIndicator)this.widget).controller ?? this.context.getInheritedWidgetOfExactType<ProgressIndicatorTheme>()?.data.controller) ?? this.context.findAncestorWidgetOfExactType<Theme>()?.data.progressIndicatorTheme.controller) ?? this._internalController));
    internal virtual void _updateControllerAnimatingStatus()
    {
        if (((this.widget._effectiveValue is null) && !((global::Doroti.Framework.Animation.AnimationController)this._internalController).isAnimating))
        {
            this._internalController.repeat();
        }
        else
        {
            if (((this.widget._effectiveValue is not null) && ((global::Doroti.Framework.Animation.AnimationController)this._internalController).isAnimating))
            {
                this._internalController.stop();
            }
        }
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildIndicator(global::Doroti.Framework.Widgets.BuildContext context, double animationValue, TextDirection textDirection)
    {
        ProgressIndicatorThemeData indicatorTheme__23578 = ProgressIndicatorTheme.of(context);
        bool year2023__23646 = ((((LinearProgressIndicator)this.widget).year2023 ?? indicatorTheme__23578.year2023) ?? true);
        ProgressIndicatorThemeData defaults__23746 = (((object)Theme.of(context).useMaterial3) switch { true => (year2023__23646 ? new _LinearProgressIndicatorDefaultsM3Year2023__progress_indicator(context) : new _LinearProgressIndicatorDefaultsM3__progress_indicator(context)), false => DartRuntimePrimitives.ConvertValue<ProgressIndicatorThemeData>(new _LinearProgressIndicatorDefaultsM2__progress_indicator(context)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Color trackColor__24038 = ((global::Doroti.Ui.Color)(object?)((((LinearProgressIndicator)this.widget).backgroundColor ?? indicatorTheme__23578.linearTrackColor) ?? defaults__23746.linearTrackColor!));
        double minHeight__24165 = ((((LinearProgressIndicator)this.widget).minHeight ?? indicatorTheme__23578.linearMinHeight) ?? DartRuntimePrimitives.RequireValue(defaults__23746.linearMinHeight));
        global::Doroti.Framework.Painting.BorderRadiusGeometry? borderRadius__24298 = ((((LinearProgressIndicator)this.widget).borderRadius ?? indicatorTheme__23578.borderRadius) ?? defaults__23746.borderRadius);
        global::Doroti.Ui.Color? stopIndicatorColor__24415 = ((global::Doroti.Ui.Color?)(object?)(!year2023__23646 ? ((((LinearProgressIndicator)this.widget).stopIndicatorColor ?? indicatorTheme__23578.stopIndicatorColor) ?? defaults__23746.stopIndicatorColor) : null));
        double? stopIndicatorRadius__24612 = (!year2023__23646 ? ((((LinearProgressIndicator)this.widget).stopIndicatorRadius ?? indicatorTheme__23578.stopIndicatorRadius) ?? defaults__23746.stopIndicatorRadius) : null);
        double? trackGap__24813 = (!year2023__23646 ? ((((LinearProgressIndicator)this.widget).trackGap ?? indicatorTheme__23578.trackGap) ?? defaults__23746.trackGap) : null);
        global::Doroti.Framework.Widgets.Widget result__24936 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: double.PositiveInfinity, minHeight: minHeight__24165), child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _LinearProgressIndicatorPainter__progress_indicator(trackColor: trackColor__24038, valueColor: this.widget._getValueColor(context, defaultColor: defaults__23746.color), value: this.widget._effectiveValue, animationValue: animationValue, textDirection: textDirection, indicatorBorderRadius: borderRadius__24298, stopIndicatorColor: stopIndicatorColor__24415, stopIndicatorRadius: stopIndicatorRadius__24612, trackGap: trackGap__24813))));
        if (((borderRadius__24298 is not null) && (this.widget._effectiveValue is null)))
        {
            result__24936 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ClipRRect(borderRadius: borderRadius__24298, child: result__24936));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)this.widget._buildSemanticsWrapper(context: context, child: result__24936));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.TextDirection textDirection__25996 = Directionality.of(context);
        if ((this.widget._effectiveValue is not null))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildIndicator(context, ((global::Doroti.Framework.Animation.AnimationController)this._controller).value, textDirection__25996));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: ((global::Doroti.Framework.Animation.AnimationController)this._controller).view, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildIndicator(context, ((global::Doroti.Framework.Animation.AnimationController)this._controller).value, textDirection__25996));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
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
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
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
    internal static double _twoPi = (Dart_mathLibrary.pi * 2.0);
    internal const double _epsilon = 0.001;
    internal static double _sweep = (_twoPi - _epsilon);
    internal static double _startAngle = (-Dart_mathLibrary.pi / 2.0);

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
        this.arcStart = ((value is not null) ? _startAngle : (((_startAngle + (((tailValue * 3L) / 2L) * Dart_mathLibrary.pi)) + ((rotationValue * Dart_mathLibrary.pi) * 2.0)) + ((offsetValue * 0.5) * Dart_mathLibrary.pi)));
        this.arcSweep = ((value is not null) ? (Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), 0.0, 1.0) * _sweep) : Math.Max(((((headValue * 3L) / 2L) * Dart_mathLibrary.pi) - (((tailValue * 3L) / 2L) * Dart_mathLibrary.pi)), _epsilon));
    }

    public override void paint(Canvas canvas, Size size)
    {
        var paint__27905 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.valueColor;
            __cascade.strokeWidth = this.strokeWidth;
            __cascade.style = PaintingStyle.stroke;
            return __cascade;        }))();
        double strokeOffset__28161 = ((this.strokeWidth / 2L) * -this.strokeAlign);
        var arcBaseOffset__28218 = new global::Doroti.Ui.Offset(strokeOffset__28161, strokeOffset__28161);
        var arcActualSize__28280 = new global::Doroti.Ui.Size((size.width - (strokeOffset__28161 * 2L)), (size.height - (strokeOffset__28161 * 2L)));
        bool hasGap__28380 = ((this.trackGap is not null) && (DartRuntimePrimitives.RequireValue(this.trackGap) > 0L));
        if ((this.trackColor is not null))
        {
            var backgroundPaint__28467 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.trackColor!;
            __cascade.strokeWidth = this.strokeWidth;
            __cascade.strokeCap = (this.strokeCap ?? StrokeCap.round);
            __cascade.style = PaintingStyle.stroke;
            return __cascade;        }))();
            if (((hasGap__28380 && (this.value is not null)) && (DartRuntimePrimitives.RequireValue(this.value) > _epsilon)))
            {
                double value__value28734 = DartRuntimePrimitives.RequireValue(value);
                double arcRadius__28793 = (arcActualSize__28280.shortestSide / 2L);
                double strokeRadius__28858 = (this.strokeWidth / arcRadius__28793);
                double gapRadius__28919 = (DartRuntimePrimitives.RequireValue(this.trackGap) / arcRadius__28793);
                double startGap__28975 = (strokeRadius__28858 + gapRadius__28919);
                double endGap__29033 = ((DartRuntimePrimitives.RequireValue(this.value) < _epsilon) ? startGap__28975 : (startGap__28975 * 2L));
                double startSweep__29108 = (((-Dart_mathLibrary.pi / 2.0)) + startGap__28975);
                double endSweep__29171 = Math.Max(0.0, ((_twoPi - (Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(this.value), 0.0, 1.0) * _twoPi)) - endGap__29033));
                canvas.save();
                canvas.scale(-1, 1);
                canvas.translate(-size.width, 0);
                canvas.drawArc((arcBaseOffset__28218 & arcActualSize__28280), startSweep__29108, endSweep__29171, false, backgroundPaint__28467);
                canvas.restore();
            }
            else
            {
                canvas.drawArc((arcBaseOffset__28218 & arcActualSize__28280), 0, _sweep, false, backgroundPaint__28467);
            }
        }
        if (this.year2023)
        {
            if (((this.value is null) && (this.strokeCap is null)))
            {
                paint__27905.strokeCap = StrokeCap.square;
            }
            else
            {
                paint__27905.strokeCap = (this.strokeCap ?? StrokeCap.butt);
            }
        }
        else
        {
            paint__27905.strokeCap = (this.strokeCap ?? StrokeCap.round);
        }
        canvas.drawArc((arcBaseOffset__28218 & arcActualSize__28280), this.arcStart, this.arcSweep, false, paint__27905);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_CircularProgressIndicatorPainter__progress_indicator)(object)oldDelegate;
        return ((((((((((((!object.Equals(((_CircularProgressIndicatorPainter__progress_indicator)__oldPainter).trackColor, this.trackColor)) || (!object.Equals(((_CircularProgressIndicatorPainter__progress_indicator)__oldPainter).valueColor, this.valueColor))) || (((_CircularProgressIndicatorPainter__progress_indicator)__oldPainter).value != this.value)) || (((_CircularProgressIndicatorPainter__progress_indicator)__oldPainter).headValue != this.headValue)) || (((_CircularProgressIndicatorPainter__progress_indicator)__oldPainter).tailValue != this.tailValue)) || (((_CircularProgressIndicatorPainter__progress_indicator)__oldPainter).offsetValue != this.offsetValue)) || (((_CircularProgressIndicatorPainter__progress_indicator)__oldPainter).rotationValue != this.rotationValue)) || (((_CircularProgressIndicatorPainter__progress_indicator)__oldPainter).strokeWidth != this.strokeWidth)) || (((_CircularProgressIndicatorPainter__progress_indicator)__oldPainter).strokeAlign != this.strokeAlign)) || (!object.Equals(((_CircularProgressIndicatorPainter__progress_indicator)__oldPainter).strokeCap, this.strokeCap))) || (((_CircularProgressIndicatorPainter__progress_indicator)__oldPainter).trackGap != this.trackGap)) || (((_CircularProgressIndicatorPainter__progress_indicator)__oldPainter).year2023 != this.year2023));
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
        this._indicatorType = _ActivityIndicatorType__progress_indicator.material;
        System.Diagnostics.Debug.Assert(((value is null) || (controller is null)));
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
    internal static long _pathCount = (checked((long)(Progress_indicatorLibrary._kIndeterminateCircularDuration / 1333L)));
    internal static long _rotationCount = (checked((long)(Progress_indicatorLibrary._kIndeterminateCircularDuration / 2222L)));
    internal static global::Doroti.Framework.Animation.Animatable<double> _strokeHeadTween = new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.0, 0.5, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn)).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.SawTooth(_pathCount)));
    internal static global::Doroti.Framework.Animation.Animatable<double> _strokeTailTween = new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.5, 1.0, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn)).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.SawTooth(_pathCount)));
    internal static global::Doroti.Framework.Animation.Animatable<double> _offsetTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.SawTooth(_pathCount)));
    internal static global::Doroti.Framework.Animation.Animatable<double> _rotationTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.SawTooth(_rotationCount)));
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
        this._internalController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual global::Doroti.Framework.Animation.AnimationController _controller => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.AnimationController>((((((CircularProgressIndicator)this.widget).controller ?? this.context.getInheritedWidgetOfExactType<ProgressIndicatorTheme>()?.data.controller) ?? this.context.findAncestorWidgetOfExactType<Theme>()?.data.progressIndicatorTheme.controller) ?? this._internalController));
    internal virtual void _updateControllerAnimatingStatus()
    {
        if (((this.widget._effectiveValue is null) && !((global::Doroti.Framework.Animation.AnimationController)this._internalController).isAnimating))
        {
            this._internalController.repeat();
        }
        else
        {
            if (((this.widget._effectiveValue is not null) && ((global::Doroti.Framework.Animation.AnimationController)this._internalController).isAnimating))
            {
                this._internalController.stop();
            }
        }
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildCupertinoIndicator(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color? tickColor__44701 = ((global::Doroti.Ui.Color?)(object?)((CircularProgressIndicator)this.widget).backgroundColor);
        double? value__44755 = this.widget._effectiveValue;
        if ((value__44755 is null))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoActivityIndicator(key: this.widget.key, color: tickColor__44701));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)CupertinoActivityIndicator.CreatePartiallyRevealed(key: this.widget.key, color: tickColor__44701, progress: DartRuntimePrimitives.RequireValue(value__44755)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildMaterialIndicator(global::Doroti.Framework.Widgets.BuildContext context, double headValue, double tailValue, double offsetValue, double rotationValue)
    {
        ProgressIndicatorThemeData indicatorTheme__45230 = ProgressIndicatorTheme.of(context);
        bool year2023__45298 = ((((CircularProgressIndicator)this.widget).year2023 ?? indicatorTheme__45230.year2023) ?? true);
        ProgressIndicatorThemeData defaults__45398 = (((object)Theme.of(context).useMaterial3) switch { true => (year2023__45298 ? new _CircularProgressIndicatorDefaultsM3Year2023__progress_indicator(context, indeterminate: (this.widget._effectiveValue is null)) : new _CircularProgressIndicatorDefaultsM3__progress_indicator(context, indeterminate: (this.widget._effectiveValue is null))), false => DartRuntimePrimitives.ConvertValue<ProgressIndicatorThemeData>(new _CircularProgressIndicatorDefaultsM2__progress_indicator(context, indeterminate: (this.widget._effectiveValue is null))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Color? trackColor__45961 = ((global::Doroti.Ui.Color?)(object?)((((CircularProgressIndicator)this.widget).backgroundColor ?? indicatorTheme__45230.circularTrackColor) ?? defaults__45398.circularTrackColor));
        double strokeWidth__46091 = ((((CircularProgressIndicator)this.widget).strokeWidth ?? indicatorTheme__45230.strokeWidth) ?? DartRuntimePrimitives.RequireValue(defaults__45398.strokeWidth));
        double strokeAlign__46205 = ((((CircularProgressIndicator)this.widget).strokeAlign ?? indicatorTheme__45230.strokeAlign) ?? DartRuntimePrimitives.RequireValue(defaults__45398.strokeAlign));
        global::Doroti.Ui.StrokeCap? strokeCap__46323 = ((global::Doroti.Ui.StrokeCap?)(object?)(((CircularProgressIndicator)this.widget).strokeCap ?? indicatorTheme__45230.strokeCap));
        global::Doroti.Framework.Rendering.BoxConstraints constraints__46406 = ((((CircularProgressIndicator)this.widget).constraints ?? indicatorTheme__45230.constraints) ?? defaults__45398.constraints!);
        double? trackGap__46521 = (year2023__45298 ? null : ((((CircularProgressIndicator)this.widget).trackGap ?? indicatorTheme__45230.trackGap) ?? defaults__45398.trackGap));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? effectivePadding__46661 = ((((CircularProgressIndicator)this.widget).padding ?? indicatorTheme__45230.circularTrackPadding) ?? defaults__45398.circularTrackPadding);
        global::Doroti.Framework.Widgets.Widget result__46788 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: constraints__46406, child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _CircularProgressIndicatorPainter__progress_indicator(trackColor: trackColor__45961, valueColor: this.widget._getValueColor(context, defaultColor: defaults__45398.color), value: this.widget._effectiveValue, headValue: headValue, tailValue: tailValue, offsetValue: offsetValue, rotationValue: rotationValue, strokeWidth: strokeWidth__46091, strokeAlign: strokeAlign__46205, strokeCap: strokeCap__46323, trackGap: trackGap__46521, year2023: year2023__45298))));
        if ((effectivePadding__46661 is not null))
        {
            result__46788 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: effectivePadding__46661, child: result__46788));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)this.widget._buildSemanticsWrapper(context: context, child: result__46788));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildAnimation()
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: this._controller, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildMaterialIndicator(context, _strokeHeadTween.evaluate(this._controller), _strokeTailTween.evaluate(this._controller), _offsetTween.evaluate(this._controller), _rotationTween.evaluate(this._controller)));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) => {
switch (((CircularProgressIndicator)this.widget)._indicatorType)
{
    case _ActivityIndicatorType__progress_indicator.material:
        {
            if ((this.widget._effectiveValue is not null))
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildMaterialIndicator(context, 0.0, 0.0, 0, 0.0));
            }
            return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildAnimation());
        }
    case _ActivityIndicatorType__progress_indicator.adaptive:
        {
            ThemeData theme__48583 = Theme.of(context);
            switch (theme__48583.platform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                    {
                        return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildCupertinoIndicator(context));
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        if ((this.widget._effectiveValue is not null))
                        {
                            return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildMaterialIndicator(context, 0.0, 0.0, 0, 0.0));
                        }
                        return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildAnimation());
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            break;
        }
}
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
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
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
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
        double arcEnd__49879 = (this.arcStart + this.arcSweep);
        double ux__49926 = global::Doroti.Runtime.Dart_mathLibrary.cos(arcEnd__49879);
        double uy__49966 = global::Doroti.Runtime.Dart_mathLibrary.sin(arcEnd__49879);
        DartRuntimePrimitives.Assert(() => (size.width == size.height));
        double radius__50046 = (size.width / 2.0);
        double arrowheadPointX__50090 = ((radius__50046 + (ux__49926 * radius__50046)) + (((-uy__49966 * this.strokeWidth) * 2.0) * this.arrowheadScale));
        double arrowheadPointY__50190 = ((radius__50046 + (uy__49966 * radius__50046)) + (((ux__49926 * this.strokeWidth) * 2.0) * this.arrowheadScale));
        double arrowheadRadius__50289 = ((this.strokeWidth * 2.0) * this.arrowheadScale);
        double innerRadius__50360 = (radius__50046 - arrowheadRadius__50289);
        double outerRadius__50417 = (radius__50046 + arrowheadRadius__50289);
        var path__50468 = ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Ui.Path();
            __cascade.moveTo((radius__50046 + (ux__49926 * innerRadius__50360)), (radius__50046 + (uy__49966 * innerRadius__50360)));
            __cascade.lineTo((radius__50046 + (ux__49926 * outerRadius__50417)), (radius__50046 + (uy__49966 * outerRadius__50417)));
            __cascade.lineTo(arrowheadPointX__50090, arrowheadPointY__50190);
            __cascade.close();
            return __cascade;        }))();
        var paint__50697 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.valueColor;
            __cascade.strokeWidth = this.strokeWidth;
            __cascade.style = PaintingStyle.fill;
            return __cascade;        }))();
        canvas.drawPath(path__50468, paint__50697);
    }

    public override void paint(Canvas canvas, Size size)
    {
        base.paint(canvas, size);
        if ((this.arrowheadScale > 0.0))
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
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __indicatorMargin = indicatorMargin ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(4.0);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __indicatorPadding = indicatorPadding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(12.0);
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
                __late__convertTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.1, _strokeHeadInterval)));
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
                __late__additionalRotationTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: -0.1, end: -0.2), weight: _strokeHeadInterval), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: -0.2, end: 1.35), weight: (1L - _strokeHeadInterval)) }));
                __late__additionalRotationTween_initialized = true;
            }
            return __late__additionalRotationTween;
        }
    }
    internal virtual double? _lastValue { get; set; } = default;

    public override RefreshProgressIndicator widget => ((RefreshProgressIndicator?)(object?)base.widget)!;
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        double? value__54874 = this.widget._effectiveValue;
        if ((value__54874 is not null))
        {
            double value__54874__value54914 = DartRuntimePrimitives.RequireValue(value__54874);
            _lastValue = DartRuntimePrimitives.RequireValue(value__54874__value54914);
            this._controller.value = (this._convertTween.transform(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value__54874__value54914))) * (((1333L / 2L) / Progress_indicatorLibrary._kIndeterminateCircularDuration)));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildAnimation());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override global::Doroti.Framework.Widgets.Widget _buildAnimation()
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: this._controller, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildMaterialIndicator(context, (1.05 * _CircularProgressIndicatorState__progress_indicator._strokeHeadTween.transform(((global::Doroti.Framework.Animation.AnimationController)this._controller).value)), _CircularProgressIndicatorState__progress_indicator._strokeTailTween.transform(((global::Doroti.Framework.Animation.AnimationController)this._controller).value), _CircularProgressIndicatorState__progress_indicator._offsetTween.transform(((global::Doroti.Framework.Animation.AnimationController)this._controller).value), _CircularProgressIndicatorState__progress_indicator._rotationTween.transform(((global::Doroti.Framework.Animation.AnimationController)this._controller).value)));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override global::Doroti.Framework.Widgets.Widget _buildMaterialIndicator(global::Doroti.Framework.Widgets.BuildContext context, double headValue, double tailValue, double offsetValue, double rotationValue)
    {
        double? value__55944 = this.widget._effectiveValue;
        double arrowheadScale__55993 = ((value__55944 is null) ? 0.0 : new global::Doroti.Framework.Animation.Interval(0.1, _strokeHeadInterval).transform(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value__55944))));
        double rotation__56124 = default!;
        if (((value__55944 is null) && (this._lastValue is null)))
        {
            rotation__56124 = 0.0;
        }
        else
        {
            rotation__56124 = (Dart_mathLibrary.pi * this._additionalRotationTween.transform((value__55944 ?? DartRuntimePrimitives.RequireValue(this._lastValue))));
        }
        global::Doroti.Ui.Color valueColor__56319 = ((global::Doroti.Ui.Color)(object?)this.widget._getValueColor(context));
        double opacity__56381 = valueColor__56319.opacity;
        valueColor__56319 = valueColor__56319.withOpacity(1.0);
        ProgressIndicatorThemeData defaults__56495 = (((object)Theme.of(context).useMaterial3) switch { true => DartRuntimePrimitives.ConvertValue<ProgressIndicatorThemeData>(new _CircularProgressIndicatorDefaultsM3Year2023__progress_indicator(context, indeterminate: (value__55944 is null))), false => DartRuntimePrimitives.ConvertValue<ProgressIndicatorThemeData>(new _CircularProgressIndicatorDefaultsM2__progress_indicator(context, indeterminate: (value__55944 is null))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        ProgressIndicatorThemeData indicatorTheme__56783 = ProgressIndicatorTheme.of(context);
        global::Doroti.Ui.Color backgroundColor__56852 = ((global::Doroti.Ui.Color)(object?)((((RefreshProgressIndicator)this.widget).backgroundColor ?? indicatorTheme__56783.refreshBackgroundColor) ?? Theme.of(context).canvasColor));
        double strokeWidth__57009 = ((this.widget.strokeWidth ?? indicatorTheme__56783.strokeWidth) ?? DartRuntimePrimitives.RequireValue(defaults__56495.strokeWidth));
        double strokeAlign__57123 = ((this.widget.strokeAlign ?? indicatorTheme__56783.strokeAlign) ?? DartRuntimePrimitives.RequireValue(defaults__56495.strokeAlign));
        global::Doroti.Ui.StrokeCap? strokeCap__57241 = ((global::Doroti.Ui.StrokeCap?)(object?)(this.widget.strokeCap ?? indicatorTheme__56783.strokeCap));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)this.widget._buildSemanticsWrapper(context: context, child: new global::Doroti.Framework.Widgets.Padding(padding: ((RefreshProgressIndicator)this.widget).indicatorMargin, child: global::Doroti.Framework.Widgets.SizedBox.CreateFromSize(size: new global::Doroti.Ui.Size(_indicatorSize), child: new Material(type: MaterialType.circle, color: backgroundColor__56852, elevation: ((RefreshProgressIndicator)this.widget).elevation, child: new global::Doroti.Framework.Widgets.Padding(padding: ((RefreshProgressIndicator)this.widget).indicatorPadding, child: new global::Doroti.Framework.Widgets.Opacity(opacity: opacity__56381, child: global::Doroti.Framework.Widgets.Transform.CreateRotate(angle: rotation__56124, child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _RefreshProgressIndicatorPainter__progress_indicator(valueColor: valueColor__56319, value: null, headValue: headValue, tailValue: tailValue, offsetValue: offsetValue, rotationValue: rotationValue, strokeWidth: strokeWidth__57009, strokeAlign: strokeAlign__57123, arrowheadScale: arrowheadScale__55993, strokeCap: strokeCap__57241))))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CircularProgressIndicatorDefaultsM2__progress_indicator : ProgressIndicatorThemeData
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
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    public virtual bool indeterminate { get; private set; } = default!;

    internal _CircularProgressIndicatorDefaultsM2__progress_indicator(global::Doroti.Framework.Widgets.BuildContext context, bool indeterminate)
    {
        this.context = context;
        this.indeterminate = indeterminate;
    }

    public virtual global::Doroti.Ui.Color color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public override double? strokeWidth => 4.0;
    public override double? strokeAlign => CircularProgressIndicator.strokeAlignCenter;
    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints => new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: 36.0, minHeight: 36.0);
}

internal class _LinearProgressIndicatorDefaultsM2__progress_indicator : ProgressIndicatorThemeData
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
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _LinearProgressIndicatorDefaultsM2__progress_indicator(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color linearTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.background);
    public virtual double linearMinHeight => 4.0;
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
                __late__colors = Theme.of(this.context).colorScheme;
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

    public virtual global::Doroti.Ui.Color color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual double strokeWidth => 4.0;
    public override double? strokeAlign => CircularProgressIndicator.strokeAlignCenter;
    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints => new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: 36.0, minHeight: 36.0);
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
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _LinearProgressIndicatorDefaultsM3Year2023__progress_indicator(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color linearTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondaryContainer);
    public virtual double linearMinHeight => 4.0;
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
                __late__colors = Theme.of(this.context).colorScheme;
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

    public virtual global::Doroti.Ui.Color color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? circularTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this.indeterminate ? null : this._colors.secondaryContainer));
    public virtual double strokeWidth => 4.0;
    public override double? strokeAlign => CircularProgressIndicator.strokeAlignInside;
    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints => new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: 40.0, minHeight: 40.0);
    public override double? trackGap => 4.0;
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? circularTrackPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.CreateAll(4.0));
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
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _LinearProgressIndicatorDefaultsM3__progress_indicator(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color linearTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondaryContainer);
    public virtual double linearMinHeight => 4.0;
    public virtual global::Doroti.Framework.Painting.BorderRadius borderRadius => global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular((4.0 / 2L)));
    public virtual global::Doroti.Ui.Color stopIndicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public override double? stopIndicatorRadius => DartRuntimePrimitives.ConvertValue<double>((4.0 / 2L));
    public override double? trackGap => 4.0;
}
