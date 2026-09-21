// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/drawer.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public enum DrawerAlignment
{
    start,
    end,
}

public static partial class DrawerLibrary
{
    internal static double _kWidth = 304.0;
}

public static partial class DrawerLibrary
{
    internal static double _kEdgeDragWidth = 20.0;
}

public static partial class DrawerLibrary
{
    internal static double _kMinFlingVelocity = 365.0;
}

public static partial class DrawerLibrary
{
    internal static Duration _kBaseSettleDuration = Duration.Create(milliseconds: 246L);
}

public class Drawer : StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual double? width { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }

    public Drawer(
        Key? key = null,
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        ShapeBorder? shape = null,
        double? width = null,
        Widget? child = null,
        string? semanticLabel = null,
        Clip? clipBehavior = null
    )
        : base(key: key)
    {
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.shape = shape;
        this.width = width;
        this.child = child;
        this.semanticLabel = semanticLabel;
        this.clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        DrawerThemeData drawerTheme = DrawerTheme.of(context);
        string? labelLocal = PlatformLibrary.defaultTargetPlatform switch
        {
            TargetPlatform.iOS => semanticLabel,
            TargetPlatform.macOS => semanticLabel,
            TargetPlatform.android or TargetPlatform.fuchsia or TargetPlatform.linux =>
                semanticLabel ?? MaterialLocalizations.of(context).drawerLabel,
            TargetPlatform.windows => semanticLabel
                ?? MaterialLocalizations.of(context).drawerLabel,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        var isDrawerStart = !Equals(
            DrawerController.maybeOf(context)?.alignment,
            DrawerAlignment.end
        );
        DrawerThemeData defaults = new _DrawerDefaultsM3__drawer(context);
        ShapeBorder? effectiveShape =
            shape
            ?? (
                isDrawerStart
                    ? (drawerTheme.shape ?? defaults.shape)
                    : (drawerTheme.endShape ?? defaults.endShape)
            );
        return new Widgets.Semantics(
            scopesRoute: true,
            namesRoute: true,
            explicitChildNodes: true,
            label: labelLocal,
            child: new ConstrainedBox(
                constraints: BoxConstraints.CreateExpand(
                    width: (width ?? drawerTheme.width) ?? DrawerLibrary._kWidth
                ),
                child: new Material(
                    color: (backgroundColor ?? drawerTheme.backgroundColor)
                        ?? defaults.backgroundColor,
                    elevation: (elevation ?? drawerTheme.elevation)
                        ?? (
                            defaults.elevation
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ),
                    shadowColor: (shadowColor ?? drawerTheme.shadowColor) ?? defaults.shadowColor,
                    surfaceTintColor: (surfaceTintColor ?? drawerTheme.surfaceTintColor)
                        ?? defaults.surfaceTintColor,
                    shape: effectiveShape,
                    clipBehavior: (effectiveShape is not null)
                        ? (
                            (clipBehavior ?? drawerTheme.clipBehavior)
                            ?? (
                                defaults.clipBehavior
                                ?? throw new global::System.NullReferenceException(
                                    "Dart null assertion failed."
                                )
                            )
                        )
                        : Clip.none,
                    child: child
                )
            )
        );
    }
}

public delegate void DrawerCallback(bool isOpened);

internal class _DrawerControllerScope__drawer : InheritedWidget
{
    public virtual DrawerController controller { get; private set; } = default!;

