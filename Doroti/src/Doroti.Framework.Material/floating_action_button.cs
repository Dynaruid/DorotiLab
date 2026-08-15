// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/floating_action_button.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

internal class _DefaultHeroTag__floating_action_button
{
    internal _DefaultHeroTag__floating_action_button()
    {
    }

    public override string ToString() => "<default FloatingActionButton tag>";
}

internal enum _FloatingActionButtonType__floating_action_button
{
    regular,
    small,
    large,
    extended
}

public class FloatingActionButton : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual object? heroTag { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? focusElevation { get; private set; }
    public virtual double? hoverElevation { get; private set; }
    public virtual double? highlightElevation { get; private set; }
    public virtual double? disabledElevation { get; private set; }
    public virtual bool mini { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool isExtended { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual double? extendedIconLabelSpacing { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? extendedPadding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? extendedTextStyle { get; private set; }
    internal virtual _FloatingActionButtonType__floating_action_button _floatingActionButtonType { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.Widget? _extendedLabel { get; private set; }

    public FloatingActionButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, string? tooltip = null, Color? foregroundColor = null, Color? backgroundColor = null, Color? focusColor = null, Color? hoverColor = null, Color? splashColor = null, object? heroTag = default!, double? elevation = null, double? focusElevation = null, double? hoverElevation = null, double? highlightElevation = null, double? disabledElevation = null, global::System.Action? onPressed = default!, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, bool mini = false, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, MaterialTapTargetSize? materialTapTargetSize = null, bool isExtended = false, bool? enableFeedback = null) : base(key: key)
    {
        object? __heroTag = heroTag ?? new _DefaultHeroTag__floating_action_button();
        this.child = child;
        this.tooltip = tooltip;
        this.foregroundColor = foregroundColor;
        this.backgroundColor = backgroundColor;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.splashColor = splashColor;
        this.heroTag = __heroTag;
        this.elevation = elevation;
        this.focusElevation = focusElevation;
        this.hoverElevation = hoverElevation;
        this.highlightElevation = highlightElevation;
        this.disabledElevation = disabledElevation;
        this.onPressed = onPressed;
        this.mouseCursor = mouseCursor;
        this.mini = mini;
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.materialTapTargetSize = materialTapTargetSize;
        this.isExtended = isExtended;
        this.enableFeedback = enableFeedback;
        this._floatingActionButtonType = (mini ? _FloatingActionButtonType__floating_action_button.small : _FloatingActionButtonType__floating_action_button.regular);
        this._extendedLabel = null;
        this.extendedIconLabelSpacing = null;
        this.extendedPadding = null;
        this.extendedTextStyle = null;
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((focusElevation is null) || (focusElevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((hoverElevation is null) || (hoverElevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((highlightElevation is null) || (highlightElevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((disabledElevation is null) || (disabledElevation >= 0.0)));
    }

    public static FloatingActionButton CreateSmall(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, string? tooltip = null, Color? foregroundColor = null, Color? backgroundColor = null, Color? focusColor = null, Color? hoverColor = null, Color? splashColor = null, object? heroTag = default!, double? elevation = null, double? focusElevation = null, double? hoverElevation = null, double? highlightElevation = null, double? disabledElevation = null, global::System.Action? onPressed = default!, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, MaterialTapTargetSize? materialTapTargetSize = null, bool? enableFeedback = null)
    {
        var __instance = new FloatingActionButton(key: key, child: child, tooltip: tooltip, foregroundColor: foregroundColor, backgroundColor: backgroundColor, focusColor: focusColor, hoverColor: hoverColor, splashColor: splashColor, heroTag: heroTag, elevation: elevation, focusElevation: focusElevation, hoverElevation: hoverElevation, highlightElevation: highlightElevation, disabledElevation: disabledElevation, onPressed: onPressed, mouseCursor: mouseCursor, shape: shape, clipBehavior: clipBehavior, focusNode: focusNode, autofocus: autofocus, materialTapTargetSize: materialTapTargetSize, enableFeedback: enableFeedback);
        object? __heroTag = heroTag ?? new _DefaultHeroTag__floating_action_button();
        __instance.child = child;
        __instance.tooltip = tooltip;
        __instance.foregroundColor = foregroundColor;
        __instance.backgroundColor = backgroundColor;
        __instance.focusColor = focusColor;
        __instance.hoverColor = hoverColor;
        __instance.splashColor = splashColor;
        __instance.heroTag = __heroTag;
        __instance.elevation = elevation;
        __instance.focusElevation = focusElevation;
        __instance.hoverElevation = hoverElevation;
        __instance.highlightElevation = highlightElevation;
        __instance.disabledElevation = disabledElevation;
        __instance.onPressed = onPressed;
        __instance.mouseCursor = mouseCursor;
        __instance.shape = shape;
        __instance.clipBehavior = clipBehavior;
        __instance.focusNode = focusNode;
        __instance.autofocus = autofocus;
        __instance.materialTapTargetSize = materialTapTargetSize;
        __instance.enableFeedback = enableFeedback;
        __instance._floatingActionButtonType = _FloatingActionButtonType__floating_action_button.small;
        __instance.mini = true;
        __instance.isExtended = false;
        __instance._extendedLabel = null;
        __instance.extendedIconLabelSpacing = null;
        __instance.extendedPadding = null;
        __instance.extendedTextStyle = null;
        return __instance;
    }

    public static FloatingActionButton CreateLarge(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, string? tooltip = null, Color? foregroundColor = null, Color? backgroundColor = null, Color? focusColor = null, Color? hoverColor = null, Color? splashColor = null, object? heroTag = default!, double? elevation = null, double? focusElevation = null, double? hoverElevation = null, double? highlightElevation = null, double? disabledElevation = null, global::System.Action? onPressed = default!, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, MaterialTapTargetSize? materialTapTargetSize = null, bool? enableFeedback = null)
    {
        var __instance = new FloatingActionButton(key: key, child: child, tooltip: tooltip, foregroundColor: foregroundColor, backgroundColor: backgroundColor, focusColor: focusColor, hoverColor: hoverColor, splashColor: splashColor, heroTag: heroTag, elevation: elevation, focusElevation: focusElevation, hoverElevation: hoverElevation, highlightElevation: highlightElevation, disabledElevation: disabledElevation, onPressed: onPressed, mouseCursor: mouseCursor, shape: shape, clipBehavior: clipBehavior, focusNode: focusNode, autofocus: autofocus, materialTapTargetSize: materialTapTargetSize, enableFeedback: enableFeedback);
        object? __heroTag = heroTag ?? new _DefaultHeroTag__floating_action_button();
        __instance.child = child;
        __instance.tooltip = tooltip;
        __instance.foregroundColor = foregroundColor;
        __instance.backgroundColor = backgroundColor;
        __instance.focusColor = focusColor;
        __instance.hoverColor = hoverColor;
        __instance.splashColor = splashColor;
        __instance.heroTag = __heroTag;
        __instance.elevation = elevation;
        __instance.focusElevation = focusElevation;
        __instance.hoverElevation = hoverElevation;
        __instance.highlightElevation = highlightElevation;
        __instance.disabledElevation = disabledElevation;
        __instance.onPressed = onPressed;
        __instance.mouseCursor = mouseCursor;
        __instance.shape = shape;
        __instance.clipBehavior = clipBehavior;
        __instance.focusNode = focusNode;
        __instance.autofocus = autofocus;
        __instance.materialTapTargetSize = materialTapTargetSize;
        __instance.enableFeedback = enableFeedback;
        __instance._floatingActionButtonType = _FloatingActionButtonType__floating_action_button.large;
        __instance.mini = false;
        __instance.isExtended = false;
        __instance._extendedLabel = null;
        __instance.extendedIconLabelSpacing = null;
        __instance.extendedPadding = null;
        __instance.extendedTextStyle = null;
        return __instance;
    }

    public static FloatingActionButton CreateExtended(global::Doroti.Generated.Framework.Foundation.Key? key = null, string? tooltip = null, Color? foregroundColor = null, Color? backgroundColor = null, Color? focusColor = null, Color? hoverColor = null, object? heroTag = default!, double? elevation = null, double? focusElevation = null, double? hoverElevation = null, Color? splashColor = null, double? highlightElevation = null, double? disabledElevation = null, global::System.Action? onPressed = default!, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, bool isExtended = true, MaterialTapTargetSize? materialTapTargetSize = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, double? extendedIconLabelSpacing = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? extendedPadding = null, global::Doroti.Generated.Framework.Painting.TextStyle? extendedTextStyle = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, global::Doroti.Generated.Framework.Widgets.Widget label = default!, bool? enableFeedback = null)
    {
        var __instance = new FloatingActionButton(key: key, tooltip: tooltip, foregroundColor: foregroundColor, backgroundColor: backgroundColor, focusColor: focusColor, hoverColor: hoverColor, splashColor: splashColor, heroTag: heroTag, elevation: elevation, focusElevation: focusElevation, hoverElevation: hoverElevation, highlightElevation: highlightElevation, disabledElevation: disabledElevation, onPressed: onPressed, mouseCursor: mouseCursor, shape: shape, clipBehavior: clipBehavior, focusNode: focusNode, autofocus: autofocus, materialTapTargetSize: materialTapTargetSize, isExtended: isExtended, enableFeedback: enableFeedback);
        object? __heroTag = heroTag ?? new _DefaultHeroTag__floating_action_button();
        __instance.tooltip = tooltip;
        __instance.foregroundColor = foregroundColor;
        __instance.backgroundColor = backgroundColor;
        __instance.focusColor = focusColor;
        __instance.hoverColor = hoverColor;
        __instance.heroTag = __heroTag;
        __instance.elevation = elevation;
        __instance.focusElevation = focusElevation;
        __instance.hoverElevation = hoverElevation;
        __instance.splashColor = splashColor;
        __instance.highlightElevation = highlightElevation;
        __instance.disabledElevation = disabledElevation;
        __instance.onPressed = onPressed;
        __instance.mouseCursor = mouseCursor;
        __instance.shape = shape;
        __instance.isExtended = isExtended;
        __instance.materialTapTargetSize = materialTapTargetSize;
        __instance.clipBehavior = clipBehavior;
        __instance.focusNode = focusNode;
        __instance.autofocus = autofocus;
        __instance.extendedIconLabelSpacing = extendedIconLabelSpacing;
        __instance.extendedPadding = extendedPadding;
        __instance.extendedTextStyle = extendedTextStyle;
        __instance.enableFeedback = enableFeedback;
        __instance.mini = false;
        __instance._floatingActionButtonType = _FloatingActionButtonType__floating_action_button.extended;
        __instance.child = icon;
        __instance._extendedLabel = label;
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__18542 = Theme.of(context);
        FloatingActionButtonThemeData floatingActionButtonTheme__18609 = FloatingActionButtonTheme.of(context);
        FloatingActionButtonThemeData defaults__18729 = (theme__18542.useMaterial3 ? new _FABDefaultsM3__floating_action_button(context, this._floatingActionButtonType, (this.child is not null)) : new _FABDefaultsM2__floating_action_button(context, this._floatingActionButtonType, (this.child is not null)));
        global::Doroti.Ui.Color foregroundColor__18929 = ((global::Doroti.Ui.Color)(object?)((this.foregroundColor ?? floatingActionButtonTheme__18609.foregroundColor) ?? defaults__18729.foregroundColor!));
        global::Doroti.Ui.Color backgroundColor__19083 = ((global::Doroti.Ui.Color)(object?)((this.backgroundColor ?? floatingActionButtonTheme__18609.backgroundColor) ?? defaults__18729.backgroundColor!));
        global::Doroti.Ui.Color focusColor__19237 = ((global::Doroti.Ui.Color)(object?)((this.focusColor ?? floatingActionButtonTheme__18609.focusColor) ?? defaults__18729.focusColor!));
        global::Doroti.Ui.Color hoverColor__19355 = ((global::Doroti.Ui.Color)(object?)((this.hoverColor ?? floatingActionButtonTheme__18609.hoverColor) ?? defaults__18729.hoverColor!));
        global::Doroti.Ui.Color splashColor__19473 = ((global::Doroti.Ui.Color)(object?)((this.splashColor ?? floatingActionButtonTheme__18609.splashColor) ?? defaults__18729.splashColor!));
        double elevation__19596 = ((this.elevation ?? floatingActionButtonTheme__18609.elevation) ?? DartRuntimePrimitives.RequireValue(defaults__18729.elevation));
        double focusElevation__19711 = ((this.focusElevation ?? floatingActionButtonTheme__18609.focusElevation) ?? DartRuntimePrimitives.RequireValue(defaults__18729.focusElevation));
        double hoverElevation__19846 = ((this.hoverElevation ?? floatingActionButtonTheme__18609.hoverElevation) ?? DartRuntimePrimitives.RequireValue(defaults__18729.hoverElevation));
        double disabledElevation__19981 = (((this.disabledElevation ?? floatingActionButtonTheme__18609.disabledElevation) ?? defaults__18729.disabledElevation) ?? DartRuntimePrimitives.RequireValue(elevation__19596));
        double highlightElevation__20164 = ((this.highlightElevation ?? floatingActionButtonTheme__18609.highlightElevation) ?? DartRuntimePrimitives.RequireValue(defaults__18729.highlightElevation));
        MaterialTapTargetSize materialTapTargetSize__20346 = (this.materialTapTargetSize ?? theme__18542.materialTapTargetSize);
        bool enableFeedback__20452 = ((this.enableFeedback ?? floatingActionButtonTheme__18609.enableFeedback) ?? DartRuntimePrimitives.RequireValue(defaults__18729.enableFeedback));
        double iconSize__20587 = (floatingActionButtonTheme__18609.iconSize ?? DartRuntimePrimitives.RequireValue(defaults__18729.iconSize));
        global::Doroti.Generated.Framework.Painting.TextStyle extendedTextStyle__20676 = ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)(((this.extendedTextStyle ?? floatingActionButtonTheme__18609.extendedTextStyle) ?? defaults__18729.extendedTextStyle!)).copyWith(color: foregroundColor__18929));
        global::Doroti.Generated.Framework.Painting.ShapeBorder shape__20908 = ((this.shape ?? floatingActionButtonTheme__18609.shape) ?? defaults__18729.shape!);
        global::Doroti.Generated.Framework.Rendering.BoxConstraints sizeConstraints__21002 = default!;
        global::Doroti.Generated.Framework.Widgets.Widget? resolvedChild__21031 = ((this.child is not null) ? IconTheme.merge(data: new global::Doroti.Generated.Framework.Widgets.IconThemeData(size: iconSize__20587), child: this.child!) : this.child);
        switch (this._floatingActionButtonType)
        {
            case _FloatingActionButtonType__floating_action_button.regular:
                {
                    sizeConstraints__21002 = (floatingActionButtonTheme__18609.sizeConstraints ?? defaults__18729.sizeConstraints!);
                    break;
                }
            case _FloatingActionButtonType__floating_action_button.small:
                {
                    sizeConstraints__21002 = (floatingActionButtonTheme__18609.smallSizeConstraints ?? defaults__18729.smallSizeConstraints!);
                    break;
                }
            case _FloatingActionButtonType__floating_action_button.large:
                {
                    sizeConstraints__21002 = (floatingActionButtonTheme__18609.largeSizeConstraints ?? defaults__18729.largeSizeConstraints!);
                    break;
                }
            case _FloatingActionButtonType__floating_action_button.extended:
                {
                    sizeConstraints__21002 = (floatingActionButtonTheme__18609.extendedSizeConstraints ?? defaults__18729.extendedSizeConstraints!);
                    double iconLabelSpacing__21900 = ((this.extendedIconLabelSpacing ?? floatingActionButtonTheme__18609.extendedIconLabelSpacing) ?? 8.0);
                    global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding__22051 = ((this.extendedPadding ?? floatingActionButtonTheme__18609.extendedPadding) ?? defaults__18729.extendedPadding!);
                    resolvedChild__21031 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new _ChildOverflowBox__floating_action_button(child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: padding__22051, child: new global::Doroti.Generated.Framework.Widgets.Row(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection22381 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); var __collectionElement22407 = this.child; if (__collectionElement22407 is { } __nonNullCollectionElement22407) { __collection22381.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement22407)); } if (((this.child is not null) && this.isExtended)) { __collection22381.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: iconLabelSpacing__21900))); } if (this.isExtended) { __collection22381.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(this._extendedLabel!)); } return __collection22381; }))()))));
                    break;
                }
        }
        global::Doroti.Generated.Framework.Widgets.Widget result__22622 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new RawMaterialButton(onPressed: () => this.onPressed(), mouseCursor: new _EffectiveMouseCursor__floating_action_button(this.mouseCursor, floatingActionButtonTheme__18609.mouseCursor), elevation: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(elevation__19596)), focusElevation: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(focusElevation__19711)), hoverElevation: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(hoverElevation__19846)), highlightElevation: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(highlightElevation__20164)), disabledElevation: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(disabledElevation__19981)), constraints: sizeConstraints__21002, materialTapTargetSize: materialTapTargetSize__20346, fillColor: backgroundColor__19083, focusColor: focusColor__19237, hoverColor: hoverColor__19355, splashColor: splashColor__19473, textStyle: extendedTextStyle__20676, shape: shape__20908, clipBehavior: this.clipBehavior, focusNode: this.focusNode, autofocus: this.autofocus, enableFeedback: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(enableFeedback__20452)), child: resolvedChild__21031));
        if ((this.tooltip is not null))
        {
            result__22622 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new Tooltip(message: this.tooltip, child: result__22622));
        }
        if ((this.heroTag is not null))
        {
            result__22622 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Hero(tag: this.heroTag!, child: result__22622));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.MergeSemantics(child: result__22622));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::System.Action>("onPressed", () => this.onPressed(), ifNull: "disabled"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.StringProperty("tooltip", this.tooltip, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("foregroundColor", this.foregroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("focusColor", this.focusColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("hoverColor", this.hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("splashColor", this.splashColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<object>("heroTag", this.heroTag, ifPresent: "hero"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("focusElevation", this.focusElevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("hoverElevation", this.hoverElevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("highlightElevation", this.highlightElevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("disabledElevation", this.disabledElevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.FocusNode>("focusNode", this.focusNode, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("isExtended", value: this.isExtended, ifTrue: "extended"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<MaterialTapTargetSize>("materialTapTargetSize", this.materialTapTargetSize, defaultValue: null));
    }

}

internal class _EffectiveMouseCursor__floating_action_button : global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor
{
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? widgetCursor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? themeCursor { get; private set; }

    internal _EffectiveMouseCursor__floating_action_button(global::Doroti.Generated.Framework.Services.MouseCursor? widgetCursor, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? themeCursor)
    {
        this.widgetCursor = widgetCursor;
        this.themeCursor = themeCursor;
    }

    public override global::Doroti.Generated.Framework.Services.MouseCursor resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)
    {
        return ((((WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor?>(this.widgetCursor, states) ?? (global::Doroti.Generated.Framework.Services.MouseCursor)this.themeCursor?.resolve(states))) ?? (global::Doroti.Generated.Framework.Services.MouseCursor)global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable.resolve(states)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescription => "WidgetStateMouseCursor(FloatActionButton)";
}

internal class _ChildOverflowBox__floating_action_button : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    internal _ChildOverflowBox__floating_action_button(global::Doroti.Generated.Framework.Widgets.Widget? child = null) : base(child: child)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderChildOverflowBox__floating_action_button(textDirection: Directionality.of(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderChildOverflowBox__floating_action_button)(object)renderObject;
        ((dynamic)__renderObject).textDirection = Directionality.of(context);
    }

}

public class _RenderChildOverflowBox__floating_action_button : global::Doroti.Generated.Framework.Rendering.RenderAligningShiftedBox
{
    internal _RenderChildOverflowBox__floating_action_button(TextDirection? textDirection = null) : base(textDirection: textDirection, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center)
    {
    }

    public override double computeMinIntrinsicWidth(double height) => 0.0;
    public override double computeMinIntrinsicHeight(double width) => 0.0;
    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        if ((this.child is not null))
        {
            global::Doroti.Ui.Size childSize__27212 = ((global::Doroti.Ui.Size)(object?)this.child!.getDryLayout(new global::Doroti.Generated.Framework.Rendering.BoxConstraints()));
            return new global::Doroti.Ui.Size(Math.Max(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).minWidth, Math.Min(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth, childSize__27212.width)), Math.Max(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).minHeight, Math.Min(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight, childSize__27212.height)));
        }
        else
        {
            return ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).biggest;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints__27598 = this.constraints;
        if ((this.child is not null))
        {
            this.child!.layout(new global::Doroti.Generated.Framework.Rendering.BoxConstraints(), parentUsesSize: true);
            size = new global::Doroti.Ui.Size(Math.Max(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__27598).minWidth, Math.Min(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__27598).maxWidth, this.child!.size.width)), Math.Max(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__27598).minHeight, Math.Min(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__27598).maxHeight, this.child!.size.height)));
            alignChild();
        }
        else
        {
            size = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__27598).biggest;
        }
    }

}

internal class _FABDefaultsM2__floating_action_button : FloatingActionButtonThemeData
{
    public virtual _FloatingActionButtonType__floating_action_button type { get; private set; } = default!;
    public virtual bool hasChild { get; private set; } = default!;
    internal virtual ThemeData _theme { get; private set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;

    internal _FABDefaultsM2__floating_action_button(global::Doroti.Generated.Framework.Widgets.BuildContext context, _FloatingActionButtonType__floating_action_button type, bool hasChild) : base(elevation: 6, focusElevation: 6, hoverElevation: 8, highlightElevation: 12, enableFeedback: true, sizeConstraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: 56.0, height: 56.0), smallSizeConstraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: 40.0, height: 40.0), largeSizeConstraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: 96.0, height: 96.0), extendedSizeConstraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(height: 48.0), extendedIconLabelSpacing: 8.0)
    {
        this.type = type;
        this.hasChild = hasChild;
        this._theme = Theme.of(context);
        this._colors = Theme.of(context).colorScheme;
    }

    internal virtual bool _isExtended => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(this.type, _FloatingActionButtonType__floating_action_button.extended)));
    internal virtual bool _isLarge => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(this.type, _FloatingActionButtonType__floating_action_button.large)));
    public virtual global::Doroti.Ui.Color? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSecondary);
    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondary);
    public virtual global::Doroti.Ui.Color? focusColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._theme.focusColor);
    public virtual global::Doroti.Ui.Color? hoverColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._theme.hoverColor);
    public virtual global::Doroti.Ui.Color? splashColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._theme.splashColor);
    public override global::Doroti.Generated.Framework.Painting.ShapeBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.ShapeBorder>((this._isExtended ? new global::Doroti.Generated.Framework.Painting.StadiumBorder() : new global::Doroti.Generated.Framework.Painting.CircleBorder()));
    public override double? iconSize => (this._isLarge ? 36.0 : 24.0);
    public override global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? extendedPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((this.hasChild && this._isExtended) ? 16.0 : 20.0), end: 20.0));
    public override global::Doroti.Generated.Framework.Painting.TextStyle? extendedTextStyle => this._theme.textTheme.labelLarge!.copyWith(letterSpacing: 1.2);
}

