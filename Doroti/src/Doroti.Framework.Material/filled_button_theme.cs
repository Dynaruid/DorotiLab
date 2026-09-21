// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/filled_button_theme.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class FilledButtonThemeData : Diagnosticable
{
    public virtual ButtonStyle? style { get; private set; }

    public FilledButtonThemeData(ButtonStyle? style = null)
    {
        this.style = style;
    }

    public static FilledButtonThemeData? lerp(
        FilledButtonThemeData? a,
        FilledButtonThemeData? b,
        double t
    )
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new FilledButtonThemeData(style: ButtonStyle.lerp(a?.style, b?.style, t));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() => style?.GetHashCode() ?? 0;

    public override bool Equals(object? other)
    {
        var __other = other as FilledButtonThemeData;
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
        return (__other is FilledButtonThemeData) && Equals(__other.style, style);
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

public class FilledButtonTheme : InheritedTheme
{
    public virtual FilledButtonThemeData data { get; private set; } = default!;

    public FilledButtonTheme(
        Key? key = null,
        FilledButtonThemeData data = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static FilledButtonThemeData of(BuildContext context)
    {
        FilledButtonTheme? buttonTheme =
            context.dependOnInheritedWidgetOfExactType<FilledButtonTheme>();
        return buttonTheme?.data ?? Theme.of(context).filledButtonTheme;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new FilledButtonTheme(data: data, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !Equals(data, ((FilledButtonTheme)oldWidget).data)
        );
}