    internal _DrawerControllerScope__drawer(DrawerController controller, Widget child)
        : base(child: child)
    {
        this.controller = controller;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __old = (_DrawerControllerScope__drawer)oldWidget;
        return !Equals(controller, __old.controller);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DrawerController : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual DrawerAlignment alignment { get; private set; } = default!;
    public virtual Action<bool>? drawerCallback { get; private set; }
    public virtual bool drawerBarrierDismissible { get; private set; } = default!;
    public virtual Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual Color? scrimColor { get; private set; }
    public virtual bool enableOpenDragGesture { get; private set; } = default!;
    public virtual double? edgeDragWidth { get; private set; }
    public virtual bool isDrawerOpen { get; private set; } = default!;

    public DrawerController(
        GlobalKeyBase? key = null,
        Widget child = default!,
        DrawerAlignment alignment = default!,
        bool isDrawerOpen = false,
        Action<bool>? drawerCallback = null,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        Color? scrimColor = null,
        double? edgeDragWidth = null,
        bool enableOpenDragGesture = true,
        bool drawerBarrierDismissible = true
    )
        : base(key: key)
    {
        this.child = child;
        this.alignment = alignment;
        this.isDrawerOpen = isDrawerOpen;
        this.drawerCallback = drawerCallback;
        this.dragStartBehavior = dragStartBehavior;
        this.scrimColor = scrimColor;
        this.edgeDragWidth = edgeDragWidth;
        this.enableOpenDragGesture = enableOpenDragGesture;
        this.drawerBarrierDismissible = drawerBarrierDismissible;
    }

    public static DrawerController? maybeOf(BuildContext context)
    {
        return context
            .dependOnInheritedWidgetOfExactType<_DrawerControllerScope__drawer>()
            ?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DrawerController of(BuildContext context)
    {
        DrawerController? controller = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (controller is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "DrawerController.of() was called with a context that does not "
                            + "contain a DrawerController widget.\n"
                            + "No DrawerController widget ancestor could be found starting from "
                            + "the context that was passed to DrawerController.of(). This can "
                            + "happen because you are using a widget that looks for a DrawerController "
                            + "ancestor, but no such ancestor exists.\n"
                            + "The context used was:\n"
                            + $"  {context}"
                    )
                );
            }
            return true;
        });
        return controller!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new DrawerControllerState());
}

