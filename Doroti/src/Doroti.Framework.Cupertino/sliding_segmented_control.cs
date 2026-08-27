// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/sliding_segmented_control.dart
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

public static partial class Sliding_segmented_controlLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsGeometry _kHorizontalItemPadding = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 2, horizontal: 3));
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Radius _kCornerRadius = global::Doroti.Ui.Radius.circular(9);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Radius _kThumbRadius = global::Doroti.Ui.Radius.circular(7);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kThumbInsets = global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 1);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kMinSegmentedControlHeight = 28.0;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Color _kSeparatorColor = new global::Doroti.Ui.Color(1301188243L);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static CupertinoDynamicColor _kThumbColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(4294967295L), darkColor: new global::Doroti.Ui.Color(4284703590L));
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kSeparatorInset = global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 5);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kSeparatorWidth = 1;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Radius _kSeparatorRadius = global::Doroti.Ui.Radius.circular((Sliding_segmented_controlLibrary._kSeparatorWidth / 2L));
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kMinThumbScale = 0.95;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kMaxThumbScaleForMomentary = 1.05;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kSegmentMinPadding = 10;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kTouchYDistanceThreshold = (50.0 * 50.0);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kContentPressedMinOpacity = 0.2;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kFontSize = 13.0;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static FontWeight _kFontWeight = FontWeight.w500;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static FontWeight _kHighlightedFontWeight = FontWeight.w600;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Color _kDisabledContentColor = global::Doroti.Ui.Color.fromARGB(115L, 122L, 122L, 122L);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static global::Doroti.Framework.Physics.SpringSimulation _kThumbSpringAnimationSimulation = new global::Doroti.Framework.Physics.SpringSimulation(new global::Doroti.Framework.Physics.SpringDescription(mass: 1, stiffness: 503.551, damping: 44.8799), 0, 1, 0);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Duration _kSpringAnimationDuration = Duration.Create(milliseconds: 412L);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Duration _kOpacityAnimationDuration = Duration.Create(milliseconds: 470L);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Duration _kHighlightAnimationDuration = Duration.Create(milliseconds: 200L);
}

public class _Segment__sliding_segmented_control<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual bool pressed { get; private set; } = default!;
    public virtual bool highlighted { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;
    public virtual _SegmentLocation__sliding_segmented_control segmentLocation { get; private set; } = default!;
    public virtual bool isMomentary { get; private set; } = default!;
    public virtual bool isDragging { get; private set; } = default!;

    internal _Segment__sliding_segmented_control(global::Doroti.Framework.Foundation.ValueKey<T> key, global::Doroti.Framework.Widgets.Widget child, bool pressed, bool highlighted, bool isDragging, bool enabled, _SegmentLocation__sliding_segmented_control segmentLocation, bool isMomentary) : base(key: key)
    {
        this.child = child;
        this.pressed = pressed;
        this.highlighted = highlighted;
        this.isDragging = isDragging;
        this.enabled = enabled;
        this.segmentLocation = segmentLocation;
        this.isMomentary = isMomentary;
    }

    public virtual bool shouldFadeoutContent => DartRuntimePrimitives.ConvertValue<bool>((((this.pressed && !this.highlighted) && this.enabled) && !this.isMomentary));
    public virtual bool shouldScaleContent => DartRuntimePrimitives.ConvertValue<bool>(((this.pressed && this.enabled) && (((this.highlighted && this.isDragging) || this.isMomentary))));
    public virtual bool shouldHighlightContent => DartRuntimePrimitives.ConvertValue<bool>((this.highlighted && !this.isMomentary));
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SegmentState__sliding_segmented_control<T>());
}

public class _SegmentState__sliding_segmented_control<T> : global::Doroti.Framework.Widgets.State<_Segment__sliding_segmented_control<T>>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<_Segment__sliding_segmented_control<T>>
{
    public virtual global::Doroti.Framework.Animation.AnimationController highlightPressScaleController { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> highlightPressScaleAnimation { get; set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        highlightPressScaleController = new global::Doroti.Framework.Animation.AnimationController(duration: Sliding_segmented_controlLibrary._kOpacityAnimationDuration, value: (((_Segment__sliding_segmented_control<T>)(object)this.widget).shouldScaleContent ? 1 : 0), vsync: this);
        highlightPressScaleAnimation = this.highlightPressScaleController.drive(new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: Sliding_segmented_controlLibrary._kMinThumbScale));
    }

