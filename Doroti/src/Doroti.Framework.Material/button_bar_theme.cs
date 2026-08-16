// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button_bar_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
        System.Diagnostics.Debug.Assert(((buttonMinWidth is null) || (buttonMinWidth >= 0.0)));
        System.Diagnostics.Debug.Assert(((buttonHeight is null) || (buttonHeight >= 0.0)));
    }

    public virtual ButtonBarThemeData copyWith(global::Doroti.Framework.Rendering.MainAxisAlignment? alignment = null, global::Doroti.Framework.Rendering.MainAxisSize? mainAxisSize = null, ButtonTextTheme? buttonTextTheme = null, double? buttonMinWidth = null, double? buttonHeight = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? buttonPadding = null, bool? buttonAlignedDropdown = null, ButtonBarLayoutBehavior? layoutBehavior = null, global::Doroti.Framework.Painting.VerticalDirection? overflowDirection = null)
    {
        return new ButtonBarThemeData(alignment: (alignment ?? this.alignment), mainAxisSize: (mainAxisSize ?? this.mainAxisSize), buttonTextTheme: (buttonTextTheme ?? this.buttonTextTheme), buttonMinWidth: (buttonMinWidth ?? this.buttonMinWidth), buttonHeight: (buttonHeight ?? this.buttonHeight), buttonPadding: (buttonPadding ?? this.buttonPadding), buttonAlignedDropdown: (buttonAlignedDropdown ?? this.buttonAlignedDropdown), layoutBehavior: (layoutBehavior ?? this.layoutBehavior), overflowDirection: (overflowDirection ?? this.overflowDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ButtonBarThemeData? lerp(ButtonBarThemeData? a, ButtonBarThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ButtonBarThemeData(alignment: ((t < 0.5) ? a?.alignment : b?.alignment), mainAxisSize: ((t < 0.5) ? a?.mainAxisSize : b?.mainAxisSize), buttonTextTheme: ((t < 0.5) ? a?.buttonTextTheme : b?.buttonTextTheme), buttonMinWidth: Dart_uiLibrary.lerpDouble(a?.buttonMinWidth, b?.buttonMinWidth, t), buttonHeight: Dart_uiLibrary.lerpDouble(a?.buttonHeight, b?.buttonHeight, t), buttonPadding: EdgeInsetsGeometry.lerp(a?.buttonPadding, b?.buttonPadding, t), buttonAlignedDropdown: ((t < 0.5) ? a?.buttonAlignedDropdown : b?.buttonAlignedDropdown), layoutBehavior: ((t < 0.5) ? a?.layoutBehavior : b?.layoutBehavior), overflowDirection: ((t < 0.5) ? a?.overflowDirection : b?.overflowDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.alignment, this.mainAxisSize, this.buttonTextTheme, this.buttonMinWidth, this.buttonHeight, this.buttonPadding, this.buttonAlignedDropdown, this.layoutBehavior, this.overflowDirection));
    public override bool Equals(object? other)
    {
        var __other = other as ButtonBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((__other is ButtonBarThemeData) && (object.Equals(((ButtonBarThemeData)((ButtonBarThemeData)__other)).alignment, this.alignment))) && (object.Equals(((ButtonBarThemeData)((ButtonBarThemeData)__other)).mainAxisSize, this.mainAxisSize))) && (object.Equals(((ButtonBarThemeData)((ButtonBarThemeData)__other)).buttonTextTheme, this.buttonTextTheme))) && (((ButtonBarThemeData)((ButtonBarThemeData)__other)).buttonMinWidth == this.buttonMinWidth)) && (((ButtonBarThemeData)((ButtonBarThemeData)__other)).buttonHeight == this.buttonHeight)) && (object.Equals(((ButtonBarThemeData)((ButtonBarThemeData)__other)).buttonPadding, this.buttonPadding))) && (((ButtonBarThemeData)((ButtonBarThemeData)__other)).buttonAlignedDropdown == this.buttonAlignedDropdown)) && (object.Equals(((ButtonBarThemeData)((ButtonBarThemeData)__other)).layoutBehavior, this.layoutBehavior))) && (object.Equals(((ButtonBarThemeData)((ButtonBarThemeData)__other)).overflowDirection, this.overflowDirection)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.MainAxisAlignment>("alignment", this.alignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.MainAxisSize>("mainAxisSize", this.mainAxisSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonTextTheme>("textTheme", this.buttonTextTheme, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("minWidth", this.buttonMinWidth, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("height", this.buttonHeight, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.buttonPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("buttonAlignedDropdown", value: this.buttonAlignedDropdown, ifTrue: "dropdown width matches button"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonBarLayoutBehavior>("layoutBehavior", this.layoutBehavior, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.VerticalDirection>("overflowDirection", this.overflowDirection, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
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
        ButtonBarTheme? buttonBarTheme__10396 = ((ButtonBarTheme?)(object?)context.dependOnInheritedWidgetOfExactType<ButtonBarTheme>());
        return (buttonBarTheme__10396?.data ?? Theme.of(context).buttonBarTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((ButtonBarTheme)oldWidget).data)));
}
