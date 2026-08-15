// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/material_button.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class MaterialButton : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::System.Action<bool>? onHighlightChanged { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
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
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Duration? animationDuration { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual double? minWidth { get; private set; }
    public virtual double? height { get; private set; }
    public virtual bool enableFeedback { get; private set; } = default!;

    public MaterialButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHighlightChanged = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, ButtonTextTheme? textTheme = null, Color? textColor = null, Color? disabledTextColor = null, Color? color = null, Color? disabledColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Brightness? colorBrightness = null, double? elevation = null, double? focusElevation = null, double? hoverElevation = null, double? highlightElevation = null, double? disabledElevation = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, VisualDensity? visualDensity = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, MaterialTapTargetSize? materialTapTargetSize = null, Duration? animationDuration = null, double? minWidth = null, double? height = null, bool enableFeedback = true, global::Doroti.Generated.Framework.Widgets.Widget? child = null) : base(key: key)
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
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((focusElevation is null) || (focusElevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((hoverElevation is null) || (hoverElevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((highlightElevation is null) || (highlightElevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((disabledElevation is null) || (disabledElevation >= 0.0)));
    }

    public virtual bool enabled => DartRuntimePrimitives.ConvertValue<bool>(((this.onPressed is not null) || (this.onLongPress is not null)));
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__13728 = Theme.of(context);
        ButtonThemeData buttonTheme__13781 = ButtonTheme.of(context);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new RawMaterialButton(onPressed: () => this.onPressed(), onLongPress: this.onLongPress, enableFeedback: this.enableFeedback, onHighlightChanged: (global::System.Action<bool>?)this.onHighlightChanged, mouseCursor: this.mouseCursor, fillColor: buttonTheme__13781.getFillColor(this), textStyle: theme__13728.textTheme.labelLarge!.copyWith(color: buttonTheme__13781.getTextColor(this)), focusColor: (this.focusColor ?? buttonTheme__13781.getFocusColor(this)), hoverColor: (this.hoverColor ?? buttonTheme__13781.getHoverColor(this)), highlightColor: (this.highlightColor ?? theme__13728.highlightColor), splashColor: (this.splashColor ?? theme__13728.splashColor), elevation: buttonTheme__13781.getElevation(this), focusElevation: buttonTheme__13781.getFocusElevation(this), hoverElevation: buttonTheme__13781.getHoverElevation(this), highlightElevation: buttonTheme__13781.getHighlightElevation(this), padding: buttonTheme__13781.getPadding(this), visualDensity: (this.visualDensity ?? theme__13728.visualDensity), constraints: buttonTheme__13781.getConstraints(this).copyWith(minWidth: this.minWidth, minHeight: this.height), shape: buttonTheme__13781.getShape(this), clipBehavior: this.clipBehavior, focusNode: this.focusNode, autofocus: this.autofocus, animationDuration: buttonTheme__13781.getAnimationDuration(this), materialTapTargetSize: (this.materialTapTargetSize ?? theme__13728.materialTapTargetSize), disabledElevation: (this.disabledElevation ?? 0.0), child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("enabled", value: this.enabled, ifFalse: "disabled"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ButtonTextTheme>("textTheme", this.textTheme, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("textColor", this.textColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("disabledTextColor", this.disabledTextColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("disabledColor", this.disabledColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("focusColor", this.focusColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("hoverColor", this.hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("highlightColor", this.highlightColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("splashColor", this.splashColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Flutter.Ui.Brightness>("colorBrightness", this.colorBrightness, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", this.visualDensity, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.FocusNode>("focusNode", this.focusNode, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<MaterialTapTargetSize>("materialTapTargetSize", this.materialTapTargetSize, defaultValue: null));
    }

}