public class DrawerControllerState
    : State<DrawerController>,
        SingleTickerProviderStateMixin<DrawerController>
{
    internal virtual LocalHistoryEntry? _historyEntry { get; set; } = default;
    internal virtual FocusScopeNode _focusScopeNode { get; private set; } = new FocusScopeNode();
    internal virtual AnimationController _controller { get; set; } = default!;
    internal virtual GlobalKey<IState> _drawerKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual bool _previouslyOpened { get; set; } = false;
    internal virtual GlobalKey<IState> _gestureDetectorKey { get; private set; } =
        GlobalKey<IState>.Create();
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new AnimationController(
            value: widget.isDrawerOpen ? 1.0 : 0.0,
            duration: DrawerLibrary._kBaseSettleDuration,
            vsync: this
        );
        DartRuntimePrimitives.Ignore(
            (
                (Func<AnimationController>)(
                    () =>
                    {
                        var __cascade = _controller;
                        __cascade.addListener(_animationChanged);
                        __cascade.addStatusListener(_animationStatusChanged);
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override void dispose()
    {
        _historyEntry?.remove();
        _controller.dispose();
        _focusScopeNode.dispose();
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

    public override void didUpdateWidget(DrawerController oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (AnimationStatusMembers.isAnimating(_controller.status))
        {
            return;
        }
        if (widget.isDrawerOpen != oldWidget.isDrawerOpen)
        {
            _controller.value = widget.isDrawerOpen ? 1.0 : 0.0;
        }
    }

    internal virtual void _animationChanged()
    {
        setState(() => { });
    }

    internal virtual void _ensureHistoryEntry()
    {
        if (_historyEntry is null)
        {
            IModalRoute? route = ModalRoute<object>.untypedOf(context);
            if (route is not null)
            {
                _historyEntry = new LocalHistoryEntry(
                    onRemove: () => _handleHistoryEntryRemoved(),
                    impliesAppBarDismissal: false
                );
                route.addLocalHistoryEntry(_historyEntry!);
                FocusScope.of(context).setFirstFocus(_focusScopeNode);
            }
        }
    }

    internal virtual void _animationStatusChanged(AnimationStatus status)
    {
        switch (status)
        {
            case AnimationStatus.forward:
            {
                _ensureHistoryEntry();
                break;
            }
            case AnimationStatus.reverse:
            {
                _historyEntry?.remove();
                _historyEntry = null;
                break;
            }
            case AnimationStatus.dismissed:
            case AnimationStatus.completed:
            {
                break;
            }
        }
    }

    internal virtual void _handleHistoryEntryRemoved()
    {
        _historyEntry = null;
        close();
    }

    internal virtual void _handleDragDown(Gestures.DragDownDetails details)
    {
        _controller.stop();
        _ensureHistoryEntry();
    }

    internal virtual void _handleDragCancel()
    {
        if (_controller.isDismissed || _controller.isAnimating)
        {
            return;
        }
        if (_controller.value < 0.5)
        {
            close();
        }
        else
        {
            open();
        }
    }

    internal virtual double _width
    {
        get
        {
            var box = ((RenderBox?)_drawerKey.currentContext?.findRenderObject())!;
            return box?.size.width ?? DrawerLibrary._kWidth;
        }
    }
    internal virtual long _directionFactor
    {
        get
        {
            return (Directionality.of(context), widget.alignment) switch
            {
                (TextDirection.rtl, DrawerAlignment.start) => -1L,
                (TextDirection.rtl, DrawerAlignment.end) => 1L,
                (TextDirection.ltr, DrawerAlignment.start) => 1L,
                (TextDirection.ltr, DrawerAlignment.end) => -1L,
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
        }
    }

    internal virtual void _move(Gestures.DragUpdateDetails details)
    {
        _controller.value +=
            (
                details.primaryDelta
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
            / _width
            * _directionFactor;
        bool opened = _controller.value > 0.5;
        if ((opened != _previouslyOpened) && (widget.drawerCallback is not null))
        {
            widget.drawerCallback!(opened);
        }
        _previouslyOpened = opened;
    }

    internal virtual void _settle(Gestures.DragEndDetails details)
    {
        if (_controller.isDismissed)
        {
            return;
        }
        double xVelocity = details.velocity.pixelsPerSecond.dx;
        if (xVelocity.abs() >= Bottom_sheetLibrary._kMinFlingVelocity)
        {
            double visualVelocity = xVelocity / _width * _directionFactor;
            _controller.fling(velocity: visualVelocity);
            widget.drawerCallback?.Invoke(visualVelocity > 0.0);
        }
        else
        {
            if (_controller.value < 0.5)
            {
                close();
            }
            else
            {
                open();
            }
        }
    }

    public virtual void open()
    {
        _controller.fling();
        widget.drawerCallback?.Invoke(true);
    }

    public virtual void close()
    {
        _controller.fling(velocity: -1.0);
        widget.drawerCallback?.Invoke(false);
    }

    internal virtual AlignmentDirectional _drawerOuterAlignment =>
        widget.alignment switch
        {
            DrawerAlignment.start => AlignmentDirectional.centerStart,
            DrawerAlignment.end => AlignmentDirectional.centerEnd,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
    internal virtual AlignmentDirectional _drawerInnerAlignment =>
        widget.alignment switch
        {
            DrawerAlignment.start => AlignmentDirectional.centerEnd,
            DrawerAlignment.end => AlignmentDirectional.centerStart,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };

    internal virtual Widget _buildDrawer(BuildContext context)
    {
        bool isDesktop = Theme.of(context).platform switch
        {
            TargetPlatform.android or TargetPlatform.iOS => false,
            TargetPlatform.fuchsia => false,
            TargetPlatform.macOS or TargetPlatform.linux => true,
            TargetPlatform.windows => true,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        double dragAreaWidth =
            widget.edgeDragWidth
            ?? (
                DrawerLibrary._kEdgeDragWidth
                + (
                    (widget.alignment, Directionality.of(context)) switch
                    {
                        (DrawerAlignment.start, TextDirection.ltr) => MediaQuery
                            .paddingOf(context)
                            .left,
                        (DrawerAlignment.start, TextDirection.rtl) => MediaQuery
                            .paddingOf(context)
                            .right,
                        (DrawerAlignment.end, TextDirection.rtl) => MediaQuery
                            .paddingOf(context)
                            .left,
                        (DrawerAlignment.end, TextDirection.ltr) => MediaQuery
                            .paddingOf(context)
                            .right,
                        _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                            throw new InvalidOperationException(
                                "Non-exhaustive Dart switch value."
                            ),
                    }
                )
            );
        if (_controller.isDismissed)
        {
            if (widget.enableOpenDragGesture && !isDesktop)
            {
                return new Align(
                    alignment: _drawerOuterAlignment,
                    child: new GestureDetector(
                        key: _gestureDetectorKey,
                        onHorizontalDragUpdate: _move,
                        onHorizontalDragEnd: _settle,
                        behavior: HitTestBehavior.translucent,
                        excludeFromSemantics: true,
                        dragStartBehavior: widget.dragStartBehavior,
                        child: new LimitedBox(
                            maxHeight: 0.0,
                            child: new SizedBox(
                                width: dragAreaWidth,
                                height: double.PositiveInfinity
                            )
                        )
                    )
                );
            }
            else
            {
                return SizedBox.CreateShrink();
            }
        }
        else
        {
            bool platformHasBackButton = PlatformLibrary.defaultTargetPlatform switch
            {
                TargetPlatform.android => true,
                TargetPlatform.iOS
                or TargetPlatform.macOS
                or TargetPlatform.fuchsia
                or TargetPlatform.linux => false,
                TargetPlatform.windows => false,
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            Color scrimColorLocal =
                (widget.scrimColor ?? DrawerTheme.of(context).scrimColor) ?? Colors.black54;
            Color effectiveScrimColor = scrimColorLocal.withValues(
                alpha: scrimColorLocal.a * _controller.value
            );
            Widget drawerScrim = new ColoredBox(
                color: effectiveScrimColor,
                child: new LimitedBox(maxWidth: 0.0, maxHeight: 0.0, child: SizedBox.CreateExpand())
            );
            Widget childLocal = new _DrawerControllerScope__drawer(
                controller: widget,
                child: new RepaintBoundary(
                    child: new Stack(
                        children: new List<Widget>
                        {
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new BlockSemantics(
                                    child: new ExcludeSemantics(
                                        excluding: platformHasBackButton,
                                        child: new GestureDetector(
                                            onTap: widget.drawerBarrierDismissible ? close : null,
                                            child: new Widgets.Semantics(
                                                label: MaterialLocalizations
                                                    .of(context)
                                                    .modalBarrierDismissLabel,
                                                child: drawerScrim
                                            )
                                        )
                                    )
                                )
                            ),
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new Align(
                                    alignment: _drawerOuterAlignment,
                                    child: new Align(
                                        alignment: _drawerInnerAlignment,
                                        widthFactor: _controller.value,
                                        child: new RepaintBoundary(
                                            child: new FocusScope(
                                                key: _drawerKey,
                                                node: _focusScopeNode,
                                                child: widget.child
                                            )
                                        )
                                    )
                                )
                            ),
                        }
                    )
                )
            );
            if (isDesktop)
            {
                return childLocal;
            }
            return new GestureDetector(
                key: _gestureDetectorKey,
                onHorizontalDragDown: _handleDragDown,
                onHorizontalDragUpdate: _move,
                onHorizontalDragEnd: _settle,
                onHorizontalDragCancel: () => _handleDragCancel(),
                excludeFromSemantics: true,
                dragStartBehavior: widget.dragStartBehavior,
                child: childLocal
            );
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        return ListTileTheme.merge(style: ListTileStyle.drawer, child: _buildDrawer(context));
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

internal class _DrawerDefaultsM3__drawer : DrawerThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    private bool __late_direction_initialized;
    private TextDirection __late_direction = default!;
    public virtual TextDirection direction
    {
        get
        {
            if (!__late_direction_initialized)
            {
                __late_direction = Directionality.of(context);
                __late_direction_initialized = true;
            }
            return __late_direction;
        }
    }

    internal _DrawerDefaultsM3__drawer(BuildContext context)
        : base(elevation: 1.0, clipBehavior: Clip.hardEdge)
    {
        this.context = context;
    }

    public override Color? backgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            Theme.of(context).colorScheme.surfaceContainerLow
        );
    public override Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? shadowColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override ShapeBorder? shape =>
        DartRuntimePrimitives.ConvertValue<ShapeBorder>(
            new RoundedRectangleBorder(
                borderRadius: BorderRadiusDirectional
                    .CreateHorizontal(end: Radius.circular(16.0))
                    .resolve(direction)
            )
        );
    public override ShapeBorder? endShape =>
        DartRuntimePrimitives.ConvertValue<ShapeBorder>(
            new RoundedRectangleBorder(
                borderRadius: BorderRadiusDirectional
                    .CreateHorizontal(start: Radius.circular(16.0))
                    .resolve(direction)
            )
        );
}