    public override void didUpdateWidget(_Segment__sliding_segmented_control<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(oldWidget.key, ((Widget)(object)this.widget).key)));
        if ((((_Segment__sliding_segmented_control<T>)oldWidget).shouldScaleContent != ((_Segment__sliding_segmented_control<T>)(object)this.widget).shouldScaleContent))
        {
            global::Doroti.Framework.Animation.Animatable<double> scaleAnimation = ((((_Segment__sliding_segmented_control<T>)(object)this.widget).isMomentary && ((_Segment__sliding_segmented_control<T>)(object)this.widget).shouldScaleContent) ? new global::Doroti.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: ((global::Doroti.Framework.Animation.Animation<double>)this.highlightPressScaleAnimation).value, end: Sliding_segmented_controlLibrary._kMaxThumbScaleForMomentary), weight: 50), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: Sliding_segmented_controlLibrary._kMaxThumbScaleForMomentary, end: 1.0), weight: 50) }) : new global::Doroti.Framework.Animation.Tween<double>(begin: ((global::Doroti.Framework.Animation.Animation<double>)this.highlightPressScaleAnimation).value, end: (((_Segment__sliding_segmented_control<T>)(object)this.widget).shouldScaleContent ? Sliding_segmented_controlLibrary._kMinThumbScale : 1.0)));
            highlightPressScaleAnimation = this.highlightPressScaleController.drive(scaleAnimation);
            this.highlightPressScaleController.animateWith(Sliding_segmented_controlLibrary._kThumbSpringAnimationSimulation);
        }
    }

    public override void dispose()
    {
        this.highlightPressScaleController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Painting.Alignment scaleAlignment = (((_Segment__sliding_segmented_control<T>)(object)this.widget).segmentLocation switch { _SegmentLocation__sliding_segmented_control.leftmost => global::Doroti.Framework.Painting.Alignment.centerLeft, _SegmentLocation__sliding_segmented_control.rightmost => global::Doroti.Framework.Painting.Alignment.centerRight, _SegmentLocation__sliding_segmented_control.inbetween => global::Doroti.Framework.Painting.Alignment.center, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.MetaData(behavior: global::Doroti.Framework.Rendering.HitTestBehavior.opaque, child: new global::Doroti.Framework.Widgets.IndexedStack(alignment: global::Doroti.Framework.Painting.Alignment.center, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedOpacity(opacity: (((_Segment__sliding_segmented_control<T>)(object)this.widget).shouldFadeoutContent ? Sliding_segmented_controlLibrary._kContentPressedMinOpacity : 1), duration: Sliding_segmented_controlLibrary._kOpacityAnimationDuration, curve: global::Doroti.Framework.Animation.Curves.ease, child: new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(style: DefaultTextStyle.of(context).style.merge(new global::Doroti.Framework.Painting.TextStyle(fontWeight: (((_Segment__sliding_segmented_control<T>)(object)this.widget).shouldHighlightContent ? Sliding_segmented_controlLibrary._kHighlightedFontWeight : Sliding_segmented_controlLibrary._kFontWeight), fontSize: Sliding_segmented_controlLibrary._kFontSize, color: (((_Segment__sliding_segmented_control<T>)(object)this.widget).enabled ? null : Sliding_segmented_controlLibrary._kDisabledContentColor))), duration: Sliding_segmented_controlLibrary._kHighlightAnimationDuration, curve: global::Doroti.Framework.Animation.Curves.ease, child: new global::Doroti.Framework.Widgets.ScaleTransition(alignment: scaleAlignment, scale: this.highlightPressScaleAnimation, child: ((_Segment__sliding_segmented_control<T>)(object)this.widget).child)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(DefaultTextStyle.merge(style: new global::Doroti.Framework.Painting.TextStyle(fontWeight: Sliding_segmented_controlLibrary._kHighlightedFontWeight, fontSize: Sliding_segmented_controlLibrary._kFontSize), child: ((_Segment__sliding_segmented_control<T>)(object)this.widget).child)) })));
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
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
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
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

public class _SegmentSeparator__sliding_segmented_control : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool highlighted { get; private set; } = default!;

    internal _SegmentSeparator__sliding_segmented_control(global::Doroti.Framework.Foundation.ValueKey<long> key, bool highlighted) : base(key: key)
    {
        this.highlighted = highlighted;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SegmentSeparatorState__sliding_segmented_control());
}

public class _SegmentSeparatorState__sliding_segmented_control : global::Doroti.Framework.Widgets.State<_SegmentSeparator__sliding_segmented_control>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<_SegmentSeparator__sliding_segmented_control>
{
    public virtual global::Doroti.Framework.Animation.AnimationController separatorOpacityController { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        separatorOpacityController = new global::Doroti.Framework.Animation.AnimationController(duration: Sliding_segmented_controlLibrary._kSpringAnimationDuration, value: (((_SegmentSeparator__sliding_segmented_control)(object)this.widget).highlighted ? 0 : 1), vsync: this);
    }

    public override void didUpdateWidget(_SegmentSeparator__sliding_segmented_control oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(oldWidget.key, this.widget.key)));
        if ((((_SegmentSeparator__sliding_segmented_control)oldWidget).highlighted != ((_SegmentSeparator__sliding_segmented_control)(object)this.widget).highlighted))
        {
            this.separatorOpacityController.animateTo((((_SegmentSeparator__sliding_segmented_control)(object)this.widget).highlighted ? 0 : 1), duration: Sliding_segmented_controlLibrary._kSpringAnimationDuration, curve: global::Doroti.Framework.Animation.Curves.ease);
        }
    }

    public override void dispose()
    {
        this.separatorOpacityController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: this.separatorOpacityController, child: new global::Doroti.Framework.Widgets.SizedBox(width: Sliding_segmented_controlLibrary._kSeparatorWidth), builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: Sliding_segmented_controlLibrary._kSeparatorInset, child: new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: Sliding_segmented_controlLibrary._kSeparatorColor.withOpacity((Sliding_segmented_controlLibrary._kSeparatorColor.opacity * ((global::Doroti.Framework.Animation.AnimationController)this.separatorOpacityController).value)), borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(Sliding_segmented_controlLibrary._kSeparatorRadius)), child: child)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
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
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
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
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

public class CupertinoSlidingSegmentedControl<T> : global::Doroti.Framework.Widgets.StatefulWidget where T : notnull
{
    public virtual DartMap<T, global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual HashSet<T> disabledChildren { get; private set; } = default!;
    public virtual T? groupValue { get; private set; }
    public virtual global::System.Action<T?> onValueChanged { get; private set; } = default!;
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual bool proportionalWidth { get; private set; } = default!;
    public virtual Color thumbColor { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual bool isMomentary { get; private set; } = default!;

    public CupertinoSlidingSegmentedControl(global::Doroti.Framework.Foundation.Key? key = null, DartMap<T, global::Doroti.Framework.Widgets.Widget> children = default!, global::System.Action<T?> onValueChanged = default!, HashSet<T> disabledChildren = default!, T? groupValue = default, Color thumbColor = default!, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding = default!, Color backgroundColor = default!, bool proportionalWidth = false, bool isMomentary = false) : base(key: key)
    {
        HashSet<T> __disabledChildren = disabledChildren ?? new HashSet<T>();
        Color __thumbColor = thumbColor ?? Sliding_segmented_controlLibrary._kThumbColor;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __padding = padding ?? Sliding_segmented_controlLibrary._kHorizontalItemPadding;
        Color __backgroundColor = backgroundColor ?? CupertinoColors.tertiarySystemFill;
        this.children = children;
        this.onValueChanged = onValueChanged;
        this.disabledChildren = __disabledChildren;
        this.groupValue = groupValue;
        this.thumbColor = __thumbColor;
        this.padding = __padding;
        this.backgroundColor = __backgroundColor;
        this.proportionalWidth = proportionalWidth;
        this.isMomentary = isMomentary;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) >= 2L));
        System.Diagnostics.Debug.Assert(((groupValue is null) || children.Keys.contains(groupValue)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SegmentedControlState__sliding_segmented_control<T>());
}

public class _SlidingSegmentButton__sliding_segmented_control<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    internal _SlidingSegmentButton__sliding_segmented_control(global::Doroti.Framework.Foundation.Key? key = null, T value = default!, global::Doroti.Framework.Widgets.Widget child = default!, bool enabled = default!) : base(key: key)
    {
        this.value = value;
        this.child = child;
        this.enabled = enabled;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SlidingSegmentButtonState__sliding_segmented_control<T>());
}

internal class _SlidingSegmentButtonState__sliding_segmented_control<T> : global::Doroti.Framework.Widgets.State<_SlidingSegmentButton__sliding_segmented_control<T>>, global::Doroti.Framework.Widgets.RadioClient<T>
{
    internal virtual global::Doroti.Framework.Widgets.FocusNode _focusNode { get; private set; } = default!;
    public virtual RadioGroupRegistry<T>? _registry { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _focusNode = new global::Doroti.Framework.Widgets.FocusNode(debugLabel: $"CupertinoSlidingSegmentedControl<{typeof(T)}>[{((_SlidingSegmentButton__sliding_segmented_control<T>)(object)this.widget).value}]");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        registry = (((_SlidingSegmentButton__sliding_segmented_control<T>)(object)this.widget).enabled ? RadioGroup.maybeOf<T>(this.context) : null);
    }

