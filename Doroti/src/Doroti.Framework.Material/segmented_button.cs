// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/segmented_button.dart
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

public class ButtonSegment<T>
{
    public virtual T value { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? icon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? label { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual bool enabled { get; private set; } = default!;

    public ButtonSegment(T value, global::Doroti.Framework.Widgets.Widget? icon = null, global::Doroti.Framework.Widgets.Widget? label = null, string? tooltip = null, bool enabled = true)
    {
        this.value = value;
        this.icon = icon;
        this.label = label;
        this.tooltip = tooltip;
        this.enabled = enabled;
        System.Diagnostics.Debug.Assert(((icon is not null) || (label is not null)));
    }

}

public class SegmentedButton<T> : global::Doroti.Framework.Widgets.StatefulWidget where T : notnull
{
    public virtual List<ButtonSegment<T>> segments { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis direction { get; private set; } = default!;
    public virtual HashSet<T> selected { get; private set; } = default!;
    public virtual global::System.Action<HashSet<T>>? onSelectionChanged { get; private set; }
    public virtual bool multiSelectionEnabled { get; private set; } = default!;
    public virtual bool emptySelectionAllowed { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets? expandedInsets { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual bool showSelectedIcon { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? selectedIcon { get; private set; }

    public SegmentedButton(global::Doroti.Framework.Foundation.Key? key = null, List<ButtonSegment<T>> segments = default!, HashSet<T> selected = default!, global::System.Action<HashSet<T>>? onSelectionChanged = null, bool multiSelectionEnabled = false, bool emptySelectionAllowed = false, global::Doroti.Framework.Painting.EdgeInsets? expandedInsets = null, ButtonStyle? style = null, bool showSelectedIcon = true, global::Doroti.Framework.Widgets.Widget? selectedIcon = null, global::Doroti.Framework.Painting.Axis direction = global::Doroti.Framework.Painting.Axis.horizontal) : base(key: key)
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
        System.Diagnostics.Debug.Assert((checked((long)(segments.Count)) > 0L));
        System.Diagnostics.Debug.Assert(((checked((long)(selected.Count)) > 0L) || emptySelectionAllowed));
        System.Diagnostics.Debug.Assert(((checked((long)(selected.Count)) < 2L) || multiSelectionEnabled));
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? selectedForegroundColor = null, Color? selectedBackgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? iconColor = null, double? iconSize = null, Color? disabledIconColor = null, Color? overlayColor = null, double? elevation = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Framework.Services.MouseCursor? enabledMouseCursor = null, global::Doroti.Framework.Services.MouseCursor? disabledMouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null)
    {
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColorProp = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)(((((foregroundColor is null) && (selectedForegroundColor is null)) && (overlayColor is null))) ? null : (overlayColor switch { (global::Doroti.Ui.Color overlayColorLocal) when ((overlayColorLocal.value == 0L)) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(Colors.transparent)), _ => _SegmentedButtonDefaultsM3__segmented_button.resolveStateColor(foregroundColor, selectedForegroundColor, overlayColor) })));
        return TextButton.styleFrom(textStyle: textStyle, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, iconColor: iconColor, iconSize: iconSize, disabledIconColor: disabledIconColor, elevation: elevation, padding: padding, minimumSize: minimumSize, fixedSize: fixedSize, maximumSize: maximumSize, side: side, shape: shape, enabledMouseCursor: enabledMouseCursor, disabledMouseCursor: disabledMouseCursor, visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory).copyWith(foregroundColor: SegmentedButton<T>._defaultColor(foregroundColor, disabledForegroundColor, selectedForegroundColor), backgroundColor: SegmentedButton<T>._defaultColor(backgroundColor, disabledBackgroundColor, selectedBackgroundColor), overlayColor: overlayColorProp);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? _defaultColor(Color? enabled, Color? disabled, Color? selected)
    {
        if (((((selected ?? enabled) ?? disabled)) is null))
        {
            return null;
        }
        return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Framework.Widgets.WidgetState.disabled.asConstraint()] = disabled, [global::Doroti.Framework.Widgets.WidgetState.selected.asConstraint()] = selected, [global::Doroti.Framework.Widgets.WidgetStateMembers.any] = enabled }.cast<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Ui.Color?>()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new SegmentedButtonState<T>());
}

public class SegmentedButtonState<T> : global::Doroti.Framework.Widgets.State<SegmentedButton<T>>
{
    internal virtual bool _hovering { get; set; } = false;
    internal virtual bool _focused { get; set; } = false;
    public virtual DartMap<ButtonSegment<T>, global::Doroti.Framework.Widgets.WidgetStatesController> statesControllers { get; private set; } = new DartMap<ButtonSegment<T>, global::Doroti.Framework.Widgets.WidgetStatesController>();

