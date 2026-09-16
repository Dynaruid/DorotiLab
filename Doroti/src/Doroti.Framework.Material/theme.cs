// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class ThemeLibrary
{
    public static Duration kThemeAnimationDuration = Duration.Create(milliseconds: 200L);
}

public class Theme : StatelessWidget
{
    public virtual ThemeData data { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    internal static ThemeData _kFallbackTheme = ThemeData.CreateFallback();

    public Theme(Key? key = null, ThemeData data = default!, Widget child = default!) : base(key: key)
    {
        this.data = data;
        this.child = child;
    }

    public static ThemeData of(BuildContext context)
    {
        _InheritedTheme__theme? inheritedTheme = context.dependOnInheritedWidgetOfExactType<_InheritedTheme__theme>();
        MaterialLocalizations? localizations = Localizations.of<MaterialLocalizations>(context, typeof(MaterialLocalizations));
        ScriptCategory category = localizations?.scriptCategory ?? ScriptCategory.englishLike;
        InheritedCupertinoTheme? inheritedCupertinoTheme = context.dependOnInheritedWidgetOfExactType<InheritedCupertinoTheme>();
        ThemeData themeLocal = inheritedTheme?.theme.data ?? ((inheritedCupertinoTheme is not null) ? new CupertinoBasedMaterialThemeData(themeData: inheritedCupertinoTheme.theme.data).materialTheme : _kFallbackTheme);
        return ThemeData.localize(themeLocal, themeLocal.typography.geometryThemeFor(category));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _wrapsWidgetThemes(BuildContext context, Widget child)
    {
        DefaultSelectionStyle selectionStyle = DefaultSelectionStyle.of(context);
        return new IconTheme(data: data.iconTheme, child: new DefaultSelectionStyle(selectionColor: data.textSelectionTheme.selectionColor ?? selectionStyle.selectionColor, cursorColor: data.textSelectionTheme.cursorColor ?? selectionStyle.cursorColor, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual CupertinoThemeData _inheritedCupertinoThemeData(BuildContext context)
    {
        InheritedCupertinoTheme? inheritedTheme = context.dependOnInheritedWidgetOfExactType<InheritedCupertinoTheme>();
        return (inheritedTheme?.theme.data ?? new MaterialBasedCupertinoThemeData(materialTheme: data)).resolveFrom(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Brightness brightnessOf(BuildContext context)
    {
        _InheritedTheme__theme? inheritedTheme = context.dependOnInheritedWidgetOfExactType<_InheritedTheme__theme>();
        return inheritedTheme?.theme.data.brightness ?? MediaQuery.platformBrightnessOf(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Brightness? maybeBrightnessOf(BuildContext context)
    {
        _InheritedTheme__theme? inheritedTheme = context.dependOnInheritedWidgetOfExactType<_InheritedTheme__theme>();
        return inheritedTheme?.theme.data.brightness ?? MediaQuery.maybePlatformBrightnessOf(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new _InheritedTheme__theme(theme: this, child: new CupertinoTheme(data: _inheritedCupertinoThemeData(context), child: _wrapsWidgetThemes(context, child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<ThemeData>("data", data, showName: false));
    }

}

internal class _InheritedTheme__theme : InheritedTheme
{
    public virtual Theme theme { get; private set; } = default!;

    internal _InheritedTheme__theme(Theme theme, Widget child) : base(child: child)
    {
        this.theme = theme;
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new Theme(data: theme.data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => !Equals(theme.data, ((_InheritedTheme__theme)oldWidget).theme.data);
}

public class ThemeDataTween : Tween<ThemeData>
{
    public ThemeDataTween(ThemeData? begin = null, ThemeData? end = null) : base(begin: begin, end: end)
    {
    }

    public override ThemeData lerp(double t) => ThemeData.lerp(begin!, end!, t);
}

public class AnimatedTheme : ImplicitlyAnimatedWidget
{
    public virtual ThemeData data { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public AnimatedTheme(Key? key = null, ThemeData data = default!, Curve curve = default!, Duration? duration = null, Action? onEnd = null, Widget child = default!) : base(key: key, curve: curve ?? Curves.linear, duration: duration ?? ThemeLibrary.kThemeAnimationDuration, onEnd: onEnd)
    {
        this.data = data;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AnimatedThemeState__theme());
}

internal class _AnimatedThemeState__theme : AnimatedWidgetBaseState<AnimatedTheme>
{
    internal virtual ThemeDataTween? _data { get; set; } = default;

    public override void forEachTween(Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor)
    {
        _data = ((ThemeDataTween?)visitor(_data, widget.data, (value) => new ThemeDataTween(begin: ((ThemeData?)value)!))!)!;
    }

    public override Widget build(BuildContext context)
    {
        return new Theme(data: _data!.evaluate(animation), child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new DiagnosticsProperty<ThemeDataTween>("data", _data, showName: false, defaultValue: null));
    }

}
