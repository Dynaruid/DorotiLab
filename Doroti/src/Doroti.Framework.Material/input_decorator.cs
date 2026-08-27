// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/input_decorator.dart
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

public static partial class Input_decoratorLibrary
{
    internal static Duration _kTransitionDuration = Duration.Create(milliseconds: 167L);
}

public static partial class Input_decoratorLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kTransitionCurve = ((global::Doroti.Framework.Animation.Curve)(object?)global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
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
        get => this._start;
        set
        {
            var __value = value;
            if ((__value != this._start))
            {
                _start = __value;
                notifyListeners();
            }
        }
    }
    public virtual double extent
    {
        get => this._extent;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) != this._extent))
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is _InputBorderGap__input_decorator) && (((_InputBorderGap__input_decorator)((_InputBorderGap__input_decorator)__other)).start == this.start)) && (((_InputBorderGap__input_decorator)((_InputBorderGap__input_decorator)__other)).extent == this.extent));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.start, this.extent));
    public override string ToString() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
}

public class _InputBorderTween__input_decorator : global::Doroti.Framework.Animation.Tween<InputBorder>
{
    internal _InputBorderTween__input_decorator(InputBorder? begin = null, InputBorder? end = null) : base(begin: begin, end: end)
    {
    }

    public override InputBorder lerp(double t) => ((InputBorder?)(object?)ShapeBorder.lerp(this.begin, this.end, t)!)!;
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

