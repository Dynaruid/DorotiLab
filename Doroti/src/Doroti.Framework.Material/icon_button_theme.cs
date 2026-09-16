// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/icon_button_theme.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class IconButtonThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual ButtonStyle? style { get; private set; }

    public IconButtonThemeData(ButtonStyle? style = null)
    {
        this.style = style;
    }

    public static IconButtonThemeData? lerp(IconButtonThemeData? a, IconButtonThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new IconButtonThemeData(style: ButtonStyle.lerp(a?.style, b?.style, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => style?.GetHashCode() ?? 0;
    public override bool Equals(object? other)
    {
        var __other = other as IconButtonThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is IconButtonThemeData) && Equals(__other.style, style);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonStyle>("style", style, defaultValue: null));
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

public class IconButtonTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual IconButtonThemeData data { get; private set; } = default!;

    public IconButtonTheme(global::Doroti.Framework.Foundation.Key? key = null, IconButtonThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static IconButtonThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        IconButtonTheme? buttonTheme = context.dependOnInheritedWidgetOfExactType<IconButtonTheme>();
        return buttonTheme?.data ?? Theme.of(context).iconButtonTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return new IconButtonTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((IconButtonTheme)oldWidget).data));
}
