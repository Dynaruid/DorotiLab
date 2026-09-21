// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/checkbox_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class CheckboxThemeData : Diagnosticable
{
    public virtual WidgetStateProperty<MouseCursor?>? mouseCursor { get; private set; }
    public virtual WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? checkColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual OutlinedBorder? shape { get; private set; }
    public virtual BorderSide? side { get; private set; }

    public CheckboxThemeData(
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        WidgetStateProperty<Color?>? fillColor = null,
        WidgetStateProperty<Color?>? checkColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        double? splashRadius = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        VisualDensity? visualDensity = null,
        OutlinedBorder? shape = null,
        BorderSide? side = null
    )
    {
        this.mouseCursor = mouseCursor;
        this.fillColor = fillColor;
        this.checkColor = checkColor;
        this.overlayColor = overlayColor;
        this.splashRadius = splashRadius;
        this.materialTapTargetSize = materialTapTargetSize;
        this.visualDensity = visualDensity;
        this.shape = shape;
        this.side = side;
    }

    public virtual CheckboxThemeData copyWith(
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        WidgetStateProperty<Color?>? fillColor = null,
        WidgetStateProperty<Color?>? checkColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        double? splashRadius = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        VisualDensity? visualDensity = null,
        OutlinedBorder? shape = null,
        BorderSide? side = null
    )
    {
        return new CheckboxThemeData(
            mouseCursor: mouseCursor ?? this.mouseCursor,
            fillColor: fillColor ?? this.fillColor,
            checkColor: checkColor ?? this.checkColor,
            overlayColor: overlayColor ?? this.overlayColor,
            splashRadius: splashRadius ?? this.splashRadius,
            materialTapTargetSize: materialTapTargetSize ?? this.materialTapTargetSize,
            visualDensity: visualDensity ?? this.visualDensity,
            shape: shape ?? this.shape,
            side: side ?? this.side
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CheckboxThemeData lerp(CheckboxThemeData? a, CheckboxThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new CheckboxThemeData(
            mouseCursor: (t < 0.5) ? a?.mouseCursor : b?.mouseCursor,
            fillColor: WidgetStateProperty.lerp(a?.fillColor, b?.fillColor, t, Color.lerp),
            checkColor: WidgetStateProperty.lerp(a?.checkColor, b?.checkColor, t, Color.lerp),
            overlayColor: WidgetStateProperty.lerp(a?.overlayColor, b?.overlayColor, t, Color.lerp),
            splashRadius: Dart_uiLibrary.lerpDouble(a?.splashRadius, b?.splashRadius, t),
            materialTapTargetSize: (t < 0.5) ? a?.materialTapTargetSize : b?.materialTapTargetSize,
            visualDensity: (t < 0.5) ? a?.visualDensity : b?.visualDensity,
            shape: ((OutlinedBorder?)ShapeBorder.lerp(a?.shape, b?.shape, t))!,
            side: _lerpSides(a?.side, b?.side, t)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                mouseCursor,
                fillColor,
                checkColor,
                overlayColor,
                splashRadius,
                materialTapTargetSize,
                visualDensity,
                shape,
                side
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as CheckboxThemeData;
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
        return (__other is CheckboxThemeData)
            && Equals(__other.mouseCursor, mouseCursor)
            && Equals(__other.fillColor, fillColor)
            && Equals(__other.checkColor, checkColor)
            && Equals(__other.overlayColor, overlayColor)
            && (__other.splashRadius == splashRadius)
            && Equals(__other.materialTapTargetSize, materialTapTargetSize)
            && Equals(__other.visualDensity, visualDensity)
            && Equals(__other.shape, shape)
            && Equals(__other.side, side);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<MouseCursor?>>(
                "mouseCursor",
                mouseCursor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "fillColor",
                fillColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "checkColor",
                checkColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "overlayColor",
                overlayColor,
                defaultValue: null
            )
        );
        properties.add(new DoubleProperty("splashRadius", splashRadius, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<MaterialTapTargetSize>(
                "materialTapTargetSize",
                materialTapTargetSize,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<VisualDensity>(
                "visualDensity",
                visualDensity,
                defaultValue: null
            )
        );
        properties.add(new DiagnosticsProperty<OutlinedBorder>("shape", shape, defaultValue: null));
        properties.add(new DiagnosticsProperty<BorderSide>("side", side, defaultValue: null));
    }

    internal static BorderSide? _lerpSides(BorderSide? a, BorderSide? b, double t)
    {
        if ((a is null) && (b is null))
        {
            return null;
        }
        if (a is WidgetStateBorderSide)
        {
            a = ((WidgetStateBorderSide)a).resolve(new HashSet<WidgetState>());
        }
        if (b is WidgetStateBorderSide)
        {
            b = ((WidgetStateBorderSide)b).resolve(new HashSet<WidgetState>());
        }
        a ??= new BorderSide(width: 0, color: b!.color.withAlpha(0L));
        b ??= new BorderSide(width: 0, color: a.color.withAlpha(0L));
        return (BorderSide?)BorderSide.lerp(a, b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

public class CheckboxTheme : InheritedWidget
{
    public virtual CheckboxThemeData data { get; private set; } = default!;

    public CheckboxTheme(
        Key? key = null,
        CheckboxThemeData data = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static CheckboxThemeData of(BuildContext context)
    {
        CheckboxTheme? checkboxThemeLocal =
            context.dependOnInheritedWidgetOfExactType<CheckboxTheme>();
        return checkboxThemeLocal?.data ?? Theme.of(context).checkboxTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((CheckboxTheme)oldWidget).data));
}
