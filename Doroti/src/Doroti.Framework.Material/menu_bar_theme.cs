// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/menu_bar_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

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

public class MenuBarTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual MenuBarThemeData data { get; private set; } = default!;

    public MenuBarTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, MenuBarThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static MenuBarThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        MenuBarTheme? menuBarTheme__3582 = ((MenuBarTheme?)(object?)context.dependOnInheritedWidgetOfExactType<MenuBarTheme>());
        return (menuBarTheme__3582?.data ?? Theme.of(context).menuBarTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new MenuBarTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((MenuBarTheme)oldWidget).data)));
}
