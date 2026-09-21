// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/bottom_sheet.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Bottom_sheetLibrary
{
    internal static Duration _kBottomSheetEnterDuration = Duration.Create(milliseconds: 250L);
}

public static partial class Bottom_sheetLibrary
{
    internal static Duration _kBottomSheetExitDuration = Duration.Create(milliseconds: 200L);
}

public static partial class Bottom_sheetLibrary
{
    internal static Curve _kModalBottomSheetCurve = Easing.legacyDecelerate;
}

public static partial class Bottom_sheetLibrary
{
    internal static double _kMinFlingVelocity = 700.0;
}

public static partial class Bottom_sheetLibrary
{
    internal static double _kCloseProgressThreshold = 0.5;
}

public static partial class Bottom_sheetLibrary
{
    internal static double _kDefaultScrollControlDisabledMaxHeightRatio = 9.0 / 16.0;
}

public delegate void BottomSheetDragStartHandler(Gestures.DragStartDetails details);

public delegate void BottomSheetDragEndHandler(Gestures.DragEndDetails details, bool isClosing);

public class BottomSheet : StatefulWidget
{
    public virtual AnimationController? animationController { get; private set; }
    public virtual Action onClosing { get; private set; } = default!;
    public virtual Func<BuildContext, Widget> builder { get; private set; } = default!;
    public virtual bool enableDrag { get; private set; } = default!;
    public virtual bool? showDragHandle { get; private set; }
    public virtual Color? dragHandleColor { get; private set; }
    public virtual Size? dragHandleSize { get; private set; }
    public virtual Action<Gestures.DragStartDetails>? onDragStart { get; private set; }
    public virtual BottomSheetDragEndHandler? onDragEnd { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }

    public BottomSheet(
        Key? key = null,
        AnimationController? animationController = null,
        bool enableDrag = true,
        bool? showDragHandle = null,
        Color? dragHandleColor = null,
        Size? dragHandleSize = null,
        Action<Gestures.DragStartDetails>? onDragStart = null,
        BottomSheetDragEndHandler? onDragEnd = null,
        Color? backgroundColor = null,
        Color? shadowColor = null,
        double? elevation = null,
        ShapeBorder? shape = null,
        Clip? clipBehavior = null,
        BoxConstraints? constraints = null,
        Action onClosing = default!,
        Func<BuildContext, Widget> builder = default!
    )
        : base(key: key)
    {
        this.animationController = animationController;
        this.enableDrag = enableDrag;
        this.showDragHandle = showDragHandle;
        this.dragHandleColor = dragHandleColor;
        this.dragHandleSize = dragHandleSize;
        this.onDragStart = onDragStart;
        this.onDragEnd = onDragEnd;
        this.backgroundColor = backgroundColor;
        this.shadowColor = shadowColor;
        this.elevation = elevation;
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.constraints = constraints;
        this.onClosing = onClosing;
        this.builder = builder;
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _BottomSheetState__bottom_sheet());

