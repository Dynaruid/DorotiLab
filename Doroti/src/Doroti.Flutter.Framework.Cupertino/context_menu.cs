// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/context_menu.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public static partial class Context_menuLibrary
{
    internal static double _kOpenScale = 1.15;
}

public static partial class Context_menuLibrary
{
    internal static double _kMinScaleFactor = 1.02;
}

public static partial class Context_menuLibrary
{
    internal static double _previewBorderRadiusRatio = 12.0;
}

public static partial class Context_menuLibrary
{
    internal static Duration _kModalPopupTransitionDuration = Duration.Create(milliseconds: 335L);
}

public static partial class Context_menuLibrary
{
    internal static Duration _previewLongPressTimeout = Duration.Create(milliseconds: 800L);
}

public static partial class Context_menuLibrary
{
    internal static long _animationDuration = (Context_menuLibrary._previewLongPressTimeout.inMilliseconds + Context_menuLibrary._kModalPopupTransitionDuration.inMilliseconds);
}

public static partial class Context_menuLibrary
{
    internal static List<global::Doroti.Generated.Framework.Painting.BoxShadow> _endBoxShadow = new List<global::Doroti.Generated.Framework.Painting.BoxShadow> { new global::Doroti.Generated.Framework.Painting.BoxShadow(color: new global::Doroti.Flutter.Ui.Color(1073741824L), blurRadius: 10.0, spreadRadius: 0.5) };
}

public static partial class Context_menuLibrary
{
    internal static Color _borderColor = ((Color)(object?)new CupertinoDynamicColor(color: new global::Doroti.Flutter.Ui.Color(4289309103L), darkColor: new global::Doroti.Flutter.Ui.Color(4283914330L)));
}

public static partial class Context_menuLibrary
{
    internal static Color _kBackgroundColor = ((Color)(object?)new CupertinoDynamicColor(color: new global::Doroti.Flutter.Ui.Color(4294046193L), darkColor: new global::Doroti.Flutter.Ui.Color(4280361250L)));
}

internal delegate void _DismissCallback__context_menu(global::Doroti.Generated.Framework.Widgets.BuildContext context, double scale, double opacity);

public delegate global::Doroti.Generated.Framework.Widgets.Widget CupertinoContextMenuBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation);

