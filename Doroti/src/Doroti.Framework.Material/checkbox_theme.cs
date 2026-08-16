// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/checkbox_theme.dart
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
        return new CheckboxThemeData(mouseCursor: (mouseCursor ?? this.mouseCursor), fillColor: (fillColor ?? this.fillColor), checkColor: (checkColor ?? this.checkColor), overlayColor: (overlayColor ?? this.overlayColor), splashRadius: (splashRadius ?? this.splashRadius), materialTapTargetSize: (materialTapTargetSize ?? this.materialTapTargetSize), visualDensity: (visualDensity ?? this.visualDensity), shape: (shape ?? this.shape), side: (side ?? this.side));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CheckboxThemeData lerp(CheckboxThemeData? a, CheckboxThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new CheckboxThemeData(mouseCursor: ((t < 0.5) ? a?.mouseCursor : b?.mouseCursor), fillColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.fillColor, b?.fillColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), checkColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.checkColor, b?.checkColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), overlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.overlayColor, b?.overlayColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), splashRadius: Dart_uiLibrary.lerpDouble(a?.splashRadius, b?.splashRadius, t), materialTapTargetSize: ((t < 0.5) ? a?.materialTapTargetSize : b?.materialTapTargetSize), visualDensity: ((t < 0.5) ? a?.visualDensity : b?.visualDensity), shape: ((global::Doroti.Framework.Painting.OutlinedBorder?)(object?)ShapeBorder.lerp(a?.shape, b?.shape, t))!, side: CheckboxThemeData._lerpSides(a?.side, b?.side, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.mouseCursor, this.fillColor, this.checkColor, this.overlayColor, this.splashRadius, this.materialTapTargetSize, this.visualDensity, this.shape, this.side));
    public override bool Equals(object? other)
    {
        var __other = other as CheckboxThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((__other is CheckboxThemeData) && (object.Equals(((CheckboxThemeData)((CheckboxThemeData)__other)).mouseCursor, this.mouseCursor))) && (object.Equals(((CheckboxThemeData)((CheckboxThemeData)__other)).fillColor, this.fillColor))) && (object.Equals(((CheckboxThemeData)((CheckboxThemeData)__other)).checkColor, this.checkColor))) && (object.Equals(((CheckboxThemeData)((CheckboxThemeData)__other)).overlayColor, this.overlayColor))) && (((CheckboxThemeData)((CheckboxThemeData)__other)).splashRadius == this.splashRadius)) && (object.Equals(((CheckboxThemeData)((CheckboxThemeData)__other)).materialTapTargetSize, this.materialTapTargetSize))) && (object.Equals(((CheckboxThemeData)((CheckboxThemeData)__other)).visualDensity, this.visualDensity))) && (object.Equals(((CheckboxThemeData)((CheckboxThemeData)__other)).shape, this.shape))) && (object.Equals(((CheckboxThemeData)((CheckboxThemeData)__other)).side, this.side)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>("mouseCursor", this.mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("fillColor", this.fillColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("checkColor", this.checkColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("overlayColor", this.overlayColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("splashRadius", this.splashRadius, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MaterialTapTargetSize>("materialTapTargetSize", this.materialTapTargetSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", this.visualDensity, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.OutlinedBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderSide>("side", this.side, defaultValue: null));
    }

    internal static global::Doroti.Framework.Painting.BorderSide? _lerpSides(global::Doroti.Framework.Painting.BorderSide? a, global::Doroti.Framework.Painting.BorderSide? b, double t)
    {
        if (((a is null) && (b is null)))
        {
            return null;
        }
        if ((a is global::Doroti.Framework.Widgets.WidgetStateBorderSide))
        {
            a = ((global::Doroti.Framework.Widgets.WidgetStateBorderSide)a).resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>());
        }
        if ((b is global::Doroti.Framework.Widgets.WidgetStateBorderSide))
        {
            b = ((global::Doroti.Framework.Widgets.WidgetStateBorderSide)b).resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>());
        }
        a ??= new global::Doroti.Framework.Painting.BorderSide(width: 0, color: b!.color.withAlpha(0L));
        b ??= new global::Doroti.Framework.Painting.BorderSide(width: 0, color: ((global::Doroti.Framework.Painting.BorderSide)a).color.withAlpha(0L));
        return ((global::Doroti.Framework.Painting.BorderSide?)(object?)BorderSide.lerp(a, b, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

public class CheckboxTheme : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual CheckboxThemeData data { get; private set; } = default!;

    public CheckboxTheme(global::Doroti.Framework.Foundation.Key? key = null, CheckboxThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static CheckboxThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        CheckboxTheme? checkboxTheme__9288 = ((CheckboxTheme?)(object?)context.dependOnInheritedWidgetOfExactType<CheckboxTheme>());
        return (checkboxTheme__9288?.data ?? Theme.of(context).checkboxTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((CheckboxTheme)oldWidget).data)));
}
