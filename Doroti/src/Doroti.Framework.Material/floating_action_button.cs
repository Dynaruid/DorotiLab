// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/floating_action_button.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

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

public class FloatingActionButton : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual object? heroTag { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? focusElevation { get; private set; }
    public virtual double? hoverElevation { get; private set; }
    public virtual double? highlightElevation { get; private set; }
    public virtual double? disabledElevation { get; private set; }
    public virtual bool mini { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool isExtended { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual double? extendedIconLabelSpacing { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? extendedPadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? extendedTextStyle { get; private set; }
    internal virtual _FloatingActionButtonType__floating_action_button _floatingActionButtonType { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.Widget? _extendedLabel { get; private set; }

    public FloatingActionButton(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? child = null, string? tooltip = null, Color? foregroundColor = null, Color? backgroundColor = null, Color? focusColor = null, Color? hoverColor = null, Color? splashColor = null, object? heroTag = default!, double? elevation = null, double? focusElevation = null, double? hoverElevation = null, double? highlightElevation = null, double? disabledElevation = null, global::System.Action? onPressed = default!, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, bool mini = false, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, MaterialTapTargetSize? materialTapTargetSize = null, bool isExtended = false, bool? enableFeedback = null) : base(key: key)
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
        _floatingActionButtonType = mini ? _FloatingActionButtonType__floating_action_button.small : _FloatingActionButtonType__floating_action_button.regular;
        _extendedLabel = null;
        extendedIconLabelSpacing = null;
        extendedPadding = null;
        extendedTextStyle = null;
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
        System.Diagnostics.Debug.Assert((focusElevation is null) || (focusElevation >= 0.0));
        System.Diagnostics.Debug.Assert((hoverElevation is null) || (hoverElevation >= 0.0));
        System.Diagnostics.Debug.Assert((highlightElevation is null) || (highlightElevation >= 0.0));
        System.Diagnostics.Debug.Assert((disabledElevation is null) || (disabledElevation >= 0.0));
    }

    public static FloatingActionButton CreateSmall(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? child = null, string? tooltip = null, Color? foregroundColor = null, Color? backgroundColor = null, Color? focusColor = null, Color? hoverColor = null, Color? splashColor = null, object? heroTag = default!, double? elevation = null, double? focusElevation = null, double? hoverElevation = null, double? highlightElevation = null, double? disabledElevation = null, global::System.Action? onPressed = default!, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, MaterialTapTargetSize? materialTapTargetSize = null, bool? enableFeedback = null)
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

    public static FloatingActionButton CreateLarge(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? child = null, string? tooltip = null, Color? foregroundColor = null, Color? backgroundColor = null, Color? focusColor = null, Color? hoverColor = null, Color? splashColor = null, object? heroTag = default!, double? elevation = null, double? focusElevation = null, double? hoverElevation = null, double? highlightElevation = null, double? disabledElevation = null, global::System.Action? onPressed = default!, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, MaterialTapTargetSize? materialTapTargetSize = null, bool? enableFeedback = null)
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

    public static FloatingActionButton CreateExtended(global::Doroti.Framework.Foundation.Key? key = null, string? tooltip = null, Color? foregroundColor = null, Color? backgroundColor = null, Color? focusColor = null, Color? hoverColor = null, object? heroTag = default!, double? elevation = null, double? focusElevation = null, double? hoverElevation = null, Color? splashColor = null, double? highlightElevation = null, double? disabledElevation = null, global::System.Action? onPressed = default!, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, bool isExtended = true, MaterialTapTargetSize? materialTapTargetSize = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, double? extendedIconLabelSpacing = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? extendedPadding = null, global::Doroti.Framework.Painting.TextStyle? extendedTextStyle = null, global::Doroti.Framework.Widgets.Widget? icon = null, global::Doroti.Framework.Widgets.Widget label = default!, bool? enableFeedback = null)
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        FloatingActionButtonThemeData floatingActionButtonTheme = FloatingActionButtonTheme.of(context);
        FloatingActionButtonThemeData defaults = new _FABDefaultsM3__floating_action_button(context, _floatingActionButtonType, child is not null);
        global::Doroti.Ui.Color foregroundColorLocal = (foregroundColor ?? floatingActionButtonTheme.foregroundColor) ?? defaults.foregroundColor!;
        global::Doroti.Ui.Color backgroundColorLocal = (backgroundColor ?? floatingActionButtonTheme.backgroundColor) ?? defaults.backgroundColor!;
        global::Doroti.Ui.Color focusColorLocal = (focusColor ?? floatingActionButtonTheme.focusColor) ?? defaults.focusColor!;
        global::Doroti.Ui.Color hoverColorLocal = (hoverColor ?? floatingActionButtonTheme.hoverColor) ?? defaults.hoverColor!;
        global::Doroti.Ui.Color splashColorLocal = (splashColor ?? floatingActionButtonTheme.splashColor) ?? defaults.splashColor!;
        double elevationLocal = (elevation ?? floatingActionButtonTheme.elevation) ?? DartRuntimePrimitives.RequireValue(defaults.elevation);
        double focusElevationLocal = (focusElevation ?? floatingActionButtonTheme.focusElevation) ?? DartRuntimePrimitives.RequireValue(defaults.focusElevation);
        double hoverElevationLocal = (hoverElevation ?? floatingActionButtonTheme.hoverElevation) ?? DartRuntimePrimitives.RequireValue(defaults.hoverElevation);
        double disabledElevationLocal = ((disabledElevation ?? floatingActionButtonTheme.disabledElevation) ?? defaults.disabledElevation) ?? DartRuntimePrimitives.RequireValue(elevationLocal);
        double highlightElevationLocal = (highlightElevation ?? floatingActionButtonTheme.highlightElevation) ?? DartRuntimePrimitives.RequireValue(defaults.highlightElevation);
        MaterialTapTargetSize materialTapTargetSizeLocal = materialTapTargetSize ?? theme.materialTapTargetSize;
        bool enableFeedbackLocal = (enableFeedback ?? floatingActionButtonTheme.enableFeedback) ?? DartRuntimePrimitives.RequireValue(defaults.enableFeedback);
        double iconSizeLocal = floatingActionButtonTheme.iconSize ?? DartRuntimePrimitives.RequireValue(defaults.iconSize);
        global::Doroti.Framework.Painting.TextStyle extendedTextStyleLocal = ((extendedTextStyle ?? floatingActionButtonTheme.extendedTextStyle) ?? defaults.extendedTextStyle!).copyWith(color: foregroundColorLocal);
        global::Doroti.Framework.Painting.ShapeBorder shapeLocal = (shape ?? floatingActionButtonTheme.shape) ?? defaults.shape!;
        global::Doroti.Framework.Rendering.BoxConstraints sizeConstraintsLocal = default!;
        global::Doroti.Framework.Widgets.Widget? resolvedChild = (child is not null) ? IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(size: iconSizeLocal), child: child!) : child;
        switch (_floatingActionButtonType)
        {
            case _FloatingActionButtonType__floating_action_button.regular:
                {
                    sizeConstraintsLocal = floatingActionButtonTheme.sizeConstraints ?? defaults.sizeConstraints!;
                    break;
                }
            case _FloatingActionButtonType__floating_action_button.small:
                {
                    sizeConstraintsLocal = floatingActionButtonTheme.smallSizeConstraints ?? defaults.smallSizeConstraints!;
                    break;
                }
            case _FloatingActionButtonType__floating_action_button.large:
                {
                    sizeConstraintsLocal = floatingActionButtonTheme.largeSizeConstraints ?? defaults.largeSizeConstraints!;
                    break;
                }
            case _FloatingActionButtonType__floating_action_button.extended:
                {
                    sizeConstraintsLocal = floatingActionButtonTheme.extendedSizeConstraints ?? defaults.extendedSizeConstraints!;
                    double iconLabelSpacing = (extendedIconLabelSpacing ?? floatingActionButtonTheme.extendedIconLabelSpacing) ?? 8.0;
                    global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = (extendedPadding ?? floatingActionButtonTheme.extendedPadding) ?? defaults.extendedPadding!;
                    resolvedChild = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _ChildOverflowBox__floating_action_button(child: new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal, child: new global::Doroti.Framework.Widgets.Row(mainAxisSize: MainAxisSize.min, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection22381 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement22407 = child; if (__collectionElement22407 is { } __nonNullCollectionElement22407) { __collection22381.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement22407)); } if ((child is not null) && isExtended) { __collection22381.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: iconLabelSpacing))); } if (isExtended) { __collection22381.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_extendedLabel!)); } return __collection22381; }))()))));
                    break;
                }
        }
        global::Doroti.Framework.Widgets.Widget result = new RawMaterialButton(onPressed: onPressed, mouseCursor: new _EffectiveMouseCursor__floating_action_button(mouseCursor, floatingActionButtonTheme.mouseCursor), elevation: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(elevationLocal)), focusElevation: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(focusElevationLocal)), hoverElevation: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(hoverElevationLocal)), highlightElevation: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(highlightElevationLocal)), disabledElevation: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(disabledElevationLocal)), constraints: sizeConstraintsLocal, materialTapTargetSize: materialTapTargetSizeLocal, fillColor: backgroundColorLocal, focusColor: focusColorLocal, hoverColor: hoverColorLocal, splashColor: splashColorLocal, textStyle: extendedTextStyleLocal, shape: shapeLocal, clipBehavior: clipBehavior, focusNode: focusNode, autofocus: autofocus, enableFeedback: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(enableFeedbackLocal)), child: resolvedChild);
        if (tooltip is not null)
        {
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Tooltip(message: tooltip, child: result));
        }
        if (heroTag is not null)
        {
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Hero(tag: heroTag!, child: result));
        }
        return new global::Doroti.Framework.Widgets.MergeSemantics(child: result);
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action>("onPressed", onPressed, ifNull: "disabled"));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("tooltip", tooltip, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("foregroundColor", foregroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", focusColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("splashColor", splashColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<object>("heroTag", heroTag, ifPresent: "hero"));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("focusElevation", focusElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("hoverElevation", hoverElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("highlightElevation", highlightElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("disabledElevation", disabledElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.FocusNode>("focusNode", focusNode, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("isExtended", value: isExtended, ifTrue: "extended"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MaterialTapTargetSize>("materialTapTargetSize", materialTapTargetSize, defaultValue: null));
    }

}