public static partial class Context_menuLibrary
{
    internal static Rect _getRect(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> globalKey)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)globalKey).currentContext is not null));
        var renderBoxContainer__2936 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)globalKey).currentContext!.findRenderObject()!)!;
        return global::Doroti.Flutter.Ui.Rect.fromPoints(((Offset)((dynamic)renderBoxContainer__2936).localToGlobal(((global::Doroti.Generated.Framework.Rendering.RenderBox)renderBoxContainer__2936).paintBounds.topLeft)), ((Offset)((dynamic)renderBoxContainer__2936).localToGlobal(((global::Doroti.Generated.Framework.Rendering.RenderBox)renderBoxContainer__2936).paintBounds.bottomRight)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal enum _ContextMenuLocation__context_menu
{
    center,
    left,
    right
}

public class CupertinoContextMenu : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public static double kOpenBorderRadius = Context_menuLibrary._previewBorderRadiusRatio;
    public static List<global::Doroti.Generated.Framework.Painting.BoxShadow> kEndBoxShadow = Context_menuLibrary._endBoxShadow;
    public static double animationOpensAt = (Context_menuLibrary._previewLongPressTimeout.inMilliseconds / Context_menuLibrary._animationDuration);
    public static Color kBackgroundColor = Context_menuLibrary._kBackgroundColor;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget> builder { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> actions { get; private set; } = default!;
    public virtual bool enableHapticFeedback { get; private set; } = default!;

    public CupertinoContextMenu(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<global::Doroti.Generated.Framework.Widgets.Widget> actions = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!, bool enableHapticFeedback = false) : base(key: key)
    {
        this.actions = actions;
        this.child = child;
        this.enableHapticFeedback = enableHapticFeedback;
        this.builder = (((context, animation) => child));
        System.Diagnostics.Debug.Assert(System.Linq.Enumerable.Any(actions));
    }

    public static CupertinoContextMenu CreateBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<global::Doroti.Generated.Framework.Widgets.Widget> actions = default!, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget> builder = default!, bool enableHapticFeedback = false)
    {
        var __instance = new CupertinoContextMenu(key: key, actions: actions, child: default!, enableHapticFeedback: enableHapticFeedback);
        __instance.actions = actions;
        __instance.builder = builder;
        __instance.enableHapticFeedback = enableHapticFeedback;
        __instance.child = null;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoContextMenuState__context_menu());
}

internal class _CupertinoContextMenuState__context_menu : global::Doroti.Generated.Framework.Widgets.State<CupertinoContextMenu>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<CupertinoContextMenu>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _childGlobalKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual bool _childHidden { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _openController { get; set; } = default!;
    internal virtual Rect? _decoyChildEndRect { get; set; } = default;
    internal virtual double _scaleFactor { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.OverlayEntry? _lastOverlayEntry { get; set; } = default;
    internal virtual _ContextMenuRoute__context_menu<object?>? _route { get; set; } = default;
    internal virtual double _midpoint { get; private set; } = (CupertinoContextMenu.animationOpensAt / 2L);
    internal virtual global::Doroti.Generated.Framework.Gestures.TapGestureRecognizer _tapGestureRecognizer { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _openController = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: Context_menuLibrary._previewLongPressTimeout, vsync: this, upperBound: CupertinoContextMenu.animationOpensAt);
        this._openController.addStatusListener((AnimationStatusListener)this._onDecoyAnimationStatusChange);
        _tapGestureRecognizer = ((Func<global::Doroti.Generated.Framework.Gestures.TapGestureRecognizer>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Gestures.TapGestureRecognizer();
            __cascade.onTapCancel = this._onTapCancel;
            __cascade.onTapDown = this._onTapDown;
            __cascade.onTapUp = this._onTapUp;
            __cascade.onTap = this._onTap;
            return __cascade;        }))();
    }

    internal virtual void _listenerCallback()
    {
        if (((!object.Equals(((global::Doroti.Generated.Framework.Animation.AnimationController)this._openController).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse)) && (((global::Doroti.Generated.Framework.Animation.AnimationController)this._openController).value >= this._midpoint)))
        {
            if (((CupertinoContextMenu)(object)this.widget).enableHapticFeedback)
            {
                DartRuntimePrimitives.Ignore(HapticFeedback.heavyImpact());
            }
            this._tapGestureRecognizer.resolve(global::Doroti.Generated.Framework.Gestures.GestureDisposition.accepted);
            this._openController.removeListener(() => this._listenerCallback());
        }
    }

    internal virtual _ContextMenuLocation__context_menu _contextMenuLocation
    {
        get
        {
            global::Doroti.Flutter.Ui.Rect childRect__16196 = ((global::Doroti.Flutter.Ui.Rect)(object?)Context_menuLibrary._getRect(this._childGlobalKey));
            double screenWidth__16252 = MediaQuery.widthOf(this.context);
            double center__16313 = (screenWidth__16252 / 2L);
            bool centerDividesChild__16354 = ((childRect__16196.left < center__16313) && (childRect__16196.right > center__16313));
            double distanceFromCenter__16445 = ((center__16313 - ((Offset)((dynamic)childRect__16196).center).dx)).abs();
            if ((centerDividesChild__16354 && (distanceFromCenter__16445 <= (childRect__16196.width / 4L))))
            {
                return _ContextMenuLocation__context_menu.center;
            }
            if ((((Offset)((dynamic)childRect__16196).center).dx > center__16313))
            {
                return _ContextMenuLocation__context_menu.right;
            }
            return _ContextMenuLocation__context_menu.left;
            return default!;
        }
    }
    internal static double _getScaleFactor(Rect childRect, global::Doroti.Generated.Framework.Painting.EdgeInsets padding, Size size)
    {
        double leftMaxScale__17018 = ((2L * ((((Offset)((dynamic)childRect).center).dx - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding).left))) / childRect.width);
        double topMaxScale__17110 = ((2L * ((((Offset)((dynamic)childRect).center).dy - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding).top))) / childRect.height);
        double rightMaxScale__17201 = ((2L * (((size.width - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding).right) - ((Offset)((dynamic)childRect).center).dx))) / childRect.width);
        double bottomMaxScale__17316 = ((2L * (((size.height - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding).bottom) - ((Offset)((dynamic)childRect).center).dy))) / childRect.height);
        double minWidth__17435 = Math.Min(leftMaxScale__17018, rightMaxScale__17201);
        double minHeight__17502 = Math.Min(topMaxScale__17110, bottomMaxScale__17316);
        return Dart_uiLibrary.clampDouble(Math.Min(minWidth__17435, minHeight__17502), Context_menuLibrary._kMinScaleFactor, Context_menuLibrary._kOpenScale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Generated.Framework.Widgets.Widget _defaultPreviewBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.FittedBox(fit: global::Doroti.Generated.Framework.Painting.BoxFit.cover, child: new global::Doroti.Generated.Framework.Widgets.ClipRSuperellipse(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateCircular((Context_menuLibrary._previewBorderRadiusRatio * ((global::Doroti.Generated.Framework.Animation.Animation<double>)animation).value)), child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _openContextMenu()
    {
        setState(((global::System.Action)(() => {
_childHidden = true;
})));
        _route = new _ContextMenuRoute__context_menu<object?>(actions: ((CupertinoContextMenu)(object)this.widget).actions, barrierLabel: CupertinoLocalizations.of(this.context).menuDismissLabel, filter: new global::Doroti.Flutter.Ui.ImageFilter(sigmaX: 5.0, sigmaY: 5.0), contextMenuLocation: this._contextMenuLocation, previousChildRect: DartRuntimePrimitives.RequireValue(this._decoyChildEndRect), scaleFactor: this._scaleFactor, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget>?)((context, animation) => {
if ((((CupertinoContextMenu)(object)this.widget).child is null))
{
    global::Doroti.Generated.Framework.Animation.Animation<double> localAnimation__18889 = ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: CupertinoContextMenu.animationOpensAt, end: 1).animate(animation));
    return this.widget.builder(context, localAnimation__18889);
}
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)_CupertinoContextMenuState__context_menu._defaultPreviewBuilder(context, animation, ((CupertinoContextMenu)(object)this.widget).child!));
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        DartRuntimePrimitives.Ignore(Navigator.of(this.context, rootNavigator: true).push<object?>(this._route!));
        this._route!.animation!.addStatusListener((AnimationStatusListener)this._routeAnimationStatusListener);
    }

    internal virtual void _removeContextMenuDecoy()
    {
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) => {
if (this.mounted)
{
    _closeContextMenu();
    this._openController.reset();
}
})), debugLabel: "removeContextMenuDecoy");
    }

    internal virtual void _closeContextMenu()
    {
        this._lastOverlayEntry?.remove();
        this._lastOverlayEntry?.dispose();
        _lastOverlayEntry = null;
    }

    internal virtual void _onDecoyAnimationStatusChange(global::Doroti.Generated.Framework.Animation.AnimationStatus animationStatus)
    {
        switch (animationStatus)
        {
            case global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed:
                {
                    if ((this._route is null))
                    {
                        setState(((global::System.Action)(() => {
_childHidden = false;
})));
                    }
                    _closeContextMenu();
                    break;
                }
            case global::Doroti.Generated.Framework.Animation.AnimationStatus.completed:
                {
                    _openContextMenu();
                    _removeContextMenuDecoy();
                    break;
                }
            case global::Doroti.Generated.Framework.Animation.AnimationStatus.forward:
            case global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse:
                {
                    if (!((bool)((dynamic)global::Doroti.Generated.Framework.Widgets.ModalRoute<object>.of<object>(this.context)!).isCurrent))
                    {
                        _removeContextMenuDecoy();
                    }
                    return;
                }
        }
    }

    internal virtual void _routeAnimationStatusListener(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
    {
        if (!global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isDismissed(status))
        {
            return;
        }
        if (this.mounted)
        {
            setState(((global::System.Action)(() => {
_childHidden = false;
})));
        }
        this._route!.animation!.removeStatusListener((AnimationStatusListener)this._routeAnimationStatusListener);
        _route = null;
    }

    internal virtual void _onTapCompleted()
    {
        this._openController.removeListener(() => this._listenerCallback());
        if ((((global::Doroti.Generated.Framework.Animation.AnimationController)this._openController).isAnimating && (((global::Doroti.Generated.Framework.Animation.AnimationController)this._openController).value < this._midpoint)))
        {
            this._openController.reverse();
        }
    }

    internal virtual void _onTap()
    {
        _onTapCompleted();
    }

    internal virtual void _onTapCancel()
    {
        _onTapCompleted();
    }

    internal virtual void _onTapUp(global::Doroti.Generated.Framework.Gestures.TapUpDetails details)
    {
        _onTapCompleted();
    }

    internal virtual void _onTapDown(global::Doroti.Generated.Framework.Gestures.TapDownDetails details)
    {
        this._openController.addListener(() => this._listenerCallback());
        setState(((global::System.Action)(() => {
_childHidden = true;
})));
        global::Doroti.Flutter.Ui.Rect childRect__21496 = ((global::Doroti.Flutter.Ui.Rect)(object?)Context_menuLibrary._getRect(this._childGlobalKey));
        _scaleFactor = _CupertinoContextMenuState__context_menu._getScaleFactor(childRect__21496, MediaQuery.paddingOf(this.context), MediaQuery.sizeOf(this.context));
        _decoyChildEndRect = global::Doroti.Flutter.Ui.Rect.fromCenter(center: ((Offset)((dynamic)childRect__21496).center), width: (childRect__21496.width * this._scaleFactor), height: (childRect__21496.height * this._scaleFactor));
        _lastOverlayEntry = new global::Doroti.Generated.Framework.Widgets.OverlayEntry(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((context) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _DecoyChild__context_menu(beginRect: childRect__21496, controller: this._openController, endRect: this._decoyChildEndRect, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget>)((CupertinoContextMenu)(object)this.widget).builder, child: ((CupertinoContextMenu)(object)this.widget).child));
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        Overlay.of(this.context, rootOverlay: true, debugRequiredFor: this.widget).insert(this._lastOverlayEntry!);
        this._openController.forward();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.MouseRegion(cursor: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb ? global::Doroti.Generated.Framework.Services.SystemMouseCursors.click : global::Doroti.Generated.Framework.Services.MouseCursor.defer), child: new global::Doroti.Generated.Framework.Widgets.Listener(onPointerDown: this._tapGestureRecognizer.addPointer, child: new global::Doroti.Generated.Framework.Widgets.TickerMode(enabled: !this._childHidden, child: new global::Doroti.Generated.Framework.Widgets.Visibility(key: this._childGlobalKey, visible: !this._childHidden, child: this.widget.builder(context, this._openController))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _closeContextMenu();
        this._tapGestureRecognizer.dispose();
        this._openController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _DecoyChild__context_menu : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual Rect? beginRect { get; private set; }
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController controller { get; private set; } = default!;
    public virtual Rect? endRect { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget>? builder { get; private set; }

    internal _DecoyChild__context_menu(Rect? beginRect = null, global::Doroti.Generated.Framework.Animation.AnimationController controller = default!, Rect? endRect = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget>? builder = null)
    {
        this.beginRect = beginRect;
        this.controller = controller;
        this.endRect = endRect;
        this.child = child;
        this.builder = builder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DecoyChildState__context_menu());
}

internal class _DecoyChildState__context_menu : global::Doroti.Generated.Framework.Widgets.State<_DecoyChild__context_menu>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<_DecoyChild__context_menu>
{
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<Rect?> _rect { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<global::Doroti.Generated.Framework.Painting.Decoration> _boxDecoration { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _boxDecorationCurvedAnimation { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        var beginPause__24596 = 1.0;
        var openAnimationLength__24624 = 5.0;
        double totalOpenAnimationLength__24668 = (beginPause__24596 + openAnimationLength__24624);
        double endPause__24746 = (((((totalOpenAnimationLength__24668 * Context_menuLibrary._animationDuration)) / Context_menuLibrary._previewLongPressTimeout.inMilliseconds)) - totalOpenAnimationLength__24668);
        _rect = new global::Doroti.Generated.Framework.Animation.TweenSequence<global::Doroti.Flutter.Ui.Rect?>(new List<global::Doroti.Generated.Framework.Animation.TweenSequenceItem<global::Doroti.Flutter.Ui.Rect?>> { new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<global::Doroti.Flutter.Ui.Rect?>(tween: new global::Doroti.Generated.Framework.Animation.RectTween(begin: ((_DecoyChild__context_menu)(object)this.widget).beginRect, end: ((_DecoyChild__context_menu)(object)this.widget).beginRect).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: global::Doroti.Generated.Framework.Animation.Curves.linear)), weight: beginPause__24596), new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<global::Doroti.Flutter.Ui.Rect?>(tween: new global::Doroti.Generated.Framework.Animation.RectTween(begin: ((_DecoyChild__context_menu)(object)this.widget).beginRect, end: ((_DecoyChild__context_menu)(object)this.widget).endRect).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: global::Doroti.Generated.Framework.Animation.Curves.easeOutSine)), weight: openAnimationLength__24624), new global::Doroti.Generated.Framework.Animation.TweenSequenceItem<global::Doroti.Flutter.Ui.Rect?>(tween: new global::Doroti.Generated.Framework.Animation.RectTween(begin: ((_DecoyChild__context_menu)(object)this.widget).endRect, end: ((_DecoyChild__context_menu)(object)this.widget).endRect).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: global::Doroti.Generated.Framework.Animation.Curves.linear)), weight: endPause__24746) }.Cast<global::Doroti.Generated.Framework.Animation.TweenSequenceItem<global::Doroti.Flutter.Ui.Rect?>>().ToList()).animate(((_DecoyChild__context_menu)(object)this.widget).controller);
        _boxDecorationCurvedAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((_DecoyChild__context_menu)(object)this.widget).controller, curve: new global::Doroti.Generated.Framework.Animation.Interval(0.0, CupertinoContextMenu.animationOpensAt));
        _boxDecoration = new global::Doroti.Generated.Framework.Widgets.DecorationTween(begin: new global::Doroti.Generated.Framework.Painting.BoxDecoration(boxShadow: new List<global::Doroti.Generated.Framework.Painting.BoxShadow>()), end: new global::Doroti.Generated.Framework.Painting.BoxDecoration(boxShadow: Context_menuLibrary._endBoxShadow)).animate(this._boxDecorationCurvedAnimation);
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildAnimation(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.Positioned.CreateFromRect(rect: DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Animation.Animation<Rect?>)this._rect).value), child: new global::Doroti.Generated.Framework.Widgets.Container(decoration: ((global::Doroti.Generated.Framework.Animation.Animation<global::Doroti.Generated.Framework.Painting.Decoration>)this._boxDecoration).value, child: ((_DecoyChild__context_menu)(object)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.Positioned.CreateFromRect(rect: DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Animation.Animation<Rect?>)this._rect).value), child: ((_DecoyChild__context_menu)(object)this.widget).builder!(context, ((_DecoyChild__context_menu)(object)this.widget).controller)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._boxDecorationCurvedAnimation.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Stack(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((((_DecoyChild__context_menu)(object)this.widget).child is not null) ? this._buildAnimation : this._buildBuilder)), animation: ((_DecoyChild__context_menu)(object)this.widget).controller)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _ContextMenuRoute__context_menu<T> : global::Doroti.Generated.Framework.Widgets.PopupRoute<T>
{
    internal static Color _kModalBarrierColor = new global::Doroti.Flutter.Ui.Color(1711539215L);
    internal virtual List<global::Doroti.Generated.Framework.Widgets.Widget> _actions { get; private set; } = default!;
    internal virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget>? _builder { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _childGlobalKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual _ContextMenuLocation__context_menu _contextMenuLocation { get; private set; } = default!;
    internal virtual bool _externalOffstage { get; set; } = false;
    internal virtual bool _internalOffstage { get; set; } = false;
    internal virtual double _scaleFactor { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.Orientation? _lastOrientation { get; set; } = default;
    internal virtual Rect _previousChildRect { get; private set; } = default!;
    internal virtual double? _scale { get; set; } = 1.0;
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _sheetGlobalKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal static global::Doroti.Generated.Framework.Animation.CurveTween _curve = new global::Doroti.Generated.Framework.Animation.CurveTween(curve: global::Doroti.Generated.Framework.Animation.Curves.easeOutBack);
    internal static global::Doroti.Generated.Framework.Animation.CurveTween _curveReverse = new global::Doroti.Generated.Framework.Animation.CurveTween(curve: global::Doroti.Generated.Framework.Animation.Curves.easeInBack);
    internal static global::Doroti.Generated.Framework.Animation.RectTween _rectTween = new global::Doroti.Generated.Framework.Animation.RectTween();
    internal static global::Doroti.Generated.Framework.Animation.Animatable<Rect?> _rectAnimatable = _rectTween.chain(_curve);
    internal static global::Doroti.Generated.Framework.Animation.RectTween _rectTweenReverse = new global::Doroti.Generated.Framework.Animation.RectTween();
    internal static global::Doroti.Generated.Framework.Animation.Animatable<Rect?> _rectAnimatableReverse = _rectTweenReverse.chain(_curveReverse);
    internal static global::Doroti.Generated.Framework.Animation.RectTween _sheetRectTween = new global::Doroti.Generated.Framework.Animation.RectTween();
    internal virtual global::Doroti.Generated.Framework.Animation.Animatable<Rect?> _sheetRectAnimatable { get; private set; } = _sheetRectTween.chain(_curve);
    internal virtual global::Doroti.Generated.Framework.Animation.Animatable<Rect?> _sheetRectAnimatableReverse { get; private set; } = _sheetRectTween.chain(_curveReverse);
    internal static global::Doroti.Generated.Framework.Animation.Tween<double> _sheetScaleTween = new global::Doroti.Generated.Framework.Animation.Tween<double>();
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _sheetScaleAnimatable = _sheetScaleTween.chain(_curve);
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _sheetScaleAnimatableReverse = _sheetScaleTween.chain(_curveReverse);
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<double> _opacityTween { get; private set; } = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0);
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _sheetOpacity { get; set; } = default!;
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel { get => __field_barrierLabel; }
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation? _curvedAnimation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation? _sheetOpacityCurvedAnimation { get; set; } = default;

    internal _ContextMenuRoute__context_menu(List<global::Doroti.Generated.Framework.Widgets.Widget> actions, _ContextMenuLocation__context_menu contextMenuLocation, string? barrierLabel = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget>? builder = null, ImageFilter? filter = null, Rect previousChildRect = default!, double scaleFactor = default!, global::Doroti.Generated.Framework.Widgets.RouteSettings? settings = null) : base(filter: filter, settings: settings)
    {
        this.__field_barrierLabel = barrierLabel;
        this._actions = actions;
        this._builder = builder;
        this._contextMenuLocation = contextMenuLocation;
        this._previousChildRect = previousChildRect;
        this._scaleFactor = scaleFactor;
        System.Diagnostics.Debug.Assert(System.Linq.Enumerable.Any(actions));
    }

    public override Color? barrierColor => _kModalBarrierColor;
    public override bool barrierDismissible => true;
    public override bool semanticsDismissible => false;
    public override Duration transitionDuration => Context_menuLibrary._kModalPopupTransitionDuration;
    internal static global::Doroti.Flutter.Ui.Rect _getScaledRect(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> globalKey, double scale)
    {
        global::Doroti.Flutter.Ui.Rect childRect__30288 = ((global::Doroti.Flutter.Ui.Rect)(object?)Context_menuLibrary._getRect(globalKey));
        global::Doroti.Flutter.Ui.Size sizeScaled__30336 = ((global::Doroti.Flutter.Ui.Size)(object?)(childRect__30288.size * scale));
        var offsetScaled__30383 = new global::Doroti.Flutter.Ui.Offset((childRect__30288.left + (((childRect__30288.size.width - sizeScaled__30336.width)) / 2L)), (childRect__30288.top + (((childRect__30288.size.height - sizeScaled__30336.height)) / 2L)));
        return (offsetScaled__30383 & sizeScaled__30336);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Painting.AlignmentDirectional getSheetAlignment(_ContextMenuLocation__context_menu contextMenuLocation, global::Doroti.Generated.Framework.Widgets.Orientation orientation)
    {
        return (contextMenuLocation switch { _ContextMenuLocation__context_menu.center when ((object.Equals(DartRuntimePrimitives.RequireValue(orientation), global::Doroti.Generated.Framework.Widgets.Orientation.landscape))) => global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topStart, _ContextMenuLocation__context_menu.center => global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topCenter, _ContextMenuLocation__context_menu.right => global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topEnd, _ContextMenuLocation__context_menu.left => global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topStart, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Flutter.Ui.Rect _getSheetRectBegin(global::Doroti.Generated.Framework.Widgets.Orientation? orientation, _ContextMenuLocation__context_menu contextMenuLocation, Rect childRect, Rect sheetRect)
    {
        switch (contextMenuLocation)
        {
            case _ContextMenuLocation__context_menu.center:
                {
                    global::Doroti.Flutter.Ui.Offset target__31527 = ((global::Doroti.Flutter.Ui.Offset)(object?)((object.Equals(orientation, global::Doroti.Generated.Framework.Widgets.Orientation.portrait)) ? childRect.bottomCenter : childRect.topCenter));
                    global::Doroti.Flutter.Ui.Offset centered__31665 = ((global::Doroti.Flutter.Ui.Offset)(object?)(target__31527 - new global::Doroti.Flutter.Ui.Offset((sheetRect.width / 2L), 0.0)));
                    return (centered__31665 & sheetRect.size);
                }
            case _ContextMenuLocation__context_menu.right:
                {
                    global::Doroti.Flutter.Ui.Offset target__31821 = ((global::Doroti.Flutter.Ui.Offset)(object?)((object.Equals(orientation, global::Doroti.Generated.Framework.Widgets.Orientation.portrait)) ? childRect.bottomRight : childRect.topRight));
                    return (((target__31821 - new global::Doroti.Flutter.Ui.Offset(sheetRect.width, 0.0))) & sheetRect.size);
                }
            case _ContextMenuLocation__context_menu.left:
                {
                    global::Doroti.Flutter.Ui.Offset target__32068 = ((global::Doroti.Flutter.Ui.Offset)(object?)((object.Equals(orientation, global::Doroti.Generated.Framework.Widgets.Orientation.portrait)) ? childRect.bottomLeft : childRect.topLeft));
                    return (target__32068 & sheetRect.size);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onDismiss(global::Doroti.Generated.Framework.Widgets.BuildContext context, double scale, double opacity)
    {
        _scale = scale;
        this._opacityTween.end = opacity;
        _sheetOpacityCurvedAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this.animation!, curve: new global::Doroti.Generated.Framework.Animation.Interval(0.9, 1.0));
        _sheetOpacity = this._opacityTween.animate(this._sheetOpacityCurvedAnimation!);
        Navigator.of(context).pop<object>();
    }

    internal virtual void _updateTweenRects()
    {
        global::Doroti.Flutter.Ui.Rect childRect__32742 = ((global::Doroti.Flutter.Ui.Rect)(object?)((this._scale is null) ? Context_menuLibrary._getRect(this._childGlobalKey) : _ContextMenuRoute__context_menu<T>._getScaledRect(this._childGlobalKey, DartRuntimePrimitives.RequireValue(this._scale))));
        _rectTween.begin = this._previousChildRect;
        _rectTween.end = childRect__32742;
        var childRectOriginal__33153 = global::Doroti.Flutter.Ui.Rect.fromCenter(center: ((Offset)((dynamic)this._previousChildRect).center), width: (this._previousChildRect.width / this._scaleFactor), height: (this._previousChildRect.height / this._scaleFactor));
        global::Doroti.Flutter.Ui.Rect sheetRect__33364 = ((global::Doroti.Flutter.Ui.Rect)(object?)Context_menuLibrary._getRect(this._sheetGlobalKey));
        global::Doroti.Flutter.Ui.Rect sheetRectBegin__33418 = ((global::Doroti.Flutter.Ui.Rect)(object?)_ContextMenuRoute__context_menu<T>._getSheetRectBegin(this._lastOrientation, this._contextMenuLocation, childRectOriginal__33153, sheetRect__33364));
        _sheetRectTween.begin = sheetRectBegin__33418;
        _sheetRectTween.end = sheetRect__33364;
        _sheetScaleTween.begin = 0.0;
        _sheetScaleTween.end = DartRuntimePrimitives.RequireValue(this._scale);
        _rectTweenReverse.begin = childRectOriginal__33153;
        _rectTweenReverse.end = childRect__32742;
    }

    internal virtual void _setOffstageInternally()
    {
        base.offstage = (this._externalOffstage || this._internalOffstage);
        changedInternalState();
    }

    public virtual bool didPop(T? result)
    {
        _updateTweenRects();
        return base.didPop(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool offstage
    {
        set
        {
            var __value = value;
            _externalOffstage = __value;
            _setOffstageInternally();
        }
    }
    public override global::Doroti.Generated.Framework.Scheduler.TickerFuture didPush()
    {
        _internalOffstage = true;
        _setOffstageInternally();
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) => {
_updateTweenRects();
_internalOffstage = false;
_setOffstageInternally();
})), debugLabel: "renderContextMenuRouteOffstage");
        return ((global::Doroti.Generated.Framework.Scheduler.TickerFuture)(object?)base.didPush());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Animation.Animation<double> createAnimation()
    {
        global::Doroti.Generated.Framework.Animation.Animation<double> animation__34786 = ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)base.createAnimation());
        if ((!object.Equals(this._curvedAnimation?.parent, animation__34786)))
        {
            this._curvedAnimation?.dispose();
            _curvedAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: animation__34786, curve: global::Doroti.Generated.Framework.Animation.Curves.linear);
        }
        _sheetOpacity = this._opacityTween.animate(this._curvedAnimation!);
        return animation__34786;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildPage(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildTransitions(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.OrientationBuilder(builder: ((context, orientation) => {
_lastOrientation = DartRuntimePrimitives.RequireValue(orientation);
if (!((global::Doroti.Generated.Framework.Animation.Animation<double>)animation).isCompleted)
{
    var reverse__36097 = (object.Equals(((global::Doroti.Generated.Framework.Animation.Animation<double>)animation).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse));
    global::Doroti.Flutter.Ui.Rect rect__36173 = ((global::Doroti.Flutter.Ui.Rect)(object?)(reverse__36097 ? DartRuntimePrimitives.RequireValue(_rectAnimatableReverse.evaluate(animation)) : DartRuntimePrimitives.RequireValue(_rectAnimatable.evaluate(animation))));
    global::Doroti.Flutter.Ui.Rect sheetRect__36323 = ((global::Doroti.Flutter.Ui.Rect)(object?)(reverse__36097 ? DartRuntimePrimitives.RequireValue(this._sheetRectAnimatableReverse.evaluate(animation)) : DartRuntimePrimitives.RequireValue(this._sheetRectAnimatable.evaluate(animation))));
    double sheetScale__36490 = (reverse__36097 ? _sheetScaleAnimatableReverse.evaluate(animation) : _sheetScaleAnimatable.evaluate(animation));
    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.Stack.Create(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(global::Doroti.Generated.Framework.Widgets.Positioned.CreateFromRect(rect: sheetRect__36323, child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this._sheetOpacity, child: global::Doroti.Generated.Framework.Widgets.Transform.CreateScale(alignment: _ContextMenuRoute__context_menu<T>.getSheetAlignment(this._contextMenuLocation, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(orientation))), scale: sheetScale__36490, child: new _ContextMenuSheet__context_menu(key: this._sheetGlobalKey, actions: this._actions, contextMenuLocation: this._contextMenuLocation, orientation: DartRuntimePrimitives.RequireValue(orientation)))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(global::Doroti.Generated.Framework.Widgets.Positioned.CreateFromRect(key: this._childGlobalKey, rect: rect__36173, child: this._builder!(context, animation))) }));
}
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ContextMenuRouteStatic__context_menu(actions: this._actions, childGlobalKey: this._childGlobalKey, contextMenuLocation: this._contextMenuLocation, onDismiss: this._onDismiss, orientation: DartRuntimePrimitives.RequireValue(orientation), sheetGlobalKey: this._sheetGlobalKey, childRect: this._previousChildRect, child: this._builder!(context, animation)));
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._curvedAnimation?.dispose();
        this._sheetOpacityCurvedAnimation?.dispose();
        base.dispose();
    }

}

internal class _ContextMenuRouteStatic__context_menu : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget>? actions { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>? childGlobalKey { get; private set; }
    public virtual _ContextMenuLocation__context_menu contextMenuLocation { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Generated.Framework.Widgets.BuildContext, double, double>? onDismiss { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Orientation orientation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>? sheetGlobalKey { get; private set; }
    public virtual Rect childRect { get; private set; } = default!;

    internal _ContextMenuRouteStatic__context_menu(List<global::Doroti.Generated.Framework.Widgets.Widget>? actions = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>? childGlobalKey = null, _ContextMenuLocation__context_menu contextMenuLocation = default!, global::System.Action<global::Doroti.Generated.Framework.Widgets.BuildContext, double, double>? onDismiss = null, global::Doroti.Generated.Framework.Widgets.Orientation orientation = default!, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>? sheetGlobalKey = null, Rect childRect = default!)
    {
        this.actions = actions;
        this.child = child;
        this.childGlobalKey = childGlobalKey;
        this.contextMenuLocation = contextMenuLocation;
        this.onDismiss = onDismiss;
        this.orientation = orientation;
        this.sheetGlobalKey = sheetGlobalKey;
        this.childRect = childRect;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ContextMenuRouteStaticState__context_menu());
}

internal class _ContextMenuRouteStaticState__context_menu : global::Doroti.Generated.Framework.Widgets.State<_ContextMenuRouteStatic__context_menu>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<_ContextMenuRouteStatic__context_menu>
{
    internal const double _kMinScale = 0.8;
    internal const double _kSheetScaleThreshold = 0.9;
    internal const double _kPadding = 20.0;
    internal const double _kDamping = 400.0;
    internal static Duration _kMoveControllerDuration = Duration.Create(milliseconds: 600L);
    internal virtual Offset _dragOffset { get; set; } = default!;
    internal virtual double _lastScale { get; set; } = 1.0;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _moveController { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _moveCurvedAnimation { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _sheetController { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _sheetCurvedAnimation { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<Offset> _moveAnimation { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _sheetScaleAnimation { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _sheetOpacityAnimation { get; set; } = default!;
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal static double _getScale(global::Doroti.Generated.Framework.Widgets.Orientation orientation, double maxDragDistance, double dy)
    {
        double dyDirectional__40051 = ((dy <= 0.0) ? dy : -dy);
        return Math.Max(_kMinScale, (((maxDragDistance + dyDirectional__40051)) / maxDragDistance));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onPanStart(global::Doroti.Generated.Framework.Gestures.DragStartDetails details)
    {
        this._moveController.value = 1.0;
        _setDragOffset(Offset.zero);
    }

    internal virtual void _onPanUpdate(global::Doroti.Generated.Framework.Gestures.DragUpdateDetails details)
    {
        _setDragOffset((this._dragOffset + ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).delta));
    }

    internal virtual void _onPanEnd(global::Doroti.Generated.Framework.Gestures.DragEndDetails details)
    {
        if ((((global::Doroti.Generated.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dy.abs() >= global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity))
        {
            bool flingIsAway__40605 = (((global::Doroti.Generated.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dy > 0L);
            double finalPosition__40679 = (flingIsAway__40605 ? (((global::Doroti.Generated.Framework.Animation.Animation<Offset>)this._moveAnimation).value.dy + 100.0) : 0.0);
            if ((flingIsAway__40605 && (!object.Equals(((global::Doroti.Generated.Framework.Animation.AnimationController)this._sheetController).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward))))
            {
                this._sheetController.forward();
            }
            else
            {
                if ((!flingIsAway__40605 && (!object.Equals(((global::Doroti.Generated.Framework.Animation.AnimationController)this._sheetController).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse))))
                {
                    this._sheetController.reverse();
                }
            }
            _moveAnimation = new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Flutter.Ui.Offset>(begin: new global::Doroti.Flutter.Ui.Offset(0.0, ((global::Doroti.Generated.Framework.Animation.Animation<Offset>)this._moveAnimation).value.dy), end: new global::Doroti.Flutter.Ui.Offset(0.0, finalPosition__40679)).animate(this._moveController);
            this._moveController.reset();
            this._moveController.duration = Duration.Create(milliseconds: 64L);
            this._moveController.forward();
            this._moveController.addStatusListener((AnimationStatusListener)this._flingStatusListener);
            return;
        }
        if ((this._lastScale == _kMinScale))
        {
            ((_ContextMenuRouteStatic__context_menu)(object)this.widget).onDismiss!(this.context, this._lastScale, ((global::Doroti.Generated.Framework.Animation.Animation<double>)this._sheetOpacityAnimation).value);
            return;
        }
        this._moveController.addListener(() => this._moveListener());
        this._moveController.reverse();
    }

    internal virtual void _moveListener()
    {
        if ((this._lastScale > _kSheetScaleThreshold))
        {
            this._moveController.removeListener(() => this._moveListener());
            if (!this._sheetController.isDismissed)
            {
                this._sheetController.reverse();
            }
        }
    }

    internal virtual void _flingStatusListener(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
    {
        if (!global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isCompleted(status))
        {
            return;
        }
        this._moveController.duration = _kMoveControllerDuration;
        this._moveController.removeStatusListener((AnimationStatusListener)this._flingStatusListener);
        if ((((global::Doroti.Generated.Framework.Animation.Animation<Offset>)this._moveAnimation).value.dy == 0.0))
        {
            return;
        }
        ((_ContextMenuRouteStatic__context_menu)(object)this.widget).onDismiss!(this.context, this._lastScale, ((global::Doroti.Generated.Framework.Animation.Animation<double>)this._sheetOpacityAnimation).value);
    }

    internal virtual void _setDragOffset(Offset dragOffset)
    {
        double endX__42643 = ((SliderLibrary._kPadding * dragOffset.dx) / _kDamping);
        double endY__42706 = ((dragOffset.dy >= 0.0) ? dragOffset.dy : ((SliderLibrary._kPadding * dragOffset.dy) / _kDamping));
        setState(((global::System.Action)(() => {
_dragOffset = dragOffset;
_moveAnimation = new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Flutter.Ui.Offset>(begin: Offset.zero, end: new global::Doroti.Flutter.Ui.Offset(Dart_uiLibrary.clampDouble(endX__42643, -SliderLibrary._kPadding, SliderLibrary._kPadding), endY__42706)).animate(this._moveCurvedAnimation);
if ((((this._lastScale <= _kSheetScaleThreshold) && (!object.Equals(((global::Doroti.Generated.Framework.Animation.AnimationController)this._sheetController).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward))) && (((global::Doroti.Generated.Framework.Animation.Animation<double>)this._sheetScaleAnimation).value != 0.0)))
{
    this._sheetController.forward();
}
else
{
    if ((((this._lastScale > _kSheetScaleThreshold) && (!object.Equals(((global::Doroti.Generated.Framework.Animation.AnimationController)this._sheetController).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse))) && (((global::Doroti.Generated.Framework.Animation.Animation<double>)this._sheetScaleAnimation).value != 1.0)))
    {
        this._sheetController.reverse();
    }
}
})));
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _getChild(global::Doroti.Generated.Framework.Widgets.Orientation orientation, _ContextMenuLocation__context_menu contextMenuLocation)
    {
        global::Doroti.Flutter.Ui.Size screenSize__43796 = ((global::Doroti.Flutter.Ui.Size)(object?)MediaQuery.sizeOf(this.context));
        global::Doroti.Generated.Framework.Painting.EdgeInsets padding__43858 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)MediaQuery.paddingOf(this.context));
        var screenBounds__43909 = global::Doroti.Flutter.Ui.Rect.fromLTWH(0, 0, ((screenSize__43796.width - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__43858).left) - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__43858).right), ((screenSize__43796.height - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__43858).top) - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__43858).bottom));
        global::Doroti.Generated.Framework.Widgets.Widget sheet__44093 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: this._sheetController, builder: this._buildSheetAnimation, child: new _ContextMenuSheet__context_menu(key: ((_ContextMenuRouteStatic__context_menu)(object)this.widget).sheetGlobalKey, actions: ((_ContextMenuRouteStatic__context_menu)(object)this.widget).actions!, contextMenuLocation: ((_ContextMenuRouteStatic__context_menu)(object)this.widget).contextMenuLocation, orientation: ((_ContextMenuRouteStatic__context_menu)(object)this.widget).orientation)));
        global::Doroti.Generated.Framework.Widgets.Widget child__44423 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ContextMenuAlignedChildren__context_menu(targetRect: ((_ContextMenuRouteStatic__context_menu)(object)this.widget).childRect, screenBounds: screenBounds__43909, sheet: sheet__44093, contextMenuLocation: contextMenuLocation, orientation: ((_ContextMenuRouteStatic__context_menu)(object)this.widget).orientation, child: new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: this._moveController, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildChildAnimation, child: ((_ContextMenuRouteStatic__context_menu)(object)this.widget).child)));
        return child__44423;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildSheetAnimation(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.Transform.CreateScale(alignment: _ContextMenuRoute__context_menu<object>.getSheetAlignment(((_ContextMenuRouteStatic__context_menu)(object)this.widget).contextMenuLocation, ((_ContextMenuRouteStatic__context_menu)(object)this.widget).orientation), scale: ((global::Doroti.Generated.Framework.Animation.Animation<double>)this._sheetScaleAnimation).value, child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this._sheetOpacityAnimation, child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildChildAnimation(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        _lastScale = _ContextMenuRouteStaticState__context_menu._getScale(((_ContextMenuRouteStatic__context_menu)(object)this.widget).orientation, MediaQuery.heightOf(context), ((global::Doroti.Generated.Framework.Animation.Animation<Offset>)this._moveAnimation).value.dy);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.Transform.CreateScale(key: ((_ContextMenuRouteStatic__context_menu)(object)this.widget).childGlobalKey, scale: this._lastScale, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildAnimation(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.Transform.CreateTranslate(offset: ((global::Doroti.Generated.Framework.Animation.Animation<Offset>)this._moveAnimation).value, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        base.initState();
        _moveController = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: _kMoveControllerDuration, value: 1.0, vsync: this);
        _moveCurvedAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._moveController, curve: global::Doroti.Generated.Framework.Animation.Curves.elasticIn);
        _sheetController = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: Duration.Create(milliseconds: 100L), reverseDuration: Duration.Create(milliseconds: 300L), vsync: this);
        _sheetCurvedAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._sheetController, curve: global::Doroti.Generated.Framework.Animation.Curves.linear, reverseCurve: global::Doroti.Generated.Framework.Animation.Curves.easeInBack);
        _sheetScaleAnimation = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).animate(this._sheetCurvedAnimation);
        _sheetOpacityAnimation = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).animate(this._sheetController);
        _setDragOffset(Offset.zero);
    }

    public override void dispose()
    {
        this._moveController.dispose();
        this._moveCurvedAnimation.dispose();
        this._sheetController.dispose();
        this._sheetCurvedAnimation.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.Widget child__46859 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)_getChild(((_ContextMenuRouteStatic__context_menu)(object)this.widget).orientation, ((_ContextMenuRouteStatic__context_menu)(object)this.widget).contextMenuLocation));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SafeArea(child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.Alignment.topLeft, child: new global::Doroti.Generated.Framework.Widgets.GestureDetector(onPanEnd: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragEndDetails>)this._onPanEnd, onPanStart: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragStartDetails>)this._onPanStart, onPanUpdate: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails>)this._onPanUpdate, child: new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: this._moveController, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildAnimation, child: child__46859)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _ContextMenuSheet__context_menu : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> actions { get; private set; } = default!;
    public virtual _ContextMenuLocation__context_menu contextMenuLocation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Orientation orientation { get; private set; } = default!;

    internal _ContextMenuSheet__context_menu(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<global::Doroti.Generated.Framework.Widgets.Widget> actions = default!, _ContextMenuLocation__context_menu contextMenuLocation = default!, global::Doroti.Generated.Framework.Widgets.Orientation orientation = default!) : base(key: key)
    {
        this.actions = actions;
        this.contextMenuLocation = contextMenuLocation;
        this.orientation = orientation;
        System.Diagnostics.Debug.Assert(System.Linq.Enumerable.Any(actions));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ContextMenuSheetState__context_menu());
}

internal class _ContextMenuSheetState__context_menu : global::Doroti.Generated.Framework.Widgets.State<_ContextMenuSheet__context_menu>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.ScrollController _controller { get; private set; } = default!;
    internal const double _kMenuWidth = 250.0;
    internal const double _kScrollbarMainAxisMargin = 13.0;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Generated.Framework.Widgets.ScrollController();
    }

    public override void dispose()
    {
        this._controller.dispose();
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(width: _kMenuWidth, child: new global::Doroti.Generated.Framework.Widgets.IntrinsicHeight(child: new global::Doroti.Generated.Framework.Widgets.ClipRSuperellipse(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(13.0)), child: new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: CupertinoDynamicColor.resolve(CupertinoContextMenu.kBackgroundColor, context), child: new global::Doroti.Generated.Framework.Widgets.ScrollConfiguration(behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false), child: new CupertinoScrollbar(mainAxisMargin: _kScrollbarMainAxisMargin, controller: this._controller, child: new global::Doroti.Generated.Framework.Widgets.SingleChildScrollView(controller: this._controller, child: new global::Doroti.Generated.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection49409 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection49409.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(((_ContextMenuSheet__context_menu)(object)this.widget).actions.First())); foreach (var action__49503 in ((_ContextMenuSheet__context_menu)(object)this.widget).actions.skip(1L)) { __collection49409.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Generated.Framework.Painting.BoxDecoration(border: new global::Doroti.Generated.Framework.Painting.Border(top: new global::Doroti.Generated.Framework.Painting.BorderSide(color: CupertinoDynamicColor.resolve(Context_menuLibrary._borderColor, context), width: 0.4))), position: global::Doroti.Generated.Framework.Rendering.DecorationPosition.foreground, child: action__49503))); } return __collection49409; }))())))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal enum _ContextMenuChild__context_menu
{
    child,
    menuSheet
}

internal class _ContextMenuAlignedChildren__context_menu : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual Rect targetRect { get; private set; } = default!;
    public virtual Rect screenBounds { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget sheet { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Orientation orientation { get; private set; } = default!;
    public virtual _ContextMenuLocation__context_menu contextMenuLocation { get; private set; } = default!;

    internal _ContextMenuAlignedChildren__context_menu(Rect targetRect, Rect screenBounds, global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Widgets.Widget sheet, global::Doroti.Generated.Framework.Widgets.Orientation orientation, _ContextMenuLocation__context_menu contextMenuLocation)
    {
        this.targetRect = targetRect;
        this.screenBounds = screenBounds;
        this.child = child;
        this.sheet = sheet;
        this.orientation = orientation;
        this.contextMenuLocation = contextMenuLocation;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.CustomMultiChildLayout(@delegate: new _ContextMenuAlignedChildrenDelegate__context_menu(targetRect: this.targetRect, screenBounds: this.screenBounds, orientation: this.orientation, contextMenuLocation: this.contextMenuLocation), children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.LayoutId(id: _ContextMenuChild__context_menu.child, child: this.child)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.LayoutId(id: _ContextMenuChild__context_menu.menuSheet, child: this.sheet)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ContextMenuAlignedChildrenDelegate__context_menu : global::Doroti.Generated.Framework.Rendering.MultiChildLayoutDelegate
{
    public virtual Rect targetRect { get; private set; } = default!;
    public virtual Rect screenBounds { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Orientation orientation { get; private set; } = default!;
    public virtual _ContextMenuLocation__context_menu contextMenuLocation { get; private set; } = default!;

    internal _ContextMenuAlignedChildrenDelegate__context_menu(Rect targetRect, Rect screenBounds, global::Doroti.Generated.Framework.Widgets.Orientation orientation, _ContextMenuLocation__context_menu contextMenuLocation)
    {
        this.targetRect = targetRect;
        this.screenBounds = screenBounds;
        this.orientation = orientation;
        this.contextMenuLocation = contextMenuLocation;
    }

    public override void performLayout(Size size)
    {
        var constraints__51659 = global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateLoose(size);
        double availableHeightForChild__51719 = (this.screenBounds.height - _ContextMenuRouteStaticState__context_menu._kPadding);
        double availableWidth__51832 = (this.screenBounds.width - (_ContextMenuRouteStaticState__context_menu._kPadding * 2L));
        double availableWidthForChild__51931 = (this.orientation switch { global::Doroti.Generated.Framework.Widgets.Orientation.portrait => availableWidth__51832, global::Doroti.Generated.Framework.Widgets.Orientation.landscape => (availableWidth__51832 - _ContextMenuSheetState__context_menu._kMenuWidth), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        DartRuntimePrimitives.Assert(() => (availableWidthForChild__51931 >= 0.0));
        DartRuntimePrimitives.Assert(() => (availableHeightForChild__51719 >= 0.0));
        global::Doroti.Flutter.Ui.Size childSize__52219 = ((global::Doroti.Flutter.Ui.Size)(object?)layoutChild(_ContextMenuChild__context_menu.child, constraints__51659.copyWith(maxHeight: availableHeightForChild__51719, maxWidth: availableWidthForChild__51931)));
        double availableHeightForMenu__52527 = (this.orientation switch { global::Doroti.Generated.Framework.Widgets.Orientation.portrait => (availableHeightForChild__51719 - ((childSize__52219.height + _ContextMenuRouteStaticState__context_menu._kPadding))), global::Doroti.Generated.Framework.Widgets.Orientation.landscape => availableHeightForChild__51719, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Flutter.Ui.Size menuSize__52779 = ((global::Doroti.Flutter.Ui.Size)(object?)layoutChild(_ContextMenuChild__context_menu.menuSheet, constraints__51659.copyWith(maxHeight: availableHeightForMenu__52527)));
        double initialChildLeft__52926 = default!;
        double initialChildTop__52961 = default!;
        double maxClampedLeft__52995 = default!;
        double maxClampedTop__53028 = default!;
        global::Doroti.Flutter.Ui.Offset secondChildOffset__53060 = default!;
        bool menuBeforeChild__53094 = default!;
        switch (this.orientation)
        {
            case global::Doroti.Generated.Framework.Widgets.Orientation.portrait:
                {
                    menuBeforeChild__53094 = false;
                    double totalHeight__53225 = ((childSize__52219.height + menuSize__52779.height) + _ContextMenuRouteStaticState__context_menu._kPadding);
                    double totalWidth__53349 = (childSize__52219.width + _ContextMenuRouteStaticState__context_menu._kPadding);
                    initialChildLeft__52926 = (((Offset)((dynamic)this.targetRect).center).dx - (childSize__52219.width / 2L));
                    initialChildTop__52961 = (((Offset)((dynamic)this.targetRect).center).dy - childSize__52219.height);
                    double secondChildDx__53579 = (this.contextMenuLocation switch { _ContextMenuLocation__context_menu.center => ((childSize__52219.width / 2L) - (menuSize__52779.width / 2L)), _ContextMenuLocation__context_menu.left => 0.0, _ContextMenuLocation__context_menu.right => (childSize__52219.width - menuSize__52779.width), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                    secondChildOffset__53060 = new global::Doroti.Flutter.Ui.Offset(secondChildDx__53579, (childSize__52219.height + _ContextMenuRouteStaticState__context_menu._kPadding));
                    maxClampedLeft__52995 = (this.screenBounds.right - totalWidth__53349);
                    maxClampedTop__53028 = (this.screenBounds.bottom - totalHeight__53225);
                    break;
                }
            case global::Doroti.Generated.Framework.Widgets.Orientation.landscape:
                {
                    menuBeforeChild__53094 = (object.Equals(this.contextMenuLocation, _ContextMenuLocation__context_menu.right));
                    double totalWidth__54228 = ((childSize__52219.width + menuSize__52779.width) + _ContextMenuRouteStaticState__context_menu._kPadding);
                    initialChildLeft__52926 = (((Offset)((dynamic)this.screenBounds).center).dx - (totalWidth__54228 / 2L));
                    initialChildTop__52961 = (((Offset)((dynamic)this.screenBounds).center).dy - (Math.Max(childSize__52219.height, menuSize__52779.height) / 2L));
                    double secondChildDx__54517 = (menuBeforeChild__53094 ? menuSize__52779.width : childSize__52219.width);
                    secondChildOffset__53060 = new global::Doroti.Flutter.Ui.Offset((secondChildDx__54517 + _ContextMenuRouteStaticState__context_menu._kPadding), 0.0);
                    maxClampedLeft__52995 = (this.screenBounds.right - totalWidth__54228);
                    maxClampedTop__53028 = this.screenBounds.bottom;
                    break;
                }
        }
        double clampedLeft__54876 = Dart_uiLibrary.clampDouble(initialChildLeft__52926, (this.screenBounds.left + _ContextMenuRouteStaticState__context_menu._kPadding), maxClampedLeft__52995);
        double clampedTop__55039 = Dart_uiLibrary.clampDouble(initialChildTop__52961, (this.screenBounds.top + _ContextMenuRouteStaticState__context_menu._kPadding), maxClampedTop__53028);
        var firstPosition__55191 = new global::Doroti.Flutter.Ui.Offset(clampedLeft__54876, clampedTop__55039);
        global::Doroti.Flutter.Ui.Offset secondPosition__55257 = ((global::Doroti.Flutter.Ui.Offset)(object?)(firstPosition__55191 + secondChildOffset__53060));
        positionChild(_ContextMenuChild__context_menu.child, (menuBeforeChild__53094 ? secondPosition__55257 : firstPosition__55191));
        positionChild(_ContextMenuChild__context_menu.menuSheet, (menuBeforeChild__53094 ? firstPosition__55191 : secondPosition__55257));
    }

    public override bool shouldRelayout(global::Doroti.Generated.Framework.Rendering.MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_ContextMenuAlignedChildrenDelegate__context_menu)(object)oldDelegate;
        return ((((!object.Equals(((_ContextMenuAlignedChildrenDelegate__context_menu)__oldDelegate).targetRect, this.targetRect)) || (!object.Equals(((_ContextMenuAlignedChildrenDelegate__context_menu)__oldDelegate).screenBounds, this.screenBounds))) || (!object.Equals(((_ContextMenuAlignedChildrenDelegate__context_menu)__oldDelegate).orientation, this.orientation))) || (!object.Equals(((_ContextMenuAlignedChildrenDelegate__context_menu)__oldDelegate).contextMenuLocation, this.contextMenuLocation)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
