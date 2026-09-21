// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/bottom_app_bar_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class BottomAppBarTheme : InheritedTheme, Diagnosticable
{
    internal virtual BottomAppBarThemeData? _data { get; private set; }
    internal virtual Color? _color { get; private set; }
    internal virtual double? _elevation { get; private set; }
    internal virtual NotchedShape? _shape { get; private set; }
    internal virtual double? _height { get; private set; }
    internal virtual Color? _surfaceTintColor { get; private set; }
    internal virtual Color? _shadowColor { get; private set; }
    internal virtual EdgeInsetsGeometry? _padding { get; private set; }

    public BottomAppBarTheme(
        Key? key = null,
        Color? color = null,
        double? elevation = null,
        NotchedShape? shape = null,
        double? height = null,
        Color? surfaceTintColor = null,
        Color? shadowColor = null,
        EdgeInsetsGeometry? padding = null,
        BottomAppBarThemeData? data = null,
        Widget? child = null
    )
        : base(key: key, child: child ?? SizedBox.CreateShrink())
    {
        _color = color;
        _elevation = elevation;
        _shape = shape;
        _height = height;
        _surfaceTintColor = surfaceTintColor;
        _shadowColor = shadowColor;
        _padding = padding;
        _data = data;
        System.Diagnostics.Debug.Assert(
            (data is null)
                || (
                    (
                        (
                            (
                                ((((object?)color ?? elevation) ?? shape) ?? height)
                                ?? surfaceTintColor
                            ) ?? shadowColor
                        ) ?? padding
                    )
                    is null
                )
        );
    }

    public virtual Color? color =>
        DartRuntimePrimitives.ConvertValue<Color>((_data is not null) ? _data.color : _color);
    public virtual double? elevation => (_data is not null) ? _data.elevation : _elevation;
    public virtual NotchedShape? shape => (_data is not null) ? _data.shape : _shape;
    public virtual double? height => (_data is not null) ? _data.height : _height;
    public virtual Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            (_data is not null) ? _data.surfaceTintColor : _surfaceTintColor
        );
    public virtual Color? shadowColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            (_data is not null) ? _data.shadowColor : _shadowColor
        );
    public virtual EdgeInsetsGeometry? padding => (_data is not null) ? _data.padding : _padding;
    public virtual BottomAppBarThemeData data =>
        DartRuntimePrimitives.ConvertValue<BottomAppBarThemeData>(
            _data
                ?? new BottomAppBarThemeData(
                    color: _color,
                    elevation: _elevation,
                    shape: _shape,
                    height: _height,
                    surfaceTintColor: _surfaceTintColor,
                    shadowColor: _shadowColor,
                    padding: _padding
                )
        );

    public virtual BottomAppBarTheme copyWith(
        Color? color = null,
        double? elevation = null,
        NotchedShape? shape = null,
        double? height = null,
        Color? surfaceTintColor = null,
        Color? shadowColor = null,
        EdgeInsetsGeometry? padding = null
    )
    {
        return new BottomAppBarTheme(
            color: color ?? this.color,
            elevation: elevation ?? this.elevation,
            shape: shape ?? this.shape,
            height: height ?? this.height,
            surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor,
            shadowColor: shadowColor ?? this.shadowColor,
            padding: padding ?? this.padding
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BottomAppBarThemeData of(BuildContext context)
    {
        BottomAppBarTheme? bottomAppBarThemeLocal =
            context.dependOnInheritedWidgetOfExactType<BottomAppBarTheme>();
        return bottomAppBarThemeLocal?.data ?? Theme.of(context).bottomAppBarTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BottomAppBarTheme lerp(BottomAppBarTheme? a, BottomAppBarTheme? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new BottomAppBarTheme(
            color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t),
            elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t),
            shape: (t < 0.5) ? a?.shape : b?.shape,
            height: Dart_uiLibrary.lerpDouble(a?.height, b?.height, t),
            surfaceTintColor: Dart_uiLibrary.Color.lerp(
                a?.surfaceTintColor,
                b?.surfaceTintColor,
                t
            ),
            shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t),
            padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !Equals(data, ((BottomAppBarTheme)oldWidget).data)
        );

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new BottomAppBarTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties) { }
}

public class BottomAppBarThemeData : Diagnosticable
{
    public virtual Color? color { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual NotchedShape? shape { get; private set; }
    public virtual double? height { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }

    public BottomAppBarThemeData(
        Color? color = null,
        double? elevation = null,
        NotchedShape? shape = null,
        double? height = null,
        Color? surfaceTintColor = null,
        Color? shadowColor = null,
        EdgeInsetsGeometry? padding = null
    )
    {
        this.color = color;
        this.elevation = elevation;
        this.shape = shape;
        this.height = height;
        this.surfaceTintColor = surfaceTintColor;
        this.shadowColor = shadowColor;
        this.padding = padding;
    }

    public virtual BottomAppBarThemeData copyWith(
        Color? color = null,
        double? elevation = null,
        NotchedShape? shape = null,
        double? height = null,
        Color? surfaceTintColor = null,
        Color? shadowColor = null,
        EdgeInsetsGeometry? padding = null
    )
    {
        return new BottomAppBarThemeData(
            color: color ?? this.color,
            elevation: elevation ?? this.elevation,
            shape: shape ?? this.shape,
            height: height ?? this.height,
            surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor,
            shadowColor: shadowColor ?? this.shadowColor,
            padding: padding ?? this.padding
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BottomAppBarThemeData lerp(
        BottomAppBarThemeData? a,
        BottomAppBarThemeData? b,
        double t
    )
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new BottomAppBarThemeData(
            color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t),
            elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t),
            shape: (t < 0.5) ? a?.shape : b?.shape,
            height: Dart_uiLibrary.lerpDouble(a?.height, b?.height, t),
            surfaceTintColor: Dart_uiLibrary.Color.lerp(
                a?.surfaceTintColor,
                b?.surfaceTintColor,
                t
            ),
            shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t),
            padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                color,
                elevation,
                shape,
                height,
                surfaceTintColor,
                shadowColor,
                padding
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as BottomAppBarThemeData;
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
        return (__other is BottomAppBarThemeData)
            && Equals(__other.color, color)
            && (__other.elevation == elevation)
            && Equals(__other.shape, shape)
            && (__other.height == height)
            && Equals(__other.surfaceTintColor, surfaceTintColor)
            && Equals(__other.shadowColor, shadowColor)
            && Equals(__other.padding, padding);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new DiagnosticsProperty<NotchedShape?>("shape", shape, defaultValue: null));
        properties.add(new DoubleProperty("height", height, defaultValue: null));
        properties.add(new ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry?>("padding", padding, defaultValue: null)
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
