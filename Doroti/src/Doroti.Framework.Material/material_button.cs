// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/material_button.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class MaterialButton : StatelessWidget
{
    public virtual Action? onPressed { get; private set; }
    public virtual Action? onLongPress { get; private set; }
    public virtual Action<bool>? onHighlightChanged { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual ButtonTextTheme? textTheme { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual Color? disabledTextColor { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? hoverElevation { get; private set; }
    public virtual double? focusElevation { get; private set; }
    public virtual double? highlightElevation { get; private set; }
    public virtual double? disabledElevation { get; private set; }
    public virtual Brightness? colorBrightness { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Duration? animationDuration { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual double? minWidth { get; private set; }
    public virtual double? height { get; private set; }
    public virtual bool enableFeedback { get; private set; } = default!;

    public MaterialButton(Key? key = null, Action? onPressed = default!, Action? onLongPress = null, Action<bool>? onHighlightChanged = null, MouseCursor? mouseCursor = null, ButtonTextTheme? textTheme = null, Color? textColor = null, Color? disabledTextColor = null, Color? color = null, Color? disabledColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Brightness? colorBrightness = null, double? elevation = null, double? focusElevation = null, double? hoverElevation = null, double? highlightElevation = null, double? disabledElevation = null, EdgeInsetsGeometry? padding = null, VisualDensity? visualDensity = null, ShapeBorder? shape = null, Clip clipBehavior = Clip.none, FocusNode? focusNode = null, bool autofocus = false, MaterialTapTargetSize? materialTapTargetSize = null, Duration? animationDuration = null, double? minWidth = null, double? height = null, bool enableFeedback = true, Widget? child = null) : base(key: key)
    {
        this.onPressed = onPressed;
        this.onLongPress = onLongPress;
        this.onHighlightChanged = onHighlightChanged;
        this.mouseCursor = mouseCursor;
        this.textTheme = textTheme;
        this.textColor = textColor;
        this.disabledTextColor = disabledTextColor;
        this.color = color;
        this.disabledColor = disabledColor;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.highlightColor = highlightColor;
        this.splashColor = splashColor;
        this.colorBrightness = colorBrightness;
        this.elevation = elevation;
        this.focusElevation = focusElevation;
        this.hoverElevation = hoverElevation;
        this.highlightElevation = highlightElevation;
        this.disabledElevation = disabledElevation;
        this.padding = padding;
        this.visualDensity = visualDensity;
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.materialTapTargetSize = materialTapTargetSize;
        this.animationDuration = animationDuration;
        this.minWidth = minWidth;
        this.height = height;
        this.enableFeedback = enableFeedback;
        this.child = child;
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
        System.Diagnostics.Debug.Assert((focusElevation is null) || (focusElevation >= 0.0));
        System.Diagnostics.Debug.Assert((hoverElevation is null) || (hoverElevation >= 0.0));
        System.Diagnostics.Debug.Assert((highlightElevation is null) || (highlightElevation >= 0.0));
        System.Diagnostics.Debug.Assert((disabledElevation is null) || (disabledElevation >= 0.0));
    }

    public virtual bool enabled => DartRuntimePrimitives.ConvertValue<bool>((onPressed is not null) || (onLongPress is not null));
    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        ButtonThemeData buttonTheme = ButtonTheme.of(context);
        return new RawMaterialButton(onPressed: onPressed, onLongPress: onLongPress, enableFeedback: enableFeedback, onHighlightChanged: onHighlightChanged, mouseCursor: mouseCursor, fillColor: buttonTheme.getFillColor(this), textStyle: theme.textTheme.labelLarge!.copyWith(color: buttonTheme.getTextColor(this)), focusColor: focusColor ?? buttonTheme.getFocusColor(this), hoverColor: hoverColor ?? buttonTheme.getHoverColor(this), highlightColor: highlightColor ?? theme.highlightColor, splashColor: splashColor ?? theme.splashColor, elevation: buttonTheme.getElevation(this), focusElevation: buttonTheme.getFocusElevation(this), hoverElevation: buttonTheme.getHoverElevation(this), highlightElevation: buttonTheme.getHighlightElevation(this), padding: buttonTheme.getPadding(this), visualDensity: visualDensity ?? theme.visualDensity, constraints: buttonTheme.getConstraints(this).copyWith(minWidth: minWidth, minHeight: height), shape: buttonTheme.getShape(this), clipBehavior: clipBehavior, focusNode: focusNode, autofocus: autofocus, animationDuration: buttonTheme.getAnimationDuration(this), materialTapTargetSize: materialTapTargetSize ?? theme.materialTapTargetSize, disabledElevation: disabledElevation ?? 0.0, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagProperty("enabled", value: enabled, ifFalse: "disabled"));
        properties.add(new DiagnosticsProperty<ButtonTextTheme>("textTheme", textTheme, defaultValue: null));
        properties.add(new ColorProperty("textColor", textColor, defaultValue: null));
        properties.add(new ColorProperty("disabledTextColor", disabledTextColor, defaultValue: null));
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new ColorProperty("disabledColor", disabledColor, defaultValue: null));
        properties.add(new ColorProperty("focusColor", focusColor, defaultValue: null));
        properties.add(new ColorProperty("hoverColor", hoverColor, defaultValue: null));
        properties.add(new ColorProperty("highlightColor", highlightColor, defaultValue: null));
        properties.add(new ColorProperty("splashColor", splashColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<Brightness>("colorBrightness", colorBrightness, defaultValue: null));
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding, defaultValue: null));
        properties.add(new DiagnosticsProperty<VisualDensity>("visualDensity", visualDensity, defaultValue: null));
        properties.add(new DiagnosticsProperty<ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(new DiagnosticsProperty<FocusNode>("focusNode", focusNode, defaultValue: null));
        properties.add(new DiagnosticsProperty<MaterialTapTargetSize>("materialTapTargetSize", materialTapTargetSize, defaultValue: null));
    }

}
