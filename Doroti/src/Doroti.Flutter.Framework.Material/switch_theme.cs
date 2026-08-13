// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/switch_theme.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class SwitchThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.Icon?>? thumbIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }

    public SwitchThemeData(global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.Icon?>? thumbIcon = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null)
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

    public virtual SwitchThemeData copyWith(global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.Icon?>? thumbIcon = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null)
    {
        return new SwitchThemeData(thumbColor: (thumbColor ?? this.thumbColor), trackColor: (trackColor ?? this.trackColor), trackOutlineColor: (trackOutlineColor ?? this.trackOutlineColor), trackOutlineWidth: (trackOutlineWidth ?? this.trackOutlineWidth), materialTapTargetSize: (materialTapTargetSize ?? this.materialTapTargetSize), mouseCursor: (mouseCursor ?? this.mouseCursor), overlayColor: (overlayColor ?? this.overlayColor), splashRadius: (splashRadius ?? this.splashRadius), thumbIcon: (thumbIcon ?? this.thumbIcon), padding: (padding ?? this.padding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SwitchThemeData lerp(SwitchThemeData? a, SwitchThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new SwitchThemeData(thumbColor: WidgetStateProperty.lerp<global::Doroti.Flutter.Ui.Color?>(a?.thumbColor, b?.thumbColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), trackColor: WidgetStateProperty.lerp<global::Doroti.Flutter.Ui.Color?>(a?.trackColor, b?.trackColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), trackOutlineColor: WidgetStateProperty.lerp<global::Doroti.Flutter.Ui.Color?>(a?.trackOutlineColor, b?.trackOutlineColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), trackOutlineWidth: WidgetStateProperty.lerp<double?>(a?.trackOutlineWidth, b?.trackOutlineWidth, t, (global::System.Func<double?, double?, double, double?>)Dart_uiLibrary.lerpDouble), materialTapTargetSize: ((t < 0.5) ? a?.materialTapTargetSize : b?.materialTapTargetSize), mouseCursor: ((t < 0.5) ? a?.mouseCursor : b?.mouseCursor), overlayColor: WidgetStateProperty.lerp<global::Doroti.Flutter.Ui.Color?>(a?.overlayColor, b?.overlayColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), splashRadius: Dart_uiLibrary.lerpDouble(a?.splashRadius, b?.splashRadius, t), thumbIcon: ((t < 0.5) ? a?.thumbIcon : b?.thumbIcon), padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.thumbColor, this.trackColor, this.trackOutlineColor, this.trackOutlineWidth, this.materialTapTargetSize, this.mouseCursor, this.overlayColor, this.splashRadius, this.thumbIcon, this.padding));
    public override bool Equals(object? other)
    {
        var __other = other as SwitchThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((__other is SwitchThemeData) && (object.Equals(((SwitchThemeData)((SwitchThemeData)__other)).thumbColor, this.thumbColor))) && (object.Equals(((SwitchThemeData)((SwitchThemeData)__other)).trackColor, this.trackColor))) && (object.Equals(((SwitchThemeData)((SwitchThemeData)__other)).trackOutlineColor, this.trackOutlineColor))) && (object.Equals(((SwitchThemeData)((SwitchThemeData)__other)).trackOutlineWidth, this.trackOutlineWidth))) && (object.Equals(((SwitchThemeData)((SwitchThemeData)__other)).materialTapTargetSize, this.materialTapTargetSize))) && (object.Equals(((SwitchThemeData)((SwitchThemeData)__other)).mouseCursor, this.mouseCursor))) && (object.Equals(((SwitchThemeData)((SwitchThemeData)__other)).overlayColor, this.overlayColor))) && (((SwitchThemeData)((SwitchThemeData)__other)).splashRadius == this.splashRadius)) && (object.Equals(((SwitchThemeData)((SwitchThemeData)__other)).thumbIcon, this.thumbIcon))) && (object.Equals(((SwitchThemeData)((SwitchThemeData)__other)).padding, this.padding)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>("thumbColor", this.thumbColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>("trackColor", this.trackColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>("trackOutlineColor", this.trackOutlineColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>>("trackOutlineWidth", this.trackOutlineWidth, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<MaterialTapTargetSize>("materialTapTargetSize", this.materialTapTargetSize, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>("mouseCursor", this.mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>("overlayColor", this.overlayColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("splashRadius", this.splashRadius, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.Icon?>>("thumbIcon", this.thumbIcon, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
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

public class SwitchTheme : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual SwitchThemeData data { get; private set; } = default!;

    public SwitchTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, SwitchThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SwitchThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        SwitchTheme? switchTheme__9535 = ((SwitchTheme?)(object?)context.dependOnInheritedWidgetOfExactType<SwitchTheme>());
        return (switchTheme__9535?.data ?? Theme.of(context).switchTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((SwitchTheme)oldWidget).data)));
}