    public override void didUpdateWidget(_SlidingSegmentButton__sliding_segmented_control<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((_SlidingSegmentButton__sliding_segmented_control<T>)oldWidget).enabled != ((_SlidingSegmentButton__sliding_segmented_control<T>)(object)this.widget).enabled))
        {
            registry = (((_SlidingSegmentButton__sliding_segmented_control<T>)(object)this.widget).enabled ? RadioGroup.maybeOf<T>(this.context) : null);
        }
    }

    public override void dispose()
    {
        registry = null;
        this._focusNode.dispose();
        base.dispose();
    }

    public virtual bool enabled => ((_SlidingSegmentButton__sliding_segmented_control<T>)(object)this.widget).enabled;
    public virtual T radioValue => ((_SlidingSegmentButton__sliding_segmented_control<T>)(object)this.widget).value;
    public virtual global::Doroti.Framework.Widgets.FocusNode focusNode => this._focusNode;
    public virtual bool tristate => false;
    public virtual void requestFocus()
    {
        if (((_SlidingSegmentButton__sliding_segmented_control<T>)(object)this.widget).enabled)
        {
            this._focusNode.requestFocus();
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Focus(focusNode: this._focusNode, canRequestFocus: ((_SlidingSegmentButton__sliding_segmented_control<T>)(object)this.widget).enabled, onKeyEvent: ((global::System.Func<global::Doroti.Framework.Widgets.FocusNode, global::Doroti.Framework.Services.KeyEvent, global::Doroti.Framework.Widgets.KeyEventResult>?)((node, @event) => global::Doroti.Framework.Widgets.KeyEventResult.ignored)), child: ((_SlidingSegmentButton__sliding_segmented_control<T>)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RadioGroupRegistry<T>? registry
    {
        get => this._registry;
        set
        {
            var newRegistry = value;
            if ((!object.Equals(this._registry, newRegistry)))
            {
                this._registry?.unregisterClient(this);
            }
            this._registry = newRegistry;
            this._registry?.registerClient(this);
        }
    }
}

public class _SegmentedControlState__sliding_segmented_control<T> : global::Doroti.Framework.Widgets.State<CupertinoSlidingSegmentedControl<T>>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<CupertinoSlidingSegmentedControl<T>> where T : notnull
{
    private bool __late_thumbController_initialized;
    private global::Doroti.Framework.Animation.AnimationController __late_thumbController = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController thumbController
    {
        get
        {
            if (!__late_thumbController_initialized)
            {
                __late_thumbController = new global::Doroti.Framework.Animation.AnimationController(duration: Sliding_segmented_controlLibrary._kSpringAnimationDuration, value: 0, vsync: this);
                __late_thumbController_initialized = true;
            }
            return __late_thumbController;
        }
    }
    public virtual global::Doroti.Framework.Animation.Animatable<Rect?>? thumbAnimatable { get; set; } = default;
    private bool __late_thumbScaleController_initialized;
    private global::Doroti.Framework.Animation.AnimationController __late_thumbScaleController = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController thumbScaleController
    {
        get
        {
            if (!__late_thumbScaleController_initialized)
            {
                __late_thumbScaleController = new global::Doroti.Framework.Animation.AnimationController(duration: Sliding_segmented_controlLibrary._kSpringAnimationDuration, value: 0, vsync: this);
                __late_thumbScaleController_initialized = true;
            }
            return __late_thumbScaleController;
        }
    }
    private bool __late_thumbScaleAnimation_initialized;
    private global::Doroti.Framework.Animation.Animation<double> __late_thumbScaleAnimation = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> thumbScaleAnimation
    {
        get
        {
            if (!__late_thumbScaleAnimation_initialized)
            {
                __late_thumbScaleAnimation = this.thumbScaleController.drive(new global::Doroti.Framework.Animation.Tween<double>(begin: 1, end: Sliding_segmented_controlLibrary._kMinThumbScale));
                __late_thumbScaleAnimation_initialized = true;
            }
            return __late_thumbScaleAnimation;
        }
        set { __late_thumbScaleAnimation = value; __late_thumbScaleAnimation_initialized = true; }
    }
    public virtual global::Doroti.Framework.Gestures.TapGestureRecognizer tap { get; private set; } = new global::Doroti.Framework.Gestures.TapGestureRecognizer();
    public virtual global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer drag { get; private set; } = new global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer();
    public virtual global::Doroti.Framework.Gestures.LongPressGestureRecognizer longPress { get; private set; } = new global::Doroti.Framework.Gestures.LongPressGestureRecognizer();
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> segmentedControlRenderWidgetKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual DartMap<T, global::Doroti.Framework.Widgets.GlobalKey<_SlidingSegmentButtonState__sliding_segmented_control<T>>> _segmentKeys { get; private set; } = new DartMap<T, global::Doroti.Framework.Widgets.GlobalKey<_SlidingSegmentButtonState__sliding_segmented_control<T>>>();
    internal virtual bool? _startedOnSelectedSegment { get; set; } = default;
    internal virtual bool _startedOnDisabledSegment { get; set; } = false;
    public virtual T? highlighted { get; set; } = default;
    public virtual T? pressed { get; set; } = default;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        var teamLocal = new global::Doroti.Framework.Gestures.GestureArenaTeam();
        this.longPress.team = teamLocal;
        this.drag.team = teamLocal;
        teamLocal.captain = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Gestures.GestureArenaMember>(this.drag);
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)(() =>
{
    var __cascade = this.drag;
    __cascade.onDown = this.onDown;
    __cascade.onUpdate = this.onUpdate;
    __cascade.onEnd = this.onEnd;
    __cascade.onCancel = this.onCancel;
    return __cascade;
}))());
        this.tap.onTapUp = (global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails>)this.onTapUp;
        this.longPress.onLongPress = (global::System.Action)(() =>
        {
        });
        highlighted = ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).groupValue;
    }

    public override void didUpdateWidget(CupertinoSlidingSegmentedControl<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!this.isThumbDragging && !EqualityComparer<T>.Default.Equals(this.highlighted, ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).groupValue)))
        {
            this.thumbController.animateWith(Sliding_segmented_controlLibrary._kThumbSpringAnimationSimulation);
            thumbAnimatable = null;
            highlighted = ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).groupValue;
        }
    }

    public override void dispose()
    {
        this.thumbScaleController.dispose();
        this.thumbController.dispose();
        this.drag.dispose();
        this.tap.dispose();
        this.longPress.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public virtual bool isThumbDragging => DartRuntimePrimitives.ConvertValue<bool>((((this._startedOnSelectedSegment ?? false)) && !this._startedOnDisabledSegment));
    public virtual T segmentForXPosition(double dx)
    {
        global::Doroti.Framework.Widgets.BuildContext currentContextLocal = ((global::Doroti.Framework.Widgets.GlobalKey<IState>)this.segmentedControlRenderWidgetKey).currentContext!;
        var renderBox = ((_RenderSegmentedControl__sliding_segmented_control<T>?)(object?)currentContextLocal.findRenderObject()!)!;
        long numOfChildren = checked((long)(((CupertinoSlidingSegmentedControl<T>)(object)this.widget).children.Count));
        DartRuntimePrimitives.Assert(() => renderBox.hasSize);
        DartRuntimePrimitives.Assert(() => (numOfChildren >= 2L));
        long segmentIndex = renderBox.getClosestSegmentIndex(dx);
        switch (Directionality.of(this.context))
        {
            case TextDirection.ltr:
                {
                    break;
                }
            case TextDirection.rtl:
                {
                    segmentIndex = ((numOfChildren - 1L) - segmentIndex);
                    break;
                }
        }
        return ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).children.Keys.elementAt(segmentIndex);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasDraggedTooFar(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.RenderBox)renderBox).hasSize);
        global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)(object?)((global::Doroti.Framework.Rendering.RenderBox)renderBox).size);
        global::Doroti.Ui.Offset offCenter = ((global::Doroti.Ui.Offset)(object?)(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).localPosition - new global::Doroti.Ui.Offset((sizeLocal.width / 2L), (sizeLocal.height / 2L))));
        var l2 = ((double)(global::Doroti.Runtime.Dart_mathLibrary.pow(Math.Max(0.0, (offCenter.dx.abs() - (sizeLocal.width / 2L))), 2L) + global::Doroti.Runtime.Dart_mathLibrary.pow(Math.Max(0.0, (offCenter.dy.abs() - (sizeLocal.height / 2L))), 2L)));
        return (l2 > Sliding_segmented_controlLibrary._kTouchYDistanceThreshold);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _playThumbScaleAnimation(bool isExpanding)
    {
        thumbScaleAnimation = this.thumbScaleController.drive(new global::Doroti.Framework.Animation.Tween<double>(begin: ((global::Doroti.Framework.Animation.Animation<double>)this.thumbScaleAnimation).value, end: (isExpanding ? 1 : Sliding_segmented_controlLibrary._kMinThumbScale)));
        this.thumbScaleController.animateWith(Sliding_segmented_controlLibrary._kThumbSpringAnimationSimulation);
    }

    public virtual void onHighlightChangedByGesture(T newValue)
    {
        if (EqualityComparer<T>.Default.Equals(this.highlighted, newValue))
        {
            return;
        }
        setState(((global::System.Action)(() =>
        {
            highlighted = newValue;
        })));
        this.thumbController.animateWith(Sliding_segmented_controlLibrary._kThumbSpringAnimationSimulation);
        thumbAnimatable = null;
    }

    public virtual void onPressedChangedByGesture(T? newValue)
    {
        if (!EqualityComparer<T>.Default.Equals(this.pressed, newValue))
        {
            setState(((global::System.Action)(() =>
            {
                pressed = newValue;
            })));
        }
    }

    public virtual void onTapUp(global::Doroti.Framework.Gestures.TapUpDetails details)
    {
        if (this.isThumbDragging)
        {
            return;
        }
        T segment = ((T)(object?)segmentForXPosition(((global::Doroti.Framework.Gestures.TapUpDetails)details).localPosition.dx));
        onPressedChangedByGesture(default);
        if (!((CupertinoSlidingSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(segment))
        {
            DartRuntimePrimitives.NullAware(this._segmentKeys.GetValueOrDefault(segment), __target => __target.currentState)?.requestFocus();
            if (!EqualityComparer<T>.Default.Equals(segment, ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).groupValue))
            {
                this.widget.onValueChanged(segment);
            }
        }
    }

    public virtual void onDown(global::Doroti.Framework.Gestures.DragDownDetails details)
    {
        T touchDownSegment = ((T)(object?)segmentForXPosition(((global::Doroti.Framework.Gestures.DragDownDetails)details).localPosition.dx));
        _startedOnSelectedSegment = EqualityComparer<T>.Default.Equals(touchDownSegment, this.highlighted);
        _startedOnDisabledSegment = ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(touchDownSegment);
        if (((CupertinoSlidingSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(touchDownSegment))
        {
            return;
        }
        onPressedChangedByGesture(touchDownSegment);
        if (this.isThumbDragging)
        {
            _playThumbScaleAnimation(isExpanding: false);
        }
    }

    public virtual void onUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (this._startedOnDisabledSegment)
        {
            return;
        }
        T touchDownSegment = ((T)(object?)segmentForXPosition(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).localPosition.dx));
        if (((CupertinoSlidingSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(touchDownSegment))
        {
            return;
        }
        if (this.isThumbDragging)
        {
            onPressedChangedByGesture(touchDownSegment);
            onHighlightChangedByGesture(touchDownSegment);
        }
        else
        {
            T? segment = (_hasDraggedTooFar(details) ? default(T) : segmentForXPosition(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).localPosition.dx));
            onPressedChangedByGesture(segment);
        }
    }

    public virtual void onEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        T? pressedLocal = this.pressed;
        if (this.isThumbDragging)
        {
            _playThumbScaleAnimation(isExpanding: true);
            if (!EqualityComparer<T>.Default.Equals(this.highlighted, ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).groupValue))
            {
                DartRuntimePrimitives.NullAware(this._segmentKeys.GetValueOrDefault(this.highlighted), __target => __target.currentState)?.requestFocus();
                this.widget.onValueChanged(this.highlighted);
            }
        }
        else
        {
            if ((pressedLocal is not null))
            {
                onHighlightChangedByGesture(pressedLocal);
                DartRuntimePrimitives.Assert(() => EqualityComparer<T>.Default.Equals(pressedLocal, this.highlighted));
                if (!EqualityComparer<T>.Default.Equals(this.highlighted, ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).groupValue))
                {
                    DartRuntimePrimitives.NullAware(this._segmentKeys.GetValueOrDefault(this.highlighted), __target => __target.currentState)?.requestFocus();
                    this.widget.onValueChanged(this.highlighted);
                }
            }
        }
        onPressedChangedByGesture(default);
        _startedOnSelectedSegment = null;
    }

    public virtual void onCancel()
    {
        if (this.isThumbDragging)
        {
            _playThumbScaleAnimation(isExpanding: true);
        }
        onPressedChangedByGesture(default);
        _startedOnSelectedSegment = null;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(((CupertinoSlidingSegmentedControl<T>)(object)this.widget).children.Count)) >= 2L));
        var childrenLocal = new List<global::Doroti.Framework.Widgets.Widget>();
        var isPreviousSegmentHighlighted = false;
        var index = 0L;
        long? highlightedIndexLocal = default!;
        foreach (MapEntry<T, global::Doroti.Framework.Widgets.Widget> entry in ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).children.entries)
        {
            var isHighlighted = EqualityComparer<T>.Default.Equals(this.highlighted, entry.key);
            if (isHighlighted)
            {
                highlightedIndexLocal = index;
            }
            if ((index != 0L))
            {
                childrenLocal.Add(new _SegmentSeparator__sliding_segmented_control(key: new global::Doroti.Framework.Foundation.ValueKey<long>(index), highlighted: (isPreviousSegmentHighlighted || isHighlighted)));
            }
            global::Doroti.Ui.TextDirection textDirection = Directionality.of(context);
            _SegmentLocation__sliding_segmented_control segmentLocationLocal = (textDirection switch { TextDirection.ltr when ((index == 0L)) => _SegmentLocation__sliding_segmented_control.leftmost, TextDirection.ltr when ((index == (checked((long)(((CupertinoSlidingSegmentedControl<T>)(object)this.widget).children.Count)) - 1L))) => _SegmentLocation__sliding_segmented_control.rightmost, TextDirection.rtl when ((index == (checked((long)(((CupertinoSlidingSegmentedControl<T>)(object)this.widget).children.Count)) - 1L))) => _SegmentLocation__sliding_segmented_control.leftmost, TextDirection.rtl when ((index == 0L)) => _SegmentLocation__sliding_segmented_control.rightmost, TextDirection.ltr => _SegmentLocation__sliding_segmented_control.inbetween, TextDirection.rtl => _SegmentLocation__sliding_segmented_control.inbetween, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            global::Doroti.Framework.Widgets.GlobalKey<_SlidingSegmentButtonState__sliding_segmented_control<T>> segmentKey = this._segmentKeys.putIfAbsent(entry.key, (() => global::Doroti.Framework.Widgets.GlobalKey<_SlidingSegmentButtonState__sliding_segmented_control<T>>.Create()));
            childrenLocal.Add(new _SlidingSegmentButton__sliding_segmented_control<T>(key: segmentKey, value: entry.key, enabled: !((CupertinoSlidingSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(entry.key), child: new global::Doroti.Framework.Widgets.Semantics(button: true, onTap: ((global::System.Action)(() =>
            {
                if (((CupertinoSlidingSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(entry.key))
                {
                    return;
                }
                DartRuntimePrimitives.NullAware(this._segmentKeys.GetValueOrDefault(entry.key), __target => __target.currentState)?.requestFocus();
                this.widget.onValueChanged(entry.key);
            })), inMutuallyExclusiveGroup: true, selected: EqualityComparer<T>.Default.Equals(((CupertinoSlidingSegmentedControl<T>)(object)this.widget).groupValue, entry.key), child: new global::Doroti.Framework.Widgets.MouseRegion(cursor: (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? global::Doroti.Framework.Services.SystemMouseCursors.click : global::Doroti.Framework.Services.MouseCursor.defer), child: new _Segment__sliding_segmented_control<T>(key: new global::Doroti.Framework.Foundation.ValueKey<T>(entry.key), highlighted: isHighlighted, pressed: EqualityComparer<T>.Default.Equals(this.pressed, entry.key), isDragging: this.isThumbDragging, enabled: !((CupertinoSlidingSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(entry.key), segmentLocation: segmentLocationLocal, isMomentary: ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).isMomentary, child: entry.value)))));
            index += 1L;
            isPreviousSegmentHighlighted = isHighlighted;
        }
        DartRuntimePrimitives.Assert(() => (((highlightedIndexLocal is null)) == ((this.highlighted is null))));
        switch (Directionality.of(context))
        {
            case TextDirection.ltr:
                {
                    break;
                }
            case TextDirection.rtl:
                {
                    childrenLocal = System.Linq.Enumerable.Reverse(childrenLocal).ToList();
                    if ((highlightedIndexLocal is not null))
                    {
                        long highlightedIndex__30239__value33090 = DartRuntimePrimitives.RequireValue(highlightedIndexLocal);
                        highlightedIndexLocal = ((index - 1L) - DartRuntimePrimitives.RequireValue(highlightedIndex__30239__value33090));
                    }
                    break;
                }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Actions(actions: new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.VoidCallbackIntent)] = new global::Doroti.Framework.Widgets.VoidCallbackAction() }, child: new global::Doroti.Framework.Widgets.RadioGroup<T>(groupValue: ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).groupValue, onChanged: ((global::System.Action<T?>)((value) =>
        {
            if (((value is not null) && !((CupertinoSlidingSegmentedControl<T>)(object)this.widget).disabledChildren.Contains(value)))
            {
                this.widget.onValueChanged(value);
            }
        })), child: new global::Doroti.Framework.Widgets.UnconstrainedBox(constrainedAxis: global::Doroti.Framework.Painting.Axis.horizontal, child: new global::Doroti.Framework.Widgets.Container(clipBehavior: Clip.antiAlias, padding: ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).padding.resolve(Directionality.of(context)), decoration: new global::Doroti.Framework.Painting.ShapeDecoration(shape: new global::Doroti.Framework.Painting.RoundedSuperellipseBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(DialogLibrary._kCornerRadius))), color: CupertinoDynamicColor.resolve(((CupertinoSlidingSegmentedControl<T>)(object)this.widget).backgroundColor, context)), child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: this.thumbScaleAnimation, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new _SegmentedControlRenderWidget__sliding_segmented_control<T>(key: this.segmentedControlRenderWidgetKey, highlightedIndex: (((CupertinoSlidingSegmentedControl<T>)(object)this.widget).isMomentary ? null : highlightedIndexLocal), thumbColor: CupertinoDynamicColor.resolve(((CupertinoSlidingSegmentedControl<T>)(object)this.widget).thumbColor, context), thumbScale: ((global::Doroti.Framework.Animation.Animation<double>)this.thumbScaleAnimation).value, proportionalWidth: ((CupertinoSlidingSegmentedControl<T>)(object)this.widget).proportionalWidth, state: this, children: childrenLocal));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))))));
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
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
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
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _SegmentedControlRenderWidget__sliding_segmented_control<T> : global::Doroti.Framework.Widgets.MultiChildRenderObjectWidget
{
    public virtual long? highlightedIndex { get; private set; }
    public virtual Color thumbColor { get; private set; } = default!;
    public virtual double thumbScale { get; private set; } = default!;
    public virtual bool proportionalWidth { get; private set; } = default!;
    public virtual _SegmentedControlState__sliding_segmented_control<T> state { get; private set; } = default!;

    internal _SegmentedControlRenderWidget__sliding_segmented_control(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget> children = default!, long? highlightedIndex = default!, Color thumbColor = default!, double thumbScale = default!, bool proportionalWidth = default!, _SegmentedControlState__sliding_segmented_control<T> state = default!) : base(key: key, children: children ?? new List<global::Doroti.Framework.Widgets.Widget>())
    {
        this.highlightedIndex = highlightedIndex;
        this.thumbColor = thumbColor;
        this.thumbScale = thumbScale;
        this.proportionalWidth = proportionalWidth;
        this.state = state;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSegmentedControl__sliding_segmented_control<T>(highlightedIndex: this.highlightedIndex, thumbColor: this.thumbColor, thumbScale: this.thumbScale, proportionalWidth: this.proportionalWidth, state: this.state));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSegmentedControl__sliding_segmented_control<T>)(object)renderObject;
        DartRuntimePrimitives.Assert(() => (object.Equals(((_RenderSegmentedControl__sliding_segmented_control<T>)__renderObject).state, this.state)));
        DartRuntimePrimitives.Ignore(((Func<_RenderSegmentedControl__sliding_segmented_control<T>>)(() =>
{
    var __cascade = __renderObject;
    __cascade.thumbColor = this.thumbColor;
    __cascade.thumbScale = this.thumbScale;
    __cascade.highlightedIndex = this.highlightedIndex;
    __cascade.proportionalWidth = this.proportionalWidth;
    return __cascade;
}))());
    }

}

