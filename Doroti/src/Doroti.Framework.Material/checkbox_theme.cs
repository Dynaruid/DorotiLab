// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/checkbox_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class CheckboxThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? checkColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? side { get; private set; }

    public CheckboxThemeData(global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? checkColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, MaterialTapTargetSize? materialTapTargetSize = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Framework.Painting.BorderSide? side = null)
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

    public virtual CheckboxThemeData copyWith(global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? checkColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, MaterialTapTargetSize? materialTapTargetSize = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Framework.Painting.BorderSide? side = null)
    {
        return new CheckboxThemeData(mouseCursor: mouseCursor ?? this.mouseCursor, fillColor: fillColor ?? this.fillColor, checkColor: checkColor ?? this.checkColor, overlayColor: overlayColor ?? this.overlayColor, splashRadius: splashRadius ?? this.splashRadius, materialTapTargetSize: materialTapTargetSize ?? this.materialTapTargetSize, visualDensity: visualDensity ?? this.visualDensity, shape: shape ?? this.shape, side: side ?? this.side);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CheckboxThemeData lerp(CheckboxThemeData? a, CheckboxThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new CheckboxThemeData(mouseCursor: (t < 0.5) ? a?.mouseCursor : b?.mouseCursor, fillColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.fillColor, b?.fillColor, t, Color.lerp), checkColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.checkColor, b?.checkColor, t, Color.lerp), overlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.overlayColor, b?.overlayColor, t, Color.lerp), splashRadius: Dart_uiLibrary.lerpDouble(a?.splashRadius, b?.splashRadius, t), materialTapTargetSize: (t < 0.5) ? a?.materialTapTargetSize : b?.materialTapTargetSize, visualDensity: (t < 0.5) ? a?.visualDensity : b?.visualDensity, shape: ((global::Doroti.Framework.Painting.OutlinedBorder?)ShapeBorder.lerp(a?.shape, b?.shape, t))!, side: _lerpSides(a?.side, b?.side, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(mouseCursor, fillColor, checkColor, overlayColor, splashRadius, materialTapTargetSize, visualDensity, shape, side));
    public override bool Equals(object? other)
    {
        var __other = other as CheckboxThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is CheckboxThemeData) && Equals(__other.mouseCursor, mouseCursor) && Equals(__other.fillColor, fillColor) && Equals(__other.checkColor, checkColor) && Equals(__other.overlayColor, overlayColor) && (__other.splashRadius == splashRadius) && Equals(__other.materialTapTargetSize, materialTapTargetSize) && Equals(__other.visualDensity, visualDensity) && Equals(__other.shape, shape) && Equals(__other.side, side);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>("mouseCursor", mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("fillColor", fillColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("checkColor", checkColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("overlayColor", overlayColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("splashRadius", splashRadius, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MaterialTapTargetSize>("materialTapTargetSize", materialTapTargetSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", visualDensity, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.OutlinedBorder>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderSide>("side", side, defaultValue: null));
    }

    internal static global::Doroti.Framework.Painting.BorderSide? _lerpSides(global::Doroti.Framework.Painting.BorderSide? a, global::Doroti.Framework.Painting.BorderSide? b, double t)
    {
        if ((a is null) && (b is null))
        {
            return null;
        }
        if (a is global::Doroti.Framework.Widgets.WidgetStateBorderSide)
        {
            a = ((global::Doroti.Framework.Widgets.WidgetStateBorderSide)a).resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>());
        }
        if (b is global::Doroti.Framework.Widgets.WidgetStateBorderSide)
        {
            b = ((global::Doroti.Framework.Widgets.WidgetStateBorderSide)b).resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>());
        }
        a ??= new global::Doroti.Framework.Painting.BorderSide(width: 0, color: b!.color.withAlpha(0L));
        b ??= new global::Doroti.Framework.Painting.BorderSide(width: 0, color: a.color.withAlpha(0L));
        return (global::Doroti.Framework.Painting.BorderSide?)BorderSide.lerp(a, b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

public class CheckboxTheme : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual CheckboxThemeData data { get; private set; } = default!;

    public CheckboxTheme(global::Doroti.Framework.Foundation.Key? key = null, CheckboxThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static CheckboxThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        CheckboxTheme? checkboxThemeLocal = context.dependOnInheritedWidgetOfExactType<CheckboxTheme>();
        return checkboxThemeLocal?.data ?? Theme.of(context).checkboxTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((CheckboxTheme)oldWidget).data));
}
