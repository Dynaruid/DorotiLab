// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/outlined_button_theme.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class OutlinedButtonThemeData : Diagnosticable
{
    public virtual ButtonStyle? style { get; private set; }

    public OutlinedButtonThemeData(ButtonStyle? style = null)
    {
        this.style = style;
    }

    public static OutlinedButtonThemeData? lerp(
        OutlinedButtonThemeData? a,
        OutlinedButtonThemeData? b,
        double t
    )
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new OutlinedButtonThemeData(style: ButtonStyle.lerp(a?.style, b?.style, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => style?.GetHashCode() ?? 0;

    public override bool Equals(object? other)
    {
        var __other = other as OutlinedButtonThemeData;
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
        return (__other is OutlinedButtonThemeData) && Equals(__other.style, style);
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

public class OutlinedButtonTheme : InheritedTheme
{
    public virtual OutlinedButtonThemeData data { get; private set; } = default!;

    public OutlinedButtonTheme(
        Key? key = null,
        OutlinedButtonThemeData data = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static OutlinedButtonThemeData of(BuildContext context)
    {
        OutlinedButtonTheme? buttonTheme =
            context.dependOnInheritedWidgetOfExactType<OutlinedButtonTheme>();
        return buttonTheme?.data ?? Theme.of(context).outlinedButtonTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new OutlinedButtonTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !Equals(data, ((OutlinedButtonTheme)oldWidget).data)
        );
}
