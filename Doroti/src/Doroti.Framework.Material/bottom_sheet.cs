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
    internal static global::Doroti.Framework.Animation.Curve _kModalBottomSheetCurve = Easing.legacyDecelerate;
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

public delegate void BottomSheetDragStartHandler(global::Doroti.Framework.Gestures.DragStartDetails details);

public delegate void BottomSheetDragEndHandler(global::Doroti.Framework.Gestures.DragEndDetails details, bool isClosing);

public class BottomSheet : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Animation.AnimationController? animationController { get; private set; }
    public virtual global::System.Action onClosing { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;
    public virtual bool enableDrag { get; private set; } = default!;
    public virtual bool? showDragHandle { get; private set; }
    public virtual Color? dragHandleColor { get; private set; }
    public virtual Size? dragHandleSize { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onDragStart { get; private set; }
    public virtual BottomSheetDragEndHandler? onDragEnd { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }

    public BottomSheet(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.AnimationController? animationController = null, bool enableDrag = true, bool? showDragHandle = null, Color? dragHandleColor = null, Size? dragHandleSize = null, global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onDragStart = null, BottomSheetDragEndHandler? onDragEnd = null, Color? backgroundColor = null, Color? shadowColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? clipBehavior = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::System.Action onClosing = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder = default!) : base(key: key)
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _BottomSheetState__bottom_sheet());
    public static global::Doroti.Framework.Animation.AnimationController createAnimationController(global::Doroti.Framework.Scheduler.TickerProvider vsync, global::Doroti.Framework.Animation.AnimationStyle? sheetAnimationStyle = null)
    {
        return new global::Doroti.Framework.Animation.AnimationController(duration: sheetAnimationStyle?.duration ?? Bottom_sheetLibrary._kBottomSheetEnterDuration, reverseDuration: sheetAnimationStyle?.reverseDuration ?? Bottom_sheetLibrary._kBottomSheetExitDuration, debugLabel: "BottomSheet", vsync: vsync);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BottomSheetState__bottom_sheet : global::Doroti.Framework.Widgets.State<BottomSheet>
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _childKey { get; private set; } = GlobalKey<IState>.Create(debugLabel: "BottomSheet child");
    public virtual HashSet<global::Doroti.Framework.Widgets.WidgetState> dragHandleStates { get; set; } = new HashSet<global::Doroti.Framework.Widgets.WidgetState>();

    internal virtual double _childHeight
    {
        get
        {
            var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)_childKey.currentContext!.findRenderObject()!)!;
            return renderBox.size.height;
        }
    }
    internal virtual bool _dismissUnderway => DartRuntimePrimitives.ConvertValue<bool>(Equals(widget.animationController!.status, AnimationStatus.reverse));
    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        setState(() =>
        {
            dragHandleStates.Add(WidgetState.dragged);
        });
        widget.onDragStart?.Invoke(details);
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(() => (widget.enableDrag || (widget.showDragHandle ?? false)) && (widget.animationController is not null), () => (object?)"'BottomSheet.animationController' cannot be null when 'BottomSheet.enableDrag' or 'BottomSheet.showDragHandle' is true. " + "Use 'BottomSheet.createAnimationController' to create one, or provide another AnimationController.");
        if (_dismissUnderway)
        {
            return;
        }
        widget.animationController!.value -= DartRuntimePrimitives.RequireValue(details.primaryDelta) / _childHeight;
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => (widget.enableDrag || (widget.showDragHandle ?? false)) && (widget.animationController is not null), () => (object?)"'BottomSheet.animationController' cannot be null when 'BottomSheet.enableDrag' or 'BottomSheet.showDragHandle' is true. " + "Use 'BottomSheet.createAnimationController' to create one, or provide another AnimationController.");
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

    public virtual bool extentChanged(global::Doroti.Framework.Widgets.DraggableScrollableNotification notification)
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        BottomSheetThemeData bottomSheetThemeLocal = Theme.of(context).bottomSheetTheme;
        BottomSheetThemeData defaults = new _BottomSheetDefaultsM3__bottom_sheet(context);
        global::Doroti.Framework.Rendering.BoxConstraints? constraintsLocal = (widget.constraints ?? bottomSheetThemeLocal.constraints) ?? defaults.constraints;
        global::Doroti.Ui.Color? colorLocal = (widget.backgroundColor ?? bottomSheetThemeLocal.backgroundColor) ?? defaults.backgroundColor;
        global::Doroti.Ui.Color? surfaceTintColorLocal = bottomSheetThemeLocal.surfaceTintColor ?? defaults.surfaceTintColor;
        global::Doroti.Ui.Color? shadowColorLocal = (widget.shadowColor ?? bottomSheetThemeLocal.shadowColor) ?? defaults.shadowColor;
        double elevationLocal = ((widget.elevation ?? bottomSheetThemeLocal.elevation) ?? defaults.elevation) ?? 0;
        global::Doroti.Framework.Painting.ShapeBorder? shapeLocal = (widget.shape ?? bottomSheetThemeLocal.shape) ?? defaults.shape;
        global::Doroti.Ui.Clip clipBehaviorLocal = (widget.clipBehavior ?? bottomSheetThemeLocal.clipBehavior) ?? Clip.none;
        bool showDragHandleLocal = widget.showDragHandle ?? widget.enableDrag && (bottomSheetThemeLocal.showDragHandle ?? false);
        global::Doroti.Framework.Widgets.Widget? dragHandle = default!;
        if (showDragHandleLocal)
        {
            dragHandle = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _DragHandle__bottom_sheet(onSemanticsTap: () => widget.onClosing(), handleHover: _handleDragHandleHover, states: dragHandleStates, dragHandleColor: widget.dragHandleColor, dragHandleSize: widget.dragHandleSize));
            if (!widget.enableDrag)
            {
                dragHandle = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _BottomSheetGestureDetector__bottom_sheet(onVerticalDragStart: _handleDragStart, onVerticalDragUpdate: _handleDragUpdate, onVerticalDragEnd: _handleDragEnd, child: dragHandle));
            }
        }
        global::Doroti.Framework.Widgets.Widget bottomSheet = new Material(key: _childKey, color: colorLocal, elevation: elevationLocal, surfaceTintColor: surfaceTintColorLocal, shadowColor: shadowColorLocal, shape: shapeLocal, clipBehavior: clipBehaviorLocal, child: new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.DraggableScrollableNotification>(onNotification: extentChanged, child: !showDragHandleLocal ? widget.builder(context) : new global::Doroti.Framework.Widgets.Stack(alignment: Alignment.topCenter, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(dragHandle!), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(top: ConstantsLibrary.kMinInteractiveDimension), child: widget.builder(context))) })));
        if (constraintsLocal is not null)
        {
            bottomSheet = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: Alignment.bottomCenter, heightFactor: 1.0, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: constraintsLocal, child: bottomSheet)));
        }
        return !widget.enableDrag ? bottomSheet : new _BottomSheetGestureDetector__bottom_sheet(onVerticalDragStart: _handleDragStart, onVerticalDragUpdate: _handleDragUpdate, onVerticalDragEnd: _handleDragEnd, child: bottomSheet);
    }

}

