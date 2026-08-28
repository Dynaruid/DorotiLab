// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/scaffold.dart
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

public static partial class ScaffoldLibrary
{
    internal static FloatingActionButtonLocation _kDefaultFloatingActionButtonLocation = FloatingActionButtonLocation.endFloat;
}

public static partial class ScaffoldLibrary
{
    internal static FloatingActionButtonAnimator _kDefaultFloatingActionButtonAnimator = FloatingActionButtonAnimator.scaling;
}

public static partial class ScaffoldLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _standardBottomSheetCurve = CurvesLibrary.standardEasing;
}

public static partial class ScaffoldLibrary
{
    internal static double _kBottomSheetDominatesPercentage = 0.3;
}

public static partial class ScaffoldLibrary
{
    internal static double _kMinBottomSheetScrimOpacity = 0.1;
}

public static partial class ScaffoldLibrary
{
    internal static double _kMaxBottomSheetScrimOpacity = 0.6;
}

public enum _ScaffoldSlot__scaffold
{
    body,
    appBar,
    bodyScrim,
    bottomSheet,
    snackBar,
    materialBanner,
    persistentFooter,
    bottomNavigationBar,
    floatingActionButton,
    drawer,
    endDrawer,
    statusBar
}

public class ScaffoldMessenger : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    public ScaffoldMessenger(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

    public static ScaffoldMessengerState of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasScaffoldMessenger(context));
        _ScaffoldMessengerScope__scaffold scope = context.dependOnInheritedWidgetOfExactType<_ScaffoldMessengerScope__scaffold>()!;
        return ((_ScaffoldMessengerScope__scaffold)scope)._scaffoldMessengerState;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ScaffoldMessengerState? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _ScaffoldMessengerScope__scaffold? scope = ((_ScaffoldMessengerScope__scaffold?)(object?)context.dependOnInheritedWidgetOfExactType<_ScaffoldMessengerScope__scaffold>());
        return scope?._scaffoldMessengerState;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new ScaffoldMessengerState());
}

