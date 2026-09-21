// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/menu_anchor.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Menu_anchorLibrary
{
    internal static DartMap<ShortcutActivator, Intent> _kMenuTraversalShortcuts = new DartMap<
        ShortcutActivator,
        Intent
    >
    {
        [new SingleActivator(LogicalKeyboardKey.arrowUp)] = new _FocusUpIntent__menu_anchor(),
        [new SingleActivator(LogicalKeyboardKey.arrowDown)] = new _FocusDownIntent__menu_anchor(),
        [new SingleActivator(LogicalKeyboardKey.home)] = new _FocusFirstIntent__menu_anchor(),
        [new SingleActivator(LogicalKeyboardKey.end)] = new _FocusLastIntent__menu_anchor(),
    };
}

public static partial class Menu_anchorLibrary
{
    internal static bool _isCupertino
    {
        get
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                {
                    return true;
                }
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                {
                    return false;
                }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
    }
}

public static partial class Menu_anchorLibrary
{
    internal static string _kBodyFont = "CupertinoSystemText";
}

public static partial class Menu_anchorLibrary
{
    internal static string _kDisplayFont = "CupertinoSystemDisplay";
}

public static partial class Menu_anchorLibrary
{
    internal static double _kCupertinoMobileBaseFontSize = 17.0;
}

