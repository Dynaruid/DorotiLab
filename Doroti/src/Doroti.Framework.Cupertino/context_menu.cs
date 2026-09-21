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
    internal static long _animationDuration =
        _previewLongPressTimeout.inMilliseconds + _kModalPopupTransitionDuration.inMilliseconds;
}

public static partial class Context_menuLibrary
{
    internal static List<BoxShadow> _endBoxShadow = new List<BoxShadow>
    {
        new BoxShadow(color: new Color(1073741824L), blurRadius: 10.0, spreadRadius: 0.5),
    };
}

public static partial class Context_menuLibrary
{
    internal static Color _borderColor = new CupertinoDynamicColor(
        color: new Color(4289309103L),
        darkColor: new Color(4283914330L)
    );
}

public static partial class Context_menuLibrary
{
    internal static Color _kBackgroundColor = new CupertinoDynamicColor(
        color: new Color(4294046193L),
        darkColor: new Color(4280361250L)
    );
}

internal delegate void _DismissCallback__context_menu(
    BuildContext context,
    double scale,
    double opacity
);

public delegate Widget CupertinoContextMenuBuilder(
    BuildContext context,
    Animation<double> animation
);

public static partial class Context_menuLibrary
{
    internal static Rect _getRect(GlobalKey<IState> globalKey)
    {
        DartRuntimePrimitives.Assert(() => globalKey.currentContext is not null);
        var renderBoxContainer = ((RenderBox?)globalKey.currentContext!.findRenderObject()!)!;
        return Rect.fromPoints(
            renderBoxContainer.localToGlobal(renderBoxContainer.paintBounds.topLeft),
            renderBoxContainer.localToGlobal(renderBoxContainer.paintBounds.bottomRight)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal enum _ContextMenuLocation__context_menu
{
    center,
    left,
    right,
}

public class CupertinoContextMenu : StatefulWidget
{
    public static double kOpenBorderRadius = Context_menuLibrary._previewBorderRadiusRatio;
    public static List<BoxShadow> kEndBoxShadow = Context_menuLibrary._endBoxShadow;
    public static double animationOpensAt =
        Context_menuLibrary._previewLongPressTimeout.inMilliseconds
        / Context_menuLibrary._animationDuration;
    public static Color kBackgroundColor = Context_menuLibrary._kBackgroundColor;
    public virtual Func<BuildContext, Animation<double>, Widget> builder { get; private set; } =
        default!;
    public virtual Widget? child { get; private set; }
    public virtual List<Widget> actions { get; private set; } = default!;
    public virtual bool enableHapticFeedback { get; private set; } = default!;

    public CupertinoContextMenu(
        Key? key = null,
        List<Widget> actions = default!,
        Widget child = default!,
        bool enableHapticFeedback = false
    )
        : base(key: key)
    {
        this.actions = actions;
        this.child = child;
        this.enableHapticFeedback = enableHapticFeedback;
        builder = (context, animation) => child;
        System.Diagnostics.Debug.Assert(Enumerable.Any(actions));
    }

    public static CupertinoContextMenu CreateBuilder(
        Key? key = null,
        List<Widget> actions = default!,
        Func<BuildContext, Animation<double>, Widget> builder = default!,
        bool enableHapticFeedback = false
    )
    {
        var __instance = new CupertinoContextMenu(
            key: key,
            actions: actions,
            child: default!,
            enableHapticFeedback: enableHapticFeedback
        );
        __instance.actions = actions;
        __instance.builder = builder;
        __instance.enableHapticFeedback = enableHapticFeedback;
        __instance.child = null;
        return __instance;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoContextMenuState__context_menu());
}

internal class _CupertinoContextMenuState__context_menu
    : State<CupertinoContextMenu>,
        TickerProviderStateMixin<CupertinoContextMenu>
{
    internal virtual GlobalKey<IState> _childGlobalKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual bool _childHidden { get; set; } = false;
    internal virtual AnimationController _openController { get; set; } = default!;
    internal virtual Rect? _decoyChildEndRect { get; set; } = default;
    internal virtual double _scaleFactor { get; set; } = default!;
    internal virtual OverlayEntry? _lastOverlayEntry { get; set; } = default;
    internal virtual _ContextMenuRoute__context_menu<object?>? _route { get; set; } = default;
    internal virtual double _midpoint { get; private set; } =
        CupertinoContextMenu.animationOpensAt / 2L;
    internal virtual Gestures.TapGestureRecognizer _tapGestureRecognizer { get; private set; } =
        default!;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _openController = new AnimationController(
            duration: Context_menuLibrary._previewLongPressTimeout,
            vsync: this,
            upperBound: CupertinoContextMenu.animationOpensAt
        );
        _openController.addStatusListener(_onDecoyAnimationStatusChange);
        _tapGestureRecognizer = (
            (Func<Gestures.TapGestureRecognizer>)(
                () =>
                {
                    var __cascade = new Gestures.TapGestureRecognizer();
                    __cascade.onTapCancel = _onTapCancel;
                    __cascade.onTapDown = _onTapDown;
                    __cascade.onTapUp = _onTapUp;
                    __cascade.onTap = _onTap;
                    return __cascade;
                }
            )
        )();
    }

    internal virtual void _listenerCallback()
    {
        if (
            (!Equals(_openController.status, AnimationStatus.reverse))
            && (_openController.value >= _midpoint)
        )
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
            Rect childRect = Context_menuLibrary._getRect(_childGlobalKey);
            double screenWidth = MediaQuery.widthOf(context);
            double centerLocal = screenWidth / 2L;
            bool centerDividesChild =
                (childRect.left < centerLocal) && (childRect.right > centerLocal);
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

    internal static double _getScaleFactor(Rect childRect, EdgeInsets padding, Size size)
    {
        double leftMaxScale = 2L * (childRect.center.dx - padding.left) / childRect.width;
        double topMaxScale = 2L * (childRect.center.dy - padding.top) / childRect.height;
        double rightMaxScale =
            2L * (size.width - padding.right - childRect.center.dx) / childRect.width;
        double bottomMaxScale =
            2L * (size.height - padding.bottom - childRect.center.dy) / childRect.height;
        double minWidth = Math.Min(leftMaxScale, rightMaxScale);
        double minHeight = Math.Min(topMaxScale, bottomMaxScale);
        return Dart_uiLibrary.clampDouble(
            Math.Min(minWidth, minHeight),
            Context_menuLibrary._kMinScaleFactor,
            Context_menuLibrary._kOpenScale
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static Widget _defaultPreviewBuilder(
        BuildContext context,
        Animation<double> animation,
        Widget child
    )
    {
        return new FittedBox(
            fit: BoxFit.cover,
            child: new ClipRSuperellipse(
                borderRadius: BorderRadius.CreateCircular(
                    Context_menuLibrary._previewBorderRadiusRatio * animation.value
                ),
                child: child
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _openContextMenu()
    {
        setState(() =>
        {
            _childHidden = true;
        });
        _route = new _ContextMenuRoute__context_menu<object?>(
            actions: widget.actions,
            barrierLabel: CupertinoLocalizations.of(context).menuDismissLabel,
            filter: new ImageFilter(sigmaX: 5.0, sigmaY: 5.0),
            contextMenuLocation: _contextMenuLocation,
            previousChildRect: (
                _decoyChildEndRect
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            scaleFactor: _scaleFactor,
            builder: (context, animation) =>
            {
                if (widget.child is null)
                {
                    Animation<double> localAnimation = new Tween<double>(
                        begin: CupertinoContextMenu.animationOpensAt,
                        end: 1
                    ).animate(animation);
                    return widget.builder(context, localAnimation);
                }
                return _defaultPreviewBuilder(context, animation, widget.child!);
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        DartRuntimePrimitives.Ignore(Navigator.of(context, rootNavigator: true).push(_route!));
        _route!.animation!.addStatusListener(_routeAnimationStatusListener);
    }

    internal virtual void _removeContextMenuDecoy()
    {
        Scheduler.SchedulerBinding.instance.addPostFrameCallback(
            (_) =>
            {
                if (mounted)
                {
                    _closeContextMenu();
                    _openController.reset();
                }
            },
            debugLabel: "removeContextMenuDecoy"
        );
    }

    internal virtual void _closeContextMenu()
    {
        _lastOverlayEntry?.remove();
        _lastOverlayEntry?.dispose();
        _lastOverlayEntry = null;
    }

    internal virtual void _onDecoyAnimationStatusChange(AnimationStatus animationStatus)
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

    internal virtual void _routeAnimationStatusListener(AnimationStatus status)
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

    internal virtual void _onTapUp(Gestures.TapUpDetails details)
    {
        _onTapCompleted();
    }

    internal virtual void _onTapDown(Gestures.TapDownDetails details)
    {
        _openController.addListener(_listenerCallback);
        setState(() =>
        {
            _childHidden = true;
        });
        Rect childRect = Context_menuLibrary._getRect(_childGlobalKey);
        _scaleFactor = _getScaleFactor(
            childRect,
            MediaQuery.paddingOf(context),
            MediaQuery.sizeOf(context)
        );
        _decoyChildEndRect = Rect.fromCenter(
            center: childRect.center,
            width: childRect.width * _scaleFactor,
            height: childRect.height * _scaleFactor
        );
        _lastOverlayEntry = new OverlayEntry(
            builder: (context) =>
            {
                return new _DecoyChild__context_menu(
                    beginRect: childRect,
                    controller: _openController,
                    endRect: _decoyChildEndRect,
                    builder: widget.builder,
                    child: widget.child
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        Overlay.of(context, rootOverlay: true, debugRequiredFor: widget).insert(_lastOverlayEntry!);
        _openController.forward();
    }

    public override Widget build(BuildContext context)
    {
        return new MouseRegion(
            cursor: Foundation.ConstantsLibrary.kIsWeb
                ? SystemMouseCursors.click
                : MouseCursor.defer,
            child: new Listener(
                onPointerDown: _tapGestureRecognizer.addPointer,
                child: new TickerMode(
                    enabled: !_childHidden,
                    child: new Visibility(
                        key: _childGlobalKey,
                        visible: !_childHidden,
                        child: widget.builder(context, _openController)
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
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
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

internal class _DecoyChild__context_menu : StatefulWidget
{
    public virtual Rect? beginRect { get; private set; }
    public virtual AnimationController controller { get; private set; } = default!;
    public virtual Rect? endRect { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual Func<BuildContext, Animation<double>, Widget>? builder { get; private set; }

    internal _DecoyChild__context_menu(
        Rect? beginRect = null,
        AnimationController controller = default!,
        Rect? endRect = null,
        Widget? child = null,
        Func<BuildContext, Animation<double>, Widget>? builder = null
    )
    {
        this.beginRect = beginRect;
        this.controller = controller;
        this.endRect = endRect;
        this.child = child;
        this.builder = builder;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _DecoyChildState__context_menu());
}

internal class _DecoyChildState__context_menu
    : State<_DecoyChild__context_menu>,
        TickerProviderStateMixin<_DecoyChild__context_menu>
{
    internal virtual Animation<Rect?> _rect { get; set; } = default!;
    internal virtual Animation<Decoration> _boxDecoration { get; set; } = default!;
    internal virtual CurvedAnimation _boxDecorationCurvedAnimation { get; private set; } = default!;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        var beginPause = 1.0;
        var openAnimationLength = 5.0;
        double totalOpenAnimationLength = beginPause + openAnimationLength;
        double endPause =
            (
                totalOpenAnimationLength
                * Context_menuLibrary._animationDuration
                / Context_menuLibrary._previewLongPressTimeout.inMilliseconds
            ) - totalOpenAnimationLength;
        _rect = new TweenSequence<Rect?>(
            new List<TweenSequenceItem<Rect?>>
            {
                new TweenSequenceItem<Rect?>(
                    tween: new RectTween(begin: widget.beginRect, end: widget.beginRect).chain(
                        new CurveTween(curve: Curves.linear)
                    ),
                    weight: beginPause
                ),
                new TweenSequenceItem<Rect?>(
                    tween: new RectTween(begin: widget.beginRect, end: widget.endRect).chain(
                        new CurveTween(curve: Curves.easeOutSine)
                    ),
                    weight: openAnimationLength
                ),
                new TweenSequenceItem<Rect?>(
                    tween: new RectTween(begin: widget.endRect, end: widget.endRect).chain(
                        new CurveTween(curve: Curves.linear)
                    ),
                    weight: endPause
                ),
            }
                .Cast<TweenSequenceItem<Rect?>>()
                .ToList()
        ).animate(widget.controller);
        _boxDecorationCurvedAnimation = new CurvedAnimation(
            parent: widget.controller,
            curve: new Interval(0.0, CupertinoContextMenu.animationOpensAt)
        );
        _boxDecoration = new DecorationTween(
            begin: new BoxDecoration(boxShadow: new List<BoxShadow>()),
            end: new BoxDecoration(boxShadow: Context_menuLibrary._endBoxShadow)
        ).animate(_boxDecorationCurvedAnimation);
    }

    internal virtual Widget _buildAnimation(BuildContext context, Widget? child)
    {
        return Positioned.CreateFromRect(
            rect: (
                _rect.value
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            child: new Container(decoration: _boxDecoration.value, child: widget.child)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildBuilder(BuildContext context, Widget? child)
    {
        return Positioned.CreateFromRect(
            rect: (
                _rect.value
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            child: widget.builder!(context, widget.controller)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        _boxDecorationCurvedAnimation.dispose();
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new Stack(
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new AnimatedBuilder(
                        builder: (widget.child is not null) ? _buildAnimation : _buildBuilder,
                        animation: widget.controller
                    )
                ),
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
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
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

internal class _ContextMenuRoute__context_menu<T> : PopupRoute<T>
{
    internal static Color _kModalBarrierColor = new Color(1711539215L);
    internal virtual List<Widget> _actions { get; private set; } = default!;
    internal virtual Func<BuildContext, Animation<double>, Widget>? _builder { get; private set; }
    internal virtual GlobalKey<IState> _childGlobalKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual _ContextMenuLocation__context_menu _contextMenuLocation { get; private set; } =
        default!;
    internal virtual bool _externalOffstage { get; set; } = false;
    internal virtual bool _internalOffstage { get; set; } = false;
    internal virtual double _scaleFactor { get; private set; } = default!;
    internal virtual Orientation? _lastOrientation { get; set; } = default;
    internal virtual Rect _previousChildRect { get; private set; } = default!;
    internal virtual double? _scale { get; set; } = 1.0;
    internal virtual GlobalKey<IState> _sheetGlobalKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal static CurveTween _curve = new CurveTween(curve: Curves.easeOutBack);
    internal static CurveTween _curveReverse = new CurveTween(curve: Curves.easeInBack);
    internal static RectTween _rectTween = new RectTween();
    internal static Animatable<Rect?> _rectAnimatable = _rectTween.chain(_curve);
    internal static RectTween _rectTweenReverse = new RectTween();
    internal static Animatable<Rect?> _rectAnimatableReverse = _rectTweenReverse.chain(
        _curveReverse
    );
    internal static RectTween _sheetRectTween = new RectTween();
    internal virtual Animatable<Rect?> _sheetRectAnimatable { get; private set; } =
        _sheetRectTween.chain(_curve);
    internal virtual Animatable<Rect?> _sheetRectAnimatableReverse { get; private set; } =
        _sheetRectTween.chain(_curveReverse);
    internal static Tween<double> _sheetScaleTween = new Tween<double>();
    internal static Animatable<double> _sheetScaleAnimatable = _sheetScaleTween.chain(_curve);
    internal static Animatable<double> _sheetScaleAnimatableReverse = _sheetScaleTween.chain(
        _curveReverse
    );
    internal virtual Tween<double> _opacityTween { get; private set; } =
        new Tween<double>(begin: 0.0, end: 1.0);
    internal virtual Animation<double> _sheetOpacity { get; set; } = default!;
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel
    {
        get => __field_barrierLabel;
    }
    internal virtual CurvedAnimation? _curvedAnimation { get; set; } = default;
    internal virtual CurvedAnimation? _sheetOpacityCurvedAnimation { get; set; } = default;

    internal _ContextMenuRoute__context_menu(
        List<Widget> actions,
        _ContextMenuLocation__context_menu contextMenuLocation,
        string? barrierLabel = null,
        Func<BuildContext, Animation<double>, Widget>? builder = null,
        ImageFilter? filter = null,
        Rect previousChildRect = default!,
        double scaleFactor = default!,
        RouteSettings? settings = null
    )
        : base(filter: filter, settings: settings)
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
    public override Duration transitionDuration =>
        Context_menuLibrary._kModalPopupTransitionDuration;

    internal static Rect _getScaledRect(GlobalKey<IState> globalKey, double scale)
    {
        Rect childRect = Context_menuLibrary._getRect(globalKey);
        Size sizeScaled = childRect.size * scale;
        var offsetScaled = new Offset(
            childRect.left + ((childRect.size.width - sizeScaled.width) / 2L),
            childRect.top + ((childRect.size.height - sizeScaled.height) / 2L)
        );
        return offsetScaled & sizeScaled;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static AlignmentDirectional getSheetAlignment(
        _ContextMenuLocation__context_menu contextMenuLocation,
        Orientation orientation
    )
    {
        return contextMenuLocation switch
        {
            _ContextMenuLocation__context_menu.center
                when Equals((orientation), Orientation.landscape) => AlignmentDirectional.topStart,
            _ContextMenuLocation__context_menu.center => AlignmentDirectional.topCenter,
            _ContextMenuLocation__context_menu.right => AlignmentDirectional.topEnd,
            _ContextMenuLocation__context_menu.left => AlignmentDirectional.topStart,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static Rect _getSheetRectBegin(
        Orientation? orientation,
        _ContextMenuLocation__context_menu contextMenuLocation,
        Rect childRect,
        Rect sheetRect
    )
    {
        switch (contextMenuLocation)
        {
            case _ContextMenuLocation__context_menu.center:
            {
                Offset target = Equals(orientation, Orientation.portrait)
                    ? childRect.bottomCenter
                    : childRect.topCenter;
                Offset centered = target - new Offset(sheetRect.width / 2L, 0.0);
                return centered & sheetRect.size;
            }
            case _ContextMenuLocation__context_menu.right:
            {
                Offset targetLocal = Equals(orientation, Orientation.portrait)
                    ? childRect.bottomRight
                    : childRect.topRight;
                return (targetLocal - new Offset(sheetRect.width, 0.0)) & sheetRect.size;
            }
            case _ContextMenuLocation__context_menu.left:
            {
                Offset targetAlternate = Equals(orientation, Orientation.portrait)
                    ? childRect.bottomLeft
                    : childRect.topLeft;
                return targetAlternate & sheetRect.size;
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _onDismiss(BuildContext context, double scale, double opacity)
    {
        _scale = scale;
        _opacityTween.end = opacity;
        _sheetOpacityCurvedAnimation = new CurvedAnimation(
            parent: animation!,
            curve: new Interval(0.9, 1.0)
        );
        _sheetOpacity = _opacityTween.animate(_sheetOpacityCurvedAnimation!);
        Navigator.of(context).pop<object>();
    }

    internal virtual void _updateTweenRects()
    {
        Rect childRect =
            (_scale is null)
                ? Context_menuLibrary._getRect(_childGlobalKey)
                : _ContextMenuRoute__context_menu<T>._getScaledRect(
                    _childGlobalKey,
                    (
                        _scale
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                );
        _rectTween.begin = _previousChildRect;
        _rectTween.end = childRect;
        var childRectOriginal = Rect.fromCenter(
            center: _previousChildRect.center,
            width: _previousChildRect.width / _scaleFactor,
            height: _previousChildRect.height / _scaleFactor
        );
        Rect sheetRect = Context_menuLibrary._getRect(_sheetGlobalKey);
        Rect sheetRectBegin = _ContextMenuRoute__context_menu<T>._getSheetRectBegin(
            _lastOrientation,
            _contextMenuLocation,
            childRectOriginal,
            sheetRect
        );
        _sheetRectTween.begin = sheetRectBegin;
        _sheetRectTween.end = sheetRect;
        _sheetScaleTween.begin = 0.0;
        _sheetScaleTween.end = (
            _scale ?? throw new global::System.NullReferenceException("A required value was null.")
        );
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
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

    public override Scheduler.TickerFuture didPush()
    {
        _internalOffstage = true;
        _setOffstageInternally();
        Scheduler.SchedulerBinding.instance.addPostFrameCallback(
            (_) =>
            {
                _updateTweenRects();
                _internalOffstage = false;
                _setOffstageInternally();
            },
            debugLabel: "renderContextMenuRouteOffstage"
        );
        return base.didPush();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Animation<double> createAnimation()
    {
        Animation<double> animation = base.createAnimation();
        if (!Equals(_curvedAnimation?.parent, animation))
        {
            _curvedAnimation?.dispose();
            _curvedAnimation = new CurvedAnimation(parent: animation, curve: Curves.linear);
        }
        _sheetOpacity = _opacityTween.animate(_curvedAnimation!);
        return animation;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget buildPage(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation
    )
    {
        return SizedBox.CreateShrink();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget buildTransitions(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child
    )
    {
        return new OrientationBuilder(
            builder: (context, orientation) =>
            {
                _lastOrientation = (orientation);
                if (!animation.isCompleted)
                {
                    var reverseLocal = Equals(animation.status, AnimationStatus.reverse);
                    Rect rectLocal = reverseLocal
                        ? (
                            _rectAnimatableReverse.evaluate(animation)
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                        : (
                            _rectAnimatable.evaluate(animation)
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        );
                    Rect sheetRect = reverseLocal
                        ? (
                            _sheetRectAnimatableReverse.evaluate(animation)
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                        : (
                            _sheetRectAnimatable.evaluate(animation)
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        );
                    double sheetScale = reverseLocal
                        ? _sheetScaleAnimatableReverse.evaluate(animation)
                        : _sheetScaleAnimatable.evaluate(animation);
                    return Stack.Create(
                        children: new List<Widget>
                        {
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                Positioned.CreateFromRect(
                                    rect: sheetRect,
                                    child: new FadeTransition(
                                        opacity: _sheetOpacity,
                                        child: Transform.CreateScale(
                                            alignment: _ContextMenuRoute__context_menu<T>.getSheetAlignment(
                                                _contextMenuLocation,
                                                ((orientation))
                                            ),
                                            scale: sheetScale,
                                            child: new _ContextMenuSheet__context_menu(
                                                key: _sheetGlobalKey,
                                                actions: _actions,
                                                contextMenuLocation: _contextMenuLocation,
                                                orientation: (orientation)
                                            )
                                        )
                                    )
                                )
                            ),
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                Positioned.CreateFromRect(
                                    key: _childGlobalKey,
                                    rect: rectLocal,
                                    child: _builder!(context, animation)
                                )
                            ),
                        }
                    );
                }
                return new _ContextMenuRouteStatic__context_menu(
                    actions: _actions,
                    childGlobalKey: _childGlobalKey,
                    contextMenuLocation: _contextMenuLocation,
                    onDismiss: _onDismiss,
                    orientation: (orientation),
                    sheetGlobalKey: _sheetGlobalKey,
                    childRect: _previousChildRect,
                    child: _builder!(context, animation)
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        _curvedAnimation?.dispose();
        _sheetOpacityCurvedAnimation?.dispose();
        base.dispose();
    }
}

internal class _ContextMenuRouteStatic__context_menu : StatefulWidget
{
    public virtual List<Widget>? actions { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual GlobalKey<IState>? childGlobalKey { get; private set; }
    public virtual _ContextMenuLocation__context_menu contextMenuLocation { get; private set; } =
        default!;
    public virtual Action<BuildContext, double, double>? onDismiss { get; private set; }
    public virtual Orientation orientation { get; private set; } = default!;
    public virtual GlobalKey<IState>? sheetGlobalKey { get; private set; }
    public virtual Rect childRect { get; private set; } = default!;

    internal _ContextMenuRouteStatic__context_menu(
        List<Widget>? actions = null,
        Widget child = default!,
        GlobalKey<IState>? childGlobalKey = null,
        _ContextMenuLocation__context_menu contextMenuLocation = default!,
        Action<BuildContext, double, double>? onDismiss = null,
        Orientation orientation = default!,
        GlobalKey<IState>? sheetGlobalKey = null,
        Rect childRect = default!
    )
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

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _ContextMenuRouteStaticState__context_menu()
        );
}

internal class _ContextMenuRouteStaticState__context_menu
    : State<_ContextMenuRouteStatic__context_menu>,
        TickerProviderStateMixin<_ContextMenuRouteStatic__context_menu>
{
    internal const double _kMinScale = 0.8;
    internal const double _kSheetScaleThreshold = 0.9;
    internal const double _kPadding = 20.0;
    internal const double _kDamping = 400.0;
    internal static Duration _kMoveControllerDuration = Duration.Create(milliseconds: 600L);
    internal virtual Offset _dragOffset { get; set; } = default!;
    internal virtual double _lastScale { get; set; } = 1.0;
    internal virtual AnimationController _moveController { get; private set; } = default!;
    internal virtual CurvedAnimation _moveCurvedAnimation { get; private set; } = default!;
    internal virtual AnimationController _sheetController { get; private set; } = default!;
    internal virtual CurvedAnimation _sheetCurvedAnimation { get; private set; } = default!;
    internal virtual Animation<Offset> _moveAnimation { get; set; } = default!;
    internal virtual Animation<double> _sheetScaleAnimation { get; set; } = default!;
    internal virtual Animation<double> _sheetOpacityAnimation { get; set; } = default!;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal static double _getScale(Orientation orientation, double maxDragDistance, double dy)
    {
        double dyDirectional = (dy <= 0.0) ? dy : -dy;
        return Math.Max(_kMinScale, (maxDragDistance + dyDirectional) / maxDragDistance);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _onPanStart(Gestures.DragStartDetails details)
    {
        _moveController.value = 1.0;
        _setDragOffset(Offset.zero);
    }

    internal virtual void _onPanUpdate(Gestures.DragUpdateDetails details)
    {
        _setDragOffset(_dragOffset + details.delta);
    }

    internal virtual void _onPanEnd(Gestures.DragEndDetails details)
    {
        if (
            details.velocity.pixelsPerSecond.dy.abs() >= Gestures.ConstantsLibrary.kMinFlingVelocity
        )
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
            _moveAnimation = new Tween<Offset>(
                begin: new Offset(0.0, _moveAnimation.value.dy),
                end: new Offset(0.0, finalPosition)
            ).animate(_moveController);
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

    internal virtual void _flingStatusListener(AnimationStatus status)
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
        double endY =
            (dragOffset.dy >= 0.0)
                ? dragOffset.dy
                : (SliderLibrary._kPadding * dragOffset.dy / _kDamping);
        setState(() =>
        {
            _dragOffset = dragOffset;
            _moveAnimation = new Tween<Offset>(
                begin: Offset.zero,
                end: new Offset(
                    Dart_uiLibrary.clampDouble(
                        endX,
                        -SliderLibrary._kPadding,
                        SliderLibrary._kPadding
                    ),
                    endY
                )
            ).animate(_moveCurvedAnimation);
            if (
                (_lastScale <= _kSheetScaleThreshold)
                && (!Equals(_sheetController.status, AnimationStatus.forward))
                && (_sheetScaleAnimation.value != 0.0)
            )
            {
                _sheetController.forward();
            }
            else
            {
                if (
                    (_lastScale > _kSheetScaleThreshold)
                    && (!Equals(_sheetController.status, AnimationStatus.reverse))
                    && (_sheetScaleAnimation.value != 1.0)
                )
                {
                    _sheetController.reverse();
                }
            }
        });
    }

    internal virtual Widget _getChild(
        Orientation orientation,
        _ContextMenuLocation__context_menu contextMenuLocation
    )
    {
        Size screenSize = MediaQuery.sizeOf(context);
        EdgeInsets padding = MediaQuery.paddingOf(context);
        var screenBoundsLocal = Rect.fromLTWH(
            0,
            0,
            screenSize.width - padding.left - padding.right,
            screenSize.height - padding.top - padding.bottom
        );
        Widget sheetLocal = new AnimatedBuilder(
            animation: _sheetController,
            builder: _buildSheetAnimation,
            child: new _ContextMenuSheet__context_menu(
                key: widget.sheetGlobalKey,
                actions: widget.actions!,
                contextMenuLocation: widget.contextMenuLocation,
                orientation: widget.orientation
            )
        );
        Widget childLocal = new _ContextMenuAlignedChildren__context_menu(
            targetRect: widget.childRect,
            screenBounds: screenBoundsLocal,
            sheet: sheetLocal,
            contextMenuLocation: contextMenuLocation,
            orientation: widget.orientation,
            child: new AnimatedBuilder(
                animation: _moveController,
                builder: _buildChildAnimation,
                child: widget.child
            )
        );
        return childLocal;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildSheetAnimation(BuildContext context, Widget? child)
    {
        return Transform.CreateScale(
            alignment: _ContextMenuRoute__context_menu<object>.getSheetAlignment(
                widget.contextMenuLocation,
                widget.orientation
            ),
            scale: _sheetScaleAnimation.value,
            child: new FadeTransition(opacity: _sheetOpacityAnimation, child: child)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildChildAnimation(BuildContext context, Widget? child)
    {
        _lastScale = _getScale(
            widget.orientation,
            MediaQuery.heightOf(context),
            _moveAnimation.value.dy
        );
        return Transform.CreateScale(key: widget.childGlobalKey, scale: _lastScale, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildAnimation(BuildContext context, Widget? child)
    {
        return Transform.CreateTranslate(offset: _moveAnimation.value, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void initState()
    {
        base.initState();
        _moveController = new AnimationController(
            duration: _kMoveControllerDuration,
            value: 1.0,
            vsync: this
        );
        _moveCurvedAnimation = new CurvedAnimation(
            parent: _moveController,
            curve: Curves.elasticIn
        );
        _sheetController = new AnimationController(
            duration: Duration.Create(milliseconds: 100L),
            reverseDuration: Duration.Create(milliseconds: 300L),
            vsync: this
        );
        _sheetCurvedAnimation = new CurvedAnimation(
            parent: _sheetController,
            curve: Curves.linear,
            reverseCurve: Curves.easeInBack
        );
        _sheetScaleAnimation = new Tween<double>(begin: 1.0, end: 0.0).animate(
            _sheetCurvedAnimation
        );
        _sheetOpacityAnimation = new Tween<double>(begin: 1.0, end: 0.0).animate(_sheetController);
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
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        Widget childLocal = _getChild(widget.orientation, widget.contextMenuLocation);
        return new SafeArea(
            child: new Align(
                alignment: Alignment.topLeft,
                child: new GestureDetector(
                    onPanEnd: _onPanEnd,
                    onPanStart: _onPanStart,
                    onPanUpdate: _onPanUpdate,
                    child: new AnimatedBuilder(
                        animation: _moveController,
                        builder: _buildAnimation,
                        child: childLocal
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
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
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

internal class _ContextMenuSheet__context_menu : StatefulWidget
{
    public virtual List<Widget> actions { get; private set; } = default!;
    public virtual _ContextMenuLocation__context_menu contextMenuLocation { get; private set; } =
        default!;
    public virtual Orientation orientation { get; private set; } = default!;

    internal _ContextMenuSheet__context_menu(
        Key? key = null,
        List<Widget> actions = default!,
        _ContextMenuLocation__context_menu contextMenuLocation = default!,
        Orientation orientation = default!
    )
        : base(key: key)
    {
        this.actions = actions;
        this.contextMenuLocation = contextMenuLocation;
        this.orientation = orientation;
        System.Diagnostics.Debug.Assert(Enumerable.Any(actions));
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _ContextMenuSheetState__context_menu());
}

internal class _ContextMenuSheetState__context_menu : State<_ContextMenuSheet__context_menu>
{
    internal virtual ScrollController _controller { get; private set; } = default!;
    internal const double _kMenuWidth = 250.0;
    internal const double _kScrollbarMainAxisMargin = 13.0;

    public override void initState()
    {
        base.initState();
        _controller = new ScrollController();
    }

    public override void dispose()
    {
        _controller.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new SizedBox(
            width: _kMenuWidth,
            child: new IntrinsicHeight(
                child: new ClipRSuperellipse(
                    borderRadius: BorderRadius.CreateAll(Radius.circular(13.0)),
                    child: new ColoredBox(
                        color: CupertinoDynamicColor.resolve(
                            CupertinoContextMenu.kBackgroundColor,
                            context
                        ),
                        child: new ScrollConfiguration(
                            behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false),
                            child: new CupertinoScrollbar(
                                mainAxisMargin: _kScrollbarMainAxisMargin,
                                controller: _controller,
                                child: new SingleChildScrollView(
                                    controller: _controller,
                                    child: new Column(
                                        crossAxisAlignment: CrossAxisAlignment.stretch,
                                        children: (
                                            (Func<List<Widget>>)(
                                                () =>
                                                {
                                                    var __collection49409 = new List<Widget>();
                                                    __collection49409.Add(
                                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                                            widget.actions.First()
                                                        )
                                                    );
                                                    foreach (var action in widget.actions.skip(1L))
                                                    {
                                                        __collection49409.Add(
                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                new DecoratedBox(
                                                                    decoration: new BoxDecoration(
                                                                        border: new Border(
                                                                            top: new BorderSide(
                                                                                color: CupertinoDynamicColor.resolve(
                                                                                    Context_menuLibrary._borderColor,
                                                                                    context
                                                                                ),
                                                                                width: 0.4
                                                                            )
                                                                        )
                                                                    ),
                                                                    position: DecorationPosition.foreground,
                                                                    child: action
                                                                )
                                                            )
                                                        );
                                                    }
                                                    return __collection49409;
                                                }
                                            )
                                        )()
                                    )
                                )
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal enum _ContextMenuChild__context_menu
{
    child,
    menuSheet,
}

internal class _ContextMenuAlignedChildren__context_menu : StatelessWidget
{
    public virtual Rect targetRect { get; private set; } = default!;
    public virtual Rect screenBounds { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    public virtual Widget sheet { get; private set; } = default!;
    public virtual Orientation orientation { get; private set; } = default!;
    public virtual _ContextMenuLocation__context_menu contextMenuLocation { get; private set; } =
        default!;

    internal _ContextMenuAlignedChildren__context_menu(
        Rect targetRect,
        Rect screenBounds,
        Widget child,
        Widget sheet,
        Orientation orientation,
        _ContextMenuLocation__context_menu contextMenuLocation
    )
    {
        this.targetRect = targetRect;
        this.screenBounds = screenBounds;
        this.child = child;
        this.sheet = sheet;
        this.orientation = orientation;
        this.contextMenuLocation = contextMenuLocation;
    }

    public override Widget build(BuildContext context)
    {
        return new CustomMultiChildLayout(
            @delegate: new _ContextMenuAlignedChildrenDelegate__context_menu(
                targetRect: targetRect,
                screenBounds: screenBounds,
                orientation: orientation,
                contextMenuLocation: contextMenuLocation
            ),
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new LayoutId(id: _ContextMenuChild__context_menu.child, child: child)
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new LayoutId(id: _ContextMenuChild__context_menu.menuSheet, child: sheet)
                ),
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _ContextMenuAlignedChildrenDelegate__context_menu : MultiChildLayoutDelegate
{
    public virtual Rect targetRect { get; private set; } = default!;
    public virtual Rect screenBounds { get; private set; } = default!;
    public virtual Orientation orientation { get; private set; } = default!;
    public virtual _ContextMenuLocation__context_menu contextMenuLocation { get; private set; } =
        default!;

    internal _ContextMenuAlignedChildrenDelegate__context_menu(
        Rect targetRect,
        Rect screenBounds,
        Orientation orientation,
        _ContextMenuLocation__context_menu contextMenuLocation
    )
    {
        this.targetRect = targetRect;
        this.screenBounds = screenBounds;
        this.orientation = orientation;
        this.contextMenuLocation = contextMenuLocation;
    }

    public override void performLayout(Size size)
    {
        var constraints = BoxConstraints.CreateLoose(size);
        double availableHeightForChild =
            screenBounds.height - _ContextMenuRouteStaticState__context_menu._kPadding;
        double availableWidth =
            screenBounds.width - (_ContextMenuRouteStaticState__context_menu._kPadding * 2L);
        double availableWidthForChild = orientation switch
        {
            Orientation.portrait => availableWidth,
            Orientation.landscape => availableWidth
                - _ContextMenuSheetState__context_menu._kMenuWidth,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        DartRuntimePrimitives.Assert(() => availableWidthForChild >= 0.0);
        DartRuntimePrimitives.Assert(() => availableHeightForChild >= 0.0);
        Size childSize = layoutChild(
            _ContextMenuChild__context_menu.child,
            constraints.copyWith(
                maxHeight: availableHeightForChild,
                maxWidth: availableWidthForChild
            )
        );
        double availableHeightForMenu = orientation switch
        {
            Orientation.portrait => availableHeightForChild
                - (childSize.height + _ContextMenuRouteStaticState__context_menu._kPadding),
            Orientation.landscape => availableHeightForChild,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        Size menuSize = layoutChild(
            _ContextMenuChild__context_menu.menuSheet,
            constraints.copyWith(maxHeight: availableHeightForMenu)
        );
        double initialChildLeft = default!;
        double initialChildTop = default!;
        double maxClampedLeft = default!;
        double maxClampedTop = default!;
        Offset secondChildOffset = default!;
        bool menuBeforeChild = default!;
        switch (orientation)
        {
            case Orientation.portrait:
            {
                menuBeforeChild = false;
                double totalHeight =
                    childSize.height
                    + menuSize.height
                    + _ContextMenuRouteStaticState__context_menu._kPadding;
                double totalWidth =
                    childSize.width + _ContextMenuRouteStaticState__context_menu._kPadding;
                initialChildLeft = targetRect.center.dx - (childSize.width / 2L);
                initialChildTop = targetRect.center.dy - childSize.height;
                double secondChildDx = contextMenuLocation switch
                {
                    _ContextMenuLocation__context_menu.center => (childSize.width / 2L)
                        - (menuSize.width / 2L),
                    _ContextMenuLocation__context_menu.left => 0.0,
                    _ContextMenuLocation__context_menu.right => childSize.width - menuSize.width,
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                };
                secondChildOffset = new Offset(
                    secondChildDx,
                    childSize.height + _ContextMenuRouteStaticState__context_menu._kPadding
                );
                maxClampedLeft = screenBounds.right - totalWidth;
                maxClampedTop = screenBounds.bottom - totalHeight;
                break;
            }
            case Orientation.landscape:
            {
                menuBeforeChild = Equals(
                    contextMenuLocation,
                    _ContextMenuLocation__context_menu.right
                );
                double totalWidthLocal =
                    childSize.width
                    + menuSize.width
                    + _ContextMenuRouteStaticState__context_menu._kPadding;
                initialChildLeft = screenBounds.center.dx - (totalWidthLocal / 2L);
                initialChildTop =
                    screenBounds.center.dy - (Math.Max(childSize.height, menuSize.height) / 2L);
                double secondChildDxLocal = menuBeforeChild ? menuSize.width : childSize.width;
                secondChildOffset = new Offset(
                    secondChildDxLocal + _ContextMenuRouteStaticState__context_menu._kPadding,
                    0.0
                );
                maxClampedLeft = screenBounds.right - totalWidthLocal;
                maxClampedTop = screenBounds.bottom;
                break;
            }
        }
        double clampedLeft = Dart_uiLibrary.clampDouble(
            initialChildLeft,
            screenBounds.left + _ContextMenuRouteStaticState__context_menu._kPadding,
            maxClampedLeft
        );
        double clampedTop = Dart_uiLibrary.clampDouble(
            initialChildTop,
            screenBounds.top + _ContextMenuRouteStaticState__context_menu._kPadding,
            maxClampedTop
        );
        var firstPosition = new Offset(clampedLeft, clampedTop);
        Offset secondPosition = firstPosition + secondChildOffset;
        positionChild(
            _ContextMenuChild__context_menu.child,
            menuBeforeChild ? secondPosition : firstPosition
        );
        positionChild(
            _ContextMenuChild__context_menu.menuSheet,
            menuBeforeChild ? firstPosition : secondPosition
        );
    }

    public override bool shouldRelayout(MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_ContextMenuAlignedChildrenDelegate__context_menu)oldDelegate;
        return (!Equals(__oldDelegate.targetRect, targetRect))
            || (!Equals(__oldDelegate.screenBounds, screenBounds))
            || (!Equals(__oldDelegate.orientation, orientation))
            || (!Equals(__oldDelegate.contextMenuLocation, contextMenuLocation));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
