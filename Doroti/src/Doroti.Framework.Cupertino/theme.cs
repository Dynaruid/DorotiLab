// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/theme.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public static partial class ThemeLibrary
{
    internal static _CupertinoThemeDefaults__theme _kDefaultTheme = new _CupertinoThemeDefaults__theme(null, CupertinoColors.systemBlue, CupertinoColors.white, CupertinoDynamicColor.CreateWithBrightness(color: new global::Doroti.Ui.Color(4042914297L), darkColor: new global::Doroti.Ui.Color(4028439837L)), CupertinoColors.systemBackground, CupertinoColors.systemBlue, false, new _CupertinoTextThemeDefaults__theme(CupertinoColors.label, CupertinoColors.inactiveGray));
}

public class CupertinoTheme : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual CupertinoThemeData data { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    public CupertinoTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, CupertinoThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.data = data;
        this.child = child;
    }

    public static CupertinoThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        InheritedCupertinoTheme? inheritedTheme__2778 = ((InheritedCupertinoTheme?)(object?)context.dependOnInheritedWidgetOfExactType<InheritedCupertinoTheme>());
        return ((CupertinoThemeData)(object?)((inheritedTheme__2778?.theme.data ?? new CupertinoThemeData())).resolveFrom(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Brightness brightnessOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        InheritedCupertinoTheme? inheritedTheme__3818 = ((InheritedCupertinoTheme?)(object?)context.dependOnInheritedWidgetOfExactType<InheritedCupertinoTheme>());
        return ((inheritedTheme__3818?.theme.data.brightness ?? (Brightness)MediaQuery.platformBrightnessOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Brightness? maybeBrightnessOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        InheritedCupertinoTheme? inheritedTheme__4825 = ((InheritedCupertinoTheme?)(object?)context.dependOnInheritedWidgetOfExactType<InheritedCupertinoTheme>());
        return ((inheritedTheme__4825?.theme.data.brightness ?? (Brightness)MediaQuery.maybePlatformBrightnessOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new InheritedCupertinoTheme(theme: this, child: new global::Doroti.Generated.Framework.Widgets.IconTheme(data: new CupertinoIconThemeData(color: ((CupertinoThemeData)this.data).primaryColor), child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        this.data.debugFillProperties(properties);
    }

}

public class InheritedCupertinoTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual CupertinoTheme theme { get; private set; } = default!;

    public InheritedCupertinoTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, CupertinoTheme theme = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.theme = theme;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoTheme(data: ((CupertinoTheme)this.theme).data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(((CupertinoTheme)this.theme).data, ((InheritedCupertinoTheme)oldWidget).theme.data)));
}

public class CupertinoThemeData : NoDefaultCupertinoThemeData, global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    internal virtual _CupertinoThemeDefaults__theme _defaults { get; private set; } = default!;

    public CupertinoThemeData(Brightness? brightness = null, Color? primaryColor = null, Color? primaryContrastingColor = null, CupertinoTextThemeData? textTheme = null, Color? barBackgroundColor = null, Color? scaffoldBackgroundColor = null, Color? selectionHandleColor = null, bool? applyThemeToAll = null) : this(brightness, primaryColor, primaryContrastingColor, textTheme, barBackgroundColor, scaffoldBackgroundColor, selectionHandleColor, applyThemeToAll, ThemeLibrary._kDefaultTheme)
    {
    }

    public static CupertinoThemeData CreateRaw(Brightness? brightness, Color? primaryColor, Color? primaryContrastingColor, CupertinoTextThemeData? textTheme, Color? barBackgroundColor, Color? scaffoldBackgroundColor, Color? selectionHandleColor, bool? applyThemeToAll)
    {
        return new CupertinoThemeData(brightness, primaryColor, primaryContrastingColor, textTheme, barBackgroundColor, scaffoldBackgroundColor, selectionHandleColor, applyThemeToAll, ThemeLibrary._kDefaultTheme);
    }

    internal CupertinoThemeData(Brightness? brightness, Color? primaryColor, Color? primaryContrastingColor, CupertinoTextThemeData? textTheme, Color? barBackgroundColor, Color? scaffoldBackgroundColor, Color? selectionHandleColor, bool? applyThemeToAll, _CupertinoThemeDefaults__theme _defaults) : base(brightness: brightness, primaryColor: primaryColor, primaryContrastingColor: primaryContrastingColor, textTheme: textTheme, barBackgroundColor: barBackgroundColor, scaffoldBackgroundColor: scaffoldBackgroundColor, selectionHandleColor: selectionHandleColor, applyThemeToAll: applyThemeToAll)
    {
        this._defaults = _defaults;
    }

    public override Color? primaryColor => DartRuntimePrimitives.ConvertValue<Color>((base.primaryColor ?? ((_CupertinoThemeDefaults__theme)this._defaults).primaryColor));
    public override Color? primaryContrastingColor => DartRuntimePrimitives.ConvertValue<Color>((base.primaryContrastingColor ?? ((_CupertinoThemeDefaults__theme)this._defaults).primaryContrastingColor));
    public override CupertinoTextThemeData? textTheme
    {
        get
        {
            return ((base.textTheme ?? (CupertinoTextThemeData)((_CupertinoThemeDefaults__theme)this._defaults).textThemeDefaults.createDefaults(primaryColor: this.primaryColor)));
            return default!;
        }
    }
    public override Color? barBackgroundColor => DartRuntimePrimitives.ConvertValue<Color>((base.barBackgroundColor ?? ((_CupertinoThemeDefaults__theme)this._defaults).barBackgroundColor));
    public override Color? scaffoldBackgroundColor => DartRuntimePrimitives.ConvertValue<Color>((base.scaffoldBackgroundColor ?? ((_CupertinoThemeDefaults__theme)this._defaults).scaffoldBackgroundColor));
    public override Color? selectionHandleColor => DartRuntimePrimitives.ConvertValue<Color>((base.selectionHandleColor ?? ((_CupertinoThemeDefaults__theme)this._defaults).selectionHandleColor));
    public override bool? applyThemeToAll => DartRuntimePrimitives.ConvertValue<bool>((base.applyThemeToAll ?? ((_CupertinoThemeDefaults__theme)this._defaults).applyThemeToAll));
    public override NoDefaultCupertinoThemeData noDefault()
    {
        return new NoDefaultCupertinoThemeData(brightness: base.brightness, primaryColor: base.primaryColor, primaryContrastingColor: base.primaryContrastingColor, textTheme: base.textTheme, barBackgroundColor: base.barBackgroundColor, scaffoldBackgroundColor: base.scaffoldBackgroundColor, selectionHandleColor: base.selectionHandleColor, applyThemeToAll: base.applyThemeToAll);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override CupertinoThemeData resolveFrom(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        Color? convertColor(Color? color)
        {
            return CupertinoDynamicColor.maybeResolve(color, context);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return new CupertinoThemeData(this.brightness, convertColor(base.primaryColor), convertColor(base.primaryContrastingColor), base.textTheme?.resolveFrom(context), convertColor(base.barBackgroundColor), convertColor(base.scaffoldBackgroundColor), convertColor(base.selectionHandleColor), this.applyThemeToAll, this._defaults.resolveFrom(context, (base.textTheme is null)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override CupertinoThemeData copyWith(Brightness? brightness = null, Color? primaryColor = null, Color? primaryContrastingColor = null, CupertinoTextThemeData? textTheme = null, Color? barBackgroundColor = null, Color? scaffoldBackgroundColor = null, Color? selectionHandleColor = null, bool? applyThemeToAll = null)
    {
        return new CupertinoThemeData((brightness ?? base.brightness), (primaryColor ?? base.primaryColor), (primaryContrastingColor ?? base.primaryContrastingColor), (textTheme ?? base.textTheme), (barBackgroundColor ?? base.barBackgroundColor), (scaffoldBackgroundColor ?? base.scaffoldBackgroundColor), (selectionHandleColor ?? base.selectionHandleColor), (applyThemeToAll ?? base.applyThemeToAll), this._defaults);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultData__12086 = new CupertinoThemeData();
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.Brightness>("brightness", this.brightness, defaultValue: null));
        properties.add(ColorsLibrary.createCupertinoColorProperty("primaryColor", this.primaryColor, defaultValue: ((CupertinoThemeData)defaultData__12086).primaryColor));
        properties.add(ColorsLibrary.createCupertinoColorProperty("primaryContrastingColor", this.primaryContrastingColor, defaultValue: ((CupertinoThemeData)defaultData__12086).primaryContrastingColor));
        properties.add(ColorsLibrary.createCupertinoColorProperty("barBackgroundColor", this.barBackgroundColor, defaultValue: ((CupertinoThemeData)defaultData__12086).barBackgroundColor));
        properties.add(ColorsLibrary.createCupertinoColorProperty("scaffoldBackgroundColor", this.scaffoldBackgroundColor, defaultValue: ((CupertinoThemeData)defaultData__12086).scaffoldBackgroundColor));
        properties.add(ColorsLibrary.createCupertinoColorProperty("selectionHandleColor", this.selectionHandleColor, defaultValue: ((CupertinoThemeData)defaultData__12086).selectionHandleColor));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("applyThemeToAll", this.applyThemeToAll, defaultValue: ((CupertinoThemeData)defaultData__12086).applyThemeToAll));
        this.textTheme.debugFillProperties(properties);
    }

    public override bool Equals(object? other)
    {
        var __other = other as CupertinoThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((__other is CupertinoThemeData) && (object.Equals(((CupertinoThemeData)__other).brightness, this.brightness))) && (object.Equals(((CupertinoThemeData)((CupertinoThemeData)__other)).primaryColor, this.primaryColor))) && (object.Equals(((CupertinoThemeData)((CupertinoThemeData)__other)).primaryContrastingColor, this.primaryContrastingColor))) && (object.Equals(((CupertinoThemeData)((CupertinoThemeData)__other)).textTheme, this.textTheme))) && (object.Equals(((CupertinoThemeData)((CupertinoThemeData)__other)).barBackgroundColor, this.barBackgroundColor))) && (object.Equals(((CupertinoThemeData)((CupertinoThemeData)__other)).scaffoldBackgroundColor, this.scaffoldBackgroundColor))) && (object.Equals(((CupertinoThemeData)((CupertinoThemeData)__other)).selectionHandleColor, this.selectionHandleColor))) && (((CupertinoThemeData)((CupertinoThemeData)__other)).applyThemeToAll == this.applyThemeToAll));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.brightness, this.primaryColor, this.primaryContrastingColor, this.textTheme, this.barBackgroundColor, this.scaffoldBackgroundColor, this.selectionHandleColor, this.applyThemeToAll));
    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class NoDefaultCupertinoThemeData
{
    public virtual Brightness? brightness { get; private set; }
    public virtual Color? primaryColor { get; private set; }
    public virtual Color? primaryContrastingColor { get; private set; }
    public virtual CupertinoTextThemeData? textTheme { get; private set; }
    public virtual Color? barBackgroundColor { get; private set; }
    public virtual Color? scaffoldBackgroundColor { get; private set; }
    public virtual Color? selectionHandleColor { get; private set; }
    public virtual bool? applyThemeToAll { get; private set; }

    public NoDefaultCupertinoThemeData(Brightness? brightness = null, Color? primaryColor = null, Color? primaryContrastingColor = null, CupertinoTextThemeData? textTheme = null, Color? barBackgroundColor = null, Color? scaffoldBackgroundColor = null, Color? selectionHandleColor = null, bool? applyThemeToAll = null)
    {
        this.brightness = brightness;
        this.primaryColor = primaryColor;
        this.primaryContrastingColor = primaryContrastingColor;
        this.textTheme = textTheme;
        this.barBackgroundColor = barBackgroundColor;
        this.scaffoldBackgroundColor = scaffoldBackgroundColor;
        this.selectionHandleColor = selectionHandleColor;
        this.applyThemeToAll = applyThemeToAll;
    }

    public virtual NoDefaultCupertinoThemeData noDefault() => this;
    public virtual NoDefaultCupertinoThemeData resolveFrom(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        Color? convertColor(Color? color)
        {
            return CupertinoDynamicColor.maybeResolve(color, context);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return new NoDefaultCupertinoThemeData(brightness: this.brightness, primaryColor: convertColor(this.primaryColor), primaryContrastingColor: convertColor(this.primaryContrastingColor), textTheme: this.textTheme?.resolveFrom(context), barBackgroundColor: convertColor(this.barBackgroundColor), scaffoldBackgroundColor: convertColor(this.scaffoldBackgroundColor), selectionHandleColor: convertColor(this.selectionHandleColor), applyThemeToAll: this.applyThemeToAll);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual NoDefaultCupertinoThemeData copyWith(Brightness? brightness = null, Color? primaryColor = null, Color? primaryContrastingColor = null, CupertinoTextThemeData? textTheme = null, Color? barBackgroundColor = null, Color? scaffoldBackgroundColor = null, Color? selectionHandleColor = null, bool? applyThemeToAll = null)
    {
        return new NoDefaultCupertinoThemeData(brightness: (brightness ?? this.brightness), primaryColor: (primaryColor ?? this.primaryColor), primaryContrastingColor: (primaryContrastingColor ?? this.primaryContrastingColor), textTheme: (textTheme ?? this.textTheme), barBackgroundColor: (barBackgroundColor ?? this.barBackgroundColor), scaffoldBackgroundColor: (scaffoldBackgroundColor ?? this.scaffoldBackgroundColor), selectionHandleColor: (selectionHandleColor ?? this.selectionHandleColor), applyThemeToAll: (applyThemeToAll ?? this.applyThemeToAll));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as NoDefaultCupertinoThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((__other is NoDefaultCupertinoThemeData) && (object.Equals(((NoDefaultCupertinoThemeData)((NoDefaultCupertinoThemeData)__other)).brightness, this.brightness))) && (object.Equals(((NoDefaultCupertinoThemeData)((NoDefaultCupertinoThemeData)__other)).primaryColor, this.primaryColor))) && (object.Equals(((NoDefaultCupertinoThemeData)((NoDefaultCupertinoThemeData)__other)).primaryContrastingColor, this.primaryContrastingColor))) && (object.Equals(((NoDefaultCupertinoThemeData)((NoDefaultCupertinoThemeData)__other)).textTheme, this.textTheme))) && (object.Equals(((NoDefaultCupertinoThemeData)((NoDefaultCupertinoThemeData)__other)).barBackgroundColor, this.barBackgroundColor))) && (object.Equals(((NoDefaultCupertinoThemeData)((NoDefaultCupertinoThemeData)__other)).scaffoldBackgroundColor, this.scaffoldBackgroundColor))) && (((NoDefaultCupertinoThemeData)((NoDefaultCupertinoThemeData)__other)).applyThemeToAll == this.applyThemeToAll));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.brightness, this.primaryColor, this.primaryContrastingColor, this.textTheme, this.barBackgroundColor, this.scaffoldBackgroundColor, this.applyThemeToAll));
}

