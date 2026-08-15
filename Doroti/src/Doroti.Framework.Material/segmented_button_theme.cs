// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/segmented_button_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class SegmentedButtonThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual ButtonStyle? style { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? selectedIcon { get; private set; }

    public SegmentedButtonThemeData(ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.Widget? selectedIcon = null)
    {
        this.style = style;
        this.selectedIcon = selectedIcon;
    }

    public virtual SegmentedButtonThemeData copyWith(ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.Widget? selectedIcon = null)
    {
        return new SegmentedButtonThemeData(style: (style ?? this.style), selectedIcon: (selectedIcon ?? this.selectedIcon));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SegmentedButtonThemeData lerp(SegmentedButtonThemeData? a, SegmentedButtonThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new SegmentedButtonThemeData(style: ButtonStyle.lerp(a?.style, b?.style, t), selectedIcon: ((t < 0.5) ? a?.selectedIcon : b?.selectedIcon));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.style, this.selectedIcon));
    public override bool Equals(object? other)
    {
        var __other = other as SegmentedButtonThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is SegmentedButtonThemeData) && (object.Equals(((SegmentedButtonThemeData)((SegmentedButtonThemeData)__other)).style, this.style))) && (object.Equals(((SegmentedButtonThemeData)((SegmentedButtonThemeData)__other)).selectedIcon, this.selectedIcon)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ButtonStyle>("style", this.style, defaultValue: null));
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

public class SegmentedButtonTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual SegmentedButtonThemeData data { get; private set; } = default!;

    public SegmentedButtonTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, SegmentedButtonThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SegmentedButtonThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return (SegmentedButtonTheme.maybeOf(context) ?? Theme.of(context).segmentedButtonTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SegmentedButtonThemeData? maybeOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<SegmentedButtonTheme>()?.data;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new SegmentedButtonTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((SegmentedButtonTheme)oldWidget).data)));
}
