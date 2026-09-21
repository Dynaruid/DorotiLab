// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/scaffold.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class ScaffoldLibrary
{
    internal static FloatingActionButtonLocation _kDefaultFloatingActionButtonLocation =
        FloatingActionButtonLocation.endFloat;
}

public static partial class ScaffoldLibrary
{
    internal static FloatingActionButtonAnimator _kDefaultFloatingActionButtonAnimator =
        FloatingActionButtonAnimator.scaling;
}

public static partial class ScaffoldLibrary
{
    internal static Curve _standardBottomSheetCurve = CurvesLibrary.standardEasing;
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
    statusBar,
}

public class ScaffoldMessenger : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;

    public ScaffoldMessenger(Key? key = null, Widget child = default!)
        : base(key: key)
    {
        this.child = child;
    }

    public static ScaffoldMessengerState of(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasScaffoldMessenger(context));
        _ScaffoldMessengerScope__scaffold scope =
            context.dependOnInheritedWidgetOfExactType<_ScaffoldMessengerScope__scaffold>()!;
        return scope._scaffoldMessengerState;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static ScaffoldMessengerState? maybeOf(BuildContext context)
    {
        _ScaffoldMessengerScope__scaffold? scope =
            context.dependOnInheritedWidgetOfExactType<_ScaffoldMessengerScope__scaffold>();
        return scope?._scaffoldMessengerState;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new ScaffoldMessengerState());
}