internal class _CupertinoThemeDefaults__theme
{
    public virtual Brightness? brightness { get; private set; }
    public virtual Color primaryColor { get; private set; } = default!;
    public virtual Color primaryContrastingColor { get; private set; } = default!;
    public virtual Color barBackgroundColor { get; private set; } = default!;
    public virtual Color scaffoldBackgroundColor { get; private set; } = default!;
    public virtual Color selectionHandleColor { get; private set; } = default!;
    public virtual bool applyThemeToAll { get; private set; } = default!;
    public virtual _CupertinoTextThemeDefaults__theme textThemeDefaults { get; private set; } = default!;

    internal _CupertinoThemeDefaults__theme(Brightness? brightness, Color primaryColor, Color primaryContrastingColor, Color barBackgroundColor, Color scaffoldBackgroundColor, Color selectionHandleColor, bool applyThemeToAll, _CupertinoTextThemeDefaults__theme textThemeDefaults)
    {
        this.brightness = brightness;
        this.primaryColor = primaryColor;
        this.primaryContrastingColor = primaryContrastingColor;
        this.barBackgroundColor = barBackgroundColor;
        this.scaffoldBackgroundColor = scaffoldBackgroundColor;
        this.selectionHandleColor = selectionHandleColor;
        this.applyThemeToAll = applyThemeToAll;
        this.textThemeDefaults = textThemeDefaults;
    }

