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
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _kBottomUpTween = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.0, 1.0), end: Offset.zero);
}

public static partial class SheetLibrary
{
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _kBottomUpTweenWhenCoveringOtherSheet = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.0, 1.0), end: new global::Doroti.Ui.Offset(0.0, -0.02));
}

public static partial class SheetLibrary
{
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _kMidUpTween = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: new global::Doroti.Ui.Offset(0.0, -0.005));
}

public static partial class SheetLibrary
{
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _kTopDownTween = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: new global::Doroti.Ui.Offset(0.0, 0.07));
}

public static partial class SheetLibrary
{
    internal static global::Doroti.Framework.Animation.Animatable<double> _kOpacityTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 0.1);
}

public static partial class SheetLibrary
{
    internal static double _kMinFlingVelocity = 2.0;
}

public static partial class SheetLibrary
{
    internal static Duration _kDroppedSheetDragAnimationDuration = Duration.Create(milliseconds: 300L);
}

public static partial class SheetLibrary
{
    internal static double _kSheetScaleFactor = 0.0835;
}

public static partial class SheetLibrary
{
    internal static global::Doroti.Framework.Animation.Animatable<double> _kScaleTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 1.0 - _kSheetScaleFactor);
}

internal delegate void _DragStartCallback__sheet();

internal delegate void _DragUpdateCallback__sheet(double delta);

internal delegate void _DragEndCallback__sheet(double velocity);

internal delegate bool _GetSheetDragged__sheet();

