// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/divider_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class DividerThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? color { get; private set; }
    public virtual double? space { get; private set; }
    public virtual double? thickness { get; private set; }
    public virtual double? indent { get; private set; }
    public virtual double? endIndent { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? radius { get; private set; }

    public DividerThemeData(Color? color = null, double? space = null, double? thickness = null, double? indent = null, double? endIndent = null, global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? radius = null)
    {
        this.color = color;
        this.space = space;
        this.thickness = thickness;
        this.indent = indent;
        this.endIndent = endIndent;
        this.radius = radius;
    }

    public virtual DividerThemeData copyWith(Color? color = null, double? space = null, double? thickness = null, double? indent = null, double? endIndent = null, global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? radius = null)
    {
        return new DividerThemeData(color: (color ?? this.color), space: (space ?? this.space), thickness: (thickness ?? this.thickness), indent: (indent ?? this.indent), endIndent: (endIndent ?? this.endIndent), radius: (radius ?? this.radius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DividerThemeData lerp(DividerThemeData? a, DividerThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new DividerThemeData(color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t), space: Dart_uiLibrary.lerpDouble(a?.space, b?.space, t), thickness: Dart_uiLibrary.lerpDouble(a?.thickness, b?.thickness, t), indent: Dart_uiLibrary.lerpDouble(a?.indent, b?.indent, t), endIndent: Dart_uiLibrary.lerpDouble(a?.endIndent, b?.endIndent, t), radius: BorderRadiusGeometry.lerp(a?.radius, b?.radius, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.color, this.space, this.thickness, this.indent, this.endIndent, this.radius));
    public override bool Equals(object? other)
    {
        var __other = other as DividerThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((__other is DividerThemeData) && (object.Equals(((DividerThemeData)((DividerThemeData)__other)).color, this.color))) && (((DividerThemeData)((DividerThemeData)__other)).space == this.space)) && (((DividerThemeData)((DividerThemeData)__other)).thickness == this.thickness)) && (((DividerThemeData)((DividerThemeData)__other)).indent == this.indent)) && (((DividerThemeData)((DividerThemeData)__other)).endIndent == this.endIndent)) && (object.Equals(((DividerThemeData)((DividerThemeData)__other)).radius, this.radius)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("space", this.space, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("thickness", this.thickness, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("indent", this.indent, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("endIndent", this.endIndent, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry>("radius", this.radius, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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

public class DividerTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual DividerThemeData data { get; private set; } = default!;

    public DividerTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, DividerThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static DividerThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DividerTheme? dividerTheme__5792 = ((DividerTheme?)(object?)context.dependOnInheritedWidgetOfExactType<DividerTheme>());
        return (dividerTheme__5792?.data ?? Theme.of(context).dividerTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new DividerTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((DividerTheme)oldWidget).data)));
}
