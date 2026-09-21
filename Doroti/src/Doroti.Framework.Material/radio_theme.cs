// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/radio_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class RadioThemeData : Diagnosticable
{
    public virtual WidgetStateProperty<MouseCursor?>? mouseCursor { get; private set; }
    public virtual WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual WidgetStateProperty<Color?>? backgroundColor { get; private set; }
    public virtual BorderSide? side { get; private set; }
    public virtual WidgetStateProperty<double?>? innerRadius { get; private set; }

    public RadioThemeData(
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        WidgetStateProperty<Color?>? fillColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        double? splashRadius = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        VisualDensity? visualDensity = null,
        WidgetStateProperty<Color?>? backgroundColor = null,
        BorderSide? side = null,
        WidgetStateProperty<double?>? innerRadius = null
    )
    {
        this.mouseCursor = mouseCursor;
        this.fillColor = fillColor;
        this.overlayColor = overlayColor;
        this.splashRadius = splashRadius;
        this.materialTapTargetSize = materialTapTargetSize;
        this.visualDensity = visualDensity;
        this.backgroundColor = backgroundColor;
        this.side = side;
        this.innerRadius = innerRadius;
    }

    public virtual RadioThemeData copyWith(
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        WidgetStateProperty<Color?>? fillColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        double? splashRadius = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        VisualDensity? visualDensity = null,
        WidgetStateProperty<Color?>? backgroundColor = null,
        BorderSide? side = null,
        WidgetStateProperty<double?>? innerRadius = null
    )
    {
        return new RadioThemeData(
            mouseCursor: mouseCursor ?? this.mouseCursor,
            fillColor: fillColor ?? this.fillColor,
            overlayColor: overlayColor ?? this.overlayColor,
            splashRadius: splashRadius ?? this.splashRadius,
            materialTapTargetSize: materialTapTargetSize ?? this.materialTapTargetSize,
            visualDensity: visualDensity ?? this.visualDensity,
            backgroundColor: backgroundColor ?? this.backgroundColor,
            side: side ?? this.side,
            innerRadius: innerRadius ?? this.innerRadius
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static RadioThemeData lerp(RadioThemeData? a, RadioThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new RadioThemeData(
            mouseCursor: (t < 0.5) ? a?.mouseCursor : b?.mouseCursor,
            fillColor: WidgetStateProperty.lerp(a?.fillColor, b?.fillColor, t, Color.lerp),
            materialTapTargetSize: (t < 0.5) ? a?.materialTapTargetSize : b?.materialTapTargetSize,
            overlayColor: WidgetStateProperty.lerp(a?.overlayColor, b?.overlayColor, t, Color.lerp),
            splashRadius: DorotiUiLibrary.lerpDouble(a?.splashRadius, b?.splashRadius, t),
            visualDensity: (t < 0.5) ? a?.visualDensity : b?.visualDensity,
            backgroundColor: WidgetStateProperty.lerp(
                a?.backgroundColor,
                b?.backgroundColor,
                t,
                Color.lerp
            ),
            side: _lerpSides(a?.side, b?.side, t),
            innerRadius: WidgetStateProperty.lerp(
                a?.innerRadius,
                b?.innerRadius,
                t,
                DorotiUiLibrary.lerpDouble
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                mouseCursor,
                fillColor,
                overlayColor,
                splashRadius,
                materialTapTargetSize,
                visualDensity,
                backgroundColor,
                side,
                innerRadius
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as RadioThemeData;
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
        return (__other is RadioThemeData)
            && Equals(__other.mouseCursor, mouseCursor)
            && Equals(__other.fillColor, fillColor)
            && Equals(__other.overlayColor, overlayColor)
            && (__other.splashRadius == splashRadius)
            && Equals(__other.materialTapTargetSize, materialTapTargetSize)
            && Equals(__other.visualDensity, visualDensity)
            && Equals(__other.backgroundColor, backgroundColor)
            && Equals(__other.side, side)
            && Equals(__other.innerRadius, innerRadius);
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
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "backgroundColor",
                backgroundColor,
                defaultValue: null
            )
        );
        properties.add(new DiagnosticsProperty<BorderSide>("side", side, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<double?>>(
                "innerRadius",
                innerRadius,
                defaultValue: null
            )
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

public class RadioTheme : InheritedWidget
{
    public virtual RadioThemeData data { get; private set; } = default!;

    public RadioTheme(Key? key = null, RadioThemeData data = default!, Widget child = default!)
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static RadioThemeData of(BuildContext context)
    {
        RadioTheme? radioThemeLocal = context.dependOnInheritedWidgetOfExactType<RadioTheme>();
        return radioThemeLocal?.data ?? Theme.of(context).radioTheme;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((RadioTheme)oldWidget).data));
}
