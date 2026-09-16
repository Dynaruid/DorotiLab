// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/activity_indicator.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Activity_indicatorLibrary
{
    internal static double _kDefaultIndicatorRadius = 10.0;
}

public static partial class Activity_indicatorLibrary
{
    internal static Color _kActiveTickColor = new CupertinoDynamicColor(color: new Color(4282137668L), darkColor: new Color(4293651445L));
}

public class CupertinoActivityIndicator : StatefulWidget
{
    public virtual Color? color { get; private set; }
    public virtual bool animating { get; private set; } = default!;
    public virtual double radius { get; private set; } = default!;
    public virtual double progress { get; private set; } = default!;

    public CupertinoActivityIndicator(Key? key = null, Color? color = null, bool animating = true, double? radius = null) : base(key: key)
    {
        double __radius = radius ?? Activity_indicatorLibrary._kDefaultIndicatorRadius;
        this.color = color;
        this.animating = animating;
        this.radius = __radius;
        progress = 1.0;
        System.Diagnostics.Debug.Assert(__radius > 0.0);
    }

    public static CupertinoActivityIndicator CreatePartiallyRevealed(Key? key = null, Color? color = null, double? radius = null, double progress = 1.0)
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

internal class _CupertinoActivityIndicatorState__activity_indicator : State<CupertinoActivityIndicator>, SingleTickerProviderStateMixin<CupertinoActivityIndicator>
{
    internal virtual AnimationController _controller { get; set; } = default!;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new AnimationController(duration: Duration.Create(seconds: 1L), vsync: this);
        if (widget.animating)
        {
            _controller.repeat();
        }
    }

    public override void didUpdateWidget(CupertinoActivityIndicator oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (widget.animating != oldWidget.animating)
        {
            if (widget.animating)
            {
                _controller.repeat();
            }
            else
            {
                _controller.stop();
            }
        }
    }

    public override void dispose()
    {
        _controller.dispose();
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

    public override Widget build(BuildContext context)
    {
        return SizedBox.CreateSquare(dimension: widget.radius * 2L, child: new CustomPaint(painter: new _CupertinoActivityIndicatorPainter__activity_indicator(position: _controller, activeColor: widget.color ?? CupertinoDynamicColor.resolve(Activity_indicatorLibrary._kActiveTickColor, context), radius: widget.radius, progress: widget.progress)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(System.Action<Duration> onTick)
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

public static partial class Activity_indicatorLibrary
{
    internal static double _kTwoPI = Dart_mathLibrary.pi * 2.0;
}

public static partial class Activity_indicatorLibrary
{
    internal static List<long> _kAlphaValues = new List<long> { 47L, 47L, 47L, 47L, 72L, 97L, 122L, 147L };
}

public static partial class Activity_indicatorLibrary
{
    internal static long _partiallyRevealedAlpha = 147L;
}

internal class _CupertinoActivityIndicatorPainter__activity_indicator : CustomPainter
{
    public virtual Animation<double> position { get; private set; } = default!;
    public virtual Color activeColor { get; private set; } = default!;
    public virtual double radius { get; private set; } = default!;
    public virtual double progress { get; private set; } = default!;
    public virtual RRect tickFundamentalShape { get; private set; } = default!;

    internal _CupertinoActivityIndicatorPainter__activity_indicator(Animation<double> position, Color activeColor, double radius, double progress) : base(repaint: position)
    {
        this.position = position;
        this.activeColor = activeColor;
        this.radius = radius;
        this.progress = progress;
        tickFundamentalShape = RRect.fromLTRBXY(-radius / Activity_indicatorLibrary._kDefaultIndicatorRadius, -radius / 3.0, radius / Activity_indicatorLibrary._kDefaultIndicatorRadius, -radius, radius / Activity_indicatorLibrary._kDefaultIndicatorRadius, radius / Activity_indicatorLibrary._kDefaultIndicatorRadius);
    }

    public override void paint(Canvas canvas, Size size)
    {
        var paintLocal = new Paint();
        long tickCount = checked(Activity_indicatorLibrary._kAlphaValues.Count);
        canvas.save();
        canvas.translate(size.width / 2.0, size.height / 2.0);
        long activeTick = (tickCount * position.value).floor();
        for (var i = 0L; i < (tickCount * progress); ++i)
        {
            long t = (i - activeTick) % tickCount;
            paintLocal.color = activeColor.withAlpha((progress < 1L) ? Activity_indicatorLibrary._partiallyRevealedAlpha : Activity_indicatorLibrary._kAlphaValues[(int)t]);
            canvas.drawRRect(tickFundamentalShape, paintLocal);
            canvas.rotate(Activity_indicatorLibrary._kTwoPI / tickCount);
        }
        canvas.restore();
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldPainter = (_CupertinoActivityIndicatorPainter__activity_indicator)oldDelegate;
        return (!Equals(__oldPainter.position, position)) || (!Equals(__oldPainter.activeColor, activeColor)) || (__oldPainter.progress != progress);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoLinearActivityIndicator : StatelessWidget
{
    public virtual double progress { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;
    public virtual Color? color { get; private set; }

    public CupertinoLinearActivityIndicator(Key? key = null, double progress = default!, double height = 4.5, Color? color = null) : base(key: key)
    {
        this.progress = progress;
        this.height = height;
        this.color = color;
        System.Diagnostics.Debug.Assert(height > 0L);
        System.Diagnostics.Debug.Assert((progress >= 0.0) && (progress <= 1.0));
    }

    public override Widget build(BuildContext context)
    {
        return new ConstrainedBox(constraints: new BoxConstraints(minHeight: height, minWidth: double.PositiveInfinity), child: new CustomPaint(painter: new _CupertinoLinearActivityIndicator__activity_indicator(progress: progress, color: color)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoLinearActivityIndicator__activity_indicator : CustomPainter
{
    public virtual double progress { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    internal virtual Paint _backgroundPaint { get; private set; } = default!;
    internal virtual Paint _progressPaint { get; private set; } = default!;

    internal _CupertinoLinearActivityIndicator__activity_indicator(double progress, Color? color = null)
    {
        this.progress = progress;
        this.color = color;
        _backgroundPaint = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.color = CupertinoColors.systemFill;
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
        _progressPaint = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.color = color ?? CupertinoColors.activeBlue;
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
    }

    public override void paint(Canvas canvas, Size size)
    {
        canvas.drawRRect(BorderRadius.CreateAll(Radius.circular(size.height / 2L)).toRRect(Offset.zero & size), _backgroundPaint);
        if (progress > 0L)
        {
            canvas.drawRRect(BorderRadius.CreateAll(Radius.circular(size.height / 2L)).toRRect(Offset.zero & new Size(Dart_uiLibrary.clampDouble(progress, 0.0, 1.0) * size.width, size.height)), _progressPaint);
        }
    }

    public override bool shouldRepaint(CustomPainter oldDelegate) => (((_CupertinoLinearActivityIndicator__activity_indicator)oldDelegate).progress != progress) || (!Equals(((_CupertinoLinearActivityIndicator__activity_indicator)oldDelegate).color, color));
}
