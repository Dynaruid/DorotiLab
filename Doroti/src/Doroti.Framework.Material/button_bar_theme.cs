// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button_bar_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ButtonBarThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Framework.Rendering.MainAxisAlignment? alignment { get; private set; }
    public virtual global::Doroti.Framework.Rendering.MainAxisSize? mainAxisSize { get; private set; }
    public virtual ButtonTextTheme? buttonTextTheme { get; private set; }
    public virtual double? buttonMinWidth { get; private set; }
    public virtual double? buttonHeight { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? buttonPadding { get; private set; }
    public virtual bool? buttonAlignedDropdown { get; private set; }
    public virtual ButtonBarLayoutBehavior? layoutBehavior { get; private set; }
    public virtual global::Doroti.Framework.Painting.VerticalDirection? overflowDirection { get; private set; }

    public ButtonBarThemeData(global::Doroti.Framework.Rendering.MainAxisAlignment? alignment = null, global::Doroti.Framework.Rendering.MainAxisSize? mainAxisSize = null, ButtonTextTheme? buttonTextTheme = null, double? buttonMinWidth = null, double? buttonHeight = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? buttonPadding = null, bool? buttonAlignedDropdown = null, ButtonBarLayoutBehavior? layoutBehavior = null, global::Doroti.Framework.Painting.VerticalDirection? overflowDirection = null)
    {
        this.alignment = alignment;
        this.mainAxisSize = mainAxisSize;
        this.buttonTextTheme = buttonTextTheme;
        this.buttonMinWidth = buttonMinWidth;
        this.buttonHeight = buttonHeight;
        this.buttonPadding = buttonPadding;
        this.buttonAlignedDropdown = buttonAlignedDropdown;
        this.layoutBehavior = layoutBehavior;
        this.overflowDirection = overflowDirection;
        System.Diagnostics.Debug.Assert((buttonMinWidth is null) || (buttonMinWidth >= 0.0));
        System.Diagnostics.Debug.Assert((buttonHeight is null) || (buttonHeight >= 0.0));
    }

    public virtual ButtonBarThemeData copyWith(global::Doroti.Framework.Rendering.MainAxisAlignment? alignment = null, global::Doroti.Framework.Rendering.MainAxisSize? mainAxisSize = null, ButtonTextTheme? buttonTextTheme = null, double? buttonMinWidth = null, double? buttonHeight = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? buttonPadding = null, bool? buttonAlignedDropdown = null, ButtonBarLayoutBehavior? layoutBehavior = null, global::Doroti.Framework.Painting.VerticalDirection? overflowDirection = null)
    {
        return new ButtonBarThemeData(alignment: alignment ?? this.alignment, mainAxisSize: mainAxisSize ?? this.mainAxisSize, buttonTextTheme: buttonTextTheme ?? this.buttonTextTheme, buttonMinWidth: buttonMinWidth ?? this.buttonMinWidth, buttonHeight: buttonHeight ?? this.buttonHeight, buttonPadding: buttonPadding ?? this.buttonPadding, buttonAlignedDropdown: buttonAlignedDropdown ?? this.buttonAlignedDropdown, layoutBehavior: layoutBehavior ?? this.layoutBehavior, overflowDirection: overflowDirection ?? this.overflowDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ButtonBarThemeData? lerp(ButtonBarThemeData? a, ButtonBarThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ButtonBarThemeData(alignment: (t < 0.5) ? a?.alignment : b?.alignment, mainAxisSize: (t < 0.5) ? a?.mainAxisSize : b?.mainAxisSize, buttonTextTheme: (t < 0.5) ? a?.buttonTextTheme : b?.buttonTextTheme, buttonMinWidth: Dart_uiLibrary.lerpDouble(a?.buttonMinWidth, b?.buttonMinWidth, t), buttonHeight: Dart_uiLibrary.lerpDouble(a?.buttonHeight, b?.buttonHeight, t), buttonPadding: EdgeInsetsGeometry.lerp(a?.buttonPadding, b?.buttonPadding, t), buttonAlignedDropdown: (t < 0.5) ? a?.buttonAlignedDropdown : b?.buttonAlignedDropdown, layoutBehavior: (t < 0.5) ? a?.layoutBehavior : b?.layoutBehavior, overflowDirection: (t < 0.5) ? a?.overflowDirection : b?.overflowDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(alignment, mainAxisSize, buttonTextTheme, buttonMinWidth, buttonHeight, buttonPadding, buttonAlignedDropdown, layoutBehavior, overflowDirection));
    public override bool Equals(object? other)
    {
        var __other = other as ButtonBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ButtonBarThemeData) && Equals(__other.alignment, alignment) && Equals(__other.mainAxisSize, mainAxisSize) && Equals(__other.buttonTextTheme, buttonTextTheme) && (__other.buttonMinWidth == buttonMinWidth) && (__other.buttonHeight == buttonHeight) && Equals(__other.buttonPadding, buttonPadding) && (__other.buttonAlignedDropdown == buttonAlignedDropdown) && Equals(__other.layoutBehavior, layoutBehavior) && Equals(__other.overflowDirection, overflowDirection);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.MainAxisAlignment>("alignment", alignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.MainAxisSize>("mainAxisSize", mainAxisSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonTextTheme>("textTheme", buttonTextTheme, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("minWidth", buttonMinWidth, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("height", buttonHeight, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", buttonPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("buttonAlignedDropdown", value: buttonAlignedDropdown, ifTrue: "dropdown width matches button"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonBarLayoutBehavior>("layoutBehavior", layoutBehavior, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.VerticalDirection>("overflowDirection", overflowDirection, defaultValue: null));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ButtonBarTheme : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual ButtonBarThemeData data { get; private set; } = default!;

    public ButtonBarTheme(global::Doroti.Framework.Foundation.Key? key = null, ButtonBarThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ButtonBarThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ButtonBarTheme? buttonBarThemeLocal = context.dependOnInheritedWidgetOfExactType<ButtonBarTheme>();
        return buttonBarThemeLocal?.data ?? Theme.of(context).buttonBarTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((ButtonBarTheme)oldWidget).data));
}
