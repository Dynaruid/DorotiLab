// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public static partial class ThemeLibrary
{
    public static Duration kThemeAnimationDuration = Duration.Create(milliseconds: 200L);
}

public class Theme : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual ThemeData data { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    internal static ThemeData _kFallbackTheme = ThemeData.CreateFallback();

    public Theme(global::Doroti.Framework.Foundation.Key? key = null, ThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.data = data;
        this.child = child;
    }

    public static ThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _InheritedTheme__theme? inheritedTheme__4799 = ((_InheritedTheme__theme?)(object?)context.dependOnInheritedWidgetOfExactType<_InheritedTheme__theme>());
        MaterialLocalizations? localizations__4921 = ((MaterialLocalizations?)(object?)Localizations.of<MaterialLocalizations>(context, typeof(MaterialLocalizations)));
        ScriptCategory category__5054 = (localizations__4921?.scriptCategory ?? ScriptCategory.englishLike);
        InheritedCupertinoTheme? inheritedCupertinoTheme__5161 = ((InheritedCupertinoTheme?)(object?)context.dependOnInheritedWidgetOfExactType<InheritedCupertinoTheme>());
        ThemeData theme__5287 = (inheritedTheme__4799?.theme.data ?? (((inheritedCupertinoTheme__5161 is not null) ? new CupertinoBasedMaterialThemeData(themeData: inheritedCupertinoTheme__5161.theme.data).materialTheme : _kFallbackTheme)));
        return ((ThemeData)(object?)ThemeData.localize(theme__5287, ((ThemeData)theme__5287).typography.geometryThemeFor(category__5054)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _wrapsWidgetThemes(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        global::Doroti.Framework.Widgets.DefaultSelectionStyle selectionStyle__5926 = ((global::Doroti.Framework.Widgets.DefaultSelectionStyle)(object?)DefaultSelectionStyle.of(context));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.IconTheme(data: ((ThemeData)this.data).iconTheme, child: new global::Doroti.Framework.Widgets.DefaultSelectionStyle(selectionColor: (((ThemeData)this.data).textSelectionTheme.selectionColor ?? ((global::Doroti.Framework.Widgets.DefaultSelectionStyle)selectionStyle__5926).selectionColor), cursorColor: (((ThemeData)this.data).textSelectionTheme.cursorColor ?? ((global::Doroti.Framework.Widgets.DefaultSelectionStyle)selectionStyle__5926).cursorColor), child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual CupertinoThemeData _inheritedCupertinoThemeData(global::Doroti.Framework.Widgets.BuildContext context)
    {
        InheritedCupertinoTheme? inheritedTheme__6401 = ((InheritedCupertinoTheme?)(object?)context.dependOnInheritedWidgetOfExactType<InheritedCupertinoTheme>());
        return ((inheritedTheme__6401?.theme.data ?? new MaterialBasedCupertinoThemeData(materialTheme: this.data))).resolveFrom(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Brightness brightnessOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _InheritedTheme__theme? inheritedTheme__7313 = ((_InheritedTheme__theme?)(object?)context.dependOnInheritedWidgetOfExactType<_InheritedTheme__theme>());
        return ((inheritedTheme__7313?.theme.data.brightness ?? (Brightness)MediaQuery.platformBrightnessOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Brightness? maybeBrightnessOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _InheritedTheme__theme? inheritedTheme__8172 = ((_InheritedTheme__theme?)(object?)context.dependOnInheritedWidgetOfExactType<_InheritedTheme__theme>());
        return ((inheritedTheme__8172?.theme.data.brightness ?? (Brightness)MediaQuery.maybePlatformBrightnessOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _InheritedTheme__theme(theme: this, child: new CupertinoTheme(data: _inheritedCupertinoThemeData(context), child: _wrapsWidgetThemes(context, this.child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ThemeData>("data", this.data, showName: false));
    }

}

internal class _InheritedTheme__theme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual Theme theme { get; private set; } = default!;

    internal _InheritedTheme__theme(Theme theme, global::Doroti.Framework.Widgets.Widget child) : base(child: child)
    {
        this.theme = theme;
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Theme(data: ((Theme)this.theme).data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => (!object.Equals(((Theme)this.theme).data, ((_InheritedTheme__theme)oldWidget).theme.data));
}

public class ThemeDataTween : global::Doroti.Framework.Animation.Tween<ThemeData>
{
    public ThemeDataTween(ThemeData? begin = null, ThemeData? end = null) : base(begin: begin, end: end)
    {
    }

    public override ThemeData lerp(double t) => ThemeData.lerp(this.begin!, this.end!, t);
}

public class AnimatedTheme : global::Doroti.Framework.Widgets.ImplicitlyAnimatedWidget
{
    public virtual ThemeData data { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    public AnimatedTheme(global::Doroti.Framework.Foundation.Key? key = null, ThemeData data = default!, global::Doroti.Framework.Animation.Curve curve = default!, Duration? duration = null, global::System.Action? onEnd = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, curve: curve ?? global::Doroti.Framework.Animation.Curves.linear, duration: duration ?? ThemeLibrary.kThemeAnimationDuration, onEnd: onEnd)
    {
        this.data = data;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AnimatedThemeState__theme());
}

internal class _AnimatedThemeState__theme : global::Doroti.Framework.Widgets.AnimatedWidgetBaseState<AnimatedTheme>
{
    internal virtual ThemeDataTween? _data { get; set; } = default;

    public override void forEachTween(global::System.Func<global::Doroti.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Framework.Animation.IDartTween>, global::Doroti.Framework.Animation.IDartTween?> visitor)
    {
        _data = ((ThemeDataTween?)(object?)visitor(this._data, ((AnimatedTheme)this.widget).data, ((value) => new ThemeDataTween(begin: ((ThemeData?)(object?)value)!)))!)!;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Theme(data: this._data!.evaluate(this.animation), child: ((AnimatedTheme)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ThemeDataTween>("data", this._data, showName: false, defaultValue: null));
    }

}