internal class _SegmentedControlContainerBoxParentData__sliding_segmented_control : global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>
{
}

public enum _SegmentLocation__sliding_segmented_control
{
    leftmost,
    rightmost,
    inbetween
}

public class _RenderSegmentedControl__sliding_segmented_control<T> : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>>, global::Doroti.Framework.Rendering.RenderBoxContainerDefaultsMixin<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>>
{
    public virtual _SegmentedControlState__sliding_segmented_control<T> state { get; private set; } = default!;
    public virtual Rect? currentThumbRect { get; set; } = default;
    internal virtual double _thumbScale { get; set; } = default!;
    internal virtual long? _highlightedIndex { get; set; } = default;
    internal virtual Color _thumbColor { get; set; } = default!;
    internal virtual bool _proportionalWidth { get; set; } = default!;
    public virtual Paint separatorPaint { get; private set; } = new global::Doroti.Ui.Paint();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderSegmentedControl__sliding_segmented_control(long? highlightedIndex, Color thumbColor, double thumbScale, bool proportionalWidth, _SegmentedControlState__sliding_segmented_control<T> state)
    {
        this.state = state;
        this._highlightedIndex = highlightedIndex;
        this._thumbColor = thumbColor;
        this._thumbScale = thumbScale;
        this._proportionalWidth = proportionalWidth;
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        global::Doroti.Framework.Rendering.RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.attach(owner);
            var childParentData = ((global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        ((_SegmentedControlState__sliding_segmented_control<T>)this.state).thumbController.addListener(() => this.markNeedsPaint());
    }

    public override void detach()
    {
        ((_SegmentedControlState__sliding_segmented_control<T>)this.state).thumbController.removeListener(() => this.markNeedsPaint());
        base.detach();
        global::Doroti.Framework.Rendering.RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.detach();
            var childParentData = ((global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual double thumbScale
    {
        get => this._thumbScale;
        set
        {
            var __value = value;
            if ((this._thumbScale == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _thumbScale = DartRuntimePrimitives.RequireValue(__value);
            if ((((_SegmentedControlState__sliding_segmented_control<T>)this.state).highlighted is not null))
            {
                markNeedsPaint();
            }
        }
    }
    public virtual long? highlightedIndex
    {
        get => this._highlightedIndex;
        set
        {
            var __value = value;
            if ((this._highlightedIndex == __value))
            {
                return;
            }
            _highlightedIndex = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.Color thumbColor
    {
        get => this._thumbColor;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this._thumbColor, __value)))
            {
                return;
            }
            _thumbColor = __value;
            markNeedsPaint();
        }
    }
    public virtual bool proportionalWidth
    {
        get => this._proportionalWidth;
        set
        {
            var __value = value;
            if ((this._proportionalWidth == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _proportionalWidth = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, global::Doroti.Framework.Gestures.HitTestEntry<global::Doroti.Framework.Gestures.HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if (((@event is global::Doroti.Framework.Gestures.PointerDownEvent) && !((_SegmentedControlState__sliding_segmented_control<T>)this.state).isThumbDragging))
        {
            global::Doroti.Framework.Gestures.PointerDownEvent @event__as40275 = (global::Doroti.Framework.Gestures.PointerDownEvent)@event;
            ((_SegmentedControlState__sliding_segmented_control<T>)this.state).tap.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)((global::Doroti.Framework.Gestures.PointerDownEvent)@event__as40275));
            ((_SegmentedControlState__sliding_segmented_control<T>)this.state).longPress.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)((global::Doroti.Framework.Gestures.PointerDownEvent)@event__as40275));
            ((_SegmentedControlState__sliding_segmented_control<T>)this.state).drag.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)((global::Doroti.Framework.Gestures.PointerDownEvent)@event__as40275));
        }
    }

    public virtual double separatorWidth => DartRuntimePrimitives.ConvertValue<double>((Sliding_segmented_controlLibrary._kSeparatorInset.horizontal + Sliding_segmented_controlLibrary._kSeparatorWidth));
    public virtual double totalSeparatorWidth => DartRuntimePrimitives.ConvertValue<double>((this.separatorWidth * ((checked((long)(this.childCount / 2L))))));
    public virtual long getClosestSegmentIndex(double dx)
    {
        var index = 0L;
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__sliding_segmented_control?)(object?)child.parentData!)!;
            double clampX = Dart_uiLibrary.clampDouble(dx, childParentData.offset.dx, (((global::Doroti.Framework.Rendering.RenderBox)child).size.width + childParentData.offset.dx));
            if ((dx <= clampX))
            {
                break;
            }
            index++;
            child = nonSeparatorChildAfter(child);
        }
        long segmentCount = ((checked((long)(this.childCount / 2L))) + 1L);
        return Math.Min(index, (segmentCount - 1L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox? nonSeparatorChildAfter(global::Doroti.Framework.Rendering.RenderBox child)
    {
        global::Doroti.Framework.Rendering.RenderBox? nextChild = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)childAfter(child));
        return ((nextChild is null) ? null : childAfter(nextChild));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        long childCountLocal = ((checked((long)(this.childCount / 2L))) + 1L);
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        double maxMinChildWidth = 0;
        while ((child is not null))
        {
            double childWidth = child.getMinIntrinsicWidth(height);
            maxMinChildWidth = Math.Max(maxMinChildWidth, childWidth);
            child = nonSeparatorChildAfter(child);
        }
        return ((((maxMinChildWidth + (2L * Sliding_segmented_controlLibrary._kSegmentMinPadding))) * childCountLocal) + this.totalSeparatorWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        long childCountLocal = ((checked((long)(this.childCount / 2L))) + 1L);
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        double maxMaxChildWidth = 0;
        while ((child is not null))
        {
            double childWidth = child.getMaxIntrinsicWidth(height);
            maxMaxChildWidth = Math.Max(maxMaxChildWidth, childWidth);
            child = nonSeparatorChildAfter(child);
        }
        return ((((maxMaxChildWidth + (2L * Sliding_segmented_controlLibrary._kSegmentMinPadding))) * childCountLocal) + this.totalSeparatorWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        double maxMinChildHeight = Segmented_controlLibrary._kMinSegmentedControlHeight;
        while ((child is not null))
        {
            double childHeight = child.getMinIntrinsicHeight(width);
            maxMinChildHeight = Math.Max(maxMinChildHeight, childHeight);
            child = nonSeparatorChildAfter(child);
        }
        return maxMinChildHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        double maxMaxChildHeight = Segmented_controlLibrary._kMinSegmentedControlHeight;
        while ((child is not null))
        {
            double childHeight = child.getMaxIntrinsicHeight(width);
            maxMaxChildHeight = Math.Max(maxMaxChildHeight, childHeight);
            child = nonSeparatorChildAfter(child);
        }
        return maxMaxChildHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return defaultComputeDistanceToHighestActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not _SegmentedControlContainerBoxParentData__sliding_segmented_control))
        {
            __child.parentData = new _SegmentedControlContainerBoxParentData__sliding_segmented_control();
        }
    }

