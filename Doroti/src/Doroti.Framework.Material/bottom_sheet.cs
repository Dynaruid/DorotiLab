// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/bottom_sheet.dart
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
    internal static double _kDefaultScrollControlDisabledMaxHeightRatio = (9.0 / 16.0);
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
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _BottomSheetState__bottom_sheet());
    public static global::Doroti.Framework.Animation.AnimationController createAnimationController(global::Doroti.Framework.Scheduler.TickerProvider vsync, global::Doroti.Framework.Animation.AnimationStyle? sheetAnimationStyle = null)
    {
        return new global::Doroti.Framework.Animation.AnimationController(duration: (sheetAnimationStyle?.duration ?? Bottom_sheetLibrary._kBottomSheetEnterDuration), reverseDuration: (sheetAnimationStyle?.reverseDuration ?? Bottom_sheetLibrary._kBottomSheetExitDuration), debugLabel: "BottomSheet", vsync: vsync);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BottomSheetState__bottom_sheet : global::Doroti.Framework.Widgets.State<BottomSheet>
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _childKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "BottomSheet child");
    public virtual HashSet<global::Doroti.Framework.Widgets.WidgetState> dragHandleStates { get; set; } = new HashSet<global::Doroti.Framework.Widgets.WidgetState>();

    internal virtual double _childHeight
    {
        get
        {
            var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)this._childKey).currentContext!.findRenderObject()!)!;
            return ((global::Doroti.Framework.Rendering.RenderBox)renderBox).size.height;
            return default!;
        }
    }
    internal virtual bool _dismissUnderway => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(((BottomSheet)(object)this.widget).animationController!.status, global::Doroti.Framework.Animation.AnimationStatus.reverse)));
    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        setState(((global::System.Action)(() =>
        {
            this.dragHandleStates.Add(global::Doroti.Framework.Widgets.WidgetState.dragged);
        })));
        ((BottomSheet)(object)this.widget).onDragStart?.Invoke(details);
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(() => (((((BottomSheet)(object)this.widget).enableDrag || ((((BottomSheet)(object)this.widget).showDragHandle ?? false)))) && (((BottomSheet)(object)this.widget).animationController is not null)), () => (object?)"'BottomSheet.animationController' cannot be null when 'BottomSheet.enableDrag' or 'BottomSheet.showDragHandle' is true. " + "Use 'BottomSheet.createAnimationController' to create one, or provide another AnimationController.");
        if (this._dismissUnderway)
        {
            return;
        }
        ((BottomSheet)(object)this.widget).animationController!.value -= (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).primaryDelta) / this._childHeight);
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => (((((BottomSheet)(object)this.widget).enableDrag || ((((BottomSheet)(object)this.widget).showDragHandle ?? false)))) && (((BottomSheet)(object)this.widget).animationController is not null)), () => (object?)"'BottomSheet.animationController' cannot be null when 'BottomSheet.enableDrag' or 'BottomSheet.showDragHandle' is true. " + "Use 'BottomSheet.createAnimationController' to create one, or provide another AnimationController.");
        if (this._dismissUnderway)
        {
            return;
        }
        setState(((global::System.Action)(() =>
        {
            this.dragHandleStates.Remove(global::Doroti.Framework.Widgets.WidgetState.dragged);
        })));
        var isClosing = false;
        if ((((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dy > Bottom_sheetLibrary._kMinFlingVelocity))
        {
            double flingVelocity = (-((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dy / this._childHeight);
            if ((((BottomSheet)(object)this.widget).animationController!.value > 0.0))
            {
                ((BottomSheet)(object)this.widget).animationController!.fling(velocity: flingVelocity);
            }
            if ((flingVelocity < 0.0))
            {
                isClosing = true;
            }
        }
        else
        {
            if ((((BottomSheet)(object)this.widget).animationController!.value < Bottom_sheetLibrary._kCloseProgressThreshold))
            {
                if ((((BottomSheet)(object)this.widget).animationController!.value > 0.0))
                {
                    ((BottomSheet)(object)this.widget).animationController!.fling(velocity: -1.0);
                }
                isClosing = true;
            }
            else
            {
                ((BottomSheet)(object)this.widget).animationController!.forward();
            }
        }
        ((BottomSheet)(object)this.widget).onDragEnd?.Invoke(details, isClosing);
        if (isClosing)
        {
            this.widget.onClosing();
        }
    }

    public virtual bool extentChanged(global::Doroti.Framework.Widgets.DraggableScrollableNotification notification)
    {
        if (((((global::Doroti.Framework.Widgets.DraggableScrollableNotification)notification).extent == ((global::Doroti.Framework.Widgets.DraggableScrollableNotification)notification).minExtent) && ((global::Doroti.Framework.Widgets.DraggableScrollableNotification)notification).shouldCloseOnMinExtent))
        {
            this.widget.onClosing();
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleDragHandleHover(bool hovering)
    {
        if ((hovering != this.dragHandleStates.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered)))
        {
            setState(((global::System.Action)(() =>
            {
                if (hovering)
                {
                    this.dragHandleStates.Add(global::Doroti.Framework.Widgets.WidgetState.hovered);
                }
                else
                {
                    this.dragHandleStates.Remove(global::Doroti.Framework.Widgets.WidgetState.hovered);
                }
            })));
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        BottomSheetThemeData bottomSheetThemeLocal = Theme.of(context).bottomSheetTheme;
        bool useMaterial3Local = Theme.of(context).useMaterial3;
        BottomSheetThemeData defaults = (useMaterial3Local ? new _BottomSheetDefaultsM3__bottom_sheet(context) : new BottomSheetThemeData());
        global::Doroti.Framework.Rendering.BoxConstraints? constraintsLocal = ((((BottomSheet)(object)this.widget).constraints ?? bottomSheetThemeLocal.constraints) ?? defaults.constraints);
        global::Doroti.Ui.Color? colorLocal = ((global::Doroti.Ui.Color?)(object?)((((BottomSheet)(object)this.widget).backgroundColor ?? bottomSheetThemeLocal.backgroundColor) ?? defaults.backgroundColor));
        global::Doroti.Ui.Color? surfaceTintColorLocal = ((global::Doroti.Ui.Color?)(object?)(bottomSheetThemeLocal.surfaceTintColor ?? defaults.surfaceTintColor));
        global::Doroti.Ui.Color? shadowColorLocal = ((global::Doroti.Ui.Color?)(object?)((((BottomSheet)(object)this.widget).shadowColor ?? bottomSheetThemeLocal.shadowColor) ?? defaults.shadowColor));
        double elevationLocal = (((((BottomSheet)(object)this.widget).elevation ?? bottomSheetThemeLocal.elevation) ?? defaults.elevation) ?? 0);
        global::Doroti.Framework.Painting.ShapeBorder? shapeLocal = ((((BottomSheet)(object)this.widget).shape ?? bottomSheetThemeLocal.shape) ?? defaults.shape);
        global::Doroti.Ui.Clip clipBehaviorLocal = ((((BottomSheet)(object)this.widget).clipBehavior ?? bottomSheetThemeLocal.clipBehavior) ?? Clip.none);
        bool showDragHandleLocal = (((BottomSheet)(object)this.widget).showDragHandle ?? ((((BottomSheet)(object)this.widget).enableDrag && ((bottomSheetThemeLocal.showDragHandle ?? false)))));
        global::Doroti.Framework.Widgets.Widget? dragHandle = default!;
        if (showDragHandleLocal)
        {
            dragHandle = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _DragHandle__bottom_sheet(onSemanticsTap: () => ((BottomSheet)(object)this.widget).onClosing(), handleHover: (global::System.Action<bool>)this._handleDragHandleHover, states: this.dragHandleStates, dragHandleColor: ((BottomSheet)(object)this.widget).dragHandleColor, dragHandleSize: ((BottomSheet)(object)this.widget).dragHandleSize));
            if (!((BottomSheet)(object)this.widget).enableDrag)
            {
                dragHandle = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _BottomSheetGestureDetector__bottom_sheet(onVerticalDragStart: (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this._handleDragStart, onVerticalDragUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handleDragUpdate, onVerticalDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._handleDragEnd, child: dragHandle));
            }
        }
        global::Doroti.Framework.Widgets.Widget bottomSheet = ((global::Doroti.Framework.Widgets.Widget)(object?)new Material(key: this._childKey, color: colorLocal, elevation: elevationLocal, surfaceTintColor: surfaceTintColorLocal, shadowColor: shadowColorLocal, shape: shapeLocal, clipBehavior: clipBehaviorLocal, child: new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.DraggableScrollableNotification>(onNotification: (global::System.Func<global::Doroti.Framework.Widgets.DraggableScrollableNotification, bool>)this.extentChanged, child: (!showDragHandleLocal ? this.widget.builder(context) : new global::Doroti.Framework.Widgets.Stack(alignment: global::Doroti.Framework.Painting.Alignment.topCenter, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(dragHandle!), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: ConstantsLibrary.kMinInteractiveDimension), child: this.widget.builder(context))) })))));
        if ((constraintsLocal is not null))
        {
            bottomSheet = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.bottomCenter, heightFactor: 1.0, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: constraintsLocal, child: bottomSheet)));
        }
        return (!((BottomSheet)(object)this.widget).enableDrag ? bottomSheet : new _BottomSheetGestureDetector__bottom_sheet(onVerticalDragStart: (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this._handleDragStart, onVerticalDragUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handleDragUpdate, onVerticalDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._handleDragEnd, child: bottomSheet));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        BottomSheetThemeData m3Defaults = ((BottomSheetThemeData)(object?)new _BottomSheetDefaultsM3__bottom_sheet(context));
        global::Doroti.Ui.Size handleSize = ((global::Doroti.Ui.Size)(object?)((this.dragHandleSize ?? bottomSheetThemeLocal.dragHandleSize) ?? DartRuntimePrimitives.RequireValue(m3Defaults.dragHandleSize)));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.MouseRegion(onEnter: ((global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>)((@event) => { this.handleHover(true); })), onExit: ((global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>)((@event) => { this.handleHover(false); })), child: new global::Doroti.Framework.Widgets.Semantics(label: MaterialLocalizations.of(context).modalBarrierDismissLabel, container: true, button: true, onTap: () => this.onSemanticsTap(), child: new global::Doroti.Framework.Widgets.SizedBox(width: Math.Max(handleSize.width, ConstantsLibrary.kMinInteractiveDimension), height: Math.Max(handleSize.height, ConstantsLibrary.kMinInteractiveDimension), child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Container(height: handleSize.height, width: handleSize.width, decoration: new global::Doroti.Framework.Painting.BoxDecoration(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateCircular((handleSize.height / 2L)), color: (((WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(this.dragHandleColor, this.states) ?? (Color)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(bottomSheetThemeLocal.dragHandleColor, this.states))) ?? m3Defaults.dragHandleColor))))))));
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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderBottomSheetLayoutWithSizeListener__bottom_sheet(onChildSizeChanged: (global::System.Action<Size>)this.onChildSizeChanged, animationValue: this.animationValue, isScrollControlled: this.isScrollControlled, scrollControlDisabledMaxHeightRatio: this.scrollControlDisabledMaxHeightRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderBottomSheetLayoutWithSizeListener__bottom_sheet)(object)renderObject;
        __renderObject.onChildSizeChanged = (global::System.Action<Size>)this.onChildSizeChanged;
        __renderObject.animationValue = this.animationValue;
        __renderObject.isScrollControlled = this.isScrollControlled;
        __renderObject.scrollControlDisabledMaxHeightRatio = this.scrollControlDisabledMaxHeightRatio;
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
        this._onChildSizeChanged = onChildSizeChanged;
        this._animationValue = animationValue;
        this._isScrollControlled = isScrollControlled;
        this._scrollControlDisabledMaxHeightRatio = scrollControlDisabledMaxHeightRatio;
    }

    public virtual global::System.Action<global::Doroti.Ui.Size> onChildSizeChanged
    {
        get => this._onChildSizeChanged;
        set
        {
            var newCallback = value;
            if ((object.Equals((global::System.Action<Size>)this._onChildSizeChanged, (global::System.Action<Size>)newCallback)))
            {
                return;
            }
            _onChildSizeChanged = (global::System.Action<Size>)newCallback;
            markNeedsLayout();
        }
    }
    public virtual double animationValue
    {
        get => this._animationValue;
        set
        {
            var newValue = value;
            if ((this._animationValue == newValue))
            {
                return;
            }
            _animationValue = newValue;
            markNeedsLayout();
        }
    }
    public virtual bool isScrollControlled
    {
        get => this._isScrollControlled;
        set
        {
            var newValue = value;
            if ((this._isScrollControlled == newValue))
            {
                return;
            }
            _isScrollControlled = newValue;
            markNeedsLayout();
        }
    }
    public virtual double scrollControlDisabledMaxHeightRatio
    {
        get => this._scrollControlDisabledMaxHeightRatio;
        set
        {
            var newValue = value;
            if ((this._scrollControlDisabledMaxHeightRatio == newValue))
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
    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints) => ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest;
    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return null;
        }
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)_getConstraintsForChild(constraints));
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if ((result is null))
        {
            return null;
        }
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)(((global::Doroti.Framework.Rendering.BoxConstraints)childConstraints).isTight ? ((global::Doroti.Framework.Rendering.BoxConstraints)childConstraints).smallest : childLocal.getDryLayout(childConstraints)));
        return (DartRuntimePrimitives.RequireValue(result) + _getPositionForChild(((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest, childSize).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Rendering.BoxConstraints _getConstraintsForChild(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth, maxWidth: ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth, maxHeight: (this.isScrollControlled ? ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight : (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight * this.scrollControlDisabledMaxHeightRatio)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _getPositionForChild(Size size, Size childSize)
    {
        return new global::Doroti.Ui.Offset(0.0, (size.height - (childSize.height * this.animationValue)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).biggest;
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return;
        }
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)_getConstraintsForChild(this.constraints));
        DartRuntimePrimitives.Assert(() => childConstraints.debugAssertIsValid(isAppliedConstraint: true));
        childLocal.layout(childConstraints, parentUsesSize: !((global::Doroti.Framework.Rendering.BoxConstraints)childConstraints).isTight);
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)childLocal.parentData!)!;
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)(((global::Doroti.Framework.Rendering.BoxConstraints)childConstraints).isTight ? ((global::Doroti.Framework.Rendering.BoxConstraints)childConstraints).smallest : ((global::Doroti.Framework.Rendering.RenderBox)childLocal).size));
        childParentData.offset = _getPositionForChild(this.size, childSize);
        if ((!object.Equals(this._lastSize, childSize)))
        {
            _lastSize = childSize;
            this._onChildSizeChanged?.Invoke(this._lastSize);
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
        _curvedSheetAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).route.animation!, curve: (((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).animationStyle?.curve ?? Bottom_sheetLibrary._kModalBottomSheetCurve), reverseCurve: (((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).animationStyle?.reverseCurve ?? Bottom_sheetLibrary._kModalBottomSheetCurve));
        _sheetAnimation = new global::Doroti.Framework.Animation.ProxyAnimation(this._curvedSheetAnimation);
    }

    public override void didUpdateWidget(_ModalBottomSheet__bottom_sheet<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(((_ModalBottomSheet__bottom_sheet<T>)oldWidget).route, ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).route)));
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Framework.Animation.CurvedAnimation)this._curvedSheetAnimation).curve, ((((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).animationStyle?.curve ?? Bottom_sheetLibrary._kModalBottomSheetCurve)))));
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Framework.Animation.CurvedAnimation)this._curvedSheetAnimation).reverseCurve, ((((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).animationStyle?.reverseCurve ?? Bottom_sheetLibrary._kModalBottomSheetCurve)))));
    }

    public override void dispose()
    {
        this._sheetAnimation.parent = global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation;
        this._curvedSheetAnimation.dispose();
        base.dispose();
    }

    internal virtual string _getRouteLabel(MaterialLocalizations localizations) => (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.iOS => "", global::Doroti.Framework.Foundation.TargetPlatform.macOS => "", global::Doroti.Framework.Foundation.TargetPlatform.android or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Framework.Foundation.TargetPlatform.linux => ((MaterialLocalizations)localizations).dialogLabel, global::Doroti.Framework.Foundation.TargetPlatform.windows => ((MaterialLocalizations)localizations).dialogLabel, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual global::Doroti.Framework.Painting.EdgeInsets _getNewClipDetails(Size topLayerSize)
    {
        return new global::Doroti.Framework.Painting.EdgeInsets(0, 0, 0, topLayerSize.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        this._sheetAnimation.parent = ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).route.animation;
    }

    public virtual void handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details, bool? isClosing = null)
    {
        double currentProgress = ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).route.animation!.value;
        this._sheetAnimation.parent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).route.animation!, curve: new global::Doroti.Framework.Animation.Split(currentProgress, endCurve: (((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).animationStyle?.curve ?? Bottom_sheetLibrary._kModalBottomSheetCurve)), reverseCurve: new global::Doroti.Framework.Animation.Split(currentProgress, endCurve: (((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).animationStyle?.reverseCurve ?? Bottom_sheetLibrary._kModalBottomSheetCurve))));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        string routeLabel = ((string)(object?)_getRouteLabel(localizations));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: this._sheetAnimation, child: new BottomSheet(animationController: ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).route._animationController, onClosing: ((global::System.Action)(() =>
        {
            if (((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).route.isCurrent)
            {
                Navigator.pop<object>(context);
            }
        })), builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).route.builder, backgroundColor: ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).backgroundColor, elevation: ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).elevation, shape: ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).shape, clipBehavior: ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).clipBehavior, constraints: ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).constraints, enableDrag: ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).enableDrag, showDragHandle: ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).showDragHandle, onDragStart: (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this.handleDragStart, onDragEnd: (BottomSheetDragEndHandler)((details, isClosing) => this.handleDragEnd(details, isClosing))), builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            double animationValueLocal = ((global::Doroti.Framework.Animation.ProxyAnimation)this._sheetAnimation).value;
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(scopesRoute: true, namesRoute: true, label: routeLabel, explicitChildNodes: true, child: new global::Doroti.Framework.Widgets.ClipRect(child: new _BottomSheetLayoutWithSizeListener__bottom_sheet(onChildSizeChanged: ((global::System.Action<Size>)((size) =>
            {
                ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).route._didChangeBarrierSemanticsClip(_getNewClipDetails(size));
            })), animationValue: animationValueLocal, isScrollControlled: ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).isScrollControlled, scrollControlDisabledMaxHeightRatio: ((_ModalBottomSheet__bottom_sheet<T>)(object)this.widget).scrollControlDisabledMaxHeightRatio, child: child))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
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
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<global::Doroti.Framework.Painting.EdgeInsets> _clipDetailsNotifier { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<global::Doroti.Framework.Painting.EdgeInsets>(global::Doroti.Framework.Painting.EdgeInsets.zero);
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel { get => __field_barrierLabel; }
    internal virtual global::Doroti.Framework.Animation.AnimationController? _animationController { get; set; } = default;

    public ModalBottomSheetRoute(global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, global::Doroti.Framework.Widgets.CapturedThemes? capturedThemes = null, string? barrierLabel = null, string? barrierOnTapHint = null, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? clipBehavior = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, Color? modalBarrierColor = null, bool isDismissible = true, bool enableDrag = true, bool? showDragHandle = null, bool isScrollControlled = default!, double? scrollControlDisabledMaxHeightRatio = null, global::Doroti.Framework.Widgets.RouteSettings? settings = null, bool? requestFocus = null, global::Doroti.Framework.Animation.AnimationController? transitionAnimationController = null, Offset? anchorPoint = null, bool useSafeArea = false, global::Doroti.Framework.Animation.AnimationStyle? sheetAnimationStyle = null) : base(settings: settings, requestFocus: requestFocus)
    {
        double __scrollControlDisabledMaxHeightRatio = scrollControlDisabledMaxHeightRatio ?? Bottom_sheetLibrary._kDefaultScrollControlDisabledMaxHeightRatio;
        this.builder = builder;
        this.capturedThemes = capturedThemes;
        this.__field_barrierLabel = barrierLabel;
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
        this._clipDetailsNotifier.dispose();
        base.dispose();
    }

    internal virtual bool _didChangeBarrierSemanticsClip(global::Doroti.Framework.Painting.EdgeInsets newClipDetails)
    {
        if ((object.Equals(((global::Doroti.Framework.Foundation.ValueNotifier<global::Doroti.Framework.Painting.EdgeInsets>)this._clipDetailsNotifier).value, newClipDetails)))
        {
            return false;
        }
        this._clipDetailsNotifier.value = newClipDetails;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Duration transitionDuration => DartRuntimePrimitives.ConvertValue<Duration>(((this.transitionAnimationController?.duration ?? this.sheetAnimationStyle?.duration) ?? Bottom_sheetLibrary._kBottomSheetEnterDuration));
    public override Duration reverseTransitionDuration => DartRuntimePrimitives.ConvertValue<Duration>((((this.transitionAnimationController?.reverseDuration ?? this.transitionAnimationController?.duration) ?? this.sheetAnimationStyle?.reverseDuration) ?? Bottom_sheetLibrary._kBottomSheetExitDuration));
    public override bool barrierDismissible => this.isDismissible;
    public override Color? barrierColor => DartRuntimePrimitives.ConvertValue<Color>((this.modalBarrierColor ?? Colors.black54));
    public override global::Doroti.Framework.Animation.AnimationController createAnimationController()
    {
        DartRuntimePrimitives.Assert(() => (this._animationController is null));
        if ((this.transitionAnimationController is not null))
        {
            _animationController = this.transitionAnimationController;
            willDisposeAnimationController = false;
        }
        else
        {
            _animationController = BottomSheet.createAnimationController(this.navigator!, sheetAnimationStyle: this.sheetAnimationStyle);
        }
        return this._animationController!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        global::Doroti.Framework.Widgets.Widget content = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.DisplayFeatureSubScreen(anchorPoint: this.anchorPoint, child: new global::Doroti.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
        {
            BottomSheetThemeData sheetTheme = Theme.of(context).bottomSheetTheme;
            BottomSheetThemeData defaults = (Theme.of(context).useMaterial3 ? new _BottomSheetDefaultsM3__bottom_sheet(context) : new BottomSheetThemeData());
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new _ModalBottomSheet__bottom_sheet<T>(route: this, animationStyle: this.sheetAnimationStyle, backgroundColor: (((this.backgroundColor ?? sheetTheme.modalBackgroundColor) ?? sheetTheme.backgroundColor) ?? defaults.backgroundColor), elevation: (((this.elevation ?? sheetTheme.modalElevation) ?? sheetTheme.elevation) ?? defaults.modalElevation), shape: this.shape, clipBehavior: this.clipBehavior, constraints: this.constraints, isScrollControlled: this.isScrollControlled, scrollControlDisabledMaxHeightRatio: DartRuntimePrimitives.RequireValue(this.scrollControlDisabledMaxHeightRatio), enableDrag: this.enableDrag, showDragHandle: (this.showDragHandle ?? ((this.enableDrag && ((sheetTheme.showDragHandle ?? false)))))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
        global::Doroti.Framework.Widgets.Widget bottomSheet = (this.useSafeArea ? new global::Doroti.Framework.Widgets.SafeArea(bottom: false, child: content) : global::Doroti.Framework.Widgets.MediaQuery.CreateRemovePadding(context: context, removeTop: true, child: content));
        bottomSheet = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(hitTestBehavior: SemanticsHitTestBehavior.opaque, child: bottomSheet));
        return (this.capturedThemes?.wrap(bottomSheet) ?? bottomSheet);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget buildModalBarrier()
    {
        if (((this.barrierColor.a != 0L) && !this.offstage))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(this.barrierColor, this.barrierColor.withValues(alpha: 0.0))));
            global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Color?> colorLocal = ((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Color?>)(object?)this.animation!.drive(new global::Doroti.Framework.Animation.ColorTween(begin: this.barrierColor.withValues(alpha: 0.0), end: this.barrierColor).chain(new global::Doroti.Framework.Animation.CurveTween(curve: this.barrierCurve))));
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedModalBarrier(color: colorLocal, dismissible: this.barrierDismissible, semanticsLabel: this.barrierLabel, barrierSemanticsDismissible: this.semanticsDismissible, clipDetailsNotifier: this._clipDetailsNotifier, semanticsOnTapHint: this.barrierOnTapHint));
        }
        else
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ModalBarrier(dismissible: this.barrierDismissible, semanticsLabel: this.barrierLabel, barrierSemanticsDismissible: this.semanticsDismissible, clipDetailsNotifier: this._clipDetailsNotifier, semanticsOnTapHint: this.barrierOnTapHint));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Bottom_sheetLibrary
{
    public static Future<T?> showModalBottomSheet<T>(global::Doroti.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, Color? backgroundColor = null, string? barrierLabel = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? clipBehavior = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, Color? barrierColor = null, bool isScrollControlled = false, double? scrollControlDisabledMaxHeightRatio = null, bool useRootNavigator = false, bool isDismissible = true, bool enableDrag = true, bool? showDragHandle = null, bool useSafeArea = false, global::Doroti.Framework.Widgets.RouteSettings? routeSettings = null, global::Doroti.Framework.Animation.AnimationController? transitionAnimationController = null, Offset? anchorPoint = null, global::Doroti.Framework.Animation.AnimationStyle? sheetAnimationStyle = null, bool? requestFocus = null)
    {
        double __scrollControlDisabledMaxHeightRatio = scrollControlDisabledMaxHeightRatio ?? Bottom_sheetLibrary._kDefaultScrollControlDisabledMaxHeightRatio;
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Framework.Widgets.NavigatorState navigator = ((global::Doroti.Framework.Widgets.NavigatorState)(object?)Navigator.of(context, rootNavigator: useRootNavigator));
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        return ((Future<T?>)(object?)navigator.push(new ModalBottomSheetRoute<T>(builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)builder, capturedThemes: InheritedTheme.capture(from: context, to: navigator.context), isScrollControlled: isScrollControlled, scrollControlDisabledMaxHeightRatio: __scrollControlDisabledMaxHeightRatio, barrierLabel: ((barrierLabel ?? (string)((MaterialLocalizations)localizations).scrimLabel)), barrierOnTapHint: localizations.scrimOnTapHint(((MaterialLocalizations)localizations).bottomSheetLabel), backgroundColor: backgroundColor, elevation: elevation, shape: shape, clipBehavior: clipBehavior, constraints: constraints, isDismissible: isDismissible, modalBarrierColor: (barrierColor ?? Theme.of(context).bottomSheetTheme.modalBarrierColor), enableDrag: enableDrag, showDragHandle: showDragHandle, settings: routeSettings, transitionAnimationController: transitionAnimationController, anchorPoint: anchorPoint, useSafeArea: useSafeArea, sheetAnimationStyle: sheetAnimationStyle, requestFocus: requestFocus)));
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
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.RawGestureDetector(excludeFromSemantics: true, gestures: new DartMap<Type, dynamic>
        {
            [typeof(global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer)] = new global::Doroti.Framework.Widgets.GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer(debugOwner: this))), ((global::System.Action<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>)((instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onStart = this.onVerticalDragStart;
                    __cascade.onUpdate = this.onVerticalDragUpdate;
                    __cascade.onEnd = this.onVerticalDragEnd;
                    __cascade.onlyAcceptDragOnThreshold = true;
                    return __cascade;
                }))());
            })))
        }, child: this.child));
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
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _BottomSheetDefaultsM3__bottom_sheet(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 1.0, modalElevation: 1.0, shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateVertical(top: global::Doroti.Ui.Radius.circular(28.0))), constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: 640))
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surfaceContainerLow);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? dragHandleColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurfaceVariant);
    public virtual global::Doroti.Ui.Size? dragHandleSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Size>(new global::Doroti.Ui.Size(32, 4));
    public override global::Doroti.Framework.Rendering.BoxConstraints? constraints => new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: 640.0);
}