    public virtual _CupertinoThemeDefaults__theme resolveFrom(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool resolveTextTheme)
    {
        Color convertColor(Color color)
        {
            return CupertinoDynamicColor.resolve(color, context);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return new _CupertinoThemeDefaults__theme(this.brightness, convertColor(this.primaryColor), convertColor(this.primaryContrastingColor), convertColor(this.barBackgroundColor), convertColor(this.scaffoldBackgroundColor), convertColor(this.selectionHandleColor), this.applyThemeToAll, (resolveTextTheme ? this.textThemeDefaults.resolveFrom(context) : this.textThemeDefaults));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _CupertinoTextThemeDefaults__theme
{
    public virtual Color labelColor { get; private set; } = default!;
    public virtual Color inactiveGray { get; private set; } = default!;

    internal _CupertinoTextThemeDefaults__theme(Color labelColor, Color inactiveGray)
    {
        this.labelColor = labelColor;
        this.inactiveGray = inactiveGray;
    }

    public virtual _CupertinoTextThemeDefaults__theme resolveFrom(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return new _CupertinoTextThemeDefaults__theme(CupertinoDynamicColor.resolve(this.labelColor, context), CupertinoDynamicColor.resolve(this.inactiveGray, context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual CupertinoTextThemeData createDefaults(Color primaryColor)
    {
        return ((CupertinoTextThemeData)(object?)new _DefaultCupertinoTextThemeData__theme(primaryColor: primaryColor, labelColor: this.labelColor, inactiveGray: this.inactiveGray));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DefaultCupertinoTextThemeData__theme : CupertinoTextThemeData
{
    public virtual Color labelColor { get; private set; } = default!;
    public virtual Color inactiveGray { get; private set; } = default!;

    internal _DefaultCupertinoTextThemeData__theme(Color labelColor, Color inactiveGray, Color primaryColor) : base(primaryColor: primaryColor)
    {
        this.labelColor = labelColor;
        this.inactiveGray = inactiveGray;
    }

    public override global::Doroti.Generated.Framework.Painting.TextStyle textStyle => base.textStyle.copyWith(color: this.labelColor);
    public override global::Doroti.Generated.Framework.Painting.TextStyle tabLabelTextStyle => base.tabLabelTextStyle.copyWith(color: this.inactiveGray);
    public override global::Doroti.Generated.Framework.Painting.TextStyle navTitleTextStyle => base.navTitleTextStyle.copyWith(color: this.labelColor);
    public override global::Doroti.Generated.Framework.Painting.TextStyle navLargeTitleTextStyle => base.navLargeTitleTextStyle.copyWith(color: this.labelColor);
    public override global::Doroti.Generated.Framework.Painting.TextStyle pickerTextStyle => base.pickerTextStyle.copyWith(color: this.labelColor);
    public override global::Doroti.Generated.Framework.Painting.TextStyle dateTimePickerTextStyle => base.dateTimePickerTextStyle.copyWith(color: this.labelColor);
}