    public virtual global::Doroti.Ui.Color blendedColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Dart_uiLibrary.Color.alphaBlend(this.hoverColorTween.evaluate(this.hoverAnimation)!, this.fillColor));
    public override void paint(Canvas canvas, Size size)
    {
        InputBorder borderValue__4594 = ((InputBorder)(object?)this.border.evaluate(this.borderAnimation));
        global::Doroti.Ui.Rect canvasRect__4657 = ((global::Doroti.Ui.Rect)(object?)(Offset.zero & size));
        global::Doroti.Ui.Color blendedFillColor__4706 = ((global::Doroti.Ui.Color)(object?)this.blendedColor);
        if ((blendedFillColor__4706.alpha > 0L))
        {
            var paint__4789 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = blendedFillColor__4706;
    __cascade.style = PaintingStyle.fill;
    return __cascade;
}))();
            if (borderValue__4594.preferPaintInterior)
            {
                borderValue__4594.paintInterior(canvas, canvasRect__4657, paint__4789, textDirection: this.textDirection);
            }
            else
            {
                canvas.drawPath(borderValue__4594.getOuterPath(canvasRect__4657, textDirection: this.textDirection), paint__4789);
            }
        }
        borderValue__4594.paint(canvas, canvasRect__4657, gapStart: (((_InputBorderGap__input_decorator)this.gap).start ?? 0.0), gapExtent: ((_InputBorderGap__input_decorator)this.gap).extent, gapPercentage: ((global::Doroti.Framework.Animation.Animation<double>)this.gapAnimation).value, textDirection: this.textDirection);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_InputBorderPainter__input_decorator)(object)oldDelegate;
        return ((((((!object.Equals(this.borderAnimation, ((_InputBorderPainter__input_decorator)__oldPainter).borderAnimation)) || (!object.Equals(this.hoverAnimation, ((_InputBorderPainter__input_decorator)__oldPainter).hoverAnimation))) || (!object.Equals(this.gapAnimation, ((_InputBorderPainter__input_decorator)__oldPainter).gapAnimation))) || (!object.Equals(this.border, ((_InputBorderPainter__input_decorator)__oldPainter).border))) || (!object.Equals(this.gap, ((_InputBorderPainter__input_decorator)__oldPainter).gap))) || (!object.Equals(this.textDirection, ((_InputBorderPainter__input_decorator)__oldPainter).textDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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
        _hoverColorController = new global::Doroti.Framework.Animation.AnimationController(duration: _kHoverDuration, value: (((_BorderContainer__input_decorator)this.widget).isHovering ? 1.0 : 0.0), vsync: this);
        _controller = new global::Doroti.Framework.Animation.AnimationController(duration: Input_decoratorLibrary._kTransitionDuration, vsync: this);
        _borderAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._controller, curve: Input_decoratorLibrary._kTransitionCurve, reverseCurve: ((global::Doroti.Framework.Animation.Curve)Input_decoratorLibrary._kTransitionCurve).flipped);
        _border = new _InputBorderTween__input_decorator(begin: ((_BorderContainer__input_decorator)this.widget).border, end: ((_BorderContainer__input_decorator)this.widget).border);
        _hoverAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._hoverColorController, curve: global::Doroti.Framework.Animation.Curves.linear);
        _hoverColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: ((_BorderContainer__input_decorator)this.widget).hoverColor.withAlpha(0L), end: ((_BorderContainer__input_decorator)this.widget).hoverColor);
    }

    public override void dispose()
    {
        this._controller.dispose();
        this._hoverColorController.dispose();
        this._borderAnimation.dispose();
        this._hoverAnimation.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(_BorderContainer__input_decorator oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_BorderContainer__input_decorator)this.widget).border, ((_BorderContainer__input_decorator)oldWidget).border)))
        {
            _border = new _InputBorderTween__input_decorator(begin: ((_BorderContainer__input_decorator)oldWidget).border, end: ((_BorderContainer__input_decorator)this.widget).border);
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = this._controller;
    __cascade.value = 0.0;
    __cascade.forward();
    return __cascade;
}))());
        }
        if ((!object.Equals(((_BorderContainer__input_decorator)this.widget).hoverColor, ((_BorderContainer__input_decorator)oldWidget).hoverColor)))
        {
            _hoverColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: ((_BorderContainer__input_decorator)this.widget).hoverColor.withAlpha(0L), end: ((_BorderContainer__input_decorator)this.widget).hoverColor);
        }
        if ((((_BorderContainer__input_decorator)this.widget).isHovering != ((_BorderContainer__input_decorator)oldWidget).isHovering))
        {
            if (((_BorderContainer__input_decorator)this.widget).isHovering)
            {
                this._hoverColorController.forward();
            }
            else
            {
                this._hoverColorController.reverse();
            }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CustomPaint(foregroundPainter: new _InputBorderPainter__input_decorator(repaint: global::Doroti.Framework.Foundation.Listenable.CreateMerge(new List<global::Doroti.Framework.Foundation.Listenable> { this._borderAnimation, ((_BorderContainer__input_decorator)this.widget).gap, this._hoverColorController }.Cast<global::Doroti.Framework.Foundation.Listenable?>()), borderAnimation: this._borderAnimation, border: this._border, gapAnimation: ((_BorderContainer__input_decorator)this.widget).gapAnimation, gap: ((_BorderContainer__input_decorator)this.widget).gap, textDirection: Directionality.of(context), fillColor: ((_BorderContainer__input_decorator)this.widget).fillColor, hoverColorTween: this._hoverColorTween, hoverAnimation: this._hoverAnimation)));
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
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values__17506).enabled;
    __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
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
    public static global::Doroti.Framework.Widgets.Widget empty = ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.Widget? _helper { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.Widget? _error { get; set; } = default;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual bool _hasHelper => DartRuntimePrimitives.ConvertValue<bool>(((((_HelperError__input_decorator)this.widget).helperText is not null) || (((_HelperError__input_decorator)this.widget).helper is not null)));
    internal virtual bool _hasError => DartRuntimePrimitives.ConvertValue<bool>(((((_HelperError__input_decorator)this.widget).errorText is not null) || (((_HelperError__input_decorator)this.widget).error is not null)));
    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(duration: Input_decoratorLibrary._kTransitionDuration, vsync: this);
        if (this._hasError)
        {
            _error = _buildError();
            this._controller.value = 1.0;
        }
        else
        {
            if (this._hasHelper)
            {
                _helper = _buildHelper();
            }
        }
        this._controller.addListener(() => this._handleChange());
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

    internal virtual void _handleChange()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public override void didUpdateWidget(_HelperError__input_decorator old)
    {
        base.didUpdateWidget(old);
        global::Doroti.Framework.Widgets.Widget? newError__11141 = ((_HelperError__input_decorator)this.widget).error;
        string? newErrorText__11184 = ((_HelperError__input_decorator)this.widget).errorText;
        global::Doroti.Framework.Widgets.Widget? newHelper__11235 = ((_HelperError__input_decorator)this.widget).helper;
        string? newHelperText__11280 = ((_HelperError__input_decorator)this.widget).helperText;
        global::Doroti.Framework.Widgets.Widget? oldError__11333 = ((_HelperError__input_decorator)old).error;
        string? oldErrorText__11373 = ((_HelperError__input_decorator)old).errorText;
        global::Doroti.Framework.Widgets.Widget? oldHelper__11421 = ((_HelperError__input_decorator)old).helper;
        string? oldHelperText__11463 = ((_HelperError__input_decorator)old).helperText;
        var errorStateChanged__11506 = (((newError__11141 is not null)) != ((oldError__11333 is not null)));
        var errorTextStateChanged__11578 = (((newErrorText__11184 is not null)) != ((oldErrorText__11373 is not null)));
        var helperStateChanged__11662 = (((newHelper__11235 is not null)) != ((oldHelper__11421 is not null)));
        bool helperTextStateChanged__11742 = ((newErrorText__11184 is null) && (((newHelperText__11280 is not null)) != ((oldHelperText__11463 is not null))));
        if ((((errorStateChanged__11506 || errorTextStateChanged__11578) || helperStateChanged__11662) || helperTextStateChanged__11742))
        {
            if (((newError__11141 is not null) || (newErrorText__11184 is not null)))
            {
                _error = _buildError();
                this._controller.forward();
            }
            else
            {
                if (((newHelper__11235 is not null) || (newHelperText__11280 is not null)))
                {
                    _helper = _buildHelper();
                    this._controller.reverse();
                }
                else
                {
                    this._controller.reverse();
                }
            }
        }
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildHelper()
    {
        DartRuntimePrimitives.Assert(() => ((((_HelperError__input_decorator)this.widget).helper is not null) || (((_HelperError__input_decorator)this.widget).helperText is not null)));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).animate(this._controller), child: (((_HelperError__input_decorator)this.widget).helper ?? new global::Doroti.Framework.Widgets.Text(((_HelperError__input_decorator)this.widget).helperText!, style: ((_HelperError__input_decorator)this.widget).helperStyle, textAlign: ((_HelperError__input_decorator)this.widget).textAlign, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, maxLines: ((_HelperError__input_decorator)this.widget).helperMaxLines)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildError()
    {
        DartRuntimePrimitives.Assert(() => ((((_HelperError__input_decorator)this.widget).error is not null) || (((_HelperError__input_decorator)this.widget).errorText is not null)));
        string? capturedErrorText__12942 = ((_HelperError__input_decorator)this.widget).errorText;
        global::Doroti.Framework.Widgets.Widget? capturedError__12992 = ((_HelperError__input_decorator)this.widget).error;
        if (((capturedError__12992 is not null) && (((_HelperError__input_decorator)this.widget).errorStyle is not null)))
        {
            capturedError__12992 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: ((_HelperError__input_decorator)this.widget).errorStyle!, child: capturedError__12992));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, liveRegion: !MediaQuery.supportsAnnounceOf(context), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: this._controller, child: new global::Doroti.Framework.Widgets.FractionalTranslation(translation: new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.0, -0.25), end: Offset.zero).evaluate(((global::Doroti.Framework.Animation.AnimationController)this._controller).view), child: (capturedError__12992 ?? new global::Doroti.Framework.Widgets.Text(capturedErrorText__12942!, style: ((_HelperError__input_decorator)this.widget).errorStyle, textAlign: ((_HelperError__input_decorator)this.widget).textAlign, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, maxLines: ((_HelperError__input_decorator)this.widget).errorMaxLines))))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (this._controller.isDismissed)
        {
            _error = null;
            if (this._hasHelper)
            {
                return _helper = _buildHelper();
            }
            else
            {
                _helper = null;
                return empty;
            }
        }
        if (this._controller.isCompleted)
        {
            _helper = null;
            if (this._hasError)
            {
                return _error = _buildError();
            }
            else
            {
                _error = null;
                return empty;
            }
        }
        if (((this._helper is null) && this._hasError))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildError());
        }
        if (((this._error is null) && this._hasHelper))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildHelper());
        }
        if (this._hasError)
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).animate(this._controller), child: this._helper)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildError()) }));
        }
        if (this._hasHelper)
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildHelper()), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: this._controller, child: this._error)) }));
        }
        return empty;
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
        System.Diagnostics.Debug.Assert(((_x >= -1.0) && (_x <= 1.0)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(this._x.GetHashCode());
    public override bool Equals(object? other)
    {
        var __other = other as FloatingLabelAlignment;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is FloatingLabelAlignment) && (this._x == ((FloatingLabelAlignment)((FloatingLabelAlignment)__other))._x));
    }

    internal static string _stringify(double x)
    {
        return (x switch { -1.0 => "FloatingLabelAlignment.start", 0.0 => "FloatingLabelAlignment.center", _ => $"FloatingLabelAlignment(x: {x.toStringAsFixed(1L)})" });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => FloatingLabelAlignment._stringify(this._x);
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((((((((((((__other is _Decoration__input_decorator) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).contentPadding, this.contentPadding))) && (((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).isCollapsed == this.isCollapsed)) && (((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).floatingLabelHeight == this.floatingLabelHeight)) && (((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).floatingLabelProgress == this.floatingLabelProgress)) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).floatingLabelAlignment, this.floatingLabelAlignment))) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).border, this.border))) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).borderGap, this.borderGap))) && (((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).alignLabelWithHint == this.alignLabelWithHint)) && (((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).isDense == this.isDense)) && (((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).isEmpty == this.isEmpty)) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).visualDensity, this.visualDensity))) && (((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).inputGap == this.inputGap)) && (((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).maintainHintSize == this.maintainHintSize)) && (((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).maintainLabelSize == this.maintainLabelSize)) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).icon, this.icon))) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).input, this.input))) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).label, this.label))) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).hint, this.hint))) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).prefix, this.prefix))) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).suffix, this.suffix))) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).prefixIcon, this.prefixIcon))) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).suffixIcon, this.suffixIcon))) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).helperError, this.helperError))) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).counter, this.counter))) && (object.Equals(((_Decoration__input_decorator)((_Decoration__input_decorator)__other)).container, this.container)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.contentPadding, this.floatingLabelHeight, this.floatingLabelProgress, this.floatingLabelAlignment, this.border, this.borderGap, this.alignLabelWithHint, this.isDense, this.isEmpty, this.visualDensity, this.inputGap, this.maintainHintSize, this.maintainLabelSize, this.icon, this.input, this.label, this.hint, this.prefix, this.suffix, FoundationRuntimePorts.ObjectHash(this.prefixIcon, this.suffixIcon, this.helperError, this.counter, this.container)));
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
    internal virtual bool _material3 { get; set; } = false;
    internal virtual Matrix4? _labelTransform { get; set; } = default;
    public virtual DartMap<_DecorationSlot__input_decorator, global::Doroti.Framework.Rendering.RenderBox> _slotToChild { get; set; } = new DartMap<_DecorationSlot__input_decorator, global::Doroti.Framework.Rendering.RenderBox>();

    internal _RenderDecoration__input_decorator(_Decoration__input_decorator decoration, TextDirection textDirection, TextBaseline textBaseline, bool isFocused, bool expands, bool material3, global::Doroti.Framework.Painting.TextAlignVertical? textAlignVertical = null)
    {
        this._decoration = decoration;
        this._textDirection = textDirection;
        this._textBaseline = textBaseline;
        this._textAlignVertical = textAlignVertical;
        this._isFocused = isFocused;
        this._expands = expands;
        this._material3 = material3;
    }

    public virtual double subtextGap => (this.material3 ? 4.0 : 8.0);
    public virtual double prefixToInputGap => (this.material3 ? 4.0 : 0.0);
    public virtual double inputToSuffixGap => (this.material3 ? 4.0 : 0.0);
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
            global::Doroti.Framework.Rendering.RenderBox? helperError__23189 = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)childForSlot(_DecorationSlot__input_decorator.helperError));
            return ((IEnumerable<global::Doroti.Framework.Rendering.RenderBox>)(object?)((Func<List<global::Doroti.Framework.Rendering.RenderBox>>)(() => { var __collection23257 = new List<global::Doroti.Framework.Rendering.RenderBox>(); var __collectionElement23276 = this.icon; if (__collectionElement23276 is { } __nonNullCollectionElement23276) { __collection23257.Add(__nonNullCollectionElement23276); } var __collectionElement23289 = this.input; if (__collectionElement23289 is { } __nonNullCollectionElement23289) { __collection23257.Add(__nonNullCollectionElement23289); } var __collectionElement23303 = this.prefixIcon; if (__collectionElement23303 is { } __nonNullCollectionElement23303) { __collection23257.Add(__nonNullCollectionElement23303); } var __collectionElement23322 = this.suffixIcon; if (__collectionElement23322 is { } __nonNullCollectionElement23322) { __collection23257.Add(__nonNullCollectionElement23322); } var __collectionElement23341 = this.prefix; if (__collectionElement23341 is { } __nonNullCollectionElement23341) { __collection23257.Add(__nonNullCollectionElement23341); } var __collectionElement23356 = this.suffix; if (__collectionElement23356 is { } __nonNullCollectionElement23356) { __collection23257.Add(__nonNullCollectionElement23356); } var __collectionElement23371 = this.label; if (__collectionElement23371 is { } __nonNullCollectionElement23371) { __collection23257.Add(__nonNullCollectionElement23371); } var __collectionElement23385 = this.hint; if (__collectionElement23385 is { } __nonNullCollectionElement23385) { __collection23257.Add(__nonNullCollectionElement23385); } var __collectionElement23398 = helperError__23189; if (__collectionElement23398 is { } __nonNullCollectionElement23398) { __collection23257.Add(__nonNullCollectionElement23398); } var __collectionElement23418 = this.counter; if (__collectionElement23418 is { } __nonNullCollectionElement23418) { __collection23257.Add(__nonNullCollectionElement23418); } var __collectionElement23434 = this.container; if (__collectionElement23434 is { } __nonNullCollectionElement23434) { __collection23257.Add(__nonNullCollectionElement23434); } return __collection23257; }))());
            return default!;
        }
    }
    public virtual _Decoration__input_decorator decoration
    {
        get => this._decoration;
        set
        {
            var __value = value;
            if ((object.Equals(this._decoration, __value)))
            {
                return;
            }
            _decoration = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextBaseline textBaseline
    {
        get => this._textBaseline;
        set
        {
            var __value = value;
            if ((object.Equals(this._textBaseline, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _textBaseline = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    internal virtual global::Doroti.Framework.Painting.TextAlignVertical _defaultTextAlignVertical => (this._isOutlineAligned ? global::Doroti.Framework.Painting.TextAlignVertical.center : global::Doroti.Framework.Painting.TextAlignVertical.top);
    public virtual global::Doroti.Framework.Painting.TextAlignVertical textAlignVertical
    {
        get => ((this._textAlignVertical ?? (global::Doroti.Framework.Painting.TextAlignVertical)this._defaultTextAlignVertical));
        set
        {
            global::Doroti.Framework.Painting.TextAlignVertical? __value = value;
            if ((object.Equals(this._textAlignVertical, __value)))
            {
                return;
            }
            if ((((global::Doroti.Framework.Painting.TextAlignVertical)this.textAlignVertical).y == ((__value?.y ?? ((global::Doroti.Framework.Painting.TextAlignVertical)this._defaultTextAlignVertical).y))))
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
        get => this._isFocused;
        set
        {
            var __value = value;
            if ((this._isFocused == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _isFocused = DartRuntimePrimitives.RequireValue(__value);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool expands
    {
        get => this._expands;
        set
        {
            var __value = value;
            if ((this._expands == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _expands = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual bool material3
    {
        get => this._material3;
        set
        {
            var __value = value;
            if ((this._material3 == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _material3 = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    internal virtual bool _isOutlineAligned
    {
        get
        {
            return (!((_Decoration__input_decorator)this.decoration).isCollapsed && ((_Decoration__input_decorator)this.decoration).border.isOutline);
            return default!;
        }
    }
    internal virtual global::Doroti.Ui.Offset _densityOffset => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(((_Decoration__input_decorator)this.decoration).visualDensity.baseSizeAdjustment);
    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        if ((this.icon is not null))
        {
            visitor(this.icon!);
        }
        if ((this.prefix is not null))
        {
            visitor(this.prefix!);
        }
        if ((this.prefixIcon is not null))
        {
            visitor(this.prefixIcon!);
        }
        if ((this.label is not null))
        {
            visitor(this.label!);
        }
        if ((this.hint is not null))
        {
            if (this.isFocused)
            {
                visitor(this.hint!);
            }
            else
            {
                if ((this.label is null))
                {
                    visitor(this.hint!);
                }
            }
        }
        if ((this.input is not null))
        {
            visitor(this.input!);
        }
        if ((this.suffixIcon is not null))
        {
            visitor(this.suffixIcon!);
        }
        if ((this.suffix is not null))
        {
            visitor(this.suffix!);
        }
        if ((this.container is not null))
        {
            visitor(this.container!);
        }
        visitor(this.helperError);
        if ((this.counter is not null))
        {
            visitor(this.counter!);
        }
    }

    internal static double _minWidth(global::Doroti.Framework.Rendering.RenderBox? box, double height) => DartRuntimePrimitives.ConvertValue<double>((box?.getMinIntrinsicWidth(height) ?? 0.0));
    internal static double _maxWidth(global::Doroti.Framework.Rendering.RenderBox? box, double height) => DartRuntimePrimitives.ConvertValue<double>((box?.getMaxIntrinsicWidth(height) ?? 0.0));
    internal static double _minHeight(global::Doroti.Framework.Rendering.RenderBox? box, double width) => DartRuntimePrimitives.ConvertValue<double>((box?.getMinIntrinsicHeight(width) ?? 0.0));
    internal static global::Doroti.Ui.Size _boxSize(global::Doroti.Framework.Rendering.RenderBox? box) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Size>((box?.size ?? Size.zero));
    internal static double _getBaseline(global::Doroti.Framework.Rendering.RenderBox box, global::Doroti.Framework.Rendering.BoxConstraints boxConstraints)
    {
        return (ChildLayoutHelper.getBaseline(box, boxConstraints, TextBaseline.alphabetic) ?? ((global::Doroti.Framework.Rendering.RenderBox)box).size.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _getDryBaseline(global::Doroti.Framework.Rendering.RenderBox box, global::Doroti.Framework.Rendering.BoxConstraints boxConstraints)
    {
        return (ChildLayoutHelper.getDryBaseline(box, boxConstraints, TextBaseline.alphabetic) ?? ChildLayoutHelper.dryLayoutChild(box, boxConstraints).height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Rendering.BoxParentData _boxParentData(global::Doroti.Framework.Rendering.RenderBox box) => ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)box.parentData!)!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsDirectional contentPadding => ((_Decoration__input_decorator)this.decoration).contentPadding;
    internal virtual (double ascent, double bottomHeight, double subtextHeight)? _computeSubtextSizes(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, double> getBaseline)
    {
        var (counterSize__27587, counterAscent__27607) = (this.counter switch { global::Doroti.Framework.Rendering.RenderBox box__27665 => (((Size, double))((layoutChild(box__27665, constraints), getBaseline(box__27665, constraints)))), null => (((Size, double))((Size.zero, 0.0))) });
        double counterPadding__27862 = ((this.counter is not null) ? Input_decoratorLibrary._kSubtextCounterPadding : 0.0);
        global::Doroti.Framework.Rendering.BoxConstraints helperErrorConstraints__27953 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.deflate(global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: (counterSize__27587.width + counterPadding__27862))));
        double helperErrorHeight__28088 = layoutChild(this.helperError, helperErrorConstraints__27953).height;
        if (((helperErrorHeight__28088 == 0.0) && (counterSize__27587.height == 0.0)))
        {
            return null;
        }
        double ascent__28513 = (Math.Max(counterAscent__27607, getBaseline(this.helperError, helperErrorConstraints__27953)) + this.subtextGap);
        double bottomHeight__28635 = (Math.Max(counterAscent__27607, helperErrorHeight__28088) + this.subtextGap);
        double subtextHeight__28724 = (Math.Max(counterSize__27587.height, helperErrorHeight__28088) + this.subtextGap);
        return (ascent: ascent__28513, bottomHeight: bottomHeight__28635, subtextHeight: subtextHeight__28724);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _RenderDecorationLayout__input_decorator _layout(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, double> getBaseline)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth < double.PositiveInfinity), () => (object?)"An InputDecorator, which is typically created by a TextField, cannot " + "have an unbounded width.\n" + "This happens when the parent widget does not provide a finite width " + "constraint. For example, if the InputDecorator is contained by a Row, " + "then its width must be constrained. An Expanded widget or a SizedBox " + "can be used to constrain the width of the InputDecorator or the " + "TextField that contains it.");
        global::Doroti.Framework.Rendering.BoxConstraints boxConstraints__29828 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.loosen());
        global::Doroti.Framework.Rendering.RenderBox? icon__29942 = this.icon;
        double iconWidth__29977 = ((icon__29942 is null) ? 0.0 : layoutChild(icon__29942, boxConstraints__29828).width);
        global::Doroti.Framework.Rendering.BoxConstraints containerConstraints__30076 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)boxConstraints__29828.deflate(global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: iconWidth__29977)));
        global::Doroti.Framework.Rendering.BoxConstraints contentConstraints__30195 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)containerConstraints__30076.deflate(global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: (((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).start + ((_Decoration__input_decorator)this.decoration).inputGap), end: (((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).end + ((_Decoration__input_decorator)this.decoration).inputGap))));
        (double ascent, double bottomHeight, double subtextHeight)? subtextSize__30549 = _computeSubtextSizes(constraints: contentConstraints__30195, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)layoutChild, getBaseline: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, double>)getBaseline);
        global::Doroti.Framework.Rendering.RenderBox? prefixIcon__30717 = this.prefixIcon;
        global::Doroti.Framework.Rendering.RenderBox? suffixIcon__30768 = this.suffixIcon;
        global::Doroti.Ui.Size prefixIconSize__30813 = ((global::Doroti.Ui.Size)(object?)((prefixIcon__30717 is null) ? Size.zero : layoutChild(prefixIcon__30717, containerConstraints__30076)));
        global::Doroti.Ui.Size suffixIconSize__30941 = ((global::Doroti.Ui.Size)(object?)((suffixIcon__30768 is null) ? Size.zero : layoutChild(suffixIcon__30768, containerConstraints__30076)));
        global::Doroti.Framework.Rendering.RenderBox? prefix__31075 = this.prefix;
        global::Doroti.Framework.Rendering.RenderBox? suffix__31118 = this.suffix;
        global::Doroti.Ui.Size prefixSize__31155 = ((global::Doroti.Ui.Size)(object?)((prefix__31075 is null) ? Size.zero : layoutChild(prefix__31075, contentConstraints__30195)));
        global::Doroti.Ui.Size suffixSize__31253 = ((global::Doroti.Ui.Size)(object?)((suffix__31118 is null) ? Size.zero : layoutChild(suffix__31118, contentConstraints__30195)));
        var accessoryHorizontalInsets__31347 = global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((iconWidth__29977 + prefixSize__31155.width) + (((prefixIcon__30717 is null) ? (((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).start + ((_Decoration__input_decorator)this.decoration).inputGap) : (prefixIconSize__30813.width + this.prefixToInputGap)))), end: (suffixSize__31253.width + (((suffixIcon__30768 is null) ? (((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).end + ((_Decoration__input_decorator)this.decoration).inputGap) : (suffixIconSize__30941.width + this.inputToSuffixGap)))));
        double inputWidth__31824 = Math.Max(0.0, (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth - accessoryHorizontalInsets__31347.horizontal));
        global::Doroti.Framework.Rendering.RenderBox? label__31953 = this.label;
        double topHeight__31990 = default!;
        if ((label__31953 is not null))
        {
            double suffixIconSpace__32045 = (((_Decoration__input_decorator)this.decoration).border.isOutline ? DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(suffixIconSize__30941.width, ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).end, ((_Decoration__input_decorator)this.decoration).floatingLabelProgress)) : suffixIconSize__30941.width);
            double labelWidth__32244 = Math.Max(0.0, (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth - (((((((_Decoration__input_decorator)this.decoration).inputGap * 2L) + iconWidth__29977) + (((prefixIcon__30717 is null) ? ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).start : prefixIconSize__30813.width))) + (((suffixIcon__30768 is null) ? ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).end : suffixIconSpace__32045))))));
            double invertedLabelScale__32646 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(1.0, (1L / Input_decoratorLibrary._kFinalLabelScale), ((_Decoration__input_decorator)this.decoration).floatingLabelProgress));
            global::Doroti.Framework.Rendering.BoxConstraints labelConstraints__32803 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)boxConstraints__29828.copyWith(maxWidth: (labelWidth__32244 * invertedLabelScale__32646)));
            layoutChild(label__31953, labelConstraints__32803);
            double labelHeight__32971 = ((_Decoration__input_decorator)this.decoration).floatingLabelHeight;
            topHeight__31990 = (((_Decoration__input_decorator)this.decoration).border.isOutline ? Math.Max((labelHeight__32971 - getBaseline(label__31953, labelConstraints__32803)), 0.0) : labelHeight__32971);
        }
        else
        {
            topHeight__31990 = 0.0;
        }
        double bottomHeight__33349 = (subtextSize__30549?.bottomHeight ?? 0.0);
        global::Doroti.Framework.Rendering.BoxConstraints inputConstraints__33423 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)boxConstraints__29828.deflate(global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: (((this.contentPadding.vertical + topHeight__31990) + bottomHeight__33349) + this._densityOffset.dy))).tighten(width: inputWidth__31824));
        global::Doroti.Framework.Rendering.RenderBox? input__33673 = this.input;
        global::Doroti.Framework.Rendering.RenderBox? hint__33714 = this.hint;
        global::Doroti.Ui.Size inputSize__33747 = ((global::Doroti.Ui.Size)(object?)((input__33673 is null) ? Size.zero : layoutChild(input__33673, inputConstraints__33423)));
        global::Doroti.Ui.Size hintSize__33840 = ((global::Doroti.Ui.Size)(object?)((hint__33714 is null) ? Size.zero : layoutChild(hint__33714, boxConstraints__29828.tighten(width: inputWidth__31824))));
        double inputBaseline__33973 = ((input__33673 is null) ? 0.0 : getBaseline(input__33673, inputConstraints__33423));
        double hintBaseline__34066 = ((hint__33714 is null) ? 0.0 : getBaseline(hint__33714, boxConstraints__29828.tighten(width: inputWidth__31824)));
        double inputHeight__34265 = Math.Max(((((_Decoration__input_decorator)this.decoration).isEmpty || ((_Decoration__input_decorator)this.decoration).maintainHintSize) ? hintSize__33840.height : 0.0), inputSize__33747.height);
        double inputInternalBaseline__34418 = Math.Max(inputBaseline__33973, hintBaseline__34066);
        double prefixBaseline__34499 = ((prefix__31075 is null) ? 0.0 : getBaseline(prefix__31075, contentConstraints__30195));
        double suffixBaseline__34597 = ((suffix__31118 is null) ? 0.0 : getBaseline(suffix__31118, contentConstraints__30195));
        double fixHeight__34792 = Math.Max(prefixBaseline__34499, suffixBaseline__34597);
        double fixAboveInput__34863 = Math.Max(0, (fixHeight__34792 - inputInternalBaseline__34418));
        double fixBelowBaseline__34944 = Math.Max((prefixSize__31155.height - prefixBaseline__34499), (suffixSize__31253.height - suffixBaseline__34597));
        double fixBelowInput__35250 = Math.Max(0, (fixBelowBaseline__34944 - ((inputHeight__34265 - inputInternalBaseline__34418))));
        double fixIconHeight__35431 = Math.Max(prefixIconSize__30813.height, suffixIconSize__30941.height);
        double contentHeight__35520 = Math.Max(fixIconHeight__35431, ((((((topHeight__31990 + ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).top) + fixAboveInput__34863) + inputHeight__34265) + fixBelowInput__35250) + ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).bottom) + this._densityOffset.dy));
        double minContainerHeight__35779 = (((DartRuntimePrimitives.RequireValue(((_Decoration__input_decorator)this.decoration).isDense) || ((_Decoration__input_decorator)this.decoration).isCollapsed) || this.expands) ? inputHeight__34265 : ConstantsLibrary.kMinInteractiveDimension);
        double maxContainerHeight__35932 = Math.Max(0.0, (((global::Doroti.Framework.Rendering.BoxConstraints)boxConstraints__29828).maxHeight - bottomHeight__33349));
        double containerHeight__36026 = (this.expands ? maxContainerHeight__35932 : Math.Min(Math.Max(contentHeight__35520, minContainerHeight__35779), maxContainerHeight__35932));
        double interactiveAdjustment__36306 = ((minContainerHeight__35779 > contentHeight__35520) ? (((minContainerHeight__35779 - contentHeight__35520)) / 2.0) : 0.0);
        double overflow__36684 = Math.Max(0, (contentHeight__35520 - maxContainerHeight__35932));
        double textAlignVerticalFactor__36895 = (((((global::Doroti.Framework.Painting.TextAlignVertical)this.textAlignVertical).y + 1.0)) / 2.0);
        double baselineAdjustment__37176 = (fixAboveInput__34863 - (overflow__36684 * ((1L - textAlignVerticalFactor__36895))));
        double topInputBaseline__37351 = (((((((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).top + topHeight__31990) + inputInternalBaseline__34418) + baselineAdjustment__37176) + interactiveAdjustment__36306) + (this._densityOffset.dy / 2.0));
        double maxContentHeight__37562 = (((containerHeight__36026 - this.contentPadding.vertical) - topHeight__31990) - this._densityOffset.dy);
        double alignableHeight__37681 = ((fixAboveInput__34863 + inputHeight__34265) + fixBelowInput__35250);
        double maxVerticalOffset__37761 = (maxContentHeight__37562 - alignableHeight__37681);
        double baseline__37835 = default!;
        if (this._isOutlineAligned)
        {
            double outlineCenterBaseline__38475 = ((inputInternalBaseline__34418 + (baselineAdjustment__37176 / 2.0)) + (((containerHeight__36026 - inputHeight__34265)) / 2.0));
            var outlineTopBaseline__38611 = topInputBaseline__37351;
            double outlineBottomBaseline__38669 = (topInputBaseline__37351 + maxVerticalOffset__37761);
            baseline__37835 = _RenderDecoration__input_decorator._interpolateThree(outlineTopBaseline__38611, outlineCenterBaseline__38475, outlineBottomBaseline__38669, this.textAlignVertical);
        }
        else
        {
            double textAlignVerticalOffset__38925 = (maxVerticalOffset__37761 * textAlignVerticalFactor__36895);
            baseline__37835 = (topInputBaseline__37351 + textAlignVerticalOffset__38925);
        }
        return new _RenderDecorationLayout__input_decorator(inputConstraints: inputConstraints__33423, containerHeight: containerHeight__36026, baseline: baseline__37835, subtextSize: subtextSize__30549, size: new global::Doroti.Ui.Size(((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth, (containerHeight__36026 + ((subtextSize__30549?.subtextHeight ?? 0.0)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _interpolateThree(double begin, double middle, double end, global::Doroti.Framework.Painting.TextAlignVertical textAlignVertical)
    {
        double basis__40131 = ((((global::Doroti.Framework.Painting.TextAlignVertical)textAlignVertical).y <= 0L) ? Math.Max((middle - begin), 0) : Math.Max((end - middle), 0));
        return (middle + (basis__40131 * ((global::Doroti.Framework.Painting.TextAlignVertical)textAlignVertical).y));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        double inputWidth__40373 = ((((_Decoration__input_decorator)this.decoration).isEmpty || ((_Decoration__input_decorator)this.decoration).maintainHintSize) ? Math.Max(_RenderDecoration__input_decorator._minWidth(this.input, height), _RenderDecoration__input_decorator._minWidth(this.hint, height)) : _RenderDecoration__input_decorator._minWidth(this.input, height));
        double contentWidth__40559 = (((_Decoration__input_decorator)this.decoration).maintainLabelSize ? Math.Max(inputWidth__40373, _RenderDecoration__input_decorator._minWidth(this.label, height)) : inputWidth__40373);
        return (((((((_RenderDecoration__input_decorator._minWidth(this.icon, height) + (((this.prefixIcon is not null) ? this.prefixToInputGap : (((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).start + ((_Decoration__input_decorator)this.decoration).inputGap)))) + _RenderDecoration__input_decorator._minWidth(this.prefixIcon, height)) + _RenderDecoration__input_decorator._minWidth(this.prefix, height)) + contentWidth__40559) + _RenderDecoration__input_decorator._minWidth(this.suffix, height)) + _RenderDecoration__input_decorator._minWidth(this.suffixIcon, height)) + (((this.suffixIcon is not null) ? this.inputToSuffixGap : (((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).end + ((_Decoration__input_decorator)this.decoration).inputGap))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double inputWidth__41166 = ((((_Decoration__input_decorator)this.decoration).isEmpty || ((_Decoration__input_decorator)this.decoration).maintainHintSize) ? Math.Max(_RenderDecoration__input_decorator._maxWidth(this.input, height), _RenderDecoration__input_decorator._maxWidth(this.hint, height)) : _RenderDecoration__input_decorator._maxWidth(this.input, height));
        double contentWidth__41352 = (((_Decoration__input_decorator)this.decoration).maintainLabelSize ? Math.Max(inputWidth__41166, _RenderDecoration__input_decorator._maxWidth(this.label, height)) : inputWidth__41166);
        return (((((((_RenderDecoration__input_decorator._maxWidth(this.icon, height) + (((this.prefixIcon is not null) ? this.prefixToInputGap : (((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).start + ((_Decoration__input_decorator)this.decoration).inputGap)))) + _RenderDecoration__input_decorator._maxWidth(this.prefixIcon, height)) + _RenderDecoration__input_decorator._maxWidth(this.prefix, height)) + contentWidth__41352) + _RenderDecoration__input_decorator._maxWidth(this.suffix, height)) + _RenderDecoration__input_decorator._maxWidth(this.suffixIcon, height)) + (((this.suffixIcon is not null) ? this.inputToSuffixGap : (((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).end + ((_Decoration__input_decorator)this.decoration).inputGap))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _lineHeight(double width, List<global::Doroti.Framework.Rendering.RenderBox?> boxes)
    {
        var height__41948 = 0.0;
        foreach (var box__41977 in boxes)
        {
            if ((box__41977 is null))
            {
                continue;
            }
            height__41948 = Math.Max(_RenderDecoration__input_decorator._minHeight(box__41977, width), height__41948);
        }
        return height__41948;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double iconHeight__42396 = _RenderDecoration__input_decorator._minHeight(this.icon, width);
        double iconWidth__42451 = _RenderDecoration__input_decorator._minWidth(this.icon, iconHeight__42396);
        width = Math.Max((width - iconWidth__42451), 0.0);
        double prefixIconHeight__42557 = _RenderDecoration__input_decorator._minHeight(this.prefixIcon, width);
        double prefixIconWidth__42624 = _RenderDecoration__input_decorator._minWidth(this.prefixIcon, prefixIconHeight__42557);
        double suffixIconHeight__42701 = _RenderDecoration__input_decorator._minHeight(this.suffixIcon, width);
        double suffixIconWidth__42768 = _RenderDecoration__input_decorator._minWidth(this.suffixIcon, suffixIconHeight__42701);
        width = Math.Max(((width - this.contentPadding.horizontal) - (((_Decoration__input_decorator)this.decoration).inputGap * 2L)), 0.0);
        double counterHeight__43078 = _RenderDecoration__input_decorator._minHeight(this.counter, width);
        double counterWidth__43139 = _RenderDecoration__input_decorator._minWidth(this.counter, counterHeight__43078);
        double counterPadding__43276 = ((this.counter is not null) ? Input_decoratorLibrary._kSubtextCounterPadding : 0.0);
        double helperErrorAvailableWidth__43359 = Math.Max(((width - counterWidth__43139) - counterPadding__43276), 0.0);
        double helperErrorHeight__43458 = _RenderDecoration__input_decorator._minHeight(this.helperError, helperErrorAvailableWidth__43359);
        double subtextHeight__43541 = Math.Max(counterHeight__43078, helperErrorHeight__43458);
        if ((subtextHeight__43541 > 0.0))
        {
            subtextHeight__43541 += this.subtextGap;
        }
        double prefixHeight__43691 = _RenderDecoration__input_decorator._minHeight(this.prefix, width);
        double prefixWidth__43750 = _RenderDecoration__input_decorator._minWidth(this.prefix, prefixHeight__43691);
        double suffixHeight__43815 = _RenderDecoration__input_decorator._minHeight(this.suffix, width);
        double suffixWidth__43874 = _RenderDecoration__input_decorator._minWidth(this.suffix, suffixHeight__43815);
        double availableInputWidth__43939 = Math.Max(((((width - prefixWidth__43750) - suffixWidth__43874) - prefixIconWidth__42624) - suffixIconWidth__42768), 0.0);
        double inputHeight__44083 = _lineHeight(availableInputWidth__43939, ((Func<List<global::Doroti.Framework.Rendering.RenderBox?>>)(() => { var __collection44130 = new List<global::Doroti.Framework.Rendering.RenderBox?>(); __collection44130.Add(this.input); if (((_Decoration__input_decorator)this.decoration).isEmpty) { __collection44130.Add(this.hint); } return __collection44130; }))());
        double inputMaxHeight__44218 = new List<double> { inputHeight__44083, prefixHeight__43691, suffixHeight__43815 }.reduce(global::Doroti.Runtime.Dart_mathLibrary.max);
        double contentHeight__44346 = ((((((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).top + (((this.label is null) ? 0.0 : ((_Decoration__input_decorator)this.decoration).floatingLabelHeight))) + inputMaxHeight__44218) + ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).bottom) + this._densityOffset.dy);
        double containerHeight__44557 = new List<double> { iconHeight__42396, contentHeight__44346, prefixIconHeight__42557, suffixIconHeight__42701 }.reduce(global::Doroti.Runtime.Dart_mathLibrary.max);
        double minContainerHeight__44713 = ((DartRuntimePrimitives.RequireValue(((_Decoration__input_decorator)this.decoration).isDense) || this.expands) ? 0.0 : ConstantsLibrary.kMinInteractiveDimension);
        return (Math.Max(containerHeight__44557, minContainerHeight__44713) + subtextHeight__43541);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return getMinIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? input__45103 = this.input;
        if ((input__45103 is null))
        {
            return 0.0;
        }
        return (_RenderDecoration__input_decorator._boxParentData(input__45103).offset.dy + ((input__45103.getDistanceToActualBaseline(baseline) ?? ((global::Doroti.Framework.Rendering.RenderBox)input__45103).size.height)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? input__45494 = this.input;
        if ((input__45494 is null))
        {
            return 0.0;
        }
        _RenderDecorationLayout__input_decorator layout__45597 = ((_RenderDecorationLayout__input_decorator)(object?)_layout(constraints, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.dryLayoutChild, getBaseline: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, double>)_getDryBaseline));
        return ((baseline switch { TextBaseline.alphabetic => 0.0, TextBaseline.ideographic => (((input__45494.getDryBaseline(((_RenderDecorationLayout__input_decorator)layout__45597).inputConstraints, TextBaseline.ideographic) ?? input__45494.getDryLayout(((_RenderDecorationLayout__input_decorator)layout__45597).inputConstraints).height)) - ((input__45494.getDryBaseline(((_RenderDecorationLayout__input_decorator)layout__45597).inputConstraints, TextBaseline.alphabetic) ?? input__45494.getDryLayout(((_RenderDecorationLayout__input_decorator)layout__45597).inputConstraints).height))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }) + ((_RenderDecorationLayout__input_decorator)layout__45597).baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        _RenderDecorationLayout__input_decorator layout__46309 = ((_RenderDecorationLayout__input_decorator)(object?)_layout(constraints, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.dryLayoutChild, getBaseline: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, double>)_getDryBaseline));
        return constraints.constrain(((_RenderDecorationLayout__input_decorator)layout__46309).size);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraints__46556 = this.constraints;
        _labelTransform = null;
        _RenderDecorationLayout__input_decorator layout__46650 = ((_RenderDecorationLayout__input_decorator)(object?)_layout(constraints__46556, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.layoutChild, getBaseline: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, double>)_getBaseline));
        size = constraints__46556.constrain(((_RenderDecorationLayout__input_decorator)layout__46650).size);
        DartRuntimePrimitives.Assert(() => (this.size.width == constraints__46556.constrainWidth(((_RenderDecorationLayout__input_decorator)layout__46650).size.width)));
        DartRuntimePrimitives.Assert(() => (this.size.height == constraints__46556.constrainHeight(((_RenderDecorationLayout__input_decorator)layout__46650).size.height)));
        double overallWidth__46991 = ((_RenderDecorationLayout__input_decorator)layout__46650).size.width;
        global::Doroti.Framework.Rendering.RenderBox? container__47047 = this.container;
        if ((container__47047 is not null))
        {
            var containerConstraints__47116 = global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(height: ((_RenderDecorationLayout__input_decorator)layout__46650).containerHeight, width: (overallWidth__46991 - _RenderDecoration__input_decorator._boxSize(this.icon).width));
            container__47047.layout(containerConstraints__47116, parentUsesSize: true);
            double x__47352 = (this.textDirection switch { TextDirection.rtl => 0.0, TextDirection.ltr => _RenderDecoration__input_decorator._boxSize(this.icon).width, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            _RenderDecoration__input_decorator._boxParentData(container__47047).offset = new global::Doroti.Ui.Offset(x__47352, 0.0);
        }
        double height__47556 = ((_RenderDecorationLayout__input_decorator)layout__46650).containerHeight;
        double centerLayout(global::Doroti.Framework.Rendering.RenderBox box, double x)
        {
            _RenderDecoration__input_decorator._boxParentData(box).offset = new global::Doroti.Ui.Offset(x, (((height__47556 - ((global::Doroti.Framework.Rendering.RenderBox)box).size.height)) / 2.0));
            return ((global::Doroti.Framework.Rendering.RenderBox)box).size.width;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if ((this.icon is not null))
        {
            double x__47799 = (this.textDirection switch { TextDirection.rtl => (overallWidth__46991 - this.icon!.size.width), TextDirection.ltr => 0.0, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            centerLayout(this.icon!, x__47799);
        }
        double subtextBaseline__47987 = (((((_RenderDecorationLayout__input_decorator)layout__46650).subtextSize?.ascent ?? 0.0)) + ((_RenderDecorationLayout__input_decorator)layout__46650).containerHeight);
        global::Doroti.Framework.Rendering.RenderBox? counter__48088 = this.counter;
        double helperErrorBaseline__48129 = DartRuntimePrimitives.RequireValue(this.helperError.getDistanceToBaseline(TextBaseline.alphabetic));
        double counterBaseline__48229 = (counter__48088?.getDistanceToBaseline(TextBaseline.alphabetic) ?? 0.0);
        double start__48324 = default!;
        double end__48331 = default!;
        switch (this.textDirection)
        {
            case TextDirection.ltr:
                {
                    start__48324 = (((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).start + _RenderDecoration__input_decorator._boxSize(this.icon).width);
                    end__48331 = (overallWidth__46991 - ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).end);
                    _RenderDecoration__input_decorator._boxParentData(this.helperError).offset = new global::Doroti.Ui.Offset((start__48324 + ((_Decoration__input_decorator)this.decoration).inputGap), (subtextBaseline__47987 - helperErrorBaseline__48129));
                    if ((counter__48088 is not null))
                    {
                        _RenderDecoration__input_decorator._boxParentData(counter__48088).offset = new global::Doroti.Ui.Offset(((end__48331 - ((global::Doroti.Framework.Rendering.RenderBox)counter__48088).size.width) - ((_Decoration__input_decorator)this.decoration).inputGap), (subtextBaseline__47987 - counterBaseline__48229));
                    }
                    break;
                }
            case TextDirection.rtl:
                {
                    start__48324 = ((overallWidth__46991 - ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).start) - _RenderDecoration__input_decorator._boxSize(this.icon).width);
                    end__48331 = ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).end;
                    _RenderDecoration__input_decorator._boxParentData(this.helperError).offset = new global::Doroti.Ui.Offset(((start__48324 - ((global::Doroti.Framework.Rendering.RenderBox)this.helperError).size.width) - ((_Decoration__input_decorator)this.decoration).inputGap), (subtextBaseline__47987 - helperErrorBaseline__48129));
                    if ((counter__48088 is not null))
                    {
                        _RenderDecoration__input_decorator._boxParentData(counter__48088).offset = new global::Doroti.Ui.Offset((end__48331 + ((_Decoration__input_decorator)this.decoration).inputGap), (subtextBaseline__47987 - counterBaseline__48229));
                    }
                    break;
                }
        }
        double baseline__49401 = ((_RenderDecorationLayout__input_decorator)layout__46650).baseline;
        double baselineLayout(global::Doroti.Framework.Rendering.RenderBox box, double x)
        {
            _RenderDecoration__input_decorator._boxParentData(box).offset = new global::Doroti.Ui.Offset(x, (baseline__49401 - DartRuntimePrimitives.RequireValue(box.getDistanceToBaseline(TextBaseline.alphabetic))));
            return ((global::Doroti.Framework.Rendering.RenderBox)box).size.width;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        switch (this.textDirection)
        {
            case TextDirection.rtl:
                {
                    if ((this.prefixIcon is not null))
                    {
                        start__48324 += ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).start;
                        start__48324 -= centerLayout(this.prefixIcon!, (start__48324 - this.prefixIcon!.size.width));
                        start__48324 -= this.prefixToInputGap;
                    }
                    else
                    {
                        start__48324 -= ((_Decoration__input_decorator)this.decoration).inputGap;
                    }
                    if ((this.label is not null))
                    {
                        if (((_Decoration__input_decorator)this.decoration).alignLabelWithHint)
                        {
                            baselineLayout(this.label!, (start__48324 - this.label!.size.width));
                        }
                        else
                        {
                            centerLayout(this.label!, (start__48324 - this.label!.size.width));
                        }
                    }
                    if ((this.prefix is not null))
                    {
                        start__48324 -= baselineLayout(this.prefix!, (start__48324 - this.prefix!.size.width));
                    }
                    if ((this.input is not null))
                    {
                        baselineLayout(this.input!, (start__48324 - this.input!.size.width));
                    }
                    if ((this.hint is not null))
                    {
                        baselineLayout(this.hint!, (start__48324 - this.hint!.size.width));
                    }
                    if ((this.suffixIcon is not null))
                    {
                        end__48331 -= ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).end;
                        end__48331 += centerLayout(this.suffixIcon!, end__48331);
                        end__48331 += this.inputToSuffixGap;
                    }
                    else
                    {
                        end__48331 += ((_Decoration__input_decorator)this.decoration).inputGap;
                    }
                    if ((this.suffix is not null))
                    {
                        end__48331 += baselineLayout(this.suffix!, end__48331);
                    }
                    break;
                    break;
                }
            case TextDirection.ltr:
                {
                    if ((this.prefixIcon is not null))
                    {
                        start__48324 -= ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).start;
                        start__48324 += centerLayout(this.prefixIcon!, start__48324);
                        start__48324 += this.prefixToInputGap;
                    }
                    else
                    {
                        start__48324 += ((_Decoration__input_decorator)this.decoration).inputGap;
                    }
                    if ((this.label is not null))
                    {
                        if (((_Decoration__input_decorator)this.decoration).alignLabelWithHint)
                        {
                            baselineLayout(this.label!, start__48324);
                        }
                        else
                        {
                            centerLayout(this.label!, start__48324);
                        }
                    }
                    if ((this.prefix is not null))
                    {
                        start__48324 += baselineLayout(this.prefix!, start__48324);
                    }
                    if ((this.input is not null))
                    {
                        baselineLayout(this.input!, start__48324);
                    }
                    if ((this.hint is not null))
                    {
                        baselineLayout(this.hint!, start__48324);
                    }
                    if ((this.suffixIcon is not null))
                    {
                        end__48331 += ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).end;
                        end__48331 -= centerLayout(this.suffixIcon!, (end__48331 - this.suffixIcon!.size.width));
                        end__48331 -= this.inputToSuffixGap;
                    }
                    else
                    {
                        end__48331 -= ((_Decoration__input_decorator)this.decoration).inputGap;
                    }
                    if ((this.suffix is not null))
                    {
                        end__48331 -= baselineLayout(this.suffix!, (end__48331 - this.suffix!.size.width));
                    }
                    break;
                    break;
                }
        }
        if ((this.label is not null))
        {
            double labelX__52148 = _RenderDecoration__input_decorator._boxParentData(this.label!).offset.dx;
            double floatAlign__52276 = (((_Decoration__input_decorator)this.decoration).floatingLabelAlignment._x + 1L);
            double floatWidth__52350 = (_RenderDecoration__input_decorator._boxSize(this.label).width * Input_decoratorLibrary._kFinalLabelScale);
            switch (this.textDirection)
            {
                case TextDirection.rtl:
                    {
                        var offsetToPrefixIcon__52609 = 0.0;
                        if (((this.prefixIcon is not null) && !((_Decoration__input_decorator)this.decoration).alignLabelWithHint))
                        {
                            offsetToPrefixIcon__52609 = (this.material3 ? (_RenderDecoration__input_decorator._boxSize(this.prefixIcon).width - ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).end) : 0);
                        }
                        ((_Decoration__input_decorator)this.decoration).borderGap.start = Dart_uiLibrary.lerpDouble(((labelX__52148 + _RenderDecoration__input_decorator._boxSize(this.label).width) + offsetToPrefixIcon__52609), ((_RenderDecoration__input_decorator._boxSize(container__47047).width / 2.0) + (floatWidth__52350 / 2.0)), floatAlign__52276);
                        break;
                    }
                case TextDirection.ltr:
                    {
                        var offsetToPrefixIcon__53325 = 0.0;
                        if (((this.prefixIcon is not null) && !((_Decoration__input_decorator)this.decoration).alignLabelWithHint))
                        {
                            offsetToPrefixIcon__53325 = (this.material3 ? ((-_RenderDecoration__input_decorator._boxSize(this.prefixIcon).width + ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).start)) : 0);
                        }
                        ((_Decoration__input_decorator)this.decoration).borderGap.start = Dart_uiLibrary.lerpDouble(((labelX__52148 - _RenderDecoration__input_decorator._boxSize(this.icon).width) + offsetToPrefixIcon__53325), ((_RenderDecoration__input_decorator._boxSize(container__47047).width / 2.0) - (floatWidth__52350 / 2.0)), floatAlign__52276);
                        break;
                    }
            }
            ((_Decoration__input_decorator)this.decoration).borderGap.extent = (this.label!.size.width * Input_decoratorLibrary._kFinalLabelScale);
        }
        else
        {
            ((_Decoration__input_decorator)this.decoration).borderGap.start = null;
            ((_Decoration__input_decorator)this.decoration).borderGap.extent = 0.0;
        }
    }

    internal virtual void _paintLabel(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        context.paintChild(this.label!, offset);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        void doPaint(global::Doroti.Framework.Rendering.RenderBox? child)
        {
            if ((child is not null))
            {
                context.paintChild(child, (_RenderDecoration__input_decorator._boxParentData(child).offset + offset));
            }
        }
        doPaint(this.container);
        if ((this.label is not null))
        {
            global::Doroti.Ui.Offset labelOffset__54368 = ((global::Doroti.Ui.Offset)(object?)_RenderDecoration__input_decorator._boxParentData(this.label!).offset);
            double labelHeight__54432 = _RenderDecoration__input_decorator._boxSize(this.label).height;
            double labelWidth__54489 = _RenderDecoration__input_decorator._boxSize(this.label).width;
            double floatAlign__54610 = (((_Decoration__input_decorator)this.decoration).floatingLabelAlignment._x + 1L);
            double floatWidth__54684 = (labelWidth__54489 * Input_decoratorLibrary._kFinalLabelScale);
            global::Doroti.Framework.Painting.BorderSide borderSide__54752 = ((_Decoration__input_decorator)this.decoration).border.borderSide;
            double t__54814 = ((_Decoration__input_decorator)this.decoration).floatingLabelProgress;
            bool isOutlineBorder__54984 = ((_Decoration__input_decorator)this.decoration).border.isOutline;
            double outlinedFloatingY__55189 = ((((-labelHeight__54432 * Input_decoratorLibrary._kFinalLabelScale)) / 2.0) - (((global::Doroti.Framework.Painting.BorderSide)borderSide__54752).strokeOffset / 2.0));
            double floatingY__55312 = (isOutlineBorder__54984 ? outlinedFloatingY__55189 : (((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).top + (this._densityOffset.dy / 2L)));
            double scale__55445 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(1.0, Input_decoratorLibrary._kFinalLabelScale, t__54814));
            double centeredFloatX__55512 = ((_RenderDecoration__input_decorator._boxParentData(this.container!).offset.dx + (_RenderDecoration__input_decorator._boxSize(this.container).width / 2.0)) - (floatWidth__54684 / 2.0));
            double startX__55649 = default!;
            double floatStartX__55670 = default!;
            switch (this.textDirection)
            {
                case TextDirection.rtl:
                    {
                        startX__55649 = (labelOffset__54368.dx + (labelWidth__54489 * ((1.0 - scale__55445))));
                        floatStartX__55670 = startX__55649;
                        if ((((this.prefixIcon is not null) && !((_Decoration__input_decorator)this.decoration).alignLabelWithHint) && isOutlineBorder__54984))
                        {
                            floatStartX__55670 += (this.material3 ? (_RenderDecoration__input_decorator._boxSize(this.prefixIcon).width - ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).end) : 0.0);
                        }
                        break;
                    }
                case TextDirection.ltr:
                    {
                        startX__55649 = labelOffset__54368.dx;
                        floatStartX__55670 = startX__55649;
                        if ((((this.prefixIcon is not null) && !((_Decoration__input_decorator)this.decoration).alignLabelWithHint) && isOutlineBorder__54984))
                        {
                            floatStartX__55670 += (this.material3 ? (-_RenderDecoration__input_decorator._boxSize(this.prefixIcon).width + ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)this.contentPadding).start) : 0.0);
                        }
                        break;
                    }
            }
            double floatEndX__56409 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(floatStartX__55670, centeredFloatX__55512, floatAlign__54610));
            double dx__56494 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(startX__55649, floatEndX__56409, t__54814));
            double dy__56553 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, (floatingY__55312 - labelOffset__54368.dy), t__54814));
            _labelTransform = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.translateByDouble(dx__56494, (labelOffset__54368.dy + dy__56553), 0, 1);
    __cascade.scaleByDouble(scale__55445, scale__55445, scale__55445, 1);
    return __cascade;
}))();
            layer = context.pushTransform(this.needsCompositing, offset, this._labelTransform!, (global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)this._paintLabel, oldLayer: ((global::Doroti.Framework.Rendering.TransformLayer?)(object?)this.layer)!);
        }
        else
        {
            layer = null;
        }
        doPaint(this.icon);
        doPaint(this.prefix);
        doPaint(this.suffix);
        doPaint(this.prefixIcon);
        doPaint(this.suffixIcon);
        if (((_Decoration__input_decorator)this.decoration).isEmpty)
        {
            doPaint(this.hint);
        }
        doPaint(this.input);
        doPaint(this.helperError);
        doPaint(this.counter);
    }

    public override void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        if (((object.Equals(child, this.label)) && (this._labelTransform is not null)))
        {
            global::Doroti.Ui.Offset labelOffset__57370 = ((global::Doroti.Ui.Offset)(object?)_RenderDecoration__input_decorator._boxParentData(this.label!).offset);
            DartRuntimePrimitives.Ignore(((Func<Matrix4>)(() =>
{
    var __cascade = transform;
    __cascade.multiply(this._labelTransform!);
    __cascade.translateByDouble(-labelOffset__57370.dx, -labelOffset__57370.dy, 0, 1);
    return __cascade;
}))());
        }
        base.applyPaintTransform(child, transform);
    }

    public override bool hitTestSelf(Offset position) => true;
    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        foreach (global::Doroti.Framework.Rendering.RenderBox child__57770 in this.children)
        {
            global::Doroti.Ui.Offset offset__57883 = ((global::Doroti.Ui.Offset)(object?)_RenderDecoration__input_decorator._boxParentData(child__57770).offset);
            bool isHit__57939 = result.addWithPaintOffset(offset: offset__57883, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - offset__57883))));
                return child__57770.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            if (isHit__57939)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResult _childSemanticsConfigurationDelegate(List<global::Doroti.Framework.Semantics.SemanticsConfiguration> childConfigs)
    {
        var builder__58441 = new global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResultBuilder();
        var mergeGroups__58507 = new DartMap<global::Doroti.Framework.Semantics.SemanticsTag, List<global::Doroti.Framework.Semantics.SemanticsConfiguration>>();
        var tags__58579 = new HashSet<global::Doroti.Framework.Semantics.SemanticsTag> { _InputDecoratorState__input_decorator._kPrefixSemanticsTag, _InputDecoratorState__input_decorator._kPrefixIconSemanticsTag, _InputDecoratorState__input_decorator._kSuffixSemanticsTag, _InputDecoratorState__input_decorator._kSuffixIconSemanticsTag };
        foreach (var childConfig__58829 in childConfigs)
        {
            global::Doroti.Framework.Semantics.SemanticsTag? tag__58886 = tags__58579.firstWhereOrNull(((tag) => childConfig__58829.tagsChildrenWith(tag)));
            if ((tag__58886 is not null))
            {
                mergeGroups__58507.putIfAbsent(tag__58886, (() => new List<global::Doroti.Framework.Semantics.SemanticsConfiguration>())).Add(childConfig__58829);
            }
            else
            {
                builder__58441.markAsMergeUp(childConfig__58829);
            }
        }
        mergeGroups__58507.Values.forEach((__arg0) => ((global::System.Action<List<global::Doroti.Framework.Semantics.SemanticsConfiguration>>)((global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResultBuilder)builder__58441).markAsSiblingMergeGroup)(__arg0));
        return ((global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResult)(object?)builder__58441.build());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        config.childConfigurationsDelegate = (global::System.Func<List<global::Doroti.Framework.Semantics.SemanticsConfiguration>, global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResult>)this._childSemanticsConfigurationDelegate;
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox? childForSlot(_DecorationSlot__input_decorator slot) => this._slotToChild.GetValueOrDefault(slot);
    public virtual string debugNameForSlot(_DecorationSlot__input_decorator slot)
    {
        if (true)
        {
            return slot.ToString();
        }
        return slot.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        foreach (global::Doroti.Framework.Rendering.RenderBox child__6961 in this.children)
        {
            ((dynamic)child__6961).attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        foreach (global::Doroti.Framework.Rendering.RenderBox child__7095 in this.children)
        {
            ((dynamic)child__7095).detach();
        }
    }

    public override void redepthChildren()
    {
        this.children.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)this.redepthChild)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        this.children.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)visitor)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var value__7401 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        var childToSlot__7440 = new DartMap<global::Doroti.Framework.Rendering.RenderBox, _DecorationSlot__input_decorator>(this._slotToChild.Values, this._slotToChild.Keys);
        foreach (global::Doroti.Framework.Rendering.RenderBox child__7578 in this.children)
        {
            _addDiagnostics(child__7578, value__7401, debugNameForSlot(((_DecorationSlot__input_decorator)DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<_DecorationSlot__input_decorator>(childToSlot__7440, child__7578)))));
        }
        return value__7401;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _addDiagnostics(global::Doroti.Framework.Rendering.RenderBox child, List<global::Doroti.Framework.Foundation.DiagnosticsNode> value, string name)
    {
        value.Add(((Diagnosticable)child).toDiagnosticsNode(name: name));
    }

    public virtual void _setChild(global::Doroti.Framework.Rendering.RenderBox? child, _DecorationSlot__input_decorator slot)
    {
        global::Doroti.Framework.Rendering.RenderBox? oldChild__8003 = this._slotToChild.GetValueOrDefault(slot);
        if ((oldChild__8003 is not null))
        {
            dropChild(oldChild__8003);
            this._slotToChild.remove(slot);
        }
        if ((child is not null))
        {
            this._slotToChild[slot] = child;
            adoptChild(child);
        }
    }

    public virtual void _moveChild(global::Doroti.Framework.Rendering.RenderBox child, _DecorationSlot__input_decorator slot, _DecorationSlot__input_decorator oldSlot)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(slot, oldSlot)));
        global::Doroti.Framework.Rendering.RenderBox? oldChild__8343 = this._slotToChild.GetValueOrDefault(oldSlot);
        if ((object.Equals(oldChild__8343, child)))
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

    public override IEnumerable<_DecorationSlot__input_decorator> slots => DartRuntimePrimitives.ConvertValue<IEnumerable<_DecorationSlot__input_decorator>>(System.Enum.GetValues<_DecorationSlot__input_decorator>().ToList());
    public override global::Doroti.Framework.Widgets.Widget? childForSlot(_DecorationSlot__input_decorator slot)
    {
        return (slot switch { _DecorationSlot__input_decorator.icon => ((_Decoration__input_decorator)this.decoration).icon, _DecorationSlot__input_decorator.input => ((_Decoration__input_decorator)this.decoration).input, _DecorationSlot__input_decorator.label => ((_Decoration__input_decorator)this.decoration).label, _DecorationSlot__input_decorator.hint => ((_Decoration__input_decorator)this.decoration).hint, _DecorationSlot__input_decorator.prefix => ((_Decoration__input_decorator)this.decoration).prefix, _DecorationSlot__input_decorator.suffix => ((_Decoration__input_decorator)this.decoration).suffix, _DecorationSlot__input_decorator.prefixIcon => ((_Decoration__input_decorator)this.decoration).prefixIcon, _DecorationSlot__input_decorator.suffixIcon => ((_Decoration__input_decorator)this.decoration).suffixIcon, _DecorationSlot__input_decorator.helperError => ((_Decoration__input_decorator)this.decoration).helperError, _DecorationSlot__input_decorator.counter => ((_Decoration__input_decorator)this.decoration).counter, _DecorationSlot__input_decorator.container => ((_Decoration__input_decorator)this.decoration).container, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderDecoration__input_decorator(decoration: this.decoration, textDirection: this.textDirection, textBaseline: this.textBaseline, textAlignVertical: this.textAlignVertical, isFocused: this.isFocused, expands: this.expands, material3: Theme.of(context).useMaterial3));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderDecoration__input_decorator)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderDecoration__input_decorator>)(() =>
{
    var __cascade = __renderObject;
    __cascade.decoration = this.decoration;
    __cascade.expands = this.expands;
    __cascade.isFocused = this.isFocused;
    __cascade.textAlignVertical = this.textAlignVertical;
    __cascade.textBaseline = this.textBaseline;
    __cascade.textDirection = this.textDirection;
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
        return ((global::Doroti.Framework.Widgets.Widget)(object?)DefaultTextStyle.merge(style: this.style, child: new global::Doroti.Framework.Widgets.IgnorePointer(ignoring: !this.labelIsFloating, child: new global::Doroti.Framework.Widgets.AnimatedOpacity(duration: Input_decoratorLibrary._kTransitionDuration, curve: Input_decoratorLibrary._kTransitionCurve, opacity: (this.labelIsFloating ? 1.0 : 0.0), child: new global::Doroti.Framework.Widgets.Semantics(sortKey: this.semanticsSortKey, tagForChildren: this.semanticsTag, child: (this.child ?? (((this.text is null) ? null : new global::Doroti.Framework.Widgets.Text(this.text!, style: this.style)))))))));
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

    internal virtual bool _labelShouldWithdraw => DartRuntimePrimitives.ConvertValue<bool>((!this.isEmpty || ((this.isFocused && ((InputDecoration)this.decoration).enabled))));
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _InputDecoratorState__input_decorator());
    public static global::Doroti.Framework.Rendering.RenderBox? containerOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _RenderDecoration__input_decorator? result__67828 = ((_RenderDecoration__input_decorator?)(object?)context.findAncestorRenderObjectOfType<_RenderDecoration__input_decorator>());
        return result__67828?.container;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputDecoration>("decoration", this.decoration));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("baseStyle", this.baseStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("isFocused", this.isFocused));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("expands", this.expands, defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("isEmpty", this.isEmpty));
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
        this._floatingLabelController.addListener(() => this._handleChange());
        _floatingLabelAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._floatingLabelController, curve: Input_decoratorLibrary._kTransitionCurve, reverseCurve: ((global::Doroti.Framework.Animation.Curve)Input_decoratorLibrary._kTransitionCurve).flipped);
        _shakingLabelController = new global::Doroti.Framework.Animation.AnimationController(duration: Input_decoratorLibrary._kTransitionDuration, vsync: this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _effectiveDecoration = null;
        bool labelIsInitiallyFloating__70244 = ((!object.Equals(((InputDecoration)this.decoration).floatingLabelBehavior, FloatingLabelBehavior.never)) && this.labelShouldWithdraw);
        this._floatingLabelController.value = (labelIsInitiallyFloating__70244 ? 1.0 : 0.0);
    }

    public override void dispose()
    {
        this._floatingLabelController.dispose();
        this._floatingLabelAnimation.dispose();
        this._shakingLabelController.dispose();
        this._borderGap.dispose();
        this._curvedAnimation?.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _handleChange()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public virtual InputDecoration decoration => _effectiveDecoration ??= ((InputDecorator)this.widget).decoration.applyDefaults(InputDecorationTheme.of(this.context));
    public virtual global::Doroti.Ui.TextAlign? textAlign => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextAlign>(((InputDecorator)this.widget).textAlign);
    public virtual bool isFocused => ((InputDecorator)this.widget).isFocused;
    internal virtual bool _hasError => DartRuntimePrimitives.ConvertValue<bool>(((((InputDecoration)this.decoration).errorText is not null) || (((InputDecoration)this.decoration).error is not null)));
    public virtual bool isHovering => DartRuntimePrimitives.ConvertValue<bool>((((InputDecorator)this.widget).isHovering && ((InputDecoration)this.decoration).enabled));
    public virtual bool isEmpty => ((InputDecorator)this.widget).isEmpty;
    internal virtual bool _floatingLabelEnabled
    {
        get
        {
            return (!object.Equals(((InputDecoration)this.decoration).floatingLabelBehavior, FloatingLabelBehavior.never));
            return default!;
        }
    }
    public virtual bool labelShouldWithdraw => DartRuntimePrimitives.ConvertValue<bool>((((InputDecorator)this.widget)._labelShouldWithdraw || (object.Equals(((InputDecoration)this.decoration).floatingLabelBehavior, FloatingLabelBehavior.always))));
    public override void didUpdateWidget(InputDecorator old)
    {
        base.didUpdateWidget(old);
        if ((!object.Equals(((InputDecorator)this.widget).decoration, ((InputDecorator)old).decoration)))
        {
            _effectiveDecoration = null;
        }
        var floatBehaviorChanged__71696 = (!object.Equals(((InputDecorator)this.widget).decoration.floatingLabelBehavior, ((InputDecorator)old).decoration.floatingLabelBehavior));
        if (((((InputDecorator)this.widget)._labelShouldWithdraw != ((InputDecorator)old)._labelShouldWithdraw) || floatBehaviorChanged__71696))
        {
            if ((this._floatingLabelEnabled && this.labelShouldWithdraw))
            {
                this._floatingLabelController.forward();
            }
            else
            {
                this._floatingLabelController.reverse();
            }
        }
        string? errorText__72094 = ((InputDecoration)this.decoration).errorText;
        string? oldErrorText__72146 = ((InputDecorator)old).decoration.errorText;
        if (((this._floatingLabelController.isCompleted && (errorText__72094 is not null)) && (errorText__72094 != oldErrorText__72146)))
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = this._shakingLabelController;
    __cascade.value = 0.0;
    __cascade.forward();
    return __cascade;
}))());
        }
    }

    internal virtual global::Doroti.Ui.Color _getDefaultM2BorderColor(ThemeData themeData)
    {
        if ((!((InputDecoration)this.decoration).enabled && !this.isFocused))
        {
            return ((global::Doroti.Ui.Color)(object?)(((((((InputDecoration)this.decoration).filled ?? false)) && !((((InputDecoration)this.decoration).border?.isOutline ?? false)))) ? Colors.transparent : themeData.disabledColor));
        }
        if (this._hasError)
        {
            return ((global::Doroti.Ui.Color)(object?)themeData.colorScheme.error);
        }
        if (this.isFocused)
        {
            return ((global::Doroti.Ui.Color)(object?)themeData.colorScheme.primary);
        }
        if (DartRuntimePrimitives.RequireValue(((InputDecoration)this.decoration).filled))
        {
            return ((global::Doroti.Ui.Color)(object?)themeData.hintColor);
        }
        global::Doroti.Ui.Color enabledColor__72859 = ((global::Doroti.Ui.Color)(object?)themeData.colorScheme.onSurface.withOpacity(0.38));
        if (this.isHovering)
        {
            global::Doroti.Ui.Color hoverColor__72965 = ((global::Doroti.Ui.Color)(object?)(((InputDecoration)this.decoration).hoverColor ?? themeData.hoverColor));
            return ((global::Doroti.Ui.Color)(object?)Dart_uiLibrary.Color.alphaBlend(hoverColor__72965.withOpacity(0.12), enabledColor__72859));
        }
        return ((global::Doroti.Ui.Color)(object?)enabledColor__72859);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _getFillColor(ThemeData themeData, InputDecorationThemeData defaults)
    {
        if ((((InputDecoration)this.decoration).filled != true))
        {
            return ((global::Doroti.Ui.Color)(object?)Colors.transparent);
        }
        if ((((InputDecoration)this.decoration).fillColor is not null))
        {
            return ((global::Doroti.Ui.Color)(object?)WidgetStateProperty.resolveAs(((InputDecoration)this.decoration).fillColor!, this.widgetState));
        }
        return ((global::Doroti.Ui.Color)(object?)WidgetStateProperty.resolveAs(((InputDecorationThemeData)defaults).fillColor!, this.widgetState));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _getHoverColor(ThemeData themeData)
    {
        if ((((((InputDecoration)this.decoration).filled is null) || !DartRuntimePrimitives.RequireValue(((InputDecoration)this.decoration).filled)) || !((InputDecoration)this.decoration).enabled))
        {
            return ((global::Doroti.Ui.Color)(object?)Colors.transparent);
        }
        return ((global::Doroti.Ui.Color)(object?)(((InputDecoration)this.decoration).hoverColor ?? themeData.hoverColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _getIconColor(ThemeData themeData, InputDecorationThemeData defaults)
    {
        return ((global::Doroti.Ui.Color)(object?)((WidgetStateProperty.resolveAs(((InputDecoration)this.decoration).iconColor, this.widgetState) ?? (Color)WidgetStateProperty.resolveAs(((InputDecorationThemeData)defaults).iconColor!, this.widgetState))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _getPrefixIconColor(IconButtonThemeData iconButtonTheme, InputDecorationThemeData defaults)
    {
        return ((global::Doroti.Ui.Color)(object?)((((WidgetStateProperty.resolveAs(((InputDecoration)this.decoration).prefixIconColor, this.widgetState) ?? (Color)iconButtonTheme.style?.foregroundColor?.resolve(this.widgetState))) ?? (Color)WidgetStateProperty.resolveAs(((InputDecorationThemeData)defaults).prefixIconColor!, this.widgetState))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _getSuffixIconColor(IconButtonThemeData iconButtonTheme, InputDecorationThemeData defaults)
    {
        return ((global::Doroti.Ui.Color)(object?)((((WidgetStateProperty.resolveAs(((InputDecoration)this.decoration).suffixIconColor, this.widgetState) ?? (Color)iconButtonTheme.style?.foregroundColor?.resolve(this.widgetState))) ?? (Color)WidgetStateProperty.resolveAs(((InputDecorationThemeData)defaults).suffixIconColor!, this.widgetState))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasInlineLabel
    {
        get
        {
            return (!this.labelShouldWithdraw && (((((InputDecoration)this.decoration).labelText is not null) || (((InputDecoration)this.decoration).label is not null))));
            return default!;
        }
    }
    internal virtual bool _shouldShowLabel => DartRuntimePrimitives.ConvertValue<bool>((this._hasInlineLabel || this._floatingLabelEnabled));
    internal virtual global::Doroti.Framework.Painting.TextStyle _getInlineLabelStyle(ThemeData themeData, InputDecorationThemeData defaults)
    {
        global::Doroti.Framework.Painting.TextStyle defaultStyle__75471 = ((global::Doroti.Framework.Painting.TextStyle)(object?)WidgetStateProperty.resolveAs(((InputDecorationThemeData)defaults).labelStyle!, this.widgetState));
        global::Doroti.Framework.Painting.TextStyle? style__75574 = ((global::Doroti.Framework.Painting.TextStyle?)(object?)WidgetStateProperty.resolveAs(((InputDecoration)this.decoration).labelStyle, this.widgetState));
        return ((global::Doroti.Framework.Painting.TextStyle)(object?)themeData.textTheme.titleMedium!.merge(((InputDecorator)this.widget).baseStyle).merge(defaultStyle__75471).merge(style__75574).copyWith(height: 1));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _getInlineHintStyle(ThemeData themeData, InputDecorationThemeData defaults)
    {
        global::Doroti.Framework.Painting.TextStyle defaultStyle__76057 = ((global::Doroti.Framework.Painting.TextStyle)(object?)WidgetStateProperty.resolveAs(((InputDecorationThemeData)defaults).hintStyle!, this.widgetState));
        global::Doroti.Framework.Painting.TextStyle? style__76159 = ((global::Doroti.Framework.Painting.TextStyle?)(object?)WidgetStateProperty.resolveAs(((InputDecoration)this.decoration).hintStyle, this.widgetState));
        return ((global::Doroti.Framework.Painting.TextStyle)(object?)((themeData.useMaterial3 ? themeData.textTheme.bodyLarge! : themeData.textTheme.titleMedium!)).merge(((InputDecorator)this.widget).baseStyle).merge(defaultStyle__76057).merge(style__76159));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _getFloatingLabelStyle(ThemeData themeData, InputDecorationThemeData defaults)
    {
        global::Doroti.Framework.Painting.TextStyle defaultTextStyle__76559 = ((global::Doroti.Framework.Painting.TextStyle)(object?)WidgetStateProperty.resolveAs(((InputDecorationThemeData)defaults).floatingLabelStyle!, this.widgetState));
        if ((this._hasError && (((InputDecoration)this.decoration).errorStyle?.color is not null)))
        {
            defaultTextStyle__76559 = defaultTextStyle__76559.copyWith(color: ((InputDecoration)this.decoration).errorStyle?.color);
        }
        defaultTextStyle__76559 = defaultTextStyle__76559.merge((((InputDecoration)this.decoration).floatingLabelStyle ?? ((InputDecoration)this.decoration).labelStyle));
        global::Doroti.Framework.Painting.TextStyle? style__76965 = ((global::Doroti.Framework.Painting.TextStyle?)(object?)WidgetStateProperty.resolveAs(((InputDecoration)this.decoration).floatingLabelStyle, this.widgetState));
        return ((global::Doroti.Framework.Painting.TextStyle)(object?)themeData.textTheme.titleMedium!.merge(((InputDecorator)this.widget).baseStyle).merge(defaultTextStyle__76559).merge(style__76965).copyWith(height: 1));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _getHelperStyle(ThemeData themeData, InputDecorationThemeData defaults)
    {
        return ((global::Doroti.Framework.Painting.TextStyle)(object?)WidgetStateProperty.resolveAs(((InputDecorationThemeData)defaults).helperStyle!, this.widgetState).merge(WidgetStateProperty.resolveAs(((InputDecoration)this.decoration).helperStyle, this.widgetState)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _getErrorStyle(ThemeData themeData, InputDecorationThemeData defaults)
    {
        return ((global::Doroti.Framework.Painting.TextStyle)(object?)WidgetStateProperty.resolveAs(((InputDecorationThemeData)defaults).errorStyle!, this.widgetState).merge(((InputDecoration)this.decoration).errorStyle));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual HashSet<global::Doroti.Framework.Widgets.WidgetState> widgetState => ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection77750 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (!((InputDecoration)this.decoration).enabled) { __collection77750.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } if (this.isFocused) { __collection77750.Add(global::Doroti.Framework.Widgets.WidgetState.focused); } if (this.isHovering) { __collection77750.Add(global::Doroti.Framework.Widgets.WidgetState.hovered); } if (this._hasError) { __collection77750.Add(global::Doroti.Framework.Widgets.WidgetState.error); } return __collection77750; }))();
    internal virtual InputBorder _getDefaultBorder(ThemeData themeData, InputDecorationThemeData defaults)
    {
        InputBorder border__78053 = (WidgetStateProperty.resolveAs(((InputDecoration)this.decoration).border, this.widgetState) ?? new UnderlineInputBorder());
        if ((((InputDecoration)this.decoration).border is global::Doroti.Framework.Widgets.WidgetStateProperty<InputBorder>))
        {
            return border__78053;
        }
        if ((object.Equals(border__78053.borderSide, global::Doroti.Framework.Painting.BorderSide.none)))
        {
            return border__78053;
        }
        if (themeData.useMaterial3)
        {
            if (DartRuntimePrimitives.RequireValue(((InputDecoration)this.decoration).filled))
            {
                InputDecorationThemeData decorationTheme__78448 = ((InputDecorationThemeData)(object?)InputDecorationTheme.of(this.context));
                return border__78053.copyWith(borderSide: WidgetStateProperty.resolveAs((((InputDecorationThemeData)decorationTheme__78448).activeIndicatorBorder ?? ((InputDecorationThemeData)defaults).activeIndicatorBorder), this.widgetState));
            }
            else
            {
                return border__78053.copyWith(borderSide: WidgetStateProperty.resolveAs(((InputDecorationThemeData)defaults).outlineBorder, this.widgetState));
            }
        }
        else
        {
            return border__78053.copyWith(borderSide: new global::Doroti.Framework.Painting.BorderSide(color: _getDefaultM2BorderColor(themeData), width: (((((DartRuntimePrimitives.RequireValue(((InputDecoration)this.decoration).isCollapsed)) || (object.Equals(((InputDecoration)this.decoration).border, InputBorder.none))) || !((InputDecoration)this.decoration).enabled)) ? 0.0 : (this.isFocused ? 2.0 : 1.0))));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.FadeTransition _buildTransition(global::Doroti.Framework.Widgets.Widget child, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        if ((!object.Equals(this._curvedAnimation?.parent, animation)))
        {
            this._curvedAnimation?.dispose();
            _curvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: animation, curve: Input_decoratorLibrary._kTransitionCurve);
        }
        return new global::Doroti.Framework.Widgets.FadeTransition(opacity: this._curvedAnimation!, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Widgets.Widget _topStartLayout(global::Doroti.Framework.Widgets.Widget? currentChild, List<global::Doroti.Framework.Widgets.Widget> previousChildren)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection79764 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection79764.AddRange(previousChildren); var __collectionElement79794 = currentChild; if (__collectionElement79794 is { } __nonNullCollectionElement79794) { __collection79764.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement79794)); } return __collection79764; }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData themeData__79887 = Theme.of(context);
        VisualDensity visualDensity__79942 = (((InputDecoration)this.decoration).visualDensity ?? themeData__79887.visualDensity);
        bool useMaterial3__80026 = Theme.of(context).useMaterial3;
        InputDecorationThemeData defaults__80108 = (useMaterial3__80026 ? new _InputDecoratorDefaultsM3__input_decorator(context) : new _InputDecoratorDefaultsM2__input_decorator(context));
        IconButtonThemeData iconButtonTheme__80253 = IconButtonTheme.of(context);
        global::Doroti.Framework.Painting.TextStyle labelStyle__80321 = ((global::Doroti.Framework.Painting.TextStyle)(object?)_getInlineLabelStyle(themeData__79887, defaults__80108));
        global::Doroti.Ui.TextBaseline textBaseline__80400 = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Painting.TextStyle)labelStyle__80321).textBaseline);
        global::Doroti.Framework.Painting.TextStyle hintStyle__80462 = ((global::Doroti.Framework.Painting.TextStyle)(object?)_getInlineHintStyle(themeData__79887, defaults__80108));
        string? hintText__80534 = ((InputDecoration)this.decoration).hintText;
        bool maintainHintSize__80581 = ((InputDecoration)this.decoration).maintainHintSize;
        global::Doroti.Framework.Widgets.Widget? hint__80641 = default!;
        if (((((InputDecoration)this.decoration).hint is not null) || (hintText__80534 is not null)))
        {
            global::Doroti.Framework.Widgets.Widget hintWidget__80721 = (((InputDecoration)this.decoration).hint ?? new global::Doroti.Framework.Widgets.Text(hintText__80534!, style: hintStyle__80462, textDirection: ((InputDecoration)this.decoration).hintTextDirection, overflow: (((global::Doroti.Framework.Painting.TextStyle)hintStyle__80462).overflow ?? (((((InputDecoration)this.decoration).hintMaxLines is null) ? null : global::Doroti.Framework.Painting.TextOverflow.ellipsis))), textAlign: this.textAlign, maxLines: ((InputDecoration)this.decoration).hintMaxLines));
            bool showHint__81142 = (this.isEmpty && !this._hasInlineLabel);
            hint__80641 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>((maintainHintSize__80581 ? new global::Doroti.Framework.Widgets.AnimatedOpacity(opacity: (showHint__81142 ? 1.0 : 0.0), duration: (((InputDecoration)this.decoration).hintFadeDuration ?? Input_decoratorLibrary._kHintFadeTransitionDuration), curve: Input_decoratorLibrary._kTransitionCurve, child: hintWidget__80721) : new global::Doroti.Framework.Widgets.AnimatedSwitcher(duration: (((InputDecoration)this.decoration).hintFadeDuration ?? Input_decoratorLibrary._kHintFadeTransitionDuration), transitionBuilder: (global::System.Func<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.FadeTransition>)this._buildTransition, layoutBuilder: (global::System.Func<global::Doroti.Framework.Widgets.Widget?, List<global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget>)_topStartLayout, child: (showHint__81142 ? hintWidget__80721 : global::Doroti.Framework.Widgets.SizedBox.CreateShrink()))));
        }
        InputBorder? border__81779 = default!;
        if (!((InputDecoration)this.decoration).enabled)
        {
            border__81779 = (this._hasError ? ((InputDecoration)this.decoration).errorBorder : ((InputDecoration)this.decoration).disabledBorder);
        }
        else
        {
            if (this.isFocused)
            {
                border__81779 = (this._hasError ? ((InputDecoration)this.decoration).focusedErrorBorder : ((InputDecoration)this.decoration).focusedBorder);
            }
            else
            {
                border__81779 = (this._hasError ? ((InputDecoration)this.decoration).errorBorder : ((InputDecoration)this.decoration).enabledBorder);
            }
        }
        border__81779 ??= _getDefaultBorder(themeData__79887, defaults__80108);
        global::Doroti.Framework.Widgets.Widget container__82180 = ((global::Doroti.Framework.Widgets.Widget)(object?)new _BorderContainer__input_decorator(border: border__81779, gap: this._borderGap, gapAnimation: this._floatingLabelAnimation, fillColor: _getFillColor(themeData__79887, defaults__80108), hoverColor: _getHoverColor(themeData__79887), isHovering: this.isHovering));
        global::Doroti.Framework.Widgets.Widget? label__82448 = default!;
        if ((((((object?)((InputDecoration)this.decoration).labelText ?? (object?)((InputDecoration)this.decoration).label))) is not null))
        {
            label__82448 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MatrixTransition(animation: this._shakingLabelController, onTransform: ((global::System.Func<double, Matrix4>)((value) =>
            {
                double shakeOffset__82654 = (value switch { <= 0.25 => -value, < 0.75 => (value - 0.5), _ => (((1.0 - value)) * 4.0) });
                return Matrix4.translationValues((shakeOffset__82654 * 4.0), 0.0, 0.0);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })), child: new global::Doroti.Framework.Widgets.AnimatedOpacity(duration: Input_decoratorLibrary._kTransitionDuration, curve: Input_decoratorLibrary._kTransitionCurve, opacity: (this._shouldShowLabel ? 1.0 : 0.0), child: new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(duration: Input_decoratorLibrary._kTransitionDuration, curve: Input_decoratorLibrary._kTransitionCurve, style: (this.labelShouldWithdraw ? _getFloatingLabelStyle(themeData__79887, defaults__80108) : labelStyle__80321), child: (((InputDecoration)this.decoration).label ?? new global::Doroti.Framework.Widgets.Text(((InputDecoration)this.decoration).labelText!, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, textAlign: this.textAlign))))));
        }
        bool hasPrefix__83587 = ((((InputDecoration)this.decoration).prefix is not null) || (((InputDecoration)this.decoration).prefixText is not null));
        bool hasSuffix__83674 = ((((InputDecoration)this.decoration).suffix is not null) || (((InputDecoration)this.decoration).suffixText is not null));
        global::Doroti.Framework.Widgets.Widget? input__83759 = ((InputDecorator)this.widget).child;
        bool needsSemanticsSortOrder__83887 = (this.labelShouldWithdraw && (((input__83759 is not null) ? ((hasPrefix__83587 || hasSuffix__83674)) : ((hasPrefix__83587 && hasSuffix__83674)))));
        global::Doroti.Framework.Widgets.Widget? prefix__84042 = ((global::Doroti.Framework.Widgets.Widget?)(object?)(hasPrefix__83587 ? new _AffixText__input_decorator(labelIsFloating: this.labelShouldWithdraw, text: ((InputDecoration)this.decoration).prefixText, style: (WidgetStateProperty.resolveAs(((InputDecoration)this.decoration).prefixStyle, this.widgetState) ?? hintStyle__80462), semanticsSortKey: (needsSemanticsSortOrder__83887 ? this._prefixSemanticsSortOrder : null), semanticsTag: _kPrefixSemanticsTag, child: ((InputDecoration)this.decoration).prefix) : null));
        global::Doroti.Framework.Widgets.Widget? suffix__84497 = ((global::Doroti.Framework.Widgets.Widget?)(object?)(hasSuffix__83674 ? new _AffixText__input_decorator(labelIsFloating: this.labelShouldWithdraw, text: ((InputDecoration)this.decoration).suffixText, style: (WidgetStateProperty.resolveAs(((InputDecoration)this.decoration).suffixStyle, this.widgetState) ?? hintStyle__80462), semanticsSortKey: (needsSemanticsSortOrder__83887 ? this._suffixSemanticsSortOrder : null), semanticsTag: _kSuffixSemanticsTag, child: ((InputDecoration)this.decoration).suffix) : null));
        if (((input__83759 is not null) && needsSemanticsSortOrder__83887))
        {
            input__83759 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(sortKey: this._inputSemanticsSortOrder, child: input__83759));
        }
        bool decorationIsDense__85082 = (((InputDecoration)this.decoration).isDense ?? false);
        var iconSize__85141 = (decorationIsDense__85082 ? 18.0 : 24.0);
        global::Doroti.Framework.Widgets.Widget? icon__85204 = ((global::Doroti.Framework.Widgets.Widget?)(object?)((((InputDecoration)this.decoration).icon is null) ? null : new global::Doroti.Framework.Widgets.MouseRegion(cursor: global::Doroti.Framework.Services.SystemMouseCursors.basic, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(end: 16.0), child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(color: _getIconColor(themeData__79887, defaults__80108), size: iconSize__85141), child: ((InputDecoration)this.decoration).icon!)))));
        global::Doroti.Framework.Widgets.Widget? prefixIcon__85654 = ((global::Doroti.Framework.Widgets.Widget?)(object?)((((InputDecoration)this.decoration).prefixIcon is null) ? null : new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: new global::Doroti.Framework.Widgets.MouseRegion(cursor: global::Doroti.Framework.Services.SystemMouseCursors.basic, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: (((InputDecoration)this.decoration).prefixIconConstraints ?? visualDensity__79942.effectiveConstraints(new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: ConstantsLibrary.kMinInteractiveDimension, minHeight: ConstantsLibrary.kMinInteractiveDimension))), child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(color: _getPrefixIconColor(iconButtonTheme__80253, defaults__80108), size: iconSize__85141), child: new IconButtonTheme(data: new IconButtonThemeData(style: new ButtonStyle(foregroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(_getPrefixIconColor(iconButtonTheme__80253, defaults__80108)), iconSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(iconSize__85141)).merge(iconButtonTheme__80253.style)), child: new global::Doroti.Framework.Widgets.Semantics(tagForChildren: _kPrefixIconSemanticsTag, child: ((InputDecoration)this.decoration).prefixIcon))))))));
        global::Doroti.Framework.Widgets.Widget? suffixIcon__87211 = ((global::Doroti.Framework.Widgets.Widget?)(object?)((((InputDecoration)this.decoration).suffixIcon is null) ? null : new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: new global::Doroti.Framework.Widgets.MouseRegion(cursor: global::Doroti.Framework.Services.SystemMouseCursors.basic, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: (((InputDecoration)this.decoration).suffixIconConstraints ?? visualDensity__79942.effectiveConstraints(new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: ConstantsLibrary.kMinInteractiveDimension, minHeight: ConstantsLibrary.kMinInteractiveDimension))), child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(color: _getSuffixIconColor(iconButtonTheme__80253, defaults__80108), size: iconSize__85141), child: new IconButtonTheme(data: new IconButtonThemeData(style: new ButtonStyle(foregroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(_getSuffixIconColor(iconButtonTheme__80253, defaults__80108)), iconSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(iconSize__85141)).merge(iconButtonTheme__80253.style)), child: new global::Doroti.Framework.Widgets.Semantics(tagForChildren: _kSuffixIconSemanticsTag, child: ((InputDecoration)this.decoration).suffixIcon))))))));
        global::Doroti.Framework.Widgets.Widget helperError__88767 = ((global::Doroti.Framework.Widgets.Widget)(object?)new _HelperError__input_decorator(textAlign: this.textAlign, helper: ((InputDecoration)this.decoration).helper, helperText: ((InputDecoration)this.decoration).helperText, helperStyle: _getHelperStyle(themeData__79887, defaults__80108), helperMaxLines: ((InputDecoration)this.decoration).helperMaxLines, error: ((InputDecoration)this.decoration).error, errorText: ((InputDecoration)this.decoration).errorText, errorStyle: _getErrorStyle(themeData__79887, defaults__80108), errorMaxLines: ((InputDecoration)this.decoration).errorMaxLines));
        global::Doroti.Framework.Widgets.Widget? counter__89195 = default!;
        if ((((InputDecoration)this.decoration).counter is not null))
        {
            counter__89195 = ((InputDecoration)this.decoration).counter;
        }
        else
        {
            if (((((InputDecoration)this.decoration).counterText is not null) && (((InputDecoration)this.decoration).counterText != "")))
            {
                counter__89195 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(container: true, liveRegion: this.isFocused, child: new global::Doroti.Framework.Widgets.Text(((InputDecoration)this.decoration).counterText!, style: _getHelperStyle(themeData__79887, defaults__80108).merge(WidgetStateProperty.resolveAs(((InputDecoration)this.decoration).counterStyle, this.widgetState)), overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, semanticsLabel: ((InputDecoration)this.decoration).semanticCounterText)));
            }
        }
        global::Doroti.Ui.TextDirection textDirection__89936 = Directionality.of(context);
        bool flipHorizontal__89995 = (textDirection__89936 switch { TextDirection.ltr => false, TextDirection.rtl => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Painting.EdgeInsets? resolvedPadding__90133 = ((global::Doroti.Framework.Painting.EdgeInsets?)(object?)((InputDecoration)this.decoration).contentPadding?.resolve(textDirection__89936));
        global::Doroti.Framework.Painting.EdgeInsetsDirectional? decorationContentPadding__90235 = ((resolvedPadding__90133 is null) ? null : new global::Doroti.Framework.Painting.EdgeInsetsDirectional((flipHorizontal__89995 ? ((global::Doroti.Framework.Painting.EdgeInsets)resolvedPadding__90133).right : ((global::Doroti.Framework.Painting.EdgeInsets)resolvedPadding__90133).left), ((global::Doroti.Framework.Painting.EdgeInsets)resolvedPadding__90133).top, (flipHorizontal__89995 ? ((global::Doroti.Framework.Painting.EdgeInsets)resolvedPadding__90133).left : ((global::Doroti.Framework.Painting.EdgeInsets)resolvedPadding__90133).right), ((global::Doroti.Framework.Painting.EdgeInsets)resolvedPadding__90133).bottom));
        global::Doroti.Framework.Painting.EdgeInsetsDirectional contentPadding__90608 = default!;
        double floatingLabelHeight__90641 = default!;
        if (DartRuntimePrimitives.RequireValue(((InputDecoration)this.decoration).isCollapsed))
        {
            floatingLabelHeight__90641 = 0.0;
            contentPadding__90608 = (decorationContentPadding__90235 ?? global::Doroti.Framework.Painting.EdgeInsetsDirectional.zero);
        }
        else
        {
            if (!border__81779.isOutline)
            {
                floatingLabelHeight__90641 = MediaQuery.textScalerOf(context).scale((4.0 + (0.75 * DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Painting.TextStyle)labelStyle__80321).fontSize))));
                if ((((InputDecoration)this.decoration).filled ?? false))
                {
                    contentPadding__90608 = (decorationContentPadding__90235 ?? ((useMaterial3__80026 ? (decorationIsDense__85082 ? new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12.0, 4.0, 12.0, 4.0) : new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12.0, 8.0, 12.0, 8.0)) : (decorationIsDense__85082 ? new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12.0, 8.0, 12.0, 8.0) : new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12.0, 12.0, 12.0, 12.0)))));
                }
                else
                {
                    contentPadding__90608 = (decorationContentPadding__90235 ?? ((useMaterial3__80026 ? (decorationIsDense__85082 ? new global::Doroti.Framework.Painting.EdgeInsetsDirectional(0.0, 4.0, 0.0, 4.0) : new global::Doroti.Framework.Painting.EdgeInsetsDirectional(0.0, 8.0, 0.0, 8.0)) : (decorationIsDense__85082 ? new global::Doroti.Framework.Painting.EdgeInsetsDirectional(0.0, 8.0, 0.0, 8.0) : new global::Doroti.Framework.Painting.EdgeInsetsDirectional(0.0, 12.0, 0.0, 12.0)))));
                }
            }
            else
            {
                floatingLabelHeight__90641 = 0.0;
                contentPadding__90608 = (decorationContentPadding__90235 ?? ((useMaterial3__80026 ? (decorationIsDense__85082 ? new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12.0, 16.0, 12.0, 8.0) : new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12.0, 20.0, 12.0, 12.0)) : (decorationIsDense__85082 ? new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12.0, 20.0, 12.0, 12.0) : new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12.0, 24.0, 12.0, 16.0)))));
            }
        }
        var inputGap__92834 = 0.0;
        if (useMaterial3__80026)
        {
            if ((border__81779 is OutlineInputBorder))
            {
                OutlineInputBorder border__81779__as92884 = (OutlineInputBorder)border__81779;
                inputGap__92834 = border__81779__as92884.gapPadding;
            }
            else
            {
                inputGap__92834 = ((border__81779.isOutline || ((((InputDecoration)this.decoration).filled ?? false))) ? Input_decoratorLibrary._kInputExtraPadding : 0.0);
            }
        }
        var decorator__93091 = new _Decorator__input_decorator(decoration: new _Decoration__input_decorator(contentPadding: contentPadding__90608, isCollapsed: DartRuntimePrimitives.RequireValue(((InputDecoration)this.decoration).isCollapsed), inputGap: inputGap__92834, floatingLabelHeight: floatingLabelHeight__90641, floatingLabelAlignment: ((InputDecoration)this.decoration).floatingLabelAlignment!, floatingLabelProgress: ((global::Doroti.Framework.Animation.CurvedAnimation)this._floatingLabelAnimation).value, border: border__81779, borderGap: this._borderGap, alignLabelWithHint: (((InputDecoration)this.decoration).alignLabelWithHint ?? false), isDense: ((InputDecoration)this.decoration).isDense, isEmpty: this.isEmpty, visualDensity: visualDensity__79942, maintainHintSize: maintainHintSize__80581, maintainLabelSize: ((InputDecoration)this.decoration).maintainLabelSize, icon: icon__85204, input: input__83759, label: label__82448, hint: hint__80641, prefix: prefix__84042, suffix: suffix__84497, prefixIcon: prefixIcon__85654, suffixIcon: suffixIcon__87211, helperError: helperError__88767, counter: counter__89195, container: container__82180), textDirection: textDirection__89936, textBaseline: textBaseline__80400, textAlignVertical: ((InputDecorator)this.widget).textAlignVertical, isFocused: this.isFocused, expands: ((InputDecorator)this.widget).expands);
        string? semanticsHint__94938 = ((InputDecoration)this.decoration).errorText;
        global::Doroti.Framework.Widgets.Widget result__94994 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(hint: semanticsHint__94938, child: decorator__93091));
        global::Doroti.Framework.Rendering.BoxConstraints? constraints__95080 = ((InputDecoration)this.decoration).constraints;
        if ((constraints__95080 is not null))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: constraints__95080, child: result__94994));
        }
        return result__94994;
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
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values__17506).enabled;
    __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
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
        System.Diagnostics.Debug.Assert(!(((label is not null) && (labelText is not null))));
        System.Diagnostics.Debug.Assert(((hint is null) || (hintText is null)));
        System.Diagnostics.Debug.Assert(!(((helper is not null) && (helperText is not null))));
        System.Diagnostics.Debug.Assert(!(((prefix is not null) && (prefixText is not null))));
        System.Diagnostics.Debug.Assert(!(((suffix is not null) && (suffixText is not null))));
        System.Diagnostics.Debug.Assert(!(((error is not null) && (errorText is not null))));
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
        __instance.contentPadding = global::Doroti.Framework.Painting.EdgeInsets.zero;
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
        return new InputDecoration(icon: (icon ?? this.icon), iconColor: (iconColor ?? this.iconColor), label: (label ?? this.label), labelText: (labelText ?? this.labelText), labelStyle: (labelStyle ?? this.labelStyle), floatingLabelStyle: (floatingLabelStyle ?? this.floatingLabelStyle), helper: (helper ?? this.helper), helperText: (helperText ?? this.helperText), helperStyle: (helperStyle ?? this.helperStyle), helperMaxLines: (helperMaxLines ?? this.helperMaxLines), hintText: (hintText ?? this.hintText), hint: (hint ?? this.hint), hintStyle: (hintStyle ?? this.hintStyle), hintTextDirection: (hintTextDirection ?? this.hintTextDirection), hintMaxLines: (hintMaxLines ?? this.hintMaxLines), hintFadeDuration: (hintFadeDuration ?? this.hintFadeDuration), maintainHintHeight: (maintainHintHeight ?? this.maintainHintHeight), maintainHintSize: (maintainHintSize ?? this.maintainHintSize), maintainLabelSize: (maintainLabelSize ?? this.maintainLabelSize), error: (error ?? this.error), errorText: (errorText ?? this.errorText), errorStyle: (errorStyle ?? this.errorStyle), errorMaxLines: (errorMaxLines ?? this.errorMaxLines), floatingLabelBehavior: (floatingLabelBehavior ?? this.floatingLabelBehavior), floatingLabelAlignment: (floatingLabelAlignment ?? this.floatingLabelAlignment), isCollapsed: (isCollapsed ?? this.isCollapsed), isDense: (isDense ?? this.isDense), contentPadding: (contentPadding ?? this.contentPadding), prefixIcon: (prefixIcon ?? this.prefixIcon), prefix: (prefix ?? this.prefix), prefixText: (prefixText ?? this.prefixText), prefixStyle: (prefixStyle ?? this.prefixStyle), prefixIconColor: (prefixIconColor ?? this.prefixIconColor), prefixIconConstraints: (prefixIconConstraints ?? this.prefixIconConstraints), suffixIcon: (suffixIcon ?? this.suffixIcon), suffix: (suffix ?? this.suffix), suffixText: (suffixText ?? this.suffixText), suffixStyle: (suffixStyle ?? this.suffixStyle), suffixIconColor: (suffixIconColor ?? this.suffixIconColor), suffixIconConstraints: (suffixIconConstraints ?? this.suffixIconConstraints), counter: (counter ?? this.counter), counterText: (counterText ?? this.counterText), counterStyle: (counterStyle ?? this.counterStyle), filled: (filled ?? this.filled), fillColor: (fillColor ?? this.fillColor), focusColor: (focusColor ?? this.focusColor), hoverColor: (hoverColor ?? this.hoverColor), errorBorder: (errorBorder ?? this.errorBorder), focusedBorder: (focusedBorder ?? this.focusedBorder), focusedErrorBorder: (focusedErrorBorder ?? this.focusedErrorBorder), disabledBorder: (disabledBorder ?? this.disabledBorder), enabledBorder: (enabledBorder ?? this.enabledBorder), border: (border ?? this.border), enabled: (enabled ?? this.enabled), semanticCounterText: (semanticCounterText ?? this.semanticCounterText), alignLabelWithHint: (alignLabelWithHint ?? this.alignLabelWithHint), constraints: (constraints ?? this.constraints), visualDensity: (visualDensity ?? this.visualDensity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InputDecoration applyDefaults(object inputDecorationTheme)
    {
        if (((inputDecorationTheme is not InputDecorationTheme) && (inputDecorationTheme is not InputDecorationThemeData)))
        {
            throw DartRuntimePrimitives.AsException(new DartArgumentError("inputDecorationTheme must be either a InputDecorationThemeData or a InputDecorationTheme"));
        }
        InputDecorationThemeData theme__151119 = (((inputDecorationTheme is InputDecorationTheme)) ? ((InputDecorationTheme)((InputDecorationTheme)inputDecorationTheme)).data : ((InputDecorationThemeData?)(object?)inputDecorationTheme)!);
        return ((InputDecoration)(object?)copyWith(labelStyle: (this.labelStyle ?? ((InputDecorationThemeData)theme__151119).labelStyle), floatingLabelStyle: (this.floatingLabelStyle ?? ((InputDecorationThemeData)theme__151119).floatingLabelStyle), helperStyle: (this.helperStyle ?? ((InputDecorationThemeData)theme__151119).helperStyle), helperMaxLines: (this.helperMaxLines ?? ((InputDecorationThemeData)theme__151119).helperMaxLines), hintStyle: (this.hintStyle ?? ((InputDecorationThemeData)theme__151119).hintStyle), hintFadeDuration: (this.hintFadeDuration ?? ((InputDecorationThemeData)theme__151119).hintFadeDuration), hintMaxLines: (this.hintMaxLines ?? ((InputDecorationThemeData)theme__151119).hintMaxLines), errorStyle: (this.errorStyle ?? ((InputDecorationThemeData)theme__151119).errorStyle), errorMaxLines: (this.errorMaxLines ?? ((InputDecorationThemeData)theme__151119).errorMaxLines), floatingLabelBehavior: (this.floatingLabelBehavior ?? ((InputDecorationThemeData)theme__151119).floatingLabelBehavior), floatingLabelAlignment: (this.floatingLabelAlignment ?? ((InputDecorationThemeData)theme__151119).floatingLabelAlignment), isDense: (this.isDense ?? ((InputDecorationThemeData)theme__151119).isDense), contentPadding: (this.contentPadding ?? ((InputDecorationThemeData)theme__151119).contentPadding), isCollapsed: (this.isCollapsed ?? ((InputDecorationThemeData)theme__151119).isCollapsed), iconColor: (this.iconColor ?? ((InputDecorationThemeData)theme__151119).iconColor), prefixStyle: (this.prefixStyle ?? ((InputDecorationThemeData)theme__151119).prefixStyle), prefixIconColor: (this.prefixIconColor ?? ((InputDecorationThemeData)theme__151119).prefixIconColor), prefixIconConstraints: (this.prefixIconConstraints ?? ((InputDecorationThemeData)theme__151119).prefixIconConstraints), suffixStyle: (this.suffixStyle ?? ((InputDecorationThemeData)theme__151119).suffixStyle), suffixIconColor: (this.suffixIconColor ?? ((InputDecorationThemeData)theme__151119).suffixIconColor), suffixIconConstraints: (this.suffixIconConstraints ?? ((InputDecorationThemeData)theme__151119).suffixIconConstraints), counterStyle: (this.counterStyle ?? ((InputDecorationThemeData)theme__151119).counterStyle), filled: (this.filled ?? ((InputDecorationThemeData)theme__151119).filled), fillColor: (this.fillColor ?? ((InputDecorationThemeData)theme__151119).fillColor), focusColor: (this.focusColor ?? ((InputDecorationThemeData)theme__151119).focusColor), hoverColor: (this.hoverColor ?? ((InputDecorationThemeData)theme__151119).hoverColor), errorBorder: (this.errorBorder ?? ((InputDecorationThemeData)theme__151119).errorBorder), focusedBorder: (this.focusedBorder ?? ((InputDecorationThemeData)theme__151119).focusedBorder), focusedErrorBorder: (this.focusedErrorBorder ?? ((InputDecorationThemeData)theme__151119).focusedErrorBorder), disabledBorder: (this.disabledBorder ?? ((InputDecorationThemeData)theme__151119).disabledBorder), enabledBorder: (this.enabledBorder ?? ((InputDecorationThemeData)theme__151119).enabledBorder), border: (this.border ?? ((InputDecorationThemeData)theme__151119).border), alignLabelWithHint: (this.alignLabelWithHint ?? ((InputDecorationThemeData)theme__151119).alignLabelWithHint), constraints: (this.constraints ?? ((InputDecorationThemeData)theme__151119).constraints), visualDensity: (this.visualDensity ?? ((InputDecorationThemeData)theme__151119).visualDensity)));
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((((((((((((((((((((((((((((((((((((((((((((((((__other is InputDecoration) && (object.Equals(((InputDecoration)((InputDecoration)__other)).icon, this.icon))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).iconColor, this.iconColor))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).label, this.label))) && (((InputDecoration)((InputDecoration)__other)).labelText == this.labelText)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).labelStyle, this.labelStyle))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).floatingLabelStyle, this.floatingLabelStyle))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).helper, this.helper))) && (((InputDecoration)((InputDecoration)__other)).helperText == this.helperText)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).helperStyle, this.helperStyle))) && (((InputDecoration)((InputDecoration)__other)).helperMaxLines == this.helperMaxLines)) && (((InputDecoration)((InputDecoration)__other)).hintText == this.hintText)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).hint, this.hint))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).hintStyle, this.hintStyle))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).hintTextDirection, this.hintTextDirection))) && (((InputDecoration)((InputDecoration)__other)).hintMaxLines == this.hintMaxLines)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).hintFadeDuration, this.hintFadeDuration))) && (((InputDecoration)((InputDecoration)__other)).maintainHintHeight == this.maintainHintHeight)) && (((InputDecoration)((InputDecoration)__other)).maintainHintSize == this.maintainHintSize)) && (((InputDecoration)((InputDecoration)__other)).maintainLabelSize == this.maintainLabelSize)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).error, this.error))) && (((InputDecoration)((InputDecoration)__other)).errorText == this.errorText)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).errorStyle, this.errorStyle))) && (((InputDecoration)((InputDecoration)__other)).errorMaxLines == this.errorMaxLines)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).floatingLabelBehavior, this.floatingLabelBehavior))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).floatingLabelAlignment, this.floatingLabelAlignment))) && (((InputDecoration)((InputDecoration)__other)).isDense == this.isDense)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).contentPadding, this.contentPadding))) && (((InputDecoration)((InputDecoration)__other)).isCollapsed == this.isCollapsed)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).prefixIcon, this.prefixIcon))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).prefixIconColor, this.prefixIconColor))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).prefix, this.prefix))) && (((InputDecoration)((InputDecoration)__other)).prefixText == this.prefixText)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).prefixStyle, this.prefixStyle))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).prefixIconConstraints, this.prefixIconConstraints))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).suffixIcon, this.suffixIcon))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).suffixIconColor, this.suffixIconColor))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).suffix, this.suffix))) && (((InputDecoration)((InputDecoration)__other)).suffixText == this.suffixText)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).suffixStyle, this.suffixStyle))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).suffixIconConstraints, this.suffixIconConstraints))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).counter, this.counter))) && (((InputDecoration)((InputDecoration)__other)).counterText == this.counterText)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).counterStyle, this.counterStyle))) && (((InputDecoration)((InputDecoration)__other)).filled == this.filled)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).fillColor, this.fillColor))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).focusColor, this.focusColor))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).hoverColor, this.hoverColor))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).errorBorder, this.errorBorder))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).focusedBorder, this.focusedBorder))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).focusedErrorBorder, this.focusedErrorBorder))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).disabledBorder, this.disabledBorder))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).enabledBorder, this.enabledBorder))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).border, this.border))) && (((InputDecoration)((InputDecoration)__other)).enabled == this.enabled)) && (((InputDecoration)((InputDecoration)__other)).semanticCounterText == this.semanticCounterText)) && (((InputDecoration)((InputDecoration)__other)).alignLabelWithHint == this.alignLabelWithHint)) && (object.Equals(((InputDecoration)((InputDecoration)__other)).constraints, this.constraints))) && (object.Equals(((InputDecoration)((InputDecoration)__other)).visualDensity, this.visualDensity)));
    }

    public override int GetHashCode()
    {
        var values__156263 = new List<object?> { this.icon, this.iconColor, this.label, this.labelText, this.floatingLabelStyle, this.labelStyle, this.helper, this.helperText, this.helperStyle, this.helperMaxLines, this.hintText, this.hint, this.hintStyle, this.hintTextDirection, this.hintMaxLines, this.hintFadeDuration, this.maintainHintHeight, this.maintainHintSize, this.maintainLabelSize, this.error, this.errorText, this.errorStyle, this.errorMaxLines, this.floatingLabelBehavior, this.floatingLabelAlignment, this.isDense, this.contentPadding, this.isCollapsed, this.filled, this.fillColor, this.focusColor, this.hoverColor, this.prefixIcon, this.prefixIconColor, this.prefix, this.prefixText, this.prefixStyle, this.prefixIconConstraints, this.suffixIcon, this.suffixIconColor, this.suffix, this.suffixText, this.suffixStyle, this.suffixIconConstraints, this.counter, this.counterText, this.counterStyle, this.errorBorder, this.focusedBorder, this.focusedErrorBorder, this.disabledBorder, this.enabledBorder, this.border, this.enabled, this.semanticCounterText, this.alignLabelWithHint, this.constraints, this.visualDensity };
        return FoundationRuntimePorts.ObjectHashAll(values__156263);
        return default!;
    }
    public override string ToString()
    {
        var description__157517 = ((Func<List<string>>)(() => { var __collection157531 = new List<string>(); if ((this.icon is not null)) { __collection157531.Add($"icon: {this.icon}"); } if ((this.iconColor is not null)) { __collection157531.Add($"iconColor: {this.iconColor}"); } if ((this.label is not null)) { __collection157531.Add($"label: {this.label}"); } if ((this.labelText is not null)) { __collection157531.Add($"labelText: \"{this.labelText}\""); } if ((this.floatingLabelStyle is not null)) { __collection157531.Add($"floatingLabelStyle: \"{this.floatingLabelStyle}\""); } if ((this.helper is not null)) { __collection157531.Add($"helper: \"{this.helper}\""); } if ((this.helperText is not null)) { __collection157531.Add($"helperText: \"{this.helperText}\""); } if ((this.helperMaxLines is not null)) { __collection157531.Add($"helperMaxLines: \"{this.helperMaxLines}\""); } if ((this.hintText is not null)) { __collection157531.Add($"hintText: \"{this.hintText}\""); } if ((this.hint is not null)) { __collection157531.Add($"hint: {this.hint}"); } if ((this.hintMaxLines is not null)) { __collection157531.Add($"hintMaxLines: \"{this.hintMaxLines}\""); } if ((this.hintFadeDuration is not null)) { __collection157531.Add($"hintFadeDuration: \"{this.hintFadeDuration}\""); } if (!this.maintainHintHeight) { __collection157531.Add("maintainHintHeight: false"); } if (!this.maintainHintSize) { __collection157531.Add("maintainHintSize: false"); } if (this.maintainLabelSize) { __collection157531.Add("maintainLabelSize: true"); } if ((this.error is not null)) { __collection157531.Add($"error: \"{this.error}\""); } if ((this.errorText is not null)) { __collection157531.Add($"errorText: \"{this.errorText}\""); } if ((this.errorStyle is not null)) { __collection157531.Add($"errorStyle: \"{this.errorStyle}\""); } if ((this.errorMaxLines is not null)) { __collection157531.Add($"errorMaxLines: \"{this.errorMaxLines}\""); } if ((this.floatingLabelBehavior is not null)) { __collection157531.Add($"floatingLabelBehavior: {this.floatingLabelBehavior}"); } if ((this.floatingLabelAlignment is not null)) { __collection157531.Add($"floatingLabelAlignment: {this.floatingLabelAlignment}"); } if ((this.isDense ?? false)) { __collection157531.Add($"isDense: {this.isDense}"); } if ((this.contentPadding is not null)) { __collection157531.Add($"contentPadding: {this.contentPadding}"); } if ((this.isCollapsed ?? false)) { __collection157531.Add($"isCollapsed: {this.isCollapsed}"); } if ((this.prefixIcon is not null)) { __collection157531.Add($"prefixIcon: {this.prefixIcon}"); } if ((this.prefixIconColor is not null)) { __collection157531.Add($"prefixIconColor: {this.prefixIconColor}"); } if ((this.prefix is not null)) { __collection157531.Add($"prefix: {this.prefix}"); } if ((this.prefixText is not null)) { __collection157531.Add($"prefixText: {this.prefixText}"); } if ((this.prefixStyle is not null)) { __collection157531.Add($"prefixStyle: {this.prefixStyle}"); } if ((this.prefixIconConstraints is not null)) { __collection157531.Add($"prefixIconConstraints: {this.prefixIconConstraints}"); } if ((this.suffixIcon is not null)) { __collection157531.Add($"suffixIcon: {this.suffixIcon}"); } if ((this.suffixIconColor is not null)) { __collection157531.Add($"suffixIconColor: {this.suffixIconColor}"); } if ((this.suffix is not null)) { __collection157531.Add($"suffix: {this.suffix}"); } if ((this.suffixText is not null)) { __collection157531.Add($"suffixText: {this.suffixText}"); } if ((this.suffixStyle is not null)) { __collection157531.Add($"suffixStyle: {this.suffixStyle}"); } if ((this.suffixIconConstraints is not null)) { __collection157531.Add($"suffixIconConstraints: {this.suffixIconConstraints}"); } if ((this.counter is not null)) { __collection157531.Add($"counter: {this.counter}"); } if ((this.counterText is not null)) { __collection157531.Add($"counterText: {this.counterText}"); } if ((this.counterStyle is not null)) { __collection157531.Add($"counterStyle: {this.counterStyle}"); } if ((this.filled ?? false)) { __collection157531.Add("filled: true"); } if ((this.fillColor is not null)) { __collection157531.Add($"fillColor: {this.fillColor}"); } if ((this.focusColor is not null)) { __collection157531.Add($"focusColor: {this.focusColor}"); } if ((this.hoverColor is not null)) { __collection157531.Add($"hoverColor: {this.hoverColor}"); } if ((this.errorBorder is not null)) { __collection157531.Add($"errorBorder: {this.errorBorder}"); } if ((this.focusedBorder is not null)) { __collection157531.Add($"focusedBorder: {this.focusedBorder}"); } if ((this.focusedErrorBorder is not null)) { __collection157531.Add($"focusedErrorBorder: {this.focusedErrorBorder}"); } if ((this.disabledBorder is not null)) { __collection157531.Add($"disabledBorder: {this.disabledBorder}"); } if ((this.enabledBorder is not null)) { __collection157531.Add($"enabledBorder: {this.enabledBorder}"); } if ((this.border is not null)) { __collection157531.Add($"border: {this.border}"); } if (!this.enabled) { __collection157531.Add("enabled: false"); } if ((this.semanticCounterText is not null)) { __collection157531.Add($"semanticCounterText: {this.semanticCounterText}"); } if ((this.alignLabelWithHint is not null)) { __collection157531.Add($"alignLabelWithHint: {this.alignLabelWithHint}"); } if ((this.constraints is not null)) { __collection157531.Add($"constraints: {this.constraints}"); } if ((this.visualDensity is not null)) { __collection157531.Add($"visualDensity: {this.visualDensity}"); } return __collection157531; }))();
        return $"InputDecoration({string.Join(", ", description__157517)})";
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

    public InputDecorationTheme(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle = null, global::Doroti.Framework.Painting.TextStyle? helperStyle = null, long? helperMaxLines = null, global::Doroti.Framework.Painting.TextStyle? hintStyle = null, Duration? hintFadeDuration = null, long? hintMaxLines = null, global::Doroti.Framework.Painting.TextStyle? errorStyle = null, long? errorMaxLines = null, FloatingLabelBehavior? floatingLabelBehavior = null, FloatingLabelAlignment? floatingLabelAlignment = null, bool? isDense = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, bool? isCollapsed = null, Color? iconColor = null, global::Doroti.Framework.Painting.TextStyle? prefixStyle = null, Color? prefixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? prefixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? suffixStyle = null, Color? suffixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? suffixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? counterStyle = null, bool? filled = null, Color? fillColor = null, global::Doroti.Framework.Painting.BorderSide? activeIndicatorBorder = null, global::Doroti.Framework.Painting.BorderSide? outlineBorder = null, Color? focusColor = null, Color? hoverColor = null, InputBorder? errorBorder = null, InputBorder? focusedBorder = null, InputBorder? focusedErrorBorder = null, InputBorder? disabledBorder = null, InputBorder? enabledBorder = null, InputBorder? border = null, bool? alignLabelWithHint = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, VisualDensity? visualDensity = null, InputDecorationThemeData? data = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key, child: (child ?? global::Doroti.Framework.Widgets.SizedBox.CreateShrink()))
    {
        this._labelStyle = labelStyle;
        this._floatingLabelStyle = floatingLabelStyle;
        this._helperStyle = helperStyle;
        this._helperMaxLines = helperMaxLines;
        this._hintStyle = hintStyle;
        this._hintFadeDuration = hintFadeDuration;
        this._hintMaxLines = hintMaxLines;
        this._errorStyle = errorStyle;
        this._errorMaxLines = errorMaxLines;
        this._floatingLabelBehavior = (floatingLabelBehavior ?? FloatingLabelBehavior.auto);
        this._floatingLabelAlignment = (floatingLabelAlignment ?? FloatingLabelAlignment.start);
        this._isDense = (isDense ?? false);
        this._contentPadding = contentPadding;
        this._isCollapsed = (isCollapsed ?? false);
        this._iconColor = iconColor;
        this._prefixStyle = prefixStyle;
        this._prefixIconColor = prefixIconColor;
        this._prefixIconConstraints = prefixIconConstraints;
        this._suffixStyle = suffixStyle;
        this._suffixIconColor = suffixIconColor;
        this._suffixIconConstraints = suffixIconConstraints;
        this._counterStyle = counterStyle;
        this._filled = (filled ?? false);
        this._fillColor = fillColor;
        this._activeIndicatorBorder = activeIndicatorBorder;
        this._outlineBorder = outlineBorder;
        this._focusColor = focusColor;
        this._hoverColor = hoverColor;
        this._errorBorder = errorBorder;
        this._focusedBorder = focusedBorder;
        this._focusedErrorBorder = focusedErrorBorder;
        this._disabledBorder = disabledBorder;
        this._enabledBorder = enabledBorder;
        this._border = border;
        this._alignLabelWithHint = (alignLabelWithHint ?? false);
        this._constraints = constraints;
        this._visualDensity = visualDensity;
        this._data = data;
        System.Diagnostics.Debug.Assert(((data is null) || (((((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)((labelStyle ?? floatingLabelStyle) ?? helperStyle) ?? (object?)helperMaxLines)) ?? (object?)hintStyle)) ?? (object?)hintFadeDuration)) ?? (object?)hintMaxLines)) ?? (object?)errorStyle)) ?? (object?)errorMaxLines)) ?? (object?)floatingLabelBehavior)) ?? (object?)floatingLabelAlignment)) ?? (object?)isDense)) ?? (object?)contentPadding)) ?? (object?)isCollapsed)) ?? (object?)iconColor)) ?? (object?)prefixStyle)) ?? (object?)prefixIconColor)) ?? (object?)prefixIconConstraints)) ?? (object?)suffixStyle)) ?? (object?)suffixIconColor)) ?? (object?)suffixIconConstraints)) ?? (object?)counterStyle)) ?? (object?)filled)) ?? (object?)fillColor)) ?? (object?)activeIndicatorBorder)) ?? (object?)outlineBorder)) ?? (object?)focusColor)) ?? (object?)hoverColor)) ?? (object?)errorBorder)) ?? (object?)focusedBorder)) ?? (object?)focusedErrorBorder)) ?? (object?)disabledBorder)) ?? (object?)enabledBorder)) ?? (object?)border)) ?? (object?)alignLabelWithHint)) ?? (object?)constraints)) ?? (object?)visualDensity))) is null)));
    }

    public virtual global::Doroti.Framework.Painting.TextStyle? labelStyle => ((this._data is not null) ? ((InputDecorationThemeData)this._data).labelStyle : this._labelStyle);
    public virtual global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle => ((this._data is not null) ? ((InputDecorationThemeData)this._data).floatingLabelStyle : this._floatingLabelStyle);
    public virtual global::Doroti.Framework.Painting.TextStyle? helperStyle => ((this._data is not null) ? ((InputDecorationThemeData)this._data).helperStyle : this._helperStyle);
    public virtual long? helperMaxLines => ((this._data is not null) ? ((InputDecorationThemeData)this._data).helperMaxLines : this._helperMaxLines);
    public virtual global::Doroti.Framework.Painting.TextStyle? hintStyle => ((this._data is not null) ? ((InputDecorationThemeData)this._data).hintStyle : this._hintStyle);
    public virtual Duration? hintFadeDuration => ((this._data is not null) ? ((InputDecorationThemeData)this._data).hintFadeDuration : this._hintFadeDuration);
    public virtual long? hintMaxLines => ((this._data is not null) ? ((InputDecorationThemeData)this._data).hintMaxLines : this._hintMaxLines);
    public virtual global::Doroti.Framework.Painting.TextStyle? errorStyle => ((this._data is not null) ? ((InputDecorationThemeData)this._data).errorStyle : this._errorStyle);
    public virtual long? errorMaxLines => ((this._data is not null) ? ((InputDecorationThemeData)this._data).errorMaxLines : this._errorMaxLines);
    public virtual FloatingLabelBehavior floatingLabelBehavior => ((this._data is not null) ? ((InputDecorationThemeData)this._data).floatingLabelBehavior : this._floatingLabelBehavior);
    public virtual FloatingLabelAlignment floatingLabelAlignment => ((this._data is not null) ? ((InputDecorationThemeData)this._data).floatingLabelAlignment : this._floatingLabelAlignment);
    public virtual bool isDense => ((this._data is not null) ? ((InputDecorationThemeData)this._data).isDense : this._isDense);
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding => ((this._data is not null) ? ((InputDecorationThemeData)this._data).contentPadding : this._contentPadding);
    public virtual bool isCollapsed => ((this._data is not null) ? ((InputDecorationThemeData)this._data).isCollapsed : this._isCollapsed);
    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((InputDecorationThemeData)this._data).iconColor : this._iconColor));
    public virtual global::Doroti.Framework.Painting.TextStyle? prefixStyle => ((this._data is not null) ? ((InputDecorationThemeData)this._data).prefixStyle : this._prefixStyle);
    public virtual global::Doroti.Ui.Color? prefixIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((InputDecorationThemeData)this._data).prefixIconColor : this._prefixIconColor));
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? prefixIconConstraints => ((this._data is not null) ? ((InputDecorationThemeData)this._data).prefixIconConstraints : this._prefixIconConstraints);
    public virtual global::Doroti.Framework.Painting.TextStyle? suffixStyle => ((this._data is not null) ? ((InputDecorationThemeData)this._data).suffixStyle : this._suffixStyle);
    public virtual global::Doroti.Ui.Color? suffixIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((InputDecorationThemeData)this._data).suffixIconColor : this._suffixIconColor));
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? suffixIconConstraints => ((this._data is not null) ? ((InputDecorationThemeData)this._data).suffixIconConstraints : this._suffixIconConstraints);
    public virtual global::Doroti.Framework.Painting.TextStyle? counterStyle => ((this._data is not null) ? ((InputDecorationThemeData)this._data).counterStyle : this._counterStyle);
    public virtual bool filled => ((this._data is not null) ? ((InputDecorationThemeData)this._data).filled : this._filled);
    public virtual global::Doroti.Ui.Color? fillColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((InputDecorationThemeData)this._data).fillColor : this._fillColor));
    public virtual global::Doroti.Framework.Painting.BorderSide? activeIndicatorBorder => ((this._data is not null) ? ((InputDecorationThemeData)this._data).activeIndicatorBorder : this._activeIndicatorBorder);
    public virtual global::Doroti.Framework.Painting.BorderSide? outlineBorder => ((this._data is not null) ? ((InputDecorationThemeData)this._data).outlineBorder : this._outlineBorder);
    public virtual global::Doroti.Ui.Color? focusColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((InputDecorationThemeData)this._data).focusColor : this._focusColor));
    public virtual global::Doroti.Ui.Color? hoverColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((InputDecorationThemeData)this._data).hoverColor : this._hoverColor));
    public virtual InputBorder? errorBorder => ((this._data is not null) ? ((InputDecorationThemeData)this._data).errorBorder : this._errorBorder);
    public virtual InputBorder? focusedBorder => ((this._data is not null) ? ((InputDecorationThemeData)this._data).focusedBorder : this._focusedBorder);
    public virtual InputBorder? focusedErrorBorder => ((this._data is not null) ? ((InputDecorationThemeData)this._data).focusedErrorBorder : this._focusedErrorBorder);
    public virtual InputBorder? disabledBorder => ((this._data is not null) ? ((InputDecorationThemeData)this._data).disabledBorder : this._disabledBorder);
    public virtual InputBorder? enabledBorder => ((this._data is not null) ? ((InputDecorationThemeData)this._data).enabledBorder : this._enabledBorder);
    public virtual InputBorder? border => ((this._data is not null) ? ((InputDecorationThemeData)this._data).border : this._border);
    public virtual bool alignLabelWithHint => ((this._data is not null) ? ((InputDecorationThemeData)this._data).alignLabelWithHint : this._alignLabelWithHint);
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints => ((this._data is not null) ? ((InputDecorationThemeData)this._data).constraints : this._constraints);
    public virtual VisualDensity? visualDensity => ((this._data is not null) ? ((InputDecorationThemeData)this._data).visualDensity : this._visualDensity);
    public virtual InputDecorationThemeData data => DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>((this._data ?? new InputDecorationThemeData(labelStyle: this._labelStyle, floatingLabelStyle: this._floatingLabelStyle, helperStyle: this._helperStyle, helperMaxLines: this._helperMaxLines, hintStyle: this._hintStyle, hintFadeDuration: this._hintFadeDuration, hintMaxLines: this._hintMaxLines, errorStyle: this._errorStyle, errorMaxLines: this._errorMaxLines, floatingLabelBehavior: this._floatingLabelBehavior, floatingLabelAlignment: this._floatingLabelAlignment, isDense: this._isDense, contentPadding: this._contentPadding, isCollapsed: this._isCollapsed, iconColor: this._iconColor, prefixStyle: this._prefixStyle, prefixIconColor: this._prefixIconColor, prefixIconConstraints: this._prefixIconConstraints, suffixStyle: this._suffixStyle, suffixIconColor: this._suffixIconColor, suffixIconConstraints: this._suffixIconConstraints, counterStyle: this._counterStyle, filled: this._filled, fillColor: this._fillColor, activeIndicatorBorder: this._activeIndicatorBorder, outlineBorder: this._outlineBorder, focusColor: this._focusColor, hoverColor: this._hoverColor, errorBorder: this._errorBorder, focusedBorder: this._focusedBorder, focusedErrorBorder: this._focusedErrorBorder, disabledBorder: this._disabledBorder, enabledBorder: this._enabledBorder, border: this._border, alignLabelWithHint: this._alignLabelWithHint, constraints: this._constraints, visualDensity: this._visualDensity)));
    public static InputDecorationThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        InputDecorationTheme? inputDecorationTheme__181761 = ((InputDecorationTheme?)(object?)context.dependOnInheritedWidgetOfExactType<InputDecorationTheme>());
        return (inputDecorationTheme__181761?.data ?? Theme.of(context).inputDecorationTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InputDecorationTheme copyWith(global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle = null, global::Doroti.Framework.Painting.TextStyle? helperStyle = null, long? helperMaxLines = null, global::Doroti.Framework.Painting.TextStyle? hintStyle = null, Duration? hintFadeDuration = null, long? hintMaxLines = null, global::Doroti.Framework.Painting.TextStyle? errorStyle = null, long? errorMaxLines = null, FloatingLabelBehavior? floatingLabelBehavior = null, FloatingLabelAlignment? floatingLabelAlignment = null, bool? isDense = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, bool? isCollapsed = null, Color? iconColor = null, global::Doroti.Framework.Painting.TextStyle? prefixStyle = null, Color? prefixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? prefixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? suffixStyle = null, Color? suffixIconColor = null, global::Doroti.Framework.Rendering.BoxConstraints? suffixIconConstraints = null, global::Doroti.Framework.Painting.TextStyle? counterStyle = null, bool? filled = null, Color? fillColor = null, global::Doroti.Framework.Painting.BorderSide? activeIndicatorBorder = null, global::Doroti.Framework.Painting.BorderSide? outlineBorder = null, Color? focusColor = null, Color? hoverColor = null, InputBorder? errorBorder = null, InputBorder? focusedBorder = null, InputBorder? focusedErrorBorder = null, InputBorder? disabledBorder = null, InputBorder? enabledBorder = null, InputBorder? border = null, bool? alignLabelWithHint = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, VisualDensity? visualDensity = null)
    {
        return new InputDecorationTheme(labelStyle: (labelStyle ?? this.labelStyle), floatingLabelStyle: (floatingLabelStyle ?? this.floatingLabelStyle), helperStyle: (helperStyle ?? this.helperStyle), helperMaxLines: (helperMaxLines ?? this.helperMaxLines), hintStyle: (hintStyle ?? this.hintStyle), hintFadeDuration: (hintFadeDuration ?? this.hintFadeDuration), hintMaxLines: (hintMaxLines ?? this.hintMaxLines), errorStyle: (errorStyle ?? this.errorStyle), errorMaxLines: (errorMaxLines ?? this.errorMaxLines), floatingLabelBehavior: (floatingLabelBehavior ?? this.floatingLabelBehavior), floatingLabelAlignment: (floatingLabelAlignment ?? this.floatingLabelAlignment), isDense: (isDense ?? this.isDense), contentPadding: (contentPadding ?? this.contentPadding), iconColor: (iconColor ?? this.iconColor), isCollapsed: (isCollapsed ?? this.isCollapsed), prefixStyle: (prefixStyle ?? this.prefixStyle), prefixIconColor: (prefixIconColor ?? this.prefixIconColor), prefixIconConstraints: (prefixIconConstraints ?? this.prefixIconConstraints), suffixStyle: (suffixStyle ?? this.suffixStyle), suffixIconColor: (suffixIconColor ?? this.suffixIconColor), suffixIconConstraints: (suffixIconConstraints ?? this.suffixIconConstraints), counterStyle: (counterStyle ?? this.counterStyle), filled: (filled ?? this.filled), fillColor: (fillColor ?? this.fillColor), activeIndicatorBorder: (activeIndicatorBorder ?? this.activeIndicatorBorder), outlineBorder: (outlineBorder ?? this.outlineBorder), focusColor: (focusColor ?? this.focusColor), hoverColor: (hoverColor ?? this.hoverColor), errorBorder: (errorBorder ?? this.errorBorder), focusedBorder: (focusedBorder ?? this.focusedBorder), focusedErrorBorder: (focusedErrorBorder ?? this.focusedErrorBorder), disabledBorder: (disabledBorder ?? this.disabledBorder), enabledBorder: (enabledBorder ?? this.enabledBorder), border: (border ?? this.border), alignLabelWithHint: (alignLabelWithHint ?? this.alignLabelWithHint), constraints: (constraints ?? this.constraints), visualDensity: (visualDensity ?? this.visualDensity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InputDecorationTheme merge(InputDecorationTheme? other)
    {
        if ((other is null))
        {
            return this;
        }
        return ((InputDecorationTheme)(object?)copyWith(labelStyle: ((this.labelStyle ?? (global::Doroti.Framework.Painting.TextStyle)((InputDecorationTheme)other).labelStyle)), floatingLabelStyle: ((this.floatingLabelStyle ?? (global::Doroti.Framework.Painting.TextStyle)((InputDecorationTheme)other).floatingLabelStyle)), helperStyle: ((this.helperStyle ?? (global::Doroti.Framework.Painting.TextStyle)((InputDecorationTheme)other).helperStyle)), helperMaxLines: ((this.helperMaxLines ?? (long)((InputDecorationTheme)other).helperMaxLines)), hintStyle: ((this.hintStyle ?? (global::Doroti.Framework.Painting.TextStyle)((InputDecorationTheme)other).hintStyle)), hintFadeDuration: ((this.hintFadeDuration ?? (Duration)((InputDecorationTheme)other).hintFadeDuration)), hintMaxLines: ((this.hintMaxLines ?? (long)((InputDecorationTheme)other).hintMaxLines)), errorStyle: ((this.errorStyle ?? (global::Doroti.Framework.Painting.TextStyle)((InputDecorationTheme)other).errorStyle)), errorMaxLines: ((this.errorMaxLines ?? (long)((InputDecorationTheme)other).errorMaxLines)), contentPadding: ((this.contentPadding ?? (global::Doroti.Framework.Painting.EdgeInsetsGeometry)((InputDecorationTheme)other).contentPadding)), iconColor: ((this.iconColor ?? (Color)((InputDecorationTheme)other).iconColor)), prefixStyle: ((this.prefixStyle ?? (global::Doroti.Framework.Painting.TextStyle)((InputDecorationTheme)other).prefixStyle)), prefixIconColor: ((this.prefixIconColor ?? (Color)((InputDecorationTheme)other).prefixIconColor)), prefixIconConstraints: ((this.prefixIconConstraints ?? (global::Doroti.Framework.Rendering.BoxConstraints)((InputDecorationTheme)other).prefixIconConstraints)), suffixStyle: ((this.suffixStyle ?? (global::Doroti.Framework.Painting.TextStyle)((InputDecorationTheme)other).suffixStyle)), suffixIconColor: ((this.suffixIconColor ?? (Color)((InputDecorationTheme)other).suffixIconColor)), suffixIconConstraints: ((this.suffixIconConstraints ?? (global::Doroti.Framework.Rendering.BoxConstraints)((InputDecorationTheme)other).suffixIconConstraints)), counterStyle: ((this.counterStyle ?? (global::Doroti.Framework.Painting.TextStyle)((InputDecorationTheme)other).counterStyle)), fillColor: ((this.fillColor ?? (Color)((InputDecorationTheme)other).fillColor)), activeIndicatorBorder: ((this.activeIndicatorBorder ?? (global::Doroti.Framework.Painting.BorderSide)((InputDecorationTheme)other).activeIndicatorBorder)), outlineBorder: ((this.outlineBorder ?? (global::Doroti.Framework.Painting.BorderSide)((InputDecorationTheme)other).outlineBorder)), focusColor: ((this.focusColor ?? (Color)((InputDecorationTheme)other).focusColor)), hoverColor: ((this.hoverColor ?? (Color)((InputDecorationTheme)other).hoverColor)), errorBorder: ((this.errorBorder ?? (InputBorder)((InputDecorationTheme)other).errorBorder)), focusedBorder: ((this.focusedBorder ?? (InputBorder)((InputDecorationTheme)other).focusedBorder)), focusedErrorBorder: ((this.focusedErrorBorder ?? (InputBorder)((InputDecorationTheme)other).focusedErrorBorder)), disabledBorder: ((this.disabledBorder ?? (InputBorder)((InputDecorationTheme)other).disabledBorder)), enabledBorder: ((this.enabledBorder ?? (InputBorder)((InputDecorationTheme)other).enabledBorder)), border: ((this.border ?? (InputBorder)((InputDecorationTheme)other).border)), constraints: ((this.constraints ?? (global::Doroti.Framework.Rendering.BoxConstraints)((InputDecorationTheme)other).constraints)), visualDensity: ((this.visualDensity ?? (VisualDensity)((InputDecorationTheme)other).visualDensity))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((InputDecorationTheme)oldWidget).data)));
    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new InputDecorationTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
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
        return new InputDecorationThemeData(labelStyle: (labelStyle ?? this.labelStyle), floatingLabelStyle: (floatingLabelStyle ?? this.floatingLabelStyle), helperStyle: (helperStyle ?? this.helperStyle), helperMaxLines: (helperMaxLines ?? this.helperMaxLines), hintStyle: (hintStyle ?? this.hintStyle), hintFadeDuration: (hintFadeDuration ?? this.hintFadeDuration), hintMaxLines: (hintMaxLines ?? this.hintMaxLines), errorStyle: (errorStyle ?? this.errorStyle), errorMaxLines: (errorMaxLines ?? this.errorMaxLines), floatingLabelBehavior: (floatingLabelBehavior ?? this.floatingLabelBehavior), floatingLabelAlignment: (floatingLabelAlignment ?? this.floatingLabelAlignment), isDense: (isDense ?? this.isDense), contentPadding: (contentPadding ?? this.contentPadding), iconColor: (iconColor ?? this.iconColor), isCollapsed: (isCollapsed ?? this.isCollapsed), prefixStyle: (prefixStyle ?? this.prefixStyle), prefixIconColor: (prefixIconColor ?? this.prefixIconColor), prefixIconConstraints: (prefixIconConstraints ?? this.prefixIconConstraints), suffixStyle: (suffixStyle ?? this.suffixStyle), suffixIconColor: (suffixIconColor ?? this.suffixIconColor), suffixIconConstraints: (suffixIconConstraints ?? this.suffixIconConstraints), counterStyle: (counterStyle ?? this.counterStyle), filled: (filled ?? this.filled), fillColor: (fillColor ?? this.fillColor), activeIndicatorBorder: (activeIndicatorBorder ?? this.activeIndicatorBorder), outlineBorder: (outlineBorder ?? this.outlineBorder), focusColor: (focusColor ?? this.focusColor), hoverColor: (hoverColor ?? this.hoverColor), errorBorder: (errorBorder ?? this.errorBorder), focusedBorder: (focusedBorder ?? this.focusedBorder), focusedErrorBorder: (focusedErrorBorder ?? this.focusedErrorBorder), disabledBorder: (disabledBorder ?? this.disabledBorder), enabledBorder: (enabledBorder ?? this.enabledBorder), border: (border ?? this.border), alignLabelWithHint: (alignLabelWithHint ?? this.alignLabelWithHint), constraints: (constraints ?? this.constraints), visualDensity: (visualDensity ?? this.visualDensity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InputDecorationThemeData merge(InputDecorationThemeData? other)
    {
        if ((other is null))
        {
            return this;
        }
        return ((InputDecorationThemeData)(object?)copyWith(labelStyle: (this.labelStyle ?? ((InputDecorationThemeData)other).labelStyle), floatingLabelStyle: (this.floatingLabelStyle ?? ((InputDecorationThemeData)other).floatingLabelStyle), helperStyle: (this.helperStyle ?? ((InputDecorationThemeData)other).helperStyle), helperMaxLines: (this.helperMaxLines ?? ((InputDecorationThemeData)other).helperMaxLines), hintStyle: (this.hintStyle ?? ((InputDecorationThemeData)other).hintStyle), hintFadeDuration: (this.hintFadeDuration ?? ((InputDecorationThemeData)other).hintFadeDuration), hintMaxLines: (this.hintMaxLines ?? ((InputDecorationThemeData)other).hintMaxLines), errorStyle: (this.errorStyle ?? ((InputDecorationThemeData)other).errorStyle), errorMaxLines: (this.errorMaxLines ?? ((InputDecorationThemeData)other).errorMaxLines), contentPadding: (this.contentPadding ?? ((InputDecorationThemeData)other).contentPadding), iconColor: (this.iconColor ?? ((InputDecorationThemeData)other).iconColor), prefixStyle: (this.prefixStyle ?? ((InputDecorationThemeData)other).prefixStyle), prefixIconColor: (this.prefixIconColor ?? ((InputDecorationThemeData)other).prefixIconColor), prefixIconConstraints: (this.prefixIconConstraints ?? ((InputDecorationThemeData)other).prefixIconConstraints), suffixStyle: (this.suffixStyle ?? ((InputDecorationThemeData)other).suffixStyle), suffixIconColor: (this.suffixIconColor ?? ((InputDecorationThemeData)other).suffixIconColor), suffixIconConstraints: (this.suffixIconConstraints ?? ((InputDecorationThemeData)other).suffixIconConstraints), counterStyle: (this.counterStyle ?? ((InputDecorationThemeData)other).counterStyle), fillColor: (this.fillColor ?? ((InputDecorationThemeData)other).fillColor), activeIndicatorBorder: (this.activeIndicatorBorder ?? ((InputDecorationThemeData)other).activeIndicatorBorder), outlineBorder: (this.outlineBorder ?? ((InputDecorationThemeData)other).outlineBorder), focusColor: (this.focusColor ?? ((InputDecorationThemeData)other).focusColor), hoverColor: (this.hoverColor ?? ((InputDecorationThemeData)other).hoverColor), errorBorder: (this.errorBorder ?? ((InputDecorationThemeData)other).errorBorder), focusedBorder: (this.focusedBorder ?? ((InputDecorationThemeData)other).focusedBorder), focusedErrorBorder: (this.focusedErrorBorder ?? ((InputDecorationThemeData)other).focusedErrorBorder), disabledBorder: (this.disabledBorder ?? ((InputDecorationThemeData)other).disabledBorder), enabledBorder: (this.enabledBorder ?? ((InputDecorationThemeData)other).enabledBorder), border: (this.border ?? ((InputDecorationThemeData)other).border), constraints: (this.constraints ?? ((InputDecorationThemeData)other).constraints), visualDensity: (this.visualDensity ?? ((InputDecorationThemeData)other).visualDensity)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.labelStyle, this.floatingLabelStyle, this.helperStyle, this.helperMaxLines, this.hintStyle, this.hintMaxLines, this.errorStyle, this.errorMaxLines, this.floatingLabelBehavior, this.floatingLabelAlignment, this.isDense, this.contentPadding, this.isCollapsed, this.iconColor, this.prefixStyle, this.prefixIconColor, this.prefixIconConstraints, this.suffixStyle, this.suffixIconColor, FoundationRuntimePorts.ObjectHash(this.suffixIconConstraints, this.counterStyle, this.filled, this.fillColor, this.activeIndicatorBorder, this.outlineBorder, this.focusColor, this.hoverColor, this.errorBorder, this.focusedBorder, this.focusedErrorBorder, this.disabledBorder, this.enabledBorder, this.border, this.alignLabelWithHint, this.constraints, this.hintFadeDuration, this.visualDensity)));
    public override bool Equals(object? other)
    {
        var __other = other as InputDecorationThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((((((((((((((((((((((((((((__other is InputDecorationThemeData) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).labelStyle, this.labelStyle))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).floatingLabelStyle, this.floatingLabelStyle))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).helperStyle, this.helperStyle))) && (((InputDecorationThemeData)((InputDecorationThemeData)__other)).helperMaxLines == this.helperMaxLines)) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).hintStyle, this.hintStyle))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).hintFadeDuration, this.hintFadeDuration))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).errorStyle, this.errorStyle))) && (((InputDecorationThemeData)((InputDecorationThemeData)__other)).errorMaxLines == this.errorMaxLines)) && (((InputDecorationThemeData)((InputDecorationThemeData)__other)).isDense == this.isDense)) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).contentPadding, this.contentPadding))) && (((InputDecorationThemeData)((InputDecorationThemeData)__other)).isCollapsed == this.isCollapsed)) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).iconColor, this.iconColor))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).prefixStyle, this.prefixStyle))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).prefixIconColor, this.prefixIconColor))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).prefixIconConstraints, this.prefixIconConstraints))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).suffixStyle, this.suffixStyle))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).suffixIconColor, this.suffixIconColor))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).suffixIconConstraints, this.suffixIconConstraints))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).counterStyle, this.counterStyle))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).floatingLabelBehavior, this.floatingLabelBehavior))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).floatingLabelAlignment, this.floatingLabelAlignment))) && (((InputDecorationThemeData)((InputDecorationThemeData)__other)).filled == this.filled)) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).fillColor, this.fillColor))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).activeIndicatorBorder, this.activeIndicatorBorder))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).outlineBorder, this.outlineBorder))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).focusColor, this.focusColor))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).hoverColor, this.hoverColor))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).errorBorder, this.errorBorder))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).focusedBorder, this.focusedBorder))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).focusedErrorBorder, this.focusedErrorBorder))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).disabledBorder, this.disabledBorder))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).enabledBorder, this.enabledBorder))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).border, this.border))) && (((InputDecorationThemeData)((InputDecorationThemeData)__other)).hintMaxLines == this.hintMaxLines)) && (((InputDecorationThemeData)((InputDecorationThemeData)__other)).alignLabelWithHint == this.alignLabelWithHint)) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).constraints, this.constraints))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).disabledBorder, this.disabledBorder))) && (object.Equals(((InputDecorationThemeData)((InputDecorationThemeData)__other)).visualDensity, this.visualDensity)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultTheme__218670 = new InputDecorationThemeData();
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("labelStyle", this.labelStyle, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).labelStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("floatingLabelStyle", this.floatingLabelStyle, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).floatingLabelStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("helperStyle", this.helperStyle, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).helperStyle));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("helperMaxLines", this.helperMaxLines, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).helperMaxLines));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("hintStyle", this.hintStyle, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).hintStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("hintFadeDuration", this.hintFadeDuration, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).hintFadeDuration));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("hintMaxLines", this.hintMaxLines, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).hintMaxLines));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("errorStyle", this.errorStyle, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).errorStyle));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("errorMaxLines", this.errorMaxLines, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).errorMaxLines));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FloatingLabelBehavior>("floatingLabelBehavior", this.floatingLabelBehavior, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).floatingLabelBehavior));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FloatingLabelAlignment>("floatingLabelAlignment", this.floatingLabelAlignment, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).floatingLabelAlignment));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("isDense", this.isDense, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).isDense));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("contentPadding", this.contentPadding, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).contentPadding));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("isCollapsed", this.isCollapsed, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).isCollapsed));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color>("iconColor", this.iconColor, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).iconColor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color>("prefixIconColor", this.prefixIconColor, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).prefixIconColor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("prefixIconConstraints", this.prefixIconConstraints, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).prefixIconConstraints));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("prefixStyle", this.prefixStyle, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).prefixStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color>("suffixIconColor", this.suffixIconColor, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).suffixIconColor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("suffixIconConstraints", this.suffixIconConstraints, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).suffixIconConstraints));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("suffixStyle", this.suffixStyle, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).suffixStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("counterStyle", this.counterStyle, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).counterStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("filled", this.filled, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).filled));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("fillColor", this.fillColor, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).fillColor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderSide>("activeIndicatorBorder", this.activeIndicatorBorder, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).activeIndicatorBorder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderSide>("outlineBorder", this.outlineBorder, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).outlineBorder));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", this.focusColor, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).focusColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", this.hoverColor, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).hoverColor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputBorder>("errorBorder", this.errorBorder, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).errorBorder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputBorder>("focusedBorder", this.focusedBorder, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).focusedErrorBorder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputBorder>("focusedErrorBorder", this.focusedErrorBorder, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).focusedErrorBorder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputBorder>("disabledBorder", this.disabledBorder, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).disabledBorder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputBorder>("enabledBorder", this.enabledBorder, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).enabledBorder));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputBorder>("border", this.border, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).border));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("alignLabelWithHint", this.alignLabelWithHint, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).alignLabelWithHint));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("constraints", this.constraints, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).constraints));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", this.visualDensity, defaultValue: ((InputDecorationThemeData)defaultTheme__218670).visualDensity));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _InputDecoratorDefaultsM2__input_decorator : InputDecorationThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _InputDecoratorDefaultsM2__input_decorator(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public override global::Doroti.Framework.Painting.TextStyle? hintStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(global::Doroti.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return new global::Doroti.Framework.Painting.TextStyle(color: Theme.of(this.context).disabledColor);
        }
        return new global::Doroti.Framework.Painting.TextStyle(color: Theme.of(this.context).hintColor);
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override global::Doroti.Framework.Painting.TextStyle? labelStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(global::Doroti.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return new global::Doroti.Framework.Painting.TextStyle(color: Theme.of(this.context).disabledColor);
        }
        return new global::Doroti.Framework.Painting.TextStyle(color: Theme.of(this.context).hintColor);
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(global::Doroti.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return new global::Doroti.Framework.Painting.TextStyle(color: Theme.of(this.context).disabledColor);
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.error))
        {
            return new global::Doroti.Framework.Painting.TextStyle(color: Theme.of(this.context).colorScheme.error);
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return new global::Doroti.Framework.Painting.TextStyle(color: Theme.of(this.context).colorScheme.primary);
        }
        return new global::Doroti.Framework.Painting.TextStyle(color: Theme.of(this.context).hintColor);
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override global::Doroti.Framework.Painting.TextStyle? helperStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(global::Doroti.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) =>
    {
        ThemeData themeData__225989 = Theme.of(this.context);
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)themeData__225989.textTheme.bodySmall!.copyWith(color: Colors.transparent));
        }
        return ((global::Doroti.Framework.Painting.TextStyle)(object?)themeData__225989.textTheme.bodySmall!.copyWith(color: themeData__225989.hintColor));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override global::Doroti.Framework.Painting.TextStyle? errorStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(global::Doroti.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) =>
    {
        ThemeData themeData__226368 = Theme.of(this.context);
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)themeData__226368.textTheme.bodySmall!.copyWith(color: Colors.transparent));
        }
        return ((global::Doroti.Framework.Painting.TextStyle)(object?)themeData__226368.textTheme.bodySmall!.copyWith(color: themeData__226368.colorScheme.error));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override Color? fillColor => DartRuntimePrimitives.ConvertValue<Color>(global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
    {
        return ((Theme.brightnessOf(this.context), states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled)) switch { (Brightness.dark, true) => new global::Doroti.Ui.Color(234881023L), (Brightness.dark, false) => new global::Doroti.Ui.Color(452984831L), (Brightness.light, true) => new global::Doroti.Ui.Color(83886080L), (Brightness.light, false) => new global::Doroti.Ui.Color(167772160L) });
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override Color? iconColor => DartRuntimePrimitives.ConvertValue<Color>(global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
    {
        if ((states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled) && !states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused)))
        {
            return Theme.of(this.context).disabledColor;
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return Theme.of(this.context).colorScheme.primary;
        }
        return (Theme.brightnessOf(this.context) switch { Brightness.dark => Colors.white70, Brightness.light => Colors.black45, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override Color? prefixIconColor => DartRuntimePrimitives.ConvertValue<Color>(global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
    {
        if ((states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled) && !states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused)))
        {
            return Theme.of(this.context).disabledColor;
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return Theme.of(this.context).colorScheme.primary;
        }
        return (Theme.brightnessOf(this.context) switch { Brightness.dark => Colors.white70, Brightness.light => Colors.black45, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override Color? suffixIconColor => DartRuntimePrimitives.ConvertValue<Color>(global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
    {
        if ((states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled) && !states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused)))
        {
            return Theme.of(this.context).disabledColor;
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.error))
        {
            return Theme.of(this.context).colorScheme.error;
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return Theme.of(this.context).colorScheme.primary;
        }
        return (Theme.brightnessOf(this.context) switch { Brightness.dark => Colors.white70, Brightness.light => Colors.black45, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
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
                __late__colors = Theme.of(this.context).colorScheme;
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
                __late__textTheme = Theme.of(this.context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _InputDecoratorDefaultsM3__input_decorator(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public override global::Doroti.Framework.Painting.TextStyle? hintStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(global::Doroti.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return new global::Doroti.Framework.Painting.TextStyle(color: this._colors.onSurface.withOpacity(0.38));
        }
        return new global::Doroti.Framework.Painting.TextStyle(color: this._colors.onSurfaceVariant);
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override Color? fillColor => DartRuntimePrimitives.ConvertValue<Color>(global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return this._colors.onSurface.withOpacity(0.04);
        }
        return this._colors.surfaceContainerHighest;
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override global::Doroti.Framework.Painting.BorderSide? activeIndicatorBorder => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.BorderSide>(global::Doroti.Framework.Widgets.WidgetStateBorderSide.CreateResolveWith(((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.onSurface.withOpacity(0.38));
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.error))
        {
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.error, width: 2.0);
            }
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.onErrorContainer);
            }
            return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.error);
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.primary, width: 2.0);
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.onSurface);
        }
        return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.onSurfaceVariant);
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override global::Doroti.Framework.Painting.BorderSide? outlineBorder => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.BorderSide>(global::Doroti.Framework.Widgets.WidgetStateBorderSide.CreateResolveWith(((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.onSurface.withOpacity(0.12));
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.error))
        {
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.error, width: 2.0);
            }
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.onErrorContainer);
            }
            return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.error);
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.primary, width: 2.0);
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.onSurface);
        }
        return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.outline);
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override Color? iconColor => this._colors.onSurfaceVariant;
    public override Color? prefixIconColor => DartRuntimePrimitives.ConvertValue<Color>(global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return this._colors.onSurface.withOpacity(0.38);
        }
        return this._colors.onSurfaceVariant;
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override Color? suffixIconColor => DartRuntimePrimitives.ConvertValue<Color>(global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return this._colors.onSurface.withOpacity(0.38);
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.error))
        {
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
            {
                return this._colors.onErrorContainer;
            }
            return this._colors.error;
        }
        return this._colors.onSurfaceVariant;
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override global::Doroti.Framework.Painting.TextStyle? labelStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(global::Doroti.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) =>
    {
        global::Doroti.Framework.Painting.TextStyle textStyle__233081 = (this._textTheme.bodyLarge ?? new global::Doroti.Framework.Painting.TextStyle());
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__233081.copyWith(color: this._colors.onSurface.withOpacity(0.38)));
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.error))
        {
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
            {
                return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__233081.copyWith(color: this._colors.error));
            }
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
            {
                return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__233081.copyWith(color: this._colors.onErrorContainer));
            }
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__233081.copyWith(color: this._colors.error));
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__233081.copyWith(color: this._colors.primary));
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__233081.copyWith(color: this._colors.onSurfaceVariant));
        }
        return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__233081.copyWith(color: this._colors.onSurfaceVariant));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override global::Doroti.Framework.Painting.TextStyle? floatingLabelStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(global::Doroti.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) =>
    {
        global::Doroti.Framework.Painting.TextStyle textStyle__234050 = (this._textTheme.bodyLarge ?? new global::Doroti.Framework.Painting.TextStyle());
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__234050.copyWith(color: this._colors.onSurface.withOpacity(0.38)));
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.error))
        {
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
            {
                return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__234050.copyWith(color: this._colors.error));
            }
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
            {
                return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__234050.copyWith(color: this._colors.onErrorContainer));
            }
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__234050.copyWith(color: this._colors.error));
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__234050.copyWith(color: this._colors.primary));
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__234050.copyWith(color: this._colors.onSurfaceVariant));
        }
        return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__234050.copyWith(color: this._colors.onSurfaceVariant));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override global::Doroti.Framework.Painting.TextStyle? helperStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(global::Doroti.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) =>
    {
        global::Doroti.Framework.Painting.TextStyle textStyle__235012 = (this._textTheme.bodySmall ?? new global::Doroti.Framework.Painting.TextStyle());
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__235012.copyWith(color: this._colors.onSurface.withOpacity(0.38)));
        }
        return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__235012.copyWith(color: this._colors.onSurfaceVariant));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override global::Doroti.Framework.Painting.TextStyle? errorStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(global::Doroti.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) =>
    {
        global::Doroti.Framework.Painting.TextStyle textStyle__235394 = (this._textTheme.bodySmall ?? new global::Doroti.Framework.Painting.TextStyle());
        return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle__235394.copyWith(color: this._colors.error));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
}
