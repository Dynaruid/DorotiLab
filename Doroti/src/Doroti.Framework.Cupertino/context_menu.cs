// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/context_menu.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

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
    internal static long _animationDuration = _previewLongPressTimeout.inMilliseconds + _kModalPopupTransitionDuration.inMilliseconds;
}

public static partial class Context_menuLibrary
{
    internal static List<global::Doroti.Framework.Painting.BoxShadow> _endBoxShadow = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(1073741824L), blurRadius: 10.0, spreadRadius: 0.5) };
}

public static partial class Context_menuLibrary
{
    internal static Color _borderColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(4289309103L), darkColor: new global::Doroti.Ui.Color(4283914330L));
}

public static partial class Context_menuLibrary
{
    internal static Color _kBackgroundColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(4294046193L), darkColor: new global::Doroti.Ui.Color(4280361250L));
}

internal delegate void _DismissCallback__context_menu(global::Doroti.Framework.Widgets.BuildContext context, double scale, double opacity);

public delegate global::Doroti.Framework.Widgets.Widget CupertinoContextMenuBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation);

public static partial class Context_menuLibrary
{
    internal static Rect _getRect(global::Doroti.Framework.Widgets.GlobalKey<IState> globalKey)
    {
        DartRuntimePrimitives.Assert(() => globalKey.currentContext is not null);
        var renderBoxContainer = ((global::Doroti.Framework.Rendering.RenderBox?)globalKey.currentContext!.findRenderObject()!)!;
        return Rect.fromPoints(renderBoxContainer.localToGlobal(renderBoxContainer.paintBounds.topLeft), renderBoxContainer.localToGlobal(renderBoxContainer.paintBounds.bottomRight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal enum _ContextMenuLocation__context_menu
{
    center,
    left,
    right
}

public class CupertinoContextMenu : global::Doroti.Framework.Widgets.StatefulWidget
{
    public static double kOpenBorderRadius = Context_menuLibrary._previewBorderRadiusRatio;
    public static List<global::Doroti.Framework.Painting.BoxShadow> kEndBoxShadow = Context_menuLibrary._endBoxShadow;
    public static double animationOpensAt = Context_menuLibrary._previewLongPressTimeout.inMilliseconds / Context_menuLibrary._animationDuration;
    public static Color kBackgroundColor = Context_menuLibrary._kBackgroundColor;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget> actions { get; private set; } = default!;
    public virtual bool enableHapticFeedback { get; private set; } = default!;

    public CupertinoContextMenu(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget> actions = default!, global::Doroti.Framework.Widgets.Widget child = default!, bool enableHapticFeedback = false) : base(key: key)
    {
        this.actions = actions;
        this.child = child;
        this.enableHapticFeedback = enableHapticFeedback;
        builder = (context, animation) => child;
        System.Diagnostics.Debug.Assert(Enumerable.Any(actions));
    }

    public static CupertinoContextMenu CreateBuilder(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget> actions = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget> builder = default!, bool enableHapticFeedback = false)
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

internal class _CupertinoContextMenuState__context_menu : global::Doroti.Framework.Widgets.State<CupertinoContextMenu>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<CupertinoContextMenu>
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _childGlobalKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual bool _childHidden { get; set; } = false;
    internal virtual global::Doroti.Framework.Animation.AnimationController _openController { get; set; } = default!;
    internal virtual Rect? _decoyChildEndRect { get; set; } = default;
    internal virtual double _scaleFactor { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.OverlayEntry? _lastOverlayEntry { get; set; } = default;
    internal virtual _ContextMenuRoute__context_menu<object?>? _route { get; set; } = default;
    internal virtual double _midpoint { get; private set; } = CupertinoContextMenu.animationOpensAt / 2L;
    internal virtual global::Doroti.Framework.Gestures.TapGestureRecognizer _tapGestureRecognizer { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _openController = new global::Doroti.Framework.Animation.AnimationController(duration: Context_menuLibrary._previewLongPressTimeout, vsync: this, upperBound: CupertinoContextMenu.animationOpensAt);
        _openController.addStatusListener(_onDecoyAnimationStatusChange);
        _tapGestureRecognizer = ((Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() =>
{
    var __cascade = new global::Doroti.Framework.Gestures.TapGestureRecognizer();
    __cascade.onTapCancel = _onTapCancel;
    __cascade.onTapDown = _onTapDown;
    __cascade.onTapUp = _onTapUp;
    __cascade.onTap = _onTap;
    return __cascade;
}))();
    }

    internal virtual void _listenerCallback()
    {
        if ((!Equals(_openController.status, AnimationStatus.reverse)) && (_openController.value >= _midpoint))
        {
            if (widget.enableHapticFeedback)
            {
                DartRuntimePrimitives.Ignore(HapticFeedback.heavyImpact());
            }
            _tapGestureRecognizer.resolve(Gestures.GestureDisposition.accepted);
            _openController.removeListener(_listenerCallback);
        }
    }

    internal virtual _ContextMenuLocation__context_menu _contextMenuLocation
    {
        get
        {
            global::Doroti.Ui.Rect childRect = Context_menuLibrary._getRect(_childGlobalKey);
            double screenWidth = MediaQuery.widthOf(context);
            double centerLocal = screenWidth / 2L;
            bool centerDividesChild = (childRect.left < centerLocal) && (childRect.right > centerLocal);
            double distanceFromCenter = (centerLocal - childRect.center.dx).abs();
            if (centerDividesChild && (distanceFromCenter <= (childRect.width / 4L)))
            {
                return _ContextMenuLocation__context_menu.center;
            }
            if (childRect.center.dx > centerLocal)
            {
                return _ContextMenuLocation__context_menu.right;
            }
            return _ContextMenuLocation__context_menu.left;
        }
    }
    internal static double _getScaleFactor(Rect childRect, global::Doroti.Framework.Painting.EdgeInsets padding, Size size)
    {
        double leftMaxScale = 2L * (childRect.center.dx - padding.left) / childRect.width;
        double topMaxScale = 2L * (childRect.center.dy - padding.top) / childRect.height;
        double rightMaxScale = 2L * (size.width - padding.right - childRect.center.dx) / childRect.width;
        double bottomMaxScale = 2L * (size.height - padding.bottom - childRect.center.dy) / childRect.height;
        double minWidth = Math.Min(leftMaxScale, rightMaxScale);
        double minHeight = Math.Min(topMaxScale, bottomMaxScale);
        return Dart_uiLibrary.clampDouble(Math.Min(minWidth, minHeight), Context_menuLibrary._kMinScaleFactor, Context_menuLibrary._kOpenScale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Widgets.Widget _defaultPreviewBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Widgets.Widget child)
    {
        return new global::Doroti.Framework.Widgets.FittedBox(fit: BoxFit.cover, child: new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: BorderRadius.CreateCircular(Context_menuLibrary._previewBorderRadiusRatio * animation.value), child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _openContextMenu()
    {
        setState(() =>
        {
            _childHidden = true;
        });
        _route = new _ContextMenuRoute__context_menu<object?>(actions: widget.actions, barrierLabel: CupertinoLocalizations.of(context).menuDismissLabel, filter: new global::Doroti.Ui.ImageFilter(sigmaX: 5.0, sigmaY: 5.0), contextMenuLocation: _contextMenuLocation, previousChildRect: DartRuntimePrimitives.RequireValue(_decoyChildEndRect), scaleFactor: _scaleFactor, builder: (context, animation) =>
        {
            if (widget.child is null)
            {
                global::Doroti.Framework.Animation.Animation<double> localAnimation = new global::Doroti.Framework.Animation.Tween<double>(begin: CupertinoContextMenu.animationOpensAt, end: 1).animate(animation);
                return widget.builder(context, localAnimation);
            }
            return _defaultPreviewBuilder(context, animation, widget.child!);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        DartRuntimePrimitives.Ignore(Navigator.of(context, rootNavigator: true).push<object?>(_route!));
        _route!.animation!.addStatusListener(_routeAnimationStatusListener);
    }

    internal virtual void _removeContextMenuDecoy()
    {
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
        {
            if (mounted)
            {
                _closeContextMenu();
                _openController.reset();
            }
        }, debugLabel: "removeContextMenuDecoy");
    }

    internal virtual void _closeContextMenu()
    {
        _lastOverlayEntry?.remove();
        _lastOverlayEntry?.dispose();
        _lastOverlayEntry = null;
    }

    internal virtual void _onDecoyAnimationStatusChange(global::Doroti.Framework.Animation.AnimationStatus animationStatus)
    {
        switch (animationStatus)
        {
            case AnimationStatus.dismissed:
                {
                    if (_route is null)
                    {
                        setState(() =>
                        {
                            _childHidden = false;
                        });
                    }
                    _closeContextMenu();
                    break;
                }
            case AnimationStatus.completed:
                {
                    _openContextMenu();
                    _removeContextMenuDecoy();
                    break;
                }
            case AnimationStatus.forward:
            case AnimationStatus.reverse:
                {
                    if (!ModalRoute<object>.untypedOf(context)!.isCurrent)
                    {
                        _removeContextMenuDecoy();
                    }
                    return;
                }
        }
    }

    internal virtual void _routeAnimationStatusListener(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (!AnimationStatusMembers.isDismissed(status))
        {
            return;
        }
        if (mounted)
        {
            setState(() =>
            {
                _childHidden = false;
            });
        }
        _route!.animation!.removeStatusListener(_routeAnimationStatusListener);
        _route = null;
    }

    internal virtual void _onTapCompleted()
    {
        _openController.removeListener(_listenerCallback);
        if (_openController.isAnimating && (_openController.value < _midpoint))
        {
            _openController.reverse();
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

    internal virtual void _onTapUp(global::Doroti.Framework.Gestures.TapUpDetails details)
    {
        _onTapCompleted();
    }

    internal virtual void _onTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        _openController.addListener(_listenerCallback);
        setState(() =>
        {
            _childHidden = true;
        });
        global::Doroti.Ui.Rect childRect = Context_menuLibrary._getRect(_childGlobalKey);
        _scaleFactor = _getScaleFactor(childRect, MediaQuery.paddingOf(context), MediaQuery.sizeOf(context));
        _decoyChildEndRect = Rect.fromCenter(center: childRect.center, width: childRect.width * _scaleFactor, height: childRect.height * _scaleFactor);
        _lastOverlayEntry = new global::Doroti.Framework.Widgets.OverlayEntry(builder: (context) =>
        {
            return new _DecoyChild__context_menu(beginRect: childRect, controller: _openController, endRect: _decoyChildEndRect, builder: widget.builder, child: widget.child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        Overlay.of(context, rootOverlay: true, debugRequiredFor: widget).insert(_lastOverlayEntry!);
        _openController.forward();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.MouseRegion(cursor: Foundation.ConstantsLibrary.kIsWeb ? SystemMouseCursors.click : MouseCursor.defer, child: new global::Doroti.Framework.Widgets.Listener(onPointerDown: _tapGestureRecognizer.addPointer, child: new global::Doroti.Framework.Widgets.TickerMode(enabled: !_childHidden, child: new global::Doroti.Framework.Widgets.Visibility(key: _childGlobalKey, visible: !_childHidden, child: widget.builder(context, _openController)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _closeContextMenu();
        _tapGestureRecognizer.dispose();
        _openController.dispose();
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

internal class _DecoyChild__context_menu : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual Rect? beginRect { get; private set; }
    public virtual global::Doroti.Framework.Animation.AnimationController controller { get; private set; } = default!;
    public virtual Rect? endRect { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>? builder { get; private set; }

    internal _DecoyChild__context_menu(Rect? beginRect = null, global::Doroti.Framework.Animation.AnimationController controller = default!, Rect? endRect = null, global::Doroti.Framework.Widgets.Widget? child = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>? builder = null)
    {
        this.beginRect = beginRect;
        this.controller = controller;
        this.endRect = endRect;
        this.child = child;
        this.builder = builder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DecoyChildState__context_menu());
}

internal class _DecoyChildState__context_menu : global::Doroti.Framework.Widgets.State<_DecoyChild__context_menu>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<_DecoyChild__context_menu>
{
    internal virtual global::Doroti.Framework.Animation.Animation<Rect?> _rect { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.Decoration> _boxDecoration { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _boxDecorationCurvedAnimation { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        var beginPause = 1.0;
        var openAnimationLength = 5.0;
        double totalOpenAnimationLength = beginPause + openAnimationLength;
        double endPause = totalOpenAnimationLength * Context_menuLibrary._animationDuration / Context_menuLibrary._previewLongPressTimeout.inMilliseconds - totalOpenAnimationLength;
        _rect = new global::Doroti.Framework.Animation.TweenSequence<global::Doroti.Ui.Rect?>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Rect?>> { new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Rect?>(tween: new global::Doroti.Framework.Animation.RectTween(begin: widget.beginRect, end: widget.beginRect).chain(new global::Doroti.Framework.Animation.CurveTween(curve: Curves.linear)), weight: beginPause), new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Rect?>(tween: new global::Doroti.Framework.Animation.RectTween(begin: widget.beginRect, end: widget.endRect).chain(new global::Doroti.Framework.Animation.CurveTween(curve: Curves.easeOutSine)), weight: openAnimationLength), new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Rect?>(tween: new global::Doroti.Framework.Animation.RectTween(begin: widget.endRect, end: widget.endRect).chain(new global::Doroti.Framework.Animation.CurveTween(curve: Curves.linear)), weight: endPause) }.Cast<global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Rect?>>().ToList()).animate(widget.controller);
        _boxDecorationCurvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.controller, curve: new global::Doroti.Framework.Animation.Interval(0.0, CupertinoContextMenu.animationOpensAt));
        _boxDecoration = new global::Doroti.Framework.Widgets.DecorationTween(begin: new global::Doroti.Framework.Painting.BoxDecoration(boxShadow: new List<global::Doroti.Framework.Painting.BoxShadow>()), end: new global::Doroti.Framework.Painting.BoxDecoration(boxShadow: Context_menuLibrary._endBoxShadow)).animate(_boxDecorationCurvedAnimation);
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildAnimation(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget? child)
    {
        return Positioned.CreateFromRect(rect: DartRuntimePrimitives.RequireValue(_rect.value), child: new global::Doroti.Framework.Widgets.Container(decoration: _boxDecoration.value, child: widget.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget? child)
    {
        return Positioned.CreateFromRect(rect: DartRuntimePrimitives.RequireValue(_rect.value), child: widget.builder!(context, widget.controller));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _boxDecorationCurvedAnimation.dispose();
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedBuilder(builder: (widget.child is not null) ? _buildAnimation : _buildBuilder, animation: widget.controller)) });
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

internal class _ContextMenuRoute__context_menu<T> : global::Doroti.Framework.Widgets.PopupRoute<T>
{
    internal static Color _kModalBarrierColor = new global::Doroti.Ui.Color(1711539215L);
    internal virtual List<global::Doroti.Framework.Widgets.Widget> _actions { get; private set; } = default!;
    internal virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>? _builder { get; private set; }
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _childGlobalKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual _ContextMenuLocation__context_menu _contextMenuLocation { get; private set; } = default!;
    internal virtual bool _externalOffstage { get; set; } = false;
    internal virtual bool _internalOffstage { get; set; } = false;
    internal virtual double _scaleFactor { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.Orientation? _lastOrientation { get; set; } = default;
    internal virtual Rect _previousChildRect { get; private set; } = default!;
    internal virtual double? _scale { get; set; } = 1.0;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _sheetGlobalKey { get; private set; } = GlobalKey<IState>.Create();
    internal static global::Doroti.Framework.Animation.CurveTween _curve = new global::Doroti.Framework.Animation.CurveTween(curve: Curves.easeOutBack);
    internal static global::Doroti.Framework.Animation.CurveTween _curveReverse = new global::Doroti.Framework.Animation.CurveTween(curve: Curves.easeInBack);
    internal static global::Doroti.Framework.Animation.RectTween _rectTween = new global::Doroti.Framework.Animation.RectTween();
    internal static global::Doroti.Framework.Animation.Animatable<Rect?> _rectAnimatable = _rectTween.chain(_curve);
    internal static global::Doroti.Framework.Animation.RectTween _rectTweenReverse = new global::Doroti.Framework.Animation.RectTween();
    internal static global::Doroti.Framework.Animation.Animatable<Rect?> _rectAnimatableReverse = _rectTweenReverse.chain(_curveReverse);
    internal static global::Doroti.Framework.Animation.RectTween _sheetRectTween = new global::Doroti.Framework.Animation.RectTween();
    internal virtual global::Doroti.Framework.Animation.Animatable<Rect?> _sheetRectAnimatable { get; private set; } = _sheetRectTween.chain(_curve);
    internal virtual global::Doroti.Framework.Animation.Animatable<Rect?> _sheetRectAnimatableReverse { get; private set; } = _sheetRectTween.chain(_curveReverse);
    internal static global::Doroti.Framework.Animation.Tween<double> _sheetScaleTween = new global::Doroti.Framework.Animation.Tween<double>();
    internal static global::Doroti.Framework.Animation.Animatable<double> _sheetScaleAnimatable = _sheetScaleTween.chain(_curve);
    internal static global::Doroti.Framework.Animation.Animatable<double> _sheetScaleAnimatableReverse = _sheetScaleTween.chain(_curveReverse);
    internal virtual global::Doroti.Framework.Animation.Tween<double> _opacityTween { get; private set; } = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0);
    internal virtual global::Doroti.Framework.Animation.Animation<double> _sheetOpacity { get; set; } = default!;
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel { get => __field_barrierLabel; }
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _curvedAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _sheetOpacityCurvedAnimation { get; set; } = default;

    internal _ContextMenuRoute__context_menu(List<global::Doroti.Framework.Widgets.Widget> actions, _ContextMenuLocation__context_menu contextMenuLocation, string? barrierLabel = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>? builder = null, ImageFilter? filter = null, Rect previousChildRect = default!, double scaleFactor = default!, global::Doroti.Framework.Widgets.RouteSettings? settings = null) : base(filter: filter, settings: settings)
    {
        __field_barrierLabel = barrierLabel;
        _actions = actions;
        _builder = builder;
        _contextMenuLocation = contextMenuLocation;
        _previousChildRect = previousChildRect;
        _scaleFactor = scaleFactor;
        System.Diagnostics.Debug.Assert(Enumerable.Any(actions));
    }

    public override Color? barrierColor => _kModalBarrierColor;
    public override bool barrierDismissible => true;
    public override bool semanticsDismissible => false;
    public override Duration transitionDuration => Context_menuLibrary._kModalPopupTransitionDuration;
    internal static global::Doroti.Ui.Rect _getScaledRect(global::Doroti.Framework.Widgets.GlobalKey<IState> globalKey, double scale)
    {
        global::Doroti.Ui.Rect childRect = Context_menuLibrary._getRect(globalKey);
        global::Doroti.Ui.Size sizeScaled = childRect.size * scale;
        var offsetScaled = new global::Doroti.Ui.Offset(childRect.left + ((childRect.size.width - sizeScaled.width) / 2L), childRect.top + ((childRect.size.height - sizeScaled.height) / 2L));
        return offsetScaled & sizeScaled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Painting.AlignmentDirectional getSheetAlignment(_ContextMenuLocation__context_menu contextMenuLocation, global::Doroti.Framework.Widgets.Orientation orientation)
    {
        return contextMenuLocation switch { _ContextMenuLocation__context_menu.center when Equals(DartRuntimePrimitives.RequireValue(orientation), Orientation.landscape) => AlignmentDirectional.topStart, _ContextMenuLocation__context_menu.center => AlignmentDirectional.topCenter, _ContextMenuLocation__context_menu.right => AlignmentDirectional.topEnd, _ContextMenuLocation__context_menu.left => AlignmentDirectional.topStart, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Rect _getSheetRectBegin(global::Doroti.Framework.Widgets.Orientation? orientation, _ContextMenuLocation__context_menu contextMenuLocation, Rect childRect, Rect sheetRect)
    {
        switch (contextMenuLocation)
        {
            case _ContextMenuLocation__context_menu.center:
                {
                    global::Doroti.Ui.Offset target = Equals(orientation, Orientation.portrait) ? childRect.bottomCenter : childRect.topCenter;
                    global::Doroti.Ui.Offset centered = target - new global::Doroti.Ui.Offset(sheetRect.width / 2L, 0.0);
                    return centered & sheetRect.size;
                }
            case _ContextMenuLocation__context_menu.right:
                {
                    global::Doroti.Ui.Offset targetLocal = Equals(orientation, Orientation.portrait) ? childRect.bottomRight : childRect.topRight;
                    return targetLocal - new global::Doroti.Ui.Offset(sheetRect.width, 0.0) & sheetRect.size;
                }
            case _ContextMenuLocation__context_menu.left:
                {
                    global::Doroti.Ui.Offset targetAlternate = Equals(orientation, Orientation.portrait) ? childRect.bottomLeft : childRect.topLeft;
                    return targetAlternate & sheetRect.size;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onDismiss(global::Doroti.Framework.Widgets.BuildContext context, double scale, double opacity)
    {
        _scale = scale;
        _opacityTween.end = opacity;
        _sheetOpacityCurvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: animation!, curve: new global::Doroti.Framework.Animation.Interval(0.9, 1.0));
        _sheetOpacity = _opacityTween.animate(_sheetOpacityCurvedAnimation!);
        Navigator.of(context).pop<object>();
    }

    internal virtual void _updateTweenRects()
    {
        global::Doroti.Ui.Rect childRect = (_scale is null) ? Context_menuLibrary._getRect(_childGlobalKey) : _ContextMenuRoute__context_menu<T>._getScaledRect(_childGlobalKey, DartRuntimePrimitives.RequireValue(_scale));
        _rectTween.begin = _previousChildRect;
        _rectTween.end = childRect;
        var childRectOriginal = Rect.fromCenter(center: _previousChildRect.center, width: _previousChildRect.width / _scaleFactor, height: _previousChildRect.height / _scaleFactor);
        global::Doroti.Ui.Rect sheetRect = Context_menuLibrary._getRect(_sheetGlobalKey);
        global::Doroti.Ui.Rect sheetRectBegin = _ContextMenuRoute__context_menu<T>._getSheetRectBegin(_lastOrientation, _contextMenuLocation, childRectOriginal, sheetRect);
        _sheetRectTween.begin = sheetRectBegin;
        _sheetRectTween.end = sheetRect;
        _sheetScaleTween.begin = 0.0;
        _sheetScaleTween.end = DartRuntimePrimitives.RequireValue(_scale);
        _rectTweenReverse.begin = childRectOriginal;
        _rectTweenReverse.end = childRect;
    }

    internal virtual void _setOffstageInternally()
    {
        base.offstage = _externalOffstage || _internalOffstage;
        changedInternalState();
    }

    public override bool didPop(T? result)
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
    public override global::Doroti.Framework.Scheduler.TickerFuture didPush()
    {
        _internalOffstage = true;
        _setOffstageInternally();
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
        {
            _updateTweenRects();
            _internalOffstage = false;
            _setOffstageInternally();
        }, debugLabel: "renderContextMenuRouteOffstage");
        return base.didPush();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Animation.Animation<double> createAnimation()
    {
        global::Doroti.Framework.Animation.Animation<double> animation = base.createAnimation();
        if (!Equals(_curvedAnimation?.parent, animation))
        {
            _curvedAnimation?.dispose();
            _curvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: animation, curve: Curves.linear);
        }
        _sheetOpacity = _opacityTween.animate(_curvedAnimation!);
        return animation;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        return SizedBox.CreateShrink();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return new global::Doroti.Framework.Widgets.OrientationBuilder(builder: (context, orientation) =>
        {
            _lastOrientation = DartRuntimePrimitives.RequireValue(orientation);
            if (!animation.isCompleted)
            {
                var reverseLocal = Equals(animation.status, AnimationStatus.reverse);
                global::Doroti.Ui.Rect rectLocal = reverseLocal ? DartRuntimePrimitives.RequireValue(_rectAnimatableReverse.evaluate(animation)) : DartRuntimePrimitives.RequireValue(_rectAnimatable.evaluate(animation));
                global::Doroti.Ui.Rect sheetRect = reverseLocal ? DartRuntimePrimitives.RequireValue(_sheetRectAnimatableReverse.evaluate(animation)) : DartRuntimePrimitives.RequireValue(_sheetRectAnimatable.evaluate(animation));
                double sheetScale = reverseLocal ? _sheetScaleAnimatableReverse.evaluate(animation) : _sheetScaleAnimatable.evaluate(animation);
                return Stack.Create(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(Positioned.CreateFromRect(rect: sheetRect, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: _sheetOpacity, child: Transform.CreateScale(alignment: _ContextMenuRoute__context_menu<T>.getSheetAlignment(_contextMenuLocation, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(orientation))), scale: sheetScale, child: new _ContextMenuSheet__context_menu(key: _sheetGlobalKey, actions: _actions, contextMenuLocation: _contextMenuLocation, orientation: DartRuntimePrimitives.RequireValue(orientation)))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(Positioned.CreateFromRect(key: _childGlobalKey, rect: rectLocal, child: _builder!(context, animation))) });
            }
            return new _ContextMenuRouteStatic__context_menu(actions: _actions, childGlobalKey: _childGlobalKey, contextMenuLocation: _contextMenuLocation, onDismiss: _onDismiss, orientation: DartRuntimePrimitives.RequireValue(orientation), sheetGlobalKey: _sheetGlobalKey, childRect: _previousChildRect, child: _builder!(context, animation));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _curvedAnimation?.dispose();
        _sheetOpacityCurvedAnimation?.dispose();
        base.dispose();
    }

}

internal class _ContextMenuRouteStatic__context_menu : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual List<global::Doroti.Framework.Widgets.Widget>? actions { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState>? childGlobalKey { get; private set; }
    public virtual _ContextMenuLocation__context_menu contextMenuLocation { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Widgets.BuildContext, double, double>? onDismiss { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Orientation orientation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState>? sheetGlobalKey { get; private set; }
    public virtual Rect childRect { get; private set; } = default!;

    internal _ContextMenuRouteStatic__context_menu(List<global::Doroti.Framework.Widgets.Widget>? actions = null, global::Doroti.Framework.Widgets.Widget child = default!, global::Doroti.Framework.Widgets.GlobalKey<IState>? childGlobalKey = null, _ContextMenuLocation__context_menu contextMenuLocation = default!, global::System.Action<global::Doroti.Framework.Widgets.BuildContext, double, double>? onDismiss = null, global::Doroti.Framework.Widgets.Orientation orientation = default!, global::Doroti.Framework.Widgets.GlobalKey<IState>? sheetGlobalKey = null, Rect childRect = default!)
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

internal class _ContextMenuRouteStaticState__context_menu : global::Doroti.Framework.Widgets.State<_ContextMenuRouteStatic__context_menu>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<_ContextMenuRouteStatic__context_menu>
{
    internal const double _kMinScale = 0.8;
    internal const double _kSheetScaleThreshold = 0.9;
    internal const double _kPadding = 20.0;
    internal const double _kDamping = 400.0;
    internal static Duration _kMoveControllerDuration = Duration.Create(milliseconds: 600L);
    internal virtual Offset _dragOffset { get; set; } = default!;
    internal virtual double _lastScale { get; set; } = 1.0;
    internal virtual global::Doroti.Framework.Animation.AnimationController _moveController { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _moveCurvedAnimation { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _sheetController { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _sheetCurvedAnimation { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<Offset> _moveAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _sheetScaleAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _sheetOpacityAnimation { get; set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal static double _getScale(global::Doroti.Framework.Widgets.Orientation orientation, double maxDragDistance, double dy)
    {
        double dyDirectional = (dy <= 0.0) ? dy : -dy;
        return Math.Max(_kMinScale, (maxDragDistance + dyDirectional) / maxDragDistance);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onPanStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        _moveController.value = 1.0;
        _setDragOffset(Offset.zero);
    }

    internal virtual void _onPanUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        _setDragOffset(_dragOffset + details.delta);
    }

    internal virtual void _onPanEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        if (details.velocity.pixelsPerSecond.dy.abs() >= Gestures.ConstantsLibrary.kMinFlingVelocity)
        {
            bool flingIsAway = details.velocity.pixelsPerSecond.dy > 0L;
            double finalPosition = flingIsAway ? (_moveAnimation.value.dy + 100.0) : 0.0;
            if (flingIsAway && (!Equals(_sheetController.status, AnimationStatus.forward)))
            {
                _sheetController.forward();
            }
            else
            {
                if (!flingIsAway && (!Equals(_sheetController.status, AnimationStatus.reverse)))
                {
                    _sheetController.reverse();
                }
            }
            _moveAnimation = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.0, _moveAnimation.value.dy), end: new global::Doroti.Ui.Offset(0.0, finalPosition)).animate(_moveController);
            _moveController.reset();
            _moveController.duration = Duration.Create(milliseconds: 64L);
            _moveController.forward();
            _moveController.addStatusListener(_flingStatusListener);
            return;
        }
        if (_lastScale == _kMinScale)
        {
            widget.onDismiss!(context, _lastScale, _sheetOpacityAnimation.value);
            return;
        }
        _moveController.addListener(_moveListener);
        _moveController.reverse();
    }

    internal virtual void _moveListener()
    {
        if (_lastScale > _kSheetScaleThreshold)
        {
            _moveController.removeListener(_moveListener);
            if (!_sheetController.isDismissed)
            {
                _sheetController.reverse();
            }
        }
    }

    internal virtual void _flingStatusListener(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (!AnimationStatusMembers.isCompleted(status))
        {
            return;
        }
        _moveController.duration = _kMoveControllerDuration;
        _moveController.removeStatusListener(_flingStatusListener);
        if (_moveAnimation.value.dy == 0.0)
        {
            return;
        }
        widget.onDismiss!(context, _lastScale, _sheetOpacityAnimation.value);
    }

    internal virtual void _setDragOffset(Offset dragOffset)
    {
        double endX = SliderLibrary._kPadding * dragOffset.dx / _kDamping;
        double endY = (dragOffset.dy >= 0.0) ? dragOffset.dy : (SliderLibrary._kPadding * dragOffset.dy / _kDamping);
        setState(() =>
        {
            _dragOffset = dragOffset;
            _moveAnimation = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(endX, -SliderLibrary._kPadding, SliderLibrary._kPadding), endY)).animate(_moveCurvedAnimation);
            if ((_lastScale <= _kSheetScaleThreshold) && (!Equals(_sheetController.status, AnimationStatus.forward)) && (_sheetScaleAnimation.value != 0.0))
            {
                _sheetController.forward();
            }
            else
            {
                if ((_lastScale > _kSheetScaleThreshold) && (!Equals(_sheetController.status, AnimationStatus.reverse)) && (_sheetScaleAnimation.value != 1.0))
                {
                    _sheetController.reverse();
                }
            }
        });
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _getChild(global::Doroti.Framework.Widgets.Orientation orientation, _ContextMenuLocation__context_menu contextMenuLocation)
    {
        global::Doroti.Ui.Size screenSize = MediaQuery.sizeOf(context);
        global::Doroti.Framework.Painting.EdgeInsets padding = MediaQuery.paddingOf(context);
        var screenBoundsLocal = Rect.fromLTWH(0, 0, screenSize.width - padding.left - padding.right, screenSize.height - padding.top - padding.bottom);
        global::Doroti.Framework.Widgets.Widget sheetLocal = new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: _sheetController, builder: _buildSheetAnimation, child: new _ContextMenuSheet__context_menu(key: widget.sheetGlobalKey, actions: widget.actions!, contextMenuLocation: widget.contextMenuLocation, orientation: widget.orientation));
        global::Doroti.Framework.Widgets.Widget childLocal = new _ContextMenuAlignedChildren__context_menu(targetRect: widget.childRect, screenBounds: screenBoundsLocal, sheet: sheetLocal, contextMenuLocation: contextMenuLocation, orientation: widget.orientation, child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: _moveController, builder: _buildChildAnimation, child: widget.child));
        return childLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildSheetAnimation(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget? child)
    {
        return Transform.CreateScale(alignment: _ContextMenuRoute__context_menu<object>.getSheetAlignment(widget.contextMenuLocation, widget.orientation), scale: _sheetScaleAnimation.value, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: _sheetOpacityAnimation, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildChildAnimation(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget? child)
    {
        _lastScale = _getScale(widget.orientation, MediaQuery.heightOf(context), _moveAnimation.value.dy);
        return Transform.CreateScale(key: widget.childGlobalKey, scale: _lastScale, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildAnimation(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget? child)
    {
        return Transform.CreateTranslate(offset: _moveAnimation.value, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        base.initState();
        _moveController = new global::Doroti.Framework.Animation.AnimationController(duration: _kMoveControllerDuration, value: 1.0, vsync: this);
        _moveCurvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _moveController, curve: Curves.elasticIn);
        _sheetController = new global::Doroti.Framework.Animation.AnimationController(duration: Duration.Create(milliseconds: 100L), reverseDuration: Duration.Create(milliseconds: 300L), vsync: this);
        _sheetCurvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _sheetController, curve: Curves.linear, reverseCurve: Curves.easeInBack);
        _sheetScaleAnimation = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).animate(_sheetCurvedAnimation);
        _sheetOpacityAnimation = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).animate(_sheetController);
        _setDragOffset(Offset.zero);
    }

    public override void dispose()
    {
        _moveController.dispose();
        _moveCurvedAnimation.dispose();
        _sheetController.dispose();
        _sheetCurvedAnimation.dispose();
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget childLocal = _getChild(widget.orientation, widget.contextMenuLocation);
        return new global::Doroti.Framework.Widgets.SafeArea(child: new global::Doroti.Framework.Widgets.Align(alignment: Alignment.topLeft, child: new global::Doroti.Framework.Widgets.GestureDetector(onPanEnd: _onPanEnd, onPanStart: _onPanStart, onPanUpdate: _onPanUpdate, child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: _moveController, builder: _buildAnimation, child: childLocal))));
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

internal class _ContextMenuSheet__context_menu : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual List<global::Doroti.Framework.Widgets.Widget> actions { get; private set; } = default!;
    public virtual _ContextMenuLocation__context_menu contextMenuLocation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Orientation orientation { get; private set; } = default!;

    internal _ContextMenuSheet__context_menu(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget> actions = default!, _ContextMenuLocation__context_menu contextMenuLocation = default!, global::Doroti.Framework.Widgets.Orientation orientation = default!) : base(key: key)
    {
        this.actions = actions;
        this.contextMenuLocation = contextMenuLocation;
        this.orientation = orientation;
        System.Diagnostics.Debug.Assert(Enumerable.Any(actions));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ContextMenuSheetState__context_menu());
}

internal class _ContextMenuSheetState__context_menu : global::Doroti.Framework.Widgets.State<_ContextMenuSheet__context_menu>
{
    internal virtual global::Doroti.Framework.Widgets.ScrollController _controller { get; private set; } = default!;
    internal const double _kMenuWidth = 250.0;
    internal const double _kScrollbarMainAxisMargin = 13.0;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Widgets.ScrollController();
    }

    public override void dispose()
    {
        _controller.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.SizedBox(width: _kMenuWidth, child: new global::Doroti.Framework.Widgets.IntrinsicHeight(child: new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: BorderRadius.CreateAll(Radius.circular(13.0)), child: new global::Doroti.Framework.Widgets.ColoredBox(color: CupertinoDynamicColor.resolve(CupertinoContextMenu.kBackgroundColor, context), child: new global::Doroti.Framework.Widgets.ScrollConfiguration(behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false), child: new CupertinoScrollbar(mainAxisMargin: _kScrollbarMainAxisMargin, controller: _controller, child: new global::Doroti.Framework.Widgets.SingleChildScrollView(controller: _controller, child: new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection49409 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection49409.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(widget.actions.First())); foreach (var action in widget.actions.skip(1L)) { __collection49409.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(border: new global::Doroti.Framework.Painting.Border(top: new global::Doroti.Framework.Painting.BorderSide(color: CupertinoDynamicColor.resolve(Context_menuLibrary._borderColor, context), width: 0.4))), position: DecorationPosition.foreground, child: action))); } return __collection49409; }))()))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal enum _ContextMenuChild__context_menu
{
    child,
    menuSheet
}

internal class _ContextMenuAlignedChildren__context_menu : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Rect targetRect { get; private set; } = default!;
    public virtual Rect screenBounds { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget sheet { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Orientation orientation { get; private set; } = default!;
    public virtual _ContextMenuLocation__context_menu contextMenuLocation { get; private set; } = default!;

    internal _ContextMenuAlignedChildren__context_menu(Rect targetRect, Rect screenBounds, global::Doroti.Framework.Widgets.Widget child, global::Doroti.Framework.Widgets.Widget sheet, global::Doroti.Framework.Widgets.Orientation orientation, _ContextMenuLocation__context_menu contextMenuLocation)
    {
        this.targetRect = targetRect;
        this.screenBounds = screenBounds;
        this.child = child;
        this.sheet = sheet;
        this.orientation = orientation;
        this.contextMenuLocation = contextMenuLocation;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.CustomMultiChildLayout(@delegate: new _ContextMenuAlignedChildrenDelegate__context_menu(targetRect: targetRect, screenBounds: screenBounds, orientation: orientation, contextMenuLocation: contextMenuLocation), children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.LayoutId(id: _ContextMenuChild__context_menu.child, child: child)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.LayoutId(id: _ContextMenuChild__context_menu.menuSheet, child: sheet)) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ContextMenuAlignedChildrenDelegate__context_menu : global::Doroti.Framework.Rendering.MultiChildLayoutDelegate
{
    public virtual Rect targetRect { get; private set; } = default!;
    public virtual Rect screenBounds { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Orientation orientation { get; private set; } = default!;
    public virtual _ContextMenuLocation__context_menu contextMenuLocation { get; private set; } = default!;

    internal _ContextMenuAlignedChildrenDelegate__context_menu(Rect targetRect, Rect screenBounds, global::Doroti.Framework.Widgets.Orientation orientation, _ContextMenuLocation__context_menu contextMenuLocation)
    {
        this.targetRect = targetRect;
        this.screenBounds = screenBounds;
        this.orientation = orientation;
        this.contextMenuLocation = contextMenuLocation;
    }

    public override void performLayout(Size size)
    {
        var constraints = BoxConstraints.CreateLoose(size);
        double availableHeightForChild = screenBounds.height - _ContextMenuRouteStaticState__context_menu._kPadding;
        double availableWidth = screenBounds.width - (_ContextMenuRouteStaticState__context_menu._kPadding * 2L);
        double availableWidthForChild = orientation switch { Orientation.portrait => availableWidth, Orientation.landscape => availableWidth - _ContextMenuSheetState__context_menu._kMenuWidth, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        DartRuntimePrimitives.Assert(() => availableWidthForChild >= 0.0);
        DartRuntimePrimitives.Assert(() => availableHeightForChild >= 0.0);
        global::Doroti.Ui.Size childSize = layoutChild(_ContextMenuChild__context_menu.child, constraints.copyWith(maxHeight: availableHeightForChild, maxWidth: availableWidthForChild));
        double availableHeightForMenu = orientation switch { Orientation.portrait => availableHeightForChild - (childSize.height + _ContextMenuRouteStaticState__context_menu._kPadding), Orientation.landscape => availableHeightForChild, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        global::Doroti.Ui.Size menuSize = layoutChild(_ContextMenuChild__context_menu.menuSheet, constraints.copyWith(maxHeight: availableHeightForMenu));
        double initialChildLeft = default!;
        double initialChildTop = default!;
        double maxClampedLeft = default!;
        double maxClampedTop = default!;
        global::Doroti.Ui.Offset secondChildOffset = default!;
        bool menuBeforeChild = default!;
        switch (orientation)
        {
            case Orientation.portrait:
                {
                    menuBeforeChild = false;
                    double totalHeight = childSize.height + menuSize.height + _ContextMenuRouteStaticState__context_menu._kPadding;
                    double totalWidth = childSize.width + _ContextMenuRouteStaticState__context_menu._kPadding;
                    initialChildLeft = targetRect.center.dx - (childSize.width / 2L);
                    initialChildTop = targetRect.center.dy - childSize.height;
                    double secondChildDx = contextMenuLocation switch { _ContextMenuLocation__context_menu.center => (childSize.width / 2L) - (menuSize.width / 2L), _ContextMenuLocation__context_menu.left => 0.0, _ContextMenuLocation__context_menu.right => childSize.width - menuSize.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                    secondChildOffset = new global::Doroti.Ui.Offset(secondChildDx, childSize.height + _ContextMenuRouteStaticState__context_menu._kPadding);
                    maxClampedLeft = screenBounds.right - totalWidth;
                    maxClampedTop = screenBounds.bottom - totalHeight;
                    break;
                }
            case Orientation.landscape:
                {
                    menuBeforeChild = Equals(contextMenuLocation, _ContextMenuLocation__context_menu.right);
                    double totalWidthLocal = childSize.width + menuSize.width + _ContextMenuRouteStaticState__context_menu._kPadding;
                    initialChildLeft = screenBounds.center.dx - (totalWidthLocal / 2L);
                    initialChildTop = screenBounds.center.dy - (Math.Max(childSize.height, menuSize.height) / 2L);
                    double secondChildDxLocal = menuBeforeChild ? menuSize.width : childSize.width;
                    secondChildOffset = new global::Doroti.Ui.Offset(secondChildDxLocal + _ContextMenuRouteStaticState__context_menu._kPadding, 0.0);
                    maxClampedLeft = screenBounds.right - totalWidthLocal;
                    maxClampedTop = screenBounds.bottom;
                    break;
                }
        }
        double clampedLeft = Dart_uiLibrary.clampDouble(initialChildLeft, screenBounds.left + _ContextMenuRouteStaticState__context_menu._kPadding, maxClampedLeft);
        double clampedTop = Dart_uiLibrary.clampDouble(initialChildTop, screenBounds.top + _ContextMenuRouteStaticState__context_menu._kPadding, maxClampedTop);
        var firstPosition = new global::Doroti.Ui.Offset(clampedLeft, clampedTop);
        global::Doroti.Ui.Offset secondPosition = firstPosition + secondChildOffset;
        positionChild(_ContextMenuChild__context_menu.child, menuBeforeChild ? secondPosition : firstPosition);
        positionChild(_ContextMenuChild__context_menu.menuSheet, menuBeforeChild ? firstPosition : secondPosition);
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_ContextMenuAlignedChildrenDelegate__context_menu)oldDelegate;
        return (!Equals(__oldDelegate.targetRect, targetRect)) || (!Equals(__oldDelegate.screenBounds, screenBounds)) || (!Equals(__oldDelegate.orientation, orientation)) || (!Equals(__oldDelegate.contextMenuLocation, contextMenuLocation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
