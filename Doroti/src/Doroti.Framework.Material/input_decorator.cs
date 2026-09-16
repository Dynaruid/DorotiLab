// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/input_decorator.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Input_decoratorLibrary
{
    internal static Duration _kTransitionDuration = Duration.Create(milliseconds: 167L);
}

public static partial class Input_decoratorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kTransitionCurve = Curves.fastOutSlowIn;
}

public static partial class Input_decoratorLibrary
{
    internal static double _kFinalLabelScale = 0.75;
}

public static partial class Input_decoratorLibrary
{
    internal static double _kInputExtraPadding = 4.0;
}

public static partial class Input_decoratorLibrary
{
    internal static double _kSubtextCounterPadding = 16.0;
}

internal delegate void _SubtextSize__input_decorator();

internal delegate double _ChildBaselineGetter__input_decorator(global::Doroti.Framework.Rendering.RenderBox child, global::Doroti.Framework.Rendering.BoxConstraints constraints);

public static partial class Input_decoratorLibrary
{
    internal static Duration _kHintFadeTransitionDuration = Duration.Create(milliseconds: 20L);
}

public class _InputBorderGap__input_decorator : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual double? _start { get; set; } = default;
    internal virtual double _extent { get; set; } = 0.0;

    public virtual double? start
    {
        get => _start;
        set
        {
            var __value = value;
            if (__value != _start)
            {
                _start = __value;
                notifyListeners();
            }
        }
    }
    public virtual double extent
    {
        get => _extent;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) != _extent)
            {
                _extent = DartRuntimePrimitives.RequireValue(__value);
                notifyListeners();
            }
        }
    }
    public override bool Equals(object? other)
    {
        var __other = other as _InputBorderGap__input_decorator;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _InputBorderGap__input_decorator) && (__other.start == start) && (__other.extent == extent);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(start, extent));
    public override string ToString() => DiagnosticsLibrary.describeIdentity(this);
}

public class _InputBorderTween__input_decorator : global::Doroti.Framework.Animation.Tween<InputBorder>
{
    internal _InputBorderTween__input_decorator(InputBorder? begin = null, InputBorder? end = null) : base(begin: begin, end: end)
    {
    }

    public override InputBorder lerp(double t) => ((InputBorder?)ShapeBorder.lerp(begin, end, t)!)!;
}

internal class _InputBorderPainter__input_decorator : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual global::Doroti.Framework.Animation.Animation<double> borderAnimation { get; private set; } = default!;
    public virtual _InputBorderTween__input_decorator border { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> gapAnimation { get; private set; } = default!;
    public virtual _InputBorderGap__input_decorator gap { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual Color fillColor { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.ColorTween hoverColorTween { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> hoverAnimation { get; private set; } = default!;

    internal _InputBorderPainter__input_decorator(global::Doroti.Framework.Foundation.Listenable repaint, global::Doroti.Framework.Animation.Animation<double> borderAnimation, _InputBorderTween__input_decorator border, global::Doroti.Framework.Animation.Animation<double> gapAnimation, _InputBorderGap__input_decorator gap, TextDirection textDirection, Color fillColor, global::Doroti.Framework.Animation.Animation<double> hoverAnimation, global::Doroti.Framework.Animation.ColorTween hoverColorTween) : base(repaint: repaint)
    {
        this.borderAnimation = borderAnimation;
        this.border = border;
        this.gapAnimation = gapAnimation;
        this.gap = gap;
        this.textDirection = textDirection;
        this.fillColor = fillColor;
        this.hoverAnimation = hoverAnimation;
        this.hoverColorTween = hoverColorTween;
    }

    public virtual global::Doroti.Ui.Color blendedColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Dart_uiLibrary.Color.alphaBlend(hoverColorTween.evaluate(hoverAnimation)!, fillColor));
    public override void paint(Canvas canvas, Size size)
    {
        InputBorder borderValue = border.evaluate(borderAnimation);
        global::Doroti.Ui.Rect canvasRect = Offset.zero & size;
        global::Doroti.Ui.Color blendedFillColor = blendedColor;
        if (blendedFillColor.alpha > 0L)
        {
            var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = blendedFillColor;
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
            if (borderValue.preferPaintInterior)
            {
                borderValue.paintInterior(canvas, canvasRect, paintLocal, textDirection: textDirection);
            }
            else
            {
                canvas.drawPath(borderValue.getOuterPath(canvasRect, textDirection: textDirection), paintLocal);
            }
        }
        borderValue.paint(canvas, canvasRect, gapStart: gap.start ?? 0.0, gapExtent: gap.extent, gapPercentage: gapAnimation.value, textDirection: textDirection);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_InputBorderPainter__input_decorator)oldDelegate;
        return (!Equals(borderAnimation, __oldPainter.borderAnimation)) || (!Equals(hoverAnimation, __oldPainter.hoverAnimation)) || (!Equals(gapAnimation, __oldPainter.gapAnimation)) || (!Equals(border, __oldPainter.border)) || (!Equals(gap, __oldPainter.gap)) || (!Equals(textDirection, __oldPainter.textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => DiagnosticsLibrary.describeIdentity(this);
}

public class _BorderContainer__input_decorator : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual InputBorder border { get; private set; } = default!;
    public virtual _InputBorderGap__input_decorator gap { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> gapAnimation { get; private set; } = default!;
    public virtual Color fillColor { get; private set; } = default!;
    public virtual Color hoverColor { get; private set; } = default!;
    public virtual bool isHovering { get; private set; } = default!;

    internal _BorderContainer__input_decorator(InputBorder border, _InputBorderGap__input_decorator gap, global::Doroti.Framework.Animation.Animation<double> gapAnimation, Color fillColor, Color hoverColor, bool isHovering)
    {
        this.border = border;
        this.gap = gap;
        this.gapAnimation = gapAnimation;
        this.fillColor = fillColor;
        this.hoverColor = hoverColor;
        this.isHovering = isHovering;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _BorderContainerState__input_decorator());
}

public class _BorderContainerState__input_decorator : global::Doroti.Framework.Widgets.State<_BorderContainer__input_decorator>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<_BorderContainer__input_decorator>
{
    internal static Duration _kHoverDuration = Duration.Create(milliseconds: 15L);
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _hoverColorController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _borderAnimation { get; set; } = default!;
    internal virtual _InputBorderTween__input_decorator _border { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _hoverAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.ColorTween _hoverColorTween { get; set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _hoverColorController = new global::Doroti.Framework.Animation.AnimationController(duration: _kHoverDuration, value: widget.isHovering ? 1.0 : 0.0, vsync: this);
        _controller = new global::Doroti.Framework.Animation.AnimationController(duration: Input_decoratorLibrary._kTransitionDuration, vsync: this);
        _borderAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _controller, curve: Input_decoratorLibrary._kTransitionCurve, reverseCurve: Input_decoratorLibrary._kTransitionCurve.flipped);
        _border = new _InputBorderTween__input_decorator(begin: widget.border, end: widget.border);
        _hoverAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _hoverColorController, curve: Curves.linear);
        _hoverColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: widget.hoverColor.withAlpha(0L), end: widget.hoverColor);
    }

    public override void dispose()
    {
        _controller.dispose();
        _hoverColorController.dispose();
        _borderAnimation.dispose();
        _hoverAnimation.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(_BorderContainer__input_decorator oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.border, oldWidget.border))
        {
            _border = new _InputBorderTween__input_decorator(begin: oldWidget.border, end: widget.border);
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = _controller;
    __cascade.value = 0.0;
    __cascade.forward();
    return __cascade;
}))());
        }
        if (!Equals(widget.hoverColor, oldWidget.hoverColor))
        {
            _hoverColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: widget.hoverColor.withAlpha(0L), end: widget.hoverColor);
        }
        if (widget.isHovering != oldWidget.isHovering)
        {
            if (widget.isHovering)
            {
                _hoverColorController.forward();
            }
            else
            {
                _hoverColorController.reverse();
            }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.CustomPaint(foregroundPainter: new _InputBorderPainter__input_decorator(repaint: Listenable.CreateMerge(new List<global::Doroti.Framework.Foundation.Listenable> { _borderAnimation, widget.gap, _hoverColorController }.Cast<global::Doroti.Framework.Foundation.Listenable?>()), borderAnimation: _borderAnimation, border: _border, gapAnimation: widget.gapAnimation, gap: widget.gap, textDirection: Directionality.of(context), fillColor: widget.fillColor, hoverColorTween: _hoverColorTween, hoverAnimation: _hoverAnimation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

public class _HelperError__input_decorator : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual TextAlign? textAlign { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? helper { get; private set; }
    public virtual string? helperText { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? helperStyle { get; private set; }
    public virtual long? helperMaxLines { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? error { get; private set; }
    public virtual string? errorText { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? errorStyle { get; private set; }
    public virtual long? errorMaxLines { get; private set; }

    internal _HelperError__input_decorator(TextAlign? textAlign = null, global::Doroti.Framework.Widgets.Widget? helper = null, string? helperText = null, global::Doroti.Framework.Painting.TextStyle? helperStyle = null, long? helperMaxLines = null, global::Doroti.Framework.Widgets.Widget? error = null, string? errorText = null, global::Doroti.Framework.Painting.TextStyle? errorStyle = null, long? errorMaxLines = null)
    {
        this.textAlign = textAlign;
        this.helper = helper;
        this.helperText = helperText;
        this.helperStyle = helperStyle;
        this.helperMaxLines = helperMaxLines;
        this.error = error;
        this.errorText = errorText;
        this.errorStyle = errorStyle;
        this.errorMaxLines = errorMaxLines;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _HelperErrorState__input_decorator());
}

public class _HelperErrorState__input_decorator : global::Doroti.Framework.Widgets.State<_HelperError__input_decorator>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<_HelperError__input_decorator>
{
    public static global::Doroti.Framework.Widgets.Widget empty = SizedBox.CreateShrink();
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.Widget? _helper { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.Widget? _error { get; set; } = default;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual bool _hasHelper => DartRuntimePrimitives.ConvertValue<bool>((widget.helperText is not null) || (widget.helper is not null));
    internal virtual bool _hasError => DartRuntimePrimitives.ConvertValue<bool>((widget.errorText is not null) || (widget.error is not null));
    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(duration: Input_decoratorLibrary._kTransitionDuration, vsync: this);
        if (_hasError)
        {
            _error = _buildError();
            _controller.value = 1.0;
        }
        else
        {
            if (_hasHelper)
            {
                _helper = _buildHelper();
            }
        }
        _controller.addListener(_handleChange);
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
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _handleChange()
    {
        setState(() =>
        {
        });
    }

    public override void didUpdateWidget(_HelperError__input_decorator old)
    {
        base.didUpdateWidget(old);
        global::Doroti.Framework.Widgets.Widget? newError = widget.error;
        string? newErrorText = widget.errorText;
        global::Doroti.Framework.Widgets.Widget? newHelper = widget.helper;
        string? newHelperText = widget.helperText;
        global::Doroti.Framework.Widgets.Widget? oldError = old.error;
        string? oldErrorText = old.errorText;
        global::Doroti.Framework.Widgets.Widget? oldHelper = old.helper;
        string? oldHelperText = old.helperText;
        var errorStateChanged = newError is not null != oldError is not null;
        var errorTextStateChanged = newErrorText is not null != oldErrorText is not null;
        var helperStateChanged = newHelper is not null != oldHelper is not null;
        bool helperTextStateChanged = (newErrorText is null) && (newHelperText is not null != oldHelperText is not null);
        if (errorStateChanged || errorTextStateChanged || helperStateChanged || helperTextStateChanged)
        {
            if ((newError is not null) || (newErrorText is not null))
            {
                _error = _buildError();
                _controller.forward();
            }
            else
            {
                if ((newHelper is not null) || (newHelperText is not null))
                {
                    _helper = _buildHelper();
                    _controller.reverse();
                }
                else
                {
                    _controller.reverse();
                }
            }
        }
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildHelper()
    {
        DartRuntimePrimitives.Assert(() => (widget.helper is not null) || (widget.helperText is not null));
        return new global::Doroti.Framework.Widgets.Semantics(container: true, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).animate(_controller), child: widget.helper ?? new global::Doroti.Framework.Widgets.Text(widget.helperText!, style: widget.helperStyle, textAlign: widget.textAlign, overflow: TextOverflow.ellipsis, maxLines: widget.helperMaxLines)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildError()
    {
        DartRuntimePrimitives.Assert(() => (widget.error is not null) || (widget.errorText is not null));
        string? capturedErrorText = widget.errorText;
        global::Doroti.Framework.Widgets.Widget? capturedError = widget.error;
        if ((capturedError is not null) && (widget.errorStyle is not null))
        {
            capturedError = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: widget.errorStyle!, child: capturedError));
        }
        return new global::Doroti.Framework.Widgets.Builder(builder: (context) =>
        {
            return new global::Doroti.Framework.Widgets.Semantics(container: true, liveRegion: !MediaQuery.supportsAnnounceOf(context), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: _controller, child: new global::Doroti.Framework.Widgets.FractionalTranslation(translation: new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.0, -0.25), end: Offset.zero).evaluate(_controller.view), child: capturedError ?? new global::Doroti.Framework.Widgets.Text(capturedErrorText!, style: widget.errorStyle, textAlign: widget.textAlign, overflow: TextOverflow.ellipsis, maxLines: widget.errorMaxLines))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (_controller.isDismissed)
        {
            _error = null;
            if (_hasHelper)
            {
                return _helper = _buildHelper();
            }
            else
            {
                _helper = null;
                return empty;
            }
        }
        if (_controller.isCompleted)
        {
            _helper = null;
            if (_hasError)
            {
                return _error = _buildError();
            }
            else
            {
                _error = null;
                return empty;
            }
        }
        if ((_helper is null) && _hasError)
        {
            return _buildError();
        }
        if ((_error is null) && _hasHelper)
        {
            return _buildHelper();
        }
        if (_hasError)
        {
            return new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).animate(_controller), child: _helper)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildError()) });
        }
        if (_hasHelper)
        {
            return new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildHelper()), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: _controller, child: _error)) });
        }
        return empty;
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

public enum FloatingLabelBehavior
{
    never,
    auto,
    always
}

public class FloatingLabelAlignment
{
    internal virtual double _x { get; private set; } = default!;
    public static FloatingLabelAlignment start = new FloatingLabelAlignment(-1.0);
    public static FloatingLabelAlignment center = new FloatingLabelAlignment(0.0);

    public FloatingLabelAlignment(double _x)
    {
        this._x = _x;
        System.Diagnostics.Debug.Assert((_x >= -1.0) && (_x <= 1.0));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(_x.GetHashCode());
    public override bool Equals(object? other)
    {
        var __other = other as FloatingLabelAlignment;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is FloatingLabelAlignment) && (_x == __other._x);
    }

    internal static string _stringify(double x)
    {
        return x switch { -1.0 => "FloatingLabelAlignment.start",0.0 => "FloatingLabelAlignment.center",_ => $"FloatingLabelAlignment(x: {x.toStringAsFixed(1L)})" };
    }

    public override string ToString() => _stringify(_x);
}

public enum _DecorationSlot__input_decorator
{
    icon,
    input,
    label,
    hint,
    prefix,
    suffix,
    prefixIcon,
    suffixIcon,
    helperError,
    counter,
    container
}

