// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/slider.dart
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

internal delegate void _SliderValueChanged__slider(double value, bool isFastDrag);

public static partial class SliderLibrary
{
    internal static double _kVelocityThreshold = 1.0;
}

public class CupertinoSlider : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual double value { get; private set; } = default!;
    public virtual global::System.Action<double>? onChanged { get; private set; }
    public virtual global::System.Action<double>? onChangeStart { get; private set; }
    public virtual global::System.Action<double>? onChangeEnd { get; private set; }
    public virtual double min { get; private set; } = default!;
    public virtual double max { get; private set; } = default!;
    public virtual long? divisions { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual Color thumbColor { get; private set; } = default!;

    public CupertinoSlider(global::Doroti.Framework.Foundation.Key? key = null, double value = default!, global::System.Action<double>? onChanged = default!, global::System.Action<double>? onChangeStart = null, global::System.Action<double>? onChangeEnd = null, double min = 0.0, double max = 1.0, long? divisions = null, Color? activeColor = null, Color thumbColor = default!) : base(key: key)
    {
        Color __thumbColor = thumbColor ?? CupertinoColors.white;
        this.value = value;
        this.onChanged = onChanged;
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        this.min = min;
        this.max = max;
        this.divisions = divisions;
        this.activeColor = activeColor;
        this.thumbColor = __thumbColor;
        System.Diagnostics.Debug.Assert(((value >= min) && (value <= max)));
        System.Diagnostics.Debug.Assert(((divisions is null) || (DartRuntimePrimitives.RequireValue(divisions) > 0L)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoSliderState__slider());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("value", this.value));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("min", this.min));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("max", this.max));
    }

}