public static partial class SheetLibrary
{
    public static Future<T?> showCupertinoSheet<T>(global::Doroti.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? pageBuilder = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? builder = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget>? scrollableBuilder = null, bool useNestedNavigation = false, bool enableDrag = true, global::Doroti.Framework.Widgets.RouteSettings? settings = null, double? topGap = null, bool showDragHandle = false)
    {
        DartRuntimePrimitives.Assert(() => (topGap is null) || (topGap >= 0.0) && (topGap <= 0.9), () => (object?)"topGap must be between 0.0 and 0.9");
        DartRuntimePrimitives.Assert(() => (pageBuilder is not null) || (builder is not null) || (scrollableBuilder is not null));
        DartRuntimePrimitives.Assert(() => (pageBuilder is null) && (builder is null) && (scrollableBuilder is not null) || (scrollableBuilder is null));
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? effectiveBuilder = builder ?? pageBuilder;
        var nestedNavigatorKey = GlobalKey<NavigatorState>.Create();
        if (!useNestedNavigation)
        {
            global::Doroti.Framework.Widgets.PageRoute<T> route = new CupertinoSheetRoute<T>(builder: effectiveBuilder, scrollableBuilder: scrollableBuilder, settings: settings, enableDrag: enableDrag, topGap: topGap);
            return Navigator.of(context, rootNavigator: true).push<T>(route);
        }
        else
        {
            global::Doroti.Framework.Widgets.Widget nestedNavigationContent(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder)
            {
                return new global::Doroti.Framework.Widgets.NavigatorPopHandler<T>(onPopWithResult: (result) =>
                {
                    DartRuntimePrimitives.Ignore(nestedNavigatorKey.currentState!.maybePop<object>());
                }, child: new global::Doroti.Framework.Widgets.Navigator(key: nestedNavigatorKey, initialRoute: "/", onGenerateInitialRoutes: (navigator, initialRouteName) =>
                {
                    return new List<object> { new CupertinoPageRoute<object?>(builder: (context) => {
return new global::Doroti.Framework.Widgets.PopScope<object>(canPop: false, onPopInvokedWithResult: (didPop, result) => {
if (didPop)
{
    return;
}
Navigator.of(context, rootNavigator: true).pop<object>(result);
}, child: builder(context));
throw new InvalidOperationException("Dart closure completed without a value.");
}) };
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }));
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            var routeLocal = new CupertinoSheetRoute<T>(scrollableBuilder: (context, controller) => nestedNavigationContent((scrollableBuilder is not null) ? ((context) => scrollableBuilder(context, controller)) : effectiveBuilder!), settings: settings, enableDrag: enableDrag, topGap: topGap);
            return Navigator.of(context, rootNavigator: true).push<T>(routeLocal);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoSheetTransition : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> primaryRouteAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> secondaryRouteAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual bool linearTransition { get; private set; } = default!;
    public virtual double topGap { get; private set; } = default!;

    public CupertinoSheetTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> primaryRouteAnimation = default!, global::Doroti.Framework.Animation.Animation<double> secondaryRouteAnimation = default!, global::Doroti.Framework.Widgets.Widget child = default!, bool linearTransition = default!, double? topGap = null) : base(key: key)
    {
        double __topGap = topGap ?? SheetLibrary._kTopGapRatio;
        this.primaryRouteAnimation = primaryRouteAnimation;
        this.secondaryRouteAnimation = secondaryRouteAnimation;
        this.child = child;
        this.linearTransition = linearTransition;
        this.topGap = __topGap;
    }

    public static global::Doroti.Framework.Widgets.Widget delegateTransition(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, bool allowSnapshotting, global::Doroti.Framework.Widgets.Widget? child)
    {
        if (CupertinoSheetRoute<object>.hasParentSheet(context))
        {
            return _delegatedCoverSheetSecondaryTransition(secondaryAnimation, child);
        }
        bool linearLocal = Navigator.of(context).userGestureInProgress;
        global::Doroti.Framework.Animation.Curve curveLocal = linearLocal ? Curves.linear : Curves.linearToEaseOut;
        global::Doroti.Framework.Animation.Curve reverseCurveLocal = linearLocal ? Curves.linear : Curves.easeInToLinear;
        var curvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(curve: curveLocal, reverseCurve: reverseCurveLocal, parent: secondaryAnimation);
        double deviceCornerRadius = (MediaQuery.maybeViewPaddingOf(context)?.top ?? 0) * SheetLibrary._kDeviceCornerRadiusSmoothingFactor;
        bool roundedDeviceCorners = deviceCornerRadius > SheetLibrary._kRoundedDeviceCornersThreshold;
        global::Doroti.Framework.Animation.Animatable<global::Doroti.Framework.Painting.BorderRadiusGeometry> decorationTween = new global::Doroti.Framework.Animation.Tween<global::Doroti.Framework.Painting.BorderRadiusGeometry>(begin: BorderRadius.CreateVertical(top: Radius.circular(roundedDeviceCorners ? deviceCornerRadius : 0)), end: BorderRadius.CreateAll(Radius.circular(12)));
        global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.BorderRadiusGeometry> radiusAnimation = curvedAnimation.drive(decorationTween);
        global::Doroti.Framework.Animation.Animation<double> opacityAnimation = curvedAnimation.drive(SheetLibrary._kOpacityTween);
        global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset> slideAnimation = curvedAnimation.drive(SheetLibrary._kTopDownTween);
        global::Doroti.Framework.Animation.Animation<double> scaleAnimation = curvedAnimation.drive(SheetLibrary._kScaleTween);
        curvedAnimation.dispose();
        var isDarkMode = Equals(CupertinoTheme.brightnessOf(context), Brightness.dark);
        var overlayColor = isDarkMode ? new global::Doroti.Ui.Color(4291348680L) : new global::Doroti.Ui.Color(4278190080L);
        global::Doroti.Framework.Widgets.Widget? contrastedChild = ((child is not null) && !secondaryAnimation.isDismissed) ? new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(child), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: opacityAnimation, child: new global::Doroti.Framework.Widgets.ColoredBox(color: overlayColor, child: SizedBox.CreateExpand()))) }) : child;
        double topGapHeight = MediaQuery.sizeOf(context).height * SheetLibrary._kTopGapRatio;
        return new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnnotatedRegion<global::Doroti.Framework.Services.SystemUiOverlayStyle>(value: new global::Doroti.Framework.Services.SystemUiOverlayStyle(statusBarBrightness: Brightness.dark, statusBarIconBrightness: Brightness.light), child: new global::Doroti.Framework.Widgets.SizedBox(height: topGapHeight, width: double.PositiveInfinity))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SlideTransition(position: slideAnimation, child: new global::Doroti.Framework.Widgets.ScaleTransition(scale: scaleAnimation, filterQuality: FilterQuality.medium, alignment: Alignment.topCenter, child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: radiusAnimation, child: child, builder: (context, child) => {
return new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: !secondaryAnimation.isDismissed ? radiusAnimation.value : BorderRadius.zero, child: contrastedChild);
throw new InvalidOperationException("Dart closure completed without a value.");
})))) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Widgets.Widget _delegatedCoverSheetSecondaryTransition(global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget? child)
    {
        global::Doroti.Framework.Animation.Curve curveLocal = Curves.linearToEaseOut;
        global::Doroti.Framework.Animation.Curve reverseCurveLocal = Curves.easeInToLinear;
        var curvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(curve: curveLocal, reverseCurve: reverseCurveLocal, parent: secondaryAnimation);
        global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset> slideAnimation = curvedAnimation.drive(SheetLibrary._kMidUpTween);
        global::Doroti.Framework.Animation.Animation<double> scaleAnimation = curvedAnimation.drive(SheetLibrary._kScaleTween);
        curvedAnimation.dispose();
        return new global::Doroti.Framework.Widgets.SlideTransition(position: slideAnimation, transformHitTests: false, child: new global::Doroti.Framework.Widgets.ScaleTransition(scale: scaleAnimation, filterQuality: FilterQuality.medium, alignment: Alignment.topCenter, child: new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: BorderRadius.CreateVertical(top: Radius.circular(12)), child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoSheetTransitionState__sheet());
}

internal class _CupertinoSheetTransitionState__sheet : global::Doroti.Framework.Widgets.State<CupertinoSheetTransition>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<CupertinoSheetTransition>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _stretchDragController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _stretchDragAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<Offset> _secondaryPositionAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _secondaryScaleAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _primaryPositionCurve { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _secondaryPositionCurve { get; set; } = default;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _stretchDragController = new global::Doroti.Framework.Animation.AnimationController(duration: Duration.Create(microseconds: 1L), vsync: this);
        _setupAnimation();
    }

    public override void didUpdateWidget(CupertinoSheetTransition oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!Equals(oldWidget.primaryRouteAnimation, widget.primaryRouteAnimation)) || (!Equals(oldWidget.secondaryRouteAnimation, widget.secondaryRouteAnimation)))
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
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _setupAnimation()
    {
        _primaryPositionCurve = new global::Doroti.Framework.Animation.CurvedAnimation(curve: Curves.fastEaseInToSlowEaseOut, reverseCurve: Curves.fastEaseInToSlowEaseOut.flipped, parent: widget.primaryRouteAnimation);
        _secondaryPositionCurve = new global::Doroti.Framework.Animation.CurvedAnimation(curve: Curves.linearToEaseOut, reverseCurve: Curves.easeInToLinear, parent: widget.secondaryRouteAnimation);
        double stretchDistance = SheetLibrary._kTopGapRatio - SheetLibrary._kStretchedTopGapRatio;
        double stretchedTopGap = widget.topGap - stretchDistance;
        _stretchDragAnimation = _stretchDragController.drive(new global::Doroti.Framework.Animation.Tween<double>(begin: widget.topGap, end: stretchedTopGap));
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

    internal virtual global::Doroti.Framework.Widgets.Widget _coverSheetPrimaryTransition(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, bool linearTransition, global::Doroti.Framework.Widgets.Widget? child)
    {
        global::Doroti.Framework.Animation.Animatable<global::Doroti.Ui.Offset> offsetTween = CupertinoSheetRoute<object>.hasParentSheet(context) ? SheetLibrary._kBottomUpTweenWhenCoveringOtherSheet : SheetLibrary._kBottomUpTween;
        var curvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: animation, curve: linearTransition ? Curves.linear : Curves.fastEaseInToSlowEaseOut, reverseCurve: linearTransition ? Curves.linear : Curves.fastEaseInToSlowEaseOut.flipped);
        global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset> positionAnimation = curvedAnimation.drive(offsetTween);
        curvedAnimation.dispose();
        return new global::Doroti.Framework.Widgets.SlideTransition(position: positionAnimation, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _coverSheetSecondaryTransition(global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget? child)
    {
        return new global::Doroti.Framework.Widgets.SlideTransition(position: _secondaryPositionAnimation, transformHitTests: false, child: new global::Doroti.Framework.Widgets.ScaleTransition(scale: _secondaryScaleAnimation, filterQuality: FilterQuality.medium, alignment: Alignment.topCenter, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _StretchDragControllerProvider__sheet(controller: _stretchDragController, child: SizedBox.CreateExpand(child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: _stretchDragAnimation, builder: (context, child) =>
        {
            return new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(top: MediaQuery.heightOf(context) * _stretchDragAnimation.value), child: _coverSheetSecondaryTransition(widget.secondaryRouteAnimation, _coverSheetPrimaryTransition(context, widget.primaryRouteAnimation, widget.linearTransition, widget.child)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
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
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

internal class _StretchDragControllerProvider__sheet : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual global::Doroti.Framework.Animation.AnimationController controller { get; private set; } = default!;

    internal _StretchDragControllerProvider__sheet(global::Doroti.Framework.Animation.AnimationController controller, global::Doroti.Framework.Widgets.Widget child) : base(child: child)
    {
        this.controller = controller;
    }

    public static _StretchDragControllerProvider__sheet? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return context.getInheritedWidgetOfExactType<_StretchDragControllerProvider__sheet>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_StretchDragControllerProvider__sheet)oldWidget;
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoSheetRoute<T> : global::Doroti.Framework.Widgets.PageRoute<T>, _CupertinoSheetRouteTransitionMixin__sheet<T>
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? builder { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget>? scrollableBuilder { get; private set; }
    public virtual bool enableDrag { get; private set; } = default!;
    internal virtual double? _topGap { get; private set; }
    public virtual bool showDragHandle { get; private set; } = default!;

    public CupertinoSheetRoute(global::Doroti.Framework.Widgets.RouteSettings? settings = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? builder = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget>? scrollableBuilder = null, bool enableDrag = true, bool showDragHandle = false, double? topGap = null) : base(settings: settings)
    {
        this.builder = builder;
        this.scrollableBuilder = scrollableBuilder;
        this.enableDrag = enableDrag;
        this.showDragHandle = showDragHandle;
        _topGap = topGap;
        System.Diagnostics.Debug.Assert((topGap is null) || (topGap >= 0.0) && (topGap <= 0.9));
        System.Diagnostics.Debug.Assert((builder is not null) || (scrollableBuilder is not null));
    }

    internal virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget> _effectiveBuilder
    {
        get
        {
            return scrollableBuilder ?? ((context, controller) => builder!(context));
        }
    }
    public virtual double topGap => DartRuntimePrimitives.ConvertValue<double>(_topGap ?? SheetLibrary._kTopGapRatio);
    public virtual bool _hasCustomTopGap => DartRuntimePrimitives.ConvertValue<bool>(_topGap is not null);
    internal virtual global::Doroti.Framework.Widgets.Widget _sheetWithDragHandle(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.ScrollController controller)
    {
        if (!showDragHandle)
        {
            return _effectiveBuilder(context, controller);
        }
        var dragHandleTopPadding = 5.0;
        var dragHandleHeight = 5.0;
        var dragHandleWidth = 36.0;
        var dragHandlePadding = 15.0;
        return new global::Doroti.Framework.Widgets.Stack(fit: StackFit.expand, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(padding: EdgeInsets.CreateOnly(top: dragHandlePadding)), child: _effectiveBuilder(context, controller))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: Alignment.topCenter, child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsGeometry.CreateOnly(top: dragHandleTopPadding), child: new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.ShapeDecoration(shape: new global::Doroti.Framework.Painting.RoundedSuperellipseBorder(borderRadius: BorderRadiusGeometry.CreateAll(Radius.circular(dragHandleWidth / 2L))), color: CupertinoColors.tertiaryLabel), child: new global::Doroti.Framework.Widgets.SizedBox(height: dragHandleHeight, width: dragHandleWidth))))) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Widgets.Widget buildContent(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return MediaQuery.CreateRemovePadding(context: context, removeTop: true, child: new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: BorderRadius.CreateVertical(top: Radius.circular(12)), child: new CupertinoUserInterfaceLevel(data: CupertinoUserInterfaceLevelData.elevated, child: new _CupertinoSheetScope__sheet(child: new _CupertinoDraggableScrollableSheet__sheet<T>(enabledCallback: () => enableDrag, onStartPopGesture: () => _CupertinoSheetRouteTransitionMixin__sheet<object>._startPopGesture<T>(this, DartRuntimePrimitives.RequireValue(topGap)), builder: _sheetWithDragHandle)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool hasParentSheet(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return _CupertinoSheetScope__sheet.maybeOf(context) is not null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void popSheet(global::Doroti.Framework.Widgets.BuildContext context)
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
    public override global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>? delegatedTransition
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
    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
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

    public override global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return _CupertinoSheetRouteTransitionMixin__sheet<object>.buildPageTransitions<T>(this, context, animation, secondaryAnimation, child, enableDrag, topGap);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoSheetScope__sheet : global::Doroti.Framework.Widgets.InheritedWidget
{
    internal _CupertinoSheetScope__sheet(global::Doroti.Framework.Widgets.Widget child) : base(child: child)
    {
    }

    public static _CupertinoSheetScope__sheet? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return context.getInheritedWidgetOfExactType<_CupertinoSheetScope__sheet>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => false;
}

public interface _CupertinoSheetRouteTransitionMixin__sheet<T>
{
    public global::Doroti.Framework.Widgets.Widget buildContent(global::Doroti.Framework.Widgets.BuildContext context);
    public Duration transitionDuration { get; }
    public global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>? delegatedTransition { get; }
    public bool enableDrag { get; }
    public double topGap { get; }
    public bool _hasCustomTopGap { get; }
    public global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation);
    public static _CupertinoDragGestureController__sheet<TRouteResult> _startPopGesture<TRouteResult>(global::Doroti.Framework.Widgets.ModalRoute<TRouteResult> route, double topGap)
    {
        return new _CupertinoDragGestureController__sheet<TRouteResult>(topGap: topGap, navigator: route.navigator!, getIsCurrent: () => route.isCurrent, getIsActive: () => route.isActive, popDragController: route.controller!);
    }
    public static global::Doroti.Framework.Widgets.Widget buildPageTransitions<TRouteResult>(global::Doroti.Framework.Widgets.ModalRoute<TRouteResult> route, global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child, bool enableDrag, double topGap)
    {
        bool linearTransitionLocal = route.popGestureInProgress;
        return new CupertinoSheetTransition(primaryRouteAnimation: animation, secondaryRouteAnimation: secondaryAnimation, linearTransition: linearTransitionLocal, topGap: topGap, child: new _CupertinoDragGestureDetector__sheet<TRouteResult>(enabledCallback: () => enableDrag, onStartPopGesture: () => _CupertinoSheetRouteTransitionMixin__sheet<TRouteResult>._startPopGesture<TRouteResult>(route, topGap), child: child));
    }
    public bool canTransitionFrom(dynamic previousRoute);
    public bool canTransitionTo(dynamic nextRoute);
    public global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child);
}

internal class _CupertinoDragGestureDetector__sheet<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::System.Func<bool> enabledCallback { get; private set; } = default!;
    public virtual global::System.Func<_CupertinoDragGestureController__sheet<T>> onStartPopGesture { get; private set; } = default!;

    internal _CupertinoDragGestureDetector__sheet(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<bool> enabledCallback = default!, global::System.Func<_CupertinoDragGestureController__sheet<T>> onStartPopGesture = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.enabledCallback = enabledCallback;
        this.onStartPopGesture = onStartPopGesture;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoDragGestureDetectorState__sheet<T>());
}

internal class _CupertinoDragGestureDetectorState__sheet<T> : global::Doroti.Framework.Widgets.State<_CupertinoDragGestureDetector__sheet<T>>
{
    internal virtual _CupertinoDragGestureController__sheet<T>? _dragGestureController { get; set; } = default;
    internal virtual global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer _recognizer { get; set; } = default!;
    internal virtual _StretchDragControllerProvider__sheet? _stretchDragController { get; set; } = default;

    internal static global::Doroti.Framework.Gestures.VelocityTracker _cupertinoVelocityBuilder(global::Doroti.Framework.Gestures.PointerEvent @event) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Gestures.VelocityTracker>(new global::Doroti.Framework.Gestures.IOSScrollViewFlingVelocityTracker(@event.kind));
    public virtual double sheetHeight => DartRuntimePrimitives.RequireValue(context.size).height;
    public override void initState()
    {
        base.initState();
        DartRuntimePrimitives.Assert(() => _stretchDragController is null);
        _stretchDragController = _StretchDragControllerProvider__sheet.maybeOf(context);
        _recognizer = ((Func<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>)(() =>
{
    var __cascade = new global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer(debugOwner: this);
    __cascade.velocityTrackerBuilder = _cupertinoVelocityBuilder;
    __cascade.onStart = _handleDragStart;
    __cascade.onUpdate = _handleDragUpdate;
    __cascade.onEnd = _handleDragEnd;
    __cascade.onCancel = _handleDragCancel;
    return __cascade;
}))();
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
            WidgetsBinding.instance.addPostFrameCallback((_) =>
            {
                if (_dragGestureController?.navigator.mounted ?? false)
                {
                    _dragGestureController?.navigator.didStopUserGesture();
                }
                _dragGestureController = null;
            });
        }
        base.dispose();
    }

    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        DartRuntimePrimitives.Assert(() => _dragGestureController is null);
        _dragGestureController = widget.onStartPopGesture();
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        DartRuntimePrimitives.Assert(() => _dragGestureController is not null);
        if (_stretchDragController is null)
        {
            return;
        }
        double delta = (sheetHeight > 0L) ? (DartRuntimePrimitives.RequireValue(details.primaryDelta) / sheetHeight) : 0.0;
        _dragGestureController!.dragUpdate(delta, _stretchDragController!.controller);
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        DartRuntimePrimitives.Assert(() => _dragGestureController is not null);
        if (_stretchDragController is null)
        {
            _dragGestureController = null;
            return;
        }
        double velocityLocal = (sheetHeight > 0L) ? (details.velocity.pixelsPerSecond.dy / sheetHeight) : 0.0;
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

    internal virtual void _handlePointerDown(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        if (widget.enabledCallback())
        {
            _recognizer.addPointer(@event);
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.Listener(onPointerDown: _handlePointerDown, behavior: HitTestBehavior.translucent, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _CupertinoDragGestureController__sheet<T>
{
    public virtual global::Doroti.Framework.Animation.AnimationController popDragController { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.NavigatorState navigator { get; private set; } = default!;
    public virtual global::System.Func<bool> getIsActive { get; private set; } = default!;
    public virtual global::System.Func<bool> getIsCurrent { get; private set; } = default!;
    public virtual double topGap { get; private set; } = default!;

    internal _CupertinoDragGestureController__sheet(global::Doroti.Framework.Widgets.NavigatorState navigator, global::Doroti.Framework.Animation.AnimationController popDragController, global::System.Func<bool> getIsActive, global::System.Func<bool> getIsCurrent, double topGap)
    {
        this.navigator = navigator;
        this.popDragController = popDragController;
        this.getIsActive = getIsActive;
        this.getIsCurrent = getIsCurrent;
        this.topGap = topGap;
        this.navigator.didStartUserGesture();
    }

    public virtual void dragUpdate(double delta, global::Doroti.Framework.Animation.AnimationController? upController)
    {
        if ((upController is not null) && (popDragController.value == 1.0) && ((upController.value > 0L) || (delta < 0L)))
        {
            double stretchDistance = SheetLibrary._kTopGapRatio - SheetLibrary._kStretchedTopGapRatio;
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

    public virtual void dragEnd(double velocity, global::Doroti.Framework.Animation.AnimationController? upController)
    {
        if ((upController is not null) && (upController.value > 0L))
        {
            upController.animateBack(0.0, duration: Duration.Create(milliseconds: 180L), curve: Curves.easeOut);
            navigator.didStopUserGesture();
            return;
        }
        global::Doroti.Framework.Animation.Curve animationCurve = Curves.easeOut;
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
            popDragController.animateTo(1.0, duration: SheetLibrary._kDroppedSheetDragAnimationDuration, curve: animationCurve);
        }
        else
        {
            if (isCurrent)
            {
                navigator.pop<object>();
            }
            if (popDragController.isAnimating)
            {
                popDragController.animateBack(0.0, duration: SheetLibrary._kDroppedSheetDragAnimationDuration, curve: animationCurve);
            }
        }
        if (popDragController.isAnimating)
        {
            void animationStatusCallback(global::Doroti.Framework.Animation.AnimationStatus status)
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

internal class _CupertinoSheetScrollController__sheet : global::Doroti.Framework.Widgets.ScrollController
{
    public virtual global::System.Action onDragStart { get; private set; } = default!;
    public virtual global::System.Action<double> onDragUpdate { get; private set; } = default!;
    public virtual global::System.Action<double> onDragEnd { get; private set; } = default!;
    public virtual global::System.Func<bool> sheetIsDraggedDown { get; private set; } = default!;

    internal _CupertinoSheetScrollController__sheet(global::System.Action onDragStart, global::System.Action<double> onDragUpdate, global::System.Action<double> onDragEnd, global::System.Func<bool> sheetIsDraggedDown)
    {
        this.onDragStart = onDragStart;
        this.onDragUpdate = onDragUpdate;
        this.onDragEnd = onDragEnd;
        this.sheetIsDraggedDown = sheetIsDraggedDown;
    }

    public override _CupertinoSheetScrollPosition__sheet createScrollPosition(global::Doroti.Framework.Widgets.ScrollPhysics physics, global::Doroti.Framework.Widgets.ScrollContext context, global::Doroti.Framework.Widgets.ScrollPosition? oldPosition)
    {
        return new _CupertinoSheetScrollPosition__sheet(physics: physics.applyTo(new global::Doroti.Framework.Widgets.AlwaysScrollableScrollPhysics()), context: context, oldPosition: oldPosition, onDragStart: () => onDragStart(), onDragUpdate: onDragUpdate, onDragEnd: onDragEnd, sheetIsDraggedDown: sheetIsDraggedDown);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _CupertinoSheetScrollPosition__sheet : global::Doroti.Framework.Widgets.ScrollPositionWithSingleContext
{
    internal virtual global::System.Action? _dragCancelCallback { get; set; } = default;
    internal virtual HashSet<global::Doroti.Framework.Animation.AnimationController> _ballisticControllers { get; private set; } = new HashSet<global::Doroti.Framework.Animation.AnimationController>();
    public virtual global::System.Action onDragStart { get; private set; } = default!;
    public virtual global::System.Action<double> onDragUpdate { get; private set; } = default!;
    public virtual global::System.Action<double> onDragEnd { get; private set; } = default!;
    public virtual global::System.Func<bool> sheetIsDraggedDown { get; private set; } = default!;

    internal _CupertinoSheetScrollPosition__sheet(global::Doroti.Framework.Widgets.ScrollPhysics physics, global::Doroti.Framework.Widgets.ScrollContext context, global::Doroti.Framework.Widgets.ScrollPosition? oldPosition = null, global::System.Action onDragStart = default!, global::System.Action<double> onDragUpdate = default!, global::System.Action<double> onDragEnd = default!, global::System.Func<bool> sheetIsDraggedDown = default!) : base(physics: physics, context: context, oldPosition: oldPosition)
    {
        this.onDragStart = onDragStart;
        this.onDragUpdate = onDragUpdate;
        this.onDragEnd = onDragEnd;
        this.sheetIsDraggedDown = sheetIsDraggedDown;
    }

    public virtual bool listShouldScroll => DartRuntimePrimitives.ConvertValue<bool>(pixels > 0.0);
    public override void absorb(global::Doroti.Framework.Widgets.ScrollPosition other)
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

    public override void beginActivity(global::Doroti.Framework.Widgets.ScrollActivity? newActivity)
    {
        foreach (global::Doroti.Framework.Animation.AnimationController ballisticController in _ballisticControllers)
        {
            ballisticController.stop();
        }
        base.beginActivity(newActivity);
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Animation.AnimationController ballisticController in _ballisticControllers)
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
        if (velocity == 0.0 || (velocity < 0.0) && listShouldScroll || (velocity > 0.0) && (pixels != maxScrollExtent))
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

    public override global::Doroti.Framework.Gestures.Drag drag(global::Doroti.Framework.Gestures.DragStartDetails details, global::System.Action dragCancelCallback)
    {
        _dragCancelCallback = dragCancelCallback;
        return base.drag(details, () => dragCancelCallback());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoDraggableScrollableSheet__sheet<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;
    public virtual global::System.Func<bool> enabledCallback { get; private set; } = default!;
    public virtual global::System.Func<_CupertinoDragGestureController__sheet<T>> onStartPopGesture { get; private set; } = default!;

    internal _CupertinoDraggableScrollableSheet__sheet(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<bool> enabledCallback = default!, global::System.Func<_CupertinoDragGestureController__sheet<T>> onStartPopGesture = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget> builder = default!) : base(key: key)
    {
        this.enabledCallback = enabledCallback;
        this.onStartPopGesture = onStartPopGesture;
        this.builder = builder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoDraggableScrollableSheetState__sheet<T>());
}

internal class _CupertinoDraggableScrollableSheetState__sheet<T> : global::Doroti.Framework.Widgets.State<_CupertinoDraggableScrollableSheet__sheet<T>>
{
    internal virtual _CupertinoSheetScrollController__sheet _scrollController { get; set; } = default!;
    internal virtual _CupertinoDragGestureController__sheet<T>? _dragGestureController { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _scrollController = new _CupertinoSheetScrollController__sheet(onDragStart: () => _dragStart(), onDragUpdate: _dragUpdate, onDragEnd: _handleDragEnd, sheetIsDraggedDown: () => _dragGestureController?.isDragged() ?? false);
    }

    public override void dispose()
    {
        if (_dragGestureController is not null)
        {
            WidgetsBinding.instance.addPostFrameCallback((_) =>
            {
                if (_dragGestureController?.navigator.mounted ?? false)
                {
                    _dragGestureController?.navigator.didStopUserGesture();
                }
                _dragGestureController = null;
            });
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
            _dragGestureController!.dragUpdate(delta / (DartRuntimePrimitives.RequireValue(context.size).height - DartRuntimePrimitives.RequireValue(context.size).height * SheetLibrary._kTopGapRatio), null);
        }
    }

    internal virtual void _handleDragEnd(double velocity)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        if (_dragGestureController is not null)
        {
            _dragGestureController!.dragEnd(-velocity / DartRuntimePrimitives.RequireValue(context.size).height, null);
            _dragGestureController = null;
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return widget.builder(context, _scrollController);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
