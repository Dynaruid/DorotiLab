// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_button_theme.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class TextButtonThemeData : Diagnosticable
{
    public virtual ButtonStyle? style { get; private set; }

    public TextButtonThemeData(ButtonStyle? style = null)
    {
        this.style = style;
    }

    public static TextButtonThemeData? lerp(TextButtonThemeData? a, TextButtonThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new TextButtonThemeData(style: ButtonStyle.lerp(a?.style, b?.style, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => style?.GetHashCode() ?? 0;
    public override bool Equals(object? other)
    {
        var __other = other as TextButtonThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is TextButtonThemeData) && Equals(__other.style, style);
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

public class TextButtonTheme : InheritedTheme
{
    public virtual TextButtonThemeData data { get; private set; } = default!;

    public TextButtonTheme(Key? key = null, TextButtonThemeData data = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static TextButtonThemeData of(BuildContext context)
    {
        TextButtonTheme? buttonTheme = context.dependOnInheritedWidgetOfExactType<TextButtonTheme>();
        return buttonTheme?.data ?? Theme.of(context).textButtonTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new TextButtonTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((TextButtonTheme)oldWidget).data));
}
