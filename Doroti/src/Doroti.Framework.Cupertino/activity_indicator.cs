// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/activity_indicator.dart
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

public static partial class Activity_indicatorLibrary
{
    internal static double _kDefaultIndicatorRadius = 10.0;
}

public static partial class Activity_indicatorLibrary
{
    internal static Color _kActiveTickColor = ((Color)(object?)new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(4282137668L), darkColor: new global::Doroti.Ui.Color(4293651445L)));
}

public class CupertinoActivityIndicator : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual Color? color { get; private set; }
    public virtual bool animating { get; private set; } = default!;
    public virtual double radius { get; private set; } = default!;
    public virtual double progress { get; private set; } = default!;

    public CupertinoActivityIndicator(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, bool animating = true, double? radius = null) : base(key: key)
    {
        double __radius = radius ?? Activity_indicatorLibrary._kDefaultIndicatorRadius;
        this.color = color;
        this.animating = animating;
        this.radius = __radius;
        this.progress = 1.0;
        System.Diagnostics.Debug.Assert((__radius > 0.0));
    }

    public static CupertinoActivityIndicator CreatePartiallyRevealed(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, double? radius = null, double progress = 1.0)
    {
        var __instance = new CupertinoActivityIndicator(key: key, color: color, radius: radius);
        double __radius = radius ?? Activity_indicatorLibrary._kDefaultIndicatorRadius;
        __instance.color = color;
        __instance.radius = __radius;
        __instance.progress = progress;
        __instance.animating = false;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoActivityIndicatorState__activity_indicator());
}

internal class _CupertinoActivityIndicatorState__activity_indicator : global::Doroti.Framework.Widgets.State<CupertinoActivityIndicator>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<CupertinoActivityIndicator>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(duration: Duration.Create(seconds: 1L), vsync: this);
        if (((CupertinoActivityIndicator)this.widget).animating)
        {
            this._controller.repeat();
        }
    }

    public override void didUpdateWidget(CupertinoActivityIndicator oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((CupertinoActivityIndicator)this.widget).animating != ((CupertinoActivityIndicator)oldWidget).animating))
        {
            if (((CupertinoActivityIndicator)this.widget).animating)
            {
                this._controller.repeat();
            }
            else
            {
                this._controller.stop();
            }
        }
    }

    public override void dispose()
    {
        this._controller.dispose();
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateSquare(dimension: (((CupertinoActivityIndicator)this.widget).radius * 2L), child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _CupertinoActivityIndicatorPainter__activity_indicator(position: this._controller, activeColor: (((CupertinoActivityIndicator)this.widget).color ?? CupertinoDynamicColor.resolve(Activity_indicatorLibrary._kActiveTickColor, context)), radius: ((CupertinoActivityIndicator)this.widget).radius, progress: ((CupertinoActivityIndicator)this.widget).progress))));
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
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

public static partial class Activity_indicatorLibrary
{
    internal static double _kTwoPI = (Dart_mathLibrary.pi * 2.0);
}

public static partial class Activity_indicatorLibrary
{
    internal static List<long> _kAlphaValues = new List<long> { 47L, 47L, 47L, 47L, 72L, 97L, 122L, 147L };
}

public static partial class Activity_indicatorLibrary
{
    internal static long _partiallyRevealedAlpha = 147L;
}

