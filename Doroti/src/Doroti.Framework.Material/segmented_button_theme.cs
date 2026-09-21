// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/segmented_button_theme.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class SegmentedButtonThemeData : Diagnosticable
{
    public virtual ButtonStyle? style { get; private set; }
    public virtual Widget? selectedIcon { get; private set; }

    public SegmentedButtonThemeData(ButtonStyle? style = null, Widget? selectedIcon = null)
    {
        this.style = style;
        this.selectedIcon = selectedIcon;
    }

    public virtual SegmentedButtonThemeData copyWith(
        ButtonStyle? style = null,
        Widget? selectedIcon = null
    )
    {
        return new SegmentedButtonThemeData(
            style: style ?? this.style,
            selectedIcon: selectedIcon ?? this.selectedIcon
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static SegmentedButtonThemeData lerp(
        SegmentedButtonThemeData? a,
        SegmentedButtonThemeData? b,
        double t
    )
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new SegmentedButtonThemeData(
            style: ButtonStyle.lerp(a?.style, b?.style, t),
            selectedIcon: (t < 0.5) ? a?.selectedIcon : b?.selectedIcon
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(style, selectedIcon)
        );

    public override bool Equals(object? other)
    {
        var __other = other as SegmentedButtonThemeData;
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
        return (__other is SegmentedButtonThemeData)
            && Equals(__other.style, style)
            && Equals(__other.selectedIcon, selectedIcon);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DiagnosticsProperty<ButtonStyle>("style", style, defaultValue: null));
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

public class SegmentedButtonTheme : InheritedTheme
{
    public virtual SegmentedButtonThemeData data { get; private set; } = default!;

    public SegmentedButtonTheme(
        Key? key = null,
        SegmentedButtonThemeData data = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SegmentedButtonThemeData of(BuildContext context)
    {
        return maybeOf(context) ?? Theme.of(context).segmentedButtonTheme;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static SegmentedButtonThemeData? maybeOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<SegmentedButtonTheme>()?.data;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new SegmentedButtonTheme(data: data, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !Equals(data, ((SegmentedButtonTheme)oldWidget).data)
        );
}
