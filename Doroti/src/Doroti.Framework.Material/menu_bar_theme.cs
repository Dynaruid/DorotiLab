// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/menu_bar_theme.dart
#pragma warning disable CS8600, CS8603
using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class MenuBarThemeData : MenuThemeData
{
    public MenuBarThemeData(MenuStyle? style = null) : base(style: style)
    {
    }

    public static MenuBarThemeData? lerp(MenuBarThemeData? a, MenuBarThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new MenuBarThemeData(style: MenuStyle.lerp(a?.style, b?.style, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MenuBarTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual MenuBarThemeData data { get; private set; } = default!;

    public MenuBarTheme(global::Doroti.Framework.Foundation.Key? key = null, MenuBarThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static MenuBarThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        MenuBarTheme? menuBarThemeLocal = ((MenuBarTheme?)(object?)context.dependOnInheritedWidgetOfExactType<MenuBarTheme>());
        return (menuBarThemeLocal?.data ?? Theme.of(context).menuBarTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new MenuBarTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((MenuBarTheme)oldWidget).data)));
}