public class ScaffoldMessengerState
    : State<ScaffoldMessenger>,
        TickerProviderStateMixin<ScaffoldMessenger>
{
    internal virtual HashSet<ScaffoldState> _scaffolds { get; private set; } =
        new HashSet<ScaffoldState>();
    internal virtual Queue<
        ScaffoldFeatureController<MaterialBanner, MaterialBannerClosedReason>
    > _materialBanners { get; private set; } =
        new Queue<ScaffoldFeatureController<MaterialBanner, MaterialBannerClosedReason>>();
    internal virtual AnimationController? _materialBannerController { get; set; } = default;
    internal virtual Queue<ScaffoldFeatureController<SnackBar, SnackBarClosedReason>> _snackBars
    {
        get;
        private set;
    } = new Queue<ScaffoldFeatureController<SnackBar, SnackBarClosedReason>>();
    internal virtual AnimationController? _snackBarController { get; set; } = default;
    internal virtual Timer? _snackBarTimer { get; set; } = default;
    internal virtual bool _accessibleNavigation { get; set; } = default!;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void didChangeDependencies()
    {
        _accessibleNavigation = MediaQuery.accessibleNavigationOf(context);
        base.didChangeDependencies();
    }

    internal virtual void _register(ScaffoldState scaffold)
    {
        _scaffolds.add(scaffold);
        if (_isRoot(scaffold))
        {
            if (Enumerable.Any(_snackBars))
            {
                scaffold._updateSnackBar();
            }
            if (Enumerable.Any(_materialBanners))
            {
                scaffold._updateMaterialBanner();
            }
        }
    }

    internal virtual void _unregister(ScaffoldState scaffold)
    {
        bool removed = _scaffolds.remove(scaffold);
        DartRuntimePrimitives.Assert(() => removed);
    }

    internal virtual void _updateScaffolds()
    {
        foreach (ScaffoldState scaffold in _scaffolds)
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
        ScaffoldState? parent = scaffold.context.findAncestorStateOfType<ScaffoldState>();
        return (parent is null) || !_scaffolds.contains(parent);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual ScaffoldFeatureController<SnackBar, SnackBarClosedReason> showSnackBar(
        SnackBar snackBar,
        AnimationStyle? snackBarAnimationStyle = null
    )
    {
        DartRuntimePrimitives.Assert(
            () => Enumerable.Any(_scaffolds),
            () =>
                (object?)"ScaffoldMessenger.showSnackBar was called, but there are currently no "
                + "descendant Scaffolds to present to."
        );
        _didUpdateAnimationStyle(snackBarAnimationStyle);
        _snackBarController ??= (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = SnackBar.createAnimationController(
                        duration: snackBarAnimationStyle?.duration,
                        reverseDuration: snackBarAnimationStyle?.reverseDuration,
                        vsync: this
                    );
                    __cascade.addStatusListener(_handleSnackBarStatusChanged);
                    return __cascade;
                }
            )
        )();
        if (!Enumerable.Any(_snackBars))
        {
            DartRuntimePrimitives.Assert(() => _snackBarController!.isDismissed);
            _snackBarController!.forward();
        }
        ScaffoldFeatureController<SnackBar, SnackBarClosedReason> controller = default!;
        controller = new ScaffoldFeatureController<SnackBar, SnackBarClosedReason>(
            snackBar.withAnimation(_snackBarController!, fallbackKey: new UniqueKey()),
            new Completer<SnackBarClosedReason>(),
            () =>
            {
                DartRuntimePrimitives.Assert(() => Equals(_snackBars.Peek(), controller));
                hideCurrentSnackBar();
            },
            null
        );
        try
        {
            setState(() =>
            {
                _snackBars.addLast(controller);
            });
            _updateScaffolds();
        }
        catch (Exception exception)
        {
            DartRuntimePrimitives.Assert(() =>
            {
                if (exception is FlutterError)
                {
                    FlutterError exception__13918__as13961 = (FlutterError)exception;
                    string summary = exception__13918__as13961.diagnostics.toDescription();
                    if (summary == "setState() or markNeedsBuild() called during build.")
                    {
                        var information = new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "The showSnackBar() method cannot be called during build."
                            ),
                            new ErrorDescription(
                                "The showSnackBar() method was called during build, which is "
                                    + "prohibited as showing snack bars requires updating state. Updating "
                                    + "state is not possible during build."
                            ),
                            new ErrorHint(
                                "Instead of calling showSnackBar() during build, call it directly "
                                    + "in your on tap (and related) callbacks. If you need to immediately "
                                    + "show a snack bar, make the call in initState() or "
                                    + "didChangeDependencies() instead. Otherwise, you can also schedule a "
                                    + "post-frame callback using SchedulerBinding.addPostFrameCallback to "
                                    + "show the snack bar after the current frame."
                            ),
                            context.describeOwnershipChain(
                                "The ownership chain for the particular ScaffoldMessenger is"
                            ),
                        };
                        throw DartRuntimePrimitives.AsException(new FlutterError(information));
                    }
                }
                return true;
            });
            throw;
        }
        return controller;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _didUpdateAnimationStyle(AnimationStyle? snackBarAnimationStyle)
    {
        if (snackBarAnimationStyle is not null)
        {
            if (
                (!Equals(_snackBarController?.duration, snackBarAnimationStyle.duration))
                || (
                    !Equals(
                        _snackBarController?.reverseDuration,
                        snackBarAnimationStyle.reverseDuration
                    )
                )
            )
            {
                _snackBarController?.dispose();
                _snackBarController = null;
            }
        }
    }

    internal virtual void _handleSnackBarStatusChanged(AnimationStatus status)
    {
        switch (status)
        {
            case AnimationStatus.dismissed:
            {
                DartRuntimePrimitives.Assert(() => Enumerable.Any(_snackBars));
                setState(() =>
                {
                    _snackBars.Dequeue();
                });
                _updateScaffolds();
                if (Enumerable.Any(_snackBars))
                {
                    _snackBarController!.forward();
                }
                break;
            }
            case AnimationStatus.completed:
            {
                setState(() =>
                {
                    DartRuntimePrimitives.Assert(() => _snackBarTimer is null);
                });
                _updateScaffolds();
                break;
            }
            case AnimationStatus.forward:
            case AnimationStatus.reverse:
            {
                break;
            }
        }
    }

    public virtual void removeCurrentSnackBar(
        SnackBarClosedReason reason = SnackBarClosedReason.remove
    )
    {
        if (!Enumerable.Any(_snackBars))
        {
            return;
        }
        Completer<SnackBarClosedReason> completer = _snackBars.Peek()._completer;
        if (!completer.isCompleted)
        {
            completer.complete(reason);
        }
        _snackBarTimer?.cancel();
        _snackBarTimer = null;
        _snackBarController!.value = 0.0;
    }

    public virtual void hideCurrentSnackBar(SnackBarClosedReason reason = SnackBarClosedReason.hide)
    {
        if (!Enumerable.Any(_snackBars) || _snackBarController!.isDismissed)
        {
            return;
        }
        Completer<SnackBarClosedReason> completer = _snackBars.Peek()._completer;
        if (_accessibleNavigation)
        {
            _snackBarController!.value = 0.0;
            completer.complete(reason);
        }
        else
        {
            DartRuntimePrimitives.Ignore(
                _snackBarController!
                    .reverse()
                    .then(
                        (value) =>
                        {
                            DartRuntimePrimitives.Assert(() => mounted);
                            if (!completer.isCompleted)
                            {
                                completer.complete(reason);
                            }
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
                    )
            );
        }
        _snackBarTimer?.cancel();
        _snackBarTimer = null;
    }

    public virtual void clearSnackBars()
    {
        if (!Enumerable.Any(_snackBars) || _snackBarController!.isDismissed)
        {
            return;
        }
        ScaffoldFeatureController<SnackBar, SnackBarClosedReason> currentSnackbar =
            _snackBars.Peek();
        _snackBars.Clear();
        _snackBars.Enqueue(currentSnackbar);
        hideCurrentSnackBar();
    }

    public virtual ScaffoldFeatureController<
        MaterialBanner,
        MaterialBannerClosedReason
    > showMaterialBanner(MaterialBanner materialBanner)
    {
        DartRuntimePrimitives.Assert(
            () => Enumerable.Any(_scaffolds),
            () =>
                (object?)
                    "ScaffoldMessenger.showMaterialBanner was called, but there are currently no "
                + "descendant Scaffolds to present to."
        );
        _materialBannerController ??= (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = MaterialBanner.createAnimationController(vsync: this);
                    __cascade.addStatusListener(_handleMaterialBannerStatusChanged);
                    return __cascade;
                }
            )
        )();
        if (!Enumerable.Any(_materialBanners))
        {
            DartRuntimePrimitives.Assert(() => _materialBannerController!.isDismissed);
            _materialBannerController!.forward();
        }
        ScaffoldFeatureController<MaterialBanner, MaterialBannerClosedReason> controller = default!;
        controller = new ScaffoldFeatureController<MaterialBanner, MaterialBannerClosedReason>(
            materialBanner.withAnimation(_materialBannerController!, fallbackKey: new UniqueKey()),
            new Completer<MaterialBannerClosedReason>(),
            () =>
            {
                DartRuntimePrimitives.Assert(() => Equals(_materialBanners.Peek(), controller));
                hideCurrentMaterialBanner();
            },
            null
        );
        setState(() =>
        {
            _materialBanners.addLast(controller);
        });
        _updateScaffolds();
        return controller;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _handleMaterialBannerStatusChanged(AnimationStatus status)
    {
        switch (status)
        {
            case AnimationStatus.dismissed:
            {
                DartRuntimePrimitives.Assert(() => Enumerable.Any(_materialBanners));
                setState(() =>
                {
                    _materialBanners.Dequeue();
                });
                _updateScaffolds();
                if (Enumerable.Any(_materialBanners))
                {
                    _materialBannerController!.forward();
                }
                break;
            }
            case AnimationStatus.completed:
            {
                _updateScaffolds();
                break;
            }
            case AnimationStatus.forward:
            case AnimationStatus.reverse:
            {
                break;
            }
        }
    }

    public virtual void removeCurrentMaterialBanner(MaterialBannerClosedReason reason = default!)
    {
        if (!Enumerable.Any(_materialBanners))
        {
            return;
        }
        Completer<MaterialBannerClosedReason> completer = _materialBanners.Peek()._completer;
        if (!completer.isCompleted)
        {
            completer.complete(reason);
        }
        _materialBannerController!.value = 0.0;
    }

    public virtual void hideCurrentMaterialBanner(MaterialBannerClosedReason reason = default!)
    {
        if (!Enumerable.Any(_materialBanners) || _materialBannerController!.isDismissed)
        {
            return;
        }
        Completer<MaterialBannerClosedReason> completer = _materialBanners.Peek()._completer;
        if (_accessibleNavigation)
        {
            _materialBannerController!.value = 0.0;
            completer.complete(reason);
        }
        else
        {
            DartRuntimePrimitives.Ignore(
                _materialBannerController!
                    .reverse()
                    .then(
                        (value) =>
                        {
                            DartRuntimePrimitives.Assert(() => mounted);
                            if (!completer.isCompleted)
                            {
                                completer.complete(reason);
                            }
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
                    )
            );
        }
    }

    public virtual void clearMaterialBanners()
    {
        if (!Enumerable.Any(_materialBanners) || _materialBannerController!.isDismissed)
        {
            return;
        }
        ScaffoldFeatureController<
            MaterialBanner,
            MaterialBannerClosedReason
        > currentMaterialBanner = _materialBanners.Peek();
        _materialBanners.Clear();
        _materialBanners.Enqueue(currentMaterialBanner);
        hideCurrentMaterialBanner();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        _accessibleNavigation = MediaQuery.accessibleNavigationOf(context);
        if (Enumerable.Any(_snackBars))
        {
            IModalRoute? route = ModalRoute<object>.untypedOf(context);
            if ((route is null) || route.isCurrent)
            {
                if (_snackBarController!.isCompleted && (_snackBarTimer is null))
                {
                    SnackBar snackBar = _snackBars.Peek()._widget;
                    _snackBarTimer = new Timer(
                        snackBar.duration,
                        () =>
                        {
                            DartRuntimePrimitives.Assert(() =>
                                _snackBarController!.isForwardOrCompleted
                            );
                            if (snackBar.persist)
                            {
                                return;
                            }
                            hideCurrentSnackBar(reason: SnackBarClosedReason.timeout);
                        }
                    );
                }
            }
        }
        return new _ScaffoldMessengerScope__scaffold(
            scaffoldMessengerState: this,
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        _materialBannerController?.dispose();
        _snackBarController?.dispose();
        _snackBarTimer?.cancel();
        _snackBarTimer = null;
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

internal class _ScaffoldMessengerScope__scaffold : InheritedWidget
{
    internal virtual ScaffoldMessengerState _scaffoldMessengerState { get; private set; } =
        default!;

    internal _ScaffoldMessengerScope__scaffold(
        Widget child,
        ScaffoldMessengerState scaffoldMessengerState
    )
        : base(child: child)
    {
        _scaffoldMessengerState = scaffoldMessengerState;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        !Equals(
            _scaffoldMessengerState,
            ((_ScaffoldMessengerScope__scaffold)oldWidget)._scaffoldMessengerState
        );
}

public class ScaffoldPrelayoutGeometry
{
    public virtual Size floatingActionButtonSize { get; private set; } = default!;
    public virtual Size bottomSheetSize { get; private set; } = default!;
    public virtual double contentBottom { get; private set; } = default!;
    public virtual double contentTop { get; private set; } = default!;
    public virtual EdgeInsets minInsets { get; private set; } = default!;
    public virtual EdgeInsets minViewPadding { get; private set; } = default!;
    public virtual Size scaffoldSize { get; private set; } = default!;
    public virtual Size snackBarSize { get; private set; } = default!;
    public virtual Size materialBannerSize { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    public ScaffoldPrelayoutGeometry(
        Size bottomSheetSize,
        double contentBottom,
        double contentTop,
        Size floatingActionButtonSize,
        EdgeInsets minInsets,
        EdgeInsets minViewPadding,
        Size scaffoldSize,
        Size snackBarSize,
        Size materialBannerSize,
        TextDirection textDirection
    )
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

    internal _TransitionSnapshotFabLocation__scaffold(
        FloatingActionButtonLocation begin,
        FloatingActionButtonLocation end,
        FloatingActionButtonAnimator animator,
        double progress
    )
    {
        this.begin = begin;
        this.end = end;
        this.animator = animator;
        this.progress = progress;
    }

    public override Offset getOffset(ScaffoldPrelayoutGeometry scaffoldGeometry)
    {
        return animator.getOffset(
            begin: begin.getOffset(scaffoldGeometry),
            end: end.getOffset(scaffoldGeometry),
            progress: progress
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "_TransitionSnapshotFabLocation")}(begin: {begin}, end: {end}, progress: {progress})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class ScaffoldGeometry
{
    public virtual double? bottomNavigationBarTop { get; private set; }
    public virtual Rect? floatingActionButtonArea { get; private set; }

    public ScaffoldGeometry(
        double? bottomNavigationBarTop = null,
        Rect? floatingActionButtonArea = null
    )
    {
        this.bottomNavigationBarTop = bottomNavigationBarTop;
        this.floatingActionButtonArea = floatingActionButtonArea;
    }

    internal virtual ScaffoldGeometry _scaleFloatingActionButton(double scaleFactor)
    {
        if (scaleFactor == 1.0)
        {
            return this;
        }
        if (scaleFactor == 0.0)
        {
            return new ScaffoldGeometry(bottomNavigationBarTop: bottomNavigationBarTop);
        }
        Rect scaledButton = (
            DorotiUiLibrary.Rect.lerp(
                (
                    floatingActionButtonArea
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).center & Size.zero,
                floatingActionButtonArea,
                scaleFactor
            ) ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        return copyWith(floatingActionButtonArea: scaledButton);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual ScaffoldGeometry copyWith(
        double? bottomNavigationBarTop = null,
        Rect? floatingActionButtonArea = null
    )
    {
        return new ScaffoldGeometry(
            bottomNavigationBarTop: bottomNavigationBarTop ?? this.bottomNavigationBarTop,
            floatingActionButtonArea: floatingActionButtonArea ?? this.floatingActionButtonArea
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class _ScaffoldGeometryNotifier__scaffold : ChangeNotifier, ValueListenable<ScaffoldGeometry>
{
    public virtual BuildContext context { get; private set; } = default!;
    public virtual double? floatingActionButtonScale { get; set; } = default;
    public virtual ScaffoldGeometry geometry { get; set; } = default!;

    internal _ScaffoldGeometryNotifier__scaffold(ScaffoldGeometry geometry, BuildContext context)
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
                RenderObject? renderObject = context.findRenderObject();
                if ((renderObject is null) || !renderObject.owner!.debugDoingPaint)
                {
                    throw DartRuntimePrimitives.AsException(
                        FlutterError.Create(
                            "Scaffold.geometryOf() must only be accessed during the paint phase.\n"
                                + "The ScaffoldGeometry is only available during the paint phase, because "
                                + "its value is computed during the animation and layout phases prior to painting."
                        )
                    );
                }
                return true;
            });
            return geometry._scaleFloatingActionButton(
                (
                    floatingActionButtonScale
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        }
    }

    internal virtual void _updateWith(
        double? bottomNavigationBarTop = null,
        Rect? floatingActionButtonArea = null,
        double? floatingActionButtonScale = null
    )
    {
        this.floatingActionButtonScale =
            floatingActionButtonScale ?? this.floatingActionButtonScale;
        geometry = geometry.copyWith(
            bottomNavigationBarTop: bottomNavigationBarTop,
            floatingActionButtonArea: floatingActionButtonArea
        );
        notifyListeners();
    }
}

internal class _BodyBoxConstraints__scaffold : BoxConstraints
{
    public virtual double bottomWidgetsHeight { get; private set; } = default!;
    public virtual double appBarHeight { get; private set; } = default!;
    public virtual double materialBannerHeight { get; private set; } = default!;

    internal _BodyBoxConstraints__scaffold(
        double maxWidth = double.PositiveInfinity,
        double maxHeight = double.PositiveInfinity,
        double bottomWidgetsHeight = default!,
        double appBarHeight = default!,
        double materialBannerHeight = default!
    )
        : base(maxWidth: maxWidth, maxHeight: maxHeight)
    {
        this.bottomWidgetsHeight = bottomWidgetsHeight;
        this.appBarHeight = appBarHeight;
        this.materialBannerHeight = materialBannerHeight;
        System.Diagnostics.Debug.Assert(bottomWidgetsHeight >= 0L);
        System.Diagnostics.Debug.Assert(appBarHeight >= 0L);
        System.Diagnostics.Debug.Assert(materialBannerHeight >= 0L);
    }

    public override bool Equals(object? other)
    {
        var __other = other as _BodyBoxConstraints__scaffold;
        if (__other is null)
        {
            return false;
        }

        if (!base.Equals(__other))
        {
            return false;
        }
        return (__other is _BodyBoxConstraints__scaffold)
            && (__other.materialBannerHeight == materialBannerHeight)
            && (__other.bottomWidgetsHeight == bottomWidgetsHeight)
            && (__other.appBarHeight == appBarHeight);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                base.GetHashCode(),
                materialBannerHeight,
                bottomWidgetsHeight,
                appBarHeight
            )
        );
}

internal class _BodyBuilder__scaffold : StatelessWidget
{
    public virtual Widget body { get; private set; } = default!;
    public virtual bool extendBody { get; private set; } = default!;
    public virtual bool extendBodyBehindAppBar { get; private set; } = default!;

    internal _BodyBuilder__scaffold(bool extendBody, bool extendBodyBehindAppBar, Widget body)
    {
        this.extendBody = extendBody;
        this.extendBodyBehindAppBar = extendBodyBehindAppBar;
        this.body = body;
    }

    public override Widget build(BuildContext context)
    {
        if (!extendBody && !extendBodyBehindAppBar)
        {
            return body;
        }
        return new LayoutBuilder(
            builder: (context, constraints) =>
            {
                var bodyConstraints = ((_BodyBoxConstraints__scaffold?)constraints)!;
                MediaQueryData metrics = MediaQuery.of(context);
                double bottomLocal = extendBody
                    ? Math.Max(metrics.padding.bottom, bodyConstraints.bottomWidgetsHeight)
                    : metrics.padding.bottom;
                double topLocal = extendBodyBehindAppBar
                    ? Math.Max(
                        metrics.padding.top,
                        bodyConstraints.appBarHeight + bodyConstraints.materialBannerHeight
                    )
                    : metrics.padding.top;
                return new MediaQuery(
                    data: metrics.copyWith(
                        padding: metrics.padding.copyWith(top: topLocal, bottom: bottomLocal)
                    ),
                    child: body
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _ScaffoldLayout__scaffold : MultiChildLayoutDelegate
{
    public virtual bool extendBody { get; private set; } = default!;
    public virtual bool extendBodyBehindAppBar { get; private set; } = default!;
    public virtual EdgeInsets minInsets { get; private set; } = default!;
    public virtual EdgeInsets minViewPadding { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual _ScaffoldGeometryNotifier__scaffold geometryNotifier { get; private set; } =
        default!;
    public virtual FloatingActionButtonLocation previousFloatingActionButtonLocation
    {
        get;
        private set;
    } = default!;
    public virtual FloatingActionButtonLocation currentFloatingActionButtonLocation
    {
        get;
        private set;
    } = default!;
    public virtual ValueListenable<double> floatingActionButtonMoveAnimation { get; private set; } =
        default!;
    public virtual FloatingActionButtonAnimator floatingActionButtonMotionAnimator
    {
        get;
        private set;
    } = default!;
    public virtual bool isSnackBarFloating { get; private set; } = default!;
    public virtual double? snackBarWidth { get; private set; }
    public virtual bool extendBodyBehindMaterialBanner { get; private set; } = default!;

    internal _ScaffoldLayout__scaffold(
        EdgeInsets minInsets,
        EdgeInsets minViewPadding,
        TextDirection textDirection,
        _ScaffoldGeometryNotifier__scaffold geometryNotifier,
        FloatingActionButtonLocation previousFloatingActionButtonLocation,
        FloatingActionButtonLocation currentFloatingActionButtonLocation,
        ValueListenable<double> floatingActionButtonMoveAnimation,
        FloatingActionButtonAnimator floatingActionButtonMotionAnimator,
        bool isSnackBarFloating,
        double? snackBarWidth,
        bool extendBody,
        bool extendBodyBehindAppBar,
        bool extendBodyBehindMaterialBanner
    )
        : base(relayout: floatingActionButtonMoveAnimation)
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
        var looseConstraints = BoxConstraints.CreateLoose(size);
        BoxConstraints fullWidthConstraints = looseConstraints.tighten(width: size.width);
        double bottomLocal = size.height;
        var contentTopLocal = 0.0;
        var bottomWidgetsHeightLocal = 0.0;
        var appBarHeightLocal = 0.0;
        if (hasChild(_ScaffoldSlot__scaffold.appBar))
        {
            appBarHeightLocal = layoutChild(
                _ScaffoldSlot__scaffold.appBar,
                fullWidthConstraints
            ).height;
            contentTopLocal = extendBodyBehindAppBar ? 0.0 : appBarHeightLocal;
            positionChild(_ScaffoldSlot__scaffold.appBar, Offset.zero);
        }
        double? bottomNavigationBarTopLocal = default!;
        if (hasChild(_ScaffoldSlot__scaffold.bottomNavigationBar))
        {
            double bottomNavigationBarHeight = layoutChild(
                _ScaffoldSlot__scaffold.bottomNavigationBar,
                fullWidthConstraints
            ).height;
            bottomWidgetsHeightLocal += bottomNavigationBarHeight;
            bottomNavigationBarTopLocal = Math.Max(0.0, bottomLocal - bottomWidgetsHeightLocal);
            positionChild(
                _ScaffoldSlot__scaffold.bottomNavigationBar,
                new Offset(
                    0.0,
                    (
                        bottomNavigationBarTopLocal
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
            );
        }
        if (hasChild(_ScaffoldSlot__scaffold.persistentFooter))
        {
            var footerConstraints = new BoxConstraints(
                maxWidth: fullWidthConstraints.maxWidth,
                maxHeight: Math.Max(0.0, bottomLocal - bottomWidgetsHeightLocal - contentTopLocal)
            );
            double persistentFooterHeight = layoutChild(
                _ScaffoldSlot__scaffold.persistentFooter,
                footerConstraints
            ).height;
            bottomWidgetsHeightLocal += persistentFooterHeight;
            positionChild(
                _ScaffoldSlot__scaffold.persistentFooter,
                new Offset(0.0, Math.Max(0.0, bottomLocal - bottomWidgetsHeightLocal))
            );
        }
        Size materialBannerSizeLocal = Size.zero;
        if (hasChild(_ScaffoldSlot__scaffold.materialBanner))
        {
            materialBannerSizeLocal = layoutChild(
                _ScaffoldSlot__scaffold.materialBanner,
                fullWidthConstraints
            );
            positionChild(
                _ScaffoldSlot__scaffold.materialBanner,
                new Offset(0.0, appBarHeightLocal)
            );
            if (!extendBodyBehindMaterialBanner)
            {
                contentTopLocal += materialBannerSizeLocal.height;
            }
        }
        double contentBottomLocal = Math.Max(
            0.0,
            bottomLocal - Math.Max(minInsets.bottom, bottomWidgetsHeightLocal)
        );
        if (hasChild(_ScaffoldSlot__scaffold.body))
        {
            double bodyMaxHeight = Math.Max(0.0, contentBottomLocal - contentTopLocal);
            if (extendBody && (minInsets.bottom <= bottomWidgetsHeightLocal))
            {
                bodyMaxHeight += bottomWidgetsHeightLocal;
                bodyMaxHeight = DorotiUiLibrary.clampDouble(
                    bodyMaxHeight,
                    0.0,
                    looseConstraints.maxHeight - contentTopLocal
                );
                DartRuntimePrimitives.Assert(() =>
                    bodyMaxHeight <= Math.Max(0.0, looseConstraints.maxHeight - contentTopLocal)
                );
            }
            else
            {
                bottomWidgetsHeightLocal = 0.0;
            }
            BoxConstraints bodyConstraints = new _BodyBoxConstraints__scaffold(
                maxWidth: fullWidthConstraints.maxWidth,
                maxHeight: bodyMaxHeight,
                materialBannerHeight: materialBannerSizeLocal.height,
                bottomWidgetsHeight: bottomWidgetsHeightLocal,
                appBarHeight: appBarHeightLocal
            );
            layoutChild(_ScaffoldSlot__scaffold.body, bodyConstraints);
            positionChild(_ScaffoldSlot__scaffold.body, new Offset(0.0, contentTopLocal));
        }
        Size bottomSheetSizeLocal = Size.zero;
        Size snackBarSizeLocal = Size.zero;
        if (hasChild(_ScaffoldSlot__scaffold.bodyScrim))
        {
            var bottomSheetScrimConstraints = new BoxConstraints(
                maxWidth: fullWidthConstraints.maxWidth,
                maxHeight: contentBottomLocal
            );
            layoutChild(_ScaffoldSlot__scaffold.bodyScrim, bottomSheetScrimConstraints);
            positionChild(_ScaffoldSlot__scaffold.bodyScrim, Offset.zero);
        }
        if (hasChild(_ScaffoldSlot__scaffold.snackBar) && !isSnackBarFloating)
        {
            snackBarSizeLocal = layoutChild(_ScaffoldSlot__scaffold.snackBar, fullWidthConstraints);
        }
        if (hasChild(_ScaffoldSlot__scaffold.bottomSheet))
        {
            var bottomSheetConstraints = new BoxConstraints(
                maxWidth: fullWidthConstraints.maxWidth,
                maxHeight: Math.Max(0.0, contentBottomLocal - contentTopLocal)
            );
            bottomSheetSizeLocal = layoutChild(
                _ScaffoldSlot__scaffold.bottomSheet,
                bottomSheetConstraints
            );
            positionChild(
                _ScaffoldSlot__scaffold.bottomSheet,
                new Offset(
                    (size.width - bottomSheetSizeLocal.width) / 2.0,
                    contentBottomLocal - bottomSheetSizeLocal.height
                )
            );
        }
        Rect floatingActionButtonRect = default!;
        if (hasChild(_ScaffoldSlot__scaffold.floatingActionButton))
        {
            Size fabSize = layoutChild(
                _ScaffoldSlot__scaffold.floatingActionButton,
                looseConstraints
            );
            var currentGeometry = new ScaffoldPrelayoutGeometry(
                bottomSheetSize: bottomSheetSizeLocal,
                contentBottom: contentBottomLocal,
                contentTop: appBarHeightLocal,
                floatingActionButtonSize: fabSize,
                minInsets: minInsets,
                scaffoldSize: size,
                snackBarSize: snackBarSizeLocal,
                materialBannerSize: materialBannerSizeLocal,
                textDirection: textDirection,
                minViewPadding: minViewPadding
            );
            Offset currentFabOffset = currentFloatingActionButtonLocation.getOffset(
                currentGeometry
            );
            Offset previousFabOffset = previousFloatingActionButtonLocation.getOffset(
                currentGeometry
            );
            Offset fabOffset = floatingActionButtonMotionAnimator.getOffset(
                begin: previousFabOffset,
                end: currentFabOffset,
                progress: floatingActionButtonMoveAnimation.value
            );
            positionChild(_ScaffoldSlot__scaffold.floatingActionButton, fabOffset);
            floatingActionButtonRect = fabOffset & fabSize;
        }
        if (hasChild(_ScaffoldSlot__scaffold.snackBar))
        {
            bool hasCustomWidth =
                (snackBarWidth is not null)
                && (
                    (
                        snackBarWidth
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) < size.width
                );
            if (Equals(snackBarSizeLocal, Size.zero))
            {
                snackBarSizeLocal = layoutChild(
                    _ScaffoldSlot__scaffold.snackBar,
                    hasCustomWidth ? looseConstraints : fullWidthConstraints
                );
            }
            double snackBarYOffsetBase = default!;
            bool showAboveFab = currentFloatingActionButtonLocation is not null;
            if (
                (!Equals(floatingActionButtonRect.size, Size.zero))
                && isSnackBarFloating
                && showAboveFab
            )
            {
                if (bottomNavigationBarTopLocal is not null)
                {
                    double bottomNavigationBarTop__40047__value47887 = (
                        bottomNavigationBarTopLocal
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    );
                    snackBarYOffsetBase = Math.Min(
                        (bottomNavigationBarTop__40047__value47887),
                        floatingActionButtonRect.top
                    );
                }
                else
                {
                    snackBarYOffsetBase = floatingActionButtonRect.top;
                }
            }
            else
            {
                double safeYOffsetBase = size.height - minViewPadding.bottom;
                snackBarYOffsetBase = isSnackBarFloating
                    ? Math.Min(contentBottomLocal, safeYOffsetBase)
                    : contentBottomLocal;
            }
            double xOffset = hasCustomWidth
                ? (
                    (
                        size.width
                        - (
                            snackBarWidth
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    ) / 2L
                )
                : 0.0;
            positionChild(
                _ScaffoldSlot__scaffold.snackBar,
                new Offset(xOffset, snackBarYOffsetBase - snackBarSizeLocal.height)
            );
            DartRuntimePrimitives.Assert(() =>
            {
                if (isSnackBarFloating)
                {
                    bool snackBarVisible = snackBarYOffsetBase - snackBarSizeLocal.height >= 0L;
                    if (!snackBarVisible)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary("Floating SnackBar presented off screen."),
                                    new ErrorDescription(
                                        "A SnackBar with behavior property set to SnackBarBehavior.floating is fully "
                                            + "or partially off screen because some or all the widgets provided to "
                                            + "Scaffold.floatingActionButton, Scaffold.persistentFooterButtons and "
                                            + "Scaffold.bottomNavigationBar take up too much vertical space.\n"
                                    ),
                                    new ErrorHint(
                                        "Consider constraining the size of these widgets to allow room for the SnackBar to be visible."
                                    ),
                                }
                            )
                        );
                    }
                }
                return true;
            });
        }
        if (hasChild(_ScaffoldSlot__scaffold.statusBar))
        {
            layoutChild(
                _ScaffoldSlot__scaffold.statusBar,
                fullWidthConstraints.tighten(height: minInsets.top)
            );
            positionChild(_ScaffoldSlot__scaffold.statusBar, Offset.zero);
        }
        if (hasChild(_ScaffoldSlot__scaffold.drawer))
        {
            layoutChild(_ScaffoldSlot__scaffold.drawer, BoxConstraints.CreateTight(size));
            positionChild(_ScaffoldSlot__scaffold.drawer, Offset.zero);
        }
        if (hasChild(_ScaffoldSlot__scaffold.endDrawer))
        {
            layoutChild(_ScaffoldSlot__scaffold.endDrawer, BoxConstraints.CreateTight(size));
            positionChild(_ScaffoldSlot__scaffold.endDrawer, Offset.zero);
        }
        geometryNotifier._updateWith(
            bottomNavigationBarTop: bottomNavigationBarTopLocal,
            floatingActionButtonArea: floatingActionButtonRect
        );
    }

    public override bool shouldRelayout(MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_ScaffoldLayout__scaffold)oldDelegate;
        return (!Equals(__oldDelegate.minInsets, minInsets))
            || (!Equals(__oldDelegate.minViewPadding, minViewPadding))
            || (!Equals(__oldDelegate.textDirection, textDirection))
            || (
                !Equals(
                    __oldDelegate.previousFloatingActionButtonLocation,
                    previousFloatingActionButtonLocation
                )
            )
            || (
                !Equals(
                    __oldDelegate.currentFloatingActionButtonLocation,
                    currentFloatingActionButtonLocation
                )
            )
            || (__oldDelegate.extendBody != extendBody)
            || (__oldDelegate.extendBodyBehindAppBar != extendBodyBehindAppBar);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class _FloatingActionButtonTransition__scaffold : StatefulWidget
{
    public virtual Widget? child { get; private set; }
    public virtual Animation<double> fabMoveAnimation { get; private set; } = default!;
    public virtual FloatingActionButtonAnimator fabMotionAnimator { get; private set; } = default!;
    public virtual _ScaffoldGeometryNotifier__scaffold geometryNotifier { get; private set; } =
        default!;
    public virtual AnimationController currentController { get; private set; } = default!;

    internal _FloatingActionButtonTransition__scaffold(
        Widget? child,
        Animation<double> fabMoveAnimation,
        FloatingActionButtonAnimator fabMotionAnimator,
        _ScaffoldGeometryNotifier__scaffold geometryNotifier,
        AnimationController currentController
    )
    {
        this.child = child;
        this.fabMoveAnimation = fabMoveAnimation;
        this.fabMotionAnimator = fabMotionAnimator;
        this.geometryNotifier = geometryNotifier;
        this.currentController = currentController;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _FloatingActionButtonTransitionState__scaffold()
        );
}

public class _FloatingActionButtonTransitionState__scaffold
    : State<_FloatingActionButtonTransition__scaffold>,
        TickerProviderStateMixin<_FloatingActionButtonTransition__scaffold>
{
    internal virtual AnimationController _previousController { get; set; } = default!;
    internal virtual CurvedAnimation? _previousExitScaleAnimation { get; set; } = default;
    internal virtual CurvedAnimation? _previousExitRotationCurvedAnimation { get; set; } = default;
    internal virtual CurvedAnimation? _currentEntranceScaleAnimation { get; set; } = default;
    internal virtual Animation<double> _previousScaleAnimation { get; set; } = default!;
    internal virtual TrainHoppingAnimation _previousRotationAnimation { get; set; } = default!;
    internal virtual Animation<double> _currentScaleAnimation { get; set; } = default!;
    internal virtual Animation<double> _extendedCurrentScaleAnimation { get; set; } = default!;
    internal virtual TrainHoppingAnimation _currentRotationAnimation { get; set; } = default!;
    internal virtual Widget? _previousChild { get; set; } = default;
    internal static Animatable<double> _entranceTurnTween = new Tween<double>(
        begin: 1.0 - Floating_action_button_locationLibrary.kFloatingActionButtonTurnInterval,
        end: 1.0
    ).chain(new CurveTween(curve: Curves.easeIn));
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _previousController = (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = new AnimationController(
                        duration: Floating_action_button_locationLibrary.kFloatingActionButtonSegue,
                        vsync: this
                    );
                    __cascade.addStatusListener(_handlePreviousAnimationStatusChanged);
                    return __cascade;
                }
            )
        )();
        _updateAnimations();
        if (widget.child is not null)
        {
            widget.currentController.value = 1.0;
            _updateGeometryScale(1.0);
        }
        else
        {
            _updateGeometryScale(0.0);
        }
    }

    public override void dispose()
    {
        _previousController.dispose();
        _previousExitScaleAnimation?.dispose();
        _previousExitRotationCurvedAnimation?.dispose();
        _currentEntranceScaleAnimation?.dispose();
        _disposeAnimations();
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

    public override void didUpdateWidget(_FloatingActionButtonTransition__scaffold oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (
            (!Equals(oldWidget.fabMotionAnimator, widget.fabMotionAnimator))
            || (!Equals(oldWidget.fabMoveAnimation, widget.fabMoveAnimation))
        )
        {
            _disposeAnimations();
            _updateAnimations();
        }
        var oldChildIsNull = oldWidget.child is null;
        var newChildIsNull = widget.child is null;
        if ((oldChildIsNull == newChildIsNull) && Equals(oldWidget.child?.key, widget.child?.key))
        {
            return;
        }
        if (_previousController.isDismissed)
        {
            double currentValue = widget.currentController.value;
            if ((currentValue == 0.0) || (oldWidget.child is null))
            {
                _previousChild = null;
                if (widget.child is not null)
                {
                    widget.currentController.forward();
                }
            }
            else
            {
                _previousChild = oldWidget.child;
                DartRuntimePrimitives.Ignore(
                    (
                        (Func<AnimationController>)(
                            () =>
                            {
                                var __cascade = _previousController;
                                __cascade.value = currentValue;
                                __cascade.reverse();
                                return __cascade;
                            }
                        )
                    )()
                );
                widget.currentController.value = 0.0;
            }
        }
    }

    internal virtual void _disposeAnimations()
    {
        _previousRotationAnimation.dispose();
        _currentRotationAnimation.dispose();
    }

    internal virtual void _updateAnimations()
    {
        _previousExitScaleAnimation?.dispose();
        _previousExitScaleAnimation = new CurvedAnimation(
            parent: _previousController,
            curve: Curves.easeIn
        );
        _previousExitRotationCurvedAnimation?.dispose();
        _previousExitRotationCurvedAnimation = new CurvedAnimation(
            parent: _previousController,
            curve: Curves.easeIn
        );
        Animation<double> previousExitRotationAnimation = new Tween<double>(
            begin: 1.0,
            end: 1.0
        ).animate(_previousExitRotationCurvedAnimation!);
        _currentEntranceScaleAnimation?.dispose();
        _currentEntranceScaleAnimation = new CurvedAnimation(
            parent: widget.currentController,
            curve: Curves.easeIn
        );
        Animation<double> currentEntranceRotationAnimation = widget.currentController.drive(
            _entranceTurnTween
        );
        Animation<double> moveScaleAnimation = widget.fabMotionAnimator.getScaleAnimation(
            parent: widget.fabMoveAnimation
        );
        Animation<double> moveRotationAnimation = widget.fabMotionAnimator.getRotationAnimation(
            parent: widget.fabMoveAnimation
        );
        if (Equals(widget.fabMotionAnimator, FloatingActionButtonAnimator.noAnimation))
        {
            _previousScaleAnimation = moveScaleAnimation;
            _currentScaleAnimation = moveScaleAnimation;
            _previousRotationAnimation = new TrainHoppingAnimation(moveRotationAnimation, null);
            _currentRotationAnimation = new TrainHoppingAnimation(moveRotationAnimation, null);
        }
        else
        {
            _previousScaleAnimation = DartRuntimePrimitives.ConvertValue<Animation<double>>(
                new AnimationMin<double>(moveScaleAnimation, _previousExitScaleAnimation!)
            );
            _currentScaleAnimation = DartRuntimePrimitives.ConvertValue<Animation<double>>(
                new AnimationMin<double>(moveScaleAnimation, _currentEntranceScaleAnimation!)
            );
            _previousRotationAnimation = new TrainHoppingAnimation(
                previousExitRotationAnimation,
                moveRotationAnimation
            );
            _currentRotationAnimation = new TrainHoppingAnimation(
                currentEntranceRotationAnimation,
                moveRotationAnimation
            );
        }
        _extendedCurrentScaleAnimation = _currentScaleAnimation.drive(
            new CurveTween(curve: new Interval(0.0, 0.1))
        );
        _currentScaleAnimation.addListener(_onProgressChanged);
        _previousScaleAnimation.addListener(_onProgressChanged);
    }

    internal virtual void _handlePreviousAnimationStatusChanged(AnimationStatus status)
    {
        setState(() =>
        {
            if ((widget.child is not null) && AnimationStatusMembers.isDismissed(status))
            {
                DartRuntimePrimitives.Assert(() => widget.currentController.isDismissed);
                widget.currentController.forward();
            }
        });
    }

    internal virtual bool _isExtendedFloatingActionButton(Widget? widget)
    {
        return widget is FloatingActionButton floatingActionButton
            && floatingActionButton.isExtended;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new Stack(
            alignment: Alignment.centerRight,
            children: (
                (Func<List<Widget>>)(
                    () =>
                    {
                        var __collection59262 = new List<Widget>();
                        if (!_previousController.isDismissed)
                        {
                            if (_isExtendedFloatingActionButton(_previousChild))
                            {
                                __collection59262.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new FadeTransition(
                                            opacity: _previousScaleAnimation,
                                            child: _previousChild
                                        )
                                    )
                                );
                            }
                            else
                            {
                                __collection59262.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new ScaleTransition(
                                            scale: _previousScaleAnimation,
                                            child: new RotationTransition(
                                                turns: _previousRotationAnimation,
                                                child: _previousChild
                                            )
                                        )
                                    )
                                );
                            }
                        }
                        if (_isExtendedFloatingActionButton(widget.child))
                        {
                            __collection59262.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new ScaleTransition(
                                        scale: _extendedCurrentScaleAnimation,
                                        child: new FadeTransition(
                                            opacity: _currentScaleAnimation,
                                            child: widget.child
                                        )
                                    )
                                )
                            );
                        }
                        else
                        {
                            __collection59262.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new ScaleTransition(
                                        scale: _currentScaleAnimation,
                                        child: new RotationTransition(
                                            turns: _currentRotationAnimation,
                                            child: widget.child
                                        )
                                    )
                                )
                            );
                        }
                        return __collection59262;
                    }
                )
            )()
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _onProgressChanged()
    {
        _updateGeometryScale(Math.Max(_previousScaleAnimation.value, _currentScaleAnimation.value));
    }

    internal virtual void _updateGeometryScale(double scale)
    {
        widget.geometryNotifier._updateWith(floatingActionButtonScale: scale);
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

public class Scaffold : StatefulWidget
{
    public virtual bool extendBody { get; private set; } = default!;
    public virtual bool drawerBarrierDismissible { get; private set; } = default!;
    public virtual bool extendBodyBehindAppBar { get; private set; } = default!;
    public virtual PreferredSizeWidget? appBar { get; private set; }
    public virtual Widget? body { get; private set; }
    public virtual Widget? floatingActionButton { get; private set; }
    public virtual FloatingActionButtonLocation? floatingActionButtonLocation { get; private set; }
    public virtual FloatingActionButtonAnimator? floatingActionButtonAnimator { get; private set; }
    public virtual List<Widget>? persistentFooterButtons { get; private set; }
    public virtual AlignmentDirectional persistentFooterAlignment { get; private set; } = default!;
    public virtual BoxDecoration? persistentFooterDecoration { get; private set; }
    public virtual Widget? drawer { get; private set; }
    public virtual Action<bool>? onDrawerChanged { get; private set; }
    public virtual Widget? endDrawer { get; private set; }
    public virtual Action<bool>? onEndDrawerChanged { get; private set; }
    public virtual Color? drawerScrimColor { get; private set; }
    public virtual Func<BuildContext, Animation<double>, Widget?> bottomSheetScrimBuilder
    {
        get;
        private set;
    } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual Widget? bottomNavigationBar { get; private set; }
    public virtual Widget? bottomSheet { get; private set; }
    public virtual bool? resizeToAvoidBottomInset { get; private set; }
    public virtual bool primary { get; private set; } = default!;
    public virtual Gestures.DragStartBehavior drawerDragStartBehavior { get; private set; } =
        default!;
    public virtual double? drawerEdgeDragWidth { get; private set; }
    public virtual bool drawerEnableOpenDragGesture { get; private set; } = default!;
    public virtual bool endDrawerEnableOpenDragGesture { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }

    public Scaffold(
        Key? key = null,
        PreferredSizeWidget? appBar = null,
        Widget? body = null,
        Widget? floatingActionButton = null,
        FloatingActionButtonLocation? floatingActionButtonLocation = null,
        FloatingActionButtonAnimator? floatingActionButtonAnimator = null,
        List<Widget>? persistentFooterButtons = null,
        AlignmentDirectional persistentFooterAlignment = default!,
        BoxDecoration? persistentFooterDecoration = null,
        Widget? drawer = null,
        Action<bool>? onDrawerChanged = null,
        Widget? endDrawer = null,
        Action<bool>? onEndDrawerChanged = null,
        Widget? bottomNavigationBar = null,
        Widget? bottomSheet = null,
        Color? backgroundColor = null,
        bool? resizeToAvoidBottomInset = null,
        bool primary = true,
        Gestures.DragStartBehavior drawerDragStartBehavior = Gestures.DragStartBehavior.start,
        bool extendBody = false,
        bool drawerBarrierDismissible = true,
        bool extendBodyBehindAppBar = false,
        Color? drawerScrimColor = null,
        Func<BuildContext, Animation<double>, Widget?> bottomSheetScrimBuilder = default!,
        double? drawerEdgeDragWidth = null,
        bool drawerEnableOpenDragGesture = true,
        bool endDrawerEnableOpenDragGesture = true,
        string? restorationId = null
    )
        : base(key: key)
    {
        AlignmentDirectional __persistentFooterAlignment =
            persistentFooterAlignment ?? AlignmentDirectional.centerEnd;
        Func<BuildContext, Animation<double>, Widget?> __bottomSheetScrimBuilder =
            bottomSheetScrimBuilder ?? _defaultBottomSheetScrimBuilder;
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

    public static ScaffoldState of(BuildContext context)
    {
        ScaffoldState? result = context.findAncestorStateOfType<ScaffoldState>();
        if (result is not null)
        {
            return result;
        }
        throw DartRuntimePrimitives.AsException(
            new FlutterError(
                new List<DiagnosticsNode>
                {
                    new ErrorSummary(
                        "Scaffold.of() called with a context that does not contain a Scaffold."
                    ),
                    new ErrorDescription(
                        "No Scaffold ancestor could be found starting from the context that was passed to Scaffold.of(). "
                            + "This usually happens when the context provided is from the same StatefulWidget as that "
                            + "whose build function actually creates the Scaffold widget being sought."
                    ),
                    new ErrorHint(
                        "There are several ways to avoid this problem. The simplest is to use a Builder to get a "
                            + "context that is \"under\" the Scaffold. For an example of this, please see the "
                            + "documentation for Scaffold.of():\n"
                            + "  https://api.flutter.dev/flutter/material/Scaffold/of.html"
                    ),
                    new ErrorHint(
                        "A more efficient solution is to split your build function into several widgets. This "
                            + "introduces a new context from which you can obtain the Scaffold. In this solution, "
                            + "you would have an outer widget that creates the Scaffold populated by instances of "
                            + "your new inner widgets, and then in these inner widgets you would use Scaffold.of().\n"
                            + "A less elegant but more expedient solution is assign a GlobalKey to the Scaffold, "
                            + "then use the key.currentState property to obtain the ScaffoldState rather than "
                            + "using the Scaffold.of() function."
                    ),
                    context.describeElement("The context used was"),
                }
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static ScaffoldState? maybeOf(BuildContext context)
    {
        return context.findAncestorStateOfType<ScaffoldState>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static ValueListenable<ScaffoldGeometry> geometryOf(BuildContext context)
    {
        _ScaffoldScope__scaffold? scaffoldScope =
            context.dependOnInheritedWidgetOfExactType<_ScaffoldScope__scaffold>();
        if (scaffoldScope is null)
        {
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            "Scaffold.geometryOf() called with a context that does not contain a Scaffold."
                        ),
                        new ErrorDescription(
                            "This usually happens when the context provided is from the same StatefulWidget as that "
                                + "whose build function actually creates the Scaffold widget being sought."
                        ),
                        new ErrorHint(
                            "There are several ways to avoid this problem. The simplest is to use a Builder to get a "
                                + "context that is \"under\" the Scaffold. For an example of this, please see the "
                                + "documentation for Scaffold.of():\n"
                                + "  https://api.flutter.dev/flutter/material/Scaffold/of.html"
                        ),
                        new ErrorHint(
                            "A more efficient solution is to split your build function into several widgets. This "
                                + "introduces a new context from which you can obtain the Scaffold. In this solution, "
                                + "you would have an outer widget that creates the Scaffold populated by instances of "
                                + "your new inner widgets, and then in these inner widgets you would use Scaffold.geometryOf()."
                        ),
                        context.describeElement("The context used was"),
                    }
                )
            );
        }
        return scaffoldScope.geometryNotifier;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static bool hasDrawer(BuildContext context, bool registerForUpdates = true)
    {
        if (registerForUpdates)
        {
            _ScaffoldScope__scaffold? scaffold =
                context.dependOnInheritedWidgetOfExactType<_ScaffoldScope__scaffold>();
            return scaffold?.hasDrawer ?? false;
        }
        else
        {
            ScaffoldState? scaffoldLocal = context.findAncestorStateOfType<ScaffoldState>();
            return scaffoldLocal?.hasDrawer ?? false;
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static Widget _defaultBottomSheetScrimBuilder(
        BuildContext context,
        Animation<double> animation
    )
    {
        return new AnimatedBuilder(
            animation: animation,
            builder: (context, child) =>
            {
                double extentRemaining =
                    ScaffoldLibrary._kBottomSheetDominatesPercentage * (1.0 - animation.value);
                double floatingButtonVisibilityValue =
                    extentRemaining * ScaffoldLibrary._kBottomSheetDominatesPercentage * 10L;
                double opacity = Math.Max(
                    ScaffoldLibrary._kMinBottomSheetScrimOpacity,
                    ScaffoldLibrary._kMaxBottomSheetScrimOpacity - floatingButtonVisibilityValue
                );
                return new ModalBarrier(
                    dismissible: false,
                    color: Colors.black.withOpacity(opacity)
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new ScaffoldState());
}

public class ScaffoldState
    : State<Scaffold>,
        TickerProviderStateMixin<Scaffold>,
        RestorationMixin<Scaffold>,
        WidgetsBindingObserver
{
    internal virtual GlobalKey<DrawerControllerState> _drawerKey { get; private set; } =
        GlobalKey<DrawerControllerState>.Create();
    internal virtual GlobalKey<DrawerControllerState> _endDrawerKey { get; private set; } =
        GlobalKey<DrawerControllerState>.Create();
    internal virtual GlobalKey<IState> _bodyKey { get; private set; } = GlobalKey<IState>.Create();
    private bool __late__statusBarKey_initialized;
    private GlobalKey<IState> __late__statusBarKey = default!;
    internal virtual GlobalKey<IState> _statusBarKey
    {
        get
        {
            if (!__late__statusBarKey_initialized)
            {
                __late__statusBarKey = GlobalKey<IState>.Create();
                __late__statusBarKey_initialized = true;
            }
            return __late__statusBarKey;
        }
    }
    internal virtual double? _appBarMaxHeight { get; set; } = default;
    internal virtual RestorableBool _drawerOpened { get; private set; } = new RestorableBool(false);
    internal virtual RestorableBool _endDrawerOpened { get; private set; } =
        new RestorableBool(false);
    internal virtual ScaffoldMessengerState? _scaffoldMessenger { get; set; } = default;
    internal virtual ScaffoldFeatureController<
        SnackBar,
        SnackBarClosedReason
    >? _messengerSnackBar { get; set; } = default;
    internal virtual ScaffoldFeatureController<
        MaterialBanner,
        MaterialBannerClosedReason
    >? _messengerMaterialBanner { get; set; } = default;
    internal virtual List<_StandardBottomSheet__scaffold> _dismissedBottomSheets
    {
        get;
        private set;
    } = new List<_StandardBottomSheet__scaffold>();
    internal virtual PersistentBottomSheetController? _currentBottomSheet { get; set; } = default;
    internal virtual GlobalKey<IState> _currentBottomSheetKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual LocalHistoryEntry? _persistentSheetHistoryEntry { get; set; } = default;
    internal virtual AnimationController _floatingActionButtonMoveController { get; set; } =
        default!;
    internal virtual FloatingActionButtonAnimator _floatingActionButtonAnimator { get; set; } =
        default!;
    internal virtual FloatingActionButtonLocation? _previousFloatingActionButtonLocation { get; set; } =
        default;
    internal virtual FloatingActionButtonLocation? _floatingActionButtonLocation { get; set; } =
        default;
    internal virtual AnimationController _floatingActionButtonVisibilityController { get; set; } =
        default!;
    internal virtual _ScaffoldGeometryNotifier__scaffold _geometryNotifier { get; set; } = default!;
    internal virtual AnimationController _bottomSheetScrimAnimationController { get; set; } =
        default!;
    internal virtual bool _showBodyScrim { get; set; } = false;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } =
        new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } =
        default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

    public virtual string? restorationId => widget.restorationId;

    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(_drawerOpened, "drawer_open");
        registerForRestoration(_endDrawerOpened, "end_drawer_open");
    }

    public virtual bool hasAppBar =>
        DartRuntimePrimitives.ConvertValue<bool>(widget.appBar is not null);
    public virtual bool hasDrawer =>
        DartRuntimePrimitives.ConvertValue<bool>(widget.drawer is not null);
    public virtual bool hasEndDrawer =>
        DartRuntimePrimitives.ConvertValue<bool>(widget.endDrawer is not null);
    public virtual bool hasFloatingActionButton =>
        DartRuntimePrimitives.ConvertValue<bool>(widget.floatingActionButton is not null);
    public virtual double? appBarMaxHeight => _appBarMaxHeight;
    public virtual bool isDrawerOpen => _drawerOpened.value;
    public virtual bool isDrawerBarrierDismissible => widget.drawerBarrierDismissible;
    public virtual bool isEndDrawerOpen => _endDrawerOpened.value;

    internal virtual void _drawerOpenedCallback(bool isOpened)
    {
        if ((_drawerOpened.value != isOpened) && (_drawerKey.currentState is not null))
        {
            setState(() =>
            {
                _drawerOpened.value = isOpened;
            });
            widget.onDrawerChanged?.Invoke(isOpened);
        }
    }

    internal virtual void _endDrawerOpenedCallback(bool isOpened)
    {
        if ((_endDrawerOpened.value != isOpened) && (_endDrawerKey.currentState is not null))
        {
            setState(() =>
            {
                _endDrawerOpened.value = isOpened;
            });
            widget.onEndDrawerChanged?.Invoke(isOpened);
        }
    }

    public virtual void openDrawer()
    {
        if ((_endDrawerKey.currentState is not null) && _endDrawerOpened.value)
        {
            _endDrawerKey.currentState!.close();
        }
        _drawerKey.currentState?.open();
    }

    public virtual void openEndDrawer()
    {
        if ((_drawerKey.currentState is not null) && _drawerOpened.value)
        {
            _drawerKey.currentState!.close();
        }
        _endDrawerKey.currentState?.open();
    }

    internal virtual void _updateSnackBar()
    {
        ScaffoldFeatureController<SnackBar, SnackBarClosedReason>? messengerSnackBar =
            Enumerable.Any(_scaffoldMessenger!._snackBars)
                ? _scaffoldMessenger!._snackBars.Peek()
                : null;
        if (!Equals(_messengerSnackBar, messengerSnackBar))
        {
            setState(() =>
            {
                _messengerSnackBar = messengerSnackBar;
            });
        }
    }

    internal virtual void _updateMaterialBanner()
    {
        ScaffoldFeatureController<
            MaterialBanner,
            MaterialBannerClosedReason
        >? messengerMaterialBanner = Enumerable.Any(_scaffoldMessenger!._materialBanners)
            ? _scaffoldMessenger!._materialBanners.Peek()
            : null;
        if (!Equals(_messengerMaterialBanner, messengerMaterialBanner))
        {
            setState(() =>
            {
                _messengerMaterialBanner = messengerMaterialBanner;
            });
        }
    }

    internal virtual void _maybeBuildPersistentBottomSheet()
    {
        if ((widget.bottomSheet is not null) && (_currentBottomSheet is null))
        {
            AnimationController animationControllerLocal = (
                (Func<AnimationController>)(
                    () =>
                    {
                        var __cascade = BottomSheet.createAnimationController(this);
                        __cascade.value = 1.0;
                        return __cascade;
                    }
                )
            )();
            bool persistentBottomSheetExtentChanged(DraggableScrollableNotification notification)
            {
                if (
                    (notification.extent - notification.initialExtent)
                    > Foundation.ConstantsLibrary.precisionErrorTolerance
                )
                {
                    if (_persistentSheetHistoryEntry is null)
                    {
                        _persistentSheetHistoryEntry = new LocalHistoryEntry(onRemove: () =>
                        {
                            DraggableScrollableActuator.reset(notification.context);
                            showBodyScrim(false, 0.0);
                            _floatingActionButtonVisibilityController.value = 1.0;
                            _persistentSheetHistoryEntry = null;
                        });
                        ModalRoute<object>
                            .untypedOf(context)!
                            .addLocalHistoryEntry(_persistentSheetHistoryEntry!);
                    }
                }
                else
                {
                    if (_persistentSheetHistoryEntry is not null)
                    {
                        _persistentSheetHistoryEntry!.remove();
                    }
                }
                return false;
                throw new InvalidOperationException(
                    "Control flow completed without returning a value."
                );
            }
            if (Enumerable.Any(_dismissedBottomSheets))
            {
                var sheets = new List<_StandardBottomSheet__scaffold>(_dismissedBottomSheets);
                foreach (var sheet in sheets)
                {
                    sheet.animationController.reset();
                }
                DartRuntimePrimitives.Assert(() => !Enumerable.Any(_dismissedBottomSheets));
            }
            _currentBottomSheet = _buildBottomSheet(
                (context) =>
                {
                    return new NotificationListener<DraggableScrollableNotification>(
                        onNotification: persistentBottomSheetExtentChanged,
                        child: new DraggableScrollableActuator(
                            child: new StatefulBuilder(
                                key: _currentBottomSheetKey,
                                builder: (context, setState) =>
                                {
                                    return widget.bottomSheet ?? SizedBox.CreateShrink();
                                    throw new InvalidOperationException(
                                        "Callback completed without returning a value."
                                    );
                                }
                            )
                        )
                    );
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                },
                isPersistent: true,
                animationController: animationControllerLocal
            );
        }
    }

    internal virtual void _closeCurrentBottomSheet()
    {
        if (_currentBottomSheet is not null)
        {
            if (!_currentBottomSheet!._isLocalHistoryEntry)
            {
                _currentBottomSheet!.close();
            }
            DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Ignore(
                    _currentBottomSheet?._completer.future.whenComplete(() =>
                    {
                        DartRuntimePrimitives.Assert(() => _currentBottomSheet is null);
                    })
                );
                return true;
            });
        }
    }

    public virtual void closeDrawer()
    {
        if (hasDrawer && isDrawerOpen)
        {
            _drawerKey.currentState!.close();
        }
    }

    public virtual void closeEndDrawer()
    {
        if (hasEndDrawer && isEndDrawerOpen)
        {
            _endDrawerKey.currentState!.close();
        }
    }

    internal virtual void _updatePersistentBottomSheet()
    {
        _currentBottomSheetKey.currentState!.setState(() => { });
    }

    internal virtual PersistentBottomSheetController _buildBottomSheet(
        Func<BuildContext, Widget> builder,
        bool isPersistent,
        AnimationController animationController,
        Color? backgroundColor = null,
        double? elevation = null,
        ShapeBorder? shape = null,
        Clip? clipBehavior = null,
        BoxConstraints? constraints = null,
        bool? enableDrag = null,
        bool? showDragHandle = null,
        bool shouldDisposeAnimationController = true
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (
                (widget.bottomSheet is not null)
                && isPersistent
                && (_currentBottomSheet is not null)
            )
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "Scaffold.bottomSheet cannot be specified while a bottom sheet "
                            + "displayed with showBottomSheet() is still visible.\n"
                            + "Rebuild the Scaffold with a null bottomSheet before calling showBottomSheet()."
                    )
                );
            }
            return true;
        });
        var completer = new Completer<object?>();
        var bottomSheetKey = GlobalKey<_StandardBottomSheetState__scaffold>.Create();
        _StandardBottomSheet__scaffold bottomSheetLocal = default!;
        var removedEntry = false;
        var doingDispose = false;
        void removePersistentSheetHistoryEntryIfNeeded()
        {
            DartRuntimePrimitives.Assert(() => isPersistent);
            if (_persistentSheetHistoryEntry is not null)
            {
                _persistentSheetHistoryEntry!.remove();
                _persistentSheetHistoryEntry = null;
            }
        }
        void removeCurrentBottomSheet()
        {
            removedEntry = true;
            if (_currentBottomSheet is null)
            {
                return;
            }
            DartRuntimePrimitives.Assert(() =>
                Equals(_currentBottomSheet!._widget, bottomSheetLocal)
            );
            DartRuntimePrimitives.Assert(() => bottomSheetKey.currentState is not null);
            _showFloatingActionButton();
            if (isPersistent)
            {
                removePersistentSheetHistoryEntryIfNeeded();
            }
            bottomSheetKey.currentState!.close();
            setState(() =>
            {
                _showBodyScrim = false;
                _bottomSheetScrimAnimationController.value = 0.0;
                _currentBottomSheet = null;
            });
            if (!animationController.isDismissed)
            {
                _dismissedBottomSheets.Add(bottomSheetLocal);
            }
            completer.complete();
        }
        LocalHistoryEntry? entry = isPersistent
            ? null
            : new LocalHistoryEntry(onRemove: () =>
            {
                if (
                    !removedEntry
                    && Equals(_currentBottomSheet?._widget, bottomSheetLocal)
                    && !doingDispose
                )
                {
                    removeCurrentBottomSheet();
                }
            });
        void removeEntryIfNeeded()
        {
            if (!isPersistent && !removedEntry)
            {
                DartRuntimePrimitives.Assert(() => entry is not null);
                entry!.remove();
                removedEntry = true;
            }
        }
        bottomSheetLocal = new _StandardBottomSheet__scaffold(
            key: bottomSheetKey,
            animationController: animationController,
            enableDrag: enableDrag ?? !isPersistent,
            showDragHandle: showDragHandle,
            onClosing: () =>
            {
                if (_currentBottomSheet is null)
                {
                    return;
                }
                DartRuntimePrimitives.Assert(() =>
                    Equals(_currentBottomSheet!._widget, bottomSheetLocal)
                );
                removeEntryIfNeeded();
            },
            onDismissed: () =>
            {
                if (_dismissedBottomSheets.Contains(bottomSheetLocal))
                {
                    setState(() =>
                    {
                        _dismissedBottomSheets.Remove(bottomSheetLocal);
                    });
                }
            },
            onDispose: () =>
            {
                doingDispose = true;
                removeEntryIfNeeded();
                if (shouldDisposeAnimationController)
                {
                    animationController.dispose();
                }
            },
            builder: builder,
            isPersistent: isPersistent,
            backgroundColor: backgroundColor,
            elevation: elevation,
            shape: shape,
            clipBehavior: clipBehavior,
            constraints: constraints
        );
        if (!isPersistent)
        {
            ModalRoute<object>.untypedOf(context)!.addLocalHistoryEntry(entry!);
        }
        return new PersistentBottomSheetController(
            bottomSheetLocal,
            completer,
            (entry is not null) ? entry.remove : removeCurrentBottomSheet,
            (fn) =>
            {
                bottomSheetKey.currentState?.setState(() => fn());
            },
            !isPersistent
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual PersistentBottomSheetController showBottomSheet(
        Func<BuildContext, Widget> builder,
        Color? backgroundColor = null,
        double? elevation = null,
        ShapeBorder? shape = null,
        Clip? clipBehavior = null,
        BoxConstraints? constraints = null,
        bool? enableDrag = null,
        bool? showDragHandle = null,
        AnimationController? transitionAnimationController = null,
        AnimationStyle? sheetAnimationStyle = null
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (widget.bottomSheet is not null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "Scaffold.bottomSheet cannot be specified while a bottom sheet "
                            + "displayed with showBottomSheet() is still visible.\n"
                            + "Rebuild the Scaffold with a null bottomSheet before calling showBottomSheet()."
                    )
                );
            }
            return true;
        });
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        _closeCurrentBottomSheet();
        AnimationController controller = (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade =
                        transitionAnimationController
                        ?? BottomSheet.createAnimationController(
                            this,
                            sheetAnimationStyle: sheetAnimationStyle
                        );
                    __cascade.forward();
                    return __cascade;
                }
            )
        )();
        setState(() =>
        {
            _currentBottomSheet = _buildBottomSheet(
                builder,
                isPersistent: false,
                animationController: controller,
                backgroundColor: backgroundColor,
                elevation: elevation,
                shape: shape,
                clipBehavior: clipBehavior,
                constraints: constraints,
                enableDrag: enableDrag,
                showDragHandle: showDragHandle,
                shouldDisposeAnimationController: transitionAnimationController is null
            );
        });
        return _currentBottomSheet!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Scheduler.TickerFuture _showFloatingActionButton()
    {
        return _floatingActionButtonVisibilityController.forward();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _moveFloatingActionButton(FloatingActionButtonLocation newLocation)
    {
        FloatingActionButtonLocation? previousLocation = _floatingActionButtonLocation;
        var restartAnimationFrom = 0.0;
        if (_floatingActionButtonMoveController.isAnimating)
        {
            previousLocation = DartRuntimePrimitives.ConvertValue<FloatingActionButtonLocation>(
                new _TransitionSnapshotFabLocation__scaffold(
                    _previousFloatingActionButtonLocation!,
                    _floatingActionButtonLocation!,
                    _floatingActionButtonAnimator,
                    _floatingActionButtonMoveController.value
                )
            );
            restartAnimationFrom = _floatingActionButtonAnimator.getAnimationRestart(
                _floatingActionButtonMoveController.value
            );
        }
        setState(() =>
        {
            _previousFloatingActionButtonLocation = previousLocation;
            _floatingActionButtonLocation = newLocation;
        });
        _floatingActionButtonMoveController.forward(from: restartAnimationFrom);
    }

    public virtual void handleStatusBarTap()
    {
        DartRuntimePrimitives.Assert(() => widget.primary);
        ScrollController? primaryScrollController = PrimaryScrollController.maybeOf(context);
        if (
            (primaryScrollController is not null)
            && primaryScrollController.hasClients
            && _HitTestableAtOrigin__scaffold.hitTestableAtOrigin(_statusBarKey)
        )
        {
            DartRuntimePrimitives.Ignore(
                primaryScrollController.animateTo(
                    0.0,
                    duration: Duration.Create(milliseconds: 1000L),
                    curve: Curves.easeOutCirc
                )
            );
        }
    }

    internal virtual bool _resizeToAvoidBottomInset
    {
        get { return widget.resizeToAvoidBottomInset ?? true; }
    }

    public override void initState()
    {
        base.initState();
        _geometryNotifier = new _ScaffoldGeometryNotifier__scaffold(
            new ScaffoldGeometry(),
            context
        );
        _floatingActionButtonLocation =
            widget.floatingActionButtonLocation
            ?? ScaffoldLibrary._kDefaultFloatingActionButtonLocation;
        _floatingActionButtonAnimator =
            widget.floatingActionButtonAnimator
            ?? ScaffoldLibrary._kDefaultFloatingActionButtonAnimator;
        _previousFloatingActionButtonLocation = _floatingActionButtonLocation;
        _floatingActionButtonMoveController = new AnimationController(
            vsync: this,
            value: 1.0,
            duration: Floating_action_button_locationLibrary.kFloatingActionButtonSegue * 2L
        );
        _floatingActionButtonVisibilityController = new AnimationController(
            duration: Floating_action_button_locationLibrary.kFloatingActionButtonSegue,
            vsync: this
        );
        _bottomSheetScrimAnimationController = new AnimationController(vsync: this);
        if (widget.primary)
        {
            WidgetsBinding.instance.addObserver(this);
        }
    }

    public override void didUpdateWidget(Scaffold oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if (!Equals(widget.floatingActionButtonAnimator, oldWidget.floatingActionButtonAnimator))
        {
            _floatingActionButtonAnimator =
                widget.floatingActionButtonAnimator
                ?? ScaffoldLibrary._kDefaultFloatingActionButtonAnimator;
        }
        if (!Equals(widget.floatingActionButtonLocation, oldWidget.floatingActionButtonLocation))
        {
            _moveFloatingActionButton(
                widget.floatingActionButtonLocation
                    ?? ScaffoldLibrary._kDefaultFloatingActionButtonLocation
            );
        }
        if (!Equals(widget.bottomSheet, oldWidget.bottomSheet))
        {
            DartRuntimePrimitives.Assert(() =>
            {
                if (
                    (widget.bottomSheet is not null)
                    && (_currentBottomSheet?._isLocalHistoryEntry ?? false)
                )
                {
                    throw DartRuntimePrimitives.AsException(
                        new FlutterError(
                            new List<DiagnosticsNode>
                            {
                                new ErrorSummary(
                                    "Scaffold.bottomSheet cannot be specified while a bottom sheet displayed "
                                        + "with showBottomSheet() is still visible."
                                ),
                                new ErrorHint(
                                    "Use the PersistentBottomSheetController "
                                        + "returned by showBottomSheet() to close the old bottom sheet before creating "
                                        + "a Scaffold with a (non null) bottomSheet."
                                ),
                            }
                        )
                    );
                }
                return true;
            });
            if (widget.bottomSheet is null)
            {
                _closeCurrentBottomSheet();
            }
            else
            {
                if ((widget.bottomSheet is not null) && (oldWidget.bottomSheet is null))
                {
                    _maybeBuildPersistentBottomSheet();
                }
                else
                {
                    _updatePersistentBottomSheet();
                }
            }
        }
        switch ((oldWidget.primary, widget.primary))
        {
            case (true, false):
            {
                WidgetsBinding.instance.removeObserver(this);
                break;
            }
            case (false, true):
            {
                WidgetsBinding.instance.addObserver(this);
                break;
            }
            case (true, true) or (false, false):
                break;
        }
    }

    public override void didChangeDependencies()
    {
        ScaffoldMessengerState? currentScaffoldMessenger = ScaffoldMessenger.maybeOf(context);
        if (
            (_scaffoldMessenger is not null)
            && (
                (currentScaffoldMessenger is null)
                || (!Equals(_scaffoldMessenger, currentScaffoldMessenger))
            )
        )
        {
            _scaffoldMessenger?._unregister(this);
        }
        _scaffoldMessenger = currentScaffoldMessenger;
        _scaffoldMessenger?._register(this);
        _maybeBuildPersistentBottomSheet();
        base.didChangeDependencies();
        RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: needsRestore
        );
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
    }

    public override void deactivate()
    {
        WidgetsBinding.instance.removeObserver(this);
        base.deactivate();
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
        if (widget.primary)
        {
            WidgetsBinding.instance.addObserver(this);
        }
    }

    public override void dispose()
    {
        _geometryNotifier.dispose();
        _floatingActionButtonMoveController.dispose();
        _floatingActionButtonVisibilityController.dispose();
        _scaffoldMessenger?._unregister(this);
        _drawerOpened.dispose();
        _endDrawerOpened.dispose();
        _bottomSheetScrimAnimationController.dispose();
        _properties.forEach(
            (property, listener) =>
            {
                if (!property._disposed)
                {
                    property.removeListener(listener);
                }
            }
        );
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    internal virtual void _addIfNonNull(
        List<LayoutId> children,
        Widget? child,
        object childId,
        bool removeLeftPadding,
        bool removeTopPadding,
        bool removeRightPadding,
        bool removeBottomPadding,
        bool removeBottomInset = false,
        bool maintainBottomViewPadding = false
    )
    {
        if (child is null)
        {
            return;
        }
        // A size change must reach each slot's MediaQuery without rebuilding
        // Scaffold's Material, gesture and layout configuration. Subscribe in
        // the local wrapper; Scaffold itself observes its padding/inset aspects.
        children.Add(
            new LayoutId(
                id: childId,
                child: new ScaffoldSlotMediaQuery(
                    child,
                    removeLeftPadding,
                    removeTopPadding,
                    removeRightPadding,
                    removeBottomPadding,
                    removeBottomInset,
                    maintainBottomViewPadding
                )
            )
        );
    }

    internal virtual void _buildEndDrawer(List<LayoutId> children, TextDirection textDirection)
    {
        if (widget.endDrawer is not null)
        {
            DartRuntimePrimitives.Assert(() => hasEndDrawer);
            _addIfNonNull(
                children,
                new DrawerController(
                    key: _endDrawerKey,
                    alignment: DrawerAlignment.end,
                    drawerCallback: _endDrawerOpenedCallback,
                    dragStartBehavior: widget.drawerDragStartBehavior,
                    scrimColor: widget.drawerScrimColor,
                    edgeDragWidth: widget.drawerEdgeDragWidth,
                    enableOpenDragGesture: widget.endDrawerEnableOpenDragGesture,
                    isDrawerOpen: _endDrawerOpened.value,
                    drawerBarrierDismissible: widget.drawerBarrierDismissible,
                    child: widget.endDrawer!
                ),
                _ScaffoldSlot__scaffold.endDrawer,
                removeLeftPadding: Equals(textDirection, TextDirection.ltr),
                removeTopPadding: false,
                removeRightPadding: Equals(textDirection, TextDirection.rtl),
                removeBottomPadding: false
            );
        }
    }

    internal virtual void _buildDrawer(List<LayoutId> children, TextDirection textDirection)
    {
        if (widget.drawer is not null)
        {
            DartRuntimePrimitives.Assert(() => hasDrawer);
            _addIfNonNull(
                children,
                new DrawerController(
                    key: _drawerKey,
                    alignment: DrawerAlignment.start,
                    drawerCallback: _drawerOpenedCallback,
                    dragStartBehavior: widget.drawerDragStartBehavior,
                    scrimColor: widget.drawerScrimColor,
                    edgeDragWidth: widget.drawerEdgeDragWidth,
                    enableOpenDragGesture: widget.drawerEnableOpenDragGesture,
                    isDrawerOpen: _drawerOpened.value,
                    drawerBarrierDismissible: widget.drawerBarrierDismissible,
                    child: widget.drawer!
                ),
                _ScaffoldSlot__scaffold.drawer,
                removeLeftPadding: Equals(textDirection, TextDirection.rtl),
                removeTopPadding: false,
                removeRightPadding: Equals(textDirection, TextDirection.ltr),
                removeBottomPadding: false
            );
        }
    }

    public virtual void showBodyScrim(bool value, double animationValue)
    {
        if (_showBodyScrim != value)
        {
            setState(() =>
            {
                _showBodyScrim = value;
            });
        }
        if (_bottomSheetScrimAnimationController.value != animationValue)
        {
            _bottomSheetScrimAnimationController.value = animationValue;
        }
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() =>
            Widgets.DebugLibrary.debugCheckHasDirectionality(context)
        );
        ThemeData themeData = Theme.of(context);
        TextDirection textDirectionLocal = Directionality.of(context);
        var childrenLocal = new List<LayoutId>();
        _addIfNonNull(
            childrenLocal,
            (widget.body is null)
                ? null
                : new _BodyBuilder__scaffold(
                    extendBody: widget.extendBody,
                    extendBodyBehindAppBar: widget.extendBodyBehindAppBar,
                    body: new KeyedSubtree(key: _bodyKey, child: widget.body!)
                ),
            _ScaffoldSlot__scaffold.body,
            removeLeftPadding: false,
            removeTopPadding: widget.appBar is not null,
            removeRightPadding: false,
            removeBottomPadding: (widget.bottomNavigationBar is not null)
                || (widget.persistentFooterButtons is not null),
            removeBottomInset: _resizeToAvoidBottomInset
        );
        if (_showBodyScrim)
        {
            _addIfNonNull(
                childrenLocal,
                widget.bottomSheetScrimBuilder(context, _bottomSheetScrimAnimationController.view),
                _ScaffoldSlot__scaffold.bodyScrim,
                removeLeftPadding: true,
                removeTopPadding: true,
                removeRightPadding: true,
                removeBottomPadding: true
            );
        }
        if (widget.appBar is not null)
        {
            double topPadding = widget.primary ? MediaQuery.paddingOf(context).top : 0.0;
            _appBarMaxHeight =
                AppBar.preferredHeightFor(context, widget.appBar!.preferredSize) + topPadding;
            DartRuntimePrimitives.Assert(() =>
                (
                    (
                        _appBarMaxHeight
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) >= 0.0
                )
                && double.IsFinite(
                    (
                        _appBarMaxHeight
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
            );
            _addIfNonNull(
                childrenLocal,
                new ConstrainedBox(
                    constraints: new BoxConstraints(
                        maxHeight: (
                            _appBarMaxHeight
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    ),
                    child: FlexibleSpaceBar.createSettings(
                        currentExtent: (
                            _appBarMaxHeight
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ),
                        child: (Widget)widget.appBar!
                    )
                ),
                _ScaffoldSlot__scaffold.appBar,
                removeLeftPadding: false,
                removeTopPadding: false,
                removeRightPadding: false,
                removeBottomPadding: true
            );
        }
        var isSnackBarFloatingLocal = false;
        double? snackBarWidthLocal = default!;
        if ((_currentBottomSheet is not null) || Enumerable.Any(_dismissedBottomSheets))
        {
            Widget stack = new Stack(
                alignment: Alignment.bottomCenter,
                children: (
                    (Func<List<Widget>>)(
                        () =>
                        {
                            var __collection121351 = new List<Widget>();
                            __collection121351.AddRange(_dismissedBottomSheets);
                            var __collectionElement121387 = _currentBottomSheet?._widget;
                            if (__collectionElement121387 is { } __nonNullCollectionElement121387)
                            {
                                __collection121351.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        __nonNullCollectionElement121387
                                    )
                                );
                            }
                            return __collection121351;
                        }
                    )
                )()
            );
            _addIfNonNull(
                childrenLocal,
                stack,
                _ScaffoldSlot__scaffold.bottomSheet,
                removeLeftPadding: false,
                removeTopPadding: true,
                removeRightPadding: false,
                removeBottomPadding: _resizeToAvoidBottomInset
            );
        }
        if (_messengerSnackBar is not null)
        {
            SnackBarThemeData snackBarTheme = SnackBarTheme.of(context);
            SnackBarBehavior snackBarBehavior =
                (_messengerSnackBar?._widget.behavior ?? snackBarTheme.behavior)
                ?? SnackBarBehavior.@fixed;
            isSnackBarFloatingLocal = Equals(snackBarBehavior, SnackBarBehavior.floating);
            snackBarWidthLocal = _messengerSnackBar?._widget.width ?? snackBarTheme.width;
            _addIfNonNull(
                childrenLocal,
                _messengerSnackBar?._widget,
                _ScaffoldSlot__scaffold.snackBar,
                removeLeftPadding: false,
                removeTopPadding: true,
                removeRightPadding: false,
                removeBottomPadding: (widget.bottomNavigationBar is not null)
                    || (widget.persistentFooterButtons is not null),
                maintainBottomViewPadding: !_resizeToAvoidBottomInset
            );
        }
        var extendBodyBehindMaterialBannerLocal = false;
        if (_messengerMaterialBanner is not null)
        {
            MaterialBannerThemeData bannerTheme = MaterialBannerTheme.of(context);
            double elevationLocal =
                (_messengerMaterialBanner?._widget.elevation ?? bannerTheme.elevation) ?? 0.0;
            extendBodyBehindMaterialBannerLocal = (elevationLocal) != 0.0;
            _addIfNonNull(
                childrenLocal,
                _messengerMaterialBanner?._widget,
                _ScaffoldSlot__scaffold.materialBanner,
                removeLeftPadding: false,
                removeTopPadding: widget.appBar is not null,
                removeRightPadding: false,
                removeBottomPadding: true,
                maintainBottomViewPadding: !_resizeToAvoidBottomInset
            );
        }
        if (widget.persistentFooterButtons is not null)
        {
            _addIfNonNull(
                childrenLocal,
                new Container(
                    decoration: widget.persistentFooterDecoration
                        ?? new BoxDecoration(
                            border: new Border(top: Divider.createBorderSide(context, width: 1.0))
                        ),
                    child: new SafeArea(
                        top: false,
                        child: new IntrinsicHeight(
                            child: new Padding(
                                padding: EdgeInsets.CreateAll(8),
                                child: new Align(
                                    alignment: widget.persistentFooterAlignment,
                                    child: new OverflowBar(
                                        spacing: 8,
                                        overflowAlignment: OverflowBarAlignment.end,
                                        children: widget.persistentFooterButtons!
                                    )
                                )
                            )
                        )
                    )
                ),
                _ScaffoldSlot__scaffold.persistentFooter,
                removeLeftPadding: false,
                removeTopPadding: true,
                removeRightPadding: false,
                removeBottomPadding: widget.bottomNavigationBar is not null,
                maintainBottomViewPadding: !_resizeToAvoidBottomInset
            );
        }
        if (widget.bottomNavigationBar is not null)
        {
            _addIfNonNull(
                childrenLocal,
                widget.bottomNavigationBar,
                _ScaffoldSlot__scaffold.bottomNavigationBar,
                removeLeftPadding: false,
                removeTopPadding: true,
                removeRightPadding: false,
                removeBottomPadding: false,
                maintainBottomViewPadding: !_resizeToAvoidBottomInset
            );
        }
        _addIfNonNull(
            childrenLocal,
            new _FloatingActionButtonTransition__scaffold(
                fabMoveAnimation: _floatingActionButtonMoveController,
                fabMotionAnimator: _floatingActionButtonAnimator,
                geometryNotifier: _geometryNotifier,
                currentController: _floatingActionButtonVisibilityController,
                child: widget.floatingActionButton
            ),
            _ScaffoldSlot__scaffold.floatingActionButton,
            removeLeftPadding: true,
            removeTopPadding: true,
            removeRightPadding: true,
            removeBottomPadding: true
        );
        Widget? statusBarLocal = themeData.platform switch
        {
            TargetPlatform.iOS => widget.primary
                ? new _HitTestableAtOrigin__scaffold(_statusBarKey)
                : null,
            TargetPlatform.macOS => widget.primary
                ? new _HitTestableAtOrigin__scaffold(_statusBarKey)
                : null,
            TargetPlatform.android or TargetPlatform.fuchsia or TargetPlatform.linux =>
                DartRuntimePrimitives.ConvertValue<_HitTestableAtOrigin__scaffold>(null),
            TargetPlatform.windows =>
                DartRuntimePrimitives.ConvertValue<_HitTestableAtOrigin__scaffold>(null),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        _addIfNonNull(
            childrenLocal,
            statusBarLocal,
            _ScaffoldSlot__scaffold.statusBar,
            removeLeftPadding: false,
            removeTopPadding: true,
            removeRightPadding: false,
            removeBottomPadding: true
        );
        if (_endDrawerOpened.value)
        {
            _buildDrawer(childrenLocal, textDirectionLocal);
            _buildEndDrawer(childrenLocal, textDirectionLocal);
        }
        else
        {
            _buildEndDrawer(childrenLocal, textDirectionLocal);
            _buildDrawer(childrenLocal, textDirectionLocal);
        }
        EdgeInsets minInsetsLocal = MediaQuery
            .paddingOf(context)
            .copyWith(
                bottom: _resizeToAvoidBottomInset ? MediaQuery.viewInsetsOf(context).bottom : 0.0
            );
        EdgeInsets minViewPaddingLocal = MediaQuery
            .viewPaddingOf(context)
            .copyWith(
                bottom: (
                    _resizeToAvoidBottomInset && (MediaQuery.viewInsetsOf(context).bottom != 0.0)
                )
                    ? 0.0
                    : null
            );
        return new _ScaffoldScope__scaffold(
            hasDrawer: hasDrawer,
            geometryNotifier: _geometryNotifier,
            child: new ScrollNotificationObserver(
                child: new Material(
                    color: widget.backgroundColor ?? themeData.scaffoldBackgroundColor,
                    child: new Builder(
                        builder: (context) =>
                        {
                            return new Actions(
                                actions: new DartMap<Type, dynamic>
                                {
                                    [typeof(DismissIntent)] = new _DismissDrawerAction__scaffold(
                                        context
                                    ),
                                },
                                child: new CustomMultiChildLayout(
                                    @delegate: new _ScaffoldLayout__scaffold(
                                        extendBody: widget.extendBody,
                                        extendBodyBehindAppBar: widget.extendBodyBehindAppBar,
                                        minInsets: minInsetsLocal,
                                        minViewPadding: minViewPaddingLocal,
                                        currentFloatingActionButtonLocation: _floatingActionButtonLocation!,
                                        floatingActionButtonMoveAnimation: _floatingActionButtonMoveController,
                                        floatingActionButtonMotionAnimator: _floatingActionButtonAnimator,
                                        geometryNotifier: _geometryNotifier,
                                        previousFloatingActionButtonLocation: _previousFloatingActionButtonLocation!,
                                        textDirection: textDirectionLocal,
                                        isSnackBarFloating: isSnackBarFloatingLocal,
                                        extendBodyBehindMaterialBanner: extendBodyBehindMaterialBannerLocal,
                                        snackBarWidth: snackBarWidthLocal
                                    ),
                                    children: childrenLocal.Cast<Widget>().ToList()
                                )
                            );
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
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

    public virtual RestorationBucket? bucket => _bucket;

    public virtual void didToggleBucket(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(
            () =>
                (property._restorationId is null)
                || (_debugDoingRestore && (property._restorationId == restorationId)),
            () => (object?)$"Property is already registered under {property._restorationId}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                _debugDoingRestore
                || !_properties.Keys.map((r) => r._restorationId).contains(restorationId),
            () => (object?)$"\"{restorationId}\" is already registered to another property."
        );
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue
            ? property.fromPrimitivesObject(bucket!.read<object>(restorationId))
            : property.createDefaultValueObject();
        if (!property.isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if (bucket is null)
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener(listener);
            _properties[property] = listener;
        }
        DartRuntimePrimitives.Assert(() =>
            (property._restorationId == restorationId)
            && Equals(property._owner, this)
            && _properties.ContainsKey(property)
        );
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
            return true;
        });
    }

    public virtual void unregisterFromRestoration(IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => Equals(property._owner, this));
        _bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((_currentParent is null) || (_bucket?.restorationId == restorationId) || restorePending)
        {
            return;
        }
        RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(
            parent: _currentParent,
            restorePending: false
        );
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            DartRuntimePrimitives.Assert(() => (_bucket is null) || (oldBucket is null));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (_firstRestorePending)
            {
                return true;
            }
            if (restorationId is null)
            {
                return false;
            }
            RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent))
                && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore =>
        DartRuntimePrimitives.ConvertValue<bool>(
            _debugPropertiesWaitingForReregistration is not null
        );

    public virtual void _doRestore(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
            return true;
        });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
        {
            if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        (
                            (Func<List<DiagnosticsNode>>)(
                                () =>
                                {
                                    var __collection41817 = new List<DiagnosticsNode>();
                                    __collection41817.Add(
                                        new ErrorSummary(
                                            "Previously registered RestorableProperties must be re-registered in \"restoreState\"."
                                        )
                                    );
                                    __collection41817.Add(
                                        new ErrorDescription(
                                            $"The RestorableProperties with the following IDs were not re-registered to {this} when "
                                                + "\"restoreState\" was called:"
                                        )
                                    );
                                    __collection41817.AddRange(
                                        _debugPropertiesWaitingForReregistration!.map<
                                            IRestorableProperty,
                                            DiagnosticsNode
                                        >(
                                            (property) =>
                                                new ErrorDescription(
                                                    $" * {property._restorationId}"
                                                )
                                        )
                                    );
                                    return __collection41817;
                                }
                            )
                        )()
                    )
                );
            }
            _debugPropertiesWaitingForReregistration = null;
            return true;
        });
    }

    public virtual bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(
                newBucket: null,
                restorePending: restorePending
            );
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(
                newBucket: newBucketLocal,
                restorePending: restorePending
            );
            DartRuntimePrimitives.Assert(() => Equals(_bucket, newBucketLocal));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => _bucket is not null);
        DartRuntimePrimitives.Assert(() => !restorePending);
        _bucket!.rename(restorationId!);
        parent.adoptChild(_bucket!);
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach(
                    (__arg0) => ((Action<IRestorableProperty>)_updateProperty)(__arg0)
                );
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _updateProperty(IRestorableProperty property)
    {
        if (property.enabled)
        {
            _bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            _bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(IRestorableProperty property)
    {
        Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
        {
            _debugPropertiesWaitingForReregistration?.Remove(property);
            return true;
        });
        property.removeListener(listener);
        property._unregister();
    }
}