public static partial class Menu_anchorLibrary
{
    internal static double _normalizeTextScale(TextScaler textScaler)
    {
        if (Equals(textScaler, TextScaler.noScaling))
        {
            return 0;
        }
        return textScaler.scale(_kCupertinoMobileBaseFontSize) - _kCupertinoMobileBaseFontSize;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Menu_anchorLibrary
{
    internal static double _kMinimumNormalizedLargeTextScale = 11;
}

public static partial class Menu_anchorLibrary
{
    internal static double _kMinimumTextScaleFactor = 1L - (3L / _kCupertinoMobileBaseFontSize);
}

public static partial class Menu_anchorLibrary
{
    internal static double _kMaximumTextScaleFactor = 1L + (36L / _kCupertinoMobileBaseFontSize);
}

public static partial class Menu_anchorLibrary
{
    internal static bool _largeTextModeEnabled(BuildContext context)
    {
        TextScaler? textScaler = MediaQuery.maybeTextScalerOf(context);
        if (textScaler is null)
        {
            return false;
        }
        return _normalizeTextScale(textScaler) >= _kMinimumNormalizedLargeTextScale;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal enum _CupertinoMenuWidth__menu_anchor
{
    iPadOS,
    iPadOSAccessible,
    iOS,
    iOSAccessible,
}

internal static class _CupertinoMenuWidth__menu_anchorMembers
{
    internal static double points(this _CupertinoMenuWidth__menu_anchor value) =>
        value switch
        {
            _CupertinoMenuWidth__menu_anchor.iPadOS => 262.0,
            _CupertinoMenuWidth__menu_anchor.iPadOSAccessible => 343.0,
            _CupertinoMenuWidth__menu_anchor.iOS => 250.0,
            _ => 370.0,
        };

    internal static _CupertinoMenuWidth__menu_anchor CreateFromScreenWidth(
        bool isLargeTextModeEnabled,
        double screenWidth
    ) =>
        screenWidth >= 768.0
            ? (
                isLargeTextModeEnabled
                    ? _CupertinoMenuWidth__menu_anchor.iPadOSAccessible
                    : _CupertinoMenuWidth__menu_anchor.iPadOS
            )
            : (
                isLargeTextModeEnabled
                    ? _CupertinoMenuWidth__menu_anchor.iOSAccessible
                    : _CupertinoMenuWidth__menu_anchor.iOS
            );
}

internal enum _DynamicTypeStyle__menu_anchor
{
    body,
    subhead,
}

internal static class _DynamicTypeStyle__menu_anchorMembers
{
    private const long _kScaleCount = 12;
    private static readonly List<long> _normalizedBodyScales = new()
    {
        -3,
        -2,
        -1,
        0,
        2,
        4,
        6,
        11,
        16,
        23,
        30,
        36,
    };
    private static readonly double[] _bodySizes =
    {
        14,
        15,
        16,
        17,
        19,
        21,
        23,
        28,
        33,
        40,
        47,
        53,
    };
    private static readonly double[] _subheadSizes =
    {
        12,
        13,
        14,
        15,
        17,
        19,
        21,
        26,
        31,
        38,
        45,
        51,
    };

    private static List<TextStyle> styles(this _DynamicTypeStyle__menu_anchor value) =>
        (value == _DynamicTypeStyle__menu_anchor.body ? _bodySizes : _subheadSizes)
            .Select(size => new TextStyle(fontSize: size))
            .ToList();

    private static double _interpolateUnits(double value, double min, double max) =>
        (value - min) / (max - min);

    public static TextStyle resolveTextStyle(
        this _DynamicTypeStyle__menu_anchor value,
        TextScaler textScaler
    )
    {
        DartRuntimePrimitives.Assert(() => checked(value.styles().Count) == _kScaleCount);
        double units = Menu_anchorLibrary._normalizeTextScale(textScaler);
        for (var i = 0L; i < checked(value.styles().Count); i++)
        {
            long bodyUnits = _normalizedBodyScales[(int)i];
            if (units > bodyUnits)
            {
                continue;
            }
            if (units == bodyUnits)
            {
                return value.styles()[(int)i];
            }
            if (i == 0L)
            {
                return value.styles().First();
            }
            return TextStyle.lerp(
                value.styles()[(int)(i - 1L)],
                value.styles()[(int)i],
                _interpolateUnits(units, _normalizedBodyScales[(int)(i - 1L)], bodyUnits)
            )!;
        }
        return value.styles().Last();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Menu_anchorLibrary
{
    internal static double _computeSquaredDistanceToRect(Offset point, Rect rect)
    {
        double dxLocal = point.dx - Dart_uiLibrary.clampDouble(point.dx, rect.left, rect.right);
        double dyLocal = point.dy - Dart_uiLibrary.clampDouble(point.dy, rect.top, rect.bottom);
        return (dxLocal * dxLocal) + (dyLocal * dyLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Menu_anchorLibrary
{
    internal static double _roundToDivisible(double value, double to)
    {
        if (to == 0L)
        {
            return value;
        }
        return (value / to).round() * to;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public interface CupertinoMenuEntry
{
    public bool hasLeading(BuildContext context);
    public bool isDivider { get; }
}

internal class _AnchorScope__menu_anchor : InheritedWidget
{
    public virtual bool hasLeading { get; private set; } = default!;

    internal _AnchorScope__menu_anchor(bool hasLeading, Widget child)
        : base(child: child)
    {
        this.hasLeading = hasLeading;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_AnchorScope__menu_anchor)oldWidget;
        return hasLeading != __oldWidget.hasLeading;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate void CupertinoMenuAnimationStatusChangedCallback(AnimationStatus status);

public class CupertinoMenuAnchor : StatefulWidget
{
    public virtual MenuController? controller { get; private set; }
    public virtual Action? onOpen { get; private set; }
    public virtual Action? onClose { get; private set; }
    public virtual AnimationStatusListener? onAnimationStatusChanged { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual bool constrainCrossAxis { get; private set; } = default!;
    public virtual bool consumeOutsideTaps { get; private set; } = default!;
    public virtual bool enableSwipe { get; private set; } = default!;
    public virtual bool enableLongPressToOpen { get; private set; } = default!;
    public virtual bool useRootOverlay { get; private set; } = default!;
    public virtual EdgeInsetsGeometry overlayPadding { get; private set; } = default!;
    public virtual List<Widget> menuChildren { get; private set; } = default!;
    public virtual Func<BuildContext, MenuController, Widget?, Widget>? builder
    {
        get;
        private set;
    }
    public virtual Widget? child { get; private set; }
    public virtual FocusNode? childFocusNode { get; private set; }

    public CupertinoMenuAnchor(
        Key? key = null,
        MenuController? controller = null,
        Action? onOpen = null,
        Action? onClose = null,
        AnimationStatusListener? onAnimationStatusChanged = null,
        BoxConstraints? constraints = null,
        bool constrainCrossAxis = false,
        bool consumeOutsideTaps = false,
        bool enableSwipe = true,
        bool enableLongPressToOpen = false,
        bool useRootOverlay = false,
        EdgeInsetsGeometry overlayPadding = default!,
        List<Widget> menuChildren = default!,
        Func<BuildContext, MenuController, Widget?, Widget>? builder = null,
        Widget? child = null,
        FocusNode? childFocusNode = null
    )
        : base(key: key)
    {
        EdgeInsetsGeometry __overlayPadding = overlayPadding ?? EdgeInsets.CreateAll(8);
        this.controller = controller;
        this.onOpen = onOpen;
        this.onClose = onClose;
        this.onAnimationStatusChanged = onAnimationStatusChanged;
        this.constraints = constraints;
        this.constrainCrossAxis = constrainCrossAxis;
        this.consumeOutsideTaps = consumeOutsideTaps;
        this.enableSwipe = enableSwipe;
        this.enableLongPressToOpen = enableLongPressToOpen;
        this.useRootOverlay = useRootOverlay;
        this.overlayPadding = __overlayPadding;
        this.menuChildren = menuChildren;
        this.builder = builder;
        this.child = child;
        this.childFocusNode = childFocusNode;
        System.Diagnostics.Debug.Assert(enableSwipe || !enableLongPressToOpen);
    }

    public static bool? maybeHasLeadingOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_AnchorScope__menu_anchor>()?.hasLeading;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoMenuAnchorState__menu_anchor());

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return menuChildren.map((child) => ((Diagnosticable)child).toDiagnosticsNode()).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<FocusNode?>("childFocusNode", childFocusNode));
        properties.add(new DiagnosticsProperty<BoxConstraints?>("constraints", constraints));
        properties.add(
            new FlagProperty(
                "constrainCrossAxis",
                value: constrainCrossAxis,
                ifTrue: "constrains cross axis"
            )
        );
        properties.add(
            new FlagProperty(
                "enableSwipe",
                value: enableSwipe,
                ifTrue: "swipe enabled",
                ifFalse: "swipe disabled"
            )
        );
        properties.add(
            new FlagProperty(
                "consumeOutsideTaps",
                value: consumeOutsideTaps,
                ifTrue: "consumes outside taps"
            )
        );
        properties.add(
            new FlagProperty("useRootOverlay", value: useRootOverlay, ifTrue: "uses root overlay")
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>("overlayPadding", overlayPadding)
        );
    }
}

internal class _CupertinoMenuAnchorState__menu_anchor
    : State<CupertinoMenuAnchor>,
        TickerProviderStateMixin<CupertinoMenuAnchor>
{
    internal static Duration _kLongPressToOpenDuration = Duration.Create(milliseconds: 400L);
    internal static Physics.Tolerance _kSpringTolerance = new Physics.Tolerance(velocity: 0.1);
    public static Physics.SpringDescription forwardSpring =
        Physics.SpringDescription.CreateWithDurationAndBounce(
            duration: Duration.Create(milliseconds: 337L),
            bounce: 0.2
        );
    public static Physics.SpringDescription reverseSpring =
        Physics.SpringDescription.CreateWithDurationAndBounce(
            duration: Duration.Create(milliseconds: 409L)
        );
    internal virtual AnimationController _animationController { get; private set; } = default!;
    internal virtual FocusScopeNode _menuScopeNode { get; private set; } =
        new FocusScopeNode(debugLabel: "Menu Scope");
    internal virtual ValueNotifier<double> _swipeDistanceNotifier { get; private set; } =
        new ValueNotifier<double>(0);
    internal virtual bool? _hasLeadingWidget { get; set; } = default;
    internal virtual MenuController? _internalMenuController { get; set; } = default;
    internal virtual AnimationStatus _animationStatus { get; set; } = AnimationStatus.dismissed;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual MenuController _menuController =>
        DartRuntimePrimitives.ConvertValue<MenuController>(
            widget.controller ?? _internalMenuController!
        );
    public virtual bool isOpenOrOpening =>
        AnimationStatusMembers.isForwardOrCompleted(_animationStatus);
    public virtual bool enableSwipe =>
        DartRuntimePrimitives.ConvertValue<bool>(
            widget.enableSwipe
                && (
                    _animationStatus switch
                    {
                        AnimationStatus.forward or AnimationStatus.completed => true,
                        AnimationStatus.dismissed => true,
                        AnimationStatus.reverse => false,
                        _ => throw new InvalidOperationException(
                            "Non-exhaustive Dart switch value."
                        ),
                    }
                )
        );

    public override void initState()
    {
        base.initState();
        if (widget.controller is null)
        {
            _internalMenuController = new MenuController();
        }
        _animationController = AnimationController.CreateUnbounded(vsync: this);
        _animationController.addStatusListener(_handleAnimationStatusChange);
    }

    public override void didUpdateWidget(CupertinoMenuAnchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.controller, widget.controller))
        {
            if (widget.controller is not null)
            {
                _internalMenuController = null;
            }
            else
            {
                DartRuntimePrimitives.Assert(() => _internalMenuController is null);
                _internalMenuController = new MenuController();
            }
        }
        if (!Equals(oldWidget.menuChildren, widget.menuChildren))
        {
            _hasLeadingWidget = _resolveHasLeading();
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _hasLeadingWidget ??= _resolveHasLeading();
    }

    public override void dispose()
    {
        _menuScopeNode.dispose();
        DartRuntimePrimitives.Ignore(
            (
                (Func<AnimationController>)(
                    () =>
                    {
                        var __cascade = _animationController;
                        __cascade.stop();
                        __cascade.dispose();
                        return __cascade;
                    }
                )
            )()
        );
        _internalMenuController = null;
        _swipeDistanceNotifier.dispose();
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

    internal virtual bool _resolveHasLeading()
    {
        return widget.menuChildren.any(
            (element) =>
            {
                return element switch
                {
                    CupertinoMenuEntry entry => entry.hasLeading(context),
                    _ => false,
                };
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleAnimationStatusChange(AnimationStatus status)
    {
        setState(() =>
        {
            _animationStatus = status;
        });
        widget.onAnimationStatusChanged?.Invoke(status);
    }

    internal virtual void _handleSwipeDistanceChange(double distance)
    {
        if (!_menuController.isOpen)
        {
            return;
        }
        _swipeDistanceNotifier.value = distance;
    }

    internal virtual void _handleAnchorSwipeStart()
    {
        if (isOpenOrOpening || !widget.enableLongPressToOpen)
        {
            return;
        }
        _menuController.open();
    }

    internal virtual void _handleCloseRequested(Action hideMenu)
    {
        if (_animationStatus is AnimationStatus.reverse or AnimationStatus.dismissed)
        {
            return;
        }
        DartRuntimePrimitives.Ignore(
            _animationController
                .animateBackWith(
                    new Physics.ClampedSimulation(
                        new Physics.SpringSimulation(
                            reverseSpring,
                            _animationController.value,
                            0.0,
                            0.0,
                            tolerance: _kSpringTolerance
                        ),
                        xMin: 0.0,
                        xMax: 1.0
                    )
                )
                .whenComplete(() =>
                {
                    hideMenu();
                    return default!;
                })
        );
    }

    internal virtual void _handleOpenRequested(Offset? position, Action showOverlay)
    {
        showOverlay();
        if (_animationStatus is AnimationStatus.completed or AnimationStatus.forward)
        {
            return;
        }
        _animationController.animateWith(
            new Physics.SpringSimulation(forwardSpring, _animationController.value, 1, 0.5)
        );
        FocusScope.of(context).setFirstFocus(_menuScopeNode);
    }

    internal virtual Widget _buildMenuOverlay(BuildContext childContext, RawMenuOverlayInfo info)
    {
        return new ExcludeSemantics(
            excluding: !isOpenOrOpening,
            child: new IgnorePointer(
                ignoring: !isOpenOrOpening,
                child: new ExcludeFocus(
                    excluding: !isOpenOrOpening,
                    child: new _MenuOverlay__menu_anchor(
                        constrainCrossAxis: widget.constrainCrossAxis,
                        visibilityAnimation: _animationController.view,
                        swipeDistanceListenable: _swipeDistanceNotifier,
                        constraints: widget.constraints,
                        consumeOutsideTaps: widget.consumeOutsideTaps,
                        overlaySize: info.overlaySize,
                        anchorRect: info.anchorRect,
                        anchorPosition: info.position,
                        tapRegionGroupId: info.tapRegionGroupId,
                        focusScopeNode: _menuScopeNode,
                        overlayPadding: widget.overlayPadding,
                        children: widget.menuChildren
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildChild(
        BuildContext context,
        MenuController controller,
        Widget? child
    )
    {
        Widget anchor =
            (
                widget.builder is null
                    ? widget.child
                    : widget.builder.Invoke(context, _menuController, widget.child)
            ) ?? SizedBox.CreateShrink();
        if (!widget.enableLongPressToOpen || !enableSwipe)
        {
            return anchor;
        }
        return new _SwipeSurface__menu_anchor(
            onStart: () => _handleAnchorSwipeStart(),
            delay: _kLongPressToOpenDuration,
            child: anchor
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new _SwipeRegion__menu_anchor(
            onDistanceChanged: _handleSwipeDistanceChange,
            enabled: enableSwipe,
            child: new _AnchorScope__menu_anchor(
                hasLeading: DartRuntimePrimitives.RequireValue(_hasLeadingWidget),
                child: new RawMenuAnchor(
                    useRootOverlay: widget.useRootOverlay,
                    onCloseRequested: _handleCloseRequested,
                    onOpenRequested: _handleOpenRequested,
                    overlayBuilder: _buildMenuOverlay,
                    builder: _buildChild,
                    controller: _menuController,
                    childFocusNode: widget.childFocusNode,
                    consumeOutsideTaps: widget.consumeOutsideTaps,
                    onClose: widget.onClose,
                    onOpen: widget.onOpen
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

public class _MenuOverlay__menu_anchor : StatefulWidget
{
    public virtual List<Widget> children { get; private set; } = default!;
    public virtual FocusScopeNode focusScopeNode { get; private set; } = default!;
    public virtual bool consumeOutsideTaps { get; private set; } = default!;
    public virtual bool constrainCrossAxis { get; private set; } = default!;
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual Size overlaySize { get; private set; } = default!;
    public virtual EdgeInsetsGeometry overlayPadding { get; private set; } = default!;
    public virtual Rect anchorRect { get; private set; } = default!;
    public virtual Offset? anchorPosition { get; private set; }
    public virtual object tapRegionGroupId { get; private set; } = default!;
    public virtual Animation<double> visibilityAnimation { get; private set; } = default!;
    public virtual ValueListenable<double> swipeDistanceListenable { get; private set; } = default!;

    internal _MenuOverlay__menu_anchor(
        List<Widget> children,
        FocusScopeNode focusScopeNode,
        bool consumeOutsideTaps,
        bool constrainCrossAxis,
        BoxConstraints? constraints,
        Size overlaySize,
        EdgeInsetsGeometry overlayPadding,
        Rect anchorRect,
        Offset? anchorPosition,
        object tapRegionGroupId,
        Animation<double> visibilityAnimation,
        ValueListenable<double> swipeDistanceListenable
    )
    {
        this.children = children;
        this.focusScopeNode = focusScopeNode;
        this.consumeOutsideTaps = consumeOutsideTaps;
        this.constrainCrossAxis = constrainCrossAxis;
        this.constraints = constraints;
        this.overlaySize = overlaySize;
        this.overlayPadding = overlayPadding;
        this.anchorRect = anchorRect;
        this.anchorPosition = anchorPosition;
        this.tapRegionGroupId = tapRegionGroupId;
        this.visibilityAnimation = visibilityAnimation;
        this.swipeDistanceListenable = swipeDistanceListenable;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _MenuOverlayState__menu_anchor());
}

internal class _MenuOverlayState__menu_anchor
    : State<_MenuOverlay__menu_anchor>,
        TickerProviderStateMixin<_MenuOverlay__menu_anchor>,
        WidgetsBindingObserver
{
    internal static Offset _kAttachmentOffset = new Offset(0, 8);
    internal static DartMap<Type, dynamic> _kActions = new DartMap<Type, dynamic>
    {
        [typeof(_FocusDownIntent__menu_anchor)] = new _FocusDownAction__menu_anchor(),
        [typeof(_FocusUpIntent__menu_anchor)] = new _FocusUpAction__menu_anchor(),
        [typeof(_FocusFirstIntent__menu_anchor)] = new _FocusFirstAction__menu_anchor(),
        [typeof(_FocusLastIntent__menu_anchor)] = new _FocusLastAction__menu_anchor(),
    };
    internal virtual AnimationController _swipeAnimationController { get; private set; } = default!;
    internal virtual ScrollController _scrollController { get; private set; } =
        new ScrollController();
    internal virtual ProxyAnimation _scaleAnimation { get; private set; } = new ProxyAnimation();
    internal virtual ProxyAnimation _fadeAnimation { get; private set; } = new ProxyAnimation();
    internal virtual ProxyAnimation _sizeAnimation { get; private set; } = new ProxyAnimation();
    internal virtual Alignment _attachmentPointAlignment { get; set; } = default!;
    internal virtual Offset _attachmentPoint { get; set; } = default!;
    internal virtual Alignment _menuAlignment { get; set; } = default!;
    internal virtual List<Widget> _children { get; set; } = new List<Widget>();
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual double _swipeTargetDistance { get; set; } = 0;
    internal virtual double _swipeCurrentDistance { get; set; } = 0;
    internal virtual double _swipeVelocity { get; set; } = 0;
    internal virtual Scheduler.Ticker? _swipeTicker { get; set; } = default;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        WidgetsBinding.instance.addObserver(this);
        _swipeAnimationController = AnimationController.CreateUnbounded(value: 1, vsync: this);
        widget.swipeDistanceListenable.addListener(_handleSwipeDistanceChanged);
        _resolveChildren();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        TextDirection newTextDirection = Directionality.of(context);
        if (!Equals(_textDirection, newTextDirection))
        {
            _textDirection = newTextDirection;
            _resolvePosition();
        }
        _resolveMotion();
    }

    public override void didUpdateWidget(_MenuOverlay__menu_anchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.swipeDistanceListenable, widget.swipeDistanceListenable))
        {
            oldWidget.swipeDistanceListenable.removeListener(_handleSwipeDistanceChanged);
            widget.swipeDistanceListenable.addListener(_handleSwipeDistanceChanged);
        }
        if (!Equals(oldWidget.visibilityAnimation, widget.visibilityAnimation))
        {
            _resolveMotion();
        }
        if (
            (!Equals(oldWidget.anchorRect, widget.anchorRect))
            || (!Equals(oldWidget.anchorPosition, widget.anchorPosition))
            || (!Equals(oldWidget.overlaySize, widget.overlaySize))
        )
        {
            _resolvePosition();
        }
        if (!Equals(oldWidget.children, widget.children))
        {
            _resolveChildren();
        }
    }

    public override void didChangeAccessibilityFeatures()
    {
        base.didChangeAccessibilityFeatures();
        _resolveMotion();
    }

    public override void dispose()
    {
        _scrollController.dispose();
        widget.swipeDistanceListenable.removeListener(_handleSwipeDistanceChanged);
        DartRuntimePrimitives.Ignore(
            (
                (Func<Scheduler.Ticker?>)(
                    () =>
                    {
                        var __cascade = _swipeTicker;
                        __cascade?.stop();
                        __cascade?.dispose();
                        return __cascade;
                    }
                )
            )()
        );
        DartRuntimePrimitives.Ignore(
            (
                (Func<AnimationController>)(
                    () =>
                    {
                        var __cascade = _swipeAnimationController;
                        __cascade.stop();
                        __cascade.dispose();
                        return __cascade;
                    }
                )
            )()
        );
        _scaleAnimation.parent = null;
        _fadeAnimation.parent = null;
        _sizeAnimation.parent = null;
        WidgetsBinding.instance.removeObserver(this);
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

    internal virtual void _resolveChildren()
    {
        if (!Enumerable.Any(widget.children))
        {
            _children = new List<Widget>();
            return;
        }
        var childrenLocal = new List<Widget>();
        Widget child = widget.children.First();
        for (var i = 0L; i < checked(widget.children.Count); i++)
        {
            childrenLocal.Add(child);
            if (Equals(child, widget.children.Last()))
            {
                break;
            }
            if (child is CupertinoMenuEntry { isDivider: true } __object31724)
            {
                child = widget.children[(int)(i + 1L)];
                continue;
            }
            child = widget.children[(int)(i + 1L)];
            if (child is CupertinoMenuEntry { isDivider: true } __object31889)
            {
                continue;
            }
            childrenLocal.Add(new _CupertinoMenuImplicitDivider__menu_anchor());
        }
        _children = childrenLocal;
    }

    internal virtual void _resolveMotion()
    {
        AccessibilityFeatures accessibilityFeaturesLocal = View.of(
            context
        ).platformDispatcher.accessibilityFeatures;
        switch (accessibilityFeaturesLocal)
        {
            case AccessibilityFeatures { disableAnimations: true } __object32475:
            {
                _scaleAnimation.parent = AnimationsLibrary.kAlwaysCompleteAnimation;
                _fadeAnimation.parent = AnimationsLibrary.kAlwaysCompleteAnimation;
                _sizeAnimation.parent = AnimationsLibrary.kAlwaysCompleteAnimation;
                break;
            }
            case AccessibilityFeatures { reduceMotion: true } __object32712:
            {
                _scaleAnimation.parent = _swipeAnimationController.view.drive(
                    new Tween<double>(begin: 0.8, end: 1)
                );
                _sizeAnimation.parent = AnimationsLibrary.kAlwaysCompleteAnimation;
                _fadeAnimation.parent = widget.visibilityAnimation.drive(
                    new CurveTween(curve: Curves.easeIn).chain(
                        new _ClampTween__menu_anchor(begin: 0, end: 1)
                    )
                );
                break;
            }
            default:
            {
                _scaleAnimation.parent = DartRuntimePrimitives.ConvertValue<Animation<double>>(
                    new _AnimationProduct__menu_anchor(
                        first: widget.visibilityAnimation,
                        next: _swipeAnimationController.view.drive(
                            new Tween<double>(begin: 0.8, end: 1)
                        )
                    )
                );
                _sizeAnimation.parent = widget.visibilityAnimation.drive(
                    new Tween<double>(begin: 0.8, end: 1)
                );
                _fadeAnimation.parent = widget.visibilityAnimation.drive(
                    new CurveTween(curve: Curves.easeIn).chain(
                        new _ClampTween__menu_anchor(begin: 0, end: 1)
                    )
                );
                break;
            }
        }
    }

    internal virtual void _resolvePosition()
    {
        Offset anchorMidpoint = default!;
        if (widget.anchorPosition is not null)
        {
            anchorMidpoint =
                widget.anchorRect.topLeft
                + DartRuntimePrimitives.RequireValue(widget.anchorPosition);
        }
        else
        {
            anchorMidpoint = widget.anchorRect.center;
        }
        double xMidpointRatio = anchorMidpoint.dx / widget.overlaySize.width;
        double yMidpointRatio = anchorMidpoint.dy / widget.overlaySize.height;
        double dyLocal = (yMidpointRatio < 0.55) ? 1 : -1;
        double dxLocal = xMidpointRatio switch
        {
            < 0.4 => -1.0,
            > 0.6 => 1.0,
            _ => 0.0,
        };
        _menuAlignment = new Alignment(dxLocal, -dyLocal);
        Offset transformOrigin = default!;
        if (widget.anchorPosition is not null)
        {
            _attachmentPoint =
                widget.anchorRect.topLeft
                + DartRuntimePrimitives.RequireValue(widget.anchorPosition);
            transformOrigin = _attachmentPoint;
        }
        else
        {
            Offset offset = _kAttachmentOffset * dyLocal;
            _attachmentPoint =
                new Alignment(dxLocal, dyLocal).withinRect(widget.anchorRect) + offset;
            transformOrigin = new Alignment(0, dyLocal).withinRect(widget.anchorRect) + offset;
        }
        double xOriginRatio = transformOrigin.dx / widget.overlaySize.width;
        double yOriginRatio = transformOrigin.dy / widget.overlaySize.height;
        _attachmentPointAlignment = new Alignment(
            (xOriginRatio * 2L) - 1L,
            (yOriginRatio * 2L) - 1L
        );
    }

    internal virtual void _handleOutsideTap(Gestures.PointerDownEvent @event)
    {
        MenuController.maybeOf(context)!.close();
    }

    internal virtual void _handleSwipeDistanceChanged()
    {
        _swipeTargetDistance = Dart_uiLibrary.clampDouble(
            widget.swipeDistanceListenable.value,
            0,
            150
        );
        if (_swipeCurrentDistance == _swipeTargetDistance)
        {
            return;
        }
        _swipeTicker ??= createTicker(_updateSwipeScale);
        if (!_swipeTicker!.isActive)
        {
            _swipeTicker!.start();
        }
    }

    internal virtual void _updateSwipeScale(Duration elapsed)
    {
        var maxVelocity = 20.0;
        var minVelocity = 8.0;
        var maxSwipeDistance = 150.0;
        var accelerationRate = 0.12;
        var decelerationDistanceThreshold = 80.0;
        var remainingDistanceSnapThreshold = 1.0;
        var terminationDistanceThreshold = 5.0;
        double distance = _swipeTargetDistance - _swipeCurrentDistance;
        double absoluteDistance = distance.abs();
        double proximityFactor = Math.Min(absoluteDistance / decelerationDistanceThreshold, 1.0);
        _swipeVelocity += accelerationRate * proximityFactor;
        _swipeVelocity = Dart_uiLibrary.clampDouble(_swipeVelocity, minVelocity, maxVelocity);
        double finalVelocity = _swipeVelocity * proximityFactor;
        double distanceReduction = Math.Sign(distance) * finalVelocity;
        _swipeCurrentDistance += distanceReduction;
        if (absoluteDistance < remainingDistanceSnapThreshold)
        {
            _swipeCurrentDistance = _swipeTargetDistance;
            _swipeVelocity = 0;
            if (_swipeTargetDistance < terminationDistanceThreshold)
            {
                _swipeTicker!.stop();
            }
        }
        _swipeAnimationController.value = 1L - (_swipeCurrentDistance / maxSwipeDistance);
    }

    internal virtual Widget _buildAlign(BuildContext context, Widget? child)
    {
        return new Align(
            heightFactor: _sizeAnimation.value,
            widthFactor: 1.0,
            alignment: Alignment.topCenter,
            child: child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        BoxConstraints constraintsLocal = default!;
        if (widget.constraints is not null)
        {
            constraintsLocal = widget.constraints!;
        }
        else
        {
            bool isLargeTextModeEnabledLocal = Menu_anchorLibrary._largeTextModeEnabled(context);
            double screenWidthLocal = MediaQuery.widthOf(context);
            var menuWidth = _CupertinoMenuWidth__menu_anchorMembers.CreateFromScreenWidth(
                isLargeTextModeEnabled: isLargeTextModeEnabledLocal,
                screenWidth: screenWidthLocal
            );
            constraintsLocal = BoxConstraints.CreateTightFor(width: menuWidth.points());
        }
        Widget childLocal = new _SwipeSurface__menu_anchor(
            child: new TapRegion(
                groupId: widget.tapRegionGroupId,
                consumeOutsideTaps: widget.consumeOutsideTaps,
                onTapOutside: _handleOutsideTap,
                child: new Actions(
                    actions: _kActions,
                    child: new Shortcuts(
                        shortcuts: Menu_anchorLibrary._kMenuTraversalShortcuts,
                        child: new FocusScope(
                            node: widget.focusScopeNode,
                            descendantsAreFocusable: true,
                            descendantsAreTraversable: true,
                            canRequestFocus: true,
                            child: new CustomPaint(
                                painter: new _ShadowPainter__menu_anchor(
                                    brightness: CupertinoTheme.maybeBrightnessOf(context)
                                        ?? Brightness.light,
                                    repaint: _fadeAnimation
                                ),
                                child: new FadeTransition(
                                    opacity: _fadeAnimation,
                                    alwaysIncludeSemantics: true,
                                    child: new CupertinoPopupSurface(
                                        child: new AnimatedBuilder(
                                            animation: _sizeAnimation,
                                            builder: _buildAlign,
                                            child: new Widgets.Semantics(
                                                explicitChildNodes: true,
                                                scopesRoute: true,
                                                child: new ConstrainedBox(
                                                    constraints: constraintsLocal,
                                                    child: new SingleChildScrollView(
                                                        clipBehavior: Clip.none,
                                                        child: new Column(
                                                            mainAxisSize: MainAxisSize.min,
                                                            children: _children
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            )
        );
        if (!widget.constrainCrossAxis)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new UnconstrainedBox(
                    clipBehavior: Clip.hardEdge,
                    alignment: AlignmentDirectional.centerStart,
                    constrainedAxis: Axis.vertical,
                    child: childLocal
                )
            );
        }
        return new ConstrainedBox(
            constraints: BoxConstraints.CreateLoose(widget.overlaySize),
            child: new ScaleTransition(
                scale: _scaleAnimation,
                alignment: _attachmentPointAlignment,
                child: new ValueListenableBuilder<double>(
                    valueListenable: _sizeAnimation,
                    child: childLocal,
                    builder: (context, value, child) =>
                    {
                        Rect effectiveAnchorRect =
                            (widget.anchorPosition is not null)
                                ? (_attachmentPoint & Size.zero)
                                : widget.anchorRect;
                        List<DisplayFeature>? displayFeatures = MediaQuery.maybeDisplayFeaturesOf(
                            context
                        );
                        return new CustomSingleChildLayout(
                            @delegate: new _MenuLayoutDelegate__menu_anchor(
                                anchorRect: effectiveAnchorRect,
                                attachmentPoint: _attachmentPoint,
                                avoidBounds: (displayFeatures is not null)
                                    ? avoidBounds(displayFeatures)
                                    : new HashSet<Rect>(),
                                heightFactor: value,
                                menuAlignment: _menuAlignment,
                                overlayPadding: widget.overlayPadding.resolve(_textDirection)
                            ),
                            child: child
                        );
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static HashSet<Rect> avoidBounds(List<DisplayFeature> displayFeatures)
    {
        var boundsLocal = new HashSet<Rect>();
        foreach (var feature in displayFeatures)
        {
            if (
                (feature.bounds.shortestSide > 0L)
                || Equals(feature.state, DisplayFeatureState.postureHalfOpened)
            )
            {
                boundsLocal.Add(feature.bounds);
            }
        }
        return boundsLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

internal class _ShadowPainter__menu_anchor : CustomPainter
{
    internal static Radius _kRadius = Radius.circular(13);
    internal const double _kShadowOpacity = 0.12;
    public virtual Animation<double> repaint { get; private set; } = default!;
    public virtual Brightness brightness { get; private set; } = default!;

    internal _ShadowPainter__menu_anchor(Brightness brightness, Animation<double> repaint)
        : base(repaint: repaint)
    {
        this.brightness = brightness;
        this.repaint = repaint;
    }

    public virtual double shadowAnimation => Dart_uiLibrary.clampDouble(repaint.value, 0, 1);

    public override void paint(Canvas canvas, Size size)
    {
        DartRuntimePrimitives.Assert(() => (shadowAnimation >= 0L) && (shadowAnimation <= 1L));
        var centerLocal = new Offset(size.width / 2L, size.height / 2L);
        var rect = Rect.fromCenter(center: centerLocal, width: size.width, height: size.height);
        var roundedRect = RSuperellipse.fromRectAndRadius(rect, _kRadius);
        double blurSigma = shadowAnimation * 50L;
        var shadowPaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.maskFilter = MaskFilter.blur(BlurStyle.normal, blurSigma);
                    __cascade.color = Color.fromRGBO(
                        0L,
                        0L,
                        10L,
                        shadowAnimation * shadowAnimation * _kShadowOpacity
                    );
                    return __cascade;
                }
            )
        )();
        var maskPath = (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.fillType = PathFillType.evenOdd;
                    __cascade.addRect(rect.inflate(200));
                    __cascade.addRRect(RRect.fromRectAndRadius(rect, _kRadius));
                    return __cascade;
                }
            )
        )();
        DartRuntimePrimitives.Ignore(
            (
                (Func<Canvas>)(
                    () =>
                    {
                        var __cascade = canvas;
                        __cascade.save();
                        __cascade.clipPath(maskPath);
                        __cascade.drawRSuperellipse(roundedRect.inflate(50), shadowPaint);
                        __cascade.restore();
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldDelegate = (_ShadowPainter__menu_anchor)oldDelegate;
        return (!Equals(__oldDelegate.brightness, brightness))
            || (!Equals(__oldDelegate.repaint, repaint));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuildSemantics(CustomPainter oldDelegate) => false;
}

internal class _MenuLayoutDelegate__menu_anchor : SingleChildLayoutDelegate
{
    public virtual Rect anchorRect { get; private set; } = default!;
    public virtual Offset attachmentPoint { get; private set; } = default!;
    public virtual HashSet<Rect> avoidBounds { get; private set; } = default!;
    public virtual double heightFactor { get; private set; } = default!;
    public virtual Alignment menuAlignment { get; private set; } = default!;
    public virtual EdgeInsets overlayPadding { get; private set; } = default!;

    internal _MenuLayoutDelegate__menu_anchor(
        Rect anchorRect,
        Offset attachmentPoint,
        HashSet<Rect> avoidBounds,
        double heightFactor,
        Alignment menuAlignment,
        EdgeInsets overlayPadding
    )
    {
        this.anchorRect = anchorRect;
        this.attachmentPoint = attachmentPoint;
        this.avoidBounds = avoidBounds;
        this.heightFactor = heightFactor;
        this.menuAlignment = menuAlignment;
        this.overlayPadding = overlayPadding;
    }

    public override BoxConstraints getConstraintsForChild(BoxConstraints constraints)
    {
        return BoxConstraints.CreateLoose(constraints.biggest).deflate(overlayPadding);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        double inverseHeightFactor = (heightFactor > 0.01) ? (1L / heightFactor) : 0;
        double finalHeight = Math.Min(childSize.height * inverseHeightFactor, size.height);
        var finalSize = new Size(childSize.width, finalHeight);
        Offset desiredPosition = attachmentPoint - menuAlignment.alongSize(finalSize);
        Rect screen = _findClosestScreen(size, anchorRect.center, avoidBounds);
        Offset finalPosition = _positionChild(screen, finalSize, desiredPosition, anchorRect);
        bool growsUp = (finalPosition.dy + finalSize.height) <= anchorRect.center.dy;
        if (growsUp)
        {
            double dyLocal = finalHeight - childSize.height;
            return new Offset(finalPosition.dx, finalPosition.dy + dyLocal);
        }
        var initialPosition = new Offset(finalPosition.dx, anchorRect.bottom);
        return DartRuntimePrimitives.RequireValue(
            Dart_uiLibrary.Offset.lerp(initialPosition, finalPosition, heightFactor)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Offset _positionChild(
        Rect screen,
        Size childSize,
        Offset position,
        Rect anchor
    )
    {
        double xLocal = position.dx;
        double yLocal = position.dy;
        bool overLeftEdge(double x)
        {
            return x < (screen.left + overlayPadding.left);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool overRightEdge(double x)
        {
            return x > (screen.right - childSize.width - overlayPadding.right);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool overTopEdge(double y)
        {
            return y < (screen.top + overlayPadding.top);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool overBottomEdge(double y)
        {
            return y > (screen.bottom - childSize.height - overlayPadding.bottom);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool hasHorizontalAnchorOverlap = childSize.width >= screen.width;
        if (hasHorizontalAnchorOverlap)
        {
            xLocal = screen.left + overlayPadding.left;
        }
        else
        {
            if (overLeftEdge(xLocal))
            {
                double flipX = (anchor.center.dx * 2L) - position.dx - childSize.width;
                hasHorizontalAnchorOverlap = overRightEdge(flipX);
                if (hasHorizontalAnchorOverlap || overLeftEdge(flipX))
                {
                    xLocal = screen.left + overlayPadding.left;
                }
                else
                {
                    xLocal = flipX;
                }
            }
            else
            {
                if (overRightEdge(xLocal))
                {
                    double flipXLocal = (anchor.center.dx * 2L) - position.dx - childSize.width;
                    hasHorizontalAnchorOverlap = overLeftEdge(flipXLocal);
                    if (hasHorizontalAnchorOverlap || overRightEdge(flipXLocal))
                    {
                        xLocal = screen.right - childSize.width - overlayPadding.right;
                    }
                    else
                    {
                        xLocal = flipXLocal;
                    }
                }
            }
        }
        if (childSize.height >= screen.height)
        {
            return new Offset(xLocal, screen.top + overlayPadding.top);
        }
        if (hasHorizontalAnchorOverlap && !anchor.isEmpty)
        {
            double below = anchor.bottom - yLocal;
            double above = yLocal + childSize.height - anchor.top;
            if ((below > 0L) && (above > 0L))
            {
                if (below > above)
                {
                    yLocal = anchor.top - childSize.height;
                }
                else
                {
                    yLocal = anchor.bottom;
                }
            }
        }
        if (overTopEdge(yLocal))
        {
            double flipY = (anchor.center.dy * 2L) - position.dy - childSize.height;
            if (overTopEdge(flipY) || overBottomEdge(flipY))
            {
                yLocal = screen.top + overlayPadding.top;
            }
            else
            {
                yLocal = flipY;
            }
        }
        else
        {
            if (overBottomEdge(yLocal))
            {
                double flipYLocal = (anchor.center.dy * 2L) - position.dy - childSize.height;
                if (overTopEdge(flipYLocal) || overBottomEdge(flipYLocal))
                {
                    yLocal = screen.bottom - childSize.height - overlayPadding.bottom;
                }
                else
                {
                    yLocal = flipYLocal;
                }
            }
        }
        return new Offset(xLocal, yLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Rect _findClosestScreen(
        Size parentSize,
        Offset point,
        HashSet<Rect> avoidBounds
    )
    {
        IEnumerable<Rect> screens = DisplayFeatureSubScreen.subScreensInBounds(
            Offset.zero & parentSize,
            avoidBounds
        );
        Rect? closest = default!;
        double closestSquaredDistance = 0;
        foreach (var screen in screens)
        {
            if (screen.contains(point))
            {
                return screen;
            }
            if (closest is null)
            {
                closest = screen;
                closestSquaredDistance = Menu_anchorLibrary._computeSquaredDistanceToRect(
                    point,
                    DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(closest))
                );
                continue;
            }
            double squaredDistance = Menu_anchorLibrary._computeSquaredDistanceToRect(
                point,
                screen
            );
            if (squaredDistance < closestSquaredDistance)
            {
                closest = screen;
                closestSquaredDistance = squaredDistance;
            }
        }
        return DartRuntimePrimitives.RequireValue(closest);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_MenuLayoutDelegate__menu_anchor)oldDelegate;
        return (!Equals(anchorRect, __oldDelegate.anchorRect))
            || (!Equals(attachmentPoint, __oldDelegate.attachmentPoint))
            || !CollectionsLibrary.setEquals(avoidBounds, __oldDelegate.avoidBounds)
            || (heightFactor != __oldDelegate.heightFactor)
            || (!Equals(menuAlignment, __oldDelegate.menuAlignment))
            || (!Equals(overlayPadding, __oldDelegate.overlayPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _FocusUpIntent__menu_anchor : DirectionalFocusIntent
{
    internal _FocusUpIntent__menu_anchor()
        : base(TraversalDirection.up) { }
}

internal class _FocusDownIntent__menu_anchor : DirectionalFocusIntent
{
    internal _FocusDownIntent__menu_anchor()
        : base(TraversalDirection.down) { }
}

internal class _FocusUpAction__menu_anchor : ContextAction<DirectionalFocusIntent>
{
    internal _FocusUpAction__menu_anchor() { }

    public override object? invoke(DirectionalFocusIntent intent, BuildContext? context = null)
    {
        FocusTraversalPolicy policy =
            FocusTraversalGroup.maybeOf(context!) ?? new ReadingOrderTraversalPolicy();
        if (Menu_anchorLibrary._isCupertino && !Foundation.ConstantsLibrary.kIsWeb)
        {
            policy.inDirection(Focus_managerLibrary.primaryFocus!, intent.direction);
            return default!;
        }
        FocusNode? firstFocus = policy.findFirstFocus(
            Focus_managerLibrary.primaryFocus!,
            ignoreCurrentFocus: true
        );
        FocusNode lastFocus = policy.findLastFocus(
            Focus_managerLibrary.primaryFocus!,
            ignoreCurrentFocus: true
        );
        if (lastFocus.context is not null)
        {
            if (
                Equals(Focus_managerLibrary.primaryFocus, lastFocus.enclosingScope)
                || Equals(Focus_managerLibrary.primaryFocus, firstFocus)
            )
            {
                policy.requestFocusCallback(lastFocus);
                return default!;
            }
        }
        policy.inDirection(Focus_managerLibrary.primaryFocus!, intent.direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _FocusDownAction__menu_anchor : ContextAction<DirectionalFocusIntent>
{
    internal _FocusDownAction__menu_anchor() { }

    public override object? invoke(DirectionalFocusIntent intent, BuildContext? context = null)
    {
        FocusTraversalPolicy policy =
            FocusTraversalGroup.maybeOf(context!) ?? new ReadingOrderTraversalPolicy();
        if (Menu_anchorLibrary._isCupertino && !Foundation.ConstantsLibrary.kIsWeb)
        {
            policy.inDirection(Focus_managerLibrary.primaryFocus!, intent.direction);
            return default!;
        }
        FocusNode? firstFocus = policy.findFirstFocus(
            Focus_managerLibrary.primaryFocus!,
            ignoreCurrentFocus: true
        );
        FocusNode lastFocus = policy.findLastFocus(
            Focus_managerLibrary.primaryFocus!,
            ignoreCurrentFocus: true
        );
        if (firstFocus?.context is not null)
        {
            if (
                Equals(Focus_managerLibrary.primaryFocus, firstFocus!.enclosingScope)
                || Equals(Focus_managerLibrary.primaryFocus, lastFocus)
            )
            {
                policy.requestFocusCallback(firstFocus);
                return default!;
            }
        }
        policy.inDirection(Focus_managerLibrary.primaryFocus!, intent.direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class _FocusFirstIntent__menu_anchor : Intent
{
    internal _FocusFirstIntent__menu_anchor() { }
}

internal class _FocusFirstAction__menu_anchor : ContextAction<_FocusFirstIntent__menu_anchor>
{
    internal _FocusFirstAction__menu_anchor() { }

    public override object? invoke(
        _FocusFirstIntent__menu_anchor intent,
        BuildContext? context = null
    )
    {
        FocusTraversalPolicy policy =
            FocusTraversalGroup.maybeOf(context!) ?? new ReadingOrderTraversalPolicy();
        FocusNode? firstFocus = policy.findFirstFocus(
            Focus_managerLibrary.primaryFocus!,
            ignoreCurrentFocus: true
        );
        if ((firstFocus is null) || (firstFocus.context is null))
        {
            return default!;
        }
        policy.requestFocusCallback(firstFocus);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class _FocusLastIntent__menu_anchor : Intent
{
    internal _FocusLastIntent__menu_anchor() { }
}

internal class _FocusLastAction__menu_anchor : ContextAction<_FocusLastIntent__menu_anchor>
{
    internal _FocusLastAction__menu_anchor() { }

    public override object? invoke(
        _FocusLastIntent__menu_anchor intent,
        BuildContext? context = null
    )
    {
        FocusTraversalPolicy policy =
            FocusTraversalGroup.maybeOf(context!) ?? new ReadingOrderTraversalPolicy();
        FocusNode lastFocus = policy.findLastFocus(
            Focus_managerLibrary.primaryFocus!,
            ignoreCurrentFocus: true
        );
        if (lastFocus.context is null)
        {
            return default!;
        }
        policy.requestFocusCallback(lastFocus);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _CupertinoMenuImplicitDivider__menu_anchor : StatelessWidget
{
    public static CupertinoDynamicColor kOverlayColor = new CupertinoDynamicColor(
        color: Color.fromRGBO(140L, 140L, 140L, 0.3),
        darkColor: Color.fromRGBO(255L, 255L, 255L, 0.25)
    );
    public static CupertinoDynamicColor kDividerColor = new CupertinoDynamicColor(
        color: Color.fromRGBO(0L, 0L, 0L, 0.25),
        darkColor: Color.fromRGBO(255L, 255L, 255L, 0.25)
    );

    internal _CupertinoMenuImplicitDivider__menu_anchor() { }

    public override Widget build(BuildContext context)
    {
        double pixelRatio = MediaQuery.maybeDevicePixelRatioOf(context) ?? 1.0;
        double displacement = 1L / pixelRatio;
        return new CustomPaint(
            size: new Size(double.PositiveInfinity, displacement),
            painter: new _CupertinoDividerPainter__menu_anchor(
                color: CupertinoDynamicColor.resolve(kDividerColor, context),
                overlayColor: CupertinoDynamicColor.resolve(kOverlayColor, context),
                antiAlias: pixelRatio < 1.0
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoMenuDivider : StatelessWidget, CupertinoMenuEntry
{
    public virtual Color color { get; private set; } = default!;
    public static CupertinoDynamicColor kDefaultColor = new CupertinoDynamicColor(
        color: Color.fromRGBO(0L, 0L, 0L, 0.08),
        darkColor: Color.fromRGBO(0L, 0L, 0L, 0.16)
    );
    internal const double _kDividerHeight = 8.0;

    public CupertinoMenuDivider(Key? key = null, Color color = default!)
        : base(key: key)
    {
        Color __color = color ?? kDefaultColor;
        this.color = __color;
    }

    public virtual bool isDivider => true;

    public virtual bool hasLeading(BuildContext context) => false;

    public override Widget build(BuildContext context)
    {
        return new ColoredBox(
            color: CupertinoDynamicColor.resolve(color, context),
            child: new SizedBox(height: _kDividerHeight, width: double.PositiveInfinity)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _CupertinoDividerPainter__menu_anchor : CustomPainter
{
    public virtual Color color { get; private set; } = default!;
    public virtual Color overlayColor { get; private set; } = default!;
    public virtual bool antiAlias { get; private set; } = default!;

    internal _CupertinoDividerPainter__menu_anchor(
        Color color,
        Color overlayColor,
        bool antiAlias = false
    )
    {
        this.color = color;
        this.overlayColor = overlayColor;
        this.antiAlias = antiAlias;
    }

    public override void paint(Canvas canvas, Size size)
    {
        Offset p1 = size.centerLeft(Offset.zero);
        Offset p2 = size.centerRight(Offset.zero);
        if (!Foundation.ConstantsLibrary.kIsWeb)
        {
            var overlayPainter = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.style = PaintingStyle.stroke;
                        __cascade.color = overlayColor;
                        __cascade.isAntiAlias = antiAlias;
                        __cascade.blendMode = BlendMode.overlay;
                        return __cascade;
                    }
                )
            )();
            canvas.drawLine(p1, p2, overlayPainter);
        }
        var colorPainter = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.style = PaintingStyle.stroke;
                    __cascade.color = color;
                    __cascade.isAntiAlias = antiAlias;
                    return __cascade;
                }
            )
        )();
        canvas.drawLine(p1, p2, colorPainter);
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldDelegate = (_CupertinoDividerPainter__menu_anchor)oldDelegate;
        return (!Equals(color, __oldDelegate.color))
            || (!Equals(overlayColor, __oldDelegate.overlayColor))
            || (antiAlias != __oldDelegate.antiAlias);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoMenuItem : StatelessWidget, CupertinoMenuEntry
{
    public virtual Widget child { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual Widget? leading { get; private set; }
    public virtual Widget? trailing { get; private set; }
    public virtual Widget? subtitle { get; private set; }
    public virtual Action? onPressed { get; private set; }
    public virtual Action<bool>? onHover { get; private set; }
    public virtual Action<bool>? onFocusChange { get; private set; }
    public virtual bool requestFocusOnHover { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual WidgetStateProperty<BoxDecoration>? decoration { get; private set; }
    public virtual WidgetStateProperty<MouseCursor>? mouseCursor { get; private set; }
    public virtual HitTestBehavior behavior { get; private set; } = default!;
    public virtual bool requestCloseOnActivate { get; private set; } = default!;
    public virtual bool isDestructiveAction { get; private set; } = default!;
    public virtual double? leadingWidth { get; private set; }
    public virtual double? trailingWidth { get; private set; }
    public virtual AlignmentGeometry? leadingMidpointAlignment { get; private set; }
    public virtual AlignmentGeometry? trailingMidpointAlignment { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public static WidgetStateProperty<BoxDecoration> kDefaultDecoration =
        WidgetStateProperty<BoxDecoration>.CreateFromMap(
            new DartMap<WidgetStatesConstraint, BoxDecoration>
            {
                [WidgetState.dragged.asConstraint()] = new BoxDecoration(
                    color: new CupertinoDynamicColor(
                        color: Color.fromRGBO(50L, 50L, 50L, 0.1),
                        darkColor: Color.fromRGBO(255L, 255L, 255L, 0.1)
                    )
                ),
                [WidgetState.pressed.asConstraint()] = new BoxDecoration(
                    color: new CupertinoDynamicColor(
                        color: Color.fromRGBO(50L, 50L, 50L, 0.1),
                        darkColor: Color.fromRGBO(255L, 255L, 255L, 0.1)
                    )
                ),
                [WidgetState.focused.asConstraint()] = new BoxDecoration(
                    color: new CupertinoDynamicColor(
                        color: Color.fromRGBO(50L, 50L, 50L, 0.075),
                        darkColor: Color.fromRGBO(255L, 255L, 255L, 0.075)
                    )
                ),
                [WidgetState.hovered.asConstraint()] = new BoxDecoration(
                    color: new CupertinoDynamicColor(
                        color: Color.fromRGBO(50L, 50L, 50L, 0.05),
                        darkColor: Color.fromRGBO(255L, 255L, 255L, 0.05)
                    )
                ),
                [WidgetStateMembers.any] = new BoxDecoration(),
            }
        );
    internal static WidgetStateProperty<MouseCursor> _kDefaultCursor =
        WidgetStateProperty.resolveWith(
            (states) =>
            {
                return (
                    !states.Contains(WidgetState.disabled) && Foundation.ConstantsLibrary.kIsWeb
                )
                    ? SystemMouseCursors.click
                    : MouseCursor.defer;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
    internal static Color _kDefaultTextColor = new CupertinoDynamicColor(
        color: Color.from(alpha: 0.96, red: 0, green: 0, blue: 0),
        darkColor: Color.from(alpha: 0.96, red: 1, green: 1, blue: 1)
    );
    internal static Color _kDefaultSubtitleTextColor = new CupertinoDynamicColor(
        color: Color.from(alpha: 0.55, red: 0, green: 0, blue: 0),
        darkColor: Color.from(alpha: 0.4, red: 1, green: 1, blue: 1)
    );
    internal const long _kDefaultMaxLines = 2L;
    internal const long _kDefaultLargeTextModeMaxLines = 100L;
    internal static TextStyle _kLeadingDefaultTextStyle = new TextStyle(
        fontSize: 15,
        fontWeight: FontWeight.w600
    );
    internal static IconThemeData _kLeadingDefaultIconTheme = new IconThemeData(
        size: 15,
        weight: 600,
        applyTextScaling: true
    );
    internal static TextStyle _kTrailingDefaultTextStyle = new TextStyle(fontSize: 21);
    internal static IconThemeData _kTrailingDefaultIconTheme = new IconThemeData(
        size: 21,
        applyTextScaling: true
    );

    public CupertinoMenuItem(
        Key? key = null,
        Widget child = default!,
        Widget? subtitle = null,
        Widget? leading = null,
        double? leadingWidth = null,
        AlignmentGeometry? leadingMidpointAlignment = null,
        Widget? trailing = null,
        double? trailingWidth = null,
        AlignmentGeometry? trailingMidpointAlignment = null,
        EdgeInsetsGeometry? padding = null,
        BoxConstraints? constraints = null,
        bool autofocus = false,
        FocusNode? focusNode = null,
        Action<bool>? onFocusChange = null,
        Action<bool>? onHover = null,
        Action? onPressed = null,
        WidgetStateProperty<BoxDecoration>? decoration = null,
        WidgetStateProperty<MouseCursor>? mouseCursor = null,
        HitTestBehavior behavior = HitTestBehavior.opaque,
        bool requestCloseOnActivate = true,
        bool requestFocusOnHover = true,
        bool isDestructiveAction = false
    )
        : base(key: key)
    {
        this.child = child;
        this.subtitle = subtitle;
        this.leading = leading;
        this.leadingWidth = leadingWidth;
        this.leadingMidpointAlignment = leadingMidpointAlignment;
        this.trailing = trailing;
        this.trailingWidth = trailingWidth;
        this.trailingMidpointAlignment = trailingMidpointAlignment;
        this.padding = padding;
        this.constraints = constraints;
        this.autofocus = autofocus;
        this.focusNode = focusNode;
        this.onFocusChange = onFocusChange;
        this.onHover = onHover;
        this.onPressed = onPressed;
        this.decoration = decoration;
        this.mouseCursor = mouseCursor;
        this.behavior = behavior;
        this.requestCloseOnActivate = requestCloseOnActivate;
        this.requestFocusOnHover = requestFocusOnHover;
        this.isDestructiveAction = isDestructiveAction;
    }

    public virtual bool hasLeading(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<bool>(leading is not null);

    public virtual bool isDivider => false;

    internal virtual TextStyle _resolveDefaultTextStyle(BuildContext context, TextScaler textScaler)
    {
        Color colorLocal = default!;
        if (onPressed is null)
        {
            colorLocal = DartRuntimePrimitives.ConvertValue<Color>(CupertinoColors.systemGrey);
        }
        else
        {
            if (isDestructiveAction)
            {
                colorLocal = DartRuntimePrimitives.ConvertValue<Color>(CupertinoColors.systemRed);
            }
            else
            {
                colorLocal = _kDefaultTextColor;
            }
        }
        return _DynamicTypeStyle__menu_anchor
            .body.resolveTextStyle(textScaler)
            .copyWith(fontSize: 17, color: CupertinoDynamicColor.resolve(colorLocal, context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TextStyle _resolveDefaultSubtitleStyle(
        BuildContext context,
        TextScaler textScaler
    )
    {
        var isDark = Equals(CupertinoTheme.maybeBrightnessOf(context), Brightness.dark);
        return _DynamicTypeStyle__menu_anchor
            .subhead.resolveTextStyle(textScaler)
            .copyWith(
                fontSize: 15,
                textBaseline: TextBaseline.alphabetic,
                foreground: (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.blendMode = isDark ? BlendMode.plus : BlendMode.hardLight;
                            __cascade.color = CupertinoDynamicColor.resolve(
                                _kDefaultSubtitleTextColor,
                                context
                            );
                            return __cascade;
                        }
                    )
                )()
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSelect(BuildContext context)
    {
        if (requestCloseOnActivate)
        {
            MenuController.maybeOf(context)?.close();
        }
        onPressed?.Invoke();
    }

    public override Widget build(BuildContext context)
    {
        TextScaler textScaler =
            MediaQuery.maybeTextScalerOf(context)
            ?? TextScaler.CreateLinear(MediaQuery.maybeTextScaleFactorOf(context) ?? 1);
        TextStyle defaultTextStyle = _resolveDefaultTextStyle(context, textScaler);
        bool isLargeTextModeEnabled = Menu_anchorLibrary._largeTextModeEnabled(context);
        Widget? leadingWidget = default!;
        Widget? trailingWidget = default!;
        if (leading is not null)
        {
            leadingWidget = DefaultTextStyle.merge(
                style: _kLeadingDefaultTextStyle,
                child: IconTheme.merge(data: _kLeadingDefaultIconTheme, child: leading!)
            );
        }
        if ((trailing is not null) && !isLargeTextModeEnabled)
        {
            trailingWidget = DefaultTextStyle.merge(
                style: _kTrailingDefaultTextStyle,
                child: IconTheme.merge(data: _kTrailingDefaultIconTheme, child: trailing!)
            );
        }
        return MediaQuery.withClampedTextScaling(
            minScaleFactor: Menu_anchorLibrary._kMinimumTextScaleFactor,
            maxScaleFactor: Menu_anchorLibrary._kMaximumTextScaleFactor,
            child: new _CupertinoMenuItemInteractionHandler__menu_anchor(
                mouseCursor: mouseCursor ?? _kDefaultCursor,
                requestFocusOnHover: requestFocusOnHover,
                onPressed: (onPressed is not null)
                    ? (
                        () =>
                        {
                            _handleSelect(context);
                        }
                    )
                    : null,
                onHover: onHover,
                onFocusChange: onFocusChange,
                autofocus: autofocus,
                focusNode: focusNode,
                decoration: decoration ?? kDefaultDecoration,
                behavior: behavior,
                child: DefaultTextStyle.merge(
                    maxLines: isLargeTextModeEnabled
                        ? _kDefaultLargeTextModeMaxLines
                        : _kDefaultMaxLines,
                    overflow: TextOverflow.ellipsis,
                    softWrap: true,
                    style: new TextStyle(color: defaultTextStyle.color),
                    child: IconTheme.merge(
                        data: new IconThemeData(color: defaultTextStyle.color),
                        child: new _CupertinoMenuItemLabel__menu_anchor(
                            padding: padding,
                            constraints: constraints,
                            trailing: trailingWidget,
                            leading: leadingWidget,
                            leadingMidpointAlignment: leadingMidpointAlignment,
                            trailingMidpointAlignment: trailingMidpointAlignment,
                            leadingWidth: leadingWidth,
                            trailingWidth: trailingWidth,
                            subtitle: (subtitle is not null)
                                ? DefaultTextStyle.merge(
                                    style: _resolveDefaultSubtitleStyle(context, textScaler),
                                    child: subtitle!
                                )
                                : null,
                            child: DefaultTextStyle.merge(style: defaultTextStyle, child: child)
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Widget?>("child", child));
        properties.add(
            new FlagProperty(
                "requestCloseOnActivate",
                value: requestCloseOnActivate,
                ifTrue: "closes on press",
                ifFalse: "does not close on press",
                defaultValue: true
            )
        );
        properties.add(
            new FlagProperty(
                "requestFocusOnHover",
                value: requestFocusOnHover,
                ifFalse: "does not request focus on hover",
                ifTrue: "requests focus on hover",
                defaultValue: true
            )
        );
        properties.add(new EnumProperty<HitTestBehavior>("hitTestBehavior", behavior));
        properties.add(
            new DiagnosticsProperty<FocusNode?>("focusNode", focusNode, defaultValue: null)
        );
        properties.add(
            new FlagProperty("enabled", value: onPressed is not null, ifFalse: "DISABLED")
        );
        if (subtitle is not null)
        {
            properties.add(new DiagnosticsProperty<Widget?>("subtitle", subtitle));
        }
        if (leading is not null)
        {
            properties.add(new DiagnosticsProperty<Widget?>("leading", leading));
        }
        if (trailing is not null)
        {
            properties.add(new DiagnosticsProperty<Widget?>("trailing", trailing));
        }
    }
}

internal class _CupertinoMenuItemLabel__menu_anchor : StatelessWidget
{
    internal const double _kDefaultHorizontalWidth = 16;
    internal static double _kLeadingWidthSlope = -311L / 1000L;
    internal const double _kLeadingWidthYIntercept = 10;
    internal static double _kLeadingMidpointSlope = 118L / 1000000L;
    internal static double _kLeadingMidpointYIntercept = 73L / 125L;
    internal static double _kTrailingWidthSlope = 1L / 10L;
    internal const double _kTrailingWidthYIntercept = 22;
    internal static double _kFirstBaselineToTopSlope = 14L / 11L;
    internal static double _kLastBaselineToBottomSlope = 71L / 100L;
    public virtual Widget? leading { get; private set; }
    public virtual double? leadingWidth { get; private set; }
    internal virtual AlignmentGeometry? _leadingAlignment { get; private set; }
    public virtual Widget? trailing { get; private set; }
    public virtual double? trailingWidth { get; private set; }
    internal virtual AlignmentGeometry? _trailingAlignment { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual Widget? subtitle { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    internal virtual BoxConstraints? _constraints { get; private set; }

    internal _CupertinoMenuItemLabel__menu_anchor(
        Widget child,
        Widget? subtitle = null,
        Widget? leading = null,
        double? leadingWidth = null,
        AlignmentGeometry? leadingMidpointAlignment = null,
        Widget? trailing = null,
        double? trailingWidth = null,
        AlignmentGeometry? trailingMidpointAlignment = null,
        BoxConstraints? constraints = null,
        EdgeInsetsGeometry? padding = null
    )
    {
        this.child = child;
        this.subtitle = subtitle;
        this.leading = leading;
        this.leadingWidth = leadingWidth;
        this.trailing = trailing;
        this.trailingWidth = trailingWidth;
        this.padding = padding;
        _leadingAlignment = leadingMidpointAlignment;
        _trailingAlignment = trailingMidpointAlignment;
        _constraints = constraints;
    }

    internal virtual double _resolveLeadingWidth(
        TextScaler textScaler,
        double pixelRatio,
        double lineHeight
    )
    {
        double units = Menu_anchorLibrary._normalizeTextScale(textScaler);
        double value = (_kLeadingWidthSlope * units) + _kLeadingWidthYIntercept;
        return Menu_anchorLibrary._roundToDivisible(value + lineHeight, to: 1L / pixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _resolveTrailingWidth(
        TextScaler textScaler,
        double pixelRatio,
        double lineHeight
    )
    {
        double units = Menu_anchorLibrary._normalizeTextScale(textScaler);
        double value = (_kTrailingWidthSlope * units) + _kTrailingWidthYIntercept;
        return Menu_anchorLibrary._roundToDivisible(value + lineHeight, to: 1L / pixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual AlignmentGeometry _resolveTrailingAlignment(double trailingWidth)
    {
        double horizontalOffset = (DartRuntimePrimitives.RequireValue(trailingWidth) / 2L) + 6L;
        double horizontalRatio =
            (DartRuntimePrimitives.RequireValue(trailingWidth) - horizontalOffset)
            / DartRuntimePrimitives.RequireValue(trailingWidth);
        double horizontalAlignment = (horizontalRatio * 2L) - 1L;
        return new AlignmentDirectional(horizontalAlignment, 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual AlignmentGeometry _resolveLeadingAlignment(
        double leadingWidth,
        TextScaler textScaler
    )
    {
        double units = Menu_anchorLibrary._normalizeTextScale(textScaler);
        double horizontalRatio = (_kLeadingMidpointSlope * units) + _kLeadingMidpointYIntercept;
        double horizontalAlignment = (horizontalRatio * 2L) - 1L;
        return new AlignmentDirectional(horizontalAlignment, 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _resolveFirstBaselineToTop(double lineHeight, double pixelRatio)
    {
        return Menu_anchorLibrary._roundToDivisible(
            lineHeight * _kFirstBaselineToTopSlope,
            to: 1L / pixelRatio
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _resolveLastBaselineToBottom(double lineHeight, double pixelRatio)
    {
        return Menu_anchorLibrary._roundToDivisible(
            lineHeight * _kLastBaselineToBottomSlope,
            to: 1L / pixelRatio
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual EdgeInsets _resolvePadding(double minimumHeight, double lineHeight)
    {
        double padding = Math.Max(0, minimumHeight - lineHeight);
        return EdgeInsets.CreateSymmetric(
            vertical: DartRuntimePrimitives.RequireValue(padding) / 2L
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        TextDirection textDirectionLocal = Directionality.maybeOf(context) ?? TextDirection.ltr;
        TextScaler textScaler = MediaQuery.maybeTextScalerOf(context) ?? TextScaler.noScaling;
        double pixelRatio = MediaQuery.maybeDevicePixelRatioOf(context) ?? 1.0;
        TextStyle dynamicBodyText = _DynamicTypeStyle__menu_anchor.body.resolveTextStyle(
            textScaler
        );
        DartRuntimePrimitives.Assert(() =>
            (dynamicBodyText.fontSize is not null) && (dynamicBodyText.height is not null)
        );
        double lineHeight =
            DartRuntimePrimitives.RequireValue(dynamicBodyText.fontSize)
            * DartRuntimePrimitives.RequireValue(dynamicBodyText.height);
        bool showLeadingWidget =
            (leading is not null) || (CupertinoMenuAnchor.maybeHasLeadingOf(context) ?? false);
        double minimumHeight =
            _resolveFirstBaselineToTop(lineHeight, pixelRatio)
            + _resolveLastBaselineToBottom(lineHeight, pixelRatio);
        BoxConstraints constraintsLocal =
            _constraints ?? new BoxConstraints(minHeight: minimumHeight);
        EdgeInsetsGeometry resolvedPadding = padding ?? _resolvePadding(minimumHeight, lineHeight);
        double resolvedLeadingWidth =
            leadingWidth
            ?? (
                showLeadingWidget
                    ? _resolveLeadingWidth(textScaler, pixelRatio, lineHeight)
                    : _kDefaultHorizontalWidth
            );
        double resolvedTrailingWidth =
            trailingWidth
            ?? (
                (trailing is not null)
                    ? _resolveTrailingWidth(textScaler, pixelRatio, lineHeight)
                    : _kDefaultHorizontalWidth
            );
        return new ConstrainedBox(
            constraints: constraintsLocal,
            child: new Padding(
                padding: resolvedPadding,
                child: new Stack(
                    children: (
                        (Func<List<Widget>>)(
                            () =>
                            {
                                var __collection83239 = new List<Widget>();
                                if (showLeadingWidget)
                                {
                                    __collection83239.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            Positioned.CreateDirectional(
                                                textDirection: textDirectionLocal,
                                                start: 0,
                                                top: 0,
                                                bottom: 0,
                                                width: resolvedLeadingWidth,
                                                child: new _AlignMidpoint__menu_anchor(
                                                    alignment: _leadingAlignment
                                                        ?? _resolveLeadingAlignment(
                                                            resolvedLeadingWidth,
                                                            textScaler
                                                        ),
                                                    child: leading
                                                )
                                            )
                                        )
                                    );
                                }
                                __collection83239.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new Padding(
                                            padding: EdgeInsetsDirectional.CreateOnly(
                                                start: resolvedLeadingWidth,
                                                end: resolvedTrailingWidth
                                            ),
                                            child: (subtitle is null)
                                                ? new global::Doroti.Framework.Widgets.Align(
                                                    alignment: global::Doroti
                                                        .Framework
                                                        .Painting
                                                        .AlignmentDirectional
                                                        .centerStart,
                                                    child: child
                                                )
                                                : new global::Doroti.Framework.Widgets.Column(
                                                    mainAxisSize: global::Doroti
                                                        .Framework
                                                        .Rendering
                                                        .MainAxisSize
                                                        .min,
                                                    crossAxisAlignment: global::Doroti
                                                        .Framework
                                                        .Rendering
                                                        .CrossAxisAlignment
                                                        .stretch,
                                                    mainAxisAlignment: global::Doroti
                                                        .Framework
                                                        .Rendering
                                                        .MainAxisAlignment
                                                        .center,
                                                    children: new List<global::Doroti.Framework.Widgets.Widget>
                                                    {
                                                        DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(
                                                            child
                                                        ),
                                                        DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(
                                                            new global::Doroti.Framework.Widgets.SizedBox(
                                                                height: 1
                                                            )
                                                        ),
                                                        DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(
                                                            subtitle!
                                                        ),
                                                    }
                                                )
                                        )
                                    )
                                );
                                if (trailing is not null)
                                {
                                    __collection83239.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            Positioned.CreateDirectional(
                                                textDirection: textDirectionLocal,
                                                end: 0,
                                                top: 0,
                                                bottom: 0,
                                                width: resolvedTrailingWidth,
                                                child: new _AlignMidpoint__menu_anchor(
                                                    alignment: _trailingAlignment
                                                        ?? _resolveTrailingAlignment(
                                                            resolvedTrailingWidth
                                                        ),
                                                    child: trailing
                                                )
                                            )
                                        )
                                    );
                                }
                                return __collection83239;
                            }
                        )
                    )()
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _AlignMidpoint__menu_anchor : SingleChildRenderObjectWidget
{
    public virtual AlignmentGeometry alignment { get; private set; } = default!;

    internal _AlignMidpoint__menu_anchor(AlignmentGeometry alignment, Widget? child)
        : base(child: child)
    {
        this.alignment = alignment;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderAlignMidpoint__menu_anchor(
            alignment: alignment,
            textDirection: Directionality.maybeOf(context)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderAlignMidpoint__menu_anchor)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderAlignMidpoint__menu_anchor>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.alignment = alignment;
                        __cascade.textDirection = Directionality.maybeOf(context);
                        return __cascade;
                    }
                )
            )()
        );
    }
}

public class _RenderAlignMidpoint__menu_anchor : RenderPositionedBox
{
    internal _RenderAlignMidpoint__menu_anchor(
        AlignmentGeometry alignment = default!,
        TextDirection? textDirection = null
    )
        : base(alignment: alignment ?? Alignment.center, textDirection: textDirection) { }

    public override void alignChild()
    {
        DartRuntimePrimitives.Assert(() => child is not null);
        DartRuntimePrimitives.Assert(() => !child!.debugNeedsLayout);
        DartRuntimePrimitives.Assert(() => child!.hasSize);
        DartRuntimePrimitives.Assert(() => hasSize);
        var childParentData = ((BoxParentData?)child!.parentData!)!;
        Offset offsetLocal = resolvedAlignment.alongSize(size) - child!.size.center(Offset.zero);
        double dxLocal = Dart_uiLibrary.clampDouble(
            offsetLocal.dx,
            0.0,
            size.width - child!.size.width
        );
        double dyLocal = Dart_uiLibrary.clampDouble(
            offsetLocal.dy,
            0.0,
            size.height - child!.size.height
        );
        childParentData.offset = new Offset(dxLocal, dyLocal);
    }
}

public class _CupertinoMenuItemInteractionHandler__menu_anchor : StatefulWidget
{
    public virtual Action<bool>? onHover { get; private set; }
    public virtual Action? onPressed { get; private set; }
    public virtual Action<bool>? onFocusChange { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool requestFocusOnHover { get; private set; } = default!;
    public virtual HitTestBehavior behavior { get; private set; } = default!;
    public virtual WidgetStateProperty<MouseCursor> mouseCursor { get; private set; } = default!;
    public virtual WidgetStateProperty<BoxDecoration> decoration { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _CupertinoMenuItemInteractionHandler__menu_anchor(
        Action<bool>? onHover,
        Action? onPressed,
        Action<bool>? onFocusChange,
        FocusNode? focusNode,
        bool autofocus,
        bool requestFocusOnHover,
        HitTestBehavior behavior,
        WidgetStateProperty<MouseCursor> mouseCursor,
        WidgetStateProperty<BoxDecoration> decoration,
        Widget child
    )
    {
        this.onHover = onHover;
        this.onPressed = onPressed;
        this.onFocusChange = onFocusChange;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.requestFocusOnHover = requestFocusOnHover;
        this.behavior = behavior;
        this.mouseCursor = mouseCursor;
        this.decoration = decoration;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CupertinoMenuItemInteractionHandlerState__menu_anchor()
        );
}

internal class _CupertinoMenuItemInteractionHandlerState__menu_anchor
    : State<_CupertinoMenuItemInteractionHandler__menu_anchor>
{
    private bool __late__actions_initialized;
    private DartMap<Type, dynamic> __late__actions = default!;
    internal virtual DartMap<Type, dynamic> _actions
    {
        get
        {
            if (!__late__actions_initialized)
            {
                __late__actions = new DartMap<Type, dynamic>
                {
                    [typeof(ActivateIntent)] = new CallbackAction<ActivateIntent>(
                        onInvoke: _handleActivation
                    ),
                    [typeof(ButtonActivateIntent)] = new CallbackAction<ButtonActivateIntent>(
                        onInvoke: _handleActivation
                    ),
                };
                __late__actions_initialized = true;
            }
            return __late__actions;
        }
    }
    internal virtual DartMap<Type, dynamic>? _gestures { get; set; } = default;
    internal virtual Gestures.DeviceGestureSettings? _gestureSettings { get; set; } = default;
    internal virtual FocusNode? _internalFocusNode { get; set; } = default;
    internal virtual WidgetStatesController _statesController { get; private set; } =
        new WidgetStatesController();

    internal virtual FocusNode _focusNode =>
        DartRuntimePrimitives.ConvertValue<FocusNode>(widget.focusNode ?? _internalFocusNode!);
    public virtual bool isHovered
    {
        get => _statesController.value.Contains(WidgetState.hovered);
        set
        {
            var __value = value;
            _statesController.update(WidgetState.hovered, __value);
        }
    }
    public virtual bool isPressed
    {
        get => _statesController.value.Contains(WidgetState.pressed);
        set
        {
            var __value = value;
            _statesController.update(WidgetState.pressed, __value);
        }
    }
    public virtual bool isSwiped
    {
        get => _statesController.value.Contains(WidgetState.dragged);
        set
        {
            var __value = value;
            _statesController.update(WidgetState.dragged, __value);
        }
    }
    public virtual bool isFocused
    {
        get => _statesController.value.Contains(WidgetState.focused);
        set
        {
            var __value = value;
            _statesController.update(
                DartRuntimePrimitives.RequireValue(WidgetState.focused),
                __value
            );
        }
    }
    public virtual bool isEnabled
    {
        get => !_statesController.value.Contains(WidgetState.disabled);
        set
        {
            var __value = value;
            _statesController.update(WidgetState.disabled, !__value);
        }
    }

    public override void initState()
    {
        base.initState();
        if (widget.focusNode is null)
        {
            _internalFocusNode = new FocusNode();
        }
        isEnabled = widget.onPressed is not null;
        isFocused = _focusNode.hasPrimaryFocus;
    }

    public override void didUpdateWidget(
        _CupertinoMenuItemInteractionHandler__menu_anchor oldWidget
    )
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.focusNode, oldWidget.focusNode))
        {
            if (widget.focusNode is not null)
            {
                _internalFocusNode?.dispose();
                _internalFocusNode = null;
            }
            else
            {
                DartRuntimePrimitives.Assert(() => _internalFocusNode is null);
                _internalFocusNode = new FocusNode();
            }
            isFocused = _focusNode.hasPrimaryFocus;
        }
        if (!Equals(widget.onPressed, oldWidget.onPressed))
        {
            if (widget.onPressed is null)
            {
                isEnabled = isHovered = isPressed = isSwiped = isFocused = false;
            }
            else
            {
                isEnabled = true;
            }
        }
    }

    public override void dispose()
    {
        _statesController.dispose();
        _internalFocusNode?.dispose();
        _internalFocusNode = null;
        base.dispose();
    }

    internal virtual void _handleFocusChange(bool? focused = null)
    {
        isFocused = _focusNode.hasPrimaryFocus;
        widget.onFocusChange?.Invoke(isFocused);
    }

    internal virtual void _handleActivation(Intent? intent = null)
    {
        isSwiped = isPressed = false;
        widget.onPressed?.Invoke();
    }

    internal virtual void _handleTapDown(Gestures.TapDownDetails details)
    {
        isPressed = true;
    }

    internal virtual void _handleTapUp(Gestures.TapUpDetails? details)
    {
        isPressed = false;
        widget.onPressed?.Invoke();
    }

    internal virtual void _handleTapCancel()
    {
        isPressed = false;
    }

    internal virtual void _handlePointerExit(Gestures.PointerExitEvent @event)
    {
        if (isHovered)
        {
            isHovered = isFocused = false;
            widget.onHover?.Invoke(false);
        }
    }

    internal virtual void _handlePointerHover(Gestures.PointerHoverEvent @event)
    {
        if (!isHovered)
        {
            isHovered = true;
            widget.onHover?.Invoke(true);
            if (widget.requestFocusOnHover)
            {
                _focusNode.requestFocus();
                FocusTraversalGroup.of(context).invalidateScopeData(FocusScope.of(context));
            }
        }
    }

    internal virtual void _handleDismissMenu()
    {
        Actions.invoke(context, new DismissIntent());
    }

    internal virtual void _handleSwipeEnter()
    {
        if (!isEnabled)
        {
            return;
        }
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.android:
            {
                DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
                break;
            }
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            case TargetPlatform.macOS:
            {
                break;
            }
        }
        isSwiped = true;
    }

    internal virtual void _handleSwipeExit()
    {
        if (mounted)
        {
            isSwiped = false;
        }
    }

    internal virtual void _handleSwipeCompleted()
    {
        if (mounted && isEnabled)
        {
            _handleActivation();
        }
    }

    internal virtual Widget _buildStatefulAppearance(
        BuildContext context,
        HashSet<WidgetState> value,
        Widget? child
    )
    {
        MouseCursor cursorLocal = widget.mouseCursor.resolve(value);
        BoxDecoration decorationLocal = widget.decoration.resolve(value);
        bool hasBackground =
            (decorationLocal.color is not null) || (decorationLocal.gradient is not null);
        return new MouseRegion(
            onHover: isEnabled ? _handlePointerHover : null,
            onExit: isEnabled ? _handlePointerExit : null,
            hitTestBehavior: HitTestBehavior.deferToChild,
            cursor: cursorLocal,
            child: new DecoratedBox(
                decoration: decorationLocal.copyWith(
                    color: CupertinoDynamicColor.maybeResolve(decorationLocal.color, context),
                    backgroundBlendMode: (
                        Foundation.ConstantsLibrary.kIsWeb
                        || !hasBackground
                        || (decorationLocal.backgroundBlendMode is not null)
                    )
                        ? decorationLocal.backgroundBlendMode
                        : (
                            Equals(CupertinoTheme.maybeBrightnessOf(context), Brightness.light)
                                ? BlendMode.multiply
                                : BlendMode.plus
                        )
                ),
                child: child
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        Gestures.DeviceGestureSettings? newGestureSettings = MediaQuery.maybeGestureSettingsOf(
            context
        );
        if (!Equals(_gestureSettings, newGestureSettings))
        {
            _gestureSettings = newGestureSettings;
            _gestures = null;
        }
        _gestures ??= new DartMap<Type, dynamic>
        {
            [typeof(Gestures.TapGestureRecognizer)] =
                new GestureRecognizerFactoryWithHandlers<Gestures.TapGestureRecognizer>(
                    () => new Gestures.TapGestureRecognizer(debugOwner: this),
                    (instance) =>
                    {
                        DartRuntimePrimitives.Ignore(
                            (
                                (Func<Gestures.TapGestureRecognizer>)(
                                    () =>
                                    {
                                        var __cascade = instance;
                                        __cascade.onTapDown = _handleTapDown;
                                        __cascade.onTapUp = _handleTapUp;
                                        __cascade.onTapCancel = _handleTapCancel;
                                        __cascade.gestureSettings = _gestureSettings;
                                        return __cascade;
                                    }
                                )
                            )()
                        );
                    }
                ),
        };
        return new MergeSemantics(
            child: Widgets.Semantics.CreateFromProperties(
                properties: new Semantics.SemanticsProperties(
                    enabled: isEnabled,
                    onDismiss: isEnabled ? _handleDismissMenu : null
                ),
                child: new Actions(
                    actions: isEnabled ? _actions : new DartMap<Type, dynamic>(),
                    child: new Focus(
                        autofocus: isEnabled && widget.autofocus,
                        focusNode: _focusNode,
                        canRequestFocus: isEnabled,
                        skipTraversal: !isEnabled,
                        onFocusChange: value => _handleFocusChange(value),
                        child: new _SwipeTarget__menu_anchor(
                            onEnter: () => _handleSwipeEnter(),
                            onExit: () => _handleSwipeExit(),
                            onCompletion: () => _handleSwipeCompleted(),
                            child: new ValueListenableBuilder<HashSet<WidgetState>>(
                                valueListenable: _statesController,
                                builder: _buildStatefulAppearance,
                                child: new RawGestureDetector(
                                    behavior: widget.behavior,
                                    gestures: isEnabled ? _gestures! : new DartMap<Type, dynamic>(),
                                    child: widget.child
                                )
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _SwipeTarget__menu_anchor : StatelessWidget
{
    public virtual Action? onEnter { get; private set; }
    public virtual Action? onExit { get; private set; }
    public virtual Action? onCompletion { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    internal _SwipeTarget__menu_anchor(
        Action? onEnter,
        Action? onExit,
        Action? onCompletion,
        Widget child
    )
    {
        this.onEnter = onEnter;
        this.onExit = onExit;
        this.onCompletion = onCompletion;
        this.child = child;
    }

    public virtual bool isOpaque => true;

    public override Widget build(BuildContext context)
    {
        return new MetaData(metaData: this, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _SwipeScope__menu_anchor : InheritedWidget
{
    public virtual _SwipeRegionState__menu_anchor state { get; private set; } = default!;

    internal _SwipeScope__menu_anchor(Widget child, _SwipeRegionState__menu_anchor state)
        : base(child: child)
    {
        this.state = state;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_SwipeScope__menu_anchor)oldWidget;
        return !Equals(state, __oldWidget.state);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class _SwipeRegion__menu_anchor : StatefulWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual Action<double> onDistanceChanged { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _SwipeRegion__menu_anchor(
        bool enabled = true,
        Action<double> onDistanceChanged = default!,
        Widget child = default!
    )
    {
        this.enabled = enabled;
        this.onDistanceChanged = onDistanceChanged;
        this.child = child;
    }

    public static _SwipeRegionState__menu_anchor? of(BuildContext context)
    {
        _SwipeScope__menu_anchor? scope =
            context.dependOnInheritedWidgetOfExactType<_SwipeScope__menu_anchor>();
        return scope?.state;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _SwipeRegionState__menu_anchor());
}

public class _SwipeRegionState__menu_anchor : State<_SwipeRegion__menu_anchor>
{
    internal virtual HashSet<_RenderSwipeSurface__menu_anchor> _surfaces { get; private set; } =
        new HashSet<_RenderSwipeSurface__menu_anchor>();
    internal virtual Gestures.MultiDragGestureRecognizer? _recognizer { get; set; } = default;
    internal virtual Offset? _position { get; set; } = default;

    public virtual bool isSwiping =>
        DartRuntimePrimitives.ConvertValue<bool>(_position is not null);

    public override void didUpdateWidget(_SwipeRegion__menu_anchor oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (widget.enabled != oldWidget.enabled)
        {
            if (!widget.enabled)
            {
                _recognizer?.dispose();
                _recognizer = null;
                _position = null;
                widget.onDistanceChanged(0);
            }
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _recognizer?.gestureSettings = MediaQuery.maybeGestureSettingsOf(context);
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => !Enumerable.Any(_surfaces));
        _recognizer?.dispose();
        _recognizer = null;
        base.dispose();
    }

    public virtual void attachSurface(_RenderSwipeSurface__menu_anchor surface)
    {
        _surfaces.Add(surface);
    }

    public virtual void detachSurface(_RenderSwipeSurface__menu_anchor surface)
    {
        _surfaces.Remove(surface);
    }

    public virtual void beginSwipe(
        Gestures.PointerDownEvent @event,
        Duration delay = default,
        Action? onStart = null
    )
    {
        if (isSwiping || !widget.enabled)
        {
            return;
        }
        _recognizer?.dispose();
        _recognizer = null;
        Gestures.Drag handleStart(Offset position)
        {
            onStart?.Invoke();
            return _createSwipeHandle(position);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if (Equals(delay, Duration.zero))
        {
            _recognizer = DartRuntimePrimitives.ConvertValue<Gestures.MultiDragGestureRecognizer>(
                (
                    (Func<Gestures.ImmediateMultiDragGestureRecognizer>)(
                        () =>
                        {
                            var __cascade = new Gestures.ImmediateMultiDragGestureRecognizer(
                                allowedButtonsFilter: (button) =>
                                    button == Gestures.EventsLibrary.kPrimaryButton
                            );
                            __cascade.onStart = handleStart;
                            return __cascade;
                        }
                    )
                )()
            );
        }
        else
        {
            _recognizer = DartRuntimePrimitives.ConvertValue<Gestures.MultiDragGestureRecognizer>(
                (
                    (Func<Gestures.DelayedMultiDragGestureRecognizer>)(
                        () =>
                        {
                            var __cascade = new Gestures.DelayedMultiDragGestureRecognizer(
                                delay: delay,
                                allowedButtonsFilter: (button) =>
                                    button == Gestures.EventsLibrary.kPrimaryButton
                            );
                            __cascade.onStart = handleStart;
                            return __cascade;
                        }
                    )
                )()
            );
        }
        _recognizer!.gestureSettings = MediaQuery.maybeGestureSettingsOf(context);
        _recognizer!.addPointer(@event);
    }

    internal virtual Gestures.Drag _createSwipeHandle(Offset position)
    {
        DartRuntimePrimitives.Assert(
            () => !isSwiping,
            () => (object?)"A new swipe should not begin while a swipe is active."
        );
        _position = position;
        return new _SwipeHandle__menu_anchor(
            viewId: checked((long)View.of(context).viewId),
            initialPosition: position,
            onSwipeUpdate: _handleSwipeUpdate,
            onSwipeEnd: _handleSwipeEnd,
            onSwipeCanceled: () => _handleSwipeCancel()
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSwipeUpdate(Gestures.DragUpdateDetails updateDetails)
    {
        _position = DartRuntimePrimitives.RequireValue(_position) + updateDetails.delta;
        double minimumSquaredDistance = double.MaxValue;
        foreach (_RenderSwipeSurface__menu_anchor surface in _surfaces)
        {
            double squaredDistance = Menu_anchorLibrary._computeSquaredDistanceToRect(
                DartRuntimePrimitives.RequireValue(_position),
                surface.computeRect()
            );
            if (squaredDistance.floor() == 0L)
            {
                widget.onDistanceChanged(0);
                return;
            }
            minimumSquaredDistance = Math.Min(squaredDistance, minimumSquaredDistance);
        }
        double distance =
            (minimumSquaredDistance == 0L) ? 0 : Dart_mathLibrary.sqrt(minimumSquaredDistance);
        widget.onDistanceChanged(distance);
    }

    internal virtual void _handleSwipeEnd(Gestures.DragEndDetails updateDetails)
    {
        _completeSwipe();
    }

    internal virtual void _handleSwipeCancel()
    {
        _completeSwipe();
    }

    internal virtual void _completeSwipe()
    {
        _recognizer?.dispose();
        _recognizer = null;
        _position = null;
        if (mounted)
        {
            widget.onDistanceChanged(0);
        }
    }

    public override Widget build(BuildContext context)
    {
        return new _SwipeScope__menu_anchor(state: this, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _SwipeSurface__menu_anchor : SingleChildRenderObjectWidget
{
    public virtual Duration delay { get; private set; } = default!;
    public virtual Action? onStart { get; private set; }

    internal _SwipeSurface__menu_anchor(
        Widget? child,
        Duration delay = default,
        Action? onStart = null
    )
        : base(child: child)
    {
        this.delay = delay;
        this.onStart = onStart;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSwipeSurface__menu_anchor(
            region: _SwipeRegion__menu_anchor.of(context)!,
            delay: delay,
            onStart: onStart
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderSwipeSurface__menu_anchor)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderSwipeSurface__menu_anchor>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.region = _SwipeRegion__menu_anchor.of(context)!;
                        __cascade.delay = delay;
                        __cascade.onStart = onStart;
                        return __cascade;
                    }
                )
            )()
        );
    }
}

public class _RenderSwipeSurface__menu_anchor : RenderProxyBoxWithHitTestBehavior
{
    internal virtual _SwipeRegionState__menu_anchor _region { get; set; } = default!;
    public virtual Duration delay { get; set; } = default!;
    public virtual Action? onStart { get; set; } = default;

    internal _RenderSwipeSurface__menu_anchor(
        _SwipeRegionState__menu_anchor region,
        Duration delay,
        Action? onStart
    )
        : base(behavior: HitTestBehavior.opaque)
    {
        this.delay = delay;
        this.onStart = onStart;
        _region = region;
        _region.attachSurface(this);
    }

    public virtual _SwipeRegionState__menu_anchor region
    {
        get => _region;
        set
        {
            var __value = value;
            if (!Equals(_region, __value))
            {
                _region.detachSurface(this);
                _region = __value;
                _region.attachSurface(this);
            }
        }
    }

    public virtual Rect computeRect() =>
        DartRuntimePrimitives.ConvertValue<Rect>(localToGlobal(Offset.zero) & size);

    public override void detach()
    {
        _region.detachSurface(this);
        base.detach();
    }

    public override void dispose()
    {
        _region.detachSurface(this);
        base.dispose();
    }

    public override void handleEvent(
        Gestures.PointerEvent @event,
        Gestures.HitTestEntry<Gestures.HitTestTarget> entry
    )
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if (@event is Gestures.PointerDownEvent)
        {
            Gestures.PointerDownEvent @event__as103538 = (Gestures.PointerDownEvent)@event;
            _region.beginSwipe(@event__as103538, delay: delay, onStart: onStart);
        }
    }
}

internal class _SwipeHandle__menu_anchor : Gestures.Drag
{
    public virtual long viewId { get; private set; } = default!;
    internal virtual List<_SwipeTarget__menu_anchor> _enteredTargets { get; private set; } =
        new List<_SwipeTarget__menu_anchor>();
    public virtual Action<Gestures.DragUpdateDetails> onSwipeUpdate { get; private set; } =
        default!;
    public virtual Action<Gestures.DragEndDetails> onSwipeEnd { get; private set; } = default!;
    public virtual Action onSwipeCanceled { get; private set; } = default!;
    internal virtual Offset _position { get; set; } = default!;

    internal _SwipeHandle__menu_anchor(
        Offset initialPosition,
        long viewId,
        Action<Gestures.DragEndDetails> onSwipeEnd,
        Action<Gestures.DragUpdateDetails> onSwipeUpdate,
        Action onSwipeCanceled
    )
    {
        this.viewId = viewId;
        this.onSwipeEnd = onSwipeEnd;
        this.onSwipeUpdate = onSwipeUpdate;
        this.onSwipeCanceled = onSwipeCanceled;
        _position = initialPosition;
        _updateSwipe();
    }

    public override void update(Gestures.DragUpdateDetails details)
    {
        Offset oldPosition = _position;
        _position += details.delta;
        if (!Equals(_position, oldPosition))
        {
            _updateSwipe();
            onSwipeUpdate?.Invoke(details);
        }
    }

    public override void end(Gestures.DragEndDetails details)
    {
        _leaveAllEntered(pointerUp: true);
        onSwipeEnd?.Invoke(details);
    }

    public override void cancel()
    {
        _leaveAllEntered();
        onSwipeCanceled();
    }

    internal virtual void _updateSwipe()
    {
        var result = new Gestures.HitTestResult();
        WidgetsBinding.instance.hitTestInView(result, _position, viewId);
        var targets = new List<_SwipeTarget__menu_anchor>();
        foreach (Gestures.HitTestEntry<Gestures.HitTestTarget> entry in result.path)
        {
            if (
                entry.target is RenderMetaData
                {
                    metaData: _SwipeTarget__menu_anchor metaDataLocal
                } __object105049
            )
            {
                targets.Add(metaDataLocal);
            }
        }
        _enteredTargets.removeWhere(
            (target) =>
            {
                if (!targets.Contains(target))
                {
                    target.onExit?.Invoke();
                    return true;
                }
                return false;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        var hitTargets = new HashSet<_SwipeTarget__menu_anchor>();
        var newlyEnteredTargets = new List<_SwipeTarget__menu_anchor>();
        var hitExistingTarget = false;
        foreach (var targetLocal in targets)
        {
            if (_enteredTargets.Contains(targetLocal))
            {
                hitTargets.Add(targetLocal);
                hitExistingTarget = true;
                continue;
            }
            if (!hitExistingTarget)
            {
                hitTargets.Add(targetLocal);
                newlyEnteredTargets.Add(targetLocal);
            }
            if (targetLocal.isOpaque)
            {
                break;
            }
        }
        foreach (_SwipeTarget__menu_anchor targetAlternate in Enumerable.Reverse(_enteredTargets))
        {
            if (!hitTargets.Contains(targetAlternate))
            {
                targetAlternate.onExit?.Invoke();
            }
        }
        foreach (_SwipeTarget__menu_anchor targetNested in Enumerable.Reverse(newlyEnteredTargets))
        {
            targetNested.onEnter?.Invoke();
        }
        DartRuntimePrimitives.Ignore(
            (
                (Func<List<_SwipeTarget__menu_anchor>>)(
                    () =>
                    {
                        var __cascade = _enteredTargets;
                        __cascade.Clear();
                        __cascade.AddRange(hitTargets);
                        return __cascade;
                    }
                )
            )()
        );
    }

    internal virtual void _leaveAllEntered(bool pointerUp = false)
    {
        for (var i = 0L; i < checked(_enteredTargets.Count); i += 1L)
        {
            _SwipeTarget__menu_anchor target = _enteredTargets[(int)i];
            target.onExit?.Invoke();
            if (pointerUp)
            {
                target.onCompletion?.Invoke();
            }
        }
        _enteredTargets.Clear();
    }
}

internal class _AnimationProduct__menu_anchor : CompoundAnimation<double>
{
    internal _AnimationProduct__menu_anchor(Animation<double> first, Animation<double> next)
        : base(first: first, next: next) { }

    public override double value =>
        DartRuntimePrimitives.ConvertValue<double>(base.first.value * base.next.value);
}

internal class _ClampTween__menu_anchor : Animatable<double>
{
    public virtual double begin { get; private set; } = default!;
    public virtual double end { get; private set; } = default!;

    internal _ClampTween__menu_anchor(double begin, double end)
    {
        this.begin = begin;
        this.end = end;
    }

    public override double transform(double t)
    {
        if (t < begin)
        {
            return begin;
        }
        if (t > end)
        {
            return end;
        }
        return t;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
