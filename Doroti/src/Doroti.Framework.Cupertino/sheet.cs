// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/sheet.dart
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
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _kBottomUpTween = ((global::Doroti.Framework.Animation.Animatable<Offset>)(object?)new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.0, 1.0), end: Offset.zero));
}

public static partial class SheetLibrary
{
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _kBottomUpTweenWhenCoveringOtherSheet = ((global::Doroti.Framework.Animation.Animatable<Offset>)(object?)new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.0, 1.0), end: new global::Doroti.Ui.Offset(0.0, -0.02)));
}

public static partial class SheetLibrary
{
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _kMidUpTween = ((global::Doroti.Framework.Animation.Animatable<Offset>)(object?)new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: new global::Doroti.Ui.Offset(0.0, -0.005)));
}

public static partial class SheetLibrary
{
    internal static global::Doroti.Framework.Animation.Animatable<Offset> _kTopDownTween = ((global::Doroti.Framework.Animation.Animatable<Offset>)(object?)new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: new global::Doroti.Ui.Offset(0.0, 0.07)));
}

public static partial class SheetLibrary
{
    internal static global::Doroti.Framework.Animation.Animatable<double> _kOpacityTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 0.1));
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
    internal static global::Doroti.Framework.Animation.Animatable<double> _kScaleTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: (1.0 - SheetLibrary._kSheetScaleFactor)));
}

internal delegate void _DragStartCallback__sheet();

internal delegate void _DragUpdateCallback__sheet(double delta);

internal delegate void _DragEndCallback__sheet(double velocity);

internal delegate bool _GetSheetDragged__sheet();

