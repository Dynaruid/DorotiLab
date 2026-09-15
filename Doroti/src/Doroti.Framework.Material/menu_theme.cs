// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/menu_theme.dart
#pragma warning disable CS8600, CS8603
using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class MenuThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual MenuStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Widget?>? submenuIcon { get; private set; }

    public MenuThemeData(MenuStyle? style = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Widget?>? submenuIcon = null)
    {
        this.style = style;
        this.submenuIcon = submenuIcon;
    }

    public static MenuThemeData? lerp(MenuThemeData? a, MenuThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new MenuThemeData(style: MenuStyle.lerp(a?.style, b?.style, t), submenuIcon: ((t < 0.5) ? a?.submenuIcon : b?.submenuIcon));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.style, this.submenuIcon));
    public override bool Equals(object? other)
    {
        var __other = other as MenuThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is MenuThemeData) && (object.Equals(((MenuThemeData)((MenuThemeData)__other)).style, this.style))) && (object.Equals(((MenuThemeData)((MenuThemeData)__other)).submenuIcon, this.submenuIcon)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MenuStyle>("style", this.style, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Widget?>>("submenuIcon", this.submenuIcon, defaultValue: null));
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

public class MenuTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual MenuThemeData data { get; private set; } = default!;

    public MenuTheme(global::Doroti.Framework.Foundation.Key? key = null, MenuThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static MenuThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        MenuTheme? menuThemeLocal = ((MenuTheme?)(object?)context.dependOnInheritedWidgetOfExactType<MenuTheme>());
        return (menuThemeLocal?.data ?? Theme.of(context).menuTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new MenuTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((MenuTheme)oldWidget).data)));
}