internal class _DragHandle__bottom_sheet : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Action? onSemanticsTap { get; private set; }
    public virtual global::System.Action<bool> handleHover { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Widgets.WidgetState> states { get; private set; } = default!;
    public virtual Color? dragHandleColor { get; private set; }
    public virtual Size? dragHandleSize { get; private set; }

    internal _DragHandle__bottom_sheet(global::System.Action? onSemanticsTap, global::System.Action<bool> handleHover, HashSet<global::Doroti.Framework.Widgets.WidgetState> states, Color? dragHandleColor = null, Size? dragHandleSize = null)
    {
        this.onSemanticsTap = onSemanticsTap;
        this.handleHover = handleHover;
        this.states = states;
        this.dragHandleColor = dragHandleColor;
        this.dragHandleSize = dragHandleSize;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        BottomSheetThemeData bottomSheetThemeLocal = Theme.of(context).bottomSheetTheme;
        BottomSheetThemeData m3Defaults = new _BottomSheetDefaultsM3__bottom_sheet(context);
        global::Doroti.Ui.Size handleSize = (dragHandleSize ?? bottomSheetThemeLocal.dragHandleSize) ?? DartRuntimePrimitives.RequireValue(m3Defaults.dragHandleSize);
        return new global::Doroti.Framework.Widgets.MouseRegion(onEnter: (@event) => { handleHover(true); }, onExit: (@event) => { handleHover(false); }, child: new global::Doroti.Framework.Widgets.Semantics(label: MaterialLocalizations.of(context).modalBarrierDismissLabel, container: true, button: true, onTap: onSemanticsTap, child: new global::Doroti.Framework.Widgets.SizedBox(width: Math.Max(handleSize.width, ConstantsLibrary.kMinInteractiveDimension), height: Math.Max(handleSize.height, ConstantsLibrary.kMinInteractiveDimension), child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Container(height: handleSize.height, width: handleSize.width, decoration: new global::Doroti.Framework.Painting.BoxDecoration(borderRadius: BorderRadius.CreateCircular(handleSize.height / 2L), color: (WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(dragHandleColor, states) ?? WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(bottomSheetThemeLocal.dragHandleColor, states)) ?? m3Defaults.dragHandleColor))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BottomSheetLayoutWithSizeListener__bottom_sheet : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual global::System.Action<Size> onChildSizeChanged { get; private set; } = default!;
    public virtual double animationValue { get; private set; } = default!;
    public virtual bool isScrollControlled { get; private set; } = default!;
    public virtual double scrollControlDisabledMaxHeightRatio { get; private set; } = default!;

    internal _BottomSheetLayoutWithSizeListener__bottom_sheet(global::System.Action<Size> onChildSizeChanged, double animationValue, bool isScrollControlled, double scrollControlDisabledMaxHeightRatio, global::Doroti.Framework.Widgets.Widget? child = null) : base(child: child)
    {
        this.onChildSizeChanged = onChildSizeChanged;
        this.animationValue = animationValue;
        this.isScrollControlled = isScrollControlled;
        this.scrollControlDisabledMaxHeightRatio = scrollControlDisabledMaxHeightRatio;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderBottomSheetLayoutWithSizeListener__bottom_sheet(onChildSizeChanged: onChildSizeChanged, animationValue: animationValue, isScrollControlled: isScrollControlled, scrollControlDisabledMaxHeightRatio: scrollControlDisabledMaxHeightRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderBottomSheetLayoutWithSizeListener__bottom_sheet)renderObject;
        __renderObject.onChildSizeChanged = onChildSizeChanged;
        __renderObject.animationValue = animationValue;
        __renderObject.isScrollControlled = isScrollControlled;
        __renderObject.scrollControlDisabledMaxHeightRatio = scrollControlDisabledMaxHeightRatio;
    }

}

public class _RenderBottomSheetLayoutWithSizeListener__bottom_sheet : global::Doroti.Framework.Rendering.RenderShiftedBox
{
    internal virtual Size _lastSize { get; set; } = Size.zero;
    internal virtual global::System.Action<Size> _onChildSizeChanged { get; set; } = default!;
    internal virtual double _animationValue { get; set; } = default!;
    internal virtual bool _isScrollControlled { get; set; } = default!;
    internal virtual double _scrollControlDisabledMaxHeightRatio { get; set; } = default!;

    internal _RenderBottomSheetLayoutWithSizeListener__bottom_sheet(global::Doroti.Framework.Rendering.RenderBox? child = null, global::System.Action<Size> onChildSizeChanged = default!, double animationValue = default!, bool isScrollControlled = default!, double scrollControlDisabledMaxHeightRatio = default!) : base(child)
    {
        _onChildSizeChanged = onChildSizeChanged;
        _animationValue = animationValue;
        _isScrollControlled = isScrollControlled;
        _scrollControlDisabledMaxHeightRatio = scrollControlDisabledMaxHeightRatio;
    }

    public virtual global::System.Action<global::Doroti.Ui.Size> onChildSizeChanged
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
    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints) => constraints.biggest;
    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = _getConstraintsForChild(constraints);
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if (result is null)
        {
            return null;
        }
        global::Doroti.Ui.Size childSize = childConstraints.isTight ? childConstraints.smallest : childLocal.getDryLayout(childConstraints);
        return DartRuntimePrimitives.RequireValue(result) + _getPositionForChild(constraints.biggest, childSize).dy;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Rendering.BoxConstraints _getConstraintsForChild(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: constraints.maxWidth, maxWidth: constraints.maxWidth, maxHeight: isScrollControlled ? constraints.maxHeight : (constraints.maxHeight * scrollControlDisabledMaxHeightRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _getPositionForChild(Size size, Size childSize)
    {
        return new global::Doroti.Ui.Offset(0.0, size.height - (childSize.height * animationValue));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = constraints.biggest;
        global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return;
        }
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = _getConstraintsForChild(constraints);
        DartRuntimePrimitives.Assert(() => childConstraints.debugAssertIsValid(isAppliedConstraint: true));
        childLocal.layout(childConstraints, parentUsesSize: !childConstraints.isTight);
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)childLocal.parentData!)!;
        global::Doroti.Ui.Size childSize = childConstraints.isTight ? childConstraints.smallest : childLocal.size;
        childParentData.offset = _getPositionForChild(size, childSize);
        if (!Equals(_lastSize, childSize))
        {
            _lastSize = childSize;
            _onChildSizeChanged?.Invoke(_lastSize);
        }
    }

}

public class _ModalBottomSheet__bottom_sheet<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual ModalBottomSheetRoute<T> route { get; private set; } = default!;
    public virtual bool isScrollControlled { get; private set; } = default!;
    public virtual double scrollControlDisabledMaxHeightRatio { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual bool enableDrag { get; private set; } = default!;
    public virtual bool showDragHandle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationStyle? animationStyle { get; private set; }

    internal _ModalBottomSheet__bottom_sheet(global::Doroti.Framework.Foundation.Key? key = null, ModalBottomSheetRoute<T> route = default!, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? clipBehavior = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, bool isScrollControlled = false, double? scrollControlDisabledMaxHeightRatio = null, bool enableDrag = true, bool showDragHandle = false, global::Doroti.Framework.Animation.AnimationStyle? animationStyle = null) : base(key: key)
    {
        double __scrollControlDisabledMaxHeightRatio = scrollControlDisabledMaxHeightRatio ?? Bottom_sheetLibrary._kDefaultScrollControlDisabledMaxHeightRatio;
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ModalBottomSheetState__bottom_sheet<T>());
}

public class _ModalBottomSheetState__bottom_sheet<T> : global::Doroti.Framework.Widgets.State<_ModalBottomSheet__bottom_sheet<T>>
{
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation _sheetAnimation { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _curvedSheetAnimation { get; private set; } = default!;

    public override void initState()
    {
        base.initState();
        _curvedSheetAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.route.animation!, curve: widget.animationStyle?.curve ?? Bottom_sheetLibrary._kModalBottomSheetCurve, reverseCurve: widget.animationStyle?.reverseCurve ?? Bottom_sheetLibrary._kModalBottomSheetCurve);
        _sheetAnimation = new global::Doroti.Framework.Animation.ProxyAnimation(_curvedSheetAnimation);
    }

    public override void didUpdateWidget(_ModalBottomSheet__bottom_sheet<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => Equals(oldWidget.route, widget.route));
        DartRuntimePrimitives.Assert(() => Equals(_curvedSheetAnimation.curve, widget.animationStyle?.curve ?? Bottom_sheetLibrary._kModalBottomSheetCurve));
        DartRuntimePrimitives.Assert(() => Equals(_curvedSheetAnimation.reverseCurve, widget.animationStyle?.reverseCurve ?? Bottom_sheetLibrary._kModalBottomSheetCurve));
    }

    public override void dispose()
    {
        _sheetAnimation.parent = AnimationsLibrary.kAlwaysDismissedAnimation;
        _curvedSheetAnimation.dispose();
        base.dispose();
    }

    internal virtual string _getRouteLabel(MaterialLocalizations localizations) => PlatformLibrary.defaultTargetPlatform switch { TargetPlatform.iOS => "", TargetPlatform.macOS => "", TargetPlatform.android or TargetPlatform.fuchsia or TargetPlatform.linux => localizations.dialogLabel, TargetPlatform.windows => localizations.dialogLabel, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    internal virtual global::Doroti.Framework.Painting.EdgeInsets _getNewClipDetails(Size topLayerSize)
    {
        return new global::Doroti.Framework.Painting.EdgeInsets(0, 0, 0, topLayerSize.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        _sheetAnimation.parent = widget.route.animation;
    }

    public virtual void handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details, bool? isClosing = null)
    {
        double currentProgress = widget.route.animation!.value;
        _sheetAnimation.parent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.route.animation!, curve: new global::Doroti.Framework.Animation.Split(currentProgress, endCurve: widget.animationStyle?.curve ?? Bottom_sheetLibrary._kModalBottomSheetCurve), reverseCurve: new global::Doroti.Framework.Animation.Split(currentProgress, endCurve: widget.animationStyle?.reverseCurve ?? Bottom_sheetLibrary._kModalBottomSheetCurve)));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        string routeLabel = _getRouteLabel(localizations);
        return new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: _sheetAnimation, child: new BottomSheet(animationController: widget.route._animationController, onClosing: () =>
        {
            if (widget.route.isCurrent)
            {
                Navigator.pop<object>(context);
            }
        }, builder: widget.route.builder, backgroundColor: widget.backgroundColor, elevation: widget.elevation, shape: widget.shape, clipBehavior: widget.clipBehavior, constraints: widget.constraints, enableDrag: widget.enableDrag, showDragHandle: widget.showDragHandle, onDragStart: handleDragStart, onDragEnd: (details, isClosing) => handleDragEnd(details, isClosing)), builder: (context, child) =>
        {
            double animationValueLocal = _sheetAnimation.value;
            return new global::Doroti.Framework.Widgets.Semantics(scopesRoute: true, namesRoute: true, label: routeLabel, explicitChildNodes: true, child: new global::Doroti.Framework.Widgets.ClipRect(child: new _BottomSheetLayoutWithSizeListener__bottom_sheet(onChildSizeChanged: (size) =>
            {
                widget.route._didChangeBarrierSemanticsClip(_getNewClipDetails(size));
            }, animationValue: animationValueLocal, isScrollControlled: widget.isScrollControlled, scrollControlDisabledMaxHeightRatio: widget.scrollControlDisabledMaxHeightRatio, child: child)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ModalBottomSheetRoute<T> : global::Doroti.Framework.Widgets.PopupRoute<T>
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.CapturedThemes? capturedThemes { get; private set; }
    public virtual bool isScrollControlled { get; private set; } = default!;
    public virtual double scrollControlDisabledMaxHeightRatio { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual Color? modalBarrierColor { get; private set; }
    public virtual bool isDismissible { get; private set; } = default!;
    public virtual bool enableDrag { get; private set; } = default!;
    public virtual bool? showDragHandle { get; private set; }
    public virtual global::Doroti.Framework.Animation.AnimationController? transitionAnimationController { get; private set; }
    public virtual Offset? anchorPoint { get; private set; }
    public virtual bool useSafeArea { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationStyle? sheetAnimationStyle { get; private set; }
    public virtual string? barrierOnTapHint { get; private set; }
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<global::Doroti.Framework.Painting.EdgeInsets> _clipDetailsNotifier { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<global::Doroti.Framework.Painting.EdgeInsets>(EdgeInsets.zero);
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel { get => __field_barrierLabel; }
    internal virtual global::Doroti.Framework.Animation.AnimationController? _animationController { get; set; } = default;

    public ModalBottomSheetRoute(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, global::Doroti.Framework.Widgets.CapturedThemes? capturedThemes = null, string? barrierLabel = null, string? barrierOnTapHint = null, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? clipBehavior = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, Color? modalBarrierColor = null, bool isDismissible = true, bool enableDrag = true, bool? showDragHandle = null, bool isScrollControlled = default!, double? scrollControlDisabledMaxHeightRatio = null, global::Doroti.Framework.Widgets.RouteSettings? settings = null, bool? requestFocus = null, global::Doroti.Framework.Animation.AnimationController? transitionAnimationController = null, Offset? anchorPoint = null, bool useSafeArea = false, global::Doroti.Framework.Animation.AnimationStyle? sheetAnimationStyle = null) : base(settings: settings, requestFocus: requestFocus)
    {
        double __scrollControlDisabledMaxHeightRatio = scrollControlDisabledMaxHeightRatio ?? Bottom_sheetLibrary._kDefaultScrollControlDisabledMaxHeightRatio;
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

    internal virtual bool _didChangeBarrierSemanticsClip(global::Doroti.Framework.Painting.EdgeInsets newClipDetails)
    {
        if (Equals(_clipDetailsNotifier.value, newClipDetails))
        {
            return false;
        }
        _clipDetailsNotifier.value = newClipDetails;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Duration transitionDuration => DartRuntimePrimitives.ConvertValue<Duration>((transitionAnimationController?.duration ?? sheetAnimationStyle?.duration) ?? Bottom_sheetLibrary._kBottomSheetEnterDuration);
    public override Duration reverseTransitionDuration => DartRuntimePrimitives.ConvertValue<Duration>(((transitionAnimationController?.reverseDuration ?? transitionAnimationController?.duration) ?? sheetAnimationStyle?.reverseDuration) ?? Bottom_sheetLibrary._kBottomSheetExitDuration);
    public override bool barrierDismissible => isDismissible;
    public override Color barrierColor => DartRuntimePrimitives.ConvertValue<Color>(modalBarrierColor ?? Colors.black54);
    public override global::Doroti.Framework.Animation.AnimationController createAnimationController()
    {
        DartRuntimePrimitives.Assert(() => _animationController is null);
        if (transitionAnimationController is not null)
        {
            _animationController = transitionAnimationController;
            willDisposeAnimationController = false;
        }
        else
        {
            _animationController = BottomSheet.createAnimationController(navigator!, sheetAnimationStyle: sheetAnimationStyle);
        }
        return _animationController!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        global::Doroti.Framework.Widgets.Widget content = new global::Doroti.Framework.Widgets.DisplayFeatureSubScreen(anchorPoint: anchorPoint, child: new global::Doroti.Framework.Widgets.Builder(builder: (context) =>
        {
            BottomSheetThemeData sheetTheme = Theme.of(context).bottomSheetTheme;
            BottomSheetThemeData defaults = new _BottomSheetDefaultsM3__bottom_sheet(context);
            return new _ModalBottomSheet__bottom_sheet<T>(route: this, animationStyle: sheetAnimationStyle, backgroundColor: ((backgroundColor ?? sheetTheme.modalBackgroundColor) ?? sheetTheme.backgroundColor) ?? defaults.backgroundColor, elevation: ((elevation ?? sheetTheme.modalElevation) ?? sheetTheme.elevation) ?? defaults.modalElevation, shape: shape, clipBehavior: clipBehavior, constraints: constraints, isScrollControlled: isScrollControlled, scrollControlDisabledMaxHeightRatio: DartRuntimePrimitives.RequireValue(scrollControlDisabledMaxHeightRatio), enableDrag: enableDrag, showDragHandle: showDragHandle ?? enableDrag && (sheetTheme.showDragHandle ?? false));
        }));
        global::Doroti.Framework.Widgets.Widget bottomSheet = useSafeArea ? new global::Doroti.Framework.Widgets.SafeArea(bottom: false, child: content) : global::Doroti.Framework.Widgets.MediaQuery.CreateRemovePadding(context: context, removeTop: true, child: content);
        bottomSheet = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(hitTestBehavior: SemanticsHitTestBehavior.opaque, child: bottomSheet));
        return capturedThemes?.wrap(bottomSheet) ?? bottomSheet;
    }

    public override global::Doroti.Framework.Widgets.Widget buildModalBarrier()
    {
        if ((barrierColor.a != 0L) && !offstage)
        {
            DartRuntimePrimitives.Assert(() => !Equals(barrierColor, barrierColor.withValues(alpha: 0.0)));
            global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Color?> colorLocal = animation!.drive(new global::Doroti.Framework.Animation.ColorTween(begin: barrierColor.withValues(alpha: 0.0), end: barrierColor).chain(new global::Doroti.Framework.Animation.CurveTween(curve: barrierCurve)));
            return new global::Doroti.Framework.Widgets.AnimatedModalBarrier(color: colorLocal, dismissible: barrierDismissible, semanticsLabel: barrierLabel, barrierSemanticsDismissible: semanticsDismissible, clipDetailsNotifier: _clipDetailsNotifier, semanticsOnTapHint: barrierOnTapHint);
        }
        else
        {
            return new global::Doroti.Framework.Widgets.ModalBarrier(dismissible: barrierDismissible, semanticsLabel: barrierLabel, barrierSemanticsDismissible: semanticsDismissible, clipDetailsNotifier: _clipDetailsNotifier, semanticsOnTapHint: barrierOnTapHint);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Bottom_sheetLibrary
{
    public static Future<T?> showModalBottomSheet<T>(global::Doroti.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, Color? backgroundColor = null, string? barrierLabel = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? clipBehavior = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, Color? barrierColor = null, bool isScrollControlled = false, double? scrollControlDisabledMaxHeightRatio = null, bool useRootNavigator = false, bool isDismissible = true, bool enableDrag = true, bool? showDragHandle = null, bool useSafeArea = false, global::Doroti.Framework.Widgets.RouteSettings? routeSettings = null, global::Doroti.Framework.Animation.AnimationController? transitionAnimationController = null, Offset? anchorPoint = null, global::Doroti.Framework.Animation.AnimationStyle? sheetAnimationStyle = null, bool? requestFocus = null)
    {
        double __scrollControlDisabledMaxHeightRatio = scrollControlDisabledMaxHeightRatio ?? _kDefaultScrollControlDisabledMaxHeightRatio;
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Framework.Widgets.NavigatorState navigator = Navigator.of(context, rootNavigator: useRootNavigator);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        return navigator.push(new ModalBottomSheetRoute<T>(builder: builder, capturedThemes: InheritedTheme.capture(from: context, to: navigator.context), isScrollControlled: isScrollControlled, scrollControlDisabledMaxHeightRatio: __scrollControlDisabledMaxHeightRatio, barrierLabel: barrierLabel ?? localizations.scrimLabel, barrierOnTapHint: localizations.scrimOnTapHint(localizations.bottomSheetLabel), backgroundColor: backgroundColor, elevation: elevation, shape: shape, clipBehavior: clipBehavior, constraints: constraints, isDismissible: isDismissible, modalBarrierColor: barrierColor ?? Theme.of(context).bottomSheetTheme.modalBarrierColor, enableDrag: enableDrag, showDragHandle: showDragHandle, settings: routeSettings, transitionAnimationController: transitionAnimationController, anchorPoint: anchorPoint, useSafeArea: useSafeArea, sheetAnimationStyle: sheetAnimationStyle, requestFocus: requestFocus));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Bottom_sheetLibrary
{
    public static PersistentBottomSheetController showBottomSheet(global::Doroti.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? clipBehavior = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, bool? enableDrag = null, bool? showDragHandle = null, global::Doroti.Framework.Animation.AnimationController? transitionAnimationController = null, global::Doroti.Framework.Animation.AnimationStyle? sheetAnimationStyle = null)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasScaffold(context));
        return Scaffold.of(context).showBottomSheet(builder, backgroundColor: backgroundColor, elevation: elevation, shape: shape, clipBehavior: clipBehavior, constraints: constraints, enableDrag: enableDrag, showDragHandle: showDragHandle, transitionAnimationController: transitionAnimationController, sheetAnimationStyle: sheetAnimationStyle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _BottomSheetGestureDetector__bottom_sheet : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails> onVerticalDragStart { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails> onVerticalDragUpdate { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails> onVerticalDragEnd { get; private set; } = default!;

    internal _BottomSheetGestureDetector__bottom_sheet(global::Doroti.Framework.Widgets.Widget child, global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails> onVerticalDragStart, global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails> onVerticalDragUpdate, global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails> onVerticalDragEnd)
    {
        this.child = child;
        this.onVerticalDragStart = onVerticalDragStart;
        this.onVerticalDragUpdate = onVerticalDragUpdate;
        this.onVerticalDragEnd = onVerticalDragEnd;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.RawGestureDetector(excludeFromSemantics: true, gestures: new DartMap<Type, dynamic>
        {
            [typeof(global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer)] = new global::Doroti.Framework.Widgets.GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>(() => new global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer(debugOwner: this), (instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onStart = onVerticalDragStart;
                    __cascade.onUpdate = onVerticalDragUpdate;
                    __cascade.onEnd = onVerticalDragEnd;
                    __cascade.onlyAcceptDragOnThreshold = true;
                    return __cascade;
                }))());
            })
        }, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BottomSheetDefaultsM3__bottom_sheet : BottomSheetThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _BottomSheetDefaultsM3__bottom_sheet(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 1.0, modalElevation: 1.0, shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateVertical(top: Radius.circular(28.0))), constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: 640))
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.surfaceContainerLow);
    public override global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Ui.Color? dragHandleColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurfaceVariant);
    public override global::Doroti.Ui.Size? dragHandleSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Size>(new global::Doroti.Ui.Size(32, 4));
    public override global::Doroti.Framework.Rendering.BoxConstraints? constraints => new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: 640.0);
}