internal class _EffectiveMouseCursor__floating_action_button : global::Doroti.Framework.Widgets.WidgetStateMouseCursor
{
    public virtual global::Doroti.Framework.Services.MouseCursor? widgetCursor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? themeCursor { get; private set; }

    internal _EffectiveMouseCursor__floating_action_button(global::Doroti.Framework.Services.MouseCursor? widgetCursor, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? themeCursor)
    {
        this.widgetCursor = widgetCursor;
        this.themeCursor = themeCursor;
    }

    public override global::Doroti.Framework.Services.MouseCursor resolve(HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
    {
        return (WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(widgetCursor, states) ?? (themeCursor?.resolve(states))) ?? adaptiveClickable.resolve(states);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescription => "WidgetStateMouseCursor(FloatActionButton)";
}

internal class _ChildOverflowBox__floating_action_button : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    internal _ChildOverflowBox__floating_action_button(global::Doroti.Framework.Widgets.Widget? child = null) : base(child: child)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderChildOverflowBox__floating_action_button(textDirection: Directionality.of(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderChildOverflowBox__floating_action_button)renderObject;
        __renderObject.textDirection = Directionality.of(context);
    }

}

public class _RenderChildOverflowBox__floating_action_button : global::Doroti.Framework.Rendering.RenderAligningShiftedBox
{
    internal _RenderChildOverflowBox__floating_action_button(TextDirection? textDirection = null) : base(textDirection: textDirection, alignment: Alignment.center)
    {
    }

