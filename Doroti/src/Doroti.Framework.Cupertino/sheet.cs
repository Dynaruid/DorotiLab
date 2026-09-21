// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/sheet.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class SheetLibrary
{
    internal static double _kDeviceCornerRadiusSmoothingFactor = 0.9;
}

public static partial class SheetLibrary
{
    internal static double _kRoundedDeviceCornersThreshold = 20.0;
}

public static partial class SheetLibrary
{
    internal static double _kTopGapRatio = 0.08;
}

public static partial class SheetLibrary
{
    internal static double _kStretchedTopGapRatio = 0.072;
}

public static partial class SheetLibrary
{
    internal static Animatable<Offset> _kBottomUpTween = new Tween<Offset>(
        begin: new Offset(0.0, 1.0),
        end: Offset.zero
    );
}

public static partial class SheetLibrary
{
    internal static Animatable<Offset> _kBottomUpTweenWhenCoveringOtherSheet = new Tween<Offset>(
        begin: new Offset(0.0, 1.0),
        end: new Offset(0.0, -0.02)
    );
}

public static partial class SheetLibrary
{
    internal static Animatable<Offset> _kMidUpTween = new Tween<Offset>(
        begin: Offset.zero,
        end: new Offset(0.0, -0.005)
    );
}

public static partial class SheetLibrary
{
    internal static Animatable<Offset> _kTopDownTween = new Tween<Offset>(
        begin: Offset.zero,
        end: new Offset(0.0, 0.07)
    );
}

public static partial class SheetLibrary
{
    internal static Animatable<double> _kOpacityTween = new Tween<double>(begin: 0.0, end: 0.1);
}

public static partial class SheetLibrary
{
    internal static double _kMinFlingVelocity = 2.0;
}

public static partial class SheetLibrary
{
    internal static Duration _kDroppedSheetDragAnimationDuration = Duration.Create(
        milliseconds: 300L
    );
}

public static partial class SheetLibrary
{
    internal static double _kSheetScaleFactor = 0.0835;
}

public static partial class SheetLibrary
{
    internal static Animatable<double> _kScaleTween = new Tween<double>(
        begin: 1.0,
        end: 1.0 - _kSheetScaleFactor
    );
}

internal delegate void _DragStartCallback__sheet();

internal delegate void _DragUpdateCallback__sheet(double delta);

internal delegate void _DragEndCallback__sheet(double velocity);

internal delegate bool _GetSheetDragged__sheet();