internal class _DismissDrawerAction__scaffold : DismissAction
{
    public virtual BuildContext context { get; private set; } = default!;

    internal _DismissDrawerAction__scaffold(BuildContext context)
    {
        this.context = context;
    }

    public override bool isEnabled(DismissIntent intent, BuildContext? context = null)
    {
        ScaffoldState scaffold = Scaffold.of(this.context);
        return (scaffold.isDrawerOpen || scaffold.isEndDrawerOpen)
            && scaffold.isDrawerBarrierDismissible;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override object? invoke(DismissIntent intent, BuildContext? context = null)
    {
        ScaffoldState scaffold = Scaffold.of(this.context);
        if (isEnabled(intent))
        {
            scaffold.closeDrawer();
            scaffold.closeEndDrawer();
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class ScaffoldFeatureController<T, U>
    where T : Widget
{
    internal virtual T _widget { get; private set; } = default!;
    internal virtual Completer<U> _completer { get; private set; } = default!;
    public virtual Action close { get; private set; } = default!;
    public virtual Action<Action>? setState { get; private set; }

    public ScaffoldFeatureController(
        T _widget,
        Completer<U> _completer,
        Action close,
        Action<Action>? setState
    )
    {
        this._widget = _widget;
        this._completer = _completer;
        this.close = close;
        this.setState = setState;
    }

    public virtual Future<U> closed => _completer.future;
}

public class _StandardBottomSheet__scaffold : StatefulWidget
{
    public virtual AnimationController animationController { get; private set; } = default!;
    public virtual bool enableDrag { get; private set; } = default!;
    public virtual bool? showDragHandle { get; private set; }
    public virtual Action? onClosing { get; private set; }
    public virtual Action? onDismissed { get; private set; }
    public virtual Action? onDispose { get; private set; }
    public virtual Func<BuildContext, Widget> builder { get; private set; } = default!;
    public virtual bool isPersistent { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }

    internal _StandardBottomSheet__scaffold(
        Key? key = null,
        AnimationController animationController = default!,
        bool enableDrag = true,
        bool? showDragHandle = null,
        Action? onClosing = default!,
        Action? onDismissed = default!,
        Func<BuildContext, Widget> builder = default!,
        bool isPersistent = false,
        Color? backgroundColor = null,
        double? elevation = null,
        ShapeBorder? shape = null,
        Clip? clipBehavior = null,
        BoxConstraints? constraints = null,
        Action? onDispose = null
    )
        : base(key: key)
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

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _StandardBottomSheetState__scaffold());
}

public class _StandardBottomSheetState__scaffold : State<_StandardBottomSheet__scaffold>
{
    public virtual ParametricCurve<double> animationCurve { get; set; } =
        ScaffoldLibrary._standardBottomSheetCurve;

    public override void initState()
    {
        base.initState();
        DartRuntimePrimitives.Assert(() => widget.animationController.isForwardOrCompleted);
        widget.animationController.addStatusListener(_handleStatusChange);
    }

    public override void dispose()
    {
        widget.animationController.removeStatusListener(_handleStatusChange);
        widget.onDispose?.Invoke();
        base.dispose();
    }

    public override void didUpdateWidget(_StandardBottomSheet__scaffold oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() =>
            Equals(widget.animationController, oldWidget.animationController)
        );
    }

    public virtual void close()
    {
        widget.animationController.reverse();
        widget.onClosing?.Invoke();
    }

    internal virtual void _handleDragStart(Gestures.DragStartDetails details)
    {
        animationCurve = DartRuntimePrimitives.ConvertValue<ParametricCurve<double>>(Curves.linear);
    }

    internal virtual void _handleDragEnd(Gestures.DragEndDetails details, bool? isClosing = null)
    {
        animationCurve = DartRuntimePrimitives.ConvertValue<ParametricCurve<double>>(
            new Split(
                widget.animationController.value,
                endCurve: ScaffoldLibrary._standardBottomSheetCurve
            )
        );
    }

    internal virtual void _handleStatusChange(AnimationStatus status)
    {
        if (AnimationStatusMembers.isDismissed(status))
        {
            widget.onDismissed?.Invoke();
        }
    }

    public virtual bool extentChanged(DraggableScrollableNotification notification)
    {
        double extentRemaining = 1.0 - notification.extent;
        ScaffoldState scaffold = Scaffold.of(context);
        if (extentRemaining < ScaffoldLibrary._kBottomSheetDominatesPercentage)
        {
            scaffold._floatingActionButtonVisibilityController.value =
                extentRemaining * ScaffoldLibrary._kBottomSheetDominatesPercentage * 10L;
            double scrimAnimationValue =
                1L - (extentRemaining / ScaffoldLibrary._kBottomSheetDominatesPercentage);
            scaffold.showBodyScrim(true, scrimAnimationValue);
        }
        else
        {
            scaffold._floatingActionButtonVisibilityController.value = 1.0;
            scaffold.showBodyScrim(false, 0.0);
        }
        if (
            (notification.extent == notification.minExtent)
            && (scaffold.widget.bottomSheet is null)
            && notification.shouldCloseOnMinExtent
        )
        {
            close();
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new AnimatedBuilder(
            animation: widget.animationController,
            builder: (context, child) =>
            {
                return new Align(
                    alignment: AlignmentDirectional.topStart,
                    heightFactor: animationCurve.transform(widget.animationController.value),
                    child: child
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: new Widgets.Semantics(
                container: true,
                onDismiss: !widget.isPersistent ? close : null,
                child: new NotificationListener<DraggableScrollableNotification>(
                    onNotification: extentChanged,
                    child: new BottomSheet(
                        animationController: widget.animationController,
                        enableDrag: widget.enableDrag,
                        showDragHandle: widget.showDragHandle,
                        onDragStart: _handleDragStart,
                        onDragEnd: (details, isClosing) => _handleDragEnd(details, isClosing),
                        onClosing: widget.onClosing!,
                        builder: widget.builder,
                        backgroundColor: widget.backgroundColor,
                        elevation: widget.elevation,
                        shape: widget.shape,
                        clipBehavior: widget.clipBehavior,
                        constraints: widget.constraints
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class PersistentBottomSheetController
    : ScaffoldFeatureController<_StandardBottomSheet__scaffold, object?>
{
    internal virtual bool _isLocalHistoryEntry { get; private set; } = default!;

    internal PersistentBottomSheetController(
        _StandardBottomSheet__scaffold widget,
        Completer<object?> completer,
        Action close,
        Action<Action> setState,
        bool _isLocalHistoryEntry
    )
        : base(widget, completer, close, setState)
    {
        this._isLocalHistoryEntry = _isLocalHistoryEntry;
    }
}

internal class _ScaffoldScope__scaffold : InheritedWidget
{
    public virtual bool hasDrawer { get; private set; } = default!;
    public virtual _ScaffoldGeometryNotifier__scaffold geometryNotifier { get; private set; } =
        default!;

    internal _ScaffoldScope__scaffold(
        bool hasDrawer,
        _ScaffoldGeometryNotifier__scaffold geometryNotifier,
        Widget child
    )
        : base(child: child)
    {
        this.hasDrawer = hasDrawer;
        this.geometryNotifier = geometryNotifier;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_ScaffoldScope__scaffold)oldWidget;
        return hasDrawer != __oldWidget.hasDrawer;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _HitTestableAtOrigin__scaffold : StatelessWidget
{
    public virtual GlobalKey<IState> globalKey { get; private set; } = default!;

    internal _HitTestableAtOrigin__scaffold(GlobalKey<IState> globalKey)
    {
        this.globalKey = globalKey;
    }

    public static bool hitTestableAtOrigin(GlobalKey<IState> key)
    {
        var context = ((Element?)key.currentContext)!;
        if (context is null)
        {
            DartRuntimePrimitives.Assert(
                () => false,
                () =>
                    (object?)$"BuildContext associated with {key} is not mounted. "
                    + "If you see this in a test, this is likely because the test was trying "
                    + "to simulate status bar tap on a non-iOS platform"
            );
            return false;
        }
        var renderObjectLocal = ((RenderMetaData?)context.renderObject!)!;
        long viewIdLocal = checked((long)View.of(context).viewId);
        var result = new Gestures.HitTestResult();
        WidgetsBinding.instance.hitTestInView(result, Offset.zero, viewIdLocal);
        return result.path.any((entry) => Equals(entry.target, renderObjectLocal));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new MetaData(
            key: globalKey,
            behavior: HitTestBehavior.translucent,
            child: SizedBox.CreateExpand()
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