    internal virtual bool _enabled => DartRuntimePrimitives.ConvertValue<bool>((((SegmentedButton<T>)(object)this.widget).onSelectionChanged is not null));
    internal virtual bool _selected => System.Linq.Enumerable.Any(((SegmentedButton<T>)(object)this.widget).selected);
    internal virtual HashSet<global::Doroti.Framework.Widgets.WidgetState> _states => ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection16615 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (!this._enabled) { __collection16615.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } if (this._hovering) { __collection16615.Add(global::Doroti.Framework.Widgets.WidgetState.hovered); } if (this._focused) { __collection16615.Add(global::Doroti.Framework.Widgets.WidgetState.focused); } if (this._selected) { __collection16615.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection16615; }))();
    public override void didUpdateWidget(SegmentedButton<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(oldWidget, this.widget)))
        {
            this.statesControllers.removeWhere(((segment, controller) =>
            {
                if (((SegmentedButton<T>)(object)this.widget).segments.Contains(segment))
                {
                    return false;
                }
                else
                {
                    controller.dispose();
                    return true;
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
    }

    internal virtual void _handleOnPressed(T segmentValue)
    {
        if (!this._enabled)
        {
            return;
        }
        bool onlySelectedSegment = ((checked((long)(((SegmentedButton<T>)(object)this.widget).selected.Count)) == 1L) && ((SegmentedButton<T>)(object)this.widget).selected.Contains(segmentValue));
        bool validChange = (((SegmentedButton<T>)(object)this.widget).emptySelectionAllowed || !onlySelectedSegment);
        if (validChange)
        {
            bool toggle = (((SegmentedButton<T>)(object)this.widget).multiSelectionEnabled || ((((SegmentedButton<T>)(object)this.widget).emptySelectionAllowed && onlySelectedSegment)));
            var pressedSegment = new HashSet<T> { segmentValue };
            HashSet<T> updatedSelection = default!;
            if (toggle)
            {
                updatedSelection = (((SegmentedButton<T>)(object)this.widget).selected.Contains(segmentValue) ? ((SegmentedButton<T>)(object)this.widget).selected.difference<T>(pressedSegment) : ((SegmentedButton<T>)(object)this.widget).selected.Union(pressedSegment).ToHashSet());
            }
            else
            {
                updatedSelection = pressedSegment;
            }
            if (!global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals(updatedSelection, ((SegmentedButton<T>)(object)this.widget).selected))
            {
                ((SegmentedButton<T>)(object)this.widget).onSelectionChanged!(updatedSelection);
            }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        SegmentedButtonThemeData theme = SegmentedButtonTheme.of(context);
        SegmentedButtonThemeData defaults = ((SegmentedButtonThemeData)(object?)new _SegmentedButtonDefaultsM3__segmented_button(context));
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        var disabledState = new HashSet<global::Doroti.Framework.Widgets.WidgetState> { global::Doroti.Framework.Widgets.WidgetState.disabled };
        P? effectiveValue<P>(global::System.Func<ButtonStyle?, P?> getProperty)
        {
            P? widgetValue = getProperty(((SegmentedButton<T>)(object)this.widget).style);
            P? themeValue = getProperty(theme.style);
            P? defaultValue = getProperty(defaults.style);
            return ((widgetValue ?? themeValue) ?? defaultValue);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(global::System.Func<ButtonStyle?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty, HashSet<global::Doroti.Framework.Widgets.WidgetState>? states = null)
        {
            return effectiveValue(((style) => DartRuntimePrimitives.NullAware(getProperty(style), __target => __target.resolve(((states ?? (HashSet<global::Doroti.Framework.Widgets.WidgetState>)this._states))))));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        ButtonStyle segmentStyleFor(ButtonStyle? style)
        {
            return new ButtonStyle(textStyle: style?.textStyle, backgroundColor: style?.backgroundColor, foregroundColor: style?.foregroundColor, overlayColor: style?.overlayColor, surfaceTintColor: style?.surfaceTintColor, elevation: style?.elevation, padding: style?.padding, iconColor: style?.iconColor, iconSize: style?.iconSize, shape: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder()), mouseCursor: style?.mouseCursor, visualDensity: style?.visualDensity, tapTargetSize: style?.tapTargetSize, animationDuration: style?.animationDuration, enableFeedback: style?.enableFeedback, alignment: style?.alignment, splashFactory: style?.splashFactory);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        ButtonStyle segmentStyle = segmentStyleFor(((SegmentedButton<T>)(object)this.widget).style);
        ButtonStyle segmentThemeStyle = segmentStyleFor(theme.style).merge(segmentStyleFor(defaults.style));
        global::Doroti.Framework.Widgets.Widget? selectedIconLocal = (((SegmentedButton<T>)(object)this.widget).showSelectedIcon ? ((((SegmentedButton<T>)(object)this.widget).selectedIcon ?? theme.selectedIcon) ?? defaults.selectedIcon) : null);
        global::Doroti.Framework.Widgets.Widget buttonFor(ButtonSegment<T> segment)
        {
            global::Doroti.Framework.Widgets.Widget labelLocal = ((((ButtonSegment<T>)segment).label ?? ((ButtonSegment<T>)segment).icon) ?? global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
            bool segmentSelected = ((SegmentedButton<T>)(object)this.widget).selected.Contains(((ButtonSegment<T>)segment).value);
            global::Doroti.Framework.Widgets.Widget? iconLocal = (((segmentSelected && ((SegmentedButton<T>)(object)this.widget).showSelectedIcon)) ? selectedIconLocal : ((((ButtonSegment<T>)segment).label is not null) ? ((ButtonSegment<T>)segment).icon : null));
            global::Doroti.Framework.Widgets.WidgetStatesController controller = ((global::Doroti.Framework.Widgets.WidgetStatesController)(object?)this.statesControllers.putIfAbsent(segment, (() => new global::Doroti.Framework.Widgets.WidgetStatesController())));
            controller.update(global::Doroti.Framework.Widgets.WidgetState.selected, segmentSelected);
            var content = labelLocal;
            var effectiveSegmentStyle = segmentStyle;
            if ((iconLocal is not null))
            {
                bool useMaterial3Local = Theme.of(context).useMaterial3;
                double defaultFontSize = (segmentStyle.textStyle?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.fontSize ?? 14.0);
                double effectiveTextScale = (MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0);
                global::Doroti.Framework.Painting.EdgeInsetsGeometry scaledPaddingLocal = ButtonStyleButton.scaledPadding((useMaterial3Local ? new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12, 8, 16, 8) : global::Doroti.Framework.Painting.EdgeInsets.CreateAll(8)), global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4), global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4), effectiveTextScale);
                effectiveSegmentStyle = segmentStyle.copyWith(padding: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(scaledPaddingLocal));
                double scaleLocal = (Dart_uiLibrary.clampDouble(effectiveTextScale, 1.0, 2.0) - 1.0);
                TextButtonThemeData textButtonTheme = TextButtonTheme.of(context);
                IconAlignment effectiveIconAlignment = ((textButtonTheme.style?.iconAlignment ?? segmentStyle.iconAlignment) ?? IconAlignment.start);
                content = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, spacing: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(8L, 4L, scaleLocal)), children: ((object.Equals(effectiveIconAlignment, IconAlignment.start)) ? new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(iconLocal), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: labelLocal)) } : new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: labelLocal)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(iconLocal) })));
            }
            global::Doroti.Framework.Widgets.Widget button = ((global::Doroti.Framework.Widgets.Widget)(object?)new TextButton(style: effectiveSegmentStyle, statesController: controller, onHover: ((global::System.Action<bool>)((hovering) =>
            {
                setState(((global::System.Action)(() =>
                {
                    _hovering = hovering;
                })));
            })), onFocusChange: ((global::System.Action<bool>)((focused) =>
            {
                setState(((global::System.Action)(() =>
                {
                    _focused = focused;
                })));
            })), onPressed: ((global::System.Action)(((this._enabled && ((ButtonSegment<T>)segment).enabled)) ? (() => { _handleOnPressed(((ButtonSegment<T>)segment).value); }) : null)), child: content));
            global::Doroti.Framework.Widgets.Widget buttonWithTooltip = ((((ButtonSegment<T>)segment).tooltip is not null) ? new Tooltip(message: ((ButtonSegment<T>)segment).tooltip, child: button) : button);
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.MergeSemantics(child: new global::Doroti.Framework.Widgets.Semantics(selected: segmentSelected, inMutuallyExclusiveGroup: (((SegmentedButton<T>)(object)this.widget).multiSelectionEnabled ? null : true), child: buttonWithTooltip)));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Framework.Painting.OutlinedBorder effectiveBorder = (resolve<global::Doroti.Framework.Painting.OutlinedBorder?>(((style) => style?.shape)) ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder());
        global::Doroti.Framework.Painting.OutlinedBorder resolvedDisabledBorder = (resolve<global::Doroti.Framework.Painting.OutlinedBorder?>(((style) => style?.shape), disabledState) ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder());
        global::Doroti.Framework.Painting.BorderSide effectiveSide = (resolve<global::Doroti.Framework.Painting.BorderSide?>(((style) => style?.side)) ?? global::Doroti.Framework.Painting.BorderSide.none);
        global::Doroti.Framework.Painting.BorderSide disabledSide = (resolve<global::Doroti.Framework.Painting.BorderSide?>(((style) => style?.side), disabledState) ?? global::Doroti.Framework.Painting.BorderSide.none);
        global::Doroti.Framework.Painting.OutlinedBorder enabledBorderLocal = ((global::Doroti.Framework.Painting.OutlinedBorder)(object?)effectiveBorder.copyWith(side: effectiveSide));
        global::Doroti.Framework.Painting.OutlinedBorder disabledBorderLocal = ((global::Doroti.Framework.Painting.OutlinedBorder)(object?)resolvedDisabledBorder.copyWith(side: disabledSide));
        VisualDensity resolvedVisualDensity = ((segmentStyle.visualDensity ?? segmentThemeStyle.visualDensity) ?? Theme.of(context).visualDensity);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry resolvedPadding = (resolve<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(((style) => style?.padding)) ?? global::Doroti.Framework.Painting.EdgeInsets.zero);
        MaterialTapTargetSize resolvedTapTargetSize = ((segmentStyle.tapTargetSize ?? segmentThemeStyle.tapTargetSize) ?? Theme.of(context).materialTapTargetSize);
        double fontSizeLocal = (resolve<global::Doroti.Framework.Painting.TextStyle?>(((style) => style?.textStyle))?.fontSize ?? 20.0);
        List<global::Doroti.Framework.Widgets.Widget> buttons = ((SegmentedButton<T>)(object)this.widget).segments.map<ButtonSegment<T>, global::Doroti.Framework.Widgets.Widget>(buttonFor).ToList().ToList();
        global::Doroti.Ui.Offset densityAdjustment = ((global::Doroti.Ui.Offset)(object?)resolvedVisualDensity.baseSizeAdjustment);
        var textButtonMinHeight = 40.0;
        double adjustButtonMinHeight = (textButtonMinHeight + densityAdjustment.dy);
        double effectiveVerticalPadding = (((global::Doroti.Framework.Painting.EdgeInsetsGeometry)resolvedPadding).vertical + (densityAdjustment.dy * 2L));
        double effectedButtonHeight = Math.Max((fontSizeLocal + effectiveVerticalPadding), adjustButtonMinHeight);
        double tapTargetVerticalPaddingLocal = (resolvedTapTargetSize switch { var __constant25770 when (object.Equals(__constant25770, MaterialTapTargetSize.shrinkWrap)) => 0.0, var __constant25817 when (object.Equals(__constant25817, MaterialTapTargetSize.padded)) => Math.Max(0, ((global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dy) - effectedButtonHeight)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Material(type: MaterialType.transparency, elevation: DartRuntimePrimitives.RequireValue(resolve<double?>(((style) => style?.elevation))), shadowColor: resolve<global::Doroti.Ui.Color?>(((style) => style?.shadowColor)), surfaceTintColor: resolve<global::Doroti.Ui.Color?>(((style) => style?.surfaceTintColor)), child: new TextButtonTheme(data: new TextButtonThemeData(style: segmentThemeStyle), child: new global::Doroti.Framework.Widgets.Padding(padding: (((SegmentedButton<T>)(object)this.widget).expandedInsets ?? global::Doroti.Framework.Painting.EdgeInsets.zero), child: new _SegmentedButtonRenderWidget__segmented_button<T>(tapTargetVerticalPadding: tapTargetVerticalPaddingLocal, segments: ((SegmentedButton<T>)(object)this.widget).segments, enabledBorder: (this._enabled ? enabledBorderLocal : disabledBorderLocal), disabledBorder: disabledBorderLocal, direction: ((SegmentedButton<T>)(object)this.widget).direction, textDirection: textDirectionLocal, isExpanded: (((SegmentedButton<T>)(object)this.widget).expandedInsets is not null), children: buttons)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Widgets.WidgetStatesController controller in this.statesControllers.Values)
        {
            controller.dispose();
        }
        base.dispose();
    }

}

internal class _SegmentedButtonRenderWidget__segmented_button<T> : global::Doroti.Framework.Widgets.MultiChildRenderObjectWidget
{
    public virtual List<ButtonSegment<T>> segments { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.OutlinedBorder enabledBorder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.OutlinedBorder disabledBorder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis direction { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual double tapTargetVerticalPadding { get; private set; } = default!;
    public virtual bool isExpanded { get; private set; } = default!;

    internal _SegmentedButtonRenderWidget__segmented_button(global::Doroti.Framework.Foundation.Key? key = null, List<ButtonSegment<T>> segments = default!, global::Doroti.Framework.Painting.OutlinedBorder enabledBorder = default!, global::Doroti.Framework.Painting.OutlinedBorder disabledBorder = default!, global::Doroti.Framework.Painting.Axis direction = default!, TextDirection textDirection = default!, double tapTargetVerticalPadding = default!, bool isExpanded = default!, List<global::Doroti.Framework.Widgets.Widget> children = default!) : base(key: key, children: children)
    {
        this.segments = segments;
        this.enabledBorder = enabledBorder;
        this.disabledBorder = disabledBorder;
        this.direction = direction;
        this.textDirection = textDirection;
        this.tapTargetVerticalPadding = tapTargetVerticalPadding;
        this.isExpanded = isExpanded;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) == checked((long)(segments.Count))));
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSegmentedButton__segmented_button<T>(segments: this.segments, enabledBorder: this.enabledBorder, disabledBorder: this.disabledBorder, textDirection: this.textDirection, direction: this.direction, tapTargetVerticalPadding: this.tapTargetVerticalPadding, isExpanded: this.isExpanded));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSegmentedButton__segmented_button<T>)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSegmentedButton__segmented_button<T>>)(() =>
{
    var __cascade = __renderObject;
    __cascade.segments = this.segments;
    __cascade.enabledBorder = this.enabledBorder;
    __cascade.disabledBorder = this.disabledBorder;
    __cascade.direction = this.direction;
    __cascade.textDirection = this.textDirection;
    return __cascade;
}))());
    }

}

