// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/dropdown_menu_theme.dart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Doroti.Framework.Material;

public class DropdownMenuThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Framework.Painting.TextStyle? textStyle { get; private set; }
    internal virtual object? _inputDecorationTheme { get; private set; }
    public virtual MenuStyle? menuStyle { get; private set; }
    public virtual Color? disabledColor { get; private set; }

    public DropdownMenuThemeData(global::Doroti.Framework.Painting.TextStyle? textStyle = null, object? inputDecorationTheme = null, MenuStyle? menuStyle = null, Color? disabledColor = null)
    {
        this.textStyle = textStyle;
        this.menuStyle = menuStyle;
        this.disabledColor = disabledColor;
        this._inputDecorationTheme = inputDecorationTheme;
        System.Diagnostics.Debug.Assert(((inputDecorationTheme is null) || (((inputDecorationTheme is InputDecorationTheme) || (inputDecorationTheme is InputDecorationThemeData)))));
    }

    public virtual InputDecorationThemeData? inputDecorationTheme
    {
        get
        {
            if ((this._inputDecorationTheme is null))
            {
                return null;
            }
            return DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>(this._inputDecorationTheme);
            return default!;
        }
    }
    public virtual DropdownMenuThemeData copyWith(global::Doroti.Framework.Painting.TextStyle? textStyle = null, object? inputDecorationTheme = null, MenuStyle? menuStyle = null, Color? disabledColor = null)
    {
        return new DropdownMenuThemeData(textStyle: (textStyle ?? this.textStyle), inputDecorationTheme: (((object?)inputDecorationTheme ?? (object?)this.inputDecorationTheme)), menuStyle: (menuStyle ?? this.menuStyle), disabledColor: (disabledColor ?? this.disabledColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DropdownMenuThemeData lerp(DropdownMenuThemeData? a, DropdownMenuThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new DropdownMenuThemeData(textStyle: TextStyle.lerp(a?.textStyle, b?.textStyle, t), inputDecorationTheme: ((t < 0.5) ? a?.inputDecorationTheme : b?.inputDecorationTheme), menuStyle: MenuStyle.lerp(a?.menuStyle, b?.menuStyle, t), disabledColor: Dart_uiLibrary.Color.lerp(a?.disabledColor, b?.disabledColor, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.textStyle, this.inputDecorationTheme, this.menuStyle, this.disabledColor));
    public override bool Equals(object? other)
    {
        var __other = other as DropdownMenuThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is DropdownMenuThemeData) && (object.Equals(((DropdownMenuThemeData)((DropdownMenuThemeData)__other)).textStyle, this.textStyle))) && (object.Equals(((DropdownMenuThemeData)((DropdownMenuThemeData)__other)).inputDecorationTheme, this.inputDecorationTheme))) && (object.Equals(((DropdownMenuThemeData)((DropdownMenuThemeData)__other)).menuStyle, this.menuStyle))) && (object.Equals(((DropdownMenuThemeData)((DropdownMenuThemeData)__other)).disabledColor, this.disabledColor)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("textStyle", this.textStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputDecorationThemeData>("inputDecorationThemeData", this.inputDecorationTheme, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MenuStyle>("menuStyle", this.menuStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledColor", this.disabledColor, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DropdownMenuTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual DropdownMenuThemeData data { get; private set; } = default!;

    public DropdownMenuTheme(global::Doroti.Framework.Foundation.Key? key = null, DropdownMenuThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static DropdownMenuThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return (DropdownMenuTheme.maybeOf(context) ?? Theme.of(context).dropdownMenuTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DropdownMenuThemeData? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<DropdownMenuTheme>()?.data;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new DropdownMenuTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((DropdownMenuTheme)oldWidget).data)));
}