public class ScaffoldMessengerState : global::Doroti.Framework.Widgets.State<ScaffoldMessenger>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<ScaffoldMessenger>
{
    internal virtual HashSet<ScaffoldState> _scaffolds { get; private set; } = new HashSet<ScaffoldState>();
    internal virtual Queue<ScaffoldFeatureController<MaterialBanner, MaterialBannerClosedReason>> _materialBanners { get; private set; } = new Queue<ScaffoldFeatureController<MaterialBanner, MaterialBannerClosedReason>>();
    internal virtual global::Doroti.Framework.Animation.AnimationController? _materialBannerController { get; set; } = default;
    internal virtual Queue<ScaffoldFeatureController<SnackBar, SnackBarClosedReason>> _snackBars { get; private set; } = new Queue<ScaffoldFeatureController<SnackBar, SnackBarClosedReason>>();
    internal virtual global::Doroti.Framework.Animation.AnimationController? _snackBarController { get; set; } = default;
    internal virtual Timer? _snackBarTimer { get; set; } = default;
    internal virtual bool _accessibleNavigation { get; set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void didChangeDependencies()
    {
        _accessibleNavigation = MediaQuery.accessibleNavigationOf(this.context);
        base.didChangeDependencies();
    }

    internal virtual void _register(ScaffoldState scaffold)
    {
        this._scaffolds.add(scaffold);
        if (_isRoot(scaffold))
        {
            if (System.Linq.Enumerable.Any(this._snackBars))
            {
                scaffold._updateSnackBar();
            }
            if (System.Linq.Enumerable.Any(this._materialBanners))
            {
                scaffold._updateMaterialBanner();
            }
        }
    }

    internal virtual void _unregister(ScaffoldState scaffold)
    {
        bool removed = this._scaffolds.remove(scaffold);
        DartRuntimePrimitives.Assert(() => removed);
    }

    internal virtual void _updateScaffolds()
    {
        foreach (ScaffoldState scaffold in this._scaffolds)
        {
            if (_isRoot(scaffold))
            {
                scaffold._updateSnackBar();
                scaffold._updateMaterialBanner();
            }
        }
    }

    internal virtual bool _isRoot(ScaffoldState scaffold)
    {
        ScaffoldState? parent = ((ScaffoldState?)(object?)scaffold.context.findAncestorStateOfType<ScaffoldState>());
        return ((parent is null) || !this._scaffolds.contains(parent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ScaffoldFeatureController<SnackBar, SnackBarClosedReason> showSnackBar(SnackBar snackBar, global::Doroti.Framework.Animation.AnimationStyle? snackBarAnimationStyle = null)
    {
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this._scaffolds), () => (object?)"ScaffoldMessenger.showSnackBar was called, but there are currently no " + "descendant Scaffolds to present to.");
        _didUpdateAnimationStyle(snackBarAnimationStyle);
        _snackBarController ??= ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = SnackBar.createAnimationController(duration: snackBarAnimationStyle?.duration, reverseDuration: snackBarAnimationStyle?.reverseDuration, vsync: this);
    __cascade.addStatusListener((AnimationStatusListener)this._handleSnackBarStatusChanged);
    return __cascade;
}))();
        if (!System.Linq.Enumerable.Any(this._snackBars))
        {
            DartRuntimePrimitives.Assert(() => this._snackBarController!.isDismissed);
            this._snackBarController!.forward();
        }
        ScaffoldFeatureController<SnackBar, SnackBarClosedReason> controller = default!;
        controller = new ScaffoldFeatureController<SnackBar, SnackBarClosedReason>(snackBar.withAnimation(this._snackBarController!, fallbackKey: new global::Doroti.Framework.Foundation.UniqueKey()), new Completer<SnackBarClosedReason>(), ((global::System.Action)(() =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._snackBars.Peek(), controller)));
            hideCurrentSnackBar();
        })), null);
        try
        {
            setState(((global::System.Action)(() =>
            {
                this._snackBars.addLast(controller);
            })));
            _updateScaffolds();
        }
        catch (Exception exception)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((exception is global::Doroti.Framework.Foundation.FlutterError))
                    {
                        global::Doroti.Framework.Foundation.FlutterError exception__13918__as13961 = (global::Doroti.Framework.Foundation.FlutterError)exception;
                        string summary = ((string)(object?)((global::Doroti.Framework.Foundation.FlutterError)((global::Doroti.Framework.Foundation.FlutterError)exception__13918__as13961)).diagnostics.toDescription());
                        if ((summary == "setState() or markNeedsBuild() called during build."))
                        {
                            var information = new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("The showSnackBar() method cannot be called during build."), new global::Doroti.Framework.Foundation.ErrorDescription("The showSnackBar() method was called during build, which is " + "prohibited as showing snack bars requires updating state. Updating " + "state is not possible during build."), new global::Doroti.Framework.Foundation.ErrorHint("Instead of calling showSnackBar() during build, call it directly " + "in your on tap (and related) callbacks. If you need to immediately " + "show a snack bar, make the call in initState() or " + "didChangeDependencies() instead. Otherwise, you can also schedule a " + "post-frame callback using SchedulerBinding.addPostFrameCallback to " + "show the snack bar after the current frame."), this.context.describeOwnershipChain("The ownership chain for the particular ScaffoldMessenger is") };
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(information));
                        }
                    }
                    return true;
                });
            throw;
        }
        return controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _didUpdateAnimationStyle(global::Doroti.Framework.Animation.AnimationStyle? snackBarAnimationStyle)
    {
        if ((snackBarAnimationStyle is not null))
        {
            if (((!object.Equals(this._snackBarController?.duration, ((global::Doroti.Framework.Animation.AnimationStyle)snackBarAnimationStyle).duration)) || (!object.Equals(this._snackBarController?.reverseDuration, ((global::Doroti.Framework.Animation.AnimationStyle)snackBarAnimationStyle).reverseDuration))))
            {
                this._snackBarController?.dispose();
                _snackBarController = null;
            }
        }
    }

    internal virtual void _handleSnackBarStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        switch (status)
        {
            case global::Doroti.Framework.Animation.AnimationStatus.dismissed:
                {
                    DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this._snackBars));
                    setState(((global::System.Action)(() =>
                    {
                        this._snackBars.Dequeue();
                    })));
                    _updateScaffolds();
                    if (System.Linq.Enumerable.Any(this._snackBars))
                    {
                        this._snackBarController!.forward();
                    }
                    break;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.completed:
                {
                    setState(((global::System.Action)(() =>
                    {
                        DartRuntimePrimitives.Assert(() => (this._snackBarTimer is null));
                    })));
                    _updateScaffolds();
                    break;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.forward:
            case global::Doroti.Framework.Animation.AnimationStatus.reverse:
                {
                    break;
                }
        }
    }

    public virtual void removeCurrentSnackBar(SnackBarClosedReason reason = SnackBarClosedReason.remove)
    {
        if (!System.Linq.Enumerable.Any(this._snackBars))
        {
            return;
        }
        Completer<SnackBarClosedReason> completer = this._snackBars.Peek()._completer;
        if (!completer.isCompleted)
        {
            completer.complete(reason);
        }
        this._snackBarTimer?.cancel();
        _snackBarTimer = null;
        this._snackBarController!.value = 0.0;
    }

    public virtual void hideCurrentSnackBar(SnackBarClosedReason reason = SnackBarClosedReason.hide)
    {
        if ((!System.Linq.Enumerable.Any(this._snackBars) || this._snackBarController!.isDismissed))
        {
            return;
        }
        Completer<SnackBarClosedReason> completer = this._snackBars.Peek()._completer;
        if (this._accessibleNavigation)
        {
            this._snackBarController!.value = 0.0;
            completer.complete(reason);
        }
        else
        {
            DartRuntimePrimitives.Ignore(this._snackBarController!.reverse().then(((global::System.Func<object?, object>)((value) =>
            {
                DartRuntimePrimitives.Assert(() => this.mounted);
                if (!completer.isCompleted)
                {
                    completer.complete(reason);
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            }))));
        }
        this._snackBarTimer?.cancel();
        _snackBarTimer = null;
    }

    public virtual void clearSnackBars()
    {
        if ((!System.Linq.Enumerable.Any(this._snackBars) || this._snackBarController!.isDismissed))
        {
            return;
        }
        ScaffoldFeatureController<SnackBar, SnackBarClosedReason> currentSnackbar = this._snackBars.Peek();
        this._snackBars.Clear();
        this._snackBars.Enqueue(currentSnackbar);
        hideCurrentSnackBar();
    }

    public virtual ScaffoldFeatureController<MaterialBanner, MaterialBannerClosedReason> showMaterialBanner(MaterialBanner materialBanner)
    {
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this._scaffolds), () => (object?)"ScaffoldMessenger.showMaterialBanner was called, but there are currently no " + "descendant Scaffolds to present to.");
        _materialBannerController ??= ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = MaterialBanner.createAnimationController(vsync: this);
    __cascade.addStatusListener((AnimationStatusListener)this._handleMaterialBannerStatusChanged);
    return __cascade;
}))();
        if (!System.Linq.Enumerable.Any(this._materialBanners))
        {
            DartRuntimePrimitives.Assert(() => this._materialBannerController!.isDismissed);
            this._materialBannerController!.forward();
        }
        ScaffoldFeatureController<MaterialBanner, MaterialBannerClosedReason> controller = default!;
        controller = new ScaffoldFeatureController<MaterialBanner, MaterialBannerClosedReason>(materialBanner.withAnimation(this._materialBannerController!, fallbackKey: new global::Doroti.Framework.Foundation.UniqueKey()), new Completer<MaterialBannerClosedReason>(), ((global::System.Action)(() =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._materialBanners.Peek(), controller)));
            hideCurrentMaterialBanner();
        })), null);
        setState(((global::System.Action)(() =>
        {
            this._materialBanners.addLast(controller);
        })));
        _updateScaffolds();
        return controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleMaterialBannerStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        switch (status)
        {
            case global::Doroti.Framework.Animation.AnimationStatus.dismissed:
                {
                    DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this._materialBanners));
                    setState(((global::System.Action)(() =>
                    {
                        this._materialBanners.Dequeue();
                    })));
                    _updateScaffolds();
                    if (System.Linq.Enumerable.Any(this._materialBanners))
                    {
                        this._materialBannerController!.forward();
                    }
                    break;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.completed:
                {
                    _updateScaffolds();
                    break;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.forward:
            case global::Doroti.Framework.Animation.AnimationStatus.reverse:
                {
                    break;
                }
        }
    }

    public virtual void removeCurrentMaterialBanner(MaterialBannerClosedReason reason = default!)
    {
        if (!System.Linq.Enumerable.Any(this._materialBanners))
        {
            return;
        }
        Completer<MaterialBannerClosedReason> completer = this._materialBanners.Peek()._completer;
        if (!completer.isCompleted)
        {
            completer.complete(reason);
        }
        this._materialBannerController!.value = 0.0;
    }

    public virtual void hideCurrentMaterialBanner(MaterialBannerClosedReason reason = default!)
    {
        if ((!System.Linq.Enumerable.Any(this._materialBanners) || this._materialBannerController!.isDismissed))
        {
            return;
        }
        Completer<MaterialBannerClosedReason> completer = this._materialBanners.Peek()._completer;
        if (this._accessibleNavigation)
        {
            this._materialBannerController!.value = 0.0;
            completer.complete(reason);
        }
        else
        {
            DartRuntimePrimitives.Ignore(this._materialBannerController!.reverse().then(((global::System.Func<object?, object>)((value) =>
            {
                DartRuntimePrimitives.Assert(() => this.mounted);
                if (!completer.isCompleted)
                {
                    completer.complete(reason);
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            }))));
        }
    }

    public virtual void clearMaterialBanners()
    {
        if ((!System.Linq.Enumerable.Any(this._materialBanners) || this._materialBannerController!.isDismissed))
        {
            return;
        }
        ScaffoldFeatureController<MaterialBanner, MaterialBannerClosedReason> currentMaterialBanner = this._materialBanners.Peek();
        this._materialBanners.Clear();
        this._materialBanners.Enqueue(currentMaterialBanner);
        hideCurrentMaterialBanner();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        _accessibleNavigation = MediaQuery.accessibleNavigationOf(context);
        if (System.Linq.Enumerable.Any(this._snackBars))
        {
            dynamic route = global::Doroti.Framework.Widgets.ModalRoute<object>.of<object>(context);
            if (((route is null) || ((bool)((dynamic)route).isCurrent)))
            {
                if ((this._snackBarController!.isCompleted && (this._snackBarTimer is null)))
                {
                    SnackBar snackBar = this._snackBars.Peek()._widget;
                    _snackBarTimer = new Timer(((SnackBar)snackBar).duration, (() =>
                    {
                        DartRuntimePrimitives.Assert(() => this._snackBarController!.isForwardOrCompleted);
                        if (((SnackBar)snackBar).persist)
                        {
                            return;
                        }
                        hideCurrentSnackBar(reason: SnackBarClosedReason.timeout);
                    }));
                }
            }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _ScaffoldMessengerScope__scaffold(scaffoldMessengerState: this, child: ((ScaffoldMessenger)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._materialBannerController?.dispose();
        this._snackBarController?.dispose();
        this._snackBarTimer?.cancel();
        _snackBarTimer = null;
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
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
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
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _ScaffoldMessengerScope__scaffold : global::Doroti.Framework.Widgets.InheritedWidget
{
    internal virtual ScaffoldMessengerState _scaffoldMessengerState { get; private set; } = default!;

    internal _ScaffoldMessengerScope__scaffold(global::Doroti.Framework.Widgets.Widget child, ScaffoldMessengerState scaffoldMessengerState) : base(child: child)
    {
        this._scaffoldMessengerState = scaffoldMessengerState;
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => (!object.Equals(this._scaffoldMessengerState, ((_ScaffoldMessengerScope__scaffold)oldWidget)._scaffoldMessengerState));
}

public class ScaffoldPrelayoutGeometry
{
    public virtual Size floatingActionButtonSize { get; private set; } = default!;
    public virtual Size bottomSheetSize { get; private set; } = default!;
    public virtual double contentBottom { get; private set; } = default!;
    public virtual double contentTop { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets minInsets { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets minViewPadding { get; private set; } = default!;
    public virtual Size scaffoldSize { get; private set; } = default!;
    public virtual Size snackBarSize { get; private set; } = default!;
    public virtual Size materialBannerSize { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    public ScaffoldPrelayoutGeometry(Size bottomSheetSize, double contentBottom, double contentTop, Size floatingActionButtonSize, global::Doroti.Framework.Painting.EdgeInsets minInsets, global::Doroti.Framework.Painting.EdgeInsets minViewPadding, Size scaffoldSize, Size snackBarSize, Size materialBannerSize, TextDirection textDirection)
    {
        this.bottomSheetSize = bottomSheetSize;
        this.contentBottom = contentBottom;
        this.contentTop = contentTop;
        this.floatingActionButtonSize = floatingActionButtonSize;
        this.minInsets = minInsets;
        this.minViewPadding = minViewPadding;
        this.scaffoldSize = scaffoldSize;
        this.snackBarSize = snackBarSize;
        this.materialBannerSize = materialBannerSize;
        this.textDirection = textDirection;
    }

}

internal class _TransitionSnapshotFabLocation__scaffold : FloatingActionButtonLocation
{
    public virtual FloatingActionButtonLocation begin { get; private set; } = default!;
    public virtual FloatingActionButtonLocation end { get; private set; } = default!;
    public virtual FloatingActionButtonAnimator animator { get; private set; } = default!;
    public virtual double progress { get; private set; } = default!;

    internal _TransitionSnapshotFabLocation__scaffold(FloatingActionButtonLocation begin, FloatingActionButtonLocation end, FloatingActionButtonAnimator animator, double progress)
    {
        this.begin = begin;
        this.end = end;
        this.animator = animator;
        this.progress = progress;
    }

    public override global::Doroti.Ui.Offset getOffset(ScaffoldPrelayoutGeometry scaffoldGeometry)
    {
        return ((global::Doroti.Ui.Offset)(object?)this.animator.getOffset(begin: this.begin.getOffset(scaffoldGeometry), end: this.end.getOffset(scaffoldGeometry), progress: this.progress));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "_TransitionSnapshotFabLocation"))}(begin: {this.begin}, end: {this.end}, progress: {this.progress})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ScaffoldGeometry
{
    public virtual double? bottomNavigationBarTop { get; private set; }
    public virtual Rect? floatingActionButtonArea { get; private set; }

    public ScaffoldGeometry(double? bottomNavigationBarTop = null, Rect? floatingActionButtonArea = null)
    {
        this.bottomNavigationBarTop = bottomNavigationBarTop;
        this.floatingActionButtonArea = floatingActionButtonArea;
    }

    internal virtual ScaffoldGeometry _scaleFloatingActionButton(double scaleFactor)
    {
        if ((scaleFactor == 1.0))
        {
            return this;
        }
        if ((scaleFactor == 0.0))
        {
            return new ScaffoldGeometry(bottomNavigationBarTop: this.bottomNavigationBarTop);
        }
        global::Doroti.Ui.Rect scaledButton = ((global::Doroti.Ui.Rect)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Rect.lerp((((Offset)((dynamic)DartRuntimePrimitives.RequireValue(this.floatingActionButtonArea)).center) & Size.zero), this.floatingActionButtonArea, scaleFactor)));
        return ((ScaffoldGeometry)(object?)copyWith(floatingActionButtonArea: scaledButton));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ScaffoldGeometry copyWith(double? bottomNavigationBarTop = null, Rect? floatingActionButtonArea = null)
    {
        return new ScaffoldGeometry(bottomNavigationBarTop: (bottomNavigationBarTop ?? this.bottomNavigationBarTop), floatingActionButtonArea: (floatingActionButtonArea ?? this.floatingActionButtonArea));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ScaffoldGeometryNotifier__scaffold : global::Doroti.Framework.Foundation.ChangeNotifier, global::Doroti.Framework.Foundation.ValueListenable<ScaffoldGeometry>
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    public virtual double? floatingActionButtonScale { get; set; } = default;
    public virtual ScaffoldGeometry geometry { get; set; } = default!;

    internal _ScaffoldGeometryNotifier__scaffold(ScaffoldGeometry geometry, global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.geometry = geometry;
        this.context = context;
    }

    public virtual ScaffoldGeometry value
    {
        get
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    global::Doroti.Framework.Rendering.RenderObject? renderObject = ((global::Doroti.Framework.Rendering.RenderObject?)(object?)this.context.findRenderObject());
                    if (((renderObject is null) || !((global::Doroti.Framework.Rendering.RenderObject)renderObject).owner!.debugDoingPaint))
                    {
                        throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Scaffold.geometryOf() must only be accessed during the paint phase.\n" + "The ScaffoldGeometry is only available during the paint phase, because " + "its value is computed during the animation and layout phases prior to painting."));
                    }
                    return true;
                });
            return ((ScaffoldGeometry)(object?)this.geometry._scaleFloatingActionButton(DartRuntimePrimitives.RequireValue(this.floatingActionButtonScale)));
            return default!;
        }
    }
    internal virtual void _updateWith(double? bottomNavigationBarTop = null, Rect? floatingActionButtonArea = null, double? floatingActionButtonScale = null)
    {
        this.floatingActionButtonScale = (floatingActionButtonScale ?? this.floatingActionButtonScale);
        geometry = this.geometry.copyWith(bottomNavigationBarTop: bottomNavigationBarTop, floatingActionButtonArea: floatingActionButtonArea);
        notifyListeners();
    }

}

internal class _BodyBoxConstraints__scaffold : global::Doroti.Framework.Rendering.BoxConstraints
{
    public virtual double bottomWidgetsHeight { get; private set; } = default!;
    public virtual double appBarHeight { get; private set; } = default!;
    public virtual double materialBannerHeight { get; private set; } = default!;

    internal _BodyBoxConstraints__scaffold(double maxWidth = double.PositiveInfinity, double maxHeight = double.PositiveInfinity, double bottomWidgetsHeight = default!, double appBarHeight = default!, double materialBannerHeight = default!) : base(maxWidth: maxWidth, maxHeight: maxHeight)
    {
        this.bottomWidgetsHeight = bottomWidgetsHeight;
        this.appBarHeight = appBarHeight;
        this.materialBannerHeight = materialBannerHeight;
        System.Diagnostics.Debug.Assert((bottomWidgetsHeight >= 0L));
        System.Diagnostics.Debug.Assert((appBarHeight >= 0L));
        System.Diagnostics.Debug.Assert((materialBannerHeight >= 0L));
    }

    public override bool Equals(object? other)
    {
        var __other = other as _BodyBoxConstraints__scaffold;
        if (__other is null) return false;
        if (!base.Equals(__other))
        {
            return false;
        }
        return ((((__other is _BodyBoxConstraints__scaffold) && (((_BodyBoxConstraints__scaffold)((_BodyBoxConstraints__scaffold)__other)).materialBannerHeight == this.materialBannerHeight)) && (((_BodyBoxConstraints__scaffold)((_BodyBoxConstraints__scaffold)__other)).bottomWidgetsHeight == this.bottomWidgetsHeight)) && (((_BodyBoxConstraints__scaffold)((_BodyBoxConstraints__scaffold)__other)).appBarHeight == this.appBarHeight));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(base.GetHashCode(), this.materialBannerHeight, this.bottomWidgetsHeight, this.appBarHeight));
}

internal class _BodyBuilder__scaffold : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget body { get; private set; } = default!;
    public virtual bool extendBody { get; private set; } = default!;
    public virtual bool extendBodyBehindAppBar { get; private set; } = default!;

    internal _BodyBuilder__scaffold(bool extendBody, bool extendBodyBehindAppBar, global::Doroti.Framework.Widgets.Widget body)
    {
        this.extendBody = extendBody;
        this.extendBodyBehindAppBar = extendBodyBehindAppBar;
        this.body = body;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((!this.extendBody && !this.extendBodyBehindAppBar))
        {
            return this.body;
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, global::Doroti.Framework.Widgets.Widget>)((context, constraints) =>
        {
            var bodyConstraints = ((_BodyBoxConstraints__scaffold?)(object?)constraints)!;
            global::Doroti.Framework.Widgets.MediaQueryData metrics = ((global::Doroti.Framework.Widgets.MediaQueryData)(object?)MediaQuery.of(context));
            double bottomLocal = (this.extendBody ? Math.Max(((global::Doroti.Framework.Widgets.MediaQueryData)metrics).padding.bottom, ((_BodyBoxConstraints__scaffold)bodyConstraints).bottomWidgetsHeight) : ((global::Doroti.Framework.Widgets.MediaQueryData)metrics).padding.bottom);
            double topLocal = (this.extendBodyBehindAppBar ? Math.Max(((global::Doroti.Framework.Widgets.MediaQueryData)metrics).padding.top, (((_BodyBoxConstraints__scaffold)bodyConstraints).appBarHeight + ((_BodyBoxConstraints__scaffold)bodyConstraints).materialBannerHeight)) : ((global::Doroti.Framework.Widgets.MediaQueryData)metrics).padding.top);
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.MediaQuery(data: metrics.copyWith(padding: ((global::Doroti.Framework.Widgets.MediaQueryData)metrics).padding.copyWith(top: topLocal, bottom: bottomLocal)), child: this.body));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ScaffoldLayout__scaffold : global::Doroti.Framework.Rendering.MultiChildLayoutDelegate
{
    public virtual bool extendBody { get; private set; } = default!;
    public virtual bool extendBodyBehindAppBar { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets minInsets { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets minViewPadding { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual _ScaffoldGeometryNotifier__scaffold geometryNotifier { get; private set; } = default!;
    public virtual FloatingActionButtonLocation previousFloatingActionButtonLocation { get; private set; } = default!;
    public virtual FloatingActionButtonLocation currentFloatingActionButtonLocation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<double> floatingActionButtonMoveAnimation { get; private set; } = default!;
    public virtual FloatingActionButtonAnimator floatingActionButtonMotionAnimator { get; private set; } = default!;
    public virtual bool isSnackBarFloating { get; private set; } = default!;
    public virtual double? snackBarWidth { get; private set; }
    public virtual bool extendBodyBehindMaterialBanner { get; private set; } = default!;

    internal _ScaffoldLayout__scaffold(global::Doroti.Framework.Painting.EdgeInsets minInsets, global::Doroti.Framework.Painting.EdgeInsets minViewPadding, TextDirection textDirection, _ScaffoldGeometryNotifier__scaffold geometryNotifier, FloatingActionButtonLocation previousFloatingActionButtonLocation, FloatingActionButtonLocation currentFloatingActionButtonLocation, global::Doroti.Framework.Foundation.ValueListenable<double> floatingActionButtonMoveAnimation, FloatingActionButtonAnimator floatingActionButtonMotionAnimator, bool isSnackBarFloating, double? snackBarWidth, bool extendBody, bool extendBodyBehindAppBar, bool extendBodyBehindMaterialBanner) : base(relayout: floatingActionButtonMoveAnimation)
    {
        this.minInsets = minInsets;
        this.minViewPadding = minViewPadding;
        this.textDirection = textDirection;
        this.geometryNotifier = geometryNotifier;
        this.previousFloatingActionButtonLocation = previousFloatingActionButtonLocation;
        this.currentFloatingActionButtonLocation = currentFloatingActionButtonLocation;
        this.floatingActionButtonMoveAnimation = floatingActionButtonMoveAnimation;
        this.floatingActionButtonMotionAnimator = floatingActionButtonMotionAnimator;
        this.isSnackBarFloating = isSnackBarFloating;
        this.snackBarWidth = snackBarWidth;
        this.extendBody = extendBody;
        this.extendBodyBehindAppBar = extendBodyBehindAppBar;
        this.extendBodyBehindMaterialBanner = extendBodyBehindMaterialBanner;
    }

    public override void performLayout(Size size)
    {
        var looseConstraints = global::Doroti.Framework.Rendering.BoxConstraints.CreateLoose(size);
        global::Doroti.Framework.Rendering.BoxConstraints fullWidthConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)looseConstraints.tighten(width: size.width));
        double bottomLocal = size.height;
        var contentTopLocal = 0.0;
        var bottomWidgetsHeightLocal = 0.0;
        var appBarHeightLocal = 0.0;
        if (hasChild(_ScaffoldSlot__scaffold.appBar))
        {
            appBarHeightLocal = layoutChild(_ScaffoldSlot__scaffold.appBar, fullWidthConstraints).height;
            contentTopLocal = (this.extendBodyBehindAppBar ? 0.0 : appBarHeightLocal);
            positionChild(_ScaffoldSlot__scaffold.appBar, Offset.zero);
        }
        double? bottomNavigationBarTopLocal = default!;
        if (hasChild(_ScaffoldSlot__scaffold.bottomNavigationBar))
        {
            double bottomNavigationBarHeight = layoutChild(_ScaffoldSlot__scaffold.bottomNavigationBar, fullWidthConstraints).height;
            bottomWidgetsHeightLocal += bottomNavigationBarHeight;
            bottomNavigationBarTopLocal = Math.Max(0.0, (bottomLocal - bottomWidgetsHeightLocal));
            positionChild(_ScaffoldSlot__scaffold.bottomNavigationBar, new global::Doroti.Ui.Offset(0.0, DartRuntimePrimitives.RequireValue(bottomNavigationBarTopLocal)));
        }
        if (hasChild(_ScaffoldSlot__scaffold.persistentFooter))
        {
            var footerConstraints = new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: ((global::Doroti.Framework.Rendering.BoxConstraints)fullWidthConstraints).maxWidth, maxHeight: Math.Max(0.0, ((bottomLocal - bottomWidgetsHeightLocal) - contentTopLocal)));
            double persistentFooterHeight = layoutChild(_ScaffoldSlot__scaffold.persistentFooter, footerConstraints).height;
            bottomWidgetsHeightLocal += persistentFooterHeight;
            positionChild(_ScaffoldSlot__scaffold.persistentFooter, new global::Doroti.Ui.Offset(0.0, Math.Max(0.0, (bottomLocal - bottomWidgetsHeightLocal))));
        }
        global::Doroti.Ui.Size materialBannerSizeLocal = ((global::Doroti.Ui.Size)(object?)Size.zero);
        if (hasChild(_ScaffoldSlot__scaffold.materialBanner))
        {
            materialBannerSizeLocal = layoutChild(_ScaffoldSlot__scaffold.materialBanner, fullWidthConstraints);
            positionChild(_ScaffoldSlot__scaffold.materialBanner, new global::Doroti.Ui.Offset(0.0, appBarHeightLocal));
            if (!this.extendBodyBehindMaterialBanner)
            {
                contentTopLocal += materialBannerSizeLocal.height;
            }
        }
        double contentBottomLocal = Math.Max(0.0, (bottomLocal - Math.Max(((global::Doroti.Framework.Painting.EdgeInsets)this.minInsets).bottom, bottomWidgetsHeightLocal)));
        if (hasChild(_ScaffoldSlot__scaffold.body))
        {
            double bodyMaxHeight = Math.Max(0.0, (contentBottomLocal - contentTopLocal));
            if ((this.extendBody && (((global::Doroti.Framework.Painting.EdgeInsets)this.minInsets).bottom <= bottomWidgetsHeightLocal)))
            {
                bodyMaxHeight += bottomWidgetsHeightLocal;
                bodyMaxHeight = Dart_uiLibrary.clampDouble(bodyMaxHeight, 0.0, (((global::Doroti.Framework.Rendering.BoxConstraints)looseConstraints).maxHeight - contentTopLocal));
                DartRuntimePrimitives.Assert(() => (bodyMaxHeight <= Math.Max(0.0, (((global::Doroti.Framework.Rendering.BoxConstraints)looseConstraints).maxHeight - contentTopLocal))));
            }
            else
            {
                bottomWidgetsHeightLocal = 0.0;
            }
            global::Doroti.Framework.Rendering.BoxConstraints bodyConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)new _BodyBoxConstraints__scaffold(maxWidth: ((global::Doroti.Framework.Rendering.BoxConstraints)fullWidthConstraints).maxWidth, maxHeight: bodyMaxHeight, materialBannerHeight: materialBannerSizeLocal.height, bottomWidgetsHeight: bottomWidgetsHeightLocal, appBarHeight: appBarHeightLocal));
            layoutChild(_ScaffoldSlot__scaffold.body, bodyConstraints);
            positionChild(_ScaffoldSlot__scaffold.body, new global::Doroti.Ui.Offset(0.0, contentTopLocal));
        }
        global::Doroti.Ui.Size bottomSheetSizeLocal = ((global::Doroti.Ui.Size)(object?)Size.zero);
        global::Doroti.Ui.Size snackBarSizeLocal = ((global::Doroti.Ui.Size)(object?)Size.zero);
        if (hasChild(_ScaffoldSlot__scaffold.bodyScrim))
        {
            var bottomSheetScrimConstraints = new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: ((global::Doroti.Framework.Rendering.BoxConstraints)fullWidthConstraints).maxWidth, maxHeight: contentBottomLocal);
            layoutChild(_ScaffoldSlot__scaffold.bodyScrim, bottomSheetScrimConstraints);
            positionChild(_ScaffoldSlot__scaffold.bodyScrim, Offset.zero);
        }
        if ((hasChild(_ScaffoldSlot__scaffold.snackBar) && !this.isSnackBarFloating))
        {
            snackBarSizeLocal = layoutChild(_ScaffoldSlot__scaffold.snackBar, fullWidthConstraints);
        }
        if (hasChild(_ScaffoldSlot__scaffold.bottomSheet))
        {
            var bottomSheetConstraints = new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: ((global::Doroti.Framework.Rendering.BoxConstraints)fullWidthConstraints).maxWidth, maxHeight: Math.Max(0.0, (contentBottomLocal - contentTopLocal)));
            bottomSheetSizeLocal = layoutChild(_ScaffoldSlot__scaffold.bottomSheet, bottomSheetConstraints);
            positionChild(_ScaffoldSlot__scaffold.bottomSheet, new global::Doroti.Ui.Offset((((size.width - bottomSheetSizeLocal.width)) / 2.0), (contentBottomLocal - bottomSheetSizeLocal.height)));
        }
        global::Doroti.Ui.Rect floatingActionButtonRect = default!;
        if (hasChild(_ScaffoldSlot__scaffold.floatingActionButton))
        {
            global::Doroti.Ui.Size fabSize = ((global::Doroti.Ui.Size)(object?)layoutChild(_ScaffoldSlot__scaffold.floatingActionButton, looseConstraints));
            var currentGeometry = new ScaffoldPrelayoutGeometry(bottomSheetSize: bottomSheetSizeLocal, contentBottom: contentBottomLocal, contentTop: appBarHeightLocal, floatingActionButtonSize: fabSize, minInsets: this.minInsets, scaffoldSize: size, snackBarSize: snackBarSizeLocal, materialBannerSize: materialBannerSizeLocal, textDirection: this.textDirection, minViewPadding: this.minViewPadding);
            global::Doroti.Ui.Offset currentFabOffset = ((global::Doroti.Ui.Offset)(object?)this.currentFloatingActionButtonLocation.getOffset(currentGeometry));
            global::Doroti.Ui.Offset previousFabOffset = ((global::Doroti.Ui.Offset)(object?)this.previousFloatingActionButtonLocation.getOffset(currentGeometry));
            global::Doroti.Ui.Offset fabOffset = ((global::Doroti.Ui.Offset)(object?)this.floatingActionButtonMotionAnimator.getOffset(begin: previousFabOffset, end: currentFabOffset, progress: ((global::Doroti.Framework.Foundation.ValueListenable<double>)this.floatingActionButtonMoveAnimation).value));
            positionChild(_ScaffoldSlot__scaffold.floatingActionButton, fabOffset);
            floatingActionButtonRect = (fabOffset & fabSize);
        }
        if (hasChild(_ScaffoldSlot__scaffold.snackBar))
        {
            bool hasCustomWidth = ((this.snackBarWidth is not null) && (DartRuntimePrimitives.RequireValue(this.snackBarWidth) < size.width));
            if ((object.Equals(snackBarSizeLocal, Size.zero)))
            {
                snackBarSizeLocal = layoutChild(_ScaffoldSlot__scaffold.snackBar, (hasCustomWidth ? looseConstraints : fullWidthConstraints));
            }
            double snackBarYOffsetBase = default!;
            bool showAboveFab = this.currentFloatingActionButtonLocation is not null;
            if ((((!object.Equals(floatingActionButtonRect.size, Size.zero)) && this.isSnackBarFloating) && showAboveFab))
            {
                if ((bottomNavigationBarTopLocal is not null))
                {
                    double bottomNavigationBarTop__40047__value47887 = DartRuntimePrimitives.RequireValue(bottomNavigationBarTopLocal);
                    snackBarYOffsetBase = Math.Min(DartRuntimePrimitives.RequireValue(bottomNavigationBarTop__40047__value47887), floatingActionButtonRect.top);
                }
                else
                {
                    snackBarYOffsetBase = floatingActionButtonRect.top;
                }
            }
            else
            {
                double safeYOffsetBase = (size.height - ((global::Doroti.Framework.Painting.EdgeInsets)this.minViewPadding).bottom);
                snackBarYOffsetBase = (this.isSnackBarFloating ? Math.Min(contentBottomLocal, safeYOffsetBase) : contentBottomLocal);
            }
            double xOffset = (hasCustomWidth ? (((size.width - DartRuntimePrimitives.RequireValue(this.snackBarWidth))) / 2L) : 0.0);
            positionChild(_ScaffoldSlot__scaffold.snackBar, new global::Doroti.Ui.Offset(xOffset, (snackBarYOffsetBase - snackBarSizeLocal.height)));
            DartRuntimePrimitives.Assert(() =>
                {
                    if (this.isSnackBarFloating)
                    {
                        bool snackBarVisible = (((snackBarYOffsetBase - snackBarSizeLocal.height)) >= 0L);
                        if (!snackBarVisible)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Floating SnackBar presented off screen."), new global::Doroti.Framework.Foundation.ErrorDescription("A SnackBar with behavior property set to SnackBarBehavior.floating is fully " + "or partially off screen because some or all the widgets provided to " + "Scaffold.floatingActionButton, Scaffold.persistentFooterButtons and " + "Scaffold.bottomNavigationBar take up too much vertical space.\n"), new global::Doroti.Framework.Foundation.ErrorHint("Consider constraining the size of these widgets to allow room for the SnackBar to be visible.") }));
                        }
                    }
                    return true;
                });
        }
        if (hasChild(_ScaffoldSlot__scaffold.statusBar))
        {
            layoutChild(_ScaffoldSlot__scaffold.statusBar, fullWidthConstraints.tighten(height: ((global::Doroti.Framework.Painting.EdgeInsets)this.minInsets).top));
            positionChild(_ScaffoldSlot__scaffold.statusBar, Offset.zero);
        }
        if (hasChild(_ScaffoldSlot__scaffold.drawer))
        {
            layoutChild(_ScaffoldSlot__scaffold.drawer, global::Doroti.Framework.Rendering.BoxConstraints.CreateTight(size));
            positionChild(_ScaffoldSlot__scaffold.drawer, Offset.zero);
        }
        if (hasChild(_ScaffoldSlot__scaffold.endDrawer))
        {
            layoutChild(_ScaffoldSlot__scaffold.endDrawer, global::Doroti.Framework.Rendering.BoxConstraints.CreateTight(size));
            positionChild(_ScaffoldSlot__scaffold.endDrawer, Offset.zero);
        }
        this.geometryNotifier._updateWith(bottomNavigationBarTop: bottomNavigationBarTopLocal, floatingActionButtonArea: floatingActionButtonRect);
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_ScaffoldLayout__scaffold)(object)oldDelegate;
        return (((((((!object.Equals(((_ScaffoldLayout__scaffold)__oldDelegate).minInsets, this.minInsets)) || (!object.Equals(((_ScaffoldLayout__scaffold)__oldDelegate).minViewPadding, this.minViewPadding))) || (!object.Equals(((_ScaffoldLayout__scaffold)__oldDelegate).textDirection, this.textDirection))) || (!object.Equals(((_ScaffoldLayout__scaffold)__oldDelegate).previousFloatingActionButtonLocation, this.previousFloatingActionButtonLocation))) || (!object.Equals(((_ScaffoldLayout__scaffold)__oldDelegate).currentFloatingActionButtonLocation, this.currentFloatingActionButtonLocation))) || (((_ScaffoldLayout__scaffold)__oldDelegate).extendBody != this.extendBody)) || (((_ScaffoldLayout__scaffold)__oldDelegate).extendBodyBehindAppBar != this.extendBodyBehindAppBar));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _FloatingActionButtonTransition__scaffold : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Framework.Animation.Animation<double> fabMoveAnimation { get; private set; } = default!;
    public virtual FloatingActionButtonAnimator fabMotionAnimator { get; private set; } = default!;
    public virtual _ScaffoldGeometryNotifier__scaffold geometryNotifier { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController currentController { get; private set; } = default!;

    internal _FloatingActionButtonTransition__scaffold(global::Doroti.Framework.Widgets.Widget? child, global::Doroti.Framework.Animation.Animation<double> fabMoveAnimation, FloatingActionButtonAnimator fabMotionAnimator, _ScaffoldGeometryNotifier__scaffold geometryNotifier, global::Doroti.Framework.Animation.AnimationController currentController)
    {
        this.child = child;
        this.fabMoveAnimation = fabMoveAnimation;
        this.fabMotionAnimator = fabMotionAnimator;
        this.geometryNotifier = geometryNotifier;
        this.currentController = currentController;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _FloatingActionButtonTransitionState__scaffold());
}

public class _FloatingActionButtonTransitionState__scaffold : global::Doroti.Framework.Widgets.State<_FloatingActionButtonTransition__scaffold>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<_FloatingActionButtonTransition__scaffold>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _previousController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _previousExitScaleAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _previousExitRotationCurvedAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _currentEntranceScaleAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _previousScaleAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.TrainHoppingAnimation _previousRotationAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _currentScaleAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _extendedCurrentScaleAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.TrainHoppingAnimation _currentRotationAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.Widget? _previousChild { get; set; } = default;
    internal static global::Doroti.Framework.Animation.Animatable<double> _entranceTurnTween = new global::Doroti.Framework.Animation.Tween<double>(begin: (1.0 - Floating_action_button_locationLibrary.kFloatingActionButtonTurnInterval), end: 1.0).chain(new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.easeIn));
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _previousController = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: Floating_action_button_locationLibrary.kFloatingActionButtonSegue, vsync: this);
    __cascade.addStatusListener((AnimationStatusListener)this._handlePreviousAnimationStatusChanged);
    return __cascade;
}))();
        _updateAnimations();
        if ((((_FloatingActionButtonTransition__scaffold)(object)this.widget).child is not null))
        {
            ((_FloatingActionButtonTransition__scaffold)(object)this.widget).currentController.value = 1.0;
            _updateGeometryScale(1.0);
        }
        else
        {
            _updateGeometryScale(0.0);
        }
    }

    public override void dispose()
    {
        this._previousController.dispose();
        this._previousExitScaleAnimation?.dispose();
        this._previousExitRotationCurvedAnimation?.dispose();
        this._currentEntranceScaleAnimation?.dispose();
        _disposeAnimations();
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
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(_FloatingActionButtonTransition__scaffold oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((!object.Equals(((_FloatingActionButtonTransition__scaffold)oldWidget).fabMotionAnimator, ((_FloatingActionButtonTransition__scaffold)(object)this.widget).fabMotionAnimator)) || (!object.Equals(((_FloatingActionButtonTransition__scaffold)oldWidget).fabMoveAnimation, ((_FloatingActionButtonTransition__scaffold)(object)this.widget).fabMoveAnimation))))
        {
            _disposeAnimations();
            _updateAnimations();
        }
        var oldChildIsNull = (((_FloatingActionButtonTransition__scaffold)oldWidget).child is null);
        var newChildIsNull = (((_FloatingActionButtonTransition__scaffold)(object)this.widget).child is null);
        if (((oldChildIsNull == newChildIsNull) && (object.Equals(((_FloatingActionButtonTransition__scaffold)oldWidget).child?.key, ((_FloatingActionButtonTransition__scaffold)(object)this.widget).child?.key))))
        {
            return;
        }
        if (this._previousController.isDismissed)
        {
            double currentValue = ((_FloatingActionButtonTransition__scaffold)(object)this.widget).currentController.value;
            if (((currentValue == 0.0) || (((_FloatingActionButtonTransition__scaffold)oldWidget).child is null)))
            {
                _previousChild = null;
                if ((((_FloatingActionButtonTransition__scaffold)(object)this.widget).child is not null))
                {
                    ((_FloatingActionButtonTransition__scaffold)(object)this.widget).currentController.forward();
                }
            }
            else
            {
                _previousChild = ((_FloatingActionButtonTransition__scaffold)oldWidget).child;
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = this._previousController;
    __cascade.value = currentValue;
    __cascade.reverse();
    return __cascade;
}))());
                ((_FloatingActionButtonTransition__scaffold)(object)this.widget).currentController.value = 0.0;
            }
        }
    }

    internal virtual void _disposeAnimations()
    {
        this._previousRotationAnimation.dispose();
        this._currentRotationAnimation.dispose();
    }

    internal virtual void _updateAnimations()
    {
        this._previousExitScaleAnimation?.dispose();
        _previousExitScaleAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._previousController, curve: global::Doroti.Framework.Animation.Curves.easeIn);
        this._previousExitRotationCurvedAnimation?.dispose();
        _previousExitRotationCurvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._previousController, curve: global::Doroti.Framework.Animation.Curves.easeIn);
        global::Doroti.Framework.Animation.Animation<double> previousExitRotationAnimation = ((global::Doroti.Framework.Animation.Animation<double>)(object?)new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 1.0).animate(this._previousExitRotationCurvedAnimation!));
        this._currentEntranceScaleAnimation?.dispose();
        _currentEntranceScaleAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_FloatingActionButtonTransition__scaffold)(object)this.widget).currentController, curve: global::Doroti.Framework.Animation.Curves.easeIn);
        global::Doroti.Framework.Animation.Animation<double> currentEntranceRotationAnimation = ((global::Doroti.Framework.Animation.Animation<double>)(object?)((_FloatingActionButtonTransition__scaffold)(object)this.widget).currentController.drive(_entranceTurnTween));
        global::Doroti.Framework.Animation.Animation<double> moveScaleAnimation = ((_FloatingActionButtonTransition__scaffold)(object)this.widget).fabMotionAnimator.getScaleAnimation(parent: ((_FloatingActionButtonTransition__scaffold)(object)this.widget).fabMoveAnimation);
        global::Doroti.Framework.Animation.Animation<double> moveRotationAnimation = ((_FloatingActionButtonTransition__scaffold)(object)this.widget).fabMotionAnimator.getRotationAnimation(parent: ((_FloatingActionButtonTransition__scaffold)(object)this.widget).fabMoveAnimation);
        if ((object.Equals(((_FloatingActionButtonTransition__scaffold)(object)this.widget).fabMotionAnimator, FloatingActionButtonAnimator.noAnimation)))
        {
            _previousScaleAnimation = moveScaleAnimation;
            _currentScaleAnimation = moveScaleAnimation;
            _previousRotationAnimation = new global::Doroti.Framework.Animation.TrainHoppingAnimation(moveRotationAnimation, null);
            _currentRotationAnimation = new global::Doroti.Framework.Animation.TrainHoppingAnimation(moveRotationAnimation, null);
        }
        else
        {
            _previousScaleAnimation = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.AnimationMin<double>(moveScaleAnimation, this._previousExitScaleAnimation!));
            _currentScaleAnimation = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.AnimationMin<double>(moveScaleAnimation, this._currentEntranceScaleAnimation!));
            _previousRotationAnimation = new global::Doroti.Framework.Animation.TrainHoppingAnimation(previousExitRotationAnimation, moveRotationAnimation);
            _currentRotationAnimation = new global::Doroti.Framework.Animation.TrainHoppingAnimation(currentEntranceRotationAnimation, moveRotationAnimation);
        }
        _extendedCurrentScaleAnimation = this._currentScaleAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.0, 0.1)));
        this._currentScaleAnimation.addListener(this._onProgressChanged);
        this._previousScaleAnimation.addListener(this._onProgressChanged);
    }

    internal virtual void _handlePreviousAnimationStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        setState(((global::System.Action)(() =>
        {
            if (((((_FloatingActionButtonTransition__scaffold)(object)this.widget).child is not null) && global::Doroti.Framework.Animation.AnimationStatusMembers.isDismissed(status)))
            {
                DartRuntimePrimitives.Assert(() => ((_FloatingActionButtonTransition__scaffold)(object)this.widget).currentController.isDismissed);
                ((_FloatingActionButtonTransition__scaffold)(object)this.widget).currentController.forward();
            }
        })));
    }

    internal virtual bool _isExtendedFloatingActionButton(global::Doroti.Framework.Widgets.Widget? widget)
    {
        return (widget is FloatingActionButton floatingActionButton && floatingActionButton.isExtended);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(alignment: global::Doroti.Framework.Painting.Alignment.centerRight, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection59262 = new List<global::Doroti.Framework.Widgets.Widget>(); if (!this._previousController.isDismissed) { if (_isExtendedFloatingActionButton(this._previousChild)) { __collection59262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: this._previousScaleAnimation, child: this._previousChild))); } else { __collection59262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ScaleTransition(scale: this._previousScaleAnimation, child: new global::Doroti.Framework.Widgets.RotationTransition(turns: this._previousRotationAnimation, child: this._previousChild)))); } } if (_isExtendedFloatingActionButton(((_FloatingActionButtonTransition__scaffold)(object)this.widget).child)) { __collection59262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ScaleTransition(scale: this._extendedCurrentScaleAnimation, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: this._currentScaleAnimation, child: ((_FloatingActionButtonTransition__scaffold)(object)this.widget).child)))); } else { __collection59262.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ScaleTransition(scale: this._currentScaleAnimation, child: new global::Doroti.Framework.Widgets.RotationTransition(turns: this._currentRotationAnimation, child: ((_FloatingActionButtonTransition__scaffold)(object)this.widget).child)))); } return __collection59262; }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onProgressChanged()
    {
        _updateGeometryScale(Math.Max(((global::Doroti.Framework.Animation.Animation<double>)this._previousScaleAnimation).value, ((global::Doroti.Framework.Animation.Animation<double>)this._currentScaleAnimation).value));
    }

    internal virtual void _updateGeometryScale(double scale)
    {
        ((_FloatingActionButtonTransition__scaffold)(object)this.widget).geometryNotifier._updateWith(floatingActionButtonScale: scale);
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
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

public class Scaffold : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool extendBody { get; private set; } = default!;
    public virtual bool drawerBarrierDismissible { get; private set; } = default!;
    public virtual bool extendBodyBehindAppBar { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.PreferredSizeWidget? appBar { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? body { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? floatingActionButton { get; private set; }
    public virtual FloatingActionButtonLocation? floatingActionButtonLocation { get; private set; }
    public virtual FloatingActionButtonAnimator? floatingActionButtonAnimator { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget>? persistentFooterButtons { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentDirectional persistentFooterAlignment { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BoxDecoration? persistentFooterDecoration { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? drawer { get; private set; }
    public virtual global::System.Action<bool>? onDrawerChanged { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? endDrawer { get; private set; }
    public virtual global::System.Action<bool>? onEndDrawerChanged { get; private set; }
    public virtual Color? drawerScrimColor { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget?> bottomSheetScrimBuilder { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomNavigationBar { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomSheet { get; private set; }
    public virtual bool? resizeToAvoidBottomInset { get; private set; }
    public virtual bool primary { get; private set; } = default!;
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior drawerDragStartBehavior { get; private set; } = default!;
    public virtual double? drawerEdgeDragWidth { get; private set; }
    public virtual bool drawerEnableOpenDragGesture { get; private set; } = default!;
    public virtual bool endDrawerEnableOpenDragGesture { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }

    public Scaffold(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.PreferredSizeWidget? appBar = null, global::Doroti.Framework.Widgets.Widget? body = null, global::Doroti.Framework.Widgets.Widget? floatingActionButton = null, FloatingActionButtonLocation? floatingActionButtonLocation = null, FloatingActionButtonAnimator? floatingActionButtonAnimator = null, List<global::Doroti.Framework.Widgets.Widget>? persistentFooterButtons = null, global::Doroti.Framework.Painting.AlignmentDirectional persistentFooterAlignment = default!, global::Doroti.Framework.Painting.BoxDecoration? persistentFooterDecoration = null, global::Doroti.Framework.Widgets.Widget? drawer = null, global::System.Action<bool>? onDrawerChanged = null, global::Doroti.Framework.Widgets.Widget? endDrawer = null, global::System.Action<bool>? onEndDrawerChanged = null, global::Doroti.Framework.Widgets.Widget? bottomNavigationBar = null, global::Doroti.Framework.Widgets.Widget? bottomSheet = null, Color? backgroundColor = null, bool? resizeToAvoidBottomInset = null, bool primary = true, global::Doroti.Framework.Gestures.DragStartBehavior drawerDragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, bool extendBody = false, bool drawerBarrierDismissible = true, bool extendBodyBehindAppBar = false, Color? drawerScrimColor = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget?> bottomSheetScrimBuilder = default!, double? drawerEdgeDragWidth = null, bool drawerEnableOpenDragGesture = true, bool endDrawerEnableOpenDragGesture = true, string? restorationId = null) : base(key: key)
    {
        global::Doroti.Framework.Painting.AlignmentDirectional __persistentFooterAlignment = persistentFooterAlignment ?? global::Doroti.Framework.Painting.AlignmentDirectional.centerEnd;
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget?> __bottomSheetScrimBuilder = bottomSheetScrimBuilder ?? _defaultBottomSheetScrimBuilder;
        this.appBar = appBar;
        this.body = body;
        this.floatingActionButton = floatingActionButton;
        this.floatingActionButtonLocation = floatingActionButtonLocation;
        this.floatingActionButtonAnimator = floatingActionButtonAnimator;
        this.persistentFooterButtons = persistentFooterButtons;
        this.persistentFooterAlignment = __persistentFooterAlignment;
        this.persistentFooterDecoration = persistentFooterDecoration;
        this.drawer = drawer;
        this.onDrawerChanged = onDrawerChanged;
        this.endDrawer = endDrawer;
        this.onEndDrawerChanged = onEndDrawerChanged;
        this.bottomNavigationBar = bottomNavigationBar;
        this.bottomSheet = bottomSheet;
        this.backgroundColor = backgroundColor;
        this.resizeToAvoidBottomInset = resizeToAvoidBottomInset;
        this.primary = primary;
        this.drawerDragStartBehavior = drawerDragStartBehavior;
        this.extendBody = extendBody;
        this.drawerBarrierDismissible = drawerBarrierDismissible;
        this.extendBodyBehindAppBar = extendBodyBehindAppBar;
        this.drawerScrimColor = drawerScrimColor;
        this.bottomSheetScrimBuilder = __bottomSheetScrimBuilder;
        this.drawerEdgeDragWidth = drawerEdgeDragWidth;
        this.drawerEnableOpenDragGesture = drawerEnableOpenDragGesture;
        this.endDrawerEnableOpenDragGesture = endDrawerEnableOpenDragGesture;
        this.restorationId = restorationId;
    }

    public static ScaffoldState of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ScaffoldState? result = ((ScaffoldState?)(object?)context.findAncestorStateOfType<ScaffoldState>());
        if ((result is not null))
        {
            return result;
        }
        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Scaffold.of() called with a context that does not contain a Scaffold."), new global::Doroti.Framework.Foundation.ErrorDescription("No Scaffold ancestor could be found starting from the context that was passed to Scaffold.of(). " + "This usually happens when the context provided is from the same StatefulWidget as that " + "whose build function actually creates the Scaffold widget being sought."), new global::Doroti.Framework.Foundation.ErrorHint("There are several ways to avoid this problem. The simplest is to use a Builder to get a " + "context that is \"under\" the Scaffold. For an example of this, please see the " + "documentation for Scaffold.of():\n" + "  https://api.flutter.dev/flutter/material/Scaffold/of.html"), new global::Doroti.Framework.Foundation.ErrorHint("A more efficient solution is to split your build function into several widgets. This " + "introduces a new context from which you can obtain the Scaffold. In this solution, " + "you would have an outer widget that creates the Scaffold populated by instances of " + "your new inner widgets, and then in these inner widgets you would use Scaffold.of().\n" + "A less elegant but more expedient solution is assign a GlobalKey to the Scaffold, " + "then use the key.currentState property to obtain the ScaffoldState rather than " + "using the Scaffold.of() function."), context.describeElement("The context used was") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ScaffoldState? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((ScaffoldState?)(object?)context.findAncestorStateOfType<ScaffoldState>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Foundation.ValueListenable<ScaffoldGeometry> geometryOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _ScaffoldScope__scaffold? scaffoldScope = ((_ScaffoldScope__scaffold?)(object?)context.dependOnInheritedWidgetOfExactType<_ScaffoldScope__scaffold>());
        if ((scaffoldScope is null))
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Scaffold.geometryOf() called with a context that does not contain a Scaffold."), new global::Doroti.Framework.Foundation.ErrorDescription("This usually happens when the context provided is from the same StatefulWidget as that " + "whose build function actually creates the Scaffold widget being sought."), new global::Doroti.Framework.Foundation.ErrorHint("There are several ways to avoid this problem. The simplest is to use a Builder to get a " + "context that is \"under\" the Scaffold. For an example of this, please see the " + "documentation for Scaffold.of():\n" + "  https://api.flutter.dev/flutter/material/Scaffold/of.html"), new global::Doroti.Framework.Foundation.ErrorHint("A more efficient solution is to split your build function into several widgets. This " + "introduces a new context from which you can obtain the Scaffold. In this solution, " + "you would have an outer widget that creates the Scaffold populated by instances of " + "your new inner widgets, and then in these inner widgets you would use Scaffold.geometryOf()."), context.describeElement("The context used was") }));
        }
        return ((global::Doroti.Framework.Foundation.ValueListenable<ScaffoldGeometry>)(object?)((_ScaffoldScope__scaffold)scaffoldScope).geometryNotifier);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool hasDrawer(global::Doroti.Framework.Widgets.BuildContext context, bool registerForUpdates = true)
    {
        if (registerForUpdates)
        {
            _ScaffoldScope__scaffold? scaffold = ((_ScaffoldScope__scaffold?)(object?)context.dependOnInheritedWidgetOfExactType<_ScaffoldScope__scaffold>());
            return (scaffold?.hasDrawer ?? false);
        }
        else
        {
            ScaffoldState? scaffoldLocal = ((ScaffoldState?)(object?)context.findAncestorStateOfType<ScaffoldState>());
            return (scaffoldLocal?.hasDrawer ?? false);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Widgets.Widget _defaultBottomSheetScrimBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: animation, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            double extentRemaining = (ScaffoldLibrary._kBottomSheetDominatesPercentage * ((1.0 - ((global::Doroti.Framework.Animation.Animation<double>)animation).value)));
            double floatingButtonVisibilityValue = ((extentRemaining * ScaffoldLibrary._kBottomSheetDominatesPercentage) * 10L);
            double opacity = Math.Max(ScaffoldLibrary._kMinBottomSheetScrimOpacity, (ScaffoldLibrary._kMaxBottomSheetScrimOpacity - floatingButtonVisibilityValue));
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ModalBarrier(dismissible: false, color: Colors.black.withOpacity(opacity)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new ScaffoldState());
}

public class ScaffoldState : global::Doroti.Framework.Widgets.State<Scaffold>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<Scaffold>, global::Doroti.Framework.Widgets.RestorationMixin<Scaffold>, global::Doroti.Framework.Widgets.WidgetsBindingObserver
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState> _drawerKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState>.Create();
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState> _endDrawerKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState>.Create();
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _bodyKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    private bool __late__statusBarKey_initialized;
    private global::Doroti.Framework.Widgets.GlobalKey<IState> __late__statusBarKey = default!;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _statusBarKey
    {
        get
        {
            if (!__late__statusBarKey_initialized)
            {
                __late__statusBarKey = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
                __late__statusBarKey_initialized = true;
            }
            return __late__statusBarKey;
        }
    }
    internal virtual double? _appBarMaxHeight { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.RestorableBool _drawerOpened { get; private set; } = new global::Doroti.Framework.Widgets.RestorableBool(false);
    internal virtual global::Doroti.Framework.Widgets.RestorableBool _endDrawerOpened { get; private set; } = new global::Doroti.Framework.Widgets.RestorableBool(false);
    internal virtual ScaffoldMessengerState? _scaffoldMessenger { get; set; } = default;
    internal virtual ScaffoldFeatureController<SnackBar, SnackBarClosedReason>? _messengerSnackBar { get; set; } = default;
    internal virtual ScaffoldFeatureController<MaterialBanner, MaterialBannerClosedReason>? _messengerMaterialBanner { get; set; } = default;
    internal virtual List<_StandardBottomSheet__scaffold> _dismissedBottomSheets { get; private set; } = new List<_StandardBottomSheet__scaffold>();
    internal virtual PersistentBottomSheetController? _currentBottomSheet { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _currentBottomSheetKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Framework.Widgets.LocalHistoryEntry? _persistentSheetHistoryEntry { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.AnimationController _floatingActionButtonMoveController { get; set; } = default!;
    internal virtual FloatingActionButtonAnimator _floatingActionButtonAnimator { get; set; } = default!;
    internal virtual FloatingActionButtonLocation? _previousFloatingActionButtonLocation { get; set; } = default;
    internal virtual FloatingActionButtonLocation? _floatingActionButtonLocation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.AnimationController _floatingActionButtonVisibilityController { get; set; } = default!;
    internal virtual _ScaffoldGeometryNotifier__scaffold _geometryNotifier { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _bottomSheetScrimAnimationController { get; set; } = default!;
    internal virtual bool _showBodyScrim { get; set; } = false;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public virtual string? restorationId => ((Scaffold)(object)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._drawerOpened), "drawer_open");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._endDrawerOpened), "end_drawer_open");
    }

    public virtual bool hasAppBar => DartRuntimePrimitives.ConvertValue<bool>((((Scaffold)(object)this.widget).appBar is not null));
    public virtual bool hasDrawer => DartRuntimePrimitives.ConvertValue<bool>((((Scaffold)(object)this.widget).drawer is not null));
    public virtual bool hasEndDrawer => DartRuntimePrimitives.ConvertValue<bool>((((Scaffold)(object)this.widget).endDrawer is not null));
    public virtual bool hasFloatingActionButton => DartRuntimePrimitives.ConvertValue<bool>((((Scaffold)(object)this.widget).floatingActionButton is not null));
    public virtual double? appBarMaxHeight => this._appBarMaxHeight;
    public virtual bool isDrawerOpen => this._drawerOpened.value;
    public virtual bool isDrawerBarrierDismissible => ((Scaffold)(object)this.widget).drawerBarrierDismissible;
    public virtual bool isEndDrawerOpen => this._endDrawerOpened.value;
    internal virtual void _drawerOpenedCallback(bool isOpened)
    {
        if (((this._drawerOpened.value != isOpened) && (((global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState>)this._drawerKey).currentState is not null)))
        {
            setState(((global::System.Action)(() =>
            {
                this._drawerOpened.value = isOpened;
            })));
            ((Scaffold)(object)this.widget).onDrawerChanged?.Invoke(isOpened);
        }
    }

    internal virtual void _endDrawerOpenedCallback(bool isOpened)
    {
        if (((this._endDrawerOpened.value != isOpened) && (((global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState>)this._endDrawerKey).currentState is not null)))
        {
            setState(((global::System.Action)(() =>
            {
                this._endDrawerOpened.value = isOpened;
            })));
            ((Scaffold)(object)this.widget).onEndDrawerChanged?.Invoke(isOpened);
        }
    }

    public virtual void openDrawer()
    {
        if (((((global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState>)this._endDrawerKey).currentState is not null) && this._endDrawerOpened.value))
        {
            ((global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState>)this._endDrawerKey).currentState!.close();
        }
        ((global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState>)this._drawerKey).currentState?.open();
    }

    public virtual void openEndDrawer()
    {
        if (((((global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState>)this._drawerKey).currentState is not null) && this._drawerOpened.value))
        {
            ((global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState>)this._drawerKey).currentState!.close();
        }
        ((global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState>)this._endDrawerKey).currentState?.open();
    }

    internal virtual void _updateSnackBar()
    {
        ScaffoldFeatureController<SnackBar, SnackBarClosedReason>? messengerSnackBar = (System.Linq.Enumerable.Any(this._scaffoldMessenger!._snackBars) ? this._scaffoldMessenger!._snackBars.Peek() : null);
        if ((!object.Equals(this._messengerSnackBar, messengerSnackBar)))
        {
            setState(((global::System.Action)(() =>
            {
                _messengerSnackBar = messengerSnackBar;
            })));
        }
    }

    internal virtual void _updateMaterialBanner()
    {
        ScaffoldFeatureController<MaterialBanner, MaterialBannerClosedReason>? messengerMaterialBanner = (System.Linq.Enumerable.Any(this._scaffoldMessenger!._materialBanners) ? this._scaffoldMessenger!._materialBanners.Peek() : null);
        if ((!object.Equals(this._messengerMaterialBanner, messengerMaterialBanner)))
        {
            setState(((global::System.Action)(() =>
            {
                _messengerMaterialBanner = messengerMaterialBanner;
            })));
        }
    }

    internal virtual void _maybeBuildPersistentBottomSheet()
    {
        if (((((Scaffold)(object)this.widget).bottomSheet is not null) && (this._currentBottomSheet is null)))
        {
            global::Doroti.Framework.Animation.AnimationController animationControllerLocal = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = BottomSheet.createAnimationController(this);
    __cascade.value = 1.0;
    return __cascade;
}))();
            bool persistentBottomSheetExtentChanged(global::Doroti.Framework.Widgets.DraggableScrollableNotification notification)
            {
                if (((((global::Doroti.Framework.Widgets.DraggableScrollableNotification)notification).extent - ((global::Doroti.Framework.Widgets.DraggableScrollableNotification)notification).initialExtent) > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
                {
                    if ((this._persistentSheetHistoryEntry is null))
                    {
                        _persistentSheetHistoryEntry = new global::Doroti.Framework.Widgets.LocalHistoryEntry(onRemove: ((global::System.Action)(() =>
                        {
                            DraggableScrollableActuator.reset(((global::Doroti.Framework.Widgets.DraggableScrollableNotification)notification).context);
                            showBodyScrim(false, 0.0);
                            this._floatingActionButtonVisibilityController.value = 1.0;
                            _persistentSheetHistoryEntry = null;
                        })));
                        ((dynamic)global::Doroti.Framework.Widgets.ModalRoute<object>.of<object>(this.context)!).addLocalHistoryEntry(this._persistentSheetHistoryEntry!);
                    }
                }
                else
                {
                    if ((this._persistentSheetHistoryEntry is not null))
                    {
                        this._persistentSheetHistoryEntry!.remove();
                    }
                }
                return false;
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            if (System.Linq.Enumerable.Any(this._dismissedBottomSheets))
            {
                var sheets = new List<_StandardBottomSheet__scaffold>(this._dismissedBottomSheets);
                foreach (var sheet in sheets)
                {
                    ((_StandardBottomSheet__scaffold)sheet).animationController.reset();
                }
                DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._dismissedBottomSheets));
            }
            _currentBottomSheet = _buildBottomSheet(((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.DraggableScrollableNotification>(onNotification: (global::System.Func<global::Doroti.Framework.Widgets.DraggableScrollableNotification, bool>)persistentBottomSheetExtentChanged, child: new global::Doroti.Framework.Widgets.DraggableScrollableActuator(child: new global::Doroti.Framework.Widgets.StatefulBuilder(key: this._currentBottomSheetKey, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::System.Action<global::System.Action>, global::Doroti.Framework.Widgets.Widget>)((context, setState) =>
                {
                    return (((Scaffold)(object)this.widget).bottomSheet ?? global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }))))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })), isPersistent: true, animationController: animationControllerLocal);
        }
    }

    internal virtual void _closeCurrentBottomSheet()
    {
        if ((this._currentBottomSheet is not null))
        {
            if (!this._currentBottomSheet!._isLocalHistoryEntry)
            {
                this._currentBottomSheet!.close();
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    DartRuntimePrimitives.Ignore(this._currentBottomSheet?._completer.future.whenComplete((() =>
                    {
                        DartRuntimePrimitives.Assert(() => (this._currentBottomSheet is null));
                    })));
                    return true;
                });
        }
    }

    public virtual void closeDrawer()
    {
        if ((this.hasDrawer && this.isDrawerOpen))
        {
            ((global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState>)this._drawerKey).currentState!.close();
        }
    }

    public virtual void closeEndDrawer()
    {
        if ((this.hasEndDrawer && this.isEndDrawerOpen))
        {
            ((global::Doroti.Framework.Widgets.GlobalKey<DrawerControllerState>)this._endDrawerKey).currentState!.close();
        }
    }

    internal virtual void _updatePersistentBottomSheet()
    {
        ((global::Doroti.Framework.Widgets.GlobalKey<IState>)this._currentBottomSheetKey).currentState!.setState(((global::System.Action)(() =>
        {
        })));
    }

    internal virtual PersistentBottomSheetController _buildBottomSheet(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, bool isPersistent, global::Doroti.Framework.Animation.AnimationController animationController, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? clipBehavior = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, bool? enableDrag = null, bool? showDragHandle = null, bool shouldDisposeAnimationController = true)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((((Scaffold)(object)this.widget).bottomSheet is not null) && isPersistent) && (this._currentBottomSheet is not null)))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Scaffold.bottomSheet cannot be specified while a bottom sheet " + "displayed with showBottomSheet() is still visible.\n" + "Rebuild the Scaffold with a null bottomSheet before calling showBottomSheet()."));
                }
                return true;
            });
        var completer = new Completer<object?>();
        var bottomSheetKey = global::Doroti.Framework.Widgets.GlobalKey<_StandardBottomSheetState__scaffold>.Create();
        _StandardBottomSheet__scaffold bottomSheetLocal = default!;
        var removedEntry = false;
        var doingDispose = false;
        void removePersistentSheetHistoryEntryIfNeeded()
        {
            DartRuntimePrimitives.Assert(() => isPersistent);
            if ((this._persistentSheetHistoryEntry is not null))
            {
                this._persistentSheetHistoryEntry!.remove();
                _persistentSheetHistoryEntry = null;
            }
        }
        void removeCurrentBottomSheet()
        {
            removedEntry = true;
            if ((this._currentBottomSheet is null))
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => (object.Equals(this._currentBottomSheet!._widget, bottomSheetLocal)));
            DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Widgets.GlobalKey<_StandardBottomSheetState__scaffold>)bottomSheetKey).currentState is not null));
            _showFloatingActionButton();
            if (isPersistent)
            {
                removePersistentSheetHistoryEntryIfNeeded();
            }
            ((global::Doroti.Framework.Widgets.GlobalKey<_StandardBottomSheetState__scaffold>)bottomSheetKey).currentState!.close();
            setState(((global::System.Action)(() =>
            {
                _showBodyScrim = false;
                this._bottomSheetScrimAnimationController.value = 0.0;
                _currentBottomSheet = null;
            })));
            if (!animationController.isDismissed)
            {
                this._dismissedBottomSheets.Add(bottomSheetLocal);
            }
            completer.complete();
        }
        global::Doroti.Framework.Widgets.LocalHistoryEntry? entry = (isPersistent ? null : new global::Doroti.Framework.Widgets.LocalHistoryEntry(onRemove: ((global::System.Action)(() =>
        {
            if (((!removedEntry && (object.Equals(this._currentBottomSheet?._widget, bottomSheetLocal))) && !doingDispose))
            {
                removeCurrentBottomSheet();
            }
        }))));
        void removeEntryIfNeeded()
        {
            if ((!isPersistent && !removedEntry))
            {
                DartRuntimePrimitives.Assert(() => (entry is not null));
                entry!.remove();
                removedEntry = true;
            }
        }
        bottomSheetLocal = new _StandardBottomSheet__scaffold(key: bottomSheetKey, animationController: animationController, enableDrag: (enableDrag ?? !isPersistent), showDragHandle: showDragHandle, onClosing: ((global::System.Action)(() =>
        {
            if ((this._currentBottomSheet is null))
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => (object.Equals(this._currentBottomSheet!._widget, bottomSheetLocal)));
            removeEntryIfNeeded();
        })), onDismissed: ((global::System.Action)(() =>
        {
            if (this._dismissedBottomSheets.Contains(bottomSheetLocal))
            {
                setState(((global::System.Action)(() =>
                {
                    this._dismissedBottomSheets.Remove(bottomSheetLocal);
                })));
            }
        })), onDispose: ((global::System.Action)(() =>
        {
            doingDispose = true;
            removeEntryIfNeeded();
            if (shouldDisposeAnimationController)
            {
                animationController.dispose();
            }
        })), builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)builder, isPersistent: isPersistent, backgroundColor: backgroundColor, elevation: elevation, shape: shape, clipBehavior: clipBehavior, constraints: constraints);
        if (!isPersistent)
        {
            ((dynamic)global::Doroti.Framework.Widgets.ModalRoute<object>.of<object>(this.context)!).addLocalHistoryEntry(entry!);
        }
        return new PersistentBottomSheetController(bottomSheetLocal, completer, ((global::System.Action)((entry is not null) ? ((global::Doroti.Framework.Widgets.LocalHistoryEntry)entry).remove : removeCurrentBottomSheet)), ((global::System.Action<global::System.Action>)((fn) =>
        {
            ((global::Doroti.Framework.Widgets.GlobalKey<_StandardBottomSheetState__scaffold>)bottomSheetKey).currentState?.setState(() => fn());
        })), !isPersistent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual PersistentBottomSheetController showBottomSheet(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? clipBehavior = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, bool? enableDrag = null, bool? showDragHandle = null, global::Doroti.Framework.Animation.AnimationController? transitionAnimationController = null, global::Doroti.Framework.Animation.AnimationStyle? sheetAnimationStyle = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((Scaffold)(object)this.widget).bottomSheet is not null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Scaffold.bottomSheet cannot be specified while a bottom sheet " + "displayed with showBottomSheet() is still visible.\n" + "Rebuild the Scaffold with a null bottomSheet before calling showBottomSheet()."));
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(this.context));
        _closeCurrentBottomSheet();
        global::Doroti.Framework.Animation.AnimationController controller = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = ((transitionAnimationController ?? BottomSheet.createAnimationController(this, sheetAnimationStyle: sheetAnimationStyle)));
    __cascade.forward();
    return __cascade;
}))();
        setState(((global::System.Action)(() =>
        {
            _currentBottomSheet = _buildBottomSheet((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)builder, isPersistent: false, animationController: controller, backgroundColor: backgroundColor, elevation: elevation, shape: shape, clipBehavior: clipBehavior, constraints: constraints, enableDrag: enableDrag, showDragHandle: showDragHandle, shouldDisposeAnimationController: (transitionAnimationController is null));
        })));
        return this._currentBottomSheet!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Scheduler.TickerFuture _showFloatingActionButton()
    {
        return ((global::Doroti.Framework.Scheduler.TickerFuture)(object?)this._floatingActionButtonVisibilityController.forward());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _moveFloatingActionButton(FloatingActionButtonLocation newLocation)
    {
        FloatingActionButtonLocation? previousLocation = this._floatingActionButtonLocation;
        var restartAnimationFrom = 0.0;
        if (((global::Doroti.Framework.Animation.AnimationController)this._floatingActionButtonMoveController).isAnimating)
        {
            previousLocation = DartRuntimePrimitives.ConvertValue<FloatingActionButtonLocation>(new _TransitionSnapshotFabLocation__scaffold(this._previousFloatingActionButtonLocation!, this._floatingActionButtonLocation!, this._floatingActionButtonAnimator, ((global::Doroti.Framework.Animation.AnimationController)this._floatingActionButtonMoveController).value));
            restartAnimationFrom = this._floatingActionButtonAnimator.getAnimationRestart(((global::Doroti.Framework.Animation.AnimationController)this._floatingActionButtonMoveController).value);
        }
        setState(((global::System.Action)(() =>
        {
            _previousFloatingActionButtonLocation = previousLocation;
            _floatingActionButtonLocation = newLocation;
        })));
        this._floatingActionButtonMoveController.forward(from: restartAnimationFrom);
    }

    public virtual void handleStatusBarTap()
    {
        DartRuntimePrimitives.Assert(() => ((Scaffold)(object)this.widget).primary);
        global::Doroti.Framework.Widgets.ScrollController? primaryScrollController = ((global::Doroti.Framework.Widgets.ScrollController?)(object?)PrimaryScrollController.maybeOf(this.context));
        if ((((primaryScrollController is not null) && ((global::Doroti.Framework.Widgets.ScrollController)primaryScrollController).hasClients) && _HitTestableAtOrigin__scaffold.hitTestableAtOrigin(this._statusBarKey)))
        {
            DartRuntimePrimitives.Ignore(primaryScrollController.animateTo(0.0, duration: Duration.Create(milliseconds: 1000L), curve: global::Doroti.Framework.Animation.Curves.easeOutCirc));
        }
    }

    internal virtual bool _resizeToAvoidBottomInset
    {
        get
        {
            return (((Scaffold)(object)this.widget).resizeToAvoidBottomInset ?? true);
            return default!;
        }
    }
    public override void initState()
    {
        base.initState();
        _geometryNotifier = new _ScaffoldGeometryNotifier__scaffold(new ScaffoldGeometry(), this.context);
        _floatingActionButtonLocation = (((Scaffold)(object)this.widget).floatingActionButtonLocation ?? ScaffoldLibrary._kDefaultFloatingActionButtonLocation);
        _floatingActionButtonAnimator = (((Scaffold)(object)this.widget).floatingActionButtonAnimator ?? ScaffoldLibrary._kDefaultFloatingActionButtonAnimator);
        _previousFloatingActionButtonLocation = this._floatingActionButtonLocation;
        _floatingActionButtonMoveController = new global::Doroti.Framework.Animation.AnimationController(vsync: this, value: 1.0, duration: (Floating_action_button_locationLibrary.kFloatingActionButtonSegue * 2L));
        _floatingActionButtonVisibilityController = new global::Doroti.Framework.Animation.AnimationController(duration: Floating_action_button_locationLibrary.kFloatingActionButtonSegue, vsync: this);
        _bottomSheetScrimAnimationController = new global::Doroti.Framework.Animation.AnimationController(vsync: this);
        if (((Scaffold)(object)this.widget).primary)
        {
            global::Doroti.Framework.Widgets.WidgetsBinding.instance.addObserver(this);
        }
    }

    public override void didUpdateWidget(Scaffold oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if ((!object.Equals(((Scaffold)(object)this.widget).floatingActionButtonAnimator, ((Scaffold)oldWidget).floatingActionButtonAnimator)))
        {
            _floatingActionButtonAnimator = (((Scaffold)(object)this.widget).floatingActionButtonAnimator ?? ScaffoldLibrary._kDefaultFloatingActionButtonAnimator);
        }
        if ((!object.Equals(((Scaffold)(object)this.widget).floatingActionButtonLocation, ((Scaffold)oldWidget).floatingActionButtonLocation)))
        {
            _moveFloatingActionButton((((Scaffold)(object)this.widget).floatingActionButtonLocation ?? ScaffoldLibrary._kDefaultFloatingActionButtonLocation));
        }
        if ((!object.Equals(((Scaffold)(object)this.widget).bottomSheet, ((Scaffold)oldWidget).bottomSheet)))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (((((Scaffold)(object)this.widget).bottomSheet is not null) && ((this._currentBottomSheet?._isLocalHistoryEntry ?? false))))
                    {
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Scaffold.bottomSheet cannot be specified while a bottom sheet displayed " + "with showBottomSheet() is still visible."), new global::Doroti.Framework.Foundation.ErrorHint("Use the PersistentBottomSheetController " + "returned by showBottomSheet() to close the old bottom sheet before creating " + "a Scaffold with a (non null) bottomSheet.") }));
                    }
                    return true;
                });
            if ((((Scaffold)(object)this.widget).bottomSheet is null))
            {
                _closeCurrentBottomSheet();
            }
            else
            {
                if (((((Scaffold)(object)this.widget).bottomSheet is not null) && (((Scaffold)oldWidget).bottomSheet is null)))
                {
                    _maybeBuildPersistentBottomSheet();
                }
                else
                {
                    _updatePersistentBottomSheet();
                }
            }
        }
        switch ((((Scaffold)oldWidget).primary, ((Scaffold)(object)this.widget).primary))
        {
            case (true, false):
                {
                    global::Doroti.Framework.Widgets.WidgetsBinding.instance.removeObserver(this);
                    break;
                }
            case (false, true):
                {
                    global::Doroti.Framework.Widgets.WidgetsBinding.instance.addObserver(this);
                    break;
                }
            case (true, true) or (false, false):
                break;
        }
    }

    public override void didChangeDependencies()
    {
        ScaffoldMessengerState? currentScaffoldMessenger = ((ScaffoldMessengerState?)(object?)ScaffoldMessenger.maybeOf(this.context));
        if (((this._scaffoldMessenger is not null) && (((currentScaffoldMessenger is null) || (!object.Equals(this._scaffoldMessenger, currentScaffoldMessenger))))))
        {
            this._scaffoldMessenger?._unregister(this);
        }
        _scaffoldMessenger = currentScaffoldMessenger;
        this._scaffoldMessenger?._register(this);
        _maybeBuildPersistentBottomSheet();
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        _currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            oldBucket?.dispose();
        }
    }

    public override void deactivate()
    {
        global::Doroti.Framework.Widgets.WidgetsBinding.instance.removeObserver(this);
        base.deactivate();
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
        if (((Scaffold)(object)this.widget).primary)
        {
            global::Doroti.Framework.Widgets.WidgetsBinding.instance.addObserver(this);
        }
    }

    public override void dispose()
    {
        this._geometryNotifier.dispose();
        this._floatingActionButtonMoveController.dispose();
        this._floatingActionButtonVisibilityController.dispose();
        this._scaffoldMessenger?._unregister(this);
        this._drawerOpened.dispose();
        this._endDrawerOpened.dispose();
        this._bottomSheetScrimAnimationController.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) =>
        {
            if (!((dynamic)property)._disposed)
            {
                property.removeListener(listener);
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual void _addIfNonNull(List<global::Doroti.Framework.Widgets.LayoutId> children, global::Doroti.Framework.Widgets.Widget? child, object childId, bool removeLeftPadding, bool removeTopPadding, bool removeRightPadding, bool removeBottomPadding, bool removeBottomInset = false, bool maintainBottomViewPadding = false)
    {
        global::Doroti.Framework.Widgets.MediaQueryData dataLocal = ((global::Doroti.Framework.Widgets.MediaQueryData)(object?)MediaQuery.of(this.context).removePadding(removeLeft: removeLeftPadding, removeTop: removeTopPadding, removeRight: removeRightPadding, removeBottom: removeBottomPadding));
        if (removeBottomInset)
        {
            dataLocal = dataLocal.removeViewInsets(removeBottom: true);
        }
        if ((maintainBottomViewPadding && (((global::Doroti.Framework.Widgets.MediaQueryData)dataLocal).viewInsets.bottom != 0.0)))
        {
            dataLocal = dataLocal.copyWith(padding: ((global::Doroti.Framework.Widgets.MediaQueryData)dataLocal).padding.copyWith(bottom: ((global::Doroti.Framework.Widgets.MediaQueryData)dataLocal).viewPadding.bottom));
        }
        if ((child is not null))
        {
            children.Add(new global::Doroti.Framework.Widgets.LayoutId(id: childId, child: new global::Doroti.Framework.Widgets.MediaQuery(data: dataLocal, child: child)));
        }
    }

    internal virtual void _buildEndDrawer(List<global::Doroti.Framework.Widgets.LayoutId> children, TextDirection textDirection)
    {
        if ((((Scaffold)(object)this.widget).endDrawer is not null))
        {
            DartRuntimePrimitives.Assert(() => this.hasEndDrawer);
            _addIfNonNull(children, new DrawerController(key: (global::Doroti.Framework.Widgets.GlobalKey<IState>)(object)this._endDrawerKey, alignment: DrawerAlignment.end, drawerCallback: this._endDrawerOpenedCallback, dragStartBehavior: ((Scaffold)(object)this.widget).drawerDragStartBehavior, scrimColor: ((Scaffold)(object)this.widget).drawerScrimColor, edgeDragWidth: ((Scaffold)(object)this.widget).drawerEdgeDragWidth, enableOpenDragGesture: ((Scaffold)(object)this.widget).endDrawerEnableOpenDragGesture, isDrawerOpen: this._endDrawerOpened.value, drawerBarrierDismissible: ((Scaffold)(object)this.widget).drawerBarrierDismissible, child: ((Scaffold)(object)this.widget).endDrawer!), _ScaffoldSlot__scaffold.endDrawer, removeLeftPadding: (object.Equals(textDirection, TextDirection.ltr)), removeTopPadding: false, removeRightPadding: (object.Equals(textDirection, TextDirection.rtl)), removeBottomPadding: false);
        }
    }

    internal virtual void _buildDrawer(List<global::Doroti.Framework.Widgets.LayoutId> children, TextDirection textDirection)
    {
        if ((((Scaffold)(object)this.widget).drawer is not null))
        {
            DartRuntimePrimitives.Assert(() => this.hasDrawer);
            _addIfNonNull(children, new DrawerController(key: (global::Doroti.Framework.Widgets.GlobalKey<IState>)(object)this._drawerKey, alignment: DrawerAlignment.start, drawerCallback: this._drawerOpenedCallback, dragStartBehavior: ((Scaffold)(object)this.widget).drawerDragStartBehavior, scrimColor: ((Scaffold)(object)this.widget).drawerScrimColor, edgeDragWidth: ((Scaffold)(object)this.widget).drawerEdgeDragWidth, enableOpenDragGesture: ((Scaffold)(object)this.widget).drawerEnableOpenDragGesture, isDrawerOpen: this._drawerOpened.value, drawerBarrierDismissible: ((Scaffold)(object)this.widget).drawerBarrierDismissible, child: ((Scaffold)(object)this.widget).drawer!), _ScaffoldSlot__scaffold.drawer, removeLeftPadding: (object.Equals(textDirection, TextDirection.rtl)), removeTopPadding: false, removeRightPadding: (object.Equals(textDirection, TextDirection.ltr)), removeBottomPadding: false);
        }
    }

    public virtual void showBodyScrim(bool value, double animationValue)
    {
        if ((this._showBodyScrim != value))
        {
            setState(((global::System.Action)(() =>
            {
                _showBodyScrim = value;
            })));
        }
        if ((((global::Doroti.Framework.Animation.AnimationController)this._bottomSheetScrimAnimationController).value != animationValue))
        {
            this._bottomSheetScrimAnimationController.value = animationValue;
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        ThemeData themeData = Theme.of(context);
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        var childrenLocal = new List<global::Doroti.Framework.Widgets.LayoutId>();
        _addIfNonNull(childrenLocal, ((((Scaffold)(object)this.widget).body is null) ? null : new _BodyBuilder__scaffold(extendBody: ((Scaffold)(object)this.widget).extendBody, extendBodyBehindAppBar: ((Scaffold)(object)this.widget).extendBodyBehindAppBar, body: new global::Doroti.Framework.Widgets.KeyedSubtree(key: this._bodyKey, child: ((Scaffold)(object)this.widget).body!))), _ScaffoldSlot__scaffold.body, removeLeftPadding: false, removeTopPadding: (((Scaffold)(object)this.widget).appBar is not null), removeRightPadding: false, removeBottomPadding: ((((Scaffold)(object)this.widget).bottomNavigationBar is not null) || (((Scaffold)(object)this.widget).persistentFooterButtons is not null)), removeBottomInset: this._resizeToAvoidBottomInset);
        if (this._showBodyScrim)
        {
            _addIfNonNull(childrenLocal, this.widget.bottomSheetScrimBuilder(context, ((global::Doroti.Framework.Animation.AnimationController)this._bottomSheetScrimAnimationController).view), _ScaffoldSlot__scaffold.bodyScrim, removeLeftPadding: true, removeTopPadding: true, removeRightPadding: true, removeBottomPadding: true);
        }
        if ((((Scaffold)(object)this.widget).appBar is not null))
        {
            double topPadding = (((Scaffold)(object)this.widget).primary ? MediaQuery.paddingOf(context).top : 0.0);
            _appBarMaxHeight = (AppBar.preferredHeightFor(context, ((Scaffold)(object)this.widget).appBar!.preferredSize) + topPadding);
            DartRuntimePrimitives.Assert(() => ((DartRuntimePrimitives.RequireValue(this._appBarMaxHeight) >= 0.0) && double.IsFinite(DartRuntimePrimitives.RequireValue(this._appBarMaxHeight))));
            _addIfNonNull(childrenLocal, new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxHeight: DartRuntimePrimitives.RequireValue(this._appBarMaxHeight)), child: FlexibleSpaceBar.createSettings(currentExtent: DartRuntimePrimitives.RequireValue(this._appBarMaxHeight), child: (global::Doroti.Framework.Widgets.Widget)(object)((Scaffold)(object)this.widget).appBar!)), _ScaffoldSlot__scaffold.appBar, removeLeftPadding: false, removeTopPadding: false, removeRightPadding: false, removeBottomPadding: true);
        }
        var isSnackBarFloatingLocal = false;
        double? snackBarWidthLocal = default!;
        if (((this._currentBottomSheet is not null) || System.Linq.Enumerable.Any(this._dismissedBottomSheets)))
        {
            global::Doroti.Framework.Widgets.Widget stack = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(alignment: global::Doroti.Framework.Painting.Alignment.bottomCenter, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection121351 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection121351.AddRange(this._dismissedBottomSheets); var __collectionElement121387 = this._currentBottomSheet?._widget; if (__collectionElement121387 is { } __nonNullCollectionElement121387) { __collection121351.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement121387)); } return __collection121351; }))()));
            _addIfNonNull(childrenLocal, stack, _ScaffoldSlot__scaffold.bottomSheet, removeLeftPadding: false, removeTopPadding: true, removeRightPadding: false, removeBottomPadding: this._resizeToAvoidBottomInset);
        }
        if ((this._messengerSnackBar is not null))
        {
            SnackBarThemeData snackBarTheme = SnackBarTheme.of(context);
            SnackBarBehavior snackBarBehavior = ((this._messengerSnackBar?._widget.behavior ?? snackBarTheme.behavior) ?? SnackBarBehavior.@fixed);
            isSnackBarFloatingLocal = (object.Equals(snackBarBehavior, SnackBarBehavior.floating));
            snackBarWidthLocal = (this._messengerSnackBar?._widget.width ?? snackBarTheme.width);
            _addIfNonNull(childrenLocal, this._messengerSnackBar?._widget, _ScaffoldSlot__scaffold.snackBar, removeLeftPadding: false, removeTopPadding: true, removeRightPadding: false, removeBottomPadding: ((((Scaffold)(object)this.widget).bottomNavigationBar is not null) || (((Scaffold)(object)this.widget).persistentFooterButtons is not null)), maintainBottomViewPadding: !this._resizeToAvoidBottomInset);
        }
        var extendBodyBehindMaterialBannerLocal = false;
        if ((this._messengerMaterialBanner is not null))
        {
            MaterialBannerThemeData bannerTheme = MaterialBannerTheme.of(context);
            double elevationLocal = ((this._messengerMaterialBanner?._widget.elevation ?? bannerTheme.elevation) ?? 0.0);
            extendBodyBehindMaterialBannerLocal = (DartRuntimePrimitives.RequireValue(elevationLocal) != 0.0);
            _addIfNonNull(childrenLocal, this._messengerMaterialBanner?._widget, _ScaffoldSlot__scaffold.materialBanner, removeLeftPadding: false, removeTopPadding: (((Scaffold)(object)this.widget).appBar is not null), removeRightPadding: false, removeBottomPadding: true, maintainBottomViewPadding: !this._resizeToAvoidBottomInset);
        }
        if ((((Scaffold)(object)this.widget).persistentFooterButtons is not null))
        {
            _addIfNonNull(childrenLocal, new global::Doroti.Framework.Widgets.Container(decoration: (((Scaffold)(object)this.widget).persistentFooterDecoration ?? new global::Doroti.Framework.Painting.BoxDecoration(border: new global::Doroti.Framework.Painting.Border(top: Divider.createBorderSide(context, width: 1.0)))), child: new global::Doroti.Framework.Widgets.SafeArea(top: false, child: new global::Doroti.Framework.Widgets.IntrinsicHeight(child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateAll(8), child: new global::Doroti.Framework.Widgets.Align(alignment: ((Scaffold)(object)this.widget).persistentFooterAlignment, child: new global::Doroti.Framework.Widgets.OverflowBar(spacing: 8, overflowAlignment: global::Doroti.Framework.Widgets.OverflowBarAlignment.end, children: ((Scaffold)(object)this.widget).persistentFooterButtons!)))))), _ScaffoldSlot__scaffold.persistentFooter, removeLeftPadding: false, removeTopPadding: true, removeRightPadding: false, removeBottomPadding: (((Scaffold)(object)this.widget).bottomNavigationBar is not null), maintainBottomViewPadding: !this._resizeToAvoidBottomInset);
        }
        if ((((Scaffold)(object)this.widget).bottomNavigationBar is not null))
        {
            _addIfNonNull(childrenLocal, ((Scaffold)(object)this.widget).bottomNavigationBar, _ScaffoldSlot__scaffold.bottomNavigationBar, removeLeftPadding: false, removeTopPadding: true, removeRightPadding: false, removeBottomPadding: false, maintainBottomViewPadding: !this._resizeToAvoidBottomInset);
        }
        _addIfNonNull(childrenLocal, new _FloatingActionButtonTransition__scaffold(fabMoveAnimation: this._floatingActionButtonMoveController, fabMotionAnimator: this._floatingActionButtonAnimator, geometryNotifier: this._geometryNotifier, currentController: this._floatingActionButtonVisibilityController, child: ((Scaffold)(object)this.widget).floatingActionButton), _ScaffoldSlot__scaffold.floatingActionButton, removeLeftPadding: true, removeTopPadding: true, removeRightPadding: true, removeBottomPadding: true);
        global::Doroti.Framework.Widgets.Widget? statusBarLocal = ((global::Doroti.Framework.Widgets.Widget?)(object?)(themeData.platform switch { global::Doroti.Framework.Foundation.TargetPlatform.iOS => (((Scaffold)(object)this.widget).primary ? new _HitTestableAtOrigin__scaffold(this._statusBarKey) : null), global::Doroti.Framework.Foundation.TargetPlatform.macOS => (((Scaffold)(object)this.widget).primary ? new _HitTestableAtOrigin__scaffold(this._statusBarKey) : null), global::Doroti.Framework.Foundation.TargetPlatform.android or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Framework.Foundation.TargetPlatform.linux => DartRuntimePrimitives.ConvertValue<_HitTestableAtOrigin__scaffold>(null), global::Doroti.Framework.Foundation.TargetPlatform.windows => DartRuntimePrimitives.ConvertValue<_HitTestableAtOrigin__scaffold>(null), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        _addIfNonNull(childrenLocal, statusBarLocal, _ScaffoldSlot__scaffold.statusBar, removeLeftPadding: false, removeTopPadding: true, removeRightPadding: false, removeBottomPadding: true);
        if (this._endDrawerOpened.value)
        {
            _buildDrawer(childrenLocal, textDirectionLocal);
            _buildEndDrawer(childrenLocal, textDirectionLocal);
        }
        else
        {
            _buildEndDrawer(childrenLocal, textDirectionLocal);
            _buildDrawer(childrenLocal, textDirectionLocal);
        }
        global::Doroti.Framework.Painting.EdgeInsets minInsetsLocal = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)MediaQuery.paddingOf(context).copyWith(bottom: (this._resizeToAvoidBottomInset ? MediaQuery.viewInsetsOf(context).bottom : 0.0)));
        global::Doroti.Framework.Painting.EdgeInsets minViewPaddingLocal = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)MediaQuery.viewPaddingOf(context).copyWith(bottom: ((this._resizeToAvoidBottomInset && (MediaQuery.viewInsetsOf(context).bottom != 0.0)) ? 0.0 : null)));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _ScaffoldScope__scaffold(hasDrawer: this.hasDrawer, geometryNotifier: this._geometryNotifier, child: new global::Doroti.Framework.Widgets.ScrollNotificationObserver(child: new Material(color: (((Scaffold)(object)this.widget).backgroundColor ?? themeData.scaffoldBackgroundColor), child: new global::Doroti.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Actions(actions: new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.DismissIntent)] = new _DismissDrawerAction__scaffold(context) }, child: new global::Doroti.Framework.Widgets.CustomMultiChildLayout(@delegate: new _ScaffoldLayout__scaffold(extendBody: ((Scaffold)(object)this.widget).extendBody, extendBodyBehindAppBar: ((Scaffold)(object)this.widget).extendBodyBehindAppBar, minInsets: minInsetsLocal, minViewPadding: minViewPaddingLocal, currentFloatingActionButtonLocation: this._floatingActionButtonLocation!, floatingActionButtonMoveAnimation: this._floatingActionButtonMoveController, floatingActionButtonMotionAnimator: this._floatingActionButtonAnimator, geometryNotifier: this._geometryNotifier, previousFloatingActionButtonLocation: this._previousFloatingActionButtonLocation!, textDirection: textDirectionLocal, isSnackBarFloating: isSnackBarFloatingLocal, extendBodyBehindMaterialBanner: extendBodyBehindMaterialBannerLocal, snackBarWidth: snackBarWidthLocal), children: childrenLocal.Cast<global::Doroti.Framework.Widgets.Widget>().ToList())));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))))));
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
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue = (hasSerializedValue ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
        if (!((dynamic)property).isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if ((this.bucket is null))
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener((global::System.Action)listener);
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue);
        if (((!hasSerializedValue && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(dynamic property)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)property)._owner, this)));
        this._bucket?.remove<object?>(((dynamic)property)._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket is null)));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (this._firstRestorePending)
            {
                return true;
            }
            if ((this.restorationId is null))
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = ((global::Doroti.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent, this._currentParent)) && ((potentialNewParent?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = ((global::Doroti.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucketLocal)));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(dynamic property)
    {
        if (((dynamic)property).enabled)
        {
            this._bucket?.write(((dynamic)property)._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(((dynamic)property)._restorationId!);
        }
    }

    public virtual void _unregister(dynamic property)
    {
        global::System.Action listener = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
    }

}

internal class _DismissDrawerAction__scaffold : global::Doroti.Framework.Widgets.DismissAction
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _DismissDrawerAction__scaffold(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public override bool isEnabled(global::Doroti.Framework.Widgets.DismissIntent intent, global::Doroti.Framework.Widgets.BuildContext? context = null)
    {
        ScaffoldState scaffold = ((ScaffoldState)(object?)Scaffold.of(this.context));
        return (((((ScaffoldState)scaffold).isDrawerOpen || ((ScaffoldState)scaffold).isEndDrawerOpen)) && ((ScaffoldState)scaffold).isDrawerBarrierDismissible);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? invoke(global::Doroti.Framework.Widgets.DismissIntent intent, global::Doroti.Framework.Widgets.BuildContext? context = null)
    {
        ScaffoldState scaffold = ((ScaffoldState)(object?)Scaffold.of(this.context));
        if (isEnabled(intent))
        {
            scaffold.closeDrawer();
            scaffold.closeEndDrawer();
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ScaffoldFeatureController<T, U> where T : global::Doroti.Framework.Widgets.Widget
{
    internal virtual T _widget { get; private set; } = default!;
    internal virtual Completer<U> _completer { get; private set; } = default!;
    public virtual global::System.Action close { get; private set; } = default!;
    public virtual global::System.Action<global::System.Action>? setState { get; private set; }

    public ScaffoldFeatureController(T _widget, Completer<U> _completer, global::System.Action close, global::System.Action<global::System.Action>? setState)
    {
        this._widget = _widget;
        this._completer = _completer;
        this.close = close;
        this.setState = setState;
    }

    public virtual Future<U> closed => this._completer.future;
}

public class _StandardBottomSheet__scaffold : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Animation.AnimationController animationController { get; private set; } = default!;
    public virtual bool enableDrag { get; private set; } = default!;
    public virtual bool? showDragHandle { get; private set; }
    public virtual global::System.Action? onClosing { get; private set; }
    public virtual global::System.Action? onDismissed { get; private set; }
    public virtual global::System.Action? onDispose { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;
    public virtual bool isPersistent { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }

    internal _StandardBottomSheet__scaffold(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.AnimationController animationController = default!, bool enableDrag = true, bool? showDragHandle = null, global::System.Action? onClosing = default!, global::System.Action? onDismissed = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder = default!, bool isPersistent = false, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? clipBehavior = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::System.Action? onDispose = null) : base(key: key)
    {
        this.animationController = animationController;
        this.enableDrag = enableDrag;
        this.showDragHandle = showDragHandle;
        this.onClosing = onClosing;
        this.onDismissed = onDismissed;
        this.builder = builder;
        this.isPersistent = isPersistent;
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.constraints = constraints;
        this.onDispose = onDispose;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _StandardBottomSheetState__scaffold());
}

public class _StandardBottomSheetState__scaffold : global::Doroti.Framework.Widgets.State<_StandardBottomSheet__scaffold>
{
    public virtual global::Doroti.Framework.Animation.ParametricCurve<double> animationCurve { get; set; } = ((global::Doroti.Framework.Animation.ParametricCurve<double>)(object?)ScaffoldLibrary._standardBottomSheetCurve);

    public override void initState()
    {
        base.initState();
        DartRuntimePrimitives.Assert(() => ((_StandardBottomSheet__scaffold)(object)this.widget).animationController.isForwardOrCompleted);
        ((_StandardBottomSheet__scaffold)(object)this.widget).animationController.addStatusListener((AnimationStatusListener)this._handleStatusChange);
    }

    public override void dispose()
    {
        ((_StandardBottomSheet__scaffold)(object)this.widget).animationController.removeStatusListener((AnimationStatusListener)this._handleStatusChange);
        ((_StandardBottomSheet__scaffold)(object)this.widget).onDispose?.Invoke();
        base.dispose();
    }

    public override void didUpdateWidget(_StandardBottomSheet__scaffold oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(((_StandardBottomSheet__scaffold)(object)this.widget).animationController, ((_StandardBottomSheet__scaffold)oldWidget).animationController)));
    }

    public virtual void close()
    {
        ((_StandardBottomSheet__scaffold)(object)this.widget).animationController.reverse();
        ((_StandardBottomSheet__scaffold)(object)this.widget).onClosing?.Invoke();
    }

    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        animationCurve = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.ParametricCurve<double>>(global::Doroti.Framework.Animation.Curves.linear);
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details, bool? isClosing = null)
    {
        animationCurve = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.ParametricCurve<double>>(new global::Doroti.Framework.Animation.Split(((_StandardBottomSheet__scaffold)(object)this.widget).animationController.value, endCurve: ScaffoldLibrary._standardBottomSheetCurve));
    }

    internal virtual void _handleStatusChange(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (global::Doroti.Framework.Animation.AnimationStatusMembers.isDismissed(status))
        {
            ((_StandardBottomSheet__scaffold)(object)this.widget).onDismissed?.Invoke();
        }
    }

    public virtual bool extentChanged(global::Doroti.Framework.Widgets.DraggableScrollableNotification notification)
    {
        double extentRemaining = (1.0 - ((global::Doroti.Framework.Widgets.DraggableScrollableNotification)notification).extent);
        ScaffoldState scaffold = ((ScaffoldState)(object?)Scaffold.of(this.context));
        if ((extentRemaining < ScaffoldLibrary._kBottomSheetDominatesPercentage))
        {
            ((ScaffoldState)scaffold)._floatingActionButtonVisibilityController.value = ((extentRemaining * ScaffoldLibrary._kBottomSheetDominatesPercentage) * 10L);
            double scrimAnimationValue = (1L - (extentRemaining / ScaffoldLibrary._kBottomSheetDominatesPercentage));
            scaffold.showBodyScrim(true, scrimAnimationValue);
        }
        else
        {
            ((ScaffoldState)scaffold)._floatingActionButtonVisibilityController.value = 1.0;
            scaffold.showBodyScrim(false, 0.0);
        }
        if ((((((global::Doroti.Framework.Widgets.DraggableScrollableNotification)notification).extent == ((global::Doroti.Framework.Widgets.DraggableScrollableNotification)notification).minExtent) && (scaffold.widget.bottomSheet is null)) && ((global::Doroti.Framework.Widgets.DraggableScrollableNotification)notification).shouldCloseOnMinExtent))
        {
            close();
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: ((_StandardBottomSheet__scaffold)(object)this.widget).animationController, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.topStart, heightFactor: this.animationCurve.transform(((_StandardBottomSheet__scaffold)(object)this.widget).animationController.value), child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: new global::Doroti.Framework.Widgets.Semantics(container: true, onDismiss: ((global::System.Action)(!((_StandardBottomSheet__scaffold)(object)this.widget).isPersistent ? this.close : null)), child: new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.DraggableScrollableNotification>(onNotification: (global::System.Func<global::Doroti.Framework.Widgets.DraggableScrollableNotification, bool>)this.extentChanged, child: new BottomSheet(animationController: ((_StandardBottomSheet__scaffold)(object)this.widget).animationController, enableDrag: ((_StandardBottomSheet__scaffold)(object)this.widget).enableDrag, showDragHandle: ((_StandardBottomSheet__scaffold)(object)this.widget).showDragHandle, onDragStart: this._handleDragStart, onDragEnd: ((details, isClosing) => this._handleDragEnd(details, isClosing)), onClosing: ((_StandardBottomSheet__scaffold)(object)this.widget).onClosing!, builder: ((_StandardBottomSheet__scaffold)(object)this.widget).builder, backgroundColor: ((_StandardBottomSheet__scaffold)(object)this.widget).backgroundColor, elevation: ((_StandardBottomSheet__scaffold)(object)this.widget).elevation, shape: ((_StandardBottomSheet__scaffold)(object)this.widget).shape, clipBehavior: ((_StandardBottomSheet__scaffold)(object)this.widget).clipBehavior, constraints: ((_StandardBottomSheet__scaffold)(object)this.widget).constraints)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PersistentBottomSheetController : ScaffoldFeatureController<_StandardBottomSheet__scaffold, object?>
{
    internal virtual bool _isLocalHistoryEntry { get; private set; } = default!;

    internal PersistentBottomSheetController(_StandardBottomSheet__scaffold widget, Completer<object?> completer, global::System.Action close, global::System.Action<global::System.Action> setState, bool _isLocalHistoryEntry) : base(widget, completer, close, setState)
    {
        this._isLocalHistoryEntry = _isLocalHistoryEntry;
    }

}

internal class _ScaffoldScope__scaffold : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual bool hasDrawer { get; private set; } = default!;
    public virtual _ScaffoldGeometryNotifier__scaffold geometryNotifier { get; private set; } = default!;

    internal _ScaffoldScope__scaffold(bool hasDrawer, _ScaffoldGeometryNotifier__scaffold geometryNotifier, global::Doroti.Framework.Widgets.Widget child) : base(child: child)
    {
        this.hasDrawer = hasDrawer;
        this.geometryNotifier = geometryNotifier;
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_ScaffoldScope__scaffold)(object)oldWidget;
        return (this.hasDrawer != ((_ScaffoldScope__scaffold)__oldWidget).hasDrawer);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _HitTestableAtOrigin__scaffold : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> globalKey { get; private set; } = default!;

    internal _HitTestableAtOrigin__scaffold(global::Doroti.Framework.Widgets.GlobalKey<IState> globalKey)
    {
        this.globalKey = globalKey;
    }

    public static bool hitTestableAtOrigin(global::Doroti.Framework.Widgets.GlobalKey<IState> key)
    {
        var context = ((global::Doroti.Framework.Widgets.Element?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)key).currentContext)!;
        if ((context is null))
        {
            DartRuntimePrimitives.Assert(() => false, () => (object?)$"BuildContext associated with {key} is not mounted. " + "If you see this in a test, this is likely because the test was trying " + "to simulate status bar tap on a non-iOS platform");
            return false;
        }
        var renderObjectLocal = ((global::Doroti.Framework.Rendering.RenderMetaData?)(object?)((global::Doroti.Framework.Widgets.Element)context).renderObject!)!;
        long viewIdLocal = checked((long)View.of(context).viewId);
        var result = new global::Doroti.Framework.Gestures.HitTestResult();
        global::Doroti.Framework.Widgets.WidgetsBinding.instance.hitTestInView(result, Offset.zero, viewIdLocal);
        return ((global::Doroti.Framework.Gestures.HitTestResult)result).path.any(((entry) => (object.Equals(((global::Doroti.Framework.Gestures.HitTestEntry<global::Doroti.Framework.Gestures.HitTestTarget>)entry).target, renderObjectLocal))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.MetaData(key: this.globalKey, behavior: global::Doroti.Framework.Rendering.HitTestBehavior.translucent, child: global::Doroti.Framework.Widgets.SizedBox.CreateExpand()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