    internal virtual double _getMaxChildWidth(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        long childCountLocal = ((checked((long)(this.childCount / 2L))) + 1L);
        double childWidth = (((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth - this.totalSeparatorWidth)) / childCountLocal);
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            childWidth = Math.Max(childWidth, (child.getMaxIntrinsicWidth(double.PositiveInfinity) + (2L * Sliding_segmented_controlLibrary._kSegmentMinPadding)));
            child = nonSeparatorChildAfter(child);
        }
        return Math.Min(childWidth, (((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth - this.totalSeparatorWidth)) / childCountLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getMaxChildHeight(global::Doroti.Framework.Rendering.BoxConstraints constraints, double childWidth)
    {
        double maxHeight = Segmented_controlLibrary._kMinSegmentedControlHeight;
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            double boxHeight = child.getMaxIntrinsicHeight(childWidth);
            maxHeight = Math.Max(maxHeight, boxHeight);
            child = nonSeparatorChildAfter(child);
        }
        return maxHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<double> _getChildWidths(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        if (!this.proportionalWidth)
        {
            double maxChildWidth = _getMaxChildWidth(constraints);
            long segmentCount = ((checked((long)(this.childCount / 2L))) + 1L);
            return new List<double>(System.Linq.Enumerable.Repeat<double>(maxChildWidth, checked((int)segmentCount)));
        }
        var segmentWidths = new List<double>();
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            double childWidth = (child.getMaxIntrinsicWidth(double.PositiveInfinity) + (2L * Sliding_segmented_controlLibrary._kSegmentMinPadding));
            child = nonSeparatorChildAfter(child);
            segmentWidths.Add(childWidth);
        }
        double totalWidth = segmentWidths.sum();
        double allowedMaxWidth = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth - this.totalSeparatorWidth);
        double allowedMinWidth = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth - this.totalSeparatorWidth);
        double scale = (Dart_uiLibrary.clampDouble(totalWidth, allowedMinWidth, allowedMaxWidth) / totalWidth);
        if ((scale != 1L))
        {
            for (var i = 0L; (i < checked((long)(segmentWidths.Count))); i++)
            {
                segmentWidths[(int)(i)] = (segmentWidths[(int)(i)] * scale);
            }
        }
        return segmentWidths;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeOverallSize(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        double maxChildHeight = _getMaxChildHeight(constraints, ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth);
        return ((global::Doroti.Ui.Size)(object?)constraints.constrain(new global::Doroti.Ui.Size((_getChildWidths(constraints).sum() + this.totalSeparatorWidth), maxChildHeight)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        List<double> segmentWidths = ((List<double>)(object?)_getChildWidths(constraints));
        double childHeight = _getMaxChildHeight(constraints, ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth);
        var index = 0L;
        global::Doroti.Framework.Rendering.BaselineOffset baselineOffset = global::Doroti.Framework.Rendering.BaselineOffset.noBaseline;
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childConstraints = global::Doroti.Framework.Rendering.BoxConstraints.CreateTight(new global::Doroti.Ui.Size(segmentWidths[(int)(index)], childHeight));
            baselineOffset = baselineOffset.minOf(new global::Doroti.Framework.Rendering.BaselineOffset(child.getDryBaseline(childConstraints, baseline)));
            child = nonSeparatorChildAfter(child);
            index++;
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeOverallSize(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraintsLocal = this.constraints;
        List<double> segmentWidths = ((List<double>)(object?)_getChildWidths(constraintsLocal));
        double childHeight = _getMaxChildHeight(constraintsLocal, double.PositiveInfinity);
        var separatorConstraints = new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: childHeight, maxHeight: childHeight);
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        var index = 0L;
        double start = 0;
        while ((child is not null))
        {
            var childConstraints = global::Doroti.Framework.Rendering.BoxConstraints.CreateTight(new global::Doroti.Ui.Size(segmentWidths[(int)((checked((long)(index / 2L))))], childHeight));
            child.layout((((checked((long)(index)) & 1L) == 0L) ? childConstraints : separatorConstraints), parentUsesSize: true);
            var childParentData = ((_SegmentedControlContainerBoxParentData__sliding_segmented_control?)(object?)child.parentData!)!;
            var childOffset = new global::Doroti.Ui.Offset(start, 0);
            childParentData.offset = childOffset;
            start += ((global::Doroti.Framework.Rendering.RenderBox)child).size.width;
            DartRuntimePrimitives.Assert(() => (((checked((long)(index)) & 1L) == 0L) || (((global::Doroti.Framework.Rendering.RenderBox)child).size.width == (Sliding_segmented_controlLibrary._kSeparatorWidth + Sliding_segmented_controlLibrary._kSeparatorInset.horizontal))), () => (object?)$"{(((global::Doroti.Framework.Rendering.RenderBox)child).size.width)} != {(Sliding_segmented_controlLibrary._kSeparatorWidth + Sliding_segmented_controlLibrary._kSeparatorInset.horizontal)}");
            child = childAfter(child);
            index += 1L;
        }
        size = _computeOverallSize(constraintsLocal);
    }

    public virtual global::Doroti.Ui.Rect? moveThumbRectInBound(Rect? thumbRect, List<global::Doroti.Framework.Rendering.RenderBox> children)
    {
        DartRuntimePrimitives.Assert(() => this.hasSize);
        DartRuntimePrimitives.Assert(() => (checked((long)(children.Count)) >= 2L));
        if ((thumbRect is null))
        {
            return null;
        }
        global::Doroti.Ui.Offset firstChildOffset = ((global::Doroti.Ui.Offset)(object?)(((_SegmentedControlContainerBoxParentData__sliding_segmented_control?)(object?)children.First().parentData!)!).offset);
        double leftMost = firstChildOffset.dx;
        double rightMost = ((((_SegmentedControlContainerBoxParentData__sliding_segmented_control?)(object?)children.Last().parentData!)!).offset.dx + children.Last().size.width);
        DartRuntimePrimitives.Assert(() => (rightMost > leftMost));
        return global::Doroti.Ui.Rect.fromLTRB(Math.Max(DartRuntimePrimitives.RequireValue(thumbRect).left, (leftMost - ((global::Doroti.Framework.Painting.EdgeInsets)Sliding_segmented_controlLibrary._kThumbInsets).left)), (firstChildOffset.dy - ((global::Doroti.Framework.Painting.EdgeInsets)Sliding_segmented_controlLibrary._kThumbInsets).top), Math.Min(DartRuntimePrimitives.RequireValue(thumbRect).right, (rightMost + ((global::Doroti.Framework.Painting.EdgeInsets)Sliding_segmented_controlLibrary._kThumbInsets).right)), ((firstChildOffset.dy + children.First().size.height) + ((global::Doroti.Framework.Painting.EdgeInsets)Sliding_segmented_controlLibrary._kThumbInsets).bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        List<global::Doroti.Framework.Rendering.RenderBox> children = ((List<global::Doroti.Framework.Rendering.RenderBox>)(object?)getChildrenAsList());
        for (var index = 1L; (index < this.childCount); index += 2L)
        {
            _paintSeparator(context, offset, children[(int)(index)]);
        }
        long? highlightedChildIndex = this.highlightedIndex;
        if ((highlightedChildIndex is not null))
        {
            long highlightedChildIndex__50131__value50234 = DartRuntimePrimitives.RequireValue(highlightedChildIndex);
            global::Doroti.Framework.Rendering.RenderBox selectedChild = children[(int)((DartRuntimePrimitives.RequireValue(highlightedChildIndex__50131__value50234) * 2L))];
            var childParentData = ((_SegmentedControlContainerBoxParentData__sliding_segmented_control?)(object?)selectedChild.parentData!)!;
            global::Doroti.Ui.Rect newThumbRect = ((global::Doroti.Ui.Rect)(object?)Sliding_segmented_controlLibrary._kThumbInsets.inflateRect((childParentData.offset & ((global::Doroti.Framework.Rendering.RenderBox)selectedChild).size)));
            if (((_SegmentedControlState__sliding_segmented_control<T>)this.state).thumbController.isAnimating)
            {
                global::Doroti.Framework.Animation.Animatable<global::Doroti.Ui.Rect?>? thumbTween = ((global::Doroti.Framework.Animation.Animatable<global::Doroti.Ui.Rect?>?)(object?)((_SegmentedControlState__sliding_segmented_control<T>)this.state).thumbAnimatable);
                if ((thumbTween is null))
                {
                    global::Doroti.Ui.Rect startingRect = ((global::Doroti.Ui.Rect)(object?)(moveThumbRectInBound(this.currentThumbRect, children) ?? newThumbRect));
                    this.state.thumbAnimatable = new global::Doroti.Framework.Animation.RectTween(begin: startingRect, end: newThumbRect);
                }
                else
                {
                    if ((!object.Equals(newThumbRect, thumbTween.transform(1))))
                    {
                        global::Doroti.Ui.Rect startingRectLocal = ((global::Doroti.Ui.Rect)(object?)(moveThumbRectInBound(this.currentThumbRect, children) ?? newThumbRect));
                        this.state.thumbAnimatable = new global::Doroti.Framework.Animation.RectTween(begin: startingRectLocal, end: newThumbRect).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(DartRuntimePrimitives.RequireValue(((_SegmentedControlState__sliding_segmented_control<T>)this.state).thumbController.value), 1)));
                    }
                }
            }
            else
            {
                this.state.thumbAnimatable = null;
            }
            global::Doroti.Ui.Rect unscaledThumbRect = ((global::Doroti.Ui.Rect)(object?)(((_SegmentedControlState__sliding_segmented_control<T>)this.state).thumbAnimatable?.evaluate(((_SegmentedControlState__sliding_segmented_control<T>)this.state).thumbController) ?? newThumbRect));
            currentThumbRect = unscaledThumbRect;
            _SegmentLocation__sliding_segmented_control childLocation = default!;
            if ((DartRuntimePrimitives.RequireValue(highlightedChildIndex__50131__value50234) == 0L))
            {
                childLocation = _SegmentLocation__sliding_segmented_control.leftmost;
            }
            else
            {
                if ((DartRuntimePrimitives.RequireValue(highlightedChildIndex__50131__value50234) == (checked((long)(checked((long)(children.Count)) / 2L)))))
                {
                    childLocation = _SegmentLocation__sliding_segmented_control.rightmost;
                }
                else
                {
                    childLocation = _SegmentLocation__sliding_segmented_control.inbetween;
                }
            }
            double delta = (childLocation switch { _SegmentLocation__sliding_segmented_control.leftmost => (unscaledThumbRect.width - (unscaledThumbRect.width * this.thumbScale)), _SegmentLocation__sliding_segmented_control.rightmost => ((unscaledThumbRect.width * this.thumbScale) - unscaledThumbRect.width), _SegmentLocation__sliding_segmented_control.inbetween => 0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            var thumbRect = global::Doroti.Ui.Rect.fromCenter(center: (((Offset)((dynamic)unscaledThumbRect).center) - new global::Doroti.Ui.Offset((delta / 2L), 0)), width: (unscaledThumbRect.width * this.thumbScale), height: (unscaledThumbRect.height * this.thumbScale));
            _paintThumb(context, offset, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(thumbRect)));
        }
        else
        {
            currentThumbRect = null;
        }
        for (var indexLocal = 0L; (indexLocal < checked((long)(children.Count))); indexLocal += 2L)
        {
            _paintChild(context, offset, children[(int)(indexLocal)]);
        }
    }

    internal virtual void _paintSeparator(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox child)
    {
        var childParentData = ((_SegmentedControlContainerBoxParentData__sliding_segmented_control?)(object?)child.parentData!)!;
        context.paintChild(child, (offset + childParentData.offset));
    }

    internal virtual void _paintChild(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox child)
    {
        var childParentData = ((_SegmentedControlContainerBoxParentData__sliding_segmented_control?)(object?)child.parentData!)!;
        context.paintChild(child, (childParentData.offset + offset));
    }

    internal virtual void _paintThumb(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, Rect thumbRect)
    {
        var thumbShadow = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(520093696L), offset: new global::Doroti.Ui.Offset(0, 3), blurRadius: 8), new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(167772160L), offset: new global::Doroti.Ui.Offset(0, 3), blurRadius: 1) };
        var thumbShape = global::Doroti.Ui.RSuperellipse.fromRectAndRadius(DartRuntimePrimitives.RequireValue(thumbRect).shift(offset), Sliding_segmented_controlLibrary._kThumbRadius);
        foreach (var shadow in thumbShadow)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRSuperellipse(thumbShape.shift(shadow.offset), shadow.toPaint());
        }
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRSuperellipse(thumbShape.inflate(0.5), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(167772160L);
    return __cascade;
}))());
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRSuperellipse(thumbShape, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.thumbColor;
    return __cascade;
}))());
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.lastChild;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedControlContainerBoxParentData__sliding_segmented_control?)(object?)child.parentData!)!;
            if (((childParentData.offset & ((global::Doroti.Framework.Rendering.RenderBox)child).size)).contains(position))
            {
                return result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, localOffset) =>
                {
                    DartRuntimePrimitives.Assert(() => (object.Equals(localOffset, (position - childParentData.offset))));
                    return child!.hitTest(result, position: localOffset);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => this._childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderBox))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((ContainerBoxParentData<RenderBox>?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            this._firstChild = child;
            this._lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this._firstChild is not null));
            DartRuntimePrimitives.Assert(() => (this._lastChild is not null));
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: this._firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: this._lastChild));
            var afterParentData = ((ContainerBoxParentData<RenderBox>?)(object?)after.parentData!)!;
            if ((afterParentData.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is ContainerBoxParentData<RenderBox>), () => (object?)$"A child of {this.GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {typeof(ContainerBoxParentData<RenderBox>)}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {typeof(ContainerBoxParentData<RenderBox>)}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((global::System.Action<RenderBox>)this.add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        this._firstChild = null;
        this._lastChild = null;
        this._childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child = this.firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if ((object.Equals(child, this.lastChild)))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if ((result is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy);
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            BaselineOffset candidate = (new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy));
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = this.lastChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((global::System.Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData.offset))));
                return child!.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            if (isHit)
            {
                return true;
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            context.paintChild(child, (childParentData.offset + offset));
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
