// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/menu_theme.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class MenuThemeData : Diagnosticable
{
    public virtual MenuStyle? style { get; private set; }
    public virtual WidgetStateProperty<Widget?>? submenuIcon { get; private set; }

    public MenuThemeData(MenuStyle? style = null, WidgetStateProperty<Widget?>? submenuIcon = null)
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
        return new MenuThemeData(style: MenuStyle.lerp(a?.style, b?.style, t), submenuIcon: (t < 0.5) ? a?.submenuIcon : b?.submenuIcon);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(style, submenuIcon));
    public override bool Equals(object? other)
    {
        var __other = other as MenuThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is MenuThemeData) && Equals(__other.style, style) && Equals(__other.submenuIcon, submenuIcon);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DiagnosticsProperty<MenuStyle>("style", style, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Widget?>>("submenuIcon", submenuIcon, defaultValue: null));
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

public class MenuTheme : InheritedTheme
{
    public virtual MenuThemeData data { get; private set; } = default!;

    public MenuTheme(Key? key = null, MenuThemeData data = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static MenuThemeData of(BuildContext context)
    {
        MenuTheme? menuThemeLocal = context.dependOnInheritedWidgetOfExactType<MenuTheme>();
        return menuThemeLocal?.data ?? Theme.of(context).menuTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new MenuTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((MenuTheme)oldWidget).data));
}
