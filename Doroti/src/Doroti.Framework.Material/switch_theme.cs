// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/switch_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class SwitchThemeData : Diagnosticable
{
    public virtual WidgetStateProperty<Color?>? thumbColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? trackColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? trackOutlineColor { get; private set; }
    public virtual WidgetStateProperty<double?>? trackOutlineWidth { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual WidgetStateProperty<MouseCursor?>? mouseCursor { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual WidgetStateProperty<Icon?>? thumbIcon { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }

    public SwitchThemeData(
        WidgetStateProperty<Color?>? thumbColor = null,
        WidgetStateProperty<Color?>? trackColor = null,
        WidgetStateProperty<Color?>? trackOutlineColor = null,
        WidgetStateProperty<double?>? trackOutlineWidth = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        double? splashRadius = null,
        WidgetStateProperty<Icon?>? thumbIcon = null,
        EdgeInsetsGeometry? padding = null
    )
    {
        this.thumbColor = thumbColor;
        this.trackColor = trackColor;
        this.trackOutlineColor = trackOutlineColor;
        this.trackOutlineWidth = trackOutlineWidth;
        this.materialTapTargetSize = materialTapTargetSize;
        this.mouseCursor = mouseCursor;
        this.overlayColor = overlayColor;
        this.splashRadius = splashRadius;
        this.thumbIcon = thumbIcon;
        this.padding = padding;
    }

    public virtual SwitchThemeData copyWith(
        WidgetStateProperty<Color?>? thumbColor = null,
        WidgetStateProperty<Color?>? trackColor = null,
        WidgetStateProperty<Color?>? trackOutlineColor = null,
        WidgetStateProperty<double?>? trackOutlineWidth = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        double? splashRadius = null,
        WidgetStateProperty<Icon?>? thumbIcon = null,
        EdgeInsetsGeometry? padding = null
    )
    {
        return new SwitchThemeData(
            thumbColor: thumbColor ?? this.thumbColor,
            trackColor: trackColor ?? this.trackColor,
            trackOutlineColor: trackOutlineColor ?? this.trackOutlineColor,
            trackOutlineWidth: trackOutlineWidth ?? this.trackOutlineWidth,
            materialTapTargetSize: materialTapTargetSize ?? this.materialTapTargetSize,
            mouseCursor: mouseCursor ?? this.mouseCursor,
            overlayColor: overlayColor ?? this.overlayColor,
            splashRadius: splashRadius ?? this.splashRadius,
            thumbIcon: thumbIcon ?? this.thumbIcon,
            padding: padding ?? this.padding
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static SwitchThemeData lerp(SwitchThemeData? a, SwitchThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new SwitchThemeData(
            thumbColor: WidgetStateProperty.lerp(a?.thumbColor, b?.thumbColor, t, Color.lerp),
            trackColor: WidgetStateProperty.lerp(a?.trackColor, b?.trackColor, t, Color.lerp),
            trackOutlineColor: WidgetStateProperty.lerp(
                a?.trackOutlineColor,
                b?.trackOutlineColor,
                t,
                Color.lerp
            ),
            trackOutlineWidth: WidgetStateProperty.lerp(
                a?.trackOutlineWidth,
                b?.trackOutlineWidth,
                t,
                DorotiUiLibrary.lerpDouble
            ),
            materialTapTargetSize: (t < 0.5) ? a?.materialTapTargetSize : b?.materialTapTargetSize,
            mouseCursor: (t < 0.5) ? a?.mouseCursor : b?.mouseCursor,
            overlayColor: WidgetStateProperty.lerp(a?.overlayColor, b?.overlayColor, t, Color.lerp),
            splashRadius: DorotiUiLibrary.lerpDouble(a?.splashRadius, b?.splashRadius, t),
            thumbIcon: (t < 0.5) ? a?.thumbIcon : b?.thumbIcon,
            padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                thumbColor,
                trackColor,
                trackOutlineColor,
                trackOutlineWidth,
                materialTapTargetSize,
                mouseCursor,
                overlayColor,
                splashRadius,
                thumbIcon,
                padding
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as SwitchThemeData;
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
        return (__other is SwitchThemeData)
            && Equals(__other.thumbColor, thumbColor)
            && Equals(__other.trackColor, trackColor)
            && Equals(__other.trackOutlineColor, trackOutlineColor)
            && Equals(__other.trackOutlineWidth, trackOutlineWidth)
            && Equals(__other.materialTapTargetSize, materialTapTargetSize)
            && Equals(__other.mouseCursor, mouseCursor)
            && Equals(__other.overlayColor, overlayColor)
            && (__other.splashRadius == splashRadius)
            && Equals(__other.thumbIcon, thumbIcon)
            && Equals(__other.padding, padding);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "thumbColor",
                thumbColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "trackColor",
                trackColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "trackOutlineColor",
                trackOutlineColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<double?>>(
                "trackOutlineWidth",
                trackOutlineWidth,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<MaterialTapTargetSize>(
                "materialTapTargetSize",
                materialTapTargetSize,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<MouseCursor?>>(
                "mouseCursor",
                mouseCursor,
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
            new DiagnosticsProperty<WidgetStateProperty<Icon?>>(
                "thumbIcon",
                thumbIcon,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding, defaultValue: null)
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

public class SwitchTheme : InheritedWidget
{
    public virtual SwitchThemeData data { get; private set; } = default!;

    public SwitchTheme(Key? key = null, SwitchThemeData data = default!, Widget child = default!)
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SwitchThemeData of(BuildContext context)
    {
        SwitchTheme? switchThemeLocal = context.dependOnInheritedWidgetOfExactType<SwitchTheme>();
        return switchThemeLocal?.data ?? Theme.of(context).switchTheme;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((SwitchTheme)oldWidget).data));
}
