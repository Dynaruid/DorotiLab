// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/elevated_button_theme.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class ElevatedButtonThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual ButtonStyle? style { get; private set; }

    public ElevatedButtonThemeData(ButtonStyle? style = null)
    {
        this.style = style;
    }

    public static ElevatedButtonThemeData? lerp(ElevatedButtonThemeData? a, ElevatedButtonThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ElevatedButtonThemeData(style: ButtonStyle.lerp(a?.style, b?.style, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => this.style?.GetHashCode() ?? 0;
    public override bool Equals(object? other)
    {
        var __other = other as ElevatedButtonThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is ElevatedButtonThemeData) && (Equals(((ElevatedButtonThemeData)((ElevatedButtonThemeData)__other)).style, this.style)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonStyle>("style", this.style, defaultValue: null));
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
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ElevatedButtonTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual ElevatedButtonThemeData data { get; private set; } = default!;

    public ElevatedButtonTheme(global::Doroti.Framework.Foundation.Key? key = null, ElevatedButtonThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ElevatedButtonThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ElevatedButtonTheme? buttonTheme = ((ElevatedButtonTheme?)context.dependOnInheritedWidgetOfExactType<ElevatedButtonTheme>());
        return (buttonTheme?.data ?? Theme.of(context).elevatedButtonTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)new ElevatedButtonTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!Equals(this.data, ((ElevatedButtonTheme)oldWidget).data)));
}
