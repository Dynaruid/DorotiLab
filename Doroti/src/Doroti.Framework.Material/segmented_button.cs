// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/segmented_button.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ButtonSegment<T>
{
    public virtual T value { get; private set; } = default!;
    public virtual Widget? icon { get; private set; }
    public virtual Widget? label { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual bool enabled { get; private set; } = default!;

    public ButtonSegment(T value, Widget? icon = null, Widget? label = null, string? tooltip = null, bool enabled = true)
    {
        this.value = value;
        this.icon = icon;
        this.label = label;
        this.tooltip = tooltip;
        this.enabled = enabled;
        System.Diagnostics.Debug.Assert((icon is not null) || (label is not null));
    }

}

public class SegmentedButton<T> : StatefulWidget where T : notnull
{
    public virtual List<ButtonSegment<T>> segments { get; private set; } = default!;
    public virtual Axis direction { get; private set; } = default!;
    public virtual HashSet<T> selected { get; private set; } = default!;
    public virtual System.Action<HashSet<T>>? onSelectionChanged { get; private set; }
    public virtual bool multiSelectionEnabled { get; private set; } = default!;
    public virtual bool emptySelectionAllowed { get; private set; } = default!;
    public virtual EdgeInsets? expandedInsets { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual bool showSelectedIcon { get; private set; } = default!;
    public virtual Widget? selectedIcon { get; private set; }

    public SegmentedButton(Key? key = null, List<ButtonSegment<T>> segments = default!, HashSet<T> selected = default!, System.Action<HashSet<T>>? onSelectionChanged = null, bool multiSelectionEnabled = false, bool emptySelectionAllowed = false, EdgeInsets? expandedInsets = null, ButtonStyle? style = null, bool showSelectedIcon = true, Widget? selectedIcon = null, Axis direction = Axis.horizontal) : base(key: key)
    {
        this.segments = segments;
        this.selected = selected;
        this.onSelectionChanged = onSelectionChanged;
        this.multiSelectionEnabled = multiSelectionEnabled;
        this.emptySelectionAllowed = emptySelectionAllowed;
        this.expandedInsets = expandedInsets;
        this.style = style;
        this.showSelectedIcon = showSelectedIcon;
        this.selectedIcon = selectedIcon;
        this.direction = direction;
        System.Diagnostics.Debug.Assert(checked(segments.Count) > 0L);
        System.Diagnostics.Debug.Assert((checked(selected.Count) > 0L) || emptySelectionAllowed);
        System.Diagnostics.Debug.Assert((checked(selected.Count) < 2L) || multiSelectionEnabled);
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? selectedForegroundColor = null, Color? selectedBackgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? iconColor = null, double? iconSize = null, Color? disabledIconColor = null, Color? overlayColor = null, double? elevation = null, TextStyle? textStyle = null, EdgeInsetsGeometry? padding = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, BorderSide? side = null, OutlinedBorder? shape = null, MouseCursor? enabledMouseCursor = null, MouseCursor? disabledMouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null)
    {
        WidgetStateProperty<Color?>? overlayColorProp = ((foregroundColor is null) && (selectedForegroundColor is null) && (overlayColor is null)) ? null : (overlayColor switch { Color overlayColorLocal when overlayColorLocal.value == 0L => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(new WidgetStatePropertyAll<Color?>(Colors.transparent)), _ => _SegmentedButtonDefaultsM3__segmented_button.resolveStateColor(foregroundColor, selectedForegroundColor, overlayColor) });
        return TextButton.styleFrom(textStyle: textStyle, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, iconColor: iconColor, iconSize: iconSize, disabledIconColor: disabledIconColor, elevation: elevation, padding: padding, minimumSize: minimumSize, fixedSize: fixedSize, maximumSize: maximumSize, side: side, shape: shape, enabledMouseCursor: enabledMouseCursor, disabledMouseCursor: disabledMouseCursor, visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory).copyWith(foregroundColor: SegmentedButton<T>._defaultColor(foregroundColor, disabledForegroundColor, selectedForegroundColor), backgroundColor: SegmentedButton<T>._defaultColor(backgroundColor, disabledBackgroundColor, selectedBackgroundColor), overlayColor: overlayColorProp);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static WidgetStateProperty<Color?>? _defaultColor(Color? enabled, Color? disabled, Color? selected)
    {
        if (((selected ?? enabled) ?? disabled) is null)
        {
            return null;
        }
        return (WidgetStateProperty<Color?>?)WidgetStateProperty<Color?>.CreateFromMap(new DartMap<WidgetStatesConstraint, Color?> { [WidgetState.disabled.asConstraint()] = disabled, [WidgetState.selected.asConstraint()] = selected, [WidgetStateMembers.any] = enabled }.cast<WidgetStatesConstraint, Color?>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new SegmentedButtonState<T>());
}

public class SegmentedButtonState<T> : State<SegmentedButton<T>> where T : notnull
{
    internal virtual bool _hovering { get; set; } = false;
    internal virtual bool _focused { get; set; } = false;
    public virtual DartMap<ButtonSegment<T>, WidgetStatesController> statesControllers { get; private set; } = new DartMap<ButtonSegment<T>, WidgetStatesController>();

    internal virtual bool _enabled => DartRuntimePrimitives.ConvertValue<bool>(widget.onSelectionChanged is not null);
    internal virtual bool _selected => Enumerable.Any(widget.selected);
    internal virtual HashSet<WidgetState> _states => ((Func<HashSet<WidgetState>>)(() => { var __collection16615 = new HashSet<WidgetState>(); if (!_enabled) { __collection16615.Add(WidgetState.disabled); } if (_hovering) { __collection16615.Add(WidgetState.hovered); } if (_focused) { __collection16615.Add(WidgetState.focused); } if (_selected) { __collection16615.Add(WidgetState.selected); } return __collection16615; }))();
    public override void didUpdateWidget(SegmentedButton<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget, widget))
        {
            statesControllers.removeWhere((segment, controller) =>
            {
                if (widget.segments.Contains(segment))
                {
                    return false;
                }
                else
                {
                    controller.dispose();
                    return true;
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }

    internal virtual void _handleOnPressed(T segmentValue)
    {
        if (!_enabled)
        {
            return;
        }
        bool onlySelectedSegment = (checked(widget.selected.Count) == 1L) && widget.selected.Contains(segmentValue);
        bool validChange = widget.emptySelectionAllowed || !onlySelectedSegment;
        if (validChange)
        {
            bool toggle = widget.multiSelectionEnabled || widget.emptySelectionAllowed && onlySelectedSegment;
            var pressedSegment = new HashSet<T> { segmentValue };
            HashSet<T> updatedSelection = default!;
            if (toggle)
            {
                updatedSelection = widget.selected.Contains(segmentValue) ? widget.selected.difference(pressedSegment) : widget.selected.Union(pressedSegment).ToHashSet();
            }
            else
            {
                updatedSelection = pressedSegment;
            }
            if (!CollectionsLibrary.setEquals(updatedSelection, widget.selected))
            {
                widget.onSelectionChanged!(updatedSelection);
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        SegmentedButtonThemeData theme = SegmentedButtonTheme.of(context);
        SegmentedButtonThemeData defaults = new _SegmentedButtonDefaultsM3__segmented_button(context);
        TextDirection textDirectionLocal = Directionality.of(context);
        var disabledState = new HashSet<WidgetState> { WidgetState.disabled };
        P? effectiveValue<P>(Func<ButtonStyle?, P?> getProperty)
        {
            P? widgetValue = getProperty(((SegmentedButton<T>)widget).style);
            P? themeValue = getProperty(theme.style);
            P? defaultValue = getProperty(defaults.style);
            return (widgetValue ?? themeValue) ?? defaultValue;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(Func<ButtonStyle?, WidgetStateProperty<P>?> getProperty, HashSet<WidgetState>? states = null)
        {
            return effectiveValue((style) => DartRuntimePrimitives.NullAware(getProperty(style), __target => __target.resolve(states ?? (HashSet<global::Doroti.Framework.Widgets.WidgetState>)_states)));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        ButtonStyle segmentStyleFor(ButtonStyle? style)
        {
            return new ButtonStyle(textStyle: style?.textStyle, backgroundColor: style?.backgroundColor, foregroundColor: style?.foregroundColor, overlayColor: style?.overlayColor, surfaceTintColor: style?.surfaceTintColor, elevation: style?.elevation, padding: style?.padding, iconColor: style?.iconColor, iconSize: style?.iconSize, shape: new WidgetStatePropertyAll<OutlinedBorder>(new RoundedRectangleBorder()), mouseCursor: style?.mouseCursor, visualDensity: style?.visualDensity, tapTargetSize: style?.tapTargetSize, animationDuration: style?.animationDuration, enableFeedback: style?.enableFeedback, alignment: style?.alignment, splashFactory: style?.splashFactory);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        ButtonStyle segmentStyle = segmentStyleFor(widget.style);
        ButtonStyle segmentThemeStyle = segmentStyleFor(theme.style).merge(segmentStyleFor(defaults.style));
        Widget? selectedIconLocal = widget.showSelectedIcon ? ((widget.selectedIcon ?? theme.selectedIcon) ?? defaults.selectedIcon) : null;
        Widget buttonFor(ButtonSegment<T> segment)
        {
            Widget labelLocal = (segment.label ?? segment.icon) ?? SizedBox.CreateShrink();
            bool segmentSelected = widget.selected.Contains(segment.value);
            Widget? iconLocal = (segmentSelected && widget.showSelectedIcon) ? selectedIconLocal : ((segment.label is not null) ? segment.icon : null);
            WidgetStatesController controller = statesControllers.putIfAbsent(segment, () => new WidgetStatesController());
            controller.update(WidgetState.selected, segmentSelected);
            var content = labelLocal;
            var effectiveSegmentStyle = segmentStyle;
            if (iconLocal is not null)
            {
                double defaultFontSize = segmentStyle.textStyle?.resolve(new HashSet<WidgetState>())?.fontSize ?? 14.0;
                double effectiveTextScale = MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0;
                EdgeInsetsGeometry scaledPaddingLocal = ButtonStyleButton.scaledPadding(new EdgeInsetsDirectional(12, 8, 16, 8), EdgeInsets.CreateSymmetric(horizontal: 4), EdgeInsets.CreateSymmetric(horizontal: 4), effectiveTextScale);
                effectiveSegmentStyle = segmentStyle.copyWith(padding: new WidgetStatePropertyAll<EdgeInsetsGeometry>(scaledPaddingLocal));
                double scaleLocal = Dart_uiLibrary.clampDouble(effectiveTextScale, 1.0, 2.0) - 1.0;
                TextButtonThemeData textButtonTheme = TextButtonTheme.of(context);
                IconAlignment effectiveIconAlignment = (textButtonTheme.style?.iconAlignment ?? segmentStyle.iconAlignment) ?? IconAlignment.start;
                content = DartRuntimePrimitives.ConvertValue<Widget>(new Row(mainAxisSize: MainAxisSize.min, spacing: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(8L, 4L, scaleLocal)), children: Equals(effectiveIconAlignment, IconAlignment.start) ? new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(iconLocal), DartRuntimePrimitives.ConvertValue<Widget>(new Flexible(child: labelLocal)) } : new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Flexible(child: labelLocal)), DartRuntimePrimitives.ConvertValue<Widget>(iconLocal) }));
            }
            Widget button = new TextButton(style: effectiveSegmentStyle, statesController: controller, onHover: (hovering) =>
            {
                setState(() =>
                {
                    _hovering = hovering;
                });
            }, onFocusChange: (focused) =>
            {
                setState(() =>
                {
                    _focused = focused;
                });
            }, onPressed: (_enabled && segment.enabled) ? (() => { _handleOnPressed(segment.value); }) : null, child: content);
            Widget buttonWithTooltip = (segment.tooltip is not null) ? new Tooltip(message: segment.tooltip, child: button) : button;
            return new MergeSemantics(child: new Widgets.Semantics(selected: segmentSelected, inMutuallyExclusiveGroup: widget.multiSelectionEnabled ? null : true, child: buttonWithTooltip));
        }
        OutlinedBorder effectiveBorder = resolve((style) => style?.shape) ?? new RoundedRectangleBorder();
        OutlinedBorder resolvedDisabledBorder = resolve((style) => style?.shape, disabledState) ?? new RoundedRectangleBorder();
        BorderSide effectiveSide = resolve((style) => style?.side) ?? BorderSide.none;
        BorderSide disabledSide = resolve((style) => style?.side, disabledState) ?? BorderSide.none;
        OutlinedBorder enabledBorderLocal = effectiveBorder.copyWith(side: effectiveSide);
        OutlinedBorder disabledBorderLocal = resolvedDisabledBorder.copyWith(side: disabledSide);
        VisualDensity resolvedVisualDensity = (segmentStyle.visualDensity ?? segmentThemeStyle.visualDensity) ?? Theme.of(context).visualDensity;
        EdgeInsetsGeometry resolvedPadding = resolve((style) => style?.padding) ?? EdgeInsets.zero;
        MaterialTapTargetSize resolvedTapTargetSize = (segmentStyle.tapTargetSize ?? segmentThemeStyle.tapTargetSize) ?? Theme.of(context).materialTapTargetSize;
        double fontSizeLocal = resolve((style) => style?.textStyle)?.fontSize ?? 20.0;
        List<Widget> buttons = widget.segments.map(buttonFor).ToList().ToList();
        Offset densityAdjustment = resolvedVisualDensity.baseSizeAdjustment;
        var textButtonMinHeight = 40.0;
        double adjustButtonMinHeight = textButtonMinHeight + densityAdjustment.dy;
        double effectiveVerticalPadding = resolvedPadding.vertical + (densityAdjustment.dy * 2L);
        double effectedButtonHeight = Math.Max(fontSizeLocal + effectiveVerticalPadding, adjustButtonMinHeight);
        double tapTargetVerticalPaddingLocal = resolvedTapTargetSize switch { var __constant25770 when Equals(__constant25770, MaterialTapTargetSize.shrinkWrap) => 0.0, var __constant25817 when Equals(__constant25817, MaterialTapTargetSize.padded) => Math.Max(0, Widgets.ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dy - effectedButtonHeight), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return new Material(type: MaterialType.transparency, elevation: DartRuntimePrimitives.RequireValue(resolve((style) => style?.elevation)), shadowColor: resolve((style) => style?.shadowColor), surfaceTintColor: resolve((style) => style?.surfaceTintColor), child: new TextButtonTheme(data: new TextButtonThemeData(style: segmentThemeStyle), child: new Padding(padding: widget.expandedInsets ?? EdgeInsets.zero, child: new _SegmentedButtonRenderWidget__segmented_button<T>(tapTargetVerticalPadding: tapTargetVerticalPaddingLocal, segments: widget.segments, enabledBorder: _enabled ? enabledBorderLocal : disabledBorderLocal, disabledBorder: disabledBorderLocal, direction: widget.direction, textDirection: textDirectionLocal, isExpanded: widget.expandedInsets is not null, children: buttons))));
    }

    public override void dispose()
    {
        foreach (WidgetStatesController controller in statesControllers.Values)
        {
            controller.dispose();
        }
        base.dispose();
    }

}

internal class _SegmentedButtonRenderWidget__segmented_button<T> : MultiChildRenderObjectWidget
{
    public virtual List<ButtonSegment<T>> segments { get; private set; } = default!;
    public virtual OutlinedBorder enabledBorder { get; private set; } = default!;
    public virtual OutlinedBorder disabledBorder { get; private set; } = default!;
    public virtual Axis direction { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual double tapTargetVerticalPadding { get; private set; } = default!;
    public virtual bool isExpanded { get; private set; } = default!;

    internal _SegmentedButtonRenderWidget__segmented_button(Key? key = null, List<ButtonSegment<T>> segments = default!, OutlinedBorder enabledBorder = default!, OutlinedBorder disabledBorder = default!, Axis direction = default!, TextDirection textDirection = default!, double tapTargetVerticalPadding = default!, bool isExpanded = default!, List<Widget> children = default!) : base(key: key, children: children)
    {
        this.segments = segments;
        this.enabledBorder = enabledBorder;
        this.disabledBorder = disabledBorder;
        this.direction = direction;
        this.textDirection = textDirection;
        this.tapTargetVerticalPadding = tapTargetVerticalPadding;
        this.isExpanded = isExpanded;
        System.Diagnostics.Debug.Assert(checked(children.Count) == checked((long)segments.Count));
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSegmentedButton__segmented_button<T>(segments: segments, enabledBorder: enabledBorder, disabledBorder: disabledBorder, textDirection: textDirection, direction: direction, tapTargetVerticalPadding: tapTargetVerticalPadding, isExpanded: isExpanded);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderSegmentedButton__segmented_button<T>)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSegmentedButton__segmented_button<T>>)(() =>
{
    var __cascade = __renderObject;
    __cascade.segments = segments;
    __cascade.enabledBorder = enabledBorder;
    __cascade.disabledBorder = disabledBorder;
    __cascade.direction = direction;
    __cascade.textDirection = textDirection;
    return __cascade;
}))());
    }

}

internal class _SegmentedButtonContainerBoxParentData__segmented_button : ContainerBoxParentData<RenderBox>
{
    public virtual RRect? surroundingRect { get; set; } = default;

}

internal delegate RenderBox? _NextChild__segmented_button(RenderBox child);

public class _RenderSegmentedButton__segmented_button<T> : RenderBox, ContainerRenderObjectMixin<RenderBox, ContainerBoxParentData<RenderBox>>, RenderBoxContainerDefaultsMixin<RenderBox, ContainerBoxParentData<RenderBox>>
{
    internal virtual List<ButtonSegment<T>> _segments { get; set; } = default!;
    internal virtual OutlinedBorder _enabledBorder { get; set; } = default!;
    internal virtual OutlinedBorder _disabledBorder { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual Axis _direction { get; set; } = default!;
    internal virtual double _tapTargetVerticalPadding { get; set; } = default!;
    internal virtual bool _isExpanded { get; set; } = default!;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderSegmentedButton__segmented_button(List<ButtonSegment<T>> segments, OutlinedBorder enabledBorder, OutlinedBorder disabledBorder, TextDirection textDirection, double tapTargetVerticalPadding, bool isExpanded, Axis direction)
    {
        _segments = segments;
        _enabledBorder = enabledBorder;
        _disabledBorder = disabledBorder;
        _textDirection = textDirection;
        _direction = direction;
        _tapTargetVerticalPadding = tapTargetVerticalPadding;
        _isExpanded = isExpanded;
    }

    public virtual List<ButtonSegment<T>> segments
    {
        get => _segments;
        set
        {
            var __value = value;
            if (CollectionsLibrary.listEquals(segments, __value))
            {
                return;
            }
            _segments = __value;
            markNeedsLayout();
        }
    }
    public virtual OutlinedBorder enabledBorder
    {
        get => _enabledBorder;
        set
        {
            var __value = value;
            if (Equals(_enabledBorder, __value))
            {
                return;
            }
            _enabledBorder = __value;
            markNeedsLayout();
        }
    }
    public virtual OutlinedBorder disabledBorder
    {
        get => _disabledBorder;
        set
        {
            var __value = value;
            if (Equals(_disabledBorder, __value))
            {
                return;
            }
            _disabledBorder = __value;
            markNeedsLayout();
        }
    }
    public virtual TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(__value, _textDirection))
            {
                return;
            }
            _textDirection = __value;
            markNeedsLayout();
        }
    }
    public virtual Axis direction
    {
        get => _direction;
        set
        {
            var __value = value;
            if (Equals(__value, _direction))
            {
                return;
            }
            _direction = __value;
            markNeedsLayout();
        }
    }
    public virtual double tapTargetVerticalPadding
    {
        get => _tapTargetVerticalPadding;
        set
        {
            var __value = value;
            if (__value == _tapTargetVerticalPadding)
            {
                return;
            }
            _tapTargetVerticalPadding = __value;
            markNeedsLayout();
        }
    }
    public virtual bool isExpanded
    {
        get => _isExpanded;
        set
        {
            var __value = value;
            if (__value == _isExpanded)
            {
                return;
            }
            _isExpanded = __value;
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        RenderBox? child = firstChild;
        var minWidth = 0.0;
        while (child is not null)
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)child.parentData!)!;
            double childWidth = child.getMinIntrinsicWidth(height);
            minWidth = Math.Max(minWidth, childWidth);
            child = childParentData.nextSibling;
        }
        return minWidth * childCount;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        RenderBox? child = firstChild;
        var maxWidth = 0.0;
        while (child is not null)
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)child.parentData!)!;
            double childWidth = child.getMaxIntrinsicWidth(height);
            maxWidth = Math.Max(maxWidth, childWidth);
            child = childParentData.nextSibling;
        }
        return maxWidth * childCount;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        RenderBox? child = firstChild;
        var minHeight = 0.0;
        while (child is not null)
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)child.parentData!)!;
            double childHeight = child.getMinIntrinsicHeight(width);
            minHeight = Math.Max(minHeight, childHeight);
            child = childParentData.nextSibling;
        }
        return minHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        RenderBox? child = firstChild;
        var maxHeight = 0.0;
        while (child is not null)
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)child.parentData!)!;
            double childHeight = child.getMaxIntrinsicHeight(width);
            maxHeight = Math.Max(maxHeight, childHeight);
            child = childParentData.nextSibling;
        }
        return maxHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return defaultComputeDistanceToHighestActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)child;
        if (__child.parentData is not _SegmentedButtonContainerBoxParentData__segmented_button)
        {
            __child.parentData = new _SegmentedButtonContainerBoxParentData__segmented_button();
        }
    }

    internal virtual void _layoutRects(Func<RenderBox, RenderBox?> nextChild, RenderBox? leftChild, RenderBox? rightChild)
    {
        var child = leftChild;
        var start = 0.0;
        while (child is not null)
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)child.parentData!)!;
            RRect rChildRect = default!;
            if (Equals(direction, Axis.vertical))
            {
                childParentData.offset = new Offset(0.0, start);
                var childRect = Rect.fromLTWH(0.0, childParentData.offset.dy, child.size.width, child.size.height);
                rChildRect = RRect.fromRectAndCorners(childRect);
                start += child.size.height;
            }
            else
            {
                childParentData.offset = new Offset(start, 0.0);
                var childRectLocal = Rect.fromLTWH(start, 0.0, child.size.width, child.size.height);
                rChildRect = RRect.fromRectAndCorners(childRectLocal);
                start += child.size.width;
            }
            childParentData.surroundingRect = rChildRect;
            child = nextChild(child);
        }
    }

    internal virtual Size _calculateChildSize(BoxConstraints constraints)
    {
        return Equals(direction, Axis.horizontal) ? _calculateHorizontalChildSize(constraints) : _calculateVerticalChildSize(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _calculateHorizontalChildSize(BoxConstraints constraints)
    {
        double maxHeight = 0;
        RenderBox? child = firstChild;
        double childWidth = default!;
        if (_isExpanded)
        {
            childWidth = constraints.maxWidth / childCount;
        }
        else
        {
            childWidth = constraints.minWidth / childCount;
            while (child is not null)
            {
                childWidth = Math.Max(childWidth, child.getMaxIntrinsicWidth(double.PositiveInfinity));
                child = childAfter(child);
            }
            childWidth = Math.Min(childWidth, constraints.maxWidth / childCount);
        }
        child = firstChild;
        while (child is not null)
        {
            double boxHeight = child.getMaxIntrinsicHeight(childWidth);
            maxHeight = Math.Max(maxHeight, boxHeight);
            child = childAfter(child);
        }
        return new Size(childWidth, maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _calculateVerticalChildSize(BoxConstraints constraints)
    {
        double maxWidthLocal = 0;
        RenderBox? child = firstChild;
        double childHeight = default!;
        if (_isExpanded)
        {
            childHeight = constraints.maxHeight / childCount;
        }
        else
        {
            childHeight = constraints.minHeight / childCount;
            while (child is not null)
            {
                childHeight = Math.Max(childHeight, child.getMaxIntrinsicHeight(double.PositiveInfinity));
                child = childAfter(child);
            }
            childHeight = Math.Min(childHeight, constraints.maxHeight / childCount);
        }
        child = firstChild;
        while (child is not null)
        {
            double boxWidth = child.getMaxIntrinsicWidth(maxWidthLocal);
            maxWidthLocal = Math.Max(maxWidthLocal, boxWidth);
            child = childAfter(child);
        }
        var childSize = new Size(maxWidthLocal, childHeight);
        if (constraints.hasTightWidth && (childSize.width < constraints.maxWidth))
        {
            childSize = new Size(constraints.maxWidth, childSize.height);
        }
        return childSize;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _computeOverallSizeFromChildSize(Size childSize)
    {
        if (Equals(direction, Axis.vertical))
        {
            return constraints.constrain(new Size(childSize.width, childSize.height * childCount));
        }
        return constraints.constrain(new Size(childSize.width * childCount, childSize.height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        Size childSize = _calculateChildSize(constraints);
        return _computeOverallSizeFromChildSize(childSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        Size childSize = _calculateChildSize(constraints);
        var childConstraints = BoxConstraints.CreateTight(childSize);
        BaselineOffset baselineOffset = BaselineOffset.noBaseline;
        for (RenderBox? child = firstChild; child is not null; child = childAfter(child))
        {
            baselineOffset = baselineOffset.minOf(new BaselineOffset(child.getDryBaseline(childConstraints, baseline)));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        Size childSize = _calculateChildSize(constraintsLocal);
        var childConstraints = BoxConstraints.CreateTightFor(width: childSize.width, height: childSize.height);
        RenderBox? child = firstChild;
        while (child is not null)
        {
            child.layout(childConstraints, parentUsesSize: true);
            child = childAfter(child);
        }
        switch (textDirection)
        {
            case TextDirection.rtl:
                {
                    _layoutRects(childBefore, lastChild, firstChild);
                    break;
                }
            case TextDirection.ltr:
                {
                    _layoutRects(childAfter, firstChild, lastChild);
                    break;
                }
        }
        size = _computeOverallSizeFromChildSize(childSize);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        Rect borderRect = offset + new Offset(0, tapTargetVerticalPadding / 2L) & new Size(size.width, size.height - tapTargetVerticalPadding);
        Path borderClipPath = enabledBorder.getInnerPath(borderRect, textDirection: textDirection);
        RenderBox? child = firstChild;
        RenderBox? previousChild = default!;
        var index = 0L;
        Path? enabledClipPath = default!;
        Path? disabledClipPath = default!;
        while (child is not null)
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)child.parentData!)!;
            Rect childRect = childParentData.surroundingRect!.outerRect.shift(offset);
            DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = context.canvas;
    __cascade.save();
    __cascade.clipPath(borderClipPath);
    return __cascade;
}))());
            context.paintChild(child, childParentData.offset + offset);
            context.canvas.restore();
            double segmentLeft = default!;
            double segmentRight = default!;
            double dividerPos = default!;
            double borderOutset = Math.Max(enabledBorder.side.strokeOutset, disabledBorder.side.strokeOutset);
            switch (textDirection)
            {
                case TextDirection.rtl:
                    {
                        segmentLeft = Equals(child, lastChild) ? (borderRect.left - borderOutset) : childRect.left;
                        segmentRight = Equals(child, firstChild) ? (borderRect.right + borderOutset) : childRect.right;
                        dividerPos = segmentRight;
                        break;
                    }
                case TextDirection.ltr:
                    {
                        segmentLeft = Equals(child, firstChild) ? (borderRect.left - borderOutset) : childRect.left;
                        segmentRight = Equals(child, lastChild) ? (borderRect.right + borderOutset) : childRect.right;
                        dividerPos = segmentLeft;
                        break;
                    }
            }
            var segmentClipRect = Rect.fromLTRB(segmentLeft, borderRect.top - borderOutset, segmentRight, borderRect.bottom + borderOutset);
            if (segments[(int)index].enabled)
            {
                enabledClipPath = ((Func<Path>)(() =>
{
    var __cascade = enabledClipPath ?? new Path();
    __cascade.addRect(segmentClipRect);
    return __cascade;
}))();
            }
            else
            {
                disabledClipPath = ((Func<Path>)(() =>
{
    var __cascade = disabledClipPath ?? new Path();
    __cascade.addRect(segmentClipRect);
    return __cascade;
}))();
            }
            if (previousChild is not null)
            {
                BorderSide divider = (segments[(int)(index - 1L)].enabled || segments[(int)index].enabled) ? enabledBorder.side.copyWith(strokeAlign: 0.0) : disabledBorder.side.copyWith(strokeAlign: 0.0);
                if (Equals(direction, Axis.horizontal))
                {
                    var topLocal = new Offset(dividerPos, borderRect.top);
                    var bottomLocal = new Offset(dividerPos, borderRect.bottom);
                    context.canvas.drawLine(topLocal, bottomLocal, divider.toPaint());
                }
                else
                {
                    if (Equals(direction, Axis.vertical))
                    {
                        var start = new Offset(borderRect.left, childRect.top);
                        var end = new Offset(borderRect.right, childRect.top);
                        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = context.canvas;
    __cascade.save();
    __cascade.clipPath(borderClipPath);
    return __cascade;
}))());
                        context.canvas.drawLine(start, end, divider.toPaint());
                        context.canvas.restore();
                    }
                }
            }
            previousChild = child;
            child = childAfter(child);
            index += 1L;
        }
        if (disabledClipPath is null)
        {
            enabledBorder.paint(context.canvas, borderRect, textDirection: textDirection);
        }
        else
        {
            if (enabledClipPath is null)
            {
                disabledBorder.paint(context.canvas, borderRect, textDirection: textDirection);
            }
            else
            {
                DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = context.canvas;
    __cascade.save();
    __cascade.clipPath(enabledClipPath);
    return __cascade;
}))());
                enabledBorder.paint(context.canvas, borderRect, textDirection: textDirection);
                DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = context.canvas;
    __cascade.restore();
    __cascade.save();
    __cascade.clipPath(disabledClipPath);
    return __cascade;
}))());
                disabledBorder.paint(context.canvas, borderRect, textDirection: textDirection);
                context.canvas.restore();
            }
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while (child is not null)
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)child.parentData!)!;
            if (childParentData.surroundingRect!.contains(position))
            {
                return result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (result, localOffset) =>
                {
                    DartRuntimePrimitives.Assert(() => Equals(localOffset, position - childParentData.offset));
                    return child!.hitTest(result, position: localOffset);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => _childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderBox)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((ContainerBoxParentData<RenderBox>?)_firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            _firstChild = child;
            _lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _firstChild is not null);
            DartRuntimePrimitives.Assert(() => _lastChild is not null);
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: _firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: _lastChild));
            var afterParentData = ((ContainerBoxParentData<RenderBox>?)after.parentData!)!;
            if (afterParentData.nextSibling is null)
            {
                DartRuntimePrimitives.Assert(() => Equals(after, _lastChild));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                _lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((ContainerBoxParentData<RenderBox>?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((ContainerBoxParentData<RenderBox>?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => !Equals(after, this), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => !Equals(child, after), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is ContainerBoxParentData<RenderBox>, () => (object?)$"A child of {GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {typeof(ContainerBoxParentData<RenderBox>)}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {typeof(ContainerBoxParentData<RenderBox>)}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((System.Action<RenderBox>)add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: _firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: _lastChild));
        DartRuntimePrimitives.Assert(() => _childCount >= 0L);
        if (childParentData.previousSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_firstChild, child));
            _firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((ContainerBoxParentData<RenderBox>?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((ContainerBoxParentData<RenderBox>?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        _childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        if (Equals(childParentData.previousSibling, after))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(System.Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if (firstChild is not null)
        {
            RenderBox child = firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if (Equals(child, lastChild))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if (result is not null)
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy;
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            BaselineOffset candidate = new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy);
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(transformed, position - childParentData.offset));
                return child!.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            if (isHit)
            {
                return true;
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            context.paintChild(child, childParentData.offset + offset);
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            result.Add(child!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SegmentedButtonDefaultsM3__segmented_button : SegmentedButtonThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = _theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _SegmentedButtonDefaultsM3__segmented_button(BuildContext context)
    {
        this.context = context;
    }

    public override ButtonStyle? style
    {
        get
        {
            return new ButtonStyle(textStyle: new WidgetStatePropertyAll<TextStyle?>(Theme.of(context).textTheme.labelLarge), backgroundColor: WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return null;
                }
                if (states.Contains(WidgetState.selected))
                {
                    return _colors.secondaryContainer;
                }
                return null;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), foregroundColor: WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return _colors.onSurface.withOpacity(0.38);
                }
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onSecondaryContainer;
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onSecondaryContainer;
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onSecondaryContainer;
                    }
                    return _colors.onSecondaryContainer;
                }
                else
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onSurface;
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onSurface;
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onSurface;
                    }
                    return _colors.onSurface;
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), overlayColor: WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onSecondaryContainer.withOpacity(0.1);
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onSecondaryContainer.withOpacity(0.08);
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onSecondaryContainer.withOpacity(0.1);
                    }
                }
                else
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onSurface.withOpacity(0.1);
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onSurface.withOpacity(0.08);
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onSurface.withOpacity(0.1);
                    }
                }
                return null;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), surfaceTintColor: new WidgetStatePropertyAll<Color>(Colors.transparent), elevation: new WidgetStatePropertyAll<double?>(0), iconSize: new WidgetStatePropertyAll<double?>(18.0), side: WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return new BorderSide(color: _colors.onSurface.withOpacity(0.12));
                }
                return new BorderSide(color: _colors.outline);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), shape: new WidgetStatePropertyAll<OutlinedBorder>(new StadiumBorder()), minimumSize: new WidgetStatePropertyAll<Size?>(new Size(40.0)));
        }
    }
    public override Widget? selectedIcon => DartRuntimePrimitives.ConvertValue<Widget>(new Icon(Icons.check));
    public static WidgetStateProperty<Color?> resolveStateColor(Color? unselectedColor, Color? selectedColor, Color? overlayColor)
    {
        Color? selectedLocal = overlayColor ?? selectedColor;
        Color? unselected = overlayColor ?? unselectedColor;
        return WidgetStateProperty<Color?>.CreateFromMap(new DartMap<WidgetStatesConstraint, Color?> { [WidgetState.selected.asConstraint().op_BitwiseAnd(WidgetState.pressed.asConstraint())] = selectedLocal?.withOpacity(0.1), [WidgetState.selected.asConstraint().op_BitwiseAnd(WidgetState.hovered.asConstraint())] = selectedLocal?.withOpacity(0.08), [WidgetState.selected.asConstraint().op_BitwiseAnd(WidgetState.focused.asConstraint())] = selectedLocal?.withOpacity(0.1), [WidgetState.pressed.asConstraint()] = unselected?.withOpacity(0.1), [WidgetState.hovered.asConstraint()] = unselected?.withOpacity(0.08), [WidgetState.focused.asConstraint()] = unselected?.withOpacity(0.1), [WidgetStateMembers.any] = Colors.transparent }.cast<WidgetStatesConstraint, Color?>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
