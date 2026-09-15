// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/menu_button_theme.dart
#pragma warning disable CS8600, CS8603
using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class MenuButtonThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual ButtonStyle? style { get; private set; }

    public MenuButtonThemeData(ButtonStyle? style = null)
    {
        this.style = style;
    }

    public static MenuButtonThemeData? lerp(MenuButtonThemeData? a, MenuButtonThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new MenuButtonThemeData(style: ButtonStyle.lerp(a?.style, b?.style, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => this.style?.GetHashCode() ?? 0;
    public override bool Equals(object? other)
    {
        var __other = other as MenuButtonThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is MenuButtonThemeData) && (object.Equals(((MenuButtonThemeData)((MenuButtonThemeData)__other)).style, this.style)));
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

public class MenuButtonTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual MenuButtonThemeData data { get; private set; } = default!;

    public MenuButtonTheme(global::Doroti.Framework.Foundation.Key? key = null, MenuButtonThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static MenuButtonThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        MenuButtonTheme? buttonTheme = ((MenuButtonTheme?)(object?)context.dependOnInheritedWidgetOfExactType<MenuButtonTheme>());
        return (buttonTheme?.data ?? Theme.of(context).menuButtonTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new MenuButtonTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((MenuButtonTheme)oldWidget).data)));
}