public static partial class SheetLibrary
{
    public static Future<T?> showCupertinoSheet<T>(global::Doroti.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? pageBuilder = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? builder = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget>? scrollableBuilder = null, bool useNestedNavigation = false, bool enableDrag = true, global::Doroti.Framework.Widgets.RouteSettings? settings = null, double? topGap = null, bool showDragHandle = false)
    {
        DartRuntimePrimitives.Assert(() => ((topGap is null) || (((topGap >= 0.0) && (topGap <= 0.9)))), () => (object?)"topGap must be between 0.0 and 0.9");
        DartRuntimePrimitives.Assert(() => (((pageBuilder is not null) || (builder is not null)) || (scrollableBuilder is not null)));
        DartRuntimePrimitives.Assert(() => (((((pageBuilder is null) && (builder is null)) && (scrollableBuilder is not null))) || (scrollableBuilder is null)));
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>? effectiveBuilder__9579 = ((builder ?? (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)pageBuilder));
        var nestedNavigatorKey__9630 = global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.NavigatorState>.Create();
        if (!useNestedNavigation)
        {
            global::Doroti.Framework.Widgets.PageRoute<T> route__9733 = ((global::Doroti.Framework.Widgets.PageRoute<T>)(object?)new CupertinoSheetRoute<T>(builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>?)effectiveBuilder__9579, scrollableBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget>?)scrollableBuilder, settings: settings, enableDrag: enableDrag, topGap: topGap));
            return ((Future<T?>)(object?)Navigator.of(context, rootNavigator: true).push<T>(route__9733));
        }
        else
        {
            global::Doroti.Framework.Widgets.Widget nestedNavigationContent(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder)
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NavigatorPopHandler<T>(onPopWithResult: ((global::System.Action<T?>)((result) =>
                {
                    DartRuntimePrimitives.Ignore(((global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.NavigatorState>)nestedNavigatorKey__9630).currentState!.maybePop<object>());
                })), child: new global::Doroti.Framework.Widgets.Navigator(key: nestedNavigatorKey__9630, initialRoute: "/", onGenerateInitialRoutes: ((global::System.Func<global::Doroti.Framework.Widgets.NavigatorState, string, List<dynamic>>)((navigator, initialRouteName) =>
                {
                    return ((List<object>)(object?)new List<global::Doroti.Framework.Widgets.Route<object?>> { new CupertinoPageRoute<object?>(builder: ((context) => {
return new global::Doroti.Framework.Widgets.PopScope<object>(canPop: false, onPopInvokedWithResult: ((global::System.Action<bool, object>)((didPop, result) => {
if (didPop)
{
    return;
}
Navigator.of(context, rootNavigator: true).pop<object>(result);
})), child: builder(context));
throw new InvalidOperationException("Dart closure completed without a value.");
})) });
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })))));
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            var route__11017 = new CupertinoSheetRoute<T>(scrollableBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget>?)((context, controller) => nestedNavigationContent(((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((scrollableBuilder is not null) ? ((context) => scrollableBuilder(context, controller)) : effectiveBuilder__9579!))))), settings: settings, enableDrag: enableDrag, topGap: topGap);
            return ((Future<T?>)(object?)Navigator.of(context, rootNavigator: true).push<T>(route__11017));
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
            return ((global::Doroti.Framework.Widgets.Widget)(object?)CupertinoSheetTransition._delegatedCoverSheetSecondaryTransition(secondaryAnimation, child));
        }
        bool linear__13799 = Navigator.of(context).userGestureInProgress;
        global::Doroti.Framework.Animation.Curve curve__13870 = (linear__13799 ? global::Doroti.Framework.Animation.Curves.linear : global::Doroti.Framework.Animation.Curves.linearToEaseOut);
        global::Doroti.Framework.Animation.Curve reverseCurve__13943 = (linear__13799 ? global::Doroti.Framework.Animation.Curves.linear : global::Doroti.Framework.Animation.Curves.easeInToLinear);
        var curvedAnimation__14016 = new global::Doroti.Framework.Animation.CurvedAnimation(curve: curve__13870, reverseCurve: reverseCurve__13943, parent: secondaryAnimation);
        double deviceCornerRadius__14164 = (((MediaQuery.maybeViewPaddingOf(context)?.top ?? 0)) * SheetLibrary._kDeviceCornerRadiusSmoothingFactor);
        bool roundedDeviceCorners__14298 = (deviceCornerRadius__14164 > SheetLibrary._kRoundedDeviceCornersThreshold);
        global::Doroti.Framework.Animation.Animatable<global::Doroti.Framework.Painting.BorderRadiusGeometry> decorationTween__14419 = ((global::Doroti.Framework.Animation.Animatable<global::Doroti.Framework.Painting.BorderRadiusGeometry>)(object?)new global::Doroti.Framework.Animation.Tween<global::Doroti.Framework.Painting.BorderRadiusGeometry>(begin: global::Doroti.Framework.Painting.BorderRadius.CreateVertical(top: global::Doroti.Ui.Radius.circular((roundedDeviceCorners__14298 ? deviceCornerRadius__14164 : 0))), end: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(12))));
        global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.BorderRadiusGeometry> radiusAnimation__14694 = ((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.BorderRadiusGeometry>)(object?)curvedAnimation__14016.drive(decorationTween__14419));
        global::Doroti.Framework.Animation.Animation<double> opacityAnimation__14780 = ((global::Doroti.Framework.Animation.Animation<double>)(object?)curvedAnimation__14016.drive(SheetLibrary._kOpacityTween));
        global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset> slideAnimation__14866 = ((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset>)(object?)curvedAnimation__14016.drive(SheetLibrary._kTopDownTween));
        global::Doroti.Framework.Animation.Animation<double> scaleAnimation__14950 = ((global::Doroti.Framework.Animation.Animation<double>)(object?)curvedAnimation__14016.drive(SheetLibrary._kScaleTween));
        curvedAnimation__14016.dispose();
        var isDarkMode__15046 = (object.Equals(CupertinoTheme.brightnessOf(context), Brightness.dark));
        var overlayColor__15126 = (isDarkMode__15046 ? new global::Doroti.Ui.Color(4291348680L) : new global::Doroti.Ui.Color(4278190080L));
        global::Doroti.Framework.Widgets.Widget? contrastedChild__15224 = (((child is not null) && !((global::Doroti.Framework.Animation.Animation<double>)secondaryAnimation).isDismissed) ? new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(child), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: opacityAnimation__14780, child: new global::Doroti.Framework.Widgets.ColoredBox(color: overlayColor__15126, child: global::Doroti.Framework.Widgets.SizedBox.CreateExpand()))) }) : child);
        double topGapHeight__15601 = (MediaQuery.sizeOf(context).height * SheetLibrary._kTopGapRatio);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnnotatedRegion<global::Doroti.Framework.Services.SystemUiOverlayStyle>(value: new global::Doroti.Framework.Services.SystemUiOverlayStyle(statusBarBrightness: Brightness.dark, statusBarIconBrightness: Brightness.light), child: new global::Doroti.Framework.Widgets.SizedBox(height: topGapHeight__15601, width: double.PositiveInfinity))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SlideTransition(position: slideAnimation__14866, child: new global::Doroti.Framework.Widgets.ScaleTransition(scale: scaleAnimation__14950, filterQuality: FilterQuality.medium, alignment: global::Doroti.Framework.Painting.Alignment.topCenter, child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: radiusAnimation__14694, child: child, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: (!((global::Doroti.Framework.Animation.Animation<double>)secondaryAnimation).isDismissed ? ((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.BorderRadiusGeometry>)radiusAnimation__14694).value : global::Doroti.Framework.Painting.BorderRadius.zero), child: contrastedChild__15224));