    public static AnimationController createAnimationController(
        Scheduler.TickerProvider vsync,
        AnimationStyle? sheetAnimationStyle = null
    )
    {
        return new AnimationController(
            duration: sheetAnimationStyle?.duration
                ?? Bottom_sheetLibrary._kBottomSheetEnterDuration,
            reverseDuration: sheetAnimationStyle?.reverseDuration
                ?? Bottom_sheetLibrary._kBottomSheetExitDuration,
            debugLabel: "BottomSheet",
            vsync: vsync
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _BottomSheetState__bottom_sheet : State<BottomSheet>
{
    internal virtual GlobalKey<IState> _childKey { get; private set; } =
        GlobalKey<IState>.Create(debugLabel: "BottomSheet child");
    public virtual HashSet<WidgetState> dragHandleStates { get; set; } = new HashSet<WidgetState>();

    internal virtual double _childHeight
    {
        get
        {
            var renderBox = ((RenderBox?)_childKey.currentContext!.findRenderObject()!)!;
            return renderBox.size.height;
        }
    }
    internal virtual bool _dismissUnderway =>
        DartRuntimePrimitives.ConvertValue<bool>(
            Equals(widget.animationController!.status, AnimationStatus.reverse)
        );

    internal virtual void _handleDragStart(Gestures.DragStartDetails details)
    {
        setState(() =>
        {
            dragHandleStates.Add(WidgetState.dragged);
        });
        widget.onDragStart?.Invoke(details);
    }

    internal virtual void _handleDragUpdate(Gestures.DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(
            () =>
                (widget.enableDrag || (widget.showDragHandle ?? false))
                && (widget.animationController is not null),
            () =>
                (object?)
                    "'BottomSheet.animationController' cannot be null when 'BottomSheet.enableDrag' or 'BottomSheet.showDragHandle' is true. "
                + "Use 'BottomSheet.createAnimationController' to create one, or provide another AnimationController."
        );
        if (_dismissUnderway)
        {
            return;
        }
        widget.animationController!.value -=
            DartRuntimePrimitives.RequireValue(details.primaryDelta) / _childHeight;
    }

    internal virtual void _handleDragEnd(Gestures.DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(
            () =>
                (widget.enableDrag || (widget.showDragHandle ?? false))
                && (widget.animationController is not null),
            () =>
                (object?)
                    "'BottomSheet.animationController' cannot be null when 'BottomSheet.enableDrag' or 'BottomSheet.showDragHandle' is true. "
                + "Use 'BottomSheet.createAnimationController' to create one, or provide another AnimationController."
        );
        if (_dismissUnderway)
        {
            return;
        }
        setState(() =>
        {
            dragHandleStates.Remove(WidgetState.dragged);
        });
        var isClosing = false;
        if (details.velocity.pixelsPerSecond.dy > Bottom_sheetLibrary._kMinFlingVelocity)
        {
            double flingVelocity = -details.velocity.pixelsPerSecond.dy / _childHeight;
            if (widget.animationController!.value > 0.0)
            {
                widget.animationController!.fling(velocity: flingVelocity);
            }
            if (flingVelocity < 0.0)
            {
                isClosing = true;
            }
        }
        else
        {
            if (widget.animationController!.value < Bottom_sheetLibrary._kCloseProgressThreshold)
            {
                if (widget.animationController!.value > 0.0)
                {
                    widget.animationController!.fling(velocity: -1.0);
                }
                isClosing = true;
            }
            else
            {
                widget.animationController!.forward();
            }
        }
        widget.onDragEnd?.Invoke(details, isClosing);
        if (isClosing)
        {
            widget.onClosing();
        }
    }

    public virtual bool extentChanged(DraggableScrollableNotification notification)
    {
        if ((notification.extent == notification.minExtent) && notification.shouldCloseOnMinExtent)
        {
            widget.onClosing();
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleDragHandleHover(bool hovering)
    {
        if (hovering != dragHandleStates.Contains(WidgetState.hovered))
        {
            setState(() =>
            {
                if (hovering)
                {
                    dragHandleStates.Add(WidgetState.hovered);
                }
                else
                {
                    dragHandleStates.Remove(WidgetState.hovered);
                }
            });
        }
    }

    public override Widget build(BuildContext context)
    {
        BottomSheetThemeData bottomSheetThemeLocal = Theme.of(context).bottomSheetTheme;
        BottomSheetThemeData defaults = new _BottomSheetDefaultsM3__bottom_sheet(context);
        BoxConstraints? constraintsLocal =
            (widget.constraints ?? bottomSheetThemeLocal.constraints) ?? defaults.constraints;
        Color? colorLocal =
            (widget.backgroundColor ?? bottomSheetThemeLocal.backgroundColor)
            ?? defaults.backgroundColor;
        Color? surfaceTintColorLocal =
            bottomSheetThemeLocal.surfaceTintColor ?? defaults.surfaceTintColor;
        Color? shadowColorLocal =
            (widget.shadowColor ?? bottomSheetThemeLocal.shadowColor) ?? defaults.shadowColor;
        double elevationLocal =
            ((widget.elevation ?? bottomSheetThemeLocal.elevation) ?? defaults.elevation) ?? 0;
        ShapeBorder? shapeLocal = (widget.shape ?? bottomSheetThemeLocal.shape) ?? defaults.shape;
        Clip clipBehaviorLocal =
            (widget.clipBehavior ?? bottomSheetThemeLocal.clipBehavior) ?? Clip.none;
        bool showDragHandleLocal =
            widget.showDragHandle
            ?? (widget.enableDrag && (bottomSheetThemeLocal.showDragHandle ?? false));
        Widget? dragHandle = default!;
        if (showDragHandleLocal)
        {
            dragHandle = DartRuntimePrimitives.ConvertValue<Widget>(
                new _DragHandle__bottom_sheet(
                    onSemanticsTap: () => widget.onClosing(),
                    handleHover: _handleDragHandleHover,
                    states: dragHandleStates,
                    dragHandleColor: widget.dragHandleColor,
                    dragHandleSize: widget.dragHandleSize
                )
            );
            if (!widget.enableDrag)
            {
                dragHandle = DartRuntimePrimitives.ConvertValue<Widget>(
                    new _BottomSheetGestureDetector__bottom_sheet(
                        onVerticalDragStart: _handleDragStart,
                        onVerticalDragUpdate: _handleDragUpdate,
                        onVerticalDragEnd: _handleDragEnd,
                        child: dragHandle
                    )
                );
            }
        }
        Widget bottomSheet = new Material(
            key: _childKey,
            color: colorLocal,
            elevation: elevationLocal,
            surfaceTintColor: surfaceTintColorLocal,
            shadowColor: shadowColorLocal,
            shape: shapeLocal,
            clipBehavior: clipBehaviorLocal,
            child: new NotificationListener<DraggableScrollableNotification>(
                onNotification: extentChanged,
                child: !showDragHandleLocal
                    ? widget.builder(context)
                    : new Stack(
                        alignment: Alignment.topCenter,
                        children: new List<Widget>
                        {
                            DartRuntimePrimitives.ConvertValue<Widget>(dragHandle!),
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new Padding(
                                    padding: EdgeInsets.CreateOnly(
                                        top: ConstantsLibrary.kMinInteractiveDimension
                                    ),
                                    child: widget.builder(context)
                                )
                            ),
                        }
                    )
            )
        );
        if (constraintsLocal is not null)
        {
            bottomSheet = DartRuntimePrimitives.ConvertValue<Widget>(
                new Align(
                    alignment: Alignment.bottomCenter,
                    heightFactor: 1.0,
                    child: new ConstrainedBox(constraints: constraintsLocal, child: bottomSheet)
                )
            );
        }
        return !widget.enableDrag
            ? bottomSheet
            : new _BottomSheetGestureDetector__bottom_sheet(
                onVerticalDragStart: _handleDragStart,
                onVerticalDragUpdate: _handleDragUpdate,
                onVerticalDragEnd: _handleDragEnd,
                child: bottomSheet
            );
    }
}

internal class _DragHandle__bottom_sheet : StatelessWidget
{
    public virtual Action? onSemanticsTap { get; private set; }
    public virtual Action<bool> handleHover { get; private set; } = default!;
    public virtual HashSet<WidgetState> states { get; private set; } = default!;
    public virtual Color? dragHandleColor { get; private set; }
    public virtual Size? dragHandleSize { get; private set; }

    internal _DragHandle__bottom_sheet(
        Action? onSemanticsTap,
        Action<bool> handleHover,
        HashSet<WidgetState> states,
        Color? dragHandleColor = null,
        Size? dragHandleSize = null
    )
    {
        this.onSemanticsTap = onSemanticsTap;
        this.handleHover = handleHover;
        this.states = states;
        this.dragHandleColor = dragHandleColor;
        this.dragHandleSize = dragHandleSize;
    }

    public override Widget build(BuildContext context)
    {
        BottomSheetThemeData bottomSheetThemeLocal = Theme.of(context).bottomSheetTheme;
        BottomSheetThemeData m3Defaults = new _BottomSheetDefaultsM3__bottom_sheet(context);
        Size handleSize =
            (dragHandleSize ?? bottomSheetThemeLocal.dragHandleSize)
            ?? DartRuntimePrimitives.RequireValue(m3Defaults.dragHandleSize);
        return new MouseRegion(
            onEnter: (@event) =>
            {
                handleHover(true);
            },
            onExit: (@event) =>
            {
                handleHover(false);
            },
            child: new Widgets.Semantics(
                label: MaterialLocalizations.of(context).modalBarrierDismissLabel,
                container: true,
                button: true,
                onTap: onSemanticsTap,
                child: new SizedBox(
                    width: Math.Max(handleSize.width, ConstantsLibrary.kMinInteractiveDimension),
                    height: Math.Max(handleSize.height, ConstantsLibrary.kMinInteractiveDimension),
                    child: new Center(
                        child: new Container(
                            height: handleSize.height,
                            width: handleSize.width,
                            decoration: new BoxDecoration(
                                borderRadius: BorderRadius.CreateCircular(handleSize.height / 2L),
                                color: (
                                    WidgetStateProperty.resolveAs(dragHandleColor, states)
                                    ?? WidgetStateProperty.resolveAs(
                                        bottomSheetThemeLocal.dragHandleColor,
                                        states
                                    )
                                ) ?? m3Defaults.dragHandleColor
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _BottomSheetLayoutWithSizeListener__bottom_sheet : SingleChildRenderObjectWidget
{
    public virtual Action<Size> onChildSizeChanged { get; private set; } = default!;
    public virtual double animationValue { get; private set; } = default!;
    public virtual bool isScrollControlled { get; private set; } = default!;
    public virtual double scrollControlDisabledMaxHeightRatio { get; private set; } = default!;

    internal _BottomSheetLayoutWithSizeListener__bottom_sheet(
        Action<Size> onChildSizeChanged,
        double animationValue,
        bool isScrollControlled,
        double scrollControlDisabledMaxHeightRatio,
        Widget? child = null
    )
        : base(child: child)
    {
        this.onChildSizeChanged = onChildSizeChanged;
        this.animationValue = animationValue;
        this.isScrollControlled = isScrollControlled;
        this.scrollControlDisabledMaxHeightRatio = scrollControlDisabledMaxHeightRatio;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderBottomSheetLayoutWithSizeListener__bottom_sheet(
            onChildSizeChanged: onChildSizeChanged,
            animationValue: animationValue,
            isScrollControlled: isScrollControlled,
            scrollControlDisabledMaxHeightRatio: scrollControlDisabledMaxHeightRatio
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderBottomSheetLayoutWithSizeListener__bottom_sheet)renderObject;
        __renderObject.onChildSizeChanged = onChildSizeChanged;
        __renderObject.animationValue = animationValue;
        __renderObject.isScrollControlled = isScrollControlled;
        __renderObject.scrollControlDisabledMaxHeightRatio = scrollControlDisabledMaxHeightRatio;
    }
}

public class _RenderBottomSheetLayoutWithSizeListener__bottom_sheet : RenderShiftedBox
{
    internal virtual Size _lastSize { get; set; } = Size.zero;
    internal virtual Action<Size> _onChildSizeChanged { get; set; } = default!;
    internal virtual double _animationValue { get; set; } = default!;
    internal virtual bool _isScrollControlled { get; set; } = default!;
    internal virtual double _scrollControlDisabledMaxHeightRatio { get; set; } = default!;

    internal _RenderBottomSheetLayoutWithSizeListener__bottom_sheet(
        RenderBox? child = null,
        Action<Size> onChildSizeChanged = default!,
        double animationValue = default!,
        bool isScrollControlled = default!,
        double scrollControlDisabledMaxHeightRatio = default!
    )
        : base(child)
    {
        _onChildSizeChanged = onChildSizeChanged;
        _animationValue = animationValue;
        _isScrollControlled = isScrollControlled;
        _scrollControlDisabledMaxHeightRatio = scrollControlDisabledMaxHeightRatio;
    }

    public virtual Action<Size> onChildSizeChanged
    {
        get => _onChildSizeChanged;
        set
        {
            var newCallback = value;
            if (Equals(_onChildSizeChanged, newCallback))
            {
                return;
            }
            _onChildSizeChanged = newCallback;
            markNeedsLayout();
        }
    }
    public virtual double animationValue
    {
        get => _animationValue;
        set
        {
            var newValue = value;
            if (_animationValue == newValue)
            {
                return;
            }
            _animationValue = newValue;
            markNeedsLayout();
        }
    }
    public virtual bool isScrollControlled
    {
        get => _isScrollControlled;
        set
        {
            var newValue = value;
            if (_isScrollControlled == newValue)
            {
                return;
            }
            _isScrollControlled = newValue;
            markNeedsLayout();
        }
    }
    public virtual double scrollControlDisabledMaxHeightRatio
    {
        get => _scrollControlDisabledMaxHeightRatio;
        set
        {
            var newValue = value;
            if (_scrollControlDisabledMaxHeightRatio == newValue)
            {
                return;
            }
            _scrollControlDisabledMaxHeightRatio = newValue;
            markNeedsLayout();
        }
    }

    public override double computeMinIntrinsicWidth(double height) => 0.0;

    public override double computeMaxIntrinsicWidth(double height) => 0.0;

    public override double computeMinIntrinsicHeight(double width) => 0.0;

    public override double computeMaxIntrinsicHeight(double width) => 0.0;

    public override Size computeDryLayout(BoxConstraints constraints) => constraints.biggest;

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        BoxConstraints childConstraints = _getConstraintsForChild(constraints);
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if (result is null)
        {
            return null;
        }
        Size childSize = childConstraints.isTight
            ? childConstraints.smallest
            : childLocal.getDryLayout(childConstraints);
        return DartRuntimePrimitives.RequireValue(result)
            + _getPositionForChild(constraints.biggest, childSize).dy;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BoxConstraints _getConstraintsForChild(BoxConstraints constraints)
    {
        return new BoxConstraints(
            minWidth: constraints.maxWidth,
            maxWidth: constraints.maxWidth,
            maxHeight: isScrollControlled
                ? constraints.maxHeight
                : (constraints.maxHeight * scrollControlDisabledMaxHeightRatio)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Offset _getPositionForChild(Size size, Size childSize)
    {
        return new Offset(0.0, size.height - (childSize.height * animationValue));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = constraints.biggest;
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return;
        }
        BoxConstraints childConstraints = _getConstraintsForChild(constraints);
        DartRuntimePrimitives.Assert(() =>
            childConstraints.debugAssertIsValid(isAppliedConstraint: true)
        );
        childLocal.layout(childConstraints, parentUsesSize: !childConstraints.isTight);
        var childParentData = ((BoxParentData?)childLocal.parentData!)!;
        Size childSize = childConstraints.isTight ? childConstraints.smallest : childLocal.size;
        childParentData.offset = _getPositionForChild(size, childSize);
        if (!Equals(_lastSize, childSize))
        {
            _lastSize = childSize;
            _onChildSizeChanged?.Invoke(_lastSize);
        }
    }
}

public class _ModalBottomSheet__bottom_sheet<T> : StatefulWidget
{
    public virtual ModalBottomSheetRoute<T> route { get; private set; } = default!;
    public virtual bool isScrollControlled { get; private set; } = default!;
    public virtual double scrollControlDisabledMaxHeightRatio { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual bool enableDrag { get; private set; } = default!;
    public virtual bool showDragHandle { get; private set; } = default!;
    public virtual AnimationStyle? animationStyle { get; private set; }

    internal _ModalBottomSheet__bottom_sheet(
        Key? key = null,
        ModalBottomSheetRoute<T> route = default!,
        Color? backgroundColor = null,
        double? elevation = null,
        ShapeBorder? shape = null,
        Clip? clipBehavior = null,
        BoxConstraints? constraints = null,
        bool isScrollControlled = false,
        double? scrollControlDisabledMaxHeightRatio = null,
        bool enableDrag = true,
        bool showDragHandle = false,
        AnimationStyle? animationStyle = null
    )
        : base(key: key)
    {
        double __scrollControlDisabledMaxHeightRatio =
            scrollControlDisabledMaxHeightRatio
            ?? Bottom_sheetLibrary._kDefaultScrollControlDisabledMaxHeightRatio;
        this.route = route;
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.constraints = constraints;
        this.isScrollControlled = isScrollControlled;
        this.scrollControlDisabledMaxHeightRatio = __scrollControlDisabledMaxHeightRatio;
        this.enableDrag = enableDrag;
        this.showDragHandle = showDragHandle;
        this.animationStyle = animationStyle;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _ModalBottomSheetState__bottom_sheet<T>());
}

public class _ModalBottomSheetState__bottom_sheet<T> : State<_ModalBottomSheet__bottom_sheet<T>>
{
    internal virtual ProxyAnimation _sheetAnimation { get; private set; } = default!;
    internal virtual CurvedAnimation _curvedSheetAnimation { get; private set; } = default!;

    public override void initState()
    {
        base.initState();
        _curvedSheetAnimation = new CurvedAnimation(
            parent: widget.route.animation!,
            curve: widget.animationStyle?.curve ?? Bottom_sheetLibrary._kModalBottomSheetCurve,
            reverseCurve: widget.animationStyle?.reverseCurve
                ?? Bottom_sheetLibrary._kModalBottomSheetCurve
        );
        _sheetAnimation = new ProxyAnimation(_curvedSheetAnimation);
    }

    public override void didUpdateWidget(_ModalBottomSheet__bottom_sheet<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => Equals(oldWidget.route, widget.route));
        DartRuntimePrimitives.Assert(() =>
            Equals(
                _curvedSheetAnimation.curve,
                widget.animationStyle?.curve ?? Bottom_sheetLibrary._kModalBottomSheetCurve
            )
        );
        DartRuntimePrimitives.Assert(() =>
            Equals(
                _curvedSheetAnimation.reverseCurve,
                widget.animationStyle?.reverseCurve ?? Bottom_sheetLibrary._kModalBottomSheetCurve
            )
        );
    }

    public override void dispose()
    {
        _sheetAnimation.parent = AnimationsLibrary.kAlwaysDismissedAnimation;
        _curvedSheetAnimation.dispose();
        base.dispose();
    }

    internal virtual string _getRouteLabel(MaterialLocalizations localizations) =>
        PlatformLibrary.defaultTargetPlatform switch
        {
            TargetPlatform.iOS => "",
            TargetPlatform.macOS => "",
            TargetPlatform.android or TargetPlatform.fuchsia or TargetPlatform.linux =>
                localizations.dialogLabel,
            TargetPlatform.windows => localizations.dialogLabel,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };

    internal virtual EdgeInsets _getNewClipDetails(Size topLayerSize)
    {
        return new EdgeInsets(0, 0, 0, topLayerSize.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleDragStart(Gestures.DragStartDetails details)
    {
        _sheetAnimation.parent = widget.route.animation;
    }

    public virtual void handleDragEnd(Gestures.DragEndDetails details, bool? isClosing = null)
    {
        double currentProgress = widget.route.animation!.value;
        _sheetAnimation.parent = DartRuntimePrimitives.ConvertValue<Animation<double>>(
            new CurvedAnimation(
                parent: widget.route.animation!,
                curve: new Split(
                    currentProgress,
                    endCurve: widget.animationStyle?.curve
                        ?? Bottom_sheetLibrary._kModalBottomSheetCurve
                ),
                reverseCurve: new Split(
                    currentProgress,
                    endCurve: widget.animationStyle?.reverseCurve
                        ?? Bottom_sheetLibrary._kModalBottomSheetCurve
                )
            )
        );
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        string routeLabel = _getRouteLabel(localizations);
        return new AnimatedBuilder(
            animation: _sheetAnimation,
            child: new BottomSheet(
                animationController: widget.route._animationController,
                onClosing: () =>
                {
                    if (widget.route.isCurrent)
                    {
                        Navigator.pop<object>(context);
                    }
                },
                builder: widget.route.builder,
                backgroundColor: widget.backgroundColor,
                elevation: widget.elevation,
                shape: widget.shape,
                clipBehavior: widget.clipBehavior,
                constraints: widget.constraints,
                enableDrag: widget.enableDrag,
                showDragHandle: widget.showDragHandle,
                onDragStart: handleDragStart,
                onDragEnd: (details, isClosing) => handleDragEnd(details, isClosing)
            ),
            builder: (context, child) =>
            {
                double animationValueLocal = _sheetAnimation.value;
                return new Widgets.Semantics(
                    scopesRoute: true,
                    namesRoute: true,
                    label: routeLabel,
                    explicitChildNodes: true,
                    child: new ClipRect(
                        child: new _BottomSheetLayoutWithSizeListener__bottom_sheet(
                            onChildSizeChanged: (size) =>
                            {
                                widget.route._didChangeBarrierSemanticsClip(
                                    _getNewClipDetails(size)
                                );
                            },
                            animationValue: animationValueLocal,
                            isScrollControlled: widget.isScrollControlled,
                            scrollControlDisabledMaxHeightRatio: widget.scrollControlDisabledMaxHeightRatio,
                            child: child
                        )
                    )
                );
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class ModalBottomSheetRoute<T> : PopupRoute<T>
{
    public virtual Func<BuildContext, Widget> builder { get; private set; } = default!;
    public virtual CapturedThemes? capturedThemes { get; private set; }
    public virtual bool isScrollControlled { get; private set; } = default!;
    public virtual double scrollControlDisabledMaxHeightRatio { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual Color? modalBarrierColor { get; private set; }
    public virtual bool isDismissible { get; private set; } = default!;
    public virtual bool enableDrag { get; private set; } = default!;
    public virtual bool? showDragHandle { get; private set; }
    public virtual AnimationController? transitionAnimationController { get; private set; }
    public virtual Offset? anchorPoint { get; private set; }
    public virtual bool useSafeArea { get; private set; } = default!;
    public virtual AnimationStyle? sheetAnimationStyle { get; private set; }
    public virtual string? barrierOnTapHint { get; private set; }
    internal virtual ValueNotifier<EdgeInsets> _clipDetailsNotifier { get; private set; } =
        new ValueNotifier<EdgeInsets>(EdgeInsets.zero);
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel
    {
        get => __field_barrierLabel;
    }
    internal virtual AnimationController? _animationController { get; set; } = default;

    public ModalBottomSheetRoute(
        Func<BuildContext, Widget> builder,
        CapturedThemes? capturedThemes = null,
        string? barrierLabel = null,
        string? barrierOnTapHint = null,
        Color? backgroundColor = null,
        double? elevation = null,
        ShapeBorder? shape = null,
        Clip? clipBehavior = null,
        BoxConstraints? constraints = null,
        Color? modalBarrierColor = null,
        bool isDismissible = true,
        bool enableDrag = true,
        bool? showDragHandle = null,
        bool isScrollControlled = default!,
        double? scrollControlDisabledMaxHeightRatio = null,
        RouteSettings? settings = null,
        bool? requestFocus = null,
        AnimationController? transitionAnimationController = null,
        Offset? anchorPoint = null,
        bool useSafeArea = false,
        AnimationStyle? sheetAnimationStyle = null
    )
        : base(settings: settings, requestFocus: requestFocus)
    {
        double __scrollControlDisabledMaxHeightRatio =
            scrollControlDisabledMaxHeightRatio
            ?? Bottom_sheetLibrary._kDefaultScrollControlDisabledMaxHeightRatio;
        this.builder = builder;
        this.capturedThemes = capturedThemes;
        __field_barrierLabel = barrierLabel;
        this.barrierOnTapHint = barrierOnTapHint;
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.constraints = constraints;
        this.modalBarrierColor = modalBarrierColor;
        this.isDismissible = isDismissible;
        this.enableDrag = enableDrag;
        this.showDragHandle = showDragHandle;
        this.isScrollControlled = isScrollControlled;
        this.scrollControlDisabledMaxHeightRatio = __scrollControlDisabledMaxHeightRatio;
        this.transitionAnimationController = transitionAnimationController;
        this.anchorPoint = anchorPoint;
        this.useSafeArea = useSafeArea;
        this.sheetAnimationStyle = sheetAnimationStyle;
    }

    public override void dispose()
    {
        _clipDetailsNotifier.dispose();
        base.dispose();
    }

    internal virtual bool _didChangeBarrierSemanticsClip(EdgeInsets newClipDetails)
    {
        if (Equals(_clipDetailsNotifier.value, newClipDetails))
        {
            return false;
        }
        _clipDetailsNotifier.value = newClipDetails;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Duration transitionDuration =>
        DartRuntimePrimitives.ConvertValue<Duration>(
            (transitionAnimationController?.duration ?? sheetAnimationStyle?.duration)
                ?? Bottom_sheetLibrary._kBottomSheetEnterDuration
        );
    public override Duration reverseTransitionDuration =>
        DartRuntimePrimitives.ConvertValue<Duration>(
            (
                (
                    transitionAnimationController?.reverseDuration
                    ?? transitionAnimationController?.duration
                ) ?? sheetAnimationStyle?.reverseDuration
            ) ?? Bottom_sheetLibrary._kBottomSheetExitDuration
        );
    public override bool barrierDismissible => isDismissible;
    public override Color barrierColor =>
        DartRuntimePrimitives.ConvertValue<Color>(modalBarrierColor ?? Colors.black54);

    public override AnimationController createAnimationController()
    {
        DartRuntimePrimitives.Assert(() => _animationController is null);
        if (transitionAnimationController is not null)
        {
            _animationController = transitionAnimationController;
            willDisposeAnimationController = false;
        }
        else
        {
            _animationController = BottomSheet.createAnimationController(
                navigator!,
                sheetAnimationStyle: sheetAnimationStyle
            );
        }
        return _animationController!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildPage(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation
    )
    {
        Widget content = new DisplayFeatureSubScreen(
            anchorPoint: anchorPoint,
            child: new Builder(
                builder: (context) =>
                {
                    BottomSheetThemeData sheetTheme = Theme.of(context).bottomSheetTheme;
                    BottomSheetThemeData defaults = new _BottomSheetDefaultsM3__bottom_sheet(
                        context
                    );
                    return new _ModalBottomSheet__bottom_sheet<T>(
                        route: this,
                        animationStyle: sheetAnimationStyle,
                        backgroundColor: (
                            (backgroundColor ?? sheetTheme.modalBackgroundColor)
                            ?? sheetTheme.backgroundColor
                        ) ?? defaults.backgroundColor,
                        elevation: (
                            (elevation ?? sheetTheme.modalElevation) ?? sheetTheme.elevation
                        ) ?? defaults.modalElevation,
                        shape: shape,
                        clipBehavior: clipBehavior,
                        constraints: constraints,
                        isScrollControlled: isScrollControlled,
                        scrollControlDisabledMaxHeightRatio: DartRuntimePrimitives.RequireValue(
                            scrollControlDisabledMaxHeightRatio
                        ),
                        enableDrag: enableDrag,
                        showDragHandle: showDragHandle
                            ?? (enableDrag && (sheetTheme.showDragHandle ?? false))
                    );
                }
            )
        );
        Widget bottomSheet = useSafeArea
            ? new global::Doroti.Framework.Widgets.SafeArea(bottom: false, child: content)
            : global::Doroti.Framework.Widgets.MediaQuery.CreateRemovePadding(
                context: context,
                removeTop: true,
                child: content
            );
        bottomSheet = DartRuntimePrimitives.ConvertValue<Widget>(
            new Widgets.Semantics(
                hitTestBehavior: SemanticsHitTestBehavior.opaque,
                child: bottomSheet
            )
        );
        return capturedThemes?.wrap(bottomSheet) ?? bottomSheet;
    }

    public override Widget buildModalBarrier()
    {
        if ((barrierColor.a != 0L) && !offstage)
        {
            DartRuntimePrimitives.Assert(() =>
                !Equals(barrierColor, barrierColor.withValues(alpha: 0.0))
            );
            Animation<Color?> colorLocal = animation!.drive(
                new ColorTween(begin: barrierColor.withValues(alpha: 0.0), end: barrierColor).chain(
                    new CurveTween(curve: barrierCurve)
                )
            );
            return new AnimatedModalBarrier(
                color: colorLocal,
                dismissible: barrierDismissible,
                semanticsLabel: barrierLabel,
                barrierSemanticsDismissible: semanticsDismissible,
                clipDetailsNotifier: _clipDetailsNotifier,
                semanticsOnTapHint: barrierOnTapHint
            );
        }
        else
        {
            return new ModalBarrier(
                dismissible: barrierDismissible,
                semanticsLabel: barrierLabel,
                barrierSemanticsDismissible: semanticsDismissible,
                clipDetailsNotifier: _clipDetailsNotifier,
                semanticsOnTapHint: barrierOnTapHint
            );
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Bottom_sheetLibrary
{
    public static Future<T?> showModalBottomSheet<T>(
        BuildContext context,
        Func<BuildContext, Widget> builder,
        Color? backgroundColor = null,
        string? barrierLabel = null,
        double? elevation = null,
        ShapeBorder? shape = null,
        Clip? clipBehavior = null,
        BoxConstraints? constraints = null,
        Color? barrierColor = null,
        bool isScrollControlled = false,
        double? scrollControlDisabledMaxHeightRatio = null,
        bool useRootNavigator = false,
        bool isDismissible = true,
        bool enableDrag = true,
        bool? showDragHandle = null,
        bool useSafeArea = false,
        RouteSettings? routeSettings = null,
        AnimationController? transitionAnimationController = null,
        Offset? anchorPoint = null,
        AnimationStyle? sheetAnimationStyle = null,
        bool? requestFocus = null
    )
    {
        double __scrollControlDisabledMaxHeightRatio =
            scrollControlDisabledMaxHeightRatio ?? _kDefaultScrollControlDisabledMaxHeightRatio;
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        NavigatorState navigator = Navigator.of(context, rootNavigator: useRootNavigator);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        return navigator.push(
            new ModalBottomSheetRoute<T>(
                builder: builder,
                capturedThemes: InheritedTheme.capture(from: context, to: navigator.context),
                isScrollControlled: isScrollControlled,
                scrollControlDisabledMaxHeightRatio: __scrollControlDisabledMaxHeightRatio,
                barrierLabel: barrierLabel ?? localizations.scrimLabel,
                barrierOnTapHint: localizations.scrimOnTapHint(localizations.bottomSheetLabel),
                backgroundColor: backgroundColor,
                elevation: elevation,
                shape: shape,
                clipBehavior: clipBehavior,
                constraints: constraints,
                isDismissible: isDismissible,
                modalBarrierColor: barrierColor
                    ?? Theme.of(context).bottomSheetTheme.modalBarrierColor,
                enableDrag: enableDrag,
                showDragHandle: showDragHandle,
                settings: routeSettings,
                transitionAnimationController: transitionAnimationController,
                anchorPoint: anchorPoint,
                useSafeArea: useSafeArea,
                sheetAnimationStyle: sheetAnimationStyle,
                requestFocus: requestFocus
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Bottom_sheetLibrary
{
    public static PersistentBottomSheetController showBottomSheet(
        BuildContext context,
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
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasScaffold(context));
        return Scaffold
            .of(context)
            .showBottomSheet(
                builder,
                backgroundColor: backgroundColor,
                elevation: elevation,
                shape: shape,
                clipBehavior: clipBehavior,
                constraints: constraints,
                enableDrag: enableDrag,
                showDragHandle: showDragHandle,
                transitionAnimationController: transitionAnimationController,
                sheetAnimationStyle: sheetAnimationStyle
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _BottomSheetGestureDetector__bottom_sheet : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Action<Gestures.DragStartDetails> onVerticalDragStart { get; private set; } =
        default!;
    public virtual Action<Gestures.DragUpdateDetails> onVerticalDragUpdate { get; private set; } =
        default!;
    public virtual Action<Gestures.DragEndDetails> onVerticalDragEnd { get; private set; } =
        default!;

    internal _BottomSheetGestureDetector__bottom_sheet(
        Widget child,
        Action<Gestures.DragStartDetails> onVerticalDragStart,
        Action<Gestures.DragUpdateDetails> onVerticalDragUpdate,
        Action<Gestures.DragEndDetails> onVerticalDragEnd
    )
    {
        this.child = child;
        this.onVerticalDragStart = onVerticalDragStart;
        this.onVerticalDragUpdate = onVerticalDragUpdate;
        this.onVerticalDragEnd = onVerticalDragEnd;
    }

    public override Widget build(BuildContext context)
    {
        return new RawGestureDetector(
            excludeFromSemantics: true,
            gestures: new DartMap<Type, dynamic>
            {
                [typeof(Gestures.VerticalDragGestureRecognizer)] =
                    new GestureRecognizerFactoryWithHandlers<Gestures.VerticalDragGestureRecognizer>(
                        () => new Gestures.VerticalDragGestureRecognizer(debugOwner: this),
                        (instance) =>
                        {
                            DartRuntimePrimitives.Ignore(
                                (
                                    (Func<Gestures.VerticalDragGestureRecognizer>)(
                                        () =>
                                        {
                                            var __cascade = instance;
                                            __cascade.onStart = onVerticalDragStart;
                                            __cascade.onUpdate = onVerticalDragUpdate;
                                            __cascade.onEnd = onVerticalDragEnd;
                                            __cascade.onlyAcceptDragOnThreshold = true;
                                            return __cascade;
                                        }
                                    )
                                )()
                            );
                        }
                    ),
            },
            child: child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _BottomSheetDefaultsM3__bottom_sheet : BottomSheetThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _BottomSheetDefaultsM3__bottom_sheet(BuildContext context)
        : base(
            elevation: 1.0,
            modalElevation: 1.0,
            shape: new RoundedRectangleBorder(
                borderRadius: BorderRadius.CreateVertical(top: Radius.circular(28.0))
            ),
            constraints: new BoxConstraints(maxWidth: 640)
        )
    {
        this.context = context;
    }

    public override Color? backgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.surfaceContainerLow);
    public override Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? shadowColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? dragHandleColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurfaceVariant);
    public override Size? dragHandleSize =>
        DartRuntimePrimitives.ConvertValue<Size>(new Size(32, 4));
    public override BoxConstraints? constraints => new BoxConstraints(maxWidth: 640.0);
}
