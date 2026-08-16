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
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColorProp__11747 = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)(((((foregroundColor is null) && (selectedForegroundColor is null)) && (overlayColor is null))) ? null : (overlayColor switch { (global::Doroti.Ui.Color overlayColor__11933) when ((overlayColor__11933.value == 0L)) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(Colors.transparent)), _ => _SegmentedButtonDefaultsM3__segmented_button.resolveStateColor(foregroundColor, selectedForegroundColor, overlayColor) })));
        return TextButton.styleFrom(textStyle: textStyle, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, iconColor: iconColor, iconSize: iconSize, disabledIconColor: disabledIconColor, elevation: elevation, padding: padding, minimumSize: minimumSize, fixedSize: fixedSize, maximumSize: maximumSize, side: side, shape: shape, enabledMouseCursor: enabledMouseCursor, disabledMouseCursor: disabledMouseCursor, visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory).copyWith(foregroundColor: SegmentedButton<T>._defaultColor(foregroundColor, disabledForegroundColor, selectedForegroundColor), backgroundColor: SegmentedButton<T>._defaultColor(backgroundColor, disabledBackgroundColor, selectedBackgroundColor), overlayColor: overlayColorProp__11747);
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
            this.statesControllers.removeWhere(((segment, controller) => {
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
        bool onlySelectedSegment__17553 = ((checked((long)(((SegmentedButton<T>)(object)this.widget).selected.Count)) == 1L) && ((SegmentedButton<T>)(object)this.widget).selected.Contains(segmentValue));
        bool validChange__17669 = (((SegmentedButton<T>)(object)this.widget).emptySelectionAllowed || !onlySelectedSegment__17553);
        if (validChange__17669)
        {
            bool toggle__17777 = (((SegmentedButton<T>)(object)this.widget).multiSelectionEnabled || ((((SegmentedButton<T>)(object)this.widget).emptySelectionAllowed && onlySelectedSegment__17553)));
            var pressedSegment__17895 = new HashSet<T> { segmentValue };
            HashSet<T> updatedSelection__17955 = default!;
            if (toggle__17777)
            {
                updatedSelection__17955 = (((SegmentedButton<T>)(object)this.widget).selected.Contains(segmentValue) ? ((SegmentedButton<T>)(object)this.widget).selected.difference<T>(pressedSegment__17895) : ((SegmentedButton<T>)(object)this.widget).selected.Union(pressedSegment__17895).ToHashSet());
            }
            else
            {
                updatedSelection__17955 = pressedSegment__17895;
            }
            if (!global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals(updatedSelection__17955, ((SegmentedButton<T>)(object)this.widget).selected))
            {
                ((SegmentedButton<T>)(object)this.widget).onSelectionChanged!(updatedSelection__17955);
            }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        SegmentedButtonThemeData theme__18466 = SegmentedButtonTheme.of(context);
        SegmentedButtonThemeData defaults__18543 = ((SegmentedButtonThemeData)(object?)new _SegmentedButtonDefaultsM3__segmented_button(context));
        global::Doroti.Ui.TextDirection textDirection__18615 = Directionality.of(context);
        var disabledState__18669 = new HashSet<global::Doroti.Framework.Widgets.WidgetState> { global::Doroti.Framework.Widgets.WidgetState.disabled };
        P? effectiveValue<P>(global::System.Func<ButtonStyle?, P?> getProperty)
        {
            P? widgetValue__18815 = getProperty(((SegmentedButton<T>)(object)this.widget).style);
            P? themeValue__18876 = getProperty(theme__18466.style);
            P? defaultValue__18935 = getProperty(defaults__18543.style);
            return ((widgetValue__18815 ?? themeValue__18876) ?? defaultValue__18935);
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
        ButtonStyle segmentStyle__20173 = segmentStyleFor(((SegmentedButton<T>)(object)this.widget).style);
        ButtonStyle segmentThemeStyle__20241 = segmentStyleFor(theme__18466.style).merge(segmentStyleFor(defaults__18543.style));
        global::Doroti.Framework.Widgets.Widget? selectedIcon__20361 = (((SegmentedButton<T>)(object)this.widget).showSelectedIcon ? ((((SegmentedButton<T>)(object)this.widget).selectedIcon ?? theme__18466.selectedIcon) ?? defaults__18543.selectedIcon) : null);
        global::Doroti.Framework.Widgets.Widget buttonFor(ButtonSegment<T> segment)
        {
            global::Doroti.Framework.Widgets.Widget label__20562 = ((((ButtonSegment<T>)segment).label ?? ((ButtonSegment<T>)segment).icon) ?? global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
            bool segmentSelected__20645 = ((SegmentedButton<T>)(object)this.widget).selected.Contains(((ButtonSegment<T>)segment).value);
            global::Doroti.Framework.Widgets.Widget? icon__20724 = (((segmentSelected__20645 && ((SegmentedButton<T>)(object)this.widget).showSelectedIcon)) ? selectedIcon__20361 : ((((ButtonSegment<T>)segment).label is not null) ? ((ButtonSegment<T>)segment).icon : null));
            global::Doroti.Framework.Widgets.WidgetStatesController controller__20915 = ((global::Doroti.Framework.Widgets.WidgetStatesController)(object?)this.statesControllers.putIfAbsent(segment, (() => new global::Doroti.Framework.Widgets.WidgetStatesController())));
            controller__20915.update(global::Doroti.Framework.Widgets.WidgetState.selected, segmentSelected__20645);
            var content__21102 = label__20562;
            var effectiveSegmentStyle__21129 = segmentStyle__20173;
            if ((icon__20724 is not null))
            {
                bool useMaterial3__21482 = Theme.of(context).useMaterial3;
                double defaultFontSize__21550 = (segmentStyle__20173.textStyle?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.fontSize ?? 14.0);
                double effectiveTextScale__21675 = (MediaQuery.textScalerOf(context).scale(defaultFontSize__21550) / 14.0);
                global::Doroti.Framework.Painting.EdgeInsetsGeometry scaledPadding__21805 = ButtonStyleButton.scaledPadding((useMaterial3__21482 ? new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12, 8, 16, 8) : global::Doroti.Framework.Painting.EdgeInsets.CreateAll(8)), global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4), global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4), effectiveTextScale__21675);
                effectiveSegmentStyle__21129 = segmentStyle__20173.copyWith(padding: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(scaledPadding__21805));
                double scale__22299 = (Dart_uiLibrary.clampDouble(effectiveTextScale__21675, 1.0, 2.0) - 1.0);
                TextButtonThemeData textButtonTheme__22390 = TextButtonTheme.of(context);
                IconAlignment effectiveIconAlignment__22465 = ((textButtonTheme__22390.style?.iconAlignment ?? segmentStyle__20173.iconAlignment) ?? IconAlignment.start);
                content__21102 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, spacing: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(8L, 4L, scale__22299)), children: ((object.Equals(effectiveIconAlignment__22465, IconAlignment.start)) ? new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(icon__20724), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: label__20562)) } : new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: label__20562)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(icon__20724) })));
            }
            global::Doroti.Framework.Widgets.Widget button__22943 = ((global::Doroti.Framework.Widgets.Widget)(object?)new TextButton(style: effectiveSegmentStyle__21129, statesController: controller__20915, onHover: ((global::System.Action<bool>)((hovering) => {
setState(((global::System.Action)(() => {
_hovering = hovering;
})));
})), onFocusChange: ((global::System.Action<bool>)((focused) => {
setState(((global::System.Action)(() => {
_focused = focused;
})));
})), onPressed: ((global::System.Action)(((this._enabled && ((ButtonSegment<T>)segment).enabled)) ? (() => { _handleOnPressed(((ButtonSegment<T>)segment).value); }) : null)), child: content__21102));
            global::Doroti.Framework.Widgets.Widget buttonWithTooltip__23429 = ((((ButtonSegment<T>)segment).tooltip is not null) ? new Tooltip(message: ((ButtonSegment<T>)segment).tooltip, child: button__22943) : button__22943);
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.MergeSemantics(child: new global::Doroti.Framework.Widgets.Semantics(selected: segmentSelected__20645, inMutuallyExclusiveGroup: (((SegmentedButton<T>)(object)this.widget).multiSelectionEnabled ? null : true), child: buttonWithTooltip__23429)));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Framework.Painting.OutlinedBorder effectiveBorder__23815 = (resolve<global::Doroti.Framework.Painting.OutlinedBorder?>(((style) => style?.shape)) ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder());
        global::Doroti.Framework.Painting.OutlinedBorder resolvedDisabledBorder__23972 = (resolve<global::Doroti.Framework.Painting.OutlinedBorder?>(((style) => style?.shape), disabledState__18669) ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder());
        global::Doroti.Framework.Painting.BorderSide effectiveSide__24147 = (resolve<global::Doroti.Framework.Painting.BorderSide?>(((style) => style?.side)) ?? global::Doroti.Framework.Painting.BorderSide.none);
        global::Doroti.Framework.Painting.BorderSide disabledSide__24270 = (resolve<global::Doroti.Framework.Painting.BorderSide?>(((style) => style?.side), disabledState__18669) ?? global::Doroti.Framework.Painting.BorderSide.none);
        global::Doroti.Framework.Painting.OutlinedBorder enabledBorder__24412 = ((global::Doroti.Framework.Painting.OutlinedBorder)(object?)effectiveBorder__23815.copyWith(side: effectiveSide__24147));
        global::Doroti.Framework.Painting.OutlinedBorder disabledBorder__24500 = ((global::Doroti.Framework.Painting.OutlinedBorder)(object?)resolvedDisabledBorder__23972.copyWith(side: disabledSide__24270));
        VisualDensity resolvedVisualDensity__24594 = ((segmentStyle__20173.visualDensity ?? segmentThemeStyle__20241.visualDensity) ?? Theme.of(context).visualDensity);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry resolvedPadding__24769 = (resolve<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(((style) => style?.padding)) ?? global::Doroti.Framework.Painting.EdgeInsets.zero);
        MaterialTapTargetSize resolvedTapTargetSize__24916 = ((segmentStyle__20173.tapTargetSize ?? segmentThemeStyle__20241.tapTargetSize) ?? Theme.of(context).materialTapTargetSize);
        double fontSize__25087 = (resolve<global::Doroti.Framework.Painting.TextStyle?>(((style) => style?.textStyle))?.fontSize ?? 20.0);
        List<global::Doroti.Framework.Widgets.Widget> buttons__25211 = ((SegmentedButton<T>)(object)this.widget).segments.map<ButtonSegment<T>, global::Doroti.Framework.Widgets.Widget>(buttonFor).ToList().ToList();
        global::Doroti.Ui.Offset densityAdjustment__25280 = ((global::Doroti.Ui.Offset)(object?)resolvedVisualDensity__24594.baseSizeAdjustment);
        var textButtonMinHeight__25352 = 40.0;
        double adjustButtonMinHeight__25398 = (textButtonMinHeight__25352 + densityAdjustment__25280.dy);
        double effectiveVerticalPadding__25483 = (((global::Doroti.Framework.Painting.EdgeInsetsGeometry)resolvedPadding__24769).vertical + (densityAdjustment__25280.dy * 2L));
        double effectedButtonHeight__25580 = Math.Max((fontSize__25087 + effectiveVerticalPadding__25483), adjustButtonMinHeight__25398);
        double tapTargetVerticalPadding__25704 = (resolvedTapTargetSize__24916 switch { var __constant25770 when (object.Equals(__constant25770, MaterialTapTargetSize.shrinkWrap)) => 0.0, var __constant25817 when (object.Equals(__constant25817, MaterialTapTargetSize.padded)) => Math.Max(0, ((global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension + densityAdjustment__25280.dy) - effectedButtonHeight__25580)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Material(type: MaterialType.transparency, elevation: DartRuntimePrimitives.RequireValue(resolve<double?>(((style) => style?.elevation))), shadowColor: resolve<global::Doroti.Ui.Color?>(((style) => style?.shadowColor)), surfaceTintColor: resolve<global::Doroti.Ui.Color?>(((style) => style?.surfaceTintColor)), child: new TextButtonTheme(data: new TextButtonThemeData(style: segmentThemeStyle__20241), child: new global::Doroti.Framework.Widgets.Padding(padding: (((SegmentedButton<T>)(object)this.widget).expandedInsets ?? global::Doroti.Framework.Painting.EdgeInsets.zero), child: new _SegmentedButtonRenderWidget__segmented_button<T>(tapTargetVerticalPadding: tapTargetVerticalPadding__25704, segments: ((SegmentedButton<T>)(object)this.widget).segments, enabledBorder: (this._enabled ? enabledBorder__24412 : disabledBorder__24500), disabledBorder: disabledBorder__24500, direction: ((SegmentedButton<T>)(object)this.widget).direction, textDirection: textDirection__18615, isExpanded: (((SegmentedButton<T>)(object)this.widget).expandedInsets is not null), children: buttons__25211)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Widgets.WidgetStatesController controller__27011 in this.statesControllers.Values)
        {
            controller__27011.dispose();
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
{            var __cascade = __renderObject;
            __cascade.segments = this.segments;
            __cascade.enabledBorder = this.enabledBorder;
            __cascade.disabledBorder = this.disabledBorder;
            __cascade.direction = this.direction;
            __cascade.textDirection = this.textDirection;
            return __cascade;        }))());
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
        global::Doroti.Framework.Rendering.RenderBox? child__31183 = this.firstChild;
        var minWidth__31211 = 0.0;
        while ((child__31183 is not null))
        {
            var childParentData__31267 = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child__31183.parentData!)!;
            double childWidth__31365 = child__31183.getMinIntrinsicWidth(height);
            minWidth__31211 = Math.Max(minWidth__31211, childWidth__31365);
            child__31183 = childParentData__31267.nextSibling;
        }
        return (minWidth__31211 * this.childCount);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__31629 = this.firstChild;
        var maxWidth__31657 = 0.0;
        while ((child__31629 is not null))
        {
            var childParentData__31713 = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child__31629.parentData!)!;
            double childWidth__31811 = child__31629.getMaxIntrinsicWidth(height);
            maxWidth__31657 = Math.Max(maxWidth__31657, childWidth__31811);
            child__31629 = childParentData__31713.nextSibling;
        }
        return (maxWidth__31657 * this.childCount);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__32075 = this.firstChild;
        var minHeight__32103 = 0.0;
        while ((child__32075 is not null))
        {
            var childParentData__32160 = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child__32075.parentData!)!;
            double childHeight__32258 = child__32075.getMinIntrinsicHeight(width);
            minHeight__32103 = Math.Max(minHeight__32103, childHeight__32258);
            child__32075 = childParentData__32160.nextSibling;
        }
        return minHeight__32103;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__32514 = this.firstChild;
        var maxHeight__32542 = 0.0;
        while ((child__32514 is not null))
        {
            var childParentData__32599 = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child__32514.parentData!)!;
            double childHeight__32697 = child__32514.getMaxIntrinsicHeight(width);
            maxHeight__32542 = Math.Max(maxHeight__32542, childHeight__32697);
            child__32514 = childParentData__32599.nextSibling;
        }
        return maxHeight__32542;
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
        var child__33327 = leftChild;
        var start__33354 = 0.0;
        while ((child__33327 is not null))
        {
            var childParentData__33407 = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child__33327.parentData!)!;
            global::Doroti.Ui.RRect rChildRect__33509 = default!;
            if ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.vertical)))
            {
                childParentData__33407.offset = new global::Doroti.Ui.Offset(0.0, start__33354);
                var childRect__33628 = global::Doroti.Ui.Rect.fromLTWH(0.0, childParentData__33407.offset.dy, ((global::Doroti.Framework.Rendering.RenderBox)child__33327).size.width, ((global::Doroti.Framework.Rendering.RenderBox)child__33327).size.height);
                rChildRect__33509 = global::Doroti.Ui.RRect.fromRectAndCorners(childRect__33628);
                start__33354 += ((global::Doroti.Framework.Rendering.RenderBox)child__33327).size.height;
            }
            else
            {
                childParentData__33407.offset = new global::Doroti.Ui.Offset(start__33354, 0.0);
                var childRect__33951 = global::Doroti.Ui.Rect.fromLTWH(start__33354, 0.0, ((global::Doroti.Framework.Rendering.RenderBox)child__33327).size.width, ((global::Doroti.Framework.Rendering.RenderBox)child__33327).size.height);
                rChildRect__33509 = global::Doroti.Ui.RRect.fromRectAndCorners(childRect__33951);
                start__33354 += ((global::Doroti.Framework.Rendering.RenderBox)child__33327).size.width;
            }
            childParentData__33407.surroundingRect = rChildRect__33509;
            child__33327 = nextChild(child__33327);
        }
    }

    internal virtual global::Doroti.Ui.Size _calculateChildSize(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal)) ? _calculateHorizontalChildSize(constraints) : _calculateVerticalChildSize(constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _calculateHorizontalChildSize(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        double maxHeight__34508 = 0;
        global::Doroti.Framework.Rendering.RenderBox? child__34538 = this.firstChild;
        double childWidth__34569 = default!;
        if (this._isExpanded)
        {
            childWidth__34569 = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth / this.childCount);
        }
        else
        {
            childWidth__34569 = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minWidth / this.childCount);
            while ((child__34538 is not null))
            {
                childWidth__34569 = Math.Max(childWidth__34569, child__34538.getMaxIntrinsicWidth(double.PositiveInfinity));
                child__34538 = childAfter(child__34538);
            }
            childWidth__34569 = Math.Min(childWidth__34569, (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth / this.childCount));
        }
        child__34538 = this.firstChild;
        while ((child__34538 is not null))
        {
            double boxHeight__35039 = child__34538.getMaxIntrinsicHeight(childWidth__34569);
            maxHeight__34508 = Math.Max(maxHeight__34508, boxHeight__35039);
            child__34538 = childAfter(child__34538);
        }
        return new global::Doroti.Ui.Size(childWidth__34569, maxHeight__34508);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _calculateVerticalChildSize(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        double maxWidth__35302 = 0;
        global::Doroti.Framework.Rendering.RenderBox? child__35331 = this.firstChild;
        double childHeight__35362 = default!;
        if (this._isExpanded)
        {
            childHeight__35362 = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight / this.childCount);
        }
        else
        {
            childHeight__35362 = (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).minHeight / this.childCount);
            while ((child__35331 is not null))
            {
                childHeight__35362 = Math.Max(childHeight__35362, child__35331.getMaxIntrinsicHeight(double.PositiveInfinity));
                child__35331 = childAfter(child__35331);
            }
            childHeight__35362 = Math.Min(childHeight__35362, (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight / this.childCount));
        }
        child__35331 = this.firstChild;
        while ((child__35331 is not null))
        {
            double boxWidth__35843 = child__35331.getMaxIntrinsicWidth(maxWidth__35302);
            maxWidth__35302 = Math.Max(maxWidth__35302, boxWidth__35843);
            child__35331 = childAfter(child__35331);
        }
        var childSize__35987 = new global::Doroti.Ui.Size(maxWidth__35302, childHeight__35362);
        if ((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).hasTightWidth && (childSize__35987.width < ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth)))
        {
            childSize__35987 = new global::Doroti.Ui.Size(((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth, childSize__35987.height);
        }
        return childSize__35987;
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
        global::Doroti.Ui.Size childSize__36846 = ((global::Doroti.Ui.Size)(object?)_calculateChildSize(constraints));
        return _computeOverallSizeFromChildSize(childSize__36846);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Ui.Size childSize__37072 = ((global::Doroti.Ui.Size)(object?)_calculateChildSize(constraints));
        var childConstraints__37128 = global::Doroti.Framework.Rendering.BoxConstraints.CreateTight(childSize__37072);
        global::Doroti.Framework.Rendering.BaselineOffset baselineOffset__37200 = global::Doroti.Framework.Rendering.BaselineOffset.noBaseline;
        for (global::Doroti.Framework.Rendering.RenderBox? child__37264 = this.firstChild; (child__37264 is not null); child__37264 = childAfter(child__37264))
        {
            baselineOffset__37200 = baselineOffset__37200.minOf(new global::Doroti.Framework.Rendering.BaselineOffset(child__37264.getDryBaseline(childConstraints__37128, baseline)));
        }
        return baselineOffset__37200.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraints__37563 = this.constraints;
        global::Doroti.Ui.Size childSize__37610 = ((global::Doroti.Ui.Size)(object?)_calculateChildSize(constraints__37563));
        var childConstraints__37667 = global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: childSize__37610.width, height: childSize__37610.height);
        global::Doroti.Framework.Rendering.RenderBox? child__37796 = this.firstChild;
        while ((child__37796 is not null))
        {
            child__37796.layout(childConstraints__37667, parentUsesSize: true);
            child__37796 = childAfter(child__37796);
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
        size = _computeOverallSizeFromChildSize(childSize__37610);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Ui.Rect borderRect__38298 = ((global::Doroti.Ui.Rect)(object?)(((offset + new global::Doroti.Ui.Offset(0, (this.tapTargetVerticalPadding / 2L)))) & (new global::Doroti.Ui.Size(this.size.width, (this.size.height - this.tapTargetVerticalPadding)))));
        global::Doroti.Ui.Path borderClipPath__38455 = ((global::Doroti.Ui.Path)(object?)this.enabledBorder.getInnerPath(borderRect__38298, textDirection: this.textDirection));
        global::Doroti.Framework.Rendering.RenderBox? child__38576 = this.firstChild;
        global::Doroti.Framework.Rendering.RenderBox? previousChild__38611 = default!;
        var index__38634 = 0L;
        global::Doroti.Ui.Path? enabledClipPath__38655 = default!;
        global::Doroti.Ui.Path? disabledClipPath__38682 = default!;
        while ((child__38576 is not null))
        {
            var childParentData__38741 = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child__38576.parentData!)!;
            global::Doroti.Ui.Rect childRect__38837 = ((global::Doroti.Ui.Rect)(object?)((_SegmentedButtonContainerBoxParentData__segmented_button)childParentData__38741).surroundingRect!.outerRect.shift(offset));
            DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{            var __cascade = ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas;
            __cascade.save();
            __cascade.clipPath(borderClipPath__38455);
            return __cascade;        }))());
            context.paintChild(child__38576, (childParentData__38741.offset + offset));
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.restore();
            double segmentLeft__39164 = default!;
            double segmentRight__39196 = default!;
            double dividerPos__39229 = default!;
            double borderOutset__39260 = Math.Max(((global::Doroti.Framework.Painting.OutlinedBorder)this.enabledBorder).side.strokeOutset, ((global::Doroti.Framework.Painting.OutlinedBorder)this.disabledBorder).side.strokeOutset);
            switch (this.textDirection)
            {
                case TextDirection.rtl:
                    {
                        segmentLeft__39164 = ((object.Equals(child__38576, this.lastChild)) ? (borderRect__38298.left - borderOutset__39260) : childRect__38837.left);
                        segmentRight__39196 = ((object.Equals(child__38576, this.firstChild)) ? (borderRect__38298.right + borderOutset__39260) : childRect__38837.right);
                        dividerPos__39229 = segmentRight__39196;
                        break;
                    }
                case TextDirection.ltr:
                    {
                        segmentLeft__39164 = ((object.Equals(child__38576, this.firstChild)) ? (borderRect__38298.left - borderOutset__39260) : childRect__38837.left);
                        segmentRight__39196 = ((object.Equals(child__38576, this.lastChild)) ? (borderRect__38298.right + borderOutset__39260) : childRect__38837.right);
                        dividerPos__39229 = segmentLeft__39164;
                        break;
                    }
            }
            var segmentClipRect__39949 = global::Doroti.Ui.Rect.fromLTRB(segmentLeft__39164, (borderRect__38298.top - borderOutset__39260), segmentRight__39196, (borderRect__38298.bottom + borderOutset__39260));
            if (this.segments[(int)(index__38634)].enabled)
            {
                enabledClipPath__38655 = ((Func<Path>)(() =>
{            var __cascade = ((enabledClipPath__38655 ?? new global::Doroti.Ui.Path()));
            __cascade.addRect(segmentClipRect__39949);
            return __cascade;        }))();
            }
            else
            {
                disabledClipPath__38682 = ((Func<Path>)(() =>
{            var __cascade = ((disabledClipPath__38682 ?? new global::Doroti.Ui.Path()));
            __cascade.addRect(segmentClipRect__39949);
            return __cascade;        }))();
            }
            if ((previousChild__38611 is not null))
            {
                global::Doroti.Framework.Painting.BorderSide divider__40534 = ((this.segments[(int)((index__38634 - 1L))].enabled || this.segments[(int)(index__38634)].enabled) ? ((global::Doroti.Framework.Painting.OutlinedBorder)this.enabledBorder).side.copyWith(strokeAlign: 0.0) : ((global::Doroti.Framework.Painting.OutlinedBorder)this.disabledBorder).side.copyWith(strokeAlign: 0.0));
                if ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal)))
                {
                    var top__40781 = new global::Doroti.Ui.Offset(dividerPos__39229, borderRect__38298.top);
                    var bottom__40839 = new global::Doroti.Ui.Offset(dividerPos__39229, borderRect__38298.bottom);
                    ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawLine(top__40781, bottom__40839, divider__40534.toPaint());
                }
                else
                {
                    if ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.vertical)))
                    {
                        var start__41019 = new global::Doroti.Ui.Offset(borderRect__38298.left, childRect__38837.top);
                        var end__41083 = new global::Doroti.Ui.Offset(borderRect__38298.right, childRect__38837.top);
                        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{            var __cascade = ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas;
            __cascade.save();
            __cascade.clipPath(borderClipPath__38455);
            return __cascade;        }))());
                        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawLine(start__41019, end__41083, divider__40534.toPaint());
                        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.restore();
                    }
                }
            }
            previousChild__38611 = child__38576;
            child__38576 = childAfter(child__38576);
            index__38634 += 1L;
        }
        if ((disabledClipPath__38682 is null))
        {
            this.enabledBorder.paint(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, borderRect__38298, textDirection: this.textDirection);
        }
        else
        {
            if ((enabledClipPath__38655 is null))
            {
                this.disabledBorder.paint(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, borderRect__38298, textDirection: this.textDirection);
            }
            else
            {
                DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{            var __cascade = ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas;
            __cascade.save();
            __cascade.clipPath(enabledClipPath__38655);
            return __cascade;        }))());
                this.enabledBorder.paint(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, borderRect__38298, textDirection: this.textDirection);
                DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{            var __cascade = ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas;
            __cascade.restore();
            __cascade.save();
            __cascade.clipPath(disabledClipPath__38682);
            return __cascade;        }))());
                this.disabledBorder.paint(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, borderRect__38298, textDirection: this.textDirection);
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.restore();
            }
        }
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__42436 = this.lastChild;
        while ((child__42436 is not null))
        {
            var childParentData__42495 = ((_SegmentedButtonContainerBoxParentData__segmented_button?)(object?)child__42436.parentData!)!;
            if (((_SegmentedButtonContainerBoxParentData__segmented_button)childParentData__42495).surroundingRect!.contains(position))
            {
                return result.addWithPaintOffset(offset: childParentData__42495.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, localOffset) => {
DartRuntimePrimitives.Assert(() => (object.Equals(localOffset, (position - childParentData__42495.offset))));
return child__42436!.hitTest(result, position: localOffset);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            }
            child__42436 = childParentData__42495.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
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
        var childParentData__175971 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((ContainerBoxParentData<RenderBox>?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData__176343.previousSibling = child;
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
            var afterParentData__176766 = ((ContainerBoxParentData<RenderBox>?)(object?)after.parentData!)!;
            if ((afterParentData__176766.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData__175971.previousSibling = after;
                afterParentData__176766.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData__175971.nextSibling = afterParentData__176766.nextSibling;
                childParentData__175971.previousSibling = after;
                var childPreviousSiblingParentData__177424 = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
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
        var childParentData__179226 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData__179226.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData__179226.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData__179613 = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((ContainerBoxParentData<RenderBox>?)(object?)childParentData__179226.nextSibling!.parentData!)!;
            childNextSiblingParentData__179965.previousSibling = childParentData__179226.previousSibling;
        }
        childParentData__179226.previousSibling = null;
        childParentData__179226.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child__180623 = this._firstChild;
        while ((child__180623 is not null))
        {
            var childParentData__180684 = ((ContainerBoxParentData<RenderBox>?)(object?)child__180623.parentData!)!;
            RenderBox? next__180762 = childParentData__180684.nextSibling;
            childParentData__180684.previousSibling = null;
            childParentData__180684.nextSibling = null;
            dropChild(child__180623);
            child__180623 = next__180762;
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
        var childParentData__181479 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData__181479.previousSibling, after)))
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
        RenderBox? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            ((dynamic)child__181803).attach(owner);
            var childParentData__181891 = ((ContainerBoxParentData<RenderBox>?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            ((dynamic)child__182065).detach();
            var childParentData__182148 = ((ContainerBoxParentData<RenderBox>?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((ContainerBoxParentData<RenderBox>?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((ContainerBoxParentData<RenderBox>?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((ContainerBoxParentData<RenderBox>?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children__183528 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child__183606 = this.firstChild!;
            var count__183637 = 1L;
            while (true)
            {
                children__183528.Add(((Diagnosticable)child__183606).toDiagnosticsNode(name: $"child__183606 {count__183637}"));
                if ((object.Equals(child__183606, this.lastChild)))
                {
                    break;
                }
                count__183637 += 1L;
                var childParentData__183833 = ((ContainerBoxParentData<RenderBox>?)(object?)child__183606.parentData!)!;
                child__183606 = childParentData__183833.nextSibling!;
            }
        }
        return children__183528;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        RenderBox? child__138717 = this.firstChild;
        while ((child__138717 is not null))
        {
            var childParentData__138777 = ((ContainerBoxParentData<RenderBox>?)(object?)child__138717.parentData!)!;
            double? result__138852 = child__138717.getDistanceToActualBaseline(baseline);
            if ((result__138852 is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result__138852);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData__138777.offset.dy);
            }
            child__138717 = childParentData__138777.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        BaselineOffset minBaseline__139372 = BaselineOffset.noBaseline;
        RenderBox? child__139428 = this.firstChild;
        while ((child__139428 is not null))
        {
            var childParentData__139488 = ((ContainerBoxParentData<RenderBox>?)(object?)child__139428.parentData!)!;
            BaselineOffset candidate__139570 = (new BaselineOffset(child__139428.getDistanceToActualBaseline(baseline)).op_Add(childParentData__139488.offset.dy));
            minBaseline__139372 = minBaseline__139372.minOf(candidate__139570);
            child__139428 = childParentData__139488.nextSibling;
        }
        return minBaseline__139372.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child__140279 = this.lastChild;
        while ((child__140279 is not null))
        {
            var childParentData__140418 = ((ContainerBoxParentData<RenderBox>?)(object?)child__140279.parentData!)!;
            bool isHit__140490 = result.addWithPaintOffset(offset: childParentData__140418.offset, position: position, hitTest: ((global::System.Func<BoxHitTestResult, Offset, bool>)((result, transformed) => {
DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData__140418.offset))));
return child__140279!.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            if (isHit__140490)
            {
                return true;
            }
            child__140279 = childParentData__140418.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child__141240 = this.firstChild;
        while ((child__141240 is not null))
        {
            var childParentData__141300 = ((ContainerBoxParentData<RenderBox>?)(object?)child__141240.parentData!)!;
            context.paintChild(child__141240, (childParentData__141300.offset + offset));
            child__141240 = childParentData__141300.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result__141793 = new List<RenderBox>();
        RenderBox? child__141832 = this.firstChild;
        while ((child__141832 is not null))
        {
            var childParentData__141892 = ((ContainerBoxParentData<RenderBox>?)(object?)child__141832.parentData!)!;
            result__141793.Add(((RenderBox?)(object?)child__141832)!);
            child__141832 = childParentData__141892.nextSibling;
        }
        return result__141793;
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
            return new ButtonStyle(textStyle: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(Theme.of(this.context).textTheme.labelLarge), backgroundColor: WidgetStateProperty.resolveWith((states) => {
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
}), foregroundColor: WidgetStateProperty.resolveWith((states) => {
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
}), overlayColor: WidgetStateProperty.resolveWith((states) => {
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
}), surfaceTintColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent), elevation: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(0), iconSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(18.0), side: WidgetStateProperty.resolveWith((states) => {
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
        global::Doroti.Ui.Color? selected__46920 = ((global::Doroti.Ui.Color?)(object?)(overlayColor ?? selectedColor));
        global::Doroti.Ui.Color? unselected__46979 = ((global::Doroti.Ui.Color?)(object?)(overlayColor ?? unselectedColor));
        return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>)(object?)global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Framework.Widgets.WidgetState.selected.asConstraint().op_BitwiseAnd(global::Doroti.Framework.Widgets.WidgetState.pressed.asConstraint())] = selected__46920?.withOpacity(0.1), [global::Doroti.Framework.Widgets.WidgetState.selected.asConstraint().op_BitwiseAnd(global::Doroti.Framework.Widgets.WidgetState.hovered.asConstraint())] = selected__46920?.withOpacity(0.08), [global::Doroti.Framework.Widgets.WidgetState.selected.asConstraint().op_BitwiseAnd(global::Doroti.Framework.Widgets.WidgetState.focused.asConstraint())] = selected__46920?.withOpacity(0.1), [global::Doroti.Framework.Widgets.WidgetState.pressed.asConstraint()] = unselected__46979?.withOpacity(0.1), [global::Doroti.Framework.Widgets.WidgetState.hovered.asConstraint()] = unselected__46979?.withOpacity(0.08), [global::Doroti.Framework.Widgets.WidgetState.focused.asConstraint()] = unselected__46979?.withOpacity(0.1), [global::Doroti.Framework.Widgets.WidgetStateMembers.any] = Colors.transparent }.cast<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Ui.Color?>()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