internal class _CupertinoSliderState__slider : global::Doroti.Framework.Widgets.State<CupertinoSlider>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<CupertinoSlider>
{
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _handleChanged(double value, bool isFastDrag)
    {
        DartRuntimePrimitives.Assert(() => (((CupertinoSlider)this.widget).onChanged is not null));
        double lerpValue__7848 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((CupertinoSlider)this.widget).min, ((CupertinoSlider)this.widget).max, value));
        bool isAtEdge__7919 = ((lerpValue__7848 == ((CupertinoSlider)this.widget).max) || (lerpValue__7848 == ((CupertinoSlider)this.widget).min));
        if ((lerpValue__7848 != ((CupertinoSlider)this.widget).value))
        {
            if (isAtEdge__7919)
            {
                _emitHapticFeedback(isFastDrag);
            }
            ((CupertinoSlider)this.widget).onChanged!(lerpValue__7848);
        }
    }

    internal virtual void _handleDragStart(double value)
    {
        DartRuntimePrimitives.Assert(() => (((CupertinoSlider)this.widget).onChangeStart is not null));
        ((CupertinoSlider)this.widget).onChangeStart!(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((CupertinoSlider)this.widget).min, ((CupertinoSlider)this.widget).max, value)));
    }

    internal virtual void _handleDragEnd(double value)
    {
        DartRuntimePrimitives.Assert(() => (((CupertinoSlider)this.widget).onChangeEnd is not null));
        ((CupertinoSlider)this.widget).onChangeEnd!(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((CupertinoSlider)this.widget).min, ((CupertinoSlider)this.widget).max, value)));
    }

    internal virtual void _emitHapticFeedback(bool isFastDrag)
    {
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    if (isFastDrag)
                    {
                        DartRuntimePrimitives.Ignore(HapticFeedback.mediumImpact());
                    }
                    else
                    {
                        DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
                    }
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    break;
                }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _CupertinoSliderRenderObjectWidget__slider(value: (((((CupertinoSlider)this.widget).value - ((CupertinoSlider)this.widget).min)) / ((((CupertinoSlider)this.widget).max - ((CupertinoSlider)this.widget).min))), divisions: ((CupertinoSlider)this.widget).divisions, activeColor: CupertinoDynamicColor.resolve((((CupertinoSlider)this.widget).activeColor ?? CupertinoTheme.of(context).primaryColor), context), thumbColor: ((CupertinoSlider)this.widget).thumbColor, onChanged: ((global::System.Action<double, bool>)((((CupertinoSlider)this.widget).onChanged is not null) ? this._handleChanged : null)), onChangeStart: ((global::System.Action<double>)((((CupertinoSlider)this.widget).onChangeStart is not null) ? this._handleDragStart : null)), onChangeEnd: ((global::System.Action<double>)((((CupertinoSlider)this.widget).onChangeEnd is not null) ? this._handleDragEnd : null)), vsync: this));
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

    public override void dispose()
    {
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
        this._tickerModeNotifier = null;
        base.dispose();
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _CupertinoSliderRenderObjectWidget__slider : global::Doroti.Framework.Widgets.LeafRenderObjectWidget
{
    public virtual double value { get; private set; } = default!;
    public virtual long? divisions { get; private set; }
    public virtual Color activeColor { get; private set; } = default!;
    public virtual Color thumbColor { get; private set; } = default!;
    public virtual global::System.Action<double, bool>? onChanged { get; private set; }
    public virtual global::System.Action<double>? onChangeStart { get; private set; }
    public virtual global::System.Action<double>? onChangeEnd { get; private set; }
    public virtual global::Doroti.Framework.Scheduler.TickerProvider vsync { get; private set; } = default!;

    internal _CupertinoSliderRenderObjectWidget__slider(double value, long? divisions = null, Color activeColor = default!, Color thumbColor = default!, global::System.Action<double, bool>? onChanged = null, global::System.Action<double>? onChangeStart = null, global::System.Action<double>? onChangeEnd = null, global::Doroti.Framework.Scheduler.TickerProvider vsync = default!)
    {
        this.value = value;
        this.divisions = divisions;
        this.activeColor = activeColor;
        this.thumbColor = thumbColor;
        this.onChanged = onChanged;
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        this.vsync = vsync;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderCupertinoSlider__slider(value: this.value, divisions: this.divisions, activeColor: this.activeColor, thumbColor: CupertinoDynamicColor.resolve(this.thumbColor, context), trackColor: CupertinoDynamicColor.resolve(CupertinoColors.systemFill, context), onChanged: (global::System.Action<double, bool>?)this.onChanged, onChangeStart: (global::System.Action<double>?)this.onChangeStart, onChangeEnd: (global::System.Action<double>?)this.onChangeEnd, vsync: this.vsync, textDirection: Directionality.of(context), cursor: (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? global::Doroti.Framework.Services.SystemMouseCursors.click : global::Doroti.Framework.Services.MouseCursor.defer)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderCupertinoSlider__slider)(object)renderObject;
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Ignore(((Func<_RenderCupertinoSlider__slider>)(() =>
{
    var __cascade = __renderObject;
    __cascade.value = this.value;
    __cascade.divisions = this.divisions;
    __cascade.activeColor = this.activeColor;
    __cascade.thumbColor = CupertinoDynamicColor.resolve(this.thumbColor, context);
    __cascade.trackColor = CupertinoDynamicColor.resolve(CupertinoColors.systemFill, context);
    __cascade.onChanged = this.onChanged;
    __cascade.onChangeStart = this.onChangeStart;
    __cascade.onChangeEnd = this.onChangeEnd;
    __cascade.textDirection = Directionality.of(context);
    return __cascade;
}))());
    }

}

public static partial class SliderLibrary
{
    internal static double _kPadding = 8.0;
}

public static partial class SliderLibrary
{
    internal static double _kSliderHeight = (2.0 * ((CupertinoThumbPainter.radius + SliderLibrary._kPadding)));
}

public static partial class SliderLibrary
{
    internal static double _kSliderWidth = 176.0;
}

public static partial class SliderLibrary
{
    internal static Duration _kDiscreteTransitionDuration = Duration.Create(milliseconds: 500L);
}

public static partial class SliderLibrary
{
    internal static double _kAdjustmentUnit = 0.1;
}

public class _RenderCupertinoSlider__slider : global::Doroti.Framework.Rendering.RenderConstrainedBox
{
    internal virtual double _value { get; set; } = default!;
    internal virtual long? _divisions { get; set; } = default;
    internal virtual Color _activeColor { get; set; } = default!;
    internal virtual Color _thumbColor { get; set; } = default!;
    internal virtual Color _trackColor { get; set; } = default!;
    internal virtual global::System.Action<double, bool>? _onChanged { get; set; } = default;
    public virtual global::System.Action<double>? onChangeStart { get; set; } = default;
    public virtual global::System.Action<double>? onChangeEnd { get; set; } = default;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _position { get; set; } = default!;
    internal virtual global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer _drag { get; set; } = default!;
    internal virtual double _currentDragValue { get; set; } = 0.0;
    internal virtual Duration? _lastUpdateTimestamp { get; set; } = default;
    internal virtual global::Doroti.Framework.Services.MouseCursor _cursor { get; set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>? onEnter { get; set; } = default;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerHoverEvent>? onHover { get; set; } = default;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>? onExit { get; set; } = default;

    internal _RenderCupertinoSlider__slider(double value, long? divisions = null, Color activeColor = default!, Color thumbColor = default!, Color trackColor = default!, global::System.Action<double, bool>? onChanged = null, global::System.Action<double>? onChangeStart = null, global::System.Action<double>? onChangeEnd = null, global::Doroti.Framework.Scheduler.TickerProvider vsync = default!, TextDirection textDirection = default!, global::Doroti.Framework.Services.MouseCursor cursor = default!) : base(additionalConstraints: global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: SliderLibrary._kSliderWidth, height: SliderLibrary._kSliderHeight))
    {
        global::Doroti.Framework.Services.MouseCursor __cursor = cursor ?? global::Doroti.Framework.Services.MouseCursor.defer;
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        this._cursor = __cursor;
        this._value = DartRuntimePrimitives.RequireValue(value);
        this._divisions = divisions;
        this._activeColor = activeColor;
        this._thumbColor = thumbColor;
        this._trackColor = trackColor;
        this._onChanged = onChanged;
        this._textDirection = textDirection;
        System.Diagnostics.Debug.Assert(((value >= 0.0) && (value <= 1.0)));
        _drag = ((Func<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)(() =>
{
    var __cascade = new global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer();
    __cascade.onStart = this._handleDragStart;
    __cascade.onUpdate = this._handleDragUpdate;
    __cascade.onEnd = this._handleDragEnd;
    return __cascade;
}))();
        _position = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(value: DartRuntimePrimitives.RequireValue(value), duration: SliderLibrary._kDiscreteTransitionDuration, vsync: vsync);
    __cascade.addListener(() => this.markNeedsPaint());
    return __cascade;
}))();
    }

    public virtual double value
    {
        get => this._value;
        set
        {
            var newValue = value;
            DartRuntimePrimitives.Assert(() => ((newValue >= 0.0) && (newValue <= 1.0)));
            if ((newValue == this._value))
            {
                return;
            }
            _value = newValue;
            if ((this.divisions is not null))
            {
                long divisions__value13358 = DartRuntimePrimitives.RequireValue(divisions);
                this._position.animateTo(newValue, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
            }
            else
            {
                this._position.value = newValue;
            }
            markNeedsSemanticsUpdate();
        }
    }
    public virtual long? divisions
    {
        get => this._divisions;
        set
        {
            var __value = value;
            if ((__value == this._divisions))
            {
                return;
            }
            _divisions = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.Color activeColor
    {
        get => this._activeColor;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._activeColor)))
            {
                return;
            }
            _activeColor = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.Color thumbColor
    {
        get => this._thumbColor;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._thumbColor)))
            {
                return;
            }
            _thumbColor = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.Color trackColor
    {
        get => this._trackColor;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._trackColor)))
            {
                return;
            }
            _trackColor = __value;
            markNeedsPaint();
        }
    }
    public virtual global::System.Action<double, bool>? onChanged
    {
        get => this._onChanged;
        set
        {
            var __value = value;
            if ((object.Equals((global::System.Action<double, bool>?)__value, (global::System.Action<double, bool>?)this._onChanged)))
            {
                return;
            }
            bool wasInteractive__14512 = this.isInteractive;
            _onChanged = (global::System.Action<double, bool>)__value;
            if ((wasInteractive__14512 != this.isInteractive))
            {
                markNeedsSemanticsUpdate();
            }
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
            markNeedsPaint();
        }
    }
    internal virtual double _discretizedCurrentDragValue
    {
        get
        {
            double dragValue__15146 = Dart_uiLibrary.clampDouble(this._currentDragValue, 0.0, 1.0);
            if ((this.divisions is not null))
            {
                long divisions__value15208 = DartRuntimePrimitives.RequireValue(divisions);
                dragValue__15146 = (((dragValue__15146 * DartRuntimePrimitives.RequireValue(this.divisions))).round() / DartRuntimePrimitives.RequireValue(this.divisions));
            }
            return dragValue__15146;
            return default!;
        }
    }
    internal virtual double _trackLeft => SliderLibrary._kPadding;
    internal virtual double _trackRight => DartRuntimePrimitives.ConvertValue<double>((this.size.width - SliderLibrary._kPadding));
    internal virtual double _thumbCenter
    {
        get
        {
            double visualPosition__15462 = (this.textDirection switch { TextDirection.rtl => (1.0 - this._value), TextDirection.ltr => this._value, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble((this._trackLeft + CupertinoThumbPainter.radius), (this._trackRight - CupertinoThumbPainter.radius), visualPosition__15462));
            return default!;
        }
    }
    public virtual bool isInteractive => DartRuntimePrimitives.ConvertValue<bool>((this.onChanged is not null));
    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details) => _startInteraction(details);
    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (!this.isInteractive)
        {
            return;
        }
        double extent__16026 = Math.Max(SliderLibrary._kPadding, (this.size.width - (2.0 * ((SliderLibrary._kPadding + CupertinoThumbPainter.radius)))));
        double valueDelta__16155 = (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).primaryDelta) / extent__16026);
        _currentDragValue += (this.textDirection switch { TextDirection.rtl => -valueDelta__16155, TextDirection.ltr => valueDelta__16155, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var isFast__16406 = false;
        Duration? currentTimestamp__16442 = ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).sourceTimeStamp;
        if (((currentTimestamp__16442 is not null) && (this._lastUpdateTimestamp is not null)))
        {
            Duration currentTimestamp__16442__value16494 = DartRuntimePrimitives.RequireValue(currentTimestamp__16442);
            long timeDelta__16570 = ((DartRuntimePrimitives.RequireValue(currentTimestamp__16442__value16494) - DartRuntimePrimitives.RequireValue(this._lastUpdateTimestamp))).inMilliseconds;
            double velocity__16660 = ((valueDelta__16155.abs() * 1000.0) / timeDelta__16570);
            isFast__16406 = (velocity__16660 > SliderLibrary._kVelocityThreshold);
        }
        _lastUpdateTimestamp = currentTimestamp__16442;
        this.onChanged!(this._discretizedCurrentDragValue, isFast__16406);
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details) => _endInteraction();
    internal virtual void _startInteraction(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        if (this.isInteractive)
        {
            this.onChangeStart?.Invoke(this._discretizedCurrentDragValue);
            _currentDragValue = this._value;
            _lastUpdateTimestamp = ((global::Doroti.Framework.Gestures.DragStartDetails)details).sourceTimeStamp;
            this.onChanged!(this._discretizedCurrentDragValue, false);
        }
    }

    internal virtual void _endInteraction()
    {
        this.onChangeEnd?.Invoke(this._discretizedCurrentDragValue);
        _currentDragValue = 0.0;
        _lastUpdateTimestamp = null;
    }

    public override bool hitTestSelf(Offset position)
    {
        return (((position.dx - this._thumbCenter)).abs() < (CupertinoThumbPainter.radius + SliderLibrary._kPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, global::Doroti.Framework.Gestures.HitTestEntry<global::Doroti.Framework.Gestures.HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if (((@event is global::Doroti.Framework.Gestures.PointerDownEvent) && this.isInteractive))
        {
            global::Doroti.Framework.Gestures.PointerDownEvent @event__as17793 = (global::Doroti.Framework.Gestures.PointerDownEvent)@event;
            this._drag.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)((global::Doroti.Framework.Gestures.PointerDownEvent)@event__as17793));
        }
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        var (visualPosition__17966, leftColor__17988, rightColor__18005) = (this.textDirection switch { TextDirection.rtl => (((double, Color, Color))(((1.0 - ((global::Doroti.Framework.Animation.AnimationController)this._position).value), this._activeColor, this.trackColor))), TextDirection.ltr => (((double, Color, Color))((((global::Doroti.Framework.Animation.AnimationController)this._position).value, this.trackColor, this._activeColor))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double trackCenter__18219 = (offset.dy + (this.size.height / 2.0));
        double trackLeft__18281 = (offset.dx + this._trackLeft);
        double trackTop__18334 = (trackCenter__18219 - 1.0);
        double trackBottom__18381 = (trackCenter__18219 + 1.0);
        double trackRight__18431 = (offset.dx + this._trackRight);
        double trackActive__18486 = (offset.dx + this._thumbCenter);
        global::Doroti.Ui.Canvas canvas__18544 = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        if ((visualPosition__17966 > 0.0))
        {
            var paint__18613 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = rightColor__18005;
    return __cascade;
}))();
            canvas__18544.drawRRect(global::Doroti.Ui.RRect.fromLTRBXY(trackLeft__18281, trackTop__18334, trackActive__18486, trackBottom__18381, 1.0, 1.0), paint__18613);
        }
        if ((visualPosition__17966 < 1.0))
        {
            var paint__18954 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = leftColor__17988;
    return __cascade;
}))();
            canvas__18544.drawRRect(global::Doroti.Ui.RRect.fromLTRBXY(trackActive__18486, trackTop__18334, trackRight__18431, trackBottom__18381, 1.0, 1.0), paint__18954);
        }
        var thumbCenter__19261 = new global::Doroti.Ui.Offset(trackActive__18486, trackCenter__18219);
        new CupertinoThumbPainter(color: this.thumbColor).paint(canvas__18544, global::Doroti.Ui.Rect.fromCircle(center: thumbCenter__19261, radius: CupertinoThumbPainter.radius));
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = this.isInteractive;
        config.isSlider = true;
        if (this.isInteractive)
        {
            ((dynamic)config).textDirection = this.textDirection;
            config.onIncrease = (global::System.Action)this._increaseAction;
            config.onDecrease = (global::System.Action)this._decreaseAction;
            config.value = $"{((this.value * 100L)).round()}%";
            config.increasedValue = $"{((Dart_uiLibrary.clampDouble((this.value + this._semanticActionUnit), 0.0, 1.0) * 100L)).round()}%";
            config.decreasedValue = $"{((Dart_uiLibrary.clampDouble((this.value - this._semanticActionUnit), 0.0, 1.0) * 100L)).round()}%";
        }
    }

    internal virtual double _semanticActionUnit => ((this.divisions is not null) ? (1.0 / DartRuntimePrimitives.RequireValue(this.divisions)) : SliderLibrary._kAdjustmentUnit);
    internal virtual void _increaseAction()
    {
        if (this.isInteractive)
        {
            this.onChanged!(Dart_uiLibrary.clampDouble((this.value + this._semanticActionUnit), 0.0, 1.0), false);
        }
    }

    internal virtual void _decreaseAction()
    {
        if (this.isInteractive)
        {
            this.onChanged!(Dart_uiLibrary.clampDouble((this.value - this._semanticActionUnit), 0.0, 1.0), false);
        }
    }

    public virtual global::Doroti.Framework.Services.MouseCursor cursor
    {
        get => this._cursor;
        set
        {
            var __value = value;
            if ((!object.Equals(this._cursor, __value)))
            {
                _cursor = __value;
                markNeedsPaint();
            }
        }
    }
    public virtual bool validForMouseTracker => false;
    public override void dispose()
    {
        this._drag.dispose();
        this._position.dispose();
        base.dispose();
    }

}
