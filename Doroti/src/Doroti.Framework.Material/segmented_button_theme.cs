// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/segmented_button_theme.dart
#pragma warning disable CS8600, CS8603
using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class SegmentedButtonThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual ButtonStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? selectedIcon { get; private set; }

    public SegmentedButtonThemeData(ButtonStyle? style = null, global::Doroti.Framework.Widgets.Widget? selectedIcon = null)
    {
        this.style = style;
        this.selectedIcon = selectedIcon;
    }

    public virtual SegmentedButtonThemeData copyWith(ButtonStyle? style = null, global::Doroti.Framework.Widgets.Widget? selectedIcon = null)
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

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonStyle>("style", this.style, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(global::Doroti.Framework.Foundation.DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SegmentedButtonTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual SegmentedButtonThemeData data { get; private set; } = default!;

    public SegmentedButtonTheme(global::Doroti.Framework.Foundation.Key? key = null, SegmentedButtonThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SegmentedButtonThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return (SegmentedButtonTheme.maybeOf(context) ?? Theme.of(context).segmentedButtonTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SegmentedButtonThemeData? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<SegmentedButtonTheme>()?.data;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new SegmentedButtonTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((SegmentedButtonTheme)oldWidget).data)));
}