throw new InvalidOperationException("Dart closure completed without a value.");
})))))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Widgets.Widget _delegatedCoverSheetSecondaryTransition(global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget? child)
    {
        global::Doroti.Framework.Animation.Curve curve__16871 = ((global::Doroti.Framework.Animation.Curve)(object?)global::Doroti.Framework.Animation.Curves.linearToEaseOut);
        global::Doroti.Framework.Animation.Curve reverseCurve__16919 = ((global::Doroti.Framework.Animation.Curve)(object?)global::Doroti.Framework.Animation.Curves.easeInToLinear);
        var curvedAnimation__16967 = new global::Doroti.Framework.Animation.CurvedAnimation(curve: curve__16871, reverseCurve: reverseCurve__16919, parent: secondaryAnimation);
        global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset> slideAnimation__17126 = ((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset>)(object?)curvedAnimation__16967.drive(SheetLibrary._kMidUpTween));
        global::Doroti.Framework.Animation.Animation<double> scaleAnimation__17208 = ((global::Doroti.Framework.Animation.Animation<double>)(object?)curvedAnimation__16967.drive(SheetLibrary._kScaleTween));
        curvedAnimation__16967.dispose();
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SlideTransition(position: slideAnimation__17126, transformHitTests: false, child: new global::Doroti.Framework.Widgets.ScaleTransition(scale: scaleAnimation__17208, filterQuality: FilterQuality.medium, alignment: global::Doroti.Framework.Painting.Alignment.topCenter, child: new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateVertical(top: global::Doroti.Ui.Radius.circular(12)), child: child))));
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
        if (((!object.Equals(((CupertinoSheetTransition)oldWidget).primaryRouteAnimation, ((CupertinoSheetTransition)(object)this.widget).primaryRouteAnimation)) || (!object.Equals(((CupertinoSheetTransition)oldWidget).secondaryRouteAnimation, ((CupertinoSheetTransition)(object)this.widget).secondaryRouteAnimation))))
        {
            _disposeCurve();
            _setupAnimation();
        }
    }

    public override void dispose()
    {
        _disposeCurve();
        this._stretchDragController.dispose();
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

    internal virtual void _setupAnimation()
    {
        _primaryPositionCurve = new global::Doroti.Framework.Animation.CurvedAnimation(curve: global::Doroti.Framework.Animation.Curves.fastEaseInToSlowEaseOut, reverseCurve: global::Doroti.Framework.Animation.Curves.fastEaseInToSlowEaseOut.flipped, parent: ((CupertinoSheetTransition)(object)this.widget).primaryRouteAnimation);
        _secondaryPositionCurve = new global::Doroti.Framework.Animation.CurvedAnimation(curve: global::Doroti.Framework.Animation.Curves.linearToEaseOut, reverseCurve: global::Doroti.Framework.Animation.Curves.easeInToLinear, parent: ((CupertinoSheetTransition)(object)this.widget).secondaryRouteAnimation);
        double stretchDistance__19870 = (SheetLibrary._kTopGapRatio - SheetLibrary._kStretchedTopGapRatio);
        double stretchedTopGap__19945 = (((CupertinoSheetTransition)(object)this.widget).topGap - stretchDistance__19870);
        _stretchDragAnimation = this._stretchDragController.drive(new global::Doroti.Framework.Animation.Tween<double>(begin: ((CupertinoSheetTransition)(object)this.widget).topGap, end: stretchedTopGap__19945));
        _secondaryPositionAnimation = this._secondaryPositionCurve!.drive(SheetLibrary._kMidUpTween);
        _secondaryScaleAnimation = this._secondaryPositionCurve!.drive(SheetLibrary._kScaleTween);
    }

    internal virtual void _disposeCurve()
    {
        this._primaryPositionCurve?.dispose();
        this._secondaryPositionCurve?.dispose();
        _primaryPositionCurve = null;
        _secondaryPositionCurve = null;
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _coverSheetPrimaryTransition(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, bool linearTransition, global::Doroti.Framework.Widgets.Widget? child)
    {
        global::Doroti.Framework.Animation.Animatable<global::Doroti.Ui.Offset> offsetTween__20645 = ((global::Doroti.Framework.Animation.Animatable<global::Doroti.Ui.Offset>)(object?)(CupertinoSheetRoute<object>.hasParentSheet(context) ? SheetLibrary._kBottomUpTweenWhenCoveringOtherSheet : SheetLibrary._kBottomUpTween));
        var curvedAnimation__20789 = new global::Doroti.Framework.Animation.CurvedAnimation(parent: animation, curve: (linearTransition ? global::Doroti.Framework.Animation.Curves.linear : global::Doroti.Framework.Animation.Curves.fastEaseInToSlowEaseOut), reverseCurve: (linearTransition ? global::Doroti.Framework.Animation.Curves.linear : global::Doroti.Framework.Animation.Curves.fastEaseInToSlowEaseOut.flipped));
        global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset> positionAnimation__21060 = ((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset>)(object?)curvedAnimation__20789.drive(offsetTween__20645));
        curvedAnimation__20789.dispose();
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SlideTransition(position: positionAnimation__21060, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _coverSheetSecondaryTransition(global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget? child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SlideTransition(position: this._secondaryPositionAnimation, transformHitTests: false, child: new global::Doroti.Framework.Widgets.ScaleTransition(scale: this._secondaryScaleAnimation, filterQuality: FilterQuality.medium, alignment: global::Doroti.Framework.Painting.Alignment.topCenter, child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _StretchDragControllerProvider__sheet(controller: this._stretchDragController, child: global::Doroti.Framework.Widgets.SizedBox.CreateExpand(child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: this._stretchDragAnimation, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: (MediaQuery.heightOf(context) * ((global::Doroti.Framework.Animation.Animation<double>)this._stretchDragAnimation).value)), child: _coverSheetSecondaryTransition(((CupertinoSheetTransition)(object)this.widget).secondaryRouteAnimation, _coverSheetPrimaryTransition(context, ((CupertinoSheetTransition)(object)this.widget).primaryRouteAnimation, ((CupertinoSheetTransition)(object)this.widget).linearTransition, ((CupertinoSheetTransition)(object)this.widget).child))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
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

internal class _StretchDragControllerProvider__sheet : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual global::Doroti.Framework.Animation.AnimationController controller { get; private set; } = default!;

    internal _StretchDragControllerProvider__sheet(global::Doroti.Framework.Animation.AnimationController controller, global::Doroti.Framework.Widgets.Widget child) : base(child: child)
    {
        this.controller = controller;
    }

    public static _StretchDragControllerProvider__sheet? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((_StretchDragControllerProvider__sheet?)(object?)context.getInheritedWidgetOfExactType<_StretchDragControllerProvider__sheet>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_StretchDragControllerProvider__sheet)(object)oldWidget;
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
        this._topGap = topGap;
        System.Diagnostics.Debug.Assert(((topGap is null) || (((topGap >= 0.0) && (topGap <= 0.9)))));
        System.Diagnostics.Debug.Assert(((builder is not null) || (scrollableBuilder is not null)));
    }

    internal virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget> _effectiveBuilder
    {
        get
        {
            return ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget>)((this.scrollableBuilder ?? (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget>)((context, controller) => this.builder!(context)))));
            return default!;
        }
    }
    public virtual double topGap => DartRuntimePrimitives.ConvertValue<double>((this._topGap ?? SheetLibrary._kTopGapRatio));
    public virtual bool _hasCustomTopGap => DartRuntimePrimitives.ConvertValue<bool>((this._topGap is not null));
    internal virtual global::Doroti.Framework.Widgets.Widget _sheetWithDragHandle(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.ScrollController controller)
    {
        if (!this.showDragHandle)
        {
            return this._effectiveBuilder(context, controller);
        }
        var dragHandleTopPadding__27600 = 5.0;
        var dragHandleHeight__27638 = 5.0;
        var dragHandleWidth__27672 = 36.0;
        var dragHandlePadding__27706 = 15.0;
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(fit: global::Doroti.Framework.Rendering.StackFit.expand, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: dragHandlePadding__27706)), child: this._effectiveBuilder(context, controller))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.topCenter, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsGeometry.CreateOnly(top: dragHandleTopPadding__27600), child: new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.ShapeDecoration(shape: new global::Doroti.Framework.Painting.RoundedSuperellipseBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadiusGeometry.CreateAll(global::Doroti.Ui.Radius.circular((dragHandleWidth__27672 / 2L)))), color: CupertinoColors.tertiaryLabel), child: new global::Doroti.Framework.Widgets.SizedBox(height: dragHandleHeight__27638, width: dragHandleWidth__27672))))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Widgets.Widget buildContent(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.MediaQuery.CreateRemovePadding(context: context, removeTop: true, child: new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateVertical(top: global::Doroti.Ui.Radius.circular(12)), child: new CupertinoUserInterfaceLevel(data: CupertinoUserInterfaceLevelData.elevated, child: new _CupertinoSheetScope__sheet(child: new _CupertinoDraggableScrollableSheet__sheet<T>(enabledCallback: ((global::System.Func<bool>)(() => this.enableDrag)), onStartPopGesture: ((global::System.Func<_CupertinoDragGestureController__sheet<T>>)(() => _CupertinoSheetRouteTransitionMixin__sheet<object>._startPopGesture<T>(this, DartRuntimePrimitives.RequireValue(this.topGap)))), builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.ScrollController, global::Doroti.Framework.Widgets.Widget>)this._sheetWithDragHandle))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool hasParentSheet(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return (_CupertinoSheetScope__sheet.maybeOf(context) is not null);
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
            if (this._hasCustomTopGap)
            {
                return null;
            }
            return ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?>)CupertinoSheetTransition.delegateTransition);
            return default!;
        }
    }
    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)buildContent(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionFrom(dynamic previousRoute)
    {
        return !this._hasCustomTopGap;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool canTransitionTo(dynamic nextRoute)
    {
        if (((this is CupertinoSheetRoute<object>) && this._hasCustomTopGap))
        {
            return false;
        }
        return (nextRoute is _CupertinoSheetRouteTransitionMixin__sheet<object>);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)_CupertinoSheetRouteTransitionMixin__sheet<object>.buildPageTransitions<T>(this, context, animation, secondaryAnimation, child, this.enableDrag, this.topGap));
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
        return ((_CupertinoSheetScope__sheet?)(object?)context.getInheritedWidgetOfExactType<_CupertinoSheetScope__sheet>());
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
    public static _CupertinoDragGestureController__sheet<T> _startPopGesture<T>(global::Doroti.Framework.Widgets.ModalRoute<T> route, double topGap)
    {
        return new _CupertinoDragGestureController__sheet<T>(topGap: topGap, navigator: route.navigator!, getIsCurrent: ((global::System.Func<bool>)(() => route.isCurrent)), getIsActive: ((global::System.Func<bool>)(() => route.isActive)), popDragController: route.controller!);
    }
    public static global::Doroti.Framework.Widgets.Widget buildPageTransitions<T>(global::Doroti.Framework.Widgets.ModalRoute<T> route, global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child, bool enableDrag, double topGap)
    {
        bool linearTransition__32494 = ((global::Doroti.Framework.Widgets.ModalRoute<T>)route).popGestureInProgress;
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoSheetTransition(primaryRouteAnimation: animation, secondaryRouteAnimation: secondaryAnimation, linearTransition: linearTransition__32494, topGap: topGap, child: new _CupertinoDragGestureDetector__sheet<T>(enabledCallback: ((global::System.Func<bool>)(() => enableDrag)), onStartPopGesture: ((global::System.Func<_CupertinoDragGestureController__sheet<T>>)(() => _CupertinoSheetRouteTransitionMixin__sheet<T>._startPopGesture<T>(route, topGap))), child: child)));
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

    internal static global::Doroti.Framework.Gestures.VelocityTracker _cupertinoVelocityBuilder(global::Doroti.Framework.Gestures.PointerEvent @event) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Gestures.VelocityTracker>(new global::Doroti.Framework.Gestures.IOSScrollViewFlingVelocityTracker(((global::Doroti.Framework.Gestures.PointerEvent)@event).kind));
    public virtual double sheetHeight => DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.BuildContext)this.context).size).height;
    public override void initState()
    {
        base.initState();
        DartRuntimePrimitives.Assert(() => (this._stretchDragController is null));
        _stretchDragController = _StretchDragControllerProvider__sheet.maybeOf(this.context);
        _recognizer = ((Func<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>)(() =>
{
    var __cascade = new global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer(debugOwner: this);
    __cascade.velocityTrackerBuilder = _cupertinoVelocityBuilder;
    __cascade.onStart = this._handleDragStart;
    __cascade.onUpdate = this._handleDragUpdate;
    __cascade.onEnd = this._handleDragEnd;
    __cascade.onCancel = this._handleDragCancel;
    return __cascade;
}))();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _stretchDragController = _StretchDragControllerProvider__sheet.maybeOf(this.context);
    }

    public override void dispose()
    {
        this._recognizer.dispose();
        if ((this._dragGestureController is not null))
        {
            global::Doroti.Framework.Widgets.WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                if ((this._dragGestureController?.navigator.mounted ?? false))
                {
                    this._dragGestureController?.navigator.didStopUserGesture();
                }
                _dragGestureController = null;
            })));
        }
        base.dispose();
    }

    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        DartRuntimePrimitives.Assert(() => (this._dragGestureController is null));
        _dragGestureController = this.widget.onStartPopGesture();
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        DartRuntimePrimitives.Assert(() => (this._dragGestureController is not null));
        if ((this._stretchDragController is null))
        {
            return;
        }
        double delta__35982 = ((this.sheetHeight > 0L) ? (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).primaryDelta) / this.sheetHeight) : 0.0);
        this._dragGestureController!.dragUpdate(delta__35982, this._stretchDragController!.controller);
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        DartRuntimePrimitives.Assert(() => (this._dragGestureController is not null));
        if ((this._stretchDragController is null))
        {
            _dragGestureController = null;
            return;
        }
        double velocity__36425 = ((this.sheetHeight > 0L) ? (((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dy / this.sheetHeight) : 0.0);
        this._dragGestureController!.dragEnd(velocity__36425, this._stretchDragController!.controller);
        _dragGestureController = null;
    }

    internal virtual void _handleDragCancel()
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        if ((this._stretchDragController is null))
        {
            _dragGestureController = null;
            return;
        }
        this._dragGestureController?.dragEnd(0.0, this._stretchDragController!.controller);
        _dragGestureController = null;
    }

    internal virtual void _handlePointerDown(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        if (this.widget.enabledCallback())
        {
            this._recognizer.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)@event);
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Listener(onPointerDown: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)this._handlePointerDown, behavior: global::Doroti.Framework.Rendering.HitTestBehavior.translucent, child: ((_CupertinoDragGestureDetector__sheet<T>)(object)this.widget).child));
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
        if ((((upController is not null) && (((global::Doroti.Framework.Animation.AnimationController)this.popDragController).value == 1.0)) && (((((global::Doroti.Framework.Animation.AnimationController)upController).value > 0L) || (delta < 0L)))))
        {
            double stretchDistance__38359 = (SheetLibrary._kTopGapRatio - SheetLibrary._kStretchedTopGapRatio);
            upController.value -= (delta / stretchDistance__38359);
        }
        else
        {
            this.popDragController.value -= delta;
        }
    }

    public virtual bool isDragged()
    {
        return (((global::Doroti.Framework.Animation.AnimationController)this.popDragController).value != 1.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dragEnd(double velocity, global::Doroti.Framework.Animation.AnimationController? upController)
    {
        if (((upController is not null) && (((global::Doroti.Framework.Animation.AnimationController)upController).value > 0L)))
        {
            upController.animateBack(0.0, duration: Duration.Create(milliseconds: 180L), curve: global::Doroti.Framework.Animation.Curves.easeOut);
            this.navigator.didStopUserGesture();
            return;
        }
        global::Doroti.Framework.Animation.Curve animationCurve__39383 = ((global::Doroti.Framework.Animation.Curve)(object?)global::Doroti.Framework.Animation.Curves.easeOut);
        bool isCurrent__39431 = this.getIsCurrent();
        bool animateForward__39474 = default!;
        if (!isCurrent__39431)
        {
            animateForward__39474 = this.getIsActive();
        }
        else
        {
            if ((velocity.abs() >= SheetLibrary._kMinFlingVelocity))
            {
                animateForward__39474 = (velocity <= 0L);
            }
            else
            {
                animateForward__39474 = (((global::Doroti.Framework.Animation.AnimationController)this.popDragController).value > 0.52);
            }
        }
        if (animateForward__39474)
        {
            this.popDragController.animateTo(1.0, duration: SheetLibrary._kDroppedSheetDragAnimationDuration, curve: animationCurve__39383);
        }
        else
        {
            if (isCurrent__39431)
            {
                this.navigator.pop<object>();
            }
            if (((global::Doroti.Framework.Animation.AnimationController)this.popDragController).isAnimating)
            {
                this.popDragController.animateBack(0.0, duration: SheetLibrary._kDroppedSheetDragAnimationDuration, curve: animationCurve__39383);
            }
        }
        if (((global::Doroti.Framework.Animation.AnimationController)this.popDragController).isAnimating)
        {
            void animationStatusCallback(global::Doroti.Framework.Animation.AnimationStatus status)
            {
                this.navigator.didStopUserGesture();
                this.popDragController.removeStatusListener((AnimationStatusListener)animationStatusCallback);
            }
            this.popDragController.addStatusListener((AnimationStatusListener)animationStatusCallback);
        }
        else
        {
            this.navigator.didStopUserGesture();
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
        return new _CupertinoSheetScrollPosition__sheet(physics: physics.applyTo(new global::Doroti.Framework.Widgets.AlwaysScrollableScrollPhysics()), context: context, oldPosition: oldPosition, onDragStart: () => this.onDragStart(), onDragUpdate: (global::System.Action<double>)this.onDragUpdate, onDragEnd: (global::System.Action<double>)this.onDragEnd, sheetIsDraggedDown: (global::System.Func<bool>)this.sheetIsDraggedDown);
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

    public virtual bool listShouldScroll => DartRuntimePrimitives.ConvertValue<bool>((this.pixels > 0.0));
    public override void absorb(global::Doroti.Framework.Widgets.ScrollPosition other)
    {
        base.absorb(other);
        DartRuntimePrimitives.Assert(() => (this._dragCancelCallback is null));
        if ((other is not _CupertinoSheetScrollPosition__sheet))
        {
            return;
        }
        if ((((_CupertinoSheetScrollPosition__sheet)((_CupertinoSheetScrollPosition__sheet)other))._dragCancelCallback is not null))
        {
            _dragCancelCallback = (global::System.Action)((_CupertinoSheetScrollPosition__sheet)((_CupertinoSheetScrollPosition__sheet)other))._dragCancelCallback;
            ((dynamic)other)._dragCancelCallback = null;
        }
    }

    public override void beginActivity(global::Doroti.Framework.Widgets.ScrollActivity? newActivity)
    {
        foreach (global::Doroti.Framework.Animation.AnimationController ballisticController__44546 in this._ballisticControllers)
        {
            ballisticController__44546.stop();
        }
        base.beginActivity(newActivity);
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Animation.AnimationController ballisticController__44743 in this._ballisticControllers)
        {
            ballisticController__44743.dispose();
        }
        this._ballisticControllers.Clear();
        base.dispose();
    }

    public override void applyUserOffset(double delta)
    {
        this.onDragStart();
        if ((!this.listShouldScroll && (((delta > 0L) || this.sheetIsDraggedDown()))))
        {
            this.onDragUpdate(delta);
        }
        else
        {
            base.applyUserOffset(delta);
        }
    }

    public override void goBallistic(double velocity)
    {
        if (((((velocity == 0.0)) || (((velocity < 0.0) && this.listShouldScroll))) || (((velocity > 0.0) && (this.pixels != this.maxScrollExtent)))))
        {
            this.onDragEnd(0.0);
            base.goBallistic(velocity);
            return;
        }
        this._dragCancelCallback?.Invoke();
        _dragCancelCallback = null;
        if (((velocity < 0.0) && !this.listShouldScroll))
        {
            this.onDragEnd(velocity);
            base.goBallistic(0);
            return;
        }
        this.onDragEnd(0.0);
        base.goBallistic(velocity);
    }

    public override global::Doroti.Framework.Gestures.Drag drag(global::Doroti.Framework.Gestures.DragStartDetails details, global::System.Action dragCancelCallback)
    {
        _dragCancelCallback = (global::System.Action)dragCancelCallback;
        return ((global::Doroti.Framework.Gestures.Drag)(object?)base.drag(details, () => dragCancelCallback()));
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
        _scrollController = new _CupertinoSheetScrollController__sheet(onDragStart: () => this._dragStart(), onDragUpdate: (global::System.Action<double>)this._dragUpdate, onDragEnd: (global::System.Action<double>)this._handleDragEnd, sheetIsDraggedDown: ((global::System.Func<bool>)(() => (this._dragGestureController?.isDragged() ?? false))));
    }

    public override void dispose()
    {
        if ((this._dragGestureController is not null))
        {
            global::Doroti.Framework.Widgets.WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
            {
                if ((this._dragGestureController?.navigator.mounted ?? false))
                {
                    this._dragGestureController?.navigator.didStopUserGesture();
                }
                _dragGestureController = null;
            })));
        }
        this._scrollController.dispose();
        base.dispose();
    }

    internal virtual void _dragStart()
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        _dragGestureController ??= this.widget.onStartPopGesture();
    }

    internal virtual void _dragUpdate(double delta)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        if ((this._dragGestureController is not null))
        {
            this._dragGestureController!.dragUpdate((delta / ((DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.BuildContext)this.context).size).height - ((DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.BuildContext)this.context).size).height * SheetLibrary._kTopGapRatio))))), null);
        }
    }

    internal virtual void _handleDragEnd(double velocity)
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        if ((this._dragGestureController is not null))
        {
            this._dragGestureController!.dragEnd((-velocity / DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.BuildContext)this.context).size).height), null);
            _dragGestureController = null;
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return this.widget.builder(context, this._scrollController);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