internal class _FABDefaultsM3__floating_action_button : FloatingActionButtonThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    public virtual _FloatingActionButtonType__floating_action_button type { get; private set; } = default!;
    public virtual bool hasChild { get; private set; } = default!;
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
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = Theme.of(this.context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _FABDefaultsM3__floating_action_button(global::Doroti.Generated.Framework.Widgets.BuildContext context, _FloatingActionButtonType__floating_action_button type, bool hasChild) : base(elevation: 6.0, focusElevation: 6.0, hoverElevation: 8.0, highlightElevation: 6.0, enableFeedback: true, sizeConstraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: 56.0, height: 56.0), smallSizeConstraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: 40.0, height: 40.0), largeSizeConstraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: 96.0, height: 96.0), extendedSizeConstraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(height: 56.0), extendedIconLabelSpacing: 8.0)
    {
        this.context = context;
        this.type = type;
        this.hasChild = hasChild;
    }

    internal virtual bool _isExtended => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(this.type, _FloatingActionButtonType__floating_action_button.extended)));
    public virtual global::Doroti.Ui.Color? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onPrimaryContainer);
    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primaryContainer);
    public virtual global::Doroti.Ui.Color? splashColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onPrimaryContainer.withOpacity(0.1));
    public virtual global::Doroti.Ui.Color? focusColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onPrimaryContainer.withOpacity(0.1));
    public virtual global::Doroti.Ui.Color? hoverColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onPrimaryContainer.withOpacity(0.08));
    public override global::Doroti.Generated.Framework.Painting.ShapeBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.ShapeBorder>((this.type switch { _FloatingActionButtonType__floating_action_button.regular => new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(16.0))), _FloatingActionButtonType__floating_action_button.small => new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(12.0))), _FloatingActionButtonType__floating_action_button.large => new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(28.0))), _FloatingActionButtonType__floating_action_button.extended => new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(16.0))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
    public override double? iconSize => (this.type switch { _FloatingActionButtonType__floating_action_button.regular => 24.0, _FloatingActionButtonType__floating_action_button.small => 24.0, _FloatingActionButtonType__floating_action_button.large => 36.0, _FloatingActionButtonType__floating_action_button.extended => 24.0, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? extendedPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((this.hasChild && this._isExtended) ? 16.0 : 20.0), end: 20.0));
    public override global::Doroti.Generated.Framework.Painting.TextStyle? extendedTextStyle => this._textTheme.labelLarge;
}