    public override double computeMinIntrinsicWidth(double height) => 0.0;
    public override double computeMinIntrinsicHeight(double width) => 0.0;
    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        if (child is not null)
        {
            global::Doroti.Ui.Size childSize = child!.getDryLayout(new global::Doroti.Framework.Rendering.BoxConstraints());
            return new global::Doroti.Ui.Size(Math.Max(constraints.minWidth, Math.Min(constraints.maxWidth, childSize.width)), Math.Max(constraints.minHeight, Math.Min(constraints.maxHeight, childSize.height)));
        }
        else
        {
            return constraints.biggest;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraintsLocal = constraints;
        if (child is not null)
        {
            child!.layout(new global::Doroti.Framework.Rendering.BoxConstraints(), parentUsesSize: true);
            size = new global::Doroti.Ui.Size(Math.Max(constraintsLocal.minWidth, Math.Min(constraintsLocal.maxWidth, child!.size.width)), Math.Max(constraintsLocal.minHeight, Math.Min(constraintsLocal.maxHeight, child!.size.height)));
            alignChild();
        }
        else
        {
            size = constraintsLocal.biggest;
        }
    }

}

internal class _FABDefaultsM3__floating_action_button : FloatingActionButtonThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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
                __late__colors = Theme.of(context).colorScheme;
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
                __late__textTheme = Theme.of(context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _FABDefaultsM3__floating_action_button(global::Doroti.Framework.Widgets.BuildContext context, _FloatingActionButtonType__floating_action_button type, bool hasChild) : base(elevation: 6.0, focusElevation: 6.0, hoverElevation: 8.0, highlightElevation: 6.0, enableFeedback: true, sizeConstraints: BoxConstraints.CreateTightFor(width: 56.0, height: 56.0), smallSizeConstraints: BoxConstraints.CreateTightFor(width: 40.0, height: 40.0), largeSizeConstraints: BoxConstraints.CreateTightFor(width: 96.0, height: 96.0), extendedSizeConstraints: BoxConstraints.CreateTightFor(height: 56.0), extendedIconLabelSpacing: 8.0)
    {
        this.context = context;
        this.type = type;
        this.hasChild = hasChild;
    }

    internal virtual bool _isExtended => DartRuntimePrimitives.ConvertValue<bool>(Equals(type, _FloatingActionButtonType__floating_action_button.extended));
    public override global::Doroti.Ui.Color? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onPrimaryContainer);
    public override global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primaryContainer);
    public override global::Doroti.Ui.Color? splashColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onPrimaryContainer.withOpacity(0.1));
    public override global::Doroti.Ui.Color? focusColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onPrimaryContainer.withOpacity(0.1));
    public override global::Doroti.Ui.Color? hoverColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onPrimaryContainer.withOpacity(0.08));
    public override global::Doroti.Framework.Painting.ShapeBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(type switch { _FloatingActionButtonType__floating_action_button.regular => new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(16.0))), _FloatingActionButtonType__floating_action_button.small => new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(12.0))), _FloatingActionButtonType__floating_action_button.large => new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(28.0))), _FloatingActionButtonType__floating_action_button.extended => new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(16.0))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override double? iconSize => type switch { _FloatingActionButtonType__floating_action_button.regular => 24.0, _FloatingActionButtonType__floating_action_button.small => 24.0, _FloatingActionButtonType__floating_action_button.large => 36.0, _FloatingActionButtonType__floating_action_button.extended => 24.0, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? extendedPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsetsDirectional.CreateOnly(start: (hasChild && _isExtended) ? 16.0 : 20.0, end: 20.0));
    public override global::Doroti.Framework.Painting.TextStyle? extendedTextStyle => _textTheme.labelLarge;
}