internal class _CupertinoActivityIndicatorPainter__activity_indicator : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual global::Doroti.Framework.Animation.Animation<double> position { get; private set; } = default!;
    public virtual Color activeColor { get; private set; } = default!;
    public virtual double radius { get; private set; } = default!;
    public virtual double progress { get; private set; } = default!;
    public virtual RRect tickFundamentalShape { get; private set; } = default!;

    internal _CupertinoActivityIndicatorPainter__activity_indicator(global::Doroti.Framework.Animation.Animation<double> position, Color activeColor, double radius, double progress) : base(repaint: position)
    {
        this.position = position;
        this.activeColor = activeColor;
        this.radius = radius;
        this.progress = progress;
        this.tickFundamentalShape = global::Doroti.Ui.RRect.fromLTRBXY((-radius / Activity_indicatorLibrary._kDefaultIndicatorRadius), (-radius / 3.0), (radius / Activity_indicatorLibrary._kDefaultIndicatorRadius), -radius, (radius / Activity_indicatorLibrary._kDefaultIndicatorRadius), (radius / Activity_indicatorLibrary._kDefaultIndicatorRadius));
    }

    public override void paint(Canvas canvas, Size size)
    {
        var paintLocal = new global::Doroti.Ui.Paint();
        long tickCount = checked((long)(Activity_indicatorLibrary._kAlphaValues.Count));
        canvas.save();
        canvas.translate((size.width / 2.0), (size.height / 2.0));
        long activeTick = ((tickCount * ((global::Doroti.Framework.Animation.Animation<double>)this.position).value)).floor();
        for (var i = 0L; (i < (tickCount * this.progress)); ++i)
        {
            long t = (((i - activeTick)) % tickCount);
            paintLocal.color = this.activeColor.withAlpha(((this.progress < 1L) ? Activity_indicatorLibrary._partiallyRevealedAlpha : Activity_indicatorLibrary._kAlphaValues[(int)(t)]));
            canvas.drawRRect(this.tickFundamentalShape, paintLocal);
            canvas.rotate((Activity_indicatorLibrary._kTwoPI / tickCount));
        }
        canvas.restore();
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_CupertinoActivityIndicatorPainter__activity_indicator)(object)oldDelegate;
        return (((!object.Equals(((_CupertinoActivityIndicatorPainter__activity_indicator)__oldPainter).position, this.position)) || (!object.Equals(((_CupertinoActivityIndicatorPainter__activity_indicator)__oldPainter).activeColor, this.activeColor))) || (((_CupertinoActivityIndicatorPainter__activity_indicator)__oldPainter).progress != this.progress));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoLinearActivityIndicator : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual double progress { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;
    public virtual Color? color { get; private set; }

    public CupertinoLinearActivityIndicator(global::Doroti.Framework.Foundation.Key? key = null, double progress = default!, double height = 4.5, Color? color = null) : base(key: key)
    {
        this.progress = progress;
        this.height = height;
        this.color = color;
        System.Diagnostics.Debug.Assert((height > 0L));
        System.Diagnostics.Debug.Assert(((progress >= 0.0) && (progress <= 1.0)));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: this.height, minWidth: double.PositiveInfinity), child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _CupertinoLinearActivityIndicator__activity_indicator(progress: this.progress, color: this.color))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoLinearActivityIndicator__activity_indicator : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual double progress { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    internal virtual Paint _backgroundPaint { get; private set; } = default!;
    internal virtual Paint _progressPaint { get; private set; } = default!;

    internal _CupertinoLinearActivityIndicator__activity_indicator(double progress, Color? color = null)
    {
        this.progress = progress;
        this.color = color;
        this._backgroundPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = CupertinoColors.systemFill;
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
        this._progressPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = (color ?? CupertinoColors.activeBlue);
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
    }

    public override void paint(Canvas canvas, Size size)
    {
        canvas.drawRRect(global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular((size.height / 2L))).toRRect((Offset.zero & size)), this._backgroundPaint);
        if ((this.progress > 0L))
        {
            canvas.drawRRect(global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular((size.height / 2L))).toRRect((Offset.zero & new global::Doroti.Ui.Size((Dart_uiLibrary.clampDouble(this.progress, 0.0, 1.0) * size.width), size.height))), this._progressPaint);
        }
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => ((((_CupertinoLinearActivityIndicator__activity_indicator)oldDelegate).progress != this.progress) || (!object.Equals(((_CupertinoLinearActivityIndicator__activity_indicator)oldDelegate).color, this.color)));
}
