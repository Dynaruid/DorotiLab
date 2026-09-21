// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/menu_bar_theme.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class MenuBarThemeData : MenuThemeData
{
    public MenuBarThemeData(MenuStyle? style = null)
        : base(style: style) { }

    public static MenuBarThemeData? lerp(MenuBarThemeData? a, MenuBarThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new MenuBarThemeData(style: MenuStyle.lerp(a?.style, b?.style, t));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class MenuBarTheme : InheritedTheme
{
    public virtual MenuBarThemeData data { get; private set; } = default!;

    public MenuBarTheme(Key? key = null, MenuBarThemeData data = default!, Widget child = default!)
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static MenuBarThemeData of(BuildContext context)
    {
        MenuBarTheme? menuBarThemeLocal =
            context.dependOnInheritedWidgetOfExactType<MenuBarTheme>();
        return menuBarThemeLocal?.data ?? Theme.of(context).menuBarTheme;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new MenuBarTheme(data: data, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((MenuBarTheme)oldWidget).data));
}