public static partial class SheetLibrary
{
    public static Future<T?> showCupertinoSheet<T>(
        BuildContext context,
        Func<BuildContext, Widget>? pageBuilder = null,
        Func<BuildContext, Widget>? builder = null,
        Func<BuildContext, ScrollController, Widget>? scrollableBuilder = null,
        bool useNestedNavigation = false,
        bool enableDrag = true,
        RouteSettings? settings = null,
        double? topGap = null,
        bool showDragHandle = false
    )
    {
        DartRuntimePrimitives.Assert(
            () => (topGap is null) || ((topGap >= 0.0) && (topGap <= 0.9)),
            () => (object?)"topGap must be between 0.0 and 0.9"
        );
        DartRuntimePrimitives.Assert(() =>
            (pageBuilder is not null) || (builder is not null) || (scrollableBuilder is not null)
        );
        DartRuntimePrimitives.Assert(() =>
            ((pageBuilder is null) && (builder is null) && (scrollableBuilder is not null))
            || (scrollableBuilder is null)
        );
        Func<BuildContext, Widget>? effectiveBuilder = builder ?? pageBuilder;
        var nestedNavigatorKey = GlobalKey<NavigatorState>.Create();
        if (!useNestedNavigation)
        {
            PageRoute<T> route = new CupertinoSheetRoute<T>(
                builder: effectiveBuilder,
                scrollableBuilder: scrollableBuilder,
                settings: settings,
                enableDrag: enableDrag,
                topGap: topGap
            );
            return Navigator.of(context, rootNavigator: true).push(route);
        }
        else
        {
            Widget nestedNavigationContent(Func<BuildContext, Widget> builder)
            {
                return new NavigatorPopHandler<T>(
                    onPopWithResult: (result) =>
                    {
                        DartRuntimePrimitives.Ignore(
                            nestedNavigatorKey.currentState!.maybePop<object>()
                        );
                    },
                    child: new Navigator(
                        key: nestedNavigatorKey,
                        initialRoute: "/",
                        onGenerateInitialRoutes: (navigator, initialRouteName) =>
                        {
                            return new List<object>
                            {
                                new CupertinoPageRoute<object?>(
                                    builder: (context) =>
                                    {
                                        return new PopScope<object>(
                                            canPop: false,
                                            onPopInvokedWithResult: (didPop, result) =>
                                            {
                                                if (didPop)
                                                {
                                                    return;
                                                }
                                                Navigator
                                                    .of(context, rootNavigator: true)
                                                    .pop(result);
                                            },
                                            child: builder(context)
                                        );
                                        throw new InvalidOperationException(
                                            "Dart closure completed without a value."
                                        );
                                    }
                                ),
                            };
                            throw new InvalidOperationException(
                                "Dart closure completed without a value."
                            );
                        }
                    )
                );
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            var routeLocal = new CupertinoSheetRoute<T>(
                scrollableBuilder: (context, controller) =>
                    nestedNavigationContent(
                        (scrollableBuilder is not null)
                            ? ((context) => scrollableBuilder(context, controller))
                            : effectiveBuilder!
                    ),
                settings: settings,
                enableDrag: enableDrag,
                topGap: topGap
            );
            return Navigator.of(context, rootNavigator: true).push(routeLocal);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoSheetTransition : StatefulWidget
{
    public virtual Animation<double> primaryRouteAnimation { get; private set; } = default!;
    public virtual Animation<double> secondaryRouteAnimation { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    public virtual bool linearTransition { get; private set; } = default!;
    public virtual double topGap { get; private set; } = default!;

    public CupertinoSheetTransition(
        Key? key = null,
        Animation<double> primaryRouteAnimation = default!,
        Animation<double> secondaryRouteAnimation = default!,
        Widget child = default!,
        bool linearTransition = default!,
        double? topGap = null
    )
        : base(key: key)
    {
        double __topGap = topGap ?? SheetLibrary._kTopGapRatio;
        this.primaryRouteAnimation = primaryRouteAnimation;
        this.secondaryRouteAnimation = secondaryRouteAnimation;
        this.child = child;
        this.linearTransition = linearTransition;
        this.topGap = __topGap;
    }

    public static Widget delegateTransition(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        bool allowSnapshotting,
        Widget? child
    )
    {
        if (CupertinoSheetRoute<object>.hasParentSheet(context))
        {
            return _delegatedCoverSheetSecondaryTransition(secondaryAnimation, child);
        }
        bool linearLocal = Navigator.of(context).userGestureInProgress;
        Curve curveLocal = linearLocal ? Curves.linear : Curves.linearToEaseOut;
        Curve reverseCurveLocal = linearLocal ? Curves.linear : Curves.easeInToLinear;
        var curvedAnimation = new CurvedAnimation(
            curve: curveLocal,
            reverseCurve: reverseCurveLocal,
            parent: secondaryAnimation
        );
        double deviceCornerRadius =
            (MediaQuery.maybeViewPaddingOf(context)?.top ?? 0)
            * SheetLibrary._kDeviceCornerRadiusSmoothingFactor;
        bool roundedDeviceCorners =
            deviceCornerRadius > SheetLibrary._kRoundedDeviceCornersThreshold;
        Animatable<BorderRadiusGeometry> decorationTween = new Tween<BorderRadiusGeometry>(
            begin: BorderRadius.CreateVertical(
                top: Radius.circular(roundedDeviceCorners ? deviceCornerRadius : 0)
            ),
            end: BorderRadius.CreateAll(Radius.circular(12))
        );
        Animation<BorderRadiusGeometry> radiusAnimation = curvedAnimation.drive(decorationTween);
        Animation<double> opacityAnimation = curvedAnimation.drive(SheetLibrary._kOpacityTween);
        Animation<Offset> slideAnimation = curvedAnimation.drive(SheetLibrary._kTopDownTween);
        Animation<double> scaleAnimation = curvedAnimation.drive(SheetLibrary._kScaleTween);
        curvedAnimation.dispose();
        var isDarkMode = Equals(CupertinoTheme.brightnessOf(context), Brightness.dark);
        var overlayColor = isDarkMode ? new Color(4291348680L) : new Color(4278190080L);
        Widget? contrastedChild =
            ((child is not null) && !secondaryAnimation.isDismissed)
                ? new Stack(
                    children: new List<Widget>
                    {
                        DartRuntimePrimitives.ConvertValue<Widget>(child),
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new FadeTransition(
                                opacity: opacityAnimation,
                                child: new ColoredBox(
                                    color: overlayColor,
                                    child: SizedBox.CreateExpand()
                                )
                            )
                        ),
                    }
                )
                : child;
        double topGapHeight = MediaQuery.sizeOf(context).height * SheetLibrary._kTopGapRatio;
        return new Stack(
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new AnnotatedRegion<SystemUiOverlayStyle>(
                        value: new SystemUiOverlayStyle(
                            statusBarBrightness: Brightness.dark,
                            statusBarIconBrightness: Brightness.light
                        ),
                        child: new SizedBox(height: topGapHeight, width: double.PositiveInfinity)
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new SlideTransition(
                        position: slideAnimation,
                        child: new ScaleTransition(
                            scale: scaleAnimation,
                            filterQuality: FilterQuality.medium,
                            alignment: Alignment.topCenter,
                            child: new AnimatedBuilder(
                                animation: radiusAnimation,
                                child: child,
                                builder: (context, child) =>
                                {
                                    return new ClipRSuperellipse(
                                        borderRadius: !secondaryAnimation.isDismissed
                                            ? radiusAnimation.value
                                            : BorderRadius.zero,
                                        child: contrastedChild
                                    );
                                    throw new InvalidOperationException(
                                        "Dart closure completed without a value."
                                    );
                                }
                            )
                        )
                    )
                ),
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Widget _delegatedCoverSheetSecondaryTransition(
        Animation<double> secondaryAnimation,
        Widget? child
    )
    {
        Curve curveLocal = Curves.linearToEaseOut;
        Curve reverseCurveLocal = Curves.easeInToLinear;
        var curvedAnimation = new CurvedAnimation(
            curve: curveLocal,
            reverseCurve: reverseCurveLocal,
            parent: secondaryAnimation
        );
        Animation<Offset> slideAnimation = curvedAnimation.drive(SheetLibrary._kMidUpTween);
        Animation<double> scaleAnimation = curvedAnimation.drive(SheetLibrary._kScaleTween);
        curvedAnimation.dispose();
        return new SlideTransition(
            position: slideAnimation,
            transformHitTests: false,
            child: new ScaleTransition(
                scale: scaleAnimation,
                filterQuality: FilterQuality.medium,
                alignment: Alignment.topCenter,
                child: new ClipRSuperellipse(
                    borderRadius: BorderRadius.CreateVertical(top: Radius.circular(12)),
                    child: child
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoSheetTransitionState__sheet());
}

internal class _CupertinoSheetTransitionState__sheet
    : State<CupertinoSheetTransition>,
        SingleTickerProviderStateMixin<CupertinoSheetTransition>
{
    internal virtual AnimationController _stretchDragController { get; set; } = default!;
    internal virtual Animation<double> _stretchDragAnimation { get; set; } = default!;
    internal virtual Animation<Offset> _secondaryPositionAnimation { get; set; } = default!;
    internal virtual Animation<double> _secondaryScaleAnimation { get; set; } = default!;
    internal virtual CurvedAnimation? _primaryPositionCurve { get; set; } = default;
    internal virtual CurvedAnimation? _secondaryPositionCurve { get; set; } = default;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _stretchDragController = new AnimationController(
            duration: Duration.Create(microseconds: 1L),
            vsync: this
        );
        _setupAnimation();
    }

    public override void didUpdateWidget(CupertinoSheetTransition oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (
            (!Equals(oldWidget.primaryRouteAnimation, widget.primaryRouteAnimation))
            || (!Equals(oldWidget.secondaryRouteAnimation, widget.secondaryRouteAnimation))
        )
        {
            _disposeCurve();
            _setupAnimation();
        }
    }

    public override void dispose()
    {
        _disposeCurve();
        _stretchDragController.dispose();
        DartRuntimePrimitives.Assert(() =>
        {
            if ((_ticker is null) || !_ticker!.isActive)
            {
                return true;
            }
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary($"{this} was disposed with an active Ticker."),
                        new ErrorDescription(
                            $"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time "
                                + "dispose() was called on the mixin, that Ticker was still active. The Ticker must "
                                + "be disposed before calling super.dispose()."
                        ),
                        new ErrorHint(
                            "Tickers used by AnimationControllers "
                                + "should be disposed by calling dispose() on the AnimationController itself. "
                                + "Otherwise, the ticker will leak."
                        ),
                        _ticker!.describeForError("The offending ticker was"),
                    }
                )
            );
        });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _setupAnimation()
    {
        _primaryPositionCurve = new CurvedAnimation(
            curve: Curves.fastEaseInToSlowEaseOut,
            reverseCurve: Curves.fastEaseInToSlowEaseOut.flipped,
            parent: widget.primaryRouteAnimation
        );
        _secondaryPositionCurve = new CurvedAnimation(
            curve: Curves.linearToEaseOut,
            reverseCurve: Curves.easeInToLinear,
            parent: widget.secondaryRouteAnimation
        );
        double stretchDistance = SheetLibrary._kTopGapRatio - SheetLibrary._kStretchedTopGapRatio;
        double stretchedTopGap = widget.topGap - stretchDistance;
        _stretchDragAnimation = _stretchDragController.drive(
            new Tween<double>(begin: widget.topGap, end: stretchedTopGap)
        );
        _secondaryPositionAnimation = _secondaryPositionCurve!.drive(SheetLibrary._kMidUpTween);
        _secondaryScaleAnimation = _secondaryPositionCurve!.drive(SheetLibrary._kScaleTween);
    }

    internal virtual void _disposeCurve()
    {
        _primaryPositionCurve?.dispose();
        _secondaryPositionCurve?.dispose();
        _primaryPositionCurve = null;
        _secondaryPositionCurve = null;
    }

    internal virtual Widget _coverSheetPrimaryTransition(
        BuildContext context,
        Animation<double> animation,
        bool linearTransition,
        Widget? child
    )
    {
        Animatable<Offset> offsetTween = CupertinoSheetRoute<object>.hasParentSheet(context)
            ? SheetLibrary._kBottomUpTweenWhenCoveringOtherSheet
            : SheetLibrary._kBottomUpTween;
        var curvedAnimation = new CurvedAnimation(
            parent: animation,
            curve: linearTransition ? Curves.linear : Curves.fastEaseInToSlowEaseOut,
            reverseCurve: linearTransition ? Curves.linear : Curves.fastEaseInToSlowEaseOut.flipped
        );
        Animation<Offset> positionAnimation = curvedAnimation.drive(offsetTween);
        curvedAnimation.dispose();
        return new SlideTransition(position: positionAnimation, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _coverSheetSecondaryTransition(
        Animation<double> secondaryAnimation,
        Widget? child
    )
    {
        return new SlideTransition(
            position: _secondaryPositionAnimation,
            transformHitTests: false,
            child: new ScaleTransition(
                scale: _secondaryScaleAnimation,
                filterQuality: FilterQuality.medium,
                alignment: Alignment.topCenter,
                child: child
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new _StretchDragControllerProvider__sheet(
            controller: _stretchDragController,
            child: SizedBox.CreateExpand(
                child: new AnimatedBuilder(
                    animation: _stretchDragAnimation,
                    builder: (context, child) =>
                    {
                        return new Padding(
                            padding: EdgeInsets.CreateOnly(
                                top: MediaQuery.heightOf(context) * _stretchDragAnimation.value
                            ),
                            child: _coverSheetSecondaryTransition(
                                widget.secondaryRouteAnimation,
                                _coverSheetPrimaryTransition(
                                    context,
                                    widget.primaryRouteAnimation,
                                    widget.linearTransition,
                                    widget.child
                                )
                            )
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

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_ticker is null)
            {
                return true;
            }
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."
                        ),
                        new ErrorDescription(
                            "A SingleTickerProviderStateMixin can only be used as a TickerProvider once."
                        ),
                        new ErrorHint(
                            "If a State is used for multiple AnimationController objects, or if it is passed to other "
                                + "objects and those objects might use it more than one time in total, then instead of "
                                + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin."
                        ),
                    }
                )
            );
        });
        _ticker = new Scheduler.Ticker(
            onTick,
            debugLabel: Foundation.ConstantsLibrary.kDebugMode
                ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                : null
        );
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
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch
        {
            (true, true) => "active but muted",
            (true, _) => "active",
            (false, true) => "inactive and muted",
            (false, _) => "inactive",
            (null, _) => DartRuntimePrimitives.ConvertValue<string>(null),
        };
        properties.add(
            new DiagnosticsProperty<Scheduler.Ticker>(
                "ticker",
                _ticker,
                description: tickerDescription,
                showSeparator: false,
                defaultValue: default
            )
        );
    }
}

internal class _StretchDragControllerProvider__sheet : InheritedWidget
{
    public virtual AnimationController controller { get; private set; } = default!;

    internal _StretchDragControllerProvider__sheet(AnimationController controller, Widget child)
        : base(child: child)
    {
        this.controller = controller;
    }

    public static _StretchDragControllerProvider__sheet? maybeOf(BuildContext context)
    {
        return context.getInheritedWidgetOfExactType<_StretchDragControllerProvider__sheet>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_StretchDragControllerProvider__sheet)oldWidget;
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoSheetRoute<T> : PageRoute<T>, _CupertinoSheetRouteTransitionMixin__sheet<T>
{
    public virtual Func<BuildContext, Widget>? builder { get; private set; }
    public virtual Func<BuildContext, ScrollController, Widget>? scrollableBuilder
    {
        get;
        private set;
    }
    public virtual bool enableDrag { get; private set; } = default!;
    internal virtual double? _topGap { get; private set; }
    public virtual bool showDragHandle { get; private set; } = default!;

    public CupertinoSheetRoute(
        RouteSettings? settings = null,
        Func<BuildContext, Widget>? builder = null,
        Func<BuildContext, ScrollController, Widget>? scrollableBuilder = null,
        bool enableDrag = true,
        bool showDragHandle = false,
        double? topGap = null
    )
        : base(settings: settings)
    {
        this.builder = builder;
        this.scrollableBuilder = scrollableBuilder;
        this.enableDrag = enableDrag;
        this.showDragHandle = showDragHandle;
        _topGap = topGap;
        System.Diagnostics.Debug.Assert((topGap is null) || ((topGap >= 0.0) && (topGap <= 0.9)));
        System.Diagnostics.Debug.Assert((builder is not null) || (scrollableBuilder is not null));
    }

    internal virtual Func<BuildContext, ScrollController, Widget> _effectiveBuilder
    {
        get { return scrollableBuilder ?? ((context, controller) => builder!(context)); }
    }
    public virtual double topGap =>
        DartRuntimePrimitives.ConvertValue<double>(_topGap ?? SheetLibrary._kTopGapRatio);
    public virtual bool _hasCustomTopGap =>
        DartRuntimePrimitives.ConvertValue<bool>(_topGap is not null);

    internal virtual Widget _sheetWithDragHandle(BuildContext context, ScrollController controller)
    {
        if (!showDragHandle)
        {
            return _effectiveBuilder(context, controller);
        }
        var dragHandleTopPadding = 5.0;
        var dragHandleHeight = 5.0;
        var dragHandleWidth = 36.0;
        var dragHandlePadding = 15.0;
        return new Stack(
            fit: StackFit.expand,
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new MediaQuery(
                        data: MediaQuery
                            .of(context)
                            .copyWith(padding: EdgeInsets.CreateOnly(top: dragHandlePadding)),
                        child: _effectiveBuilder(context, controller)
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new Align(
                        alignment: Alignment.topCenter,
                        child: new Padding(
                            padding: EdgeInsetsGeometry.CreateOnly(top: dragHandleTopPadding),
                            child: new DecoratedBox(
                                decoration: new ShapeDecoration(
                                    shape: new RoundedSuperellipseBorder(
                                        borderRadius: BorderRadiusGeometry.CreateAll(
                                            Radius.circular(dragHandleWidth / 2L)
                                        )
                                    ),
                                    color: CupertinoColors.tertiaryLabel
                                ),
                                child: new SizedBox(
                                    height: dragHandleHeight,
                                    width: dragHandleWidth
                                )
                            )
                        )
                    )
                ),
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildContent(BuildContext context)
    {
        return MediaQuery.CreateRemovePadding(
            context: context,
            removeTop: true,
            child: new ClipRSuperellipse(
                borderRadius: BorderRadius.CreateVertical(top: Radius.circular(12)),
                child: new CupertinoUserInterfaceLevel(
                    data: CupertinoUserInterfaceLevelData.elevated,
                    child: new _CupertinoSheetScope__sheet(
                        child: new _CupertinoDraggableScrollableSheet__sheet<T>(
                            enabledCallback: () => enableDrag,
                            onStartPopGesture: () =>
                                _CupertinoSheetRouteTransitionMixin__sheet<object>._startPopGesture(
                                    this,
                                    DartRuntimePrimitives.RequireValue(topGap)
                                ),
                            builder: _sheetWithDragHandle
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool hasParentSheet(BuildContext context)
    {
        return _CupertinoSheetScope__sheet.maybeOf(context) is not null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void popSheet(BuildContext context)
    {
        if (CupertinoSheetRoute<T>.hasParentSheet(context))
        {
            Navigator.of(context, rootNavigator: true).pop<object>();
        }
    }

    public override Color? barrierColor => CupertinoColors.transparent;
    public override bool barrierDismissible => false;
    public override string? barrierLabel => DartRuntimePrimitives.ConvertValue<string>(null);
    public override bool maintainState => true;
    public override bool opaque => false;
    public override Duration transitionDuration => Duration.Create(milliseconds: 500L);
    public override Func<
        BuildContext,
        Animation<double>,
        Animation<double>,
        bool,
        Widget?,
        Widget?
    >? delegatedTransition
    {
        get
        {
            if (_hasCustomTopGap)
            {
                return null;
            }
            return CupertinoSheetTransition.delegateTransition;
        }
    }

    public override Widget buildPage(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation
    )
    {
        return buildContent(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionFrom(dynamic previousRoute)
    {
        return !_hasCustomTopGap;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionTo(dynamic nextRoute)
    {
        if ((this is CupertinoSheetRoute<object>) && _hasCustomTopGap)
        {
            return false;
        }
        return nextRoute is _CupertinoSheetRouteTransitionMixin__sheet<object>;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildTransitions(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child
    )
    {
        return _CupertinoSheetRouteTransitionMixin__sheet<object>.buildPageTransitions(
            this,
            context,
            animation,
            secondaryAnimation,
            child,
            enableDrag,
            topGap
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _CupertinoSheetScope__sheet : InheritedWidget
{
    internal _CupertinoSheetScope__sheet(Widget child)
        : base(child: child) { }

    public static _CupertinoSheetScope__sheet? maybeOf(BuildContext context)
    {
        return context.getInheritedWidgetOfExactType<_CupertinoSheetScope__sheet>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => false;
}

public interface _CupertinoSheetRouteTransitionMixin__sheet<T>
{
    public Widget buildContent(BuildContext context);
    public Duration transitionDuration { get; }
    public Func<
        BuildContext,
        Animation<double>,
        Animation<double>,
        bool,
        Widget?,
        Widget?
    >? delegatedTransition { get; }
    public bool enableDrag { get; }
    public double topGap { get; }
    public bool _hasCustomTopGap { get; }
    public Widget buildPage(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation
    );
    public static _CupertinoDragGestureController__sheet<TRouteResult> _startPopGesture<TRouteResult>(
        ModalRoute<TRouteResult> route,
        double topGap
    )
    {
        return new _CupertinoDragGestureController__sheet<TRouteResult>(
            topGap: topGap,
            navigator: route.navigator!,
            getIsCurrent: () => route.isCurrent,
            getIsActive: () => route.isActive,
            popDragController: route.controller!
        );
    }
    public static Widget buildPageTransitions<TRouteResult>(
        ModalRoute<TRouteResult> route,
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child,
        bool enableDrag,
        double topGap
    )
    {
        bool linearTransitionLocal = route.popGestureInProgress;
        return new CupertinoSheetTransition(
            primaryRouteAnimation: animation,
            secondaryRouteAnimation: secondaryAnimation,
            linearTransition: linearTransitionLocal,
            topGap: topGap,
            child: new _CupertinoDragGestureDetector__sheet<TRouteResult>(
                enabledCallback: () => enableDrag,
                onStartPopGesture: () =>
                    _CupertinoSheetRouteTransitionMixin__sheet<TRouteResult>._startPopGesture(
                        route,
                        topGap
                    ),
                child: child
            )
        );
    }
    public bool canTransitionFrom(dynamic previousRoute);
    public bool canTransitionTo(dynamic nextRoute);
    public Widget buildTransitions(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child
    );
}

internal class _CupertinoDragGestureDetector__sheet<T> : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Func<bool> enabledCallback { get; private set; } = default!;
    public virtual Func<_CupertinoDragGestureController__sheet<T>> onStartPopGesture
    {
        get;
        private set;
    } = default!;

    internal _CupertinoDragGestureDetector__sheet(
        Key? key = null,
        Func<bool> enabledCallback = default!,
        Func<_CupertinoDragGestureController__sheet<T>> onStartPopGesture = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        this.enabledCallback = enabledCallback;
        this.onStartPopGesture = onStartPopGesture;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CupertinoDragGestureDetectorState__sheet<T>()
        );
}

internal class _CupertinoDragGestureDetectorState__sheet<T>
    : State<_CupertinoDragGestureDetector__sheet<T>>
{
    internal virtual _CupertinoDragGestureController__sheet<T>? _dragGestureController { get; set; } =
        default;
    internal virtual Gestures.VerticalDragGestureRecognizer _recognizer { get; set; } = default!;
    internal virtual _StretchDragControllerProvider__sheet? _stretchDragController { get; set; } =
        default;

    internal static Gestures.VelocityTracker _cupertinoVelocityBuilder(
        Gestures.PointerEvent @event
    ) =>
        DartRuntimePrimitives.ConvertValue<Gestures.VelocityTracker>(
            new Gestures.IOSScrollViewFlingVelocityTracker(@event.kind)
        );

    public virtual double sheetHeight => DartRuntimePrimitives.RequireValue(context.size).height;

    public override void initState()
    {
        base.initState();
        DartRuntimePrimitives.Assert(() => _stretchDragController is null);
        _stretchDragController = _StretchDragControllerProvider__sheet.maybeOf(context);
        _recognizer = (
            (Func<Gestures.VerticalDragGestureRecognizer>)(
                () =>
                {
                    var __cascade = new Gestures.VerticalDragGestureRecognizer(debugOwner: this);
                    __cascade.velocityTrackerBuilder = _cupertinoVelocityBuilder;
                    __cascade.onStart = _handleDragStart;
                    __cascade.onUpdate = _handleDragUpdate;
                    __cascade.onEnd = _handleDragEnd;
                    __cascade.onCancel = _handleDragCancel;
                    return __cascade;
                }
            )
        )();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _stretchDragController = _StretchDragControllerProvider__sheet.maybeOf(context);
    }

    public override void dispose()
    {
        _recognizer.dispose();
        if (_dragGestureController is not null)
        {
            WidgetsBinding.instance.addPostFrameCallback(
                (_) =>
                {
                    if (_dragGestureController?.navigator.mounted ?? false)
                    {
                        _dragGestureController?.navigator.didStopUserGesture();
                    }
                    _dragGestureController = null;
                }
            );
        }
        base.dispose();
    }

    internal virtual void _handleDragStart(Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        DartRuntimePrimitives.Assert(() => _dragGestureController is null);
        _dragGestureController = widget.onStartPopGesture();
    }

    internal virtual void _handleDragUpdate(Gestures.DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        DartRuntimePrimitives.Assert(() => _dragGestureController is not null);
        if (_stretchDragController is null)
        {
            return;
        }
        double delta =
            (sheetHeight > 0L)
                ? (DartRuntimePrimitives.RequireValue(details.primaryDelta) / sheetHeight)
                : 0.0;
        _dragGestureController!.dragUpdate(delta, _stretchDragController!.controller);
    }

    internal virtual void _handleDragEnd(Gestures.DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        DartRuntimePrimitives.Assert(() => _dragGestureController is not null);
        if (_stretchDragController is null)
        {
            _dragGestureController = null;
            return;
        }
        double velocityLocal =
            (sheetHeight > 0L) ? (details.velocity.pixelsPerSecond.dy / sheetHeight) : 0.0;
        _dragGestureController!.dragEnd(velocityLocal, _stretchDragController!.controller);
        _dragGestureController = null;
    }

    internal virtual void _handleDragCancel()
    {
        DartRuntimePrimitives.Assert(() => mounted);
        if (_stretchDragController is null)
        {
            _dragGestureController = null;
            return;
        }
        _dragGestureController?.dragEnd(0.0, _stretchDragController!.controller);
        _dragGestureController = null;
    }

    internal virtual void _handlePointerDown(Gestures.PointerDownEvent @event)
    {
        if (widget.enabledCallback())
        {
            _recognizer.addPointer(@event);
        }
    }

    public override Widget build(BuildContext context)
    {
        return new Listener(
            onPointerDown: _handlePointerDown,
            behavior: HitTestBehavior.translucent,
            child: widget.child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class _CupertinoDragGestureController__sheet<T>
{
    public virtual AnimationController popDragController { get; private set; } = default!;
    public virtual NavigatorState navigator { get; private set; } = default!;
    public virtual Func<bool> getIsActive { get; private set; } = default!;
    public virtual Func<bool> getIsCurrent { get; private set; } = default!;
    public virtual double topGap { get; private set; } = default!;

    internal _CupertinoDragGestureController__sheet(
        NavigatorState navigator,
        AnimationController popDragController,
        Func<bool> getIsActive,
        Func<bool> getIsCurrent,
        double topGap
    )
    {
        this.navigator = navigator;
        this.popDragController = popDragController;
        this.getIsActive = getIsActive;
        this.getIsCurrent = getIsCurrent;
        this.topGap = topGap;
        this.navigator.didStartUserGesture();
    }

    public virtual void dragUpdate(double delta, AnimationController? upController)
    {
        if (
            (upController is not null)
            && (popDragController.value == 1.0)
            && ((upController.value > 0L) || (delta < 0L))
        )
        {
            double stretchDistance =
                SheetLibrary._kTopGapRatio - SheetLibrary._kStretchedTopGapRatio;
            upController.value -= delta / stretchDistance;
        }
        else
        {
            popDragController.value -= delta;
        }
    }

    public virtual bool isDragged()
    {
        return popDragController.value != 1.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dragEnd(double velocity, AnimationController? upController)
    {
        if ((upController is not null) && (upController.value > 0L))
        {
            upController.animateBack(
                0.0,
                duration: Duration.Create(milliseconds: 180L),
                curve: Curves.easeOut
            );
            navigator.didStopUserGesture();
            return;
        }
        Curve animationCurve = Curves.easeOut;
        bool isCurrent = getIsCurrent();
        bool animateForward = default!;
        if (!isCurrent)
        {
            animateForward = getIsActive();
        }
        else
        {
            if (velocity.abs() >= SheetLibrary._kMinFlingVelocity)
            {
                animateForward = velocity <= 0L;
            }
            else
            {
                animateForward = popDragController.value > 0.52;
            }
        }
        if (animateForward)
        {
            popDragController.animateTo(
                1.0,
                duration: SheetLibrary._kDroppedSheetDragAnimationDuration,
                curve: animationCurve
            );
        }
        else
        {
            if (isCurrent)
            {
                navigator.pop<object>();
            }
            if (popDragController.isAnimating)
            {
                popDragController.animateBack(
                    0.0,
                    duration: SheetLibrary._kDroppedSheetDragAnimationDuration,
                    curve: animationCurve
                );
            }
        }
        if (popDragController.isAnimating)
        {
            void animationStatusCallback(AnimationStatus status)
            {
                navigator.didStopUserGesture();
                popDragController.removeStatusListener(animationStatusCallback);
            }
            popDragController.addStatusListener(animationStatusCallback);
        }
        else
        {
            navigator.didStopUserGesture();
        }
    }
}

internal class _CupertinoSheetScrollController__sheet : ScrollController
{
    public virtual Action onDragStart { get; private set; } = default!;
    public virtual Action<double> onDragUpdate { get; private set; } = default!;
    public virtual Action<double> onDragEnd { get; private set; } = default!;
    public virtual Func<bool> sheetIsDraggedDown { get; private set; } = default!;

    internal _CupertinoSheetScrollController__sheet(
        Action onDragStart,
        Action<double> onDragUpdate,
        Action<double> onDragEnd,
        Func<bool> sheetIsDraggedDown
    )
    {
        this.onDragStart = onDragStart;
        this.onDragUpdate = onDragUpdate;
        this.onDragEnd = onDragEnd;
        this.sheetIsDraggedDown = sheetIsDraggedDown;
    }

    public override _CupertinoSheetScrollPosition__sheet createScrollPosition(
        ScrollPhysics physics,
        ScrollContext context,
        ScrollPosition? oldPosition
    )
    {
        return new _CupertinoSheetScrollPosition__sheet(
            physics: physics.applyTo(new AlwaysScrollableScrollPhysics()),
            context: context,
            oldPosition: oldPosition,
            onDragStart: () => onDragStart(),
            onDragUpdate: onDragUpdate,
            onDragEnd: onDragEnd,
            sheetIsDraggedDown: sheetIsDraggedDown
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class _CupertinoSheetScrollPosition__sheet : ScrollPositionWithSingleContext
{
    internal virtual Action? _dragCancelCallback { get; set; } = default;
    internal virtual HashSet<AnimationController> _ballisticControllers { get; private set; } =
        new HashSet<AnimationController>();
    public virtual Action onDragStart { get; private set; } = default!;
    public virtual Action<double> onDragUpdate { get; private set; } = default!;
    public virtual Action<double> onDragEnd { get; private set; } = default!;
    public virtual Func<bool> sheetIsDraggedDown { get; private set; } = default!;

    internal _CupertinoSheetScrollPosition__sheet(
        ScrollPhysics physics,
        ScrollContext context,
        ScrollPosition? oldPosition = null,
        Action onDragStart = default!,
        Action<double> onDragUpdate = default!,
        Action<double> onDragEnd = default!,
        Func<bool> sheetIsDraggedDown = default!
    )
        : base(physics: physics, context: context, oldPosition: oldPosition)
    {
        this.onDragStart = onDragStart;
        this.onDragUpdate = onDragUpdate;
        this.onDragEnd = onDragEnd;
        this.sheetIsDraggedDown = sheetIsDraggedDown;
    }

    public virtual bool listShouldScroll => DartRuntimePrimitives.ConvertValue<bool>(pixels > 0.0);

    public override void absorb(ScrollPosition other)
    {
        base.absorb(other);
        DartRuntimePrimitives.Assert(() => _dragCancelCallback is null);
        if (other is not _CupertinoSheetScrollPosition__sheet)
        {
            return;
        }
        if (((_CupertinoSheetScrollPosition__sheet)other)._dragCancelCallback is not null)
        {
            _dragCancelCallback = ((_CupertinoSheetScrollPosition__sheet)other)._dragCancelCallback;
            ((_CupertinoSheetScrollPosition__sheet)other)._dragCancelCallback = null;
        }
    }

    public override void beginActivity(ScrollActivity? newActivity)
    {
        foreach (AnimationController ballisticController in _ballisticControllers)
        {
            ballisticController.stop();
        }
        base.beginActivity(newActivity);
    }

    public override void dispose()
    {
        foreach (AnimationController ballisticController in _ballisticControllers)
        {
            ballisticController.dispose();
        }
        _ballisticControllers.Clear();
        base.dispose();
    }

    public override void applyUserOffset(double delta)
    {
        onDragStart();
        if (!listShouldScroll && ((delta > 0L) || sheetIsDraggedDown()))
        {
            onDragUpdate(delta);
        }
        else
        {
            base.applyUserOffset(delta);
        }
    }

    public override void goBallistic(double velocity)
    {
        if (
            velocity == 0.0
            || ((velocity < 0.0) && listShouldScroll)
            || ((velocity > 0.0) && (pixels != maxScrollExtent))
        )
        {
            onDragEnd(0.0);
            base.goBallistic(velocity);
            return;
        }
        _dragCancelCallback?.Invoke();
        _dragCancelCallback = null;
        if ((velocity < 0.0) && !listShouldScroll)
        {
            onDragEnd(velocity);
            base.goBallistic(0);
            return;
        }
        onDragEnd(0.0);
        base.goBallistic(velocity);
    }

    public override Gestures.Drag drag(Gestures.DragStartDetails details, Action dragCancelCallback)
    {
        _dragCancelCallback = dragCancelCallback;
        return base.drag(details, () => dragCancelCallback());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _CupertinoDraggableScrollableSheet__sheet<T> : StatefulWidget
{
    public virtual Func<BuildContext, ScrollController, Widget> builder { get; private set; } =
        default!;
    public virtual Func<bool> enabledCallback { get; private set; } = default!;
    public virtual Func<_CupertinoDragGestureController__sheet<T>> onStartPopGesture
    {
        get;
        private set;
    } = default!;

    internal _CupertinoDraggableScrollableSheet__sheet(
        Key? key = null,
        Func<bool> enabledCallback = default!,
        Func<_CupertinoDragGestureController__sheet<T>> onStartPopGesture = default!,
        Func<BuildContext, ScrollController, Widget> builder = default!
    )
        : base(key: key)
    {
        this.enabledCallback = enabledCallback;
        this.onStartPopGesture = onStartPopGesture;
        this.builder = builder;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CupertinoDraggableScrollableSheetState__sheet<T>()
        );
}

internal class _CupertinoDraggableScrollableSheetState__sheet<T>
    : State<_CupertinoDraggableScrollableSheet__sheet<T>>
{
    internal virtual _CupertinoSheetScrollController__sheet _scrollController { get; set; } =
        default!;
    internal virtual _CupertinoDragGestureController__sheet<T>? _dragGestureController { get; set; } =
        default;

    public override void initState()
    {
        base.initState();
        _scrollController = new _CupertinoSheetScrollController__sheet(
            onDragStart: () => _dragStart(),
            onDragUpdate: _dragUpdate,
            onDragEnd: _handleDragEnd,
            sheetIsDraggedDown: () => _dragGestureController?.isDragged() ?? false
        );
    }

    public override void dispose()
    {
        if (_dragGestureController is not null)
        {
            WidgetsBinding.instance.addPostFrameCallback(
                (_) =>
                {
                    if (_dragGestureController?.navigator.mounted ?? false)
                    {
                        _dragGestureController?.navigator.didStopUserGesture();
                    }
                    _dragGestureController = null;
                }
            );
        }
        _scrollController.dispose();
        base.dispose();
    }

    internal virtual void _dragStart()
    {
        DartRuntimePrimitives.Assert(() => mounted);
        _dragGestureController ??= widget.onStartPopGesture();
    }

    internal virtual void _dragUpdate(double delta)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        if (_dragGestureController is not null)
        {
            _dragGestureController!.dragUpdate(
                delta
                    / (
                        DartRuntimePrimitives.RequireValue(context.size).height
                        - (
                            DartRuntimePrimitives.RequireValue(context.size).height
                            * SheetLibrary._kTopGapRatio
                        )
                    ),
                null
            );
        }
    }

    internal virtual void _handleDragEnd(double velocity)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        if (_dragGestureController is not null)
        {
            _dragGestureController!.dragEnd(
                -velocity / DartRuntimePrimitives.RequireValue(context.size).height,
                null
            );
            _dragGestureController = null;
        }
    }

    public override Widget build(BuildContext context)
    {
        return widget.builder(context, _scrollController);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
