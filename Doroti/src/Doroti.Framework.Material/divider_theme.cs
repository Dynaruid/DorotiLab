// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/divider_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class DividerThemeData : Diagnosticable
{
    public virtual Color? color { get; private set; }
    public virtual double? space { get; private set; }
    public virtual double? thickness { get; private set; }
    public virtual double? indent { get; private set; }
    public virtual double? endIndent { get; private set; }
    public virtual BorderRadiusGeometry? radius { get; private set; }

    public DividerThemeData(
        Color? color = null,
        double? space = null,
        double? thickness = null,
        double? indent = null,
        double? endIndent = null,
        BorderRadiusGeometry? radius = null
    )
    {
        this.color = color;
        this.space = space;
        this.thickness = thickness;
        this.indent = indent;
        this.endIndent = endIndent;
        this.radius = radius;
    }

    public virtual DividerThemeData copyWith(
        Color? color = null,
        double? space = null,
        double? thickness = null,
        double? indent = null,
        double? endIndent = null,
        BorderRadiusGeometry? radius = null
    )
    {
        return new DividerThemeData(
            color: color ?? this.color,
            space: space ?? this.space,
            thickness: thickness ?? this.thickness,
            indent: indent ?? this.indent,
            endIndent: endIndent ?? this.endIndent,
            radius: radius ?? this.radius
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static DividerThemeData lerp(DividerThemeData? a, DividerThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new DividerThemeData(
            color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t),
            space: Dart_uiLibrary.lerpDouble(a?.space, b?.space, t),
            thickness: Dart_uiLibrary.lerpDouble(a?.thickness, b?.thickness, t),
            indent: Dart_uiLibrary.lerpDouble(a?.indent, b?.indent, t),
            endIndent: Dart_uiLibrary.lerpDouble(a?.endIndent, b?.endIndent, t),
            radius: BorderRadiusGeometry.lerp(a?.radius, b?.radius, t)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(color, space, thickness, indent, endIndent, radius)
        );

    public override bool Equals(object? other)
    {
        var __other = other as DividerThemeData;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is DividerThemeData)
            && Equals(__other.color, color)
            && (__other.space == space)
            && (__other.thickness == thickness)
            && (__other.indent == indent)
            && (__other.endIndent == endIndent)
            && Equals(__other.radius, radius);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new DoubleProperty("space", space, defaultValue: null));
        properties.add(new DoubleProperty("thickness", thickness, defaultValue: null));
        properties.add(new DoubleProperty("indent", indent, defaultValue: null));
        properties.add(new DoubleProperty("endIndent", endIndent, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<BorderRadiusGeometry>("radius", radius, defaultValue: null)
        );
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class DividerTheme : InheritedTheme
{
    public virtual DividerThemeData data { get; private set; } = default!;

    public DividerTheme(Key? key = null, DividerThemeData data = default!, Widget child = default!)
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static DividerThemeData of(BuildContext context)
    {
        DividerTheme? dividerThemeLocal =
            context.dependOnInheritedWidgetOfExactType<DividerTheme>();
        return dividerThemeLocal?.data ?? Theme.of(context).dividerTheme;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new DividerTheme(data: data, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((DividerTheme)oldWidget).data));
}