public class _Decoration__input_decorator
{
    public virtual global::Doroti.Framework.Painting.EdgeInsetsDirectional contentPadding { get; private set; } = default!;
    public virtual bool isCollapsed { get; private set; } = default!;
    public virtual double floatingLabelHeight { get; private set; } = default!;
    public virtual double floatingLabelProgress { get; private set; } = default!;
    public virtual FloatingLabelAlignment floatingLabelAlignment { get; private set; } = default!;
    public virtual InputBorder border { get; private set; } = default!;
    public virtual _InputBorderGap__input_decorator borderGap { get; private set; } = default!;
    public virtual bool alignLabelWithHint { get; private set; } = default!;
    public virtual bool? isDense { get; private set; }
    public virtual bool isEmpty { get; private set; } = default!;
    public virtual VisualDensity visualDensity { get; private set; } = default!;
    public virtual double inputGap { get; private set; } = default!;
    public virtual bool maintainHintSize { get; private set; } = default!;
    public virtual bool maintainLabelSize { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? icon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? input { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? label { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? hint { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? prefix { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? suffix { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? prefixIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? suffixIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? helperError { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? counter { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? container { get; private set; }

    internal _Decoration__input_decorator(global::Doroti.Framework.Painting.EdgeInsetsDirectional contentPadding, bool isCollapsed, double floatingLabelHeight, double floatingLabelProgress, FloatingLabelAlignment floatingLabelAlignment, InputBorder border, _InputBorderGap__input_decorator borderGap, bool alignLabelWithHint, bool? isDense, bool isEmpty, VisualDensity visualDensity, double inputGap, bool maintainHintSize, bool maintainLabelSize, global::Doroti.Framework.Widgets.Widget? icon = null, global::Doroti.Framework.Widgets.Widget? input = null, global::Doroti.Framework.Widgets.Widget? label = null, global::Doroti.Framework.Widgets.Widget? hint = null, global::Doroti.Framework.Widgets.Widget? prefix = null, global::Doroti.Framework.Widgets.Widget? suffix = null, global::Doroti.Framework.Widgets.Widget? prefixIcon = null, global::Doroti.Framework.Widgets.Widget? suffixIcon = null, global::Doroti.Framework.Widgets.Widget? helperError = null, global::Doroti.Framework.Widgets.Widget? counter = null, global::Doroti.Framework.Widgets.Widget? container = null)
    {
        this.contentPadding = contentPadding;
        this.isCollapsed = isCollapsed;
        this.floatingLabelHeight = floatingLabelHeight;
        this.floatingLabelProgress = floatingLabelProgress;
        this.floatingLabelAlignment = floatingLabelAlignment;
        this.border = border;
        this.borderGap = borderGap;
        this.alignLabelWithHint = alignLabelWithHint;
        this.isDense = isDense;
        this.isEmpty = isEmpty;
        this.visualDensity = visualDensity;
        this.inputGap = inputGap;
        this.maintainHintSize = maintainHintSize;
        this.maintainLabelSize = maintainLabelSize;
        this.icon = icon;
        this.input = input;
        this.label = label;
        this.hint = hint;
        this.prefix = prefix;
        this.suffix = suffix;
        this.prefixIcon = prefixIcon;
        this.suffixIcon = suffixIcon;
        this.helperError = helperError;
        this.counter = counter;
        this.container = container;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _Decoration__input_decorator;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _Decoration__input_decorator) && Equals(__other.contentPadding, contentPadding) && (__other.isCollapsed == isCollapsed) && (__other.floatingLabelHeight == floatingLabelHeight) && (__other.floatingLabelProgress == floatingLabelProgress) && Equals(__other.floatingLabelAlignment, floatingLabelAlignment) && Equals(__other.border, border) && Equals(__other.borderGap, borderGap) && (__other.alignLabelWithHint == alignLabelWithHint) && (__other.isDense == isDense) && (__other.isEmpty == isEmpty) && Equals(__other.visualDensity, visualDensity) && (__other.inputGap == inputGap) && (__other.maintainHintSize == maintainHintSize) && (__other.maintainLabelSize == maintainLabelSize) && Equals(__other.icon, icon) && Equals(__other.input, input) && Equals(__other.label, label) && Equals(__other.hint, hint) && Equals(__other.prefix, prefix) && Equals(__other.suffix, suffix) && Equals(__other.prefixIcon, prefixIcon) && Equals(__other.suffixIcon, suffixIcon) && Equals(__other.helperError, helperError) && Equals(__other.counter, counter) && Equals(__other.container, container);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(contentPadding, floatingLabelHeight, floatingLabelProgress, floatingLabelAlignment, border, borderGap, alignLabelWithHint, isDense, isEmpty, visualDensity, inputGap, maintainHintSize, maintainLabelSize, icon, input, label, hint, prefix, suffix, FoundationRuntimePorts.ObjectHash(prefixIcon, suffixIcon, helperError, counter, container)));
}

internal class _RenderDecorationLayout__input_decorator
{
    public virtual global::Doroti.Framework.Rendering.BoxConstraints inputConstraints { get; private set; } = default!;
    public virtual double baseline { get; private set; } = default!;
    public virtual double containerHeight { get; private set; } = default!;
    public virtual (double ascent, double bottomHeight, double subtextHeight)? subtextSize { get; private set; }
    public virtual Size size { get; private set; } = default!;

    internal _RenderDecorationLayout__input_decorator(global::Doroti.Framework.Rendering.BoxConstraints inputConstraints, double baseline, double containerHeight, (double ascent, double bottomHeight, double subtextHeight)? subtextSize, Size size)
    {
        this.inputConstraints = inputConstraints;
        this.baseline = baseline;
        this.containerHeight = containerHeight;
        this.subtextSize = subtextSize;
        this.size = size;
    }

}

public class _RenderDecoration__input_decorator : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Widgets.SlottedContainerRenderObjectMixin<_DecorationSlot__input_decorator, global::Doroti.Framework.Rendering.RenderBox>
{
    internal virtual _Decoration__input_decorator _decoration { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual TextBaseline _textBaseline { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.TextAlignVertical? _textAlignVertical { get; set; } = default;
    internal virtual bool _isFocused { get; set; } = default!;
    internal virtual bool _expands { get; set; } = false;
    internal virtual Matrix4? _labelTransform { get; set; } = default;
    public virtual DartMap<_DecorationSlot__input_decorator, global::Doroti.Framework.Rendering.RenderBox> _slotToChild { get; set; } = new DartMap<_DecorationSlot__input_decorator, global::Doroti.Framework.Rendering.RenderBox>();

    internal _RenderDecoration__input_decorator(_Decoration__input_decorator decoration, TextDirection textDirection, TextBaseline textBaseline, bool isFocused, bool expands, global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical = null)
    {
        _decoration = decoration;
        _textDirection = textDirection;
        _textBaseline = textBaseline;
        _textAlignVertical = textAlignVertical;
        _isFocused = isFocused;
        _expands = expands;
    }

    public virtual double subtextGap => 4.0;
    public virtual double prefixToInputGap => 4.0;
    public virtual double inputToSuffixGap => 4.0;
    public virtual global::Doroti.Framework.Rendering.RenderBox? icon => childForSlot(DartRuntimePrimitives.RequireValue(_DecorationSlot__input_decorator.icon));
    public virtual global::Doroti.Framework.Rendering.RenderBox? input => childForSlot(DartRuntimePrimitives.RequireValue(_DecorationSlot__input_decorator.input));
    public virtual global::Doroti.Framework.Rendering.RenderBox? label => childForSlot(DartRuntimePrimitives.RequireValue(_DecorationSlot__input_decorator.label));
    public virtual global::Doroti.Framework.Rendering.RenderBox? hint => childForSlot(DartRuntimePrimitives.RequireValue(_DecorationSlot__input_decorator.hint));
    public virtual global::Doroti.Framework.Rendering.RenderBox? prefix => childForSlot(DartRuntimePrimitives.RequireValue(_DecorationSlot__input_decorator.prefix));
    public virtual global::Doroti.Framework.Rendering.RenderBox? suffix => childForSlot(DartRuntimePrimitives.RequireValue(_DecorationSlot__input_decorator.suffix));
    public virtual global::Doroti.Framework.Rendering.RenderBox? prefixIcon => childForSlot(DartRuntimePrimitives.RequireValue(_DecorationSlot__input_decorator.prefixIcon));
    public virtual global::Doroti.Framework.Rendering.RenderBox? suffixIcon => childForSlot(DartRuntimePrimitives.RequireValue(_DecorationSlot__input_decorator.suffixIcon));
    public virtual global::Doroti.Framework.Rendering.RenderBox helperError => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderBox>(childForSlot(_DecorationSlot__input_decorator.helperError)!);
    public virtual global::Doroti.Framework.Rendering.RenderBox? counter => childForSlot(DartRuntimePrimitives.RequireValue(_DecorationSlot__input_decorator.counter));
    public virtual global::Doroti.Framework.Rendering.RenderBox? container => childForSlot(DartRuntimePrimitives.RequireValue(_DecorationSlot__input_decorator.container));
    public virtual IEnumerable<global::Doroti.Framework.Rendering.RenderBox> children
    {
        get
        {
            global::Doroti.Framework.Rendering.RenderBox? helperErrorLocal = childForSlot(_DecorationSlot__input_decorator.helperError);
            return ((Func<List<global::Doroti.Framework.Rendering.RenderBox>>)(() => { var __collection23257 = new List<global::Doroti.Framework.Rendering.RenderBox>(); var __collectionElement23276 = icon; if (__collectionElement23276 is { } __nonNullCollectionElement23276) { __collection23257.Add(__nonNullCollectionElement23276); } var __collectionElement23289 = input; if (__collectionElement23289 is { } __nonNullCollectionElement23289) { __collection23257.Add(__nonNullCollectionElement23289); } var __collectionElement23303 = prefixIcon; if (__collectionElement23303 is { } __nonNullCollectionElement23303) { __collection23257.Add(__nonNullCollectionElement23303); } var __collectionElement23322 = suffixIcon; if (__collectionElement23322 is { } __nonNullCollectionElement23322) { __collection23257.Add(__nonNullCollectionElement23322); } var __collectionElement23341 = prefix; if (__collectionElement23341 is { } __nonNullCollectionElement23341) { __collection23257.Add(__nonNullCollectionElement23341); } var __collectionElement23356 = suffix; if (__collectionElement23356 is { } __nonNullCollectionElement23356) { __collection23257.Add(__nonNullCollectionElement23356); } var __collectionElement23371 = label; if (__collectionElement23371 is { } __nonNullCollectionElement23371) { __collection23257.Add(__nonNullCollectionElement23371); } var __collectionElement23385 = hint; if (__collectionElement23385 is { } __nonNullCollectionElement23385) { __collection23257.Add(__nonNullCollectionElement23385); } var __collectionElement23398 = helperErrorLocal; if (__collectionElement23398 is { } __nonNullCollectionElement23398) { __collection23257.Add(__nonNullCollectionElement23398); } var __collectionElement23418 = counter; if (__collectionElement23418 is { } __nonNullCollectionElement23418) { __collection23257.Add(__nonNullCollectionElement23418); } var __collectionElement23434 = container; if (__collectionElement23434 is { } __nonNullCollectionElement23434) { __collection23257.Add(__nonNullCollectionElement23434); } return __collection23257; }))();
        }
    }
    public virtual _Decoration__input_decorator decoration
    {
        get => _decoration;
        set
        {
            var __value = value;
            if (Equals(_decoration, __value))
            {
                return;
            }
            _decoration = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextBaseline textBaseline
    {
        get => _textBaseline;
        set
        {
            var __value = value;
            if (Equals(_textBaseline, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _textBaseline = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    internal virtual global::Doroti.Framework.Painting.TextAlignVertical _defaultTextAlignVertical => _isOutlineAligned ? TextAlignVertical.center : TextAlignVertical.top;
    [global::System.Diagnostics.CodeAnalysis.AllowNull]
    public virtual global::Doroti.Framework.Painting.TextAlignVertical textAlignVertical
    {
        get => _textAlignVertical ?? _defaultTextAlignVertical;
        set
        {
            global::Doroti.Framework.Painting.TextAlignVertical? __value = value;
            if (Equals(_textAlignVertical, __value))
            {
                return;
            }
            if (textAlignVertical.y == (__value?.y ?? _defaultTextAlignVertical.y))
            {
                _textAlignVertical = __value;
                return;
            }
            _textAlignVertical = __value;
            markNeedsLayout();
        }
    }
    public virtual bool isFocused
    {
        get => _isFocused;
        set
        {
            var __value = value;
            if (_isFocused == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _isFocused = DartRuntimePrimitives.RequireValue(__value);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool expands
    {
        get => _expands;
        set
        {
            var __value = value;
            if (_expands == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _expands = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    internal virtual bool _isOutlineAligned
    {
        get
        {
            return !decoration.isCollapsed && decoration.border.isOutline;
        }
    }
    internal virtual global::Doroti.Ui.Offset _densityOffset => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(decoration.visualDensity.baseSizeAdjustment);
    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        if (icon is not null)
        {
            visitor(icon!);
        }
        if (prefix is not null)
        {
            visitor(prefix!);
        }
        if (prefixIcon is not null)
        {
            visitor(prefixIcon!);
        }
        if (label is not null)
        {
            visitor(label!);
        }
        if (hint is not null)
        {
            if (isFocused)
            {
                visitor(hint!);
            }
            else
            {
                if (label is null)
                {
                    visitor(hint!);
                }
            }
        }
        if (input is not null)
        {
            visitor(input!);
        }
        if (suffixIcon is not null)
        {
            visitor(suffixIcon!);
        }
        if (suffix is not null)
        {
            visitor(suffix!);
        }
        if (container is not null)
        {
            visitor(container!);
        }
        visitor(helperError);
        if (counter is not null)
        {
            visitor(counter!);
        }
    }

    internal static double _minWidth(global::Doroti.Framework.Rendering.RenderBox? box, double height) => DartRuntimePrimitives.ConvertValue<double>(box?.getMinIntrinsicWidth(height) ?? 0.0);
    internal static double _maxWidth(global::Doroti.Framework.Rendering.RenderBox? box, double height) => DartRuntimePrimitives.ConvertValue<double>(box?.getMaxIntrinsicWidth(height) ?? 0.0);
    internal static double _minHeight(global::Doroti.Framework.Rendering.RenderBox? box, double width) => DartRuntimePrimitives.ConvertValue<double>(box?.getMinIntrinsicHeight(width) ?? 0.0);
    internal static global::Doroti.Ui.Size _boxSize(global::Doroti.Framework.Rendering.RenderBox? box) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Size>(box?.size ?? Size.zero);
    internal static double _getBaseline(global::Doroti.Framework.Rendering.RenderBox box, global::Doroti.Framework.Rendering.BoxConstraints boxConstraints)
    {
        return ChildLayoutHelper.getBaseline(box, boxConstraints, TextBaseline.alphabetic) ?? box.size.height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _getDryBaseline(global::Doroti.Framework.Rendering.RenderBox box, global::Doroti.Framework.Rendering.BoxConstraints boxConstraints)
    {
        return ChildLayoutHelper.getDryBaseline(box, boxConstraints, TextBaseline.alphabetic) ?? ChildLayoutHelper.dryLayoutChild(box, boxConstraints).height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Rendering.BoxParentData _boxParentData(global::Doroti.Framework.Rendering.RenderBox box) => ((global::Doroti.Framework.Rendering.BoxParentData?)box.parentData!)!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsDirectional contentPadding => decoration.contentPadding;
    internal virtual (double ascent, double bottomHeight, double subtextHeight)? _computeSubtextSizes(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, double> getBaseline)
    {
        var (counterSize, counterAscent) = counter switch { global::Doroti.Framework.Rendering.RenderBox box => ((Size, double))(layoutChild(box, constraints), getBaseline(box, constraints)),null => ((Size, double))(Size.zero, 0.0) };
        double counterPadding = (counter is not null) ? Input_decoratorLibrary._kSubtextCounterPadding : 0.0;
        global::Doroti.Framework.Rendering.BoxConstraints helperErrorConstraints = constraints.deflate(EdgeInsets.CreateOnly(left: counterSize.width + counterPadding));
        double helperErrorHeight = layoutChild(helperError, helperErrorConstraints).height;
        if ((helperErrorHeight == 0.0) && (counterSize.height == 0.0))
        {
            return null;
        }
        double ascentLocal = Math.Max(counterAscent, getBaseline(helperError, helperErrorConstraints)) + subtextGap;
        double bottomHeightLocal = Math.Max(counterAscent, helperErrorHeight) + subtextGap;
        double subtextHeightLocal = Math.Max(counterSize.height, helperErrorHeight) + subtextGap;
        return (ascent: ascentLocal, bottomHeight: bottomHeightLocal, subtextHeight: subtextHeightLocal);
    }

    internal virtual _RenderDecorationLayout__input_decorator _layout(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, double> getBaseline)
    {
        DartRuntimePrimitives.Assert(() => constraints.maxWidth < double.PositiveInfinity, () => (object?)"An InputDecorator, which is typically created by a TextField, cannot " + "have an unbounded width.\n" + "This happens when the parent widget does not provide a finite width " + "constraint. For example, if the InputDecorator is contained by a Row, " + "then its width must be constrained. An Expanded widget or a SizedBox " + "can be used to constrain the width of the InputDecorator or the " + "TextField that contains it.");
        global::Doroti.Framework.Rendering.BoxConstraints boxConstraints = constraints.loosen();
        global::Doroti.Framework.Rendering.RenderBox? iconLocal = icon;
        double iconWidth = (iconLocal is null) ? 0.0 : layoutChild(iconLocal, boxConstraints).width;
        global::Doroti.Framework.Rendering.BoxConstraints containerConstraints = boxConstraints.deflate(EdgeInsets.CreateOnly(left: iconWidth));
        global::Doroti.Framework.Rendering.BoxConstraints contentConstraints = containerConstraints.deflate(EdgeInsetsDirectional.CreateOnly(start: contentPadding.start + decoration.inputGap, end: contentPadding.end + decoration.inputGap));
        (double ascent, double bottomHeight, double subtextHeight)? subtextSizeLocal = _computeSubtextSizes(constraints: contentConstraints, layoutChild: layoutChild, getBaseline: getBaseline);
        global::Doroti.Framework.Rendering.RenderBox? prefixIconLocal = prefixIcon;
        global::Doroti.Framework.Rendering.RenderBox? suffixIconLocal = suffixIcon;
        global::Doroti.Ui.Size prefixIconSize = (prefixIconLocal is null) ? Size.zero : layoutChild(prefixIconLocal, containerConstraints);
        global::Doroti.Ui.Size suffixIconSize = (suffixIconLocal is null) ? Size.zero : layoutChild(suffixIconLocal, containerConstraints);
        global::Doroti.Framework.Rendering.RenderBox? prefixLocal = prefix;
        global::Doroti.Framework.Rendering.RenderBox? suffixLocal = suffix;
        global::Doroti.Ui.Size prefixSize = (prefixLocal is null) ? Size.zero : layoutChild(prefixLocal, contentConstraints);
        global::Doroti.Ui.Size suffixSize = (suffixLocal is null) ? Size.zero : layoutChild(suffixLocal, contentConstraints);
        var accessoryHorizontalInsets = EdgeInsetsDirectional.CreateOnly(start: iconWidth + prefixSize.width + ((prefixIconLocal is null) ? (contentPadding.start + decoration.inputGap) : (prefixIconSize.width + prefixToInputGap)), end: suffixSize.width + ((suffixIconLocal is null) ? (contentPadding.end + decoration.inputGap) : (suffixIconSize.width + inputToSuffixGap)));
        double inputWidth = Math.Max(0.0, constraints.maxWidth - accessoryHorizontalInsets.horizontal);
        global::Doroti.Framework.Rendering.RenderBox? labelLocal = label;
        double topHeight = default!;
        if (labelLocal is not null)
        {
            double suffixIconSpace = decoration.border.isOutline ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(suffixIconSize.width, contentPadding.end, decoration.floatingLabelProgress)) : suffixIconSize.width;
            double labelWidth = Math.Max(0.0, constraints.maxWidth - ((decoration.inputGap * 2L) + iconWidth + ((prefixIconLocal is null) ? contentPadding.start : prefixIconSize.width) + ((suffixIconLocal is null) ? contentPadding.end : suffixIconSpace)));
            double invertedLabelScale = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(1.0, 1L / Input_decoratorLibrary._kFinalLabelScale, decoration.floatingLabelProgress));
            global::Doroti.Framework.Rendering.BoxConstraints labelConstraints = boxConstraints.copyWith(maxWidth: labelWidth * invertedLabelScale);
            layoutChild(labelLocal, labelConstraints);
            double labelHeight = decoration.floatingLabelHeight;
            topHeight = decoration.border.isOutline ? Math.Max(labelHeight - getBaseline(labelLocal, labelConstraints), 0.0) : labelHeight;
        }
        else
        {
            topHeight = 0.0;
        }
        double bottomHeightLocal = subtextSizeLocal?.bottomHeight ?? 0.0;
        global::Doroti.Framework.Rendering.BoxConstraints inputConstraintsLocal = boxConstraints.deflate(EdgeInsets.CreateOnly(top: contentPadding.vertical + topHeight + bottomHeightLocal + _densityOffset.dy)).tighten(width: inputWidth);
        global::Doroti.Framework.Rendering.RenderBox? inputLocal = input;
        global::Doroti.Framework.Rendering.RenderBox? hintLocal = hint;
        global::Doroti.Ui.Size inputSize = (inputLocal is null) ? Size.zero : layoutChild(inputLocal, inputConstraintsLocal);
        global::Doroti.Ui.Size hintSize = (hintLocal is null) ? Size.zero : layoutChild(hintLocal, boxConstraints.tighten(width: inputWidth));
        double inputBaseline = (inputLocal is null) ? 0.0 : getBaseline(inputLocal, inputConstraintsLocal);
        double hintBaseline = (hintLocal is null) ? 0.0 : getBaseline(hintLocal, boxConstraints.tighten(width: inputWidth));
        double inputHeight = Math.Max((decoration.isEmpty || decoration.maintainHintSize) ? hintSize.height : 0.0, inputSize.height);
        double inputInternalBaseline = Math.Max(inputBaseline, hintBaseline);
        double prefixBaseline = (prefixLocal is null) ? 0.0 : getBaseline(prefixLocal, contentConstraints);
        double suffixBaseline = (suffixLocal is null) ? 0.0 : getBaseline(suffixLocal, contentConstraints);
        double fixHeight = Math.Max(prefixBaseline, suffixBaseline);
        double fixAboveInput = Math.Max(0, fixHeight - inputInternalBaseline);
        double fixBelowBaseline = Math.Max(prefixSize.height - prefixBaseline, suffixSize.height - suffixBaseline);
        double fixBelowInput = Math.Max(0, fixBelowBaseline - (inputHeight - inputInternalBaseline));
        double fixIconHeight = Math.Max(prefixIconSize.height, suffixIconSize.height);
        double contentHeight = Math.Max(fixIconHeight, topHeight + contentPadding.top + fixAboveInput + inputHeight + fixBelowInput + contentPadding.bottom + _densityOffset.dy);
        double minContainerHeight = (DartRuntimePrimitives.RequireValue(decoration.isDense) || decoration.isCollapsed || expands) ? inputHeight : ConstantsLibrary.kMinInteractiveDimension;
        double maxContainerHeight = Math.Max(0.0, boxConstraints.maxHeight - bottomHeightLocal);
        double containerHeightLocal = expands ? maxContainerHeight : Math.Min(Math.Max(contentHeight, minContainerHeight), maxContainerHeight);
        double interactiveAdjustment = (minContainerHeight > contentHeight) ? ((minContainerHeight - contentHeight) / 2.0) : 0.0;
        double overflow = Math.Max(0, contentHeight - maxContainerHeight);
        double textAlignVerticalFactor = (textAlignVertical.y + 1.0) / 2.0;
        double baselineAdjustment = fixAboveInput - (overflow * (1L - textAlignVerticalFactor));
        double topInputBaseline = contentPadding.top + topHeight + inputInternalBaseline + baselineAdjustment + interactiveAdjustment + (_densityOffset.dy / 2.0);
        double maxContentHeight = containerHeightLocal - contentPadding.vertical - topHeight - _densityOffset.dy;
        double alignableHeight = fixAboveInput + inputHeight + fixBelowInput;
        double maxVerticalOffset = maxContentHeight - alignableHeight;
        double baselineLocal = default!;
        if (_isOutlineAligned)
        {
            double outlineCenterBaseline = inputInternalBaseline + (baselineAdjustment / 2.0) + ((containerHeightLocal - inputHeight) / 2.0);
            var outlineTopBaseline = topInputBaseline;
            double outlineBottomBaseline = topInputBaseline + maxVerticalOffset;
            baselineLocal = _interpolateThree(outlineTopBaseline, outlineCenterBaseline, outlineBottomBaseline, textAlignVertical);
        }
        else
        {
            double textAlignVerticalOffset = maxVerticalOffset * textAlignVerticalFactor;
            baselineLocal = topInputBaseline + textAlignVerticalOffset;
        }
        return new _RenderDecorationLayout__input_decorator(inputConstraints: inputConstraintsLocal, containerHeight: containerHeightLocal, baseline: baselineLocal, subtextSize: subtextSizeLocal, size: new global::Doroti.Ui.Size(constraints.maxWidth, containerHeightLocal + (subtextSizeLocal?.subtextHeight ?? 0.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _interpolateThree(double begin, double middle, double end, global::Doroti.Framework.Painting.TextAlignVertical textAlignVertical)
    {
        double basis = (textAlignVertical.y <= 0L) ? Math.Max(middle - begin, 0) : Math.Max(end - middle, 0);
        return middle + (basis * textAlignVertical.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        double inputWidth = (decoration.isEmpty || decoration.maintainHintSize) ? Math.Max(_minWidth(input, height), _minWidth(hint, height)) : _minWidth(input, height);
        double contentWidth = decoration.maintainLabelSize ? Math.Max(inputWidth, _minWidth(label, height)) : inputWidth;
        return _minWidth(icon, height) + ((prefixIcon is not null) ? prefixToInputGap : (contentPadding.start + decoration.inputGap)) + _minWidth(prefixIcon, height) + _minWidth(prefix, height) + contentWidth + _minWidth(suffix, height) + _minWidth(suffixIcon, height) + ((suffixIcon is not null) ? inputToSuffixGap : (contentPadding.end + decoration.inputGap));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double inputWidth = (decoration.isEmpty || decoration.maintainHintSize) ? Math.Max(_maxWidth(input, height), _maxWidth(hint, height)) : _maxWidth(input, height);
        double contentWidth = decoration.maintainLabelSize ? Math.Max(inputWidth, _maxWidth(label, height)) : inputWidth;
        return _maxWidth(icon, height) + ((prefixIcon is not null) ? prefixToInputGap : (contentPadding.start + decoration.inputGap)) + _maxWidth(prefixIcon, height) + _maxWidth(prefix, height) + contentWidth + _maxWidth(suffix, height) + _maxWidth(suffixIcon, height) + ((suffixIcon is not null) ? inputToSuffixGap : (contentPadding.end + decoration.inputGap));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _lineHeight(double width, List<global::Doroti.Framework.Rendering.RenderBox?> boxes)
    {
        var height = 0.0;
        foreach (var box in boxes)
        {
            if (box is null)
            {
                continue;
            }
            height = Math.Max(_minHeight(box, width), height);
        }
        return height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double iconHeight = _minHeight(icon, width);
        double iconWidth = _minWidth(icon, iconHeight);
        width = Math.Max(width - iconWidth, 0.0);
        double prefixIconHeight = _minHeight(prefixIcon, width);
        double prefixIconWidth = _minWidth(prefixIcon, prefixIconHeight);
        double suffixIconHeight = _minHeight(suffixIcon, width);
        double suffixIconWidth = _minWidth(suffixIcon, suffixIconHeight);
        width = Math.Max(width - contentPadding.horizontal - (decoration.inputGap * 2L), 0.0);
        double counterHeight = _minHeight(counter, width);
        double counterWidth = _minWidth(counter, counterHeight);
        double counterPadding = (counter is not null) ? Input_decoratorLibrary._kSubtextCounterPadding : 0.0;
        double helperErrorAvailableWidth = Math.Max(width - counterWidth - counterPadding, 0.0);
        double helperErrorHeight = _minHeight(helperError, helperErrorAvailableWidth);
        double subtextHeight = Math.Max(counterHeight, helperErrorHeight);
        if (subtextHeight > 0.0)
        {
            subtextHeight += subtextGap;
        }
        double prefixHeight = _minHeight(prefix, width);
        double prefixWidth = _minWidth(prefix, prefixHeight);
        double suffixHeight = _minHeight(suffix, width);
        double suffixWidth = _minWidth(suffix, suffixHeight);
        double availableInputWidth = Math.Max(width - prefixWidth - suffixWidth - prefixIconWidth - suffixIconWidth, 0.0);
        double inputHeight = _lineHeight(availableInputWidth, ((Func<List<global::Doroti.Framework.Rendering.RenderBox?>>)(() => { var __collection44130 = new List<global::Doroti.Framework.Rendering.RenderBox?>(); __collection44130.Add(input); if (decoration.isEmpty) { __collection44130.Add(hint); } return __collection44130; }))());
        double inputMaxHeight = new List<double> { inputHeight, prefixHeight, suffixHeight }.reduce(Dart_mathLibrary.max);
        double contentHeight = contentPadding.top + ((label is null) ? 0.0 : decoration.floatingLabelHeight) + inputMaxHeight + contentPadding.bottom + _densityOffset.dy;
        double containerHeight = new List<double> { iconHeight, contentHeight, prefixIconHeight, suffixIconHeight }.reduce(Dart_mathLibrary.max);
        double minContainerHeight = (DartRuntimePrimitives.RequireValue(decoration.isDense) || expands) ? 0.0 : ConstantsLibrary.kMinInteractiveDimension;
        return Math.Max(containerHeight, minContainerHeight) + subtextHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return getMinIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? inputLocal = input;
        if (inputLocal is null)
        {
            return 0.0;
        }
        return _boxParentData(inputLocal).offset.dy + (inputLocal.getDistanceToActualBaseline(baseline) ?? inputLocal.size.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? inputLocal = input;
        if (inputLocal is null)
        {
            return 0.0;
        }
        _RenderDecorationLayout__input_decorator layout = _layout(constraints, layoutChild: ChildLayoutHelper.dryLayoutChild, getBaseline: _getDryBaseline);
        return (baseline switch { TextBaseline.alphabetic => 0.0,TextBaseline.ideographic => (inputLocal.getDryBaseline(layout.inputConstraints, TextBaseline.ideographic) ?? inputLocal.getDryLayout(layout.inputConstraints).height) - (inputLocal.getDryBaseline(layout.inputConstraints, TextBaseline.alphabetic) ?? inputLocal.getDryLayout(layout.inputConstraints).height),_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }) + layout.baseline;
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        _RenderDecorationLayout__input_decorator layout = _layout(constraints, layoutChild: ChildLayoutHelper.dryLayoutChild, getBaseline: _getDryBaseline);
        return constraints.constrain(layout.size);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraintsLocal = constraints;
        _labelTransform = null;
        _RenderDecorationLayout__input_decorator layoutLocal = _layout(constraintsLocal, layoutChild: ChildLayoutHelper.layoutChild, getBaseline: _getBaseline);
        size = constraintsLocal.constrain(layoutLocal.size);
        DartRuntimePrimitives.Assert(() => size.width == constraintsLocal.constrainWidth(layoutLocal.size.width));
        DartRuntimePrimitives.Assert(() => size.height == constraintsLocal.constrainHeight(layoutLocal.size.height));
        double overallWidth = layoutLocal.size.width;
        global::Doroti.Framework.Rendering.RenderBox? containerLocal = container;
        if (containerLocal is not null)
        {
            var containerConstraints = BoxConstraints.CreateTightFor(height: layoutLocal.containerHeight, width: overallWidth - _boxSize(icon).width);
            containerLocal.layout(containerConstraints, parentUsesSize: true);
            double xLocal = textDirection switch { TextDirection.rtl => 0.0,TextDirection.ltr => _boxSize(icon).width,_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            _boxParentData(containerLocal).offset = new global::Doroti.Ui.Offset(xLocal, 0.0);
        }
        double heightLocal = layoutLocal.containerHeight;
        double centerLayout(global::Doroti.Framework.Rendering.RenderBox box, double x)
        {
            _boxParentData(box).offset = new global::Doroti.Ui.Offset(x, (heightLocal - box.size.height) / 2.0);
            return box.size.width;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if (icon is not null)
        {
            double xAlternate = textDirection switch { TextDirection.rtl => overallWidth - icon!.size.width,TextDirection.ltr => 0.0,_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            centerLayout(icon!, xAlternate);
        }
        double subtextBaseline = (layoutLocal.subtextSize?.ascent ?? 0.0) + layoutLocal.containerHeight;
        global::Doroti.Framework.Rendering.RenderBox? counterLocal = counter;
        double helperErrorBaseline = DartRuntimePrimitives.RequireValue(helperError.getDistanceToBaseline(TextBaseline.alphabetic));
        double counterBaseline = counterLocal?.getDistanceToBaseline(TextBaseline.alphabetic) ?? 0.0;
        double startLocal = default!;
        double endLocal = default!;
        switch (textDirection)
        {
            case TextDirection.ltr:
                {
                    startLocal = contentPadding.start + _boxSize(icon).width;
                    endLocal = overallWidth - contentPadding.end;
                    _boxParentData(helperError).offset = new global::Doroti.Ui.Offset(startLocal + decoration.inputGap, subtextBaseline - helperErrorBaseline);
                    if (counterLocal is not null)
                    {
                        _boxParentData(counterLocal).offset = new global::Doroti.Ui.Offset(endLocal - counterLocal.size.width - decoration.inputGap, subtextBaseline - counterBaseline);
                    }
                    break;
                }
            case TextDirection.rtl:
                {
                    startLocal = overallWidth - contentPadding.start - _boxSize(icon).width;
                    endLocal = contentPadding.end;
                    _boxParentData(helperError).offset = new global::Doroti.Ui.Offset(startLocal - helperError.size.width - decoration.inputGap, subtextBaseline - helperErrorBaseline);
                    if (counterLocal is not null)
                    {
                        _boxParentData(counterLocal).offset = new global::Doroti.Ui.Offset(endLocal + decoration.inputGap, subtextBaseline - counterBaseline);
                    }
                    break;
                }
        }
        double baselineLocal = layoutLocal.baseline;
        double baselineLayout(global::Doroti.Framework.Rendering.RenderBox box, double x)
        {
            _boxParentData(box).offset = new global::Doroti.Ui.Offset(x, baselineLocal - DartRuntimePrimitives.RequireValue(box.getDistanceToBaseline(TextBaseline.alphabetic)));
            return box.size.width;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        switch (textDirection)
        {
            case TextDirection.rtl:
                {
                    if (prefixIcon is not null)
                    {
                        startLocal += contentPadding.start;
                        startLocal -= centerLayout(prefixIcon!, startLocal - prefixIcon!.size.width);
                        startLocal -= prefixToInputGap;
                    }
                    else
                    {
                        startLocal -= decoration.inputGap;
                    }
                    if (label is not null)
                    {
                        if (decoration.alignLabelWithHint)
                        {
                            baselineLayout(label!, startLocal - label!.size.width);
                        }
                        else
                        {
                            centerLayout(label!, startLocal - label!.size.width);
                        }
                    }
                    if (prefix is not null)
                    {
                        startLocal -= baselineLayout(prefix!, startLocal - prefix!.size.width);
                    }
                    if (input is not null)
                    {
                        baselineLayout(input!, startLocal - input!.size.width);
                    }
                    if (hint is not null)
                    {
                        baselineLayout(hint!, startLocal - hint!.size.width);
                    }
                    if (suffixIcon is not null)
                    {
                        endLocal -= contentPadding.end;
                        endLocal += centerLayout(suffixIcon!, endLocal);
                        endLocal += inputToSuffixGap;
                    }
                    else
                    {
                        endLocal += decoration.inputGap;
                    }
                    if (suffix is not null)
                    {
                        endLocal += baselineLayout(suffix!, endLocal);
                    }
                    break;
                }
            case TextDirection.ltr:
                {
                    if (prefixIcon is not null)
                    {
                        startLocal -= contentPadding.start;
                        startLocal += centerLayout(prefixIcon!, startLocal);
                        startLocal += prefixToInputGap;
                    }
                    else
                    {
                        startLocal += decoration.inputGap;
                    }
                    if (label is not null)
                    {
                        if (decoration.alignLabelWithHint)
                        {
                            baselineLayout(label!, startLocal);
                        }
                        else
                        {
                            centerLayout(label!, startLocal);
                        }
                    }
                    if (prefix is not null)
                    {
                        startLocal += baselineLayout(prefix!, startLocal);
                    }
                    if (input is not null)
                    {
                        baselineLayout(input!, startLocal);
                    }
                    if (hint is not null)
                    {
                        baselineLayout(hint!, startLocal);
                    }
                    if (suffixIcon is not null)
                    {
                        endLocal += contentPadding.end;
                        endLocal -= centerLayout(suffixIcon!, endLocal - suffixIcon!.size.width);
                        endLocal -= inputToSuffixGap;
                    }
                    else
                    {
                        endLocal -= decoration.inputGap;
                    }
                    if (suffix is not null)
                    {
                        endLocal -= baselineLayout(suffix!, endLocal - suffix!.size.width);
                    }
                    break;
                }
        }
        if (label is not null)
        {
            double labelX = _boxParentData(label!).offset.dx;
            double floatAlign = decoration.floatingLabelAlignment._x + 1L;
            double floatWidth = _boxSize(label).width * Input_decoratorLibrary._kFinalLabelScale;
            switch (textDirection)
            {
                case TextDirection.rtl:
                    {
                        var offsetToPrefixIcon = 0.0;
                        if ((prefixIcon is not null) && !decoration.alignLabelWithHint)
                        {
                            offsetToPrefixIcon = _boxSize(prefixIcon).width - contentPadding.end;
                        }
                        decoration.borderGap.start = Dart_uiLibrary.lerpDouble(labelX + _boxSize(label).width + offsetToPrefixIcon, (_boxSize(containerLocal).width / 2.0) + (floatWidth / 2.0), floatAlign);
                        break;
                    }
                case TextDirection.ltr:
                    {
                        var offsetToPrefixIconLocal = 0.0;
                        if ((prefixIcon is not null) && !decoration.alignLabelWithHint)
                        {
                            offsetToPrefixIconLocal = -_boxSize(prefixIcon).width + contentPadding.start;
                        }
                        decoration.borderGap.start = Dart_uiLibrary.lerpDouble(labelX - _boxSize(icon).width + offsetToPrefixIconLocal, (_boxSize(containerLocal).width / 2.0) - (floatWidth / 2.0), floatAlign);
                        break;
                    }
            }
            decoration.borderGap.extent = label!.size.width * Input_decoratorLibrary._kFinalLabelScale;
        }
        else
        {
            decoration.borderGap.start = null;
            decoration.borderGap.extent = 0.0;
        }
    }

    internal virtual void _paintLabel(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        context.paintChild(label!, offset);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        void doPaint(global::Doroti.Framework.Rendering.RenderBox? child)
        {
            if (child is not null)
            {
                context.paintChild(child, _boxParentData(child).offset + offset);
            }
        }
        doPaint(container);
        if (label is not null)
        {
            global::Doroti.Ui.Offset labelOffset = _boxParentData(label!).offset;
            double labelHeight = _boxSize(label).height;
            double labelWidth = _boxSize(label).width;
            double floatAlign = decoration.floatingLabelAlignment._x + 1L;
            double floatWidth = labelWidth * Input_decoratorLibrary._kFinalLabelScale;
            global::Doroti.Framework.Painting.BorderSide borderSideLocal = decoration.border.borderSide;
            double t = decoration.floatingLabelProgress;
            bool isOutlineBorder = decoration.border.isOutline;
            double outlinedFloatingY = (-labelHeight * Input_decoratorLibrary._kFinalLabelScale / 2.0) - (borderSideLocal.strokeOffset / 2.0);
            double floatingY = isOutlineBorder ? outlinedFloatingY : (contentPadding.top + (_densityOffset.dy / 2L));
            double scale = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(1.0, Input_decoratorLibrary._kFinalLabelScale, t));
            double centeredFloatX = _boxParentData(container!).offset.dx + (_boxSize(container).width / 2.0) - (floatWidth / 2.0);
            double startX = default!;
            double floatStartX = default!;
            switch (textDirection)
            {
                case TextDirection.rtl:
                    {
                        startX = labelOffset.dx + (labelWidth * (1.0 - scale));
                        floatStartX = startX;
                        if ((prefixIcon is not null) && !decoration.alignLabelWithHint && isOutlineBorder)
                        {
                            floatStartX += _boxSize(prefixIcon).width - contentPadding.end;
                        }
                        break;
                    }
                case TextDirection.ltr:
                    {
                        startX = labelOffset.dx;
                        floatStartX = startX;
                        if ((prefixIcon is not null) && !decoration.alignLabelWithHint && isOutlineBorder)
                        {
                            floatStartX += -_boxSize(prefixIcon).width + contentPadding.start;
                        }
                        break;
                    }
            }
            double floatEndX = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(floatStartX, centeredFloatX, floatAlign));
            double dxLocal = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(startX, floatEndX, t));
            double dyLocal = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, floatingY - labelOffset.dy, t));
            _labelTransform = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.translateByDouble(dxLocal, labelOffset.dy + dyLocal, 0, 1);
    __cascade.scaleByDouble(scale, scale, scale, 1);
    return __cascade;
}))();
            layer = context.pushTransform(needsCompositing, offset, _labelTransform!, _paintLabel, oldLayer: ((global::Doroti.Framework.Rendering.TransformLayer?)layer)!);
        }
        else
        {
            layer = null;
        }
        doPaint(icon);
        doPaint(prefix);
        doPaint(suffix);
        doPaint(prefixIcon);
        doPaint(suffixIcon);
        if (decoration.isEmpty)
        {
            doPaint(hint);
        }
        doPaint(input);
        doPaint(helperError);
        doPaint(counter);
    }

    public override void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        if (Equals(child, label) && (_labelTransform is not null))
        {
            global::Doroti.Ui.Offset labelOffset = _boxParentData(label!).offset;
            DartRuntimePrimitives.Ignore(((Func<Matrix4>)(() =>
{
    var __cascade = transform;
    __cascade.multiply(_labelTransform!);
    __cascade.translateByDouble(-labelOffset.dx, -labelOffset.dy, 0, 1);
    return __cascade;
}))());
        }
        base.applyPaintTransform(child, transform);
    }

    public override bool hitTestSelf(Offset position) => true;
    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
        {
            global::Doroti.Ui.Offset offsetLocal = _boxParentData(child).offset;
            bool isHit = result.addWithPaintOffset(offset: offsetLocal, position: position, hitTest: (result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(transformed, position - offsetLocal));
                return child.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            if (isHit)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResult _childSemanticsConfigurationDelegate(List<global::Doroti.Framework.Semantics.SemanticsConfiguration> childConfigs)
    {
        var builder = new global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResultBuilder();
        var mergeGroups = new DartMap<global::Doroti.Framework.Semantics.SemanticsTag, List<global::Doroti.Framework.Semantics.SemanticsConfiguration>>();
        var tags = new HashSet<global::Doroti.Framework.Semantics.SemanticsTag> { _InputDecoratorState__input_decorator._kPrefixSemanticsTag, _InputDecoratorState__input_decorator._kPrefixIconSemanticsTag, _InputDecoratorState__input_decorator._kSuffixSemanticsTag, _InputDecoratorState__input_decorator._kSuffixIconSemanticsTag };
        foreach (var childConfig in childConfigs)
        {
            global::Doroti.Framework.Semantics.SemanticsTag? tagLocal = tags.firstWhereOrNull((tag) => childConfig.tagsChildrenWith(tag));
            if (tagLocal is not null)
            {
                mergeGroups.putIfAbsent(tagLocal, () => new List<global::Doroti.Framework.Semantics.SemanticsConfiguration>()).Add(childConfig);
            }
            else
            {
                builder.markAsMergeUp(childConfig);
            }
        }
        mergeGroups.Values.forEach((__arg0) => ((global::System.Action<List<global::Doroti.Framework.Semantics.SemanticsConfiguration>>)builder.markAsSiblingMergeGroup)(__arg0));
        return builder.build();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        config.childConfigurationsDelegate = _childSemanticsConfigurationDelegate;
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox? childForSlot(_DecorationSlot__input_decorator slot) => _slotToChild.GetValueOrDefault(slot);
    public virtual string debugNameForSlot(_DecorationSlot__input_decorator slot)
    {
        {
            return slot.ToString();
        }
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
        {
            child.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
        {
            child.detach();
        }
    }

    public override void redepthChildren()
    {
        children.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)redepthChild)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        children.forEach((__arg0) => visitor(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        var childToSlot = new DartMap<global::Doroti.Framework.Rendering.RenderBox, _DecorationSlot__input_decorator>(_slotToChild.Values, _slotToChild.Keys);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
        {
            _addDiagnostics(child, value, debugNameForSlot(DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<_DecorationSlot__input_decorator>(childToSlot, child))));
        }
        return value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _addDiagnostics(global::Doroti.Framework.Rendering.RenderBox child, List<global::Doroti.Framework.Foundation.DiagnosticsNode> value, string name)
    {
        value.Add(((Diagnosticable)child).toDiagnosticsNode(name: name));
    }

    public virtual void _setChild(global::Doroti.Framework.Rendering.RenderBox? child, _DecorationSlot__input_decorator slot)
    {
        global::Doroti.Framework.Rendering.RenderBox? oldChild = _slotToChild.GetValueOrDefault(slot);
        if (oldChild is not null)
        {
            dropChild(oldChild);
            _slotToChild.remove(slot);
        }
        if (child is not null)
        {
            _slotToChild[slot] = child;
            adoptChild(child);
        }
    }

    public virtual void _moveChild(global::Doroti.Framework.Rendering.RenderBox child, _DecorationSlot__input_decorator slot, _DecorationSlot__input_decorator oldSlot)
    {
        DartRuntimePrimitives.Assert(() => !Equals(slot, oldSlot));
        global::Doroti.Framework.Rendering.RenderBox? oldChild = _slotToChild.GetValueOrDefault(oldSlot);
        if (Equals(oldChild, child))
        {
            _setChild(null, oldSlot);
        }
        _setChild(child, slot);
    }

}

internal class _Decorator__input_decorator : global::Doroti.Framework.Widgets.SlottedMultiChildRenderObjectWidget<_DecorationSlot__input_decorator, global::Doroti.Framework.Rendering.RenderBox>
{
    public virtual _Decoration__input_decorator decoration { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual TextBaseline textBaseline { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical { get; private set; }
    public virtual bool isFocused { get; private set; } = default!;
    public virtual bool expands { get; private set; } = default!;

    internal _Decorator__input_decorator(global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical, _Decoration__input_decorator decoration, TextDirection textDirection, TextBaseline textBaseline, bool isFocused, bool expands)
    {
        this.textAlignVertical = textAlignVertical;
        this.decoration = decoration;
        this.textDirection = textDirection;
        this.textBaseline = textBaseline;
        this.isFocused = isFocused;
        this.expands = expands;
    }

    public override IEnumerable<_DecorationSlot__input_decorator> slots => DartRuntimePrimitives.ConvertValue<IEnumerable<_DecorationSlot__input_decorator>>(Enum.GetValues<_DecorationSlot__input_decorator>().ToList());
    public override global::Doroti.Framework.Widgets.Widget? childForSlot(_DecorationSlot__input_decorator slot)
    {
        return slot switch { _DecorationSlot__input_decorator.icon => decoration.icon,_DecorationSlot__input_decorator.input => decoration.input,_DecorationSlot__input_decorator.label => decoration.label,_DecorationSlot__input_decorator.hint => decoration.hint,_DecorationSlot__input_decorator.prefix => decoration.prefix,_DecorationSlot__input_decorator.suffix => decoration.suffix,_DecorationSlot__input_decorator.prefixIcon => decoration.prefixIcon,_DecorationSlot__input_decorator.suffixIcon => decoration.suffixIcon,_DecorationSlot__input_decorator.helperError => decoration.helperError,_DecorationSlot__input_decorator.counter => decoration.counter,_DecorationSlot__input_decorator.container => decoration.container,_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderDecoration__input_decorator(decoration: decoration, textDirection: textDirection, textBaseline: textBaseline, textAlignVertical: textAlignVertical, isFocused: isFocused, expands: expands);
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderDecoration__input_decorator)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderDecoration__input_decorator>)(() =>
{
    var __cascade = __renderObject;
    __cascade.decoration = decoration;
    __cascade.expands = expands;
    __cascade.isFocused = isFocused;
    __cascade.textAlignVertical = textAlignVertical;
    __cascade.textBaseline = textBaseline;
    __cascade.textDirection = textDirection;
    return __cascade;
}))());
    }

}

internal class _AffixText__input_decorator : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual bool labelIsFloating { get; private set; } = default!;
    public virtual string? text { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Framework.Semantics.SemanticsSortKey? semanticsSortKey { get; private set; }
    public virtual global::Doroti.Framework.Semantics.SemanticsTag semanticsTag { get; private set; } = default!;

    internal _AffixText__input_decorator(bool labelIsFloating, string? text = null, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Widgets.Widget? child = null, global::Doroti.Framework.Semantics.SemanticsSortKey? semanticsSortKey = null, global::Doroti.Framework.Semantics.SemanticsTag semanticsTag = default!)
    {
        this.labelIsFloating = labelIsFloating;
        this.text = text;
        this.style = style;
        this.child = child;
        this.semanticsSortKey = semanticsSortKey;
        this.semanticsTag = semanticsTag;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return DefaultTextStyle.merge(style: style, child: new global::Doroti.Framework.Widgets.IgnorePointer(ignoring: !labelIsFloating, child: new global::Doroti.Framework.Widgets.AnimatedOpacity(duration: Input_decoratorLibrary._kTransitionDuration, curve: Input_decoratorLibrary._kTransitionCurve, opacity: labelIsFloating ? 1.0 : 0.0, child: new global::Doroti.Framework.Widgets.Semantics(sortKey: semanticsSortKey, tagForChildren: semanticsTag, child: child ?? ((text is null) ? null : new global::Doroti.Framework.Widgets.Text(text!, style: style))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class InputDecorator : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual InputDecoration decoration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? baseStyle { get; private set; }
    public virtual TextAlign? textAlign { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical { get; private set; }
    public virtual bool isFocused { get; private set; } = default!;
    public virtual bool isHovering { get; private set; } = default!;
    public virtual bool expands { get; private set; } = default!;
    public virtual bool isEmpty { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    public InputDecorator(global::Doroti.Framework.Foundation.Key? key = null, InputDecoration decoration = default!, global::Doroti.Framework.Painting.TextStyle? baseStyle = null, TextAlign? textAlign = null, global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical = null, bool isFocused = false, bool isHovering = false, bool expands = false, bool isEmpty = false, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key)
    {
        this.decoration = decoration;
        this.baseStyle = baseStyle;
        this.textAlign = textAlign;
        this.textAlignVertical = textAlignVertical;
        this.isFocused = isFocused;
        this.isHovering = isHovering;
        this.expands = expands;
        this.isEmpty = isEmpty;
        this.child = child;
    }

    internal virtual bool _labelShouldWithdraw => DartRuntimePrimitives.ConvertValue<bool>(!isEmpty || isFocused && decoration.enabled);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _InputDecoratorState__input_decorator());
    public static global::Doroti.Framework.Rendering.RenderBox? containerOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _RenderDecoration__input_decorator? result = context.findAncestorRenderObjectOfType<_RenderDecoration__input_decorator>();
        return result?.container;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputDecoration>("decoration", decoration));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("baseStyle", baseStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("isFocused", isFocused));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("expands", expands, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("isEmpty", isEmpty));
    }

}

internal class _InputDecoratorState__input_decorator : global::Doroti.Framework.Widgets.State<InputDecorator>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<InputDecorator>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _floatingLabelController { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _floatingLabelAnimation { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _shakingLabelController { get; private set; } = default!;
    internal virtual _InputBorderGap__input_decorator _borderGap { get; private set; } = new _InputBorderGap__input_decorator();
    private bool __late__prefixSemanticsSortOrder_initialized;
    private global::Doroti.Framework.Semantics.OrdinalSortKey __late__prefixSemanticsSortOrder = default!;
    internal virtual global::Doroti.Framework.Semantics.OrdinalSortKey _prefixSemanticsSortOrder
    {
        get
        {
            if (!__late__prefixSemanticsSortOrder_initialized)
            {
                __late__prefixSemanticsSortOrder = new global::Doroti.Framework.Semantics.OrdinalSortKey(0, name: GetHashCode().ToString());
                __late__prefixSemanticsSortOrder_initialized = true;
            }
            return __late__prefixSemanticsSortOrder;
        }
    }
    private bool __late__inputSemanticsSortOrder_initialized;
    private global::Doroti.Framework.Semantics.OrdinalSortKey __late__inputSemanticsSortOrder = default!;
    internal virtual global::Doroti.Framework.Semantics.OrdinalSortKey _inputSemanticsSortOrder
    {
        get
        {
            if (!__late__inputSemanticsSortOrder_initialized)
            {
                __late__inputSemanticsSortOrder = new global::Doroti.Framework.Semantics.OrdinalSortKey(1, name: GetHashCode().ToString());
                __late__inputSemanticsSortOrder_initialized = true;
            }
            return __late__inputSemanticsSortOrder;
        }
    }
    private bool __late__suffixSemanticsSortOrder_initialized;
    private global::Doroti.Framework.Semantics.OrdinalSortKey __late__suffixSemanticsSortOrder = default!;
    internal virtual global::Doroti.Framework.Semantics.OrdinalSortKey _suffixSemanticsSortOrder
    {
        get
        {
            if (!__late__suffixSemanticsSortOrder_initialized)
            {
                __late__suffixSemanticsSortOrder = new global::Doroti.Framework.Semantics.OrdinalSortKey(2, name: GetHashCode().ToString());
                __late__suffixSemanticsSortOrder_initialized = true;
            }
            return __late__suffixSemanticsSortOrder;
        }
    }
    internal static global::Doroti.Framework.Semantics.SemanticsTag _kPrefixSemanticsTag = new global::Doroti.Framework.Semantics.SemanticsTag("_InputDecoratorState.prefix");
    internal static global::Doroti.Framework.Semantics.SemanticsTag _kPrefixIconSemanticsTag = new global::Doroti.Framework.Semantics.SemanticsTag("_InputDecoratorState.prefixIcon");
    internal static global::Doroti.Framework.Semantics.SemanticsTag _kSuffixSemanticsTag = new global::Doroti.Framework.Semantics.SemanticsTag("_InputDecoratorState.suffix");
    internal static global::Doroti.Framework.Semantics.SemanticsTag _kSuffixIconSemanticsTag = new global::Doroti.Framework.Semantics.SemanticsTag("_InputDecoratorState.suffixIcon");
    internal virtual InputDecoration? _effectiveDecoration { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _curvedAnimation { get; set; } = default;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _floatingLabelController = new global::Doroti.Framework.Animation.AnimationController(duration: Input_decoratorLibrary._kTransitionDuration, vsync: this);
        _floatingLabelController.addListener(_handleChange);
        _floatingLabelAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _floatingLabelController, curve: Input_decoratorLibrary._kTransitionCurve, reverseCurve: Input_decoratorLibrary._kTransitionCurve.flipped);
        _shakingLabelController = new global::Doroti.Framework.Animation.AnimationController(duration: Input_decoratorLibrary._kTransitionDuration, vsync: this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _effectiveDecoration = null;
        bool labelIsInitiallyFloating = (!Equals(decoration.floatingLabelBehavior, FloatingLabelBehavior.never)) && labelShouldWithdraw;
        _floatingLabelController.value = labelIsInitiallyFloating ? 1.0 : 0.0;
    }

    public override void dispose()
    {
        _floatingLabelController.dispose();
        _floatingLabelAnimation.dispose();
        _shakingLabelController.dispose();
        _borderGap.dispose();
        _curvedAnimation?.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _handleChange()
    {
        setState(() =>
        {
        });
    }

    public virtual InputDecoration decoration => _effectiveDecoration ??= widget.decoration.applyDefaults(InputDecorationTheme.of(context));
    public virtual global::Doroti.Ui.TextAlign? textAlign => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextAlign>(widget.textAlign);
    public virtual bool isFocused => widget.isFocused;
    internal virtual bool _hasError => DartRuntimePrimitives.ConvertValue<bool>((decoration.errorText is not null) || (decoration.error is not null));
    public virtual bool isHovering => DartRuntimePrimitives.ConvertValue<bool>(widget.isHovering && decoration.enabled);
    public virtual bool isEmpty => widget.isEmpty;
    internal virtual bool _floatingLabelEnabled
    {
        get
        {
            return !Equals(decoration.floatingLabelBehavior, FloatingLabelBehavior.never);
        }
    }
    public virtual bool labelShouldWithdraw => DartRuntimePrimitives.ConvertValue<bool>(widget._labelShouldWithdraw || Equals(decoration.floatingLabelBehavior, FloatingLabelBehavior.always));
    public override void didUpdateWidget(InputDecorator old)
    {
        base.didUpdateWidget(old);
        if (!Equals(widget.decoration, old.decoration))
        {
            _effectiveDecoration = null;
        }
        var floatBehaviorChanged = !Equals(widget.decoration.floatingLabelBehavior, old.decoration.floatingLabelBehavior);
        if ((widget._labelShouldWithdraw != old._labelShouldWithdraw) || floatBehaviorChanged)
        {
            if (_floatingLabelEnabled && labelShouldWithdraw)
            {
                _floatingLabelController.forward();
            }
            else
            {
                _floatingLabelController.reverse();
            }
        }
        string? errorTextLocal = decoration.errorText;
        string? oldErrorText = old.decoration.errorText;
        if (_floatingLabelController.isCompleted && (errorTextLocal is not null) && (errorTextLocal != oldErrorText))
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = _shakingLabelController;
    __cascade.value = 0.0;
    __cascade.forward();
    return __cascade;
}))());
        }
    }

    internal virtual global::Doroti.Ui.Color _getFillColor(ThemeData themeData, InputDecorationThemeData defaults)
    {
        if (decoration.filled != true)
        {
            return Colors.transparent;
        }
        if (decoration.fillColor is not null)
        {
            return WidgetStateProperty.resolveAs(decoration.fillColor!, widgetState);
        }
        return WidgetStateProperty.resolveAs(defaults.fillColor!, widgetState);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _getHoverColor(ThemeData themeData)
    {
        if ((decoration.filled is null) || !DartRuntimePrimitives.RequireValue(decoration.filled) || !decoration.enabled)
        {
            return Colors.transparent;
        }
        return decoration.hoverColor ?? themeData.hoverColor;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _getIconColor(ThemeData themeData, InputDecorationThemeData defaults)
    {
        return WidgetStateProperty.resolveAs(decoration.iconColor, widgetState) ?? WidgetStateProperty.resolveAs(defaults.iconColor!, widgetState);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _getPrefixIconColor(IconButtonThemeData iconButtonTheme, InputDecorationThemeData defaults)
    {
        return (WidgetStateProperty.resolveAs(decoration.prefixIconColor, widgetState) ?? (iconButtonTheme.style?.foregroundColor?.resolve(widgetState))) ?? WidgetStateProperty.resolveAs(defaults.prefixIconColor!, widgetState);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _getSuffixIconColor(IconButtonThemeData iconButtonTheme, InputDecorationThemeData defaults)
    {
        return (WidgetStateProperty.resolveAs(decoration.suffixIconColor, widgetState) ?? (iconButtonTheme.style?.foregroundColor?.resolve(widgetState))) ?? WidgetStateProperty.resolveAs(defaults.suffixIconColor!, widgetState);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasInlineLabel
    {
        get
        {
            return !labelShouldWithdraw && ((decoration.labelText is not null) || (decoration.label is not null));
        }
    }
    internal virtual bool _shouldShowLabel => DartRuntimePrimitives.ConvertValue<bool>(_hasInlineLabel || _floatingLabelEnabled);
    internal virtual global::Doroti.Framework.Painting.TextStyle _getInlineLabelStyle(ThemeData themeData, InputDecorationThemeData defaults)
    {
        global::Doroti.Framework.Painting.TextStyle defaultStyle = WidgetStateProperty.resolveAs(defaults.labelStyle!, widgetState);
        global::Doroti.Framework.Painting.TextStyle? style = WidgetStateProperty.resolveAs(decoration.labelStyle, widgetState);
        return themeData.textTheme.titleMedium!.merge(widget.baseStyle).merge(defaultStyle).merge(style).copyWith(height: 1);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _getInlineHintStyle(ThemeData themeData, InputDecorationThemeData defaults)
    {
        global::Doroti.Framework.Painting.TextStyle defaultStyle = WidgetStateProperty.resolveAs(defaults.hintStyle!, widgetState);
        global::Doroti.Framework.Painting.TextStyle? style = WidgetStateProperty.resolveAs(decoration.hintStyle, widgetState);
        return themeData.textTheme.bodyLarge!.merge(widget.baseStyle).merge(defaultStyle).merge(style);
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _getFloatingLabelStyle(ThemeData themeData, InputDecorationThemeData defaults)
    {
        global::Doroti.Framework.Painting.TextStyle defaultTextStyle = WidgetStateProperty.resolveAs(defaults.floatingLabelStyle!, widgetState);
        if (_hasError && (decoration.errorStyle?.color is not null))
        {
            defaultTextStyle = defaultTextStyle.copyWith(color: decoration.errorStyle?.color);
        }
        defaultTextStyle = defaultTextStyle.merge(decoration.floatingLabelStyle ?? decoration.labelStyle);
        global::Doroti.Framework.Painting.TextStyle? style = WidgetStateProperty.resolveAs(decoration.floatingLabelStyle, widgetState);
        return themeData.textTheme.titleMedium!.merge(widget.baseStyle).merge(defaultTextStyle).merge(style).copyWith(height: 1);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _getHelperStyle(ThemeData themeData, InputDecorationThemeData defaults)
    {
        return WidgetStateProperty.resolveAs(defaults.helperStyle!, widgetState).merge(WidgetStateProperty.resolveAs(decoration.helperStyle, widgetState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _getErrorStyle(ThemeData themeData, InputDecorationThemeData defaults)
    {
        return WidgetStateProperty.resolveAs(defaults.errorStyle!, widgetState).merge(decoration.errorStyle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual HashSet<global::Doroti.Framework.Widgets.WidgetState> widgetState => ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection77750 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (!decoration.enabled) { __collection77750.Add(WidgetState.disabled); } if (isFocused) { __collection77750.Add(WidgetState.focused); } if (isHovering) { __collection77750.Add(WidgetState.hovered); } if (_hasError) { __collection77750.Add(WidgetState.error); } return __collection77750; }))();
    internal virtual InputBorder _getDefaultBorder(ThemeData themeData, InputDecorationThemeData defaults)
    {
        InputBorder borderLocal = WidgetStateProperty.resolveAs(decoration.border, widgetState) ?? new UnderlineInputBorder();
        if (decoration.border is global::Doroti.Framework.Widgets.WidgetStateProperty<InputBorder>)
        {
            return borderLocal;
        }
        if (Equals(borderLocal.borderSide, BorderSide.none))
        {
            return borderLocal;
        }
        {
            if (DartRuntimePrimitives.RequireValue(decoration.filled))
            {
                InputDecorationThemeData decorationTheme = InputDecorationTheme.of(context);
                return borderLocal.copyWith(borderSide: WidgetStateProperty.resolveAs(decorationTheme.activeIndicatorBorder ?? defaults.activeIndicatorBorder, widgetState));
            }
            else
            {
                return borderLocal.copyWith(borderSide: WidgetStateProperty.resolveAs(defaults.outlineBorder, widgetState));
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.FadeTransition _buildTransition(global::Doroti.Framework.Widgets.Widget child, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        if (!Equals(_curvedAnimation?.parent, animation))
        {
            _curvedAnimation?.dispose();
            _curvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: animation, curve: Input_decoratorLibrary._kTransitionCurve);
        }
        return new global::Doroti.Framework.Widgets.FadeTransition(opacity: _curvedAnimation!, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Widgets.Widget _topStartLayout(global::Doroti.Framework.Widgets.Widget? currentChild, List<global::Doroti.Framework.Widgets.Widget> previousChildren)
    {
        return new global::Doroti.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection79764 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection79764.AddRange(previousChildren); var __collectionElement79794 = currentChild; if (__collectionElement79794 is { } __nonNullCollectionElement79794) { __collection79764.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement79794)); } return __collection79764; }))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData themeData = Theme.of(context);
        VisualDensity visualDensityLocal = decoration.visualDensity ?? themeData.visualDensity;
        InputDecorationThemeData defaults = new _InputDecoratorDefaultsM3__input_decorator(context);
        IconButtonThemeData iconButtonTheme = IconButtonTheme.of(context);
        global::Doroti.Framework.Painting.TextStyle labelStyle = _getInlineLabelStyle(themeData, defaults);
        global::Doroti.Ui.TextBaseline textBaselineLocal = DartRuntimePrimitives.RequireValue(labelStyle.textBaseline);
        global::Doroti.Framework.Painting.TextStyle hintStyle = _getInlineHintStyle(themeData, defaults);
        string? hintTextLocal = decoration.hintText;
        bool maintainHintSizeLocal = decoration.maintainHintSize;
        global::Doroti.Framework.Widgets.Widget? hintLocal = default!;
        if ((decoration.hint is not null) || (hintTextLocal is not null))
        {
            global::Doroti.Framework.Widgets.Widget hintWidget = decoration.hint ?? new global::Doroti.Framework.Widgets.Text(hintTextLocal!, style: hintStyle, textDirection: decoration.hintTextDirection, overflow: hintStyle.overflow ?? ((decoration.hintMaxLines is null) ? null : global::Doroti.Framework.Painting.TextOverflow.ellipsis), textAlign: textAlign, maxLines: decoration.hintMaxLines);
            bool showHint = isEmpty && !_hasInlineLabel;
            hintLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(maintainHintSizeLocal ? new global::Doroti.Framework.Widgets.AnimatedOpacity(opacity: showHint ? 1.0 : 0.0, duration: decoration.hintFadeDuration ?? Input_decoratorLibrary._kHintFadeTransitionDuration, curve: Input_decoratorLibrary._kTransitionCurve, child: hintWidget) : new global::Doroti.Framework.Widgets.AnimatedSwitcher(duration: decoration.hintFadeDuration ?? Input_decoratorLibrary._kHintFadeTransitionDuration, transitionBuilder: (global::System.Func<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.FadeTransition>)_buildTransition, layoutBuilder: _topStartLayout, child: showHint ? hintWidget : global::Doroti.Framework.Widgets.SizedBox.CreateShrink()));
        }
        InputBorder? borderLocal = default!;
        if (!decoration.enabled)
        {
            borderLocal = _hasError ? decoration.errorBorder : decoration.disabledBorder;
        }
        else
        {
            if (isFocused)
            {
                borderLocal = _hasError ? decoration.focusedErrorBorder : decoration.focusedBorder;
            }
            else
            {
                borderLocal = _hasError ? decoration.errorBorder : decoration.enabledBorder;
            }
        }
        borderLocal ??= _getDefaultBorder(themeData, defaults);
        global::Doroti.Framework.Widgets.Widget containerLocal = new _BorderContainer__input_decorator(border: borderLocal, gap: _borderGap, gapAnimation: _floatingLabelAnimation, fillColor: _getFillColor(themeData, defaults), hoverColor: _getHoverColor(themeData), isHovering: isHovering);
        global::Doroti.Framework.Widgets.Widget? labelLocal = default!;
        if (((object?)decoration.labelText ?? decoration.label) is not null)
        {
            labelLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MatrixTransition(animation: _shakingLabelController, onTransform: (value) =>
            {
                double shakeOffset = value switch { <= 0.25 => -value, < 0.75 => value - 0.5, _ => (1.0 - value) * 4.0 };
                return Matrix4.translationValues(shakeOffset * 4.0, 0.0, 0.0);
            }, child: new global::Doroti.Framework.Widgets.AnimatedOpacity(duration: Input_decoratorLibrary._kTransitionDuration, curve: Input_decoratorLibrary._kTransitionCurve, opacity: _shouldShowLabel ? 1.0 : 0.0, child: new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(duration: Input_decoratorLibrary._kTransitionDuration, curve: Input_decoratorLibrary._kTransitionCurve, style: labelShouldWithdraw ? _getFloatingLabelStyle(themeData, defaults) : labelStyle, child: decoration.label ?? new global::Doroti.Framework.Widgets.Text(decoration.labelText!, overflow: TextOverflow.ellipsis, textAlign: textAlign)))));
        }
        bool hasPrefix = (decoration.prefix is not null) || (decoration.prefixText is not null);
        bool hasSuffix = (decoration.suffix is not null) || (decoration.suffixText is not null);
        global::Doroti.Framework.Widgets.Widget? inputLocal = widget.child;
        bool needsSemanticsSortOrder = labelShouldWithdraw && ((inputLocal is not null) ? (hasPrefix || hasSuffix) : (hasPrefix && hasSuffix));
        global::Doroti.Framework.Widgets.Widget? prefixLocal = hasPrefix ? new _AffixText__input_decorator(labelIsFloating: labelShouldWithdraw, text: decoration.prefixText, style: WidgetStateProperty.resolveAs(decoration.prefixStyle, widgetState) ?? hintStyle, semanticsSortKey: needsSemanticsSortOrder ? _prefixSemanticsSortOrder : null, semanticsTag: _kPrefixSemanticsTag, child: decoration.prefix) : null;
        global::Doroti.Framework.Widgets.Widget? suffixLocal = hasSuffix ? new _AffixText__input_decorator(labelIsFloating: labelShouldWithdraw, text: decoration.suffixText, style: WidgetStateProperty.resolveAs(decoration.suffixStyle, widgetState) ?? hintStyle, semanticsSortKey: needsSemanticsSortOrder ? _suffixSemanticsSortOrder : null, semanticsTag: _kSuffixSemanticsTag, child: decoration.suffix) : null;
        if ((inputLocal is not null) && needsSemanticsSortOrder)
        {
            inputLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(sortKey: _inputSemanticsSortOrder, child: inputLocal));
        }
        bool decorationIsDense = decoration.isDense ?? false;
        var iconSizeLocal = decorationIsDense ? 18.0 : 24.0;
        global::Doroti.Framework.Widgets.Widget? iconLocal = (decoration.icon is null) ? null : new global::Doroti.Framework.Widgets.MouseRegion(cursor: SystemMouseCursors.basic, child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(end: 16.0), child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(color: _getIconColor(themeData, defaults), size: iconSizeLocal), child: decoration.icon!)));
        global::Doroti.Framework.Widgets.Widget? prefixIconLocal = (decoration.prefixIcon is null) ? null : new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: new global::Doroti.Framework.Widgets.MouseRegion(cursor: SystemMouseCursors.basic, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: decoration.prefixIconConstraints ?? visualDensityLocal.effectiveConstraints(new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: ConstantsLibrary.kMinInteractiveDimension, minHeight: ConstantsLibrary.kMinInteractiveDimension)), child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(color: _getPrefixIconColor(iconButtonTheme, defaults), size: iconSizeLocal), child: new IconButtonTheme(data: new IconButtonThemeData(style: new ButtonStyle(foregroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(_getPrefixIconColor(iconButtonTheme, defaults)), iconSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(iconSizeLocal)).merge(iconButtonTheme.style)), child: new global::Doroti.Framework.Widgets.Semantics(tagForChildren: _kPrefixIconSemanticsTag, child: decoration.prefixIcon))))));
        global::Doroti.Framework.Widgets.Widget? suffixIconLocal = (decoration.suffixIcon is null) ? null : new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: new global::Doroti.Framework.Widgets.MouseRegion(cursor: SystemMouseCursors.basic, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: decoration.suffixIconConstraints ?? visualDensityLocal.effectiveConstraints(new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: ConstantsLibrary.kMinInteractiveDimension, minHeight: ConstantsLibrary.kMinInteractiveDimension)), child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(color: _getSuffixIconColor(iconButtonTheme, defaults), size: iconSizeLocal), child: new IconButtonTheme(data: new IconButtonThemeData(style: new ButtonStyle(foregroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(_getSuffixIconColor(iconButtonTheme, defaults)), iconSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(iconSizeLocal)).merge(iconButtonTheme.style)), child: new global::Doroti.Framework.Widgets.Semantics(tagForChildren: _kSuffixIconSemanticsTag, child: decoration.suffixIcon))))));
        global::Doroti.Framework.Widgets.Widget helperErrorLocal = new _HelperError__input_decorator(textAlign: textAlign, helper: decoration.helper, helperText: decoration.helperText, helperStyle: _getHelperStyle(themeData, defaults), helperMaxLines: decoration.helperMaxLines, error: decoration.error, errorText: decoration.errorText, errorStyle: _getErrorStyle(themeData, defaults), errorMaxLines: decoration.errorMaxLines);
        global::Doroti.Framework.Widgets.Widget? counterLocal = default!;
        if (decoration.counter is not null)
        {
            counterLocal = decoration.counter;
        }
        else
        {
            if ((decoration.counterText is not null) && (decoration.counterText != ""))
            {
                counterLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(container: true, liveRegion: isFocused, child: new global::Doroti.Framework.Widgets.Text(decoration.counterText!, style: _getHelperStyle(themeData, defaults).merge(WidgetStateProperty.resolveAs(decoration.counterStyle, widgetState)), overflow: TextOverflow.ellipsis, semanticsLabel: decoration.semanticCounterText)));
            }
        }
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        bool flipHorizontal = textDirectionLocal switch { TextDirection.ltr => false,TextDirection.rtl => true,_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        global::Doroti.Framework.Painting.EdgeInsets? resolvedPadding = decoration.contentPadding?.resolve(textDirectionLocal);
        global::Doroti.Framework.Painting.EdgeInsetsDirectional? decorationContentPadding = (resolvedPadding is null) ? null : new global::Doroti.Framework.Painting.EdgeInsetsDirectional(flipHorizontal ? resolvedPadding.right : resolvedPadding.left, resolvedPadding.top, flipHorizontal ? resolvedPadding.left : resolvedPadding.right, resolvedPadding.bottom);
        global::Doroti.Framework.Painting.EdgeInsetsDirectional contentPaddingLocal = default!;
        double floatingLabelHeightLocal = default!;
        if (DartRuntimePrimitives.RequireValue(decoration.isCollapsed))
        {
            floatingLabelHeightLocal = 0.0;
            contentPaddingLocal = decorationContentPadding ?? EdgeInsetsDirectional.zero;
        }
        else
        {
            if (!borderLocal.isOutline)
            {
                floatingLabelHeightLocal = MediaQuery.textScalerOf(context).scale(4.0 + (0.75 * DartRuntimePrimitives.RequireValue(labelStyle.fontSize)));
                if (decoration.filled ?? false)
                {
                    contentPaddingLocal = decorationContentPadding ?? (decorationIsDense ? new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12.0, 4.0, 12.0, 4.0) : new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12.0, 8.0, 12.0, 8.0));
                }
                else
                {
                    contentPaddingLocal = decorationContentPadding ?? (decorationIsDense ? new global::Doroti.Framework.Painting.EdgeInsetsDirectional(0.0, 4.0, 0.0, 4.0) : new global::Doroti.Framework.Painting.EdgeInsetsDirectional(0.0, 8.0, 0.0, 8.0));
                }
            }
            else
            {
                floatingLabelHeightLocal = 0.0;
                contentPaddingLocal = decorationContentPadding ?? (decorationIsDense ? new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12.0, 16.0, 12.0, 8.0) : new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12.0, 20.0, 12.0, 12.0));
            }
        }
        var inputGapLocal = 0.0;
        {
            if (borderLocal is OutlineInputBorder)
            {
                OutlineInputBorder border__81779__as92884 = (OutlineInputBorder)borderLocal;
                inputGapLocal = border__81779__as92884.gapPadding;
            }
            else
            {
                inputGapLocal = (borderLocal.isOutline || (decoration.filled ?? false)) ? Input_decoratorLibrary._kInputExtraPadding : 0.0;
            }
        }
        var decorator = new _Decorator__input_decorator(decoration: new _Decoration__input_decorator(contentPadding: contentPaddingLocal, isCollapsed: DartRuntimePrimitives.RequireValue(decoration.isCollapsed), inputGap: inputGapLocal, floatingLabelHeight: floatingLabelHeightLocal, floatingLabelAlignment: decoration.floatingLabelAlignment!, floatingLabelProgress: _floatingLabelAnimation.value, border: borderLocal, borderGap: _borderGap, alignLabelWithHint: decoration.alignLabelWithHint ?? false, isDense: decoration.isDense, isEmpty: isEmpty, visualDensity: visualDensityLocal, maintainHintSize: maintainHintSizeLocal, maintainLabelSize: decoration.maintainLabelSize, icon: iconLocal, input: inputLocal, label: labelLocal, hint: hintLocal, prefix: prefixLocal, suffix: suffixLocal, prefixIcon: prefixIconLocal, suffixIcon: suffixIconLocal, helperError: helperErrorLocal, counter: counterLocal, container: containerLocal), textDirection: textDirectionLocal, textBaseline: textBaselineLocal, textAlignVertical: widget.textAlignVertical, isFocused: isFocused, expands: widget.expands);
        string? semanticsHint = decoration.errorText;
        global::Doroti.Framework.Widgets.Widget result = new global::Doroti.Framework.Widgets.Semantics(hint: semanticsHint, child: decorator);
        global::Doroti.Framework.Rendering.BoxConstraints? constraintsLocal = decoration.constraints;
        if (constraintsLocal is not null)
        {
            return new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: constraintsLocal, child: result);
        }
        return result;
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

public class InputDecoration
{
    public virtual global::Doroti.Framework.Widgets.Widget? icon { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? label { get; private set; }
    public virtual string? labelText { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? labelStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? helper { get; private set; }
    public virtual string? helperText { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? helperStyle { get; private set; }
    public virtual long? helperMaxLines { get; private set; }
    public virtual string? hintText { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? hint { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? hintStyle { get; private set; }
    public virtual TextDirection? hintTextDirection { get; private set; }
    public virtual long? hintMaxLines { get; private set; }
    public virtual Duration? hintFadeDuration { get; private set; }
    public virtual bool maintainHintHeight { get; private set; } = default!;
    public virtual bool maintainHintSize { get; private set; } = default!;
    public virtual bool maintainLabelSize { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? error { get; private set; }
    public virtual string? errorText { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? errorStyle { get; private set; }
    public virtual long? errorMaxLines { get; private set; }
    public virtual FloatingLabelBehavior? floatingLabelBehavior { get; private set; }
    public virtual FloatingLabelAlignment? floatingLabelAlignment { get; private set; }
    public virtual bool? isDense { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual bool? isCollapsed { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? prefixIcon { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? prefixIconConstraints { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? prefix { get; private set; }
    public virtual string? prefixText { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? prefixStyle { get; private set; }
    public virtual Color? prefixIconColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? suffixIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? suffix { get; private set; }
    public virtual string? suffixText { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? suffixStyle { get; private set; }
    public virtual Color? suffixIconColor { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? suffixIconConstraints { get; private set; }
    public virtual string? counterText { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? counter { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? counterStyle { get; private set; }
    public virtual bool? filled { get; private set; }
    public virtual Color? fillColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual InputBorder? errorBorder { get; private set; }
    public virtual InputBorder? focusedBorder { get; private set; }
    public virtual InputBorder? focusedErrorBorder { get; private set; }
    public virtual InputBorder? disabledBorder { get; private set; }
    public virtual InputBorder? enabledBorder { get; private set; }
    public virtual InputBorder? border { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual string? semanticCounterText { get; private set; }
    public virtual bool? alignLabelWithHint { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }

    public InputDecoration(global::Doroti.Framework.Widgets.Widget? icon = null, Color? iconColor = null, global::Doroti.Framework.Widgets.Widget? label = null, string? labelText = null, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle = null, global::Doroti.Framework.Widgets.Widget? helper = null, string? helperText = null, global::Doroti.Framework.Painting.TextStyle? helperStyle = null, long? helperMaxLines = null, string? hintText = null, global::Doroti.Framework.Widgets.Widget? hint = null, global::Doroti.Framework.Painting.TextStyle? hintStyle = null, TextDirection? hintTextDirection = null, long? hintMaxLines = null, Duration? hintFadeDuration = null, bool maintainHintHeight = true, bool maintainHintSize = true, bool maintainLabelSize = false, global::Doroti.Framework.Widgets.Widget? error = null, string? errorText = null, global::Doroti.Framework.Painting.TextStyle? errorStyle = null, long? errorMaxLines = null, FloatingLabelBehavior? floatingLabelBehavior = null, FloatingLabelAlignment? floatingLabelAlignment = null, bool? isCollapsed = null, bool? isDense = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Framework.Widgets.Widget? prefixIcon = null, global::Doroti.Framework.Rendering.BoxConstraints? prefixIconConstraints = null, global::Doroti.Framework.Widgets.Widget? prefix = null, string? prefixText = null, global::Doroti.Framework.Painting.TextStyle? prefixStyle = null, Color? prefixIconColor = null, global::Doroti.Framework.Widgets.Widget? suffixIcon = null, global::Doroti.Framework.Widgets.Widget? suffix = null, string? suffixText = null, global::Doroti.Framework.Painting.TextStyle? suffixStyle = null, Color? suffixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? suffixIconConstraints = null, global::Doroti.Framework.Widgets.Widget? counter = null, string? counterText = null, global::Doroti.Framework.Painting.TextStyle? counterStyle = null, bool? filled = null, Color? fillColor = null, Color? focusColor = null, Color? hoverColor = null, InputBorder? errorBorder = null, InputBorder? focusedBorder = null, InputBorder? focusedErrorBorder = null, InputBorder? disabledBorder = null, InputBorder? enabledBorder = null, InputBorder? border = null, bool enabled = true, string? semanticCounterText = null, bool? alignLabelWithHint = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, VisualDensity? visualDensity = null)
    {
        this.icon = icon;
        this.iconColor = iconColor;
        this.label = label;
        this.labelText = labelText;
        this.labelStyle = labelStyle;
        this.floatingLabelStyle = floatingLabelStyle;
        this.helper = helper;
        this.helperText = helperText;
        this.helperStyle = helperStyle;
        this.helperMaxLines = helperMaxLines;
        this.hintText = hintText;
        this.hint = hint;
        this.hintStyle = hintStyle;
        this.hintTextDirection = hintTextDirection;
        this.hintMaxLines = hintMaxLines;
        this.hintFadeDuration = hintFadeDuration;
        this.maintainHintHeight = maintainHintHeight;
        this.maintainHintSize = maintainHintSize;
        this.maintainLabelSize = maintainLabelSize;
        this.error = error;
        this.errorText = errorText;
        this.errorStyle = errorStyle;
        this.errorMaxLines = errorMaxLines;
        this.floatingLabelBehavior = floatingLabelBehavior;
        this.floatingLabelAlignment = floatingLabelAlignment;
        this.isCollapsed = isCollapsed;
        this.isDense = isDense;
        this.contentPadding = contentPadding;
        this.prefixIcon = prefixIcon;
        this.prefixIconConstraints = prefixIconConstraints;
        this.prefix = prefix;
        this.prefixText = prefixText;
        this.prefixStyle = prefixStyle;
        this.prefixIconColor = prefixIconColor;
        this.suffixIcon = suffixIcon;
        this.suffix = suffix;
        this.suffixText = suffixText;
        this.suffixStyle = suffixStyle;
        this.suffixIconColor = suffixIconColor;
        this.suffixIconConstraints = suffixIconConstraints;
        this.counter = counter;
        this.counterText = counterText;
        this.counterStyle = counterStyle;
        this.filled = filled;
        this.fillColor = fillColor;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.errorBorder = errorBorder;
        this.focusedBorder = focusedBorder;
        this.focusedErrorBorder = focusedErrorBorder;
        this.disabledBorder = disabledBorder;
        this.enabledBorder = enabledBorder;
        this.border = border;
        this.enabled = enabled;
        this.semanticCounterText = semanticCounterText;
        this.alignLabelWithHint = alignLabelWithHint;
        this.constraints = constraints;
        this.visualDensity = visualDensity;
        System.Diagnostics.Debug.Assert(!((label is not null) && (labelText is not null)));
        System.Diagnostics.Debug.Assert((hint is null) || (hintText is null));
        System.Diagnostics.Debug.Assert(!((helper is not null) && (helperText is not null)));
        System.Diagnostics.Debug.Assert(!((prefix is not null) && (prefixText is not null)));
        System.Diagnostics.Debug.Assert(!((suffix is not null) && (suffixText is not null)));
        System.Diagnostics.Debug.Assert(!((error is not null) && (errorText is not null)));
    }

    public static InputDecoration CreateCollapsed(string? hintText, FloatingLabelBehavior? floatingLabelBehavior = null, FloatingLabelAlignment? floatingLabelAlignment = null, global::Doroti.Framework.Painting.TextStyle? hintStyle = null, global::Doroti.Framework.Widgets.Widget? hint = null, TextDirection? hintTextDirection = null, long? hintMaxLines = null, Duration? hintFadeDuration = null, bool maintainHintHeight = true, bool maintainHintSize = true, bool maintainLabelSize = false, bool? filled = false, Color? fillColor = null, Color? focusColor = null, Color? hoverColor = null, InputBorder? border = default!, bool enabled = true, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null)
    {
        var __instance = new InputDecoration(hintText: hintText, hint: hint, hintStyle: hintStyle, hintTextDirection: hintTextDirection, hintMaxLines: hintMaxLines, hintFadeDuration: hintFadeDuration, maintainHintHeight: maintainHintHeight, maintainHintSize: maintainHintSize, maintainLabelSize: maintainLabelSize, floatingLabelBehavior: floatingLabelBehavior, floatingLabelAlignment: floatingLabelAlignment, filled: filled, fillColor: fillColor, focusColor: focusColor, hoverColor: hoverColor, border: border, enabled: enabled, constraints: constraints);
        InputBorder? __border = border ?? InputBorder.none;
        __instance.hintText = hintText;
        __instance.hintStyle = hintStyle;
        __instance.hint = hint;
        __instance.hintTextDirection = hintTextDirection;
        __instance.hintMaxLines = hintMaxLines;
        __instance.hintFadeDuration = hintFadeDuration;
        __instance.maintainHintHeight = maintainHintHeight;
        __instance.maintainHintSize = maintainHintSize;
        __instance.maintainLabelSize = maintainLabelSize;
        __instance.filled = filled;
        __instance.fillColor = fillColor;
        __instance.focusColor = focusColor;
        __instance.hoverColor = hoverColor;
        __instance.border = __border;
        __instance.enabled = enabled;
        __instance.constraints = constraints;
        __instance.icon = null;
        __instance.iconColor = null;
        __instance.label = null;
        __instance.labelText = null;
        __instance.labelStyle = null;
        __instance.floatingLabelStyle = null;
        __instance.helper = null;
        __instance.helperText = null;
        __instance.helperStyle = null;
        __instance.helperMaxLines = null;
        __instance.error = null;
        __instance.errorText = null;
        __instance.errorStyle = null;
        __instance.errorMaxLines = null;
        __instance.isDense = false;
        __instance.contentPadding = EdgeInsets.zero;
        __instance.isCollapsed = true;
        __instance.prefixIcon = null;
        __instance.prefix = null;
        __instance.prefixText = null;
        __instance.prefixStyle = null;
        __instance.prefixIconColor = null;
        __instance.prefixIconConstraints = null;
        __instance.suffix = null;
        __instance.suffixIcon = null;
        __instance.suffixText = null;
        __instance.suffixStyle = null;
        __instance.suffixIconColor = null;
        __instance.suffixIconConstraints = null;
        __instance.counter = null;
        __instance.counterText = null;
        __instance.counterStyle = null;
        __instance.errorBorder = null;
        __instance.focusedBorder = null;
        __instance.focusedErrorBorder = null;
        __instance.disabledBorder = null;
        __instance.enabledBorder = null;
        __instance.semanticCounterText = null;
        __instance.floatingLabelBehavior = floatingLabelBehavior;
        __instance.floatingLabelAlignment = floatingLabelAlignment;
        __instance.alignLabelWithHint = false;
        __instance.visualDensity = null;
        return __instance;
    }

    public virtual InputDecoration copyWith(global::Doroti.Framework.Widgets.Widget? icon = null, Color? iconColor = null, global::Doroti.Framework.Widgets.Widget? label = null, string? labelText = null, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle = null, global::Doroti.Framework.Widgets.Widget? helper = null, string? helperText = null, global::Doroti.Framework.Painting.TextStyle? helperStyle = null, long? helperMaxLines = null, string? hintText = null, global::Doroti.Framework.Widgets.Widget? hint = null, global::Doroti.Framework.Painting.TextStyle? hintStyle = null, TextDirection? hintTextDirection = null, Duration? hintFadeDuration = null, long? hintMaxLines = null, bool? maintainHintHeight = null, bool? maintainHintSize = null, bool? maintainLabelSize = null, global::Doroti.Framework.Widgets.Widget? error = null, string? errorText = null, global::Doroti.Framework.Painting.TextStyle? errorStyle = null, long? errorMaxLines = null, FloatingLabelBehavior? floatingLabelBehavior = null, FloatingLabelAlignment? floatingLabelAlignment = null, bool? isCollapsed = null, bool? isDense = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Framework.Widgets.Widget? prefixIcon = null, global::Doroti.Framework.Widgets.Widget? prefix = null, string? prefixText = null, global::Doroti.Framework.Rendering.BoxConstraints? prefixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? prefixStyle = null, Color? prefixIconColor = null, global::Doroti.Framework.Widgets.Widget? suffixIcon = null, global::Doroti.Framework.Widgets.Widget? suffix = null, string? suffixText = null, global::Doroti.Framework.Painting.TextStyle? suffixStyle = null, Color? suffixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? suffixIconConstraints = null, global::Doroti.Framework.Widgets.Widget? counter = null, string? counterText = null, global::Doroti.Framework.Painting.TextStyle? counterStyle = null, bool? filled = null, Color? fillColor = null, Color? focusColor = null, Color? hoverColor = null, InputBorder? errorBorder = null, InputBorder? focusedBorder = null, InputBorder? focusedErrorBorder = null, InputBorder? disabledBorder = null, InputBorder? enabledBorder = null, InputBorder? border = null, bool? enabled = null, string? semanticCounterText = null, bool? alignLabelWithHint = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Semantics.SemanticsService? semanticsService = null)
    {
        return new InputDecoration(icon: icon ?? this.icon, iconColor: iconColor ?? this.iconColor, label: label ?? this.label, labelText: labelText ?? this.labelText, labelStyle: labelStyle ?? this.labelStyle, floatingLabelStyle: floatingLabelStyle ?? this.floatingLabelStyle, helper: helper ?? this.helper, helperText: helperText ?? this.helperText, helperStyle: helperStyle ?? this.helperStyle, helperMaxLines: helperMaxLines ?? this.helperMaxLines, hintText: hintText ?? this.hintText, hint: hint ?? this.hint, hintStyle: hintStyle ?? this.hintStyle, hintTextDirection: hintTextDirection ?? this.hintTextDirection, hintMaxLines: hintMaxLines ?? this.hintMaxLines, hintFadeDuration: hintFadeDuration ?? this.hintFadeDuration, maintainHintHeight: maintainHintHeight ?? this.maintainHintHeight, maintainHintSize: maintainHintSize ?? this.maintainHintSize, maintainLabelSize: maintainLabelSize ?? this.maintainLabelSize, error: error ?? this.error, errorText: errorText ?? this.errorText, errorStyle: errorStyle ?? this.errorStyle, errorMaxLines: errorMaxLines ?? this.errorMaxLines, floatingLabelBehavior: floatingLabelBehavior ?? this.floatingLabelBehavior, floatingLabelAlignment: floatingLabelAlignment ?? this.floatingLabelAlignment, isCollapsed: isCollapsed ?? this.isCollapsed, isDense: isDense ?? this.isDense, contentPadding: contentPadding ?? this.contentPadding, prefixIcon: prefixIcon ?? this.prefixIcon, prefix: prefix ?? this.prefix, prefixText: prefixText ?? this.prefixText, prefixStyle: prefixStyle ?? this.prefixStyle, prefixIconColor: prefixIconColor ?? this.prefixIconColor, prefixIconConstraints: prefixIconConstraints ?? this.prefixIconConstraints, suffixIcon: suffixIcon ?? this.suffixIcon, suffix: suffix ?? this.suffix, suffixText: suffixText ?? this.suffixText, suffixStyle: suffixStyle ?? this.suffixStyle, suffixIconColor: suffixIconColor ?? this.suffixIconColor, suffixIconConstraints: suffixIconConstraints ?? this.suffixIconConstraints, counter: counter ?? this.counter, counterText: counterText ?? this.counterText, counterStyle: counterStyle ?? this.counterStyle, filled: filled ?? this.filled, fillColor: fillColor ?? this.fillColor, focusColor: focusColor ?? this.focusColor, hoverColor: hoverColor ?? this.hoverColor, errorBorder: errorBorder ?? this.errorBorder, focusedBorder: focusedBorder ?? this.focusedBorder, focusedErrorBorder: focusedErrorBorder ?? this.focusedErrorBorder, disabledBorder: disabledBorder ?? this.disabledBorder, enabledBorder: enabledBorder ?? this.enabledBorder, border: border ?? this.border, enabled: enabled ?? this.enabled, semanticCounterText: semanticCounterText ?? this.semanticCounterText, alignLabelWithHint: alignLabelWithHint ?? this.alignLabelWithHint, constraints: constraints ?? this.constraints, visualDensity: visualDensity ?? this.visualDensity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InputDecoration applyDefaults(object inputDecorationTheme)
    {
        if ((inputDecorationTheme is not InputDecorationTheme) && (inputDecorationTheme is not InputDecorationThemeData))
        {
            throw DartRuntimePrimitives.AsException(new DartArgumentError("inputDecorationTheme must be either a InputDecorationThemeData or a InputDecorationTheme"));
        }
        InputDecorationThemeData theme = (inputDecorationTheme is InputDecorationTheme) ? ((InputDecorationTheme)inputDecorationTheme).data : ((InputDecorationThemeData?)inputDecorationTheme)!;
        return copyWith(labelStyle: labelStyle ?? theme.labelStyle, floatingLabelStyle: floatingLabelStyle ?? theme.floatingLabelStyle, helperStyle: helperStyle ?? theme.helperStyle, helperMaxLines: helperMaxLines ?? theme.helperMaxLines, hintStyle: hintStyle ?? theme.hintStyle, hintFadeDuration: hintFadeDuration ?? theme.hintFadeDuration, hintMaxLines: hintMaxLines ?? theme.hintMaxLines, errorStyle: errorStyle ?? theme.errorStyle, errorMaxLines: errorMaxLines ?? theme.errorMaxLines, floatingLabelBehavior: floatingLabelBehavior ?? theme.floatingLabelBehavior, floatingLabelAlignment: floatingLabelAlignment ?? theme.floatingLabelAlignment, isDense: isDense ?? theme.isDense, contentPadding: contentPadding ?? theme.contentPadding, isCollapsed: isCollapsed ?? theme.isCollapsed, iconColor: iconColor ?? theme.iconColor, prefixStyle: prefixStyle ?? theme.prefixStyle, prefixIconColor: prefixIconColor ?? theme.prefixIconColor, prefixIconConstraints: prefixIconConstraints ?? theme.prefixIconConstraints, suffixStyle: suffixStyle ?? theme.suffixStyle, suffixIconColor: suffixIconColor ?? theme.suffixIconColor, suffixIconConstraints: suffixIconConstraints ?? theme.suffixIconConstraints, counterStyle: counterStyle ?? theme.counterStyle, filled: filled ?? theme.filled, fillColor: fillColor ?? theme.fillColor, focusColor: focusColor ?? theme.focusColor, hoverColor: hoverColor ?? theme.hoverColor, errorBorder: errorBorder ?? theme.errorBorder, focusedBorder: focusedBorder ?? theme.focusedBorder, focusedErrorBorder: focusedErrorBorder ?? theme.focusedErrorBorder, disabledBorder: disabledBorder ?? theme.disabledBorder, enabledBorder: enabledBorder ?? theme.enabledBorder, border: border ?? theme.border, alignLabelWithHint: alignLabelWithHint ?? theme.alignLabelWithHint, constraints: constraints ?? theme.constraints, visualDensity: visualDensity ?? theme.visualDensity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as InputDecoration;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is InputDecoration) && Equals(__other.icon, icon) && Equals(__other.iconColor, iconColor) && Equals(__other.label, label) && (__other.labelText == labelText) && Equals(__other.labelStyle, labelStyle) && Equals(__other.floatingLabelStyle, floatingLabelStyle) && Equals(__other.helper, helper) && (__other.helperText == helperText) && Equals(__other.helperStyle, helperStyle) && (__other.helperMaxLines == helperMaxLines) && (__other.hintText == hintText) && Equals(__other.hint, hint) && Equals(__other.hintStyle, hintStyle) && Equals(__other.hintTextDirection, hintTextDirection) && (__other.hintMaxLines == hintMaxLines) && Equals(__other.hintFadeDuration, hintFadeDuration) && (__other.maintainHintHeight == maintainHintHeight) && (__other.maintainHintSize == maintainHintSize) && (__other.maintainLabelSize == maintainLabelSize) && Equals(__other.error, error) && (__other.errorText == errorText) && Equals(__other.errorStyle, errorStyle) && (__other.errorMaxLines == errorMaxLines) && Equals(__other.floatingLabelBehavior, floatingLabelBehavior) && Equals(__other.floatingLabelAlignment, floatingLabelAlignment) && (__other.isDense == isDense) && Equals(__other.contentPadding, contentPadding) && (__other.isCollapsed == isCollapsed) && Equals(__other.prefixIcon, prefixIcon) && Equals(__other.prefixIconColor, prefixIconColor) && Equals(__other.prefix, prefix) && (__other.prefixText == prefixText) && Equals(__other.prefixStyle, prefixStyle) && Equals(__other.prefixIconConstraints, prefixIconConstraints) && Equals(__other.suffixIcon, suffixIcon) && Equals(__other.suffixIconColor, suffixIconColor) && Equals(__other.suffix, suffix) && (__other.suffixText == suffixText) && Equals(__other.suffixStyle, suffixStyle) && Equals(__other.suffixIconConstraints, suffixIconConstraints) && Equals(__other.counter, counter) && (__other.counterText == counterText) && Equals(__other.counterStyle, counterStyle) && (__other.filled == filled) && Equals(__other.fillColor, fillColor) && Equals(__other.focusColor, focusColor) && Equals(__other.hoverColor, hoverColor) && Equals(__other.errorBorder, errorBorder) && Equals(__other.focusedBorder, focusedBorder) && Equals(__other.focusedErrorBorder, focusedErrorBorder) && Equals(__other.disabledBorder, disabledBorder) && Equals(__other.enabledBorder, enabledBorder) && Equals(__other.border, border) && (__other.enabled == enabled) && (__other.semanticCounterText == semanticCounterText) && (__other.alignLabelWithHint == alignLabelWithHint) && Equals(__other.constraints, constraints) && Equals(__other.visualDensity, visualDensity);
    }

    public override int GetHashCode()
    {
        var values = new List<object?> { icon, iconColor, label, labelText, floatingLabelStyle, labelStyle, helper, helperText, helperStyle, helperMaxLines, hintText, hint, hintStyle, hintTextDirection, hintMaxLines, hintFadeDuration, maintainHintHeight, maintainHintSize, maintainLabelSize, error, errorText, errorStyle, errorMaxLines, floatingLabelBehavior, floatingLabelAlignment, isDense, contentPadding, isCollapsed, filled, fillColor, focusColor, hoverColor, prefixIcon, prefixIconColor, prefix, prefixText, prefixStyle, prefixIconConstraints, suffixIcon, suffixIconColor, suffix, suffixText, suffixStyle, suffixIconConstraints, counter, counterText, counterStyle, errorBorder, focusedBorder, focusedErrorBorder, disabledBorder, enabledBorder, border, enabled, semanticCounterText, alignLabelWithHint, constraints, visualDensity };
        return FoundationRuntimePorts.ObjectHashAll(values);
    }
    public override string ToString()
    {
        var description = ((Func<List<string>>)(() => { var __collection157531 = new List<string>(); if (icon is not null) { __collection157531.Add($"icon: {icon}"); } if (iconColor is not null) { __collection157531.Add($"iconColor: {iconColor}"); } if (label is not null) { __collection157531.Add($"label: {label}"); } if (labelText is not null) { __collection157531.Add($"labelText: \"{labelText}\""); } if (floatingLabelStyle is not null) { __collection157531.Add($"floatingLabelStyle: \"{floatingLabelStyle}\""); } if (helper is not null) { __collection157531.Add($"helper: \"{helper}\""); } if (helperText is not null) { __collection157531.Add($"helperText: \"{helperText}\""); } if (helperMaxLines is not null) { __collection157531.Add($"helperMaxLines: \"{helperMaxLines}\""); } if (hintText is not null) { __collection157531.Add($"hintText: \"{hintText}\""); } if (hint is not null) { __collection157531.Add($"hint: {hint}"); } if (hintMaxLines is not null) { __collection157531.Add($"hintMaxLines: \"{hintMaxLines}\""); } if (hintFadeDuration is not null) { __collection157531.Add($"hintFadeDuration: \"{hintFadeDuration}\""); } if (!maintainHintHeight) { __collection157531.Add("maintainHintHeight: false"); } if (!maintainHintSize) { __collection157531.Add("maintainHintSize: false"); } if (maintainLabelSize) { __collection157531.Add("maintainLabelSize: true"); } if (error is not null) { __collection157531.Add($"error: \"{error}\""); } if (errorText is not null) { __collection157531.Add($"errorText: \"{errorText}\""); } if (errorStyle is not null) { __collection157531.Add($"errorStyle: \"{errorStyle}\""); } if (errorMaxLines is not null) { __collection157531.Add($"errorMaxLines: \"{errorMaxLines}\""); } if (floatingLabelBehavior is not null) { __collection157531.Add($"floatingLabelBehavior: {floatingLabelBehavior}"); } if (floatingLabelAlignment is not null) { __collection157531.Add($"floatingLabelAlignment: {floatingLabelAlignment}"); } if (isDense ?? false) { __collection157531.Add($"isDense: {isDense}"); } if (contentPadding is not null) { __collection157531.Add($"contentPadding: {contentPadding}"); } if (isCollapsed ?? false) { __collection157531.Add($"isCollapsed: {isCollapsed}"); } if (prefixIcon is not null) { __collection157531.Add($"prefixIcon: {prefixIcon}"); } if (prefixIconColor is not null) { __collection157531.Add($"prefixIconColor: {prefixIconColor}"); } if (prefix is not null) { __collection157531.Add($"prefix: {prefix}"); } if (prefixText is not null) { __collection157531.Add($"prefixText: {prefixText}"); } if (prefixStyle is not null) { __collection157531.Add($"prefixStyle: {prefixStyle}"); } if (prefixIconConstraints is not null) { __collection157531.Add($"prefixIconConstraints: {prefixIconConstraints}"); } if (suffixIcon is not null) { __collection157531.Add($"suffixIcon: {suffixIcon}"); } if (suffixIconColor is not null) { __collection157531.Add($"suffixIconColor: {suffixIconColor}"); } if (suffix is not null) { __collection157531.Add($"suffix: {suffix}"); } if (suffixText is not null) { __collection157531.Add($"suffixText: {suffixText}"); } if (suffixStyle is not null) { __collection157531.Add($"suffixStyle: {suffixStyle}"); } if (suffixIconConstraints is not null) { __collection157531.Add($"suffixIconConstraints: {suffixIconConstraints}"); } if (counter is not null) { __collection157531.Add($"counter: {counter}"); } if (counterText is not null) { __collection157531.Add($"counterText: {counterText}"); } if (counterStyle is not null) { __collection157531.Add($"counterStyle: {counterStyle}"); } if (filled ?? false) { __collection157531.Add("filled: true"); } if (fillColor is not null) { __collection157531.Add($"fillColor: {fillColor}"); } if (focusColor is not null) { __collection157531.Add($"focusColor: {focusColor}"); } if (hoverColor is not null) { __collection157531.Add($"hoverColor: {hoverColor}"); } if (errorBorder is not null) { __collection157531.Add($"errorBorder: {errorBorder}"); } if (focusedBorder is not null) { __collection157531.Add($"focusedBorder: {focusedBorder}"); } if (focusedErrorBorder is not null) { __collection157531.Add($"focusedErrorBorder: {focusedErrorBorder}"); } if (disabledBorder is not null) { __collection157531.Add($"disabledBorder: {disabledBorder}"); } if (enabledBorder is not null) { __collection157531.Add($"enabledBorder: {enabledBorder}"); } if (border is not null) { __collection157531.Add($"border: {border}"); } if (!enabled) { __collection157531.Add("enabled: false"); } if (semanticCounterText is not null) { __collection157531.Add($"semanticCounterText: {semanticCounterText}"); } if (alignLabelWithHint is not null) { __collection157531.Add($"alignLabelWithHint: {alignLabelWithHint}"); } if (constraints is not null) { __collection157531.Add($"constraints: {constraints}"); } if (visualDensity is not null) { __collection157531.Add($"visualDensity: {visualDensity}"); } return __collection157531; }))();
        return $"InputDecoration({string.Join(", ", description)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class InputDecorationTheme : global::Doroti.Framework.Widgets.InheritedTheme, global::Doroti.Framework.Foundation.Diagnosticable
{
    internal virtual InputDecorationThemeData? _data { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _labelStyle { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _floatingLabelStyle { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _helperStyle { get; private set; }
    internal virtual long? _helperMaxLines { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _hintStyle { get; private set; }
    internal virtual Duration? _hintFadeDuration { get; private set; }
    internal virtual long? _hintMaxLines { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _errorStyle { get; private set; }
    internal virtual long? _errorMaxLines { get; private set; }
    internal virtual FloatingLabelBehavior _floatingLabelBehavior { get; private set; } = default!;
    internal virtual FloatingLabelAlignment _floatingLabelAlignment { get; private set; } = default!;
    internal virtual bool _isDense { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? _contentPadding { get; private set; }
    internal virtual bool _isCollapsed { get; private set; } = default!;
    internal virtual Color? _iconColor { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _prefixStyle { get; private set; }
    internal virtual Color? _prefixIconColor { get; private set; }
    internal virtual global::Doroti.Framework.Rendering.BoxConstraints? _prefixIconConstraints { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _suffixStyle { get; private set; }
    internal virtual Color? _suffixIconColor { get; private set; }
    internal virtual global::Doroti.Framework.Rendering.BoxConstraints? _suffixIconConstraints { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _counterStyle { get; private set; }
    internal virtual bool _filled { get; private set; } = default!;
    internal virtual Color? _fillColor { get; private set; }
    internal virtual global::Doroti.Framework.Painting.BorderSide? _activeIndicatorBorder { get; private set; }
    internal virtual global::Doroti.Framework.Painting.BorderSide? _outlineBorder { get; private set; }
    internal virtual Color? _focusColor { get; private set; }
    internal virtual Color? _hoverColor { get; private set; }
    internal virtual InputBorder? _errorBorder { get; private set; }
    internal virtual InputBorder? _focusedBorder { get; private set; }
    internal virtual InputBorder? _focusedErrorBorder { get; private set; }
    internal virtual InputBorder? _disabledBorder { get; private set; }
    internal virtual InputBorder? _enabledBorder { get; private set; }
    internal virtual InputBorder? _border { get; private set; }
    internal virtual bool _alignLabelWithHint { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Rendering.BoxConstraints? _constraints { get; private set; }
    internal virtual VisualDensity? _visualDensity { get; private set; }

    public InputDecorationTheme(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle = null, global::Doroti.Framework.Painting.TextStyle? helperStyle = null, long? helperMaxLines = null, global::Doroti.Framework.Painting.TextStyle? hintStyle = null, Duration? hintFadeDuration = null, long? hintMaxLines = null, global::Doroti.Framework.Painting.TextStyle? errorStyle = null, long? errorMaxLines = null, FloatingLabelBehavior? floatingLabelBehavior = null, FloatingLabelAlignment? floatingLabelAlignment = null, bool? isDense = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, bool? isCollapsed = null, Color? iconColor = null, global::Doroti.Framework.Painting.TextStyle? prefixStyle = null, Color? prefixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? prefixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? suffixStyle = null, Color? suffixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? suffixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? counterStyle = null, bool? filled = null, Color? fillColor = null, global::Doroti.Framework.Painting.BorderSide? activeIndicatorBorder = null, global::Doroti.Framework.Painting.BorderSide? outlineBorder = null, Color? focusColor = null, Color? hoverColor = null, InputBorder? errorBorder = null, InputBorder? focusedBorder = null, InputBorder? focusedErrorBorder = null, InputBorder? disabledBorder = null, InputBorder? enabledBorder = null, InputBorder? border = null, bool? alignLabelWithHint = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, VisualDensity? visualDensity = null, InputDecorationThemeData? data = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key, child: child ?? SizedBox.CreateShrink())
    {
        _labelStyle = labelStyle;
        _floatingLabelStyle = floatingLabelStyle;
        _helperStyle = helperStyle;
        _helperMaxLines = helperMaxLines;
        _hintStyle = hintStyle;
        _hintFadeDuration = hintFadeDuration;
        _hintMaxLines = hintMaxLines;
        _errorStyle = errorStyle;
        _errorMaxLines = errorMaxLines;
        _floatingLabelBehavior = floatingLabelBehavior ?? FloatingLabelBehavior.auto;
        _floatingLabelAlignment = floatingLabelAlignment ?? FloatingLabelAlignment.start;
        _isDense = isDense ?? false;
        _contentPadding = contentPadding;
        _isCollapsed = isCollapsed ?? false;
        _iconColor = iconColor;
        _prefixStyle = prefixStyle;
        _prefixIconColor = prefixIconColor;
        _prefixIconConstraints = prefixIconConstraints;
        _suffixStyle = suffixStyle;
        _suffixIconColor = suffixIconColor;
        _suffixIconConstraints = suffixIconConstraints;
        _counterStyle = counterStyle;
        _filled = filled ?? false;
        _fillColor = fillColor;
        _activeIndicatorBorder = activeIndicatorBorder;
        _outlineBorder = outlineBorder;
        _focusColor = focusColor;
        _hoverColor = hoverColor;
        _errorBorder = errorBorder;
        _focusedBorder = focusedBorder;
        _focusedErrorBorder = focusedErrorBorder;
        _disabledBorder = disabledBorder;
        _enabledBorder = enabledBorder;
        _border = border;
        _alignLabelWithHint = alignLabelWithHint ?? false;
        _constraints = constraints;
        _visualDensity = visualDensity;
        _data = data;
        System.Diagnostics.Debug.Assert((data is null) || ((((((((((((((((((((((((((((((((((((object?)((labelStyle ?? floatingLabelStyle) ?? helperStyle) ?? helperMaxLines) ?? hintStyle) ?? hintFadeDuration) ?? hintMaxLines) ?? errorStyle) ?? errorMaxLines) ?? floatingLabelBehavior) ?? floatingLabelAlignment) ?? isDense) ?? contentPadding) ?? isCollapsed) ?? iconColor) ?? prefixStyle) ?? prefixIconColor) ?? prefixIconConstraints) ?? suffixStyle) ?? suffixIconColor) ?? suffixIconConstraints) ?? counterStyle) ?? filled) ?? fillColor) ?? activeIndicatorBorder) ?? outlineBorder) ?? focusColor) ?? hoverColor) ?? errorBorder) ?? focusedBorder) ?? focusedErrorBorder) ?? disabledBorder) ?? enabledBorder) ?? border) ?? alignLabelWithHint) ?? constraints) ?? visualDensity) is null));
    }

    public virtual global::Doroti.Framework.Painting.TextStyle? labelStyle => (_data is not null) ? _data.labelStyle : _labelStyle;
    public virtual global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle => (_data is not null) ? _data.floatingLabelStyle : _floatingLabelStyle;
    public virtual global::Doroti.Framework.Painting.TextStyle? helperStyle => (_data is not null) ? _data.helperStyle : _helperStyle;
    public virtual long? helperMaxLines => (_data is not null) ? _data.helperMaxLines : _helperMaxLines;
    public virtual global::Doroti.Framework.Painting.TextStyle? hintStyle => (_data is not null) ? _data.hintStyle : _hintStyle;
    public virtual Duration? hintFadeDuration => (_data is not null) ? _data.hintFadeDuration : _hintFadeDuration;
    public virtual long? hintMaxLines => (_data is not null) ? _data.hintMaxLines : _hintMaxLines;
    public virtual global::Doroti.Framework.Painting.TextStyle? errorStyle => (_data is not null) ? _data.errorStyle : _errorStyle;
    public virtual long? errorMaxLines => (_data is not null) ? _data.errorMaxLines : _errorMaxLines;
    public virtual FloatingLabelBehavior floatingLabelBehavior => (_data is not null) ? _data.floatingLabelBehavior : _floatingLabelBehavior;
    public virtual FloatingLabelAlignment floatingLabelAlignment => (_data is not null) ? _data.floatingLabelAlignment : _floatingLabelAlignment;
    public virtual bool isDense => (_data is not null) ? _data.isDense : _isDense;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding => (_data is not null) ? _data.contentPadding : _contentPadding;
    public virtual bool isCollapsed => (_data is not null) ? _data.isCollapsed : _isCollapsed;
    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.iconColor : _iconColor);
    public virtual global::Doroti.Framework.Painting.TextStyle? prefixStyle => (_data is not null) ? _data.prefixStyle : _prefixStyle;
    public virtual global::Doroti.Ui.Color? prefixIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.prefixIconColor : _prefixIconColor);
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? prefixIconConstraints => (_data is not null) ? _data.prefixIconConstraints : _prefixIconConstraints;
    public virtual global::Doroti.Framework.Painting.TextStyle? suffixStyle => (_data is not null) ? _data.suffixStyle : _suffixStyle;
    public virtual global::Doroti.Ui.Color? suffixIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.suffixIconColor : _suffixIconColor);
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? suffixIconConstraints => (_data is not null) ? _data.suffixIconConstraints : _suffixIconConstraints;
    public virtual global::Doroti.Framework.Painting.TextStyle? counterStyle => (_data is not null) ? _data.counterStyle : _counterStyle;
    public virtual bool filled => (_data is not null) ? _data.filled : _filled;
    public virtual global::Doroti.Ui.Color? fillColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.fillColor : _fillColor);
    public virtual global::Doroti.Framework.Painting.BorderSide? activeIndicatorBorder => (_data is not null) ? _data.activeIndicatorBorder : _activeIndicatorBorder;
    public virtual global::Doroti.Framework.Painting.BorderSide? outlineBorder => (_data is not null) ? _data.outlineBorder : _outlineBorder;
    public virtual global::Doroti.Ui.Color? focusColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.focusColor : _focusColor);
    public virtual global::Doroti.Ui.Color? hoverColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.hoverColor : _hoverColor);
    public virtual InputBorder? errorBorder => (_data is not null) ? _data.errorBorder : _errorBorder;
    public virtual InputBorder? focusedBorder => (_data is not null) ? _data.focusedBorder : _focusedBorder;
    public virtual InputBorder? focusedErrorBorder => (_data is not null) ? _data.focusedErrorBorder : _focusedErrorBorder;
    public virtual InputBorder? disabledBorder => (_data is not null) ? _data.disabledBorder : _disabledBorder;
    public virtual InputBorder? enabledBorder => (_data is not null) ? _data.enabledBorder : _enabledBorder;
    public virtual InputBorder? border => (_data is not null) ? _data.border : _border;
    public virtual bool alignLabelWithHint => (_data is not null) ? _data.alignLabelWithHint : _alignLabelWithHint;
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints => (_data is not null) ? _data.constraints : _constraints;
    public virtual VisualDensity? visualDensity => (_data is not null) ? _data.visualDensity : _visualDensity;
    public virtual InputDecorationThemeData data => DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>(_data ?? new InputDecorationThemeData(labelStyle: _labelStyle, floatingLabelStyle: _floatingLabelStyle, helperStyle: _helperStyle, helperMaxLines: _helperMaxLines, hintStyle: _hintStyle, hintFadeDuration: _hintFadeDuration, hintMaxLines: _hintMaxLines, errorStyle: _errorStyle, errorMaxLines: _errorMaxLines, floatingLabelBehavior: _floatingLabelBehavior, floatingLabelAlignment: _floatingLabelAlignment, isDense: _isDense, contentPadding: _contentPadding, isCollapsed: _isCollapsed, iconColor: _iconColor, prefixStyle: _prefixStyle, prefixIconColor: _prefixIconColor, prefixIconConstraints: _prefixIconConstraints, suffixStyle: _suffixStyle, suffixIconColor: _suffixIconColor, suffixIconConstraints: _suffixIconConstraints, counterStyle: _counterStyle, filled: _filled, fillColor: _fillColor, activeIndicatorBorder: _activeIndicatorBorder, outlineBorder: _outlineBorder, focusColor: _focusColor, hoverColor: _hoverColor, errorBorder: _errorBorder, focusedBorder: _focusedBorder, focusedErrorBorder: _focusedErrorBorder, disabledBorder: _disabledBorder, enabledBorder: _enabledBorder, border: _border, alignLabelWithHint: _alignLabelWithHint, constraints: _constraints, visualDensity: _visualDensity));
    public static InputDecorationThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        InputDecorationTheme? inputDecorationThemeLocal = context.dependOnInheritedWidgetOfExactType<InputDecorationTheme>();
        return inputDecorationThemeLocal?.data ?? Theme.of(context).inputDecorationTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InputDecorationTheme copyWith(global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle = null, global::Doroti.Framework.Painting.TextStyle? helperStyle = null, long? helperMaxLines = null, global::Doroti.Framework.Painting.TextStyle? hintStyle = null, Duration? hintFadeDuration = null, long? hintMaxLines = null, global::Doroti.Framework.Painting.TextStyle? errorStyle = null, long? errorMaxLines = null, FloatingLabelBehavior? floatingLabelBehavior = null, FloatingLabelAlignment? floatingLabelAlignment = null, bool? isDense = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, bool? isCollapsed = null, Color? iconColor = null, global::Doroti.Framework.Painting.TextStyle? prefixStyle = null, Color? prefixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? prefixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? suffixStyle = null, Color? suffixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? suffixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? counterStyle = null, bool? filled = null, Color? fillColor = null, global::Doroti.Framework.Painting.BorderSide? activeIndicatorBorder = null, global::Doroti.Framework.Painting.BorderSide? outlineBorder = null, Color? focusColor = null, Color? hoverColor = null, InputBorder? errorBorder = null, InputBorder? focusedBorder = null, InputBorder? focusedErrorBorder = null, InputBorder? disabledBorder = null, InputBorder? enabledBorder = null, InputBorder? border = null, bool? alignLabelWithHint = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, VisualDensity? visualDensity = null)
    {
        return new InputDecorationTheme(labelStyle: labelStyle ?? this.labelStyle, floatingLabelStyle: floatingLabelStyle ?? this.floatingLabelStyle, helperStyle: helperStyle ?? this.helperStyle, helperMaxLines: helperMaxLines ?? this.helperMaxLines, hintStyle: hintStyle ?? this.hintStyle, hintFadeDuration: hintFadeDuration ?? this.hintFadeDuration, hintMaxLines: hintMaxLines ?? this.hintMaxLines, errorStyle: errorStyle ?? this.errorStyle, errorMaxLines: errorMaxLines ?? this.errorMaxLines, floatingLabelBehavior: floatingLabelBehavior ?? this.floatingLabelBehavior, floatingLabelAlignment: floatingLabelAlignment ?? this.floatingLabelAlignment, isDense: isDense ?? this.isDense, contentPadding: contentPadding ?? this.contentPadding, iconColor: iconColor ?? this.iconColor, isCollapsed: isCollapsed ?? this.isCollapsed, prefixStyle: prefixStyle ?? this.prefixStyle, prefixIconColor: prefixIconColor ?? this.prefixIconColor, prefixIconConstraints: prefixIconConstraints ?? this.prefixIconConstraints, suffixStyle: suffixStyle ?? this.suffixStyle, suffixIconColor: suffixIconColor ?? this.suffixIconColor, suffixIconConstraints: suffixIconConstraints ?? this.suffixIconConstraints, counterStyle: counterStyle ?? this.counterStyle, filled: filled ?? this.filled, fillColor: fillColor ?? this.fillColor, activeIndicatorBorder: activeIndicatorBorder ?? this.activeIndicatorBorder, outlineBorder: outlineBorder ?? this.outlineBorder, focusColor: focusColor ?? this.focusColor, hoverColor: hoverColor ?? this.hoverColor, errorBorder: errorBorder ?? this.errorBorder, focusedBorder: focusedBorder ?? this.focusedBorder, focusedErrorBorder: focusedErrorBorder ?? this.focusedErrorBorder, disabledBorder: disabledBorder ?? this.disabledBorder, enabledBorder: enabledBorder ?? this.enabledBorder, border: border ?? this.border, alignLabelWithHint: alignLabelWithHint ?? this.alignLabelWithHint, constraints: constraints ?? this.constraints, visualDensity: visualDensity ?? this.visualDensity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InputDecorationTheme merge(InputDecorationTheme? other)
    {
        if (other is null)
        {
            return this;
        }
        return copyWith(labelStyle: labelStyle ?? other.labelStyle, floatingLabelStyle: floatingLabelStyle ?? other.floatingLabelStyle, helperStyle: helperStyle ?? other.helperStyle, helperMaxLines: helperMaxLines ?? other.helperMaxLines, hintStyle: hintStyle ?? other.hintStyle, hintFadeDuration: hintFadeDuration ?? other.hintFadeDuration, hintMaxLines: hintMaxLines ?? other.hintMaxLines, errorStyle: errorStyle ?? other.errorStyle, errorMaxLines: errorMaxLines ?? other.errorMaxLines, contentPadding: contentPadding ?? other.contentPadding, iconColor: iconColor ?? other.iconColor, prefixStyle: prefixStyle ?? other.prefixStyle, prefixIconColor: prefixIconColor ?? other.prefixIconColor, prefixIconConstraints: prefixIconConstraints ?? other.prefixIconConstraints, suffixStyle: suffixStyle ?? other.suffixStyle, suffixIconColor: suffixIconColor ?? other.suffixIconColor, suffixIconConstraints: suffixIconConstraints ?? other.suffixIconConstraints, counterStyle: counterStyle ?? other.counterStyle, fillColor: fillColor ?? other.fillColor, activeIndicatorBorder: activeIndicatorBorder ?? other.activeIndicatorBorder, outlineBorder: outlineBorder ?? other.outlineBorder, focusColor: focusColor ?? other.focusColor, hoverColor: hoverColor ?? other.hoverColor, errorBorder: errorBorder ?? other.errorBorder, focusedBorder: focusedBorder ?? other.focusedBorder, focusedErrorBorder: focusedErrorBorder ?? other.focusedErrorBorder, disabledBorder: disabledBorder ?? other.disabledBorder, enabledBorder: enabledBorder ?? other.enabledBorder, border: border ?? other.border, constraints: constraints ?? other.constraints, visualDensity: visualDensity ?? other.visualDensity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((InputDecorationTheme)oldWidget).data));
    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return new InputDecorationTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

}

public class InputDecorationThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Framework.Painting.TextStyle? labelStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? helperStyle { get; private set; }
    public virtual long? helperMaxLines { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? hintStyle { get; private set; }
    public virtual Duration? hintFadeDuration { get; private set; }
    public virtual long? hintMaxLines { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? errorStyle { get; private set; }
    public virtual long? errorMaxLines { get; private set; }
    public virtual FloatingLabelBehavior floatingLabelBehavior { get; private set; } = default!;
    public virtual FloatingLabelAlignment floatingLabelAlignment { get; private set; } = default!;
    public virtual bool isDense { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual bool isCollapsed { get; private set; } = default!;
    public virtual Color? iconColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? prefixStyle { get; private set; }
    public virtual Color? prefixIconColor { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? prefixIconConstraints { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? suffixStyle { get; private set; }
    public virtual Color? suffixIconColor { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? suffixIconConstraints { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? counterStyle { get; private set; }
    public virtual bool filled { get; private set; } = default!;
    public virtual Color? fillColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? outlineBorder { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? activeIndicatorBorder { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual InputBorder? errorBorder { get; private set; }
    public virtual InputBorder? focusedBorder { get; private set; }
    public virtual InputBorder? focusedErrorBorder { get; private set; }
    public virtual InputBorder? disabledBorder { get; private set; }
    public virtual InputBorder? enabledBorder { get; private set; }
    public virtual InputBorder? border { get; private set; }
    public virtual bool alignLabelWithHint { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }

    public InputDecorationThemeData(global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle = null, global::Doroti.Framework.Painting.TextStyle? helperStyle = null, long? helperMaxLines = null, global::Doroti.Framework.Painting.TextStyle? hintStyle = null, Duration? hintFadeDuration = null, long? hintMaxLines = null, global::Doroti.Framework.Painting.TextStyle? errorStyle = null, long? errorMaxLines = null, FloatingLabelBehavior floatingLabelBehavior = FloatingLabelBehavior.auto, FloatingLabelAlignment floatingLabelAlignment = default!, bool isDense = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, bool isCollapsed = false, Color? iconColor = null, global::Doroti.Framework.Painting.TextStyle? prefixStyle = null, Color? prefixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? prefixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? suffixStyle = null, Color? suffixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? suffixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? counterStyle = null, bool filled = false, Color? fillColor = null, global::Doroti.Framework.Painting.BorderSide? activeIndicatorBorder = null, global::Doroti.Framework.Painting.BorderSide? outlineBorder = null, Color? focusColor = null, Color? hoverColor = null, InputBorder? errorBorder = null, InputBorder? focusedBorder = null, InputBorder? focusedErrorBorder = null, InputBorder? disabledBorder = null, InputBorder? enabledBorder = null, InputBorder? border = null, bool alignLabelWithHint = false, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, VisualDensity? visualDensity = null)
    {
        FloatingLabelAlignment __floatingLabelAlignment = floatingLabelAlignment ?? FloatingLabelAlignment.start;
        this.labelStyle = labelStyle;
        this.floatingLabelStyle = floatingLabelStyle;
        this.helperStyle = helperStyle;
        this.helperMaxLines = helperMaxLines;
        this.hintStyle = hintStyle;
        this.hintFadeDuration = hintFadeDuration;
        this.hintMaxLines = hintMaxLines;
        this.errorStyle = errorStyle;
        this.errorMaxLines = errorMaxLines;
        this.floatingLabelBehavior = floatingLabelBehavior;
        this.floatingLabelAlignment = __floatingLabelAlignment;
        this.isDense = isDense;
        this.contentPadding = contentPadding;
        this.isCollapsed = isCollapsed;
        this.iconColor = iconColor;
        this.prefixStyle = prefixStyle;
        this.prefixIconColor = prefixIconColor;
        this.prefixIconConstraints = prefixIconConstraints;
        this.suffixStyle = suffixStyle;
        this.suffixIconColor = suffixIconColor;
        this.suffixIconConstraints = suffixIconConstraints;
        this.counterStyle = counterStyle;
        this.filled = filled;
        this.fillColor = fillColor;
        this.activeIndicatorBorder = activeIndicatorBorder;
        this.outlineBorder = outlineBorder;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.errorBorder = errorBorder;
        this.focusedBorder = focusedBorder;
        this.focusedErrorBorder = focusedErrorBorder;
        this.disabledBorder = disabledBorder;
        this.enabledBorder = enabledBorder;
        this.border = border;
        this.alignLabelWithHint = alignLabelWithHint;
        this.constraints = constraints;
        this.visualDensity = visualDensity;
    }

    public virtual InputDecorationThemeData copyWith(global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle = null, global::Doroti.Framework.Painting.TextStyle? helperStyle = null, long? helperMaxLines = null, global::Doroti.Framework.Painting.TextStyle? hintStyle = null, Duration? hintFadeDuration = null, long? hintMaxLines = null, global::Doroti.Framework.Painting.TextStyle? errorStyle = null, long? errorMaxLines = null, FloatingLabelBehavior? floatingLabelBehavior = null, FloatingLabelAlignment? floatingLabelAlignment = null, bool? isDense = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, bool? isCollapsed = null, Color? iconColor = null, global::Doroti.Framework.Painting.TextStyle? prefixStyle = null, Color? prefixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? prefixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? suffixStyle = null, Color? suffixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? suffixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? counterStyle = null, bool? filled = null, Color? fillColor = null, global::Doroti.Framework.Painting.BorderSide? activeIndicatorBorder = null, global::Doroti.Framework.Painting.BorderSide? outlineBorder = null, Color? focusColor = null, Color? hoverColor = null, InputBorder? errorBorder = null, InputBorder? focusedBorder = null, InputBorder? focusedErrorBorder = null, InputBorder? disabledBorder = null, InputBorder? enabledBorder = null, InputBorder? border = null, bool? alignLabelWithHint = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, VisualDensity? visualDensity = null)
    {
        return new InputDecorationThemeData(labelStyle: labelStyle ?? this.labelStyle, floatingLabelStyle: floatingLabelStyle ?? this.floatingLabelStyle, helperStyle: helperStyle ?? this.helperStyle, helperMaxLines: helperMaxLines ?? this.helperMaxLines, hintStyle: hintStyle ?? this.hintStyle, hintFadeDuration: hintFadeDuration ?? this.hintFadeDuration, hintMaxLines: hintMaxLines ?? this.hintMaxLines, errorStyle: errorStyle ?? this.errorStyle, errorMaxLines: errorMaxLines ?? this.errorMaxLines, floatingLabelBehavior: floatingLabelBehavior ?? this.floatingLabelBehavior, floatingLabelAlignment: floatingLabelAlignment ?? this.floatingLabelAlignment, isDense: isDense ?? this.isDense, contentPadding: contentPadding ?? this.contentPadding, iconColor: iconColor ?? this.iconColor, isCollapsed: isCollapsed ?? this.isCollapsed, prefixStyle: prefixStyle ?? this.prefixStyle, prefixIconColor: prefixIconColor ?? this.prefixIconColor, prefixIconConstraints: prefixIconConstraints ?? this.prefixIconConstraints, suffixStyle: suffixStyle ?? this.suffixStyle, suffixIconColor: suffixIconColor ?? this.suffixIconColor, suffixIconConstraints: suffixIconConstraints ?? this.suffixIconConstraints, counterStyle: counterStyle ?? this.counterStyle, filled: filled ?? this.filled, fillColor: fillColor ?? this.fillColor, activeIndicatorBorder: activeIndicatorBorder ?? this.activeIndicatorBorder, outlineBorder: outlineBorder ?? this.outlineBorder, focusColor: focusColor ?? this.focusColor, hoverColor: hoverColor ?? this.hoverColor, errorBorder: errorBorder ?? this.errorBorder, focusedBorder: focusedBorder ?? this.focusedBorder, focusedErrorBorder: focusedErrorBorder ?? this.focusedErrorBorder, disabledBorder: disabledBorder ?? this.disabledBorder, enabledBorder: enabledBorder ?? this.enabledBorder, border: border ?? this.border, alignLabelWithHint: alignLabelWithHint ?? this.alignLabelWithHint, constraints: constraints ?? this.constraints, visualDensity: visualDensity ?? this.visualDensity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InputDecorationThemeData merge(InputDecorationThemeData? other)
    {
        if (other is null)
        {
            return this;
        }
        return copyWith(labelStyle: labelStyle ?? other.labelStyle, floatingLabelStyle: floatingLabelStyle ?? other.floatingLabelStyle, helperStyle: helperStyle ?? other.helperStyle, helperMaxLines: helperMaxLines ?? other.helperMaxLines, hintStyle: hintStyle ?? other.hintStyle, hintFadeDuration: hintFadeDuration ?? other.hintFadeDuration, hintMaxLines: hintMaxLines ?? other.hintMaxLines, errorStyle: errorStyle ?? other.errorStyle, errorMaxLines: errorMaxLines ?? other.errorMaxLines, contentPadding: contentPadding ?? other.contentPadding, iconColor: iconColor ?? other.iconColor, prefixStyle: prefixStyle ?? other.prefixStyle, prefixIconColor: prefixIconColor ?? other.prefixIconColor, prefixIconConstraints: prefixIconConstraints ?? other.prefixIconConstraints, suffixStyle: suffixStyle ?? other.suffixStyle, suffixIconColor: suffixIconColor ?? other.suffixIconColor, suffixIconConstraints: suffixIconConstraints ?? other.suffixIconConstraints, counterStyle: counterStyle ?? other.counterStyle, fillColor: fillColor ?? other.fillColor, activeIndicatorBorder: activeIndicatorBorder ?? other.activeIndicatorBorder, outlineBorder: outlineBorder ?? other.outlineBorder, focusColor: focusColor ?? other.focusColor, hoverColor: hoverColor ?? other.hoverColor, errorBorder: errorBorder ?? other.errorBorder, focusedBorder: focusedBorder ?? other.focusedBorder, focusedErrorBorder: focusedErrorBorder ?? other.focusedErrorBorder, disabledBorder: disabledBorder ?? other.disabledBorder, enabledBorder: enabledBorder ?? other.enabledBorder, border: border ?? other.border, constraints: constraints ?? other.constraints, visualDensity: visualDensity ?? other.visualDensity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(labelStyle, floatingLabelStyle, helperStyle, helperMaxLines, hintStyle, hintMaxLines, errorStyle, errorMaxLines, floatingLabelBehavior, floatingLabelAlignment, isDense, contentPadding, isCollapsed, iconColor, prefixStyle, prefixIconColor, prefixIconConstraints, suffixStyle, suffixIconColor, FoundationRuntimePorts.ObjectHash(suffixIconConstraints, counterStyle, filled, fillColor, activeIndicatorBorder, outlineBorder, focusColor, hoverColor, errorBorder, focusedBorder, focusedErrorBorder, disabledBorder, enabledBorder, border, alignLabelWithHint, constraints, hintFadeDuration, visualDensity)));
    public override bool Equals(object? other)
    {
        var __other = other as InputDecorationThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is InputDecorationThemeData) && Equals(__other.labelStyle, labelStyle) && Equals(__other.floatingLabelStyle, floatingLabelStyle) && Equals(__other.helperStyle, helperStyle) && (__other.helperMaxLines == helperMaxLines) && Equals(__other.hintStyle, hintStyle) && Equals(__other.hintFadeDuration, hintFadeDuration) && Equals(__other.errorStyle, errorStyle) && (__other.errorMaxLines == errorMaxLines) && (__other.isDense == isDense) && Equals(__other.contentPadding, contentPadding) && (__other.isCollapsed == isCollapsed) && Equals(__other.iconColor, iconColor) && Equals(__other.prefixStyle, prefixStyle) && Equals(__other.prefixIconColor, prefixIconColor) && Equals(__other.prefixIconConstraints, prefixIconConstraints) && Equals(__other.suffixStyle, suffixStyle) && Equals(__other.suffixIconColor, suffixIconColor) && Equals(__other.suffixIconConstraints, suffixIconConstraints) && Equals(__other.counterStyle, counterStyle) && Equals(__other.floatingLabelBehavior, floatingLabelBehavior) && Equals(__other.floatingLabelAlignment, floatingLabelAlignment) && (__other.filled == filled) && Equals(__other.fillColor, fillColor) && Equals(__other.activeIndicatorBorder, activeIndicatorBorder) && Equals(__other.outlineBorder, outlineBorder) && Equals(__other.focusColor, focusColor) && Equals(__other.hoverColor, hoverColor) && Equals(__other.errorBorder, errorBorder) && Equals(__other.focusedBorder, focusedBorder) && Equals(__other.focusedErrorBorder, focusedErrorBorder) && Equals(__other.disabledBorder, disabledBorder) && Equals(__other.enabledBorder, enabledBorder) && Equals(__other.border, border) && (__other.hintMaxLines == hintMaxLines) && (__other.alignLabelWithHint == alignLabelWithHint) && Equals(__other.constraints, constraints) && Equals(__other.disabledBorder, disabledBorder) && Equals(__other.visualDensity, visualDensity);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultTheme = new InputDecorationThemeData();
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("labelStyle", labelStyle, defaultValue: defaultTheme.labelStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("floatingLabelStyle", floatingLabelStyle, defaultValue: defaultTheme.floatingLabelStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("helperStyle", helperStyle, defaultValue: defaultTheme.helperStyle));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("helperMaxLines", helperMaxLines, defaultValue: defaultTheme.helperMaxLines));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("hintStyle", hintStyle, defaultValue: defaultTheme.hintStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("hintFadeDuration", hintFadeDuration, defaultValue: defaultTheme.hintFadeDuration));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("hintMaxLines", hintMaxLines, defaultValue: defaultTheme.hintMaxLines));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("errorStyle", errorStyle, defaultValue: defaultTheme.errorStyle));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("errorMaxLines", errorMaxLines, defaultValue: defaultTheme.errorMaxLines));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FloatingLabelBehavior>("floatingLabelBehavior", floatingLabelBehavior, defaultValue: defaultTheme.floatingLabelBehavior));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FloatingLabelAlignment>("floatingLabelAlignment", floatingLabelAlignment, defaultValue: defaultTheme.floatingLabelAlignment));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("isDense", isDense, defaultValue: defaultTheme.isDense));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("contentPadding", contentPadding, defaultValue: defaultTheme.contentPadding));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("isCollapsed", isCollapsed, defaultValue: defaultTheme.isCollapsed));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color>("iconColor", iconColor, defaultValue: defaultTheme.iconColor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color>("prefixIconColor", prefixIconColor, defaultValue: defaultTheme.prefixIconColor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("prefixIconConstraints", prefixIconConstraints, defaultValue: defaultTheme.prefixIconConstraints));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("prefixStyle", prefixStyle, defaultValue: defaultTheme.prefixStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color>("suffixIconColor", suffixIconColor, defaultValue: defaultTheme.suffixIconColor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("suffixIconConstraints", suffixIconConstraints, defaultValue: defaultTheme.suffixIconConstraints));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("suffixStyle", suffixStyle, defaultValue: defaultTheme.suffixStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("counterStyle", counterStyle, defaultValue: defaultTheme.counterStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("filled", filled, defaultValue: defaultTheme.filled));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("fillColor", fillColor, defaultValue: defaultTheme.fillColor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderSide>("activeIndicatorBorder", activeIndicatorBorder, defaultValue: defaultTheme.activeIndicatorBorder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderSide>("outlineBorder", outlineBorder, defaultValue: defaultTheme.outlineBorder));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", focusColor, defaultValue: defaultTheme.focusColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", hoverColor, defaultValue: defaultTheme.hoverColor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputBorder>("errorBorder", errorBorder, defaultValue: defaultTheme.errorBorder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputBorder>("focusedBorder", focusedBorder, defaultValue: defaultTheme.focusedErrorBorder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputBorder>("focusedErrorBorder", focusedErrorBorder, defaultValue: defaultTheme.focusedErrorBorder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputBorder>("disabledBorder", disabledBorder, defaultValue: defaultTheme.disabledBorder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputBorder>("enabledBorder", enabledBorder, defaultValue: defaultTheme.enabledBorder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputBorder>("border", border, defaultValue: defaultTheme.border));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("alignLabelWithHint", alignLabelWithHint, defaultValue: defaultTheme.alignLabelWithHint));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("constraints", constraints, defaultValue: defaultTheme.constraints));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", visualDensity, defaultValue: defaultTheme.visualDensity));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _InputDecoratorDefaultsM3__input_decorator : InputDecorationThemeData
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
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = Theme.of(context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _InputDecoratorDefaultsM3__input_decorator(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public override global::Doroti.Framework.Painting.TextStyle? hintStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(WidgetStateTextStyle.CreateResolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return new global::Doroti.Framework.Painting.TextStyle(color: _colors.onSurface.withOpacity(0.38));
        }
        return new global::Doroti.Framework.Painting.TextStyle(color: _colors.onSurfaceVariant);
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override Color? fillColor => DartRuntimePrimitives.ConvertValue<Color>(WidgetStateColor.CreateResolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return _colors.onSurface.withOpacity(0.04);
        }
        return _colors.surfaceContainerHighest;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Painting.BorderSide? activeIndicatorBorder => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.BorderSide>(WidgetStateBorderSide.CreateResolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: _colors.onSurface.withOpacity(0.38));
        }
        if (states.Contains(WidgetState.error))
        {
            if (states.Contains(WidgetState.focused))
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: _colors.error, width: 2.0);
            }
            if (states.Contains(WidgetState.hovered))
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: _colors.onErrorContainer);
            }
            return new global::Doroti.Framework.Painting.BorderSide(color: _colors.error);
        }
        if (states.Contains(WidgetState.focused))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: _colors.primary, width: 2.0);
        }
        if (states.Contains(WidgetState.hovered))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: _colors.onSurface);
        }
        return new global::Doroti.Framework.Painting.BorderSide(color: _colors.onSurfaceVariant);
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Painting.BorderSide? outlineBorder => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.BorderSide>(WidgetStateBorderSide.CreateResolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: _colors.onSurface.withOpacity(0.12));
        }
        if (states.Contains(WidgetState.error))
        {
            if (states.Contains(WidgetState.focused))
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: _colors.error, width: 2.0);
            }
            if (states.Contains(WidgetState.hovered))
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: _colors.onErrorContainer);
            }
            return new global::Doroti.Framework.Painting.BorderSide(color: _colors.error);
        }
        if (states.Contains(WidgetState.focused))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: _colors.primary, width: 2.0);
        }
        if (states.Contains(WidgetState.hovered))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: _colors.onSurface);
        }
        return new global::Doroti.Framework.Painting.BorderSide(color: _colors.outline);
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override Color? iconColor => _colors.onSurfaceVariant;
    public override Color? prefixIconColor => DartRuntimePrimitives.ConvertValue<Color>(WidgetStateColor.CreateResolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return _colors.onSurface.withOpacity(0.38);
        }
        return _colors.onSurfaceVariant;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override Color? suffixIconColor => DartRuntimePrimitives.ConvertValue<Color>(WidgetStateColor.CreateResolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return _colors.onSurface.withOpacity(0.38);
        }
        if (states.Contains(WidgetState.error))
        {
            if (states.Contains(WidgetState.hovered))
            {
                return _colors.onErrorContainer;
            }
            return _colors.error;
        }
        return _colors.onSurfaceVariant;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Painting.TextStyle? labelStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(WidgetStateTextStyle.CreateResolveWith((states) =>
    {
        global::Doroti.Framework.Painting.TextStyle textStyle = _textTheme.bodyLarge ?? new global::Doroti.Framework.Painting.TextStyle();
        if (states.Contains(WidgetState.disabled))
        {
            return textStyle.copyWith(color: _colors.onSurface.withOpacity(0.38));
        }
        if (states.Contains(WidgetState.error))
        {
            if (states.Contains(WidgetState.focused))
            {
                return textStyle.copyWith(color: _colors.error);
            }
            if (states.Contains(WidgetState.hovered))
            {
                return textStyle.copyWith(color: _colors.onErrorContainer);
            }
            return textStyle.copyWith(color: _colors.error);
        }
        if (states.Contains(WidgetState.focused))
        {
            return textStyle.copyWith(color: _colors.primary);
        }
        if (states.Contains(WidgetState.hovered))
        {
            return textStyle.copyWith(color: _colors.onSurfaceVariant);
        }
        return textStyle.copyWith(color: _colors.onSurfaceVariant);
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(WidgetStateTextStyle.CreateResolveWith((states) =>
    {
        global::Doroti.Framework.Painting.TextStyle textStyle = _textTheme.bodyLarge ?? new global::Doroti.Framework.Painting.TextStyle();
        if (states.Contains(WidgetState.disabled))
        {
            return textStyle.copyWith(color: _colors.onSurface.withOpacity(0.38));
        }
        if (states.Contains(WidgetState.error))
        {
            if (states.Contains(WidgetState.focused))
            {
                return textStyle.copyWith(color: _colors.error);
            }
            if (states.Contains(WidgetState.hovered))
            {
                return textStyle.copyWith(color: _colors.onErrorContainer);
            }
            return textStyle.copyWith(color: _colors.error);
        }
        if (states.Contains(WidgetState.focused))
        {
            return textStyle.copyWith(color: _colors.primary);
        }
        if (states.Contains(WidgetState.hovered))
        {
            return textStyle.copyWith(color: _colors.onSurfaceVariant);
        }
        return textStyle.copyWith(color: _colors.onSurfaceVariant);
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Painting.TextStyle? helperStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(WidgetStateTextStyle.CreateResolveWith((states) =>
    {
        global::Doroti.Framework.Painting.TextStyle textStyle = _textTheme.bodySmall ?? new global::Doroti.Framework.Painting.TextStyle();
        if (states.Contains(WidgetState.disabled))
        {
            return textStyle.copyWith(color: _colors.onSurface.withOpacity(0.38));
        }
        return textStyle.copyWith(color: _colors.onSurfaceVariant);
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Painting.TextStyle? errorStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(WidgetStateTextStyle.CreateResolveWith((states) =>
    {
        global::Doroti.Framework.Painting.TextStyle textStyle = _textTheme.bodySmall ?? new global::Doroti.Framework.Painting.TextStyle();
        return textStyle.copyWith(color: _colors.error);
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
}