internal class _SegmentedButtonContainerBoxParentData__segmented_button : global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>
{
    public virtual RRect? surroundingRect { get; set; } = default;

}

internal delegate global::Doroti.Framework.Rendering.RenderBox? _NextChild__segmented_button(global::Doroti.Framework.Rendering.RenderBox child);

public class _RenderSegmentedButton__segmented_button<T> : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>>, global::Doroti.Framework.Rendering.RenderBoxContainerDefaultsMixin<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>>
{
    internal virtual List<ButtonSegment<T>> _segments { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.OutlinedBorder _enabledBorder { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.OutlinedBorder _disabledBorder { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.Axis _direction { get; set; } = default!;
    internal virtual double _tapTargetVerticalPadding { get; set; } = default!;
    internal virtual bool _isExpanded { get; set; } = default!;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderSegmentedButton__segmented_button(List<ButtonSegment<T>> segments, global::Doroti.Framework.Painting.OutlinedBorder enabledBorder, global::Doroti.Framework.Painting.OutlinedBorder disabledBorder, TextDirection textDirection, double tapTargetVerticalPadding, bool isExpanded, global::Doroti.Framework.Painting.Axis direction)
    {
        this._segments = segments;
        this._enabledBorder = enabledBorder;
        this._disabledBorder = disabledBorder;
        this._textDirection = textDirection;
        this._direction = direction;
        this._tapTargetVerticalPadding = tapTargetVerticalPadding;
        this._isExpanded = isExpanded;
    }

    public virtual List<ButtonSegment<T>> segments
    {
        get => this._segments;
        set
        {
            var __value = value;
            if (global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(this.segments, __value))
            {
                return;
            }
            _segments = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder enabledBorder
    {
        get => this._enabledBorder;
        set
        {
            var __value = value;
            if ((object.Equals(this._enabledBorder, __value)))
            {
                return;
            }
            _enabledBorder = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder disabledBorder
    {
        get => this._disabledBorder;
        set
        {
            var __value = value;
            if ((object.Equals(this._disabledBorder, __value)))
            {
                return;
            }
            _disabledBorder = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._textDirection)))
            {
                return;
            }
            _textDirection = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.Axis direction
    {
        get => this._direction;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._direction)))
            {
                return;
            }
            _direction = __value;
            markNeedsLayout();
        }
    }
    public virtual double tapTargetVerticalPadding
    {
        get => this._tapTargetVerticalPadding;
        set
        {
            var __value = value;
            if ((__value == this._tapTargetVerticalPadding))
            {
                return;
            }
            _tapTargetVerticalPadding = __value;
            markNeedsLayout();
        }
    }
    public virtual bool isExpanded
    {
        get => this._isExpanded;
        set
        {
            var __value = value;
            if ((__value == this._isExpanded))
            {
                return;
            }
            _isExpanded = __value;
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        var minWidth = 0.0;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child.parentData!)!;
            double childWidth = child.getMinIntrinsicWidth(height);
            minWidth = Math.Max(minWidth, childWidth);
            child = childParentData.nextSibling;
        }
        return (minWidth * this.childCount);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        var maxWidth = 0.0;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child.parentData!)!;
            double childWidth = child.getMaxIntrinsicWidth(height);
            maxWidth = Math.Max(maxWidth, childWidth);
            child = childParentData.nextSibling;
        }
        return (maxWidth * this.childCount);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        var minHeight = 0.0;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child.parentData!)!;
            double childHeight = child.getMinIntrinsicHeight(width);
            minHeight = Math.Max(minHeight, childHeight);
            child = childParentData.nextSibling;
        }
        return minHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        var maxHeight = 0.0;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child.parentData!)!;
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

    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not _SegmentedButtonContainerBoxParentData__segmented_button))
        {
            __child.parentData = new _SegmentedButtonContainerBoxParentData__segmented_button();
        }
    }

    internal virtual void _layoutRects(global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RenderBox?> nextChild, global::Doroti.Framework.Rendering.RenderBox? leftChild, global::Doroti.Framework.Rendering.RenderBox? rightChild)
    {
        var child = leftChild;
        var start = 0.0;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child.parentData!)!;
            global::Doroti.Ui.RRect rChildRect = default!;
            if ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.vertical)))
            {
                childParentData.offset = new global::Doroti.Ui.Offset(0.0, start);
                var childRect = global::Doroti.Ui.Rect.fromLTWH(0.0, childParentData.offset.dy, ((global::Doroti.Framework.Rendering.RenderBox)child).size.width, ((global::Doroti.Framework.Rendering.RenderBox)child).size.height);
                rChildRect = global::Doroti.Ui.RRect.fromRectAndCorners(childRect);
                start += ((global::Doroti.Framework.Rendering.RenderBox)child).size.height;
            }
            else
            {
                childParentData.offset = new global::Doroti.Ui.Offset(start, 0.0);
                var childRectLocal = global::Doroti.Ui.Rect.fromLTWH(start, 0.0, ((global::Doroti.Framework.Rendering.RenderBox)child).size.width, ((global::Doroti.Framework.Rendering.RenderBox)child).size.height);
                rChildRect = global::Doroti.Ui.RRect.fromRectAndCorners(childRectLocal);
                start += ((global::Doroti.Framework.Rendering.RenderBox)child).size.width;
            }
            childParentData.surroundingRect = rChildRect;
            child = nextChild(child);
        }
    }

    internal virtual global::Doroti.Ui.Size _calculateChildSize(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal)) ? _calculateHorizontalChildSize(constraints) : _calculateVerticalChildSize(constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _calculateHorizontalChildSize(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        double maxHeight = 0;
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        double childWidth = default!;
        if (this._isExpanded)
        {
            childWidth = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth / this.childCount);
        }
        else
        {
            childWidth = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth / this.childCount);
            while ((child is not null))
            {
                childWidth = Math.Max(childWidth, child.getMaxIntrinsicWidth(double.PositiveInfinity));
                child = childAfter(child);
            }
            childWidth = Math.Min(childWidth, (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth / this.childCount));
        }
        child = this.firstChild;
        while ((child is not null))
        {
            double boxHeight = child.getMaxIntrinsicHeight(childWidth);
            maxHeight = Math.Max(maxHeight, boxHeight);
            child = childAfter(child);
        }
        return new global::Doroti.Ui.Size(childWidth, maxHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _calculateVerticalChildSize(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        double maxWidthLocal = 0;
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        double childHeight = default!;
        if (this._isExpanded)
        {
            childHeight = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight / this.childCount);
        }
        else
        {
            childHeight = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minHeight / this.childCount);
            while ((child is not null))
            {
                childHeight = Math.Max(childHeight, child.getMaxIntrinsicHeight(double.PositiveInfinity));
                child = childAfter(child);
            }
            childHeight = Math.Min(childHeight, (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight / this.childCount));
        }
        child = this.firstChild;
        while ((child is not null))
        {
            double boxWidth = child.getMaxIntrinsicWidth(maxWidthLocal);
            maxWidthLocal = Math.Max(maxWidthLocal, boxWidth);
            child = childAfter(child);
        }
        var childSize = new global::Doroti.Ui.Size(maxWidthLocal, childHeight);
        if ((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).hasTightWidth && (childSize.width < ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth)))
        {
            childSize = new global::Doroti.Ui.Size(((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth, childSize.height);
        }
        return childSize;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeOverallSizeFromChildSize(Size childSize)
    {
        if ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.vertical)))
        {
            return ((global::Doroti.Ui.Size)(object?)this.constraints.constrain(new global::Doroti.Ui.Size(childSize.width, (childSize.height * this.childCount))));
        }
        return ((global::Doroti.Ui.Size)(object?)this.constraints.constrain(new global::Doroti.Ui.Size((childSize.width * this.childCount), childSize.height)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)_calculateChildSize(constraints));
        return _computeOverallSizeFromChildSize(childSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)_calculateChildSize(constraints));
        var childConstraints = global::Doroti.Framework.Rendering.BoxConstraints.CreateTight(childSize);
        global::Doroti.Framework.Rendering.BaselineOffset baselineOffset = global::Doroti.Framework.Rendering.BaselineOffset.noBaseline;
        for (global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild; (child is not null); child = childAfter(child))
        {
            baselineOffset = baselineOffset.minOf(new global::Doroti.Framework.Rendering.BaselineOffset(child.getDryBaseline(childConstraints, baseline)));
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraintsLocal = this.constraints;
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)_calculateChildSize(constraintsLocal));
        var childConstraints = global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: childSize.width, height: childSize.height);
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            child.layout(childConstraints, parentUsesSize: true);
            child = childAfter(child);
        }
        switch (this.textDirection)
        {
            case TextDirection.rtl:
                {
                    _layoutRects((global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RenderBox?>)this.childBefore, this.lastChild, this.firstChild);
                    break;
                }
            case TextDirection.ltr:
                {
                    _layoutRects((global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RenderBox?>)this.childAfter, this.firstChild, this.lastChild);
                    break;
                }
        }
        size = _computeOverallSizeFromChildSize(childSize);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Ui.Rect borderRect = ((global::Doroti.Ui.Rect)(object?)(((offset + new global::Doroti.Ui.Offset(0, (this.tapTargetVerticalPadding / 2L)))) & (new global::Doroti.Ui.Size(this.size.width, (this.size.height - this.tapTargetVerticalPadding)))));
        global::Doroti.Ui.Path borderClipPath = ((global::Doroti.Ui.Path)(object?)this.enabledBorder.getInnerPath(borderRect, textDirection: this.textDirection));
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        global::Doroti.Framework.Rendering.RenderBox? previousChild = default!;
        var index = 0L;
        global::Doroti.Ui.Path? enabledClipPath = default!;
        global::Doroti.Ui.Path? disabledClipPath = default!;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child.parentData!)!;
            global::Doroti.Ui.Rect childRect = ((global::Doroti.Ui.Rect)(object?)((_SegmentedButtonContainerBoxParentData__segmented_button)childParentData).surroundingRect!.outerRect.shift(offset));
            DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas;
    __cascade.save();
    __cascade.clipPath(borderClipPath);
    return __cascade;
}))());
            context.paintChild(child, (childParentData.offset + offset));
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.restore();
            double segmentLeft = default!;
            double segmentRight = default!;
            double dividerPos = default!;
            double borderOutset = Math.Max(((global::Doroti.Framework.Painting.OutlinedBorder)this.enabledBorder).side.strokeOutset, ((global::Doroti.Framework.Painting.OutlinedBorder)this.disabledBorder).side.strokeOutset);
            switch (this.textDirection)
            {
                case TextDirection.rtl:
                    {
                        segmentLeft = ((object.Equals(child, this.lastChild)) ? (borderRect.left - borderOutset) : childRect.left);
                        segmentRight = ((object.Equals(child, this.firstChild)) ? (borderRect.right + borderOutset) : childRect.right);
                        dividerPos = segmentRight;
                        break;
                    }
                case TextDirection.ltr:
                    {
                        segmentLeft = ((object.Equals(child, this.firstChild)) ? (borderRect.left - borderOutset) : childRect.left);
                        segmentRight = ((object.Equals(child, this.lastChild)) ? (borderRect.right + borderOutset) : childRect.right);
                        dividerPos = segmentLeft;
                        break;
                    }
            }
            var segmentClipRect = global::Doroti.Ui.Rect.fromLTRB(segmentLeft, (borderRect.top - borderOutset), segmentRight, (borderRect.bottom + borderOutset));
            if (this.segments[(int)(index)].enabled)
            {
                enabledClipPath = ((Func<Path>)(() =>
{
    var __cascade = ((enabledClipPath ?? new global::Doroti.Ui.Path()));
    __cascade.addRect(segmentClipRect);
    return __cascade;
}))();
            }
            else
            {
                disabledClipPath = ((Func<Path>)(() =>
{
    var __cascade = ((disabledClipPath ?? new global::Doroti.Ui.Path()));
    __cascade.addRect(segmentClipRect);
    return __cascade;
}))();
            }
            if ((previousChild is not null))
            {
                global::Doroti.Framework.Painting.BorderSide divider = ((this.segments[(int)((index - 1L))].enabled || this.segments[(int)(index)].enabled) ? ((global::Doroti.Framework.Painting.OutlinedBorder)this.enabledBorder).side.copyWith(strokeAlign: 0.0) : ((global::Doroti.Framework.Painting.OutlinedBorder)this.disabledBorder).side.copyWith(strokeAlign: 0.0));
                if ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal)))
                {
                    var topLocal = new global::Doroti.Ui.Offset(dividerPos, borderRect.top);
                    var bottomLocal = new global::Doroti.Ui.Offset(dividerPos, borderRect.bottom);
                    ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawLine(topLocal, bottomLocal, divider.toPaint());
                }
                else
                {
                    if ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.vertical)))
                    {
                        var start = new global::Doroti.Ui.Offset(borderRect.left, childRect.top);
                        var end = new global::Doroti.Ui.Offset(borderRect.right, childRect.top);
                        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas;
    __cascade.save();
    __cascade.clipPath(borderClipPath);
    return __cascade;
}))());
                        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawLine(start, end, divider.toPaint());
                        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.restore();
                    }
                }
            }
            previousChild = child;
            child = childAfter(child);
            index += 1L;
        }
        if ((disabledClipPath is null))
        {
            this.enabledBorder.paint(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, borderRect, textDirection: this.textDirection);
        }
        else
        {
            if ((enabledClipPath is null))
            {
                this.disabledBorder.paint(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, borderRect, textDirection: this.textDirection);
            }
            else
            {
                DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas;
    __cascade.save();
    __cascade.clipPath(enabledClipPath);
    return __cascade;
}))());
                this.enabledBorder.paint(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, borderRect, textDirection: this.textDirection);
                DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas;
    __cascade.restore();
    __cascade.save();
    __cascade.clipPath(disabledClipPath);
    return __cascade;
}))());
                this.disabledBorder.paint(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, borderRect, textDirection: this.textDirection);
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.restore();
            }
        }
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.lastChild;
        while ((child is not null))
        {
            var childParentData = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child.parentData!)!;
            if (((_SegmentedButtonContainerBoxParentData__segmented_button)childParentData).surroundingRect!.contains(position))
            {
                return result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, localOffset) =>
                {
                    DartRuntimePrimitives.Assert(() => (object.Equals(localOffset, (position - childParentData.offset))));
                    return child!.hitTest(result, position: localOffset);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => this._childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderBox))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((ContainerBoxParentData<RenderBox>?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            this._firstChild = child;
            this._lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this._firstChild is not null));
            DartRuntimePrimitives.Assert(() => (this._lastChild is not null));
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: this._firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: this._lastChild));
            var afterParentData = ((ContainerBoxParentData<RenderBox>?)(object?)after.parentData!)!;
            if ((afterParentData.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is ContainerBoxParentData<RenderBox>), () => (object?)$"A child of {this.GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {typeof(ContainerBoxParentData<RenderBox>)}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {typeof(ContainerBoxParentData<RenderBox>)}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((global::System.Action<RenderBox>)this.add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        this._firstChild = null;
        this._lastChild = null;
        this._childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData.previousSibling, after)))
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
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            ((dynamic)child).attach(owner);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            ((dynamic)child).detach();
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child = this.firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if ((object.Equals(child, this.lastChild)))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if ((result is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy);
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            BaselineOffset candidate = (new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy));
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = this.lastChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((global::System.Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData.offset))));
                return child!.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
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
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            context.paintChild(child, (childParentData.offset + offset));
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SegmentedButtonDefaultsM3__segmented_button : SegmentedButtonThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
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
                __late__colors = this._theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _SegmentedButtonDefaultsM3__segmented_button(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual ButtonStyle? style
    {
        get
        {
            return new ButtonStyle(textStyle: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(Theme.of(this.context).textTheme.labelLarge), backgroundColor: WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    return null;
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return (this._colors.secondaryContainer);
                }
                return null;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), foregroundColor: WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    return (this._colors.onSurface.withOpacity(0.38));
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                    {
                        return (this._colors.onSecondaryContainer);
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                    {
                        return (this._colors.onSecondaryContainer);
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                    {
                        return (this._colors.onSecondaryContainer);
                    }
                    return (this._colors.onSecondaryContainer);
                }
                else
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                    {
                        return (this._colors.onSurface);
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                    {
                        return (this._colors.onSurface);
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                    {
                        return (this._colors.onSurface);
                    }
                    return (this._colors.onSurface);
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), overlayColor: WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                    {
                        return (this._colors.onSecondaryContainer.withOpacity(0.1));
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                    {
                        return (this._colors.onSecondaryContainer.withOpacity(0.08));
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                    {
                        return (this._colors.onSecondaryContainer.withOpacity(0.1));
                    }
                }
                else
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                    {
                        return (this._colors.onSurface.withOpacity(0.1));
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                    {
                        return (this._colors.onSurface.withOpacity(0.08));
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                    {
                        return (this._colors.onSurface.withOpacity(0.1));
                    }
                }
                return null;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), surfaceTintColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent), elevation: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(0), iconSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(18.0), side: WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    return (new global::Doroti.Framework.Painting.BorderSide(color: this._colors.onSurface.withOpacity(0.12)));
                }
                return (new global::Doroti.Framework.Painting.BorderSide(color: this._colors.outline));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), shape: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.StadiumBorder()), minimumSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size?>(new global::Doroti.Ui.Size(40.0)));
            return default!;
        }
    }
    public override global::Doroti.Framework.Widgets.Widget? selectedIcon => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Icon(Icons.check));
    public static global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> resolveStateColor(Color? unselectedColor, Color? selectedColor, Color? overlayColor)
    {
        global::Doroti.Ui.Color? selectedLocal = ((global::Doroti.Ui.Color?)(object?)(overlayColor ?? selectedColor));
        global::Doroti.Ui.Color? unselected = ((global::Doroti.Ui.Color?)(object?)(overlayColor ?? unselectedColor));
        return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>)(object?)global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Framework.Widgets.WidgetState.selected.asConstraint().op_BitwiseAnd(global::Doroti.Framework.Widgets.WidgetState.pressed.asConstraint())] = selectedLocal?.withOpacity(0.1), [global::Doroti.Framework.Widgets.WidgetState.selected.asConstraint().op_BitwiseAnd(global::Doroti.Framework.Widgets.WidgetState.hovered.asConstraint())] = selectedLocal?.withOpacity(0.08), [global::Doroti.Framework.Widgets.WidgetState.selected.asConstraint().op_BitwiseAnd(global::Doroti.Framework.Widgets.WidgetState.focused.asConstraint())] = selectedLocal?.withOpacity(0.1), [global::Doroti.Framework.Widgets.WidgetState.pressed.asConstraint()] = unselected?.withOpacity(0.1), [global::Doroti.Framework.Widgets.WidgetState.hovered.asConstraint()] = unselected?.withOpacity(0.08), [global::Doroti.Framework.Widgets.WidgetState.focused.asConstraint()] = unselected?.withOpacity(0.1), [global::Doroti.Framework.Widgets.WidgetStateMembers.any] = Colors.transparent }.cast<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Ui.Color?>()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
